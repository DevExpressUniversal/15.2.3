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
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraGrid.Printing;
namespace DevExpress.XtraGrid.Views.Printing {
	public class ColumnViewPrintInfo : BaseViewPrintInfo {
		public ColumnViewPrintInfo(PrintInfoArgs args) : base(args) { }
		public new ColumnView View { get { return base.View as ColumnView; } }
		public new ColumnViewPrintAppearances AppearancePrint { get { return base.AppearancePrint as ColumnViewPrintAppearances; } }
		public override void Initialize() {
			base.Initialize();
			Bricks.Add("FilterPanel", AppearancePrint.FilterPanel, BorderSide.All, LineColor, 1);
		}
		protected void PrintViewHeaderCore(IBrickGraphics graph, int width) {
			if(View.OptionsView.ShowViewCaption) {
				bool usePrintStyles = false;
				if(!usePrintStyles) {
					AppearanceObject temp = new AppearanceObject();
					AppearanceHelper.Combine(temp, new AppearanceObject[] { View.ViewInfo.PaintAppearance.ViewCaption });
					SetDefaultBrickStyle(graph, Bricks.Create(temp, BorderSide.All, temp.BorderColor, 1));
				}
				else SetDefaultBrickStyle(graph, Bricks["HeaderPanel"] == null ? Bricks["CardCaption"] : Bricks["HeaderPanel"]);
				Rectangle r = new Rectangle(0, 0, width, 30); 
				r.Offset(Indent, 0);
				ITextBrick itb = DrawTextBrick(graph, View.GetViewCaption(), r);
			}
		}
		public override void PrintViewHeader(IBrickGraphics graph) {
			PrintViewHeaderCore(graph, MaximumWidth);
		}
		protected virtual bool PrintSelectedRowsOnly { get { return false; } }
		protected virtual bool AllowPrintFilterInfo { get { return false; } }
		protected virtual void PrintFilterInfo(DevExpress.XtraPrinting.IBrickGraphics graph) {
			if(!AllowPrintFilterInfo) return;
			if(View.RowFilter == null || View.RowFilter.Length == 0 || !View.ActiveFilterEnabled) return;
			RectangleF r = RectangleF.Empty;
			r.Y = Y;
			r.X = Indent;
			r.Width = MaxWidth - r.X;
			GInfo.AddGraphics(null);
			try {
				r.Height = (int)AppearancePrint.FilterPanel.CalcTextSize(GInfo.Graphics, View.FilterPanelText, (int)r.Width - 2).Height + 1;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			r.Inflate(0, 1);
			r.Y++;
			if(r.Y < 0) r.Y = 0;
			SetDefaultBrickStyle(graph, Bricks["FilterPanel"]);
			DrawTextBrick(graph, View.FilterPanelText, r, true);
		}
		protected virtual void MakeSelectedRowList() {
			Rows.AddRange(MakeSelectedRowListCore(View));
		}
		internal static IList MakeSelectedRowListCore(ColumnView view) {
			ArrayList rows = new ArrayList();
			if(!view.IsMultiSelect) {
				if(view.IsValidRowHandle(view.FocusedRowHandle)) rows.Add(view.FocusedRowHandle);
				return rows;
			}
			if(view.SelectedRowsCount == 0) return rows;
			int[] rowsa = view.DataController.Selection.GetNormalizedSelectedRowsEx2();
			rows.AddRange(rowsa);
			return rows;
		}
		protected virtual void AddSelectedRows(int[] rows) {
			Rows.AddRange(rows);
		}
	}
	public class BaseViewPrintInfo : IDisposable {
		ArrayList rows;
		BaseViewAppearanceCollection appearancePrint;
		BaseView view;
		IPrintingSystem ps;
		IBrickGraphics graph;
		int indent, y;
		GraphicsInfo ginfo;
		BrickCache bricks;
		ViewPrintWrapper printWrapper;
		public BaseViewPrintInfo(PrintInfoArgs args) {
			this.printWrapper = args.PrintWrapper;
			this.bricks = new BrickCache();
			this.view = args.View as ColumnView;
			this.appearancePrint = CreatePrintAppearance();
			this.rows = new ArrayList();
			this.ginfo = new GraphicsInfo();
			this.ginfo.CreateCache();
			this.ginfo.Cache.Paint = new DevExpress.Utils.Paint.XPrintPaint();
			this.indent = args.Indent;
			this.y = args.Y;
			this.graph = args.Graph;
		}
		int lastProgress = -1;
		protected virtual void ReportProgress(int progress) {
			if(progress == lastProgress) return;
			this.lastProgress = progress;
			View.ReportProgress(progress);
		}
		protected ViewPrintWrapper PrintWrapper { get { return printWrapper; } }
		protected virtual Color LineColor { get { return Color.Black; } }
		public GraphicsInfo GInfo { get { return ginfo; } }
		public virtual void Initialize() {
			InitializeAppearances();
			UpdateAppearances();
			Bricks.Clear();
		}
		protected internal bool CancelPending { get { return PrintingSystemBase.CancelPending || (View.ProgressWindow != null && View.ProgressWindow.CancelPending); } }
		protected virtual void UpdateAppearances() {
		}
		protected AppearanceDefault Find(string name, AppearanceDefaultInfo[] info) {
			for(int n = info.Length - 1; n >= 0; n--) {
				if(info[n].Name == name) return info[n].DefaultAppearance;
			}
			return null;
		}
		protected virtual void InitializeAppearances() {
			AppearancePrint.Combine(View.AppearancePrint, View.BaseInfo.GetDefaultPrintAppearance());
		}
		public BrickCache Bricks { get { return bricks; } }
		protected virtual BaseViewAppearanceCollection CreatePrintAppearance() {
			return new BaseViewAppearanceCollection(View);
		}
		public BaseViewAppearanceCollection AppearancePrint { get { return appearancePrint; } }
		protected virtual BaseViewInfo ViewViewInfoCore { get { return View.ViewInfo; } }
		protected BaseViewInfo ViewViewInfo { get { return ViewViewInfoCore; } }
		public virtual void Dispose() {
		}
		public GridControl Grid { get { return View == null ? null : View.GridControl; } }
		public BaseView View { get { return view; } }
		public int Y { get { return y; } set { y = value; } }
		public int Indent { get { return indent; } set { indent = value; } }
		public IPrintingSystem PS { 
			get {
				if(ps == null) ps = PrintWrapper.PrintingSystem;
				return ps; 
			} 
		}
		public PrintingSystemBase PrintingSystemBase { get { return PS as PrintingSystemBase; } }
		public IBrickGraphics Graph { get { return graph; } }
		public ArrayList Rows { get { return rows; } }
		public virtual void PrintHeader(DevExpress.XtraPrinting.IBrickGraphics graph) {
		}
		public virtual void PrintViewHeader(DevExpress.XtraPrinting.IBrickGraphics graph) {
		}
		public virtual void PrintRows(DevExpress.XtraPrinting.IBrickGraphics graph) {
		}
		public virtual void PrintNextRow(DevExpress.XtraPrinting.IBrickGraphics graph) {
		}
		public virtual int GetDetailCount() {
			return 0;
		}
		public virtual void PrintFooter(DevExpress.XtraPrinting.IBrickGraphics graph) {
			ReportProgress(100);
		}
		protected int MaximumWidth {
			get {
				if(Graph == null) return 1;
				SizeF size = ((BrickGraphics)Graph).ClientPageSize;
				return (int)size.Width - Indent;
			}
		}
		protected Color ConvertColor(Color color) {
			return Color.FromArgb(color.R, color.G, color.B);
		}
		protected virtual int MaxWidth { get { return MaximumWidth; } }
		protected int CalcStyleHeight(AppearanceObject appearance) {
			int res = 0;
			GInfo.AddGraphics(null);
			try {
				res = appearance.CalcDefaultTextSize(GInfo.Graphics).Height;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		protected virtual void SetDefaultBrickStyle(IBrickGraphics graph, BrickStyle style) {
			graph.DefaultBrickStyle = style;
		}
		protected IVisualBrick DrawEmptyBrick(IBrickGraphics graph, RectangleF rect) {
			return DrawBrick(graph, "EmptyBrick", rect);
		}
		protected ITextBrick DrawTextBrick(IBrickGraphics graph, string text, RectangleF rect, bool separable) {
			ITextBrick brick = (ITextBrick)DrawBrick(graph, "TextBrick", rect);
			brick.Text = text;
			brick.TextValue = text;
			if(brick.ForeColor == Color.Empty) brick.ForeColor = Color.Black;
			brick.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 0, 0, 0, GraphicsUnit.Pixel);
			if(separable) brick.SeparableHorz = true;
			return brick;
		}
		protected ITextBrick DrawTextBrick(IBrickGraphics graph, string text, RectangleF rect) {
			return DrawTextBrick(graph, text, rect, false);
		}
		bool usePanelBrickDisableDrawing = false;
		protected bool UsePanelBrickDisableDrawing {
			get { return usePanelBrickDisableDrawing; }
			set { usePanelBrickDisableDrawing = value; }
		}
		protected IVisualBrick DrawBrick(IBrickGraphics graph, string typeName, RectangleF rect) {
			IVisualBrick brick = null;
			switch(typeName) {
				case "PanelBrick": brick = new PanelBrick(); break;
				case "TextBrick": brick = new TextBrick(); break;
				case "EmptyBrick": brick = new TextBrick(); break;
			}
			brick.Style.StringFormat = new BrickStringFormat(brick.Style.StringFormat, StringFormatFlags.NoClip | brick.Style.StringFormat.FormatFlags);
			brick.Style.StringFormat.PrototypeKind = BrickStringFormatPrototypeKind.GenericTypographic;
			if(UsePanelBrickDisableDrawing) {
				brick.Rect = rect;
				return brick;
			}
			return (IVisualBrick)graph.DrawBrick(brick, rect);
		}
		protected virtual void MakeRowList() {
			int n;
			rows.Clear();
			int rCount = View.DataController.VisibleCount;
			for(n = 0; n < rCount; n++) {
				rows.Add(View.DataController.GetControllerRowHandle(n));
			}
		}
	}
	public class CellInfo {
		public int RowHandle;
		public DevExpress.XtraEditors.Repository.RepositoryItem Item;
		public object EditValue;
		public string DisplayText;
		public AppearanceObject Appearance;
		public CellInfo(int rowHandle, string displayText, object editValue, AppearanceObject appearance, DevExpress.XtraEditors.Repository.RepositoryItem item) {
			this.DisplayText = displayText;
			this.Appearance = appearance;
			this.RowHandle = rowHandle;
			this.Item = item;
			this.EditValue = editValue;
		}
	}
}
