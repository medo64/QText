#pragma once

#include <QBuffer>
#include <QString>

class RtfConverter {

    public:
        static bool convertFile(QString filePath);

    private:
        enum class State {
            Text,
            Group,
            Command,
            Escape,
            EscapeHex1,
            EscapeHex2,
        };
        struct Context {
            int defaultCodePage = 0;
            int codePage = 0;
            bool fontBold = false;
            bool fontItalic = false;
            bool fontUnderline = false;
            bool fontStrike = false;
            bool fontSub = false;
            bool fontSuper = false;
            QString htmlOutput;
        };

    private:
        static QChar nextChar(QBuffer& buffer);
        static bool processGroup(QBuffer& buffer, Context& context, int level);
        static void processCommand(QString& command, Context& context, int& level);
        static void processText(QByteArray& bytes, Context& context, int level);
        static int codePageFromLocale(int locale, int defaultCodePage);

};
