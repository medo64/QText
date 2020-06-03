#include <QPushButton>
#include "renamefiledialog.h"
#include "ui_renamefiledialog.h"
#include "helpers.h"

RenameFileDialog::RenameFileDialog(QWidget* parent, FileItem* file) : QDialog(parent), ui(new Ui::RenameFileDialog) {
    ui->setupUi(this);
    Helpers::setupFixedSizeDialog(this);

    _file = file;

    ui->textTitle->setText(file->title());

    connect(ui->textTitle, &QLineEdit::textChanged, this, &RenameFileDialog::onChanged);
    onChanged();

    ui->textTitle->setFocus();
}

RenameFileDialog::~RenameFileDialog() {
    delete ui;
}


QString RenameFileDialog::title() const {
    return ui->textTitle->text();
}


void RenameFileDialog::onChanged() {
    QString text = ui->textTitle->text();

    if (text.length() == 0) {
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(false);
    } else {
        bool foundMatch = false;
        for (FileItem* iFile : *_file->folder()) {
            if (_file == iFile) {
                if (text.compare(iFile->title(), Qt::CaseSensitive) == 0) { foundMatch = true; break; }
            } else {
                if (text.compare(iFile->title(), Qt::CaseInsensitive) == 0) { foundMatch = true; break; }
            }
        }
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(!foundMatch);
    }
}
