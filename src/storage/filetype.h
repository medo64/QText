#pragma once
#include <QtGlobal>

enum class FileType {
    Plain    = 0,
    Html     = 1,
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
    Markdown = 2,
#endif
};
