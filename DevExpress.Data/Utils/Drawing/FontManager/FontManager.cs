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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Utils;
namespace DevExpress.Utils.Internal {
	public interface IFontBasedElement {
		void OnFontsChanged();
	}
	public class FontDescriptorManager {
		readonly FontFamily fontFamily;
		readonly Stream normalFontStream, boldFontStream, italicFontStream, boldItalicFontStream;
		readonly FontDescriptor[] fontDescriptors;
		readonly bool isPredefined;
		public FontDescriptorManager(string fontFamilyName, Stream normalFontStream, Stream boldFontStream,
				Stream italicFontStream, Stream boldItalicFontStream, bool isPredefined) {
			Guard.ArgumentNotNull(normalFontStream, "normalFontStream");
			this.fontFamily = new FontFamily(fontFamilyName);
			this.fontDescriptors = new FontDescriptor[4];
			this.normalFontStream = normalFontStream;
			this.boldFontStream = boldFontStream;
			this.italicFontStream = italicFontStream;
			this.boldItalicFontStream = boldItalicFontStream;
			this.isPredefined = isPredefined;
		}
		public FontDescriptor GetFontDescriptor(bool bold, bool italic) {
			return GetFontDescriptor(GetFontStyle(bold, italic));
		}
		FontDescriptor GetFontDescriptor(FontStyle fontStyle) {
			FontDescriptor result = fontDescriptors[(int)fontStyle];
			if (result == null) {
				result = CreateFontDescriptor(fontStyle);
				fontDescriptors[(int)fontStyle] = result;
			}
			return result;
		}
		FontDescriptor CreateFontDescriptor(FontStyle fontStyle) {
			switch (fontStyle) {
				case FontStyle.Regular:
					return CreateFontDescriptorCore(FontStyle.Regular, normalFontStream);
				case FontStyle.Bold:
					if (boldFontStream != null)
						return CreateFontDescriptorCore(FontStyle.Bold, boldFontStream);
					else
						return GetFontDescriptor(FontStyle.Regular);
				case FontStyle.Italic:
					if (italicFontStream != null)
						return CreateFontDescriptorCore(FontStyle.Italic, italicFontStream);
					else
						return GetFontDescriptor(FontStyle.Regular);
				case FontStyle.Bold | FontStyle.Italic:
					if (boldItalicFontStream != null)
						return CreateFontDescriptorCore(FontStyle.Bold | FontStyle.Italic, boldItalicFontStream);
					else
						if (boldFontStream != null)
							return GetFontDescriptor(FontStyle.Bold);
						else
							if (italicFontStream != null)
								return GetFontDescriptor(FontStyle.Italic);
							else
								return GetFontDescriptor(FontStyle.Regular);
				default:
					throw new ArgumentException("fontStyle");
			}
		}
		FontDescriptor CreateFontDescriptorCore(FontStyle fontStyle, Stream fontStream) {
			if (isPredefined)
				return new FontDescriptor(fontFamily, fontStyle, fontStream, null);
			else
#if !SL
				return new FontDescriptor(fontFamily, fontStyle, fontStream, null);
#else
				return new FontDescriptor(fontFamily, fontStyle, fontStream, new FontSource(fontStream));
#endif
		}
		FontStyle GetFontStyle(bool bold, bool italic) {
			FontStyle res = FontStyle.Regular;
			if (bold)
				res |= FontStyle.Bold;
			if (italic)
				res |= FontStyle.Italic;
			return res;
		}
	}
	public class FontManager {
		public const string DefaultFontFamilyName = "Arial";
		static string[] PredefinedFontFamilyNamesWin = new string[] {
			"Arial",
			"Arial Black",
			"Comic Sans MS",
			"Courier New",
			"Georgia",
			"Lucida Sans Unicode",
			"Times New Roman",
			"Trebuchet MS",
			"Verdana",
			"MS Mincho",
			"MS PMincho",
			"SimSun",
			"SimSun-ExtB",
			"Calibri",
			"Book Antiqua",
			"Bookman Old Style",
			"Cambria",
			"Candara",
			"Century",
			"Century Gothic",
			"Century Schoolbook",
			"Consolas",
			"Cordia New",
			"Franklin Gothic Book",
			"Garamond",
			"Gill Sans MT",
			"Impact",
			"Lucida Console",
			"Lucida Sans",
			"MingLiU",
			"MingLiU_HKSCS",
			"MS Gothic",
			"Palatino Linotype",
			"PMingLiU-ExtB",
			"PMingLiU",
			"Rockwell",
			"Segoe UI",
			"Shruti",
			"Simhei",
			"Sylfaen",
			"Tahoma",
			"Tunga",
			"TW Cen MT",
			"Vrinda",
			"Angsana New"
		};
		static string[] PredefinedFontFilesWin = new string[] {
			"Arial",
			"ArialBlack",
			"ComicSansMS",
			"CourierNew",
			"Georgia",
			"LucidaSansUnicode",
			"Times",
			"TrebuchetMS",
			"Verdana",
			"MSMincho",
			"MSPMincho",
			"SimSun",
			"SimSunExtB",
			"Calibri",
			"BookAntiqua",
			"BookmanOldStyle",
			"Cambria",
			"Candara",
			"Century",
			"CenturyGothic",
			"CenturySchoolbook",
			"Consolas",
			"CordiaNew",
			"FranklinGothicBook",
			"Garamond",
			"GillSansMT",
			"Impact",
			"LucidaConsole",
			"LucidaSans",
			"MingLiU",
			"MingLiU_HKSCS",
			"MSGothic",
			"PalatinoLinotype",
			"PMingLiU-ExtB",
			"PMingLiU",
			"Rockwell",
			"SegoeUI",
			"Shruti",
			"Simhei",
			"Sylfaen",
			"Tahoma",
			"Tunga",
			"TWCenMT",
			"Vrinda",
			"AngsaNew"
		};
		static Dictionary<string, FontDescriptorManager> customFontDescriptorCache = new Dictionary<string, FontDescriptorManager>();
		static Dictionary<string, FontDescriptorManager> predefinedFontDescriptorCache = new Dictionary<string, FontDescriptorManager>();
		static List<WeakReference> ElementsToNotify = new List<WeakReference>();
		static FontManager() {
			RegisterPredefinedFonts();
		}
		static void RegisterPredefinedFonts() {
			string[] families = PredefinedFontFamilyNamesWin;
			string[] files = PredefinedFontFilesWin;
			for (int i = 0; i < families.Length; i++) {
				FontDescriptorManager value = CreateFontDescriptorManager(families[i], files[i]);
				if (value != null)
					predefinedFontDescriptorCache.Add(families[i], value);
			}
		}
		static FontDescriptorManager CreateFontDescriptorManager(string familyName, string fileName) {
			try {
				return new FontDescriptorManager(familyName,
					GetPredefinedFontStream(fileName),
					GetPredefinedFontStream(fileName + "B"),
					GetPredefinedFontStream(fileName + "I"),
					GetPredefinedFontStream(fileName + "BI"),
					true);
			}
			catch {
				return null;
			}
		}
		internal static Stream GetPredefinedFontStream(string name) {
			return typeof(FontManager).GetAssembly().GetManifestResourceStream("DevExpress.Data.Utils.Drawing.FontManager.Fonts." + name + ".fmx");
		}
		static public FontDescriptor GetFontDescriptor(string familyName, bool bold, bool italic) {
			FontDescriptorManager fontDescriptorManager;
			if (customFontDescriptorCache.TryGetValue(familyName, out fontDescriptorManager))
				return fontDescriptorManager.GetFontDescriptor(bold, italic);
			else
				if (predefinedFontDescriptorCache.TryGetValue(familyName, out fontDescriptorManager))
					return fontDescriptorManager.GetFontDescriptor(bold, italic);
				else
					return GetFontDescriptor(DefaultFontFamilyName, bold, italic);
		}
		public static void RegisterFontBasedElement(IFontBasedElement element) {
			ElementsToNotify.Add(new WeakReference(element));
		}
		public static IEnumerable<string> GetFontFamilyNames() {
			List<string> result = new List<string>();
			result.AddRange(customFontDescriptorCache.Keys);
			foreach (string fontFamilyName in predefinedFontDescriptorCache.Keys)
				if (!result.Contains(fontFamilyName))
					result.Add(fontFamilyName);
			result.Sort();
			return result;
		}
		public static IEnumerable<int> GetPredefinedFontSizes() {
			List<int> result = new List<int>();
			result.Add(8);
			result.Add(9);
			result.Add(10);
			result.Add(11);
			result.Add(12);
			result.Add(14);
			result.Add(16);
			result.Add(18);
			result.Add(20);
			result.Add(22);
			result.Add(24);
			result.Add(26);
			result.Add(28);
			result.Add(36);
			result.Add(48);
			result.Add(72);
			return result;
		}
		static public void RegisterFontFamily(string familyName, Stream normalFontStream) {
			RegisterFontFamily(familyName, normalFontStream, null, null, null);
		}
		static public void RegisterFontFamily(string familyName, Stream normalFontStream, Stream boldFontStream, Stream italicFontStream, Stream boldItalicFontStream) {
			if (customFontDescriptorCache.ContainsKey(familyName))
				return;
			customFontDescriptorCache.Add(familyName, new FontDescriptorManager(familyName, normalFontStream, boldFontStream, italicFontStream, boldItalicFontStream, false));
			RaiseFontFamilyNamesChanged();
		}
		static public void UnregisterFontFamily(string familyName) {
			if (!customFontDescriptorCache.ContainsKey(familyName))
				return;
			customFontDescriptorCache.Remove(familyName);
			RaiseFontFamilyNamesChanged();
		}
		static void RaiseFontFamilyNamesChanged() {
			foreach (WeakReference element in ElementsToNotify) {
				IFontBasedElement fontBasedElement = element.Target as IFontBasedElement;
				fontBasedElement.OnFontsChanged();
			}
		}
	}
}
