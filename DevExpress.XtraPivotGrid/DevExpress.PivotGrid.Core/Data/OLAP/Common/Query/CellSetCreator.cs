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
using DevExpress.PivotGrid.QueryMode;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.PivotGrid.QueryMode.TuplesTree;
namespace DevExpress.PivotGrid.OLAP {
	class CellSetCreator {
		public static CellSet<OLAPCubeColumn> CreateCellSet(OLAPCachedCellSet queryResult, IQueryContext<OLAPCubeColumn> context, IDataSourceHelpersOwner<OLAPCubeColumn> owner) {
			return new CellSetCreator(queryResult, context, owner).Parse();
		}
		OLAPCachedCellSet s2;
		IQueryContext<OLAPCubeColumn> context;
		LevelRecord columnRecord = null;
		IOLAPTuple prev = null;
		int[] toAddIndexes;
		IQueryMetadataColumn[] toAddMeasures;
		int toAddCount = 0;
		bool haveDataCells;
		CellSet<OLAPCubeColumn> set;
		int maxColumnNonAggregatable;
		int maxRowNonAggregatable;
		List<LevelRecord> rowGroups;
		Dictionary<IQueryMetadataColumn, int> rowColumnColumnsAreaIndexes;
		Dictionary<string, IQueryMetadataColumn> measuresByName;
		int dataAreaCount;
		List<int[]> toAddIndexesList;
		List<IQueryMetadataColumn[]> toAddMeasuresList;
		List<int> toAddCountList;
		List<LevelRecord> toAddColumnTuples;
		protected CellSetCreator(OLAPCachedCellSet s2, IQueryContext<OLAPCubeColumn> context, IDataSourceHelpersOwner<OLAPCubeColumn> owner) {
			this.s2 = s2;
			this.context = context;
			List<OLAPCubeColumn> dataArea = context.Areas.ServerSideDataArea;
			dataAreaCount = dataArea.Count;
			if(dataAreaCount > 0) {
				toAddIndexesList = new List<int[]>();
				toAddMeasuresList = new List<IQueryMetadataColumn[]>();
				toAddCountList = new List<int>();
				toAddColumnTuples = new List<LevelRecord>();
			}
			toAddIndexes = new int[dataAreaCount];
			toAddMeasures = new IQueryMetadataColumn[dataAreaCount];
			haveDataCells = context.Areas.ServerSideDataArea.Count > 0;
			measuresByName = new Dictionary<string, IQueryMetadataColumn>();
			for(int i = 0; i < dataArea.Count; i++)
				measuresByName.Add(dataArea[i].UniqueName, dataArea[i].Metadata);
			set = new CellSet<OLAPCubeColumn>(context, owner);
			maxColumnNonAggregatable = GetMaxNonAggregatable(context.Areas.ColumnArea);
			maxRowNonAggregatable = GetMaxNonAggregatable(context.Areas.RowArea);
			rowGroups = new List<LevelRecord>(1000);
			rowColumnColumnsAreaIndexes = new Dictionary<IQueryMetadataColumn, int>();
			for(int i = 0; i < context.Areas.ColumnArea.Count; i++)
				rowColumnColumnsAreaIndexes.Add(context.Areas.ColumnArea[i].Metadata, i);
			for(int i = 0; i < context.Areas.RowArea.Count; i++)
				rowColumnColumnsAreaIndexes.Add(context.Areas.RowArea[i].Metadata, i);
		}
		 int GetMaxNonAggregatable(List<OLAPCubeColumn> columns) {
			int level = -2;
			for(int i = 0; i < columns.Count; i++)
				if(!columns[i].IsAggregatable && columns[i].ParentColumn == null)
					level = i;
			return level;
		}
		public CellSet<OLAPCubeColumn> Parse() {
			OLAPMemberCreator rowCreator = new OLAPMemberCreator(context.Areas.RowArea, context.RowArea, context.RowExpand);
			int index = 0;
			OLAPMeasuredMemberCreator columnCreator = new OLAPMeasuredMemberCreator(context.Areas.ColumnArea, context.ColumnArea, context.ColumnExpand, context.Areas.ServerSideDataArea);
			index = 0;
			TuplesIndexedTreeCache<OLAPCubeColumn> columnsTree = set.ColumnIndexes;
			foreach(IOLAPTuple tuple in s2.ColumnAxis) {
				int tupleCount = tuple.Count;
				if(prev != null && prev.Count == tupleCount) {
					bool eq = true;
					for(int i = 0; i < tupleCount - 1; i++) {
						if(prev[i].UniqueName != tuple[i].UniqueName) {
							eq = false;
							break;
						}
					}
					if(eq) {
						if(haveDataCells) {
							toAddIndexes[toAddCount] = index;
							toAddMeasures[toAddCount] = measuresByName[tuple[tupleCount - 1].UniqueName];
							toAddCount++;
						}
						index++;
						continue;
					} else {
						AddDataCells(columnRecord);
						columnRecord = null;
					}
				}
				if(!columnCreator.SetCurrent(tuple)) {
					index++;
					prev = null;
					AddDataCells(columnRecord);
					columnRecord = null;
					continue;
				}
				bool prevNull = prev == null;
				prev = tuple;
				columnRecord = columnsTree.AddResultValues(columnCreator);
				if(haveDataCells) {
					toAddIndexes[toAddCount] = index;
					toAddMeasures[toAddCount] = measuresByName[tuple[tupleCount - 1].UniqueName];
					toAddCount++;
				}
				index++;
			}
			AddDataCells(columnRecord);
			index = 0;
			TuplesIndexedTreeCache<OLAPCubeColumn> rowsTree = set.RowIndexes;
			if(s2.RowAxis != null) {
				foreach(IOLAPTuple tuple in s2.RowAxis) {
					if(!rowCreator.SetCurrent(tuple)) {
						index++;
						continue;
					}
					AddRowGroup(rowsTree.AddResultValues(rowCreator), index);
					index++;
				}
			}
			if(index == 0) {
				QueryTuple tuple = new QueryTuple(GroupInfo.GrandTotalGroup, GroupInfo.GrandTotalGroup.Member);
				PreLastLevelRecord rec = new PreLastLevelRecord(GroupInfo.GrandTotalGroup.Member) { GroupInfo = GroupInfo.GrandTotalGroup };
				rowsTree.Root.AddRecord(rec);
				AddRowGroup(rec, 0);
			}
			ReadData();
			return set;
		}
		List<LevelRecord> rowsToAdd = new List<LevelRecord>();
		List<int> rowsToAddIndeces = new List<int>();
		void AddRowGroup(LevelRecord rowRecord, int rowIndex) {
			rowsToAdd.Add(rowRecord);
			rowsToAddIndeces.Add(rowIndex);
		}
		void ReadData() {
			if(!haveDataCells || toAddIndexesList.Count == 0)
				return;
			for(int k = 0; k < rowsToAdd.Count; k++) {
				LevelRecord rowRecord = rowsToAdd[k];
				int rowIndex = rowsToAddIndeces[k];
				if(!haveDataCells || rowRecord == null || !(maxRowNonAggregatable == -2 || rowColumnColumnsAreaIndexes[rowRecord.Member.Column] >= maxRowNonAggregatable))
					return;
				s2.MoveToRow(rowIndex);
				for(int i = 0; i < toAddIndexesList.Count; i++) {
					OlapMeasuresStorage cell = MeasureStorageKeepHelper.CreateBySummaryCount(toAddCountList[i])();
					int[] columnsIndexes = toAddIndexesList[i];
					IQueryMetadataColumn[] measures = toAddMeasuresList[i];
					int count = toAddCountList[i];
					for(int j = 0; j < count; j++)
						cell.SetFormattedValue(measures[j], s2.GetColumnValue(columnsIndexes[j]));
					set.SetRowValue(rowRecord, toAddColumnTuples[i], cell, rowGroups.Count);
				}
			}
		}
		void AddDataCells(LevelRecord columnRecord) {
			if(toAddCount == 0 || !haveDataCells)
				return;
			if(maxColumnNonAggregatable == -2 || rowColumnColumnsAreaIndexes[columnRecord.Member.Column] >= maxColumnNonAggregatable) {
				toAddIndexesList.Add(toAddIndexes);
				toAddMeasuresList.Add(toAddMeasures);
				toAddCountList.Add(toAddCount);
				toAddColumnTuples.Add(columnRecord);
			}
			toAddIndexes = new int[dataAreaCount];
			toAddMeasures = new IQueryMetadataColumn[dataAreaCount];
			toAddCount = 0;
		}
	}
}
