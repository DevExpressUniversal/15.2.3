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
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrintingLinks 
{
	[DefaultProperty("DataGrid")]
	public class DataGridLinkBase : DevExpress.XtraPrinting.LinkBase
	{
		private DataGrid dataGrid;
		private bool autoHeight = true;
		private DataGridTableStyle tableStyle;
		private DataGridPrintStyle printStyle;
		private DataGridPrintStyle activePrintStyle;
		private DataTable dummyTable;
		private bool useDataGridView;
		private object oldDataSource;
		private Point offset;
		private Rectangle[] rects;
		public override Type PrintableObjectType {
			get { return typeof(DataGrid); }
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(null),
		]
		public DataGrid DataGrid {
			get { return dataGrid; }
			set { dataGrid = value; }
		}
		[
		DefaultValue(true),
		Category(NativeSR.CatPrintOptions),
		]
		public bool AutoHeight {
			get { return autoHeight; }
			set { autoHeight = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category(NativeSR.CatPrintOptions),
		]
		public DataGridPrintStyle PrintStyle {
			get {
				if (printStyle == null) printStyle = new DataGridPrintStyle();
				return printStyle;
			}
			set { printStyle = value; }
		}
		[
		DefaultValue(false),
		Category(NativeSR.CatPrintOptions),
		]
		public bool UseDataGridView {
			get { return useDataGridView; }
			set { useDataGridView = value; }
		}
		protected bool IsEmpty { 
			get { 
				try {
					if(dataGrid.DataSource == null || dataGrid.VisibleRowCount == 0) return true;
					return dataGrid[0,0] == null; 
				} catch { return true; } 
			}
		}
		protected bool IsPrintable { get { return (IsEmpty == false && tableStyle != null); }
		}
		protected DataGridPrintStyle ActivePrintStyle {
			get { 
				if(activePrintStyle == null) 
					activePrintStyle = (UseDataGridView && dataGrid != null) ? new DataGridPrintStyle(dataGrid) :
						new DataGridPrintStyle(printStyle);
				return activePrintStyle;
			} 
		}
		protected int ColumnCount { get { return tableStyle.GridColumnStyles.Count; }
		}
		static string GetDisplayText(DataGridTextBoxColumn textBoxStyle, object obj) {
			MethodInfo methodInfo = textBoxStyle.GetType().GetMethod("GetText", BindingFlags.NonPublic | BindingFlags.Instance);
			if(methodInfo != null)
				return (string)methodInfo.Invoke(textBoxStyle, new object[] { obj });
			return Convert.ToString(obj);
		}
		static DataTable MakeDataTable() {
			DataTable table = new DataTable();
			for(int i = 0; i < 3; i++) {
				DataColumn column = new DataColumn();
				column.DataType = System.Type.GetType("System.String");
				column.ColumnName = String.Format("Column{0}", i);
				table.Columns.Add(column);
			}
			for(int i = 0; i < 5; i++) {
				DataRow row = table.NewRow();
				foreach(DataColumn column in table.Columns) {
					row[column] = "abc";
				}
				table.Rows.Add(row);
			}
			table.AcceptChanges();
			return table;
		}
		public DataGridLinkBase() : base() {
		}
		public DataGridLinkBase(System.ComponentModel.IContainer container) : base(container) {
		}
		public DataGridLinkBase(PrintingSystemBase ps) : base(ps) {
		}
		public override void SetDataObject(object data) {
			if(data is DataGrid)
				dataGrid = data as DataGrid;
		}
		private DataGridTableStyle GetTableStyle() {
			try { 
				if(dataGrid.TableStyles.Count > 0) return dataGrid.TableStyles[0]; 
			} 
			catch { 
			}
			try {
				Type t = dataGrid.GetType();
				return (DataGridTableStyle)t.InvokeMember("defaultTableStyle", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, dataGrid, null);
			}
			catch { return null; }
		}
		public override void AddSubreport(PointF offset) {
			if (dataGrid != null) 
				base.AddSubreport(offset);
		}
		protected override void BeforeCreate() {
			if(DataGrid == null)
				throw new NullReferenceException("The DataGrid property value must not be null");
			base.BeforeCreate();
			ps.Graph.PageUnit = GraphicsUnit.Pixel;
			tableStyle = GetTableStyle();
			activePrintStyle = null;
			dummyTable = null;
			try {
				if(dataGrid.Site != null && IsEmpty) {
					dummyTable = MakeDataTable();
					oldDataSource = dataGrid.DataSource;
					dataGrid.DataSource = dummyTable;
				}
			} catch {;}
			FillRects();
			offset = IsEmpty ? Point.Empty : 
				new Point(0, dataGrid.GetCellBounds(0, 0).Y);
			PrintingSystemBase.SetCommandVisibility(new PrintingSystemCommand[] { 
				PrintingSystemCommand.ExportXls,
				PrintingSystemCommand.ExportXlsx, 
				PrintingSystemCommand.ExportTxt, 
				PrintingSystemCommand.ExportCsv, 
				PrintingSystemCommand.ExportRtf, 
				PrintingSystemCommand.ExportHtm, 
				PrintingSystemCommand.ExportMht,
				PrintingSystemCommand.SendXls, 
				PrintingSystemCommand.SendXlsx,
				PrintingSystemCommand.SendTxt, 
				PrintingSystemCommand.SendCsv,
				PrintingSystemCommand.SendRtf,
				PrintingSystemCommand.SendMht }, 
				CommandVisibility.All, Priority.Low);
		}
		private void FillRects() {
			rects = new Rectangle[ColumnCount];
			for(int i = 0; i < rects.Length; i ++) {
				rects[i] = dataGrid.GetCellBounds(0, i);
			}
			if(rects.Length > 0) {
				rects[0].X = 0;
				for(int i = 1; i < rects.Length; i ++) {
					rects[i].X = rects[i - 1].Right + 1;
				}
			}
		}
		protected override void AfterCreate() {
			base.AfterCreate();
			try { 
				if(dummyTable != null) 
					dataGrid.DataSource = oldDataSource;
			} catch {}
			dummyTable = null;
		}
		private Rectangle GetCellBounds(BrickGraphics gr, int row, int col) {
			Rectangle r = dataGrid.GetCellBounds(row,col);
			r.X = rects[col].X;
			r.Width += (int)gr.BorderWidth;
			r.Height += (int)gr.BorderWidth;
			r.Offset(-offset.X, -offset.Y);
			return r;
		}
		protected override void CreateReportHeader(BrickGraphics gr) {
			if(IsPrintable == false || dataGrid.CaptionVisible == false)
				return;
			gr.Font = dataGrid.CaptionFont;
			gr.BackColor = ActivePrintStyle.CaptionBackColor;
			gr.ForeColor = ActivePrintStyle.CaptionForeColor;
			gr.BorderColor = ActivePrintStyle.CaptionBackColor;
			gr.StringFormat = new BrickStringFormat(StringFormatFlags.NoWrap, StringAlignment.Near, StringAlignment.Center);
			Rectangle r1 = GetCellBounds(gr, 0, 0);
			Rectangle r2 = GetCellBounds(gr, 0, tableStyle.GridColumnStyles.Count - 1);
			r1.Height = dataGrid.CaptionFont.Height + 4;
			r1.Width = r2.Right - r1.Left;
			string text = dataGrid.CaptionText;
			if(text.Length == 0 && dummyTable != null) text = "Caption";
			VisualBrick brick = PrintObj(gr, text, ref r1);
			brick.SeparableHorz = true;
		}
		protected override void CreateDetailHeader(BrickGraphics gr) {
			if(IsPrintable == false || dataGrid.ColumnHeadersVisible == false)
				return;
			gr.Font = dataGrid.HeaderFont;
			gr.BackColor = ActivePrintStyle.HeaderBackColor;
			gr.ForeColor = ActivePrintStyle.HeaderForeColor;
			gr.BorderColor = ActivePrintStyle.FlatMode ? ActivePrintStyle.HeaderBackColor :
				Color.White;
			gr.StringFormat = new BrickStringFormat(StringFormatFlags.NoWrap, StringAlignment.Near, StringAlignment.Center);
			int rectHeight = dataGrid.HeaderFont.Height + 4;
			GridColumnStylesCollection gridColumns = tableStyle.GridColumnStyles;
			for(int j = 0; j < gridColumns.Count; j++) {
				Rectangle r = GetCellBounds(gr, 0, j);
				r.Height = rectHeight;
				PrintObj(gr, gridColumns[j].HeaderText, ref r);
			}
		}
		protected override void CreateDetail(BrickGraphics gr) {
			if(IsPrintable == false) return;
			gr.Font = dataGrid.Font;
			gr.BackColor = ActivePrintStyle.AlternatingBackColor;
			gr.ForeColor = ActivePrintStyle.ForeColor;
			gr.BorderColor = ActivePrintStyle.GridLineStyle == DataGridLineStyle.Solid ?
				ActivePrintStyle.GridLineColor : Color.Empty;
			gr.BackColor = gr.BackColor.Equals(ActivePrintStyle.BackColor) ?
				ActivePrintStyle.AlternatingBackColor : ActivePrintStyle.BackColor;
			gr.StringFormat = new BrickStringFormat(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit);
			GridColumnStylesCollection gridColumns = tableStyle.GridColumnStyles;
			Brick[] bricks = new Brick[gridColumns.Count];
			Rectangle[] rects = new Rectangle[gridColumns.Count];
			int rowPos = 0;
			for(int i = 0; ; i++) {
				int maxHeight = 0;
				for(int j = 0; j < gridColumns.Count; j++) {
					try {
						Rectangle rect = GetCellBounds(gr, i, j);
						rect.Y = rowPos;
						bricks[j] = CreateBrick(dataGrid[i, j], ref rect, gridColumns[j]);
						rects[j] = rect;
						maxHeight = Math.Max(maxHeight, rect.Height);
					} catch { return; }
				}
				rowPos += maxHeight;
				for(int k = 0; k < bricks.Length; k++) {
					rects[k].Height = maxHeight;
					gr.DrawBrick(bricks[k], rects[k]);
				}
			}
		}
		private VisualBrick CreateBrick(object obj, ref Rectangle rect, DataGridColumnStyle style) {
			VisualBrick brick = null;
			if(obj is byte[]) {
				byte[] bytes = (byte[]) obj;
				Image img = PSConvert.ImageFromArray(bytes);
				if(img != null) {
					if(autoHeight) rect.Height = (int)((float)img.Size.Height / (float)img.Size.Width * (float)rect.Width);
					brick = new ImageBrick();
					((ImageBrick)brick).Image = img;
					brick.BackColor = Color.Transparent;
				}
			} else if(obj is bool) {
				brick = new CheckBoxBrick();
				((CheckBoxBrick)brick).Checked = Convert.ToBoolean(obj);
			} else {
				brick = new TextBrick();
				DataGridTextBoxColumn textBoxStyle = style as DataGridTextBoxColumn;
				if (textBoxStyle != null) {
					((TextBrick)brick).Text = GetDisplayText(textBoxStyle, obj); 
				}
				else {
					((TextBrick)brick).Text = Convert.ToString(obj);
				}
			}
			if(brick != null) brick.Sides = BorderSide.Left | BorderSide.Top | BorderSide.Right | BorderSide.Right | BorderSide.Bottom;
			return brick;
		}
		private VisualBrick PrintObj(BrickGraphics gr, object obj, ref Rectangle rect) {
			VisualBrick brick = CreateBrick(obj, ref rect, null);
			if(brick != null) gr.DrawBrick(brick, rect); 
			return brick;
		}
	}
	[
	TypeConverter(typeof(ExpandableObjectConverter)),
	]
	public class DataGridPrintStyle 
	{
		#region Fields & Properties
		private Color captionBackColor = SystemColors.ActiveCaption;
		private Color captionForeColor = SystemColors.ActiveCaptionText;
		private Color headerBackColor = SystemColors.Control;
		private Color headerForeColor = SystemColors.ControlText;
		private Color alternatingBackColor = SystemColors.Window;
		private Color backColor = SystemColors.Window;
		private Color foreColor = SystemColors.WindowText;
		private Color gridLineColor = SystemColors.Control;
		private bool flatMode;
		private DataGridLineStyle gridLineStyle = DataGridLineStyle.Solid;
		[
		NotifyParentProperty(true),
		]
		public Color CaptionBackColor {
			get { return captionBackColor; }
			set { captionBackColor = value; }
		}
		[
		NotifyParentProperty(true),
		]
		public Color CaptionForeColor {
			get { return captionForeColor; }
			set { captionForeColor = value; }
		}
		[
		NotifyParentProperty(true),
		]
		public Color HeaderBackColor {
			get { return headerBackColor; }
			set { headerBackColor = value; }
		}
		[
		NotifyParentProperty(true),
		]
		public Color HeaderForeColor {
			get { return headerForeColor; }
			set { headerForeColor = value; }
		}
		[
		NotifyParentProperty(true),
		]
		public Color AlternatingBackColor {
			get { return alternatingBackColor; }
			set { alternatingBackColor = value; }
		}
		[
		NotifyParentProperty(true),
		]
		public Color BackColor {
			get { return backColor; }
			set { backColor = value; }
		}
		[
		NotifyParentProperty(true),
		]
		public Color ForeColor {
			get { return foreColor; }
			set { foreColor = value; }
		}
		[
		NotifyParentProperty(true),
		]
		public Color GridLineColor {
			get { return gridLineColor; }
			set { gridLineColor = value; }
		}
		[
		DefaultValue(false),
		NotifyParentProperty(true),
		]
		public bool FlatMode {
			get { return flatMode; }
			set { flatMode = value; }
		}
		[
		DefaultValue(DataGridLineStyle.Solid),
		NotifyParentProperty(true),
		]
		public DataGridLineStyle GridLineStyle {
			get { return gridLineStyle; }
			set { gridLineStyle = value; }
		}
		#endregion
		public DataGridPrintStyle() {
		}
		public DataGridPrintStyle(DataGrid dataGrid) {
			CopyFrom(dataGrid);
		}
		public DataGridPrintStyle(DataGridPrintStyle printStyle) {
			CopyFrom(printStyle);
		}
		public void CopyFrom(DataGrid dataGrid) {
			try {
				this.captionBackColor = dataGrid.CaptionBackColor;
				this.captionForeColor = dataGrid.CaptionForeColor;
				this.headerBackColor = dataGrid.HeaderBackColor;
				this.headerForeColor = dataGrid.HeaderForeColor;
				this.alternatingBackColor = dataGrid.AlternatingBackColor;
				this.backColor = dataGrid.BackColor;
				this.foreColor = dataGrid.ForeColor;
				this.gridLineColor = dataGrid.GridLineColor;
				this.flatMode = dataGrid.FlatMode;
				this.gridLineStyle = dataGrid.GridLineStyle;
			} catch {;}
		}
		public void CopyFrom(DataGridPrintStyle printStyle) {
			if(printStyle == null) return;
			try {
				this.captionBackColor = printStyle.CaptionBackColor;
				this.captionForeColor = printStyle.CaptionForeColor;
				this.headerBackColor = printStyle.HeaderBackColor;
				this.headerForeColor = printStyle.HeaderForeColor;
				this.alternatingBackColor = printStyle.AlternatingBackColor;
				this.backColor = printStyle.BackColor;
				this.foreColor = printStyle.ForeColor;
				this.gridLineColor = printStyle.GridLineColor;
				this.flatMode = printStyle.FlatMode;
				this.gridLineStyle = printStyle.GridLineStyle;
			} catch {;}
		}
		public override string ToString() {
			return GetType().Name;
		}
		#region serialization
		protected bool ShouldSerializeCaptionBackColor() { 
			return captionBackColor != SystemColors.ActiveCaption;
		}
		protected bool ShouldSerializeCaptionForeColor() {
			return captionForeColor != SystemColors.ActiveCaptionText;
		}
		protected bool ShouldSerializeHeaderBackColor() {
			return headerBackColor != SystemColors.Control;
		}
		protected bool ShouldSerializeHeaderForeColor() {
			return headerForeColor != SystemColors.ControlText;
		}
		protected bool ShouldSerializeAlternatingBackColor() {
			return alternatingBackColor != SystemColors.Window;
		}
		protected bool ShouldSerializeBackColor() {
			return backColor != SystemColors.Window;
		}
		protected bool ShouldSerializeForeColor() {
			return foreColor != SystemColors.WindowText;
		}
		protected bool ShouldSerializeGridLineColor() {
			return gridLineColor != SystemColors.Control;
		}
		#endregion
	}
}
