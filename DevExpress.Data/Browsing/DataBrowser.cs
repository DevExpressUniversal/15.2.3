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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Data.Access;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Utils;
namespace DevExpress.Data.Browsing {
	public static class ListTypeHelper {
		readonly static ConcurrentDictionary<Type, bool> genericTypes = new ConcurrentDictionary<Type, bool>();
		public static bool IsListType(Type type) {
			return (typeof(IList).IsAssignableFrom(type)
				|| typeof(IListSource).IsAssignableFrom(type)
				|| FindTypeDefinition(type, x => typeof(IList<>) == x || typeof(IEnumerable<>) == x || typeof(ICollection<>) == x) != null)
				&& !ExpandoPropertyDescriptor.IsDynamicType(type);
		}
		public static Type FindTypeDefinition(Type type, Predicate<Type> match) {
			if(IsGenericType(type) && match(type.GetGenericTypeDefinition()))
				return type;
			foreach(Type item in type.GetInterfaces()) {
				if(IsGenericType(item) && match(item.GetGenericTypeDefinition()))
					return item;
			}
			return null;
		}
		public static Type[] FindGenericArguments(Type type, Predicate<Type> match) {
			Type listType = FindTypeDefinition(type, match);
			return listType != null ? listType.GetGenericArguments() : null;
		}
		static bool IsGenericType(Type type) {
			return type.IsGenericType() && genericTypes.GetOrAdd(type, IsGenericTypeCore);
		}
		static bool IsGenericTypeCore(Type type) {
			Type[] args = type.GetGenericArguments();
			return args.Length == 1 && !args[0].IsPrimitive();
		}
	}
	public class DataContextBase : IDisposable {
		#region inner classes
		protected class HashObj {
			public static HashObj CreateInstance(object dataSource, string[] members, int count) {
				string dataMember = GetDataMember(members, count);
				return new HashObj(dataSource, dataMember);
			}
			public static string GetDataMember(string[] members, int count) {
				return String.Join(".", members, 0, count);
			}
			WeakReference weakRef;
			string dataMember;
			public string DataMember { get { return dataMember; } }
			public object DataSource { get { return weakRef.Target; } }
			public HashObj(object dataSource, string dataMember) {
				if(dataSource == null)
					throw new ArgumentNullException("dataSource");
				if(dataMember == null)
					dataMember = "";
				this.weakRef = new WeakReference(dataSource, false);
				this.dataMember = dataMember.ToLower(System.Globalization.CultureInfo.InvariantCulture);
			}
			public override int GetHashCode() {
				return this.DataSource.GetHashCode() * dataMember.GetHashCode();
			}
			public override bool Equals(object obj) {
				if(obj is HashObj) {
					HashObj hashObj = (HashObj)obj;
					return this.DataSource == hashObj.DataSource && dataMember == hashObj.dataMember;
				}
				return false;
			}
		}
		class StateItem {
			public DataBrowser DataBrowser { get; set; }
			public HashObj Key { get; set; }
			public object State { get; set; }
		}
		#endregion
		Dictionary<HashObj, DataBrowser> browserDictionary;
		public bool IsDisposed { get; private set; }
		public DataBrowser this[object dataSource, string dataMember] {
			get { return GetDataBrowserInternal(dataSource, dataMember, false); }
		}
		public DataBrowser this[object dataSource] { get { return this[dataSource, ""]; } }
		protected DataBrowser this[HashObj hashObj] {
			get {
				DataBrowser dataBrowser;
				browserDictionary.TryGetValue(hashObj, out dataBrowser);
				return dataBrowser != null && !dataBrowser.IsClosed ? dataBrowser : null;
			}
		}
		protected bool SuppressListFilling { get; private set; }
		public DataContextBase(bool suppressListFilling) {
			browserDictionary = new Dictionary<HashObj, DataBrowser>();
			this.SuppressListFilling = suppressListFilling;
		}
		public DataContextBase()
			: this(false) {
		}
		protected virtual ListControllerBase CreateListCotroller() {
			return new ListControllerBase();
		}
		public void Dispose() {
			Clear();
			IsDisposed = true;
		}
		protected IDictionaryEnumerator GetEnumerator() {
			return browserDictionary.GetEnumerator();
		}
		public object SaveState() {
			List<StateItem> state = new List<StateItem>();
			foreach(var item in browserDictionary)
				state.Add(new StateItem() { DataBrowser = item.Value, Key = item.Key, State = item.Value.SaveState() });
			return state;
		}
		public void LoadState(object state) {
			List<StateItem> myState = (List<StateItem>)state;
			foreach(StateItem item in myState) {
				item.DataBrowser.LoadState(item.State);
				browserDictionary.Remove(item.Key);
			}
			Clear();
			foreach(StateItem item in myState)
				browserDictionary[item.Key] = item.DataBrowser;
		}
		public virtual void Clear() {
			List<DataBrowser> items = new List<DataBrowser>(browserDictionary.Values);
			foreach(var item in items)
				item.Close();
			browserDictionary.Clear();
		}
		public string GetListName(object dataSource) {
			return GetListName(dataSource, string.Empty);
		}
		public string GetListName(object dataSource, string dataMember) {
			if(dataSource != null) {
				DataBrowser dataBrowser = GetDataBrowser(dataSource, dataMember, true);
				if(dataBrowser != null)
					return dataBrowser.GetListName();
			}
			return string.Empty;
		}
		protected static string GetNameFromTypedList(ITypedList dataSource) {
			return dataSource != null
				? dataSource.GetListName(null)
				: string.Empty;
		}
		protected static object ForceList(object dataSource) {
			return dataSource is IListSource ? ((IListSource)dataSource).GetList() : dataSource;
		}
		public PropertyDescriptorCollection GetListItemProperties(object dataSource, string dataMember) {
			DataBrowser dataBrowser = GetDataBrowser(dataSource, dataMember, true);
			return dataBrowser != null && ShouldExpand(dataBrowser) ? dataBrowser.GetItemProperties() : PropertyDescriptorCollection.Empty;
		}
		public PropertyDescriptorCollection GetItemProperties(object dataSource, string dataMember, Predicate<Type> match) {
			PropertyDescriptorCollection props = GetItemProperties(dataSource, dataMember);
			PropertyDescriptorCollection resultProps = new PropertyDescriptorCollection(null);
			foreach(PropertyDescriptor item in props)
				if(match(item.PropertyType))
					resultProps.Add(item);
			return resultProps;
		}
		public PropertyDescriptorCollection GetItemProperties(object dataSource, string dataMember, Type[] types) {
			if(types == null)
				throw new ArgumentNullException("types");
			return GetItemProperties(dataSource, dataMember, delegate(Type type) {
				foreach(Type item in types)
					if(item.IsAssignableFrom(type))
						return true;
				return false;
			});
		}
		public Type GetPropertyType(object dataSource, string dataMember) {
			DataBrowser dataBrowser = GetDataBrowser(dataSource, dataMember, true);
			return dataBrowser != null ? dataBrowser.DataSourceType : null;
		}
		public DataBrowser GetDataBrowser(object dataSource, string dataMember, bool suppressException) {
			if(dataSource == null)
				return null;
			return GetDataBrowserInternal(dataSource, dataMember, suppressException);
		}
		public PropertyDescriptorCollection GetItemProperties(object dataSource, string dataMember) {
			DataBrowser dataBrowser = GetDataBrowser(dataSource, dataMember, true);
			return dataBrowser != null ? dataBrowser.GetItemProperties() : new PropertyDescriptorCollection(null);
		}
		public string GetColumnName(object dataSource, string dataMember) {
			return GetColumnName(dataSource, dataMember, false);
		}
		public string GetColumnName(object dataSource, string dataMember, bool suppressException) {
			PropertyDescriptor property = GetProperty(dataSource, dataMember, suppressException);
			return property != null ? property.Name : null;
		}
		PropertyDescriptor GetProperty(object dataSource, string dataMember, bool suppressException) {
			if(dataSource == null) {
				if(!suppressException)
					throw new ArgumentNullException("dataSource");
				return null;
			}
			if(string.IsNullOrEmpty(dataMember)) {
				if(!suppressException)
					throw new ArgumentException("dataMember");
				return null;
			}
			IRelatedDataBrowser dataBrowser = GetDataBrowserInternal(dataSource, dataMember, suppressException) as IRelatedDataBrowser;
			return dataBrowser != null ? dataBrowser.RelatedProperty : null;
		}
		public DataBrowser GetDataBrowserInternal(object dataSource, string parentDataMember, string dataMember) {
			DataBrowser parentBrowser = this[new HashObj(dataSource, parentDataMember)];
			if(String.IsNullOrEmpty(dataMember))
				return String.IsNullOrEmpty(parentDataMember) ? null : parentBrowser;
			string[] members = dataMember.Split('.');
			if(parentBrowser == null)
				return GetDataBrowserInternal(dataSource, String.IsNullOrEmpty(parentDataMember) ? members[0] : parentDataMember + '.' + members[0],
					String.Join(".", members, 1, members.Length - 1));
			for(int i = 1; i <= members.Length; i++) {
				string propName = String.Join(".", members, 0, i);
				string fullPath = String.IsNullOrEmpty(parentDataMember) ? propName : parentDataMember + '.' + propName;
				DataBrowser browser = this[new HashObj(dataSource, fullPath)];
				if(browser == null) {
					PropertyDescriptor propDesc = parentBrowser.FindItemProperty(propName, true);
					if(propDesc != null)
						AddToHashtable(dataSource, fullPath, CreateDataBrowser(parentBrowser, propDesc));
				}
				String childDataMember;
				if(i < members.Length) {
					childDataMember = String.Join(".", members, i, members.Length - i);
					if(String.IsNullOrEmpty(childDataMember))
						continue;
				} else
					childDataMember = String.Empty;
				browser = GetDataBrowserInternal(dataSource,
					String.IsNullOrEmpty(parentDataMember) ? propName : parentDataMember + '.' + propName, childDataMember);
				if(browser != null)
					return browser;
			}
			return null;
		}
		protected virtual DataBrowser GetDataBrowserInternal(object dataSource, string dataMember, bool suppressException) {
			if(dataSource == null)
				return null;
			if(dataMember == null)
				dataMember = String.Empty;
			DataBrowser dataBrowser = null;
			try {
				DataBrowser parentBrowser = this[new HashObj(dataSource, String.Empty)];
				if(parentBrowser == null) {
					parentBrowser = CreateDataBrowser(dataSource);
					AddToHashtable(dataSource, String.Empty, parentBrowser);
				}
				dataBrowser = this[new HashObj(dataSource, dataMember)];
				if(dataBrowser != null)
					return dataBrowser;
				dataBrowser = GetDataBrowserInternal(dataSource, String.Empty, dataMember);
			} catch {
				if(!suppressException)
					throw;
			}
			if(dataBrowser == null && !suppressException)
				throw new ArgumentException(string.Format("Invalid data member '{0}'", dataMember));
			return dataBrowser;
		}
		protected virtual void AddToHashtable(object dataSource, string dataMember, DataBrowser dataBrowser) {
			browserDictionary.Add(new HashObj(dataSource, dataMember), dataBrowser);
		}
		DataBrowser CreateDataBrowser(object dataSource) {
			return dataSource != null && ListTypeHelper.IsListType(dataSource.GetType()) ?
				CreateListBrowser(dataSource, CreateListCotroller()) :
				new DataBrowser(dataSource, SuppressListFilling);
		}
		protected virtual ListBrowser CreateListBrowser(object dataSource, ListControllerBase listController) {
			return new ListBrowser(dataSource, listController, SuppressListFilling);
		}
		DataBrowser CreateDataBrowser(DataBrowser parent, PropertyDescriptor prop) {
			return ListTypeHelper.IsListType(prop.PropertyType) ?
					CreateRelatedListBrowser(parent, prop, CreateListCotroller()) as DataBrowser :
					new RelatedDataBrowser(parent, prop, SuppressListFilling);
		}
		protected virtual RelatedListBrowser CreateRelatedListBrowser(DataBrowser parent, PropertyDescriptor prop, ListControllerBase listController) {
			return new RelatedListBrowser(parent, prop, listController, SuppressListFilling);
		}
		protected virtual bool ShouldExpand(DataBrowser dataBrowser) {
			return dataBrowser is ListBrowser;
		}
	}
	public class DataBrowser {
		static readonly object notSet = new object();
		bool isClosed;
		object dataSource = notSet;
		bool isValidDataSource;
		protected bool suppressListFilling;
		protected bool IsValidDataSource {
			get { return isValidDataSource; }
		}
		public virtual object DataSource {
			get {
				if(!isValidDataSource)
					SetDataSource(RetrieveDataSource());
				return dataSource;
			}
		}
		protected bool DataSourceIsSet {
			get {
				return dataSource != notSet;
			}
		}
		public virtual Object Current { get { return this.DataSource; } }
		public virtual int Position { get { return 0; } set { } }
		public virtual int Count { get { return 1; } }
		public virtual Type DataSourceType { get { return this.DataSource.GetType(); } }
		public bool IsClosed { get { return isClosed; } }
		public bool HasLastPosition { get { return Position + 1 >= Count; } }
		public virtual DataBrowser Parent { get { return null; } }
		protected DataBrowser() {
		}
		protected DataBrowser(bool suppressListFilling) {
			this.suppressListFilling = suppressListFilling;
		}
		internal DataBrowser(Object dataSource, bool suppressListFilling)
			: this(suppressListFilling) {
			SetDataSource(dataSource);
		}
		internal DataBrowser(Object dataSource)
			: this(dataSource, false) {
		}
		#region events
		protected EventHandler onCurrentChangedHandler;
		protected EventHandler onPositionChangedHandler;
		public event EventHandler CurrentChanged {
			add {
				onCurrentChangedHandler += value;
			}
			remove {
				onCurrentChangedHandler -= value;
			}
		}
		public event EventHandler PositionChanged {
			add {
				onPositionChangedHandler += value;
			}
			remove {
				onPositionChangedHandler -= value;
			}
		}
		#endregion
		public virtual object SaveState() {
			return null;
		}
		public virtual void LoadState(object state) {
		}
		protected virtual object RetrieveDataSource() {
			return dataSource;
		}
		protected void InvalidateDataSource() {
			isValidDataSource = false;
		}
		protected internal virtual void Close() {
			isClosed = true;
			isValidDataSource = true;
			dataSource = null;
		}
		public virtual object GetValue() {
			return null;
		}
		public virtual string GetListName() {
			return "";
		}
		protected internal virtual string GetListName(PropertyDescriptorCollection listAccessors) {
			return "";
		}
		public virtual PropertyDescriptorCollection GetItemProperties() {
			return GetItemProperties(null);
		}
		public PropertyDescriptor[] GetPropertyPath(string path) {
			List<PropertyDescriptor> result = new List<PropertyDescriptor>();
			PropertyDescriptorCollection properties = GetItemProperties();
			string[] members = path.Split('.');
			for(int i = 0; i < members.Length; i++) {
				PropertyDescriptor property = properties.Find(members[i], true);
				if(property == null)
					return null;
				result.Add(property);
				properties = GetItemProperties(result.ToArray());
			}
			return result.ToArray();
		}
		internal virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return new DataBrowserHelper(suppressListFilling).GetListItemProperties(this.DataSource, listAccessors);
		}
		public PropertyDescriptor FindItemProperty(string name, bool ignoreCase) {
			return GetItemProperties().Find(name, ignoreCase);
		}
		protected virtual void SetDataSource(object value) {
			dataSource = value;
			isValidDataSource = true;
		}
		protected virtual void OnCurrentChanged(EventArgs e) {
			if(onCurrentChangedHandler != null)
				onCurrentChangedHandler(this, e);
		}
		public void RaiseCurrentChanged() {
			OnCurrentChanged(EventArgs.Empty);
		}
		protected virtual bool IsStandartType(Type propType) {
			return propType == typeof(object);
		}
		protected object GetPropertyValue(PropertyDescriptor prop, object obj) {
			return obj != null ? prop.GetValue(obj) : null;
		}
		public void EnumParents(Predicate<DataBrowser> predicate) {
			DataBrowser parent = this.Parent;
			while(parent != null && predicate(parent)) {
				parent = parent.Parent;
			}
		}
		public DataBrowser GetRootBrowser() {
			DataBrowser value = this;
			while(value.Parent != null)
				value = value.Parent;
			return value;
		}
	}
	public interface IRelatedDataBrowser {
		PropertyDescriptor RelatedProperty { get; }
		IRelatedDataBrowser Parent { get; }
	}
	public class RelatedDataBrowser : DataBrowser, IRelatedDataBrowser {
		PropertyDescriptor prop;
		DataBrowser parent;
		public override DataBrowser Parent { get { return parent; } }
		public override Type DataSourceType { get { return prop.PropertyType; } }
		PropertyDescriptor IRelatedDataBrowser.RelatedProperty { get { return prop; } }
		IRelatedDataBrowser IRelatedDataBrowser.Parent { get { return Parent as IRelatedDataBrowser; } }
		internal RelatedDataBrowser(DataBrowser parent, PropertyDescriptor prop, bool suppressListFilling)
			: base(suppressListFilling) {
			if(parent == null || prop == null)
				throw new ArgumentNullException();
			this.parent = parent;
			this.prop = prop;
			if(!suppressListFilling) {
				parent.PositionChanged += new EventHandler(OnParentStateChanged);
				parent.CurrentChanged += new EventHandler(OnParentStateChanged);
			}
		}
		public override object SaveState() {
			return IsValidDataSource ? DataSource : null;
		}
		public override void LoadState(object state) {
			if(state != null)
				SetDataSource(state);
			else
				InvalidateDataSource();
		}
		public override object GetValue() {
			return RetrieveDataSource();
		}
		protected override object RetrieveDataSource() {
			return GetPropertyValue(prop, parent.Current);
		}
		protected internal override void Close() {
			if(IsClosed)
				return;
			parent.PositionChanged -= new EventHandler(OnParentStateChanged);
			parent.CurrentChanged -= new EventHandler(OnParentStateChanged);
			base.Close();
		}
		internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			PropertyDescriptor[] descriptorArray;
			if(listAccessors != null && listAccessors.Length > 0) {
				descriptorArray = new PropertyDescriptor[listAccessors.Length + 1];
				listAccessors.CopyTo(descriptorArray, 1);
			} else
				descriptorArray = new PropertyDescriptor[1];
			descriptorArray[0] = this.prop;
			return this.parent.GetItemProperties(descriptorArray);
		}
		public override string GetListName() {
			string listName = this.GetListName(new PropertyDescriptorCollection(new PropertyDescriptor[] { }));
			if(!string.IsNullOrEmpty(listName))
				return listName;
			return base.GetListName();
		}
		protected internal override string GetListName(PropertyDescriptorCollection listAccessors) {
			listAccessors.Insert(0, this.prop);
			return this.parent.GetListName(listAccessors);
		}
		private void OnParentStateChanged(object sender, EventArgs e) {
			InvalidateDataSource();
			OnCurrentChanged(EventArgs.Empty);
		}
	}
}
