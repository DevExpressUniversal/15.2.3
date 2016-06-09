#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using DevExpress.Office.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
namespace DevExpress.Office.Drawing {
	#region ThemeFontScheme
	public class ThemeFontScheme {
		#region Fields
		readonly ThemeFontSchemePart minorFont;
		readonly ThemeFontSchemePart majorFont;
		string name = String.Empty;
		#endregion
		public ThemeFontScheme(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.minorFont = new ThemeFontSchemePart(documentModel);
			this.majorFont = new ThemeFontSchemePart(documentModel);
		}
		#region Properties
		public string Name { get { return name; } set { name = value; } }
		public ThemeFontSchemePart MinorFont { get { return minorFont; } }
		public ThemeFontSchemePart MajorFont { get { return majorFont; } }
		public bool IsValidate { get { return !String.IsNullOrEmpty(name) && minorFont.IsValid && majorFont.IsValid; } }
		#endregion
		public string GetTypeface(XlFontSchemeStyles schemeStyle, CultureInfo currentUICulture) {
			if (schemeStyle == XlFontSchemeStyles.None)
				return String.Empty;
			if (schemeStyle == XlFontSchemeStyles.Minor)
				return MinorFont.GetTypeface(currentUICulture);
			return MajorFont.GetTypeface(currentUICulture);
		}
		public void CopyFrom(ThemeFontScheme sourceObj) {
			name = sourceObj.Name;
			majorFont.CopyFrom(sourceObj.majorFont);
			minorFont.CopyFrom(sourceObj.minorFont);
		}
		protected internal void Clear() {
			name = String.Empty;
			majorFont.Clear();
			minorFont.Clear();
		}
	}
	#endregion
	#region ThemeFontSchemePart
	public class ThemeFontSchemePart {
		#region Static Members
		static Dictionary<string, string> cultureNameToScriptTable = GetCultureNameToScriptTable();
		static Dictionary<string, string> GetCultureNameToScriptTable() {
			Dictionary<string, string> result = new Dictionary<string, string>();
			result.Add("ar-LY", "Arab");
			result.Add("ar-DZ", "Arab");
			result.Add("ar-MA", "Arab");
			result.Add("ar-TN", "Arab");
			result.Add("ar-OM", "Arab");
			result.Add("ar-YE", "Arab");
			result.Add("ar-SY", "Arab");
			result.Add("ar-JO", "Arab");
			result.Add("ar-LB", "Arab");
			result.Add("ar-KW", "Arab");
			result.Add("ar-AE", "Arab");
			result.Add("ar-BH", "Arab");
			result.Add("ar-QA", "Arab");
			result.Add("ar-SA", "Arab");
			result.Add("ur-PK", "Arab");
			result.Add("fa-IR", "Arab");
			result.Add("ps-AF", "Arab");
			result.Add("prs-AF", "Arab");
			result.Add("ar-IQ", "Arab");
			result.Add("ar-EG", "Arab");
			result.Add("hy-AM", "Armn");
			result.Add("bn-IN", "Beng");
			result.Add("as-IN", "Beng");
			result.Add("bn-BD", "Beng");
			result.Add("iu-Cans-CA", "Cans");
			result.Add("sr-Cyrl-BA", "Cyrl");
			result.Add("bs-Cyrl-BA", "Cyrl");
			result.Add("sr-Cyrl-RS", "Cyrl");
			result.Add("sr-Cyrl-ME", "Cyrl");
			result.Add("bg-BG", "Cyrl");
			result.Add("ru-RU", "Cyrl");
			result.Add("uk-UA", "Cyrl");
			result.Add("be-BY", "Cyrl");
			result.Add("tg-Cyrl-TJ", "Cyrl");
			result.Add("mk-MK", "Cyrl");
			result.Add("kk-KZ", "Cyrl");
			result.Add("ky-KG", "Cyrl");
			result.Add("tt-RU", "Cyrl");
			result.Add("mn-MN", "Cyrl");
			result.Add("ba-RU", "Cyrl");
			result.Add("sah-RU", "Cyrl");
			result.Add("az-Cyrl-AZ", "Cyrl");
			result.Add("uz-Cyrl-UZ", "Cyrl");
			result.Add("sr-Cyrl-CS", "Cyrl");
			result.Add("hi-IN", "Deva");
			result.Add("mr-IN", "Deva");
			result.Add("sa-IN", "Deva");
			result.Add("kok-IN", "Deva");
			result.Add("ne-NP", "Deva");
			result.Add("am-ET", "Ethi");
			result.Add("ka-GE", "Geor");
			result.Add("el-GR", "Grek");
			result.Add("gu-IN", "Gujr");
			result.Add("pa-IN", "Guru");
			result.Add("ko-KR", "Hang");
			result.Add("zh-SG", "Hans");
			result.Add("zh-CN", "Hans");
			result.Add("zh-MO", "Hant");
			result.Add("zh-TW", "Hant");
			result.Add("zh-HK", "Hant");
			result.Add("he-IL", "Hebr");
			result.Add("ja-JP", "Jpan");
			result.Add("km-KH", "Khmr");
			result.Add("kn-IN", "Knda");
			result.Add("lo-LA", "Laoo");
			result.Add("de-LU", "Latn");
			result.Add("en-CA", "Latn");
			result.Add("es-GT", "Latn");
			result.Add("fr-CH", "Latn");
			result.Add("hr-BA", "Latn");
			result.Add("smj-NO", "Latn");
			result.Add("de-LI", "Latn");
			result.Add("en-NZ", "Latn");
			result.Add("es-CR", "Latn");
			result.Add("fr-LU", "Latn");
			result.Add("bs-Latn-BA", "Latn");
			result.Add("smj-SE", "Latn");
			result.Add("en-IE", "Latn");
			result.Add("es-PA", "Latn");
			result.Add("fr-MC", "Latn");
			result.Add("sr-Latn-BA", "Latn");
			result.Add("sma-NO", "Latn");
			result.Add("en-ZA", "Latn");
			result.Add("es-DO", "Latn");
			result.Add("sma-SE", "Latn");
			result.Add("en-JM", "Latn");
			result.Add("es-VE", "Latn");
			result.Add("sms-FI", "Latn");
			result.Add("en-029", "Latn");
			result.Add("es-CO", "Latn");
			result.Add("sr-Latn-RS", "Latn");
			result.Add("smn-FI", "Latn");
			result.Add("en-BZ", "Latn");
			result.Add("es-PE", "Latn");
			result.Add("en-TT", "Latn");
			result.Add("es-AR", "Latn");
			result.Add("sr-Latn-ME", "Latn");
			result.Add("en-ZW", "Latn");
			result.Add("es-EC", "Latn");
			result.Add("en-PH", "Latn");
			result.Add("es-CL", "Latn");
			result.Add("es-UY", "Latn");
			result.Add("es-PY", "Latn");
			result.Add("en-IN", "Latn");
			result.Add("es-BO", "Latn");
			result.Add("ca-ES", "Latn");
			result.Add("cs-CZ", "Latn");
			result.Add("da-DK", "Latn");
			result.Add("de-DE", "Latn");
			result.Add("en-US", "Latn");
			result.Add("es-ES", "Latn");
			result.Add("fi-FI", "Latn");
			result.Add("fr-FR", "Latn");
			result.Add("hu-HU", "Latn");
			result.Add("is-IS", "Latn");
			result.Add("it-IT", "Latn");
			result.Add("nl-NL", "Latn");
			result.Add("nb-NO", "Latn");
			result.Add("pl-PL", "Latn");
			result.Add("pt-BR", "Latn");
			result.Add("rm-CH", "Latn");
			result.Add("ro-RO", "Latn");
			result.Add("hr-HR", "Latn");
			result.Add("sk-SK", "Latn");
			result.Add("sq-AL", "Latn");
			result.Add("sv-SE", "Latn");
			result.Add("tr-TR", "Latn");
			result.Add("id-ID", "Latn");
			result.Add("sl-SI", "Latn");
			result.Add("et-EE", "Latn");
			result.Add("lv-LV", "Latn");
			result.Add("lt-LT", "Latn");
			result.Add("az-Latn-AZ", "Latn");
			result.Add("eu-ES", "Latn");
			result.Add("hsb-DE", "Latn");
			result.Add("tn-ZA", "Latn");
			result.Add("xh-ZA", "Latn");
			result.Add("zu-ZA", "Latn");
			result.Add("af-ZA", "Latn");
			result.Add("fo-FO", "Latn");
			result.Add("mt-MT", "Latn");
			result.Add("se-NO", "Latn");
			result.Add("ms-MY", "Latn");
			result.Add("en-MY", "Latn");
			result.Add("es-SV", "Latn");
			result.Add("sw-KE", "Latn");
			result.Add("tk-TM", "Latn");
			result.Add("uz-Latn-UZ", "Latn");
			result.Add("cy-GB", "Latn");
			result.Add("gl-ES", "Latn");
			result.Add("fy-NL", "Latn");
			result.Add("fil-PH", "Latn");
			result.Add("ha-Latn-NG", "Latn");
			result.Add("yo-NG", "Latn");
			result.Add("quz-BO", "Latn");
			result.Add("nso-ZA", "Latn");
			result.Add("lb-LU", "Latn");
			result.Add("kl-GL", "Latn");
			result.Add("ig-NG", "Latn");
			result.Add("arn-CL", "Latn");
			result.Add("moh-CA", "Latn");
			result.Add("br-FR", "Latn");
			result.Add("en-SG", "Latn");
			result.Add("es-HN", "Latn");
			result.Add("mi-NZ", "Latn");
			result.Add("oc-FR", "Latn");
			result.Add("co-FR", "Latn");
			result.Add("gsw-FR", "Latn");
			result.Add("qut-GT", "Latn");
			result.Add("rw-RW", "Latn");
			result.Add("wo-SN", "Latn");
			result.Add("gd-GB", "Latn");
			result.Add("es-NI", "Latn");
			result.Add("es-PR", "Latn");
			result.Add("es-US", "Latn");
			result.Add("de-CH", "Latn");
			result.Add("en-GB", "Latn");
			result.Add("es-MX", "Latn");
			result.Add("fr-BE", "Latn");
			result.Add("it-CH", "Latn");
			result.Add("nl-BE", "Latn");
			result.Add("nn-NO", "Latn");
			result.Add("pt-PT", "Latn");
			result.Add("sr-Latn-CS", "Latn");
			result.Add("sv-FI", "Latn");
			result.Add("dsb-DE", "Latn");
			result.Add("se-SE", "Latn");
			result.Add("ga-IE", "Latn");
			result.Add("ms-BN", "Latn");
			result.Add("iu-Latn-CA", "Latn");
			result.Add("tzm-Latn-DZ", "Latn");
			result.Add("quz-EC", "Latn");
			result.Add("de-AT", "Latn");
			result.Add("en-AU", "Latn");
			result.Add("fr-CA", "Latn");
			result.Add("se-FI", "Latn");
			result.Add("quz-PE", "Latn");
			result.Add("ml-IN", "Mlym");
			result.Add("mn-Mong-CN", "Mong");
			result.Add("or-IN", "Orya");
			result.Add("si-LK", "Sinh");
			result.Add("syr-SY", "Syrc");
			result.Add("ta-IN", "Taml");
			result.Add("te-IN", "Telu");
			result.Add("dv-MV", "Thaa");
			result.Add("th-TH", "Thai");
			result.Add("bo-CN", "Tibt");
			result.Add("ug-CN", "Uigh");
			result.Add("vi-VN", "Viet");
			result.Add("ii-CN", "Yiii");
			return result;
		}
		#endregion
		#region Fields
		readonly DrawingTextFont latin;
		readonly DrawingTextFont eastAsian;
		readonly DrawingTextFont complexScript;
		readonly Dictionary<string, string> supplementalFonts = new Dictionary<string, string>();
		bool isValid = false;
		#endregion
		public ThemeFontSchemePart(IDocumentModel documentModel){
			this.latin = new DrawingTextFont(documentModel);
			this.eastAsian = new DrawingTextFont(documentModel);
			this.complexScript = new DrawingTextFont(documentModel);
		}
		#region Properties
		public DrawingTextFont Latin { get { return latin; } }
		public DrawingTextFont EastAsian { get { return eastAsian; } }
		public DrawingTextFont ComplexScript { get { return complexScript; } }
		public Dictionary<string, string> SupplementalFonts { get { return supplementalFonts; } }
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		#endregion
		public void AddSupplementalFont(string script, string typeface) {
			if (!String.IsNullOrEmpty(script) && !supplementalFonts.ContainsKey(script))
				supplementalFonts.Add(script, typeface);
		}
		public void CopyFrom(ThemeFontSchemePart value) {
			latin.CopyFrom(value.latin);
			eastAsian.CopyFrom(value.eastAsian);
			complexScript.CopyFrom(value.complexScript);
			isValid = value.isValid;
			supplementalFonts.Clear();
			foreach (KeyValuePair<string, string> source in value.supplementalFonts)
				AddSupplementalFont(source.Key, source.Value);
		}
		public string GetTypeface(CultureInfo currentUICulture) {
			string name = currentUICulture.Name;
			if (cultureNameToScriptTable.ContainsKey(name)) {
				string script = cultureNameToScriptTable[name];
				if (supplementalFonts.ContainsKey(script))
					return supplementalFonts[script];
			}
			return GetDefaultTypeface();
		}
		protected internal void Clear() {
			Latin.Clear();
			EastAsian.Clear();
			ComplexScript.Clear();
			SupplementalFonts.Clear();
			IsValid = false;
		}
		string GetDefaultTypeface() {
			return latin.Typeface;
		}
	}
	#endregion
}
