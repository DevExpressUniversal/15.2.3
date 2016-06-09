#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class GridDeltaColumnControl : DashboardUserControl, IHolderOptionsControl {
		GridColumnDragSection section;
		int groupIndex;
		GridDashboardItem grid;
		GridColumnBase initialColumn;
		double fixedWidth;
		double weight;
		GridColumnFixedWidthType widthType;
		DataItem dataItem;
		GridDeltaColumn column;
		bool showZeroLevel;
		public bool Active { get { return column != null; } }
		public event EventHandler OptionsChanged;
		public GridDeltaColumnControl() {
			InitializeComponent();
		}
		IHistoryItem IHolderOptionsControl.CreateHistoryItem() {
			GridDeltaColumnDisplayMode displayMode = valueBarControl.DisplayMode == GridColumnValueBarDisplayMode.Bar ? GridDeltaColumnDisplayMode.Bar : GridDeltaColumnDisplayMode.Value;
			if(column == null) {
				Measure measure = DashboardWinHelper.CloneToMeasure(dataItem);
				DashboardWinHelper.SetCorrectSummaryType(measure, grid.DataSourceSchema);
				GridDeltaColumn newColumn = new GridDeltaColumn() {
					ActualValue = measure,
					DisplayMode = displayMode,
					AlwaysShowZeroLevel = showZeroLevel,
					FixedWidth = fixedWidth,
					WidthType = widthType,
					Weight = weight
				};
				newColumn.DeltaOptions.Assign(deltaOptionsControl.DeltaOptions);
				return new CreateGridColumnHistoryItem(section, groupIndex, grid, initialColumn, newColumn);
			}
			else
				return new ChangeGridDeltaColumnHistoryItem(grid, column, displayMode, showZeroLevel, deltaOptionsControl.DeltaOptions);
		}
		void RaiseOptionsChanged() {
			if(OptionsChanged != null)
				OptionsChanged(this, EventArgs.Empty);
		}
		void ValueBarControlDisplayModeChanged(object sender, ValueBarDisplayModeEventArgs e) {
			bool isBarMode = e.DisplayMode == GridColumnValueBarDisplayMode.Bar;
			showZeroLevelCheckEdit.Visible = isBarMode;
			deltaOptionsControl.Visible = !isBarMode;
			RaiseOptionsChanged();
		}
		void ShowZeroLevelCheckEditCheckedChanged(object sender, EventArgs e) {
			showZeroLevel = showZeroLevelCheckEdit.Checked;
			RaiseOptionsChanged();
		}
		void DeltaOptionsControlChanged(object sender, EventArgs e) {
			RaiseOptionsChanged();
		}
		public void Initialize(GridColumnDragSection section, int groupIndex, GridDashboardItem grid, GridColumnBase initialColumn) {
			this.section = section;
			this.groupIndex = groupIndex;
			this.grid = grid;
			this.initialColumn = initialColumn;
			this.fixedWidth = initialColumn.FixedWidth;
			this.widthType = initialColumn.WidthType;
			this.weight = initialColumn.Weight;
			dataItem = initialColumn.DataItems.FirstOrDefault();
			column = initialColumn as GridDeltaColumn;
			if(column != null) {
				valueBarControl.Initialize(column);
				showZeroLevelCheckEdit.Checked = showZeroLevel = column.AlwaysShowZeroLevel;
				deltaOptionsControl.PrepareOptions(column.DeltaOptions);
			}
		}
		public void SetAllowBarMode(bool allowBarMode) {
			if(!allowBarMode) {
				valueBarControl.DisableBarCheckEdit();
				showZeroLevelCheckEdit.Visible = false;
			}
		}
	}
}
