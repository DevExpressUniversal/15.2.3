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
using DevExpress.XtraExport;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
using System.Collections.Generic;
namespace DevExpress.Printing.ExportHelpers{
	internal class MergeInfo{
		public int RowHandle { get; set; }
		public XlVariantValue Value { get; set; }
	}
	public class ExportCellMerger<TCol, TRow> : ExportHelperBase<TCol, TRow> 
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		Dictionary<object, MergeInfo> mergeInfo;
		public ExportCellMerger(ExporterInfo<TCol, TRow> exportInfo): base(exportInfo){
		}
		Dictionary<object, MergeInfo> MergeInformation{
			get{
				if(mergeInfo == null) mergeInfo = new Dictionary<object, MergeInfo>();
				return mergeInfo;
			}
		}
		bool CanMergeCurrentColoumn(TCol col){
			return ExportInfo.View.GetAllowMerge(col) && MergeInformation.ContainsKey(col);
		}
		public void ProcessVerticalMerging(int rowInd, TCol col, int colInd, IXlCell cell){
			if(!MergeInformation.ContainsKey(col))
				MergeInformation.Add(col, new MergeInfo {RowHandle = rowInd, Value = cell.Value });
			else{
				bool viewAllowMerge = ExportInfo.View.GetAllowMerge(col);
				bool canMerge = Equals(cell.Value, MergeInformation[col].Value) && viewAllowMerge;
				if(viewAllowMerge){
					int eventCanMerge = ExportInfo.View.RaiseMergeEvent(MergeInformation[col].RowHandle - 1, rowInd - 1, col);
					if(eventCanMerge < 0) canMerge = false;
					if(eventCanMerge == 0) canMerge = true;
				}
				if(!canMerge){
					MergeCellsCore(rowInd, col);
					MergeInformation[col].Value = cell.Value;
					MergeInformation[col].RowHandle = rowInd;
				}
			}
		}
		public void CompleteMergingInGroup(TRow lastRow, int endGroupIndex){
			MergedLastCells(lastRow, endGroupIndex);
			MergeInformation.Clear();
		}
		public void MergedLastCells(TRow gridRow, int endGroupIndex){
			if(gridRow == null) return;
			foreach(TCol gridColumn in ExportInfo.ColumnsInfoColl)
				if(CanMergeCurrentColoumn(gridColumn) && MergeInformation.Count != 0)
					MergeCellsCore(endGroupIndex, gridColumn);
		}
		public void Merge(XlCellRange range){
			try{
				ExportInfo.Sheet.MergedCells.Add(range);
			} catch(ArgumentException) { }
		}
		void MergeCellsCore(int endGroupIndex, TCol column){
			if(endGroupIndex - MergeInformation[column].RowHandle > 1){
				ExportInfo.Sheet.MergedCells.Add(XlCellRange.FromLTRB(column.LogicalPosition, MergeInformation[column].RowHandle,
					column.LogicalPosition, endGroupIndex - 1));
			}
		}
	}
}
