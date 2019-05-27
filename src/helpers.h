#ifndef HELPERS_H
#define HELPERS_H

#include <QString>
#include <QStringLiteral>

class Helpers {

    public:
        static QString getFSNameFromTitle(QString title);
        static QString getTitleFromFSName(QString fileName);

    private:
        static bool isValidTitleChar(QChar ch);

};

#endif // HELPERS_H
