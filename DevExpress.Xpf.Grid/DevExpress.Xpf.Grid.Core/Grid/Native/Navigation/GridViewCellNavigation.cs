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
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;
using System.Collections.Generic;
using System.Collections;
namespace DevExpress.Xpf.Grid.Native {
	public class GridViewCellNavigation : GridViewRowNavigation {
		public GridViewCellNavigation(DataViewBase view) : base(view) { }
		public override void OnHome(KeyEventArgs e) {
			if(ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
				View.SelectRowForce();
				base.OnHome(e);
			} else
				View.MoveFirstNavigationIndex();
		}
		public override void OnEnd(KeyEventArgs e) {
			if(ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
				View.SelectRowForce();
				base.OnEnd(e);
			} else
				View.MoveLastNavigationIndex();
		}
		public override void OnLeft(bool isCtrlPressed) {
			if(UseAdvHorzNavigation)
				DoBandedViewHorzNavigation(false);
			else
				OnLeftCore(isCtrlPressed);
		}
		void OnLeftCore(bool isCtrlPressed) {
			if(View.IsAdditionalRowFocused && View.IsRootView) {
				View.MovePrevCellCore();
				return;
			}
			if(View.FocusedRowElement == null)
				return;
			if(!View.IsExpandableRowFocused()) {
				View.MovePrevCell();
			} else {
				base.OnLeft(isCtrlPressed);
			}
		}
		public override void OnRight(bool isCtrlPressed) {
			if(UseAdvHorzNavigation)
				DoBandedViewHorzNavigation(true);
			else
				OnRightCore(isCtrlPressed);
		}
		protected void DoBandedViewHorzNavigation(bool isRight) {
			ColumnBase found = FindNavigationColumn(isRight);
			if(found != null)
				DataControl.CurrentColumn = found;
		}
		ColumnBase FindNavigationColumn(bool isRight) {
			List<List<List<ColumnBase>>> bandRows = CreateBandRows(DataControl.BandsLayoutCore.VisibleBands);
			if(bandRows.Count == 0) return null;
			int bandIndex, rowIndex, columnIndex;
			FindColumn(DataControl.CurrentColumn, bandRows, out bandIndex, out rowIndex, out columnIndex);
			if(isRight) {
				if(columnIndex < bandRows[bandIndex][rowIndex].Count - 1)
					return bandRows[bandIndex][rowIndex][columnIndex + 1];
				if(bandIndex >= bandRows.Count - 1 && (!View.ViewBehavior.AutoMoveRowFocusCore || View.IsAdditionalRowFocused))
					return null;
				if(bandIndex < bandRows.Count - 1)
					bandIndex++;
				else if (View.ViewBehavior.AutoMoveRowFocusCore && !View.IsAdditionalRowFocused)
					bandIndex = 0;
				if(rowIndex >= bandRows[bandIndex].Count)
					rowIndex = bandRows[bandIndex].Count - 1;
				return bandRows[bandIndex][rowIndex][0];
			}
			else {
				if(columnIndex > 0)
					return bandRows[bandIndex][rowIndex][columnIndex - 1];
				if(bandIndex <= 0 && (!View.ViewBehavior.AutoMoveRowFocusCore || View.IsAdditionalRowFocused))
					return null;
				if(bandIndex > 0)
					bandIndex--;
				else if(View.ViewBehavior.AutoMoveRowFocusCore && !View.IsAdditionalRowFocused)
					bandIndex = bandRows.Count - 1;
				if(rowIndex >= bandRows[bandIndex].Count)
					rowIndex = bandRows[bandIndex].Count - 1;
				return bandRows[bandIndex][rowIndex][bandRows[bandIndex][rowIndex].Count - 1];
			}
		}
		List<List<List<ColumnBase>>> CreateBandRows(IList bands) {
			List<List<List<ColumnBase>>> bandRows = new List<List<List<ColumnBase>>>();
			CreateBandRowsCore(bands, bandRows);
			return bandRows;
		}
		void CreateBandRowsCore(IList bands, List<List<List<ColumnBase>>> bandRows) {
			foreach(BandBase band in bands) {
				if(band.BandsCore.Count != 0)
					CreateBandRowsCore(band.BandsCore, bandRows);
				else {
					if(band.ActualRows.Count == 0)
						continue;
					List<List<ColumnBase>> bandRow = new List<List<ColumnBase>>();
					for(int i = 0; i < band.ActualRows.Count; i++) {
						List<ColumnBase> columnRow = new List<ColumnBase>();
						foreach(ColumnBase column in band.ActualRows[i].Columns) {
							if(View.IsColumnNavigatable(column, false))
								columnRow.Add(column);
						}
						if(columnRow.Count > 0)
							bandRow.Add(columnRow);
					}
					if(bandRow.Count > 0)
						bandRows.Add(bandRow);
				}
			}
		}
		void FindColumn(ColumnBase column, List<List<List<ColumnBase>>> bandRows, out int bandIndex, out int rowIndex, out int columnIndex) {
			bandIndex = 0;
			rowIndex = 0;
			columnIndex = 0;
			for(int i = 0; i < bandRows.Count; i++) {
				for(int j = 0; j < bandRows[i].Count; j++) {
					for(int k = 0; k < bandRows[i][j].Count; k++) {
						if(bandRows[i][j][k] == column) {
							bandIndex = i;
							rowIndex = j;
							columnIndex = k;
							return;
						}
					}
				}
			}
		}
		void OnRightCore(bool isCtrlPressed) {
			if(View.IsAdditionalRowFocused && View.IsRootView) {
				View.MoveNextCellCore();
				return;
			}
			if(View.FocusedRowElement == null)
				return;
			if(!View.IsExpandableRowFocused()) {
				View.MoveNextCell();
			} else {
				base.OnRight(isCtrlPressed);
				View.MoveFirstNavigationIndex();
			}
		}
		public override void OnTab(bool isShiftPressed) {
			if(View.IsAdditionalRowFocused) {
				if(isShiftPressed)
					View.MovePrevCell(true);
				else
					View.MoveNextCell(true);
				return;
			}
			TabNavigation(isShiftPressed);
		}
		protected override void SetRowFocus(DependencyObject row, bool focus) {
			SetRowFocusOnCell(row, focus);
			base.SetRowFocus(row, focus);
		}
		protected internal override void ClearAllStates() {
			ClearAllCellsStates();
		}
		protected internal override void ProcessMouse(DependencyObject originalSource) {
			base.ProcessMouse(originalSource);
			ProcessMouseOnCell(originalSource);
		}
		public override bool CanSelectCell { get { return true; } }
		public override bool GetIsFocusedCell(int rowHandle, ColumnBase column) {
			return View.IsFocusedView && View.CalcActualFocusedRowHandle() == rowHandle && DataControl.CurrentColumn == column;
		}
		public override void OnDown() {
			if(UseAdvVertNavigation)
				DoVertNavigation(base.OnDown, true);
			else
				base.OnDown();
		}
		public override void OnUp(bool allowNavigateToAutoFilterRow) {
			if(UseAdvVertNavigation)
				DoVertNavigation(() => base.OnUp(allowNavigateToAutoFilterRow), false);
			else
				base.OnUp(allowNavigateToAutoFilterRow);
		}
		bool UseAdvVertNavigation { get { return DataControl.BandsLayoutCore != null && DataControl.BandsLayoutCore.AllowAdvancedVerticalNavigation && View.VisibleColumnsCore.Count != 0; } }
		protected bool UseAdvHorzNavigation { get { return !DataControl.IsGroupRowHandleCore(View.FocusedRowHandle) && DataControl.BandsLayoutCore != null && DataControl.BandsLayoutCore.AllowAdvancedHorizontalNavigation; } }
		void DoVertNavigation(Action action, bool isDown) {
			ColumnBase column = DataControl.CurrentColumn ?? View.VisibleColumnsCore[0];
			int columnIndex = column.BandRow.Columns.IndexOf(column);
			if(!DataControl.IsGroupRowHandleCore(View.FocusedRowHandle)) {
				int rowIndex = column.ParentBand.ActualRows.IndexOf(column.BandRow);
				if(TryDoVertNavigation(column.ParentBand, isDown, rowIndex, columnIndex))
					return;
			}
			int oldRowHandle = View.FocusedRowHandle;
			action();
			if(oldRowHandle == View.FocusedRowHandle || !UseAdvVertNavigation || DataControl.IsGroupRowHandleCore(View.FocusedRowHandle)) return;
			TryDoVertNavigation(column.ParentBand, isDown, isDown ? -1 : column.ParentBand.ActualRows.Count, columnIndex);
		}
		bool TryDoVertNavigation(BandBase band, bool isDown, int rowIndex, int columnIndex) {
			ColumnBase column = null;
			int diff = int.MaxValue;
			int delta = isDown ? 1 : -1;
			do {
				rowIndex += delta;
				if(rowIndex < 0 || rowIndex > band.ActualRows.Count - 1)
					break;
				for(int i = 0; i < band.ActualRows[rowIndex].Columns.Count; i++) {
					if(View.IsColumnNavigatable(band.ActualRows[rowIndex].Columns[i], false)) {
						int current = Math.Abs(i - columnIndex);
						if(diff <= current) continue;
						diff = current;
						column = band.ActualRows[rowIndex].Columns[i];
					}
				}
				if(column != null) {
					DataControl.CurrentColumn = column;
					return true;
				}
			} while(true);
			return false;
		}
	}
}
