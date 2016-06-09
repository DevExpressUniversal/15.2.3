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
using System.Web.UI;
using DevExpress.Data.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Web.Editors {
	public class WebDataSource : IDataSource {
		private WebDataSourceView view;
		private Object collection;
		protected internal IObjectSpace objectSpace;
		private ITypeInfo typeInfo;
		protected void OnDataSourceChanged() {
			if(DataSourceChanged != null) {
				DataSourceChanged(this, EventArgs.Empty);
			}
		}
		public WebDataSource(IObjectSpace objectSpace, ITypeInfo typeInfo, Object collection) {
			this.collection = collection;
			this.objectSpace = objectSpace;
			this.typeInfo = typeInfo;
		}
		public Object Collection {
			get { return collection; }
			set {
				if(collection != value) {
					collection = value;
					OnDataSourceChanged();
				}
			}
		}
		public DataSourceView GetView(String viewName) {
			return View;
		}
		public WebDataSourceView View {
			get {
				if(view == null) {
					view = CreateView();
				}
				return view;
			}
		}
		protected virtual WebDataSourceView CreateView() {
#pragma warning disable 0618
			if(typeInfo.FindAttribute<DevExpress.ExpressApp.Editors.NonPersistentEditableAttribute>() != null) {
#pragma warning restore 0618
				return new WebNonPersistentDataSourceView(this, typeInfo, collection);
			}
			if(collection is IColumnsServerActions) {
				return new WebPersistentDataSourceViewWithServerActions(this, objectSpace, typeInfo, collection, (IColumnsServerActions)collection);
			}
			return new WebPersistentDataSourceView(this, objectSpace, typeInfo, collection);
		}
		public ICollection GetViewNames() {
			return new String[] { "Default" };
		}
		public bool ShowNewObjects {
			get { return view.ShowNewObjects; }
			set {
				view.ShowNewObjects = value;
			}
		}
		public event EventHandler DataSourceChanged;
	}
	public abstract class WebDataSourceView : DataSourceView {
		private ITypeInfo typeInfo;
		private Object collection;
		private bool showNewObjects = true;
		private object editingObject;
		protected WebDataSource owner;
		private Type GetRealType(Type type) {
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if(underlyingType != null) {
				return underlyingType;
			}
			return type;
		}
		private Object ConvertValue(Object value, Type targetType) {
			if(!targetType.IsInstanceOfType(value)) {
				Type type = GetRealType(targetType);
				String text = value as String;
				if(text != null) {
					TypeConverter converter = TypeDescriptor.GetConverter(type);
					if(converter != null) {
						return converter.ConvertFromString(text);
					}
				}
			}
			return value;
		}
		private void SetValues(IDictionary values, ITypeInfo typeInfo, Object targetObject) {
			foreach(DictionaryEntry entry in values) {
				String memberName = (String)entry.Key;
				IMemberInfo memberInfo = typeInfo.FindMember(memberName);
				if(memberInfo == null) {
					throw new MemberAccessException(String.Format("The '{0}' member not found", memberName));
				}
				memberInfo.SetValue(targetObject, ConvertValue(entry.Value, memberInfo.MemberType));
			}
		}
		protected abstract bool IsNewObject(object obj);
		protected abstract object GetObjectByKey(Type type, object key);
		protected abstract object CreateObject(Type type);
		public abstract object GetKeyValue(object obj);
		protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments) {
			if(collection == null) {
				return new Object[] { };
			}
			IList list = ListHelper.GetList(collection);
			if(EditingObject == null && ShowNewObjects) {
				return list;
			}
			else {
				return GetShownCollection(list);
			}
		}
		protected virtual IList GetShownCollection(IList list) {
			ArrayList innerList = new ArrayList();
			if(list != null) {
				for(int i = 0; i < list.Count; i++) {
					object obj = list[i];
					if(obj != EditingObject && (ShowNewObjects || !IsNewObject(obj))) {
						innerList.Add(obj);
					}
				}
			}
			ProxyCollection result = new ProxyCollection(null, typeInfo, innerList);
			result.DisplayableMembers = owner.objectSpace.GetDisplayableProperties(list);
			return result;
		}
		protected virtual void OnObjectUpdated() {
			if(ObjectUpdated != null) {
				ObjectUpdated(this, EventArgs.Empty);
			}
		}
		protected override Int32 ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues) {
			if(keys.Count == 0) {
				throw new NotSupportedException();
			}
			Object key = null;
			foreach(DictionaryEntry entry in keys) {
				key = entry.Value;
			}
			Object targetObject = GetObjectByKey(typeInfo.Type, key);
			SetValues(values, typeInfo, targetObject);
			OnObjectUpdated();
			return 1;
		}
		protected override Int32 ExecuteInsert(IDictionary values) {
			Object targetObject = CreateObject(typeInfo.Type);
			SetValues(values, typeInfo, targetObject);
			if(!(owner.Collection is XafDataView)) {
				ListHelper.GetList(owner.Collection).Add(targetObject);
			}
			return 1;
		}
		public WebDataSourceView(WebDataSource owner, ITypeInfo typeInfo, Object collection)
			: base(owner, "Default") {
			this.owner = owner;
			this.typeInfo = typeInfo;
			this.collection = collection;
		}
		public bool ShowNewObjects {
			get { return showNewObjects; }
			set { showNewObjects = value; }
		}
		public object EditingObject {
			get { return editingObject; }
			set { editingObject = value; }
		}
		public override Boolean CanUpdate {
			get { return true; }
		}
		public event EventHandler<EventArgs> ObjectUpdated;
	}
	public class WebPersistentDataSourceView : WebDataSourceView {
		private IObjectSpace objectSpace;
		public WebPersistentDataSourceView(WebDataSource owner, IObjectSpace objectSpace, ITypeInfo typeInfo, Object collection)
			: base(owner, typeInfo, collection) {
			this.objectSpace = objectSpace;
		}
		public override object GetKeyValue(object obj) {
			return objectSpace.GetKeyValue(obj);
		}
		protected override bool IsNewObject(object obj) {
			return objectSpace.IsNewObject(obj);
		}
		protected override object GetObjectByKey(Type type, object key) {
			return objectSpace.GetObjectByKey(type, key);
		}
		protected override object CreateObject(Type type) {
			object result = objectSpace.CreateObject(type);
			LastCreatedObjectKey = objectSpace.GetKeyValue(result);
			return result;
		}
		public object LastCreatedObjectKey { get; private set; }
	}
	public class WebPersistentDataSourceViewWithServerActions : WebPersistentDataSourceView, IColumnsServerActions {
		private IColumnsServerActions serverActions;
		public WebPersistentDataSourceViewWithServerActions(WebDataSource owner, IObjectSpace objectSpace, ITypeInfo typeInfo, Object collection, IColumnsServerActions serverActions)
			: base(owner, objectSpace, typeInfo, collection) {
			Guard.ArgumentNotNull(serverActions, "serverActions");
			this.serverActions = serverActions;
		}
		bool IColumnsServerActions.AllowAction(string fieldName, ColumnServerActionType action) {
			return serverActions.AllowAction(fieldName, action);
		}
	}
	public class WebNonPersistentDataSourceView : WebDataSourceView {
		private IBindingList bindingList;
		public WebNonPersistentDataSourceView(WebDataSource owner, ITypeInfo typeInfo, Object collection)
			: base(owner, typeInfo, collection) {
			this.bindingList = ListHelper.GetList(collection) as IBindingList;
		}
		public override object GetKeyValue(object obj) {
			return owner.objectSpace.GetKeyValue(obj);
		}
		protected override bool IsNewObject(object obj) {
			return !bindingList.Contains(obj);
		}
		protected override object GetObjectByKey(Type type, object key) {
			if(bindingList != null) {
				foreach(object obj in bindingList) {
					if(MatchKey(obj, key)) {
						return obj;
					}
				}
			}
			return null;
		}
		private bool MatchKey(object obj, object key) {
			return GetKeyValue(obj).Equals(key);
		}
		protected override object CreateObject(Type type) {
			if(bindingList != null) {
				return bindingList.AddNew();
			}
			return null;
		}
	}
}
