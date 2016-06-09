#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.DataProcessing {
	class InMemoryUnderlyingDataSet : IDashboardDataSet {
		class InMemoryUnderlyingDataRow : IDashboardDataRow {
			readonly object[] data;
			readonly int index;
			readonly InMemoryUnderlyingDataSet dataSet;
			public InMemoryUnderlyingDataRow(object[] data, int index, InMemoryUnderlyingDataSet dataSet) {
				this.data = data;
				this.index = index;
				this.dataSet = dataSet;
			}
			IDashboardDataSet IDashboardDataRow.DataSource {
				get { return dataSet; }
			}
			int IDashboardDataRow.Length {
				get { return data.Length; }
			}
			int IDashboardDataRow.Index {
				get { return index; }
			}
			int IDashboardDataRow.ListSourceRowIndex {
				get { throw new NotSupportedException(); }
			}
			object IDashboardDataRow.this[int index] {
				get {
					return data[index];
				}
			}
			object IDashboardDataRow.this[string columnName] {
				get {
					int index = -1;
					if(dataSet.ColumnByIndex.TryGetValue(columnName, out index))
						return data[index];
					return null;
				}
			}
			System.Collections.IList IDashboardDataRow.ToList() {
				return data;
			}
		}
		readonly Dictionary<string, int> columnByIndex = new Dictionary<string, int>();
		readonly List<IDashboardDataRow> rows = new List<IDashboardDataRow>();
		readonly PropertyDescriptorCollection propertyDescriptorCollection;
		public Dictionary<string, int> ColumnByIndex { get { return columnByIndex; } }
		public InMemoryUnderlyingDataSet(OperationGraph graph, IDictionary<SingleBlockOperation, DataVectorBase> res, IList<string> dataMembers, DataSourceModel model) {
			IEnumerator<object>[] enums = new IEnumerator<object>[dataMembers.Count];
			for(int i = 0; i < dataMembers.Count; i++) {
				enums[i] = res[graph.Output.Where(pair => pair.Key.DataItemID == dataMembers[i]).Single().Value].GetUntypedData().GetEnumerator();
				columnByIndex.Add(dataMembers[i], i);
			}
			int index = 0;
			int count = res.First().Value.Count;
			for(int i = 0; i < count; i++) {
				enums.All(e => e.MoveNext());
				object[] data = new object[dataMembers.Count];
				for(int j = 0; j < data.Length; j++)
					data[j] = enums[j].Current;
				rows.Add(new InMemoryUnderlyingDataRow(data, index++, this));
			}
			PropertyDescriptor[] props = new PropertyDescriptor[dataMembers.Count];
			for(int i = 0; i < dataMembers.Count; i++)
				props[i] = new InMemoryUnderlyingDataPropertyDescriptor(model.DataSourceInfo.DataSource.GetFieldSourceType(dataMembers[i], model.DataSourceInfo.DataMember), i, dataMembers[i]);
			propertyDescriptorCollection = new PropertyDescriptorCollection(props);
		}
		int IDashboardDataSet.GetRowIndex(object value) {
			return rows.IndexOf(value);
		}
		int IDashboardDataSet.RowCount {
			get { return rows.Count; }
		}
		IDashboardDataRow IDashboardDataSet.this[int index] {
			get { return rows[index]; }
		}
		object IDashboardDataSet.this[int rowIndex, int columnIndex] {
			get { return rows[rowIndex][columnIndex]; }
		}
		object IDashboardDataSet.this[int rowIndex, string columnName] {
			get { return rows[rowIndex][columnName]; }
		}
		event ListChangedEventHandler IDashboardDataSet.ListChanged {
			add { }
			remove { }
		}
		List<string> IDashboardDataSet.GetColumnNames() {
			return columnByIndex.Keys.ToList();
		}
		IEnumerator<IDashboardDataRow> IEnumerable<IDashboardDataRow>.GetEnumerator() {
			return rows.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return rows.GetEnumerator();
		}
		void IDisposable.Dispose() {
			columnByIndex.Clear();
			rows.Clear();
		}
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			return propertyDescriptorCollection;
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return "DashboardDataSetInternal";
		}
		class InMemoryUnderlyingDataPropertyDescriptor : PropertyDescriptor {
			readonly Type type;
			readonly int index;
			public InMemoryUnderlyingDataPropertyDescriptor(Type type, int index, string name)
				: base(name, new Attribute[0]) {
				this.type = type;
				this.index = index;
			}
			public override bool CanResetValue(object component) {
				return false;
			}
			public override Type ComponentType {
				get { return typeof(object); }
			}
			public override object GetValue(object component) {
				return ((DashboardDataRow)component)[index];
			}
			public override bool IsReadOnly {
				get { return true; }
			}
			public override Type PropertyType {
				get { return type; }
			}
			public override void ResetValue(object component) {
				throw new NotSupportedException();
			}
			public override void SetValue(object component, object value) {
				throw new NotSupportedException();
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
		}
	}
}
