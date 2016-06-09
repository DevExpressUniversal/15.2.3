/// <reference path="..\_references.js"/>

(function() {
var loadFilteredItemsCallbackPrefix = "CBLF";
var correctFilterCallbackPrefix = "CBCF";
var currentSelectedItemCallbackPrefix = "CBSI";
var loadDropDownOnDemandCallbackPrefix = "CBLD";
var listBoxNameSuffix = "_L";
var comboBoxValueInputSuffix = "VI";
var ASPxClientComboBoxBase = ASPx.CreateClass(ASPxClientDropDownEditBase, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        
        this.lbEventLockCount = 0;
        this.receiveGlobalMouseWheel = false;
        
        // Cache
        this.listBox = null;
        this.lastSuccessValue = "";
        this.islastSuccessValueInit = false;        
        this.SelectedIndexChanged = new ASPxClientEvent();
    },
    Initialize: function(){
        this.InitializeListBoxOwnerName();
        ASPxClientDropDownEditBase.prototype.Initialize.call(this);
        this.InitLastSuccessValue();
    },
    InitializeListBoxOwnerName: function(){
        var lb = this.GetListBoxControl();
        if(lb)
            lb.ownerName = this.name;
    },
    InitLastSuccessValue: function(){
        this.SetLastSuccessValue(this.GetValue());
    },
    SetLastSuccessValue: function (value) {
        if(this.convertEmptyStringToNull && value === "")
            value = null;
        this.lastSuccessValue = value;
        this.islastSuccessValueInit = true;
    },
    GetDropDownInnerControlName: function(suffix){
        return "";
    },
    GetListBoxControl: function(){
        if(!ASPx.IsExists(this.listBox)){
            var name = this.GetDropDownInnerControlName(listBoxNameSuffix);
            this.listBox = ASPx.GetControlCollection().Get(name);
        }
        if(this.isNative || (this.listBox && !!this.listBox.GetMainElement()))
            return this.listBox;
        return null;
    },
    GetCallbackArguments: function(){
        return this.GetListBoxCallbackArguments();
    },
    GetListBoxCallbackArguments: function(){
        var lb = this.GetListBoxControl();
        return lb.GetCallbackArguments();
    },
    SendCallback: function() {
        this.CreateCallback(this.GetCallbackArguments());
    },
    SendSpecialCallback: function(args) {
        this.CreateCallback(args);
    },
    SetText: function (text){
        var lb = this.GetListBoxControl();
        var index = this.GetAdjustedSelectedIndexByText(lb, text);
        this.SelectIndex(index, false);
        this.SetTextBase(text);
        this.SetLastSuccessText(text);
        this.SetLastSuccessValue(index >= 0 ? lb.GetValue() : text);
    },
    GetValue: function(){
        var value = this.islastSuccessValueInit ? this.lastSuccessValue : this.GetValueInternal();
        if(this.convertEmptyStringToNull && value === "")
            value = null;
        return value;
    },
    GetValueInternal: function(){
        var text = this.GetTextInternal();
        var textChanges = text != this.lastSuccessText;
        if(textChanges){
            var lb = this.GetListBoxControl();
            if(lb){
                var index = this.GetAdjustedSelectedIndexByText(lb, text);
                this.SelectIndexSilent(lb, index); // Prevent to direct input value changing
                if(index != -1)
                    return lb.GetValue();
            }
        }
        return ASPxClientDropDownEditBase.prototype.GetValue.call(this);
    },
    GetLastSuccesfullValue: function() {
        return this.GetValue();
    },
    GetSetValueOptimizeHelper: function () {
        this.setValueOptimizeHelper = this.setValueOptimizeHelper || new setValueOptimizeHelper();
        return this.setValueOptimizeHelper;
    },
    SetValue: function (value) {
        this.GetSetValueOptimizeHelper().setValue(this, value);
    },
    SetValueInternal: function(value) {
        var lb = this.GetListBoxControl();
        if (lb) {
            lb.SetValue(value);
            // TODO Extract method with OnSelectChanged (params: value)
            var item = lb.GetSelectedItem();
            var text = item ? item.text : value;
            this.OnSelectionChangedCore(text, item, false);
            this.UpdateValueInput();
            //
        }
    },
    GetFormattedText: function() {
        return this.GetText();
    },
    
    GetAdjustedSelectedIndexByText: function(lb, text){
        var lbSelectedItem = lb.GetSelectedItem();
        if(lbSelectedItem != null && lbSelectedItem.text == text)
            return lbSelectedItem.index;
        return this.FindItemIndexByText(lb, text);
    },
    FindItemIndexByText: function(lb, text){
        if(lb)
            return lb.FindItemIndexByText(text);
    },
    CollectionChanged: function(){
    },
    HasChanges: function(){
        return false;
    },
    SelectIndex: function(index, initialize){
        var lb = this.GetListBoxControl();
        var isSelectionChanged = lb.SelectIndexSilentAndMakeVisible(index, initialize);
        var item = lb.GetSelectedItem();
        var text = item != null ? item.text : "";

        if(isSelectionChanged || this.HasChanges())
            this.OnSelectionChangedCore(text, item, false);
            
        this.UpdateValueInput();
        
        return isSelectionChanged;
    },
    OnSelectChanged: function(){
        if(this.lbEventLockCount > 0) return;

        var lb = this.GetListBoxControl();
        // TODO Extract method with SetValue (params: value = "")
        var item = lb.GetSelectedItem();
        var text = item != null ? item.text : "";
        this.OnSelectionChangedCore(text, item, false);
        //
        this.OnChange();
    },
    OnSelectionChangedCore: function(text, item, canBeRolledBack){
        this.SetTextBase(text);
        this.ShowItemImage(item);
        
        if(!canBeRolledBack){
            this.SetLastSuccessText(text);
            this.SetLastSuccessValue(item != null ? item.value : text);
        }
        if(this.filterStrategy) {
            if(!canBeRolledBack)
                this.filterStrategy.OnSelectionChanged();
            if(ASPx.Browser.IE) { // B186238
                var inputElement = this.GetInputElement();
                if(ASPx.GetActiveElement() == inputElement)
                    ASPx.Selection.Set(inputElement, inputElement.value.length, inputElement.value.length);
            }
        }
    },
    ShowItemImageByIndex: function(index){
        var item = this.GetItem(index);
        this.ShowItemImage(item);
    },
    ShowItemImage: function(item){
        var imageUrl = item != null ? item.imageUrl : "";
        this.SetSelectedImage(imageUrl);
    },
    
    GetDropDownImageElement: function(){
        var itemImageCell = this.GetDropDownItemImageCell();
        if(itemImageCell != null)
            return ASPx.GetNodeByTagName(itemImageCell, "IMG", 0);
        return null;
    },
    SetSelectedImage: function(imageUrl) {
        var imgElement = this.GetDropDownImageElement();
        if(imgElement != null) {
            var imageExists = imageUrl != "";
            imageUrl = imageExists ? imageUrl : ASPx.EmptyImageUrl;
            imgElement.src = imageUrl;
            var itemImageCell = this.GetDropDownItemImageCell();
            if(ASPx.GetElementDisplay(itemImageCell) != imageExists)
                ASPx.SetElementDisplay(itemImageCell, imageExists);
            if(ASPx.Browser.IE) {
                this.AdjustControl();
            }
        }
    },
    OnCallback: function(result) {
    },
    OnChange: function(){
        this.UpdateValueInput();
        this.RaisePersonalStandardValidation();
        this.OnValueChanged();
    },
    UpdateValueInput: function() {
    },
    
    RaiseValueChangedEvent: function() {
        if(!this.isInitialized) return;
        var processOnServer = ASPxClientTextEdit.prototype.RaiseValueChangedEvent.call(this);
        processOnServer = this.RaiseSelectedIndexChanged(processOnServer);
        return processOnServer;
    },
    RaiseSelectedIndexChanged: function(processOnServer) {
        if(!this.SelectedIndexChanged.IsEmpty()){
            var args = new ASPxClientProcessingModeEventArgs(processOnServer);
            this.SelectedIndexChanged.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    AddItem: function(text, value, imageUrl){
        var index = this.GetListBoxControl().AddItem(text, value, imageUrl);
        this.CollectionChanged();
        return index;
    },
    InsertItem: function(index, text, value, imageUrl){
        this.GetListBoxControl().InsertItem(index, text, value, imageUrl);
        this.CollectionChanged();
    },
    RemoveItem: function(index){
        this.GetListBoxControl().RemoveItem(index);
        this.CollectionChanged();
    },
    ClearItems: function(){
        this.GetListBoxControl().ClearItems();
        this.ClearItemsInternal();
    },
    BeginUpdate: function(){
         this.GetListBoxControl().BeginUpdate();
    },
    EndUpdate: function(){
        this.GetListBoxControl().EndUpdate();
        this.CollectionChanged();
    },
    MakeItemVisible: function(index){
    },
    GetItem: function(index){
        var lb = this.GetListBoxControl();
        if(lb)
            return this.GetListBoxControl().GetItem(index);
        else
            return null;
    },
    FindItemByText: function(text) {
        var lb = this.GetListBoxControl();
        if(lb)
            return lb.FindItemByText(text);
        return null;
    },
    FindItemByValue: function(value){
        return this.GetListBoxControl().FindItemByValue(value);
    },
    GetItemCount: function(){
        return this.GetListBoxControl().GetItemCount(); 
    },
    GetSelectedIndex: function(){
        var lb = this.GetListBoxControl();
        if(lb)
            return lb.GetSelectedIndex();
        else
            return -1;
    },
    SetSelectedIndex: function(index){
        this.SelectIndex(index, false);
    },
    GetSelectedItem: function(){
        var lb = this.GetListBoxControl();
        var index = lb.GetSelectedIndex();
        return lb.GetItem(index);
    },
    SetSelectedItem: function(item){
        var index = (item != null) ? item.index : -1;
        this.SelectIndex(index, false);
    },
    GetText: function(){
        return this.lastSuccessText;
    },
    PerformCallback: function(arg) {
    },
    ClearItemsInternal: function(){
        this.SetValue(null);
        this.CollectionChanged();
    }
});
var ASPxClientComboBox = ASPx.CreateClass(ASPxClientComboBoxBase, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        // Callback
        this.allowMultipleCallbacks = false;
        this.allowNull = false;
        this.isCallbackMode = false;
        this.loadDropDownOnDemand = false;
        this.needToLoadDropDown = false;
        this.isPerformCallback = false;
        this.changeSelectAfterCallback = 0;
        // Filtering
        this.incrementalFilteringMode = "Contains";
        this.filterStrategy = null;
        this.filterTimer = ASPx.Browser.WebKitTouchUI ? 300 : 100; // B211635
        this.filterMinLength = 0;
        
        this.initTextCorrectionRequired = false;
        this.isDropDownListStyle = true;
        this.defaultDropDownHeight = "";
        this.dropDownHeight = "";
        this.dropDownWidth = "";
        this.dropDownRows = 7;
        this.enterKeyPressed = false;
        this.onLoadDropDownOnDemandCallbackFinalizedEventHandler = null;
        this.callBackCoreComplete = false; //B235715
        this.adroidSamsungBugTimeout = 0; //T170541

        this.isNeedToForceFirstShowLoadingPanel = true; //T126461
    },
    Initialize: function(){
		this.needToLoadDropDown = this.loadDropDownOnDemand;
        var lb = this.GetListBoxControl();
        this.InitializeListBoxOwnerName();
        this.InitScrollSpacerVisibility();
        this.FilterStrategyInitialize();
        var mainElement = this.GetMainElement();
        var input = this.GetInputElement();       
        var ddbutton = this.GetDropDownButton();
        if(this.isDropDownListStyle && (ASPx.Browser.IE || (ASPx.Browser.WindowsPlatform && ASPx.Browser.Safari))) {
            ASPx.Evt.AttachEventToElement(this.GetInputElement(), "blur", function() {
                var inputElementValue = this.GetInputElement().value;
                if(!this.focusEventsLocked && !inputElementValue && !!this.GetText())
                    this.OnTextChanged();
            }.aspxBind(this)); //T243995

            if(ASPx.Browser.IE) {
                ASPx.Evt.PreventElementDragAndSelect(mainElement, true, true);
                ASPx.Evt.PreventElementDragAndSelect(input, true, true);
                if(ddbutton)
                    ASPx.Evt.PreventElementDragAndSelect(ddbutton, true);
            }
        }
        if(this.isToolbarItem){
            if(ASPx.Browser.IE && ASPx.Browser.Version == 9)
                input.onmousedown = function(evt) { ASPx.Evt.PreventEvent(evt); };
            else {
                mainElement.unselectable="on";
                input.unselectable="on";
                if(input.offsetParent)
                    input.offsetParent.unselectable="on";
                if(ddbutton)
                    ddbutton.unselectable="on";
                if(lb){
                    var table = lb.GetListTable();
                    for(var i = 0; i < table.rows.length; i ++){
                        for(var j = 0; j < table.rows[i].cells.length; j ++)
                            ASPx.Selection.SetElementAsUnselectable(table.rows[i].cells[j], true);
                    }
                }
            }
        }
        
        this.RemoveRaisePSValidationFromListBox();
        this.RedirectStandardValidators();

        if(lb && lb.GetItemCount() > 0)
            this.EnsureListBoxSelectionSynchronized(lb);

        ASPxClientComboBoxBase.prototype.Initialize.call(this);
    },
    InitScrollSpacerVisibility: function() {
        var lb = this.GetListBoxControl();
        if(lb) {
            if(lb.GetItemCount() < lb.callbackPageSize) {
                lb.SetScrollSpacerVisibility(true, false);
                lb.SetScrollSpacerVisibility(false, false);
            }
        }
    },
    FilterStrategyInitialize: function() {
        if(this.incrementalFilteringMode == "Contains")
            this.filterStrategy = new ASPxContainsFilteringStrategy(this);
        else if(this.incrementalFilteringMode == "StartsWith")
            this.filterStrategy = new ASPxStartsWithFilteringStrategy(this);
        else if(this.incrementalFilteringMode == "None")
            this.filterStrategy = new ASPxComboBoxDisableFilteringStrategy(this);
        this.filterStrategy.Initialize();
    },
    InlineInitialize: function() {
        this.BeforeInlineInitialize();

        this.InitSpecialKeyboardHandling();
        ASPxClientComboBoxBase.prototype.InlineInitialize.call(this);
    },
    BeforeInlineInitialize: function() {
        this.lastSuccessValue = this.GetDecodeValue(this.lastSuccessValue);
        this.InsureInputValueCorrect();
    },
    InsureInputValueCorrect: function(){ // B139038
        if(this.initTextCorrectionRequired){
            var lb = this.GetListBoxControl();
            if(lb){
                var initSelectedIndex = lb.GetSelectedIndexInternal();
                if(initSelectedIndex >= 0){
                    var initSelectedText = lb.GetItem(initSelectedIndex).text;
                    var input = this.GetInputElement();
                    if(ASPx.IsExists(this.GetRawValue()) && this.GetRawValue() != initSelectedText){
                        this.SetRawValue(initSelectedText);
                        input.value = this.GetDecoratedText(initSelectedText);
                    } 
                    else if(input.value != initSelectedText)
                        input.value = initSelectedText;
                }
            }
        }
    },
    ChangeEnabledAttributes: function(enabled){
        ASPxClientComboBoxBase.prototype.ChangeEnabledAttributes.call(this, enabled);
        var changeEventsMethod = ASPx.Attr.ChangeEventsMethod(enabled);
        var mainElement = this.GetMainElement();
        if(mainElement)
            changeEventsMethod(mainElement, ASPx.Evt.GetMouseWheelEventName(), aspxCBMouseWheel);
        var btnElement = this.GetDropDownButton();
        if(btnElement)
            changeEventsMethod(btnElement, "onmousemove", ASPx.CBDDButtonMMove);
    },

    GetDropDownInnerControlName: function(suffix){
        return ASPxClientDropDownEditBase.prototype.GetDropDownInnerControlName.call(this, suffix);
    },
    AdjustControlCore: function() {
        ASPxClientEdit.prototype.AdjustControlCore.call(this);
        this.ResetDropDownSizeCache();
    },
    
    RemoveRaisePSValidationFromListBox: function() {
        var listBox = this.GetListBoxControl();
        if(listBox)
            listBox.RaisePersonalStandardValidation = function() { };
    },
    RedirectStandardValidators: function() {
        var valueInput = this.GetValueInput();
        if(ASPx.IsExistsElement(valueInput) && valueInput.Validators) {
            for(var i = 0; i < valueInput.Validators.length; i++)
                valueInput.Validators[i].controltovalidate = valueInput.id;
        }
    },
    GetValueInputToValidate: function(){
        return this.GetValueInput();
    },
    GetValueInput: function(){
        return document.getElementById(this.name + "_" + comboBoxValueInputSuffix);
    },
    GetListBoxScrollDivElement: function(){
        return this.GetListBoxControl().GetScrollDivElement();
    },
    RollbackValueInputValue: function(){
        var inputElement = this.GetValueInput();
        if(inputElement){
            inputElement.value = this.lastSuccessValue;
        }
    },
    UpdateValueInput: function() {
        var inputElement = this.GetValueInput();
        if(inputElement){
            var value = this.GetValue();
            inputElement.value = value != null ? value : "";
        }
    },
    
    VisibleCollectionChanged: function() {
		this.CollectionChangedCore();
    },
    CollectionChanged: function(){
        this.CollectionChangedCore();
    },
    CollectionChangedCore: function(byTimer){
        this.GetSetValueOptimizeHelper().onItemCollectionChanged();

        if(this.GetListBoxControl().APILockCount == 0){
            this.UpdateDropDownPositionAndSize();
            
            if(ASPx.Browser.IE){ // Prevent wrong selected style render
                var lb = this.GetListBoxControl();
                var selectedIndex = lb.GetSelectedIndex();
                if(selectedIndex > -1){
                    var selectedItemTextCell = lb.GetItemFirstTextCell(selectedIndex);
                    var controller = ASPx.GetStateController();
                    controller.DeselectElementBySrcElement(selectedItemTextCell);
                    controller.SelectElementBySrcElement(selectedItemTextCell);
                }
            }
        }
    },
    UpdateDropDownPositionAndSize: function(){
        this.InitDropDownSize();
        if(this.droppedDown){
            var pc = this.GetPopupControl();
            var element = this.GetMainElement();
            pc.UpdatePositionAtElement(element);
        }
        if(!this.clientVisible)
            this.ResetControlAdjustment();
    },
    GetDropDownHeight: function(){
        if(this.ddHeightCache != ASPx.InvalidDimension)
            return this.ddHeightCache;
        this.EnsureDropDownWidth(); //T190070
        return this.InitListBoxHeight();
    },
    GetDropDownWidth: function(){
        return (this.ddWidthCache != ASPx.InvalidDimension && !this.GetIsControlWidthWasChanged()) ? this.ddWidthCache : this.InitListBoxWidth();
    },
    EnsureDropDownWidth: function() {
        this.GetDropDownWidth();
    },
    InitDropDownSize: function() {
        if(!this.enabled || this.GetItemCount() == 0) return; // B132741
        
        var pc = this.GetPopupControl();
        if(pc && this.IsDisplayed()) {
            var pcwElement = pc.GetWindowElement(-1);
            if(ASPx.IsExistsElement(pcwElement)){
                var isPcwDisplayed = ASPx.GetElementDisplay(pcwElement);
                if(!isPcwDisplayed)
                    pc.SetWindowDisplay(-1, true);
                var listBoxHeight = this.InitListBoxHeight();
                var listBoxWidth = this.InitListBoxWidth();
                if(listBoxHeight != this.ddHeightCache || listBoxWidth != this.ddWidthCache){
                    this.ddHeightCache = listBoxHeight;
                    this.ddWidthCache = listBoxWidth;
                    pc.SetSize(this.ddWidthCache, this.ddHeightCache);
                }
                if(!isPcwDisplayed)
                    pc.SetWindowDisplay(-1, false);
            }
        }
    },
    InitMainElementCache: function(){
        this.mainElementWidthCache = this.GetMainElement().clientWidth;
    },
    GetVisibleItemCount: function(lb){  
        var lbTable = lb.GetListTable();
        var count = this.GetItemCount();
        var visibleItemCount = 0;
        for(var i = 0; i < count; i ++){
            if(ASPx.GetElementDisplay(lbTable.rows[i]))
                visibleItemCount++;
        }
        return visibleItemCount;
    },
    GetDefaultDropDownHeight: function(listHeight, count){
        if(this.defaultDropDownHeight == ""){
            this.defaultDropDownHeight = ((listHeight / count) * this.dropDownRows) + "px";
        }
        return this.defaultDropDownHeight;
    },
    InitListBoxHeight: function () {
        var lb = this.GetListBoxControl();
        lb.GetMainElement().style.height = "0px";

        var lbHeight = 0;
        if(this.dropDownHeight == "") {
            lbHeight = this.GetListBoxHeightByContent();
        } else {
            lbHeight = this.GetListBoxHeightByServerValue();
        }

        lb.InitializePageSize();
        return lbHeight;
    },
    GetListBoxHeightByContent: function () {        
        var lb = this.GetListBoxControl(),
            lbScrollDiv = lb.GetScrollDivElement(),
            itemCount = this.GetVisibleItemCount(lb),
            hasVerticalScrollbar = itemCount > this.dropDownRows;
        if(!hasVerticalScrollbar)
            lbScrollDiv.style.height = lb.GetListTableHeight() + "px";
        var height = lb.GetListTableHeight();
        
        if(hasVerticalScrollbar)
            height = this.GetDefaultDropDownHeight(height, itemCount);
        else
            height = itemCount == 0 ? "0px" : height + "px";

        lbScrollDiv.style.height = height;
        height = lbScrollDiv.offsetHeight;

		// B253405
        height += ASPx.GetTopBottomBordersAndPaddingsSummaryValue(lb.GetMainElement());
        var lbHeaderDiv = lb.GetHeaderDivElement();
        if(ASPx.IsExists(lbHeaderDiv))
            height += lbHeaderDiv.offsetHeight;
        
        return height;
    },
    GetListBoxHeightByServerValue: function () {
        var lb = this.GetListBoxControl();
        var lbMainElement = lb.GetMainElement();
        var lbScrollDiv = lb.GetScrollDivElement()
        var height = this.dropDownHeight;

        lbMainElement.style.height = "0px";
        lbScrollDiv.style.height = "0px";
        lbMainElement.style.height = height;
        var trueLbOffsetHeight = lbMainElement.offsetHeight;
        var trueLbClientHeight = lbMainElement.clientHeight;
        lbScrollDiv.style.height = lbMainElement.clientHeight + "px";
        var lbHeightCorrection = lbMainElement.offsetHeight - trueLbOffsetHeight;
        lbScrollDiv.style.height = (trueLbClientHeight - lbHeightCorrection) + "px";
        height = lbMainElement.offsetHeight;
        return height;
    },
    InitListBoxWidth: function(){
        this.InitMainElementCache();
    
        var mainElement = this.GetMainElement();
        var lbScrollDiv = this.GetListBoxScrollDivElement();
        var lb = this.GetListBoxControl();
        var lbMainElement = lb.GetMainElement();
        var lbTable = lb.GetListTable();
        var scrollWidth = 0;
        
		lbMainElement.style.width = "";
        lbScrollDiv.style.paddingRight = "0px";
        lbScrollDiv.style.width = "100%";

        if(this.dropDownWidth != ""){
            lbMainElement.style.width = this.dropDownWidth;
            var width = lbMainElement.clientWidth;
            var scrollInfo = this.SetLbScrollDivAndCorrectionForScroll(lb, width, false);
            width = scrollInfo.scrollDivWidth;
            scrollWidth = scrollInfo.scrollWidth;
            if(!ASPx.Browser.IE) {
                var difference = lbTable.offsetWidth - lbScrollDiv.clientWidth;
                if(difference > 0){
                    lbMainElement.style.width = (lbMainElement.offsetWidth + difference) + "px";
                    lbScrollDiv.style.width = (lbMainElement.clientWidth)  + "px";
                }
            }
        } else {
            var width = lbTable.offsetWidth;
            var scrollInfo = this.SetLbScrollDivAndCorrectionForScroll(lb, width, true);
            width = scrollInfo.scrollDivWidth;
            scrollWidth = scrollInfo.scrollWidth;
            if(ASPx.Browser.Firefox && lbMainElement.offsetWidth < lbScrollDiv.offsetWidth)
                lbMainElement.style.width = "0%"; // FIX B19014
            var widthDifference = mainElement.offsetWidth - lbMainElement.offsetWidth;
            if(widthDifference > 0){
                lbScrollDiv.style.width = (width + widthDifference) + "px";
                var twoBorderSize = (lbMainElement.offsetWidth - lbMainElement.clientWidth);
                lbMainElement.style.width = (width + widthDifference + twoBorderSize) + "px"; // prevent lbMainElement size hover ScrollBar
            }
        }
        if(lb.IsMultiColumn())
            lb.CorrectMultiColumnHeaderWidth(scrollWidth);
        return lbScrollDiv.offsetWidth;
    },
    SetLbScrollDivAndCorrectionForScroll: function(lb, width, widthByContent){
        var lbScrollDiv = this.GetListBoxScrollDivElement();
        var scrollWidth = lb.GetVerticalScrollBarWidth();

        var browserCanHaveScroll = lb.GetVerticalOverflow(lbScrollDiv) == "auto";
        if(widthByContent && browserCanHaveScroll)
            width += scrollWidth;

        lbScrollDiv.style.width = width + "px";
        return {scrollDivWidth: width, scrollWidth: scrollWidth};
    },
    
    SelectIndexSilent: function(lb, index){
        this.lbEventLockCount ++;
        lb.SelectIndexSilentAndMakeVisible(index);
        this.ShowItemImageByIndex(index);
        this.lbEventLockCount --;
    },
    GetDecoratedText: function (text) {
        var lb = this.GetListBoxControl();
        var selectedItem = this.GetSelectedItem();
        
        if(this.displayFormat != null && lb.IsMultiColumn() && selectedItem != null){
            var textColumnCount = lb.GetItemTextCellCount();
            var texts = [textColumnCount];
            for(var i = 0; i < textColumnCount; i++){
                texts[i] = selectedItem.GetColumnTextByIndex(i)
            }
            return ASPx.Formatter.Format(this.displayFormat, texts);
        } else
            return ASPxClientComboBoxBase.prototype.GetDecoratedText.call(this, text);
    },
    CanApplyNullTextDecoration: function () {
        if(this.listBox || !this.loadDropDownOnDemand) {
			var value = this.isInitialized ? this.GetRawValue() : this.GetValue();
			var isValueNull = this.convertEmptyStringToNull && value === "" ? true : value === null;
            return (this.GetSelectedIndex() == -1 && isValueNull);
        } else
            return (this.GetValue() != null || this.GetText() != "");
    },
    
    ShowDropDownArea: function(isRaiseEvent){
		if(this.needToLoadDropDown) {
			this.EnsureDropDownLoaded();
			return;
		}

        var lb = this.GetListBoxControl();
        if(!lb || lb.GetItemCount() == 0) 
            return;

        if(!this.filterStrategy.IsShowDropDownAllowed()) {
            this.DropDownButtonPop(true); 
            return;
        }

        ASPxClientDropDownEditBase.prototype.ShowDropDownArea.call(this, isRaiseEvent);
	        
        this.EnsureListBoxSelectionSynchronized(lb);
        this.EnsureSelectedItemVisibleOnShow(lb);
        lb.CallbackSpaceInit();
    },
    ForceShowLoadingPanel: function() {
        if(this.GetItemCount() == 0 && !this.needToLoadDropDown) {
            var lb = this.GetListBoxControl();
            if(lb) {
                var sizes = { width: this.GetWidth(), height: 100 };
                ASPxClientDropDownEditBase.prototype.ShowDropDownArea.call(this, false, sizes);
                lb.SetHeight(sizes.height);
                lb.SetWidth(sizes.width);
            }
        }
    },
    FireFoxRequiresCacheScrollBar: function(){
		return ASPx.Browser.Firefox && ASPx.Browser.Version >= 3.6; // B155293
    },
	BrowserRequiresCacheScrollBar: function(){
        return ASPx.Browser.WebKitFamily || ASPx.Browser.Opera || this.FireFoxRequiresCacheScrollBar(); // B36477
	},
    HideDropDownArea: function(isRaiseEvent){
        if(this.filterStrategy)
            this.filterStrategy.OnBeforeHideDropDownArea();
        if(this.BrowserRequiresCacheScrollBar())
            this.CachedScrollTop();
        ASPxClientDropDownEditBase.prototype.HideDropDownArea.call(this, isRaiseEvent);
    },
    EnsureSelectedItemTextSynchronized: function(){
        var selectedItem = this.GetSelectedItem();
        if(!selectedItem) return;

		var textUnsynchronized = this.GetText() !== selectedItem.text;
		var textDecorationUnsynchronized = this.HasTextDecorators() && this.CanApplyTextDecorators() && 
			this.GetDecoratedText(selectedItem.text) !== this.GetInputElement().value; // BUG T255950
            
        var ddImageElement =  this.GetDropDownImageElement();
        var imageUrl = selectedItem.imageUrl != "" ? selectedItem.imageUrl : ASPx.Url.GetAbsoluteUrl(ASPx.EmptyImageUrl);
        var imageUrlUnsynchronized = ddImageElement && ASPx.ImageUtils.GetImageSrc(ddImageElement) !== imageUrl;

        if(textUnsynchronized || textDecorationUnsynchronized || imageUrlUnsynchronized) {
            this.SetTextBase(selectedItem.text);
            this.ShowItemImage(selectedItem);
            this.SetLastSuccessText(selectedItem.text);
            this.UpdateValueInput();
        }
    },
    EnsureListBoxSelectionSynchronized: function(listBox){
        var rawText = this.GetTextInternal();
        var lbItem = listBox.GetSelectedItem();
        var lbText = lbItem != null ? lbItem.text : "";
        if(rawText != lbText && rawText != null && lbText != ""){
            var newSelectedIndex = this.GetAdjustedSelectedIndexByText(listBox, rawText);
            listBox.SelectIndexSilent(newSelectedIndex, false);
        }
    },
    EnsureSelectedItemVisibleOnShow: function(listBox){
        if(this.BrowserRequiresCacheScrollBar()) // B36477 Before EnsureSelectedItemVisible will make decision about item's visibility
            listBox.RestoreScrollTopFromCache();
        listBox.EnsureSelectedItemVisible();
    },
    CachedScrollTop: function(){
		this.GetListBoxControl().CachedScrollTop();
        if(this.BrowserRequiresCacheScrollBar()){ // B155293
			var scrollDiv = this.GetListBoxScrollDivElement();
			if(scrollDiv != null)
            	scrollDiv.scrollTop = 0;
		}
    },
    IsFilterEnabled: function() {
        return this.incrementalFilteringMode != "None";
    },
    ChangeInputEnabled: function(element, enabled, readOnly){
        ASPxClientTextEdit.prototype.ChangeInputEnabled.call(this, element, enabled, readOnly || (this.isDropDownListStyle && !this.IsFilterEnabled()));
    },

    GetCallbackArguments: function(){
        var args = ASPxClientComboBoxBase.prototype.GetCallbackArguments.call(this);
        args += this.GetCallbackArgumentsInternal();
        return args;
    },
    GetCallbackArgumentsInternal: function(){
        var args = "";
        args = this.filterStrategy.GetCallbackArguments();
        return args;
    },
    ClearEditorValueByClearButtonCore: function() {
        this.enterKeyPressed = false;
        ASPxClientDropDownEditBase.prototype.ClearEditorValueByClearButtonCore.call(this);
        this.GetInputElement().value = '';
        if(this.IsFilterEnabled()) {
            this.filterStrategy.FilteringStop();
            if(this.isCallbackMode)
                this.filterStrategy.Filtering();
        }
        else if(this.isDropDownListStyle) {
            this.HideDropDownArea();
        }
    },
    IsFilterRollbackRequiredAfterApply: function() {
        var filterRollbackRequired = !this.GetSelectedItem() && !this.focused && this.InCallback();
        return filterRollbackRequired;
    },
    IsNullState: function() {
        return this.GetSelectedIndex() === -1 && ASPxClientComboBoxBase.prototype.IsNullState.call(this);
    },
    // LoadingPanel
    ShowLoadingPanel: function() {    
        var lb = this.GetListBoxControl();
        var loadingParentElement = lb.GetScrollDivElement().parentNode;
        if(!this.loadingPanelElement) {
            var loadingPanel = this.CreateLoadingPanelWithAbsolutePosition(loadingParentElement, loadingParentElement);
            lb.PreventMouseWheelOnElement(loadingPanel);    
        }
    },
    ShowLoadingDiv: function () {
        var lb = this.GetListBoxControl();
        var loadingParentElement = lb.GetScrollDivElement().parentNode;
        if(!this.loadingDivElement) {
            var loadingDiv = this.CreateLoadingDiv(loadingParentElement);
            lb.PreventMouseWheelOnElement(loadingDiv);
        }
    },
    HideLoadingPanelOnCallback: function(){
        return false;
    },
    // CallbackMode
    OnCallback: function(result) {
        if(ASPx.Browser.WebKitTouchUI) { // B211635 !!!TODO: Unite with Finallized
            if(this.needToLoadDropDown)
                this.OnLoadDropDownOnDemandCallback(result);
            window.setTimeout(function() {
                this.OnCallbackCore(result);
                this.DoEndCallback();
            }.aspxBind(this), 300);
        } else
            this.OnCallbackCore(result);
    },
    OnCallbackCore: function(result) {
        if(this.needToLoadDropDown) {
            if(!ASPx.Browser.WebKitTouchUI)
                this.OnLoadDropDownOnDemandCallback(result);
        } else if(this.filterStrategy.IsCallbackResultNotDiscarded()) {
            this.OnCallbackBeforeListBox();
            this.OnListBoxCallback(result)
            this.OnCallbackInternal(result);
            this.OnCallbackFinally(true);
        }
        this.callBackCoreComplete = true;
    },
    OnListBoxCallback: function(result) {
        var selectedValue = this.GetValue();
        var selectedText = this.GetText();
        function shouldSelectSilent(value, text) {
            return value === selectedValue || text === selectedText;
        }

        this.GetListBoxControl().OnCallback(result, shouldSelectSilent);
        
        this.EnsureSelectedItemTextSynchronized();
    },
    OnLoadDropDownOnDemandCallbackFinalized: function() {
        this.DoReInitializeAfterLoadDropDownOnDemand();
        this.HideLoadingPanel();
        this.HideLoadingDiv();
        var isCallbackForShowDropDownArea = !this.onLoadDropDownOnDemandCallbackFinalizedEventHandler;
        if(isCallbackForShowDropDownArea) {
            if(this.filterStrategy.IsShowDropDownAllowed())
                this.ShowDropDown();
        } else
            this.onLoadDropDownOnDemandCallbackFinalizedEventHandler();
        this.FixButtonState();
    },
    OnCallbackFinalized: function() {
        if(this.needToLoadDropDown)
            this.OnLoadDropDownOnDemandCallbackFinalized();
    },
    OnLoadDropDownOnDemandCallback: function(result) {
        var node = this.GetMainElement();
        var tempDiv = node.ownerDocument.createElement('div');
        tempDiv.innerHTML = eval(result);
        var len = tempDiv.childNodes.length;
        for(ind = 0; ind < len; ind++) {
            ASPx.InsertElementAfter(tempDiv.childNodes.item(0), node);
        }
    },
    ProcessCallbackError: function(errorObj){
        this.callBackCoreComplete = true;
        ASPxClientDropDownEditBase.prototype.ProcessCallbackError.call(this, errorObj);
    },
	DoEndCallback: function(){ 
        if(!this.callBackCoreComplete && ASPx.Browser.WebKitTouchUI) return;
	    this.filterStrategy.BeforeDoEndCallback();
	    ASPxClientDropDownEditBase.prototype.DoEndCallback.call(this);
		this.filterStrategy.AfterDoEndCallback();
        this.callBackCoreComplete = false; //B235715
    },
    RaiseEndCallback: function(){
        if(this.preventEndCallbackRising) {
            this.preventEndCallbackRising = false;
            ASPx.GetControlCollection().DecrementRequestCount(); // for testCafe tests only
        } else {
            ASPxClientDropDownEditBase.prototype.RaiseEndCallback.call(this);
        }
    },
    OnCallbackError: function(result, data){
        this.GetListBoxControl().OnCallbackError(result);
        this.OnCallbackFinally(false);
    },
    OnCallbackFinally: function(isSuccessful){
        this.filterStrategy.OnBeforeCallbackFinally();
        this.CollectionChanged();
        this.HideLoadingElements();
        if(this.isNeedToForceFirstShowLoadingPanel)
            this.isNeedToForceFirstShowLoadingPanel = false;
        this.isPerformCallback = false;
        this.changeSelectAfterCallback = 0;
        
        if(isSuccessful)
            this.filterStrategy.OnAfterCallbackFinally();
    },
    OnCallbackBeforeListBox: function(){
        var lb = this.GetListBoxControl();
        this.changeSelectAfterCallback = lb.changeSelectAfterCallback;
    },
    OnCallbackCorrectSelectedIndex: function(){
        var lb = this.GetListBoxControl();
        if(this.changeSelectAfterCallback != 0)
            this.SetTextInternal(lb.GetSelectedItem().text);
    },
    OnCallbackInternal: function(result){
        this.OnCallbackCorrectSelectedIndex();
        if(this.isPerformCallback) {
		    var lb = this.GetListBoxControl();
		    var resultIsEmpty = lb.GetItemCount() == 0;
		    if(resultIsEmpty)
		        this.HideDropDownArea(true);
	    } 
        this.filterStrategy.OnCallbackInternal(result);
    },
    DoReInitializeAfterLoadDropDownOnDemand: function() {
        this.InitializeListBoxOwnerName();
        this.needToLoadDropDown = false;
    }, //TODO resolve type for arg
    EnsureDropDownLoaded: function(callbackFunction) {
        if(this.needToLoadDropDown) {
            this.onLoadDropDownOnDemandCallbackFinalizedEventHandler = callbackFunction ? function() {
                callbackFunction();
            } : null;
            var args = this.FormatLoadDropDownOnDemandCallbackArguments();
            this.SendLoadDropDownOnDemandCallback(args);
        }
    },
    IsDropDownButtonClick: function(evt) {
        return ASPx.GetIsParent(this.GetDropDownButton(), ASPx.Evt.GetEventSource(evt));
    },
    OnDropDown: function(evt) {
    	var returnValue = ASPxClientDropDownEditBase.prototype.OnDropDown.call(this, evt);
        if(this.IsDropDownButtonClick(evt) && this.IsCanToDropDown()) {
            this.OnDropDownButtonClick(evt);
            return returnValue;
        }
        return true;
    },
    OnDropDownButtonClick: function(evt) {
        if(this.filterStrategy != null)
            this.filterStrategy.OnDropDownButtonClick();           
        this.ForceRefocusEditor(evt);
    },
    SendCallback: function(){
        if(!this.pcIsShowingNow)
            this.ShowLoadingElements();
        ASPxClientComboBoxBase.prototype.SendCallback.call(this);
    },

    IsAndroidKeyEventsLocked: function() {
        return ASPx.Browser.AndroidMobilePlatform && this.androidKeyEventsLocked;
    },
    LockAndroidKeyEvents: function() {
        this.androidKeyEventsLocked = true;
    },
    UnlockAndroidKeyEvents: function() {
        this.androidKeyEventsLocked = false;
    },
    CancelChangesOnUndo: function() {
        if(this.isCallbackMode) {
            this.SetTextInternal(this.lastSuccessText);
            this.filterStrategy.Filtering();
        }
        else {
            this.OnCancelChanges();
            this.filterStrategy.FilteringStopClient();
            this.GetListBoxControl().EnsureSelectedItemVisible();
        }
    },
    OnKeyDown: function(evt) {
        if(this.IsCtrlZ(evt)) {
            this.CancelChangesOnUndo();
            return ASPx.Evt.PreventEvent(evt);
        }
        var isClearKey = ASPx.Data.ArrayIndexOf([ASPx.Key.Delete, ASPx.Key.Backspace], evt.keyCode) >= 0;
        if(isClearKey && this.allowNull && (this.IsAllTextSelected() || this.isDropDownListStyle && !this.IsFilterEnabled()))
            this.ClearEditorValueAndForceOnChange();
        else if(this.IsAndroidKeyEventsLocked())
            return ASPx.Evt.PreventEventAndBubble(evt);
        else
            ASPxClientComboBoxBase.prototype.OnKeyDown.call(this, evt);
    },
    IsAllTextSelected: function() {
        var input = this.GetInputElement();
        var textLength = input.value.length;
        if(textLength === 0)
            return false;
        var selectionInfo = ASPx.Selection.GetExtInfo(input);
        var selectionLength = selectionInfo.endPos - selectionInfo.startPos;
        return selectionLength === textLength;
    },
    
    // Key Board support
    SelectNeighbour: function (step){
        if((this.isToolBarItem && !this.droppedDown) || this.readOnly) return;
        
        var lb = this.GetListBoxControl();
        var step = this.filterStrategy.GetStepForClientFiltrationEnabled(lb, step);
        this.SelectNeighbourInternal(lb, step);
    },
    SelectNeighbourInternal: function(lb, step){
        if(this.droppedDown)
            this.lbEventLockCount ++;
        
        lb.SelectNeighbour(step);
        
        if(this.droppedDown) {
            var selectedItem = lb.GetSelectedItem();
            if (selectedItem) {
                this.OnSelectionChangedCore(selectedItem.text, selectedItem, true);
            }
            this.lbEventLockCount --;
        }
    },
    
    GetFocusSelectAction: function() {
        return (this.isToolbarItem || ASPx.Browser.IE && this.refocusWhenInputClicked) ? null : "all";
    },
    
    OnSpecialKeyDown: function(evt){
        if(this.filterStrategy)
            this.filterStrategy.OnSpecialKeyDown(evt);
        return ASPxClientEdit.prototype.OnSpecialKeyDown.call(this, evt);
    },
    OnArrowUp: function(evt){
        if(!this.isInitialized) return true;
        var isProcessed = ASPxClientDropDownEditBase.prototype.OnArrowUp.call(this, evt);
        if(!isProcessed && this.filterStrategy.IsFilterMeetRequirementForMinLength())
            this.SelectNeighbour(-1);
        return true;
    },
    OnTextChanged: function(){
        if(!this.IsFocusEventsLocked())
            ASPxClientComboBoxBase.prototype.OnTextChanged.call(this);
    },
    OnTextChangedInternal: function() {
		if(!this.forceValueChanged) {
			var preserveFilterInInput = this.InCallback() && this.filterStrategy.currentCallbackIsFiltration;
        	if(preserveFilterInInput) return;
		}

        ASPxClientComboBoxBase.prototype.OnTextChangedInternal.call(this);
        this.filterStrategy.OnTextChanged();
    },
    OnArrowDown: function(evt){
        if(!this.isInitialized) return true;
        var isProcessed = ASPxClientDropDownEditBase.prototype.OnArrowDown.call(this, evt);
        if(!isProcessed && this.filterStrategy.IsFilterMeetRequirementForMinLength())
            this.SelectNeighbour(1);
        return true;
    },
    OnPageUp: function(){
        if(!this.isInitialized || !this.filterStrategy.IsFilterMeetRequirementForMinLength()) return true;
        return this.OnPageButtonDown(false);
    },
    OnPageDown: function(){
        if(!this.isInitialized || !this.filterStrategy.IsFilterMeetRequirementForMinLength()) return true;
        return this.OnPageButtonDown(true);
    },
    OnPageButtonDown: function(isDown){
        if(!this.isInitialized) return true;
        var lb = this.GetListBoxControl();
        if(lb){
            var direction = isDown ? 1 : -1;
            this.SelectNeighbour(lb.scrollPageSize * direction);
        }
        return true;
    },
    OnHomeKeyDown: function(evt){
        if(!this.isInitialized) return true;
        return this.OnHomeEndKeyDown(evt, true);
    },
    OnEndKeyDown: function(evt){
        if(!this.isInitialized) return true;
        return this.OnHomeEndKeyDown(evt, false);
    },
    OnHomeEndKeyDown: function(evt, isHome){
        if(!this.isInitialized) return true;
        var input = this.GetValueInput();
        if((input && input.readOnly) || evt.ctrlKey){
            var lb = this.GetListBoxControl();
            var count = lb.GetItemCount();
            this.SelectNeighbour(isHome ? -count : count);
            return true;
        }
        return false;
    },
    OnEscape: function() {
        this.filterStrategy.OnEscape();
        return ASPxClientComboBoxBase.prototype.OnEscape.call(this);
    },
    OnEnter: function(){
        if(!this.isInitialized) return true;
        if(this.isDropDownListStyle) this.enterKeyPressed = true;
        if(this.filterStrategy.IsCloseByEnterLocked()) return;
        this.enterProcessed = this.droppedDown; // hack for defaultbutton
        if(!this.isEnterLocked) { //Prevent to extra fast push enter while filtering continue
            this.OnApplyChangesAndCloseWithEvents();
            this.filterStrategy.OnAfterEnter();
        }
        return this.enterProcessed;
    },
    OnTab: function(evt){
        if(!this.isInitialized) 
            return true;
        this.filterStrategy.OnTab();
    },

    OnApplyChanges: function(){
        if(!this.focused || (this.isDropDownListStyle && !this.IsFilterEnabled())) return;
        this.OnApplyChangesInternal();
    },
    OnApplyChangesAndCloseWithEvents: function() {
        this.OnApplyChangesInternal();
        this.HideDropDownArea(true);
    },
    OnApplyChangesInternal: function(){
        var inCallback = this.InCallback();
        var lb = this.GetListBoxControl();

        var isChanged = this.HasChanges();
        var isRollback = false;
        if(isChanged){
            var text = this.GetCurrentText();
            if(this.filterStrategy.IsFilterTimerActive()) {
				this.filterStrategy.FilterNowAndApply();
                return;
            }
            var adjustedSelectedIndex = this.GetAdjustedSelectedIndexByText(lb, text);
            var isNullState = this.allowNull && !text;
            var rollbackRequired = this.isDropDownListStyle && adjustedSelectedIndex < 0 && !isNullState;
            if(rollbackRequired) {
                var rollbackToItem = lb.GetSelectedItem();
                isRollback = rollbackToItem == null && this.isCallbackMode; 
                if(isRollback) {
                    this.RollbackValueInputValue();
                    this.RollbackTextInputValue();
                }
                text = rollbackToItem != null ? rollbackToItem.text : this.lastSuccessText;
            } 
            if(!isRollback) {
                var adjustedSelectedItem = this.GetItem(adjustedSelectedIndex);
                var adjustedSelectedItemText = adjustedSelectedItem && adjustedSelectedItem.text;
                this.SetText(adjustedSelectedItemText || text);
                this.OnChange();
            }
            else if(!inCallback) {
                this.filterStrategy.OnFilterRollback();
            }
        } 
    },
    GetCurrentText: function(){
        var textDecorated = !this.focused && this.GetRawValue() != null;
        return textDecorated ? this.GetRawValue() : this.GetInputElement().value;
    },
    GetCurrentValue: function(){
        return this.listBox.GetSelectedItem() ? this.listBox.GetSelectedItem().value : this.GetValue();
    },
    HasChanges: function(){
        return this.lastSuccessText != this.GetCurrentText() || this.lastSuccessValue != this.GetCurrentValue();
    },
    OnButtonClick: function(number){
        if(number != this.dropDownButtonIndex)
            this.droppedDown ? this.OnApplyChangesAndCloseWithEvents(false) : ASPxClientComboBoxBase.prototype.OnTextChanged.call(this);
        ASPxClientButtonEditBase.prototype.OnButtonClick.call(this, number);
    },
    OnCancelChanges: function(){
        var isCancelProcessed = ASPxClientDropDownEditBase.prototype.OnCancelChanges.call(this);
        this.filterStrategy.OnCancelChanges();
        var lb = this.GetListBoxControl();
        if(ASPx.IsExists(lb)) {
            var index = this.GetAdjustedSelectedIndexByText(lb, this.lastSuccessText);
            this.SelectIndexSilent(lb, index);
        }
        return isCancelProcessed;
    },
    ShouldCloseOnMCMouseDown: function () {
        return this.GetInputElement().readOnly;
    },
    OnCloseUp: function(evt){
        var evt = ASPx.Evt.GetEvent(evt);
        if(ASPx.Browser.Firefox && evt.type == "mouseup" && ASPx.Evt.GetEventSource(evt).tagName == "DIV") { // Prevent FF closing DropDownArea by DropDownArea ScrollBar MouseUp
            var scrollDiv = this.GetListBoxControl().GetScrollDivElement();
            var scrollDivID = scrollDiv ? scrollDiv.id : "";
            if(scrollDivID == ASPx.Evt.GetEventSource(evt).id) 
                return;
        }
        ASPxClientDropDownEditBase.prototype.OnCloseUp.call(this, evt);
    },
    OnDDButtonMouseMove: function(evt){
        return (this.droppedDown ? ASPx.Evt.CancelBubble(evt) : true);
    },
    CloseDropDownByDocumentOrWindowEvent: function(causedByWindowResizing){
        this.filterStrategy.OnCloseDropDownByDocumentOrWindowEvent(causedByWindowResizing);
    },
    IsCanToDropDown: function() {
        if(this.loadDropDownOnDemand) {
            var lb = this.GetListBoxControl();
            var itemCount = lb ? lb.GetItemCount() : 0;
            return (!this.needToLoadDropDown && itemCount > 0);
        }
        return ASPxClientDropDownEditBase.prototype.IsCanToDropDown.call(this);
    },
    OnPopupControlShown: function(){
        if(!this.isInitialized) return;
        if(ASPx.Browser.Opera) // B36477
            this.GetListBoxControl().RestoreScrollTopFromCache();
        if(this.lockListBoxClick)
            this.RemoveLockListBoxClick();
        if(this.InCallback()) 
            this.ShowLoadingDivAndPanel();
        ASPxClientDropDownEditBase.prototype.OnPopupControlShown.call(this);
    },
    OnLBSelectedIndexChanged: function(){
        if(!this.lockListBoxClick) {
            this.OnSelectChanged();
			if(this.IsNavigationOnKeyPress()){
				if(!this.droppedDown) {
                    ASPx.Selection.Set(this.GetInputElement());
        		}
			} else if(this.focused) {
				this.ForceRefocusEditor();
            }
        }
    },
	IsNavigationOnKeyPress: function() {
		var lb = this.GetListBoxControl();
		return lb.IsScrollOnKBNavigationLocked();
	},
    OnListBoxItemMouseUp: function(evt){
        if(!this.lockListBoxClick && !this.InCallback()){
            this.OnApplyChangesInternal();
            this.OnCloseUp(evt);
        }
    },
    OnMouseWheel: function(evt){
        if(this.allowMouseWheel && !this.droppedDown && this.filterStrategy.IsFilterMeetRequirementForMinLength()) {
            var wheelDelta = ASPx.Evt.GetWheelDelta(evt);
            if(wheelDelta > 0)
			    this.SelectNeighbour(-1);
		    else  if(wheelDelta < 0)
			    this.SelectNeighbour(1);
			return ASPx.Evt.PreventEvent(evt);
		}
    },
    OnOpenAnotherDropDown: function(){
        this.OnApplyChangesAndCloseWithEvents();
    },

    ParseValue: function() {
        var forceValueChanged = !this.IsValueChanging() && this.IsValueChangeForced();
        var newText = this.GetInputElement().value;
        var oldText = this.GetText();
        var isNeedToParseValue = oldText != newText;
        if(isNeedToParseValue || forceValueChanged) {
            this.StartValueChanging();
            if(this.CanTextBeAccepted(newText, oldText) || forceValueChanged){
                this.SetText(newText);
                this.OnChange();
            } else
                this.SetTextInternal(oldText);
            this.EndValueChanging();
        }
    },
    CanTextBeAccepted: function(newText, oldText){
        var notAnyTextCanBeAccepted = this.isDropDownListStyle;
        if(notAnyTextCanBeAccepted){
            var lb = this.GetListBoxControl();
            var newTextPresentInItemCollection = this.GetAdjustedSelectedIndexByText(lb, newText) != -1;
            return newTextPresentInItemCollection;
        }
        var wasTextErased = !newText && oldText;
        if((!wasTextErased) && this.nullText && this.CanApplyNullTextDecoration()) {
            return false;
        }
        return true;
    },

    MakeItemVisible: function(index){
        var lb = this.GetListBoxControl();
        lb.MakeItemVisible(index);
    },
    PerformCallback: function(arg) {
        this.isPerformCallback = true;
        this.filterStrategy.PerformCallback();
		
		if(this.needToLoadDropDown) {
            var formatCallbackArg = function(prefix, arg) {  //TODO Refactor with FormatCallbackArg from ListEdit.js to Callback.js
                arg = arg.toString();
                return (ASPx.IsExists(arg) ? prefix + "|" + arg.length + ';' + arg + ';' : "");
            };
            if(arg === undefined || arg == null)
                arg = "";
            var performArgs = formatCallbackArg("LECC", arg);
            this.onLoadDropDownOnDemandCallbackFinalizedEventHandler = function() {
                var selectedItem = this.listBox.GetSelectedItem();
                if(selectedItem != null)
                    this.SetTextInternal(selectedItem.text);
                var lb = this.GetListBoxControl();
                if(lb)
                    lb.SetCustomCallbackArg(performArgs);
            };
            var loadItemsRangeArgs = formatCallbackArg("LBCRI", "0:-2");
            var args = this.FormatLoadDropDownOnDemandCallbackArguments(performArgs + loadItemsRangeArgs);
            this.SendLoadDropDownOnDemandCallback(args);
        } else {
            this.ClearItemsInternal();
            this.GetListBoxControl().PerformCallback(arg);
        }
    },

    ClearItemsInternal: function(){
        ASPxClientComboBoxBase.prototype.ClearItemsInternal.call(this);
        var lbScrollDiv = this.GetListBoxScrollDivElement();
        if(lbScrollDiv)
            lbScrollDiv.scrollTop = "0px";
    },

    SendLoadDropDownOnDemandCallback: function(args) {
        this.ShowInputLoadingPanel();
        this.SendSpecialCallback(args);
    },

    ShowInputLoadingPanel: function() {
        var inputElement = this.GetInputElement();
        var parentElement = inputElement.parentNode;
        this.CreateLoadingDiv(parentElement, inputElement);
        this.CreateLoadingPanelWithAbsolutePosition(parentElement, inputElement);
    },

    FormatLoadDropDownOnDemandCallbackArguments: function(arguments) {
		var internalArgs = ASPx.IsExists(arguments) ? arguments.toString() : "";
        var resultArgs = loadDropDownOnDemandCallbackPrefix + "|0;;";
        return resultArgs + internalArgs;
    },

    CorrectCaretPositionInChrome: function() {
        if(!ASPx.Browser.Chrome) return;
        var currentSelection = ASPx.Selection.GetInfo(this.GetInputElement());
        if(currentSelection.startPos === currentSelection.endPos)
            ASPx.Selection.SetCaretPosition(this.GetInputElement(), 0);
    },
    ForceRefocusEditor: function(evt, isNativeFocus) {
        this.CorrectCaretPositionInChrome();
        this.refocusWhenInputClicked = this.IsElementBelongToInputElement(ASPx.Evt.GetEventSource(evt));
        ASPxClientEdit.prototype.ForceRefocusEditor.call(this, evt, isNativeFocus);
    },
    OnFocus: function () {
        if(this.needToLoadDropDown) {
            var args = this.FormatLoadDropDownOnDemandCallbackArguments();
            this.SendLoadDropDownOnDemandCallback(args);
        } else
            this.FixButtonState();
        ASPxClientDropDownEditBase.prototype.OnFocus.call(this);
    },
    FixButtonState: function() {
        var lb = this.GetListBoxControl();
        if(lb && this.ddButtonPushed) {
            this.DropDownButtonPop(true);
        }
    }
});
ASPxClientComboBox.Cast = ASPxClientControl.Cast;

var CallbackProcessingStateManager = function(){
	var states = { 
		Default:            0,
		Rollback:           1,
		Apply:              2,
		ApplyAfterRollback: 3
	}
	
	var state = states.Default;
	
	return {
        ResetState: function() {
            state = states.Default;
        },
        IsApply: function() {
            return state == states.Apply;
        },
        IsRollback: function() {
            return state == states.Rollback;
        },
        IsApplyAfterRollback: function() {
            return state == states.ApplyAfterRollback;
        },
		NoFilterPostProcessing: function() {
            return !this.IsApply() && !this.IsApplyAfterRollback() && !this.IsRollback();
        },
		SetRollback: function() {
			if(state == states.Default)
				state = states.Rollback;
		},
		SetApply: function() {
			if(state == states.Default)
				state = states.Apply;
		},
        SetApplyAfterRollback: function() {
            if(state == states.Default || state == states.Apply)
                state = states.ApplyAfterRollback;
        },
		OnCallbackResult: function(filterRollbackRequired) {
            if(state == states.Apply) {
                state = filterRollbackRequired ? states.ApplyAfterRollback : states.Default;
                return;
            }
			if(state == states.Rollback) {
				state = states.ApplyAfterRollback;
                return;
			}
			if(state == states.ApplyAfterRollback) {
				state = states.Default;
                return;
			}
		}
	}
};

var ASPxComboBoxDisableFilteringStrategy = ASPx.CreateClass(null, {
    constructor: function(comboBox) {
        this.comboBox = comboBox;
        this.isDropDownListStyle = this.comboBox.isDropDownListStyle;
        this.callbackProcessingStateManager = new CallbackProcessingStateManager();
    },
    Initialize: function() {},
    
    AfterDoEndCallback: function() {},
    BeforeDoEndCallback: function() {},
    IsCallbackResultNotDiscarded: function() { return true; },
    IsCloseByEnterLocked: function() { return false; },
    IsFilterTimerActive: function() { return false; },
    
    OnAfterCallbackFinally: function() {
        if(this.callbackProcessingStateManager.IsApply() || this.callbackProcessingStateManager.IsApplyAfterRollback()) {
            this.comboBox.OnApplyChangesAndCloseWithEvents();
            this.callbackProcessingStateManager.ResetState();
        }
    },
    OnAfterEnter: function() {}, 
    OnApplyChanges: function() {},
    OnBeforeCallbackFinally: function() {},
    OnBeforeHideDropDownArea: function() {},
    OnCallbackInternal: function(result) {},
    OnCancelChanges: function () {
        this.OnFilterRollback();
    },
    OnFilterRollback: function() {},
    OnDropDownButtonClick: function() {},
    OnEscape: function() {},
    OnFilteringKeyUp: function (evt) { },
    OnFilterRollback: function (withoutCallback) { },
    SetFilter: function (value) {},
    Filtering: function() {},
    OnSelectionChanged: function() {},
    OnSpecialKeyDown: function(evt) {},
    OnTab: function() {
        if(this.comboBox.InCallback())
            this.callbackProcessingStateManager.SetApply()
        else
            this.comboBox.OnApplyChangesAndCloseWithEvents();
    },
    OnCloseDropDownByDocumentOrWindowEvent: function(causedByWindowResizing) {
        if(!this.comboBox.InCallback()) {
            this.comboBox.OnApplyChangesInternal();
            ASPxClientDropDownEditBase.prototype.CloseDropDownByDocumentOrWindowEvent.call(this.comboBox, causedByWindowResizing);
        }
        else
            this.callbackProcessingStateManager.SetApply();
    },
    OnTextChanged: function() {},
    
    PerformCallback: function() {},
    GetCallbackArguments: function() { return ""; },
    GetInputElement: function() {
        return this.comboBox.GetInputElement();
    },
    GetListBoxControl: function() {
        return this.comboBox.GetListBoxControl();
    },
    GetCurrentSelectedItemCallbackArguments: function () {
        return ASPx.FilteringUtils.FormatCallbackArg(currentSelectedItemCallbackPrefix, "");
    },
    GetStepForClientFiltrationEnabled: function(lb, step) {
        return step;
    },
    IsFilterMeetRequirementForMinLength: function() {
        return true;   
    },
    IsShowDropDownAllowed: function() {
        return this.IsFilterMeetRequirementForMinLength();
    }
});

var ASPxComboBoxIncrementalFilteringStrategy = ASPx.CreateClass(ASPxComboBoxDisableFilteringStrategy, {
    constructor: function(comboBox) {
        this.constructor.prototype.constructor.call(this, comboBox);
        
        this.currentCallbackIsFiltration = false;
        this.refiltrationRequired = false;
        this.isEnterLocked = false;
        
        this.filter = "";
        this.filterInitialized = false;
        this.filterTimerId = -1;
        this.filterTimer = comboBox.filterTimer;
        this.hasInputBeenChanged = false;
    },
    
    Initialize: function() {
        ASPx.Evt.AttachEventToElement(this.GetInputElement(), "keydown", this.OnInputKeyDown);
        ASPx.Evt.AttachEventToElement(this.GetInputElement(), "keyup", this.OnInputKeyUpOrPaste);
        ASPx.Evt.AttachEventToElement(this.GetInputElement(), "paste", function(evt) { this.OnInputKeyUpOrPaste(evt, true); }.aspxBind(this));
    },
    OnInputKeyDown: function(evt) {
        var cb = ASPx.GetDropDownCollection().GetFocusedDropDown();
        if(cb != null) {
            var cbInputElement = cb.GetInputElement();
            var selInfo = ASPx.Selection.GetInfo(cbInputElement);
            if(selInfo.startPos != selInfo.endPos && evt.keyCode == ASPx.Key.Backspace) {
                var currentText = cbInputElement.value;
                var cutText = currentText.slice(0, selInfo.startPos) + currentText.slice(selInfo.endPos);
                var newFilter = ASPx.Str.PrepareStringForFilter(cutText);
                if(cutText != currentText && cb.filterStrategy.FilterCompareLower(newFilter)) {
                    cbInputElement.value = cutText;
                    ASPx.Selection.SetCaretPosition(cbInputElement, selInfo.startPos);
                }
            }
        }
    },
    OnInputKeyUpOrPaste: function(evt, needSyncRawValue) {
        var cb = ASPx.GetDropDownCollection().GetFocusedDropDown();
        if(cb != null) {
            if(cb.IsAndroidKeyEventsLocked())
                return ASPx.Evt.PreventEventAndBubble(evt);
            if(needSyncRawValue) 
                cb.SyncRawValueIfHasTextDecorators(true);
            cb.filterStrategy.OnFilteringKeyUp(evt);
        }
    },
    OnEscape: function() {
        this.FilterStopTimer();
        if(this.comboBox.InCallback())
            this.callbackProcessingStateManager.SetRollback();
    },
    ClearFilter: function() {
        this.filter = "";
        this.filterInitialized = false;
    },
    ClearFilterApplied: function() {
        this.filterInitialized = false;
    },
    FilterApplied: function() {
        return this.filterInitialized;
    },
    SetFilter: function(value){
        this.filter = value;
        this.filterInitialized = true;
    },
    FilterCompare: function(value){
        if(!this.filterInitialized && this.hasInputBeenChanged)
            return false;
        return this.filter == value;
    },
    FilterCompareLower: function(value){
        if(!this.filterInitialized)
            return false;
        return ASPx.Str.PrepareStringForFilter(this.filter) == value;
    },
    
    OnCallbackInternal: function(result){
        if(!this.comboBox.isPerformCallback) 
            this.RefreshHighlightInItems();
        if(!this.currentCallbackIsFiltration)
            return;
        var lb = this.GetListBoxControl();
        if(lb.GetItemCount() == 0)
            this.comboBox.HideDropDownArea(true);
        else 
            this.OnFilterCallbackWithResult(lb);        
        this.isEnterLocked = false;
    },
    OnBeforeCallbackFinally: function() {
        this.currentCallbackIsFiltration = false;
    },
    OnAfterCallbackFinally: function() {
        var filterRollbackRequired = false;
        
        if(this.callbackProcessingStateManager.IsApply() || this.callbackProcessingStateManager.IsApplyAfterRollback()) {
            this.ApplyAfterFinalFiltrationCallback();
            filterRollbackRequired = this.IsFilterRollbackRequiredAfterApply();
        }
        
        if(this.callbackProcessingStateManager.IsRollback())
            filterRollbackRequired = true;

        this.callbackProcessingStateManager.OnCallbackResult(filterRollbackRequired);

        if(filterRollbackRequired)
            this.RollbackFilterDeferred();
    },
    ApplyAfterFinalFiltrationCallback: function() {
        var itemIsSelected = this.TrySelectItemAfterFilter();

        this.comboBox.HideDropDownArea();
        var requiredApplyFailed = this.comboBox.isDropDownListStyle && !itemIsSelected && this.comboBox.focused && this.callbackProcessingStateManager.IsApply();
        if(requiredApplyFailed)
            this.comboBox.ShowDropDownArea(true);
        
        var needApplyChanges = itemIsSelected || !this.comboBox.focused;
        if(needApplyChanges)
            this.comboBox.OnApplyChangesInternal();
    },
    IsFilterRollbackRequiredAfterApply: function() {
        return this.comboBox.IsFilterRollbackRequiredAfterApply();
    },
    RollbackFilterDeferred: function() {
        window.setTimeout(function() { 
            this.OnFilterRollback(); 
        }.aspxBind(this), 0);
    },
    OnEndFiltering: function(visibleCollectionChanged) {
		if(visibleCollectionChanged) 
			this.comboBox.VisibleCollectionChanged();
        this.HighlightTextInItems();
	},
    OnFilteringKeyUp: function(evt){
        if(this.comboBox.InCallback() || !this.comboBox.GetEnabled()) return;

        if(ASPx.FilteringUtils.EventKeyCodeChangesTheInput(evt)){
            this.FilterStopTimer();
            this.FilterStartTimer();
        }
    },
    OnFilterCallbackHighlightAndSelect: function(lb){
        var firstItemText = lb.GetItem(0).text;
        var isTextClearing = !this.isDropDownListStyle && this.FilterCompare("") && !this.FilterCompare(firstItemText);
        if(!isTextClearing){
            var isFilterRollBack = this.CheckForFilterRollback(lb, firstItemText);
            var isNonFilterChangingCallback = (lb.GetSelectedItem() == null);
            if(isFilterRollBack || isNonFilterChangingCallback) {
                this.HighlightTextAfterCallback(firstItemText);
            }
        }
    },
    OnFilterCallbackWithResult: function(lb) {
        this.OnFilterCallbackHighlightAndSelect(lb);
        var isNeedToKeepDropDownVisible = !this.comboBox.isPerformCallback && this.callbackProcessingStateManager.NoFilterPostProcessing();
        if(isNeedToKeepDropDownVisible)
            this.EnsureShowDropDownArea();
        this.OnEndFiltering();
    },
    OnSpecialKeyDown: function(evt) {
        if(ASPx.FilteringUtils.EventKeyCodeChangesTheInput(evt)) {
            this.FilterStopTimer();
            this.hasInputBeenChanged = true;
        }
    },
    OnFilterRollback: function () {
        if(this.comboBox.InCallback() && this.currentCallbackIsFiltration)
            return;
        if(this.comboBox.isCallbackMode && this.FilterApplied()) {
            this.callbackProcessingStateManager.SetApplyAfterRollback();
            if(this.comboBox.GetText() != "" && this.isDropDownListStyle) {
                this.comboBox.GetListBoxControl().ClearItems();
                this.comboBox.SendSpecialCallback(this.GetCurrentSelectedItemCallbackArguments());
            } else
                this.Filtering();

            this.SetFilter(this.comboBox.GetText());
			this.ClearFilterApplied();
        }
    },
    //Callback
    BeforeDoEndCallback: function() {
        if(this.refiltrationRequired)
	        this.comboBox.preventEndCallbackRising = true;
    },
    AfterDoEndCallback: function() {
        if(this.refiltrationRequired){
		    this.refiltrationRequired = false;
            window.setTimeout(function() {
                var cb = ASPx.GetControlCollection().Get(this.comboBox.name);
                if(cb != null) cb.filterStrategy.Filtering();
            }.aspxBind(this), 0);
		}
    },
    GetCallbackArguments: function() { 
        var args = "";
        if(!this.FilterCompare(""))
            args = this.GetCallbackArgumentFilter(this.filter);
        return args;
    },    
    GetCallbackArgumentFilter: function(value){
        var callbackPrefix = this.isDropDownListStyle ? correctFilterCallbackPrefix : loadFilteredItemsCallbackPrefix;
        return ASPx.FilteringUtils.FormatCallbackArg(callbackPrefix, value);
    },
    PerformCallback: function() {
            this.ClearFilter();
    },
    SendFilteringCallback: function(){
        this.currentCallbackIsFiltration = true;
        this.comboBox.SendCallback();
    },
    IsCallbackResultNotDiscarded: function(){
        var discardCallbackResult = this.FilterChanged() && this.currentCallbackIsFiltration;
        if(discardCallbackResult)
			this.refiltrationRequired = true;
        return !discardCallbackResult;
    },
    //Filter timer
    IsFilterTimerActive: function() {
        return (this.filterTimerId != -1);
    },
    FilterStopTimer: function() {
        this.filterTimerId = ASPx.Timer.ClearTimer(this.filterTimerId);
    },
    FilterStartTimer: function() {
        this.isEnterLocked = true;
        this.filterTimerId = window.setTimeout(function() {
            var cb = ASPx.GetControlCollection().Get(this.comboBox.name);
            if(cb != null) {
                cb.filterStrategy.Filtering();
                cb.EnsureClearButtonVisibility();
            }
        }.aspxBind(this), this.filterTimer);
    },

    CheckForFilterRollback: function(lb, firstItemText){
        var isHasCorrection = false;
        var filter = ASPx.Str.PrepareStringForFilter(this.filter);
        firstItemText = ASPx.Str.PrepareStringForFilter(firstItemText);
        
        while(!this.IsSatisfy(firstItemText, filter)){
            filter = filter.slice(0, -1);
            isHasCorrection = true;
        }
        if(isHasCorrection){
            this.SetFilter(this.filter.substring(0, filter.length));
            this.GetInputElement().value = this.filter;
        } 
        return isHasCorrection;
    },
    EnsureShowDropDownArea: function(){
        if(!this.comboBox.droppedDown)
            this.comboBox.ShowDropDownArea(true);
    },
    FilterChanged: function(){
		var currentFilter = this.GetInputElementWithFilterValue();
        return !this.FilterCompareLower(ASPx.Str.PrepareStringForFilter(currentFilter));
    },
    FilteringStop: function(){
        this.isEnterLocked = false;
        if(!this.comboBox.isCallbackMode)
            this.FilteringStopClient();
    },
    FilteringStopClient: function(){
        this.MakeAllItemsClientVisible();
        this.ClearFilter();
        this.RemoveHighlightInItems();
    },
    MakeAllItemsClientVisible: function() {
        this.MakeAllItemsClientVisibleCore();
        this.comboBox.VisibleCollectionChanged();
    },
    MakeAllItemsClientVisibleCore: function() {
        var lb = this.GetListBoxControl();
        var listTable = lb.GetListTable();
        var count = lb.GetItemCount();
        for(var i = 0; i < count; i ++)
            ASPx.SetElementDisplay(listTable.rows[i], true);
    },
    FilteringBackspace: function(){
        var input = this.GetInputElement();
        ASPx.StartWithFilteringUtils.RollbackOneSuggestedChar(input);
        this.FilterStartTimer();
    },
    GetMinFilterLengthProcessed: function() {
        if(!this.IsFilterMeetRequirementForMinLength()) {
            this.comboBox.HideDropDownArea(true);
            var lb = this.GetListBoxControl();
            this.callbackProcessingStateManager.ResetState();
            lb.SelectIndexSilent(-1, false); 
            return true;
        }
        return false;
    },
    GetInputElementWithFilterValue: function() {
        return ASPx.IsExists(this.comboBox.GetRawValue()) ? this.comboBox.GetRawValue() : this.comboBox.GetInputElement().value;
    },
    Filtering: function(){
        this.FilterStopTimer();

        var newFilter = this.GetInputElementWithFilterValue();
		var filterChanged = !this.FilterCompare(newFilter);
        if(filterChanged){
            this.SetFilter(newFilter);
            if(this.GetMinFilterLengthProcessed())
                return;

			if(this.callbackProcessingStateManager.NoFilterPostProcessing())
            	this.EnsureShowDropDownArea();

            if(this.comboBox.isCallbackMode) {
                if(!this.comboBox.droppedDown && this.comboBox.isNeedToForceFirstShowLoadingPanel)
                    this.comboBox.ForceShowLoadingPanel();
                this.FilteringOnServer();
	        } else {
                this.FilteringOnClient(); 
                this.callbackProcessingStateManager.ResetState();
            }
        } else {
            this.isEnterLocked = false;
            this.callbackProcessingStateManager.ResetState();
        }
    },
    FilteringOnServer: function(){
        if(!this.comboBox.InCallback()){
            var listBox = this.GetListBoxControl();
            
            listBox.ClearItems(); // TODO Extract method in 8.x with ListBoxAPI
            listBox.serverIndexOfFirstItem = 0;
            listBox.SetScrollSpacerVisibility(true, false);
            listBox.SetScrollSpacerVisibility(false, false);
            
            this.SendFilteringCallback();
        }
    },
    FilteringOnClient: function() {
        this.RemoveHighlightInItems();
        var filter = ASPx.Str.PrepareStringForFilter(this.filter),
            lb = this.GetListBoxControl(),
            listTable = lb.GetListTable(),
            count = lb.GetItemCount(),
            text = "",
            isSatisfy = false,
            firstSatisfyItemIndex = -1;
        
        if(this.isDropDownListStyle) {
            var coincide = new Array(count);
            var maxCoincide = 0;
            for(var i = count - 1; i >= 0; i--) {
                coincide[i] = this.GetCoincideCharCount(ASPx.Str.PrepareStringForFilter(lb.GetItem(i).text), filter);
                if(coincide[i] > maxCoincide)
                    maxCoincide = coincide[i];
            }
            filter = this.filter.substr(0, maxCoincide);
            if(ASPx.IsExists(this.comboBox.GetRawValue()))
                this.comboBox.SetRawValue(filter);
            else
                this.comboBox.GetInputElement().value = filter;
        }
        if(ASPx.Browser.IE && ASPx.Browser.Version > 9) //T197866
            ASPx.SetElementDisplay(listTable, false);
        for(var i = 0; i < count; i ++) {
            text = lb.GetItem(i).text; //TODO create able to get server-cached text
            
            var showDropDownCustomizable = typeof(this.comboBox.showDropDownOnFocus) != "undefined";
            if(showDropDownCustomizable && filter === "")
                isSatisfy = this.comboBox.showDropDownOnFocus !== "Never";
            else {
                if(this.isDropDownListStyle) {
                    isSatisfy = maxCoincide && coincide[i] === maxCoincide;
                } else {
                    isSatisfy = this.IsSatisfy(text, filter);
                }
            }
            ASPx.SetElementDisplay(listTable.rows[i], isSatisfy);
          
            if(firstSatisfyItemIndex == -1 && isSatisfy) {
                //var isTextClearing = !this.isDropDownListStyle && this.filter != text && !this.filter.length;
                var isTextClearing = !this.isDropDownListStyle && this.FilterCompare("") && this.filter != text;
                this.OnFirstSatisfiedItemFound(i, text, isTextClearing);
                firstSatisfyItemIndex = i;
            }
        }
        if(ASPx.Browser.IE && ASPx.Browser.Version > 9) //T197866
            ASPx.SetElementDisplay(listTable, true);
        if(this.isDropDownListStyle) {
            this.SetFilter(filter);
        }
        var visibleCollectionChanged = firstSatisfyItemIndex != -1;
        if(visibleCollectionChanged) {
            lb.CopyCellWidths(0, firstSatisfyItemIndex);
        } else {
            this.UpdateDropDownAfterIneffectualFiltering();
        }
        this.isEnterLocked = false;
        this.OnEndFiltering(visibleCollectionChanged);
    },
    UpdateDropDownAfterIneffectualFiltering: function() {
        if(this.isDropDownListStyle) {
            this.MakeAllItemsClientVisibleCore();
            this.comboBox.UpdateDropDownPositionAndSize();
        } else 
            this.comboBox.HideDropDownArea(true);
    },
    GetFirstVisibleItem: function(lb, listTable) {
        var itemCount = lb.GetItemCount();
        for(var i = 0; i < itemCount; i++)
            if(ASPx.GetElementDisplay(listTable.rows[i]))
                return i;
        return -1;
    },
    GetVisibleItemsCount: function() {
        var visibleItemCount = 0;        
        var lb = this.GetListBoxControl();
        if(ASPx.IsExists(lb)) {
            var listTable = lb.GetListTable();
            var itemCount = lb.GetItemCount();
        
            for(var i = 0; i < itemCount; i++)
                if(ASPx.GetElementDisplay(listTable.rows[i]))
                    visibleItemCount++;
        }
        return visibleItemCount;   
    },
    IsSelectedElementVisible: function(listTable, selectedIndex) {
        return ASPx.GetElementDisplay(listTable.rows[selectedIndex]);
    },
    GetStepForClientFiltrationEnabled: function(lb, step) {
        if(this.comboBox.isCallbackMode) return step;
        
        var listTable = lb.GetListTable();
        var startIndex = this.comboBox.GetSelectedIndex();
        
        var firstVisibleElementIndex = this.GetFirstVisibleItem(lb, listTable);
        if(startIndex > -1) {
            if(!this.IsSelectedElementVisible(listTable, startIndex))
                return firstVisibleElementIndex - startIndex;
        } else return firstVisibleElementIndex + 1;
        
        var stepDirection = step > 0 ? 1 : -1;
        
        var count = lb.GetItemCount();
        
        var needVisibleItemCount = Math.abs(step);
        var outermostVisibleIndex = startIndex;
        for(var index = startIndex + stepDirection; needVisibleItemCount > 0; index += stepDirection){
            if(index < 0 || count <= index) break;
                
            if(ASPx.GetElementDisplay(listTable.rows[index])) {
                outermostVisibleIndex = index;
                needVisibleItemCount--;
            }
        }
        step = outermostVisibleIndex - this.comboBox.GetSelectedIndex();
        return step;
    },
    GetCoincideCharCount: function(text, filter) {
        while(filter != "" && !this.IsSatisfy(text, filter)) {
            filter = filter.slice(0, -1);
        }
        return filter.length;
    },
    OnSelectionChanged: function() {
    },
    IsFilterMeetRequirementForMinLength: function() {
        var inputElement = this.GetInputElement();
        var isFilterExists = inputElement && (inputElement.value || inputElement.value == "");
        return isFilterExists ? inputElement.value.length >= this.comboBox.filterMinLength : true;
    },
    FilterNowAndApply: function() {
        this.callbackProcessingStateManager.SetApply();
        this.Filtering();
        if(!this.comboBox.isCallbackMode)
            this.comboBox.OnApplyChangesInternal();
    },
    // Select contains text
    RemoveHighlightInItems: function() {
        this.ApplySelectionFunctionToItems(ASPx.ContainsFilteringUtils.UnselectContainsTextInElement, true);
    },
    RefreshHighlightInItems: function() {
        if(this.filter != "")
            this.ApplySelectionFunctionToItems(ASPx.ContainsFilteringUtils.ReselectContainsTextInElement, false);
    },
    HighlightTextInItems: function() {
        if(this.filter != "")
            this.ApplySelectionFunctionToItems(ASPx.ContainsFilteringUtils.SelectContainsTextInElement, false);
    },
    ApplySelectionFunctionToItems: function(selectionFunction, applyToAllColumns) {
        var lb = this.GetListBoxControl();
        var count = lb.GetItemCount();
        for(var i = 0; i < count; i ++) {
            var item = lb.GetItemRow(i);
            if(applyToAllColumns || (!applyToAllColumns && ASPx.GetElementDisplay(item))) 
                this.ApplySelectionFunctionToItem(item, selectionFunction, applyToAllColumns);           
        }
    },
    GetFirstTextCellIndex: function () {
        return this.GetListBoxControl().GetItemFirstTextCellIndex();
    },
    ApplySelectionFunctionToItem: function(item, selectionFunction, applyToAllColumns) {
        var itemValues = this.GetItemValuesByItem(item);
        var itemSelection = ASPx.ContainsFilteringUtils.GetColumnSelectionsForItem(itemValues, this.GetListBoxControl().textFormatString, this.filter);
        var firstTextCellIndex = this.GetFirstTextCellIndex();
        if(applyToAllColumns) {
            for(var i = 0; i < item.cells.length; i++)
                selectionFunction(item.cells[i], itemSelection[i]);
        } else {
            for(var i = 0; i < itemSelection.length; i++)
                selectionFunction(item.cells[itemSelection[i].index + firstTextCellIndex], itemSelection[i]);
        }
    },
    GetItemValuesByItem: function(item) {
        var result = [];
        for(var i = this.GetFirstTextCellIndex(); i < item.cells.length; i++)
            result.push(ASPx.GetInnerText(item.cells[i]));
        return result;
    },

    // Abstract methods
    IsSatisfy: function(text, filter) {},
    OnFirstSatisfiedItemFound: function(index, text, isTextClearing) {},   
    HighlightTextAfterCallback: function() {}
});

var ASPxContainsFilteringStrategy = ASPx.CreateClass(ASPxComboBoxIncrementalFilteringStrategy, {
    constructor: function(comboBox) {
        this.constructor.prototype.constructor.call(this, comboBox);
    },
   
    IsSatisfy: function(text, filter) {
        return ASPx.Str.PrepareStringForFilter(text).indexOf(filter) != -1;
    },
    IsCloseByEnterLocked: function() {
        if(this.isDropDownListStyle) {
            if(this.IsFilterTimerActive()) return false;
            if(this.GetVisibleItemsCount() == 1) return false;
            var selectedItem = this.comboBox.GetSelectedItem();
            if(selectedItem)
                if(this.GetInputElement().value == selectedItem.text)
                    return false;
            return true;
        }
        return false;
    },
    OnBeforeCallbackFinally: function() {
        this.RefreshHighlightInItems();
        this.SetListBoxSuggestionSelection();
        ASPxComboBoxIncrementalFilteringStrategy.prototype.OnBeforeCallbackFinally.call(this);      
    },
    OnDropDownButtonClick: function() {
        if(this.GetVisibleItemsCount() == 0 && this.isDropDownListStyle) 
            this.comboBox.OnCancelChanges();
    },
    OnTextChanged: function() {
        if(!this.comboBox.IsFocusEventsLocked())
            if(!this.comboBox.ChangedByEnterKeyPress())
                this.OnFilterRollback();
    },
    OnEndFiltering: function(visibleCollectionChanged) {
        ASPxComboBoxIncrementalFilteringStrategy.prototype.OnEndFiltering.call(this, visibleCollectionChanged);     
        this.HighlightTextInItems();
        this.SetListBoxSuggestionSelection();
	},
    OnBeforeHideDropDownArea: function(){
        if(!this.comboBox.isCallbackMode)
            this.FilteringStopClient();
    },
    OnCallbackInternal: function() {
        if(!this.comboBox.isPerformCallback) 
            this.RefreshHighlightInItems();
        ASPxComboBoxIncrementalFilteringStrategy.prototype.OnCallbackInternal.call(this);    
    },
    TrySelectItemAfterFilter: function() {
        var lb = this.GetListBoxControl();
        var selectedItem = null;
        
        var mustSelectFromList = this.isDropDownListStyle;
        
        if(mustSelectFromList) 
            selectedItem = this.GetVisibleItemsCount() == 1 ? lb.GetItem(0) : null;
        else
            selectedItem = lb.FindItemByText(this.filter);

        if(selectedItem) {
            lb.SelectIndexSilent(selectedItem.index);
            this.comboBox.SetTextInternal(selectedItem.text);
        }
        return !!selectedItem;
    },
    OnFirstSatisfiedItemFound: function() {
    },
    SetListBoxSuggestionSelection: function() {
        var mustSelectFromList = this.isDropDownListStyle;
        var suggestionListVisible = this.comboBox.droppedDown;
        var singleItem = this.GetVisibleItemsCount() == 1;
        
        var canSuggest = mustSelectFromList && suggestionListVisible && singleItem;
        
        if(canSuggest) {
            var lb = this.GetListBoxControl();
            var listTable = lb.GetListTable();
            this.comboBox.SelectIndexSilent(lb, this.GetFirstVisibleItem(lb,listTable));
        }
    }
});

var ASPxStartsWithFilteringStrategy = ASPx.CreateClass(ASPxComboBoxIncrementalFilteringStrategy, {
    constructor: function(comboBox) {
        this.constructor.prototype.constructor.call(this, comboBox);
    },
    
    IsSatisfy: function(text, filter) {
        return ASPx.Str.PrepareStringForFilter(text).indexOf(filter) == 0;
    },
    FilteringHighlightCompletedText: function(filterItemText){
        var input = this.GetInputElement();
        ASPx.StartWithFilteringUtils.HighlightSuggestedText(input, filterItemText, this.comboBox);
    },
    HighlightTextAfterCallback: function(firstItemText) {
        var lb = this.GetListBoxControl();
        this.FilteringHighlightCompletedText(firstItemText);
        if(!this.comboBox.isPerformCallback )
            this.comboBox.SelectIndexSilent(lb, 0);
    },
    OnAfterEnter: function() {
        this.ClearInputSelection();
    },
    OnBeforeHideDropDownArea: function() {
        this.FilteringStop();
    },
    OnFirstSatisfiedItemFound: function(index, text, isTextClearing) {
        var lb = this.GetListBoxControl();
        if(!isTextClearing) 
            this.FilteringHighlightCompletedText(text);
        this.comboBox.SelectIndexSilent(lb, isTextClearing ? -1 : index);
    },
    ClearInputSelection: function() {
        var inputElement = this.comboBox.GetInputElement();
        ASPx.Selection.SetCaretPosition(inputElement);
    },
    TrySelectItemAfterFilter: function() {
        var lb = this.GetListBoxControl();
        var selectedItem = lb.GetItem(0);
        return !!selectedItem;
    }
});

var ASPxClientNativeComboBox = ASPx.CreateClass(ASPxClientComboBoxBase, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        
        this.initSelectedIndex = -1;
        this.raiseValueChangedOnEnter = false;
    },
    InlineInitialize: function () {
        ASPxClientComboBoxBase.prototype.InlineInitialize.call(this);
        var lb = this.GetListBoxControl();
        if(lb != null) lb.SetMainElement(this.GetMainElement());
        
        if(this.initSelectedIndex == -1) // workaround - empty selection can't be passed with the select tag
            this.SelectIndex(this.initSelectedIndex, true);
    },
    Initialize: function(){
        ASPxClientComboBoxBase.prototype.Initialize.call(this);
    },
    FindInputElement: function(){
        return this.GetMainElement();
    },    
    GetDropDownInnerControlName: function(suffix){
        return this.name + suffix;
    },
    PerformCallback: function(arg) {
        this.GetListBoxControl().PerformCallback(arg);
    },
    GetTextInternal: function(){
        var selectedItem = this.GetSelectedItem();
        return (selectedItem != null) ? selectedItem.text : "";
    },
    HasTextDecorators: function() {
        return false;
    },
    SetText: function (text){
        var lb = this.GetListBoxControl();
        var index = this.FindItemIndexByText(lb, text);
        this.SelectIndex(index, false);
        this.SetLastSuccessText((index > -1) ? text : "");
        this.SetLastSuccessValue((index > -1) ? lb.GetValue() : null);
    },
    GetValue: function(){
        var selectedItem = this.GetSelectedItem();
        return (selectedItem != null) ? selectedItem.value : null;
    },
    SetValue: function(value){
        var lb = this.GetListBoxControl();
        if(lb){
            lb.SetValue(value);
            // TODO Extract method with OnSelectChanged (params: value)
            var item = lb.GetSelectedItem();
            var text = item ? item.text : value;
            this.SetLastSuccessText((item != null) ? text : "");
            this.SetLastSuccessValue(item != null) ? item.value : null;
        }
    },
    ForceRefocusEditor: function(){
    },
    OnCallback: function(result) {
        this.GetListBoxControl().OnCallback(result);
        if(this.GetItemCount() > 0)
            this.SetSelectedIndex(0);
    },
    OnTextChanged: function() {
        this.OnChange();
    },
    SetTextInternal: function(text){
    },
    SetTextBase: function(text){
    },
    ChangeEnabledAttributes: function(enabled){
        this.GetMainElement().disabled = !enabled;
    }
});

function setValueOptimizeHelper() {
    var itemCollectionWasChangedSinceLastSetValue = false;

    function canReSetValue() {
        return itemCollectionWasChangedSinceLastSetValue;
    }
    function onSetValue() {
        itemCollectionWasChangedSinceLastSetValue = false;
    }

    return {
        onItemCollectionChanged: function () {
            itemCollectionWasChangedSinceLastSetValue = true;
        },
        setValue: function (comboBox, newValue, methodName) {
            var item = comboBox.FindItemByValue(newValue);
            var reSetValue = comboBox.GetValue() === newValue;
            if (reSetValue && !canReSetValue() && (!item || item.selected)) return;

            comboBox.SetValueInternal(newValue);
            onSetValue();
        }
    }
}

ASPx.CBDDButtonMMove = function(evt){
    return ASPx.GetDropDownCollection().OnDDButtonMouseMove(evt);
}
function aspxCBMouseWheel(evt){
    var srcElement = ASPx.Evt.GetEventSource(evt);
    var focusedCB = ASPx.GetDropDownCollection().GetFocusedDropDown();
    if(focusedCB != null && ASPx.GetIsParent(focusedCB.GetMainElement(), srcElement))
        return focusedCB.OnMouseWheel(evt);
}

window.ASPxClientComboBoxBase = ASPxClientComboBoxBase;
window.ASPxClientComboBox = ASPxClientComboBox;
window.ASPxClientNativeComboBox = ASPxClientNativeComboBox;

})();