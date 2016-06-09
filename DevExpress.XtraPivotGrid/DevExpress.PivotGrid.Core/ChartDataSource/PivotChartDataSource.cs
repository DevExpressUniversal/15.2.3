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

using System.Collections.Generic;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL && !DXPORTABLE
using System.Timers;
#endif
namespace DevExpress.XtraPivotGrid {
	public class PivotChartDataSource : PivotChartDataSourceBase {
		Timer selectionChangedTimer;
		public PivotChartDataSource(PivotGridData data) : base(data) { }
		protected override List<PivotChartDataSourceRowBase> GetCells() {
			if(Data.IsLockUpdate || Data.IsLoading)
				return CreateDataSourceRowsCollection();
			else
				return SelectionOnly ? SelectedCells : AllCells;
		}
		public override void OnCellSelectionChanged() {
			PivotGridOptionsChartDataSource options = Options as PivotGridOptionsChartDataSource;
			if(options.SelectionOnly) {
				if(options.UpdateDelay == 0)
					InvalidateOnCellSelectionChanged();
				else
					StartSelectionChangedTimer(options.UpdateDelay);
			}
			base.OnCellSelectionChanged();
		}
		void InvalidateOnCellSelectionChanged() {
			if(Options.AutoTransposeChart)
				InvalidateChartData();
			else
				InvalidateChartDataSourceCells();
		}
		public override void OnFocusedCellChanged() {
			PivotGridOptionsChartDataSource options = Options as PivotGridOptionsChartDataSource;
			if(options.SelectionOnly) {
				if(options.UpdateDelay == 0)
					InvalidateOnCellSelectionChanged();
				else
					StartSelectionChangedTimer(options.UpdateDelay);
			}
			base.OnFocusedCellChanged();
		}
		List<PivotChartDataSourceRowBase> SelectedCells {
			get {
				SelectionVisualItems visualItems = Data.VisualItems as SelectionVisualItems;
				if(visualItems == null)
					return CreateDataSourceRowsCollection();
				visualItems.CorrectSelection();
				List<PivotChartDataSourceRowBase> cells = new List<PivotChartDataSourceRowBase>(visualItems.SelectedCells.Count + 1);
				foreach(Point cell in visualItems.SelectedCells)
					cells.Add(CreateDataSourceRow(cell));
				if(cells.Count == 0 && visualItems.ColumnCount > 0 && visualItems.RowCount > 0)
					cells.Add(CreateDataSourceRow(visualItems.FocusedCell));
				return cells;
			}
		}
		Timer SelectionChangedTimer {
			get {
				if(selectionChangedTimer == null) {
					selectionChangedTimer = new Timer() { Enabled = false };
#if !SL && !DXPORTABLE
					selectionChangedTimer.Elapsed += OnSelectionChangedTimerElapsed;
#else
					selectionChangedTimer.Tick += OnSelectionChangedTimerElapsed;
#endif
				}
				return selectionChangedTimer;
			}
		}
		void StartSelectionChangedTimer(int updateDelay) {
			SelectionChangedTimer.Enabled = false;
#if !SL
			SelectionChangedTimer.Interval = updateDelay;
#else
			SelectionChangedTimer.Interval = TimeSpan.FromMilliseconds(updateDelay);
#endif
			SelectionChangedTimer.Enabled = true;
		}
		void OnSelectionChangedTimerElapsed(object sender, EventArgs e) {
			SelectionChangedTimer.Enabled = false;
			if(!Data.IsLocked) {
				InvalidateChartDataSourceInMainThread();
			}
		}
		protected virtual void InvalidateChartDataSourceInMainThread() {
			InvalidateOnCellSelectionChanged();
		}
		#region IPivotGrid Members
		public override bool SelectionSupported {
			get { return true; }
		}
		public override bool SelectionOnly {
			get { return (Options as PivotGridOptionsChartDataSource).SelectionOnly; }
			set { (Options as PivotGridOptionsChartDataSource).SelectionOnly = value; }
		}
		public override int UpdateDelay {
			get { return (Options as PivotGridOptionsChartDataSource).UpdateDelay; }
			set { (Options as PivotGridOptionsChartDataSource).UpdateDelay = value; }
		}
		#endregion
	}
}
