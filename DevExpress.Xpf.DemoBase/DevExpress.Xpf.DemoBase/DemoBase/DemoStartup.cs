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
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using DevExpress.DemoData;
using DevExpress.DemoData.DemoParts;
using DevExpress.DemoData.Helpers;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.DemoBase.DemoTesting;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.Xpf.DemoBase.Internal;
using DevExpress.Utils.About;
namespace DevExpress.Xpf.DemoBase {
	public abstract class DemoStartup : DemoStartupBase {
		const string MenuString = "Menu=";
		const string MainMenuString = "MainMenu";
		const string EmptyLocationString = "Empty";
		public static StartupBase DemoLauncherRun(Assembly demoAssembly, string moduleName, UIElement page, Action onModuleLoaded, IDemoLauncherLoader loader) {
			MethodInfo runMethod = typeof(StartupBase).GetMethod("Run", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Application), typeof(bool), typeof(bool), typeof(UIElement), typeof(object), typeof(IDemoLauncherLoader) }, null);
			runMethod = runMethod.MakeGenericMethod(DemoHelper.GetStartup(demoAssembly));
			DemoStartup startup = (DemoStartup)runMethod.Invoke(null, new object[] { Application.Current, false, false, page, moduleName, loader });			
			return startup;
		}
		static DemoStartup() {
			DemoThemeName = "Demo";
			ThemeHelper.SetActualThemeName(DemoThemeName, Theme.Office2013.Name);
			DefaultTheme = Theme.Office2013;			
		}
		public DemoStartup() {			
			Title = "DevExpress WPF Demos";
		}
		public Assembly DemoAssembly { get; private set; }
		public bool Debug { get; private set; }		
		protected bool RunFromDemoLauncher { get { return UserData != null; } }
		protected override bool DoStartup() {
			ThemeManager.ApplicationThemeName = GetDefaultTheme().Name;
			if(!base.DoStartup()) return false;
			ExitAtRequest = true;
			DemoAssembly = GetAssembly();
			Icon = GetIcon();
			Debug = GetDebug();
			Type fixtureTypeForXBAPOrSLTesting = GetFixtureTypeForXBAPOrSLTesting();
			if(EnvironmentHelper.IsClickOnce || EnvironmentHelper.IsSL || EnvironmentHelper.IsXBAP) {
				if(fixtureTypeForXBAPOrSLTesting != null || !Debug) {
					if(fixtureTypeForXBAPOrSLTesting == null && RunFromDemoLauncher) {
						if(EnvironmentHelper.IsSL)
							DemoTestingHelper.PrepareXBAPTesting(DevExpress.Xpf.DemoBase.DemoTesting.ServiceHelper.ServiceUri);
						else
							DemoTestingHelper.PrepareXBAPTesting(DevExpress.Xpf.DemoBase.DemoTesting.ServiceHelper.SecureServiceUri);
					} else {
						DemoTestingHelper.PrepareXBAPTesting(fixtureTypeForXBAPOrSLTesting);
					}
				}
			} else {
				if(!RunFromDemoLauncher)
					DemoTestingHelper.ProcessArgs(Args);
			}
			return true;
		}
		protected override UIElement CreateMainElement() {
			DemoHelper.InitDemo(DemoAssembly);
			DemoBase = new DemoBase(DemoAssembly, UserData as string, this);
			DemoBase.DemoBaseControl.AllowRunAnotherDemo = RunFromDemoLauncher;
			var mainProduct = DemoBase.DemoBaseControl.Data.MainProduct;
			if(mainProduct != null) {
				Title = string.Format("DevExpress {0} Demos", mainProduct.Title.Text);
			}
			return DemoBase.DemoBaseControl;
		}
		bool GetIsMainMenuBookmark(string value) {
			return value == MainMenuString;
		}
		string GetDemoNameBookmark(string value) {
			return value.Contains(MenuString) ? value.Replace(MenuString, string.Empty) : null;
		}
		protected virtual ImageSource GetIcon() {
			return ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(GetAssembly(), "demoicon.ico"));
		}
		protected virtual Assembly GetAssembly() { return GetType().Assembly; }
		protected abstract bool GetDebug();
		public static Theme DefaultTheme { get; set; }
		public static Theme DemoTheme { get { return Theme.FindTheme(DemoThemeName); } }
		public static string DemoThemeName { get; set; }
		internal DemoBase DemoBase { get; set; }
		protected override void OnRootVisualLoaded(object sender, RoutedEventArgs e) {
			base.OnRootVisualLoaded(sender, e);
			FrameworkElement root = Application.Current == null ? null : (FrameworkElement)Application.Current.MainWindow;
			if(root == null) return;
			DemoTestingHelper.StartTestingIfNeeded(DemoBase.Testing, DemoAssembly);
		}
		protected virtual Type GetFixtureTypeForXBAPOrSLTesting() { return null; }
		static Theme GetDefaultTheme() {
			return DefaultTheme == null ? Theme.Default : DefaultTheme;
		}
	}
}
