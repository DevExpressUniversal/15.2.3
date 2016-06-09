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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class Sequence {
		enum DateTimeDistance {
			None,
			Irregular,
			Month,
			Year
		}
		CellRange sourceRange;
		List<double> values;
		int sourceCellLength;
		int sequenceStartIndex;
		bool reverse;
		DateTimeDistance distanceType = DateTimeDistance.None;
		int distanceValue = 0;
		public Sequence(CellRange sourceRange, int sourceRangeLength, int sequenceStartIndex, bool reverse) {
			Guard.ArgumentNotNull(sourceRange, "sourceRange");
			this.sourceRange = sourceRange;
			this.reverse = reverse;
			this.sourceCellLength = sourceRangeLength;
			this.sequenceStartIndex = sequenceStartIndex;
		}
		public double GetValue(int sourceCellRepeatIndex, int sourceCellIndex) {
			return values[sourceCellLength * (sourceCellRepeatIndex + 1) + sourceCellIndex - sequenceStartIndex];
		}
		public bool ContainsCell(ICell cell) {
			return sourceRange.ContainsCell(cell);
		}
		public void GenerateValues(int sourceCellRepeatsCount) {
			values = new List<double>();
			IEnumerator<VariantValue> variantValues = sourceRange.GetExistingValuesEnumerator();
			while (variantValues.MoveNext())
				values.Add(variantValues.Current.NumericValue);
			if (reverse)
				values.Reverse();
			if (IsDateTimeValues()) {
				CalculateDateDistance();
				if (distanceType != DateTimeDistance.None) {
					GenerateDateValues(sourceCellRepeatsCount);
					return;
				}
			}
			GenerateNumericValues(sourceCellRepeatsCount);
		}
		void GenerateNumericValues(int sourceCellRepeatsCount) {
			double sumX = 0;
			double sumY = 0;
			double sumXY = 0;
			double sumSquareX = 0;
			int index = 0;
			for (; index < values.Count; ++index) {
				sumX += index;
				sumY += values[index];
				sumXY += index * values[index];
				sumSquareX += Math.Pow(index, 2);
			}
			int listCount = index + sourceCellRepeatsCount * sourceCellLength;
			for (; index < listCount; ++index) {
				double factorA = (index * sumXY - sumX * sumY) / (index * sumSquareX - Math.Pow(sumX, 2));
				if (double.IsNaN(factorA))
					factorA = reverse ? -1 : 1;
				double factorB = (sumY - factorA * sumX) / index;
				double result = factorA * index + factorB;
				values.Add(result);
				sumX += index;
				sumY += result;
				sumXY += index * result;
				sumSquareX += Math.Pow(index, 2);
			}
		}
		bool IsDateTimeValues() {
			bool isDateTimeEverywhere = true;
			bool isAllCellsEmpty = true;
			foreach (ICellBase cell in sourceRange.GetExistingCellsEnumerable()) {
				VariantValue cellValue = cell.Value;
				if (cellValue.IsEmpty || cellValue.IsError)
					continue;
				isAllCellsEmpty = false;
				IFormatStringAccessor formatStringAccessor = cell as IFormatStringAccessor;
				if (formatStringAccessor != null) {
					NumberFormat format = new NumberFormat(0, formatStringAccessor.FormatString);
					if (!format.IsDateTime || !cellValue.IsNumeric) {
						isDateTimeEverywhere = false;
						break;
					}
				}
			}
			return isDateTimeEverywhere && !isAllCellsEmpty;
		}
		void CalculateDateDistance() {
			distanceType = DateTimeDistance.None;
			WorkbookDataContext context = sourceRange.Worksheet.Workbook.DataContext;
			DateTime previous = context.FromDateTimeSerial(values[0]);
			for (int i = 1; i < values.Count; i++) {
				DateTime current = context.FromDateTimeSerial(values[i]);
				int dayDelta = current.Day - previous.Day;
				if (dayDelta != 0) {
					distanceType = DateTimeDistance.None;
					return;
				}
				if (distanceType == DateTimeDistance.Year) {
					if (previous.AddYears(distanceValue) != current) {
						distanceType = DateTimeDistance.Irregular;
						return;
					}
				}
				else if (distanceType == DateTimeDistance.Month) {
					if (previous.AddMonths(distanceValue) != current) {
						distanceType = DateTimeDistance.Irregular;
						return;
					}
				}
				else {
					int yearDelta = current.Year - previous.Year;
					int monthDelta = current.Month - previous.Month;
					if (dayDelta != 0 || (yearDelta == 0 && monthDelta == 0)) {
						distanceType = DateTimeDistance.None;
						return;
					}
					if (monthDelta == 0) {
						distanceType = DateTimeDistance.Year;
						distanceValue = yearDelta;
					}
					else {
						distanceType = DateTimeDistance.Month;
						distanceValue = monthDelta + yearDelta * 12;
					}
				}
				previous = current;
			}
		}
		void GenerateDateValues(int sourceCellRepeatsCount) {
			int count = sourceCellRepeatsCount * sourceCellLength;
			if (distanceType == DateTimeDistance.Irregular) {
				int originalCount = values.Count;
				for (int i = 0; i < count; i++)
					values.Add(values[i % originalCount]);
			}
			else {
				WorkbookDataContext context = sourceRange.Worksheet.Workbook.DataContext;
				DateTime dateValue = context.FromDateTimeSerial(values[values.Count - 1]);
				for (int i = 0; i < count; i++) {
					dateValue = (distanceType == DateTimeDistance.Month) ? dateValue.AddMonths(distanceValue) : dateValue.AddYears(distanceValue);
					values.Add(context.ToDateTimeSerialDouble(dateValue));
				}
			}
		}
	}
}
