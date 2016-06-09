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
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DataAccess.Native.Data {
	public interface IPropertyDescriptorCollectionOwner {
		PropertyDescriptorCollection Properties { get; }
	}
	public class CachedPropertyDescriptorCollection : PropertyDescriptorCollection {
		readonly Dictionary<string, PropertyDescriptor> propertyByName = new Dictionary<string, PropertyDescriptor>();
		public override PropertyDescriptor this[string name] {
			get { return this.propertyByName[name]; }
		}
		public CachedPropertyDescriptorCollection(PropertyDescriptor[] properties) : base(properties) { }
		public void AddCached(PropertyDescriptor pd) {
			this.propertyByName[pd.Name] = pd;
			Add(pd);
		}
		public void ClearCached(PropertyDescriptor pd) {
			this.propertyByName.Clear();
			Clear();
		}
		public bool ContainsName(string name) {
			return this.propertyByName.ContainsKey(name);
		}
	}
	public class PropertiesRepository : IPropertyDescriptorCollectionOwner, IEnumerable {
		readonly CachedPropertyDescriptorCollection properties = new CachedPropertyDescriptorCollection(null);
		PropertyDescriptorCollection IPropertyDescriptorCollectionOwner.Properties { get { return this.properties; } }
		public PropertyDescriptor this[string name] {
			get { return this.properties[name]; }
		}
		public PropertyDescriptor this[int index] {
			get { return this.properties[index]; }
		}
		public int Count { get { return this.properties.Count; } }
		public void Add(PropertyDescriptor pd) {
			this.properties.AddCached(pd);
		}
		public void Clear() {
			this.properties.Clear();
		}
		public bool Contains(string name) {
			return this.properties.ContainsName(name);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)this.properties).GetEnumerator();
		}
	}
	public abstract class ReadOnlyTypedList : IList, ITypedList {
		readonly PropertiesRepository properties = new PropertiesRepository();
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return true; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return null; } }
		public PropertiesRepository Properties { get { return this.properties; } }
		public int Count { 
			get {
				EnsureData();
				return GetItemsCount();
			} 
		}
		public object this[int index] { 
			get {
				EnsureData();
				return GetItemValue(index); 
			} 
			set { throw new NotSupportedException(); } }
		int IList.Add(object value) {
			throw new NotSupportedException();
		}
		void IList.Clear() {
			throw new NotSupportedException();
		}
		bool IList.Contains(object value) {
			throw new NotSupportedException();
		}
		int IList.IndexOf(object value) {
			throw new NotSupportedException();
		}
		void IList.Insert(int index, object value) {
			throw new NotSupportedException();
		}
		void IList.Remove(object value) {
			throw new NotSupportedException();
		}
		void IList.RemoveAt(int index) {
			throw new NotSupportedException();
		}
		void ICollection.CopyTo(Array array, int index) {
			CopyToInternal(array, index);
		}
		protected virtual void CopyToInternal(Array array, int index) {
			throw new NotSupportedException();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			EnsureData();
			for(int i = 0; i < Count; i++)
				yield return this[i];
		}
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			EnsureData();
			if(listAccessors != null && listAccessors.Length > 0)
				return GetItemPropertiesByListAccessors(listAccessors);
			return ((IPropertyDescriptorCollectionOwner)this.properties).Properties;
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return GetType().Name;
		}
		protected virtual void EnsureData() {			
		}
		protected virtual PropertyDescriptorCollection GetItemPropertiesByListAccessors(PropertyDescriptor[] listAccessors) {
			return new PropertyDescriptorCollection(null);
		}
		protected abstract object GetItemValue(int index);
		protected abstract int GetItemsCount();
	}
}
