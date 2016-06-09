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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region TabsController
	public class TabsController {
		static char[] defaultDecimalChars = new char[] { '.', ','};
		#region Fields
		Stack<int> tabStack = new Stack<int>(2);
		BoxCollection boxes;
		int columnLeft;
		int paragraphRight;
		DocumentModel documentModel;
		PieceTable pieceTable;
		TabFormattingInfo tabs;
		Paragraph paragraph;
		#endregion
		#region Properties
		public int ColumnLeft { get { return columnLeft; } set { columnLeft = value; } }
		public int ParagraphRight { get { return paragraphRight; } set { paragraphRight = value; } }
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } internal set { pieceTable = value; } }
		public TabFormattingInfo Tabs { get { return tabs; } internal set { tabs = value; } }
		public int TabsCount { get { return Tabs != null ? Tabs.Count : 0; } }
		public TabInfo this[int index] {
			get {
				if (index < 0 || index >= TabsCount)
					Exceptions.ThrowArgumentException("index", index);
				return Tabs[index];
			}
		}
		public bool SingleDecimalTabInTable { get; private set; }
		internal bool SingleDecimalTab { get { return Tabs != null && Tabs.Count == 1 && Tabs[0].Alignment == TabAlignmentType.Decimal; } }
		#endregion
		public void ClearLastTabPosition() {
			tabStack.Clear();
		}
		public void UpdateLastTabPosition(int boxesCount) {
			if (tabStack.Count > 0 && tabStack.Peek() >= boxesCount)
				tabStack.Pop();
		}
		public void SaveLastTabPosition(int pos) {
			tabStack.Push(pos);
		}
		public int CalcLastTabWidth(Row row, BoxMeasurer measurer) {
			if (tabStack.Count <= 0)
				return 0;
			this.boxes = row.Boxes;
			int index = tabStack.Pop();
			TabSpaceBox box = index >= 0 ? (TabSpaceBox)boxes[index] : null;
			TabInfo tab = box != null ? box.TabInfo : tabs[0];
			int tabWidth;
			switch (tab.Alignment) {
				default:
				case TabAlignmentType.Left:
					return 0;
				case TabAlignmentType.Right:
					tabWidth = CalcLastRightTabWidth(tab, index);
					break;
				case TabAlignmentType.Center:
					tabWidth = CalcLastCenterTabWidth(tab, index);
					break;
				case TabAlignmentType.Decimal:
					tabWidth = CalcLastDecimalTabWidth(tab, index, measurer, row.Bounds.Left);
					break;
			}
			if (box != null) {
				Rectangle r = box.Bounds;
				r.Width = tabWidth;
				box.Bounds = r;
				box.LeaderCount = CalculateLeaderCount(box, this.pieceTable);
				CalculatePreciselyLeaderCount(box, this.pieceTable, measurer);
			}
			else {
				row.TextOffset += tabWidth;
			}
			return tabWidth;
		}
		internal static int CalculateLeaderCount(TabSpaceBox box, PieceTable pieceTable) {
			if(box.TabInfo.Leader == TabLeaderType.None)
				return -1;
			int characterWidth = GetTabLeaderCharacterWidth(box, pieceTable);
			int count = box.Bounds.Width / Math.Max(1, characterWidth);
			return count;
		}
		internal static void CalculatePreciselyLeaderCount(TabSpaceBox box, PieceTable pieceTable, BoxMeasurer measurer) {
			TextViewInfo textViewInfo = CacheLeader(box, pieceTable, measurer);
			int characterWidth = GetTabLeaderCharacterWidth(box, pieceTable);
			if (textViewInfo != null && Math.Abs(textViewInfo.Size.Width - box.Bounds.Width) > characterWidth && textViewInfo.Size.Width > 0) {
				box.LeaderCount = (box.LeaderCount * box.Bounds.Width) / textViewInfo.Size.Width;
				CacheLeader(box, pieceTable, measurer);
			}
		}
		internal static int GetTabLeaderCharacterWidth(TabSpaceBox box, PieceTable pieceTable) {
			DevExpress.Office.Drawing.FontInfo fontInfo = box.GetFontInfo(pieceTable);
			switch(box.TabInfo.Leader) {
				default:
				case TabLeaderType.Dots:
					return fontInfo.DotWidth;
				case TabLeaderType.MiddleDots:
					return fontInfo.MiddleDotWidth;
				case TabLeaderType.Hyphens:
					return fontInfo.DashWidth;
				case TabLeaderType.EqualSign:
					return fontInfo.EqualSignWidth;
				case TabLeaderType.ThickLine:
				case TabLeaderType.Underline:
					return fontInfo.UnderscoreWidth;
			}
		}
		internal static TextViewInfo CacheLeader(TabSpaceBox box, PieceTable pieceTable, BoxMeasurer measurer) {
			try {
				if(box.LeaderCount > 0) {
					string text = new string(GetTabLeaderCharacter(box.TabInfo.Leader), box.LeaderCount);
					DevExpress.Office.Drawing.FontInfo fontInfo = box.GetFontInfo(pieceTable);
					TextViewInfo textInfo = measurer.TextViewInfoCache.TryGetTextViewInfo(text, fontInfo);
					if(object.ReferenceEquals(textInfo, null)) {
						textInfo = measurer.CreateTextViewInfo(null, text, fontInfo);
						measurer.TextViewInfoCache.AddTextViewInfo(text, fontInfo, textInfo);
					}
					return textInfo;
				}
				return null;
			}
#if DEBUG || DEBUGTEST
			catch(Exception e) { 
				Debug.WriteLine("TabsController.CacheLeader failed: {0}", e.Message);
			}
#else
			catch { }
#endif
			return null;
		}
		internal static char GetTabLeaderCharacter(TabLeaderType leaderType) {
			switch(leaderType) {
				default:
				case TabLeaderType.Dots:
					return Characters.Dot;
				case TabLeaderType.MiddleDots:
					return Characters.MiddleDot;
				case TabLeaderType.Hyphens:
					return Characters.Dash;
				case TabLeaderType.EqualSign:
					return Characters.EqualSign;
				case TabLeaderType.ThickLine:
				case TabLeaderType.Underline:
					return Characters.Underscore;
			}
		}
		public void BeginParagraph(Paragraph paragraph) {
			this.paragraph = paragraph;
			this.pieceTable = paragraph.PieceTable;
			this.documentModel = paragraph.DocumentModel;
			ClearLastTabPosition();
			ObtainTabs();
		}
		void ObtainTabs() {
			if (paragraph.FirstLineIndentType == ParagraphFirstLineIndent.Hanging && !paragraph.IsInList()) {
				TabFormattingInfo automaticTabAtHangingIndent = new TabFormattingInfo();
				automaticTabAtHangingIndent.Add(new DefaultTabInfo(paragraph.LeftIndent));
				tabs = TabFormattingInfo.Merge(paragraph.GetTabs(), automaticTabAtHangingIndent);
			}
			else
				tabs = paragraph.GetTabs();
			if (tabs.Count == 1 && tabs[0].Alignment == TabAlignmentType.Decimal && paragraph.IsInCell())
				SingleDecimalTabInTable = true;
			else
				SingleDecimalTabInTable = false;
		}
		public TabInfo GetNextTab(int pos) {
			TabInfo tab = tabs.FindNextTab(pos);
			if (tab != null)
				return tab;
			int defaultTabWidth = documentModel.DocumentProperties.DefaultTabWidth;
			if (defaultTabWidth <= 0)
				return new DefaultTabInfo(pos);
			int tabNumber = (pos / defaultTabWidth) + 1;
			return new DefaultTabInfo(tabNumber * defaultTabWidth);
		}
		int CalcLastRightTabWidth(TabInfo tab, int startIndex) {
			int lastNonSpaceBoxIndex = Row.FindLastNonSpaceBoxIndex(boxes, startIndex);
			int tabPosition = tab.GetLayoutPosition(documentModel.ToDocumentLayoutUnitConverter);
			return Math.Max(0, (columnLeft + tabPosition) - boxes[lastNonSpaceBoxIndex].Bounds.Right);
		}
		int CalcLastCenterTabWidth(TabInfo tab, int startIndex) {
			int lastNonSpaceBoxIndex = Row.FindLastNonSpaceBoxIndex(boxes, startIndex);
			int tabbedContentWidth = boxes[lastNonSpaceBoxIndex].Bounds.Right - boxes[startIndex].Bounds.Left;
			int lastNonSpaceBoxRight = boxes[lastNonSpaceBoxIndex].Bounds.Right;
			int tabPosition = tab.GetLayoutPosition(documentModel.ToDocumentLayoutUnitConverter);
			int tabWidth = tabbedContentWidth / 2 + (columnLeft + tabPosition) - lastNonSpaceBoxRight;
			tabWidth = AdjustAlignedTabWidth(tab, tabWidth, lastNonSpaceBoxRight);
			return Math.Max(0, tabWidth);
		}
		int CalcLastDecimalTabWidth(TabInfo tab, int startIndex, BoxMeasurer measurer, int rowLeft) {
			int lastNonSpaceBoxIndex = Row.FindLastNonSpaceBoxIndex(boxes, startIndex);
			int decimalPointPosition = FindDecimalPointPosition(boxes, startIndex + 1, lastNonSpaceBoxIndex, measurer, rowLeft);
			int lastNonSpaceBoxRight = lastNonSpaceBoxIndex >= 0 ? boxes[lastNonSpaceBoxIndex].Bounds.Right : rowLeft;
			int tabPosition = tab.GetLayoutPosition(documentModel.ToDocumentLayoutUnitConverter);
			int tabWidth = (columnLeft + tabPosition) - decimalPointPosition;
			tabWidth = AdjustAlignedTabWidth(tab, tabWidth, lastNonSpaceBoxRight);
			return Math.Max(0, tabWidth);
		}
		int AdjustAlignedTabWidth(TabInfo tab, int tabWidth, int lastNonSpaceBoxRight) {
			if (IsTabPositionBehindParagraphRight(tab) && lastNonSpaceBoxRight + tabWidth > paragraphRight)
				return paragraphRight - lastNonSpaceBoxRight;
			else
				return tabWidth;
		}
		bool IsTabPositionBehindParagraphRight(TabInfo tab) {
			int tabPosition = tab.GetLayoutPosition(documentModel.ToDocumentLayoutUnitConverter);
			return tabPosition + columnLeft <= paragraphRight;
		}
		int FindDecimalPointPosition(BoxCollection boxes, int from, int to, BoxMeasurer measurer, int rowLeft) {
			int boxIndex;
			int offset;
			if (!TryFindDecimalSeparator(boxes, from, to, out boxIndex, out offset))
				return to >= 0 ? boxes[to].Bounds.Right : rowLeft;
			Box box = boxes[boxIndex];
#if DEBUGTEST
			Debug.Assert(box.StartPos.RunIndex == box.EndPos.RunIndex);
#endif
			if (offset == 0)
				return box.Bounds.Left;
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = box.StartPos;
			FormatterPosition endPos = new FormatterPosition(box.EndPos.RunIndex, box.StartPos.Offset + offset - 1, 0);
			boxInfo.EndPos = endPos;
			measurer.MeasureText(boxInfo);
			return box.Bounds.Left + boxInfo.Size.Width;
		}
		bool TryFindDecimalSeparator(BoxCollection boxes, int from, int to, out int boxIndex, out int offset) {
			boxIndex = from;
			offset = -1;
			bool numberStarted = false;
			for (; boxIndex <= to; boxIndex++) {
				string text = boxes[boxIndex].GetText(pieceTable);
				int length = text.Length;
				for (offset = 0; offset < length; offset++) {
					if (IsDecimalSeparator(text, offset))
						return true;
					if (Char.IsDigit(text[offset]))
						numberStarted = true;
					else if (numberStarted && !IsGroupSeparator(text, offset))
						return true;
				}
			}
			return false;
		}
		bool IsDecimalSeparator(string source, int index) {
			return IsSubstringStarts(source, index, DocumentModel.NumberDecimalSeparator);
		}
		bool IsGroupSeparator(string source, int index) {
			return IsSubstringStarts(source, index, DocumentModel.NumberGroupSeparator);
		}
		bool IsSubstringStarts(string source, int index, string substring) {
			return String.Compare(source, index, substring, 0, substring.Length, StringComparison.OrdinalIgnoreCase) == 0;
		}
	}
	struct CharPosition {
		public int BoxIndex { get; set; }
		public int Offset { get; set; }
	}
	#endregion
}
