module __aspxRichEdit {
    export class HtmlConverter {
        // no decoration used for strikeout, underline, back color
        public static getCssRules(charProps: CharacterProperties, isWordBox: boolean, noStrikeoutAndUnderline: boolean): string[] {
            var rules = HtmlConverter.getSizeSignificantRules(charProps);
            if(charProps.foreColor !== ColorHelper.AUTOMATIC_COLOR)
                rules.push("color: " + ColorHelper.getCssStringInternal(charProps.foreColor));
            if (!noStrikeoutAndUnderline) {
                if (charProps.fontStrikeoutType != StrikeoutType.None && (isWordBox || !charProps.strikeoutWordsOnly))
                    rules.push("text-decoration: line-through");
                if (charProps.fontUnderlineType != UnderlineType.None && (isWordBox || !charProps.underlineWordsOnly))
                    rules.push("text-decoration: underline");
            }
            return rules;
        }

        public static getSizeSignificantCssString(characterProperties: CharacterProperties): string {
            return HtmlConverter.getSizeSignificantRules(characterProperties).join(";");
        }

        public static buildHyperlinkTipString(hyperlinkTip: string): string {
            return hyperlinkTip + "\nCtrl + Click to follow link";
        }
        
        public static getSizeSignificantRules(characterProperties: CharacterProperties): string[] {
            var rules = [];
            if (characterProperties.allCaps)
                rules.push("text-transform: uppercase");
            rules.push("font-family: " + characterProperties.fontInfo.cssString);
            if (characterProperties.fontBold)
                rules.push("font-weight: bold");
            if (characterProperties.fontItalic)
                rules.push("font-style: italic");

            var fontSizePx = UnitConverter.pointsToPixels(characterProperties.fontSize);
            if (characterProperties.script == CharacterFormattingScript.Normal)
                rules.push("font-size: " + fontSizePx + "px");
            else {
                rules.push("font-size: " + Math.round(fontSizePx * characterProperties.fontInfo.scriptMultiplier) + "px");
            }
            return rules;
        }

        //// no used
        //private static getBorderStyleString(fontUnderlineType: UnderlineType): string {
        //    switch (fontUnderlineType) {
        //        case UnderlineType.DashDotDotted:
        //        case UnderlineType.ThickDashDotDotted:
        //        case UnderlineType.DashDotted:
        //        case UnderlineType.ThickDashDotted:
        //        case UnderlineType.Dashed:
        //        case UnderlineType.ThickDashed:
        //        case UnderlineType.DashSmallGap:
        //        case UnderlineType.ThickLongDashed:
        //            return "dashed";
        //        case UnderlineType.Dotted:
        //            return "dotted";
        //        case UnderlineType.Double:
        //        case UnderlineType.DoubleWave:
        //            return "double";
        //        default:
        //            return "solid";
        //    }
        //}
    }
} 