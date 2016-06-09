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

using System.Drawing;
using System.IO;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Layout;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region RangeMailMergeLayoutItem
	public class RangeMailMergeLayoutItem : RangeSelectionLayoutItemBase {
		const string spaces = " ";
		#region static
		const int fontSize = 9;
		protected const byte textWidthCorrection = 10;
		public const string HeaderText = "Header";
		public const string FooterText = "Footer";
		public const string DetailText = "Detail";
		public const string DetailLevelText = "DetailLevel";
		public const string GroupHeaderText = "GroupHeader";
		public const string GroupFooterText = "GroupFooter";
		#endregion
		#region fields
		string text;
		Rectangle textBounds;
		FontInfo fontInfo;
		readonly RangeMailMergeBorderSelectionLayoutItem borderItem;
		#endregion
		public RangeMailMergeLayoutItem(SelectionLayout layout, CellRangeBase range, string text, string mailMergeDefinedName)
			: base(layout, range.TopLeft, range.BottomRight) {
			this.text = text;
			borderItem = new RangeMailMergeBorderSelectionLayoutItem(layout, range.TopLeft, range.BottomRight, mailMergeDefinedName);
		}
		#region Properties
		public string Text { get { return spaces + text; } }
		public Rectangle TextBounds { get { return textBounds; } }
		public FontInfo FontInfo { get { return fontInfo; } }
		public RangeMailMergeBorderSelectionLayoutItem BorderItem { get { return borderItem; } }
		bool IsFooter { get { return text == FooterText; } }
		#endregion
		public override void Draw(ISelectionPainter selectionPainter) {
			selectionPainter.Draw(this);
		}
		public override void Update(Page page) {
			base.Update(page);
			textBounds = GetTextBounds(GetTextSize(page.Sheet.Workbook));
			borderItem.Update(page);
		}
		protected virtual Rectangle GetTextBounds(Size textSize) {
			if (IsFooter)
				return Rectangle.FromLTRB(Bounds.Right - textSize.Width - textWidthCorrection, Bounds.Bottom - textSize.Height, Bounds.Right, Bounds.Bottom);
			else
				return Rectangle.FromLTRB(Bounds.Right - textSize.Width - textWidthCorrection, Bounds.Top, Bounds.Right, Bounds.Top + textSize.Height);
		}
		Size GetTextSize(DocumentModel workbook) {
			FontCache fontCache = workbook.FontCache;
			fontInfo = fontCache[fontCache.CalcFontIndex("Segoe UI", fontSize * 2, false, false, Office.CharacterFormattingScript.Normal, false, false)];
			return fontCache.Measurer.MeasureString(Text, fontInfo);
		}
	}
	#endregion
}
