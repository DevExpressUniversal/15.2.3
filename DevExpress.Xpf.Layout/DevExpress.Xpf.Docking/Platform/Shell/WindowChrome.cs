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
using Assert = DevExpress.Xpf.Layout.Core.Base.AssertionException;
namespace DevExpress.Xpf.Docking.Platform.Shell {
	class WindowChrome : Freezable {
		#region static
		public static readonly DependencyProperty WindowChromeProperty =
			DependencyProperty.RegisterAttached("WindowChrome", typeof(WindowChrome), typeof(WindowChrome),
			new PropertyMetadata(null, OnChromeChanged));
		private static void OnChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(System.ComponentModel.DesignerProperties.GetIsInDesignMode(d)) {
				return;
			}
			var window = (Window)d;
			var newChrome = (WindowChrome)e.NewValue;
			Assert.IsNotNull(window);
			WindowChromeWorker chromeWorker = WindowChromeWorker.GetWindowChromeWorker(window);
			if(chromeWorker == null) {
				chromeWorker = new WindowChromeWorker();
				WindowChromeWorker.SetWindowChromeWorker(window, chromeWorker);
			}
			chromeWorker.SetWindowChrome(newChrome);
		}
		public static WindowChrome GetWindowChrome(Window window) {
			Assert.IsNotNull(window, "window");
			return (WindowChrome)window.GetValue(WindowChromeProperty);
		}
		public static void SetWindowChrome(Window window, WindowChrome chrome) {
			Assert.IsNotNull(window, "window");
			window.SetValue(WindowChromeProperty, chrome);
		}
		#endregion
		public WindowChrome() {
		}
		protected override Freezable CreateInstanceCore() {
			return new WindowChrome();
		}
	}
}
