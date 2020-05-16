#include "fileitem.h"
#include "qtabwidgetex.h"
#include "qtabbarex.h"

QTabWidgetEx::QTabWidgetEx(QWidget* parent)
    : QTabWidget(parent) {
    QTabBarEx* bar = new QTabBarEx();
    this->setTabBar(bar);

    connect(bar, &QTabBarEx::tabMoved, this, &QTabWidgetEx::onTabMoved);
}


void QTabWidgetEx::onTabMoved(int from, int to) {
    auto fileFrom = dynamic_cast<FileItem*>(this->widget(from));
    this->setTabText(from, fileFrom->title());
    auto fileTo = dynamic_cast<FileItem*>(this->widget(to));
    this->setTabText(to, fileTo->title());
    emit tabMoved(from, to);
}
