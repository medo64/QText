#include "qtabwidgetex.h"
#include "qtabbarex.h"

QTabWidgetEx::QTabWidgetEx(QWidget* parent)
    : QTabWidget(parent) {
    this->setTabBar(new QTabBarEx());
}
