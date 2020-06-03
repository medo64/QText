#include <QLineEdit>
#include <QPushButton>
#include "medo/config.h"
#include "helpers.h"
#include "finddialog.h"
#include "ui_finddialog.h"

FindDialog::FindDialog(QWidget* parent) : QDialog(parent), ui(new Ui::FindDialog) {
    ui->setupUi(this);
    Helpers::setupFixedSizeDialog(this);

    auto terms = Config::stateReadMany("SearchTerms");
    ui->comboSearch->setInsertPolicy(QComboBox::NoInsert);
    for (auto term : terms) {
        ui->comboSearch->addItem(decomposeTerm(term), term);
    }

    ui->comboSearch->setEditable(true);
    ui->comboSearch->setCompleter(static_cast<QCompleter*>(nullptr));
    ui->comboSearch->setFocus();

    if (ui->comboSearch->count() > 0) {
        ui->comboSearch->setCurrentIndex(ui->comboSearch->count() - 1);
        onHistorySelected();
    }

    connect(ui->comboSearch, QOverload<const QString&>::of(&QComboBox::currentIndexChanged), this, &FindDialog::onHistorySelected);
    connect(ui->comboSearch, &QComboBox::editTextChanged, this, &FindDialog::onStateChanged);
    connect(ui->checkMatchCase, &QCheckBox::stateChanged, this, &FindDialog::onStateChanged);
    connect(ui->checkWholeWord, &QCheckBox::stateChanged, this, &FindDialog::onStateChanged);
    connect(ui->checkUseRegEx, &QCheckBox::stateChanged, this, &FindDialog::onStateChanged);
    onStateChanged();
}

FindDialog::~FindDialog() {
    delete ui;
}

void FindDialog::accept() {
    auto text = searchText();
    auto term = composeTerm(text, matchCase(), wholeWord(), useRegEx(), searchScope());

    auto terms = Config::stateReadMany("SearchTerms");
    for (int i = terms.count() - 1; i >= 0; i--) {
        auto termText = decomposeTerm(terms[i]);
        if (termText == text) { terms.removeAt(i); }
    }
    terms.append(term);

    while (terms.length() > 10) { terms.removeAt(0); }
    Config::stateWriteMany("SearchTerms", terms);

    QDialog::accept();
}


QString FindDialog::searchText() const {
    return ui->comboSearch->lineEdit()->text();
}

bool FindDialog::matchCase() const {
    return ui->checkMatchCase->isChecked();
}

bool FindDialog::wholeWord() const {
    return ui->checkWholeWord->isChecked();
}

bool FindDialog::useRegEx() const {
    return ui->checkUseRegEx->isChecked();
}

Find::SearchScope FindDialog::searchScope() const {
    if (ui->radioCurrentFile->isChecked()) {
        return Find::SearchScope::CurrentFile;
    } else if (ui->radioCurrentFolder->isChecked()) {
        return Find::SearchScope::CurrentFolder;
    } else {
        return Find::SearchScope::AllFolders;
    }
}

void FindDialog::onStateChanged() {
    auto text = ui->comboSearch->lineEdit()->text();
    bool isRegEx = ui->checkUseRegEx->isChecked();

    ui->checkWholeWord->setEnabled(!isRegEx);

    if (isRegEx) {
        auto toolTipText = "\\d: Matches a digit."  "\n"
                           "\\D: Matches a non-digit."  "\n"
                           "\\s: Matches a whitespace character."  "\n"
                           "\\S: Matches a non-whitespace character."  "\n"
                           "\\w: Matches a word character."  "\n"
                           "\\W: Matches a non-word character."  "\n"
                           "\\b: Word boundary assertions."  "\n"
                           "\\a: Matches the ASCII BEL (BEL, 0x07)." "\n"
                           "\\f: Matches the ASCII form feed (FF, 0x0C)." "\n"
                           "\\n: Matches the ASCII line feed (LF, 0x0A, Unix newline)."  "\n"
                           "\\r: Matches the ASCII carriage return (CR, 0x0D)."  "\n"
                           "\\t: Matches the ASCII horizontal tab (HT, 0x09)."  "\n"
                           "\\v: Matches the ASCII vertical tab (VT, 0x0B).";
        ui->comboSearch->setToolTip(toolTipText);
        auto regex = QRegExp(text);
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(regex.isValid() && !regex.isEmpty());
    } else {
        ui->comboSearch->setToolTip(QString());
        ui->buttonBox->button(QDialogButtonBox::Ok)->setEnabled(text.length() > 0);
    }
}

void FindDialog::onHistorySelected() {
    auto term = ui->comboSearch->currentData().toString();
    bool matchCase, wholeWord, useRegEx;
    Find::SearchScope searchScope;
    decomposeTerm(term, &matchCase, &wholeWord, &useRegEx, &searchScope);
    ui->checkMatchCase->setChecked(matchCase);
    ui->checkWholeWord->setChecked(wholeWord);
    ui->checkUseRegEx->setChecked(useRegEx);
    switch (searchScope) {
        case Find::SearchScope::CurrentFile:   ui->radioCurrentFile->setChecked(true);   break;
        case Find::SearchScope::CurrentFolder: ui->radioCurrentFolder->setChecked(true); break;
        default:                               ui->radioAllFolders->setChecked(true);    break;
    }
}


QString FindDialog::composeTerm(QString text, bool matchCase, bool wholeWord, bool useRegEx, Find::SearchScope searchScope) {
    auto flags = 0;
    if (matchCase) { flags |= 0x01; }
    if (wholeWord) { flags |= 0x02; }
    if (useRegEx)  { flags |= 0x04; }
    switch (searchScope) {
        case Find::SearchScope::CurrentFile:   flags |= 0x10; break;
        case Find::SearchScope::CurrentFolder: flags |= 0x20; break;
        case Find::SearchScope::AllFolders:    flags |= 0x40; break;
    }
    return text + "\f" + QString::number(flags, 16);
}

QString FindDialog::decomposeTerm(QString term, bool* matchCase, bool* wholeWord, bool* useRegEx, Find::SearchScope* searhScope) {
    auto parts = term.split("\f");
    if (parts.length() == 2) {
        bool ok;
        auto flags = parts[1].toInt(&ok, 16);
        if (ok) {
            *matchCase = ((flags & 0x01) == 0x01);
            *wholeWord = ((flags & 0x02) == 0x02);
            *useRegEx = ((flags & 0x04) == 0x04);
            if ((flags & 0x10) == 0x10) {
                *searhScope = Find::SearchScope::CurrentFile;
            } else if ((flags & 0x20) == 0x20) {
                *searhScope = Find::SearchScope::CurrentFolder;
            } else {
                *searhScope = Find::SearchScope::AllFolders;
            }
            return parts[0];
        }
    }

    *matchCase = false;
    *wholeWord = false;
    *useRegEx = false;
    *searhScope = Find::SearchScope::AllFolders;
    return parts[0];
}

QString FindDialog::decomposeTerm(QString term) {
    bool matchCase, wholeWord, useRegEx;
    Find::SearchScope searchScope;
    return decomposeTerm(term, &matchCase, &wholeWord, &useRegEx, &searchScope);
}
