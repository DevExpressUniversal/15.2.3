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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.DemoData.Model {
	public enum KnownDXVersion {
		Before142, V142, V151, V152
	}
	public class OpenSolutionMenuItem {
		public string Title { get; set; }
		public ICommand OpenCommand { get; set; }
	}
	public interface IExecutable {
		Requirement[] Requirements { get; }
		string LaunchPath { get; }
		string[] Arguments { get; }
		Platform Platform { get; }
		string DoEventMessage { get; }
	}
	public abstract class Module {
		public Module(Demo demo,
					  string name,
					  string displayName,
					  string group,
					  string type,
					  string description,
					  KnownDXVersion addedIn,
					  KnownDXVersion updatedIn = KnownDXVersion.Before142)
		{
			Demo = demo;
			Name = name;
			DisplayName = displayName.Replace("%MarketingVersion%", AssemblyInfo.MarketingVersion);
			Group = group;
			Type = type;
			Description = description;
			IsNew = addedIn == Repository.CurrentDXVersion;
			IsUpdated = updatedIn == Repository.CurrentDXVersion;
			if (IsNew && IsUpdated) {
				IsUpdated = false;
			}
		}
		public Demo Demo { get; private set; }
		public string Name { get; private set; }
		public string DisplayName { get; private set; }
		public string Group { get; private set; }
		public string Type { get; private set; }
		public string Description { get; private set; }
		public bool IsNew { get; private set; }
		public bool IsUpdated { get; private set; }
	}
	public abstract class Demo : IExecutable {
		List<Module> modules;
		KnownDXVersion addedIn;
		KnownDXVersion updatedIn;
		public Demo(Product product,
					Func<Demo, List<Module>> createModules,
					string name,
					string displayName,
					string csSolutionPath,
					string vbSolutionPath,
					KnownDXVersion addedIn = KnownDXVersion.Before142,
					KnownDXVersion updatedIn = KnownDXVersion.Before142,
					Requirement[] requirements = null,
					string color = "Auto")
		{
			Product = product;
			modules = createModules(this);
			Name = name;
			DisplayName = displayName;
			CsSolutionPath = csSolutionPath;
			VbSolutionPath = vbSolutionPath;
			Requirements = requirements ?? new Requirement[0]; ;
			Color = color;
			this.addedIn = addedIn;
			this.updatedIn = updatedIn;
		}
		public List<Module> Modules { get { return modules; } }
		public Product Product { get; private set; }
		public string Name { get; private set; }
		public string DisplayName { get; private set; }
		public string CsSolutionPath { get; private set; }
		public string VbSolutionPath { get; private set; }
		public Requirement[] Requirements { get; private set; }
		public string Color { get; private set; }
		public bool IsNew {
			get {
				return addedIn == Repository.CurrentDXVersion
					|| (Modules.Any() && Modules.All(m => m.IsNew));
			}
		}
		public bool IsUpdated { get { return updatedIn == Repository.CurrentDXVersion || Modules.Any(m => m.IsNew || m.IsUpdated); } }
		public IEnumerable<OpenSolutionMenuItem> CreateOpenSolutionMenu(CallingSite site) {
			if (CsSolutionPath != null) {
				yield return new OpenSolutionMenuItem {
					Title = "Open CS Solution",
					OpenCommand = new DevExpress.DemoData.Helpers.ActionToCommandConverter.DelegateCommand(
						() => DemoRunner.OpenCSSolution(this, site))
				};
			}
			if(VbSolutionPath != null) {
				yield return new OpenSolutionMenuItem {
					Title = "Open VB Solution",
					OpenCommand = new DevExpress.DemoData.Helpers.ActionToCommandConverter.DelegateCommand(
						() => DemoRunner.OpenVBSolution(this, site))
				};
			}
		}
		#region IExecutable
		public abstract string LaunchPath { get; }
		public abstract string[] Arguments { get; }
		public Platform Platform { get { return Product.Platform; } }
		public string DoEventMessage { get { return "StartDemo:{0}"; } }
		#endregion
	}
	public class Product {
		List<Demo> demos;
		public Product(Platform platform,
					   Func<Product, List<Demo>> createDemos,
					   string name,
					   string displayName,
					   string componentName,
					   string shortDescription,
					   string description,
					   Int64 licenseInfo,
					   Guid? id = null,
					   bool isAvailableOnline = true,
					   string group = "",
					   string color = "Auto",
					   string tag = null)
		{
			Platform = platform;
			demos = createDemos(this);
			Name = name;
			DisplayName = displayName;
			ComponentName = componentName;
			ShortDescription = shortDescription;
			Description = description;
			LicenseInfo = licenseInfo;
			Id = id;
			IsAvailableOnline = isAvailableOnline;
			Group = group;
			Color = color;
			Tag = tag;
		}
		public Platform Platform { get; private set; }
		public List<Demo> Demos { get { return demos; } }
		public string Name { get; private set; }
		public string DisplayName { get; private set; }
		public string ComponentName { get; private set; }
		public string ShortDescription { get; private set; }
		public string Description { get; private set; }
		public Int64 LicenseInfo { get; private set; }
		public Guid? Id { get; private set; }
		public bool IsAvailableOnline { get; private set; }
		public string Group { get; private set; }
		public DemoImage Icon { get { return new DemoImage(string.Format("{0}/{1}/Icon.png", Platform.Name, Name)); } }
		public DemoImage Image { get { return new DemoImage(string.Format("{0}/{1}/Image.png", Platform.Name, Name)); } }
		public string Color { get; private set; }
		public string Tag { get; private set; }
		public bool IsInstalled {
			get { return Linker.IsProductInstalled(ComponentName); }
		}
		public bool IsLicensed {
			get { return (LicenseHelper.GetLicensedProducts() & LicenseInfo) == LicenseInfo; }
		}
		public bool IsNew { get { return Demos.All(d => d.IsNew); } }
		public bool IsUpdated { get { return !IsNew && Demos.Any(d => d.IsNew || d.IsUpdated); } }
		public Product WithDisplayName(string displayName) {
			return new Product(Platform, _ => Demos, Name, displayName, ComponentName, ShortDescription,
							   Description, LicenseInfo, Id, IsAvailableOnline, Group, Color);
		}
	}
	public class ReallifeDemo : IExecutable {
		public ReallifeDemo(Platform platform,
							string name,
							string displayName,
							string launchPath,
							string csSolutionPath,
							string vbSolutionPath,
							bool showInDemoCenter,
							bool isAvailableInClickonce = true,
							int demoCenterPosition = 100,
							string group = null,
							Requirement[] requirements = null,
							string color = "Auto")
		{
			Name = name;
			DisplayName = displayName;
			LaunchPath = launchPath;
			CsSolutionPath = csSolutionPath;
			VbSolutionPath = vbSolutionPath;
			ShowInDemoCenter = showInDemoCenter;
			IsAvailableInClickonce = isAvailableInClickonce;
			DemoCenterPosition = demoCenterPosition;
			Group = group;
			Platform = platform;
			Color = color;
			Requirements = requirements ?? new Requirement[0];
		}
		public string Name { get; private set; }
		public string DisplayName { get; private set; }
		public string LaunchPath { get; private set; }
		public string CsSolutionPath { get; private set; }
		public string VbSolutionPath { get; private set; }
		public bool IsAvailableInClickonce { get; private set; }
		public bool ShowInDemoCenter { get; private set; }
		public int DemoCenterPosition { get; private set; }
		public string Group { get; private set; }
		public DemoImage MediumImage { get { return new DemoImage(string.Format("{0}/ReallifeDemos/{1}.Medium.png", Platform.Name, Name)); } }
		public DemoImage LargeImage { get { return new DemoImage(string.Format("{0}/ReallifeDemos/{1}.Large.png", Platform.Name, Name)); } }
		public Platform Platform { get; private set; }
		public Requirement[] Requirements { get; private set; }
		public string Color { get; private set; }
		public IEnumerable<OpenSolutionMenuItem> CreateOpenSolutionMenu(CallingSite site) {
			if(CsSolutionPath != null) {
				yield return new OpenSolutionMenuItem {
					Title = "Open CS Solution",
					OpenCommand = new DevExpress.DemoData.Helpers.ActionToCommandConverter.DelegateCommand(
						() => DemoRunner.OpenCSSolution(this, site))
				};
			}
			if(VbSolutionPath != null) {
				yield return new OpenSolutionMenuItem {
					Title = "Open VB Solution",
					OpenCommand = new DevExpress.DemoData.Helpers.ActionToCommandConverter.DelegateCommand(
						() => DemoRunner.OpenVBSolution(this, site))
				};
			}
		}
		#region IExecutable
		Requirement[] IExecutable.Requirements {
			get { return Requirements; }
		}
		string IExecutable.LaunchPath {
			get { return LaunchPath; }
		}
		string[] IExecutable.Arguments {
			get { return new string[0]; }
		}
		Platform IExecutable.Platform {
			get { return Platform; }
		}
		string IExecutable.DoEventMessage {
			get { return "StartDemo:{0}"; }
		}
		#endregion
	}
	public class Platform : IExecutable {
		Lazy<List<Product>> products;
		Lazy<List<ReallifeDemo>> reallifeDemos;
		string PatchVersion(string str) {
			return str.Replace("%Version%", AssemblyInfo.VersionShort);
		}
		public Platform(Func<Platform, List<Product>> createProducts,
						Func<Platform, List<ReallifeDemo>> createReallifeDemos,
						string name,
						string displayName,
						string productListTitle,
						string productListSubtitle,
						string getStartedLink,
						string demoLauncherPath,
						string demoLauncherArgument = null,
						Requirement[] requirements = null)
		{
			products = new Lazy<List<Product>>(() => createProducts(this));
			reallifeDemos = new Lazy<List<ReallifeDemo>>(() => createReallifeDemos(this));
			Name = name;
			DisplayName = displayName;
			ProductListTitle = productListTitle;
			ProductListSubtitle = productListSubtitle;
			DemoLauncherPath = PatchVersion(demoLauncherPath);
			DemoLauncherArgument = demoLauncherArgument;
			Requirements = requirements ?? new Requirement[0];
			GetStartedLink = getStartedLink;
		}
		public List<Product> Products { get { return products.Value; } }
		public List<ReallifeDemo> ReallifeDemos { get { return reallifeDemos.Value; } }
		public string Name { get; private set; }
		public string DisplayName { get; private set; }
		public string ProductListTitle { get; private set; }
		public string ProductListSubtitle { get; private set; }
		public string GetStartedLink { get; private set; }
		public string DemoLauncherPath { get; private set; }
		public string DemoLauncherArgument { get; private set; }
		public DemoImage Icon { get { return new DemoImage(string.Format("Platforms/{0}.png", Name)); } }
		public Requirement[] Requirements { get; private set; }
		public bool IsInstalled {
			get {
				bool withReports = this == Repository.ReportingPlatform;
				foreach(var product in Products) {
					if(Name == "Frameworks" && product.Name == "Xpo")
						continue;
					if(!withReports && (product.ComponentName != null && product.ComponentName.StartsWith("XtraReports")))
						continue; 
					if(product.IsInstalled)
						return true;
				}
				return false;
			}
		}
		public long LicenseInfo {
			get { return Products.Aggregate(0L, (l, r) => l | r.LicenseInfo); }
		}
		public bool IsLicensed {
			get { return (LicenseHelper.GetLicensedProducts() & LicenseInfo) == LicenseInfo; }
		}
		#region IExecutable
		Requirement[] IExecutable.Requirements {
			get { return Requirements; }
		}
		string IExecutable.LaunchPath {
			get { return DemoLauncherPath; }
		}
		string[] IExecutable.Arguments {
			get { return new[] { DemoLauncherArgument }; }
		}
		Platform IExecutable.Platform {
			get { return this; }
		}
		string IExecutable.DoEventMessage {
			get { return "StartDemoLauncher"; }
		}
		#endregion
	}
	static class LicenseHelper {
		public static long GetLicensedProducts() {
			var licensedProducts = long.MaxValue;
			return licensedProducts;
		}
	}
	public class DemoImage {
		static FlowDocument initializePackUri = new FlowDocument();
		Lazy<ImageSource> imageSource;
		Lazy<Image> image;
		Lazy<Uri> uri;
		internal DemoImage(string demoDataPath) {
			Debug.Assert(demoDataPath != null);
			string path = string.Format("pack://application:,,,/DevExpress.DemoData.v{0};component/Data/{1}",
				AssemblyInfo.VersionShort, demoDataPath);
			uri = new Lazy<Uri>(() => new Uri(path, UriKind.Absolute));
			imageSource = new Lazy<ImageSource>(() => new BitmapImage(Uri));
			image = new Lazy<Image>(() =>
			{
				try {
					string relative = path.Substring("pack://application:,,,/devexpress.demodata.vXX.X;component/".Length);
					using(var stream = AssemblyHelper.GetResourceStream(typeof(DemoImage).Assembly, relative, true)) {
						return (stream != null) ? Image.FromStream(stream) : null;
					}
				}
				catch { return null; }
			});
		}
		public Uri Uri { get { return uri.Value; } }
		public ImageSource ImageSource { get { return imageSource.Value; } }
		public Image Image { get { return image.Value; } }
	}
}
