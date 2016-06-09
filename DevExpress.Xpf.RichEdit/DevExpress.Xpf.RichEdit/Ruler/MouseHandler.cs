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
using System.Text;
using DevExpress.Services;
using DevExpress.Services.Implementation;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Ruler;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
#if !SL
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentKeyEventArgs = System.Windows.Forms.KeyEventArgs;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
#else
using PlatformIndependentKeyEventArgs = DevExpress.Data.KeyEventArgs;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentDragEventArgs = DevExpress.Utils.DragEventArgs;
using PlatformIndependentMouseButtons = DevExpress.Data.MouseButtons;
using DevExpress.Data;
using DevExpress.Xpf.Core.Native;
#endif
#if !SL && !WPF
namespace DevExpress.XtraRichEdit.Ruler {
#else
using DevExpress.Xpf.RichEdit.Controls.Internal;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit;
using DevExpress.Office.Internal;
namespace DevExpress.Xpf.RichEdit.Ruler {
#endif
	#region RulerMouseHandlerService
	public class RulerMouseHandlerService : MouseHandlerService {
		readonly RulerControlBase control;
		public RulerMouseHandlerService(RulerControlBase control)
			: base(control.MouseHandler) {
			this.control = control;
		}
		public RulerControlBase Control { get { return control; } }
		public override MouseHandler Handler { get { return Control.MouseHandler; } }
	}
	#endregion
	#region RulerAutoScroller
	public class RulerAutoScroller : AutoScroller {
		public RulerAutoScroller(MouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override void PopulateHotZones() {
		}
	}
	#endregion
	#region RulerMouseHandler (abstract class)
	public abstract class RulerMouseHandler : MouseHandler, IOrientation {
		readonly RulerControlBase control;
		protected RulerMouseHandler(RulerControlBase control) {
			this.control = control;
		}
		public RulerControlBase Control { get { return control; } }
		public IOrientation Orientation { get { return Control.Orientation; } }
		protected override void CalculateAndSaveHitInfo(PlatformIndependentMouseEventArgs e) {
		}
		protected override AutoScroller CreateAutoScroller() {
			return new RulerAutoScroller(this);
		}
		protected override IOfficeScroller CreateOfficeScroller() {
			return null;
		}
		protected override void HandleClickTimerTick() {
		}
		protected override void HandleMouseWheel(PlatformIndependentMouseEventArgs e) {
		}
		protected override void StartOfficeScroller(Point clientPoint) {
		}
		protected internal virtual void SetMouseCursor(RichEditCursor cursor) {
#if SL
			Control.Surface.SetCursor(cursor);
#elif WPF
			Control.Cursor = XpfTypeConverter.ToPlatformCursor(cursor.Cursor);
#else
			Control.Cursor = cursor.Cursor;
#endif
		}
		protected internal virtual void CaptureMouse() {
#if SL || WPF
			Control.Focus();
			Control.CaptureMouse();
#endif
		}
		protected internal virtual void ReleaseMouseCapture() {
#if SL || WPF
			Control.ReleaseMouseCapture();
#endif
		}
		protected internal virtual Rectangle GetHotZoneBounds(RulerHotZone hotZone) {
			return hotZone.Bounds;
		}
		public override void SwitchToDefaultState() {
			MouseHandlerState newState = new DefaultRulerMouseHandlerState(this);
			SwitchStateCore(newState, Point.Empty);
		}
		protected internal virtual int GetSnappedPosition(int pos) {
			if (KeyboardHandler.IsAltPressed)
				return pos;
			return GetSnappedPositionCore(pos);
		}
		protected internal virtual int GetSnappedPositionCore(int pos) {
			List<RulerTickmark> tickmarks = Control.ViewInfoBase.RulerTickmarks;
			int index = Algorithms.BinarySearch(tickmarks, new TickMarkPositionComparable(this, pos));
			if (index < 0) {
				index = ~index;
				if (index >= tickmarks.Count || index == 0)
					return pos;
				index = GetNearestIndex(pos, tickmarks, index);
			}
			return (int)GetTickMiddlePoint(tickmarks[index].Bounds);
		}
		protected internal int GetNearestIndex(int pos, List<RulerTickmark> tickmarks, int index) {
			if (index > 0) {
				float nearInterval = pos - GetFarPrimaryCoordinate(tickmarks[index - 1].Bounds);
				float farInterval = GetNearPrimaryCoordinate(tickmarks[index].Bounds) - pos;
				if (nearInterval <= farInterval)
					index -= 1;
			}
			return index;
		}
		protected internal float GetTickMiddlePoint(RectangleF tickmarkBounds) {
			return GetNearPrimaryCoordinate(tickmarkBounds) + tickmarkBounds.Width / 2;
		}
		protected internal abstract IVisualFeedback CreateLineVisualFeedback(IVisualFeedbackValueProvider<RectangleVisualFeedbackValue> valueProvider);
		public int GetPrimaryCoordinate(Point point) {
			return Orientation.GetPrimaryCoordinate(point);
		}
		public int GetSecondaryCoordinate(Point point) {
			return Orientation.GetSecondaryCoordinate(point);
		}
		public Point CreatePoint(int primary, int secondary) {
			return Orientation.CreatePoint(primary, secondary);
		}
		public float GetNearPrimaryCoordinate(RectangleF bounds) {
			return Orientation.GetNearPrimaryCoordinate(bounds);
		}
		public float GetFarPrimaryCoordinate(RectangleF bounds) {
			return Orientation.GetFarPrimaryCoordinate(bounds);
		}
		public float GetPrimaryCoordinateExtent(RectangleF bounds) {
			return Orientation.GetPrimaryCoordinateExtent(bounds);
		}
		public int GetNearPrimaryCoordinate(Rectangle bounds) {
			return Orientation.GetNearPrimaryCoordinate(bounds);
		}
		public int GetFarPrimaryCoordinate(Rectangle bounds) {
			return Orientation.GetFarPrimaryCoordinate(bounds);
		}
		public RectangleF SetNearPrimaryCoordinate(RectangleF bounds, float value) {
			return Orientation.SetNearPrimaryCoordinate(bounds, value);
		}
	}
	#endregion
	#region HorizontalRulerMouseHandler
	public class HorizontalRulerMouseHandler : RulerMouseHandler {
		public HorizontalRulerMouseHandler(HorizontalRulerControl control)
			: base(control) {
		}
		public new HorizontalRulerControl Control { get { return (HorizontalRulerControl)base.Control; } }
		public override void OnMouseDown(PlatformIndependentMouseEventArgs e) {
			TabTypeToggleHotZone tabTypeToggleHotZone = Control.ViewInfo.TabTypeToggleHotZone;
#if !SL && !WPF
			if (tabTypeToggleHotZone.DisplayBounds.Contains(e.Location)) {
				tabTypeToggleHotZone.Commit(e.Location);
				Control.Repaint();
			}
#else
			int x = Control.GetPhysicalLeftInvisibleWidthInPixel();
			Point point = new Point((int)e.Location.X - x, (int)e.Location.Y);
			Rectangle tabTypeToggleBounds = Control.LayoutUnitsToPixels(GetHotZoneBounds(tabTypeToggleHotZone));
			if (tabTypeToggleBounds.Contains(point)) {
				tabTypeToggleHotZone.Commit(point);
				Control.Reset();
			}
#endif
			else
				base.OnMouseDown(e);
		}
		protected internal override Rectangle GetHotZoneBounds(RulerHotZone hotZone) {
#if !SL && !WPF
			return hotZone.Bounds;
#else
			Rectangle margin = Control.PixelsToLayoutUnits(Control.CalculateHotZoneMarginInPixels(hotZone));
			Rectangle bounds = hotZone.Bounds;
			return new Rectangle(bounds.Left + margin.Left, bounds.Top + margin.Top, bounds.Width + margin.Right, bounds.Height + margin.Bottom);
#endif
		}
		protected override PlatformIndependentMouseEventArgs ConvertMouseEventArgs(PlatformIndependentMouseEventArgs screenMouseEventArgs) {
#if !SL && !WPF
			Point location = Control.GetPhysicalPoint(screenMouseEventArgs.Location);
			location = Control.RichEditControl.ActiveView.CreateLogicalPoint(Control.ViewInfo.Bounds, location);
			location = new Point((int)(location.X * Control.ZoomFactor), (int)(location.Y * Control.ZoomFactor));
			return new PlatformIndependentMouseEventArgs(screenMouseEventArgs.Button, screenMouseEventArgs.Clicks, location.X, location.Y, screenMouseEventArgs.Delta);
#else
			DocumentLayoutUnitConverter unitConverter = Control.RichEditControl.DocumentModel.LayoutUnitConverter;
			return new PlatformIndependentMouseEventArgs(screenMouseEventArgs.Button, screenMouseEventArgs.Clicks, unitConverter.PixelsToLayoutUnits(screenMouseEventArgs.X), unitConverter.PixelsToLayoutUnits(screenMouseEventArgs.Y), screenMouseEventArgs.Delta);
#endif
		}
		protected internal override IVisualFeedback CreateLineVisualFeedback(IVisualFeedbackValueProvider<RectangleVisualFeedbackValue> valueProvider) {
			return new RichEditVerticalLineVisualFeedback(Control.RichEditControl, valueProvider);
		}
	}
	#endregion
	#region DefaultRulerMouseHandlerState
	public class DefaultRulerMouseHandlerState : MouseHandlerState {
		public DefaultRulerMouseHandlerState(RulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		#region Properties
		public override bool StopClickTimerOnStart { get { return false; } }
		public new RulerMouseHandler MouseHandler { get { return (RulerMouseHandler)base.MouseHandler; } }
		protected RulerControlBase Control { get { return MouseHandler.Control; } }
		#endregion
		public override void OnMouseDown(PlatformIndependentMouseEventArgs e) {
			if (!Control.RichEditControl.InnerControl.IsEditable)
				return;
			MouseHandler.CaptureMouse();
			Point point = new Point((int)e.Location.X, (int)e.Location.Y);
			RulerHotZone hotZone = GetRulerMovableElementIndex(point);
			if (hotZone == null)
				hotZone = CreateZone(point);
			if (hotZone != null && hotZone.Enabled && hotZone.CanEdit()) {
				hotZone.Activate(MouseHandler, point);
				Control.Repaint();
			}
			else
				base.OnMouseDown(e);
		}
		public override void OnMouseDoubleClick(PlatformIndependentMouseEventArgs e) {
			if (!Control.RichEditControl.InnerControl.IsEditable)
				return;
			Point point = new Point((int)e.Location.X, (int)e.Location.Y);
			RulerHotZone hotZone = GetRulerMovableElementIndex(point);
			if (hotZone != null) {
				hotZone.OnMouseDoubleClick();
				Control.Reset();
			}
			MouseHandler.ReleaseMouseCapture();
			Control.RichEditControl.SetFocus();
		}
		public override void OnMouseMove(PlatformIndependentMouseEventArgs e) {
			if (!Control.RichEditControl.InnerControl.IsEditable) {
				MouseHandler.SetMouseCursor(RichEditCursors.Default);
				return;
			}
			Point point = new Point((int)e.Location.X, (int)e.Location.Y);
			RulerHotZone hotZone = GetRulerMovableElementIndex(point);
			if (hotZone == null || !hotZone.CanEdit())
				MouseHandler.SetMouseCursor(RichEditCursors.Default);
			else
				MouseHandler.SetMouseCursor(hotZone.Cursor);
		}
		protected internal virtual RulerHotZone CreateZone(Point point) {
			HorizontalRulerControl rulerControl = Control as HorizontalRulerControl;
			if (rulerControl == null) 
				return null;
			int hitActiveAreaStart = GetHitActiveAreaStartPosition(point);
			if (hitActiveAreaStart >= 0) {
				int position = MouseHandler.GetSnappedPosition(MouseHandler.GetPrimaryCoordinate(point));
				RulerHotZone hotZone = rulerControl.ViewInfo.TabTypeToggleHotZone.HotZone.Clone();
				hotZone.SetNewValue(Control.ViewInfoBase.GetRulerModelPosition(position - rulerControl.ViewInfo.GetAdditionalCellIndent(true)));
				hotZone.IsNew = true;
				Control.ViewInfoBase.HotZones.Add(hotZone);
				return hotZone;
			}
			return null;
		}
		protected internal RulerHotZone GetRulerMovableElementIndex(Point point) {
			List<RulerHotZone> hotZones = Control.ViewInfoBase.HotZones;
			for (int i = hotZones.Count - 1; i >= 0; i--) {
				Rectangle bounds = MouseHandler.GetHotZoneBounds(hotZones[i]);
				if (bounds.Contains(point))
					return hotZones[i];
			}
			return null;
		}
		protected internal virtual int GetHitActiveAreaStartPosition(Point point) {
			List<RectangleF> activeAreaCollection = MouseHandler.Control.ViewInfoBase.ActiveAreaCollection;
			RectangleF bounds = activeAreaCollection[Control.ViewInfoBase.CurrentActiveAreaIndex];
			if (bounds.Contains(point))
				return (int)MouseHandler.GetNearPrimaryCoordinate(bounds);
			return -1;
		}
	}
	#endregion
	#region BeginDragHotZoneMouseDragHelperState
	public class BeginDragHotZoneMouseDragHelperState : BeginMouseDragHelperState {
		readonly RulerHotZone hotZone;
		public BeginDragHotZoneMouseDragHelperState(RulerMouseHandler mouseHandler, MouseHandlerState dragState, Point point, RulerHotZone hotZone)
			: base(mouseHandler, dragState, point) {
			Guard.ArgumentNotNull(hotZone, "hotZone");
			this.hotZone = hotZone;
		}
		public override void OnMouseUp(MouseEventArgs e) {
			if (hotZone.IsNew)
				DragState.OnMouseUp(e);
			else
				base.OnMouseUp(e);
			hotZone.RulerControl.Reset();
		}
	}
	#endregion
	#region DragAndDropMouseHandlerState
	public class DragAndDropMouseHandlerState : MouseHandlerState, IKeyboardHandlerService, IVisualFeedbackValueProvider<RectangleVisualFeedbackValue> {
		readonly RulerHotZone hotZone;
		readonly PageViewInfo pageViewInfo;
		readonly IVisualFeedback visualFeedback;
		IKeyboardHandlerService previousService;
		Point point;
		public DragAndDropMouseHandlerState(RulerMouseHandler mouseHandler, RulerHotZone hotZone, Point startMousePosition)
			: base(mouseHandler) {
			Guard.ArgumentNotNull(hotZone, "hotZone");
			this.hotZone = hotZone;
			this.point = startMousePosition;
			this.visualFeedback = MouseHandler.CreateLineVisualFeedback(this);
			RichEditView view = Control.RichEditControl.InnerControl.ActiveView;
			CaretPosition caretPosition = view.CaretPosition;
			if (caretPosition.Update(DocumentLayoutDetailsLevel.Column)) {
				this.pageViewInfo = caretPosition.PageViewInfo;
			}
		}
		public override bool StopClickTimerOnStart { get { return true; } }
		public new RulerMouseHandler MouseHandler { get { return (RulerMouseHandler)base.MouseHandler; } }
		protected RulerControlBase Control { get { return MouseHandler.Control; } }
		public override void Start() {
			this.previousService = Control.GetService<IKeyboardHandlerService>();
			Control.AddService(typeof(IKeyboardHandlerService), this);
			if (!hotZone.IsNew)
				hotZone.AddFakeHotZone();
			MouseMoveCore(point);
			base.Start();
			BeginVisualFeedback();
		}
		public override void Finish() {
			EndVisualFeedback();
			Control.RemoveService(typeof(IKeyboardHandlerService));
			if (this.previousService != null)
				Control.AddService(typeof(IKeyboardHandlerService), previousService);
			base.Finish();
		}
		public override void OnMouseMove(MouseEventArgs e) {
			HideVisualFeedback();
			Point point = new Point((int)e.Location.X, (int)e.Location.Y);
			MouseMoveCore(point);
			ShowVisualFeedback();
		}
		protected internal void MouseMoveCore(Point pt) {
			int primary = MouseHandler.GetSnappedPosition(MouseHandler.GetPrimaryCoordinate(pt));
			point = MouseHandler.CreatePoint(primary, MouseHandler.GetSecondaryCoordinate(pt));
			hotZone.OnMove(point);
			Control.Repaint();
		}
		public override void OnMouseUp(MouseEventArgs e) {
			HideVisualFeedback();
			Point pt = new Point((int)e.Location.X, (int)e.Location.Y);
			int primary = MouseHandler.GetSnappedPosition(MouseHandler.GetPrimaryCoordinate(pt));
			point = MouseHandler.CreatePoint(primary, MouseHandler.GetSecondaryCoordinate(pt));
			hotZone.Commit(point);
			Control.Reset();
			Control.Repaint();
			MouseHandler.SwitchToDefaultState();
			MouseHandler.ReleaseMouseCapture();
			Control.RichEditControl.SetFocus();
		}
		#region IKeyboardHandlerService Members
		public void OnKeyDown(KeyEventArgs e) {
			Keys key = e.KeyCode;
			if (key.Equals(Keys.Escape)) {
				HideVisualFeedback();
				MouseHandler.SwitchToDefaultState();
				Control.Reset();
				Control.Repaint();
			}
			if (key.Equals(Keys.Enter)) {
				HideVisualFeedback();
				hotZone.Commit(point);
				Control.Repaint();
			}
		}
		public void OnKeyPress(KeyPressEventArgs e) {
		}
		public void OnKeyUp(KeyEventArgs e) {
		}
		#endregion
		protected internal virtual void BeginVisualFeedback() {
			visualFeedback.Begin();
		}
		protected internal virtual void ShowVisualFeedback() {
			visualFeedback.Show();
		}
		protected internal virtual void HideVisualFeedback() {
			visualFeedback.Hide();
		}
		protected internal virtual void EndVisualFeedback() {
			visualFeedback.End();
		}
		#region IVisualFeedbackValueProvider<RectangleVisualFeedbackValue> Members
		public RectangleVisualFeedbackValue VisualFeedbackValue {
			get {
				int value = hotZone.GetVisualFeedbackValue(point, pageViewInfo);
				return new RectangleVisualFeedbackValue(pageViewInfo, new Rectangle(MouseHandler.CreatePoint(value, 0), Size.Empty));
			}
		}
		#endregion
	}
	#endregion
	#region TickMarkPositionComparable
	public class TickMarkPositionComparable : IComparable<RulerTickmark> {
		readonly IOrientation orientation;
		readonly int pos;
		public TickMarkPositionComparable(IOrientation orientation, int pos) {
			this.orientation = orientation;
			this.pos = pos;
		}
		#region IComparable<RulerTickmark> Members
		public int CompareTo(RulerTickmark tickmark) {
			if (pos < orientation.GetNearPrimaryCoordinate(tickmark.Bounds))
				return 1;
			else if (pos > orientation.GetFarPrimaryCoordinate(tickmark.Bounds))
				return -1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region VerticalRulerMouseHandler
	public class VerticalRulerMouseHandler : RulerMouseHandler {
		public VerticalRulerMouseHandler(VerticalRulerControl control)
			: base(control) {
		}
		protected override PlatformIndependentMouseEventArgs ConvertMouseEventArgs(PlatformIndependentMouseEventArgs screenMouseEventArgs) {
#if !SL && !WPF
			Point location = Control.GetPhysicalPoint(screenMouseEventArgs.Location);
			location = CreateLogicalPoint(Control.ViewInfoBase.ClientBounds, location);
			location = new Point((int)(location.X), (int)(location.Y * Control.ZoomFactor));
			return new PlatformIndependentMouseEventArgs(screenMouseEventArgs.Button, screenMouseEventArgs.Clicks, location.X, location.Y, screenMouseEventArgs.Delta);
#else
			DocumentLayoutUnitConverter unitConverter = Control.RichEditControl.DocumentModel.LayoutUnitConverter;
			return new PlatformIndependentMouseEventArgs(screenMouseEventArgs.Button, screenMouseEventArgs.Clicks, unitConverter.PixelsToLayoutUnits(screenMouseEventArgs.X), unitConverter.PixelsToLayoutUnits(screenMouseEventArgs.Y), screenMouseEventArgs.Delta);
#endif
		}
		protected internal virtual Point CreateLogicalPoint(Rectangle clientBounds, Point point) {
			int y = Control.RichEditControl.ActiveView.CaretPosition.PageViewInfo.ClientBounds.Y;
			Matrix mat = new Matrix();
			mat.Translate(0, y);
			mat.Scale(1, Control.ZoomFactor);
			mat.Invert();
			Point[] result = new Point[1] { point };
			mat.TransformPoints(result);
			return result[0];
		}
		protected internal override IVisualFeedback CreateLineVisualFeedback(IVisualFeedbackValueProvider<RectangleVisualFeedbackValue> valueProvider) {
			return new RichEditHorizontalLineVisualFeedback(Control.RichEditControl, valueProvider);
		}
	}
	#endregion
#if !SL && !WPF
	#region RichEditReversibleLineVisualFeedback<T> (abstract class)
	public abstract class RichEditReversibleLineVisualFeedback<T> : VisualFeedback<T> {
		readonly RichEditControl control;
		protected RichEditReversibleLineVisualFeedback(RichEditControl control, IVisualFeedbackValueProvider<T> valueProvider)
			: base(valueProvider) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public RichEditControl Control { get { return control; } }
		public override void Begin() {
			DrawReversibleLine();
		}
		public override void Show() {
			DrawReversibleLine();
		}
		public override void Hide() {
			DrawReversibleLine();
		}
		public override void End() {
		}
		protected internal abstract void DrawReversibleLine();
	}
	#endregion
#else
	#region RichEditReversibleLineVisualFeedback<T> (abstract class)
	public abstract class RichEditReversibleLineVisualFeedback<T> : VisualFeedback<T> {
		readonly RichEditControl control;
		readonly XpfRichEditSizerSelection xpfSelection = new XpfRichEditSizerSelection();
		protected RichEditReversibleLineVisualFeedback(RichEditControl control, IVisualFeedbackValueProvider<T> valueProvider)
			: base(valueProvider) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public RichEditControl Control { get { return control; } }
		protected XpfRichEditSizerSelection XpfSelection { get { return xpfSelection; } }
		public override void Begin() {
			Show();
			Control.Surface.Children.Add(XpfSelection);
		}
		public override void Show() {
			DrawReversibleLine();
		}
		public override void Hide() {
		}
		public override void End() {
			Control.Surface.Children.Remove(XpfSelection);
		}
		protected internal virtual void DrawReversibleLine() {
			System.Windows.Rect rect = GetSelectionBounds(ValueProvider.VisualFeedbackValue);
			if (rect != System.Windows.Rect.Empty)
				XpfSelection.AddRect(rect, null);
		}
		protected internal abstract System.Windows.Rect GetSelectionBounds(T value);
	}
	#endregion
#endif
	#region RichEditHorizontalLineVisualFeedback
	public class RichEditHorizontalLineVisualFeedback : RichEditReversibleLineVisualFeedback<RectangleVisualFeedbackValue> {
		public RichEditHorizontalLineVisualFeedback(RichEditControl control, IVisualFeedbackValueProvider<RectangleVisualFeedbackValue> valueProvider)
			: base(control, valueProvider) {
		}
#if !SL && !WPF
		protected internal override void DrawReversibleLine() {
			RectangleVisualFeedbackValue value = ValueProvider.VisualFeedbackValue;
			if (value.PageViewInfo != null)
				Control.Painter.DrawReversibleHorizontalLine(value.Bounds.Y, value.PageViewInfo);
		}
#else
		protected internal override System.Windows.Rect GetSelectionBounds(RectangleVisualFeedbackValue value) {
			if (value.PageViewInfo == null)
				return System.Windows.Rect.Empty;
			Rectangle pageBounds = value.PageViewInfo.ClientBounds;
			Rectangle bounds = new Rectangle();
			bounds.Width = pageBounds.Width;
			bounds.Y = value.Bounds.Y;
			bounds.Height = 1;
			return XpfRichEditRectangularObjectResizeMouseHandlerStateStrategy.GetSelectionBounds(Control, bounds, pageBounds);
		}
#endif
	}
	#endregion
	#region RichEditVerticalLineVisualFeedback
	public class RichEditVerticalLineVisualFeedback : RichEditReversibleLineVisualFeedback<RectangleVisualFeedbackValue> {
		public RichEditVerticalLineVisualFeedback(RichEditControl control, IVisualFeedbackValueProvider<RectangleVisualFeedbackValue> valueProvider)
			: base(control, valueProvider) {
		}
#if !SL && !WPF
		protected internal override void DrawReversibleLine() {
			RectangleVisualFeedbackValue value = ValueProvider.VisualFeedbackValue;
			if (value.PageViewInfo != null)
				Control.Painter.DrawReversibleVerticalLine(value.Bounds.X, value.PageViewInfo);
		}
#else
		protected internal override System.Windows.Rect GetSelectionBounds(RectangleVisualFeedbackValue value) {
			if (value.PageViewInfo == null)
				return System.Windows.Rect.Empty;
			Rectangle pageBounds = value.PageViewInfo.ClientBounds;
			Rectangle bounds = new Rectangle();
			bounds.Height = pageBounds.Height;
			bounds.X = value.Bounds.X;
			bounds.Width = 1;
			return XpfRichEditRectangularObjectResizeMouseHandlerStateStrategy.GetSelectionBounds(Control, bounds, pageBounds);
		}
#endif
	}
	#endregion
}
