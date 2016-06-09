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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
using System.Reflection;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Filtering.Helpers {
	using DevExpress.Compatibility.System;
	using DevExpress.Data.Filtering;
	public interface IFilteredDataSource {
		CriteriaOperator Filter { get; set; }
	}
	public interface IFilteredXtraBindingList : IBindingList, IFilteredDataSource {		
	}
	public struct CriteriaTypeResolverResult {
		readonly Type type;
		public Type Type {
			get { return type; }
		}
		readonly object tag;
		public object Tag {
			get { return tag; }
		}
		public CriteriaTypeResolverResult(Type type)
			: this(type, null) {
		}
		public CriteriaTypeResolverResult(Type type, object tag){
			this.type = type;
			this.tag = tag;
		}
	}
	public abstract class CriteriaTypeResolverBase : ICriteriaVisitor<CriteriaTypeResolverResult> {
		CriteriaTypeResolverResult ICriteriaVisitor<CriteriaTypeResolverResult>.Visit(BetweenOperator theOperator) {
			return new CriteriaTypeResolverResult(typeof(bool));
		}
		CriteriaTypeResolverResult ICriteriaVisitor<CriteriaTypeResolverResult>.Visit(BinaryOperator theOperator) {
			switch(theOperator.OperatorType) {
				case BinaryOperatorType.Minus:
				case BinaryOperatorType.Modulo:
				case BinaryOperatorType.Multiply:
				case BinaryOperatorType.Divide:
				case BinaryOperatorType.Plus:
					return new CriteriaTypeResolverResult(GetBinaryPromotionType(Process(theOperator.LeftOperand).Type, Process(theOperator.RightOperand).Type, theOperator.OperatorType));
				case BinaryOperatorType.Equal:
				case BinaryOperatorType.Greater:
				case BinaryOperatorType.GreaterOrEqual:
				case BinaryOperatorType.Less:
				case BinaryOperatorType.LessOrEqual:
				case BinaryOperatorType.NotEqual:
					return new CriteriaTypeResolverResult(typeof(bool));
#pragma warning disable 618
				case BinaryOperatorType.Like:
#pragma warning restore 618
					return Process(LikeCustomFunction.Convert(theOperator));
			}
			return new CriteriaTypeResolverResult(typeof(object));
		}
		static Type GetBinaryPromotionType(Type left, Type right, BinaryOperatorType exceptionType) {
			switch(DXTypeExtensions.GetTypeCode(left)) {
				case TypeCode.Byte:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return GetTypeFromCode(EvalHelpers.GetBinaryNumericPromotionCode(left, right, exceptionType, true));
				default:
					return left;
			}
		}
		public static Type GetTypeFromCode(TypeCode typeCode) { 
			switch(typeCode) {
				case TypeCode.Boolean:
					return typeof(bool);
				case TypeCode.Byte:
					return typeof(byte);
				case TypeCode.Char:
					return typeof(char);
				case TypeCode.DateTime:
					return typeof(DateTime);
				case DXTypeExtensions.TypeCodeDBNull:
					return typeof(DBNull);
				case TypeCode.Decimal:
					return typeof(decimal);
				case TypeCode.Double:
					return typeof(double);
				case TypeCode.Int16:
					return typeof(Int16);
				case TypeCode.Int32:
					return typeof(Int32);
				case TypeCode.Int64:
					return typeof(Int64);
				case TypeCode.SByte:
					return typeof(sbyte);
				case TypeCode.Single:
					return typeof(Single);
				case TypeCode.String:
					return typeof(string);
				case TypeCode.UInt16:
					return typeof(UInt16);
				case TypeCode.UInt32:
					return typeof(UInt32);
				case TypeCode.UInt64:
					return typeof(UInt64);
			}
			return typeof(object);
		}
		CriteriaTypeResolverResult ICriteriaVisitor<CriteriaTypeResolverResult>.Visit(UnaryOperator theOperator) {
			switch(theOperator.OperatorType) {
				case UnaryOperatorType.IsNull:
				case UnaryOperatorType.Not:
					return new CriteriaTypeResolverResult(typeof(bool));
				default:
					return Process(theOperator.Operand);
			}
		}
		CriteriaTypeResolverResult ICriteriaVisitor<CriteriaTypeResolverResult>.Visit(InOperator theOperator) {
			return new CriteriaTypeResolverResult(typeof(bool));
		}
		CriteriaTypeResolverResult ICriteriaVisitor<CriteriaTypeResolverResult>.Visit(GroupOperator theOperator) {
			return new CriteriaTypeResolverResult(typeof(bool));
		}
		CriteriaTypeResolverResult ICriteriaVisitor<CriteriaTypeResolverResult>.Visit(OperandValue theOperand) {
			if(theOperand.Value == null)
				return new CriteriaTypeResolverResult(typeof(object));
			else
				return new CriteriaTypeResolverResult(theOperand.Value.GetType());
		}
		CriteriaTypeResolverResult ICriteriaVisitor<CriteriaTypeResolverResult>.Visit(FunctionOperator theOperator) {
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.Iif:
					return FnIif(theOperator);
				case FunctionOperatorType.IsNullOrEmpty:
				case FunctionOperatorType.StartsWith:
				case FunctionOperatorType.EndsWith:
				case FunctionOperatorType.Contains:
					return new CriteriaTypeResolverResult(typeof(bool));
				case FunctionOperatorType.IsNull:
					if(theOperator.Operands.Count > 1)
						return Process((CriteriaOperator)theOperator.Operands[0]);
					else
						return new CriteriaTypeResolverResult(typeof(bool));
				case FunctionOperatorType.Sign:
					return new CriteriaTypeResolverResult(typeof(int));
				case FunctionOperatorType.Abs:
				case FunctionOperatorType.Ceiling:
				case FunctionOperatorType.Floor:
				case FunctionOperatorType.Round:
					return Process((CriteriaOperator)theOperator.Operands[0]);
				case FunctionOperatorType.Len:
				case FunctionOperatorType.Ascii:
				case FunctionOperatorType.CharIndex:
				case FunctionOperatorType.GetDay:
				case FunctionOperatorType.GetDayOfYear:
				case FunctionOperatorType.GetHour:
				case FunctionOperatorType.GetMilliSecond:
				case FunctionOperatorType.GetMinute:
				case FunctionOperatorType.GetMonth:
				case FunctionOperatorType.GetSecond:
				case FunctionOperatorType.GetYear:
				case FunctionOperatorType.GetDayOfWeek:
				case FunctionOperatorType.DateDiffDay:
				case FunctionOperatorType.DateDiffHour:
				case FunctionOperatorType.DateDiffMilliSecond:
				case FunctionOperatorType.DateDiffMinute:
				case FunctionOperatorType.DateDiffMonth:
				case FunctionOperatorType.DateDiffSecond:
				case FunctionOperatorType.DateDiffTick:
				case FunctionOperatorType.DateDiffYear:
				case FunctionOperatorType.ToInt:
					return new CriteriaTypeResolverResult(typeof(int));
				case FunctionOperatorType.Substring:
				case FunctionOperatorType.Trim:
				case FunctionOperatorType.Concat:
				case FunctionOperatorType.Upper:
				case FunctionOperatorType.Lower:
				case FunctionOperatorType.Remove:
				case FunctionOperatorType.Replace:
				case FunctionOperatorType.Reverse:
				case FunctionOperatorType.Insert:
				case FunctionOperatorType.ToStr:
				case FunctionOperatorType.PadLeft:
				case FunctionOperatorType.PadRight:
					return new CriteriaTypeResolverResult(typeof(string));
				case FunctionOperatorType.Char:
					return new CriteriaTypeResolverResult(typeof(char));
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
				case FunctionOperatorType.AddDays:
				case FunctionOperatorType.AddHours:
				case FunctionOperatorType.AddMilliSeconds:
				case FunctionOperatorType.AddMinutes:
				case FunctionOperatorType.AddMonths:
				case FunctionOperatorType.AddSeconds:
				case FunctionOperatorType.AddTicks:
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddYears:
				case FunctionOperatorType.UtcNow:
				case FunctionOperatorType.Now:
				case FunctionOperatorType.Today:
				case FunctionOperatorType.GetDate:
					return new CriteriaTypeResolverResult(typeof(DateTime));
				case FunctionOperatorType.GetTimeOfDay:
				case FunctionOperatorType.BigMul:
				case FunctionOperatorType.ToLong:
					return new CriteriaTypeResolverResult(typeof(long));
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
				case FunctionOperatorType.IsOutlookIntervalToday:
				case FunctionOperatorType.IsOutlookIntervalYesterday:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
				case FunctionOperatorType.IsThisMonth:
				case FunctionOperatorType.IsThisWeek:
				case FunctionOperatorType.IsThisYear:
					return new CriteriaTypeResolverResult(typeof(bool));
				case FunctionOperatorType.Power:
				case FunctionOperatorType.Exp:
				case FunctionOperatorType.Cos:
				case FunctionOperatorType.Atn:
				case FunctionOperatorType.Tan:
				case FunctionOperatorType.Sin:
				case FunctionOperatorType.Rnd:
				case FunctionOperatorType.Log:
				case FunctionOperatorType.Log10:
				case FunctionOperatorType.Sqr:
				case FunctionOperatorType.Acos:
				case FunctionOperatorType.Asin:
				case FunctionOperatorType.Atn2:
				case FunctionOperatorType.Cosh:
				case FunctionOperatorType.Sinh:
				case FunctionOperatorType.Tanh:
				case FunctionOperatorType.ToDouble:
					return new CriteriaTypeResolverResult(typeof(double));
				case FunctionOperatorType.CustomNonDeterministic:
				case FunctionOperatorType.Custom:
					return new CriteriaTypeResolverResult(FnCustom(theOperator));
				case FunctionOperatorType.ToFloat:
					return new CriteriaTypeResolverResult(typeof(float));
				case FunctionOperatorType.ToDecimal:
					return new CriteriaTypeResolverResult(typeof(decimal));
				default:
					return new CriteriaTypeResolverResult(typeof(object));
			}
		}
		public CriteriaTypeResolverResult FnIif(FunctionOperator theOperator) {
			var operands = theOperator.Operands;
			if(operands.Count < 3 || operands.Count % 2 != 1)
				throw new InvalidOperationException("Incorrect arguments count: " + theOperator.ToString());
			Type right = FnIifProcess(operands[operands.Count - 1]);
			for(int i = operands.Count - 2; i > 0; i -= 2) {
				Type left = FnIifProcess(operands[i]);
				if(left == typeof(string)) {
					if(right != typeof(string))
						right = left;
					continue;
				}
				if(right == typeof(string)) {
					continue;
				}
				if(right == null) {
					if(left.IsValueType() && Nullable.GetUnderlyingType(left) == null)
						left = typeof(Nullable<>).MakeGenericType(left);
					right = left;
					continue;
				}
				if(left == null) {
					if(right.IsValueType() && Nullable.GetUnderlyingType(right) == null)
						right = typeof(Nullable<>).MakeGenericType(right);
					continue;
				}
				if(left == typeof(object)) {
					if(right != typeof(object))
						right = left;
					continue;
				}
				if(right == typeof(object)) {
					continue;
				}
				Type lType = Nullable.GetUnderlyingType(left) ?? left;
				Type rType = Nullable.GetUnderlyingType(right) ?? right;
				Type resultType;
				if(lType.IsAssignableFrom(rType)) {
					resultType = lType;
				} else if(rType.IsAssignableFrom(lType)) {
					resultType = rType;
				} else {
					resultType = EvalHelpers.GetBinaryNumericPromotionType(rType, lType);
				}
				if(resultType == typeof(object))
					throw new InvalidOperationException("Incompatible arguments types");
				if(resultType.IsValueType() && (Nullable.GetUnderlyingType(left) != null || Nullable.GetUnderlyingType(right) != null))
					resultType = typeof(Nullable<>).MakeGenericType(resultType);
				right = resultType;
			}
			return new CriteriaTypeResolverResult(right);
		}
		Type FnIifProcess(CriteriaOperator operand) {
			CriteriaTypeResolverResult result = Process(operand);
			if(result.Type == typeof(object) && operand is OperandValue && ((OperandValue)operand).Value == null) {
				return null;
			}
			return result.Type;
		}
		Type FnCustom(FunctionOperator theOperator) {
			if (!(theOperator.Operands[0] is OperandValue) || (((OperandValue)theOperator.Operands[0]).Value == null) || !(((OperandValue)theOperator.Operands[0]).Value is string)) {
				return typeof(object);
			}
			string functionName = (string)((OperandValue)theOperator.Operands[0]).Value;
			if (theOperator.Operands.Count > 1) {
				Type[] operands = new Type[theOperator.Operands.Count - 1];
				for (int i = 1; i < theOperator.Operands.Count; i++) {
					operands[i - 1] = Process((CriteriaOperator)theOperator.Operands[i]).Type;
				}
				return GetCustomFunctionType(functionName, operands);
			}
			return GetCustomFunctionType(functionName);
		}
		protected virtual Type GetCustomFunctionType(string functionName, params Type[] operands) {
			if(CriteriaOperator.CustomFunctionCount > 0) {
				ICustomFunctionOperator customFunction = CriteriaOperator.GetCustomFunction(functionName);
				if(customFunction != null) {
					return customFunction.ResultType(operands);
				}
			}
			return typeof(object);
		}
		protected CriteriaTypeResolverResult Process(CriteriaOperator criteria) {
			if(ReferenceEquals(criteria, null))
				return new CriteriaTypeResolverResult(typeof(object), null);
			return criteria.Accept(this);
		}
	}
	public class CriteriaToStringVisitResult {
		const string NullCriteriaResult = "()";
		public readonly string Result;
		public readonly CriteriaPriorityClass Priority;
		public CriteriaToStringVisitResult(string result, CriteriaPriorityClass priorityClass) {
			this.Result = result;
			this.Priority = priorityClass;
		}
		public CriteriaToStringVisitResult(string result) : this(result, CriteriaPriorityClass.Atom) { }
		public bool IsNull { get { return Result == NullCriteriaResult; } }
		public static readonly CriteriaToStringVisitResult Null = new CriteriaToStringVisitResult(NullCriteriaResult, CriteriaPriorityClass.Atom);
		public string GetEnclosedResult() {
			return '(' + Result + ')';
		}
		public string GetEnclosedResultOnGreaterOrEqual(CriteriaPriorityClass basePriority) {
			if(this.Priority >= basePriority)
				return GetEnclosedResult();
			else
				return Result;
		}
		public string GetEnclosedResultOnGreater(CriteriaPriorityClass basePriority) {
			if(this.Priority > basePriority)
				return GetEnclosedResult();
			else
				return Result;
		}
	}
	public enum CriteriaPriorityClass { Atom, Neg, Mul, Add, BinaryNot, BinaryAnd, BinaryXor, BinaryOr, InBetween, CmpGt, CmpEq, IsNull, Not, And, Or }
	public abstract partial class CriteriaToStringBase : IClientCriteriaVisitor<CriteriaToStringVisitResult> {
		protected string ProcessToCommaDelimitedList(ICollection operands) {
			StringBuilder operandsList = new StringBuilder();
			foreach(CriteriaOperator op in operands) {
				if(operandsList.Length > 0)
					operandsList.Append(", ");
				operandsList.Append(Process(op).Result);
			}
			return operandsList.ToString();
		}
		protected virtual string GetBetweenText() {
			return "Between";
		}
		public virtual CriteriaToStringVisitResult Visit(BetweenOperator operand) {
			CriteriaToStringVisitResult visitResult = Process(operand.TestExpression);
			string result = visitResult.GetEnclosedResultOnGreaterOrEqual(CriteriaPriorityClass.InBetween);
			result += " " + GetBetweenText() + "(";
			result += ProcessToCommaDelimitedList(new CriteriaOperator[] { operand.BeginExpression, operand.EndExpression });
			result += ')';
			return new CriteriaToStringVisitResult(result, CriteriaPriorityClass.InBetween);
		}
		public abstract string GetOperatorString(BinaryOperatorType opType);
		public virtual CriteriaToStringVisitResult Visit(BinaryOperator operand) {
			string operatorString = GetOperatorString(operand.OperatorType);
			CriteriaPriorityClass priority;
			switch(operand.OperatorType) {
				default:
					throw new InvalidOperationException();
				case BinaryOperatorType.Divide:
				case BinaryOperatorType.Multiply:
				case BinaryOperatorType.Modulo:
					priority = CriteriaPriorityClass.Mul;
					break;
				case BinaryOperatorType.Plus:
				case BinaryOperatorType.Minus:
					priority = CriteriaPriorityClass.Add;
					break;
				case BinaryOperatorType.BitwiseAnd:
					priority = CriteriaPriorityClass.BinaryAnd;
					break;
				case BinaryOperatorType.BitwiseXor:
					priority = CriteriaPriorityClass.BinaryXor;
					break;
				case BinaryOperatorType.BitwiseOr:
					priority = CriteriaPriorityClass.BinaryOr;
					break;
				case BinaryOperatorType.Greater:
				case BinaryOperatorType.GreaterOrEqual:
				case BinaryOperatorType.Less:
				case BinaryOperatorType.LessOrEqual:
					priority = CriteriaPriorityClass.CmpGt;
					break;
				case BinaryOperatorType.Equal:
				case BinaryOperatorType.NotEqual:
#pragma warning disable 618
				case BinaryOperatorType.Like:
#pragma warning restore 618
					priority = CriteriaPriorityClass.CmpEq;
					break;
			}
			CriteriaToStringVisitResult leftVisitResult = Process(operand.LeftOperand);
			CriteriaToStringVisitResult rightVisitResult = Process(operand.RightOperand);
			string result = leftVisitResult.GetEnclosedResultOnGreater(priority);
			result += ' ';
			result += operatorString;
			result += ' ';
			result += rightVisitResult.GetEnclosedResultOnGreaterOrEqual(priority);
			return new CriteriaToStringVisitResult(result, priority);
		}
		public abstract string GetOperatorString(UnaryOperatorType opType);
		protected virtual string GetIsNotNullText() {
			return "Is Not Null";
		}
		CriteriaToStringVisitResult CreateIsNotNull(UnaryOperator nullOp) {
			CriteriaToStringVisitResult innerResult = Process(nullOp.Operand);
			string result = innerResult.GetEnclosedResultOnGreaterOrEqual(CriteriaPriorityClass.IsNull);
			result += " " + GetIsNotNullText();
			return new CriteriaToStringVisitResult(result, CriteriaPriorityClass.IsNull);
		}
		protected virtual string GetNotLikeText() {
			return "Not Like";
		}
		CriteriaToStringVisitResult CreateNotLike(BinaryOperator likeOp) {
			CriteriaToStringVisitResult leftResult = Process(likeOp.LeftOperand);
			CriteriaToStringVisitResult rightResult = Process(likeOp.RightOperand);
			string result = leftResult.GetEnclosedResultOnGreaterOrEqual(CriteriaPriorityClass.CmpEq);
			result += " " + GetNotLikeText() + " ";
			result += rightResult.GetEnclosedResultOnGreaterOrEqual(CriteriaPriorityClass.CmpEq);
			return new CriteriaToStringVisitResult(result, CriteriaPriorityClass.CmpEq);
		}
		public virtual CriteriaToStringVisitResult Visit(UnaryOperator operand) {
			if(operand.OperatorType == UnaryOperatorType.Not) {
				UnaryOperator nullOp = operand.Operand as UnaryOperator;
				if(!ReferenceEquals(nullOp, null) && nullOp.OperatorType == UnaryOperatorType.IsNull)
					return CreateIsNotNull(nullOp);
				BinaryOperator likeOp = operand.Operand as BinaryOperator;
#pragma warning disable 618
				if(!ReferenceEquals(likeOp, null) && likeOp.OperatorType == BinaryOperatorType.Like)
					return CreateNotLike(likeOp);
#pragma warning restore 618
				FunctionOperator fnLikeOp = operand.Operand as FunctionOperator;
				if(LikeCustomFunction.IsBinaryCompatibleLikeFunction(fnLikeOp))
					return Process(!LikeCustomFunction.Convert(fnLikeOp));
			}
			CriteriaToStringVisitResult innerResult = Process(operand.Operand);
			CriteriaPriorityClass priority;
			switch(operand.OperatorType) {
				default:
					throw new InvalidOperationException();
				case UnaryOperatorType.BitwiseNot:
					priority = CriteriaPriorityClass.BinaryNot;
					break;
				case UnaryOperatorType.IsNull:
					priority = CriteriaPriorityClass.IsNull;
					break;
				case UnaryOperatorType.Minus:
				case UnaryOperatorType.Plus:
					priority = CriteriaPriorityClass.Neg;
					break;
				case UnaryOperatorType.Not:
					priority = CriteriaPriorityClass.Not;
					break;
			}
			string result = innerResult.GetEnclosedResultOnGreater(priority);
			if(operand.OperatorType == UnaryOperatorType.IsNull) {
				result += " " + GetIsNullText();
			} else {
				string operatorString = GetOperatorString(operand.OperatorType);
				result = operatorString + ' ' + result;
			}
			return new CriteriaToStringVisitResult(result, priority);
		}
		protected virtual string GetIsNullText() {
			return "Is Null";
		}
		protected virtual string GetInText() {
			return "In";
		}
		public virtual CriteriaToStringVisitResult Visit(InOperator operand) {
			CriteriaToStringVisitResult result = Process(operand.LeftOperand);
			string strRes = result.GetEnclosedResultOnGreaterOrEqual(CriteriaPriorityClass.InBetween);
			strRes += " " + GetInText() + " (";
			strRes += ProcessToCommaDelimitedList(operand.Operands);
			strRes += ')';
			return new CriteriaToStringVisitResult(strRes, CriteriaPriorityClass.InBetween);
		}
		public abstract string GetOperatorString(GroupOperatorType opType);
		public virtual CriteriaToStringVisitResult Visit(GroupOperator operand) {
			switch(operand.Operands.Count) {
				case 0:
					return CriteriaToStringVisitResult.Null;
				case 1:
					return Process((CriteriaOperator)operand.Operands[0]);
			}
			string delimiter = ' ' + GetOperatorString(operand.OperatorType) + ' ';
			CriteriaPriorityClass basePriority;
			switch(operand.OperatorType) {
				case GroupOperatorType.And:
					basePriority = CriteriaPriorityClass.And;
					break;
				case GroupOperatorType.Or:
					basePriority = CriteriaPriorityClass.Or;
					break;
				default:
					throw new InvalidOperationException();
			}
			CriteriaToStringVisitResult currentResult = Process((CriteriaOperator)operand.Operands[0]);
			StringBuilder result = new StringBuilder(currentResult.GetEnclosedResultOnGreater(basePriority));
			for(int i = 1; i < operand.Operands.Count; ++i) {
				result.Append(delimiter);
				currentResult = Process((CriteriaOperator)operand.Operands[i]);
				result.Append(currentResult.GetEnclosedResultOnGreater(basePriority));
			}
			return new CriteriaToStringVisitResult(result.ToString(), basePriority);
		}
		public abstract CriteriaToStringVisitResult Visit(OperandValue operand);
		protected virtual string GetFunctionText(FunctionOperatorType operandType) {
			return operandType.ToString();
		}
		public virtual CriteriaToStringVisitResult Visit(FunctionOperator operand) {
			if(operand.OperatorType == FunctionOperatorType.Custom) {
				if(LikeCustomFunction.IsBinaryCompatibleLikeFunction(operand))
					return Process(LikeCustomFunction.Convert(operand));
				string fnName = GetCustomFunctionText((string)((OperandValue)operand.Operands[0]).Value);
				string result = CriteriaLexer.IsGoodUnescapedName(fnName) ? fnName : new OperandProperty(fnName).ToString();
				result += '(';
				List<CriteriaOperator> ps = new List<CriteriaOperator>();
				ps.AddRange(operand.Operands);
				ps.RemoveAt(0);
				result += ProcessToCommaDelimitedList(ps);
				result += ')';
				return new CriteriaToStringVisitResult(result);
			} else {
				string result = GetFunctionText(operand.OperatorType);
				result += '(';
				result += ProcessToCommaDelimitedList(operand.Operands);
				result += ')';
				return new CriteriaToStringVisitResult(result);
			}
		}
		protected virtual string GetCustomFunctionText(string p) {
			return p;
		}
		protected virtual string GetOperatorString(Aggregate operandType) {
			return operandType.ToString();
		}
		public virtual CriteriaToStringVisitResult Visit(AggregateOperand operand) {
			OperandProperty toStringProperty = operand.CollectionProperty;
			if(ReferenceEquals(toStringProperty, null))
				toStringProperty = new OperandProperty();
			string result = Process(toStringProperty).Result;
			result += '[';
			CriteriaToStringVisitResult operandVisitResult = Process(operand.Condition);
			if(!operandVisitResult.IsNull)
				result += operandVisitResult.Result;
			result += ']';
			if(operand.AggregateType != Aggregate.Exists || !ReferenceEquals(operand.AggregatedExpression, null)) {
				if(result == "[][]") {
					result = string.Empty;
				} else {
					result += '.';
				}
				result += GetOperatorString(operand.AggregateType);
				result += '(';
				CriteriaToStringVisitResult aggregatedPropertyVisitResult = Process(operand.AggregatedExpression);
				if(!aggregatedPropertyVisitResult.IsNull)
					result += aggregatedPropertyVisitResult.Result;
				result += ')';
			}
			return new CriteriaToStringVisitResult(result);
		}
		public virtual CriteriaToStringVisitResult Visit(JoinOperand operand) {
			OperandProperty toStringProperty = new OperandProperty('<' + operand.JoinTypeName + '>');
			string result = Process(toStringProperty).Result;
			result += '[';
			CriteriaToStringVisitResult operandVisitResult = Process(operand.Condition);
			if(!operandVisitResult.IsNull)
				result += operandVisitResult.Result;
			result += ']';
			if(operand.AggregateType != Aggregate.Exists || !ReferenceEquals(operand.AggregatedExpression, null)) {
				result += '.';
				result += GetOperatorString(operand.AggregateType);
				result += '(';
				CriteriaToStringVisitResult aggregatedPropertyVisitResult = Process(operand.AggregatedExpression);
				if(!aggregatedPropertyVisitResult.IsNull)
					result += aggregatedPropertyVisitResult.Result;
				result += ')';
			}
			return new CriteriaToStringVisitResult(result);
		}
		public virtual CriteriaToStringVisitResult Visit(OperandProperty operand) {
			string result = operand.PropertyName;
			if(result == null)
				result = string.Empty;
			result = "[" + result.Replace("\\", "\\\\").Replace("]", "\\]").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t") + "]";
			return new CriteriaToStringVisitResult(result);
		}
		protected CriteriaToStringVisitResult Process(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null))
				return CriteriaToStringVisitResult.Null;
			else
				return operand.Accept(this);
		}
	}
	public abstract class CriteriaToStringWithParametersProcessor : CriteriaToStringBase {
		public const string ParameterPrefix = "?";
		protected readonly List<OperandValue> Parameters;
		protected CriteriaToStringWithParametersProcessor() {
			this.Parameters = new List<OperandValue>();
		}
		public override CriteriaToStringVisitResult Visit(OperandValue operand) {
			Parameters.Add(operand);
			OperandParameter param = operand as OperandParameter;
			if(!ReferenceEquals(param, null))
				return new CriteriaToStringVisitResult(ParameterPrefix + param.ParameterName);
			else
				return new CriteriaToStringVisitResult(ParameterPrefix);
		}
	}
	public abstract class CriteriaToStringParameterlessProcessor : CriteriaToStringBase {
		const string nullString = "null";	
		public static string ValueToString(object value) {
			return ValueToString(value, false);
		}
		public static string ValueToString(object value, bool isLegacy) {
			if(value == null)
				return nullString;
			TypeCode tc = DXTypeExtensions.GetTypeCode(value.GetType());
			switch(tc) {
				case DXTypeExtensions.TypeCodeDBNull:
				case TypeCode.Empty:
					return nullString;
				case TypeCode.Boolean:
					return ((bool)value) ? "True" : "False";
				case TypeCode.Char:
					return "'" + (char)value + (isLegacy ? "'" : "'c");
				case TypeCode.DateTime:
					DateTime datetimeValue = (DateTime)value;
					string dateTimeFormatPattern;
					if(datetimeValue.TimeOfDay == TimeSpan.Zero) {
						dateTimeFormatPattern = "yyyy-MM-dd";
					} else if(datetimeValue.Millisecond == 0) {
						dateTimeFormatPattern = "yyyy-MM-dd HH:mm:ss";
					} else {
						dateTimeFormatPattern = "yyyy-MM-dd HH:mm:ss.fffff";
					}
					return "#" + ((DateTime)value).ToString(dateTimeFormatPattern, CultureInfo.InvariantCulture).Replace("#", "##") + "#";
				case TypeCode.String:
					return "'" + ((string)value).Replace("'", "''") + "'";
				case TypeCode.Decimal:
					return FixNonFixedText(((Decimal)value).ToString(CultureInfo.InvariantCulture), isLegacy, tc);
				case TypeCode.Double:
					return FixNonFixedText(((Double)value).ToString("r", CultureInfo.InvariantCulture), isLegacy, tc);
				case TypeCode.Single:
					return FixNonFixedText(((Single)value).ToString("r", CultureInfo.InvariantCulture), isLegacy, tc);
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
					if(value is Enum)
						return UserTypeToString(value, isLegacy);
					if(isLegacy)
						return value.ToString();
					else
						return value.ToString() + GetSuffix(tc);
				case TypeCode.Object:
				default:
					if(value is Guid) {
						return "{" + ((Guid)value).ToString() + "}";
					} else if(value is TimeSpan) {
						return "#" + ((TimeSpan)value).ToString().Replace("#", "##") + "#";
					} else {
						return UserTypeToString(value, isLegacy);
					}
			}
		}
		static string UserTypeToString(object value, bool isLegacy) {
			if(isLegacy) {
				return "'" + (value.ToString() ?? string.Empty).Replace("'", "''") + "'";
			}
			UserValueProcessingEventArgs e = CriteriaOperator.DoUserValueToString(value);
			EnumProcessingHelper.ToStringEnumIfNeeded(e);
			if(!e.Handled) {
				e.Tag = CriteriaOperator.TagToString;
				e.Data = value.ToString();
				e.Handled = true;
			}
			if(e.Data == null)
				e.Data = string.Empty;
			if(string.IsNullOrEmpty(e.Tag))
				throw new ArgumentNullException("e.Tag");
			if(e.Tag.Contains("#"))
				throw new ArgumentException("e.Tag should not contains '#': (" + e.Tag + ")");
			return string.Format("##{0}#{1}#", e.Tag.Replace("#", "##"), e.Data.Replace("#", "##"));
		}
#if SL
		class GPTypeConverter : TypeConverter {
			protected Type targetType;
			internal GPTypeConverter(Type targetType) {
				this.targetType = targetType;
			}
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
			}
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));
			}
			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
				if(this.targetType == null) {
					throw new InvalidOperationException("No targetType");
				}
				string str = value as string;
				if(str == null) {
					throw new ArgumentException("value: string expected");
				}
				if(this.targetType.IsEnum) {
					return Enum.Parse(this.targetType, str, true);
				}
				return str;
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if(destinationType != typeof(string)) {
					return base.ConvertTo(context, culture, value, destinationType);
				}
				if(value == null) {
					return string.Empty;
				}
				return value.ToString();
			}
		}
#endif
		internal static TypeConverter GetTypeConverter(Type type) {
#if SL
			object[] customAttributes = type.GetCustomAttributes(typeof(TypeConverterAttribute), true);
			if((customAttributes == null) || (customAttributes.Length <= 0)) {
				return new GPTypeConverter(type);
			} else {
				return Activator.CreateInstance(Type.GetType(((TypeConverterAttribute)customAttributes[0]).ConverterTypeName)) as TypeConverter;
			}
#else
#if CF
			return null;
#else
			return TypeDescriptor.GetConverter(type);
#endif
#endif
		}
		internal static TypeConverter GetTypeConverter(object value) {
			if(value == null)
				throw new ArgumentNullException("value");
#if SL || CF || DXPORTABLE
			return GetTypeConverter(value.GetType());
#else
			return TypeDescriptor.GetConverter(value);
#endif
		}
		public static string OperandValueOrParameterToString(OperandValue val, bool isLegacy) {
			if(ReferenceEquals(val, null))
				throw new ArgumentNullException("val");
			OperandParameter p = val as OperandParameter;
			if(!ReferenceEquals(p, null))
				return CriteriaToStringWithParametersProcessor.ParameterPrefix + p.ParameterName;
			if(val.Value == null && !(val is ConstantValue))
				return CriteriaToStringWithParametersProcessor.ParameterPrefix;
			return ValueToString(val.Value, isLegacy);
		}
		static string FixNonFixedText(string toFix, bool isLegacy, TypeCode tc) {
			if(toFix.IndexOfAny(new char[] { '.', 'e', 'E' }) < 0)
				toFix += ".0";
			if(!isLegacy)
				toFix += GetSuffix(tc);
			return toFix;
		}
		static string GetSuffix(TypeCode tc) {
			switch(tc) {
				case TypeCode.Decimal:
					return "m";
				case TypeCode.Single:
					return "f";
				case TypeCode.Byte:
					return "b";
				case TypeCode.SByte:
					return "sb";
				case TypeCode.Int16:
					return "s";
				case TypeCode.UInt16:
					return "us";
				case TypeCode.UInt32:
					return "u";
				case TypeCode.Int64:
					return "L";
				case TypeCode.UInt64:
					return "uL";
				default:
					return string.Empty;
			}
		}
		public static CriteriaToStringVisitResult ValueToCriteriaToStringVisitResult(OperandValue operand) {
			return new CriteriaToStringVisitResult(OperandValueOrParameterToString(operand, false));
		}
		public override CriteriaToStringVisitResult Visit(OperandValue operand) {
			return ValueToCriteriaToStringVisitResult(operand);
		}
	}
	public class CriteriaToBasicStyleParameterlessProcessor : CriteriaToStringParameterlessProcessor {
		protected static CriteriaToBasicStyleParameterlessProcessor Instance = new CriteriaToBasicStyleParameterlessProcessor();
		protected CriteriaToBasicStyleParameterlessProcessor() { }
		public static string GetBasicOperatorString(BinaryOperatorType opType) {
			switch(opType) {
				default:
					throw new InvalidOperationException();
				case BinaryOperatorType.BitwiseAnd:
					return "&";
				case BinaryOperatorType.BitwiseOr:
					return "|";
				case BinaryOperatorType.BitwiseXor:
					return "^";
				case BinaryOperatorType.Divide:
					return "/";
				case BinaryOperatorType.Equal:
					return "=";
				case BinaryOperatorType.Greater:
					return ">";
				case BinaryOperatorType.GreaterOrEqual:
					return ">=";
				case BinaryOperatorType.Less:
					return "<";
				case BinaryOperatorType.LessOrEqual:
					return "<=";
#pragma warning disable 618
				case BinaryOperatorType.Like:
#pragma warning restore 618
					return "Like";
				case BinaryOperatorType.Minus:
					return "-";
				case BinaryOperatorType.Modulo:
					return "%";
				case BinaryOperatorType.Multiply:
					return "*";
				case BinaryOperatorType.NotEqual:
					return "<>";
				case BinaryOperatorType.Plus:
					return "+";
			}
		}
		public static string GetBasicOperatorString(UnaryOperatorType opType) {
			switch(opType) {
				case UnaryOperatorType.IsNull:
				default:
					throw new InvalidOperationException();
				case UnaryOperatorType.BitwiseNot:
					return "~";
				case UnaryOperatorType.Minus:
					return "-";
				case UnaryOperatorType.Not:
					return "Not";
				case UnaryOperatorType.Plus:
					return "+";
			}
		}
		public static string GetBasicOperatorString(GroupOperatorType opType) {
			switch(opType) {
				default:
					throw new InvalidOperationException();
				case GroupOperatorType.And:
					return "And";
				case GroupOperatorType.Or:
					return "Or";
			}
		}
		public override string GetOperatorString(UnaryOperatorType opType) {
			return GetBasicOperatorString(opType);
		}
		public override string GetOperatorString(BinaryOperatorType opType) {
			return GetBasicOperatorString(opType);
		}
		public override string GetOperatorString(GroupOperatorType opType) {
			return GetBasicOperatorString(opType);
		}
		public static string ToString(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null))
				return string.Empty;
			return Instance.Process(operand).Result;
		}
	}
	public class CriteriaToStringLegacyProcessor : CriteriaToBasicStyleParameterlessProcessor {
		new protected static CriteriaToStringLegacyProcessor Instance = new CriteriaToStringLegacyProcessor();
		protected CriteriaToStringLegacyProcessor() { }
		public override CriteriaToStringVisitResult Visit(OperandValue operand) {
			return new CriteriaToStringVisitResult(OperandValueOrParameterToString(operand, true));
		}
		new public static string ToString(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null))
				return string.Empty;
			return Instance.Process(operand).Result;
		}
	}
	public class CriteriaToCStyleParameterlessProcessor : CriteriaToStringParameterlessProcessor {
		protected static CriteriaToCStyleParameterlessProcessor Instance = new CriteriaToCStyleParameterlessProcessor();
		protected CriteriaToCStyleParameterlessProcessor() { }
		public static string GetCOperatorString(BinaryOperatorType opType) {
			switch(opType) {
				default:
					throw new InvalidOperationException();
				case BinaryOperatorType.BitwiseAnd:
					return "&";
				case BinaryOperatorType.BitwiseOr:
					return "|";
				case BinaryOperatorType.BitwiseXor:
					return "^";
				case BinaryOperatorType.Divide:
					return "/";
				case BinaryOperatorType.Equal:
					return "==";
				case BinaryOperatorType.Greater:
					return ">";
				case BinaryOperatorType.GreaterOrEqual:
					return ">=";
				case BinaryOperatorType.Less:
					return "<";
				case BinaryOperatorType.LessOrEqual:
					return "<=";
#pragma warning disable 618
				case BinaryOperatorType.Like:
#pragma warning restore 618
					return "Like";
				case BinaryOperatorType.Minus:
					return "-";
				case BinaryOperatorType.Modulo:
					return "%";
				case BinaryOperatorType.Multiply:
					return "*";
				case BinaryOperatorType.NotEqual:
					return "!=";
				case BinaryOperatorType.Plus:
					return "+";
			}
		}
		public static string GetCOperatorString(UnaryOperatorType opType) {
			switch(opType) {
				case UnaryOperatorType.IsNull:
				default:
					throw new InvalidOperationException();
				case UnaryOperatorType.BitwiseNot:
					return "~";
				case UnaryOperatorType.Minus:
					return "-";
				case UnaryOperatorType.Not:
					return "!";
				case UnaryOperatorType.Plus:
					return "+";
			}
		}
		public static string GetCOperatorString(GroupOperatorType opType) {
			switch(opType) {
				default:
					throw new InvalidOperationException();
				case GroupOperatorType.And:
					return "&&";
				case GroupOperatorType.Or:
					return "||";
			}
		}
		public override string GetOperatorString(UnaryOperatorType opType) {
			return GetCOperatorString(opType);
		}
		public override string GetOperatorString(BinaryOperatorType opType) {
			return GetCOperatorString(opType);
		}
		public override string GetOperatorString(GroupOperatorType opType) {
			return GetCOperatorString(opType);
		}
		public static string ToString(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null))
				return string.Empty;
			return Instance.Process(operand).Result;
		}
	}
	public class CriteriaToBasicStyleWithParametersProcessor : CriteriaToStringWithParametersProcessor {
		protected CriteriaToBasicStyleWithParametersProcessor() : base() { }
		public override string GetOperatorString(UnaryOperatorType opType) {
			return CriteriaToBasicStyleParameterlessProcessor.GetBasicOperatorString(opType);
		}
		public override string GetOperatorString(BinaryOperatorType opType) {
			return CriteriaToBasicStyleParameterlessProcessor.GetBasicOperatorString(opType);
		}
		public override string GetOperatorString(GroupOperatorType opType) {
			return CriteriaToBasicStyleParameterlessProcessor.GetBasicOperatorString(opType);
		}
		public static string ToString(CriteriaOperator criteria, out OperandValue[] parameters) {
			if(ReferenceEquals(criteria, null)) {
				parameters = new OperandValue[] { };
				return string.Empty;
			}
			CriteriaToBasicStyleWithParametersProcessor processor = new CriteriaToBasicStyleWithParametersProcessor();
			CriteriaToStringVisitResult visitResult = processor.Process(criteria);
			parameters = processor.Parameters.ToArray();
			return visitResult.Result;
		}
	}
	public class CriteriaToCStyleWithParametersProcessor : CriteriaToStringWithParametersProcessor {
		protected CriteriaToCStyleWithParametersProcessor() : base() { }
		public override string GetOperatorString(UnaryOperatorType opType) {
			return CriteriaToCStyleParameterlessProcessor.GetCOperatorString(opType);
		}
		public override string GetOperatorString(BinaryOperatorType opType) {
			return CriteriaToCStyleParameterlessProcessor.GetCOperatorString(opType);
		}
		public override string GetOperatorString(GroupOperatorType opType) {
			return CriteriaToCStyleParameterlessProcessor.GetCOperatorString(opType);
		}
		public static string ToString(CriteriaOperator criteria, out OperandValue[] parameters) {
			if(ReferenceEquals(criteria, null)) {
				parameters = new OperandValue[] { };
				return string.Empty;
			}
			CriteriaToCStyleWithParametersProcessor processor = new CriteriaToCStyleWithParametersProcessor();
			CriteriaToStringVisitResult visitResult = processor.Process(criteria);
			parameters = processor.Parameters.ToArray();
			return visitResult.Result;
		}
	}
	public class LikeCustomFunction: ICustomFunctionOperatorEvaluatableWithCaseSensitivity,
#if !DXPORTABLE
		ICustomFunctionOperatorConvertibleToExpression,
#endif
		ICustomFunctionOperatorCompileableWithCaseSensitivity, ICustomFunctionOperatorFormattable {
		public const string Name = "Like";
		public static FunctionOperator Create(CriteriaOperator value, CriteriaOperator pattern) {
			return new FunctionOperator(Name, value, pattern);
		}
		public static bool IsName(string nm) {
			return StringExtensions.CompareInvariantCultureIgnoreCase(nm, Name) == 0;
		}
		public static FunctionOperator Convert(BinaryOperator like) {
#pragma warning disable 618
			if(like.OperatorType != BinaryOperatorType.Like)
				throw new ArgumentException("like.OperatorType != BinaryOperatorType.Like");
#pragma warning restore 618
			return Create(like.LeftOperand, like.RightOperand);
		}
		public static BinaryOperator Convert(FunctionOperator like) {
			if(!IsBinaryCompatibleLikeFunction(like))
				throw new ArgumentException("!IsBinaryCompatibleLikeFunction(like)");
#pragma warning disable 618
			return new BinaryOperator(like.Operands[1], like.Operands[2], BinaryOperatorType.Like);
#pragma warning restore 618
		}
		public static bool IsBinaryCompatibleLikeFunction(FunctionOperator func) {
			if(ReferenceEquals(func, null))
				return false;
			if(func.OperatorType != FunctionOperatorType.Custom)
				return false;
			if(func.Operands.Count != 3)
				return false;
			var nmv = func.Operands[0] as OperandValue;
			if(ReferenceEquals(nmv, null))
				return false;
			var nm = nmv.Value as string;
			return IsName(nm);
		}
#if !DXPORTABLE
		static Expression MakeLinq(Linq.ICriteriaToExpressionConverter converter, Expression vExpr, Expression pExpr) {
			if(pExpr.NodeType == ExpressionType.Constant) {
				object boxedVal = ((ConstantExpression)pExpr).Value;
				string pattern = boxedVal != null ? boxedVal.ToString() : null;
				if(pattern == null)
					return Expression.Constant(false);
				string body = pattern;
				bool lastPercent = body.EndsWith("%");
				if(lastPercent)
					body = body.Substring(0, body.Length - 1);
				bool firstPercent = body.StartsWith("%");
				if(firstPercent)
					body = body.Substring(1);
				if(!body.Contains('%') && !body.Contains('_') && !body.Contains('[') && (lastPercent || firstPercent)) {
					var stringExpr = EvalHelpers.SafeToString(vExpr);
					{
						var maybeIifConverter = converter as Linq.Helpers.CriteriaToExpressionConverterInternal;
						if(maybeIifConverter != null && maybeIifConverter.ForceIifForInstance) {
							Expression<Func<string, bool>> method;
							if(!firstPercent)
								method = s => s != null ? s.StartsWith(body) : false;
							else if(!lastPercent)
								method = s => s != null ? s.EndsWith(body) : false;
							else
								method = s => s != null ? s.Contains(body) : false;
							return Expression.Invoke(Expression.Constant(method), stringExpr);
						}
					}
					string methodName;
					if(!firstPercent)
						methodName = "StartsWith";
					else if(!lastPercent)
						methodName = "EndsWith";
					else
						methodName = "Contains";
					return Expression.Call(stringExpr, methodName, null, Expression.Constant(body));
				}
			}
			return Expression.Call(typeof(System.Data.Linq.SqlClient.SqlMethods), "Like", null, EvalHelpers.SafeToString(vExpr), EvalHelpers.SafeToString(pExpr));
		}
		Expression MakeEf(Expression vExpr, Expression pExpr) {
			var patIndexCall = Expression.Call(DevExpress.Data.Linq.Helpers.CriteriaToEFExpressionConverterInternal.SqlFunctionsType, "PatIndex", new Type[0], pExpr, vExpr);
			return Expression.GreaterThan(Expression.Convert(patIndexCall, typeof(int)), Expression.Constant(0));
		}
		Expression ICustomFunctionOperatorConvertibleToExpression.Convert(Linq.ICriteriaToExpressionConverter converter, Expression[] operands) {
			if(operands.Length != 2)
				throw new ArgumentException("operands.Length != 2");
			if(converter is DevExpress.Data.Linq.Helpers.CriteriaToEFExpressionConverterInternal || converter is DevExpress.Data.Linq.CriteriaToEFExpressionConverter)
				return MakeEf(operands[0], operands[1]);
			else
				return MakeLinq(converter, operands[0], operands[1]);
		}
#endif
		Expression ICustomFunctionOperatorCompileableWithCaseSensitivity.Create(bool caseSensitive, Expression[] operands) {
			return MakeCompile(caseSensitive, operands);
		}
		Expression ICustomFunctionOperatorCompileable.Create(Expression[] operands) {
			return MakeCompile(false, operands);
		}
		static Expression MakeCompile(bool caseSensitive, Expression[] operands) {
			if(operands.Length != 2)
				throw new ArgumentException("operands.Length != 2");
			Expression vExpr = operands[0];
			Expression pExpr = operands[1];
			if(pExpr.NodeType == ExpressionType.Constant) {
				object boxedVal = ((ConstantExpression)pExpr).Value;
				string pattern = boxedVal != null ? boxedVal.ToString() : null;
				if(pattern == null) {
					return Expression.Constant(false);
				} else {
					var likeData = LikeDataCache.Get(pattern, caseSensitive);
					Func<string, bool> body = x => likeData(x);
					return Expression.Invoke(Expression.Constant(body), EvalHelpers.SafeToString(vExpr));
				}
			} else {
				Func<string, string, bool> body = (x, y) => {
					if(y == null)
						return false;
					else
						return LikeDataCache.Get(y, caseSensitive)(x);
				};
				return Expression.Invoke(Expression.Constant(body), EvalHelpers.SafeToString(vExpr), EvalHelpers.SafeToString(pExpr));
			}
		}
		string ICustomFunctionOperator.Name {
			get { return Name; }
		}
		Type ICustomFunctionOperator.ResultType(params Type[] operands) {
			return typeof(bool);
		}
		object ICustomFunctionOperator.Evaluate(params object[] operands) {
			return EvaluateCore(false, operands);
		}
		object ICustomFunctionOperatorEvaluatableWithCaseSensitivity.Evaluate(bool caseSensitive, params object[] operands) {
			return EvaluateCore(caseSensitive, operands);
		}
		static object EvaluateCore(bool caseSensitive, object[] operands) {
			if(operands.Length != 2)
				throw new ArgumentException("operands.Length != 3");
			var left = operands[0];
			var right = operands[1];
			if(left == null || right == null)
				return false;
			return ExpressionEvaluatorCoreBase.GetBool(LikeDataCache.Get(right.ToString(), caseSensitive)(left.ToString()));
		}
		static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, bool> IsPostgreSqlConnectionProviderCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, bool>();
		static bool IsPostgreSqlConnectionProviderCore(Type type) {
			for(Type t = type; t != null; t = t.GetBaseType()) {
				if(t.FullName == "DevExpress.Xpo.DB.PostgreSqlConnectionProvider")
					return true;
			}
			return false;
		}
		static bool IsPostgreSqlConnectionProvider(Type type) {
			if(type == null)
				return false;
			return IsPostgreSqlConnectionProviderCache.GetOrAdd(type, t => IsPostgreSqlConnectionProviderCore(t));
		}
		string ICustomFunctionOperatorFormattable.Format(Type providerType, params string[] operands) {
			if(operands.Length != 2)
				throw new ArgumentException("operands.Length != 2");
			if(IsPostgreSqlConnectionProvider(providerType))
				return string.Format(CultureInfo.InvariantCulture, "({0} LIKE {1} ESCAPE '')", operands[0], operands[1]);
			return string.Format(CultureInfo.InvariantCulture, "({0} like {1})", operands[0], operands[1]);
		}
	}
}
namespace DevExpress.Data.Filtering {
	using System.Collections.Generic;
	using System.Text;
	using DevExpress.Data.Filtering.Helpers;
	using System.Reflection;
	using System.Linq.Expressions;
	using DevExpress.Compatibility.System;
	using DevExpress.Compatibility.System.Xml.Serialization;
	public interface ICriteriaVisitor {
		void Visit(BetweenOperator theOperator);
		void Visit(BinaryOperator theOperator);
		void Visit(UnaryOperator theOperator);
		void Visit(InOperator theOperator);
		void Visit(GroupOperator theOperator);
		void Visit(OperandValue theOperand);
		void Visit(FunctionOperator theOperator);
	}
	public interface ICriteriaVisitor<T> {
		T Visit(BetweenOperator theOperator);
		T Visit(BinaryOperator theOperator);
		T Visit(UnaryOperator theOperator);
		T Visit(InOperator theOperator);
		T Visit(GroupOperator theOperator);
		T Visit(OperandValue theOperand);
		T Visit(FunctionOperator theOperator);
	}
	public interface IClientCriteriaVisitor: ICriteriaVisitor {
		void Visit(AggregateOperand theOperand);
		void Visit(OperandProperty theOperand);
		void Visit(JoinOperand theOperand);
	}
	public interface IClientCriteriaVisitor<T>: ICriteriaVisitor<T> {
		T Visit(AggregateOperand theOperand);
		T Visit(OperandProperty theOperand);
		T Visit(JoinOperand theOperand);
	}
	[Serializable]
	public sealed class CriteriaOperatorCollection : List<CriteriaOperator> {
		public CriteriaOperatorCollection() { }
		public CriteriaOperatorCollection(int capacity) : base(capacity) { }
		public override bool Equals(object obj) {
			CriteriaOperatorCollection another = obj as CriteriaOperatorCollection;
			if(another == null)
				return false;
			if(this.Count != another.Count)
				return false;
			for(int i = 0; i < this.Count; ++i) {
				if(!Equals(this[i], another[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int result = 0;
			foreach(object o in this) {
				result ^= o.GetHashCode();
			}
			return result;
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder();
			foreach(CriteriaOperator op in this) {
				if(result.Length > 0)
					result.Append("; ");
				result.Append(CriteriaOperator.ToString(op));
			}
			return result.ToString();
		}
	}
	[Serializable]
	[XmlInclude(typeof(ContainsOperator))]
	[XmlInclude(typeof(BetweenOperator))]
	[XmlInclude(typeof(BinaryOperator))]
	[XmlInclude(typeof(UnaryOperator))]
	[XmlInclude(typeof(InOperator))]
	[XmlInclude(typeof(GroupOperator))]
	[XmlInclude(typeof(OperandValue))]
	[XmlInclude(typeof(ConstantValue))]
	[XmlInclude(typeof(OperandProperty))]
	[XmlInclude(typeof(AggregateOperand))]
	[XmlInclude(typeof(JoinOperand))]
	[XmlInclude(typeof(FunctionOperator))]
	[XmlInclude(typeof(NotOperator))]
	[XmlInclude(typeof(NullOperator))]
	public abstract partial class CriteriaOperator : ICloneable {
		public override bool Equals(object obj) {
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public abstract void Accept(ICriteriaVisitor visitor);
		public abstract T Accept<T>(ICriteriaVisitor<T> visitor);
		public static CriteriaOperator Parse(string stringCriteria, out OperandValue[] criteriaParametersList) {
			return CriteriaParser.Parse(stringCriteria, out criteriaParametersList);
		}
		public static CriteriaOperator Parse(string criteria, params object[] parameters) {
			OperandValue[] criteriaParametersList;
			CriteriaOperator result = Parse(criteria, out criteriaParametersList);
			if(parameters != null && criteriaParametersList != null) {
				for(int i = 0; i < Math.Min(parameters.Length, criteriaParametersList.Length); i++)
					criteriaParametersList[i].Value = parameters[i];
			}
			return result;
		}
		public static CriteriaOperator TryParse(string criteria, params object[] parameters) {
			try {
				return Parse(criteria, parameters);
			} catch {
				return null;
			}
		}
		public static CriteriaOperator[] ParseList(string criteriaList, out OperandValue[] criteriaParametersList) {
			return CriteriaParser.ParseList(criteriaList, out criteriaParametersList, false);
		}
		public static CriteriaOperator[] ParseList(string criteriaList, params object[] parameters) {
			OperandValue[] criteriaParametersList;
			CriteriaOperator[] result = ParseList(criteriaList, out criteriaParametersList);
			if(parameters != null && criteriaParametersList != null) {
				for(int i = 0; i < Math.Min(parameters.Length, criteriaParametersList.Length); i++)
					criteriaParametersList[i].Value = parameters[i];
			}
			return result;
		}
		public static string ToBasicStyleString(CriteriaOperator criteria) {
			return CriteriaToBasicStyleParameterlessProcessor.ToString(criteria);
		}
		public static string ToCStyleString(CriteriaOperator criteria) {
			return CriteriaToCStyleParameterlessProcessor.ToString(criteria);
		}
		public static string ToBasicStyleString(CriteriaOperator criteria, out OperandValue[] criteriaParametersList) {
			return CriteriaToBasicStyleWithParametersProcessor.ToString(criteria, out criteriaParametersList);
		}
		public static string ToCStyleString(CriteriaOperator criteria, out OperandValue[] criteriaParametersList) {
			return CriteriaToCStyleWithParametersProcessor.ToString(criteria, out criteriaParametersList);
		}
		public static string LegacyToString(CriteriaOperator criteria) {
			return CriteriaToStringLegacyProcessor.ToString(criteria);
		}
		public static string ToString(CriteriaOperator criteria) {
			return ToBasicStyleString(criteria);
		}
		public static string ToString(CriteriaOperator criteria, out OperandValue[] criteriaParametersList) {
			return ToBasicStyleString(criteria, out criteriaParametersList);
		}
		public override string ToString() {
			return ToString(this);
		}
		public string LegacyToString() { return LegacyToString(this); }
		public static CriteriaOperator And(CriteriaOperator left, CriteriaOperator right) {
			return GroupOperator.Combine(GroupOperatorType.And, left, right);
		}
		public static CriteriaOperator Or(CriteriaOperator left, CriteriaOperator right) {
			return GroupOperator.Combine(GroupOperatorType.Or, left, right);
		}
		public static CriteriaOperator And(params CriteriaOperator[] operands) {
			return GroupOperator.Combine(GroupOperatorType.And, operands);
		}
		public static CriteriaOperator And(IEnumerable<CriteriaOperator> operands) {
			return GroupOperator.Combine(GroupOperatorType.And, operands);
		}
		public static CriteriaOperator Or(params CriteriaOperator[] operands) {
			return GroupOperator.Combine(GroupOperatorType.Or, operands);
		}
		public static CriteriaOperator Or(IEnumerable<CriteriaOperator> operands) {
			return GroupOperator.Combine(GroupOperatorType.Or, operands);
		}
		const string operatorTrueFalseObsoleteText =
			"Please replace == operator with ReferenceEquals, != with !ReferenceEquals (or use | and & operators instead of || and && in the simplified criteria syntax)"; 
		[Obsolete(operatorTrueFalseObsoleteText)]
		public static bool operator true(CriteriaOperator operand) {
			BinaryOperator binOp = operand as BinaryOperator;
			if(ReferenceEquals(binOp, null))
				throw new InvalidOperationException(operatorTrueFalseObsoleteText);
			switch(binOp.OperatorType) {
				case BinaryOperatorType.Equal:
					return ReferenceEquals(binOp.LeftOperand, binOp.RightOperand);
				case BinaryOperatorType.NotEqual:
					return !ReferenceEquals(binOp.LeftOperand, binOp.RightOperand);
				default:
					throw new InvalidOperationException(operatorTrueFalseObsoleteText);
			}
		}
		[Obsolete(operatorTrueFalseObsoleteText, true)]
		public static bool operator false(CriteriaOperator operand) {
			if(operand)
				return false;
			else
				return true;
		}
		public static BinaryOperator operator ==(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Equal);
		}
		public static BinaryOperator operator !=(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.NotEqual);
		}
		public static BinaryOperator operator >(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Greater);
		}
		public static BinaryOperator operator <(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Less);
		}
		public static BinaryOperator operator >=(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.GreaterOrEqual);
		}
		public static BinaryOperator operator <=(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.LessOrEqual);
		}
		public static BinaryOperator operator +(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Plus);
		}
		public static BinaryOperator operator -(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Minus);
		}
		public static BinaryOperator operator *(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Multiply);
		}
		public static BinaryOperator operator /(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Divide);
		}
		public static BinaryOperator operator %(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Modulo);
		}
		public static CriteriaOperator operator &(CriteriaOperator left, CriteriaOperator right) {
			return And(left, right);
		}
		public static CriteriaOperator operator |(CriteriaOperator left, CriteriaOperator right) {
			return Or(left, right);
		}
		public static UnaryOperator operator +(CriteriaOperator operand) {
			return new UnaryOperator(UnaryOperatorType.Plus, operand);
		}
		public static UnaryOperator operator -(CriteriaOperator operand) {
			return new UnaryOperator(UnaryOperatorType.Minus, operand);
		}
		public static UnaryOperator operator !(CriteriaOperator operand) {
			return new UnaryOperator(UnaryOperatorType.Not, operand);
		}
		public UnaryOperator IsNull() {
			return new UnaryOperator(UnaryOperatorType.IsNull, this);
		}
		public UnaryOperator IsNotNull() {
			return !this.IsNull();
		}
		public UnaryOperator Not() {
			return !this;
		}
		public static explicit operator CriteriaOperator(Boolean val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(Byte val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(Char val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(Decimal val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(Double val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(Single val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(Int16 val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(Int32 val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(Int64 val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(Guid val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(String val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(DateTime val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(TimeSpan val) {
			return (OperandValue)val;
		}
		public static implicit operator CriteriaOperator(Byte[] val) {
			return (OperandValue)val;
		}
		object ICloneable.Clone() {
			return CloneCommon();
		}
		protected abstract CriteriaOperator CloneCommon();
		public static CriteriaOperator Clone(CriteriaOperator origin) {
			if(ReferenceEquals(origin, null))
				return null;
			return origin.CloneCommon();
		}
		public static OperandProperty Clone(OperandProperty origin) {
			if(ReferenceEquals(origin, null))
				return null;
			return origin.Clone();
		}
		protected static ICollection<CriteriaOperator> Clone(ICollection origins) {
			List<CriteriaOperator> result = new List<CriteriaOperator>(origins.Count);
			foreach(CriteriaOperator op in origins)
				result.Add(Clone(op));
			return result;
		}
		protected static CriteriaOperator ObjectToCriteriaSafe(object o) {
			CriteriaOperator co = o as CriteriaOperator;
			if(ReferenceEquals(co, null))
				co = new OperandValue(o);
			return co;
		}
#region CustomFunctionsEverywhere
		static CustomFunctionCollection commonCustomFunctionCollection = new CustomFunctionCollection() { new LikeCustomFunction() };
		public static void RegisterCustomFunction(ICustomFunctionOperator customFunction) {
			lock(commonCustomFunctionCollection) {
				commonCustomFunctionCollection.Add(customFunction);
			}
		}
		public static void RegisterCustomFunctions(IEnumerable<ICustomFunctionOperator> customFunctions) {
			lock(commonCustomFunctionCollection) {
				commonCustomFunctionCollection.Add(customFunctions);
			}
		}
		public static bool UnregisterCustomFunction(string functionName) {
			lock(commonCustomFunctionCollection) {
				ICustomFunctionOperator customFunction = commonCustomFunctionCollection.GetCustomFunction(functionName);
				if(customFunction == null) return false;
				return commonCustomFunctionCollection.Remove(customFunction);
			}
		}
		public static bool UnregisterCustomFunction(ICustomFunctionOperator customFunction) {
			lock(commonCustomFunctionCollection) {
				return commonCustomFunctionCollection.Remove(customFunction);
			}
		}
		public static int CustomFunctionCount {
			get {
				return commonCustomFunctionCollection.Count;
			}
		}
		public static ICustomFunctionOperator GetCustomFunction(string functionName) {
			lock(commonCustomFunctionCollection) {
				return commonCustomFunctionCollection.GetCustomFunction(functionName);
			}
		}
		public static CustomFunctionCollection GetCustomFunctions() {
			CustomFunctionCollection resultCollection = new CustomFunctionCollection();
			lock(commonCustomFunctionCollection) {
				resultCollection.Add(commonCustomFunctionCollection);
			}
			return resultCollection;
		}
#endregion CustomFunctionsEverywhere
		public static event EventHandler<UserValueProcessingEventArgs> UserValueToString;
		public static event EventHandler<UserValueProcessingEventArgs> UserValueParse;
		internal static UserValueProcessingEventArgs DoUserValueToString(object value) {
			UserValueProcessingEventArgs e = new UserValueProcessingEventArgs();
			e.Value = value;
			EventHandler<UserValueProcessingEventArgs> userValueToString = UserValueToString;
			if(userValueToString != null)
				userValueToString(null, e);
			return e;
		}
		internal static UserValueProcessingEventArgs DoUserValueParse(string tag, string data) {
			UserValueProcessingEventArgs e = new UserValueProcessingEventArgs();
			e.Tag = tag;
			e.Data = data;
			EventHandler<UserValueProcessingEventArgs> userValueParse = UserValueParse;
			if(userValueParse != null)
				userValueParse(null, e);
			return e;
		}
		internal const string TagToString = "ToString";
		internal const string TagEnum = "Enum";
		public static bool CriterionEquals(CriteriaOperator left, CriteriaOperator right) {
			if(ReferenceEquals(left, right)) return true;
			if(ReferenceEquals(left, null)) return false;
			return left.Equals(right);
		}
	}
	[Serializable]
	public class OperandProperty : CriteriaOperator {
		string propertyName;
		public override void Accept(ICriteriaVisitor visitor) {
			IClientCriteriaVisitor clientVisitor = (IClientCriteriaVisitor)visitor;
			clientVisitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			var clientVisitor = (IClientCriteriaVisitor<T>)visitor;
			return clientVisitor.Visit(this);
		}
		public OperandProperty() : this(string.Empty) { }
		public OperandProperty(string propertyName) {
			this.PropertyName = propertyName;
		}
		[XmlAttribute]
		public string PropertyName {
			get { return propertyName; }
			set { propertyName = value; }
		}
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			if(!object.ReferenceEquals(this.GetType(), obj.GetType()))
				return false;
			return object.Equals(this.PropertyName, ((OperandProperty)obj).PropertyName);
		}
		public override int GetHashCode() {
			return PropertyName != null ? PropertyName.GetHashCode() : -1;
		}
		public AggregateOperand this[CriteriaOperator condition] {
			get {
				return new AggregateOperand(this, null, Aggregate.Exists, condition);
			}
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public OperandProperty Clone() {
			return new OperandProperty(this.PropertyName);
		}
	}
	[XmlType("DBNull")]
	public class NullValue {
		public static NullValue Value = new NullValue();
	}
	[Serializable]
	public class ConstantValue : OperandValue {
		public ConstantValue(object value) : base(value) { }
		public ConstantValue() : base() { }
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public new ConstantValue Clone() {
			ICloneable cloneableValue = Value as ICloneable;
			if(cloneableValue != null)
				return new ConstantValue(cloneableValue.Clone());
			return new ConstantValue(Value);
		}
	}
	[Serializable]
	public class OperandValue : CriteriaOperator {
		object value;
		public OperandValue(object value) {
			this.value = value;
		}
		public OperandValue() : this(null) { }
		public override void Accept(ICriteriaVisitor visitor) {
			visitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		internal static string FormatString(string value) {
			int count = value.Length;
			StringBuilder res = new StringBuilder(value.Length);
			for(int i = 0; i < count; i++) {
				char c = value[i];
				switch(c) {
					case '\n': res.Append("\\n"); break;
					case '\r': res.Append("\\r"); break;
					case ' ': res.Append("\\s"); break;
					case '\\': res.Append("\\\\"); break;
					case '\0': res.Append("\\0"); break;
					default: res.Append(c); break;
				}
			}
			return res.ToString();
		}
		internal static string ReformatString(string value) {
			int count = value.Length;
			StringBuilder res = new StringBuilder(value.Length);
			for(int i = 0; i < count; i++) {
				char c = value[i];
				if(c == '\\') {
					i++;
					if(i < count) {
						c = value[i];
						switch(c) {
							case 'n': res.Append('\n'); break;
							case 'r': res.Append('\r'); break;
							case 's': res.Append(' '); break;
							case '\\': res.Append('\\'); break;
							case '0': res.Append('\0'); break;
							default: res.Append(c); break;
						}
					}
				} else {
					res.Append(c);
				}
			}
			return res.ToString();
		}
		[XmlElement(typeof(System.Boolean))]
		[XmlElement(typeof(System.Byte))]
		[XmlElement(typeof(System.SByte))]
		[XmlElement(typeof(System.Char))]
		[XmlElement(typeof(System.Decimal))]
		[XmlElement(typeof(System.Double))]
		[XmlElement(typeof(System.Single))]
		[XmlElement(typeof(System.Int32))]
		[XmlElement(typeof(System.UInt32))]
		[XmlElement(typeof(System.Int64))]
		[XmlElement(typeof(System.UInt64))]
		[XmlElement(typeof(System.Int16))]
		[XmlElement(typeof(System.UInt16))]
		[XmlElement(typeof(System.Guid))]
		[XmlElement(typeof(String))]
		[XmlElement(typeof(DateTime))]
		[XmlElement(typeof(TimeSpan))]
		[XmlElement(typeof(NullValue))]
		[XmlElement(typeof(System.Byte[]))]
		public virtual object XmlValue {
			get {
				object value = GetXmlValue();
				if(value == null)
					return NullValue.Value;
				string stringVal = value as string;
				return stringVal != null ? FormatString(stringVal) : value;
			}
			set {
				if(value is NullValue)
					Value = null;
				else {
					string stringVal = value as string;
					Value = stringVal != null ? ReformatString(stringVal) : value;
				}
			}
		}
		protected virtual object GetXmlValue() {
			return Value;
		}
		[XmlIgnore]
		public virtual object Value {
			get { return this.value; }
			set { this.value = value; }
		}
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			if(!object.ReferenceEquals(this.GetType(), obj.GetType()))
				return false;
			return object.Equals(this.Value, ((OperandValue)obj).Value);
		}
		public override int GetHashCode() {
			object value = Value;
			return value != null ? value.GetHashCode() : -1;
		}
		public static explicit operator OperandValue(Boolean val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(Byte val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(Char val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(Decimal val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(Double val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(Single val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(Int16 val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(Int32 val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(Int64 val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(Guid val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(String val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(DateTime val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(TimeSpan val) {
			return new OperandValue(val);
		}
		public static implicit operator OperandValue(Byte[] val) {
			return new OperandValue(val);
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public OperandValue Clone() {
			ICloneable cloneableValue = Value as ICloneable;
			if(cloneableValue != null)
				return new OperandValue(cloneableValue.Clone());
			return new OperandValue(Value);
		}
	}
	[Serializable]
	public class OperandParameter : OperandValue {
		public OperandParameter(string parameterName, object value):base(value) {
			this.parameterName = parameterName;
		}
		public OperandParameter(string parameterName) : this(parameterName, null) { }
		public OperandParameter() : this(null, null) { }
		string parameterName;
		[XmlAttribute]
		public string ParameterName {
			get { return parameterName; }
			set { parameterName = value; }
		}
		public override int GetHashCode() {
			if(ParameterName == null)
				return base.GetHashCode();
			else
				return base.GetHashCode() ^ ParameterName.GetHashCode();
		}
		public override bool Equals(object obj) {
			return base.Equals(obj) && this.ParameterName == ((OperandParameter)obj).ParameterName;
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public new OperandParameter Clone() {
			ICloneable cloneableValue = Value as ICloneable;
			if(cloneableValue != null)
				return new OperandParameter(ParameterName, cloneableValue.Clone());
			else
				return new OperandParameter(ParameterName, Value);
		}
	}
	[Serializable]
	public enum Aggregate { Exists, Count, Max, Min, Avg, Sum, Single };
	[Serializable]
	[XmlInclude(typeof(ContainsOperator))]
	public class AggregateOperand : CriteriaOperator {
		static OperandProperty GetPropertyByName(string propertyName) {
			if(propertyName == null || propertyName.Length == 0)
				return null;
			else
				return new OperandProperty(propertyName);
		}
		CriteriaOperator condition;
		OperandProperty property;
		CriteriaOperator aggregatedExpression;
		Aggregate type;
		public override void Accept(ICriteriaVisitor visitor) {
			IClientCriteriaVisitor clientVisitor = (IClientCriteriaVisitor)visitor;
			clientVisitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			var clientVisitor = (IClientCriteriaVisitor<T>)visitor;
			return clientVisitor.Visit(this);
		}
		public AggregateOperand(OperandProperty collectionProperty, CriteriaOperator aggregatedExpression, Aggregate type, CriteriaOperator condition) {
			this.condition = condition;
			this.property = collectionProperty;
			this.aggregatedExpression = aggregatedExpression;
			this.type = type;
		}
		public AggregateOperand(string collectionProperty, string aggregatedExpression, Aggregate type, CriteriaOperator condition) : this(GetPropertyByName(collectionProperty), GetPropertyByName(aggregatedExpression), type, condition) { }
		public AggregateOperand(string collectionProperty, Aggregate type, CriteriaOperator condition) : this(GetPropertyByName(collectionProperty), null, type, condition) { }
		public AggregateOperand(string collectionProperty, string aggregatedExpression, Aggregate type) : this(collectionProperty, aggregatedExpression, type, null) { }
		public AggregateOperand(string collectionProperty, Aggregate type) : this(collectionProperty, type, null) { }
		public AggregateOperand() : this((OperandProperty)null, (CriteriaOperator)null, Aggregate.Exists, null) { }
		public CriteriaOperator Condition {
			get { return condition; }
			set { condition = value; }
		}
		public OperandProperty CollectionProperty {
			get { return property; }
			set { property = value; }
		}
		public CriteriaOperator AggregatedExpression {
			get { return aggregatedExpression; }
			set { aggregatedExpression = value; }
		}
		public Aggregate AggregateType {
			get { return type; }
			set { type = value; }
		}
		public override bool Equals(object obj) {
			AggregateOperand another = obj as AggregateOperand;
			if(ReferenceEquals(another, null))
				return false;
			return object.Equals(this.Condition, another.Condition) &&
				object.Equals(this.CollectionProperty, another.CollectionProperty) &&
				object.Equals(this.AggregatedExpression, another.AggregatedExpression) &&
				object.Equals(this.AggregateType, another.AggregateType);
		}
		public override int GetHashCode() {
			return ReferenceEquals(Condition, null) ? -1 : Condition.GetHashCode();
		}
		public bool IsTopLevel {
			get {
				if(ReferenceEquals(CollectionProperty, null))
					return true;
				if(string.IsNullOrEmpty(CollectionProperty.PropertyName))
					return true;
				return false;
			}
		}
		public AggregateOperand Exists(CriteriaOperator aggregatedExpression) {
			return new AggregateOperand(this.CollectionProperty, aggregatedExpression, Aggregate.Exists, this.Condition);
		}
		public AggregateOperand Exists() {
			return this.Exists(null);
		}
		public AggregateOperand Count(CriteriaOperator aggregatedExpression) {
			return new AggregateOperand(this.CollectionProperty, aggregatedExpression, Aggregate.Count, this.Condition);
		}
		public AggregateOperand Count() {
			return this.Count(null);
		}
		public AggregateOperand Avg(CriteriaOperator aggregatedExpression) {
			return new AggregateOperand(this.CollectionProperty, aggregatedExpression, Aggregate.Avg, this.Condition);
		}
		public AggregateOperand Max(CriteriaOperator aggregatedExpression) {
			return new AggregateOperand(this.CollectionProperty, aggregatedExpression, Aggregate.Max, this.Condition);
		}
		public AggregateOperand Min(CriteriaOperator aggregatedExpression) {
			return new AggregateOperand(this.CollectionProperty, aggregatedExpression, Aggregate.Min, this.Condition);
		}
		public AggregateOperand Sum(CriteriaOperator aggregatedExpression) {
			return new AggregateOperand(this.CollectionProperty, aggregatedExpression, Aggregate.Sum, this.Condition);
		}
		public AggregateOperand Single(CriteriaOperator aggregatedExpression) {
			return new AggregateOperand(this.CollectionProperty, aggregatedExpression, Aggregate.Single, this.Condition);
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public AggregateOperand Clone() {
			return new AggregateOperand(Clone(CollectionProperty), Clone(AggregatedExpression), this.AggregateType, Clone(Condition));
		}
	}
	[Serializable]
	public class JoinOperand: CriteriaOperator {
		string joinTypeName;
		CriteriaOperator condition;
		CriteriaOperator aggregatedExpression;
		Aggregate type;
		public override void Accept(ICriteriaVisitor visitor) {
			IClientCriteriaVisitor clientVisitor = (IClientCriteriaVisitor)visitor;
			clientVisitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			var clientVisitor = (IClientCriteriaVisitor<T>)visitor;
			return clientVisitor.Visit(this);
		}
		public JoinOperand(string joinTypeName, CriteriaOperator condition, Aggregate type, CriteriaOperator aggregatedExpression) {
			this.condition = condition;
			this.joinTypeName = joinTypeName;
			this.aggregatedExpression = aggregatedExpression;
			this.type = type;
		}
		public JoinOperand(string joinTypeName, CriteriaOperator condition) : this(joinTypeName, condition, Aggregate.Exists, null) { }
		public JoinOperand() : this(null, null, Aggregate.Exists, null) { }
		public CriteriaOperator Condition {
			get { return condition; }
			set { condition = value; }
		}
		public string JoinTypeName {
			get { return joinTypeName; }
			set { joinTypeName = value; }
		}
		public CriteriaOperator AggregatedExpression {
			get { return aggregatedExpression; }
			set { aggregatedExpression = value; }
		}
		public Aggregate AggregateType {
			get { return type; }
			set { type = value; }
		}
		public override bool Equals(object obj) {
			JoinOperand another = obj as JoinOperand;
			if(ReferenceEquals(another, null))
				return false;
			return object.Equals(this.Condition, another.Condition) &&
				object.Equals(this.JoinTypeName, another.JoinTypeName) &&
				object.Equals(this.AggregatedExpression, another.AggregatedExpression) &&
				object.Equals(this.AggregateType, another.AggregateType);
		}
		public override int GetHashCode() {
			return (ReferenceEquals(Condition, null) ? -1 : Condition.GetHashCode()) ^ (ReferenceEquals(JoinTypeName, null) ? -1 : JoinTypeName.GetHashCode());
		}
		public JoinOperand Exists(CriteriaOperator aggregatedExpression) {
			return new JoinOperand(this.JoinTypeName, this.Condition, Aggregate.Exists, aggregatedExpression);
		}
		public JoinOperand Exists() {
			return this.Exists(null);
		}
		public JoinOperand Count(CriteriaOperator aggregatedExpression) {
			return new JoinOperand(this.JoinTypeName, this.Condition, Aggregate.Count, aggregatedExpression);
		}
		public JoinOperand Count() {
			return this.Count(null);
		}
		public JoinOperand Avg(CriteriaOperator aggregatedExpression) {
			return new JoinOperand(this.JoinTypeName, this.Condition, Aggregate.Avg, aggregatedExpression);
		}
		public JoinOperand Max(CriteriaOperator aggregatedExpression) {
			return new JoinOperand(this.JoinTypeName, this.Condition, Aggregate.Max, aggregatedExpression);
		}
		public JoinOperand Min(CriteriaOperator aggregatedExpression) {
			return new JoinOperand(this.JoinTypeName, this.Condition, Aggregate.Min, aggregatedExpression);
		}
		public JoinOperand Sum(CriteriaOperator aggregatedExpression) {
			return new JoinOperand(this.JoinTypeName, this.Condition, Aggregate.Sum, aggregatedExpression);
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public JoinOperand Clone() {
			return new JoinOperand(JoinTypeName, Clone(Condition), this.AggregateType, Clone(AggregatedExpression));
		}
		internal static CriteriaOperator JoinOrAggreagate(OperandProperty collectionProperty, CriteriaOperator condition, Aggregate type, CriteriaOperator aggregated) {
			if(ReferenceEquals(collectionProperty, null) || collectionProperty.PropertyName.Length < 2 || collectionProperty.PropertyName[0] != '<' || collectionProperty.PropertyName[collectionProperty.PropertyName.Length - 1] != '>') {
				return new AggregateOperand(collectionProperty, aggregated, type, condition);
			} else {
				return new JoinOperand(collectionProperty.PropertyName.Substring(1, collectionProperty.PropertyName.Length - 2), condition, type, aggregated);
			}
		}
	}
	[Serializable]
	public sealed class NotOperator : UnaryOperator {
		public NotOperator() : this(null) { }
		public NotOperator(CriteriaOperator operand) : base(UnaryOperatorType.Not, operand) { }
	}
	[Serializable]
	public sealed class ContainsOperator : AggregateOperand {
		public ContainsOperator(OperandProperty collectionProperty, CriteriaOperator condition) : base(collectionProperty, null, Aggregate.Exists, condition) { }
		public ContainsOperator() : this((OperandProperty)null, null) { }
		public ContainsOperator(string collectionProperty, CriteriaOperator condition) : this(new OperandProperty(collectionProperty), condition) { }
	}
	[Serializable]
	public sealed class BetweenOperator : CriteriaOperator {
		public override void Accept(ICriteriaVisitor visitor) {
			visitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		CriteriaOperator testExpression;
		CriteriaOperator beginExpression;
		CriteriaOperator endExpression;
		public BetweenOperator() : this((CriteriaOperator)null, null, null) { }
		public BetweenOperator(string testPropertyName, object beginValue, object endValue)
			: this(new OperandProperty(testPropertyName), ObjectToCriteriaSafe(beginValue), ObjectToCriteriaSafe(endValue)) { }
		public BetweenOperator(CriteriaOperator testExpression, CriteriaOperator beginExpression, CriteriaOperator endExpression) {
			this.beginExpression = beginExpression;
			this.endExpression = endExpression;
			this.testExpression = testExpression;
		}
		public BetweenOperator(string testPropertyName, CriteriaOperator beginExpression, CriteriaOperator endExpression)
			: this(new OperandProperty(testPropertyName), beginExpression, endExpression) { }
		public CriteriaOperator BeginExpression {
			get { return beginExpression; }
			set { beginExpression = value; }
		}
		public CriteriaOperator EndExpression {
			get { return endExpression; }
			set { endExpression = value; }
		}
		public CriteriaOperator TestExpression {
			get { return testExpression; }
			set { testExpression = value; }
		}
		[Obsolete("Use BeginExpression property instead")]
		public CriteriaOperator LeftOperand {
			get { return BeginExpression; }
			set { BeginExpression = value; }
		}
		[Obsolete("Use EndExpression property instead")]
		public CriteriaOperator RightOperand {
			get { return EndExpression; }
			set { EndExpression = value; }
		}
		[Obsolete("Use TestExpression property instead")]
		public CriteriaOperator Property {
			get { return TestExpression; }
			set { TestExpression = value; }
		}
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			if(!object.ReferenceEquals(this.GetType(), obj.GetType()))
				return false;
			return object.Equals(this.TestExpression, ((BetweenOperator)obj).TestExpression) &&
				object.Equals(this.BeginExpression, ((BetweenOperator)obj).BeginExpression) &&
				object.Equals(this.EndExpression, ((BetweenOperator)obj).EndExpression);
		}
		public override int GetHashCode() {
			return (ReferenceEquals(TestExpression, null) ? -1 : TestExpression.GetHashCode()) ^ (ReferenceEquals(BeginExpression, null) ? -1 : BeginExpression.GetHashCode()) ^ (ReferenceEquals(EndExpression, null) ? -1 : EndExpression.GetHashCode());
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public BetweenOperator Clone() {
			return new BetweenOperator(Clone(TestExpression), Clone(BeginExpression), Clone(EndExpression));
		}
	}
	[Serializable]
	public sealed class NullOperator : UnaryOperator {
		public NullOperator() : this((CriteriaOperator)null) { }
		public NullOperator(CriteriaOperator operand) : base(UnaryOperatorType.IsNull, operand) { }
		public NullOperator(string operand) : this(new OperandProperty(operand)) { }
	}
	[Serializable]
	public class InOperator : CriteriaOperator {
		public override void Accept(ICriteriaVisitor visitor) {
			visitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		CriteriaOperator leftOperand;
		public CriteriaOperator LeftOperand {
			get {
				return leftOperand;
			}
			set {
				leftOperand = value;
			}
		}
		CriteriaOperatorCollection operands = new CriteriaOperatorCollection();
		[XmlArrayItem(typeof(CriteriaOperator))]
		public virtual CriteriaOperatorCollection Operands { get { return operands; } }
		public InOperator() : this((CriteriaOperator)null) { }
		public InOperator(string propertyName, IEnumerable values)
			: this(new OperandProperty(propertyName)) {
			operands.AddRange(values.Cast<object>().Select(val => ObjectToCriteriaSafe(val)));
		}
		public InOperator(string propertyName, params CriteriaOperator[] operands) : this(new OperandProperty(propertyName), operands) { }
		public InOperator(CriteriaOperator leftOperand, params CriteriaOperator[] operands) : this(leftOperand, (IEnumerable<CriteriaOperator>)operands) { }
		public InOperator(CriteriaOperator leftOperand, IEnumerable<CriteriaOperator> operands) {
			this.leftOperand = leftOperand;
			if(operands != null)
				this.operands.AddRange(operands);
		}
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			if(!object.ReferenceEquals(this.GetType(), obj.GetType()))
				return false;
			CriteriaOperatorCollection leftOperands = Operands;
			CriteriaOperatorCollection rightOperands = ((InOperator)obj).Operands;
			if(this.Operands.Count != rightOperands.Count)
				return false;
			if(!object.Equals(this.LeftOperand, ((InOperator)obj).LeftOperand))
				return false;
			for(int i = 0; i < leftOperands.Count; ++i) {
				if(!object.Equals(leftOperands[i], rightOperands[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int hash = ReferenceEquals(LeftOperand, null) ? -1 : LeftOperand.GetHashCode();
			foreach(object obj in Operands)
				if(obj != null)
					hash ^= obj.GetHashCode();
			return hash;
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public InOperator Clone() {
			return new InOperator(Clone(LeftOperand), Clone(Operands));
		}
	}
	[Serializable]
	public enum BinaryOperatorType { Equal, NotEqual, Greater, Less, LessOrEqual, GreaterOrEqual,
	[Obsolete("Use StartsWith, EndsWith, Contains functions or Like custom function instead. See https://www.devexpress.com/Support/Center/Question/Details/T313960 for details.")]
	Like,
		BitwiseAnd, BitwiseOr, BitwiseXor, Divide, Modulo, Multiply, Plus, Minus };
	[Serializable]
	public class BinaryOperator : CriteriaOperator {
		public override void Accept(ICriteriaVisitor visitor) {
			visitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		[XmlAttribute]
		public BinaryOperatorType OperatorType;
		public BinaryOperator() : this((CriteriaOperator)null, (CriteriaOperator)null, BinaryOperatorType.Equal) { }
		public BinaryOperator(CriteriaOperator opLeft, CriteriaOperator opRight, BinaryOperatorType type) {
			this.LeftOperand = opLeft;
			this.RightOperand = opRight;
			this.OperatorType = type;
		}
		public CriteriaOperator LeftOperand;
		public CriteriaOperator RightOperand;
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			if(!object.ReferenceEquals(this.GetType(), obj.GetType()))
				return false;
			return object.Equals(this.OperatorType, ((BinaryOperator)obj).OperatorType) &&
				object.Equals(this.LeftOperand, ((BinaryOperator)obj).LeftOperand) &&
				object.Equals(this.RightOperand, ((BinaryOperator)obj).RightOperand);
		}
		public override int GetHashCode() {
			return (ReferenceEquals(LeftOperand, null) ? -1 : LeftOperand.GetHashCode()) ^ (ReferenceEquals(RightOperand, null) ? -1 : RightOperand.GetHashCode());
		}
		public BinaryOperator(string propertyName, Boolean value, BinaryOperatorType type) : this(new OperandProperty(propertyName), (OperandValue)value, type) { }
		public BinaryOperator(string propertyName, Byte value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, Char value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, Decimal value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, Double value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, Single value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, Int32 value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, Int64 value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, Int16 value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, Guid value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, String value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, DateTime value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, TimeSpan value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, Byte[] value, BinaryOperatorType type) : this(new OperandProperty(propertyName), value, type) { }
		public BinaryOperator(string propertyName, object value, BinaryOperatorType type) : this(new OperandProperty(propertyName), ObjectToCriteriaSafe(value), type) { }
		public BinaryOperator(string propertyName, Boolean value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, Byte value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, Char value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, Decimal value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, Double value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, Single value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, Int32 value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, Int64 value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, Int16 value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, Guid value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, String value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, DateTime value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, TimeSpan value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, Byte[] value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		public BinaryOperator(string propertyName, object value) : this(propertyName, value, BinaryOperatorType.Equal) { }
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public BinaryOperator Clone() {
			return new BinaryOperator(Clone(LeftOperand), Clone(RightOperand), OperatorType);
		}
	}
	[Serializable]
	public enum GroupOperatorType { And, Or };
	[Serializable]
	public sealed class GroupOperator : CriteriaOperator {
		CriteriaOperatorCollection operands = new CriteriaOperatorCollection();
		public override void Accept(ICriteriaVisitor visitor) {
			visitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		[XmlArrayItem(typeof(CriteriaOperator))]
		public CriteriaOperatorCollection Operands { get { return operands; } }
		[XmlAttribute]
		public GroupOperatorType OperatorType;
		public GroupOperator()
			: this(GroupOperatorType.And) {
		}
		public GroupOperator(GroupOperatorType type, params CriteriaOperator[] operands)
			: this(type, (IEnumerable<CriteriaOperator>)operands) { }
		public GroupOperator(GroupOperatorType type, IEnumerable<CriteriaOperator> operands) {
			this.OperatorType = type;
			this.operands.AddRange(operands);
		}
		public GroupOperator(params CriteriaOperator[] operands) : this(GroupOperatorType.And, operands) { }
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			if(!object.ReferenceEquals(this.GetType(), obj.GetType()))
				return false;
			if(this.OperatorType != ((GroupOperator)obj).OperatorType)
				return false;
			if(this.Operands.Count != ((GroupOperator)obj).Operands.Count)
				return false;
			for(int i = 0; i < this.Operands.Count; ++i) {
				if(!object.Equals(this.Operands[i], ((GroupOperator)obj).Operands[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int hash = -1;
			foreach(object obj in Operands)
				if(obj != null)
					hash ^= obj.GetHashCode();
			return hash;
		}
		public static CriteriaOperator Combine(GroupOperatorType opType, CriteriaOperator left, CriteriaOperator right) {
			if(ReferenceEquals(left, null))
				return right;
			if(ReferenceEquals(right, null))
				return left;
			return Combine(opType, new CriteriaOperator[] { left, right });
		}
		public static CriteriaOperator Combine(GroupOperatorType opType, params CriteriaOperator[] operands) {
			return Combine(opType, (IEnumerable<CriteriaOperator>)operands);
		}
		public static CriteriaOperator Combine(GroupOperatorType opType, IEnumerable<CriteriaOperator> operands) {
			if(operands == null)
				return null;
			ICollection countSource = operands as ICollection;
			if(countSource != null) {
				switch(countSource.Count) {
					case 0:
						return null;
					case 1:
						return operands.First();
				}
			}
			var rvOperands = CombineCore(opType, operands).ToList();
			switch(rvOperands.Count){
				case 0:
					return null;
				case 1:
					return rvOperands[0];
				default:
					return new GroupOperator(opType, rvOperands);
			}
		}
		static IEnumerable<CriteriaOperator> CombineCore(GroupOperatorType opType, IEnumerable<CriteriaOperator> operands) {
			foreach(var op in operands) {
				if(ReferenceEquals(op, null))
					continue;
				GroupOperator gop = op as GroupOperator;
				if(ReferenceEquals(gop, null) || gop.OperatorType != opType) {
					yield return op;
					continue;
				}
				foreach(var op2 in gop.Operands)
					yield return op2;
			}
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public GroupOperator Clone() {
			return new GroupOperator(OperatorType, Clone(Operands));
		}
	}
	[Serializable]
	public enum UnaryOperatorType { BitwiseNot, Plus, Minus, Not, IsNull };
	[Serializable]
	public class UnaryOperator : CriteriaOperator {
		public override void Accept(ICriteriaVisitor visitor) {
			visitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		public CriteriaOperator Operand;
		[XmlAttribute]
		public UnaryOperatorType OperatorType;
		public UnaryOperator() : this(UnaryOperatorType.Not, (CriteriaOperator)null) { }
		public UnaryOperator(UnaryOperatorType operatorType, string propertyName) : this(operatorType, new OperandProperty(propertyName)) { }
		public UnaryOperator(UnaryOperatorType operatorType, CriteriaOperator operand) {
			this.Operand = operand;
			this.OperatorType = operatorType;
		}
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			if(!object.ReferenceEquals(this.GetType(), obj.GetType()))
				return false;
			return object.Equals(this.OperatorType, ((UnaryOperator)obj).OperatorType) &&
				object.Equals(this.Operand, ((UnaryOperator)obj).Operand);
		}
		public override int GetHashCode() {
			return (!ReferenceEquals(Operand, null)) ? Operand.GetHashCode() : -1;
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public UnaryOperator Clone() {
			return new UnaryOperator(this.OperatorType, Clone(Operand));
		}
	}
	[Serializable]
	public enum FunctionOperatorType {
		None,
		Custom,
		CustomNonDeterministic,
		Iif,
		IsNull, IsNullOrEmpty,
		Trim, Len, Substring, Upper, Lower, Concat, Ascii, Char, ToStr, Replace, Reverse, Insert, CharIndex, Remove,
		Abs, Sqr, Cos, Sin, Atn, Exp, Log, Rnd, Tan, Power, Sign, Round, Ceiling, Floor, Max, Min,
		Acos, Asin, Atn2, BigMul, Cosh, Log10, Sinh, Tanh,
		PadLeft, PadRight,
		StartsWith, EndsWith, Contains,		
		ToInt, ToLong, ToFloat, ToDouble, ToDecimal,
		LocalDateTimeThisYear,
		LocalDateTimeThisMonth,
		LocalDateTimeLastWeek,
		LocalDateTimeThisWeek,
		LocalDateTimeYesterday,
		LocalDateTimeToday,
		LocalDateTimeNow,
		LocalDateTimeTomorrow,
		LocalDateTimeDayAfterTomorrow,
		LocalDateTimeNextWeek,
		LocalDateTimeTwoWeeksAway,
		LocalDateTimeNextMonth,
		LocalDateTimeNextYear,
		IsOutlookIntervalBeyondThisYear,			
		IsOutlookIntervalLaterThisYear,	 
		IsOutlookIntervalLaterThisMonth,	
		IsOutlookIntervalNextWeek,		  
		IsOutlookIntervalLaterThisWeek,	 
		IsOutlookIntervalTomorrow,		  
		IsOutlookIntervalToday,			 
		IsOutlookIntervalYesterday,		 
		IsOutlookIntervalEarlierThisWeek,   
		IsOutlookIntervalLastWeek,		  
		IsOutlookIntervalEarlierThisMonth,  
		IsOutlookIntervalEarlierThisYear,   
		IsOutlookIntervalPriorThisYear,	 
		IsThisWeek, IsThisMonth, IsThisYear,
		DateDiffTick,
		DateDiffSecond,
		DateDiffMilliSecond,
		DateDiffMinute,
		DateDiffHour,
		DateDiffDay,
		DateDiffMonth,
		DateDiffYear,
		GetDate,
		GetMilliSecond,
		GetSecond,
		GetMinute,
		GetHour,
		GetDay,
		GetMonth,
		GetYear,
		GetDayOfWeek,
		GetDayOfYear,
		GetTimeOfDay,
		Now,
		UtcNow,
		Today,
		AddTimeSpan,
		AddTicks,
		AddMilliSeconds,
		AddSeconds,
		AddMinutes,
		AddHours,
		AddDays,
		AddMonths,
		AddYears,
	}
	[Serializable]
	public class FunctionOperator : CriteriaOperator {
		CriteriaOperatorCollection operands = new CriteriaOperatorCollection();
		public override void Accept(ICriteriaVisitor visitor) {
			visitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		[XmlArrayItem(typeof(CriteriaOperator))]
		public CriteriaOperatorCollection Operands { get { return operands; } }
		[XmlAttribute]
		public FunctionOperatorType OperatorType;
		public FunctionOperator() : this(FunctionOperatorType.None) { }
		public FunctionOperator(FunctionOperatorType type, params CriteriaOperator[] operands)
			: this(type, (IEnumerable<CriteriaOperator>)operands) { }
		public FunctionOperator(string customFunctionName, params CriteriaOperator[] operands)
			: this(customFunctionName, (IEnumerable<CriteriaOperator>)operands) {
		}
		public FunctionOperator(string customFunctionName, IEnumerable<CriteriaOperator> operands) {
			this.OperatorType = FunctionOperatorType.Custom;
			List<CriteriaOperator> list = new List<CriteriaOperator>();
			list.Add(new ConstantValue(customFunctionName));
			list.AddRange(operands);
			this.operands.AddRange(list);
		}
		public FunctionOperator(FunctionOperatorType type, IEnumerable<CriteriaOperator> operands) {
			this.OperatorType = type;
			this.operands.AddRange(operands);
		}
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			if(!object.ReferenceEquals(this.GetType(), obj.GetType()))
				return false;
			if(this.OperatorType != ((FunctionOperator)obj).OperatorType)
				return false;
			if(this.Operands.Count != ((FunctionOperator)obj).Operands.Count)
				return false;
			for(int i = 0; i < this.Operands.Count; ++i) {
				if(!object.Equals(this.Operands[i], ((FunctionOperator)obj).Operands[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int hash = -1;
			foreach(object obj in Operands)
				if(obj != null)
					hash ^= obj.GetHashCode();
			return hash;
		}
		public static bool GuessIsLogicalCustomFunction(FunctionOperator theOperator) {
			if(LikeCustomFunction.IsBinaryCompatibleLikeFunction(theOperator))
				return true;
			string customFunctionName = null;
			OperandValue customFunctionNameOperand = theOperator.Operands[0] as OperandValue;
			if(!ReferenceEquals(customFunctionNameOperand, null) && !string.IsNullOrEmpty(customFunctionName = customFunctionNameOperand.Value as string)
				&& customFunctionName.StartsWith("Is") && customFunctionName.Length > 2 && char.IsUpper(customFunctionName[2])) {
				return true;
			}
			return false;
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public FunctionOperator Clone() {
			return new FunctionOperator(OperatorType, Clone(Operands));
		}
	}
	public interface ICustomFunctionOperator {
		string Name { get; }
		Type ResultType(params Type[] operands);
		object Evaluate(params object[] operands);
	}
	public interface ICustomFunctionOperatorEvaluatableWithCaseSensitivity: ICustomFunctionOperator {
		object Evaluate(bool caseSensetive, params object[] operands);
	}
	public interface ICustomFunctionOperatorFormattable : ICustomFunctionOperator {
		string Format(Type providerType, params string[] operands);
	}
	public interface ICustomFunctionOperatorBrowsable : ICustomFunctionOperator {
		int MinOperandCount { get; }
		int MaxOperandCount { get; }
		bool IsValidOperandCount(int count);
		string Description { get; }
		bool IsValidOperandType(int operandIndex, int operandCount, Type type);
		FunctionCategory Category { get; }
	}
	public interface ICustomFunctionOperatorConvertibleToExpression : ICustomFunctionOperator {
		Expression Convert(DevExpress.Data.Linq.ICriteriaToExpressionConverter converter, params Expression[] operands);
	}
	public interface ICustomFunctionOperatorCompileable: ICustomFunctionOperator {
		Expression Create(params Expression[] operands);
	}
	public interface ICustomFunctionOperatorCompileableWithCaseSensitivity: ICustomFunctionOperatorCompileable {
		Expression Create(bool caseSensetive, params Expression[] operands);
	}
	public enum FunctionCategory {
		DateTime = 1,
		Logical = 2,
		Math = 4,
		Text = 8,
		All = DateTime | Logical | Math | Text
	}
	public abstract class CustomDictionaryCollection<K, T> : ICollection<T> {
		readonly Dictionary<K, T> items;
		protected CustomDictionaryCollection() {
			items = new Dictionary<K, T>();
		}
		protected CustomDictionaryCollection(IEqualityComparer<K> customComparer) {
			items = new Dictionary<K, T>(customComparer);
		}
		protected abstract K GetKey(T item);
		public void Add(T item) {
			if (item == null) throw new ArgumentNullException();
			items.Add(GetKey(item), item);
		}
		public void Add(IEnumerable<T> items) {
			foreach (T item in items) {
				Add(item);
			}
		}
		public void Clear() {
			items.Clear();
		}
		public bool Contains(T item) {
			if (item == null) return false;
			return items.ContainsKey(GetKey(item));
		}
		public void CopyTo(T[] array, int arrayIndex) {
			int index = arrayIndex;
			foreach (T customFunction in items.Values) {
				array[index++] = customFunction;
			}
		}
		public int Count {
			get { return items.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(T item) {
			if (item == null) return false;
			K key = GetKey(item);
			return items.Remove(key);
		}
		public IEnumerator<T> GetEnumerator() {
			return items.Values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)items.Values).GetEnumerator();
		}
		public T GetItem(K key) {
			T item;
			if(items.TryGetValue(key, out item)) {
				return item;
			}
			return default(T);
		}
	}
	public abstract class CustomMultiKeyDictionaryCollection<K, T> : ICollection<T> {
		readonly Dictionary<K, T> items;
		protected CustomMultiKeyDictionaryCollection() {
			items = new Dictionary<K, T>();
		}
		protected CustomMultiKeyDictionaryCollection(IEqualityComparer<K> customComparer) {
			items = new Dictionary<K, T>(customComparer);
		}
		protected abstract K[] GetKey(T item);
		public void Add(T item) {
			if(item == null) throw new ArgumentNullException();
			K[] keys = GetKey(item);
			if(keys == null) return;
			for(int i = 0; i < keys.Length; i++){
				items.Add(keys[i], item);
			}
		}
		public void Add(IEnumerable<T> items) {
			foreach(T item in items) {
				Add(item);
			}
		}
		public void Clear() {
			items.Clear();
		}
		public bool Contains(T item) {
			if(item == null) return false;
			K[] keys = GetKey(item);
			if(keys == null || keys.Length == 0) return false;
			return items.ContainsKey(keys[0]);
		}
		public void CopyTo(T[] array, int arrayIndex) {
			int index = arrayIndex;
			foreach(T customFunction in items.Values) {
				array[index++] = customFunction;
			}
		}
		public int Count {
			get { return items.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(T item) {
			if(item == null) return false;
			K[] keys = GetKey(item);
			if(keys == null || keys.Length == 0) return false;
			bool result = false;
			for(int i = 0; i < keys.Length; i++) {
				if(items.Remove(keys[i])) result = true;
			}
			return result;
		}
		public IEnumerator<T> GetEnumerator() {
			return items.Values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)items.Values).GetEnumerator();
		}
		public T GetItem(K key) {
			T item;
			if(items.TryGetValue(key, out item)) {
				return item;
			}
			return default(T);
		}
	}
	public class CustomFunctionCollection : ICollection<ICustomFunctionOperator> {
		Dictionary<string, ICustomFunctionOperator> customFunctionByName = new Dictionary<string, ICustomFunctionOperator>(StringExtensions.ComparerInvariantCultureIgnoreCase);
		public void Add(ICustomFunctionOperator item) {
			if (item == null) throw new ArgumentNullException();
			customFunctionByName[item.Name] = item;
		}
		public void Add(IEnumerable<ICustomFunctionOperator> items) {
			foreach (ICustomFunctionOperator item in items) {
				Add(item);
			}
		}
		public void Clear() {
			customFunctionByName.Clear();
		}
		public bool Contains(ICustomFunctionOperator item) {
			if (item == null) return false;
			return customFunctionByName.ContainsKey(item.Name);
		}
		public void CopyTo(ICustomFunctionOperator[] array, int arrayIndex) {
			int index = arrayIndex;
			foreach (ICustomFunctionOperator customFunction in customFunctionByName.Values) {
				array[index++] = customFunction;
			}
		}
		public int Count {
			get { return customFunctionByName.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(ICustomFunctionOperator item) {
			if (item == null) return false;
			return customFunctionByName.Remove(item.Name);
		}
		public IEnumerator<ICustomFunctionOperator> GetEnumerator() {
			return customFunctionByName.Values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)customFunctionByName.Values).GetEnumerator();
		}
		public ICustomFunctionOperator GetCustomFunction(string functionName) {
			ICustomFunctionOperator customFunction;
			if (customFunctionByName.TryGetValue(functionName, out customFunction)) {
				return customFunction;
			}
			return null;
		}
	}
	public class UserValueProcessingEventArgs : HandledEventArgs
	{
		public object Value;
		public string Tag;
		public string Data;
	}
	public static class EnumProcessingHelper {
		internal static void ToStringEnumIfNeeded(UserValueProcessingEventArgs e) {
			if(e.Handled)
				return;
			if(!(e.Value is Enum))
				return;
			Type t = e.Value.GetType();
			if(!t.IsEnum())
				return;
			string name = GetNameForEnumType(t);
			if(string.IsNullOrEmpty(name))
				return;
			e.Data = name + "," + Enum.GetName(t, e.Value);
			e.Tag = CriteriaOperator.TagEnum;
			e.Handled = true;
		}
		internal static void ExtractEnumIfNeeded(UserValueProcessingEventArgs e) {
			if(e.Handled)
				return;
			if(e.Tag != CriteriaOperator.TagEnum)
				return;
			if(e.Data == null)
				throw new ArgumentNullException(e.Data);
			int separatorPos = e.Data.IndexOf(',');
			if(separatorPos < 0)
				throw new ArgumentException("malformed enum definition (',' expected)");
			string typeString = e.Data.Substring(0, separatorPos);
			Type t = GetEnumTypeFromName(typeString);
			string enumName = e.Data.Substring(separatorPos + 1);
			e.Value = Enum.Parse(t, enumName, false);
			e.Handled = true;
		}
		static Type GetEnumTypeFromName(string typeName) {
			if(string.IsNullOrEmpty(typeName))
				throw new ArgumentNullException("typeName");
			using(LockForRead()) {
				Type t;
				if(stringsToEnumType.TryGetValue(typeName, out t))
					return t;
			}
			throw new InvalidOperationException(string.Format("The '{0}' enum is not registered for the parse operation. Please use EnumProcessingHelper.RegisterEnum for that type.", typeName));
		}
		static string GetNameForEnumType(Type enumType) {
			if(enumType == null)
				throw new ArgumentNullException("enumType");
			if(!enumType.IsEnum())
				throw new ArgumentException("enumType is not enum");
			using(LockForRead()) {
				string nm;
				if(enumsToToStringName.TryGetValue(enumType, out nm))
					return nm;
			}
			return null;
		}
		public static void RegisterEnum(Type enumType) {
			RegisterEnum(enumType, null);
		}
		public static void RegisterEnum<T>() {
			RegisterEnum(typeof(T));
		}
		public static void RegisterEnum(Type enumType, string toStringName) {
			if(enumType == null)
				throw new ArgumentNullException("enumType");
			if(!enumType.IsEnum())
				throw new ArgumentException("enumType is not enum");
			using(LockForChange()) {
				if(!stringsToEnumType.ContainsKey(enumType.FullName))
					stringsToEnumType.Add(enumType.FullName, enumType);
				if(string.IsNullOrEmpty(toStringName) || toStringName == enumType.FullName) {
					if(!enumsToToStringName.ContainsKey(enumType))
						enumsToToStringName.Add(enumType, enumType.FullName);
				} else {
					if(!stringsToEnumType.ContainsKey(toStringName))
						stringsToEnumType.Add(toStringName, enumType);
					if(!enumsToToStringName.ContainsKey(enumType))
						enumsToToStringName.Add(enumType, toStringName);
				}
			}
		}
		public static void RegisterEnums(Assembly assembly) {
			RegisterEnums(assembly, false);
		}
#if !SL
		public
#endif
			static void RegisterEnums(Assembly assembly, bool suppressReferencesProcessing) {
			ProcessRegistering(assembly, new Dictionary<Assembly, object>(), suppressReferencesProcessing);
		}
#if !SL && !DXPORTABLE
		public static void RegisterEnums() {
			RegisterEnums(AppDomain.CurrentDomain.GetAssemblies());
		}
#endif
		public static void RegisterEnums(Assembly[] assemblies) {
			RegisterEnums(assemblies, false);
		}
#if !SL
		public
#endif
			static void RegisterEnums(Assembly[] assemblies, bool suppressReferencesProcessing) {
			Dictionary<Assembly, object> passedAssemblies = new Dictionary<Assembly, object>();
			foreach(Assembly a in assemblies)
				ProcessRegistering(a, passedAssemblies, suppressReferencesProcessing);
		}
		static void ProcessRegistering(Assembly currentAssembly, IDictionary<Assembly, object> passedAssemblies, bool suppressReferencesProcessing) {
			if(currentAssembly == null)
				return;
			if(passedAssemblies.ContainsKey(currentAssembly))
				return;
			passedAssemblies.Add(currentAssembly, currentAssembly);
			Type[] types;
			try {
				types = currentAssembly.GetExportedTypes();
			} catch(ReflectionTypeLoadException tle) {
				types = tle.Types;
			} catch {
				types = null;
			}
			if(types != null) {
				foreach(Type t in types) {
					if(!t.IsEnum())
						continue;
					RegisterEnum(t);
				}
			}
#if !DXPORTABLE
			if(suppressReferencesProcessing)
				return;
			AssemblyName[] assNames = currentAssembly.GetReferencedAssemblies();
			foreach(var an in assNames) {
				Assembly a;
				try {
					a = Assembly.Load(an);
				} catch {
					continue;
				}
				ProcessRegistering(a, passedAssemblies, suppressReferencesProcessing);
			}
#endif
		}
#if DXPORTABLE
		static IDisposable LockForRead() {
			return LockForChange();
		}
		class DataCacheLockForAwfulFrameworks : IDisposable {
			public readonly object Lock;
			public DataCacheLockForAwfulFrameworks(object obj) {
				Lock = obj;
				Monitor.Enter(Lock);
			}
			public void Dispose() {
				Monitor.Exit(Lock);
			}
		}
		static IDisposable LockForChange() {
			return new DataCacheLockForAwfulFrameworks(stringsToEnumType);
		}
#else
		static System.Threading.ReaderWriterLock _RWL = new System.Threading.ReaderWriterLock();
		static IDisposable LockForRead() {
			return new DevExpress.Xpo.DB.Helpers.DataCacheReaderLock(_RWL);
		}
		static IDisposable LockForChange() {
			if(_RWL.IsReaderLockHeld)
				throw new InvalidOperationException("internal error: reader lock already held!!!");
			return new DevExpress.Xpo.DB.Helpers.DataCacheWriterLock(_RWL);
		}
#endif
		static readonly Dictionary<string, Type> stringsToEnumType = new Dictionary<string, Type>();
		static readonly Dictionary<Type, string> enumsToToStringName = new Dictionary<Type, string>();
	}
}
namespace DevExpress.Xpo {
	using DevExpress.Compatibility.System.ComponentModel;
	using DevExpress.Data.Filtering;
#if !DXPORTABLE
#if !CF && !SL
	using System.ComponentModel.Design.Serialization;
	using System.Reflection;
	[TypeConverter(typeof(SortProperty.SortPropertyConverter))]
#endif
	public sealed class SortProperty {
		CriteriaOperator property;
		SortingDirection direction;
#if !CF && !SL
		[DefaultValue("")]
		[Browsable(false)]
#endif
		public string PropertyName {
			get { return (object)property == null ? String.Empty : property.ToString(); }
			set { property = String.IsNullOrEmpty(value) ? null : CriteriaOperator.Parse(value); }
		}
#if !CF && !SL
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
		public CriteriaOperator Property {
			get { return property; }
			set { property = value; }
		}
#if !CF && !SL
		[DefaultValue(SortingDirection.Ascending)]
#endif
		public SortingDirection Direction {
			get { return direction; }
			set { direction = value; }
		}
		public SortProperty(CriteriaOperator property, SortingDirection direction) {
			this.property = property;
			this.direction = direction;
		}
		public SortProperty(string propertyName, SortingDirection sorting) : this(CriteriaOperator.Parse(propertyName), sorting) { }
		public SortProperty() : this((CriteriaOperator)null, SortingDirection.Ascending) { }
#if !CF && !SL && !DXPORTABLE
		internal class SortPropertyConverter: TypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				if(destinationType == typeof(InstanceDescriptor))
					return true;
				return base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destinationType) {
				if(destinationType == typeof(InstanceDescriptor) && val is SortProperty) {
					ConstructorInfo ctor = typeof(SortProperty).GetConstructor(new Type[] { typeof(string), typeof(SortingDirection) });
					if(ctor != null) {
						return new InstanceDescriptor(ctor, new object[] { ((SortProperty)val).PropertyName, ((SortProperty)val).Direction });
					}
				}
				return base.ConvertTo(context, culture, val, destinationType);
			}
		}
#endif
	}
#endif
}
