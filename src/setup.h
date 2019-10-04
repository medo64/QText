#ifndef SETUP_H
#define SETUP_H

class Setup {

    public:
        static bool autostart();
        static void setAutostart(bool newAutostart);

    private:
        static void nativeAutostartAdd();
        static void nativeAutostartRemove();
        static bool nativeAutostartCheck();

};

#endif // SETUP_H
