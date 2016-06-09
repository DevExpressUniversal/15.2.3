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

using System.Windows;
namespace DevExpress.Xpf.DataAccess.Native {
	public static class WindowMinSizeToContent {
		public static readonly DependencyProperty AllowMinSizeToContentProperty = DependencyProperty.RegisterAttached(
			"AllowMinSizeToContent", typeof(bool), typeof(WindowMinSizeToContent), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(AllowMinSizeToContentChanged))
		);
		public static bool GetAllowMinSizeToContent(DependencyObject d) { return (bool)d.GetValue(AllowMinSizeToContentProperty); }
		public static void SetAllowMinSizeToContent(DependencyObject d, bool v) { d.SetValue(AllowMinSizeToContentProperty, v); }
		static void AllowMinSizeToContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs eProperty) {
			if((bool)eProperty.NewValue == true) {
				var window = d as Window;
				if(window == null)
					return;
				RoutedEventHandler loaded = null;
				loaded = (s, e) => {
					var width = d.GetValue(Window.ActualWidthProperty);
					var height = d.GetValue(Window.ActualHeightProperty);
					d.SetValue(Window.MinWidthProperty, width);
					d.SetValue(Window.MinHeightProperty, height);
					window.Loaded -= loaded;
				};
				window.Loaded += loaded;
			}
		}
	}
}
