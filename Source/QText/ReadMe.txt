                                                Josip Medved
                                                http://www.jmedved.com/

                                     QText


SHORTCUT KEYS

    Esc                     Close
    F2                      Rename
    F3                      Find next
    F5                      Refresh
    F10                     Activate toolbar.

    Ctrl+A                  Select all
    Ctrl+F                  Find
    Ctrl+G                  Navigation
    Ctrl+N                  New
    Ctrl+R                  Reopen
    Ctrl+S                  Save now
    Ctrl+T                  Always on top
    Ctrl+Y                  Redo
    Ctrl+Z                  Undo
    Ctrl+Tab                Alternates between two last used tabs

    Ctrl+X                  Cut
    Ctrl+C                  Copy
    Ctrl+V                  Paste
    Ctrl+Shift+X            Cut as plain text
    Ctrl+Shift+C            Copy as plain text
    Ctrl+Shift+V            Paste as plain text

    Ctrl+Backspace          Delete word on left.
    Ctrl+Delete             Delete word on right.

    Ctrl+Plus               Zoom in.
    Ctrl+Minus              Zoom out.
    Ctrl+0                  Reset zoom.
    Ctrl+PgUp               Select tab on the left.
    Ctrl+PgDn               Select tab on the right.

    Alt+1                   Selects 1st tab.
    Alt+2                   Selects 2nd tab.
    Alt+3                   Selects 3rd tab.
    Alt+4                   Selects 4th tab.
    Alt+5                   Selects 5th tab.
    Alt+6                   Selects 6th tab.
    Alt+7                   Selects 7th tab.
    Alt+8                   Selects 8th tab.
    Alt+9                   Selects 9th tab.
    Alt+0                   Selects 10th tab.
    Alt+Left                Select tab on the left.
    Alt+Right               Select tab on the right.
    Alt+Up                  Select previous folder.
    Alt+Down                Select next folder.
    Alt+PageUp              Select previous folder.
    Alt+PageDown            Select next folder.

    Alt+Shift+D             Inserts current date into text.
    Alt+Shift+T             Inserts current time into text.


ENCRYPTION

All encrypted files are compatible with salted OpenSSL 256-bit AES CBC file
encryption (as defined at http://www.openssl.org/docs/apps/enc.html). In
order to decrypt file OpenSSL command line would be:

   openssl aes-256-cbc -d -in file.txt.aes256cbc -out file.txt -k "password"

That means that, even if you stop using this fabulous program, you can always
get to your data. I consider this an important feature.

Downside to using this standard algorithm is that it is rather fast. And fast
is bad when you need to deal with brute force attacks. This encryption is no
worse than what OpenSSL normally does (it is equivalent actually) but beware
that lot of security lies in well chosen password.


CONTRIBUTORS (in alphabetic order)

    Dražen Bajus            Croatia
    Radu Capan              Romaina
    Marc Desbiens           Canada
    Ivan Gabajček           Croatia
    Igor Griner             Croatia
    Michael Jagersberger    Austria
    Randy Simat
    Steven Yarnot           United States


LICENCE (MIT)

   Copyright (c) 2009 Josip Medved <jmedved@jmedved.com>

   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to
   deal in the Software without restriction, including without limitation the
   rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
   sell copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:
    
      o  The above copyright notice and this permission notice shall be
         included in all copies or substantial portions of the Software.
        
      o  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
         EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
         MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
         IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
         CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
         TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
         SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
