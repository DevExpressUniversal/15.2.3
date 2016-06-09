module __aspxRichEdit {
    export class SpecialCharacters {
        Dot: string = '.';
        Colon: string = ':';
        Underscore: string = '_';
        EqualSign: string = '=';
        MiddleDot: string = '\u00B7';
        Dash: string = '-';
        ParagraphMark: string = String.fromCharCode(0xD);
        SectionMark: string = String.fromCharCode(0x1D);
        Hyphen: string = '\u001F';
        TabMark: string = String.fromCharCode(0x09);
        NonBreakingSpace: string = '\u00A0';
        Space: string = ' ';
        EmSpace: string = '\u2003';
        EnSpace: string = '\u2002';
        QmSpace: string = '\u2005';
        LineBreak: string = '\u000B';
        PageBreak: string = '\u000C';
        ColumnBreak: string = '\u000E';
        ObjectMark: string = '\uFFFC';
        FloatingObjectMark: string = '\u0008';
        NumberingListMark: string = '\uFFFB';
        EmDash: string = '\u2014';
        EnDash: string = '\u2013';
        Bullet: string = '\u2022';
        LeftSingleQuote: string = '\u2018';
        RightSingleQuote: string = '\u2019';
        LeftDoubleQuote: string = '\u201C';
        RightDoubleQuote: string = '\u201D';
        PilcrowSign: string = '\u00B6';
        CurrencySign: string = '\u00A4';
        CopyrightSymbol: string = '\u00A9';
        TrademarkSymbol: string = '\u2122';
        OptionalHyphen: string = '\u00AD';
        RegisteredTrademarkSymbol: string = '\u00AE';
        Ellipsis: string = '\u2026';
        OpeningSingleQuotationMark: string = '\u2018';
        ClosingSingleQuotationMark: string = '\u2019';
        OpeningDoubleQuotationMark: string = '\u201C';
        ClosingDoubleQuotationMark: string = '\u201D';
        SeparatorMark: string = '|';

        HiddenLineBreak: string = String.fromCharCode(0x21B2);
        HiddenParagraphMark: string = String.fromCharCode(0x00B6);
        HiddenSpace: string = String.fromCharCode(0x00B7);
        HiddenTabSpace: string = String.fromCharCode(0x2192);

        FieldCodeStartRun: string = "{";
        FieldCodeEndRun: string = "}";
        FieldResultEndRun: string = ">";

        LayoutDependentText: string = "#";
    }
} 