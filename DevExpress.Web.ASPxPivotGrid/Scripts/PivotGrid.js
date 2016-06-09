(function(){

ASPx.pivotGrid_FilterPopupSize = [];
var ASPxClientPivotGridControlCollection = ASPx.CreateClass(ASPxClientControlCollection, {
    constructor: function () {
        this.constructor.prototype.constructor.call(this);
    },
    GetCollectionType: function(){
        return "Pivot";
    },
    Remove: function(pivot) {

        if(ASPx.pivotGrid_FilterPopupSize[pivot.name])
            delete ASPx.pivotGrid_FilterPopupSize[pivot.name];

        ASPxClientControlCollection.prototype.Remove.call(this, pivot);
    },
    OnMouseMove: function(evt) {
        this.ForEachControl(function(control){
        	control.OnMouseMove(evt);
        });
    }
});
ASPxClientPivotGridControlCollection.GetPivotGridControlCollection = function () {
    return aspxGetPivotGridControlCollection();
};

var pivotGrid_GetControlCollection = null;
function aspxGetPivotGridControlCollection() {
    if (pivotGrid_GetControlCollection == null)
        pivotGrid_GetControlCollection = new ASPxClientPivotGridControlCollection();
    return pivotGrid_GetControlCollection;
}

var ASPxRect = ASPx.CreateClass(null, {
    constructor: function(left, top, right, bottom) {
        if(ASPx.IsExistsElement(arguments[0])) {
            this.left = ASPx.GetAbsoluteX(arguments[0]);
            this.top = ASPx.GetAbsoluteY(arguments[0]);
            this.right = this.left + (arguments[0]).offsetWidth;
            this.bottom = this.top + (arguments[0]).offsetHeight;
        }
        else {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
    },
    Contains: function(x, y) {
        return this.left < x && x < this.right && this.top < y && y < this.bottom;
    },
    Intersects: function(rect) {
        if(rect.left > this.right || rect.top > this.bottom ||
			this.left > rect.right || this.top > rect.bottom) return false;
        return true;
    },
    ToString: function() {
        return "{" + this.left + "," + this.top + "," + this.right + "," + this.bottom + "}";
    }
});

ASPx.GetBounds = function(elem) {
    var left = ASPx.GetAbsoluteX(elem),
		top = ASPx.GetAbsoluteY(elem),
		right = left + elem.offsetWidth,
		bottom = top + elem.offsetHeight;
    return new ASPxRect(left, top, right, bottom);
}

var ASPxClientPivotGridGroup = ASPx.CreateClass(null, {
    constructor: function(pivotName, fields) {
        this.pivotName = pivotName;
        this.fields = fields;
        this.count = fields.length;
    },
    ContainsField: function(fieldId) {
        return this.IndexOf(fieldId) >= 0;
    },
    IsInnerField: function(fieldId) {
        var index = this.IndexOf(fieldId);
        return index > 0 && index < this.count - 1 && this.count >= 2;
    },
    IndexOf: function(fieldId) {
        for(var i = 0; i < this.count; i++) {
            if(this.GetFieldId(i) == fieldId)
                return i;
        }
        return -1;
    },
    GetFieldId: function(index) {
        return this.pivotName + "_" + this.fields[index];
    }
});

var ASPxCheckBoxHelper = ASPx.CreateClass(null, {
    IsNative: function(checkBox) {
        if(checkBox.GetChecked)
            return false;
        return true;
    },
    Find: function(id) {
        var checkBox = this.Get(id, false);
        return (checkBox) ? checkBox : this.Get(id, true);
    },
    Get: function(id, isNative) {
        return isNative ? ASPx.GetElementById(id) : ASPx.GetControlCollection().Get(id);
    },
    GetChecked: function(checkBox) {
        return (checkBox.GetChecked) ? checkBox.GetChecked() : checkBox.checked;
    },
    SetChecked: function(checkBox, check) {
        if(checkBox.SetCheckState)
            this.SetCheckedCore(checkBox, check);
        else
            checkBox.checked = check != null ? check : false;
    },
    SetCheckedCore: function(checkBox, check) {
        if(check == null) {
            checkBox.SetCheckState('Indeterminate');
            return;
        }
        if(check)
            checkBox.SetCheckState('Checked');
        else
            checkBox.SetCheckState('Unchecked');
    }
});
var ASPxClientPivotGrid = ASPx.CreateClass(ASPxClientControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        this.pivotGrid_FieldValueCMParams = [];
        this.pivotGrid_HeaderCMParams = [];
        this.pivotGrid_AllowedAreaIds = [];
        this.pivotGrid_Groups = [];

        this.adjustingManager = new ASPx.PivotAdjustingManager(this, this.AdjustPagers);

        this.filterValues = "";
        this.initialFilterValues = "";
        this.filterPersister = "";
        this.filterFieldIndex = "";
        this.headerMenuHideColumn = "";
        this.fieldMenuCellState = "";
        this.DragDropManager = null;
        this.isCallbackInProcess = false;
        this.ResetCallBackStateFlags();
        this.HFPFieldIndex = -1;
        this.HFPFieldDefere = "N";
        this.HFPDefereHeader = null,

        this.pageIndex = 0;
        this.pageCount = 0;
        this.supportGestures = true;

        this.CustomizationFieldsVisibleChanged = new ASPxClientEvent();
        this.AfterCallback = this.EndCallback; //obsolete
        this.BeforeCallback = this.BeginCallback; //obsolete
        this.CellClick = new ASPxClientEvent();
        this.CellDblClick = new ASPxClientEvent();
        this.PopupMenuItemClick = new ASPxClientEvent();
        this.customTargets = new ASPxClientEvent();
        this.checkBoxHelper = new ASPxCheckBoxHelper();
        this.currentEnableFieldListAnimation = false;
        this.forcePerformDeferUpdatesCallback = false;
    },

    HeaderFilterPopupSuffix: "_DXHFP",
    HeaderFilterButtonClassName: "dxpg__hfb",

    GetMainElement: function() { return ASPx.GetElementById(this.name); },
    GetMainDiv: function() { return this.GetChildElement("MTD"); },
    GetMainTable: function() { return this.GetChildElement("MT"); },
    GetEmptyAreaCell: function(intAreaID) { return this.GetChildElement("pgArea" + intAreaID); },
    GetHeadersTable: function(stringAreaID) { return this.GetChildElement("ACC" + stringAreaID); },
    GetFilterWindowContent: function() { return this.GetChildElement("FPC"); },

    GetArrowDragDownImage: function() { return this.GetChildElement("IADD"); },
    GetArrowDragUpImage: function() { return this.GetChildElement("IADU"); },
    GetArrowDragRightImage: function() { return this.GetChildElement("IADR"); },
    GetArrowDragLeftImage: function() { return this.GetChildElement("IADL"); },
    GetDragHideFieldImage: function() { return this.GetChildElement("IDHF"); },
    GetGroupSeparatorImage: function() { return this.GetChildElement("IGS"); },
    GetResizerImage: function() { return this.GetChildElement("FPWR"); },

    GetCustomizationFieldsWindow: function() {
        if(typeof(ASPx.GetPopupControlCollection) == "undefined") return null;
        return ASPx.GetPopupControlCollection().Get(this.name + "_DXCustFields");
    },
    GetCustomizationFieldsWindowElement: function() {
        var custFieldsWindow = this.GetCustomizationFieldsWindow();
        if(custFieldsWindow)
            return custFieldsWindow.GetWindowElement(-1);
        return null;
    },
    GetCustomizationFieldsWindowContentDiv: function() {
        return this.GetChildElement("dxpgCustFields");
    },
    GetPrefilterWindow: function() {
        if(typeof(ASPx.GetPopupControlCollection) == "undefined") return null;
        return ASPx.GetPopupControlCollection().Get(this.name + "_DXPFCForm");
    },
    GetFilterControl: function() {
        return ASPx.GetControlCollection().Get(this.name + "_DXPFCForm_DXPFC");
    },
    GetDataHeadersPopupCell: function() { return this.GetChildElement("DHPC"); },
    GetDataHeadersPopup: function() {
        if(typeof(ASPx.GetPopupControlCollection) == "undefined") return null;
        return ASPx.GetPopupControlCollection().Get(this.name + "_DHP");
    },
    GetDataHeadersPopupWindowElement: function() {
        var popupControl = this.GetDataHeadersPopup();
        if(popupControl == null) return null;
        return popupControl.GetWindowElement(-1);
    },

    GetTreeView: function() { return ASPx.GetControlCollection().Get(this.name + this.HeaderFilterPopupSuffix + "_treeGFTR"); },
    GetCustomizationTreeView: function() { return ASPx.GetControlCollection().Get(this.name + "_dxpgCustFields_treeCF"); },
    GetCustomizationHeaders: function() { return ASPx.GetElementById(this.name + "_dxpgCustFields_listCF"); },
    GetHeaderFilterPopup: function() { return ASPx.GetControlCollection().Get(this.name + this.HeaderFilterPopupSuffix); },

    IsDataHeadersPopupExists: function() {
        var headersPopup = this.GetDataHeadersPopup();
        if(!ASPx.IsExists(headersPopup)) return false;
        var headersPopupWindow = headersPopup.GetWindowElement(-1);
        return ASPx.IsExists(headersPopupWindow) && ASPx.IsExists(headersPopupWindow.id);
    },
    IsHeadersTable: function(element) {
        if(element == null) return false;
        var id = this.getLastIdPart(element.id);
        if(id.length == 0) return false;
        return id.substr(0, 3) == "ACC";
    },
    IsValidDragDropTarget: function(element) {
        var id = this.getLastIdPart(element.id);
        if(id.length == 0) return false;
        var lastChar = id.charAt(id.length - 1);
        return lastChar >= '0' && lastChar <= '9' &&
			    (id.indexOf("pgGroupHeader") >= 0 || (id.indexOf("pgHeader") >= 0 && !this.isInnerRowAreaGroupField(element.id)));
    },
    InlineInitialize: function() {
        this.constructor.prototype.InlineInitialize.call(this);
        this.adjustingManager.OnNewMarkup();
    },
    Initialize: function() {
        this.constructor.prototype.Initialize.call(this);
        ASPxClientPivotGridControlCollection.GetPivotGridControlCollection().Add(this);

        this.SubscribeFilterPopupEvents();
        var resizerImage = this.GetResizerImage();
        if(resizerImage != null)
            resizerImage.ondragstart = function() { this.releaseCapture(); return false; };
        this.AfterCallBackInitialize();
        this.InitializeGroupFilterTreeView();
        this.InitializeCustomizationTreeView();
        this.InitializeHeaderFilterPopup();
        this.InitializeHeaderFilterPopupEvents();
    },
    InitializeHeaderFilterPopupEvents: function() {
        var filterPopup = this.GetHeaderFilterPopup();
        if(!filterPopup)
            return;

        filterPopup.PopUp.AddHandler(function() { this.OnPopUpHeaderFilterWindow(); } .aspxBind(this));
    },
    InitializeHeaderFilterPopup: function() {
        var filterPopup = this.GetHeaderFilterPopup();
        if(!filterPopup)
            return;

        var buttons = this.GetHeaderFilterButtons();
        for(var i = 0; i < buttons.length; i++)
            filterPopup.AddPopupElement(buttons[i]);
    },
    GetHeaderFilterButtons: function() {
        var buttons = [];
        this.PopulateHeaderFilterButtons(this.GetCustomizationFieldsWindowContentDiv(), buttons);
        this.PopulateHeaderFilterButtons(this.GetHeadersTable("RowArea"), buttons);
        this.PopulateHeaderFilterButtons(this.GetHeadersTable("DataArea"), buttons);
        this.PopulateHeaderFilterButtons(this.GetHeadersTable("ColumnArea"), buttons);
        this.PopulateHeaderFilterButtons(this.GetHeadersTable("FilterArea"), buttons);
        return buttons;
    },
    PopulateHeaderFilterButtons: function(container, buttons) {
        if(!container) return;
        var images = container.getElementsByTagName("IMG");
        for(var i = 0; i < images.length; i++) {
            var image = images[i];
            if(ASPx.ElementContainsCssClass(image, this.HeaderFilterButtonClassName))
                buttons.push(image);
        }
    },
    GetHeaderElementByHeaderFilterButton: function(element) {
        var buttonId = this.getHeaderFilterButtonIDByButton(element);
        var headerId = buttonId.substr(0, buttonId.length - 1);
        var header = ASPx.GetElementById(headerId);
        if(header == null)
            header = ASPx.GetElementById(buttonId);
        return header;
    },
    GetFieldIndexByHeaderFilterButton: function(element) {
        var buttonId = this.getHeaderFilterButtonIDByButton(element);
        var headerId = buttonId.substr(0, buttonId.length - 1);
        return this.GetNumberFromEndOfString(headerId);
    },
    getHeaderFilterButtonIDByButton: function (element) {
        if(!element)
            return "";
        var level = 0;
        while(level < 4) {
            if(element.id)
                return element.id;
            element = element.parentNode;
            level++;
        }
        return "";
    },
    GetPopupElementIndex: function(headerFilterButton) {
        if(!headerFilterButton)
            return -1;
        var filterPopup = this.GetHeaderFilterPopup();
        var popupElements = filterPopup.GetPopupElementList(-1);
        for(var i = 0; i < popupElements.length; i++) {
            if(popupElements[i] === headerFilterButton)
                return i;
        }
        return -1;
    },
    GetNumberFromEndOfString: function(st) {
        var value = -1;
        if(!st)
            return value;
        var n = st.length - 1;
        while(parseInt(st.substr(n), 10) >= 0) {
            value = parseInt(st.substr(n), 10);
            n--;
        }
        return value;
    },
    SubscribeFilterPopupEvents: function() {
        var filterPopup = this.GetHeaderFilterPopup();
        filterPopup.Resize.AddHandler(function(s) {
            ASPx.pivotGrid_FilterPopupSize[this.name][this.HFPFieldIndex] = [s.GetWidth(), s.GetHeight()];
        }.aspxBind(this));
        filterPopup.Closing.AddHandler(function(s) {
            if(s.GetContentHtml().length > 100000 || this.isFilterValuesChanged()) {
                this.ResetFilterCache();
                this.resetFilterValues();
                window.setTimeout(function() { s.SetContentHtml("") }, 0);
            }
        }.aspxBind(this));
    },
    InitializeGroupFilterTreeView: function() {
        var treeView = this.GetTreeView();
        if(treeView)
            treeView.InitPivotGridCallbacks(this);
    },
    InitializeCustomizationTreeView: function() {
        var customizationTreeView = this.GetCustomizationTreeView();
        if(customizationTreeView)
            customizationTreeView.InitializePivotGrid(this, this.isDeferUpdatesChecked());
    },
    OnMouseMove: function(evt) {
        if(!this.IsDataHeadersPopupExists())
            return;
        var headersPopup = this.GetDataHeadersPopup();
        if(!headersPopup.IsVisible())
            return;
        var pe = headersPopup.GetWindowElement(-1);
        var rect = new ASPxRect(pe);
        if(!rect.Contains(ASPx.Evt.GetEventX(evt), ASPx.Evt.GetEventY(evt)))
            headersPopup.Hide();
        if(ASPx.Browser.WebKitTouchUI)
            evt.preventDefault();
    },
    AfterInitialize: function() {
        this.constructor.prototype.AfterInitialize.call(this);
        this.AfterInitializeCore();
    },
    AfterInitializeCore: function() {
        if(this.IsDataHeadersPopupExists()) {
            var headersPopup = this.GetDataHeadersPopup();
            headersPopup.SetSize(1, 1);
            if(headersPopup.IsVisible())
                headersPopup.Hide();
        }
        if(!this.isFilterValuesCallBack) {
            this.FixHeaderImageDrag(this.GetHeadersTable("RowArea"));
            this.FixHeaderImageDrag(this.GetHeadersTable("ColumnArea"));
            this.FixHeaderImageDrag(this.GetHeadersTable("FilterArea"));
        }
        this.InitExcelCustForm();
        this.UpdateExcelCustForm();
        this.ResetCallBackStateFlags();
    },
    AdjustControlCore: function() {
        ASPxClientControl.prototype.AdjustControlCore.call(this);
        if(!this.isFilterValuesCallBack) {
            if(this.adjustingManager.HasAdjustingLogic()) {
                this.adjustingManager.Adjust();
                this.AfterCallBackInitialize();
            }
            else {
                this.AdjustPagerControls();
            }
        }
    },
    AdjustPagers: function() {
        if(typeof(ASPx.GetPagersCollection) != "undefined") {
            ASPx.GetPagersCollection().ForEachControl(function(pager) {
                pager.AdjustControl();
            });
        }
    },
    NeedCollapseControlCore: function() {
        return this.adjustingManager.HasAdjustingLogic();
    },
    BrowserWindowResizeSubscriber: function() {
        return this.adjustingManager.HasAdjustingLogic();
    },
    OnBrowserWindowResize: function() {
        this.AdjustControlCore();
    },
    AfterCallBackInitialize: function() {
        this.AssignContextMenus(this.pivotGrid_FieldValueCMParams, pivotGrid_FieldValueContextMenuHandler);
        this.AssignContextMenus(this.pivotGrid_HeaderCMParams, pivotGrid_HeaderContextMenuHandler);
    },
    SetRenderOptions: function(options) {
        this.adjustingManager.SetRenderOptions(options);
    },
    SetCallBackStateFlags: function() {
        this.isFilterValuesCallBack = true;
    },
    ResetCallBackStateFlags: function() {
        this.isFilterValuesCallBack = false;
    },
    FixHeaderImageDrag: function(fobject) {
        if(fobject == null) return;
        var images = ASPx.GetNodesByTagName(fobject, "img");
        if(!images || !images.length) return;
        for(var i = 0; i < images.length; i++)
            if(ASPx.ElementContainsCssClass(images[i], "Button"))
                images[i].ondragstart = function() { return false };
    },
    AssignContextMenus: function(cmParams, handler) {
        if(ASPx.IsExists(cmParams) && ASPx.IsExists(cmParams[this.name])) {
            var params = cmParams[this.name];
            for(var i = 0; i < params.length; i++) {
                var id = params[i][0];
                var elem = this.GetChildElement(id);
                if(elem != null) {
                    elem.contextMenuParams = params[i];
                    elem.pivotClientID = this.name;
                    ASPx.Evt.AttachEventToElement(elem, "contextmenu", handler);
                }
            }
        }
    },
    SetHFPOkButtonEnabled: function(enabled) {
        var okBtn = ASPx.GetControlCollection().Get(this.GetHeaderFilterPopup().cpOkButtonID);
        if(okBtn)
            okBtn.SetEnabled(enabled);
    },
    getDefereText: function(header){
        return this.isFieldListElement(header) && this.isDeferUpdatesChecked() ? "D" : "N";
    },
    OnPopUpHeaderFilterWindow: function() {
        var filterPopup = this.GetHeaderFilterPopup();
        var headerFilterButton = filterPopup.GetCurrentPopupElement();
        var fieldIndex = this.GetFieldIndexByHeaderFilterButton(headerFilterButton);
        var header = this.GetHeaderElementByHeaderFilterButton(headerFilterButton);
        if(header)
            filterPopup.UpdatePositionAtElement(header);
        if(fieldIndex == -1 || this.HFPFieldIndex == fieldIndex && this.HFPFieldDefere == this.getDefereText(header))
            return;

        var size = this.GetFilterPopupSize(fieldIndex);
        if(size && size.length == 2)
            filterPopup.SetSize(size[0], size[1]);

        this.SetCallBackStateFlags();
        var addPars = "";
        if(header && this.isFieldListElement(header) && this.isDeferUpdatesChecked())
            addPars = "|D";
        this.HFPFieldIndex = fieldIndex;
        this.CreateCallback("FS|" + fieldIndex + addPars);

        filterPopup.SetContentHtml('');
        var element = this.CreateLoadingPanelInsideContainer(filterPopup.GetContentContainer(-1), false, true, true);
        ASPx.RemoveBordersAndShadows(element);
        this.SetHFPOkButtonEnabled(false);
    },
    ShowFilterPopup: function(headerId) {
        var filterPopup = this.GetHeaderFilterPopup();
        var buttons = [];
        this.PopulateHeaderFilterButtons(ASPx.GetElementById(headerId), buttons);
        var popupElementIndex = this.GetPopupElementIndex(buttons[0]);
        if(popupElementIndex == -1)
            return;
        if(filterPopup.IsVisible())
            filterPopup.Hide();
        filterPopup.Show(popupElementIndex);
    },
    GetFilterPopupSize: function(index) {
        if(ASPx.pivotGrid_FilterPopupSize[this.name] == null)
            return null;
        if(ASPx.pivotGrid_FilterPopupSize[this.name][index] == null)
            return ASPx.pivotGrid_FilterPopupSize[this.name]["default"];
        else return ASPx.pivotGrid_FilterPopupSize[this.name][index];
    },
    SaveContentDivScrollTop: function() {
        var contentDiv = this.GetCustomizationFieldsWindowContentDiv();
        if(contentDiv != null)
            this.contentDivScrollTop = contentDiv.scrollTop;
        else
            this.contentDivScrollTop = -1;
    },
    RestoreContentDivScrollTop: function() {
        if(!ASPx.IsExists(this.contentDivScrollTop) || this.contentDivScrollTop < 0) return;
        var contentDiv = this.GetCustomizationFieldsWindowContentDiv();
        if(contentDiv != null) {
            contentDiv.scrollTop = this.contentDivScrollTop;
            this.contentDivScrollTop = -1;
        }
    },
    performCallbackInternalBase: function(el, arg, isOwnerCallBack, command) {
        if(!isOwnerCallBack) {
            this.ClearCallbackOwner();
        }
        this.isCallbackInProcess = true;
        if(isOwnerCallBack || this.callBacksEnabled) {
            this.OnBeginCallback(el);
            this.CreateCallback(arg, command);
        }
        else
            this.SendPostBack(arg);
    },
    TestPerformDragHeaderCallback: function(arg) {
        this.PerformCallbackInternal(this.GetMainTable(), arg);
    },
    PerformCallbackInternal: function(el, arg, command) {
        var value = new String(arg);
        var sIndex = value.indexOf("|");
        var id = value.substr(0, sIndex);
        this.forcePerformDeferUpdatesCallback = !((id == "S" || id == "DF" || id == "FL") && this.isDeferUpdatesChecked());
        this.performCallbackInternalBase(el, arg, false, command);
    },
    ClearCallbackOwner: function() {
        this.callbackOwner = null;
    },
    OnCallback: function(resultObj) {
        this.UpdateStateObjectWithObject(resultObj.stateObject);

        var result = resultObj.result;
        var isOwnerCallback = this.callbackOwner != null;
        if(isOwnerCallback) {
            this.callbackOwner.OnCallback(result[1]);
        }
        this.OnInnerCallback(result[0], !isOwnerCallback);
    },
    BeforeDoEndCallback: function() {
        if(this.isGeneralCallback) {
            this.AdjustControlCore();
        }
    },
    OnInnerCallback: function(result, state) {
        this.ChangePrefilterVisibilityClientSize(false);
        var separatorIndex = result.indexOf("|");
        var id;
		if(separatorIndex == -1)
			id = result;
		else
			id = result.substr(0, separatorIndex);
		var argument = result.substr(separatorIndex + 1),
            isGeneral = id == "G";
        if(id == "F") {
            this.PreventCallbackAnimation();
            this.doFilterCallback(argument, state);
        }
        if (id == "DF") {
            this.PreventCallbackAnimation();
            this.doDefereFilterCallback(argument);
        }
        if(isGeneral) {
            this.adjustingManager.OnNewMarkup();
            ASPx.RelatedControlManager.ParseResult(argument);
            var popup = this.GetHeaderFilterPopup();
            if(popup) {
                popup.Hide();
                popup.RemoveAllPopupElements();
                this.InitializeHeaderFilterPopup();
            }
        }
        this.OnEndCallback(isGeneral);
        this.isGeneralCallback = isGeneral;
    },
    DoEndCallback: function() {
        this.BeforeDoEndCallback();
        ASPxClientControl.prototype.DoEndCallback.call(this);
        this.InitializeGroupFilterTreeView();
        this.InitializeCustomizationTreeView();
        this.isCallbackInProcess = false;
        this.ResetCallBackStateFlags();
    },
    ProcessCallbackResult: function(id, html, params) {
        if(params && params.length) {
            switch(params) {
                case 'pivotTable':
                    this.adjustingManager.UpdatePartial(html);
                    break;
                default:
                    var element = ASPx.GetElementById(id);
                    if(ASPx.IsExistsElement(element))
                        element.innerHTML = html;
                    break;
            }
        }
        else{
            this.doGridRefreshCallback(html);
        }
    },
    OnCallbackError: function(result, data) {
        this.constructor.prototype.OnCallbackError.call(this, result, data);
        this.OnEndCallback();
    },
    OnBeginCallback: function(el) {
        this.SaveContentDivScrollTop();
        this.ShowLoadingElements();
    },
    ShowLoadingPanel: function() {
        var mainDiv = this.GetMainDiv();
        if(!mainDiv) return;

        this.CreateLoadingPanelWithAbsolutePosition(mainDiv, this.GetLoadingPanelOffsetElement(this.GetMainElement()));
    },
    ShowLoadingDiv: function () {
        this.CreateLoadingDiv(this.GetMainDiv(), this.GetMainElement());
    },
    GetCallbackAnimationElement: function() {
        return this.adjustingManager.GetCallbackAnimationElement();
    },
    OnEndCallback: function() {
        this.AfterInitializeCore();
    },
    SendTreeViewCallback: function(treeView, callbackString) {
        this.SetCallBackStateFlags(); // IE6
        this.callbackOwner = treeView;
        var pivotCallbackString = "FC|" + this.filterFieldIndex + '|' + this.filterPersister + '|' + this.filterValues;
        this.performCallbackInternalBase(this.GetMainTable(), pivotCallbackString + '|' + callbackString, true); //this.CreateCallback(pivotCallbackString + '|' + callbackString);
    },
    doDefereFilterCallback: function (argument) {
        if (this.HFPDefereHeader) {
            var list = []
            this.PopulateHeaderFilterButtons(this.HFPDefereHeader, list);
            if (list.length != 0) {
                var image = list[0];
                if (!ASPx.ElementContainsCssClass(image, "pgFilterButtonActive")) {
                    image.className = image.className.replace("pgFilterButton", "pgFilterButtonActive");
                }
            }
            this.HFPDefereHeader = null;
        }
    },
    doFilterCallback: function(argument, changeContent) {
        var values = argument.split("|");
        if (values.length != 5) return;
        if (values[2] == this.HFPFieldIndex) {
		    this.filterValues = values[0];
		    this.filterPersister = values[1];
		    this.filterFieldIndex = values[2];
		    //this.HFPFieldIndex = this.filterFieldIndex;
		    this.HFPFieldDefere = values[3];
		    if(changeContent) {//values[4] != ""
		        this.initialFilterValues = values[0];
		        this.GetHeaderFilterPopup().SetContentHtml(values[4], this.enableCallbackAnimation);
		    }
		    this.SetHFPOkButtonEnabled(true);
        }
    },
    doGridRefreshCallback: function(argument) {
        this.ResetFilterCache();
        var mainDiv = this.GetMainDiv();
        if(mainDiv == null) return;
        ASPx.SetInnerHtml(mainDiv, argument);
    },
    isAllFilterValuesChecked: function() {
        var hasCheckedItems = this.filterValues.indexOf('T') != -1;
        var hasUncheckedItems = this.filterValues.indexOf('F') != -1;
        if(hasCheckedItems && hasUncheckedItems) return null;
        return hasCheckedItems;
    },
    isFilterValuesChanged: function() {
        return this.filterValues != this.initialFilterValues;
    },
    resetFilterValues: function() {
        this.initialFilterValues = this.filterValues;
    },
    fieldFilterValueChanged: function(index) {
        var showAllCheckBox = this.checkBoxHelper.Find(this.name + this.HeaderFilterPopupSuffix + '_FTRIAll');
        if(index < 0) {
            var isNative = this.checkBoxHelper.IsNative(showAllCheckBox);
            var isShowAllChecked = this.checkBoxHelper.GetChecked(showAllCheckBox);
            var newFilterValues = '', newChar = isShowAllChecked ? 'T' : 'F';
            var i = 0, checkBox;
            while(ASPx.IsExists(checkBox = this.checkBoxHelper.Get(this.name + this.HeaderFilterPopupSuffix + '_FTRI' + i, isNative))) {
                if(!isNative && !checkBox.GetMainElement())
                    break;
                this.checkBoxHelper.SetChecked(checkBox, isShowAllChecked);
                newFilterValues += newChar;
                i++;
            }
            this.filterValues = newFilterValues;
        } else {
            var oldChar = this.filterValues.charAt(index);
            this.UpdateFilterValues(index, 1, oldChar != 'T');
            this.checkBoxHelper.SetChecked(showAllCheckBox, this.isAllFilterValuesChecked());
        }
        this.UpdateFilterButtons();
    },
    UpdateFilterValues: function(index, count, isChecked) {
        var newChar = 'I';
        if(isChecked != null)
            newChar = isChecked ? 'T' : 'F';
        var newValues = newChar;
        for(var i = 1; i < count; i++)
            newValues += newChar;
        this.filterValues = this.filterValues.substr(0, index) + newValues + this.filterValues.substr(index + count, this.filterValues.length - index - count);
    },
    UpdateFilterButtons: function() {
    	this.SetHFPOkButtonEnabled(this.filterValues.indexOf('T') != -1 || this.filterValues.indexOf('I') != -1);
    },
    SelectAllFilterValues: function(checked) {
        var newChar = checked ? 'T' : 'F';
        var newValues = '';
        for(var i = 0; i < this.filterValues.length; i++) {
            newValues += newChar;
        }
        this.filterValues = newValues;
    },
    applyFilter: function() {
        var isChanged = this.isFilterValuesChanged() ? 'T' : 'F';// before filter will be hidden
        var filterPopup = this.GetHeaderFilterPopup()
        var header = this.GetHeaderElementByHeaderFilterButton(filterPopup.GetCurrentPopupElement());
        var filterString;
        if (header && this.isFieldListElement(header) && this.isDeferUpdatesChecked()) {
            filterString = "DF";
            if (isChanged == "T")
                this.HFPDefereHeader = header;
        }
        else
            filterString = "F";
        filterPopup.Hide();
        this.PerformCallbackInternal(this.GetMainTable(), filterString + '|' + this.filterValues + '|' + this.filterPersister + '|' + this.filterFieldIndex + '|' + isChanged);
    },
    headerMouseDown: function(root, e) {
        if(!ASPx.Evt.IsLeftButtonPressed(e) || this.isCallbackInProcess) return;
        var drag = new ASPx.DragHelper(e, root, true);
        if(ASPx.Browser.MacOSMobilePlatform)
            drag.onDoClick = function(arg) { this.dnDHelperClickedHeader = true; } .aspxBind(this);
        this.DragDropManager = new ASPx.PivotDragManager(this, drag);
    },
    getFilterAreaFieldList: function() {
        return ASPx.GetNodesByPartialClassName(this.GetCustomizationFieldsWindowContentDiv(), "dxpgCustFieldsFilterAreaHeaders")[0];
    },
    getColumnAreaFieldList: function() {
        return ASPx.GetNodesByPartialClassName(this.GetCustomizationFieldsWindowContentDiv(), "dxpgCustFieldsColumnAreaHeaders")[0];
    },
    getRowAreaFieldList: function() {
        return ASPx.GetNodesByPartialClassName(this.GetCustomizationFieldsWindowContentDiv(), "dxpgCustFieldsRowAreaHeaders")[0];
    },
    getDataAreaFieldList: function() {
        return ASPx.GetNodesByPartialClassName(this.GetCustomizationFieldsWindowContentDiv(), "dxpgCustFieldsDataAreaHeaders")[0];
    },
    isInnerRowAreaGroupField: function(id) {
        var groups = this.getGroups();
        if(groups != null) {
            for(var i = 0; i < groups.length; i++) {
                if(!groups[i].ContainsField(id))
                    continue;
                return groups[i].IsInnerField(id);
            }
        }
        return false;
    },
    isInnerGroupTarget: function(targetElement, isLeft) {
        if(targetElement == null) return false;
        var id = targetElement.id;
        var groups = this.getGroups();
        if(groups == null) return false;
        for(var i = 0; i < groups.length; i++) {
            var index = groups[i].IndexOf(id);
            if(index < 0) continue;
            return groups[i].count >= 2 &&
				((index > 0 && index < groups[i].count - 1) ||
				    (index == 0 && !isLeft) ||
				    (index == groups[i].count - 1 && isLeft));
        }
        return false;
    },
    getLastIdPart: function(id) {
        var separatorIndex = id.lastIndexOf('_');
        return separatorIndex >= 0 ? id.substr(separatorIndex + 1) : id;
    },
    getGroups: function() {
        return this.pivotGrid_Groups[this.name];
    },
    getField: function(group, index) {
        return this.GetChildElement(group.fields[index]);
    },
    headerClick: function(element) {
        if((element.id.indexOf("sorted") > 0 || element.id.indexOf("pgGroupHeader") > 0) && (this.dnDHelperClickedHeader || !ASPx.Browser.MacOSMobilePlatform)) {
            this.ResetFilterCache(this.filterFieldIndex);
            this.PerformCallbackInternal(element, 'S|' + element.id);
        }
    },
    ResetFilterCache: function(indexToReset) {
        if(!ASPx.IsExists(indexToReset) || indexToReset == this.HFPFieldIndex)
            this.HFPFieldIndex = -1;
    },
    cloneGroup: function(group) {
        var separatorWidth = this.getSeparatorWidth(group);
        var table = document.createElement("table");
        table.cellPadding = 0;
        table.cellSpacing = 0;
        var row = table.insertRow(0);
        for(var i = 0; i < group.count; i++) {
            var header = this.getField(group, i);
            if(header == null) continue;
            var cell = row.insertCell(row.cells.length);
            cell.appendChild(this.cloneHeader(header));
            if(i != group.count - 1) {
                cell = row.insertCell(row.cells.length);
                cell.appendChild(this.cloneGroupSeparator(separatorWidth));
            }
        }
        return table;
    },
    cloneHeader: function(header) {
        var clone = header.cloneNode(true);
        clone.style.width = header.offsetWidth + "px";
        return clone;
    },
    cloneGroupSeparator: function(separatorWidth) {
        // this.GetGroupSeparatorImage().cloneNode() doesn't work in IE7- produces empty image
        var groupSeparator = this.GetGroupSeparatorImage();
        var clone = document.createElement("img");
        clone.src = groupSeparator.src;
        clone.width = separatorWidth;
        clone.height = groupSeparator.height;
        return clone;
    },
    getSeparatorWidth: function(group) {
        if(group.count < 2) return 0;
        var header0 = this.getField(group, 0);
			header1 = this.getField(group, 1);
        if(header0 == null || header1 == null) return 0;
        return ASPx.GetAbsoluteX(header1) - ASPx.GetAbsoluteX(header0) - header0.offsetWidth;
    },
    IsTargetElementArea: function(targets) {
        return targets.targetElement != null && targets.targetElement.id.indexOf("_pgArea") > -1;
    },
    isFieldListHeader: function(element) {
        return ASPx.ElementContainsCssClass(element, "dxpgHeader") && this.isFieldListElement(element);
    },
    isFieldListElement: function(element) {
        return element != null && element.id.indexOf("dxpgCustFields") != -1;
    },
    IsFieldListTargetAllowed: function(element) {
        if(this.isFieldListHeader(element)) {
            var scrolledList = ASPx.GetParentByPartialClassName(element, "dxpgFLListDiv");
            var headerTop = ASPx.GetAbsolutePositionY(element);
            var listY = ASPx.GetAbsolutePositionY(scrolledList);
            if(headerTop < listY || headerTop + element.offsetHeight - 1 > listY + scrolledList.offsetHeight) return false;
        }
        return true;
    },
    resetDragOverFieldList: function(element) {
        if(element != null)
            element.className = element.className.replace("DragOver", "");
    },
    targetImagesChangeVisibility: function(vis, name) {
        if(this.GetArrowDragDownImage() == null) return;
        if(!vis || name == "v") {
            this.changeElementVisibility(this.GetArrowDragDownImage(), vis);
            this.changeElementVisibility(this.GetArrowDragUpImage(), vis);
        }
        if(!vis || name == "h") {
            this.changeElementVisibility(this.GetArrowDragRightImage(), vis);
            this.changeElementVisibility(this.GetArrowDragLeftImage(), vis);
        }
        if(ASPx.currentDragHelper != null) {
            ASPx.currentDragHelper.removeElementFromDragDiv();
        }
    },
    changeElementVisibility: function(elem, vis) {
        elem.style.visibility = vis ? "visible" : "hidden";
        elem.style.display = vis ? "inline" : "none";
    },
    getFieldListFields: function() {
        if(this.GetCustomizationFieldsWindowContentDiv())
            return ASPx.GetNodesByPartialClassName(this.GetCustomizationFieldsWindowContentDiv(), "dxpgCustFieldsFieldList")[0];
    },
    setVerticalDragImagePosition: function(el, isLeft) {
        var mainElement = this.GetMainElement();
        var windowElem = this.GetDataHeadersPopupWindowElement();
        var prevElementZIndex = windowElem ? windowElem.style.zIndex : mainElement.style.zIndex;
        var arrowUp = this.GetArrowDragUpImage();
        var arrowDown = this.GetArrowDragDownImage();
        var left = ASPx.GetAbsoluteX(el);
        var top = ASPx.GetAbsoluteY(el);
        arrowDown.style.zIndex = prevElementZIndex + 1;
        arrowUp.style.zIndex = prevElementZIndex + 1;
        if (!isLeft) {
            left += el.offsetWidth;
        }
        this.targetImagesChangeVisibility(true, "v");

        var x = left - (arrowDown.offsetWidth / 2);
        x = isLeft ? Math.floor(x) : Math.ceil(x);
        ASPx.SetAbsoluteX(arrowDown, x);
        ASPx.SetAbsoluteY(arrowDown, top - arrowDown.offsetHeight);
        ASPx.SetAbsoluteX(arrowUp, x);
        ASPx.SetAbsoluteY(arrowUp, top + el.offsetHeight);
    },
    setHorizontalDragImagePosition: function(el, isTop) {
        var arrowLeft = this.GetArrowDragLeftImage();
        var arrowRight = this.GetArrowDragRightImage();
        var left = 0, top = 0;
        if(el.id.indexOf("DHP") < 0) {
            left = ASPx.GetAbsoluteX(el);
            top = ASPx.GetAbsoluteY(el);
        } else {
            var windowElem = this.GetDataHeadersPopupWindowElement();
            left = ASPx.GetAbsoluteX(el);
            top = ASPx.GetAbsoluteY(el);
            arrowDown.style.zIndex = windowElem.style.zIndex + 1;
            arrowUp.style.zIndex = windowElem.style.zIndex + 1;
        }
        if(!isTop)
            top += el.offsetHeight;
        this.targetImagesChangeVisibility(true, "h");

        top -= (arrowLeft.offsetHeight / 2);
        top = isTop ? Math.floor(top) : Math.ceil(top);
        ASPx.SetAbsoluteX(arrowRight, left - arrowLeft.offsetWidth);
        ASPx.SetAbsoluteY(arrowRight, top);
        ASPx.SetAbsoluteX(arrowLeft, left + el.offsetWidth);
        ASPx.SetAbsoluteY(arrowLeft, top);
    },
    isVerticalElement: function(el) {
        if(el == null) return false;
        return this.isFieldListElement(el) || el.pgddVerticalElement;
    },
    setPivotDragImagePosition: function(el, isLeft) {
        if(this.isVerticalElement(el))
            this.setHorizontalDragImagePosition(el, isLeft);
        else
            this.setVerticalDragImagePosition(el, isLeft);
    },
    ScrollFieldList: function(event) {
        var target = this.DragDropHelper.targets.targetElement;
        var pivotGrid = this.DragDropHelper.pGrid;
        if(!pivotGrid.isFieldListElement(target))
            return;
        var wheelDelta = 0;
        if(!event)
            event = window.event;
        if(event.wheelDelta)
            wheelDelta = event.wheelDelta / 120;
        else if(event.detail)
            wheelDelta = -event.detail / 3;
        if(wheelDelta) {
            var targetDiv;
            if(target.id.indexOf("pgHeader") < 0 && target.id.indexOf("pgGroupHeader") < 0)
                targetDiv = pivotGrid.getFielListArrowsHorElement(ASPx.GetNodesByPartialClassName(target, "dxpgFLListDiv")[0]);
            else
                targetDiv = pivotGrid.getFielListArrowsHorElement(ASPx.GetParentByPartialClassName(target, "dxpgFLListDiv"));
            if(targetDiv == null) return;
            if(wheelDelta > 0 && targetDiv.scrollTop > 0) {
                if(targetDiv.scrollTop < 20)
                    targetDiv.scrollTop = 0;
                else
                    targetDiv.scrollTop -= 20;
            }
            if(wheelDelta < 0 && targetDiv.scrollTop + targetDiv.clientHeight < targetDiv.scrollHeight)
                targetDiv.scrollTop += 20;
            if(event.preventDefault) {
                event.preventDefault();
            }
            event.returnValue = false;
        }
    },
    updateListElements: function(targets, event) {
        for(var i = targets.list.length - 1; i >= 0; i--) {
            if(!this.IsFieldListTargetAllowed(targets.list[i].targetElement)) {
                this.ListTargets.push(targets.list[i].targetElement);
                targets.removeElement(targets.list[i].element);
            } else {
                if(this.isFieldListElement(targets.list[i].targetElement))
                    targets.list[i].absoluteY = ASPx.GetAbsoluteY(targets.list[i].targetElement);
            }
        }
        for(var i = this.ListTargets.length - 1; i >= 0; i--) {
            if(this.IsFieldListTargetAllowed(this.ListTargets[i])) {
                var target = new ASPx.CursorTarget(this.ListTargets[i]);
                target.element = target.targetElement.parentNode;
                targets.list.unshift(target);
                this.ListTargets.splice(i, 1);
            }
        }
        targets.doTargetChanged(event);
    },
    getFielListArrowsHorElement: function(element) {
        return ASPx.GetNodeByTagName(ASPx.GetNodeByTagName(element, "div", 0), "div", 0);
    },
    setFieldListDragImagePosition: function(el, isLeft) {
        this.LastHoverFieldList = ASPx.GetParentByPartialClassName(el, "dxpgCustFields");
        var horEl = el, vertEl = el;
        var left = 0, top = 0;
        if(el.id.indexOf("pgHeader") < 0 && el.id.indexOf("pgGroupHeader") < 0) {
            var el1 = ASPx.GetNodeByTagName(el, "table", 0);
            var el2 = ASPx.GetNodesByPartialClassName(el, "dxpgFLListDiv")[0];
            if(el1.offsetHeight < el2.offsetHeight)
                vertEl = el1;
            else
                vertEl = el2;
            horEl = this.getFielListArrowsHorElement(el2);
        } else {
            var lists = ASPx.GetParentByPartialClassName(el, "dxpgFLListDiv");
            horEl = this.getFielListArrowsHorElement(lists);
        }
        var leftArrow = this.GetArrowDragRightImage();
        var rightArrow = this.GetArrowDragLeftImage();
        var scrollWidth;
        if(horEl.clientHeight >= horEl.scrollHeight)
            scrollWidth = 0;
        else
            scrollWidth = ASPx.GetVerticalScrollBarWidth();
        left = ASPx.GetAbsoluteX(horEl);
        top = ASPx.GetAbsoluteY(vertEl);
        if(!isLeft)
            top += vertEl.offsetHeight;
        this.targetImagesChangeVisibility(true, "h");
        var windowElem = this.GetCustomizationFieldsWindowElement();
        rightArrow.style.zIndex = windowElem ? windowElem.style.zIndex + 1 : 12000;
        leftArrow.style.zIndex = windowElem ? windowElem.style.zIndex + 1 : 12000;
        var y = top - (leftArrow.offsetHeight / 2);
        y = isLeft ? Math.floor(y) : Math.ceil(y);
        if(ASPx.Browser.WebKitFamily) {
            if(!isLeft)
                y += 2;
            left += 1;
        } else {
            if(isLeft)
                y -= 1;
            else
                y += 1;
        }
        ASPx.SetAbsoluteX(leftArrow, (left - leftArrow.offsetWidth));
        ASPx.SetAbsoluteY(leftArrow, y);
        ASPx.SetAbsoluteX(rightArrow, left + horEl.offsetWidth - scrollWidth);
        ASPx.SetAbsoluteY(rightArrow, y);
    },
    setDragImagesPosition: function(el, isLeft) {
        this.LastHoverFieldList = null;
        this.targetImagesChangeVisibility(false);
        if(el == this.getFieldListFields()) {
            ASPx.currentDragHelper.addElementToDragDiv(this.GetDragHideFieldImage());
            this.LastHoverFieldList = el;
        } else {
            if(this.isFieldListElement(el))
                this.setFieldListDragImagePosition(el, isLeft);
            else
                this.setPivotDragImagePosition(el, isLeft);
        }
        if(this.LastHoverFieldList != null)
            this.LastHoverFieldList.className = this.LastHoverFieldList.className + " DragOver";
    },
    isDeferUpdatesChecked: function() {
        var input = this.GetChildElement("dxpgCustFields_dxpgFLDefere_S");
        return input == null ? false : input.value != "U";
    },
    isTreeViewNode: function(element) {
        return element != null && element.id.indexOf("treeCF") != -1;
    },
    PerformDeferUpdatesCallback: function() {
        function SplitHeadersList(fieldsContainer) {
            var fields = ASPx.GetNodesByPartialClassName(fieldsContainer, "dxpgHeader");
            var str = "";
            for(var i = 0; i < fields.length; i++) {
                var lastChar = fields[i].id.charAt(fields[i].id.length - 1);
                if(lastChar >= '0' && lastChar <= '9') {
                    if(str != "")
                        str += ",";
                    str += fields[i].id.substr(fields[i].id.lastIndexOf('_'));
                }
            }
            return str;
        }
        var callbackStr = "FL";
        callbackStr += "|" + SplitHeadersList(this.getRowAreaFieldList());
        callbackStr += "|" + SplitHeadersList(this.getColumnAreaFieldList());
        callbackStr += "|" + SplitHeadersList(this.getFilterAreaFieldList());
        callbackStr += "|" + SplitHeadersList(this.getDataAreaFieldList());
        callbackStr += "|" + SplitHeadersList(this.getFieldListFields());
        this.PerformCallbackInternal(null, callbackStr)
    },
    HideCustomizationFields: function() {
        this.ChangeCustomizationFieldsVisibilityInternal(false, true);
    },
    ShowCustomizationFields: function(animate) {
        this.ChangeCustomizationFieldsVisibilityInternal(true, animate);
    },
    IsCustomizationFieldsVisible: function() {
        var fieldsWindow = this.GetCustomizationFieldsWindow();
        if(fieldsWindow == null) return false;
        return fieldsWindow.IsVisible();
    },
    ChangeCustomizationFieldsVisibilityInternal: function(visible, animate) {
        if(!ASPx.IsExists(visible)) {
            visible = !this.IsCustomizationFieldsVisible();
        }
        var fieldsWindow = this.GetCustomizationFieldsWindow();
        if(fieldsWindow == null) return;
        if(!animate)
            fieldsWindow.LockAnimation();
        this.currentEnableFieldListAnimation = animate;
        if(visible) {
            fieldsWindow.Show();
        } else {
            fieldsWindow.Hide();
        }
        if(!animate)
            fieldsWindow.UnlockAnimation();
    },
    IsPrefilterVisible: function() {
        var prefilterPopup = this.GetPrefilterWindow();
        return prefilterPopup != null && prefilterPopup.GetVisible();
    },
    ShowPrefilter: function() {
        this.ChangePrefilterVisibility(true);
    },
    ApplyPrefilter: function() {
        this.PreventCallbackAnimation();

        var fc = this.GetFilterControl();
        if(fc == null) return;
        fc.Apply(this);
    },
    HidePrefilter: function() {
        this.PreventCallbackAnimation();
        this.ChangePrefilterVisibility(false);
    },
    ChangePrefilterVisibility: function(visible) {
        this.PreventCallbackAnimation();
        this.ChangePrefilterVisibilityClientSize(visible);
        this.PerformCallbackInternal(this.GetMainTable(), "PREFILTER|" + (visible ? "Show" : "Hide"));
    },
    ChangePrefilterVisibilityClientSize: function(visible) {
        if(!visible) {
            var prefilterPopup = this.GetPrefilterWindow();
            if(prefilterPopup != null)
                prefilterPopup.Hide();
        }
    },
    ClearPrefilter: function() {
        this.PerformCallbackInternal(this.GetMainTable(), "PREFILTER|Reset");
    },
    ChangePrefilterEnabled: function() {
        this.PerformCallbackInternal(this.GetMainTable(), "PREFILTER|ChangeEnabled");
    },
    showHeaderMenu: function(e, fieldID) {
        this.headerMenuFieldID = fieldID;
        var source = ASPx.Evt.GetEventSource(e);
        if(source == null)
            return;
        this.headerMenuElementID = source.id;
        if(!this.headerMenuElementID && source.parentNode && source.parentNode.id)
            this.headerMenuElementID = source.parentNode.id;
        var menu = ASPx.GetControlCollection().Get(this.name + "_HM");
        if (menu == null) return;
        this.SetMenuItemVisibilityState(menu, "Hide", this.headerMenuElementID.indexOf("Header") > 0 && this.headerMenuElementID.indexOf("scig") == -1  && this.headerMenuElementID.indexOf("pgdthdr") == -1);
        this.SetMenuItemVisibilityState(menu, "HideList", this.IsCustomizationFieldsVisible());
        this.SetMenuItemVisibilityState(menu, "ShowList", !this.IsCustomizationFieldsVisible());
        this.SetMenuItemVisibilityState(menu, "ShowPrefilter", !this.IsPrefilterVisible());
        var isVisible = this.isOLAPSortModeNoneItemsVisible();
        this.SetMenuItemState(menu, "SortAZ", isVisible, true, this.isOLAPSortModeNoneSortAZChecked());
        this.SetMenuItemState(menu, "SortZA", isVisible, true, this.isOLAPSortModeNoneSortZAChecked());
        this.SetMenuItemState(menu, "ClearSort", isVisible, this.isOLAPSortModeNoneClearSortEnabled(), false);

        this.showMenu(e, menu);
    },
    showFieldListMenu: function(e) {
        var menu = ASPx.GetControlCollection().Get(this.name + "_FM");
        this.showMenu(e, menu);
    },
    SetMenuItemVisibilityState: function(menu, name, isVisible) {
        menuItem = menu.GetItemByName(name);
        if(menuItem != null)
            menuItem.SetVisible(isVisible);
    },
    SetMenuItemState: function(menu, name, isVisible, isEnabled, isChecked) {
        menuItem = menu.GetItemByName(name);
        if(menuItem != null) {
            menuItem.SetVisible(isVisible);
            menuItem.SetEnabled(isEnabled);
            menuItem.SetChecked(isChecked);
        }
    },
    isOLAPSortModeNoneItemsVisible: function() {
        return (this.headerMenuElementID.indexOf("osmn") > 0);
    },
    isOLAPSortModeNoneSortAZChecked: function() {
        return (this.headerMenuElementID.indexOf("osmnSAZ") > 0);
    },
    isOLAPSortModeNoneSortZAChecked: function() {
        return (this.headerMenuElementID.indexOf("osmnSZA") > 0);
    },
    isOLAPSortModeNoneClearSortEnabled: function() {
        return (((this.headerMenuElementID.indexOf("osmnSAZ") > 0) || (this.headerMenuElementID.indexOf("osmnSZA") > 0)));
    },

    onHeaderMenuClick: function(itemName) {
        switch(itemName) {
            case "Refresh": this.PerformCallbackInternal(this.GetMainTable(), "RELOAD"); break;
            case "Hide":
                if(this.headerMenuElementID)
                    this.PerformCallbackInternal(this.GetMainTable(), "H|" + this.headerMenuElementID);
                break;
            case "HideList": this.HideCustomizationFields(); break;
            case "ShowList": this.ShowCustomizationFields(true); break;
            case "ShowPrefilter": this.ShowPrefilter(); break;
            case "SortAZ":
                if(this.isOLAPSortModeNoneSortAZChecked()) break;
                this.PerformCallbackInternal(this.GetMainTable(), "SAZ|" + this.headerMenuElementID);
                this.ResetFilterCache(this.filterFieldIndex);
                break;
            case "SortZA":
                if(this.isOLAPSortModeNoneSortZAChecked()) break;
                this.PerformCallbackInternal(this.GetMainTable(), "SZA|" + this.headerMenuElementID);
                this.ResetFilterCache(this.filterFieldIndex);
                break;
            case "ClearSort":
                this.PerformCallbackInternal(this.GetMainTable(), "CS|" + this.headerMenuElementID);
                this.ResetFilterCache(this.filterFieldIndex);
                break;
            default:
                if(ASPx.IsExists(this.RaisePopupMenuItemClick))
                    this.RaisePopupMenuItemClick("HeaderMenu", itemName, this.headerMenuFieldID, -1);
                break;
        }
    },
    showFieldValueMenu: function(e, state, iscollapsed, fieldID, itemVisibleIndex, canShowSortBySummary, area, sortedFields, dataIndex, itemIndex) {
        this.fieldMenuCellState = state;
        this.fieldMenuFieldID = fieldID;
        this.fieldMenuFieldValueIndex = itemVisibleIndex;
        this.fieldMenuFieldItemIndex = itemIndex;
        this.fieldMenuDataIndex = dataIndex;
        this.fieldMenuArea = area;
        var menu = ASPx.GetControlCollection().Get(this.name + "_FVM");
        if(menu == null) return;
        this.FilterFieldValueMenuItems(menu, state, iscollapsed, canShowSortBySummary, area, sortedFields, dataIndex);
        if(this.MenuHasVisibleItems(menu))
            this.showMenu(e, menu);
    },
    MenuHasVisibleItems: function(menu) {
        var itemsCount = menu.GetItemCount();
        for(var i = 0; i < itemsCount; i++) {
            var menuItem = menu.GetItem(i);
            if(menuItem.GetVisible())
                return true;
        }
        return false;
    },
    FilterFieldValueMenuItems: function(menu, state, iscollapsed, canShowSortBySummary, area, sortedFields, dataIndex) {
        this.SetMenuItemVisibilityState(menu, "Expand", iscollapsed && state != "");
        this.SetMenuItemVisibilityState(menu, "Collapse", !iscollapsed && state != "");
        this.SetMenuItemVisibilityState(menu, "ExpandAll", state != "");
        this.SetMenuItemVisibilityState(menu, "CollapseAll", state != "");

        var itemsCount = menu.GetItemCount(),
			showRemoveAll = false;
        for(var i = 0; i < itemsCount; i++) {
            var menuItem = menu.GetItem(i);
            if(menuItem.name.indexOf("SortBy_") == 0) {
                var isRemoveAll = menuItem.name.indexOf("RemoveAll") >= 0;
                var visible = canShowSortBySummary && menuItem.name.indexOf(area) > 0;
                if(isRemoveAll) {
                    visible = visible && showRemoveAll;
                } else {
                    if(dataIndex >= 0)
                        visible = visible && menuItem.name.indexOf("_" + dataIndex) == menuItem.name.lastIndexOf("_");
                }
                menuItem.SetVisible(visible);
                if(visible && !isRemoveAll) {
                    var isChecked = this.GetSortByMenuItemCheckedState(menuItem, sortedFields);
                    showRemoveAll |= isChecked;
                    menuItem.SetChecked(isChecked);
                }
            }
        }
    },
    GetSortByMenuItemCheckedState: function(menuItem, sortedFields) {
        if(menuItem.name.indexOf("RemoveAll") >= 0) return false;
        for(var i = 0; i < sortedFields.length; i++) {
            if(menuItem.name.indexOf(sortedFields[i]) >= 0)
                return true;
        }
        return false;
    },
    showMenu: function(e, menu) {
        menu.ShowInternal(e);
        ASPx.Evt.PreventEventAndBubble(e);
    },
    onFieldValueMenuClick: function(itemName) {
        switch(itemName) {
            case "Expand":
            case "Collapse": this.PerformCallbackInternal(this.GetMainTable(), this.fieldMenuCellState); break;
            case "ExpandAll": this.PerformCallbackInternal(this.GetMainTable(), this.fieldMenuCellState + "|EA"); break;
            case "CollapseAll": this.PerformCallbackInternal(this.GetMainTable(), this.fieldMenuCellState + "|CA"); break;
            default:
                if(itemName.indexOf("SortBy_") == 0)
                    this.onSortByFieldValueMenuClick(itemName);
                else {
                    if(ASPx.IsExists(this.RaisePopupMenuItemClick))
                        this.RaisePopupMenuItemClick("FieldValueMenu", itemName, this.fieldMenuFieldID, this.fieldMenuFieldValueIndex);
                }
                break;
        }
    },

    onFieldListMenuClick: function(itemName) {
        this.GetCustomizationFieldsWindowContentDiv().className = itemName;
        this.GetChildElement("dxpgCustFields_dxpgFLButton").value = itemName;
        this.UpdateExcelCustForm();
    },
    onSortByFieldValueMenuClick: function(itemName) {
        var argument = this.GetSortByArgument(itemName);
        this.PerformCallbackInternal(this.GetMainTable(), "SS|" + this.fieldMenuFieldID + "|" + this.fieldMenuFieldValueIndex + "|" + argument + "|" + this.fieldMenuFieldItemIndex);
        this.ResetFilterCache(this.filterFieldIndex);
    },
    GetSortByArgument: function(itemName) {
        var sortByStr = "SortBy_";
        var startIndex = itemName.indexOf("_", sortByStr.length + 1) + 1;
        var res = itemName.substr(startIndex).replace("_", "|");
        if(res == "RemoveAll")
            res += "|" + this.fieldMenuDataIndex + "|" + this.fieldMenuArea;
        return res;
    },
    DoPagerClick: function(element, value) {
        this.AssignSlideAnimationDirectionByPagerArgument(value, this.pageIndex);
        this.PerformCallbackInternal(element, "P|" + value);
        this.adjustingManager.ResetScrollPos(false, true);
    },
    CanHandleGesture: function(evt) {
        var source = ASPx.Evt.GetEventSource(evt);
        var el = this.adjustingManager.GetCallbackAnimationElement();
        return source === el || ASPx.GetIsParent(el, source);
    },
    AllowStartGesture: function() {
        return ASPxClientControl.prototype.AllowStartGesture.call(this) && 
            (this.AllowExecutePagerGesture(this.pageIndex, this.pageCount, 1) || this.AllowExecutePagerGesture(this.pageIndex, this.pageCount, -1));
    },
    AllowExecuteGesture: function(value) {
        return this.AllowExecutePagerGesture(this.pageIndex, this.pageCount, value);
    },
    ExecuteGesture: function(value, count) {
        this.ExecutePagerGesture(this.pageIndex, this.pageCount, value, count, function(arg) { this.DoPagerClick(null, arg); }.aspxBind(this));
    },

    ExcelCustFormRerenderList: function(element) {
        if(element == null) return;
        element = ASPx.GetNodesByTagName(element, "tbody")[0];
        var child = element.lastChild;
        element.removeChild(child);
        element.appendChild(child);
    },
    ForceUpdateFieldListLists: function() {
        this.ExcelCustFormRerenderList(this.getFieldListFields());
        this.ExcelCustFormRerenderList(this.getFilterAreaFieldList());
        this.ExcelCustFormRerenderList(this.getColumnAreaFieldList());
        this.ExcelCustFormRerenderList(this.getRowAreaFieldList());
        this.ExcelCustFormRerenderList(this.getDataAreaFieldList());
    },
    InitExcelCustForm: function() {
        var content = this.GetCustomizationFieldsWindowContentDiv();
        if(content == null)
            return;
        var custFields = ASPx.GetNodesByPartialClassName(content, "dxpgCustFields");
        for(var i = 0; i < custFields.length; i++) {
            if(ASPx.ElementContainsCssClass(custFields[i], "dxpgCustFieldsDiv")) continue;
			if(ASPx.ElementContainsCssClass(custFields[i], "FieldsFieldList")) {
				var listDiv = ASPx.GetNodesByPartialClassName(custFields[i], "dxpgFLListDiv")[0];
				var scrolledDiv = this.getFielListArrowsHorElement(listDiv);
				if(scrolledDiv)
				    ASPx.Evt.AttachEventToElement(scrolledDiv, "scroll", this.UpdateFieldListScrollLeft);
			} else {
				var listDiv = ASPx.GetNodesByPartialClassName(custFields[i], "dxpgFLListDiv")[0];
				var scrolledDiv = this.getFielListArrowsHorElement(listDiv);
				ASPx.Evt.AttachEventToElement(scrolledDiv, "scroll", this.UpdateFieldListTargets);
				scrolledDiv.pivotGrid = this;
				this.FixHeaderImageDrag(scrolledDiv);
			}
        }
    },
	UpdateFieldListScrollLeft : function(event) {
		var source = ASPx.Evt.GetEventSource(event);
        if(source == null)
        	return;
        if(source.scrollLeft > 0 && ASPx.GetCurrentStyle(source).overflowX == "hidden")
            source.scrollLeft = 0;
	},
    UpdateFieldListTargets: function(event) {
        var source = ASPx.Evt.GetEventSource(event);
        if(source == null || source.pivotGrid == null)
            return;
        if(source.scrollLeft > 0)
            source.scrollLeft = 0;
        var pivotGrid = source.pivotGrid;
        if(pivotGrid.DragDropManager == null || pivotGrid.DragDropManager.DragTargets == null)
            return;
        pivotGrid.updateListElements(pivotGrid.DragDropManager.DragTargets, ASPx.Evt.GetEvent(event));
    },
    UpdateExcelCustForm: function() {
        var contentDiv = this.GetCustomizationFieldsWindowContentDiv();
        if(contentDiv) {
            pivotGrid_UpdateCustomizationFieldsHeight(contentDiv);
            this.UpdateExcelCustFormHeaders();
            this.UpdateExcelCustomFormFieldsContent();
            // opera scroll-y render bug correction
            if (ASPx.Browser.Opera)
                this.ForceUpdateFieldListLists();
            //ie scrollbar render error
            this.FixIEFieldListScrollbar();
            var menu = ASPx.GetControlCollection().Get(this.name + "_FM");
            if (contentDiv.className != "" && menu) {
                this.SetMenuItemState(menu, "StackedDefault", true, true, false);
                this.SetMenuItemState(menu, "StackedSideBySide", true, true, false);
                this.SetMenuItemState(menu, "TopPanelOnly", true, true, false);
                this.SetMenuItemState(menu, "BottomPanelOnly2by2", true, true, false);
                this.SetMenuItemState(menu, "BottomPanelOnly1by4", true, true, false);
                this.SetMenuItemState(menu, contentDiv.className, true, false, false);
            }
        }
    },
    FixIEFieldListScrollbar: function() {
        var browserPutsScrollBarOnContent = ASPx.Browser.IE && document.documentMode && document.documentMode < 8;
        if(!browserPutsScrollBarOnContent) return;
        this.FixIEFieldListScrollbarCore(this.getFieldListFields());
        this.FixIEFieldListScrollbarCore(this.getFilterAreaFieldList());
        this.FixIEFieldListScrollbarCore(this.getColumnAreaFieldList());
        this.FixIEFieldListScrollbarCore(this.getRowAreaFieldList());
        this.FixIEFieldListScrollbarCore(this.getDataAreaFieldList());
    },
    FixIEFieldListScrollbarCore: function(element) {
        if(element == null) return;

        var scrollBarWidth = ASPx.GetVerticalScrollBarWidth();
        var element = ASPx.GetNodesByTagName(element, "table")[0].parentNode;
        if(element.clientHeight < element.scrollHeight) 
            element.style.paddingRight = scrollBarWidth + "px";
        else 
            element.style.paddingRight = "0px";
    },
    UpdateExcelCustFormHeaders: function() {
        var contentDiv = this.GetCustomizationFieldsWindowContentDiv();
        if(contentDiv == null)
            return;
        var listsDiv = ASPx.GetNodesByPartialClassName(contentDiv, "dxpgCustFieldsDiv")[0];
        if(listsDiv == null)
            listsDiv = this.getFieldListFields();
        if(listsDiv == null)
            return;
        var headersList = ASPx.GetNodes(listsDiv, function(e) { return ASPx.ElementContainsCssClass(e, "dxpgHeaderTable"); });
        for(var i = 0; i < headersList.length; i++) {
            var headerTable = headersList[i];
            var sortCells = ASPx.GetNodesByPartialClassName(headerTable, "dxpgHeaderSort");
            var filterCells = ASPx.GetNodesByPartialClassName(headerTable, "dxpgHeaderFilter");
            if(sortCells.length > 0)
                this.UpdateExcelCustFormSortOrFilterCell(sortCells[0], false);
            if(filterCells.length > 0)
                this.UpdateExcelCustFormSortOrFilterCell(filterCells[0], true);
            headerTable.style.borderCollapse = this.isDeferUpdatesChecked() ? "separate" : "collapse";
        }
    },
    UpdateExcelCustomFormFieldsContent: function() {
        var customizationTreeView = this.GetCustomizationTreeView();
        if (customizationTreeView)
            customizationTreeView.AdjustControl();
       },
    UpdateExcelCustFormSortOrFilterCell: function(cell, isfilter) {
		var visible = isfilter || !this.isDeferUpdatesChecked();
        cell.style.display = visible ? "" : "none";
        cell.style.width = "";
        var width = cell.childNodes[0].offsetWidth;
		if(visible && width == 0)
			width = cell.childNodes[0].clientWidth;
        cell.style.width = (width + (ASPx.Browser.WebKitFamily ? ASPx.GetLeftRightBordersAndPaddingsSummaryValue(cell) : 0)) + "px";
    },
    RaiseCustomTargets: function(targets) {
        if(!this.customTargets.IsEmpty()) {
            var args = new ASPxClientPivotCustomTargetsEventArgs(targets);
            this.customTargets.FireEvent(this, args);
        }
    },
    RaiseCustomizationFieldsVisibleChanged: function() {
        if(!this.CustomizationFieldsVisibleChanged.IsEmpty()) {
            var args = new ASPxClientEventArgs();
            this.CustomizationFieldsVisibleChanged.FireEvent(this, args);
        }
    },
    RaiseCellClick: function(htmlEvent, value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex) {
        if(!this.CellClick.IsEmpty()) {
            var args = new ASPxClientClickEventArgs(htmlEvent, value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex);
            this.CellClick.FireEvent(this, args);
        }
    },
    RaiseCellDblClick: function(htmlEvent, value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex) {
        if(!this.CellDblClick.IsEmpty()) {
            var args = new ASPxClientClickEventArgs(htmlEvent, value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex);
            this.CellDblClick.FireEvent(this, args);
        }
    },
    RaisePopupMenuItemClick: function(menuType, menuItemName, fieldID, fieldValueIndex) {
        if(!this.PopupMenuItemClick.IsEmpty()) {
            var args = new ASPxClientPivotMenuItemClickEventArgs(menuType, menuItemName, fieldID, fieldValueIndex);
            this.PopupMenuItemClick.FireEvent(this, args);
        }
    },
    GetCustomizationFieldsVisibility: function() {
        return this.IsCustomizationFieldsVisible();
    },
    SetCustomizationFieldsVisibility: function(value, animate) {
        if(!ASPx.IsExists(animate))
            animate = true;
        this.ChangeCustomizationFieldsVisibilityInternal(value, animate);
    },
    ChangeCustomizationFieldsVisibility: function(animate) {
        if(!ASPx.IsExists(animate))
            animate = true;
        this.ChangeCustomizationFieldsVisibilityInternal(!this.IsCustomizationFieldsVisible(), animate);
    },
    PerformCallback: function(args) {
        if(!ASPx.IsExists(args)) args = "";
        this.ResetFilterCache();
        this.PerformCallbackInternal(this.GetMainTable(), "C|" + args, "CUSTOMCALLBACK");
    }
});
ASPxClientPivotGrid.Cast = ASPxClientControl.Cast;
var ASPxClientClickEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
	constructor: function(htmlEvent, value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex){
        this.constructor.prototype.constructor.call(this);
        this.HtmlEvent = htmlEvent;
        this.Value = value;
        this.ColumnIndex = columnIndex;
        this.RowIndex = rowIndex;
        this.ColumnValue = columnValue;
        this.RowValue = rowValue;
        this.ColumnFieldName = columnFieldName;
        this.RowFieldName = rowFieldName;
        this.ColumnValueType = columnValueType;
        this.RowValueType = rowValueType;
        this.DataIndex = dataIndex;
    }
});
var ASPxClientPivotMenuItemClickEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(menuType, menuItemName, fieldID, fieldValueIndex) {
        this.constructor.prototype.constructor.call(this);
        this.MenuType = menuType;
        this.MenuItemName = menuItemName;
        this.FieldID = fieldID;
        this.FieldValueIndex = fieldValueIndex;
    }
});

var ASPxClientPivotCustomTargetsEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(targets) {
        this.constructor.prototype.constructor.call(this);
        this.Targets = targets;
    }
});
var ASPxClientPivotCustomization = ASPx.CreateClass(ASPxClientControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
    },
    GetMainContainer: function() {
        if(this.ContainerID == null)
            return null;
        return document.getElementById(this.ContainerID);
    },
    GetPivotGrid: function() {
        if(this.PivotGridID == null)
            return null;
        return pivotGrid_GetGrid(this.PivotGridID);
    },
    SetHeight: function(value) {
        var mainContainer = this.GetMainContainer();
        var borderTop = ASPx.PxToInt(ASPx.GetCurrentStyle(mainContainer).borderTopWidth);
        if(!isNaN(borderTop))
            value -= borderTop;
        var borderBottom = ASPx.PxToInt(ASPx.GetCurrentStyle(mainContainer).borderBottomWidth);
        if(!isNaN(borderBottom))
            value -= borderBottom;
        this.GetMainContainer().style.height = value + "px";
        this.UpdateHeight();
    },
    SetWidth: function(value) {
        var mainContainer = this.GetMainContainer();
        var borderLeft = ASPx.PxToInt(ASPx.GetCurrentStyle(mainContainer).borderLeftWidth);
        if(!isNaN(borderLeft))
            value -= borderLeft;
        var borderRight = ASPx.PxToInt(ASPx.GetCurrentStyle(mainContainer).borderRightWidth);
        if(!isNaN(borderRight))
            value -= borderRight;
        this.GetMainContainer().style.width = value + "px";
    },
    UpdateHeight: function() {
        var PivotGrid = this.GetPivotGrid();
        if(PivotGrid != null)
            this.GetPivotGrid().UpdateExcelCustForm();
    },
    InitForm: function () {
        var controlName = this.GetPivotGrid() != null ? this.GetPivotGrid().name : this.name;
        var custFieldsContent = ASPx.GetElementById(controlName + "_dxpgCustFields");
        pivotGrid_UpdateCustomizationFieldsHeight(custFieldsContent);
    },
    SetLayout: function(layout) {
        var PivotGrid = this.GetPivotGrid();
        if(PivotGrid == null)
            return;
        PivotGrid.GetChildElement("dxpgCustFields_dxpgFLButton").value = layout;
        PivotGrid.GetCustomizationFieldsWindowContentDiv().className = layout;
        PivotGrid.UpdateExcelCustForm();
    },
    AdjustControlCore: function() {
        ASPxClientControl.prototype.AdjustControlCore.call(this);
        this.InitForm();
    }
});


ASPx.pivotGrid_GetGrid = function (id) {
    return pivotGrid_GetGrid(id);
}
ASPx.pivotGrid_IsBusy = function (id) {
    return pivotGrid_IsBusy(id);
}

function pivotGrid_GetGrid(id) {
    return ASPx.GetControlCollection().Get(id);
}
function pivotGrid_IsBusy(id) {
    var grid = pivotGrid_GetGrid(id);
    if(grid.isCallbackInProcess)
        return true;
    var filterControl = grid.GetFilterControl();
    if(filterControl != null && filterControl.isCallbackInProcess)
        return true;
    return false;
}
function pivotGrid_GetGridFromMenu(menu) {
    if(menu == null) return null;
    var pos = menu.name.lastIndexOf("_");
    if(pos > -1) {
        return pivotGrid_GetGrid(menu.name.substring(0, pos));
    }
    return null;
}
ASPx.pivotGrid_OnHeaderMenuClick = function(s, args) {
    var grid = pivotGrid_GetGridFromMenu(s);
    if(grid != null) {
        grid.onHeaderMenuClick(args.item.name);
    }
}
function pivotGrid_ShowHeaderMenu(id, e, fieldID) {
    var grid = pivotGrid_GetGrid(id);
    if(grid != null) {
        grid.showHeaderMenu(e, fieldID);
    }
}
ASPx.pivotGrid_OnFieldValueMenuClick = function(s, args) {
    var grid = pivotGrid_GetGridFromMenu(s);
    if(grid != null) {
        grid.onFieldValueMenuClick(args.item.name);
    }
}
ASPx.pivotGrid_OnFieldListMenuClick = function(s, args) {
    var grid = pivotGrid_GetGridFromMenu(s);
    if(grid != null) {
        grid.onFieldListMenuClick(args.item.name);
    }
}
ASPx.pivotGrid_OnFieldListLayoutButtonClick = function(id, evt) {
    var evt = ASPx.Evt.GetEvent(evt);
    var pivotGrid = pivotGrid_GetGrid(id);
    if(pivotGrid == null) return;
    pivotGrid.showFieldListMenu(evt);
}
function pivotGrid_ShowFieldValueMenu(id, e, state, iscollapsed, fieldID, itemVisibleIndex, canShowSortBySummary, area, sortedFields, dataIndex, itemIndex) {
    var grid = pivotGrid_GetGrid(id);
    if(grid != null) {
        grid.showFieldValueMenu(e, state, iscollapsed, fieldID, itemVisibleIndex, canShowSortBySummary, area, sortedFields, dataIndex, itemIndex);
    }
}
ASPx.pivotGrid_HeaderMouseDown = function(id, element, e) {
    var grid = pivotGrid_GetGrid(id);
    if(grid != null) {
        if(!element.id || !(element.id.indexOf("pgHeader") != -1 || element.id.indexOf("pgGroupHeader") != -1))
            return;
        grid.headerMouseDown(element, e);
    }
}
ASPx.pivotGrid_HeaderClick = function(id, element, e) {
    var grid = pivotGrid_GetGrid(id);
    if(grid != null) {
        grid.headerClick(element);
    }
}

ASPx.pivotGrid_CustFormHeaderClick = function(id, element, e) {
    var grid = pivotGrid_GetGrid(id);
    if(grid != null && !pivotGrid_IsBusy(id))
        grid.headerClick(element);
}

ASPx.pivotGrid_ShowFilterPopup = function(name, headerId, isFieldList) {
    var pg = pivotGrid_GetGrid(name);
    if(pg != null)
        pg.ShowFilterPopup(headerId);
}

ASPx.pivotGrid_PerformCallback = function(name, el, value) {
    var pg = pivotGrid_GetGrid(name);
    if(pg != null)
        pg.PerformCallbackInternal(el, value);
}

ASPx.pivotGrid_FieldFilterValueChanged = function(name, index) {
    var pg = pivotGrid_GetGrid(name);
    if(pg != null)
        pg.fieldFilterValueChanged(index);
    else
        pivotGrid_WasNotFound();
}

ASPx.pivotGrid_ApplyFilter = function(name) {
    var pg = pivotGrid_GetGrid(name);
    if(pg != null) {
        if(!pg.GetHeaderFilterPopup().IsVisible()) return;
        pg.applyFilter();
    }
    else
        pivotGrid_WasNotFound();
}

function pivotGrid_WasNotFound() {
    alert("PivotGrid was not found");
}

ASPx.pivotGrid_HideFilter = function(name) {
    var pg = pivotGrid_GetGrid(name);
    if(pg != null)
        pg.GetHeaderFilterPopup().Hide();
}

ASPx.pivotGrid_ClearSelection = function() {
    if(!ASPx.Browser.Opera)
        return;
    if(ASPx.IsExists(window.getSelection)) {
        if(ASPx.Browser.WebKitFamily)
            window.getSelection().collapse();
        else
            window.getSelection().removeAllRanges();
    }
    else if(ASPx.IsExists(document.selection)) {
        if(ASPx.IsExists(document.selection.empty))
            document.selection.empty();
        else if(ASPx.IsExists(document.selection.clear))
            document.selection.clear();
    }
}

ASPx.pivotGrid_PagerClick = function(name, element, id) {
    var pg = pivotGrid_GetGrid(name);
    if(pg != null) pg.DoPagerClick(element, id);
}

function pivotGrid_GetGridByCustomizationFields(custFields) {
    if(!ASPx.IsExists(custFields.pivotGrid) || custFields.pivotGrid == null) {
        var name = custFields.name.substr(0, custFields.name.length - ("_DXCustFields").length);
        custFields.pivotGrid = ASPx.GetControlCollection().Get(name);
    }
    return custFields.pivotGrid;
}

ASPx.pivotGrid_CustomizationFormDeferUpdates = function(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg == null) return;
    pg.UpdateExcelCustFormHeaders();
}

ASPx.pivotGrid_CustomizationFormResumeUpdates = function(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg == null) return;
    if (pg.forcePerformDeferUpdatesCallback) {
    	pg.PerformDeferUpdatesCallback();
        pg.forcePerformDeferUpdatesCallback = false;
    }
	else
		pg.UpdateExcelCustFormHeaders();
}

ASPx.pivotGrid_CustomizationFormUpdate = function(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg == null) return;
    pg.PerformDeferUpdatesCallback();
}

function pivotGrid_UpdateCustomizationFieldsHeight(content) {
    if(content == null)
        return;
    var buttonDiv = ASPx.GetNodesByPartialClassName(content, "dxpgFLButtonDiv")[0];
    var deferDiv = ASPx.GetNodesByPartialClassName(content, "dxpgFLDefereDiv")[0];
    var listsDiv = ASPx.GetNodesByPartialClassName(content, "dxpgCustFieldsDiv")[0];
    if(buttonDiv == null || deferDiv == null || listsDiv == null)
        return;
    var isIE10 = ASPx.Browser.IE && ASPx.Browser.Version > 9;
    var isIE11 = ASPx.Browser.IE && ASPx.Browser.Version > 10;
    var isNotTopPanelOnly = content.className != "TopPanelOnly";
    if(ASPx.Browser.IE && isNotTopPanelOnly) {
        deferDiv.style.display = "none";
        deferDiv.style.display = "block";
    }
    var custFields = ASPx.GetNodesByPartialClassName(content, "dxpgCustFields");
    for(var i = 0; i < custFields.length; i++) {
        if(!ASPx.ElementContainsCssClass(custFields[i], "dxpgCustFieldsDiv")) {
            var list = ASPx.GetNodesByPartialClassName(custFields[i], "dxpgFLListDiv")[0];
            if (isIE11) {
                list.style.maxHeight = "1px";
            } else {
                list.style.display = "none";
                if (isIE10) {
                    list.style.display = "block";
                    list.style.height = "1px";
                    list.style.overflow = "hidden";
                }
            }
        }
    }
    var fullHeight = parseInt(content.clientHeight);
    if(fullHeight == 0)
        fullHeight = parseInt(content.offsetHeight);
    var height = fullHeight - parseInt(deferDiv.clientHeight) - parseInt(buttonDiv.clientHeight);
    if(height < 0) return;
    listsDiv.style.height = height + "px";
    for(var i = 0; i < custFields.length; i++) {
        if(!ASPx.ElementContainsCssClass(custFields[i], "dxpgCustFieldsDiv")) {
            var text = ASPx.GetNodesByPartialClassName(custFields[i], "dxpgFLTextDiv")[0];
            var list = ASPx.GetNodesByPartialClassName(custFields[i], "dxpgFLListDiv")[0];
            list.style.display = "block";
            list.style.height = "auto";
            list.style.overflow = "visible";
            list.style.maxHeight = "";
            var height2 = parseInt(custFields[i].clientHeight) - parseInt(text.clientHeight);
            if (content.className == "StackedSideBySide" && ASPx.ElementContainsCssClass(custFields[i], "DataArea")) {
                height2 += pivotGrid_GetFieldList1By425percRound(height);
            }
            if (height2 >= 0) {
                list.style.height = height2 + "px";
            }
        }
    }
}
function pivotGrid_GetFieldList1By425percRound(height) {
    if(ASPx.Browser.IE && ASPx.Browser.Version < 8) {
        if(height % 4 == 2 || height % 4 == 3)
            return -1;
        if(height % 4 == 1)
            return -2;
    }
    if(ASPx.Browser.WebKitFamily || ASPx.Browser.Opera) {
        if(height % 4 == 1 || height % 4 == 2)
            return 1;
        if(height % 4 == 3)
            return 2;
    }
    if(ASPx.Browser.Firefox && height % 4 == 2)
        return -1;
    return 0;
}
ASPx.pivotGrid_DoCustomizationFieldsVisibleChanged = function(sender) {
    var control = pivotGrid_GetGridByCustomizationFields(sender);
    if(control == null) return;
    if(ASPx.IsExists(control.RaiseCustomizationFieldsVisibleChanged)) {
        control.RaiseCustomizationFieldsVisibleChanged();
    }
}
ASPx.pivotGrid_DoUpdateContentSize = function(sender) {
    var control = pivotGrid_GetGridByCustomizationFields(sender);
    var content = control.GetCustomizationFieldsWindowContentDiv();
    var popup = control.GetCustomizationFieldsWindow();
    var contentCell = popup.GetWindowContentWrapperElement(-1);

    content.style.width = ASPx.GetClearClientWidth(contentCell) + "px";
    content.style.height = ASPx.GetClearClientHeight(contentCell) + "px";
    content.style.display = "block";

    control.RestoreContentDivScrollTop();
    control.UpdateExcelCustForm();
}

ASPx.pivotGrid_DoResetContentSize = function(sender) {
    var content = pivotGrid_GetGridByCustomizationFields(sender).GetCustomizationFieldsWindowContentDiv();
    content.style.width = "0px";
    content.style.height = "0px"
}

function pivotGrid_FieldValueContextMenuHandler(e) {
    var source = ASPx.Evt.GetEventSource(e);
    while(source != null && !ASPx.IsExists(source.contextMenuParams))
        source = source.parentNode;
    if(source == null)
        return;
    pivotGrid_ShowFieldValueMenu(source.pivotClientID, e, source.contextMenuParams[1], source.contextMenuParams[2],
		source.contextMenuParams[3], source.contextMenuParams[4], source.contextMenuParams[5], source.contextMenuParams[6],
		source.contextMenuParams[7], source.contextMenuParams[8], source.contextMenuParams[9]);
}

function pivotGrid_HeaderContextMenuHandler(e) {
    var source = ASPx.Evt.GetEventSource(e);
    while(source != null && !ASPx.IsExists(source.contextMenuParams))
        source = source.parentNode;
    if(source == null)
        return;
    pivotGrid_ShowHeaderMenu(source.pivotClientID, e, source.contextMenuParams[1]);
}

ASPx.pivotGrid_AfterCallBackInitialize = function(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null)
        pg.AfterCallBackInitialize();
}

ASPx.pivotGrid_SetRenderOptions = function(id, options) {
    var pg = pivotGrid_GetGrid(id);
    if (pg != null)
        pg.SetRenderOptions(options);
}

ASPx.pivotGrid_CellClick = function(id, htmlEvent, value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null && ASPx.IsExists(pg.RaiseCellClick))
		pg.RaiseCellClick(htmlEvent, value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex);
}

ASPx.pivotGrid_CellDoubleClick = function(id, htmlEvent, value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null && ASPx.IsExists(pg.RaiseCellDblClick))
		pg.RaiseCellDblClick(htmlEvent, value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex);
}

ASPx.pivotGrid_Sort508 = function(id, headerSuffix) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null) {
        pg.ResetFilterCache(pg.filterFieldIndex);
        pg.PerformCallbackInternal(null, 'S|' + id + "_" + headerSuffix);
    }
}

ASPx.pivotGrid_ApplyPrefilter = function(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null)
        pg.ApplyPrefilter();
}

ASPx.pivotGrid_HidePrefilter = function(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null)
        pg.HidePrefilter();
}

ASPx.pivotGrid_ShowPrefilter = function(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null)
        pg.ShowPrefilter();
}

ASPx.pivotGrid_ClearPrefilter = function(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null)
        pg.ClearPrefilter();
}

ASPx.pivotGrid_ChangePrefilterEnabled = function(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null)
        pg.ChangePrefilterEnabled();
}


ASPx.pivotGrid_FixIE8RowTreeLayout = function(maxCellID, pivotGrid) {
    var cell = ASPx.GetElementById(maxCellID);
    cell.style.width = cell.offsetWidth + 'px';
    var pivot = pivotGrid_GetGrid(pivotGrid);
    if (pivot == null) return;
    pivot.AdjustControl();
}

ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseMoveEventName, function(evt) {
    var collection = ASPxClientPivotGridControlCollection.GetPivotGridControlCollection();
    collection.OnMouseMove(evt);
});

window.ASPxClientPivotGrid = ASPxClientPivotGrid;
window.ASPxClientPivotGridGroup = ASPxClientPivotGridGroup;
window.ASPxClientPivotGridControlCollection = ASPxClientPivotGridControlCollection;

window.ASPxClientClickEventArgs = ASPxClientClickEventArgs;
window.ASPxClientPivotMenuItemClickEventArgs = ASPxClientPivotMenuItemClickEventArgs;
window.ASPxClientPivotCustomTargetsEventArgs = ASPxClientPivotCustomTargetsEventArgs;
window.ASPxClientPivotCustomization = ASPxClientPivotCustomization;
})();