/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

// 2020-06-05: Initial version

#pragma once

#include <QNetworkReply>
#include <QWidget>

namespace Medo { class Feedback; }

class Feedback {

    public:
        /*! Shows feedback dialog. */
        static bool showDialog(QWidget* parent, QUrl url);

    private:
        static bool sendFeedback(QString text, QString senderEmail, QString senderDisplayName, QUrl url);
        static QString _errorMessage;

};
