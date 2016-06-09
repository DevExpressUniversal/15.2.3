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

using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System;
using DevExpress.Utils;
using System.Collections;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region PivotFieldListPanelOperationType
	public enum PivotFieldListPanelOperationType {
		NotAllowed = 0,
		Move = 1,
		Remove
	}
	#endregion
	#region PivotFieldListPanelBoxType
	public enum PivotFieldListPanelBoxType {
		None = 0,
		Fields = 1,
		Filters = 2,
		Columns = 3,
		Rows = 4,
		Values = 5
	}
	#endregion
	#region PivotFieldViewInfo
	public struct PivotFieldViewInfo {
		string caption;
		int fieldIndex;
		bool isSelected;
		public PivotFieldViewInfo(string caption, int fieldIndex)
			: this(caption, fieldIndex, false) {
		}
		public PivotFieldViewInfo(string caption, int fieldIndex, bool isSelected) {
			this.caption = caption;
			this.fieldIndex = fieldIndex;
			this.isSelected = isSelected;
		}
		public string Caption { get { return caption; } }
		public int FieldIndex { get { return fieldIndex; } }
		public bool IsSelected { get { return isSelected; } }
		public override string ToString() {
			return caption;
		}
	}
	#endregion
	#region FieldListPanelPivotTableViewModel
	public class FieldListPanelPivotTableViewModel : ViewModelBase, IDisposable {
		#region PivotFieldPosition
		struct PivotFieldPosition {
			internal PivotTableAxis OldAxis { get; set; }
			internal PivotTableAxis NewAxis { get; set; }
			internal int OldPosition { get; set; }
			internal int NewPosition { get; set; }
		}
		#endregion
		#region Fields
		PivotTable pivotTable;
		IErrorHandler errorHandler;
		Dictionary<int, string> fieldNames;
		Dictionary<int, PivotFieldPosition> deferedInfos = new Dictionary<int, PivotFieldPosition>();
		bool isCalculateCaptions;
		List<int> selectedFieldIndexes;
		bool deferLayoutUpdate;
		#endregion
		public FieldListPanelPivotTableViewModel(PivotTable pivotTable, IErrorHandler errorHandler) {
			Guard.ArgumentNotNull(pivotTable, "pivotTable");
			Guard.ArgumentNotNull(errorHandler, "errorHandler");
			this.pivotTable = pivotTable;
			this.errorHandler = errorHandler;
			SubscribeEvents();
		}
		#region Events
		#region DataChanged
		EventHandler onDataChanged;
		public event EventHandler DataChanged { add { onDataChanged += value; } remove { onDataChanged -= value; } }
		protected virtual void RaiseDataChanged() {
			if (onDataChanged != null)
				onDataChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		void SubscribeEvents() {
			pivotTable.FieldsChanged += OnFieldsChanged;
		}
		void UnsubscribeEvents() {
			if (pivotTable != null)
				pivotTable.FieldsChanged -= OnFieldsChanged;
		}
		void OnFieldsChanged(object sender, EventArgs e) {
			RaiseDataChanged();
		}
		public bool Equals(FieldListPanelPivotTableViewModel other) {
			return other != null && object.ReferenceEquals(pivotTable, other.pivotTable);
		}
		#region Captions
		public void BeginCalculateFieldCaptions() {
			this.isCalculateCaptions = true;
			this.selectedFieldIndexes = new List<int>();
			PopulateFieldNames();
		}
		void PopulateFieldNames() {
			fieldNames = new Dictionary<int, string>();
			for (int i = 0; i < pivotTable.Fields.Count; i++) {
				fieldNames.Add(i, pivotTable.GetFieldCaption(i));
			}
		}
		public List<PivotFieldViewInfo> EndCalculateFieldCaptions() {
			List<PivotFieldViewInfo> result = new List<PivotFieldViewInfo>();
			foreach (KeyValuePair<int, string> item in fieldNames) {
				int fieldIndex = item.Key;
				bool isSelected = CalculateSelected(fieldIndex);
				result.Add(new PivotFieldViewInfo(item.Value, fieldIndex, isSelected));
			}
			return result;
		}
		bool CalculateSelected(int fieldIndex) {
			if (deferedInfos.ContainsKey(fieldIndex))
				return deferedInfos[fieldIndex].NewAxis != PivotTableAxis.None;
			return selectedFieldIndexes.Contains(fieldIndex);
		}
		void CheckCalculation() {
			if (!isCalculateCaptions)
				throw new InvalidOperationException("Use BeginCalculateFieldCaptions and EndCalculateFieldCaptions methods");
		}
		public List<PivotFieldViewInfo> GetFilterInfos() {
			return GetCaptionsCore(pivotTable.PageFields, PivotTableAxis.Page, GetFieldCaption);
		}
		public List<PivotFieldViewInfo> GetColumnInfos() {
			return GetCaptionsCore(pivotTable.ColumnFields, PivotTableAxis.Column, GetFieldCaption);
		}
		public List<PivotFieldViewInfo> GetRowInfos() {
			return GetCaptionsCore(pivotTable.RowFields, PivotTableAxis.Row, GetFieldCaption);
		}
		public List<PivotFieldViewInfo> GetValueInfos() {
			return GetCaptionsCore(pivotTable.DataFields, PivotTableAxis.Value, GetDataFieldCaption);
		}
		List<PivotFieldViewInfo> GetCaptionsCore(IEnumerable collection, PivotTableAxis axis, Func<PivotFieldReference, string> getCaption) {
			CheckCalculation();
			List<PivotFieldViewInfo> result = new List<PivotFieldViewInfo>();
			foreach (PivotFieldReference field in collection) {
				int fieldIndex = field.FieldIndex;
				selectedFieldIndexes.Add(fieldIndex);
				if (!deferedInfos.ContainsKey(fieldIndex))
					result.Add(new PivotFieldViewInfo(getCaption(field), fieldIndex));
			}
			AddDeferedItems(result, axis);
			return result;
		}
		string GetFieldCaption(PivotFieldReference field) {
			return field.IsValuesReference ? pivotTable.DefaultDataCaption : fieldNames[field.FieldIndex];
		}
		string GetDataFieldCaption(PivotFieldReference field) {
			return ((PivotDataField)field).GetFullName();
		}
		void AddDeferedItems(List<PivotFieldViewInfo> collection, PivotTableAxis axis) {
			foreach (KeyValuePair<int, PivotFieldPosition> info in deferedInfos) {
				PivotTableAxis currentAxis = info.Value.NewAxis;
				if (currentAxis != axis)
					continue;
				int fieldIndex = info.Key;
				string caption;
				if (currentAxis == PivotTableAxis.Value) {
					int dataFieldIndex = pivotTable.DataFields.GetIndexElementByFieldIndex(fieldIndex);
					PivotDataField dataField = dataFieldIndex == -1 ? new PivotDataField(pivotTable, fieldIndex) : pivotTable.DataFields[dataFieldIndex];
					caption = dataField.GetFullName();
				}
				else
					caption = GetFieldCaption(fieldIndex);
				collection.Add(new PivotFieldViewInfo(caption, fieldIndex));
			}
		}
		#endregion
		public PivotFieldListPanelOperationType GetOperation(int fieldIndex, PivotFieldListPanelBoxType initialBox, PivotFieldListPanelBoxType lastBox, bool outsideParent) {
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				return GetAllowedOperationValuesFakeField(initialBox, lastBox, outsideParent);
			PivotField field = pivotTable.Fields[fieldIndex];
			if (!field.DragOff && (lastBox == PivotFieldListPanelBoxType.None || lastBox == PivotFieldListPanelBoxType.Fields))
				return PivotFieldListPanelOperationType.NotAllowed;
			if (!field.DragToCol && lastBox == PivotFieldListPanelBoxType.Columns)
				return GetAllowedOperationDragRestricted(field, PivotTableAxis.Column, initialBox, lastBox);
			if (!field.DragToRow && lastBox == PivotFieldListPanelBoxType.Rows)
				return GetAllowedOperationDragRestricted(field, PivotTableAxis.Row, initialBox, lastBox);
			if (!field.DragToPage && lastBox == PivotFieldListPanelBoxType.Filters)
				return GetAllowedOperationDragRestricted(field, PivotTableAxis.Page, initialBox, lastBox);
			if (!field.DragToData && initialBox != PivotFieldListPanelBoxType.Values && lastBox == PivotFieldListPanelBoxType.Values)
				return PivotFieldListPanelOperationType.NotAllowed;
			return GetAllowedOperationCore(initialBox, lastBox, outsideParent);
		}
		PivotFieldListPanelOperationType GetAllowedOperationDragRestricted(PivotField field, PivotTableAxis axis, PivotFieldListPanelBoxType initialBox, PivotFieldListPanelBoxType lastBox) {
			if (field.IsDataField)
				return PivotFieldListPanelOperationType.NotAllowed;
			if ((initialBox == PivotFieldListPanelBoxType.Fields && field.Axis == axis) || initialBox == lastBox)
				return PivotFieldListPanelOperationType.Move;
			return PivotFieldListPanelOperationType.NotAllowed;
		}
		PivotFieldListPanelOperationType GetAllowedOperationValuesFakeField(PivotFieldListPanelBoxType initialBox, PivotFieldListPanelBoxType lastBox, bool outsideParent) {
			if ((initialBox == PivotFieldListPanelBoxType.Columns || initialBox == PivotFieldListPanelBoxType.Rows) &&
					(lastBox == PivotFieldListPanelBoxType.Columns || lastBox == PivotFieldListPanelBoxType.Rows))
				return PivotFieldListPanelOperationType.Move;
			if (CantDragOffDataField())
				return PivotFieldListPanelOperationType.NotAllowed;
			return GetAllowedOperationCore(initialBox, lastBox, outsideParent);
		}
		bool CantDragOffDataField() {
			foreach (PivotDataField dataField in pivotTable.DataFields)
				if (!pivotTable.Fields[dataField.FieldIndex].DragOff)
					return true;
			return false;
		}
		PivotFieldListPanelOperationType GetAllowedOperationCore(PivotFieldListPanelBoxType initialBox, PivotFieldListPanelBoxType lastBox, bool outsideParent) {
			if (outsideParent || lastBox == PivotFieldListPanelBoxType.Fields)
				return initialBox == PivotFieldListPanelBoxType.Fields ? PivotFieldListPanelOperationType.NotAllowed : PivotFieldListPanelOperationType.Remove;
			if (lastBox == PivotFieldListPanelBoxType.None)
				return PivotFieldListPanelOperationType.NotAllowed;
			return PivotFieldListPanelOperationType.Move;
		}
		#region Move
		public void MovePivotField(int index, PivotTableAxis from, int oldPosition, PivotTableAxis to, int newPosition) {
			if (deferLayoutUpdate) {
				if (deferedInfos.ContainsKey(index))
					deferedInfos.Remove(index);
				deferedInfos.Add(index, new PivotFieldPosition() { OldAxis = from, OldPosition = oldPosition, NewAxis = to, NewPosition = newPosition });
				RaiseDataChanged();
			}
			else
				UpdateNow(index, from, oldPosition, to, newPosition);
		}
		void UpdateNow(int index, PivotTableAxis from, int oldPosition, PivotTableAxis to, int newPosition) {
			pivotTable.BeginTransaction(errorHandler);
			try {
				MoveField(index, from, oldPosition, to, newPosition);
			}
			finally {
				pivotTable.EndTransaction();
			}
		}
		void MoveField(int index, PivotTableAxis from, int oldPosition, PivotTableAxis to, int newPosition) {
			if (to == PivotTableAxis.None) {
				pivotTable.RemoveKeyField(oldPosition, from, errorHandler);
				return;
			}
			if (from != PivotTableAxis.None) {
				pivotTable.MoveKeyField(from, oldPosition, to, newPosition, errorHandler);
				return;
			}
			if (to == PivotTableAxis.Value)
				pivotTable.InsertDataField(index, newPosition, errorHandler);
			else
				pivotTable.InsertFieldToKeyFields(index, to, newPosition, errorHandler);
		}
		#endregion
		public void Update() {
			if (deferedInfos.Count == 0)
				return;
			pivotTable.BeginTransaction(errorHandler);
			try {
				foreach (KeyValuePair<int, PivotFieldPosition> info in deferedInfos) {
					PivotFieldPosition value = info.Value;
					MoveField(info.Key, value.OldAxis, value.OldPosition, value.NewAxis, value.NewPosition);
				}
			}
			finally {
				pivotTable.EndTransaction();
			}
			deferedInfos.Clear();
		}
		public PivotTableAxis ConvertBoxTypeToAxis(PivotFieldListPanelBoxType type) {
			switch (type) {
				case PivotFieldListPanelBoxType.None:
				case PivotFieldListPanelBoxType.Fields:
					return PivotTableAxis.None;
				case PivotFieldListPanelBoxType.Filters:
					return PivotTableAxis.Page;
				case PivotFieldListPanelBoxType.Columns:
					return PivotTableAxis.Column;
				case PivotFieldListPanelBoxType.Rows:
					return PivotTableAxis.Row;
				case PivotFieldListPanelBoxType.Values:
					return PivotTableAxis.Value;
			}
			throw new ArgumentException("Invalid Type");
		}
		public bool DeferLayoutUpdate {
			get { return deferLayoutUpdate; }
			set {
				if (DeferLayoutUpdate == value)
					return;
				deferedInfos.Clear();
				deferLayoutUpdate = value;
				RaiseDataChanged();
				OnPropertyChanged("DeferLayoutUpdate");
			}
		}
		public bool UpdateEnabled { get { return DeferLayoutUpdate; } }
		public void Dispose() {
			Dispose(true);
		}
		public void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeEvents();
				this.pivotTable = null;
				this.errorHandler = null;
			}
		}
	}
	#endregion
}
