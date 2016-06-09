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
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	[Designer("DevExpress.Web.Design.ASPxDataWebControlDesignerBase, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	public abstract class ASPxDataWebControlBase : ASPxWebControl, IDataSourceViewSchemaAccessor {
		protected const string DefaultDataHelperName = "DataSource";
		private DataContainer dataContainer;
		private IDataSourceViewSchemaAccessor fDataSourceViewSchemaAccessor = null;
		private static readonly object EventDataBound = new object();
		public ASPxDataWebControlBase()
			: this(null) {
		}
		protected ASPxDataWebControlBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
			this.dataContainer = new DataContainer(this);
			RegisterDataHelpers();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataWebControlBaseDataSource"),
#endif
		Bindable(false), Category("Data"), Themeable(false), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public virtual object DataSource {
			get { return DataContainer[DefaultDataHelperName].DataSource; }
			set { DataContainer[DefaultDataHelperName].DataSource = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataWebControlBaseDataSourceID"),
#endif
		DefaultValue(""), Themeable(false), Category("Data"), AutoFormatDisable, Localizable(false)]
		public virtual string DataSourceID {
			get { return DataContainer[DefaultDataHelperName].DataSourceID; }
			set { DataContainer[DefaultDataHelperName].DataSourceID = value; }
		}
		protected internal DataContainer DataContainer {
			get { return dataContainer; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Bound {
			get { return DataContainer.Bound; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataWebControlBaseDataBound"),
#endif
		Category("Data")]
		public event EventHandler DataBound
		{
			add
			{
				Events.AddHandler(EventDataBound, value);
			}
			remove
			{
				Events.RemoveHandler(EventDataBound, value);
			}
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			DataContainer.Init();
		}
		protected override void OnLoad(EventArgs e) {
			DataContainer.Load();
			base.OnLoad(e);
		}
		protected override void OnPreRender(EventArgs e) {
			DataContainer.EnsureDataBound();
			base.OnPreRender(e);
		}
		protected override void BeforeRender() {
			if(!PreRendered && !DesignMode)
				DataContainer.EnsureDataBound();
			base.BeforeRender();
		}
		protected override void DataBindInternal() {
			if(DesignMode) {
				IDictionary designModeState = GetDesignModeState();
				if(((designModeState == null) || (designModeState["EnableDesignTimeDataBinding"] == null)) && (Site == null)) {
					if(IsDesignTimeDataBindingRequired())
						OnDataBinding(EventArgs.Empty); 
					return; 
				}
			}
			DataBindCore();
			EnsureChildControls();
			DataBindContainers(this, true, true);
		}
		protected virtual void DataBindCore() {
			DataContainer.PerformSelect();
		}
		protected virtual bool IsDesignTimeDataBindingRequired() {
			return false;
		}
		protected virtual void RegisterDataHelpers() {
			DataContainer.RegisterDataHelper(CreateDataHelper(DefaultDataHelperName));
		}
		protected abstract DataHelperBase CreateDataHelper(string name);
		protected internal StateBag GetViewState() {
			return ViewState;
		}
		protected internal virtual bool HasDataInViewState() {
			return IsViewStateEnabled;
		}
		protected void EnsureDataBound(string dataHelperName) {
			DataContainer[dataHelperName].EnsureDataBound();
		}
		protected void EnsureDataBound() {
			DataContainer.EnsureDataBound();
		}
		protected virtual void PerformSelect(string dataHelperName) {
			DataContainer[dataHelperName].PerformSelect();
		}
		protected void PerformSelect() {
			PerformSelect(DefaultDataHelperName);
		}
		protected bool IsBoundUsingDataSourceID(string dataHelperName) {
			return DataContainer[dataHelperName].IsBoundUsingDataSourceID;
		}
		protected bool IsBoundUsingDataSourceID() {
			return IsBoundUsingDataSourceID(DefaultDataHelperName);
		}
		protected bool RequireDataBinding(string dataHelperName) {
			return DataContainer[dataHelperName].RequiresDataBinding = true;
		}
		protected bool RequireDataBinding() {
			return RequireDataBinding(DefaultDataHelperName);
		}
		protected virtual void OnDataPropertyChanged() {
			OnDataPropertyChanged(DefaultDataHelperName);
		}
		protected virtual void OnDataPropertyChanged(string dataHelperName) {
			DataContainer[dataHelperName].OnDataPropertyChanged();
		}
		protected void OnDataFieldChanged() {
			OnDataFieldChanged(DefaultDataHelperName);
		}
		protected virtual void OnDataFieldChanged(string dataHelperName) {
			DataContainer[dataHelperName].OnDataFieldChanged();
		}
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema {
			get { return GetDataSourceViewSchemaAccessor(); }
			set { }
		}
		protected virtual object GetDataSourceViewSchemaAccessor() {
			return fDataSourceViewSchemaAccessor != null ? fDataSourceViewSchemaAccessor.DataSourceViewSchema : null;
		}
		protected internal void SetDataSourceViewSchemaAccessor(IDataSourceViewSchemaAccessor accessor) {
			fDataSourceViewSchemaAccessor = accessor;
		}
		protected internal void RaiseDataBinding() {
			OnDataBinding(EventArgs.Empty);
		}
		protected internal void RaiseDataBound() {
			OnDataBound(EventArgs.Empty);
		}
		protected virtual void OnDataBound(EventArgs e) {
			EventHandler handler = Events[EventDataBound] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
	}
	[Designer("DevExpress.Web.Design.ASPxHierarchicalDataWebControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	public abstract class ASPxHierarchicalDataWebControl : ASPxDataWebControlBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHierarchicalDataWebControlDataSourceID"),
#endif
		IDReferenceProperty(typeof(HierarchicalDataSourceControl))]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set { base.DataSourceID = value; }
		}
		public ASPxHierarchicalDataWebControl()
			: this(null) {
		}
		protected ASPxHierarchicalDataWebControl(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		protected HierarchicalDataSourceView GetData(string name, string viewPath) {
			HierarchicalDataHelper helper = (HierarchicalDataHelper)DataContainer[name];
			return helper.GetData(viewPath);
		}
		protected HierarchicalDataSourceView GetData(string viewPath) {
			return GetData(DefaultDataHelperName, viewPath);
		}
		protected IHierarchicalDataSource GetDataSource(string name) {
			HierarchicalDataHelper helper = (HierarchicalDataHelper)DataContainer[name];
			return helper.GetDataSource();
		}
		protected IHierarchicalDataSource GetDataSource() {
			return GetDataSource(DefaultDataHelperName);
		}
		protected internal virtual void PerformDataBinding(string dataHelperName) {
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new HierarchicalDataHelper(this, name);
		}
		[System.Security.SecuritySafeCritical]
		protected override object GetDataSourceViewSchemaAccessor() {
			object accessor = base.GetDataSourceViewSchemaAccessor();
			if(accessor is System.Web.UI.Design.XmlDocumentSchema){
				System.Web.UI.Design.IDataSourceViewSchema[] schemas = ((System.Web.UI.Design.XmlDocumentSchema)accessor).GetViews();
				return (schemas.Length > 0) ? schemas[0] : null;
			}
			if(accessor is System.Web.UI.Design.IDataSourceSchema) {
				System.Web.UI.Design.IDataSourceViewSchema[] schemas = ((System.Web.UI.Design.IDataSourceSchema)accessor).GetViews();
				return (schemas.Length > 0) ? schemas[0] : null;
			}
			return accessor;
		}
	}
	[Designer("DevExpress.Web.Design.ASPxDataWebControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	public abstract class ASPxDataWebControl : ASPxDataWebControlBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataWebControlDataMember"),
#endif
		DefaultValue(""), Themeable(false), Category("Data"), AutoFormatDisable(), Localizable(false)]
		public virtual string DataMember {
			get { return DataContainer[DefaultDataHelperName].DataMember; }
			set { DataContainer[DefaultDataHelperName].DataMember = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataWebControlDataSourceID"),
#endif
		IDReferenceProperty(typeof(DataSourceControl))]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set { base.DataSourceID = value; }
		}
		public ASPxDataWebControl()
			: this(null) {
		}
		protected ASPxDataWebControl(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		protected virtual DataSourceView GetData(string name) {
			DataHelper helper = (DataHelper)DataContainer[name];
			return helper.GetData();
		}
		protected DataSourceView GetData() {
			return GetData(DefaultDataHelperName);
		}
		protected IDataSource GetDataSource(string name) {
			DataHelper helper = (DataHelper)DataContainer[name];
			return helper.GetDataSource();
		}
		protected IDataSource GetDataSource() {
			return GetDataSource(DefaultDataHelperName);
		}
		protected object GetFieldValue(object dataObject, string fieldName, bool isRequired) {
			return DataUtils.GetFieldValue(dataObject, fieldName, isRequired, DesignMode);
		}
		protected object GetFieldValue(object dataObject, string fieldName, bool isRequired, object defaultValue) {
			return DataUtils.GetFieldValue(dataObject, fieldName, isRequired, DesignMode, defaultValue);
		}
		protected internal virtual void PerformDataBinding(string dataHelperName, IEnumerable data) {
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new DataHelper(this, name);
		}
	}
}
