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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
#if !SL
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfExportHelper (helper class)
	public class RtfExportHelper : IRtfExportHelper {
		#region Fields
		readonly string defaultFontName;
		readonly int defaultFontIndex;
		readonly List<string> fontNamesCollection;
		readonly Dictionary<int, string> listCollection;
		readonly Dictionary<int, int> listOverrideCollectionIndex;
		readonly List<string> listOverrideCollection;
		readonly ColorCollection colorCollection;
		readonly Dictionary<string, int> paragraphStylesCollectionIndex;
		readonly Dictionary<string, int> characterStylesCollectionIndex;
		readonly Dictionary<string, int> tableStylesCollectionIndex;
		readonly Dictionary<int, int> fontCharsetTable;
		readonly List<string> stylesCollection;
		readonly List<string> userCollection;
		string defaultCharacterProperties;
		string defaultParagraphProperties;
		#endregion
		public RtfExportHelper() {
			colorCollection = new ColorCollection();
			colorCollection.Add(DXColor.Empty);
			fontNamesCollection = new List<string>();
			listCollection = new Dictionary<int, string>();
			listOverrideCollectionIndex = new Dictionary<int, int>();
			listOverrideCollection = new List<string>();
			defaultFontName = RichEditControlCompatibility.DefaultFontName;
			defaultFontIndex = GetFontNameIndex(defaultFontName);
			paragraphStylesCollectionIndex = new Dictionary<string, int>();
			characterStylesCollectionIndex = new Dictionary<string, int>();
			tableStylesCollectionIndex = new Dictionary<string, int>();
			fontCharsetTable = new Dictionary<int, int>();
			stylesCollection = new List<string>();
			userCollection = new List<string>();
		}
		#region Properties
		public ColorCollection ColorCollection { get { return colorCollection; } }
		public List<string> FontNamesCollection { get { return fontNamesCollection; } }
		public Dictionary<int, string> ListCollection { get { return listCollection; } }
		public Dictionary<int, int> ListOverrideCollectionIndex { get { return listOverrideCollectionIndex; } }
		public List<string> ListOverrideCollection { get { return listOverrideCollection; } }
		public Dictionary<string, int> ParagraphStylesCollectionIndex { get { return paragraphStylesCollectionIndex; } }
		public Dictionary<string, int> CharacterStylesCollectionIndex { get { return characterStylesCollectionIndex; } }
		public Dictionary<string, int> TableStylesCollectionIndex { get { return tableStylesCollectionIndex; } }
		public Dictionary<int, int> FontCharsetTable { get { return fontCharsetTable; } }
		public List<string> StylesCollection { get { return stylesCollection; } }
		public List<string> UserCollection { get { return userCollection; } }
		public bool SupportStyle { get { return true; } }
		public int DefaultFontIndex { get { return defaultFontIndex; } }
		public string DefaultCharacterProperties { get { return defaultCharacterProperties; } set { defaultCharacterProperties = value; } }
		public string DefaultParagraphProperties { get { return defaultParagraphProperties; } set { defaultParagraphProperties = value; } }
		#endregion
		#region IRtfExportHelper Members
		public int GetFontNameIndex(string fontName) {
			int fontIndex = fontNamesCollection.IndexOf(fontName);
			if (fontIndex >= 0)
				return fontIndex;
			fontNamesCollection.Add(fontName);
			return fontNamesCollection.Count - 1;
		}
		public int GetColorIndex(Color color) {
			if ((color == DXColor.Transparent) || (color == DXColor.Empty))
				return 0;
#if !SL
			if (color.IsNamedColor)
				color = Color.FromArgb(color.R, color.G, color.B);
#endif
			int colorIndex = colorCollection.IndexOf(color);
			if (colorIndex < 0) {
				colorIndex = colorCollection.Count;
				colorCollection.Add(color);
			}
			return colorIndex;
		}
		public Color BlendColor(Color c) {
			return DXColor.Blend(c, DXColor.White);
		}
		public int GetUserIndex(RangePermission rangePermission) {
			int index = UserCollection.IndexOf(rangePermission.UserName);
			if (index >= 0)
				return index + 1;
			Dictionary<int, string> predefinedUserGroups = RtfContentExporter.PredefinedUserGroups;
			foreach (int id in predefinedUserGroups.Keys) {
				if (predefinedUserGroups[id] == rangePermission.Group)
					return id;
			}
			return 0;
		}
		#endregion
	}
	#endregion
}
