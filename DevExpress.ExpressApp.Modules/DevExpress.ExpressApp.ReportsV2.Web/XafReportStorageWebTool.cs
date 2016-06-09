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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.XtraReports.Web.Extensions;
namespace DevExpress.ExpressApp.ReportsV2.Web {
	public class XafReportStorageWebTool : IReportStorageWebTool {
		private static XafReportStorageWebTool instance;
		private XafReportStorageWebTool() { }
		public static XafReportStorageWebTool Instance {
			get {
				if(instance == null) {
					instance = new XafReportStorageWebTool();
				}
				return instance;
			}
		}
		public INewReportWizardParameters NewReportParameters {
			get {
				return ValueManager.GetValueManager<INewReportWizardParameters>("ReportTag").Value;
			}
			set {
				ValueManager.GetValueManager<INewReportWizardParameters>("ReportTag").Value = value;
			}
		}
		public Dictionary<string, string> GetUrls() {
			Dictionary<string, string> urls = new Dictionary<string, string>();
			Type reportDataType = FindReportDataType();
			using(IObjectSpace objectSpace = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(reportDataType)) {
				CriteriaOperator criteriaNotPredefined = new NullOperator(ReportsModuleV2.FindPredefinedReportTypeMemberName(reportDataType));
				SortProperty sortbyName = new SortProperty("DisplayName", Xpo.DB.SortingDirection.Ascending);
				IList allReports = objectSpace.CreateCollection(reportDataType, criteriaNotPredefined, new SortProperty[] { sortbyName });
				foreach(object reportObject in allReports) {
					IReportDataV2 reportData = reportObject as IReportDataV2;
					urls.Add(ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData), reportData.DisplayName);
				}
			}
			return urls;
		}
		public string SetNewData(XtraReports.UI.XtraReport report, string defaultUrl) {
			string url = defaultUrl;
			if(!ReportDataProvider.ReportsStorage.IsValidUrl(url)) {
				url = ReportDataProvider.ReportsStorage.CreateNewReportHandle(FindReportDataType());
				report.Tag = NewReportParameters;
			}
			return ReportDataProvider.ReportsStorage.SetNewData(report, url);
		}
		private Type FindReportDataType() {
			Guard.ArgumentNotNull(ApplicationReportObjectSpaceProvider.ContextApplication, "ContextApplication");
			return ReportsModuleV2.FindReportsModule(ApplicationReportObjectSpaceProvider.ContextApplication.Modules).ReportDataType;
		}
	}
}
