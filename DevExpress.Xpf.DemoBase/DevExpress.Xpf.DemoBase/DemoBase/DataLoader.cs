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

using DevExpress.DemoData;
using DevExpress.DemoData.Core;
using DevExpress.DemoData.Helpers;
using DevExpress.Xpf.DemoBase.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.DemoBase.DemoTesting;
using DevExpress.DemoData.Utils;
using DevExpress.Mvvm;
using DevExpress.DemoData.Model;
using System.IO;
namespace DevExpress.Xpf.DemoBase {
	public class DataLoaderActions {
		public Action<WpfModuleLink> OnModuleLinkDescriptionClick;
		public Action<ReallifeDemo> StartFeaturedDemo;
		public Action<ModuleDescription> OnModuleDescriptionSelected;
		public Action<ProductDescription> OnProductDescriptionSelected;
	}
	public static class DataExtensions {
		public static LString AsLString(this string str) {
			return LString.Get(l => str);
		}
	}
	public class DataLoader {
		string platformName;
		string mainProductID;
		Platform platform;
		DataLoaderActions actions;
		DemoBaseColorHelper colorHelper = new DemoBaseColorHelper();
		public List<ProductDescription> Products { get; private set; }
		public ProductDescription MainProduct { get; private set; }
		public LString Title { get; private set; }
		public static IXpfDemoModuleResolver ModuleResolver { get; set; }
		public DataLoader(string platform, DataLoaderActions actions, string mainProjectID) {
			this.platformName = platform;
			this.mainProductID = mainProjectID;
			this.actions = actions;
		}
		internal static Platform DefaultPlatform = Repository.WpfPlatform;
		public PlatformDescription LoadCurrentPlatform() {
			var platformDescription = new PlatformDescription();
			platformDescription.ID = new PlatformID();
			platform = DefaultPlatform;
			platformDescription.IsAvailable = platform.IsInstalled;
			platformDescription.IsLicensed = platform.IsLicensed;
			platformDescription.Title = platform.ProductListTitle.AsLString();
			platformDescription.Subtitle = platform.ProductListSubtitle.AsLString();
			platformDescription.Path = platform.Name;
			return platformDescription;
		}
		public void LoadProducts() {
			List<ProductDescription> data = new List<ProductDescription>();
			foreach(var demo in platform.ReallifeDemos) {
				if(EnvironmentHelper.IsClickOnce && !demo.IsAvailableInClickonce) continue;
				data.Add(LoadFeaturedDemo(demo, platform));
			}
			List<string> groups = new List<string>();
			Dictionary<string, List<ProductDescription>> pds = new Dictionary<string, List<ProductDescription>>();
			foreach(var product in platform.Products) {
				string groupNL = product.Group;
				List<ProductDescription> groupPds;
				if(!pds.TryGetValue(groupNL, out groupPds)) {
					groupPds = new List<ProductDescription>();
					groups.Add(groupNL);
					pds.Add(groupNL, groupPds);
				}
				ProductDescription pd = LoadProduct(product);
				groupPds.Add(pd);
			}
			foreach(string group in groups) {
				List<ProductDescription> groupPds = pds[group];
				foreach(ProductDescription pd in groupPds) {
					data.Add(pd);
				}
			}
			Products = data;
		}
		ProductDescription LoadProduct(Product product) {
			ProductDescription pd = new ProductDescription(this, product);
			pd.DisplayName = product.DisplayName;
			pd.Name = product.Name;
			pd.ExamplesProductName = product.Id.ToString();
			pd.GroupTitle = product.Group.AsLString();
			pd.Description = product.Description.AsLString();
			pd.ShortDescription = product.ShortDescription.AsLString();
			pd.IsLicensed = product.IsLicensed;
			pd.IsFeaturedDemo = false;
			pd.IsAvailable = product.IsInstalled;
			pd.Title = product.DisplayName.AsLString();
			pd.DemosLoad += (s, e) => pd_DemosLoad(s, e, product);
			pd.CSSolutionPath = Path.GetDirectoryName(product.Demos.First().CsSolutionPath);
			pd.VBSolutionPath = Path.GetDirectoryName(product.Demos.First().VbSolutionPath);
			pd.Solutions = new List<SolutionLink>();
			if(!EnvironmentHelper.IsClickOnce) {
				pd.Solutions = product.Demos.First().CreateOpenSolutionMenu(CallingSite.WpfDemoLauncher)
				   .Select(i => new SolutionLink {
					   OpenCommand = i.OpenCommand,
					   Title = i.Title.AsLString()
				   }).ToList();
			}
			pd.Selected += (s, e) => actions.OnProductDescriptionSelected(pd);
			return pd;
		}
		void pd_DemosLoad(object sender, DataLoadEventArgs<ReadOnlyCollection<DemoDescription>> e, Product product) {
			var demos = new List<DemoDescription>();
			foreach(var demo in product.Demos) {
				demos.Add(LoadDemo((WpfDemo)demo, product));
			}
			e.Data = new ReadOnlyCollection<DemoDescription>(demos);
		}
		DemoDescription LoadDemo(WpfDemo demo, Product product) {
			var demoDescription = new DemoDescription();
			demoDescription.Title = demo.DisplayName.AsLString();
			demoDescription.IsAvailable = product.IsInstalled;
			return demoDescription;
		}
		public ReadOnlyCollection<ModuleDescription> LoadModules(ProductDescription pd, WpfDemo demo) {
			List<string> groups = new List<string>();
			Dictionary<string, List<ModuleDescription>> mds = new Dictionary<string, List<ModuleDescription>>();
			foreach(var module in demo.Modules.Cast<WpfModule>()) {
				string groupNL = module.Group;
				List<ModuleDescription> groupMds;
				if(!mds.TryGetValue(groupNL, out groupMds)) {
					groupMds = new List<ModuleDescription>();
					groups.Add(groupNL);
					mds.Add(groupNL, groupMds);
				}
				bool isCurrentDemo = pd.Name == this.mainProductID;
				ModuleDescription md = LoadModule(isCurrentDemo, demo, module);
				md.Product = pd;
				if(pd.Name != "DXEditors" || ShouldShowDXEditorsModule(md)) {
					groupMds.Add(md);
				}
			}
			List<ModuleDescription> data = new List<ModuleDescription>();
			foreach(string group in groups) {
				List<ModuleDescription> groupMds = mds[group];
				foreach(ModuleDescription md in groupMds) {
					data.Add(md);
				}
			}
			return data.AsReadOnly();
		}
		bool ShouldShowDXEditorsModule(ModuleDescription md) {
			return true;
		}
		public static string GetSearchPattern(LString searchText) {
			if(searchText == null)
				return string.Empty;
			return GetSearchPattern(searchText.Text);
		}
		public static string GetSearchPattern(string searchText) {
			if(string.IsNullOrEmpty(searchText)) return string.Empty;
			return searchText.Replace("/", "").Replace("-", "").ToLower();
		}
		ModuleDescription LoadModule(bool isCurrentDemo, WpfDemo demo, WpfModule module) {
			ModuleDescription md = new ModuleDescription();
			md.DemoAssemblyName = demo.AssemblyName;
			md.Name = module.Name;
			md.Title = module.DisplayName.AsLString();
			md.AllowRtl = module.AllowRtl;
			md.GroupName = module.Group;
			md.GroupTitle = module.Group.AsLString();
			md.IsHighlighted = module.IsFeatured;
			md.IsNew = module.IsNew;
			md.IsUpdated = module.IsUpdated;
			md.Description = module.Description.AsLString();
			md.ShortDescription = module.ShortDescription.AsLString();
			md.SupportTouchThemes = module.AllowTouchThemes;
			md.SupportDarkThemes = module.AllowDarkThemes;
			md.AllowSwitchingTheme = module.AllowSwitchingThemes;
			md.Links = LoadLinks(demo.Product, module);
			md.SearchPattern = GetSearchPattern(md.Title).Replace(" ", "")
					+ " " + GetSearchPattern(md.GroupTitle).Replace(" ", "")
					+ " " + GetSearchPattern(md.Tags);
			md.Selected += new EventHandler((s, e) => actions.OnModuleDescriptionSelected((ModuleDescription)s));
			md.UpdateModuleType = a => {
				Type moduleType = ModuleResolver.GetModuleType(module, a);
				if(moduleType == null)
					throw new Exception(string.Format("DemoModule *{0}* not found", module.Type));
				md.ModuleType = moduleType;
			};
			if(isCurrentDemo) {
				try { md.UpdateModuleType(null); } catch { }
			}
			return md;
		}
		ReadOnlyCollection<ReadOnlyCollection<ModuleLinkDescription>> LoadLinks(Product product, WpfModule module) {
			var groups = new List<WpfModuleLinkType>();
			var links = new Dictionary<WpfModuleLinkType, List<ModuleLinkDescription>>();
			foreach(var link in module.Links) {
				List<ModuleLinkDescription> groupLinks;
				if(!links.TryGetValue(link.Type, out groupLinks)) {
					groups.Add(link.Type);
					groupLinks = new List<ModuleLinkDescription>();
					links.Add(link.Type, groupLinks);
				}
				ModuleLinkDescription d = new ModuleLinkDescription();
				d.Title = link.Title.AsLString();
				d.Icon = GeneralizedUri.GetUri(ResourceUri.GetUriString(typeof(DemoBase).Assembly, "Images/Links/" + link.Type.ToString() + ".png", true));
				d.Click += (s, e) => actions.OnModuleLinkDescriptionClick(link);
				groupLinks.Add(d);
			}
			List<ReadOnlyCollection<ModuleLinkDescription>> data = new List<ReadOnlyCollection<ModuleLinkDescription>>();
			foreach(var group in groups) {
				data.Add(links[group].AsReadOnly());
			}
			return data.AsReadOnly();
		}
		ProductDescription LoadFeaturedDemo(ReallifeDemo featuredDemo, Platform platform) {
			ProductDescription pd = new ProductDescription(this, null);
			LoadFeatureDemoCommon(featuredDemo, pd, platform);
			pd.Solutions = new List<SolutionLink>();
			if(!EnvironmentHelper.IsClickOnce) {
				pd.Solutions = featuredDemo.CreateOpenSolutionMenu(CallingSite.WpfDemoLauncher)
					.Select(i => new SolutionLink {
						OpenCommand = i.OpenCommand,
						Title = i.Title.AsLString()
					}).ToList();
			}
			pd.Selected += (s, e) => actions.StartFeaturedDemo(featuredDemo);
			return pd;
		}
		private void LoadFeatureDemoCommon(ReallifeDemo demo, ProductDescription pd, Platform platform) {
			pd.Name = demo.Name;
			pd.DisplayName = demo.DisplayName;
			pd.GroupTitle = (demo.Group ?? DemoDataSettings.ShowCasesTitle).AsLString();
			pd.Screenshot = GeneralizedUri.GetUri(demo.LargeImage.Uri.ToString());
			pd.IsFeaturedDemo = true;
			pd.IsAvailable = demo.Platform.IsInstalled;
			pd.Title = demo.DisplayName.AsLString();
		}
	}
}
