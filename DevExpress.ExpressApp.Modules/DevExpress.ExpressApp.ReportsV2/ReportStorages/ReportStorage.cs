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
using System.ComponentModel;
using System.IO;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class ReportsStorage : ReportStorageExtension, IPreviousReportStorageExtensionContainer {
		public const string NewStorageUrlPrefix = "new:";
		static ReportsStorage() {
			IgnoreSecurity = false;
		}
		public ReportsStorage() {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] 
		public static bool IgnoreSecurity { get; set; }
		public virtual void CopyFrom(IReportDataV2 sourceReportData, IReportDataV2Writable targetReportData) {
			Guard.ArgumentNotNull(sourceReportData, "sourceReportData");
			Guard.ArgumentNotNull(targetReportData, "targetReportData");
			if(sourceReportData.IsPredefined) {
				Guard.ArgumentNotNull(sourceReportData.PredefinedReportType, "PredefinedReportType");
				targetReportData.SetContent(null);
				if(sourceReportData is PredefinedReportDataContainer) {
					targetReportData.SetPredefinedReportType(sourceReportData.PredefinedReportType);
				}
				else {
					targetReportData.SetContent(GetContent(sourceReportData));
				}
			}
			else {
				targetReportData.SetContent(GetContent(sourceReportData));
			}
			targetReportData.SetDataType(sourceReportData.DataType);
			targetReportData.SetDisplayName(sourceReportData.DisplayName);
			targetReportData.SetParametersObjectType(sourceReportData.ParametersObjectType);
			if(sourceReportData is IInplaceReportV2 && targetReportData is IInplaceReportV2) {
				((IInplaceReportV2)targetReportData).IsInplaceReport = ((IInplaceReportV2)sourceReportData).IsInplaceReport;
			}
		}
		public virtual XtraReport LoadReport(IReportDataV2 reportData) {
			XtraReport result = null;
			if(reportData.IsPredefined) {
				Guard.TypeArgumentIs(typeof(XtraReport), reportData.PredefinedReportType, "PredefinedReportType");
				result = Activator.CreateInstance(reportData.PredefinedReportType) as XtraReport;
				Guard.ArgumentNotNull(result, "result");
			}
			else {
				result = CreateReport();
				LoadReportCore(reportData, result);  
			}
			RegisterExtension(result);
			return result;
		}
		protected virtual XtraReport CreateReport() {
			return new XtraReport();
		}
		private void LoadReportCore(IReportDataV2 reportData, XtraReport report) {
			byte[] content = reportData.Content;
			if(content != null && content.Length > 0) {
				int realLength = content.Length;
				while(content[realLength - 1] == 0) {
					realLength--;
				}
				MemoryStream stream = new MemoryStream(content, 0, realLength);
				if(ReportStoreMode == ReportStoreModes.XML) {
					report.LoadLayoutFromXml(stream);
				}
				else {
					report.LoadLayout(stream);
				}
				stream.Close();
			}
			DataSourceBase dataTypeContainer = report.DataSource as DataSourceBase;
			if(dataTypeContainer != null && reportData is IReportDataV2Writable) {
				((IReportDataV2Writable)reportData).SetDataType(dataTypeContainer.DataType);
			}
			report.DisplayName = reportData.DisplayName;
		}
		protected virtual byte[] GetContent(IReportDataV2 reportData) {
			Guard.ArgumentNotNull(reportData, "reportData");
			if(reportData.IsPredefined) {
				XtraReport report = ReportDataProvider.ReportsStorage.LoadReport(reportData);
				return GetBuffer(report, ReportStoreMode == ReportStoreModes.XML);
			}
			else {
				return ((IReportDataV2)reportData).Content;
			}
		}
		public virtual void SaveReport(IReportDataV2Writable reportData, XtraReport report) {
			if(reportData.IsPredefined) {
				throw new NotImplementedException();
			}
			reportData.SetContent(GetBuffer(report, ReportStoreMode == ReportStoreModes.XML));
			if(report.DisplayName != reportData.DisplayName) {
				if(!string.IsNullOrEmpty(report.DisplayName))
					reportData.SetDisplayName(report.DisplayName);
			}
			DataSourceBase dataTypeContainer = report.DataSource as DataSourceBase;
			if(dataTypeContainer != null) {
				reportData.SetDataType(dataTypeContainer.DataType);
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void RegisterExtension(XtraReport report) {
			ReportDesignExtension.AssociateReportWithExtension(report, ReportsModuleV2.XtraReportContextName);
		}
		private static byte[] GetBuffer(XtraReport report, bool saveToXml) {
			using(MemoryStream stream = new MemoryStream()) {
				if(saveToXml) {
					report.SaveLayoutToXml(stream);
				}
				else {
					report.SaveLayout(stream, true);
				}
				return stream.ToArray();
			}
		}
		public Type ReportDataType {
			get;
			set;
		}
		private ReportStoreModes reportStoreMode = ReportStoreModes.DOM;
		internal ReportStoreModes ReportStoreMode {
			get { return reportStoreMode; }
			set { reportStoreMode = value; }
		}
		public override bool IsValidUrl(string url) {
			if(!string.IsNullOrEmpty(url)) {
				if(PreviousReportStorageCanHandleUrl(url))
					return previousReportStorageExtension.IsValidUrl(url);
				if(!IsNewReportHandle(url)) {
					return IsValidV2Url(url);
				}
			}
			return false;
		}
		public override byte[] GetData(string url) {
			if(PreviousReportStorageCanHandleUrl(url)) {
				return previousReportStorageExtension.GetData(url);
			}
			Type reportDataType = GetReportDataTypeFromHandle(url);
			using(IObjectSpace objectSpace = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(reportDataType)) {
				IReportDataV2 reportData = GetReportDataV2(url, objectSpace);
				if(reportData != null) {
					return GetContent(reportData);
				}
				else {
					throw new ArgumentException("reportContainer, " + url);
				}
			}
		}
		public override bool CanSetData(string url) {
			if(PreviousReportStorageCanHandleUrl(url)) {
				return previousReportStorageExtension.CanSetData(url);
			}
			return IsValidUrl(url);
		}
		public override void SetData(XtraReport report, string url) {
			SetNewData(report, url);
		}
		public override void SetData(XtraReport report, Stream stream) {
			if(ReportStoreMode == ReportStoreModes.XML) {
				report.SaveLayoutToXml(stream);
			}
			else {
				report.SaveLayout(stream, true);
			}
		}
		public override string SetNewData(XtraReport report, string defaultUrl) {
			if(PreviousReportStorageCanHandleUrl(defaultUrl)) {
				return previousReportStorageExtension.SetNewData(report, defaultUrl);
			}
			Type reportDataType = GetReportDataTypeFromHandle(defaultUrl);
			using(IObjectSpace os = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(reportDataType)) {
				IReportDataV2Writable reportData = null;
				if(IsNewReportHandle(defaultUrl)) {
					reportData = CreateReportData(report, reportDataType, os);
				}
				else {
					reportData = os.GetObject(GetReportDataV2(defaultUrl, os)) as IReportDataV2Writable;
					if(reportData == null) {
						throw new ArgumentException("defaultUrl, " + defaultUrl);
					}
				}
				ReportDataProvider.ReportsStorage.SaveReport(reportData, report);
				os.CommitChanges();
				return CreateReportHandle(reportData, os);
			}
		}
		public virtual void CopyReports(params IReportDataV2[] reportsData) {
			Guard.ArgumentNotNull(reportsData, "reportsData");
			string newhandle = CreateNewReportHandle();
			Type reportDataType = GetReportDataTypeFromHandle(newhandle);
			using(IObjectSpace os = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(reportDataType)) {
				foreach(IReportDataV2 item in reportsData) {
					IReportDataV2Writable reportData = (IReportDataV2Writable)os.CreateObject(reportDataType);
					ReportDataProvider.ReportsStorage.CopyFrom(item, reportData);
				}
				os.CommitChanges();
			}
		}
		public override string GetNewUrl() {
			QuerySubReportUrlEventArgs args = new QuerySubReportUrlEventArgs(ReportDataType);
			if(!OnQuerySubReportUrl(args)) {
				if(previousReportStorageExtension != null) {
					return previousReportStorageExtension.GetNewUrl();
				}
			}
			return args.SubReportUrl;
		}
		public IList<ReportDataInfo> CollectInplaceReportsData() {
			IList<ReportDataInfo> result = new List<ReportDataInfo>();
			if(ReportDataType != null && typeof(IInplaceReportV2).IsAssignableFrom(ReportDataType)) {
				if(IgnoreSecurity || DataManipulationRight.CanRead(ReportDataType, null, null, null, null)) {
					using(IObjectSpace objectSpace = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(ReportDataType)) {
						IList allReports = objectSpace.CreateCollection(ReportDataType, new BinaryOperator(ReportsModuleV2.IsInplaceReportMemberName, true));
						foreach(IReportDataV2 reportData in allReports) {
							if(IgnoreSecurity || DataManipulationRight.CanRead(ReportDataType, null, reportData, null, objectSpace)) {
								result.Add(new ReportDataInfo(CreateReportHandle(reportData, objectSpace), reportData.DisplayName, reportData.DataType));
							}
						}
					}
				}
			}
			return result;
		}
		#region Url to read a ReportData object. XtraReports works in this way.
		internal IReportContainer GetReportContainerByHandle(string handle, bool loadReport) {
			Type reportDataType = GetReportDataTypeFromHandle(handle);
			Guard.ArgumentNotNull(handle, "handle");
			using(IObjectSpace objectSpace = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(reportDataType)) {
				IReportDataV2 data = GetReportDataV2(handle, objectSpace);
				Guard.ArgumentNotNull(data, "data");
				IReportContainer result = null;
				CreateCustomReportContainerEventArgs args = new CreateCustomReportContainerEventArgs(null, data);
				OnCreateCustomReportContainer(args);
				if(args.ReportContainer == null)
					result = new ReportContainer(data, loadReport);
				else result = args.ReportContainer;
				return result;
			}
		}
		public virtual IReportContainer GetReportContainerByHandle(string handle) {
			return GetReportContainerByHandle(handle, true);
		}
		public virtual string GetReportContainerHandle(IReportDataV2 reportData) {
			Guard.ArgumentNotNull(reportData, "reportData");
			using(IObjectSpace os = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(reportData.GetType())) {
				return CreateReportHandle(reportData, os);
			}
		}
		protected virtual string CreateReportHandle(IReportDataV2 reportData, IObjectSpace objectSpace) {
			return objectSpace.GetObjectHandle(reportData);
		}
		#endregion
		public string CreateNewReportHandle() {
			return CreateNewReportHandle(ReportDataType);
		}
		public string CreateNewReportHandle(Type reportDataType) {
			Guard.ArgumentNotNull(reportDataType, "reportDataType");
			return ReportsStorage.NewStorageUrlPrefix + ObjectHandleHelper.CreateObjectHandle(XafTypesInfo.Instance, reportDataType, "");
		}
		public virtual bool IsNewReportHandle(string url) {
			return string.IsNullOrEmpty(url) || url.StartsWith(NewStorageUrlPrefix);
		}
		public virtual string GetReportDataObjectHandleFromUrl(string url) {
			string objectHandle = url;
			if(IsNewReportHandle(objectHandle)) {
				objectHandle = objectHandle.Replace(NewStorageUrlPrefix, "");
			}
			return objectHandle;
		}
		public virtual Type GetReportDataTypeFromHandle(string url) {
			if(string.IsNullOrEmpty(url)) {
				return ReportDataType;
			}
			string objectHandle = url;
			if(IsNewReportHandle(objectHandle)) {
				objectHandle = objectHandle.Replace(NewStorageUrlPrefix, "");
			}
			string objectKey;
			Type result;
			ObjectHandleHelper.ParseObjectHandle(XafTypesInfo.Instance, objectHandle, out result, out objectKey);
			return result;
		}
		protected virtual void OnCreateCustomReportContainer(CreateCustomReportContainerEventArgs args) {
			if(CreateCustomReportContainer != null) {
				CreateCustomReportContainer(this, args);
			}
		}
		protected virtual bool OnQuerySubReportUrl(QuerySubReportUrlEventArgs args) {
			if(QuerySubReportUrl != null) {
				QuerySubReportUrl(this, args);
				return true;
			}
			return false;
		}
		private IReportDataV2 GetReportDataV2(string url, IObjectSpace objectSpace) {
			if(IsNewReportHandle(url)) {
				return null;
			}
			try {
				return (IReportDataV2)objectSpace.GetObjectByHandle(GetReportDataObjectHandleFromUrl(url));
			}
			catch(Exception) {
				throw new ArgumentException(String.Format("Invalid url '{0}'.", url), "url");
			}
		}
		private IReportDataV2Writable CreateReportData(XtraReport report, Type reportDataType, IObjectSpace os) {
			IReportDataV2Writable reportData = (IReportDataV2Writable)os.CreateObject(reportDataType);
			INewReportWizardParameters wizardParams = report.Tag as INewReportWizardParameters;
			if(wizardParams != null) {
				wizardParams.AssignData(reportData);
			}
			else {
				if(!string.IsNullOrEmpty(report.DisplayName)) {
					reportData.SetDisplayName(report.DisplayName);
				}
			}
			return reportData;
		}
		private bool PreviousReportStorageCanHandleUrl(string url) {
			if(previousReportStorageExtension == null) {
				return false;
			}
			if(IsValidV2Url(url)) {
				return false;
			}
			return true;
		}
		private bool IsValidV2Url(string url) {
			Type reportDataType = null;
			try {
				reportDataType = GetReportDataTypeFromHandle(url);
			}
			catch(ArgumentException) { }
			return reportDataType != null;
		}
		public event EventHandler<CreateCustomReportContainerEventArgs> CreateCustomReportContainer;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<QuerySubReportUrlEventArgs> QuerySubReportUrl;
		#region IPreviousReportStorageExtensionContainer Members
		private ReportStorageExtension previousReportStorageExtension;
		ReportStorageExtension IPreviousReportStorageExtensionContainer.PreviousReportStorageExtension {
			get { return previousReportStorageExtension; }
			set { previousReportStorageExtension = value; }
		}
		#endregion
	}
}
