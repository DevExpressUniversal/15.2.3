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
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.Handler;
namespace DevExpress.XtraGrid.Views.Grid.ViewInfo {
	public class GridSelectionInfo : BaseSelectionInfo { 
		public GridSelectionInfo(GridView gv) : base(gv) {
		}
		protected override int NoneHitTest { get { return (int)GridHitTest.None; } }
		protected override int DefaultState { get { return (int)GridState.Normal; } }
		public new GridView View { get { return base.View as GridView; } }
		public new GridHitInfo PressedInfo { 
			get { 
				GridHitInfo res = base.PressedInfo as GridHitInfo; 
				if(res == null) res = GridHitInfo.Empty;
				return res;
			} 
		}
		public new GridHitInfo HotTrackedInfo { 
			get { 
				GridHitInfo res = base.HotTrackedInfo as GridHitInfo; 
				if(res == null) res = GridHitInfo.Empty;
				return res;
			} 
		}
		protected override int GetRowHandle(BaseHitInfo hitInfo) {
			if(hitInfo == null) return base.GetRowHandle(hitInfo);
			return ((GridHitInfo)hitInfo).RowHandle;
		}
		public GridColumn PressedColumn { 
			get { 
				return IsPressed() && 
					PressedColumnFilter == null && (PressedInfo.InColumn) ? PressedInfo.Column : null;
			}
		}
		public GridColumn HotTrackedColumn {
			get {
				if(HotTrackedInfo == null || HotTrackedInfo.Column == null) return null;
				if(HotTrackedInfo.InColumn || HotTrackedInfo.InGroupColumn || HotTrackedColumnFilterButton != null) return HotTrackedInfo.Column;
				return null;
			}
		}
		public GridColumn HotTrackedColumnFilterButton { 
			get { 
				return  IsHotTrack((int)GridHitTest.ColumnFilterButton) ||
					IsHotTrack((int)GridHitTest.GroupPanelColumnFilterButton) ?
					HotTrackedInfo.Column : null;
			}
		}
		public bool HotTrackedColumnFilterButtonInGroup {
			get {
				return IsHotTrack(GridHitTest.GroupPanelColumnFilterButton);
			}
		}
		public virtual bool IsHotTrack(GridHitTest test) { return IsHotTrack((int)test); }
		public GridColumn PressedColumnFilter { 
			get { 
				return IsPressed() && 
					(PressedInfo.HitTest == GridHitTest.ColumnFilterButton ||
					PressedInfo.HitTest == GridHitTest.GroupPanelColumnFilterButton) ? PressedInfo.Column : null;
			}
		}
		public GridColumn PressedColumnInGroup {
			get {
				return IsPressed() && PressedColumnFilter == null && 
					!PressedInfo.InColumnPanel ? PressedInfo.Column : null;
			}
		}
		public bool PressedFilterPanelCloseButton { get { return IsPressed((int)GridHitTest.FilterPanelCloseButton); }	}
		protected override void CreateStates() {
			fValidHotTracks = new int[fGvalidHotTracks.Length];
			Array.Copy(fGvalidHotTracks, fValidHotTracks, fGvalidHotTracks.Length);
			fValidPressedHitTests = new int[fGvalidPressedHitTests.Length];
			Array.Copy(fGvalidPressedHitTests, fValidPressedHitTests, fGvalidPressedHitTests.Length);
			fViewPressedStates = new int[fGgridStates.Length];
			Array.Copy(fGgridStates, fViewPressedStates, fGgridStates.Length);
		}
		static GridHitTest[] fGvalidHotTracks = {GridHitTest.ColumnFilterButton,
												  GridHitTest.FilterPanelActiveButton,
												  GridHitTest.FilterPanelText, 
												  GridHitTest.FilterPanelMRUButton, 
												  GridHitTest.FilterPanelCustomizeButton,
												  GridHitTest.FilterPanelCloseButton, GridHitTest.GroupPanelColumnFilterButton,
												  GridHitTest.RowCell, GridHitTest.CellButton, GridHitTest.Column, GridHitTest.GroupPanelColumn, GridHitTest.Row, GridHitTest.RowIndicator, 
												  GridHitTest.Footer, GridHitTest.RowFooter, GridHitTest.RowEdge};
		static GridHitTest[] fGvalidPressedHitTests = {GridHitTest.Column, 
														GridHitTest.FilterPanelActiveButton,
														GridHitTest.FilterPanelText, 
														GridHitTest.FilterPanelMRUButton, 
														GridHitTest.FilterPanelCustomizeButton,
														GridHitTest.ColumnFilterButton,	GridHitTest.FilterPanelCloseButton, 
														GridHitTest.ColumnButton, GridHitTest.GroupPanelColumn, 
														GridHitTest.GroupPanelColumnFilterButton};
		static GridState[] fGgridStates = { GridState.ColumnDown, 
											GridState.FilterPanelActiveButtonPressed,
											GridState.FilterPanelTextPressed, 
											GridState.FilterPanelMRUButtonPressed, 
											GridState.FilterPanelCustomizeButtonPressed,
											GridState.ColumnFilterDown, GridState.FilterPanelCloseButtonPressed,
											GridState.ColumnButtonDown, GridState.ColumnDown,
											GridState.ColumnFilterDown};
		protected override void DoSetState(int newState) {
			View.SetStateCore((GridState)newState);
		}
		protected override int GetState() { return View.StateInt; }
		protected override bool IsHotEquals(BaseHitInfo h1, BaseHitInfo h2) {
			GridHitInfo gh1 = h1 as GridHitInfo, gh2 = h2 as GridHitInfo;
			if((gh1.HitTest == gh2.HitTest &&
				(gh1.InRowCell == gh2.InRowCell)) &&
				gh1.Column == gh2.Column &&
				gh1.RowHandle == gh2.RowHandle) {
				return true;
			}
			return false;
		}
		protected override bool IsPressedEquals(BaseHitInfo h1, BaseHitInfo h2) {
			GridHitInfo gh1 = h1 as GridHitInfo, gh2 = h2 as GridHitInfo;
			if(gh2.HitTest == gh1.HitTest &&
				gh2.InColumnPanel == gh1.InColumnPanel &&
				gh2.InGroupPanel == gh1.InGroupPanel && gh1.Column == gh2.Column) {
				return true;
			}
			return false;
		}
	}
}
