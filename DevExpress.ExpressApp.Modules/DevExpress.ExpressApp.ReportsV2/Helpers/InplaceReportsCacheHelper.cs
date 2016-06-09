#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class InplaceReportsCacheHelper {
		private bool isCompleteCache = false;
		private LightDictionary<Type, List<ReportDataInfo>> inplaceReportsCache = new LightDictionary<Type, List<ReportDataInfo>>();
		public virtual List<ReportDataInfo> GetReportDataInfoList(Type targetObjectType) {
			EnsureCache();
			List<ReportDataInfo> cachedReports = new List<ReportDataInfo>();
			foreach(Type key in inplaceReportsCache.Keys) {
				if(key.IsAssignableFrom(targetObjectType)) {
					cachedReports.AddRange(inplaceReportsCache[key]);
				}
			}
			return cachedReports;
		}
		public void ClearInplaceReportsCache() {
			isCompleteCache = false;
			inplaceReportsCache.Clear();
		}
		protected virtual IList<ReportDataInfo> CollectAllInplaceReportsData() {
			List<ReportDataInfo> result = new List<ReportDataInfo>();
			if(ReportDataProvider.ReportsStorage != null) {
				result.AddRange(ReportDataProvider.ReportsStorage.CollectInplaceReportsData());
			}
			return result;
		}
		private void EnsureCache() {
			if(!isCompleteCache) {
				IList<ReportDataInfo> allInPlaceReportsData = CollectAllInplaceReportsData();
				foreach(ReportDataInfo item in allInPlaceReportsData) {
					if(item.DataType != null) {
						List<ReportDataInfo> items;
						if(!inplaceReportsCache.TryGetValue(item.DataType, out items)) {
							items = new List<ReportDataInfo>();
							inplaceReportsCache[item.DataType] = items;
						}
						Guard.ArgumentNotNullOrEmpty(item.ReportContainerHandle, "handle");
						items.Add(item);
					}
				}
				isCompleteCache = true;
			}
		}
	}
}
