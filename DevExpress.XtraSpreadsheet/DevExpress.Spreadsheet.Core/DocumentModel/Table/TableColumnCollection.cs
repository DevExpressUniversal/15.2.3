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
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region TableColumnInfoCollection
	public class TableColumnInfoCollection : DXNamedItemCollection<TableColumn>, IDisposable {
		public TableColumnInfoCollection() {
		}
		protected override string GetItemName(TableColumn item) {
			return item.Name;
		}
		protected internal virtual void OnNameChanged(object sender, NameChangedEventArgs e) {
			TableColumn column = this[e.OldName];
			Debug.Assert(column.Name == e.Name);
			NameHash.Remove(e.OldName);
			NameHash.Add(e.Name, column);
		}
		#region Insert/Add column
		protected override void OnInsertComplete(int index, TableColumn value) {
			value.NameChanged += OnNameChanged;
			value.DocumentModel.InternalAPI.OnTableColumnAdd(value, index);
			base.OnInsertComplete(index, value);
		}
		#endregion
		#region Remove column
		protected override void OnRemoveComplete(int index, TableColumn value) {
			value.NameChanged -= OnNameChanged;
			value.DocumentModel.InternalAPI.OnTableColumnRemoveAt(value.Table, index);
			base.OnRemoveComplete(index, value);
		}
		protected override bool OnClear() {
			bool result = base.OnClear();
			if (!result)
				return false;
			foreach (TableColumn column in this) {
				column.NameChanged -= OnNameChanged;
			}
			return true;
		}
		#endregion
		public override int Add(TableColumn value) {
			return AddCore(value);
		}
		protected override void InsertIfNotAlreadyInserted(int index, TableColumn obj) {
			InsertCore(index, obj);
		}
		#region IDisposable Members
		public void Dispose() {
			Clear();
		}
		#endregion
		public List<string> GetNamesList() {
			List<string> result = new List<string>(InnerList.Count);
			foreach (TableColumn tableColumn in InnerList)
				result.Add(tableColumn.Name);
			return result;
		}
		internal bool ContainsName(string name) {
			return NameHash.ContainsKey(name);
		}
		internal void SubscribeOnNameChanged(TableColumn column) {
			column.NameChanged += OnNameChanged;
		}
		internal void UnsubscribeOnNameChanged(TableColumn column) {
			column.NameChanged -= OnNameChanged;
		}
		internal void RemoveHashKey(TableColumn column) {
			NameHash.Remove(column.Name);
		}
		internal void AddHashKey(TableColumn column) {
			NameHash.Add(column.Name, column);
		}
	}
	#endregion
}
