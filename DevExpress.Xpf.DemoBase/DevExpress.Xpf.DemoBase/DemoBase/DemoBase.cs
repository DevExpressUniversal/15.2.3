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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.DemoData;
using DevExpress.DemoData.Core;
using DevExpress.DemoData.Helpers;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.Xpf.DemoBase.Internal;
using System.Diagnostics;
using DevExpress.Mvvm;
using System.IO;
using System.Windows.Threading;
using System.Threading.Tasks;
using DevExpress.Xpf.DemoBase.DemoTesting;
using DevExpress.Images;
using DevExpress.DemoData.Model;
using DevExpress.DemoData.DemoParts;
namespace DevExpress.Xpf.DemoBase {
	sealed class DemoBase : BindableBase, IBackButton {
		Assembly demoAssembly;
		string currentlyLoadedProductName;
		string moduleName;
		DemoBaseColorHelper colorHelper;
		public DemoBase(Assembly demoAssembly, string moduleName, StartupBase startup) {
			this.currentlyLoadedProductName = DemoHelper.GetProductName(demoAssembly);
			this.demoAssembly = demoAssembly;
			this.moduleName = moduleName;
			DataLoader.ModuleResolver = new XpfDemoModuleResolver(demoAssembly, typeof(DemoBase).Assembly);
			DemoBaseControl = new DemoBaseControl(PlatformName, new DataLoaderActions {
				OnModuleDescriptionSelected = OnModuleDescriptionSelected,
				OnProductDescriptionSelected = OnProductDescriptionSelected,
				OnModuleLinkDescriptionClick = OnModuleLinkDescriptionClick,
				StartFeaturedDemo = StartFeaturedDemo
			}, currentlyLoadedProductName, demoAssembly, moduleName, startup);
			colorHelper = new DemoBaseColorHelper();
			DemosIntercallHelper.RegisterDemoBackButton(this);
			ImagesAssemblyType = new ImagesAssemblyType();
		}
		public DemoBaseControl DemoBaseControl { get; private set; }
		void OnModuleLinkDescriptionClick(WpfModuleLink link) {
			switch(link.Type) {
				case WpfModuleLinkType.Video:
					DocumentPresenter.OpenTabLink(link.Url, OpenLinkType.Smart);
					break;
				case WpfModuleLinkType.KB:
					DocumentPresenter.OpenTabLink(link.Url, OpenLinkType.Smart);
					break;
				case WpfModuleLinkType.Documentation:
					DocumentPresenter.OpenTabLink(link.Url, OpenLinkType.Smart);
					break;
				case WpfModuleLinkType.Demos:
					string[] linkParts = link.Url.Split(':');
					if(linkParts.Length < 2)
						return;
					if(linkParts[0].Equals("local")) {
						ModuleDescription md = DemoBaseControl.PagesContainer.CompleteState.Product.Modules.FirstOrDefault(m => m.Name == linkParts[1]);
						if(md == null) {
							MessageBox.Show("Requested Demo not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
							break;
						}
						DemoBaseControl.PagesContainer.NavigateToMainPage(md);
					} else {
						DemosIntercallHelper.RunXpfDemo(linkParts[0], linkParts[1]);
					}
					break;
				case WpfModuleLinkType.Blogs:
					DocumentPresenter.OpenTabLink(link.Url, OpenLinkType.Smart);
					break;
			}
		}
		void StartFeaturedDemo(ReallifeDemo demo) {
			if(EnvironmentHelper.IsClickOnce) {
				DemosIntercallHelper.RunXpfDemo(Path.GetFileNameWithoutExtension(demo.LaunchPath), string.Empty, true);
			} else {
				DemoRunner.TryStartExecutableAndShowErrorMessage(demo, CallingSite.WpfDemoLauncher, new DefaultDemoRunnerMessageBox());
			}
		}
		void OnProductDescriptionSelected(ProductDescription pd) {
			if(pd.Modules.Count == 1) {
				DemoBaseControl.PagesContainer.NavigateToMainPage(pd.Modules.First());
			} else {
				DemoBaseControl.PagesContainer.NavigateToModulesPage(pd, "");
			}
		}
		void OnModuleDescriptionSelected(ModuleDescription module) {
			DemoBaseControl.PagesContainer.NavigateToMainPage(module);
		}
		string PlatformName {
			get { return DataLoader.DefaultPlatform.Name; }
		}
		private void UpdateBackButtonVisibility(DemoBaseControlPage newPage) {
			backButtonDesiredVisibility = newPage != DemoBaseControlPage.Products;
			if(backButtonDesiredVisibilityChanged != null)
				backButtonDesiredVisibilityChanged(this, EventArgs.Empty);
		}
		#region IBackButton
		EventHandler backButtonDesiredVisibilityChanged;
		bool backButtonDesiredVisibility;
		bool temp;
		bool IBackButton.ActualVisibility {
			get { return temp; }
			set { temp = value; }
		}
		event EventHandler IBackButton.Click {
			add { }
			remove { }
		}
		bool IBackButton.DesiredVisibility {
			get { return backButtonDesiredVisibility; }
		}
		event EventHandler IBackButton.DesiredVisibilityChanged {
			add { backButtonDesiredVisibilityChanged += value; }
			remove { backButtonDesiredVisibilityChanged -= value; }
		}
		void IBackButton.DoClick() { }
		#endregion
		class XpfDemoModuleResolver : IXpfDemoModuleResolver {
			Assembly[] assemblies;
			public XpfDemoModuleResolver(params Assembly[] assemblies) {
				this.assemblies = assemblies;
			}
			public Type GetModuleType(WpfModule module, Assembly demoAssembly) {
				var allAssemblies = new List<Assembly>();
				if(demoAssembly != null) {
					allAssemblies.Add(demoAssembly);
				}
				allAssemblies.AddRange(assemblies);
				return allAssemblies.Select(a => a.GetType(module.Type)).FirstOrDefault(t => t != null);
			}
		}
		#region Testing
		class DemoBaseTesting : IDemoBaseTesting {
			DemoBase demoBase;
			public DemoBaseTesting(DemoBase demoBase) {
				this.demoBase = demoBase;
			}
			public Assembly DemoAssembly {
				get {
					return this.demoBase.demoAssembly;
				}
			}
			public bool IsReady { get { return this.demoBase.DemoBaseControl.Data.Products != null; } }
			public FrameworkElement ResetFocusElement { get { return this.demoBase.DemoBaseControl; } }
			public FrameworkElement CurrentDemoModule { get { return this.demoBase.DemoBaseControl.CurrentDemoModule; } }
			public DemoModuleControl CurrentDemoModuleControl { get { return ((DemoModule)demoBase.DemoBaseControl.CurrentDemoModule).DemoModuleControl; } }
			public Exception DemoModuleException { get { return this.demoBase.DemoBaseControl.DemoModuleException; } }
			public IList<ModuleDescription> Modules {
				get { return demoBase.DemoBaseControl.Data.MainProduct.Modules; }
			}
			public int GetCurrentModuleSourcesCount() {
				DemoModule currentDemoModule = (DemoModule)CurrentDemoModule;
				return DemoHelper.GetCodeTexts(DemoAssembly, currentDemoModule.GetCodeFileNames()).Count;
			}
			public ModuleDescription CurrentModule {
				get {
					CompletePageState state = demoBase.DemoBaseControl.PagesContainer.CompleteState;
					return state == null ? null : state.Module;
				}
			}
			public void LoadModule(ModuleDescription module, bool reloadIfNeeded) {
				var moduleName = (module == null ? "<null>" : module.Name);
				DemoTestingHelper.Log("start DemoBaseTesting.LoadModule " + moduleName);
				Action backgroundAction = new Action(() => {
					while(demoBase.DemoBaseControl.PagesContainer.CompleteState == null) {
						Thread.Sleep(25);
					}
				});
				Action mainThreadAction = new Action(() => {
					bool canGetCurrentModule = demoBase.DemoBaseControl != null
						&& demoBase.DemoBaseControl.PagesContainer != null
						&& demoBase.DemoBaseControl.PagesContainer.CompleteState != null;
					if(module != null && (!reloadIfNeeded || (canGetCurrentModule && demoBase.DemoBaseControl.PagesContainer.CompleteState.Module != module)))
						demoBase.DemoBaseControl.PagesContainer.NavigateToMainPage(module);
					else
						demoBase.DemoBaseControl.ReloadModule();
					DemoTestingHelper.Log("real finish DemoBaseTesting.LoadModule" + moduleName);
				});
				BackgroundHelper.DoInBackground(backgroundAction, mainThreadAction);
				DemoTestingHelper.Log("finish DemoBaseTesting.LoadModule" + moduleName);
			}
			public object SubscribeToModuleAppear(EventHandler handler) {
				EventHandler<ModuleAppearEventArgs> d = (s, e) => handler(s, e);
				demoBase.DemoBaseControl.ModuleAppear += d;
				return d;
			}
			public void UnsubscribeFromModuleAppear(object handler) {
				EventHandler<ModuleAppearEventArgs> d = (EventHandler<ModuleAppearEventArgs>)handler;
				demoBase.DemoBaseControl.ModuleAppear -= d;
			}
			public void ShowCode() { demoBase.DemoBaseControl.DemoModuleView = ToolbarView.Code; }
			public void ShowDemo() { demoBase.DemoBaseControl.DemoModuleView = ToolbarView.Demo; }
			public void ShowCodeFile(int index) {
				CurrentDemoModuleControl.CodeTextViewer.SelectedItem = CurrentDemoModuleControl.CodeTextViewer.ItemsSource[index];
			}
			public void ShowThemeSelector() {
				demoBase.DemoBaseControl.DemoModuleOptionsView = ToolbarSidebarView.ClassicThemes;
			}
			public bool IsThemeSelectorHidden {
				get { return demoBase.DemoBaseControl.DemoModuleOptionsView != ToolbarSidebarView.ClassicThemes; }
			}
			public bool IsDemoOpen {
				get {
					return demoBase.DemoBaseControl.DemoModuleView == ToolbarView.Demo && !demoBase.DemoBaseControl.IsLoading;
				}
			}
			public bool IsCodeOpen {
				get {
					return demoBase.DemoBaseControl.DemoModuleView == ToolbarView.Code
						&& !demoBase.DemoBaseControl.IsLoading 
						&& CurrentDemoModuleControl.CodeTextViewer.IsTemplateApplied;
				}
			}
			public string CodeFileName {
				get {
					CodeTextDescription d = (CodeTextDescription)CurrentDemoModuleControl.CodeTextViewer.SelectedItem;
					return d.FileName;
				}
			}
			public string CodeText { get { return CurrentDemoModuleControl.CodeTextViewer.GetDisplayedText(); } }
			public string DemoModuleDescription {
				get {
					RichTextBox richTextBox = (RichTextBox)DevExpress.Xpf.Core.Native.LayoutHelper.FindElement(CurrentDemoModuleControl.DescriptionContentContainer, (fe) => fe is RichTextBox);
					return RichTextBoxHelper.GetText(richTextBox);
				}
			}
			public void ShowDemoModuleDescription() {
				demoBase.DemoBaseControl.DemoModuleOptionsView = ToolbarSidebarView.About;
			}
			public void HideDemoModuleDescription() {
				demoBase.DemoBaseControl.DemoModuleOptionsView = ToolbarSidebarView.None;
			}
			public bool IsDemoModuleDescriptionHidden {
				get { return demoBase.DemoBaseControl.DemoModuleOptionsView != ToolbarSidebarView.About; }
			}
		}
		DemoBaseTesting testing;
		public IDemoBaseTesting Testing {
			get {
				if(testing == null)
					testing = new DemoBaseTesting(this);
				return testing;
			}
		}
		#endregion
		ImagesAssemblyType ImagesAssemblyType { get; set; }
	}
	public interface IXpfDemoModuleResolver {
		Type GetModuleType(WpfModule module, Assembly assembly);
	}
}
