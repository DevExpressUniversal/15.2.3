var __aspxAgent = navigator.userAgent.toLowerCase();
var __aspxOpera = (__aspxAgent.indexOf("opera") > -1);
var __aspxIE = (__aspxAgent.indexOf("msie") > -1 && !__aspxOpera);

window.ASPx = {};

window.ASPx.xr_NavigateUrl = function(url, target) {
    if(__aspxIE)
        xr_NavigateUrlForIE(url, target);
    else
        xr_NavigateUrlForNonIE(url, target);
}

function xr_NavigateUrlForIE(url, target) {
    if(url == null)
        return;
    var a = document.createElement("a");
    a.setAttribute('href', url);
    if(target != null)
        a.setAttribute('target', target);
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

function xr_NavigateUrlForNonIE(url, target) {
    var javascriptPrefix = "javascript:";
    if(url == "")
        return;
    else if(url.indexOf(javascriptPrefix) != -1)
        eval(url.substr(javascriptPrefix.length));
    else {
        if(target != "")
            _aspxNavigateTo(url, target);
        else
            location.href = url;
    }
}

function _aspxNavigateTo(url, target) {
    var lowerCaseTarget = target.toLowerCase();
    if("_top" == lowerCaseTarget)
        top.location.href = url;
    else if("_self" == lowerCaseTarget)
        location.href = url;
    else if("_search" == lowerCaseTarget)
        window.open(url, 'blank');
    else if("_media" == lowerCaseTarget)
        window.open(url, 'blank');
    else if("_parent" == lowerCaseTarget)
        window.parent.location.href = url;
    else if("_blank" == lowerCaseTarget)
        window.open(url, 'blank');
    else {
        var frame = _aspxGetFrame(top.frames, target);
        if(frame != null)
            frame.location.href = url;
        else
            window.open(url, 'blank');
    }
}
function _aspxGetFrame(frames, name) {
    if(frames[name])
        return frames[name];
    for(var i = 0; i < frames.length; i++) {
        try {
            var frame = frames[i];
            if(frame.name == name)
                return frame;

            frame = _aspxGetFrame(frame.frames, name);
            if(frame != null)
                return frame;
        } catch(e) {
        }
    }
    return null;
}