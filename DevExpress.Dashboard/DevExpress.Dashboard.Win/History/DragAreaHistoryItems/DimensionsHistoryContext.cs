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

using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public class DimensionsHistoryContext {
		class UndoTopNMeasuresItem {
			public Measure Measure { get; private set; }
			public bool? Enabled { get; private set; }
			public UndoTopNMeasuresItem(Measure measure, bool? enabled) {
				Measure = measure;
				Enabled = enabled;
			}
		}
		readonly DataDashboardItem dashboardItem;
		readonly Dictionary<Dimension, Measure> undoSortByMeasures = new Dictionary<Dimension, Measure>();
		readonly Dictionary<Dimension, UndoTopNMeasuresItem> undoTopNMeasures = new Dictionary<Dimension, UndoTopNMeasuresItem>();
		public DimensionsHistoryContext(DataDashboardItem dashboardItem) {
			this.dashboardItem = dashboardItem;
			foreach (Dimension dimension in dashboardItem.GetDimensions()) {
				Measure undoSortByMeasure = dimension.SortByMeasure;
				if(ShouldReplaceMeasure(undoSortByMeasure)) {
					dimension.SortByMeasure = FindMeasureForReplace(undoSortByMeasure);
					undoSortByMeasures.Add(dimension, undoSortByMeasure);
				}
				Measure undoTopNMeasure = dimension.TopNOptions.Measure;
				if(ShouldReplaceMeasure(undoTopNMeasure)) {
					Measure redoTopNMeasure = FindMeasureForReplace(undoTopNMeasure);
					UndoTopNMeasuresItem item = null;
					if(redoTopNMeasure != null) 
						item = new UndoTopNMeasuresItem(undoTopNMeasure, null);
					else {
						item = new UndoTopNMeasuresItem(undoTopNMeasure, dimension.TopNOptions.Enabled);
						dimension.TopNOptions.Enabled = false;
					}
					dimension.TopNOptions.Measure = redoTopNMeasure;
					undoTopNMeasures.Add(dimension, item);
				}
			}
		}
		public void Undo() {
			foreach(KeyValuePair<Dimension, Measure> pair in undoSortByMeasures) {
				Dimension dimension = pair.Key;
				Measure undoSortByMeasure = pair.Value;
				dimension.SortByMeasure = undoSortByMeasure;
			}
			foreach(KeyValuePair<Dimension, UndoTopNMeasuresItem> pair in undoTopNMeasures) {
				Dimension dimension = pair.Key;
				UndoTopNMeasuresItem item = pair.Value;
				dimension.TopNOptions.Measure = item.Measure;
				if(item.Enabled.HasValue)
					dimension.TopNOptions.Enabled = item.Enabled.Value;
			}
		}
		bool ShouldReplaceMeasure(Measure measure) {
			return measure != null && !dashboardItem.DataItemRepository.Contains(measure);
		}
		Measure FindMeasureForReplace(Measure measure) {
			foreach (Measure m in dashboardItem.Measures)
				if (m.EqualsByDefinition(measure))
					return m;
			return null;
		}
	}
}
