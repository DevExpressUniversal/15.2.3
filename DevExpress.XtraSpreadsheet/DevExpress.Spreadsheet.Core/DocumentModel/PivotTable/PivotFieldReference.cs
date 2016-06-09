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

using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using System.Collections.Generic;
using System;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotFieldReference
	public interface IPivotFieldReference {
		int FieldIndex { get; }
		void OnFieldIndexChanged(DocumentModel documentModel, int oldIndex, int newIndex);
	}
	public class PivotFieldReference : IPivotFieldReference {
		int fieldIndex;
		public PivotFieldReference(int fieldIndex) {
			this.fieldIndex = fieldIndex;
		}
		public int FieldIndex { get { return fieldIndex; } }
		public bool IsValuesReference { get { return fieldIndex == PivotTable.ValuesFieldFakeIndex; } }
		public void OnFieldIndexChanged(DocumentModel documentModel, int oldIndex, int newIndex) {
			HistoryHelper.SetValue(documentModel, oldIndex, newIndex, SetFieldIndexCore);
		}
		void SetFieldIndexCore(int value) {
			this.fieldIndex = value;
		}
		public static implicit operator PivotFieldReference(int value) {
			return new PivotFieldReference(value);
		}
		public static implicit operator int(PivotFieldReference value) {
			return value.fieldIndex;
		}
		public override int GetHashCode() {
			return fieldIndex;
		}
		public override bool Equals(object obj) {
			PivotFieldReference other = obj as PivotFieldReference;
			if (other == null)
				return false;
			return fieldIndex == other.fieldIndex;
		}
	}
	#endregion
	#region PivotFieldReferenceCollection
	public abstract class PivotFieldReferenceCollection<T> : UndoableCollection<T> where T : IPivotFieldReference {
		protected PivotFieldReferenceCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		internal IEnumerable<int> GetIndicesEnumerable() {
			return new Enumerable<int>(new EnumeratorConverter<T, int>(GetEnumerator(), GetItemFieldIndex));
		}
		internal virtual IEnumerable<int> GetKeyIndicesEnumerable() {
			return GetIndicesEnumerable();
		}
		int GetItemFieldIndex(T value) {
			return value.FieldIndex;
		}
		public int GetIndexElementByFieldIndex(int fieldIndex) {
			for (int index = 0; index < Count; index++)
				if (this[index].FieldIndex == fieldIndex)
					return index;
			return -1;
		}
		public void RemoveByFieldIndex(int fieldIndex) {
			for (int i = 0; i < Count; i++) {
				if (this[i].FieldIndex == fieldIndex) {
					this.RemoveAt(i);
					break;
				}
			}
		}
	}
	#endregion
	#region PivotTableColumnRowFieldIndices
	public class PivotTableColumnRowFieldIndices : PivotFieldReferenceCollection<PivotFieldReference> {
		int valuesFieldIndex = -1;
		public PivotTableColumnRowFieldIndices(DocumentModel documentModel)
			: base(documentModel) {
		}
		protected internal bool HasKeyField { get { return KeyIndicesCount > 0; } }
		protected internal bool HasValuesField { get { return valuesFieldIndex >= 0; } }
		protected internal int KeyIndicesCount { get { return HasValuesField ? Count - 1 : Count; } }
		protected internal int ValuesFieldIndex { get { return valuesFieldIndex; } }
		protected internal int LastKeyFieldIndex {
			get {
				int index = Count - 1;
				if (!HasValuesField)
					return index;
				if (valuesFieldIndex == index)
					--index;
				return index;
			}
		}
		internal override IEnumerable<int> GetKeyIndicesEnumerable() {
			int i = 0;
			if (HasValuesField) {
				for (; i < valuesFieldIndex; ++i)
					yield return this[i].FieldIndex;
				++i;
			}
			for (; i < Count; ++i)
				yield return this[i].FieldIndex;
		}
		protected override void OnItemInserted(int index, PivotFieldReference item) {
			if (HasValuesField) {
				Debug.Assert(item.FieldIndex != PivotTable.ValuesFieldFakeIndex);
				if (index <= valuesFieldIndex)
					++valuesFieldIndex;
			}
			else
				if (item.FieldIndex == PivotTable.ValuesFieldFakeIndex)
					valuesFieldIndex = index;
		}
		protected override void OnItemRemoved(int index, PivotFieldReference item) {
			if (HasValuesField)
				if (index < valuesFieldIndex)
					--valuesFieldIndex;
				else
					if (index == valuesFieldIndex)
						valuesFieldIndex = -1;
		}
		public override void ClearCore() {
			base.ClearCore();
			valuesFieldIndex = -1;
		}
		public override void AddRangeCore(IEnumerable<PivotFieldReference> collection) {
			base.AddRangeCore(collection);
			int i = 0;
			foreach (PivotFieldReference reference in collection)
				if (reference.FieldIndex == PivotTable.ValuesFieldFakeIndex) {
					valuesFieldIndex = i;
					break;
				}
				else
					++i;
		}
		protected internal void RemoveValuesField() {
			if (HasValuesField)
				RemoveAt(ValuesFieldIndex);
		}
		public void CopyFrom(PivotTableColumnRowFieldIndices source) {
			this.InnerList.AddRange(source.InnerList);
			valuesFieldIndex = source.valuesFieldIndex;
		}
		public void CopyFromNoHistory(PivotTableColumnRowFieldIndices source) {
			ClearCore();
			Capacity = source.Count;
			foreach (PivotFieldReference item in source)
				AddCore(item.FieldIndex);
			valuesFieldIndex = source.valuesFieldIndex;
		}
	}
	#endregion
}
