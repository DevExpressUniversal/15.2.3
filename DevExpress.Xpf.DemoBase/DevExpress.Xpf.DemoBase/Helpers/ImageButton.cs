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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public class ImageButton : Control {
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(ImageButton), new PropertyMetadata(null));
		public SolidColorBrush TextForeground {
			get { return (SolidColorBrush)GetValue(TextForegroundProperty); }
			set { SetValue(TextForegroundProperty, value); }
		}
		public static readonly DependencyProperty TextForegroundProperty =
			DependencyProperty.Register("TextForeground", typeof(SolidColorBrush), typeof(ImageButton), new PropertyMetadata(null));
		public SolidColorBrush TextForegroundHover {
			get { return (SolidColorBrush)GetValue(TextForegroundHoverProperty); }
			set { SetValue(TextForegroundHoverProperty, value); }
		}
		public static readonly DependencyProperty TextForegroundHoverProperty =
			DependencyProperty.Register("TextForegroundHover", typeof(SolidColorBrush), typeof(ImageButton), new PropertyMetadata(null));
		public ImageSource Image {
			get { return (ImageSource)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));
		public ImageSource ImageHover {
			get { return (ImageSource)GetValue(ImageHoverProperty); }
			set { SetValue(ImageHoverProperty, value); }
		}
		public static readonly DependencyProperty ImageHoverProperty =
			DependencyProperty.Register("ImageHover", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(ImageButton), new PropertyMetadata(null));
		public double ImageWidth {
			get { return (double)GetValue(ImageWidthProperty); }
			set { SetValue(ImageWidthProperty, value); }
		}
		public static readonly DependencyProperty ImageWidthProperty =
			DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButton), new PropertyMetadata(90d));
		public override void OnApplyTemplate() {
			((StackPanel)GetTemplateChild("stackPanel")).MouseUp += (s, e) => {
				if (e.ChangedButton == MouseButton.Left) {
					Command.Execute(null);
				}
			};
		}
		static ImageButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
		}
	}
	class ImageButtonTextVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var typed = (string)value;
			return string.IsNullOrEmpty(typed) ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
