#include "filenamedialog.h"
#include "ui_filenamedialog.h"
#include <QPushButton>

FileNameDialog::FileNameDialog(QWidget *parent, QString fileName, std::shared_ptr<FolderItem> folder)
    : QDialog(parent)
    , ui(new Ui::FileNameDialog) {
    ui->setupUi(this);

    _folder = folder;

    this->layout()->setSizeConstraint(QLayout::SetFixedSize);
    if (fileName != nullptr) {
        ui->textFileName->setText(fileName);
    } else {
        fileName = "New file";
        auto index = 1;
        auto newFileName = fileName;
        while (folder->fileExists(newFileName)) {
            index++;
            newFileName = fileName;
            newFileName.append(QString(" (%1)").arg(index));
        }
        ui->textFileName->setText(newFileName);

    }
    ui->textFileName->setFocus();

    connect(ui->textFileName, SIGNAL(textEdited(const QString&)), this, SLOT(onTextEdited(const QString&)));
    onTextEdited(ui->textFileName->text());
}

FileNameDialog::~FileNameDialog() {
    delete ui;
}


QString FileNameDialog::getFileName() {
    return ui->textFileName->text();
}


void FileNameDialog::onTextEdited(const QString &text) {
    if (text.length() == 0) {
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(false);
    } else {
        bool foundMatch = false;
        for (size_t i = 0; i < _folder->fileCount(); i++) {
            if (text.compare(_folder->getFile(i)->getTitle(), Qt::CaseInsensitive) == 0) { foundMatch = true; break; }
        }
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(!foundMatch);
    }
}
