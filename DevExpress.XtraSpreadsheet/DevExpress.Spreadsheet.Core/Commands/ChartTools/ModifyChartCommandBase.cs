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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ModifyChartCommandBase (abstract class)
	public abstract class ModifyChartCommandBase : SpreadsheetMenuItemSimpleCommand {
		protected ModifyChartCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override void ExecuteCore() {
			List<Chart> selectedCharts = GetSelectedCharts();
			if (selectedCharts.Count <= 0)
				return;
			DocumentModel.BeginUpdate();
			try {
				foreach (Chart chart in selectedCharts) {
					if (CanModifyChart(chart)) {
						chart.BeginUpdate();
						try {
							ModifyChart(chart);
						}
						finally {
							chart.EndUpdate();
						}
					}
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal List<Chart> GetSelectedCharts() {
			return GetSelectedCharts(ActiveSheet);
		}
		protected internal static List<Chart> GetSelectedCharts(Worksheet sheet) {
			List<Chart> result = new List<Chart>();
			List<int> pictureIndexes = sheet.Selection.SelectedDrawingIndexes;
			int count = pictureIndexes.Count;
			for (int i = 0; i < count; i++) {
				int index = pictureIndexes[i];
				Chart chart = sheet.DrawingObjects[index] as Chart;
				if (chart != null)
					result.Add(chart);
			}
			return result;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, InnerControl.DocumentModel.DocumentCapabilities.Charts, !InnerControl.IsInplaceEditorActive);
			List<Chart> selectedCharts = GetSelectedCharts();
			foreach (Chart chart in selectedCharts) {
				if (ShouldHideCommand(chart)) {
					state.Visible = false;
					return;
				}
			}
			state.Visible = true;
			if (selectedCharts.Count <= 0) {
				state.Enabled = false;
				return;
			}
			foreach (Chart chart in selectedCharts) {
				if (!CanModifyChart(chart)) {
					state.Enabled = false;
					return;
				}
			}
			bool initialChecked = IsChecked(selectedCharts[0]);
			for (int i = 1; i < selectedCharts.Count; i++) {
				if (IsChecked(selectedCharts[i]) != initialChecked) {
					state.Checked = false;
					return;
				}
			}
			state.Checked = initialChecked;
		}
		protected abstract bool CanModifyChart(Chart chart);
		protected abstract void ModifyChart(Chart chart);
		protected abstract bool IsChecked(Chart chart);
		protected abstract bool ShouldHideCommand(Chart chart);
	}
	#endregion
}
