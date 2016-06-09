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
using System.Collections.Specialized;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Collections.Specialized;
namespace DevExpress.XtraRichEdit.Export.Rtf {
	public interface IRtfExportHelper { 
		int GetFontNameIndex(string name);
		int DefaultFontIndex { get; }
		int GetColorIndex(Color backColor);
		Color BlendColor(Color color);
		Dictionary<int, string> ListCollection { get; }
		Dictionary<int, int> ListOverrideCollectionIndex { get; }
		Dictionary<string, int> ParagraphStylesCollectionIndex { get; }
		Dictionary<string, int> CharacterStylesCollectionIndex { get; }
		Dictionary<string, int> TableStylesCollectionIndex { get; }
		Dictionary<int, int> FontCharsetTable { get; }
		List<string> ListOverrideCollection { get; }
		List<string> StylesCollection { get; }
		String DefaultCharacterProperties { get; set; }
		String DefaultParagraphProperties { get; set; }
		bool SupportStyle { get; }
	}
}
namespace DevExpress.XtraPrinting.Export.Rtf {
	public class RtfExportHelper : IRtfExportHelper {
		const string defaultFontName = "Times New Roman";
		readonly ColorCollection colorCollection;
		readonly StringCollection fontNamesCollection;
		readonly Dictionary<int, string> listCollection;
		readonly Dictionary<int, int> listOverrideCollectionIndex;
		readonly Dictionary<object, int> listCollectionIndex;
		readonly List<string> listOverrideCollection;
		readonly Dictionary<string, int> paragraphStylesCollectionIndex;
		readonly Dictionary<string, int> characterStylesCollectionIndex;
		readonly Dictionary<string, int> tableStylesCollectionIndex;
		readonly Dictionary<int, int> fontCharsetTable;
		readonly List<string> stylesCollection;
		string defaultCharacterProperties;
		string defaultParagraphProperties;
		int defaultFontIndex;
		public RtfExportHelper() {
			colorCollection = new ColorCollection();
			fontNamesCollection = new StringCollection();
			defaultFontIndex = GetFontNameIndex(defaultFontName);
			colorCollection.Add(Color.Empty);
			listCollection = new Dictionary<int, string>();
			listOverrideCollectionIndex = new Dictionary<int, int>();
			listCollectionIndex = new Dictionary<object, int>();
			listOverrideCollection = new List<string>();
			paragraphStylesCollectionIndex = new Dictionary<string, int>();
			characterStylesCollectionIndex = new Dictionary<string, int>();
			tableStylesCollectionIndex = new Dictionary<string, int>();
			fontCharsetTable = new Dictionary<int, int>();
			stylesCollection = new List<string>();
		}
		public ColorCollection ColorCollection { get { return colorCollection; } }
		public StringCollection FontNamesCollection { get { return fontNamesCollection; } }
		public Dictionary<int, string> ListCollection { get { return listCollection; } }
		public Dictionary<int, int> ListOverrideCollectionIndex { get { return listOverrideCollectionIndex; } }
		public Dictionary<object, int> ListCollectionIndex { get { return listCollectionIndex; } }
		public List<string> ListOverrideCollection { get { return listOverrideCollection; } }
		public int DefaultFontIndex { get { return defaultFontIndex; } }
		public Dictionary<string, int> ParagraphStylesCollectionIndex { get { return paragraphStylesCollectionIndex; } }
		public Dictionary<string, int> CharacterStylesCollectionIndex { get { return characterStylesCollectionIndex; } }
		public Dictionary<string, int> TableStylesCollectionIndex { get { return tableStylesCollectionIndex; } }
		public Dictionary<int, int> FontCharsetTable { get { return fontCharsetTable; } }
		public List<string> StylesCollection { get { return stylesCollection; } }
		public bool SupportStyle { get { return false; } }
		public string DefaultCharacterProperties { get { return defaultCharacterProperties; } set { defaultCharacterProperties = value; } }
		public string DefaultParagraphProperties { get { return defaultParagraphProperties; } set { defaultParagraphProperties = value; } }
		public int GetColorIndex(Color color) {
			if(color == Color.Transparent)
				return 0;
			return colorCollection.IndexOf(color) >= 0 ? colorCollection.IndexOf(color) :
				colorCollection.Add(color);
		}
		public int GetFontNameIndex(string fontName) {
			int fontIndex = fontNamesCollection.IndexOf(fontName);
			if(fontIndex >= 0)
				return fontIndex;
			fontNamesCollection.Add(fontName);
			return FontNamesCollection.Count - 1;
		}
		public Color BlendColor(Color color) {
			return DXColor.Blend(color, Color.White);
		}
	}
	public static class RtfExportBorderHelper {
		const int maxSingleBorderWidth = 75;
		static string GetBorderStyle(BorderDashStyle style, int borderWidth) {
			switch(style) {
				case BorderDashStyle.Dash:
					return RtfTags.DashBorderStyle;
				case BorderDashStyle.DashDot:
					return RtfTags.DashDotBorderStyle;
				case BorderDashStyle.DashDotDot:
					return RtfTags.DashDotDotBorderStyle;
				case BorderDashStyle.Dot:
					return RtfTags.DotBorderStyle;
				case BorderDashStyle.Double:
					return RtfTags.DoubleBorderStyle;
				default:
					if(borderWidth > maxSingleBorderWidth)
						return RtfTags.DoubleBorderWidth;
					else
						return RtfTags.SingleBorderWidth;
			}
		}
		static int GetBorderWidth(BorderDashStyle style, int borderWidth) {
			return style == BorderDashStyle.Double ? borderWidth / 3 : borderWidth;
		}
		public static string GetFullBorderStyle(BorderDashStyle style, int borderWidth, int borderColorIndex) {
			string borderStyle = RtfExportBorderHelper.GetBorderStyle(style, borderWidth);
			string formattedBorderWidth = String.Format(RtfTags.BorderWidth, RtfExportBorderHelper.GetBorderWidth(style, borderWidth));
			string borderColor = String.Format(RtfTags.BorderColor, borderColorIndex);
			return borderStyle + formattedBorderWidth + borderColor;
		}
	}
	public static class RtfAlignmentConverter {
		public static string ToHorzRtfAlignment(TextAlignment align) {
			switch(align) {
				case TextAlignment.TopLeft:
				case TextAlignment.MiddleLeft:
				case TextAlignment.BottomLeft:
					return RtfTags.LeftAligned + " ";
				case TextAlignment.TopCenter:
				case TextAlignment.MiddleCenter:
				case TextAlignment.BottomCenter:
					return RtfTags.Centered + " ";
				case TextAlignment.TopJustify:
				case TextAlignment.MiddleJustify:
				case TextAlignment.BottomJustify:
					return RtfTags.Justified + " ";
				case TextAlignment.TopRight:
				case TextAlignment.MiddleRight:
				case TextAlignment.BottomRight:
					return RtfTags.RightAligned + " ";
			}
			throw new ArgumentException("align");
		}
		 public static string ToVertRtfAlignment(TextAlignment align) {
			 switch(align) {
				 case TextAlignment.TopLeft:
				 case TextAlignment.TopCenter:
				 case TextAlignment.TopJustify:
					 return RtfTags.TopAlignedInCell;
				 case TextAlignment.MiddleLeft:
				 case TextAlignment.MiddleCenter:
				 case TextAlignment.MiddleJustify:
					 return RtfTags.VerticalCenteredInCell;
				 case TextAlignment.BottomLeft:
				 case TextAlignment.BottomCenter:
				 case TextAlignment.BottomJustify:
					 return RtfTags.BottomAlignedInCell;
				 case TextAlignment.TopRight:
					 return RtfTags.TopAlignedInCell + " ";
				 case TextAlignment.MiddleRight:
					 return RtfTags.VerticalCenteredInCell + " ";
				 case TextAlignment.BottomRight:
					 return RtfTags.BottomAlignedInCell + " ";
			 }
			throw new ArgumentException("align");
		}
	}
}
