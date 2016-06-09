(function() {
    ASPxClientSpreadsheet.PopupMenuHelper = function(spreadsheetControl) {
        this.spreadsheetControl = spreadsheetControl;
        this.menuItems = ASPxClientSpreadsheet.PopupMenuItems;

        initializeItems(this.menuItems, spreadsheetControl.menuIconSet);

        function initializeItems(items, iconSet) {
            ASPx.Data.ForEach(items, function(item) {
                initializeItem(item, iconSet);
            });
        }
        function initializeItem(item, iconSet) {
            item.text = getLocalizedString(item.textID);
            if(item.imageName)
                item.imageClassName = prepareImageClassName(item.imageName, iconSet);

            if(item.items)
                initializeItems(item.items, iconSet);
        }
        function getLocalizedString(stringId) {
            return ASPxClientSpreadsheet.Localization[stringId];
        }
        function prepareImageClassName(imageName, iconSet) {
            return "dxSpreadsheet_" + iconSet + "_" + imageName;
        }

        function getPopupMenu() {
            return spreadsheetControl.getPopupMenu();
        }
        function getAutoFilterPopupMenu() {
            return spreadsheetControl.getAutoFilterPopupMenu();
        }

        function isItemVisible(item, context) {
            return isItemSatisfyingCriteria(item, context, "menuVisible");
        }

        function isItemEnabled(item, context) {
            return isItemSatisfyingCriteria(item, context, "menuEnabled");
        }

        function isItemSatisfyingCriteria(item, context, criteriaName) {
            var command = ASPxClientSpreadsheet.ServerCommands.getCommandConfigByID(item.name);

            if(command[criteriaName]) {
                for(var contextVarName in command[criteriaName]) {
                    if(context[contextVarName] !== command[criteriaName][contextVarName])
                        return false;
                }
            }

            return true;
        }

        function populateItems(items, context) {
            var resultItems = [];

            ASPx.Data.ForEach(items, function(item) {
                if(isItemVisible(item, context)) {
                    if(item.items)
                        item.items = populateItems(item.items, context);

                    resultItems.push(item);
                }
            });

            return resultItems;
        }

        function preparePopupMenu(popupMenu, items, context) {
            var menuItems = populateItems(items, context);

            popupMenu.ClearItems();
            popupMenu.CreateItems(menuItems);

            applyEnabledState(popupMenu, context);
            applyReadOnly(popupMenu);
        }

        function applyReadOnly(popupMenu) {
            if(!spreadsheetControl.readOnly) return;

            applyEnabledStateCore(popupMenu, function(item) {
                if(item.name != ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("CopySelection").id)
                    item.SetEnabled(false);
            });
        }

        function applyEnabledState(popupMenu, context) {
            applyEnabledStateCore(popupMenu, function(item) {
                item.SetEnabled(isItemEnabled(item, context));
            });
        }

        function applyEnabledStateCore(popupMenu, enableFn) {
            for(var i = 0; i < popupMenu.GetItemCount(); i++) {
                var item = popupMenu.GetItem(i);

                enableFn(item);
            }
        }

        function prepareClearFilterColumnItem(popupMenu, context) {
            var clearFilterCommandId = ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterColumnClear").id;
            var clearFilterMenuItem = popupMenu.GetItemByName(clearFilterCommandId);
            var clearFilterColumnText = clearFilterMenuItem.GetText().replace("{0}", "'" + context.columnCaption + "'");

            clearFilterMenuItem.SetText(clearFilterColumnText);
            clearFilterMenuItem.SetEnabled(context.isFilterApplied);
        }

        this.showAutoFilterMenu = function(context, autoFilterImage) {
            var popupMenu = getAutoFilterPopupMenu();

            preparePopupMenu(popupMenu, this.menuItems, context);
            prepareClearFilterColumnItem(popupMenu, context);

            popupMenu.ShowAtElement(autoFilterImage);
        };

        this.showPopupMenu = function(context) {
            preparePopupMenu(getPopupMenu(), this.menuItems, context);

            getPopupMenu().ShowAtPos(context.mouseX, context.mouseY);
        };
    };
})();