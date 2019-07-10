## Contributing

Welcome and thank you for your interest in contributing.


### Compiling Code

Project uses Qt 5 and Qt Creator is the preferred development environment.

All code is situated in `src` directory.


### Running Unit Tests

To run unit tests, you will need to create a new build configuration. To do so
go to Projects and use Add button to add it. Under qmake build configuration add
`CONFIG+=Test` in Additional arguments. If you press Run, it will now execute
all test cases.


### Contributing Fixes

Fixes can be contributed using pull requests. Do note project is under MIT
license and any contribution will inherit the same.

LF line ending is strongly preffered. To have Git check it for you, configure
`core.whitespace` setting:

    git config core.whitespace blank-at-eol,blank-at-eof,space-before-tab,cr-at-eol

All textual files should be encoded as UTF-8 without BOM.


### Coding style

For non code files (xml, etc), use the best judgment. Albeit, do note that `tab`
character should be avoided and a number of `space` characters should be used
instead (4 being preffered).

Code uses K&R coding style. Please do make sure any contribution follows the
coding style already in use.


# Thank You!
