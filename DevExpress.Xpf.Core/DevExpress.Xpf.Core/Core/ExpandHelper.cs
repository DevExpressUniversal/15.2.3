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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
#if !DEMO
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Map;
#endif
using DevExpress.Xpf.Core.Native;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using Storyboard = System.String;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Core {
	public class ExpandHelper : DependencyObject {
		public static readonly Size DefaultVisibleSize = new Size(double.NaN, double.NaN);
		public static readonly RoutedEvent IsExpandedChangedEvent;
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty VisibleSizeProperty;
		public static readonly DependencyProperty ItemsContainerProperty;
		public static readonly DependencyProperty RowContainerProperty;
		public static readonly DependencyProperty ExpandSpeedProperty;
		public static readonly DependencyProperty ExpandStoryboardProperty;
		public static readonly DependencyProperty CollapseStoryboardProperty;
		public static readonly DependencyProperty IsContinuousAnimationProperty;
		static ExpandHelper() {
			IsExpandedChangedEvent = EventManager.RegisterRoutedEvent("IsExpandedChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ExpandHelper));
			IsExpandedProperty = DependencyPropertyManager.RegisterAttached("IsExpanded", typeof(bool), typeof(ExpandHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, OnIsExpandedChanged));
			VisibleSizeProperty = DependencyPropertyManager.RegisterAttached("VisibleSize", typeof(Size), typeof(ExpandHelper), new FrameworkPropertyMetadata(DefaultVisibleSize));
			ItemsContainerProperty = DependencyPropertyManager.RegisterAttached("ItemsContainer", typeof(FrameworkElement), typeof(ExpandHelper), new FrameworkPropertyMetadata(null));
			RowContainerProperty = DependencyPropertyManager.RegisterAttached("RowContainer", typeof(FrameworkElement), typeof(ExpandHelper), new FrameworkPropertyMetadata(null));
			ExpandSpeedProperty = DependencyPropertyManager.RegisterAttached("ExpandSpeed", typeof(double), typeof(ExpandHelper), new FrameworkPropertyMetadata(1500d));
			ExpandStoryboardProperty = DependencyPropertyManager.RegisterAttached("ExpandStoryboard", typeof(Storyboard), typeof(ExpandHelper), new FrameworkPropertyMetadata(null));
			CollapseStoryboardProperty = DependencyPropertyManager.RegisterAttached("CollapseStoryboard", typeof(Storyboard), typeof(ExpandHelper), new FrameworkPropertyMetadata(null));
			IsContinuousAnimationProperty = DependencyPropertyManager.RegisterAttached("IsContinuousAnimation", typeof(bool), typeof(ExpandHelper), new FrameworkPropertyMetadata(true));
		}
		public static void SetIsContinuousAnimation(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(IsContinuousAnimationProperty, value);
		}
		public static bool GetIsContinuousAnimation(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(IsContinuousAnimationProperty);
		}
		public static void SetCollapseStoryboard(DependencyObject element, Storyboard value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(CollapseStoryboardProperty, value);
		}
		public static Storyboard GetCollapseStoryboard(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (Storyboard)element.GetValue(CollapseStoryboardProperty);
		}
		public static void SetExpandStoryboard(DependencyObject element, Storyboard value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ExpandStoryboardProperty, value);
		}
		public static Storyboard GetExpandStoryboard(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (Storyboard)element.GetValue(ExpandStoryboardProperty);
		}
		public static void SetExpandSpeed(DependencyObject element, double value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ExpandSpeedProperty, value);
		}
		public static double GetExpandSpeed(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (double)element.GetValue(ExpandSpeedProperty);
		}
		public static void SetRowContainer(DependencyObject element, FrameworkElement value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(RowContainerProperty, value);
		}
		public static FrameworkElement GetRowContainer(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (FrameworkElement)element.GetValue(RowContainerProperty);
		}
		public static void SetItemsContainer(DependencyObject element, FrameworkElement value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ItemsContainerProperty, value);
		}
		public static FrameworkElement GetItemsContainer(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (FrameworkElement)element.GetValue(ItemsContainerProperty);
		}
		public static void SetVisibleSize(DependencyObject element, Size value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(VisibleSizeProperty, value);
		}
		public static Size GetVisibleSize(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (Size)element.GetValue(VisibleSizeProperty);
		}
		public static void SetIsExpanded(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(IsExpandedProperty, value);
		}
		public static bool GetIsExpanded(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(IsExpandedProperty);
		}
		static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
#if DEBUGTEST
			EventLog.Default.AddEvent(new DependencyPropertyValueSnapshot<DependencyObject, bool>(ExpandHelper.IsExpandedProperty, d, ExpandHelper.GetIsExpanded(d)));
#endif
			((UIElement)d).RaiseEvent(new RoutedEventArgs(IsExpandedChangedEvent, e));
		}
	}
}
