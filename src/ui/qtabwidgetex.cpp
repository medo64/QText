#include "storage/fileitem.h"
#include "qtabwidgetex.h"
#include "qtabbarex.h"

QTabWidgetEx::QTabWidgetEx(QWidget* parent)
    : QTabWidget(parent) {
    QTabBarEx* bar = new QTabBarEx();
    this->setTabBar(bar);

    connect(bar, &QTabBarEx::tabMoved, this, &QTabWidgetEx::onTabMoved);
}


int QTabWidgetEx::addTab(QWidget* widget, const QString& text) {
    FileItem* item = dynamic_cast<FileItem*>(widget);

    QTabBar* tabBar = this->tabBar();
    int index = QTabWidget::addTab(widget, text);

    if (Settings::tabTextColorPerType()) { //playing with color
        QColor color = tabBar->tabTextColor(index);
        bool isDark = ((color.red() + color.green() + color.blue()) / 3) < 64;
        QColor newColor;
        switch (item->type()) {
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
            case FileType::Markdown: //green
                newColor = isDark ? QColor(color.red(), 96, color.blue())
                           : QColor(color.red() * 0.75, color.green(), color.blue() * 0.75);
                break;
#endif
            case FileType::Html: //blue
                newColor = isDark ? QColor(color.red(), color.green(), 128)
                           : QColor(color.red() * 0.75, color.green() * 0.75, color.blue());
                break;
            default:
                newColor = color;
                break;
        }
        this->tabBar()->setTabTextColor(index, newColor);
    }

    return index;
}


void QTabWidgetEx::onTabMoved(int from, int to) {
    auto fileFrom = dynamic_cast<FileItem*>(this->widget(from));
    this->setTabText(from, fileFrom->title());
    auto fileTo = dynamic_cast<FileItem*>(this->widget(to));
    this->setTabText(to, fileTo->title());
    emit tabMoved(from, to);
}
