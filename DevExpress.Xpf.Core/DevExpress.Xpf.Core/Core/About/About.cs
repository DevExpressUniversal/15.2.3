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

#if SLDESIGN
extern alias Platform;
using AssemblyInfo = Platform::AssemblyInfo;
#endif
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Data;
using System;
using DevExpress.Xpf.Core;
using System.Threading;
using System.Windows.Threading;
using DevExpress.Utils.About;
using System.ComponentModel;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Reflection;
using AboutAlias = DevExpress.Utils.About;
using System.Security;
namespace DevExpress.Xpf {
	public class About {
		public static ProductKind GetDefaultProductKind() {
			return
#if !SL
				ProductKind.DXperienceWPF;
#else
				ProductKind.DXperienceSliverlight;
#endif
		} 
		public static void ShowAbout(Type type) {
			ProductKind kind = GetDefaultProductKind();
			Dispatcher.CurrentDispatcher.BeginInvoke(new Action<ProductKind>(DevExpress.Xpf.About.ShowAbout), new object[] { kind });
		}
		[SecuritySafeCritical]
		public static void ShowAbout(ProductKind kind) {
			if (!BrowserInteropHelper.IsBrowserHosted) {
#if !SLDESIGN
				AppDomain domain = AppDomain.CreateDomain("About window domain");
				domain.SetData("ProductKind", kind);
				CrossAppDomainDelegate action = () => {
					ProductKind productKind = (ProductKind)AppDomain.CurrentDomain.GetData("ProductKind");
					AboutInfoBase aboutInfo = new AboutInfo();
					Application app = new Application();
					app.ShutdownMode = ShutdownMode.OnMainWindowClose;
#else
					ProductKind productKind = kind;
#endif
					AboutWindow aWindow = new AboutWindow();
					AboutInfo aif = new AboutInfo();
					ProductStringInfo info = GetProductInfo(productKind);
					aif.ProductPlatform = info.ProductPlatform;
					aif.ProductName = info.ProductName;
					aif.Version = AssemblyInfo.MarketingVersion;
					ControlAbout cAbout = new ControlAbout(aif);
					aWindow.Content = cAbout;
#if SLDESIGN
					aWindow.ShowDialog();
#else
					app.MainWindow = aWindow;
					app.MainWindow.Show();
					app.Run();
				};
				domain.DoCallBack(action);
#endif
			}
			else {
				MessageBox.Show("You're using the trial version. For information on buying DevExpress products, please visit us at www.devexpress.com.", "Info", MessageBoxButton.OK);
			}
		}
		static ProductStringInfo GetProductInfo(ProductKind productKind) {
			switch (productKind) {
				case ProductKind.FreeOffer:
					return new ProductStringInfo("DevExpress Free Controls");
				case ProductKind.DXperienceWPF:
					return new ProductStringInfo("DevExpress WPF Subscription");
				case ProductKind.DXperienceSliverlight:
					return new ProductStringInfo("DevExpress Silverlight Subscription");
			}
			return ProductInfoHelper.GetProductInfo(productKind);
		}
	}
	public class AboutWindow : Window {
		public AboutWindow() {
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			SizeToContent = SizeToContent.WidthAndHeight;
			ResizeMode = ResizeMode.NoResize;
			ShowActivated = true;
			WindowStyle = WindowStyle.None;
			AllowsTransparency = true;
			Background = Brushes.Transparent;
			ShowInTaskbar = false;
			Topmost = true;
		}
		protected override void OnClosed(EventArgs e) {
#if !DEBUG
#endif
			base.OnClosed(e);
		}
	}
}
