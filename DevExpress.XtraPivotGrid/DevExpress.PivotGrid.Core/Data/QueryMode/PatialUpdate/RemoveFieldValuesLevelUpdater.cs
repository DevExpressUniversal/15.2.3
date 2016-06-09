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
namespace DevExpress.PivotGrid.QueryMode {
	class RemoveFieldValuesLevelUpdater<TColumn> : UpdaterBase<TColumn> where TColumn : QueryColumn {
		bool isColumn;
		int level;
		public RemoveFieldValuesLevelUpdater(IPartialUpdaterOwner<TColumn> owner, bool isColumn, int level) : base(owner) {
			this.isColumn = isColumn;
			this.level = level;
		}
		public override void Update() {
			QueryAreas<TColumn> areas = Areas;
			CellTable<TColumn> cells = areas.Cells;
			if(isColumn) {
				List<GroupInfo> columns = new List<GroupInfo>();
				foreach(KeyValuePair<GroupInfo, GroupInfoColumn> group in cells)
					if(group.Key.Level >= level)
						columns.Add(group.Key);
				cells.RemoveItems(columns);
				UpdateFieldValues(Areas.ColumnValues, g => g.ForEach(g2 => g2.ClearChildren()), level - 1);
			} else {
				List<GroupInfo> rowsToRemove = new List<GroupInfo>();
				List<GroupInfo> tempRowsToRemove = new List<GroupInfo>();
				foreach(KeyValuePair<GroupInfo, GroupInfoColumn> group in cells) {
					foreach(KeyValuePair<GroupInfo, MeasuresStorage> summary in group.Value)
						if(summary.Key.Level >= level)
							tempRowsToRemove.Add(summary.Key);
					group.Value.RemoveItems(tempRowsToRemove);
					rowsToRemove.AddRange(tempRowsToRemove);
				}
				UpdateFieldValues(Areas.RowValues, g => g.ForEach(g2 => g2.ClearChildren()), level - 1);
			}
		}
	}
}
