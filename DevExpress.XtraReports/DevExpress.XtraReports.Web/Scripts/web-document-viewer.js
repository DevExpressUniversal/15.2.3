/*! DevExpress HTML/JS Designer - v15.2.2 - 2015-12-01
* http://www.devexpress.com
* Copyright (c) 2015 Developer Express Inc; Licensed Commercial */

var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Report;
        (function (Report) {
            Report.pageBorderColor = { propertyName: "pageBorderColor", modelName: "@PageBorderColor", from: Designer.colorFromString, toJsonObject: Designer.colorToString, displayName: "Page Border Color", editor: DevExpress.JS.Widgets.editorTemplates.color, defaultVal: "Black" };
            Report.pageBorderWidth = { propertyName: "pageBorderWidth", modelName: "@PageBorderWidth", displayName: "Page Border Width", from: Designer.floatFromModel, editor: DevExpress.JS.Widgets.editorTemplates.numeric, defaultVal: 1 };
            Report.pageRange = { propertyName: "pageRange", modelName: "@PageRange", displayName: "Page Range", editor: DevExpress.JS.Widgets.editorTemplates.text, defaultVal: "" };
            Report.expotOptionsTitle = { propertyName: "title", modelName: "@Title", displayName: "Title", editor: DevExpress.JS.Widgets.editorTemplates.text, defaultVal: "Document" };
            Report.tableLayout = { propertyName: "tableLayout", modelName: "@TableLayout", displayName: "Table Layout", editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool, defaultVal: true };
            Report.removeSecondarySymbols = { propertyName: "removeSecondarySymbols", modelName: "@RemoveSecondarySymbols", displayName: "Remove Secondary Symbols", editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool, defaultVal: false };
            Report.characterSet = {
                propertyName: "characterSet",
                modelName: "@CharacterSet",
                displayName: "Character Set",
                editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                defaultVal: "utf-8",
                values: {
                    "utf-8": "utf-8",
                    "windows-1256": "Arabic (Windows)",
                    "iso-8859-4": "Baltic (ISO)",
                    "windows-1257": "Baltic (Windows)",
                    "iso-8859-2": "Central European (ISO)",
                    "windows-1250": "Central European (Windows)",
                    "iso-8859-5": "Cyrillic (ISO)",
                    "koi8-r": "Cyrillic (KOI8-r)",
                    "windows-1251": "Cyrillic (Windows)",
                    "iso-8859-15": "Latin 9 (ISO)",
                    "utf-7": "Unicode (UTF-7)",
                    "iso-8859-1": "Western European (ISO)",
                    "windows-1252": "Western European (Windows)"
                }
            };
            Report.rtfExportMode = {
                propertyName: "rtfExportMode",
                modelName: "@ExportMode",
                defaultVal: "SingleFilePageByPage",
                editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                displayName: "Export Mode",
                values: {
                    "SingleFile": "Single File",
                    "SingleFilePageByPage": "Single File PageByPage",
                }
            };
            Report.htmlExportMode = {
                propertyName: "htmlExportMode",
                modelName: "@ExportMode",
                defaultVal: "SingleFile",
                editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                displayName: "Export Mode",
                values: {
                    "SingleFile": "Single File",
                    "SingleFilePageByPage": "Single File PageByPage",
                    "DifferentFiles": "Different Files"
                }
            };
            Report.embedImagesInHTML = {
                propertyName: "embedImagesInHTML",
                modelName: "@EmbedImagesInHTML",
                defaultVal: false,
                editor: DevExpress.JS.Widgets.editorTemplates.bool,
                from: Designer.parseBool,
                displayName: "Embed Images In HTML"
            };
            Report.imageExportMode = {
                propertyName: "imageExportMode",
                modelName: "@ExportMode",
                defaultVal: "SingleFile",
                editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                displayName: "Export Mode",
                values: {
                    "SingleFile": "Single File",
                    "SingleFilePageByPage": "Single File PageByPage",
                    "DifferentFiles": "Different Files"
                }
            };
            Report.xlsExportMode = {
                propertyName: "xlsExportMode",
                modelName: "@ExportMode",
                defaultVal: "SingleFile",
                editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                displayName: "Export Mode",
                values: {
                    "SingleFile": "Single File",
                    "DifferentFiles": "Different Files"
                }
            };
            Report.xlsxExportMode = {
                propertyName: "xlsxExportMode",
                modelName: "@ExportMode",
                defaultVal: "SingleFile",
                editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                displayName: "Export Mode",
                values: {
                    "SingleFile": "Single File",
                    "SingleFilePageByPage": "Single File PageByPage",
                    "DifferentFiles": "Different Files"
                }
            };
            Report.textExportMode = {
                propertyName: "textExportMode",
                modelName: "@TextExportMode",
                displayName: "Text Export Mode",
                defaultVal: "Text",
                editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                values: {
                    "Text": "Text",
                    "Value": "Value"
                }
            };
            Report.xlsTextExportMode = {
                propertyName: "textExportMode",
                modelName: "@TextExportMode",
                displayName: "Text Export Mode",
                defaultVal: "Value",
                editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                values: {
                    "Text": "Text",
                    "Value": "Value"
                }
            };
            Report.csvTextSeparator = { propertyName: "separator", modelName: "@Separator", displayName: "Separator", editor: DevExpress.JS.Widgets.editorTemplates.text, defaultVal: "," };
            Report.textEncodingType = {
                propertyName: "encodingType",
                modelName: "@EncodingType",
                displayName: "Encoding",
                editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                defaultVal: "Default",
                from: Designer.fromEnum,
                values: {
                    "Default": "Windows-1252",
                    "ASCII": "us-ascii",
                    "Unicode": "utf-16",
                    "BigEndianUnicode": "utf-16BE",
                    "UTF7": "utf-7",
                    "UTF8": "utf-8",
                    "UTF32": "utf-32"
                }
            };
            Report.xlsExportHyperlinks = {
                propertyName: "exportHyperlinks",
                modelName: "@ExportHyperlinks",
                displayName: "Export Hyperlinks",
                defaultVal: true,
                editor: DevExpress.JS.Widgets.editorTemplates.bool,
                from: Designer.parseBool
            };
            Report.xlsRawDataMode = {
                propertyName: "rawDataMode",
                modelName: "@RawDataMode",
                displayName: "Raw Data Mode",
                defaultVal: false,
                editor: DevExpress.JS.Widgets.editorTemplates.bool,
                from: Designer.parseBool
            };
            Report.xlsShowGridLines = {
                propertyName: "showGridLines",
                modelName: "@ShowGridLines",
                displayName: "Show Grid Lines",
                defaultVal: false,
                editor: DevExpress.JS.Widgets.editorTemplates.bool,
                from: Designer.parseBool
            };
            Report.xlsExportOptionsSheetName = {
                propertyName: "sheetName",
                modelName: "@SheetName",
                displayName: "Sheet Name",
                defaultVal: "Sheet",
                editor: DevExpress.JS.Widgets.editorTemplates.text
            };
            function excludeDifferentFilesMode(val) {
                return ko.observable(val === "DifferentFiles" ? "SingleFile" : val);
            }
            Report.excludeDifferentFilesMode = excludeDifferentFilesMode;
            Report.htmlExportModePreview = {
                propertyName: Report.htmlExportMode.propertyName,
                modelName: Report.htmlExportMode.modelName,
                defaultVal: Report.htmlExportMode.defaultVal,
                editor: Report.htmlExportMode.editor,
                displayName: Report.htmlExportMode.displayName,
                from: excludeDifferentFilesMode,
                values: {
                    "SingleFile": "Single File",
                    "SingleFilePageByPage": "Single File PageByPage"
                }
            };
            Report.xlsExportModePreview = {
                propertyName: Report.xlsExportMode.propertyName,
                modelName: Report.xlsExportMode.modelName,
                defaultVal: Report.xlsExportMode.defaultVal,
                editor: Report.xlsExportMode.editor,
                displayName: Report.xlsExportMode.displayName,
                from: excludeDifferentFilesMode,
                values: {
                    "SingleFile": "Single File"
                }
            };
            Report.imageExportModePreview = {
                propertyName: Report.imageExportMode.propertyName,
                modelName: Report.imageExportMode.modelName,
                defaultVal: Report.imageExportMode.defaultVal,
                editor: Report.imageExportMode.editor,
                displayName: Report.imageExportMode.displayName,
                from: excludeDifferentFilesMode,
                values: {
                    "SingleFile": "Single File",
                    "SingleFilePageByPage": "Single File PageByPage"
                }
            };
            Report.xlsxExportModePreview = {
                propertyName: Report.xlsxExportMode.propertyName,
                modelName: Report.xlsxExportMode.modelName,
                defaultVal: Report.xlsxExportMode.defaultVal,
                editor: Report.xlsxExportMode.editor,
                displayName: Report.xlsxExportMode.displayName,
                from: excludeDifferentFilesMode,
                values: {
                    "SingleFile": "Single File",
                    "SingleFilePageByPage": "Single File PageByPage"
                }
            };
        })(Report = Designer.Report || (Designer.Report = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var DevExpress;
(function (DevExpress) {
    var Report;
    (function (Report) {
        var Preview;
        (function (Preview) {
            Preview.AsyncExportApproach = false;
            var ExportOptionsModel = (function () {
                function ExportOptionsModel(reportPreview) {
                    this.actions = [];
                    var exportDisabled = ko.pureComputed(function () {
                        var inProgress = reportPreview.progressBar.inProgress();
                        var documentBuilding = reportPreview.documentBuilding();
                        return reportPreview.pageIndex() === -1 || inProgress || documentBuilding;
                    });
                    this.tabInfo = new DevExpress.Designer.TabInfo(DevExpress.Designer.getLocalization("Export Options"), "dxrd-preview-export-options", reportPreview.exportOptionsModel, "properties", ko.pureComputed(function () { return !!reportPreview.exportOptionsModel(); }));
                    this.actions.push({
                        text: "Export To",
                        disabled: exportDisabled,
                        visible: true,
                        clickAction: function (model) {
                            if (exportDisabled())
                                return;
                            if (model.itemData.format) {
                                reportPreview.exportDocumentTo(model.itemData.format);
                            }
                        },
                        items: ko.pureComputed(function () {
                            var result = [];
                            if (reportPreview.exportOptionsModel()) {
                                reportPreview.exportOptionsModel().pdf && result.push({ text: 'PDF', format: 'pdf' });
                                reportPreview.exportOptionsModel().xls && result.push({ text: 'XLS', format: 'xls' });
                                reportPreview.exportOptionsModel().xlsx && result.push({ text: 'XLSX', format: 'xlsx' });
                                reportPreview.exportOptionsModel().rtf && result.push({ text: 'RTF', format: 'rtf' });
                                reportPreview.exportOptionsModel().mht && result.push({ text: 'MHT', format: 'mht' });
                                reportPreview.exportOptionsModel().html && result.push({ text: 'HTML', format: 'html' });
                                reportPreview.exportOptionsModel().textExportOptions && result.push({ text: 'Text', format: 'txt' });
                                reportPreview.exportOptionsModel().csv && result.push({ text: 'CSV', format: 'csv' });
                                reportPreview.exportOptionsModel().image && result.push({ text: 'Image', format: 'image' });
                            }
                            ;
                            return [{
                                text: "Export To",
                                imageClassName: "dxrd-image-export-to",
                                items: result
                            }];
                        }),
                        templateName: "dxrd-preview-export-to"
                    });
                }
                ExportOptionsModel.prototype.getActions = function (context) {
                    return this.actions;
                };
                return ExportOptionsModel;
            })();
            Preview.ExportOptionsModel = ExportOptionsModel;
        })(Preview = Report.Preview || (Report.Preview = {}));
    })(Report = DevExpress.Report || (DevExpress.Report = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Report;
        (function (Report) {
            var CsvExportOptions = (function () {
                function CsvExportOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                CsvExportOptions.from = function (model, serializer) {
                    return new CsvExportOptions(model || {}, serializer);
                };
                CsvExportOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, csvExportOptionsSerializationInfo, refs);
                };
                CsvExportOptions.prototype.getInfo = function () {
                    return csvExportOptionsSerializationInfo;
                };
                return CsvExportOptions;
            })();
            Report.CsvExportOptions = CsvExportOptions;
            var csvExportOptionsSerializationInfo = [
                Report.textEncodingType,
                Report.textExportMode,
                { propertyName: "quoteStringsWithSeparators", modelName: "@QuoteStringsWithSeparators", displayName: "Quote Strings with Separators", defaultVal: true, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool },
                Report.csvTextSeparator,
                { propertyName: "skipEmptyRows", modelName: "@SkipEmptyRows", displayName: "Skip Empty Rows", defaultVal: true, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool },
                { propertyName: "skipEmptyColumns", modelName: "@SkipEmptyColumns", displayName: "Skip Empty Columns", defaultVal: true, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool }
            ];
            var ImageExportOptions = (function () {
                function ImageExportOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                ImageExportOptions.from = function (model, serializer) {
                    return new ImageExportOptions(model || {}, serializer);
                };
                ImageExportOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, imageExportOptionsSerializationInfo, refs);
                };
                ImageExportOptions.prototype.getInfo = function () {
                    return imageExportOptionsSerializationInfo;
                };
                ImageExportOptions.prototype.isPropertyDisabled = function (name) {
                    return ((name === "pageRange") || (name === "pageBorderWidth")) && this.imageExportMode() === "SingleFile";
                };
                return ImageExportOptions;
            })();
            Report.ImageExportOptions = ImageExportOptions;
            var imageExportOptionsSerializationInfoBase = [
                Report.pageBorderColor,
                Report.pageBorderWidth,
                Report.pageRange,
                { propertyName: "resolution", modelName: "@Resolution", displayName: "Resolution", editor: DevExpress.JS.Widgets.editorTemplates.numeric, defaultVal: 96 },
                {
                    propertyName: "format",
                    modelName: "@Format",
                    displayName: "Format",
                    editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                    defaultVal: "Png",
                    from: Designer.fromEnum,
                    values: {
                        "Bmp": "BMP",
                        "Gif": "GIF",
                        "Jpeg": "JPEG",
                        "Png": "PNG",
                        "Emf": "EMF",
                        "Wmf": "WMF",
                        "Tiff": "TIFF"
                    }
                }
            ];
            var imageExportOptionsSerializationInfo = [Report.imageExportMode].concat(imageExportOptionsSerializationInfoBase);
            var htmlExportOptionsSerializationInfoBase = [
                Report.pageBorderColor,
                Report.pageBorderWidth,
                Report.pageRange,
                Report.expotOptionsTitle,
                Report.tableLayout,
                Report.removeSecondarySymbols
            ];
            var htmlExportOptionsSerializationInfo = [Report.htmlExportMode, Report.embedImagesInHTML].concat(htmlExportOptionsSerializationInfoBase);
            var HtmlExportOptions = (function () {
                function HtmlExportOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                HtmlExportOptions.from = function (model, serializer) {
                    return new HtmlExportOptions(model || {}, serializer);
                };
                HtmlExportOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, htmlExportOptionsSerializationInfo, refs);
                };
                HtmlExportOptions.prototype.getInfo = function () {
                    return htmlExportOptionsSerializationInfo;
                };
                HtmlExportOptions.prototype.isPropertyDisabled = function (name) {
                    return ((name === "pageRange") || (name === "pageBorderWidth")) && this.htmlExportMode() === "SingleFile";
                };
                return HtmlExportOptions;
            })();
            Report.HtmlExportOptions = HtmlExportOptions;
            var mhtExportOptionsSerializationInfoBase = [
                Report.pageBorderColor,
                Report.pageBorderWidth,
                Report.pageRange,
                Report.expotOptionsTitle,
                Report.characterSet,
                Report.tableLayout,
                Report.removeSecondarySymbols
            ];
            var mhtExportOptionsSerializationInfo = [Report.htmlExportMode].concat(mhtExportOptionsSerializationInfoBase);
            var MhtExportOptions = (function () {
                function MhtExportOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                MhtExportOptions.from = function (model, serializer) {
                    return new MhtExportOptions(model || {}, serializer);
                };
                MhtExportOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, mhtExportOptionsSerializationInfo, refs);
                };
                MhtExportOptions.prototype.getInfo = function () {
                    return mhtExportOptionsSerializationInfo;
                };
                MhtExportOptions.prototype.isPropertyDisabled = function (name) {
                    return ((name === "pageRange") || (name === "pageBorderWidth")) && this.htmlExportMode() === "SingleFile";
                };
                return MhtExportOptions;
            })();
            Report.MhtExportOptions = MhtExportOptions;
            var PdfExportDocumentOptions = (function () {
                function PdfExportDocumentOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                PdfExportDocumentOptions.from = function (model, serializer) {
                    return new PdfExportDocumentOptions(model || {}, serializer);
                };
                PdfExportDocumentOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, pdfExportDocumentOptionsSerializationInfo, refs);
                };
                PdfExportDocumentOptions.prototype.getInfo = function () {
                    return pdfExportDocumentOptionsSerializationInfo;
                };
                return PdfExportDocumentOptions;
            })();
            Report.PdfExportDocumentOptions = PdfExportDocumentOptions;
            var pdfExportDocumentOptionsSerializationInfo = [
                { propertyName: "author", modelName: "@Author", displayName: "Author", defaultVal: "", editor: DevExpress.JS.Widgets.editorTemplates.text },
                { propertyName: "application", modelName: "@Application", displayName: "Application", defaultVal: "", editor: DevExpress.JS.Widgets.editorTemplates.text },
                { propertyName: "title", modelName: "@Title", displayName: "Title", defaultVal: "", editor: DevExpress.JS.Widgets.editorTemplates.text },
                { propertyName: "subject", modelName: "@Subject", displayName: "Subject", defaultVal: "", editor: DevExpress.JS.Widgets.editorTemplates.text },
                { propertyName: "keywords", modelName: "@Keywords", displayName: "Keywords", defaultVal: "", editor: DevExpress.JS.Widgets.editorTemplates.text }
            ];
            var PdfPermissionsOptions = (function () {
                function PdfPermissionsOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                PdfPermissionsOptions.from = function (model, serializer) {
                    return new PdfPermissionsOptions(model || {}, serializer);
                };
                PdfPermissionsOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, pdfExportPermissionsOptionsSerializationInfo, refs);
                };
                PdfPermissionsOptions.prototype.getInfo = function () {
                    return pdfExportPermissionsOptionsSerializationInfo;
                };
                return PdfPermissionsOptions;
            })();
            Report.PdfPermissionsOptions = PdfPermissionsOptions;
            var pdfExportPermissionsOptionsSerializationInfo = [
                {
                    propertyName: "printingPermissions",
                    modelName: "@PrintingPermissions",
                    displayName: "PrintingPermissions",
                    defaultVal: "None",
                    editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                    values: {
                        "None": "None",
                        "LowResolution": "LowResolution",
                        "HighResolution": "HighResolution"
                    }
                },
                {
                    propertyName: "changingPermissions",
                    modelName: "@ChangingPermissions",
                    displayName: "ChangingPermissions",
                    defaultVal: "None",
                    editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                    values: {
                        "None": "None",
                        "InsertingDeletingRotating": "InsertingDeletingRotating",
                        "FillingSigning": "FillingSigning",
                        "CommentingFillingSigning": "CommentingFillingSigning",
                        "AnyExceptExtractingPages": "AnyExceptExtractingPages"
                    }
                },
                { propertyName: "enableCopying", modelName: "@EnableCopying", displayName: "EnableCopying", defaultVal: false, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool },
                { propertyName: "enableScreenReaders", modelName: "@EnableScreenReaders", displayName: "EnableScreenReaders", defaultVal: true, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool }
            ];
            var PdfPasswordSecurityOptions = (function () {
                function PdfPasswordSecurityOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                PdfPasswordSecurityOptions.from = function (model, serializer) {
                    return new PdfPasswordSecurityOptions(model || {}, serializer);
                };
                PdfPasswordSecurityOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, pdfExportPasswordSecurityOptionsSerializationInfo, refs);
                };
                PdfPasswordSecurityOptions.prototype.getInfo = function () {
                    return pdfExportPasswordSecurityOptionsSerializationInfo;
                };
                return PdfPasswordSecurityOptions;
            })();
            Report.PdfPasswordSecurityOptions = PdfPasswordSecurityOptions;
            var pdfExportPasswordSecurityOptionsSerializationInfo = [
                { propertyName: "openPassword", modelName: "@OpenPassword", displayName: "OpenPassword", defaultVal: "", editor: DevExpress.JS.Widgets.editorTemplates.text },
                { propertyName: "permissionsPassword", modelName: "@PermissionsPassword", displayName: "PermissionsPassword", defaultVal: "", editor: DevExpress.JS.Widgets.editorTemplates.text },
                { propertyName: "permissionsOptions", modelName: "PermissionsOptions", displayName: "Pdf Permissions Options", from: PdfPermissionsOptions.from, toJsonObject: PdfPermissionsOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor }
            ];
            var PdfExportOptions = (function () {
                function PdfExportOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                PdfExportOptions.from = function (model, serializer) {
                    return new PdfExportOptions(model || {}, serializer);
                };
                PdfExportOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, pdfExportOptionsSerializationInfo, refs);
                };
                PdfExportOptions.prototype.getInfo = function () {
                    return pdfExportOptionsSerializationInfo;
                };
                PdfExportOptions.prototype.isPropertyDisabled = function (name) {
                    return false;
                };
                return PdfExportOptions;
            })();
            Report.PdfExportOptions = PdfExportOptions;
            var pdfExportOptionsSerializationInfo = [
                { propertyName: "convertImagesToJpeg", modelName: "@ConvertImagesToJpeg", displayName: "Convert Images to Jpeg", defaultVal: true, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool },
                { propertyName: "showPrintDialogOnOpen", modelName: "@ShowPrintDialogOnOpen", displayName: "Show Print Dialog on Open", defaultVal: false, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool },
                { propertyName: "compressed", modelName: "@Compressed", displayName: "Compressed", defaultVal: true, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool },
                { propertyName: "neverEmbeddedFonts", modelName: "@NeverEmbeddedFonts", displayName: "Never Embedded Fonts", defaultVal: "", editor: DevExpress.JS.Widgets.editorTemplates.text },
                {
                    propertyName: "imageQuality",
                    modelName: "@ImageQuality",
                    displayName: "Image Quality",
                    editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                    defaultVal: "Highest",
                    from: Designer.fromEnum,
                    values: {
                        "Lowest": "Lowest",
                        "Low": "Low",
                        "Medium": "Medium",
                        "Hight": "Hight",
                        "Highest": "Highest"
                    }
                },
                {
                    propertyName: "pdfACompatibility",
                    modelName: "@PdfACompatibility",
                    displayName: "PDF A Compatibility",
                    editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                    defaultVal: "None",
                    from: Designer.fromEnum,
                    values: {
                        "Hight": "None",
                        "PdfA2b": "PdfA2b"
                    }
                },
                Report.pageRange,
                { propertyName: "documentOptions", modelName: "DocumentOptions", displayName: "Document Options", from: PdfExportDocumentOptions.from, toJsonObject: PdfExportDocumentOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "pdfPasswordSecurityOptions", modelName: "PdfPasswordSecurityOptions", displayName: "Pdf Password Security Options", from: PdfPasswordSecurityOptions.from, toJsonObject: PdfPasswordSecurityOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor }
            ];
            var RtfExportOptions = (function () {
                function RtfExportOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                RtfExportOptions.from = function (model, serializer) {
                    return new RtfExportOptions(model || {}, serializer);
                };
                RtfExportOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, rtfExportOptionsSerializationInfo, refs);
                };
                RtfExportOptions.prototype.getInfo = function () {
                    return rtfExportOptionsSerializationInfo;
                };
                RtfExportOptions.prototype.isPropertyDisabled = function (name) {
                    return ((name === "pageRange") || (name === "exportWatermarks")) && this.rtfExportMode() === "SingleFile";
                };
                return RtfExportOptions;
            })();
            Report.RtfExportOptions = RtfExportOptions;
            var rtfExportOptionsSerializationInfo = [
                Report.rtfExportMode,
                Report.pageRange,
                { propertyName: "exportWatermarks", modelName: "@ExportWatermarks", displayName: "Export Watermarks", defaultVal: true, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool }
            ];
            var TextExportOptions = (function () {
                function TextExportOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                TextExportOptions.from = function (model, serializer) {
                    return new TextExportOptions(model || {}, serializer);
                };
                TextExportOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, textExportOptionsSerializationInfo, refs);
                };
                TextExportOptions.prototype.getInfo = function () {
                    return textExportOptionsSerializationInfo;
                };
                return TextExportOptions;
            })();
            Report.TextExportOptions = TextExportOptions;
            var textExportOptionsSerializationInfo = [
                Report.textEncodingType,
                { propertyName: "quoteStringsWithSeparators", modelName: "@QuoteStringsWithSeparators", displayName: "Quote Strings with Separators", defaultVal: false, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool },
                { propertyName: "separator", modelName: "@Separator", displayName: "Separator", defaultVal: "TAB", editor: DevExpress.JS.Widgets.editorTemplates.text },
                Report.textExportMode
            ];
            var xlsExportOptionsSerializationInfoBase = [
                Report.xlsExportHyperlinks,
                Report.pageRange,
                Report.xlsRawDataMode,
                Report.xlsExportOptionsSheetName,
                Report.xlsShowGridLines,
                { propertyName: "suppress256ColumnsWarning", modelName: "@Suppress256ColumnsWarning", displayName: "Suppress 256 Columns Warning", defaultVal: false, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool },
                { propertyName: "suppress65536RowsWarning", modelName: "@Suppress65536RowsWarning", displayName: "Suppress 65536 Rows Warning", defaultVal: false, editor: DevExpress.JS.Widgets.editorTemplates.bool, from: Designer.parseBool },
                Report.xlsTextExportMode,
                {
                    propertyName: "workbookColorPaletteCompliance",
                    modelName: "@WorkbookColorPaletteCompliance",
                    displayName: "Workbook Color Palette Compliance",
                    editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                    defaultVal: "ReducePaletteForExactColors",
                    from: Designer.fromEnum,
                    values: {
                        "ReducePaletteForExactColors": "ReducePaletteForExactColors",
                        "AdjustColorsToDefaultPalette": "AdjustColorsToDefaultPalette"
                    }
                }
            ];
            var xlsExportOptionsSerializationInfo = [Report.xlsExportMode].concat(xlsExportOptionsSerializationInfoBase);
            var XlsExportOptions = (function () {
                function XlsExportOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                XlsExportOptions.from = function (model, serializer) {
                    return new XlsExportOptions(model || {}, serializer);
                };
                XlsExportOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, xlsExportOptionsSerializationInfo, refs);
                };
                XlsExportOptions.prototype.getInfo = function () {
                    return xlsExportOptionsSerializationInfo;
                };
                XlsExportOptions.prototype.isPropertyDisabled = function (name) {
                    return name === "pageRange" && this.xlsExportMode() === "SingleFile";
                };
                return XlsExportOptions;
            })();
            Report.XlsExportOptions = XlsExportOptions;
            var xlsxExportOptionsSerializationInfoBase = [
                Report.xlsExportHyperlinks,
                Report.pageRange,
                Report.xlsRawDataMode,
                Report.xlsExportOptionsSheetName,
                Report.xlsShowGridLines,
                Report.xlsTextExportMode
            ];
            var xlsxExportOptionsSerializationInfo = [Report.xlsxExportMode].concat(xlsxExportOptionsSerializationInfoBase);
            var XlsxExportOptions = (function () {
                function XlsxExportOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                XlsxExportOptions.from = function (model, serializer) {
                    return new XlsxExportOptions(model || {}, serializer);
                };
                XlsxExportOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, xlsxExportOptionsSerializationInfo, refs);
                };
                XlsxExportOptions.prototype.getInfo = function () {
                    return xlsxExportOptionsSerializationInfo;
                };
                XlsxExportOptions.prototype.isPropertyDisabled = function (name) {
                    return name === "pageRange" && this.xlsxExportMode() === "SingleFile";
                };
                return XlsxExportOptions;
            })();
            Report.XlsxExportOptions = XlsxExportOptions;
            var ExportOptions = (function () {
                function ExportOptions(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                ExportOptions.from = function (model, serializer) {
                    return new ExportOptions(model || {}, serializer);
                };
                ExportOptions.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, exportOptionsSerializationInfo, refs);
                };
                ExportOptions.prototype.getInfo = function () {
                    return exportOptionsSerializationInfo;
                };
                return ExportOptions;
            })();
            Report.ExportOptions = ExportOptions;
            var exportOptionsSerializationInfo = [
                { propertyName: "csv", modelName: "Csv", displayName: "CSV Export Options", from: CsvExportOptions.from, toJsonObject: CsvExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "html", modelName: "Html", displayName: "HTML Export Options", from: HtmlExportOptions.from, toJsonObject: HtmlExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "image", modelName: "Image", displayName: "Image Export Options", from: ImageExportOptions.from, toJsonObject: ImageExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "mht", modelName: "Mht", displayName: "MHT Export Options", from: MhtExportOptions.from, toJsonObject: MhtExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "pdf", modelName: "Pdf", displayName: "PDF Export Options", from: PdfExportOptions.from, toJsonObject: PdfExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "rtf", modelName: "Rtf", displayName: "RTF Export Options", from: RtfExportOptions.from, toJsonObject: RtfExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "textExportOptions", modelName: "Text", displayName: "Text Export Options", from: TextExportOptions.from, toJsonObject: TextExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "xls", modelName: "Xls", displayName: "XLS Export Options", from: XlsExportOptions.from, toJsonObject: XlsExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "xlsx", modelName: "Xlsx", displayName: "XLSx Export Options", from: XlsxExportOptions.from, toJsonObject: XlsxExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor }
            ];
            var htmlExportOptionsSerializationInfoPreview = [Report.htmlExportModePreview].concat(htmlExportOptionsSerializationInfoBase);
            var HtmlExportOptionsPreview = (function (_super) {
                __extends(HtmlExportOptionsPreview, _super);
                function HtmlExportOptionsPreview() {
                    _super.apply(this, arguments);
                }
                HtmlExportOptionsPreview.from = function (model, serializer) {
                    return new HtmlExportOptionsPreview(model || {}, serializer);
                };
                HtmlExportOptionsPreview.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, htmlExportOptionsSerializationInfoPreview, refs);
                };
                HtmlExportOptionsPreview.prototype.getInfo = function () {
                    return htmlExportOptionsSerializationInfoPreview;
                };
                return HtmlExportOptionsPreview;
            })(HtmlExportOptions);
            Report.HtmlExportOptionsPreview = HtmlExportOptionsPreview;
            var imageExportOptionsSerializationInfoPreview = [Report.imageExportModePreview].concat(imageExportOptionsSerializationInfoBase);
            var ImageExportOptionsPreview = (function (_super) {
                __extends(ImageExportOptionsPreview, _super);
                function ImageExportOptionsPreview() {
                    _super.apply(this, arguments);
                }
                ImageExportOptionsPreview.from = function (model, serializer) {
                    return new ImageExportOptionsPreview(model || {}, serializer);
                };
                ImageExportOptionsPreview.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, imageExportOptionsSerializationInfoPreview, refs);
                };
                ImageExportOptionsPreview.prototype.getInfo = function () {
                    return imageExportOptionsSerializationInfoPreview;
                };
                return ImageExportOptionsPreview;
            })(ImageExportOptions);
            Report.ImageExportOptionsPreview = ImageExportOptionsPreview;
            var mhtExportOptionsSerializationInfoPreview = [Report.htmlExportModePreview].concat(mhtExportOptionsSerializationInfoBase);
            var MhtExportOptionsPreview = (function (_super) {
                __extends(MhtExportOptionsPreview, _super);
                function MhtExportOptionsPreview() {
                    _super.apply(this, arguments);
                }
                MhtExportOptionsPreview.from = function (model, serializer) {
                    return new MhtExportOptionsPreview(model || {}, serializer);
                };
                MhtExportOptionsPreview.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, mhtExportOptionsSerializationInfoPreview, refs);
                };
                MhtExportOptionsPreview.prototype.getInfo = function () {
                    return mhtExportOptionsSerializationInfoPreview;
                };
                return MhtExportOptionsPreview;
            })(MhtExportOptions);
            Report.MhtExportOptionsPreview = MhtExportOptionsPreview;
            var xlsExportOptionsSerializationInfoPreview = [Report.xlsExportModePreview].concat(xlsExportOptionsSerializationInfoBase);
            var XlsExportOptionsPreview = (function (_super) {
                __extends(XlsExportOptionsPreview, _super);
                function XlsExportOptionsPreview() {
                    _super.apply(this, arguments);
                }
                XlsExportOptionsPreview.from = function (model, serializer) {
                    return new XlsExportOptionsPreview(model || {}, serializer);
                };
                XlsExportOptionsPreview.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, xlsExportOptionsSerializationInfoPreview, refs);
                };
                XlsExportOptionsPreview.prototype.getInfo = function () {
                    return xlsExportOptionsSerializationInfoPreview;
                };
                return XlsExportOptionsPreview;
            })(XlsExportOptions);
            Report.XlsExportOptionsPreview = XlsExportOptionsPreview;
            var xlsxExportOptionsSerializationInfoPreview = [Report.xlsxExportModePreview].concat(xlsxExportOptionsSerializationInfoBase);
            var XlsxExportOptionsPreview = (function (_super) {
                __extends(XlsxExportOptionsPreview, _super);
                function XlsxExportOptionsPreview() {
                    _super.apply(this, arguments);
                }
                XlsxExportOptionsPreview.from = function (model, serializer) {
                    return new XlsxExportOptionsPreview(model || {}, serializer);
                };
                XlsxExportOptionsPreview.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, xlsxExportOptionsSerializationInfoPreview, refs);
                };
                XlsxExportOptionsPreview.prototype.getInfo = function () {
                    return xlsxExportOptionsSerializationInfoPreview;
                };
                return XlsxExportOptionsPreview;
            })(XlsxExportOptions);
            Report.XlsxExportOptionsPreview = XlsxExportOptionsPreview;
            var ExportOptionsPreview = (function (_super) {
                __extends(ExportOptionsPreview, _super);
                function ExportOptionsPreview() {
                    _super.apply(this, arguments);
                }
                ExportOptionsPreview.from = function (model, serializer) {
                    return new ExportOptionsPreview(model || {}, serializer);
                };
                ExportOptionsPreview.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, exportOptionsSerializationInfoPreview, refs);
                };
                ExportOptionsPreview.prototype.getInfo = function () {
                    return exportOptionsSerializationInfoPreview;
                };
                return ExportOptionsPreview;
            })(ExportOptions);
            Report.ExportOptionsPreview = ExportOptionsPreview;
            var exportOptionsSerializationInfoPreview = [
                { propertyName: "csv", modelName: "Csv", displayName: "CSV Export Options", from: CsvExportOptions.from, toJsonObject: CsvExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "html", modelName: "Html", displayName: "HTML Export Options", from: HtmlExportOptionsPreview.from, toJsonObject: HtmlExportOptionsPreview.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "image", modelName: "Image", displayName: "Image Export Options", from: ImageExportOptionsPreview.from, toJsonObject: ImageExportOptionsPreview.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "mht", modelName: "Mht", displayName: "MHT Export Options", from: MhtExportOptionsPreview.from, toJsonObject: MhtExportOptionsPreview.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "pdf", modelName: "Pdf", displayName: "PDF Export Options", from: PdfExportOptions.from, toJsonObject: PdfExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "rtf", modelName: "Rtf", displayName: "RTF Export Options", from: RtfExportOptions.from, toJsonObject: RtfExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "textExportOptions", modelName: "Text", displayName: "Text Export Options", from: TextExportOptions.from, toJsonObject: TextExportOptions.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "xls", modelName: "Xls", displayName: "XLS Export Options", from: XlsExportOptionsPreview.from, toJsonObject: XlsExportOptionsPreview.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                { propertyName: "xlsx", modelName: "Xlsx", displayName: "XLSx Export Options", from: XlsxExportOptionsPreview.from, toJsonObject: XlsxExportOptionsPreview.toJson, editor: DevExpress.JS.Widgets.editorTemplates.objecteditor }
            ];
        })(Report = Designer.Report || (Designer.Report = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Report;
    (function (Report) {
        var Preview;
        (function (Preview) {
            function convetToPercent(childSize, parentSize) {
                return childSize * 100 / parentSize + '%';
            }
            function initializeBrick(brick, processClick) {
                if (!brick) {
                    return;
                }
                !!brick.active && !!brick.active(false) || (brick.active = ko.observable(false));
                brick["onClick"] = function () {
                    processClick && processClick(brick);
                };
                brick.bricks && brick.bricks.forEach(function (childBrick) {
                    childBrick.leftP = convetToPercent(childBrick.left, brick.width);
                    childBrick.widthP = convetToPercent(childBrick.width, brick.width);
                    childBrick.topP = convetToPercent(childBrick.top, brick.height);
                    childBrick.heightP = convetToPercent(childBrick.height, brick.height);
                    initializeBrick(childBrick, processClick);
                });
            }
            Preview.initializeBrick = initializeBrick;
            var PreviewPageBrickProvider = (function () {
                function PreviewPageBrickProvider(handlerUri, documentId) {
                    this.getBricks = function (pageIndex) {
                        var deffered = $.Deferred();
                        if (!handlerUri || !documentId) {
                            deffered.resolve(null);
                        }
                        else {
                            $.post(handlerUri, {
                                actionKey: 'getBrickMap',
                                arg: encodeURIComponent(JSON.stringify({ pageIndex: pageIndex, documentId: documentId }))
                            }).done(function (response) {
                                deffered.resolve(response.result);
                            }).fail(function (error) {
                                deffered.reject(error);
                            });
                        }
                        return deffered.promise();
                    };
                }
                return PreviewPageBrickProvider;
            })();
            Preview.PreviewPageBrickProvider = PreviewPageBrickProvider;
            var PreviewPage = (function (_super) {
                __extends(PreviewPage, _super);
                function PreviewPage(pageIndex, width, height, zoom, documentId, brickProvider, loading, processClick) {
                    var _this = this;
                    _super.call(this);
                    this.originalHeight = ko.observable(0);
                    this.originalWidth = ko.observable(0);
                    this.loadingText = "Loading...";
                    this.realZoom = ko.observable(1);
                    this.pageLoadFailed = ko.observable(false);
                    this.pageLoaded = ko.observable(false);
                    this.imageSrc = ko.observable("");
                    this.cachedImageSrc = ko.observable("");
                    this.brick = ko.observable(null);
                    this.active = ko.observable(false);
                    this._selectedBrickPath = null;
                    this.pageIndex = pageIndex;
                    this.brickProvider = brickProvider;
                    this.pageLoading = loading || ko.observable(true);
                    this.originalHeight(ko.unwrap(height));
                    this.originalWidth(ko.unwrap(width));
                    this.zoom = zoom;
                    this.width = ko.pureComputed(function () {
                        return Math.ceil(_this.originalWidth() * _this.realZoom());
                    });
                    this.height = ko.pureComputed(function () {
                        return Math.ceil(_this.originalHeight() * _this.realZoom());
                    });
                    this._disposables.push(this.zoom.subscribe(function (newVal) {
                        _this._setPageImgSrc(documentId(), newVal);
                    }));
                    var _self = this;
                    this.isEmpty = pageIndex === -1 && !brickProvider && !processClick;
                    this.onPageLoaded = function (s, e) {
                        _self.pageLoading(false);
                        _self.pageLoaded(true);
                        if (_self.cachedImageSrc() !== _self.imageSrc()) {
                            _self.originalHeight() && ko.isObservable(height) && height(_self.originalHeight());
                            _self.originalWidth() && ko.isObservable(width) && width(_self.originalWidth());
                            if (!_self.brick()) {
                                _self.brickProvider && _self.brickProvider.getBricks(_self.pageIndex).done(function (result) {
                                    if (!result || !result.brick) {
                                        return;
                                    }
                                    _self.brickColumnWidthArray = result.columnWidthArray;
                                    _self.originalWidth(result.brick.width);
                                    _self.originalHeight(result.brick.height);
                                    initializeBrick(result.brick, processClick);
                                    result.brick.leftP = convetToPercent(result.brick.left, _this.originalWidth());
                                    result.brick.widthP = convetToPercent(result.brick.width, _this.originalWidth());
                                    result.brick.topP = convetToPercent(result.brick.top, _this.originalHeight());
                                    result.brick.heightP = convetToPercent(result.brick.height, _this.originalHeight());
                                    _self.brick(result.brick);
                                    _self._selectedBrickPath && _self.selectBrick(_self._selectedBrickPath);
                                });
                            }
                            _self.cachedImageSrc(_self.imageSrc());
                        }
                    };
                    this.onPageLoadFailed = function (s, e) {
                        if (_self.pageIndex !== -1) {
                            _self.pageLoadFailed(true);
                            _self.pageLoading(false);
                        }
                    };
                    this.resetBrickRecusive = function (brick) {
                        if (brick && brick.active) {
                            brick.active(false);
                            if (brick.bricks) {
                                brick.bricks.forEach(function (childBrick) {
                                    _self.resetBrickRecusive(childBrick);
                                });
                            }
                        }
                    };
                    this.selectBrick = function (path, ctrlKey) {
                        processClick && processClick(null);
                        var currentBrick = _self.brick();
                        !ctrlKey && _self.resetBrickRecusive(currentBrick);
                        if (!path) {
                            return;
                        }
                        if (!currentBrick) {
                            _self._selectedBrickPath = path;
                            return;
                        }
                        var pathElements = path.split(",");
                        pathElements.forEach(function (el) {
                            currentBrick = currentBrick.bricks[parseInt(el)];
                        });
                        currentBrick.active(true);
                    };
                    this._disposables.push(ko.computed(function () {
                        if (!_this.active()) {
                            _this.resetBrickRecusive(_this.brick());
                            _this._selectedBrickPath = null;
                        }
                    }));
                }
                PreviewPage.prototype._setPageImgSrc = function (documentId, zoom) {
                    this.pageLoadFailed(false);
                    if (!documentId || this.pageIndex === -1) {
                        this.imageSrc(null);
                        return;
                    }
                    var magicNumber = 96;
                    var newResolution = Math.round((zoom ? zoom : 1) * magicNumber);
                    this.realZoom(newResolution / magicNumber);
                    this.imageSrc(Preview.HandlerUri + "?actionKey=getPage&unifier=" + Preview.generateGuid() + "&arg=" + encodeURIComponent(JSON.stringify({ pageIndex: this.pageIndex, documentId: documentId, resolution: newResolution })));
                };
                return PreviewPage;
            })(DevExpress.Designer.Disposable);
            Preview.PreviewPage = PreviewPage;
        })(Preview = Report.Preview || (Report.Preview = {}));
    })(Report = DevExpress.Report || (DevExpress.Report = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Report;
    (function (Report) {
        var Preview;
        (function (Preview) {
            Preview.formatSearchResult = function (value) {
                return value && ('page ' + (value.pageIndex + 1));
            };
            Preview.searchAvailable = ko.observable(true);
            var SearchViewModel = (function (_super) {
                __extends(SearchViewModel, _super);
                function SearchViewModel(reportPreview) {
                    var _this = this;
                    _super.call(this);
                    this._cachedRequests = {};
                    this._cachedWholeWordRequests = {};
                    this._cachedCaseSensitiveRequests = {};
                    this._cachedWholeWordWithCaseRequests = {};
                    this.actions = [];
                    this.focusRequested = ko.observable(true);
                    this.matchWholeWord = ko.observable(false);
                    this.matchCase = ko.observable(false);
                    this.searchUp = ko.observable(false);
                    this.searchText = ko.observable("");
                    this.searchResult = ko.observable([]);
                    this.loading = ko.observable(false);
                    this._resultNavigator = SearchViewModel.createResultNavigator(this, reportPreview);
                    this.clean = function () {
                        _this.searchText("");
                    };
                    var lastMatchCase = this.matchCase();
                    var lastMatchWholeWord = this.matchWholeWord();
                    this.findUp = function () {
                        _this.searchUp(true);
                        _this.findNext();
                    };
                    this.findDown = function () {
                        _this.searchUp(false);
                        _this.findNext();
                    };
                    this.goToResult = function (result) {
                        _this._resultNavigator.goToResult(result.id);
                    };
                    var newSearch = function (text) {
                        _this.searchResult([]);
                        _this._resultNavigator.currentResult(null);
                        lastMatchCase = _this.matchCase();
                        lastMatchWholeWord = _this.matchWholeWord();
                        if (!text) {
                            reportPreview.pages() && reportPreview.pages()[reportPreview.pageIndex()] && reportPreview.pages()[reportPreview.pageIndex()].selectBrick("");
                            return;
                        }
                        var mCase = _this.matchCase();
                        var text = mCase ? _this.searchText() : _this.searchText().toLocaleLowerCase();
                        var cache = _this.matchWholeWord() ? mCase ? _this._cachedWholeWordWithCaseRequests : _this._cachedWholeWordRequests : mCase ? _this._cachedCaseSensitiveRequests : _this._cachedRequests;
                        if (cache[text]) {
                            _this.searchResult(cache[text]);
                            return;
                        }
                        _this.loading(true);
                        var self = _this;
                        reportPreview.previewWrapper.findTextRequest(text).done(function (response) {
                            self.findTextRequestDone(response.result, cache[text]);
                        }).fail(function (error) {
                            self.loading(false);
                            reportPreview._processError(DevExpress.Designer.getLocalization("An error occurred during search"));
                        });
                    };
                    this.findNext = function () {
                        if (_this.loading()) {
                            return;
                        }
                        (lastMatchCase === _this.matchCase() || _this.searchText().toUpperCase() === _this.searchText().toLowerCase()) && lastMatchWholeWord === _this.matchWholeWord() && _this._resultNavigator.next(_this.searchUp()) || newSearch(_this.searchText());
                    };
                    this._disposables.push(this.searchText.subscribe(function (newVal) {
                        newSearch(newVal);
                    }));
                    var disabled = ko.pureComputed(function () {
                        var documentId = reportPreview["_currentDocumentId"]();
                        return reportPreview.documentBuilding() || !documentId;
                    });
                    this.actions.push({
                        text: "Search",
                        imageClassName: "dxrd-image-search",
                        disabled: disabled,
                        visible: ko.pureComputed(function () { return Preview.searchAvailable(); }),
                        hasSeparator: true,
                        clickAction: function () {
                            if (!_this.tabInfo.active()) {
                                _this.tabInfo.active(true);
                            }
                            else {
                                _this.tabInfo.active.notifySubscribers();
                            }
                        }
                    });
                    this.tabInfo = new DevExpress.Designer.TabInfo(DevExpress.Designer.getLocalization("Search"), "dxrd-preview-search", this, "search", ko.pureComputed(function () { return !disabled() && Preview.searchAvailable(); }));
                    this._disposables.push(this.tabInfo.active.subscribe(function (newVal) {
                        newVal && setTimeout(function () { return _this.focusRequested.notifySubscribers(); }, 100);
                    }));
                }
                SearchViewModel.prototype.findTextRequestDone = function (result, cache) {
                    this.loading(false);
                    if (!result) {
                        this.searchResult([]);
                        return;
                    }
                    cache = (result.success ? result.matches : []) || [];
                    this.searchResult(cache);
                };
                SearchViewModel.prototype.getActions = function (context) {
                    return this.actions;
                };
                Object.defineProperty(SearchViewModel.prototype, "disabled", {
                    get: function () {
                        return this.loading();
                    },
                    enumerable: true,
                    configurable: true
                });
                SearchViewModel.createResultNavigator = function (seacrhModel, reportPreview) {
                    return new SearchResultNavigator(seacrhModel, reportPreview);
                };
                return SearchViewModel;
            })(DevExpress.Designer.Disposable);
            Preview.SearchViewModel = SearchViewModel;
            var SearchResultNavigator = (function (_super) {
                __extends(SearchResultNavigator, _super);
                function SearchResultNavigator(seachModel, reportPreview) {
                    var _this = this;
                    _super.call(this);
                    this.currentResult = ko.observable(null);
                    var goToMatchedResult = function (foundResult) {
                        if (!foundResult) {
                            return;
                        }
                        reportPreview.goToPage && reportPreview.goToPage(foundResult.pageIndex);
                        var page = reportPreview.pages.peek()[foundResult.pageIndex];
                        page && page.selectBrick(foundResult.indexes);
                    };
                    this.getFirstMatchFromPage = function (pageIndex, up, thisPageOnly) {
                        if (!seachModel.searchResult() || seachModel.searchResult().length === 0) {
                            return null;
                        }
                        var firstMatch;
                        var sortOutResult = function (index) {
                            seachModel.searchResult().forEach(function (m) {
                                if (thisPageOnly && m.pageIndex === index) {
                                    if (!firstMatch || (m.id < firstMatch.id && !up || m.id > firstMatch.id && up)) {
                                        firstMatch = m;
                                    }
                                }
                                else {
                                    if (m.pageIndex >= index && !up && (!firstMatch || m.id < firstMatch.id) || m.pageIndex <= index && up && (!firstMatch || m.id > firstMatch.id)) {
                                        firstMatch = m;
                                    }
                                }
                            });
                        };
                        sortOutResult(pageIndex);
                        !firstMatch && sortOutResult(up ? reportPreview.pages().length : 0);
                        return firstMatch;
                    };
                    var _setCurrentResult = function (highlight, resultId, thisPageOnly) {
                        if (seachModel.searchResult() && seachModel.searchResult().length !== 0) {
                            var currentResult = (resultId >= 0 && seachModel.searchResult().length > resultId) ? seachModel.searchResult()[resultId] : _this.getFirstMatchFromPage(reportPreview.pageIndex.peek(), seachModel.searchUp.peek(), thisPageOnly);
                            _this.currentResult(currentResult);
                            highlight && goToMatchedResult(_this.currentResult.peek());
                        }
                        else {
                            reportPreview.pages() && reportPreview.pages()[reportPreview.pageIndex()] && reportPreview.pages()[reportPreview.pageIndex()].selectBrick("");
                        }
                    };
                    this.goToResult = function (id) {
                        if (id !== 0 && !id) {
                            return null;
                        }
                        _setCurrentResult(true, id);
                    };
                    this._disposables.push(reportPreview.pageIndex.subscribe(function (newPageIndex) {
                        _this.currentResult(null);
                    }));
                    this._disposables.push(seachModel.searchResult.subscribe(function () {
                        _setCurrentResult(true);
                    }));
                    this.next = function (up) {
                        if (!seachModel.searchResult()) {
                            return false;
                        }
                        !_this.currentResult() && _this.currentResult(_this.getFirstMatchFromPage(reportPreview.pageIndex(), up));
                        if (!_this.currentResult()) {
                            return false;
                        }
                        var id, currentId = _this.currentResult().id;
                        if (up) {
                            id = (currentId === 0) ? seachModel.searchResult().length - 1 : (currentId - 1);
                        }
                        else {
                            id = (currentId === seachModel.searchResult().length - 1) ? 0 : (currentId + 1);
                        }
                        _this.currentResult(seachModel.searchResult()[id]);
                        goToMatchedResult(_this.currentResult());
                        return true;
                    };
                }
                return SearchResultNavigator;
            })(DevExpress.Designer.Disposable);
            Preview.SearchResultNavigator = SearchResultNavigator;
            var editor_prefix = "dx-searcheditor", EDITOR_CLASS = editor_prefix + "", EDITOR_BUTTON_CLASS = editor_prefix + "-button dx-widget dx-dropdowneditor-button", EDITOR_BUTTON_SELECTOR = "." + editor_prefix + "-button", EDITOR_BUTTON_ICON = editor_prefix + "-icon dx-dropdowneditor-icon dx-icon-dxrd-image-move";
            var dxSearchEditor = (function (_super) {
                __extends(dxSearchEditor, _super);
                function dxSearchEditor(element, options) {
                    this._activeStateUnit = EDITOR_BUTTON_SELECTOR;
                    var _self = this;
                    var searchModel = options && options.searchModel;
                    this._focusRequestRaised = function () {
                        _self.focus();
                    };
                    searchModel().focusRequested.subscribe(function (val) { return _self._focusRequestRaised(); });
                    options["onEnterKey"] = function (e) {
                        if (DevExpress.browser && DevExpress.browser.msie && e && e.component) {
                            e.component.blur();
                            e.component.focus();
                        }
                        _self.findNext(searchModel, e && e.jQueryEvent && e.jQueryEvent.shiftKey);
                    };
                    _super.call(this, element, options);
                }
                dxSearchEditor.prototype.findNext = function (searchModel, searchUp) {
                    if (!searchModel || !searchModel()) {
                        return false;
                    }
                    try {
                        if (searchUp) {
                            (searchModel().loading && !searchModel().loading()) && searchModel().findUp();
                        }
                        else {
                            (searchModel().loading && !searchModel().loading()) && searchModel().findDown();
                        }
                    }
                    finally {
                        return true;
                    }
                };
                dxSearchEditor.prototype._init = function () {
                    _super.prototype._init.call(this);
                    this.element().addClass(EDITOR_CLASS);
                };
                dxSearchEditor.prototype._optionChanged = function (obj, value) {
                    var name = obj.name || obj, newValue = value || obj.value;
                    switch (name) {
                        case "searchModel":
                            newValue && this.option("value", newValue.searchText());
                            break;
                        case "value":
                            var searchModel = this.option("searchModel");
                            searchModel && searchModel() && searchModel().searchText(newValue);
                        default:
                            _super.prototype._optionChanged.apply(this, arguments);
                    }
                };
                dxSearchEditor.prototype._render = function () {
                    _super.prototype._render.call(this);
                    this._renderButton("Down");
                    this._renderButton("Up");
                };
                dxSearchEditor.prototype._renderButton = function (direction) {
                    this._button = $("<div />").addClass(EDITOR_BUTTON_CLASS);
                    this._attachButtonEvents(direction);
                    this._buttonIcon = $("<div />").addClass(EDITOR_BUTTON_ICON + direction.toLowerCase()).appendTo(this._button);
                    var buttonsContainer = _super.prototype._buttonsContainer.call(this);
                    this._button.appendTo(buttonsContainer);
                };
                dxSearchEditor.prototype._attachButtonEvents = function (direction) {
                    var _this = this;
                    this._button.off("click");
                    if (!this.option("disabled")) {
                        this._button.on("click", function (e) {
                            _this.findNext(_this.option("searchModel"), direction.toLowerCase() === "up") && e.stopPropagation();
                        });
                    }
                };
                return dxSearchEditor;
            })(DevExpress.ui.dxTextBox);
            Preview.dxSearchEditor = dxSearchEditor;
            DevExpress.registerComponent("dxSearchEditor", dxSearchEditor);
        })(Preview = Report.Preview || (Report.Preview = {}));
    })(Report = DevExpress.Report || (DevExpress.Report = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Report;
    (function (Report) {
        var Preview;
        (function (Preview) {
            var DocumentMapModel = (function () {
                function DocumentMapModel(reportPreview) {
                    var _this = this;
                    var treeListController = new DocumentMapTreeListController();
                    this.isEmpty = ko.pureComputed(function () {
                        return !(reportPreview.documentMap() && reportPreview.documentMap().nodes && (reportPreview.documentMap().nodes.length !== 0));
                    });
                    treeListController.clickHandler = function (item) {
                        var bookmark = item.data.bookmark;
                        if (bookmark) {
                            var pageIndex = bookmark.pageIndex < 0 ? 0 : bookmark.pageIndex;
                            reportPreview.pages.peek()[pageIndex].selectBrick(bookmark.indexes);
                        }
                    };
                    this.documentMapOptions = ko.pureComputed(function () {
                        var rootNodeSubscribtion = treeListController.selectedItem.subscribe(function (item) {
                            item.path === "0" && item.collapsed() && item.toggleCollapsed();
                            rootNodeSubscribtion.dispose();
                        });
                        return {
                            itemsProvider: new DocumentMapItemsProvider(reportPreview.documentMap()),
                            selectedPath: ko.observable("0"),
                            treeListController: treeListController
                        };
                    });
                    this.tabInfo = new DevExpress.Designer.TabInfo(DevExpress.Designer.getLocalization("Document Map"), "dxrd-preview-document-map", this, "reportexplorer", ko.pureComputed(function () { return !_this.isEmpty(); }));
                }
                return DocumentMapModel;
            })();
            Preview.DocumentMapModel = DocumentMapModel;
            var DocumentMapItemsProvider = (function () {
                function DocumentMapItemsProvider(bookmark) {
                    var _this = this;
                    this.bookmarkDict = {};
                    this.getItems = function (pathRequest) {
                        var result = $.Deferred();
                        if (bookmark) {
                            if (pathRequest.fullPath) {
                                var nodes = _this._selectNode(bookmark, pathRequest.fullPath);
                                result.resolve(nodes);
                            }
                            else {
                                var root = _this._getRootNode(bookmark);
                                result.resolve(root);
                            }
                        }
                        return result.promise();
                    };
                }
                DocumentMapItemsProvider.prototype._selectNode = function (root, path) {
                    if (!!this.bookmarkDict[path]) {
                        return this.bookmarkDict[path];
                    }
                    var pathComponents = path.split("."), currentNode = root;
                    if (pathComponents[0] !== "0") {
                        return null;
                    }
                    for (var i = 1, index = pathComponents[i]; i < pathComponents.length; i++, index = pathComponents[i]) {
                        if (currentNode && currentNode.nodes && currentNode.nodes[index]) {
                            currentNode = currentNode.nodes[index];
                        }
                        else {
                            return null;
                        }
                    }
                    var result = DocumentMapItemsProvider.fillNode(currentNode);
                    if (result && result.length !== 0) {
                        this.bookmarkDict[path] = result;
                    }
                    return result;
                };
                DocumentMapItemsProvider.fillNode = function (bookmark) {
                    if (!bookmark || !bookmark.nodes || bookmark.nodes.length <= 0) {
                        return null;
                    }
                    return bookmark.nodes.map(function (node, i) {
                        return { name: i + "", displayName: node.text, isList: node.nodes && node.nodes.length > 0, bookmark: node, specifics: "node" };
                    });
                };
                DocumentMapItemsProvider.prototype._getRootNode = function (bookmark) {
                    return [{ name: "0", displayName: bookmark.text, isList: bookmark.nodes && bookmark.nodes.length > 0, bookmark: bookmark, specifics: "node" }];
                };
                return DocumentMapItemsProvider;
            })();
            Preview.DocumentMapItemsProvider = DocumentMapItemsProvider;
            var DocumentMapTreeListController = (function () {
                function DocumentMapTreeListController() {
                    this.selectedItem = ko.observable(null);
                    this.clickHandler = function (item) { return void 0; };
                }
                DocumentMapTreeListController.prototype.itemsFilter = function (item) {
                    return true;
                };
                DocumentMapTreeListController.prototype.hasItems = function (item) {
                    return item.isList === true;
                };
                DocumentMapTreeListController.prototype.canSelect = function (value) {
                    return true;
                };
                DocumentMapTreeListController.prototype.select = function (value) {
                    if (this.canSelect(value) && value !== this.selectedItem.peek()) {
                        this.selectedItem.peek() && this.selectedItem.peek().isSelected(false);
                        this.selectedItem(value);
                        value.isSelected(true);
                    }
                };
                return DocumentMapTreeListController;
            })();
            Preview.DocumentMapTreeListController = DocumentMapTreeListController;
        })(Preview = Report.Preview || (Report.Preview = {}));
    })(Report = DevExpress.Report || (DevExpress.Report = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Report;
    (function (Report) {
        var Preview;
        (function (Preview) {
            var ParameterHelper = (function () {
                function ParameterHelper() {
                }
                ParameterHelper.prototype._isKnownEnumType = function (type) {
                    return !!this._knownEnums && this._knownEnums.some(function (knownEnumType) {
                        return knownEnumType.enumType === type;
                    });
                };
                ParameterHelper.getSerializationValue = function (value, dateConverter) {
                    if (value instanceof Array) {
                        return value.map(function (item) {
                            var itemValue = ko.isObservable(item.value) ? item.value() : item;
                            return (itemValue instanceof Date) ? dateConverter(itemValue) : itemValue;
                        });
                    }
                    return (value instanceof Date) ? dateConverter(value) : value;
                };
                ParameterHelper.prototype.initialize = function (knownEnums, _customizeParameterEditors) {
                    if (arguments.length > 0) {
                        this._knownEnums = knownEnums;
                        if (arguments.length > 1) {
                            this._customizeParameterEditors = _customizeParameterEditors;
                        }
                    }
                };
                ParameterHelper.prototype.createInfo = function (parameter) {
                    var parameterDescriptor = parameter.getParameterDescriptor();
                    var typeString = this.isEnumType(parameter) ? "Enum" : ko.unwrap(parameterDescriptor.type);
                    return { propertyName: "value", displayName: parameterDescriptor["displayName"], editor: DevExpress.Designer.getEditorType(typeString), valuesArray: this.getValueCollection(parameter) };
                };
                ParameterHelper.prototype.createMultiValue = function (parameter, value) {
                    var newValue = ko.observable();
                    if (value !== null && value !== void 0) {
                        newValue(value);
                    }
                    return { value: newValue, getInfo: function () {
                        return [parameter.multiValueInfo()];
                    } };
                };
                ParameterHelper.prototype.createMultiValueArray = function (fromArray, parameter, convertSingleValue) {
                    var _this = this;
                    var converter = convertSingleValue ? convertSingleValue : this.valueConverter(ko.unwrap(parameter.type));
                    return ko.observableArray(fromArray.map(function (item) {
                        return _this.createMultiValue(parameter, converter(item));
                    }));
                };
                ParameterHelper.prototype.isEnumType = function (parameter) {
                    return this._isKnownEnumType(ko.unwrap(parameter.type));
                };
                ParameterHelper.prototype.getValueCollection = function (parameter) {
                    var type = ko.unwrap(parameter.type);
                    if (this._isKnownEnumType(type)) {
                        var currentKnownEnumInfo = this._knownEnums.filter(function (knownEnumType) {
                            return knownEnumType.enumType === type;
                        })[0];
                        if (currentKnownEnumInfo && currentKnownEnumInfo.values && currentKnownEnumInfo.values.length !== 0) {
                            return currentKnownEnumInfo.values.map(function (val) {
                                return { value: val.value, displayValue: val.displayName };
                            });
                        }
                    }
                };
                ParameterHelper.prototype.getParameterInfo = function (parameter) {
                    var _this = this;
                    var valueInfo = this.createInfo(parameter);
                    if (this._customizeParameterEditors) {
                        this._customizeParameterEditors(parameter.getParameterDescriptor(), valueInfo);
                    }
                    parameter.multiValueInfo($.extend(true, {}, valueInfo, { propertyName: "value" }));
                    if (ko.unwrap(parameter.isMultiValue)) {
                        valueInfo.editor = DevExpress.Designer.getEditorType(parameter["isMultiValueWithLookUp"] ? "multiValueWithLookUp" : "multiValue");
                        valueInfo["addHandler"] = function () {
                            return _this.createMultiValue(parameter);
                        };
                    }
                    return valueInfo;
                };
                ParameterHelper.prototype.valueConverter = function (type) {
                    return (function (val) {
                        return val;
                    });
                };
                return ParameterHelper;
            })();
            Preview.ParameterHelper = ParameterHelper;
            var PreviewParameterHelper = (function (_super) {
                __extends(PreviewParameterHelper, _super);
                function PreviewParameterHelper(knownEnums, customizeParameterEditors) {
                    _super.call(this);
                    this.initialize(knownEnums, customizeParameterEditors);
                }
                PreviewParameterHelper.prototype.mapLookUpValues = function (type, lookUpValues) {
                    var converter = this.valueConverter(type);
                    return $.map(lookUpValues || [], function (lookUpValue) {
                        return { value: converter(lookUpValue.Value), displayValue: lookUpValue.Description };
                    });
                };
                PreviewParameterHelper.prototype.createInfo = function (parameter) {
                    var _helper = this;
                    var info = _super.prototype.createInfo.call(this, parameter);
                    info.propertyName = parameter.path;
                    if (parameter.type === "System.DateTime") {
                        info.validationRules = [{ type: 'required', message: DevExpress.Designer.getLocalization('The value cannot be empty') }];
                    }
                    Object.defineProperty(info, 'valuesArray', {
                        get: function () {
                            if (parameter.isFilteredLookUpSettings || parameter.lookUpValues() && parameter.lookUpValues().length !== 0) {
                                return parameter.lookUpValues();
                            }
                            return _helper.getValueCollection(parameter);
                        },
                        set: function (values) {
                            parameter.lookUpValues(values);
                        }
                    });
                    return info;
                };
                PreviewParameterHelper.prototype.isEnumType = function (parameter) {
                    return parameter.isFilteredLookUpSettings || !!parameter.lookUpValues() || _super.prototype.isEnumType.call(this, parameter);
                };
                PreviewParameterHelper.prototype.valueConverter = function (type) {
                    if (type === "System.DateTime") {
                        return function (dateString) {
                            return Globalize.parseDate(dateString, ["yyyy-MM-dd", "MM/dd/yyyy HH:mm:ss"]);
                        };
                    }
                    return _super.prototype.valueConverter.call(this, type);
                };
                return PreviewParameterHelper;
            })(ParameterHelper);
            Preview.PreviewParameterHelper = PreviewParameterHelper;
            var PreviewParameter = (function (_super) {
                __extends(PreviewParameter, _super);
                function PreviewParameter(parameterModel, parameterHelper) {
                    var _this = this;
                    _super.call(this);
                    this.valueInfo = ko.observable();
                    this.lookUpValues = ko.observableArray();
                    this.multiValueInfo = ko.observable();
                    this.type = parameterModel.TypeString;
                    this.path = parameterModel.Path;
                    this.isFilteredLookUpSettings = parameterModel.IsFilteredLookUpSettings;
                    this._originalLookUpValues = parameterModel.LookUpValues ? parameterHelper.mapLookUpValues(this.type, parameterModel.LookUpValues || []) : null;
                    this.lookUpValues(this._originalLookUpValues);
                    this.isMultiValue = parameterModel.MultiValue;
                    this.isMultiValueWithLookUp = this.isMultiValue && !!this.lookUpValues();
                    this.getParameterDescriptor = function () {
                        return { description: parameterModel.Description, displayName: parameterModel.Description || parameterModel.Name, name: parameterModel.Name, type: parameterModel.TypeString, value: parameterModel.Value, visible: parameterModel["Visible"] };
                    };
                    this._disposables.push(ko.computed(function () {
                        var info = parameterHelper.getParameterInfo(_this);
                        info.propertyName = parameterModel.Path;
                        _this.valueInfo(info);
                    }));
                    this._originalValue = parameterModel.Value;
                    this.initialize(this._originalValue, parameterHelper);
                }
                PreviewParameter.prototype.safeAssignObservable = function (name, value) {
                    if (this[name]) {
                        this[name](value());
                    }
                    else {
                        this[name] = value;
                    }
                };
                PreviewParameter.prototype.initialize = function (value, parameterHelper) {
                    var _this = this;
                    if (this.isMultiValueWithLookUp) {
                        this.safeAssignObservable("_value", ko.observableArray((value || []).map(function (arrayItem) {
                            return parameterHelper.valueConverter(_this.type)(arrayItem);
                        })));
                        this.safeAssignObservable("value", ko.observable(new DevExpress.Designer.Widgets.MultiValuesHelper(this._value, this.lookUpValues())));
                    }
                    else if (this.isMultiValue) {
                        this.safeAssignObservable("value", parameterHelper.createMultiValueArray(value, this));
                    }
                    else {
                        this.safeAssignObservable("value", ko.observable(parameterHelper.valueConverter(this.type)(value)));
                    }
                };
                return PreviewParameter;
            })(DevExpress.Designer.Disposable);
            Preview.PreviewParameter = PreviewParameter;
            var PreviewParametersViewModel = (function (_super) {
                __extends(PreviewParametersViewModel, _super);
                function PreviewParametersViewModel(reportPreview, parameterHelper) {
                    var _this = this;
                    _super.call(this);
                    this._parameters = [];
                    this._getLookUpValueRequest = function (argsObject) {
                        return $.post(Preview.HandlerUri, { actionKey: 'getLookUpValues', arg: encodeURIComponent(JSON.stringify(argsObject)) });
                    };
                    this._getDoneGetLookUpValueHandler = function () {
                        var parametersViewModel = _this;
                        return function (response) {
                            try {
                                if (!response.result || !response.result.parameters) {
                                    return;
                                }
                                response.result.parameters.forEach(function (parameter, i) {
                                    parametersViewModel._setLookUpValues(parameter.Key, parameter.Value, i === 0);
                                });
                            }
                            finally {
                                parametersViewModel.parametersLoading(false);
                            }
                        };
                    };
                    this._getFailGetLookUpValueHandler = function () {
                        var parametersViewModel = _this;
                        return function (jqXHRError) {
                            parametersViewModel.parametersLoading(false);
                            var errorMessage = DevExpress.Designer.getLocalization("Cannot supply filtered lookup values to a report parameter editor");
                            var serverError = DevExpress.Designer.getErrorMessage(jqXHRError);
                            serverError && (errorMessage += ": " + serverError);
                            Preview.MessageHandler.processError(errorMessage);
                        };
                    };
                    this.getInfo = ko.observable(null);
                    this.needToRefreshLookUps = ko.observable(false);
                    this.isEmpty = ko.observable(true);
                    this.parametersLoading = ko.observable(false);
                    this._reportPreview = reportPreview;
                    this.parameterHelper = parameterHelper || new PreviewParameterHelper();
                    var _self = this;
                    this.submit = function () {
                        if (_self.parametersLoading()) {
                            return;
                        }
                        _self.parametersLoading(true);
                        var promise = reportPreview.startBuild();
                        promise && promise.done(function (val) {
                            _self.parametersLoading(false);
                        });
                    };
                    this._disposables.push(reportPreview.originalParametersInfo.subscribe(function (originalParametersInfo) {
                        _this.initialize(originalParametersInfo);
                    }));
                    this.initialize(reportPreview.originalParametersInfo());
                    this.tabInfo = new DevExpress.Designer.TabInfo(DevExpress.Designer.getLocalization("Parameters"), "dxrd-preview-parameters", this, "parameters", ko.pureComputed(function () { return !_this.isEmpty(); }));
                }
                PreviewParametersViewModel.prototype._convertLocalDateToUTC = function (localDate) {
                    return new Date(Date.UTC(localDate.getFullYear(), localDate.getMonth(), localDate.getDate(), localDate.getHours(), localDate.getMinutes(), localDate.getSeconds()));
                };
                PreviewParametersViewModel.prototype._add = function (parameterModel) {
                    var _this = this;
                    this.isEmpty(false);
                    var parameter = new PreviewParameter(parameterModel, this.parameterHelper);
                    this._parameters.push(parameter);
                    this.needToRefreshLookUps(this.needToRefreshLookUps() || parameter.isFilteredLookUpSettings);
                    this._disposables.push((parameter.isMultiValueWithLookUp ? parameter._value : parameter.value).subscribe(function (newValue) {
                        if (!_this.parametersLoading() && _this.needToRefreshLookUps()) {
                            _this.getLookUpValues(parameter.path);
                        }
                    }));
                    this[parameter.path] = parameter.value;
                };
                PreviewParametersViewModel.prototype._setLookUpValues = function (parameterPath, parameterLookUpValues, assignFirstLookUpValue) {
                    var _this = this;
                    if (!this[parameterPath]) {
                        return;
                    }
                    this._parameters.forEach(function (matchedParameter) {
                        if (matchedParameter.path !== parameterPath) {
                            return;
                        }
                        matchedParameter.lookUpValues(_this.parameterHelper.mapLookUpValues(matchedParameter.type, parameterLookUpValues));
                        var _parameterValuesContainedInLookUps = _this._getParameterValuesContainedInLookups(parameterLookUpValues, matchedParameter);
                        if (matchedParameter.isMultiValue) {
                            matchedParameter.initialize(_parameterValuesContainedInLookUps.length > 0 ? _parameterValuesContainedInLookUps : assignFirstLookUpValue && parameterLookUpValues.length > 0 ? [parameterLookUpValues[0].Value] : [], _this.parameterHelper);
                        }
                        else {
                            matchedParameter.initialize(_parameterValuesContainedInLookUps[0] && _parameterValuesContainedInLookUps[0].Value || assignFirstLookUpValue && parameterLookUpValues.length > 0 ? parameterLookUpValues[0].Value : null, _this.parameterHelper);
                        }
                    });
                };
                PreviewParametersViewModel.prototype._getParameterValuesContainedInLookups = function (parameterLookUpValues, parameter) {
                    var _this = this;
                    if (parameterLookUpValues) {
                        if (parameter.isMultiValue) {
                            var selectedItems = (parameter.value() && parameter.value().selectedItems() || []).map(function (item) {
                                return item.value;
                            });
                            return selectedItems.filter(function (item) { return _this._filterParameterValuesContainsInLookups(parameterLookUpValues, parameter.type, item).length > 0; });
                        }
                        else {
                            return this._filterParameterValuesContainsInLookups(parameterLookUpValues, parameter.type, parameter.value());
                        }
                    }
                    return [];
                };
                PreviewParametersViewModel.prototype._filterParameterValuesContainsInLookups = function (parameterLookUpValues, parameterType, value) {
                    var _this = this;
                    return parameterLookUpValues.filter(function (x) { return _this._compareValues(_this.parameterHelper.valueConverter(parameterType)(x.Value), value); });
                };
                PreviewParametersViewModel.prototype._compareValues = function (value1, value2) {
                    if (value1 instanceof Date && value2 instanceof Date) {
                        return value1 - value2 === 0;
                    }
                    return value1 === value2;
                };
                PreviewParametersViewModel.prototype.initialize = function (originalParametersInfo) {
                    var _this = this;
                    this._parameters = [];
                    if (!originalParametersInfo) {
                        this.isEmpty(true);
                        this.getInfo([]);
                        return;
                    }
                    this.parameterHelper.initialize(originalParametersInfo.knownEnums);
                    (originalParametersInfo.parameters || []).forEach(function (parameter) {
                        if (parameter["Visible"]) {
                            _this._add(parameter);
                        }
                    });
                    if (this._parameters.length === 0) {
                        this.isEmpty(true);
                    }
                    var info = this._parameters.map(function (parameter) {
                        return parameter.valueInfo();
                    });
                    this.getInfo(info);
                    if (!originalParametersInfo.shouldRequestParameters || this.isEmpty()) {
                        this.submit();
                    }
                    else {
                        this._reportPreview.removeEmptyPages();
                        this.tabInfo.active(true);
                        this._reportPreview.pageLoading(false);
                    }
                };
                PreviewParametersViewModel.prototype.getPathsAfterPath = function (parameterPath) {
                    var startIndex = 0;
                    $.each(this._parameters, function (index, value) {
                        if (value.path === parameterPath) {
                            startIndex = index + 1;
                            return true;
                        }
                    });
                    var paths = this._parameters.filter(function (value, index) {
                        return (value.isFilteredLookUpSettings && (index >= startIndex));
                    }).map(function (x) { return x.path; });
                    return paths || [];
                };
                PreviewParametersViewModel.prototype.serializeParameters = function () {
                    var params = [], self = this;
                    this._parameters.forEach(function (parameter) {
                        var convertItem = function (item) {
                            return (parameter.type === "System.DateTime" && !!item && (item instanceof Date)) ? self._convertLocalDateToUTC(item) : item;
                        };
                        var value = parameter.isMultiValueWithLookUp ? parameter._value() : parameter.value();
                        params.push({ Value: ParameterHelper.getSerializationValue(value, convertItem), Key: parameter.path, TypeString: parameter.type });
                    });
                    return params;
                };
                PreviewParametersViewModel.prototype.restore = function () {
                    var _this = this;
                    if (this.parametersLoading()) {
                        return;
                    }
                    try {
                        this.parametersLoading(true);
                        this._parameters.forEach(function (parameter, i) {
                            parameter.lookUpValues(parameter._originalLookUpValues);
                            parameter.initialize(parameter._originalValue, _this.parameterHelper);
                        });
                    }
                    finally {
                        this.parametersLoading(false);
                    }
                };
                PreviewParametersViewModel.prototype.isPropertyDisabled = function (name) {
                    return this.parametersLoading();
                };
                PreviewParametersViewModel.prototype.getLookUpValues = function (changedParameterPath) {
                    var requiredParameterPaths = this.getPathsAfterPath(changedParameterPath);
                    if (!requiredParameterPaths || requiredParameterPaths.length === 0) {
                        return;
                    }
                    this.parametersLoading(true);
                    var argsObject = {
                        reportId: this._reportPreview.reportId,
                        requiredParameterPaths: requiredParameterPaths,
                        parameters: this.serializeParameters(),
                        timeZoneOffset: 0 - new Date().getTimezoneOffset()
                    };
                    this._getLookUpValueRequest(argsObject).done(this._getDoneGetLookUpValueHandler()).fail(this._getFailGetLookUpValueHandler());
                };
                return PreviewParametersViewModel;
            })(DevExpress.Designer.Disposable);
            Preview.PreviewParametersViewModel = PreviewParametersViewModel;
        })(Preview = Report.Preview || (Report.Preview = {}));
    })(Report = DevExpress.Report || (DevExpress.Report = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Report;
    (function (Report) {
        var Preview;
        (function (Preview) {
            Preview.HandlerUri = "DXXRDV.axd";
            Preview.TimeOut = 120000;
            Preview.MessageHandler = {
                processError: function (message) {
                    DevExpress.Designer.NotifyAboutWarning(message);
                },
                processMessage: function (message) {
                    DevExpress.Designer.NotifyAboutWarning(message);
                },
                processWarning: function (message) {
                    DevExpress.Designer.NotifyAboutWarning(message);
                }
            };
            Preview.generateGuid = function () {
                var getNewQuartet = function (i) {
                    return Math.floor((1 + Math.random()) * Math.pow(0x10000, i)).toString(16).substring(1);
                };
                return getNewQuartet(2) + '-' + getNewQuartet(1) + '-' + getNewQuartet(1) + '-' + getNewQuartet(1) + '-' + getNewQuartet(3);
            };
            var ReportPreview = (function (_super) {
                __extends(ReportPreview, _super);
                function ReportPreview(handlerUri, previewRequestWrapper, previewHandlersHelper) {
                    var _this = this;
                    _super.call(this);
                    this.getPreviewPageBrickProvider = function (handlerUri, documentId) {
                        return new Preview.PreviewPageBrickProvider(Preview.HandlerUri, documentId);
                    };
                    this.predefinedZoomLevels = ko.observableArray([5, 2, 1.5, 1, 0.75, 0.5, 0.25]);
                    this._pageWidth = ko.observable(818);
                    this._pageHeight = ko.observable(1058);
                    this._currentReportId = ko.observable(null);
                    this._currentDocumentId = ko.observable(null);
                    this._currentOperationId = ko.observable(null);
                    this._stopBuildRequests = {};
                    this._closeDocumentRequests = {};
                    this._startBuildOperationId = "";
                    this._drillDownState = [];
                    this.originalParametersInfo = ko.observable(null);
                    this.pageIndex = ko.observable(0);
                    this.showMultipagePreview = ko.observable(false);
                    this.documentMap = ko.observable();
                    this.exportOptionsModel = ko.observable();
                    this.pageLoading = ko.observable(false);
                    this.documentBuilding = ko.observable(false);
                    this.pendingOperation = ko.observable(false);
                    this.progressBar = new Preview.ProgressViewModel();
                    this.pages = ko.observableArray([]).extend({ rateLimit: { timeout: 20, method: "notifyWhenChangesStop" } });
                    this.isAutoFit = ko.observable(true);
                    this._zoom = ko.observable(1);
                    this.zoom = ko.pureComputed({
                        read: function () {
                            if (_this.isAutoFit() || _this._zoom() === 0) {
                                return 0;
                            }
                            return _this._zoom();
                        },
                        write: function (val) {
                            if (val !== 0) {
                                _this._zoom(val);
                                _this.isAutoFit(false);
                            }
                            else {
                                _this.isAutoFit(true);
                            }
                        }
                    });
                    this._currentPage = ko.pureComputed(function () {
                        if (_this.pageIndex() === -1) {
                            return DevExpress.Designer.getLocalization('0 pages');
                        }
                        else {
                            return (_this.pageIndex() + 1) + " " + DevExpress.Designer.getLocalization('of') + " " + _this.pages().length;
                        }
                    });
                    this._raiseOnSizeChanged = function () {
                        _this.onSizeChanged() && _this.onSizeChanged()();
                    };
                    this.previewSize = ko.observable(0);
                    this.onSizeChanged = ko.observable();
                    this.previewVisible = ko.observable(false);
                    this.canSwitchToDesigner = ko.observable(true);
                    Preview.HandlerUri = handlerUri || Preview.HandlerUri;
                    this.previewHandlersHelper = previewHandlersHelper || new Preview.PreviewHandlersHelper(this);
                    this.previewWrapper = previewRequestWrapper || new Preview.PreviewRequestWrapper();
                    this._disposables.push(this.previewSize.subscribe(function () { return _this._raiseOnSizeChanged(); }));
                    this._disposables.push(this.zoom.subscribe(function () { return _this._raiseOnSizeChanged(); }));
                }
                ReportPreview.prototype._doDrillDown = function (drillDownKey) {
                    this._drillDownState.filter(function (x) { return x.Key === drillDownKey && (x.Value = !x.Value); });
                    this.closeDocument();
                    this.progressBar.complete();
                    this.documentMap(null);
                    for (var i = this.pages().length - 1; i >= 0; i--) {
                        var page = this.pages()[i];
                        if (i > this.pageIndex()) {
                            this.pages.remove(page);
                        }
                        else {
                            page.pageIndex = -1;
                            page.brick(null);
                            page._setPageImgSrc(null, 1);
                        }
                    }
                    this._startBuildRequest();
                };
                ReportPreview.prototype.removeEmptyPages = function () {
                    for (var idx = this.pages().length - 1; idx >= 0; idx--) {
                        var tempPage = this.pages()[idx];
                        (tempPage.isEmpty || tempPage.pageIndex === -1) && this.pages.remove(tempPage);
                    }
                };
                ReportPreview.prototype._initialize = function () {
                    this._drillDownState = [];
                    this.closeDocument();
                    this.documentMap(null);
                    this.pageIndex(-1);
                    this.pageLoading(true);
                    this.progressBar.complete();
                    this.pages([new Preview.PreviewPage(-1, this._pageWidth, this._pageHeight, this._zoom, this._currentDocumentId, null, this.pageLoading)]);
                };
                ReportPreview.prototype._cleanTabInfo = function () {
                    this.exportOptionsModel(null);
                    this.documentMap(null);
                };
                ReportPreview.prototype._export = function (args, actionUri, printable) {
                    var _this = this;
                    var deffered = $.Deferred();
                    if (Preview.AsyncExportApproach) {
                        var doneRequest = this.previewHandlersHelper.getDoneStartExportHandler(deffered, printable);
                        var errorRequest = this.previewHandlersHelper.getErrorStartBuildHandler(deffered);
                        this.progressBar.text(DevExpress.Designer.getLocalization('Document is exporting...'));
                        this.progressBar.cancelText(DevExpress.Designer.getLocalization('Cancel'));
                        this.progressBar.startProgress(function () {
                            _this._currentOperationId(null);
                        });
                        this.previewWrapper.getStartExportOperation(args).done(doneRequest).fail(errorRequest);
                    }
                    else {
                        deffered.resolve(true);
                        window.open(actionUri + "?actionKey=exportTo&arg=" + args, "_blank");
                    }
                    return deffered.promise();
                };
                ReportPreview.prototype.createBrickClickProcessor = function (cyclePageIndex) {
                    var _self = this;
                    return function (brick) {
                        _self.goToPage(cyclePageIndex, true);
                        brick && _self.pages()[cyclePageIndex].selectBrick("");
                        if (_self.customProcessBrickClick && _self.customProcessBrickClick(cyclePageIndex, brick)) {
                            return;
                        }
                        if (brick && brick.navigation) {
                            brick.navigation.drillDownKey && _self._doDrillDown(brick.navigation.drillDownKey);
                            if (brick.navigation.pageIndex >= 0) {
                                var targetPage = _self.pages().filter(function (page) { return page.pageIndex === brick.navigation.pageIndex; })[0];
                                if (targetPage) {
                                    _self.goToPage(brick.navigation.pageIndex);
                                    targetPage.selectBrick(brick.navigation.indexes);
                                }
                            }
                            else {
                                var _target = brick.navigation.target || '_blank';
                                brick.navigation.url && window.open(brick.navigation.url, _target);
                            }
                        }
                    };
                };
                ReportPreview.prototype.openReport = function (reportName) {
                    var _this = this;
                    this._cleanTabInfo();
                    this.closeReport();
                    this.originalParametersInfo(null);
                    var deferred = $.Deferred();
                    this._openReportOperationDeferred = deferred;
                    this.previewWrapper.openReport(reportName).done(function (response) {
                        deferred.resolve(response);
                    }).fail(function (error) {
                        deferred.reject(error);
                        _this._processError(DevExpress.Designer.getLocalization("Could not open report '" + reportName + "'"), error);
                    });
                    this.initialize(deferred.promise());
                };
                ReportPreview.prototype.initialize = function (initializeData) {
                    this._currentReportId(null);
                    this._currentDocumentId(null);
                    var _self = this;
                    this._initialize();
                    initializeData.done(function (result) {
                        var previewInitialize = result.result;
                        if (previewInitialize && !previewInitialize.error && previewInitialize.reportId) {
                            _self._currentReportId(previewInitialize.reportId);
                            if (previewInitialize.pageHeight) {
                                _self._pageHeight(previewInitialize.pageHeight);
                            }
                            if (previewInitialize.pageWidth) {
                                _self._pageWidth(previewInitialize.pageWidth);
                            }
                            var deserializedExportOptions = new DevExpress.Designer.Report.ExportOptionsPreview(JSON.parse(previewInitialize.exportOptions));
                            _self.exportOptionsModel(deserializedExportOptions);
                            _self.originalParametersInfo(previewInitialize.parametersInfo);
                        }
                        else {
                            _self.pageLoading(false);
                            _self._processError(DevExpress.Designer.getLocalization("The report preview initialization has failed"), previewInitialize && previewInitialize.error);
                        }
                    }).fail(function (error) {
                        _self.removeEmptyPages();
                    });
                };
                ReportPreview.prototype.deactivate = function () {
                    this._initialize();
                    this._cleanTabInfo();
                    this.closeReport();
                    this._currentDocumentId(null);
                    this._currentReportId(null);
                };
                ReportPreview.prototype.startBuild = function () {
                    this._initialize();
                    return this._startBuildRequest();
                };
                ReportPreview.prototype._initializeStartBuild = function () {
                    var _this = this;
                    if (this.pendingOperation() || this._startBuildOperationId) {
                        return false;
                    }
                    this._startBuildOperationId = Preview.generateGuid();
                    this._currentDocumentId(null);
                    this.progressBar.text(DevExpress.Designer.getLocalization('Document is building...'));
                    this.progressBar.cancelText(DevExpress.Designer.getLocalization('Cancel'));
                    this.progressBar.startProgress(function () {
                        _this.documentBuilding(false);
                    }, function () {
                        _this.stopBuild();
                    });
                    this.documentBuilding(true);
                    return true;
                };
                ReportPreview.prototype._startBuildRequest = function () {
                    if (!this._initializeStartBuild()) {
                        return null;
                    }
                    var deffered = $.Deferred();
                    var doneRequest = this.previewHandlersHelper.getDoneStartBuildHandler(deffered);
                    var errorRequest = this.previewHandlersHelper.getErrorStartBuildHandler(deffered);
                    this.previewWrapper.startBuildRequest().done(doneRequest).fail(errorRequest);
                    return deffered.promise();
                };
                ReportPreview.prototype.getExportStatus = function (operationId) {
                    var _this = this;
                    var deffered = $.Deferred();
                    var doneHandler = this.previewHandlersHelper.getDoneExportStatusHandler(deffered, operationId);
                    var errorHandler = this.previewHandlersHelper.getErrorExportStatusHandler(deffered);
                    setTimeout(function () {
                        _this.previewWrapper.getExportStatusRequest(operationId).done(doneHandler).fail(errorHandler);
                    }, 250);
                    return deffered.promise();
                };
                ReportPreview.prototype.getExportResult = function (operationId, printDisposition) {
                    var arg = encodeURIComponent(JSON.stringify({ id: operationId, printable: !!printDisposition }));
                    window.open(Preview.HandlerUri + "?actionKey=getExportResult&arg=" + arg, "_blank");
                };
                ReportPreview.prototype.getBuildStatus = function (documentId) {
                    var _this = this;
                    var deffered = $.Deferred();
                    var doneHandler = this.previewHandlersHelper.getDoneGetBuildStatusHandler(deffered, documentId);
                    var errorHandler = this.previewHandlersHelper.getErrorGetBuildStatusHandler(deffered);
                    setTimeout(function () {
                        _this.previewWrapper.getBuildStatusRequest(documentId).done(doneHandler).fail(errorHandler);
                    }, 250);
                    return deffered.promise();
                };
                ReportPreview.prototype.getDocumentData = function (documentId) {
                    var _self = this;
                    this.previewWrapper.getDocumentData(documentId).done(function (response) {
                        if (!response.result) {
                            return;
                        }
                        _self._drillDownState = response.result.drillDownKeys || [];
                        _self.documentMap(response.result.documentMap);
                    }).fail(function (error) {
                        _self._processError(DevExpress.Designer.getLocalization("Cannot obtain additional document data for the current document"), error, _self._closeDocumentRequests[documentId]);
                    });
                };
                ReportPreview.prototype.exportDocumentTo = function (format) {
                    if (!this._currentDocumentId())
                        return;
                    var serializedExportOptions = this.exportOptionsModel() ? JSON.stringify(new DevExpress.JS.Utils.ModelSerializer().serialize(this.exportOptionsModel())) : null;
                    var args = encodeURIComponent(JSON.stringify({
                        documentId: this._currentDocumentId(),
                        exportOptions: serializedExportOptions,
                        format: format
                    }));
                    this._export(args, Preview.HandlerUri);
                };
                ReportPreview.prototype.printDocument = function (pageIndex) {
                    if (!this._currentDocumentId())
                        return;
                    var exportOptions = new DevExpress.Designer.Report.ExportOptionsPreview({});
                    exportOptions.pdf["showPrintDialogOnOpen"] = true;
                    pageIndex = parseInt(pageIndex);
                    if ((!!pageIndex && pageIndex > 0 || pageIndex === 0) && (this.pages().length > pageIndex)) {
                        (exportOptions.pdf["pageRange"] = pageIndex + 1);
                    }
                    var serializedExportOptions = JSON.stringify(new DevExpress.JS.Utils.ModelSerializer().serialize(exportOptions));
                    var args = encodeURIComponent(JSON.stringify({
                        documentId: this._currentDocumentId(),
                        exportOptions: serializedExportOptions,
                        format: "printpdf"
                    }));
                    this._export(args, Preview.HandlerUri, true);
                };
                ReportPreview.prototype.stopBuild = function (documentId) {
                    var id = documentId || this._currentDocumentId();
                    if (!id) {
                        this._startBuildOperationId && (this._stopBuildRequests[this._startBuildOperationId] = true);
                        return;
                    }
                    this._stopBuildRequests[id] = true;
                    this.progressBar.complete();
                    this.previewWrapper.stopBuild(id);
                };
                ReportPreview.prototype.closeDocument = function (documentId) {
                    var _documentId = documentId || this._currentDocumentId();
                    if (!_documentId) {
                        this._startBuildOperationId && (this._closeDocumentRequests[this._startBuildOperationId] = true);
                        return;
                    }
                    this._closeDocumentRequests[_documentId] = true;
                    this.progressBar.complete();
                    this.previewWrapper.sendCloseRequest(_documentId);
                };
                ReportPreview.prototype.closeReport = function (reportId) {
                    this._openReportOperationDeferred && this._openReportOperationDeferred.reject();
                    if (!this._currentReportId()) {
                        return;
                    }
                    this.previewWrapper.sendCloseRequest(null, this._currentReportId());
                };
                ReportPreview.prototype.goToPage = function (pageIndex, forcePageChanging) {
                    if (!forcePageChanging && this.pageIndex.peek() === pageIndex || this.pages.peek().length === 0 || pageIndex < 0 || pageIndex >= this.pages.peek().length) {
                        return;
                    }
                    this.pageIndex(pageIndex);
                    this.pages.peek().forEach(function (page) {
                        page.active(page.pageIndex === pageIndex);
                    });
                };
                ReportPreview.prototype.getSelectedContent = function () {
                    var currentPage = this.pages()[this.pageIndex()];
                    if (!currentPage || !currentPage.brickColumnWidthArray) {
                        return "";
                    }
                    var activeBricks = [];
                    var getActiveBricks = function (currentBrick, resultArray) {
                        if (!currentBrick) {
                            return;
                        }
                        currentBrick.active() && currentBrick.genlIndex != -1 && activeBricks.push(currentBrick);
                        currentBrick.bricks && currentBrick.bricks.length != 0 && currentBrick.bricks.forEach(function (innerBrick) {
                            getActiveBricks(innerBrick, resultArray);
                        });
                    };
                    getActiveBricks(currentPage.brick(), activeBricks);
                    if (!activeBricks) {
                        return "";
                    }
                    var sortedActiveBricks = [];
                    var extendWithSpaces = function (width, text) {
                        text = text || "";
                        var spaceCount = width - text.length;
                        for (var i = 0; i <= spaceCount; i++) {
                            text += " ";
                        }
                        return text;
                    };
                    var firstUsedColumn = currentPage.brickColumnWidthArray.length, lastUsedColumn = -1;
                    activeBricks.forEach(function (activeBrick) {
                        var row = sortedActiveBricks[activeBrick.row];
                        if (!row) {
                            row = [];
                            sortedActiveBricks[activeBrick.row] = row;
                        }
                        row[activeBrick.col] = activeBrick.content && activeBrick.content[0].Value;
                        if (firstUsedColumn > activeBrick.col) {
                            firstUsedColumn = activeBrick.col;
                        }
                        if (lastUsedColumn < activeBrick.col) {
                            lastUsedColumn = activeBrick.col;
                        }
                    });
                    var result = "";
                    sortedActiveBricks.forEach(function (row) {
                        for (var c = firstUsedColumn; c <= lastUsedColumn; c++) {
                            result += extendWithSpaces(currentPage.brickColumnWidthArray[c], row[c]);
                        }
                        result += "\r\n";
                    });
                    return result;
                };
                ReportPreview.prototype._processError = function (error, jqXHR, ignoreExceptionPredicate) {
                    if (ignoreExceptionPredicate) {
                        return;
                    }
                    var serverError = DevExpress.Designer.getErrorMessage(jqXHR);
                    serverError && (error += ": " + serverError);
                    Preview.MessageHandler.processError(error);
                };
                Object.defineProperty(ReportPreview.prototype, "reportId", {
                    get: function () {
                        return this._currentReportId();
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(ReportPreview.prototype, "documentId", {
                    get: function () {
                        return this._currentDocumentId();
                    },
                    enumerable: true,
                    configurable: true
                });
                return ReportPreview;
            })(DevExpress.Designer.Disposable);
            Preview.ReportPreview = ReportPreview;
            function updatePreviewContentSize(previewSize) {
                return function () {
                    var rightAreaWidth = $(".dxrd-preview .dxrd-right-panel").outerWidth() + $(".dxrd-right-tabs").outerWidth();
                    var surfaceWidth = $(".dxrd-preview").width() - (rightAreaWidth + 5);
                    $(".dxrd-preview-wrapper").css("right", rightAreaWidth);
                    $(".dxrd-preview-wrapper").css("width", surfaceWidth);
                    previewSize(surfaceWidth);
                };
            }
            Preview.updatePreviewContentSize = updatePreviewContentSize;
            function updatePreviewZoomWithAutoFit(width, height) {
                var $previewWrapper = $(".dxrd-preview-wrapper");
                var $preview = $(".dxrd-preview");
                if ($previewWrapper.length === 0 || $preview.length === 0) {
                    return 1;
                }
                var rightAreaWidth = $(".dxrd-preview .dxrd-right-panel").outerWidth() + $(".dxrd-right-tabs").outerWidth();
                var surfaceWidth = $preview.width() - (rightAreaWidth + 10);
                var topAreaHeight = parseFloat($previewWrapper.css("top").split("px")[0]);
                var designerHeight = $preview.outerHeight();
                var needHeight = designerHeight - topAreaHeight;
                var zoomHeight = needHeight / (height + 6);
                var zoomWidth = surfaceWidth / width;
                return Math.min(zoomHeight, zoomWidth);
            }
            Preview.updatePreviewZoomWithAutoFit = updatePreviewZoomWithAutoFit;
            function createPreview(element, callbacks, localization, parametersInfo, handlerUri, previewVisible) {
                if (previewVisible === void 0) { previewVisible = true; }
                if (localization) {
                    Globalize.addCultureInfo("default", {
                        messages: localization
                    });
                }
                var previewWrapper = new Preview.PreviewRequestWrapper();
                var reportPreview = new ReportPreview(handlerUri, previewWrapper);
                var searchModel = new Preview.SearchViewModel(reportPreview);
                var documentMapModel = new Preview.DocumentMapModel(reportPreview);
                var parametersModel = new Preview.PreviewParametersViewModel(reportPreview, new Preview.PreviewParameterHelper(parametersInfo && parametersInfo.knownEnums, callbacks.customizeParameterEditors));
                var exportModel = new Preview.ExportOptionsModel(reportPreview);
                previewWrapper.initialize(reportPreview, parametersModel, searchModel);
                var tabPanel = new DevExpress.Designer.TabPanel([
                    parametersModel.tabInfo,
                    exportModel.tabInfo,
                    searchModel.tabInfo,
                    documentMapModel.tabInfo
                ], true);
                tabPanel.collapsed(true);
                var globalActionProviders = ko.observableArray([new Preview.PreviewActions(reportPreview), exportModel, searchModel, new Preview.PreviewDesignerActions(reportPreview)]);
                reportPreview.previewVisible(previewVisible);
                reportPreview.canSwitchToDesigner(!previewVisible);
                var designerModel = {
                    reportPreview: reportPreview,
                    parametersModel: parametersModel,
                    exportModel: exportModel,
                    searchModel: searchModel,
                    documentMapModel: documentMapModel,
                    tabPanel: tabPanel,
                    globalActionProviders: globalActionProviders,
                    globalActions: ko.pureComputed(function () {
                        var globalActions = [];
                        globalActionProviders().forEach(function (actionProvider) {
                            globalActions.push.apply(globalActions, actionProvider.getActions(reportPreview));
                        });
                        callbacks.customizeActions(globalActions);
                        return globalActions;
                    })
                };
                if (element) {
                    $(element).children().remove();
                    ko.applyBindings(designerModel, element);
                }
                var updatePreviewContentSize_ = updatePreviewContentSize(reportPreview.previewSize);
                $(window).bind("resize", function () {
                    updatePreviewContentSize_();
                });
                tabPanel.width.subscribe(function () {
                    setTimeout(updatePreviewContentSize_, 1);
                });
                updatePreviewContentSize_();
                return designerModel;
            }
            Preview.createPreview = createPreview;
        })(Preview = Report.Preview || (Report.Preview = {}));
    })(Report = DevExpress.Report || (DevExpress.Report = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Report;
    (function (Report) {
        var Preview;
        (function (Preview) {
            var PreviewDesignerActions = (function () {
                function PreviewDesignerActions(reportPreview) {
                    this.actions = [];
                    this.actions.push({
                        text: DevExpress.Designer.getLocalization("Design"),
                        imageClassName: "dxrd-image-design",
                        disabled: ko.observable(false),
                        visible: ko.pureComputed(function () {
                            return reportPreview.canSwitchToDesigner();
                        }),
                        hotKey: { ctrlKey: true, keyCode: "P".charCodeAt(0) },
                        clickAction: function () {
                            reportPreview.previewVisible(false);
                            reportPreview.deactivate();
                        }
                    });
                }
                PreviewDesignerActions.prototype.getActions = function (context) {
                    return this.actions;
                };
                return PreviewDesignerActions;
            })();
            Preview.PreviewDesignerActions = PreviewDesignerActions;
            var PreviewActions = (function () {
                function PreviewActions(reportPreview) {
                    this.actions = [];
                    var zoomStep = ko.observable(0.01);
                    var printDisabled = ko.pureComputed(function () {
                        var inProgress = reportPreview.progressBar.inProgress();
                        var documentBuilding = reportPreview.documentBuilding();
                        return reportPreview.pageIndex() === -1 || inProgress || documentBuilding;
                    });
                    this.actions.push({
                        text: DevExpress.Designer.getLocalization("First Page"),
                        imageClassName: "dxrd-image-preview-first",
                        disabled: ko.pureComputed(function () {
                            return reportPreview.pageIndex() < 1;
                        }),
                        visible: ko.pureComputed(function () {
                            return reportPreview.previewVisible();
                        }),
                        hotKey: { ctrlKey: true, keyCode: "1".charCodeAt(0) },
                        clickAction: function () {
                            if (reportPreview.pageIndex() > 0) {
                                reportPreview.goToPage(0);
                            }
                        }
                    });
                    this.actions.push({
                        text: DevExpress.Designer.getLocalization("Previous Page"),
                        imageClassName: "dxrd-image-preview-prev",
                        disabled: ko.pureComputed(function () {
                            return reportPreview.pageIndex() < 1;
                        }),
                        visible: ko.pureComputed(function () {
                            return reportPreview.previewVisible();
                        }),
                        hotKey: { ctrlKey: true, keyCode: ",".charCodeAt(0) },
                        clickAction: function () {
                            if (reportPreview.pageIndex() >= 1) {
                                reportPreview.goToPage(reportPreview.pageIndex() - 1);
                            }
                        },
                    });
                    var paginationSelectBoxViewModel = {
                        text: "Pagination",
                        imageClassName: "dxrd-image-pager",
                        disabled: ko.observable(false),
                        visible: ko.pureComputed(function () {
                            return reportPreview.previewVisible();
                        }),
                        clickAction: $.noop,
                        selectedItem: ko.pureComputed({
                            read: function () {
                                if (reportPreview.pageIndex() < 0) {
                                    return null;
                                }
                                var items = paginationSelectBoxViewModel.pageItems();
                                return items && items.store && (items.store.length > reportPreview.pageIndex()) && items.store[reportPreview.pageIndex()];
                            },
                            write: function (newValue) {
                                if (!!newValue && (newValue.index || newValue.index === 0)) {
                                    reportPreview.goToPage(newValue.index);
                                }
                            },
                            deferEvaluation: true
                        }),
                        pageItems: ko.pureComputed(function () {
                            var pageCount = reportPreview.pages().length;
                            if (pageCount === 0 || reportPreview.pageIndex.peek() === -1) {
                                return [];
                            }
                            var pagesArray = new Array();
                            for (var i = 0; i < pageCount;) {
                                pagesArray.push({ index: i, text: ++i });
                            }
                            return {
                                store: pagesArray,
                                paginate: pageCount > 200,
                                pageSize: 100
                            };
                        }),
                        currentPage: reportPreview._currentPage,
                        focusOut: function (e) {
                            reportPreview._currentPage.notifySubscribers();
                        },
                        valueChanged: function (e) {
                            var val = parseInt && parseInt(e.value);
                            if (!!val && val-- > 0 && val < reportPreview.pages().length) {
                                reportPreview.goToPage(val);
                            }
                        },
                        templateName: "dxrd-preview-pager"
                    };
                    this.actions.push(paginationSelectBoxViewModel);
                    this.actions.push({
                        text: DevExpress.Designer.getLocalization("Next Page"),
                        imageClassName: "dxrd-image-preview-next",
                        disabled: ko.pureComputed(function () {
                            return reportPreview.pageIndex() < 0 || reportPreview.pageIndex() >= reportPreview.pages().length - 1;
                        }),
                        visible: ko.pureComputed(function () {
                            return reportPreview.previewVisible();
                        }),
                        hotKey: { ctrlKey: true, keyCode: ".".charCodeAt(0) },
                        clickAction: function () {
                            if (reportPreview.pageIndex() < reportPreview.pages().length - 1) {
                                reportPreview.goToPage(reportPreview.pageIndex() + 1);
                            }
                        }
                    });
                    this.actions.push({
                        text: DevExpress.Designer.getLocalization("Last Page"),
                        imageClassName: "dxrd-image-preview-last",
                        disabled: ko.pureComputed(function () {
                            return reportPreview.pageIndex() < 0 || reportPreview.pageIndex() >= reportPreview.pages().length - 1;
                        }),
                        visible: ko.pureComputed(function () {
                            return reportPreview.previewVisible();
                        }),
                        hotKey: { ctrlKey: true, keyCode: "L".charCodeAt(0) },
                        clickAction: function () {
                            if (reportPreview.pageIndex() < reportPreview.pages().length - 1) {
                                reportPreview.goToPage(reportPreview.pages().length - 1);
                            }
                        }
                    });
                    this.actions.push({
                        text: DevExpress.Designer.getLocalization("Toggle Multipage Mode"),
                        imageClassName: ko.pureComputed(function () {
                            return reportPreview.showMultipagePreview() ? "dxrd-image-preview-single-page" : "dxrd-image-preview-multipage";
                        }),
                        disabled: ko.observable(false),
                        visible: ko.pureComputed(function () {
                            return reportPreview.previewVisible();
                        }),
                        hotKey: { ctrlKey: true, keyCode: "M".charCodeAt(0) },
                        clickAction: function () {
                            var zoom = reportPreview._zoom();
                            reportPreview.showMultipagePreview(!reportPreview.showMultipagePreview());
                            reportPreview.zoom(zoom);
                        },
                        hasSeparator: true
                    });
                    this.actions.push({
                        text: DevExpress.Designer.getLocalization("Zoom Out"),
                        imageClassName: "dxrd-image-zoomout",
                        disabled: ko.pureComputed(function () {
                            return reportPreview.isAutoFit();
                        }),
                        visible: true,
                        zoomStep: zoomStep,
                        clickAction: function () {
                            reportPreview.zoom(Math.max(reportPreview.zoom() - zoomStep(), 0.01));
                        },
                        hasSeparator: true
                    });
                    this.actions.push({
                        text: DevExpress.Designer.getLocalization("Zoom 100%"),
                        imageClassName: "dxrd-image-zoom",
                        disabled: ko.observable(false),
                        visible: true,
                        clickAction: function () {
                            reportPreview.zoom(1);
                        },
                        templateName: "dxrd-zoom-autofit-select-template",
                        zoom: reportPreview.zoom,
                        zoomLevels: reportPreview.predefinedZoomLevels,
                        zoomItems: ko.pureComputed(function () {
                            var items = reportPreview.predefinedZoomLevels.slice(0);
                            if (reportPreview.showMultipagePreview() === false && items.indexOf(0) === -1) {
                                items.push(0);
                            }
                            return items;
                        })
                    });
                    this.actions.push({
                        text: DevExpress.Designer.getLocalization("Zoom In"),
                        imageClassName: "dxrd-image-zoomin",
                        disabled: ko.pureComputed(function () {
                            return reportPreview.isAutoFit();
                        }),
                        visible: true,
                        zoomStep: zoomStep,
                        clickAction: function () {
                            reportPreview.zoom(Math.min(reportPreview.zoom() + zoomStep(), 10));
                        }
                    });
                    this.actions.push({
                        text: DevExpress.Designer.getLocalization("Print"),
                        imageClassName: "dxrd-image-print",
                        hasSeparator: true,
                        disabled: printDisabled,
                        visible: true,
                        clickAction: function () {
                            if (printDisabled()) {
                                return;
                            }
                            reportPreview.printDocument();
                        }
                    });
                    this.actions.push({
                        text: DevExpress.Designer.getLocalization("Print Page"),
                        imageClassName: "dxrd-image-print-page",
                        disabled: printDisabled,
                        visible: true,
                        clickAction: function () {
                            if (printDisabled()) {
                                return;
                            }
                            reportPreview.printDocument(reportPreview.pageIndex());
                        }
                    });
                }
                PreviewActions.prototype.getActions = function (context) {
                    return this.actions;
                };
                return PreviewActions;
            })();
            Preview.PreviewActions = PreviewActions;
        })(Preview = Report.Preview || (Report.Preview = {}));
    })(Report = DevExpress.Report || (DevExpress.Report = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Report;
    (function (Report) {
        var Preview;
        (function (Preview) {
            ko.bindingHandlers["toView"] = {
                init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                    var $previewPage = $(element), $container = $previewPage.parent(".dxrd-report-preview-holder"), pageActive = valueAccessor().active, subscription = pageActive.subscribe(function (active) {
                        if (active) {
                            var pageTop = $previewPage.position().top;
                            if (pageTop < 0 && (pageTop + $previewPage.height() < 0) || pageTop > $container.height()) {
                                $container.scrollTop($container.scrollTop() + pageTop);
                            }
                        }
                    });
                    ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                        subscription.dispose();
                    });
                }
            };
            ko.bindingHandlers["lazyImages"] = {
                init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                    var $element = $(element), enabled = valueAccessor().enabled, loadVisibleImages = function () {
                        if (!enabled()) {
                            return;
                        }
                        var visibleArea = $element.height() + 100;
                        ;
                        for (var i = 0; i < element.children.length; i++) {
                            var previewPage = element.children[i], rect = previewPage.getBoundingClientRect(), pageTop = rect.top;
                            if (visibleArea > pageTop && pageTop >= 0 || pageTop < 0 && pageTop + rect.height > -100) {
                                var image = previewPage.children[0].children[1];
                                var $image = $(image);
                                if ($image.attr('imageSrc') && $image.is(':visible') && image.src !== $image.attr('imageSrc')) {
                                    image.src = $image.attr('imageSrc');
                                }
                            }
                        }
                    };
                    if (ko.isObservable(valueAccessor().updateCallback)) {
                        valueAccessor().updateCallback(loadVisibleImages);
                    }
                    var load;
                    var throttledLoad = function () {
                        if (load) {
                            clearTimeout(load);
                        }
                        load = setTimeout(function () {
                            loadVisibleImages();
                            load = undefined;
                        }, 700);
                    };
                    var subscribtion = enabled.subscribe(function (newVal) {
                        if (newVal) {
                            loadVisibleImages();
                        }
                    });
                    $element.on("scroll", throttledLoad);
                    setTimeout(loadVisibleImages, 500);
                    ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                        $element.off("scroll", throttledLoad);
                        subscribtion.dispose();
                    });
                }
            };
            ko.bindingHandlers["brick-selection"] = {
                update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                    var values = valueAccessor(), selectBrick = ko.unwrap(values).selectBrick, toggleBrickSelection = function (el, ctrl) {
                        if (el) {
                            var brick = ko.dataFor(el);
                            if (brick && brick.active) {
                                brick.active(ctrl || !brick.active());
                            }
                        }
                    }, resetBrickSelection = function (el) {
                        if (el) {
                            var brick = ko.dataFor(el);
                            if (brick && brick.active) {
                                brick.active(false);
                            }
                        }
                    }, options = $.extend({ filter: '.dxrd-report-preview-brick-selectable', distance: 5 }, ko.unwrap(values), {
                        selecting: function (event, ui) {
                            toggleBrickSelection(ui.selecting, event.ctrlKey);
                        },
                        start: function (event, ui) {
                            selectBrick && selectBrick("", event.ctrlKey);
                        },
                        unselecting: function (event, ui) {
                            resetBrickSelection(ui.unselecting);
                        }
                    });
                    $(element).selectable(options);
                }
            };
            ko.bindingHandlers["textCopier"] = {
                init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                    var keyDownHandler = function (e) {
                        var value = viewModel.getSelectedContent();
                        if (!value || !(e.ctrlKey || e.metaKey) || $(e.target).is("input:visible,textarea:visible") || window.getSelection && window.getSelection() && window.getSelection().toString() || document["selection"] && document["selection"].createRange().text) {
                            return;
                        }
                        var $clipboardContainer = $("#clipboard-container");
                        $clipboardContainer.empty().show();
                        $("<textarea id='clipboard'></textarea>").val(value).appendTo($clipboardContainer).focus().select();
                    };
                    var keyUpHandler = function (e) {
                        if ($(e.target).is("#clipboard")) {
                            $("#clipboard-container").empty().hide();
                        }
                    };
                    $(document).on("keydown", keyDownHandler);
                    $(document).on("keyup", keyUpHandler);
                    ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                        $(document).off("keydown", keyDownHandler);
                        $(document).off("keyup", keyUpHandler);
                    });
                }
            };
            ko.bindingHandlers["autoFit"] = {
                init: function (element, valueAccessor) {
                    var options = valueAccessor();
                    var subscriptions = [];
                    var updateZoom = function (options) {
                        if (options.isAutoFit() === true) {
                            options.zoom(Math.round(Preview.updatePreviewZoomWithAutoFit(options.width(), options.height()) * 100 - 1) / 100);
                        }
                    };
                    updateZoom(options);
                    var onResize = function () {
                        updateZoom(options);
                    };
                    $(window).bind("resize", onResize);
                    subscriptions.push(options.rightPanelWidth.subscribe(function (newVal) {
                        updateZoom(options);
                    }));
                    subscriptions.push(options.width.subscribe(function (newVal) {
                        updateZoom(options);
                    }));
                    subscriptions.push(options.height.subscribe(function (newVal) {
                        updateZoom(options);
                    }));
                    subscriptions.push(options.isAutoFit.subscribe(function (newVal) {
                        updateZoom(options);
                    }));
                    ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                        $(window).unbind("resize", onResize);
                        subscriptions.forEach(function (subscription) {
                            subscription.dispose();
                        });
                    });
                }
            };
        })(Preview = Report.Preview || (Report.Preview = {}));
    })(Report = DevExpress.Report || (DevExpress.Report = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Report;
    (function (Report) {
        var Preview;
        (function (Preview) {
            var PreviewHandlersHelper = (function () {
                function PreviewHandlersHelper(preview) {
                    this._preview = preview;
                }
                PreviewHandlersHelper.prototype.getDoneStartExportHandler = function (deffered, printable) {
                    var preview = this._preview;
                    return function (response) {
                        try {
                            if (!response.result || response.error) {
                                preview.progressBar.complete();
                                return;
                            }
                            preview._currentOperationId(response.result);
                            var doGetExportStatus = function (operationId) {
                                var promise = preview.getExportStatus(operationId);
                                promise.done(function (result) {
                                    if (result && result.requestAgain) {
                                        preview.progressBar && preview.progressBar.updateProgress(result.progress);
                                        doGetExportStatus(operationId);
                                    }
                                    else {
                                        preview.progressBar.complete();
                                        if (!result.requestAgain && result.completed) {
                                            preview.getExportResult(operationId, printable);
                                        }
                                        if (result.error) {
                                            preview._processError(DevExpress.Designer.getLocalization("Error on retrieving an exporting status: ") + result.error);
                                            return;
                                        }
                                    }
                                });
                            };
                            doGetExportStatus(preview._currentOperationId());
                        }
                        finally {
                            deffered.resolve(true);
                            preview._startBuildOperationId = "";
                        }
                    };
                };
                PreviewHandlersHelper.prototype.getErrorStartExportHandler = function (deffered) {
                    var preview = this._preview;
                    return function (error) {
                        preview.progressBar.complete();
                        if (!error) {
                            debugger;
                        }
                        else {
                            preview._processError(DevExpress.Designer.getLocalization("An error occurred during the export"), error);
                            debugger;
                        }
                    };
                };
                PreviewHandlersHelper.prototype.getDoneExportStatusHandler = function (deffered, operationId) {
                    var preview = this._preview;
                    return function (response) {
                        try {
                            if (!response.result) {
                                deffered.resolve({ requestAgain: false });
                                preview.progressBar.complete();
                                return;
                            }
                            if (response.result.faultMessage) {
                                deffered.resolve({ requestAgain: false, error: response.result.faultMessage });
                                preview._processError(response.result.faultMessage, null);
                                preview.progressBar.complete();
                                return;
                            }
                            preview.progressBar && preview.progressBar.progress() < response.result.progress && preview.progressBar.updateProgress(response.result.progress);
                            deffered.resolve(response.result);
                        }
                        catch (e) {
                            deffered.resolve({ requestAgain: false });
                        }
                    };
                };
                PreviewHandlersHelper.prototype.getErrorExportStatusHandler = function (deffered) {
                    var preview = this._preview;
                    return function (error) {
                        preview.progressBar.complete();
                        deffered.resolve({ requestAgain: false, created: false });
                        preview._processError(DevExpress.Designer.getLocalization("Error obtaining an export status"), error);
                    };
                };
                PreviewHandlersHelper.prototype.getDoneStartBuildHandler = function (deffered) {
                    var preview = this._preview;
                    return function (response) {
                        try {
                            var removeAllEmptyPages = function (all) {
                                all && preview.pages.removeAll();
                                preview.removeEmptyPages();
                            };
                            if (!response.result || !response.result.documentId) {
                                preview.progressBar.complete();
                                removeAllEmptyPages();
                                return;
                            }
                            var stopBuildRequest = preview._stopBuildRequests[preview._startBuildOperationId];
                            var closeDocumentRequest = preview._closeDocumentRequests[preview._startBuildOperationId];
                            if (preview._startBuildOperationId && (stopBuildRequest || closeDocumentRequest)) {
                                if (closeDocumentRequest) {
                                    closeDocumentRequest && preview.closeDocument(response.result.documentId);
                                }
                                else {
                                    stopBuildRequest && preview.stopBuild(response.result.documentId);
                                }
                                preview.progressBar.complete();
                                removeAllEmptyPages();
                                return;
                            }
                            preview._currentDocumentId(response.result.documentId);
                            var doGetBuildStatus = function (documentId) {
                                var promise = preview.getBuildStatus(documentId);
                                promise.done(function (result) {
                                    if (result && result.requestAgain && !preview._stopBuildRequests[documentId] && !preview._closeDocumentRequests[documentId]) {
                                        doGetBuildStatus(documentId);
                                    }
                                    else {
                                        try {
                                            if (result.error || !result.requestAgain && !result.pageCount) {
                                                removeAllEmptyPages(!result.pageCount);
                                                return;
                                            }
                                            if (!result.completed) {
                                                return;
                                            }
                                            preview.getDocumentData(documentId);
                                            preview.pages().forEach(function (page) {
                                                page._setPageImgSrc(documentId, preview._zoom());
                                            });
                                        }
                                        finally {
                                            preview.progressBar.complete();
                                            setTimeout(preview._raiseOnSizeChanged, 1000);
                                        }
                                    }
                                });
                            };
                            doGetBuildStatus(preview._currentDocumentId());
                        }
                        finally {
                            deffered.resolve(true);
                            preview._startBuildOperationId = "";
                        }
                    };
                };
                PreviewHandlersHelper.prototype.getErrorStartBuildHandler = function (deffered) {
                    var preview = this._preview;
                    return function (error) {
                        preview.pageLoading(false);
                        preview.progressBar.complete();
                        deffered.resolve(true);
                        preview._startBuildOperationId = "";
                        preview.removeEmptyPages();
                        preview._processError(DevExpress.Designer.getLocalization("Cannot create a document for the current report"), error, preview._closeDocumentRequests[preview._startBuildOperationId]);
                    };
                };
                PreviewHandlersHelper.prototype.getErrorGetBuildStatusHandler = function (deffered) {
                    var preview = this._preview;
                    return function (error) {
                        deffered.resolve({ requestAgain: false, created: false });
                        preview._processError(DevExpress.Designer.getLocalization("Error obtaining a build status"), error, preview._closeDocumentRequests[preview._currentDocumentId()]);
                    };
                };
                PreviewHandlersHelper.prototype.getDoneGetBuildStatusHandler = function (deffered, documentId) {
                    var preview = this._preview;
                    return function (response) {
                        try {
                            if (!response.result) {
                                deffered.resolve({ requestAgain: false });
                                return;
                            }
                            if (response.result.faultMessage) {
                                deffered.resolve({ requestAgain: false, pageCount: -1, error: response.result.faultMessage });
                                preview._processError(response.result.faultMessage, null, preview._closeDocumentRequests[documentId]);
                                return;
                            }
                            preview.progressBar.progress() < response.result.progress && !preview._stopBuildRequests[documentId] && !preview._closeDocumentRequests[documentId] && preview.progressBar.updateProgress(response.result.progress);
                            var wereNoPagesAndNewOnesExist = preview.pageIndex() === -1 && response.result.pageCount > 0;
                            if (wereNoPagesAndNewOnesExist) {
                                preview.pageIndex(0);
                            }
                            var brickProvider = preview.getPreviewPageBrickProvider(Preview.HandlerUri, documentId);
                            for (var i = 0; i < response.result.pageCount && !preview._stopBuildRequests[documentId] && !preview._closeDocumentRequests[documentId]; i++) {
                                var createNewPage = function (index) {
                                    return new Preview.PreviewPage(index, preview._pageWidth, preview._pageHeight, preview._zoom, preview._currentDocumentId, brickProvider, null, preview.createBrickClickProcessor(index));
                                };
                                if (i < preview.pages().length) {
                                    var page = preview.pages()[i];
                                    if (!page || page.isEmpty) {
                                        page = createNewPage(i);
                                        preview.pages.splice(i, 1, page);
                                        page._setPageImgSrc(documentId, preview._zoom());
                                    }
                                    if (page.pageIndex === -1) {
                                        page.pageIndex = i;
                                        page.brickProvider = brickProvider;
                                        page._setPageImgSrc(documentId, preview._zoom());
                                    }
                                }
                                else {
                                    var newPage = createNewPage(i);
                                    newPage._setPageImgSrc(documentId, preview._zoom());
                                    preview.pages.push(newPage);
                                }
                            }
                            if (wereNoPagesAndNewOnesExist) {
                                var pageIndex = preview.pages().length ? 0 : -1;
                                preview.goToPage(pageIndex, true);
                            }
                            deffered.resolve(response.result);
                        }
                        catch (e) {
                            deffered.resolve({ requestAgain: false });
                        }
                    };
                };
                return PreviewHandlersHelper;
            })();
            Preview.PreviewHandlersHelper = PreviewHandlersHelper;
        })(Preview = Report.Preview || (Report.Preview = {}));
    })(Report = DevExpress.Report || (DevExpress.Report = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Report;
    (function (Report) {
        var Preview;
        (function (Preview) {
            var ProgressViewModel = (function () {
                function ProgressViewModel() {
                    var _this = this;
                    this._progressObservable = ko.observable(0);
                    this._forceInvisible = ko.observable(false);
                    this.stop = ko.observable(function () {
                    });
                    this.inProgress = ko.observable(false);
                    this.cancelText = ko.observable(DevExpress.Designer.getLocalization('Cancel'));
                    this.text = ko.observable('');
                    this.visible = ko.pureComputed({
                        read: function () {
                            if (_this._forceInvisible()) {
                                return false;
                            }
                            return _this.inProgress();
                        },
                        write: function (visibleState) {
                            _this._forceInvisible(!visibleState);
                        }
                    });
                    this.complete = function () {
                        _this.inProgress(false);
                        _this._progressObservable(0);
                        $.isFunction(_this._onComplete) && _this._onComplete();
                        _this._onComplete = null;
                    };
                    this.startProgress = function (onComplete, onStop) {
                        _this.inProgress(true);
                        _this._progressObservable(0);
                        _this._onComplete = onComplete;
                        _this.stop(function () {
                            try {
                                $.isFunction(onStop) && onStop();
                            }
                            finally {
                                _this.complete();
                            }
                        });
                    };
                    this.progress = ko.pureComputed({
                        read: function () {
                            return _this._progressObservable();
                        }
                    });
                }
                ProgressViewModel.prototype.updateProgress = function (progress) {
                    this._progressObservable(progress);
                };
                return ProgressViewModel;
            })();
            Preview.ProgressViewModel = ProgressViewModel;
        })(Preview = Report.Preview || (Report.Preview = {}));
    })(Report = DevExpress.Report || (DevExpress.Report = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Report;
    (function (Report) {
        var Preview;
        (function (Preview) {
            var PreviewRequestWrapper = (function () {
                function PreviewRequestWrapper(handlers) {
                    var _this = this;
                    $.each(handlers || {}, function (name, func) {
                        _this[name] = func;
                    });
                }
                PreviewRequestWrapper.prototype.initialize = function (reportPreview, parametersModel, searchModel) {
                    this._reportPreview = reportPreview;
                    this._parametersModel = parametersModel;
                    this._searchModel = searchModel;
                };
                PreviewRequestWrapper.prototype.findTextRequest = function (text) {
                    return $.post(Preview.HandlerUri, {
                        actionKey: 'findText',
                        arg: encodeURIComponent(JSON.stringify({
                            text: text,
                            documentId: this._reportPreview.documentId,
                            matchCase: this._searchModel.matchCase(),
                            wholeWord: this._searchModel.matchWholeWord(),
                            searchUp: this._searchModel.searchUp()
                        }))
                    });
                };
                PreviewRequestWrapper.prototype.stopBuild = function (id) {
                    $.post(Preview.HandlerUri, { actionKey: 'stopBuild', arg: encodeURIComponent(id) });
                };
                PreviewRequestWrapper.prototype.sendCloseRequest = function (documentId, reportId) {
                    $.post(Preview.HandlerUri, {
                        actionKey: 'close',
                        arg: encodeURIComponent(JSON.stringify({
                            reportId: reportId,
                            documentId: documentId
                        }))
                    });
                };
                PreviewRequestWrapper.prototype.startBuildRequest = function () {
                    return $.post(Preview.HandlerUri, {
                        actionKey: 'startBuild',
                        arg: encodeURIComponent(JSON.stringify({
                            reportId: this._reportPreview.reportId,
                            drillDownKeys: this._reportPreview["_drillDownState"],
                            timeZoneOffset: 0 - new Date().getTimezoneOffset(),
                            parameters: this._parametersModel.serializeParameters()
                        }))
                    });
                };
                PreviewRequestWrapper.prototype.getBuildStatusRequest = function (documentId) {
                    return $.post(Preview.HandlerUri, {
                        actionKey: 'getBuildStatus',
                        arg: encodeURIComponent(JSON.stringify({
                            documentId: documentId,
                            timeOut: Math.max(5000, DevExpress.Report.Preview.TimeOut)
                        }))
                    });
                };
                PreviewRequestWrapper.prototype.getDocumentData = function (documentId) {
                    return $.post(Preview.HandlerUri, {
                        actionKey: 'getDocumentData',
                        arg: encodeURIComponent(documentId)
                    });
                };
                PreviewRequestWrapper.prototype.openReport = function (reportName) {
                    return $.post(Preview.HandlerUri, {
                        actionKey: 'openReport',
                        arg: encodeURIComponent(reportName)
                    });
                };
                PreviewRequestWrapper.prototype.getStartExportOperation = function (arg) {
                    return $.post(Preview.HandlerUri, {
                        actionKey: 'startExport',
                        arg: arg
                    });
                };
                PreviewRequestWrapper.prototype.getExportStatusRequest = function (operationId) {
                    return $.post(Preview.HandlerUri, {
                        actionKey: 'getExportStatus',
                        arg: encodeURIComponent(JSON.stringify({
                            id: operationId,
                            timeOut: Math.max(5000, DevExpress.Report.Preview.TimeOut)
                        }))
                    });
                };
                return PreviewRequestWrapper;
            })();
            Preview.PreviewRequestWrapper = PreviewRequestWrapper;
        })(Preview = Report.Preview || (Report.Preview = {}));
    })(Report = DevExpress.Report || (DevExpress.Report = {}));
})(DevExpress || (DevExpress = {}));
//# sourceMappingURL=web-document-viewer.js.map