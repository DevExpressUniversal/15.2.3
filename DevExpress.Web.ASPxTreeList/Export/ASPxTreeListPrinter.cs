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
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxTreeList.Internal;
using DevExpress.XtraExport;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Web.ASPxTreeList.Export {
	public class ASPxTreeListLink : LinkBase {
		ASPxTreeList treeList;
		ASPxTreeListExporter exporter;
		TreeListExportHelper exportHelper;
		TreeListExportStyles styles;
		TreeListPrintInfo printInfo;
		TreeListRowsCreationFlags savedRowsCreationFlags;
		public ASPxTreeListLink(ASPxTreeListExporter exporter) {
			this.printInfo = new TreeListPrintInfo();
			this.exporter = exporter; 
			this.treeList = exporter.TreeList;
			this.exportHelper = new TreeListExportHelper(treeList);
			this.styles = new TreeListExportStyles(null);
			this.styles.CopyFrom(new TreeListExportSystemStyles(null));
			this.styles.CopyFrom(exporter.Styles);
			this.styles.MergeWithDefault();
		}
		public TreeListExportStyles Styles { get { return styles; } }
		public ASPxTreeList TreeList { get { return treeList; } }
		public ASPxTreeListExporter Exporter { get { return exporter; } }
		protected TreeListExportHelper ExportHelper { get { return exportHelper; } }
		protected internal TreeListRenderHelper RenderHelper { get { return ExportHelper.RenderHelper; } }
		protected internal TreeListPrintInfo PrintInfo { get { return printInfo; } }
		bool AutoWidth { get { return Exporter.Settings.AutoWidth; } }
		protected override void BeforeCreate() {
			base.BeforeCreate();
			SetRowsCreationFlags();
			CalcPrintInfo();			
		}
		protected override void AfterCreate() {
			base.AfterCreate();
			RestoreRowsCreationFlags();
		}
		void SetRowsCreationFlags() {
			this.savedRowsCreationFlags = ExportHelper.RowsCreationFlags;
			TreeListRowsCreationFlags flags = TreeListRowsCreationFlags.Empty;
			if(Exporter.Settings.ExpandAllNodes) flags |= TreeListRowsCreationFlags.ExpandAll;
			if(Exporter.Settings.ExportAllPages) flags |= TreeListRowsCreationFlags.IgnorePaging;
			ExportHelper.RowsCreationFlags = flags;
		}
		void RestoreRowsCreationFlags() {
			ExportHelper.RowsCreationFlags = this.savedRowsCreationFlags;
		}
		protected override void ApplyPageSettings() {
			base.ApplyPageSettings();
			PageSettings pageSettings = Exporter.Settings.PageSettings;
			ps.PageSettings.Assign(pageSettings.Margins.ToPrintingMargins(), pageSettings.PaperKind, pageSettings.PaperName, pageSettings.Landscape);
		}
		protected virtual void CalcPrintInfo() {
			this.printInfo = (new TreeListPrintInfoCalculator(RenderHelper, ps.Graph, Styles, !AutoWidth)).Calculate();
			if(AutoWidth)
				this.printInfo = (new TreeListAutoWidthPrintInfoCalculator(PrintInfo, RenderHelper, ps.Graph, Styles)).Calculate();
		}
		protected override void CreateDetailHeader(BrickGraphics graph) {
			DrawColumnHeaders(graph);
		}
		protected override void CreateDetail(BrickGraphics graph) {
			BrickStyle defBrickStyle = graph.DefaultBrickStyle;
			TreeListBuilderHelper.Build(new TreeListDetailContentPrinter(this, graph));
			graph.DefaultBrickStyle = defBrickStyle;
		}
		protected internal void CreateArea(string areaName, IBrickGraphics graph) {
			if(areaName == SR.DetailHeader)
				CreateDetailHeader((BrickGraphics)graph);
			if(areaName == SR.Detail)
				CreateDetail((BrickGraphics)graph);
		}
		protected internal void InitializePrintableLink() {
			BeforeCreate();
		}
		protected internal void FinalizePrintableLink() {
			AfterCreate();
		}
		protected void DrawColumnHeaders(BrickGraphics graph) {
			if(!RenderHelper.IsHeaderRowVisible) return;
			BrickStyle defBrickStyle = graph.DefaultBrickStyle;
			int left = 0;
			foreach(KeyValuePair<TreeListDataColumn, int> pair in PrintInfo.ColumnWidths) {
				int width = pair.Value;
				TreeListDataColumn column = pair.Key;
				RectangleF rect = new RectangleF(left, 0, width, PrintInfo.ColumnPanelHeight);
				DrawTextBrick(graph, TreeListExportHelper.GetColumnCaption(column), Styles.Header, null, column, rect, TreeListRowKind.Header);
				left += width;
			}
			graph.DefaultBrickStyle = defBrickStyle;
		}
		public void DrawTextBrick(BrickGraphics graph, string text, TreeListExportAppearance printAppearance, TreeListRowInfo ri, TreeListDataColumn column, RectangleF rect, TreeListRowKind rowKind) {
			DrawTextBrick(graph, text, null, String.Empty, printAppearance, ri, column, rect, rowKind);
		}
		public void DrawTextBrick(BrickGraphics graph, string text, object value, string url, TreeListExportAppearance printAppearance, TreeListRowInfo ri, TreeListDataColumn column, 
			RectangleF rect, TreeListRowKind rowKind) {
			BrickStyle style = TreeListPrintUtils.CreateBrickStyleFromAppearance(printAppearance, graph);
			HorzAlignment horzAlign = TreeListPrintUtils.GetBrickHorzAlignment(printAppearance, column == null ? HorizontalAlign.Left : RenderHelper.GetColumnDefaultDisplayControlAlign(column));
			VertAlignment vertAlign = TreeListPrintUtils.GetBrickVertAlignment(printAppearance);
			style.SetAlignment(horzAlign, vertAlign);
			ASPxTreeListExportRenderBrickEventArgs e = RaiseRenderEvent(ri, text, value, url, column, style, rowKind, null);
			DrawTextBrickCore(graph, rect, e);
		}
		ASPxTreeListExportRenderBrickEventArgs RaiseRenderEvent(TreeListRowInfo ri, string text, object value, string url, TreeListDataColumn column, BrickStyle style, TreeListRowKind rowKind, byte[] imageValue) {
			string nodeKey = ri == null ? string.Empty : ri.NodeKey;
			ASPxTreeListExportRenderBrickEventArgs e = new ASPxTreeListExportRenderBrickEventArgs(nodeKey, text, value, url, column, style, rowKind, imageValue);
			Exporter.RaiseRenderBrick(e);
			return e;
		}
		void DrawTextBrickCore(BrickGraphics graph, RectangleF rect, ASPxTreeListExportRenderBrickEventArgs e) {
			graph.DefaultBrickStyle = e.BrickStyle;
			TextBrick brick = new TextBrick();
			brick.Text = e.Text;
			brick.TextValue = e.TextValue;
			brick.TextValueFormatString = e.TextValueFormatString;
			brick.Url = e.Url;			
			graph.DrawBrick(brick, rect);
		}
		public void DrawIndentImageBrick(BrickGraphics graph, TreeListExportAppearance printAppearance, System.Drawing.Image image, RectangleF rect, bool needTopPadding) {
			graph.DefaultBrickStyle = TreeListPrintUtils.CreateBrickStyleFromAppearance(printAppearance, graph);
			ImageBrick brick = new ImageBrick();
			brick.Sides = BorderSide.None;
			brick.BorderWidth = 0;
			brick.Padding = new PaddingInfo(1, 1, needTopPadding ? 1 : 0, 0);
			brick.Image = image;
			graph.DrawBrick(brick, rect);
		}
		public void DrawImageBrick(BrickGraphics graph, TreeListExportAppearance printAppearance, byte[] imageValue, string url, TreeListRowInfo rowInfo,
			TreeListDataColumn column, DevExpress.XtraPrinting.ImageSizeMode sizeMode, RectangleF rect, TreeListRowKind rowKind) {
			BrickStyle style = TreeListPrintUtils.CreateBrickStyleFromAppearance(printAppearance, graph);
			ASPxTreeListExportRenderBrickEventArgs e = RaiseRenderEvent(rowInfo, string.Empty, null, url, column, style, rowKind, imageValue);
			DrawImageBrickCore(graph, rect, sizeMode, e);
		}
		void DrawImageBrickCore(BrickGraphics graph, RectangleF rect, DevExpress.XtraPrinting.ImageSizeMode sizeMode, ASPxTreeListExportRenderBrickEventArgs e) {
			ImageBrick brick = new ImageBrick();
			brick.Style = e.BrickStyle;
			brick.SizeMode = sizeMode;
			brick.Image = ByteImageConverter.FromByteArray(e.ImageValue);
			graph.DrawBrick(brick, rect);
		}
	}
	public class ColumnWidthsType : Dictionary<TreeListDataColumn, int> {
		public List<TreeListDataColumn> GetColumns() {
			return new List<TreeListDataColumn>(Keys);
		}
	}
	public class RowHeightsType : Dictionary<int, int> { } 
	public class TreeListPrintInfo {
		public static readonly int MinColumnWidth = 20;
		public static readonly int MaxColumnWidth = 300;
		public static readonly int LevelWidth = 20;
		public static readonly int ButtonSize = 8;
		protected ColumnWidthsType fColumnWidths;
		RowHeightsType rowHeights;
		RowHeightsType groupFooterHeights;
		int columnPanelHeight;
		int footerHeight;
		int totalColumnWidth;
		public TreeListPrintInfo() {
			this.fColumnWidths = new ColumnWidthsType();
			this.rowHeights = new RowHeightsType();
			this.groupFooterHeights = new RowHeightsType();
			this.ResetCore();
		}
		public void Reset() {
			this.ResetCore();
			this.rowHeights.Clear();
			this.groupFooterHeights.Clear();
			this.fColumnWidths.Clear();
		}
		void ResetCore() {
			this.columnPanelHeight = this.footerHeight = this.totalColumnWidth = 0;
		}
		public int ColumnPanelHeight { get { return columnPanelHeight; } }
		public int FooterHeight { get { return footerHeight; } }
		public ColumnWidthsType ColumnWidths { get { return fColumnWidths; } }
		public RowHeightsType RowHeights { get { return rowHeights; } }
		public RowHeightsType GroupFooterHeights { get { return groupFooterHeights; } }
		public int TotalColumnWidth { get { return totalColumnWidth; } }
		public virtual int UpdateColumnWidth(TreeListDataColumn column, int width) {
			if(ColumnWidths.ContainsKey(column))
				ColumnWidths[column] = Math.Max(ColumnWidths[column], width);
			else
				ColumnWidths.Add(column, Math.Max(GetMinColumnWidth(column), width));
			return ColumnWidths[column];
		}
		int GetMinColumnWidth(TreeListDataColumn column) {
			return TreeListPrintUtils.IsExportWidthAssigned(column) ? column.ExportWidth : MinColumnWidth;
		}
		void UpdateHeightCore(RowHeightsType source, int index, int height) {
			if(source.ContainsKey(index))
				source[index] = Math.Max(source[index], height);
			else
				source.Add(index, height);
		}
		public int UpdateRowHeight(int index, int height) {
			UpdateHeightCore(RowHeights, index, height);
			return RowHeights[index];
		}
		public int UpdateGroupFooterHeight(int index, int height) {
			UpdateHeightCore(GroupFooterHeights, index, height);
			return GroupFooterHeights[index];
		}
		public int UpdateColumnPanelHeight(int height) {
			columnPanelHeight = Math.Max(ColumnPanelHeight, height);
			return ColumnPanelHeight;
		}
		public int UpdateFooterHeight(int height) {
			footerHeight = Math.Max(FooterHeight, height);
			return FooterHeight;
		}
		public virtual void UpdateTotalColumnWidth() {
			totalColumnWidth = 0;
			foreach(int width in ColumnWidths.Values) 
				totalColumnWidth += width;
		}
	}
	public class TreeListPrintInfoCalculator : ITreeListBuilder {
		BrickGraphics graph;
		TreeListPrintInfo printInfo;
		TreeListExportStyles styles;
		TreeListRenderHelper renderHelper;
		List<TreeListDataColumn> columns;
		bool calcHeight;
		public TreeListPrintInfoCalculator(TreeListRenderHelper renderHelper, BrickGraphics graph, TreeListExportStyles styles, bool calcHeight) {
			this.calcHeight = calcHeight;
			this.graph = graph;
			this.styles = styles;
			this.renderHelper = renderHelper;
			this.printInfo = CreatePrintInfo();
			this.columns = CreateDataColumns();
		}
		public TreeListRenderHelper RenderHelper { get { return renderHelper; } }
		protected TreeListExportStyles Styles { get { return styles; } }
		protected BrickGraphics Graph { get { return graph; } }
		protected TreeListPrintInfo PrintInfo { get { return printInfo; } }
		protected bool IsColumnHeaderVisible { get { return RenderHelper.IsHeaderRowVisible; } }
		protected bool IsFooterVisible { get { return RenderHelper.IsTotalFooterVisible; } }
		protected List<TreeListDataColumn> DataColumns { get { return columns; } }
		bool CanCalcHeight { get { return calcHeight; } }
		protected TreeListDataColumn FirstDataColumn {
			get {
				if(DataColumns.Count < 1) return null;
				return DataColumns[0];
			}
		}
		protected virtual TreeListPrintInfo CreatePrintInfo() {
			return new TreeListPrintInfo();
		} 
		protected List<TreeListDataColumn> CreateDataColumns() {
			List<TreeListDataColumn> columns = new List<TreeListDataColumn>();
			foreach(TreeListColumn column in RenderHelper.VisibleColumns) {
				TreeListDataColumn dataColumn = column as TreeListDataColumn;
				if(dataColumn != null)
					columns.Add(dataColumn);
			}
			return columns;
		}
		void ITreeListBuilder.CreateHeader() {
			foreach(TreeListDataColumn column in DataColumns) {
				Size size = TreeListPrintUtils.CalcTextSize(TreeListExportHelper.GetColumnCaption(column), Styles.Header, Graph, GetMaxColumnWidth(column));
				if(CanCalcHeight) 
					PrintInfo.UpdateColumnPanelHeight(Math.Max(GetDefaultHeight(Styles.Header, column), size.Height));
				PrintInfo.UpdateColumnWidth(column, size.Width);
			}
		}
		void ITreeListBuilder.CreateDataRow(int rowIndex) {
			TreeListRowInfo ri = RenderHelper.Rows[rowIndex];
			foreach(TreeListDataColumn column in DataColumns) {
				string displayText = column.PropertiesEdit.GetExportDisplayText(RenderHelper.GetEditorDisplayControlArgs(ri, column));
				Size size = GetSizeFromDataCell(column, ri);
				if(CanCalcHeight) 
					PrintInfo.UpdateRowHeight(rowIndex, Math.Max(GetDefaultHeight(Styles.Cell, column), size.Height));
				PrintInfo.UpdateColumnWidth(column, size.Width);
			}
		}
		Size GetSizeFromDataCell(TreeListDataColumn column, TreeListRowInfo rowInfo) {
			Size size;
			IImageExportSettings imageExportSettings = TreeListPrintUtils.GetImageExportSettings(column);
			if(imageExportSettings != null) {
				var useColumnWidth = column.ExportWidth != 0;
				var width = useColumnWidth ? column.ExportWidth : imageExportSettings.Width;
				size = TreeListPrintUtils.CalcImageSize(width, imageExportSettings.Height, Styles.Image, graph, useColumnWidth);
			} else {
				string displayText = column.PropertiesEdit.GetExportDisplayText(RenderHelper.GetEditorDisplayControlArgs(rowInfo, column));
				size = TreeListPrintUtils.CalcTextSize(displayText, Styles.Cell, Graph, GetMaxColumnWidth(column));
			}
			return size;
		}
		void ITreeListBuilder.CreateInlineEditRow(int rowIndex) {
			(this as ITreeListBuilder).CreateDataRow(rowIndex);
		}
		void ITreeListBuilder.CreateGroupFooter(int rowIndex) {
			CreateFooterCore(rowIndex, true);
		}
		void ITreeListBuilder.CreateFooter(int rowIndex) {
			CreateFooterCore(rowIndex, false);
		}
		void CreateFooterCore(int rowIndex, bool groupFooter)  {
			foreach(TreeListDataColumn column in DataColumns) {
				TreeListRowInfo ri = RenderHelper.GetRowByIndex(rowIndex);
				string text = RenderHelper.GetFooterText(ri, column, "\n", true);
				Size size = TreeListPrintUtils.CalcTextSize(text, Styles.GroupFooter, Graph, GetMaxColumnWidth(column));
				PrintInfo.UpdateColumnWidth(column, size.Width); 
				if(CanCalcHeight) {
					if(groupFooter)
						PrintInfo.UpdateGroupFooterHeight(rowIndex, Math.Max(GetDefaultHeight(Styles.GroupFooter, column), size.Height));
					else
						PrintInfo.UpdateFooterHeight(size.Height);
				}
			}			
		}
		public TreeListPrintInfo Calculate() {
			CalculateCore();
			FinalizeCalculations();
			return PrintInfo;
		}
		protected virtual void CalculateCore() {
			TreeListBuilderHelper.Build(this);
		}
		protected virtual void FinalizeCalculations() {
			ApplyIndentsWidth();
			PrintInfo.UpdateTotalColumnWidth();
		}
		protected virtual int GetMaxColumnWidth(TreeListDataColumn column) {
			return TreeListPrintUtils.IsExportWidthAssigned(column) ? column.ExportWidth : TreeListPrintInfo.MaxColumnWidth;
		}
		protected void ApplyIndentsWidth() {
			TreeListDataColumn firstColumn = FirstDataColumn;
			if(firstColumn == null) return;
			int width = PrintInfo.ColumnWidths[firstColumn];
			width += RenderHelper.MaxVisibleIndentCount * TreeListPrintInfo.LevelWidth;
			PrintInfo.UpdateColumnWidth(firstColumn, width);
		}
		protected int GetDefaultHeight(TreeListExportAppearance apperance, TreeListDataColumn column) {
			return TreeListPrintUtils.GetDefaultHeight(Graph, apperance, GetMaxColumnWidth(column));
		}
		void ITreeListBuilder.CreatePreview(int rowIndex) { } 
		void ITreeListBuilder.CreateErrorRow(int rowIndex) { }
		void ITreeListBuilder.CreateEditFormRow(int rowIndex, bool isAuxRow) { }
	}
	public class TreeListAutoWidthPrintInfo : TreeListPrintInfo {
		protected internal TreeListAutoWidthPrintInfo() : base() { }
		protected internal void AssignColumnWidths(ColumnWidthsType sourceColumnWidths) {
			this.fColumnWidths = sourceColumnWidths;
		}
		public override int UpdateColumnWidth(TreeListDataColumn column, int width) {
			return ColumnWidths[column];
		}
	}
	public class TreeListAutoWidthPrintInfoCalculator : TreeListPrintInfoCalculator {
		TreeListPrintInfo sourcePrintInfo;
		public TreeListAutoWidthPrintInfoCalculator(TreeListPrintInfo sourcePrintInfo, TreeListRenderHelper renderHelper, BrickGraphics graph, TreeListExportStyles styles) 
			: base(renderHelper, graph, styles, true) {
			this.sourcePrintInfo = sourcePrintInfo;
		}
		protected override TreeListPrintInfo CreatePrintInfo() {
			return new TreeListAutoWidthPrintInfo();
		}
		protected TreeListAutoWidthPrintInfo AutoWidthPrintInfo { get { return PrintInfo as TreeListAutoWidthPrintInfo; } }
		protected TreeListPrintInfo SourcePrintInfo { get { return sourcePrintInfo; } }
		protected override int GetMaxColumnWidth(TreeListDataColumn column) {
			return PrintInfo.ColumnWidths[column];
		}
		protected override void FinalizeCalculations() {
			PrintInfo.UpdateTotalColumnWidth();
		}
		protected override void CalculateCore() {
			CalcAutoWidths();
			base.CalculateCore();
		}
		protected void CalcAutoWidths() {
			AutoWidthPrintInfo.AssignColumnWidths(SourcePrintInfo.ColumnWidths);
			RecalcColumnWidths((int)Graph.ClientPageSize.Width - 2);
		}
		protected void RecalcColumnWidths(int pageWidth) {
			SourcePrintInfo.UpdateTotalColumnWidth();
			int totalColumnWidth = SourcePrintInfo.TotalColumnWidth;
			if(pageWidth - totalColumnWidth == 0)
				return;
			double remainder = 0;
			double colFactor = pageWidth * 1.0 / totalColumnWidth;
			foreach(TreeListDataColumn col in DataColumns) {
				double width = AutoWidthPrintInfo.ColumnWidths[col] * colFactor;
				int resultWidth = (int)width;
				remainder += width - resultWidth;
				if(remainder > 1) {
					resultWidth++;
					remainder--;
				}
				if(resultWidth < TreeListPrintInfo.MinColumnWidth) {
					resultWidth = TreeListPrintInfo.MinColumnWidth;
					pageWidth -= resultWidth;
					totalColumnWidth -= resultWidth;
					colFactor = pageWidth * 1.0 / totalColumnWidth;
				}
				AutoWidthPrintInfo.ColumnWidths[col] = resultWidth;
			}
		}
	}
	public class TreeListDetailContentPrinter : ITreeListBuilder {
		ASPxTreeListLink link;
		BrickGraphics graph;
		int topLocation;
		public TreeListDetailContentPrinter(ASPxTreeListLink link, BrickGraphics graph) {
			this.link = link;
			this.graph = graph;
			this.topLocation = 0;
		}
		protected ASPxTreeListLink Link { get { return link; } }
		protected BrickGraphics Graph { get { return graph; } }
		protected TreeListPrintInfo PrintInfo { get { return Link.PrintInfo; } }
		protected TreeListExportStyles Styles { get { return Link.Styles;} }
		public TreeListRenderHelper RenderHelper { get { return Link.RenderHelper; } }
		protected TreeListDataColumn FirstDataColumn {
			get {
				List<TreeListDataColumn> columns = PrintInfo.ColumnWidths.GetColumns();
				if(columns.Count > 0) 
					return columns[0];
				return null;
			}
		}
		public void CreateDataRow(int rowIndex) {
			TreeListRowInfo ri = RenderHelper.Rows[rowIndex];
			int height = PrintInfo.RowHeights[rowIndex];
			BrickStyle defBrickStyle = graph.DefaultBrickStyle;
			DrawIndents(graph, height, ri, false);
			DrawCells(graph, height, ri);
			graph.DefaultBrickStyle = defBrickStyle;
			this.topLocation += height;
		}
		protected void DrawCells(BrickGraphics graph, int height, TreeListRowInfo ri) {
			int left = 0;
			foreach(KeyValuePair<TreeListDataColumn, int> pair in PrintInfo.ColumnWidths) {
				TreeListDataColumn column = pair.Key;
				int width = pair.Value;
				if(column == FirstDataColumn) {
					left = ri.Indents.Count * TreeListPrintInfo.LevelWidth;
					RectangleF rect = new RectangleF(left, topLocation, width - left, height);
					DrawCell(graph, column, rect, ri);
					left = width;
					continue;
				}
				DrawCell(graph, column, new RectangleF(left, topLocation, width, height), ri);
				left += width;
			}
		}
		protected void DrawIndents(BrickGraphics graph, int height, TreeListRowInfo ri, bool isAux) {
			List<TreeListRowIndentType> indents = ri.Indents;
			int left = 0;
			int width = TreeListPrintInfo.LevelWidth;
			for(int i = 0; i < indents.Count; i++) {
				TreeListRowIndentType indent = indents[i];
				if(isAux)
					indent = RenderHelper.FilterAuxRowIndent(indent);
				if(indent == TreeListRowIndentType.None) {
					left += TreeListPrintInfo.LevelWidth;
					continue;
				}
				bool drawButton = false;
				if(Link.Exporter.Settings.ShowTreeButtons)
					drawButton = !isAux && ri.HasButton && i == indents.Count - 1;
				if(drawButton || Link.Exporter.Settings.ShowTreeLines)
					DrawIndent(graph, left, new Size(width, height), indent, ri, drawButton);
				left += width;
			}
		}
		protected void DrawIndent(BrickGraphics graph, int left, Size size, TreeListRowIndentType indent, TreeListRowInfo ri, bool drawButton) {
			System.Drawing.Bitmap image = new System.Drawing.Bitmap(size.Width, size.Height);
			using(Graphics g = Graphics.FromImage(image)) {
				Color indentColor = Styles.Indent.BorderColor;
				using(SolidBrush brush = new SolidBrush(indentColor)) { 
					if(Link.Exporter.Settings.ShowTreeLines)
						IndentsPainter.DrawIndent(g, indent, brush, new Rectangle(0, 0, size.Width, size.Height));
					if(drawButton) {
						using(Pen pen = new Pen(indentColor)) {
							int buttonSize = TreeListPrintInfo.ButtonSize; 
							RectangleF buttonRect = new RectangleF((size.Width - buttonSize) / 2, (size.Height - buttonSize) / 2, buttonSize, buttonSize);
							IndentsPainter.DrawButton(g, brush, pen, buttonRect, ri.Expanded || Link.Exporter.Settings.ExpandAllNodes);   
						}
					}
				}
				bool needTopPadding = false;
				if(indent == TreeListRowIndentType.Middle || indent == TreeListRowIndentType.Last) {
					TreeListNode node = Link.TreeList.FindNodeByKeyValue(ri.NodeKey); 
					needTopPadding = (node.ParentNode.ChildNodes[0] == node);
				}
				Link.DrawIndentImageBrick(graph, Styles.Indent, image, new RectangleF(left, topLocation, size.Width, size.Height), needTopPadding);
			}
		}
		protected void DrawCell(BrickGraphics graph, TreeListDataColumn column, RectangleF rect, TreeListRowInfo ri) {
			DevExpress.Web.CreateDisplayControlArgs args = RenderHelper.GetEditorDisplayControlArgs(ri, column);
			string text = column.PropertiesEdit.GetExportDisplayText(args);
			object value = column.PropertiesEdit.GetExportValue(args);
			string url = column.PropertiesEdit.GetExportNavigateUrl(args);
			TreeListExportAppearance style = Styles.Cell;
			if(!String.IsNullOrEmpty(url)) {
				style = new TreeListExportAppearance();
				style.CopyFrom(Styles.Cell);
				style.CopyFrom(Styles.HyperLink);
			}
			if(TreeListPrintUtils.IsImageColumn(column))
				Link.DrawImageBrick(graph, Styles.Image, value as byte[], url, ri, column, TreeListPrintUtils.GetImageExportSettings(column).SizeMode, rect, TreeListRowKind.Data);
			else
				Link.DrawTextBrick(graph, text, value, url, style, ri, column, rect, TreeListRowKind.Data);
		}
		public void CreatePreview(int rowIndex) {
			TreeListRowInfo ri = RenderHelper.Rows[rowIndex];
			string displayText = RenderHelper.GetPreviewText(ri, true);			
			int left = ri.Indents.Count * TreeListPrintInfo.LevelWidth;
			int width = PrintInfo.TotalColumnWidth - left;
			Size textSize = TreeListPrintUtils.CalcTextSize(displayText, Styles.Preview, Graph, width);
			int height = Math.Max(TreeListPrintUtils.GetDefaultHeight(Graph, Styles.Preview, width), textSize.Height);
			BrickStyle defBrickStyle = graph.DefaultBrickStyle;
			DrawIndents(graph, height, ri, true);
			DrawPreview(graph, displayText, new RectangleF(left, topLocation, width, height), ri);
			graph.DefaultBrickStyle = defBrickStyle;
			this.topLocation += height;
		}
		protected void DrawPreview(BrickGraphics graph, string displayText, RectangleF rect, TreeListRowInfo ri) {
			Link.DrawTextBrick(graph, displayText, Styles.Preview, ri, null, rect, TreeListRowKind.Preview);
		}
		public void CreateGroupFooter(int rowIndex) {
			TreeListRowInfo ri = RenderHelper.Rows[rowIndex];
			int height = PrintInfo.GroupFooterHeights[rowIndex];
			BrickStyle defBrickStyle = graph.DefaultBrickStyle;
			DrawIndents(graph, height, ri, true);
			DrawGroupFooter(graph, height, ri);
			graph.DefaultBrickStyle = defBrickStyle;
			this.topLocation += height;
		}
		protected void DrawGroupFooter(BrickGraphics graph, int height, TreeListRowInfo ri) {
			int left = 0;
			foreach(KeyValuePair<TreeListDataColumn, int> pair in PrintInfo.ColumnWidths) {
				TreeListDataColumn column = pair.Key;
				int width = pair.Value;
				if(column == FirstDataColumn) {
					int indentCount = RenderHelper.IsRootIndentVisible ? ri.Indents.Count + 1 : ri.Indents.Count;
					left = indentCount * TreeListPrintInfo.LevelWidth;
					RectangleF rect = new RectangleF(left, topLocation, width - left, height);
					DrawFooterCell(graph, pair.Key, rect, ri);
					left = width;
					continue;
				}
				DrawFooterCell(graph, column, new RectangleF(left, topLocation, width, height), ri);
				left += width;
			}
		}
		public void CreateFooter(int rowIndex) {
			BrickStyle defBrickStyle = graph.DefaultBrickStyle;
			int left = 0;
			foreach(KeyValuePair<TreeListDataColumn, int> pair in PrintInfo.ColumnWidths) {
				int width = pair.Value;
				TreeListDataColumn column = pair.Key;
				if(column == FirstDataColumn) {
					int indentCount = RenderHelper.IsRootIndentVisible ? 1 : 0;
					left = indentCount * TreeListPrintInfo.LevelWidth;
					RectangleF rect = new RectangleF(left, topLocation, width - left, PrintInfo.FooterHeight);
					DrawFooterCell(graph, pair.Key, rect, RenderHelper.RootRowInfo);
					left = width;
					continue;
				}
				DrawFooterCell(graph, pair.Key, new RectangleF(left, topLocation, width, PrintInfo.FooterHeight), RenderHelper.RootRowInfo);
				left += width;
			}
			graph.DefaultBrickStyle = defBrickStyle;
		}
		protected void DrawFooterCell(BrickGraphics graph, TreeListDataColumn column, RectangleF rect, TreeListRowInfo ri) {
			bool isTotal = ri == RenderHelper.RootRowInfo;
			Link.DrawTextBrick(graph, RenderHelper.GetFooterText(ri, column, "\n", true), isTotal ? Styles.Footer : Styles.GroupFooter, ri, column, rect, isTotal ? TreeListRowKind.Footer : TreeListRowKind.GroupFooter);
		}	
		public void CreateInlineEditRow(int rowIndex) {
			CreateDataRow(rowIndex);
		}
		public void CreateHeader() { }
		public void CreateEditFormRow(int rowIndex) { }
		public void CreateErrorRow(int rowIndex) { }
		public void CreateEditFormRow(int rowIndex, bool isAuxRow) { }
	}
	#region Helpers
	public class TreeListPrintUtils {
		public static Size CalcTextSize(string text, TreeListExportAppearance appearance, BrickGraphics graph, int width) {
			BrickStyle brickStyle = CreateBrickStyleFromAppearance(appearance, graph);
			SizeF sizeF = Measurement.MeasureString(text, brickStyle.Font, width, brickStyle.StringFormat.Value, graph.PageUnit);
			return CalcSize(brickStyle, sizeF, brickStyle.Padding.Left + brickStyle.Padding.Right, brickStyle.Padding.Top + brickStyle.Padding.Bottom);
		}
		public static Size CalcImageSize(int width, int height, TreeListExportAppearance appearance, BrickGraphics graph, bool horPaddingInside) {
			BrickStyle brickStyle = CreateBrickStyleFromAppearance(appearance, graph);
			int horPadding = brickStyle.Padding.Left + brickStyle.Padding.Right;
			if(horPaddingInside)
				horPadding = -horPadding;
			return CalcSize(brickStyle, new SizeF(width, height), horPadding, brickStyle.Padding.Top + brickStyle.Padding.Bottom);
		}
		static Size CalcSize(BrickStyle brickStyle, SizeF sizeF, int horPadding, int vertPadding) {
			RectangleF rect;
			rect = new RectangleF(PointF.Empty, sizeF);
			rect = brickStyle.InflateBorderWidth(rect, GraphicsDpi.Pixel);
			Size size = Size.Ceiling(rect.Size);
			size.Width += horPadding;
			size.Height += vertPadding;
			return size;
		}
		public static BrickStyle CreateBrickStyleFromAppearance(TreeListExportAppearance appearance, BrickGraphics graph) {
			BrickStyle brickStyle = new BrickStyle();
			brickStyle.Font = CreateFontFromFontInfo(graph.DefaultFont, appearance.Font);
			brickStyle.BorderColor = appearance.BorderColor;
			brickStyle.BorderWidth = appearance.BorderWidth;
			brickStyle.Sides = appearance.BorderSides;
			brickStyle.BackColor = appearance.BackColor.IsEmpty ? graph.BackColor : appearance.BackColor;
			brickStyle.ForeColor = appearance.ForeColor.IsEmpty ? graph.ForeColor : appearance.ForeColor;
			if(appearance.Wrap == DefaultBoolean.False)
				brickStyle.StringFormat = new BrickStringFormat(brickStyle.StringFormat, StringFormatFlags.NoWrap);
			if(!appearance.Paddings.IsEmpty) {
				brickStyle.Padding = CreatePaddingFromAppearancePadding(appearance.Paddings);
			}
			return brickStyle;
		}
		public static int GetDefaultHeight(BrickGraphics graph, TreeListExportAppearance appearance, int maxWidth) {
			return CalcTextSize("Wg", appearance, graph, maxWidth).Height;
		}
		static Font CreateFontFromFontInfo(Font defaultFont, FontInfo fontInfo) {
			FontStyle fontStyle = FontStyle.Regular;
			if(fontInfo.Bold) fontStyle |= FontStyle.Bold;
			if(fontInfo.Italic) fontStyle |= FontStyle.Italic;
			if(fontInfo.Strikeout) fontStyle |= FontStyle.Strikeout;
			if(fontInfo.Underline) fontStyle |= FontStyle.Underline;
			string familyName = string.IsNullOrEmpty(fontInfo.Name) ? defaultFont.Name : fontInfo.Name;
			float emSize = defaultFont.Size;
			if(!fontInfo.Size.IsEmpty)
				emSize = GetFontSize(fontInfo.Size, emSize);
			return new Font(familyName, emSize, fontStyle);
		}
		static float GetFontSize(FontUnit size, float defaultSize) {
			if(size.Type == FontSize.NotSet || size.Type == FontSize.Medium) return defaultSize;
			if(size.Type == FontSize.AsUnit && !size.Unit.IsEmpty)
				return (float)size.Unit.Value;
			int rank = 10, defaultRank = 10;
			switch(size.Type) {
				case FontSize.Large: rank = 14; break;
				case FontSize.Larger: rank = 16; break;
				case FontSize.XLarge: rank = 20; break;
				case FontSize.XXLarge: rank = 24; break;
				case FontSize.Small: rank = 8; break;
				case FontSize.Smaller: rank = 6; break;
				case FontSize.XSmall: rank = 5; break;
				case FontSize.XXSmall: rank = 4; break;
			}
			return defaultSize * rank / defaultRank;
		}
		static PaddingInfo CreatePaddingFromAppearancePadding(Paddings paddings) {
			int left = GetPaddingValue(paddings.PaddingLeft);
			int right = GetPaddingValue(paddings.PaddingRight);
			int top = GetPaddingValue(paddings.PaddingTop);
			int bottom = GetPaddingValue(paddings.PaddingBottom);
			return new PaddingInfo(left, right, top, bottom);
		}
		static int GetPaddingValue(Unit unit) {
			if(unit.IsEmpty || unit.Type != UnitType.Pixel) return 0;
			return (int)unit.Value;
		}
		public static HorzAlignment GetBrickHorzAlignment(TreeListExportAppearance apperance, HorizontalAlign defaultAlign) {
			HorizontalAlign align = apperance.HorizontalAlign == HorizontalAlign.NotSet ? defaultAlign : apperance.HorizontalAlign;  
			switch(align) {
				case HorizontalAlign.Right: return HorzAlignment.Far;
				case HorizontalAlign.Center: return HorzAlignment.Center;
			}
			return HorzAlignment.Near;
		}
		public static VertAlignment GetBrickVertAlignment(TreeListExportAppearance apperance) {
			switch(apperance.VerticalAlign) {
				case VerticalAlign.Bottom: return VertAlignment.Bottom;
				case VerticalAlign.Top: return VertAlignment.Top;
			}
			return VertAlignment.Center;
		}
		public static bool IsExportWidthAssigned(TreeListDataColumn column) {
			return column.ExportWidth > 0;
		}
		public static bool IsImageColumn(TreeListColumn column) {
			return column as TreeListBinaryImageColumn != null || column as TreeListImageColumn != null;
		}
		public static IImageExportSettings GetImageExportSettings(TreeListDataColumn column) {
			return column != null ? column.PropertiesEdit as IImageExportSettings : null;
		}
	}
	public class IndentsPainter {
		static readonly float Delta = 1f;
		public static void DrawIndent(Graphics g, TreeListRowIndentType indent, Brush brush, Rectangle rect) {
			switch(indent) {
				case TreeListRowIndentType.First:
					DrawFirstIndent(g, brush, rect); 
					break;
				case TreeListRowIndentType.Middle:
					DrawMiddleIndent(g, brush, rect);
					break;
				case TreeListRowIndentType.Root:
					DrawRootIndent(g, brush, rect);
					break;
				case TreeListRowIndentType.Last:
					DrawLastIndent(g, brush, rect);
					break;
			}
		}
		public static void DrawButton(Graphics g, Brush foreBrush, Pen forePen, RectangleF rect, bool expanded) {
			g.FillRectangle(Brushes.White, rect);
			g.DrawRectangle(forePen, rect.X, rect.Y, rect.Width, rect.Height);
			g.FillRectangle(foreBrush, new RectangleF(rect.Left + 2 * Delta, rect.Top + (rect.Height - Delta) / 2, rect.Width - 2 * Delta - 1, Delta));
			if(!expanded)
				g.FillRectangle(foreBrush, new RectangleF(rect.Left + (rect.Width - Delta) / 2, rect.Top + 2 * Delta, Delta, rect.Height - 2 * Delta - 1));
		}
		static void DrawFirstIndent(Graphics g, Brush brush, Rectangle rect) {
			g.FillRectangle(brush,
				new RectangleF(rect.Left + (rect.Width - Delta) / 2,
				rect.Top + (rect.Height - Delta) / 2,
				rect.Width / 2,
				Delta));
			g.FillRectangle(brush,
				new RectangleF(rect.Left + (rect.Width - Delta) / 2,
				rect.Top + (rect.Height - Delta) / 2,
				Delta,
				rect.Height / 2 + Delta));
		}
		static void DrawMiddleIndent(Graphics g, Brush brush, Rectangle rect) {
			g.FillRectangle(brush,
				new RectangleF(rect.Left + (rect.Width - Delta) / 2,
				rect.Top,
				Delta,
				rect.Height));
			g.FillRectangle(brush,
				new RectangleF(rect.Left + (rect.Width - Delta) / 2,
				rect.Top + (rect.Height - Delta) / 2,
				rect.Width / 2,
				Delta));
		}
		static void DrawRootIndent(Graphics g, Brush brush, Rectangle rect) {
			g.FillRectangle(brush,
				new RectangleF(rect.Left + (rect.Width - Delta) / 2,
				rect.Top,
				Delta,
				rect.Height));
		}
		static void DrawLastIndent(Graphics g, Brush brush, Rectangle rect){
			g.FillRectangle(brush, 
				new RectangleF(rect.Left + (rect.Width - Delta) / 2,
				rect.Top, 
				Delta,
				rect.Height / 2));
			g.FillRectangle(brush, 
				new RectangleF(rect.Left + (rect.Width - Delta) / 2,
				rect.Top + (rect.Height - Delta) / 2, 
				rect.Width / 2,
				Delta));
		}
	}
	#endregion
}
