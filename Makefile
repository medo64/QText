.PHONY: all clean distclean install uninstall dist release debug package

ifeq ($(PREFIX),)
    PREFIX := /usr/local/
endif


DIST_NAME := qtext
DIST_VERSION := $(shell grep VERSION src/QText.pro | head -1 | cut -d'=' -f2 | awk '{print $$1}')
DIST_SHORT_VERSION := $(shell echo $(DIST_VERSION) | cut -d. -f1-2)

DEB_BUILD_ARCH := $(shell getconf LONG_BIT | sed "s/32/i386/" | sed "s/64/amd64/")


SOURCE_LIST := Makefile CONTRIBUTING.md LICENSE.md README.md src/ docs/


all: release


clean:
	-@$(RM) -r bin/
	-@$(RM) -r build/

distclean: clean
	-@$(RM) -r dist/
	-@$(RM) -r target/


install: bin/qtext
	@sudo install -d $(DESTDIR)/$(PREFIX)/bin/
	@sudo install bin/qtext $(DESTDIR)/$(PREFIX)/bin/
	@mkdir -p build/man/
	@sed 's/MAJOR.MINOR//g' docs/man/qtext.1 > build/man/qtext.1
	@gzip -cn --best build/man/qtext.1 > build/man/qtext.1.gz
	@sudo install -m 644 build/man/qtext.1.gz /usr/share/man/man1/
	@sudo mandb -q
	@echo Installed at $(DESTDIR)/$(PREFIX)/bin/ | sed 's^//^/^g'

uninstall: $(DESTDIR)/$(PREFIX)/bin/qtext
	@sudo $(RM) $(DESTDIR)/$(PREFIX)/bin/qtext
	@sudo $(RM) /usr/share/man/man1/qtext.1.gz
	@sudo mandb -q

dist: release
	@$(RM) -r build/dist/
	@mkdir -p build/dist/$(DIST_NAME)-$(DIST_VERSION)/
	@cp -r $(SOURCE_LIST) build/dist/$(DIST_NAME)-$(DIST_VERSION)/
	@tar -cz -C build/dist/  --owner=0 --group=0 -f build/dist/$(DIST_NAME)-$(DIST_VERSION).tar.gz $(DIST_NAME)-$(DIST_VERSION)/
	@mkdir -p dist/
	@mv build/dist/$(DIST_NAME)-$(DIST_VERSION).tar.gz dist/
	@echo Output at dist/$(DIST_NAME)-$(DIST_VERSION).tar.gz


release: src/QText.pro
	@command -v qmake >/dev/null 2>&1 || { echo >&2 "No 'qmake' in path, consider installing 'qtbase5-dev' package!"; exit 1; }
	@test -d /usr/share/doc/libqt5x11extras5-dev || { echo >&2 "X11 extras not found, consider installing 'libqt5x11extras5-dev' package!"; exit 1; }
	@mkdir -p build/
	@cd build/ ; qmake -qt=qt5 CONFIG+=release ../src/QText.pro ; make
	@mkdir -p bin/
	@cp build/qtext bin/qtext

debug: src/QText.pro
	@command -v qmake >/dev/null 2>&1 || { echo >&2 "No 'qmake' in path, consider installing 'qtbase5-dev' package!"; exit 1; }
	@test -d /usr/share/doc/libqt5x11extras5-dev || { echo >&2 "X11 extras not found, consider installing 'libqt5x11extras5-dev' package!"; exit 1; }
	@mkdir -p build/
	@cd build/ ; qmake -qt=qt5 CONFIG+=debug ../src/QText.pro ; make
	@mkdir -p bin/
	@cp build/qtext bin/qtext


package: dist
	@command -v dpkg-deb >/dev/null 2>&1 || { echo >&2 "Package 'dpkg-deb' not installed!"; exit 1; }
	@echo "Packaging for $(DEB_BUILD_ARCH)"
	@$(eval PACKAGE_NAME = $(DIST_NAME)_$(DIST_SHORT_VERSION)_$(DEB_BUILD_ARCH))
	@$(eval PACKAGE_DIR = /tmp/$(PACKAGE_NAME)/)
	-@$(RM) -r $(PACKAGE_DIR)/
	@mkdir $(PACKAGE_DIR)/
	@cp -r package/deb/DEBIAN $(PACKAGE_DIR)/
	@cp -r package/deb/usr $(PACKAGE_DIR)/
	@sed -i "s/MAJOR.MINOR/$(DIST_SHORT_VERSION)/" $(PACKAGE_DIR)/DEBIAN/control
	@sed -i "s/ARCHITECTURE/$(DEB_BUILD_ARCH)/" $(PACKAGE_DIR)/DEBIAN/control
	@mkdir -p $(PACKAGE_DIR)/usr/share/doc/qtext/
	@cp package/deb/copyright $(PACKAGE_DIR)/usr/share/doc/qtext/copyright
	@cp CHANGES.md build/changelog
	@sed -i '/^$$/d' build/changelog
	@sed -i '/## Release Notes ##/d' build/changelog
	@sed -i '1{s/### \(.*\) \[.*/qtext \(\1\) stable; urgency=low/}' build/changelog
	@sed -i '/###/,$$d' build/changelog
	@sed -i 's/\* \(.*\)/  \* \1/' build/changelog
	@echo >> build/changelog
	@echo ' -- Josip Medved <jmedved@jmedved.com>  $(shell date -R)' >> build/changelog
	@gzip -cn --best build/changelog > $(PACKAGE_DIR)/usr/share/doc/qtext/changelog.gz
	@mkdir -p build/man/
	@sed 's/MAJOR.MINOR//g' docs/man/qtext.1 > build/man/qtext.1
	@mkdir -p $(PACKAGE_DIR)/usr/share/man/man1/
	@gzip -cn --best build/man/qtext.1 > $(PACKAGE_DIR)/usr/share/man/man1/qtext.1.gz
	@find $(PACKAGE_DIR)/ -type d -exec chmod 755 {} +
	@find $(PACKAGE_DIR)/ -type f -exec chmod 644 {} +
	@chmod 755 $(PACKAGE_DIR)/DEBIAN/p*inst $(PACKAGE_DIR)/DEBIAN/p*rm
	@install -d $(PACKAGE_DIR)/opt/qtext/
	@install bin/qtext $(PACKAGE_DIR)/opt/qtext/
	@strip $(PACKAGE_DIR)/opt/qtext/qtext
	@fakeroot dpkg-deb --build $(PACKAGE_DIR)/ > /dev/null
	@cp /tmp/$(PACKAGE_NAME).deb dist/
	@$(RM) -r $(PACKAGE_DIR)/
	-@lintian --suppress-tags dir-or-file-in-opt dist/$(PACKAGE_NAME).deb
	@echo Output at dist/$(PACKAGE_NAME).deb
