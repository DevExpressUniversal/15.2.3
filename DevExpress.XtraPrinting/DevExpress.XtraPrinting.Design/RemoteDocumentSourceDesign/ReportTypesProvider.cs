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
using System.ComponentModel.Design;
using System.Linq;
using DevExpress.XtraReports;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign {
	public class ReportTypesProvider {
		static readonly Type ReportType = typeof(IReport);
		readonly ITypeDiscoveryService typeDiscoveryService;
		public ReportTypesProvider(ITypeDiscoveryService typeDiscoveryService) {
			this.typeDiscoveryService = typeDiscoveryService;
		}
		public IList<string> GetTypeNames() {
			var reportTypes = GetTypes(ReportType);
			if(!reportTypes.Any()) {
				reportTypes = GetXtraReportTypes();
			}
			if(!reportTypes.Any()) {
				reportTypes = ScanAllTypes();
			}
			var list = new List<string>(reportTypes.Select(x => x.FullName));
			list.Sort(StringComparer.InvariantCultureIgnoreCase);
			return list;
		}
		IEnumerable<Type> GetXtraReportTypes() {
			var xtraReportAssembly = AppDomain.CurrentDomain
				.GetAssemblies()
				.FirstOrDefault(x => x.GetName().Name == AssemblyInfo.SRAssemblyReports);
			if(xtraReportAssembly == null) {
				return Enumerable.Empty<Type>();
			}
			var xtraReportType = xtraReportAssembly.GetType("DevExpress.XtraReports.UI.XtraReport", false, false);
			return GetTypes(xtraReportType);
		}
		IEnumerable<Type> GetTypes(Type type) {
			return typeDiscoveryService
				.GetTypes(type, false)
				.Cast<Type>()
				.Where(ValidateReportType);
		}
		IEnumerable<Type> ScanAllTypes() {
			var result = GetTypes(typeof(object));
			result = GetTypes(typeof(object));
			result = GetTypes(typeof(object));
			return result;
		}
		static bool ValidateReportType(Type type) {
			return type.IsClass
				&& !type.IsAbstract
				&& type.GetInterfaces().Contains(ReportType)
				&& !type.Assembly.FullName.StartsWith("DevExpress.", StringComparison.Ordinal);
		}
	}
}
