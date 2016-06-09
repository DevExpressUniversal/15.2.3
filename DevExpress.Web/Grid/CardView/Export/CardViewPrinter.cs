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
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.NativeBricks;
namespace DevExpress.Web.Export {
	public class CardViewPrinter : GridPrinterBase {
		CardViewPrintInfoCalculator calculator;
		CardViewStyleHelper styleHelper;
		Size pageSize;
		public CardViewPrinter(ASPxCardViewExporter exporter, BrickGraphics brickGraphics)
			: base(exporter, exporter.GridBase) {
			Graph = brickGraphics;
			BeforeCreate();
		}
		public BrickGraphics Graph { get; private set; }
		public new ASPxCardViewExporter Exporter { get { return base.Exporter as ASPxCardViewExporter; } }
		public new CardViewColumnHelper ColumnHelper { get { return base.ColumnHelper as CardViewColumnHelper; } }
		public ASPxCardView CardView { get { return Grid as ASPxCardView; } }
		public Size PageSize {
			get {
				if(Graph == null || Graph.PrintingSystem == null)
					return Size.Empty;
				if(pageSize.IsEmpty) {
					var printingSystem = Graph.PrintingSystem;
					var pageBounds = printingSystem.PageSettings.UsablePageSizeInPixels;
					var pageMargins = printingSystem.PageMargins;
					pageSize = new Size((int)pageBounds.Width - (pageMargins.Left + pageMargins.Right), (int)pageBounds.Height - (pageMargins.Top + pageMargins.Bottom));
				}
				return pageSize;
			}
		}
		public int CardWidth { get { return Exporter.CardWidth; } }
		public int HorizontalMargins { get { return LeftMargin + RightMargin; } }
		public int LeftMargin { get; set; }
		public int RightMargin { get; set; }
		public CardViewTextBuilder TextBuilder { get { return CardView.RenderHelper.TextBuilder; } }
		public CardViewStyleHelper StyleHelper {
			get {
				if(styleHelper == null)
					styleHelper = new CardViewStyleHelper(Exporter.Styles, CardView);
				return styleHelper;
			}
		}
		internal CardViewPrintInfoCalculator Calculator {
			get {
				if(calculator == null)
					calculator = new CardViewPrintInfoCalculator(this);
				return calculator;
			}
		}
		public IEnumerable<int> EnumerateExportedVisibleCards() {
			return EnumerateExportedVisibleRows();
		}
		void BeforeCreate() {
			Calculator.CalculateCards();
			Calculator.CalculateTotalSummary();
		}
		protected override void PrepareGridForExport() {
			if(CardView.CardLayoutProperties.Root.Items.Count == 0)
				CardView.CardLayoutProperties.Assign(CardView.GenerateDefaultLayout(false));
			base.PrepareGridForExport();
		}
		internal void CreateDetailHeader() {
		}
		internal void CreateDetail() {
			PrintCards();
			PrintTotalSummary();
		}
		public virtual CardViewPrintInfoBase CreatePrintInfoItem(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase layoutItem, int colIndex, int rowIndex) {
			if(layoutItem is CardViewCommandLayoutItem) {
				var commandItem = (CardViewCommandLayoutItem)layoutItem;
				if(!Exporter.PrintSelectCheckBox || !commandItem.ShowSelectCheckbox)
					return null;
				return new CardViewCommandLayoutItemPrintInfo(printInfoOwner, commandItem, colIndex, rowIndex);
			}
			if(layoutItem is CardViewLayoutGroup)
				return new CardViewGroupPrintInfo(printInfoOwner, layoutItem, colIndex, rowIndex);
			if(layoutItem is CardViewTabbedLayoutGroup)
				return new CardViewTabbedGroupPrintInfo(printInfoOwner, layoutItem, colIndex, rowIndex);
			var column = layoutItem is CardViewColumnLayoutItem ? ((CardViewColumnLayoutItem)layoutItem).Column : null;
			if(column is CardViewBinaryImageColumn)
				return new CardViewImagePrintInfo(printInfoOwner, layoutItem, colIndex, rowIndex);
			if(column is CardViewCheckColumn)
				return new CardViewCheckPrintInfo(printInfoOwner, layoutItem, colIndex, rowIndex);
			return new CardViewTextPrintInfo(printInfoOwner, layoutItem, colIndex, rowIndex);
		}
		public void PrintCards() {
			var cards = Calculator.Cards;
			for(var cardIndex = 0; cardIndex < cards.Count; ++cardIndex)
				cards[cardIndex].Draw();
		}
		bool CanPrintItem(CardViewPrintInfoBase printingItem) {
			return printingItem != null && (printingItem as CardViewBusyPrintInfo) == null;
		}
		void PrintTotalSummary() {
			if(Calculator.TotalSummaries.Count == 0)
				return;
			var summaryPanelStyle = StyleHelper.GetTotalSummaryPanelStyle(Graph);
			var location = new Point(0, Calculator.CardsBottom + 14);
			var rect = new Rectangle(location, new Size(Calculator.CardsWidth, Calculator.SummaryHeight + summaryPanelStyle.Padding.Top + summaryPanelStyle.Padding.Bottom));
			PrintingHelper.DrawTextBrick(Graph, string.Empty, rect, summaryPanelStyle);
			location.Y = rect.Y + (rect.Height - Calculator.SummaryHeight) / 2;
			foreach(var summary in Calculator.TotalSummaries) {
				PrintingHelper.DrawTextBrick(Graph, summary.Text, new Rectangle(location, summary.TextSize), summary.Style);
				location.X += summary.TextSize.Width + 4;
				if(location.X > Calculator.CardsWidth) {
					location.X = 1;
					location.Y += summary.TextSize.Height;
				}
			}
		}
	}
	public class CardViewPrintInfoCalculator {
		List<CardViewPrintingCard> cards;
		List<SummaryPrintInfo> totalSummaries;
		public CardViewPrintInfoCalculator(CardViewPrinter printer) {
			Printer = printer;
		}
		public List<CardViewPrintingCard> Cards {
			get {
				if(cards == null)
					cards = new List<CardViewPrintingCard>();
				return cards;
			}
		}
		public List<SummaryPrintInfo> TotalSummaries {
			get {
				if(totalSummaries == null)
					totalSummaries = new List<SummaryPrintInfo>();
				return totalSummaries;
			}
		}
		public int CardsBottom { get; set; }
		public int CardsWidth { get; set; }
		public int SummaryHeight { get; set; }
		CardViewPrinter Printer { get; set; }
		ASPxCardView CardView { get { return Printer.CardView; } }
		public void CalculateCards() {
			var location = new Point(0, 0);
			var pageWidth = Printer.PageSize.Width;
			var pageHeight = Printer.PageSize.Height;
			var summaryPageHeight = pageHeight;
			foreach(var cardIndex in Printer.EnumerateExportedVisibleCards()) {
				var cardInfo = CreatePrintingCard(location, cardIndex);
				cardInfo.CalculateCard(location);
				var right = cardInfo.Right + cardInfo.RightMargin;
				if(right > pageWidth) {
					location.X = 0;
					location.Y = CardsBottom;
					if(location.Y + (cardInfo.Height + cardInfo.BottomMargin) > summaryPageHeight) {
						location.Y = summaryPageHeight;
						summaryPageHeight += pageHeight;
					}
					cardInfo.CalculateCard(location);
				} else {
					CardsWidth = Math.Max(CardsWidth, cardInfo.Right);
				}
				CardsBottom = Math.Max(CardsBottom, cardInfo.Bottom + cardInfo.BottomMargin);
				location.X = cardInfo.Right + cardInfo.RightMargin;
			}
		}
		public void CalculateTotalSummary() { 
			if(CardView.TotalSummary.Count == 0)
				return;
			var pageWidth = Printer.PageSize.Width;
			var summarySeparator = ";";
			var count = CardView.TotalSummary.Count;
			var width = pageWidth + 1;
			var graph = Printer.Graph;
			for(var i = 0; i < count; ++i) {
				var totalSummary = CardView.TotalSummary[i];
				var separator = i != count - 1 ? summarySeparator : string.Empty;
				var summaryText = totalSummary.GetSummaryDisplayText(CardView.Columns[totalSummary.FieldName], CardView.GetTotalSummaryValue(totalSummary)) + separator;
				var style = Printer.StyleHelper.GetTotalSummaryItemStyle(graph, totalSummary);
				var textSize = Printer.StyleHelper.CalcTextSize(graph, summaryText, style, pageWidth);
				TotalSummaries.Add(new SummaryPrintInfo(style, summaryText, textSize));
				if(width > pageWidth) {
					width = 1;
					SummaryHeight += textSize.Height + 4;
				}
				width += textSize.Width + 4;
			}
		}
		CardViewPrintingCard CreatePrintingCard(Point location, int cardIndex) {
			var rootItem = Printer.CardView.CardLayoutProperties.Root;
			var cardInfo = new CardViewPrintingCard(rootItem, Printer, location, cardIndex);
			var exporter = Printer.Exporter;
			Cards.Add(cardInfo);
			return cardInfo;
		}
	}
	public class SummaryPrintInfo {
		public SummaryPrintInfo(BrickStyle style, string text, Size textSize) {
			Style = style;
			Text = text;
			TextSize = textSize;
		}
		public BrickStyle Style { get; private set; }
		public string Text { get; private set; }
		public Size TextSize { get; private set; }
	}
	public class CardViewPrintingCard : CardViewGroupPrintInfo {
		public CardViewPrintingCard(LayoutItemBase rootItem, CardViewPrinter printer, Point location, int cardIndex)
			: base(null, rootItem, -1, -1) {
			Printer = printer;
			Width = Printer.CardWidth;
			Location = location;
			CardIndex = cardIndex;
			LeftMargin = Printer.Exporter.LeftMargin;
			TopMargin = Printer.Exporter.TopMargin;
			RightMargin = Printer.Exporter.RightMargin;
			BottomMargin = Printer.Exporter.BottomMargin;
		}
		public override string Caption { get { return string.Empty; } }
		public override Rectangle CaptionRect { get { return Rectangle.Empty; } }
		public void CalculateCard(Point location) {
			CalculateGroup(Printer, location, CardIndex);
		}
		protected override void DrawCaptionRect() {
		}
		public override void Draw() {
			PrintingHelper.DrawTextBrick(Printer.Graph, new CardViewDrawTextBrickArgs(string.Empty, StyleHelper.GetCardStyle(Printer.Graph, LayoutItem, CardIndex), LocationRect, null, CardIndex));
			base.Draw();
		}
	}
	public class CardViewGroupPrintInfo : CardViewGroupPrintInfoBase {
		CardViewEmptyBusyPrintInfo busyItem;
		LayoutGroup layoutGroup;
		public CardViewGroupPrintInfo(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase layoutItem, int startCol, int startRow)
			: base(printInfoOwner, layoutItem, startCol, startRow) {
		}
		protected CardViewEmptyBusyPrintInfo BusyEmptyItem {
			get {
				if(busyItem == null)
					busyItem = new CardViewEmptyBusyPrintInfo(DefaultCellHeight);
				return busyItem;
			}
		}
		LayoutGroup LayoutGroupItem {
			get {
				if(layoutGroup == null)
					layoutGroup = (LayoutGroup)LayoutItem;
				return layoutGroup;
			}
		}
		public override int TopPadding { get { return base.TopPadding + CaptionRect.Height / 2; } set { } }
		public void CalculateGroup(CardViewPrinter printer, Point location, int cardIndex) {
			CardIndex = cardIndex;
			Calculate(printer, printer.CardWidth);
			CalculateSize(printer, location, printer.CardWidth);
		}
		public override int GetMaxItemWidth(CardViewPrintInfoBase infoItem) {
			if(infoItem.LayoutItem.ColSpan == LayoutGroupItem.ColCount)
				return Width - HorizontalPaddings - infoItem.HorizontalMargins;
			var width = Convert.ToInt32(Width * infoItem.LayoutItem.ColSpan / (double)LayoutGroupItem.ColCount - infoItem.HorizontalMargins);
			if(infoItem.LayoutItem.ColSpan + infoItem.StartCol == LayoutGroupItem.ColCount)
				width -= RightPadding;
			return width;
		}
		protected override void ProcessLayoutGroup(LayoutGroupBase layoutGroupBase) {
			var colIndex = 0;
			var rowIndex = 0;
			var layoutGroup = (LayoutGroup)layoutGroupBase;
			var maxRowCount = GetMaxRowCount(layoutGroup);
			var visibleItemsCount = layoutGroup.Items.GetVisibleItemCount();
			PrintInfoMap = new CardViewPrintInfoBase[layoutGroup.ColCount, maxRowCount];
			for(int i = 0; i < visibleItemsCount; ++i) {
				var item = layoutGroup.Items.GetVisibleItemOrGroup(i);
				var itemColSpan = item.GetColSpan();
				PlaceItemInsideGroupRecursive(itemColSpan, layoutGroup.ColCount, maxRowCount, ref colIndex, ref rowIndex);
				FillSpaceByItemSpans(item, itemColSpan, colIndex, rowIndex);
				var printInfo = Printer.CreatePrintInfoItem(this, item, colIndex, rowIndex);
				if(printInfo != null)
					printInfo.Calculate(Printer, Width - printInfo.HorizontalMargins);
				PrintInfoMap[colIndex, rowIndex] = printInfo;
			}
		}
		protected int GetMaxRowCount(LayoutGroup layoutGroup) {
			int maxRowCount = 0;
			for(int i = 0; i < layoutGroup.Items.GetVisibleItemCount(); i++)
				maxRowCount += layoutGroup.Items.GetVisibleItemOrGroup(i).GetRowSpan();
			return maxRowCount;
		}
		void PlaceItemInsideGroupRecursive(int itemColSpan, int maxColCount, int maxRowCount, ref int colIndex, ref int rowIndex) {
			for(var r = rowIndex; r < maxRowCount; ++r) {
				for(var c = colIndex; c < maxColCount; ++c) {
					if(PrintInfoMap[c, r] == null) {
						colIndex = c;
						rowIndex = r;
						return;
					}
				}
				if(r == rowIndex)
					colIndex = 0;
			}
			if(!HasEnoughSpace(itemColSpan, maxColCount, colIndex, rowIndex)) {
				PrintInfoMap[colIndex, rowIndex] = BusyEmptyItem;
				PlaceItemInsideGroupRecursive(itemColSpan, maxColCount, maxRowCount, ref colIndex, ref rowIndex);
			}
		}
		protected bool HasEnoughSpace(int itemColSpan, int maxColCount, int colIndex, int rowIndex) {
			int firstBusyIndex = colIndex;
			while(firstBusyIndex < maxColCount && !(PrintInfoMap[firstBusyIndex, rowIndex] is CardViewBusyPrintInfo))
				firstBusyIndex++;
			return colIndex + itemColSpan - 1 < firstBusyIndex;
		}
		void FillSpaceByItemSpans(LayoutItemBase item, int itemColSpan, int colIndex, int rowIndex) {
			var itemRowSpan = item.GetRowSpan();
			var currentBusy = itemRowSpan > 1 || itemColSpan > 1 ? new CardViewBusyPrintInfo(this, item, -1, colIndex, rowIndex) : BusyEmptyItem;
			var maxColIndex = colIndex + itemColSpan;
			var maxRowIndex = rowIndex + itemRowSpan;
			for(var c = colIndex; c < maxColIndex; ++c)
				for(var r = rowIndex; r < maxRowIndex; ++r)
					PrintInfoMap[c, r] = currentBusy;
		}
		bool CanPrintItem(CardViewPrintInfoBase printingItem) {
			return printingItem != null && (printingItem as CardViewBusyPrintInfo) == null;
		}
		protected override void DrawCaptionRect() {
			if(LayoutGroupItem.GroupBoxDecoration != GroupBoxDecoration.HeadingLine) {
				PrintingHelper.DrawTextBrick(Printer.Graph, string.Empty, LocationRect, StyleHelper.GetGroupStyle(Printer.Graph, LayoutGroupItem));
				if(!string.IsNullOrEmpty(Caption))
					PrintingHelper.DrawTextBrick(Printer.Graph, Caption, GetItemCaptionRect(Width), StyleHelper.GetGroupCaptionStyle(Printer.Graph, LayoutItem));
			} else {
				var captionRect = GetItemCaptionRect(Width);
				var captionStyle = StyleHelper.GetGroupCaptionStyle(Printer.Graph, LayoutItem);
				if(!string.IsNullOrEmpty(Caption))
					PrintingHelper.DrawTextBrick(Printer.Graph, Caption, captionRect, captionStyle);
				var groupStyle = StyleHelper.GetGroupStyle(Printer.Graph, LayoutGroupItem);
				var borderRect = new Rectangle(Left, captionRect.Bottom + captionStyle.Padding.Bottom, Width, Convert.ToInt32(groupStyle.BorderWidth));
				PrintingHelper.DrawTextBrick(Printer.Graph, string.Empty, borderRect, groupStyle);
			}
		}
		public override void UpdateLocation(Point nextLocation) {
			if(!CaptionRect.IsEmpty)
				nextLocation.Y += CaptionRect.Height / 2;
			base.UpdateLocation(nextLocation);
		}
		protected override Rectangle GetItemCaptionRect(int maxCellWidth) {
			var styleHelper = Printer.StyleHelper;
			var graph = Printer.Graph;
			if(graph == null)
				return Rectangle.Empty;
			CaptionSize = styleHelper.CalcTextSize(graph, Caption, styleHelper.GetGroupCaptionStyle(graph, LayoutItem), maxCellWidth);
			return new Rectangle(Location.X + 10, LocationRect.Y - CaptionSize.Height / 2, CaptionSize.Width, CaptionSize.Height);
		}
	}
	public class CardViewTabbedGroupPrintInfo : CardViewGroupPrintInfoBase {
		TabbedLayoutGroup tabbedGroup;
		BrickStyle tabGroupStyle;
		LayoutItemBase activeLayoutItem;
		public CardViewTabbedGroupPrintInfo(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase layoutItem, int startCol, int startRow)
			: base(printInfoOwner, layoutItem, startCol, startRow) {
		}
		TabbedLayoutGroup TabbedGroup {
			get {
				if(tabbedGroup == null)
					tabbedGroup = (TabbedLayoutGroup)LayoutItem;
				return tabbedGroup;
			}
		}
		BrickStyle TabGroupStyle {
			get {
				if(tabGroupStyle == null)
					tabGroupStyle = StyleHelper.GetTabbedGroupStyle(Printer.Graph, LayoutItem);
				return tabGroupStyle;
			}
		}
		Size ActiveTabSize { get; set; }
		LayoutItemBase ActiveLayoutItem {
			get {
				if(activeLayoutItem == null) {
					var activeTabIndex = TabbedGroup.ActiveTabIndex;
					if(activeTabIndex >= 0)
						activeLayoutItem = TabbedGroup.Items.GetVisibleItemOrGroup(activeTabIndex);
				}
				return activeLayoutItem;
			}
		} 
		protected override void ProcessLayoutGroup(LayoutGroupBase layoutGroupBase) {
			if(ActiveLayoutItem == null)
				return;
			PrintInfoMap = new CardViewPrintInfoBase[1, 1];
			var printInfo = Printer.CreatePrintInfoItem(this, ActiveLayoutItem, 0, 0);
			if(printInfo is CardViewGroupPrintInfoBase) {
				var groupInfo = (CardViewGroupPrintInfoBase)printInfo;
				groupInfo.CanDrawCaption = false;
			}
			if(Printer.Graph != null)
				ActiveTabSize = StyleHelper.CalcTextSize(Printer.Graph, ActiveLayoutItem.Caption, StyleHelper.GetTabStyle(Printer.Graph, LayoutItem), Width - 4);
			printInfo.Calculate(Printer, Width - printInfo.HorizontalMargins);
			PrintInfoMap[0, 0] = printInfo;
		}
		public override int GetMaxItemWidth(CardViewPrintInfoBase infoItem) {
			return Width - HorizontalPaddings;
		}
		public override void Draw() {
			DrawFrame();
			DrawHeaders();
			base.Draw();
		}
		void DrawHeaders() {
			var tabStyle = StyleHelper.GetTabStyle(Printer.Graph, LayoutItem);
			var disabledTabStyle = StyleHelper.GetDisabledTabStyle(Printer.Graph, LayoutItem);
			var visibleItemCount = TabbedGroup.Items.GetVisibleItemCount();
			var borderWidth = (int)TabGroupStyle.BorderWidth;
			var location = new Point(Location.X + 4, Location.Y + borderWidth);
			var graph = Printer.Graph;
			var textSize = Size.Empty;
			for(var i = 0; i < visibleItemCount; ++i) {
				var item = tabbedGroup.Items.GetVisibleItemOrGroup(i);
				var caption = item.Caption;
				if(item.Path == ActiveLayoutItem.Path) {
					textSize = StyleHelper.CalcTextSize(graph, caption, tabStyle, Width - 4);
					PrintingHelper.DrawTextBrick(graph, caption, new Rectangle(location.X, location.Y, textSize.Width, textSize.Height), tabStyle);
				} else {
					textSize = StyleHelper.CalcTextSize(graph, caption, disabledTabStyle, Width - 4);
					PrintingHelper.DrawTextBrick(graph, caption, new Rectangle(location.X, location.Y, textSize.Width, textSize.Height - borderWidth), disabledTabStyle);
				}
				location.X += textSize.Width + 2;
			}
		}
		void DrawFrame() {
			var rect = new Rectangle(Location.X, Location.Y + (int)TabGroupStyle.BorderWidth + ActiveTabSize.Height, Width, Height - ActiveTabSize.Height);
			PrintingHelper.DrawTextBrick(Printer.Graph, string.Empty, rect, TabGroupStyle);
		}
	}
	public abstract class CardViewGroupPrintInfoBase : CardViewPrintInfoBase {
		public CardViewGroupPrintInfoBase(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase layoutItem, int startCol, int startRow)
			: base(printInfoOwner, layoutItem, startCol, startRow) {
			CanDrawCaption = true;
		}
		public CardViewPrintInfoBase[,] PrintInfoMap { get; set; }
		public bool CanDrawCaption { get; set; }
		public abstract int GetMaxItemWidth(CardViewPrintInfoBase infoItem);
		protected abstract void ProcessLayoutGroup(LayoutGroupBase layoutGroupBase);
		protected override void CalculateCore(int maxWidth) {
			CalculateMatrix(LayoutItem);
		}
		protected override void CalculateInnerSize(CardViewPrinter printer, int maxCellWidth) {
			Width = maxCellWidth;
			CalculateItemSizes();
		}
		Dictionary<int, int> maxColumnCaptionWidthes;
		protected void DetermineCaptionWidthes() {
			maxColumnCaptionWidthes = new Dictionary<int, int>();
			var cols = PrintInfoMap.GetLength(0);
			var rows = PrintInfoMap.GetLength(1);
			for(var c = 0; c < cols; ++c) {
				for(var r = 0; r < rows; ++r) {
					var item = PrintInfoMap[c, r] as CardViewPrintInfoItem;
					if(item == null || item.LayoutItem == null)
						continue;
					item.CalcItemSize(Printer, GetMaxItemWidth(item), false);
					int key = GetMaxColumnCaptionWidthesKey(c, item.LayoutItem.ColSpan);
					if(!maxColumnCaptionWidthes.ContainsKey(key) || maxColumnCaptionWidthes[key] < item.CaptionSize.Width)
						maxColumnCaptionWidthes[key] = item.CaptionSize.Width;
				}
			}
		}
		public int GetMaxColumnCaptionWidth(CardViewPrintInfoItem item) {
			if(maxColumnCaptionWidthes == null || item.LayoutItem == null)
				return 0;
			int key = GetMaxColumnCaptionWidthesKey(item.StartCol, item.LayoutItem.ColSpan);
			return maxColumnCaptionWidthes.ContainsKey(key) ? maxColumnCaptionWidthes[key] : 0;
		}
		int GetMaxColumnCaptionWidthesKey(int colIndex, int colSpan) {
			return colIndex | (colSpan << 8);
		}
		protected void CalculateItemSizes() {
			DetermineCaptionWidthes();
			var cols = PrintInfoMap.GetLength(0);
			var rows = PrintInfoMap.GetLength(1);
			var location = GetItemLocation();
			var defaultLeft = location.X;
			var maxHeight = 0;
			Height = 0;
			for(var r = 0; r < rows; ++r) {
				for(var c = 0; c < cols; ++c) {
					var infoItem = PrintInfoMap[c, r];
					if(!IsNotBusyInfoItem(infoItem))
						continue;
					if(!infoItem.IsColumnSibling) {
						location.X += infoItem.LeftMargin;
						infoItem.CalculateSize(Printer, location, GetMaxItemWidth(infoItem));
						maxHeight = Math.Max(maxHeight, infoItem.Height + infoItem.VerticalMargins);
						location.X += infoItem.Right + infoItem.RightMargin;
						location.Y = infoItem.Y;
					}
					if(infoItem.Bottom > Bottom)
						Height += infoItem.Bottom - Bottom + infoItem.BottomMargin + BottomPadding;
				}
				location.X = defaultLeft;
				location.Y += maxHeight;
				maxHeight = 0;
			}
		}
		protected Point GetItemLocation() {
			return new Point(Left + LeftPadding, Y + TopPadding);
		}
		public void CalculateMatrix(LayoutItemBase layoutItemBase) {
			if(layoutItemBase is LayoutGroupBase)
				ProcessLayoutGroup((LayoutGroupBase)layoutItemBase);
			DetermineSiblings();
		}
		void DetermineSiblings() {
			if(PrintInfoMap == null)
				return;
			var cols = PrintInfoMap.GetLength(0);
			var rows = PrintInfoMap.GetLength(1);
			var lastColIndex = cols - 1;
			var lastRowIndex = rows - 1;
			for(var r = 0; r < rows; ++r) {
				for(var c = 0; c < cols; ++c) {
					var targetItem = PrintInfoMap[c, r];
					if(!IsValidInfoItem(targetItem))
						continue;
					var siblingTarget = PrintInfoMap[targetItem.StartCol, targetItem.StartRow];
					if(c > 0)
						siblingTarget.AddPrevColumnSibling(PrintInfoMap[c - 1, r]);
					if(c < lastColIndex)
						siblingTarget.AddNextColumnSibling(PrintInfoMap[c + 1, r]);
					if(r > 0)
						siblingTarget.AddPrevRowSibling(PrintInfoMap[c, r - 1]);
					if(r < lastRowIndex)
						siblingTarget.AddNextRowSibling(PrintInfoMap[c, r + 1]);
				}
			}
		}
		bool IsValidInfoItem(CardViewPrintInfoBase infoItem) {
			return infoItem != null && !(infoItem is CardViewEmptyBusyPrintInfo) && infoItem.LayoutItem != null;
		}
		bool IsNotBusyInfoItem(CardViewPrintInfoBase infoItem) {
			return infoItem != null && infoItem.LayoutItem != null && (infoItem as CardViewBusyPrintInfo) == null;
		}
		public override void MoveItemVertical(int offset) {
			base.MoveItemVertical(offset);
			var cols = PrintInfoMap.GetLength(0);
			var rows = PrintInfoMap.GetLength(1);
			for(var r = 0; r < rows; ++r) {
				for(var c = 0; c < cols; ++c) {
					var targetItem = PrintInfoMap[c, r];
					if(!IsValidInfoItem(targetItem))
						continue;
					targetItem.MoveItemVertical(offset);
				}
			}
		}
		public override void Draw() {
			var cols = PrintInfoMap.GetLength(0);
			var rows = PrintInfoMap.GetLength(1);
			for(var r = 0; r < rows; ++r) {
				for(var c = 0; c < cols; ++c) {
					var item = PrintInfoMap[c, r];
					if(CanPrintItem(item))
						item.Draw();
				}
			}
			if(CanDrawCaption)
				DrawCaptionRect();
		}
		protected virtual void DrawCaptionRect() {
		}
		bool CanPrintItem(CardViewPrintInfoBase printingItem) {
			return printingItem != null && (printingItem as CardViewBusyPrintInfo) == null;
		}
	}
	public class CardViewTextPrintInfo : CardViewPrintInfoItem {
		public CardViewTextPrintInfo(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase layoutItem, int startCol, int startRow) 
			: base(printInfoOwner, layoutItem, startCol, startRow) {
		}
		protected override Size GetDataObjectSize(BrickGraphics graph, CardViewStyleHelper styleHelper, int maxCellWidth) {
			var style = styleHelper.GetCellStyle(graph, Column, CardIndex, false, Printer.TextBuilder.GetColumnDisplayControlDefaultAlignment(Column), false, true);
			return StyleHelper.CalcTextSize(graph, Text, style, maxCellWidth);
		}
	}
	public class CardViewImagePrintInfo : CardViewPrintInfoItem {
		public CardViewImagePrintInfo(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase layoutItem, int startCol, int startRow) 
			: base(printInfoOwner, layoutItem, startCol, startRow) {
		}
		protected override Size GetDataObjectSize(BrickGraphics graph, CardViewStyleHelper styleHelper, int maxCellWidth) {
			var imageColumn = (CardViewBinaryImageColumn)Column;
			var imageExportSettings = PrintingHelper.GetColumnExportProperties(imageColumn) as IImageExportSettings;
			if(imageExportSettings == null)
				return Size.Empty;
			var textBuilder = Printer.TextBuilder;
			var useColumnWidth = imageColumn.ExportWidth != 0;
			var width = useColumnWidth ? imageColumn.ExportWidth : imageExportSettings.Width;
			if(width > maxCellWidth)
				width = maxCellWidth;
			var style = styleHelper.GetCellStyle(graph, imageColumn, CardIndex, false, textBuilder.GetColumnDisplayControlDefaultAlignment(imageColumn), false, true);
			return styleHelper.CalcImageSize(width, imageExportSettings.Height, graph, style, useColumnWidth);
		}
		protected override void DrawDataObject() {
			DrawDataImageCell();
		}
		void DrawDataImageCell() {
			var textBuilder = Printer.TextBuilder;
			var args = textBuilder.GetDisplayControlArgs(Column, CardIndex);
			var exportProperties = Column.ColumnAdapter.ExportPropertiesEdit;
			var url = exportProperties.GetExportNavigateUrl(args);
			var imageValue = exportProperties.GetExportValue(args) as byte[];
			var graph = Printer.Graph;
			var style = StyleHelper.GetCellStyle(graph, Column, CardIndex, false, textBuilder.GetColumnDisplayControlDefaultAlignment(Column), !String.IsNullOrEmpty(url), true);
			var exporter = Printer.Exporter;
			var e = exporter.RaiseRenderBrick(CardIndex, Column, Printer.DataProxy, style, string.Empty, string.Empty, string.Empty, url, imageValue);
			if(e != null) {
				url = e.Url;
				style = e.BrickStyle;
				imageValue = e.ImageValue;
				if(imageValue == null && (!string.IsNullOrEmpty(e.Text) || e.TextValue != null)) {
					PrintingHelper.DrawTextBrick(graph, new CardViewDrawTextBrickArgs(e.Text, e.BrickStyle, ObjectRect, Column, CardIndex));
					return;
				}
			}
			PrintingHelper.DrawImageBrick(Printer.Graph, style, ObjectRect, ((IImageExportSettings)exportProperties).SizeMode, string.Empty, imageValue);
		}
	}
	public class CardViewCommandLayoutItemPrintInfo : CardViewCheckPrintInfo {
		public CardViewCommandLayoutItemPrintInfo(CardViewGroupPrintInfoBase printInfoOwner, CardViewCommandLayoutItem commandItem, int startCol, int startRow)
			: base(printInfoOwner, commandItem, startCol, startRow) {
			CommandItem = commandItem;
		}
		CardViewCommandLayoutItem CommandItem { get; set; }
		protected override Size GetDataObjectSize(BrickGraphics graph, CardViewStyleHelper styleHelper, int maxCellWidth) {
			return base.GetDataObjectSize(graph, styleHelper, maxCellWidth);
		}
		protected override bool GetIsChecked() {
			return Printer.CardView.Selection.IsCardSelected(CardIndex);
		}
		protected override BrickStyle GetStyle(BrickGraphics graph) {
			return BrickStyle.CreateDefault();
		}
		protected override Rectangle GetItemCaptionRect(int maxCellWidth) {
			var styleHelper = Printer.StyleHelper;
			var graph = Printer.Graph;
			if(graph == null)
				return Rectangle.Empty;
			var locationType = GetCaptionLocationType();
			var captionStyle = styleHelper.GetCaptionStyle(graph, LayoutItem);
			var location = Location;
			var maxCaptionWidth = GetCaptionWidthByLocation(locationType, maxCellWidth);
			var captionX = LocationRect.Right - CaptionSize.Width;
			switch(locationType) {
				case LayoutItemCaptionLocation.Top:
					location = new Point(captionX, LocationRect.Top);
					break;
				case LayoutItemCaptionLocation.Bottom:
					location = new Point(captionX, LocationRect.Bottom - CaptionSize.Height);
					break;
				case LayoutItemCaptionLocation.Left:
				case LayoutItemCaptionLocation.NotSet:
					location = new Point(captionX - DataObjectSize.Width - 4, Location.Y + LocationRect.Height / 2 - CaptionSize.Height / 2);
					break;
				case LayoutItemCaptionLocation.Right:
					location = new Point(captionX, Location.Y + LocationRect.Height / 2 - CaptionSize.Height / 2);
					break;
			}
			return new Rectangle(location, CaptionSize);
		}
		protected override Rectangle GetObjectRect(CardViewPrinter printer, int maxCellWidth) {
			var styleHelper = printer.StyleHelper;
			var graph = printer.Graph;
			if(graph == null || LayoutItem == null)
				return Rectangle.Empty;
			var location = Point.Empty;
			switch(GetCaptionLocationType()) {
				case LayoutItemCaptionLocation.Top:
					location = new Point(CaptionRect.Left, CaptionRect.Bottom);
					break;
				case LayoutItemCaptionLocation.Bottom:
					location = new Point(Left, LocationRect.Top);
					break;
				case LayoutItemCaptionLocation.Left:
				case LayoutItemCaptionLocation.NotSet:
					location = new Point(CaptionRect.Right, Y + LocationRect.Height / 2 - DataObjectSize.Height / 2);
					break;
				case LayoutItemCaptionLocation.Right:
					location = new Point(LocationRect.Left - DataObjectSize.Width, Y + (LocationRect.Height - DataObjectSize.Height) / 2);
					break;
			}
			return new Rectangle(location, DataObjectSize);
		}
	}
	public class CardViewCheckPrintInfo : CardViewPrintInfoItem {
		Size CheckBoxSize = new Size(12, 12);
		const string
			CheckedDisplayText = "[+]",
			UncheckedDisplayText = "[-]";
		public CardViewCheckPrintInfo(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase layoutItem, int startCol, int startRow)
			: base(printInfoOwner, layoutItem, startCol, startRow) {
		}
		protected override Size GetDataObjectSize(BrickGraphics graph, CardViewStyleHelper styleHelper, int maxCellWidth) {
			return CheckBoxSize;
		}
		protected override void DrawDataObject() {
			var graph = Printer.Graph;
			var isChecked = GetIsChecked();
			var displayText = isChecked ? CheckedDisplayText : UncheckedDisplayText;
			PrintingHelper.DrawCheckBoxBrick(graph, GetStyle(graph), isChecked, displayText, ObjectRect);
		}
		protected virtual bool GetIsChecked() {
			return Convert.ToBoolean(Printer.DataProxy.GetRowValue(CardIndex, Column.FieldName)) ? true : false;
		}
		protected virtual BrickStyle GetStyle(BrickGraphics graph) {
			return StyleHelper.GetCellStyle(graph, Column, CardIndex, IsAltRow(), HorizontalAlign.Center, false);
		}
		protected override int GetCaptionWidthByLocation(LayoutItemCaptionLocation captionType, int cellWidth) {
			if(captionType == LayoutItemCaptionLocation.Top || captionType == LayoutItemCaptionLocation.Bottom)
				return cellWidth;
			return cellWidth - CheckBoxSize.Width - 4;
		}
	}
	public abstract class CardViewPrintInfoItem : CardViewPrintInfoBase {
		Rectangle objectRect;
		CardViewColumn column;
		string text;
		public CardViewPrintInfoItem(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase layoutItem, int startCol, int startRow)
			: base(printInfoOwner, layoutItem, startCol, startRow) {
		}
		public string Text {
			get {
				if(text == null && CardViewLayoutItem != null) {
					var column = CardViewLayoutItem.Column;
					text = column != null ? Printer.TextBuilder.GetRowDisplayText(column, CardIndex) : string.Empty;
				}
				return text;
			}
		}
		public Size DataObjectSize { get; set; }
		public Rectangle ObjectRect {
			get {
				if(objectRect.IsEmpty)
					objectRect = GetObjectRect(Printer, Width);
				return objectRect;
			}
		}
		protected CardViewColumn Column {
			get {
				if(column == null && CardViewLayoutItem != null)
					column = CardViewLayoutItem.Column;
				return column;
			}
		}
		protected override void CalculateInnerSize(CardViewPrinter printer, int maxCellWidth) {
			LocationRect = new Rectangle(Location, CalcItemSize(printer, maxCellWidth, true));
		}
		internal Size CalcItemSize(CardViewPrinter printer, int maxCellWidth, bool useMaxCaptionColumnWidth) {
			var graph = printer.Graph;
			if(graph == null || LayoutItem == null)
				return Size.Empty;
			var captionLocation = GetCaptionLocationType();
			var caption = LayoutItem.GetItemCaption();
			var captionStyle = StyleHelper.GetCaptionStyle(graph, LayoutItem);
			if(captionLocation == LayoutItemCaptionLocation.Top || captionLocation == LayoutItemCaptionLocation.Bottom)
				return CalcItemSizeByTopBottomCaption(graph, caption, captionStyle, maxCellWidth, useMaxCaptionColumnWidth);
			return CalcItemSizeByLeftRightCaption(graph, caption, captionStyle, maxCellWidth, useMaxCaptionColumnWidth);
		}
		protected Size CalcItemSizeByTopBottomCaption(BrickGraphics graph, string caption, BrickStyle captionStyle, int maxCellWidth, bool useMaxCaptionColumnWidth) {
			CaptionSize = StyleHelper.CalcTextSize(graph, caption, captionStyle, maxCellWidth);
			DataObjectSize = GetDataObjectSize(graph, StyleHelper, maxCellWidth);
			return new Size(Math.Max(CaptionSize.Width, DataObjectSize.Width), CaptionSize.Height + DataObjectSize.Height);
		}
		Size CalcItemSizeByLeftRightCaption(BrickGraphics graph, string caption, BrickStyle captionStyle, int maxCellWidth, bool useMaxCaptionColumnWidth) {
			if(CaptionSize.Width > maxCellWidth) {
				CaptionSize = StyleHelper.CalcTextSize(graph, caption, captionStyle, maxCellWidth / 2);
				DataObjectSize = GetDataObjectSize(graph, StyleHelper, maxCellWidth - CaptionSize.Width);
				return new Size(maxCellWidth, Math.Max(CaptionSize.Height, DataObjectSize.Height));
			}
			var maxColumnCaptionWidth = useMaxCaptionColumnWidth ? PrintInfoGroupOwner.GetMaxColumnCaptionWidth(this) : 0;
			var maxCaptionWidth = maxColumnCaptionWidth != 0 ? maxColumnCaptionWidth : maxCellWidth;
			CaptionSize = StyleHelper.CalcTextSize(graph, caption, captionStyle, maxCaptionWidth);
			DataObjectSize = GetDataObjectSize(graph, StyleHelper, maxCellWidth - maxColumnCaptionWidth);
			var width = DataObjectSize.Width;
			width += maxColumnCaptionWidth != 0 ? maxColumnCaptionWidth : CaptionSize.Width;
			if(width > maxCellWidth) {
				var koef = (float)CaptionSize.Width / maxCellWidth;
				if(koef >= 0.5f)
					CaptionSize = StyleHelper.CalcTextSize(graph, caption, captionStyle, maxCellWidth / 2);
				DataObjectSize = GetDataObjectSize(graph, StyleHelper, maxCellWidth - CaptionSize.Width);
			}
			return new Size(maxCellWidth, Math.Max(CaptionSize.Height, DataObjectSize.Height));
		}
		protected virtual LayoutItemCaptionLocation GetCaptionLocationType() {
			return LayoutItem is LayoutItem ? ((LayoutItem)LayoutItem).CaptionSettings.Location : LayoutItemCaptionLocation.NotSet;
		}
		protected virtual Rectangle GetObjectRect(CardViewPrinter printer, int maxCellWidth) {
			var styleHelper = printer.StyleHelper;
			var graph = printer.Graph;
			if(graph == null || LayoutItem == null)
				return Rectangle.Empty;
			var location = Point.Empty;
			switch(GetCaptionLocationType()) {
				case LayoutItemCaptionLocation.Top:
					location = new Point(CaptionRect.Left, CaptionRect.Bottom);
					break;
				case LayoutItemCaptionLocation.Bottom:
					location = new Point(Left, LocationRect.Top);
					break;
				case LayoutItemCaptionLocation.Left:
				case LayoutItemCaptionLocation.NotSet:
					var maxColumnCaptionWidth = PrintInfoGroupOwner.GetMaxColumnCaptionWidth(this);
					location = new Point(Left + maxColumnCaptionWidth, Y + LocationRect.Height / 2 - DataObjectSize.Height / 2);
					break;
				case LayoutItemCaptionLocation.Right:
					location = new Point(LocationRect.X, Y + (LocationRect.Height - DataObjectSize.Height) / 2);
					break;
			}
			return new Rectangle(location, DataObjectSize);
		}
		protected abstract Size GetDataObjectSize(BrickGraphics graph, CardViewStyleHelper styleHelper, int maxCellWidth);
		public override void Draw() {
			DrawCaption();
			DrawDataObject();
		}
		protected virtual void DrawCaption() {
			PrintingHelper.DrawTextBrick(Printer.Graph, new CardViewDrawTextBrickArgs(Caption, StyleHelper.GetCaptionStyle(Printer.Graph, LayoutItem), CaptionRect, Column, CardIndex));
		}
		protected virtual void DrawDataObject() {
			var style = StyleHelper.GetCellStyle(Printer.Graph, Column, CardIndex, false, Printer.TextBuilder.GetColumnDisplayControlDefaultAlignment(Column), false);
			DrawTextBrick(Printer.Graph, Text, style, ObjectRect);
		}
		protected void DrawTextBrick(BrickGraphics graph, string text, BrickStyle style, Rectangle rect) {
			var args = new CardViewDrawTextBrickArgs(text, style, rect, Column, CardIndex);
			if(object.Equals(args.TextValue, DateTime.MinValue))
				args.TextValue = string.Empty; 
			var e = Printer.Exporter.RaiseRenderBrick(args.CardIndex, args.Column, Printer.DataProxy, args.Style, args.Text, args.TextValue, args.TextValueFormatString, args.Url, null);
			if(e != null) {
				args.Text = e.Text;
				args.TextValue = e.TextValue;
				args.TextValueFormatString = e.TextValueFormatString;
				args.Url = e.Url;
				args.Style = e.BrickStyle;
			}
			PrintingHelper.DrawTextBrick(graph, args);
		}
	}
	public abstract class CardViewPrintInfoBase {
		protected const int DefaultEditorHeight = 24;
		protected const int CellPadding = 4;
		protected const int BorderSize = 1;
		protected const int LayoutCellContentMargin = BorderSize + CellPadding;
		protected const int DefaultCellHeight = DefaultEditorHeight + LayoutCellContentMargin * 2;
		protected const int DefaultCellWidth = 20;
		int leftPadding = CellPadding;
		int topPadding = CellPadding;
		int rightPadding = CellPadding;
		int bottomPadding = CellPadding;
		int leftMargin = LayoutCellContentMargin;
		int topMargin = LayoutCellContentMargin;
		int rightMargin = LayoutCellContentMargin;
		int bottomMargin = LayoutCellContentMargin;
		int cardIndex = -1;
		string caption;
		CardViewGroupPrintInfoBase printInfoGroupOwner;
		CardViewColumnLayoutItem cardViewLayoutItem;
		Rectangle locationRect;
		Rectangle captionRect;
		CardViewPrintInfoBase[] nextColumnSiblings;
		CardViewPrintInfoBase[] prevColumnSiblings;
		CardViewPrintInfoBase[] nextRowSiblings;
		CardViewPrintInfoBase[] prevRowSiblings;
		public CardViewPrintInfoBase(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase layoutItem, int startCol, int startRow) {
			PrintInfoGroupOwner = printInfoOwner;
			LayoutItem = layoutItem;
			StartCol = startCol;
			StartRow = startRow;
		}
		public CardViewPrintInfoBase DependentColumnSibling { get; set; }
		public CardViewGroupPrintInfoBase PrintInfoGroupOwner {
			get { return printInfoGroupOwner; }
			private set {
				printInfoGroupOwner = value;
				if(printInfoGroupOwner != null)
					CardIndex = printInfoGroupOwner.CardIndex;
			}
		}
		public CardViewPrintInfoBase StartItem { get { return PrintInfoGroupOwner.PrintInfoMap[StartCol, StartRow]; } }
		public Point Location {
			get { return LocationRect.Location; }
			set {
				var rect = LocationRect;
				rect.Location = value;
				LocationRect = rect;
			}
		}
		public Rectangle LocationRect {
			get {
				if(locationRect.IsEmpty)
					locationRect = new Rectangle(LeftMargin, TopMargin, DefaultCellWidth, DefaultCellHeight);
				return locationRect;
			}
			set { locationRect = value; }
		}
		public virtual int LeftMargin { get { return leftMargin; } set { leftMargin = value; } }
		public virtual int RightMargin { get { return rightMargin; } set { rightMargin = value; } }
		public virtual int TopMargin { get { return topMargin; } set { topMargin = value; } }
		public virtual int BottomMargin { get { return bottomMargin; } set { bottomMargin = value; } }
		public int HorizontalMargins { get { return LeftMargin + RightMargin; } }
		public int VerticalMargins { get { return TopMargin + BottomMargin; } }
		public virtual int LeftPadding { get { return leftPadding; } set { leftPadding = value; } }
		public virtual int RightPadding { get { return rightPadding; } set { rightPadding = value; } }
		public virtual int TopPadding { get { return topPadding; } set { topPadding = value; } }
		public virtual int BottomPadding { get { return bottomPadding; } set { bottomPadding = value; } }
		public int HorizontalPaddings { get { return LeftPadding + RightPadding; } }
		public int VerticalPaddings { get { return TopPadding + BottomPadding; } }
		public CardViewPrintInfoBase[] NextColumnSiblings {
			get {
				if(nextColumnSiblings == null)
					nextColumnSiblings = new CardViewPrintInfoBase[LayoutItem.RowSpan];
				return nextColumnSiblings;
			}
		}
		public CardViewPrintInfoBase[] PrevColumnSiblings {
			get {
				if(prevColumnSiblings == null)
					prevColumnSiblings = new CardViewPrintInfoBase[LayoutItem.RowSpan];
				return prevColumnSiblings;
			}
		}
		public CardViewPrintInfoBase[] NextRowSiblings {
			get {
				if(nextRowSiblings == null)
					nextRowSiblings = new CardViewPrintInfoBase[LayoutItem.ColSpan];
				return nextRowSiblings;
			}
		}
		public CardViewPrintInfoBase[] PrevRowSiblings {
			get {
				if(prevRowSiblings == null)
					prevRowSiblings = new CardViewPrintInfoBase[LayoutItem.ColSpan];
				return prevRowSiblings;
			}
		}
		public int CellWidth { get; set; }
		public int Width {
			get { return LocationRect.Width; }
			set {
				var rect = LocationRect;
				if(rect.Width != value) {
					rect.Width = value;
					LocationRect = rect;
				}
			}
		}
		public int Height {
			get { return LocationRect.Height; }
			set {
				var rect = LocationRect;
				if(rect.Height != value) {
					rect.Height = value;
					LocationRect = rect;
				}
			}
		}
		public int X {
			get { return LocationRect.X; }
			set {
				var rect = LocationRect;
				if(rect.X != value) {
					rect.X = value;
					LocationRect = rect;
				}
			}
		}
		public int Y {
			get { return LocationRect.Y; }
			set {
				var rect = LocationRect;
				if(rect.Y != value) {
					rect.Y = value;
					LocationRect = rect;
				}
			}
		}
		protected int CardWidth { get { return Printer.CardWidth; } }
		public int Left { get { return LocationRect.Left; } }
		public int Right { get { return LocationRect.Right; } }
		public int Bottom { get { return LocationRect.Bottom; } }
		public Size CaptionSize { get; set; }
		public int StartCol { get; set; }
		public int StartRow { get; set; }
		public LayoutItemBase LayoutItem { get; set; }
		public CardViewColumnLayoutItem CardViewLayoutItem {
			get {
				if(cardViewLayoutItem == null)
					cardViewLayoutItem = LayoutItem as CardViewColumnLayoutItem;
				return cardViewLayoutItem;
			}
		}
		public CardViewPrinter Printer { get; set; }
		protected CardViewStyleHelper StyleHelper { get { return Printer.StyleHelper; } }
		public bool IsColumnSibling { get; set; }
		public int CardIndex { get { return cardIndex; } protected set { cardIndex = value; } }
		public virtual string Caption {
			get {
				if(caption == null)
					caption = LayoutItem != null && LayoutItem.GetShowCaption() ? LayoutItem.GetItemCaption() : string.Empty;
				return caption;
			}
		}
		public virtual Rectangle CaptionRect {
			get {
				if(captionRect.IsEmpty)
					captionRect = GetItemCaptionRect(Width);
				return captionRect;
			}
		}
		public void Calculate(CardViewPrinter printer, int maxWidth) {
			Width = maxWidth;
			Printer = printer;
			CalculateCore(maxWidth);
		}
		protected virtual void CalculateCore(int maxWidth) { 
		}
		public void CalculateSize(CardViewPrinter printer, Point location, int maxCellWidth) {
			var graph = printer.Graph;
			if(graph == null)
				return;
			UpdateLocation(location);
			CalculateInnerSize(printer, maxCellWidth);
			CalculateColumnSiblingSizes();
			CompensateColumnSiblings();
		}
		protected abstract void CalculateInnerSize(CardViewPrinter printer, int maxCellWidth);
		protected void CalculateColumnSiblingSizes() {
			var location = new Point(Right + RightMargin, Y);
			var height = 0;
			var nextSiblings = NextColumnSiblings.Where(s => s != null && s.IsColumnSibling).ToArray();
			foreach(var sibling in nextSiblings) {
				sibling.CalculateSize(Printer, location, PrintInfoGroupOwner.GetMaxItemWidth(sibling));
				var newHeight = sibling.Height;
				height += newHeight;
				location.Y += newHeight;
			}
		}
		protected void CompensateColumnSiblings() {
			var nextSiblingsRowHeight = 0;
			var nextSiblings = NextColumnSiblings.Where(s => s != null).ToArray();
			var nextSiblingsRowSpans = 0;
			foreach(var sibling in nextSiblings) {
				var startSibling = sibling.StartItem;
				nextSiblingsRowSpans += startSibling.LayoutItem.RowSpan;
				nextSiblingsRowHeight += startSibling.Height;
				nextSiblingsRowHeight += VerticalMargins;
			}
			CompensateColumnSiblingsCore(nextSiblings, nextSiblingsRowHeight, nextSiblingsRowSpans);
		}
		void CompensateColumnSiblingsCore(CardViewPrintInfoBase[] nextSiblings, int nextSiblingsRowHeight, int nextSiblingsRowSpans) {
			var lastNextSibling = nextSiblings.LastOrDefault();
			if(lastNextSibling == null)
				return;
			var targetLastSibling = lastNextSibling.StartItem;
			var prevSiblings = targetLastSibling.PrevColumnSiblings.Where(s => s != null).ToArray();
			var lastPrevSibling = prevSiblings.Last();
			if(lastPrevSibling != null && lastPrevSibling.LayoutItem.Path == LayoutItem.Path) {
				var prevSiblingsRowSpans = 0;
				var prevSiblingsRowHeight = 0;
				var firstTop = 0;
				foreach(var prevSibling in prevSiblings) {
					var startPrevSibling = prevSibling.StartItem;
					var top = startPrevSibling.Y;
					if(firstTop == 0)
						firstTop = top;
					prevSiblingsRowSpans += startPrevSibling.LayoutItem.RowSpan;
					prevSiblingsRowHeight = startPrevSibling.Height + top - firstTop;
					prevSiblingsRowHeight += prevSibling.VerticalMargins;
				}
				var deltaRowSpan = prevSiblingsRowSpans - nextSiblingsRowSpans;
				var deltaRowHeight = 0;
				if(deltaRowSpan != 0)
					deltaRowHeight = prevSiblingsRowHeight - nextSiblingsRowHeight * prevSiblingsRowSpans / deltaRowSpan;
				else
					deltaRowHeight = prevSiblingsRowHeight - nextSiblingsRowHeight;
				if(deltaRowHeight < 0)
					AlignPrevColumnSiblings(prevSiblings, nextSiblingsRowHeight);
				else if(deltaRowHeight > 0)
					AlignSiblings(nextSiblings, prevSiblingsRowHeight);
			}
		}
		void AlignPrevColumnSiblings(CardViewPrintInfoBase[] prevSiblings, int nextSiblingsRowHeight) {
			AlignSiblings(prevSiblings, nextSiblingsRowHeight);
			foreach(var sibling in prevSiblings)
				AlignPrevColumnSiblings(sibling.PrevColumnSiblings.Where(s => s != null).ToArray(), nextSiblingsRowHeight);
		}
		void AlignSiblings(CardViewPrintInfoBase[] siblings, int height) {
			var siblingsCount = siblings.Length;
			if(siblingsCount <= 0)
				return;
			var deltaRowHeight = Convert.ToInt32(Math.Ceiling(height / (double)siblingsCount));
			foreach(var sibling in siblings) {
				var startSibling = sibling.StartItem;
				var offset = (deltaRowHeight - startSibling.Height) / 2;
				startSibling.MoveItemVertical(offset);
			}
		}
		public virtual void UpdateLocation(Point nextLocation) {
			var actualSiblings = PrevRowSiblings.Where(s => s != null).ToArray();
			foreach(var sibling in actualSiblings) {
				if(sibling == null)
					continue;
				var startItem = sibling.StartItem;
				var locationY = startItem.Bottom + startItem.BottomMargin;
				if(nextLocation.Y < locationY) {
					var delta = locationY - nextLocation.Y;
					PrintInfoGroupOwner.Height += delta;
					nextLocation.Y = locationY;
				}
			}
			Location = nextLocation;
		}
		public virtual void MoveItemVertical(int offset) {
			Y += offset;
		}
		public void AddNextColumnSibling(CardViewPrintInfoBase sibling) {
			if(AddSiblingCore(NextColumnSiblings, LayoutItem, sibling))
				sibling.IsColumnSibling = true;
		}
		public void AddPrevColumnSibling(CardViewPrintInfoBase sibling) {
			if(AddSiblingCore(PrevColumnSiblings, LayoutItem, sibling)) {
				var lastIndex = PrevColumnSiblings.Length - 1;
				if(lastIndex > 0) {
					sibling.DependentColumnSibling = this;
					if(PrevColumnSiblings[lastIndex - 1] != null)
						PrevColumnSiblings[lastIndex - 1].DependentColumnSibling = null;
				}
			}
		}
		public void AddNextRowSibling(CardViewPrintInfoBase sibling) {
			AddSiblingCore(NextRowSiblings, LayoutItem, sibling);
		}
		public void AddPrevRowSibling(CardViewPrintInfoBase sibling) {
			AddSiblingCore(PrevRowSiblings, LayoutItem, sibling);
		}
		bool AddSiblingCore(CardViewPrintInfoBase[] siblings, LayoutItemBase targetLayoutItem, CardViewPrintInfoBase sibling) {
			if(!IsValidInfoItem(sibling) || targetLayoutItem.Path == sibling.LayoutItem.Path)
				return false;
			var group = targetLayoutItem as LayoutGroupBase;
			if(group != null && group.FindItemOrGroupByPath(group.Path + "_" + sibling.LayoutItem.Path) != null) 
				return false;
			for(var i = 0; i < siblings.Length; ++i) {
				if(siblings[i] == null) {
					siblings[i] = sibling;
					break;
				}
			}
			return true;
		}
		bool IsValidInfoItem(CardViewPrintInfoBase infoItem) {
			return infoItem != null && !(infoItem is CardViewEmptyBusyPrintInfo) && infoItem.LayoutItem != null;
		}
		public abstract void Draw();
		protected virtual Rectangle GetItemCaptionRect(int maxCellWidth) {
			var styleHelper = Printer.StyleHelper;
			var graph = Printer.Graph;
			if(graph == null || CardViewLayoutItem == null)
				return Rectangle.Empty;
			var captionLocation = CardViewLayoutItem.CaptionSettings.Location;
			var captionStyle = styleHelper.GetCaptionStyle(graph, LayoutItem);
			var location = Location;
			var locationType = CardViewLayoutItem.CaptionSettings.Location;
			var maxCaptionWidth = GetCaptionWidthByLocation(locationType, maxCellWidth);
			switch(locationType) {
				case LayoutItemCaptionLocation.Top:
					location = new Point(Location.X, LocationRect.Top);
					break;
				case LayoutItemCaptionLocation.Bottom:
					location = new Point(Location.X, LocationRect.Bottom - CaptionSize.Height);
					break;
				case LayoutItemCaptionLocation.Left:
				case LayoutItemCaptionLocation.NotSet:
					location = new Point(Location.X, Location.Y + LocationRect.Height / 2 - CaptionSize.Height / 2);
					break;
				case LayoutItemCaptionLocation.Right:
					location = new Point(LocationRect.Right - CaptionSize.Width, Location.Y + LocationRect.Height / 2 - CaptionSize.Height / 2);
					break;
			}
			return new Rectangle(location, CaptionSize);
		}
		protected virtual int GetCaptionWidthByLocation(LayoutItemCaptionLocation captionType, int cellWidth) {
			if(captionType == LayoutItemCaptionLocation.Top || captionType == LayoutItemCaptionLocation.Bottom)
				return cellWidth;
			return cellWidth / 2;
		}
		protected bool IsAltRow() {
			return false;
		}
	}
	public class CardViewEmptyBusyPrintInfo : CardViewBusyPrintInfo {
		public CardViewEmptyBusyPrintInfo(int height) 
			: base(null, new EmptyLayoutItem(), height) {
		}
	}
	public class CardViewBusyPrintInfo : CardViewPrintInfoBase {
		public CardViewBusyPrintInfo(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase item, int groupHeight)
			: base(printInfoOwner, item, -1, -1) {
		}
		public CardViewBusyPrintInfo(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase item, int groupHeight, int startCol, int startRow)
			: base(printInfoOwner, item, startCol, startRow) {
			StartCol = startCol;
			StartRow = startRow;
		}
		protected override void CalculateInnerSize(CardViewPrinter printer, int maxCellWidth) { }
		public override void Draw() { }
	}
	public static class PrintingHelper {
		public static EditPropertiesBase GetColumnExportProperties(IWebGridDataColumn column) {
			return column != null ? column.Adapter.ExportPropertiesEdit : null;
		}
		public static void DrawTextBrick(BrickGraphics graph, string text, Rectangle rect, BrickStyle style) {
			var temp = graph.DefaultBrickStyle;
			graph.DefaultBrickStyle = style;
			var textBrick = new TextBrick() { Text = text };
			DrawBrickCore(graph, textBrick, rect.Left, rect.Top, rect.Width, rect.Height);
			graph.DefaultBrickStyle = temp;
		}
		public static void DrawTextBrick(BrickGraphics graph, CardViewDrawTextBrickArgs args) {
			var temp = graph.DefaultBrickStyle;
			graph.DefaultBrickStyle = args.Style;
			var textBrick = new TextBrick();
			textBrick.Url = args.Url;
			textBrick.Text = args.Text;
			textBrick.TextValue = args.TextValue;
			textBrick.TextValueFormatString = args.TextValueFormatString;
			textBrick.XlsExportNativeFormat = args.XlsExportNativeFormat;
			DrawBrickCore(graph, textBrick, args.Left, args.Top, args.Width, args.Height);
			graph.DefaultBrickStyle = temp;
		}
		public static void DrawImageBrick(BrickGraphics graph, BrickStyle style, Rectangle rect, DevExpress.XtraPrinting.ImageSizeMode sizeMode, string url, byte[] imageValue) {
			var temp = graph.DefaultBrickStyle;
			graph.DefaultBrickStyle = style;
			var imageBrick = new ImageBrick();
			imageBrick.SizeMode = sizeMode;
			imageBrick.Url = url;
			imageBrick.Image = ByteImageConverter.FromByteArray(imageValue);
			imageBrick.DisposeImage = true;
			DrawBrickCore(graph, imageBrick, rect.Left, rect.Top, rect.Width, rect.Height);
			graph.DefaultBrickStyle = temp;
		}
		public static void DrawCheckBoxBrick(BrickGraphics graph, BrickStyle style, bool isChecked, string displayText, Rectangle rect) {
			var temp = graph.DefaultBrickStyle;
			graph.DefaultBrickStyle = style;
			var brick = new XECheckBoxBrick();
			brick.Checked = isChecked;
			brick.CheckText = displayText;
			DrawBrickCore(graph, brick, rect.Left, rect.Top, rect.Width, rect.Height);
			graph.DefaultBrickStyle = temp;
		}
		static void DrawBrickCore(BrickGraphics graph, Brick brick, int left, int top, int width, int height) {
			var rect = new RectangleF(left, top, width, height);
			graph.DrawBrick(brick, rect);
		}
	}
	public class CardViewDrawTextBrickArgs {
		public CardViewDrawTextBrickArgs(string text, BrickStyle style, Rectangle rect, CardViewColumn column, int cardIndex) {
			Text = text;
			Style = style;
			Left = rect.Left;
			Top = rect.Top;
			Width = rect.Width;
			Height = rect.Height;
			Column = column;
			CardIndex = cardIndex;
			TextValue = null;
			XlsExportNativeFormat = DefaultBoolean.Default;
		}
		public string Text { get; set; }
		public object TextValue { get; set; }
		public string TextValueFormatString { get; set; }
		public string Url { get; set; }
		public BrickStyle Style;
		public int Left { get; set; }
		public int Top { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int CardIndex { get; set; }
		public CardViewColumn Column { get; private set; }
		public DefaultBoolean XlsExportNativeFormat { get; set; }
	}
}
