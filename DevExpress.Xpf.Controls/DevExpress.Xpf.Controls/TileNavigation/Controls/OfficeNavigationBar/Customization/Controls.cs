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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
namespace DevExpress.Xpf.Navigation.Internal {
	public enum CustomizationArrowControlDirection { Up, Down, Left, Right }
	public class CustomizationArrowControl : Control {
		public static readonly DependencyProperty DirectionProperty =
			DependencyPropertyManager.Register("Direction", typeof(CustomizationArrowControlDirection), typeof(CustomizationArrowControl), new FrameworkPropertyMetadata(CustomizationArrowControlDirection.Up, new PropertyChangedCallback(OnDirectionPropertyChanged)));
		public CustomizationArrowControlDirection Direction {
			get { return (CustomizationArrowControlDirection)GetValue(DirectionProperty); }
			set { SetValue(DirectionProperty, value); }
		}
		protected static void OnDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CustomizationArrowControl)d).OnDirectionChanged((CustomizationArrowControlDirection)e.OldValue);
		}
		protected virtual void OnDirectionChanged(CustomizationArrowControlDirection oldValue) {
			VisualStateManager.GoToState(this, Direction.ToString(), false);
		}
		public CustomizationArrowControl() {
			DefaultStyleKey = typeof(CustomizationArrowControl);
			Loaded += new RoutedEventHandler(OnLoaded);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			VisualStateManager.GoToState(this, Direction.ToString(), false);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			VisualStateManager.GoToState(this, Direction.ToString(), false);
		}
	}
	public class CustomizationSeparatorControl : Control {
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public static readonly DependencyProperty OrientationProperty =
			DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(CustomizationSeparatorControl), new FrameworkPropertyMetadata(Orientation.Horizontal));
		public CustomizationSeparatorControl() {
			DefaultStyleKey = typeof(CustomizationSeparatorControl);
		}
	}
	public class CustomizationItemControl : ContentControl {
		protected override void OnContentChanged(object oldContent, object newContent) { }
	}
	[Browsable(false)]
	public class NavigationBarThemeDependentValuesProvider : FrameworkElement {
		#region static
		public static readonly DependencyProperty CustomizationFormMinWidthProperty;
		public static readonly DependencyProperty CustomizationFormMinHeightProperty;
		public static readonly DependencyProperty CustomizationFormFloatSizeProperty;
		static NavigationBarThemeDependentValuesProvider() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationBarThemeDependentValuesProvider), new FrameworkPropertyMetadata(typeof(NavigationBarThemeDependentValuesProvider)));
			CustomizationFormMinWidthProperty = DependencyPropertyManager.Register("CustomizationFormMinWidth", typeof(double), typeof(NavigationBarThemeDependentValuesProvider), new FrameworkPropertyMetadata(0d));
			CustomizationFormMinHeightProperty = DependencyPropertyManager.Register("CustomizationFormMinHeight", typeof(double), typeof(NavigationBarThemeDependentValuesProvider), new FrameworkPropertyMetadata(0d));
			CustomizationFormFloatSizeProperty = DependencyPropertyManager.Register("CustomizationFormFloatSize", typeof(SizeEx), typeof(NavigationBarThemeDependentValuesProvider), new FrameworkPropertyMetadata(new SizeEx()));
		}
		#endregion
		public SizeEx CustomizationFormFloatSize {
			get { return (SizeEx)GetValue(CustomizationFormFloatSizeProperty); }
			set { SetValue(CustomizationFormFloatSizeProperty, value); }
		}
		public double CustomizationFormMinHeight {
			get { return (double)GetValue(CustomizationFormMinHeightProperty); }
			set { SetValue(CustomizationFormMinHeightProperty, value); }
		}
		public double CustomizationFormMinWidth {
			get { return (double)GetValue(CustomizationFormMinWidthProperty); }
			set { SetValue(CustomizationFormMinWidthProperty, value); }
		}
		internal NavigationBarThemeDependentValuesProvider() {
		}
	}
	public class SizeEx {
		public double Width { get; set; }
		public double Height { get; set; }
		public SizeEx() {
			Width = 0d;
			Height = 0d;
		}
		public SizeEx(double w, double h) {
			this.Width = w;
			this.Height = h;
		}
		public static implicit operator Size(SizeEx second) {
			return new Size(second.Width, second.Height);
		}
	}
}
