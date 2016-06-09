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
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.Data.Browsing;
using DevExpress.Data;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.XtraPrinting.Native;
using System.Drawing;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Parameters;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.Data.Native;
namespace DevExpress.XtraReports.UI {
	[
	TypeConverter("DevExpress.XtraReports.Design.XRBindingConverter," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRBinding : IXRBinding, IXRSerializable, IDisposable, ICloneable, IDataContainerBase {
		#region inner classes
		class DisplayColumnInfo {
			object prefixDataSource = new object();
			string prefixDataMember;
			object dataSource = new object();
			string dataMember;
			string storedResult = null;
			public string GetDisplayName(DataContext dc, object prefixDataSource, string prefixDataMember, object dataSource, string dataMember) {
				if(Equals(this.prefixDataSource, prefixDataSource) && Equals(this.prefixDataMember, prefixDataMember) &&
					Equals(this.dataSource, dataSource) && Equals(this.dataMember, dataMember) && storedResult != null) {
						return storedResult;
				}
				this.prefixDataSource = prefixDataSource;
				this.prefixDataMember = prefixDataMember;
				this.dataSource = dataSource;
				this.dataMember = dataMember;
				storedResult = GetDisplayNameCore(dc, prefixDataSource, prefixDataMember, dataSource, dataMember);
				return storedResult;
			}
			string GetDisplayNameCore(DataContext dc, object prefixDataSource, string prefixDataMember, object dataSource, string dataMember) {
				if(dataSource != null && Reference.Equals(dataSource, prefixDataSource) && !string.IsNullOrEmpty(prefixDataMember)) {
					string displayName;
					if(dc.TryGetDataMemberDisplayName(dataSource, dataMember, out displayName) ||
						dc.TryGetDataMemberDisplayName(dataSource, BindingHelper.JoinStrings(".", prefixDataMember, dataMember), out displayName)) {
						string displayPrefix = GetDisplayName(dc, prefixDataSource, prefixDataMember, prefixDataMember) + ".";
						return displayName.StartsWith(displayPrefix) ? displayName.Remove(0, displayPrefix.Length) : displayName;
					}
				} else if(dataSource != null)
					return GetDisplayName(dc, dataSource, dataMember, dataMember);
				else if(dataSource == null && prefixDataSource == null && !string.IsNullOrEmpty(prefixDataMember))
					return dataMember.StartsWith(prefixDataMember + ".") ? dataMember.Remove(0, prefixDataMember.Length + 1) : dataMember;
				return dataMember;
			}
			static string GetDisplayName(DataContext dc, object dataSource, string dataMember, string defaultResult) {
				string result;
				return dc.TryGetDataMemberDisplayName(dataSource, dataMember, out result) ? result : defaultResult;
			}
		}
		#endregion
		#region static
		public static XRBinding Create(string propertyName, object dataSource, string dataMember, string formatString) {
			if(dataSource is DevExpress.XtraReports.Native.Parameters.ParametersDataSource) {
				return new XRBinding((Parameter)((ParametersDataSource)dataSource).Parameters.GetByName(dataMember), propertyName, formatString);
			} else
				return new XRBinding(propertyName, dataSource, dataMember, formatString);
		}
		#endregion
		#region ICloneable
		object ICloneable.Clone() {
			return parameter != null ? new XRBinding(parameter, propertyName, formatString) :
				new XRBinding(propertyName, dataSource, dataMember, formatString);
		}
		#endregion
		#region fields used in function NullFields()
		string columnName;
		string bindingPath;
		DisplayColumnInfo displayColumnInfo = new DisplayColumnInfo();
		DataBrowser dataBrowser;
		PropertyDescriptor propertyDescriptor;
		#endregion
		#region fields & properties
		Parameter parameter;
		object dataSource;
		string propertyName = string.Empty;				
		string dataMember = string.Empty;
		string formatString = string.Empty;
		object columnValue;
		Type propertyType = typeof(object);
		XRBindingCollection owner;
		PropertyDescriptor bindProperty;
		static readonly object notSetValue = new object();
		object initialPropertyValue = notSetValue;
		Pair<Type, TypeConverter> typeConverterCached;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public Parameter Parameter {
			get {
				if(parameter == null)
					parameter = GetParameter(dataSource as ParametersDataSource, dataMember);
				return parameter;
			}
			set {
				parameter = value;
			}
		}
		static Parameter GetParameter(ParametersDataSource dataSource, string dataMember) {
			return dataSource != null ? (Parameter)dataSource.Parameters.GetByName(dataMember) : null;
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBindingFormatString"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBinding.FormatString"),
		Editor("DevExpress.XtraReports.Design.FormatStringEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DefaultValue(""),
		XtraSerializableProperty,
		]
		public string FormatString { get { return formatString; } set { formatString = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBindingPropertyName"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBinding.PropertyName"),
		XtraSerializableProperty,
		]
		public string PropertyName { get { return propertyName; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRBindingDataMember"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRBinding.DataMember"),
		DefaultValue(""),
		XtraSerializableProperty,
		]
		public string DataMember { 
			get { 
				return parameter != null ? parameter.Name : dataMember;
			}
		}
		string IDataContainerBase.DataMember {
			get { return DataMember; }
			set { }
		}
		[
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public object DataSource { 
			get {
				if(parameter != null && RootReport != null)
					return RootReport.ParametersDataSource;
				return dataSource != null ? dataSource : ParentDataSource;
			}
			internal set { dataSource = value; }
		}
		object ParentDataSource {
			get { return Report != null ? Report.GetEffectiveDataSource() : null; }
		}
		object IDataContainerBase.DataSource {
			get { return DataSource; }
			set { DataSource = value; }
		}
		bool ShouldSerializeDataSource() {
			return SerializeDataSource != null && Parameter == null;
		}
		internal object SerializeDataSource {
			get {
#if !DEBUGTEST
				if(SerializationUtils.IsFakedComponent(NativeDataSource))
					return null;
#endif
				return NativeDataSource;
			}
		}
		object NativeDataSource {
			get {
				return dataSource != ParentDataSource ? dataSource : null;
			}
		}
		internal string ColumnName { 
			get {
				if(columnName == null)
					UpdateFields(DataSource, DataMember);
				return columnName;
			}
		}
		internal object ColumnValue { get { return columnValue; } }
		internal string DisplayColumnName { 
			get {
				if(Parameter != null)
					return ParametersReplacer.GetParameterFullName(Parameter.Name);
				using(XRDataContextBase dc = new XRDataContextBase(RootReport != null ? RootReport.CalculatedFields : null, true)) {
					return displayColumnInfo.GetDisplayName(dc, Report != null ? Report.GetEffectiveDataSource() : null, Report != null ? Report.DataMember : string.Empty, DataSource, DataMember);
				}
			}
		}
		internal XRControl Control {
			get {
				return owner != null ? owner.Control : null;
			}
		}
		XtraReportBase Report {
			get {
				return Control != null ? Control.Report : null;
			}
		}
		XtraReport RootReport {
			get {
				return Control != null ? Control.RootReport : null;
			}
		}
		string BindingPath {
			get {
				if(bindingPath == null)
					UpdateFields(DataSource, DataMember);
				return bindingPath;
			}
		}
		bool IsImageType { get { return typeof(System.Drawing.Image).IsAssignableFrom(propertyType); } }
		#endregion
		public XRBinding() {
		}
		public XRBinding(Parameter parameter, string propertyName, string formatString) {
			this.parameter = parameter;
			this.propertyName = propertyName;
			this.formatString = formatString;
		}
		public XRBinding(string propertyName, object dataSource, string dataMember, string formatString) {
			Assign(propertyName, dataSource, dataMember);
			this.formatString = formatString;
		}
		public XRBinding(string propertyName, object dataSource, string dataMember) {
			Assign(propertyName, dataSource, dataMember);
		}
		internal XRBinding(string propertyName, DataPair dataPair) {
			Assign(propertyName, dataPair.Source, dataPair.Member);
		}
		private void Assign(string propertyName, object dataSource, string dataMember) {
			this.propertyName = propertyName;
			Assign(dataSource, dataMember);
		}
		public void Assign(object dataSource, string dataMember) {
			if(dataMember == null)
				throw new ArgumentNullException("dataMember");
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			NullFields();
			parameter = GetParameter(dataSource as ParametersDataSource, dataMember);
		}
		internal void Assign(DataPair dataPair) {
			Assign(dataPair.Source, dataPair.Member);
		}
		internal void CopyDataProperties(XRBinding binding) {
			Assign(binding.NativeDataSource, binding.dataMember);
			if(binding.NativeDataSource == null)
				this.Parameter = binding.Parameter;
		}
		public void Dispose() {
		}
		internal void AdjustDataSourse() {
			if(dataSource == ParentDataSource || dataSource == null) {
				dataSource = null;
				NullFields();
			}
		}
		internal XtraReportBase[] GetAppropriateReports() {
			List<XtraReportBase> reports = new List<XtraReportBase>();
			Report.CollectAllReports(reports);
			SetReportIndex(reports, Report, 0);
			using(XRDataContext dataContext = new XRDataContext(null, true)) {
				return GetAppropriateReports(dataContext, GetDataBrowser(dataContext), reports);
			}
		}
		static void SetReportIndex(List<XtraReportBase> reports, XtraReportBase report, int index) {
			reports.Remove(report);
			reports.Insert(index, report);
		}
		static XtraReportBase[] GetAppropriateReports(XRDataContext dataContext, DataBrowser dataBrowser, List<XtraReportBase> reports) {
			if (dataBrowser == null)
				return null;
			List<XtraReportBase> list = new List<XtraReportBase>();
			foreach (XtraReportBase report in reports) {
				DataBrowser reportDataBrowser = dataContext[report.DataSource, report.DataMember];
				if (reportDataBrowser == dataBrowser)
					list.Add(report);
			}
			if (list.Count == 0) {
				XtraReportBase[] parentReports = GetAppropriateReports(dataContext, dataBrowser.Parent, reports);
				if (parentReports != null)
					list.AddRange(parentReports);
			}
			return list.ToArray();
		}
		internal void NullFields() {
			bindingPath = null;
			columnName = null;
			propertyDescriptor = null;
			dataBrowser = null;
		}
		private void UpdateFields(object dataSource, string dataMember) {
			bindingPath = "";
			columnName = dataMember;
			int index = dataMember.LastIndexOf(".");
			if(index < 0) return;
			if(dataSource != null) {
				using(XRDataContext dataContext = new XRDataContext(null, true)) {
					columnName = dataContext.GetColumnName(dataSource, dataMember, true);
					if(columnName != null && dataMember.Length > columnName.Length) 
						bindingPath = dataMember.Substring(0, (dataMember.Length - columnName.Length - 1));
				}
			} 
			if(dataSource == null || columnName == null) {
				bindingPath = dataMember.Substring(0, index);
				columnName = dataMember.Substring(index + 1);
			}
		}
		internal void UpdatePropertyValue(XRDataContext dataContext, ImagesContainer images) {
			object val = GetColumnValue(dataContext, images);
			if(val != null && propertyType.IsValueType)
				val = Convert.ChangeType(val, propertyType);
			this.bindProperty.SetValue(Control, val);
		}
		internal void SaveInitialPropertyValue() {
			initialPropertyValue = NativeMethods.GetCloneValue(Accessor.GetProperty(Control, propertyName));
		}
		internal void RestoreInitialPropertyValue() {
			if(!ReferenceEquals(initialPropertyValue, notSetValue))
				Accessor.SetProperty(Control, propertyName, initialPropertyValue);
		}
		#region Serialization
		void IXRSerializable.SerializeProperties(XRSerializer serializer) {
			serializer.SerializeString("PropertyName", propertyName);
			serializer.SerializeString("DataMember", dataMember);
			serializer.SerializeString("FormatString", formatString);
			serializer.SerializeDataSourceReference("DataSource", dataSource);
		}
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			propertyName = serializer.DeserializeString("PropertyName", "");
			dataMember = serializer.DeserializeString("DataMember", "");
			formatString = serializer.DeserializeString("FormatString", "");
			dataSource = serializer.DeserializeDataSourceReference("DataSource", null);
			NullFields();
		}
		IList IXRSerializable.SerializableObjects { get { return new object[] {}; } }
		#endregion
		internal void SetOwner(XRBindingCollection owner) {
			this.owner = owner;
			if(this.owner == null) {
				NullFields();
				return;
			}
			PropertyDescriptor descriptor = XRAccessor.GetPropertyDescriptor(Control, PropertyName);
			if(descriptor != null) {
				this.propertyType = GetActualType(descriptor.PropertyType);
				this.bindProperty = XRAccessor.GetFastPropertyDescriptor(descriptor);
			} else {
				this.propertyType = null;
				this.bindProperty = null;
			}
		}
		static Type GetActualType(Type type) {
			if(type.IsGenericType) {
				Type[] args = type.GetGenericArguments();
				if(args != null && args.Length == 1)
					return args[0];
			}
			return type;
		}
		internal object GetColumnValue(XRDataContext dataContext, ImagesContainer images) {
			object unconvertedColumnValue = GetUnconvertedColumnValue(dataContext, images);
			return ConvertObject(unconvertedColumnValue, images);
		}
		internal object GetUnconvertedColumnValue(XRDataContext dataContext, ImagesContainer images) {
			columnValue = GetImmediateColumnValue(dataContext, images);
			columnValue = OracleConverter.Instance.Convert(columnValue);
			return columnValue;
		}
		internal object GetImmediateColumnValue(XRDataContext dataContext, ImagesContainer images) {
			UpdateDataBrowser(dataContext);
			if(dataBrowser == null || dataBrowser.Current == null) {
				TraceErrorConditionally(true);
				return null;
			}
			if(images != null && IsImageType) {
				Image image = images.GetImageByKey(CreateKey());
				if(image != null)
					return image;
			}
			if(propertyDescriptor == null || propertyDescriptor.ComponentType != dataBrowser.Current.GetType())
				propertyDescriptor = GetPropertyDescriptorCore(dataBrowser);
			TraceErrorConditionally(propertyDescriptor == null);
			object value = propertyDescriptor != null ? propertyDescriptor.GetValue(dataBrowser.Current) : null;
			if(Control != null) {
				BindingEventArgs e = new BindingEventArgs(this, value);
				Control.OnEvaluateBinding(e);
				return e.Value;
			}
			return value;
		}
		void TraceErrorConditionally(bool condition) {
			if(Control != null && condition)
				TraceCenter.TraceErrorOnce(Control, TraceSR.Format_InvalidBinding, PropertyName);
		}
		MultiKey CreateKey() {
			ArrayList keyParts = new ArrayList(4);
			if(dataBrowser == null)
				return new MultiKey(keyParts.ToArray());
			keyParts.AddRange(new object[] { DataMember, dataBrowser.DataSource });
			DataBrowser currentBrowser = dataBrowser;
			while(currentBrowser != null) {
				keyParts.Add(currentBrowser.Position);
				ListBrowser listBrowser = currentBrowser as ListBrowser;
				if(listBrowser != null) {
					SortedListController sortedListController = listBrowser.ListController as SortedListController;
					if(sortedListController != null)
						keyParts.Add(sortedListController.FilterCriteria);
				}
				currentBrowser = currentBrowser.Parent;
			}
			return new MultiKey(keyParts.ToArray());
		}
		internal bool IsValidDataSource(XRDataContextBase dataContext) {
			DataBrowser dataBrowser = GetDataBrowser(dataContext);
			return dataBrowser != null && GetPropertyDescriptorCore(dataBrowser) != null;
		}
		PropertyDescriptor GetPropertyDescriptorCore(DataBrowser dataBrowser) {
			DataBrowser rootBrowser = dataBrowser.GetRootBrowser();
			PropertyDescriptor propertyDescriptor = !(rootBrowser.DataSource is ITypedList) && dataBrowser.Current != null ? 
				FindProperty(dataBrowser.Current, ColumnName) : null;
			propertyDescriptor = propertyDescriptor ?? dataBrowser.FindItemProperty(ColumnName, true);
			propertyDescriptor = propertyDescriptor ?? ((rootBrowser.DataSource is ITypedList) && dataBrowser.Current != null ? FindProperty(dataBrowser.Current, ColumnName) : null);
			return propertyDescriptor != null ? XRAccessor.GetFastPropertyDescriptor(propertyDescriptor) : null;
		}
		static PropertyDescriptor FindProperty(object component, string name) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
			return properties.Find(name, false);
		}
		DataBrowser GetDataBrowser(XRDataContextBase dataContext) {
			return dataContext.GetDataBrowser(DataSource, BindingPath, true);
		}
		void UpdateDataBrowser(XRDataContext dataContext) {
			if (dataBrowser == null || dataBrowser.IsClosed)
				dataBrowser = GetDataBrowser(dataContext);
		}
		object ConvertObject(object obj, ImagesContainer images) {
			if(typeof(string) == propertyType) {
				if(obj is char[])
					obj = new String((char[])obj);
				else if(obj is byte[])
					obj = BitConverter.ToString((byte[])obj);
				if(obj == null || obj is DBNull)
					return string.Empty;
				if(formatString.Length > 0)
					return String.Format(formatString, obj);
				return ConvertToString(obj);		 
			} else if(IsImageType) {
				if(obj is Image)
					return obj;
				if(images != null) {
					object key = CreateKey();
					Image image = images.GetImageByKey(key);
					if(image != null)
						return image;
					if(obj is Byte[]) {
						image = PSConvert.ImageFromArray(obj as Byte[]);
						if(image != null) {
							images.Add(key, image);
							return image;
						}
					}
				}
				return null;
			} else if(typeof(CheckState).IsAssignableFrom(propertyType)) {
				try { 
					return (CheckState)Convert.ToInt32(obj); } 
				catch {
					return CheckState.Indeterminate;
				}
			}
			return obj;
		}
		string ConvertToString(object obj) {
			Type objType = obj.GetType();
			if(objType.IsEnum || !DataContext.IsStandardType(objType)) {
				TypeConverter typeConverter = null;
				if(typeConverterCached != null && objType == typeConverterCached.First) {
					typeConverter = typeConverterCached.Second;
				} else {
					object[] typeConverterAttributes = objType.GetCustomAttributes(typeof(TypeConverterAttribute), false);
					if(typeConverterAttributes != null && typeConverterAttributes.Length > 0) {
						TypeConverter converter = TypeDescriptor.GetConverter(objType);
						if(converter != null && converter.CanConvertTo(typeof(string))) {
							typeConverter = converter;
							typeConverterCached = new Pair<Type, TypeConverter>(objType, typeConverter);
						}
					}
				}
				if(typeConverter != null) {
					return typeConverter.ConvertToString(obj);
				}
			}
			return obj.ToString();	
		}
		public override string ToString() {
			string result = String.Format("[{0}]", DisplayColumnName);
			if(!String.IsNullOrEmpty(FormatString))
				result = String.Format(FormatString, result);
			return result;
		}
	}
	[
	Editor(typeof(System.Drawing.Design.UITypeEditor), typeof(System.Drawing.Design.UITypeEditor)), 
	TypeConverter(typeof(DevExpress.XtraReports.Design.XRBindingCollectionConverter)),
	ListBindable(BindableSupport.No),
	]
	public class XRBindingCollection : CollectionBase, ICloneable, IDisposable, IEnumerable<XRBinding> {
		private XRControl control;
		internal XRControl Control {
			get { return control; }
		} 
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRBindingCollectionItem")]
#endif
		public XRBinding this[int index] { 	
			get { return List[index] as XRBinding; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRBindingCollectionItem")]
#endif
		public XRBinding this[string propName] { 	
			get {
				for(int i = 0; i < Count; i++)
					if(this[i].PropertyName == propName) return this[i];
				return null;
			}
		}
		internal XRBindingCollection(XRControl control) {
			this.control = control;
		}
		#region ICloneable
		object ICloneable.Clone() {
			XRBindingCollection bindings = new XRBindingCollection(control);
			foreach(XRBinding binding in this) {
				bindings.Add((XRBinding)((ICloneable)binding).Clone());
			}
			return bindings;
		}
		#endregion
		public XRBindingCollection() {
		}
		private void AddCore(XRBinding binding) {
			if(binding == null) throw( new ArgumentNullException("binding") ); 
			XRBinding b = this[binding.PropertyName];
			if(b != null) List.Remove(b);
			List.Add(binding);
			binding.SetOwner(this);
		}
		public void Add(XRBinding binding) {
			AddCore(binding);
		}
		public XRBinding Add(string propertyName, object dataSource, string dataMember) {
			XRBinding binding = new XRBinding(propertyName, dataSource, dataMember);
			AddCore(binding);
			return binding;
		}
		public XRBinding Add(string propertyName, object dataSource, string dataMember, string formatString) {
			XRBinding binding = new XRBinding(propertyName, dataSource, dataMember, formatString);
			AddCore(binding);
			return binding;
		}
		public void AddRange(XRBinding[] bindings) {
			foreach(XRBinding binding in bindings) {
				Add(binding);
			}
		}
		public void Remove(XRBinding binding) {
			List.Remove(binding);
			binding.SetOwner(null);
		}
		internal bool Contains(params string[] propNames) {
			foreach(string propName in propNames) {
				if(this[propName] != null) return true;
			}
			return false;
		}
		public void Dispose() {
			foreach(XRBinding binding in this)
				binding.Dispose();
			this.control = null;
		}
		#region IEnumerable<IXRBinding> Members
		IEnumerator<XRBinding> IEnumerable<XRBinding>.GetEnumerator() {
			foreach(XRBinding binding in List)
				yield return binding;
		}
		#endregion
	}
}
