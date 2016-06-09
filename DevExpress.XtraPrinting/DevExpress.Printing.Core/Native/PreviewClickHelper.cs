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
using System.Windows.Input;
#if SL
using System.Windows.Browser;
#endif
namespace DevExpress.XtraPrinting.Native {
	public static class PreviewClickHelper {
		public static readonly DependencyProperty NavigationPairProperty =
			DependencyProperty.RegisterAttached("NavigationPair", typeof(string), typeof(PreviewClickHelper), new PropertyMetadata(null, OnNavigationPairChanged));
		public static string GetNavigationPair(DependencyObject obj) {
			return (string)obj.GetValue(NavigationPairProperty);
		}
		public static void SetNavigationPair(DependencyObject obj, string value) {
			obj.SetValue(NavigationPairProperty, value);
		}
		static void OnNavigationPairChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FrameworkElement element = d as FrameworkElement;
			if(element == null)
				throw new ArgumentException();
			string navigationPair = (string)e.NewValue;
			if(!string.IsNullOrEmpty(navigationPair))
				element.Cursor = Cursors.Hand;
		}
		public static readonly DependencyProperty TagProperty =
			DependencyProperty.RegisterAttached("Tag", typeof(string), typeof(PreviewClickHelper), new PropertyMetadata(null));
		public static readonly DependencyProperty UrlProperty =
			DependencyProperty.RegisterAttached("Url", typeof(string), typeof(PreviewClickHelper), new PropertyMetadata(null, OnUrlChanged));
		public static string GetTag(DependencyObject obj) {
			return (string)obj.GetValue(TagProperty);
		}
		public static void SetTag(DependencyObject obj, string value) {
			obj.SetValue(TagProperty, value);
		}	   
		public static string GetUrl(DependencyObject obj) {
			return (string)obj.GetValue(UrlProperty);
		}
		public static void SetUrl(DependencyObject obj, string value) {
			obj.SetValue(UrlProperty, value);
		}
		static void OnUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FrameworkElement element = d as FrameworkElement;
			if(element == null)
				throw new ArgumentException();
			string url = (string)e.NewValue;
			if(string.IsNullOrEmpty(url) || String.Compare(url, "empty", StringComparison.InvariantCultureIgnoreCase) == 0)
				return;
			element.Cursor = Cursors.Hand;
			element.MouseLeftButtonUp += (o, args) => NavigateToUrl(url);
		}
		static void NavigateToUrl(string url) { 
#if SL
			HtmlPage.Window.Navigate(new UriBuilder(url).Uri, "_blank");
#else
			ProcessLaunchHelper.StartProcess(url);
#endif
		}
	}
}
