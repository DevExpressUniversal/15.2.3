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
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraVerticalGrid.Events;
namespace DevExpress.XtraVerticalGrid.Internal {
	public class FocusManager {
		const string NullName = "#NullName#";
		WeakReference owner;
		string name;
		BaseRow focusedRow;
		int lockCount;
		public BaseRow FocusedRow {
			get {
				if(Owner == null)
					return null;
				return focusedRow;
			}
			set {
				if(FocusedRow == value)
					return;
				if(value != null && (value.Grid != Owner || !value.Visible || !value.OptionsRow.AllowFocus))
					return;
				BaseRow nearestRow = Owner.GetNearestRowCanFocus(value);
				if(FocusedRow == nearestRow)
					return;
				name = GetName(nearestRow);
				BeginUpdate();
				try {
					Owner.CloseEditor();
				} catch(Exception exception) {
					CancelUpdate();
					throw exception;
				}
				EndUpdate();
			}
		}
		bool IsUpdateLocked { get { return 0 < lockCount; } }
		VGridControlBase Owner { get { return (VGridControlBase)owner.Target; } }
		public FocusManager(VGridControlBase owner) {
			this.owner = new WeakReference(owner);
		}
		public void BeginUpdate() {
			lockCount++;
		}
		public void CancelUpdate() {
			lockCount--;
		}
		public void EndUpdate() {
			CancelUpdate();
			Update();
		}
		public void Update() {
			if(IsUpdateLocked)
				return;
			if(name == GetName(focusedRow) &&
				((focusedRow == null || focusedRow.Grid != null) && focusedRow != null))
				return;
			if(name == null) {
				focusedRow = Owner.GetNearestRowCanFocus(Owner.VisibleRows[0]);
				if(focusedRow != null)
					name = focusedRow.Name;
				return;
			}
			if(name == NullName) {
				focusedRow = null;
			}
			BaseRow oldFocusedRow = FocusedRow;
			focusedRow = Owner.GetRowByName(name);
			FocusedRowChangedEventArgs e = new FocusedRowChangedEventArgs(FocusedRow, oldFocusedRow);
			IndexChangedEventArgs eInd = null;
			if(Owner.FocusedRecordCellIndex > Owner.CurrentNumCells - 1) {
				eInd = new IndexChangedEventArgs(Owner.CurrentNumCells - 1, Owner.FocusedRecordCellIndex);
				Owner.fFocusedRecordCellIndex = Owner.CurrentNumCells - 1;
			} else if(Owner.FocusedRecordCellIndex < 0 || Owner.CurrentNumCells > 0) {
				eInd = new IndexChangedEventArgs(0, Owner.FocusedRecordCellIndex);
				Owner.fFocusedRecordCellIndex = 0;
			}
			Owner.InternalMakeRowVisible(e.Row, e.OldRow);
			Owner.RaiseFocusedRowChanged(e);
			if(eInd != null && eInd.NewIndex != eInd.OldIndex) {
				Owner.RaiseFocusedRecordCellChanged(eInd);
			}
		}
		string GetName(BaseRow row) {
			return row == null ? NullName : row.Name;
		}
	}
}
