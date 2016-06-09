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
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.Office.Drawing {
	#region WpfFontInfo
	public class WpfFontInfo : GdiFontInfo {
		Typeface typeface;
		GlyphTypeface glyphTypeface;
		public WpfFontInfo(FontInfoMeasurer measurer, string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline)
			: base(measurer, fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline) {
		}
		#region Properties
		public System.Windows.Media.FontFamily FontFamily { get { return Typeface.FontFamily; } }
		public GlyphTypeface GlyphTypeface { get { return glyphTypeface; } }
		public Typeface Typeface { get { return typeface; } }
		#endregion
		protected override void CreateFont(FontInfoMeasurer measurer, string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			base.CreateFont(measurer, fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline);
			TypefaceResolver typefaceResolver = new TypefaceResolver();
			this.typeface = typefaceResolver.GetTypeface(this.Font.Name, fontBold, fontItalic);
			if (!typeface.TryGetGlyphTypeface(out this.glyphTypeface)) {
			}
		}
	}
	#endregion
	#region TypefaceCreator
	public class TypefaceResolver {
		static readonly List<string> fontFamilyNames;
		static readonly Dictionary<string, Typeface> knownTypefaces;
		static readonly StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
		static TypefaceResolver() {
			fontFamilyNames = new List<string>();
			foreach (string fontFamilyName in DevExpress.Office.Internal.FontManager.GetFontFamilyNames())
				fontFamilyNames.Add(fontFamilyName);
			fontFamilyNames.Sort(comparer);
			knownTypefaces = new Dictionary<string, Typeface>(comparer);
		}
		public Typeface GetTypeface(string fontName, bool fontBold, bool fontItalic) {
			bool needFindFontFamily = (fontBold || fontItalic) && !FontFamilyExist(fontName);
			if (needFindFontFamily) {
				Typeface typeface = FindTypeface(fontName);
				if (typeface != null) {
					System.Windows.FontStyle fontStyle = GetActualFontStyle(fontItalic, typeface.Style);
					FontWeight fontWeight = GetActualFontWeigth(fontBold, typeface.Weight);
					FontStretch fontStretch = typeface.Stretch;
					return new Typeface(typeface.FontFamily, fontStyle, fontWeight, fontStretch);
				}
			}
			System.Windows.FontStyle desiredFontStyle = (fontItalic ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal);
			FontWeight desiredFontWeight = (fontBold ? FontWeights.Bold : FontWeights.Normal);
			FontStretch desiredFontStretches = FontStretches.Normal;
			return new Typeface(new System.Windows.Media.FontFamily(fontName), desiredFontStyle, desiredFontWeight, desiredFontStretches);
		}
		Typeface FindTypeface(string fontName) {
			Typeface result;
			if (knownTypefaces.TryGetValue(fontName, out result))
				return result;
			result = FindTypefaceCore(fontName);
			knownTypefaces.Add(fontName, result);
			return result;
		}
		Typeface FindTypefaceCore(string fontName) {
			string fontFamilyName = FindFontFamily(fontName);
			if (String.IsNullOrEmpty(fontFamilyName))
				return null;
			System.Windows.Media.FontFamily fontFamily = new System.Windows.Media.FontFamily(fontFamilyName);
			string faceName = fontName.Remove(0, fontFamilyName.Length).Trim();
			System.Diagnostics.Debug.Assert(faceName.Length > 0);
			foreach (Typeface typeface in fontFamily.GetTypefaces())
				if (ContainsFaceName(typeface.FaceNames, faceName))
					return typeface;
			return null;
		}
		FontWeight GetActualFontWeigth(bool fontBold, FontWeight fontWeight) {
			if (fontBold) {
				if(fontWeight <= FontWeights.Bold)
					return FontWeights.Bold;
			}
			return fontWeight;
		}
		System.Windows.FontStyle GetActualFontStyle(bool fontItalic, System.Windows.FontStyle typefaceFontStyle) {
			if (fontItalic && typefaceFontStyle == FontStyles.Normal)
				return FontStyles.Italic;
			else
				return typefaceFontStyle;
		}
		bool ContainsFaceName(LanguageSpecificStringDictionary faceNames, string faceName) {
			foreach (var face in faceNames)
				if (comparer.Compare(face.Value, faceName) == 0)
					return true;
			return false;
		}
		bool FontFamilyExist(string fontName) {
			return fontFamilyNames.BinarySearch(fontName, comparer) >= 0;			
		}
		string FindFontFamily(string fontName) {
			int count = fontFamilyNames.Count;
			string maxSubstring = String.Empty;
			for (int i = 0; i < count; i++) {
				string fontFamilyName = fontFamilyNames[i];
				if (fontName.StartsWith(fontFamilyName, true, System.Globalization.CultureInfo.InvariantCulture) && fontFamilyName.Length > maxSubstring.Length)
					maxSubstring = fontFamilyName;
			}
			return maxSubstring;
		}		
	}
	#endregion
}
