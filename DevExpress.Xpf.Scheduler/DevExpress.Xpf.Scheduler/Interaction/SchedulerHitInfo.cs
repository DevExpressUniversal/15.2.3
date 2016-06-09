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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Core.Native;
using DevExpress.XtraScheduler.Internal.Implementations;
#if !SILVERLIGHT
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentDependencyPropertyChangedEventArgs = System.Windows.DependencyPropertyChangedEventArgs;
#else
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentPropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using PlatformIndependentDependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using System.Windows.Interop;
#endif
namespace DevExpress.XtraScheduler.Native {
	public class SchedulerHitInfo : ISchedulerHitInfo {
		static readonly SchedulerHitInfo none = new EmptySchedulerHitInfo();
		public static SchedulerHitInfo None { get { return none; } }
		class EmptySelectableIntervalViewInfo : ISelectableIntervalViewInfo {
			#region ISelectableIntervalViewInfo Members
			public TimeInterval Interval { get { return TimeInterval.Empty; } }
			public bool Selected { get { return false; } }
			public Resource Resource { get { return ResourceBase.Empty; } }
			public SchedulerHitTest HitTestType { get { return SchedulerHitTest.None; } }
			#endregion
		}
		#region EmptySchedulerHitInfo
		class EmptySchedulerHitInfo : SchedulerHitInfo {
			public EmptySchedulerHitInfo()
				: base(null, System.Drawing.Point.Empty, null, SchedulerHitTest.None, new EmptySelectableIntervalViewInfo(), null) {
			}
			public override ISchedulerHitInfo NextHitInfo { get { return this; } }
			public override bool Contains(SchedulerHitTest types) {
				return false;
			}
			public override ISchedulerHitInfo FindHitInfo(SchedulerHitTest types, SchedulerHitTest stopTypes) {
				return this;
			}
		}
		#endregion
#if SL
		public static ISchedulerHitInfo CreateSchedulerHitInfo(SchedulerControl control, PlatformIndependentMouseEventArgs e) {
			Point point = new System.Windows.Point(e.X, e.Y);
			UIElement element = control.HitTestManager.GetContainer(point);
			if(element == null)
				element = control;
			Point transformedPoint = GetTransformedPoint(control, point);
			IEnumerable<UIElement> uiElements = VisualTreeHelper.FindElementsInHostCoordinates(transformedPoint, element);
			return GetNextHitInfo(control, XpfTypeConverter.FromPlatformPoint(point), uiElements.GetEnumerator());
		}		
		public static ISchedulerHitInfo CreateSchedulerHitInfo(SchedulerControl control, System.Windows.Point point) {
			UIElement root = System.Windows.Application.Current.RootVisual;
			IEnumerable<UIElement> uiElements = VisualTreeHelper.FindElementsInHostCoordinates(point, root);
			return GetNextHitInfo(control, XpfTypeConverter.FromPlatformPoint(point), uiElements.GetEnumerator());
		}
		static Point GetTransformedPoint(UIElement control, Point point) {
			GeneralTransform transform = control.TransformToVisual(null);
			Point transformedPoint = transform.Transform(point);
			double? zoomFactor = GetZoomFactor();
			if (zoomFactor != null) {
				GeneralTransform inversedZoomTransform = new ScaleTransform() { ScaleX = (double)zoomFactor, ScaleY = (double)zoomFactor }.Inverse;
				if (inversedZoomTransform != null)
					transformedPoint = inversedZoomTransform.Transform(transformedPoint);
			}
			return transformedPoint;
		}
		static double? GetZoomFactor() {
			Application currentApplication = Application.Current;
			if (currentApplication != null) {
				SilverlightHost silverlightHost = currentApplication.Host;
				if (silverlightHost != null) {
					Content content = silverlightHost.Content;
					if (content != null)
						return content.ZoomFactor;
				}
			}
			return null;
		}
#else
		public static ISchedulerHitInfo CreateSchedulerHitInfo(SchedulerControl control, PlatformIndependentMouseEventArgs e) {
			Point hitPoint = new System.Windows.Point(e.X, e.Y);
			SchedulerHitTestResult hitTestResult = new SchedulerHitTestResult();
			VisualTreeHelper.HitTest(control, hitTestResult.FilterCallback, hitTestResult.ResultCallback, new PointHitTestParameters(hitPoint));
			return GetNextHitInfo(control, XpfTypeConverter.FromPlatformPoint(hitPoint), hitTestResult.Elements.GetEnumerator());
		}
		public static ISchedulerHitInfo CreateSchedulerHitInfo(SchedulerControl control, System.Windows.Point point) {
			SchedulerHitTestResult hitTestResult = new SchedulerHitTestResult();
			VisualTreeHelper.HitTest(control, hitTestResult.FilterCallback, hitTestResult.ResultCallback, new PointHitTestParameters(point));
			return GetNextHitInfo(control, XpfTypeConverter.FromPlatformPoint(point), hitTestResult.Elements.GetEnumerator());
		}
		public static ISchedulerHitInfo CreateSchedulerHitInfo(SchedulerControl control, System.Windows.Point point, bool layoutOnly) {
			SchedulerHitTestResult hitTestResult = new SchedulerHitTestResult();
			VisualTreeHelper.HitTest(control, hitTestResult.FilterCallback, hitTestResult.ResultCallback, new PointHitTestParameters(point));
			return GetNextHitInfo(control, XpfTypeConverter.FromPlatformPoint(point), hitTestResult.Elements.GetEnumerator(), layoutOnly);
		}
#endif
		protected static ISchedulerHitInfo GetNextHitInfo(SchedulerControl control, System.Drawing.Point hitPoint, IEnumerator<UIElement> uiElements) {
			return GetNextHitInfo(control, hitPoint, uiElements, false);
		}
		protected static ISchedulerHitInfo GetNextHitInfo(SchedulerControl control, System.Drawing.Point hitPoint, IEnumerator<UIElement> uiElements, bool layoutOnly) {
			while (uiElements.MoveNext()) {
				UIElement current = uiElements.Current;
				SchedulerHitTest hitTestType = SchedulerControl.GetHitTestType(current);
				if (hitTestType > 0 && hitTestType != SchedulerHitTest.None) {
					if (hitTestType == SchedulerHitTest.Undefined)
						break;
					ISelectableIntervalViewInfo viewInfo = GetSelectableIntervalViewInfo(control, current);
					IAppointmentView appointmentView = viewInfo as IAppointmentView;
					if (layoutOnly || (appointmentView != null && appointmentView.Appointment.IsDisposed))
						continue;
					if (viewInfo != null) {
						return new SchedulerHitInfo(control, hitPoint, uiElements, hitTestType, viewInfo, current);
					}
				}
			}
			return SchedulerHitInfo.None;
		}
		protected internal static ISelectableIntervalViewInfo GetSelectableIntervalViewInfo(SchedulerControl control, UIElement element) {
			ISelectableIntervalViewInfo viewInfo = SchedulerControl.GetSelectableIntervalViewInfo(element);
			if (viewInfo != null)
				return viewInfo;
			VisualNavigationButton button = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<VisualNavigationButton>(element);
			if (button != null)
				return (ISelectableIntervalViewInfo)button;
			System.Windows.DependencyObject current = element;
			while (current != null) {
				ISupportHitTest supportHitTest = current as ISupportHitTest;
				if (supportHitTest != null)
					return supportHitTest.GetSelectableIntervalViewInfo(control);
				ContentPresenter contentPresenter = current as ContentPresenter;
				if (contentPresenter != null) {
					ISelectableIntervalViewInfo result = contentPresenter.Content as ISelectableIntervalViewInfo;
					if (result != null)
						return result;
				}
				XPFContentControl contentControl = current as XPFContentControl;
				if (contentControl != null) {
					ISelectableIntervalViewInfo result = contentControl.Content as ISelectableIntervalViewInfo;
					if (result != null)
						return result;
				}
				current = VisualTreeHelper.GetParent(current);
			}
			return null;
		}
		ISelectableIntervalViewInfo viewInfo;
		ISchedulerHitInfo nextHitInfo;
		IEnumerator<UIElement> hitTestElements;
		SchedulerHitTest hitTest;
		SchedulerControl control;
		System.Drawing.Point hitPoint;
		UIElement visualHit;
		protected internal SchedulerHitInfo(SchedulerControl control, System.Drawing.Point hitPoint, IEnumerator<UIElement> hitTestElements, SchedulerHitTest hitTest, ISelectableIntervalViewInfo viewInfo, UIElement visualHit) {
			this.control = control;
			this.hitTest = hitTest;
			this.hitTestElements = hitTestElements;
			this.viewInfo = viewInfo;
			this.hitPoint = hitPoint;
			this.visualHit = visualHit;
		}
		public virtual ISchedulerHitInfo NextHitInfo {
			get {
				if (nextHitInfo == null)
					nextHitInfo = GetNextHitInfo(control, HitPoint, hitTestElements);
				return nextHitInfo;
			}
		}
		public virtual UIElement VisualHit {
			get {
				return visualHit;
			}
		}
		public System.Drawing.Point HitPoint { get { return hitPoint; } }
		#region ISchedulerHitInfo Members
		public virtual ISelectableIntervalViewInfo ViewInfo { get { return viewInfo; } }
		public virtual bool Contains(SchedulerHitTest types) {
			ISchedulerHitInfo current = this;
			while (current != SchedulerHitInfo.None) {
				if ((current.HitTest & types) != 0)
					return true;
				current = current.NextHitInfo;
			}
			return false;
		}
		public virtual SchedulerHitTest HitTest { get { return hitTest; } }
		public virtual ISchedulerHitInfo FindFirstLayoutHitInfo() {
			return FindHitInfo(SchedulerHitTest.Cell | SchedulerHitTest.DayHeader | SchedulerHitTest.AllDayArea | SchedulerHitTest.DayOfWeekHeader | SchedulerHitTest.TimeScaleHeader | SchedulerHitTest.ResourceHeader | SchedulerHitTest.SelectionBarCell);
		}
		public virtual ISchedulerHitInfo FindHitInfo(SchedulerHitTest types) {
			return FindHitInfo(types, SchedulerHitTest.None);
		}
		public virtual ISchedulerHitInfo FindHitInfo(SchedulerHitTest types, SchedulerHitTest stopTypes) {
			if ((types & stopTypes) != 0)
				Exceptions.ThrowArgumentException("stopTypes", stopTypes);
			ISchedulerHitInfo current = this;
			while (current != SchedulerHitInfo.None) {
				if ((current.HitTest & types) != 0)
					return current;
				if ((current.HitTest & stopTypes) != 0)
					return SchedulerHitInfo.None;
				current = current.NextHitInfo;
			}
			return SchedulerHitInfo.None;
		}
		#endregion
	}
	public class SchedulerHitTestResult {
		readonly List<UIElement> elements;
		public SchedulerHitTestResult() {
			this.elements = new List<UIElement>();
		}
		public List<UIElement> Elements { get { return elements; } }
		public HitTestFilterBehavior FilterCallback(System.Windows.DependencyObject target) {
			UIElement element = target as UIElement;
			if (element == null)
				return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
			if (element.Visibility == System.Windows.Visibility.Collapsed)
				return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
			return HitTestFilterBehavior.Continue;
		}
		public HitTestResultBehavior ResultCallback(HitTestResult result) {
			UIElement element = result.VisualHit as UIElement;
			if (element != null)
				elements.Add(element);
			return HitTestResultBehavior.Continue;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class HitTestManager : DependencyObject {
		public static void AddHitContainer(FrameworkElement element) {
#if SL
			SchedulerControl control = LayoutHelper.FindParentObject<SchedulerControl>(element);
			if (control != null)
				control.HitTestManager.Add(element);
#endif
		}
		public static void RemoveHitContainer(UIElement element) {
		}
		static void OnElementLoaded(object sender, RoutedEventArgs e) {
			FrameworkElement fe = sender as FrameworkElement;
			HitTestManager.AddHitContainer(fe);
		}
		static bool IsPointInElementBounds(UIElement root, UIElement element, Point point) {
			if (element.Visibility == Visibility.Collapsed || !element.IsHitTestVisible)
				return false;
			Rect r = element.TransformToVisual(root).TransformBounds(new Rect(0, 0, element.RenderSize.Width, element.RenderSize.Height));
			return point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height;
		}
		SchedulerControl scheduler;
		List<FrameworkElement> elements = new List<FrameworkElement>();
		public HitTestManager(SchedulerControl scheduler) {
			this.scheduler = scheduler;
		}
		#region IsHitTestContainer
		public static readonly DependencyProperty IsHitTestContainerProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedPropertyCore<HitTestManager, bool>("IsHitTestContainer", false, FrameworkPropertyMetadataOptions.None, OnIsHitTestContainerPropertyChanged);
		static void OnIsHitTestContainerPropertyChanged(DependencyObject d, PlatformIndependentDependencyPropertyChangedEventArgs e) {
#if SL
			FrameworkElement fe = d as FrameworkElement;
			if(fe == null)
				return;
			fe.Loaded += new RoutedEventHandler(OnElementLoaded);
#endif
		}
		public static void SetIsHitTestContainer(DependencyObject d, bool value) {
			d.SetValue(IsHitTestContainerProperty, value);
		}
		public static bool GetIsHitTestContainer(DependencyObject d) {
			return (bool)d.GetValue(IsHitTestContainerProperty);
		}
		#endregion
		public UIElement GetContainer(Point point) {
			foreach (FrameworkElement element in this.elements) {
				if (!element.IsInVisualTree())
					continue;
				if (IsPointInElementBounds(this.scheduler, element, point))
					return element;
			}
			return null;
		}
		public void Add(FrameworkElement element) {
			element.Unloaded += new RoutedEventHandler(OnElementUnloaded);
			this.elements.Add(element);
		}
		void OnElementUnloaded(object sender, RoutedEventArgs e) {
			FrameworkElement fe = sender as FrameworkElement;
			this.elements.Remove(fe);
		}
	}
}
