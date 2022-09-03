/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

// 2020-01-03: Initial version

#pragma once

#include <QWidget>

namespace Medo { class About; }

class About {

    public:
        static void showDialog(QWidget* parent = nullptr);

};
