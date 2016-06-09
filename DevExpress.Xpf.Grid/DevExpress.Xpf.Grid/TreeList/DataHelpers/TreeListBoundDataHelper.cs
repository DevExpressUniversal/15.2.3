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
using DevExpress.Data.Helpers;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data;
using System.Collections.Generic;
using DevExpress.Xpf.Data;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
using HierarchicalDataTemplate = DevExpress.Xpf.Core.HierarchicalDataTemplate;
#else
using System.Windows.Forms;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public abstract class TreeListBoundDataHelper : TreeListDataHelperBase {
		public override Type ItemType { get { return ListBindingHelper.GetListItemType(ListSource); } }
		public override bool IsReady { get { return ListSource != null; } }
		public override bool IsLoaded { get { return IsReady && ListSource.Count > 0; } }
		public override bool AllowEdit { get { return BindingList == null || BindingList.AllowEdit; } }
		public override bool AllowRemove { get { return BindingList == null || BindingList.AllowRemove; } }
		public IList ListSource { get; private set; }
		protected ICollectionView CollectionViewSource { get; private set; }
		protected ITypedList TypedList { get { return ListSource as ITypedList; } }
		protected virtual IBindingList BindingList { get; set; }
		public TreeListBoundDataHelper(TreeListDataProvider provider, object dataSource) : base(provider) {
			CollectionViewSource = dataSource as ICollectionView;
			ListSource = ExtractListSource(dataSource);
			BindingList = ListSource as IBindingList;
		}
		protected internal override bool SupportNotifications { get { return BindingList != null;  } }
		protected virtual IList ExtractListSource(object dataSource) {
			if(dataSource is ICollectionView)
				return DataBindingHelper.ExtractDataSourceFromCollectionView((ICollectionView)dataSource);
			return DataBindingHelper.ExtractDataSource(dataSource);
		}
		public override void PopulateColumns() {
			if(!IsReady) return;
			PropertyDescriptorCollection properties = GetPropertyDescriptorCollection();
			if(properties != null && DataProvider.CanUseFastPropertyDescriptors)
				properties = DevExpress.Data.Access.DataListDescriptor.GetFastProperties(properties);
			if(properties != null) {
				foreach(PropertyDescriptor descriptor in properties)
					PopulateColumn(GetActualPropertyDescriptor(descriptor));
			}
			ComplexColumnInfoCollection complex = DataProvider.GetComplexColumns();
			if(complex != null) {
				foreach(ComplexColumnInfo complexColumn in complex)
					PopulateColumn(GetActualComplexPropertyDescriptor(DataProvider, GetFirstItem(), complexColumn.Name));
			}
			PatchColumnCollection(properties);
			PopulateUnboundColumns();
		}
		protected override bool IsColumnVisible(DataColumnInfo column) {
			return base.IsColumnVisible(column) && (View.AutoPopulateServiceColumns || IsColumnsVisibleCore(column));
		}
		protected virtual bool IsColumnsVisibleCore(DataColumnInfo column) {
			return ((column.Name != View.ImageFieldName && column.Name != View.CheckBoxFieldName));
		}
		protected virtual PropertyDescriptor GetActualComplexPropertyDescriptor(TreeListDataProvider provider, object obj, string name) {
			return new TreeListComplexPropertyDescriptor(provider, obj, name);
		}
		protected virtual PropertyDescriptor GetActualPropertyDescriptor(PropertyDescriptor descriptor) {
			return descriptor;
		}
		protected virtual PropertyDescriptorCollection GetPropertyDescriptorCollection() {
			if(ListSource == null)
				return null;
			if(TypedList != null) return TypedList.GetItemProperties(null);
			Type itemType = GetListItemPropertyType(ListSource);
			if(itemType != null && !DevExpress.Data.Access.ExpandoPropertyDescriptor.IsDynamicType(itemType)) {
				if(DataProvider.UseFirstRowTypeWhenPopulatingColumns(itemType) && ListSource.Count > 0)
					return TypeDescriptor.GetProperties(ListSource[0].GetType());
				if(itemType.Equals(typeof(string)) || itemType.IsPrimitive || itemType.IsDefined(typeof(DevExpress.Data.Access.DataPrimitiveAttribute), true))
					return CreateSimplePropertyDescriptor();
				if(CollectionViewSource != null) {
					PropertyDescriptorCollection properties = GetIItemProperties(CollectionViewSource);
					if(properties != null)
						return properties;
				}
				return TypeDescriptor.GetProperties(itemType);
			}
			object item = GetFirstItem();
			if(item != null) {
				itemType = item.GetType();
				if(DevExpress.Data.Access.ExpandoPropertyDescriptor.IsDynamicType(itemType))
					return GetExpandoObjectProperties(item);
				return TypeDescriptor.GetProperties(item);
			}
			return null;
		}
		PropertyDescriptorCollection GetExpandoObjectProperties(object item) {
			if(item == null) return null;
			IDictionary<string, object> properties = item as IDictionary<string, object>;
			if(properties == null) return null;
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach(KeyValuePair<string, object> pair in properties) 
				list.Add(new DevExpress.Data.Access.ExpandoPropertyDescriptor(null, pair.Key, pair.Value == null ? null : pair.Value.GetType()));
			return new PropertyDescriptorCollection(list.ToArray());
		}
		protected PropertyDescriptorCollection GetIItemProperties(object source) {
#if !SL
			IItemProperties itemProperties = source as IItemProperties;
			if(itemProperties == null)
				return null;
			IList<ItemPropertyInfo> propertyInfos = itemProperties.ItemProperties;
			if(propertyInfos == null)
				return null;
			List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>();
			foreach(ItemPropertyInfo item in propertyInfos)
				if(item.Descriptor is PropertyDescriptor)
					descriptors.Add((PropertyDescriptor)item.Descriptor);
			return new PropertyDescriptorCollection(descriptors.ToArray());
#else
			return null;
#endif
		}
		protected Type GetListItemPropertyType(IList list) {
			return DataProviderBase.GetListItemPropertyType(list, CollectionViewSource);
		}
		PropertyDescriptorCollection CreateSimplePropertyDescriptor() {
			return new PropertyDescriptorCollection(new PropertyDescriptor[] { new DevExpress.Data.Access.SimpleListPropertyDescriptor() });
		}
		protected object GetFirstItem() {
			return (ListSource != null && ListSource.Count > 0) ? ListSource[0] : null;
		}
		protected virtual void PatchColumnCollection(PropertyDescriptorCollection properties) { }
		public override void LoadData() { }
		public override void Dispose() {
			base.Dispose();
			DataProvider.Nodes.Clear();
			CollectionViewSource = null;
			ListSource = null;
		}
	}
}
