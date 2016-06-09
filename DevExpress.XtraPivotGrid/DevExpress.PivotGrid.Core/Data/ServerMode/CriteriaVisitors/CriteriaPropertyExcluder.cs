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
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.QueryMode;
using PivotGroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval;
namespace DevExpress.PivotGrid.ServerMode {
	class CriteriaPropertyExcluder : CriteriaPatcherBase {
		internal static CriteriaOperator Patch(CriteriaOperator criteria, string name) {
			return new CriteriaPropertyExcluder(name).Process(criteria);
		}
		string name;
		protected CriteriaPropertyExcluder(string name) {
			this.name = name; 
		}
		public override CriteriaOperator Visit(OperandProperty theOperand) {
			if(theOperand.PropertyName == name)
				return null;
			return theOperand;
		}
	}
	class GroupIntervalFilterSimplifier : CriteriaPatcherBase {
		internal static CriteriaOperator Patch(CriteriaOperator criteria, QueryColumns<ServerModeColumn> columns) {
			return new GroupIntervalFilterSimplifier(columns).Process(criteria);
		}
		static BinaryOperatorType GetOppositeOperatorType(BinaryOperatorType type) {
			switch(type) {
				case BinaryOperatorType.Less:
					return BinaryOperatorType.Greater;
				case BinaryOperatorType.LessOrEqual:
					return BinaryOperatorType.GreaterOrEqual;
				case BinaryOperatorType.Greater:
					return BinaryOperatorType.Less;
				case BinaryOperatorType.GreaterOrEqual:
					return BinaryOperatorType.LessOrEqual;
				default:
					return type;
			}
		}
		QueryColumns<ServerModeColumn> columns;
		protected GroupIntervalFilterSimplifier(QueryColumns<ServerModeColumn> columns) {
			this.columns = columns;
		}
		public override CriteriaOperator Visit(BinaryOperator theOperator) {
			if(!(theOperator.OperatorType == BinaryOperatorType.Less || theOperator.OperatorType == BinaryOperatorType.LessOrEqual ||
				theOperator.OperatorType == BinaryOperatorType.Greater || theOperator.OperatorType == BinaryOperatorType.GreaterOrEqual))
				return base.Visit(theOperator);
			CriteriaOperator patchedOperator = null;
			OperandValue value;
			OperandProperty property = theOperator.LeftOperand as OperandProperty;
			if(!ReferenceEquals(property, null)) {
				value = theOperator.RightOperand as OperandValue;
				if(!ReferenceEquals(value, null))
					patchedOperator = Patch(property, value, theOperator.OperatorType);
			}
			else {
				property = theOperator.RightOperand as OperandProperty;
				if(!ReferenceEquals(property, null)) {
					value = theOperator.LeftOperand as OperandValue;
					if(!ReferenceEquals(value, null))
						patchedOperator = Patch(property, value, GetOppositeOperatorType(theOperator.OperatorType));
				}
			}
			return patchedOperator ?? base.Visit(theOperator);
		}
		CriteriaOperator Patch(OperandProperty property, OperandValue value, BinaryOperatorType binaryType) {
			if(object.ReferenceEquals(null, property) || object.ReferenceEquals(null, value) || object.Equals(property, value))
				return null;
			ServerModeColumn column = columns[property.PropertyName];
			if(column == null)
				return null;
			BinaryOperatorType newOperatorType;
			object ungroupedValue = GetUngroupedValue(column.GroupInterval, value.Value, binaryType, out newOperatorType);
			return object.ReferenceEquals(null, ungroupedValue) ? null : new BinaryOperator(column.Metadata.Name, ungroupedValue, newOperatorType);
		}
		object GetUngroupedValue(PivotGroupInterval groupInterval, object value, BinaryOperatorType currentOperatorType, out BinaryOperatorType newOperatorType) {
			try {
				newOperatorType = GetNewOperatorType(currentOperatorType);
				DateTime pathedValue;
				switch(groupInterval) {
					case PivotGroupInterval.DateHour:
						pathedValue = PatchToHour((DateTime)value);
						return pathedValue.AddHours(GetAddition(currentOperatorType, pathedValue == (DateTime)value));
					case PivotGroupInterval.DateQuarterYear:
						pathedValue = PatchToQuarter((DateTime)value);
						return pathedValue.AddMonths(GetAddition(currentOperatorType, pathedValue == (DateTime)value) * 3);
					case PivotGroupInterval.DateMonthYear:
						pathedValue = PatchToMonth((DateTime)value);
						return pathedValue.AddMonths(GetAddition(currentOperatorType, pathedValue == (DateTime)value));
					case PivotGroupInterval.DateHourMinute:
						pathedValue = PatchToMinute((DateTime)value);
						return pathedValue.AddMinutes(GetAddition(currentOperatorType, pathedValue == (DateTime)value));
					case PivotGroupInterval.DateHourMinuteSecond:
						pathedValue = PatchToSecond((DateTime)value);
						return pathedValue.AddSeconds(GetAddition(currentOperatorType, pathedValue == (DateTime)value));
					case PivotGroupInterval.Date:
						pathedValue = PatchToDay((DateTime)value);
						return pathedValue.AddDays(GetAddition(currentOperatorType, pathedValue == (DateTime)value));
					case PivotGroupInterval.DateYear:
						return new DateTime(Convert.ToInt32(value) + GetAddition(currentOperatorType, true), 1, 1);
					default:
						newOperatorType = default(BinaryOperatorType);
						return null;
				}
			} catch {
				newOperatorType = default(BinaryOperatorType);
				return null;
			}
		}
		BinaryOperatorType GetNewOperatorType(BinaryOperatorType type) {
			if(type == BinaryOperatorType.Greater || type == BinaryOperatorType.GreaterOrEqual)
				return BinaryOperatorType.GreaterOrEqual;
			return BinaryOperatorType.Less;
		}
		int GetAddition(BinaryOperatorType type, bool isWholeValue) {
			if(type == BinaryOperatorType.Greater || type == BinaryOperatorType.LessOrEqual)
				return 1;
			if((type == BinaryOperatorType.Less || type == BinaryOperatorType.GreaterOrEqual) && !isWholeValue)
				return 1;
			return 0;
		}
		DateTime PatchToQuarter(DateTime value) {
			return new DateTime(value.Year, (value.Month - 1) / 3 * 3 + 1, 1);
		}
		DateTime PatchToMonth(DateTime value) {
			return new DateTime(value.Year, value.Month, 1);
		}
		DateTime PatchToDay(DateTime value) {
			return new DateTime(value.Year, value.Month, value.Day);
		}
		DateTime PatchToHour(DateTime value) {
			return new DateTime(value.Year, value.Month, value.Day, value.Hour, 0, 0);
		}
		DateTime PatchToMinute(DateTime value) {
			return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
		}
		DateTime PatchToSecond(DateTime value) {
			return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
		}
	}
}
