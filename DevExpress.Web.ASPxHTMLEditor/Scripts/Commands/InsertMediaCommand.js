(function() {
    var MediaObject = ASPx.CreateClass(null, {
        constructor: function(document, settings, elementEvents) {
            this.settings = settings;
            this.document = document;
            this.elementEvents = elementEvents;
        },
        getHtml: function() {
            var element = this.createElement();
            return element.outerHTML;
        },
        createElement: function() {
            var element = this.createElementInternal();
            this.prepareElement(element);
            if(element) {
                if(this.settings.id)
                    element.id = this.settings.id;
                ASPx.HtmlEditorClasses.Utils.AppendStyleSettings(element, this.settings);
                if(this.elementEvents) {
                    for(var e in this.elementEvents)
                        ASPx.Evt.AttachEventToElement(element, e, this.elementEvents[e]);
                }
            }
            return element;
        },
        prepareElement: function(element) {
        },
        createElementInternal: function() {
            var result = this.createDomElement(this.getTagName());
            if(this.settings && this.settings.htmlBeforeChange) {
                var oldElementContainer = this.createDomElement("DIV");
                oldElementContainer.innerHTML = this.settings.htmlBeforeChange;
                var oldElement = oldElementContainer.childNodes[0];
                if(oldElement && oldElement.tagName == result.tagName)
                    result = oldElement;
            }
            return result;
        },
        getTagName: function() {
            return "";    
        },
        createDomElement: function(tagName) {
            var element = this.document.createElement(tagName);
            if(this.elementEvents) {
                for(var p in this.elementEvents)
                   ASPx.Evt.AttachEventToElement(element, p, this.elementEvents[p]);
            }
            return element;
        }
    });
    var HTML5MediaObject = ASPx.CreateClass(MediaObject, {
        prepareElement: function(result) {
            if(!this.settings)
                return;
            ASPx.Attr.SetAttribute(result, "src" , this.getSrc(this.settings.src));
            if(this.settings.sources) {
                for(var i = 0; i < this.settings.sources.length; i++)
                    this.appendSource(result, this.settings.sources[i]);
            }
            ASPx.Attr.SetOrRemoveAttribute(result, "preload", this.settings.preload);
            ASPx.Attr.SetOrRemoveAttribute(result, "controls", this.settings.showControls);
            ASPx.Attr.SetOrRemoveAttribute(result, ASPx.HtmlEditorClasses.preservedAttributeNamePrefix + "autoplay", this.settings.autoPlay);
            ASPx.Attr.SetOrRemoveAttribute(result, "loop", this.settings.repeat);
            if(this.settings.muted)
                ASPx.Attr.SetOrRemoveAttribute(result, "muted", this.settings.muted);
            this.appendCustomSettings(result);
            this.setFallbackMessage(result);
        },
        setFallbackMessage: function(element) {
            if(!this.settings.fallbackMessage)
                return;
            var errorContainer = this.createDomElement("SPAN");
            errorContainer.innerHTML = this.settings.fallbackMessage;
            element.appendChild(errorContainer);
        },
        appendCustomSettings: function(element) {
        },
        getTagName: function() {
            return "";
        },
        getSrc: function(src) {
            var range = this.settings.range;
            if(range)
                src += "#t=" + (range.start || "") + "," + (range.end || "");
            return src;
        },
        getType: function(src) {
            return "";
        },
        appendSource: function(audioElement, originalSrc) {
            var source = this.createDomElement("source");
            ASPx.Attr.SetAttribute(source, "src", this.getSrc(originalSrc));
            ASPx.Attr.SetAttribute(source, "type" , this.getType(originalSrc));
            audioElement.appendChild(source);
        }
    });
    var AudioObject = ASPx.CreateClass(HTML5MediaObject, {
        getTagName: function() {
            return "AUDIO";
        }
    });
    
    var VideoObject = ASPx.CreateClass(HTML5MediaObject, {
        getTagName: function() {
            return "VIDEO";
        },
        appendCustomSettings: function(videoDOMElement) {
            ASPx.Attr.SetOrRemoveAttribute(videoDOMElement, "poster", this.settings.poster);
        }
    });

    var FlashObject = ASPx.CreateClass(MediaObject, {
        getTagName: function() {
            return "OBJECT";    
        },
        prepareElement: function(result) {
            var settings = this.settings;
            if(!settings)
                return;
            if(settings.src)
                ASPx.Attr.SetAttribute(result, "data", this.settings.src);
            if(settings.type)
                ASPx.Attr.SetAttribute(result, "type", this.settings.type);
            var fields = this.getParamFields();
            for(var i = 0; i < fields.length; i++)
                this.addParameter(result, settings, fields[i]);
        },
        getParamFields: function() {
            return ["loop", "allowFullScreen", "quality", "play", "menu"];    
        },
        addParameter: function(element, settings, name) {
            var value = settings[name];
            if(!ASPx.IsExists(value))
                return;
            var param = this.getParameter(element, name);
            ASPx.Attr.SetAttribute(param, "value", value);
            element.appendChild(param);
        },
        getParameter: function(element, name) {
            for(var i = 0; i < element.childNodes.length; i++) {
                if(ASPx.Attr.GetAttribute(element.childNodes[i], "name") == name)
                    return element.childNodes[i];
            }
            var param = this.createDomElement("PARAM");
            ASPx.Attr.SetAttribute(param, "name", name);
            return param;
        }
    });
    var YoutubeObject = ASPx.CreateClass(MediaObject, {
        getTagName: function() {
            return "IFRAME";    
        },
        prepareElement: function(element) {
            if(!this.settings)
                return null;
            this.settings.youtubeVideoId = YoutubeObject.getYoutubeIdFromURL(this.settings.src);
            ASPx.Attr.SetAttribute(element, "src" , YoutubeObject.createURL(this.settings));
        }
    });
    YoutubeObject.getYoutubeIdFromURL = function(url) {
        var matches = (new RegExp(ASPx.HtmlEditorClasses.YouTubeSrcRegExpPattern, "g")).exec(url);
        return matches && matches.length > 0 ? matches[matches.length - 1] : "";
    };
    YoutubeObject.createURL = function(settings) {
        if(!settings)
            return "";
        var url = "https://www.youtube";
        if(settings.highSecureMode)
            url += "-nocookie";
        url += ".com/embed/";
        url += settings.youtubeVideoId;
        var queryParams = [];
        if(!settings.showSameVideos)
            queryParams.push("rel=0");
        if(!settings.showVideoName)
            queryParams.push("showinfo=0");
        if(!settings.showControls)
            queryParams.push("controls=0");
        for(var i = 0; i < queryParams.length; i++)
            url += ((i == 0 ? "?" : "&") + queryParams[i]);
        return url;
    };
    YoutubeObject.createSettings = function(src) {
        var settings = {};
        settings.youtubeVideoId = YoutubeObject.getYoutubeIdFromURL(src);
        if(settings.youtubeVideoId != "") {
            settings.highSecureMode = src.indexOf("youtube-nocookie") > -1;
            settings.showSameVideos = src.indexOf("rel=0") == -1;
            settings.showVideoName = src.indexOf("showinfo=0") == -1;
            settings.showControls = src.indexOf("controls=0") == -1;
            settings.src = "www.youtube.com/watch?v=" + settings.youtubeVideoId;
        }
        return settings;
    };

    var MediaCommandBase = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        canBeExecutedOnSelectedElement: function(selectedElement) {
            return selectedElement && ASPx.Data.ArrayIndexOf(this.getCssClassMarker(), selectedElement.className, function(classMarker, className) { return className.indexOf(classMarker) > -1; }) > -1;
        },
        getCssClassMarker: function() {
            return [""];
        }
    });
    var InsertMediaBase = ASPx.CreateClass(MediaCommandBase, {
        // cmdValue - item properties
        Execute: function(cmdValue, wrapper) {
            var item = this.createItem(cmdValue, wrapper.getDocument());
            if(wrapper.tagIsFiltered(item.getTagName()))
                return false;
            this.insertMedia(item, wrapper);
            return true;
        },
        createItem: function(itemProperties, document) {
            return null;
        },
        insertMedia: function(item, wrapper) {
            var mediaElement;
            var markerID = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
            mediaElement = item.createElement();
            var tempId = mediaElement.id;
            mediaElement.id = markerID;
            this.owner.getCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND).Execute(mediaElement.outerHTML, wrapper);
            var insertedImage = ASPx.GetElementByIdInDocument(wrapper.getDocument(), markerID);
            if(!insertedImage)
                return;
            else {
                insertedImage = ASPx.HtmlEditorClasses.Utils.setHorizontalAlign(insertedImage, item.settings.position, markerID);
                if (ASPx.Browser.NetscapeFamily)
                    insertedImage.offsetHeight;
            }
            mediaElement = ASPx.HtmlEditorClasses.HtmlProcessor.restoreDomElementBySpecialImageElement(wrapper.getDocument(), insertedImage);
            ASPx.Attr.RemoveAttribute(mediaElement, "id");
            ASPx.Attr.RemoveAttribute(insertedImage, "id");
            if(tempId)
                ASPx.Attr.SetAttribute(mediaElement, "id", tempId);
            var floatAttribute = ASPx.GetElementFloat(insertedImage);
            if(floatAttribute && floatAttribute != "none")
                ASPx.SetElementFloat(mediaElement, floatAttribute);
            ASPx.Attr.SetAttribute(insertedImage, ASPx.HtmlEditorClasses.SpecialImageAttributeName, ASPx.HtmlEditorClasses.HtmlProcessor.encodeURIComponent(mediaElement.outerHTML));
            ASPxClientHtmlEditorSelection.SelectElement(insertedImage, wrapper.getWindow(), true);
        }
    });

    ASPx.HtmlEditorClasses.Commands.Browser.InsertAudio = ASPx.CreateClass(InsertMediaBase, {
        createItem: function(settings, document) {
            return new AudioObject(document, settings);
        },
        getCssClassMarker: function() {
            return [ASPx.HtmlEditorClasses.MediaCssClasses.Audio];
        }
    });
    ASPx.HtmlEditorClasses.Commands.Browser.InsertVideo = ASPx.CreateClass(InsertMediaBase, {
        createItem: function(settings, document) {
            return new VideoObject(document, settings);
        },
        getCssClassMarker: function() {
            return [ASPx.HtmlEditorClasses.MediaCssClasses.Video];
        }
    });
    ASPx.HtmlEditorClasses.Commands.Browser.InsertFlash = ASPx.CreateClass(InsertMediaBase, {
        createItem: function(settings, document) {
            return new FlashObject(document, settings);
        },
        getCssClassMarker: function() {
            return [ASPx.HtmlEditorClasses.MediaCssClasses.Flash, ASPx.HtmlEditorClasses.MediaCssClasses.NotSupported];
        }
    });
    ASPx.HtmlEditorClasses.Commands.Browser.InsertYoutubeVideo = ASPx.CreateClass(InsertMediaBase, {
        createItem: function(settings, document) {
            return new YoutubeObject(document, settings);
        },
        getCssClassMarker: function() {
            return [ASPx.HtmlEditorClasses.MediaCssClasses.YouTube];
        }
    });

    ASPx.HtmlEditorClasses.Commands.Browser.ChangeAudio = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.InsertAudio, {
    });
    ASPx.HtmlEditorClasses.Commands.Browser.ChangeVideo = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.InsertVideo, {
    });
    ASPx.HtmlEditorClasses.Commands.Browser.ChangeFlash = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.InsertFlash, {
    });
    ASPx.HtmlEditorClasses.Commands.Browser.ChangeYoutubeVideo = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.InsertYoutubeVideo, {
    });

    ASPx.HtmlEditorClasses.AudioObject = AudioObject;
    ASPx.HtmlEditorClasses.VideoObject = VideoObject;
    ASPx.HtmlEditorClasses.FlashObject = FlashObject;
    ASPx.HtmlEditorClasses.YoutubeObject = YoutubeObject;
})();