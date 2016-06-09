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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Native;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer;
using System.Threading;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.DemoBase.DemoTesting {
	public class CheckAllDemosFixture : BaseDemoTestingFixture {
		Theme CurrentTheme { get; set; }
		const string DemoThemeName = "Demo";
		protected virtual void AdditionalChecks() { }
		protected override void CreateActions() {
			IAmAlive("Getting modules");
			IList<ModuleDescription> modules = DemoBaseTesting.Modules;
			IAmAlive("Modules count: " + (modules == null ? "<null>" : modules.Count.ToString()));
			List<ModuleDescription> modulesToCheck = DemoBaseTesting.Modules.Where(m => CanRunModule(m.ModuleType)).ToList();
			foreach(ModuleDescription module in modulesToCheck) {
				IAmAlive("Checks");
				CheckDemo(module, true);
				IAmAlive("Additional Checks");
				AdditionalChecks();
			}
			IAmAlive("Loading the first module");
			CreateSetCurrentDemoActions(modulesToCheck.First(), false);
			Dispatch(PopupCloser.CloseAllPopups);
		}
		public class PopupCloser {
			public static void CloseAllPopups() {
				EnumChildWindows(GetDesktopWindow(), EnumWindow, IntPtr.Zero);
			}
			private static bool EnumWindow(IntPtr hwnd, IntPtr lParam) {
				HwndSource source = HwndSource.FromHwnd(hwnd);
				if(source != null) {
					if(source.RootVisual.GetType().Name.EndsWith("PopupRoot")) {
						Win32.SendMessage(hwnd, 0x0010, 0, IntPtr.Zero); 
					}
				}
				return true; 
			}
			private delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);
			[DllImport("user32.dll", SetLastError = false)]
			static extern IntPtr GetDesktopWindow();
		}
		void CheckDemo(ModuleDescription module, bool checkMemoryLeaks) {
			IAmAlive("Loading");
			CreateSetCurrentDemoActions(module, checkMemoryLeaks);
			IAmAlive("Switching themes");
			if(SwitchAllThemes(module.ModuleType)) {
				CreateSwitchAllThemesActions();
			} else {
				CreateSwitchNextThemeAction();
			}
			IAmAlive("Checking options");
			CreateCheckOptionsAction();
			if(module.Product.Name != "DXPdfViewer") { 
				IAmAlive("Checking code files");
				CreateSwitchToCodeTabActions(module.Name);
			}
			IAmAlive("Switching back to demo tab");
			CreateSwitchToDemoTabActions(module.Name);
		}
		protected virtual bool SwitchAllThemes(Type moduleType) {
			return DemoBaseTesting.Modules[0].ModuleType == moduleType;
		}
		protected virtual bool CanRunModule(Type moduleType) {
			return true;
		}
		protected virtual void CreateCheckOptionsAction() { }
		protected virtual bool AllowSwitchToTheTheme(Type moduleType, Theme theme) {
			return theme != Theme.HybridApp;
		}
		protected virtual bool AllowCheckCodeFile(Type moduleType, CodeLanguage fileLanguage) {
			return true;
		}
		protected virtual void CreateSwitchNextThemeAction() {
			IAmAlive("Getting next theme");
			Theme nextTheme = GetNextTheme();
			IAmAlive("Switching to next theme");
			CreateSwitchThemeActions(nextTheme);
			IAmAlive("Switching to default theme");
			CreateSwitchThemeActions(defaultTheme);
			CurrentTheme = nextTheme;
		}
		protected virtual void CreateSwitchAllThemesActions() {
			foreach(Theme theme in Theme.Themes) {
				if(theme.Name == DemoThemeName) continue;
				IAmAlive("Switching to theme: " + theme);
				CreateSwitchThemeActions(theme);
			}
			IAmAlive("Switching to default theme: " + defaultTheme);
			CreateSwitchThemeActions(defaultTheme);
		}
		protected virtual void CreateCheckDemoDescriptionAction(object moduleID) { }
		void CreateSwitchToCodeTabActions(object moduleID) {
			DispatchAsync(DemoBaseTesting.ShowCode);
			DispatchBusyWait(() => DemoBaseTesting.IsCodeOpen);
			int filesCount = DispatchExpr(() => DemoBaseTesting.GetCurrentModuleSourcesCount());
			if(filesCount == 0)
				return;
			if(filesCount == 1) {
				string error = DispatchExpr(() => CheckCodeText());
				AssertLog.IsTrue(string.IsNullOrEmpty(error), error);
			}
			foreach(int i in Enumerable.Range(1, filesCount - 1).Union(new[] { 0 })) {
				string prevCodeFileName = DispatchExpr(() => DemoBaseTesting.CodeFileName);
				Dispatch(() => DemoBaseTesting.ShowCodeFile(i));
				DispatchBusyWait(() => DemoBaseTesting.CodeFileName != prevCodeFileName);
				string error = DispatchExpr(() => CheckCodeText());
				AssertLog.IsTrue(string.IsNullOrEmpty(error), error);
			}
		}
		string CheckCodeText() {
			string fileName = DemoBaseTesting.CodeFileName;
			CodeLanguage fileLanguage;
			if(fileName.EndsWith(".xaml"))
				fileLanguage = CodeLanguage.XAML;
			else if(fileName.EndsWith(".cs"))
				fileLanguage = CodeLanguage.CS;
			else if(fileName.EndsWith(".vb"))
				fileLanguage = CodeLanguage.VB;
			else
				fileLanguage = CodeLanguage.Plain;
			Type moduleType = DemoBaseTesting.CurrentDemoModule.GetType();
			if(!AllowCheckCodeFile(moduleType, fileLanguage)) return string.Empty;
			bool isMainXamlFile = fileLanguage == CodeLanguage.XAML && fileName == moduleType.Name + ".xaml";
			bool isMainCSFile = fileLanguage == CodeLanguage.CS && fileName == moduleType.Name + ".xaml.cs";
			bool isMainVBFile = fileLanguage == CodeLanguage.VB && fileName == moduleType.Name + ".xaml.vb";
			if(isMainXamlFile) return CheckMainXamlCodeText(fileName, DemoBaseTesting.CodeText);
			if(isMainCSFile) return CheckMainCSCodeText(fileName, DemoBaseTesting.CodeText);
			if(isMainVBFile) return CheckMainVBCodeText(fileName, DemoBaseTesting.CodeText);
			if(fileLanguage == CodeLanguage.XAML) return CheckAnotherXamlText(fileName, DemoBaseTesting.CodeText);
			if(fileLanguage == CodeLanguage.CS) return CheckAnotherCSCodeText(fileName, DemoBaseTesting.CodeText);
			if(fileLanguage == CodeLanguage.VB) return CheckAnotherVBCodeText(fileName, DemoBaseTesting.CodeText);
			return string.Empty;
		}
		string CheckMainXamlCodeText(string fileName, string text) {
			string errorMessage = String.Format("incorrect XAML in {0} - {1}", DemoBaseTesting.CurrentDemoModule.GetType().FullName, fileName);
			if(1 >= text.Split('\n').Length)
				return errorMessage + "(Empty)";
			if(!text.Contains(@"http://schemas.microsoft.com/winfx/2006/xaml/presentation"))
				return errorMessage + "(No Schemas)";
			if(!text.Contains(String.Format("x:Class=\"{0}\"", DemoBaseTesting.CurrentDemoModule.GetType().FullName)))
				return errorMessage + "(No ClassName)";
			string directivesError = CheckDirectivesHelper.CheckXAML(text);
			if(!string.IsNullOrEmpty(directivesError))
				return directivesError;
			return string.Empty;
		}
		string CheckMainCSCodeText(string fileName, string text) {
			string errorMessage = String.Format("incorrect CS code in {0} - {1}", DemoBaseTesting.CurrentDemoModule.GetType().FullName, fileName);
			if(1 >= text.Split('\n').Length)
				return errorMessage + "(Empty)";
			if(!text.Contains(DemoBaseTesting.CurrentDemoModule.GetType().Namespace))
				return errorMessage + "(No NameSpace)";
			if(!text.Contains(DemoBaseTesting.CurrentDemoModule.GetType().Name))
				return errorMessage + "(No Type Name)";
			return string.Empty;
		}
		string CheckMainVBCodeText(string fileName, string text) {
			string errorMessage = String.Format("incorrect VB code in {0} - {1}", DemoBaseTesting.CurrentDemoModule.GetType().FullName, fileName);
			if(1 >= text.Split('\n').Length)
				return errorMessage + "(Empty)";
			if(!text.Contains(DemoBaseTesting.CurrentDemoModule.GetType().Namespace))
				return errorMessage + "(No NameSpace)";
			if(!text.Contains(DemoBaseTesting.CurrentDemoModule.GetType().Name))
				return errorMessage + "(No Type Name)";
			return string.Empty;
		}
		string CheckAnotherXamlText(string fileName, string text) {
			string errorMessage = String.Format("incorrect XAML in {0} - {1}", DemoBaseTesting.CurrentDemoModule.GetType().FullName, fileName);
			if(1 >= text.Split('\n').Length)
				return errorMessage + "(Empty)";
			if(!text.Contains(@"http://schemas.microsoft.com/winfx/2006/xaml/presentation"))
				return errorMessage + "(No Schemas)";
			string directivesError = CheckDirectivesHelper.CheckXAML(text);
			if(!string.IsNullOrEmpty(directivesError))
				return directivesError;
			return string.Empty;
		}
		string CheckAnotherCSCodeText(string fileName, string text) {
			string errorMessage = String.Format("incorrect CS code in {0} - {1}", DemoBaseTesting.CurrentDemoModule.GetType().FullName, fileName);
			if(1 >= text.Split('\n').Length)
				return errorMessage + "(Empty)";
			return string.Empty;
		}
		string CheckAnotherVBCodeText(string fileName, string text) {
			string errorMessage = String.Format("incorrect VB code in {0} - {1}", DemoBaseTesting.CurrentDemoModule.GetType().FullName, fileName);
			if(1 >= text.Split('\n').Length)
				return errorMessage + "(Empty)";
			return string.Empty;
		}
		void CreateSwitchToDemoTabActions(object moduleID) {
			DispatchAsync(DemoBaseTesting.ShowDemo);
			BusyWait(() => DemoBaseTesting.IsDemoOpen);
		}
		bool CanSwitchToTheme(Theme theme) {
			if(!AllowSwitchToTheTheme(DemoBaseTesting.CurrentDemoModule == null ? null : DemoBaseTesting.CurrentDemoModule.GetType(), theme)) return false;
			return ThemeManager.ActualApplicationThemeName != (theme == null ? Theme.Default.Name : theme.Name);
		}
		void SubscribeThemeChanged(ThemeChangedRoutedEventHandler handler) { ThemeManager.ThemeChanged += handler; }
		void UnsubscribeThemeChanged(ThemeChangedRoutedEventHandler handler) { ThemeManager.ThemeChanged -= handler; }
		void CreateSwitchThemeActions(Theme theme) {
			if(!CanSwitchToTheme(theme))
				return;
			bool invoked = false;
			ThemeChangedRoutedEventHandler d = (s, e) => {
				invoked = true;
			};
			SubscribeThemeChanged(d);
			DispatchAsync(() => ThemeManager.ApplicationThemeName = theme == null ? null : theme.Name);
			BusyWait(() => invoked);
			UnsubscribeThemeChanged(d);
		}
		void CheckPopupInnerElements(DependencyObject item) {
			if(!IsPopupInNeededState(item as LookUpEditBase)) return;
			for(int i = 0; i < VisualTreeHelper.GetChildrenCount(item); ++i) {
				DependencyObject currItem = VisualTreeHelper.GetChild(item, i);
				if(currItem is TextBlock && ((TextBlock)currItem).Foreground is SolidColorBrush && ((SolidColorBrush)((TextBlock)currItem).Foreground).Color == Colors.White) {
					AssertLog.IsTrue(false, "Foreground for PopupBaseEdit's items can't be set to \"White\"");
				} else {
					CheckPopupInnerElements(currItem);
				}
			}
		}
		bool IsPopupInNeededState(LookUpEditBase popup) {
			if(popup != null && !popup.IsPopupOpen) {
				popup.PopupOpened += OnPopupOpened;
				popup.ShowPopup();
				return false;
			}
			return true;
		}
		void OnPopupOpened(object sender, RoutedEventArgs e) {
			((LookUpEditBase)sender).PopupOpened -= OnPopupOpened;
			CheckPopupInnerElements(LookUpEditHelper.GetVisualClient(sender as LookUpEditBase).InnerEditor);
		}
		Theme GetNextTheme() {
			bool currentFound = false;
			bool firstThemeAssigned = false;
			Theme result = null;
			foreach(Theme theme in Theme.Themes) {
				if(theme.Name == DemoThemeName) continue;
				if(currentFound) {
					result = theme;
					break;
				}
				if(CurrentTheme == theme)
					currentFound = true;
				if(!firstThemeAssigned) {
					result = theme;
					firstThemeAssigned = true;
				}
			}
			return result;
		}
	}
}
