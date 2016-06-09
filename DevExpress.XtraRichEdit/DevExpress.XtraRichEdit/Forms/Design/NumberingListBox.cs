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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Design {
	#region NumberingListViewInfo
	public class NumberingListViewInfo : DevExpress.XtraEditors.ViewInfo.BaseListBoxViewInfo {
		public NumberingListViewInfo(BaseListBoxControl owner)
			: base(owner) {
		}
		protected override bool CalcVScrollVisibility(Rectangle bounds) {
			return true;
		}
	}
	#endregion
	#region NumberingListBox
	[DXToolboxItem(false)]
	public class NumberingListBox : ListBoxControl {
		#region Fields
		static string CaptionNone = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_NumberingListBoxNone);
		const int focusRectangleMargin = 1;
		const int selectRectangleMargin = 3;
		const int selectRectangleThickness = 2;
		const int indentFromEdges = 1;
		int columnWidth;
		const int columnCount = 4;
		const int rowCount = 2;
		const int itemPadding = 10;
		int selectedIndex;
		int selectRectX = indentFromEdges;
		int selectRectY = indentFromEdges;
		List<SimpleRichEditControl> controlsList;
		internal bool isMouseClickInItems;
		int levelIndex;
		bool isMouseDown;
		const int dpiX = 96;  
		const int dpiY = 96;  
		#endregion
		public NumberingListBox()
			: base() {
			this.levelIndex = 0;
			this.controlsList = new List<SimpleRichEditControl>();
		}
		#region Properties
		protected internal int ScrollValue { get { return ScrollInfo.VScroll.Value; } set { ScrollInfo.VScroll.Value = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int SelectedIndex {
			get {
				return selectedIndex;
			}
			set {
				if (selectedIndex < -1 || selectedIndex > controlsList.Count - 1)
					Exceptions.ThrowArgumentException("SelectedIndexNumberingListBox", selectedIndex);
				selectedIndex = value;
				UpdateSelectedRectCoordinate(value);
				RaiseNumberingListBoxSelectedIndexChanged();
				Refresh();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AbstractNumberingList SelectedAbstractList {
			get {
				return selectedIndex < controlsList.Count ? controlsList[selectedIndex].AbstractList : null;
			}
			set {
				SelectedIndex = GetSelectedListIndex(value);
			}
		}
		#endregion
		#region Events
		static readonly object selectedIndexChanged = new object();
		public new event EventHandler SelectedIndexChanged {
			add { Events.AddHandler(selectedIndexChanged, value); }
			remove { Events.RemoveHandler(selectedIndexChanged, value); }
		}
		protected internal virtual void RaiseNumberingListBoxSelectedIndexChanged() {
			EventHandler handler = (EventHandler)this.Events[selectedIndexChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		protected override void OnMouseWheelCore(MouseEventArgs e) {
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			isMouseDown = false;
		}
		protected internal override void SetTopIndexCore(int value) {
			if (!isMouseDown)
				base.SetTopIndexCore(value);
		}
		protected virtual int GetSelectedListIndex(AbstractNumberingList value) {
			if (value == null)
				return -1;
			int count = controlsList.Count;
			for (int i = 0; i < count; i++) {
				if (controlsList[i].AbstractList.IsEqual(value))
					return i;
			}
			Debug.Assert(false);
			return -1;
		}
		void UpdateSelectedRectCoordinate(int selectedIndex) {
			int controlsCount = controlsList.Count;
			if (controlsCount == 0)
				return;
			if (selectedIndex == -1) {
				selectRectX = -columnWidth;
				selectRectY = -ItemHeight;
				return;
			}
			base.SelectedIndex = selectedIndex / columnCount;
			if (base.SelectedIndex < ScrollValue)
				ScrollValue = base.SelectedIndex;
			if (base.SelectedIndex >= rowCount)
				ScrollValue = base.SelectedIndex - rowCount + 1;
			selectRectX = (selectedIndex % columnCount) * columnWidth + indentFromEdges;
			selectRectY = (base.SelectedIndex - ScrollValue) * ItemHeight + indentFromEdges;
		}
		protected internal virtual List<SimpleRichEditControl> GetControls() {
			return controlsList;
		}
		protected internal virtual void InitializeControl(RichEditControl control, NumberingType numberingType, int levelIndex) {
			Guard.ArgumentNotNull(control, "control");
			if (levelIndex == -1)
				this.levelIndex = 0;
			else
				this.levelIndex = levelIndex;
			columnWidth = CalculateWidthColumn();
			ItemHeight = CalculateItemHeight();
			controlsList.Clear();
			AddNoneItem();
			InitializeControlCore(control, numberingType, control.DocumentModel);
			InitializeControlCore(control, numberingType, control.InnerControl.DocumentModelTemplate);
			UpdateItems();
			SubscribeControlEvents();
			Refresh();
		}
		private void AddNoneItem() {
			SimpleRichEditControl simpleControl = CreateSimpleControlNoneText();
			controlsList.Add(simpleControl);
		}
		protected internal virtual void InitializeControlCore(RichEditControl control, NumberingType numberingType, DocumentModel sourceModel) {
			AbstractNumberingListCollection sourceLists = sourceModel.AbstractNumberingLists;
			AbstractNumberingListIndex count = new AbstractNumberingListIndex(sourceLists.Count);
			for (AbstractNumberingListIndex i = new AbstractNumberingListIndex(0); i < count; i++) {
				AbstractNumberingList abstractList = sourceLists[i];
				NumberingType listType = NumberingListHelper.GetListType(abstractList);
				if (numberingType != listType)
					continue;
				if (ContainsListWithSameProperties(abstractList))
					continue;
				SimpleRichEditControl simpleControl = CreateSimpleControl();
				simpleControl.AbstractList = abstractList;
				PrepareListPreviewContent(simpleControl.DocumentModel, 4, abstractList);
				controlsList.Add(simpleControl);
			}
		}
		protected virtual bool ContainsListWithSameProperties(AbstractNumberingList abstractList) {
			int count = controlsList.Count;
			for (int i = 0; i < count; i++) {
				AbstractNumberingList existingList = controlsList[i].AbstractList;
				if (existingList == NumberingListFormController.NoneList)
					continue;
				if (existingList.IsEqual(abstractList))
					return true;
			}
			return false;
		}
		NumberingList CreateNumberingListCopy(DocumentModel targetDocumentModel, AbstractNumberingList sourceList) {
			AbstractNumberingList abstractNumberingList = new AbstractNumberingList(targetDocumentModel);
			abstractNumberingList.CopyFrom(sourceList); 
			targetDocumentModel.AddAbstractNumberingListUsingHistory(abstractNumberingList);
			NumberingList result = new NumberingList(targetDocumentModel, new AbstractNumberingListIndex());
			targetDocumentModel.AddNumberingListUsingHistory(result);
			return result;
		}
		SimpleRichEditControl CreateSimpleControl() {
			SimpleRichEditControl result = new SimpleRichEditControl();
			result.BeginUpdate();
			result.Options.VerticalRuler.Visibility = RichEditRulerVisibility.Hidden;
			result.Options.HorizontalRuler.Visibility = RichEditRulerVisibility.Hidden;
			result.ReadOnly = true;
			result.Size = new Size(columnWidth - 2 * itemPadding, ItemHeight - 2 * itemPadding);
			result.ActiveViewType = RichEditViewType.Simple;
			result.Views.SimpleView.Padding = new Padding(0);
			result.BorderStyle = BorderStyles.NoBorder;
			result.Views.SimpleView.HidePartiallyVisibleRow = true;
			result.Options.VerticalScrollbar.Visibility = RichEditScrollbarVisibility.Hidden;
			result.Options.HorizontalScrollbar.Visibility = RichEditScrollbarVisibility.Hidden;
			result.Appearance.Text.Font = new Font("Arial", 10);
			result.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			result.EndUpdate();
			return result;
		}
		SimpleRichEditControl CreateSimpleControlNoneText() {
			SimpleRichEditControl result = CreateSimpleControl();
			result.ReadOnly = true;
			result.Size = new Size(columnWidth - 2 * itemPadding, ItemHeight - 2 * itemPadding);
			result.AbstractList = NumberingListFormController.NoneList;
			FontCache cache = result.DocumentModel.FontCache;
			PieceTable pieceTable = result.DocumentModel.MainPieceTable;
			FontInfo fontInfo = cache[pieceTable.Runs.First.FontCacheIndex];
			int textHeight = result.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(cache.Measurer.MeasureString(CaptionNone, fontInfo).Height, dpiY);
			int paddingTop = (result.Height - textHeight) / 2;
			result.Views.SimpleView.Padding = new Padding(0, paddingTop, 0, 0);
			Paragraph paragraph = pieceTable.Paragraphs[ParagraphIndex.Zero];
			paragraph.FirstLineIndent = 0;
			paragraph.LeftIndent = 0;
			paragraph.Alignment = ParagraphAlignment.Center;
			pieceTable.InsertPlainText(DocumentLogPosition.Zero, CaptionNone);
			return result;
		}
		void AddNumberingListsToParagraph(PieceTable pieceTable) {
			DocumentModel documentModel = pieceTable.DocumentModel;
			NumberingListIndex numberinglistIndex = new NumberingListIndex(documentModel.NumberingLists.Count - 1);
			IListLevel level = documentModel.NumberingLists[numberinglistIndex].Levels[levelIndex];
			AdjustLevel(level, 0);
			ParagraphCollection paragraphs = pieceTable.Paragraphs;
			ParagraphIndex count = new ParagraphIndex(paragraphs.Count);
			for (ParagraphIndex i = ParagraphIndex.Zero; i < count; i++)
				AddNumberingListLevelToParagraph(pieceTable, paragraphs[i], numberinglistIndex, levelIndex);
		}
		void AddMultiLevelNumberingListsToParagraph(PieceTable pieceTable) {
			DocumentModel documentModel = pieceTable.DocumentModel;
			NumberingListIndex numberinglistIndex = new NumberingListIndex(documentModel.NumberingLists.Count - 1);
			NumberingList list = documentModel.NumberingLists[numberinglistIndex];
			ParagraphCollection paragraphs = pieceTable.Paragraphs;
			ParagraphIndex count = new ParagraphIndex(paragraphs.Count - 1);
			for (ParagraphIndex i = ParagraphIndex.Zero; i < count; i++) {
				int levelIndex = ((IConvertToInt<ParagraphIndex>)i).ToInt();
				AdjustLevel(list.Levels[levelIndex], levelIndex);
				AddNumberingListLevelToParagraph(pieceTable, paragraphs[i], numberinglistIndex, levelIndex);
			}
			pieceTable.AddNumberingListToParagraph(paragraphs[count], numberinglistIndex, 0);
		}
		void AdjustLevel(IListLevel level, int levelIndex) {
			int levelOffset = level.DocumentModel.UnitConverter.PixelsToModelUnits(20, dpiX);
			int adjustment = CalculateLevelIndentAdjustement(level);
			level.LeftIndent -= adjustment;
			level.LeftIndent += levelIndex * levelOffset;
			level.ListLevelProperties.Alignment = ListNumberAlignment.Left;
		}
		void AddNumberingListLevelToParagraph(PieceTable pieceTable, Paragraph paragraph, NumberingListIndex numberinglistIndex, int levelIndex) {
			DocumentModel documentModel = pieceTable.DocumentModel;
			IListLevel level = documentModel.NumberingLists[numberinglistIndex].Levels[levelIndex];
			TabFormattingInfo tabs = paragraph.GetOwnTabs();
			tabs.Add(new TabInfo(level.LeftIndent));
			tabs.Add(new TabInfo(level.LeftIndent + documentModel.UnitConverter.PixelsToModelUnits(ItemHeight, dpiX)));
			paragraph.SetOwnTabs(tabs);
			pieceTable.AddNumberingListToParagraph(paragraph, numberinglistIndex, levelIndex);
		}
		int CalculateLevelIndentAdjustement(IListLevel level) {
			switch (level.FirstLineIndentType) {
				case ParagraphFirstLineIndent.Hanging:
					return level.LeftIndent - level.FirstLineIndent;
				case ParagraphFirstLineIndent.Indented:
					return level.LeftIndent;
				default:
					return 0;
			}
		}
		protected internal virtual void PrepareListPreviewContent(DocumentModel documentModel, int previewParagraphCount, AbstractNumberingList sourceList) {
			documentModel.BeginUpdate();
			try {
				CreateNumberingListCopy(documentModel, sourceList); 
				PieceTable pieceTable = documentModel.MainPieceTable;
				CreatePreviewContent(pieceTable, previewParagraphCount);
				FormatPreviewContent(pieceTable);
				documentModel.Selection.Start = DocumentLogPosition.Zero;
				documentModel.Selection.End = DocumentLogPosition.Zero;
				if (NumberingListHelper.GetListType(sourceList) == NumberingType.MultiLevel)
					AddMultiLevelNumberingListsToParagraph(pieceTable);
				else
					AddNumberingListsToParagraph(pieceTable);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal virtual void CreatePreviewContent(PieceTable pieceTable, int paragraphCount) {
			string listParagraphContent = new String(Characters.TabMark, 1) + new String(Characters.LineBreak, 1) + new String(Characters.TabMark, 1);
			pieceTable.InsertPlainText(DocumentLogPosition.Zero, listParagraphContent);
			for (int i = 1; i < paragraphCount; i++) {
				pieceTable.InsertParagraph(DocumentLogPosition.Zero);
				pieceTable.InsertPlainText(DocumentLogPosition.Zero, listParagraphContent);
			}
		}
		protected internal virtual void FormatPreviewContent(PieceTable pieceTable) {
			DocumentLogPosition from = pieceTable.DocumentStartLogPosition;
			int length = pieceTable.DocumentEndLogPosition - pieceTable.DocumentStartLogPosition;
			RunFontUnderlineTypeModifier underlineModifier = new RunFontUnderlineTypeModifier(UnderlineType.ThickSingle);
			pieceTable.ApplyCharacterFormatting(from, length, underlineModifier);
			RunUnderlineColorModifier underlineColorModifier = new RunUnderlineColorModifier(SystemColors.ControlDark);
			pieceTable.ApplyCharacterFormatting(from, length, underlineColorModifier);
		}
		protected internal virtual void UpdateItems() {
			Items.Clear();
			for (int i = 0; i < controlsList.Count; i++) {
				if (i % columnCount == 0)
					Items.Add(controlsList[i]);
			}
		}
		protected internal int CalculateWidthColumn() {
			return (Width - ScrollInfo.VScroll.Width) / columnCount;
		}
		protected internal virtual int CalculateItemHeight() {
			return Height / rowCount;
		}
		protected internal virtual void SubscribeControlEvents() {
			this.DrawItem += OnDrawItem;
			this.ScrollInfo.VScroll.Scroll += OnScroll;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			this.DrawItem -= OnDrawItem;
			this.ScrollInfo.VScroll.Scroll -= OnScroll;
		}
		void OnScroll(object sender, ScrollEventArgs e) {
			int value = (e.NewValue - ScrollValue) * columnCount;
			int newSelectedIndex = SelectedIndex + value;
			if (newSelectedIndex < 0)
				return;
			if (newSelectedIndex < controlsList.Count) {
				selectedIndex = newSelectedIndex;
				RaiseNumberingListBoxSelectedIndexChanged();
			}
			else
				selectRectY -= ItemHeight;
		}
		protected override DevExpress.XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new NumberingListViewInfo(this);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Left:
					OnKeyDownLeft();
					break;
				case Keys.Right:
					OnKeyDownRight();
					break;
				case Keys.Up:
					OnKeyDownUp();
					break;
				case Keys.Down:
					OnKeyDownDown();
					break;
				case Keys.Home:
					OnKeyDownHome();
					break;
				case Keys.End:
					OnKeyDownEnd();
					break;
				case Keys.PageUp:
					OnKeyDownPageUp();
					break;
				case Keys.PageDown:
					OnKeyDownPageDown();
					break;
			}
		}
		protected internal void OnKeyDownLeft() {
			if (SelectedIndex == 0)
				return;
			if (selectRectX > indentFromEdges)
				selectRectX -= columnWidth;
			else {
				if (selectRectY == indentFromEdges)
					ScrollValue -= 1;
				else
					selectRectY -= ItemHeight;
				selectRectX = (columnCount - 1) * columnWidth + indentFromEdges;
			}
			selectedIndex--;
			RaiseNumberingListBoxSelectedIndexChanged();
			Refresh();
		}
		protected internal void OnKeyDownRight() {
			if (SelectedIndex == controlsList.Count - 1)
				return;
			if (selectRectX < columnWidth * (columnCount - 1))
				selectRectX += columnWidth;
			else {
				if (selectRectY >= columnWidth * (rowCount - 1))
					ScrollValue += 1;
				else
					selectRectY += ItemHeight;
				selectRectX = indentFromEdges;
			}
			selectedIndex++;
			RaiseNumberingListBoxSelectedIndexChanged();
			Refresh();
		}
		protected internal void OnKeyDownUp() {
			int index = SelectedIndex - columnCount;
			if (index < 0)
				return;
			if (selectRectY == indentFromEdges)
				ScrollValue -= 1;
			else
				selectRectY -= ItemHeight;
			selectedIndex -= columnCount;
			RaiseNumberingListBoxSelectedIndexChanged();
			Refresh();
		}
		protected internal void OnKeyDownDown() {
			int newSelectedIndex = SelectedIndex + columnCount;
			if (newSelectedIndex >= controlsList.Count)
				return;
			if (selectRectY >= ItemHeight * (rowCount - 1))
				ScrollValue += 1;
			else
				selectRectY += ItemHeight;
			selectedIndex = newSelectedIndex;
			RaiseNumberingListBoxSelectedIndexChanged();
			Refresh();
		}
		protected internal void OnKeyDownHome() {
			SelectedIndex = 0;
		}
		protected internal void OnKeyDownEnd() {
			SelectedIndex = controlsList.Count - 1;
		}
		protected internal void OnKeyDownPageUp() {
			if (ScrollValue == 0)
				return;
			if (ScrollValue - rowCount >= 0) {
				selectedIndex -= rowCount * columnCount;
				ScrollValue -= rowCount;
			}
			else {
				selectedIndex -= ScrollValue * columnCount;
				ScrollValue = 0;
			}
			RaiseNumberingListBoxSelectedIndexChanged();
		}
		protected internal void OnKeyDownPageDown() {
			if (Items.Count < rowCount)
				return;
			int value = rowCount * columnCount;
			int newSelectedIndex = SelectedIndex + value;
			int deltaScroll = rowCount;
			int bottomBorder = ScrollValue + 2 * rowCount;
			if (bottomBorder > Items.Count) {
				int deltaColumn = bottomBorder - Items.Count;
				newSelectedIndex -= deltaColumn * columnCount;
				deltaScroll -= deltaColumn;
			}
			if (newSelectedIndex < controlsList.Count) {
				ScrollValue += deltaScroll;
				selectedIndex = newSelectedIndex;
				RaiseNumberingListBoxSelectedIndexChanged();
			}
		}
		private void OnDrawItem(object sender, ListBoxDrawItemEventArgs e) {
			int x = indentFromEdges;
			int index = e.Index * columnCount;
			for (int i = 0; i < columnCount; i++) {
				if (index < controlsList.Count) {
					DrawControl(e.Graphics, controlsList[index], x, e.Bounds.Y);
					x += columnWidth;
				}
				index++;
			}
			Color backColor = ViewInfo.GetSystemColor(SystemColors.Highlight);
			if (Focused)
				DrawFocusRectangle(e.Cache.Graphics, backColor);
			DrawSelectRectangle(e.Cache, backColor);
			e.Handled = true;
		}
		void DrawFocusRectangle(Graphics graphics, Color backColor) {
			int boundsWidth = columnWidth - (focusRectangleMargin * 2 + 1);
			int boundsHeight = ItemHeight - (focusRectangleMargin * 2 + 1);
			Rectangle bounds = new Rectangle(selectRectX + focusRectangleMargin, selectRectY + focusRectangleMargin, boundsWidth, boundsHeight);
			ControlPaint.DrawFocusRectangle(graphics, bounds, Color.Empty, backColor);
		}
		void DrawSelectRectangle(GraphicsCache cache, Color backColor) {
			int selectRectWidth = columnWidth - (selectRectangleMargin * 2);
			int selectRectHeight = ItemHeight - (selectRectangleMargin * 2);
			Rectangle drawRect = new Rectangle(selectRectX + selectRectangleMargin, selectRectY + selectRectangleMargin, selectRectWidth, selectRectHeight);
			cache.DrawRectangle(cache.GetPen(backColor, selectRectangleThickness), drawRect);
		}
		protected internal virtual void DrawControl(Graphics graphics, SimpleRichEditControl control, int x, int y) {
			Bitmap bmp = new Bitmap(control.Width, control.Height);
			control.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
			graphics.DrawImage(bmp, new Rectangle(x + itemPadding, y + itemPadding, bmp.Width, bmp.Height));
			ControlPaint.DrawBorder3D(graphics, new Rectangle(x, y, columnWidth, ItemHeight));
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			isMouseDown = true;
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Left)
				UpdateSelectIndexAfterMouseDown(e.X, e.Y);
		}
		protected internal virtual void UpdateSelectIndexAfterMouseDown(int mouseClickX, int mouseClickY) {
			isMouseClickInItems = false;
			int x = indentFromEdges;
			int y = indentFromEdges - ItemHeight;
			int visibleItemIndex = ScrollValue * columnCount;
			for (int i = visibleItemIndex; i < visibleItemIndex + columnCount * rowCount; i++) {
				if (i == controlsList.Count)
					return;
				if (i % columnCount == 0) {
					x = indentFromEdges;
					y += ItemHeight;
				}
				Rectangle rect = new Rectangle(x, y, columnWidth, ItemHeight);
				if (rect.Contains(mouseClickX, mouseClickY)) {
					selectRectX = x;
					selectRectY = y;
					int numberSelectedColumn = i % columnCount;
					selectedIndex = (base.SelectedIndex * columnCount) + numberSelectedColumn;
					isMouseClickInItems = true;
					RaiseNumberingListBoxSelectedIndexChanged();
					Refresh();
					return;
				}
				x += columnWidth;
			}
		}
	}
	#endregion
	#region SimpleSymbolListBox
	[DXToolboxItem(false)]
	public class SimpleSymbolListBox : ListBoxControl {
		const int selectRectangleMargin = 4;
		const int focusRectangleMargin = 2;
		const int selectRectangleThickness = 2;
		const int fontSize = 17;
		string fontName;
		internal bool isActive;
		StringFormat stringFormat;
		Font font;
		public SimpleSymbolListBox()
			: base() {
			this.fontName = String.Empty;
			this.font = new Font(String.Empty, fontSize);
			this.DrawItem += new ListBoxDrawItemEventHandler(OnDrawItem);
			InitializeStringFormat();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string FontName {
			get {
				return fontName;
			}
			set {
				fontName = value;
				RecreateFont();
			}
		}
		void RecreateFont() {
			if (this.font != null) {
				this.font.Dispose();
				this.font = null;
			}
			this.font = new Font(FontName, fontSize);
		}
		protected internal virtual void InitializeStringFormat() {
			this.stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
			this.stringFormat.Alignment = StringAlignment.Center;
			this.stringFormat.LineAlignment = StringAlignment.Center;
		}
		void OnDrawItem(object sender, ListBoxDrawItemEventArgs e) {
			Brush brush = e.Cache.GetSolidBrush(ViewInfo.GetSystemColor(ViewInfo.PaintAppearance.ForeColor));
			e.Graphics.DrawString(e.Item.ToString(), font, brush, new Rectangle(0, 0, Width, Height), stringFormat);
			Color backColor = ViewInfo.GetSystemColor(SystemColors.Highlight);
			if (Focused)
				DrawFocusRectangle(e.Cache.Graphics, backColor);
			if (isActive)
				DrawSelectRectangle(e.Cache, backColor);
			e.Handled = true;
		}
		void DrawFocusRectangle(Graphics graphics , Color backColor) {
			int focusRectWidth = Width - 4;
			int focusRectHeight = Height - 4;
			Rectangle bounds = new Rectangle(focusRectangleMargin, focusRectangleMargin, focusRectWidth, focusRectHeight);
			ControlPaint.DrawFocusRectangle(graphics, bounds, Color.Empty, backColor);
		}
		void DrawSelectRectangle(GraphicsCache cache, Color backColor) {
			int selectRectWidth = Width - selectRectangleMargin * 2 + 1;
			int selectRectHeight = Height - selectRectangleMargin * 2 + 1;
			Rectangle rect = new Rectangle(selectRectangleMargin, selectRectangleMargin, selectRectWidth, selectRectHeight);
			cache.DrawRectangle(cache.GetPen(backColor, selectRectangleThickness), rect);
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (stringFormat != null) {
						stringFormat.Dispose();
						stringFormat = null;
					}
					if (font != null) {
						font.Dispose();
						font = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
	}
	#endregion
	#region SymbolListBox
	[DXToolboxItem(false)]
	public class SymbolListBox : ListBoxControl {
		#region Fields
		internal List<char> unicodeChars;
		int columnCount;
		int rowCount;
		string fontName;
		int selectRectX = 0;
		int selectRectY = 0;
		int selectedIndexSymbolListBox;
		internal bool isMouseClickInItems;
		StringFormat stringFormat;
		Font font;
		bool isMouseDown;
		#endregion
		public SymbolListBox()
			: base() {
			SubscribeControlEvents();
			this.columnCount = 1000;
			this.unicodeChars = GetCharsContainsInFont(this.Appearance.Font.Name);
			this.FontName = this.Appearance.Font.Name;
			InitializeStringFormat();
		}
		#region Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override int ItemHeight { get { return base.ItemHeight; } set {} }
		bool ShouldSerializeItemHeight() { return false; }
		protected internal int ScrollValue { get { return ScrollInfo.VScroll.Value; } set { ScrollInfo.VScroll.Value = value; } }
		protected internal new int ColumnWidth { get { return ItemHeight; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string FontName {
			get { return fontName; }
			set {
				if (fontName == value)
					return;
				fontName = value;
				RecreateFont();
				char oldChar = GetSelectedChar();
				unicodeChars = GetCharsContainsInFont(font.Name);
				CalculateColumnCount();
				CalculateRowCount();
				InitializeItems();
				SetSelectedChar(oldChar);
			}
		}
		public int SelectedIndexSymbolListBox {
			get {
				return selectedIndexSymbolListBox;
			}
			set {
				selectedIndexSymbolListBox = value;
				UpdateSelectedRectCoordinate(selectedIndexSymbolListBox);
				RaiseSelectedIndexSymbolListBoxChanged();
				Refresh();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Localizable(false)]
		public override ListBoxItemCollection Items { get { return base.Items; } }
		protected internal virtual bool ShouldSerializeItems() {
			return false;
		}
		protected internal virtual void ResetItems() {
			Items.Clear();
		}
		#endregion
		#region Events
		static readonly object selectedIndexChanged = new object();
		public event EventHandler SelectedIndexSymbolListBoxChanged {
			add { Events.AddHandler(selectedIndexChanged, value); }
			remove { Events.RemoveHandler(selectedIndexChanged, value); }
		}
		protected internal virtual void RaiseSelectedIndexSymbolListBoxChanged() {
			EventHandler handler = (EventHandler)this.Events[selectedIndexChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (font != null) {
					font.Dispose();
					font = null;
				}
				if (stringFormat != null) {
					stringFormat.Dispose();
					stringFormat = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			CalculateColumnCount();
			CalculateRowCount();
			InitializeItems();
		}
		void RecreateFont() {
			if (this.font != null) {
				this.font.Dispose();
				this.font = null;
			}
			Font appearanceFont = Appearance.Font;
			this.font = new Font(FontName, appearanceFont.Size, appearanceFont.Style);
			base.ItemHeight = font.Height * 3 / 2;
		}
		protected internal override void SetTopIndexCore(int value) {
			if (!isMouseDown)
				base.SetTopIndexCore(value);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			isMouseDown = false;
		}
		protected override DevExpress.XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new NumberingListViewInfo(this);
		}
		protected internal virtual void InitializeStringFormat() {
			this.stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
			this.stringFormat.Alignment = StringAlignment.Center;
			this.stringFormat.LineAlignment = StringAlignment.Center;
		}
		protected internal virtual void UpdateSelectedRectCoordinate(int selectedIndex) {
			int countChars = unicodeChars.Count;
			if (countChars == 0)
				return;
			if (selectedIndex < 0 || selectedIndex > countChars - 1)
				Exceptions.ThrowArgumentException("SelectedIndexSymbolListBox", selectedIndex);
			SelectedIndex = selectedIndex / columnCount;
			if (SelectedIndex < ScrollValue)
				ScrollValue = SelectedIndex;
			if (SelectedIndex >= rowCount)
				ScrollValue = SelectedIndex - rowCount + 1;
			selectRectX = (selectedIndex % columnCount) * ColumnWidth;
			selectRectY = (SelectedIndex - ScrollValue) * ColumnWidth;
		}
		protected internal virtual List<char> GetCharsContainsInFont(string fontName) {
			List<char> result = new List<char>();
			using (FontCache cache = new GdiPlusFontCache(new DocumentLayoutUnitDocumentConverter())) {
				FontCharacterSet sourceFontCharacterSet = cache.GetFontCharacterSet(fontName);
				if (sourceFontCharacterSet == null)
					return result;
				for (int i = 0; i < UInt16.MaxValue; i++) {
					char character = (char)i;
					bool isCharCategoryControl = Char.IsControl(character);
					bool isCharCategoryPrivateUse = Char.GetUnicodeCategory(character) == UnicodeCategory.PrivateUse;
					if (sourceFontCharacterSet.ContainsChar(character) && !isCharCategoryControl && !isCharCategoryPrivateUse)
						result.Add(character);
				}
				return result;
			}
		}
		protected internal virtual void OnSizeChanged(object sender, EventArgs e) {
			CalculateColumnCount();
			CalculateRowCount();
			InitializeItems();
			UpdateSelectedRectCoordinate(selectedIndexSymbolListBox);
		}
		protected internal virtual void SubscribeControlEvents() {
			this.DrawItem += new ListBoxDrawItemEventHandler(OnDrawItem);
			this.SizeChanged += new EventHandler(OnSizeChanged);
			this.ScrollInfo.VScroll_Scroll += new ScrollEventHandler(OnScroll);
			this.MeasureItem += new MeasureItemEventHandler(OnMeasureItem);
			this.Appearance.Changed += OnAppearanceChanged;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			this.DrawItem -= new ListBoxDrawItemEventHandler(OnDrawItem);
			this.SizeChanged -= new EventHandler(OnSizeChanged);
			this.ScrollInfo.VScroll_Scroll -= new ScrollEventHandler(OnScroll);
			this.MeasureItem -= new MeasureItemEventHandler(OnMeasureItem);
			this.Appearance.Changed -= OnAppearanceChanged;
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			RecreateFont();
		}
		void OnScroll(object sender, ScrollEventArgs e) {
			UpdateSelectedIndex(e.NewValue);
		}
		protected internal virtual void UpdateSelectedIndex(int newScrollValue) {
			int deltaScroll = (newScrollValue - ScrollValue) * columnCount;
			int newSelectedIndex = selectedIndexSymbolListBox + deltaScroll;
			if (newSelectedIndex < 0)
				return;
			if (newSelectedIndex < unicodeChars.Count) {
				selectedIndexSymbolListBox = newSelectedIndex;
				RaiseSelectedIndexSymbolListBoxChanged();
			}
			else
				selectRectY -= ColumnWidth;
		}
		protected internal virtual void InitializeItems() {
			int countChars = unicodeChars.Count;
			Items.BeginUpdate();
			Items.Clear();
			for (int i = 0; i < countChars; i += columnCount) {
				Items.Add(unicodeChars[i]);
			}
			Items.EndUpdate();
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			SetSelectedChar(e.KeyChar);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Left:
					OnKeyDownLeft();
					break;
				case Keys.Right:
					OnKeyDownRight();
					break;
				case Keys.Up:
					OnKeyDownUp();
					break;
				case Keys.Down:
					OnKeyDownDown();
					break;
				case Keys.Home:
					OnKeyDownHome();
					break;
				case Keys.End:
					OnKeyDownEnd();
					break;
				case Keys.PageUp:
					OnKeyDownPageUp();
					break;
				case Keys.PageDown:
					OnKeyDownPageDown();
					break;
			}
		}
		protected internal void OnKeyDownLeft() {
			if (SelectedIndexSymbolListBox == 0)
				return;
			if (selectRectX > 0)
				selectRectX -= ColumnWidth;
			else  {
				if (selectRectY == 0)
					ScrollValue -= 1;
				else
					selectRectY -= ColumnWidth;
				selectRectX = (columnCount - 1) * ColumnWidth;
			}
			selectedIndexSymbolListBox -= 1;
			RaiseSelectedIndexSymbolListBoxChanged();
			Refresh();
		}
		protected internal void OnKeyDownRight() {
			if (SelectedIndexSymbolListBox == unicodeChars.Count - 1)
				return;
			if (selectRectX < ColumnWidth * (columnCount - 1))
				selectRectX += ColumnWidth;
			else {
				if (selectRectY >= ColumnWidth * (rowCount - 1))
					ScrollValue += 1;
				else
					selectRectY += ColumnWidth;
				selectRectX = 0;
			}
			selectedIndexSymbolListBox += 1;
			RaiseSelectedIndexSymbolListBoxChanged();
			Refresh();
		}
		protected internal void OnKeyDownUp() {
			int index = selectedIndexSymbolListBox - columnCount;
			if (index < 0)
				return;
			if (selectRectY == 0)
				ScrollValue -= 1;
			else
				selectRectY -= ItemHeight;
			selectedIndexSymbolListBox -= columnCount;
			RaiseSelectedIndexSymbolListBoxChanged();
			Refresh();
		}
		protected internal void OnKeyDownDown() {
			int newSelectedIndex = selectedIndexSymbolListBox + columnCount;
			if (newSelectedIndex >= unicodeChars.Count)
				return;
			if (selectRectY >= ItemHeight * (rowCount - 1))
				ScrollValue += 1;
			else
				selectRectY += ItemHeight;
			selectedIndexSymbolListBox = newSelectedIndex;
			RaiseSelectedIndexSymbolListBoxChanged();
			Refresh();
		}
		protected internal void OnKeyDownHome() {
			SelectedIndexSymbolListBox = 0;
		}
		protected internal void OnKeyDownEnd() {
			SelectedIndexSymbolListBox = unicodeChars.Count - 1;
		}
		protected internal void OnKeyDownPageUp() {
			if (ScrollValue == 0)
				return;
			if (ScrollValue - rowCount >= 0) {
				selectedIndexSymbolListBox -= rowCount * columnCount;
				ScrollValue -= rowCount;
			}
			else {
				selectedIndexSymbolListBox -= ScrollValue * columnCount;
				ScrollValue = 0;
			}
			RaiseSelectedIndexSymbolListBoxChanged();
		}
		protected internal void OnKeyDownPageDown() {
			if (Items.Count < rowCount)
				return;
			int value = rowCount * columnCount;
			int newSelectedIndex = selectedIndexSymbolListBox + value;
			int deltaScroll = rowCount;
			int bottomBorder = ScrollValue + 2 * rowCount;
			if (bottomBorder > Items.Count) {
				int deltaColumn = bottomBorder - Items.Count;
				newSelectedIndex -= deltaColumn * columnCount;
				deltaScroll -= deltaColumn;
			}
			if (newSelectedIndex < unicodeChars.Count) {
				ScrollValue += deltaScroll;
				selectedIndexSymbolListBox = newSelectedIndex;
				RaiseSelectedIndexSymbolListBoxChanged();
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			isMouseDown = true;
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Left)
				UpdateSelectIndexAfterMouseDown(e);
		}
		protected internal void UpdateSelectIndexAfterMouseDown(MouseEventArgs e) {
			isMouseClickInItems = false;
			int x = 0;
			int y = - ColumnWidth;
			int visibleItemIndex = ScrollValue * columnCount;
			for (int i = visibleItemIndex; i < visibleItemIndex + columnCount * rowCount; i++) {
				if (i == unicodeChars.Count)
					return;
				if (i % columnCount == 0) {
					x = 0;
					y += ColumnWidth;
				}
				Rectangle rect = new Rectangle(x, y, ColumnWidth, ColumnWidth + 1);
				if (rect.Contains(e.X, e.Y)) {
					selectRectX = x;
					selectRectY = y;
					int selectedColumnNumber = i % columnCount;
					selectedIndexSymbolListBox = (SelectedIndex * columnCount) + selectedColumnNumber;
					RaiseSelectedIndexSymbolListBoxChanged();
					isMouseClickInItems = true;
					Refresh();
					return;
				}
				x += ColumnWidth;
			}
		}
		void OnMeasureItem(object sender, MeasureItemEventArgs e) {
			e.ItemHeight = this.ItemHeight;
		}
		void OnDrawItem(object sender, ListBoxDrawItemEventArgs e) {
			e.Handled = true;
			if (e.Index == -1 || (e.Index - ScrollValue) == rowCount)
				return;
			DrawGrid(e.Graphics, e.Cache.GetSolidBrush(ViewInfo.PaintAppearance.ForeColor));
			Size size = new Size(ColumnWidth, ColumnWidth);
			if (this.SelectedIndexSymbolListBox / columnCount == e.Index)
				DrawSelectRectangle(e.Cache, size);
			int index = e.Index * columnCount;
			int x = 0;
			for (int i = 0; i < columnCount; i++) {
				if (index < unicodeChars.Count) {
					Color textColor = (index == SelectedIndexSymbolListBox) ? SystemColors.HighlightText : ViewInfo.PaintAppearance.ForeColor;
					Brush brush = e.Cache.GetSolidBrush(ViewInfo.GetSystemColor(textColor));
					e.Graphics.DrawString(unicodeChars[index].ToString(), font, brush, new Rectangle(new Point(x, e.Bounds.Y), size), stringFormat);
					x += ColumnWidth;
				}
				index++;
			}
		}
		protected internal virtual void DrawSelectRectangle(GraphicsCache cache, Size size) {
			Rectangle bounds = GetBoundsSelectRectangle(size);
			Color backColor = ViewInfo.GetSystemColor(SystemColors.Highlight);
			cache.FillRectangle(backColor, bounds);
			ControlPaint.DrawFocusRectangle(cache.Graphics, bounds, Color.Empty, backColor);
		}
		protected internal virtual Rectangle GetBoundsSelectRectangle(Size size) {
			Rectangle result = new Rectangle(new Point(selectRectX, selectRectY), size);
			result.X++;
			result.Y++;
			result.Width--;
			result.Height--;
			return result;
		}
		protected internal virtual void CalculateColumnCount() {
			columnCount = (Width - ScrollInfo.VScroll.Width) / ColumnWidth;
		}
		protected internal virtual void CalculateRowCount() {
			rowCount = Height / ColumnWidth;
		}
		protected internal virtual void DrawGrid(Graphics graphics, Brush brush) {
			DrawVerticalLine(graphics, brush);
			DrawHorizontalLine(graphics, brush);
		}
		protected internal virtual void DrawHorizontalLine(Graphics graphics, Brush brush) {
			int y = 0;
			int lineWidth = columnCount * ColumnWidth;
			for (int i = 0; i <= rowCount; i++) {
				graphics.FillRectangle(brush, new Rectangle(0, y, lineWidth, 1));
				y += ColumnWidth;
			}
		}
		protected internal virtual void DrawVerticalLine(Graphics graphics, Brush brush) {
			int x = 0;
			int lineHeight = rowCount * ColumnWidth;
			for (int i = 0; i <= columnCount; i++) {
				graphics.FillRectangle(brush, new Rectangle(x, 0, 1, lineHeight));
				x += ColumnWidth;
			}
		}
		protected internal virtual void SetSelectedChar(char ch) {
			int index = unicodeChars.IndexOf(ch);
			if (index == -1)
				index = 0;
			SelectedIndexSymbolListBox = index;
		}
		protected internal virtual char GetSelectedChar() {
			if (selectedIndexSymbolListBox >= 0 && selectedIndexSymbolListBox < unicodeChars.Count)
				return unicodeChars[selectedIndexSymbolListBox];
			else
				return '\x0';
		}
	}
	#endregion
}
