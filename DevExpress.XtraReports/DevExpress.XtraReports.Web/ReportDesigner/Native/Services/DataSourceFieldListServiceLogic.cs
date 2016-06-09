#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public static class DataSourceFieldListServiceLogic {
		public static string GetDataSourceDisplayName(object dataSource) {
			string displayName = null;
			DoDataContextAction(x => x.GetDataSourceDisplayName(dataSource, "", (_, e) => displayName = e.DataSourceDisplayName));
			if(string.IsNullOrEmpty(displayName)) {
				displayName = GetDataSourceDisplayNameFromType(dataSource.GetType());
			}
			return displayName;
		}
		public static void DoDataContextAction(Action<IPropertiesProvider> action) {
			var dataContextService = new RemoteReportDesignerDataContextService();
			var dataContextOptions = new DataContextOptions(true, false);
			using(dataContextService)
			using(var provider = new DataSortedPropertiesProvider(dataContextService.CreateDataContext(dataContextOptions), dataContextService)) {
				action(provider);
			}
		}
		static string GetDataSourceDisplayNameFromType(Type type) {
			var typeName = type.Name;
			var chars = typeName.ToCharArray();
			chars[0] = char.ToLowerInvariant(chars[0]);
			if(typeName.StartsWith("XR", StringComparison.InvariantCulture) || typeName.StartsWith("XP", StringComparison.InvariantCulture)) {
				chars[1] = char.ToLowerInvariant(chars[1]);
			}
			return new string(chars);
		}
	}
}
