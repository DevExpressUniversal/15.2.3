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

using DevExpress.Utils;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DataAccess.Native.ObjectBinding {
	public class ResultTypedList : ITypedList, IList {
		readonly string name;
		readonly PropertyDescriptorCollection pdc;
		readonly ITypedList typedList;
		readonly IList innerList;
		public ResultTypedList(string name, ITypedList typedList, IEnumerable data) {
			Guard.ArgumentNotNull(data, "data");
			this.typedList = typedList;
			this.name = name;
			innerList =  data as IList ?? data.Cast<object>().ToList();
		}
		public ResultTypedList(string name, PropertyDescriptorCollection pdc) {
			this.pdc = pdc;
			this.name = name;
			innerList = new ArrayList(0);
		}
		public ResultTypedList(string name, PropertyDescriptorCollection pdc, IEnumerable data) {
			Guard.ArgumentNotNull(data, "data");
			this.pdc = pdc;
			this.name = name;
			innerList = data as IList ?? data.Cast<object>().ToList();
		}
		#region Implementation of ITypedList
		public string GetListName(PropertyDescriptor[] listAccessors) {
			if(listAccessors == null || listAccessors.Length == 0)
				return this.name;
			ITypedList child = GetList(listAccessors[0]);
			if(child == null)
				return null;
			return child.GetListName(listAccessors.Skip(1).ToArray());
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(this.typedList != null)
				return this.typedList.GetItemProperties(listAccessors);
			if(listAccessors == null || listAccessors.Length == 0)
				return this.pdc;
			ITypedList child = GetList(listAccessors[0]);
			if(child == null)
				return PropertyDescriptorCollection.Empty;
			return child.GetItemProperties(listAccessors.Skip(1).ToArray());
		}
		#endregion
		ITypedList GetList(PropertyDescriptor pd) {
			if(typeof(ITypedList).IsAssignableFrom(pd.PropertyType))
				return GetNestedList(pd);
			return ObjectDataSourceFillHelper.CreateTypedList(pd.PropertyType, pd.Name);
		}
		ITypedList GetNestedList(PropertyDescriptor pd) {
			object instance;
			if(Count > 0)
				instance = this[0];
			else {
				Type itemType = pd.ComponentType;
				ConstructorInfo ctor = itemType.GetConstructor(new Type[0]);
				if(ctor == null)
					return null;
				instance = ctor.Invoke(new object[0]);
			}
			object value = pd.GetValue(instance);
			ITypedList child = value as ITypedList;
			if(child == null)
				return null;
			return child;
		}
		#region Implementation of IList
		public bool IsFixedSize { get { return true; } }
		public bool IsReadOnly { get { return true; } }
		public object this[int index] { get { return innerList[index]; } set { throw new NotSupportedException(); } }
		public int Add(object value) { throw new NotSupportedException(); }
		public void Clear() { throw new NotSupportedException(); }
		public bool Contains(object value) { return innerList.Contains(value); }
		public int IndexOf(object value) { return innerList.IndexOf(value); }
		public void Insert(int index, object value) { throw new NotSupportedException(); }
		public void Remove(object value) { throw new NotSupportedException(); }
		public void RemoveAt(int index) { throw new NotSupportedException(); }
		#endregion
		#region Implementation of ICollection
		public int Count { get { return innerList.Count; } }
		public bool IsSynchronized { get { return false; } }
		public object SyncRoot { get { return null; } }
		public void CopyTo(Array array, int index) { innerList.CopyTo(array, index); }
		#endregion
		#region Implementation of IEnumerable
		public IEnumerator GetEnumerator() { return innerList.GetEnumerator(); }
		#endregion
	}
}
