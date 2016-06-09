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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
namespace DevExpress.Web.DemoUtils {
	[ToolboxItem(false)]
	public class DemoDataSource : SqlDataSource {
		string[] _viewNames;
		DemoDataSourceView _view;
		string sessionKey;
		static bool? _isSiteMode;
		public DemoDataSource() { }
		[DefaultValue(""), Themeable(false), Localizable(false)]
		public string DataSourceID {
			get { return ViewStateUtils.GetStringProperty(ViewState, "DataSourceID", string.Empty); }
			set { ViewStateUtils.SetStringProperty(ViewState, "DataSourceID", string.Empty, value); }
		}
		[DefaultValue(""), Themeable(false), Localizable(false)]
		public string DataMember {
			get { return ViewStateUtils.GetStringProperty(ViewState, "DataMember", string.Empty); }
			set { ViewStateUtils.SetStringProperty(ViewState, "DataMember", string.Empty, value); }
		}
		[DefaultValue(""), Themeable(false), Localizable(false)]
		public string IdentityKey {
			get { return ViewStateUtils.GetStringProperty(ViewState, "IdentityKey", string.Empty); }
			set { ViewStateUtils.SetStringProperty(ViewState, "IdentityKey", string.Empty, value); }
		}
		[DefaultValue(true), Bindable(true)]
		public DefaultBoolean IsSiteMode {
			get { return ViewStateUtils.GetDefaultBooleanProperty(ViewState, "IsSiteMode", DefaultBoolean.Default); }
			set { ViewStateUtils.SetDefaultBooleanProperty(ViewState, "IsSiteMode", DefaultBoolean.Default, value); }
		}
		internal string SessionKey {
			get {
				if(string.IsNullOrEmpty(sessionKey))
					sessionKey = string.Format("{0}-{1}", UniqueID, Page.Request.PhysicalPath);
				return sessionKey;
			}
		}
		protected internal virtual DataTable InMemoryDataTable {
			get { return Page.Session[SessionKey] as DataTable; }
			set { Page.Session[SessionKey] = value; }
		}
		internal new bool DesignMode {
			get { return base.DesignMode; }
		}
		protected override DataSourceView GetView(string viewName) {
			if(this._view == null)
				this._view = new DemoDataSourceView(this, "DefaultView", this.Context);
			return this._view;
		}
		protected override ICollection GetViewNames() {
			if(_viewNames == null)
				this._viewNames = new string[] { "DefaultView" };
			return this._viewNames;
		}
		protected internal virtual bool GetIsSiteMode() {
			if(IsSiteMode == DefaultBoolean.Default) {
				if(!_isSiteMode.HasValue) {
					var appSettingsValue = ConfigurationManager.AppSettings["SiteMode"];
					_isSiteMode = appSettingsValue == null || appSettingsValue.Equals("true", StringComparison.InvariantCultureIgnoreCase);
				}
				return _isSiteMode.Value;
			}
			return IsSiteMode == DefaultBoolean.True;
		}
		#region Find Underlying DataSource
		IDataSource underlyingDataSource;
		DataSourceView underlyingView;
		protected internal virtual DataSourceView GetUnderlyingView() {
			if(this.underlyingView == null || DesignMode) {
				this.underlyingDataSource = GetUnderlyingDataSource();
				DataSourceView view = this.underlyingDataSource.GetView(DataMember ?? string.Empty);
				if(view == null)
					throw new InvalidOperationException(string.Format(StringResources.DataControl_ViewNotFound, ID));
				this.underlyingView = view;
			}
			return this.underlyingView;
		}
		IDataSource GetUnderlyingDataSource() {
			if(!DesignMode && this.underlyingDataSource != null)
				return this.underlyingDataSource;
			IDataSource dataSource = null;
			if(!String.IsNullOrEmpty(DataSourceID)) {
				Control dataSourceControl = DataControlHelper.FindControl(this, DataSourceID);
				if(dataSourceControl == null)
					throw new HttpException(string.Format(StringResources.DataControl_DataSourceDoesNotExist, ID, DataSourceID));
				dataSource = dataSourceControl as IDataSource;
				if(dataSource == null)
					throw new HttpException(string.Format(StringResources.DataControl_DataSourceIDMustBeDataControl, ID, DataSourceID));
			}
			return dataSource;
		}
		#endregion
	}
	public class DemoDataSourceView : SqlDataSourceView {
		public DemoDataSourceView(DemoDataSource owner, string viewName, HttpContext context)
			: base(owner, viewName, context) {
			this.Owner = owner;
			var underlyingView = Owner.GetUnderlyingView();
			var sqlUnderlyingView = underlyingView as SqlDataSourceView;
			if(sqlUnderlyingView != null) {
				foreach(Parameter param in sqlUnderlyingView.UpdateParameters) {
					UpdateParameters.Add(param);
				}
				foreach(Parameter param in sqlUnderlyingView.DeleteParameters) {
					DeleteParameters.Add(param);
				}
				foreach(Parameter param in sqlUnderlyingView.InsertParameters) {
					InsertParameters.Add(param);
				}
			}
		}
		protected DemoDataSource Owner { get; private set; }
		protected DataTable InMemoryDataTable {
			get {
				if(Owner.InMemoryDataTable == null) {
					Owner.GetUnderlyingView().Select(DataSourceSelectArguments.Empty, CreateInMemoryDataTable);
				}
				return Owner.InMemoryDataTable;
			}
			set { Owner.InMemoryDataTable = value; }
		}
		private void CreateInMemoryDataTable(IEnumerable data) {
			var dataView = data as DataView;
			bool dataStoredAsTable = dataView != null;
			if(dataStoredAsTable)
				CreateInMemoryDataTableFromTable(dataView);
			else
				CreateInMemmoryDataTableFromList(data);
		}
		protected void CreateInMemoryDataTableFromTable(DataView dataView) {
			InMemoryDataTable = dataView.Table.Copy();
		}
		protected void CreateInMemmoryDataTableFromList(IEnumerable data) {
			Owner.InMemoryDataTable = new DataTable();
			foreach(var item in data) {
				DataRow row = CreateInMemoryDataRow(Owner.InMemoryDataTable, item);
				Owner.InMemoryDataTable.Rows.Add(row);
			}
		}
		private DataRow CreateInMemoryDataRow(DataTable table, object item) {
			var properties = ReflectionUtils.GetProperties(item);
			DataRow row = table.NewRow();
			foreach(PropertyDescriptor property in properties) {
				EnsureColumnExist(table, property);
				var value = property.GetValue(item);
				row[property.Name] = value ?? DBNull.Value;
			}
			return row;
		}
		private void EnsureColumnExist(DataTable table, PropertyDescriptor property) {
			if(table.Columns[property.Name] == null) {
				Type propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
				table.Columns.Add(property.Name, propertyType);
			}
		}
		protected bool DirectMode {
			get { return Owner.DesignMode || !Owner.GetIsSiteMode(); }
		}
		protected string IdentityKey {
			get {
				var key = Owner.IdentityKey;
				if(!string.IsNullOrEmpty(key) && !InMemoryDataTable.Columns[key].DataType.Equals(typeof(Int32)))
					throw new ArgumentException("Identity column should have the Int32 type");
				return key;
			}
		}
		protected object GenerateIdentityValue() {
			var rows = InMemoryDataTable.Select();
			if(rows.Count() == 0)
				return 0;
			return rows.Max(row => Convert.ToInt32(row[IdentityKey])) + 1;
		}
		public override void Select(DataSourceSelectArguments arguments, DataSourceViewSelectCallback callback) {
			if(DirectMode)
				Owner.GetUnderlyingView().Select(arguments, callback);
			else
				callback(ExecuteSelect(arguments));
		}
		protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments) {
			DataView data = new DataView(InMemoryDataTable);
			if(!string.IsNullOrEmpty(arguments.SortExpression))
				data.Sort = arguments.SortExpression;
			return data;
		}
		public override void Insert(IDictionary values, DataSourceViewOperationCallback callback) {
			if(DirectMode)
				Owner.GetUnderlyingView().Insert(values, callback);
			else
				ExecuteOperation(callback, () => this.ExecuteInsert(values));
		}
		protected override int ExecuteInsert(IDictionary values) {
			var row = InMemoryDataTable.NewRow();
			foreach(DictionaryEntry val in values) {
				row[val.Key.ToString()] = val.Value != null ? val.Value : DBNull.Value;
			}
			if(!string.IsNullOrEmpty(IdentityKey))
				row[IdentityKey] = GenerateIdentityValue();
			InMemoryDataTable.Rows.Add(row);
			InMemoryDataTable.AcceptChanges();
			return 1;
		}
		public override void Update(IDictionary keys, IDictionary values, IDictionary oldValues, DataSourceViewOperationCallback callback) {
			if(DirectMode)
				Owner.GetUnderlyingView().Update(keys, values, oldValues, callback);
			else
				ExecuteOperation(callback, () => this.ExecuteUpdate(keys, values, oldValues));
		}
		protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues) {
			var affectedRowCount = 0;
			foreach(DataRow row in InMemoryDataTable.Select().Where(row => CompareRow(row, keys))) {
				affectedRowCount++;
				foreach(DictionaryEntry val in values) {
					row[val.Key.ToString()] = val.Value != null ? val.Value : DBNull.Value;
				}
			}
			InMemoryDataTable.AcceptChanges();
			return affectedRowCount;
		}
		public override void Delete(IDictionary keys, IDictionary oldValues, DataSourceViewOperationCallback callback) {
			if(DirectMode)
				Owner.GetUnderlyingView().Delete(keys, oldValues, callback);
			else
				ExecuteOperation(callback, () => this.ExecuteDelete(keys, oldValues));
		}
		protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues) {
			var affectedRowCount = 0;
			foreach(DataRow row in InMemoryDataTable.Select().Where(row => CompareRow(row, keys))) {
				affectedRowCount++;
				row.Delete();
			}
			InMemoryDataTable.AcceptChanges();
			return affectedRowCount;
		}
		public override bool CanDelete { get { return true; } }
		public override bool CanUpdate { get { return true; } }
		public override bool CanInsert { get { return true; } }
		void ExecuteOperation(DataSourceViewOperationCallback callback, Func<int> operation) {
			int affectedRecords = 0;
			try {
				affectedRecords = operation();
				callback(affectedRecords, null);
			}
			catch(Exception exception) {
				if(!callback(affectedRecords, exception))
					throw;
			}
		}
		bool CompareRow(DataRow row, IDictionary keys) {
			foreach(DictionaryEntry key in keys) {
				if(!CompareValues(row[key.Key.ToString()], key.Value))
					return false;
			}
			return true;
		}
		bool CompareValues(object val1, object val2) {
			return (val1 != null) ? val1.Equals(val2) : (val1 == val2);
		}
	}
}
