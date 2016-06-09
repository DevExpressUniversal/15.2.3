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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.Reports {
	public class XpoReportDesignerFilterService : ITypeDescriptorFilterService {
		public ITypeDescriptorFilterService oldService = null;
		public XpoReportDesignerFilterService() { }
		public XpoReportDesignerFilterService(ITypeDescriptorFilterService oldService) {
			this.oldService = oldService;
		}
		public bool FilterAttributes(IComponent component, IDictionary attributes) {
			if(oldService != null)
				oldService.FilterAttributes(component, attributes);
			return true;
		}
		public bool FilterEvents(IComponent component, IDictionary events) {
			if(oldService != null)
				oldService.FilterEvents(component, events);
			return true;
		}
		public bool FilterProperties(IComponent component, IDictionary properties) {
			if(oldService != null)
				oldService.FilterProperties(component, properties);
			if(properties.Contains("DataSource")) {
				properties.Remove("DataSource");
			}
			if(properties.Contains("DataAdapter")) {
				properties.Remove("DataAdapter");
			}
			return true;
		}
	}
	public class FilterChangingEventArgs : CancelEventArgs {
		private string newFilter;
		public FilterChangingEventArgs(string newFilter)
			: base() {
			this.newFilter = newFilter;
			Cancel = false;
		}
		public string NewFilter {
			get { return newFilter; }
		}
	}
	public abstract class CustomSerializationBaseEventArgs : HandledEventArgs {
		private string valueName;
		public CustomSerializationBaseEventArgs(string valueName) {
			this.valueName = valueName;
		}
		public string ValueName {
			get { return valueName; }
		}
	}
	public class CustomDeserializeValueEventArgs : CustomSerializationBaseEventArgs {
		private string serializedValue;
		private object resultValue;
		public CustomDeserializeValueEventArgs(string valueName, string serializedValue)
			: base(valueName) {
			this.serializedValue = serializedValue;
		}
		public string SerializedValue {
			get { return serializedValue; }
		}
		public object ResultValue {
			get { return resultValue; }
			set { resultValue = value; }
		}
	}
	public class CustomSerializeValueEventArgs : CustomSerializationBaseEventArgs {
		private object sourceValue;
		private string resultValue = null;
		public CustomSerializeValueEventArgs(string valueName, object sourceValue)
			: base(valueName) {
			this.sourceValue = sourceValue;
		}
		public object SourceValue {
			get { return sourceValue; }
		}
		public string ResultValue {
			get { return resultValue; }
			set { resultValue = value; }
		}
	}
	public class ReportEnumLocalizer {
		private XtraReport report;
		private void control_EvaluateBinding(object sender, BindingEventArgs e) {
			object controlUnderlyingValue = e.Value;
			e.Value = GetLocalizedValueIfEnum((XRControl)sender, controlUnderlyingValue);
			if(controlUnderlyingValue == null || !controlUnderlyingValue.GetType().IsEnum) {
				((XRControl)sender).EvaluateBinding -= new BindingEventHandler(control_EvaluateBinding);
			}
		}
		private void AssignBeforePrintToChilds(XRControl parent) {
			foreach(XRControl control in parent.Controls) {
				if(control is XRLabel || control is XRTableCell) {
					control.EvaluateBinding += new BindingEventHandler(control_EvaluateBinding);
				}
				else {
					AssignBeforePrintToChilds(control);
				}
			}
		}
		protected virtual object GetLocalizedValueIfEnum(XRControl control, object controlUnderlyingValue) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(control, "control");
			if(controlUnderlyingValue != null && controlUnderlyingValue.GetType().IsEnum) {
				return new EnumDescriptor(controlUnderlyingValue.GetType()).GetCaption(controlUnderlyingValue);
			}
			return controlUnderlyingValue;
		}
		public void Attach(XtraReport report) {
			this.report = report;
			AssignBeforePrintToChilds(report);
		}
		public XtraReport Report {
			get { return report; }
		}
	}
	public enum XafReportPropertyNamesId {
		DataType,
		Filter,
		FilterDescription,
		FilterForDesignPreview,
		Filtering,
		IsParametrized,
		ParametersObjectType,
		ReportName
	}
	[XRDesigner("DevExpress.ExpressApp.Reports.Win.XpoReportDesigner,DevExpress.ExpressApp.Reports.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IRootDesigner))]
	[RootClass]
	public class XafReport : XtraReport {
		private const String DeserializationErrorMessage = @"The ""{0}"" is not initialized. The ""{1}"" type was not found.";
		private const String ReportNameDeserializationErrorMessage = @"Deserialized ReportName is null or empty. Report name still ""{0}"".";
		public const string SerializedValueFilter = "Filter";
		public const string SerializedValueFilterForDesignPreview = "FilterForDesignPreview";
		public const string SerializedValueFilterDescription = "FilterDescription";
		public const string SerializedValueParametersObjectTypeName = "ParametersObjectTypeName";
		public const string SerializedValueReportName = "ReportName";
		public const string SerializedValueDataType = "DataSourceClassName";
		public const string XafReportContextName = "XafReport";
		public const string DefaultResourceFile = "Resources." + DXDisplayNameAttribute.DefaultResourceFile;
		private bool isObjectSpaceOwner;
		private IObjectSpace objectSpace;
		private Type dataType;
		private ReportFiltering filtering;
		private LocalizedCriteriaWrapper criteriaWrapper;
		private string reportName;
		private bool refreshDataSource = true;
		private bool isInitializingDataSource = false;
		private ReportParametersObjectBase reportParametersObject;
		internal bool isClone = false;
		private bool IsClone {
			get { return isClone; }
		}
		protected LocalizedCriteriaWrapper CreateCriteriaWrapper() {
			if((dataType != null) && !String.IsNullOrEmpty(filtering.Filter)) {
				return CriteriaEditorHelper.GetCriteriaWrapper(filtering.Filter, dataType, objectSpace);
			}
			else {
				return null;
			}
		}
		protected internal void RaiseFilterChanged() {
			criteriaWrapper = null;
		}
		protected internal bool RaiseFilterChanging(string newFilter) {
			ValidateFilter(newFilter);
			if(FilterChanging != null) {
				FilterChangingEventArgs e = new FilterChangingEventArgs(newFilter);
				FilterChanging(this, e);
				return !e.Cancel;
			}
			else {
				return true;
			}
		}
		private void ValidateFilter(string newFilter) {
			CriteriaEditorHelper.GetCriteriaWrapper(newFilter, dataType, objectSpace);
		}
		private void SerializeValue(XRSerializer serializer, string valueName, object sourceValue) {
			CustomSerializeValueEventArgs args = new CustomSerializeValueEventArgs(valueName, sourceValue);
			OnCustomSerializeValue(args);
			if(!args.Handled && (sourceValue != null)) {
				if(sourceValue is Type) {
					args.ResultValue = ((Type)sourceValue).FullName;
				}
				else {
					args.ResultValue = sourceValue.ToString();
				}
			}
			serializer.SerializeString(valueName, args.ResultValue);
		}
		private TargetType DeserializeValue<TargetType>(XRSerializer serializer, string serializedValueName) {
			string serializedValue = serializer.DeserializeString(serializedValueName, "");
			CustomDeserializeValueEventArgs args = new CustomDeserializeValueEventArgs(serializedValueName, serializedValue);
			OnCustomDeserializeValue(args);
			if(args.Handled) {
				return (TargetType)args.ResultValue;
			}
			else {
				if(typeof(TargetType) == typeof(string)) {
					return (TargetType)(object)serializedValue;
				}
				else if(typeof(TargetType) == typeof(Type)) {
					if(!String.IsNullOrEmpty(serializedValue)) {
						Type result = DevExpress.Persistent.Base.ReflectionHelper.FindType(serializedValue);
						if(result == null) {
							Tracing.Tracer.LogError(DeserializationErrorMessage, serializedValueName, serializedValue);
						}
						return (TargetType)(object)result;
					}
				}
				else {
					throw new NotSupportedException(typeof(TargetType).FullName);
				}
			}
			return default(TargetType);
		}
		protected virtual void OnCustomDeserializeValue(CustomDeserializeValueEventArgs args) {
			if(CustomDeserializeValue != null) {
				CustomDeserializeValue(this, args);
			}
		}
		protected virtual void OnCustomSerializeValue(CustomSerializeValueEventArgs args) {
			if(CustomSerializeValue != null) {
				CustomSerializeValue(this, args);
			}
		}
		protected internal bool RaiseFilterForDesignPreviewChanging(string newFilter) {
			if(FilterForDesignPreviewChanging != null) {
				FilterChangingEventArgs e = new FilterChangingEventArgs(newFilter);
				FilterForDesignPreviewChanging(this, e);
				return !e.Cancel;
			}
			else {
				return true;
			}
		}
		protected override void InitializeScripts() {
			try {
				foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
					try {
						if(!AssemblyHelper.IsDynamic(assembly) && (ScriptReferencesString.IndexOf(assembly.Location) < 0) && File.Exists(assembly.Location)) {
							ScriptReferencesString = ScriptReferencesString + "\r\n" + assembly.Location;
						}
					}
					catch { } 
				}
			}
			catch { } 
			base.InitializeScripts();
		}
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			SerializeValue(serializer, SerializedValueDataType, dataType);
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(filtering, "filtering");
			SerializeValue(serializer, SerializedValueFilter, filtering.Filter);
			SerializeValue(serializer, SerializedValueFilterForDesignPreview, filtering.FilterForDesignPreview);
			SerializeValue(serializer, SerializedValueFilterDescription, filtering.FilterDescription);
			SerializeValue(serializer, SerializedValueParametersObjectTypeName, filtering.ParametersObjectType);
			SerializeValue(serializer, SerializedValueReportName, ReportName);
		}
		protected override void OnDataSourceChanging() {
			base.OnDataSourceChanging();
			if(!isInitializingDataSource) {
				CanAutomaticallyManageDataSource = false;
			}
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(filtering, "filtering");
			filtering.BeginInit();
			try {
				filtering.Filter = DeserializeValue<string>(serializer, SerializedValueFilter);
				filtering.FilterForDesignPreview = DeserializeValue<string>(serializer, SerializedValueFilterForDesignPreview);
			}
			finally {
				filtering.EndInit();
			}
			filtering.FilterDescription = DeserializeValue<string>(serializer, SerializedValueFilterDescription);
			filtering.ParametersObjectType = DeserializeValue<Type>(serializer, SerializedValueParametersObjectTypeName);
			string reportNameFromStream = DeserializeValue<string>(serializer, SerializedValueReportName);
			if(!string.IsNullOrEmpty(reportNameFromStream)) {
				ReportName = reportNameFromStream;
			}
			dataType = DeserializeValue<Type>(serializer, SerializedValueDataType);
			InitializeDataSource();
		}
		protected virtual ReportEnumLocalizer CreateReportEnumLocalizer() {
			return new ReportEnumLocalizer();
		}
		protected virtual void InitializeDataSource() {
			if(refreshDataSource  && !IsClone) {
				try {
					isInitializingDataSource = true;
					if(objectSpace != null && dataType != null) {
						IList originalDataSource = objectSpace.GetObjects(dataType, CollectionSourceBase.EmptyCollectionCriteria);
						base.DataSource = new ProxyCollection(objectSpace, XafTypesInfo.Instance.FindTypeInfo(dataType), originalDataSource);
					}
					else {
						base.DataSource = null;
					}
				}
				finally {
					isInitializingDataSource = false;
				}
			}
		}
		protected internal virtual void RefreshDataSourceForPrint() {
			object originalDataSource = null;
			if(base.DataSource is ProxyCollection) {
				originalDataSource = ((ProxyCollection)base.DataSource).OriginalCollection;
			}
			else {
				originalDataSource = base.DataSource;
			}
			if(originalDataSource != null) {
				CriteriaOperator criteria = null;
				if(IsClone) {
					if(!String.IsNullOrEmpty(filtering.FilterForDesignPreview)) {
						CriteriaWrapper previewCriteriaWrapper = CriteriaEditorHelper.GetCriteriaWrapper(filtering.FilterForDesignPreview, dataType, objectSpace);
						previewCriteriaWrapper.UpdateParametersValues();
						criteria = previewCriteriaWrapper.CriteriaOperator;
					}
				}
				else {
					if(reportParametersObject != null) {
						criteria = reportParametersObject.GetCriteria();
						SortProperty[] sorting = reportParametersObject.GetSorting();
						ObjectSpace.SetCollectionSorting(originalDataSource, sorting);
					}
					else if(CriteriaWrapper != null) {
						CriteriaWrapper.UpdateParametersValues();
						criteria = CriteriaWrapper.CriteriaOperator;
					}
				}
				CriteriaOperator criteriaClone = null;
				if(!ReferenceEquals(criteria, null)) {
					criteriaClone = (CriteriaOperator)((ICloneable)criteria).Clone();
				}
				new FilterWithObjectsProcessor(ObjectSpace).Process(criteriaClone, FilterWithObjectsProcessorMode.ObjectToObject);
				ObjectSpace.ApplyCriteria(originalDataSource, criteriaClone);
				ObjectSpace.ReloadCollection(originalDataSource);
				if(base.DataSource is ProxyCollection) {
					((ProxyCollection)base.DataSource).Refresh();
				}
			}
		}
		protected override void OnBeforePrint(PrintEventArgs e) {
			if(ObjectSpace == null && MasterReport is XafReport) {
				IObjectSpace masterObjectSpace = ((XafReport)MasterReport).ObjectSpace;
				if(masterObjectSpace != null) {
					ObjectSpace = masterObjectSpace.CreateNestedObjectSpace();
					IsObjectSpaceOwner = true;
				}
			}
			if(refreshDataSource) {
				RefreshDataSourceForPrint();
			}
			ReportEnumLocalizer enumLocalizer = CreateReportEnumLocalizer();
			enumLocalizer.Attach(this);
			base.OnBeforePrint(e);
		}
		protected override void CollectAssociatedComponents(DesignItemList components) {
			base.CollectAssociatedComponents(components);
			components.Remove(base.DataSource);
			components.Remove(DataType);
			components.Remove(ObjectSpace);
			components.Remove(Filtering);
			components.Remove(ReportParametersObject);
		}
		protected override XtraReport CreateClone() {
			XafReport clone = (XafReport)base.CreateClone();
			clone.isClone = true;
			clone.ObjectSpace = ObjectSpace;
			return clone;
		}
		protected override void CopyFrom(XtraReport source, bool forceDataSource) {
			try {
				isInitializingDataSource = true;
				base.DataSource = null;
			}
			finally {
				isInitializingDataSource = false;
			} 
			base.CopyFrom(source, forceDataSource);
		}
		public XafReport()
			: base() {
			filtering = new ReportFiltering(this);
			PageHeaderBand pageHeader = new PageHeaderBand();
			PageFooterBand pageFooter = new PageFooterBand();
			pageHeader.Height = 30;
			pageFooter.Height = 30;
			Bands.Add(new DetailBand());
			Bands.Add(pageHeader);
			Bands.Add(pageFooter);
			ReportDesignExtension.AssociateReportWithExtension(this, XafReportContextName);
		}
		private string disposeCallStack = "";
		protected override void Dispose(bool disposing) {
			disposeCallStack += "\r\n";
			disposeCallStack += DevExpress.Persistent.Base.ReflectionHelper.GetCurrentCallStack();
			base.Dispose(disposing); 
			if(objectSpace != null && IsObjectSpaceOwner) {
				objectSpace.Dispose();
			}
			objectSpace = null;
		}
		public object GetFilteringObject() {
			if(reportParametersObject != null) {
				return reportParametersObject;
			}
			else if(CriteriaWrapper != null) {
				return CriteriaWrapper;
			}
			return null;
		}
		public void SetFilteringObject(LocalizedCriteriaWrapper criteriaWrapper) {
			this.criteriaWrapper = criteriaWrapper;
		}
		public void SetFilteringObject(object filteringObject) {
			if(filteringObject == null) {
				return;
			}
			else if(filteringObject is ReportParametersObjectBase) {
				reportParametersObject = (ReportParametersObjectBase)filteringObject;
			}
			else if(filteringObject is LocalizedCriteriaWrapper) {
				SetFilteringObject((LocalizedCriteriaWrapper)filteringObject);
			}
			else {
				throw new ArgumentException("Unknown value: " + filteringObject.GetType().FullName);
			}
		}
		public DetailView CreateParametersDetailView(XafApplication application) {
			DetailView detailView = null;
			if(typeof(ReportParametersObjectBase).IsAssignableFrom(filtering.ParametersObjectType)) {
				reportParametersObject = (ReportParametersObjectBase)TypeHelper.CreateInstance(filtering.ParametersObjectType, ObjectSpace, dataType);
				detailView = application.CreateDetailView(ObjectSpace, reportParametersObject, false); 
				if(detailView.Items.Count == 0) {
					detailView.Dispose();
					detailView = null;
				}
			}
			else if(CriteriaWrapper != null) {
				ParametersObject parametersObject = ParametersObject.CreateBoundObject(CriteriaWrapper.EditableParameters);
				detailView = parametersObject.CreateDetailView(objectSpace, application, false);
				if(!String.IsNullOrEmpty(filtering.FilterDescription)) {
					IModelStaticText detailViewItem = detailView.Model.AddNode<IModelStaticText>("Description");
					detailViewItem.Text = filtering.FilterDescription;
					detailView.InsertItem(0, detailViewItem);
					detailView.Model.Layout[0].Remove();
				}
			}
			return detailView;
		}
		public object GetParameterValue(string name) {
			if(CriteriaWrapper != null) {
				return CriteriaWrapper.GetParameterValue(name);
			}
			return null;
		}
		public object GetParameterValue(int index) {
			if(CriteriaWrapper != null) {
				return CriteriaWrapper.GetParameterValue(index);
			}
			return null;
		}
		[DXDisplayName(typeof(XafReport), DefaultResourceFile, "DevExpress.ExpressApp.Reports.IsParametrized", "Is Parametrized"),
		Localizable(true)]
		[Browsable(false)]
		public bool IsParametrized {
			get {
				return (CriteriaWrapper != null) && CriteriaWrapper.HasVisibleParameters
					|| typeof(ReportParametersObjectBase).IsAssignableFrom(filtering.ParametersObjectType);
			}
		}
		[Browsable(false)]
		new public string DisplayName {
			get { return base.DisplayName; }
			set { base.DisplayName = value; }
		}
		[DXDisplayName(typeof(XafReport), DefaultResourceFile, "DevExpress.ExpressApp.Reports.ReportName", "Report Name"),
		Localizable(true)]
		public string ReportName {
			get { return reportName; }
			set {
				reportName = value;
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(this)["DisplayName"];
				if(propertyDescriptor != null) {
					propertyDescriptor.SetValue(this, reportName);
				}
			}
		}
		[Browsable(false)]
		public string DisposeCallStack {
			get { return disposeCallStack; }
		}
		[Browsable(true), TypeConverter(typeof(ReportDataTypeConverter))]
		[DXDisplayName(typeof(XafReport), DefaultResourceFile, "DevExpress.ExpressApp.Reports.DataType", "Data Type"),
		SRCategory(ReportStringId.CatData), Localizable(true)]
		public Type DataType {
			get { return dataType; }
			set {
				dataType = value;
				InitializeDataSource();
				criteriaWrapper = null;
			}
		}
		[DefaultValue(true)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanAutomaticallyManageDataSource {
			get { return refreshDataSource; }
			set { refreshDataSource = value; }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[DXDisplayName(typeof(XafReport), DefaultResourceFile, "DevExpress.ExpressApp.Reports.Filtering"),
		SRCategory(ReportStringId.CatData), Localizable(true)]
		public ReportFiltering Filtering {
			get { return filtering; }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
			set {
				objectSpace = value;
				InitializeDataSource();
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReportParametersObjectBase ReportParametersObject {
			get { return reportParametersObject; }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LocalizedCriteriaWrapper CriteriaWrapper {
			get {
				if(criteriaWrapper == null) {
					criteriaWrapper = CreateCriteriaWrapper();
				}
				return criteriaWrapper;
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsObjectSpaceOwner {
			get { return isObjectSpaceOwner; }
			set { isObjectSpaceOwner = value; }
		}
		public event EventHandler<FilterChangingEventArgs> FilterChanging;
		public event EventHandler<FilterChangingEventArgs> FilterForDesignPreviewChanging;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<CustomDeserializeValueEventArgs> CustomDeserializeValue;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<CustomSerializeValueEventArgs> CustomSerializeValue;
#if DebugTest
		public void DebugTest_RefreshDataSourceForPrint() {
			RefreshDataSourceForPrint();
		}
		public bool DebugTest_IsClone {
			get { return isClone; }
			set { isClone = value; }
		}
#endif
	}
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class XafReportSerializationHelper {
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void LoadReport(IXtraReportData reportData, XafReport report) {
			byte[] content = reportData.Content;
			if(content != null && content.Length > 0) {
				int realLength = content.Length;
				while(content[realLength - 1] == 0) {
					realLength--;
				}
				MemoryStream stream = new MemoryStream(content, 0, realLength);
				report.LoadLayout(stream);
				ReportDesignExtension.AssociateReportWithExtension(report, XafReport.XafReportContextName); 
				stream.Close();
			}
			report.ReportName = reportData.ReportName;
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SaveReport(IXtraReportData reportData, XafReport report) {
			MemoryStream stream = new MemoryStream();
			report.SaveLayout(stream, true);
			reportData.Content = stream.GetBuffer();
			stream.Close();
			if(report.ReportName != reportData.ReportName) {
				reportData.ReportName = report.ReportName;
			}
			if(report.DataType != null) {
				reportData.DataType = report.DataType;
			}
		}
	}
}
