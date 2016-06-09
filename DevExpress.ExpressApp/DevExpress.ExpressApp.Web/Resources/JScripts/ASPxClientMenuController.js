function aspxClientMenuController() {


    this.SetupASPxClientMenu = function (clientMenu) {
        this.DropDownItemsOverrideHover(clientMenu);
        this.SetupASPxPopupMenu(clientMenu);
        this.UpdateNestedFrameMenuWidth();
    }
    this.UpdateNestedFrameMenuWidth = function () {
        this.UpdatePopupNestedFrameMenu();
        this.UpdateDetailNestedFrameMenu();
    }
    this.UpdatePopupNestedFrameMenu = function () {
        if (window.viewSite) {
            var nestedFrameMenuTables = $(".dialogContent .Item .NestedFrame .nf_Menu");
            var styleType = "width";
            var allWidth = window.viewSite.clientWidth;
            $.each(nestedFrameMenuTables, function (i) {
                if (this.children.length == 2) {
                    var leftMenuWidth = this.children[0].scrollWidth;
                    if (this.children[1].scrollWidth > 0) {
                        var rightMenuWidth = allWidth - leftMenuWidth - 70 + "px";
                        var rightmenu = $(this.children[1]);
                        if (rightmenu.attr("style") != undefined) {
                            if (rightmenu.attr("style").indexOf(styleType) == -1) {
                                rightmenu.attr("style", rightmenu.attr("style") + styleType + ":" + rightMenuWidth + ";");
                            } else {
                                rightmenu.attr("style", styleType + ":" + rightMenuWidth + ";");
                            }

                        } else {
                            rightmenu.attr("style", styleType + ":" + rightMenuWidth + ";");
                        }
                    }
                }
            });
        }
    }
    this.UpdateDetailNestedFrameMenu = function () {
        if (window.viewSite) {
            var nestedFrameMenuTables = $(".LayoutTabContainer .Item .NestedFrame .nf_Menu");
            var styleType = "width";
            var allWidth = window.viewSite.clientWidth;
            $.each(nestedFrameMenuTables, function (i) {
                if (this.children.length == 2) {
                    var leftMenuWidth = this.children[0].scrollWidth;
                    if (this.children[1].scrollWidth > 0) {
                        var rightMenuWidth = allWidth - leftMenuWidth - 70 + "px";
                        var rightmenu = $(this.children[1]);
                        if (rightmenu.attr("style") != undefined) {
                            if (rightmenu.attr("style").indexOf(styleType) == -1) {
                                rightmenu.attr("style", rightmenu.attr("style") + styleType + ":" + rightMenuWidth + ";");
                            } else {
                                rightmenu.attr("style", styleType + ":" + rightMenuWidth + ";");
                            }

                        } else {
                            rightmenu.attr("style", styleType + ":" + rightMenuWidth + ";");
                        }
                    }
                }
            });
        }
    }
    this.AddCssStyle = function (selector, styleType, styleValue) {
        var elements = $(selector);
        $.each(elements, function (i) {
            if ($(this).attr("style") != undefined) {
                if ($(this).attr("style").indexOf(styleType) == -1) {
                    $(this).attr("style", $(this).attr("style") + styleType + ":" + styleValue + ";");
                }
            } else {
                $(this).attr("style", styleType + ":" + styleValue + ";");
            }
        });
    }
    this.SetupDropDownItemsHover = function (dropDownCssStyle, clientMenu) {
        var dxmItemsSelector = ".menuButtons li." + dropDownCssStyle + " a.dx, .menuButtons .dxm-hovered." + dropDownCssStyle;
        var popOut = $(".dxmLite_XafTheme.dxm-ltr .menuButtons li." + dropDownCssStyle + " .dxm-popOut");
        var popUp = $(".dxm-popup li." + dropDownCssStyle).parents(".dxm-popup");
        var menuButtonsPopOutSelector = ".menuButtons li." + dropDownCssStyle + " .dxm-popOut";

        popOut.hover(
            function (e) {
                xaf.ASPxClientMenuController.AddCssStyle(dxmItemsSelector, "background-color", "#fff!important");
                xaf.ASPxClientMenuController.AddCssStyle(menuButtonsPopOutSelector, "background-color", "#f0f0f0!important");
            },
            function (e) {
                if (window.countVisiblePopup > 0) {
                    clientMenu.PopupMenuHide.AddHandler(function (s, e) {
                        if (window.countVisiblePopup == 0) {
                            $(dxmItemsSelector).css("background-color", "");
                            $(menuButtonsPopOutSelector).css("background-color", "");
                            clientMenu.PopupMenuHide.ClearHandlers();
                        }
                    });
                } else {
                    $(dxmItemsSelector).css("background-color", "");
                    $(menuButtonsPopOutSelector).css("background-color", "");
                }
            }
        );
        popUp.hover(
            function (e) {
                xaf.ASPxClientMenuController.AddCssStyle(dxmItemsSelector, "background-color", "#fff!important");
                xaf.ASPxClientMenuController.AddCssStyle(menuButtonsPopOutSelector, "background-color", "#f0f0f0!important");
            }
        );
    }
    this.DropDownItemsOverrideHover = function (clientMenu) {
        for (var i = 0; i < clientMenu.cpDropDownHoverClasses.length; i++) {
            this.SetupDropDownItemsHover(clientMenu.cpDropDownHoverClasses[i], clientMenu);
        }
    }
    this.SetupASPxPopupMenu = function (clientMenu) {
        clientMenu.OldDoShowPopupMenu = clientMenu.DoShowPopupMenu;
        clientMenu.OldDoHidePopupMenu = clientMenu.DoHidePopupMenu;

        clientMenu.PopupMenuHide = new ASPxClientEvent();

        clientMenu.DoShowPopupMenu = function (element, x, y, indexPath) {
            this.OldDoShowPopupMenu(element, x, y, indexPath);
            window.countVisiblePopup++;
        }
        clientMenu.DoHidePopupMenu = function (evt, element) {
            this.OldDoHidePopupMenu(evt, element);
            window.countVisiblePopup--;
            if (!clientMenu.PopupMenuHide.IsEmpty()) {
                clientMenu.PopupMenuHide.FireEvent(this);
            }
        }
    }
};

xaf.ASPxClientMenuController = xaf.ControllersManager.CreateController(aspxClientMenuController);