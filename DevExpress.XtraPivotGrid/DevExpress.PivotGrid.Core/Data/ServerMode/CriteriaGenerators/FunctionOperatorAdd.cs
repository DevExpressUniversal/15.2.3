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

using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.PivotGrid.ServerMode {
	public enum FunctionOperatorTypeAdd {
		GetQuarter,
		GetDateMonthYear,
		GetDateQuarterYear,
		GetDateHour,
		GetDateHourMinute,
		GetDateHourMinuteSecond,
		GetWeekOfYear
	}
	public class FunctionOperatorAdd : FunctionOperator {
		static CriteriaOperator CreateDefaultOperator(FunctionOperatorTypeAdd type, CriteriaOperator[] operands) {
			switch(type) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return new BinaryOperator(
						new FunctionOperator(
							FunctionOperatorType.Floor,
							new BinaryOperator(
								new BinaryOperator(
									new FunctionOperator(FunctionOperatorType.GetMonth, operands[0]),
									new ConstantValue(1),
									BinaryOperatorType.Minus
								),
								new ConstantValue(3),
								BinaryOperatorType.Divide)
						),
						new ConstantValue(1),
						BinaryOperatorType.Plus
					);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return ColumnCriteriaHelper.GetDateMonthYear(operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return new FunctionOperator(
						FunctionOperatorType.AddMonths,
						ColumnCriteriaHelper.GetDateMonthYear(operands[0]),
						new BinaryOperator(
							new BinaryOperator(
								new ConstantValue(1),
								new FunctionOperator(FunctionOperatorType.GetMonth, operands[0]),
								BinaryOperatorType.Minus
							),
							new ConstantValue(3),
							BinaryOperatorType.Modulo
						)
					);
				case FunctionOperatorTypeAdd.GetDateHour:
					return ColumnCriteriaHelper.GetDateHour(operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return ColumnCriteriaHelper.GetDateHourMinute(operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return new FunctionOperator(
						FunctionOperatorType.AddSeconds,
						ColumnCriteriaHelper.GetDateHourMinute(operands[0]),
						new FunctionOperator(FunctionOperatorType.GetSecond, operands[0])
					);
				case FunctionOperatorTypeAdd.GetWeekOfYear:
					return ColumnCriteriaHelper.GetWeekNumberExpression(
							new FunctionOperator(FunctionOperatorType.GetDayOfYear, operands[0]),
							operands[0]
						);
				default:
					return null;
			}
		}
		public static string GetFunctionName(FunctionOperatorTypeAdd type) {
			return string.Format("{0}CustomFunctionAdd", type.ToString());
		}
		public static CriteriaOperator Create(bool isServerCriteria, FunctionOperatorTypeAdd type, params CriteriaOperator[] operands) {
			if(!isServerCriteria)
				return CreateDefaultOperator(type, operands);
			string customFunctionName = GetFunctionName(type);
			if(CriteriaOperator.GetCustomFunction(customFunctionName) != null)
				return new FunctionOperatorAdd(type, operands);
			return CreateDefaultOperator(type, operands);
		}
		readonly FunctionOperatorTypeAdd operatorTypeAdd;
		FunctionOperatorTypeAdd OperatorTypeAdd { get { return operatorTypeAdd; } }
		FunctionOperatorAdd(FunctionOperatorTypeAdd operatorTypeAdd, params CriteriaOperator[] operands)
			: base (GetFunctionName(operatorTypeAdd), operands){
			this.operatorTypeAdd = operatorTypeAdd;
		}
		public override void Accept(ICriteriaVisitor visitor) {
			visitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			var baseResult = visitor.Visit(this);
			if(baseResult == null && visitor is BaseSqlGenerator) {
				CriteriaOperator defaultOperator = CreateDefaultOperator(OperatorTypeAdd, Operands.Skip(1).ToArray());
				return defaultOperator.Accept(visitor);
			}
			return baseResult;
		}
	}
}
