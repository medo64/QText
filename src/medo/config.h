/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

// 2019-07-17: Initial version
// 2019-10-13: Added stateRead and stateWrite operations
// 2019-11-01: Fixed readMany implementation
// 2019-11-17: Added stateReadMany and stateWriteMany
//             Added option to set paths manually
// 2020-03-15: If QApplication hasn't bee initializesd, assume installed on Linux

#ifndef CONFIG_H
#define CONFIG_H

#include <QHash>
#include <QMutex>
#include <QString>
#include <QStringList>
#include <QVariant>
#include <QVector>

namespace Medo { class Config; }

class Config {

    public:

        /*! Resets configuration. This includes cached data. */
        static void reset();

        /*! Forces reload of config file. Returns true if file exists. */
        static bool load();

        /*! Forces immediate save of a config file. Returns true if operation was successful. */
        static bool save();


        /*! Returns if application is considered portable. Initial value will be auto-detected.
        * Auto-detection:
        *   Linux: It's assumed portable if config file is in the same directory or if executable's directory is not in one of the bin directories (~/bin, ~/.local/bin, /opt, /bin, /usr/bin, or /usr/local/bin)
        *   Windows: It's assumed portable if config file is in current directory or if executable's directory is not C:\Program Files\. */
        static bool isPortable();

        /*! Sets if application will be considered portable. Invalidates configuration file and data directory path cache.
         * \param isPortable If true, application will be considered portable. */
        static void setPortable(bool portable);


        /*! Returns if write to config will perform automatic save. Default is true. */
        static bool immediateSave();

        /*! Sets if write to config will perform automatic save.
         * \param saveImmediately If true, any write will result in immediate save. */
        static void setImmediateSave(bool saveImmediately);


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

        /*! Sets configuration file path. No verification is made in regards to path suitability for purpose.
         * \param configurationFilePath Full path to the configuration file. */
        static void setConfigurationFilePath(QString configurationFilePath);


        /*! Returns state file path. If file doesn't exist, it's created. Returned value is cached. */
        static QString stateFile();

        /*! Returns state file path. Returned value is cached.
         * When installed:
         *   Linux: File is saved in config directory (e.g. ~/.config/<appname>.user).
         *   Windows: File is saved under application data path (e.g. C:/Users/<UserName>/AppData/Roaming/<OrgName>/<AppName>/<AppName>.user).
         * When iportable (either not installed or config file exists):
         *   Linux: Config file under current directory is used (e.g. ./.<appname>.user).
         *   Windows: Config file under current directory is used (e.g. ./<AppName>.user). */
        static QString stateFilePath();

        /*! Sets state file path. No verification is made in regards to path suitability for purpose.
         * \param stateFilePath Full path to the state file. */
        static void setStateFilePath(QString stateFilePath);


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

        /*! Sets data directory path. No verification is made in regards to path suitability for purpose.
         * \param dataDirectoryPath Full path to the data directory. */
        static void setDataDirectoryPath(QString dataDirectoryPath);


        /*! Returns value for a given key or null QString if one is not found.
         * /param key Key. */
        static QString read(QString key);

        /*! Returns value for a given key or default value if one is not found.
         * /param key Key.
         * /param defaultValue Default value. */
        static QString read(QString key, QString defaultValue);

        /*! Writes value to a given key.
         * /param key Key.
         * /param value Value. */
        static void write(QString key, QString value);

        /*! Returns value for a given key or default value if one is not found.
         * /param key Key.
         * /param defaultValue Default value. */
        static QString read(QString key, const char* defaultValue);

        /*! Writes value to a given key.
         * /param key Key.
         * /param value Value. */
        static void write(QString key, const char* value);

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
        static long long read(QString key, long long defaultValue);

        /*! Writes value to a given key.
         * /param key Key.
         * /param value Value. */
        static void write(QString key, long long value);

        /*! Returns value for a given key or default value if one is not found or cannot be converted to double.
         * /param key Key.
         * /param defaultValue Default value. */
        static double read(QString key, double defaultValue);

        /*! Writes value to a given key.
         * /param key Key.
         * /param value Value. */
        static void write(QString key, double value);


        /*! Returns all values for a given key or empty list if key doesn't exist.
         * /param key Key. */
        static QStringList readMany(QString key);

        /*! Returns all values for a given key or default values if key doesn't exist.
         * /param key Key.
         * /param defaultValues Default values. */
        static QStringList readMany(QString key, QStringList defaultValues);

        /*! Writes values to a given key.
         * /param key Key.
         * /param values Values. */
        static void writeMany(QString key, QStringList values);


        /*! Remove all values of a given key.
         * /param key Key. */
        static void remove(QString key);

        /*! Remove all values. */
        static void removeAll();


        /*! Returns state value for a given key or default value if one is not found.
         * /param key Key.
         * /param defaultValue Default value. */
        static QString stateRead(QString key, QString defaultValue);

        /*! Writes state value to a given key.
         * /param key Key.
         * /param value Value. */
        static void stateWrite(QString key, QString value);

        /*! Returns state value for a given key or default value if one is not found.
         * /param key Key.
         * /param defaultValue Default value. */
        static QString stateRead(QString key, const char* defaultValue);

        /*! Writes state value to a given key.
         * /param key Key.
         * /param value Value. */
        static void stateWrite(QString key, const char* value);

        /*! Returns state value for a given key or default value if one is not found or cannot be converted to bool.
         * /param key Key.
         * /param defaultValue Default value. */
        static bool stateRead(QString key, bool defaultValue);

        /*! Writes state value to a given key.
         * /param key Key.
         * /param value Value. */
        static void stateWrite(QString key, bool value);


        /*! Returns all state values for a given key or empty list if key doesn't exist.
         * /param key Key. */
        static QStringList stateReadMany(QString key);

        /*! Returns all state values for a given key or default values if key doesn't exist.
         * /param key Key.
         * /param defaultValues Default values. */
        static QStringList stateReadMany(QString key, QStringList defaultValues);

        /*! Writes state values to a given key.
         * /param key Key.
         * /param values Values. */
        static void stateWriteMany(QString key, QStringList values);


    private:
        typedef enum {
           UNKNOWN  = -1,
           FALSE    = 0,
           TRUE     = 1,
        } PortableStatus;

    private:
        static QMutex _publicAccessMutex; //to ensure multi-threaded access works without conflict
        static QString _configurationFilePath;
        static QString _stateFilePath;
        static QString _dataDirectoryPath;
        static PortableStatus _isPortable;
        static bool _immediateSave;
        static QString configurationFilePathWhenPortable();
        static QString configurationFilePathWhenInstalled();
        static QString stateFilePathWhenPortable();
        static QString stateFilePathWhenInstalled();
        static QString dataDirectoryPathWhenPortable();
        static QString dataDirectoryPathWhenInstalled();

    private:
        class ConfigFile {
            public:
                ConfigFile(QString filePath);
                bool save();
                QString readOne(QString key);
                QStringList readMany(QString key);
                void writeOne(QString key, QString value);
                void writeMany(QString key, QStringList value);
                void removeMany(QString key);
                void removeAll();

            private:
                QMutex _cacheMutex;
                QHash<QString, QVariant> _cache;

            private:
                typedef enum {
                    Default,
                    Comment,
                    Key,
                    KeyEscape,
                    KeyEscapeLong,
                    SeparatorOrValue,
                    ValueOrWhitespace,
                    Value,
                    ValueEscape,
                    ValueEscapeLong,
                    ValueOrComment,
                } ProcessState;

            private:
                class LineData {

                    public:
                        LineData();
                        LineData(LineData* lineTemplate, QString key, QString value);
                        LineData(QString key, QString separatorPrefix, QString separator, QString separatorSuffix, QString value, QString commentPrefix, QString comment);

                    public:
                        QString getKey();
                        QString getValue();
                        void setValue(QString newValue);
                        QString toString();
                        static void escapeIntoStringBuilder(QString* sb, QString text, bool isKey = false);
                        bool isEmpty();

                    private:
                        QString _key;
                        QString _separatorPrefix;
                        QString _separator;
                        QString _separatorSuffix;
                        QString _value;
                        QString _commentPrefix;
                        QString _comment;
                };

            private:
                void processLine(QString line);
                QVector<LineData> _lines;
                bool _fileLoaded;
                QString _filePath;
                QString _lineEnding;

        };

    private:
        static ConfigFile* getConfigFile();
        static void resetConfigFile();
        static ConfigFile* _configFile;
        static ConfigFile* getStateFile();
        static void resetStateFile();
        static ConfigFile* _stateFile;

};

#endif // CONFIG_H
