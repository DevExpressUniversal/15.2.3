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

extern alias Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.PropertyEditing;
using Microsoft.Windows.Design.Services;
using System.Windows.Input;
using System.Windows.Media;
#if SL
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Xpf.Core.WPFCompatibility;
using Platform::DevExpress.Xpf.Core;
using DependencyObject = Platform::System.Windows.DependencyObject;
using Point = Platform::System.Windows.Point;
using UIElement = Platform::System.Windows.UIElement;
using RoutedEvent = Platform::DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = Platform::DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = Platform::DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
using HitTestResult = Platform::DevExpress.Xpf.Core.HitTestResult;
using HitTestResultBehavior = Platform::DevExpress.Xpf.Core.HitTestResultBehavior;
using HitTestFilterCallback = Platform::DevExpress.Xpf.Core.HitTestFilterCallback;
using HitTestResultCallback = Platform::DevExpress.Xpf.Core.HitTestResultCallback;
using HitTestFilterBehavior = Platform::DevExpress.Xpf.Core.HitTestFilterBehavior;
using PointHitTestParameters = Platform::DevExpress.Xpf.Core.PointHitTestParameters;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Xpf.Core.Design {
	public class DesignTimeInpuEventsHelper {
		public static void RaiseDesignTimeMouseLeftButtonUpEvent(HitTestFilterCallback filter, Point point, MouseButtonEventArgs originalArgs, FrameworkElement adorner, Platform::System.Windows.FrameworkElement platformControl) {
			Func<object, RoutedEventArgs> createEventArgsDelegate = originalSource => new DesignTimeMouseButtonEventArgs(originalArgs, adorner, platformControl, originalSource) { RoutedEvent = InputEventsHelper.MouseLeftButtonUpEvent };
			RaiseDesignTimeMouseEvent(filter, point, adorner, platformControl, createEventArgsDelegate);
		}
		public static void RaiseDesignTimeMouseLeftButtonDownEvent(HitTestFilterCallback filter, Point point, MouseButtonEventArgs originalArgs, FrameworkElement adorner, Platform::System.Windows.FrameworkElement platformControl) {
			Func<object, RoutedEventArgs> createEventArgsDelegate = originalSource => new DesignTimeMouseButtonEventArgs(originalArgs, adorner, platformControl, originalSource) { RoutedEvent = InputEventsHelper.MouseLeftButtonDownEvent };
			RaiseDesignTimeMouseEvent(filter, point, adorner, platformControl, createEventArgsDelegate);
		}
		public static void RaiseDesignTimeMouseMoveEvent(HitTestFilterCallback filter, Point point, MouseEventArgs originalArgs, FrameworkElement adorner, Platform::System.Windows.FrameworkElement platformControl) {
			Func<object, RoutedEventArgs> createEventArgsDelegate = originalSource => new DesignTimeMouseEventArgs(originalArgs, adorner, platformControl, originalSource) { RoutedEvent = InputEventsHelper.MouseMoveEvent };
			RaiseDesignTimeMouseEvent(filter, point, adorner, platformControl, createEventArgsDelegate);
		}
		static void RaiseDesignTimeMouseEvent(HitTestFilterCallback filter, Point point, FrameworkElement adorner, Platform::System.Windows.FrameworkElement platformControl, Func<object, RoutedEventArgs> createEventArgsDelegate) {
			DependencyObject visualHit;
			if(DesignTimeMouseEventArgs.CapturedPlatformControl == null) {
				HitTestResult hitResult = null;
				HitTestResultCallback resultCallback = delegate(HitTestResult result) {
					hitResult = result;
					return HitTestResultBehavior.Stop;
				};
#if SL
				HitTestHelper.HitTest(platformControl, filter, resultCallback, new PointHitTestParameters(point), false, false);
#else
				VisualTreeHelper.HitTest(platformControl, filter, resultCallback, new PointHitTestParameters(point));
#endif
				if(hitResult == null)
					return;
				visualHit = hitResult.VisualHit;
			} else {
				visualHit = DesignTimeMouseEventArgs.CapturedPlatformControl;
			}
			RoutedEventArgs e = createEventArgsDelegate(visualHit);
			while(visualHit != null) {
				UIElement element = visualHit as UIElement;
				if(element != null)
					element.RaiseEvent(e);
				if(e.Handled)
					break;
				visualHit = LayoutHelper.GetParent(visualHit);
			}
		}
	}
	public class DesignTimeMouseEventArgs : IndependentMouseEventArgs {
		internal static UIElement CapturedPlatformControl { get; private set; }
		internal static void CaptureMouseCore(FrameworkElement adorner, UIElement platformControl) {
			Mouse.Capture(adorner);
			CapturedPlatformControl = platformControl;
		}
		internal static void ReleaseMouseCaptureCore() {
			Mouse.Capture(null);
			CapturedPlatformControl = null;
		}
		protected readonly DXMouseEventArgsFromWPF e;
		FrameworkElement adorner;
		public DesignTimeMouseEventArgs(MouseEventArgs e, FrameworkElement adorner, Platform::System.Windows.FrameworkElement platformControl, object originalSource)
			: base(originalSource) {
			this.e = new DXMouseEventArgsFromWPF(e, adorner, platformControl);
			this.adorner = adorner;
		}
		public override Point GetPosition(UIElement relativeTo) {
			return e.GetPosition(relativeTo);
		}
		public override void CaptureMouse(UIElement element) {
			CaptureMouseCore(adorner, element);
		}
		public override void ReleaseMouseCapture(UIElement element) {
			ReleaseMouseCaptureCore();
		}
#if !SL
		public override MouseButtonState LeftButton { get { return Mouse.LeftButton; } }
#endif
	}
	public class DesignTimeMouseButtonEventArgs : IndependentMouseButtonEventArgs {
		protected readonly DXMouseButtonEventArgsFromWPF e;
		FrameworkElement adorner;
		public DesignTimeMouseButtonEventArgs(MouseButtonEventArgs e, FrameworkElement adorner, Platform::System.Windows.FrameworkElement platformControl, object originalSource)
			: base(originalSource) {
			this.e = new DXMouseButtonEventArgsFromWPF(e, adorner, platformControl);
			this.adorner = adorner;
		}
		public override Point GetPosition(UIElement relativeTo) {
			return e.GetPosition(relativeTo);
		}
		public override void CaptureMouse(UIElement element) {
			DesignTimeMouseEventArgs.CaptureMouseCore(adorner, element);
		}
		public override void ReleaseMouseCapture(UIElement element) {
			DesignTimeMouseEventArgs.ReleaseMouseCaptureCore();
		}
#if !SL
		public override MouseButtonState LeftButton { get { return Mouse.LeftButton; } }
#endif
	}
}
