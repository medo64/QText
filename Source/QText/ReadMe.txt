                                              Josip Medved
                                              http://www.jmedved.com


                                 QText


Shortcut keys

   Del                     Delete
   Esc                     Close
   F2                      Rename
   F3                      Find next
   F5                      Refresh

   Ctrl+1                  Selects 1st tab
   Ctrl+2                  Selects 2nd tab
   Ctrl+3                  Selects 3rd tab
   Ctrl+4                  Selects 4th tab
   Ctrl+5                  Selects 5th tab
   Ctrl+6                  Selects 6th tab
   Ctrl+7                  Selects 7th tab
   Ctrl+8                  Selects 8th tab
   Ctrl+9                  Selects 9th tab
   Ctrl+0                  Selects 10th tab
   Ctrl+A                  Select all
   Ctrl+C                  Copy
   Ctrl+F                  Find
   Ctrl+N                  New
   Ctrl+R                  Reopen
   Ctrl+S                  Save now
   Ctrl+T                  Always on top
   Ctrl+V                  Paste
   Ctrl+X                  Cut
   Ctrl+Y                  Redo
   Ctrl+Z                  Undo
   Ctrl+PageUp             Selects tab to the left
   Ctrl+PageDown           Selects tab to the right
   Ctrl+Tab                Alternates between two last used tabs

   Ctrl+Shift+S            File, Save all

   Ctrl+Alt+C              Copy without formatting
   Ctrl+Alt+V              Paste without formatting
   Ctrl+Alt+X              Cut without formatting


Version history

   Version 2.32 (2010-12-20)

      o  Closing is faster.
      o  Bug-fixing.

   Version 2.31 (2010-11-11)

      o  Directory is now created during startup.

   Version 2.30 (2010-11-10)

      o  All text is now internally handled via RichTextBox.
      o  Files can be hidden.
      o  QText updates files when another process changes them.
      o  Lot of minor improvements.
      o  No reqistry writes if program is not installed.
      o  Fix: Cursor does not reset position upon save.

   Version 2.20 (2009-10-01)

      o  Fix: Window restoring via shortcut.
      o  Fix: Clipboard bug (Requested Clipboard operation did not
         succeed).
      o  Added Find option.
      o  Added license.

   Version 2.10 (2009-06-30)

      o  Changed installation engine to Inno Setup.
      o  Installation support for 64-bit.
      o  RichText files obey formatting when pasting.
      o  Removed Calendar.
      o  Minor bug fixes.

   Version 2.00 (2009-03-19)

      o  Changed menu appearance.
      o  Redesigned Options.
      o  Added Ctrl+1, Ctrl+2, ..., Ctrl+0 shortcuts.
      o  All code is in one .exe.
      o  Application name is no longer written on top of each printed
         page.
      o  Sorting works on Windows Vista.
      o  Support for RTF.
      o  Tray option to restore on primary screen.
      o  Auto update in case of direct file system changes.
      o  Fixed loading of form on repeated start.
      o  Support for high DPI.
      o  Defaults for font are taken from system.
      o  Splash screen is now not displayed by default.
      o  Full support for 64-bit systems.
      o  Removed auto-start after install (Vista's UAC made some
         problems).

   Version 1.40 (2007-12-03)

      o  Manifest for Windows Vista.
      o  Changed toolbar and menu appearance.
      o  Fixed multiple screens issues.
      o  Added support for Redo (only with Rich Text Box).
      o  Fixed right-click cut behaviour.

   Version 1.31 (2007-02-08)

      o  Fixed taskbar click behaviour.
      o  Fixed problems with window positioning.

   Version 1.30 (2007-01-25)

      o  Added unhandled exceptions reporting.
      o  Added Ctrl-Backspace and Ctrl-Del shortcuts.
      o  Fixed small multiline tab problems.
      o  Fixed disabled exit in case of permissions problem.
      o  Added option to reset options to default.
      o  Added selection sorting.
      o  Added selection case conversion.
      o  Added option of minimize/maximize buttons.
      o  Added option to minimize to tray.
      o  Added option to activate tray icon on one-click.

   Version 1.20 (2006-11-10)

      o  Added Save on Close option.
      o  Added Alt+Backspace shortcut.
      o  Added option to reopen last selected file upon startup.
      o  Minimum window size is allowed to be smaller (200x160).
      o  Added carbon copy.
      o  Changed TextBox to allow multiline tab identing.
      o  Fixed bug that caused save every second.
      o  User is not asked for delete confirmation of empty files.
      o  Added scrollbar settings.
      o  It sends deleted files to recycle bin if appropriate.
      o  Added printing.
      o  Fixed bug that prevented shutdown.

   Version 1.11 (2006-09-25)

      o  Fixed issue when program needed to be restarted in order for
         Hotkey to be activated.
      o  Added option entries for foreground and background text color.

   Version 1.10 (2006-09-25)

      o  Built on .NET Framework 2.0.
      o  Redesigned interface.
      o  Tabs can be reordered.
      o  First tab can be deleted.

   Version 1.04 (2005-10-06)

      o  Small update.

   Version 1.03 (2004-11-16)

      o  Added option to select storage folder.
      o  Did not save when only single file existed.

   Version 1.02 (2004-09-18)

      o  Added tray context menu.
      o  Added "Reopen".
      o  Increased startup speeds.

   Version 1.01 (2004-09-04)

      o  Fixed tray icon appearance.
      o  Vertical scroll added.

   Version 1.00 (2004-08-28)

      o  First public release.


Contributors (in alphabetic order)

    Dražen Bajus           Croatia
    Radu Capan             Romaina
    Marc Desbiens          Canada
    Ivan Gabajček          Croatia
    Michael Jagersberger   Austria
    Randy Simat
    Steven Yarnot          United States


Licence (MIT)

   Copyright (c) 2004 Josip Medved <jmedved@jmedved.com>

   Permission is hereby granted, free of charge, to any person obtaining
   a copy of this software and associated documentation files (the
   "Software"), to deal in the Software without restriction, including
   without limitation the rights to use, copy, modify, merge, publish,
   distribute, sublicense, and/or sell copies of the Software, and to
   permit persons to whom the Software is furnished to do so, subject to
   the following conditions:

      o  The above copyright notice and this permission notice shall be
         included in all copies or substantial portions of the Software.

      o  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
         EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
         OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
         NONINFRINGEMENT.  IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
         HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
         WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
         FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
         OTHER DEALINGS IN THE SOFTWARE.
