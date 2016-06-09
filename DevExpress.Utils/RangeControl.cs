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
using System.Text;
using System.Drawing;
using DevExpress.Utils.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Paint;
namespace DevExpress.XtraEditors {
	public delegate void RangeChangedEventHandler(object sender, RangeControlRangeEventArgs range);
	public enum PrintImageFormat { Bitmap, Metafile }
	public class PrintingRangeControl : IPrintable, IRangeControl {
		RangeControlRange range = new RangeControlRange();
		RangeControlRange selectedRange = new RangeControlRange();
		IPrintingSystem printingSystem;
		ILink link;
		public PrintingRangeControl() :this(PrintImageFormat.Metafile){ }
		public PrintingRangeControl(PrintImageFormat imageFormat) {
			ImageFormat = imageFormat;
		}
		public Size Size { get; set; }
		RangeContolViewInfoBase viewInfo;
		RangeContolViewInfoBase ViewInfo {
			get {
				if(viewInfo == null) {
					viewInfo = CreateViewInfo();
				}
				return viewInfo;
			}
		}
		RangeContolViewInfoBase CreateViewInfo() {
			return new RangeContolViewInfoBase(this);
		}
		IRangeControlClient client;
		public IRangeControlClient Client {
			get { return client; }
			set {
				if(Client == value)
					return;
				if(Client != null)
					Client.RangeChanged -= new ClientRangeChangedEventHandler(OnClientRangeChanged);
				client = value;
				if(Client != null) {
					Client.RangeChanged -= new ClientRangeChangedEventHandler(OnClientRangeChanged);
					Client.RangeChanged += new ClientRangeChangedEventHandler(OnClientRangeChanged);
				}
			}
		}
		protected internal UserLookAndFeel LookAndFeel { get { return UserLookAndFeel.Default; } }
		protected virtual bool EnableAzureCompatibility { get { return false; } }
		public PrintImageFormat ImageFormat { get; set; }
		#region IBasePrintable, IPrintable
		void IPrintable.AcceptChanges() { }
		bool IPrintable.CreatesIntersectedBricks { get { return true; } }
		bool IPrintable.HasPropertyEditor() { return false; }
		UserControl IPrintable.PropertyEditorControl { get { return null; } }
		void IPrintable.RejectChanges() { }
		void IPrintable.ShowHelp() { }
		bool IPrintable.SupportsHelp() { return false; }
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			if(printingSystem != null && areaName == SR.Detail) {
				CreateDetail(graph);
			}
		}
		public virtual void CreateDetail(IBrickGraphics graph) {
			ImageBrick brick = printingSystem.CreateBrick("ImageBrick") as ImageBrick;
			brick.Image = ImageFormat == PrintImageFormat.Bitmap ? (Image)CreateBitmap(Size) : brick.Image = (Image)CreateMetafile(Size, MetafileFrameUnit.Pixel);
			brick.Sides = BorderSide.None;
			brick.SizeMode = ImageSizeMode.Normal;
			Rectangle bounds = new Rectangle(Point.Empty, Size);
			graph.DrawBrick(brick, bounds);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			SetPS(null);
			link = null;
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink lnk) {
			SetPS(ps);
			link = lnk as LinkBase;
		}
		public IPrintable Printable { get { return (IPrintable)this; } }
		void OnPrintingSystem_AfterChange(object sender, ChangeEventArgs e) {
			PrintingSystemBase printingSystemBase = printingSystem as PrintingSystemBase;
			LinkBase linkBase = link as LinkBase;
			if(printingSystemBase != null && linkBase != null && (e.EventName == SR.PageSettingsChanged || e.EventName == SR.AfterMarginsChange)) {
				linkBase.Margins = printingSystemBase.PageMargins;
				linkBase.CreateDocument();
			}
		}
		void SetPS(IPrintingSystem ps) {
			if(printingSystem != null)
				printingSystem.AfterChange -= new ChangeEventHandler(OnPrintingSystem_AfterChange);
			printingSystem = ps;
			if(printingSystem != null) {
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportGraphic, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportHtm, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportPdf, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportRtf, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportXls, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportXlsx, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.SendGraphic, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.SendPdf, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.SendRtf, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.SendXls, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.SendXlsx, true);
				printingSystem.AfterChange += new ChangeEventHandler(OnPrintingSystem_AfterChange);
			}
		}
		#endregion
		void DrawMetafile(Metafile metafile, Rectangle bounds) {
			using(Graphics gr = Graphics.FromImage(metafile)) {
				DrawContent(gr, bounds, false);
				gr.Dispose();
			}
		}
		public Metafile CreateMetafile(Size size, MetafileFrameUnit units) {
			Rectangle metafileBounds = new Rectangle(Point.Empty, new Size(size.Width, size.Height));
			const int indent = 3;
			Rectangle drawingBounds = new Rectangle(metafileBounds.X + indent, metafileBounds.Y + indent, metafileBounds.Right - 2 * indent, metafileBounds.Bottom - 2 * indent);
			Metafile metafile = CreateMetafileInstance(null, metafileBounds, units, EmfType.EmfPlusOnly);
			try {
				DrawMetafile(metafile, drawingBounds);
			}
			catch {
				metafile.Dispose();
				throw;
			}
			return metafile;
		}
		public Bitmap CreateBitmap(Size size) {
			return PrintingRangeBitMapContainer.Draw(this, size);
		}
		public void DrawContent(Graphics gr, Rectangle bounds, bool useImageCache) {
			XPaint currentPaint = XPaint.GetCurrentPaint();
			if(EnableAzureCompatibility)
				XPaint.ForceGDIPlusPaint();
			gr.PageUnit = GraphicsUnit.Pixel;
			GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(new PaintEventArgs(gr, bounds)));
			ViewInfo.CalcViewInfo(bounds, gr);
			GraphicsInfoArgs info = new GraphicsInfoArgs(cache, ViewInfo.Bounds);
			try {
				DrawBackground(info);
				DrawClient(info);
				DrawOutOfRangeMask(info);
				DrawRuler(info);
				DrawFlags(info);
			}
			catch {
				cache.Dispose();
			}
			finally {
				XPaint.RestorePaint(currentPaint);
			}
		}
		protected virtual RangeControlPaintEventArgs CreatePaintArgs(GraphicsCache cache) {
			RangeControlPaintEventArgs res = new RangeControlPaintEventArgs() {
				RangeControl = this,
				Cache = cache,
				ContentBounds = ViewInfo.ClientBounds,
				ActualRangeMaximum = Client.GetNormalizedValue(this.selectedRange.Maximum),
				ActualRangeMinimum = Client.GetNormalizedValue(this.selectedRange.Minimum)
			};
			return res;
		}
		protected virtual void DrawFlags(GraphicsInfoArgs info) {
			object minValue = Client.GetNormalizedValue(this.selectedRange.Minimum);
			object maxValue = Client.GetNormalizedValue(this.selectedRange.Maximum);
			ViewInfo.Calculator.DrawFlags(info, Client, ViewInfo.PaintAppearance, minValue, maxValue);
		}
		protected virtual void DrawClient(GraphicsInfoArgs info) {
			Client.DrawContent(CreatePaintArgs(info.Cache));
		}
		protected virtual void DrawBackground(GraphicsInfoArgs info) {
			info.Graphics.FillRectangle(info.Cache.GetSolidBrush(ViewInfo.BackColor), ViewInfo.RangeBounds);
		}
		protected virtual void DrawOutOfRangeMask(GraphicsInfoArgs info) {
			if(ViewInfo.LeftOutOfRangeBounds.Width > 0)
				info.Graphics.FillRectangle(info.Cache.GetSolidBrush(ViewInfo.OutOfRangeMaskColor), ViewInfo.LeftOutOfRangeBounds);
			if(viewInfo.RightOutOfRangeBounds.Width > 0)
				info.Graphics.FillRectangle(info.Cache.GetSolidBrush(ViewInfo.OutOfRangeMaskColor), viewInfo.RightOutOfRangeBounds);
		}
		protected virtual void DrawRuler(GraphicsInfoArgs info) {
			ViewInfo.Calculator.DrawRuler(info, ViewInfo.PaintAppearance, Client, CreatePaintArgs(info.Cache), ViewInfo.Ruler); 
		}
		protected virtual void OnClientRangeChanged(object sender, RangeControlClientRangeEventArgs e) {
			selectedRange.InternalSetMinimum(e.Range.Minimum);
			selectedRange.InternalSetMaximum(e.Range.Maximum);
		}
		static Metafile CreateMetafileInstance(Stream stream, Rectangle bounds, MetafileFrameUnit units, EmfType emfType) {
			using(Graphics gr = Graphics.FromHwnd(IntPtr.Zero)) {
				IntPtr hdc = gr.GetHdc();
				try {
					return stream == null ? new Metafile(hdc, bounds, units, emfType) : new Metafile(stream, hdc, bounds, units, emfType);
				}
				finally {
					gr.ReleaseHdc(hdc);
				}
			}
		}
		#region IRangeControl
		int IRangeControl.CalcX(double normalizedValue) {
			Rectangle rect = ViewInfo.ScrollBounds;
			return rect.X + (int)((normalizedValue - ViewInfo.VisibleStartPosition) / ViewInfo.VisibleRangeWidth * rect.Width);
		}
		Matrix IRangeControl.NormalTransform {
			get { return new Matrix(); }
		}
		Color IRangeControl.BorderColor { get { return ViewInfo.BorderColor; } }
		Color IRangeControl.RulerColor { get { return ViewInfo.RulerColor; } }
		Color IRangeControl.LabelColor { get { return ViewInfo.LabelColor; } }
		public virtual double VisibleRangeStart { get { return 0.0; } }
		double IRangeControl.VisibleRangeStartPosition { get { return VisibleRangeStart; } }
		public virtual double VisibleRangeEnd { get { return 1.0; } }
		double IRangeControl.VisibleRangeWidth { get { return VisibleRangeEnd - VisibleRangeStart; } }
		RangeControlRange IRangeControl.SelectedRange {
			get { return selectedRange; }
			set {
				if(selectedRange == value)
					return;
				selectedRange = value;
			}
		}
		void IRangeControl.CenterSelectedRange() { }
		bool IRangeControl.AnimateOnDataChange {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		bool IRangeControl.IsValidValue(object value) { return true; }
		void IRangeControl.OnRangeMinimumChanged(object range) { }
		void IRangeControl.OnRangeMaximumChanged(object range) { }
		double IRangeControl.ConstrainRangeMinimum(double value) {
			throw new NotImplementedException();
		}
		double IRangeControl.ConstrainRangeMaximum(double value) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class RangeContolViewInfoBase {
		IRangeControl RangeControl { get; set; }
		public RangeContolViewInfoBase(IRangeControl rangeControl) {
			RangeControl = rangeControl;
			PaintAppearance = CreatePaintAppearance();
		}
		List<object> ruler;
		protected internal virtual List<object> Ruler { 
			get { 
				if(ruler == null)
					ruler = RangeControl.Client.GetRuler(new RulerInfoArgs(RangeControl.Client.GetValue(VisibleStartPosition), RangeControl.Client.GetValue(VisibleRangeWidth), Bounds.Size.Width));
				return ruler;
			} 
		}
		public virtual int MinIndentBetweenTicks { get { return Calculator.MinIndentBetweenTicks; } }
		public virtual int RuleTextTopIndent { get { return Calculator.RuleTextTopIndent; } }
		public virtual int RuleTextBottomIndent { get { return Calculator.RuleTextBottomIndent; } }
		protected internal Color LabelColor { get { return Calculator.LabelColor; } }
		protected internal Color BorderColor { get { return Color.FromArgb(255, 185, 185, 185); } }
		protected internal Color BackColor { get { return Color.FromArgb(255, 255, 255, 255); } }
		protected internal Color RulerColor { get { return Calculator.RulerColor; } }
		protected internal Color OutOfRangeMaskColor {
			get {
				return Color.FromArgb(120, 255, 255, 255);
			}
		}
		protected internal double VisibleStartPosition { get{ return Calculator.VisibleRangeStartPosition; } set { Calculator.VisibleRangeStartPosition = value; } }
		protected internal double VisibleRangeWidth { get { return Calculator.VisibleRangeWidth; } set { Calculator.VisibleRangeWidth = value; } }
		protected internal Graphics Graphics { get { return Calculator.Graphics; } set { Calculator.Graphics = value; } }
		protected virtual AppearanceObjectPrint CreatePaintAppearance() { return new AppearanceObjectPrint(); }
		public AppearanceObject PaintAppearance { get; set; }
		protected internal Rectangle Bounds { get; set; }
		protected internal Rectangle ClientRect { get; set; }
		protected internal Rectangle ClientBounds { get; set; }
		protected internal Rectangle ScrollBounds { get { return Calculator.ScrollBounds; } set { Calculator.ScrollBounds = value; } }
		public Rectangle ScrollBarAreaBounds { get { return Calculator.ScrollBarAreaBounds; } set { Calculator.ScrollBarAreaBounds = value; } }
		public Rectangle LeftOutOfRangeBounds { get { return Calculator.LeftOutOfRangeBounds; } set { Calculator.LeftOutOfRangeBounds = value; } }
		public Rectangle RightOutOfRangeBounds { get { return Calculator.RightOutOfRangeBounds; } set { Calculator.RightOutOfRangeBounds = value; } }
		public Rectangle RangeBounds { get { return Calculator.RangeBounds; } set { Calculator.RangeBounds = value; } }
		public Rectangle RulerBounds { get { return Calculator.RulerBounds; } set { Calculator.RulerBounds = value; } }
		public Rectangle MinRangeFlagBounds { get { return Calculator.MinRangeFlagBounds; } set { Calculator.MinRangeFlagBounds = value; } }
		public Rectangle MaxRangeFlagBounds { get { return Calculator.MaxRangeFlagBounds; } set { Calculator.MaxRangeFlagBounds = value; } }
		protected UserLookAndFeel LookAndFeel { get { return ((PrintingRangeControl)RangeControl).LookAndFeel; } }
		protected internal void CalcViewInfo(Rectangle bounds, Graphics gr) {
			Bounds = bounds;
			ClientRect = Bounds;
			Graphics = gr;
			ScrollBarAreaBounds = CalcScrollBarAreaBounds(bounds);
			RulerBounds = GetRulerBounds(bounds);
			ScrollBounds = CalcScrollBounds(bounds);
			VisibleStartPosition = RangeControl.VisibleRangeStartPosition;
			VisibleRangeWidth = RangeControl.VisibleRangeWidth;
			RangeBounds = GetRangeBounds(ScrollBounds);
			ClientBounds = ScrollBounds;
			LeftOutOfRangeBounds = GetLeftOutArea();
			RightOutOfRangeBounds = GetRightOutArea();
			MinRangeFlagBounds = GetMinFlagBounds(gr);
			MaxRangeFlagBounds = GetMaxFlagBounds(gr);
		}
		protected int ScrollBarHeight { get { return Calculator.ScrollBarHeight; } }
		protected virtual Rectangle CalcScrollBarAreaBounds(Rectangle bounds) {
			return Calculator.CalcScrollBarAreaBounds(bounds);
		}
		public virtual Rectangle GetRulerBounds(Rectangle bounds) {
			return Calculator.GetRulerBounds(Bounds, PaintAppearance, false, true);
		}
		protected virtual Rectangle CalcScrollBounds(Rectangle bounds) {
			return Calculator.CalcScrollBounds(bounds);
		}
		Rectangle GetRangeBounds(Rectangle scrollbounds) {
			object min = RangeControl.Client.GetNormalizedValue(RangeControl.SelectedRange.Minimum);
			object max = RangeControl.Client.GetNormalizedValue(RangeControl.SelectedRange.Maximum);
			int rangeRight = (int)(((double)max - VisibleStartPosition) * scrollbounds.Width + 0.5);
			int rangeLeft = (int)(((double)min - VisibleStartPosition) * scrollbounds.Width + 0.5);
			int rangeWidth = rangeRight - rangeLeft;
			return new Rectangle(scrollbounds.X + rangeLeft, scrollbounds.Y + RangeControl.Client.RangeBoxTopIndent, rangeWidth, scrollbounds.Height - RangeControl.Client.RangeBoxBottomIndent);
		}
		Rectangle GetLeftOutArea() {
			return Calculator.CalcLeftOutOfRangeAreaBounds(Bounds, RangeBounds);
		}
		Rectangle GetRightOutArea() {
			return Calculator.CalcRightOutOfRangeAreaBounds(Bounds, RangeBounds);
		}
		Rectangle GetMaxFlagBounds(Graphics g) {
			string text = RangeControl.Client.ValueToString(RangeControl.Client.GetNormalizedValue(RangeControl.SelectedRange.Maximum));
			return Calculator.CalcMaxRangeFlagBounds(g, text, PaintAppearance);
		}
		Rectangle GetMinFlagBounds(Graphics g) {
			string text = RangeControl.Client.ValueToString(RangeControl.Client.GetNormalizedValue(RangeControl.SelectedRange.Minimum));
			return Calculator.CalcMinRangeFlagBounds(g, text, PaintAppearance);
		}
		protected internal virtual int GetRulerIndexBeforeValue(double value) {
			return Calculator.GetRulerIndexBeforeValue(value, RangeControl.Client, Ruler);
		}
		RangeControlViewInfoCalc calculator;
		public RangeControlViewInfoCalc Calculator {
			get {
				if(calculator == null)
					calculator = CreateCalculator();
				return calculator;
			}
		}
		RangeControlViewInfoCalc CreateCalculator() {
			return new RangeControlViewInfoCalc(Bounds, Bounds, Orientation.Horizontal, 1.0, true, LookAndFeel);
		}
	}
	public enum RangeControlParts { ScrollArea, Range, MinRangeThumb, MaxRangeThumb, MinRangeFlag, MaxRangeFlag, MinRangeFlagLine, MaxRangeFlagLine, MinFlagText, MaxFlagText, Ruler, LeftOutOfRangeArea, RightOutOfRangeArea, ScrollBarArea, ScrollBarThumb, RangeIndicatorBounds, SelectionBounds, LeftScaleThumbBounds, RightScaleThumbBounds }
	public class RangeControlViewInfoCalc : IDisposable {
		public RangeControlViewInfoCalc(Rectangle bounds, Rectangle clientRect, Orientation orientation, double visibleScaleFactor, bool isItPrinting, UserLookAndFeel lookAndFeel) {
			Rects = new Dictionary<RangeControlParts, Rectangle>();
			Bounds = bounds;
			ClientRect = clientRect;
			Orientation = orientation;
			VisibleRangeScaleFactor = visibleScaleFactor;
			IsPrinting = isItPrinting;
			LookAndFeel = lookAndFeel;
			TextSizeCache = new Dictionary<string, Size>(500);
		}
		public RangeControlViewInfoCalc(Rectangle bounds, Rectangle clientRect, Orientation orientation, double visibleScaleFactor, bool isItPrinting, UserLookAndFeel lookAndFeel, AppearanceObject app) 
			: this(bounds, clientRect, orientation, visibleScaleFactor, isItPrinting, lookAndFeel){
				PaintAppearance = app;
		}
		protected internal Dictionary<string, Size> TextSizeCache { get; set; }
		public Graphics Graphics { get; set; }
		public Rectangle ScrollBounds { get { return GetRect(RangeControlParts.ScrollArea); } set { SetRect(RangeControlParts.ScrollArea, value); } }
		public Rectangle LeftOutOfRangeBounds { get { return GetRect(RangeControlParts.LeftOutOfRangeArea); } set { SetRect(RangeControlParts.LeftOutOfRangeArea, value); } }
		public Rectangle RightOutOfRangeBounds { get { return GetRect(RangeControlParts.RightOutOfRangeArea); } set { SetRect(RangeControlParts.RightOutOfRangeArea, value); } }
		public Rectangle RulerBounds { get { return GetRect(RangeControlParts.Ruler); } set { SetRect(RangeControlParts.Ruler, value); } }
		public Rectangle RangeBounds { get { return GetRect(RangeControlParts.Range); } set { SetRect(RangeControlParts.Range, value); } }
		public Rectangle MinRangeThumbBounds { get { return GetRect(RangeControlParts.MinRangeThumb); } set { SetRect(RangeControlParts.MinRangeThumb, value); } }
		public Rectangle MaxRangeThumbBounds { get { return GetRect(RangeControlParts.MaxRangeThumb); } set { SetRect(RangeControlParts.MaxRangeThumb, value); } }
		public Rectangle MinRangeFlagBounds { get { return GetRect(RangeControlParts.MinRangeFlag); } set { SetRect(RangeControlParts.MinRangeFlag, value); } }
		public Rectangle MaxRangeFlagBounds { get { return GetRect(RangeControlParts.MaxRangeFlag); } set { SetRect(RangeControlParts.MaxRangeFlag, value); } }
		public Rectangle ScrollBarAreaBounds { get { return GetRect(RangeControlParts.ScrollBarArea); } set { SetRect(RangeControlParts.ScrollBarArea, value); } }
		public Rectangle ScrollBarThumbBounds { get { return GetRect(RangeControlParts.ScrollBarThumb); } set { SetRect(RangeControlParts.ScrollBarThumb, value); } }
		public Rectangle RangeIndicatorBounds { get { return GetRect(RangeControlParts.RangeIndicatorBounds); } set { SetRect(RangeControlParts.RangeIndicatorBounds, value); } }
		public Rectangle SelectionBounds { get { return GetRect(RangeControlParts.SelectionBounds); } set { SetRect(RangeControlParts.SelectionBounds, value); } }
		public Rectangle LeftScaleThumbBounds { get { return GetRect(RangeControlParts.LeftScaleThumbBounds); } set { SetRect(RangeControlParts.LeftScaleThumbBounds, value); } }
		public Rectangle RightScaleThumbBounds { get { return GetRect(RangeControlParts.RightScaleThumbBounds); } set { SetRect(RangeControlParts.RightScaleThumbBounds, value); } }
		public Rectangle MinRangeFlagLineBounds { get { return GetRect(RangeControlParts.MinRangeFlagLine); } set { SetRect(RangeControlParts.MinRangeFlagLine, value); } }
		public Rectangle MaxRangeFlagLineBounds { get { return GetRect(RangeControlParts.MaxRangeFlagLine); } set { SetRect(RangeControlParts.MaxRangeFlagLine, value); } }
		public Rectangle MinFlagTextBounds { get { return GetRect(RangeControlParts.MinFlagText); } set { SetRect(RangeControlParts.MinFlagText, value); } }
		public Rectangle MaxFlagTextBounds { get { return GetRect(RangeControlParts.MaxFlagText); } set { SetRect(RangeControlParts.MaxFlagText, value); } }
		public Rectangle Bounds { get; set; }
		public Rectangle ClientRect { get; set; }
		public UserLookAndFeel LookAndFeel { get; set; }
		public Color LabelColor {
			get {
				if(PaintAppearance != null && PaintAppearance.ForeColor != Color.Empty && (LookAndFeel.Style == LookAndFeelStyle.Flat || LookAndFeel.Style == LookAndFeelStyle.UltraFlat))
					return PaintAppearance.ForeColor;
				if(IsPrinting)
					return Color.FromArgb(255, 150, 150, 150);
				return GetColorProperty(EditorsSkins.OptRangeControlLabelColor); 
			} 
		}
		public Color BorderColor { 
			get { 
				if(IsPrinting)
				return Color.FromArgb(255, 185, 185, 185);
			return GetColorProperty(EditorsSkins.OptRangeControlBorderColor); 
			} 
		}
		public Color RulerColor { 
			get {
				if(IsPrinting)
					return Color.FromArgb(20, 0, 0, 0);
				return GetColorProperty(EditorsSkins.OptRangeControlRuleColor); } }
		public Color BackColor {
			get {
				if(IsPrinting)
					return Color.FromArgb(255, 255, 255, 255);
				return GetColorProperty(EditorsSkins.OptRangeControlBackColor);
			}
		}
		public int RuleTextBottomIndent { get { return 2; } }
		public int RuleTextTopIndent { get { return 2; } }
		public int MinIndentBetweenTicks { get { return 10; } }
		public double VisibleRangeScaleFactor { get; set; }
		public double VisibleRangeStartPosition { get; set; }
		public double VisibleRangeWidth { get; set; }
		public Orientation Orientation { get; set; }
		bool IsPrinting { get; set; }
		AppearanceObject PaintAppearance { get; set; }
		public bool AllowScrollBar { get; set; }
		public int ScrollBarHeight { get { return GetIntProperty(EditorsSkins.OptRangeControlScrollAreaHeight); } }
		public bool IsRightToLeft { get; set; }
		public int GetIntProperty(string propertyName) {
			SkinElementInfo info = GetBorderInfo();
			if(info.Element != null)
				return info.Element.Properties.GetInteger(propertyName);
			return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel).Properties.GetInteger(propertyName);
		}
		public SkinElementInfo GetBorderInfo() {
			SkinElement elem = EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinRangeControlBorder];
			return new SkinElementInfo(elem, new Rectangle(new Point(0, 0), Bounds.Size));
		}
		public Color GetColorProperty(string propertyName) {
			SkinElementInfo info = GetBorderInfo();
			if(info.Element != null)
				return info.Element.Properties.GetColor(propertyName);
			return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel).Properties.GetColor(propertyName);
		}
		protected Dictionary<RangeControlParts, Rectangle> Rects { get; private set; }
		public Rectangle GetRect(RangeControlParts part) {
			if(Rects.ContainsKey(part))
				return Rects[part];
			return Rectangle.Empty;
		}
		public void SetRect(RangeControlParts part, Rectangle value) {
			Rects[part] = value;
		}
		public void RotateRects() {
			foreach(RangeControlParts part in Enum.GetValues(typeof(RangeControlParts))) {
				SetRect(part, Horizontal2VerticalCore(GetRect(part)));
			}
		}
		public void MirrorRectsHorizontal() {
			foreach(RangeControlParts part in Enum.GetValues(typeof(RangeControlParts))) {
				SetRect(part, HorizontalRotateCore(GetRect(part)));
			}
		}
		public Rectangle HorizontalRotateCore(Rectangle rect) {
			return new Rectangle(Bounds.Right-rect.X - rect.Width,rect.Y, rect.Width, rect.Height);
		}
		public Rectangle Horizontal2VerticalCore(Rectangle rect) {
			return new Rectangle(Bounds.Right - rect.Y - rect.Height, rect.X, rect.Height, rect.Width);
		}
		public Rectangle GetRulerBounds(Rectangle bounds, AppearanceObject PaintAppearance, bool isCustomRuler, bool allowScrollBar) {
			int height = !isCustomRuler ? PaintAppearance.CalcDefaultTextSize(Graphics).Height + RuleTextBottomIndent + RuleTextTopIndent : 0;
			if(allowScrollBar) return new Rectangle(bounds.X, ScrollBarAreaBounds.Y - height, bounds.Width, height);
			else return new Rectangle(bounds.X, bounds.Bottom - height, bounds.Width, height);
		}
		public void Reset() {
			TextSizeCache.Clear();
		}
		public Rectangle CalcScrollBounds(Rectangle bounds) {
			return new Rectangle(bounds.X, bounds.Y, bounds.Width, RulerBounds.Y - bounds.Y);
		}
		public Rectangle CalcScrollBarAreaBounds(Rectangle bounds) {
			Rectangle barBounds = bounds;
			barBounds.Y = barBounds.Bottom - ScrollBarHeight;
			barBounds.Height = ScrollBarHeight;
			return barBounds;
		}
		public Rectangle CalcRightOutOfRangeAreaBounds(Rectangle bounds, Rectangle rangeBounds) {
			return new Rectangle(rangeBounds.Right, rangeBounds.Y, bounds.Right - rangeBounds.Right, rangeBounds.Height);
		}
		public Rectangle CalcLeftOutOfRangeAreaBounds(Rectangle bounds, Rectangle rangeBounds) {
			return new Rectangle(bounds.X, rangeBounds.Y, rangeBounds.X - bounds.X, rangeBounds.Height);
		}
		public Rectangle CalcMinRangeFlagBounds(Graphics g, string text, AppearanceObject paintAppearance) {
			return CalcMinRangeFlagBounds(g, text, paintAppearance, 1, 10);
		}
		public Rectangle CalcMinRangeFlagBounds(Graphics g, string text, AppearanceObject paintAppearance, float touchScale) {
			return CalcMinRangeFlagBounds(g, text, paintAppearance, touchScale, 0);
		}
		protected internal Rectangle CalcMinRangeFlagBounds(Graphics g, string text, AppearanceObject paintAppearance, float touchScale, int offset) {
			Size textSize = CalcTextSize(g, text, paintAppearance);
			Size flagSize = new Size(Convert.ToInt16(textSize.Width * touchScale), Convert.ToInt16(textSize.Height * touchScale));
			flagSize.Width += offset;
			Rectangle res = Rectangle.Empty;
			if(LeftOutOfRangeBounds.Right - flagSize.Width > Bounds.X)
				res = new Rectangle(LeftOutOfRangeBounds.Right - flagSize.Width, LeftOutOfRangeBounds.Y, flagSize.Width, flagSize.Height);
			else res = new Rectangle(LeftOutOfRangeBounds.Right, LeftOutOfRangeBounds.Y, flagSize.Width, flagSize.Height);
			MinFlagTextBounds = CalcFlagTextBounds(textSize, res);
			return res;
		}
		public Rectangle CalcMaxRangeFlagBounds(Graphics g, string text, AppearanceObject paintAppearance) {
			return CalcMaxRangeFlagBounds(g, text, paintAppearance, 1, 10);
		}
		public Rectangle CalcMaxRangeFlagBounds(Graphics g, string text, AppearanceObject paintAppearance, float touchScale) {
			return CalcMaxRangeFlagBounds(g, text, paintAppearance, touchScale, 0);
		}
		protected Rectangle CalcMaxRangeFlagBounds(Graphics g, string text, AppearanceObject paintAppearance, float touchScale, int offset) {
			Size textSize = CalcTextSize(g, text, paintAppearance);
			Size flagSize = new Size(Convert.ToInt16(textSize.Width * touchScale), Convert.ToInt16(textSize.Height * touchScale));
			flagSize.Width += offset;
			Rectangle res = Rectangle.Empty;
			if(RightOutOfRangeBounds.X + flagSize.Width < Bounds.Width)
				res = new Rectangle(RightOutOfRangeBounds.X, RightOutOfRangeBounds.Y, flagSize.Width, flagSize.Height);
			else res = new Rectangle(RightOutOfRangeBounds.X - flagSize.Width, RightOutOfRangeBounds.Y, flagSize.Width, flagSize.Height);
			MaxFlagTextBounds = CalcFlagTextBounds(textSize, res);
			return res;
		}
		Rectangle CalcFlagTextBounds(Size textSize, Rectangle flagBounds) {
			Rectangle res = flagBounds;
			res.Width = textSize.Width;
			res.Height = textSize.Height;
			res.X = flagBounds.Right - (flagBounds.Width + textSize.Width)/2;
			res.Y = flagBounds.Bottom - (flagBounds.Height + textSize.Height) / 2;
			return res;
		}
		public int GetRulerIndexBeforeValue(double value, IRangeControlClient Client, List<object> Ruler) {
			if(Client == null)
				return 0;
			if(Ruler == null) {
				return (int)(value / Client.NormalizedRulerDelta);
			}
			if(Ruler.Count == 0)
				return 0;
			int start = 0, end = Ruler.Count - 1;
			double startPos, endPos, middlePos;
			startPos = Client.GetNormalizedValue(Ruler[start]);
			endPos = Client.GetNormalizedValue(Ruler[end]);
			if(value < startPos)
				return start;
			if(value > endPos)
				return end;
			for(int i = 0; i < 1000; i++) {
				int middle = start + (end - start) / 2;
				startPos = Client.GetNormalizedValue(Ruler[start]);
				endPos = Client.GetNormalizedValue(Ruler[end]);
				middlePos = Client.GetNormalizedValue(Ruler[middle]);
				if(startPos <= value && middlePos >= value) {
					if(middle - start == 1)
						return start;
					end = middle;
				}
				else {
					if(end - middle == 1)
						return middle;
					start = middle;
				}
			}
			return 0;
		}
		public virtual Size CalcTextSize(Graphics g, string text, AppearanceObject PaintAppearance) {
			if(TextSizeCache.ContainsKey(text))
				return TextSizeCache[text];
			Size sz = PaintAppearance.CalcTextSize(g, text, 0).ToSize();
			TextSizeCache.Add(text, sz);
			return sz;
		}
		public void Dispose() {
			Graphics.Dispose();
		}
		public virtual int Delta2Pixel(double value) {
			if(Orientation == Orientation.Horizontal)
				return (int)(value * ScrollBounds.Width * VisibleRangeScaleFactor);
			return (int)(value * ScrollBounds.Height * VisibleRangeScaleFactor);
		}
		public virtual double Pixel2Delta(int pixel) {
			if(Orientation == Orientation.Horizontal)
				return (double)pixel / VisibleRangeScaleFactor / ScrollBounds.Width;
			return (double)pixel / VisibleRangeScaleFactor / ScrollBounds.Height;
		}
		public virtual int Value2Pixel(double value) {
			if(Orientation == Orientation.Horizontal) {
				if(IsRightToLeft)
					return ScrollBounds.Right - (int)((value - VisibleRangeStartPosition) * ScrollBounds.Width * VisibleRangeScaleFactor - 0.5);
				return ScrollBounds.X + (int)((value - VisibleRangeStartPosition) * ScrollBounds.Width * VisibleRangeScaleFactor + 0.5);
			}
			return ScrollBounds.Y + (int)((value - VisibleRangeStartPosition) * ScrollBounds.Height * VisibleRangeScaleFactor + 0.5);
		}
		protected virtual void DrawFlag(GraphicsInfoArgs info, Rectangle bounds, string text, int position, AppearanceObject paintAppearance, Rectangle textBounds) {
			info.Graphics.DrawLine(info.Cache.GetPen(BorderColor), position, RangeBounds.Y, position, RangeBounds.Bottom);
			info.Graphics.FillRectangle(info.Cache.GetSolidBrush(BorderColor), bounds);
			if(IsPrinting)
				info.Graphics.DrawString(text, paintAppearance.Font, info.Cache.GetSolidBrush(BackColor), textBounds.Location);
			else {
				Rectangle textRect = bounds;
				if(Orientation == System.Windows.Forms.Orientation.Horizontal) {
					textRect.X += 2;
					paintAppearance.DrawString(info.Cache, text, textBounds, info.Cache.GetSolidBrush(BackColor));
				}
				else {
					textRect.Y += 2;
					int angle = IsRightToLeft ? 270 : 90;
					paintAppearance.DrawVString(info.Cache, text, paintAppearance.Font, info.Cache.GetSolidBrush(BackColor), textBounds, paintAppearance.GetStringFormat(), angle);
				}
			}
		}
		public virtual void DrawFlags(GraphicsInfoArgs info, IRangeControlClient client, AppearanceObject paintAppearance, object minValue, object maxValue) {
			string min = client.ValueToString((double)minValue);
			string max = client.ValueToString((double)maxValue);
			DrawFlag(info, MinRangeFlagBounds, min, LeftOutOfRangeBounds.Right, paintAppearance, MinFlagTextBounds);
			DrawFlag(info, MaxRangeFlagBounds, max, RightOutOfRangeBounds.X, paintAppearance, MaxFlagTextBounds);
		}
		public virtual void DrawRuler(GraphicsInfoArgs info, AppearanceObject paintAppearance, IRangeControlClient client, RangeControlPaintEventArgs args, List<object> ruler) {
			if(client == null || client.DrawRuler(args))
				return;
			Pen rp = info.Cache.GetPen(RulerColor);
			Pen bp = info.Cache.GetPen(BorderColor);
			if(!IsPrinting)
				if(Orientation == System.Windows.Forms.Orientation.Vertical) {
					if(IsRightToLeft)
						info.Graphics.DrawLine(bp, new Point(RulerBounds.X, ClientRect.Y), new Point(RulerBounds.X, ClientRect.Bottom));
					else info.Graphics.DrawLine(bp, new Point(RulerBounds.Right, ClientRect.Y), new Point(RulerBounds.Right, ClientRect.Bottom));
				}
				else info.Graphics.DrawLine(bp, new Point(ClientRect.X, RulerBounds.Y), new Point(ClientRect.Right, RulerBounds.Y));
			int start = Math.Max(0, GetRulerFirstVisibleValueIndex(info, client, ruler) - 2);
			int end = Math.Min(GetRulesCount(client, ruler), GetRulerLastVisibleValueIndex(info, client, ruler) + 2);
			int prevXCoor = -10000;
			Rectangle prevLabelRect = new Rectangle();
			double rulerDelta = client.NormalizedRulerDelta;
			int step = 1;
			int rulerDeltaPixels = Delta2Pixel(rulerDelta);
			if(rulerDeltaPixels < MinIndentBetweenTicks) {
				step = (int)(Pixel2Delta(MinIndentBetweenTicks) / rulerDelta);
				step = Math.Max(1, step);
			}
			if(ruler != null)
				end = Math.Min(ruler.Count, end);
			for(int i = start; i < end; i += step) {
				double pos = 0.0;
				if(ruler == null) 
					pos = client.NormalizedRulerDelta * i;
				else
					pos = client.GetNormalizedValue(ruler[i]);
				int xCoor = Value2Pixel(pos);
				if(Orientation == Orientation.Horizontal)
					info.Graphics.DrawLine(rp, xCoor, ScrollBounds.Y, xCoor, ScrollBounds.Bottom - 1);
				else
					info.Graphics.DrawLine(rp, ScrollBounds.X, xCoor, ScrollBounds.Right - 1, xCoor);
				prevXCoor = xCoor;
				string labelText = client.RulerToString(i);
				Rectangle labelRect = CalcLabelRect(info, i, labelText, xCoor, paintAppearance, client, ruler);
				Rectangle labelRectWithIndent = labelRect;
				labelRectWithIndent.Inflate(10, 10);
				if(prevLabelRect.IntersectsWith(labelRectWithIndent))
					continue;
				if(!IsPrinting)
					if(Orientation == Orientation.Horizontal)
						paintAppearance.DrawString(info.Cache, labelText, labelRect, info.Cache.GetSolidBrush(LabelColor));
					else {
						int angle = IsRightToLeft ? 270 : 90;
						paintAppearance.DrawVString(info.Cache, labelText, paintAppearance.Font, info.Cache.GetSolidBrush(LabelColor), labelRect, paintAppearance.GetStringFormat(), angle);
					}
				else {
					info.Graphics.FillRectangle(info.Cache.GetSolidBrush(BackColor), new Rectangle(labelRect.X, labelRect.Y, labelRect.Width + 2, labelRect.Height));
					info.Graphics.DrawString(labelText, paintAppearance.Font, info.Cache.GetSolidBrush(LabelColor), labelRect.Location);
				}
				prevLabelRect = labelRect;
			}
		}
		protected virtual Rectangle CalcLabelRect(GraphicsInfoArgs info, int ruleIndex, string labelText, int xCoor, AppearanceObject paintAppearance, IRangeControlClient client, List<object> ruler) {
			Rectangle labelRect = Rectangle.Empty;
			if(Orientation == Orientation.Horizontal) {
				labelRect = new Rectangle(new Point(xCoor, RulerBounds.Y + RuleTextTopIndent), CalcTextSize(info.Graphics, labelText, paintAppearance));
				if((ruleIndex > 0 && ruleIndex < GetRulesCount(client, ruler) - 1) || (labelRect.X - 1 > labelRect.Width / 2 && labelRect.X + labelRect.Width / 2 < ClientRect.Width))
					labelRect.X -= labelRect.Width / 2;
				else if((ruleIndex == GetRulesCount(client, ruler) - 1) != IsRightToLeft)
					labelRect.X = ClientRect.Width - labelRect.Width;
				else
					labelRect.X = 2;
			}
			else {
				Size sz = CalcTextSize(info.Graphics, labelText, paintAppearance);
				labelRect = new Rectangle(new Point(RulerBounds.X + RuleTextTopIndent, xCoor), new Size(sz.Height, sz.Width));
				if(ruleIndex > 0 && ruleIndex < GetRulesCount(client, ruler) - 1)
					labelRect.Y -= labelRect.Height / 2;
				else if(ruleIndex == GetRulesCount(client, ruler) - 1)
					labelRect.Y -= labelRect.Height + 3;
				else
					labelRect.Y += 3;
			}
			return labelRect;
		}
		private int GetRulesCount(IRangeControlClient client, List<object> ruler) {
			if(ruler != null)
				return ruler.Count;
			return (int)(1.0 / client.NormalizedRulerDelta) + 1;
		}
		protected int GetRulerFirstVisibleValueIndex(GraphicsInfoArgs info, IRangeControlClient client, List<object> ruler) {
			return GetRulerIndexBeforeValue(VisibleRangeStartPosition, client, ruler);
		}
		protected int GetRulerLastVisibleValueIndex(GraphicsInfoArgs info, IRangeControlClient client, List<object> ruler) {
			return GetRulerIndexBeforeValue(VisibleRangeStartPosition + VisibleRangeWidth, client, ruler);
		}
	}
	public abstract class PrintingRangeBitMapContainer : IDisposable {
		public static Bitmap Draw(PrintingRangeControl rangeControl, Size size) {
			using(PrintingRangeBitMapContainer container = CreateInstance(rangeControl, size)) {
				try {
					container.DrawRangeControl(true);
					return container.GetBitmap();
				}
				catch {
					container.DisposeBitmap();
					return null;
				}
			}
		}
		public static PrintingRangeBitMapContainer CreateInstance(PrintingRangeControl rangeControl, Size size) {
			return new RangeBitMapContainer(rangeControl, size);
		}
		public PrintingRangeBitMapContainer(PrintingRangeControl rangeControl, Size size) {
			this.rangeControl = rangeControl;
			this.size = size;
			this.bitmap = new Bitmap(size.Width, size.Height);
		}
		PrintingRangeControl rangeControl;
		Size size;
		Bitmap bitmap;
		Graphics graphics;
		public Size Size { get { return size; } }
		void DisposeGraphics() {
			if(this.graphics != null) {
				this.graphics.Dispose();
				this.graphics = null;
			}
		}
		void Finish() {
			DisposeGraphics();
		}
		protected void DisposeBitmap() {
			if(this.bitmap != null) {
				this.bitmap.Dispose();
				this.bitmap = null;
			}
		}
		protected void Start() {
			if(this.bitmap != null) {
				Finish();
				this.graphics = GetBitmapGraphics();
			}
		}
		public void DrawRangeControl(bool lockDrawingHelper) {
			using(Graphics gr = GetBitmapGraphics())
				rangeControl.DrawContent(gr, new Rectangle(Point.Empty, size), true);
		}
		protected abstract Graphics GetBitmapGraphics();
		protected abstract Bitmap GetBitmap();
		protected Graphics CreateGraphicsFromBitmap() {
			Graphics res = Graphics.FromImage(this.bitmap);
			res.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
			return res;
		}
		protected Bitmap PrepareBitmap() {
			if(this.bitmap == null)
				return null;
			Finish();
			return this.bitmap;
		}
		public virtual void Dispose() {
			Finish();
			GC.SuppressFinalize(this);
		}
	}
	public class RangeBitMapContainer : PrintingRangeBitMapContainer {
		public RangeBitMapContainer(PrintingRangeControl rangeControl, Size size)
			: base(rangeControl, size) {
			Start();
		}
		protected override Graphics GetBitmapGraphics() {
			return CreateGraphicsFromBitmap();
		}
		protected override Bitmap GetBitmap() {
			return PrepareBitmap();
		}
	}
}
