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

using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotItemMoveType
	public enum PivotItemMoveType {
		Up,
		Down,
		ToBeginning,
		ToEnd
	}
	#endregion
	#region MovePivotFieldItemCommandBase
	public abstract class MovePivotFieldItemCommandBase<T> : PivotTableTransactedCommand {
		#region Fields
		readonly UndoableCollection<T> collection;
		readonly PivotItemMoveType moveType;
		readonly int sourceIndex;
		readonly int targetIndex;
		#endregion
		protected MovePivotFieldItemCommandBase(PivotTable pivotTable, UndoableCollection<T> collection, int sourceIndex, PivotItemMoveType moveType, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			Guard.ArgumentNotNull(pivotTable, "pivotTable");
			Guard.ArgumentNotNull(collection, "collection");
			this.collection = collection;
			ValueChecker.CheckValue(sourceIndex, 0, Count - 1);
			this.sourceIndex = sourceIndex;
			this.moveType = moveType;
			this.targetIndex = GetTargetIndex();
		}
		#region Properties
		protected UndoableCollection<T> Collection { get { return collection; } }
		protected virtual int Count { get { return collection.Count; } }
		protected int SourceIndex { get { return sourceIndex; } }
		protected int TargetIndex { get { return targetIndex; } }
		#endregion
		protected internal override bool Validate() {
			if (Count <= 1)
				return false;
			if (moveType == PivotItemMoveType.Up || moveType == PivotItemMoveType.ToBeginning)
				return sourceIndex > 0;
			return sourceIndex < Count - 1;
		}
		protected internal override void ExecuteCore() {
			collection.Move(sourceIndex, targetIndex);
			ProcessItem(collection[targetIndex]);
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
		protected virtual void ProcessItem(T item) {
		}
		int GetTargetIndex() {
			if (moveType == PivotItemMoveType.Up)
				return sourceIndex - 1;
			else if (moveType == PivotItemMoveType.Down)
				return sourceIndex + 1;
			else if (moveType == PivotItemMoveType.ToEnd)
				return Count - 1;
			return 0;
		}
	}
	#endregion
	#region MovePivotFieldReferenceCommand
	public class MovePivotFieldReferenceCommand : MovePivotFieldItemCommandBase<PivotFieldReference> {
		public MovePivotFieldReferenceCommand(PivotTable pivotTable, PivotFieldReferenceCollection<PivotFieldReference> collection, int sourceIndex, PivotItemMoveType moveType, IErrorHandler errorHandler)
			: base(pivotTable, collection, sourceIndex, moveType, errorHandler) {
		}
		protected override void ProcessItem(PivotFieldReference reference) {
			if (reference.IsValuesReference)
				PivotTable.DataPosition = TargetIndex == Count - 1 ? -1 : TargetIndex;
		}
	}
	#endregion
	#region MovePivotPageFieldCommand
	public class MovePivotPageFieldCommand : MovePivotFieldItemCommandBase<PivotPageField> {
		public MovePivotPageFieldCommand(PivotTable pivotTable, PivotFieldReferenceCollection<PivotPageField> collection, int sourceIndex, PivotItemMoveType moveType, IErrorHandler errorHandler)
			: base(pivotTable, collection, sourceIndex, moveType, errorHandler) {
		}
	}
	#endregion
	#region MovePivotDataFieldCommand
	public class MovePivotDataFieldCommand : MovePivotFieldItemCommandBase<PivotDataField> {
		public MovePivotDataFieldCommand(PivotTable pivotTable, PivotFieldReferenceCollection<PivotDataField> collection, int sourceIndex, PivotItemMoveType moveType, IErrorHandler errorHandler)
			: base(pivotTable, collection, sourceIndex, moveType, errorHandler) {
		}
		protected override void ProcessItem(PivotDataField item) {
			PivotTable.Filters.CorrectMeasureFieldIndexesAfterMove(SourceIndex, TargetIndex);
		}
	}
	#endregion
	#region PivotMoveFieldReferenceToTargetIndexCommand
	public class PivotMoveFieldReferenceToTargetIndexCommand : PivotTableTransactedCommand {
		readonly PivotTableAxis sourceAxis;
		readonly PivotTableAxis targetAxis;
		readonly int sourceIndex;
		int targetIndex;
		int sourceFieldIndex;
		public PivotMoveFieldReferenceToTargetIndexCommand(PivotTable pivotTable, PivotTableAxis sourceAxis, int sourceIndex, PivotTableAxis targetAxis, int targetIndex, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			if (sourceAxis == PivotTableAxis.None)
				Exceptions.ThrowArgumentException("axis", sourceAxis);
			if (targetAxis == PivotTableAxis.None)
				Exceptions.ThrowArgumentException("axis", targetAxis);
			this.sourceAxis = sourceAxis;
			this.targetAxis = targetAxis;
			int sourceCount = GetCount(sourceAxis) - 1;
			int targetCount = GetCount(targetAxis);
			ValueChecker.CheckValue(sourceIndex, 0, sourceCount);
			ValueChecker.CheckValue(targetIndex, 0, targetCount);
			this.sourceIndex = sourceIndex;
			this.targetIndex = targetIndex;
		}
		int GetCount(PivotTableAxis axis) {
			switch (axis) {
				case PivotTableAxis.Column:
					return PivotTable.ColumnFields.Count;
				case PivotTableAxis.Row:
					return PivotTable.RowFields.Count;
				case PivotTableAxis.Page:
					return PivotTable.PageFields.Count;
				default:
					return PivotTable.DataFields.Count;
			}
		}
		protected internal override bool Validate() {
			switch (sourceAxis) {
				case PivotTableAxis.Column:
					sourceFieldIndex = PivotTable.ColumnFields[sourceIndex];
					break;
				case PivotTableAxis.Page:
					sourceFieldIndex = PivotTable.PageFields[sourceIndex];
					break;
				case PivotTableAxis.Row:
					sourceFieldIndex = PivotTable.RowFields[sourceIndex];
					break;
				case PivotTableAxis.Value:
					sourceFieldIndex = PivotTable.DataFields[sourceIndex];
					break;
				default:
					return false;
			}
			return PivotInsertFieldToKeyFieldsCommand.Validate(PivotTable, sourceFieldIndex, targetAxis, ErrorHandler);
		}
		protected internal override void ExecuteCore() {
			if (sourceAxis == targetAxis) {
				if (sourceIndex < targetIndex)
					--targetIndex;
				if (sourceIndex == targetIndex)
					return;
				if (sourceAxis == PivotTableAxis.Page)
					PivotTable.PageFields.Move(sourceIndex, targetIndex);
				else if (sourceAxis == PivotTableAxis.Value) {
					PivotTable.DataFields.Move(sourceIndex, targetIndex);
					PivotTable.Filters.CorrectMeasureFieldIndexesAfterMove(sourceIndex, targetIndex);
				}
				else {
					PivotTableColumnRowFieldIndices collection = sourceAxis == PivotTableAxis.Column ? PivotTable.ColumnFields : PivotTable.RowFields;
					if (sourceFieldIndex == PivotTable.ValuesFieldFakeIndex)
						PivotInsertFieldToKeyFieldsCommand.AddValuesField(PivotTable, collection, targetIndex);
					else
						collection.Move(sourceIndex, targetIndex);
				}
			}
			else
				if (targetAxis == PivotTableAxis.Value) {
					PivotTable.RemoveKeyField(sourceIndex, sourceAxis, ErrorHandler); 
					PivotTable.InsertDataField(sourceFieldIndex, targetIndex, ErrorHandler);
				}
				else {
					PivotTableAxis fieldAxis = sourceFieldIndex == PivotTable.ValuesFieldFakeIndex ? PivotTableAxis.Value : PivotTable.Fields[sourceFieldIndex].Axis;
					PivotTable.InsertFieldToKeyFields(sourceFieldIndex, targetAxis, targetIndex, ErrorHandler); 
					if (sourceAxis == PivotTableAxis.Value && fieldAxis == PivotTableAxis.None)
						PivotTable.RemoveKeyField(sourceIndex, PivotTableAxis.Value, ErrorHandler);
				}
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
	}
	#endregion
}
