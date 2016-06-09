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
namespace DevExpress.XtraCharts.Native {
	public delegate T GetPropertyValue<T>();
	public delegate void SetPropertyValue<T>(T value);
	public class AspxSerializerWrapper<T> : IList where T : ChartElement {
		readonly GetPropertyValue<T> getter;
		readonly SetPropertyValue<T> setter;
		bool shouldSerialize = true;
		public AspxSerializerWrapper(GetPropertyValue<T> getter, SetPropertyValue<T> setter, bool shouldSerialize) : this(getter, setter) {
			this.shouldSerialize = shouldSerialize;
		}
		public AspxSerializerWrapper(GetPropertyValue<T> getter, SetPropertyValue<T> setter){
			this.getter = getter;
			this.setter = setter;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			ChartElement value = getter();
			if (value != null && value.ShouldSerialize())
				yield return value;
		}
		int ICollection.Count { get { return GetCount(); } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return null; } }
		void ICollection.CopyTo(Array array, int index) {
			array.SetValue(getter(), index);
		}
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return false; } }
		object IList.this[int index] { 
			get { return getter(); } 
			set { setter(value as T); }
		}
		bool IList.Contains(object value) {
			return Object.ReferenceEquals(value, getter());
		}
		int IList.IndexOf(object value) {
			return ((IList)this).Contains(value) ? 0 : -1;
		}
		int IList.Add(object value) {
			T val = value as T;
			if (val != null)
				setter(val);
			return 0;
		}
		void IList.Insert(int index, object value) {
		}
		void IList.Remove(object value) {
		}
		void IList.RemoveAt(int index) {
		}
		void IList.Clear() {
		}
		int GetCount() {
			ChartElement value = getter();
			return shouldSerialize && value != null && value.ShouldSerialize() ? 1 : 0;
		}
	}
}
