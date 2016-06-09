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

using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public interface IModelModifier<T> where T : ICommandState {
		void Modify(T stateObject);
	}
	public abstract class IntervalModelModifier<T> : IModelModifier<IntervalCommandState> {
		public IntervalModelModifier(PieceTable pieceTable) {
			PieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get; private set; }
		public void Modify(IntervalCommandState stateObject) {
			ModifyCore(stateObject.Position, stateObject.Length, GetNewValue(stateObject.Value));
		}
		protected abstract void ModifyCore(DocumentLogPosition position, int length, T value);
		protected virtual T GetNewValue(object value) {
			return (T)value;
		}
	}
	public abstract class IntervalWithUseModelModifier : IModelModifier<IntervalWithUseCommandState> {
		public IntervalWithUseModelModifier(PieceTable pieceTable) {
			PieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get; private set; }
		public void Modify(IntervalWithUseCommandState stateObject) {
			ModifyCore(stateObject.Position, stateObject.Length, stateObject.Value, stateObject.UseValue);
		}
		protected abstract void ModifyCore(DocumentLogPosition position, int length, object value, bool useValue);
	}
	public abstract class ListLevelModelModifier<T> : IModelModifier<ListLevelCommandState> {
		public ListLevelModelModifier(DocumentModel documentModel) {
			DocumentModel = documentModel;
		}
		protected DocumentModel DocumentModel { get; private set; }
		public void Modify(ListLevelCommandState stateObject) {
			ModifyCore(stateObject.IsAbstract, stateObject.ListIndex, stateObject.ListLevelIndex, GetNewValue(stateObject.Value));
		}
		protected abstract void ModifyCore(bool isAbstract, int listIndex, int listLevelIndex, T value);
		protected virtual T GetNewValue(object value) {
			return (T)value;
		}
	}
	public abstract class ListLevelWithUseModelModifier : IModelModifier<ListLevelWithUseCommandState> {
		public ListLevelWithUseModelModifier(DocumentModel documentModel) {
			DocumentModel = documentModel;
		}
		protected DocumentModel DocumentModel { get; private set; }
		public void Modify(ListLevelWithUseCommandState stateObject) {
			ModifyCore(stateObject.IsAbstract, stateObject.ListIndex, stateObject.ListLevelIndex, stateObject.Value, stateObject.UseValue);
		}
		protected abstract void ModifyCore(bool isAbstract, int listIndex, int listLevelIndex, object value, bool useValue);
	}
	public abstract class SectionModelModifier<T> : IModelModifier<SectionCommandState> {
		public SectionModelModifier(DocumentModel model) {
			DocumentModel = model;
		}
		protected DocumentModel DocumentModel { get; private set; }
		public void Modify(SectionCommandState stateObject) {
			ModifyCore(stateObject.SectionIndex, GetNewValue(stateObject.Value));
		}
		protected abstract void ModifyCore(SectionIndex sectionIndex, T value);
		protected virtual T GetNewValue(object value) {
			return (T)value;
		}
	}
	public abstract class TablePropertyModelModifier<T> : IModelModifier<TableState> {
		public TablePropertyModelModifier(PieceTable pieceTable) {
			PieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get; private set; }
		public void Modify(TableState stateObject) {
			ModifyCore(stateObject.TablePosition, stateObject.TableNestedLevel, GetNewValue(stateObject.Value));
		}
		protected abstract void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, T value);
		protected virtual T GetNewValue(object value) {
			return (T)value;
		}
	}
	public abstract class TablePropertyWithUseModelModifier<T> : IModelModifier<TablePropertyState> {
		public TablePropertyWithUseModelModifier(PieceTable pieceTable) {
			PieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get; private set; }
		public void Modify(TablePropertyState stateObject) {
			ModifyCore(stateObject.TablePosition, stateObject.TableNestedLevel, GetNewValue(stateObject.Value), stateObject.UseValue);
		}
		protected abstract void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, T value, bool useValue);
		protected virtual T GetNewValue(object value) {
			return (T)value;
		}
	}
	public abstract class TableComplexPropertyWithUseModelModifier<T> : IModelModifier<TablePropertyState> {
		public TableComplexPropertyWithUseModelModifier(PieceTable pieceTable) {
			PieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get; private set; }
		public void Modify(TablePropertyState stateObject) {
			var values = ((ArrayList)stateObject.Value).Cast<object>().Select(obj => GetNewValue(obj)).ToArray();
			ModifyCore(stateObject.TablePosition, stateObject.TableNestedLevel, values, stateObject.ComplexUseValues);
		}
		protected abstract void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, T[] values, bool[] useValues);
		protected virtual T GetNewValue(object value) {
			return (T)value;
		}
	}
	public abstract class TableRowPropertyModelModifier<T> : IModelModifier<TableRowState> {
		public TableRowPropertyModelModifier(PieceTable pieceTable) {
			PieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get; private set; }
		public void Modify(TableRowState stateObject) {
			ModifyCore(stateObject.TablePosition, stateObject.TableNestedLevel, stateObject.RowIndex, GetNewValue(stateObject.Value));
		}
		protected abstract void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, int rowIndex, T value);
		protected virtual T GetNewValue(object value) {
			return (T)value;
		}
	}
	public abstract class TableRowPropertyWithUseModelModifier<T> : IModelModifier<TableRowPropertyState> {
		public TableRowPropertyWithUseModelModifier(PieceTable pieceTable) {
			PieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get; private set; }
		public void Modify(TableRowPropertyState stateObject) {
			ModifyCore(stateObject.TablePosition, stateObject.TableNestedLevel, stateObject.RowIndex, GetNewValue(stateObject.Value), stateObject.UseValue);
		}
		protected abstract void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, int rowIndex, T value, bool useValue);
		protected virtual T GetNewValue(object value) {
			return (T)value;
		}
	}
	public abstract class TableCellPropertyModelModifier<T> : IModelModifier<TableCellState> {
		public TableCellPropertyModelModifier(PieceTable pieceTable) {
			PieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get; private set; }
		public void Modify(TableCellState stateObject) {
			ModifyCore(stateObject.TablePosition, stateObject.TableNestedLevel, stateObject.RowIndex, stateObject.CellIndex, GetNewValue(stateObject.Value));
		}
		protected abstract void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, int rowIndex, int cellIndex, T value);
		protected virtual T GetNewValue(object value) {
			return (T)value;
		}
	}
	public abstract class TableCellPropertyWithUseModelModifier<T> : IModelModifier<TableCellPropertyState> {
		public TableCellPropertyWithUseModelModifier(PieceTable pieceTable) {
			PieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get; private set; }
		public void Modify(TableCellPropertyState stateObject) {
			ModifyCore(stateObject.TablePosition, stateObject.TableNestedLevel, stateObject.RowIndex, stateObject.CellIndex, GetNewValue(stateObject.Value), stateObject.UseValue);
		}
		protected abstract void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, int rowIndex, int cellIndex, T value, bool useValue);
		protected virtual T GetNewValue(object value) {
			return (T)value;
		}
	}
	public abstract class TableCellComplexPropertyWithUseModelModifier<T> : IModelModifier<TableCellPropertyState> {
		public TableCellComplexPropertyWithUseModelModifier(PieceTable pieceTable) {
			PieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get; private set; }
		public void Modify(TableCellPropertyState stateObject) {
			var values = ((ArrayList)stateObject.Value).Cast<object>().Select(obj => GetNewValue(obj)).ToArray();
			ModifyCore(stateObject.TablePosition, stateObject.TableNestedLevel, stateObject.RowIndex, stateObject.CellIndex, values, stateObject.ComplexUseValues);
		}
		protected abstract void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, int rowIndex, int cellIndex, T[] values, bool[] useValues);
		protected virtual T GetNewValue(object value) {
			return (T)value;
		}
	}
}
