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
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Internal;
namespace DevExpress.Data.Linq.Design {
	using System.ComponentModel.Design;
using System.Data;
using System.Globalization;
using System.Web.UI.Design;
using System.Windows.Forms.Design;
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	[SecurityCritical]
	public class LinqServerModeDataSourceDesigner : DataSourceDesigner {
		[SecuritySafeCritical]
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(designerHost != null) {
				DevExpress.Utils.Design.ReferencesHelper.EnsureReferences(
					designerHost,
					AssemblyInfo.SRAssemblyData
				);
			}
		}
		LinqServerModeDataSourceDesignerView _view;
		[SecurityCritical]
		public override DesignerDataSourceView GetView(string viewName) {
			if(string.IsNullOrEmpty(viewName)) {
				viewName = "DefaultView";
			}
			if(!string.Equals(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase)) {
				return null;
			}
			if(this._view == null) {
				this._view = new LinqServerModeDataSourceDesignerView(this, viewName);
			}
			return this._view;
		}
		[SecurityCritical]
		public override string[] GetViewNames() {
			return new string[] { "DefaultView" };
		}
		LinqServerModeDataSource LinqServerModeDataSource {
			get {
				return (LinqServerModeDataSource)base.Component;
			}
		}
		internal Type GetDesignedType() {
			return LinqServerModeDataSource.GetView().GetDesignType();
		}
		bool TypeServiceAvailable {
			get {
				IServiceProvider provider = Component.Site;
				if(provider == null)
					return false;
				ITypeResolutionService res = (ITypeResolutionService)provider.GetService(typeof(ITypeResolutionService));
				ITypeDiscoveryService dis = (ITypeDiscoveryService)provider.GetService(typeof(ITypeDiscoveryService));
				return res != null || dis != null;
			}
		}
		[SecurityCritical]
		public override void RefreshSchema(bool preferSilent) {
			OnSchemaRefreshed(EventArgs.Empty);
		}
		public override bool CanRefreshSchema {
			[SecurityCritical]
			get {
				if(TableName == null || TableName.Length == 0)
					return false;
				if(ContextTypeName == null || ContextTypeName.Length == 0)
					return false;
				return TypeServiceAvailable;
			}
		}
		[SecurityCritical]
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PropertyDescriptor descriptorTbl = (PropertyDescriptor)properties["TableName"];
			properties["TableName"] = TypeDescriptor.CreateProperty(GetType(), descriptorTbl, new Attribute[] { new TypeConverterAttribute(typeof(LinqWebTableNameTypeConverter)) });
			PropertyDescriptor descriptorCnt = (PropertyDescriptor)properties["ContextTypeName"];
			properties["ContextTypeName"] = TypeDescriptor.CreateProperty(GetType(), descriptorCnt, new Attribute[] { new TypeConverterAttribute(typeof(LinqWebDataContextTypeConverter)) });
		}
		public string TableName {
			get {
				return LinqServerModeDataSource.TableName;
			}
			set {
				if(value == this.TableName)
					return;
				LinqServerModeDataSource.TableName = value;
				UpdateDesignTimeHtml();
				if(CanRefreshSchema)
					RefreshSchema(true);
				else
					OnDataSourceChanged(EventArgs.Empty);
			}
		}
		public string ContextTypeName {
			get {
				return LinqServerModeDataSource.ContextTypeName;
			}
			set {
				if(value == this.ContextTypeName)
					return;
				LinqServerModeDataSource.ContextTypeName = value;
				UpdateDesignTimeHtml();
				if(CanRefreshSchema)
					RefreshSchema(true);
				else
					OnDataSourceChanged(EventArgs.Empty);
			}
		}
	}
	[SecurityCritical]
	class LinqServerModeDataSourceDesignerView : DesignerDataSourceView {
		private LinqServerModeDataSourceDesigner _owner;
		[SecurityCritical]
		public override IEnumerable GetDesignTimeData(int minimumRows, out bool isSampleData) {
			Type t = _owner.GetDesignedType();
			if(t != null && t != typeof(object)) {
				DataTable table = CreateSchemaTable(t);
				if(table.Columns.Count > 0) {
					isSampleData = true;
					return DesignTimeData.GetDesignTimeDataSource(DesignTimeData.CreateSampleDataTable(new DataView(table), true), minimumRows);
				}
			}
			return base.GetDesignTimeData(minimumRows, out isSampleData);
		}
		public static DataTable CreateSchemaTable(Type t) {
			string keyName = LinqServerModeCore.GuessKeyExpression(t);
			DataTable table = new DataTable(t.FullName);
			DataColumn key = null;
			foreach(var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
				DataColumn column = table.Columns.Add(p.Name, UnNullType(p.PropertyType));
				if(!string.IsNullOrEmpty(keyName) && p.Name == keyName)
					key = column;
			}
			if(key != null) {
				table.PrimaryKey = new DataColumn[] { key };
			}
			return table;
		}
		static Type UnNullType(Type type) {
			Type ut = Nullable.GetUnderlyingType(type);
			if(ut != null)
				return ut;
			return type;
		}
		public LinqServerModeDataSourceDesignerView(LinqServerModeDataSourceDesigner owner, string viewName)
			: base(owner, viewName) {
			this._owner = owner;
		}
		public override IDataSourceViewSchema Schema {
			[SecurityCritical]
			get {
				Type t = _owner.GetDesignedType();
				if(t == null || t == typeof(object)) {
					return null;
				}
				return new DataSetViewSchema(CreateSchemaTable(t));
			}
		}
	}
	public class LinqWebDataContextTypeConverter : TypeConverter {
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			SortedList<string, Type> list = new SortedList<string, Type>();
			if(context != null) {
				try {
					ITypeDiscoveryService typeDiscovery = (ITypeDiscoveryService)context.GetService(typeof(ITypeDiscoveryService));
					if(typeDiscovery == null)
						typeDiscovery = (ITypeDiscoveryService)((IServiceProvider)((System.Web.UI.Control)context.Instance).Site).GetService(typeof(ITypeDiscoveryService));
					if(typeDiscovery != null) {
						foreach(Type t in typeDiscovery.GetTypes(typeof(object), true)) {
							if(!IsGoodType(t))
								continue;
							list.Add(t.FullName, t);
						}
					}
				} catch(Exception e) {
					IUIService s = (IUIService)context.GetService(typeof(IUIService));
					if(s != null)
						s.ShowError(e);
				}
			}
			return new StandardValuesCollection(list.Keys.ToArray());
		}
		static bool IsGoodType(Type t) {
			try {
				if(t.ContainsGenericParameters)
					return false;
				if(t.IsValueType)
					return false;
				if(t.IsAbstract && t.IsSealed)
					return false;
				if(t.GetConstructor(new Type[0]) == null)
					return false;
				foreach(var p in t.GetProperties()) {
					if(LinqWebTableNameTypeConverter.IsTableProperty(p))
						return true;
				}
			} catch { }
			return false;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string)
				return value;
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(value is string)
				return value;
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class LinqWebTableNameTypeConverter : TypeConverter {
		protected virtual string GetContextTypeName(object isntance) {
			return ((LinqServerModeDataSource)isntance).ContextTypeName;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			var list = new SortedList<string, PropertyInfo>();
			if(context != null) {
				try {
					string contextTypeName = GetContextTypeName(context.Instance);
					if(contextTypeName != null && contextTypeName.Length > 0) {
						Type contextType = BuildManager.GetType(contextTypeName, false, true);
						if(contextType != null) {
							foreach(PropertyInfo pi in contextType.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
								if(IsTableProperty(pi)) {
									list.Add(pi.Name, pi);
								}
							}
						}
					}
				} catch(Exception e) {
					IUIService s = (IUIService)context.GetService(typeof(IUIService));
					if(s != null)
						s.ShowError(e);
				}
			}
			return new StandardValuesCollection(list.Keys.ToArray());
		}
		public static bool IsTableProperty(PropertyInfo p) {
			foreach(Type iface in p.PropertyType.GetInterfaces()) {
				if(iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IQueryable<>)) {
					return true;
				}
			}
			return false;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string)
				return value;
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(value is string)
				return value;
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
namespace DevExpress.Data.Linq {
	[PersistChildren(false)]
	[ParseChildren(true)]
	[DXWebToolboxItem(true)]
	[Designer(typeof(DevExpress.Data.Linq.Design.LinqServerModeDataSourceDesigner))]
	[System.ComponentModel.DisplayName("Linq server mode Source")]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)]
	[System.Drawing.ToolboxBitmap(typeof(DevExpress.Web.ToolboxBitmapAccess), DevExpress.Web.ToolboxBitmapAccess.BitmapPath + "LinqServerModeDataSource.bmp")]
	public class LinqServerModeDataSource : DataSourceControl {
		public LinqServerModeDataSource() {
			this.EnableViewState = false;
		}
		protected override DataSourceView GetView(string viewName) {
			if((viewName != null) && ((viewName.Length == 0) || string.Equals(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase))) {
				return this.GetView();
			}
			throw new ArgumentException("", "viewName");
		}
		LinqServerModeDataSourceView _view;
		internal LinqServerModeDataSourceView GetView() {
			if(this._view == null) {
				this._view = new LinqServerModeDataSourceView(this, "DefaultView", this.Context);
			}
			return this._view;
		}
		string[] _viewNames;
		protected override ICollection GetViewNames() {
			if(_viewNames == null)
				this._viewNames = new string[] { "DefaultView" };
			return this._viewNames;
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if(this.Page != null) {
				this.Page.LoadComplete += new EventHandler(LoadCompleteEventHandler);
			}
		}
		void LoadCompleteEventHandler(object sender, EventArgs e) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LinqServerModeDataSourceContextTypeName"),
#endif
		DefaultValue("")]
		public string ContextTypeName {
			get {
				return this.GetView().ContextTypeName;
			}
			set {
				this.GetView().ContextTypeName = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LinqServerModeDataSourceTableName"),
#endif
		DefaultValue("")]
		public string TableName {
			get {
				return this.GetView().TableName;
			}
			set {
				this.GetView().TableName = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LinqServerModeDataSourceDefaultSorting"),
#endif
		DefaultValue("")]
		public string DefaultSorting
		{
			get { return this.GetView().DefaultSorting; }
			set { this.GetView().DefaultSorting = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("LinqServerModeDataSourceSelecting")]
#endif
		public event EventHandler<LinqServerModeDataSourceSelectEventArgs> Selecting {
			add {
				this.GetView().Selecting += value;
			}
			remove {
				this.GetView().Selecting -= value;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("LinqServerModeDataSourceInconsistencyDetected")]
#endif
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected {
			add {
				this.GetView().InconsistencyDetected += value;
			}
			remove {
				this.GetView().InconsistencyDetected -= value;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("LinqServerModeDataSourceExceptionThrown")]
#endif
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown {
			add {
				this.GetView().ExceptionThrown += value;
			}
			remove {
				this.GetView().ExceptionThrown -= value;
			}
		}
		internal bool IsDesignTime() {
			return this.DesignMode;
		}
		[DefaultValue(false), Browsable(false)]
		public override bool EnableViewState {
			get {
				return base.EnableViewState;
			}
			set {
				base.EnableViewState = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LinqServerModeDataSourceEnableDelete"),
#endif
		DefaultValue(false)]
		public bool EnableDelete {
			get;
			set;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LinqServerModeDataSourceEnableInsert"),
#endif
		DefaultValue(false)]
		public bool EnableInsert {
			get;
			set;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LinqServerModeDataSourceEnableUpdate"),
#endif
		DefaultValue(false)]
		public bool EnableUpdate {
			get;
			set;
		}
		protected virtual DataSourceView GetEditHelperView() {
			var helper = new System.Web.UI.WebControls.LinqDataSource();
			helper.ContextTypeName = this.ContextTypeName;
			helper.TableName = this.TableName;
			helper.EnableDelete = GetView().CanDelete;
			helper.EnableInsert = GetView().CanInsert;
			helper.EnableUpdate = GetView().CanUpdate;
			return ((IDataSource)helper).GetView("");
		}
		internal void Insert(IDictionary values, DataSourceViewOperationCallback callback) {
			DoInsert(values, callback);
		}
		internal void Update(IDictionary keys, IDictionary values, IDictionary oldValues, DataSourceViewOperationCallback callback) {
			DoUpdate(keys, values, oldValues, callback);
		}
		internal void Delete(IDictionary keys, IDictionary oldValues, DataSourceViewOperationCallback callback) {
			DoDelete(keys, oldValues, callback);
		}
		protected virtual void DoInsert(IDictionary values, DataSourceViewOperationCallback callback) {
			var ea = new LinqServerModeDataSourceEditEventArgs(null, values, null, callback);
			OnInserting(ea);
			if(ea.Handled)
				return;
			GetEditHelperView().Insert(values, callback);
		}
		protected virtual void DoUpdate(IDictionary keys, IDictionary values, IDictionary oldValues, DataSourceViewOperationCallback callback) {
			var ea = new LinqServerModeDataSourceEditEventArgs(keys, values, oldValues, callback);
			OnUpdating(ea);
			if(ea.Handled)
				return;
			GetEditHelperView().Update(keys, values, oldValues, callback);
		}
		protected virtual void DoDelete(IDictionary keys, IDictionary oldValues, DataSourceViewOperationCallback callback) {
			var ea = new LinqServerModeDataSourceEditEventArgs(keys, null, oldValues, callback);
			OnDeleting(ea);
			if(ea.Handled)
				return;
			GetEditHelperView().Delete(keys, oldValues, callback);
		}
		protected virtual void OnInserting(LinqServerModeDataSourceEditEventArgs e) {
			var handler = (EventHandler<LinqServerModeDataSourceEditEventArgs>)base.Events[ins];
			if(handler != null) {
				handler(this, e);
			}
		}
		protected virtual void OnUpdating(LinqServerModeDataSourceEditEventArgs e) {
			var handler = (EventHandler<LinqServerModeDataSourceEditEventArgs>)base.Events[upd];
			if(handler != null) {
				handler(this, e);
			}
		}
		protected virtual void OnDeleting(LinqServerModeDataSourceEditEventArgs e) {
			var handler = (EventHandler<LinqServerModeDataSourceEditEventArgs>)base.Events[del];
			if(handler != null) {
				handler(this, e);
			}
		}
		static readonly object ins = new object();
		static readonly object upd = new object();
		static readonly object del = new object();
#if !SL
	[DevExpressWebLocalizedDescription("LinqServerModeDataSourceInserting")]
#endif
		public event EventHandler<LinqServerModeDataSourceEditEventArgs> Inserting {
			add { this.Events.AddHandler(ins, value); }
			remove { this.Events.RemoveHandler(ins, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("LinqServerModeDataSourceUpdating")]
#endif
		public event EventHandler<LinqServerModeDataSourceEditEventArgs> Updating {
			add { this.Events.AddHandler(upd, value); }
			remove { this.Events.RemoveHandler(upd, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("LinqServerModeDataSourceDeleting")]
#endif
		public event EventHandler<LinqServerModeDataSourceEditEventArgs> Deleting {
			add { this.Events.AddHandler(del, value); }
			remove { this.Events.RemoveHandler(del, value); }
		}
		public override void RenderControl(HtmlTextWriter writer) {
			base.RenderControl(writer);
		}
	}
	public class LinqServerModeDataSourceEditEventArgs : HandledEventArgs {
		public IDictionary Keys { get; set; }
		public IDictionary Values { get; set; }
		public IDictionary OldValues { get; set; }
		public DataSourceViewOperationCallback Callback { get; set; }
		public LinqServerModeDataSourceEditEventArgs(IDictionary keys, IDictionary values, IDictionary oldValues, DataSourceViewOperationCallback callback) : base(false) {
			this.Keys = keys;
			this.Values = values;
			this.OldValues = oldValues;
			this.Callback = callback;
		}
	}
	public class LinqServerModeDataSourceView : DataSourceView {
		protected readonly LinqServerModeDataSource Owner;
		protected readonly HttpContext Context;
		public LinqServerModeDataSourceView(LinqServerModeDataSource owner, string viewName, HttpContext context)
			: base(owner, viewName) {
			this.Owner = owner;
			this.Context = context;
		}
		internal Type GetDesignType() {
			Type contextType;
			PropertyInfo tableProperty;
			GetContextTypeAndTableProperty(out contextType, out tableProperty);
			if(tableProperty != null) {
				foreach(Type iface in tableProperty.PropertyType.GetInterfaces()) {
					if(iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IQueryable<>)) {
						return iface.GetGenericArguments()[0];
					}
				}
			}
			return null;
		}
		void GetContextTypeAndTableProperty(out Type contextType, out PropertyInfo tableProperty) {
			contextType = null;
			tableProperty = null;
			if(string.IsNullOrEmpty(ContextTypeName) || string.IsNullOrEmpty(TableName))
				return;
			if((contextType = GetType(ContextTypeName)) == null)
				return;
			try {
				tableProperty = contextType.GetProperty(TableName, BindingFlags.Instance | BindingFlags.Public);
			} catch { }
		}
		public Type GetType(string typeName) {
			Type type = null;
			if(Owner.IsDesignTime()) {
				ITypeResolutionService typeResolutionService = ((IServiceProvider)Owner.Site).GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
				if(typeResolutionService != null && !string.IsNullOrEmpty(typeName))
					type = typeResolutionService.GetType(typeName, false, true);
			} else {
				type = BuildManager.GetType(typeName, false, true);
			}
			return type;
		}
		protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments) {
			LinqServerModeDataSourceSelectEventArgs e = new LinqServerModeDataSourceSelectEventArgs();
			e.DefaultSorting = this.DefaultSorting;
			OnSelecting(e);
			IQueryable queryable = null;
			string keyExpression = null;
			if(e.QueryableSource != null) {
				queryable = e.QueryableSource;
			}
			if(e.KeyExpression != null && e.KeyExpression.Length > 0) {
				keyExpression = e.KeyExpression;
			}
			if(queryable == null) {
				Type contextType;
				PropertyInfo tableProperty;
				GetContextTypeAndTableProperty(out contextType, out tableProperty);
				if(contextType != null && tableProperty != null) {
					object context = Activator.CreateInstance(contextType);
					if(context != null) {
						System.Data.Linq.DataContext dataContext = context as System.Data.Linq.DataContext;
						if(dataContext != null) {
							try {
								dataContext.ObjectTrackingEnabled = false;
							} catch { }
						}
						object candidate = tableProperty.GetValue(context, null);
						if(Owner.IsDesignTime())
							queryable = candidate as IQueryable;
						else
							queryable = (IQueryable)candidate;
					}
				}
			}
			if(keyExpression == null && queryable != null) {
				keyExpression = LinqServerModeCore.GuessKeyExpression(queryable.ElementType);
			}
			if(!Owner.IsDesignTime()) {
				if(queryable == null)
					throw new InvalidOperationException("Unable to obtain IQueryable instance from '" + ContextTypeName + "'.'" + TableName + "' or Selecting event");
				if(keyExpression == null || keyExpression.Length == 0)
					throw new InvalidOperationException("Key expression is undefined");
			}
			IListServer iListServer = new LinqServerModeFrontEnd(new LinqServerModeWebFrontEndOwner(queryable, keyExpression, !Owner.IsDesignTime(), e.DefaultSorting));
			iListServer.ExceptionThrown += new EventHandler<ServerModeExceptionThrownEventArgs>(iListServer_ExceptionThrown);
			iListServer.InconsistencyDetected += new EventHandler<ServerModeInconsistencyDetectedEventArgs>(iListServer_InconsistencyDetected);
			return iListServer;
		}
		void iListServer_InconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
			OnInconsistencyDetected(e);
		}
		void iListServer_ExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
			OnExceptionThrown(e);
		}
		string _ContextTypeName = string.Empty;
		public string ContextTypeName {
			get {
				return _ContextTypeName;
			}
			set {
				if(value == null)
					value = string.Empty;
				if(ContextTypeName == value)
					return;
				_ContextTypeName = value;
				OnDataSourceViewChanged(EventArgs.Empty);
			}
		}
		string _TableName = string.Empty;
		public string TableName {
			get {
				return _TableName;
			}
			set {
				if(value == null)
					value = string.Empty;
				if(TableName == value)
					return;
				_TableName = value;
				OnDataSourceViewChanged(EventArgs.Empty);
			}
		}
		string _DefaultSorting = string.Empty;
		public string DefaultSorting {
			get { return _DefaultSorting; }
			set {
				if(value == null)
					value = string.Empty;
				if(DefaultSorting == value)
					return;
				_DefaultSorting = value;
				OnDataSourceViewChanged(EventArgs.Empty);
			}
		}
		static readonly object EventSelecting = new object();
		protected virtual void OnSelecting(LinqServerModeDataSourceSelectEventArgs e) {
			EventHandler<LinqServerModeDataSourceSelectEventArgs> handler = (EventHandler<LinqServerModeDataSourceSelectEventArgs>)Events[EventSelecting];
			if(handler != null)
				handler(this, e);
		}
		public event EventHandler<LinqServerModeDataSourceSelectEventArgs> Selecting {
			add {
				Events.AddHandler(EventSelecting, value);
			}
			remove {
				Events.RemoveHandler(EventSelecting, value);
			}
		}
		static readonly object EventInconsistencyDetected = new object();
		protected virtual void OnInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs e) {
			EventHandler<ServerModeInconsistencyDetectedEventArgs> handler = (EventHandler<ServerModeInconsistencyDetectedEventArgs>)Events[EventInconsistencyDetected];
			if(handler != null)
				handler(this, e);
		}
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected {
			add {
				Events.AddHandler(EventInconsistencyDetected, value);
			}
			remove {
				Events.RemoveHandler(EventInconsistencyDetected, value);
			}
		}
		static readonly object EventExceptionThrown = new object();
		protected virtual void OnExceptionThrown(ServerModeExceptionThrownEventArgs e) {
			EventHandler<ServerModeExceptionThrownEventArgs> handler = (EventHandler<ServerModeExceptionThrownEventArgs>)Events[EventExceptionThrown];
			if(handler != null)
				handler(this, e);
		}
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown {
			add {
				Events.AddHandler(EventExceptionThrown, value);
			}
			remove {
				Events.RemoveHandler(EventExceptionThrown, value);
			}
		}
		public override bool CanDelete {
			get {
				return Owner.EnableDelete;
			}
		}
		public override bool CanUpdate {
			get {
				return Owner.EnableUpdate;
			}
		}
		public override bool CanInsert {
			get {
				return Owner.EnableInsert;
			}
		}
		public override void Insert(IDictionary values, DataSourceViewOperationCallback callback) {
			Owner.Insert(values, callback);
		}
		public override void Update(IDictionary keys, IDictionary values, IDictionary oldValues, DataSourceViewOperationCallback callback) {
			Owner.Update(keys, values, oldValues, callback);
		}
		public override void Delete(IDictionary keys, IDictionary oldValues, DataSourceViewOperationCallback callback) {
			Owner.Delete(keys, oldValues, callback);
		}
	}
	class LinqServerModeWebFrontEndOwner : ILinqServerModeFrontEndOwner {
		readonly IQueryable Src;
		readonly string Key;
		readonly bool isRuntime;
		readonly string defaultSorting;
		public LinqServerModeWebFrontEndOwner(IQueryable source, string keyExpression, bool isRuntime, string defaultSorting) {
			this.Src = source;
			this.Key = keyExpression;
			this.isRuntime = isRuntime;
			this.defaultSorting = defaultSorting;
		}
		public Type ElementType {
			get {
				if(QueryableSource != null)
					return QueryableSource.ElementType;
				return null;
			}
		}
		public IQueryable QueryableSource {
			get { return Src; }
		}
		public bool IsReadyForTakeOff() {
			if(QueryableSource == null)
				return false;
			if(KeyExpression == null || KeyExpression.Length == 0)
				return false;
			return isRuntime;
		}
		public string KeyExpression {
			get { return Key; }
		}
		public string DefaultSorting { get { return defaultSorting; } }
	}
	public class LinqServerModeDataSourceSelectEventArgs : EventArgs {
		public IQueryable QueryableSource { get; set; }
		public string KeyExpression { get; set; }
		public string DefaultSorting { get; set; }
	}
}
