#include "filenamedialog.h"
#include "ui_filenamedialog.h"

FileNameDialog::FileNameDialog(QWidget *parent) : QDialog(parent), ui(new Ui::FileNameDialog) {
    ui->setupUi(this);

    this->layout()->setSizeConstraint(QLayout::SetFixedSize);
    ui->textFileName->setFocus();
}

FileNameDialog::~FileNameDialog() {
    delete ui;
}


QString FileNameDialog::getFileName() {
    return ui->textFileName->text();
}
