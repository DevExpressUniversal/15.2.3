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
using System.ComponentModel.Composition.Hosting;
using System.Linq;
namespace DevExpress.XtraReports.Service.Native.Services {
	public class ConfiguredMefExtensionsResolver : MefExtensionsResolver {
		static ExportProvider CreateExportProvider(IConfigurationSettingsProvider configurationSettingsProvider) {
			var section = configurationSettingsProvider.ConfigurationSettings.GetPrintingServiceSection();
			var includeAssemblies = section != null
				? section.IncludeAssemblies.AssemblyNames
				: Enumerable.Empty<string>();
			var customLoadedAssemblyCatalogs = includeAssemblies.Select(n => new AssemblyCatalog(n));
			var excludedAssemblyPrefixes = Helper.ExcludedAssemblyPrefixes.Union(new[] { "DevExpress." });
			var assemblyCatalogs = AppDomain.CurrentDomain.GetAssemblies()
				.Where(a => !excludedAssemblyPrefixes.Any(p => a.FullName.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
				.Select(a => new AssemblyCatalog(a))
				.Union(customLoadedAssemblyCatalogs);
			var aggregateCatalog = new AggregateCatalog(assemblyCatalogs);
			return new CompositionContainer(aggregateCatalog, true);
		}
		public ConfiguredMefExtensionsResolver(IConfigurationSettingsProvider configurationSettingsProvider)
			: base(CreateExportProvider(configurationSettingsProvider)) {
		}
	}
}
