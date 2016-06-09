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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.XtraReports.UI;
using System.Collections;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraReports.Design.Ruler {
	public enum RulerState { None, LeftMarginChanged, RightMarginChanged, SectionChanged };
	public class RulerUnit {
		#region static
		public static RulerUnit CreateRulerUnit(ReportUnit reportUnit, ZoomService zoomService) {
			if(reportUnit == ReportUnit.HundredthsOfAnInch) {
				const int pointsPerUnit = 8;
				float quantizationStep = CalculateQuantization(pointsPerUnit, GraphicsDpi.HundredthsOfAnInch / 100);
				return new RulerUnit(zoomService.ScaleValueF(quantizationStep), pointsPerUnit / 2, pointsPerUnit);
			}
			if(reportUnit == ReportUnit.TenthsOfAMillimeter) {
				const int pointsPerUnit = 2;
				float quantizationStep = CalculateQuantization(pointsPerUnit, GraphicsDpi.TenthsOfAMillimeter / 100);
				return new RulerUnit(zoomService.ScaleValueF(quantizationStep), 0, pointsPerUnit);
			}
			if(reportUnit == ReportUnit.Pixels) {
				const int pointsPerUnit = 8;
				float quantizationStep = CalculateQuantization(pointsPerUnit, GraphicsDpi.DeviceIndependentPixel / 100);
				return new RulerUnit(zoomService.ScaleValueF(quantizationStep), pointsPerUnit / 2, pointsPerUnit);
			}
			throw new NotSupportedException();
		}
		static float CalculateQuantization(int pointsPerUnit, float dpi) {
			return XRConvert.Convert(1f, dpi, GraphicsDpi.Pixel) / pointsPerUnit;
		}
		#endregion
		public float QuantizationStep {
			get { return quantizationStep; }
		}
		public int PointsPerHalfUnit {
			get { return pointsPerHalfUnit; }
		}
		public int PointsPerUnit {
			get { return pointsPerUnit; }
		}
		float quantizationStep;
		int pointsPerHalfUnit;
		int pointsPerUnit;
		public RulerUnit(float quantizationStep, int pointsPerHalfUnit, int pointsPerUnit) {
			this.quantizationStep = quantizationStep;
			this.pointsPerHalfUnit = pointsPerHalfUnit;
			this.pointsPerUnit = pointsPerUnit;
		}
	}
	[ToolboxItem(false)]
	public abstract class RulerBase : Control, ISupportInitialize {
		protected static int DrawingFontHeight {
			get {
				int height = RulerSection.fFont.Height;
				if(height % 2 != 0)
					height++;
				return height;
			}
		}
		IServiceProvider servProvider;
		protected RulerSectionPaintHelper paintHelper;
		protected int clientLength;
		protected enum OffsetMode { Horz, Vert, All };
		public const int PixelFault = 3;
		protected MouseEventHandler mouseDownHandler;
		protected Rectangle fSelectionRect = Rectangle.Empty;
		protected Point fStartSelectionPos = Point.Empty;
		protected Rectangle[] fShadowRects;
		protected RulerViewInfo fViewInfo;
		protected int fShadowLevel;
		SectionCollection sections;
		RulerUnit rulerUnit;
		bool paintInProcess;
		int offset;
		public int Offset {
			get { return offset; }
			set { offset = value; }
		}
		public SectionCollection Sections {
			get { return sections; }
		}
		public RulerUnit RulerUnit {
			get { return rulerUnit; }
			set { rulerUnit = value; Invalidate(); }
		}
		protected virtual Cursor SelectionCursor {
			get { return Cursors.Default; }
		}
		public abstract Rectangle ClientBounds { get; }
		protected internal Rectangle SelectionRect {
			get { return fSelectionRect; }
		}
		protected Point StartSelectionPos {
			get { return fStartSelectionPos; }
		}
		protected internal Rectangle[] ShadowRects {
			get { return fShadowRects ?? new Rectangle[0]; }
		}
		protected internal int ShadowLevel {
			get { return fShadowLevel; }
		}
		public Point ViewOffset {
			get { return fViewInfo.ViewOffset; }
			set { SetViewOffset(value); }
		}
		public virtual Size RulerSize {
			get { return new Size(20, 20); }
		}
		protected bool IsDragging {
			get { return Capture; }
		}
		public void SetUnit(ReportUnit repUnit) {
			ZoomService zoomService = ZoomService.GetInstance(servProvider);
			RulerUnit = RulerUnit.CreateRulerUnit(repUnit, zoomService);
		}
		#region Events
		private static readonly object BeginDragEvent = new object();
		private static readonly object EndDragEvent = new object();
		private static readonly object SelectionChangedEvent = new object();
		public event EventHandler BeginDrag {
			add { Events.AddHandler(BeginDragEvent, value); }
			remove { Events.RemoveHandler(BeginDragEvent, value); }
		}
		public event EventHandler EndDrag {
			add { Events.AddHandler(EndDragEvent, value); }
			remove { Events.RemoveHandler(EndDragEvent, value); }
		}
		public event BoundsEventHandler SelectionChanged {
			add { Events.AddHandler(SelectionChangedEvent, value); }
			remove { Events.RemoveHandler(SelectionChangedEvent, value); }
		}
		protected void OnBeginDrag(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[BeginDragEvent];
			if(handler != null)
				handler(this, e);
		}
		protected void OnEndDrag(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[EndDragEvent];
			if(handler != null)
				handler(this, e);
		}
		protected void OnSelectionChanged(BoundsEventArgs e) {
			BoundsEventHandler handler = (BoundsEventHandler)this.Events[SelectionChangedEvent];
			if(handler != null)
				handler(this, e);
		}
		#endregion
		protected RulerBase(IServiceProvider servProvider) {
			this.servProvider = servProvider;
			rulerUnit = RulerUnit.CreateRulerUnit(ReportUnit.HundredthsOfAnInch, ZoomService.NullZoomService);
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlConstants.DoubleBuffer, true);
			sections = new SectionCollection(this);
			mouseDownHandler = OnBaseMouseDown;
			fViewInfo = new RulerViewInfo(this);
			UserLookAndFeel lf = DesignLookAndFeelHelper.GetLookAndFeel(servProvider);
			UpdatePaintHelper(lf);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				fViewInfo = null;
				sections = null;
			}
			base.Dispose(disposing);
		}
		public void UpdatePaintHelper(UserLookAndFeel lookAndFeel) {
			paintHelper = ReportPaintStyles.GetPaintStyle(lookAndFeel).CreateRulerSectionPaintHelper(lookAndFeel);
		}
		public virtual void BeginInit() {
		}
		public virtual void EndInit() {
		}
		protected UserLookAndFeel GetLookAndFeel() {
			return DesignLookAndFeelHelper.GetLookAndFeel(servProvider);
		}
		public virtual void SetClientLength(int length) {
			clientLength = length;
		}
		protected internal Rectangle OffsetBounds(Rectangle r) {
			r.Offset(fViewInfo.ViewOffset);
			return r;
		}
		protected Point OffsetPoint(Point pt, OffsetMode type) {
			int xFactor = (type == OffsetMode.Vert) ? 0 : 1;
			int yFactor = (type == OffsetMode.Horz) ? 0 : 1;
			pt.Offset(-ViewOffset.X * xFactor, -ViewOffset.Y * yFactor);
			return pt;
		}
		protected Point PointToClientBounds(Point pt) {
			pt = PointToClient(pt);
			pt.Offset(-fViewInfo.ViewOffset.X, -fViewInfo.ViewOffset.Y);
			return pt;
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == Win32.WM_CAPTURECHANGED) {
				CancelDrag();
			}
			base.WndProc(ref m);
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(paintInProcess)
				return;
			IDesignerHost host = servProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null || host.RootComponent == null) return;
			UpdateViewInfoProperties(e);
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				try {
					paintInProcess = true;
					fViewInfo.Cache = cache;
					DrawBackground();
					TransformGraphics(e.Graphics);
					DrawSections(e.ClipRectangle);
				} finally {
					fViewInfo.Cache = null;
					paintInProcess = false;
				}
			}
		}
		protected virtual void DrawBackground() {
			paintHelper.DrawBackground(fViewInfo);
		}
		protected virtual void DrawSections(Rectangle clipRect) {
			for(int i = 0; i < Sections.Count; i++) {
				Rectangle bounds = Sections[i].Bounds;
				if(clipRect.IntersectsWith(OffsetBounds(bounds))) {
					fViewInfo.SectionBounds = bounds;
					Sections[i].DrawContent(fViewInfo, paintHelper);
				}
			}
		}
		protected virtual void UpdateViewInfoProperties(PaintEventArgs e) {
			fViewInfo.Bounds = Bounds;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(mouseDownHandler != null)
				mouseDownHandler(this, e);
		}
		protected void OnBaseMouseDown(object sender, MouseEventArgs e) {
			if(!e.Button.IsLeft()) {
				Capture = false;
				return;
			}
			if(XRControlDesignerBase.CursorEquals(new Cursor[] { Cursors.HSplit, Cursors.VSplit }))
				OnBeginDrag(EventArgs.Empty);
			else
				if(XRControlDesignerBase.CursorEquals(SelectionCursor)) {
					fStartSelectionPos = OffsetPoint(new Point(e.X, e.Y), OffsetMode.All);
					fSelectionRect.Location = fStartSelectionPos;
				}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Cursor = GetCursor();
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(IsDragging) {
				CommitSelection();
				Capture = false;
			}
		}
		protected internal virtual void DrawShadows(Rectangle[] shadowRects, int level) {
			fShadowLevel = level;
			int count = shadowRects != null ? shadowRects.Length : 0;
			fShadowRects = new Rectangle[count];
		}
		protected internal virtual void HideShadows() {
			fShadowRects = new Rectangle[0];
			fShadowLevel = 0;
			Invalidate();
		}
		protected virtual Cursor GetCursor() {
			return ClientBounds.Contains(PointToClientBounds(MousePosition)) ?
				SelectionCursor : Cursors.Default;
		}
		void CancelDrag() {
			OnEndDrag(EventArgs.Empty);
			fSelectionRect = Rectangle.Empty;
			Invalidate();
		}
		void CommitSelection() {
			if(fSelectionRect != Rectangle.Empty) {
				fSelectionRect = OffsetBounds(fSelectionRect);
				fSelectionRect = RectangleToScreen(fSelectionRect);
				OnSelectionChanged(new BoundsEventArgs(new RectangleF[] { fSelectionRect }));
			}
		}
		void TransformGraphics(Graphics gr) {
			Matrix transformMatrix = new Matrix();
			try {
				transformMatrix.Translate(ViewOffset.X, ViewOffset.Y);
				gr.MultiplyTransform(transformMatrix, MatrixOrder.Append);
			} finally {
				transformMatrix.Dispose();
			}
		}
		void SetViewOffset(Point offset) {
			if(!ViewOffset.Equals(offset)) {
				fViewInfo.ViewOffset = offset;
				Invalidate();
				Update();
			}
		}
	}
	public class HRuler : RulerBase {
		int leftMargin;
		int rightMargin;
		RulerState state = RulerState.None;
		Rectangle rightMarginBounds;
		Rectangle leftMarginBounds;
		public RulerState State {
			get { return state; }
			set { state = value; }
		}
		public int RightMargin {
			get { return rightMargin; }
			set { rightMargin = SetHMargin(value); }
		}
		public int LeftMargin {
			get { return leftMargin; }
			set { leftMargin = SetHMargin(value); }
		}
		public override Rectangle ClientBounds {
			get {
				int height = DrawingFontHeight + paintHelper.RulerSectionMargins.Top + paintHelper.RulerSectionMargins.Bottom;
				height = (int)NativeMethods.GetValidValue(0, Height, height);
				System.Diagnostics.Debug.Assert(0 == Left);
				return new Rectangle(Offset, paintHelper.ClientMargins.Top, clientLength, height);
			}
		}
		public override Size RulerSize {
			get {
				return new Size(ClientBounds.Width,
					ClientBounds.Height + paintHelper.ClientMargins.Bottom + paintHelper.ClientMargins.Top);
			}
		}
		public Rectangle RightMarginBounds {
			get {
				CalculateBounds();
				return rightMarginBounds;
			}
		}
		public Rectangle LeftMarginBounds {
			get {
				CalculateBounds();
				return leftMarginBounds;
			}
		}
		protected override Cursor SelectionCursor {
			get { return XRCursors.DownArrow; }
		}
		public HRuler(IDesignerHost host)
			: base(host) {
			Sections.Add(new HRulerSection(ClientBounds.Width));
			fViewInfo.RulerOrientation = RulerOrientation.Horizontal;
		}
		public override void EndInit() {
			SetClientLength(Width);
		}
		private void CalculateBounds() {
			leftMarginBounds = ClientBounds;
			leftMarginBounds.Width = leftMargin;
			rightMarginBounds = ClientBounds;
			rightMarginBounds.X = ClientBounds.Right - rightMargin;
			rightMarginBounds.Width = rightMargin;
		}
		public override void SetClientLength(int length) {
			base.SetClientLength(length);
			Sections[0].SetLength(length);
			Sections[0].UpdateBounds(ClientBounds.Location);
			ResetMarginBounds();
		}
		public void ResizeSelectedMargin(int margin, Rectangle bounds) {
			Action2<int, Rectangle> mouseMarginChange = GetMouseMarginChange();
			if(mouseMarginChange != null)
				mouseMarginChange(margin, bounds);
		}
		Action2<int, Rectangle> GetMouseMarginChange() {
			if(state == RulerState.LeftMarginChanged)
				return MouseLeftMarginChange;
			if(state == RulerState.RightMarginChanged)
				return MouseRightMarginChange;
			return null;
		}
		protected override Cursor GetCursor() {
			if(!IsDragging)
				state = GetState(PointToClient(MousePosition));
			return state == RulerState.LeftMarginChanged ? Cursors.VSplit :
				state == RulerState.RightMarginChanged ? Cursors.VSplit :
				base.GetCursor();
		}
		internal RulerState GetState(Point pt) {
			pt = OffsetPoint(pt, OffsetMode.Horz);
			return (Math.Abs(LeftMarginBounds.Right - pt.X) < RulerBase.PixelFault)
					? RulerState.LeftMarginChanged
					: ((Math.Abs(RightMarginBounds.Left - pt.X) < RulerBase.PixelFault)
						? RulerState.RightMarginChanged
						: RulerState.None);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!SelectionRect.IsEmpty) {
				Point pt = OffsetPoint(new Point(e.X, e.Y), OffsetMode.Horz);
				int x = Math.Min(pt.X, StartSelectionPos.X);
				int width = Math.Abs(pt.X - StartSelectionPos.X);
				fSelectionRect = RectHelper.ValidateRect(new Rectangle(x, ClientBounds.Y, width, ClientBounds.Height));
				Invalidate();
				Update();
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			state = RulerState.None;
		}
		protected internal override void DrawShadows(Rectangle[] shadowRects, int level) {
			base.DrawShadows(shadowRects, level);
			for(int i = 0; i < shadowRects.Length; i++) {
				Rectangle r = RectangleToClient(shadowRects[i]);
				r.Y = ClientBounds.Y;
				r.Height = ClientBounds.Height;
				r.Location = OffsetPoint(r.Location, OffsetMode.Horz);
				fShadowRects[i] = r;
			}
			Invalidate();
		}
		void MouseLeftMarginChange(int leftMargin, Rectangle bounds) {
			this.leftMargin = Math.Min(Math.Max(leftMargin - bounds.Left, 0), ClientBounds.Width - rightMargin);
		}
		void MouseRightMarginChange(int rightMargin, Rectangle bounds) {
			this.rightMargin = Math.Min(Math.Max(bounds.Right - rightMargin, 0), ClientBounds.Width - leftMargin);
		}
		int SetHMargin(int x) {
			int result = (int)NativeMethods.GetValidValue(0, ClientBounds.Width - PixelFault, x);
			ResetMarginBounds();
			Invalidate();
			return result;
		}
		void ResetMarginBounds() {
			rightMarginBounds = Rectangle.Empty;
			leftMarginBounds = Rectangle.Empty;
		}
	}
	public class VRuler : RulerBase {
		public override Rectangle ClientBounds {
			get {
				int width = DrawingFontHeight + paintHelper.RulerSectionMargins.Left + paintHelper.RulerSectionMargins.Right;
				width = (int)NativeMethods.GetValidValue(0, Width, width);
				System.Diagnostics.Debug.Assert(0 == Top);
				return new Rectangle(paintHelper.ClientMargins.Left, Offset, width, clientLength);
			}
		}
		public override Size RulerSize {
			get {
				return new Size(
					ClientBounds.Width + paintHelper.ClientMargins.Left + paintHelper.ClientMargins.Right,
					ClientBounds.Height);
			}
		}
		#region Events
		private static readonly object SliderClickEvent = new object();
		public event RulerSectionEventHandler SliderClick {
			add { Events.AddHandler(SliderClickEvent, value); }
			remove { Events.RemoveHandler(SliderClickEvent, value); }
		}
		protected void OnSliderClick(RulerSectionEventArgs e) {
			RulerSectionEventHandler handler = (RulerSectionEventHandler)this.Events[SliderClickEvent];
			if(handler != null)
				handler(this, e);
		}
		#endregion
		public VRuler(IDesignerHost host)
			: base(host) {
			fViewInfo.RulerOrientation = RulerOrientation.Vertical;
		}
		public override void EndInit() {
			SetClientLength(Height);
		}
		protected override Cursor SelectionCursor {
			get { return XRCursors.RightArrow; }
		}
		protected override Cursor GetCursor() {
			mouseDownHandler = new MouseEventHandler(OnBaseMouseDown);
			if(Sections.Count > 0 && SelectionRect.IsEmpty) {
				Point pt = PointToClientBounds(MousePosition);
				VRulerSection s = GetSectionBySlider(pt);
				if(s != null) {
					if(CanResizeSection(s)) {
						mouseDownHandler = new MouseEventHandler(OnHandGrabMouseDown);
						return Cursors.HSplit;
					} else
						return Cursors.Default;
				}
				RulerState state = Sections[Sections.Count - 1].GetState(pt);
				if(state == RulerState.SectionChanged) {
					return Cursors.HSplit;
				}
			}
			return base.GetCursor();
		}
		internal bool CanResizeSection(VRulerSection section) {
			return section.CanResize && IsPreviousExpanded(section);
		}
		private bool IsPreviousExpanded(RulerSection section) {
			VRulerSection prevSection = this.Sections[section.Index - 1] as VRulerSection;
			return prevSection != null ? prevSection.IsExpanded : false;
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!SelectionRect.IsEmpty) {
				Point pt = OffsetPoint(new Point(e.X, e.Y), OffsetMode.Vert);
				int y = Math.Min(pt.Y, StartSelectionPos.Y);
				int height = Math.Abs(pt.Y - StartSelectionPos.Y);
				fSelectionRect = RectHelper.ValidateRect(new Rectangle(ClientBounds.X, y, ClientBounds.Width, height));
				Invalidate();
				Update();
			}
		}
		protected internal override void DrawShadows(Rectangle[] shadowRects, int level) {
			base.DrawShadows(shadowRects, level);
			for(int i = 0; i < shadowRects.Length; i++) {
				Rectangle r = RectangleToClient(shadowRects[i]);
				r.X = ClientBounds.X;
				r.Width = ClientBounds.Width;
				r.Location = OffsetPoint(r.Location, OffsetMode.Vert);
				fShadowRects[i] = r;
			}
			Invalidate();
		}
		protected void OnHandGrabMouseDown(object sender, MouseEventArgs e) {
			RulerSection section = GetSectionByGrab(MousePosition);
			if(e.Button.IsLeft() && XRControlDesignerBase.CursorEquals(Cursors.HSplit) && section != null) {
				OnSliderClick(new RulerSectionEventArgs(section));
			}
			Capture = false;
		}
		public void SetSectionHeight(RulerSection section, int height) {
			if(section != null) {
				section.SetLength(height);
				UpdateSectionPosition(section.Index);
				SetClientLength(CalcTotalSectionHeight());
				Invalidate();
			}
		}
		public void RecreateSections(IList sectionList) {
			Sections.Clear();
			foreach(VRulerSection s in sectionList) {
				System.Diagnostics.Debug.Assert(s != null);
				Sections.Add(s);
			}
			UpdateSectionPosition(0);
			SetClientLength(CalcTotalSectionHeight());
			Invalidate();
		}
		private void UpdateSectionPosition(int startIndex) {
			Point pos = CalcSectionValidTop(startIndex);
			for(int i = startIndex; i < Sections.Count; i++) {
				RulerSection s = Sections[i];
				if(s != null) {
					s.UpdateBounds(pos);
					pos.Y = s.Bounds.Bottom;
				}
			}
		}
		private int CalcTotalSectionHeight() {
			int totalHeight = 0;
			foreach(RulerSection s in Sections)
				totalHeight += s.Bounds.Height;
			return totalHeight;
		}
		private Point CalcSectionValidTop(int index) {
			Point pos = ClientBounds.Location;
			RulerSection s = Sections[index - 1];
			if(s != null)
				pos.Y = s.Bounds.Bottom;
			return pos;
		}
		private RulerSection GetSectionByGrab(Point screenPos) {
			Point pt = PointToClientBounds(screenPos);
			VRulerSection s = GetSectionBySlider(pt);
			return (s != null) ? Sections[s.Index - 1] : null;
		}
		private VRulerSection GetSectionBySlider(Point pt) {
			foreach(VRulerSection s in Sections)
				if(s.SliderPlaceBounds.Contains(pt))
					return s;
			return null;
		}
		public Rectangle GetMarkingBounds(int sectionIndex) {
			return this.OffsetBounds(Sections[sectionIndex].MarkingBounds);
		}
	}
	public class SectionCollection : CollectionBase {
		RulerBase ruler;
		public SectionCollection(RulerBase ruler) {
			this.ruler = ruler;
		}
		public RulerSection this[int index] {
			get { return (index >= 0 && index < List.Count) ? (RulerSection)List[index] : null; }
		}
		public int Add(RulerSection section) {
			List.Add(section);
			return List.IndexOf(section);
		}
		public int Insert(int index, RulerSection section) {
			List.Insert(index, section);
			return List.IndexOf(section);
		}
		protected override void OnInsertComplete(int index, object value) {
			RulerSection s = value as RulerSection;
			s.SetRulerInternal(ruler);
		}
		public void Remove(RulerSection section) {
			List.Remove(section);
		}
		public int IndexOf(RulerSection section) {
			return List.IndexOf(section);
		}
	}
	public abstract class RulerSection {
		#region static
		internal static Color MarginColor = Color.FromArgb(90, 0, 0, 0);
		internal static Font fFont = new Font("Arial", 7);
		protected static bool IsSeparatorDrawNeeded(RulerUnit rulerUnits, int num) {
			return rulerUnits.PointsPerHalfUnit > 0 && (num % rulerUnits.PointsPerHalfUnit) == 0;
		}
		static protected void DrawStringCore(Graphics gr, Font font, int x, int y, SizeF size, string sign, Brush textBrush) {
			gr.DrawString(sign, font, textBrush, x, y);
		}
		#endregion
		protected const int SeparatorLenght = 3;
		RulerBase ruler;
		protected Rectangle fMarkingBounds = Rectangle.Empty;
		protected int fLength;
		public RulerBase Ruler {
			get { return ruler; }
		}
		public Rectangle MarkingBounds {
			get { return fMarkingBounds; }
		}
		public virtual Rectangle Bounds {
			get { return fMarkingBounds; }
		}
		public int Index {
			get { return Ruler.Sections.IndexOf(this); }
		}
		protected RulerSection(int length) {
			SetLength(length);
		}
		internal void SetRulerInternal(RulerBase ruler) {
			this.ruler = ruler;
		}
		internal void SetLength(int value) {
			fLength = Math.Max(0, value);
		}
		protected internal abstract void UpdateBounds(Point pos);
		protected internal virtual void DrawContent(RulerViewInfo e, RulerSectionPaintHelper paintHelper) {
			FillBackground(e, paintHelper);
			DrawMarking(e, paintHelper.TextBrush);
		}
		protected virtual void DrawMarking(RulerViewInfo e, Brush textBrush) {
		}
		protected virtual void DrawMarkingCore(RulerViewInfo e, int maxVal, int offset, Brush textBrush) {
			double x = 0;
			int num = 0;
			int direction = maxVal >= 0 ? 1 : -1;
			int clientCenter = GetClientCenter(e.SectionBounds);
			while(Math.Abs(x) <= Math.Abs(maxVal)) {
				if(num > 0) {
					if(ShouldDrawValue(num))
						DrawValue(e.Graphics, clientCenter, (int)x + offset, GetDrawingValue(num), textBrush);
					else if(IsSeparatorDrawNeeded(Ruler.RulerUnit, num))
						DrawSeparator(e.Graphics, clientCenter, (int)x + offset, textBrush);
					else
						DrawDot(e.Graphics, clientCenter, (int)x + offset, textBrush);
				}
				num++;
				x += Ruler.RulerUnit.QuantizationStep * direction;
			}
		}
#if DEBUGTEST
		internal
#endif
		protected bool ShouldDrawValue(int num) {
			return num % Ruler.RulerUnit.PointsPerUnit == 0;
		}
		int GetDrawingValue(int num) {
			return num / Ruler.RulerUnit.PointsPerUnit;
		}
		void FillBackground(RulerViewInfo e, RulerSectionPaintHelper paintHelper) {
			paintHelper.FillSectionBackground(e, this);
		}
		protected abstract int GetClientCenter(Rectangle bounds);
		protected abstract void DrawDot(Graphics gr, int clientCenter, int offset, Brush textBrush);
		protected abstract void DrawSeparator(Graphics gr, int clientCenter, int offset, Brush textBrush);
		protected abstract void DrawValue(Graphics gr, int clientCenter, int offset, int sign, Brush textBrush);
		public virtual RulerState GetState(Point pt) {
			return RulerState.None;
		}
	}
	public class HRulerSection : RulerSection {
		public new HRuler Ruler {
			get { return base.Ruler as HRuler; }
		}
		public HRulerSection(int workPlaceLength)
			: base(workPlaceLength) {
		}
		protected internal override void UpdateBounds(Point pos) {
			fMarkingBounds = new Rectangle(pos.X, pos.Y, fLength, Ruler.ClientBounds.Height);
		}
		protected override void DrawMarking(RulerViewInfo e, Brush textBrush) {
			DrawMarkingCore(e, e.SectionBounds.Width, e.SectionBounds.Left, textBrush);
		}
		protected override int GetClientCenter(Rectangle bounds) {
			return RectHelper.CenterOf(bounds).Y;
		}
		protected override void DrawDot(Graphics gr, int clientCenter, int offset, Brush textBrush) {
			gr.FillRectangle(textBrush, offset, clientCenter - 1, 1, 2);
		}
		protected override void DrawSeparator(Graphics gr, int clientCenter, int offset, Brush textBrush) {
			gr.FillRectangle(textBrush, offset, clientCenter - SeparatorLenght, 1, 2 * SeparatorLenght);
		}
		protected override void DrawValue(Graphics gr, int clientCenter, int offset, int sign, Brush textBrush) {
			SizeF size = gr.MeasureString(sign.ToString(), fFont);
			int x1 = Convert.ToInt32(offset - size.Width / 2);
			int y1 = clientCenter - Convert.ToInt32(size.Height / 2);
			DrawStringCore(gr, fFont, x1, y1, size, sign.ToString(), textBrush);
		}
		protected override void DrawMarkingCore(RulerViewInfo e, int maxVal, int offset, Brush textBrush) {
			base.DrawMarkingCore(e, maxVal - Ruler.LeftMargin, offset + Ruler.LeftMargin, textBrush);
			base.DrawMarkingCore(e, -Ruler.LeftMargin, offset + Ruler.LeftMargin, textBrush);
		}
	}
	public class VRulerSection : RulerSection {
		int sliderHeight;
		Rectangle sliderPlaceBounds = Rectangle.Empty;
		bool isExpanded = true;
		bool canResize = true;
		int level;
		bool isTopNeighbor = false;
		bool selected;
		protected internal Rectangle SliderPlaceBounds {
			get { return sliderPlaceBounds; }
		}
		protected internal bool IsExpanded {
			get { return isExpanded; }
		}
		protected internal bool CanResize {
			get { return canResize; }
		}
		protected internal int Level {
			get { return level; }
		}
		protected internal int SliderHeight {
			get { return sliderHeight; }
		}
		protected internal bool IsTopNeighbor {
			get { return isTopNeighbor; }
		}
		protected internal bool Selected {
			get { return selected; }
		}
		public override Rectangle Bounds {
			get { return Rectangle.Union(sliderPlaceBounds, fMarkingBounds); }
		}
		public VRulerSection(int workPlaceLength, bool isExpanded, bool canResize, int level, int sliderHeight, bool isTopNeighbor, bool selected)
			: base(workPlaceLength) {
			this.isExpanded = isExpanded;
			this.canResize = canResize;
			this.level = level;
			this.sliderHeight = sliderHeight;
			this.isTopNeighbor = isTopNeighbor;
			this.selected = selected;
		}
		protected internal override void UpdateBounds(Point pos) {
			sliderPlaceBounds = new Rectangle(pos.X, pos.Y, Ruler.ClientBounds.Width, sliderHeight);
			fMarkingBounds = new Rectangle(pos.X, pos.Y + sliderHeight, Ruler.ClientBounds.Width, fLength);
		}
		public override RulerState GetState(Point pt) {
			return (isExpanded && Math.Abs(Bounds.Bottom - pt.Y) < RulerBase.PixelFault) ?
				RulerState.SectionChanged : RulerState.None;
		}
		protected internal override void DrawContent(RulerViewInfo e, RulerSectionPaintHelper paintHelper) {
			base.DrawContent(e, paintHelper);
			if(SliderHeight > 0)
				paintHelper.DrawSlider(e.Cache, this, e.RulerOrientation);
		}
		protected override void DrawMarking(RulerViewInfo e, Brush textBrush) {
			const int VerticalOffset = 1;
			int y = e.SectionBounds.Top + sliderPlaceBounds.Height - VerticalOffset;
			DrawMarkingCore(e, e.SectionBounds.Bottom - y, y, textBrush);
		}
		protected override int GetClientCenter(Rectangle bounds) {
			return RectHelper.CenterOf(bounds).X;
		}
		protected override void DrawDot(Graphics gr, int clientCenter, int offset, Brush textBrush) {
			gr.FillRectangle(textBrush, clientCenter - 1, offset, 2, 1);
		}
		protected override void DrawSeparator(Graphics gr, int clientCenter, int offset, Brush textBrush) {
			gr.FillRectangle(textBrush, clientCenter - SeparatorLenght, offset, 2 * SeparatorLenght, 1);
		}
		protected override void DrawValue(Graphics gr, int clientCenter, int offset, int sign, Brush textBrush) {
			SizeF size = gr.MeasureString(sign.ToString(), fFont);
			int x1 = clientCenter - Convert.ToInt32(size.Width / 2);
			int y1 = Convert.ToInt32(offset - size.Height / 2);
			DrawStringCore(gr, fFont, x1, y1, size, sign.ToString(), textBrush);
		}
	}
	public class RulerSectionEventArgs : EventArgs {
		private RulerSection section;
		public RulerSection RulerSection {
			get { return section; }
		}
		public RulerSectionEventArgs(RulerSection section) {
			this.section = section;
		}
	}
	public delegate void RulerSectionEventHandler(object sender, RulerSectionEventArgs e);
	public class RulerViewInfo : ObjectInfoArgs {
		Rectangle sectionBounds = Rectangle.Empty;
		Point viewOffset = Point.Empty;
		RulerBase ruler;
		RulerOrientation rulerOrientation;
		public RulerBase Ruler {
			get { return ruler; }
		}
		public RulerOrientation RulerOrientation {
			get { return rulerOrientation; }
			set { rulerOrientation = value; }
		}
		public Rectangle SectionBounds {
			get { return sectionBounds; }
			set { sectionBounds = value; }
		}
		public Point ViewOffset {
			get { return viewOffset; }
			set { viewOffset = value; }
		}
		public RulerViewInfo(RulerBase ruler) {
			this.ruler = ruler;
		}
	}
}
