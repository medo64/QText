### QText ###

Simple note taking program with multi-folder support, auto-save, and encryption support.


#### Shortcut Keys ####

  * `Esc`                     Close
  * `F2`                      Rename
  * `F3`                      Find next
  * `F5`                      Refresh
  * `F10`                     Activate toolbar.
  * `Ctrl+A`                  Select all
  * `Ctrl+C`                  Copy
  * `Ctrl+F`                  Find
  * `Ctrl+G`                  Navigation
  * `Ctrl+N`                  New
  * `Ctrl+R`                  Reopen
  * `Ctrl+S`                  Save now
  * `Ctrl+T`                  Always on top
  * `Ctrl+V`                  Paste
  * `Ctrl+X`                  Cut
  * `Ctrl+Y`                  Redo
  * `Ctrl+Z`                  Undo
  * `Ctrl+0`                  Reset zoom.
  * `Ctrl+Tab`                Alternates between two last used tabs
  * `Ctrl+Plus`               Zoom in.
  * `Ctrl+Minus`              Zoom out.
  * `Ctrl+Backspace`          Delete word on left.
  * `Ctrl+Delete`             Delete word on right.
  * `Ctrl+PgUp`               Select tab on the left.
  * `Ctrl+PgDn`               Select tab on the right.
  * `Ctrl+Shift+C`            Copy as plain text
  * `Ctrl+Shift+V`            Paste as plain text
  * `Ctrl+Shift+X`            Cut as plain text
  * `Alt+1`                   Selects 1st tab.
  * `Alt+2`                   Selects 2nd tab.
  * `Alt+3`                   Selects 3rd tab.
  * `Alt+4`                   Selects 4th tab.
  * `Alt+5`                   Selects 5th tab.
  * `Alt+6`                   Selects 6th tab.
  * `Alt+7`                   Selects 7th tab.
  * `Alt+8`                   Selects 8th tab.
  * `Alt+9`                   Selects 9th tab.
  * `Alt+0`                   Selects 10th tab.
  * `Alt+Left`                Select tab on the left.
  * `Alt+Right`               Select tab on the right.
  * `Alt+Up`                  Select previous folder.
  * `Alt+Down`                Select next folder.
  * `Alt+Home`                Select default folder.
  * `Alt+PageUp`              Select previous folder.
  * `Alt+PageDown`            Select next folder.
  * `Alt+Shift+D`             Inserts current date into text.
  * `Alt+Shift+T`             Inserts current time into text.


#### Encryption ####

All encrypted files are compatible with salted OpenSSL 256-bit AES CBC file
encryption (as defined at http://www.openssl.org/docs/apps/enc.html). In order
to decrypt file OpenSSL command line would be:

    openssl aes-256-cbc -d -in file.txt.aes256cbc -out file.txt -k "password"

That means that, even if you stop using this fabulous program, you can always
get to your data. I consider this an important feature.

Downside to using this standard algorithm is that it is rather fast. And fast
is bad when you need to deal with brute force attacks. This encryption is no
worse than what OpenSSL normally does (it is equivalent actually) but beware
that lot of security lies in well chosen password.


#### Contributors ####

List of people that helped with the project (in alphabetic order):

  * Dražen Bajus            (Croatia)
  * Radu Capan              (Romaina)
  * Marc Desbiens           (Canada)
  * Ivan Gabajček           (Croatia)
  * Igor Griner             (Croatia)
  * Michael Jagersberger    (Austria)
  * Randy Simat
  * Steven Yarnot           (United States)
