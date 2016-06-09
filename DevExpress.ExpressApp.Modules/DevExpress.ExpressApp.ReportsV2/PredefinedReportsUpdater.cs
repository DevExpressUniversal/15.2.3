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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Updating;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class PredefinedReportsUpdater : ModuleUpdater {
		private List<IReportDataV2> predefinedReports = new List<IReportDataV2>();
		private IEqualityComparer<IReportDataV2> reportDataComparer;
		private Type _reportDataType;
		private XafApplication application;
		public bool UseMultipleUpdaters { get; set; }
		public PredefinedReportsUpdater(XafApplication application, IObjectSpace objectSpace, Version currentDBVersion) :
			base(objectSpace, currentDBVersion) {
			this.application = application;
			reportDataComparer = new ReportDataEqualityComparer();
			UseMultipleUpdaters = false;
		}
		public override void UpdateDatabaseAfterUpdateSchema() {
			base.UpdateDatabaseAfterUpdateSchema();
			UpdatePredefinedReports(ObjectSpace);
		}
		public void AddPredefinedReport<T>(string displayName) where T : XtraReport {
			AddPredefinedReport<T>(displayName, null);
		}
		public void AddPredefinedReport<T>(string displayName, Type dataType) where T : XtraReport {
			AddPredefinedReport<T>(displayName, dataType, null);
		}
		public void AddPredefinedReport<T>(string displayName, Type dataType, Type parametersObjectType) where T : XtraReport {
			AddPredefinedReport<T>(displayName, dataType, parametersObjectType, false);
		}
		public void AddPredefinedReport<T>(string displayName, Type dataType, bool isInplaceReport) where T : XtraReport {
			AddPredefinedReport<T>(displayName, dataType, null, isInplaceReport);
		}
		public void AddPredefinedReport<T>(string displayName, Type dataType, Type parametersObjectType, bool isInplaceReport) where T : XtraReport {
			predefinedReports.Add(new PredefinedReportDataContainer(typeof(T), displayName, dataType, parametersObjectType, isInplaceReport));
		}
		public void AddPredefinedReport(IReportDataV2 reportData) {
			predefinedReports.Add(reportData);
		}
		public IList<IReportDataV2> PredefinedReports {
			get {
				return predefinedReports;
			}
		}
		public IEqualityComparer<IReportDataV2> ReportDataComparer {
			get {
				return reportDataComparer;
			}
			set {
				reportDataComparer = value;
			}
		}
		private Type ReportDataType {
			get {
				if(_reportDataType == null) {
					ReportsModuleV2 reportModule = ReportsModuleV2.FindReportsModule(application.Modules);
					_reportDataType = reportModule.ReportDataType;
				}
				return _reportDataType;
			}
		}
		protected virtual string FindPredefinedReportTypeMemberName() {
			return ReportsModuleV2.FindPredefinedReportTypeMemberName(ReportDataType);
		}
		private void UpdatePredefinedReports(IObjectSpace objectSpace) {
			Type internalReportDataType = ReportDataType;
			if(internalReportDataType == null) {
				throw new ArgumentOutOfRangeException(typeof(ReportsModuleV2).Name + ".ReportDataType", "Cannot update predefined reports because ReportDataType is null.");
			}
			if(objectSpace.CanInstantiate(internalReportDataType)) {
				CriteriaOperator filter = new NotOperator(new NullOperator(FindPredefinedReportTypeMemberName()));
				IList allPredefinedReports = objectSpace.CreateCollection(internalReportDataType, filter);
				IEnumerable<IReportDataV2> storedPredefinedList = allPredefinedReports.Cast<IReportDataV2>();
				List<IReportDataV2> removeReportData = storedPredefinedList.Except(PredefinedReports, ReportDataComparer).ToList();
				IEnumerable<IReportDataV2> newReportData = PredefinedReports.Except(storedPredefinedList, ReportDataComparer);
				if(UseMultipleUpdaters) {
					RemoveReportsFromCurrentAssemblyOnly(objectSpace, removeReportData);
				}
				else {
					objectSpace.Delete(removeReportData);
				}
				foreach(PredefinedReportDataContainer predefinedReportData in newReportData) {
					IReportDataV2Writable reportData = (IReportDataV2Writable)objectSpace.CreateObject(internalReportDataType);
					ReportDataProvider.ReportsStorage.CopyFrom(predefinedReportData, reportData);
				}
			}
			objectSpace.CommitChanges();
		}
		private void RemoveReportsFromCurrentAssemblyOnly(IObjectSpace objectSpace, List<IReportDataV2> removeReportData) {
			foreach(IReportDataV2 predefinedReportData in removeReportData) {
				if(predefinedReportData.PredefinedReportType != null && ReportInCurrentAssembly(predefinedReportData.PredefinedReportType.FullName)) {
					objectSpace.Delete(predefinedReportData);
				}
			}
		}
		private bool ReportInCurrentAssembly(string reportTypeName) {
			Type reportType = System.Reflection.Assembly.GetExecutingAssembly().GetType(reportTypeName, false);
			return reportType != null;
		}
		private class ReportDataEqualityComparer : IEqualityComparer<IReportDataV2> {
			public bool Equals(IReportDataV2 reportData1, IReportDataV2 reportData2) {
				if(Object.ReferenceEquals(reportData1, null)) return false;
				if(Object.ReferenceEquals(reportData2, null)) return false;
				if(Object.ReferenceEquals(reportData1, reportData2)) return true;
				if(!reportData1.IsPredefined) return false;
				if(!reportData2.IsPredefined) return false;
				if(!Type.Equals(reportData1.PredefinedReportType, reportData2.PredefinedReportType)) return false;
				if(!Type.Equals(reportData1.ParametersObjectType, reportData2.ParametersObjectType)) return false;
				if(!string.Equals(reportData1.DisplayName, reportData2.DisplayName)) return false;
				if(!Type.Equals(reportData1.DataType, reportData2.DataType)) return false;
				if(reportData1 is IInplaceReportV2 && reportData2 is IInplaceReportV2) {
					if(((IInplaceReportV2)reportData1).IsInplaceReport != ((IInplaceReportV2)reportData2).IsInplaceReport) return false;
				}
				else if((reportData1 is IInplaceReportV2 && !(reportData2 is IInplaceReportV2)) ||
					(!(reportData1 is IInplaceReportV2) && reportData2 is IInplaceReportV2)) return false;
				return true;
			}
			public int GetHashCode(IReportDataV2 reportData) {
				int hashIsPredefined = reportData.IsPredefined.GetHashCode();
				int hashPredefinedReportType = reportData.PredefinedReportType == null ? 0 : reportData.PredefinedReportType.GetHashCode();
				int hashParametersObjectType = reportData.ParametersObjectType == null ? 0 : reportData.ParametersObjectType.GetHashCode();
				int hashDisplayName = reportData.DisplayName == null ? 0 : reportData.DisplayName.GetHashCode();
				int hashDataType = reportData.DataType == null ? 0 : reportData.DataType.GetHashCode();
				int hashIsInplaceReport = 0;
				if(reportData is IInplaceReportV2) {
					hashIsInplaceReport = ((IInplaceReportV2)reportData).IsInplaceReport.GetHashCode();
				}
				return hashIsPredefined ^ hashPredefinedReportType ^ hashDisplayName ^ hashDataType ^ hashIsInplaceReport ^ hashParametersObjectType;
			}
		}
	}
}
