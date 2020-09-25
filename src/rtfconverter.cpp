#include <QDir>
#include <QFile>
#include <QFileInfo>
#include <QTextCodec>
#include <QTextStream>
#include "rtfconverter.h"

bool RtfConverter::convertFile(QString filePath) {
    QFile file(filePath);
    QFileInfo fileInfo(filePath);
    QString newFilePath = QDir::cleanPath(fileInfo.absolutePath() + "/" + fileInfo.completeBaseName() + ".html" );

    if (file.open(QIODevice::ReadOnly)) {
        QByteArray bytes = file.readAll();
        QBuffer buffer(&bytes);
        buffer.open(QIODevice::ReadOnly);

        QChar firstChar = nextChar(buffer);
        if (firstChar == '{') {
            Context context;
            if (processGroup(buffer, context, 1)) {
                file.close();

                QFile newFile(newFilePath);
                if (newFile.open(QIODevice::WriteOnly)) {
                    QTextStream out(&newFile);
                    out.setCodec(QTextCodec::codecForName("UTF-8"));
                    out.setAutoDetectUnicode(true);
                    out << "<html><head></head><body>";
                    out << context.htmlOutput;
                    out << "</body>";
                    out.flush();
                    newFile.close();

#ifndef QT_DEBUG
                    //remove old file
                    file.rename(filePath + ".bak");
#endif

                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        } else {
            return false;
        }
    } else {
        return false;
    }
}

QChar RtfConverter::nextChar(QBuffer& buffer) {
    char ch[1];
    if (buffer.read(ch, 1) == 1) {
        return QChar(ch[0]);
    } else {
        return QChar(0);
    }
}

bool RtfConverter::processGroup(QBuffer& buffer, Context& context, int level) {
    State state = State::Text;

    QString currCommand;
    QByteArray currTextBytes;
    int currHexEscape = 0;

    while (true) {
        if (buffer.atEnd()) { return false; }

        auto ch = nextChar(buffer);
        if (ch == '\0') { return true; } //assume we're done

        switch (state) {
            case State::Escape:
                if (currCommand.length() > 0) { processCommand(currCommand, context, level); }
                if (ch.isLetter()) {
                    if (currTextBytes.length() > 0) { processText(currTextBytes, context, level); }
                    currCommand = ch;
                    state = State::Command;
                } else if (ch == '\'') {
                    currHexEscape = 0;
                    state = State::EscapeHex1;
                } else if (ch == '*') { //ignore
                } else if (ch == '~') { //non-breaking space - replace with normal space
                    if (currTextBytes.length() > 0) { processText(currTextBytes, context, level); } //process all text up to now
                    context.htmlOutput += " ";
                } else if (ch == '-') { //ignore optional hyphen
                } else if (ch == '_') { //ignore non-breaking hyphen
                } else {
                    currTextBytes.append(ch.toLatin1());
                    state = State::Text;
                }
                break;

            case State::EscapeHex1: {
                    bool ok;
                    int value = QString(ch).toInt(&ok, 16);
                    currHexEscape = ok ? value : -1;
                    state = State::EscapeHex2;
                } break;

            case State::EscapeHex2: {
                    bool ok;
                    int value = QString(ch).toInt(&ok, 16);
                    currHexEscape = (ok && (currHexEscape >= 0)) ? (currHexEscape << 4) | value : -1;
                    currTextBytes.append((char)currHexEscape);
                    state = State::Text;
                } break;

            default:
                if (ch == '{') { //new group
                    if (currCommand.length() > 0) { processCommand(currCommand, context, level); }
                    if (currTextBytes.length() > 0) { processText(currTextBytes, context, level); }
                    processGroup(buffer, context, level + 1);
                } else if (ch == '}') { //done with group
                    if (currCommand.length() > 0) { processCommand(currCommand, context, level); }
                    if (currTextBytes.length() > 0) { processText(currTextBytes, context, level); }
                    return true;
                } else if (ch == '\\') { //escape
                    state = State::Escape;
                } else {
                    switch (state) {
                        case State::Command:
                            if ((ch == ' ') || (ch == 10) || (ch == 13)) {
                                if (currCommand.length() > 0) { processCommand(currCommand, context, level); }
                                if (currTextBytes.length() > 0) { processText(currTextBytes, context, level); }
                                state = State::Text;
                            } else {
                                currCommand += ch;
                            }
                            break;

                        case State::Text:
                            currTextBytes.append(ch.toLatin1());
                            break;

                        default: break;
                    }
                }
                break;

        }
    }

    return false;
}

void RtfConverter::processCommand(QString& command, Context& context, int& level) {
    if (level == 1) { //ignore details - we are just interested in basic formatting
        if (command == "ansi" ) {
            context.codePage = 0;
        } else if (command.startsWith("ansicpg")) {
            context.codePage = command.remove(0, 7).toInt();
        } else if (command.startsWith("deflang")) {
            int locale = command.remove(0, 7).toInt();
            context.defaultCodePage = codePageFromLocale(locale, 0);
        } else if (command.startsWith("lang")) {
            int locale = command.remove(0, 4).toInt();
            context.codePage = codePageFromLocale(locale, context.defaultCodePage);
        } else if (command == "par") {
            context.htmlOutput += "<br/>"; //proper paragraph handling results in too much spacing
        } else if (command == "line") {
            context.htmlOutput += "<br/>";
        } else if (command == "b") {
            context.htmlOutput += "<b>";
        } else if (command == "b0") {
            context.htmlOutput += "</b>";
        } else if (command == "i") {
            context.htmlOutput += "<i>";
        } else if (command == "i0") {
            context.htmlOutput += "</i>";
        } else if (command == "ul") {
            context.htmlOutput += "<u>";
        } else if (command == "ulnone") {
            context.htmlOutput += "</u>";
        } else if (command == "sub") {
            context.htmlOutput += "<sub>"; context.fontSub = true;
        } else if (command == "super") {
            context.htmlOutput += "<sup>"; context.fontSuper = true;
        } else if (command == "nosupersub") {
            if (context.fontSub) {
                context.htmlOutput += "</sub>";
            } else if (context.fontSuper) {
                context.htmlOutput += "</sup>";
            }
        } else if (command == "strike") {
            context.htmlOutput += "<s>";
        } else if (command == "strike0") {
            context.htmlOutput += "</s>";
        } else if (command == "tab") {
            context.htmlOutput += "\t";
        }
    } else {
        if (command == "pntext") { //bullets are in a separate group
            level = 1; //temporarily pretend we're at level 1
        }
    }
    command.clear();
}

void RtfConverter::processText(QByteArray& bytes, Context& context, int level) {
    if (level == 1) { //ignore details - we are just interested in basic formatting
        QString text;
        switch (context.codePage) {
            case 0: {
                    text = QString::fromLatin1(bytes);
                } break;

            case 1250: case 1251: case 1252: case 1253: case 1254: case 1255: case 1256: case 1257: case 1258: {
                    QString codePage = "Windows-" + QString::number(context.codePage);
                    QTextCodec* codec = QTextCodec::codecForName(codePage.toLatin1());
                    if (codec != nullptr) {
                        QTextDecoder* decoder = codec->makeDecoder();
                        if (decoder != nullptr) {
                            text = decoder->toUnicode(bytes, bytes.length());
                            delete decoder;
                        }
                    }
                } break;

            default:
                break;
        }

        if (text.length() > 0) {
            context.htmlOutput += text;
        }
    }
    bytes.clear();
}

int RtfConverter::codePageFromLocale(int locale, int defaultCodePage) {
    switch (locale) {
        case 9: return 1252; //English
        case 1025: return 1256; //Arabic - Saudi Arabia
        case 1026: return 1251; //Bulgarian
        case 1027: return 1252; //Catalan
        case 1029: return 1250; //Czech
        case 1030: return 1252; //Danish
        case 1031: return 1252; //German - Germany
        case 1032: return 1253; //Greek
        case 1033: return 1252; //English - United States
        case 1034: return 1252; //Spanish - Spain (Traditional)
        case 1035: return 1252; //Finnish
        case 1036: return 1252; //French - France
        case 1037: return 1255; //Hebrew
        case 1038: return 1250; //Hungarian
        case 1039: return 1252; //Icelandic
        case 1040: return 1252; //Italian - Italy
        case 1043: return 1252; //Dutch - Netherlands
        case 1044: return 1252; //Norwegian - Bokml
        case 1045: return 1250; //Polish
        case 1046: return 1252; //Portuguese - Brazil
        case 1048: return 1250; //Romanian - Romania
        case 1049: return 1251; //Russian
        case 1050: return 1250; //Croatian
        case 1051: return 1250; //Slovak
        case 1052: return 1250; //Albanian
        case 1053: return 1252; //Swedish - Sweden
        case 1055: return 1254; //Turkish
        case 1056: return 1256; //Urdu
        case 1057: return 1252; //Indonesian
        case 1058: return 1251; //Ukrainian
        case 1059: return 1251; //Belarusian
        case 1060: return 1250; //Slovenian
        case 1061: return 1257; //Estonian
        case 1062: return 1257; //Latvian
        case 1063: return 1257; //Lithuanian
        case 1065: return 1256; //Farsi - Persian
        case 1066: return 1258; //Vietnamese
        case 1068: return 1254; //Azeri - Latin
        case 1069: return 1252; //Basque
        case 1071: return 1251; //FYRO Macedonia
        case 1078: return 1252; //Afrikaans
        case 1080: return 1252; //Faroese
        case 1086: return 1252; //Malay - Malaysia
        case 1087: return 1251; //Kazakh
        case 1088: return 1251; //Kyrgyz - Cyrillic
        case 1089: return 1252; //Swahili
        case 1091: return 1254; //Uzbek - Latin
        case 1092: return 1251; //Tatar
        case 1104: return 1251; //Mongolian
        case 1110: return 1252; //Galician
        case 2049: return 1256; //Arabic - Iraq
        case 2055: return 1252; //German - Switzerland
        case 2057: return 1252; //English - Great Britain
        case 2058: return 1252; //Spanish - Mexico
        case 2060: return 1252; //French - Belgium
        case 2064: return 1252; //Italian - Switzerland
        case 2067: return 1252; //Dutch - Belgium
        case 2068: return 1252; //Norwegian - Nynorsk
        case 2070: return 1252; //Portuguese - Portugal
        case 2074: return 1250; //Serbian - Latin
        case 2077: return 1252; //Swedish - Finland
        case 2092: return 1251; //Azeri - Cyrillic
        case 2110: return 1252; //Malay - Brunei
        case 2115: return 1251; //Uzbek - Cyrillic
        case 3073: return 1256; //Arabic - Egypt
        case 3079: return 1252; //German - Austria
        case 3081: return 1252; //English - Australia
        case 3084: return 1252; //French - Canada
        case 3098: return 1251; //Serbian - Cyrillic
        case 4097: return 1256; //Arabic - Libya
        case 4103: return 1252; //German - Luxembourg
        case 4105: return 1252; //English - Canada
        case 4106: return 1252; //Spanish - Guatemala
        case 4108: return 1252; //French - Switzerland
        case 5121: return 1256; //Arabic - Algeria
        case 5127: return 1252; //German - Liechtenstein
        case 5129: return 1252; //English - New Zealand
        case 5130: return 1252; //Spanish - Costa Rica
        case 5132: return 1252; //French - Luxembourg
        case 6145: return 1256; //Arabic - Morocco
        case 6153: return 1252; //English - Ireland
        case 6154: return 1252; //Spanish - Panama
        case 6156: return 1252; //French - Monaco
        case 7169: return 1256; //Arabic - Tunisia
        case 7177: return 1252; //English - Southern Africa
        case 7178: return 1252; //Spanish - Dominican Republic
        case 8193: return 1256; //Arabic - Oman
        case 8201: return 1252; //English - Jamaica
        case 8202: return 1252; //Spanish - Venezuela
        case 9217: return 1256; //Arabic - Yemen
        case 9225: return 1252; //English - Caribbean
        case 9226: return 1252; //Spanish - Colombia
        case 10241: return 1256; //Arabic - Syria
        case 10249: return 1252; //English - Belize
        case 10250: return 1252; //Spanish - Peru
        case 11265: return 1256; //Arabic - Jordan
        case 11273: return 1252; //English - Trinidad
        case 11274: return 1252; //Spanish - Argentina
        case 12289: return 1256; //Arabic - Lebanon
        case 12297: return 1252; //English - Zimbabwe
        case 12298: return 1252; //Spanish - Ecuador
        case 13313: return 1256; //Arabic - Kuwait
        case 13321: return 1252; //English - Philippines
        case 13322: return 1252; //Spanish - Chile
        case 14337: return 1256; //Arabic - United Arab Emirates
        case 14346: return 1252; //Spanish - Uruguay
        case 15361: return 1256; //Arabic - Bahrain
        case 15370: return 1252; //Spanish - Paraguay
        case 16385: return 1256; //Arabic - Qatar
        case 16394: return 1252; //Spanish - Bolivia
        case 17418: return 1252; //Spanish - El Salvador
        case 18442: return 1252; //Spanish - Honduras
        case 19466: return 1252; //Spanish - Nicaragua
        case 20490: return 1252; //Spanish - Puerto Rico
    }
    return defaultCodePage;
}
