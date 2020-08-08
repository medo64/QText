#pragma once

#include <QString>

class PhoneticAlphabet {

    public:
        static QString getNatoText(QString character);

    private:
        static QString getNonAlphanumericCharacterText(QChar character);

};
