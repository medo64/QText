#include <QPushButton>

#include "helpers.h"

#include "finddialog.h"
#include "ui_finddialog.h"

FindDialog::FindDialog(QWidget *parent, QString searchText, bool matchCase, bool wholeWord, bool useRegEx) : QDialog(parent), ui(new Ui::FindDialog) {
    ui->setupUi(this);
    Helpers::setupFixedSizeDialog(this);

    ui->editSearch->setText(searchText);
    ui->editSearch->selectAll();
    ui->editSearch->setFocus();

    ui->checkMatchCase->setChecked(matchCase);
    ui->checkWholeWord->setChecked(wholeWord);
    ui->checkUseRegEx->setChecked(useRegEx);

    connect(ui->editSearch, &QLineEdit::textChanged, this, &FindDialog::onStateChanged);
    connect(ui->checkMatchCase, &QCheckBox::stateChanged, this, &FindDialog::onStateChanged);
    connect(ui->checkWholeWord, &QCheckBox::stateChanged, this, &FindDialog::onStateChanged);
    connect(ui->checkUseRegEx, &QCheckBox::stateChanged, this, &FindDialog::onStateChanged);
    onStateChanged();
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

bool FindDialog::wholeWord() {
    return ui->checkWholeWord->isChecked();
}

bool FindDialog::useRegEx() {
    return ui->checkUseRegEx->isChecked();
}


void FindDialog::onStateChanged() {
    auto text = ui->editSearch->text();
    bool isRegEx = ui->checkUseRegEx->isChecked();

    ui->checkWholeWord->setEnabled(!isRegEx);

    if (isRegEx) {
        auto regex = QRegExp(text);
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(regex.isValid() && !regex.isEmpty());
    } else {
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(text.length() > 0);
    }
}
