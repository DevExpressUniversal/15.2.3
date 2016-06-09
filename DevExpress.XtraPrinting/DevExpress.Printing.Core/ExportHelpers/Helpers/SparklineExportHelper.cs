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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.Printing.ExportHelpers.Helpers {
	class SparklineExportHelper<TCol, TRow> : ExportHelperBase<TCol, TRow> where TRow : class, IRowBase where TCol : class, IColumn {
		readonly IEnumerable<TCol> sparklineColumns;
		HashSet<IEnumerable> sparklineData;
		public SparklineExportHelper(ExporterInfo<TCol, TRow> exportInfo) : base(exportInfo){
			this.sparklineColumns = ExportInfo.GridColumns.Where(col => col.ColEditType == ColumnEditTypes.Sparkline && col.SparklineInfo != null);
			this.sparklineData = new HashSet<IEnumerable>();
		}
		public void Execute(int percentage){
			if(sparklineColumns.Any()){
				AssignSparklinesRanges();
				ExportSparklineData(percentage);
			}
		}
		void AssignSparklinesRanges(){
			CacheValues();
			if(sparklineData.Count > 0){
				foreach(var col in sparklineColumns){
					foreach(var rangeitem in ExportInfo.GroupsList){
						var spGroup = new XlSparklineGroup();
						AssignSettings(spGroup, col);
						if(  rangeitem.End != 0){
							int endGroup = rangeitem.End;
							if(Equals(rangeitem, ExportInfo.GroupsList.Last())) endGroup--;
							AddSparklines(spGroup, col, sparklineData.ElementAt(dataSheetRow).Count() - 1, rangeitem.Start, endGroup);
						}
						ExportInfo.Sheet.SparklineGroups.Add(spGroup);
					}
				}
			}
		}
		void CacheValues(){
			foreach(var col in sparklineColumns){
				ExportInfo.Helper.ForAllRows(ExportInfo.View, x =>{
					var value = ExportInfo.View.GetRowCellValue(x, col);
					var castedValue = value as IEnumerable;
					if(castedValue != null) {
						var correctedSet = CorrectValues(castedValue);
						sparklineData.Add(correctedSet);
					}
				});
			}
		}
		private IEnumerable CorrectValues(IEnumerable castedValue) {
			int valueCnt = 0;
			var ret = new List<object>();
			foreach(var item in castedValue) {
				if (valueCnt >= ExportInfo.Exporter.DocumentOptions.MaxColumnCount) break;
				ret.Add(item);
				valueCnt++;
			}
			return ret;
		}
		int dataSheetRow;
		void AddSparklines(XlSparklineGroup spGroup, TCol col,int dataRowLen, int startGroup, int endGroup){
			int location = startGroup;
			for(int i = 0; i < endGroup - startGroup + 1; i++){
				var sparkline = new XlSparkline(GetDataRange(dataRowLen,dataSheetRow), GetLocation(col, location));
				dataSheetRow++;
				location++;
				spGroup.Sparklines.Add(sparkline);
			}
		}
		XlCellRange GetLocation(TCol col, int location){
			return new XlCellRange(
				new XlCellPosition(col.LogicalPosition, location, XlPositionType.Absolute, XlPositionType.Absolute),
				new XlCellPosition(col.LogicalPosition, location, XlPositionType.Absolute, XlPositionType.Absolute));
		}
		XlCellRange GetDataRange(int dataRowLen,int index){
			return new XlCellRange(
				new XlCellPosition(0, index, XlPositionType.Absolute, XlPositionType.Absolute),
				new XlCellPosition(dataRowLen, index, XlPositionType.Absolute, XlPositionType.Absolute)){SheetName = ExportInfo.View.AdditionalSheetInfo.Name}; 
		}
		void AssignSettings(XlSparklineGroup spGroup, TCol col){
			if(col.SparklineInfo == null) return;
			spGroup.SparklineType = col.SparklineInfo.SparklineType;
			spGroup.ColorSeries = col.SparklineInfo.ColorSeries;
			spGroup.ColorFirst = col.SparklineInfo.ColorFirst;
			spGroup.ColorHigh = col.SparklineInfo.ColorHigh;
			spGroup.ColorLast = col.SparklineInfo.ColorLast;
			spGroup.ColorLow = col.SparklineInfo.ColorLow;
			spGroup.ColorMarker = col.SparklineInfo.ColorMarker;
			spGroup.ColorNegative = col.SparklineInfo.ColorNegative;
			spGroup.HighlightFirst = col.SparklineInfo.HighlightFirst;
			spGroup.HighlightHighest = col.SparklineInfo.HighlightHighest;
			spGroup.HighlightLast = col.SparklineInfo.HighlightLast;
			spGroup.HighlightLowest = col.SparklineInfo.HighlightLowest;
			spGroup.HighlightNegative = col.SparklineInfo.HighlightNegative;
			spGroup.LineWeight = col.SparklineInfo.LineWeight;
			spGroup.DisplayMarkers = col.SparklineInfo.DisplayMarkers;
		}
		void ExportSparklineData(int percentage){
			ExportInfo.Exporter.EndSheet();
			var sheet = ExportInfo.Exporter.BeginSheet();
			sheet.Name = ExportInfo.View.AdditionalSheetInfo.Name;
			sheet.VisibleState = ExportInfo.View.AdditionalSheetInfo.VisibleState;
			for(int i = 0; i < sparklineData.Count; i++){
				ExportInfo.Exporter.BeginRow();
				var items = sparklineData.ElementAt(i);
				foreach(var item in items){
					var cell = ExportInfo.Exporter.BeginCell();
					cell.Value = XlVariantValue.FromObject(item);
					ExportInfo.Exporter.EndCell();
				}
				ExportInfo.Exporter.EndRow();
				ExportInfo.ExportRowIndex++;
				ExportInfo.ReportProgress(ExportInfo.ExportRowIndex, ref percentage);
			}
		}
	}
	internal static class EnumerableExtensions {
		public static int Count(this IEnumerable source){
			int res = 0;
			foreach(var item in source)
			   res++;
			return res;
		}
	}
}
