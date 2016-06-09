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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.NavBar {
	public partial class NavBarAnimationOptions : DependencyObject {
		public static readonly DependencyProperty HorizontalExpandProperty;
		public static readonly DependencyProperty VerticalExpandProperty;		
		public static readonly DependencyProperty StretchChildProperty;
		public static readonly DependencyProperty MinWidthProperty;
		public static readonly DependencyProperty MinHeightProperty;
		public static readonly DependencyProperty ExpandStoryboardProperty;
		public static readonly DependencyProperty CollapseStoryboardProperty;
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty AnimationProgressProperty;
		public static readonly DependencyProperty IsAnimationEnabledProperty;
		static NavBarAnimationOptions() {	
			HorizontalExpandProperty = DependencyPropertyManager.RegisterAttached("HorizontalExpand", typeof(HorizontalExpandMode), typeof(NavBarAnimationOptions), new FrameworkPropertyMetadata(HorizontalExpandMode.None));
			VerticalExpandProperty = DependencyPropertyManager.RegisterAttached("VerticalExpand", typeof(VerticalExpandMode), typeof(NavBarAnimationOptions), new FrameworkPropertyMetadata(VerticalExpandMode.None));
			StretchChildProperty = DependencyPropertyManager.RegisterAttached("StretchChild", typeof(bool), typeof(NavBarAnimationOptions), new FrameworkPropertyMetadata(false));
			MinWidthProperty = DependencyPropertyManager.RegisterAttached("MinWidth", typeof(double), typeof(NavBarAnimationOptions), new FrameworkPropertyMetadata(0d));
			MinHeightProperty = DependencyPropertyManager.RegisterAttached("MinHeight", typeof(double), typeof(NavBarAnimationOptions), new FrameworkPropertyMetadata(0d));
			ExpandStoryboardProperty = DependencyPropertyManager.RegisterAttached("ExpandStoryboard", typeof(Storyboard), typeof(NavBarAnimationOptions), new FrameworkPropertyMetadata(null));
			CollapseStoryboardProperty = DependencyPropertyManager.RegisterAttached("CollapseStoryboard", typeof(Storyboard), typeof(NavBarAnimationOptions), new FrameworkPropertyMetadata(null));
			IsExpandedProperty = DependencyPropertyManager.RegisterAttached("IsExpanded", typeof(bool), typeof(NavBarAnimationOptions), new PropertyMetadata(false, OnIsExpandedPropertyChanged));
			AnimationProgressProperty = DependencyPropertyManager.RegisterAttached("AnimationProgress", typeof(double), typeof(NavBarAnimationOptions), new FrameworkPropertyMetadata(1d));
			IsAnimationEnabledProperty = DependencyPropertyManager.RegisterAttached("IsAnimationEnabled", typeof(bool), typeof(NavBarAnimationOptions), new FrameworkPropertyMetadata(true));
		}
		public static void OnIsExpandedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			Storyboard storyboard = (bool)e.NewValue ? (Storyboard)GetExpandStoryboard(sender) : (Storyboard)GetCollapseStoryboard(sender);
			FrameworkElement element = (FrameworkElement)sender;
			ValueSource source = System.Windows.DependencyPropertyHelper.GetValueSource(sender, e.Property);
			if(storyboard == null || source.BaseValueSource == BaseValueSource.Default)
				return;
			storyboard.Begin(element, true);
			if(!GetIsAnimationEnabled(sender)) {
				storyboard.SkipToFill(element);
				return;
			}
			if(!element.IsLoaded)
				storyboard.SkipToFill(element);
		}
		public static bool GetIsAnimationEnabled(DependencyObject obj) {
			return (bool)obj.GetValue(IsAnimationEnabledProperty);
		}
		public static void SetIsAnimationEnabled(DependencyObject obj, bool value) {
			obj.SetValue(IsAnimationEnabledProperty, value);
		}
		public static void SetAnimationProgress(DependencyObject d, double progress) {
			d.SetValue(AnimationProgressProperty, progress);
		}
		public static double GetAnimationProgress(DependencyObject d) {
			return (double)d.GetValue(AnimationProgressProperty);
		}
		public static void SetHorizontalExpand(DependencyObject d, HorizontalExpandMode expander) {
			d.SetValue(HorizontalExpandProperty, expander);
		}
		public static HorizontalExpandMode GetHorizontalExpand(DependencyObject d) {
			return (HorizontalExpandMode)d.GetValue(HorizontalExpandProperty);
		}
		public static void SetVerticalExpand(DependencyObject d, VerticalExpandMode expander) {
			d.SetValue(VerticalExpandProperty, expander);
		}
		public static VerticalExpandMode GetVerticalExpand(DependencyObject d) {
			return (VerticalExpandMode)d.GetValue(VerticalExpandProperty);
		}		
		public static void SetStretchChild(DependencyObject d, bool stretchChild) {
			d.SetValue(StretchChildProperty, stretchChild);
		}
		public static bool GetStretchChild(DependencyObject d) {
			return (bool)d.GetValue(StretchChildProperty);
		}
		public static void SetMinWidth(DependencyObject d, double minWidth) {
			d.SetValue(MinWidthProperty, minWidth);
		}
		public static double GetMinWidth(DependencyObject d) {
			return (double)d.GetValue(MinWidthProperty);
		}
		public static void SetMinHeight(DependencyObject d, double minHeight) {
			d.SetValue(MinHeightProperty, minHeight);
		}
		public static double GetMinHeight(DependencyObject d) {
			return (double)d.GetValue(MinHeightProperty);
		}
		public static void SetExpandStoryboard(DependencyObject element, Storyboard value) {			
			element.SetValue(ExpandStoryboardProperty, value);
		}
		public static Storyboard GetExpandStoryboard(DependencyObject element) {			
			return (Storyboard)element.GetValue(ExpandStoryboardProperty);
		}
		public static void SetCollapseStoryboard(DependencyObject element, Storyboard value) {
			element.SetValue(CollapseStoryboardProperty, value);
		}
		public static Storyboard GetCollapseStoryboard(DependencyObject element) {
			return (Storyboard)element.GetValue(CollapseStoryboardProperty);
		}
		public static void SetIsExpanded(DependencyObject element, bool value) {			
			element.SetValue(IsExpandedProperty, value);
		}
		public static bool GetIsExpanded(DependencyObject element) {			
			return (bool)element.GetValue(IsExpandedProperty);
		}		
	}	
}
