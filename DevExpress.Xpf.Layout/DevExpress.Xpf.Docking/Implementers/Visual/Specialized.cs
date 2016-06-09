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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Base;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Docking.VisualElements {
	[Core.DXToolboxBrowsable(false)]
	public class DockBarContainerControlBase : BarContainerControl {
		internal DockBarContainerControlBase() {
			Unloaded += OnUnloaded;	  
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			OnUnloaded();
		}
		protected sealed override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			OnLoaded();
		}
		protected virtual void OnLoaded() { }
		protected virtual void OnUnloaded() { }
		[Obsolete("Use BarManager.Bars property instead.")]
		new public ItemCollection Items { get { return null; } }
	}
	[Core.DXToolboxBrowsable(false)]
	public class DockBarContainerControl : DockBarContainerControlBase, IDisposable {
		#region static
		public static readonly DependencyProperty BindableNameProperty;
		public static readonly DependencyProperty BindableAllowDropProperty;
		public static readonly DependencyProperty LayoutItemProperty;
		static DockBarContainerControl() {
			var dProp = new DependencyPropertyRegistrator<DockBarContainerControl>();
			dProp.Register("BindableName", ref BindableNameProperty, (string)null, OnBindableNameChanged);
			dProp.Register("BindableAllowDrop", ref BindableAllowDropProperty, (bool?)null, OnBindableAllowDropChanged);			
			dProp.Register("LayoutItem", ref LayoutItemProperty, (BaseLayoutItem)null,
				(dObj, e) => ((DockBarContainerControl)dObj).OnLayoutItemChanged((BaseLayoutItem)e.NewValue));
		}
		static void OnBindableNameChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((FrameworkElement)dObj).Name = (string)e.NewValue;
		}
		static void OnBindableAllowDropChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((FrameworkElement)dObj).AllowDrop = CalcAllowDrop((bool?)e.NewValue);
		}
		static bool CalcAllowDrop(bool? value) {
			return value.HasValue && value.Value;
		}
		#endregion static
		public DockBarContainerControl() {
			AllowDrop = CalcAllowDrop(BindableAllowDrop);
			Focusable = false;
		}
		protected override void OnUnloaded() {
			BarManagerPropertyHelper.ClearBarManager(this);
		}
		protected bool IsDisposing { get; private set; }
		public void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				OnDispose();
				Container = null;
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDispose() {
			BarManagerPropertyHelper.ClearBarManager(this);
		}
		protected override void OnLoaded() {			
		}
		public string BindableName {
			get { return (string)GetValue(BindableNameProperty); }
			set { SetValue(BindableNameProperty, value); }
		}
		public bool? BindableAllowDrop {
			get { return (bool?)GetValue(BindableAllowDropProperty); }
			set { SetValue(BindableAllowDropProperty, value); }
		}
		public BaseLayoutItem LayoutItem {
			get { return (BaseLayoutItem)GetValue(LayoutItemProperty); }
			set { SetValue(LayoutItemProperty, value); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Container = DockLayoutManager.FindManager(this);
		}		
		protected virtual void OnLayoutItemChanged(BaseLayoutItem item) {
			if(item != null) {
				BindingHelper.SetBinding(this, BindableAllowDropProperty, item, "HeaderBarContainerControlAllowDrop");
				BindingHelper.SetBinding(this, BindableNameProperty, item, "HeaderBarContainerControlName");
			}
			else {
				BindingHelper.ClearBinding(this, BindableAllowDropProperty);
				BindingHelper.ClearBinding(this, BindableNameProperty);
			}
		}	   
		public DockLayoutManager Container { get; private set; }
		void EnsureBarManager() {
			if(Manager == null) {
				Container = DockLayoutManager.FindManager(this);
				if(Container != null)
					BarManager.SetBarManager(this, Container.CustomizationController.BarManager);
			}
		}		
	}
	[Core.DXToolboxBrowsable(false)]
	public abstract class BaseHeadersPanel : psvPanel {
		#region static
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty TabHeaderLayoutTypeProperty;
		public static readonly DependencyProperty IsAutoFillHeadersProperty;
		public static readonly DependencyProperty ScrollOffsetProperty;
		public static readonly DependencyProperty ScrollIndexProperty;
		public static readonly DependencyProperty ClipMarginProperty;
		public static readonly DependencyProperty TransparencySizeProperty;
		static BaseHeadersPanel() {
			var dProp = new DependencyPropertyRegistrator<BaseHeadersPanel>();
			dProp.OverrideMetadataNotDataBindable(IsItemsHostProperty, true);
			dProp.Register("ClipMargin", ref ClipMarginProperty, new Thickness(0, -2, 0, -2));
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal,
				(dObj, e) => ((BaseHeadersPanel)dObj).OnOrientationChanged((Orientation)e.NewValue));
			dProp.Register("TabHeaderLayoutType", ref TabHeaderLayoutTypeProperty, TabHeaderLayoutType.Default,
				(dObj, e) => ((BaseHeadersPanel)dObj).OnTabHeaderLayoutChanged((TabHeaderLayoutType)e.NewValue));
			dProp.Register("IsAutoFillHeaders", ref IsAutoFillHeadersProperty, false,
				(dObj, e) => ((BaseHeadersPanel)dObj).OnIsAutoFillHeadersChanged((bool)e.NewValue));
			dProp.Register("ScrollOffset", ref ScrollOffsetProperty, 0d,
				(dObj, e) => ((BaseHeadersPanel)dObj).OnScrollOffsetChanged((double)e.NewValue));
			dProp.Register("ScrollIndex", ref ScrollIndexProperty, 0,
				(dObj, e) => ((BaseHeadersPanel)dObj).OnScrollIndexChanged((int)e.NewValue, (int)e.OldValue));
			dProp.Register("TransparencySize", ref TransparencySizeProperty, 7d);
		}
		#endregion static
		public BaseHeadersPanel() {
			Background = FreezedBrushes.Transparent;
			DockPane.SetHitTestType(this, HitTestType.PageHeaders);
		}
		public TabHeaderLayoutType TabHeaderLayoutType {
			get { return (TabHeaderLayoutType)GetValue(TabHeaderLayoutTypeProperty); }
			set { SetValue(TabHeaderLayoutTypeProperty, value); }
		}
		public bool IsAutoFillHeaders {
			get { return (bool)GetValue(IsAutoFillHeadersProperty); }
			set { SetValue(IsAutoFillHeadersProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public int ScrollIndex {
			get { return (int)GetValue(ScrollIndexProperty); }
			set { SetValue(ScrollIndexProperty, value); }
		}
		public double ScrollOffset {
			get { return (double)GetValue(ScrollOffsetProperty); }
			set { SetValue(ScrollOffsetProperty, value); }
		}
		public Thickness ClipMargin {
			get { return (Thickness)GetValue(ClipMarginProperty); }
			set { SetValue(ClipMarginProperty, value); }
		}
		public double TransparencySize {
			get { return (double)GetValue(TransparencySizeProperty); }
			set { SetValue(TransparencySizeProperty, value); }
		}
		public bool IsHorizontal {
			get { return Orientation == Orientation.Horizontal; }
		}
		OrientationHelper _OrientationHelper { get { return OrientationHelper.GetInstance(IsHorizontal); } }
		bool canAnimate;
		bool hasScroll;
		bool isScrollNext;
		protected virtual void OnScrollOffsetChanged(double scrollOffset) {
			InvalidateArrange();
		}
		protected virtual void OnIsAutoFillHeadersChanged(bool isAutoFill) {
			InvalidateMeasure();
			InvalidateArrange();
		}
		protected virtual void OnTabHeaderLayoutChanged(TabHeaderLayoutType type) {
			InvalidateMeasure();
			InvalidateArrange();
		}
		protected virtual void OnOrientationChanged(Orientation orientation) {
			InvalidateMeasure();
		}
		protected virtual void OnScrollIndexChanged(int index, int oldIndex) {
			canAnimate = true;
			isScrollNext = index > oldIndex;
			InvalidateMeasure();
		}
		protected virtual ITabHeaderLayoutCalculator GetCalculator(TabHeaderLayoutType type) {
			return HeaderLayoutCalculatorFactory.GetCalculator(type);
		}
		protected virtual ITabHeaderLayoutOptions GetOptions(Size availableSize) {
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(this);
			bool selectedRowFirst = (item != null) && ((item.CaptionLocation == CaptionLocation.Right) || (item.CaptionLocation == CaptionLocation.Bottom));
			return new TabHeaderOptions(availableSize, IsHorizontal, IsAutoFillHeaders, ScrollIndex, selectedRowFirst);
		}
		protected override Size MeasureOverride(Size availableSize) {
			base.MeasureOverride(availableSize);
			ITabHeaderLayoutCalculator calculator = GetCalculator(TabHeaderLayoutType);
			ITabHeaderLayoutOptions options = GetOptions(availableSize);
			ITabHeaderLayoutResult result = Measure(calculator, options);
			LayoutGroup group = DockLayoutManager.GetLayoutItem(this) as LayoutGroup;
			hasScroll = false;
			if(group != null && !result.IsEmpty) {
				hasScroll = group.TabHeaderHasScroll = result.HasScroll;
				if(result.HasScroll) {
					group.TabHeaderMaxScrollIndex = result.ScrollResult.MaxIndex;
					group.TabHeaderScrollIndex = result.ScrollResult.Index;
					group.TabHeaderCanScrollPrev = result.ScrollResult.CanScrollPrev;
					group.TabHeaderCanScrollNext = result.ScrollResult.CanScrollNext;
					if(canAnimate) {
						canAnimate = false;
						StartScrollAnimation(result, group);
					}
				}
			}
			return result.Size;
		}
		protected virtual ITabHeaderLayoutResult Measure(ITabHeaderLayoutCalculator calculator, ITabHeaderLayoutOptions options) {
			return HeadersPanelHelper.Measure(InternalChildren, calculator, options);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			bool horz = IsHorizontal;
			foreach(UIElement element in InternalChildren) {
				ITabHeader header = element as ITabHeader;
				if(header != null) {
					Rect rect = header.ArrangeRect;
					if(!rect.IsEmpty) {
						if(!header.IsPinned)
							RectHelper.Offset(ref rect, horz ? ScrollOffset : 0, horz ? 0 : ScrollOffset);
						element.Arrange(rect);
					}
					else element.Arrange(new Rect(0, 0, 0, 0));
				}
			}
			UpdateOpacityMask(finalSize);
			return finalSize;
		}
		void ResetOpacityMask(UIElement element) {
			element.OpacityMask = null;
			element.Opacity = 1;
			element.IsHitTestVisible = true;
		}
		void UpdateOpacityMask(Size finalSize) {
			double pinned = 0;
			double nearOffset = 0;
			var orientationHelper = _OrientationHelper;
			double farOffset = orientationHelper.GetLength(finalSize);
			foreach(UIElement element in InternalChildren) {
				ResetOpacityMask(element);
				ITabHeader tabHeader = element as ITabHeader;
				if(tabHeader != null && tabHeader.IsPinned) {
					Rect headerRect = tabHeader.ArrangeRect;
					if(!headerRect.IsEmpty) {
						var length = orientationHelper.GetLength(headerRect);
						if(tabHeader.PinLocation == TabHeaderPinLocation.Far)
							farOffset = Math.Min(farOffset, orientationHelper.GetValue(headerRect.Location));
						else
							nearOffset = Math.Max(nearOffset, orientationHelper.GetValue(headerRect.BottomRight));
						pinned += length;
					}
				}
			}
			if(pinned > 0 && hasScroll) {
				Point avLocation = orientationHelper.GetPoint(nearOffset, 0);
				Size avSize = orientationHelper.GetSize(
					(farOffset > nearOffset ? farOffset : orientationHelper.GetLength(finalSize)) - nearOffset,
					orientationHelper.GetDim(finalSize));
				Rect avRect = new Rect(avLocation, avSize);
				UpdateOpacityMask(avRect);
			}
		}
		void UpdateOpacityMask(Rect available) {
			bool isHorz = IsHorizontal;
			var orientationHelper = _OrientationHelper;
			foreach(UIElement element in InternalChildren) {
				ITabHeader tabHeader = element as ITabHeader;
				if(tabHeader != null) {
					Rect headerRect = tabHeader.ArrangeRect;
					if(!headerRect.IsEmpty && !tabHeader.IsPinned) {
						RectHelper.Offset(ref headerRect, IsHorizontal ? ScrollOffset : 0, isHorz ? 0 : ScrollOffset);
						var intersectionRect = Rect.Intersect(headerRect, available);
						var intersect = new Interval(orientationHelper.GetValue(intersectionRect.Location), orientationHelper.GetLength(intersectionRect));
						var rect = new Interval(orientationHelper.GetValue(headerRect.Location), orientationHelper.GetLength(headerRect));
						var rectLength = rect.Length;
						if(intersect.Length == rectLength) continue;
						if(intersectionRect.IsEmpty || intersect.Length < TransparencySize) {
							element.Opacity = 0;
							element.IsHitTestVisible = false;
							continue;
						}
						var leftTransparent = new Interval(0, intersect.Start - rect.Start);
						var leftSemiTransparent = new Interval(leftTransparent.End, TransparencySize);
						var rightTransparent = new Interval(intersect.End - rect.Start, rect.End - intersect.End);
						var rightSemiTransparent = new Interval(rightTransparent.Start - TransparencySize, TransparencySize);
						LinearGradientBrush opacityBrush = new LinearGradientBrush()
						{
							StartPoint = orientationHelper.GetPoint(0, 0.5),
							EndPoint = orientationHelper.GetPoint(1, 0.5)
						};
						if(intersect.Start != rect.Start) {
							opacityBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 0));
							opacityBrush.GradientStops.Add(new GradientStop(Colors.Transparent, leftTransparent.Length / rectLength));
						}
						opacityBrush.GradientStops.Add(new GradientStop(Colors.Black, (leftTransparent.Length + leftSemiTransparent.Length) / rectLength));
						if(intersect.End != rect.End) {
							opacityBrush.GradientStops.Add(new GradientStop(Colors.Black, (rightSemiTransparent.Start) / rectLength));
							opacityBrush.GradientStops.Add(new GradientStop(Colors.Transparent, (rightSemiTransparent.End) / rectLength));
							opacityBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));
						}
						element.OpacityMask = opacityBrush;
					}
				}
			}
		}
		DoubleAnimation ScrollAnimation;
		int lockStartAnimation;
		void StartScrollAnimation(ITabHeaderLayoutResult result, LayoutGroup group) {
			if(lockStartAnimation > 0) return;
			lockStartAnimation++;
			group.IsAnimated = true;
			double offset = isScrollNext ? result.ScrollResult.ScrollOffset : -result.ScrollResult.ScrollOffset;
			ScrollAnimation = new DoubleAnimation() { From = offset, To = 0.0, Duration = new Duration(TimeSpan.FromMilliseconds(150)) };
			ScrollAnimation.Completed += OnScrollAnimationCompleted;
			BeginAnimation(ScrollOffsetProperty, ScrollAnimation);
		}
		void OnScrollAnimationCompleted(object sender, EventArgs e) {
			ScrollAnimation.Completed -= OnScrollAnimationCompleted;
			ScrollAnimation = null;
			LayoutGroup group = DockLayoutManager.GetLayoutItem(this) as LayoutGroup;
			if(group != null)
				group.IsAnimated = false;
			lockStartAnimation--;
		}
		protected override Geometry GetLayoutClip(Size layoutSlotSize) {
			Thickness clipMargin = ClipMargin;
			if(!IsHorizontal)
				clipMargin = new Thickness(clipMargin.Top, clipMargin.Right, clipMargin.Bottom, clipMargin.Left);
			Rect clipRect = new Rect(
					clipMargin.Left,
					clipMargin.Top,
					layoutSlotSize.Width - (clipMargin.Left + clipMargin.Right),
					layoutSlotSize.Height - (clipMargin.Top + clipMargin.Bottom)
				);
			return new RectangleGeometry() { Rect = clipRect };
		}
		public static void Invalidate(DependencyObject dObj) {
			BaseHeadersPanel headersPanel = dObj as BaseHeadersPanel;
			if(headersPanel == null && dObj != null)
				headersPanel = Core.Native.LayoutHelper.FindParentObject<BaseHeadersPanel>(dObj);
			if(headersPanel != null)
				headersPanel.InvalidateMeasure();
		}
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new UIAutomation.BaseHeadersPanelAutomationPeer(this);
		}
		#endregion UIAutomation
		class Interval : Tuple<double, double> {
			public Interval(double start, double length)
				: base(start, length) {
			}
			public double Start { get { return base.Item1; } }
			public double End { get { return Start + Length; } }
			public double Length { get { return base.Item2; } }
			[Obsolete("Use Start property insted")]
			public new double Item1 { get; set; }
			[Obsolete("Use Length property insted")]
			public new double Item2 { get; set; }
		}
		class OrientationHelper {
			OrientationHelper() { }
			static readonly OrientationHelper Vertical = new OrientationHelper() { IsHorz = false };
			static readonly OrientationHelper Horizontal = new BaseHeadersPanel.OrientationHelper() { IsHorz = true };
			public bool IsHorz { get; private set; }
			public static OrientationHelper GetInstance(bool horz) {
				return horz ? Horizontal : Vertical;
			}
			public double GetLength(Rect rect) {
				return TabHeaderHelper.GetLength(IsHorz, rect);
			}
			public double GetLength(Size size) {
				return TabHeaderHelper.GetLength(IsHorz, size);
			}
			public double GetValue(Point point) {
				return IsHorz ? point.X : point.Y;
			}
			public Size GetSize(double length, double dim) {
				return TabHeaderHelper.GetSize(IsHorz, length, dim);
			}
			public Point GetPoint(double x, double y) {
				return IsHorz ? new Point(x, y) : new Point(y, x);
			}
			public double GetDim(Size size) {
				return IsHorz ? size.Height : size.Width;
			}
		}
	}
	public interface ITabHeader {
		ITabHeaderInfo CreateInfo(Size size);
		void Apply(ITabHeaderInfo info);
		Rect ArrangeRect { get; }
		bool IsPinned { get; }
		TabHeaderPinLocation PinLocation { get; }
	}
	public class TabHeaderOptions : ITabHeaderLayoutOptions {
		public TabHeaderOptions(Size size, bool horz, bool autoFill, int scrolIndex, bool selectedRowFirst) {
			Size = size;
			IsHorizontal = horz;
			IsAutoFill = autoFill;
			ScrollIndex = scrolIndex;
			SelectedRowFirst = selectedRowFirst;
		}
		public Size Size { get; private set; }
		public bool IsHorizontal { get; private set; }
		public bool IsAutoFill { get; private set; }
		public bool SelectedRowFirst { get; private set; }
		public int ScrollIndex { get; private set; }
	}
	public static class HeadersPanelHelper {
		public static ITabHeaderLayoutResult Measure(UIElementCollection children, ITabHeaderLayoutCalculator calculator, ITabHeaderLayoutOptions options) {
			ITabHeaderInfo[] headers = GetHeaderInfos(children, options.Size);
			ITabHeaderLayoutResult result = calculator.Calc(headers, options);
			ApplyMeasure(headers);
			return result;
		}
		static ITabHeaderInfo[] GetHeaderInfos(UIElementCollection children, Size size) {
			ITabHeaderInfo[] headerInfos = new ITabHeaderInfo[children.Count];
			for(int i = 0; i < headerInfos.Length; i++) {
				ITabHeader header = children[i] as ITabHeader;
				if(header != null)
					headerInfos[i] = header.CreateInfo(size);
			}
			Array.Sort(headerInfos, Compare);
			return headerInfos;
		}
		static int Compare(ITabHeaderInfo info1, ITabHeaderInfo info2) {
			if(info1 == info2) return 0;
			return ((BaseHeaderInfo)info1).Index.CompareTo(((BaseHeaderInfo)info2).Index);
		}
		static void ApplyMeasure(ITabHeaderInfo[] headers) {
			for(int i = 0; i < headers.Length; i++) {
				ITabHeaderInfo info = headers[i];
				ITabHeader header = info.TabHeader as ITabHeader;
				if(header != null)
					header.Apply(info);
			}
		}
		public static Orientation GetOrientation(CaptionLocation captionLocation) {
			switch(captionLocation) {
				case CaptionLocation.Right:
				case CaptionLocation.Left:
					return Orientation.Vertical;
				default:
					return Orientation.Horizontal;
			}
		}
	}
	public class BaseHeaderInfo : ITabHeaderInfo {
		public BaseHeaderInfo(UIElement tabHeader, FrameworkElement caption, bool IsCaptionHorz, BaseControlBoxControl controlBox, bool selected, TabHeaderPinLocation pinLocation, bool isPinned) :
			this(tabHeader, caption, controlBox, selected) {
			if(!IsCaptionHorz) {
				CaptionText = new Size(CaptionText.Height, CaptionText.Width);
				CaptionImage = new Size(CaptionImage.Height, CaptionImage.Width);
			}
			PinLocation = pinLocation;
			IsPinned = isPinned;
		}
		public BaseHeaderInfo(UIElement tabHeader, FrameworkElement caption, BaseControlBoxControl controlBox, bool selected) {
			TabHeader = tabHeader;
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(tabHeader);
			LayoutGroup parent = item.Parent ?? LogicalTreeHelper.GetParent(item) as LayoutGroup;
			if(parent != null) {
				Index = parent.TabIndexFromItem(item);
			}
			DesiredSize = tabHeader.DesiredSize;
			if(caption != null) {
				CaptionControl captionControl = caption as CaptionControl;
				if(captionControl != null) {
					if(captionControl.PartText != null)
						CaptionText = captionControl.PartText.DesiredSize;
					if(captionControl.PartImage != null)
						CaptionImage = captionControl.PartImage.DesiredSize;
					if(captionControl.PartSpace != null)
						CaptionImageToCaptionDistance = captionControl.PartSpace.Width.Value;
				}
				else
					CaptionText = caption.DesiredSize;
			}
			if(controlBox != null) {
				ControlBox = controlBox.DesiredSize;
				CaptionToControlBoxDistance = 0;
			}
			IsSelected = selected;
		}
		public int Index { get; private set; }
		public object TabHeader { get; private set; }
		public Rect Rect { get; set; }
		public bool IsVisible { get; set; }
		public bool IsSelected { get; set; }
		public bool ShowCaption { get; set; }
		public bool ShowCaptionImage { get; set; }
		public int ZIndex { get; set; }
		public Size DesiredSize { get; private set; }
		public Size CaptionImage { get; private set; }
		public Size CaptionText { get; private set; }
		public Size ControlBox { get; private set; }
		public double CaptionImageToCaptionDistance { get; private set; }
		public double CaptionToControlBoxDistance { get; private set; }
		public TabHeaderPinLocation PinLocation { get; private set; }
		public bool IsPinned { get; private set;}
	}
	internal class VisualStateController : VisualStateControllerBase<FrameworkElement>, IDisposable {
		#region states
		public const string NormalState = "Normal";
		public const string PressedState = "Pressed";
		public const string MouseOverState = "MouseOver";
		public const string DisabledState = "Disabled";
		#endregion
		#region static
		public static readonly DependencyProperty VisualStateControllerProperty;
		static VisualStateController() {
			var dProp = new DependencyPropertyRegistrator<VisualStateController>();
			dProp.RegisterAttached("VisualStateController", ref VisualStateControllerProperty, (VisualStateController)null);
		}
		#endregion
		List<FrameworkElement> VisualChildren = new List<FrameworkElement>();
		public VisualStateController(FrameworkElement owner) {
			Attach(owner);
		}
		protected override void OnAttached() {
			base.OnAttached();
			SetIsEnabled(Owner.IsEnabled);
			VisualChildren.Add(Owner);
		}
		protected override void Subscribe() {
			base.Subscribe();
			Owner.IsEnabledChanged += owner_IsEnabledChanged;
		}
		protected override void Unsubscribe() {
			Owner.IsEnabledChanged -= owner_IsEnabledChanged;
			base.Unsubscribe();
		}
		void owner_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			SetIsEnabled((bool)e.NewValue);
			UpdateVisualState(true);
		}
		public void UpdateState() {
			UpdateVisualState();
		}
		protected override void UpdateVisualState(bool useTransitions = false) {
			foreach(var child in VisualChildren) {
				UpdateChildVisualState(child, useTransitions);
			}
		}
		void UpdateChildVisualState(FrameworkElement child, bool useTransitions) {
			ISupportVisualStates supportVisualStates = child as ISupportVisualStates;
			if(supportVisualStates != null)
				supportVisualStates.UpdateVisualState();
			if(IsEnabled) {
				UpdateVisualStateCore(child, VisualStateController.NormalState, useTransitions);
				if(IsMousePressed)
					UpdateVisualStateCore(child, VisualStateController.PressedState, useTransitions);
				else {
					if(IsMouseOver)
						UpdateVisualStateCore(child, VisualStateController.MouseOverState, useTransitions);
				}
			} else
				UpdateVisualStateCore(child, VisualStateController.DisabledState, false);
		}
		void UpdateVisualStateCore(FrameworkElement child, string stateName, bool useTransitions) {
			VisualStateManager.GoToState(child, stateName, useTransitions);
		}
		internal void Add(FrameworkElement child) {
			VisualChildren.Add(child);
		}
		internal void Remove(FrameworkElement child) {
			VisualChildren.Remove(child);
		}
		void IDisposable.Dispose() {			
			VisualChildren.Clear();
		}
	}
	internal class VisualStateControllerBase<T> where T : FrameworkElement {
		protected T Owner { get; private set; }
		protected bool IsMousePressed { get; private set; }
		protected bool IsMouseOver { get; private set; }
		protected bool IsEnabled { get; private set; }
		protected void SetIsEnabled(bool isEnabled) {
			IsEnabled = isEnabled;
		}
		protected void Detach(T owner) {
			AssertionException.IsNotNull(owner);
			Unsubscribe();
			Owner = null;
		}
		protected void Attach(T owner) {
			AssertionException.IsNotNull(owner);
			Owner = owner;
			Subscribe();
			OnAttached();
		}
		protected virtual void OnAttached() { }
		protected virtual void Unsubscribe() {
			Owner.MouseEnter -= owner_MouseEnter;
			Owner.MouseLeave -= owner_MouseLeave;
			Owner.MouseLeftButtonDown -= owner_MouseLeftButtonDown;
			Owner.MouseLeftButtonUp -= owner_MouseLeftButtonUp;
			Owner.LostMouseCapture -= Owner_LostMouseCapture;
			Owner.Loaded -= owner_Loaded;
		}
		protected virtual void Subscribe() {
			Owner.MouseEnter += owner_MouseEnter;
			Owner.MouseLeave += owner_MouseLeave;
			Owner.MouseLeftButtonDown += owner_MouseLeftButtonDown;
			Owner.MouseLeftButtonUp += owner_MouseLeftButtonUp;
			Owner.LostMouseCapture += Owner_LostMouseCapture;
			Owner.Loaded += owner_Loaded;
		}
		void owner_Loaded(object sender, RoutedEventArgs e) {
			UpdateVisualState();
		}
		void Owner_LostMouseCapture(object sender, MouseEventArgs e) {
			if(!Owner.IsMouseOver)
				IsMouseOver = false;
			IsMousePressed = false;
			UpdateVisualState(true);
		}
		void owner_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			Owner.ReleaseMouseCapture();
			IsMousePressed = false;
			UpdateVisualState(true);
		}
		void owner_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			IsMousePressed = true;
			Owner.CaptureMouse();
			UpdateVisualState(true);
		}
		void owner_MouseLeave(object sender, MouseEventArgs e) {
			IsMouseOver = false;
			UpdateVisualState(true);
		}
		void owner_MouseEnter(object sender, MouseEventArgs e) {
			IsMouseOver = true;
			UpdateVisualState(true);
		}
		protected virtual void UpdateVisualState(bool useTransitions = false) { }
	}
}
