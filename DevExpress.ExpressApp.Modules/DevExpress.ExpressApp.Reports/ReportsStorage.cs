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
using System.IO;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.ExpressApp.Reports {
	public class XafReportTypeService : ReportTypeService {
		public XtraReport GetDefaultReport() {
			XafReport createdReport = new XafReport();
			createdReport.ReportName = createdReport.Name;
			return createdReport;
		}
		public Type GetType(Type reportType) {
			if(typeof(XafReport).IsAssignableFrom(reportType)) {
				return reportType;
			}
			throw new ArgumentOutOfRangeException("reportType");
		}
	}
	public class XafReportStorageExtension : ReportStorageExtension, IDisposable {
		public const string NewStorageUrlPrefix = "new:";
		private Dictionary<string, Type> newReportDataTypeByHandle;
		private XafApplication Application {
			get { return ValueManager.GetValueManager<XafApplication>("XafReportStorageExtension_XafApplication").Value; }
			set { ValueManager.GetValueManager<XafApplication>("XafReportStorageExtension_XafApplication").Value = value; }
		}
		private Type ReportDataType {
			get {
				ReportsModule reportsModule = (ReportsModule)Application.Modules.FindModule(typeof(ReportsModule)); 
				if(reportsModule == null) {
					throw new InvalidOperationException("The ReportsModule is not found");
				}
				return reportsModule.ReportDataType;
			}
		}
		public XafReportStorageExtension(XafApplication application) {
			Guard.ArgumentNotNull(application, "application");
			Application = application;
			newReportDataTypeByHandle = new Dictionary<string, Type>();
		}
		public override bool IsValidUrl(string url) {
			if(string.IsNullOrEmpty(url)) {
				return false;
			}
			if(IsNewReportHandle(url)) {
				return false;
			}
			using(IObjectSpace os = Application.CreateObjectSpace(ReportDataType)) {
				return FindReportData(url, os) != null;
			}
		}
		public override byte[] GetData(string url) {
			using(IObjectSpace os = Application.CreateObjectSpace(ReportDataType)) {
				IReportData reportData = FindReportData(url, os);
				if(reportData != null) {
					if(!(reportData is IXtraReportData) || !FileFormatDetector.CreateXmlDetector().FormatExists(((IXtraReportData)reportData).Content)) {
						XafReport report = (XafReport)reportData.LoadReport(Application.CreateObjectSpace(ReportDataType));
						report.IsObjectSpaceOwner = true;
						using(MemoryStream ms = new MemoryStream()) {
							report.SaveLayout(ms);
							return ms.ToArray();
						}
					}
					else {
						return ((IXtraReportData)reportData).Content;
					}
				}
				else {
					throw new ArgumentException("reportData, " + url);
				}
			}
		}
		public override bool CanSetData(string url) {
			return IsValidUrl(url);
		}
		public override void SetData(XtraReport report, string url) {
			SetNewData(report, url);
		}
		public override void SetData(XtraReport report, Stream stream) {
			if(report is XafReport) {
				report.SaveLayout(stream);
			}
			else {
				report.SaveLayoutToXml(stream);
			}
		}
		public override string SetNewData(XtraReport report, string defaultUrl) {
			using(IObjectSpace os = Application.CreateObjectSpace(ReportDataType)) {
				IReportData reportData = null;
				if(IsNewReportHandle(defaultUrl)) {
					reportData = (IReportData)os.CreateObject(GetNewReportDataTypeFromHandle(defaultUrl));
				}
				else {
					reportData = os.GetObject(FindReportData(defaultUrl, os));
					if(reportData == null) {
						throw new ArgumentException("defaultUrl, " + defaultUrl);
					}
				}
				XafReport xafReport = report as XafReport;
				if(xafReport != null) {
					reportData.SaveReport(xafReport);
					os.CommitChanges();
				}
				else if(report.DataSource is ObjectSpaceComponent && reportData is IXtraReportData) {
					ObjectSpaceComponent osc = report.DataSource as ObjectSpaceComponent;
					using(MemoryStream stream = new MemoryStream()) {
						SetData(xafReport, stream);
						((IXtraReportData)reportData).Content = stream.GetBuffer();
						((IXtraReportData)reportData).ReportName = defaultUrl;
						((IXtraReportData)reportData).DataType = osc.DataType;
					}
					os.CommitChanges();
				}
				return reportData.ReportName;
			}
		}
		public override string GetNewUrl() {
			string url = string.Empty;
			using(IObjectSpace os = Application.CreateObjectSpace(ReportDataType)) {
				using(ListView chooseReportDataListView = Application.CreateListView(os, ReportDataType, true)) {
					ShowViewParameters parameters = new ShowViewParameters(chooseReportDataListView);
					parameters.TargetWindow = TargetWindow.NewModalWindow;
					parameters.Context = TemplateContext.PopupWindow;
					DialogController dialogController = Application.CreateController<ReportDataSelectionDialogController>();
					dialogController.Accepting += delegate(object sender, DialogControllerAcceptingEventArgs args) {
						if(chooseReportDataListView.SelectedObjects.Count > 0) {
							url = CreateReportHandle((IReportData)chooseReportDataListView.SelectedObjects[0]);
						}
					};
					parameters.Controllers.Add(dialogController);
					Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(null, null));
					return url;
				}
			}
		}
		#region Url to read a ReportData object. XtraReports works in this way.
		public virtual IReportData FindReportData(string handle, IObjectSpace objectSpace) {
			if(IsNewReportHandle(handle)) {
				return null;
			}
			try {
				return (IReportData)objectSpace.FindObject(ReportDataType, new BinaryOperator("ReportName", handle));
			}
			catch(Exception) {
				throw new ArgumentException(String.Format("Invalid handle '{0}'.", handle), "handle");
			}
		}
		public virtual string CreateReportHandle(IReportData reportData) {
			return reportData.ReportName;
		}
		#endregion
		#region Support for different ReportData types. There is no such a functionality in XtraReports.
		public virtual string CreateNewReportHandle(IReportData reportData) {
			return CreateNewReportHandle(reportData.GetType());
		}
		public string CreateNewReportHandle() {
			return CreateNewReportHandle(ReportDataType);
		}
		private string CreateNewReportHandle(Type reportDataType) {
			string handle = NewStorageUrlPrefix + reportDataType.FullName;
			if(!newReportDataTypeByHandle.ContainsKey(handle)) {
				newReportDataTypeByHandle.Add(handle, reportDataType);
			}
			return handle;
		}
		public virtual bool IsNewReportHandle(string url) {
			return string.IsNullOrEmpty(url) || url.StartsWith(NewStorageUrlPrefix);
		}
		public virtual Type GetNewReportDataTypeFromHandle(string url) {
			if(string.IsNullOrEmpty(url)) {
				return ReportDataType;
			}
			Type reportDataType;
			if(IsNewReportHandle(url) && newReportDataTypeByHandle.TryGetValue(url, out reportDataType)) {
				return reportDataType;
			}
			return null;
		}
		#endregion
		public void Dispose() {
			Application = null;
			newReportDataTypeByHandle = null;
		}
	}
	public class ReportDataSelectionDialogController : DialogController {
		private const string IsSingleReportSelected = "IsSingleReportSelected";
		private void ListViewProcessCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
			e.Handled = true;
		}
		private ListViewProcessCurrentObjectController ListViewProcessCurrentObjectController {
			get {
				if(Frame != null) {
					return Frame.GetController<ListViewProcessCurrentObjectController>();
				}
				return null;
			}
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			AcceptAction.Enabled[IsSingleReportSelected] = ((View)sender).SelectedObjects.Count == 1;
		}
		private void Frame_ViewChanging(object sender, ViewChangingEventArgs e) {
			if(Frame.View != null) {
				Frame.View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			}
			if(e.View != null) {
				e.View.SelectionChanged += new EventHandler(View_SelectionChanged);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(ListViewProcessCurrentObjectController != null) {
				ListViewProcessCurrentObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(ListViewProcessCurrentObjectController_CustomProcessSelectedItem);
			}
			Frame.ViewChanging += new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
		}
		protected override void OnDeactivated() {
			Frame.ViewChanging -= new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
			if(ListViewProcessCurrentObjectController != null) {
				ListViewProcessCurrentObjectController.CustomProcessSelectedItem -= new EventHandler<CustomProcessListViewSelectedItemEventArgs>(ListViewProcessCurrentObjectController_CustomProcessSelectedItem);
			}
			base.OnDeactivated();
		}
	}
}
