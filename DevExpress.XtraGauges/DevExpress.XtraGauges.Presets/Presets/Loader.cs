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

using System.IO;
using DevExpress.XtraGauges.Core.Presets;
namespace DevExpress.XtraGauges.Presets.PresetManager {
	public sealed class PresetLoader {
		public static string[] Resources;
		static PresetLoader() {
			DevExpress.XtraGauges.Presets.Styles.StyleLoader.RegisterServices();
			Resources = Core.Styles.StyleResourcesHelper.GetPresetNames(Core.Styles.StyleCollectionHelper.ServiceProvider);
		}
		public static GaugePreset LoadFromFile(string path) {
			GaugePreset preset = new GaugePreset();
			using(Stream stream = new FileStream(path, FileMode.Open)) {
				if(stream != null) {
					preset.LoadFromStream(stream);
					stream.Close();
				}
			}
			return preset;
		}
		public static GaugePreset LoadFromResources(string resourcePath) {
			GaugePreset preset = new GaugePreset();
			var resourcesProvider = Core.Styles.StyleCollectionHelper.GetService<Core.Styles.IStyleResourceProvider>();
			var assembly = resourcesProvider.GetAssembly();
			using(Stream resourceStream = assembly.GetManifestResourceStream(resourcePath)) {
				if(resourceStream != null) {
					preset.LoadFromStream(resourceStream);
					resourceStream.Close();
				}
			}
			return preset;
		}
	}
}
