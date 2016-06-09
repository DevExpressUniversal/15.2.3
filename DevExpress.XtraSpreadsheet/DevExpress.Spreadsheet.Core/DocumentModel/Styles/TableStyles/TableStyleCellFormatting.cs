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

using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ActualTableStyleCellFormatting
	public class ActualTableStyleCellFormatting : IEnumerable<TableStyleFormatCellPositionInfo> {
		readonly Dictionary<int, TableStyleFormatCellPositionInfo> orderedCollection = new Dictionary<int, TableStyleFormatCellPositionInfo>();
		#region Properties
		public TableStyleFormatCellPositionInfo this[int order] { get { return orderedCollection[order]; } }
		public int Count { get { return orderedCollection.Count; } }
		public bool IsEmpty { get { return Count == 0; } }
		#endregion
		public void RegisterCellFormatting(int formatIndex, TableStyleFormatBordersOutlineInfo info) {
			int order = Count;
			TableStyleFormatCellPositionInfo item = new TableStyleFormatCellPositionInfo(formatIndex, order, info);
			orderedCollection.Add(order, item);
		}
		public void Clear() {
			orderedCollection.Clear();
		}
		#region CalculateCellFormatValue
		public T CalculateCellFormatValue<T>(CellFormatCache cache, IDifferentialFormatPropertyAccessor<T> accessor, T defaultValue) {
			foreach (TableStyleFormatCellPositionInfo info in this) {
				DifferentialFormat format = cache[info.DifferentialFormatIndex] as DifferentialFormat;
				if (format != null && accessor.GetApply(format))
					return accessor.GetValue(format);
			}
			return defaultValue;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		public T CalculateCellFormatValue<T>(CellFormatCache cache, IDifferentialFormatDisplayBorderPropertyAccessor<T> accessor, T defaultValue, out int order) {
			foreach (TableStyleFormatCellPositionInfo info in orderedCollection.Values) {
				order = info.Order;
				DifferentialFormat format = cache[info.DifferentialFormatIndex] as DifferentialFormat;
				bool isOutline = info.BordersOutlineInfo[accessor.BorderIndex];
				BorderOptionsInfo optionsInfo = format.BorderOptionsInfo;
				if (format != null && accessor.GetApply(optionsInfo, isOutline))
					return accessor.GetValue(format.BorderInfo, isOutline);
			}
			order = TableStyle.ElementsCount;
			return defaultValue;
		}
		#endregion
		public bool CheckApplyCellFormatValue(CellFormatCache cache, IDifferentialFormatPropertyAccessorBase accessor) {
			foreach (TableStyleFormatCellPositionInfo info in orderedCollection.Values) {
				DifferentialFormat format = cache[info.DifferentialFormatIndex] as DifferentialFormat;
				if (format != null && accessor.GetApply(format))
					return true;
			}
			return false;
		}
		#region IEnumerable<TableStyleFormatCellPositionInfo> Members
		public IEnumerator<TableStyleFormatCellPositionInfo> GetEnumerator() {
			return orderedCollection.Values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return orderedCollection.Values.GetEnumerator();
		}
		#endregion
	}
	#endregion
	#region TableStyleFormatBordersOutlineInfo (struct)
	public struct TableStyleFormatBordersOutlineInfo {
		#region Static Members
		static Dictionary<BorderIndex, int> borderIndexToMaskTranslationTable = GetBorderIndexToMaskTranslationTable();
		static Dictionary<BorderIndex, int> GetBorderIndexToMaskTranslationTable() {
			Dictionary<BorderIndex, int> result = new Dictionary<BorderIndex, int>();
			result.Add(BorderIndex.Left, LeftOutlineMask);
			result.Add(BorderIndex.Right, RightOutlineMask);
			result.Add(BorderIndex.Top, TopOutlineMask);
			result.Add(BorderIndex.Bottom, BottomOutlineMask);
			return result;
		}
		static TableStyleFormatBordersOutlineInfo CreateCore(byte packedValues) {
			TableStyleFormatBordersOutlineInfo result = new TableStyleFormatBordersOutlineInfo();
			result.packedValues = packedValues;
			return result;
		}
		public static TableStyleFormatBordersOutlineInfo CreatePositionInfo() {
			return CreateCore(LeftOutlineMask + RightOutlineMask + TopOutlineMask + BottomOutlineMask);
		}
		public static TableStyleFormatBordersOutlineInfo CreateRowInfo(bool leftOutline, bool rightOutline) {
			byte packedValues = TopOutlineMask + BottomOutlineMask;
			if (leftOutline)
				packedValues += LeftOutlineMask;
			if (rightOutline)
				packedValues += RightOutlineMask;
			return CreateCore(packedValues);
		}
		public static TableStyleFormatBordersOutlineInfo CreateColumnInfo(bool topOutline, bool bottomOutline) {
			byte packedValues = LeftOutlineMask + RightOutlineMask;
			if (topOutline)
				packedValues += TopOutlineMask;
			if (bottomOutline)
				packedValues += BottomOutlineMask;
			return CreateCore(packedValues);
		}
		public static TableStyleFormatBordersOutlineInfo CreateVectorInfo(bool firstOutline, bool lastOutline, bool isVertical) {
			return isVertical ? CreateColumnInfo(firstOutline, lastOutline) : CreateRowInfo(firstOutline, lastOutline);
		}
		#endregion
		#region Fields
		const int LeftOutlineMask = 1;
		const int RightOutlineMask = 2;
		const int TopOutlineMask = 4;
		const int BottomOutlineMask = 8;
		byte packedValues;
		#endregion
		public TableStyleFormatBordersOutlineInfo(bool leftOutline, bool rightOutline, bool topOutline, bool bottomOutline) {
			packedValues = 0;
			if (leftOutline)
				packedValues += LeftOutlineMask;
			if (rightOutline)
				packedValues += RightOutlineMask;
			if (topOutline)
				packedValues += TopOutlineMask;
			if (bottomOutline)
				packedValues += BottomOutlineMask;
		}
		#region Properties
		public bool this[BorderIndex index] { get { return PackedValues.GetBoolBitValue((uint)packedValues, (uint)borderIndexToMaskTranslationTable[index]); } }
		#endregion
	}
	#endregion
	#region TableStyleFormatCellPositionInfo (struct)
	public struct TableStyleFormatCellPositionInfo {
		#region Fields
		int differentialFormatIndex;
		int order;
		TableStyleFormatBordersOutlineInfo bordersOutlineInfo;
		#endregion
		public TableStyleFormatCellPositionInfo(int differentialFormatIndex, int order, TableStyleFormatBordersOutlineInfo bordersOutlineInfo) {
			this.differentialFormatIndex = differentialFormatIndex;
			this.order = order;
			this.bordersOutlineInfo = bordersOutlineInfo;
		}
		#region Properties
		public int DifferentialFormatIndex { get { return differentialFormatIndex; } }
		public int Order { get { return order; } }
		public TableStyleFormatBordersOutlineInfo BordersOutlineInfo { get { return bordersOutlineInfo; } }
		#endregion
	}
	#endregion
} 
