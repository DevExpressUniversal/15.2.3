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
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.Xpo;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class ReportDataSourceHelper {
		public const string XafReportParametersObjectName = "XafReportParametersObject";
		private XafApplication application;
		private Dictionary<int, IReportObjectSpaceProvider> objectSpaceProvidersCache = new Dictionary<int, IReportObjectSpaceProvider>();
		public ReportDataSourceHelper(XafApplication application)
			: this() {
			this.application = application;
		}
		private ReportDataSourceHelper() { }
		public void SetupBeforePrint(XtraReport report) {
			SetupBeforePrint(report, null, null, false, null, false);
		}
		public void SetupBeforePrint(XtraReport report, ReportParametersObjectBase parametersObject, CriteriaOperator criteria, bool canApplyCriteria, SortProperty[] sortProperty, bool canApplySortProperty) {
			SetupReport(report, parametersObject, criteria, canApplyCriteria, sortProperty, canApplySortProperty);
			ReportParametersDataSourceInitializer.SetupMultiValueParametersDataSources(report);
			OnBeforeShowPreview(report);
		}
		public void SetupReport(XtraReport report) {
			SetupReport(report, null, null, false, null, false);
		}
		public void SetupReport(XtraReport report, ReportParametersObjectBase parametersObject, CriteriaOperator criteria, bool canApplyCriteria, SortProperty[] sortProperty, bool canApplySortProperty) {
			SetupReportDataSource(report, criteria, canApplyCriteria, sortProperty, canApplySortProperty);
			SetXafReportParametersObject(report, parametersObject);
			RegisterObjectSpaceProviderService(report);
			RegisterReportEnumLocalizer(report);
			AttachCriteriaWithReportParametersManager(report);
		}
		public void AttachCriteriaWithReportParametersManager(XtraReport report) {
			report.BeforePrint += Report_BeforePrint;
		}
		public virtual object GetMasterReportDataSource(XtraReport report) {
			object result = null;
			if(report.DataSource != null) {
				result = report.DataSource;
			}
			return result;
		}
		public IReportObjectSpaceProvider CreateReportObjectSpaceProviderCore(Component component) {
			DisposeReportObjectSpaceProvider(component);
			IReportObjectSpaceProvider objectSpaceProvider = CreateReportObjectSpaceProvider();
			objectSpaceProvidersCache.Add(component.GetHashCode(), objectSpaceProvider);
			component.Disposed += new EventHandler(Report_Disposed);
			return objectSpaceProvider;
		}
		protected virtual void OnCustomSetupReportDataSource(CustomSetupReportDataSourceEventArgs args) {
			if(CustomSetupReportDataSource != null) {
				CustomSetupReportDataSource(this, args);
			}
		}
		public virtual void SetXafReportParametersObject(XtraReport report, ReportParametersObjectBase parametersObject) {
			if(parametersObject != null) {
				XtraReports.Parameters.Parameter xafReportParameter = new XtraReports.Parameters.Parameter();
				xafReportParameter.Name = XafReportParametersObjectName;
				xafReportParameter.Value = parametersObject;
				xafReportParameter.Type = typeof(ReportParametersObjectBase);
				xafReportParameter.Visible = false;
				report.Parameters.Add(xafReportParameter);
			}
		}
		protected void SetCriteria(CriteriaOperator criteria, object dataSource) {
			CustomSetCriteriaEventArgs args = new CustomSetCriteriaEventArgs(criteria, dataSource);
			OnCustomSetCriteria(args);
			if(!args.Handled && dataSource is ISupportCriteria) {
				((ISupportCriteria)dataSource).Criteria = criteria;
			}
		}
		protected void SetSorting(SortProperty[] sortProperty, object dataSource) {
			CustomSetSortingEventArgs args = new CustomSetSortingEventArgs(sortProperty, dataSource);
			OnCustomSetSorting(args);
			if(!args.Handled && dataSource is ISupportSorting) {
				((ISupportSorting)dataSource).Sorting.Clear();
				if(sortProperty != null) {
					((ISupportSorting)dataSource).Sorting.AddRange(sortProperty);
				}
			}
		}
		protected virtual void OnCustomSetCriteria(CustomSetCriteriaEventArgs args) {
			if(CustomSetCriteria != null) {
				CustomSetCriteria(this, args);
			}
		}
		protected virtual void OnCustomSetSorting(CustomSetSortingEventArgs args) {
			if(CustomSetSorting != null) {
				CustomSetSorting(this, args);
			}
		}
		protected virtual ReportEnumLocalizer CreateReportEnumLocalizer() {
			return new ReportEnumLocalizer();
		}
		protected virtual IReportObjectSpaceProvider CreateReportObjectSpaceProvider() {
			return new ReportObjectSpaceProvider(application);
		}
		protected virtual void OnBeforeShowPreview(XtraReport report) {
			if(BeforeShowPreview != null) {
				BeforeShowPreviewEventArgs args = new BeforeShowPreviewEventArgs(report);
				BeforeShowPreview(this, args);
			}
		}
		public void SetupReportDataSource(XtraReport report, CriteriaOperator criteria, bool canApplyCriteria, SortProperty[] sortProperty, bool canApplySortProperty) {
			CustomSetupReportDataSourceEventArgs args = new CustomSetupReportDataSourceEventArgs(report, criteria, canApplyCriteria, sortProperty, canApplySortProperty);
			OnCustomSetupReportDataSource(args);
			if(!args.Handled) {
				if(canApplyCriteria || canApplySortProperty) {
					object dataSource = GetMasterReportDataSource(report);
					if(dataSource != null) {
						if(canApplyCriteria) {
							SetCriteria(criteria, dataSource);
						}
						if(canApplySortProperty) {
							SetSorting(sortProperty, dataSource);
						}
					}
				}
			}
		}
		public void RegisterObjectSpaceProviderService(XtraReport report) {
			IServiceContainer serviceContainer = report.PrintingSystem;
			serviceContainer.RemoveService(typeof(IReportObjectSpaceProvider));
			IReportObjectSpaceProvider objectSpaceProvider = CreateReportObjectSpaceProviderCore(report);
			serviceContainer.AddService(typeof(IReportObjectSpaceProvider), objectSpaceProvider);
		}
		public void RegisterReportEnumLocalizer(XtraReport report) {
			ReportEnumLocalizer enumLocalizer = CreateReportEnumLocalizer();
			enumLocalizer.Attach(report);
		}
		private void ClearObjectSpaceProvidersCache() {
			foreach(KeyValuePair<int, IReportObjectSpaceProvider> item in objectSpaceProvidersCache) {
				item.Value.DisposeObjectSpaces();
			}
			objectSpaceProvidersCache.Clear();
		}
		private void DisposeReportObjectSpaceProvider(Component report) {
			IReportObjectSpaceProvider objectSpaceProvider;
			objectSpaceProvidersCache.TryGetValue(report.GetHashCode(), out objectSpaceProvider);
			if(objectSpaceProvider != null) {
				objectSpaceProvidersCache.Remove(report.GetHashCode());
				objectSpaceProvider.DisposeObjectSpaces();
			}
		}
		private void Report_Disposed(object sender, EventArgs e) {
			Component report = (Component)sender;
			report.Disposed -= new EventHandler(Report_Disposed);
			DisposeReportObjectSpaceProvider(report);
		}
		private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e) {
			UpdateReportDataSourceCriteria((XtraReport)sender);
		}
		private void UpdateReportDataSourceCriteria(XtraReport report) {
			if(report.Parameters.Count > 0) {
				if(report.DataSource is DataSourceBase) {
					((DataSourceBase)report.DataSource).UpdateCriteriaWithReportParameters(report);
				}
			}
		}
		public event EventHandler<CustomSetCriteriaEventArgs> CustomSetCriteria;
		public event EventHandler<CustomSetSortingEventArgs> CustomSetSorting;
		public event EventHandler<CustomSetupReportDataSourceEventArgs> CustomSetupReportDataSource;
		public event EventHandler<BeforeShowPreviewEventArgs> BeforeShowPreview;
	}
}
