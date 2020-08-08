#include "helpers.h"
#include "phoneticalphabet.h"
#include "phoneticdialog.h"
#include "ui_phoneticdialog.h"

PhoneticDialog::PhoneticDialog(QWidget* parent, QString text)
    : QDialog(parent), ui(new Ui::PhoneticDialog) {
    ui->setupUi(this);
    Helpers::setupResizableDialog(this);
    Helpers::setReadonlyPalette(ui->txtOutput);

    connect(ui->txtInput, &QLineEdit::textChanged, this, &PhoneticDialog::onChanged);

    ui->txtInput->setText(text);
}

PhoneticDialog::~PhoneticDialog() {
    delete ui;
}


void PhoneticDialog::onChanged() {
    QString text = ui->txtInput->text();
    QString output;

    bool addSpace = false;
    foreach (QString ch, text.split("", QString::SkipEmptyParts)) { //splits on UTF-8 borders correctly
        if (ch == " ") {
            output += "\n";
            addSpace = false;
        } else {
            if (addSpace) { output += " "; } else { addSpace = true; }
            output += PhoneticAlphabet::getNatoText(ch);
        }
    }

    ui->txtOutput->document()->setPlainText(output);
}
