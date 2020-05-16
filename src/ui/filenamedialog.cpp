#include <QPushButton>
#include "filenamedialog.h"
#include "ui_filenamedialog.h"
#include "helpers.h"

FileNameDialog::FileNameDialog(QWidget* parent) : QDialog(parent), ui(new Ui::FileNameDialog) {
    ui->setupUi(this);
    Helpers::setupFixedSizeDialog(this);

    ui->textFileName->setFocus();

    connect(ui->textFileName, &QLineEdit::textChanged, this, &FileNameDialog::onTextChanged);
    onTextChanged(ui->textFileName->text());
}

FileNameDialog::FileNameDialog(QWidget* parent, FolderItem* folder) : FileNameDialog(parent) {
    _folder = folder;
    _file = nullptr;

    QString baseFileTitle = "New file";
    int index = 1;
    QString newFileTitle = baseFileTitle;
    while (folder->fileExists(newFileTitle)) {
        index++;
        newFileTitle = QString("%1 (%2)").arg(baseFileTitle, QString::number(index));
    }
    ui->textFileName->setText(newFileTitle);
}

FileNameDialog::FileNameDialog(QWidget* parent, FileItem* file) : FileNameDialog(parent) {
    _folder = file->folder();
    _file = file;

    ui->textFileName->setText(file->title());
}

FileNameDialog::~FileNameDialog() {
    delete ui;
}


QString FileNameDialog::getTitle() {
    return ui->textFileName->text();
}


void FileNameDialog::onTextChanged(const QString& text) {
    if (text.length() == 0) {
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(false);
    } else {
        bool foundMatch = false;
        for (FileItem* iFile : *_folder) {
            if (_file == iFile) {
                if (text.compare(iFile->title(), Qt::CaseSensitive) == 0) { foundMatch = true; break; }
            } else {
                if (text.compare(iFile->title(), Qt::CaseInsensitive) == 0) { foundMatch = true; break; }
            }
        }
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(!foundMatch);
    }
}
