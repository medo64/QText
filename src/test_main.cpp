#include "test_helpers.h"

int main(int argc, char *argv[]) {
    int status = 0;
    status |= QTest::qExec(new Test_Helpers, argc, argv);

    return status;
}
