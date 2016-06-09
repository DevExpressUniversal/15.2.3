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
using System.Text;
using DevExpress.Data.Filtering;
using System.Linq.Expressions;
using System.Linq;
using DevExpress.Data.Filtering.Helpers;
using System.Reflection;
using DevExpress.Data.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Utils;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
#else
using System.ComponentModel;
using System.Globalization;
#endif
namespace DevExpress.Data.Linq {
using DevExpress.Data.Linq.Helpers;
	public interface ICriteriaToExpressionConverter {
		Expression Convert(ParameterExpression thisExpression, CriteriaOperator op);
	}
	public interface ICriteriaToExpressionConverterCustomizable {
		Expression Convert(ParameterExpression thisExpression, CriteriaOperator op, CriteriaToExpressionConverterEventsHelper eventsHelper);
	}
	public class CriteriaToExpressionConverter : ICriteriaToExpressionConverter, ICriteriaToExpressionConverterCustomizable {
		public Expression Convert(ParameterExpression thisExpression, CriteriaOperator op) {
			return new CriteriaToExpressionConverterInternal(this, thisExpression).Process(op);
		}
		public Expression Convert(ParameterExpression thisExpression, CriteriaOperator op, CriteriaToExpressionConverterEventsHelper eventsHelper) {
			CriteriaToExpressionConverterInternal converter = new CriteriaToExpressionConverterInternal(this, thisExpression);
			if(eventsHelper != null) {
				eventsHelper.Subscribe(converter);
			}
			return converter.Process(op);
		}
	}
	public class CriteriaToExpressionConverterForObjects : ICriteriaToExpressionConverter, ICriteriaToExpressionConverterCustomizable {
		public Expression Convert(ParameterExpression thisExpression, CriteriaOperator op) {
			return new CriteriaToExpressionConverterForObjectsInternal(this, thisExpression).Process(op);
		}
		public Expression Convert(ParameterExpression thisExpression, CriteriaOperator op, CriteriaToExpressionConverterEventsHelper eventsHelper) {
			CriteriaToExpressionConverterForObjectsInternal converter = new CriteriaToExpressionConverterForObjectsInternal(this, thisExpression);
			if(eventsHelper != null) {
				eventsHelper.Subscribe(converter);
			}
			return converter.Process(op);
		}
	}
}
namespace DevExpress.Data.Linq.Helpers {
	public class CriteriaToExpressionConverterForObjectsInternal : CriteriaToExpressionConverterInternal {
		public override bool ForceIifForInstance {
			get { return true; }
		}
		public CriteriaToExpressionConverterForObjectsInternal(ICriteriaToExpressionConverter owner, ParameterExpression thisExpression)
			: base(owner, thisExpression) {
		}
		protected override Expression VisitInternal(FunctionOperator theOperator) {
			switch(theOperator.OperatorType){
				case FunctionOperatorType.DateDiffDay:
					return MakeStaticCall("DateDiffDay", typeof(CriteriaToExpressionConverterForObjectsInternal), theOperator);
				case FunctionOperatorType.DateDiffHour:
					return MakeStaticCall("DateDiffHour", typeof(CriteriaToExpressionConverterForObjectsInternal), theOperator);
				case FunctionOperatorType.DateDiffMilliSecond:
					return MakeStaticCall("DateDiffMilliSecond", typeof(CriteriaToExpressionConverterForObjectsInternal), theOperator);
				case FunctionOperatorType.DateDiffMinute:
					return MakeStaticCall("DateDiffMinute", typeof(CriteriaToExpressionConverterForObjectsInternal), theOperator);
				case FunctionOperatorType.DateDiffMonth:
					return MakeStaticCall("DateDiffMonth", typeof(CriteriaToExpressionConverterForObjectsInternal), theOperator);
				case FunctionOperatorType.DateDiffSecond:
					return MakeStaticCall("DateDiffSecond", typeof(CriteriaToExpressionConverterForObjectsInternal), theOperator);
				case FunctionOperatorType.DateDiffYear:
					return MakeStaticCall("DateDiffYear", typeof(CriteriaToExpressionConverterForObjectsInternal), theOperator);
			}
			return base.VisitInternal(theOperator);
		}
		public static int DateDiffYear(DateTime startDate, DateTime endDate) {
			return endDate.Year - startDate.Year;
		}
		public static int? DateDiffYear(DateTime? startDate, DateTime? endDate) {
			if(!startDate.HasValue || !endDate.HasValue)
				return null;
			return DateDiffYear(startDate.Value, endDate.Value);
		}
		public static int DateDiffMonth(DateTime startDate, DateTime endDate) {
			return DateDiffYear(startDate, endDate) * 12 + (endDate.Month - startDate.Month);
		}
		public static int? DateDiffMonth(DateTime? startDate, DateTime? endDate) {
			if(!startDate.HasValue || !endDate.HasValue)
				return null;
			return DateDiffMonth(startDate, endDate);
		}
		public static int DateDiffDay(DateTime startDate, DateTime endDate) {
			return (endDate.Date - startDate.Date).Days;
		}
		public static int? DateDiffDay(DateTime? startDate, DateTime? endDate) {
			if(!startDate.HasValue || !endDate.HasValue)
				return null;
			return DateDiffDay(startDate.Value, endDate.Value);
		}
		public static int DateDiffHour(DateTime startDate, DateTime endDate) {
			return DateDiffDay(startDate, endDate) * 24 + (endDate.Hour - startDate.Hour);
		}
		public static int? DateDiffHour(DateTime? startDate, DateTime? endDate) {
			if(!startDate.HasValue || !endDate.HasValue)
				return null;
			return DateDiffHour(startDate.Value, endDate.Value);
		}
		public static int DateDiffMinute(DateTime startDate, DateTime endDate) {
			return DateDiffHour(startDate, endDate) * 60 + (endDate.Minute - startDate.Minute);
		}
		public static int? DateDiffMinute(DateTime? startDate, DateTime? endDate) {
			if(!startDate.HasValue || !endDate.HasValue)
				return null;
			return DateDiffMinute(startDate.Value, endDate.Value);
		}
		public static int DateDiffSecond(DateTime startDate, DateTime endDate) {
			return DateDiffMinute(startDate, endDate) * 60 + (endDate.Second - startDate.Second);
		}
		public static int? DateDiffSecond(DateTime? startDate, DateTime? endDate) {
			if(!startDate.HasValue || !endDate.HasValue)
				return null;
			return DateDiffSecond(startDate.Value, endDate.Value);
		}
		public static int DateDiffMillisecond(DateTime startDate, DateTime endDate) {
			return DateDiffSecond(startDate, endDate) * 1000 + (endDate.Millisecond - startDate.Millisecond);
		}
		public static int? DateDiffMillisecond(DateTime? startDate, DateTime? endDate) {
			if(!startDate.HasValue || !endDate.HasValue)
				return null;
			return DateDiffMillisecond(startDate.Value, endDate.Value);
		}
	}
	public class CriteriaToExpressionConverterInternal : IClientCriteriaVisitor<Expression> {
		static Dictionary<Type, bool> CompareToTypes;
		static CriteriaToExpressionConverterInternal() {
			CompareToTypes = new Dictionary<Type, bool>();
			CompareToTypes.Add(typeof(string), true);
			CompareToTypes.Add(typeof(Guid), true);
			CompareToTypes.Add(typeof(Guid?), true);
			CompareToTypes.Add(typeof(bool), true);
			CompareToTypes.Add(typeof(bool?), true);
		}
		public static void RemoveCompareType(Type t) {
			CompareToTypes.Remove(t);
		}
		public static void SetCompareType(Type t, bool v) {
			CompareToTypes[t] = v;
		}
		public event EventHandler<CriteriaToExpressionConverterOnCriteriaArgs> OnFunctionOperator;
		protected static bool IsCompareToExpressions(Type leftType, Type rightType) {
			if(leftType == null)
				return false;
			return CompareToTypes.ContainsKey(leftType);
		}
		protected readonly ICriteriaToExpressionConverter Owner;
		protected readonly Stack<ParameterExpression> ExpressionStack = new Stack<ParameterExpression>();
		protected ParameterExpression ThisExpression;
		public CriteriaToExpressionConverterInternal(ICriteriaToExpressionConverter owner, ParameterExpression thisExpression) {
			Owner = owner;
			ThisExpression = thisExpression;
		}
		public virtual bool ForceIifForInstance {
			get { return false; }
		}
		public Expression Visit(OperandProperty theOperand) {
			return VisitInternal(theOperand);
		}
		protected virtual Expression VisitInternal(OperandProperty theOperand) {
			string propertyName = theOperand.PropertyName;
			Stack<ParameterExpression> tempStack = null;
			while(propertyName.StartsWith("^.")) {
				if(ExpressionStack.Count == 0) throw new InvalidOperationException(theOperand.PropertyName);
				if(tempStack == null) tempStack = new Stack<ParameterExpression>();
				tempStack.Push(ThisExpression);
				ThisExpression = ExpressionStack.Pop();
				propertyName = propertyName.Remove(0, 2);
			}
			try {
				Expression rv = ThisExpression;
				foreach(string prop in propertyName.Split('.')) {
					var propertyExpression = (EvaluatorProperty.GetIsThisProperty(prop) && rv.Type.GetMember(prop).Length == 0) ? rv : Expression.PropertyOrField(rv, prop);
					if(ForceIifForInstance && (!rv.Type.IsValueType() || rv.Type.IsGenericType() && rv.Type.GetGenericTypeDefinition() == typeof(Nullable<>))) {						
						rv = Expression.Condition(Expression.Equal(rv, Expression.Constant(null, rv.Type)), Expression.Default(propertyExpression.Type), propertyExpression);
					} else {
						rv = propertyExpression;
					}
				}
				return rv;
			} finally {
				if(tempStack != null) {
					while(tempStack.Count > 0) {
						ExpressionStack.Push(ThisExpression);
						ThisExpression = tempStack.Pop();
					}
				}
			}
		}
		public Expression Visit(AggregateOperand theOperand) {
			return VisitInternal(theOperand);
		}
		protected virtual Expression ProcessInContext(ParameterExpression thisExpression, CriteriaOperator op) {
			ExpressionStack.Push(ThisExpression);
			ThisExpression = thisExpression;
			try {
				return Process(op);
			} finally {
				ThisExpression = ExpressionStack.Pop();
			}
		}
		protected virtual Expression VisitInternal(AggregateOperand theOperand) {
			if(theOperand.IsTopLevel) throw new NotSupportedException("Top-level aggregates are not supported.");
			Expression property = Process(theOperand.CollectionProperty);
			Type collectionType = property.Type;
			Type rowType = null;
			if(collectionType.IsGenericType()) {
				rowType = collectionType.GetGenericArguments()[0];
			} else if(collectionType.IsArray) {
				rowType = collectionType.GetElementType();
			} else throw new InvalidOperationException(string.Format("The {0} collection has wrong type: {1}", theOperand.CollectionProperty.PropertyName, collectionType));
			var elementParameter = Expression.Parameter(rowType, "elem");
			LambdaExpression argExpr = null;
			if(!ReferenceEquals(null, theOperand.AggregatedExpression)) {
				argExpr = Expression.Lambda(ProcessInContext(elementParameter, theOperand.AggregatedExpression), elementParameter);
			}
			LambdaExpression whereExpr = null;
			if(!ReferenceEquals(null, theOperand.Condition)) {
				whereExpr = Expression.Lambda(ProcessInContext(elementParameter, theOperand.Condition), elementParameter);
			}
			Expression e;
			if(theOperand.AggregateType == Aggregate.Exists) {
				if(whereExpr == null) {
					e = Expression.Call(typeof(Enumerable), "Any", new Type[] { rowType }, property);
				} else {
					e = Expression.Call(typeof(Enumerable), "Any", new Type[] { rowType }, property, whereExpr);
				}
			} else if(theOperand.AggregateType == Aggregate.Count) {
				if(whereExpr == null) {
					e = Expression.Call(typeof(Enumerable), "Count", new Type[] { rowType }, property);
				} else {
					e = Expression.Call(typeof(Enumerable), "Count", new Type[] { rowType }, property, whereExpr);
				}
			} else {
				if(whereExpr != null) {
					property = Expression.Call(typeof(Enumerable), "Where", new Type[] { rowType }, property, whereExpr);
				}
				switch(theOperand.AggregateType) {
					case Aggregate.Max:
						e = Expression.Call(typeof(Enumerable), "Max", new Type[] { rowType, argExpr.Body.Type }, property, argExpr);
						break;
					case Aggregate.Min:
						e = Expression.Call(typeof(Enumerable), "Min", new Type[] { rowType, argExpr.Body.Type }, property, argExpr);
						break;
					case Aggregate.Avg:
						e = Expression.Call(typeof(Enumerable), "Average", new Type[] { rowType }, property, argExpr);
						break;
					case Aggregate.Sum:
						e = Expression.Call(typeof(Enumerable), "Sum", new Type[] { rowType }, property, argExpr);
						break;
					default:
						throw new NotSupportedException(theOperand.AggregateType.ToString());
				}
			}
			return e;			
		}
		public Expression Visit(JoinOperand theOperand) {
			return VisitInternal(theOperand);
		}
		protected virtual Expression VisitInternal(JoinOperand theOperand) {
			throw new NotSupportedException(theOperand.ToString());
		}
		public Expression Visit(FunctionOperator theOperator) {
			if(OnFunctionOperator != null) {
				CriteriaToExpressionConverterOnCriteriaArgs args = new CriteriaToExpressionConverterOnCriteriaArgs(theOperator, Process);
				OnFunctionOperator(this, args);
				if(args.Handled)
					return args.Result;
			}
			return VisitInternal(theOperator);
		}
		protected virtual Expression VisitInternal(FunctionOperator theOperator) {
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.Abs:
					return MakeStaticCall("Abs", typeof(Math), theOperator);
				case FunctionOperatorType.Acos:
					return MakeStaticCall("Acos", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Asin:
					return MakeStaticCall("Asin", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Atn:
					return MakeStaticCall("Atan", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Atn2:
					return MakeStaticCall("Atan2", typeof(Math), theOperator, typeof(double), typeof(double));
				case FunctionOperatorType.BigMul:
					return MakeStaticCall("BigMul", typeof(Math), theOperator, typeof(long), typeof(long));
				case FunctionOperatorType.Ceiling:
					return MakeStaticCall("Ceiling", typeof(Math), theOperator);
				case FunctionOperatorType.Cos:
					return MakeStaticCall("Cos", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Cosh:
					return MakeStaticCall("Cosh", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Exp:
					return MakeStaticCall("Exp", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Floor:
					return MakeStaticCall("Floor", typeof(Math), theOperator);
				case FunctionOperatorType.Log10:
					return MakeStaticCall("Log10", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Log:
					return MakeStaticCall("Log", typeof(Math), theOperator, typeof(double), typeof(double));
				case FunctionOperatorType.Power:
					return MakeStaticCall("Pow", typeof(Math), theOperator, typeof(double), typeof(double));
				case FunctionOperatorType.Round:
					return MakeStaticCall("Round", typeof(Math), theOperator);
				case FunctionOperatorType.Sign:
					return MakeStaticCall("Sign", typeof(Math), theOperator);
				case FunctionOperatorType.Sin:
					return MakeStaticCall("Sin", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Sinh:
					return MakeStaticCall("Sinh", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Tan:
					return MakeStaticCall("Tan", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Tanh:
					return MakeStaticCall("Tanh", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Sqr:
					return MakeStaticCall("Sqrt", typeof(Math), theOperator, typeof(double));
				case FunctionOperatorType.Lower:
					return MakeInstanceCall("ToLower", typeof(string), theOperator, true);
				case FunctionOperatorType.Upper:
					return MakeInstanceCall("ToUpper", typeof(string), theOperator, true);
				case FunctionOperatorType.Len:
					return MakeInstanceMemberAccess("Length", typeof(string), theOperator, true);
				case FunctionOperatorType.ToStr:
					return MakeInstanceCall("ToString", typeof(object), theOperator, true);
				case FunctionOperatorType.ToLong:
					return FnToType(theOperator, typeof(long));
				case FunctionOperatorType.ToInt:
					return FnToType(theOperator, typeof(int));
				case FunctionOperatorType.ToFloat:
					return FnToType(theOperator, typeof(float));
				case FunctionOperatorType.ToDouble:
					return FnToType(theOperator, typeof(double));
				case FunctionOperatorType.ToDecimal:
					return FnToType(theOperator, typeof(decimal));
				case FunctionOperatorType.Trim:
					return MakeInstanceCall("Trim", typeof(string), theOperator, true);
				case FunctionOperatorType.Substring:
					return MakeInstanceCall("Substring", typeof(string), theOperator, true);
				case FunctionOperatorType.StartsWith:
					return MakeInstanceCall("StartsWith", typeof(string), theOperator, true, typeof(string));
				case FunctionOperatorType.EndsWith:
					return MakeInstanceCall("EndsWith", typeof(string), theOperator, true, typeof(string));
				case FunctionOperatorType.Contains:
					return MakeInstanceCall("Contains", typeof(string), theOperator, true, typeof(string));
				case FunctionOperatorType.IsNull:
					return FnIsNull(theOperator);
				case FunctionOperatorType.IsNullOrEmpty:
					return FnIsNullOrEmpty(theOperator);
				case FunctionOperatorType.PadLeft:
					return MakeInstanceCall("PadLeft", typeof(string), theOperator, true);
				case FunctionOperatorType.PadRight:
					return MakeInstanceCall("PadRight", typeof(string), theOperator, true);
				case FunctionOperatorType.CharIndex:
					return FnCharIndex(theOperator);
				case FunctionOperatorType.Insert:
					return MakeInstanceCall("Insert", typeof(string), theOperator, true);
				case FunctionOperatorType.Remove:
					return MakeInstanceCall("Remove", typeof(string), theOperator, true);
				case FunctionOperatorType.Replace:
					return MakeInstanceCall("Replace", typeof(string), theOperator, true);
				case FunctionOperatorType.AddTimeSpan:
					return MakeInstanceCall("Add", typeof(DateTime), theOperator);
				case FunctionOperatorType.AddDays:
					return MakeInstanceCall("AddDays", typeof(DateTime), theOperator, typeof(double));
				case FunctionOperatorType.AddHours:
					return MakeInstanceCall("AddHours", typeof(DateTime), theOperator, typeof(double));
				case FunctionOperatorType.AddMilliSeconds:
					return MakeInstanceCall("AddMilliseconds", typeof(DateTime), theOperator, typeof(double));
				case FunctionOperatorType.AddSeconds:
					return MakeInstanceCall("AddSeconds", typeof(DateTime), theOperator, typeof(double));
				case FunctionOperatorType.AddMinutes:
					return MakeInstanceCall("AddMinutes", typeof(DateTime), theOperator, typeof(double));
				case FunctionOperatorType.AddMonths:
					return MakeInstanceCall("AddMonths", typeof(DateTime), theOperator, typeof(int));
				case FunctionOperatorType.AddYears:
					return MakeInstanceCall("AddYears", typeof(DateTime), theOperator, typeof(int));
				case FunctionOperatorType.AddTicks:
					return MakeInstanceCall("AddTicks", typeof(DateTime), theOperator, typeof(long));
				case FunctionOperatorType.GetDate:
					return MakeInstanceMemberAccess("Date", typeof(DateTime), theOperator);
				case FunctionOperatorType.GetMilliSecond:
					return MakeInstanceMemberAccess("Millisecond", typeof(DateTime), theOperator);
				case FunctionOperatorType.GetSecond:
					return MakeInstanceMemberAccess("Second", typeof(DateTime), theOperator);
				case FunctionOperatorType.GetMinute:
					return MakeInstanceMemberAccess("Minute", typeof(DateTime), theOperator);
				case FunctionOperatorType.GetHour:
					return MakeInstanceMemberAccess("Hour", typeof(DateTime), theOperator);
				case FunctionOperatorType.GetDay:
					return MakeInstanceMemberAccess("Day", typeof(DateTime), theOperator);
				case FunctionOperatorType.GetMonth:
					return MakeInstanceMemberAccess("Month", typeof(DateTime), theOperator);
				case FunctionOperatorType.GetYear:
					return MakeInstanceMemberAccess("Year", typeof(DateTime), theOperator);
				case FunctionOperatorType.GetDayOfWeek:
					return MakeInstanceMemberAccess("DayOfWeek", typeof(DateTime), theOperator);
				case FunctionOperatorType.GetDayOfYear:
					return MakeInstanceMemberAccess("DayOfYear", typeof(DateTime), theOperator);
				case FunctionOperatorType.GetTimeOfDay:
					return MakeInstanceMemberAccess("TimeOfDay", typeof(DateTime), theOperator);
#if !DXPORTABLE
				case FunctionOperatorType.DateDiffDay:
					return MakeStaticCall("DateDiffDay", typeof(System.Data.Linq.SqlClient.SqlMethods), theOperator);
				case FunctionOperatorType.DateDiffHour:
					return MakeStaticCall("DateDiffHour", typeof(System.Data.Linq.SqlClient.SqlMethods), theOperator);
				case FunctionOperatorType.DateDiffMilliSecond:
					return MakeStaticCall("DateDiffMilliSecond", typeof(System.Data.Linq.SqlClient.SqlMethods), theOperator);
				case FunctionOperatorType.DateDiffMinute:
					return MakeStaticCall("DateDiffMinute", typeof(System.Data.Linq.SqlClient.SqlMethods), theOperator);
				case FunctionOperatorType.DateDiffMonth:
					return MakeStaticCall("DateDiffMonth", typeof(System.Data.Linq.SqlClient.SqlMethods), theOperator);
				case FunctionOperatorType.DateDiffSecond:
					return MakeStaticCall("DateDiffSecond", typeof(System.Data.Linq.SqlClient.SqlMethods), theOperator);
				case FunctionOperatorType.DateDiffYear:
					return MakeStaticCall("DateDiffYear", typeof(System.Data.Linq.SqlClient.SqlMethods), theOperator);
#endif
				case FunctionOperatorType.Concat:
					return FnConcat(theOperator);
				case FunctionOperatorType.Now:
					return MakeStaticMemberAccess("Now", typeof(DateTime));
				case FunctionOperatorType.UtcNow:
					return MakeStaticMemberAccess("UtcNow", typeof(DateTime));
				case FunctionOperatorType.Today:
					return MakeStaticMemberAccess("Today", typeof(DateTime));
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
					return Process(new OperandValue(EvalHelpers.EvaluateLocalDateTime(theOperator.OperatorType)));
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
					return Process(EvalHelpers.ExpandIsOutlookInterval(theOperator));
				case FunctionOperatorType.Iif:
					return FnIif(theOperator);
				case FunctionOperatorType.Custom:
				case FunctionOperatorType.CustomNonDeterministic:
					return FnCustom(theOperator);
			}
			throw new NotSupportedException(theOperator.ToString());
		}
		public Expression FnToType(FunctionOperator theOperator, Type type) {
			if(theOperator.Operands == null || theOperator.Operands.Count != 1) {
				throw new NotSupportedException();
			}
			var arg = Process(theOperator.Operands[0]);
			return ConvertToType(arg, type);
		}
		Expression FnCustom(FunctionOperator theOperator) {
			string customFnName = null;
			if(theOperator.Operands.Count >= 1) {
				OperandValue fnNameOv = theOperator.Operands[0] as OperandValue;
				if(!ReferenceEquals(fnNameOv, null)) {
					customFnName = fnNameOv.Value as string;
				}
			}
			return FnCustomCore(customFnName, theOperator.Operands.Skip(1));
		}
		Expression FnCustomCore(string name, IEnumerable<CriteriaOperator> operands) {
			if(string.IsNullOrEmpty(name))
				throw new InvalidOperationException("Custom function had no name");
			var customFunction = CriteriaOperator.GetCustomFunction(name);
			if(customFunction == null)
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Custom function '{0}' not found.", name));
			var customFunctionConvertable = customFunction as ICustomFunctionOperatorConvertibleToExpression;
			if(customFunctionConvertable == null)
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Custom function '{0}' does not implement the ICustomFunctionOperatorConvertableToExpression interface.", name));
			var args = operands.Select(p => Process(p)).ToArray();
			return customFunctionConvertable.Convert(Owner, args);
		}
		Expression FnIif(FunctionOperator theOperator) {
			if(theOperator.Operands.Count < 3 || ((theOperator.Operands.Count % 2) == 0)) throw new NotSupportedException();
			Expression[] args = new Expression[theOperator.Operands.Count];
			for(int i = 0; i < theOperator.Operands.Count; i++) {
				args[i] = Process(theOperator.Operands[i]);
			}
			return FnIif(args, 0);
		}
		Expression FnIif(Expression[] args, int index) {
			Expression condition = args[index];
			Expression ifTrue = args[index + 1];
			if(condition == null) throw new ArgumentNullException("condition");
			if(ifTrue == null) throw new ArgumentNullException("ifTrue");
			if(condition.Type != typeof(bool)) {
				try {
					condition = ConvertToType(condition, typeof(bool));
				} catch(Exception) {
					throw new NotSupportedException();
				}
			}
			Expression ifFalse;
			if((index + 3) < args.Length) {
				ifFalse = FnIif(args, index + 2);
			} else {
				ifFalse = args[index + 2];
			}
			if(ifFalse == null) throw new ArgumentNullException("ifFalse");
			if(ifTrue.Type != ifFalse.Type) {
				if(ifTrue.Type == typeof(object)) {
					if(IsNotNullType(ifFalse.Type)) ifFalse = ConvertToNullable(ifFalse);
					ifTrue = ConvertToType(ifTrue, ifFalse.Type);
				} else if(ifFalse.Type == typeof(object)) {
					if(IsNotNullType(ifTrue.Type)) ifTrue = ConvertToNullable(ifTrue);
					ifFalse = ConvertToType(ifFalse, ifTrue.Type);
				} else {
					Nullabler(ref ifTrue, ref ifFalse);
					if(ifTrue.Type != ifFalse.Type) throw new NotSupportedException();
				}
			}
			return Expression.Condition(condition, ifTrue, ifFalse);
		}
		Expression FnConcat(FunctionOperator theOperator) {
			bool allStrings = true;
			Expression[] args = new Expression[theOperator.Operands.Count];
			for(int i = 0; i < theOperator.Operands.Count; i++) {
				args[i] = Process(theOperator.Operands[i]);
				if(args[i].Type != typeof(string)) {
					allStrings = false;
				}
			}
			return FnConcat(args, allStrings);
		}
		private static Expression FnConcat(Expression[] args, bool allStrings) {
			if(!allStrings) {
				for(int i = 0; i < args.Length; i++) {
					if(args[i].Type != typeof(string)) {
						args[i] = Expression.Call(args[i], "ToString", new Type[0]);
					}
				}
			}			
			Type[] parameters;
			if(args.Length <= 1) throw new NotSupportedException();
			switch(args.Length) {
				case 2:
					parameters = new Type[] { typeof(string), typeof(string) };
					break;
				case 3:
					parameters = new Type[] { typeof(string), typeof(string), typeof(string) };
					break;
				case 4:
					parameters = new Type[] { typeof(string), typeof(string), typeof(string), typeof(string) };
					break;
				default:
					parameters = new Type[] { typeof(string[]) };
					args = new Expression[] { Expression.NewArrayInit(typeof(string), args) };
					break;
			}
			MethodInfo info = GetStringConcatMethod(parameters);
			if (info == null) throw new NotSupportedException();
			return Expression.Call(info, args);
		}
#if DXPORTABLE
		static MethodInfo GetStringConcatMethod(Type[] parameters) {
			foreach (MethodInfo method in typeof(String).GetTypeInfo().DeclaredMethods) {
				if (method.Name == "Concat") {
					ParameterInfo[] methodParameters = method.GetParameters();
					if (methodParameters.Length == parameters.Length)
						if (AreParametersMatch(methodParameters, parameters))
							return method;
				}
			}
			return null;
		}
		static bool AreParametersMatch(ParameterInfo[] methodParameters, Type[] parameters) {
			int count = parameters.Length;
			for (int i = 0; i < count; i++) {
				if (parameters[i] != methodParameters[i].ParameterType)
					return false;
			}
			return true;
		}
#else
		static MethodInfo GetStringConcatMethod(Type[] parameters) {
			return typeof(string).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static, null, CallingConventions.Standard, parameters, null);
		}
#endif
		Expression FnIsNull(FunctionOperator theOperator) {
			switch(theOperator.Operands.Count) {
				case 1:
					return Expression.Equal(Process(theOperator.Operands[0]), Expression.Constant(null));
				case 2:
					return Expression.Coalesce(Process(theOperator.Operands[0]), Process(theOperator.Operands[1]));
			}
			throw new NotSupportedException(theOperator.ToString());
		}
		Expression FnIsNullOrEmpty(FunctionOperator theOperator) {
			if(theOperator.Operands.Count != 1) throw new NotSupportedException(theOperator.ToString());
			Expression arg = Process(theOperator.Operands[0]);
			Expression equals = Expression.Equal(arg, Expression.Constant(null, typeof(string)));
			Expression length = Expression.Equal(MakeInstanceMemberAccessCore("Length", typeof(string), arg), Expression.Constant(0));
			return Expression.OrElse(equals, length);
		}
		public static bool IsNotNullType(Type type) {
			return type.IsValueType() && Nullable.GetUnderlyingType(type) == null;
		}
		public Expression MakeInstanceMemberAccess(string memberName, Type type, FunctionOperator theOperator) {
			return MakeInstanceMemberAccess(memberName, type, theOperator, false);
		}
		public Expression MakeInstanceMemberAccess(string memberName, Type type, FunctionOperator theOperator, bool forceIifForInstance) {
			Expression arg = Process(theOperator.Operands[0]);
			if(Nullable.GetUnderlyingType(arg.Type) != null) {
				Expression valueExpression = MakeInstanceMemberAccessCore("Value", arg.Type, arg);
				Expression memberAccess = MakeInstanceMemberAccessCore(memberName, type, valueExpression);
				Expression nullableMemberAccess = ConvertToType(memberAccess, typeof(Nullable<>).MakeGenericType(memberAccess.Type));
				return Expression.Condition(Expression.Equal(arg, Expression.Constant(null)), arg.Type == nullableMemberAccess.Type ? arg : Expression.Constant(null, nullableMemberAccess.Type), nullableMemberAccess);
			} else {
				Expression memberAccess = MakeInstanceMemberAccessCore(memberName, type, arg);
				if(IsNotNullType(arg.Type) && !(forceIifForInstance && ForceIifForInstance)) {
					return memberAccess;
				} else {
					Expression nullableMemberAccess;
					if(IsNotNullType(memberAccess.Type)) {
						nullableMemberAccess = ConvertToType(memberAccess, typeof(Nullable<>).MakeGenericType(memberAccess.Type));
					} else {
						nullableMemberAccess = memberAccess;
					}
					return Expression.Condition(Expression.Equal(arg, Expression.Constant(null)), Expression.Constant(null, nullableMemberAccess.Type), nullableMemberAccess);
				}
			}
		}
		public MemberExpression MakeInstanceMemberAccessCore(string memberName, Type type, Expression arg) {
			return Expression.MakeMemberAccess(arg, type.GetMember(memberName, BindingFlags.Instance | BindingFlags.Public)[0]);
		}
		public MemberExpression MakeStaticMemberAccess(string memberName, Type type) {
			return Expression.MakeMemberAccess(null, type.GetMember(memberName, BindingFlags.Static | BindingFlags.Public)[0]);
		}
		MethodCallExpression FnCharIndex(FunctionOperator theOperator) {
			if(theOperator.Operands.Count > 1) {
				Expression[] args = new Expression[theOperator.Operands.Count - 1];
				args[0] = Process(theOperator.Operands[0]);
				for(int i = 1; i < theOperator.Operands.Count - 1; i++) {
					args[i] = Process(theOperator.Operands[i + 1]);
				}
				return Expression.Call(Process(theOperator.Operands[1]), "IndexOf", new Type[0], args);
			}
			return Expression.Call(Process(theOperator.Operands[1]), "IndexOf", new Type[0]);
		}
		public Expression MakeInstanceCall(string methodName, Type type, FunctionOperator theOperator, params Type[] argTypes) {
			return MakeInstanceCall(methodName, type, theOperator, false, argTypes);
		}
		protected virtual Expression ConvertToType(Expression instanceExpr, Type type) {
			if(instanceExpr.NodeType == ExpressionType.Convert) {
				UnaryExpression nestedConvert = (UnaryExpression)instanceExpr;
				if(nestedConvert.Operand.NodeType == ExpressionType.Constant) {
					ConstantExpression constant = (ConstantExpression)nestedConvert.Operand;
					if(constant.Value == null) {
						return Expression.Constant(null, type);
					}
				}
			}
			if(type == typeof(string)) {
				Expression result = Expression.Call(instanceExpr, "ToString", new Type[0]);
				if(IsNotNullType(instanceExpr.Type)) {
					return result;
				}
				return ConvertIfArgumentsAreNull(new Expression[] { instanceExpr }, result);
			}
			return Expression.Convert(instanceExpr, type);
		}
		public Expression MakeInstanceCall(string methodName, Type type, FunctionOperator theOperator, bool forceIifForInstance, params Type[] argTypes) {
			List<Expression> nullableExpressions = null;
			Expression instanceExpr = Process(theOperator.Operands[0]);
			if(IsNullable(instanceExpr)) {
				nullableExpressions = new List<Expression>();
				nullableExpressions.Add(instanceExpr);
				instanceExpr = ConvertFromNullable(instanceExpr);
			} else if(forceIifForInstance && ForceIifForInstance && !instanceExpr.Type.IsValueType()) {
				nullableExpressions = new List<Expression>();
				nullableExpressions.Add(instanceExpr);
			}
			Expression[] args = new Expression[theOperator.Operands.Count - 1];
			for(int i = 0; i < theOperator.Operands.Count - 1; i++) {
				args[i] = Process(theOperator.Operands[i + 1]);
				if(IsNullable(args[i])) {
					if(nullableExpressions == null) nullableExpressions = new List<Expression>();
					nullableExpressions.Add(args[i]);
					args[i] = ConvertFromNullable(args[i]);
				}
				if(i < argTypes.Length) {
					if(argTypes[i] != args[i].Type) {
						if(args[i].NodeType == ExpressionType.Constant && ((ConstantExpression)args[i]).Value == null) {
							args[i] = Expression.Constant(null, argTypes[i]);
						} else {
							args[i] = ConvertToType(args[i], argTypes[i]);
						}
					}
				}
			}
			if(type != instanceExpr.Type) {
				instanceExpr = ConvertToType(instanceExpr, type);
			}
			MethodCallExpression result = Expression.Call(instanceExpr, methodName, new Type[0], args);
			if(nullableExpressions == null || nullableExpressions.Count == 0) return result;
			return ConvertIfArgumentsAreNull(nullableExpressions, result);
		}
		public Expression MakeStaticCall(string methodName, Type type, FunctionOperator theOperator, params Type[] argTypes) {
			if(theOperator.Operands.Count > 0) {
				List<Expression> nullableExpressions = null;
				Expression[] args = new Expression[theOperator.Operands.Count];
				for(int i = 0; i < theOperator.Operands.Count; i++) {
					args[i] = Process(theOperator.Operands[i]);
					if(IsNullable(args[i]) && ((i >= argTypes.Length) || IsNotNullType(argTypes[i]))) {
						if(nullableExpressions == null) nullableExpressions = new List<Expression>();
						nullableExpressions.Add(args[i]);
						args[i] = ConvertFromNullable(args[i]);
					}
					if(i < argTypes.Length) {
						if(argTypes[i] != args[i].Type) {
							if(args[i].NodeType == ExpressionType.Constant && ((ConstantExpression)args[i]).Value == null) {
								args[i] = Expression.Constant(null, argTypes[i]);
							} else {
								args[i] = ConvertToType(args[i], argTypes[i]);
							}
						}
					}
				}
				Expression result = Expression.Call(type, methodName, new Type[0], args);
				if(nullableExpressions == null || nullableExpressions.Count == 0) return result;
				return ConvertIfArgumentsAreNull(nullableExpressions, result);
			}
			return Expression.Call(type, methodName, new Type[0]);
		}
		public Expression Visit(OperandValue theOperand) {
			return VisitInternal(theOperand);
		}
		protected virtual Expression VisitInternal(OperandValue theOperand) {
			return Expression.Constant(theOperand.Value);
		}
		public Expression Visit(GroupOperator theOperator) {
			return VisitInternal(theOperator);
		}
		protected virtual Expression VisitInternal(GroupOperator theOperator) {
			Expression result = null;
			foreach(CriteriaOperator op in theOperator.Operands) {
				Expression added = Process(op);
				if(result == null) {
					result = added;
				} else {
					if(theOperator.OperatorType == GroupOperatorType.And) {
						result = Expression.AndAlso(result, added);
					} else {
						result = Expression.OrElse(result, added);
					}
				}
			}
			return result;
		}
		public Expression Visit(InOperator theOperator) {
			return VisitInternal(theOperator);
		}
		protected virtual Expression VisitInternal(InOperator theOperator) {
			CriteriaOperator opa = null;
			foreach(CriteriaOperator val in theOperator.Operands) {
				opa |= theOperator.LeftOperand == val;
			}
			return Process(opa);
		}
		public Expression Visit(UnaryOperator theOperator) {
			return VisitInternal(theOperator);
		}
		protected virtual Expression VisitInternal(UnaryOperator theOperator) {
			Expression oper = Process(theOperator.Operand);
			switch(theOperator.OperatorType) {
				case UnaryOperatorType.IsNull:
					if(oper.Type.IsValueType())
						return Expression.Equal(ConvertToType(oper, typeof(object)), Expression.Constant(null));
					else
						return Expression.Equal(oper, Expression.Constant(null));
				case UnaryOperatorType.Minus:
					return Expression.Negate(oper);
				case UnaryOperatorType.Not:
					return Expression.Not(oper);
				case UnaryOperatorType.Plus:
					return Expression.UnaryPlus(oper);
				default:
					throw new NotSupportedException(theOperator.ToString());
			}
		}
		void Nullabler(ref Expression left, ref Expression right) {
			Type leftUnderlyingType = Nullable.GetUnderlyingType(left.Type);
			Type rightUnderlyingType = Nullable.GetUnderlyingType(right.Type);
			if(leftUnderlyingType == null && rightUnderlyingType != null) {
				if(rightUnderlyingType == left.Type) {
					left = ConvertToType(left, right.Type);
				} else {
					left = ConvertToType(ConvertToType(left, rightUnderlyingType), right.Type);
				}
			} else if(rightUnderlyingType == null && leftUnderlyingType != null) {
				if(leftUnderlyingType == right.Type) {
					right = ConvertToType(right, left.Type);
				} else {
					right = ConvertToType(ConvertToType(right, leftUnderlyingType), left.Type);
				}
			}
		}
		public Expression Visit(BinaryOperator theOperator) {
			return VisitInternal(theOperator);
		}
		protected virtual Expression VisitInternal(BinaryOperator theOperator) {
#pragma warning disable 618
			if(theOperator.OperatorType == BinaryOperatorType.Like)
				return Process(LikeCustomFunction.Convert(theOperator));
#pragma warning restore 618
			Expression left = Process(theOperator.LeftOperand);
			if(!NullableHelpers.CanAcceptNull(left.Type) && theOperator.RightOperand is OperandValue && ((OperandValue)theOperator.RightOperand).Value == null) {
				switch(theOperator.OperatorType){
					case BinaryOperatorType.Equal:
						return Expression.Constant(false);
					case BinaryOperatorType.NotEqual:
						return Expression.Constant(true);
				}
			}
			Expression right = Process(theOperator.RightOperand);
			Nullabler(ref left, ref right);
			switch(theOperator.OperatorType) {
				case BinaryOperatorType.Divide:
					return Expression.Divide(left, right);
				case BinaryOperatorType.Equal:
					if(left.Type != typeof(string) && right.Type == typeof(string) && theOperator.RightOperand is OperandValue) {
						TypeConverter cnv = TypeDescriptor.GetConverter(left.Type);
						if(cnv != null) {
							string str = (string)((OperandValue)theOperator.RightOperand).Value;
							try {
								object converted = cnv.ConvertFromInvariantString(str);
								OperandValue val = new OperandValue(converted);
								right = Process(val);
								Nullabler(ref left, ref right);
							} catch {
								try {
									object converted = cnv.ConvertFromString(str);
									OperandValue val = new OperandValue(converted);
									right = Process(val);
									Nullabler(ref left, ref right);
								} catch { }
							}
						}
					}
					return Expression.Equal(left, right);
				case BinaryOperatorType.Greater:
					if(left.Type == typeof(string))
						return Expression.GreaterThan(Expression.Call(left.Type, "Compare", new Type[0], left, right), Expression.Constant(0));
					if(IsCompareToExpressions(left.Type, right.Type))
						return Expression.GreaterThan(MakeComparerCallExpression(left, right), Expression.Constant(0));
					return Expression.GreaterThan(left, right);
				case BinaryOperatorType.GreaterOrEqual:
					if(left.Type == typeof(string))
						return Expression.GreaterThanOrEqual(Expression.Call(left.Type, "Compare", new Type[0], left, right), Expression.Constant(0));
					if(IsCompareToExpressions(left.Type, right.Type))
						return Expression.GreaterThanOrEqual(MakeComparerCallExpression(left, right), Expression.Constant(0));
					return Expression.GreaterThanOrEqual(left, right);
				case BinaryOperatorType.Less:
					if(left.Type == typeof(string))
						return Expression.LessThan(Expression.Call(left.Type, "Compare", new Type[0], left, right), Expression.Constant(0));
					if(IsCompareToExpressions(left.Type, right.Type))
						return Expression.LessThan(MakeComparerCallExpression(left, right), Expression.Constant(0));
					return Expression.LessThan(left, right);
				case BinaryOperatorType.LessOrEqual:
					if(left.Type == typeof(string))
						return Expression.LessThanOrEqual(Expression.Call(left.Type, "Compare", new Type[0], left, right), Expression.Constant(0));
					if(IsCompareToExpressions(left.Type, right.Type))
						return Expression.LessThanOrEqual(MakeComparerCallExpression(left, right), Expression.Constant(0));
					return Expression.LessThanOrEqual(left, right);
				case BinaryOperatorType.Minus:
					return Expression.Subtract(left, right);
				case BinaryOperatorType.Modulo:
					return Expression.Modulo(left, right);
				case BinaryOperatorType.Multiply:
					return Expression.Multiply(left, right);
				case BinaryOperatorType.NotEqual:
					return Expression.NotEqual(left, right);
				case BinaryOperatorType.Plus:
					if(left.Type == typeof(string) || right.Type == typeof(string)) {
						return FnConcat(new Expression[] { left, right }, (left.Type == typeof(string) && right.Type == typeof(string)));
					}
					return Expression.Add(left, right);
				default:
					throw new NotSupportedException(theOperator.ToString());
			}
		}
		Expression MakeComparerCallExpression(Expression left, Expression right) {
			Type ut = Nullable.GetUnderlyingType(left.Type);
			if(ut != null) {
				left = ConvertToType(left, ut);
				right = ConvertToType(right, ut);
			}
			return Expression.Call(left, "CompareTo", new Type[0], right);
		}
		public Expression Visit(BetweenOperator theOperator) {
			return VisitInternal(theOperator);
		}
		protected virtual Expression VisitInternal(BetweenOperator theOperator) {
			return Process(theOperator.TestExpression >= theOperator.BeginExpression & theOperator.TestExpression <= theOperator.EndExpression);
		}
		public Expression Process(CriteriaOperator op) {
			if(ReferenceEquals(op, null))
				return null;
			return op.Accept(this);
		}
		public static bool IsNullable(Expression exp) {
			if(exp == null) return false;
			Type sourceType = exp.Type;
			return sourceType.IsValueType() && sourceType.IsGenericType() && sourceType.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		public Expression ConvertToNullable(Expression exp) {
			return ConvertToType(exp, typeof(Nullable<>).MakeGenericType(exp.Type));
		}
		public Expression ConvertFromNullable(Expression exp) {
			return ConvertToType(exp, exp.Type.GetGenericArguments()[0]);
		}
		public Expression ConvertIfArgumentsAreNull(IList<Expression> expression, Expression result) {
			Expression condition = Expression.Equal(expression[0], Expression.Constant(null));
			for(int i = 1; i < expression.Count; i++) {
				condition = Expression.OrElse(condition, Expression.Equal(expression[i], Expression.Constant(null)));
			}
			Type sourceType = result.Type;
			if(IsNotNullType(sourceType)) {
				if(sourceType == typeof(bool)) {
					return Expression.Condition(condition, Expression.Constant(false, sourceType), ConvertToType(result, sourceType));
				}
				sourceType = typeof(Nullable<>).MakeGenericType(sourceType);
				return Expression.Condition(condition, Expression.Constant(null, sourceType), ConvertToType(result, sourceType));
			}
			return Expression.Condition(condition, Expression.Constant(null, sourceType), result);
		}
	}
	public class CriteriaToExpressionConverterEventsHelper {
		public event EventHandler<CriteriaToExpressionConverterOnCriteriaArgs> OnFunctionOperator;
		public void Subscribe(CriteriaToExpressionConverterInternal converter) {
			if(converter == null) return;
			SubscribeInternal(converter);
		}
		protected virtual void SubscribeInternal(CriteriaToExpressionConverterInternal converter) {
			converter.OnFunctionOperator += new EventHandler<CriteriaToExpressionConverterOnCriteriaArgs>(FunctionOperatorHandler);
		}
		void FunctionOperatorHandler(object sender, CriteriaToExpressionConverterOnCriteriaArgs e) {
			if(OnFunctionOperator != null) {
				OnFunctionOperator(sender, e);
			}
		}
	}
	public class CriteriaToExpressionConverterOnCriteriaArgs : EventArgs {
		CriteriaOperator criteria;
		Func<CriteriaOperator, Expression> processHandler;
		public CriteriaOperator Criteria {
			get { return criteria; }
		}
		public Func<CriteriaOperator, Expression> ProcessHandler {
			get { return processHandler; }
		}
		public bool Handled { get; set; }
		public Expression Result { get; set; }
		public CriteriaToExpressionConverterOnCriteriaArgs(CriteriaOperator criteria, Func<CriteriaOperator, Expression> processHandler) {
			this.criteria = criteria;
			this.processHandler = processHandler;
		}
	}
	public static class CriteriaToQueryableExtender {
		public static object EvaluateOnInstance(ICriteriaToExpressionConverter converter, Type instanceType, object instance, CriteriaOperator subj) {
			if(instance == null)
				return null;
			try {
				var lst = (System.Collections.IList)Activator.CreateInstance(instanceType.MakeArrayType(), 1);
				lst[0] = instance;
				var en = lst.AsQueryable().MakeSelect(converter, subj).GetEnumerator();
				if(en.MoveNext())
					return en.Current;
			} catch { }
			return null;
		}
		public static IQueryable MakeSelect(this IQueryable src, ICriteriaToExpressionConverter converter, CriteriaOperator subj) {
			var thisParameter = Expression.Parameter(src.ElementType, "");
			Expression expr = converter.Convert(thisParameter, subj);
			LambdaExpression lambda = Expression.Lambda(expr, thisParameter);
			MethodCallExpression callSelect =
				Expression.Call(typeof(Queryable), "Select",
					new Type[] { src.ElementType, lambda.Body.Type }, src.Expression,
					Expression.Quote(lambda));
			return src.Provider.CreateQuery(callSelect);
		}
		public static IEnumerable<object[]> DoSelectSeveral(this IQueryable src, ICriteriaToExpressionConverter converter, CriteriaOperator[] subjs) {
			if(subjs.Length == 1) {
				IQueryable qq = MakeSelect(src, converter, subjs[0]);
				List<object[]> rv = new List<object[]>();
				foreach(object q in qq) {
					rv.Add(new object[] { q });
				}
				return rv;
			} else {
				ParameterExpression thisParameter = Expression.Parameter(src.ElementType, "");
				Expression[] exprs = subjs.Select(s => converter.Convert(thisParameter, s)).ToArray();
				return DoSelectSeveral(src, thisParameter, exprs);
			}
		}
		public static IQueryable MakeSelectThis(this IQueryable src) {
			var thisParameter = Expression.Parameter(src.ElementType, "");
			LambdaExpression lambda = Expression.Lambda(thisParameter, thisParameter);
			MethodCallExpression callSelect =
				Expression.Call(typeof(Queryable), "Select",
					new Type[] { src.ElementType, lambda.Body.Type }, src.Expression,
					Expression.Quote(lambda));
			return src.Provider.CreateQuery(callSelect);
		}
		public static IQueryable AppendWhere(this IQueryable src, ICriteriaToExpressionConverter converter, CriteriaOperator op) {
			if(ReferenceEquals(op, null))
				return src;
			var thisParameter = Expression.Parameter(src.ElementType, "");
			Expression expr = converter.Convert(thisParameter, op);
			Expression lambda = Expression.Quote(Expression.Lambda(expr, thisParameter));
			MethodCallExpression callWhere =
				Expression.Call(typeof(Queryable), "Where",
				new Type[] { src.ElementType }, src.Expression,
				lambda);
			return src.Provider.CreateQuery(callWhere);
		}
		public static IQueryable Take(this IQueryable src, int count) {
			return src.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable), "Take",
					new Type[] { src.ElementType }, src.Expression,
					Expression.Constant(count)));
		}
		public static IQueryable Skip(this IQueryable src, int count) {
			return src.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable), "Skip",
					new Type[] { src.ElementType }, src.Expression,
					Expression.Constant(count)));
		}
		public static IQueryable Cast(this IQueryable src, Type t) {
			return src.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable), "Cast",
					new Type[] { t }, src.Expression));
		}
		public static IQueryable MakeOrderBy(this IQueryable src, ICriteriaToExpressionConverter converter, params ServerModeOrderDescriptor[] orders) {
			var thisParameter = Expression.Parameter(src.ElementType, "");
			IQueryable ordered = src;
			for(int i = 0; i < orders.Length; ++i) {
				ServerModeOrderDescriptor od = orders[i];
				Expression ord = converter.Convert(thisParameter, od.SortExpression);
				string methodName;
				if(i == 0) {
					if(od.IsDesc) methodName = "OrderByDescending";
					else methodName = "OrderBy";
				} else {
					if(od.IsDesc) methodName = "ThenByDescending";
					else methodName = "ThenBy";
				}
				Expression orderedExpression = Expression.Call(typeof(Queryable), methodName,
					new Type[] { src.ElementType, ord.Type },
					ordered.Expression, Expression.Quote(Expression.Lambda(ord, thisParameter)));
				ordered = ordered.Provider.CreateQuery(orderedExpression);
			}
			return ordered;
		}
		public static IQueryable MakeGroupBy(this IQueryable src, ICriteriaToExpressionConverter converter, CriteriaOperator subj) {
			var thisParameter = Expression.Parameter(src.ElementType, "");
			Expression expr = converter.Convert(thisParameter, subj);
			LambdaExpression lambda = Expression.Lambda(expr, thisParameter);
			MethodCallExpression callSelect =
				Expression.Call(typeof(Queryable), "GroupBy",
					new Type[] { src.ElementType, lambda.Body.Type }, src.Expression,
					Expression.Quote(lambda));
			return src.Provider.CreateQuery(callSelect);
		}
		public static bool? UseMS362794Workaround;
		internal static IEnumerable<object[]> DoSelectSummary(this IQueryable grouped, ICriteriaToExpressionConverter converter, Type rowType, IList<ServerModeSummaryDescriptor> summary) {
			bool substituteExceptionalSummaryWithNulls = false;
			var groupParameter = Expression.Parameter(grouped.ElementType, "");
			var elementParameter = Expression.Parameter(rowType, "elem");
			List<Expression> aggregates = new List<Expression>();
			aggregates.Add(Expression.Property(groupParameter, "Key"));
			aggregates.Add(Expression.Call(typeof(Enumerable), "Count", new Type[] { rowType }, groupParameter));
			foreach(ServerModeSummaryDescriptor i in summary) {
				Expression e;
				try {
					LambdaExpression argExpr = null;
					if(!ReferenceEquals(null, i.SummaryExpression)) {
						argExpr = Expression.Lambda(converter.Convert(elementParameter, i.SummaryExpression), elementParameter);
					}
					switch(i.SummaryType) {
						case Aggregate.Count:
							e = Expression.Call(typeof(Enumerable), "Count", new Type[] { rowType }, groupParameter);
							break;
						case Aggregate.Max:
							e = Expression.Call(typeof(Enumerable), "Max", new Type[] { rowType, argExpr.Body.Type }, groupParameter, argExpr);
							break;
						case Aggregate.Min:
							e = Expression.Call(typeof(Enumerable), "Min", new Type[] { rowType, argExpr.Body.Type }, groupParameter, argExpr);
							break;
						case Aggregate.Avg:
							e = Expression.Call(typeof(Enumerable), "Average", new Type[] { rowType }, groupParameter, argExpr);
							break;
						case Aggregate.Sum:
							e = Expression.Call(typeof(Enumerable), "Sum", new Type[] { rowType }, groupParameter, argExpr);
							break;
						default:
							throw new NotSupportedException(i.SummaryType.ToString());
					}
				} catch {
					if(substituteExceptionalSummaryWithNulls) {
						e = Expression.Constant(null);
					} else
						throw;
				}
				aggregates.Add(e);
			}
			return DoSelectSeveral(grouped, groupParameter, aggregates);
		}
		static IEnumerable<object[]> DoSelectSeveral(IQueryable queryable, ParameterExpression parameter, IList<Expression> selectList) {
			if(UseMS362794Workaround.HasValue) {
				if(UseMS362794Workaround.Value) {
					return DoSelectSeveralMS362794Tail(queryable, parameter, selectList);
				} else {
					return DoSelectSeveralArrayTail(queryable, parameter, selectList);
				}
			} else {
				if(selectList.Count > 0 && selectList.Count < SummaryWorkaroundForMS362794.TypesArray.Length) {
					try {
						return DoSelectSeveralMS362794Tail(queryable, parameter, selectList);
					} catch {
						try {
							return DoSelectSeveralArrayTail(queryable, parameter, selectList);
						} catch { }
						throw;
					}
				} else {
					try {
						return DoSelectSeveralArrayTail(queryable, parameter, selectList);
					} catch(NotSupportedException e) {
						if(e.Message.Contains("NewArrayInit") || e.Message.Contains("System.Object"))
							throw new NotSupportedException(
								string.Format("An exception has occurred while requesting {1} columns from the data source. A possible cause of this problem is that too many operands (totals) were requested. Exception text: {0}"
								, e.Message, selectList.Count
								), e);
						throw;
					}
				}
			}
		}
		static IEnumerable<object[]> DoSelectSeveralArrayTail(IQueryable queryable, ParameterExpression parameter, IList<Expression> selectList) {
			var arrayExpr = Expression.NewArrayInit(typeof(object), selectList.Select(expr => Expression.Convert(expr, typeof(object))).ToArray());
			IQueryable query = DoSelectSeveralMakeQuery(queryable, parameter, arrayExpr);
			return ((IQueryable<object[]>)query).ToArray();
		}
		static IEnumerable<object[]> DoSelectSeveralMS362794Tail(IQueryable queryable, ParameterExpression parameter, IList<Expression> selectList) {
			Type genericType = SummaryWorkaroundForMS362794.TypesArray[selectList.Count];
			Type realType = genericType.MakeGenericType(selectList.Select(expr => expr.Type).ToArray());
			List<MemberBinding> bindings = new List<MemberBinding>(selectList.Count);
			for(int i = 0; i < selectList.Count; ++i) {
				Expression expr = selectList[i];
				bindings.Add(Expression.Bind(realType.GetProperty("P" + i.ToString()), expr));
			}
			var cExpr = Expression.MemberInit(Expression.New(realType), bindings);
			IQueryable query = DoSelectSeveralMakeQuery(queryable, parameter, cExpr);
			List<object[]> result = new List<object[]>();
			foreach(MS362794 r in query) {
				result.Add(r.Container);
			}
			return result;
		}
		static IQueryable DoSelectSeveralMakeQuery(IQueryable queryable, ParameterExpression parameter, Expression selectList) {
			LambdaExpression lambda = Expression.Lambda(selectList, parameter);
			MethodCallExpression callSelect =
				Expression.Call(typeof(Queryable), "Select",
					new Type[] { queryable.ElementType, lambda.Body.Type }, queryable.Expression,
					Expression.Quote(lambda));
			IQueryable query = queryable.Provider.CreateQuery(callSelect);
			return query;
		}
		public static int Count(this IQueryable src) {
			return (int)src.Provider.Execute(
				Expression.Call(typeof(Queryable), "Count", new Type[] { src.ElementType }, src.Expression));
		}
	}
}
