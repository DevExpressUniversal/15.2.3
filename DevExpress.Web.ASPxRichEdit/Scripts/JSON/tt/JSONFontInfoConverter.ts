﻿


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
module __aspxRichEdit {
	export class JSONFontInfoConverter {
		static convertFromJSON(obj: any): FontInfo {
			var result = new FontInfo();
			result.name = obj[JSONFontInfoProperty.Name];
			result.scriptMultiplier = obj[JSONFontInfoProperty.ScriptMultiplier];
			result.cssString = obj[JSONFontInfoProperty.CssString];
			result.canBeSet = obj[JSONFontInfoProperty.CanBeSet];
			result.subScriptOffset = obj[JSONFontInfoProperty.SubScriptOffset];
			return result;
		}
		static convertToJSON(source: FontInfo): any {
			var result = {};
			result[JSONFontInfoProperty.Name] = source.name;
			result[JSONFontInfoProperty.ScriptMultiplier] = source.scriptMultiplier;
			result[JSONFontInfoProperty.CssString] = source.cssString;
			result[JSONFontInfoProperty.CanBeSet] = source.canBeSet;
			result[JSONFontInfoProperty.SubScriptOffset] = source.subScriptOffset;
			return result;
		}
	}
}