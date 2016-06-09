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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.Collections;
using DevExpress.Utils;
namespace DevExpress.Mvvm.UI.Native.ViewGenerator.Model {
	public class ModelItemCollectionBase : IModelItemCollection {
		protected readonly IEnumerable computedValue;
		protected readonly EditingContextBase context;
		protected readonly IModelItem parent;
		public ModelItemCollectionBase(EditingContextBase context, IEnumerable computedValue, IModelItem parent) {
			Guard.ArgumentNotNull(computedValue, "computedValue");
			Guard.ArgumentNotNull(context, "context");
			this.computedValue = computedValue;
			this.context = context;
			this.parent = parent;
		}
		IEnumerator<IModelItem> IEnumerable<IModelItem>.GetEnumerator() {
			return GetEnumerator();
		}
#if DEBUGTEST
		public static int debug_getEnumeratorCount = 0;
#endif
		IEnumerator<IModelItem> GetEnumerator() {
#if DEBUGTEST
			debug_getEnumeratorCount++;
#endif
			return computedValue.Cast<object>().Select(o => context.CreateModelItem(o, parent)).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		void IModelItemCollection.Add(IModelItem value) {
			((IModelItemCollection)this).Add(value.GetCurrentValue());
		}
		IModelItem IModelItemCollection.Add(object value) {
			IList list = computedValue as IList;
			if(list == null)
				throw new InvalidOperationException();
			object obj = value;
			IModelItem item = value as IModelItem;
			if (item != null) {
				obj = item.GetCurrentValue();
			} else {
				item = context.CreateModelItem(obj, parent);
			}
			list.Add(obj);
			OnChanged();
			return item;
		}
		public IModelItem this[int index] {
			get {
				IList list = computedValue as IList;
				if(list == null)
					throw new InvalidOperationException();
				return context.CreateModelItem(list[index], parent);
			}
			set {
				IList list = computedValue as IList;
				if(list == null)
					throw new InvalidOperationException();
				list[index] = value.GetCurrentValue();
			}
		}
		public void Clear() {
			IList list = computedValue as IList;
			if(list == null)
				throw new InvalidOperationException();
			list.Clear();
		}
		public int IndexOf(IModelItem item) {
			IList list = computedValue as IList;
			if(list == null)
				throw new InvalidOperationException();
			return list.IndexOf(item.GetCurrentValue());
		}
		public void Insert(int index, object value) {
			IList list = computedValue as IList;
			if(list == null)
				throw new InvalidOperationException();
			list.Insert(index, value);
		}
		public void Insert(int index, IModelItem valueItem) {
			Insert(index, valueItem.GetCurrentValue());
		}
		public void RemoveAt(int index) {
			IList list = computedValue as IList;
			if(list == null)
				throw new InvalidOperationException();
			list.RemoveAt(index);
		}
		bool IModelItemCollection.Remove(IModelItem item) {
			return ((IModelItemCollection)this).Remove(item.GetCurrentValue());
		}
		bool IModelItemCollection.Remove(object item) {
			IList list = computedValue as IList;
			if(list == null)
				throw new InvalidOperationException();
			object value = item is IModelItem ? ((IModelItem)item).GetCurrentValue() : item;
			if (list.Contains(value)) {
				list.Remove(value);
				OnChanged();
				return true;
			}
			return false;
		}
		protected virtual void OnChanged() { }
	}
}
