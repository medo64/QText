#ifndef CONFIG_H
#define CONFIG_H

#include <QString>

class Config {

    public:
        static void reset();
        static QString getConfigurationFile();
        static QString getDataDirectory();

        static QString read(QString key, QString defaultValue);
        static void write(QString key, QString value);

    private:
        static QString _configurationFile;
        static QString _dataDirectory;

};

#endif // CONFIG_H
