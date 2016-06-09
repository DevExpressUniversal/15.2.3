var isAdjusting = false;
function AdjustSize() {
    if(!isAdjusting) {
        isAdjusting = true;
        window.setTimeout(AdjustSizeCore, 100);
    }
}
function AdjustSizeCore() {
    var middleRowContent = document.getElementById("MRC");
    var mainTable = document.getElementById("MT");
    var windowHeight = GetWindowHeight();
    var hasHScroll = document.body.scrollWidth > document.body.offsetWidth;
    var newHeight = windowHeight - mainTable.offsetHeight - (hasHScroll ? 20 : 0);
    if(middleRowContent.style.height != middleRowContent.offsetHeight + newHeight + "px") {
        middleRowContent.style.height = middleRowContent.offsetHeight + newHeight + "px";
    }
    isAdjusting = false;
}