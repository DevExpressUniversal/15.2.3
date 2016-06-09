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
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraBars.Docking2010 {
	public enum ScrollBarVisibility {
		Disabled,
		Hidden,
		Auto,
		Visible
	}
	public interface IScrollBarInfo : IBaseElementInfo,
		IInteractiveElementInfo {
		bool IsScrolling { get; set; }
	}
	public interface IScrollBarHandler {
		void OnKeyDown(KeyEventArgs e);
		void OnMouseDown(MouseEventArgs e);
		void OnMouseMove(MouseEventArgs e);
		void OnMouseUp(MouseEventArgs e);
		void OnMouseWheel(MouseEventArgs e);
		void OnMouseEnter();
		void OnMouseLeave();
	}
	public interface IScrollBarInfoOwner {
		Point PointToClient(Point point);
		Rectangle Bounds { get; }
		void Invalidate();
		bool IsHorizontal { get; }
		UserLookAndFeel GetLookAndFeel();
		ScrollBarVisibility ScrollBarVisibility { get; }
	}
	public abstract class ScrollBarInfoWithHandler : IScrollBar, ISupportLookAndFeel, IDisposable {
		internal const int DefaultSecondScrollInterval = 50;
		internal const int DefaultFirstScrollInterval = 200;
		IScrollBarHandler handlerCore;
		ScrollBarViewInfo viewInfoCore;
		ScrollBarPainterBase painter;
		Rectangle bounds = Rectangle.Empty;
		AppearanceObject style;
		AppearanceObject backStyle;
		bool enabled;
		IScrollBarInfoOwner ownerCore;
		int beforeThumbDraggingValueCore;
		public ScrollBarInfoWithHandler(IScrollBarInfoOwner owner) {
			this.viewInfoCore = CreateScrollBarViewInfo();
			this.handlerCore = CreateHandler();
			painter = null;
			ownerCore = owner;
			enabled = true;
			SetDefaultScrollBarHeight();
			style = new AppearanceObject();
			backStyle = new AppearanceObject();
			backStyle.BackColor = SystemColors.WindowText;
		}
		public IScrollBarInfoOwner Owner { get { return ownerCore; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public virtual bool Enabled { get { return enabled; } set { enabled = value; } }
		public virtual RightToLeft RightToLeft { get { return RightToLeft.No; } }
		public virtual UserLookAndFeel LookAndFeel { get { return Owner.GetLookAndFeel(); } }
		protected internal int BeforeThumbDraggingValue {
			get { return beforeThumbDraggingValueCore; }
			set { beforeThumbDraggingValueCore = value; }
		}
		public IScrollBarHandler Handler {
			get { return handlerCore; }
		}
		bool IScrollBar.ContainsMouse() { return false; }
		bool IScrollBar.IsOverlapScrollBar { get { return false; } }
		ScrollBarDrawMode IScrollBar.DrawMode {
			get { return DrawModeCore; }
		}
		public int Maximum {
			get { return MaximumCore; }
			set {
				if(value != Maximum) {
					if(value < Minimum) {
						Minimum = value;
					}
					MaximumCore = value;
					Value = GetScrollBarValue(Value);
					LayoutChanged();
				}
			}
		}
		public int Minimum {
			get { return MinimumCore; }
			set {
				if(value != Minimum) {
					if(value > Maximum) {
						Maximum = value;
					}
					MinimumCore = value;
					Value = GetScrollBarValue(Value);
					LayoutChanged();
				}
			}
		}
		public int LargeChange {
			get { return LargeChangeCore; }
			set {
				if((value > 0) && (value != LargeChangeCore)) {
					if(value > ((Maximum - Minimum) + 1))
						value = (Maximum - Minimum) + 1;
					CorrectionValue(value);
					LargeChangeCore = value;
					LayoutChanged();
				}
			}
		}
		void CorrectionValue(int value) {
			if(value + Value >= Maximum)
				Value = Maximum - value;
			if(value + Value < Maximum && LargeChange + Value >= Maximum)
				Value = Maximum - value;
			if(LargeChange > Maximum)
				Value = 0;
		}
		public int Value {
			get { return ValueCore; }
			set {
				if(((value >= Minimum) && (value <= Maximum)) && (value != Value)) {
					int num = Value;
					ValueCore = value;
					LayoutChanged();
					if(num != Value) {
						OnValueChanged(EventArgs.Empty);
					}
				}
			}
		}
		public int SmallChange {
			get { return SmallChangeCore; }
			set {
				if((value > 0) || (value < (Maximum - Minimum))) {
					SmallChangeCore = value;
				}
			}
		}
		public int Width {
			get { return bounds.Width; }
			set {
				if(value < 0) return;
				bounds = new Rectangle(bounds.X, bounds.Y, value, bounds.Height);
			}
		}
		public int Height {
			get { return bounds.Height; }
			set {
				if(value < 0) return;
				bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, value);
			}
		}
		public bool Visible { get { return CalcIsVisible(); } }
		public ScrollBarVisibility ScrollBarVisibility {
			get { return Owner.ScrollBarVisibility; }
		}
		bool CalcIsVisible() {
			if(ScrollBarVisibility == ScrollBarVisibility.Visible) return true;
			if(ScrollBarVisibility == ScrollBarVisibility.Auto && LargeChange <= Maximum) return true;
			return false;
		}
		public void Invalidate(Rectangle rect) { Invalidate(); }
		public abstract ScrollBarType ScrollBarType { get; }
		public void DoDraw(GraphicsCache cache, Rectangle clipRect) {
			if(Visible) {
				ScrollBarInfoArgs args = new ScrollBarInfoArgs(cache, Bounds, style, backStyle, this);
				args.State = Enabled ? ObjectState.Normal : ObjectState.Disabled;
				Painter.DrawObject(args);
			}
		}
		public virtual void Invalidate() { Owner.Invalidate(); }
		public ScrollBarViewInfo ViewInfo {
			get { return viewInfoCore; }
		}
		public void PerformLayout() { LayoutChanged(); }
		public virtual void OnLostCapture() { SetNormalState(); }
		public void OnLostFocus(EventArgs e) { }
		public bool IsDisposing { get; private set; }
		public virtual void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				IDisposable handlerDisposable = handlerCore as IDisposable;
				Ref.Dispose(ref handlerDisposable);
				handlerCore = null;
				if(painter != null) painter.DestroyBrushes();
			}
			GC.SuppressFinalize(this);
		}
		public event ScrollEventHandler Scroll;
		public event EventHandler ScrollBarAutoSizeChanged;
		public event EventHandler ValueChanged;
		protected virtual ScrollBarViewInfo CreateScrollBarViewInfo() {
			return new ScrollBarInfoWithOffset(this);
		}
		protected virtual IScrollBarHandler CreateHandler() {
			return new ScrollBarInfoHandler(this);
		}
		protected internal int VisibleValue {
			get {
				if(ViewInfo.IsRightToLeft) {
					return ((ViewInfo.VisibleMaximum - Minimum) - Value);
				}
				return Value;
			}
			set {
				if(ViewInfo.IsRightToLeft) {
					Value = (ViewInfo.VisibleMaximum - Minimum) - value;
				}
				else Value = value;
			}
		}
		protected virtual ScrollBarDrawMode DrawModeCore {
			get { return ScrollBarDrawMode.Desktop; }
		}
		protected abstract int LargeChangeCore { get; set; }
		protected abstract int MaximumCore { get; set; }
		protected abstract int MinimumCore { get; set; }
		protected abstract int ValueCore { get; set; }
		protected abstract int SmallChangeCore { get; set; }
		protected internal ScrollBarPainterBase Painter {
			get {
				if(painter == null) {
					painter = CreateScrollBarPainter();
				}
				return painter;
			}
		}
		protected virtual ScrollBarPainterBase CreateScrollBarPainter() {
			return LookAndFeelScrollBarPainter.GetPainter(LookAndFeel);
		}
		protected virtual void OnScroll(ScrollEventArgs e) {
			if(Scroll != null) {
				Scroll(this, e);
			}
		}
		protected virtual void OnScrollBarScrollBarAutoSizeChanged(EventArgs e) {
			if(ScrollBarAutoSizeChanged != null) {
				ScrollBarAutoSizeChanged(this, e);
			}
		}
		protected virtual void OnValueChanged(EventArgs e) {
			if(ValueChanged != null) {
				ValueChanged(this, e);
			}
		}
		protected internal virtual void SetNormalState() {
			if(State != ScrollBarState.Normal) {
				if(IsScrollingState(State) || (State == ScrollBarState.ThumbPressed)) {
					if(State == ScrollBarState.ThumbPressed) {
						SetScrollBarValue(ScrollEventType.ThumbPosition, Value);
					}
					State = ScrollBarState.Normal;
					SetScrollBarValue(ScrollEventType.EndScroll, Value);
				}
				State = ScrollBarState.Normal;
				LayoutChanged();
			}
		}
		protected internal virtual ScrollBarState GetScrollBarState() {
			Point mousePosition = Owner.PointToClient(Control.MousePosition);
			if(ScrollBarType == ScrollBarType.Horizontal)
				mousePosition = new Point(mousePosition.X, Owner.Bounds.Height - mousePosition.Y);
			else
				mousePosition = new Point(ownerCore.Bounds.Width - mousePosition.X, mousePosition.Y);
			return ViewInfo.GetScrollBarState(mousePosition, true);
		}
		protected internal ScrollBarState State {
			get { return ViewInfo.State; }
			set { if(value != State) { ViewInfo.State = value; } }
		}
		internal bool IsScrollingState(ScrollBarState state) {
			return ((((state == ScrollBarState.IncButtonPressed) || (state == ScrollBarState.DecButtonPressed)) || (state == ScrollBarState.IncAreaPressed)) || (state == ScrollBarState.DecAreaPressed));
		}
		void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			if(painter != null) painter.DestroyBrushes();
			painter = CreateScrollBarPainter();
			ViewInfo.Reset();
			Invalidate();
		}
		void SetDefaultScrollBarHeight() {
			if(ScrollBarType == ScrollBarType.Horizontal) {
				bounds.Height = SystemInformation.HorizontalScrollBarHeight;
			}
			else {
				bounds.Width = SystemInformation.VerticalScrollBarWidth;
			}
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		internal int GetScrollBarValue(int value) {
			if(value < Minimum) { return Minimum; }
			if(value > ViewInfo.VisibleMaximum) { return ViewInfo.VisibleMaximum; }
			return value;
		}
		int IScrollBar.VisibleValue { get { return VisibleValue; } }
		protected internal void SetScrollBarValue(ScrollEventType scrollType, int value) {
			value = GetScrollBarValue(value);
			ScrollEventArgs args = new ScrollEventArgs(scrollType, Value, value);
			OnScroll(args);
			Value = args.NewValue;
		}
		internal void LayoutChanged() {
			LayoutChanged(false);
		}
		void LayoutChanged(bool allowDelayed) {
			ViewInfo.Reset();
			Invalidate();
		}
		protected virtual int GetActualSmallChange() {
			return SmallChange;
		}
		protected virtual int GetActualLargeChange() {
			return LargeChange;
		}
		void IScrollBar.ChangeValueBasedByState(ScrollBarState state) {
			int num = Value;
			ScrollEventType type = ScrollEventType.EndScroll;
			switch(state) {
				case ScrollBarState.IncButtonPressed:
					num += GetActualSmallChange();
					type = ScrollEventType.SmallIncrement;
					break;
				case ScrollBarState.DecButtonPressed:
					num -= GetActualSmallChange();
					type = ScrollEventType.SmallDecrement;
					break;
				case ScrollBarState.IncAreaPressed:
					num += GetActualLargeChange();
					type = ScrollEventType.LargeIncrement;
					break;
				case ScrollBarState.DecAreaPressed:
					num -= GetActualLargeChange();
					type = ScrollEventType.LargeDecrement;
					break;
			}
			if(type != ScrollEventType.EndScroll) {
				SetScrollBarValue(type, num);
			}
		}
		internal void SetScrollBarValueInternal(int value) {
			ValueCore = value;
		}
		public Rectangle CalcClientRectangle(Rectangle bounds) {
			if(!Visible) return bounds;
			if(ScrollBarType == ScrollBarType.Horizontal)
				bounds.Height -= Bounds.Height;
			else
				bounds.Width -= Bounds.Width;
			return bounds;
		}
		#region IScrollBar Members
		int IScrollBar.GetWidth() {
			return Width;
		}
		int IScrollBar.GetHeight() {
			return Height;
		}
		bool IScrollBar.GetEnabled() {
			return Enabled;
		}
		Control IScrollBar.Parent {
			get { return null; }
		}
		ScrollBarPainterBase IScrollBar.Painter {
			get { return Painter; }
		}
		void IScrollBar.ProcessMouseEnter() {
			Handler.OnMouseEnter();
		}
		void IScrollBar.ProcessMouseLeave() {
			Handler.OnMouseLeave();
		}
		void IScrollBar.ProcessMouseDown(MouseEventArgs e) {
			Handler.OnMouseDown(e);
		}
		void IScrollBar.ProcessMouseMove(MouseEventArgs e) {
			Handler.OnMouseMove(e);
		}
		void IScrollBar.ProcessMouseUp(MouseEventArgs e) {
			Handler.OnMouseUp(e);
		}
		void IScrollBar.ProcessPaint(PaintEventArgs e) {
			using(var cache = new GraphicsCache(e)) {
				DoDraw(cache, e.ClipRectangle);
			}
		}
		#endregion
	}
	class ScrollBarInfoWithOffset : ScrollBarViewInfo {
		ScrollBarInfoWithHandler scrollBar;
		public ScrollBarInfoWithOffset(ScrollBarInfoWithHandler scroll)
			: base(scroll) {
			this.scrollBar = scroll;
		}
		public virtual int OffsetX { get { return scrollBar.Bounds.X; } }
		public virtual int OffsetY { get { return scrollBar.Bounds.Y; } }
		Rectangle GetRectangleWithOffset(Rectangle rect) {
			return new Rectangle(new Point(rect.X + OffsetX, rect.Y + OffsetY), rect.Size);
		}
		public override Rectangle VisibleIncButtonBounds {
			get {
				Rectangle rect = base.VisibleIncButtonBounds;
				return GetRectangleWithOffset(rect);
			}
		}
		public override Rectangle VisibleDecButtonBounds {
			get {
				Rectangle rect = base.VisibleDecButtonBounds;
				return GetRectangleWithOffset(rect);
			}
		}
		protected override Rectangle CalcThumbButtonBounds {
			get {
				Rectangle rect = base.CalcThumbButtonBounds;
				return GetRectangleWithOffset(rect);
			}
		}
		public override ScrollBarHitTest GetHitTest(Point pt) {
			return base.GetHitTest(new Point(pt.X, pt.Y));
		}
	}
	public class ScrollBarInfo : ScrollBarInfoWithHandler {
		int largeChangeCore;
		int maximumCore;
		int minimumCore;
		int smallChangeCore;
		int valueCore;
		public ScrollBarInfo(IScrollBarInfoOwner owner)
			: base(owner) {
			this.largeChangeCore = 100;
			this.maximumCore = 100;
			this.minimumCore = 0;
			this.smallChangeCore = 1;
			this.valueCore = this.minimumCore;
		}
		public override ScrollBarType ScrollBarType {
			get { return ScrollBarType.Vertical; }
		}
		protected override int LargeChangeCore {
			get { return largeChangeCore; }
			set { largeChangeCore = value; }
		}
		protected override int MaximumCore {
			get { return maximumCore; }
			set { maximumCore = value; }
		}
		protected override int MinimumCore {
			get { return minimumCore; }
			set { minimumCore = value; }
		}
		protected override int SmallChangeCore {
			get { return smallChangeCore; }
			set { smallChangeCore = value; }
		}
		protected override int ValueCore {
			get { return valueCore; }
			set { valueCore = value; }
		}
		protected override int GetActualLargeChange() {
			return 10;
		}
		protected override int GetActualSmallChange() {
			return 1;
		}
	}
	class ScrollBarInfoHandler : IScrollBarHandler, IDisposable {
		ScrollBarInfoWithHandler ownerCore;
		Timer scrollTimerCore;
		public ScrollBarInfoHandler(ScrollBarInfoWithHandler owner) {
			ownerCore = owner;
			scrollTimerCore = new Timer();
			scrollTimerCore.Enabled = false;
			scrollTimerCore.Tick += ScrollTimerEventProcessor;
		}
		void IDisposable.Dispose() {
			if(scrollTimerCore != null) {
				scrollTimerCore.Enabled = false;
				scrollTimerCore.Tick -= ScrollTimerEventProcessor;
			}
			Ref.Dispose(ref scrollTimerCore);
		}
		internal Timer ScrollTimer {
			get { return scrollTimerCore; }
		}
		public ScrollBarInfoWithHandler Owner { get { return ownerCore; } }
		public virtual void OnKeyDown(KeyEventArgs e) {
			if(!e.Handled) {
				if((e.KeyData == Keys.Up) || (e.KeyData == Keys.Left)) {
					((IScrollBar)Owner).ChangeValueBasedByState(ScrollBarState.DecButtonPressed);
				}
				if((e.KeyData == Keys.Down) || (e.KeyData == Keys.Right)) {
					((IScrollBar)Owner).ChangeValueBasedByState(ScrollBarState.IncButtonPressed);
				}
				if(e.KeyData == Keys.Prior) {
					((IScrollBar)Owner).ChangeValueBasedByState(ScrollBarState.DecAreaPressed);
				}
				if(e.KeyData == Keys.Next) {
					((IScrollBar)Owner).ChangeValueBasedByState(ScrollBarState.IncAreaPressed);
				}
				if(e.KeyData == Keys.Home) {
					Owner.Value = Owner.Minimum;
				}
				if(e.KeyData == Keys.End) {
					Owner.Value = Owner.ViewInfo.VisibleMaximum;
				}
			}
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				Owner.State = Owner.ViewInfo.GetScrollBarState(new Point(e.X, e.Y), true);
				if(Owner.State == ScrollBarState.ThumbPressed) {
					Owner.BeforeThumbDraggingValue = Owner.Value;
					Owner.ViewInfo.BeforeThumbTracking((Owner.ScrollBarType == ScrollBarType.Horizontal) ? e.X : e.Y);
				}
				((IScrollBar)Owner).ChangeValueBasedByState(Owner.State);
				ScrollTimer.Enabled = Owner.IsScrollingState(Owner.State);
				if(ScrollTimer.Enabled) {
					ScrollTimer.Interval = ScrollBarInfoWithHandler.DefaultFirstScrollInterval;
				}
			}
		}
		void ScrollTimerEventProcessor(Object item, EventArgs e) {
			scrollTimerCore.Interval = ScrollBarInfoWithHandler.DefaultSecondScrollInterval;
			ScrollBarState state = Owner.GetScrollBarState();
			if(state == Owner.State)
				((IScrollBar)Owner).ChangeValueBasedByState(state);
		}
		public virtual void OnMouseEnter() {
		}
		public virtual void OnMouseLeave() {
			Owner.ViewInfo.UpdateHotState(new Point(-10000, -10000));
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			if(Owner.State == ScrollBarState.ThumbPressed) {
				int num1 = (Owner.ScrollBarType == ScrollBarType.Horizontal) ? e.X : e.Y;
				if((num1 < (-3 * Owner.ViewInfo.ButtonWidth)) || ((num1 - Owner.ViewInfo.ScrollBarWidth) > (2 * Owner.ViewInfo.ButtonWidth))) {
					Owner.SetScrollBarValue(ScrollEventType.ThumbTrack, Owner.BeforeThumbDraggingValue);
				}
				else {
					int num2 = Owner.ViewInfo.GetValueByPos(num1);
					Owner.SetScrollBarValue(ScrollEventType.ThumbTrack, num2);
					Owner.LayoutChanged();
				}
			}
			Owner.ViewInfo.UpdateHotState(new Point(e.X, e.Y));
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			ScrollTimer.Enabled = false;
			Owner.SetNormalState();
			Owner.ViewInfo.UpdateHotState(new Point(e.X, e.Y));
		}
		public virtual void OnMouseWheel(MouseEventArgs e) {
			Owner.SetScrollBarValue(ScrollEventType.ThumbTrack, Owner.Value - e.Delta);
		}
	}
}
