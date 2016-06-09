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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Xpo;
namespace DevExpress.Persistent.Base.ReportsV2 {
	[ListBindable(BindableSupport.No)]
	public sealed class SortingCollection : IList<SortProperty>, IList, IXtraSerializable2 {
		List<SortProperty> list = new List<SortProperty>();
		public SortingCollection(params SortProperty[] sortProperties) {
			if(sortProperties != null)
				AddRange(sortProperties);
		}
		public void AddRange(SortingCollection sortProperties) {
			foreach(SortProperty sp in sortProperties)
				if(sp != null)
					list.Add(sp);
			OnChanded();
		}
		public void AddRange(SortProperty[] sortProperties) {
			foreach(SortProperty sp in sortProperties)
				if(sp != null)
					list.Add(sp);
			OnChanded();
		}
		public void Add(SortingCollection sortProperties) {
			foreach(SortProperty sp in sortProperties)
				list.Add(sp);
			OnChanded();
		}
		public void Add(SortProperty sortProperty) {
			list.Add(sortProperty);
			OnChanded();
		}
		private void OnChanded() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		#region IList<SortProperty> Members
		public int IndexOf(SortProperty item) {
			return list.IndexOf(item);
		}
		public void Insert(int index, SortProperty item) {
			list.Insert(index, item);
			OnChanded();
		}
		public void RemoveAt(int index) {
			list.RemoveAt(index);
			OnChanded();
		}
		public SortProperty this[int index] {
			get {
				return list[index];
			}
			set {
				list[index] = value;
				OnChanded();
			}
		}
		#endregion
		#region ICollection<SortProperty> Members
		public void Clear() {
			list.Clear();
			OnChanded();
		}
		public bool Contains(SortProperty item) {
			return list.Contains(item);
		}
		public void CopyTo(SortProperty[] array, int arrayIndex) {
			list.CopyTo(array, arrayIndex);
		}
		public int Count {
			get { return list.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(SortProperty item) {
			bool result = list.Remove(item);
			OnChanded();
			return result;
		}
		#endregion
		#region IEnumerable<SortProperty> Members
		public IEnumerator<SortProperty> GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
		#region IList Members
		public int Add(object value) {
			int pos = ((IList)list).Add(value);
			OnChanded();
			return pos;
		}
		public bool Contains(object value) {
			return ((IList)list).Contains(value);
		}
		public int IndexOf(object value) {
			return ((IList)list).IndexOf(value);
		}
		public void Insert(int index, object value) {
			((IList)list).Insert(index, value);
			OnChanded();
		}
		public bool IsFixedSize {
			get { return false; }
		}
		public void Remove(object value) {
			((IList)list).Remove(value);
			OnChanded();
		}
		object IList.this[int index] {
			get {
				return ((IList)list)[index];
			}
			set {
				((IList)list)[index] = value;
				OnChanded();
			}
		}
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			((IList)list).CopyTo(array, index);
		}
		public bool IsSynchronized {
			get { return false; }
		}
		public object SyncRoot {
			get { return ((IList)list).SyncRoot; }
		}
		#endregion
		public event EventHandler Changed;
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(IList props) {
			XtraPropertyInfo sortingPropertyInfo = FindSorting(props);
			if(sortingPropertyInfo != null && sortingPropertyInfo.ChildProperties != null) {
				DeserializeSorting(sortingPropertyInfo);
			}
		}
		private XtraPropertyInfo FindSorting(IList props) {
			foreach(object property in props) {
				XtraPropertyInfo propertyInfo = property as XtraPropertyInfo;
				if(propertyInfo != null && propertyInfo.Name == "Sorting") {
					return propertyInfo;
				}
			}
			return null;
		}
		private void DeserializeSorting(XtraPropertyInfo sortingPropertyInfo) {
			foreach(XtraPropertyInfo itemPropertyInfo in sortingPropertyInfo.ChildProperties) {
				SortProperty item = new SortProperty();
				item.PropertyName = FindPropertyName(itemPropertyInfo);
				item.Direction = FindDirection(itemPropertyInfo);
				list.Add(item);
			}
		}
		private string FindPropertyName(XtraPropertyInfo itemPropertyInfo) {
			foreach(XtraPropertyInfo propertyInfo in itemPropertyInfo.ChildProperties) {
				if(propertyInfo.Name == "PropertyName") {
					return propertyInfo.Value.ToString();
				}
			}
			return string.Empty;
		}
		private DevExpress.Xpo.DB.SortingDirection FindDirection(XtraPropertyInfo itemPropertyInfo) {
			foreach(XtraPropertyInfo propertyInfo in itemPropertyInfo.ChildProperties) {
				if(propertyInfo.Name == "Direction") {
					return (DevExpress.Xpo.DB.SortingDirection)Enum.Parse(typeof(DevExpress.Xpo.DB.SortingDirection), propertyInfo.Value.ToString());
				}
			}
			return DevExpress.Xpo.DB.SortingDirection.Descending;
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			XtraPropertyInfo rootPropertyInfo = new XtraPropertyInfo("Sorting", null, 0, true);
			foreach(SortProperty item in list) {
				XtraPropertyInfo itemProperty = SerializeSortProperty(item, rootPropertyInfo.ChildProperties.Count + 1);
				rootPropertyInfo.ChildProperties.Add(itemProperty);
			}
			rootPropertyInfo.Value = rootPropertyInfo.ChildProperties.Count;
			return new XtraPropertyInfo[] { rootPropertyInfo };
		}
		private XtraPropertyInfo SerializeSortProperty(SortProperty item, int itemIndex) {
			XtraPropertyInfo itemProperty = new XtraPropertyInfo("Item" + itemIndex.ToString(), null, 0, true);
			itemProperty.ChildProperties.Add(new XtraPropertyInfo("PropertyName", typeof(String), item.PropertyName));
			itemProperty.ChildProperties.Add(new XtraPropertyInfo("Direction", typeof(String), item.Direction.ToString()));
			return itemProperty;
		}
		#endregion
	}
}
