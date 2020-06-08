#include <QPushButton>
#include "helpers.h"
#include "newfiledialog.h"
#include "settings.h"
#include "ui_newfiledialog.h"

NewFileDialog::NewFileDialog(QWidget* parent, FolderItem* folder) : QDialog(parent), ui(new Ui::NewFileDialog) {
    ui->setupUi(this);
    Helpers::setupFixedSizeDialog(this);

    _folder = folder;

    QString baseFileTitle = "New file";
    int index = 1;
    QString newFileTitle = baseFileTitle;
    while (folder->fileExists(newFileTitle)) {
        index++;
        newFileTitle = QString("%1 (%2)").arg(baseFileTitle, QString::number(index));
    }
    ui->textTitle->setText(newFileTitle);

#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
    bool showMarkdown = Settings::showMarkdown() || (Settings::defaultFileType() == FileType::Markdown);
#else
    bool showMarkdown = false;
#endif
    ui->radioMarkdown->setVisible(showMarkdown);

    switch (Settings::defaultFileType()) {
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
        case FileType::Markdown: ui->radioMarkdown->setChecked(true); break;
#endif
        case FileType::Html:     ui->radioHtml->setChecked(true);     break;
        default:                 ui->radioPlain->setChecked(true);    break;
    }

    connect(ui->textTitle, &QLineEdit::textChanged, this, &NewFileDialog::onChanged);
    connect(ui->radioMarkdown, &QRadioButton::toggled, this, &NewFileDialog::onChanged);
    connect(ui->radioPlain, &QRadioButton::toggled, this, &NewFileDialog::onChanged);
    connect(ui->radioHtml, &QRadioButton::toggled, this, &NewFileDialog::onChanged);
    onChanged();

    ui->textTitle->setFocus();
}

NewFileDialog::~NewFileDialog() {
    delete ui;
}


QString NewFileDialog::title() const {
    return ui->textTitle->text();
}

FileType NewFileDialog::type() const {
    if (ui->radioHtml->isChecked()) {
        return FileType::Html;
#if QT_VERSION >= QT_VERSION_CHECK(5, 14, 0)
    } else if (ui->radioMarkdown->isChecked()) {
        return FileType::Markdown;
#endif
    } else {
        return FileType::Plain;
    }
}


void NewFileDialog::onChanged() {
    QString text = ui->textTitle->text();

    if (text.length() == 0) {
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(false);
    } else {
        bool matchesName = false;
        for (FileItem* iFile : *_folder) {
            if (text.compare(iFile->title(), Qt::CaseInsensitive) == 0) { matchesName = true; break; }
        }
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(!matchesName);
    }
}
