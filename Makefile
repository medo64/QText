.PHONY: all clean distclean install uninstall dist release debug test package preview

ifeq ($(PREFIX),)
    PREFIX := /usr/local/
endif


DIST_NAME := qtext
DIST_VERSION := $(shell grep VERSION src/QText.pro | head -1 | cut -d'=' -f2 | awk '{print $$1}' | sed 's/"//g')

MAN_DATE := $(shell date +'%d %b %Y')

DEB_BUILD_ARCH := $(shell getconf LONG_BIT | sed "s/32/i386/" | sed "s/64/amd64/")


SOURCE_LIST := Makefile CONTRIBUTING.md LICENSE.md README.md src/ docs/


HAS_QMAKE := $(shell which qmake >/dev/null ; echo $$?)
HAS_X11EXTRAS := $(shell test -d /usr/share/doc/libqt5x11extras5-dev ; echo $$?)
HAS_DPKGDEB := $(shell command -v dpkg-deb >/dev/null 2>&1 ; echo $$?)
HAS_LINTIAN := $(shell which lintian >/dev/null ; echo $$?)
HAS_UNCOMMITTED := $(shell git diff --quiet ; echo $$?)


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
	@sed 's/MAJOR.MINOR.PATCH/$(DIST_VERSION)/g' docs/man/qtext.1 > build/man/qtext.1
	@sed -i 's/CURR_DATE/$(MAN_DATE)/g' build/man/qtext.1
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
	$(if $(findstring 0,$(HAS_QMAKE)),,$(error No 'qmake' in path, consider installing 'qtbase5-dev' package))
	$(if $(findstring 0,$(HAS_X11EXTRAS)),,$(error X11 extras not found, consider installing 'libqt5x11extras5-dev' package))
	$(if $(findstring 0,$(HAS_UNCOMMITTED)),,$(warning Uncommitted changes present))
	-@rm -r build/releases 2>/dev/null || true
	@mkdir -p build/release
	@cd build/release ; qmake -qt=qt5 CONFIG+=release ../../src/QText.pro ; make
	@mkdir -p bin/
	@cp build/release/qtext bin/qtext

debug: src/QText.pro
	$(if $(findstring 0,$(HAS_QMAKE)),,$(error No 'qmake' in path, consider installing 'qtbase5-dev' package))
	$(if $(findstring 0,$(HAS_X11EXTRAS)),,$(error X11 extras not found, consider installing 'libqt5x11extras5-dev' package))
	-@rm -r build/debug 2>/dev/null || true
	@mkdir -p build/debug
	@cd build/debug ; qmake -qt=qt5 CONFIG+=debug ../../src/QText.pro ; make
	@mkdir -p bin/
	@cp build/debug/qtext bin/qtext

test: src/QText.pro
	$(if $(findstring 0,$(HAS_QMAKE)),,$(error No 'qmake' in path, consider installing 'qtbase5-dev' package))
	$(if $(findstring 0,$(HAS_X11EXTRAS)),,$(error X11 extras not found, consider installing 'libqt5x11extras5-dev' package))
	-@rm -r build/test 2>/dev/null || true
	@mkdir -p build/test
	@cd build/test ; qmake -qt=qt5 CONFIG+=test ../../src/QText.pro ; make
	@build/test/qtexttests


package: dist
	$(if $(findstring 0,$(HAS_DPKGDEB)),,$(error Package 'dpkg-deb' not installed))
	$(if $(findstring 0,$(HAS_LINTIAN)),,$(warning No 'lintian' in path, consider installing 'lintian' package))
	$(if $(findstring 0,$(HAS_UNCOMMITTED)),,$(warning Uncommitted changes present))
	@echo "Packaging for $(DEB_BUILD_ARCH)"
	@$(eval PACKAGE_NAME = $(DIST_NAME)_$(DIST_VERSION)_$(DEB_BUILD_ARCH))
	@$(eval PACKAGE_DIR = /tmp/$(PACKAGE_NAME)/)
	-@$(RM) -r $(PACKAGE_DIR)/
	@mkdir $(PACKAGE_DIR)/
	@cp -r package/deb/DEBIAN $(PACKAGE_DIR)/
	@cp -r package/deb/usr $(PACKAGE_DIR)/
	@mkdir -p $(PACKAGE_DIR)/usr/share/icons/hicolor/16x16/apps/
	@mkdir -p $(PACKAGE_DIR)/usr/share/icons/hicolor/24x24/apps/
	@mkdir -p $(PACKAGE_DIR)/usr/share/icons/hicolor/32x32/apps/
	@mkdir -p $(PACKAGE_DIR)/usr/share/icons/hicolor/48x48/apps/
	@mkdir -p $(PACKAGE_DIR)/usr/share/icons/hicolor/64x64/apps/
	@mkdir -p $(PACKAGE_DIR)/usr/share/icons/hicolor/96x96/apps/
	@mkdir -p $(PACKAGE_DIR)/usr/share/icons/hicolor/128x128/apps/
	@mkdir -p $(PACKAGE_DIR)/usr/share/icons/hicolor/192x192/apps/
	@mkdir -p $(PACKAGE_DIR)/usr/share/icons/hicolor/256x256/apps/
	@cp src/icons/16x16/app.png $(PACKAGE_DIR)/usr/share/icons/hicolor/16x16/apps/qtext.png
	@cp src/icons/24x24/app.png $(PACKAGE_DIR)/usr/share/icons/hicolor/24x24/apps/qtext.png
	@cp src/icons/32x32/app.png $(PACKAGE_DIR)/usr/share/icons/hicolor/32x32/apps/qtext.png
	@cp src/icons/48x48/app.png $(PACKAGE_DIR)/usr/share/icons/hicolor/48x48/apps/qtext.png
	@cp src/icons/64x64/app.png $(PACKAGE_DIR)/usr/share/icons/hicolor/64x64/apps/qtext.png
	@cp src/icons/96x96/app.png $(PACKAGE_DIR)/usr/share/icons/hicolor/96x96/apps/qtext.png
	@cp src/icons/128x128/app.png $(PACKAGE_DIR)/usr/share/icons/hicolor/128x128/apps/qtext.png
	@cp src/icons/192x192/app.png $(PACKAGE_DIR)/usr/share/icons/hicolor/192x192/apps/qtext.png
	@cp src/icons/256x256/app.png $(PACKAGE_DIR)/usr/share/icons/hicolor/256x256/apps/qtext.png
	@sed -i "s/MAJOR.MINOR.PATCH/$(DIST_VERSION)/" $(PACKAGE_DIR)/DEBIAN/control
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
	@sed 's/MAJOR.MINOR.PATCH/$(DIST_VERSION)/g' docs/man/qtext.1 > build/man/qtext.1
	@sed -i 's/CURR_DATE/$(MAN_DATE)/g' build/man/qtext.1
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
	@lintian --suppress-tags dir-or-file-in-opt dist/$(PACKAGE_NAME).deb
	@echo Output at dist/$(PACKAGE_NAME).deb

preview: docs/man/qtext.1
	@mkdir -p build/man/
	@sed 's/MAJOR.MINOR.PATCH/$(DIST_VERSION)/g' docs/man/qtext.1 > build/man/qtext.1
	@sed -i 's/CURR_DATE/$(MAN_DATE)/g' build/man/qtext.1
	@man -l build/man/qtext.1
