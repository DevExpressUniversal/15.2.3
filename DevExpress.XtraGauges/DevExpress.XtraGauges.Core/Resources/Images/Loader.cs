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

using DevExpress.Compatibility.System.Drawing;
using System;
using System.Drawing;
using System.Reflection;
namespace DevExpress.XtraGauges.Core.Resources {
	static class PresetsLoader {
#if !DXPORTABLE
		public static readonly Assembly PresetsAssembly;
		static PresetsLoader() {
			AssemblyName presetsAssemblyName = new AssemblyName(AssemblyInfo.SRAssemblyGaugesPresets + AssemblyInfo.FullAssemblyVersionExtension);
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for(int i = 0; i < assemblies.Length; i++) {
				if(PresetsAssembly != null) break;
				AssemblyName name = assemblies[i].GetName();
				if(name.Equals(presetsAssemblyName)) PresetsAssembly = assemblies[i];
			}
			try {
				if(PresetsAssembly == null)
					PresetsAssembly = Assembly.Load(presetsAssemblyName);
			}
			catch { }
			EnsureStyleLoaderServices(PresetsAssembly);
			EnsureUIHelperService(PresetsAssembly);
		}
		public static Assembly GetPresetsAssembly() {
			return PresetsAssembly;
		}
#endif
		internal static void EnsureUIHelperService(Assembly assembly) {
			if(assembly == null) return;
			Type uiHelperLoader = assembly.GetType("DevExpress.XtraGauges.Presets.Resources.UIHelperLoader");
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(uiHelperLoader.TypeHandle);
		}
		internal static void EnsureStyleLoaderServices(Assembly assembly) {
			if(assembly == null) return;
			Type typeStyleLoader = assembly.GetType("DevExpress.XtraGauges.Presets.Styles.StyleLoader");
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeStyleLoader.TypeHandle);
		}
		static IStyleChooserService styleChooserCore;
		public static IStyleChooserService StyleChooser {
			get {
				if(styleChooserCore == null)
					styleChooserCore = Core.Styles.StyleCollectionHelper.GetService<IStyleChooserService>();
				return styleChooserCore;
			}
		}
		static IStyleManagerService styleManagerCore;
		public static IStyleManagerService StyleManager {
			get {
				if(styleManagerCore == null)
					styleManagerCore = Core.Styles.StyleCollectionHelper.GetService<IStyleManagerService>();
				return styleManagerCore;
			}
		}
		static IUIHelperService uiHelperServiceCore;
		public static IUIHelperService UIHelper {
			get {
				if(uiHelperServiceCore == null)
					uiHelperServiceCore = Core.Styles.StyleCollectionHelper.GetService<IUIHelperService>();
				return uiHelperServiceCore;
			}
		}
	}
}
namespace DevExpress.XtraGauges.Core {
	public interface IImageAccessor {
		Image this[int index] { get; }
	}
	public interface IUIHelperService {
		IImageAccessor UIButtonImages { get; }
		IImageAccessor UIActionImages { get; }
		IImageAccessor UIOtherImages { get; }
		IImageAccessor OverviewImages { get; }
		IImageAccessor GaugeTypeImages { get; }
		IImageAccessor CircularGaugeElementImages { get; }
		IImageAccessor LinearGaugeElementImages { get; }
		IImageAccessor DigitalGaugesMenu { get; }
		IImageAccessor ExpandCollapseImages { get; }
		Image ChangeStyleImage { get; }
	}
	public interface IStyleChooserService {
		bool Show(DevExpress.XtraGauges.Base.IGauge gauge);
	}
	public interface IStyleManagerService {
		bool Show(DevExpress.XtraGauges.Base.IGaugeContainer gaugeContainer);
	}
	public static class StyleChooser {
		public static bool Show(DevExpress.XtraGauges.Base.IGauge gauge) {
			return Resources.PresetsLoader.StyleChooser.Show(gauge);
		}
	}
	public static class StyleManager {
		public static bool Show(DevExpress.XtraGauges.Base.IGaugeContainer gaugeContainer) {
			return Resources.PresetsLoader.StyleManager.Show(gaugeContainer);
		}
	}
}
namespace DevExpress.XtraGauges.Core.Resources {
	public static class UIHelper {
		public static IImageAccessor UIButtonImages {
			get { return Resources.PresetsLoader.UIHelper.UIButtonImages; }
		}
		public static IImageAccessor UIActionImages {
			get { return Resources.PresetsLoader.UIHelper.UIActionImages; }
		}
		public static IImageAccessor UIOtherImages {
			get { return Resources.PresetsLoader.UIHelper.UIOtherImages; }
		}
		public static IImageAccessor GaugeTypeImages {
			get { return Resources.PresetsLoader.UIHelper.GaugeTypeImages; }
		}
		public static IImageAccessor CircularGaugeElementImages {
			get { return Resources.PresetsLoader.UIHelper.CircularGaugeElementImages; }
		}
		public static IImageAccessor LinearGaugeElementImages {
			get { return Resources.PresetsLoader.UIHelper.LinearGaugeElementImages; }
		}
		public static IImageAccessor DigitalGaugesMenu {
			get { return Resources.PresetsLoader.UIHelper.DigitalGaugesMenu; }
		}
		public static IImageAccessor OverviewImages {
			get { return Resources.PresetsLoader.UIHelper.OverviewImages; }
		}
		public static IImageAccessor ExpandCollapseImages {
			get { return Resources.PresetsLoader.UIHelper.ExpandCollapseImages; }
		}
		public static Image ChangeStyleImage {
			get { return Resources.PresetsLoader.UIHelper.ChangeStyleImage; }
		}
	}
}
