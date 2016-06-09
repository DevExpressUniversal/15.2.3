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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using DevExpress.Utils.Design;
using DevExpress.Web.ASPxTreeList;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public abstract class ASPxDataWebControlDesignerBase : ASPxWebControlDesigner, IDataSourceViewSchemaAccessor {
		private bool fKeepDataSourceBrowsable = false;
		private ASPxDataWebControlBase fDataControlBase = null;
		protected ASPxDataWebControlBase DataControl { get { return fDataControlBase; } }
		public string DataSource {
			get {
				DataBinding binding = DataBindings["DataSource"];
				return (binding != null) ? binding.Expression : "";
			}
			set {
				if (string.IsNullOrEmpty(value))
					base.DataBindings.Remove("DataSource");
				else {
					DataBinding binding = DataBindings["DataSource"];
					if (binding == null)
						binding = new DataBinding("DataSource", typeof(IEnumerable), value);
					else
						binding.Expression = value;
					DataBindings.Add(binding);
				}
				OnDataSourceChanged(true);
#pragma warning disable 618
				OnBindingsCollectionChanged("DataSource");
#pragma warning restore 618
			}
		}
		public string DataSourceID {
			get { return DataControl.DataSourceID; }
			set {
				if (value != DataSourceID) {
					SetDataSourceID("DataSourceID", value);
				}
			}
		}
		protected void SetDataSourceID(string propertyName, string value) {
			if(value == SystemDesignSRHelper.GetDataControlNewDataSource()) {
				CreateDataSource(propertyName);
				TypeDescriptor.Refresh(base.Component);
			} else {
				if(value == SystemDesignSRHelper.GetDataControlNoDataSource())
					value = string.Empty;
				TypeDescriptor.Refresh(base.Component);
				SetDataSourceIDCore(propertyName, value);
			}
		}
		protected void SetDataSourceIDCore(string propertyName, string value) {
			Type type = DataControl.GetType();
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(type)[propertyName];
			string oldValue = (string)descriptor.GetValue(Component);
			if(oldValue != value) {
				descriptor.SetValue(Component, value);
				OnDataSourceChanged(false);
				OnSchemaRefreshed();
			}
		}
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema {
			get {
				return GetDesignerViewSchema();
			}
			set { }
		}
		public override void Initialize(IComponent component) {
			fDataControlBase = (ASPxDataWebControlBase)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.DesignTimeHtmlRequiresLoadComplete, true);
		}
		protected override void Dispose(bool disposing) {
			if (disposing && Component != null && Component.Site != null)
				DisconnectFromDataSource();
			base.Dispose(disposing);
		}
		protected override void OnBeforeControlHierarchyCompleted() {
			DataBind((ASPxDataWebControlBase)base.ViewControl);
		}
		protected override string GetDesignTimeHtmlInternal() {
			DataControl.SetDataSourceViewSchemaAccessor(this);
			TypeDescriptor.Refresh(DataControl);
			return base.GetDesignTimeHtmlInternal();
		}
		protected abstract bool ConnectToDataSource();
		protected abstract void CreateDataSource(string propertyName);
		protected abstract void DataBind(ASPxDataWebControlBase dataControl);
		protected abstract void DisconnectFromDataSource();
		protected abstract object GetDesignerViewSchema();
		protected override void OnDesignerLoadComplete(object sender, EventArgs e) {
			OnDataSourceChanged(false);
		}
		protected override void OnAnyComponentChanged(object sender, ComponentChangedEventArgs ce) {
			if (ce.Component is Control && ce.Member != null && ce.Member.Name == "ID" && Component != null &&
				((string)ce.OldValue == DataSourceID || (string)ce.NewValue == DataSourceID))
				OnDataSourceChanged(false);
		}
		protected override void OnComponentAdded(object sender, ComponentEventArgs e) {
			Control control = e.Component as Control;
			if (control != null && control.ID == DataSourceID)
				OnDataSourceChanged(false);
		}
		protected override void OnComponentRemoved(object sender, ComponentEventArgs e) {
			Control control = e.Component as Control;
			if (control != null && Component != null && control.ID == DataSourceID) {
				if (DesignerHost != null && !DesignerHost.Loading)
					OnDataSourceChanged(true);
			}
		}
		protected override void OnComponentRemoving(object sender, ComponentEventArgs e) {
			Control control = e.Component as Control;
			if (control != null && Component != null && control.ID == DataSourceID)
				DisconnectFromDataSource();
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PropertyDescriptor descriptor = (PropertyDescriptor)properties["DataSource"];
			if (descriptor == null)
				return;
			System.ComponentModel.AttributeCollection collection = descriptor.Attributes;
			int attributeIndex = -1;
			int attributesCount = -1;
			if (!string.IsNullOrEmpty(DataSource))
				fKeepDataSourceBrowsable = true;
			for (int i = 0; i < collection.Count; i++) {
				if (collection[i] is BrowsableAttribute) {
					attributeIndex = i;
					break;
				}
			}
			if (attributeIndex == -1 && string.IsNullOrEmpty(DataSource) && !fKeepDataSourceBrowsable)
				attributesCount = collection.Count + 1;
			else
				attributesCount = collection.Count;
			Attribute[] attributes = new Attribute[attributesCount];
			collection.CopyTo(attributes, 0);
			if (string.IsNullOrEmpty(DataSource) && !fKeepDataSourceBrowsable) {
				if (attributeIndex == -1)
					attributes[collection.Count] = BrowsableAttribute.No;
				else
					attributes[attributeIndex] = BrowsableAttribute.No;
			}
			descriptor = TypeDescriptor.CreateProperty(base.GetType(), "DataSource", typeof(string), attributes);
			properties["DataSource"] = descriptor;
		}
		protected void PerformPrefilterProperty(IDictionary properties, string propertyName, Type converterAttributeType) {
			PropertyDescriptor descriptor = (PropertyDescriptor)properties[propertyName];
			if (descriptor != null) {
				descriptor = TypeDescriptor.CreateProperty(GetType(), descriptor, new Attribute[] { new TypeConverterAttribute(converterAttributeType) });
				properties[propertyName] = descriptor;
			}
		}
		protected virtual void OnDataSourceChanged(bool forceUpdateView) {
			if (ConnectToDataSource() || forceUpdateView)
				UpdateDesignTimeHtml();
		}
		protected virtual void OnSchemaRefreshed() {
		}
		protected static Control FindControl(IServiceProvider serviceProvider, Control control, string controlIdToFind) {
			Type controlHelperType = ReflectionUtils.GetNonPublicTypeFromAssembly("System.Design", "ControlHelper");
			if (controlHelperType != null) {
				MethodInfo methodInfo = controlHelperType.GetMethod("FindControl", BindingFlags.NonPublic | BindingFlags.Static);
				if (methodInfo != null) {
					object[] args = new object[3] { serviceProvider, control, controlIdToFind };
					return methodInfo.Invoke(null, args) as Control;
				}
			}
			return null;
		}
		protected static System.Windows.Forms.DialogResult ShowCreateDataSourceDialog(ControlDesigner controlDesigner, Type dataSourceType, bool configure, out string dataSourceID) {
			return BaseDataBoundControlDesigner.ShowCreateDataSourceDialog(controlDesigner, dataSourceType, configure, out dataSourceID);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(CreateDataActionList());
			base.RegisterActionLists(list);
		}
		protected abstract DataWebControlActionListBase CreateDataActionList();
	}
	public class DataWebControlActionListBase : DesignerActionList {
		private ASPxDataWebControlDesignerBase designer;
		private object dataSourceDesigner;
		public override bool AutoShow {
			get { return true; }
			set { }
		}
		public ASPxDataWebControlDesignerBase Designer {
			get { return designer; }
		}
		public virtual string DataSourceID {
			get {
				if(string.IsNullOrEmpty(Designer.DataSourceID))
					return SystemDesignSRHelper.GetDataControlNoDataSource();
				return Designer.DataSourceID;
			}
			set {
				Designer.DataSourceID = value;
			}
		}
		public DataWebControlActionListBase(ASPxDataWebControlDesignerBase designer, object dataSourceDesigner)
			: base(designer.Component) {
			this.designer = designer;
			this.dataSourceDesigner = dataSourceDesigner;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			ControlDesigner designer = this.dataSourceDesigner as ControlDesigner;
			IComponent relatedComponent = designer != null ? designer.Component : null;
			return DesignUtils.CreateDataSourceDesignerActions(Designer, relatedComponent);
		}
	}
	public class ASPxHierarchicalDataWebControlDesigner : ASPxDataWebControlDesignerBase {
		private IHierarchicalDataSourceDesigner fDataSourceDesigner = null;
		private ASPxHierarchicalDataWebControl fHierarchicalDataControl = null;
		public IHierarchicalDataSourceDesigner DataSourceDesigner {
			get { return fDataSourceDesigner; }
		}
		public DesignerHierarchicalDataSourceView DesignerView {
			get { return (DataSourceDesigner != null) ? DataSourceDesigner.GetView("") : null; }
		}
		public override void Initialize(IComponent component) {
			fHierarchicalDataControl = (ASPxHierarchicalDataWebControl)component;
			base.Initialize(component);
		}
		protected override bool ConnectToDataSource() {
			IHierarchicalDataSourceDesigner designer = GetDataSourceDesigner();
			if (fDataSourceDesigner == designer)
				return false;
			if (fDataSourceDesigner != null) {
				fDataSourceDesigner.DataSourceChanged -= new EventHandler(OnDataSourceChanged);
				fDataSourceDesigner.SchemaRefreshed -= new EventHandler(OnSchemaRefreshed);
			}
			fDataSourceDesigner = designer;
			if (fDataSourceDesigner != null) {
				fDataSourceDesigner.DataSourceChanged += new EventHandler(OnDataSourceChanged);
				fDataSourceDesigner.SchemaRefreshed += new EventHandler(OnSchemaRefreshed);
			}
			return true;
		}
		protected override void CreateDataSource(string propertyName) {
			ControlDesigner.InvokeTransactedChange(Component, new TransactedChangeCallback(CreateDataSourceCallback), propertyName, StringResources.DataControl_CreateDataSourceTransaction);
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			IHierarchicalEnumerable enumerable = GetDesignTimeDataSource();
			string dataSourceID = dataControl.DataSourceID;
			object dataSource = dataControl.DataSource;
			ASPxHierarchicalDataWebControl hierarchicalDataControl = dataControl as ASPxHierarchicalDataWebControl;
			hierarchicalDataControl.DataSource = enumerable;
			hierarchicalDataControl.DataSourceID = "";
			try {
				if (enumerable != null)
					hierarchicalDataControl.DataBind();
			}
			finally {
				hierarchicalDataControl.DataSource = dataSource;
				hierarchicalDataControl.DataSourceID = dataSourceID;
			}
		}
		protected override void DisconnectFromDataSource() {
			if (fDataSourceDesigner != null) {
				fDataSourceDesigner.DataSourceChanged -= new EventHandler(OnDataSourceChanged);
				fDataSourceDesigner.SchemaRefreshed -= new EventHandler(OnSchemaRefreshed);
				fDataSourceDesigner = null;
			}
		}
		protected override object GetDesignerViewSchema() {
			if (DesignerView == null)
				return null;
			if (DesignerView.Schema == null && DataSourceDesigner != null)
				DataSourceDesigner.RefreshSchema(true);
			return DesignerView.Schema;  
		}
		protected virtual IHierarchicalEnumerable GetDesignTimeDataSource() {
			bool dummy;
			IHierarchicalEnumerable enumerable = null;
			if (DesignerView != null)
				enumerable = DesignerView.GetDesignTimeData(out dummy);
			else {
				DataBinding binding = DataBindings["DataSource"];
				if (binding != null)
					enumerable = DesignTimeData.GetSelectedDataSource(Component, binding.Expression, null) as IHierarchicalEnumerable;
			}
			if (enumerable != null) {
				ICollection collection = enumerable as ICollection;
				if (collection == null || collection.Count > 0)
					return enumerable;
			}
			return GetSampleDataSource();
		}
		protected virtual IHierarchicalEnumerable GetSampleDataSource() {
			return new HierarchicalSampleData(0, string.Empty);
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PropertyDescriptor descriptor = (PropertyDescriptor)properties["DataSource"];
			descriptor = TypeDescriptor.CreateProperty(GetType(), descriptor, new Attribute[] { new TypeConverterAttribute(typeof(HierarchicalDataSourceConverter)) });
			properties["DataSource"] = descriptor;
			descriptor = (PropertyDescriptor)properties["DataSourceID"];
			descriptor = TypeDescriptor.CreateProperty(GetType(), descriptor, new Attribute[] { new TypeConverterAttribute(typeof(HierarchicalDataSourceIDConverter)) });
			properties["DataSourceID"] = descriptor;
		}
		private bool CreateDataSourceCallback(object context) {
			string id = "";
			System.Windows.Forms.DialogResult result = ShowCreateDataSourceDialog(this, typeof(IHierarchicalDataSource), true, out id);
			if (!string.IsNullOrEmpty(id))
				DataSourceID = id;
			return (result == System.Windows.Forms.DialogResult.OK);
		}
		private IHierarchicalDataSourceDesigner GetDataSourceDesigner() {
			if (!string.IsNullOrEmpty(DataSourceID)) {
				Control dataSourceControl = FindControl(Component.Site, (Control)Component, DataSourceID);
				if (dataSourceControl != null && DesignerHost != null)
					return DesignerHost.GetDesigner(dataSourceControl) as IHierarchicalDataSourceDesigner;
			}
			return null;
		}
		private void OnDataSourceChanged(object sender, EventArgs e) {
			OnDataSourceChanged(true);
		}
		private void OnSchemaRefreshed(object sender, EventArgs e) {
			OnSchemaRefreshed();
		}
		protected override DataWebControlActionListBase CreateDataActionList() {
			return new HierarchicalDataWebControlActionList(this, DataSourceDesigner);
		}
	}
	public class HierarchicalDataWebControlActionList : DataWebControlActionListBase {
		[TypeConverter(typeof(HierarchicalDataSourceIDConverter))]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set { base.DataSourceID = value; }
		}
		public HierarchicalDataWebControlActionList(ASPxHierarchicalDataWebControlDesigner designer, IHierarchicalDataSourceDesigner dataSourceDesigner)
			: base(designer, dataSourceDesigner) {
		}
	}
	public class ASPxDataWebControlDesigner : ASPxDataWebControlDesignerBase, IDataSourceProvider {
		protected static Type[] ColumnTypesRequireHttpHandlerRegistration = { typeof(GridViewDataBinaryImageColumn), typeof(CardViewBinaryImageColumn), typeof(TreeListBinaryImageColumn) };
		private IDataSourceDesigner fDataSourceDesigner = null;
		private ASPxDataWebControl fDataControl = null;
		public string DataMember {
			get { return fDataControl.DataMember; }
			set {
				fDataControl.DataMember = value;
				OnDataSourceChanged(true);
			}
		}
		public IDataSourceDesigner DataSourceDesigner {
			get { return fDataSourceDesigner; }
		}
		public DesignerDataSourceView DesignerView {
			get {
				DesignerDataSourceView view = null;
				if (DataSourceDesigner != null) {
					view = DataSourceDesigner.GetView(DataMember);
					if (view == null && string.IsNullOrEmpty(DataMember)) {
						string[] views = DataSourceDesigner.GetViewNames();
						if (views != null && views.Length > 0)
							view = DataSourceDesigner.GetView(views[0]);
					}
				}
				return view;
			}
		}
		protected virtual int SampleRowCount {
			get { return 5; }
		}		
		public override void Initialize(IComponent component) {
			fDataControl = (ASPxDataWebControl)component;
			base.Initialize(component);
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				DisconnectFromDataSource();
			base.Dispose(disposing);
		}
		protected override bool ConnectToDataSource() {
			IDataSourceDesigner designer = GetDataSourceDesigner();
			if (fDataSourceDesigner == designer)
				return false;
			if (fDataSourceDesigner != null) {
				fDataSourceDesigner.DataSourceChanged -= new EventHandler(OnDataSourceChanged);
				fDataSourceDesigner.SchemaRefreshed -= new EventHandler(OnSchemaRefreshed);
			}
			fDataSourceDesigner = designer;
			if (fDataSourceDesigner != null) {
				fDataSourceDesigner.DataSourceChanged += new EventHandler(OnDataSourceChanged);
				fDataSourceDesigner.SchemaRefreshed += new EventHandler(OnSchemaRefreshed);
			}
			return true;
		}
		protected override void CreateDataSource(string propertyName) {
			ControlDesigner.InvokeTransactedChange(Component, new TransactedChangeCallback(CreateDataSourceCallback), propertyName, StringResources.DataControl_CreateDataSourceTransaction);
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			IEnumerable enumerable = GetDesignTimeDataSource();
			string dataSourceID = dataControl.DataSourceID;
			object dataSource = dataControl.DataSource;
			dataControl.DataContainer.IgnoreDataSourceID = true;
			dataControl.DataSource = enumerable;
			try {
				if (enumerable != null)
					dataControl.DataBind();
			}
			finally {
				dataControl.DataContainer.IgnoreDataSourceID = false;
				dataControl.DataSource = dataSource;
			}
		}
		protected override void DisconnectFromDataSource() {
			if (fDataSourceDesigner != null) {
				fDataSourceDesigner.DataSourceChanged -= new EventHandler(OnDataSourceChanged);
				fDataSourceDesigner.SchemaRefreshed -= new EventHandler(OnSchemaRefreshed);
				fDataSourceDesigner = null;
			}
		}
		private IDataSourceDesigner GetDataSourceDesigner() {
			IDataSourceDesigner designer = null;
			if (!string.IsNullOrEmpty(DataSourceID)) {
				Control control = FindControl(Component.Site, (Control)base.Component, DataSourceID);
				if (control != null && DesignerHost != null)
					designer = DesignerHost.GetDesigner(control) as IDataSourceDesigner;
			}
			return designer;
		}
		protected override object GetDesignerViewSchema() {
			if (DesignerView == null)
				return null;
			if (DesignerView.Schema == null && DataSourceDesigner != null)
				try {
					DataSourceDesigner.RefreshSchema(true);
				}
#pragma warning disable 168
				catch(NotSupportedException e) { }
#pragma warning restore 168
			return DesignerView.Schema;				
		}
		protected virtual IEnumerable GetDesignTimeDataSource() {
			bool dummy;
			IEnumerable enumerable = null;
			if (DesignerView != null)
				enumerable = DesignerView.GetDesignTimeData(SampleRowCount, out dummy);
			else {
				IEnumerable resolvedEnumerable = ((IDataSourceProvider)this).GetResolvedSelectedDataSource();
				if (resolvedEnumerable != null) {
					DataTable table = DesignTimeData.CreateSampleDataTable(resolvedEnumerable);
					enumerable = DesignTimeData.GetDesignTimeDataSource(table, SampleRowCount);
				}
			}
			if (enumerable != null) {
				ICollection collection = enumerable as ICollection;
				if (collection == null || collection.Count > 0)
					return enumerable;
			}
			return GetSampleDataSource();
		}
		protected virtual IEnumerable GetSampleDataSource() {
			DataTable table = !string.IsNullOrEmpty(fDataControl.DataSourceID) ? DesignTimeData.CreateDummyDataBoundDataTable() : DesignTimeData.CreateDummyDataTable();
			return DesignTimeData.GetDesignTimeDataSource(table, SampleRowCount);
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PerformPrefilterProperty(properties, "DataMember", typeof(DataMemberConverter));
			PerformPrefilterProperty(properties, "DataSource", typeof(DataSourceConverter));
			PerformPrefilterProperty(properties, "DataSourceID", typeof(DataSourceIDConverter));
		}
		private bool CreateDataSourceCallback(object context) {
			string id;
			string propertyName = (string)context;
			System.Windows.Forms.DialogResult result1 = ShowCreateDataSourceDialog(this, typeof(IDataSource), true, out id);
			if (!string.IsNullOrEmpty(id))
				SetDataSourceIDCore(propertyName, id);
			return (result1 == System.Windows.Forms.DialogResult.OK);
		}
		private void OnDataSourceChanged(object sender, EventArgs e) {
			OnDataSourceChanged(true);
		}
		private void OnSchemaRefreshed(object sender, EventArgs e) {
			OnSchemaRefreshed();
		}
		protected override IDataSourceViewSchema GetSchema() {
			if (DesignerView != null)
				return DesignerView.Schema;
			else
				return null;
		}
		protected override bool CanRefreshSchema() {
			if (DataSourceDesigner != null)
				return DataSourceDesigner.CanRefreshSchema;
			else
				return false;
		}
		protected override void RefreshSchema(bool preferSilent) {
			if (DataSourceDesigner != null)
				DataSourceDesigner.RefreshSchema(preferSilent);
		}
		IEnumerable IDataSourceProvider.GetResolvedSelectedDataSource() {
			DataBinding binding = DataBindings["DataSource"];
			if (binding != null)
				return DesignTimeData.GetSelectedDataSource(base.Component, binding.Expression, DataMember);
			return null;
		}
		object IDataSourceProvider.GetSelectedDataSource() {
			DataBinding binding = DataBindings["DataSource"];
			if (binding != null)
				return DesignTimeData.GetSelectedDataSource(base.Component, binding.Expression);
			return null;
		}
		protected override DataWebControlActionListBase CreateDataActionList() {
			return new DataWebControlActionList(this, DataSourceDesigner);
		}
	}
	public class DataWebControlActionList : DataWebControlActionListBase {
		[TypeConverter(typeof(DataSourceIDConverter))]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set {
				ControlDesigner.InvokeTransactedChange(Designer.Component, new TransactedChangeCallback(SetDataSourceIDCallback), value, StringResources.DataControl_SetDataSourceIDTransaction);
			}
		}
		public DataWebControlActionList(ASPxDataWebControlDesigner designer, IDataSourceDesigner dataSourceDesigner)
			: base(designer, dataSourceDesigner) {
		}
		private bool SetDataSourceIDCallback(object context) {
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(Designer.Component)["DataSourceID"];
			descriptor.SetValue(Designer.Component, context);
			return true;
		}
	}
}
