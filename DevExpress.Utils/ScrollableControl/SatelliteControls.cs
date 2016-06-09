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
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.NonclientArea;
using System.ComponentModel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.XtraEditors.Internal;
namespace DevExpress.XtraEditors {
	public interface IScrollView {
		void DoDraw(GraphicsCache g, Rectangle clipRect);
		void OnMouseDown(MouseEventArgs e);
		void OnMouseLeave(EventArgs e);
		void OnMouseMove(MouseEventArgs e);
		void OnMouseUp(MouseEventArgs e);
		void OnKeyDown(KeyEventArgs e);
		void OnLostFocus(EventArgs e);
		void OnLostCapture();
		void Invalidate();
		void PerformLayout();
		Rectangle Bounds { get; set;}
		XtraScrollableControl Owner { get; }
		bool TouchMode { get; }
		bool IsOverlap { get; }
	}
	public class SizeGripViewInfoWithHandler : IScrollView, ISupportLookAndFeel, IDisposable {
		protected UserLookAndFeel lookAndFeelCore;
		XtraScrollableControl owner;
		bool drawEmpty = false;
		Rectangle bounds = Rectangle.Empty;
		public SizeGripViewInfoWithHandler(XtraScrollableControl owner)
			: base() {
			this.owner = owner;
			this.lookAndFeelCore = new DevExpress.LookAndFeel.UserLookAndFeel(this);
			this.lookAndFeelCore.StyleChanged += new EventHandler(LookAndFeelStyleChanged);
			this.drawEmpty = true;
		}
		public bool TouchMode { get { return false; } }
		public bool IgnoreChildren { get { return false; } }
		public bool DrawEmpty { get { return drawEmpty; } set { drawEmpty = value; } }
		public UserLookAndFeel LookAndFeel { get { return lookAndFeelCore; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public XtraScrollableControl Owner { get { return owner; } }
		public virtual Color BackColor {
			get { return DefaultAppearance.BackColor; }
		}
		SkinElement GetSkin() {
			if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return null;
			return CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinForm];
		}
		void LookAndFeelStyleChanged(object sender, EventArgs e) {
			defaultAppearance = null;
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault res = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			if(LookAndFeel != null && LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement form = GetSkin();
				form.Apply(res);
				LookAndFeelHelper.CheckColors(LookAndFeel, res, this.Owner);
			}
			return res;
		}
		AppearanceDefault defaultAppearance = null;
		protected AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		public void DoDraw(GraphicsCache g, Rectangle clipRect) {
			if(!drawEmpty) {
				SizeGripObjectPainter painter = SizeGripHelper.GetPainter(lookAndFeelCore);
				SizeGripObjectInfoArgs infoArgs = new SizeGripObjectInfoArgs();
				infoArgs.Cache = g;
				infoArgs.GripPosition = SizeGripPosition.RightBottom;
				infoArgs.Bounds = Bounds;
				infoArgs.State = 0;
				AppearanceObject app = new AppearanceObject();
				app.BackColor = BackColor;
				infoArgs.SetAppearance(app);
				painter.DrawObject(infoArgs);
			}
			else {
				ISupportLookAndFeel parent = Owner.Parent as ISupportLookAndFeel;
				if(parent != null && parent.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					g.FillRectangle(g.GetSolidBrush(BackColor), Bounds);
				}
				else {
					Color backColor = Owner.Parent != null ? Owner.Parent.BackColor : Owner.BackColor;
					g.FillRectangle(g.GetSolidBrush(backColor), Bounds);
				}
			}
		}
		public void Dispose() {
			if(lookAndFeelCore != null) {
				lookAndFeelCore.StyleChanged -= new EventHandler(LookAndFeelStyleChanged);
				lookAndFeelCore.Dispose();
			}
		}
		bool IScrollView.IsOverlap { get { return false; } }
		void IScrollView.Invalidate() { Owner.RepaintNcElement(this); }
		void IScrollView.PerformLayout() { ((IScrollView)this).Invalidate(); }
		void IScrollView.OnMouseDown(MouseEventArgs e) { }
		void IScrollView.OnMouseMove(MouseEventArgs e) { }
		void IScrollView.OnMouseUp(MouseEventArgs e) { }
		void IScrollView.OnMouseLeave(EventArgs e) { }
		void IScrollView.OnLostCapture() { }
		void IScrollView.OnLostFocus(EventArgs e) { }
		void IScrollView.OnKeyDown(KeyEventArgs e) { }
	}
	public abstract class ScrollBarViewInfoWithHandlerBase : ScrollBarViewInfoWithHandlerBaseOld {
		bool isCreated = false;
		public ScrollBarViewInfoWithHandlerBase(XtraScrollableControl owner)
			: base(owner) {
				if(ScrollBarBase.GetUIMode(ScrollUIMode.Default) == ScrollUIMode.Touch)
					TouchMode = true;
				isCreated = true;
		}
		#region TouchBase
		FloatingScrollbar floatingScrollbar = null;
		bool touchMode;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool TouchMode {
			get {
				return touchMode;
			}
			set {
				if(touchMode == value) return;
				touchMode = value;
				OnTouchModeChanged();
			}
		}
		protected virtual bool IsAllowFloatingScrollbar {
			get { return TouchMode; }
		}
		protected virtual bool IsCreated { get { return isCreated; } }
		public override ScrollBarDrawMode DrawMode { get { return TouchMode ? ScrollBarDrawMode.TouchMode : ScrollBarDrawMode.Desktop; } }
		protected virtual void OnTouchModeChanged() {
			if(TouchMode) {
				Visible = false;
				LayoutChanged();
				if(ActualVisible) OnScrollChanged();
			}
			else {
				FloatingScrollbar.Hide();
				if(ActualVisible) {
					Visible = true;
					LayoutChanged();
					OnScrollChanged();
				}
			}
		}
		protected override void LayoutChanged(bool allowDelayed) {
			if(!IsCreated) return;
			base.LayoutChanged(allowDelayed);
		}
		protected virtual void DestroyFloatingScrollbar() {
			if(floatingScrollbar != null) floatingScrollbar.CleanUp();
		}
		public override void Dispose() {
			base.Dispose();
			DestroyFloatingScrollbar();
		}
		public virtual bool IsOverlapScrollBar { get { return TouchMode; } }
		protected FloatingScrollbar FloatingScrollbar { get { return floatingScrollbar; } }
		bool actualVisible = false;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool ActualVisible {
			get { return actualVisible; }
			protected set { actualVisible = value; }
		}
		public virtual void SetVisibility(bool visible) {
			bool prevVisible = ActualVisible;
			ActualVisible = visible;
			Visible = ActualVisible && !IsAllowFloatingScrollbar;
			if(prevVisible != ActualVisible) OnActualVisibleChanged();
		}
		public override Rectangle Bounds {
			get {
				return base.Bounds;
			}
			set {
				if(Bounds == value) return;
				base.Bounds = value;
				UpdateSize();
			}
		}
		protected virtual void UpdateSize() {
			if(Owner == null) return;
			if(!IsAllowFloatingScrollbar) return;
			if(Owner.FindForm() == null || !Owner.FindForm().IsHandleCreated || !Owner.IsHandleCreated) return;
			if(ActualVisible) EnsureCreated();
			Rectangle bounds = new Rectangle(Owner.PointToScreen(Bounds.Location), Bounds.Size);
			if(FloatingScrollbar != null) FloatingScrollbar.Bounds = bounds;
		}
		protected virtual void OnActualVisibleChanged() {
			if(ActualVisible) {
				if(IsAllowFloatingScrollbar) {
					EnsureCreated();
					UpdateSize();
					if(!FloatingScrollbar.AllowAutoHide) FloatingScrollbar.Show();
					FloatingScrollbar.OnScrollChanged();
				}
			}
			else {
				HideFloatingScrollbar();
			}
		}
		protected override void OnValueChanged(EventArgs e) {
			base.OnValueChanged(e);
			OnScrollChanged();
		}
		protected override void OnScroll(ScrollEventArgs e) {
			base.OnScroll(e);
			OnScrollChanged();
		}
		Point lastMouseActionPoint = Point.Empty;
		public void OnAction(ScrollNotifyAction action) {
			if(action == ScrollNotifyAction.MouseMove) {
				if(this.lastMouseActionPoint == Control.MousePosition) return;
				this.lastMouseActionPoint = Control.MousePosition;
			}
			if(action == ScrollNotifyAction.Resize) {
				this.resizeActionTime = DateTime.Now;
				DelayedOnActionCore(ScrollNotifyAction.Hide);
			}
			if(action == ScrollNotifyAction.Hide) {
				DelayedOnActionCore(action);
				return;
			}
			OnActionCore(action);
		}
		void DelayedOnActionCore(ScrollNotifyAction action) {
			if(Owner.IsHandleCreated) {
				Owner.BeginInvoke(new MethodInvoker(() => { OnActionCore(action); }));
			}
			else {
				OnActionCore(action);
			}
		}
		protected virtual void OnActionCore(ScrollNotifyAction action) {
			if(action == ScrollNotifyAction.Hide) {
				HideFloatingScrollbar();
				return;
			}
			if(action == ScrollNotifyAction.MouseMove) {
				if(WindowsFormsSettings.ShowTouchScrollBarOnMouseMove) OnScrollChanged();
			}
		}
		protected void HideFloatingScrollbar() {
			if(FloatingScrollbar == null) return;
			FloatingScrollbar.Hide();
		}
		DateTime resizeActionTime = DateTime.MinValue;
		void OnScrollChanged() {
			if(!IsCreated) return;
			if(DateTime.Now.Subtract(resizeActionTime).TotalMilliseconds < 100) return;
			if(!IsAllowFloatingScrollbar) return;
			EnsureCreated();
			if(ActualVisible) {
				UpdateSize();
				FloatingScrollbar.OnScrollChanged();
				FloatingScrollbar.Show();
			}
		}
		void EnsureCreated() {
			if(!IsAllowFloatingScrollbar) return;
			if(floatingScrollbar == null) floatingScrollbar = new FloatingScrollbar(this);
			if(Owner != null && Owner.IsHandleCreated && Owner.FindForm() != null) FloatingScrollbar.Create(Owner);
		}
		#endregion TouchBase
	}
	public abstract class ScrollBarViewInfoWithHandlerBaseOld : IScrollBar, IScrollView, ISupportLookAndFeel, IDisposable {
		const int DefaultSecondScrollInterval = 50;
		const int DefaultFirstScrollInterval = 200;
		ScrollBarViewInfo viewInfo;
		ScrollBarPainterBase painter;
		Rectangle bounds = Rectangle.Empty;
		UserLookAndFeel lookAndFeel;
		AppearanceObject style;
		AppearanceObject backStyle;
		Timer scrollTimer;
		bool enabled;
		XtraScrollableControl owner;
		int beforeThumbDraggingValue;
		public ScrollBarViewInfoWithHandlerBaseOld(XtraScrollableControl owner) {
			this.viewInfo = CreateScrollBarViewInfo();
			this.painter = null;
			this.owner = owner;
			this.enabled = true;
			this.SetDefaultScrollBarHeight();
			this.lookAndFeel = new UserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelStyleChanged);
			this.style = new AppearanceObject();
			this.backStyle = new AppearanceObject();
			this.backStyle.BackColor = SystemColors.WindowText;
			this.scrollTimer = new Timer();
			this.scrollTimer.Enabled = false;
			this.scrollTimer.Tick += new EventHandler(ScrollTimerEventProcessor);
		}
		public virtual bool TouchMode { get; set; }
		bool IScrollView.IsOverlap { 
			get {
				if(ScrollBarType == ScrollBarType.Horizontal) return Owner.IsOverlapHScrollBar;
				return Owner.IsOverlapVScrollBar;
			}
		}
		bool IScrollBar.IsOverlapScrollBar { get { return ((IScrollView)this).IsOverlap; } }
		void IScrollBar.ProcessMouseMove(MouseEventArgs e) { ((IScrollView)this).OnMouseMove(e); }
		void IScrollBar.ProcessMouseDown(MouseEventArgs e) { ((IScrollView)this).OnMouseDown(e); }
		void IScrollBar.ProcessMouseUp(MouseEventArgs e) { ((IScrollView)this).OnMouseUp(e); }
		void IScrollBar.ProcessMouseEnter() { ContainsMouse = true; }
		void IScrollBar.ProcessMouseLeave() { ContainsMouse = false;  }
		void IScrollBar.ProcessPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				DoDraw(cache, e.ClipRectangle);
			}
		}
		bool IScrollBar.ContainsMouse() { return ContainsMouse; }
		Control IScrollBar.Parent { get { return Owner; } }
		public virtual ScrollBarDrawMode DrawMode { get { return ScrollBarDrawMode.Desktop; } }
		public XtraScrollableControl Owner { get { return owner; } }
		public virtual Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public virtual bool Enabled { get { return enabled; } set { enabled = value; } }
		public virtual RightToLeft RightToLeft { get { return RightToLeft.No; } }
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		public int Maximum {
			get { return MaximumCore; }
			set {
				if(value != Maximum) {
					if(value < Minimum) {
						Minimum = value;
					}
					MaximumCore = value;
					if(Maximum < Value) {
						Value = Maximum;
					}
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
					if(Minimum > Value) {
						Value = value;
					}
					LayoutChanged();
				}
			}
		}
		public int LargeChange {
			get { return LargeChangeCore; }
			set {
				if(((value > 0) && (value <= ((Maximum - Minimum) + 1))) && (value != LargeChangeCore)) {
					LargeChangeCore = value;
					LayoutChanged();
				}
			}
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
				Bounds = new Rectangle(bounds.X, bounds.Y, value, bounds.Height);
			}
		}
		public int Height {
			get { return bounds.Height; }
			set {
				if(value < 0) return;
				Bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, value);
			}
		}
		public bool Visible { get { return owner.Visible; } set {   } }
		public void Invalidate(Rectangle rect) { Invalidate(); }
		public abstract ScrollBarType ScrollBarType { get; }
		bool containsMouse = false;
		protected bool ContainsMouse {
			get { return containsMouse; }
			set {
				if(ContainsMouse == value) return;
				containsMouse = value;
				Invalidate();
			}
		}
		public void DoDraw(GraphicsCache cache, Rectangle clipRect) {
			Rectangle bounds = Bounds;
			if(TouchMode) bounds.Location = Point.Empty;
			ScrollBarInfoArgs args = new ScrollBarInfoArgs(cache, bounds, style, backStyle, this);
			args.State = Enabled ? ObjectState.Normal : ObjectState.Disabled;
			if(ContainsMouse && Enabled) args.State |= ObjectState.Selected;
			args.State |= ObjectState.Selected;
			Painter.UpdateThumbBounds(args);
			Painter.DrawObject(args);
		}
		public virtual void Invalidate() { Owner.RepaintNcElement(this); }
		public ScrollBarViewInfo ViewInfo { get { return viewInfo; } }
		public void PerformLayout() { LayoutChanged(); }
		public virtual void OnLostCapture() { SetNormalState(); }
		public void OnLostFocus(EventArgs e) { }
		public virtual void OnKeyDown(KeyEventArgs e) {
			if(!e.Handled) {
				if((e.KeyData == Keys.Up) || (e.KeyData == Keys.Left)) {
					((IScrollBar)this).ChangeValueBasedByState(ScrollBarState.DecButtonPressed);
				}
				if((e.KeyData == Keys.Down) || (e.KeyData == Keys.Right)) {
					((IScrollBar)this).ChangeValueBasedByState(ScrollBarState.IncButtonPressed);
				}
				if(e.KeyData == Keys.Prior) {
					((IScrollBar)this).ChangeValueBasedByState(ScrollBarState.DecAreaPressed);
				}
				if(e.KeyData == Keys.Next) {
					((IScrollBar)this).ChangeValueBasedByState(ScrollBarState.IncAreaPressed);
				}
				if(e.KeyData == Keys.Home) {
					Value = Minimum;
				}
				if(e.KeyData == Keys.End) {
					Value = ViewInfo.VisibleMaximum;
				}
			}
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				State = ViewInfo.GetScrollBarState(new Point(e.X, e.Y), true);
				if(State == ScrollBarState.ThumbPressed) {
					beforeThumbDraggingValue = Value;
					ViewInfo.BeforeThumbTracking((ScrollBarType == ScrollBarType.Horizontal) ? e.X : e.Y);
				}
				((IScrollBar)this).ChangeValueBasedByState(State);
				scrollTimer.Enabled = IsScrollingState(State);
				if(scrollTimer.Enabled) {
					scrollTimer.Interval = DefaultFirstScrollInterval;
				}
			}
			RaiseMouseDown(e);
		}
		public virtual void OnMouseLeave(EventArgs e) {
			ViewInfo.UpdateHotState(new Point(-10000, -10000));
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			if(State == ScrollBarState.ThumbPressed) {
				int num1 = (ScrollBarType == ScrollBarType.Horizontal) ? e.X : e.Y;
				if((num1 < (-3 * ViewInfo.ButtonWidth)) || ((num1 - ViewInfo.ScrollBarWidth) > (2 * ViewInfo.ButtonWidth))) {
					SetScrollBarValue(ScrollEventType.ThumbTrack, beforeThumbDraggingValue);
				}
				else {
					int num2 = ViewInfo.GetValueByPos(num1);
					SetScrollBarValue(ScrollEventType.ThumbTrack, num2);
					LayoutChanged();
				}
			}
			ViewInfo.UpdateHotState(new Point(e.X, e.Y));
			RaiseMouseMove(e);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			scrollTimer.Enabled = false;
			SetNormalState();
			ViewInfo.UpdateHotState(new Point(e.X, e.Y));
			RaiseMouseUp(e);
		}
		public virtual void Dispose() {
			if(lookAndFeel != null) lookAndFeel.Dispose();
			scrollTimer.Dispose();
			if(painter != null) painter.DestroyBrushes();
		}
		public event ScrollEventHandler Scroll;
		public event EventHandler ScrollBarAutoSizeChanged;
		public event EventHandler ValueChanged;
		public event MouseEventHandler MouseMove;
		public event MouseEventHandler MouseDown;
		public event MouseEventHandler MouseUp;
		protected ScrollBarViewInfo CreateScrollBarViewInfo() {
			return new ScrollBarViewInfoWithOffset(this);
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
		protected virtual void RaiseMouseMove(MouseEventArgs e) {
			if(MouseMove != null) {
				MouseMove(this, e);
			}
		}
		protected virtual void RaiseMouseDown(MouseEventArgs e) {
			if(MouseDown != null) {
				MouseDown(this, e);
			}
		}
		protected virtual void RaiseMouseUp(MouseEventArgs e) {
			if(MouseUp != null) {
				MouseUp(this, e);
			}
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
		protected virtual void SetNormalState() {
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
		protected virtual ScrollBarState GetScrollBarState() {
			Point mousePosition = Owner.PointToClient(Control.MousePosition);
			if(ScrollBarType == ScrollBarType.Horizontal)
				mousePosition = new Point(mousePosition.X, Owner.Bounds.Height - mousePosition.Y);
			else
				mousePosition = new Point(owner.Bounds.Width - mousePosition.X, mousePosition.Y);
			return ViewInfo.GetScrollBarState(mousePosition, true);
		}
		protected ScrollBarState State {
			get { return ViewInfo.State; }
			set { if(value != State) { ViewInfo.State = value; } }
		}
		bool IsScrollingState(ScrollBarState state) {
			return ((((state == ScrollBarState.IncButtonPressed) || (state == ScrollBarState.DecButtonPressed)) || (state == ScrollBarState.IncAreaPressed)) || (state == ScrollBarState.DecAreaPressed));
		}
		void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			if(painter != null) painter.DestroyBrushes();
			painter = CreateScrollBarPainter();
			ViewInfo.Reset();
			Invalidate();
		}
		void SetDefaultScrollBarHeight() {
			Rectangle bounds = Bounds;
			if(ScrollBarType == ScrollBarType.Horizontal) {
				bounds.Height = SystemInformation.HorizontalScrollBarHeight;
			}
			else {
				bounds.Width = SystemInformation.VerticalScrollBarWidth;
			}
			Bounds = bounds;
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		internal int GetScrollBarValue(int value) {
			if(value < Minimum) { return Minimum; }
			if(value > ViewInfo.VisibleMaximum) { return ViewInfo.VisibleMaximum; }
			return value;
		}
		ScrollBarPainterBase IScrollBar.Painter { get { return Painter; } }
		int IScrollBar.VisibleValue { get { return VisibleValue; } }
		void SetScrollBarValue(ScrollEventType scrollType, int value) {
			value = GetScrollBarValue(value);
			ScrollEventArgs args = new ScrollEventArgs(scrollType, value);
			OnScroll(args);
			Value = args.NewValue;
		}
		protected virtual void LayoutChanged() {
			LayoutChanged(false);
		}
		protected virtual void LayoutChanged(bool allowDelayed) {
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
		void ScrollTimerEventProcessor(Object item, EventArgs e) {
			scrollTimer.Interval = DefaultSecondScrollInterval;
			ScrollBarState state = GetScrollBarState();
			if(state == State)
				((IScrollBar)this).ChangeValueBasedByState(state);
		}
		internal void SetScrollBarValueInternal(int value) {
			ValueCore = value;
		}
		#region IScrollBar Members
		public int GetWidth() { return Width; }
		public int GetHeight() { return Height; }
		public bool GetEnabled() { return Enabled; }
		#endregion
	}
	public abstract class HScrollBarViewInfoWithHandlerBase : ScrollBarViewInfoWithHandlerBase {
		protected HScrollBarViewInfoWithHandlerBase(XtraScrollableControl owner)
			: base(owner) {
		}
		public override ScrollBarType ScrollBarType { get { return ScrollBarType.Horizontal; } }
	}
	public abstract class VScrollBarViewInfoWithHandlerBase : ScrollBarViewInfoWithHandlerBase {
		protected VScrollBarViewInfoWithHandlerBase(XtraScrollableControl owner)
			: base(owner) {
		}
		public override ScrollBarType ScrollBarType { get { return ScrollBarType.Vertical; } }
	}
	public class HScrollBarViewInfoWithHandler : HScrollBarViewInfoWithHandlerBase {
		int largeChangeCore;
		int maximumCore;
		int minimumCore;
		int smallChangeCore;
		int valueCore;
		public HScrollBarViewInfoWithHandler(XtraScrollableControl owner)
			: base(owner) {
			this.largeChangeCore = 10;
			this.maximumCore = 100;
			this.minimumCore = 0;
			this.smallChangeCore = 1;
			this.valueCore = minimumCore;
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
			return Owner.HorizontalScroll.LargeChange;
		}
		protected override int GetActualSmallChange() {
			return Owner.HorizontalScroll.SmallChange;
		}
	}
	public class VScrollBarViewInfoWithHandler : VScrollBarViewInfoWithHandlerBase {
		int largeChangeCore;
		int maximumCore;
		int minimumCore;
		int smallChangeCore;
		int valueCore;
		public VScrollBarViewInfoWithHandler(XtraScrollableControl owner)
			: base(owner) {
			this.largeChangeCore = 10;
			this.maximumCore = 100;
			this.minimumCore = 0;
			this.smallChangeCore = 1;
			this.valueCore = this.minimumCore;
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
			return Owner.VerticalScroll.LargeChange;
		}
		protected override int GetActualSmallChange() {
			return Owner.VerticalScroll.SmallChange;
		}
	}
	public class ScrollBarViewInfoWithOffset : ScrollBarViewInfo {
		ScrollBarViewInfoWithHandlerBaseOld scrollBar;
		public ScrollBarViewInfoWithOffset(ScrollBarViewInfoWithHandlerBaseOld scroll)
			: base(scroll) {
			this.scrollBar = scroll;
		}
		public virtual int OffsetX { get { return scrollBar.TouchMode ? 0 :  scrollBar.Bounds.X; } }
		public virtual int OffsetY { get { return scrollBar.TouchMode ? 0 : scrollBar.Bounds.Y; } }
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
			return base.GetHitTest(new Point(pt.X + OffsetX, pt.Y + OffsetY));
		}
	} 
}
