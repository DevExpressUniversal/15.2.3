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
using DevExpress.DemoData.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer;
using System.Reflection;
using DevExpress.DemoData.Utils;
using System.Threading.Tasks;
using DevExpress.DemoData;
using DevExpress.Mvvm;
using DevExpress.DemoData.Model;
namespace DevExpress.Xpf.DemoBase {
	public sealed class DemoBaseControlData : BindableBase {
		public LString Title { get; private set; }
		string mainProductName;
		public ReadOnlyCollection<ProductDescription> Products { get; set; }
		public ProductDescription MainProduct {
			get { return Products.FirstOrDefault(p => p.Name == mainProductName); }
		}
		public DemoBaseControlData(string platform, DataLoaderActions actions, string productId) {
			var loader = new DataLoader(platform, actions, productId);
			this.mainProductName = productId;
			loader.LoadCurrentPlatform();
			loader.LoadProducts();
			Products = loader.Products.AsReadOnly();
			foreach(var product in loader.Products) {
				product.LoadModules();
			}
		}
	}
	public sealed class PlatformDescription : BindableBase {
		public object ID { get; set; }
		public bool IsAvailable { get; set; }
		public bool IsLicensed { get; set; }
		public GeneralizedUri Preview { get; set; }
		public LString Title { get; set; }
		public LString Subtitle { get; set; }
		public string Path { get; set; }
	}
	sealed class CodeTextDescription {
		public string FileName { get; set; }
		public DefferableValue<string> CodeText { get; set; }
	}
	public sealed class SolutionLink : BindableBase {
		public LString Title { get; set; }
		public ICommand OpenCommand { get; set; }
	}
	public sealed class ProductDescription : BindableBase {
		bool demosLoaded = false;
		object modulesLoadedLock = new object();
		object demosLoadedLock = new object();
		ICommand selectCommand;
		DataLoader dataLoader;
		DevExpress.DemoData.Model.Product realProduct;
		public ProductDescription(DataLoader loader, DevExpress.DemoData.Model.Product realProduct) {
			IsFeaturedDemo = realProduct == null;
			dataLoader = loader;
			this.realProduct = realProduct;
		}
		public void OpenCSSolution(string[] filesToOpen) {
			DemoRunner.OpenCSSolution(realProduct.Demos.First(), CallingSite.WpfDemoLauncher, true, filesToOpen);
		}
		public void OpenVBSolution(string[] filesToOpen) {
			DemoRunner.OpenVBSolution(realProduct.Demos.First(), CallingSite.WpfDemoLauncher, true, filesToOpen);
		}
		public string Filter { get; set; }
		public bool IsFeaturedDemo { get; set; }
		public bool IsAvailable { get; set; }
		public string DisplayName { get; set; }
		public string Name { get; set; }
		public LString Title { get; set; }
		public GeneralizedUri Icon { get; set; }
		public LString GroupTitle { get; set; }
		public ReadOnlyCollection<ModuleDescription> Modules { get; set; }
		public ReadOnlyCollection<DemoDescription> Demos { get; set; }
		public GeneralizedUri Screenshot { get; set; }
		public LString Description { get; set; }
		public LString DescriptionHeader { get; set; }
		public string DemoStatus { get; set; }
		public LString ShortDescription { get; set; }
		public Color TileBackground { get; set; }
		public bool IsLicensed { get; set; }
		public List<SolutionLink> Solutions { get; set; }
		public ICommand OpenCSCommand { get; set; }
		public ICommand OpenVBCommand { get; set; }
		public string OpenCSTitleNL { get; set; }
		public string OpenVBTitleNL { get; set; }
		public string CSSolutionPath { get; set; }
		public string VBSolutionPath { get; set; }
		public bool IsMultiDemo { get; set; }
		public string ExamplesProductName { get; set; }
		public event EventHandler<DataLoadEventArgs<ReadOnlyCollection<DemoDescription>>> DemosLoad;
		public event ThePropertyChangedEventHandler<ReadOnlyCollection<DemoDescription>> DemosChanged;
		public event EventHandler Selected;
		public event EventHandler ModuleSearchPatternChanged;
		public ModuleDescription AdvanceModule(ModuleDescription module, int delta) {
			int index = Modules.IndexOf(module);
			if(index < 0) return null;
			index = (index + Modules.Count + delta) % Modules.Count;
			return Modules[index];
		}
		public void Select() {
			if(Selected != null)
				Selected(this, EventArgs.Empty);
		}
		public void LoadModules() {
			if(IsFeaturedDemo)
				return;
			Modules = dataLoader.LoadModules(this, (DevExpress.DemoData.Model.WpfDemo)realProduct.Demos.First());
		}
		public void LoadDemos(Action<Action> dispatcher) {
			lock(demosLoadedLock) {
				if(demosLoaded) return;
				demosLoaded = true;
			}
			var e = new DataLoadEventArgs<ReadOnlyCollection<DemoDescription>>();
			if(DemosLoad != null)
				DemosLoad(this, e);
			if(e.Data == null)
				e.Data = new List<DemoDescription>().AsReadOnly();
			dispatcher(() => Demos = e.Data);
		}
		public ICommand SelectCommand {
			get {
				if(selectCommand == null)
					selectCommand = new DelegateCommand(Select);
				return selectCommand;
			}
		}
		void RaiseDemosChanged() {
			if(DemosChanged != null)
				DemosChanged(this, new ThePropertyChangedEventArgs<ReadOnlyCollection<DemoDescription>>(null, Demos));
		}
		void OnModuleSearchPatternChanged(object sender, EventArgs e) {
			if(ModuleSearchPatternChanged != null)
				ModuleSearchPatternChanged(this, EventArgs.Empty);
		}
		public string PlatformLabel { get; set; }
		#region Equality
		public override int GetHashCode() {
			return Name.GetHashCode();
		}
		public static bool operator ==(ProductDescription d1, ProductDescription d2) {
			if(ReferenceEquals(d1, d2))
				return true;
			if(ReferenceEquals(d1, null) || ReferenceEquals(d2, null))
				return false;
			return d1.realProduct == d2.realProduct;
		}
		public override bool Equals(object obj) {
			return this == obj as ProductDescription;
		}
		public static bool operator !=(ProductDescription d1, ProductDescription d2) {
			return !(d1 == d2);
		}
		#endregion
	}
	public class ContextMenuItemDescription {
		ICommand launchCommand;
		public GeneralizedUri Icon { get; set; }
		public LString Header { get; set; }
		public Action LaunchAction { get; set; }
		public void Launch() {
			if(LaunchAction != null)
				LaunchAction();
		}
		public ICommand LaunchCommand {
			get {
				if(launchCommand == null)
					launchCommand = new DelegateCommand(Launch);
				return launchCommand;
			}
		}
	}
	public sealed class DemoDescription : BindableBase {
		ICommand selectCommand;
		public object ID { get; set; }
		public bool IsAvailable { get; set; }
		public bool IsLicensed { get; set; }
		public GeneralizedUri Preview { get; set; }
		public LString Title { get; set; }
		public string Path { get; set; }
		public LString Description { get; set; }
		public LString ShortDescription { get; set; }
		public Color ItemBackground { get; set; }
		public event EventHandler Selected;
		public string DemoGroupNL { get; set; }
		public LString GroupTitle { get; set; }
		public bool IsMultiVariant { get; set; }
		public ReadOnlyCollection<DemoDescription> Variants { get; set; }
		public string OpenCSTitleNL { get; set; }
		public string OpenVBTitleNL { get; set; }
		public string TitleNL { get; set; }
		public ICommand SelectCommand {
			get {
				if(selectCommand == null)
					selectCommand = new DelegateCommand(() => {
						if(Selected != null) {
							Selected(this, EventArgs.Empty);
						}
					});
				return selectCommand;
			}
		}
		public List<ContextMenuItemDescription> ContextMenu { get; set; }
		public ICommand OpenCSCommand { get; set; }
		public ICommand OpenVBCommand { get; set; }
		public bool IsVBLinkPresent { get { return OpenVBTitleNL != null; } }
	}
	enum ModuleModifier { None, New, Updated, Highlighted, HighlightedNew, HighlightedUpdated }
	public sealed class ModuleDescription : BindableBase {
		ICommand selectCommand;
		public Action<Assembly> UpdateModuleType { get; set; }
		public ProductDescription Product { get; set; }
		public string Tags { get; set; }
		public string DemoAssemblyName { get; set; }
		public string Name { get; set; }
		public LString Title { get; set; }
		public bool AllowRtl { get; set; }
		public string GroupName { get; set; }
		public LString GroupTitle { get; set; }
		public GeneralizedUri Icon { get; set; }
		public GeneralizedUri Preview { get; set; }
		public LString ShortDescription { get; set; }
		public LString Description { get; set; }
		public bool IsNew { get; set; }
		public bool IsUpdated { get; set; }
		public bool IsHighlighted { get; set; }
		public bool SupportTouchThemes { get; set; }
		public bool SupportDarkThemes { get; set; }
		public bool AllowSwitchingTheme { get; set; }
		public string SearchPattern { get; set; }
		public ReadOnlyCollection<ReadOnlyCollection<ModuleLinkDescription>> Links { get; set; }
		internal ModuleModifier Modifier { get; set; }
		public Color Color { get; set; }
		public Type ModuleType { get; set; }
		public event EventHandler Selected;
		public void Select() {
			if(Selected != null)
				Selected(this, EventArgs.Empty);
		}
		public ICommand SelectCommand {
			get {
				if(selectCommand == null)
					selectCommand = new DelegateCommand(Select);
				return selectCommand;
			}
		}
		void RaiseIsNewChanged() {
			UpdateModifier();
		}
		void RaiseIsUpdatedChanged() {
			UpdateModifier();
		}
		void RaiseIsHighlightedChanged() {
			UpdateModifier();
		}
		void UpdateModifier() {
			if(IsHighlighted) {
				Modifier = IsUpdated ? ModuleModifier.HighlightedUpdated : IsNew ? ModuleModifier.HighlightedNew : ModuleModifier.Highlighted;
			} else {
				Modifier = IsUpdated ? ModuleModifier.Updated : IsNew ? ModuleModifier.New : ModuleModifier.None;
			}
		}
		#region Equality
		public override int GetHashCode() {
			return Name.GetHashCode();
		}
		public static bool operator ==(ModuleDescription d1, ModuleDescription d2) {
			if(ReferenceEquals(d1, d2))
				return true;
			if(ReferenceEquals(d1, null) || ReferenceEquals(d2, null))
				return false;
			return d1.Name == d2.Name && d1.Product == d2.Product;
		}
		public override bool Equals(object obj) {
			return this == obj as ModuleDescription;
		}
		public static bool operator !=(ModuleDescription d1, ModuleDescription d2) {
			return !(d1 == d2);
		}
		#endregion
		public override string ToString() {
			return Name;
		}
	}
	public sealed class ModuleLinkDescription : BindableBase {
		ICommand clickCommand;
		public event EventHandler Click;
		public LString Title { get; set; }
		public GeneralizedUri Icon { get; set; }
		public void PerformClick() {
			if(Click != null)
				Click(this, EventArgs.Empty);
		}
		public ICommand ClickCommand {
			get {
				if(clickCommand == null)
					clickCommand = new DelegateCommand(PerformClick);
				return clickCommand;
			}
		}
	}
}
