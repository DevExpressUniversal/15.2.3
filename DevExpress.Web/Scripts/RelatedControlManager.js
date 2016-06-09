/// <reference path="_references.js"/>

(function() {
var RelatedControlManager = {

    // Master-Related info storage
    storage: { },
    GetRelatedCollection: function(masterName) {
        if(!this.storage[masterName])
            this.storage[masterName] = [ ];
        return this.storage[masterName];
    },
    RegisterRelatedControl: function(masterName, name) {
        this.GetRelatedCollection(masterName)[name] = name;
    },
    RegisterRelatedControls: function(masterName, names) {
        var relatedCollection = this.GetRelatedCollection(masterName);
        var name;
        for(var i = 0; i < names.length; i++) {
            name = names[i];
            relatedCollection[name] = name;
        }
    },
    GetLinkedControls: function(masterControl) {
        var result = [ masterControl ];
        for(var name in this.GetRelatedCollection(masterControl.name)) {
            var control = ASPx.GetControlCollection().Get(name);
            if(control)
                result.push(control);
        }
        return result;
    },
        
    // Shading
    coverCache: { },
    panelCache: { },
    timers: { },
    
    Shade: function(masterControl) {		
		this.ShadeCore(masterControl, true);
        this.timers[masterControl.name] = window.setTimeout(function() { ASPx.RelatedControlManager.ShadeTransition(masterControl.name); }, 750);
    },
    ShadeCore: function(masterControl, isTransparent) {
		if(!isTransparent) {
			this.panelCache[masterControl.name] = masterControl.ShowLoadingPanel();
		}    
        var controls = this.GetLinkedControls(masterControl);
        for(var i = 0; i < controls.length; i++) {
            var control = controls[i];
            var cover = control.CreateLoadingDiv(document.body, control.GetMainElement());
            if(ASPx.IsExistsElement(cover)) {
				if(isTransparent) {
					cover.className = "";
					cover.style.background = "white";									
					if(ASPx.Browser.IE)
					    cover.style.filter = "alpha(opacity=1)";
					else
					    cover.style.opacity = "0.01";
				}
				this.coverCache[control.name] = cover;
			}
        }		
    },
    ShadeTransition: function(masterName) {
		var obj = ASPx.GetControlCollection().Get(masterName);
		if(obj) {
			this.Unshade(obj);
			this.ShadeCore(obj, false);
		}
    },
    Unshade: function(masterControl) {
		var masterName = masterControl.name;
		ASPx.Timer.ClearTimer(this.timers[masterName]);
		delete this.timers[masterName];
		
		var panel = this.panelCache[masterName];
		if(ASPx.IsExistsElement(panel))
			ASPx.RemoveElement(panel);
		delete this.panelCache[masterName];
    
        var controls = this.GetLinkedControls(masterControl);
        for(var i = 0; i < controls.length; i++) {
            var control = controls[i];
            var cover = this.coverCache[control.name];
            if(ASPx.IsExistsElement(cover))
				ASPx.RemoveElement(cover);                
            delete this.coverCache[control.name];
        }
    },
    
    // Response processing
    CreateInfo: function() {
        return { 
            clientObjectName:   "",
            elementId:          "",
            innerHtml:          "",
            parameters:         ""
        };    
    },
    ProcessInfo: function(info) {
        var control = ASPx.GetControlCollection().Get(info.clientObjectName);        
        if(!control || !ASPx.IsFunction(control.ProcessCallbackResult))
            this.ProcessCallbackResultDefault(info.elementId, info.innerHtml, info.parameters);
        else
            control.ProcessCallbackResult(info.elementId, info.innerHtml, info.parameters);
    },
    ProcessCallbackResultDefault: function(elementId, innerHtml, parameters) {
        var element = ASPx.GetElementById(elementId);
        if(ASPx.IsExistsElement(element))
            element.innerHTML = innerHtml;        
    },
    // TODO: refactor
    ParseResult: function(result) {        
        var prevIndex = 0;
        var index;
        var lens;
        var info;
        
        while(true) {
            var remm = result.substring(prevIndex);
            index = result.indexOf("|", prevIndex);
            if(index < 0)
                break;
            lens = result.substring(prevIndex, index).split(",");
            
            prevIndex = index + 1;

            info = this.CreateInfo();
            info.clientObjectName = result.substr(prevIndex, lens[0]);
            prevIndex += parseInt(lens[0]);
            info.elementId = result.substr(prevIndex, lens[1]);
            prevIndex += parseInt(lens[1]);
            info.innerHtml = result.substr(prevIndex, lens[2]);
            prevIndex += parseInt(lens[2]);
            info.parameters = result.substr(prevIndex, lens[3]);
            prevIndex += parseInt(lens[3]);
            this.ProcessInfo(info);
        }
    }
};

ASPx.RelatedControlManager = RelatedControlManager;
})();