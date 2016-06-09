attachElementEvent(document, 'onorientationchange', adjustControlsInIFrame);

function Init() {
}
function adjustControlsInIFrame() { //T222010
    if (window != window.top) {
        if (window.ASPxClientControl) {
            window.ASPxClientControl.AdjustControls(null, true);
        }
    }
}