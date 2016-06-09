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
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Utils;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChartDataSource
	public class ChartDataSource : IList<ChartDataRow>, ITypedList, IBindingList, IDisposable {
		#region Fields
		readonly IDataReference argumentData;
		readonly List<IDataReference> valueData;
		PropertyDescriptorCollection propertyDescriptorCollectionCache;
		#endregion
		public ChartDataSource(IDataReference argumentData, List<IDataReference> valueData) {
			this.argumentData = argumentData;
			this.valueData = valueData;
		}
		public ChartDataSource(IDataReference argumentData, params IDataReference[] valueData)
			: this(argumentData, new List<IDataReference>(valueData)) {
		}
		protected IDataReference ArgumentData { get { return argumentData; } }
		protected List<IDataReference> ValueData { get { return valueData; } }
		protected internal Chart Parent { get; set; }
		public void OnDataChanged() {
			if (argumentData != null)
				argumentData.OnContentVersionChanged();
			if (valueData != null) {
				foreach (IDataReference reference in valueData)
					reference.OnContentVersionChanged();
			}
			RaiseListChanged();
		}
		#region ITypedList Members
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			if (propertyDescriptorCollectionCache == null) {
				Debug.Assert(argumentData != null);
				int count = GetColumnCount();
				PropertyDescriptor[] properties = new PropertyDescriptor[count];
				properties[0] = new ChartDataRowPropertyDescriptor("Argument", argumentData, Parent);
				if (valueData != null) {
					for (int i = 0; i < valueData.Count; i++)
						properties[i + 1] = new ChartDataRowPropertyDescriptor("Value" + i, valueData[i], Parent);
				}
				propertyDescriptorCollectionCache = new PropertyDescriptorCollection(properties);
			}
			return propertyDescriptorCollectionCache;
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return "Table1";
		}
		int GetColumnCount() {
			int count = argumentData != null ? 1 : 0;
			if (valueData != null)
				count += valueData.Count;
			return count;
		}
		protected long GetRowCount() {
			long count = 0;
			if (valueData != null) {
				foreach (IDataReference reference in valueData)
					count = Math.Max(count, reference.ValuesCount);
			}
			return count;
		}
		protected virtual ChartDataRow PrepareRowByIndex(int index) {
			return new ChartDataRow(index);
		}
		#endregion
		#region IBindingList Members
		public bool AllowEdit { get { return false; } }
		public bool AllowNew { get { return false; } }
		public bool AllowRemove { get { return false; } }
		public bool IsSorted { get { throw new NotSupportedException(); } }
		public ListSortDirection SortDirection { get { throw new NotSupportedException(); } }
		public bool SupportsChangeNotification { get { return true; } }
		public bool SupportsSearching { get { return false; } }
		public bool SupportsSorting { get { return false; } }
		public void AddIndex(PropertyDescriptor property) {
			throw new NotSupportedException();
		}
		public object AddNew() {
			throw new NotSupportedException();
		}
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new NotSupportedException();
		}
		public int Find(PropertyDescriptor property, object key) {
			throw new NotSupportedException();
		}
		public void RemoveIndex(PropertyDescriptor property) {
			throw new NotSupportedException();
		}
		public void RemoveSort() {
			throw new NotSupportedException();
		}
		public PropertyDescriptor SortProperty {
			get { throw new NotSupportedException(); }
		}
		#endregion
		#region DataChanged
		ListChangedEventHandler onListChanged;
		public event ListChangedEventHandler ListChanged { add { onListChanged += value; } remove { onListChanged -= value; } }
		protected internal virtual void RaiseListChanged() {
			if (onListChanged != null) {
				ListChangedEventArgs args = new ListChangedEventArgs(ListChangedType.ItemChanged, 0); 
				onListChanged(this, args);
			}
		}
		#endregion
		#region IList<ChartDataRow> Members
		public ChartDataRow this[int index] { get { return PrepareRowByIndex(index); } set { throw new NotSupportedException(); } }
		public int IndexOf(ChartDataRow item) {
			throw new NotSupportedException();
		}
		public void Insert(int index, ChartDataRow item) {
			throw new NotSupportedException();
		}
		public void RemoveAt(int index) {
			throw new NotSupportedException();
		}
		#endregion
		#region ICollection<ChartDataRow> Members
		public virtual int Count { get { return (int)GetRowCount(); } }
		public bool IsReadOnly { get { return true; } }
		public void Add(ChartDataRow item) {
			throw new NotSupportedException();
		}
		public void Clear() {
			throw new NotSupportedException();
		}
		public bool Contains(ChartDataRow item) {
			throw new NotSupportedException();
		}
		public void CopyTo(ChartDataRow[] array, int arrayIndex) {
			throw new NotSupportedException();
		}
		public bool Remove(ChartDataRow item) {
			throw new NotSupportedException();
		}
		#endregion
		#region IEnumerable<ChartDataRow> Members
		public IEnumerator<ChartDataRow> GetEnumerator() {
			return null;
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
		#region IList Members
		public bool IsFixedSize { get { return false; } }
		public int Add(object value) {
			throw new NotSupportedException();
		}
		public bool Contains(object value) {
			throw new NotSupportedException();
		}
		public int IndexOf(object value) {
			throw new NotSupportedException();
		}
		public void Insert(int index, object value) {
			throw new NotSupportedException();
		}
		public void Remove(object value) {
			throw new NotSupportedException();
		}
		object IList.this[int index] { get { return this[index]; } set { throw new NotSupportedException(); } }
		#endregion
		#region ICollection Members
		public bool IsSynchronized { get { return false; } }
		public object SyncRoot { get { return this; } }
		public void CopyTo(Array array, int index) {
			throw new NotSupportedException();
		}
		#endregion
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				Parent = null;
				argumentData.Dispose();
				if (valueData != null)
					foreach (IDataReference valueReference in valueData)
						valueReference.Dispose();
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public void ObtainArgumentReferencedRanges(FormulaReferencedRanges where) {
			if (ArgumentData != null)
				ArgumentData.ObtainReferencedRanges(where);
		}
		public void ObtainValueReferencedRanges(FormulaReferencedRanges where) {
			if (ValueData != null) {
				foreach (IDataReference reference in ValueData)
					reference.ObtainReferencedRanges(where);
			}
		}
	}
	#endregion
	#region ChartDataRowPropertyDescriptor
	public class ChartDataRowPropertyDescriptor : PropertyDescriptor {
		#region Fields
		readonly IDataReference dataReference;
		readonly Chart parent;
		#endregion
		public ChartDataRowPropertyDescriptor(string name, IDataReference dataReference, Chart parent)
			: base(name, null) {
			this.dataReference = dataReference;
			this.parent = parent;
		}
		public override Type ComponentType { get { return typeof(ChartDataRow); } }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType {
			get {
				switch (dataReference.ValueType){
					case DataReferenceValueType.DateTime:
						return typeof(DateTime);
					case DataReferenceValueType.Number:
						return typeof(double);
					case DataReferenceValueType.String:
						return typeof(string);
					default:
						throw new ArgumentException("Invalid DataReferenceValueType : " + dataReference.ValueType.ToString());
				}
			}
		}
		public override object GetValue(object component) {
			ChartDataRow row = (ChartDataRow)component;
			return row.GetValue(dataReference, parent);
		}
		public override bool CanResetValue(object component) {
			return false;
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
	#endregion
	#region ChartDataRow
	public class ChartDataRow {
		#region Fields
		int index;
		#endregion
		public ChartDataRow(int index) {
			this.index = index;
		}
		public int Index { get { return index; } }
		public object GetValue(IDataReference column, Chart parent) {
			object result = column[index];
			if (result == null && parent != null && parent.DispBlanksAs == DisplayBlanksAs.Zero && column.IsNumber)
				result = 0.0;
			return result;
		}
	}
	#endregion
}
