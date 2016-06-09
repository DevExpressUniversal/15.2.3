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
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base.ReportsV2;
namespace DevExpress.ExpressApp.ReportsV2 {
	public sealed class ReportDataProvider {
		private static ReportsStorage reportsStorageExtension;
		private static IObjectSpaceCreator reportObjectSpaceProvider;
		static ReportDataProvider() {
			ReportsStorage = new ReportsStorage();
			if(ReportObjectSpaceProvider == null) {
				ReportObjectSpaceProvider = ApplicationReportObjectSpaceProvider.Instance;
			}
		}
		public static ReportsStorage ReportsStorage {
			get { return reportsStorageExtension; }
			set {
				reportsStorageExtension = value;
				ReportStorageExtension.RegisterExtensionGlobal(reportsStorageExtension);
			}
		}
		public static IObjectSpaceCreator ReportObjectSpaceProvider {
			get {
				return reportObjectSpaceProvider;
			}
			set {
				reportObjectSpaceProvider = value;
			}
		}
		public static void MassUpdateDataType<T>(IObjectSpace objectSpace, string oldDataType, Type newDataType) where T : IReportDataV2Writable {
			MassUpdateDataType<T>(objectSpace, oldDataType, newDataType, "dataTypeName");
		}
		public static void MassUpdateDataType<T>(IObjectSpace objectSpace, string oldDataType, Type newDataType, string dataTypeMemberName) where T : IReportDataV2Writable {
			GroupOperator filter = new GroupOperator(GroupOperatorType.And, new NullOperator(ReportsModuleV2.FindPredefinedReportTypeMemberName(typeof(T))), new BinaryOperator(dataTypeMemberName, oldDataType));
			foreach(T reportData in objectSpace.GetObjects<T>(filter)) {
				XtraReport report = ReportsStorage.LoadReport(reportData);
				DataSourceBase dataSource = report.DataSource as DataSourceBase;
				if(dataSource != null) {
					dataSource.ObjectTypeName = newDataType.FullName;
				}
				ReportsStorage.SaveReport(reportData, report);
			}
			objectSpace.CommitChanges();
		}
	}
}
