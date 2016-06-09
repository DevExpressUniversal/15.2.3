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
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP {
	public class QueryCriteriaOptimizer : CriteriaPatcherBase {
		OLAPMetadataColumn column;
		public static CriteriaOperator Optimize(CriteriaOperator criteria, OLAPMetadataColumn column) {
			return new QueryCriteriaOptimizer(column).Process(CriteriaOperator.Clone(criteria));
		}
		public QueryCriteriaOptimizer(OLAPMetadataColumn column) {
			this.column = column;
		}
		public override CriteriaOperator Visit(InOperator theOperator) {
			CriteriaOperator left = Process(theOperator.LeftOperand);
			if(ReferenceEquals(left, null))
				return null;
			IEnumerable<CriteriaOperator> operands = ProcessAny(theOperator.Operands);
			if(operands == null)
				return null;
			OperandProperty prop = left as OperandProperty;
			if(object.ReferenceEquals(null, prop) || prop.PropertyName != column.UniqueName)
				return new InOperator(left, operands);
			bool propValue = true;
			foreach(CriteriaOperator op in operands) {
				OperandValue opValue = op as OperandValue;
				if(ReferenceEquals(null, opValue)) {
					propValue = false;
					break;
				}
				if(!(opValue.Value is string)) {
					propValue = false;
					break;
				}
			}
			if(!propValue)
				return new InOperator(left, operands);
			prop.PropertyName = prop.PropertyName + ".Self";
			foreach(CriteriaOperator op in operands) {
				OperandValue opValue = (OperandValue)op;
				opValue.Value = column.GetMemberByUniqueLevelValue((string)opValue.Value);
			}
			return new InOperator(left, operands);
		}
	}
	public class QueryFilterVisitor : IClientCriteriaVisitor<Expression> {
		QueryGroupFilterEvaluator evaluator;
		CriteriaOperator criteria;
		OLAPMetadataColumn column;
		QueryContextCache contextCache;
		Func<bool?> action;
		public QueryContextCache ContextCache { get { return contextCache; } }
		public QueryFilterVisitor(QueryGroupFilterEvaluator evaluator, CriteriaOperator criteria, OLAPMetadataColumn column) {
			this.evaluator = evaluator;
			this.criteria = QueryCriteriaOptimizer.Optimize(criteria, column);
			this.column = column;
		}
		public bool? Fit(QueryContextCache contextCache) {
			if(ReferenceEquals(criteria, null))
				return true;
			this.contextCache = contextCache;
			if(action == null) {
				Expression expr = criteria.Accept(this);
				LabelTarget returnTarget = Expression.Label(typeof(bool?), "ret");
				ParameterExpression retVal = Expression.Variable(typeof(bool?), "retVal");
				expr = Expression.Block(typeof(bool?),
										new ParameterExpression[] { retVal },
										Expression.Assign(retVal, Expression.TypeAs(expr, typeof(bool?))),
										retVal
				);
				action = Expression.Lambda<Func<bool?>>(expr).Compile();
			}
			return action();
		}
		Dictionary<string, Expression> propCache = new Dictionary<string,Expression>();
		Expression IClientCriteriaVisitor<Expression>.Visit(OperandProperty theOperand) {
			Expression result;
			if(propCache.TryGetValue(theOperand.PropertyName, out result))
				return result;
			string path;
			string suffix;
			QueryMemberEvaluatorBase.GetPathAndSuffix(EvaluatorProperty.Create(theOperand).PropertyPath, out path, out suffix);
			Expression queryContextCache = Expression.Property(Expression.Constant(this), "ContextCache");
			ParameterExpression param = Expression.Variable(typeof(IOLAPMember));
			LabelTarget returnTarget = Expression.Label(typeof(object));
			ParameterExpression retVar = Expression.Variable(typeof(object), "retVar");
			result = Expression.Block(typeof(object),
				new ParameterExpression[] { retVar, param },
				Expression.IfThenElse(
											  Expression.Call(queryContextCache, "TryGetValue", null, new Expression[] { Expression.Constant(path), param }),
											  Expression.Assign(retVar, Expression.Convert(GetProp(suffix, param), typeof(object))),
											  Expression.Assign(retVar, Expression.Constant(OLAPQQueryMemberEvaluatorBase.ErrorValue, typeof(object)))
											), retVar);
			propCache[theOperand.PropertyName] = result;
			return result;
		}
		Expression GetProp(string suffix, Expression param) {
			if(suffix == null)
				return Expression.Property(Expression.Convert(param, typeof(IQueryMember)), "UniqueLevelValue");
			else
				if(suffix == "Self")
					return param;
				else
					if(suffix == "Value")
						return Expression.Property(Expression.Convert(param, typeof(IQueryMember)), "Value");
					else
						if(suffix == "Caption")
							return Expression.Property(Expression.Convert(param, typeof(IOLAPTitledEntity)), "Caption");
						else
							return Expression.Constant(OLAPQQueryMemberEvaluatorBase.ErrorValue);
		}
		Expression ICriteriaVisitor<Expression>.Visit(OperandValue theOperand) {
			return Expression.Constant(theOperand.Value);
		}
		Expression ICriteriaVisitor<Expression>.Visit(GroupOperator theOperator) {
			int count = theOperator.Operands.Count;
			if(count == 0)
				return Expression.Constant(null, typeof(object));
			Expression[] ops = new Expression[theOperator.Operands.Count];
			for(int i = 0; i < theOperator.Operands.Count; i++)
				ops[i] = Expression.Parameter(typeof(object), "val" + i);
			List<Expression> block = new List<Expression>();
			LabelTarget returnTarget = Expression.Label(typeof(bool?));
			ParameterExpression result = Expression.Variable(typeof(bool?), "result");
			block.Add(Expression.Assign(result, Expression.Constant((bool?)(theOperator.OperatorType == GroupOperatorType.And), typeof(bool?))));
			for(int i = 0; i < theOperator.Operands.Count; i++) {
				CriteriaOperator value = theOperator.Operands[i];
				Expression res = Process(value);
				if(res.Type != typeof(bool?))
					res = Expression.TypeAs(res, typeof(bool?));
				Expression testExpr;
				if(theOperator.OperatorType == GroupOperatorType.Or)
					testExpr = Expression.Call(typeof(QueryFilterVisitor).GetMethod("OpOr"), res, result);
				else
					testExpr = Expression.Call(typeof(QueryFilterVisitor).GetMethod("OpAnd"), res, result);
				if(i == theOperator.Operands.Count - 1)
					block.Add(testExpr);
				else
					block.Add(Expression.IfThen(testExpr,
												Expression.Return(returnTarget, result)));
			}
			block.Add(Expression.Label(returnTarget, result));
			return Expression.Block(typeof(bool?),
									new ParameterExpression[] { result },
									block);
		}
		public static bool OpAnd(bool? op1, ref bool? result) {
			if(op1 == false) {
				result = false;
				return true;
			}
			if(op1 == null) {
				result = null;
				return true;
			}
			return false;
		}
		public static bool OpOr(bool? op1, ref bool? result) {
			if(op1 == true) {
				result = true;
				return true;
			}
			if(op1 == null) {
				result = null;
			}
			return false;
		}
		Expression ICriteriaVisitor<Expression>.Visit(InOperator theOperator) {
			OperandProperty property = theOperator.LeftOperand as OperandProperty;
			Expression val = Process(theOperator.LeftOperand);
			bool simplify = !object.ReferenceEquals(property, null) && property.PropertyName.EndsWith(".Self");
			NullableDictionary<object, bool> dic = new NullableDictionary<object, bool>();
			foreach(CriteriaOperator op in theOperator.Operands)
				dic[((OperandValue)op).Value] = true;
			LabelTarget returnTarget = Expression.Label(typeof(object));
			ParameterExpression val0 = Expression.Variable(typeof(object), "val");
			return Expression.Block(typeof(object),
				new ParameterExpression[] { val0 },
				Expression.Assign(val0, val),
				Expression.IfThen(Expression.ReferenceEqual(val0, Expression.Constant(QueryGroupFilterEvaluator.ErrorValue)),
						 Expression.Return(returnTarget, Expression.Constant(null, typeof(object)))),
				Expression.Return(returnTarget, Expression.Convert(Expression.Call(Expression.Constant(dic), "ContainsKey", null, new Expression[] { val0 }), typeof(object))),
				Expression.Label(returnTarget, Expression.Constant(null, typeof(object)))
			);
		}
		Expression ICriteriaVisitor<Expression>.Visit(UnaryOperator theOperator) {
			if(theOperator.OperatorType != UnaryOperatorType.Not)
				throw new NotImplementedException(); 
			Expression result = Process(theOperator.Operand);
			ParameterExpression var1 = Expression.Variable(typeof(bool?), "var1");
			LabelTarget returnTarget = Expression.Label(typeof(object));
			return Expression.Block(
									typeof(object),
									new ParameterExpression[] { var1 },
									Expression.Assign(var1, Expression.TypeAs(result, typeof(bool?))),
								   Expression.IfThenElse(
													Expression.Equal(Expression.Constant(true, typeof(bool?)), var1),
													Expression.Return(returnTarget, Expression.Constant(false, typeof(object))),
													Expression.IfThenElse(
																Expression.Equal(Expression.Constant(false, typeof(bool?)), var1),
																Expression.Return(returnTarget, Expression.Constant(true, typeof(object))),
																Expression.Return(returnTarget, Expression.Constant(null, typeof(object))))),
								   Expression.Label(returnTarget, Expression.Constant(null, typeof(object)))
							);
		}
		Expression ICriteriaVisitor<Expression>.Visit(BinaryOperator theOperator) {
			if(theOperator.OperatorType == BinaryOperatorType.Equal) {
				OperandValue valOp = theOperator.RightOperand as OperandValue;
				if(!ReferenceEquals(null, valOp)) {
					OperandProperty propOp = theOperator.LeftOperand as OperandProperty;
					if(!ReferenceEquals(null, propOp))
						return ComparePropertyWithConst(valOp, propOp);
				}
			}
			Expression val = Process(theOperator.LeftOperand);
			Expression result = Process(theOperator.RightOperand);
			ParameterExpression retVal = Expression.Variable(typeof(object), "retVal");
			val = Convert(val, typeof(object));
			result = Convert(result, typeof(object));
			LabelTarget returnTarget = Expression.Label(typeof(bool?));
			switch(theOperator.OperatorType) {
				case BinaryOperatorType.Equal: {
						ParameterExpression in1 = Expression.Variable(typeof(object), "in1");
						ParameterExpression in2 = Expression.Variable(typeof(object), "in2");
						List<Expression> block = new List<Expression>();
						block.Add(Expression.Assign(in1, val));
						block.Add(Expression.Assign(in2, result));
						block.Add(Expression.IfThen(Expression.Equal(Expression.Constant(0), Expression.Call(typeof(EvalHelpers), "CompareObjects", null, new Expression[] {  in1, 
																															  in2,
																															 Expression.Constant(true),
																															 Expression.Constant(false), 
																															 Expression.Constant(null, typeof(IComparer)) })),
													 Expression.Return(returnTarget, Expression.Constant(true, typeof(bool?)))));
						if(!(result is ConstantExpression))
							block.Add(Expression.IfThen(Expression.ReferenceEqual(in2, Expression.Constant(QueryGroupFilterEvaluator.ErrorValue)),
												  Expression.Return(returnTarget, Expression.Constant(null, typeof(bool?)))));
						if(!(val is ConstantExpression))
							block.Add(Expression.IfThen(Expression.ReferenceEqual(in1, Expression.Constant(QueryGroupFilterEvaluator.ErrorValue)),
												  Expression.Return(returnTarget, Expression.Constant(null, typeof(bool?)))));
						block.Add(Expression.Return(returnTarget, Expression.Constant(false, typeof(bool?))));
						block.Add(Expression.Label(returnTarget, Expression.Constant(null, typeof(bool?))));
						return Expression.Block(
							typeof(bool?),
							new ParameterExpression[] { retVal, in1, in2 },
							block
						 );
					}
				case BinaryOperatorType.Greater:
					return CompareByExpression(val, result, (a) => Expression.GreaterThan(a, Expression.Constant(0))); 
				case BinaryOperatorType.Less:
					return CompareByExpression(val, result, (a) => Expression.LessThan(a, Expression.Constant(0))); 
				case BinaryOperatorType.LessOrEqual:
					return CompareByExpression(val, result, (a) => Expression.LessThanOrEqual(a, Expression.Constant(0))); 
				case BinaryOperatorType.GreaterOrEqual:
					return CompareByExpression(val, result, (a) => Expression.GreaterThanOrEqual(a, Expression.Constant(0))); 
				default:
					throw new NotImplementedException();
			}
		}
		Expression ComparePropertyWithConst(OperandValue opVal, OperandProperty prop) {
			string path;
			string suffix;
			Expression valExpr = opVal.Value == null ? Expression.Constant(null, typeof(object)) : Expression.Constant(opVal.Value);
			QueryMemberEvaluatorBase.GetPathAndSuffix(EvaluatorProperty.Create(prop).PropertyPath, out path, out suffix);
			return Expression.Call(
									Expression.Constant(this),
								 	"ComparePropertyWithConstExpr" + suffix,
									new Type[] { valExpr.Type },
									new Expression[] {	
														Expression.Constant(prop.PropertyName), 
														valExpr
													}
								);
		}
		Func<IOLAPMember, object> GetProp(string suffix) {
			if(suffix == null)
				return (member) => member.UniqueLevelValue;
			else
				if(suffix == "Self")
					return (member) => member;
				else
					if(suffix == "Value")
						return (member) => member.Value;
					else
						if(suffix == "Caption")
							return (member) => member.Caption;
						else
							return (member) => OLAPQQueryMemberEvaluatorBase.ErrorValue;
		}
		bool? ComparePropertyWithConstExpr<TType>(string propName, TType value) {
			IOLAPMember member;
			contextCache.TryGetValue(propName, out member);
			if(member == null)
				return null;
			return member.UniqueName == (value as string);
		}
		bool? ComparePropertyWithConstExprSelf<TType>(string propName, TType value) {
			IOLAPMember member;
			contextCache.TryGetValue(propName, out member);
			if(member == null)
				return null;
			return ReferenceEquals(member, value);
		}
		bool? ComparePropertyWithConstExprValue<TType>(string propName, TType value) {
			IOLAPMember member;
			contextCache.TryGetValue(propName, out member);
			if(member == null)
				return null;
			if(value == null)
				return member.Value == null;
			return ((IComparable)value).CompareTo(member.Value) == 0;
		}
		bool? ComparePropertyWithConstExprCaption<TType>(string propName, TType value) {
			IOLAPMember member;
			contextCache.TryGetValue(propName, out member);
			if(member == null)
				return null;
			return member.Caption == (value as string);
		}
		static Expression Convert(Expression val, Type type) {
			if(val.Type == type)
				return val;
			ConstantExpression constExpr = val as ConstantExpression;
			if(constExpr == null)
				return Expression.Convert(val, typeof(object));
			else {
				if(type == typeof(object))
					return Expression.Constant(constExpr.Value, type);
				else
					return Expression.Convert(val, typeof(object));
			}
		}
		Expression CompareByExpression(Expression a, Expression b, Func<Expression, Expression> action) {
			LabelTarget returnTarget = Expression.Label(typeof(object));
			ParameterExpression parA = Expression.Parameter(typeof(IComparable), "parA");
			ParameterExpression parB = Expression.Parameter(typeof(IComparable), "parB");
			ParameterExpression in1 = Expression.Variable(typeof(object), "in1");
			ParameterExpression in2 = Expression.Variable(typeof(object), "in2");
			return Expression.Block(typeof(object),
				new ParameterExpression[] { parA, parB, in1, in2 },
							Expression.Assign(in1, a),
							Expression.Assign(in2, b),
						 Expression.IfThen(Expression.ReferenceEqual(in1, Expression.Constant(QueryGroupFilterEvaluator.ErrorValue)),
													Expression.Return(returnTarget, Expression.Constant(null, typeof(object)))),
						 Expression.IfThen(Expression.ReferenceEqual(in2, Expression.Constant(QueryGroupFilterEvaluator.ErrorValue)),
													Expression.Return(returnTarget, Expression.Constant(null, typeof(object)))),
						Expression.Assign(parA, Expression.TypeAs(in1, typeof(IComparable))),
						Expression.IfThen(Expression.ReferenceEqual(parA, Expression.Constant(null, typeof(object))),
													Expression.Return(returnTarget, Expression.Constant(false, typeof(object)))),
					   Expression.Assign(parB, Expression.TypeAs(in2, typeof(IComparable))),
					   Expression.IfThen(Expression.ReferenceEqual(parB, Expression.Constant(null, typeof(object))),
													Expression.Return(returnTarget, Expression.Constant(false, typeof(object)))),
				Expression.TryCatch(Expression.Return(returnTarget, Expression.Convert(action(Expression.Call(parA, "CompareTo", null, new Expression[] { parB })), typeof(object))),
													Expression.Catch(
																 Expression.Parameter(typeof(Exception)), Expression.Return(returnTarget, Expression.Constant(false, typeof(object)))
																)),
				Expression.Label(returnTarget, Expression.Constant(null, typeof(object)))
				);
		}
		Expression Process(CriteriaOperator criteriaOperator) {
			if(ReferenceEquals(criteriaOperator, null))
				return null;
			return criteriaOperator.Accept(this);
		}
		Expression IClientCriteriaVisitor<Expression>.Visit(JoinOperand theOperand) {
			throw new NotImplementedException(); 
		}
		Expression IClientCriteriaVisitor<Expression>.Visit(AggregateOperand theOperand) {
			throw new NotImplementedException(); 
		}
		Expression ICriteriaVisitor<Expression>.Visit(BetweenOperator theOperator) {
			throw new NotImplementedException(); 
		}
		Expression ICriteriaVisitor<Expression>.Visit(FunctionOperator theOperator) {
			foreach(CriteriaOperator op in theOperator.Operands)
				if(object.Equals(null, (op as OperandValue)))
					throw new NotImplementedException(); 
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.IsNullOrEmpty: {
						foreach(OperandValue val in theOperator.Operands)
							if(val.Value != null) {
								string str = val.Value as string;
								if(str != string.Empty)
									return Expression.Constant(false);
							}
					}
					return Expression.Constant(true);
				default:
					throw new NotImplementedException(); 
			}
		}
	}
}
