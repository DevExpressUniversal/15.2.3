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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public class DataContainer {
		private Dictionary<string, DataHelperBase> helpers;
		private bool ignoreDataSourceID = false;
		public DataContainer(ASPxDataWebControlBase control) {
			helpers = new Dictionary<string, DataHelperBase>();
		}
		public DataHelperBase this[string name] {
			get { return helpers[name]; }
		}
		public bool Bound {
			get {
				foreach(DataHelperBase helper in helpers.Values) {
					if(!helper.Bound)
						return false;
				}
				return true;
			}
		}
		protected internal bool IgnoreDataSourceID {
			get { return ignoreDataSourceID; }
			set { ignoreDataSourceID = value; }
		}
		public void RegisterDataHelper(DataHelperBase helper) {
			helper.DataContainer = this;
			helpers.Add(helper.Name, helper);
		}
		public void Init() {
			foreach(DataHelperBase helper in helpers.Values)
				helper.Init();
		}
		public void Load() {
			foreach(DataHelperBase helper in helpers.Values)
				helper.Load();
		}
		public void EnsureDataBound() {
			foreach(DataHelperBase helper in helpers.Values)
				helper.EnsureDataBound();
		}
		public void PerformSelect() {
			foreach(DataHelperBase helper in helpers.Values)
				helper.PerformSelect();
		}
	}
	public abstract class DataHelperBase {
		private DataContainer dataContainer;
		private ASPxDataWebControlBase control;
		private string name;
		private object dataSource = null;
		private bool requiresBindToNull = false;
		private bool requiresDataBinding = false;
		private bool lockDataPropertyChange = false;
		public DataHelperBase(ASPxDataWebControlBase control, string name) {
			this.control = control;
			this.name = name;
		}
		public DataContainer DataContainer {
			private get { return dataContainer; }
			set { dataContainer = value; }
		}
		public object DataSource {
			get { return dataSource; }
			set {
				if(value != null)
					ValidateDataSource(value);
				dataSource = value;
				OnDataPropertyChanged();
			}
		}
		public string DataSourceID {
			get {
				return (this.dataContainer != null && this.dataContainer.IgnoreDataSourceID) ?
					string.Empty : ViewStateUtils.GetStringProperty(ControlViewState, Name + "ID", "");
			}
			set {
				CheckRequireBindToNull(DataSourceID, value);
				ViewStateUtils.SetStringProperty(ControlViewState, Name + "ID", "", value);
				OnDataPropertyChanged();
			}
		}
		public string DataMember {
			get { return ViewStateUtils.GetStringProperty(ControlViewState, Name + "Member", ""); }
			set {
				ViewStateUtils.SetStringProperty(ControlViewState, Name + "Member", "", value);
				OnDataPropertyChanged();
			}
		}
		public ASPxDataWebControlBase Control {
			get { return control; }
		}
		public StateBag ControlViewState {
			get { return Control.GetViewState(); }
		}
		public string Name {
			get { return name; }
		}
		public bool IsBoundUsingDataSourceID {
			get { return !string.IsNullOrEmpty(DataSourceID); }
		}
		public bool RequiresDataBinding {
			get { return requiresDataBinding; }
			set {
				if(value && Control.PreRendered && IsBoundUsingDataSourceID && Control.Page != null &&
					!Control.Page.IsCallback) { 
					requiresDataBinding = true;
					EnsureDataBound();
				} else
					requiresDataBinding = value;
			}
		}
		public bool Bound {
			get { return ViewStateUtils.GetBoolProperty(ControlViewState, Name + "Bound", false); }
			set { ViewStateUtils.SetBoolProperty(ControlViewState, Name + "Bound", false, value); }
		}
		public void Init() {
			if(Control.Page != null && Control.Page.IsPostBack && !Control.HasDataInViewState())
				RequiresDataBinding = true;
		}
		public void Load() {
			ConnectToData();
			if(Control.Page != null && !Bound) {
				if(!Control.Page.IsPostBack)
					RequiresDataBinding = true;
				else if(Control.HasDataInViewState())
					RequiresDataBinding = true;
			}
		}
		protected void EnsureDataBound(bool ensureChildControls) {
			try {
				this.lockDataPropertyChange = true;
				if(CanBindOnEnsureDataBound) {
					if(ensureChildControls)
						Control.DataBind();
					else
						PerformSelect();
					this.requiresBindToNull = false;
				}
			} finally {
				this.lockDataPropertyChange = false;
			}
		}
		protected virtual bool CanBindOnEnsureDataBound {
			get { return RequiresDataBinding && (IsBoundUsingDataSourceID || requiresBindToNull); }
		}
		public void EnsureDataBound() {
			EnsureDataBound(true);
		}
		protected void CheckRequireBindToNull(string oldValue, string newValue) {
			if(string.IsNullOrEmpty(newValue) && !string.IsNullOrEmpty(oldValue))
				this.requiresBindToNull = true;
		}
		protected internal virtual void OnDataPropertyChanged() {
			if(this.lockDataPropertyChange)
				throw new HttpException(string.Format(StringResources.DataControl_InvalidDataPropertyChange, Control.ID)); 
			if(!Control.IsLoading())
				RequiresDataBinding = true;
		}
		protected internal virtual void OnDataFieldChanged() {
			if(!Control.IsLoading())
				RequiresDataBinding = true;
		}
		protected abstract void ConnectToData();
		public abstract void PerformSelect();
		protected virtual void ValidateDataSource(object dataSource) {
		}
	}
	public class HierarchicalDataHelper : DataHelperBase {
		private bool currentDataSourceIsFromControl = false;
		private bool currentDataSourceValid = false;
		private IHierarchicalDataSource currentHierarchicalDataSource = null;
		public HierarchicalDataHelper(ASPxHierarchicalDataWebControl control, string name)
			: base(control, name) {
		}
		public new ASPxHierarchicalDataWebControl Control {
			get { return (ASPxHierarchicalDataWebControl)base.Control; }
		}
		protected internal virtual HierarchicalDataSourceView GetData(string viewPath) {
			IHierarchicalDataSource source = ConnectToHierarchicalDataSource();
			HierarchicalDataSourceView view = source.GetHierarchicalView(viewPath);
			if(view == null)
				throw new InvalidOperationException(string.Format(StringResources.HierarchicalDataControl_ViewNotFound, Control.ID)); 
			return view;
		}
		protected internal virtual IHierarchicalDataSource GetDataSource() {
			if(!Control.DesignMode && this.currentDataSourceValid && this.currentHierarchicalDataSource != null)
				return this.currentHierarchicalDataSource;
			IHierarchicalDataSource dataSource = null;
			if(IsBoundUsingDataSourceID) {
				Control dataSourceControl = DataControlHelper.FindControl(Control, DataSourceID);
				if(dataSourceControl == null)
					throw new HttpException(string.Format(StringResources.DataControl_DataSourceDoesNotExist, Control.ID, DataSourceID));
				dataSource = GetValidDataSourceControl(dataSourceControl);
			}
			return dataSource;
		}
		protected override void ConnectToData() {
			ConnectToHierarchicalDataSource();
		}
		protected virtual void PerformDataBinding() {
			Control.PerformDataBinding(Name);
		}
		public override void PerformSelect() {
			Control.RaiseDataBinding();
			PerformDataBinding();
			RequiresDataBinding = false;
			Bound = true;
			Control.RaiseDataBound();
		}
		protected override void ValidateDataSource(object dataSource) {
			if(dataSource != null && !(dataSource is IHierarchicalEnumerable) && !(dataSource is IHierarchicalDataSource))
				throw new InvalidOperationException(StringResources.HierarchicalDataControl_InvalidDataSourceType);
		}
		protected virtual IHierarchicalDataSource GetValidDataSourceControl(Control dataSourceControl) {
			IHierarchicalDataSource dataSource = dataSourceControl as IHierarchicalDataSource;
			if(dataSource == null)
				throw new HttpException(string.Format(StringResources.HierarchicalDataControl_DataSourceIDMustBeHierarchicalDataControl, Control.ID, DataSourceID));
			return dataSource;
		}
		protected virtual void OnDataSourceChanged(object sender, EventArgs e) {
			RequiresDataBinding = true;
		}
		protected internal override void OnDataPropertyChanged() {
			this.currentDataSourceValid = false;
			base.OnDataPropertyChanged();
		}
		private IHierarchicalDataSource ConnectToHierarchicalDataSource() {
			if(this.currentDataSourceValid && !Control.DesignMode) {
				if(!this.currentDataSourceIsFromControl && IsBoundUsingDataSourceID)
					throw new InvalidOperationException(string.Format(StringResources.DataControl_MultipleDataSources, Control.ID));
				return currentHierarchicalDataSource;
			}
			if(this.currentHierarchicalDataSource != null && this.currentDataSourceIsFromControl)
				this.currentHierarchicalDataSource.DataSourceChanged -= new EventHandler(OnDataSourceChanged);
			this.currentHierarchicalDataSource = GetDataSource();
			this.currentDataSourceIsFromControl = IsBoundUsingDataSourceID;
			if(this.currentHierarchicalDataSource == null)
				this.currentHierarchicalDataSource = new ReadOnlyHierarchicalDataSource(DataSource);
			else if(DataSource != null)
				throw new InvalidOperationException(string.Format(StringResources.DataControl_MultipleDataSources, Control.ID));
			this.currentDataSourceValid = true;
			if(this.currentHierarchicalDataSource != null && this.currentDataSourceIsFromControl)
				this.currentHierarchicalDataSource.DataSourceChanged += new EventHandler(OnDataSourceChanged);
			return this.currentHierarchicalDataSource;
		}
	}
	public class DataHelper : DataHelperBase {
		DataHelperCore coreHelper;
		private DataSourceSelectArguments selectArguments = null;
		private bool ignoreDataSourceViewChanged = false;
		private bool canServerPaging = false;
		public DataHelper(ASPxDataWebControl control, string name)
			: base(control, name) {
			this.coreHelper = new DataHelperCore(this);
		}
		protected void ResetSelectArguments() {
			this.selectArguments = null;
		}
		protected DataHelperCore CoreHelper {
			get { return coreHelper; }
		}
		public virtual bool CanBindToSingleObject {
			get { return false; }
		}
		public new ASPxDataWebControl Control {
			get { return (ASPxDataWebControl)base.Control; }
		}
		public virtual bool CanServerPaging {
			get { return canServerPaging; }
		}
		protected DataSourceSelectArguments SelectArguments {
			get {
				if(selectArguments == null) selectArguments = CreateDataSourceSelectArguments();
				return selectArguments;
			}
		}
		protected override void ConnectToData() {
			CoreHelper.GetView();
		}
		protected virtual DataSourceSelectArguments CreateDataSourceSelectArguments() {
			return DataSourceSelectArguments.Empty;
		}
		protected internal virtual DataSourceView GetData() {
			return CoreHelper.GetView();
		}
		protected internal virtual IDataSource GetDataSource() {
			return CoreHelper.GetDataSource();
		}
		protected virtual void PerformDataBinding(IEnumerable data) {
			Control.PerformDataBinding(Name, data);
		}
		public override void PerformSelect() {
			if(!IsBoundUsingDataSourceID)
				Control.RaiseDataBinding();
			DataSourceView view = GetData();
			this.ignoreDataSourceViewChanged = true;
			this.canServerPaging = view.CanPage;
			RequiresDataBinding = false;
			Bound = true;
			view.Select(SelectArguments, new DataSourceViewSelectCallback(OnDataSourceViewSelectCallback));
		}
		protected virtual void PerformSelectCore(DataSourceSelectArguments select, DataSourceViewSelectCallback callBack) {
			if(callBack == null) callBack = new DataSourceViewSelectCallback(OnEmptyDataSourceViewSelectCallback);
			this.ignoreDataSourceViewChanged = true;
			try {
				DataSourceView view = GetData();
				view.Select(select, callBack);
			} finally {
				this.ignoreDataSourceViewChanged = false;
			}
		}
		void OnEmptyDataSourceViewSelectCallback(IEnumerable data) { }
		protected override void ValidateDataSource(object dataSource) {
			if(dataSource != null && !(dataSource is IListSource) && !(dataSource is IEnumerable) && !(dataSource is IDataSource))
				throw new InvalidOperationException(StringResources.DataControl_InvalidDataSourceType);
		}
		internal void DataSourceViewChanged() {
			if(!this.ignoreDataSourceViewChanged)
				OnDataSourceViewChanged();
		}
		protected virtual void OnDataSourceViewChanged() {
			RequiresDataBinding = true;
		}
		protected internal override void OnDataPropertyChanged() {
			CoreHelper.Invalidate();
			base.OnDataPropertyChanged();
		}
		protected void OnDataSourceViewSelectCallback(IEnumerable data) {
			this.ignoreDataSourceViewChanged = false;
			if(IsBoundUsingDataSourceID)
				Control.RaiseDataBinding();
			PerformDataBinding(data);
			Control.RaiseDataBound();
		}
	}
	public sealed class DataHelperCore {
		#region Adapters
		abstract class Adapter {
			public abstract ASPxWebControl Control { get; }
			public abstract string DataSourceID { get; }
			public abstract object DataSource { get; }
			public abstract string DataMember { get; }
			public virtual void DataSourceViewChanged() {
			}
		}
		class SimpleAdapter : Adapter {
			ASPxWebControl control;
			string dataSourceID;
			object dataSource;
			string dataMember;
			public SimpleAdapter(ASPxWebControl control, object dataSource, string dataSourceID, string dataMember) {
				this.control = control;
				this.dataSourceID = dataSourceID;
				this.dataSource = dataSource;
				this.dataMember = dataMember;
			}
			public override ASPxWebControl Control { get { return control; } }
			public override string DataSourceID { get { return dataSourceID; } }
			public override object DataSource { get { return dataSource; } }
			public override string DataMember { get { return dataMember; } }
		}
		class DataHelperAdapter : Adapter {
			DataHelper helper;
			public DataHelperAdapter(DataHelper helper) {
				this.helper = helper;
			}
			public override ASPxWebControl Control { get { return helper.Control; } }
			public override string DataSourceID { get { return helper.DataSourceID; } }
			public override object DataSource { get { return helper.DataSource; } }
			public override string DataMember { get { return helper.DataMember; } }
			public override void DataSourceViewChanged() {
				helper.DataSourceViewChanged();
			}
		}
		class SingleObjectDataHelperAdapter : DataHelperAdapter {
			public SingleObjectDataHelperAdapter(DataHelper helper) : base(helper) { }
			public override object DataSource {
				get {
					if (base.DataSource != null && !(base.DataSource is IListSource) && !(base.DataSource is IEnumerable) && !(base.DataSource is IDataSource))
						return new object[] { base.DataSource };
					return base.DataSource;
				}
			}
		}
		#endregion
		Adapter adapter;
		IDataSource currentDataSource = null;
		DataSourceView currentView = null;
		bool currentViewIsFromDataSourceID = false;
		bool currentDataSourceValid = false;
		bool currentViewValid = false;
		public DataHelperCore(DataHelper helper)
			: this(CreateDataHelperAdapter(helper)) {
		}
		public DataHelperCore(ASPxWebControl control, object dataSource, string dataSourceID, string dataMember)
			: this(new SimpleAdapter(control, dataSource, dataSourceID, dataMember)) {
		}
		DataHelperCore(Adapter adapter) {
			this.adapter = adapter;
		}
		ASPxWebControl Control { get { return adapter.Control; } }
		string DataSourceID { get { return adapter.DataSourceID; } }
		object DataSource { get { return adapter.DataSource; } }
		string DataMember { get { return adapter.DataMember; } }
		private static DataHelperAdapter CreateDataHelperAdapter(DataHelper helper) {
			if (helper.CanBindToSingleObject)
				return new SingleObjectDataHelperAdapter(helper);
			return new DataHelperAdapter(helper);
		}
		public void Invalidate() {
			this.currentViewValid = false;
			this.currentDataSourceValid = false;
		}
		public DataSourceView GetView() {
			if(!this.currentViewValid || Control.DesignMode) {
				if(this.currentView != null && this.currentViewIsFromDataSourceID)
					this.currentView.DataSourceViewChanged -= OnViewChanged;
				this.currentDataSource = GetDataSource();
				if(this.currentDataSource == null)
					this.currentDataSource = new ReadOnlyDataSource(DataSource, DataMember);
				else if(DataSource != null)
					throw new InvalidOperationException(string.Format(StringResources.DataControl_MultipleDataSources, Control.ID));
				this.currentDataSourceValid = true;
				DataSourceView view = this.currentDataSource.GetView(DataMember);
				if(view == null)
					throw new InvalidOperationException(string.Format(StringResources.DataControl_ViewNotFound, Control.ID));
				this.currentViewIsFromDataSourceID = !String.IsNullOrEmpty(DataSourceID);
				this.currentView = view;
				if(this.currentView != null && this.currentViewIsFromDataSourceID)
					this.currentView.DataSourceViewChanged += OnViewChanged;
				this.currentViewValid = true;
			}
			return this.currentView;
		}
		void OnViewChanged(object s, EventArgs e) {
			this.adapter.DataSourceViewChanged();
		}
		public IDataSource GetDataSource() {
			if(!Control.DesignMode && this.currentDataSourceValid && this.currentDataSource != null)
				return this.currentDataSource;
			IDataSource dataSource = null;
			if(!String.IsNullOrEmpty(DataSourceID)) {
				Control dataSourceControl = DataControlHelper.FindControl(Control, DataSourceID);
				if(dataSourceControl == null)
					throw new HttpException(string.Format(StringResources.DataControl_DataSourceDoesNotExist, Control.ID, DataSourceID));
				dataSource = dataSourceControl as IDataSource;
				if(dataSource == null)
					throw new HttpException(string.Format(StringResources.DataControl_DataSourceIDMustBeDataControl, Control.ID, DataSourceID));
			}
			return dataSource;
		}
		public IEnumerable Select(DataSourceSelectArguments args) {
			IEnumerable result = null;
			GetView().Select(args, delegate(IEnumerable res) {
				result = res;
			});
			return result;
		}
		public IEnumerable Select() {
			return Select(DataSourceSelectArguments.Empty);
		}
	}
	public class HybridDataHelper : DataHelper {
		public HybridDataHelper(ASPxDataWebControl control, string name)
			: base(control, name) {
		}
		protected override void PerformDataBinding(IEnumerable data) {
			IHierarchicalEnumerable hierarchicalData = ExtractHierarchicalData();
			if(hierarchicalData != null)
				data = hierarchicalData;
			base.PerformDataBinding(data);
		}
		public override void PerformSelect() {
			IHierarchicalDataSource hierSource = Control.DataSource as IHierarchicalDataSource;
			if(hierSource != null) {
				Control.RaiseDataBinding();
				RequiresDataBinding = false;
				Bound = true;
				OnDataSourceViewSelectCallback(hierSource.GetHierarchicalView(String.Empty).Select());
			} else {
				base.PerformSelect();
			}
		}
		IHierarchicalEnumerable ExtractHierarchicalData() {
			IHierarchicalDataSource source = GetDataSource() as IHierarchicalDataSource;
			if(source == null)
				return null;
			return source.GetHierarchicalView(String.Empty).Select();
		}
	}
	public class ReadOnlyHierarchicalDataSource : IHierarchicalDataSource {
		private object fDataSource = null;
		event EventHandler IHierarchicalDataSource.DataSourceChanged {
			add { }
			remove { }
		}
		public ReadOnlyHierarchicalDataSource(object dataSource) {
			fDataSource = dataSource;
		}
		HierarchicalDataSourceView IHierarchicalDataSource.GetHierarchicalView(string viewPath) {
			IHierarchicalDataSource dataSource = fDataSource as IHierarchicalDataSource;
			if(dataSource != null)
				return dataSource.GetHierarchicalView(viewPath);
			IHierarchicalEnumerable enumerable = fDataSource as IHierarchicalEnumerable;
			if(enumerable != null && !string.IsNullOrEmpty(viewPath))
				throw new InvalidOperationException(StringResources.ReadOnlyHierarchicalDataSourceView_CantAccessPathInEnumerable);
			return new ReadOnlyHierarchicalDataSourceView(enumerable);
		}
	}
	public class ReadOnlyHierarchicalDataSourceView : HierarchicalDataSourceView {
		private IHierarchicalEnumerable fDataSource = null;
		public ReadOnlyHierarchicalDataSourceView(IHierarchicalEnumerable dataSource) {
			fDataSource = dataSource;
		}
		public override IHierarchicalEnumerable Select() {
			return fDataSource;
		}
	}
	public class ReadOnlyDataSource : IDataSource {
		private string fDataMember;
		private object fDataSource;
		event EventHandler IDataSource.DataSourceChanged {
			add { }
			remove { }
		}
		public ReadOnlyDataSource(object dataSource, string dataMember) {
			fDataSource = dataSource;
			fDataMember = dataMember;
		}
		DataSourceView IDataSource.GetView(string viewName) {
			IDataSource dataSource = fDataSource as IDataSource;
			if(dataSource != null)
				return dataSource.GetView(viewName);
			IEnumerable enumerable = DataSourceHelper.GetResolvedDataSource(fDataSource, fDataMember);
			return new ReadOnlyDataSourceView(this, fDataMember, enumerable);
		}
		ICollection IDataSource.GetViewNames() {
			return new string[0];
		}
	}
	public class ReadOnlyDataSourceView : DataSourceView {
		private IEnumerable fDataSource = null;
		public ReadOnlyDataSourceView(ReadOnlyDataSource owner, string name, IEnumerable dataSource)
			: base(owner, name) {
			fDataSource = dataSource;
		}
		protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments) {
			arguments.RaiseUnsupportedCapabilitiesError(this);
			return fDataSource;
		}
	}
	public class DataSourceHelper {
		public static IEnumerable GetResolvedDataSource(object dataSource, string dataMember) {
			if(dataSource != null) {
				IListSource listSource = dataSource as IListSource;
				if(listSource != null) {
					IList list = listSource.GetList();
					if(!listSource.ContainsListCollection)
						return list;
					if(list is ITypedList) {
						ITypedList typedList = (ITypedList)list;
						PropertyDescriptorCollection collection = typedList.GetItemProperties(new PropertyDescriptor[0]);
						if(collection == null || collection.Count == 0)
							throw new HttpException(StringResources.DataSourceHelper_ListSourceWithoutDataMembers);
						PropertyDescriptor descriptor = string.IsNullOrEmpty(dataMember) ? collection[0] : collection.Find(dataMember, true);
						if(descriptor != null) {
							object obj1 = list[0];
							object obj2 = descriptor.GetValue(obj1);
							if(obj2 is IEnumerable)
								return (IEnumerable)obj2;
						}
						throw new HttpException(string.Format(StringResources.DataSourceHelper_ListSourceMissingDataMember, dataMember));
					}
				}
				if(dataSource is IEnumerable)
					return (IEnumerable)dataSource;
			}
			return null;
		}
	}
	public static class DataControlHelper {
		public static Control FindControl(Control control, string controlID) {
			if(control == control.Page)
				return control.FindControl(controlID);
			Control foundControl = null;
			Control parentControl = control;
			while((foundControl == null) && (parentControl != control.Page)) {
				parentControl = parentControl.NamingContainer;
				if(parentControl == null)
					throw new HttpException(string.Format(StringResources.DataBoundControlHelper_NoNamingContainer, control.GetType().Name, control.ID));
				foundControl = parentControl.FindControl(controlID);
			}
			return foundControl;
		}
	}
}
