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
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.Collections;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Grid {
	public interface IColumnCollection : IList, INotifyCollectionChanged, ILockable, ISupportGetCachedIndex<ColumnBase> {
		void OnColumnsChanged();
		DataControlBase Owner { get; }
		ColumnBase this[string fieldName] { get; }
		new ColumnBase this[int index] { get; }
		ColumnBase GetColumnByName(string name);
	}
	public abstract class ColumnCollectionBase<TColumn> : ObservableCollectionCore<TColumn>, IColumnCollection where TColumn : ColumnBase {
		readonly DataControlBase owner;
		readonly Dictionary<string, TColumn> fieldsNameCache = new Dictionary<string, TColumn>();
#if DEBUGTEST
		internal Dictionary<string, TColumn> FieldsNameCacheInternal { get { return this.fieldsNameCache; } }
#endif
		Dictionary<string, TColumn> FieldsNameCache { get { return fieldsNameCache; } }
		public ColumnCollectionBase(DataControlBase owner) {
			this.owner = owner;
		}
		public TColumn this[string fieldName] {
			get { return GetColumnByFieldName(fieldName); }
		}
		public TColumn GetColumnByName(string name) {
			return ListHelper.Find<TColumn>(this, (column) => column.Name == name);
		}
		public TColumn GetColumnByFieldName(string fieldName) {
			if(String.IsNullOrEmpty(fieldName) || (this.Count == 0))
				return null;
			TColumn column;
			if(!fieldsNameCache.TryGetValue(fieldName, out column))
				foreach(TColumn col in this) {
					if(col.FieldName == fieldName) {
						fieldsNameCache.Add(fieldName, col);
						return col;
					}
				}
			return column;
		}
		protected void ResetColumnsByFieldsNameCache() {
			fieldsNameCache.Clear();
		}
		protected override void InsertItem(int index, TColumn item) {
			OnInsertItem(item);
			owner.OnColumnAdding(item);
			base.InsertItem(index, item);
			owner.OnColumnAdded(item);
		}
		protected override void RemoveItem(int index) {
			TColumn column = this[index];
			OnRemoveItem(column);
			base.RemoveItem(index);
			owner.OnColumnRemoved(column);
			ResetColumnsByFieldsNameCache();
		}
		protected override void ClearItems() {
			for(int i = 0; i < Count; i++) {
				OnRemoveItem(this[i]);
			}
			base.ClearItems();
			ResetColumnsByFieldsNameCache();
		}
		protected virtual void OnRemoveItem(TColumn column) {
			column.ClearBindingValues();
			column.OwnerControl = null;
			owner.RemoveChild(column);
		}
		protected virtual void OnInsertItem(TColumn column) {
			owner.AddChild(column);
			column.OwnerControl = owner;
		}
		public override void EndUpdate() {
			base.EndUpdate();
			owner.OnColumnCollectionEndUpdate();
		}
		DataControlBase IColumnCollection.Owner { get { return owner; } }
		void IColumnCollection.OnColumnsChanged() {
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			ResetColumnsByFieldsNameCache();
		}
		ColumnBase IColumnCollection.this[string fieldName] { get { return this[fieldName]; } }
		ColumnBase IColumnCollection.this[int index] { get { return this[index]; } }
		ColumnBase IColumnCollection.GetColumnByName(string name) {
			return GetColumnByName(name);
		}
		int ISupportGetCachedIndex<ColumnBase>.GetCachedIndex(ColumnBase column) {
			return GetCachedIndex((TColumn)column);
		}
	}
	public class ColumnCollection : ColumnCollectionBase<ColumnBase> {
		public ColumnCollection(DataControlBase dataControl)
			: base(dataControl) {
		}
	}
}
