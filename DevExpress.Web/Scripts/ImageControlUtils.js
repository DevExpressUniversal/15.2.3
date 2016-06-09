/// <reference path="_references.js"/>

(function() {
    var ASPxClientImageControlBase = ASPx.CreateClass(null, {
        constructor: function(control) {
            this.control = control;
            this.Initialize();
            this.CreateControlHierarchy();
            this.PrepareControlHierarchy();
            if(this.IsEnabled())
                this.InitializeHandlers();
        },
        Initialize: function() {
        },
        CreateControlHierarchy: function() {
        },
        PrepareControlHierarchy: function() {
        },
        AdjustControl: function() {
        },
        InitializeHandlers: function() {
        },
        IsEnabled: function() {
            return this.control.enabled;
        }
    });

    var ImageSizeModeEnum = {
        ActualSizeOrFit: 0,
        FitProportional: 1,
        FitAndCrop: 2
    };
    var RenderMode = {
        Image: 0,
        Canvas: 1
    };

    var Utils = {
        RemoveLoadingGif: function(element) {
            ASPx.SetStyles(element, { backgroundImage: "url(" + ASPx.EmptyImageUrl + ")" });
        },
        IsImageLoaded: function(image) {
            if(ASPx.Browser.IE && image.complete)
                return true;
            if(image.naturalWidth && image.naturalHeight)
                return true;
            return false;
        },

        IsNotEmptyImageSize: function(image) {
            return image.naturalWidth && image.naturalHeight || image.width && image.height ? true : false;
        },
        TryGetImageSize: function(image, onComplete) {
            var fakeImage = new Image();
            ASPx.Evt.AttachEventToElement(fakeImage, "load", function(evt) {
                var img = evt.srcElement || this;
                image.width = fakeImage.width;
                image.height = fakeImage.height;
                image.naturalWidth = fakeImage.width;
                image.naturalHeight = fakeImage.height;

                onComplete();
            });
            fakeImage.src = image.src;
        },
        ChangeImageSource: function(image, src, onComplete) {
            var newImage = document.createElement("IMG");
            newImage.id = image.id;
            newImage.className = image.className;
            newImage.alt = image.alt;
            ASPx.Evt.AttachEventToElement(newImage, "load", function(evt) {
                image.parentNode.appendChild(newImage);
                ASPx.RemoveElement(image);
                onComplete();
            });
            newImage.src = src;
        }
    };

    var ResizeUtils = {
        ResizeImage: function(image, options) {
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9 && !Utils.IsNotEmptyImageSize(image) || !Utils.IsNotEmptyImageSize(image)) //T105909
                Utils.TryGetImageSize(image, function() { ResizeUtils.ResizeImageCore(image, options); });
            else
                ResizeUtils.ResizeImageCore(image, options);
        },
        ResizeImageCore: function(image, options) {
            var parent = image.parentNode;
            var canvas = ResizeUtils.GetCanvas(parent);
            var sizeMode = options.sizeMode == undefined ? ImageSizeModeEnum.ActualSizeOrFit : options.sizeMode;
            var properties = ResizeUtils.GetImageProperties(image.naturalWidth || image.width, image.naturalHeight || image.height, options.width, options.height, sizeMode);
            var useCanvas = properties.renderMode == RenderMode.Canvas && (options.canUseCanvas == undefined ? true : options.canUseCanvas);
            if(useCanvas) {
                if(canvas) {
                    canvas.width = options.width;
                    canvas.height = options.height;
                    ResizeUtils.DrawImage(canvas, image, properties);
                }
                else {
                    canvas = ResizeUtils.CreateCanvas(parent, options.width, options.height, image, properties);
                    ASPx.RemoveElement(image);
                }
            }
            else {
                if(canvas)
                    ASPx.RemoveElement(canvas);
                ResizeUtils.SetImageProperties(image, properties, options.rtl);
            }
            if(options.onEndResize)
                options.onEndResize(canvas || image, useCanvas);
        },
        SetImageProperties: function(image, properties, rtl) {
            var style = {
                marginTop: properties.y, display: ""
            };
            if(rtl)
                style.marginRight = properties.x;
            else
                style.marginLeft = properties.x;

            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9) {
                image.naturalWidth = image.width;
                image.naturalHeight = image.height;
            }
            image.width = properties.w;
            image.height = properties.h;

            ASPx.SetStyles(image, style);
        },
        GetImageProperties: function(naturalWidth, naturalHeight, width, height, sizeMode) {
            var properties = {};
            switch (sizeMode) {
                case ImageSizeModeEnum.FitAndCrop:
                    properties = ResizeUtils.GetFitAndCropImageProperties(naturalWidth, naturalHeight, width, height);
                    break;
                case ImageSizeModeEnum.FitProportional:
                    properties = ResizeUtils.GetFitProportionalImageProperties(naturalWidth, naturalHeight, width, height);
                    break;
                case ImageSizeModeEnum.ActualSizeOrFit:
                    properties = ResizeUtils.GetActualSizeOrFitImageProperties(naturalWidth, naturalHeight, width, height);
                    break;
            }
            properties.renderMode = RenderMode.Image;
            if(window.HTMLCanvasElement && (naturalWidth > width * 2 || naturalHeight > height * 2)) {
                if(ASPx.Browser.MacOSMobilePlatform && (naturalWidth > 2200 || naturalHeight > 2200)) //T160184
                    properties.renderMode = RenderMode.Image;
                else
                    properties.renderMode = RenderMode.Canvas;
            }
            return properties;
        },
        GetFitAndCropImageProperties: function(naturalWidth, naturalHeight, width, height) {
            var ratio = naturalWidth / naturalHeight;

            naturalWidth = width;
            naturalHeight = naturalWidth / ratio;

            if(naturalHeight < height) {
                naturalHeight = height;
                naturalWidth = naturalHeight * ratio;
            }
            var left = -(naturalWidth - width) / 2;
            var top = -(naturalHeight - height) / 2;

            return ResizeUtils.CreateImagePropertiesObject(naturalWidth, naturalHeight, left, top);
        },
        GetFitProportionalImageProperties: function(naturalWidth, naturalHeight, width, height) {
            var ratio = naturalWidth / naturalHeight;

            naturalWidth = width;
            naturalHeight = naturalWidth / ratio;

            if(naturalHeight > height) {
                naturalHeight = height;
                naturalWidth = naturalHeight * ratio;
            }
            var left = (width - naturalWidth) / 2;
            var top = (height - naturalHeight) / 2;

            return ResizeUtils.CreateImagePropertiesObject(naturalWidth, naturalHeight, left, top);
        },
        GetActualSizeOrFitImageProperties: function(naturalWidth, naturalHeight, width, height) {
            if(naturalWidth > width || naturalHeight > height)
                return ResizeUtils.GetFitProportionalImageProperties(naturalWidth, naturalHeight, width, height);
            return ResizeUtils.CreateImagePropertiesObject(naturalWidth, naturalHeight, (width - naturalWidth) / 2, (height - naturalHeight) / 2);
        },
        CreateImagePropertiesObject: function(w, h, x, y) {
            var obj = { w: w, h: h, x: x, y: y };

            obj.w = Math.round(obj.w);
            obj.h = Math.round(obj.h);
            obj.x = Math.round(obj.x);
            obj.y = Math.round(obj.y);

            return obj;
        },

        GetCanvas: function (container) {
            return ASPx.GetChildByTagName(container, "CANVAS");
        },
        CreateCanvas: function(container, width, height, image, properties) {
            var canvas = document.createElement("CANVAS");
            canvas.width = width;
            canvas.height = height;
            container.appendChild(canvas);
            if(image && properties)
                ResizeUtils.DrawImage(canvas, image, properties);
            return canvas;
        },
        DrawImage: function(canvas, image, properties) {
            var context = canvas.getContext("2d");
            context.drawImage(image, properties.x, properties.y, properties.w, properties.h);
        }
    };

    ASPx.ImageControlUtils = {};

    ASPx.ImageControlUtils.ResizeImage = ResizeUtils.ResizeImage;
    ASPx.ImageControlUtils.RemoveLoadingGif = Utils.RemoveLoadingGif;
    ASPx.ImageControlUtils.IsImageLoaded = Utils.IsImageLoaded;
    ASPx.ImageControlUtils.IsNotEmptyImageSize = Utils.IsNotEmptyImageSize;
    ASPx.ImageControlUtils.ChangeImageSource = Utils.ChangeImageSource;
    ASPx.ImageControlUtils.TryGetImageSize = Utils.TryGetImageSize;

    ASPx.ImageControlUtils.ImageSizeModeEnum = ImageSizeModeEnum;

    window.ASPxClientImageControlBase = ASPxClientImageControlBase;
})();