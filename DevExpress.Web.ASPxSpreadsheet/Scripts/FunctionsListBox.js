(function() {

    var SpreadsheetFunctionsListBox = ASPx.CreateClass(ASPxClientListBox, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.functionDescriptions = [];
            this.positionX = null;
            this.positionY = null;
            this.maxScrollPageSize = 10;
        },
        InlineInitialize: function() {
            ASPxClientListBox.prototype.InlineInitialize.call(this);
            this.PreventMouseEvents();
        },
        PreventMouseEvents: function() {
            var mainElement = this.GetMainElement();
            ASPx.Evt.AttachEventToElement(mainElement, "mousedown", ASPx.Evt.PreventEventAndBubble);
            ASPx.Evt.AttachEventToElement(mainElement, "mouseup", ASPx.Evt.PreventEventAndBubble);
        },
        GetFocusableInputElement: function() {
            return null;
        },
        AddFunctionItemsByFilter: function(filter) {
            this.BeginUpdate();
            ASPx.Data.ForEach(ASPxClientSpreadsheet.Functions, function(currentFunction) {
                if(filter(currentFunction))
                    this.AddFunctionItem(currentFunction);
            }.aspxBind(this));
            this.UpdateHeightAfterItemsAdding();
            this.EndUpdate();
        },
        AddFunctionItem: function(spreadsheetFunction) {
            var itemIndex = this.AddItem(spreadsheetFunction.name);
            this.functionDescriptions[itemIndex] = spreadsheetFunction.description;
        },
        ClearItems: function() {
            ASPxClientListBox.prototype.ClearItems.call(this);
            ASPx.Data.ArrayClear(this.functionDescriptions);
        },
        OnItemSelectionChanged: function(index, selected) {
            ASPxClientListBox.prototype.OnItemSelectionChanged.call(this, index, selected);
            if(index !== -1 && selected)
                this.UpdateFunctionDescriptionHintState(index);
        },
        UpdateFunctionDescriptionHintState: function(selectedIndex) {
            var mainElement = this.GetMainElement();
            var newHintText = this.functionDescriptions[selectedIndex];
            if(newHintText === "") {
                this.HideDescription();
                return;
            }
            var selectedItemElement = this.GetItemElement(selectedIndex);
            setTimeout(function() {
                var newHintPosX = ASPx.GetAbsoluteX(mainElement) + mainElement.offsetWidth;
                var newHintPosY = ASPx.GetAbsoluteY(selectedItemElement);
                this.ShowDescription(newHintText, newHintPosX, newHintPosY);
            }.aspxBind(this), 0);
        },
        CorrectHeight: function() { },
        UpdateHeightAfterItemsAdding: function() {
            this.GetMainElement().style.height = "0px";
            var scrollDiv = this.GetScrollDivElement();
            var itemCount = this.GetItemCount();
            var hasVerticalScrollbar = itemCount > this.maxScrollPageSize;
            var listTableHeight = this.GetListTableHeight();
            var maxScrollDivHeight = (listTableHeight / itemCount) * this.maxScrollPageSize;
            var requiredScrollDivHeight = hasVerticalScrollbar ? maxScrollDivHeight : listTableHeight;
            scrollDiv.style.height = requiredScrollDivHeight + "px";
            var requiredListBoxHeight = scrollDiv.offsetHeight + ASPx.GetTopBottomBordersAndPaddingsSummaryValue(this.GetMainElement());
            this.InitializePageSize();
            this.GetMainElement().style.height = requiredListBoxHeight + "px";
        },
        ShowAtPos: function(x, y) {
            var mainElement = this.GetMainElement();
            ASPx.SetElementVisibility(mainElement, true);
            if(this.positionX !== x || this.positionY !== y) {
                ASPx.SetAbsoluteX(mainElement, x);
                ASPx.SetAbsoluteY(mainElement, y);
                this.positionX = x;
                this.positionY = y;
            }
        },
        Hide: function() {
            var mainElement = this.GetMainElement();
            ASPx.SetElementDisplay(mainElement, false);
            ASPx.SetElementVisibility(mainElement, false);
            this.HideDescription();
        },
        ShowDescription: function(text, posX, posY) {
            ASPxClientSpreadsheet.HintManager.ShowHint(this.GetMainElement().parentNode,
                this.name, text, posX, posY, this.functionDescriptionHintStyle, function(hint) {
                    return this.GetMainElement().offsetWidth + hint.offsetWidth + ASPx.GetLeftRightMargins(hint);
                }.aspxBind(this));
        },
        HideDescription: function() {
            ASPxClientSpreadsheet.HintManager.HideHint(this.GetMainElement().parentNode, this.name);
        },
    });

    ASPx.SpreadsheetFunctionsListBox = SpreadsheetFunctionsListBox;
})();