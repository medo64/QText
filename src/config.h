#ifndef CONFIG_H
#define CONFIG_H

#include <QString>

class Config {

    public:

        /*! Returns if application is considered portable. Initial value will be auto-detected.
        * Auto-detection:
        *   Linux: It's assumed portable if config file is in the same directory or if executable's directory is not in one of the bin directories (~/bin, ~/.local/bin, /opt, /bin, /usr/bin, or /usr/local/bin)
        *   Windows: It's assumed portable if config file is in current directory or if executable's directory is not C:\Program Files\. */
        static bool isPortable();

        /*! Sets if application will be considered portable. Invalidates configuration file and data directory path cache.
         * \param isPortable If true, application will be considered portable. */
        static void setPortable(bool portable);


        /*! Returns configuration file path. If file doesn't exist, it's created. Returned value is cached. */
        static QString configurationFile();

        /*! Returns configuration file path. Returned value is cached.
         * When installed:
         *   Linux: File is saved in config directory (e.g. ~/.config/<appname>.conf).
         *   Windows: File is saved under application data path (e.g. C:/Users/<UserName>/AppData/Roaming/<OrgName>/<AppName>/<AppName>.cfg).
         * When iportable (either not installed or config file exists):
         *   Linux: Config file under current directory is used (e.g. ./.<appname>).
         *   Windows: Config file under current directory is used (e.g. ./<AppName>.cfg). */
        static QString configurationFilePath();


        /*! Returns data directory path. If directory doesn't exist, it's created. Returned value is cached. */
        static QString dataDirectory();

        /*! Returns data directory path. Returned value is cached.
         * When installed:
         *   Linux: Data will be saved in local share directory (e.g. ~/.local/share/<appname>).
         *   Windows: Data subdirectory under application data path will be used (e.g. C:/Users/<UserName>/AppData/Roaming/<OrgName>/<AppName>/Data/).
         * When portable (either not installed or config file exists):
         *   Linux: Subdirectory is used (e.g. ./.<appname>.data).
         *   Windows: Subdirectory is used (e.g. ./<AppName>.Data/). */
        static QString dataDirectoryPath();


        /*! Returns all values for a given key or empty list if key doesn't exist.
         * /param key Key. */
        static QStringList read(QString key);

        /*! Writes values to a given key.
         * /param key Key.
         * /param values Values. */
        static void write(QString key, QStringList values);


        /*! Returns value for a given key or default value if one is not found.
         * /param key Key.
         * /param defaultValue Default value. */
        static QString read(QString key, const char* defaultValue);

        /*! Writes value to a given key.
         * /param key Key.
         * /param value Value. */
        static void write(QString key, const char* value);


        /*! Returns value for a given key or default value if one is not found.
         * /param key Key.
         * /param defaultValue Default value. */
        static QString read(QString key, QString defaultValue);

        /*! Writes value to a given key.
         * /param key Key.
         * /param value Value. */
        static void write(QString key, QString value);


        /*! Returns value for a given key or default value if one is not found or cannot be converted to bool.
         * /param key Key.
         * /param defaultValue Default value. */
        static bool read(QString key, bool defaultValue);

        /*! Writes value to a given key.
         * /param key Key.
         * /param value Value. */
        static void write(QString key, bool value);


        /*! Returns value for a given key or default value if one is not found or cannot be converted to int.
         * /param key Key.
         * /param defaultValue Default value. */
        static int read(QString key, int defaultValue);

        /*! Writes value to a given key.
         * /param key Key.
         * /param value Value. */
        static void write(QString key, int value);


        /*! Returns value for a given key or default value if one is not found or cannot be converted to long.
         * /param key Key.
         * /param defaultValue Default value. */
        static long read(QString key, long defaultValue);

        /*! Writes value to a given key.
         * /param key Key.
         * /param value Value. */
        static void write(QString key, long value);


        /*! Returns value for a given key or default value if one is not found or cannot be converted to double.
         * /param key Key.
         * /param defaultValue Default value. */
        static double read(QString key, double defaultValue);

        /*! Writes value to a given key.
         * /param key Key.
         * /param value Value. */
        static void write(QString key, double value);


    private:
        typedef enum {
           UNKNOWN  = -1,
           FALSE    = 0,
           TRUE     = 1,
        } PortableStatus;

    private:
        static QString _configurationFilePath;
        static QString _dataDirectoryPath;
        static PortableStatus _isPortable;
        static QString configurationFilePathWhenPortable();
        static QString configurationFilePathWhenInstalled();
        static QString dataDirectoryPathWhenPortable();
        static QString dataDirectoryPathWhenInstalled();

};

#endif // CONFIG_H
