#include "test_config.h"
#include "test_helpers.h"

int main(int argc, char* argv[]) {
    int status = 0;

    QApplication a(argc, argv);

    status |= QTest::qExec(new Test_Config, argc, argv);
    status |= QTest::qExec(new Test_Helpers, argc, argv);

    return status;
}
