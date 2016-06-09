#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.PivotGrid.Printing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.DashboardExport {
	public class DashboardGridPrinter : DashboardCellItemPrinter, IPivotGridPrinterOwner, ICellValueGetter, IDashboardItemFooterProvider {
		const int MaxTextLength = 10000;
		const int DefaultColumnMinWidth = 10;
		const int BrickHorizontalPadding = 2;
		readonly GridDashboardItemViewControl viewControl;
		readonly GridDashboardItemViewModel gridViewModel;
		readonly GridPrinterSourceListWrapper sourceList;
		readonly IList<IGridColumn> columns;
		readonly DashboardSparklinePainterCore sparklinePainter = new DashboardSparklinePainterCore();
		readonly Dictionary<GridColumnViewModel, DashboardSparklineCalculator> sparklineCalculatorsCache = new Dictionary<GridColumnViewModel, DashboardSparklineCalculator>();
		readonly ColumnsWidthOptionsInfo initialWidthOptions;
		readonly ItemViewerClientState clientState;
		readonly float charWidth;
		IList<int> selectedIndices = new List<int>();
		int maxFieldHeigth = -1;
		GridDeltaInfo deltaInfo;
		readonly GridBestFitter gridBestFitter;
		readonly PrintingSystemBase ps = new PrintingSystemBase();
		readonly XRGridFooterPanel footer;
		readonly int footerHeight = 0;
		readonly int hashCode;
		readonly int columnOffset;
		int topVisibleTotalIndex = -1;
		event EventHandler<GridCustomDrawCellEventArgsBase> customDrawCell;
		Dictionary<int, MergedCellInfo> mergedCellInfos = new Dictionary<int, MergedCellInfo>();
		public event EventHandler<GridCustomDrawCellEventArgsBase> CustomDrawCell {
			add { customDrawCell += value; }
			remove { customDrawCell -= value; }
		}
		int TotalItemHeight { get { return CellSizeProvider.DefaultFieldValueHeight; } }
		GridDeltaInfo DeltaInfo {
			get {
				if(deltaInfo == null)
					deltaInfo = new GridDeltaInfo(new CardStyleProperties());
				return deltaInfo;
			}
		}
		int ICellValueGetter.RowCount { get { return Data.VisualItems.RowCount; } }
		object ICellValueGetter.GetCellValue(int columnIndex, int rowIndex) {
			return Data.VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public bool CreatesIntersectedBricks {
			get { return false; }
		}
		protected override int ColumnAreaX {
			get { return 0; }
		}
		protected override int RowAreaY {
			get {
				return gridViewModel.ShowColumnHeaders ? base.RowAreaY : 0;
			}
		}
		protected override int RowCount { get { return VisibleRowCount; } }
		public DashboardGridPrinter(GridDashboardItemViewControl viewControl, ExportGridControl exportControl, IList selectedValues, GridDashboardItemViewModel gridControl, PrintAppearance appearance, ItemContentOptions options, DashboardExportMode mode, ItemViewerClientState clientState, int pathRowIndex, int pathColumnIndex)
			: base(null, exportControl.PivotData, appearance) {
			this.viewControl = viewControl;
			this.gridViewModel = gridControl;
			this.sourceList = exportControl.DataSource != null ? new GridPrinterSourceListWrapper(exportControl.DataSource, pathRowIndex) : null;
			Owner = this;
			ApplyPrintingSettings(options);
			CalculateSelectedIndex(mode == DashboardExportMode.EntireDashboard ? selectedValues : null);
			PrepareColumns();
			Font cellFont = Appearances.Cell.Font;
			this.hashCode = GetHashCode();
			this.columnOffset = pathColumnIndex;
			this.columns = ((IGridControl)exportControl).Columns;
			this.clientState = clientState;
			this.gridBestFitter = new GridBestFitter(this);
			this.ps.Graph.Font = cellFont;
			this.ps.Graph.PageUnit = GraphicsUnit.Pixel;
			this.charWidth = FontMeasurer.MeasureMaxDigitWidthF(cellFont);
			this.initialWidthOptions = GetInitialWidthOptionsInfo(clientState, mode);
			int totalsCount = gridViewModel.TotalsCount;
			if(totalsCount > 0) {
				footer = new XRGridFooterPanel(cellFont, viewControl.DataController);
				footerHeight = CalculateVisibleFooterHeight(totalsCount);
				clientState.ViewerArea.Height -= footerHeight;
			}
			VisibleDataAreaHeight = clientState.ViewerArea.Height - RowAreaY;
			VisibleDataAreaWidth = clientState.ViewerArea.Width;
			VisibleRowCount = CalcVisibleRowCount(mode, pathRowIndex);
			if(mode == DashboardExportMode.SingleItem)
				ColumnWidths = CalcColumnWidths();
			else
				CalcScrollVisibilityAndColumnsWidths(mode, clientState, pathRowIndex, pathColumnIndex);
			BestFitter.SetSizeProvider(new GridCellSizeProvider(Data, Data.VisualItems, this, ColumnWidths, maxFieldHeigth, columnOffset));
		}
		int CalculateVisibleFooterHeight(int totalsCount) {
			int count = totalsCount;
			int height = 0;
			if (count > 0) {
				int totalItemHeight = TotalItemHeight;
				int areaHeight = clientState.ViewerArea.Height;
				for (int i = 1; i <= count; i++) {
					if (areaHeight <= totalItemHeight) {
						count = i;
						break;
					}
					else
						areaHeight -= totalItemHeight;
				}
				height = totalItemHeight * count;
				topVisibleTotalIndex = totalsCount - count;
			}
			return height;
		}
		protected override List<int> CalcColumnWidths() {
			int maxVisibleWidth = clientState.ViewerArea.Width;
			List<int> columnWidths = new List<int>();
			ColumnsWidthOptionsInfo optionsInfo = ColumnWidthCalculator.CalcActualWidth(initialWidthOptions, maxVisibleWidth);
			foreach(ColumnWidthOptionsInfo info in optionsInfo.ColumnsInfo)
				columnWidths.Add(info.ActualWidth);
			return columnWidths;
		}
		ColumnsWidthOptionsInfo GetInitialWidthOptionsInfo(ItemViewerClientState clientState, DashboardExportMode mode) {
			if(mode == DashboardExportMode.EntireDashboard && clientState.SpecificState != null && clientState.SpecificState.ContainsKey("ColumnsWidthOptionsState")) {
				ColumnsWidthOptionsInfo optionsInfo = ((ColumnsWidthOptionsInfo)clientState.SpecificState["ColumnsWidthOptionsState"]).Clone();
				for(int i = 0; i < optionsInfo.ColumnsInfo.Count; i++) {
					ColumnWidthOptionsInfo columnInfo = optionsInfo.ColumnsInfo[i];
					columnInfo.InitialWidth = CalcInitialWidth(optionsInfo.ColumnWidthMode, columnInfo, i);
					columnInfo.MinWidth = DefaultColumnMinWidth;
					columnInfo.ActualWidth = 0;
				}
				return optionsInfo;
			}
			return CreateDefaultColumnsWidthOptionsInfo();
		}
		double CalcInitialWidth(GridColumnWidthMode widthMode, ColumnWidthOptionsInfo columnInfo, int i) {
			bool manualColumnWidthMode = widthMode == GridColumnWidthMode.Manual;
			double initialWidth = 0;
			if(!manualColumnWidthMode || columnInfo.WidthType == GridColumnFixedWidthType.FitToContent)
				initialWidth = GetBestFitWidth(gridViewModel.Columns[i], i);
			else if(manualColumnWidthMode && columnInfo.WidthType == GridColumnFixedWidthType.FixedWidth)
				initialWidth = (int)Math.Round(columnInfo.FixedWidth * charWidth);
			else
				initialWidth = columnInfo.Weight;
			return initialWidth;
		}
		ColumnsWidthOptionsInfo CreateDefaultColumnsWidthOptionsInfo() {
			ColumnsWidthOptionsInfo columnsWidthOptionsInfo = new ColumnsWidthOptionsInfo();
			columnsWidthOptionsInfo.ColumnWidthMode = GridColumnWidthMode.AutoFitToGrid;
			for(int i = 0; i < gridViewModel.Columns.Count; i++) {
				GridColumnViewModel col = gridViewModel.Columns[i];
				double initialWidth = GetBestFitWidth(col, i);
				ColumnWidthOptionsInfo columnInfo = new ColumnWidthOptionsInfo() {
					WidthType = GridColumnFixedWidthType.Weight,
					InitialWidth = initialWidth,
					MinWidth = DefaultColumnMinWidth,
					ActualIndex = col.ActualIndex,
					Weight = col.Weight,
					FixedWidth = col.FixedWidth,
					DisplayMode = col.DisplayMode,
					DefaultBestCharacterCount = col.DefaultBestCharacterCount
				};
				columnsWidthOptionsInfo.ColumnsInfo.Add(columnInfo);
			}
			return columnsWidthOptionsInfo;
		}
		double GetBestFitWidth(GridColumnViewModel columnModel, int columnIndex) {
			IGridColumn gridColumn = columns[columnIndex];
			int maxTextWidth = GetMaxTextWidth(columnModel, gridColumn.TextIsHidden);
			int captionTextWidth = GetTextWidth(columnModel.Caption);
			GridBestFitColumnInfo colInfo = new GridBestFitColumnInfo() {
				DisplayMode = columnModel.DisplayMode,
				DefaultBestCharacterCount = columnModel.DefaultBestCharacterCount,
				IgnoreDeltaIndication = columnModel.IgnoreDeltaIndication,
				Caption = columnModel.Caption,
				Index = columnIndex,
				MaxTextWidth = maxTextWidth,
				CaptionTextWidth = captionTextWidth,
				MaxIconStyleImageWidth = gridColumn.MaxIconStyleImageWidth,
				TextIsHidden = gridColumn.TextIsHidden
			};
			if(columnModel.DisplayMode == GridColumnDisplayMode.Sparkline) {
				DashboardSparklineCalculator calculator = new DashboardSparklineCalculator(GetSparklineCalculatorData(columnModel), GetSparklineCalculatorTexts(columnModel), columnModel.ShowStartEndValues);
				return gridBestFitter.GetSparklineBestFitWidth(colInfo, Appearances.Cell.Font, charWidth, calculator, BrickHorizontalPadding, BrickHorizontalPadding);
			}
			return gridBestFitter.GetBestFitWidth(colInfo, charWidth, BrickHorizontalPadding, BrickHorizontalPadding);
		}
		int GetTextWidth(string text) {
			int newlineIndex = text.IndexOf("\n");
			string substring = newlineIndex != -1 ? text.Substring(0, newlineIndex) : text;
			if(substring.Length > MaxTextLength)
				substring = text.Substring(0, MaxTextLength);
			SizeF size = ps.Graph.MeasureString(substring);
			return Convert.ToInt32(size.Width);
		}
		int GetMaxTextWidth(GridColumnViewModel columnModel, bool textIsHidden) {
			int maxTextWidth = 0;
			if(!textIsHidden && (columnModel.DisplayMode == GridColumnDisplayMode.Value || columnModel.DisplayMode == GridColumnDisplayMode.Delta)) {
				IList<string> texts = sourceList.GetAllDisplayTexts(columnModel.DataId);
				foreach(string text in texts) {
					int textWidth = GetTextWidth(text);
					maxTextWidth = Math.Max(maxTextWidth, textWidth);
				}
			}
			return maxTextWidth;
		}
		protected override PivotPrintBestFitter CreatePivotPrintBestFitter() {
			return new PivotPrintBestFitter(Data, this, new GridCellSizeProvider(Data, Data.VisualItems, this, ColumnWidths, maxFieldHeigth, columnOffset));
		}
		internal DashboardSparklineCalculator GetDashboardSparklineCalculator(GridColumnViewModel column) {
			return sparklineCalculatorsCache[column];
		}
		void ApplyPrintingSettings(ItemContentOptions options) {
			Data.OptionsView.ShowColumnGrandTotalHeader = false;
			Data.OptionsPrint.PrintColumnHeaders = DefaultBoolean.False;
			Data.OptionsPrint.PrintDataHeaders = DefaultBoolean.False;
			Data.OptionsPrint.PrintFilterHeaders = DefaultBoolean.False;
			Data.OptionsPrint.PrintRowHeaders = DefaultBoolean.False;
			bool printHeadersOnEveryPage = false;
			if(options != null && options.HeadersOptions != null)
				printHeadersOnEveryPage = options.HeadersOptions.PrintHeadersOnEveryPage;
			Data.OptionsPrint.PrintHeadersOnEveryPage = printHeadersOnEveryPage;
		}
		void CalculateSelectedIndex(IList selectedValues) {
			string[] selectionMembers = gridViewModel.SelectionDataMembers;
			if(sourceList != null && selectionMembers != null && selectionMembers.Length > 0 && selectedValues != null) {
				foreach(IList selectedValue in selectedValues) {
					if(selectionMembers.Length == selectedValue.Count) {
						for(int j = 0; j < sourceList.Count; j++) {
							AxisPoint axisPoint = sourceList[j] as AxisPoint;
							bool hit = true;
							for(int i = 0; i < selectedValue.Count; i++) {
								AxisPoint pointByMember = axisPoint.GetParentByDimensionID(selectionMembers[i]);
								if(!Object.Equals(pointByMember.UniqueValue, selectedValue[i])) {
									hit = false;
									break;
								}
							}
							if(hit) {
								selectedIndices.Add(j);
							}
						}
					}
				}
			}
		}
		void PrepareColumns() {
			for(int i = 0; i < gridViewModel.Columns.Count; i++) {
				GridColumnViewModel column = gridViewModel.Columns[i];
				if(column.DisplayMode == GridColumnDisplayMode.Image && sourceList != null && sourceList.Count > 0) {
					maxFieldHeigth = GetImageMaxHeight(column);
				}
				else if(column.DisplayMode == GridColumnDisplayMode.Sparkline && sourceList != null && sourceList.Count > 0)
					sparklineCalculatorsCache.Add(column, new DashboardSparklineCalculator(GetSparklineCalculatorData(column), GetSparklineCalculatorTexts(column), column.ShowStartEndValues));
			}
		}
		int GetImageMaxHeight(GridColumnViewModel column) {
			int maxHeight = -1;
			for(int j = 0; j < sourceList.Count; j++) {
				byte[] bytes = sourceList.GetPropertyValue(column.DataId, j) as byte[];
				Image image = null;
				if(bytes != null)
					image = DevExpress.XtraEditors.Controls.ByteImageConverter.FromByteArray(bytes);
				if(image != null) {
					maxHeight = Math.Max(maxHeight, image.Height);
				}
			}
			return maxHeight;
		}
		List<double> GetSparklineCalculatorData(GridColumnViewModel column) {
			List<double> data = new List<double>();
			for(int j = 0; j < sourceList.Count; j++) {
				try {
					double startValue = DevExpress.DashboardCommon.Native.Helper.ConvertToDouble(sourceList.GetPropertyValue(column.DataId + GridMultiDimensionalDataSource.SparklineStartValue, j));
					data.Add(startValue);
					double endValue = DevExpress.DashboardCommon.Native.Helper.ConvertToDouble(sourceList.GetPropertyValue(column.DataId + GridMultiDimensionalDataSource.SparklineEndValue, j));
					data.Add(endValue);
				}
				catch {
				}
			}
			return data;
		}
		List<string> GetSparklineCalculatorTexts(GridColumnViewModel column) {
			List<string> texts = new List<string>();
			for(int j = 0; j < sourceList.Count; j++) {
				try {
					string startText = sourceList.GetPropertyValue(column.DataId + GridMultiDimensionalDataSource.SparklineStartDisplayText, j) as string;
					if(startText != null)
						texts.Add(startText);
					string endText = sourceList.GetPropertyValue(column.DataId + GridMultiDimensionalDataSource.SparklineEndDisplayText, j) as string;
					if(endText != null)
						texts.Add(endText);
				}
				catch {
				}
			}
			return texts;
		}
		protected override IVisualBrick DrawCellBrick(IPivotPrintAppearance appearance, Rectangle bounds, PivotGridCellItem cellItem) {
			if(gridViewModel.Columns.Count == 0)
				return new VisualBrick();
			int columnIndex = cellItem.ColumnIndex + columnOffset;
			if(columnIndex >= gridViewModel.Columns.Count || sourceList == null || cellItem.RowValueType == PivotGridValueType.GrandTotal)
				return new VisualBrick();
			GridColumnViewModel column = gridViewModel.Columns[columnIndex];
			bool selectedRow = selectedIndices.Contains<int>(cellItem.RowIndex);
			Color backColor = appearance.BackColor;
			if(gridViewModel.EnableBandedRows && cellItem.RowIndex % 2 == 1)
				backColor = SystemColors.Control;
			if(selectedRow)
				backColor = SystemColors.Highlight;
			ExportGridCustomDrawCellEventArgs args = new ExportGridCustomDrawCellEventArgs(backColor, appearance.ForeColor, appearance.Font, column.DataId, sourceList.CorrectIndex(cellItem.RowIndex), false, selectedRow, ExportHelper.GetDefaultBackColor());
			if(customDrawCell != null)
				customDrawCell(this, args);
			PrintAppearanceStyleSettingsInfo styleSettings = (PrintAppearanceStyleSettingsInfo)args.StyleSettings;
			if(column.DisplayMode == GridColumnDisplayMode.Value) {
				string displayText = GetText(cellItem, column);
				if(styleSettings.Image != null)
					return DrawFormatConditionIconBrick(appearance, ref bounds, cellItem, styleSettings, displayText);
				if(styleSettings.Bar != null)
					return DrawFormatConditionBarBrick(appearance, cellItem, column, styleSettings, displayText);
				VisualBrick brick = (VisualBrick)base.DrawCellBrick(appearance, bounds, cellItem);
				brick.Text = displayText;
				brick.BackColor = styleSettings.BackColor;
				ITextBrick textBrick = brick as ITextBrick;
				if(textBrick != null) {
					textBrick.ForeColor = styleSettings.ForeColor;
					textBrick.Font = styleSettings.Font;
				}
				if(gridViewModel.AllowCellMerge && column.AllowCellMerge)
					MergeCellBricks(brick, columnIndex);
				return brick;
			}
			else {
				PanelBrick panel = CreatePanelBrick(appearance, bounds, cellItem, column, styleSettings);
				panel.BackColor = styleSettings.BackColor;
				return panel;
			}
		}
		string GetText(PivotGridCellItem cellItem, GridColumnViewModel column) {
			string text = (string)sourceList.GetPropertyValue(column.DataId + GridMultiDimensionalDataSource.DisplayTextPostfix, cellItem.RowIndex);
			return text.Length > MaxTextLength ? text.Substring(0, MaxTextLength) : text;
		}
		static IVisualBrick DrawFormatConditionIconBrick(IPivotPrintAppearance appearance, ref Rectangle bounds, PivotGridCellItem cellItem, PrintAppearanceStyleSettingsInfo styleSettings, string displayText) {
			FormatInfo cellFormat = cellItem.GetCellFormatInfo();
			string formatString = cellFormat == null ? String.Empty : cellFormat.FormatString;
			IVisualBrick panel = ExportImageStyleSettingsPainter.CreatePanelBrick(appearance, bounds, formatString, displayText, cellItem.Value, styleSettings);
			return panel;
		}
		IVisualBrick DrawFormatConditionBarBrick(IPivotPrintAppearance appearance, PivotGridCellItem cellItem, GridColumnViewModel column, PrintAppearanceStyleSettingsInfo styleSettings, string displayText) {
			object value = sourceList.GetPropertyValue(column.DataId, cellItem.RowIndex);
			BrickStyle brickStyle = new BrickStyle(BorderSide.All, 1, appearance.BorderColor, styleSettings.BackColor, styleSettings.ForeColor, styleSettings.Font, new BrickStringFormat(appearance.StringFormat));
			FormatConditionBarBrick barBrick = new FormatConditionBarBrick(brickStyle) { BarValue = value, Text = displayText };
			barBrick.Assign(styleSettings.Bar);
			return barBrick;
		}
		void MergeCellBricks(VisualBrick brick, int columnIndex) {
			if(mergedCellInfos.ContainsKey(columnIndex)) {
				MergedCellInfo mergedCellInfo = mergedCellInfos[columnIndex];
				if(mergedCellInfo.Text != brick.Text) {
					mergedCellInfo.MergeIndex++;
					mergedCellInfo.Brick = brick;
					mergedCellInfo.MultiKey = new MultiKey(hashCode, brick.Text, mergedCellInfo.MergeIndex);
				}
				else {
					object mergeValue;
					if(!mergedCellInfo.Brick.TryGetAttachedValue(BrickAttachedProperties.MergeValue, out mergeValue))
						mergedCellInfo.Brick.SetAttachedValue(BrickAttachedProperties.MergeValue, mergedCellInfo.MultiKey);
					brick.SetAttachedValue(BrickAttachedProperties.MergeValue, mergedCellInfo.MultiKey);
				}
			}
			else {
				int mergeIndex = 0;
				mergedCellInfos.Add(columnIndex, new MergedCellInfo() { MergeIndex = mergeIndex, Brick = brick, MultiKey = new MultiKey(hashCode, brick.Text, mergeIndex) });
			}
		}
		protected override ITextBrick DrawTextBrick(string text, object textValue, Rectangle bounds, string textValueFormatString) {
			ITextBrick brick = base.DrawTextBrick(text, textValue, bounds, textValueFormatString);
			brick.StringFormat = BrickStringFormat.Create(brick.Style.TextAlignment, false, StringTrimming.EllipsisCharacter);
			return brick;
		}
		PanelBrick CreatePanelBrick(IPivotPrintAppearance appearance, Rectangle bounds, PivotGridCellItem cellItem, GridColumnViewModel column, PrintAppearanceStyleSettingsInfo styleSettings) {
			PanelBrick panel = new PanelBrick();
			switch(column.DisplayMode) {
				case GridColumnDisplayMode.Bar:
					PrepareBarPanelBrick(panel, bounds, cellItem, column);
					break;
				case GridColumnDisplayMode.Delta:
					PrepareDeltaPanelBrick(panel, bounds, cellItem, column, styleSettings);
					break;
				case GridColumnDisplayMode.Sparkline:
					PrepareSparklinePanelBrick(panel, appearance, bounds, cellItem, column, styleSettings);
					break;
				case GridColumnDisplayMode.Image:
					PrepareImagePanelBrick(panel, bounds, cellItem);
					break;
				default:
					throw new Exception("Incorrect grid column display mode");
			}
			return panel;
		}
		void PrepareImagePanelBrick(PanelBrick panel, Rectangle bounds, PivotGridCellItem cellItem) {
			byte[] value = cellItem.Value as byte[];
			Image image = null;
			if(value != null)
				image = DevExpress.XtraEditors.Controls.ByteImageConverter.FromByteArray(value);
			panel.Rect = bounds;
			if(image != null)
				panel.Bricks.Add(new ImageBrick() {
					Image = image,
					Rect = new RectangleF(0, 0, Math.Min(image.Width, bounds.Width), Math.Min(image.Height, bounds.Height)),
					Sides = BorderSide.None
				});
		}
		void PrepareDeltaPanelBrick(PanelBrick panel, Rectangle bounds, PivotGridCellItem cellItem, GridColumnViewModel column, PrintAppearanceStyleSettingsInfo styleSettings) {
			DeltaValue delta = sourceList.GetPropertyValue(column.DataId + GridMultiDimensionalDataSource.DeltaDescriptorPostFix, cellItem.RowIndex) as DeltaValue;
			string displayText = (string)sourceList.GetPropertyValue(column.DataId + GridMultiDimensionalDataSource.DisplayTextPostfix, cellItem.RowIndex);
			List<VisualBrick> bricks = ExportDeltaPainter.CreateBrick(displayText, bounds, column,
				delta != null ? (IndicatorType?)delta.IndicatorType : null, delta != null && delta.IsGood, cellItem.Value, styleSettings, DeltaInfo);
			foreach(VisualBrick brick in bricks)
				panel.Bricks.Add(brick);
		}
		void PrepareBarPanelBrick(PanelBrick panel, Rectangle bounds, PivotGridCellItem cellItem, GridColumnViewModel column) {
			decimal normalizedValue = viewControl.NormalizeBarValue(column.DataId, sourceList.GetPropertyValue(column.DataId, cellItem.RowIndex));
			decimal zeroValue = viewControl.GetBarZeroPosition(column.DataId);
			List<VisualBrick> bricks = ExportBarPainter.CreateBrick(bounds, normalizedValue, zeroValue);
			foreach(VisualBrick brick in bricks)
				panel.Bricks.Add(brick);
		}
		void PrepareSparklinePanelBrick(PanelBrick panel, IPivotPrintAppearance appearance, Rectangle bounds, PivotGridCellItem cellItem, GridColumnViewModel column, PrintAppearanceStyleSettingsInfo styleSettings) {
			IList<double> values = cellItem.Value as IList<double>;
			if(values != null && values.Count > 0) {
				const int sparklinePadding = 1;
				Rectangle cellBounds = new Rectangle(sparklinePadding, sparklinePadding, bounds.Width - 2 * sparklinePadding, bounds.Height - 2 * sparklinePadding);
				ImageBrick sparklineBrick = new ImageBrick() {
					Rect = cellBounds,
					Sides = BorderSide.None,
					BackColor = Color.Empty
				};
				MultiKey key = new MultiKey(new object[] { values, column });
				Image image = PrintingSystem.Images.GetImageByKey(key);
				if(image == null) {
					Bitmap bitmap = new Bitmap(cellBounds.Width, cellBounds.Height);
					Graphics graphics = Graphics.FromImage(bitmap);
					Rectangle sparklineBounds = cellBounds;
					DashboardSparklineCalculator calculator = sparklineCalculatorsCache[column];
					double start = values.FirstOrDefault();
					double end = values.LastOrDefault();
					Font font = appearance.Font;
					if(column.ShowStartEndValues) {
						TextBrick leftTextBrick = new TextBrick() {
							Rect = calculator.GetStartValueBounds(start, cellBounds, graphics, font),
							Text = calculator.GetValueString(start, graphics, font),
							ForeColor = styleSettings.ForeColor,
							Sides = BorderSide.None,
							BackColor = styleSettings.BackColor,
							Font = styleSettings.Font,
							Padding = new PaddingInfo(BrickHorizontalPadding, 0, 0, 0)
						};
						leftTextBrick.Style.StringFormat = BrickStringFormat.Create(leftTextBrick.Style.TextAlignment, false, StringTrimming.EllipsisCharacter);
						panel.Bricks.Add(leftTextBrick);
						TextBrick rightTextBrick = new TextBrick() {
							Rect = calculator.GetEndValueBounds(end, cellBounds, graphics, font),
							Text = calculator.GetValueString(end, graphics, font),
							ForeColor = styleSettings.ForeColor,
							Sides = BorderSide.None,
							BackColor = styleSettings.BackColor,
							Font = styleSettings.Font,
							Padding = new PaddingInfo(0, BrickHorizontalPadding, 0, 0)
						};
						rightTextBrick.Style.StringFormat = BrickStringFormat.Create(rightTextBrick.Style.TextAlignment, false, StringTrimming.EllipsisCharacter);
						panel.Bricks.Add(rightTextBrick);
						sparklineBrick.Rect = calculator.GetSparklineStartEndBounds(cellBounds, graphics, font);
						sparklineBrick.Padding = new PaddingInfo(BrickHorizontalPadding, BrickHorizontalPadding, 0, 0);
					}
					using(DashboardSparklineInfo info = new DashboardSparklineInfo(column.SparklineOptions, values, sparklineBounds, new GraphicsCache(graphics), null)) {
						sparklinePainter.Draw(info);
					}
					image = PrintingSystem.Images.GetImage(key, new Bitmap(bitmap));
				}
				sparklineBrick.Image = image;
				panel.Bricks.Add(sparklineBrick);
			}
		}
		protected override void DrawFieldValue(PivotFieldValueItem fieldValue, Rectangle bounds) {
			if(fieldValue.IsColumn && gridViewModel.ShowColumnHeaders && gridViewModel.Columns.Count > 0)
				base.DrawFieldValue(fieldValue, bounds);
		}
		protected override IPivotPrintAppearance GetCellAppearance(PivotGridCellItem cell, Rectangle? bounds) {
			IPivotPrintAppearance appearance = base.GetCellAppearance(cell, bounds);
			if(cell.ColumnIndex >= gridViewModel.Columns.Count)
				return appearance;
			appearance.TextHorizontalAlignment = gridViewModel.Columns[cell.ColumnIndex].HorzAlignment == GridColumnHorzAlignment.Left ? HorzAlignment.Near : HorzAlignment.Far;
			return appearance;
		}
		public void Finalize(IPrintingSystem ps, ILink link) {
			Release();
		}
		bool IPivotGridPrinterOwner.CustomExportCell(ref IVisualBrick brick, PivotGridCellItem cellItem, IPivotPrintAppearance appearance, GraphicsUnit graphicsUnit, ref Rectangle rect) {
			return false;
		}
		bool IPivotGridPrinterOwner.CustomExportFieldValue(ref IVisualBrick brick, PivotFieldValueItem fieldValue, IPivotPrintAppearance appearance, ref Rectangle rect) {
			return false;
		}
		bool IPivotGridPrinterOwner.CustomExportHeader(ref IVisualBrick brick, PivotFieldItemBase field, IPivotPrintAppearance appearance, ref Rectangle rect) {
			return false;
		}
		int IDashboardItemFooterProvider.FooterHeight { get { return footerHeight; } }
		bool IDashboardItemFooterProvider.ShowFooter { get { return gridViewModel.ShowFooter; } }
		void IDashboardItemFooterProvider.AddFooterToDetailBand(DetailBand band, PointF footerLocation, float viewAreaTop) {
			if (gridViewModel.ShowFooter) {
				band.Controls.Add(footer);
				float footerY = footerLocation.Y > viewAreaTop ? footerLocation.Y : viewAreaTop;
				float footerWidth = ShowVScroll ? clientState.ViewerArea.Width + ExportScrollBar.PrintSize : clientState.ViewerArea.Width;
				footer.BoundsF = new RectangleF(footerLocation.X, footerY, footerWidth, footerHeight);
				footer.Initialize(gridViewModel.Columns, ColumnWidths, TotalItemHeight, topVisibleTotalIndex, columnOffset);
			}
		}
	}
	public class GridCellSizeProvider : PrintCellSizeProvider {
		readonly List<int> fieldWidths;
		readonly int maxFieldHeigth;
		readonly int columnOffset;
		internal GridCellSizeProvider(PivotGridData data, PivotVisualItemsBase visualItems, PivotGridPrinterBase printer, List<int> fieldWidths, int maxFieldHeigth, int columnOffset)
			: base(data, visualItems, printer) {
			this.fieldWidths = fieldWidths;
			this.maxFieldHeigth = maxFieldHeigth;
			this.columnOffset = columnOffset;
		}
		protected override int GetRowFieldHeight(PivotFieldValueItem item) {
			return Math.Max(maxFieldHeigth, base.GetRowFieldHeight(item));
		}
		protected override int GetColumnFieldWidth(PivotFieldValueItem item) {
			if(fieldWidths.Count == 0)
				return base.GetColumnFieldWidth(item);
			return fieldWidths[item.MinLastLevelIndex + columnOffset];
		}
	}
	public class MergedCellInfo {
		public string Text { get { return Brick.Text; } }
		public int MergeIndex { get; set; }
		public VisualBrick Brick { get; set; }
		public MultiKey MultiKey { get; set; }
	}
	public class ExportGridCustomDrawCellEventArgs : GridCustomDrawCellEventArgsBase {
		public ExportGridCustomDrawCellEventArgs(Color backColor, Color foreColor, Font font, string columnId, int rowIndex, bool isDarkSkin, bool ignoreColorAndBackColor, Color defaultBackColor)
			: base(columnId, rowIndex, isDarkSkin, ignoreColorAndBackColor, defaultBackColor) {
			StyleSettings = new PrintAppearanceStyleSettingsInfo(backColor, foreColor, font);
		}
	}
}
