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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Model.History {
	#region ToggleFieldCodesHistoryItem
	public class ToggleFieldCodesHistoryItem : RichEditHistoryItem {
		readonly int fieldIndex;
		public ToggleFieldCodesHistoryItem(PieceTable pieceTable, int fieldIndex)
			: base(pieceTable) {
			Guard.ArgumentNonNegative(fieldIndex, "fieldIndex");
			this.fieldIndex = fieldIndex;
		}
		#region Properties
		public override bool ChangeModified { get { return false; } }
		public int FieldIndex { get { return fieldIndex; } }
		#endregion
		protected override void UndoCore() {
			ToggleFieldCodes();
		}
		protected override void RedoCore() {
			ToggleFieldCodes();
		}
		void ToggleFieldCodes() {
			Field field = PieceTable.Fields[FieldIndex];
			field.IsCodeView = !field.IsCodeView;
			PieceTable.ApplyChanges(DocumentModelChangeType.ToggleFieldCodes, field.FirstRunIndex, field.LastRunIndex);
		}
	}
	#endregion
	#region ToggleFieldLockedHistoryItem
	public class ToggleFieldLockedHistoryItem : RichEditHistoryItem {
		readonly int fieldIndex;
		public ToggleFieldLockedHistoryItem(PieceTable pieceTable, int fieldIndex)
			: base(pieceTable) {
			Guard.ArgumentNonNegative(fieldIndex, "fieldIndex");
			this.fieldIndex = fieldIndex;
		}
		#region Properties
		public override bool ChangeModified { get { return false; } }
		public int FieldIndex { get { return fieldIndex; } }
		#endregion
		protected override void UndoCore() {
			ToggleFieldLocked();
		}
		protected override void RedoCore() {
			ToggleFieldLocked();
		}
		void ToggleFieldLocked() {
			Field field = PieceTable.Fields[FieldIndex];
			field.Locked = !field.Locked;
			PieceTable.ApplyChanges(DocumentModelChangeType.ToggleFieldLocked, field.FirstRunIndex, field.LastRunIndex);
		}
	}
	#endregion
	#region RemoveFieldHistoryItem
	public class RemoveFieldHistoryItem : RichEditHistoryItem {
		#region Fields
		Field deletedField;
		int removedFieldIndex = -1;
		List<Field> childFields;
		#endregion
		public RemoveFieldHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int RemovedFieldIndex { get { return removedFieldIndex; } set { removedFieldIndex = value; } }
		protected Field DeletedField { get { return deletedField; } }
		#endregion
		protected override void RedoCore() {
			FieldCollection fields = PieceTable.Fields;
			this.deletedField = fields[this.removedFieldIndex];
			fields.RemoveAt(this.removedFieldIndex);
			this.childFields = new List<Field>();
			for (int i = this.removedFieldIndex - 1; i >= 0; i--) {
				Field field = fields[i];
				if (field.Parent == this.deletedField) {
					this.childFields.Add(field);
					field.Parent = this.deletedField.Parent;
				}
			}
			DocumentModel.ApplyChanges(PieceTable, DocumentModelChangeType.Fields, RunIndex.Zero, RunIndex.Zero);
			DocumentModelStructureChangedNotifier.NotifyFieldRemoved(PieceTable, PieceTable, removedFieldIndex);
		}
		protected override void UndoCore() {
			PieceTable.Fields.Insert(this.removedFieldIndex, this.deletedField);
			int count = this.childFields.Count;
			for (int i = 0; i < count; i++)
				this.childFields[i].Parent = this.deletedField;
			DocumentModel.ApplyChanges(PieceTable, DocumentModelChangeType.Fields, deletedField.FirstRunIndex, deletedField.LastRunIndex);
			DocumentModelStructureChangedNotifier.NotifyFieldInserted(PieceTable, PieceTable, removedFieldIndex);
		}
	}
	#endregion
	#region RichEditFieldHistoryItem
	public abstract class RichEditFieldHistoryItem : RichEditHistoryItem {
		int[] childFieldIndices;
		protected RichEditFieldHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int[] ChildFieldIndices { get { return childFieldIndices; } }
		protected internal void SetAsParentForAllChildren(Field field) {
			FieldCollection fields = PieceTable.Fields;
			childFieldIndices = PieceTable.GetChildFieldIndexes(field);
			if (childFieldIndices.Length > 0) {
				foreach (int childIndex in childFieldIndices) {
					if (fields[childIndex].Parent == field.Parent)
						fields[childIndex].Parent = field;
				}
			}
		}
		protected internal void RemoveParentFromAllChildren(Field removedField) {
			if (childFieldIndices == null)
				return;
			FieldCollection fields = PieceTable.Fields;
			foreach (int childIndex in childFieldIndices) {
				if (fields[childIndex].Parent == removedField)
					fields[childIndex].Parent = removedField.Parent;
			}
		}
	}
	#endregion
	#region InsertFieldHistoryItem
	public class InsertFieldHistoryItem : RichEditFieldHistoryItem {
		#region Fields
		Field insertedField;
		int insertedFieldIndex = -1;
		bool disableUpdate;
		#endregion
		public InsertFieldHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int InsertedFieldIndex { get { return insertedFieldIndex; } set { insertedFieldIndex = value; } }
		public Field InsertedField { get { return insertedField; } set { insertedField = value; } }
		#endregion
		public override void Execute() {
			PieceTable.Fields.Insert(this.insertedFieldIndex, this.insertedField);
			DocumentModelStructureChangedNotifier.NotifyFieldInserted(PieceTable, PieceTable, insertedFieldIndex);
		}
		protected override void RedoCore() {
			Field parentField = PieceTable.FindFieldByRunIndex(this.insertedField.FirstRunIndex);
			this.insertedField.Parent = parentField;
			SetAsParentForAllChildren(this.insertedField);
			Execute();
			this.insertedField.DisableUpdate = this.disableUpdate;
		}
		protected override void UndoCore() {
			this.insertedField.Parent = null;
			this.disableUpdate = this.insertedField.DisableUpdate;			
			PieceTable.Fields.RemoveAt(this.insertedFieldIndex);
			RemoveParentFromAllChildren(this.insertedField);
			DocumentModelStructureChangedNotifier.NotifyFieldRemoved(PieceTable, PieceTable, insertedFieldIndex);
		}
	}
	#endregion
	#region DisableUpdateChangedHistoryItem
	public class DisableUpdateChangedHistoryItem : RichEditHistoryItem {
		#region Fields
		readonly int fieldIndex;
		#endregion
		public DisableUpdateChangedHistoryItem(PieceTable pieceTable, int index)
			: base(pieceTable) {
				this.fieldIndex = index;
		}
		protected override void RedoCore() {
			ToggleDisableUpdate();
		}
		protected override void UndoCore() {
			ToggleDisableUpdate();
		}
		void ToggleDisableUpdate() {
			Field field = PieceTable.Fields[fieldIndex];
			field.ToggleDisableUpdate();
		}
	}
	#endregion
}
