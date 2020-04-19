#include <QPushButton>

#include "helpers.h"

#include "finddialog.h"
#include "ui_finddialog.h"

FindDialog::FindDialog(QWidget *parent, QString searchText, bool matchCase) : QDialog(parent), ui(new Ui::FindDialog) {
    ui->setupUi(this);
    Helpers::setupFixedSizeDialog(this);

    ui->editSearch->setText(searchText);
    ui->editSearch->selectAll();
    ui->editSearch->setFocus();

    ui->checkMatchCase->setChecked(matchCase);

    connect(ui->editSearch, &QLineEdit::textChanged, this, &FindDialog::onTextChanged);
    onTextChanged(ui->editSearch->text());
}

FindDialog::~FindDialog() {
    delete ui;
}


QString FindDialog::searchText() {
    return ui->editSearch->text();
}

bool FindDialog::matchCase() {
    return ui->checkMatchCase->isChecked();
}


void FindDialog::onTextChanged(const QString &text) {
    bool hasText = text.length() > 0;
    ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(hasText);
}
