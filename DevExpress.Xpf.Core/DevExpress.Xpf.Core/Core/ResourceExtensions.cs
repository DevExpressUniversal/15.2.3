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
using System.Windows.Markup;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Reflection;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core {
	[Obsolete("for testing purposes only DO not use in production code!")]
	public class TestThemeResourceExtension : MarkupExtension {
		public string ResourcePath { get; set; }
		public bool DenyXBAP { get; set; }
		protected string Namespace { get { return "TestTheme"; } }
		public TestThemeResourceExtension(string resourcePath) {
			ResourcePath = resourcePath;
		}
		public sealed override object ProvideValue(IServiceProvider serviceProvider) {
			return new Uri(string.Format("/{0};component/{1}", Namespace, DenyXBAP && BrowserInteropHelper.IsBrowserHosted ? "Themes/Fake.xaml" : ResourcePath), UriKind.RelativeOrAbsolute);
		}
	}
	public abstract class ThemeResourceExtensionBase : MarkupExtension {
		public string ResourcePath { get; set; }
		public bool DenyXBAP { get; set; }
		protected abstract string Namespace { get; }
		protected ThemeResourceExtensionBase(string resourcePath) {
			ResourcePath = resourcePath;
		}
		public sealed override object ProvideValue(IServiceProvider serviceProvider) {
			var resourcePath = DenyXBAP && BrowserInteropHelper.IsBrowserHosted ? "Themes/Fake.xaml" : GetResourcePath(serviceProvider);
			return new Uri(string.Format("/{0}{1};component/{2}", Namespace, AssemblyInfo.VSuffix, resourcePath), UriKind.RelativeOrAbsolute);
		}
		protected abstract string GetResourcePath(IServiceProvider serviceProvider);
	}
	public abstract class ResourceExtensionBase : ThemeResourceExtensionBase {
		protected ResourceExtensionBase(string resourcePath) : base(resourcePath) { }
		protected sealed override string GetResourcePath(IServiceProvider serviceProvider) {
			return GetResourcePath();
		}
		protected virtual string GetResourcePath() {
			return ResourcePath;
		}
	}
	public class DynamicThemeResourceExtension : MarkupExtension {
		public string ResourceName { get; set; }
		public string ResourcePath { get; set; }
		public string ResourcePathInTheme { get; set; }
		public Type TypeInTargetAssembly { get; set; }
		public DynamicThemeResourceExtension(string resourceName) {
			ResourceName = resourceName;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			var themeName = DevExpress.Xpf.Utils.Themes.ThemeNameHelper.GetThemeName(serviceProvider);
			if (string.IsNullOrEmpty(themeName))
				return new Uri(string.Format("/{0};component/{1}/{2}/{3}", TypeInTargetAssembly.Assembly.GetName().Name, ResourcePath, Theme.DeepBlue.Name, ResourceName).Replace("//", "/"), UriKind.RelativeOrAbsolute);
			return new Uri(string.Format("/DevExpress.Xpf.Themes.{0}{1};component/{2}/{3}/{4}",themeName, AssemblyInfo.VSuffix, ResourcePathInTheme, themeName, ResourceName).Replace("//", "/"), UriKind.RelativeOrAbsolute);
		}
	}
#if !SL
	public class DynamicEditorsResourceExtension : ThemeResourceExtensionBase {
		const string coreAssemblyName = "DevExpress.Xpf.Core";
		const string editorsInThemePath = coreAssemblyName + "/" + coreAssemblyName + "/";
		const string defaultThemeName = Theme.DeepBlueName;
		const string themesPrefix = "DevExpress.Xpf.Themes";
		string themeName = null;
		protected sealed override string Namespace {
			get { return themeName != null ? String.Format("{0}.{1}", themesPrefix, themeName) : coreAssemblyName; }
		}
		public DynamicEditorsResourceExtension(string resourcePath) : base(resourcePath) { }
		protected override string GetResourcePath(IServiceProvider serviceProvider) {
			themeName = DevExpress.Xpf.Utils.Themes.ThemeNameHelper.GetThemeName(serviceProvider);
			if (string.IsNullOrEmpty(themeName))
				themeName = null;
			return string.Format("{0}Editors/Themes/{1}/{2}", themeName == null ? "" : editorsInThemePath, themeName ?? defaultThemeName, ResourcePath);
		}
	}
#endif
	public class GridResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.GridNamespace; } }
		public GridResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class PropertyGridResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return "DevExpress.Xpf.PropertyGrid"; } }
		public PropertyGridResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class PdfViewerResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return "DevExpress.Xpf.PdfViewer"; } }
		public PdfViewerResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class SpreadsheetResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return "DevExpress.Xpf.Spreadsheet"; } }
		public SpreadsheetResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class PivotGridResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.PivotGridNamespace; } }
		public PivotGridResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class BarsResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.UtilsNamespace; } }
		public BarsResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
		protected override string GetResourcePath() {
			return "Bars/" + ResourcePath;
		}
	}
	public class RibbonResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.RibbonNamespace; } }
		public RibbonResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
		protected override string GetResourcePath() {
			return ResourcePath;
		}
	}
	public class DockingResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.DockingNamespace; } }
		public DockingResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class ChartsResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.ChartsNamespace; } }
		public ChartsResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class ChartDesignerResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.ChartDesignerNamespace; } }
		public ChartDesignerResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class GaugesResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.GaugesNamespace; } }
		public GaugesResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class MapResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.MapNamespace; } }
		public MapResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class UtilsResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return "DevExpress.Xpf.Core"; } }
		public UtilsResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class DemoBaseResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return "DevExpress.Xpf.DemoBase"; } }
		public DemoBaseResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class EditorsResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return "DevExpress.Xpf.Core"; } }
		public EditorsResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
		protected override string GetResourcePath() {
			return "Editors/" + ResourcePath;
		}
	}
	public class OfficeResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return "DevExpress.Xpf.Core"; } }
		public OfficeResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
		protected override string GetResourcePath() {
			return "Office/" + ResourcePath;
		}
	}
	public class CarouselResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return "DevExpress.Xpf.Carousel"; } }
		public CarouselResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class NavBarResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return "DevExpress.Xpf.NavBar"; } }
		public NavBarResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class SchedulerResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.SchedulerNamespace; } }
		public SchedulerResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class DXTabControlResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.UtilsNamespace; } }
		protected override string GetResourcePath() { return "TabControl/" + ResourcePath; }
		public DXTabControlResourceExtension(string resourcePath) : base(resourcePath) { }
	}
	public class RangeControlResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.UtilsNamespace; } }
		protected override string GetResourcePath() { return "RangeControl/" + ResourcePath; }
		public RangeControlResourceExtension(string resourcePath) : base(resourcePath) { }
	}
	public class PrintingResourceExtension : ResourceExtensionBase {
		protected override string Namespace {
			get { return XmlNamespaceConstants.PrintingNamespace; }
		}
		public PrintingResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class ReportDesignerResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return "DevExpress.Xpf.ReportDesigner"; } }
		public ReportDesignerResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
	public class DiagramResourceExtension : ResourceExtensionBase {
		protected override string Namespace { get { return XmlNamespaceConstants.DiagramCoreNamespace; } }
		public DiagramResourceExtension(string resourcePath)
			: base(resourcePath) {
		}
	}
# if!WPFMAP
	public abstract class RelativeImageSourceExtensionBase : MarkupExtension {
		public string RelativePath { get; set; }
		protected abstract string Namespace { get; }
		public bool DenyXBAP { get; set; }
		protected RelativeImageSourceExtensionBase(string relativePath) {
			RelativePath = relativePath;
		}
		protected virtual Uri GetUri(IServiceProvider serviceProvider) {
			return UriHelper.GetUri(Namespace, DenyXBAP && BrowserInteropHelper.IsBrowserHosted ? "Themes/Fake.xaml" : RelativePath);
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new BitmapImage(GetUri(serviceProvider));
		}
	}
	public abstract class RelativeIconSourceExtensionBase : RelativeImageSourceExtensionBase {
		protected RelativeIconSourceExtensionBase(string relativePath) : base(relativePath) { }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return BitmapFrame.Create(GetUri(serviceProvider));
		}
	}
	public class DemoBaseImageSourceExtension : RelativeImageSourceExtensionBase {
		public DemoBaseImageSourceExtension(string relativePath) : base(relativePath) { }
		protected override string Namespace { get { return "DevExpress.Xpf.DemoBase"; } }
	}
	public class UtilsIconSourceExtension : RelativeIconSourceExtensionBase {
		public UtilsIconSourceExtension(string relativePath) : base(relativePath) { }
		protected override string Namespace {
			get { return "DevExpress.Xpf.Core"; }
		}
	}
#endif
}
