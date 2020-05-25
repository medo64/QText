#include <QDateTime>
#include <QTimer>
#include "inserttimedialog.h"
#include "ui_inserttimedialog.h"
#include "helpers.h"
#include "settings.h"

InsertTimeDialog::InsertTimeDialog(QWidget* parent) : QDialog(parent), ui(new Ui::InsertTimeDialog) {
    ui->setupUi(this);
    Helpers::setupFixedSizeDialog(this);

    Helpers::setReadonlyPalette(ui->exampleText);
    //QPalette readOnlyPalette = ui->exampleText->palette();
    //readOnlyPalette.setColor(QPalette::Base, ui->exampleText->palette().color(QPalette::Window));
    //ui->exampleText->setPalette(readOnlyPalette);

    ui->formatText->setText(Settings::timeFormat());

    updateExample = new QTimer(this);
    connect(updateExample, &QTimer::timeout, this, &InsertTimeDialog::onUpdateExampleTimeout);
    updateExample->start(1000);

    connect(ui->formatText, &QLineEdit::textEdited, this, &InsertTimeDialog::onTextEdited);
    onUpdateExampleTimeout();
}

InsertTimeDialog::~InsertTimeDialog() {
    delete ui;
}

void InsertTimeDialog::accept() {
    Settings::setTimeFormat(ui->formatText->text());

    _formattedTime = getFormattedText(ui->formatText->text());
    QDialog::accept();
}


void InsertTimeDialog::onTextEdited(const QString& text) {
    ui->exampleText->setText(getFormattedText(text));
}

void InsertTimeDialog::onUpdateExampleTimeout() {
    ui->exampleText->setText(getFormattedText(ui->formatText->text()));
}

QString InsertTimeDialog::getFormattedText(QString format) {
    if (format.trimmed().length() == 0) {
        return QDateTime::currentDateTime().toString(Qt::SystemLocaleShortDate);
    } else {
        return QDateTime::currentDateTime().toString(format);
    }
}
