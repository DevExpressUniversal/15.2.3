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

#if !SL
namespace DevExpress.Office.Internal {
	using DevExpress.Xpf.Editors.Settings;
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Windows.Markup;
	using System.Windows.Media;
	public interface IFontBasedElement {
	}
	public static class FontManager {
		static Dictionary<string, FontFamily> fontFamilyCache = new Dictionary<string, FontFamily>();
		public static void RegisterFontBasedElement(IFontBasedElement element) {
		}
		public static IEnumerable<string> GetFontFamilyNames() {
			List<string> result = new List<string>();
			foreach (FontFamily fontFamily in FontUtility.GetSystemFontFamilies())
				result.Add(fontFamily.ToString());
			return result;
		}
		public static FontFamily GetFontFamily(string fontFamilyName) {
			foreach (FontFamily fontFamily in FontUtility.GetSystemFontFamilies()) {
				if (fontFamily.FamilyNames.Values.Contains(fontFamilyName))
					return fontFamily;
			}
			return new FontFamily(fontFamilyName);
		}
		public static IEnumerable<string> GetFontNames() {
			System.Drawing.Text.InstalledFontCollection fontCollection = new System.Drawing.Text.InstalledFontCollection();
			List<String> result = new List<string>();
			foreach (System.Drawing.FontFamily family in fontCollection.Families) {
				result.Add(family.Name);
			}
			return result;
		}
		public static IEnumerable<string> GetLocalizedFontNames() {
			Dictionary<string, FontFamily> systemFamilies = new Dictionary<string, FontFamily>();
			foreach (FontFamily fontFamily in FontUtility.GetSystemFontFamilies())
				systemFamilies.Add(fontFamily.Source, fontFamily);
			XmlLanguage lang = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
			List<string> result = new List<string>();
			foreach (string fontFamilyName in GetFontNames()) {
				FontFamily fontFamily;
				if (systemFamilies.TryGetValue(fontFamilyName, out fontFamily))
					result.Add(GetFontFamilyName(fontFamily, lang));
				else
					result.Add(fontFamilyName);
			}
			return result;
		}
		public static string GetFontFamilyName(FontFamily fontFamily) {
			XmlLanguage lang = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
			return GetFontFamilyName(fontFamily, lang);
		}
		public static string GetFontFamilyName(FontFamily fontFamily, XmlLanguage lang) {
			try {
				return fontFamily.FamilyNames.ContainsKey(lang) ? fontFamily.FamilyNames[lang] : fontFamily.Source;
			}
			catch {
			}
			return fontFamily.Source;
		}
	}
}
#endif
