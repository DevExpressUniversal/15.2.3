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
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using System.ComponentModel;
using DevExpress.Data.Filtering.Exceptions;
using System.Reflection;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using System.Reflection.Emit;
namespace DevExpress.Data.Filtering {
	public static class CriteriaCompiler {
		public static LambdaExpression ToLambda(CriteriaOperator expression, CriteriaCompilerDescriptor descriptor, CriteriaCompilerAuxSettings settings) {
			if(ReferenceEquals(expression, null))
				return null;
			return CriteriaCompilerCore.Compile(descriptor, settings, expression);
		}
		public static LambdaExpression ToLambda(CriteriaOperator expression, CriteriaCompilerDescriptor descriptor) {
			return ToLambda(expression, descriptor, null);
		}
		public static Func<object, bool> ToUntypedPredicate(CriteriaOperator expression, CriteriaCompilerDescriptor descriptor, CriteriaCompilerAuxSettings settings) {
			LambdaExpression lambda = ToLambda(expression, descriptor, settings);
			if(lambda == null)
				return null;
			if(lambda.Parameters[0].Type != typeof(object)) {
				ParameterExpression untyped = Expression.Parameter(typeof(object), "untyped");
				lambda = Expression.Lambda(Expression.Invoke(lambda, Expression.Convert(untyped, descriptor.ObjectType)), untyped);
			}
			return ToBoolLambda<object>(lambda);
		}
		public static Func<object, bool> ToUntypedPredicate(CriteriaOperator expression, CriteriaCompilerDescriptor descriptor) {
			return ToUntypedPredicate(expression, descriptor, null);
		}
		public static Func<T, bool> ToPredicate<T>(CriteriaOperator expression, CriteriaCompilerDescriptor descriptor, CriteriaCompilerAuxSettings settings) {
			LambdaExpression lambda = ToLambda(expression, descriptor, settings);
			if(lambda == null)
				return null;
			return ToBoolLambda<T>(lambda);
		}
		public static Func<T, bool> ToPredicate<T>(CriteriaOperator expression, CriteriaCompilerDescriptor descriptor) {
			return ToPredicate<T>(expression, descriptor, null);
		}
		static Func<T, bool> ToBoolLambda<T>(LambdaExpression lambda) {
			if(lambda.Body.Type == typeof(bool))
				return ((Expression<Func<T, bool>>)lambda).Compile();
			if(lambda.Body.Type == typeof(bool?)) {
				var compiled = ((Expression<Func<T, bool?>>)lambda).Compile();
				return x => compiled(x) ?? false;
			}
			if(lambda.Body.Type == typeof(object)) {
				var compiled = ((Expression<Func<T, object>>)lambda).Compile();
				return x => ((bool?)compiled(x)) ?? false;
			}
			throw new InvalidOperationException("Compiled criteria type is '" + lambda.Body.Type.FullName + "'; object, bool or bool? expected.");
		}
		public static Func<object, object> ToUntypedDelegate(CriteriaOperator expression, CriteriaCompilerDescriptor descriptor, CriteriaCompilerAuxSettings settings) {
			LambdaExpression lambda = ToLambda(expression, descriptor, settings);
			if(lambda == null)
				return null;
			Expression<Func<object, object>> typedLambda = lambda as Expression<Func<object, object>>;
			if(typedLambda == null) {
				var newParameter = Expression.Parameter(typeof(object), "objectParameter");
				Expression body = Expression.Invoke(lambda, lambda.Parameters[0].Type == typeof(object) ? (Expression)newParameter : Expression.Convert(newParameter, lambda.Parameters[0].Type));
				if(body.Type != typeof(object))
					body = Expression.Convert(body, typeof(object));
				typedLambda = Expression.Lambda<Func<object, object>>(body, newParameter);
			}
			return typedLambda.Compile();
		}
		public static Func<object, object> ToUntypedDelegate(CriteriaOperator expression, CriteriaCompilerDescriptor descriptor) {
			return ToUntypedDelegate(expression, descriptor, null);
		}
	}
	public abstract class CriteriaCompilerDescriptor {
		public static CriteriaCompilerDescriptor Get() {
			return CriteriaCompilerContextDescriptorReflective.Instance;
		}
		public static CriteriaCompilerDescriptor GetExpando() {
			return CriteriaCompilerContextDescriptorExpando.Instance;
		}
		public static CriteriaCompilerDescriptor Get(Type t) {
			if(typeof(System.Dynamic.ExpandoObject).IsAssignableFrom(t))
				return GetExpando();
			return new CriteriaCompiledContextDescriptorTyped(t);
		}
		public static CriteriaCompilerDescriptor Get<T>() {
			return Get(typeof(T));
		}
		public static CriteriaCompilerDescriptor Get(PropertyDescriptorCollection pds) {
			return new CriteriaCompiledContextDescriptorDescripted(pds);
		}
		public abstract Type ObjectType { get; }
		public abstract Expression MakePropertyAccess(Expression baseExpression, string propertyPath);
		public virtual CriteriaCompilerRefResult DiveIntoCollectionProperty(Expression baseExpression, string collectionPropertyPath) {
			throw new NotSupportedException("Collection-related operations not supported by " + this.GetType().FullName);
		}
		public virtual LambdaExpression MakeFreeJoinLambda(string joinTypeName, CriteriaOperator condition, OperandParameter[] conditionParameters, Aggregate aggregateType, CriteriaOperator aggregateExpression, OperandParameter[] aggregateExpresssionParameters, Type[] invokeTypes) {
			throw new NotSupportedException("Free join not supported by " + this.GetType().FullName);
		}
	}
	public class CriteriaCompilerLocalContext {
		public readonly Expression Expression;
		public readonly CriteriaCompilerDescriptor Descriptor;
		public CriteriaCompilerLocalContext(Expression expression, CriteriaCompilerDescriptor descriptor) {
			this.Expression = expression;
			this.Descriptor = descriptor;
		}
	}
	public class CriteriaCompilerRefResult {
		public readonly CriteriaCompilerLocalContext LocalContext;
		public readonly string SubProperty;
		public bool IsCollection { get { return SubProperty == null; } }
		public CriteriaCompilerRefResult(CriteriaCompilerLocalContext _LocalContext, string _SubProperty) {
			this.LocalContext = _LocalContext;
			this.SubProperty = _SubProperty;
		}
	}
	public class CriteriaCompilerAuxSettings {
		internal static readonly CriteriaCompilerAuxSettings DefaultInstance = new CriteriaCompilerAuxSettings();
		public bool CaseSensitive;
		public CustomFunctionCollection AdditionalFunctions;
		public CriteriaCompilerAuxSettings() { }
		public CriteriaCompilerAuxSettings(bool caseSensitive) {
			this.CaseSensitive = caseSensitive;
		}
		public CriteriaCompilerAuxSettings(bool caseSensitive, CustomFunctionCollection additionalFunctions)
			: this(caseSensitive) {
			this.AdditionalFunctions = additionalFunctions;
		}
	}
	public class CriteriaCompilerException: InvalidOperationException {
		public CriteriaCompilerException(string message, Exception innerException) : base(message, innerException) { }
		public CriteriaCompilerException(CriteriaOperator wholeCriteria, CriteriaOperator exceptionCauseCriteria, Exception innerException)
			: this(MakeExceptionText(wholeCriteria, exceptionCauseCriteria, innerException), innerException) {
		}
		private static string MakeExceptionText(CriteriaOperator wholeCriteria, CriteriaOperator exceptionCauseCriteria, Exception innerException) {
			string whole = CriteriaOperator.ToString(wholeCriteria);
			string cause;
			if(ReferenceEquals(exceptionCauseCriteria , null) || ReferenceEquals(wholeCriteria, exceptionCauseCriteria))
				cause = null;
			else
				cause = CriteriaOperator.ToString(exceptionCauseCriteria);
			string message = innerException == null ? string.Empty : innerException.Message;
			if(cause == null)
				return string.Format("Error '{0}' compiling criteria '{1}'", message, whole);
			else
				return string.Format("Error '{0}' at '{1}' compiling criteria '{2}'", message, cause, whole);
		}
	}
}
namespace DevExpress.Data.Filtering.Helpers {
	public abstract class CriteriaCompilerContext {
		public abstract CriteriaCompilerAuxSettings AuxSettings { get; }
		public abstract CriteriaCompilerLocalContext GetLocalContext(int upLevels);
		public Expression MakePropertyAccess(string propertyPath) {
			EvaluatorProperty ep = EvaluatorProperty.Create(new OperandProperty(propertyPath));
			CriteriaCompilerLocalContext localContext = GetLocalContext(ep.UpDepth);
			return localContext.Descriptor.MakePropertyAccess(localContext.Expression, ep.PropertyPath);
		}
		public CriteriaCompilerRefResult DiveIntoCollectionProperty(string collectionPath) {
			if(string.IsNullOrEmpty(collectionPath)) {
				CriteriaCompilerLocalContext localContext = GetLocalContext(0);
				return localContext.Descriptor.DiveIntoCollectionProperty(localContext.Expression, null);
			} else {
				EvaluatorProperty ep = EvaluatorProperty.Create(new OperandProperty(collectionPath));
				CriteriaCompilerLocalContext localContext = GetLocalContext(ep.UpDepth);
				var rv = localContext.Descriptor.DiveIntoCollectionProperty(localContext.Expression, ep.PropertyPath);
				return rv;
			}
		}
		public LambdaExpression MakeFreeJoinLambda(string joinTypeName, CriteriaOperator condition, OperandParameter[] conditionParameters, Aggregate aggregateType, CriteriaOperator aggregateExpression, OperandParameter[] aggregateExpresssionParameters, Type[] invokeTypes) {
			return GetLocalContext(0).Descriptor.MakeFreeJoinLambda(joinTypeName, condition, conditionParameters, aggregateType, aggregateExpression, aggregateExpresssionParameters, invokeTypes);
		}
	}
	public class CriteriaCompilerRootContext: CriteriaCompilerContext {
		readonly CriteriaCompilerAuxSettings _AuxSettings;
		public readonly CriteriaCompilerDescriptor Descriptor;
		public readonly ParameterExpression ThisExpression;
		public CriteriaCompilerRootContext(CriteriaCompilerDescriptor descriptor, CriteriaCompilerAuxSettings auxSettings) {
			this._AuxSettings = auxSettings ?? CriteriaCompilerAuxSettings.DefaultInstance;
			this.Descriptor = descriptor;
			this.ThisExpression = Expression.Parameter(Descriptor.ObjectType, "self");
		}
		public override CriteriaCompilerLocalContext GetLocalContext(int upLevels) {
			if(upLevels != 0)
				throw new InvalidOperationException("upLevels is non-zero for a root context");
			return new CriteriaCompilerLocalContext(ThisExpression, Descriptor);
		}
		public override CriteriaCompilerAuxSettings AuxSettings {
			get { return _AuxSettings; }
		}
	}
	public class CriteriaCompilerLocalException: Exception {
		public CriteriaOperator Cause;
		public CriteriaCompilerLocalException(Exception innerException, CriteriaOperator cause)
			: base(string.Empty, innerException) {
			this.Cause = cause;
		}
	}
	public class CriteriaCompilerCore: IClientCriteriaVisitor<Expression> {
		public readonly CriteriaCompilerContext Context;
		static void GuardNull(object arg, Func<string> getParameterName) {
			if(arg != null)
				return;
			throw new ArgumentNullException(getParameterName());
		}
		static void GuardNulls(IEnumerable arg, Func<int, string> getParameterName) {
			int i = 0;
			foreach(object o in arg) {
				if(o == null)
					throw new ArgumentNullException(getParameterName(i));
				++i;
			}
		}
		Expression IClientCriteriaVisitor<Expression>.Visit(AggregateOperand theOperand) {
			if(theOperand.IsTopLevel) {
				var topLevelCollectionAccess = this.Context.DiveIntoCollectionProperty(null);
				if(!topLevelCollectionAccess.IsCollection)
					throw new InvalidOperationException("!topLevelCollectionAccess.IsCollection");
				var cc = new CriteriaCompilerInsideAggregateClojureContext(this.Context, topLevelCollectionAccess.LocalContext);
				return DoFinalAggregate(cc, theOperand.Condition, theOperand.AggregateType, theOperand.AggregatedExpression);
			} else {
				EvaluatorProperty ep = EvaluatorProperty.Create(theOperand.CollectionProperty);
				if(ep.UpDepth > 0) {
					var upLevelsContext = new CriteriaCompilerUpLevelsContext(this.Context, ep.UpDepth);
					Expression body = DoSubAggragate(upLevelsContext, ep.PropertyPath, theOperand.Condition, theOperand.AggregateType, theOperand.AggregatedExpression);
					return new NestedLambdaCompiler(upLevelsContext).Make(body);
				} else {
					return DoSubAggragate(this.Context, ep.PropertyPath, theOperand.Condition, theOperand.AggregateType, theOperand.AggregatedExpression);
				}
			}
		}
		static Expression DoSubAggragate(CriteriaCompilerContext outerContext, string propertyPath, CriteriaOperator filter, Aggregate aggregateType, CriteriaOperator aggregateExpression) {
			CriteriaCompilerRefResult dive = outerContext.DiveIntoCollectionProperty(propertyPath);
			CriteriaCompilerDownLevelContext refContext = new CriteriaCompilerDownLevelContext(outerContext, dive.LocalContext);
			CriteriaCompilerLambdaContext nullProtectedRefContext = new CriteriaCompilerLambdaContext(refContext);
			Expression aggregate;
			if(dive.IsCollection){
				aggregate = DoFinalAggregate(nullProtectedRefContext, filter, aggregateType, aggregateExpression);
			} else {
				aggregate = DoSubAggragate(nullProtectedRefContext, dive.SubProperty, filter, aggregateType, aggregateExpression);
			}
			Expression aggregateInRefContext = new NestedLambdaCompiler(nullProtectedRefContext).Make(aggregate);
			ConstantExpression failedRefValue = aggregateType == Aggregate.Exists ? Expression.Constant(false) : Expression.Constant(null, aggregateInRefContext.Type);
			ParameterExpression refInRefContext = (ParameterExpression)refContext.GetLocalContext(0).Expression;
			Expression body = Expression.Condition(Expression.Call(typeof(object), "ReferenceEquals", null, refInRefContext, Expression.Constant(null)), failedRefValue, aggregateInRefContext);
			return new NestedLambdaCompiler(refContext).Make(body);
		}
		static Expression DoFinalAggregate(CriteriaCompilerContext collectionContext, CriteriaOperator filter, Aggregate aggregateType, CriteriaOperator aggregateExpression) {
			Expression aggregate;
			var nullProtectedLocal = collectionContext.GetLocalContext(0);
			CriteriaCompilerDescriptor rowDescriptor = nullProtectedLocal.Descriptor;
			Expression collectionExpression = nullProtectedLocal.Expression;
			Type targetCollectionType = typeof(IEnumerable<>).MakeGenericType(rowDescriptor.ObjectType);
			if(!targetCollectionType.IsAssignableFrom(collectionExpression.Type)) {
				if(!typeof(IEnumerable).IsAssignableFrom(collectionExpression.Type))
					collectionExpression = Expression.Convert(collectionExpression, typeof(IEnumerable));
				collectionExpression = Expression.Call(typeof(Enumerable), "Cast", new Type[] { rowDescriptor.ObjectType }, collectionExpression);
			}
			if(!ReferenceEquals(filter, null)) {
				ParameterExpression toFilterParameter = Expression.Parameter(targetCollectionType, "toFilter");
				CriteriaCompilerLambdaContext lc = new CriteriaCompilerLambdaContext(collectionContext, new KeyValuePair<ParameterExpression, Expression>(toFilterParameter, collectionExpression));
				LambdaExpression whereLambda = MakeInsideAggregateLambda(rowDescriptor, lc, filter, true);
				Expression filteredCollectionBody = Expression.Call(typeof(Enumerable), "Where", new Type[] { rowDescriptor.ObjectType }, toFilterParameter, whereLambda);
				collectionExpression = new NestedLambdaCompiler(lc).MakeCompiled(filteredCollectionBody);
			}
			ParameterExpression toAggParameter = Expression.Parameter(targetCollectionType, "toAgg");
			CriteriaCompilerLambdaContext aggContext = new CriteriaCompilerLambdaContext(collectionContext, new KeyValuePair<ParameterExpression, Expression>(toAggParameter, collectionExpression));
			LambdaExpression argLambda;
			if(aggregateType == Aggregate.Exists || aggregateType == Aggregate.Count) {
				argLambda = null;
			} else {
				GuardNull(aggregateExpression, () => "theOperand.AggregateExpression");
				argLambda = MakeInsideAggregateLambda(rowDescriptor, aggContext, aggregateExpression, false);
			}
			aggregate = MakeAggregate(aggregateType, toAggParameter, argLambda, rowDescriptor.ObjectType, collectionContext.AuxSettings);
			return new NestedLambdaCompiler(aggContext).MakeCompiled(aggregate);
		}
		static class AggregatesHelpers {
			public static IEnumerable<T> Cast<T>(IEnumerable seq) {
				if(seq == null)
					return null;
				else
					return seq.Cast<T>();
			}
			public static IEnumerable<T> Where<T>(IEnumerable<T> seq, Func<T, bool> predicate) {
				if(seq == null)
					return null;
				else
					return seq.Where(predicate);
			}
			public static int? Count<T>(IEnumerable<T> seq) {
				if(seq == null)
					return null;
				else
					return seq.Count();
			}
			public static bool Exists<T>(IEnumerable<T> seq) {
				if(seq == null)
					return false;
				else
					return seq.Any();
			}
			public static IEnumerable<E> Select<T, E>(IEnumerable<T> seq, Func<T, E> selector) {
				if(seq == null) {
					return null;
				} else {
					return seq.Select(row => selector(row));
				}
			}
			public static T Single<T>(IEnumerable<T> seq) {
				if(seq == null)
					return default(T);
				ICollection icoll = seq as ICollection;
				if(icoll != null) {
					switch(icoll.Count){
						case 0:
							return default(T);
						case 1: {
								IList<T> ilist = seq as IList<T>;
								if(ilist != null)
									return ilist[0];
								else
									foreach(T tt in seq)
										return tt;
								return default(T);
						}
						default:
							throw new InvalidOperationException("The collection to which the Single aggregate is applied must be empty or contain exactly one item");
					}
				} else {
					using(IEnumerator<T> e = seq.GetEnumerator()) {
						if(!e.MoveNext())
							return default(T);
						T rv = e.Current;
						if(e.MoveNext())
							throw new InvalidOperationException("The collection to which the Single aggregate is applied must be empty or contain exactly one item");
						return rv;
					}
				}
			}
			public static Int32? SumInt32(IEnumerable<Int32?> seq) {
				if(seq == null)
					return null;
				int? rv = null;
				foreach(var i in seq) {
					if(!i.HasValue)
						continue;
					else if(rv.HasValue)
						rv = rv.Value + i.Value;
					else
						rv = i;
				}
				return rv;
			}
			public static Int64? SumInt64(IEnumerable<Int64?> seq) {
				if(seq == null)
					return null;
				else
					return seq.Sum();
			}
			public static UInt32? SumUInt32(IEnumerable<UInt32?> seq) {
				if(seq == null)
					return null;
				else
					return (UInt32?)SumDecimal(seq.Cast<Decimal?>());
			}
			public static UInt64? SumUInt64(IEnumerable<UInt64?> seq) {
				if(seq == null)
					return null;
				else
					return (UInt64?)SumDecimal(seq.Cast<Decimal?>());
			}
			public static Single? SumSingle(IEnumerable<Single?> seq) {
				if(seq == null)
					return null;
				else
					return seq.Sum();
			}
			public static Double? SumDouble(IEnumerable<Double?> seq) {
				if(seq == null)
					return null;
				else
					return seq.Sum();
			}
			public static Decimal? SumDecimal(IEnumerable<Decimal?> seq) {
				if(seq == null)
					return null;
				else
					return seq.Sum();
			}
			public static object SumObject(IEnumerable<object> seq) {
				throw new NotImplementedException();
			}
			public static double? AvgInt32(IEnumerable<Int32?> seq) {
				if(seq == null)
					return null;
				else
					return seq.Average();
			}
			public static double? AvgInt64(IEnumerable<Int64?> seq) {
				if(seq == null)
					return null;
				else
					return seq.Average();
			}
			public static double? AvgUInt32(IEnumerable<UInt32?> seq) {
				if(seq == null)
					return null;
				else
					return seq.Cast<double?>().Average();
			}
			public static double? AvgUInt64(IEnumerable<UInt64?> seq) {
				if(seq == null)
					return null;
				else
					return seq.Cast<double?>().Average();
			}
			public static Single? AvgSingle(IEnumerable<Single?> seq) {
				if(seq == null)
					return null;
				else
					return seq.Average();
			}
			public static Double? AvgDouble(IEnumerable<Double?> seq) {
				if(seq == null)
					return null;
				else
					return seq.Average();
			}
			public static Decimal? AvgDecimal(IEnumerable<Decimal?> seq) {
				if(seq == null)
					return null;
				else
					return seq.Average();
			}
			public static object AvgObject(IEnumerable<object> seq) {
				throw new NotImplementedException();
			}
			public static Nullable<T> MinMaxStruct<T>(IEnumerable<T> seq, Func<T, T, bool> isBetter) where T: struct {
				if(seq == null)
					return null;
				Nullable<T> rv = null;
				foreach(T t in seq) {
					if(rv.HasValue) {
						if(isBetter(t, rv.Value))
							rv = t;
					} else
						rv = t;
				}
				return rv;
			}
			public static T MinMaxClass<T>(IEnumerable<T> seq, Func<T, T, bool> isBetter) where T: class {
				if(seq == null)
					return null;
				T rv = null;
				foreach(T t in seq) {
					if(!ReferenceEquals(rv, null)) {
						if(isBetter(t, rv))
							rv = t;
					} else
						rv = t;
				}
				return rv;
			}
		}
		static Expression MakeAggregate(Aggregate aggregate, Expression collectionArg, LambdaExpression argLambda, Type rowType, CriteriaCompilerAuxSettings settings) {
			switch(aggregate) {
				case Aggregate.Count:
					return Expression.Call(typeof(AggregatesHelpers), "Count", new Type[] { rowType }, collectionArg);
				case Aggregate.Exists:
					return Expression.Call(typeof(AggregatesHelpers), "Exists", new Type[] { rowType }, collectionArg);
			}
			if(!NullableHelpers.CanAcceptNull(argLambda.Body.Type)) {
				ParameterExpression pe = Expression.Parameter(rowType, "vt");
				argLambda = Expression.Lambda(Expression.Convert(Expression.Invoke(argLambda, pe), NullableHelpers.GetUnBoxedType(argLambda.Body.Type)), pe);
			}
			switch(aggregate) {
				case Aggregate.Single:
					return Expression.Call(typeof(AggregatesHelpers), "Single", new Type[] { argLambda.Body.Type }, Expression.Call(typeof(AggregatesHelpers), "Select", new Type[] { rowType, argLambda.Body.Type }, collectionArg, argLambda));
				case Aggregate.Sum:
				case Aggregate.Avg:
					if(argLambda.Body.Type == typeof(Object)) {
						string fnName = aggregate.ToString() + "Object";
						return Expression.Call(typeof(AggregatesHelpers), fnName, null, Expression.Call(typeof(AggregatesHelpers), "Select", new Type[] { rowType, argLambda.Body.Type }, collectionArg, argLambda));
					} else {
						Type unnulledType = Nullable.GetUnderlyingType(argLambda.Body.Type);
						if(unnulledType == null)
							throw new InvalidOperationException("can't sum or average " + argLambda.Body.Type.FullName);
						Type mathType = EvalHelpers.GetBinaryNumericPromotionType(unnulledType, unnulledType);
						if(!mathType.IsValueType())
							throw new InvalidOperationException("can't sum or average " + argLambda.Body.Type.FullName);
						Type nullableMathType = typeof(Nullable<>).MakeGenericType(mathType);
						if(argLambda.Body.Type != nullableMathType) {
							ParameterExpression r = Expression.Parameter(rowType, "r");
							argLambda = Expression.Lambda(Expression.Convert(Expression.Invoke(argLambda, r), nullableMathType), r);
						}
						Expression select = Expression.Call(typeof(AggregatesHelpers), "Select", new Type[] { rowType, argLambda.Body.Type }, collectionArg, argLambda);
						string mathName = aggregate.ToString() + mathType.Name;
						return Expression.Call(typeof(AggregatesHelpers), mathName, null, select);
					}
				case Aggregate.Max:
				case Aggregate.Min: {
						Expression withoutNulls;
						Type minmaxType = argLambda.Body.Type;
						Type cmpType = minmaxType;
						Expression select = Expression.Call(typeof(AggregatesHelpers), "Select", new Type[] { rowType, minmaxType }, collectionArg, argLambda);
						if(minmaxType.IsValueType()) {
							Type underlying = Nullable.GetUnderlyingType(minmaxType);
							if(underlying == null) {
								withoutNulls = select;
							} else {
								ParameterExpression x = Expression.Parameter(minmaxType, "x");
								ParameterExpression y = Expression.Parameter(minmaxType, "y");
								Expression filtered = Expression.Call(typeof(AggregatesHelpers), "Where", new Type[] { minmaxType }, select, Expression.Lambda(Expression.PropertyOrField(x, "HasValue"), x));
								withoutNulls = Expression.Call(typeof(AggregatesHelpers), "Select", new Type[] { minmaxType, underlying }, filtered, Expression.Lambda(Expression.PropertyOrField(y, "Value"), y));
								cmpType = underlying;
							}
						} else {
							ParameterExpression x = Expression.Parameter(minmaxType, "x");
							withoutNulls = Expression.Call(typeof(AggregatesHelpers), "Where", new Type[] { minmaxType }, select, Expression.Lambda(Expression.Not(Expression.Call(typeof(object), "ReferenceEquals", null, x, Expression.Constant(null))), x));
						}
						ParameterExpression a = Expression.Parameter(cmpType, "a");
						ParameterExpression b = Expression.Parameter(cmpType, "b");
						Expression comparer;
						if(aggregate == Aggregate.Min) {
							comparer = MakeCompare(a, b, false, cmp => cmp < 0, (x, y) => Expression.LessThan(x, y), settings);
						} else {
							comparer = MakeCompare(a, b, false, cmp => cmp > 0, (x, y) => Expression.GreaterThan(x, y), settings);
						}
						LambdaExpression comparerLambda = Expression.Lambda(comparer, a, b);
						string fn = cmpType.IsValueType() ? "MinMaxStruct" : "MinMaxClass";
						return Expression.Call(typeof(AggregatesHelpers), fn, new Type[]{ cmpType}, withoutNulls, comparerLambda);
					}
			}
			throw new NotSupportedException(aggregate.ToString());
		}
		static LambdaExpression MakeInsideAggregateLambda(CriteriaCompilerDescriptor collectionAccessDescriptor, CriteriaCompilerContext donorContext, CriteriaOperator toCompile, bool needBoolRetType) {
			ParameterExpression rowParameter = Expression.Parameter(collectionAccessDescriptor.ObjectType, "row");
			CriteriaCompilerInsideAggregateClojureContext whereContext = new CriteriaCompilerInsideAggregateClojureContext(donorContext, new CriteriaCompilerLocalContext(rowParameter, collectionAccessDescriptor));
			CriteriaCompilerCore whereCore = new CriteriaCompilerCore(whereContext);
			Expression whereBody = whereCore.Process(toCompile);
			if(needBoolRetType) {
				if(whereBody.Type == typeof(object))
					whereBody = Expression.Convert(whereBody, typeof(bool?));
				if(whereBody.Type == typeof(bool?))
					whereBody = Expression.Coalesce(whereBody, Expression.Constant(false));
				if(whereBody.Type != typeof(bool))
					throw new InvalidOperationException(string.Format("Compiled criteria '{0}' type is '{1}'; object, bool or bool? expected.", toCompile, whereBody.Type.FullName));
			}
			LambdaExpression whereLambda = Expression.Lambda(whereBody, rowParameter);
			return whereLambda;
		}
		Expression IClientCriteriaVisitor<Expression>.Visit(OperandProperty theOperand) {
			return Context.MakePropertyAccess(theOperand.PropertyName);
		}
		Expression IClientCriteriaVisitor<Expression>.Visit(JoinOperand theOperand) {
			OperandParameter[] conditionParameters;
			var processedCondition = CriteriaCompilerFreeJoinCriteriaReprocessor.Process(theOperand.Condition, out conditionParameters);
			OperandParameter[] aeParameters;
			var processedAggregateExpression = CriteriaCompilerFreeJoinCriteriaReprocessor.Process(theOperand.AggregatedExpression, out aeParameters);
			Expression[] invokeParams = conditionParameters.Concat(aeParameters).Select(op => Process(new OperandProperty(op.ParameterName))).ToArray();
			LambdaExpression coreLambda = this.Context.MakeFreeJoinLambda(theOperand.JoinTypeName, processedCondition, conditionParameters, theOperand.AggregateType, processedAggregateExpression, aeParameters, invokeParams.Select(p => p.Type).ToArray());
			return Expression.Invoke(coreLambda, invokeParams);
		}
		Expression ICriteriaVisitor<Expression>.Visit(BetweenOperator theOperator) {
			GuardNull(theOperator.TestExpression, () => "BetweenOperator.TestExpression");
			GuardNull(theOperator.BeginExpression, () => "BetweenOperator.BeginExpression");
			GuardNull(theOperator.EndExpression, () => "BetweenOperator.EndExpression");
			Expression testExpression = Process(theOperator.TestExpression);
			ParameterExpression nestedParameter = Expression.Parameter(testExpression.Type, "testExpression");
			NestedLambdaCompiler nlc = new NestedLambdaCompiler(new CriteriaCompilerLambdaContext(this.Context, new KeyValuePair<ParameterExpression, Expression>(nestedParameter, testExpression)));
			Expression begin = nlc.Process(theOperator.BeginExpression);
			Expression end = nlc.Process(theOperator.EndExpression);
			Expression testBegin = MakeCompare(begin, nestedParameter, false, cmp => cmp <= 0, (x, y) => Expression.LessThanOrEqual(x, y), this.Context.AuxSettings);
			Expression testEnd = MakeCompare(nestedParameter, end, false, cmp => cmp <= 0, (x, y) => Expression.LessThanOrEqual(x, y), this.Context.AuxSettings);
			Expression body = Expression.AndAlso(testBegin, testEnd);
			return nlc.Make(body);
		}
		Expression ICriteriaVisitor<Expression>.Visit(BinaryOperator theOperator) {
#pragma warning disable 618
			if(theOperator.OperatorType == BinaryOperatorType.Like)
				return Process(LikeCustomFunction.Convert(theOperator));
#pragma warning restore 618
			GuardNull(theOperator.LeftOperand, () => "BinaryOperator.LeftOperand");
			GuardNull(theOperator.RightOperand, () => "BinaryOperator.RightOperand");
			Expression left = Process(theOperator.LeftOperand);
			Expression right = Process(theOperator.RightOperand);
			switch(theOperator.OperatorType) {
				case BinaryOperatorType.Equal:
					return MakeCompare(left, right, true, cmp => cmp == 0, (x, y) => Expression.Equal(x, y), this.Context.AuxSettings);
				case BinaryOperatorType.NotEqual:
					return MakeCompare(left, right, true, cmp => cmp != 0, (x, y) => Expression.NotEqual(x, y), this.Context.AuxSettings);
				case BinaryOperatorType.Less:
					return MakeCompare(left, right, false, cmp => cmp < 0, (x, y) => Expression.LessThan(x, y), this.Context.AuxSettings);
				case BinaryOperatorType.Greater:
					return MakeCompare(left, right, false, cmp => cmp > 0, (x, y) => Expression.GreaterThan(x, y), this.Context.AuxSettings);
				case BinaryOperatorType.LessOrEqual:
					return MakeCompare(left, right, false, cmp => cmp <= 0, (x, y) => Expression.LessThanOrEqual(x, y), this.Context.AuxSettings);
				case BinaryOperatorType.GreaterOrEqual:
					return MakeCompare(left, right, false, cmp => cmp >= 0, (x, y) => Expression.GreaterThanOrEqual(x, y), this.Context.AuxSettings);
				case BinaryOperatorType.Plus:
					if(left.Type == typeof(string) || right.Type == typeof(string)) {
						Func<string, string, string> combinator =
							(l, r) => l + r;
						return Expression.Invoke(Expression.Constant(combinator), EvalHelpers.SafeToString(left), EvalHelpers.SafeToString(right));
					}
					return MakeBinaryMath(left, right
						, (l, r) => EvalHelpers.DoObjectsPlus(l, r)
						, (l, r) => Expression.Add(l, r)
						);
				case BinaryOperatorType.Minus:
					return MakeBinaryMath(left, right
						, (l, r) => EvalHelpers.DoObjectsMinus(l, r)
						, (l, r) => Expression.Subtract(l, r)
						);
				case BinaryOperatorType.Multiply:
					return MakeBinaryMath(left, right
						, (l, r) => EvalHelpers.DoObjectsMultiply(l, r)
						, (l, r) => Expression.Multiply(l, r)
						);
				case BinaryOperatorType.Divide:
					return MakeBinaryMath(left, right
						, (l, r) => EvalHelpers.DoObjectsDivide(l, r)
						, (l, r) => Expression.Divide(l, r)
						);
				case BinaryOperatorType.Modulo:
					return MakeBinaryMath(left, right
						, (l, r) => EvalHelpers.DoObjectsModulo(l, r)
						, (l, r) => Expression.Modulo(l, r)
						);
				case BinaryOperatorType.BitwiseAnd:
					return MakeBinaryMath(left, right
						, (l, r) => EvalHelpers.DoObjectsBitwiseAnd(l, r)
						, (l, r) => Expression.And(l, r)
						);
				case BinaryOperatorType.BitwiseOr:
					return MakeBinaryMath(left, right
						, (l, r) => EvalHelpers.DoObjectsBitwiseOr(l, r)
						, (l, r) => Expression.Or(l, r)
						);
				case BinaryOperatorType.BitwiseXor:
					return MakeBinaryMath(left, right
						, (l, r) => EvalHelpers.DoObjectsBitwiseXor(l, r)
						, (l, r) => Expression.ExclusiveOr(l, r)
						);
				default:
					throw new NotImplementedException(theOperator.OperatorType.ToString());
			}
		}
		static Expression MakeCompare(Expression left, Expression right, bool isEqualsCompare, Func<int, bool> postCmpLambda, Func<Expression, Expression, Expression> directCmp, CriteriaCompilerAuxSettings auxSettings) {
			if(left.Type == typeof(string) || right.Type == typeof(string)) {
				StringComparer cmp = auxSettings.CaseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase;
				if(isEqualsCompare) {
					Expression<Func<string, string, bool>> eq;
					if(postCmpLambda(0))
						eq = (x, y) => cmp.Equals(x, y);
					else
						eq = (x, y) => !cmp.Equals(x, y);
					return Expression.Invoke(eq, EvalHelpers.SafeToString(left), EvalHelpers.SafeToString(right));
				} else {
					Expression<Func<string, string, int>> cmpInt = (x, y) => cmp.Compare(x, y);
					return Expression.Invoke(Expression.Constant(postCmpLambda), Expression.Invoke(cmpInt, EvalHelpers.SafeToString(left), EvalHelpers.SafeToString(right)));
				}
			}
			if(left.Type == typeof(object) || right.Type == typeof(object)) {
				Expression leftToObject = left.Type == typeof(object) ? left : Expression.Convert(left, typeof(object));
				Expression rightToObject = right.Type == typeof(object) ? right : Expression.Convert(right, typeof(object));
				Expression<Func<object, object, int>> cmpIntLambda = (x, y) => EvalHelpers.CompareObjects(x, y, isEqualsCompare, false, null);
				Expression cmpInt = Expression.Invoke(cmpIntLambda, leftToObject, rightToObject);
				return Expression.Invoke(Expression.Constant(postCmpLambda), cmpInt);
			}
			Type lType = Nullable.GetUnderlyingType(left.Type);
			Type rType = Nullable.GetUnderlyingType(right.Type);
			bool lNullable = lType != null;
			bool rNullable = rType != null;
			if(lType == null)
				lType = left.Type;
			if(rType == null)
				rType = right.Type;
			Type elevatedType = lType == rType ? lType : EvalHelpers.GetBinaryNumericPromotionType(rType, lType);
			if(elevatedType == typeof(object)) {
				if(isEqualsCompare) {
					Func<object, object, bool> eqFunc = (x, y) =>
						EqualityComparer<object>.Default.Equals(x, y);
					Expression ex = Expression.Invoke(Expression.Constant(eqFunc), Expression.Convert(left, typeof(object)), Expression.Convert(right, typeof(object)));
					return postCmpLambda(0) ? ex : Expression.Not(ex);
				}
				if(typeof(IComparable).IsAssignableFrom(lType) || typeof(IComparable).IsAssignableFrom(rType)) {
					Func<object, object, int> cmpInt = (x, y) => {
						return Comparer<object>.Default.Compare(x, y);
					};
					return Expression.Invoke(Expression.Constant(postCmpLambda), Expression.Invoke(Expression.Constant(cmpInt), Expression.Convert(left, typeof(object)), Expression.Convert(right, typeof(object))));
				}
			}
			ParameterExpression lP = Expression.Parameter(left.Type, "lp");
			ParameterExpression rP = Expression.Parameter(right.Type, "rp");
			Expression lValue = lNullable ? (Expression)Expression.PropertyOrField(lP, "Value") : lP;
			Expression rValue = rNullable ? (Expression)Expression.PropertyOrField(rP, "Value") : rP;
			Expression lComparable = elevatedType == typeof(object) || elevatedType == lType ? lValue : Expression.Convert(lValue, elevatedType);
			Expression rComparable = elevatedType == typeof(object) || elevatedType == rType ? rValue : Expression.Convert(rValue, elevatedType);
			Expression comparison;
			if(isEqualsCompare) {
				Expression plainCompare = Expression.Call(typeof(CompareHelper), "DoEquals", new Type[] { elevatedType }, lComparable, rComparable);
				comparison = postCmpLambda(0) ? plainCompare : Expression.Not(plainCompare);
			} else if(typeof(IComparable).IsAssignableFrom(elevatedType) || typeof(IComparable<>).MakeGenericType(elevatedType).IsAssignableFrom(elevatedType)) {
				Expression cmpInt = Expression.Call(typeof(CompareHelper), "DoCompare", new Type[] { elevatedType }, lComparable, rComparable);
				comparison = Expression.Invoke(Expression.Constant(postCmpLambda), cmpInt);
			} else {
				comparison = directCmp(lComparable, rComparable);
			}
			Expression body;
			if(lNullable) {
				if(rNullable) {
					body = Expression.Condition(Expression.PropertyOrField(lP, "HasValue")
						, Expression.Condition(Expression.PropertyOrField(rP, "HasValue"), comparison, Expression.Constant(postCmpLambda(1)))
						, Expression.Condition(Expression.PropertyOrField(rP, "HasValue"), Expression.Constant(postCmpLambda(-1)), Expression.Constant(postCmpLambda(0)))
						);
				} else {
					body = Expression.Condition(Expression.PropertyOrField(lP, "HasValue"), comparison, Expression.Constant(postCmpLambda(-1)));
				}
			} else {
				if(rNullable) {
					body = Expression.Condition(Expression.PropertyOrField(rP, "HasValue"), comparison, Expression.Constant(postCmpLambda(1)));
				} else {
					body = comparison;
				}
			}
			return Expression.Invoke(Expression.Lambda(body, lP, rP), left, right);
		}
		public static class CompareHelper {
			public static int DoCompare<T>(T x, T y) {
				return Comparer<T>.Default.Compare(x, y);
			}
			public static bool DoEquals<T>(T x, T y) {
				return EqualityComparer<T>.Default.Equals(x, y);
			}
		}
		static Expression MakeBinaryMath(Expression left, Expression right, Expression<Func<object, object, object>> objectsCombinator, Func<Expression, Expression, Expression> typedCombinator) {
			if(IsConstantUntypedNull(left) || IsConstantUntypedNull(right))
				return Expression.Constant(null);
			if(left.Type == typeof(object) || right.Type == typeof(object)) {
				return Expression.Invoke(objectsCombinator, Expression.Convert(left, typeof(object)), Expression.Convert(right, typeof(object)));
			}
			bool leftNullable = Nullable.GetUnderlyingType(left.Type) != null;
			bool rightNullable = Nullable.GetUnderlyingType(right.Type) != null;
			Type leftType = leftNullable ? Nullable.GetUnderlyingType(left.Type) : left.Type;
			Type rightType = rightNullable ? Nullable.GetUnderlyingType(right.Type) : right.Type;
			Type resultType = EvalHelpers.GetBinaryNumericPromotionType(leftType, rightType);
			Type finalResultType = leftNullable || rightNullable ? typeof(Nullable<>).MakeGenericType(resultType) : resultType;
			ParameterExpression lambdaLeftParam = Expression.Parameter(left.Type, "lambdaLeftParam");
			ParameterExpression lambdaRightParam = Expression.Parameter(right.Type, "lambdaRightParam");
			Expression lValue = leftNullable ? (Expression)Expression.PropertyOrField(lambdaLeftParam, "Value") : lambdaLeftParam;
			Expression lTyped = resultType == leftType || resultType == typeof(object) ? lValue : Expression.Convert(lValue, resultType);
			Expression rValue = rightNullable ? (Expression)Expression.PropertyOrField(lambdaRightParam, "Value") : lambdaRightParam;
			Expression rTyped = resultType == rightType || resultType == typeof(object) ? rValue : Expression.Convert(rValue, resultType);
			Expression realAdd = typedCombinator(lTyped, rTyped);
			Expression valueCondition;
			if(leftNullable) {
				if(rightNullable) {
					valueCondition = Expression.AndAlso(Expression.PropertyOrField(lambdaLeftParam, "HasValue"), Expression.PropertyOrField(lambdaRightParam, "HasValue"));
				} else {
					valueCondition = Expression.PropertyOrField(lambdaLeftParam, "HasValue");
				}
			} else {
				if(rightNullable) {
					valueCondition = Expression.PropertyOrField(lambdaRightParam, "HasValue");
				} else {
					valueCondition = null;
				}
			}
			Expression res;
			if(valueCondition == null) {
				res = realAdd;
			} else {
				res = Expression.Condition(valueCondition, Expression.Convert(realAdd, finalResultType), Expression.Constant(null, finalResultType));
			}
			return Expression.Invoke(Expression.Lambda(res, lambdaLeftParam, lambdaRightParam), left, right);
		}
		static bool IsConstantUntypedNull(Expression e) {
			if(e.Type != typeof(object))
				return false;
			ConstantExpression ce = e as ConstantExpression;
			if(ce == null)
				return false;
			return ce.Value == null;
		}
		Expression ICriteriaVisitor<Expression>.Visit(UnaryOperator theOperator) {
			GuardNull(theOperator.Operand, () => "UnaryOperator.Operand");
			Expression processed = Process(theOperator.Operand);
			switch(theOperator.OperatorType){
				case UnaryOperatorType.BitwiseNot:
				case UnaryOperatorType.Not:
					return Expression.Not(processed);
				case UnaryOperatorType.IsNull:
					if(processed.Type.IsValueType()) {
						if(Nullable.GetUnderlyingType(processed.Type) != null)
							return Expression.Not(Expression.PropertyOrField(processed, "HasValue"));
						else
							return Expression.Constant(false);
					} else {
						return Expression.Equal(processed, Expression.Constant(null));
					}
				case UnaryOperatorType.Minus:
					return Expression.Negate(processed);
				case UnaryOperatorType.Plus:
					return Expression.UnaryPlus(processed);
				default:
					throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, DevExpress.Data.Filtering.Exceptions.FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotImplemented, typeof(UnaryOperator).Name, theOperator.OperatorType.ToString()));
			}
		}
		Expression ICriteriaVisitor<Expression>.Visit(InOperator theOperator) {
			if(theOperator.Operands.Count == 0)
				return Expression.Constant(false);
			GuardNull(theOperator.LeftOperand, () => "InOperator.LeftOperand");
			GuardNulls(theOperator.Operands, i => "InOperator.Operands[" + i.ToString() + "]");
			Expression left = Process(theOperator.LeftOperand);
			ParameterExpression leftParam = Expression.Parameter(left.Type, "left");
			NestedLambdaCompiler n = new NestedLambdaCompiler(new CriteriaCompilerLambdaContext(Context, new KeyValuePair<ParameterExpression, Expression>(leftParam, left)));
			Expression resultCore = MakeInSmart(leftParam, theOperator.Operands, n);
			return n.Make(resultCore);
		}
		public static class InSmartHelper {
			public abstract class AnyBase {
				public abstract Array ToTypedArray(IEnumerable boxed);
				public class Impl<T>: AnyBase {
					public override Array ToTypedArray(IEnumerable boxed) {
						return boxed.Cast<T>().ToArray();
					}
				}
				public static AnyBase GetImpl(Type type) {
					return (AnyBase)Activator.CreateInstance(typeof(Impl<>).MakeGenericType(type));
				}
			}
			public static Array ToTypedArray(Type type, IEnumerable boxed) {
				return AnyBase.GetImpl(type).ToTypedArray(boxed);
			}
			public abstract class StructBase {
				public abstract Delegate MakePrimitiveIn(IEnumerable boxedValues);
				public abstract Delegate MakeNullablePrimitiveIn(IEnumerable boxedValues);
				public class Impl<T>: StructBase where T: struct {
					public override Delegate MakePrimitiveIn(IEnumerable boxedValues) {
						return MakePrimitiveInCore(boxedValues);
					}
					static Func<T, bool> MakePrimitiveInCore(IEnumerable boxedValues) {
						Dictionary<T, byte> primitives = new Dictionary<T, byte>();
						foreach(T p in boxedValues)
							primitives[p] = 0;
						return (T p) => primitives.ContainsKey(p);
					}
					public override Delegate MakeNullablePrimitiveIn(IEnumerable boxedValues) {
						var notNullable = MakePrimitiveInCore(boxedValues);
						Func<T?, bool> rv = (T? p) => p.HasValue ? notNullable(p.Value) : false;
						return rv;
					}
				}
				public static StructBase GetImpl(Type type) {
					return (StructBase)Activator.CreateInstance(typeof(Impl<>).MakeGenericType(type));
				}
			}
			public static Delegate MakePrimitiveIn(Type type, IEnumerable boxedValues) {
				return StructBase.GetImpl(type).MakePrimitiveIn(boxedValues);
			}
			public static Delegate MakeNullablePrimitiveIn(Type type, IEnumerable boxedValues) {
				return StructBase.GetImpl(type).MakeNullablePrimitiveIn(boxedValues);
			}
			static Func<string, bool> MakeStringIn(IEnumerable boxedValues, CriteriaCompilerAuxSettings auxSettings) {
				Dictionary<string, byte> stringDictionary = new Dictionary<string, byte>(auxSettings.CaseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase);
				foreach(string s in boxedValues)
					stringDictionary[s] = 0;
				return (string s) => s == null ? false : stringDictionary.ContainsKey(s);
			}
			public static Expression MakeInAgainstConstants(Expression leftParam, IEnumerable boxedValues, Type t, CriteriaCompilerAuxSettings auxSettings) {
				if(t == typeof(string) && leftParam.Type == typeof(string)) {
					var stringFunc = MakeStringIn(boxedValues, auxSettings);
					return Expression.Invoke(Expression.Constant(stringFunc), leftParam);
				}
				if(t.IsPrimitive()) {
					if(t == leftParam.Type) {
						var func = MakePrimitiveIn(t, boxedValues);
						return Expression.Invoke(Expression.Constant(func), leftParam);
					} else if(t == Nullable.GetUnderlyingType(leftParam.Type)) {
						var func = MakeNullablePrimitiveIn(t, boxedValues);
						return Expression.Invoke(Expression.Constant(func), leftParam);
					}
				}
				Delegate enumerableAnyDelegate;
				{
					Delegate compiledComparer;
					{
						var parameterA = Expression.Parameter(leftParam.Type, "a");
						var parameterB = Expression.Parameter(t, "b");
						Expression compare = MakeCompare(parameterA, parameterB, true, i => i == 0, (x, y) => Expression.Equal(x, y), auxSettings);
						if(NullableHelpers.CanAcceptNull(compare.Type))
							compare = Expression.Coalesce(compare, Expression.Constant(false));
						compiledComparer = Expression.Lambda(compare, parameterA, parameterB).Compile();
					}
					var leftParam2 = Expression.Parameter(leftParam.Type, "left2");
					var parameterP = Expression.Parameter(t, "p");
					var predicateLambda = Expression.Lambda(Expression.Invoke(Expression.Constant(compiledComparer), leftParam2, parameterP), parameterP);
					Array typedArray = InSmartHelper.ToTypedArray(t, boxedValues);
					var enumerableAnyCall = Expression.Call(typeof(Enumerable), "Any", new Type[] { t }, Expression.Constant(typedArray), predicateLambda);
					enumerableAnyDelegate = Expression.Lambda(enumerableAnyCall, leftParam2).Compile();
				}
				Expression res = Expression.Invoke(Expression.Constant(enumerableAnyDelegate), leftParam);
				return res;
			}
		}
		static Expression MakeInSmart(Expression leftParam, IList<CriteriaOperator> operands, NestedLambdaCompiler nestedLambdaCompiler) {
			List<CriteriaOperator> nonValues = new List<CriteriaOperator>();
			Dictionary<Type, List<OperandValue>> valuesByType = new Dictionary<Type, List<OperandValue>>();
			if(operands.Count <= 6) {
				nonValues.AddRange(operands);
			} else {
				foreach(CriteriaOperator op in operands) {
					OperandValue v = op as OperandValue;
					if(ReferenceEquals(null, v) || v.Value == null) {
						nonValues.Add(op);
					} else {
						Type t = v.Value.GetType();
						List<OperandValue> values;
						if(!valuesByType.TryGetValue(t, out values)) {
							values = new List<OperandValue>();
							valuesByType.Add(t, values);
						}
						values.Add(v);
					}
				}
			}
			List<Expression> expressions = new List<Expression>();
			foreach(var typeValuesPair in valuesByType) {
				var list = typeValuesPair.Value;
				if(list.Count < 4) {
					nonValues.InsertRange(0, list);
				} else {
					Expression res = InSmartHelper.MakeInAgainstConstants(leftParam, list.Select(op => op.Value), typeValuesPair.Key, nestedLambdaCompiler.Context.AuxSettings);
					expressions.Add(res);
				}
			}
			expressions.AddRange(nonValues.Select(op => MakeCompare(leftParam, nestedLambdaCompiler.Process(op), true, i => i == 0, (x, y) => Expression.Equal(x, y), nestedLambdaCompiler.Context.AuxSettings)));
			return MakeGroupCore(GroupOperatorType.Or, expressions.ToArray(), 0, expressions.Count);
		}
		Expression ICriteriaVisitor<Expression>.Visit(GroupOperator theOperator) {
			var operands = Process(theOperator.Operands.Where(c => !ReferenceEquals(c, null))).ToArray();
			return MakeGroupCore(theOperator.OperatorType, operands, 0, operands.Length);
		}
		static Expression MakeGroupCore(GroupOperatorType opType, Expression[] operands, int start, int count) {
			switch(count){
				case 0:
					return Expression.Constant(null, typeof(bool?));
				case 1:
					return operands[start];
				default:
					break;
			}
			int subCount = count / 2;
			Expression right = MakeGroupCore(opType, operands, start, subCount);
			Expression left = MakeGroupCore(opType, operands, start + subCount, count - subCount);
			bool rightNullable = NullableHelpers.CanAcceptNull(right.Type);
			bool leftNullable = NullableHelpers.CanAcceptNull(left.Type);
			if(rightNullable != leftNullable) {
				if(!rightNullable)
					right = Expression.Convert(right, typeof(bool?));
				if(!leftNullable)
					left = Expression.Convert(left, typeof(bool?));
			}
			if(opType == GroupOperatorType.And)
				return Expression.AndAlso(right, left);
			else
				return Expression.OrElse(right, left);
		}
		Expression ICriteriaVisitor<Expression>.Visit(OperandValue theOperand) {
			return Expression.Constant(theOperand.Value);
		}
		Expression ICriteriaVisitor<Expression>.Visit(FunctionOperator theOperator) {
			GuardNulls(theOperator.Operands, i => "FunctionOperator.Operands[" + i.ToString() + "]");
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.Iif :
					return MakeIif(theOperator);
				case FunctionOperatorType.IsNull:
					return MakeIsNull(theOperator);
				case FunctionOperatorType.Custom:
				case FunctionOperatorType.CustomNonDeterministic:
					return MakeCustom(theOperator);
				case FunctionOperatorType.Concat:
					return MakeConcat(theOperator);
				default:
					break;
			}
			Expression[] args = Process(theOperator.Operands).ToArray();
			LambdaExpression fnLambda = EvalHelpers.MakeFnLambda(theOperator.OperatorType, args.Select(x => x.Type).ToArray(), this.Context.AuxSettings.CaseSensitive);
			return Expression.Invoke(fnLambda, args);
		}
		Expression MakeConcat(FunctionOperator theOperator) {
			switch(theOperator.Operands.Count) {
				case 0:
					return Expression.Constant(null, typeof(string));
				case 1:
					return EvalHelpers.SafeToString(Process(theOperator.Operands[0]));
			}
			ParameterExpression concater = Expression.Parameter(typeof(EvalHelpers.FnConcater), "concater");
			CriteriaCompilerLambdaContext lc = new CriteriaCompilerLambdaContext(this.Context, new KeyValuePair<ParameterExpression, Expression>(concater, Expression.New(typeof(EvalHelpers.FnConcater))));
			NestedLambdaCompiler nc = new NestedLambdaCompiler(lc);
			Expression body = Expression.Call(concater, "ToString", null);
			foreach(Expression e in theOperator.Operands.Select(o => EvalHelpers.SafeToString(nc.Process(o))).Reverse()) {
				body = Expression.Condition(Expression.Call(concater, "Append", null, e), body, Expression.Constant(null, typeof(string)));
			}
			return nc.Make(body);
		}
		Expression MakeCustom(FunctionOperator fn) {
			string customFnName = null;
			if(fn.Operands.Count >= 1) {
				OperandValue fnNameOv = fn.Operands[0] as OperandValue;
				if(!ReferenceEquals(fnNameOv, null)) {
					customFnName = fnNameOv.Value as string;
				}
			}
			if(string.IsNullOrEmpty(customFnName))
				throw new InvalidOperationException("Custom function had no name");
			ICustomFunctionOperator customFunction = null;
			CustomFunctionCollection cfc = this.Context.AuxSettings.AdditionalFunctions;
			if(cfc != null)
				customFunction = cfc.GetCustomFunction(customFnName);
			if(customFunction == null)
				customFunction = CriteriaOperator.GetCustomFunction(customFnName);
			if(customFunction == null)
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Custom function '{0}' not found.", customFnName));
			Expression[] processedArgs = Process(fn.Operands.Skip(1)).ToArray();
			ICustomFunctionOperatorCompileable compileable = customFunction as ICustomFunctionOperatorCompileable;
			if(compileable != null) {
				var withCaseCompileable = compileable as ICustomFunctionOperatorCompileableWithCaseSensitivity;
				if(withCaseCompileable != null) {
					return withCaseCompileable.Create(Context.AuxSettings.CaseSensitive, processedArgs);
				} else {
					return compileable.Create(processedArgs);
				}
			}
			Type rt = customFunction.ResultType(processedArgs.Select(a => a.Type).ToArray());
			if(rt.IsValueType() && Nullable.GetUnderlyingType(rt) == null)
				rt = typeof(Nullable<>).MakeGenericType(rt);
			Expression[] objectArgs = processedArgs.Select(p => p.Type == typeof(object) ? p : Expression.Convert(p, typeof(object))).ToArray();
			Expression e = Expression.Call(Expression.Constant(customFunction, typeof(ICustomFunctionOperator)), "Evaluate", null, Expression.NewArrayInit(typeof(object), objectArgs));
			if(rt != typeof(object))
				e = Expression.Convert(e, rt);
			return e;
		}
		Expression MakeIif(FunctionOperator theOperator) {
			if(theOperator.Operands.Count < 3 || theOperator.Operands.Count % 2 != 1)
				throw new InvalidOperationException("Incorrect arguments count: " + theOperator.ToString());
			using(IEnumerator<Expression> seq = Process(theOperator.Operands).Reverse().GetEnumerator()) {
				if(!seq.MoveNext())
					throw new InvalidOperationException("Internal error (first seq.MoveNext() failed)");
				Expression rv = seq.Current;
				bool moved = seq.MoveNext();
				while(moved) {
					Expression trueExpression = seq.Current;
					if(!seq.MoveNext())
						throw new InvalidOperationException("Internal error (seq.MoveNext() for condition failed)");
					Expression conditionExpression = seq.Current;
					rv = MakeElementaryConditional(conditionExpression, trueExpression, rv);
					moved = seq.MoveNext();
				}
				return rv;
			}
		}
		static Expression MakeElementaryConditional(Expression conditionExpression, Expression trueExpression, Expression falseExpression) {
			Expression te = trueExpression;
			Expression fe = falseExpression;
			ResolveElementaryConditionalType(ref te, ref fe);
			Expression ce = conditionExpression;
			if(NullableHelpers.CanAcceptNull(ce.Type))
				ce = Expression.Coalesce(ce, Expression.Constant(false));
			return Expression.Condition(ce, te, fe);
		}
		static void ResolveElementaryConditionalType(ref Expression left, ref Expression right) {
			if(left.Type == typeof(string)) {
				if(right.Type != typeof(string))
					right = EvalHelpers.SafeToString(right);
				return;
			}
			if(right.Type == typeof(string)) {
				left = EvalHelpers.SafeToString(left);
				return;
			}
			if(IsConstantUntypedNull(right)) {
				if(left.Type.IsValueType() && Nullable.GetUnderlyingType(left.Type) == null) {
					left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
				}
				right = Expression.Constant(null, left.Type);
				return;
			}
			if(IsConstantUntypedNull(left)) {
				if(right.Type.IsValueType() && Nullable.GetUnderlyingType(right.Type) == null) {
					right = Expression.Convert(right, typeof(Nullable<>).MakeGenericType(right.Type));
				}
				left = Expression.Constant(null, right.Type);
				return;
			}
			if(left.Type == typeof(object)) {
				if(right.Type != typeof(object))
					right = Expression.Convert(right, typeof(object));
				return;
			}
			if(right.Type == typeof(object)) {
				left = Expression.Convert(left, typeof(object));
				return;
			}
			Type lType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
			Type rType = Nullable.GetUnderlyingType(right.Type) ?? right.Type;
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
			if(resultType.IsValueType() && (Nullable.GetUnderlyingType(left.Type) != null || Nullable.GetUnderlyingType(right.Type) != null))
				resultType = typeof(Nullable<>).MakeGenericType(resultType);
			if(left.Type != resultType)
				left = Expression.Convert(left, resultType);
			if(right.Type != resultType)
				right = Expression.Convert(right, resultType);
		}
		Expression MakeIsNull(FunctionOperator theOperator) {
			bool simple;
			switch(theOperator.Operands.Count) {
				case 1:
					simple = true;
					break;
				case 2:
					simple = false;
					break;
				default:
					throw new InvalidOperationException("Incorrect arguments count: " + theOperator.ToString());
			}
			Expression firstOperand = Process(theOperator.Operands[0]);
			if(simple)
				return MakeSimpleIsNull(firstOperand);
			var firstParameter = Expression.Parameter(firstOperand.Type, "firstParameter");
			var lc = new CriteriaCompilerLambdaContext(this.Context, new KeyValuePair<ParameterExpression, Expression>(firstParameter, firstOperand));
			var nc = new NestedLambdaCompiler(lc);
			Expression unnulledFirst = firstParameter;
			if(Nullable.GetUnderlyingType(unnulledFirst.Type) != null)
				unnulledFirst = Expression.PropertyOrField(unnulledFirst, "Value");
			var body = MakeElementaryConditional(MakeSimpleIsNull(firstParameter), nc.Process(theOperator.Operands[1]), unnulledFirst);
			return nc.Make(body);
		}
		static Expression MakeSimpleIsNull(Expression operand) {
			if(operand.Type.IsValueType()) {
				if(Nullable.GetUnderlyingType(operand.Type) == null)
					return Expression.Constant(false);
				else
					return Expression.Not(Expression.PropertyOrField(operand, "HasValue"));
			} else {
				return Expression.Call(typeof(object), "ReferenceEquals", null, Expression.Constant(null, typeof(object)), Expression.Convert(operand, typeof(object)));
			}
		}
		Expression Process(CriteriaOperator criteria) {
			if(ReferenceEquals(null, criteria))
				throw new ArgumentNullException("criteria");
			try {
				return criteria.Accept(this);
			} catch(CriteriaCompilerLocalException) {
				throw;
			} catch(Exception e) {
				throw new CriteriaCompilerLocalException(e, criteria);
			}
		}
		IEnumerable<Expression> Process(IEnumerable<CriteriaOperator> ops) {
			foreach(CriteriaOperator op in ops)
				yield return Process(op);
		}
		CriteriaCompilerCore(CriteriaCompilerContext context) {
			this.Context = context;
		}
		public static LambdaExpression Compile(CriteriaCompilerDescriptor descriptor, CriteriaCompilerAuxSettings settings, CriteriaOperator op) {
			try {
				CriteriaCompilerRootContext context = new CriteriaCompilerRootContext(descriptor, settings);
				return Expression.Lambda(new CriteriaCompilerCore(context).Process(op), context.ThisExpression);
			} catch(CriteriaCompilerLocalException le) {
				throw new CriteriaCompilerException(op, le.Cause, le.InnerException);
			} catch(Exception e) {
				throw new CriteriaCompilerException(op, op, e);
			}
		}
		public class NestedLambdaCompiler {
			public readonly CriteriaCompilerNestedContext Context;
			CriteriaCompilerCore _Core;
			protected CriteriaCompilerCore Core {
				get {
					if(_Core == null)
						_Core = new CriteriaCompilerCore(Context);
					return _Core;
				}
			}
			public NestedLambdaCompiler(CriteriaCompilerNestedContext context) {
				Context = context;
			}
			public Expression Process(CriteriaOperator op) {
				return Core.Process(op);
			}
			public Expression Make(Expression body) {
				Expression lambda = Expression.Lambda(body, Context.GetParams());
				return Expression.Invoke(lambda, Context.GetArgs());
			}
			public Expression MakeCompiled(Expression body) {
				var lambda = Expression.Lambda(body, Context.GetParams());
				var compiled = lambda.Compile();
				return Expression.Invoke(Expression.Constant(compiled), Context.GetArgs());
			}
		}
		public abstract class CriteriaCompilerNestedContext: CriteriaCompilerContext {
			protected readonly CriteriaCompilerContext Parent;
			readonly List<Expression> AuxArgs = new List<Expression>();
			readonly List<ParameterExpression> AuxParams = new List<ParameterExpression>();
			Dictionary<int, CriteriaCompilerLocalContext> Map = new Dictionary<int, CriteriaCompilerLocalContext>();
			bool auxExported;
			public CriteriaCompilerNestedContext(CriteriaCompilerContext _Parent, params KeyValuePair<ParameterExpression, Expression>[] mandatoryParams) {
				this.Parent = _Parent;
				foreach(var pair in mandatoryParams) {
					AuxArgs.Add(pair.Value);
					AuxParams.Add(pair.Key);
				}
			}
			public ParameterExpression[] GetParams() {
				auxExported = true;
				return AuxParams.ToArray();
			}
			public Expression[] GetArgs() {
				auxExported = true;
				return AuxArgs.ToArray();
			}
			public sealed override CriteriaCompilerLocalContext GetLocalContext(int upLevels) {
				CriteriaCompilerLocalContext rv;
				if(Map.TryGetValue(upLevels, out rv))
					return rv;
				if(auxExported)
					throw new InvalidOperationException("auxExported");
				CriteriaCompilerLocalContext parentPair = GetParentPair(upLevels);
				ParameterExpression newParameter = Expression.Parameter(parentPair.Expression.Type, upLevels == 0 ? "self" : "up" + upLevels.ToString());
				rv = new CriteriaCompilerLocalContext(newParameter, parentPair.Descriptor);
				Map.Add(upLevels, rv);
				AuxArgs.Add(parentPair.Expression);
				AuxParams.Add(newParameter);
				return rv;
			}
			protected abstract CriteriaCompilerLocalContext GetParentPair(int upLevels);
			public override CriteriaCompilerAuxSettings AuxSettings {
				get { return Parent.AuxSettings; }
			}
		}
		public class CriteriaCompilerLambdaContext: CriteriaCompilerNestedContext {
			public CriteriaCompilerLambdaContext(CriteriaCompilerContext parent, params KeyValuePair<ParameterExpression, Expression>[] mandatoryParams)
				: base(parent, mandatoryParams) { }
			protected override CriteriaCompilerLocalContext GetParentPair(int upLevels) {
				return Parent.GetLocalContext(upLevels);
			}
		}
		public class CriteriaCompilerUpLevelsContext: CriteriaCompilerNestedContext {
			readonly int UpLevels;
			public CriteriaCompilerUpLevelsContext(CriteriaCompilerContext parent, int upLevels)
				: base(parent) {
				if(upLevels < 1)
					throw new ArgumentException("upLevels");
				this.UpLevels = upLevels;
			}
			protected override CriteriaCompilerLocalContext GetParentPair(int upLevels) {
				return Parent.GetLocalContext(upLevels + this.UpLevels);
			}
		}
		public class CriteriaCompilerDownLevelContext: CriteriaCompilerNestedContext {
			readonly CriteriaCompilerLocalContext SelfContext;
			public CriteriaCompilerDownLevelContext(CriteriaCompilerContext parent, CriteriaCompilerLocalContext _SelfContext)
				: base(parent) {
				this.SelfContext = _SelfContext;
			}
			protected override CriteriaCompilerLocalContext GetParentPair(int upLevels) {
				if(upLevels == 0)
					return SelfContext;
				else
					return Parent.GetLocalContext(upLevels - 1);
			}
		}
		public class CriteriaCompilerInsideAggregateClojureContext: CriteriaCompilerContext {
			public readonly CriteriaCompilerContext Donor;
			public CriteriaCompilerLocalContext SelfContext;
			public CriteriaCompilerInsideAggregateClojureContext(CriteriaCompilerContext _Donor, CriteriaCompilerLocalContext _SelfContext) {
				this.Donor = _Donor;
				this.SelfContext = _SelfContext;
			}
			public override CriteriaCompilerLocalContext GetLocalContext(int upLevels) {
				if(upLevels == 0)
					return SelfContext;
				else
					return Donor.GetLocalContext(upLevels);
			}
			public override CriteriaCompilerAuxSettings AuxSettings {
				get { return Donor.AuxSettings; }
			}
		}
	}
	public class CriteriaCompilerFreeJoinCriteriaReprocessor: IClientCriteriaVisitor<CriteriaOperator> {
		readonly int CutOffDepth;
		IList<OperandParameter> Map;
		CriteriaCompilerFreeJoinCriteriaReprocessor(int cutOffDepth, IList<OperandParameter> map) {
			this.CutOffDepth = cutOffDepth;
			this.Map = map;
		}
		CriteriaOperator ProcessProperty(OperandProperty prop) {
			int upLevels = 0;
			string propertyName = prop.PropertyName;
			while(propertyName.StartsWith("^.")) {
				++upLevels;
				propertyName = propertyName.Substring(2);
			}
			if(upLevels <= CutOffDepth)
				return prop;
			string nm = prop.PropertyName.Substring((CutOffDepth + 1) * 2);
			OperandParameter subst = new OperandParameter(nm, null);
			Map.Add(subst);
			return subst;
		}
		private CriteriaOperator SubProcess(int cutOffDepth, CriteriaOperator criteriaOperator) {
			return Process(cutOffDepth, criteriaOperator, Map);
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(AggregateOperand theOperand) {
			CriteriaOperator processedCollection_ = ProcessProperty(theOperand.CollectionProperty);
			OperandProperty processedCollection = processedCollection_ as OperandProperty;
			if(ReferenceEquals(processedCollection, null))
				throw new NotSupportedException("UpLevels in collection property name " + theOperand.CollectionProperty.PropertyName);
			int collectionPropertyDepth = EvaluatorProperty.CalcCollectionPropertyDepth(processedCollection.PropertyName);
			CriteriaOperator cond = SubProcess(CutOffDepth + collectionPropertyDepth, theOperand.Condition);
			CriteriaOperator aggr = SubProcess(CutOffDepth + collectionPropertyDepth, theOperand.AggregatedExpression);
			return new AggregateOperand(processedCollection, aggr, theOperand.AggregateType, cond);
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(OperandProperty theOperand) {
			return ProcessProperty(theOperand);
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(JoinOperand theOperand) {
			return DoFreeJoin(theOperand);
		}
		JoinOperand DoFreeJoin(JoinOperand theOperand) {
			CriteriaOperator cond = SubProcess(CutOffDepth + 1, theOperand.Condition);
			CriteriaOperator aggr = SubProcess(CutOffDepth + 1, theOperand.AggregatedExpression);
			return new JoinOperand(theOperand.JoinTypeName, cond, theOperand.AggregateType, aggr);
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BetweenOperator theOperator) {
			return new BetweenOperator(Process(theOperator.TestExpression), Process(theOperator.BeginExpression), Process(theOperator.EndExpression));
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BinaryOperator theOperator) {
			return new BinaryOperator(Process(theOperator.LeftOperand), Process(theOperator.RightOperand), theOperator.OperatorType);
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(UnaryOperator theOperator) {
			return new UnaryOperator(theOperator.OperatorType, Process(theOperator.Operand));
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(InOperator theOperator) {
			return new InOperator(Process(theOperator.LeftOperand), Process(theOperator.Operands));
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(GroupOperator theOperator) {
			return GroupOperator.Combine(theOperator.OperatorType, Process(theOperator.Operands));
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(OperandValue theOperand) {
			return new ConstantValue(theOperand.Value);
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(FunctionOperator theOperator) {
			return new FunctionOperator(theOperator.OperatorType, Process(theOperator.Operands));
		}
		CriteriaOperator Process(CriteriaOperator op) {
			if(ReferenceEquals(op, null))
				return null;
			return op.Accept(this);
		}
		IEnumerable<CriteriaOperator> Process(IEnumerable<CriteriaOperator> ops) {
			foreach(CriteriaOperator op in ops)
				yield return Process(op);
		}
		static CriteriaOperator Process(int depth, CriteriaOperator op, IList<OperandParameter> mappings) {
			if(ReferenceEquals(op, null))
				return null;
			return new CriteriaCompilerFreeJoinCriteriaReprocessor(depth, mappings).Process(op);
		}
		public static CriteriaOperator Process(CriteriaOperator op, out OperandParameter[] tbdValues) {
			List<OperandParameter> map = new List<OperandParameter>();
			var rv = Process(0, op, map);
			tbdValues = map.ToArray();
			return rv;
		}
	}
	public static class PropertyDescriptorCriteriaCompilationSupport {
		public interface IHelper {
			Expression TryMakeFastExpression(Expression baseExpression);
			Delegate TryGetFastGetter(out Type rowType, out Type valueType);
		}
		const string ReflectPropertyDescriptorTypeName = "System.ComponentModel.ReflectPropertyDescriptor";
		const string ReflectPropertyDescriptorCompatibilityTypeName = "DevExpress.Compatibility.System.ComponentModel.ReflectPropertyDescriptor";
		const string ReflectPropertyDescriptorDxTypeName = "DevExpress.Data.Browsing.ReflectPropertyDescriptor";
		public static bool IsReflectPropertyDescriptor(PropertyDescriptor pd) {
			if(pd == null)
				return false;
			string pdType = pd.GetType().FullName;
			return ReflectPropertyDescriptorTypeName == pdType || ReflectPropertyDescriptorCompatibilityTypeName == pdType || ReflectPropertyDescriptorDxTypeName == pdType;
		}
		static readonly Dictionary<Tuple<Type, string>, Tuple<Delegate, Type, Type>> fastReflects = new Dictionary<Tuple<Type, string>, Tuple<Delegate, Type, Type>>();
		static Tuple<Delegate, Type, Type> TryMakeFastAccessCore(Type t, string memberName) {
			lock(fastReflects) {
				var index = Tuple.Create(t, memberName);
				Tuple<Delegate, Type, Type> result;
				if(fastReflects.TryGetValue(index, out result)) {
					return result;
				}
				if(!CompileHelper.IsPublicExposable(t)) {
					fastReflects.Add(index, null);
					return null;
				}
				var member = CompileHelper.FindPropertyOrField(t, memberName, false, false) as PropertyInfo;
				if(member == null || !CompileHelper.IsPublicExposable(member.DeclaringType) || !CompileHelper.IsPublicExposable(member.PropertyType)) {
					fastReflects.Add(index, null);
					return null;
				}
				if(member.DeclaringType != t || member.Name != memberName) {
					var coreIndex = Tuple.Create(member.DeclaringType, member.Name);
					if(fastReflects.TryGetValue(coreIndex, out result)) {
						fastReflects.Add(index, result);
						return result;
					}
					result = MakeFastAccessorCoreCore(member);
					fastReflects.Add(coreIndex, result);
					fastReflects.Add(index, result);
					return result;
				} else {
					result = MakeFastAccessorCoreCore(member);
					fastReflects.Add(index, result);
					return result;
				}
			}
		}
		static Tuple<Delegate, Type, Type> MakeFastAccessorCoreCore(PropertyInfo member) {
			if(CompileHelper.CanDynamicMethodWithSkipVisibility()) {
				DynamicMethod method = new DynamicMethod(String.Empty, member.PropertyType, new Type[] { member.DeclaringType }, member.DeclaringType.IsInterface() ? typeof(PropertyDescriptorCriteriaCompilationSupport) : member.DeclaringType, true);
				var ilGen = method.GetILGenerator();
				ilGen.Emit(OpCodes.Ldarg_0);
				MethodInfo mi = member.GetGetMethod(true);
				ilGen.Emit(mi.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, mi);
				ilGen.Emit(OpCodes.Ret);
				var dlgType = typeof(Func<,>).MakeGenericType(member.DeclaringType, member.PropertyType);
				var dlg = method.CreateDelegate(dlgType);
				return Tuple.Create(dlg, member.DeclaringType, member.PropertyType);
			} else {
				var parm = Expression.Parameter(member.DeclaringType, "component");
				var body = Expression.MakeMemberAccess(parm, member);
				var compiled = Expression.Lambda(body, parm).Compile();
				return Tuple.Create(compiled, member.DeclaringType, body.Type);
			}
		}
		static Delegate TryMakeFastAccessForReflectPD(PropertyDescriptor pd, out Type rowType, out Type valueType) {
			if(IsReflectPropertyDescriptor(pd)) {
				var fastAccess = TryMakeFastAccessCore(pd.ComponentType, pd.Name);
				if(fastAccess != null) {
					rowType = fastAccess.Item2;
					valueType = fastAccess.Item3;
					return fastAccess.Item1;
				}
			}
			rowType = null;
			valueType = null;
			return null;
		}
		public static Expression TryMakeFastAccessFromDescriptor(Expression baseExpression, PropertyDescriptor pd) {
			IHelper helper = pd as IHelper;
			if(helper != null) {
				var help = helper.TryMakeFastExpression(baseExpression);
				if(help != null)
					return help;
			}
			Type rowType, valueType;
			var dlg = TryMakeFastAccessForReflectPD(pd, out rowType, out valueType);
			if(dlg != null) {
				Expression row = baseExpression;
				if(row.Type == typeof(object))
					row = Expression.Convert(row, rowType);
				return Expression.Invoke(Expression.Constant(dlg), row);
			}
			return null;
		}
		public static Delegate TryGetFastGetter(PropertyDescriptor pd, out Type rowType, out Type valueType){
			IHelper helper = pd as IHelper;
			if(helper != null) {
				var help = helper.TryGetFastGetter(out rowType, out valueType);
				if(help != null)
					return help;
			}
			var dlg = TryMakeFastAccessForReflectPD(pd, out rowType, out valueType);
			return dlg;
		}
	}
	public abstract class CriteriaCompilerContextDescriptorDefaultBase: CriteriaCompilerDescriptor {
		static IEnumerable ToCollection(object o) {
			IEnumerable e = (IEnumerable)o;
			ITypedList tl = e as ITypedList;
			if(tl == null)
				return e;
			else
				return WrapTypedCollection(tl, e);
		}
		static IEnumerable WrapTypedCollection(ITypedList tl, IEnumerable e) {
			var rv = new CriteriaCompilerContextDescriptorReflective.TypedListUndObjectPair(tl.GetItemProperties(null));
			foreach(object obj in e) {
				rv.Row = obj;
				yield return rv;
			}
		}
		public override CriteriaCompilerRefResult DiveIntoCollectionProperty(Expression baseExpression, string collectionPropertyPath) {
			if(string.IsNullOrEmpty(collectionPropertyPath))
				throw new InvalidOperationException("Top-level aggregates not supported by " + this.GetType().FullName);
			int dotPos = collectionPropertyPath.IndexOf('.');
			if(dotPos < 0) {
				Expression collection = MakePropertyAccessCoreChecked(baseExpression, collectionPropertyPath);
				Type rowType = GenericTypeHelper.GetGenericIListTypeArgument(collection.Type);
				if(typeof(ITypedList).IsAssignableFrom(collection.Type) || rowType == null || rowType == typeof(object) || typeof(ICustomTypeDescriptor).IsAssignableFrom(rowType)) {
					Func<object, IEnumerable> interpretationCollectionWrapper = ToCollection;
					Expression convertedCollection = Expression.Invoke(Expression.Constant(interpretationCollectionWrapper), collection);
					return new CriteriaCompilerRefResult(new CriteriaCompilerLocalContext(convertedCollection, CriteriaCompilerDescriptor.Get()), null);
				} else {
					return new CriteriaCompilerRefResult(new CriteriaCompilerLocalContext(collection, CriteriaCompilerDescriptor.Get(rowType)), null);
				}
			} else {
				string refProperty = collectionPropertyPath.Substring(0, dotPos);
				string subProperty = collectionPropertyPath.Substring(dotPos + 1);
				Expression refExpression = MakePropertyAccessCoreChecked(baseExpression, refProperty);
				CriteriaCompilerDescriptor descr;
				if(refExpression.Type == typeof(object) || typeof(ICustomTypeDescriptor).IsAssignableFrom(refExpression.Type) || typeof(ITypedList).IsAssignableFrom(refExpression.Type))
					descr = CriteriaCompilerDescriptor.Get();
				else
					descr = CriteriaCompilerDescriptor.Get(refExpression.Type);
				return new CriteriaCompilerRefResult(new CriteriaCompilerLocalContext(refExpression, descr), subProperty);
			}
		}
		public override Expression MakePropertyAccess(Expression baseExpression, string propertyPath) {
			int dotPos = propertyPath.IndexOf('.');
			if(dotPos < 0) {
				if(EvaluatorProperty.GetIsThisProperty(propertyPath))
					return MakeThisAccess(baseExpression);
				Expression ex = MakePropertyAccessCoreChecked(baseExpression, propertyPath);
				return LeafProcessing(ex, propertyPath);
			} else {
				Expression ex = MakePathAccess(baseExpression, propertyPath);
				if(ex != null)
					return LeafProcessing(ex, propertyPath);
			}
			var dive = DiveIntoCollectionProperty(baseExpression, propertyPath);
			Expression refExpression = dive.LocalContext.Expression;
			ParameterExpression subParameter = Expression.Parameter(refExpression.Type, "sub");
			Expression subAccess = dive.LocalContext.Descriptor.MakePropertyAccess(subParameter, dive.SubProperty);
			Expression body;
			if(!NullableHelpers.CanAcceptNull(subParameter.Type))
				body = subAccess;
			else {
				Type rvType = NullableHelpers.GetUnBoxedType(subAccess.Type);
				Expression convertedSubAccess = rvType == subAccess.Type ? subAccess : Expression.Convert(subAccess, rvType);
				body = Expression.Condition(Expression.Call(typeof(object), "ReferenceEquals", null, subParameter, Expression.Constant(null)), Expression.Constant(null, rvType), convertedSubAccess);
			}
			LambdaExpression subLambda = Expression.Lambda(body, subParameter);
			return Expression.Invoke(subLambda, refExpression);
		}
		static object MayBeRefAsCollectionExtractor(object mayBeRefAsCollection, string propertyPathForException) {
			ITypedList tl = mayBeRefAsCollection as ITypedList;
			if(tl != null) {
				IList l = tl as IList;
				if(l != null) {
					switch(l.Count){
						case 0:
							return null;
						case 1:
							return l[0];
						default:
							throw new ArgumentException("single row expected at '" + propertyPathForException + "', provided: " + l.Count.ToString());	
					}
				}
			}
			return mayBeRefAsCollection;
		}
		Expression LeafProcessing(Expression ex, string propertyPathForException) {
			if(ex.Type == typeof(object) || (typeof(ITypedList).IsAssignableFrom(ex.Type) && typeof(IList).IsAssignableFrom(ex.Type))) {
				Func<object, string, object> extractor = MayBeRefAsCollectionExtractor;
				ex = Expression.Invoke(Expression.Constant(extractor), ex, Expression.Constant(propertyPathForException));
			}
			return ex;
		}
		protected Expression MakePropertyAccessCoreChecked(Expression baseExpression, string property) {
			var ex = MakePropertyAccessCore(baseExpression, property);
			if(ex == null)
				throw new InvalidPropertyPathException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorInvalidPropertyPath, property));
			return ex;
		}
		protected abstract Expression MakePropertyAccessCore(Expression baseExpression, string property);
		protected virtual Expression MakePathAccess(Expression baseExpression, string propertyPath) {
			return null;
		}
		public static object KillDBNull(object nullableSomethig) {
			if(nullableSomethig is DBNull)
				return null;
			else
				return nullableSomethig;
		}
		protected virtual Expression MakeThisAccess(Expression baseExpression) {
			return baseExpression;
		}
	}
	public class CriteriaCompiledContextDescriptorTyped: CriteriaCompilerContextDescriptorDefaultBase {
		public CriteriaCompiledContextDescriptorTyped(Type type) {
			this.Type = type;
		}
		readonly Type Type;
		protected override Expression MakePropertyAccessCore(Expression baseExpression, string property) {
			var member = CompileHelper.FindPropertyOrField(baseExpression.Type, property, true, true);
			if(member != null)
				return Expression.MakeMemberAccess(baseExpression, member);
			return null;
		}
		public override Type ObjectType {
			get { return Type; }
		}
	}
	public class CriteriaCompiledContextDescriptorDescripted: CriteriaCompilerContextDescriptorDefaultBase {
		readonly PropertyDescriptorCollection PDs;
		public CriteriaCompiledContextDescriptorDescripted(PropertyDescriptorCollection pds) {
			this.PDs = pds;
		}
		public static Expression MakeAccessFromDescriptor(Expression baseExpression, PropertyDescriptor pd) {
			Expression fastExpression = PropertyDescriptorCriteriaCompilationSupport.TryMakeFastAccessFromDescriptor(baseExpression, pd);
			if(fastExpression != null)
				return fastExpression;
			Func<object, object> pdAccess = x => KillDBNull(pd.GetValue(x));		
			Expression rv = Expression.Invoke(Expression.Constant(pdAccess), baseExpression);
			Type t = NullableHelpers.GetUnBoxedType(pd.PropertyType);
			if(pd.PropertyType.IsEnum()) {
				ParameterExpression rvParameter = Expression.Parameter(rv.Type, "rv");
				rv = Expression.Invoke(Expression.Lambda(Expression.Condition(Expression.TypeIs(rvParameter, Enum.GetUnderlyingType(pd.PropertyType)), Expression.Convert(Expression.Convert(rvParameter, pd.PropertyType), rv.Type != t ? t : typeof(object)), (rv.Type != t ? Expression.Convert(rvParameter, t) : (Expression)rvParameter)), rvParameter), rv);
			} else {
				if(rv.Type != t)
					rv = Expression.Convert(rv, t);
			}
			return rv;
		}
		protected override Expression MakePropertyAccessCore(Expression baseExpression, string property) {
			return CoreAccess(baseExpression, property);
		}
		Expression CoreAccess(Expression baseExpression, string propertPath) {
			PropertyDescriptor pd = PDs.Find(propertPath, false) ?? PDs.Find(propertPath, true);
			if(pd != null) {
				return MakeAccessFromDescriptor(baseExpression, pd);
			}
			return null;
		}
		protected override Expression MakePathAccess(Expression baseExpression, string propertyPath) {
			return CoreAccess(baseExpression, propertyPath);
		}
		public override Type ObjectType {
			get { return typeof(object); }
		}
	}
	public class CriteriaCompilerContextDescriptorExpando: CriteriaCompilerContextDescriptorDefaultBase {
		public static readonly CriteriaCompilerContextDescriptorExpando Instance = new CriteriaCompilerContextDescriptorExpando();
		protected override Expression MakePropertyAccessCore(Expression baseExpression, string property) {
			Func<IDictionary<string, object>, object> accessor = expando => {
				return expando[property];
			};
			return Expression.Invoke(Expression.Constant(accessor), baseExpression);
		}
		public override Type ObjectType {
			get { return typeof(IDictionary<string, object>); }
		}
	}
	public class CriteriaCompilerContextDescriptorReflective: CriteriaCompilerContextDescriptorDefaultBase {
		public static readonly CriteriaCompilerContextDescriptorReflective Instance = new CriteriaCompilerContextDescriptorReflective();
		CriteriaCompilerContextDescriptorReflective() { }
		public class TypedListUndObjectPair {
			public readonly PropertyDescriptorCollection PDs;
			public object Row;
			public TypedListUndObjectPair(PropertyDescriptorCollection _PDs) {
				this.PDs = _PDs;
			}
		}
		public class ReflectiveAccessor {
			public readonly string PropertyName;
			public ReflectiveAccessor(string propertyName) {
				this.PropertyName = propertyName;
			}
			Func<object, object> CachedAccesor;
			bool AccessorCached;
			public object GetReflectiveValue(object source) {
				if(source == null)
					return null;
				if(AccessorCached)
					return CachedAccesor(source);
				{
					IListSource ls = source as IListSource;
					if(ls != null) {
						source = ls.GetList();
						if(source == null)
							return null;
					}
				}
				{
					var pair = source as TypedListUndObjectPair;
					if(pair != null) {
						return AccessThroughPropertyDescriptors(pair.PDs, pair.Row);
					}
				}
				{
					ICustomTypeDescriptor ctd = source as ICustomTypeDescriptor;
					if(ctd != null) {
						return AccessThroughPropertyDescriptors(ctd.GetProperties(), ctd);
					}
				}
				{
					var tl = source as ITypedList;
					if(tl != null) {
						var l = source as IList;
						if(l != null) {
							if(l.Count == 1) {
								return AccessThroughPropertyDescriptors(tl.GetItemProperties(null), l[0]);
							} else if(l.Count == 0) {
								return null;
							} else {
								throw new ArgumentException("single row expected at '" + this.PropertyName + "', provided: " + l.Count.ToString());	
							}
						}
					}
				}
				lock(this) {
					if(!AccessorCached) {
						if (source is System.Dynamic.ExpandoObject) {
							CachedAccesor = src => {
								var expando = (IDictionary<string, object>)src;
								return expando[PropertyName];
							};
							AccessorCached = true;
						} else {
#if DXPORTABLE
							List<MemberInfo> memberList = new List<MemberInfo>();
							foreach (PropertyInfo info in source.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)) {
								if (info.Name == PropertyName)
									memberList.Add(info);
							}
							foreach (FieldInfo info in source.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)) {
								if (info.Name == PropertyName)
									memberList.Add(info);
							}
							MemberInfo[] members = memberList.ToArray();
#else
							MemberInfo[] members = source.GetType().GetMember(PropertyName, System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
#endif
							Type baseDeclType = members.Select(mi => mi.DeclaringType).Aggregate((Type)null, (acc, next) => (acc != null && acc.IsAssignableFrom(next)) ? acc : next);
							if(baseDeclType == null)
								throw new InvalidPropertyPathException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorInvalidPropertyPath, PropertyName));
							ParameterExpression pe = Expression.Parameter(typeof(object), "");
							Expression body = Expression.PropertyOrField(Expression.Convert(pe, baseDeclType), PropertyName);
							if(body.Type != typeof(object))
								body = Expression.Convert(body, typeof(object));
							CachedAccesor = Expression.Lambda<Func<object, object>>(body, pe).Compile();
							AccessorCached = true;
						}
					}
				}
				return CachedAccesor(source);
			}
			object AccessThroughPropertyDescriptors(PropertyDescriptorCollection props, object row) {
				PropertyDescriptor pd = props.Find(PropertyName, false) ?? props.Find(PropertyName, true);
				if(pd == null)
					throw new InvalidPropertyPathException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorInvalidPropertyPath, PropertyName));
				return KillDBNull(pd.GetValue(row));
			}
		}
		protected override Expression MakePropertyAccessCore(Expression baseExpression, string property) {
			ReflectiveAccessor acc = new ReflectiveAccessor(property);
			return Expression.Call(Expression.Constant(acc), "GetReflectiveValue", null, baseExpression);
		}
		public override Type ObjectType {
			get { return typeof(object); }
		}
		static object GetThis(object obj) {
			TypedListUndObjectPair wrappedInPlastic = obj as TypedListUndObjectPair;
			if(wrappedInPlastic != null)
				return wrappedInPlastic.Row;
			return obj;
		}
		protected override Expression MakeThisAccess(Expression baseExpression) {
			Func<object, object> getThisDelegate = GetThis;
			return Expression.Invoke(Expression.Constant(getThisDelegate), baseExpression);
		}
	}
	public class DefaultTopLevelCriteriaCompilerContextDescriptor<T>: CriteriaCompilerDescriptor {
		public override Type ObjectType {
			get { return typeof(IEnumerable<T>); }
		}
		public override Expression MakePropertyAccess(Expression baseExpression, string propertyPath) {
			throw new InvalidOperationException(this.GetType().FullName);
		}
		public override CriteriaCompilerRefResult DiveIntoCollectionProperty(Expression baseExpression, string collectionName) {
			if(!string.IsNullOrEmpty(collectionName))
				throw new ArgumentException("collectionName");
			return new CriteriaCompilerRefResult(new CriteriaCompilerLocalContext(baseExpression, CriteriaCompilerDescriptor.Get<T>()), null);
		}
	}
	public class DefaultTopLevelCriteriaCompilerContextDescriptor: CriteriaCompilerDescriptor {
		readonly CriteriaCompilerDescriptor RowDescriptor;
		public DefaultTopLevelCriteriaCompilerContextDescriptor(CriteriaCompilerDescriptor rowDescriptor) {
			this.RowDescriptor = rowDescriptor;
		}
		public override Type ObjectType {
			get { return typeof(IEnumerable); }
		}
		public override Expression MakePropertyAccess(Expression baseExpression, string propertyPath) {
			throw new InvalidOperationException(this.GetType().FullName);
		}
		public override CriteriaCompilerRefResult DiveIntoCollectionProperty(Expression baseExpression, string collectionName) {
			if(!string.IsNullOrEmpty(collectionName))
				throw new ArgumentException("collectionName");
			return new CriteriaCompilerRefResult(new CriteriaCompilerLocalContext(baseExpression, RowDescriptor), null);
		}
	}
	public class CriteriaCompilerDescriptorITypedListComplete: CriteriaCompilerDescriptor {
		readonly ITypedList Root;
		readonly PropertyDescriptor[] ListAccessors;
		public CriteriaCompilerDescriptorITypedListComplete(ITypedList _Root, PropertyDescriptor[] _ListAccessors) {
			this.Root = _Root;
			this.ListAccessors = _ListAccessors;
		}
		PropertyDescriptorCollection _PDs;
		PropertyDescriptorCollection PDs {
			get {
				if(_PDs == null)
					_PDs = Root.GetItemProperties(ListAccessors);
				return _PDs;
			}
		}
		public override Type ObjectType {
			get { return typeof(object); }
		}
		public override CriteriaCompilerRefResult DiveIntoCollectionProperty(Expression baseExpression, string collectionPropertyPath) {
			return base.DiveIntoCollectionProperty(baseExpression, collectionPropertyPath);
		}
		public override Expression MakePropertyAccess(Expression baseExpression, string propertyPath) {
			throw new NotImplementedException();
		}
	}
}
