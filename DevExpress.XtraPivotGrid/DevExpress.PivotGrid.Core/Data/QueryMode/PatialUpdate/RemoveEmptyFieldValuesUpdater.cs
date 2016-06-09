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
	class RemoveEmptyFieldValuesUpdater<TColumn> : UpdaterBase<TColumn> where TColumn : QueryColumn {
		public RemoveEmptyFieldValuesUpdater(IPartialUpdaterOwner<TColumn> owner) : base(owner) {
		}
		public override void Update() {
			QueryAreas<TColumn> areas = Areas;
			AreaFieldValues rowValues = areas.RowValues;
			AreaFieldValues columnValues = areas.ColumnValues;
			CellTable<TColumn> cells = areas.Cells;
			List<bool> empty = new List<bool>();
			for(int i = 0; i < rowValues.Count + 1; i++)
				empty.Add(true);
			List<GroupInfo> sums = new List<GroupInfo>();
			List<GroupInfo> columns = new List<GroupInfo>();
			if(areas.ServerSideDataArea.Count > 0) {
				foreach(KeyValuePair<GroupInfo, GroupInfoColumn> group in cells) {
					foreach(KeyValuePair<GroupInfo, MeasuresStorage> summary in group.Value) {
						if(summary.Value.Count == 0)
							sums.Add(summary.Key);
					}
					for(int i = 0; i < empty.Count; i++) {
						if(!empty[i])
							continue;
						MeasuresStorage info = null;
						if(!group.Value.TryGetValue(rowValues[i - 1], out info))
							continue;
						if(info != null && info.Count != 0)
							empty[i] = false;
					}
					group.Value.RemoveItems(sums);
					sums.Clear();
					if(group.Value.IsEmpty())
						columns.Add(group.Key);
				}
				List<GroupInfo> emptyInfos = new List<GroupInfo>();
				for(int i = 0; i < empty.Count; i++) {
					if(!empty[i] || !rowValues.ForAnyChild(i, (infoIndex) => empty[infoIndex + 1]))
						continue;
					emptyInfos.Add(rowValues[i - 1]);
				}
				rowValues.RemoveChildren(emptyInfos);
				for(int i = columns.Count - 1; i >= 0; i--) {
					int index = columnValues.IndexOf(columns[i]);
					if(!columnValues.ForAnyChild(index, (infoIndex) => columns.Contains(columnValues[infoIndex])))
						columns.RemoveAt(i);
				}
				cells.RemoveItems(columns);
				columnValues.RemoveChildren(columns);
			} else {
				areas.ClearCells();
			}
		}
	}
}
