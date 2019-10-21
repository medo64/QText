#include <QPushButton>
#include "filenamedialog.h"
#include "ui_filenamedialog.h"

FileNameDialog::FileNameDialog(QWidget *parent)
    : QDialog(parent)
    , ui(new Ui::FileNameDialog) {
    ui->setupUi(this);
    this->setFixedSize(this->geometry().width(), this->geometry().height());

    ui->textFileName->setFocus();

    connect(ui->textFileName, SIGNAL(textChanged(const QString&)), this, SLOT(onTextChanged(const QString&)));
    onTextChanged(ui->textFileName->text());
}

FileNameDialog::FileNameDialog(QWidget *parent, FolderItem* folder) : FileNameDialog(parent) {
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

FileNameDialog::FileNameDialog(QWidget *parent, FileItem* file) : FileNameDialog(parent) {
    _folder = file->getFolder();
    _file = file;

    ui->textFileName->setText(file->getTitle());
}

FileNameDialog::~FileNameDialog() {
    delete ui;
}


QString FileNameDialog::getTitle() {
    return ui->textFileName->text();
}


void FileNameDialog::onTextChanged(const QString &text) {
    if (text.length() == 0) {
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(false);
    } else {
        bool foundMatch = false;
        for (size_t i = 0; i < _folder->fileCount(); i++) {
            FileItem* iFile = _folder->getFile(i);
            if (_file == iFile) {
                if (text.compare(iFile->getTitle(), Qt::CaseSensitive) == 0) { foundMatch = true; break; }
            } else {
                if (text.compare(iFile->getTitle(), Qt::CaseInsensitive) == 0) { foundMatch = true; break; }
            }
        }
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(!foundMatch);
    }
}
