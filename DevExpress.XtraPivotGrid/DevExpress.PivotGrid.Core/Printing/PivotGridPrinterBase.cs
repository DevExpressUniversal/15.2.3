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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrintingLinks;
namespace DevExpress.PivotGrid.Printing {
	public abstract class PivotGridPrinterBase : IDisposable { 
		public const int RowsPerBand = 50;
		IPivotGridPrinterOwner owner;
		PivotGridData data;
		IPivotGridOptionsPrintOwner optionsPrintOwner;
		IPrintingSystem printingSystem;
		ComponentPrinterBase componentPrinter;
		ILink link;
		IBrickGraphics graph;
		PivotPrintHeaders rowHeaders, columnHeaders, dataHeaders, filterHeaders;
		PivotGridBestFitterBase bestFitter;
		public PivotGridBestFitterBase BestFitter {
			get {
				if(bestFitter == null)
					bestFitter = CreatePivotPrintBestFitter();
				return bestFitter;
			}
		}
		protected virtual PivotPrintBestFitter CreatePivotPrintBestFitter() {
			return new PivotPrintBestFitter(Data, this);
		}
		protected PivotPrintHeaders ColumnHeaders {
			get {
				if(columnHeaders == null)
					columnHeaders = new PivotPrintHeaders(PivotArea.ColumnArea, data, BestFitter, OptionsPrint);
				return columnHeaders;
			}
		}
		protected PivotPrintHeaders RowHeaders {
			get {
				if(rowHeaders == null)
					rowHeaders = new PivotPrintHeaders(PivotArea.RowArea, data, BestFitter, OptionsPrint);
				return rowHeaders;
			}
		}
		protected PivotPrintHeaders FilterHeaders {
			get {
				if(filterHeaders == null)
					filterHeaders = new PivotPrintHeaders(PivotArea.FilterArea, data, BestFitter, OptionsPrint);
				return filterHeaders;
			}
		}
		protected PivotPrintHeaders DataHeaders {
			get {
				if(dataHeaders == null)
					dataHeaders = new PivotPrintHeaders(PivotArea.DataArea, data, BestFitter, OptionsPrint);
				return dataHeaders;
			}
		}
		protected internal CellSizeProvider CellSizeProvider { get { return BestFitter.CellSizeProvider; } }
		public IPivotGridPrinterOwner Owner { get { return owner; } set { owner = value; } }
		protected PivotGridData Data { get { return data; } }
		protected PivotVisualItemsBase VisualItems { get { return data.visualItems; } }
		protected virtual IPivotGridOptionsPrintOwner OptionsPrintOwner { get { return optionsPrintOwner; } }
		protected PivotGridOptionsPrint OptionsPrint { get { return (OptionsPrintOwner ?? data).OptionsPrint; } }
		protected PivotGridOptionsViewBase OptionsView { get { return data.OptionsView; } }
		protected bool PrintHeadersOnEveryPage { get { return OptionsPrint.PrintHeadersOnEveryPage; } }
		protected IPrintingSystem PrintingSystem { get { return printingSystem; } }
		protected ILink Link { get { return link; } }
		public bool IsPrintingAvailable { get { return ComponentPrinterBase.IsPrintingAvailable(false); } }
		protected internal
#if DEBUGTEST
			virtual
#endif
			IBrickGraphics Graph { get { return graph; } }
		public virtual UserControl PropertyEditorControl { get { return null; } }
		public ComponentPrinterBase ComponentPrinter {
			get {
				if(componentPrinter == null)
					componentPrinter = new ComponentPrinterDynamic(owner);
				if(OptionsPrint != null) {
					componentPrinter.SetPageSettings(OptionsPrint.PageSettings.ToPageSettings());
					UpdateVerticalContentSplitting(componentPrinter.PrintingSystemBase.Document);
				}
				return componentPrinter;
			}
		}
		protected int ColumnAreaHeight { get { return CellSizeProvider.GetHeightDifference(true, 0, VisualItems.GetLevelCount(true)); } }
		public bool ShowVertLines {
			get {
				if(OptionsPrint.PrintVertLines == DefaultBoolean.Default)
					return OptionsView.ShowVertLines;
				return OptionsPrint.PrintVertLines == DefaultBoolean.True ? true : false;
			}
		}
		public bool ShowHorzLines {
			get {
				if(OptionsPrint.PrintHorzLines == DefaultBoolean.Default)
					return OptionsView.ShowHorzLines;
				return OptionsPrint.PrintHorzLines == DefaultBoolean.True ? true : false;
			}
		}
		protected virtual int ColumnAreaX { get { return CellSizeProvider.GetWidthDifference(false, 0, VisualItems.GetItemsCreator(false).GetUnpagedItems().LevelCount); } }
		protected int ColumnHeadersY {
			get {
				int y = FilterHeaders.Height;
				if(y > 0 && OptionsView.ShowFilterSeparatorBar)
					y += GetFilterSeparatorHeight();
				return y;
			}
		}
		protected int ColumnAreaY {
			get {
				int y;
				if(RowHeaders.Height + DataHeaders.Height < ColumnHeaders.Height + ColumnAreaHeight)
					y = ColumnHeadersY + Math.Max(DataHeaders.Height, ColumnHeaders.Height);
				else
					y = ColumnHeadersY + DataHeaders.Height + RowHeaders.Height - ColumnAreaHeight;
				return Math.Max(y, ColumnHeadersY + DataHeaders.Height);
			}
		}
		protected int RowHeadersY {
			get {
				if(RowHeaders.Height + DataHeaders.Height + ColumnHeadersY < ColumnAreaY + ColumnAreaHeight)
					return ColumnAreaY + ColumnAreaHeight - RowHeaders.Height;
				else
					return ColumnHeadersY + DataHeaders.Height;
			}
		}
		protected virtual int RowAreaY {
			get {
				if(PrintHeadersOnEveryPage)
					return 0;
				else
					return ColumnAreaY + ColumnAreaHeight;
			}
		}
		protected virtual int RowCount { get { return VisualItems.GetItemsCreator(false).UnpagedItemsCount; } }
		protected PivotGridPrinterBase(IPivotGridPrinterOwner owner, PivotGridData data) : this(owner, data, null) {
		}
		protected PivotGridPrinterBase(IPivotGridPrinterOwner owner, PivotGridData data, IPivotGridOptionsPrintOwner optionsPrintOwner) {
			this.owner = owner;
			this.data = data;
			this.optionsPrintOwner = optionsPrintOwner ?? data;
			if(data == null)
				return;
			Data.VisualItems.Cleared += OnPivotChangedCleared;
			Data.FieldSizeChanged += OnPivotChangedCleared;
		}
		protected internal abstract IPivotPrintAppearance GetCellAppearance(PivotGridCellItem cell, Rectangle? bounds);
		protected internal abstract IPivotPrintAppearance GetValueAppearance(PivotGridValueType valueType, PivotFieldItemBase field);
		protected internal abstract IPivotPrintAppearance GetFieldAppearance(PivotFieldItemBase field);
		protected internal abstract IPivotPrintAppearance GetCellAppearance();
		protected internal abstract IPivotPrintAppearance GetTotalCellAppearance();
		protected internal abstract IPivotPrintAppearance GetGrandTotalCellAppearance();
		public virtual void AcceptChanges() { }
		public void RejectChanges() { }
		public void ShowHelp() { }
		public bool SupportsHelp() { return false; }
		public virtual bool HasPropertyEditor() { return false; } 
		public virtual void Dispose() {
			Release();
			if(this.componentPrinter != null) {
				componentPrinter.Dispose();
				componentPrinter = null;
			}
			if(data == null || owner == null)
				return;
			VisualItems.Cleared -= OnPivotChangedCleared;
			Data.FieldSizeChanged -= OnPivotChangedCleared;
			this.owner = null;
			this.data = null;
		}
		public virtual void Release() {
			if(this.printingSystem != null) {
				this.printingSystem.AfterChange -= OnAfterChange;
				this.printingSystem = null;
			}
			this.link = null;
		}
		public void Initialize(IPrintingSystem ps, ILink link) {
			if(this.printingSystem != ps) {
				this.printingSystem = ps;
				this.printingSystem.AfterChange += OnAfterChange;
			}
			this.link = link;
			Clear();
			PrintingSystemBase psb = ps as PrintingSystemBase;
			if(psb == null)
				return;
			UpdateVerticalContentSplitting(psb.Document);
			VisualItems.EnsureIsCalculated();
		}
		protected void Clear() {
			rowHeaders = null;
			columnHeaders = null;
			dataHeaders = null;
			filterHeaders = null;
			if(bestFitter != null) {
				bestFitter.CellSizeProvider.Clear();
				bestFitter = null;
			}
		}
		void OnPivotChangedCleared(object sender, EventArgs e) {
			Clear();
		}
		public void PerformPrintingExportAction(Action0 action) {
			ComponentPrinter.ClearDocument();
			action();
		}
		public void PerformPrintingAction(Action0 action) {
			if(IsPrintingAvailable) {
				ComponentPrinter.ClearDocument();
				action();
			}
		}
		public void CreateArea(string areaName, IBrickGraphics graph) {
			SetGraph(graph);
			switch(areaName) {
				case SR.MarginalHeader:
					CreateHeader();
					break;
				case SR.Detail:
					CreateDetails();
					break;
				case SR.DetailHeader:
					CreateDetailHeader();
					break;
			}
		}
		protected void CreateHeader() {
		}
		protected void CreateDetailHeader() {
			if(PrintHeadersOnEveryPage) {
				DrawHeaderBricks();
			}
		}
		protected void CreateDetails() {
			if(!PrintHeadersOnEveryPage) {
				DrawHeaderBricks();
			}
			DrawDetailBricks();
		}
		void DrawHeaderBricks() {
			DrawHeaders();
			DrawColumns();
		}
		protected void DrawHeaders() {
			for(int i = 0; i < 4; i++)
				DrawHeader((PivotArea)i);
		}
		protected void DrawHeader(PivotArea area) {
			PivotPrintHeaders headers;
			if(area == PivotArea.RowArea)
				headers = RowHeaders;
			else
				if(area == PivotArea.ColumnArea)
					headers = ColumnHeaders;
				else
					if(area == PivotArea.DataArea)
						headers = DataHeaders;
					else
						headers = FilterHeaders;
			if(headers.Width == 0 || headers.Height == 0)
				return;
			BrickStyle defBrick = Graph.DefaultBrickStyle;
			int y = 0;
			if(area == PivotArea.ColumnArea || area == PivotArea.DataArea)
				y = ColumnHeadersY;
			else
				if(area == PivotArea.RowArea)
					y = RowHeadersY;
			for(int i = 0; i < headers.Count; i++) {
				IVisualBrick brick = DrawHeaderBrick(headers[i], new Rectangle(headers.GetItemWidthOffset(i), y, headers.GetItemWidth(i), headers.Height));
			}
			Graph.DefaultBrickStyle = defBrick;
		}
		protected virtual int GetFilterSeparatorHeight() {
			return OptionsPrint.FilterSeparatorBarPadding >= 0 ? OptionsPrint.FilterSeparatorBarPadding : 3;
		}
		protected virtual IVisualBrick DrawHeaderBrick(PivotFieldItemBase field, Rectangle bounds) {
			IPivotPrintAppearance appearance = GetFieldAppearance(field);
			SetDefaultBrickStyle(appearance, new Padding(CellSizeProvider.FieldValueTextOffset, 0, 0, 0));
			IVisualBrick brick = DrawTextBrick(field.HeaderDisplayText, bounds);
			brick.Separable = false;
			if(Owner != null && Owner.CustomExportHeader(ref brick, field, appearance, ref bounds))
				ApplyAppearanceToBrickStyle(brick, appearance);
			DrawBrickCore(brick, bounds);
			return brick;
		}
		void ApplyAppearanceToBrickStyle(IVisualBrick brick, IPivotPrintAppearance appearance) {
			IPanelBrick panelBrick = brick as IPanelBrick;
			if(panelBrick != null) {
				foreach(IVisualBrick item in panelBrick.Bricks)
					ApplyAppearanceToBrickStyleCore(item, appearance);
			}
			ApplyAppearanceToBrickStyleCore(brick, appearance);
		}
		void ApplyAppearanceToBrickStyleCore(IVisualBrick brick, IPivotPrintAppearance appearance) {
			BrickStyle brickStyle = brick.Style != null ?
				(BrickStyle)brick.Style.Clone() :
				(BrickStyle)Graph.DefaultBrickStyle.Clone();
			if(appearance.Options.UseBackColor)
				brickStyle.BackColor = appearance.BackColor;
			if(appearance.Options.UseBorderColor)
				brickStyle.BorderColor = appearance.BorderColor;
			if(appearance.Options.UseBorderWidth)
				brickStyle.BorderWidth = appearance.BorderWidth;
			if(appearance.Options.UseBorderStyle)
				brickStyle.BorderStyle = appearance.BorderStyle;
			if(appearance.Options.UseFont)
				brickStyle.Font = appearance.Font;
			if(appearance.Options.UseForeColor)
				brickStyle.ForeColor = appearance.ForeColor;
			if(appearance.Options.UseTextOptions) {
				brickStyle.TextAlignment = TextAlignmentConverter.ToTextAlignment(appearance.TextHorizontalAlignment, appearance.TextVerticalAlignment);
				brickStyle.StringFormat = new BrickStringFormat(appearance.StringFormat);
			}
			brickStyle.StringFormat.PrototypeKind = BrickStringFormatPrototypeKind.GenericTypographic;
			brick.Style = brickStyle;
		}
		protected void DrawColumns() {
			DrawColumnFieldValues();
		}
		protected void DrawColumnFieldValues() {
			int columnAreaX = ColumnAreaX;
			int columnAreaY = ColumnAreaY;
			BrickStyle defBrick = Graph.DefaultBrickStyle;
			PivotFieldValueItem lastFirstLevelItem = null;
			int widthPrinted = 0;
			try {
				PivotVisualItemsBase visualItems = VisualItems;
				int unpagedCount = visualItems.GetItemsCreator(true).UnpagedItemsCount;
				for(int i = 0; i < unpagedCount ; i++) {
					PivotFieldValueItem item = visualItems.GetItemsCreator(true).GetUnpagedItem(i);
					if(item.StartLevel == 0) {
						if(lastFirstLevelItem != null) {
							widthPrinted += CellSizeProvider.GetWidthDifference(true, lastFirstLevelItem.MinLastLevelIndex, lastFirstLevelItem.MaxLastLevelIndex + 1);
						}
						widthPrinted += CellSizeProvider.GetFieldValuePrintSeparator(item);
						lastFirstLevelItem = item;
					}
					PivotFieldValueItem[] children = new PivotFieldValueItem[0];
					if(!OptionsPrint.IsMergeFieldValues(true)) {
						children = item.GetLastLevelCells();
					}
					int x = widthPrinted + columnAreaX + CellSizeProvider.GetWidthDifference(true, lastFirstLevelItem.MinLastLevelIndex, item.MinLastLevelIndex);
					int y = CellSizeProvider.GetHeightDifference(true, 0, item.StartLevel) + columnAreaY;
					if(children.Length == 0) {
						int width = CellSizeProvider.GetWidthDifference(true, item.MinLastLevelIndex, item.MaxLastLevelIndex + 1);
						int height = CellSizeProvider.GetHeightDifference(true, item.StartLevel, item.EndLevel + 1);
						DrawFieldValue(item, new Rectangle(x, y, width, height));
					} else {
						DrawUnmergeFieldValue(item, children, x, y);
					}
				}
			} finally {
				Graph.DefaultBrickStyle = defBrick;
			}
		}
		protected void DrawDetailBricks() {
			int rowAreaY = RowAreaY;
			BrickStyle defBrick = Graph.DefaultBrickStyle;
			int offsetY = rowAreaY;
			int rowsPrinted = 0, lastBandRows = 0, heightPrinted = 0;
			PivotFieldValueItem lastFirstLevelItem = null;
			try {
				for(int i = 0; i < RowCount; i++) {
					PivotFieldValueItem fieldValueInfo = VisualItems.GetItemsCreator(false).GetUnpagedItem(i);
					if(fieldValueInfo.StartLevel == 0) {
						if(lastFirstLevelItem != null) {
							DrawCells(offsetY + heightPrinted, lastFirstLevelItem);
							rowsPrinted += lastFirstLevelItem.MaxLastLevelIndex - lastFirstLevelItem.MinLastLevelIndex + 1;
							heightPrinted += CellSizeProvider.GetHeightDifference(false, lastFirstLevelItem.MinLastLevelIndex, lastFirstLevelItem.MaxLastLevelIndex + 1);
							heightPrinted += CellSizeProvider.GetFieldValuePrintSeparator(lastFirstLevelItem);
							if(rowsPrinted - lastBandRows >= RowsPerBand) {
								((BrickGraphics)Graph).Modifier = BrickModifier.None;
								((BrickGraphics)Graph).Modifier = BrickModifier.Detail;
								offsetY = -heightPrinted;
								lastBandRows = rowsPrinted;
							}
						}
						lastFirstLevelItem = fieldValueInfo;
					}
					PivotFieldValueItem[] children;
					if(!OptionsPrint.IsMergeFieldValues(false))
						children = fieldValueInfo.GetLastLevelCells();
					else
						children = new PivotFieldValueItem[0];
					int x = CellSizeProvider.GetWidthDifference(false, 0, fieldValueInfo.StartLevel);
					int y = offsetY + heightPrinted + CellSizeProvider.GetFieldValuePrintSeparator(lastFirstLevelItem);
					if(fieldValueInfo.StartLevel != 0)
						y += CellSizeProvider.GetHeightDifference(false, (lastFirstLevelItem == null ? 0 : lastFirstLevelItem.MinLastLevelIndex), fieldValueInfo.MinLastLevelIndex);
					if(children.Length == 0)
						DrawFieldValue(fieldValueInfo, new Rectangle(
							x,
							y,
							CellSizeProvider.GetWidthDifference(false, fieldValueInfo.StartLevel, fieldValueInfo.EndLevel + 1),
							CellSizeProvider.GetHeightDifference(false, fieldValueInfo.MinLastLevelIndex, fieldValueInfo.MaxLastLevelIndex + 1)));
					else
						DrawUnmergeFieldValue(fieldValueInfo, children, x, y);
				}
				if(lastFirstLevelItem != null)
					DrawCells(offsetY + heightPrinted, lastFirstLevelItem);
			} finally {
				Graph.DefaultBrickStyle = defBrick;
			}
		}
		protected void DrawUnmergeFieldValue(PivotFieldValueItem fieldValue, PivotFieldValueItem[] children, int x, int y) {
			for(int i = fieldValue.MinLastLevelIndex; i <= fieldValue.MaxLastLevelIndex; i++) {
				Rectangle bounds = new Rectangle();
				if(fieldValue.IsColumn) {
					bounds.X = x + CellSizeProvider.GetWidthDifference(true, fieldValue.MinLastLevelIndex, i);
					bounds.Y = y;
					bounds.Width = CellSizeProvider.GetWidthDifference(true, i, i + 1);
					bounds.Height = CellSizeProvider.GetHeightDifference(true, fieldValue.StartLevel, fieldValue.EndLevel + 1);
				} else {
					bounds.X = x;
					bounds.Y = y + CellSizeProvider.GetHeightDifference(false, fieldValue.MinLastLevelIndex, i);
					bounds.Width = CellSizeProvider.GetWidthDifference(false, fieldValue.StartLevel, fieldValue.EndLevel + 1);
					bounds.Height = CellSizeProvider.GetHeightDifference(false, i, i + 1);
				}
				DrawFieldValue(fieldValue, bounds);
			}
		}
		protected virtual void DrawFieldValue(PivotFieldValueItem fieldValue, Rectangle bounds) {
			IPivotPrintAppearance appearance = GetValueAppearance(fieldValue.ValueType, fieldValue.Field);
			bool wrapText = (appearance.StringFormat.FormatFlags & StringFormatFlags.NoWrap) != StringFormatFlags.NoWrap;
			if(appearance.TextVerticalAlignment == VertAlignment.Default)
				appearance.TextVerticalAlignment = wrapText ? VertAlignment.Top : VertAlignment.Center;
			SetDefaultBrickStyle(appearance, new Padding(CellSizeProvider.FieldValueTextOffset, 0, CellSizeProvider.FieldValueTextOffset, 0));
			FormatInfo format = fieldValue.Field != null ? fieldValue.Field.GetValueFormat(fieldValue.ValueType) : null;
			string formatString = format == null ? "" : format.FormatString;
			int height = wrapText ? bounds.Height : CellSizeProvider.GetDefaultFieldValueHeight(fieldValue);
			Rectangle textBounds = Rectangle.Empty;
			if(bounds.Height > height) {
				textBounds.Height = height;
				textBounds.Width = bounds.Width;
			}
			IVisualBrick brick = DrawFieldValueBrick(fieldValue, bounds, textBounds, formatString);
			bool allowDefaultNativeFormat = !(fieldValue.IsCustomDisplayText
				|| fieldValue.ValueType == PivotGridValueType.Total
				|| fieldValue.ValueType == PivotGridValueType.CustomTotal);
			ApplyXlsExportNativeFormat(brick, fieldValue.Field,
				!allowDefaultNativeFormat ? DefaultBoolean.False : DefaultBoolean.Default);
			SetTextBrickVerticalAlignment(brick, appearance.TextVerticalAlignment);
			if(Owner != null && Owner.CustomExportFieldValue(ref brick, fieldValue, appearance, ref bounds))
				ApplyAppearanceToBrickStyle(brick, appearance);
			DrawBrickCore(brick, bounds);
			if(fieldValue.IsLastFieldLevel)
				brick.Separable = false;
			else {
				brick.SeparableHorz = fieldValue.IsColumn;
				brick.SeparableVert = !fieldValue.IsColumn;
			}
		}
		protected virtual void DrawCells(int heightPrinted, PivotFieldValueItem firstLevelItemRowItem) {
			int y = heightPrinted;
			for(int j = firstLevelItemRowItem.MinLastLevelIndex; j <= firstLevelItemRowItem.MaxLastLevelIndex; j++) {
				int x = ColumnAreaX;
				Rectangle lastCellBounds = Rectangle.Empty;
				PivotGridCellItem item = null;
				PivotVisualItemsBase visualItems = VisualItems;
				for(int i = 0; i < visualItems.GetItemsCreator(true).LastLevelUnpagedItemCount; i++) {
					item = visualItems.CreateUnpagedCellItem(i, j);
					if(i == 0)
						y += CellSizeProvider.GetFieldValuePrintSeparator(item.RowFieldValueItem);
					x += CellSizeProvider.GetFieldValuePrintSeparator(item.ColumnFieldValueItem);
					lastCellBounds = DrawCell(item, x, y);
					x += lastCellBounds.Width;
				}
				y += lastCellBounds.Height;
			}
		}
		protected virtual Rectangle DrawCell(PivotGridCellItem pivotGridCellItem, int x, int y) {
			Rectangle bounds = new Rectangle(x,
													y,
													CellSizeProvider.GetWidthDifference(true, pivotGridCellItem.ColumnIndex, pivotGridCellItem.ColumnIndex + 1),
													CellSizeProvider.GetHeightDifference(false, pivotGridCellItem.RowIndex, pivotGridCellItem.RowIndex + 1));
			IPivotPrintAppearance appearance = GetCellAppearance(pivotGridCellItem, bounds);
			if(appearance.TextHorizontalAlignment == HorzAlignment.Default)
				appearance.TextHorizontalAlignment = HorzAlignment.Far;
			IVisualBrick brick = DrawCellBrick(appearance, bounds, pivotGridCellItem);
			ApplyXlsExportNativeFormat(brick, pivotGridCellItem.DataField, DefaultBoolean.Default);
			SetBorderSides(brick, pivotGridCellItem);
			if(Owner != null) {
				if(Owner.CustomExportCell(ref brick, pivotGridCellItem, appearance, GetPageUnit(), ref bounds))
					ApplyAppearanceToBrickStyle(brick, appearance);
			}
			DrawBrickCore(brick, bounds);
			return bounds;
		}
		protected virtual IVisualBrick DrawCellBrick(IPivotPrintAppearance appearance, Rectangle bounds, PivotGridCellItem pivotGridCellItem) {
			SetDefaultBrickStyle(appearance, new Padding(CellSizeProvider.LeftCellPadding, CellSizeProvider.TopCellPadding, CellSizeProvider.RightCellPadding, CellSizeProvider.BottomCellPadding));
			FormatInfo cellFormat = pivotGridCellItem.GetCellFormatInfo();
			string formatString = cellFormat == null ? "" : cellFormat.FormatString;
			if(pivotGridCellItem.ShowKPIGraphic)
				return DrawImageBrick(pivotGridCellItem, bounds, data.GetKPIBitmap(pivotGridCellItem.KPIGraphic, pivotGridCellItem.KPIValue));
			else
				return DrawTextBrick(pivotGridCellItem.Text, pivotGridCellItem.Value, bounds, formatString);
		}
		protected virtual IVisualBrick DrawFieldValueBrick(PivotFieldValueItem fieldValue, Rectangle bounds, Rectangle textBounds, string formatString) {
			object value = fieldValue.Value;
			if(fieldValue.IsOthersRow || (fieldValue.Field != null && fieldValue.Field.Area == PivotArea.DataArea))
				value = fieldValue.DisplayText;
			if(fieldValue.DisplayText == "" && string.IsNullOrEmpty(formatString) && !fieldValue.IsCustomDisplayText && fieldValue.ValueType == PivotGridValueType.Value)
				value = null;
			return DrawFieldValueBrickCore(fieldValue.DisplayText, value, bounds, textBounds, formatString);
		}
		protected virtual IVisualBrick DrawFieldValueBrickCore(string text, object textValue, Rectangle bounds, Rectangle textBounds, string textValueFormatString) {
			ITextBrick textBrick;
			if(textBounds.IsEmpty)
				return DrawTextBrick(text, textValue, bounds, textValueFormatString);
			textBrick = CreateTextBrick();
			textBrick.Text = text;
			if(textValue != null)
				textBrick.TextValue = textValue;
			textBrick.TextValueFormatString = textValueFormatString;
			textBrick.Rect = (RectangleF)textBounds;
			textBrick.Sides = BorderSide.All ^ BorderSide.Bottom;
			IPanelBrick brick;
			if(textBrick == null)
				brick = new PanelBrick();
			else
				brick = new PivotPanelBrick(textBrick);
			brick.Bricks.Add(textBrick);
			return brick;
		}
		protected virtual ITextBrick DrawTextBrick(string text, object textValue, Rectangle bounds, string textValueFormatString) {
			ITextBrick brick = CreateTextBrick();
			brick.Text = text;
			if(textValue != null)
				brick.TextValue = textValue;
			brick.TextValueFormatString = textValueFormatString;
			return brick;
		}
		protected ITextBrick DrawTextBrick(string text, Rectangle bounds) {
			return DrawTextBrick(text, text, bounds, "");
		}
		protected IImageBrick DrawImageBrick(PivotGridCellItem cellItem, Rectangle bounds, Bitmap bitmap) {
			IImageBrick brick = CreateImageBrick();
			brick.Image = bitmap;
			brick.SizeMode = ImageSizeMode.CenterImage;
			return brick;
		}
		protected virtual ITextBrick CreateTextBrick() {
			ITextBrick brick = PrintingSystem.CreateTextBrick();
			return brick;
		}
		protected virtual IImageBrick CreateImageBrick() {
			return PrintingSystem.CreateImageBrick();
		}		
		protected virtual void DrawBrickCore(IBrick brick, RectangleF rect) {
			Graph.DrawBrick(brick, rect);
		}
		void SetTextBrickVerticalAlignment(IVisualBrick brick, VertAlignment vertAlignment) {
			ITextBrick textBrick = brick as ITextBrick;
			if(textBrick != null) {
				textBrick.VertAlignment = vertAlignment;
				return;
			}
			IPanelBrick textContainerBrick = brick as IPanelBrick;
			if(textContainerBrick != null) {
				foreach(IVisualBrick item in textContainerBrick.Bricks) {
					ITextBrick textBrick1 = item as ITextBrick;
					if(textBrick1 != null)
						textBrick1.VertAlignment = vertAlignment;
				}
			}
		}
		void SetDefaultBrickStyle(IPivotPrintAppearance appearance, Padding padding) {
			if(appearance == null)
				return;
			Graph.DefaultBrickStyle = CreateBrickStyle(appearance, padding);
		}
		protected BrickStyle CreateBrickStyle(IPivotPrintAppearance appearance, Padding padding) {
			BrickStyle brickStyle = CreateBrick(appearance, BorderSide.All, appearance.BorderColor, 1);
			brickStyle.TextAlignment = TextAlignmentConverter.ToTextAlignment(appearance.TextHorizontalAlignment, appearance.TextVerticalAlignment);
			brickStyle.Padding = new PaddingInfo(padding.Left, padding.Right, padding.Top, padding.Bottom, GraphicsUnit.Pixel);
			brickStyle.StringFormat = new BrickStringFormat(appearance.StringFormat);
			brickStyle.StringFormat.PrototypeKind = BrickStringFormatPrototypeKind.GenericTypographic;
			return brickStyle;
		}
		public static BrickStyle CreateBrick(IPivotPrintAppearance appearance, BorderSide borderSide, Color borderColor, int borderWidth) {
			if(borderColor == Color.Empty)
				borderColor = Color.Gray;
			BrickStyle brick = new BrickStyle(borderSide, borderWidth, borderColor, appearance.BackColor,
				appearance.ForeColor, appearance.Font, new BrickStringFormat(appearance.StringFormat));
			return brick;
		}
		protected void ApplyXlsExportNativeFormat(IVisualBrick brick, PivotFieldItemBase dataField, DefaultBoolean defaultFormat) {
			ITextBrick textBrick = brick as ITextBrick;
			if(textBrick == null) {
				IPanelBrick panelBrick = brick as IPanelBrick;
				if(panelBrick == null || panelBrick.Bricks.Count != 1)
					return;
				textBrick = panelBrick.Bricks[0] as ITextBrick;
			}
			if(textBrick == null)
				return;
			textBrick.XlsExportNativeFormat = dataField != null ? dataField.UseNativeFormat : DefaultBoolean.Default;
			if(textBrick.XlsExportNativeFormat == DefaultBoolean.Default)
				textBrick.XlsExportNativeFormat = defaultFormat;
			if(textBrick.XlsExportNativeFormat == DefaultBoolean.False && !string.IsNullOrEmpty(textBrick.Text)) {
				textBrick.TextValue = textBrick.Text;
				textBrick.TextValueFormatString = null;
			}
		}
		protected void SetBorderSides(IVisualBrick brick, PivotGridCellItem pivotGridCellItem) {
			if(!ShowHorzLines  || !ShowVertLines) {
				BorderSide sides = BorderSide.None;
				if(pivotGridCellItem.RowIndex == 0)
					sides |= BorderSide.Top;
				if(pivotGridCellItem.RowIndex == RowCount - 1)
					sides |= BorderSide.Bottom;
				if(pivotGridCellItem.ColumnIndex == 0)
					sides |= BorderSide.Left;
				if(pivotGridCellItem.ColumnIndex == VisualItems.ColumnCount - 1)
					sides |= BorderSide.Right;
				if(OptionsPrint.PrintHorzLines != DefaultBoolean.False)
					sides |= BorderSide.Top | BorderSide.Bottom;
				if(OptionsPrint.PrintVertLines != DefaultBoolean.False)
					sides |= BorderSide.Left | BorderSide.Right;
				brick.Sides = sides;
			}
		}
		internal GraphicsUnit GetPageUnit() {
			return GraphicsUnit.Pixel;
		}
		void SetGraph(IBrickGraphics graph) {
			this.graph = graph;
		}
		void OnAfterChange(object sender, ChangeEventArgs e) {
			if(PrintingSystem == null || Link == null)
				return;
			switch(e.EventName) {
				case SR.PageSettingsChanged:
				case SR.AfterMarginsChange:
					((PrintableComponentLinkBase)Link).Margins = ((PrintingSystemBase)PrintingSystem).PageMargins;
					Link.CreateDocument();
					break;
			}
		}
		void UpdateVerticalContentSplitting(Document document) {
			PrintingDocument doc = document as PrintingDocument;
			if(doc == null)
				return;
			doc.VerticalContentSplitting = OptionsPrint.VerticalContentSplitting;
		}
		public void SetCommandsVisibility(IPrintingSystem ps) {
			ps.SetCommandVisibility(PrintingSystemCommand.ExportCsv, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportFile, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportGraphic, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportHtm, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportMht, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportPdf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportRtf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportTxt, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportXls, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportXlsx, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendCsv, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendFile, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendGraphic, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendMht, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendPdf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendRtf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendTxt, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendXls, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendXlsx, true);
		}
	}
	class PivotPanelBrick : PanelBrick {
		ITextBrick textBrick;
		public PivotPanelBrick(ITextBrick textBrick) {
			this.textBrick = textBrick;
		}
		public override string Text {
			get { return textBrick.Text; }
			set { textBrick.Text = value; }
		}
		public override object TextValue {
			get { return textBrick.TextValue; }
			set { textBrick.TextValue = value; }
		}
		public override string TextValueFormatString {
			get { return textBrick.TextValueFormatString; }
			set { textBrick.TextValueFormatString = value; }
		}
	}
}
