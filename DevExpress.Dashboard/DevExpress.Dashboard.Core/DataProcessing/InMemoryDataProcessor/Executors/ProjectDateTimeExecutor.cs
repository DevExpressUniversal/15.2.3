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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.XtraPivotGrid;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	class ProjectDateTimeExecutor : ExecutorBase<ProjectDateTime>{
		DataVectorBase resultVector;
		DataVector<DateTime> inputVector;
		Action worker;
		public ProjectDateTimeExecutor(ProjectDateTime operation, IExecutorQueueContext context)
			: base(operation, context) {
			this.resultVector = DataVectorBase.New(operation.OperationType, context.VectorSize);
			this.Result = new SingleFlowExecutorResult(resultVector);
			this.inputVector = (DataVector<DateTime>)((SingleFlowExecutorResult)context.GetExecutorResult(operation.Argument)).ResultVector;
			PivotGroupInterval groupInterval = Operation.GroupInterval;
			if(ProjectDateTime.IntGroupIntervals.Contains(Operation.GroupInterval)) {
				Func<DateTime, int> resultCalculator = IntResultCalculators[groupInterval];
				worker = () => CalculateResult<int>(resultCalculator);
			} else if(ProjectDateTime.DayOfWeekGroupIntervals.Contains(Operation.GroupInterval)) {
				Func<DateTime, DayOfWeek> resultCalculator = DayOfWeekResultCalculators[groupInterval];
				worker = () => CalculateResult<DayOfWeek>(resultCalculator);
			} else if(ProjectDateTime.DateTimeGroupIntervals.Contains(Operation.GroupInterval)) {
				Func<DateTime, DateTime> resultCalculator = DateTimeResultCalculators[groupInterval];
				worker = () => CalculateResult<DateTime>(resultCalculator);
			}
		}
		protected override ExecutorProcessResult Process() {
			worker();
			return ExecutorProcessResult.ResultReady;
		}
		void CalculateResult<T>(Func<DateTime, T> resultCalculator) {
			DataVector<T> typedResultVector = resultVector as DataVector<T>;
			for(int i = 0; i < inputVector.Count; i++)
				if(inputVector.SpecialData[i] == SpecialDataValue.None)
					typedResultVector.Data[i] = resultCalculator(inputVector.Data[i]);
				else
					typedResultVector.SpecialData[i] = inputVector.SpecialData[i];
			resultVector.Count = inputVector.Count;
		}
		#region static 
		static Dictionary<PivotGroupInterval, Func<DateTime, int>> IntResultCalculators = new Dictionary<PivotGroupInterval, Func<DateTime, int>>();
		static Dictionary<PivotGroupInterval, Func<DateTime, DayOfWeek>> DayOfWeekResultCalculators = new Dictionary<PivotGroupInterval, Func<DateTime, DayOfWeek>>();
		static Dictionary<PivotGroupInterval, Func<DateTime, DateTime>> DateTimeResultCalculators = new Dictionary<PivotGroupInterval, Func<DateTime, DateTime>>();
		static CultureInfo CultureInfo { get { return CultureInfo.CurrentCulture; } }
		static int GetWeekOfMonth(DateTime dateTime) {
			DateTime firstDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
			int weekOfYear = GetWeekOfYear(dateTime);
			int weekOfMonth = weekOfYear - GetWeekOfYear(firstDayOfMonth) + 1;
			return weekOfMonth < 0 ? weekOfYear + 1 : weekOfMonth;
		}
		static int GetWeekOfYear(DateTime dateTime) {
			return CultureInfo.Calendar.GetWeekOfYear(dateTime, CultureInfo.DateTimeFormat.CalendarWeekRule, CultureInfo.DateTimeFormat.FirstDayOfWeek);
		}
		static ProjectDateTimeExecutor() {
			IntResultCalculators[PivotGroupInterval.DateDay] = date => date.Day;
			IntResultCalculators[PivotGroupInterval.DateDayOfYear] = date => date.DayOfYear;
			IntResultCalculators[PivotGroupInterval.Hour] = date => date.Hour;
			IntResultCalculators[PivotGroupInterval.Minute] = date => date.Minute;
			IntResultCalculators[PivotGroupInterval.DateMonth] = date => date.Month;
			IntResultCalculators[PivotGroupInterval.DateQuarter] = date => (date.Month - 1) / 3 + 1;
			IntResultCalculators[PivotGroupInterval.Second] = date => date.Second;
			IntResultCalculators[PivotGroupInterval.DateWeekOfMonth] = date => GetWeekOfMonth(date);
			IntResultCalculators[PivotGroupInterval.DateWeekOfYear] = date => GetWeekOfYear(date);
			IntResultCalculators[PivotGroupInterval.DateYear] = date => date.Year;
			DayOfWeekResultCalculators[PivotGroupInterval.DateDayOfWeek] = date => date.DayOfWeek;
			DateTimeResultCalculators[PivotGroupInterval.DateHour] = date => new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
			DateTimeResultCalculators[PivotGroupInterval.DateHourMinute] = date => new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
			DateTimeResultCalculators[PivotGroupInterval.DateHourMinuteSecond] = date => new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
			DateTimeResultCalculators[PivotGroupInterval.Date] = date => date.Date;
			DateTimeResultCalculators[PivotGroupInterval.DateMonthYear] = date => new DateTime(date.Year, date.Month, 1);
			DateTimeResultCalculators[PivotGroupInterval.DateQuarterYear] = date => new DateTime(date.Year, (date.Month - 1) / 3 * 3 + 1, 1);
			DateTimeResultCalculators[PivotGroupInterval.Default] = date => date;
		}
		#endregion
	}
}
