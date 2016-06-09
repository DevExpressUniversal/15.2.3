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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core {
	[StyleTypedProperty(Property = "HorizontalScrollBarStyle", StyleTargetType = typeof(ScrollBar))]
	[StyleTypedProperty(Property = "VerticalScrollBarStyle", StyleTargetType = typeof(ScrollBar))]
	[StyleTypedProperty(Property = "CornerBoxStyle", StyleTargetType = typeof(CornerBox))]
	public abstract class ScrollControl : PanelBase, IScrollControl {
		public static int AnimatedScrollingDuration = 500;
		#region Dependency Properties
		public static readonly DependencyProperty AllowBringChildIntoViewProperty =
			DependencyProperty.Register("AllowBringChildIntoView", typeof(bool), typeof(ScrollControl), new PropertyMetadata(true));
		public static readonly DependencyProperty AnimateScrollingProperty =
			DependencyProperty.Register("AnimateScrolling", typeof(bool), typeof(ScrollControl),
				new PropertyMetadata(true, (o, e) => ((ScrollControl)o).Controller.AnimateScrolling = (bool)e.NewValue));
		public static readonly DependencyProperty DragScrollingProperty =
			DependencyProperty.Register("DragScrolling", typeof(bool), typeof(ScrollControl),
				new PropertyMetadata(true, (o, e) => ((ScrollControl)o).Controller.DragScrolling = (bool)e.NewValue));
		public static readonly DependencyProperty HorizontalOffsetProperty =
			DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ScrollControl),
				new PropertyMetadata((o, e) => ((ScrollControl)o).OnOffsetChanged(true, (double)e.OldValue, (double)e.NewValue)));
		public static readonly DependencyProperty VerticalOffsetProperty =
			DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ScrollControl),
				new PropertyMetadata((o, e) => ((ScrollControl)o).OnOffsetChanged(false, (double)e.OldValue, (double)e.NewValue)));
		public static readonly DependencyProperty HorizontalScrollBarStyleProperty =
			DependencyProperty.Register("HorizontalScrollBarStyle", typeof(Style), typeof(ScrollControl),
				new PropertyMetadata((o, e) => ((ScrollControl)o).Controller.HorzScrollBarStyle = (Style)e.NewValue));
		public static readonly DependencyProperty VerticalScrollBarStyleProperty =
			DependencyProperty.Register("VerticalScrollBarStyle", typeof(Style), typeof(ScrollControl),
				new PropertyMetadata((o, e) => ((ScrollControl)o).Controller.VertScrollBarStyle = (Style)e.NewValue));
		public static readonly DependencyProperty CornerBoxStyleProperty =
			DependencyProperty.Register("CornerBoxStyle", typeof(Style), typeof(ScrollControl),
				new PropertyMetadata((o, e) => ((ScrollControl)o).Controller.CornerBoxStyle = (Style)e.NewValue));
		public static readonly DependencyProperty ScrollBarsProperty =
			DependencyProperty.Register("ScrollBars", typeof(ScrollBars), typeof(ScrollControl),
				new PropertyMetadata(ScrollBars.Auto, (o, e) => ((ScrollControl)o).Controller.ScrollBars = (ScrollBars)e.NewValue));
		#endregion Dependency Properties
#if !SILVERLIGHT
		static ScrollControl() {
			EventManager.RegisterClassHandler(typeof(ScrollControl), FrameworkElement.RequestBringIntoViewEvent,
				new RequestBringIntoViewEventHandler((o, e) => ((ScrollControl)o).OnRequestBringIntoView(e)));
		}
#endif
		public bool BringChildIntoView(FrameworkElement child, bool allowAnimation = false) {
			if (!Controller.IsScrollable() || child == null || child == this || !child.FindIsInParents(this) || !child.IsInVisualTree() ||
				IsInternalElement(child))
				return false;
#if !SILVERLIGHT
			if (!IsArrangeValid)
#endif
			UpdateLayout();
			if (!child.IsInVisualTree())
				return false;
			bool result = false;
			Rect bounds = ContentBounds;
			Rect childBounds = child.GetBounds(this);
			if (childBounds.Left < bounds.Left || childBounds.Right > bounds.Right) {
				result = true;
				double offset;
				if (childBounds.Left < bounds.Left)
					offset = -(bounds.Left - childBounds.Left);
				else
					offset = Math.Min(childBounds.Right - bounds.Right, childBounds.Left - bounds.Left);
				Controller.Scroll(Orientation.Horizontal, HorizontalOffset + offset, allowAnimation, false);
			}
			if (childBounds.Top < bounds.Top || childBounds.Bottom > bounds.Bottom) {
				result = true;
				double offset;
				if (childBounds.Top < bounds.Top)
					offset = -(bounds.Top - childBounds.Top);
				else
					offset = Math.Min(childBounds.Bottom - bounds.Bottom, childBounds.Top - bounds.Top);
				Controller.Scroll(Orientation.Vertical, VerticalOffset + offset, allowAnimation, false);
			}
			return result;
		}
		public void SetOffset(Point offset) {
			HorizontalOffset = offset.X;
			VerticalOffset = offset.Y;
		}
		public bool AllowBringChildIntoView {
			get { return (bool)GetValue(AllowBringChildIntoViewProperty); }
			set { SetValue(AllowBringChildIntoViewProperty, value); }
		}
		public bool AnimateScrolling {
			get { return (bool)GetValue(AnimateScrollingProperty); }
			set { SetValue(AnimateScrollingProperty, value); }
		}
		public new ScrollControlController Controller { get { return (ScrollControlController)base.Controller; } }
		public bool DragScrolling {
			get { return (bool)GetValue(DragScrollingProperty); }
			set { SetValue(DragScrollingProperty, value); }
		}
		public double HorizontalOffset {
			get { return (double)GetValue(HorizontalOffsetProperty); }
			set { SetValue(HorizontalOffsetProperty, value); }
		}
		public double VerticalOffset {
			get { return (double)GetValue(VerticalOffsetProperty); }
			set { SetValue(VerticalOffsetProperty, value); }
		}
		public Point Offset { get { return new Point(HorizontalOffset, VerticalOffset); } }
		public Style HorizontalScrollBarStyle {
			get { return (Style)GetValue(HorizontalScrollBarStyleProperty); }
			set { SetValue(HorizontalScrollBarStyleProperty, value); }
		}
		public Style VerticalScrollBarStyle {
			get { return (Style)GetValue(VerticalScrollBarStyleProperty); }
			set { SetValue(VerticalScrollBarStyleProperty, value); }
		}
		public Style CornerBoxStyle {
			get { return (Style)GetValue(CornerBoxStyleProperty); }
			set { SetValue(CornerBoxStyleProperty, value); }
		}
		public Size ScrollAreaSize { get { return Controller.ScrollAreaSize; } }
		public ScrollBars ScrollBars {
			get { return (ScrollBars)GetValue(ScrollBarsProperty); }
			set { SetValue(ScrollBarsProperty, value); }
		}
		#region Children
		protected virtual FrameworkElement GetChildContainer(FrameworkElement child) {
			return child;
		}
		#endregion Children
		protected override PanelControllerBase CreateController() {
			return new ScrollControlController(this);
		}
#if SILVERLIGHT
		protected override void AttachToEvents() {
			base.AttachToEvents();
			GotFocus += (sender, e) => OnGotFocus(e);
		}
		protected virtual void OnGotFocus(RoutedEventArgs e) {
			BringChildIntoView(GetChildContainer(e.OriginalSource as FrameworkElement), true);
		}
#endif
		protected virtual void OnOffsetChanged(bool isHorizontal, double oldValue, double newValue) {
			InvalidateArrange();
		}
#if !SILVERLIGHT
		protected virtual void OnRequestBringIntoView(RequestBringIntoViewEventArgs e) {
			if (!AllowBringChildIntoView)
				return;
			var child = e.TargetObject as FrameworkElement;
			if (child == null)
				return;
			if (BringChildIntoView(GetChildContainer(child), true))
				e.Handled = true;
		}
#endif
	}
	public interface IScrollControl : IPanel {
		void SetOffset(Point offset);
		double HorizontalOffset { get; set; }
		double VerticalOffset { get; set; }
		Point Offset { get; }
	}
	public class ScrollControlController : PanelControllerBase {
		public ScrollControlController(IScrollControl control) : base(control) {
			AccumulateOffsetValuesDuringAnimatedScrolling = true;
			AnimateScrolling = true;
			DragScrolling = true;
			ScrollBars = ScrollBars.Auto;
		}
		public IScrollControl IScrollControl { get { return IControl as IScrollControl; } }
		#region Scrolling
		public override bool IsScrollable() {
			return true;
		}
		public void Scroll(Orientation orientation, double position, bool animate = false, bool accumulateOffsetValuesDuringAnimation = true) {
			if (!IsScrollable())
				return;
			AccumulateOffsetValuesDuringAnimatedScrolling = accumulateOffsetValuesDuringAnimation;
			CanAnimateScrolling = animate;
			try {
				GetScrollParams(orientation).Scroll(position, false);
			}
			finally {
				CanAnimateScrolling = false;
				AccumulateOffsetValuesDuringAnimatedScrolling = true;
			}
		}
		public bool AnimateScrolling { get; protected internal set; }
		public bool DragScrolling { get; protected internal set; }
		public Size ScrollAreaSize { get { return IPanel.ChildrenBounds.Size(); } }
		protected override void InitScrollParams(ScrollParams horzScrollParams, ScrollParams vertScrollParams) {
			horzScrollParams.Max = ScrollAreaSize.Width;
			horzScrollParams.PageSize = ContentBounds.Width;
			horzScrollParams.Position = IScrollControl.HorizontalOffset;
			vertScrollParams.Max = ScrollAreaSize.Height;
			vertScrollParams.PageSize = ContentBounds.Height;
			vertScrollParams.Position = IScrollControl.VerticalOffset;
		}
		protected override bool Scroll(Orientation orientation, double position) {
			double oldOffset = GetOffset(orientation);
			bool allowAnimation = CanAnimateScrolling && AnimateScrolling;
			if (allowAnimation && AnimatedScrollingStoryboard != null) {
				AnimatedScrollingStoryboard.Stop();
#if !SILVERLIGHT
				Control.BeginAnimation(GetOffsetProperty(orientation), null);
#endif
				AnimatedScrollingStoryboard = null;
				if (AccumulateOffsetValuesDuringAnimatedScrolling) {
					double offset = GetOffset(orientation);
					int oldDirection = Math.Sign(offset - oldOffset);
					int newDirection = Math.Sign(position - oldOffset);
					if (newDirection == oldDirection)
						position += offset - oldOffset;
				}
			}
			bool isDragScrolling = IsDragScrolling || DragScrollingController.IsInertialScrolling(Control);
			ScrollParams scrollParams = GetScrollParams(orientation);
			if (isDragScrolling && scrollParams.Enabled)
				return false;
			position = Math.Max(scrollParams.Min, Math.Min(position, scrollParams.MaxPosition));
			if (GetOffset(orientation) != position)
				SetOffset(orientation, position);
			double newOffset = GetOffset(orientation);
			if (allowAnimation) {
				AnimatedScrollingStoryboard = new Storyboard();
				AnimatedScrollingStoryboard.Children.Add(
					CreateAnimatedScrollingAnimation(orientation, oldOffset, newOffset, ScrollControl.AnimatedScrollingDuration,
						easingFunction: new QuadraticEase()));
				AnimatedScrollingStoryboard.FillBehavior = FillBehavior.Stop;
				AnimatedScrollingStoryboard.Completed += (o, e) => AnimatedScrollingStoryboard = null;
				AnimatedScrollingStoryboard.Begin();
				return false;
			}
			else
				return newOffset != oldOffset;
		}
		protected internal virtual Timeline CreateAnimatedScrollingAnimation(Orientation orientation, double oldOffset, double newOffset,
			int duration, double finalOffset = double.NaN, int finalOffsetDuration = 0, IEasingFunction easingFunction = null) {
			var animation = new DoubleAnimationUsingKeyFrames();
			Storyboard.SetTarget(animation, Control);
			Storyboard.SetTargetProperty(animation, new PropertyPath(GetOffsetPropertyName(orientation)));
			animation.KeyFrames.Add(
				new LinearDoubleKeyFrame {
					KeyTime = TimeSpan.Zero,
					Value = oldOffset
				});
			animation.KeyFrames.Add(
				new EasingDoubleKeyFrame {
					KeyTime = TimeSpan.FromMilliseconds(duration),
					Value = newOffset,
					EasingFunction = easingFunction
				});
			if (!double.IsNaN(finalOffset))
				animation.KeyFrames.Add(
					new EasingDoubleKeyFrame {
						KeyTime = TimeSpan.FromMilliseconds(duration + finalOffsetDuration),
						Value = finalOffset,
						EasingFunction = easingFunction
					});
			animation.Duration = TimeSpan.FromMilliseconds(duration + finalOffsetDuration);
			return animation;
		}
		protected double GetOffset(Orientation orientation) {
			return orientation == Orientation.Horizontal ? IScrollControl.HorizontalOffset : IScrollControl.VerticalOffset;
		}
#if !SILVERLIGHT
		protected DependencyProperty GetOffsetProperty(Orientation orientation) {
			return orientation == Orientation.Horizontal ? ScrollControl.HorizontalOffsetProperty : ScrollControl.VerticalOffsetProperty;
		}
#endif
		protected string GetOffsetPropertyName(Orientation orientation) {
			return orientation == Orientation.Horizontal ? "HorizontalOffset" : "VerticalOffset";
		}
		protected void SetOffset(Orientation orientation, double value) {
			if (orientation == Orientation.Horizontal)
				IScrollControl.HorizontalOffset = value;
			else
				IScrollControl.VerticalOffset = value;
		}
		protected bool AccumulateOffsetValuesDuringAnimatedScrolling { get; set; }
		protected Storyboard AnimatedScrollingStoryboard { get; private set; }
		protected bool IsDragScrolling { get { return IsDragAndDrop && DragAndDropController is DragScrollingController; } }
		#endregion Scrolling
		#region Drag&Drop
		protected override bool WantsDragAndDrop(Point p, out DragAndDropController controller) {
			if (CanScroll() && DragScrolling && ScrollableAreaBounds.Contains(p)) {
				controller = new DragScrollingController(this, p);
				return true;
			}
			else
				return base.WantsDragAndDrop(p, out controller);
		}
		#endregion Drag&Drop
	}
	public class DragScrollingController : DragAndDropController {
		private static Dictionary<FrameworkElement, Storyboard> InertialScrollingAnimations = new Dictionary<FrameworkElement, Storyboard>();
		public static bool IsInertialScrolling(FrameworkElement element) {
			return InertialScrollingAnimations.ContainsKey(element);
		}
		public DragScrollingController(Controller controller, Point startDragPoint) : base(controller, startDragPoint) {
			OriginalOffset = Controller.IScrollControl.Offset;
			StopInertialScrolling();
		}
		public override void DragAndDrop(Point p) {
			base.DragAndDrop(p);
			ChangeOffset(PointHelper.Subtract(StartDragPoint, p));
		}
		public override void EndDragAndDrop(bool accept) {
			base.EndDragAndDrop(accept);
			if (Controller.AnimateScrolling)
				StartInertialScrolling();
			else
				Controller.UpdateScrolling(); 
		}
		protected void ChangeOffset(Point by) {
			CorrectOffsetChange(OriginalOffset, ref by, new Point(GetMaxOffsetExcess(by.X), GetMaxOffsetExcess(by.Y)));
			if (OffsetChanges == null)
				OffsetChanges = new Point[3];
			OffsetChanges[0] = OffsetChanges[1];
			OffsetChanges[1] = OffsetChanges[2];
			OffsetChanges[2] = by;
			Controller.IScrollControl.SetOffset(PointHelper.Add(OriginalOffset, by));
		}
		protected void CorrectOffsetChange(Point offset, ref Point offsetChange, Point maxOffsetExcess) {
			if (!Controller.HorzScrollParams.Enabled)
				maxOffsetExcess.X = 0;
			if (!Controller.VertScrollParams.Enabled)
				maxOffsetExcess.Y = 0;
			var minOffset = new Point(Controller.HorzScrollParams.Min, Controller.VertScrollParams.Min);
			var maxOffset = new Point(Controller.HorzScrollParams.MaxPosition, Controller.VertScrollParams.MaxPosition);
			Point newOffset = PointHelper.Add(offset, offsetChange);
			newOffset = PointHelper.Max(PointHelper.Subtract(minOffset, maxOffsetExcess), newOffset);
			newOffset = PointHelper.Min(newOffset, PointHelper.Add(maxOffset, maxOffsetExcess));
			offsetChange = PointHelper.Subtract(newOffset, offset);
		}
		protected virtual void CreateInertialScrollingAnimation(Storyboard storyboard, Orientation orientation, double offset, double newOffset, int duration) {
			const int ReturnFromExcessiveOffsetDuration = 300;
			const int ReturnFromExcessiveOffsetAnimationDuration = 150;
			if (IsOffsetExcessive(orientation, offset))
				storyboard.Children.Add(
					Controller.CreateAnimatedScrollingAnimation(orientation, offset, GetOffsetExcessBoundary(orientation, offset),
						ReturnFromExcessiveOffsetDuration, easingFunction: new QuadraticEase { EasingMode = EasingMode.EaseOut }));
			else {
				double finalOffset = double.NaN;
				int finalOffsetDuration = 0;
				if (IsOffsetExcessive(orientation, newOffset)) {
					finalOffset = GetOffsetExcessBoundary(orientation, newOffset);
					finalOffsetDuration = ReturnFromExcessiveOffsetAnimationDuration;
				}
				storyboard.Children.Add(
					Controller.CreateAnimatedScrollingAnimation(orientation, offset, newOffset,
						duration, finalOffset, finalOffsetDuration, new QuadraticEase { EasingMode = EasingMode.EaseOut }));
			}
		}
		protected virtual void GetInertialScrollingParams(out Point offsetChange, out int duration) {
			const int MinDuration = 300;
			const double MinOffsetChange = 150;
			const double OffsetChangeMultiplier = 0.015;
			const double OffsetExcess = 150;
			if (OffsetChanges == null) {
				offsetChange = new Point();
				duration = 0;
				return;
			}
			offsetChange = PointHelper.Add(
				PointHelper.Multiply(PointHelper.Subtract(OffsetChanges[1], OffsetChanges[0]), 1.0 / 3.0),
				PointHelper.Multiply(PointHelper.Subtract(OffsetChanges[2], OffsetChanges[1]), 2.0 / 3.0));
			if (Math.Abs(offsetChange.X) < 5 && Math.Abs(offsetChange.Y) < 5) {
				offsetChange = new Point();
				duration = 0;
				return;
			}
			Rect contentBounds = Controller.IScrollControl.ContentBounds;
			offsetChange = PointHelper.Multiply(offsetChange,
				new Point(OffsetChangeMultiplier * contentBounds.Width, OffsetChangeMultiplier * contentBounds.Height));
			offsetChange = PointHelper.Multiply(PointHelper.Sign(offsetChange),
				PointHelper.Max(new Point(MinOffsetChange, MinOffsetChange), PointHelper.Abs(offsetChange)));
			CorrectOffsetChange(Controller.IScrollControl.Offset, ref offsetChange, new Point(OffsetExcess, OffsetExcess));
			double speed = 1 + Math.Pow(Math.Max(Math.Abs(offsetChange.X), Math.Abs(offsetChange.Y)) - 150, 2) / 2000000;
			duration = Math.Max(MinDuration, (int)Math.Max(Math.Abs(offsetChange.X) / speed, Math.Abs(offsetChange.Y) / speed));
		}
		protected double GetMinOffset(Orientation orientation) {
			return orientation == Orientation.Horizontal ? Controller.HorzScrollParams.Min : Controller.VertScrollParams.Min;
		}
		protected double GetMaxOffset(Orientation orientation) {
			return orientation == Orientation.Horizontal ? Controller.HorzScrollParams.MaxPosition : Controller.VertScrollParams.MaxPosition;
		}
		protected virtual double GetMaxOffsetExcess(double offsetChange) {
			return 600 * Math.Log10(1 + Math.Abs(offsetChange) / 200);
		}
		protected double GetOffsetExcessBoundary(Orientation orientation, double offset) {
			return offset < GetMinOffset(orientation) ? GetMinOffset(orientation) : GetMaxOffset(orientation);
		}
		protected bool IsOffsetExcessive(Orientation orientation, double offset) {
			return offset < GetMinOffset(orientation) || offset > GetMaxOffset(orientation);
		}
		protected void StartInertialScrolling() {
			Point offsetChange;
			int duration;
			GetInertialScrollingParams(out offsetChange, out duration);
			Point offset = Controller.IScrollControl.Offset;
			Point newOffset = PointHelper.Add(offset, offsetChange);
			Storyboard storyboard = new Storyboard();
			if (Controller.HorzScrollParams.Enabled)
				CreateInertialScrollingAnimation(storyboard, Orientation.Horizontal, offset.X, newOffset.X, duration);
			if (Controller.VertScrollParams.Enabled)
				CreateInertialScrollingAnimation(storyboard, Orientation.Vertical, offset.Y, newOffset.Y, duration);
			storyboard.Completed += (o, e) => StopInertialScrolling();
			InertialScrollingAnimations[Controller.Control] = storyboard;
			storyboard.Begin();
		}
		protected void StopInertialScrolling() {
			if (!IsInertialScrolling(Controller.Control))
				return;
			Point offset = Controller.IScrollControl.Offset;
			InertialScrollingAnimations[Controller.Control].Stop();
			InertialScrollingAnimations.Remove(Controller.Control);
			Controller.IScrollControl.SetOffset(offset);
		}
		protected new ScrollControlController Controller { get { return (ScrollControlController)base.Controller; } }
		protected Point[] OffsetChanges { get; private set; }
		protected Point OriginalOffset { get; private set; }
	}
}
