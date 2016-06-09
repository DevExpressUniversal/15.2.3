#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Linq;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.EF.Utils {
	public class CriteriaToEFSqlConverter : CriteriaToCStyleParameterlessProcessor {
		private ITypesInfo typesInfo;
		private ITypeInfo objectTypeInfo;
		private String currentTableName;
		private CriteriaToStringVisitResult ConvertCastFunction(FunctionOperator theOperator, Type type) {
			CriteriaToStringVisitResult result = null;
			if(theOperator.Operands.Count >= 1) {
				result = new CriteriaToStringVisitResult(String.Format("Cast({0} as {1})", Process(theOperator.Operands[0]).Result, type.FullName));
			}
			return result;
		}
		private Type GetListElementType(String memberName) {
			Type result = null;
			if(objectTypeInfo != null) {
				IMemberInfo memberInfo = objectTypeInfo.FindMember(memberName);
				if((memberInfo != null)) {
					result = memberInfo.ListElementType;
				}
			}
			return result;
		}
		private Boolean ConvertCustomFunctionToValue(FunctionOperator functionOperator, out Object value) {
			Boolean result = false;
			value = null;
			if((functionOperator.OperatorType == FunctionOperatorType.Custom)
					|| (functionOperator.OperatorType == FunctionOperatorType.CustomNonDeterministic)) {
				String customFunctionName = "";
				if(functionOperator.Operands[0] is OperandValue) {
					OperandValue operandValue = (OperandValue)functionOperator.Operands[0];
					if(operandValue.Value is String) {
						customFunctionName = (String)operandValue.Value;
					}
				}
				if(!String.IsNullOrWhiteSpace(customFunctionName)) {
					ICustomFunctionOperator customFunctionOperator = CriteriaOperator.GetCustomFunction(customFunctionName);
					if(customFunctionOperator != null) {
						value = customFunctionOperator.Evaluate(functionOperator.Operands);
						result = true;
					}
				}
			}
			return result;
		}
		protected override String GetBetweenText() {
			return base.GetBetweenText();
		}
		protected override String GetCustomFunctionText(String p) {
			return base.GetCustomFunctionText(p);
		}
		protected override String GetFunctionText(FunctionOperatorType operatorType) {
			switch(operatorType) {
				case FunctionOperatorType.Upper:
					return "ToUpper";
				case FunctionOperatorType.Lower:
					return "ToLower";
				case FunctionOperatorType.Len:
					return "Length";
				case FunctionOperatorType.GetHour:
					return "Hour";
				case FunctionOperatorType.GetDay:
					return "Day";
				case FunctionOperatorType.GetMonth:
					return "Month";
				case FunctionOperatorType.GetYear:
					return "Year";
				case FunctionOperatorType.GetSecond:
					return "Second";
				case FunctionOperatorType.GetMinute:
					return "Minute";
				case FunctionOperatorType.GetMilliSecond:
					return "Millisecond";
				case FunctionOperatorType.GetDayOfYear:
					return "DayOfYear";
				case FunctionOperatorType.Now:
					return "CurrentDateTime";
				case FunctionOperatorType.UtcNow:
					return "CurrentUtcDateTime";
				case FunctionOperatorType.DateDiffDay:
					return "DiffDays";
				case FunctionOperatorType.DateDiffHour:
					return "DiffHours";
				case FunctionOperatorType.DateDiffMilliSecond:
					return "DiffMilliseconds";
				case FunctionOperatorType.DateDiffMinute:
					return "DiffMinutes";
				case FunctionOperatorType.DateDiffMonth:
					return "DiffMonths";
				case FunctionOperatorType.DateDiffSecond:
					return "DiffSeconds";
				case FunctionOperatorType.DateDiffTick:
					return "DiffTicks";
				case FunctionOperatorType.DateDiffYear:
					return "DiffYears";
				case FunctionOperatorType.GetDate:
					return "TruncateTime";
				default:
					return base.GetFunctionText(operatorType);
			}
		}
		protected override String GetInText() {
			return base.GetInText();
		}
		protected override String GetIsNotNullText() {
			return base.GetIsNotNullText();
		}
		protected override String GetIsNullText() {
			return base.GetIsNullText();
		}
		protected override String GetNotLikeText() {
			return base.GetNotLikeText();
		}
		protected override String GetOperatorString(Aggregate operatorType) {
			return base.GetOperatorString(operatorType);
		}
		public CriteriaToEFSqlConverter(String currentTableName, ITypesInfo typesInfo, Type objectType)
			: base() {
			this.currentTableName = currentTableName;
			this.typesInfo = typesInfo;
			objectTypeInfo = typesInfo.FindTypeInfo(objectType);
		}
		public override String GetOperatorString(BinaryOperatorType operatorType) {
			return base.GetOperatorString(operatorType);
		}
		public override String GetOperatorString(GroupOperatorType operatorType) {
			return base.GetOperatorString(operatorType);
		}
		public override String GetOperatorString(UnaryOperatorType operatorType) {
			return base.GetOperatorString(operatorType);
		}
		public override CriteriaToStringVisitResult Visit(AggregateOperand operand) {
			CriteriaToStringVisitResult result = null;
			if(operand.AggregateType == Aggregate.Count) {
				if(Object.ReferenceEquals(operand.CollectionProperty, null)) {
					result = new CriteriaToStringVisitResult("Count(0)");
				}
				else {
					result = new CriteriaToStringVisitResult(String.Format("Count(select value 1 from {0})", Process(operand.CollectionProperty).Result));
				}
			}
			else if(operand.AggregateType == Aggregate.Sum) {
				if(Object.ReferenceEquals(operand.CollectionProperty, null)) {
					result = new CriteriaToStringVisitResult(String.Format("Sum({0})", Process(operand.AggregatedExpression).Result));
				}
				else {
					String collectionTableName = currentTableName + operand.CollectionProperty.PropertyName;
					CriteriaToEFSqlConverter criteriaToEFSqlConverter = new CriteriaToEFSqlConverter(collectionTableName, typesInfo, GetListElementType(operand.CollectionProperty.PropertyName));
					result = new CriteriaToStringVisitResult(String.Format("Sum(select value {0} from {1} as {2})",
						criteriaToEFSqlConverter.Convert(operand.AggregatedExpression),
						Process(operand.CollectionProperty).Result,
						collectionTableName));
				}
			}
			else if(operand.AggregateType == Aggregate.Exists) {
				String collectionTableName = currentTableName + operand.CollectionProperty.PropertyName;
				CriteriaToEFSqlConverter criteriaToEFSqlConverter = new CriteriaToEFSqlConverter(collectionTableName, typesInfo, GetListElementType(operand.CollectionProperty.PropertyName));
				result = new CriteriaToStringVisitResult(String.Format("Exists(select value 1 from {0} as {1} where {2})",
					Process(operand.CollectionProperty).Result,
					collectionTableName,
					criteriaToEFSqlConverter.Convert(operand.Condition)));
			}
			else {
				String resultString = ((CriteriaToStringVisitResult)base.Visit(operand)).Result;
				result = new CriteriaToStringVisitResult(resultString.TrimStart('[', ']', '.'));
			}
			return result;
		}
		public override CriteriaToStringVisitResult Visit(BetweenOperator theOperator) {
			return new CriteriaToStringVisitResult(
				"(" + Process(theOperator.TestExpression).Result +
				" between " +
				Process(theOperator.BeginExpression).Result +
				" and " +
				Process(theOperator.EndExpression).Result + ")");
		}
		public override CriteriaToStringVisitResult Visit(BinaryOperator theOperator) {
			return base.Visit(theOperator);
		}
		public override CriteriaToStringVisitResult Visit(FunctionOperator theOperator) {
			CriteriaToStringVisitResult result = null;
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.ToStr:
					result = ConvertCastFunction(theOperator, typeof(String));
					break;
				case FunctionOperatorType.ToDecimal:
					result = ConvertCastFunction(theOperator, typeof(Decimal));
					break;
				case FunctionOperatorType.ToDouble:
					result = ConvertCastFunction(theOperator, typeof(Double));
					break;
				case FunctionOperatorType.ToFloat:
					result = ConvertCastFunction(theOperator, typeof(Single));
					break;
				case FunctionOperatorType.ToInt:
					result = ConvertCastFunction(theOperator, typeof(Int32));
					break;
				case FunctionOperatorType.ToLong:
					result = ConvertCastFunction(theOperator, typeof(Int64));
					break;
				case FunctionOperatorType.Iif:
					if(theOperator.Operands.Count >= 3) {
						result = new CriteriaToStringVisitResult(
							String.Format("case when ({0}) then {1} else {2} end",
								Process(theOperator.Operands[0]).Result,
								Process(theOperator.Operands[1]).Result,
								Process(theOperator.Operands[2]).Result)
						);
					}
					else {
						result = base.Visit(theOperator);
					}
					break;
				case FunctionOperatorType.IsNullOrEmpty:
					result = new CriteriaToStringVisitResult(String.Format("{0} Is Null || {0} == ''", Process(theOperator.Operands[0]).Result));
					break;
				case FunctionOperatorType.Insert:
					CriteriaOperator insertCriteriaOperator = CriteriaOperator.Parse(String.Format("Concat(Concat(Substring({0}, 1, {1}), {2}), Substring({0}, {1}+1, Len({0})-{1}))", theOperator.Operands[0], theOperator.Operands[1], theOperator.Operands[2]));
					result = new CriteriaToStringVisitResult(Convert(insertCriteriaOperator));
					break;
				case FunctionOperatorType.Remove:
					if(theOperator.Operands.Count == 3) {
						CriteriaOperator removeCriteriaOperator = CriteriaOperator.Parse(String.Format("Concat(Substring({0}, 1, {1}), Substring({0}, {1} + {2} + 1, Length({0})-({1} + {2})))", theOperator.Operands[0], theOperator.Operands[1], theOperator.Operands[2]));
						result = new CriteriaToStringVisitResult(Convert(removeCriteriaOperator));
					}
					else {
						CriteriaOperator removeCriteriaOperator = CriteriaOperator.Parse(String.Format("Substring({0}, 1, {1})", theOperator.Operands[0], theOperator.Operands[1]));
						result = new CriteriaToStringVisitResult(Convert(removeCriteriaOperator));
					}
					break;
				case FunctionOperatorType.Today:
					result = new CriteriaToStringVisitResult("TruncateTime(CurrentDateTime())");
					break;
				case FunctionOperatorType.CharIndex:
					if(theOperator.Operands.Count == 2) {
						CriteriaOperator charIndexCriteriaOperator = CriteriaOperator.Parse(String.Format("IndexOf({0}, {1}) - 1", theOperator.Operands[0], theOperator.Operands[1]));
						result = new CriteriaToStringVisitResult(Convert(charIndexCriteriaOperator));
					}
					else if(theOperator.Operands.Count == 3) {
						CriteriaOperator charIndexCriteriaOperator = CriteriaOperator.Parse(String.Format("IndexOf({0}, Substring({1}, {2}, Length({1})-{2})) - 1", theOperator.Operands[0], theOperator.Operands[1], theOperator.Operands[2]));
						result = new CriteriaToStringVisitResult(Convert(charIndexCriteriaOperator));
					}
					else {
						CriteriaOperator charIndexCriteriaOperator = CriteriaOperator.Parse(String.Format("IndexOf({0}, Substring({1}, {2}, {3})) - 1", theOperator.Operands[0], theOperator.Operands[1], theOperator.Operands[2], theOperator.Operands[3]));
						result = new CriteriaToStringVisitResult(Convert(charIndexCriteriaOperator));
					}
					break;
				case FunctionOperatorType.LocalDateTimeThisYear:
				case FunctionOperatorType.LocalDateTimeThisMonth:
				case FunctionOperatorType.LocalDateTimeLastWeek:
				case FunctionOperatorType.LocalDateTimeThisWeek:
				case FunctionOperatorType.LocalDateTimeYesterday:
				case FunctionOperatorType.LocalDateTimeToday:
				case FunctionOperatorType.LocalDateTimeNow:
				case FunctionOperatorType.LocalDateTimeTomorrow:
				case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
				case FunctionOperatorType.LocalDateTimeNextWeek:
				case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
				case FunctionOperatorType.LocalDateTimeNextMonth:
				case FunctionOperatorType.LocalDateTimeNextYear:
					if(theOperator.Operands.Count != 0) {
						throw new ArgumentException("theOperator.Operands.Count != 0");
					}
					result = this.Process(new ConstantValue(EvalHelpers.EvaluateLocalDateTime(theOperator.OperatorType)));
					break;
				case FunctionOperatorType.IsThisYear:
				case FunctionOperatorType.IsThisWeek:
				case FunctionOperatorType.IsThisMonth:
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
				case FunctionOperatorType.IsOutlookIntervalToday:
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
				case FunctionOperatorType.IsOutlookIntervalYesterday:
					result = this.Process(EvalHelpers.ExpandIsOutlookInterval(theOperator));
					break;
				default:
					Object value = null;
					if(ConvertCustomFunctionToValue(theOperator, out value)) {
						result = new CriteriaToStringVisitResult(ValueToString(value));
					}
					else {
						result = base.Visit(theOperator);
					}
					break;
			}
			return result;
		}
		public override CriteriaToStringVisitResult Visit(GroupOperator theOperator) {
			return base.Visit(theOperator);
		}
		public override CriteriaToStringVisitResult Visit(InOperator theOperator) {
			return new CriteriaToStringVisitResult(Process(theOperator.LeftOperand).Result + " " + GetInText() + " {" + ProcessToCommaDelimitedList(theOperator.Operands) + "}");
		}
		public override CriteriaToStringVisitResult Visit(JoinOperand operand) {
			return base.Visit(operand);
		}
		public override CriteriaToStringVisitResult Visit(OperandProperty operand) {
			String result = operand.PropertyName;
			if(!String.IsNullOrWhiteSpace(operand.PropertyName)) {
				result = currentTableName + "." + operand.PropertyName;
			}
			return new CriteriaToStringVisitResult(result);
		}
		public override CriteriaToStringVisitResult Visit(OperandValue operand) {
			CriteriaToStringVisitResult result = null;
			if(operand is OperandParameter) {
				if(operand.Value is DateTime) {
					result = new CriteriaToStringVisitResult("DateTime'" + ((DateTime)operand.Value).ToString("yyyy-MM-dd HH:mm:ss.fffff") + "'");
				}
				else {
					result = new CriteriaToStringVisitResult(ValueToString(operand.Value));
				}
			}
			else if((operand is OperandValue) && (operand.Value is DateTime)) {
				result = new CriteriaToStringVisitResult("DateTime'" + ((DateTime)operand.Value).ToString("yyyy-MM-dd HH:mm:ss.fffff") + "'");
			}
			else if((operand is OperandValue) && (operand.Value != null) && operand.Value.GetType().IsEnum) {
				result = new CriteriaToStringVisitResult(
					String.Format("Cast({0} as {1})", ValueToString(System.Convert.ToInt64(operand.Value)), operand.Value.GetType().FullName));
			}
			else {
				result = base.Visit(operand);
			}
			return result;
		}
		public override CriteriaToStringVisitResult Visit(QueryOperand operand) {
			return base.Visit(operand);
		}
		public override CriteriaToStringVisitResult Visit(QuerySubQueryContainer operand) {
			return base.Visit(operand);
		}
		public override CriteriaToStringVisitResult Visit(UnaryOperator theOperator) {
			return base.Visit(theOperator);
		}
		public String Convert(CriteriaOperator expression) {
			String result = null;
			if(!Object.ReferenceEquals(expression, null)) {
				result = Process(expression).Result;
			}
			return result;
		}
		public String Convert(String expression) {
			return Convert(CriteriaOperator.Parse(expression));
		}
	}
}
