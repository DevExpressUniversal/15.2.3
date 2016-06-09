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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.ServerMode.Queryable {
	class CriteriaToExpressionConverterBase : CriteriaToExpressionConverterInternal {
		static readonly Dictionary<Type, List<Type>> impicitConvertDic = new Dictionary<Type, List<Type>>()  {
		   { typeof(sbyte) , new List<Type> { typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
		   { typeof(byte) , new List<Type> { typeof(short), typeof(ushort), typeof(int), typeof(uint),typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
		   { typeof(short), new List<Type> { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
		   { typeof(ushort), new List<Type> { typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
		   { typeof(int) , new List<Type> { typeof(long), typeof(float), typeof(double), typeof(decimal) } },
		   { typeof(uint), new List<Type> { typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
		   { typeof(long ), new List<Type> { typeof(float), typeof(double), typeof(decimal) } },
		   { typeof(char), new List<Type> {   typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
		   { typeof(float), new List<Type> { typeof(double) } },
		   { typeof(ulong), new List<Type> {  typeof(float), typeof(double), typeof(decimal) } },
		   { typeof(Enum), new List<Type> { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } }
		};
		static readonly Dictionary<FunctionOperatorType, Type> typeConvertDic = new Dictionary<FunctionOperatorType, Type>() { 
																							 { FunctionOperatorType.ToDecimal, typeof(decimal) },
																							 { FunctionOperatorType.ToInt, typeof(int) },
																							 { FunctionOperatorType.ToDouble, typeof(double) }
																										};
		static readonly List<Type> compatibleForSummaryTypes = new List<Type>() { typeof(decimal?), typeof(decimal), typeof(double?), typeof(double), typeof(float?), typeof(float), typeof(int?), typeof(int), typeof(long?), typeof(long) };
		readonly IPivotCriteriaToExpressionConverter basic;
		readonly bool useEntity;
		readonly bool useLinq2Sql;
		EntityCriteriaHelper EntityCriteriaHelper { get { return basic.EntityCriteriaHelper; } }
		public CriteriaToExpressionConverterBase(ParameterExpression thisExpression, IPivotCriteriaToExpressionConverter basic, bool useEntity, bool useLinq2Sql)
			: base(basic, thisExpression) {
			this.basic = basic;
			this.useEntity = useEntity;
			this.useLinq2Sql = useLinq2Sql;
		}
		protected override Expression VisitInternal(AggregateOperand theOperand) {
			if(theOperand.IsTopLevel) {
				Type elementType = ThisExpression.Type.GetInterfaces().First((type) => type.IsGenericType() && type.Name == "IEnumerable`1").GetGenericArguments()[0];
				ParameterExpression elementParameter = Expression.Parameter(elementType, "elem");
				Expression expr = ThisExpression;
				if(!ReferenceEquals(null, theOperand.Condition)) {
					ParameterExpression elementParameter2 = Expression.Parameter(elementType, "elem");
					expr = Expression.Call(typeof(Enumerable), "Where", new Type[] { elementType }, expr, Expression.Lambda(
										  basic.Convert(elementParameter2, FixFilter(theOperand.Condition, elementType, basic)),
										  elementParameter2));
				}
				return GetSummaryExpression(expr,
					theOperand.AggregateType == Aggregate.Count || theOperand.AggregateType == Aggregate.Single || theOperand.AggregateType == Aggregate.Exists ? null : Expression.Lambda(
														 basic.Convert(elementParameter, theOperand.AggregatedExpression),
														 elementParameter),
											theOperand.AggregateType,
											elementType, basic.EntityCriteriaHelper, useLinq2Sql);
			} else
				return base.VisitInternal(theOperand);
		}
		public static CriteriaOperator FixFilter(CriteriaOperator filter, Type elementType, IPivotCriteriaToExpressionConverter basic) {
			if(IsLogicalCriteriaChecker.GetBooleanState(filter) == BooleanCriteriaState.Logical) {
				if(ValidateCriteria(filter, elementType, basic) != typeof(bool))
					filter = new FunctionOperator(FunctionOperatorType.Iif, new BinaryOperator(filter, new OperandValue(true), BinaryOperatorType.Equal), new OperandValue(true), new OperandValue(false));
				else
					filter = new FunctionOperator(FunctionOperatorType.Iif, filter, new OperandValue(true), new OperandValue(false));
			}
			return filter;
		}
		public static Type ValidateCriteria(CriteriaOperator criteria, Type elementType, IPivotCriteriaToExpressionConverter basic) {
			try {
				ParameterExpression thisParameter = Expression.Parameter(elementType, "");
				return Expression.Lambda(basic.Convert(thisParameter, criteria), thisParameter).ReturnType;
			} catch {
				return null;
			}
		} 
		public static Expression GetSummaryExpression(Expression queryable, LambdaExpression selectMethod, Aggregate summaryType, Type queryableType, EntityCriteriaHelper helper, bool useLinq) {
			try {
				switch(summaryType) {
					case Aggregate.Count:
						return Expression.Call(typeof(Enumerable), "Count", new Type[] { queryableType }, queryable);
					case Aggregate.Max:
						return Expression.Call(typeof(Enumerable), "Max", new Type[] { queryableType, selectMethod.Body.Type }, queryable, selectMethod);
					case Aggregate.Min:
						return Expression.Call(typeof(Enumerable), "Min", new Type[] { queryableType, selectMethod.Body.Type }, queryable, selectMethod);
					case Aggregate.Avg:
						return Expression.Call(typeof(Enumerable), "Average", new Type[] { queryableType }, queryable, GetLambdaForSumAvgAggregate(selectMethod, helper, useLinq));
					case Aggregate.Sum:
						return Expression.Call(typeof(Enumerable), "Sum", new Type[] { queryableType }, queryable, GetLambdaForSumAvgAggregate(selectMethod, helper, useLinq));
					case Aggregate.Single:
						return Expression.Call(typeof(Enumerable), "Single", new Type[] { queryableType }, queryable);
					case Aggregate.Exists:
					   	return Expression.GreaterThan(GetSummaryExpression(queryable, selectMethod, Aggregate.Count, queryableType, helper, useLinq), Expression.Constant(0));
					default:
						throw new NotSupportedException(summaryType.ToString());
				}
			} catch {
				return Expression.Constant(PivotGridLocalizer.GetString(PivotGridStringId.ValueError));
			}
		}
		static LambdaExpression GetLambdaForSumAvgAggregate(LambdaExpression lambda, EntityCriteriaHelper helper, bool useLinq) {
			if(compatibleForSummaryTypes.Contains(lambda.ReturnType))
				return lambda;
			return Expression.Lambda(ChangeType(lambda.Body, typeof(decimal), helper, useLinq, false), lambda.Parameters);
		}
		protected override Expression VisitInternal(BinaryOperator theOperator) {
			Expression left = Process(theOperator.LeftOperand);
			Expression right = Process(theOperator.RightOperand);
			if(left.Type.IsValueType() && right.Type.IsValueType()) {
				ConvertTypesImplicit(ref left, ref right);
				bool? isNullConv = null;
				Expression expr = null;
				switch(theOperator.OperatorType) {
					case BinaryOperatorType.Equal:
						return Expression.Equal(left, right);
					case BinaryOperatorType.NotEqual:
						return Expression.NotEqual(left, right);
					case BinaryOperatorType.Greater:
						expr = Expression.GreaterThan(left, right);
						isNullConv = true;
						break;
					case BinaryOperatorType.GreaterOrEqual:
						expr = Expression.GreaterThanOrEqual(left, right);
						isNullConv = true;
						break;
					case BinaryOperatorType.Less:
						expr = Expression.LessThan(left, right);
						isNullConv = true;
						break;
					case BinaryOperatorType.LessOrEqual:
						expr = Expression.LessThanOrEqual(left, right);
						isNullConv = true;
						break;
					case BinaryOperatorType.Divide:
						expr = Expression.Divide(left, right);
						isNullConv = false;
						break;
					case BinaryOperatorType.Minus:
						expr = Expression.Subtract(left, right);
						isNullConv = false;
						break;
					case BinaryOperatorType.Modulo:
						expr = Expression.Modulo(left, right);
						isNullConv = false;
						break;
					case BinaryOperatorType.Multiply:
						expr = Expression.Multiply(left, right);
						isNullConv = false;
						break;
					case BinaryOperatorType.Plus:
						expr = Expression.Add(left, right);
						isNullConv = false;
						break;
				}
				if(useLinq2Sql || useEntity) {
					if(Nullable.GetUnderlyingType(right.Type) == null && Nullable.GetUnderlyingType(left.Type) == null)
						return expr;
					if(isNullConv.HasValue && isNullConv.Value) {
						Type nType = isNullConv.Value ? typeof(bool?) : right.Type;
						return Expression.Convert(expr, nType);			  
					}
				}
				return expr;
			}
			if(useEntity && left.Type.IsEnum() && right.Type == typeof(string)) {
				ConstantExpression constExpr = right as ConstantExpression;
				if(constExpr != null) {
					string stringVal = constExpr.Value as string;
					if(stringVal != null) {
						Func<string, bool> lambda = GetEnumValuesLambda(theOperator.OperatorType, stringVal);
						if(lambda != null)
							return CheckEnumValues(left, lambda);
					}
				}
			}
			return base.VisitInternal(theOperator);
		}
		Func<string, bool> GetEnumValuesLambda(BinaryOperatorType opType, string val) {
			Comparer<string> comparer = Comparer<string>.Default;
			switch(opType) {
				case BinaryOperatorType.Equal:
					return (a) => val.Length == a.Length && val.IndexOf(a, StringComparison.CurrentCultureIgnoreCase) == 0;
				case BinaryOperatorType.NotEqual:
					return (a) => val.Length != a.Length || val.IndexOf(a, StringComparison.CurrentCultureIgnoreCase) < 0;
				case BinaryOperatorType.Greater:
					return (a) => comparer.Compare(a, val) > 0;
				case BinaryOperatorType.GreaterOrEqual:
					return (a) => comparer.Compare(a, val) >= 0;
				case BinaryOperatorType.Less:
					return (a) => comparer.Compare(a, val) < 0;
				case BinaryOperatorType.LessOrEqual:
					return (a) => comparer.Compare(a, val) <= 0;
			}
			return null;
		}
		Func<string, bool> GetEnumValuesLambda(FunctionOperatorType fType, string constant) {
			switch(fType) {
				case FunctionOperatorType.StartsWith:
					return (a) => a.StartsWith(constant);
				case FunctionOperatorType.EndsWith:
					return (a) => a.EndsWith(constant);
				case FunctionOperatorType.Contains:
					return (a) => a.IndexOf(constant, StringComparison.CurrentCultureIgnoreCase) >= 0;
				default:
					return null;
			}
		}
		Expression MakeOr(Expression a, Expression b) {
			ConstantExpression consta = a as ConstantExpression;
			ConstantExpression constb = b as ConstantExpression;
			if(consta != null) {
				if(consta.Value is bool && (bool)consta.Value)
					return consta;
				else
					return b;
			}
			if(constb != null) {
				if(constb.Value is bool && (bool)constb.Value)
					return constb;
				else
					return a;
			}
			return Expression.Or(a, b);
		}
		Expression MakeIsNull(Expression exp) {
			if(exp.Type.IsValueType() && Nullable.GetUnderlyingType(exp.Type) == null)
				return Expression.Constant(false);
			ConstantExpression conste = exp as ConstantExpression;
			if(conste != null)
				return Expression.Constant(ReferenceEquals(null, conste.Value));
			return Expression.Equal(exp, Expression.Constant(null, exp.Type));
		}
		void ConvertTypesImplicit(ref Expression left, ref Expression right) {
			if(left.Type == right.Type)
				return;
			Type nullableLeftType = Nullable.GetUnderlyingType(left.Type);
			Type nullableRightType = Nullable.GetUnderlyingType(right.Type);
			bool createNullable = nullableLeftType != null || nullableRightType != null;
			Type resultType = null;
			if(CanImpicitConvertTo(nullableLeftType ?? left.Type, nullableRightType ?? right.Type))
				resultType = right.Type;
			else
				if(CanImpicitConvertTo(nullableRightType ?? right.Type, nullableLeftType ?? left.Type))
					resultType = left.Type;
			if(resultType != null) {
				Type newType = createNullable ? MakeNullable(resultType) : resultType;
				right = ChangeType(right, newType, false);
				left = ChangeType(left, newType, false);
			}
			if(left.Type != right.Type) {
				ConstantExpression constExpr = GetInnerConstant(left);
				if(constExpr != null) {
					left = ConvertConstant(right.Type, constExpr, nullableLeftType);
				} else {
					constExpr = GetInnerConstant(right);
					if(constExpr != null)
						right = ConvertConstant(left.Type, constExpr, nullableLeftType);
				}					
			}
		}
		Type MakeNullable(Type type) {
			if(Nullable.GetUnderlyingType(type) != null)
				return type;
			return typeof(Nullable<>).MakeGenericType(type);
		}
		bool CanImpicitConvertTo(Type from, Type to) {
			List<Type> list;
			if(from.IsEnum())
				from = typeof(Enum);
			return from == to || impicitConvertDic.TryGetValue(from, out list) && list.Contains(to);
		}
		protected override Expression VisitInternal(GroupOperator theOperator) {
			Expression result = null;
			foreach(CriteriaOperator op in theOperator.Operands) {
				Expression added = Process(op);
				if(result == null) {
					result = added;
				} else {
					if(added.Type == typeof(bool) && result.Type == typeof(Nullable<bool>))
						added = Expression.Convert(added, result.Type);
					if(result.Type == typeof(bool) && added.Type == typeof(Nullable<bool>))
						result = Expression.Convert(result, added.Type);
					if(theOperator.OperatorType == GroupOperatorType.And)
						result = Expression.AndAlso(result, added);
					else
						result = Expression.OrElse(result, added);
				}
			}
			return result;
		}
		protected override Expression VisitInternal(FunctionOperator theOperator) {
			Type type;
			if(typeConvertDic.TryGetValue(theOperator.OperatorType, out type))
				return ChangeType(basic.Convert(ThisExpression, theOperator.Operands[0]), type, true);
			if(theOperator.OperatorType == FunctionOperatorType.ToStr) {
				Expression condition = basic.Convert(ThisExpression, theOperator.Operands[0]);
				return ConvertToString(condition, EntityCriteriaHelper);
			}
			if(theOperator.OperatorType == FunctionOperatorType.Custom) {
				IQueryableConvertible conv = CriteriaOperator.GetCustomFunction(((OperandValue)theOperator.Operands[0]).Value as string) as IQueryableConvertible;
				if(conv != null)
					return conv.Convert(theOperator.Operands, ThisExpression, basic);
			}
			if(theOperator.OperatorType == FunctionOperatorType.Floor || theOperator.OperatorType == FunctionOperatorType.Ceiling) {
				Expression expr = basic.Convert(ThisExpression, theOperator.Operands[0]);
				if((Nullable.GetUnderlyingType(expr.Type) ?? expr.Type) == typeof(int))
					return expr;
			}
			if(useEntity) {
				Expression expr = EntityCriteriaHelper.ConvertFunctionOperator(theOperator, (op) => Process(op));
				if(expr != null)
					return expr;
			}
			if(useLinq2Sql)
				if(theOperator.OperatorType == FunctionOperatorType.GetDate)
					return Expression.New(typeof(DateTime).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int) }),
						 this.Process(new FunctionOperator(FunctionOperatorType.GetYear, theOperator.Operands[0])),
						 this.Process(new FunctionOperator(FunctionOperatorType.GetMonth, theOperator.Operands[0])),
						 this.Process(new FunctionOperator(FunctionOperatorType.GetDay, theOperator.Operands[0])));
			if(useEntity && theOperator.Operands.Count == 2) {
				Expression left = basic.Convert(ThisExpression, theOperator.Operands[0]);
				if(left.Type.IsEnum()) {
					OperandValue val = theOperator.Operands[1] as OperandValue;
					if(!ReferenceEquals(val, null)) {
						string strVal = val.Value as string;
						if(strVal != null) {
							Func<string, bool> lambda = GetEnumValuesLambda(theOperator.OperatorType, strVal);
							if(lambda != null)
								return CheckEnumValues(left, lambda);
						}
					}
				}
			}
			if(useEntity || useLinq2Sql) {
				switch(theOperator.OperatorType) {
					case FunctionOperatorType.GetDate:
						return MakeSQLInstanceMemberAccess("Date", typeof(DateTime), theOperator);
					case FunctionOperatorType.GetMilliSecond:
						return MakeSQLInstanceMemberAccess("Millisecond", typeof(DateTime), theOperator);
					case FunctionOperatorType.GetSecond:
						return MakeSQLInstanceMemberAccess("Second", typeof(DateTime), theOperator);
					case FunctionOperatorType.GetMinute:
						return MakeSQLInstanceMemberAccess("Minute", typeof(DateTime), theOperator);
					case FunctionOperatorType.GetHour:
						return MakeSQLInstanceMemberAccess("Hour", typeof(DateTime), theOperator);
					case FunctionOperatorType.GetDay:
						return MakeSQLInstanceMemberAccess("Day", typeof(DateTime), theOperator);
					case FunctionOperatorType.GetMonth:
						return MakeSQLInstanceMemberAccess("Month", typeof(DateTime), theOperator);
					case FunctionOperatorType.GetYear:
						return MakeSQLInstanceMemberAccess("Year", typeof(DateTime), theOperator);
					case FunctionOperatorType.GetDayOfWeek:
						return MakeSQLInstanceMemberAccess("DayOfWeek", typeof(DateTime), theOperator);
					case FunctionOperatorType.GetDayOfYear:
						return MakeSQLInstanceMemberAccess("DayOfYear", typeof(DateTime), theOperator);
					case FunctionOperatorType.GetTimeOfDay:
						return MakeSQLInstanceMemberAccess("TimeOfDay", typeof(DateTime), theOperator);
				}
			}
			if(theOperator.OperatorType == FunctionOperatorType.Iif)
				return MakeIFFunction(theOperator);
			return base.VisitInternal(theOperator);
		}
		Expression MakeSQLInstanceMemberAccess(string memberName, Type type, FunctionOperator theOperator) {
			Expression result = Process(theOperator.Operands[0]);
			bool wasNullable = false;
			if(Nullable.GetUnderlyingType(result.Type) != null) {
				result = MakeInstanceMemberAccessCore("Value", result.Type, result);
				wasNullable = true;
			}
			result = MakeInstanceMemberAccessCore(memberName, type, result);
			return wasNullable ? Expression.Convert(result, typeof(Nullable<>).MakeGenericType(new Type[] { result.Type })) : result;
		}
		Expression MakeIFFunction(FunctionOperator theOperator) {
			if(theOperator.Operands.Count < 3 || ((theOperator.Operands.Count % 2) == 0))
				throw new ArgumentException("Iif function args count must be more than 2");
			Expression[] args = new Expression[theOperator.Operands.Count];
			for(int i = 0; i < theOperator.Operands.Count; i++)
				args[i] = Process(theOperator.Operands[i]);
			if(theOperator.Operands.Count == 3) {
				if(IsLogicalCriteriaChecker.GetBooleanState(theOperator.Operands[0]) != BooleanCriteriaState.Logical) {
					ConstantExpression val1 = args[1] as ConstantExpression;
					ConstantExpression val2 = args[2] as ConstantExpression;
					if(val1 != null && val2 != null) {
						if(object.Equals(val1.Value, true) && object.Equals(val2.Value, false))
							return args[0];
						if(object.Equals(val1.Value, false) && object.Equals(val2.Value, true))
							return Expression.Not(args[0]);
					}
				}
			}
			return MakeIFFunction(args, 0);
		}
		Expression MakeIFFunction(Expression[] args, int index) {
			Expression condition = args[index];
			Expression ifTrue = args[index + 1];
			if(condition == null)
				throw new ArgumentNullException("condition");
			if(ifTrue == null)
				throw new ArgumentNullException("ifTrue");
			if(condition.Type != typeof(bool)) {
				try {
					if(useLinq2Sql)
						condition = Expression.Condition(MakeIsNull(condition), Expression.Constant(false), ChangeType(condition, typeof(bool), false));
					else
						condition = ConvertToType(condition, typeof(bool));
				} catch(Exception) {
					throw new ArgumentException("Can't create make condition with type {0}", condition.Type.ToString());
				}
			}
			Expression ifFalse;
			if((index + 3) < args.Length) {
				ifFalse = MakeIFFunction(args, index + 2);
			} else {
				ifFalse = args[index + 2];
			}
			if(ifFalse == null)
				throw new ArgumentNullException("ifFalse");
			if(ifTrue.Type != ifFalse.Type) {
				if(ifTrue.Type == typeof(object)) {
					if(IsNotNullType(ifFalse.Type)) ifFalse = ConvertToNullable(ifFalse);
					ifTrue = ConvertToType(ifTrue, ifFalse.Type);
				} else if(ifFalse.Type == typeof(object)) {
					if(IsNotNullType(ifTrue.Type)) ifTrue = ConvertToNullable(ifTrue);
					ifFalse = ConvertToType(ifFalse, ifTrue.Type);
				} else {
					ConvertTypesImplicit(ref ifTrue, ref ifFalse);
					if(ifTrue.Type != ifFalse.Type)
						throw new NotSupportedException(string.Format("ifTrue type {0} is incompatible with ifFalse type {1}", ifTrue.Type.FullName, ifFalse.Type.FullName));
				}
			}
			return Expression.Condition(condition, ifTrue, ifFalse);
		}
		class EnumValuesChecker<TType> {
			public EnumValuesChecker() {
			}
			public Expression Write(Expression expr, List<object> values, bool useContains) {
				List<TType> res = new List<TType>(values.Count);
				foreach(object val in values)
					res.Add((TType)val);
				if(res.Count == 0)
					return Expression.Equal(Expression.Constant(true), Expression.Constant(false));
				if(res.Count == 1)
					return Expression.Equal(expr, Expression.Constant(res[0]));
				if(useContains)
					return Expression.Call(Expression.Constant(res), res.GetType().GetMethod("Contains"), new Expression[] { expr });
				else {
					Expression result = Expression.Equal(expr, Expression.Constant(res[0]));
					for(int i = 1; i < res.Count; i++)
						result = Expression.Or(result, Expression.Equal(expr, Expression.Constant(res[i])));
					return result;
				}
			}
		}
		Expression CheckEnumValues(Expression expr, Func<string, bool>  lambda){ 
			return CreateInExpression(expr, Enum.GetValues(expr.Type).Cast<object>().Where((a) => lambda(Enum.GetName(expr.Type, a))).ToList<object>());
		}
		Expression CreateInExpression(Expression expr, List<object> values) {
			Type type = typeof(EnumValuesChecker<>).MakeGenericType(new Type[] { expr.Type });			
			return (Expression)type.GetMethod("Write").Invoke(Activator.CreateInstance(type), new object[] { expr, values, EntityCriteriaHelper == null || EntityCriteriaHelper.ListContainsSupported });
		}
		static Expression ConvertToString(Expression expr, EntityCriteriaHelper helper) {
			if(expr.Type == typeof(string))
				return expr;
			Expression rc;
			if(helper != null) {
				rc = helper.ConvertExpressionToString(expr);
				if(rc == null)
					rc = Expression.Convert(rc, typeof(string));
			} else
				rc = Expression.Call(
												 Expression.Convert(expr, typeof(object)),
													 typeof(object).GetMethod("ToString"));
			if(!expr.Type.IsValueType() || Nullable.GetUnderlyingType(expr.Type) != null) {
				return Expression.Condition(
									Expression.Equal(expr, Expression.Constant(null)),
									Expression.Constant(null, typeof(string)),
									rc,
									typeof(string));
			} else {
				return rc;
			}
		}
		protected override Expression VisitInternal(InOperator theOperator) {
			Expression left = Process(theOperator.LeftOperand);
			List<object> values = new List<object>();
			for(int i = 0; i < theOperator.Operands.Count; i++) {
				OperandValue val = theOperator.Operands[i] as OperandValue;
				if(ReferenceEquals(null, val))
					return base.VisitInternal(theOperator);
				object itemValue = val.Value;
				if(itemValue != null && itemValue.GetType() != left.Type || itemValue == null && left.Type.IsValueType())
					try {
						itemValue = DevExpress.PivotGrid.QueryMode.Sorting.BaseComparer.ChangeType(itemValue, left.Type, itemValue.GetType());
					} catch { }
				values.Add(itemValue);
			}
			return CreateInExpression(left, values);
		}
		protected override Expression ConvertToType(Expression instanceExpr, Type type) {
			if(type == typeof(string))
				return ConvertToString(instanceExpr, EntityCriteriaHelper);
			return base.ConvertToType(instanceExpr, type);
		}
		internal Expression ChangeType(Expression expr, Type type, bool strong) {
			return ChangeType(expr, type, EntityCriteriaHelper, useLinq2Sql, strong);
		}
		internal static Expression ChangeType(Expression expr, Type type, EntityCriteriaHelper helper, bool isLinq, bool strong) {
			ConstantExpression constExpr = GetInnerConstant(expr);
			if(constExpr != null && constExpr.Value == null)
				return Expression.Constant(null, type);
			if(expr.Type == type)
				return expr;
			if(type == typeof(string))
				return ConvertToString(expr, helper);
			Type toUnderlying = Nullable.GetUnderlyingType(type);
			if(constExpr != null) {
				 Expression convertedConst = ConvertConstantCatched(type, constExpr, toUnderlying, true);
				if(convertedConst != null)
					return convertedConst;
			}
			Type undelying = Nullable.GetUnderlyingType(expr.Type);
			if(undelying == null)
				return Expression.Convert(expr, type);
			Expression resultExpression;
			if(undelying == type)
				if(undelying == null)
					resultExpression = expr;
				else
					resultExpression = Expression.Convert(expr, type);
			else
				if(toUnderlying == null)
					resultExpression = Expression.Convert(expr, type);
				else
					resultExpression = Expression.Convert(Expression.Convert(expr, toUnderlying), type);
			if(!strong && (helper != null || isLinq)) {
				Type from = undelying ?? expr.Type;
				Type to = toUnderlying ?? type;
				if(from.IsValueType() && to.IsValueType()) {
					return Expression.Convert(expr, type);
				}
			}
			return Expression.Condition(
											 Expression.Equal(expr, Expression.Constant(null)),
											 Expression.Constant(toUnderlying == null ? Convert.ChangeType(0, type) : null, type),
											 resultExpression,
											 type
										);
		}
		static Expression ConvertConstantCatched(Type type, ConstantExpression constExpr, Type toUnderlying, bool catched) {
			try {
				return ConvertConstant(type, constExpr, toUnderlying);
			} catch {
				return null;
			}
		}
		static Expression ConvertConstant(Type type, ConstantExpression constExpr, Type toUnderlying) {
			return Expression.Constant(constExpr.Value == null ? null : Convert.ChangeType(constExpr.Value, toUnderlying ?? type), type);
		}
		static ConstantExpression GetInnerConstant(Expression expr) {
			ConstantExpression constExpr = expr as ConstantExpression;
			if(constExpr != null)
				return constExpr;
			if(expr.NodeType == ExpressionType.Convert)
				return GetInnerConstant(((UnaryExpression)expr).Operand);
			return null;
		}
	}
	interface IPivotCriteriaToExpressionConverter : ICriteriaToExpressionConverter {
		int? DateFirst { get; set; }
		EntityCriteriaHelper EntityCriteriaHelper { get; }
		bool UseLinq2Sql { get; }
		void EnsureServerConstants(IEnumerable<CriteriaOperator> criterias, IQueryable queryable);
	}
	class CriteriaToExpressionConverter : IPivotCriteriaToExpressionConverter {
		readonly DevExpress.Data.Linq.CriteriaToExpressionConverter converter = new DevExpress.Data.Linq.CriteriaToExpressionConverter();
		readonly EntityCriteriaHelper entityCriteriaHelper;
		readonly bool useEntity;
		readonly bool useLinq2Sql;
		public int? DateFirst {
			get { return entityCriteriaHelper.DateFirst; }
			set { entityCriteriaHelper.DateFirst = value; }
		}
		public bool UseLinq2Sql { get { return useLinq2Sql; } }
		public CriteriaToExpressionConverter(bool useEntity, bool useLinq2Sql, Assembly assembly) {
			this.useEntity = useEntity;
			this.useLinq2Sql = useLinq2Sql;
			if(useEntity)
				entityCriteriaHelper = EntityCriteriaHelper.Create(assembly);
		}
		Expression ICriteriaToExpressionConverter.Convert(ParameterExpression thisExpression, CriteriaOperator op) {
			return new CriteriaToExpressionConverterBase(thisExpression, this, useEntity, useLinq2Sql).Process(op);
		}
		EntityCriteriaHelper IPivotCriteriaToExpressionConverter.EntityCriteriaHelper { get { return entityCriteriaHelper; } }
		public void EnsureServerConstants(IEnumerable<CriteriaOperator> criterias, IQueryable queryable) {
			entityCriteriaHelper.EnsureServerConstants(criterias, queryable, this);
		}
	}
}
