#pragma once

class Setup {

    public:
        static bool autostart();
        static void setAutostart(bool newAutostart);

    private:
        static void nativeAutostartAdd();
        static void nativeAutostartRemove();
        static bool nativeAutostartCheck();

};
