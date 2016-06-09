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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public enum NavigationElementHorizontalAlignment {
		Left,
		Right,
		Center
	}
	public enum NavigationElementVerticalAlignment {
		Top,
		Bottom,
		Center
	}
	public class NavigationElementHorizontalAlignmentToHorizontalAlignmentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(HorizontalAlignment)) {
				if (value is NavigationElementHorizontalAlignment) {
					switch ((NavigationElementHorizontalAlignment)value) {
						case NavigationElementHorizontalAlignment.Left: return HorizontalAlignment.Left;
						case NavigationElementHorizontalAlignment.Center: return HorizontalAlignment.Center;
						case NavigationElementHorizontalAlignment.Right: return HorizontalAlignment.Right;
					}
				}
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is HorizontalAlignment)
				if (targetType == typeof(NavigationElementHorizontalAlignment)) {
					switch ((HorizontalAlignment)value) {
						case HorizontalAlignment.Left: return NavigationElementHorizontalAlignment.Left;
						case HorizontalAlignment.Center: return NavigationElementHorizontalAlignment.Center;
						case HorizontalAlignment.Right: return NavigationElementHorizontalAlignment.Right;
					}
				}
			return null;
		}
	}
	public class NavigationElementVerticalAlignmentToVerticalAlignmentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(VerticalAlignment)) {
				if (value is NavigationElementVerticalAlignment) {
					switch ((NavigationElementVerticalAlignment)value) {
						case NavigationElementVerticalAlignment.Top: return VerticalAlignment.Top;
						case NavigationElementVerticalAlignment.Center: return VerticalAlignment.Center;
						case NavigationElementVerticalAlignment.Bottom: return VerticalAlignment.Bottom;
					}
				}
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is VerticalAlignment)
				if (targetType == typeof(NavigationElementVerticalAlignment)) {
					switch ((VerticalAlignment)value) {
						case VerticalAlignment.Top: return NavigationElementVerticalAlignment.Top;
						case VerticalAlignment.Center: return NavigationElementVerticalAlignment.Center;
						case VerticalAlignment.Bottom: return NavigationElementVerticalAlignment.Bottom;
					}
				}
			return null;
		}
	}
	public class BoolToVisibilityConverter : MarkupExtension, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Visibility))
				if (value is bool)
					return (bool)value ? Visibility.Visible : Visibility.Collapsed;
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is Visibility)
				if (targetType == typeof(bool))
					return (Visibility)value == Visibility.Visible ? true : false;
			return null;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public abstract class NavigationElementOptions : MapDependencyObject {
		public static readonly DependencyProperty HorizontalAlignmentProperty = DependencyPropertyManager.Register("HorizontalAlignment",
			typeof(NavigationElementHorizontalAlignment), typeof(NavigationElementOptions), new PropertyMetadata(NavigationElementHorizontalAlignment.Left, NotifyPropertyChanged));
		public static readonly DependencyProperty VerticalAlignmentProperty = DependencyPropertyManager.Register("VerticalAlignment",
			typeof(NavigationElementVerticalAlignment), typeof(NavigationElementOptions), new PropertyMetadata(NavigationElementVerticalAlignment.Top, NotifyPropertyChanged));
		public static readonly DependencyProperty MarginProperty = DependencyPropertyManager.Register("Margin",
			typeof(Thickness), typeof(NavigationElementOptions), new PropertyMetadata(new Thickness(16.0), NotifyPropertyChanged));
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(NavigationElementOptions), new PropertyMetadata(true, NotifyPropertyChanged));
		[Category(Categories.Layout)]
		public NavigationElementHorizontalAlignment HorizontalAlignment {
			get { return (NavigationElementHorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
			set { SetValue(HorizontalAlignmentProperty, value); }
		}
		[Category(Categories.Layout)]
		public NavigationElementVerticalAlignment VerticalAlignment {
			get { return (NavigationElementVerticalAlignment)GetValue(VerticalAlignmentProperty); }
			set { SetValue(VerticalAlignmentProperty, value); }
		}
		[Category(Categories.Layout)]
		public Thickness Margin {
			get { return (Thickness)GetValue(MarginProperty); }
			set { SetValue(MarginProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
	}
	public static class MapControlCommands {
		static readonly RoutedCommand scroll = new RoutedCommand("Scroll", typeof(MapControlCommands));
		static readonly RoutedCommand zoom = new RoutedCommand("Zoom", typeof(MapControlCommands));
		public static RoutedCommand Scroll { get { return scroll; } }
		public static RoutedCommand Zoom { get { return zoom; } }
	}
}
