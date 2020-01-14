#include <QDateTime>
#include <QTimer>
#include "../helpers.h"
#include "../settings.h"
#include "inserttimedialog.h"
#include "ui_inserttimedialog.h"

InsertTimeDialog::InsertTimeDialog(QWidget *parent) : QDialog(parent), ui(new Ui::InsertTimeDialog) {
    ui->setupUi(this);
    this->setFixedSize(this->geometry().width(), this->geometry().height());

    Helpers::setReadonlyPalette(ui->exampleText);
    //QPalette readOnlyPalette = ui->exampleText->palette();
    //readOnlyPalette.setColor(QPalette::Base, ui->exampleText->palette().color(QPalette::Window));
    //ui->exampleText->setPalette(readOnlyPalette);

    ui->formatText->setText(Settings::timeFormat());

    updateExample = new QTimer(this);
    connect(updateExample, &QTimer::timeout, this, &InsertTimeDialog::onUpdateExampleTimeout);
    updateExample->start(1000);

    connect(ui->formatText, SIGNAL(textEdited(const QString&)), SLOT(onTextEdited(const QString&)));
    onUpdateExampleTimeout();
}

InsertTimeDialog::~InsertTimeDialog() {
    delete ui;
}

void InsertTimeDialog::accept() {
    Settings::setTimeFormat(ui->formatText->text());

    this->FormattedTime = getFormattedText(ui->formatText->text());
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
