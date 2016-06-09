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
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using DevExpress.Utils;
using System.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
namespace DevExpress.Data.Browsing {
	public interface IListController {
		int Count { get; }
		object GetItem(int index);
		object GetColumnValue(int position, string columnName);
	}
	public interface IFilteredListController : IListController {
		DevExpress.Data.Filtering.CriteriaOperator FilterCriteria { get; set; }
	}
	public class ListControllerBase : IListController {
		protected IList list;
		public virtual int Count {
			get { return list != null ? list.Count : 0; }
		}
		public virtual object GetColumnValue(int position, string columnName) {
			return null;
		}
		public virtual object GetItem(int index) {
			return list[index];
		}
		protected internal virtual void SetList(IList list) {
			this.list = list;
		}
	}
	public class ListBrowser : DataBrowser {
		static readonly object nullObject = new object();
		IList list;
		int position;
		object current = nullObject;
		ListControllerBase listController;
		public override int Position {
			get { return IsValidDataSource ? position : 0; }
			set {
				int oldPosition = position;
				SetPositionCore(value);
				if(oldPosition != position)
					OnPositionChanged(EventArgs.Empty);
			}
		}
		public override int Count {
			get { return List != null ? listController.Count : 0; }
		}
		public override Object Current {
			get {
				if(Count > 0) {
					if(position < 0 || position >= Count)
						throw new IndexOutOfRangeException();
					if(current == nullObject)
						current = listController.GetItem(position);
					return current;
				}
				return null;
			}
		}
		public IList List {
			get {
				if(IsClosed)
					return null;
				object ignore = DataSource;
				return list;
			}
		}
		public ListControllerBase ListController { get { return listController; } }
		protected ListBrowser(ListControllerBase listController, bool suppressListFilling) : base(suppressListFilling) {
			if(listController == null)
				throw new ArgumentNullException("listController");
			this.listController = listController;
		}
		public ListBrowser(Object dataSource, ListControllerBase listController, bool suppressListFilling)
			: this(listController, suppressListFilling) {
			SetDataSource(dataSource);
		}
		protected void SetPositionCore(int value) {
			position = ValidatePosition(value);
			current = nullObject;
		}
		int ValidatePosition(int value) {
			return value == 0 || this.Count == 0 ? 0 :
					Math.Max(0, Math.Min(value, this.Count - 1));
		}
		protected internal override void Close() {
			if(IsClosed)
				return;
			SetList(null);
			base.Close();
		}
		protected override void SetDataSource(object value) {
			if(suppressListFilling && value is IListSource && value is IQueryable) {
				IList suppressedList = FakedListCreator.CreateGenericList(value);
				if(suppressedList != null) 
					value = suppressedList;
			} 
			if(value is IListSource)
				value = ((IListSource)value).GetList();
			if(value == DBNull.Value)
				value = null;
			if(value != null && !(value is IList))
				value = FakedListCreator.CreateFakedList(value);
			if(value != null && !(value is IList))
				throw new ArgumentException("value");
			base.SetDataSource(value);
			SetList((IList)value);
		}
		void SetList(IList list) {
			if(this.list != list) {
				this.list = list;
				listController.SetList(list);
				Position = 0;
			}
		}
		public override object SaveState() {
			return IsValidDataSource ? position : 0;
		}
		public override void LoadState(object state) {
			SetPositionCore((int)state);
		}
		public override string GetListName() {
			if(this.List is ITypedList)
				return ((ITypedList)this.List).GetListName(null);
			return this.DataSourceType.Name;
		}
		protected internal override string GetListName(PropertyDescriptorCollection listAccessors) {
			if(this.List is ITypedList) {
				PropertyDescriptor[] array = new PropertyDescriptor[listAccessors.Count];
				listAccessors.CopyTo(array, 0);
				return ((ITypedList)this.List).GetListName(array);
			}
			return "";
		}
		PropertyDescriptor[] ToArray(PropertyDescriptorCollection propCollection) {
			if(propCollection == null)
				return null;
			PropertyDescriptor[] items = new PropertyDescriptor[propCollection.Count];
			propCollection.CopyTo(items, 0);
			return items;
		}
		protected virtual void OnPositionChanged(EventArgs e) {
			if(onPositionChangedHandler != null)
				onPositionChangedHandler(this, e);
		}
		public object GetColumnValue(int position, string columnName) {
			return this.ListController.GetColumnValue(Math.Max(0, Math.Min(position, this.Count - 1)), columnName);
		}
		public object GetRow(int position) {
			return this.ListController.GetItem(Math.Max(0, Math.Min(position, this.Count - 1)));
		}
	}
	public class RelatedListBrowser : ListBrowser, IRelatedDataBrowser {
		IDisposable disposableListItem;
		DataBrowser parent;
		PropertyDescriptor listAccessor;
		public override DataBrowser Parent { get { return parent; } }
		public override Type DataSourceType { get { return listAccessor.PropertyType; } }
		PropertyDescriptor IRelatedDataBrowser.RelatedProperty { get { return listAccessor; } }
		IRelatedDataBrowser IRelatedDataBrowser.Parent { get { return Parent as IRelatedDataBrowser; } }
		public override object DataSource {
			get {
				if(!DataSourceIsSet)
					InitializeDataSource(parent as ListBrowser);
				return base.DataSource;
			}
		}
		public RelatedListBrowser(DataBrowser parent, PropertyDescriptor listAccessor, ListControllerBase listController, bool suppressListFilling)
			: base(listController, suppressListFilling) {
			if(parent == null)
				throw new ArgumentNullException("parent");
			if(listAccessor == null || !ListTypeHelper.IsListType(listAccessor.PropertyType))
				throw new ArgumentException("listAccessor");
			this.parent = parent;
			this.listAccessor = listAccessor;
			if(!suppressListFilling) {
				parent.PositionChanged += new EventHandler(OnParentStateChanged);
				parent.CurrentChanged += new EventHandler(OnParentStateChanged);
				InitializeDataSource(parent as ListBrowser);
			}
		}
		public override object SaveState() {
			return this.IsValidDataSource ? new object[] { DataSource, Position } : null;
		}
		public override void LoadState(object state) {
			object[] pair = state as object[];
			if(pair != null) {
				SetDataSource(pair[0]);
				SetPositionCore((int)pair[1]);
			} else
				InvalidateDataSource();
		}
		void InitializeDataSource(ListBrowser parentBrowser) {
			if(parentBrowser != null && parentBrowser.Count == 0 && !(parentBrowser.List is ITypedList)) {
				IBindingList list = parentBrowser.List as IBindingList;
				object listItem = AddNewItemIfEmpty(list);
				if(listItem != null) {
					try {
						SetDataSource(GetPropertyValue(listAccessor, listItem));
						return;
					} catch {
					} finally {
						IEditableObject editable = listItem as IEditableObject;
						if(editable != null) {
							editable.BeginEdit();
							editable.CancelEdit();
						}
						ICancelAddNew cancelAddNew = list as ICancelAddNew;
						if(cancelAddNew != null)
							cancelAddNew.CancelNew(0);
						if(list.Count > 0 && list.AllowRemove)
							list.Remove(listItem);
						disposableListItem = listItem as IDisposable;
					}
				}
			}
			SetDataSource(RetrieveDataSource());
		}
		object AddNewItemIfEmpty(IBindingList list) {
			try {
				if(list != null && list.Count == 0 && list.AllowNew)
					return list.AddNew();
			} catch {
			}
			return null;
		}
		protected internal override void Close() {
			if(IsClosed)
				return;
			if(disposableListItem != null) {
				disposableListItem.Dispose();
				disposableListItem = null;
			}
			parent.PositionChanged -= new EventHandler(OnParentStateChanged);
			parent.CurrentChanged -= new EventHandler(OnParentStateChanged);
			base.Close();
		}
		public override object GetValue() {
			return RetrieveDataSource();
		}
		protected override object RetrieveDataSource() {
			return GetPropertyValue(listAccessor, parent.Current);
		}
		public override string GetListName() {
			return GetListName(new PropertyDescriptorCollection(null));
		}
		protected internal override string GetListName(PropertyDescriptorCollection listAccessors) {
			listAccessors.Insert(0, listAccessor);
			return parent.GetListName(listAccessors);
		}
		internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			PropertyDescriptor[] descriptorArray = listAccessors == null ? 
				new PropertyDescriptor[] { listAccessor } :
				ArrayHelper.InsertItem<PropertyDescriptor>(listAccessors, listAccessor, 0);
			return this.parent.GetItemProperties(descriptorArray);
		}
		private void OnParentStateChanged(object sender, EventArgs e) {
			InvalidateDataSource();
			OnCurrentChanged(EventArgs.Empty);
		}
	}
}
