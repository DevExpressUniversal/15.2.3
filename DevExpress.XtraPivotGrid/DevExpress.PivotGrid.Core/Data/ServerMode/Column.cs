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
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.PivotGrid.ServerMode.Sorting;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.ServerMode {
	public class ServerModeColumn : QueryColumn, ISelectCriteriaConvertible, IDrillDownProvider {
		static Exception GetUnsupportedException(object value) {
			return new Exception(string.Format(unsupportedText, value.GetType().Name, value));
		}
		const string unsupportedText = "Cannot apply {0}: '{1}'";
		string uniqueName;
		bool isMeasure;
		PivotGroupInterval groupInterval;
		int groupIntervalNumericRange;
		TopValueMode topValueMode;
		bool calculateTotals = true;
		string drillDownName;
		CriteriaSyntax CriteriaSyntax { get { return ((ServerModeMetadata)Metadata.Owner).Exec.CriteriaSyntax; } }
		bool IsServerCriteria { get { return (CriteriaSyntax & CriteriaSyntax.ServerCriteria) != 0; } }
		bool ForceInt16SummaryAsInt32 { get { return (CriteriaSyntax & CriteriaSyntax.ForceInt16SummaryAsInt32) != 0; } }
		public override bool IsMeasure { get { return isMeasure; } }
		public override string UniqueName { get { return string.IsNullOrWhiteSpace(uniqueName) ? base.UniqueName : uniqueName; } }
		public PivotGroupInterval GroupInterval { get { return groupInterval; } }
		public TopValueMode TopValueMode { get { return topValueMode; } }
		public bool CalculateTotals { get { return calculateTotals; } }
		public override bool TopValueHiddenOthersShowedInTotal {
			get {
				if(TopValueCount == 0)
					return true;
				if(TopValueShowOthers)
					return true;
				return TopValueMode == XtraPivotGrid.TopValueMode.ParentFieldValues;
			}
		}
		internal ServerModeColumn(IQueryMetadataColumn column, bool isMeasure, string uniqueName = null)
			: base(column) {
			Guard.ArgumentNotNull(column, "column");
			this.isMeasure = isMeasure;
			this.uniqueName = uniqueName;
		}
		public override void Assign(DevExpress.XtraPivotGrid.PivotGridFieldBase field, bool forceSort) {
			base.Assign(field, forceSort);
			groupInterval = field.GroupInterval;
			groupIntervalNumericRange = field.GroupIntervalNumericRange;
			topValueMode = field.TopValueMode;
			calculateTotals = field.CalculateTotals;
			drillDownName = field.DrillDownColumnName;
		}
		public override bool Equals(object obj) {
			ServerModeColumn col = obj as ServerModeColumn;
			return base.Equals(obj)
				&& SummaryType == col.SummaryType
				&& isMeasure == col.isMeasure
				&& uniqueName == col.uniqueName
				&& groupInterval == col.groupInterval
				&& topValueMode == col.topValueMode
				&& calculateTotals == col.calculateTotals;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected override bool Equals(DevExpress.XtraPivotGrid.PivotGridFieldBase field, bool forceSort) {
			return base.Equals(field, forceSort) && field.GroupInterval == groupInterval && field.TopValueMode == topValueMode && field.CalculateTotals == calculateTotals && drillDownName == field.DrillDownColumnName;
		}
		internal void SetIsMeasure(bool value) {
			isMeasure = value;
		}
		public override bool HasMeasureData { get { return base.HasMeasureData && !object.ReferenceEquals(null, GetRawCriteria()); } } 
		CriteriaOperator GetSummaryExpression(CriteriaOperator opCore, PivotSummaryType summaryType) {
			OperandProperty prop = opCore as OperandProperty;
			if(!ReferenceEquals(prop, null)) {
				IQueryMetadataColumn col = Metadata.Owner.Columns[prop.PropertyName];
				if(col != null && col.DataType == typeof(Int16) && ForceInt16SummaryAsInt32)
					opCore = new FunctionOperator(FunctionOperatorType.ToInt, prop);
			}
			QueryOperand prop2 = opCore as QueryOperand;
			if(!ReferenceEquals(null, prop2) && prop2.ColumnType == DBColumnType.Int16 && ForceInt16SummaryAsInt32)
				opCore = new FunctionOperator(FunctionOperatorType.ToInt, prop2);
			switch(summaryType) {
				case PivotSummaryType.Count:
					return ColumnCriteriaHelper.GetAggregate(opCore, Aggregate.Count, IsServerCriteria);
				case PivotSummaryType.Average:
					return ColumnCriteriaHelper.GetAggregate(opCore, Aggregate.Avg, IsServerCriteria);
				case PivotSummaryType.Max:
					return ColumnCriteriaHelper.GetAggregate(opCore, Aggregate.Max, IsServerCriteria);
				case PivotSummaryType.Min:
					return ColumnCriteriaHelper.GetAggregate(opCore, Aggregate.Min, IsServerCriteria);
				case PivotSummaryType.StdDev:
					return new FunctionOperator(
							FunctionOperatorType.Iif,
							new FunctionOperator(FunctionOperatorType.IsNull, GetSummaryExpression(opCore, PivotSummaryType.Var)),
							new ConstantValue(null),
							new FunctionOperator(FunctionOperatorType.Sqr, GetSummaryExpression(opCore, PivotSummaryType.Var))
						);
				case PivotSummaryType.StdDevp:
					return new FunctionOperator(FunctionOperatorType.Sqr, GetSummaryExpression(opCore, PivotSummaryType.Varp));
				case PivotSummaryType.Sum:
					return ColumnCriteriaHelper.GetAggregate(opCore, Aggregate.Sum, IsServerCriteria);
				case PivotSummaryType.Var:
					return new FunctionOperator(
						FunctionOperatorType.Iif,
						new BinaryOperator(
							GetSummaryExpression(opCore, PivotSummaryType.Count),
							2,
							BinaryOperatorType.Less
						),
						new ConstantValue(null),
						new BinaryOperator(
							new FunctionOperator(
								FunctionOperatorType.Abs,
								new BinaryOperator(
									ColumnCriteriaHelper.GetDoubleSquareSum(new FunctionOperator(FunctionOperatorType.ToDouble, opCore), IsServerCriteria),
									new BinaryOperator(
										new BinaryOperator(
											new FunctionOperator(FunctionOperatorType.ToDouble, GetSummaryExpression(opCore, PivotSummaryType.Sum)),
											new FunctionOperator(FunctionOperatorType.ToDouble, GetSummaryExpression(opCore, PivotSummaryType.Sum)),
											BinaryOperatorType.Multiply
											),
										GetSummaryExpression(opCore, PivotSummaryType.Count),
										BinaryOperatorType.Divide
									),
									BinaryOperatorType.Minus
								)
							),
							new BinaryOperator(
								GetSummaryExpression(opCore, PivotSummaryType.Count),
								1,
								BinaryOperatorType.Minus
							),
							BinaryOperatorType.Divide
						)
					);
				case PivotSummaryType.Varp:
					return new BinaryOperator(
						new FunctionOperator(
							FunctionOperatorType.Abs,
							new BinaryOperator(
								ColumnCriteriaHelper.GetDoubleSquareSum(new FunctionOperator(FunctionOperatorType.ToDouble, opCore), IsServerCriteria),
								new BinaryOperator(
									new BinaryOperator(
										new FunctionOperator(FunctionOperatorType.ToDouble, GetSummaryExpression(opCore, PivotSummaryType.Sum)),
										new FunctionOperator(FunctionOperatorType.ToDouble, GetSummaryExpression(opCore, PivotSummaryType.Sum)),
										BinaryOperatorType.Multiply
									),
									GetSummaryExpression(opCore, PivotSummaryType.Count),
									BinaryOperatorType.Divide
								),
								BinaryOperatorType.Minus
							)
						),
						GetSummaryExpression(opCore, PivotSummaryType.Count),
						BinaryOperatorType.Divide
					);
				case PivotSummaryType.Custom:
					return opCore;
				default:
					throw GetUnsupportedException(summaryType);
			}
		}
#if DEBUGTEST
		protected
#endif
		CriteriaOperator GetDimensionExpression() {
			switch(groupInterval) {
				case PivotGroupInterval.Alphabetical:
					return new FunctionOperator(FunctionOperatorType.Substring, GetRawCriteria(), new ConstantValue(0), new ConstantValue(1));
				case PivotGroupInterval.Numeric:
					return ColumnCriteriaHelper.GetNumericRangeGroupIntervalExpression(
						ColumnCriteriaHelper.GetTruncatedFractionalPartExpression(
							new FunctionOperator(FunctionOperatorType.ToDecimal, GetRawCriteria())
						),
						groupIntervalNumericRange
				);
				case PivotGroupInterval.Date:
					return new FunctionOperator(FunctionOperatorType.GetDate, GetRawCriteria());
				case PivotGroupInterval.DateDay:
					return new FunctionOperator(FunctionOperatorType.GetDay, GetRawCriteria());
				case PivotGroupInterval.DateMonth:
					return new FunctionOperator(FunctionOperatorType.GetMonth, GetRawCriteria());
				case PivotGroupInterval.DateQuarter:
					return FunctionOperatorAdd.Create(IsServerCriteria, FunctionOperatorTypeAdd.GetQuarter, GetRawCriteria());
				case PivotGroupInterval.DateYear:
					return new FunctionOperator(FunctionOperatorType.GetYear, GetRawCriteria());
				case PivotGroupInterval.Hour:
					return new FunctionOperator(FunctionOperatorType.GetHour, GetRawCriteria());
				case PivotGroupInterval.Minute:
					return new FunctionOperator(FunctionOperatorType.GetMinute, GetRawCriteria());
				case PivotGroupInterval.Second:
					return new FunctionOperator(FunctionOperatorType.GetSecond, GetRawCriteria());
				case PivotGroupInterval.DateDayOfWeek:
					return new FunctionOperator(FunctionOperatorType.GetDayOfWeek, GetRawCriteria());
				case PivotGroupInterval.DateDayOfYear:
					return new FunctionOperator(FunctionOperatorType.GetDayOfYear, GetRawCriteria());
				case PivotGroupInterval.DateWeekOfMonth:
					return ColumnCriteriaHelper.GetWeekOfMonthExpression(GetRawCriteria());
				case PivotGroupInterval.DateWeekOfYear:
					return FunctionOperatorAdd.Create(IsServerCriteria, FunctionOperatorTypeAdd.GetWeekOfYear, GetRawCriteria());
				case PivotGroupInterval.DayAge:
					return ColumnCriteriaHelper.GetNumericRangeGroupIntervalExpression(
						ColumnCriteriaHelper.GetDateDiffFullDays(
							GetRawCriteria(),
							new FunctionOperator(FunctionOperatorType.LocalDateTimeToday)
						),
						groupIntervalNumericRange
					);
				case PivotGroupInterval.WeekAge:
					return ColumnCriteriaHelper.GetNumericRangeGroupIntervalExpression(
						 new BinaryOperator(
							ColumnCriteriaHelper.GetDateDiffFullDays(
								GetRawCriteria(),
								new FunctionOperator(FunctionOperatorType.LocalDateTimeToday)
							),
							new ConstantValue(7),
							BinaryOperatorType.Divide
						),
						groupIntervalNumericRange
					);
				case PivotGroupInterval.MonthAge:
					return ColumnCriteriaHelper.GetNumericRangeGroupIntervalExpression(
						ColumnCriteriaHelper.GetDateDiffFullMonths(
							GetRawCriteria(),
							new FunctionOperator(FunctionOperatorType.LocalDateTimeToday)
						),
						groupIntervalNumericRange
					);
				case PivotGroupInterval.YearAge:
					return ColumnCriteriaHelper.GetNumericRangeGroupIntervalExpression(
						ColumnCriteriaHelper.GetDateDiffFullYears(
							GetRawCriteria(),
							new FunctionOperator(FunctionOperatorType.LocalDateTimeToday)
						),
						groupIntervalNumericRange
					);
				case PivotGroupInterval.DateMonthYear:
					return FunctionOperatorAdd.Create(IsServerCriteria, FunctionOperatorTypeAdd.GetDateMonthYear, GetRawCriteria());
				case PivotGroupInterval.DateQuarterYear:
					return FunctionOperatorAdd.Create(IsServerCriteria, FunctionOperatorTypeAdd.GetDateQuarterYear, GetRawCriteria());
				case PivotGroupInterval.DateHour:
					return FunctionOperatorAdd.Create(IsServerCriteria, FunctionOperatorTypeAdd.GetDateHour, GetRawCriteria());
				case PivotGroupInterval.DateHourMinute:
					return FunctionOperatorAdd.Create(IsServerCriteria, FunctionOperatorTypeAdd.GetDateHourMinute, GetRawCriteria());
				case PivotGroupInterval.DateHourMinuteSecond:
					return FunctionOperatorAdd.Create(IsServerCriteria, FunctionOperatorTypeAdd.GetDateHourMinuteSecond, GetRawCriteria());
				case PivotGroupInterval.Default:
					return GetRawCriteria();
				default:
				case PivotGroupInterval.Custom:
					throw GetUnsupportedException(groupInterval);
			}
		}
#if DEBUGTEST
		protected virtual
#endif
		CriteriaOperator GetRawCriteria() {
			return ((IRawCriteriaConvertible)Metadata).GetRawCriteria();
		}
		CriteriaOperator ISelectCriteriaConvertible.GetSelectCriteria() {
			if(isMeasure) {
				CriteriaOperator opCore = GetRawCriteria();
				if(object.ReferenceEquals(null, opCore))
					return new OperandValue(null);
				if(HasAggregateCriteriaChecker.Check(opCore))
					return opCore;
				return GetSummaryExpression(opCore, SummaryType);
			} else
				return GetDimensionExpression();
		}
		CriteriaOperator IGroupCriteriaConvertible.GetGroupCriteria() {
			if(isMeasure)
				return GetRawCriteria();
			else
				return GetDimensionExpression();
		}
		CriteriaOperator IRawCriteriaConvertible.GetRawCriteria() {
			if(IsMeasure && SummaryType == PivotSummaryType.Custom)
				return ((FunctionOperator)GetRawCriteria()).Operands[1];
			else
				return GetRawCriteria();
		}
		internal override IComparer<TMember> GetByMemberComparer<TMember>(Func<object, string> customText) {
			if(SortMode == PivotSortMode.DisplayText)
				return new QueryMemberProviderComparer<TMember>(new ByMemberCustomDisplayTextComparer(customText));
			if(Metadata.SafeDataType != typeof(object) && typeof(IComparable).IsAssignableFrom(Metadata.SafeDataType))
				return new ByMemberValueComparer<TMember>();
			else
				return new EmptyComparer<TMember>();
		}
		#region IDrillDownProvider
		string IDrillDownProvider.DrillDownName {
			get {
				if(!string.IsNullOrEmpty(drillDownName))
					return drillDownName;
				if(!string.IsNullOrEmpty(Name))
					return Name;
				if(!string.IsNullOrEmpty(UniqueName))
					return UniqueName;
				return ((IRawCriteriaConvertible)this).ToString();
			}
		}
		string IDrillDownProvider.Name {
			get { return Name; }
		}
		Type IDrillDownProvider.DataType {
			get { return Metadata.DataType; }
		}
		#endregion
	}
}
