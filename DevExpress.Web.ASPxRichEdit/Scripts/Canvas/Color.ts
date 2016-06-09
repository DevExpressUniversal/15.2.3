module __aspxRichEdit {
    export class ColorHelper {
        static colorNames = {
            aliceblue: '#f0f8ff', antiquewhite: '#faebd7', aqua: '#00ffff',
            aquamarine: '#7fffd4', azure: '#f0ffff', beige: '#f5f5dc',
            bisque: '#ffe4c4', black: '#000000', blanchedalmond: '#ffebcd',
            blue: '#0000ff', blueviolet: '#8a2be2', brown: '#a52a2a',
            burlywood: '#deb887', cadetblue: '#5f9ea0', chartreuse: '#7fff00',
            chocolate: '#d2691e', coral: '#ff7f50', cornflowerblue: '#6495ed',
            cornsilk: '#fff8dc', crimson: '#dc143c', cyan: '#00ffff',
            darkblue: '#00008b', darkcyan: '#008b8b', darkgoldenrod: '#b8860b',
            darkgray: '#a9a9a9', darkgreen: '#006400', darkkhaki: '#bdb76b',
            darkmagenta: '#8b008b', darkolivegreen: '#556b2f', darkorange: '#ff8c00',
            darkorchid: '#9932cc', darkred: '#8b0000', darksalmon: '#e9967a',
            darkseagreen: '#8fbc8f', darkslateblue: '#483d8b', darkslategray: '#2f4f4f',
            darkturquoise: '#00ced1', darkviolet: '#9400d3', deeppink: '#ff1493',
            deepskyblue: '#00bfff', dimgray: '#696969', dodgerblue: '#1e90ff',
            firebrick: '#b22222', floralwhite: '#fffaf0',
            forestgreen: '#228b22', fuchsia: '#ff00ff', gainsboro: '#dcdcdc',
            ghostwhite: '#f8f8ff', gold: '#ffd700', goldenrod: '#daa520', gray: '#808080',
            green: '#008000', greenyellow: '#adff2f', honeydew: '#f0fff0',
            hotpink: '#ff69b4', indianred: '#cd5c5c', indigo: '#4b0082',
            ivory: '#fffff0', khaki: '#f0e68c', lavender: '#e6e6fa',
            lavenderblush: '#fff0f5', lawngreen: '#7cfc00', lemonchiffon: '#fffacd',
            lightblue: '#add8e6', lightcoral: '#f08080', lightcyan: '#e0ffff',
            lightgoldenrodyellow: '#fafad2', lightgray: '#d3d3d3', lightgreen: '#90ee90',
            lightpink: '#ffb6c1', lightsalmon: '#ffa07a', lightseagreen: '#20b2aa',
            lightskyblue: '#87cefa', lightslategray: '#778899', lightsteelblue: '#b0c4de',
            lightyellow: '#ffffe0', lime: '#00ff00', limegreen: '#32cd32', linen: '#faf0e6',
            magenta: '#ff00ff', maroon: '#800000', mediumaquamarine: '#66cdaa',
            mediumblue: '#0000cd', mediumorchid: '#ba55d3', mediumpurple: '#9370db',
            mediumseagreen: '#3cb371', mediumslateblue: '#7b68ee',
            mediumspringgreen: '#00fa9a', mediumturquoise: '#48d1cc',
            mediumvioletred: '#c71585', midnightblue: '#191970', mintcream: '#f5fffa',
            mistyrose: '#ffe4e1', moccasin: '#ffe4b5', navajowhite: '#ffdead',
            navy: '#000080', oldlace: '#fdf5e6', olive: '#808000', olivedrab: '#6b8e23',
            orange: '#ffa500', orangered: '#ff4500', orchid: '#da70d6',
            alegoldenrod: '#eee8aa', palegreen: '#98fb98', paleturquoise: '#afeeee',
            palevioletred: '#db7093', papayawhip: '#ffefd5', peachpuff: '#ffdab9',
            peru: '#cd853f', pink: '#ffc0cb', plum: '#dda0dd', powderblue: '#b0e0e6',
            purple: '#800080', red: '#ff0000', rosybrown: '#bc8f8f', royalblue: '#4169e1',
            saddlebrown: '#8b4513', salmon: '#fa8072', sandybrown: '#f4a460',
            seagreen: '#2e8b57', seashell: '#fff5ee', sienna: '#a0522d',
            silver: '#c0c0c0', skyblue: '#87ceeb', slateblue: '#6a5acd',
            slategray: '#708090', snow: '#fffafa', springgreen: '#00ff7f',
            steelblue: '#4682b4', tan: '#d2b48c', teal: '#008080', thistle: '#d8bfd8',
            tomato: '#ff6347', turquoise: '#40e0d0', violet: '#ee82ee', wheat: '#f5deb3',
            white: '#ffffff', whitesmoke: '#f5f5f5', yellow: '#ffff00', yellowgreen: '#9acd32'
        };

        //ITU 601:
        //Y' = 0.299 * R + 0.587 * G + 0.114 * B
        static DEFAULT_BOUNDARY_LUMA = 60.762 * 65536; // exeprimental value;
        static DEFAULT_BOUNDARY_LUMA_RED = 0.299 * 65536;
        static DEFAULT_BOUNDARY_LUMA_BLUE = 0.114 * 65536;
        static DEFAULT_BOUNDARY_LUMA_GREEN = 0.587 * 65536;

        static DARK_COLOR = -16777216; // FFFFFFFFFF000000
        static LIGHT_COLOR = -1; // FFFFFFFFFFFFFFFF
        
        static BLACK_COLOR: number = -16777216; // FFFFFFFFFF000000 // todo delete

        // here need get color, that different from back color. (back color that table, paragraph, box back colors)
        static AUTOMATIC_COLOR: number = 0; // for text color, text strikeout, text underline. Character back color don't affect fore color, bot paragraph - affect
        static NO_COLOR: number = 16777215; // 00000000FFFFFF for back color

        static getAlpha(color: number): number {
            return (color >> 24) & 255;
        }

        static getRed(color: number): number {
            return (color >> 16) & 255;
        }

        static getGreen(color: number): number {
            return (color >> 8) & 255;
        }

        static getBlue(color: number): number {
            return color & 255;
        }

        static redPartToString(color: number): string {
            var redStr: string = ColorHelper.getRed(color).toString(16);
            return redStr.length > 1 ? redStr : "0" + redStr;
        }

        static greenPartToString(color: number): string {
            var greenStr: string = ColorHelper.getGreen(color).toString(16);
            return greenStr.length > 1 ? greenStr : "0" + greenStr;
        }

        static bluePartToString(color: number): string {
            var blueStr: string = ColorHelper.getBlue(color).toString(16);
            return blueStr.length > 1 ? blueStr : "0" + blueStr;
        }

        static colorToHash(color: number): string {
            return "#" + ColorHelper.redPartToString(color) + ColorHelper.greenPartToString(color) + ColorHelper.bluePartToString(color);
        }

        static hashToColor(hash: string, alpha: number = 255): number {
            return ((hash.charAt(0) == "#" ? parseInt(hash.substr(1), 16) : parseInt(hash, 16))) | (alpha << 24);
        }

        static getActualForeColor(foreColor: number, backColor: number): string {
            if (foreColor == ColorHelper.AUTOMATIC_COLOR) {
                var backColorIsLight: boolean =
                    backColor == ColorHelper.AUTOMATIC_COLOR ||
                    backColor == ColorHelper.NO_COLOR ||
                    ColorHelper.calculateLumaY(backColor) >= ColorHelper.DEFAULT_BOUNDARY_LUMA;
                foreColor = backColorIsLight ? ColorHelper.DARK_COLOR : ColorHelper.LIGHT_COLOR;
            }
            return ColorHelper.getCssString(foreColor, true);
        }

        static getCssString(color: number, isAutoColorTranslateToDark: boolean): string {
            if (color == ColorHelper.AUTOMATIC_COLOR)
                return ColorHelper.colorToHash(isAutoColorTranslateToDark ? ColorHelper.DARK_COLOR : ColorHelper.LIGHT_COLOR);
            return ColorHelper.getCssStringInternal(color);
        }

        static IsDarkColor(color: number): boolean {
            return ColorHelper.calculateLumaY(color) < ColorHelper.DEFAULT_BOUNDARY_LUMA;
        }

        static getCssStringInternal(color: number): string {
            var alpfa: number = ColorHelper.getAlpha(color);
            switch (alpfa) {
                case 0: return "transparent";
                case 255: return ColorHelper.colorToHash(color);
                default: return "rgba(" + ColorHelper.getRed(color) + "," + ColorHelper.getGreen(color) + "," + ColorHelper.getBlue(color) + "," + (alpfa / 255) + ")";
            }
        }

        static isEmptyBgColor(color: number): boolean {
            return color === this.AUTOMATIC_COLOR || color === this.NO_COLOR;
        }

        private static calculateLumaY(color: number): number {
            //ITU 601:
            //Y' = 0.299 * R + 0.587 * G + 0.114 * B
            //Word boundary: 60.762 (possible not very accurate). All tests based on MSWord results
            //y = Y' * 65536
            return ColorHelper.DEFAULT_BOUNDARY_LUMA_RED * ColorHelper.getRed(color) +
                ColorHelper.DEFAULT_BOUNDARY_LUMA_GREEN * ColorHelper.getGreen(color) +
                ColorHelper.DEFAULT_BOUNDARY_LUMA_BLUE * ColorHelper.getBlue(color);
        }
    }
}