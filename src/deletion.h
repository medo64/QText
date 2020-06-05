#pragma once

#include <QString>

class Deletion {

    public:
        static bool deleteFile(QString path);
        static bool overwriteFile(QString path);
        static bool recycleFile(QString path);

};
