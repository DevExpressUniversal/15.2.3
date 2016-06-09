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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Security.Permissions;
using System.Web.UI.Design;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.Web.Design;
namespace DevExpress.ExpressApp.Design {
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class XafWebDesignDataSourceDesigner : DataSourceDesigner, IServiceProvider {
		private XafDesignerWebDataSourceView view = null;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			ITypesInfoProvider typesInfoProvider = designerHost.GetService(typeof(ITypesInfoProvider)) as ITypesInfoProvider;
			designerHost.RemoveService(typeof(ITypesInfo));
			designerHost.AddService(typeof(ITypesInfo), typesInfoProvider.CreatedTypesInfo(designerHost));
		}
		public override DesignerDataSourceView GetView(string viewName) {
			if(view == null) {
				view = new XafDesignerWebDataSourceView(this, (IServiceProvider)this, "DefaultView");
			}
			if(Component is XafWebDesignDataSource) {
				view.ObjectTypeName = ((XafWebDesignDataSource)Component).ObjectTypeName;
			}
			return view;
		}
		public override string[] GetViewNames() {
			return new string[] { "DefaultView" };
		}
		public override void RefreshSchema(bool preferSilent) {
			OnSchemaRefreshed(EventArgs.Empty);
		}
		public override bool CanRefreshSchema {
			get {
				if(Component is XafWebDesignDataSource) {
					return TypeServiceAvailable;
				}
				return false;
			}
		}
		bool TypeServiceAvailable {
			get {
				IServiceProvider provider = Component.Site;
				if(provider == null)
					return false;
				ITypesInfo res = (ITypesInfo)provider.GetService(typeof(ITypesInfo));
				return res != null;
			}
		}
		object IServiceProvider.GetService(Type serviceType) {
			return GetService(serviceType);
		}
	}
	class XafDesignerWebDataSourceView : DesignerDataSourceView {
		IServiceProvider serviceProvider = null;
		private XafWebDesignDataSourceDesigner _owner;
		public XafDesignerWebDataSourceView(XafWebDesignDataSourceDesigner owner, IServiceProvider serviceProvider, string viewName)
			: base(owner, viewName) {
			this.serviceProvider = serviceProvider;
			this._owner = owner;
		}
		public override IDataSourceViewSchema Schema {
			get {
				if(!string.IsNullOrEmpty(ObjectTypeName)) {
					return new XafViewSchema(serviceProvider, ObjectTypeName);
				}
				return null;
			}
		}
		public string ObjectTypeName {
			get;
			set;
		}
		public override bool CanDelete { get { return true; } }
		public override bool CanInsert { get { return true; } }
		public override bool CanPage { get { return true; } }
		public override bool CanRetrieveTotalRowCount { get { return true; } }
		public override bool CanSort { get { return true; } }
		public override bool CanUpdate { get { return true; } }
	}
	class XafViewSchema : IDataSourceViewSchema {
	  class TypeFieldSchema : IDataSourceFieldSchema {
		private PropertyDescriptor fFieldDescriptor;
		private bool fIsIdentity;
		private bool fIsNullable;
		private int fLength;
		private bool fPrimaryKey;
		private bool fRetrievedMetaData;
		public TypeFieldSchema(PropertyDescriptor fieldDescriptor) {
			fLength = -1;
			if(fieldDescriptor == null)
				throw new ArgumentNullException();
			fFieldDescriptor = fieldDescriptor;
		}
		private void EnsureMetaData() {
			if(!fRetrievedMetaData) {
				DataObjectFieldAttribute attribute = (DataObjectFieldAttribute)fFieldDescriptor.Attributes[typeof(DataObjectFieldAttribute)];
				if(attribute != null) {
					fPrimaryKey = attribute.PrimaryKey;
					fIsIdentity = attribute.IsIdentity;
					fIsNullable = attribute.IsNullable;
					fLength = attribute.Length;
				}
				fRetrievedMetaData = true;
			}
		}
		Type System.Web.UI.Design.IDataSourceFieldSchema.DataType {
			get {
				Type type = fFieldDescriptor.PropertyType;
				if(type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>))) {
					return type.GetGenericArguments()[0];
				}
				return type;
			}
		}
		bool System.Web.UI.Design.IDataSourceFieldSchema.Identity {
			get {
				EnsureMetaData();
				return fIsIdentity;
			}
		}
		bool System.Web.UI.Design.IDataSourceFieldSchema.IsReadOnly {
			get {
				return fFieldDescriptor.IsReadOnly;
			}
		}
		bool System.Web.UI.Design.IDataSourceFieldSchema.IsUnique {
			get {
				return false;
			}
		}
		int System.Web.UI.Design.IDataSourceFieldSchema.Length {
			get {
				EnsureMetaData();
				return fLength;
			}
		}
		string System.Web.UI.Design.IDataSourceFieldSchema.Name {
			get {
				return fFieldDescriptor.Name;
			}
		}
		bool System.Web.UI.Design.IDataSourceFieldSchema.Nullable {
			get {
				EnsureMetaData();
				Type type = fFieldDescriptor.PropertyType;
				if(!type.IsValueType || fIsNullable) {
					return true;
				}
				if(type.IsGenericType) {
					return (type.GetGenericTypeDefinition() == typeof(Nullable<>));
				}
				return false;
			}
		}
		int System.Web.UI.Design.IDataSourceFieldSchema.Precision {
			get {
				return -1;
			}
		}
		bool System.Web.UI.Design.IDataSourceFieldSchema.PrimaryKey {
			get {
				EnsureMetaData();
				return fPrimaryKey;
			}
		}
		int System.Web.UI.Design.IDataSourceFieldSchema.Scale {
			get {
				return -1;
			}
		}
	}
		private IServiceProvider serviceProvider = null;
		private string objectTypeName;
		public XafViewSchema(IServiceProvider serviceProvider, string objectTypeName) {
			this.serviceProvider = serviceProvider;
			this.objectTypeName = objectTypeName;
		}
		#region IDataSourceViewSchema Members
		public IDataSourceViewSchema[] GetChildren() {
			return null;
		}
		public IDataSourceFieldSchema[] GetFields() {
			List<TypeFieldSchema> list = new List<TypeFieldSchema>();
			CollectionPropertyDescriptorProvider collectionPropertyDescriptorProvider = new CollectionPropertyDescriptorProvider(objectTypeName, "DefaultView", serviceProvider, true);
			foreach(PropertyDescriptor item in ((ITypedList)collectionPropertyDescriptorProvider).GetItemProperties(null)) {
				list.Add(new TypeFieldSchema(item));
			}
			return list.ToArray();
		}
		public string Name {
			get { return objectTypeName; }
		}
		#endregion
	}
}
