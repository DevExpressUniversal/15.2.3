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
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
using DevExpress.Office;
using System.Drawing;
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CellRangeType
	public enum CellRangeType {
		SingleRange = 0,
		IntervalRange = 1,
		UnionRange = 3,
	}
	#endregion
	#region ICellRangeBase
	public interface ICellRangeBase : ISupportsExistingValuesEnumeration {
		#region Properties
		ICellTable Worksheet { get; }
		long CellCount { get; }
		CellRangeType RangeType { get; }
		#endregion
		ICellBase GetFirstCellUnsafe();
		VariantValue GetFirstCellValue();
		VariantValue GetCellValueRelative(int relativeColumn, int relativeRow);
		bool ContainsCell(ICellBase cell);
		bool ContainsCell(CellKey cellKey);
		bool Intersects(CellRangeBase rangeBase);
		bool IsAbsolute();
		bool IsRelative();
		bool Includes(CellRangeBase rangeBase);
		VariantValue UnionWith(CellRangeBase cellrange);
		VariantValue IntersectionWith(CellRangeBase cellrange);
		VariantValue ConcatinateWith(CellRangeBase cellrange);
		IEnumerable<ICellBase> GetExistingCellsEnumerable();
		IEnumerator<ICellBase> GetExistingCellsEnumerator(bool reversed);
		IEnumerator<ICellBase> GetExistingCellsEnumeratorByColumns();
		IEnumerator<ICellBase> GetAllCellsEnumerator();
		IEnumerable<ICellBase> GetAllCellsEnumerable();
		IEnumerator<CellKey> GetAllCellKeysEnumerator();
		IEnumerator<VariantValue> GetValuesEnumerator();
	}
	#endregion
	#region CellRangeBase (abstract class)
	public abstract class CellRangeBase : ICellRangeBase,  ICloneable<CellRangeBase> {  
		protected CellRangeBase() {
		}
		#region Properties
		public abstract int Width { get; }
		public abstract int Height { get; }
		public abstract long CellCount { get; }
		public abstract CellRangeType RangeType { get; }
		public abstract ICellTable Worksheet { get; set; }
		public abstract CellPosition TopLeft { get; set; }
		public abstract CellPosition BottomRight { get; set; }
		public CellPosition TopRight { get { return new CellPosition(BottomRight.Column, TopLeft.Row, BottomRight.ColumnType, TopLeft.RowType); } }
		public CellPosition BottomLeft { get { return new CellPosition(TopLeft.Column, BottomRight.Row, TopLeft.ColumnType, BottomRight.RowType); } }
		public abstract int AreasCount { get; }
		#endregion
		public abstract int GetMaxWidth();
		public abstract int GetMaxHeight();
		public abstract bool ContainsCell(ICellBase cell);
		public abstract bool ContainsCell(CellKey cellKey);
		public abstract bool ContainsCell(int columnIndex, int rowIndex);
		public abstract bool Intersects(CellRangeBase range);
		public abstract bool Includes(CellRangeBase rangeBase);
		public abstract VariantValue UnionWith(CellRangeBase cellrange);
		public abstract VariantValue IntersectionWith(CellRangeBase cellrange);
		public abstract VariantValue ConcatinateWith(CellRangeBase cellrange);
		public abstract List<Point> GetCornerPoints();
		public abstract CellRange GetFirstInnerCellRange();
		public abstract bool IsAbsolute();
		public abstract bool IsRelative();
		public abstract IEnumerator<ICellBase> GetAllCellsEnumerator();
		public abstract IEnumerator<ICellBase> GetExistingCellsEnumerator(bool reversed);
		public abstract IEnumerator<ICellBase> GetExistingCellsEnumeratorByColumns();
		public abstract IEnumerable<ICellBase> GetExistingCellsEnumerable();
		public abstract IEnumerable<ICellBase> GetExistingCellsEnumerable(bool reversed);
		public IEnumerator<VariantValue> GetExistingValuesEnumerator() {
			return new CellValuesEnumerator(GetExistingCellsEnumerable().GetEnumerator());
		}
		public abstract IEnumerator<CellKey> GetAllCellKeysEnumerator();
		public abstract IEnumerator<VariantValue> GetValuesEnumerator();
		public abstract IEnumerable<CellRange> GetAreasEnumerable();
		public IEnumerable<CellKey> GetAllCellKeysEnumerable() {
			return new Enumerable<CellKey>(GetAllCellKeysEnumerator());
		}
		public abstract ICellBase GetFirstCellUnsafe();
		public abstract VariantValue GetFirstCellValue();
		public abstract CellRangeBase GetWithoutIntervals();
		internal abstract bool IsColumnRangeInterval();
		internal abstract bool IsRowRangeInterval();
		internal abstract void AddRanges(List<CellRange> collection);
		public abstract CellRange GetAreaByIndex(int index);
		public abstract bool IsMerged { get; }
		public abstract CellRange GetCoveredRange();
		public abstract void EnlargeByMergedRanges();
		public abstract bool HasData();
		public IEnumerable<ICellBase> GetAllCellsEnumerable() {
			return new Enumerable<ICellBase>(GetAllCellsEnumerator());
		}
		public static CellRangeBase TryParse(string reference, WorkbookDataContext context) {
			ParsedExpression parsedExpression = context.ParseExpression(reference, OperandDataType.None, false);
			if (parsedExpression != null) {
				VariantValue cellRangeValue = parsedExpression.Evaluate(context);
				if (cellRangeValue.IsCellRange)
					return cellRangeValue.CellRangeValue;
			}
			return null;
		}
		public static CellRangeBase CreateRangeBase(Worksheet workSheet, string reference) {
			CellRangeBase result = CreateRangeBase(workSheet, reference, ',');
			if (result == null)
				throw new ArgumentException("Incorrect reference for union range");
			return result;
		}
		public static CellRangeBase CreateRangeBase(Worksheet workSheet, string reference, char delimiter) {
			string[] ranges = reference.Split(new char[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
			if (ranges.Length < 1)
				return null;
			if (ranges.Length == 1) {
				CellRange range = CellRange.TryCreate(workSheet, ranges[0]);
				if (range == null)
					return null;
				return range.TryConvertToCellInterval();
			}
			List<CellRangeBase> innerRanges = new List<CellRangeBase>();
			for (int i = 0; i < ranges.Length; i++) {
				CellRange tempRange = CellRange.TryCreate(workSheet, ranges[i]);
				if (tempRange == null)
					return null;
				innerRanges.Add(tempRange.TryConvertToCellInterval());
			}
			CellUnion union = new CellUnion(innerRanges);
			return union;
		}
		public bool IsEqualSurfacesWith(CellRangeBase cellrange) {
			List<Point> points1 = GetCornerPoints();
			List<Point> points2 = cellrange.GetCornerPoints();
			if (points1.Count != points2.Count)
				return false;
			for (int i = 0; i < points1.Count; i++)
				if (!points2.Contains(points1[i]))
					return false;
			return true;
		}
		#region ICloneable<CellRangeBase> Members
		public CellRangeBase Clone() {
			return CloneCore();
		}
		protected abstract CellRangeBase CloneCore();
		public abstract CellRangeBase Clone(ICellTable sheet);
		#endregion
		public abstract CellRangeBase GetShifted(Worksheet worksheet, CellPositionOffset offset);
		public abstract CellRangeBase GetShiftedAny(CellPositionOffset offset, ICellTable newWorksheet);
		public abstract CellRangeBase GetForceShifted(CellRange shiftArea, bool horizontal, bool positive);
		public override bool Equals(object obj) {
			CellRangeBase other = obj as CellRangeBase;
			if (other != null)
				return Object.ReferenceEquals(Worksheet, other.Worksheet);
			else
				return false;
		}
		public bool EqualsPosition(object obj) {
			CellRangeBase other = obj as CellRangeBase;
			if (other == null)
				return false;
			else
				return this.TopLeft.EqualsPosition(other.TopLeft) && this.BottomRight.EqualsPosition(other.BottomRight);
		}
		public override int GetHashCode() {
			return Worksheet.SheetId;
		}
		public VariantValue GetCellValueRelative(int relativeColumn, int relativeRow) {
			CellPosition topLeft = TopLeft;
			if (relativeRow < 0 || relativeColumn < 0)
				return VariantValue.ErrorValueNotAvailable;
			if (relativeRow >= Height) {
				if (Height != 1)
					return VariantValue.ErrorValueNotAvailable;
				relativeRow = 0;
			}
			if (relativeColumn >= Width) {
				if (Width != 1)
					return VariantValue.ErrorValueNotAvailable;
				relativeColumn = 0;
			}
			return Worksheet.GetCalculatedCellValue(topLeft.Column + relativeColumn, topLeft.Row + relativeRow);
		}
		public VariantValue GetCellValueRelativeCore(int relativeColumn, int relativeRow) {
			return Worksheet.GetCalculatedCellValue(TopLeft.Column + relativeColumn, TopLeft.Row + relativeRow);
		}
		public abstract VariantValue GetCellValueByZOrder(long cellIndex);
		public abstract string ToString(WorkbookDataContext context, bool includeSheetName);
		public abstract string ToString(bool includeSheetName);
		public string ToString(WorkbookDataContext context) {
			return ToString(context, false);
		}
		public abstract CellRangeBase GetWithModifiedPositionType(PositionType columnPositionType, PositionType rowPositionType);
		public CellRangeBase GetWithModifiedPositionType(PositionType positionType) {
			return GetWithModifiedPositionType(positionType, positionType);
		}
		public abstract CellRangeBase TryMergeWithRange(CellRangeBase anotherRange);
		public abstract CellRangeBase MergeWithRange(CellRangeBase anotherRange);
		public static CellRangeBase MergeRanges(CellRangeBase range1, CellRangeBase range2) {
			if (range1 == null)
				return range2;
			if (range2 == null)
				return range1;
			return range1.MergeWithRange(range2);
		}
		public CellRangeBase ExcludeRange(CellRangeBase what) {
			CellRangeBase result = null;
			switch (what.RangeType) {
				case CellRangeType.UnionRange:
					result = ExcludeUnion(this, what as CellUnion);
					break;
				case CellRangeType.IntervalRange:
				case CellRangeType.SingleRange:
					result = ExcludeRange(this, what as CellRange);
					break;
			}
			return result;
		}
		#region ExcludeRange helpers
		static IRangeExcludeHelper[] helpers = new IRangeExcludeHelper[16] {
			new RangeExcludeOverlapHelper(),		  
			new RangeExcludeRightHelper(),			
			new RangeExcludeLeftHelper(),			 
			new RangeExcludeVerticalCenterHelper(),   
			new RangeExcludeBottomHelper(),		   
			new RangeExcludeBottomRightHelper(),	  
			new RangeExcludeBottomLeftHelper(),	   
			new RangeExcludeBottomHoleHelper(),	   
			new RangeExcludeTopHelper(),			  
			new RangeExcludeTopRightHelper(),		 
			new RangeExcludeLeftTopHelper(),		  
			new RangeExcludeTopHoleHelper(),		  
			new RangeExcludeHorizontalCenterHelper(), 
			new RangeExcludeRightHoleHelper(),		
			new RangeExcludeLeftHoleHelper(),		 
			new RangeExcludeCenterHelper(),		   
		};
		internal static readonly List<CellRangeBase> EmptyRangeSet = new List<CellRangeBase>();
		CellRangeBase ExcludeUnion(CellRangeBase source, CellUnion what) {
			CellRangeBase result = source;
			foreach (CellRangeBase item in what.InnerCellRanges) {
				result = result.ExcludeRange(item);
				if (result == null)
					break;
			}
			return result;
		}
		List<CellRangeBase> ExcludeRangeCore(CellRangeBase source, CellRange what) {
			List<CellRangeBase> rangeList = new List<CellRangeBase>();
			switch (source.RangeType) {
				case CellRangeType.IntervalRange:
				case CellRangeType.SingleRange:
					if (source.Worksheet == what.Worksheet)
						rangeList = ExcludeRangeAsList(source as CellRange, what);
					else
						rangeList.Add(source);
					break;
				case CellRangeType.UnionRange:
					foreach (CellRangeBase item in (source as CellUnion).InnerCellRanges)
						rangeList.AddRange(ExcludeRangeCore(item, what));
					break;
			}
			return rangeList;
		}
		CellRangeBase ExcludeRange(CellRangeBase source, CellRange what) {
			CellRangeBase result = null;
			List<CellRangeBase> rangeList = ExcludeRangeCore(source, what);
			if (rangeList.Count == 1)
				result = rangeList[0];
			else if (rangeList.Count > 0) {
				result = new CellUnion(rangeList);
			}
			return result;
		}
		List<CellRangeBase> ExcludeRangeAsList(CellRange owner, CellRange what) {
			CellRange intersection = owner.Intersection(what);
			if (intersection == null) {
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(owner);
				return result;
			}
			IRangeExcludeHelper helper = GetExcludeHelper(owner, intersection);
			return helper.GetResult(owner, intersection);
		}
		static IRangeExcludeHelper GetExcludeHelper(CellRange range, CellRange intersection) {
			IRangeExcludeHelper result;
			int index = 0;
			if (intersection.TopLeft.Column > range.TopLeft.Column)
				index |= 1;
			if (intersection.BottomRight.Column < range.BottomRight.Column)
				index |= 2;
			if (intersection.TopLeft.Row > range.TopLeft.Row)
				index |= 4;
			if (intersection.BottomRight.Row < range.BottomRight.Row)
				index |= 8;
			result = helpers[index];
			return result;
		}
		#region IRangeExcludeHelper
		interface IRangeExcludeHelper {
			List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange);
		}
		#endregion
		#region RangeExcludeLeftTopHelper
		class RangeExcludeLeftTopHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange part1 = new CellRange(sourceRange.Worksheet,
												new CellPosition(intersectionRange.BottomRight.Column + 1, sourceRange.TopLeft.Row, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												new CellPosition(sourceRange.BottomRight.Column, intersectionRange.BottomRight.Row, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange part2 = new CellRange(sourceRange.Worksheet,
												new CellPosition(sourceRange.TopLeft.Column, intersectionRange.BottomRight.Row + 1, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												sourceRange.BottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(part2); 
				result.Add(part1);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeTopHelper
		class RangeExcludeTopHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange item = new CellRange(sourceRange.Worksheet,
											   new CellPosition(sourceRange.TopLeft.Column, intersectionRange.BottomRight.Row + 1, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
											   sourceRange.BottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(item);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeTopHoleHelper
		class RangeExcludeTopHoleHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange part1 = new CellRange(sourceRange.Worksheet,
												sourceRange.TopLeft,
												new CellPosition(intersectionRange.TopLeft.Column - 1, intersectionRange.BottomRight.Row, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange part2 = new CellRange(sourceRange.Worksheet,
												new CellPosition(intersectionRange.BottomRight.Column + 1, sourceRange.TopLeft.Row, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												new CellPosition(sourceRange.BottomRight.Column, intersectionRange.BottomRight.Row, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange part3 = new CellRange(sourceRange.Worksheet,
												new CellPosition(sourceRange.TopLeft.Column, intersectionRange.BottomRight.Row + 1, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												sourceRange.BottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(part3);
				result.Add(part1);
				result.Add(part2);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeTopRightHelper
		class RangeExcludeTopRightHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange part1 = new CellRange(sourceRange.Worksheet,
												sourceRange.TopLeft,
												new CellPosition(intersectionRange.TopLeft.Column - 1, intersectionRange.BottomRight.Row, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange part2 = new CellRange(sourceRange.Worksheet,
												new CellPosition(sourceRange.TopLeft.Column, intersectionRange.BottomRight.Row + 1, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												sourceRange.BottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(part2); 
				result.Add(part1);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeLeftHelper
		class RangeExcludeLeftHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange item = new CellRange(sourceRange.Worksheet,
											   new CellPosition(intersectionRange.BottomRight.Column + 1, sourceRange.TopLeft.Row, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
											   sourceRange.BottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(item);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeLeftHoleHelper
		class RangeExcludeLeftHoleHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange item1 = new CellRange(sourceRange.Worksheet,
												sourceRange.TopLeft,
												new CellPosition(sourceRange.BottomRight.Column, intersectionRange.TopLeft.Row - 1, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange item2 = new CellRange(sourceRange.Worksheet,
												new CellPosition(intersectionRange.BottomRight.Column + 1, intersectionRange.TopLeft.Row, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												new CellPosition(sourceRange.BottomRight.Column, intersectionRange.BottomRight.Row, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange item3 = new CellRange(sourceRange.Worksheet,
												new CellPosition(sourceRange.TopLeft.Column, intersectionRange.BottomRight.Row + 1, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												sourceRange.BottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(item1);
				result.Add(item3); 
				result.Add(item2);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeCenterHelper
		class RangeExcludeCenterHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				ICellTable sourceRangeWorksheet = sourceRange.Worksheet;
				CellPosition sourceRangeTopLeft = sourceRange.TopLeft;
				CellPosition sourceRangeBottomRight = sourceRange.BottomRight;
				CellPosition intersectionRangeTopLeft = intersectionRange.TopLeft;
				CellPosition intersectionRangeBottomRight = intersectionRange.BottomRight;
				CellRange part1 = new CellRange(sourceRangeWorksheet,
												sourceRangeTopLeft,
												new CellPosition(sourceRangeBottomRight.Column, intersectionRangeTopLeft.Row - 1, sourceRangeBottomRight.ColumnType, sourceRangeBottomRight.RowType));
				CellRange part2 = new CellRange(sourceRangeWorksheet,
												new CellPosition(sourceRangeTopLeft.Column, intersectionRangeTopLeft.Row, sourceRangeTopLeft.ColumnType, sourceRangeTopLeft.RowType),
												new CellPosition(intersectionRangeTopLeft.Column - 1, intersectionRangeBottomRight.Row, sourceRangeBottomRight.ColumnType, sourceRangeBottomRight.RowType));
				CellRange part3 = new CellRange(sourceRangeWorksheet,
												new CellPosition(intersectionRangeBottomRight.Column + 1, intersectionRangeTopLeft.Row, sourceRangeTopLeft.ColumnType, sourceRangeTopLeft.RowType),
												new CellPosition(sourceRangeBottomRight.Column, intersectionRangeBottomRight.Row, sourceRangeBottomRight.ColumnType, sourceRangeBottomRight.RowType));
				CellRange part4 = new CellRange(sourceRangeWorksheet,
												new CellPosition(sourceRangeTopLeft.Column, intersectionRangeBottomRight.Row + 1, sourceRangeTopLeft.ColumnType, sourceRangeTopLeft.RowType),
												sourceRangeBottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(part1);
				result.Add(part2);
				result.Add(part3);
				result.Add(part4);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeOverlapHelper
		class RangeExcludeOverlapHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				return CellRangeBase.EmptyRangeSet; 
			}
			#endregion
		}
		#endregion
		#region RangeExcludeRightHelper
		class RangeExcludeRightHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange item = new CellRange(sourceRange.Worksheet,
											   sourceRange.TopLeft,
											   new CellPosition(intersectionRange.TopLeft.Column - 1, sourceRange.BottomRight.Row, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(item);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeRightHoleHelper
		class RangeExcludeRightHoleHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange item1 = new CellRange(sourceRange.Worksheet,
												sourceRange.TopLeft,
												new CellPosition(sourceRange.BottomRight.Column, intersectionRange.TopLeft.Row - 1, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange item2 = new CellRange(sourceRange.Worksheet,
												new CellPosition(sourceRange.TopLeft.Column, intersectionRange.TopLeft.Row, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												new CellPosition(intersectionRange.TopLeft.Column - 1, intersectionRange.BottomRight.Row, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange item3 = new CellRange(sourceRange.Worksheet,
												new CellPosition(sourceRange.TopLeft.Column, intersectionRange.BottomRight.Row + 1, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												sourceRange.BottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(item1);
				result.Add(item3); 
				result.Add(item2);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeBottomLeftHelper
		class RangeExcludeBottomLeftHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange item1 = new CellRange(sourceRange.Worksheet,
												sourceRange.TopLeft,
												new CellPosition(sourceRange.BottomRight.Column, intersectionRange.TopLeft.Row - 1, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange item2 = new CellRange(sourceRange.Worksheet,
												new CellPosition(intersectionRange.BottomRight.Column + 1, intersectionRange.TopLeft.Row, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												sourceRange.BottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(item1);
				result.Add(item2);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeBottomHelper
		class RangeExcludeBottomHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange item = new CellRange(sourceRange.Worksheet,
											   sourceRange.TopLeft,
											   new CellPosition(sourceRange.BottomRight.Column, intersectionRange.TopLeft.Row - 1, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(item);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeBottomHoleHelper
		class RangeExcludeBottomHoleHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange item1 = new CellRange(sourceRange.Worksheet,
												sourceRange.TopLeft,
												new CellPosition(sourceRange.BottomRight.Column, intersectionRange.TopLeft.Row - 1, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange item2 = new CellRange(sourceRange.Worksheet,
												new CellPosition(sourceRange.TopLeft.Column, intersectionRange.TopLeft.Row, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												new CellPosition(intersectionRange.TopLeft.Column - 1, sourceRange.BottomRight.Row, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange item3 = new CellRange(sourceRange.Worksheet,
												new CellPosition(intersectionRange.BottomRight.Column + 1, intersectionRange.TopLeft.Row, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												sourceRange.BottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(item1);
				result.Add(item2);
				result.Add(item3);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeBottomRightHelper
		class RangeExcludeBottomRightHelper :  IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange item1 = new CellRange(sourceRange.Worksheet,
												sourceRange.TopLeft,
												new CellPosition(sourceRange.BottomRight.Column, intersectionRange.TopLeft.Row - 1, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange item2 = new CellRange(sourceRange.Worksheet,
												new CellPosition(sourceRange.TopLeft.Column, intersectionRange.TopLeft.Row, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												new CellPosition(intersectionRange.TopLeft.Column - 1, sourceRange.BottomRight.Row, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(item1);
				result.Add(item2);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeBottomRightHelper
		class RangeExcludeVerticalCenterHelper : IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange item1 = new CellRange(sourceRange.Worksheet,
												sourceRange.TopLeft,
												new CellPosition(intersectionRange.TopLeft.Column - 1, sourceRange.BottomRight.Row, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange item2 = new CellRange(sourceRange.Worksheet,
												new CellPosition(intersectionRange.BottomRight.Column + 1, sourceRange.TopLeft.Row, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												sourceRange.BottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(item1);
				result.Add(item2);
				return result;
			}
			#endregion
		}
		#endregion
		#region RangeExcludeBottomRightHelper
		class RangeExcludeHorizontalCenterHelper : IRangeExcludeHelper {
			#region IRangeExcludeHelper Members
			public List<CellRangeBase> GetResult(CellRange sourceRange, CellRange intersectionRange) {
				CellRange item1 = new CellRange(sourceRange.Worksheet,
												sourceRange.TopLeft,
												new CellPosition(sourceRange.BottomRight.Column, intersectionRange.TopLeft.Row - 1, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType));
				CellRange item2 = new CellRange(sourceRange.Worksheet,
												new CellPosition(sourceRange.TopLeft.Column, intersectionRange.BottomRight.Row + 1, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType),
												sourceRange.BottomRight);
				List<CellRangeBase> result = new List<CellRangeBase>();
				result.Add(item1);
				result.Add(item2);
				return result;
			}
			#endregion
		}
		#endregion
		#endregion
		public abstract bool Exists(Predicate<CellRange> match);
		public abstract bool ForAll(Predicate<CellRange> p);
		public abstract void ForEach(Action<CellRange> action);
	}
	#endregion
	#region CellRange
	public class CellRange : CellRangeBase, ISheetPosition, ICloneable<CellRange> {
		#region Fields
		CellPosition topLeft;
		CellPosition bottomRight;
		ICellTable worksheet;
		#endregion
		public CellRange(ICellTable workSheet, CellPosition topLeft, CellPosition bottomRight) {
			this.topLeft = topLeft;
			this.bottomRight = bottomRight;
			this.worksheet = workSheet;
			Normalize();
		}
		#region Properties
		public override int Width { get { return BottomRight.Column - TopLeft.Column + 1; } }
		public override int Height { get { return BottomRight.Row - TopLeft.Row + 1; } }
		public override long CellCount { get { return (long)Width * (long)Height; } }
		public override CellRangeType RangeType { [DebuggerStepThrough] get { return CellRangeType.SingleRange; } }
		public override CellPosition TopLeft { [DebuggerStepThrough] get { return topLeft; } set { topLeft = value; } }
		public override CellPosition BottomRight { [DebuggerStepThrough]get { return bottomRight; } set { bottomRight = value; } }
		public override ICellTable Worksheet { [DebuggerStepThrough] get { return worksheet; } set { worksheet = value; } }
		public override bool IsMerged {
			get {
				CellRange found = WorksheetCasted.MergedCells.FindMergedCell(this.LeftColumnIndex, this.TopRowIndex);
				if (Equals(found))
					return true;
				return false;
			}
		}
		Worksheet WorksheetCasted { get { return (Worksheet)worksheet; } }
		public override int AreasCount { get { return 1; } }
		public override CellRange GetCoveredRange() {
			return this;
		}
		#endregion
		public override IEnumerator<ICellBase> GetAllCellsEnumerator() {
			return new CellRangeEnumerator(Worksheet, topLeft, bottomRight);
		}
		public IEnumerator<ICellBase> GetCellsForReadingEnumerator() {
			return new CellRangeForReadingEnumerator(WorksheetCasted, topLeft, bottomRight);
		}
		public override int GetMaxWidth() {
			return Width;
		}
		public override int GetMaxHeight() {
			return Height;
		}
		public CellPositionEnumerator GetAllPositionsEnumerator() {
			return new CellPositionEnumerator(topLeft, bottomRight);
		}
		public override IEnumerator<ICellBase> GetExistingCellsEnumerator(bool reversed) {
			External.ExternalWorksheet externalWorksheet = worksheet as External.ExternalWorksheet;
			if (externalWorksheet != null && externalWorksheet.ApiExternalWorksheet != null) {
				if (reversed)
					throw new ArgumentException("Reversed existing cell enumeration is not supported yet.");
				return new ExternalDataRangeEnumerator(externalWorksheet, topLeft, bottomRight);
			}
			if (reversed)
				return new CellRangeExistingCellsEnumerator(Worksheet, topLeft, bottomRight, reversed);
			return new CellRangeExistingCellsEnumeratorFast(Worksheet, topLeft, bottomRight);
		}
		public override IEnumerator<ICellBase> GetExistingCellsEnumeratorByColumns() {
			return new CellRangeExistingCellsEnumeratorByColumns(worksheet, topLeft, bottomRight);
		}
		public override IEnumerable<ICellBase> GetExistingCellsEnumerable() {
			return new Enumerable<ICellBase>(GetExistingCellsEnumerator(false));
		}
		public override IEnumerable<ICellBase> GetExistingCellsEnumerable(bool reversed) {
			return new Enumerable<ICellBase>(GetExistingCellsEnumerator(reversed));
		}
		public virtual IEnumerable<ICellBase> GetLayoutVisibleCellsEnumerable() {
			return new Enumerable<ICellBase>(new CellRangeLayoutVisibleCellsEnumerator(WorksheetCasted, topLeft, bottomRight, false));
		}
		public IEnumerable<ICellBase> GetLayoutVisibleCellsEnumerable(bool continuousRows) {
			return new Enumerable<ICellBase>(new CellRangeLayoutVisibleCellsEnumerator(WorksheetCasted, topLeft, bottomRight, continuousRows));
		}
		public override IEnumerator<CellKey> GetAllCellKeysEnumerator() {
			return new CellRangeCellKeysEnumerator(worksheet, topLeft, bottomRight);
		}
		public override IEnumerator<VariantValue> GetValuesEnumerator() {
			return new CellRangeValuesEnumerator(worksheet, topLeft, bottomRight);
		}
		public override IEnumerable<CellRange> GetAreasEnumerable() {
			yield return this;
		}
		public ICellBase GetCellRelative(int relativeColumn, int relativeRow) {
			return Worksheet.GetCell(topLeft.Column + relativeColumn, topLeft.Row + relativeRow);
		}
		public override VariantValue GetCellValueByZOrder(long cellIndex) {
			if (cellIndex < 0 || cellIndex >= CellCount)
				throw new IndexOutOfRangeException();
			int rowIndex = (int)(cellIndex / Width);
			int columnIndex = (int)(cellIndex - rowIndex * Width);
			return GetCellValueRelative(columnIndex, rowIndex);
		}
		public override ICellBase GetFirstCellUnsafe() {
			CellPosition topLeft = TopLeft;
			return Worksheet.TryGetCell(topLeft.Column, topLeft.Row);
		}
		public override VariantValue GetFirstCellValue() {
			CellPosition topLeft = TopLeft;
			return Worksheet.GetCalculatedCellValue(topLeft.Column, topLeft.Row);
		}
		public override CellRange GetAreaByIndex(int index) {
			if (index != 0)
				throw new ArgumentOutOfRangeException();
			return this;
		}
		public ICellBase TryGetCellRelative(int relativeColumn, int relativeRow) {
			return Worksheet.TryGetCell(topLeft.Column + relativeColumn, topLeft.Row + relativeRow);
		}
		public virtual CellRange GetRelativeToCurrent(int currentColumnIndex, int currentRowIndex) {
			CellPosition relatedTopLeft = TopLeft.GetRelativeToCurrent(currentColumnIndex, currentRowIndex);
			CellPosition relatedBottomRight = BottomRight.GetRelativeToCurrent(currentColumnIndex, currentRowIndex);
			return new CellRange(Worksheet, relatedTopLeft, relatedBottomRight);
		}
		public void Normalize() {
			if (topLeft.Row > bottomRight.Row) {
				int topLeftRow = bottomRight.Row;
				PositionType topLeftType = bottomRight.RowType;
				int bottomRightRow = topLeft.Row;
				PositionType bottomRightType = topLeft.RowType;
				topLeft = new CellPosition(topLeft.Column, topLeftRow, topLeft.ColumnType, topLeftType);
				bottomRight = new CellPosition(bottomRight.Column, bottomRightRow, bottomRight.ColumnType, bottomRightType);
			}
			if (topLeft.Column > bottomRight.Column) {
				int topLeftColumn = bottomRight.Column;
				PositionType topLeftType = bottomRight.ColumnType;
				int bottomRightColumn = topLeft.Column;
				PositionType bottomRightType = topLeft.ColumnType;
				topLeft = new CellPosition(topLeftColumn, topLeft.Row, topLeftType, topLeft.RowType);
				bottomRight = new CellPosition(bottomRightColumn, bottomRight.Row, bottomRightType, bottomRight.RowType);
			}
		}
		public override bool IsAbsolute() {
			return TopLeft.IsAbsolute && BottomRight.IsAbsolute;
		}
		public override bool IsRelative() {
			return TopLeft.IsRelative && BottomRight.IsRelative;
		}
		internal override void AddRanges(List<CellRange> collection) {
			collection.Add(this);
		}
		public override bool Intersects(CellRangeBase rangeBase) {
			switch (rangeBase.RangeType) {
				case CellRangeType.IntervalRange:
				case CellRangeType.SingleRange:
					CellRange cellRange = rangeBase.GetFirstInnerCellRange();
					if (!Object.ReferenceEquals(Worksheet, cellRange.Worksheet))
						return false;
					bool hasCommonColumns =
						Math.Max(TopLeft.Column, cellRange.TopLeft.Column) <= Math.Min(BottomRight.Column, cellRange.BottomRight.Column);
					bool hasCommonRows =
						Math.Max(TopLeft.Row, cellRange.TopLeft.Row) <= Math.Min(BottomRight.Row, cellRange.BottomRight.Row);
					return hasCommonColumns && hasCommonRows;
				case CellRangeType.UnionRange:
					return rangeBase.Intersects(this);
				default:
					throw new ArgumentException("Unknown CellRangeType " + rangeBase.RangeType);
			}
		}
		public override bool Includes(CellRangeBase rangeBase) {
			if (rangeBase == null)
				return false;
			switch (rangeBase.RangeType) {
				case CellRangeType.IntervalRange:
				case CellRangeType.SingleRange:
					CellRange cellRange = rangeBase.GetFirstInnerCellRange();
					if (!Object.ReferenceEquals(Worksheet, cellRange.Worksheet))
						return false;
					bool includedColumns =
						(TopLeft.Column <= cellRange.TopLeft.Column) &&
						(cellRange.BottomRight.Column <= BottomRight.Column);
					bool includedRows =
						(TopLeft.Row <= cellRange.TopLeft.Row) &&
						(cellRange.BottomRight.Row <= BottomRight.Row);
					return includedColumns && includedRows;
				case CellRangeType.UnionRange:
					foreach (CellRange currentRange in (rangeBase as CellUnion).InnerCellRanges)
						if (!this.Includes(currentRange))
							return false;
					return true;
				default:
					throw new ArgumentException("Unknown CellRangeType " + rangeBase.RangeType);
			}
		}
		public override VariantValue IntersectionWith(CellRangeBase range) {
			switch (range.RangeType) {
				case CellRangeType.IntervalRange:
				case CellRangeType.SingleRange:
					CellRange cellRange = range.GetFirstInnerCellRange();
					if (!Object.ReferenceEquals(Worksheet, cellRange.Worksheet))
						return VariantValue.ErrorInvalidValueInFunction;
					if (!Intersects(range))
						return VariantValue.ErrorNullIntersection;
					CellPosition topLeft = CellPosition.UnionPosition(TopLeft, cellRange.TopLeft, false);
					CellPosition bottomRight = CellPosition.UnionPosition(BottomRight, cellRange.BottomRight, true);
					VariantValue result = new VariantValue();
					result.CellRangeValue = PrepareCellRangeBaseValue(Worksheet, topLeft, bottomRight);
					return result;
				case CellRangeType.UnionRange:
					return range.IntersectionWith(this);
				default:
					throw new ArgumentException("Unknown CellRangeType " + range.RangeType);
			}
		}
		public static CellRange PrepareCellRangeBaseValue(ICellTable sheet, CellPosition topLeft, CellPosition bottomRight) {
			int maxRowCount = IndicesChecker.MaxRowCount;
			int maxColumnCount = IndicesChecker.MaxColumnCount;
			if (topLeft.Column <= 0 && bottomRight.Column >= maxColumnCount - 1)
				return CellIntervalRange.CreateRowInterval(sheet, topLeft.Row, topLeft.RowType, bottomRight.Row, bottomRight.RowType);
			if (topLeft.Row <= 0 && bottomRight.Row >= maxRowCount - 1)
				return CellIntervalRange.CreateColumnInterval(sheet, topLeft.Column, topLeft.ColumnType, bottomRight.Column, bottomRight.ColumnType);
			return new CellRange(sheet, topLeft, bottomRight);
		}
		public CellRange TryConvertToCellInterval() {
			int maxRowCount = IndicesChecker.MaxRowCount;
			int maxColumnCount = IndicesChecker.MaxColumnCount;
			if (topLeft.Row <= 0 && bottomRight.Row >= maxRowCount - 1)
				return CellIntervalRange.CreateColumnInterval(worksheet, topLeft.Column, topLeft.ColumnType, bottomRight.Column, bottomRight.ColumnType);
			if (topLeft.Column <= 0 && bottomRight.Column >= maxColumnCount - 1)
				return CellIntervalRange.CreateRowInterval(worksheet, topLeft.Row, topLeft.RowType, bottomRight.Row, bottomRight.RowType);
			return this;
		}
		public override VariantValue UnionWith(CellRangeBase range) {
			switch (range.RangeType) {
				case CellRangeType.IntervalRange:
				case CellRangeType.SingleRange:
					if (!Object.ReferenceEquals(Worksheet, range.Worksheet))
						return VariantValue.ErrorInvalidValueInFunction;
					CellRange cellRange = range.GetFirstInnerCellRange();
					CellPosition resultTopLeft;
					CellPosition resultBottomRight;
					if (TopLeft.Equals(BottomRight) && cellRange.TopLeft.Equals(cellRange.BottomRight)) {
						resultTopLeft = TopLeft;
						resultBottomRight = cellRange.TopLeft;
					}
					else {
						resultTopLeft = CellPosition.UnionPosition(TopLeft, cellRange.TopLeft, true);
						resultBottomRight = CellPosition.UnionPosition(BottomRight, cellRange.BottomRight, false);
					}
					VariantValue result = new VariantValue();
					result.CellRangeValue = PrepareCellRangeBaseValue(Worksheet, resultTopLeft, resultBottomRight);
					return result;
				case CellRangeType.UnionRange:
					return range.UnionWith(this);
				default:
					throw new ArgumentException("Unknown CellRangeType " + range.RangeType);
			}
		}
		public bool ContainsRange(CellRange range) {
			if (!Object.ReferenceEquals(range.Worksheet, this.Worksheet))
				return false;
			return ContainsRange(range.TopLeft, range.BottomRight);
		}
		public bool ContainsRange(CellPosition _topLeft, CellPosition _bottomRight) {
			return this.TopLeft.Column <= _topLeft.Column && this.BottomRight.Column >= _bottomRight.Column &&
				this.TopLeft.Row <= _topLeft.Row && this.BottomRight.Row >= _bottomRight.Row;
		}
		public override bool ContainsCell(ICellBase cell) {
			return Object.ReferenceEquals(cell.Sheet, worksheet) && ContainsCell(cell.ColumnIndex, cell.RowIndex);
		}
		public override bool ContainsCell(CellKey cellKey) {
			return worksheet.SheetId == cellKey.SheetId && ContainsCell(cellKey.ColumnIndex, cellKey.RowIndex);
		}
		public override bool ContainsCell(int columnIndex, int rowIndex) {
			return
				columnIndex >= TopLeft.Column &&
				columnIndex <= BottomRight.Column &&
				rowIndex >= TopLeft.Row &&
				rowIndex <= BottomRight.Row;
		}
		public override VariantValue ConcatinateWith(CellRangeBase cellrange) {
			CellRangeList resultList = new CellRangeList();
			resultList.Add(this);
			switch (cellrange.RangeType) {
				case CellRangeType.IntervalRange:
				case CellRangeType.SingleRange:
					resultList.Add(cellrange);
					break;
				case CellRangeType.UnionRange:
					resultList.AddRange((cellrange as CellUnion).InnerCellRanges);
					break;
				default:
					throw new ArgumentException("Unknown CellRangeType " + cellrange.RangeType);
			}
			CellUnion resultUnion = new CellUnion(resultList);
			if (!resultUnion.CheckSheetSameness())
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue resultValue = new VariantValue();
			resultValue.CellRangeValue = resultUnion;
			return resultValue;
		}
		public override CellRange GetFirstInnerCellRange() {
			return this;
		}
		public CellPositionOffset GetCellPositionOffset(ICellBase cell) {
			CellPositionOffset result = new CellPositionOffset(cell.ColumnIndex - TopLeft.Column, cell.RowIndex - TopLeft.Row);
			return result;
		}
		public CellPositionOffset GetCellPositionOffset(CellPosition pos) {
			CellPositionOffset result = new CellPositionOffset(pos.Column - TopLeft.Column, pos.Row - TopLeft.Row);
			return result;
		}
		public override CellRangeBase GetShifted(Worksheet worksheet, CellPositionOffset offset) {
			CellPosition topLeft = TopLeft.GetShifted(offset, worksheet);
			CellPosition bottomRight = BottomRight.GetShifted(offset, worksheet);
			return new CellRange(worksheet, topLeft, bottomRight);
		}
		public CellRange GetShifted(CellPositionOffset offset, ICellTable newWorksheet) {
			if (offset.ColumnOffset == 0 && offset.RowOffset == 0)
				return Clone(newWorksheet) as CellRange;
			CellPosition shiftedTopLefPosition = TopLeft.GetShifted(offset, newWorksheet);
			CellPosition shiftedBottomRightPosition = BottomRight.GetShifted(offset, newWorksheet);
			if (shiftedTopLefPosition.EqualsPosition(CellPosition.InvalidValue) || shiftedBottomRightPosition.EqualsPosition(CellPosition.InvalidValue))
				return null;
			return new CellRange(newWorksheet, shiftedTopLefPosition, shiftedBottomRightPosition);
		}
		public CellRange GetShifted(CellPositionOffset offset) {
			return GetShifted(offset, Worksheet);
		}
		public override CellRangeBase GetShiftedAny(CellPositionOffset offset, ICellTable newWorksheet) {
			if (offset.ColumnOffset == 0 && offset.RowOffset == 0)
				return Clone(newWorksheet) as CellRange;
			CellPosition shiftedTopLefPosition = TopLeft.GetShiftedAny(offset, newWorksheet);
			CellPosition shiftedBottomRightPosition = BottomRight.GetShiftedAny(offset, newWorksheet);
			if (shiftedTopLefPosition.EqualsPosition(CellPosition.InvalidValue) || shiftedBottomRightPosition.EqualsPosition(CellPosition.InvalidValue))
				return null;
			return new CellRange(newWorksheet, shiftedTopLefPosition, shiftedBottomRightPosition);
		}
		public CellRange GetShiftedAny(CellPositionOffset offset) {
			return (CellRange)GetShiftedAny(offset, Worksheet);
		}
		public CellRange GetShiftedAbsolute(CellPositionOffset offset, ICellTable worksheet) {
			if (offset.ColumnOffset == 0 && offset.RowOffset == 0)
				return Clone(worksheet) as CellRange;
			CellPosition shiftedTopLefPosition = TopLeft.GetShiftedAbsolute(offset, worksheet);
			CellPosition shiftedBottomRightPosition = BottomRight.GetShiftedAbsolute(offset, worksheet);
			if (shiftedTopLefPosition.EqualsPosition(CellPosition.InvalidValue) || shiftedBottomRightPosition.EqualsPosition(CellPosition.InvalidValue))
				return null;
			return new CellRange(worksheet, shiftedTopLefPosition, shiftedBottomRightPosition);
		}
		#region GetForceShifted
		public override CellRangeBase GetForceShifted(CellRange shiftArea, bool horizontal, bool positive) {
			CellPositionOffset offset;
			if (horizontal) {
				if (!NotificationsHelper.LeftRightShiftAffectsRange(shiftArea, this))
					return this;
				offset = new CellPositionOffset(positive ? shiftArea.Width : -shiftArea.Width, 0);
				if (NotificationsHelper.RangeHeightIsCovered(shiftArea, this) &&
				   (NotificationsHelper.RangeWidthIsCovered(shiftArea, this) || shiftArea.TopLeft.Column <= topLeft.Column))
					return GetShifted(offset, Worksheet);
			}
			else {
				if (!NotificationsHelper.UpDownShiftAffectsRange(shiftArea, this))
					return this;
				offset = new CellPositionOffset(0, positive ? shiftArea.Height : -shiftArea.Height);
				if (NotificationsHelper.RangeWidthIsCovered(shiftArea, this) &&
				   (NotificationsHelper.RangeHeightIsCovered(shiftArea, this) || shiftArea.TopLeft.Row <= topLeft.Row))
					return GetShifted(offset, Worksheet);
			}
			return GetForceShiftedCellUnion(GetSubRange(shiftArea, horizontal), offset);
		}
		CellRange GetSubRange(CellRange shiftArea, bool horizontal) {
			int firstColumnIndex = Math.Max(0, shiftArea.TopLeft.Column - topLeft.Column);
			int firstRowIndex = Math.Max(0, shiftArea.TopLeft.Row - topLeft.Row);
			int lastColumnIndex;
			int lastRowIndex;
			if (horizontal) {
				lastRowIndex = Math.Min(Height - 1, Height - 1 + shiftArea.BottomRight.Row - bottomRight.Row);
				lastColumnIndex = Width - 1;
			}
			else {
				lastColumnIndex = Math.Min(Width - 1, Width - 1 + shiftArea.BottomRight.Column - bottomRight.Column);
				lastRowIndex = Height - 1;
			}
			return GetSubRange(firstColumnIndex, firstRowIndex, lastColumnIndex, lastRowIndex);
		}
		CellUnion GetForceShiftedCellUnion(CellRange rangeToShift, CellPositionOffset offset) {
			CellRangeBase remainingRange = ExcludeRange(rangeToShift);
			CellRange shiftedRange = rangeToShift.GetShifted(offset, Worksheet);
			return new CellUnion(Worksheet, new List<CellRangeBase>() { shiftedRange, remainingRange });
		}
		#endregion
		public CellRange GetSubRange(int startCol, int startRow, int endCol, int endRow) {
			CellPosition topLeft = new CellPosition(TopLeft.Column + startCol, TopLeft.Row + startRow, this.TopLeft.ColumnType, this.TopLeft.RowType);
			if (!topLeft.IsValid)
				return null;
			CellPosition bottomRight = new CellPosition(TopLeft.Column + endCol, TopLeft.Row + endRow, this.BottomRight.ColumnType, this.BottomRight.RowType);
			if (!bottomRight.IsValid)
				return null;
			if (CellRange.CheckIsColumnRangeInterval(topLeft, bottomRight, Worksheet))
				return CellIntervalRange.CreateColumnInterval(Worksheet, topLeft.Column, topLeft.ColumnType, bottomRight.Column, bottomRight.ColumnType);
			if (CellRange.CheckIsRowRangeInterval(topLeft, bottomRight, Worksheet))
				return CellIntervalRange.CreateRowInterval(Worksheet, topLeft.Row, topLeft.RowType, bottomRight.Row, bottomRight.RowType);
			return new CellRange(Worksheet, topLeft, bottomRight);
		}
		public CellRange GetSubColumnRange(int startCol, int endCol) {
			return GetSubRange(startCol, 0, endCol, Height - 1);
		}
		protected internal CellRange GetSubColumnRange(int startCol, int endCol, int headerRowCount, int totalRowCount) {
			return GetSubRange(startCol, headerRowCount, endCol, Height - totalRowCount - 1);
		}
		public CellRange GetSubRowRange(int startRow, int endRow) {
			return GetSubRange(0, startRow, Width - 1, endRow);
		}
		public bool Resize(int topLeftColD, int topLeftRowD, int bottomRightColD, int bottomRightRowD) {
			TopLeft = new CellPosition(TopLeft.Column + topLeftColD, TopLeft.Row + topLeftRowD, TopLeft.ColumnType, TopLeft.RowType);
			BottomRight = new CellPosition(BottomRight.Column + bottomRightColD, BottomRight.Row + bottomRightRowD, BottomRight.ColumnType, BottomRight.RowType);
			return TopLeft.IsValid && BottomRight.IsValid;
		}
		public CellRange GetResized(int topLeftColD, int topLeftRowD, int bottomRightColD, int bottomRightRowD) {
			CellPosition topLeft = new CellPosition(TopLeft.Column + topLeftColD, TopLeft.Row + topLeftRowD, TopLeft.ColumnType, TopLeft.RowType);
			CellPosition bottomRight = new CellPosition(BottomRight.Column + bottomRightColD, BottomRight.Row + bottomRightRowD, BottomRight.ColumnType, BottomRight.RowType);
			if (!topLeft.IsValid || !bottomRight.IsValid)
				throw new ArgumentException("Resized range is invalid");
			return new CellRange(Worksheet, topLeft, bottomRight);
		}
		public CellRange GetResizedLimited(int topLeftColD, int topLeftRowD, int bottomRightColD, int bottomRightRowD) {
			CellPosition topLeft = new CellPosition(TopLeft.Column + topLeftColD, TopLeft.Row + topLeftRowD, TopLeft.ColumnType, TopLeft.RowType);
			CellPosition bottomRight = new CellPosition(
				Math.Min(BottomRight.Column + bottomRightColD, IndicesChecker.MaxColumnIndex),
				Math.Min(BottomRight.Row + bottomRightRowD, IndicesChecker.MaxRowIndex),
				BottomRight.ColumnType, BottomRight.RowType);
			if (!topLeft.IsValid)
				throw new ArgumentException("Resized range is invalid");
			return new CellRange(Worksheet, topLeft, bottomRight);
		}
		public override CellRangeBase GetWithoutIntervals() {
			return this;
		}
		public override List<Point> GetCornerPoints() {
			List<Point> result = new List<Point>();
			result.Add(new Point(TopLeft.Column, TopLeft.Row));
			result.Add(new Point(TopLeft.Column, BottomRight.Row + 1));
			result.Add(new Point(BottomRight.Column + 1, TopLeft.Row));
			result.Add(new Point(BottomRight.Column + 1, BottomRight.Row + 1));
			return result;
		}
		public override void EnlargeByMergedRanges() {
			Worksheet sheet = Worksheet as Worksheet;
			if (sheet != null) {
				List<CellRange> mergedRanges = sheet.MergedCells.GetMergedCellRangesIntersectsRange(this);
				foreach (CellRange mergedRange in mergedRanges) {
					TopLeft = CellPosition.UnionPosition(mergedRange.TopLeft, TopLeft, true);
					BottomRight = CellPosition.UnionPosition(mergedRange.BottomRight, BottomRight, false);
				}
			}
		}
		public int GetCellPositionColumnOffset(CellPosition position, RemoveCellMode mode) {
			if (mode == RemoveCellMode.ShiftCellsLeft && position.Column >= bottomRight.Column)
				if (position.Row >= topLeft.Row && position.Row <= bottomRight.Row)
					return Width;
			return 0;
		}
		public int GetCellPositionRowOffset(CellPosition position, RemoveCellMode mode) {
			if (mode == RemoveCellMode.ShiftCellsUp && position.Row >= bottomRight.Row)
				if (position.Column >= topLeft.Column && position.Column <= bottomRight.Column)
					return Height;
			return 0;
		}
		public int GetCellPositionColumnOffset(CellPosition position, InsertCellMode mode) {
			if (mode == InsertCellMode.ShiftCellsRight && position.Column >= topLeft.Column)
				if (position.Row >= topLeft.Row && position.Row <= bottomRight.Row)
					return Width;
			return 0;
		}
		public int GetCellPositionRowOffset(CellPosition position, InsertCellMode mode) {
			if (mode == InsertCellMode.ShiftCellsDown && position.Row >= topLeft.Row)
				if (position.Column >= topLeft.Column && position.Column <= bottomRight.Column)
					return Height;
			return 0;
		}
		public override string ToString() {
			return ToString(false);
		}
		public override string ToString(bool includeSheetName) {
			StringBuilder builder = new StringBuilder();
			if (includeSheetName)
				builder.Append(GetSheetName());
			if (this.RangeType != CellRangeType.SingleRange || !TopLeft.Equals(bottomRight)) {
				CellReferenceParser.ToString(TopLeft, builder);
				builder.Append(':');
				CellReferenceParser.ToString(BottomRight, builder);
			}
			else
				CellReferenceParser.ToString(TopLeft, builder);
			return builder.ToString();
		}
		protected string GetSheetName() {
			string worksheetName = Worksheet.Name;
			bool addQuotes = Model.SheetDefinition.ShouldAddQuotes(false, worksheetName, worksheetName);
			string format = (addQuotes) ? "'{0}'!" : "{0}!";
			return string.Format(format, worksheetName);
		}
		public override string ToString(WorkbookDataContext context, bool includeSheetName) {
			StringBuilder builder = new StringBuilder();
			if (includeSheetName)
				builder.Append(GetSheetName());
			if (this.RangeType != CellRangeType.SingleRange || !TopLeft.Equals(bottomRight)) {
				CellReferenceParser.ToString(TopLeft, context, builder);
				builder.Append(':');
				CellReferenceParser.ToString(BottomRight, context, builder);
			}
			else
				CellReferenceParser.ToString(TopLeft, context, builder);
			return builder.ToString();
		}
		#region Clone
		protected override CellRangeBase CloneCore() {
			return Clone();
		}
		public virtual new CellRange Clone() {
			return new CellRange(worksheet, topLeft, bottomRight);
		}
		public override CellRangeBase Clone(ICellTable sheet) {
			return new CellRange(sheet, TopLeft, BottomRight);
		}
		#endregion
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			CellRange other = obj as CellRange;
			if (other == null)
				return false;
			return topLeft.Equals(other.topLeft) && bottomRight.Equals(other.bottomRight);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), topLeft.GetHashCode(), bottomRight.GetHashCode());
		}
		public static CellRange Create(ICellTable workSheet, string reference) {
			return TryCreate(workSheet, reference);
		}
		public static CellRange TryCreate(ICellTable workSheet, string reference) {
			CellRange result = CellRangeFactory.Default.TryCreate(workSheet, reference);
			if (result != null)
				return result;
			return CellRangeFactory.CellIntervalRange.TryCreate(workSheet, reference);
		}
		public virtual CellRange Expand(CellRange otherRange) {
			CellPosition topLeft = new CellPosition(Math.Min(TopLeft.Column, otherRange.topLeft.Column), Math.Min(TopLeft.Row, otherRange.topLeft.Row));
			if (!topLeft.IsValid)
				return null;
			CellPosition bottomRight = new CellPosition(Math.Max(BottomRight.Column, otherRange.BottomRight.Column), Math.Max(BottomRight.Row, otherRange.BottomRight.Row));
			if (!bottomRight.IsValid)
				return null;
			return new CellRange(Worksheet, topLeft, bottomRight);
		}
		internal override bool IsColumnRangeInterval() {
			return CellRange.CheckIsColumnRangeInterval(topLeft, bottomRight, worksheet);
		}
		internal override bool IsRowRangeInterval() {
			return CellRange.CheckIsRowRangeInterval(topLeft, bottomRight, worksheet);
		}
		public bool IsWholeWorksheetRange() {
			return IsColumnRangeInterval() && CellRange.CheckIsRowRangeInterval(TopLeft, BottomRight, Worksheet as Model.Worksheet);
		}
		internal static bool CheckIsColumnRangeInterval(CellPosition topLeft, CellPosition bottomRight, ICellTable sheet) {
			return topLeft.Row <= 0 && bottomRight.Row >= IndicesChecker.MaxRowCount - 1;
		}
		internal static bool CheckIsRowRangeInterval(CellPosition topLeft, CellPosition bottomRight, ICellTable sheet) {
			return topLeft.Column <= 0 && bottomRight.Column >= IndicesChecker.MaxColumnCount - 1;
		}
		public override bool HasData() {
			foreach (ICellBase cell in GetExistingCellsEnumerable())
				if (!cell.Value.IsEmpty)
					return true;
			return false;
		}
		public override CellRangeBase GetWithModifiedPositionType(PositionType columnPositionType, PositionType rowPositionType) {
			CellPosition first = new CellPosition(topLeft.Column, topLeft.Row, columnPositionType, rowPositionType);
			CellPosition last = new CellPosition(bottomRight.Column, bottomRight.Row, columnPositionType, rowPositionType);
			return new CellRange(worksheet, first, last);
		}
		public override CellRangeBase TryMergeWithRange(CellRangeBase anotherRange) {
			if (anotherRange.RangeType == CellRangeType.UnionRange)
				return anotherRange.TryMergeWithRange(this);
			CellRange anotherCellRange = anotherRange as CellRange;
			if (Object.ReferenceEquals(Worksheet, anotherRange.Worksheet)) {
				CellRange expanded = this.Expand(anotherCellRange);
				long thisSquare = Width * Height;
				long anotherSquare = anotherCellRange.Width * anotherRange.Height;
				long expandedSquare = expanded.Width * expanded.Height;
				if (expandedSquare <= Math.Max(anotherSquare, thisSquare))
					return expanded; 
				if (expandedSquare <= anotherSquare + thisSquare) {
					if (TopLeft.Row == anotherRange.TopLeft.Row && BottomRight.Row == anotherRange.BottomRight.Row ||
						TopLeft.Column == anotherRange.TopLeft.Column && BottomRight.Column == anotherRange.BottomRight.Column)
						return expanded;
				}
			}
			return null;
		}
		public override CellRangeBase MergeWithRange(CellRangeBase anotherRange) {
			CellRangeBase merged = TryMergeWithRange(anotherRange);
			if (merged != null)
				return merged;
			if (anotherRange.RangeType == CellRangeType.UnionRange)
				return anotherRange.MergeWithRange(this);
			List<CellRangeBase> innerList = new List<CellRangeBase>();
			innerList.Add(this);
			innerList.Add(anotherRange);
			return new CellUnion(innerList);
		}
		#region ISheetPosition Members
		public int SheetId { get { return worksheet != null ? worksheet.SheetId : -1; } }
		public int LeftColumnIndex {
			get { return TopLeft.Column; }
			set {
				if (LeftColumnIndex == value)
					return;
				TopLeft = new CellPosition(value, TopRowIndex, TopLeft.ColumnType, TopLeft.RowType);
				Normalize();
			}
		}
		public int RightColumnIndex {
			get { return BottomRight.Column; }
			set {
				if (RightColumnIndex == value)
					return;
				BottomRight = new CellPosition(value, BottomRowIndex, BottomRight.ColumnType, BottomRight.RowType);
				Normalize();
			}
		}
		public int TopRowIndex {
			get { return TopLeft.Row; }
			set {
				if (TopRowIndex == value)
					return;
				TopLeft = new CellPosition(LeftColumnIndex, value, TopLeft.ColumnType, TopLeft.RowType);
				Normalize();
			}
		}
		public int BottomRowIndex {
			get { return BottomRight.Row; }
			set {
				if (BottomRowIndex == value)
					return;
				this.BottomRight = new CellPosition(RightColumnIndex, value, BottomRight.ColumnType, BottomRight.RowType);
				Normalize();
			}
		}
		#endregion
		protected internal CellRange Intersection(CellRange what) {
			VariantValue intersectionValue = IntersectionWith(what);
			return intersectionValue.IsError ? null : intersectionValue.CellRangeValue.GetFirstInnerCellRange();
		}
		public override bool Exists(Predicate<CellRange> match) {
			return match(this);
		}
		public override void ForEach(Action<CellRange> action) {
			action(this);
		}
		public override bool ForAll(Predicate<CellRange> p) {
			return p(this);
		}
		public bool ContainsLockedCells() {
			int count = 0;
			IEnumerable<ICellBase> rangeCells = new Enumerable<ICellBase>(GetCellsForReadingEnumerator());
			foreach (ICellBase cellBase in rangeCells) {
				Cell cell = cellBase as Cell;
				if (cell != null && cell.ActualProtection.Locked)
					return true;
				count++;
			}
			return count != CellCount;
		}
	}
	#endregion
	#region CellIntervalRange
	#region CellIntervalRangeType
	public enum CellIntervalRangeType {
		ColumnInterval,
		RowInterval,
	}
	#endregion
	public class CellIntervalRange : CellRange {
		#region Fields
		readonly CellIntervalRangeType intervalRangeType;
		#endregion
		CellIntervalRange(ICellTable workSheet, CellPosition topLeft, CellPosition bottomRight, CellIntervalRangeType rangeType)
			: base(workSheet, topLeft, bottomRight) {
			this.intervalRangeType = rangeType;
		}
		#region Properties
		public override CellRangeType RangeType { get { return CellRangeType.IntervalRange; } }
		public bool IsRowInterval { get { return intervalRangeType == CellIntervalRangeType.RowInterval; } }
		public bool IsColumnInterval { get { return intervalRangeType == CellIntervalRangeType.ColumnInterval; } }
		#endregion
		public static CellIntervalRange CreateRowInterval(ICellTable worksheet, int firstRow, PositionType firstRowType, int lastRow, PositionType lastRowType) {
			CellPosition topLeft = new CellPosition(0, firstRow, PositionType.Absolute, firstRowType);
			CellPosition bottomRight = new CellPosition(IndicesChecker.MaxColumnCount - 1, lastRow, PositionType.Absolute, lastRowType);
			return new CellIntervalRange(worksheet, topLeft, bottomRight, CellIntervalRangeType.RowInterval);
		}
		public static CellIntervalRange CreateColumnInterval(ICellTable workSheet, int firstColumn, PositionType firstColumnType, int lastColumn, PositionType lastColumnType) {
			CellPosition topLeft = new CellPosition(firstColumn, 0, firstColumnType, PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(lastColumn, IndicesChecker.MaxRowCount - 1, lastColumnType, PositionType.Absolute);
			return new CellIntervalRange(workSheet, topLeft, bottomRight, CellIntervalRangeType.ColumnInterval);
		}
		public static CellRange CreateAllWorksheetRange(ICellTable worksheet) {
			int lastcolumn = worksheet.MaxColumnCount - 1;
			return CreateColumnInterval(worksheet, 0, PositionType.Absolute, lastcolumn, PositionType.Absolute); 
		}
		public override CellRange GetRelativeToCurrent(int currentColumnIndex, int currentRowIndex) {
			CellPosition relatedTopLeft = TopLeft.GetRelativeToCurrent(currentColumnIndex, currentRowIndex);
			CellPosition relatedBottomRight = BottomRight.GetRelativeToCurrent(currentColumnIndex, currentRowIndex);
			if (this.intervalRangeType == CellIntervalRangeType.ColumnInterval)
				return CellIntervalRange.CreateColumnInterval(Worksheet, relatedTopLeft.Column, relatedTopLeft.ColumnType, relatedBottomRight.Column, relatedBottomRight.ColumnType);
			return CellIntervalRange.CreateRowInterval(Worksheet, relatedTopLeft.Row, relatedTopLeft.RowType, relatedBottomRight.Row, relatedBottomRight.RowType);
		}
		public override string ToString() {
			return ToString(null);
		}
		public override string ToString(bool includeSheetName) {
			return ToString(null, includeSheetName);
		}
		public override string ToString(WorkbookDataContext context, bool includeSheetName) {
			StringBuilder stringBuilder = new StringBuilder();
			if (includeSheetName)
				stringBuilder.Append(GetSheetName());
			CellPosition tempCellPosStart;
			CellPosition tempCellPosEnd;
			bool identicalPositions = true;
			if (intervalRangeType == CellIntervalRangeType.RowInterval) {
				tempCellPosStart = new CellPosition(-1, TopLeft.Row, PositionType.Absolute, TopLeft.RowType);
				tempCellPosEnd = new CellPosition(-1, BottomRight.Row, PositionType.Absolute, BottomRight.RowType);
				identicalPositions &= TopLeft.Row == BottomRight.Row && TopLeft.RowType == BottomRight.RowType;
			}
			else {
				tempCellPosStart = new CellPosition(TopLeft.Column, -1, TopLeft.ColumnType, PositionType.Absolute);
				tempCellPosEnd = new CellPosition(BottomRight.Column, -1, BottomRight.ColumnType, PositionType.Absolute);
				identicalPositions &= TopLeft.Column == BottomRight.Column && TopLeft.ColumnType == BottomRight.ColumnType;
			}
			identicalPositions &= context != null && context.UseR1C1ReferenceStyle;
			stringBuilder.Append(tempCellPosStart.ToString(context));
			if (!identicalPositions) {
				stringBuilder.Append(":");
				stringBuilder.Append(tempCellPosEnd.ToString(context));
			}
			return stringBuilder.ToString();
		}
		#region ICloneable<CellIntervalRange> Members
		public override CellRange Clone() {
			return new CellIntervalRange(Worksheet, TopLeft, BottomRight, intervalRangeType);
		}
		public override CellRangeBase Clone(ICellTable sheet) {
			return new CellIntervalRange(sheet, TopLeft, BottomRight, intervalRangeType);
		}
		#endregion
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			CellIntervalRange other = obj as CellIntervalRange;
			if (other == null)
				return false;
			return intervalRangeType.Equals(other.intervalRangeType);
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ (int)intervalRangeType;
		}
		public override CellRange Expand(CellRange otherRange) {
			CellPosition topLeft = new CellPosition(Math.Min(TopLeft.Column, otherRange.TopLeft.Column), Math.Min(TopLeft.Row, otherRange.TopLeft.Row));
			if (!topLeft.IsValid)
				return null;
			CellPosition bottomRight = new CellPosition(Math.Max(BottomRight.Column, otherRange.BottomRight.Column), Math.Max(BottomRight.Row, otherRange.BottomRight.Row));
			if (!bottomRight.IsValid)
				return null;
			if (IsRowInterval)
				return CreateRowInterval(Worksheet, topLeft.Row, this.TopLeft.RowType, bottomRight.Row, this.BottomRight.RowType);
			else
				return CreateColumnInterval(Worksheet, topLeft.Column, this.TopLeft.ColumnType, bottomRight.Column, this.BottomRight.ColumnType);
		}
		internal override bool IsColumnRangeInterval() {
			return this.IsColumnInterval;
		}
		internal override bool IsRowRangeInterval() {
			return this.IsRowInterval;
		}
		public override CellRangeBase GetWithoutIntervals() {
			return new CellRange(Worksheet, TopLeft, BottomRight);
		}
		public override CellRangeBase GetWithModifiedPositionType(PositionType columnPositionType, PositionType rowPositionType) {
			CellPosition first = new CellPosition(TopLeft.Column, TopLeft.Row, columnPositionType, rowPositionType);
			CellPosition last = new CellPosition(BottomRight.Column, BottomRight.Row, columnPositionType, rowPositionType);
			return new CellIntervalRange(Worksheet, first, last, intervalRangeType);
		}
	}
	#endregion
	#region CellUnion
	public class CellUnion : CellRangeBase, ICloneable<CellUnion> {
		#region inner classes
		class RightSortComparer : IComparer<CellRangeBase> {
			#region IComparer<CellRangeBase> Members
			public int Compare(CellRangeBase x, CellRangeBase y) {
				CellRange xRange = x as CellRange;
				CellRange yRange = y as CellRange;
				if (xRange.BottomRight.Column < yRange.BottomRight.Column)
					return 1;
				if (xRange.BottomRight.Column > yRange.BottomRight.Column)
					return -1;
				return 0;
			}
			#endregion
		}
		class BottomSortComparer : IComparer<CellRangeBase> {
			#region IComparer<CellRangeBase> Members
			public int Compare(CellRangeBase x, CellRangeBase y) {
				CellRange xRange = x as CellRange;
				CellRange yRange = y as CellRange;
				if (xRange.BottomRight.Row < yRange.BottomRight.Row)
					return 1;
				if (xRange.BottomRight.Row > yRange.BottomRight.Row)
					return -1;
				return 0;
			}
			#endregion
		}
		#endregion
		#region Fields
		List<CellRangeBase> innerCellRanges;
		int summaryInnerRangesSquare;
		int topInnerRanges;
		int leftInnerRanges;
		int bottomInnerRanges;
		int rightInnerRanges;
		ICellTable worksheet;
		#endregion
		public CellUnion(List<CellRangeBase> cellRanges)
			: this(null, cellRanges) {
			if (innerCellRanges.Count > 0 && CheckSheetSameness())
				worksheet = innerCellRanges[0].Worksheet;
		}
		public CellUnion(ICellTable worksheet, List<CellRangeBase> cellRanges) {
			Guard.ArgumentNotNull(cellRanges, "cellRanges");
			Guard.Equals(true, cellRanges.Count > 0);
			this.worksheet = worksheet;
			innerCellRanges = cellRanges;
		}
		#region Properies
		public override long CellCount {
			get {
				long result = 0;
				int count = innerCellRanges.Count;
				for (int i = 0; i < count; i++)
					result += innerCellRanges[i].CellCount;
				return result;
			}
		}
		public override int Width { get { throw new ArgumentException(); } }
		public override int Height { get { throw new ArgumentException(); } }
		public override CellRangeType RangeType { get { return CellRangeType.UnionRange; } }
		public List<CellRangeBase> InnerCellRanges { get { return innerCellRanges; } }
		public override ICellTable Worksheet { get { return worksheet; } set { SetWorksheet(value); } }
		public override CellPosition TopLeft { get { return GetTopLeft(); } set { throw new InvalidOperationException(); } }
		public override CellPosition BottomRight { get { return GetBottomRight(); } set { throw new InvalidOperationException(); } }
		public override int AreasCount { get { return innerCellRanges.Count; } }
		#endregion
		public override IEnumerator<ICellBase> GetAllCellsEnumerator() {
			return new CellUnionEnumerator(innerCellRanges);
		}
		public static CellUnion Combine(CellRange first, CellRangeBase second) {
			List<CellRangeBase> pair = new List<CellRangeBase>();
			pair.Add(first);
			pair.Add(second);
			CellUnion union = new CellUnion(pair);
			return union;
		}
		public override int GetMaxWidth() {
			int result = 0;
			foreach (CellRangeBase innerRange in InnerCellRanges)
				result = Math.Max(result, innerRange.GetMaxWidth());
			return result;
		}
		public override int GetMaxHeight() {
			int result = 0;
			foreach (CellRangeBase innerRange in InnerCellRanges)
				result = Math.Max(result, innerRange.GetMaxHeight());
			return result;
		}
		void SetWorksheet(ICellTable value) {
			worksheet = value;
			foreach (CellRangeBase currentRange in innerCellRanges)
				currentRange.Worksheet = value;
		}
		#region Enumerators
		public override IEnumerator<ICellBase> GetExistingCellsEnumerator(bool reversed) {
			return new CellUnionExistingCellsEnumerator(innerCellRanges, reversed);
		}
		public override IEnumerator<ICellBase> GetExistingCellsEnumeratorByColumns() {
			return new CellUnionExistingCellsEnumeratorByColumns(innerCellRanges);
		}
		public override IEnumerable<ICellBase> GetExistingCellsEnumerable() {
			return new Enumerable<ICellBase>(GetExistingCellsEnumerator(false));
		}
		public override IEnumerable<ICellBase> GetExistingCellsEnumerable(bool reversed) {
			return new Enumerable<ICellBase>(GetExistingCellsEnumerator(reversed));
		}
		public override IEnumerator<CellKey> GetAllCellKeysEnumerator() {
			return new CellUnionCellKeysEnumerator(innerCellRanges);
		}
		public override IEnumerator<VariantValue> GetValuesEnumerator() {
			return new CellUnionValuesEnumerator(innerCellRanges);
		}
		public override IEnumerable<CellRange> GetAreasEnumerable() {
			return new Enumerable<CellRange>(new CellUnionAreasEnumerator(innerCellRanges));
		}
		#endregion
		#region ContainsCell
		public override bool ContainsCell(ICellBase cell) {
			for (int i = 0; i < innerCellRanges.Count; i++)
				if (innerCellRanges[i].ContainsCell(cell))
					return true;
			return false;
		}
		public override bool ContainsCell(CellKey cellKey) {
			for (int i = 0; i < innerCellRanges.Count; i++)
				if (innerCellRanges[i].ContainsCell(cellKey))
					return true;
			return false;
		}
		public override bool ContainsCell(int columnIndex, int rowIndex) {
			for (int i = 0; i < innerCellRanges.Count; i++)
				if (innerCellRanges[i].ContainsCell(columnIndex, rowIndex))
					return true;
			return false;
		}
		#endregion
		public override bool IsAbsolute() {
			for (int i = 0; i < innerCellRanges.Count; i++)
				if (!innerCellRanges[i].IsAbsolute())
					return false;
			return true;
		}
		public override bool IsRelative() {
			for (int i = 0; i < innerCellRanges.Count; i++)
				if (!innerCellRanges[i].IsRelative())
					return false;
			return true;
		}
		public override bool HasData() {
			for (int i = 0; i < innerCellRanges.Count; i++)
				if (innerCellRanges[i].HasData())
					return true;
			return false;
		}
		#region ToString
		public override string ToString() {
			if (Worksheet == null)
				return ToString(',', false);
			else
				return ToString(Worksheet.Workbook.DataContext);
		}
		public override string ToString(bool includeSheetName) {
			return ToString(',', includeSheetName);
		}
		public override string ToString(WorkbookDataContext context, bool includeSheetName) {
			char separator = context.GetListSeparator();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < innerCellRanges.Count; i++) {
				stringBuilder.Append(innerCellRanges[i].ToString(context, includeSheetName));
				stringBuilder.Append(separator);
			}
			if (stringBuilder.Length > 0)
				stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}
		public string ToString(char separator, bool includeSheetName) {
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < innerCellRanges.Count; i++) {
				stringBuilder.Append(innerCellRanges[i].ToString(includeSheetName));
				stringBuilder.Append(separator);
			}
			if (stringBuilder.Length > 0)
				stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}
		#endregion
		public override VariantValue UnionWith(CellRangeBase cellrange) {
			VariantValue tempValue = new VariantValue();
			tempValue.CellRangeValue = cellrange;
			for (int i = 0; i < innerCellRanges.Count; i++) {
				tempValue = tempValue.CellRangeValue.UnionWith(innerCellRanges[i]);
				if (tempValue.IsError)
					break;
			}
			return tempValue;
		}
		public override bool Includes(CellRangeBase rangeBase) {
			foreach (CellRangeBase currentRange in innerCellRanges)
				if (currentRange.Includes(rangeBase))
					return true;
			return false;
		}
		public override bool Intersects(CellRangeBase rangeBase) {
			foreach (CellRangeBase currentRange in innerCellRanges)
				if (currentRange.Intersects(rangeBase))
					return true;
			return false;
		}
		public override VariantValue IntersectionWith(CellRangeBase cellrange) {
			int innerListCount = innerCellRanges.Count;
			CellRangeList resultList = new CellRangeList() { Capacity = innerListCount };
			VariantValue tempValue;
			for (int i = 0; i < innerListCount; i++) {
				tempValue = innerCellRanges[i].IntersectionWith(cellrange);
				if (!tempValue.IsError)
					resultList.Add(tempValue.CellRangeValue);
			}
			int count = resultList.Count;
			if (count <= 0)
				return VariantValue.ErrorNullIntersection;
			VariantValue result = new VariantValue();
			if (count == 1)
				result.CellRangeValue = resultList[0];
			else
				result.CellRangeValue = new CellUnion(resultList);
			return result;
		}
		public override VariantValue ConcatinateWith(CellRangeBase cellrange) {
			List<CellRangeBase> resultList = innerCellRanges;
			switch (cellrange.RangeType) {
				case CellRangeType.IntervalRange:
				case CellRangeType.SingleRange:
					resultList.Add(cellrange);
					break;
				case CellRangeType.UnionRange:
					resultList.AddRange((cellrange as CellUnion).InnerCellRanges);
					break;
				default:
					throw new ArgumentException("Unknown CellRangeType " + cellrange.RangeType);
			}
			CellUnion resultUnion = new CellUnion(resultList);
			if (!resultUnion.CheckSheetSameness())
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue resultValue = new VariantValue();
			resultValue.CellRangeValue = resultUnion;
			return resultValue;
		}
		public override CellRange GetFirstInnerCellRange() {
			return this.innerCellRanges[0].GetFirstInnerCellRange();
		}
		public override ICellBase GetFirstCellUnsafe() {
			return GetFirstInnerCellRange().GetFirstCellUnsafe();
		}
		public override VariantValue GetFirstCellValue() {
			return GetFirstInnerCellRange().GetFirstCellValue();
		}
		public override VariantValue GetCellValueByZOrder(long cellIndex) {
			int i = 0;
			while (i < innerCellRanges.Count && cellIndex >= innerCellRanges[i].CellCount) {
				cellIndex -= innerCellRanges[i].CellCount;
				i++;
			}
			if (i < innerCellRanges.Count)
				return innerCellRanges[i].GetCellValueByZOrder(cellIndex);
			else
				throw new IndexOutOfRangeException();
		}
		public override List<Point> GetCornerPoints() {
			List<Point> result = new List<Point>();
			for (int i = 0; i < InnerCellRanges.Count; i++) {
				List<Point> innerCellRangePoints = InnerCellRanges[i].GetCornerPoints();
				foreach (Point point in innerCellRangePoints)
					if (!result.Contains(point))
						result.Add(point);
					else
						result.Remove(point);
			}
			return result;
		}
		public override CellRange GetAreaByIndex(int index) {
			if (index < 0)
				throw new ArgumentOutOfRangeException();
			for (int i = 0; i < innerCellRanges.Count; i++) {
				if (index < innerCellRanges[i].AreasCount) {
					return innerCellRanges[i].GetAreaByIndex(index);
				}
				index -= innerCellRanges[i].AreasCount;
			}
			throw new IndexOutOfRangeException();
		}
		public bool CheckSheetSameness() {
			ICellTable modelSheet = innerCellRanges[0].Worksheet;
			for (int i = 1; i < innerCellRanges.Count; i++)
				if (!object.ReferenceEquals(modelSheet, innerCellRanges[i].Worksheet))
					return false;
			return true;
		}
		public override CellRange GetCoveredRange() {
			return new CellRange(Worksheet, TopLeft, BottomRight);
		}
		#region ICloneable<CellUnion> Members
		public new CellUnion Clone() {
			int innerListCount = innerCellRanges.Count;
			CellRangeList resultList = new CellRangeList() { Capacity = innerListCount };
			for (int i = 0; i < innerListCount; i++)
				resultList.Add(innerCellRanges[i].Clone());
			return new CellUnion(worksheet, resultList);
		}
		protected override CellRangeBase CloneCore() {
			return Clone();
		}
		public override CellRangeBase Clone(ICellTable sheet) {
			int innerListCount = innerCellRanges.Count;
			CellRangeList resultList = new CellRangeList() { Capacity = innerListCount };
			for (int i = 0; i < innerListCount; i++)
				resultList.Add(innerCellRanges[i].Clone(sheet));
			return new CellUnion(sheet, resultList);
		}
		#endregion
		public override bool IsMerged {
			get {
				foreach (CellRangeBase currentRange in innerCellRanges)
					if (!currentRange.IsMerged)
						return false;
				return true;
			}
		}
		public bool HasNonConsistantRanges() {
			if (innerCellRanges.Count < 2)
				return false;
			CellRangeType cellRangeType = innerCellRanges[0].RangeType;
			foreach (CellRangeBase cellRange in innerCellRanges) {
				if (cellRange.RangeType != cellRangeType)
					return true;
			}
			return false;
		}
		public bool HasIntersectedRanges() {
			int count = InnerCellRanges.Count;
			for (int i = 0; i < count; i++)
				for (int j = i + 1; j < count; j++)
					if (innerCellRanges[i].Intersects(innerCellRanges[j]))
						return true;
			return false;
		}
		CellPosition GetTopLeft() {
			int minColumn = this.innerCellRanges[0].TopLeft.Column;
			int minRow = this.innerCellRanges[0].TopLeft.Row;
			for (int i = 1; i < innerCellRanges.Count; i++) {
				CellPosition processingPosition = innerCellRanges[i].TopLeft;
				if (processingPosition.Row < minRow)
					minRow = processingPosition.Row;
				if (processingPosition.Column < minColumn)
					minColumn = processingPosition.Column;
			}
			return new CellPosition(minColumn, minRow);
		}
		CellPosition GetBottomRight() {
			int maxColumn = this.innerCellRanges[0].BottomRight.Column;
			int maxRow = this.innerCellRanges[0].BottomRight.Row;
			for (int i = 1; i < innerCellRanges.Count; i++) {
				CellPosition processingPosition = innerCellRanges[i].BottomRight;
				if (processingPosition.Row > maxRow)
					maxRow = processingPosition.Row;
				if (processingPosition.Column > maxColumn)
					maxColumn = processingPosition.Column;
			}
			return new CellPosition(maxColumn, maxRow);
		}
		public override void EnlargeByMergedRanges() {
			for (int i = 0; i < InnerCellRanges.Count; i++)
				InnerCellRanges[i].EnlargeByMergedRanges();
		}
		internal override void AddRanges(List<CellRange> collection) {
			foreach (CellRangeBase range in innerCellRanges)
				range.AddRanges(collection);
		}
		public CellUnion GetUnionWithSortedInnerRanges(InsertCellMode mode) {
			return CreateSortedRange(GetComparer(mode), true);
		}
		public CellUnion GetUnionWithSortedInnerRanges(RemoveCellMode mode) {
			return CreateSortedRange(GetComparer(mode), false);
		}
		CellUnion CreateSortedRange(IComparer<CellRangeBase> comparer, bool suppressReplaceRanges) {
			CellRangeList resultRanges = GetInnerNonUnionRanges(suppressReplaceRanges);
			if (resultRanges == null)
				return null;
			if (comparer != null && resultRanges.Count > 1)
				resultRanges.Sort(comparer);
			return new CellUnion(resultRanges);
		}
		IComparer<CellRangeBase> GetComparer(RemoveCellMode mode) {
			if (mode == RemoveCellMode.ShiftCellsUp)
				return new BottomSortComparer();
			else if (mode == RemoveCellMode.ShiftCellsLeft)
				return new RightSortComparer();
			return null;
		}
		IComparer<CellRangeBase> GetComparer(InsertCellMode mode) {
			if (mode == InsertCellMode.ShiftCellsDown)
				return new BottomSortComparer();
			return new RightSortComparer();
		}
		CellRangeList GetInnerNonUnionRanges(bool suppressReplaceRanges) {
			CellRangeList resultRanges = new CellRangeList();
			summaryInnerRangesSquare = 0;
			topInnerRanges = worksheet != null ? worksheet.MaxRowCount : IndicesChecker.MaxRowCount;
			leftInnerRanges = worksheet != null ? worksheet.MaxColumnCount : IndicesChecker.MaxColumnCount;
			bottomInnerRanges = -1;
			rightInnerRanges = -1;
			foreach (CellRangeBase currentRange in innerCellRanges) {
				if (currentRange.RangeType == CellRangeType.UnionRange) {
					CellRangeList nonUnionRanges = (currentRange as CellUnion).GetInnerNonUnionRanges(suppressReplaceRanges);
					if (nonUnionRanges == null)
						return null;
					resultRanges.AddRange(nonUnionRanges);
				}
				else {
					if (currentRange.RangeType == CellRangeType.IntervalRange)
						suppressReplaceRanges = true;
					if (HasCommonCells(currentRange, resultRanges))
						return null;
					CellRange cellRange = currentRange as CellRange;
					resultRanges.Add(cellRange);
					if (!suppressReplaceRanges) {
						summaryInnerRangesSquare += cellRange.Width * cellRange.Height;
						topInnerRanges = Math.Min(topInnerRanges, cellRange.TopLeft.Row);
						leftInnerRanges = Math.Min(leftInnerRanges, cellRange.TopLeft.Column);
						bottomInnerRanges = Math.Max(bottomInnerRanges, cellRange.BottomRight.Row);
						rightInnerRanges = Math.Max(rightInnerRanges, cellRange.BottomRight.Column);
					}
				}
			}
			if (summaryInnerRangesSquare == ((bottomInnerRanges - topInnerRanges + 1) * (rightInnerRanges - leftInnerRanges + 1)) && !suppressReplaceRanges) {
				CellPosition topLeft = new CellPosition(leftInnerRanges, topInnerRanges, (resultRanges[0] as CellRange).TopLeft.ColumnType, (resultRanges[0] as CellRange).TopLeft.RowType);
				CellPosition bottomRight = new CellPosition(rightInnerRanges, bottomInnerRanges, (resultRanges[0] as CellRange).BottomRight.ColumnType, (resultRanges[0] as CellRange).BottomRight.RowType);
				CellRange unionRange = new CellRange(resultRanges[0].Worksheet, topLeft, bottomRight);
				resultRanges.Clear();
				resultRanges.Add(unionRange);
			}
			return resultRanges;
		}
		bool HasCommonCells(CellRangeBase range, CellRangeList ranges) {
			for (int i = 0; i < ranges.Count; i++)
				if (ranges[i].Intersects(range))
					return true;
			return false;
		}
		#region GetShifted
		public override CellRangeBase GetShifted(Worksheet worksheet, CellPositionOffset offset) {
			int innerListCount = innerCellRanges.Count;
			CellRangeList resultList = new CellRangeList() { Capacity = innerListCount };
			for (int i = 0; i < innerListCount; i++) {
				CellRangeBase sourceInnerRange = InnerCellRanges[i];
				resultList.Add(sourceInnerRange.GetShifted(worksheet, offset));
			}
			return new CellUnion(worksheet, resultList);
		}
		public override CellRangeBase GetShiftedAny(CellPositionOffset offset, ICellTable newWorksheet) {
			int count = InnerCellRanges.Count;
			CellRangeList resultList = new CellRangeList() { Capacity = count };
			for (int i = 0; i < count; i++) {
				CellRangeBase sourceInnerRange = InnerCellRanges[i];
				CellRangeBase sourceInnerRangeShifted = sourceInnerRange.GetShiftedAny(offset, newWorksheet);
				if (sourceInnerRangeShifted == null)
					return null;
				resultList.Add(sourceInnerRangeShifted);
			}
			return new CellUnion(newWorksheet, resultList);
		}
		#endregion
		internal override bool IsColumnRangeInterval() {
			foreach (CellRangeBase range in innerCellRanges)
				if (!range.IsColumnRangeInterval())
					return false;
			return true;
		}
		internal override bool IsRowRangeInterval() {
			foreach (CellRangeBase range in innerCellRanges)
				if (!range.IsRowRangeInterval())
					return false;
			return true;
		}
		public override CellRangeBase GetForceShifted(CellRange shiftArea, bool horizontal, bool positive) {
			List<CellRangeBase> newInnerRanges = new List<CellRangeBase>();
			foreach (CellRange innerRange in GetAreasEnumerable()) {
				CellRangeBase shifted = innerRange.GetForceShifted(shiftArea, horizontal, positive);
				newInnerRanges.AddRange(shifted.GetAreasEnumerable());
			}
			return new CellUnion(newInnerRanges);
		}
		public override CellRangeBase GetWithModifiedPositionType(PositionType columnPositionType, PositionType rowPositionType) {
			List<CellRangeBase> list = new List<CellRangeBase>();
			foreach (CellRangeBase range in innerCellRanges)
				list.Add(range.GetWithModifiedPositionType(columnPositionType, rowPositionType));
			CellUnion result = new CellUnion(list);
			result.worksheet = worksheet;
			return result;
		}
		public override CellRangeBase GetWithoutIntervals() {
			List<CellRangeBase> list = new List<CellRangeBase>();
			foreach (CellRangeBase range in innerCellRanges)
				list.Add(range.GetWithoutIntervals());
			CellUnion result = new CellUnion(list);
			result.worksheet = worksheet;
			return result;
		}
		#region MergeWithRange
		public override CellRangeBase TryMergeWithRange(CellRangeBase anotherRange) {
			if (anotherRange.RangeType != CellRangeType.UnionRange)
				return TryMergeWithSingleRange(anotherRange);
			else
				return TryMergeWithUnion(anotherRange);
		}
		public override CellRangeBase MergeWithRange(CellRangeBase anotherRange) {
			CellRangeBase merged = TryMergeWithRange(anotherRange);
			if (merged != null)
				return merged;
			if (anotherRange.RangeType != CellRangeType.UnionRange)
				innerCellRanges.Add(anotherRange);
			else {
				CellUnion union = (CellUnion)anotherRange;
				innerCellRanges.AddRange(union.InnerCellRanges);
			}
			return this;
		}
		CellRangeBase TryMergeInnerRanges() {
			int innerCount = innerCellRanges.Count;
			if (innerCellRanges.Count == 1)
				return innerCellRanges[0];
			bool merged = true;
			while (merged) {
				merged = false;
				int i = 0;
				int j = 1;
				innerCount = innerCellRanges.Count;
				while (i < innerCount - 1) {
					CellRangeBase mergedRange = innerCellRanges[i].TryMergeWithRange(innerCellRanges[j]);
					if (mergedRange != null) {
						merged = true;
						innerCellRanges[j] = mergedRange;
						innerCellRanges.RemoveAt(i);
						break;
					}
					j++;
					if (j >= innerCellRanges.Count) {
						i++;
						j = i + 1;
					}
				}
			}
			if (innerCellRanges.Count == 1)
				return innerCellRanges[0];
			return this;
		}
		CellRangeBase TryMergeWithSingleRange(CellRangeBase anotherRange) {
			int i = 0;
			while (i < innerCellRanges.Count) {
				CellRangeBase innerRange = innerCellRanges[i];
				CellRangeBase merged = innerRange.TryMergeWithRange(anotherRange);
				if (merged != null) {
					innerCellRanges[i] = merged;
					return TryMergeInnerRanges();
				}
				i++;
			}
			return null;
		}
		CellRangeBase TryMergeWithUnion(CellRangeBase anotherRange) {
			bool merged = true;
			bool smthMerged = false;
			CellUnion anotherUnionRange = (CellUnion)anotherRange;
			while (merged) {
				merged = false;
				int innerCount = innerCellRanges.Count;
				int anotherCount = anotherUnionRange.InnerCellRanges.Count;
				for (int i = 0; i < innerCount * anotherCount; i++) {
					int anotherIndex = i % anotherCount;
					int innerIndex = i / anotherCount;
					CellRangeBase mergedRange = innerCellRanges[innerIndex].TryMergeWithRange(anotherUnionRange.InnerCellRanges[anotherIndex]);
					if (mergedRange != null) {
						innerCellRanges[innerIndex] = mergedRange;
						anotherUnionRange.InnerCellRanges.RemoveAt(anotherIndex);
						merged = true;
						smthMerged = true;
						break;
					}
				}
			}
			if (smthMerged) {
				foreach (CellRangeBase innerAnotherRange in anotherUnionRange.InnerCellRanges)
					this.innerCellRanges.Add(innerAnotherRange);
				return TryMergeInnerRanges();
			}
			return null;
		}
		#endregion
		public override bool Exists(Predicate<CellRange> match) {
			foreach (CellRangeBase rangeBase in InnerCellRanges) {
				if (rangeBase.Exists(match))
					return true;
			}
			return false;
		}
		public override bool ForAll(Predicate<CellRange> p) {
			foreach (CellRangeBase rangeBase in InnerCellRanges) {
				if (!rangeBase.ForAll(p))
					return false;
			}
			return true;
		}
		public override void ForEach(Action<CellRange> action) {
			foreach (CellRangeBase rangeBase in InnerCellRanges) {
				rangeBase.ForEach(action);
			}
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			CellUnion other = obj as CellUnion;
			if (other == null)
				return false;
			int count = innerCellRanges.Count;
			if (other.innerCellRanges.Count != count)
				return false;
			for (int i = 0; i < count; i++)
				if (!innerCellRanges[i].Equals(other.innerCellRanges[i]))
					return false;
			return true;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(InnerCellRanges);
		}
	}
	#endregion
	#region CellRangeFactory
	public class CellRangeFactory {
		static readonly CellRangeFactory defaultInstance = new CellRangeFactory();
		static readonly CellRangeFactory cellIntervalRange = new CellIntervalRangeFactory();
		public static CellRangeFactory Default { get { return defaultInstance; } }
		public static CellRangeFactory CellIntervalRange { get { return cellIntervalRange; } }
		public virtual CellRange Create(ICellTable workSheet, string reference) {
			string[] cellPositions = reference.Split(':');
			if (cellPositions.Length == 1) {
				return Create(workSheet, CellReferenceParser.Parse(cellPositions[0]), CellReferenceParser.Parse(cellPositions[0]));
			}
			if (cellPositions.Length != 2)
				Exceptions.ThrowArgumentException("reference", reference);
			return Create(workSheet, CellReferenceParser.Parse(cellPositions[0]), CellReferenceParser.Parse(cellPositions[1]));
		}
		public virtual CellRange TryCreate(ICellTable workSheet, string reference) {
			string[] cellPositions = reference.Split(':');
			if (cellPositions.Length == 1) {
				CellPosition position = CellReferenceParser.TryParse(cellPositions[0]);
				if (!position.IsValid)
					return null;
				return Create(workSheet, position, position);
			}
			if (cellPositions.Length != 2)
				return null;
			CellPosition position1 = CellReferenceParser.TryParse(cellPositions[0]);
			if (!position1.IsValid)
				return null;
			CellPosition position2 = CellReferenceParser.TryParse(cellPositions[1]);
			if (!position2.IsValid)
				return null;
			return Create(workSheet, position1, position2);
		}
		public CellRange Create(ICellTable workSheet, CellPosition topLeft, CellPosition bottomRight) {
			return CreateCore(workSheet, topLeft, bottomRight);
		}
		protected virtual CellRange CreateCore(ICellTable workSheet, CellPosition topLeft, CellPosition bottomRight) {
			return new CellRange(workSheet, topLeft, bottomRight);
		}
	}
	#endregion
	#region CellIntervalRangeFactory
	public class CellIntervalRangeFactory : CellRangeFactory {
		public override CellRange Create(ICellTable workSheet, string reference) {
			CellRange result = CellReferenceParser.ParseIntervalRange(workSheet, reference);
			if (result == null)
				throw new ArgumentException();
			return result;
		}
		public override CellRange TryCreate(ICellTable workSheet, string reference) {
			return CellReferenceParser.ParseIntervalRange(workSheet, reference);
		}
	}
	#endregion
	#region CellRangeEnumeratorBase
	public abstract class CellRangeEnumeratorBase<T> : IEnumerator<T> {
		int relativeColumn;
		int relativeRow;
		CellPosition topLeft;
		ICellTable worksheet;
		readonly int width;
		readonly int height;
		protected CellRangeEnumeratorBase(ICellTable worksheet, CellPosition topLeft, CellPosition bottomRight) {
			this.topLeft = topLeft;
			this.worksheet = worksheet;
			relativeColumn = -1;
			relativeRow = 0;
			width = bottomRight.Column - topLeft.Column + 1;
			height = bottomRight.Row - topLeft.Row + 1;
		}
		#region Properties
		public int CurrentColumnIndex { get { return this.topLeft.Column + relativeColumn; } }
		public int CurrentRowIndex { get { return this.topLeft.Row + relativeRow; } }
		public int RelativeColumn { get { return relativeColumn; } set { relativeColumn = value; } }
		public int RelativeRow { get { return relativeRow; } set { relativeRow = value; } }
		public CellPosition TopLeft { get { return topLeft; } }
		public ICellTable Worksheet { get { return worksheet; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		#endregion
		#region IEnumerator<T> Members
		public abstract T Current { get; }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}
		public virtual bool MoveNext() {
			if (++relativeColumn >= width) {
				if (++relativeRow >= height)
					return false;
				relativeColumn = 0;
			}
			return true;
		}
		public virtual void Reset() {
			relativeColumn = -1;
			relativeRow = 0;
		}
		#endregion
		public CellPositionOffset GetPositionOffset() {
			return new CellPositionOffset(CurrentColumnIndex - TopLeft.Column, CurrentRowIndex - TopLeft.Row);
		}
	}
	#endregion
	#region CellRangeEnumerator
	public class CellRangeEnumerator : CellRangeEnumeratorBase<ICellBase> {
		public CellRangeEnumerator(ICellTable worksheet, CellPosition topLeft, CellPosition bottomRight)
			: base(worksheet, topLeft, bottomRight) {
		}
		#region IEnumerator<ICellBase> Members
		public override ICellBase Current {
			get { return Worksheet.GetCell(CurrentColumnIndex, CurrentRowIndex); }
		}
		#endregion
	}
	#endregion
	#region CellRangeCellKeysEnumerator
	public class CellRangeCellKeysEnumerator : CellRangeEnumeratorBase<CellKey> {
		public CellRangeCellKeysEnumerator(ICellTable worksheet, CellPosition topLeft, CellPosition bottomRight)
			: base(worksheet, topLeft, bottomRight) {
		}
		public override CellKey Current {
			get {
				return new CellKey(Worksheet.SheetId, CurrentColumnIndex, CurrentRowIndex);
			}
		}
	}
	#endregion
	#region CellRangeValuesEnumerator
	public class CellRangeValuesEnumerator : CellRangeEnumeratorBase<VariantValue> {
		public CellRangeValuesEnumerator(ICellTable worksheet, CellPosition topLeft, CellPosition bottomRight)
			: base(worksheet, topLeft, bottomRight) {
		}
		#region IEnumerator<ICellBase> Members
		public override VariantValue Current {
			get {
				return Worksheet.GetCalculatedCellValue(CurrentColumnIndex, CurrentRowIndex);
			}
		}
		#endregion
	}
	#endregion
	#region CellRangeForReadingEnumerator
	public class CellRangeForReadingEnumerator : CellRangeEnumeratorBase<ICellBase> {
		public CellRangeForReadingEnumerator(Worksheet worksheet, CellPosition topLeft, CellPosition bottomRight)
			: base(worksheet, topLeft, bottomRight) {
		}
		#region IEnumerator<ICellBase> Members
		public override ICellBase Current {
			get {
				ICellBase cell = Worksheet.TryGetCell(CurrentColumnIndex, CurrentRowIndex);
				if (cell != null)
					return cell;
				return new FakeCell(new CellPosition(CurrentColumnIndex, CurrentRowIndex), (Worksheet)Worksheet);
			}
		}
		#endregion
	}
	#endregion
	#region ExternalDataRangeEnumerator
	public class ExternalDataRangeEnumerator : CellRangeEnumeratorBase<ICellBase> {
		ICellBase currentCell;
		public ExternalDataRangeEnumerator(ICellTable worksheet, CellPosition topLeft, CellPosition bottomRight)
			: base(worksheet, topLeft, bottomRight) {
		}
		public override bool MoveNext() {
			currentCell = null;
			do {
				if (++RelativeColumn >= Width) {
					if (++RelativeRow >= Height)
						return false;
					RelativeColumn = 0;
				}
				currentCell = Worksheet.TryGetCell(CurrentColumnIndex, CurrentRowIndex);
			} while (currentCell == null);
			return true;
		}
		public override void Reset() {
			base.Reset();
			currentCell = null;
		}
		#region IEnumerator<ICellBase> Members
		public override ICellBase Current { get { return currentCell; } }
		#endregion
	}
	#endregion
	#region CellPositionEnumerator
	public class CellPositionEnumerator : CellRangeEnumeratorBase<CellPosition> {
		public CellPositionEnumerator(CellPosition topLeft, CellPosition bottomRight)
			: base(null, topLeft, bottomRight) {
		}
		#region IEnumerator<CellPosition> Members
		public override CellPosition Current {
			get { return new CellPosition(CurrentColumnIndex, CurrentRowIndex); }
		}
		#endregion
	}
	#endregion
	#region CellUnionEnumeratorBase
	public abstract class CellUnionEnumeratorBase<T> : IEnumerator<T> {
		int curInnerRange;
		List<CellRangeBase> innerList;
		IEnumerator<T> curEnumerator;
		protected CellUnionEnumeratorBase(List<CellRangeBase> innerList) {
			this.innerList = innerList;
			Reset();
		}
		#region IEnumerator<T> Members
		public T Current { get { return curEnumerator.Current; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
			curEnumerator = null;
			innerList = null;
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}
		public bool MoveNext() {
			while (curEnumerator == null || !curEnumerator.MoveNext()) {
				if (++curInnerRange >= innerList.Count)
					return false;
				curEnumerator = GetEnumeratorForRange(innerList[curInnerRange]);
			}
			return true;
		}
		public void Reset() {
			curInnerRange = -1;
			curEnumerator = null;
		}
		#endregion
		protected internal abstract IEnumerator<T> GetEnumeratorForRange(CellRangeBase range);
	}
	#endregion
	#region CellUnionEnumerator
	public class CellUnionEnumerator : CellUnionEnumeratorBase<ICellBase> {
		public CellUnionEnumerator(List<CellRangeBase> innerList)
			: base(innerList) {
		}
		protected internal override IEnumerator<ICellBase> GetEnumeratorForRange(CellRangeBase range) {
			return range.GetAllCellsEnumerator();
		}
	}
	#endregion
	#region CellUnionAreasEnumerator
	public class CellUnionAreasEnumerator : CellUnionEnumeratorBase<CellRange> {
		public CellUnionAreasEnumerator(List<CellRangeBase> innerList)
			: base(innerList) {
		}
		protected internal override IEnumerator<CellRange> GetEnumeratorForRange(CellRangeBase range) {
			return range.GetAreasEnumerable().GetEnumerator();
		}
	}
	#endregion
	#region CellUnionExistingCellsEnumerator
	public class CellUnionExistingCellsEnumerator : CellUnionEnumeratorBase<ICellBase> {
		readonly bool reversed;
		public CellUnionExistingCellsEnumerator(List<CellRangeBase> innerList)
			: this(innerList, false) {
		}
		public CellUnionExistingCellsEnumerator(List<CellRangeBase> innerList, bool reversed)
			: base(innerList) {
			this.reversed = reversed;
			Reset();
		}
		protected internal override IEnumerator<ICellBase> GetEnumeratorForRange(CellRangeBase range) {
			return range.GetExistingCellsEnumerator(reversed);
		}
	}
	#endregion
	#region CellUnionCellKeysEnumerator
	public class CellUnionCellKeysEnumerator : CellUnionEnumeratorBase<CellKey> {
		public CellUnionCellKeysEnumerator(List<CellRangeBase> innerList)
			: base(innerList) {
			Reset();
		}
		protected internal override IEnumerator<CellKey> GetEnumeratorForRange(CellRangeBase range) {
			return range.GetAllCellKeysEnumerator();
		}
	}
	#endregion
	#region CellUnionValuesEnumerator
	public class CellUnionValuesEnumerator : CellUnionEnumeratorBase<VariantValue> {
		public CellUnionValuesEnumerator(List<CellRangeBase> innerList)
			: base(innerList) {
			Reset();
		}
		protected internal override IEnumerator<VariantValue> GetEnumeratorForRange(CellRangeBase range) {
			return range.GetValuesEnumerator();
		}
	}
	#endregion
	#region CellValuesEnumerator
	public class CellValuesEnumerator : IEnumerator<VariantValue> {
		readonly IEnumerator<ICellBase> enumerator;
		public CellValuesEnumerator(IEnumerator<ICellBase> enumerator) {
			Guard.ArgumentNotNull(enumerator, "enumerator");
			this.enumerator = enumerator;
		}
		VariantValue GetCurrent() {
			ICellBase current = enumerator.Current;
			VariantValue result;
			current.Sheet.Workbook.CalculationChain.TryGetCalculatedValue(current, out result);
			return result;
		}
		#region IEnumerator<VariantValue> Members
		public VariantValue Current {
			get {
				return GetCurrent();
			}
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return GetCurrent(); } }
		public bool MoveNext() {
			return enumerator.MoveNext();
		}
		public void Reset() {
			enumerator.Reset();
		}
		#endregion
	}
	#endregion
	#region CellBaseEnumeratorBase (abstract class)
	public abstract class CellBaseEnumeratorBase : IEnumerator<ICellBase> {
		readonly ICellTable worksheet;
		IEnumerator rowsEnumerator;
		IEnumerator cellsEnumerator;
		readonly CellPosition topLeft;
		protected CellBaseEnumeratorBase(ICellTable worksheet, CellPosition topLeft) {
			this.worksheet = worksheet;
			this.topLeft = topLeft;
			Reset();
		}
		public ICellTable Worksheet { get { return worksheet; } }
		public CellPosition TopLeft { get { return topLeft; } }
		#region IEnumerator<ICellBase> Members
		public ICellBase Current { get { return cellsEnumerator.Current as ICellBase; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		void Dispose(bool disposing) {
			if (disposing) {
				IDisposable disposableRowsEnumerator = rowsEnumerator as IDisposable;
				if (disposableRowsEnumerator != null)
					disposableRowsEnumerator.Dispose();
				IDisposable disposableCellsEnumerator = cellsEnumerator as IDisposable;
				if (disposableCellsEnumerator != null)
					disposableCellsEnumerator.Dispose();
			}
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current { get { return Current; } }
		public bool MoveNext() {
			while (cellsEnumerator == null || !cellsEnumerator.MoveNext()) {
				if (rowsEnumerator == null)
					rowsEnumerator = CreateRowsEnumerator();
				if (!rowsEnumerator.MoveNext())
					return false;
				cellsEnumerator = CreateCellsEnumerator((IRowBase)rowsEnumerator.Current);
			}
			return true;
		}
		public void Reset() {
			cellsEnumerator = null;
			rowsEnumerator = null;
		}
		#endregion
		protected abstract IEnumerator CreateRowsEnumerator();
		protected abstract IEnumerator CreateCellsEnumerator(IRowBase row);
	}
	#endregion
	#region CellRangeExistingCellsEnumerator
	public class CellRangeExistingCellsEnumerator : CellBaseEnumeratorBase {
		readonly CellPosition bottomRight;
		readonly bool reversed;
		public CellRangeExistingCellsEnumerator(ICellTable worksheet, CellPosition topLeft, CellPosition bottomRight)
			: this(worksheet, topLeft, bottomRight, false) {
		}
		public CellRangeExistingCellsEnumerator(ICellTable worksheet, CellPosition topLeft, CellPosition bottomRight, bool reversed)
			: base(worksheet, topLeft) {
			this.bottomRight = bottomRight;
			this.reversed = reversed;
			Reset();
		}
		#region Properties
		public CellPosition BottomRight { get { return bottomRight; } }
		protected bool Reversed { get { return reversed; } }
		#endregion
		protected override IEnumerator CreateRowsEnumerator() {
			return Worksheet.Rows.GetExistingRowsEnumerator(TopLeft.Row, bottomRight.Row, reversed);
		}
		protected override IEnumerator CreateCellsEnumerator(IRowBase row) {
			return row.Cells.GetExistingCellsEnumerator(TopLeft.Column, bottomRight.Column, reversed);
		}
	}
	#endregion
	#region CellRangeExistingCellsEnumeratorFast
	public class CellRangeExistingCellsEnumeratorFast : CellBaseEnumeratorBase {
		readonly CellPosition bottomRight;
		public CellRangeExistingCellsEnumeratorFast(ICellTable worksheet, CellPosition topLeft, CellPosition bottomRight)
			: base(worksheet, topLeft) {
			this.bottomRight = bottomRight;
			Reset();
		}
		#region Properties
		public CellPosition BottomRight { get { return bottomRight; } }
		#endregion
		protected override IEnumerator CreateRowsEnumerator() {
			return Worksheet.Rows.GetExistingRowsEnumerator(TopLeft.Row, bottomRight.Row);
		}
		protected override IEnumerator CreateCellsEnumerator(IRowBase row) {
			return row.Cells.GetExistingCellsEnumerator(TopLeft.Column, bottomRight.Column);
		}
	}
	#endregion
	#region CellRangeExistingCellsEnumeratorByColumns
	public class CellRangeExistingCellsEnumeratorByColumns : IEnumerator<ICellBase> {
		readonly ICellTable worksheet;
		IEnumerator rowsEnumerator;
		readonly CellPosition topLeft;
		readonly CellPosition bottomRight;
		int columnIndex;
		ICellBase current;
		public CellRangeExistingCellsEnumeratorByColumns(ICellTable worksheet, CellPosition topLeft, CellPosition bottomRight) {
			this.worksheet = worksheet;
			this.topLeft = topLeft;
			this.bottomRight = bottomRight;
			Reset();
		}
		public ICellTable Worksheet { get { return worksheet; } }
		public CellPosition TopLeft { get { return topLeft; } }
		public CellPosition BottomRight { get { return bottomRight; } }
		#region IEnumerator<ICellBase> Members
		public ICellBase Current { get { return current; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current { get { return Current; } }
		public bool MoveNext() {
			if (rowsEnumerator == null)
				rowsEnumerator = CreateRowsEnumerator();
			for (; ; ) {
				while (rowsEnumerator.MoveNext()) {
					Row row = (Row)rowsEnumerator.Current;
					ICell cell = row.Cells.TryGetCell(columnIndex);
					if (cell != null) {
						this.current = cell;
						return true;
					}
				}
				columnIndex++;
				if (columnIndex > bottomRight.Column)
					return false;
				rowsEnumerator.Reset();
			}
		}
		public void Reset() {
			rowsEnumerator = null;
			columnIndex = topLeft.Column;
			current = null;
		}
		#endregion
		protected virtual IEnumerator CreateRowsEnumerator() {
			return Worksheet.Rows.GetExistingRows(TopLeft.Row, bottomRight.Row, false).GetEnumerator();
		}
		protected virtual IEnumerator CreateCellsEnumerator(IRowBase row, int column) {
			return row.Cells.GetExistingCells(column, column, false).GetEnumerator();
		}
	}
	#endregion
	#region CellUnionExistingCellsEnumerator
	public class CellUnionExistingCellsEnumeratorByColumns : CellUnionEnumeratorBase<ICellBase> {
		public CellUnionExistingCellsEnumeratorByColumns(List<CellRangeBase> innerList)
			: base(innerList) {
			Reset();
		}
		protected internal override IEnumerator<ICellBase> GetEnumeratorForRange(CellRangeBase range) {
			return range.GetExistingCellsEnumeratorByColumns();
		}
	}
	#endregion
	#region CellRangeNonEmptyVisibleCellsEnumerator
	public class CellRangeNonEmptyVisibleCellsEnumerator : CellRangeExistingCellsEnumerator {
		public CellRangeNonEmptyVisibleCellsEnumerator(ICellTable worksheet, CellPosition topLeft, CellPosition bottomRight)
			: base(worksheet, topLeft, bottomRight) {
		}
		protected override IEnumerator CreateCellsEnumerator(IRowBase row) {
			return row.Cells.GetExistingNonEmptyVisibleCells(TopLeft.Column, BottomRight.Column, Reversed).GetEnumerator();
		}
	}
	#endregion
	#region EntireWorksheetExistingCellRangeEnumerator
	public class EntireWorksheetExistingCellRangeEnumerator : CellBaseEnumeratorBase {
		static readonly CellPosition topLeft = new CellPosition(0, 0, PositionType.Relative, PositionType.Relative);
		public EntireWorksheetExistingCellRangeEnumerator(ICellTable worksheet)
			: base(worksheet, topLeft) {
			Reset();
		}
		protected override IEnumerator CreateRowsEnumerator() {
			return Worksheet.Rows.GetEnumerator();
		}
		protected override IEnumerator CreateCellsEnumerator(IRowBase row) {
			return row.Cells.GetEnumerator();
		}
	}
	#endregion
	#region CellRangeExistingVisibleCellsEnumerator (abstract class)
	public abstract class CellRangeExistingVisibleCellsEnumerator : CellRangeExistingCellsEnumerator {
		protected CellRangeExistingVisibleCellsEnumerator(ICellTable worksheet, CellPosition topLeft, CellPosition bottomRight)
			: base(worksheet, topLeft, bottomRight) {
		}
		new public Worksheet Worksheet { get { return (Worksheet)base.Worksheet; } }
		protected override IEnumerator CreateRowsEnumerator() {
			return GetOrderedRowsEnumerator();
		}
		protected IOrderedEnumerator<Row> GetOrderedRowsEnumerator() {
			return Worksheet.Rows.GetExistingVisibleRowsEnumerator(TopLeft.Row, BottomRight.Row, false);
		}
	}
	#endregion
	#region CellRangeLayoutVisibleCellsEnumerator
	public class CellRangeLayoutVisibleCellsEnumerator : CellRangeExistingVisibleCellsEnumerator {
		readonly IList<IColumnRange> layoutVisibleColumns;
		readonly IList<ITableBase> layoutVisibleTables;
		readonly bool continuousRows;
		public CellRangeLayoutVisibleCellsEnumerator(Worksheet worksheet, CellPosition topLeft, CellPosition bottomRight, bool continuousRows)
			: base(worksheet, topLeft, bottomRight) {
			this.layoutVisibleColumns = CalculateLayoutVisibleColumns();
			this.layoutVisibleTables = worksheet.TryGetTableBases(new CellRange(worksheet, topLeft, bottomRight));
			this.continuousRows = continuousRows;
		}
		public new Worksheet Worksheet { get { return (Worksheet)base.Worksheet; } }
		bool HasVisibleTables { get { return layoutVisibleTables.Count > 0; } }
		List<IColumnRange> CalculateLayoutVisibleColumns() {
			List<IColumnRange> result = new List<IColumnRange>();
			foreach (IColumnRange columnRange in Worksheet.Columns.GetExistingColumnRanges())
				if ((columnRange.EndIndex >= TopLeft.Column) && (columnRange.StartIndex <= BottomRight.Column) && columnRange.HasVisibleFill)
					result.Add(columnRange);
			return result;
		}
		protected override IEnumerator CreateRowsEnumerator() {
			if (layoutVisibleColumns.Count <= 0 && !HasVisibleTables && !continuousRows) {
				return base.CreateRowsEnumerator();
			}
			else {
				IOrderedEnumerator<Row> existingRows = GetOrderedRowsEnumerator();
				IOrderedEnumerator<Row> fakeRows = new ContinuousFakeRowsEnumerator((Worksheet)Worksheet, TopLeft.Row, BottomRight.Row, false, null);
				return new JoinedOrderedEnumerator<Row>(existingRows, fakeRows);
			}
		}
		protected override IEnumerator CreateCellsEnumerator(IRowBase row) {
			Row sheetRow = row as Row;
			if (sheetRow == null)
				return row.Cells.GetExistingVisibleCells(TopLeft.Column, BottomRight.Column, false).GetEnumerator();
			if (row.HasVisibleFill) {
				IOrderedEnumerator<ICell> columnsCellsEnumerator = new RowLayoutVisibleCellsEnumerator(sheetRow, TopLeft.Column, BottomRight.Column);
				IOrderedEnumerator<ICell> cellsEnumerator = CreateExistingVisibleCellsEnumerator(sheetRow);
				return new JoinedOrderedEnumerator<ICell>(cellsEnumerator, columnsCellsEnumerator);
			}
			else {
				if (layoutVisibleColumns.Count <= 0)
					return ApplyTableFormatting(CreateExistingVisibleCellsEnumerator(sheetRow), sheetRow);
				else {
					IOrderedEnumerator<ICell> columnsCellsEnumerator = new ColumnsLayoutVisibleCellsEnumerator(layoutVisibleColumns.GetEnumerator(), sheetRow, TopLeft.Column, BottomRight.Column);
					IOrderedEnumerator<ICell> cellsEnumerator = CreateExistingVisibleCellsEnumerator(sheetRow);
					return ApplyTableFormatting(new JoinedOrderedEnumerator<ICell>(cellsEnumerator, columnsCellsEnumerator), sheetRow);
				}
			}
		}
		IOrderedEnumerator<ICell> CreateExistingVisibleCellsEnumerator(Row sheetRow) {
			return sheetRow.Cells.GetExistingVisibleCellsEnumerator(TopLeft.Column, BottomRight.Column, false);
		}
		IOrderedEnumerator<ICell> ApplyTableFormatting(IOrderedEnumerator<ICell> cells, Row sheetRow) {
			if (!HasVisibleTables)
				return cells;
			List<ITableBase> tables = GetLayoutVisibleTables(sheetRow.Index);
			if (tables.Count == 0)
				return cells;
			tables.Sort(new TableLeftPositionComparer());
			TableRowAllCellsEnumerator rowAllTableCells = new TableRowAllCellsEnumerator(Worksheet, sheetRow, tables.GetEnumerator(), TopLeft.Column, BottomRight.Column);
			return new JoinedOrderedEnumerator<ICell>(cells, rowAllTableCells);
		}
		List<ITableBase> GetLayoutVisibleTables(int rowIndex) {
			CellPosition topLeft = new CellPosition(TopLeft.Column, rowIndex);
			CellPosition bottomRight = new CellPosition(BottomRight.Column, rowIndex);
			CellRange intersectedRange = new CellRange(Worksheet, topLeft, bottomRight);
			int count = layoutVisibleTables.Count;
			List<ITableBase> result = new List<ITableBase>();
			for (int i = 0; i < count; i++) {
				ITableBase table = layoutVisibleTables[i];
				CellRangeBase tableRange = table.WholeRange;
				if (intersectedRange.Intersects(tableRange))
					result.Add(table);
			}
			return result;
		}
	}
	#endregion
	#region CellRangeInfo
	public class CellRangeInfo {
		public CellRangeInfo() { }
		public CellRangeInfo(CellPosition first, CellPosition last) {
			First = first;
			Last = last;
		}
		public CellPosition First { get; set; }
		public CellPosition Last { get; set; }
		public bool IsValid() {
			return First.Row <= Last.Row && First.Column <= Last.Column;
		}
		public bool HasCommonCells(CellRangeInfo other) {
			bool hasCommonColumns = Math.Max(First.Column, other.First.Column) <= Math.Min(Last.Column, other.Last.Column);
			bool hasCommonRows = Math.Max(First.Row, other.First.Row) <= Math.Min(Last.Row, other.Last.Row);
			return hasCommonColumns && hasCommonRows;
		}
		public bool ContainsCell(CellPosition position) {
			if (!IsValid())
				return false;
			return position.Column >= First.Column && position.Column <= Last.Column && position.Row >= First.Row && position.Row <= Last.Row;
		}
		public override bool Equals(object obj) {
			CellRangeInfo other = obj as CellRangeInfo;
			if (other == null)
				return false;
			return First.Equals(other.First) && Last.Equals(other.Last);
		}
		public override int GetHashCode() {
			return First.GetHashCode() ^ Last.GetHashCode();
		}
		public CellRangeInfo Clone() {
			return new CellRangeInfo(First, Last);
		}
	}
	#endregion
	public class CellRangeList : List<CellRangeBase> { }
	#region CellRangeToString
	public static class CellRangeToString {
		public static string GetReferenceCommon(CellRangeBase modelRange, bool isR1C1Style,
			CellPosition baseCellPosition,
			PositionType rowPositionType,
			PositionType columnPositionType,
			bool includeSheetName) {
			CellRangeBase cloneRange = modelRange.GetWithModifiedPositionType(columnPositionType, rowPositionType);
			Model.WorkbookDataContext context = modelRange.Worksheet.Workbook.DataContext;
			try {
				if (baseCellPosition.IsValid)
					context.PushCurrentCell(baseCellPosition);
				context.PushUseR1C1(isR1C1Style);
				return cloneRange.ToString(context, includeSheetName);
			}
			finally {
				if (baseCellPosition.IsValid)
					context.PopCurrentCell();
				context.PopUseR1C1();
			}
		}
	}
	#endregion
	#region ISheetPosition
	public interface ISheetPosition {
		int SheetId { get; }
		int LeftColumnIndex { get; }
		int RightColumnIndex { get; }
		int TopRowIndex { get; }
		int BottomRowIndex { get; }
	}
	#endregion
	public static class SheetPositionExtensions {
		public static bool Includes(this ISheetPosition first, ISheetPosition second) {
			if (first.SheetId != second.SheetId)
				return false;
			return first.LeftColumnIndex <= second.LeftColumnIndex &&
				   first.TopRowIndex <= second.TopRowIndex &&
				   first.RightColumnIndex >= second.RightColumnIndex &&
				   first.BottomRowIndex >= second.BottomRowIndex;
		}
		public static bool Intersects(this ISheetPosition first, ISheetPosition second) {
			if (first.SheetId != second.SheetId)
				return false;
			bool hasCommonColumns =
				Math.Max(first.LeftColumnIndex, second.LeftColumnIndex) <= Math.Min(first.RightColumnIndex, second.RightColumnIndex);
			bool hasCommonRows =
				Math.Max(first.TopRowIndex, second.TopRowIndex) <= Math.Min(first.BottomRowIndex, second.BottomRowIndex);
			return hasCommonColumns && hasCommonRows;
		}
	}
}
