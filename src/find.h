#ifndef FIND_H
#define FIND_H

#include <QString>
#include "storage.h"

class Find {

    public:
        static void setup(QString text);
        static bool findNext(FileItem* selectedFile);
        static QString lastText();
        static bool hasText();

    private:
        static QString _text;

};

#endif // FIND_H
