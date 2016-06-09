﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Data {
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class DataView : ReadOnlyTypedList {
		readonly SelectStatementResultRow[] dataRows;
		public SelectStatementResultRow[]  DataRows { get { return this.dataRows; } }
		public PropertiesRepository ColumnDescriptors { get { return Properties; } }
		public DataView(SelectStatementResultRow[] dataRows) {
			Guard.ArgumentNotNull(dataRows, "dataRows");
			this.dataRows = dataRows;
		}
		protected override object GetItemValue(int index) {
			return index >= 0 && index < Count ? this.dataRows[index] : null;
		}
		protected override int GetItemsCount() {
			return this.dataRows.Length;
		}
		public void AddColumn(string name, Type type) {
			ColumnDescriptors.Add(new ViewColumnPropertyDescriptor(name, type, ColumnDescriptors.Count));
		}
		public void AddColumn(string name, Type type, string displayName) {
			ColumnDescriptors.Add(new ViewColumnPropertyDescriptor(name, type, displayName, ColumnDescriptors.Count));
		}
		public bool ContainsColumnName(string name) {
			foreach (PropertyDescriptor pd in ColumnDescriptors) {
				if (pd.Name == name)
					return true;
			}
			return false;
		}
	}
	public class ViewColumnPropertyDescriptor : TypedPropertyDescriptor {
		readonly int index;
		public ViewColumnPropertyDescriptor(string name, Type type, int index)
			: base(name, type) {
			this.index = index;
		}
		public ViewColumnPropertyDescriptor(string name, Type type, string displayName, int index)
			: base(name, type, displayName) {
			this.index = index;
		}
		public override object GetValue(object component) {
			SelectStatementResultRow row = component as SelectStatementResultRow;
			return row == null ? null : row.Values[this.index];
		}
	}
}
