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
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	public enum SelectPivotType {
		Label,
		Value,
		LabelAndValue,
		EntirePivotTable
	}
	#region PivotSelection
	public class PivotSelection {
		#region Fields
		const byte maskDataSelection = 1;
		const byte maskExtendable = 2;
		const byte maskLabel = 4;
		const byte maskShowHeader = 8;
		readonly DocumentModel documentModel;
		PivotArea pivotArea;
		PivotTableAxis axis;
		ViewPaneType pane;
		PivotTable pivotTable;
		int activeColumn;
		int activeRow;
		int countClick;
		int countSelection;
		int dimension;
		int maximum;
		int minimum;
		int previousColumn;
		int previousRow;
		int start;
		byte packedValues;
		#endregion
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public bool HasInitSelection { get { return PivotTable != null; } }
		public PivotArea PivotArea { get { return pivotArea; } }
		public PivotTable PivotTable { 
			get { return pivotTable; } 
			set { pivotTable = value; } 
		}
		public PivotTableAxis Axis { get { return axis; } set { SetAxis((int)value); } }
		public ViewPaneType Pane { get { return pane; } set { SetPane((int)value); } }
		public int ActiveColumn { get { return activeColumn; } set { SetActiveColumn(value); } }
		public int ActiveRow { get { return activeRow; } set { SetActiveRow(value); } }
		public int CountClick { get { return countClick; } set { SetCountClick(value); } }
		public int CountSelection { get { return countSelection; } set { SetCountSelection(value); } }
		public int Dimension { get { return dimension; } set { SetDimension(value); } }
		public int Maximum { get { return maximum; } set { SetMaximum(value); } }
		public int Minimum { get { return minimum; } set { SetMinimum(value); } }
		public int PreviousColumn { get { return previousColumn; } set { SetPreviousColumn(value); } }
		public int PreviousRow { get { return previousRow; } set { SetPreviousRow(value); } }
		public int Start { get { return start; } set { SetStart(value); } }
		public bool IsDataSelection {
			get { return GetBooleanValue(maskDataSelection); }
			set {
				if (IsDataSelection != value)
					SetHistory(maskDataSelection, value);
			}
		}
		public bool IsExtendable {
			get { return GetBooleanValue(maskExtendable); }
			set {
				if (IsExtendable != value)
					SetHistory(maskExtendable, value);
			}
		}
		public bool IsLabel {
			get { return GetBooleanValue(maskLabel); }
			set {
				if (IsLabel != value)
					SetHistory(maskLabel, value);
			}
		}
		public bool IsShowHeader {
			get { return GetBooleanValue(maskShowHeader); }
			set {
				if (IsShowHeader != value)
					SetHistory(maskShowHeader, value);
			}
		}
		#endregion
		#region Constructors
		public PivotSelection(Worksheet worksheet) {
			this.documentModel = worksheet.Workbook;
			this.pivotArea = new PivotArea(documentModel);
		}
		#endregion
		#region Action - set param without history
		public CellRangeBase GetCurrentSelection() {
			CellRangeBase range = null;
			if (PivotTable != null) {
				range = PivotTable.Location.WholeRange;
				if (IsDataSelection && !IsLabel)
					range = CalculateValue();
				else if (IsLabel && !IsDataSelection) {
					CellRangeBase valuesRange = CalculateValue();
					if (!Object.ReferenceEquals(valuesRange, range))
						range = range.ExcludeRange(valuesRange);
				}
				SetColumnRow(DocumentModel.ActiveSheet.Selection, range);
			}
			return range;
		}
		public CellRangeBase SetSelection(PivotTable pivotTable, SelectPivotType action) {
			this.pivotTable = pivotTable;
			if (PivotTable != null) {
				switch(action){
					case SelectPivotType.LabelAndValue:
					case SelectPivotType.EntirePivotTable:
						InitSelection(false, false);
						break;
					case SelectPivotType.Label:
						InitSelection(false, true);
						break;
					case SelectPivotType.Value:
						InitSelection(true, false);
						break;
				}
			}
			return GetCurrentSelection();
		}
		void InitSelection(bool isData, bool isLabel) {
			IsDataSelection = isData;
			IsLabel = isLabel;
			pane = ModelWorksheetView.DefaultItem.ActivePaneType;
			IsShowHeader = true;
			countClick = 1;
			SetPivotAreaByType();
		}
		void SetPivotAreaByType() {
			PivotArea.Type = PivotAreaType.All;
			PivotArea.IsOutline = false;
			PivotArea.FieldPosition = 0;
			PivotArea.IsLabelOnly = IsLabel;
			PivotArea.IsDataOnly = IsDataSelection;
		}
		void SetColumnRow(SheetViewSelection selection, CellRangeBase range) {
			CellPosition topLeft = range.GetFirstInnerCellRange().TopLeft;
			CellPosition activePosition = selection.ActiveCell;
			int row = 0;
			int column = 0;
			if(topLeft.Equals(activePosition)){
				row = selection.ActiveCell.Row;
				column = selection.ActiveCell.Column;
			}
			activeColumn = column;
			previousColumn = column;
			activeRow = row;
			previousRow = row;
		}
		CellRangeBase CalculateValue() {
			bool isPage = PivotTable.PageFields.Count > 0;
			bool isRowEmpty = PivotTable.RowFields.Count == 0;
			bool isColumnEmpty = PivotTable.ColumnFields.Count == 0;
			if (PivotTable.DataFields.Count == 0)
				if ((!isRowEmpty && isColumnEmpty) || (!isColumnEmpty && isRowEmpty) || (isPage && isRowEmpty && isColumnEmpty))
					return PivotTable.Location.WholeRange;
			return PivotTable.Location.TryGetDataRange();
		}
		#endregion
		#region History
		void SetAxis(int value) {
			if ((int)Axis != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, (int)Axis, value, SetAxisCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetPane(int value) {
			if ((int)Pane != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, (int)Pane, value, SetPaneCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetActiveColumn(int value) {
			if (ActiveColumn != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, ActiveColumn, value, SetActiveColumnCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetActiveRow(int value) {
			if (ActiveRow != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, ActiveRow, value, SetActiveRowCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetCountClick(int value) {
			if (CountClick != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, CountClick, value, SetCountClickCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetCountSelection(int value) {
			if (CountSelection != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, CountSelection, value, SetCountSelectionCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetDimension(int value) {
			if (Dimension != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, Dimension, value, SetDimensionCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetMaximum(int value) {
			if (Maximum != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, Maximum, value, SetMaximumCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetMinimum(int value) {
			if (Minimum != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, Minimum, value, SetMinimumCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetPreviousColumn(int value) {
			if (PreviousColumn != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, PreviousColumn, value, SetPreviousColumnCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetPreviousRow(int value) {
			if (PreviousRow != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, PreviousRow, value, SetPreviousRowCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetStart(int value) {
			if (Start != value) {
				ActionHistoryItem<int> history = new ActionHistoryItem<int>(DocumentModel, Start, value, SetStartCore);
				DocumentModel.History.Add(history);
				history.Execute();
			}
		}
		void SetHistory(byte mask, bool value) {
			ActionHistoryItem<byte> historyItem =
				new ActionHistoryItem<byte>(DocumentModel, packedValues, CreateNewPackedValue(mask, value), SetPackageNewValueCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		byte CreateNewPackedValue(byte mask, bool value) {
			byte newPack = this.packedValues;
			PackedValues.SetBoolBitValue(ref newPack, mask, value);
			return newPack;
		}
		void SetPackageNewValueCore(byte value) {
			packedValues = value;
		}
		bool GetBooleanValue(uint mask) {
			return PackedValues.GetBoolBitValue(this.packedValues, mask);
		}
		protected internal void SetAxisCore(int value) {
			this.axis = (PivotTableAxis)value;
		}
		protected internal void SetPaneCore(int value) {
			this.pane = (ViewPaneType)value;
		}
		protected internal void SetActiveColumnCore(int value) {
			this.activeColumn = value;
		}
		protected internal void SetActiveRowCore(int value) {
			this.activeRow = value;
		}
		protected internal void SetCountClickCore(int value) {
			this.countClick = value;
		}
		protected internal void SetCountSelectionCore(int value) {
			this.countSelection = value;
		}
		protected internal void SetDimensionCore(int value) {
			this.dimension = value;
		}
		protected internal void SetMaximumCore(int value) {
			this.maximum = value;
		}
		protected internal void SetMinimumCore(int value) {
			this.minimum = value;
		}
		protected internal void SetPreviousColumnCore(int value) {
			this.previousColumn = value;
		}
		protected internal void SetPreviousRowCore(int value) {
			this.previousRow = value;
		}
		protected internal void SetStartCore(int value) {
			this.start = value;
		}
		protected internal bool IsExportNeed() {
			return HasInitSelection;
		}
		protected internal void Clear() {
			if (HasInitSelection) {
				this.FromTo(new PivotSelection(DocumentModel.ActiveSheet));
			}
		}
		protected internal void FromTo(PivotSelection other) {
			this.pivotArea = other.pivotArea;
			this.axis = other.axis;
			this.pane = other.pane;
			this.pivotTable = other.pivotTable;
			this.activeColumn = other.activeColumn;
			this.activeRow = other.activeRow;
			this.countClick = other.countClick;
			this.countSelection = other.countSelection;
			this.dimension = other.dimension;
			this.maximum = other.maximum;
			this.minimum = other.minimum;
			this.previousColumn = other.previousColumn;
			this.previousRow = other.previousRow;
			this.start = other.start;
			this.packedValues = other.packedValues;
		}
		#endregion
	}
	#endregion
}
