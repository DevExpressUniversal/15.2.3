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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Utils;
using System.Reflection;
namespace DevExpress.Data.Helpers {
	public abstract class GenericInvoker<Signature, ImplType> where ImplType : GenericInvoker<Signature, ImplType> {
		class TypesArrayComparer : IEqualityComparer<Type[]> {
			TypesArrayComparer() { }
			public static readonly TypesArrayComparer Instance = new TypesArrayComparer();
			public bool Equals(Type[] x, Type[] y) {
				if (x.Length != y.Length)
					throw new InvalidOperationException("x.Length != y.Length");
				for (int i = 0; i < x.Length; ++i) {
					if (x[i] != y[i])
						return false;
				}
				return true;
			}
			public int GetHashCode(Type[] types) {
				int hash = 0;
				for (int i = 0; i < types.Length; ++i) {
					var t = types[i];
					if (t == null)
						throw new ArgumentNullException("types[" + i.ToString() + "]");
					hash = (hash << 5) + hash ^ t.GetHashCode();
				}
				return hash;
			}
		}
		static readonly Type ImplGenericTypeDefinition = typeof(ImplType).GetGenericTypeDefinition();
		static readonly System.Collections.Concurrent.ConcurrentDictionary<Type[], Signature> Store = new System.Collections.Concurrent.ConcurrentDictionary<Type[], Signature>(TypesArrayComparer.Instance);
		protected abstract Signature CreateInvoker();
		static Signature CreateInvoker(Type[] genericArgs) {
			return ((GenericInvoker<Signature, ImplType>)Activator.CreateInstance(ImplGenericTypeDefinition.MakeGenericType(genericArgs))).CreateInvoker();
		}
		public static Signature GetInvoker(params Type[] genericArgs) {
			return Store.GetOrAdd(genericArgs, CreateInvoker);
		}
	}
	public static class GenericEnumerableHelper {
		public abstract class SelectApplier : GenericInvoker<Func<IEnumerable, Delegate, IEnumerable>, SelectApplier.Impl<object, object>> {
			public class Impl<T, R> : SelectApplier {
				static IEnumerable<R> ApplySelect(IEnumerable<T> src, Func<T, R> selector) {
					return src.Select(selector);
				}
				protected override Func<IEnumerable, Delegate, IEnumerable> CreateInvoker() {
					return (src, selector) => ApplySelect((IEnumerable<T>)src, (Func<T, R>)selector);
				}
			}
		}
		public static IEnumerable ApplySelect(this IEnumerable src, Type srcGenericArgument, Delegate selector, Type selectorResultType) {
			return SelectApplier.GetInvoker(srcGenericArgument, selectorResultType)(src, selector);
		}
		public static IEnumerable ApplySelect<TSource>(this IEnumerable<TSource> src, Delegate selector, Type selectorResultType) {
			return ApplySelect(src, typeof(TSource), selector, selectorResultType);
		}
		public static IEnumerable<TResult> ApplySelect<TResult>(this IEnumerable src, Type srcGenericArgument, Delegate selector) {
			return (IEnumerable<TResult>)ApplySelect(src, srcGenericArgument, selector, typeof(TResult));
		}
		public static IEnumerable ApplyCast(this IEnumerable src, Type srcGenericArgument, Type castToType, Func<string[]> exceptionAuxInfoGetter = null) {
			if (srcGenericArgument == castToType)
				return src;
			var cast = GenericDelegateHelper.GetCastFunc(srcGenericArgument, castToType, exceptionAuxInfoGetter);
			return src.ApplySelect(srcGenericArgument, cast, castToType);
		}
		public static IEnumerable ApplyCast<T>(this IEnumerable<T> src, Type castToType, Func<string[]> exceptionAuxInfoGetter = null) {
			return ApplyCast(src, typeof(T), castToType, exceptionAuxInfoGetter);
		}
		public static IEnumerable<T> ApplyCast<T>(this IEnumerable src, Type srcGenericArgument, Func<string[]> exceptionAuxInfoGetter = null) {
			return (IEnumerable<T>)ApplyCast(src, srcGenericArgument, typeof(T), exceptionAuxInfoGetter);
		}
		public abstract class CountApplier : GenericInvoker<Func<IEnumerable, int>, CountApplier.Impl<object>> {
			public class Impl<T> : CountApplier {
				static int ApplyCount(IEnumerable<T> src) {
					return src.Count();
				}
				protected override Func<IEnumerable, int> CreateInvoker() {
					return (src) => ApplyCount((IEnumerable<T>)src);
				}
			}
		}
		public static int ApplyCount(this IEnumerable src, Type srcGenericArgument) {
			return CountApplier.GetInvoker(srcGenericArgument)(src);
		}
		public abstract class WhereApplier : GenericInvoker<Func<IEnumerable, Delegate, IEnumerable>, WhereApplier.Impl<object>> {
			public class Impl<T> : WhereApplier {
				static IEnumerable<T> Apply(IEnumerable<T> src, Func<T, bool> where) {
					return src.Where(where);
				}
				protected override Func<IEnumerable, Delegate, IEnumerable> CreateInvoker() {
					return (enumerable, predicate) => Apply((IEnumerable<T>)enumerable, (Func<T, bool>)predicate);
				}
			}
		}
		public static IEnumerable ApplyWhere(this IEnumerable src, Delegate predicate, Type srcGenericArgument) {
			return WhereApplier.GetInvoker(srcGenericArgument)(src, predicate);
		}
		public static IEnumerable ApplyWhereNotNull(this IEnumerable src, Type srcGenericArgument) {
			if (NullableHelpers.CanAcceptNull(srcGenericArgument))
				return src.ApplyWhere(GenericDelegateHelper.MakeNullCheck(srcGenericArgument, true), srcGenericArgument);
			else
				return src;
		}
	}
	public static class GenericDelegateHelper {
		static readonly System.Collections.Concurrent.ConcurrentDictionary<Tuple<Type, Type>, Delegate> Conversions = new System.Collections.Concurrent.ConcurrentDictionary<Tuple<Type, Type>, Delegate>();
		static Delegate GetCastFuncCore(Type from, Type to) {
			if (from == to)
				throw new ArgumentException("Identity requested!");
			if (from == null)
				throw new ArgumentNullException("from");
			if (to == null)
				throw new ArgumentNullException("to");
			return Conversions.GetOrAdd(Tuple.Create(from, to), CreateConversion);
		}
#if DEBUG
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static R ConversionMarker<S, R>(Func<S, R> conversion, S src) {
			return conversion(src);
		}
#endif
		static Delegate CreateConversion(Tuple<Type, Type> types) {
			if (types.Item1 == typeof(object))
				return UnboxingConversionCreator.GetInvoker(types.Item2)();
			if (types.Item2 == typeof(object))
				return BoxingConversionCreator.GetInvoker(types.Item1)();
			if (types.Item1 == Nullable.GetUnderlyingType(types.Item2))
				return ToNullableConversionCreator.GetInvoker(types.Item1)();
			var prm = Expression.Parameter(types.Item1);
			var conversion = Expression.Convert(prm, types.Item2);
			var rv = Expression.Lambda(conversion, prm).Compile();
#if !DEBUG
			return rv;
#else
			var paramMarked = Expression.Parameter(types.Item1);
			var marked = Expression.Lambda(Expression.Call(typeof(GenericDelegateHelper), "ConversionMarker", new Type[] { types.Item1, types.Item2 }, Expression.Constant(rv), paramMarked), paramMarked).Compile();
			return marked;
#endif
		}
		public abstract class UnboxingConversionCreator : GenericInvoker<Func<Delegate>, UnboxingConversionCreator.Impl<object>> {
			public class Impl<T> : UnboxingConversionCreator {
				static T Unbox(object arg) {
					return (T)arg;
				}
				protected override Func<Delegate> CreateInvoker() {
					return () => (Func<object, T>)Unbox;
				}
			}
		}
		public abstract class BoxingConversionCreator : GenericInvoker<Func<Delegate>, BoxingConversionCreator.Impl<object>> {
			public class Impl<T> : BoxingConversionCreator {
				static object Box(T arg) {
					return arg;
				}
				protected override Func<Delegate> CreateInvoker() {
					return () => (Func<T, object>)Box;
				}
			}
		}
		public abstract class ToNullableConversionCreator : GenericInvoker<Func<Delegate>, ToNullableConversionCreator.Impl<int>> {
			public class Impl<T> : ToNullableConversionCreator where T : struct {
				static T? Nullise(T arg) {
					return arg;
				}
				protected override Func<Delegate> CreateInvoker() {
					return () => (Func<T, T?>)Nullise;
				}
			}
		}
		public abstract class InvalidCastVerboseLogger : GenericInvoker<Func<Delegate, Func<string[]>, Delegate>, InvalidCastVerboseLogger.Impl<object, object>> {
			public class Impl<A, R> : InvalidCastVerboseLogger {
				static R LoggedCast(Func<A, R> nakedCast, A arg, Func<string[]> exceptionAuxInfoGetter = null) {
					try {
						return nakedCast(arg);
					} catch (InvalidCastException ice) {
						string[] auxParams;
						try {
							auxParams = exceptionAuxInfoGetter() ?? new string[0];
						} catch (Exception e) {
							auxParams = new string[] { "exceptionAuxInfoGetter Exception: " + e.Message };
						}
						string auxString = string.Join(", ", new string[] { "Original Message: " + ice.Message }.Concat(auxParams).Select(s => "(" + s + ")"));
						string message = string.Format("Unable to cast object '{2}' of type '{3}' from type '{0}' to type '{1}'. {4}", typeof(A).Name, typeof(R).Name, arg, arg == null ? "none" : arg.GetType().Name, auxString);
						throw new InvalidCastException(message, ice);
					}
				}
				static Func<A, R> CreateLoggedCast(Func<A, R> nakedCast, Func<string[]> exceptionAuxInfoGetter = null) {
					return a => LoggedCast(nakedCast, a, exceptionAuxInfoGetter);
				}
				protected override Func<Delegate, Func<string[]>, Delegate> CreateInvoker() {
					return (nakedCast, exceptionAuxInfoGetter) => CreateLoggedCast((Func<A, R>)nakedCast, exceptionAuxInfoGetter);
				}
			}
		}
		public static Delegate GetCastFunc(Type from, Type to, Func<string[]> exceptionAuxInfoGetter = null) {
			Delegate caster = GetCastFuncCore(from, to);
			if (exceptionAuxInfoGetter == null)
				return caster;
			else
				return InvalidCastVerboseLogger.GetInvoker(from, to)(caster, exceptionAuxInfoGetter);
		}
		public static Func<From, To> GetCastFunc<From, To>(Func<string[]> exceptionAuxInfoGetter = null) {
			return (Func<From, To>)GetCastFunc(typeof(From), typeof(To), exceptionAuxInfoGetter);
		}
		public static Delegate ConvertFuncArgument(this Delegate func, Type currentArgumentType, Type resultType, Type expectedArgumentType, Func<string[]> exceptionAuxInfoGetter = null) {
			if (currentArgumentType == expectedArgumentType)
				return func;
			var cast = GetCastFunc(expectedArgumentType, currentArgumentType, exceptionAuxInfoGetter);
			return cast.ApplyChain(func, expectedArgumentType, currentArgumentType, resultType);
		}
		public static Delegate ConvertFuncArgument<A, R>(this Func<A, R> func, Type expectedArgumentType, Func<string[]> exceptionAuxInfoGetter = null) {
			return ConvertFuncArgument(func, typeof(A), typeof(R), expectedArgumentType, exceptionAuxInfoGetter);
		}
		public static Func<A, R> ConvertFuncArgument<A, R>(this Delegate func, Type currentArgumentType, Func<string[]> exceptionAuxInfoGetter = null) {
			return (Func<A, R>)ConvertFuncArgument(func, currentArgumentType, typeof(R), typeof(A), exceptionAuxInfoGetter);
		}
		public static Delegate ConvertFuncResult(this Delegate func, Type argumentType, Type currentResultType, Type expectedResultType, Func<string[]> exceptionAuxInfoGetter = null) {
			if (currentResultType == expectedResultType)
				return func;
			var cast = GetCastFunc(currentResultType, expectedResultType, exceptionAuxInfoGetter);
			return func.ApplyChain(cast, argumentType, currentResultType, expectedResultType);
		}
		public static Delegate ConvertFuncResult<A, R>(this Func<A, R> func, Type expectedResultType, Func<string[]> exceptionAuxInfoGetter = null) {
			return ConvertFuncResult(func, typeof(A), typeof(R), expectedResultType, exceptionAuxInfoGetter);
		}
		public static Func<A, R> ConvertFuncResult<A, R>(this Delegate func, Type currentResultType, Func<string[]> exceptionAuxInfoGetter = null) {
			return (Func<A, R>)ConvertFuncResult(func, typeof(A), currentResultType, typeof(R), exceptionAuxInfoGetter);
		}
		public static Delegate ConvertFunc(this Delegate func, Type currentArgumentType, Type currentResultType, Type expectedArgumentType, Type expectedResultType, Func<string[]> exceptionAuxInfoGetter = null) {
			return func.ConvertFuncArgument(currentArgumentType, currentResultType, expectedArgumentType, exceptionAuxInfoGetter).ConvertFuncResult(expectedArgumentType, currentResultType, expectedResultType, exceptionAuxInfoGetter);
		}
		public static Func<A, R> ConvertFunc<A, R>(this Delegate func, Type currentArgumentType, Type currentResultType, Func<string[]> exceptionAuxInfoGetter = null) {
			return (Func<A, R>)func.ConvertFunc(currentArgumentType, currentResultType, typeof(A), typeof(R), exceptionAuxInfoGetter);
		}
		public static Delegate ConvertFunc<A, R>(this Func<A, R> func, Type expectedArgumentType, Type expectedResultType, Func<string[]> exceptionAuxInfoGetter = null) {
			return func.ConvertFunc(typeof(A), typeof(R), expectedArgumentType, expectedResultType, exceptionAuxInfoGetter);
		}
		public abstract class ChainApplier: GenericInvoker<Func<Delegate, Delegate, Delegate>, ChainApplier.Impl<object, object, object>> {
			public class Impl<A, I, R> : ChainApplier {
				static Func<A, R> ApplyChain(Func<A, I> nested, Func<I, R> wrapper) {
					return a => wrapper(nested(a));
				}
				protected override Func<Delegate, Delegate, Delegate> CreateInvoker() {
					return (nested, wrapper) => ApplyChain((Func<A, I>)nested, (Func<I, R>)wrapper);
				}
			}
		}
		public static Delegate ApplyChain(this Delegate nested, Delegate wrapper, Type nestedArgType, Type intermediateType, Type wrapperResultType) {
			return ChainApplier.GetInvoker(nestedArgType, intermediateType, wrapperResultType)(nested, wrapper);
		}
		public static Delegate ApplyChain<A, I>(this Func<A, I> nested, Delegate wrapper, Type wrapperResultType) {
			return nested.ApplyChain(wrapper, typeof(A), typeof(I), wrapperResultType);
		}
		public static Delegate ApplyChain<I, R>(this Delegate nested, Func<I, R> wrapper, Type nestedArgType) {
			return nested.ApplyChain(wrapper, nestedArgType, typeof(I), typeof(R));
		}
		public static Func<A, R> ApplyChain<A, R>(this Delegate nested, Delegate wrapper, Type intermediateType) {
			return (Func<A, R>)nested.ApplyChain(wrapper, typeof(A), intermediateType, typeof(R));
		}
		public abstract class NullArgHedger : GenericInvoker<Func<Delegate, Delegate>, NullArgHedger.Impl<object, object>> {
			public class Impl<A, R> : NullArgHedger {
				static Func<A, R> Hedge(Func<A, R> nakedFunc) {
					return arg => {
						if (arg == null)
							return default(R);
						else
							return nakedFunc(arg);
					};
				}
				protected override Func<Delegate, Delegate> CreateInvoker() {
					return nakedFunc => {
						if (!NullableHelpers.CanAcceptNull(typeof(A)))
							return nakedFunc;
						return Hedge((Func<A, R>)nakedFunc);
					};
				}
			}
		}
		public static Delegate HedgeNullArg(Delegate dlg, Type mayBeNullArgumentType, Type resultType) {
			return NullArgHedger.GetInvoker(mayBeNullArgumentType, resultType)(dlg);
		}
		public abstract class NullCheckCreatorNullableStruct : GenericInvoker<Func<bool, Delegate>, NullCheckCreatorNullableStruct.Impl<bool>> {
			public class Impl<T> : NullCheckCreatorNullableStruct where T : struct {
				static bool IsNull(T? arg) {
					return !arg.HasValue;
				}
				static bool IsNotNull(T? arg) {
					return arg.HasValue;
				}
				protected override Func<bool, Delegate> CreateInvoker() {
					return isNotNull => isNotNull ? (Func<T?, bool>)IsNotNull : (Func<T?, bool>)IsNull;
				}
			}
		}
		public abstract class NullCheckCreatorClass : GenericInvoker<Func<bool, Delegate>, NullCheckCreatorClass.Impl<object>> {
			public class Impl<T> : NullCheckCreatorClass where T : class {
				static bool IsNull(T arg) {
					return arg == null;
				}
				static bool IsNotNull(T arg) {
					return arg != null;
				}
				protected override Func<bool, Delegate> CreateInvoker() {
					return isNotNull => isNotNull ? (Func<T, bool>)IsNotNull : (Func<T, bool>)IsNull;
				}
			}
		}
		public static Delegate MakeNullCheck(Type argType, bool isNotCheck) {
			Type underlyingType = Nullable.GetUnderlyingType(argType);
			if (underlyingType != null)
				return NullCheckCreatorNullableStruct.GetInvoker(underlyingType)(isNotCheck);
			if (!NullableHelpers.CanAcceptNull(argType))
				throw new ArgumentException("argType can't be null");
			return NullCheckCreatorClass.GetInvoker(argType)(isNotCheck);
		}
		public static Delegate MakeNullCheck(Type argType) {
			return MakeNullCheck(argType, false);
		}
		public static Func<T, bool> MakeNullCheck<T>(bool isNotCheck) {
			return (Func<T, bool>)MakeNullCheck(typeof(T), isNotCheck);
		}
		public static Func<T, bool> MakeNullCheck<T>() {
			return MakeNullCheck<T>(false);
		}
	}
	public static class NullableHelpers {
		public static Type GetBoxedType(Type t) {
			return Nullable.GetUnderlyingType(t) ?? t;
		}
		public static Type GetUnBoxedType(Type t) {
			if (!CanAcceptNull(t))
				return typeof(Nullable<>).MakeGenericType(t);
			else
				return t;
		}
		public static bool CanAcceptNull(Type t) {
			if (t.IsValueType() && Nullable.GetUnderlyingType(t) == null)
				return false;
			else
				return true;
		}
	}
	public static class GenericTypeHelper {
		public static Type GetGenericIListTypeArgument(Type elementType) {
			return GetGenericTypeArgument(elementType, typeof(System.Collections.Generic.IList<>)) ??
				   GetGenericTypeArgument(elementType, typeof(System.Collections.Generic.IEnumerable<>));
		}
		public static Type GetGenericTypeArgument(Type elementType, Type genericType) {
			if (elementType == null)
				return null;
			if (elementType.IsInterface() && elementType.IsGenericType() && elementType.GetGenericTypeDefinition() == genericType) {
				return elementType.GetGenericArguments()[0];
			}
			foreach (Type iface in elementType.GetInterfaces()) {
				if (iface.IsGenericType() && iface.GetGenericTypeDefinition() == genericType) {
					return iface.GetGenericArguments()[0];
				}
			}
			return null;
		}
		public static Type GetGenericIListType(Type listType) {
			Type genericIListTypeArgument = GetGenericIListTypeArgument(listType);
			if (genericIListTypeArgument == typeof(object))
				genericIListTypeArgument = null;
			return genericIListTypeArgument;
		}
	}
}
