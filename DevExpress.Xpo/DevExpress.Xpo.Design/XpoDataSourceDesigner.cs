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
using System.Data;
using System.Web.UI.Design;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DevExpress.Xpo.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Security.Permissions;
using DevExpress.Xpo.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using System.Drawing.Design;
using DevExpress.Utils.About;
namespace DevExpress.Xpo.Design {
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class XpoDataSourceDesigner : DataSourceDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(designerHost != null) {
				DevExpress.Utils.Design.ReferencesHelper.EnsureReferences(
					designerHost,
					AssemblyInfo.SRAssemblyData,
					AssemblyInfo.SRAssemblyXpo
				);
			}
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
		}
		XpoDesignerDataSourceView _view;
		public override DesignerDataSourceView GetView(string viewName) {
			if (string.IsNullOrEmpty(viewName)) {
				viewName = "DefaultView";
			}
			if (!string.Equals(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase)) {
				return null;
			}
			if (this._view == null) {
				this._view = new XpoDesignerDataSourceView(this, viewName);
			}
			return this._view;
		}
		public override string[] GetViewNames() {
			return new string[] { "DefaultView" };
		}
		internal static Type GetType(IServiceProvider serviceProvider, string typeName) {
			if (serviceProvider == null)
				return null;
			ITypeResolutionService service = (ITypeResolutionService)serviceProvider.GetService(typeof(ITypeResolutionService));
			if (service == null)
				return null;
			try {
				return service.GetType(typeName, true, true);
			} catch {
				return null;
			}
		}
		public XPClassInfo GetClassInfo() {
			Type type = GetType(Component.Site, TypeName);
			if (type == null)
				return null;
			return new DesignTimeReflection(Component.Site).QueryClassInfo(type);
		}
		XpoDataSource XpoDataSource {
			get {
				return (XpoDataSource)base.Component;
			}
		}
		bool TypeServiceAvailable {
			get {
				IServiceProvider provider = Component.Site;
				if (provider == null)
					return false;
				ITypeResolutionService res = (ITypeResolutionService)provider.GetService(typeof(ITypeResolutionService));
				ITypeDiscoveryService dis = (ITypeDiscoveryService)provider.GetService(typeof(ITypeDiscoveryService));
				return res != null || dis != null;
			}
		}
		public override void RefreshSchema(bool preferSilent) {
			OnSchemaRefreshed(EventArgs.Empty);
		}
		public override bool CanRefreshSchema {
			get {
				if (!string.IsNullOrEmpty(TypeName)) {
					return TypeServiceAvailable;
				}
				return false;
			}
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PropertyDescriptor descriptor1 = (PropertyDescriptor)properties["TypeName"];
			properties["TypeName"] = TypeDescriptor.CreateProperty(base.GetType(), descriptor1, new Attribute[0]);
		}
		public string TypeName {
			get {
				return XpoDataSource.TypeName;
			}
			set {
				if (value != this.TypeName) {
					XpoDataSource.TypeName = value;
					UpdateDesignTimeHtml();
					if (CanRefreshSchema)
						RefreshSchema(true);
					else
						OnDataSourceChanged(EventArgs.Empty);
				}
			}
		}
	}
	class XpoMemberSchema : IDataSourceFieldSchema {
		XPMemberInfo mi;
		Type type;
		string name;
		public XpoMemberSchema(XPMemberInfo mi) {
			this.mi = mi;
			name = mi.Name;
			type = mi.MemberType;
		}
		public XpoMemberSchema(XPMemberInfo mi, string name, Type type) {
			this.mi = mi;
			this.name = name;
			this.type = type;
		}
		#region IDataSourceFieldSchema Members
		static Type GetType(Type type) {
			Type nullType = System.Nullable.GetUnderlyingType(type);
			return nullType != null ? nullType : type;
		}
		public Type DataType {
			get { return GetType(type); }
		}
		public bool Identity {
			get { return false; }
		}
		public bool IsReadOnly {
			get { return mi.IsReadOnly; }
		}
		public bool IsUnique {
			get { return false; }
		}
		public int Length {
			get { return mi.MappingFieldSize; }
		}
		public string Name {
			get { return name; }
		}
		public bool Nullable {
			get { return true; }
		}
		public int Precision {
			get { return -1; }
		}
		public bool PrimaryKey {
			get { return mi.IsKey; }
		}
		public int Scale {
			get { return -1; }
		}
		#endregion
	}
	class XpoViewSchema : IDataSourceViewSchema {
		XPClassInfo ci;
		public XpoViewSchema(XPClassInfo ci) {
			this.ci = ci;
		}
		#region IDataSourceViewSchema Members
		public IDataSourceViewSchema[] GetChildren() {
			return null;
		}
		static bool IsGoodDefaultProperty(XPMemberInfo mi) {
			if (!mi.IsPublic)
				return false;
			if (mi.MemberType == typeof(IBindingList))
				return false;
			if (!mi.IsVisibleInDesignTime)
				return false;
			if (mi is DevExpress.Xpo.Metadata.Helpers.ServiceField)
				return false;
			if (mi.Name == "This")
				return false;
			return true;
		}
		public IDataSourceFieldSchema[] GetFields() {
			List<XpoMemberSchema> list = new List<XpoMemberSchema>();
			foreach (XPMemberInfo mi in this.ci.Members) {
				if (IsGoodDefaultProperty(mi)) {
					if (mi.ReferenceType != null && mi.ReferenceType.IdClass != null)
						list.Add(new XpoMemberSchema(mi, mi.Name + XPPropertyDescriptor.ReferenceAsKeyTail, mi.ReferenceType.KeyProperty.MemberType));
					else
						list.Add(new XpoMemberSchema(mi));
				}
			}
			return list.ToArray();
		}
		public string Name {
			get { return ci.FullName; }
		}
		#endregion
	}
	class XpoDesignerDataSourceView : DesignerDataSourceView {
		private XpoDataSourceDesigner _owner;
		public override IEnumerable GetDesignTimeData(int minimumRows, out bool isSampleData) {
			XPClassInfo ci = this._owner.GetClassInfo();
			if (ci != null) {
				DataTable table = new DataTable(_owner.TypeName);
				foreach (IDataSourceFieldSchema mi in Schema.GetFields())
					table.Columns.Add(mi.Name, mi.DataType);
				isSampleData = true;
				return DesignTimeData.GetDesignTimeDataSource(DesignTimeData.CreateSampleDataTable(new DataView(table), true), minimumRows);
			} else
				return base.GetDesignTimeData(minimumRows, out isSampleData);
		}
		public XpoDesignerDataSourceView(XpoDataSourceDesigner owner, string viewName)
			: base(owner, viewName) {
			this._owner = owner;
		}
		public override IDataSourceViewSchema Schema {
			get {
				XPClassInfo ci = this._owner.GetClassInfo();
				if (ci == null)
					return null;
				return new XpoViewSchema(ci);
			}
		}
		public override bool CanDelete { get { return true; } }
		public override bool CanInsert { get { return true; } }
		public override bool CanPage { get { return true; } }
		public override bool CanRetrieveTotalRowCount { get { return true; } }
		public override bool CanSort { get { return true; } }
		public override bool CanUpdate { get { return true; } }
	}
	public class XpoDataSourceTypeNameConverter : System.ComponentModel.ReferenceConverter {
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
			if (val is string) {
				if ((string)val == "(none)")
					return String.Empty;
				return val;
			}
			return base.ConvertFrom(context, culture, val);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)) {
				return true;
			}
			return base.CanConvertFrom(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destType) {
			if (destType == typeof(string)) {
				if (val == null || (string)val == String.Empty)
					return "(none)";
				return val;
			}
			return base.ConvertTo(context, culture, val, destType);
		}
		public XpoDataSourceTypeNameConverter() : base(typeof(string)) { }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			SortedList list = new SortedList();
			list.Add("(none)", null);
			ITypeDiscoveryService srv = (ITypeDiscoveryService)((IServiceProvider)((System.Web.UI.Control)context.Instance).Site).GetService(typeof(ITypeDiscoveryService));
			foreach (Type type in srv.GetTypes(typeof(object), true)) {
				if(ReflectionDictionary.DefaultCanGetClassInfoByType(type)) {
					list[type.FullName] = type.FullName;
				}
			}
			return new StandardValuesCollection(list.Values);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
	class XpoDataSourceCriteriaEditor : CriteriaEditor {
		protected override FilterColumnCollection GetColumns(object instance) {
			FilterColumnCollection cols = new FilterColumnCollection();
			XpoDataSource col = instance as XpoDataSource;
			if (col != null) {
				Type type = XpoDataSourceDesigner.GetType(col.Site, col.TypeName);
				if (type != null) {
					XPClassInfo ci = new DesignTimeReflection(col.Site).QueryClassInfo(type);
					Hashtable names = new Hashtable();
					if (ci != null) {
						foreach (XPMemberInfo mi in ci.Members) {
							if (!names.ContainsKey(mi.Name) && IsGoodProperty(mi)) {
								cols.Add(CreateColumn(mi));
								names.Add(mi.Name, null);
							}
						}
					}
				}
			}
			return cols;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (value == null || value is string) {
				CriteriaOperator newValue = (CriteriaOperator)base.EditValue(context, provider, CriteriaOperator.Parse((string)value));
				return ReferenceEquals(newValue, null) ? (value == null ? null : String.Empty) : newValue.ToString();
			}
			return value;
		}
	}
	class XpoDataSourceDefaultSortingCollectionEditor : DefaultSortingCollectionEditor {
		public XpoDataSourceDefaultSortingCollectionEditor()
			: base() {
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			try {
				XpoDataSource dataSource = context.Instance as XpoDataSource;
				if(dataSource != null) {
					string typeName = dataSource.TypeName;
					if(string.IsNullOrEmpty(typeName)) return null;				  
					Type type = XpoDataSourceDesigner.GetType(provider, typeName);
					if(type != null) {
						ci = new DesignTimeReflection(provider).QueryClassInfo(type);
					} else {
						ci = new DesignTimeReflection(provider).QueryClassInfo("", typeName);
					}
				}
				return base.EditValue(context, provider, value);
			} finally {
				ci = null;
			}
		}
	}
}
