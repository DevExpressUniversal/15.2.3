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

using System.Collections;
using System.ComponentModel;
using DevExpress.Data;
using System.Collections.Generic;
using System;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraReports.Native.Parameters {
	public class ParametersDataSource : IList, ITypedList {
		IEnumerable<IParameter> parameters;
		List<object> innerList;
		string listName;
		public IEnumerable<IParameter> Parameters {
			get { return parameters; }
		}
		public ParametersDataSource(IEnumerable<IParameter> parameters, string listName) {
			this.parameters = parameters;
			innerList = new List<object>(new object[] { parameters });
			this.listName = listName;
		}
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			if (listAccessors != null && listAccessors.Length > 0) {
				return new PropertyDescriptorCollection(null);
			}
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
			foreach (IParameter item in parameters)
				properties.Add(new ParameterPropertyDescriptor(item));
			return new PropertyDescriptorCollection(properties.ToArray());
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return listName;
		}
		#endregion
		#region IList Members
		int IList.Add(object value) {
			return -1;
		}
		void IList.Clear() {
		}
		bool IList.Contains(object value) {
			return innerList.Contains(value);
		}
		int IList.IndexOf(object value) {
			return innerList.IndexOf(value);
		}
		void IList.Insert(int index, object value) {
		}
		bool IList.IsFixedSize {
			get { return true; }
		}
		bool IList.IsReadOnly {
			get { return true; }
		}
		void IList.Remove(object value) {
		}
		void IList.RemoveAt(int index) {
		}
		object IList.this[int index] {
			get {
				return innerList[index];
			}
			set {
			}
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			ICollection thisCollection = innerList;
			thisCollection.CopyTo(array, index);
		}
		int ICollection.Count {
			get { return innerList.Count; }
		}
		bool ICollection.IsSynchronized {
			get {
				ICollection thisCollection = innerList;
				return thisCollection.IsSynchronized;
			}
		}
		object ICollection.SyncRoot {
			get {
				ICollection thisCollection = innerList;
				return thisCollection.SyncRoot;
			}
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
	}
}
