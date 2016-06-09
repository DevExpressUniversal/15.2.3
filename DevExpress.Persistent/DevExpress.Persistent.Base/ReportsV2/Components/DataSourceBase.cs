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
using System.Drawing.Design;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraReports.UI;
namespace DevExpress.Persistent.Base.ReportsV2 {
	[Designer("DevExpress.ExpressApp.ReportsV2.Win.Designers.ComponentDataSourceDesigner, DevExpress.ExpressApp.ReportsV2.Win" + XafApplication.CurrentVersion + XafAssemblyInfo.AssemblyNamePostfix)]
	[ComponentStorageItem]
	public abstract class DataSourceBase : Component, IList, ITypedList, ISupportCriteria, ISupportSorting, IListAdapter, ISupportInitialize, IServiceProvider, IBindingList, IObject {
		private string objectTypeName;
		private IObjectSpace currentObjectSpace;
		private object viewDataSource;
		private string name = string.Empty;
		private object designTimeDataSource;
		private bool isDisposed = false;
		private SortingCollection sorting;
		private CriteriaOperator criteria;
		private CriteriaOperator criteriaWithParameters;
		private int topReturnedRecords;
		private Locker initalizationCounter = new Locker();
		private static bool? isDesignMode;
		static DataSourceBase() {
			if(IsDesignMode) {
				RegisterCustomOperators();
			}
		}
		public DataSourceBase() {
			criteria = null;
		}
		#region Properties
		[Category("Data")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[SRCategory(ReportStringId.CatData)]
		[Editor("DevExpress.ExpressApp.ReportsV2.Win.Editors.FilterEditor, DevExpress.ExpressApp.ReportsV2.Win" + XafApplication.CurrentVersion + XafAssemblyInfo.AssemblyNamePostfix, typeof(UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("DataSourceBaseCriteria")]
#endif
		public CriteriaOperator Criteria {
			get { return criteria; }
			set {
				SetCriteria(value, true);
			}
		}
		[Category("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.ExpressApp.ReportsV2.Win.Editors.SortingCollectionEditor, DevExpress.ExpressApp.ReportsV2.Win" + XafApplication.CurrentVersion + XafAssemblyInfo.AssemblyNamePostfix, typeof(UITypeEditor))]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("DataSourceBaseSorting")]
#endif
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public SortingCollection Sorting {
			get {
				if(sorting == null) {
					sorting = new SortingCollection();
					sorting.Changed += new EventHandler(sorting_Changed);
				}
				return sorting;
			}
			set {
				if(sorting != null) {
					sorting.Changed -= new EventHandler(sorting_Changed);
				}
				sorting = new SortingCollection();
				if(value != null) {
					sorting.AddRange(value);
				}
				sorting.Changed += new EventHandler(sorting_Changed);
				RefreshViewDataSource();
				SortingChanged();
			}
		}
		private void sorting_Changed(object sender, EventArgs e) {
			RefreshViewDataSource();
			SortingChanged();
		}
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("DataSourceBaseTopReturnedRecords")]
#endif
		[XtraSerializableProperty]
		public int TopReturnedRecords {
			get { return topReturnedRecords; }
			set { topReturnedRecords = value; }
		}
		[DefaultValue(""), Browsable(false), XtraSerializableProperty(-1)]
		public string Name {
			get { return Site != null ? Site.Name : name; }
			set {
				if(Site == null || string.Equals(Site.Name, value))
					name = value;
			}
		}
		[Category("Data")]
		[TypeConverter("DevExpress.ExpressApp.ReportsV2.Win.ReportDataTypeConverterForDesigner, DevExpress.ExpressApp.ReportsV2.Win" + XafApplication.CurrentVersion + XafAssemblyInfo.AssemblyNamePostfix)]
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("DataSourceBaseObjectTypeName")]
#endif
		[XtraSerializableProperty(-1)]
		public string ObjectTypeName {
			get {
				return objectTypeName;
			}
			set {
				SetObjectTypeName(value);
			}
		}
		#endregion
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(-1)]
		public string ObjectType {
			get {
				Type type = GetType();
				return string.Format("{0},{1}", type.FullName, type.Assembly.GetName().Name);
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Type DataType {
			get {
				if(!string.IsNullOrEmpty(ObjectTypeName)) {
					ITypesInfo typesInfoService = GetService(typeof(ITypesInfo)) as ITypesInfo;
					ITypeInfo typeInfo = typesInfoService.FindTypeInfo(ObjectTypeName);
					if(typeInfo != null) {
						return typeInfo.Type;
					}
				}
				return null;
			}
		}
		public static bool IsDesignMode {
			get {
				if(!isDesignMode.HasValue) {
					isDesignMode = DesignTimeTools.IsDesignMode;
				}
				return isDesignMode.Value;
			}
		}
		public static IObjectSpace CreateObjectSpace(Type dataType, IServiceProvider serviceProvider) {
			if(dataType != null) {
				IReportObjectSpaceProvider osProvider = serviceProvider.GetService(typeof(IReportObjectSpaceProvider)) as IReportObjectSpaceProvider;
				if(osProvider != null) {
					return osProvider.GetObjectSpace(dataType);
				}
			}
			return null;
		}
		[Browsable(false)]
		public bool IsDisposed {
			get { return isDisposed; }
		}
		[Browsable(false)]
		[DefaultValue(null)]
		[XtraSerializableProperty(-1)]
		public string CriteriaString {
			get {
				CriteriaOperator criteria = Criteria;
				return (object)criteria == null ? null : criteria.ToString();
			}
			set {
				if(CriteriaString != value) {
					Criteria = CriteriaOperator.Parse(value);
				}
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetCriteria(CriteriaOperator newCriteria) {
			SetCriteria(newCriteria, false);
		}
		public void UpdateCriteriaWithReportParameters(XtraReport report) {
			if(!CriteriaOperator.Equals(Criteria, null)) {
				CriteriaOperator newCriteria = CriteriaOperator.Clone(Criteria);
				ParametersValueSetter param = new ParametersValueSetter(report.Parameters);
				newCriteria.Accept(param);
				if(!CriteriaOperator.Equals(Criteria, newCriteria) && !CriteriaOperator.Equals(criteriaWithParameters, newCriteria)) {
					criteriaWithParameters = newCriteria;
					RefreshViewDataSource();
				}
			}
		}
		public object GetParameterValue(DevExpress.XtraReports.Parameters.Parameter parameter) {
			Guard.ArgumentNotNull(parameter, "parameter");
			DevExpress.XtraReports.Parameters.DynamicListLookUpSettings settings = parameter.LookUpSettings as DevExpress.XtraReports.Parameters.DynamicListLookUpSettings;
			if(settings == null) {
				return parameter.Value;
			}
			Type objectType = ((DataSourceBase)settings.DataSource).DataType;
			if(parameter.MultiValue) {
				ArrayList values = new ArrayList();
				foreach(var value in (IEnumerable)parameter.Value) {
					values.Add(ObjectSpace.GetObjectByKey(objectType, value));
				}
				return values;
			}
			else {
				return ObjectSpace.GetObjectByKey(objectType, parameter.Value);
			}
		}
		public void BeginInit() {
			initalizationCounter.Lock();
		}
		public void EndInit() {
			initalizationCounter.Unlock();
		}
		internal static void RegisterCustomOperators() {
			CustomOperatorDesignTimeHelper.Register("CurrentUserId");
			CustomOperatorDesignTimeHelper.Register("IsCurrentUserInRole");
			CustomOperatorDesignTimeHelper.Register("IsNewObject");
			foreach(string parameterName in ParametersFactory.GetRegisteredParameterNames()) {
				CustomOperatorDesignTimeHelper.Register(parameterName);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual ITypedList CriteriaPropertyDescriptorProvider {
			get {
				return new CollectionPropertyDescriptorProvider(objectTypeName, this.Name, this, true);
			}
		}
		protected ITypesInfo TypesInfo {
			get {
				return GetService(typeof(ITypesInfo)) as ITypesInfo;
			}
		}
		protected abstract object CreateViewDataSource();
		protected abstract object CreateDesignTimeDataSource();
		protected virtual CriteriaOperator DataSourceCriteria {
			get {
				CriteriaOperator result = null;
				if(!ReferenceEquals(criteriaWithParameters, null)) {
					result = criteriaWithParameters;
				}
				else if(!ReferenceEquals(Criteria, null)) {
					result = (CriteriaOperator)((ICloneable)Criteria).Clone();
				}
				new FilterWithObjectsProcessor(ObjectSpace).Process(result, FilterWithObjectsProcessorMode.ObjectToObject);
				return result;
			}
		}
		protected virtual void ObjectSpaceChanging(IObjectSpace newObjectSpace) { }
		protected virtual void ObjectTypeChanged() { }
		protected virtual void ObjectSpaceChanged() { }
		protected virtual void CriteriaChanged() { }
		protected virtual void SortingChanged() { }
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			isDisposed = true;
			DisposeDesignTimeDataSource();
			DisposeViewDataSource();
			SetObjectSpace(null);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected IObjectSpace ObjectSpace {
			get { return currentObjectSpace; }
		}
		protected void ObjectTypeChangedCore() {
			ObjectTypeChanged();
		}
		protected void ObjectSpaceChangingCore(IObjectSpace newObjectSpace) {
			ObjectSpaceChanging(newObjectSpace);
		}
		protected object DataSource {
			get {
				if(IsDisposed) {
					throw new ObjectDisposedException(GetType().FullName);
				}
				if(ViewDataSource != null) {
					return ViewDataSource;
				}
				if(designTimeDataSource == null) {
					designTimeDataSource = CreateDesignTimeDataSource();
				}
				return designTimeDataSource;
			}
		}
		protected void RefreshViewDataSource() {
			DisposeViewDataSource();
			if(listChanged != null) {
				listChanged(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
			}
		}
		protected void RefreshDesignTimeDataSource() {
			DisposeDesignTimeDataSource();
		}
		protected bool Initializing {
			get {
				return initalizationCounter.Locked;
			}
		}
		private void SetCriteria(CriteriaOperator newCriteria, bool validate) {
			if(validate) {
				ValidateCriteria(newCriteria);
			}
			criteria = newCriteria;
			RefreshViewDataSource();
			CriteriaChanged();
		}
		private void ValidateCriteria(CriteriaOperator criteria) {
			if(DataType != null && !CriteriaOperator.Equals(criteria, null)) {
				CriteriaWrapper wrapper = new CriteriaWrapper(DataType, criteria);
				wrapper.Validate();
			}
		}
		private object ViewDataSource {
			get {
				if(viewDataSource == null) {
					viewDataSource = CreateViewDataSource();
				}
				return viewDataSource;
			}
		}
		private void SetObjectSpace(IObjectSpace objectSpace) {
			if(currentObjectSpace != objectSpace) {
				ObjectSpaceChangingCore(objectSpace);
				DisposeViewDataSource();
				if(currentObjectSpace != null) {
					currentObjectSpace.Disposed -= new EventHandler(objectSpace_Disposed);
				}
				currentObjectSpace = objectSpace;
				if(currentObjectSpace != null) {
					currentObjectSpace.Disposed += new EventHandler(objectSpace_Disposed);
				}
				ObjectSpaceChanged();
			}
		}
		private void SetObjectTypeName(string typeName) {
			objectTypeName = typeName;
			RefreshDesignTimeDataSource();
			ObjectTypeChangedCore();
		}
		private void objectSpace_Disposed(object sender, EventArgs e) {
			SetObjectSpace(null);
		}
		private void DisposeViewDataSource() {
			if(viewDataSource != null && viewDataSource is IDisposable) {
				((IDisposable)viewDataSource).Dispose();
			}
			viewDataSource = null;
		}
		private void DisposeDesignTimeDataSource() {
			if(designTimeDataSource != null) {
				if(designTimeDataSource is IDisposable) {
					((IDisposable)designTimeDataSource).Dispose();
				}
				designTimeDataSource = null;
			}
		}
		void IListAdapter.FillList(IServiceProvider servProvider) {
			SetObjectSpace(CreateObjectSpace(DataType, servProvider));
		}
		bool IListAdapter.IsFilled {
		get { return false; }
		}
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			return GetService(serviceType);
		}
		protected override object GetService(Type service) {
			if(!IsDesignMode && service == typeof(ITypesInfo)) {
				return XafTypesInfo.Instance;
			}
			return base.GetService(service);
		}
		#endregion
		#region ITypedList
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(IsDisposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}
			return ((ITypedList)DataSource).GetItemProperties(listAccessors);
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			if(IsDisposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}
			return ((ITypedList)DataSource).GetListName(listAccessors); ;
		}
		#endregion
		#region IList
		void ICollection.CopyTo(Array array, int index) {
			((ICollection)DataSource).CopyTo(array, index);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		int ICollection.Count {
			get { return ((ICollection)DataSource).Count; }
		}
		int IList.Add(object value) {
			return ((IList)DataSource).Add(value);
		}
		void IList.Clear() {
			((IList)DataSource).Clear();
		}
		bool IList.Contains(object value) {
			return ((IList)DataSource).Contains(value);
		}
		int IList.IndexOf(object value) {
			return ((IList)DataSource).IndexOf(value);
		}
		void IList.Insert(int index, object value) {
			((IList)DataSource).Insert(index, value);
		}
		bool IList.IsFixedSize {
			get { return ((IList)DataSource).IsFixedSize; }
		}
		bool IList.IsReadOnly {
			get { return ((IList)DataSource).IsReadOnly; }
		}
		void IList.Remove(object value) {
			((IList)DataSource).Remove(value);
		}
		void IList.RemoveAt(int index) {
			((IList)DataSource).RemoveAt(index);
		}
		object IList.this[int index] {
			get {
				return ((IList)DataSource)[index];
			}
			set {
				((IList)DataSource)[index] = value;
			}
		}
		bool ICollection.IsSynchronized {
			get { return ((ICollection)DataSource).IsSynchronized; }
		}
		object ICollection.SyncRoot {
			get { return ((ICollection)DataSource).SyncRoot; }
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)DataSource).GetEnumerator();
		}
		#endregion
#if DebugTest
		public object GetDataSource() {
			return DataSource;
		}
		public void TestSetObjectSpace(IObjectSpace objectSpace) {
			SetObjectSpace(objectSpace);
		}
		public static void SetDesignMode(bool value) {
			isDesignMode = value;
		}
		public IObjectSpace ObjectSpaceForTests {
			get {
				return ObjectSpace;
			}
		}
#endif
		#region IBindingList for T103855, refresh binding to controls through IBindingList.ListChanged
		void IBindingList.AddIndex(PropertyDescriptor property) {
			throw new NotImplementedException();
		}
		object IBindingList.AddNew() {
			throw new NotImplementedException();
		}
		bool IBindingList.AllowEdit {
			get {
				return DataSource is IBindingList ? ((IBindingList)DataSource).AllowEdit : false;
			}
		}
		bool IBindingList.AllowNew {
			get {
				return DataSource is IBindingList ? ((IBindingList)DataSource).AllowNew : false;
			}
		}
		bool IBindingList.AllowRemove {
			get {
				return DataSource is IBindingList ? ((IBindingList)DataSource).AllowRemove : false;
			}
		}
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			Sorting = new SortingCollection(new DevExpress.Xpo.SortProperty(new OperandProperty(property.Name),
				direction == ListSortDirection.Ascending ? DevExpress.Xpo.DB.SortingDirection.Ascending : DevExpress.Xpo.DB.SortingDirection.Descending));
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new NotImplementedException();
		}
		bool IBindingList.IsSorted {
			get { throw new NotImplementedException(); }
		}
		ListChangedEventHandler listChanged;
		event ListChangedEventHandler IBindingList.ListChanged {
			add { this.listChanged += value; }
			remove { this.listChanged -= value; }
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
			throw new NotImplementedException();
		}
		void IBindingList.RemoveSort() {
			throw new NotImplementedException();
		}
		ListSortDirection IBindingList.SortDirection {
			get {
				return Sorting.Count == 0 || Sorting[0].Direction != DevExpress.Xpo.DB.SortingDirection.Descending ? ListSortDirection.Ascending : ListSortDirection.Descending;
			}
		}
		PropertyDescriptor IBindingList.SortProperty {
			get {
				return DataSource is IBindingList ? ((IBindingList)DataSource).SortProperty : null;
			}
		}
		bool IBindingList.SupportsChangeNotification {
			get {
				return DataSource is IBindingList ? ((IBindingList)DataSource).SupportsChangeNotification : false;
			}
		}
		bool IBindingList.SupportsSearching {
			get {
				return DataSource is IBindingList ? ((IBindingList)DataSource).SupportsSearching : false;
			}
		}
		bool IBindingList.SupportsSorting {
			get {
				return DataSource is IBindingList ? ((IBindingList)DataSource).SupportsSorting : false;
			}
		}
		#endregion
	}
	public class ComponentStorageItemAttribute : Attribute {
	}
}
