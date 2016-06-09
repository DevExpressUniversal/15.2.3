(function() {
    var AttributeValueListConst = {
        Align: ["left", "center", "right" ],
        ButtonType: ["button", "submit", "reset" ],
        CaptionAlign: ["top", "bottom", "left", "right" ],
        CellHAlign: ["left", "center", "right", "justify", "char" ],
        CellVAlign: ["top", "middle", "bottom", "baseline" ],
        Clear: ["left", "all", "right", "none" ],
        Direction: ["ltr", "rtl" ],
        ImageAlign: ["top", "middle", "bottom", "left", "right" ],
        InputType: ["text", "password", "checkbox", "radio", "submit", "reset", "file", "hidden", "image", "button", 
            "color", "date", "datetime", "datetime-local", "email", "month", "number", "range", "search", "tel", "time", "url", "week" ],
        FrameBorder: ["1", "0" ],
        LegendAlign: ["top", "bottom", "left", "right" ],
        Method: ["get", "post" ],
        Scope: ["row", "col", "rowgroup", "colgroup" ],
        Scrolling: ["yes", "no", "auto" ],
        Shape: ["rect", "circle", "poly", "default" ],
        TableAlign: ["left", "center", "right" ],
        TableFrame: ["void", "above", "below", "hsides", "lhs", "rhs", "vsides", "box", "border" ],
        TableRules: ["none", "groups", "rows", "cols", "all" ],
        TextAlign: ["left", "center", "right", "justify" ],
        ULStyle: ["disc", "square", "circle" ],
        ValueType: ["data", "ref", "object" ]
    };

    var A = {
        Abbr: "abbr",
        Accept: "accept",
        AcceptCharset: "accept-charset",
        AccessKey: "accesskey",
        Action: "action",
        Align: "align",
        Alt: "alt",
        Archive: "archive",
        Axis: "axis",
        Background: "background",
        BgColor: "bgcolor",
        Border: "border",
        BorderColor: "bordercolor",
        Char: "char",
        CharOffset: "charoff",
        CellPadding: "cellpadding",
        CellSpacing: "cellspacing",
        Charset: "charset",
        Checked: "checked",
        Cite: "cite",
        Class: "class",
        ClassID: "classid",
        Clear: "clear",
        Code: "code",
        CodeBase: "codebase",
        CodeType: "codetype",
        Color: "color",
        Cols: "cols",
        ColSpan: "colspan",
        Compact: "compact",
        Content: "content",
        Coords: "coords",
        Data: "data",
        DateTime: "datetime",
        Declare: "declare",
        Defer: "defer",
        Dir: "dir",
        Disabled: "disabled",
        EncType: "enctype",
        Face: "face",
        For: "for",
        Frame: "frame",
        FrameBorder: "frameborder",
        Headers: "headers",
        Height: "height",
        Href: "href",
        HrefLang: "hreflang",
        HSpace: "hspace",
        HttpEquiv: "http-equiv",
        ID: "id",
        IsMap: "ismap",
        Label: "label",
        Lang: "lang",
        Language: "language",
        Link: "link",
        LongDesc: "longdesc",
        MarginHeight: "marginheight",
        MarginWidth: "marginwidth",
        MaxLength: "maxlength",
        Media: "media",
        Method: "method",
        Multiple: "multiple",
        Name: "name",
        NoHref: "nohref",
        NoShade: "noshade",
        NoWrap: "nowrap",
        Object: "object",
        OnClick: "onclick",
        OnDblClick: "ondblclick",
        OnMouseDown: "onmousedown",
        OnMouseUp: "onmouseup",
        OnMouseOver: "onmouseover",
        OnMouseMove: "onmousemove",
        OnMouseOut: "onmouseout",
        OnKeyPress: "onkeypress",
        OnKeyDown: "onkeydown",
        OnKeyUp: "onkeyup",
        OnBlur: "onblur",
        OnChange: "onchange",
        OnFocus: "onfocus",
        OnLoad: "onload",
        OnReset: "onreset",
        OnSelect: "onselect",
        OnSubmit: "onsubmit",
        OnUnload: "onunload",
        Profile: "profile",
        Prompt: "prompt",
        ReadOnly: "readonly",
        Rel: "rel",
        Rev: "rev",
        Rows: "rows",
        RowSpan: "rowspan",
        Rules: "rules",
        Scheme: "scheme",
        Scope: "scope",
        Scrolling: "scrolling",
        Selected: "selected",
        Shape: "shape",
        Size: "size",
        Span: "span",
        Src: "src",
        StandBy: "standby",
        Start: "start",
        Style: "style",
        Summary: "summary",
        TabIndex: "tabindex",
        Target: "target",
        Text: "text",
        Title: "title",
        Type: "type",
        UseMap: "usemap",
        Value: "value",
        ValueType: "valuetype",
        VerticalAlign: "valign",
        VLink: "vlink",
        VSpace: "vspace",
        Width: "width",
        XmlLang: "xml:lang",
        
        // HTML5
        ContentEditable: "contenteditable",
        ContextMenu: "contextmenu",
        Draggable: "draggable",
        Dropzone: "dropzone",
        Hidden: "hidden",
        Spellcheck: "spellcheck",
        AutoPlay: "autoplay",
        Controls: "controls",
        Loop: "loop",
        PreLoad: "preload",
        Icon: "icon",
        RadioGroup: "radiogroup",
        Open: "open",
        Keytype: "keytype",
        Optimum: "optimum",
        High: "high",
        Low: "low",
        Max: "max",
        Min: "min",
        SrcLang: "srclang",
        Kind: "kind",
        Default: "default",
        PubDate: "pubdate",
        Poster: "poster",
        Muted: "muted",
        Async: "async",
        Form: "form",
        Challenge: "challenge",

        AutoComplete: "autocomplete",
        AutoFocus: "autofocus",
        FormAction: "formaction",
        FormEncType: "formenctype",
        FormMethod: "formmethod",
        FormNoValidate: "formnovalidate",
        FormTarget: "formtarget",
        List: "list",
        Pattern: "pattern",
        Placeholder: "placeholder",
        Step: "step",
        Required: "required",
        Wrap: "wrap",
        Reversed: "reversed",
            
        OnAfterPrint: "onafterprint",
        OnBeforePrint: "onbeforeprint",
        OnBeforeOnLoad: "onbeforeonload",
        OnError: "onerror",
        OnHasChange: "onhaschange",
        OnMessage: "onmessage",
        OnOffline: "onoffline",
        OnOnline: "ononline",
        OnPageHide: "onpagehide",
        OnPageShow: "onpageshow",
        OnPopState: "onpopstate",
        OnRedo: "onredo",
        OnResize: "onresize",
        OnStorage: "onstorage",
        OnUndo: "onundo",
        OnContextMenu: "oncontextmenu",
        OnFormChange: "onformchange",
        OnFormInput: "onforminput",
        OnInput: "oninput",
        OnInvalid: "oninvalid",
        OnDrag: "ondrag",
        OnDragEnd: "ondragend",
        OnDragEnter: "ondragenter",
        OnDragLeave: "ondragleave",
        OnDragOver: "ondragover",
        OnDragStart: "ondragstart",
        OnDrop: "ondrop",
        OnMouseWheel: "onmousewheel",
        OnScroll: "onscroll",

        OnCanPlay: "oncanplay",
        OnCanPlayThrough: "oncanplaythrough",
        OnDurationChange: "ondurationchange",
        OnEmptied: "onemptied",
        OnEnded: "onended",
        OnLoadeddata: "onloadeddata",
        OnLoadedmetadata: "onloadedmetadata",
        OnLoadstart: "onloadstart",
        OnPause: "onpause",
        OnPlay: "onplay",
        OnPlaying: "onplaying",
        OnProgress: "onprogress",
        OnRatechange: "onratechange",
        OnReadystatechange: "onreadystatechange",
        OnSeeked: "onseeked",
        OnSeeking: "onseeking",
        OnStalled: "onstalled",
        OnSuspend: "onsuspend",
        OnTimeupdate: "ontimeupdate",
        OnVolumechange: "onvolumechange",
        OnWaiting: "onwaiting",

        Property: "property",
        CrossOrigin: "crossorigin",
        Sizes: "sizes",
        Translate: "translate",
        Manifest: "manifest"
    };

    var E = {
        Html: "html",
        Head: "head",
        Title: "title",
        BaseElement: "base",
        Meta: "meta",
        Style: "style",
        Link: "link",
        Body: "body",
        NoFrames: "noframes",
        BaseFont: "basefont",
        Applet: "applet",
        Menu: "menu",
        Dir: "dir",
        IsIndex: "isindex",
        Script: "script",
        IFrame: "iframe",
        Form: "form",
        Label: "label",
        Input: "input",
        Select: "select",
        Option: "option",
        OptGroup: "optgroup",
        TextArea: "textarea",
        Button: "button",
        U: "u",
        S: "s",
        Strike: "strike",
        Font: "font",
        Center: "center",
        I: "i",
        B: "b",
        NoScript: "noscript",
        Div: "div",
        P: "p",
        FieldSet: "fieldset",
        ObjectElement: "object",
        Param: "param",
        Legend: "legend",
        Span: "span",
        A: "a",
        Img: "img",
        Map: "map",
        Area: "area",
        Bdo: "bdo",
        Hr: "hr",
        Br: "br",
        H1: "h1",
        H2: "h2",
        H3: "h3",
        H4: "h4",
        H5: "h5",
        H6: "h6",
        UL: "ul",
        LI: "li",
        OL: "ol",
        DL: "dl",
        DT: "dt",
        DD: "dd",
        Pre: "pre",
        Address: "address",
        BlockQuote: "blockquote",
        Ins: "ins",
        Del: "del",
        Q: "q",
        Em: "em",
        Strong: "strong",
        Dfn: "dfn",
        Code: "code",
        Samp: "samp",
        Kbd: "kbd",
        Var: "var",
        Cite: "cite",
        Abbr: "abbr",
        Acronym: "acronym",
        Sub: "sub",
        Sup: "sup",
        TT: "tt",
        Big: "big",
        Small: "small",
        Table: "table",
        Caption: "caption",
        THead: "thead",
        TFoot: "tfoot",
        TBody: "tbody",
        ColGroup: "colgroup",
        Col: "col",
        TR: "tr",
        TH: "th",
        TD: "td",

        // HTML5
        Article: "article",
        Aside: "aside",
        Audio: "audio",
        Bdi: "bdi",
        Canvas: "canvas",
        Command: "command",
        Datalist: "datalist",
        Details: "details",
        Embed: "embed",
        FigCaption: "figcaption",
        Figure: "figure",
        Footer: "footer",
        Header: "header",
        Hgroup: "hgroup",
        Keygen: "keygen",
        Mark: "mark",
        Meter: "meter",
        Nav: "nav",
        Output: "output",
        Progress: "progress",
        Rp: "rp",
        Rt: "rt",
        Ruby: "ruby",
        Section: "section",
        Source: "source",
        Summary: "summary",
        Time: "time",
        Track: "track",
        Video: "video",
        Wbr: "wbr"
    };

    var GroupElements = {
        SpecialExtra: function() { return [E.ObjectElement, E.Applet, E.Img, E.Map, E.IFrame, E.Canvas, E.Command, E.Figure]; },
        SpecialBasic: function() { return [E.Br, E.Span, E.Bdo]; },
        Special: function() { 
            var res = this.SpecialBasic();
            return res.concat(this.SpecialExtra());
        },
        FontStyleExtra: function() { return [E.Big, E.Small, E.Font, E.BaseFont]; },
        FontStyleBasic: function() { return [E.TT, E.I, E.B, E.U, E.S, E.Strike]; },
        FontStyle: function() { 
            var res = this.FontStyleBasic();
            return res.concat(this.FontStyleExtra());
        },
        PhraseExtra: function() { return [E.Sub, E.Sup]; },
        PhraseBasic: function() { return [E.Em, E.Strong, E.Dfn, E.Code, E.Q, E.Samp, E.Kbd, E.Var, E.Cite, E.Abbr, E.Acronym]; },
        Phrase: function() {
            var res = this.PhraseBasic();
            return res.concat(this.PhraseExtra());
        },
        InlineForms: function() { return [E.Input, E.Select, E.TextArea, E.Label, E.Button, E.Datalist, E.Keygen, E.Meter, E.Output]; },
        MiscInline: function() { return [E.Ins, E.Del, E.Script, E.Progress]; },
        Misc: function() {
            var res = [E.NoScript];
            return res.concat(this.MiscInline());
        },
        Inline: function() {
            var res = [E.A, E.Bdi, E.Mark, E.Wbr, E.Track, E.Time];
            res = res.concat(this.Special());
            res = res.concat(this.FontStyle());
            res = res.concat(this.Phrase());
            return res.concat(this.InlineForms());
        },
        InlineAndTextLevel: function() { 
            var res = this.Inline();
            return res.concat(this.MiscInline());
        },
        Heading: function() { return [E.H1, E.H2, E.H3, E.H4, E.H5, E.H6]; },
        Lists: function() { return [E.UL, E.OL, E.DL, E.Menu, E.Dir]; },
        BlockText: function() { return [E.Pre, E.Hr, E.BlockQuote, E.Address, E.Center, E.NoFrames]; },
        Block: function() { 
            var res = [E.P, E.Div, E.Section, E.Article, E.Aside, E.IsIndex, E.FieldSet, E.Table, E.Footer, E.Header, E.Hgroup, E.Nav];
            res = res.concat(this.Heading());
            res = res.concat(this.Lists());
            return res.concat(this.BlockText());
        },
        Flow: function() { 
            var res = [E.Form, E.Video, E.Audio, E.Embed, E.Details];
            res = res.concat(this.Block());
            res = res.concat(this.Inline());
            return res.concat(this.Misc());
        },
        AContent: function() { 
            var res = this.Special();
            res = res.concat(this.FontStyle());
            res = res.concat(this.Phrase());
            res = res.concat(this.InlineForms());
            return res.concat(this.MiscInline());
        },
        PreContent: function() { 
            var res = [E.A];
            res = res.concat(this.SpecialBasic());
            res = res.concat(this.FontStyleBasic());
            res = res.concat(this.PhraseBasic());
            res = res.concat(this.InlineForms());
            return res.concat(this.MiscInline());
        },
        FormContent: function() { 
            var res = this.Block();
            res = res.concat(this.Inline());
            return res.concat(this.Misc());
        },
        ButtonContent: function() { 
            var res = [E.P, E.Div, E.Table, E.Br, E.Span, E.Bdo, E.ObjectElement, E.Applet, E.Img, E.Map];
            res = res.concat(this.Heading());
            res = res.concat(this.Lists());
            res = res.concat(this.BlockText());
            res = res.concat(this.FontStyle());
            res = res.concat(this.Phrase());
            return res.concat(this.Misc());
        },
        HeadMisc: function() { return [E.Script, E.Style, E.Meta, E.Link, E.ObjectElement, E.IsIndex]; },
        Media: function() {
            var res = [E.Track, E.Source];
            return res.concat(this.InlineAndTextLevel());
        }
    };

    var GroupAttributes = {
        GlobalAttributes: function() { return [
            this.createAttr(A.AccessKey, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.Class, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.ContentEditable, ASPx.HtmlEditorClasses.DocumentType.Both, ["true", "false"]),
            this.createAttr(A.Dir, ASPx.HtmlEditorClasses.DocumentType.Both, AttributeValueListConst.Direction),
            this.createAttr(A.Hidden, ASPx.HtmlEditorClasses.DocumentType.Both, ["true", "false"]),
            this.createAttr(A.ID, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.Lang, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.Spellcheck, ASPx.HtmlEditorClasses.DocumentType.Both, ["true", "false"]),
            this.createAttr(A.Style, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.TabIndex, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.Title, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.Translate, ASPx.HtmlEditorClasses.DocumentType.Both)
        ]},
        CoreAttrs: function() { return [
            this.createAttr(A.ID, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.Class, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.Style, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.Title, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.AccessKey, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.TabIndex, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.ContextMenu, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createAttr(A.Draggable, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["copy", "move", "link"]),
            this.createAttr(A.Dropzone, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["true", "false", "auto"]),
            this.createAttr(A.Hidden, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["true", "false"]),
            this.createAttr(A.Spellcheck, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["true", "false"]),
            this.createAttr(A.ContentEditable, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["true", "false"])
        ]},
        DataCellHAlign: function() { return [
            this.createAttr(A.Align, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.CellHAlign),
            this.createAttr(A.Char, ASPx.HtmlEditorClasses.DocumentType.XHTML),
            this.createAttr(A.CharOffset, ASPx.HtmlEditorClasses.DocumentType.XHTML)
        ]},
        HeaderCellHAlign: function() { return [
            this.createAttr(A.Align, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.CellHAlign),
            this.createAttr(A.Char, ASPx.HtmlEditorClasses.DocumentType.XHTML),
            this.createAttr(A.CharOffset, ASPx.HtmlEditorClasses.DocumentType.XHTML)
        ]},
        I18n: function() { return [
            this.createAttr(A.Lang, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createAttr(A.XmlLang, ASPx.HtmlEditorClasses.DocumentType.XHTML),
            this.createAttr(A.Dir, ASPx.HtmlEditorClasses.DocumentType.Both, AttributeValueListConst.Direction)
        ]},
        Events: function() { return [
            this.createEventAttr(A.OnClick, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnDblClick, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnDrag, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnDragEnd, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnDragEnter, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnDragLeave, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnDragOver, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnDragStart, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnDrop, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnMouseDown, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnMouseUp, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnMouseOver, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnMouseMove, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnMouseOut, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnMouseWheel, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnScroll, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnKeyPress, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnKeyDown, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnKeyUp, ASPx.HtmlEditorClasses.DocumentType.Both)
        ]},
        MediaEvents: function() { return [
            this.createEventAttr(A.OnCanPlay, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnCanPlayThrough, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnDurationChange, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnEmptied, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnEnded, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnError, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnLoadeddata, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnLoadedmetadata, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnLoadstart, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnPause, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnPlay, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnPlaying, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnProgress, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnRatechange, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnReadystatechange, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnSeeked, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnSeeking, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnStalled, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnSuspend, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnTimeupdate, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnVolumechange, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnWaiting, ASPx.HtmlEditorClasses.DocumentType.HTML5)
        ]},
        InputEvents: function() { return [
            this.createEventAttr(A.OnBlur, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnChange, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnContextMenu, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnFocus, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnFormChange, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnFormInput, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnInput, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnInvalid, ASPx.HtmlEditorClasses.DocumentType.HTML5),
            this.createEventAttr(A.OnReset, ASPx.HtmlEditorClasses.DocumentType.XHTML),
            this.createEventAttr(A.OnSelect, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnSubmit, ASPx.HtmlEditorClasses.DocumentType.Both)
        ]},
        Focus: function() { return [
            this.createEventAttr(A.OnFocus, ASPx.HtmlEditorClasses.DocumentType.Both),
            this.createEventAttr(A.OnBlur, ASPx.HtmlEditorClasses.DocumentType.Both)
        ]},
        FormEvents: function() { 
            var res = this.Focus();
            return res.concat(this.InputEvents());
        },
        Attrs: function() {
            var res = this.CoreAttrs();
            res = res.concat(this.I18n());
            return res.concat(this.Events());
        },
        AttrsAndTextAlign: function() {
            var res = [this.createAttr(A.Align, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.TableAlign)];
            return res.concat(this.Attrs());
        },
        Cell: function() {
            var res = [
               this.createAttr(A.VerticalAlign, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.CellVAlign),
               this.createAttr(A.Abbr, ASPx.HtmlEditorClasses.DocumentType.XHTML),
               this.createAttr(A.Axis, ASPx.HtmlEditorClasses.DocumentType.XHTML),
               this.createAttr(A.Headers, ASPx.HtmlEditorClasses.DocumentType.Both),
               this.createAttr(A.Scope, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.Scope),
               this.createAttr(A.RowSpan, ASPx.HtmlEditorClasses.DocumentType.Both),
               this.createAttr(A.ColSpan, ASPx.HtmlEditorClasses.DocumentType.Both),
               this.createAttr(A.NoWrap, ASPx.HtmlEditorClasses.DocumentType.XHTML),
               this.createAttr(A.BgColor, ASPx.HtmlEditorClasses.DocumentType.XHTML),
               this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.XHTML),
               this.createAttr(A.Height, ASPx.HtmlEditorClasses.DocumentType.XHTML)
            ];
            res = res.concat(this.Attrs());
            return res.concat(this.DataCellHAlign());
        },
        Col: function() {
            var res = [
               this.createAttr(A.VerticalAlign, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.CellVAlign),
               this.createAttr(A.Span, ASPx.HtmlEditorClasses.DocumentType.Both),
               this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.XHTML)
            ];
            res = res.concat(this.Attrs());
            return res.concat(this.DataCellHAlign());
        },
        createAttr: function(name, documentType, valueList, iconType) {
            return { 
                name: name,
                documentType: documentType,
                valueList: valueList ? valueList : [],
                iconType: iconType ? iconType : ASPx.HtmlEditorClasses.IconType.Field
            };
        },
        createEventAttr: function(name, documentType, valueList, iconType) {
            return this.createAttr(name, documentType, null, ASPx.HtmlEditorClasses.IconType.Event);
        }
    };

    var DtdElementDeclaration = ASPx.CreateClass(null, {
	    constructor: function(documentType) {
            this.documentType = documentType;
            this.elements = {};
            this.initElements();
            if(this.documentType != ASPx.HtmlEditorClasses.DocumentType.Both)
                this.filterContentElementsAndAttributes();
        },
        initElements: function() {
            this.addElement(E.Video, ASPx.HtmlEditorClasses.DocumentType.HTML5, 
                function() {
                    var res = [
                        this.createAttr(A.AutoPlay, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["autoplay"]),
                        this.createAttr(A.Controls, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["controls"]),
                        this.createAttr(A.Loop, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["loop"]),
                        this.createAttr(A.PreLoad, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["auto", "metadata", "none"]),
                        this.createAttr(A.Muted, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["muted"]),
                        this.createAttr(A.Src, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Height, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Poster, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    res = res.concat(GroupAttributes.Attrs());
                    return res.concat(GroupAttributes.MediaEvents());
                }.aspxBind(this)(), 
                [E.Source, E.Track]
            ),

            this.addElement(E.Audio, ASPx.HtmlEditorClasses.DocumentType.HTML5,
                function() {
                    var res = [
                        this.createAttr(A.AutoPlay, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["autoplay"]),
                        this.createAttr(A.Controls, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["controls"]),
                        this.createAttr(A.Loop, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["loop"]),
                        this.createAttr(A.PreLoad, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["auto", "metadata", "none"]),
                        this.createAttr(A.Src, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    res = res.concat(GroupAttributes.Attrs());
                    return res.concat(GroupAttributes.MediaEvents());
                }.aspxBind(this)(), 
                [E.Source, E.Track]
            ),

            this.addElement(E.Source, ASPx.HtmlEditorClasses.DocumentType.HTML5,
                function() {
                    var res = [
                        this.createAttr(A.Media, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Src, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.Track, ASPx.HtmlEditorClasses.DocumentType.HTML5,
                function() {
                    var res = [
                        this.createAttr(A.Default, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["default"]),
                        this.createAttr(A.Kind, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["captions", "chapters", "descriptions", "metadata", "subtitles"]),
                        this.createAttr(A.Label, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Src, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.SrcLang, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.Html, ASPx.HtmlEditorClasses.DocumentType.HTML5,
                [this.createAttr(A.Manifest, ASPx.HtmlEditorClasses.DocumentType.HTML5)], 
                [E.Head, E.Body]
            ),

            this.addElement(E.Head, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.GlobalAttributes(),
                function() {
                    var res = [E.Title, E.BaseElement, E.NoScript];
                    return res.concat(GroupElements.HeadMisc());
                }()
            ),

            this.addElement(E.Title, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.GlobalAttributes()),

            this.addElement(E.BaseElement, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                         this.createAttr(A.Href, ASPx.HtmlEditorClasses.DocumentType.Both),
                         this.createAttr(A.Target, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                    return res.concat(GroupAttributes.GlobalAttributes());
                }.aspxBind(this)()
            ),

            this.addElement(E.Meta, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                         this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.Both, ["application-name", "author", "description", "generator", "keywords"]),
                         this.createAttr(A.HttpEquiv, ASPx.HtmlEditorClasses.DocumentType.Both, ["content-type", "default-style", "refresh"]),
                         this.createAttr(A.Charset, ASPx.HtmlEditorClasses.DocumentType.Both),
                         this.createAttr(A.Content, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    return res.concat(GroupAttributes.GlobalAttributes());
                }.aspxBind(this)()
            ),

            this.addElement(E.Style, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                        this.createAttr(A.Media, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.Both, ["text/css"]),
                        this.createAttr(A.Title, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                    return res.concat(GroupAttributes.GlobalAttributes());
                }.aspxBind(this)()
            ),

            this.addElement(E.Link, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                        this.createAttr(A.Href, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.CrossOrigin, ASPx.HtmlEditorClasses.DocumentType.Both, ["anonymous", "use-credentials"]),
                        this.createAttr(A.Rel, ASPx.HtmlEditorClasses.DocumentType.Both, ["alternate", "archives", "author", "bookmark", "external", "first", "help", "icon", "last", "license", "next", "nofollow", "noreferrer", "pingback", "prefetch", "prev", "search", "sidebar", "stylesheet", "tag", "up"]),
                        this.createAttr(A.Media, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.HrefLang, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Sizes, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Title, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ]
                    return res.concat(GroupAttributes.GlobalAttributes());
                }.aspxBind(this)()
            ),

            this.addElement(E.Body, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.GlobalAttributes(), GroupElements.Flow()),

            this.addElement(E.Script, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    return [
                        this.createAttr(A.ID, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.Both, ["text/javascript"]),
                        this.createAttr(A.Charset, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Language, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Src, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Defer, ASPx.HtmlEditorClasses.DocumentType.Both, ["defer"]),
                        this.createAttr(A.Async, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["async"])
                    ];
                }.aspxBind(this)()
            ),

            this.addElement(E.IFrame, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.LongDesc, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Src, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.FrameBorder, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.FrameBorder),
                        this.createAttr(A.MarginWidth, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.MarginHeight, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Scrolling, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.Scrolling),
                        this.createAttr(A.Align, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.ImageAlign),
                        this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Height, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                    return res.concat(GroupAttributes.CoreAttrs());
                }.aspxBind(this)(), 
                GroupElements.Flow()
            ),

            this.addElement(E.Form, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.Action, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Method, ASPx.HtmlEditorClasses.DocumentType.Both, AttributeValueListConst.Method),
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.EncType, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Accept, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.AcceptCharset, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Target, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                    res = res.concat(GroupAttributes.FormEvents());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.FormContent()
            ),

            this.addElement(E.Label, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.For, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Form, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    res = res.concat(GroupAttributes.Focus());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(),
                GroupElements.InlineAndTextLevel()
            ),

            this.addElement(E.Input, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.Both, AttributeValueListConst.InputType),
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Value, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Checked, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Disabled, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.ReadOnly, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Size, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.MaxLength, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Src, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Alt, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.UseMap, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Accept, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Align, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.ImageAlign),
                        this.createAttr(A.AutoComplete, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["on", "off"]),
                        this.createAttr(A.AutoFocus, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Form, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.FormAction, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.FormEncType, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.FormMethod, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["get", "post"]),
                        this.createAttr(A.FormNoValidate, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.FormTarget, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Height, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.List, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Max, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Min, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Multiple, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Pattern, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Placeholder, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Required, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Step, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    res = res.concat(GroupAttributes.FormEvents());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.Select, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Size, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Multiple, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Disabled, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                    res = res.concat(GroupAttributes.FormEvents());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                [E.OptGroup, E.Option]
            ),

            this.addElement(E.Option, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                        this.createAttr(A.Selected, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Disabled, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Label, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Value, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                    res = res.concat(GroupAttributes.FormEvents());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.OptGroup, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                        this.createAttr(A.Disabled, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Label, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.TextArea, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Rows, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Cols, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Disabled, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.ReadOnly, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.AutoFocus, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Form, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.MaxLength, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Placeholder, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Required, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Wrap, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["hard", "soft"])
                    ];
                    res = res.concat(GroupAttributes.FormEvents());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.Datalist, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), [E.Option]),

            this.addElement(E.Button, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                     var res = [
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Value, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.Both, AttributeValueListConst.ButtonType),
                        this.createAttr(A.Disabled, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.AutoFocus, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Form, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.FormAction, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.FormEncType, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.FormMethod, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.FormNoValidate, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.FormTarget, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    res = res.concat(GroupAttributes.FormEvents());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.ButtonContent()
            ),

            this.addElement(E.Keygen, ASPx.HtmlEditorClasses.DocumentType.HTML5,
                function() {
                    var res = [
                        this.createAttr(A.AutoFocus, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Challenge, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Disabled, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Form, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Keytype, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["rsa", "dsa", "ec"]),
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    res = res.concat(GroupAttributes.FormEvents());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.Output, ASPx.HtmlEditorClasses.DocumentType.HTML5, 
                function() {
                    var res = [
                        this.createAttr(A.For, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Form, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    res = res.concat(GroupAttributes.FormEvents());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.Flow()
            ),

            this.addElement(E.Progress, ASPx.HtmlEditorClasses.DocumentType.HTML5, 
                function() {
                    var res = [
                        this.createAttr(A.Max, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Value, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.Flow()
            ),

            this.addElement(E.Table, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.Summary, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Height, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Border, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.BorderColor, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Frame, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.TableFrame),
                        this.createAttr(A.Rules, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.TableRules),
                        this.createAttr(A.CellPadding, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.CellSpacing, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Align, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.TableAlign),
                        this.createAttr(A.BgColor, ASPx.HtmlEditorClasses.DocumentType.XHTML)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                [E.Caption, E.THead, E.TFoot, E.TBody, E.Col, E.ColGroup, E.TR]
            ),

            this.addElement(E.U, ASPx.HtmlEditorClasses.DocumentType.XHTML, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.S, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Strike, ASPx.HtmlEditorClasses.DocumentType.XHTML, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),

            this.addElement(E.Font, ASPx.HtmlEditorClasses.DocumentType.XHTML, 
                function() {
                    var res = [
                        this.createAttr(A.Size, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Color, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Face, ASPx.HtmlEditorClasses.DocumentType.XHTML)
                    ];
                    res = res.concat(GroupAttributes.CoreAttrs());
                    return res.concat(GroupAttributes.I18n());
                }.aspxBind(this)(), 
                GroupElements.InlineAndTextLevel()
            ),

            this.addElement(E.Center, ASPx.HtmlEditorClasses.DocumentType.XHTML, GroupAttributes.Attrs(), GroupElements.Flow()),
            this.addElement(E.I, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.B, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),

            this.addElement(E.Meter, ASPx.HtmlEditorClasses.DocumentType.HTML5, 
                function() {
                    var res = [
                        this.createAttr(A.Form, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.High, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Low, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Max, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Min, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Optimum, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Value, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.Flow()
            ),

            this.addElement(E.NoScript, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.Flow()),
            this.addElement(E.Div, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.AttrsAndTextAlign(), GroupElements.Flow()),
            this.addElement(E.P, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.AttrsAndTextAlign(), GroupElements.InlineAndTextLevel()),

            this.addElement(E.FieldSet, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), 
                function() {
                    var res = [E.Legend, E.Form];
                    res = res.concat(GroupElements.Block());
                    res = res.concat(GroupElements.Inline());
                    return res.concat(GroupElements.Misc());
                }.aspxBind(this)()
            ),

            this.addElement(E.ObjectElement, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.Declare, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.ClassID, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.CodeBase, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Data, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.CodeType, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Archive, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.StandBy, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Height, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.UseMap, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Align, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.ImageAlign),
                        this.createAttr(A.Border, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.HSpace, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.VSpace, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Form, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(),
                function() {
                    var res = [E.Param, E.Form, E.Embed];
                    res = res.concat(GroupElements.Block());
                    res = res.concat(GroupElements.Inline());
                    return res.concat(GroupElements.Misc());
                }()
            ),

            this.addElement(E.Param, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Value, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.ValueType, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.ValueType),
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.XHTML)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.Legend, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [this.createAttr(A.Align, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.LegendAlign)];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(),
                GroupElements.InlineAndTextLevel()
            ),

            this.addElement(E.Span, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),

            this.addElement(E.A, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.Charset, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Href, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.HrefLang, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Rel, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Rev, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Shape, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.Shape),
                        this.createAttr(A.Coords, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Target, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Media, ASPx.HtmlEditorClasses.DocumentType.HTML5) 
                    ];
                    res = res.concat(GroupAttributes.Focus());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.AContent()
            ),

            this.addElement(E.Img, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                        this.createAttr(A.Src, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Alt, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.LongDesc, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Height, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.Both ),
                        this.createAttr(A.UseMap, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.IsMap, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Align, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.ImageAlign),
                        this.createAttr(A.Border, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.HSpace, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.VSpace, ASPx.HtmlEditorClasses.DocumentType.XHTML)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.Map, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.ID, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Name, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(),
                function() {
                    var res = [E.Form, E.Area];
                    res = res.concat(GroupElements.Block());
                    return res.concat(GroupElements.Misc());
                }()
            ),

            this.addElement(E.Area, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                        this.createAttr(A.Shape, ASPx.HtmlEditorClasses.DocumentType.Both, AttributeValueListConst.Shape),
                        this.createAttr(A.Coords, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Href, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.NoHref, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Alt, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Target, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                    res = res.concat(GroupAttributes.Focus());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.Bdo, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.Lang, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.XmlLang, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Dir, ASPx.HtmlEditorClasses.DocumentType.Both, AttributeValueListConst.Direction)
                    ];
                    res = res.concat(GroupAttributes.CoreAttrs());
                    return res.concat(GroupAttributes.Events());
                }.aspxBind(this)(), 
                GroupElements.InlineAndTextLevel()
            ),

            this.addElement(E.Hr, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [
                        this.createAttr(A.Align, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.Align),
                        this.createAttr(A.NoShade, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Size, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.XHTML)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.Br, ASPx.HtmlEditorClasses.DocumentType.Both,
                function() {
                    var res = [this.createAttr(A.Clear, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.Clear)];
                    return res.concat(GroupAttributes.CoreAttrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.H1, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.AttrsAndTextAlign(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.H2, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.AttrsAndTextAlign(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.H3, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.AttrsAndTextAlign(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.H4, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.AttrsAndTextAlign(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.H5, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.AttrsAndTextAlign(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.H6, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.AttrsAndTextAlign(), GroupElements.InlineAndTextLevel()),

            this.addElement(E.UL, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.ULStyle),
                        this.createAttr(A.Compact, ASPx.HtmlEditorClasses.DocumentType.XHTML)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                [E.LI]
            ),

            this.addElement(E.OL, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Compact, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Start, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.Reversed, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                [E.LI]
            ),

            this.addElement(E.LI, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.XHTML),
                        this.createAttr(A.Value, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.Flow()
            ),

            this.addElement(E.DL, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [this.createAttr(A.Compact, ASPx.HtmlEditorClasses.DocumentType.XHTML)];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                [E.DT, E.DD]
            ),

            this.addElement(E.DT, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.DD, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.Flow()),

            this.addElement(E.Pre, ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.XHTML)];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.PreContent()
            ),

            this.addElement(E.Address,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), 
                function() {
                    var res = [E.P];
                    res = res.concat(GroupElements.Inline());
                    return res.concat(GroupElements.MiscInline());
                }()
            ),

            this.addElement(E.BlockQuote,ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {   
                    var res = [this.createAttr(A.Cite, ASPx.HtmlEditorClasses.DocumentType.Both)];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.Flow()
            ),

            this.addElement(E.Q,ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [this.createAttr(A.Cite, ASPx.HtmlEditorClasses.DocumentType.Both)];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.InlineAndTextLevel()
            ),

            this.addElement(E.Ins,ASPx.HtmlEditorClasses.DocumentType.Both, 
               function() {
                    var res = [
                        this.createAttr(A.Cite, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.DateTime, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                   return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.Flow()
            ),
            this.addElement(E.Del,ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.Cite, ASPx.HtmlEditorClasses.DocumentType.Both),
                        this.createAttr(A.DateTime, ASPx.HtmlEditorClasses.DocumentType.Both)
                    ];
                    return res.concat(GroupAttributes.Attrs());
               }.aspxBind(this)(), 
               GroupElements.Flow()
            ),

            this.addElement(E.Em,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Strong,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Dfn,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Code,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Samp,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Kbd,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Var,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Cite,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Abbr,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Acronym, ASPx.HtmlEditorClasses.DocumentType.XHTML, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Sub,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Sup,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.TT, ASPx.HtmlEditorClasses.DocumentType.XHTML, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Big, ASPx.HtmlEditorClasses.DocumentType.XHTML, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Small,ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),

            this.addElement(E.Caption,ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [this.createAttr(A.Align, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.CaptionAlign)];
                    return res.concat(GroupAttributes.Attrs());
               }.aspxBind(this)(), 
               GroupElements.InlineAndTextLevel()
            ),

            this.addElement(E.THead,ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [this.createAttr(A.VerticalAlign, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.CellVAlign)];
                    res = res.concat(GroupAttributes.HeaderCellHAlign());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                [E.TR]
            ),

            this.addElement(E.TFoot,ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [this.createAttr(A.VerticalAlign, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.CellVAlign)];
                    res = res.concat(GroupAttributes.HeaderCellHAlign());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                [E.TR]
            ),

            this.addElement(E.TBody,ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [this.createAttr(A.VerticalAlign, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.CellVAlign)];
                    res = res.concat(GroupAttributes.DataCellHAlign());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                [E.TR]
            ),

            this.addElement(E.TR,ASPx.HtmlEditorClasses.DocumentType.Both, 
                function() {
                    var res = [
                        this.createAttr(A.VerticalAlign, ASPx.HtmlEditorClasses.DocumentType.XHTML, AttributeValueListConst.CellVAlign),
                        this.createAttr(A.BgColor, ASPx.HtmlEditorClasses.DocumentType.XHTML)
                    ];
                    res = res.concat(GroupAttributes.DataCellHAlign());
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                [E.TD, E.TH]
            ),

            this.addElement(E.TH, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Cell(), GroupElements.Flow()),
            this.addElement(E.TD, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Cell(), GroupElements.Flow()),
            this.addElement(E.ColGroup, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Col(), [E.Col]),
            this.addElement(E.Col, ASPx.HtmlEditorClasses.DocumentType.Both, GroupAttributes.Col()),

            /* HTML5 */
            this.addElement(E.Article, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), GroupElements.Flow()),
            this.addElement(E.Aside, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), GroupElements.Flow()),

            this.addElement(E.Canvas, ASPx.HtmlEditorClasses.DocumentType.HTML5, 
                function() {
                    var res = [
                        this.createAttr(A.Height, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.Flow()
            ),

            this.addElement(E.Command, ASPx.HtmlEditorClasses.DocumentType.HTML5, 
                function() {
                    var res = [
                        this.createAttr(A.Checked, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Disabled, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Icon, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Label, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.RadioGroup, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.HTML5, ["checkbox", "command", "radio"])
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.Details, ASPx.HtmlEditorClasses.DocumentType.HTML5, 
                function() {
                    var res = [this.createAttr(A.Open, ASPx.HtmlEditorClasses.DocumentType.HTML5)];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                function() {
                    var res = [E.Summary];
                    return res.concat(GroupElements.Flow());
                }()
            ),

            this.addElement(E.Embed, ASPx.HtmlEditorClasses.DocumentType.HTML5,
                function() {
                    var res = [
                        this.createAttr(A.Height, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Width, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Type, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.Src, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)()
            ),

            this.addElement(E.Figure, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), 
                function() {
                    var res = [E.FigCaption];
                    return res.concat(GroupElements.Flow());
                }()
            ),

            this.addElement(E.Time, ASPx.HtmlEditorClasses.DocumentType.HTML5, 
                function() {
                    var res = [
                        this.createAttr(A.DateTime, ASPx.HtmlEditorClasses.DocumentType.HTML5),
                        this.createAttr(A.PubDate, ASPx.HtmlEditorClasses.DocumentType.HTML5)
                    ];
                    return res.concat(GroupAttributes.Attrs());
                }.aspxBind(this)(), 
                GroupElements.InlineAndTextLevel()
            ),

            this.addElement(E.FigCaption, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), GroupElements.Flow()),
            this.addElement(E.Footer, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), GroupElements.Flow()),
            this.addElement(E.Header, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), GroupElements.Flow()),
            this.addElement(E.Hgroup, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), [E.H1, E.H2, E.H3, E.H4, E.H5, E.H6]),
            this.addElement(E.Mark, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Nav, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), GroupElements.Flow()),
            this.addElement(E.Rp, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Rt, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), GroupElements.InlineAndTextLevel()),
            this.addElement(E.Ruby, ASPx.HtmlEditorClasses.DocumentType.HTML5, [], GroupElements.Flow()),
            this.addElement(E.Section, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), GroupElements.Flow()),
            this.addElement(E.Summary, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs(), GroupElements.Flow()),
            this.addElement(E.Wbr, ASPx.HtmlEditorClasses.DocumentType.HTML5, GroupAttributes.Attrs())
        },

        addElement: function(elementName, documentType, attrList, contentElements) {
            this.elements[elementName] = { 
                elementName: elementName,
                documentType: documentType,
                attributes: attrList,
                contentElements: contentElements ? contentElements : []
            };
        },
        filterContentElementsAndAttributes: function() {
            for(var elementName in this.elements) { 
                if (this.elements.hasOwnProperty(elementName)) {
                    var element = this.elements[elementName];
                    element.attributes = this.filterAttributes(element.attributes);
                    element.contentElements = this.filterContentElements(element.contentElements);
                }
            }
        },
        filterContentElements: function(elementList) {
            var result = [];
            for(var i = 0, elementName; elementName = elementList[i]; i++) {
                if(this.elements.hasOwnProperty(elementName)) {
                    var element = this.elements[elementName];
                    if(this.documentType == element.documentType || element.documentType == ASPx.HtmlEditorClasses.DocumentType.Both)
                        result.push(elementName);
                }
            }
            return result;
        },
        filterAttributes: function(attrList) {
            var result = [];
            for(var i = 0, attr; attr = attrList[i]; i++) {
                if(this.documentType == attr.documentType || attr.documentType == ASPx.HtmlEditorClasses.DocumentType.Both)
                    result.push(attr);
            }
            return result;
        },
        createAttr: function(name, documentType, valueList, iconType) {
            return { 
                name: name,
                documentType: documentType,
                valueList: valueList ? valueList : [],
                iconType: iconType ? iconType : ASPx.HtmlEditorClasses.IconType.Field
            };
        },
        createEventAttr: function(name, documentType, valueList, iconType) {
            return this.createAttr(name, documentType, null, ASPx.HtmlEditorClasses.IconType.Event);
        },
        getContentElements: function(elementName) {
            var element = this.elements[elementName];
            return element ? element.contentElements : [];
        },
        getAttrListByElementName: function(elementName) {
            var element = this.elements[elementName];
            return element ? element.attributes : [];
        },
        getAttr: function(elementName, attrName) {
            var attrList = this.getAttrListByElementName(elementName);
            for(var i = 0, attr; attr = attrList[i]; i++) {
                if(attr.name == attrName)
                    return attr;
            }
            return null;
        }
    });

    ASPx.HtmlEditorClasses.Utils.DtdElementDeclaration = DtdElementDeclaration;
})();