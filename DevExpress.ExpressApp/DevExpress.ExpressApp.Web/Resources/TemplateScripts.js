var theme;
var postfix;
var LPcellSize;
var tableCellDefaultDisplay;
var isIE7;
for (var i = 0; i < document.forms.length; i++) {
    var input = document.createElement("input");
    input.type = "text";
    input.style.display = "none";
    document.forms[i].insertBefore(input, document.forms[i].firstChild)
}
function Init(themeName, cookiePostfix) {
    theme = themeName;
    postfix = cookiePostfix;
    isIE7 = ASPx.Browser.IE && ASPx.Browser.Version == 7;
}
function OnMouseEnter(separatorButtonId) {
    var separatorButton = document.getElementById(separatorButtonId);
    separatorButton.className += (' dxsplVSeparatorButtonHover_' + theme);
}
function OnMouseLeave(separatorButtonId) {
    var separatorButton = document.getElementById(separatorButtonId);
    separatorButton.className = 'dxsplVSeparatorButton_' + theme;
}
function UpdateSeparatorsImages(separatorImageId, collapseLeft, targetPanelDisplay) {
    var separatorImage = document.getElementById(separatorImageId);
    if (collapseLeft) {
        separatorImage.className = (targetPanelDisplay == 'none' ? 'dxWeb_splVCollapseForwardButton_' : 'dxWeb_splVCollapseBackwardButton_');
    }
    else {
        separatorImage.className = (targetPanelDisplay == 'none' ? 'dxWeb_splVCollapseBackwardButton_' : 'dxWeb_splVCollapseForwardButton_');
    }
    separatorImage.className += theme;
}
function OnClick(panelId, separatorImageId, collapseLeft) {
    var panell = document.getElementById(panelId);
    panell.style.display = (!panell.style.display || panell.style.display == tableCellDefaultDisplay) ? 'none' : tableCellDefaultDisplay;
    if(!ASPx.Browser.IE) {
        panell.style.width = LPcellSize + 'px';
    }
    UpdateSeparatorsImages(separatorImageId, collapseLeft, panell.style.display);
    ASPx.Cookie.SetCookie(panelId + postfix, panell.style.display);
    AdjustSize();
}
function OnLoadCore(panelId, separatorCellId, separtorImageId, display, collapseLeft) {
    tableCellDefaultDisplay = isIE7 ? "block" : "table-cell";
    var LPcell = document.getElementById(panelId);
    var separatorCell = document.getElementById(separatorCellId);

    LPcell.style.display = display ? tableCellDefaultDisplay : 'none';
    separatorCell.style.display = LPcell.style.display;
    var leftPanelDisplay = ASPx.Cookie.GetCookie(panelId + postfix);
    if (leftPanelDisplay && separatorCell.style.display != 'none') {
        LPcell.style.display = leftPanelDisplay;
    }
    UpdateSeparatorsImages(separtorImageId, collapseLeft, leftPanelDisplay);
    AdjustSize();
    attachWindowEvent('resize', AdjustSize);
}