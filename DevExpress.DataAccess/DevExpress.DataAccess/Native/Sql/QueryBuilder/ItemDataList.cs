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
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	public class ItemDataList<T> : BindingList<T> {
		readonly QueryBuilderViewModel owner;
		protected ItemDataList(QueryBuilderViewModel owner) { this.owner = owner; }
		public void RemoveAll(Predicate<T> predicate) {
			for(int i = Count - 1; i >= 0; i--)
				if(predicate(this[i]))
					RemoveAt(i);
		}
		protected QueryBuilderViewModel Owner { get { return this.owner; } }
		void RaiseItemChanged(T item, PropertyDescriptor pd) {
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, IndexOf(item), pd));
		}
		protected void SetItemProperty(T item, bool value, PropertyDescriptor pd, Func<T, bool> getValue, Action<T, bool> setValue) {
			SetItemProperty(item, value, EqualityComparer<bool>.Default, pd, getValue, setValue);
		}
		protected void SetItemProperty(T item, string value, PropertyDescriptor pd, Func<T, string> getValue, Action<T, string> setValue) {
			SetItemProperty(item, value, StringComparer.Ordinal, pd, getValue, setValue);
		}
		protected void SetItemProperty<TVal>(T item, TVal value, IEqualityComparer<TVal> eqComparer, PropertyDescriptor pd, Func<T, TVal> getValue, Action<T, TVal> setValue) {
			if(eqComparer.Equals(getValue(item), value))
				return;
			Owner.BeginUpdate(false);
			try {
				setValue(item, value);
				RaiseItemChanged(item, pd);
			}
			finally { Owner.EndUpdate(false); }
		}
	}
}
