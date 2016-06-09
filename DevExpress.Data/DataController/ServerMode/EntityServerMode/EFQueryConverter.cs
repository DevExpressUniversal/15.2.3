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
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Reflection;
namespace DevExpress.Data.Linq {
	using DevExpress.Data.Linq.Helpers;
	public class CriteriaToEFExpressionConverter : ICriteriaToExpressionConverter, ICriteriaToExpressionConverterCustomizable {
		public static Type EntityFunctionsType = typeof(EntityFunctions);
		public static Type SqlFunctionsType = typeof(SqlFunctions);
		public Expression Convert(ParameterExpression thisExpression, CriteriaOperator op) {
			return new CriteriaToEFExpressionConverterInternal(this, thisExpression).Process(op);
		}
		public Expression Convert(ParameterExpression thisExpression, CriteriaOperator op, CriteriaToExpressionConverterEventsHelper eventsHelper) {
			CriteriaToEFExpressionConverterInternal converter = new CriteriaToEFExpressionConverterInternal(this, thisExpression);
			if(eventsHelper != null) {
				eventsHelper.Subscribe(converter);
			}
			return converter.Process(op);			
		}
	}
}
namespace DevExpress.Data.Linq.Helpers {
	public class CriteriaToEFExpressionConverterInternal : CriteriaToExpressionConverterInternal {
		public CriteriaToEFExpressionConverterInternal(ICriteriaToExpressionConverter owner, ParameterExpression thisExpression)
			: base(owner, thisExpression) {
		}
		static Type EntityFunctionsType {
			get { return CriteriaToEFExpressionConverter.EntityFunctionsType ?? typeof(EntityFunctions); }
		}
		internal static Type SqlFunctionsType {
			get { return CriteriaToEFExpressionConverter.SqlFunctionsType ?? typeof(SqlFunctions); }
		}
		protected override Expression VisitInternal(UnaryOperator theOperator) {
			if (theOperator.OperatorType == UnaryOperatorType.IsNull) {
				Expression oper = Process(theOperator.Operand);
				if (IsNotNullType(oper.Type))
					return Expression.Equal(ConvertToNullable(oper), Expression.Constant(null));
				else
					return Expression.Equal(oper, Expression.Constant(null));
			}
			return base.VisitInternal(theOperator);
		}
		protected override Expression ConvertToType(Expression instanceExpr, Type type) {
			if (type == typeof(string)) {
				Type underlyingType = instanceExpr.Type.IsValueType ? Nullable.GetUnderlyingType(instanceExpr.Type) : null;
				TypeCode code = Type.GetTypeCode(underlyingType ?? instanceExpr.Type);
				switch (code) {
					case TypeCode.Double:
					case TypeCode.Single: {
							MethodInfo convertMi = SqlFunctionsType.GetMethod("StringConvert", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(double?), typeof(int?) }, null);
							MethodInfo trimMi = typeof(string).GetMethod("Trim", BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
							return Expression.Call(Expression.Call(convertMi, Expression.Convert(instanceExpr, typeof(double?)), Expression.Constant((int?)19, typeof(int?))), trimMi);
						}
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.UInt16:
					case TypeCode.UInt32:
					case TypeCode.UInt64:
					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Decimal: {
							MethodInfo convertMi = SqlFunctionsType.GetMethod("StringConvert", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(decimal?), typeof(int?) }, null);
							MethodInfo trimMi = typeof(string).GetMethod("Trim", BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
							return Expression.Call(Expression.Call(convertMi, Expression.Convert(instanceExpr, typeof(decimal?)), Expression.Constant((int?)19, typeof(int?))), trimMi);
						}
					case TypeCode.String: {
							return instanceExpr;
						}
				}
			}
			return base.ConvertToType(instanceExpr, type);
		}
		public Expression FnToStr(FunctionOperator theOperator) {
			if (ReferenceEquals(theOperator, null) || theOperator.Operands.Count != 1) return base.VisitInternal(theOperator);
			return ConvertToType(Process(theOperator.Operands[0]), typeof(string));
		}
		protected override Expression VisitInternal(FunctionOperator theOperator) {
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.ToStr:
					return FnToStr(theOperator);
				case FunctionOperatorType.AddDays:
					return MakeStaticCall("AddDays", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(int?));
				case FunctionOperatorType.AddHours:
					return MakeStaticCall("AddHours", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(int?));
				case FunctionOperatorType.AddMilliSeconds:
					return MakeStaticCall("AddMilliseconds", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(int?));
				case FunctionOperatorType.AddSeconds:
					return MakeStaticCall("AddSeconds", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(int?));
				case FunctionOperatorType.AddMinutes:
					return MakeStaticCall("AddMinutes", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(int?));
				case FunctionOperatorType.AddMonths:
					return MakeStaticCall("AddMonths", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(int?));
				case FunctionOperatorType.AddYears:
					return MakeStaticCall("AddYears", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(int?));
				case FunctionOperatorType.GetDate:
					return FnGetDate(theOperator);
#if !SL //TODO SL
				case FunctionOperatorType.DateDiffDay:
					return MakeStaticCall("DiffDays", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(DateTime?));
				case FunctionOperatorType.DateDiffHour:
					return MakeStaticCall("DiffHours", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(DateTime?));
				case FunctionOperatorType.DateDiffMilliSecond:
					return MakeStaticCall("DiffMilliSeconds", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(DateTime?));
				case FunctionOperatorType.DateDiffMinute:
					return MakeStaticCall("DiffMinutes", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(DateTime?));
				case FunctionOperatorType.DateDiffMonth:
					return MakeStaticCall("DiffMonths", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(DateTime?));
				case FunctionOperatorType.DateDiffSecond:
					return MakeStaticCall("DiffSeconds", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(DateTime?));
				case FunctionOperatorType.DateDiffYear:
					return MakeStaticCall("DiffYears", EntityFunctionsType, theOperator, typeof(DateTime?), typeof(DateTime?));
#endif
				default:
					break;
			}
			return base.VisitInternal(theOperator);
		}
		public Expression FnGetDate(FunctionOperator theOperator) {
			if (theOperator.Operands.Count != 1) throw new ArgumentException();
			CriteriaOperator operand = theOperator.Operands[0];
			Expression argument = Process(operand);
			return Expression.Call(EntityFunctionsType, "TruncateTime", new Type[0], new Expression[] { Expression.Convert(argument, typeof(DateTime?)) });
		}
	}
}
