module __aspxRichEdit {
    export enum JSONParagraphFormattingProperty {
        Alignment = 0,
        FirstLineIndent = 1,
        FirstLineIndentType = 2,
        LeftIndent = 3,
        LineSpacing = 4,
        LineSpacingType = 5,
        RightIndent = 6,
        SpacingBefore = 7,
        SpacingAfter = 8,
        SuppressHyphenation = 9,
        SuppressLineNumbers = 10,
        ContextualSpacing = 11,
        PageBreakBefore = 12,
        BeforeAutoSpacing = 13,
        AfterAutoSpacing = 14,
        KeepWithNext = 15,
        KeepLinesTogether = 16,
        WidowOrphanControl = 17,
        OutlineLevel = 18,
        BackColor = 19,
        LeftBorder = 20,
        RightBorder = 21,
        TopBorder = 22,
        BottomBorder = 23,
        UseValue = 24
    }

    export enum JSONCharacterFormattingProperty {
        FontInfoIndex = 0,
        FontSize = 1,
        FontBold = 2,
        FontItalic = 3,
        FontStrikeoutType = 4,
        FontUnderlineType = 5,
        AllCaps = 6,
        StrikeoutWordsOnly = 7,
        UnderlineWordsOnly = 8,
        ForeColor = 9,
        BackColor = 10,
        UnderlineColor = 11,
        StrikeoutColor = 12,
        Script = 13,
        Hidden = 14,
        LangInfo = 15,
        NoProof = 16,
        UseValue = 17
    }

    export enum JSONInlineObjectProperty {
        Scales = 0,
        LockAspectRatio = 1
    }

    export enum JSONSectionProperty {
        MarginLeft = 0,
        MarginTop = 1,
        MarginRight = 2,
        MarginBottom = 3,
        ColumnCount = 4,
        Space = 5,
        ColumnsInfo = 6,
        PageWidth = 7,
        PageHeight = 8,
        StartType = 9,
        Landscape = 10,
        EqualWidthColumns = 11,
        DifferentFirstPage = 12,
        HeaderOffset = 13,
        FooterOffset = 14
    }

    export enum JSONBorderBaseProperty {
        Style = 0,
        Color = 1,
        Width = 2,
        Offset = 3,
        Frame = 4,
        Shadow = 5
    }

    export enum JSONFontInfoProperty {
        Name = 0,
        ScriptMultiplier = 1,
        CssString = 2,
        CanBeSet = 3,
        SubScriptOffset = 4
    }

    export enum JSONLangInfoProperty {
        Latin = 0,
        Bidi = 1,
        EastAsia = 2
    }

    export enum IsModified {
        False = 0,
        True = 1,
        SaveInProgress = 2
    }

    export enum DeletedContent {//flags to prevent excessive items in JSON
        Undefinded = 0,
        Text = 1,
        Paragraph = 2,
        Section = 4
    }

    export enum ResponseError {
        ModelIsChanged = 1,
        InnerException = 2,
        AuthException = 3,
        CantSaveToAlreadyOpenedFile = 4,
        CantSaveDocument = 5,
        CantOpenDocument = 6,
        CalculateDocumentVariableException = 7,
        PathTooLongException = 8
    }

    export enum JSONListLevelProperty {
        Start = 0,
        Format = 1,
        ConvertPreviousLevelNumberingToDecimal = 2,
        SuppressBulletResize = 3,
        SuppressRestart = 4,
        Alignment = 5,
        DisplayFormatString = 6,
        RelativeRestartLevel = 7,
        Separator = 8,
        TemplateCode = 9,
        OriginalLeftIndent = 10,
        Legacy = 11,
        LegacySpace = 12,
        LegacyIndent = 13
    }

    export enum JSONIOverrideListLevelProperty {
        NewStart = 0,
        OverrideStart = 1
    }


} 