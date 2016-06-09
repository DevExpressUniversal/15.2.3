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

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core;
using DevExpress.XtraGauges.Core.Presets;
using DevExpress.XtraGauges.Core.Styles;
namespace DevExpress.XtraGauges.Presets.Styles {
	class StyleResourceProvider : IStyleResourceProvider {
		Assembly IStyleResourceProvider.GetAssembly() {
			return typeof(StyleResourceProvider).Assembly;
		}
		string IStyleResourceProvider.GetPathPrefix() {
			return "DevExpress.XtraGauges.Presets.Resources.";
		}
		string IStyleResourceProvider.GetPathSuffix() {
			return ".preset";
		}
	}
	public static class StyleLoader {
		public static IDictionary<StyleCollectionKey, string> Resources = new Dictionary<StyleCollectionKey, string>();
		static StyleLoader() {
			RegisterServices();
			RegisterStyleChooserService();
			RegisterStyleManagerServiceService();
			StyleResourcesHelper.BuildResourcesMap(Resources, Core.Styles.StyleCollectionHelper.ServiceProvider);
		}
		public static void RegisterStyleChooserService() {
			Core.Styles.StyleCollectionHelper.Register<IStyleChooserService, StyleChooserServiceProvider>();
		}
		public static void RegisterStyleManagerServiceService() {
			Core.Styles.StyleCollectionHelper.Register<IStyleManagerService, StyleManagerServiceProvider>();
		}
		public static void RegisterServices() {
			Core.Styles.StyleCollectionHelper.Register<IStyleResourceProvider, StyleResourceProvider>();
			Core.Styles.StyleCollectionHelper.Register<IThemeNameResolutionService, ThemeNameResolutionService>();
		}
		public static StyleCollection Load(string path) {
			return StyleCollectionHelper.Load(path, StyleCollectionHelper.ServiceProvider);
		}
		public static StyleCollection Load(StyleCollectionKey key) {
			return StyleCollectionHelper.Load(key, StyleCollectionHelper.ServiceProvider);
		}
		public static StyleCollection Load(string scope, string name, string tag) {
			return StyleCollectionHelper.Load(scope, name, tag, StyleCollectionHelper.ServiceProvider);
		}
		internal static StyleCollectionKey ExtractKey(string path) {
			return StyleCollectionKey.ExtractKey(path, StyleCollectionHelper.ServiceProvider);
		}
		public static System.Drawing.Image Preview(StyleCollectionKey key) {
			if(key == null) return null;
			System.Drawing.Image result = null;
			string path;
			if(StyleLoader.Resources.TryGetValue(key, out path)) {
				using(GaugePreset preset = new GaugePreset()) {
					var resourceProvider = Core.Styles.StyleCollectionHelper.GetService<IStyleResourceProvider>();
					var assembly = resourceProvider.GetAssembly();
					using(Stream stream = assembly.GetManifestResourceStream(path)) {
						preset.LoadFromStream(stream);
						IGaugeContainer container = ControlLoader.CreateGaugeContainer();
						using(container as System.IDisposable) {
							using(MemoryStream ms = new MemoryStream(preset.LayoutInfo)) {
								new BinaryXtraSerializer().DeserializeObject(container, ms, "IGaugeContainer");
							}
							return container.GetImage(container.Bounds.Width, container.Bounds.Height);
						}
					}
				}
			}
			return result;
		}
	}
}
