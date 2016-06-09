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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Rendering;
using DevExpress.Web.Data;
using DevExpress.XtraExport;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.NativeBricks;
namespace DevExpress.Web.Export {
	public class ASPxGridViewPrintTextBuilder : GridViewTextBuilder {
		public ASPxGridViewPrintTextBuilder(ASPxGridView grid) : base(grid) { }
		protected override string GetEditorDisplayTextCore(EditPropertiesBase editor, IWebGridDataColumn column, int visibleIndex, object value, bool encodeValue = false) {
			return editor.GetExportDisplayText(GetDisplayControlArgsCore(column, visibleIndex, value));
		}
		public CreateDisplayControlArgs GetDisplayControlArgs(IWebGridDataColumn column, int visibleIndex) {
			return GetDisplayControlArgsCore(column, visibleIndex, DataProxy.GetRowValue(visibleIndex, column.FieldName));
		}
		protected internal override CreateDisplayControlArgs GetDisplayControlArgsCore(IWebGridDataColumn column, int visibleIndex, IValueProvider provider, ASPxGridBase grid, object value) {
			CreateDisplayControlArgs controlArgs = base.GetDisplayControlArgsCore(column, visibleIndex, provider, grid, value);
			controlArgs.EncodeHtml = false;
			return controlArgs;
		}
	}
	public class GridViewLinkWebStylePrintStyle {
		GridViewExportAppearance webStyle;
		BrickStyle brickStyle;
		GridViewColumn column;
		public GridViewLinkWebStylePrintStyle(GridViewExportAppearance webStyle, BrickStyle brickStyle, GridViewColumn column) {
			this.webStyle = webStyle;
			this.brickStyle = brickStyle;
			this.column = column;
		}
		public bool IsFit(GridViewExportAppearance webStyle, GridViewColumn column) {
			return WebStyle == webStyle && Column == column;
		}
		public GridViewExportAppearance WebStyle { get { return webStyle; } }
		public BrickStyle BrickStyle { get { return brickStyle; } }
		public GridViewColumn Column { get { return column; } }
	}
	public delegate void GridViewPrinterDrawDetailGrid(ASPxGridView detailGrid, int indent);
	public struct DrawTextBrickArgs {
		public string Text;
		public object TextValue;
		public string TextValueFormatString;
		public string Url;
		public BrickStyle Style;
		public int Left;
		public int Top;
		public int Width;
		public int Height;
		public GridViewColumn Column;
		public int VisibleIndex;
		public GridViewRowType RowType;
		public DefaultBoolean XlsExportNativeFormat;
		public DrawTextBrickArgs(string text, BrickStyle style,
			int left, int top, int width, int height, GridViewColumn column,
			int visibleIndex, GridViewRowType rowType) {
			Text = text;
			Style = style;
			Left = left;
			Top = top;
			Width = width;
			Height = height;
			Column = column;
			VisibleIndex = visibleIndex;
			RowType = rowType;
			TextValueFormatString = String.Empty;
			TextValue = null;
			Url = String.Empty;
			XlsExportNativeFormat = DefaultBoolean.Default;
		}
	}
	public class GridViewPrinter : GridPrinterBase {
		const string 
			CheckedDisplayText = "[+]",
			UncheckedDisplayText = "[-]";
		BrickGraphics graph;
		GridExportStyleHelper styleHelper;
		GridViewPrinterDrawDetailGrid onDrawDetailGrid;
		int indent;
		int level;
		ASPxGridViewPrintTextBuilder textBuilder;
		GridViewExportColumnHelper columnHelper;
		GridViewPrintInfo printInfo;
		int graphBrickTop;
		int dataRowIndex;
		public GridViewPrinter(ASPxGridViewExporter exporter, ASPxGridView grid, BrickGraphics graph, GridExportStyleHelper styleHelper, GridViewPrinterDrawDetailGrid onDrawDetailGrid, int level, int indent) 
			: base(exporter, grid) {
			this.graph = graph;
			this.styleHelper = styleHelper;
			this.onDrawDetailGrid = onDrawDetailGrid;
			this.indent = indent;
			this.level = level;
			this.textBuilder = new ASPxGridViewPrintTextBuilder(Grid);
			this.columnHelper = new GridViewExportColumnHelper(Grid, Exporter);
			this.printInfo = new GridViewPrintInfo();
			BeforeCreate();
		}
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public new ASPxGridViewExporter Exporter { get { return base.Exporter as ASPxGridViewExporter; } }
		public GridExportStyleHelper StyleHelper { get { return styleHelper; } }
		public BrickGraphics Graph { get { return graph; } }
		public GridViewPrintInfo PrintInfo { get { return printInfo; } set { printInfo = value; } }
		public new GridViewExportColumnHelper ColumnHelper { get { return columnHelper; } }
		public ASPxGridViewPrintTextBuilder TextBuilder { get { return textBuilder; } }
		protected GridViewPrinterDrawDetailGrid OnDrawDetailGrid { get { return onDrawDetailGrid; } }
		protected int Indent { get { return indent; } }
		protected int Level { get { return level; } }
		public bool ShowColumnHeaders { get { return Grid.Settings.ShowColumnHeaders; } }
		public bool ShowFooter { get { return Grid.Settings.ShowFooter; } }
		protected bool ShowTitle { get { return Grid.Settings.ShowTitlePanel && !string.IsNullOrEmpty(TitleText); } }
		protected string TitleText { get { return Grid.SettingsText.Title; } }
		protected bool PrintSelectCheckBox { get { return Exporter.PrintSelectCheckBox; } }
		protected int GraphBrickTop { get { return graphBrickTop; } set { graphBrickTop = value; } }
		protected int DataRowIndex { get { return dataRowIndex; } set { dataRowIndex = value; } }
		protected void BeforeCreate() {
			BeginExport();
			new GridViewPrintInfoCalculator().Calculate(this);
		}
		protected override bool RequireExpandAllGroups { get { return (!Exporter.PreserveGroupRowStates || Exporter.ExportSelectedRowsOnly) && Grid.GroupCount > 0; } }
		protected override bool NeedRebindGrid() {
			if(Grid.SettingsDetail.ExportMode != GridViewDetailExportMode.None && Grid.Templates.DetailRow != null && Grid.PageCount > 1)
				return true;
			return base.NeedRebindGrid();
		}
		protected override void FilterOutNonSelectedRows() {
			if(Level > 0 && Grid.Selection.Count < 1) return; 
			base.FilterOutNonSelectedRows();
		}
		public WebRowType GetRowType(int rowIndex) {
			return DataProxy.GetRowType(rowIndex);
		}
		protected virtual int GetRowLevel(int rowIndex) {
			return DataProxy.GetRowLevel(rowIndex);
		}
		public string GetHeaderText(GridViewColumn column) {
			return TextBuilder.GetHeaderCaption(column);
		}
		public string GetDataRowText(GridViewDataColumn column, int rowIndex) {
			return TextBuilder.GetRowDisplayText(column, rowIndex);
		}
		public string GetFooterText(GridViewColumn column) {
			if(column is GridViewDataColumn)
				return TextBuilder.GetFooterCaption(column, "\xd\xa");
			return string.Empty;
		}
		public string GetGroupFooterText(GridViewColumn column, int rowIndex) {
			if(column is GridViewDataColumn)
				return TextBuilder.GetGroupRowFooterText(column, rowIndex, "\xd\xa");
			return string.Empty;
		}
		public string GetGroupRowText(int rowIndex) {
			return TextBuilder.GetGroupRowText(Grid.GetSortedColumns()[DataProxy.GetRowLevel(rowIndex)], rowIndex);
		}
		protected string GetPreviewText(int rowIndex) {
			return TextBuilder.GetPreviewText(rowIndex);
		}
		protected CreateDisplayControlArgs GetDisplayControlArgs(GridViewDataColumn column, int rowIndex) {
			return TextBuilder.GetDisplayControlArgs(column, rowIndex);
		}
		protected int GridWidth { get { return PrintInfo.GridWidth; } }
		protected int GetColumnWidth(GridViewColumn column) {
			return PrintInfo.GetColumnWidth(column);
		}
		public int GetGroupRowWidth(int rowIndex) {
			return GridWidth - GetGroupLevelOffSetByRowIndex(rowIndex);
		}
		protected int FooterHeight { get { return PrintInfo.FooterHeight; } }
		protected int GetRowHeight(int rowIndex) {
			if(GetRowType(rowIndex) == WebRowType.Group)
				return PrintInfo.GetGroupRowHeight(rowIndex);
			return PrintInfo.GetDataRowHeight(rowIndex);
		}
		protected int GetGroupFooterHeight(int rowIndex) {
			return PrintInfo.GetGroupFooterRowHeight(rowIndex);
		}
		protected int GroupRowHorizontalOffset {
			get { return 10; } 
		}
		public int GetGroupLevelOffSet(int groupLevel) {
			return groupLevel * GroupRowHorizontalOffset;
		}
		protected int GetGroupLevelOffSetByRowIndex(int rowIndex) {
			return GetGroupLevelOffSet(GetRowLevel(rowIndex));
		}
		public List<int> GetGroupFooterVisibleIndexes(int visibleIndex) {
			if(Grid.Settings.ShowGroupFooter == GridViewGroupFooterMode.Hidden) return null;
			return DataProxy.GetGroupFooterVisibleIndexes(visibleIndex, Grid.Settings.ShowGroupFooter == GridViewGroupFooterMode.VisibleIfExpanded);
		}
		bool IsFirstColumn(GridViewColumn column) {
			if(ColumnHelper.Leafs.Count > 0 && ColumnHelper.Leafs[0].Column == column)
				return true;
			return false;
		}
		public void CreateDetailHeader() {
			SetupGraphBrickTop();
			DrawTitle();
			DrawHeaders();
		}
		public void CreateDetail() {
			SetupGraphBrickTop();
			DrawRows();
			DrawFooter();
		}
		void SetupGraphBrickTop() {
			GraphBrickTop = GraphBrickTop == 0 && !string.IsNullOrEmpty(Exporter.ReportHeader) ? 1 : 0;
		}
		protected virtual void DrawTitle() {
			if(!ShowTitle) return;
			BrickStyle defBrick = Graph.DefaultBrickStyle;
			string text = TitleText;
			BrickStyle brickStyle = StyleHelper.GetTitlePanelStyle(Graph, Grid.Styles.TitlePanel.HorizontalAlign);
			int offset = brickStyle.Padding.Left + brickStyle.Padding.Right + 2; 
			int height = StyleHelper.CalcTextSize(Graph, text, brickStyle, GridWidth - offset).Height;
			DrawTextBrick(new DrawTextBrickArgs(text, brickStyle, 0, GraphBrickTop, GridWidth, height, null, -1, GridViewRowType.Title));
			GraphBrickTop += height;
			Graph.DefaultBrickStyle = defBrick;
		}
		protected virtual void DrawHeaders() {
			if (!ShowColumnHeaders) return;
			BrickStyle defBrick = Graph.DefaultBrickStyle;
			foreach(List<GridViewColumnVisualTreeNode> layoutLevel in ColumnHelper.Layout) {
				foreach(GridViewColumnVisualTreeNode node in layoutLevel) {
					Rectangle rect = PrintInfo.GetHeaderRect(node.Column);
					DrawTextBrick(new DrawTextBrickArgs(GetHeaderText(node.Column), StyleHelper.GetHeaderStyle(Graph, GetColumnHorizontalAlign(node.Column)),
						rect.Left, GraphBrickTop + rect.Top, rect.Width, rect.Height, node.Column, -1, GridViewRowType.Header));
				}
			}
			Graph.DefaultBrickStyle = defBrick;
		}
		protected virtual void DrawRows() {
			BrickStyle defBrick = Graph.DefaultBrickStyle;
			foreach(int i in EnumerateExportedVisibleRows()) {
				if(GetRowType(i) == WebRowType.Group)
					DrawGroupRow(i);
				else
					DrawDataRow(i);
				GraphBrickTop += GetRowHeight(i);
				DrawDetailRow(i);
				DrawPreview(i);
				DrawGroupFooters(i);
				if(i > 0 && i % 10 == 0) {
					Graph.Modifier = BrickModifier.None;
					Graph.Modifier = BrickModifier.Detail;
					GraphBrickTop = 0;
				}
			}
			Graph.DefaultBrickStyle = defBrick;
		}
		protected virtual void DrawGroupRow(int rowIndex) {
			int left = GetGroupLevelOffSetByRowIndex(rowIndex);
			DrawTextBrick(new DrawTextBrickArgs(GetGroupRowText(rowIndex),
				StyleHelper.GetGroupRowStyle(Graph), left, GraphBrickTop, GetGroupRowWidth(rowIndex), GetRowHeight(rowIndex), null, rowIndex, GridViewRowType.Group));
		}
		protected virtual void DrawDataRow(int rowIndex) {
			int left = GetGroupLevelOffSet(GetRowLevel(rowIndex));
			foreach(GridViewColumnVisualTreeNode node in ColumnHelper.Leafs) {
				DrawCell(rowIndex, node.Column, ref left);
			}
			DataRowIndex++;
		}
		protected virtual void DrawGroupFooters(int rowIndex) {
			List<int> indexes = GetGroupFooterVisibleIndexes(rowIndex);
			if(indexes == null) return;
			for(int i = 0; i < indexes.Count; i++) {
				DrawGroupFooter(indexes[i]);
			}
		}
		protected virtual void DrawGroupFooter(int parentGroupVisibleIndex) {
			int left = GetGroupLevelOffSet(GetRowLevel(parentGroupVisibleIndex) + 1);
			foreach(GridViewColumnVisualTreeNode node in ColumnHelper.Leafs) {
				DrawGroupFooterCell(parentGroupVisibleIndex, node.Column, ref left);
			}
			GraphBrickTop += GetGroupFooterHeight(parentGroupVisibleIndex);
		}
		protected virtual void DrawFooter() {
			if (!ShowFooter || FooterHeight <= 0) return;
			BrickStyle defBrick = Graph.DefaultBrickStyle;
			int left = 0;
			foreach(GridViewColumnVisualTreeNode node in ColumnHelper.Leafs) {
				DrawFooterCell(node.Column, ref left);
			}
			GraphBrickTop += FooterHeight;
			Graph.DefaultBrickStyle = defBrick;
		}
		protected virtual void DrawFooterCell(GridViewColumn column, ref int left) {
			int width = GetColumnWidth(column);
			if(IsFirstColumn(column))
				width += GetGroupLevelOffSet(Grid.GroupCount);
			BrickStyle style = StyleHelper.GetFooterStyle(Graph, GetColumnHorizontalAlign(column));
			DrawTextBrickArgs args = new DrawTextBrickArgs(GetFooterText(column), 
				style, left, GraphBrickTop, width, FooterHeight, column, -1, GridViewRowType.Footer);
			args.XlsExportNativeFormat = DefaultBoolean.False;
			DrawTextBrick(args);
			left += width;
		}
		protected virtual void DrawCell(int rowIndex, GridViewColumn column, ref int left) {
			int width = GetColumnWidth(column);
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			if(dataColumn != null)
				DrawDataCell(rowIndex, dataColumn,  width, left);
			else if(column is GridViewBandColumn)
				DrawBandDataCell(rowIndex, column, width, left);
			else
				DrawSelectedCheckBox(rowIndex, column, width, left);
			left += width;
		}
		protected virtual void DrawDataCell(int rowIndex, GridViewDataColumn column, int width, int left) {
			if(IsImageColommn(column))
				DrawDataImageCell(rowIndex, column, width, left);
			else
				DrawDataTextCell(rowIndex, column, width, left);
		}
		protected virtual void DrawDataTextCell(int rowIndex, GridViewDataColumn dataColumn, int width, int left) {
			object textValue = null;
			string textValueFormatString = string.Empty;
			string url = string.Empty;
			if(dataColumn.ExportPropertiesEdit != null) {
				CreateDisplayControlArgs args = GetDisplayControlArgs(dataColumn, rowIndex);
				textValue = dataColumn.ExportPropertiesEdit.GetExportValue(args);
				textValueFormatString = dataColumn.ExportPropertiesEdit.DisplayFormatString;
				url = dataColumn.ExportPropertiesEdit.GetExportNavigateUrl(args);
			}
			BrickStyle style = StyleHelper.GetCellStyle(Graph, dataColumn, rowIndex, IsAltRow(), GetColumnHorizontalAlign(dataColumn), !String.IsNullOrEmpty(url));
			DrawTextBrickArgs drawArgs = new DrawTextBrickArgs(GetDataRowText(dataColumn, rowIndex),
				style, left, GraphBrickTop, width, GetRowHeight(rowIndex), dataColumn, rowIndex, GridViewRowType.Data);
			drawArgs.TextValue = textValue;
			drawArgs.TextValueFormatString = textValueFormatString;
			drawArgs.Url = url;
			DrawTextBrick(drawArgs);
		}
		protected virtual void DrawDataImageCell(int rowIndex, GridViewDataColumn column, int width, int left) {
			CreateDisplayControlArgs args = GetDisplayControlArgs(column, rowIndex);
			string url = column.ExportPropertiesEdit.GetExportNavigateUrl(args);
			BrickStyle style = StyleHelper.GetCellStyle(Graph, column, rowIndex, IsAltRow(), GetColumnHorizontalAlign(column), !String.IsNullOrEmpty(url), true);
			byte[] imageValue = column.ExportPropertiesEdit.GetExportValue(args) as byte[];
			ASPxGridViewExportRenderingEventArgs e = Exporter.RaiseRenderBrick(rowIndex, column, GridViewRowType.Data, DataProxy, style, string.Empty, string.Empty, string.Empty, url, imageValue);
			if(e != null) {
				url = e.Url;
				style = e.BrickStyle;
				imageValue = e.ImageValue;
				if(imageValue == null && (!string.IsNullOrEmpty(e.Text) || e.TextValue != null)) {
					DrawTextBrick(Graph, new DrawTextBrickArgs() {
						Text = e.Text,
						TextValueFormatString = e.TextValueFormatString,
						TextValue = e.TextValue,
						Style = e.BrickStyle,
						Left = left,
						Top = GraphBrickTop,
						Width = width,
						Height = GetRowHeight(rowIndex),
						Column = column,
						VisibleIndex = rowIndex,
						RowType = GridViewRowType.Data
					});
					return;
				}
			}
			DrawImageBrick(style, left, GraphBrickTop, width, GetRowHeight(rowIndex), GetImageExportSettings(column).SizeMode, url, imageValue);
		}
		protected virtual void DrawBandDataCell(int rowIndex, GridViewColumn column, int width, int left) {
			BrickStyle style = StyleHelper.GetCellStyle(Graph, (IWebGridDataColumn)column, rowIndex, IsAltRow(), HorizontalAlign.NotSet, false);
			DrawTextBrick(new DrawTextBrickArgs(string.Empty, style, left, GraphBrickTop, width, GetRowHeight(rowIndex),
				column, rowIndex, GridViewRowType.Data));
		}
		protected virtual void DrawSelectedCheckBox(int rowIndex, GridViewColumn column, int width, int left) {
			BrickStyle brickStyle = StyleHelper.GetCellStyle(Graph, (IWebGridDataColumn)column, rowIndex, IsAltRow(), HorizontalAlign.Center, false);
			bool selected = DataProxy.Selection.IsRowSelected(rowIndex);
			string displayText = selected ? CheckedDisplayText : UncheckedDisplayText;
			DrawCheckBoxBrick(Graph, brickStyle, selected, displayText, left + Indent, GraphBrickTop, width, GetRowHeight(rowIndex));
		}
		bool IsAltRow() {
			return DataRowIndex % 2 == 1 && UseAltRowStyle();
		}
		bool UseAltRowStyle() {
			switch(StyleHelper.AlternatingRowEnabled()) {
				case DefaultBoolean.False:
					return false;
				case DefaultBoolean.True:
					return true;
			}
			return Grid.Styles.AlternatingRow.Enabled == DefaultBoolean.True || !Grid.Styles.AlternatingRow.IsEmpty;
		}
		protected virtual void DrawGroupFooterCell(int rowIndex, GridViewColumn column, ref int left) {
			int width = GetColumnWidth(column);
			if(IsFirstColumn(column))
				width += GetGroupLevelOffSet(Grid.GroupCount - GetRowLevel(rowIndex) - 1);
			BrickStyle style = StyleHelper.GetGroupFooterStyle(Graph, GetColumnHorizontalAlign(column));
			DrawTextBrickArgs args = new DrawTextBrickArgs(GetGroupFooterText(column, rowIndex), style, left, GraphBrickTop, width, GetGroupFooterHeight(rowIndex), column, rowIndex, GridViewRowType.GroupFooter);
			args.XlsExportNativeFormat = DefaultBoolean.False;
			DrawTextBrick(args);
			left += width;
		}
		protected virtual void DrawPreview(int rowIndex) {
			if(!Grid.Settings.ShowPreview) return;
			if(DataProxy.GetRowType(rowIndex) != WebRowType.Data) return;
			string text = GetPreviewText(rowIndex);
			if(string.IsNullOrEmpty(text)) return;
			int left = GetGroupLevelOffSetByRowIndex(rowIndex);
			int width = GridWidth - left;
			BrickStyle brickStyle = StyleHelper.GetPreviewRowStyle(Graph);
			int offset = brickStyle.Padding.Left + brickStyle.Padding.Right + 2; 
			int height = StyleHelper.CalcTextSize(Graph, text, brickStyle, width - offset).Height;
			DrawTextBrick(new DrawTextBrickArgs(text, StyleHelper.GetPreviewRowStyle(Graph), left, GraphBrickTop, width, height, null, rowIndex, GridViewRowType.Preview));
			GraphBrickTop += height;
		}
		protected virtual void DrawDetailRow(int rowIndex) {
			int indent = GetGroupLevelOffSetByRowIndex(rowIndex) + (1 + Level) * Exporter.DetailHorizontalOffset;
			if(Grid.SettingsDetail.ExportMode == GridViewDetailExportMode.None
				|| Grid.Templates.DetailRow == null
				|| Grid.Page == null
				|| DataProxy.GetRowType(rowIndex) != WebRowType.Data) return;
			if(Grid.DetailRows.IsVisible(rowIndex)) {
				Control container = Grid.FindControl(GridViewRenderHelper.DetailRowID + rowIndex.ToString());
				DrawDetailRowCore(container, indent);
			} else if(Grid.SettingsDetail.ExportMode == GridViewDetailExportMode.All) {
				Control parent = Grid.Parent;
				using(GridViewDetailRowTemplateContainer container = new GridViewDetailRowTemplateContainer(Grid, DataProxy.GetRowForTemplate(rowIndex), rowIndex)) {
					container.ID = "dxPrinterLink" + Grid.ID;
					Grid.Templates.DetailRow.InstantiateIn(container);
					parent.Controls.Add(container);
					DrawDetailRowCore(container, indent);
					parent.Controls.Remove(container);
				}
			}
		}
		void DrawDetailRowCore(Control container, int indent) {
			if(container == null) return;
			List<ASPxGridView> grids = new List<ASPxGridView>();
			FindDetailGrids(container, grids);
			grids.Sort(DoDetailGridCompare);
			foreach(ASPxGridView grid in grids) {
				grid.DataBind();
				if(Exporter.ExportEmptyDetailGrid || grid.VisibleRowCount > 0) {
					DrawDetailGrid(grid, indent);
				}
			}
		}
		void FindDetailGrids(Control parent, List<ASPxGridView> list) {
			if(parent is ASPxGridView) {
				ASPxGridView grid = parent as ASPxGridView;
				if(grid.SettingsDetail.ExportIndex > -1) {
					list.Add(parent as ASPxGridView);
				}
				return;
			}
			for(int i = 0; i < parent.Controls.Count; i++) {
				FindDetailGrids(parent.Controls[i], list);
			}
		}
		int DoDetailGridCompare(ASPxGridView g1, ASPxGridView g2) {
			return Comparer<int>.Default.Compare(g1.SettingsDetail.ExportIndex, g2.SettingsDetail.ExportIndex);
		}
		protected virtual void DrawDetailGrid(ASPxGridView detailGrid, int indent) {
			if(OnDrawDetailGrid != null) {
				GraphBrickTop = Exporter.DetailVerticalOffset;
				OnDrawDetailGrid(detailGrid, indent);
			}
		}
		protected void DrawImageBrick(BrickStyle style, int left, int top, int width, int height, XtraPrinting.ImageSizeMode sizeMode, string url, byte[] imageValue) {
			BrickStyle temp = graph.DefaultBrickStyle;
			graph.DefaultBrickStyle = style;
			ImageBrick brick = new ImageBrick();
			brick.SizeMode = sizeMode;
			brick.Url = url;
			brick.Image = ByteImageConverter.FromByteArray(imageValue);
			brick.DisposeImage = true;
			left += Indent;
			DrawBrickCore(graph, brick, left, top, width, height);
			graph.DefaultBrickStyle = temp;
		}
		protected void DrawTextBrick(DrawTextBrickArgs args) {
			if(object.Equals(args.TextValue, DateTime.MinValue))
				args.TextValue = string.Empty; 
			ASPxGridViewExportRenderingEventArgs e = Exporter.RaiseRenderBrick(args.VisibleIndex, args.Column,
				args.RowType, DataProxy, args.Style, args.Text, args.TextValue, args.TextValueFormatString, args.Url, null);
			if(e != null) {				
				args.Text = e.Text;
				args.TextValue = e.TextValue;
				args.TextValueFormatString = e.TextValueFormatString;
				args.Url = e.Url;
				args.Style = e.BrickStyle;
			}
			args.Left += Indent;
			DrawTextBrick(Graph, args);
		}
		static void DrawTextBrick(BrickGraphics graph, DrawTextBrickArgs args) {
			BrickStyle temp = graph.DefaultBrickStyle;
			graph.DefaultBrickStyle = args.Style;
			TextBrick brick = new TextBrick();
			brick.Url = args.Url;
			brick.Text = args.Text;
			brick.TextValue = args.TextValue;
			brick.TextValueFormatString = args.TextValueFormatString;
			brick.XlsExportNativeFormat = args.XlsExportNativeFormat;
			DrawBrickCore(graph, brick, args.Left, args.Top, args.Width, args.Height);
			graph.DefaultBrickStyle = temp;
		}
		static void DrawCheckBoxBrick(BrickGraphics graph, BrickStyle brickStyle, bool value, string displayText, int left, int top, int width, int height) {
			graph.DefaultBrickStyle = brickStyle;
			XECheckBoxBrick brick = new XECheckBoxBrick();
			brick.Checked = value;
			brick.CheckText = displayText;
			DrawBrickCore(graph, brick, left, top, width, height);
		}
		static void DrawBrickCore(BrickGraphics graph, Brick brick, int left, int top, int width, int height) {
			RectangleF rect = new RectangleF(left, top, width, height);
			graph.DrawBrick(brick, rect);
		}
		static internal IImageExportSettings GetImageExportSettings(GridViewDataColumn column) {
			return column != null ? column.ExportPropertiesEdit as IImageExportSettings : null;
		}
		static internal bool IsImageColommn(GridViewColumn column) {
			return column as GridViewDataBinaryImageColumn != null || column as GridViewDataImageColumn != null || GetImageExportSettings(column as GridViewDataColumn) != null;
		}
		HorizontalAlign GetColumnHorizontalAlign(GridViewColumn column) {
			return TextBuilder.GetColumnDisplayControlDefaultAlignment(column);
		}
	}
}
