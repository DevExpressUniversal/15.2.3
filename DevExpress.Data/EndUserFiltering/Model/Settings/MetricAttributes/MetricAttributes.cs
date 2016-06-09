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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	abstract partial class MetricAttributes : IMetricAttributes, INotifyPropertyChanged {
		internal static Type GetMetricAttributesTypeDefinition(IMetricAttributesCache cache, string path, Type type, Type enumDataType) {
			return cache.GetValueOrCache(path, () =>
			{
				Type result = null;
				if(IsRange(type))
					result = typeof(IRangeMetricAttributes<>);
				if(IsLookup(type) || IsLookupMember(cache as IMetadataStorage, type, path))
					result = typeof(ILookupMetricAttributes<>);
				if(IsBooleanChoice(type))
					result = typeof(IChoiceMetricAttributes<>);
				if(IsEnumChoice(type) || IsEnumChoice(enumDataType))
					result = typeof(IEnumChoiceMetricAttributes<>);
				if(result != null)
					return result;
				throw new ResolveMetricTypeDefinitionException(path, type);
			});
		}
		static IDictionary<Type, Func<Type, Type, IMetricAttributes>> lazyInstantiatorsMapping = new Dictionary<Type, Func<Type, Type, IMetricAttributes>>() { 
				{ typeof(IRangeMetricAttributes<>), (type, enumDataType) => CreateLazyRange(type) },
				{ typeof(ILookupMetricAttributes<>), (type, enumDataType) => CreateLazyLookup(type) },
				{ typeof(IChoiceMetricAttributes<>), (type, enumDataType) => CreateLazyBooleanChoice(type) },
				{ typeof(IEnumChoiceMetricAttributes<>), (type, enumDataType) => CreateLazyEnumChoice(type, enumDataType) },
		};
		internal static IMetricAttributes CreateLazyMetricAttributes(IMetricAttributesCache cache, string path, Type type, Type enumDataType) {
			return cache.GetValueOrCache(path, () =>
			{
				Type typeDefinition = GetMetricAttributesTypeDefinition(cache, path, type, enumDataType);
				Func<Type, Type, IMetricAttributes> create;
				if(lazyInstantiatorsMapping.TryGetValue(typeDefinition, out create))
					return create(type, enumDataType);
				throw new CreateMetricException(path);
			});
		}
		static string[] EmptyRangeOrLookupMembers = new string[] { null, null, null };
		static IMetricAttributes CreateLazyRange(Type type) {
			object min = null, max = null;
			if(IsDateTimeRange(type)) {
				CheckDataTimeRangeCore(type, ref min, ref max);
				return CreateRange(type, min, max, null, null, DateTimeRangeUIEditorType.Default, EmptyRangeOrLookupMembers);
			}
			else {
				CheckNumericRange(type, ref min, ref max);
				return CreateRange(type, min, max, null, null, null, RangeUIEditorType.Text, EmptyRangeOrLookupMembers);
			}
		}
		static IMetricAttributes CreateLazyLookup(Type type) {
			return CreateLookup(type, null, null, null, null, null, LookupUIEditorType.Default, null, null, null, null, EmptyRangeOrLookupMembers);
		}
		static IMetricAttributes CreateLazyBooleanChoice(Type type) {
			return CreateBooleanChoice(type, null, null, null, null, BooleanUIEditorType.Default, new string[] { null });
		}
		static IMetricAttributes CreateLazyEnumChoice(Type type, Type enumDataType) {
			return CreateEnumChoice(type, enumDataType, LookupUIEditorType.Default, null, null, null, null, FilterAttribute.EmptyMembers);
		}
		#region Converter
		static class Converter {
			internal static T Convert<T>(object value) {
				if(object.ReferenceEquals(null, value))
					return default(T);
				if(typeof(T).IsAssignableFrom(value.GetType()))
					return (T)value;
				return (T)System.Convert.ChangeType(value, typeof(T));
			}
			internal static IEnumerable<T> ConvertToIEnumerable<T>(object value, string member) {
				if(string.IsNullOrEmpty(member))
					return ConvertToIEnumerable<T>(value);
				else {
					IEnumerable enumerable = value as IEnumerable;
					if(enumerable != null)
						return Iterate<T>(enumerable, member);
				}
				return Enumerable.Empty<T>();
			}
			internal static IEnumerable<T> ConvertToIEnumerable<T>(object value) {
				IEnumerable<T> genericEnumerable = value as IEnumerable<T>;
				if(genericEnumerable != null)
					return genericEnumerable;
				IEnumerable enumerable = value as IEnumerable;
				if(enumerable != null)
					return Iterate<T>(enumerable);
				return ConvertToArray<T>(value);
			}
			internal static IEnumerable<T> Iterate<T>(IEnumerable enumerable, string member) {
				foreach(object item in enumerable)
					yield return Convert<T>(item.@GetMemberValue(member));
			}
			internal static IEnumerable<T> Iterate<T>(IEnumerable enumerable) {
				foreach(var item in enumerable)
					yield return Convert<T>(item);
			}
			internal static T[] ConvertToArray<T>(object value) {
				T[] arr = value as T[];
				if(arr != null)
					return arr;
				object[] objArr = value as object[];
				if(objArr != null)
					return Array.ConvertAll(objArr, (e) => Convert<T>(e));
				IEnumerable<T> genericEnumerable = value as IEnumerable<T>;
				if(genericEnumerable != null)
					return genericEnumerable.ToArray();
				IEnumerable enumerable = value as IEnumerable;
				if(enumerable != null)
					return Iterate<T>(enumerable).ToArray();
				return new T[] { };
			}
			internal static T? ConvertNullable<T>(object value) where T : struct {
				if(object.ReferenceEquals(null, value))
					return null;
				return new Nullable<T>(Convert<T>(value));
			}
			static MethodInfo GetConvertMethod(Type type) {
				return typeof(Converter).GetMethod("Convert",
					BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(type);
			}
			static MethodInfo GetConvertNullableMethod(Type type) {
				return typeof(Converter).GetMethod("ConvertNullable",
					BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(type);
			}
			internal static MethodCallExpression GetConvertExpression(Type type, ParameterExpression p) {
				return Expression.Call(GetConvertMethod(type), p);
			}
			internal static MethodCallExpression GetConvertNullableExpression(Type type, ParameterExpression p) {
				return Expression.Call(GetConvertNullableMethod(type), p);
			}
			static MethodInfo selectInfo = typeof(Enumerable).GetMember("Select",
						BindingFlags.Static | BindingFlags.Public).First() as MethodInfo;
			internal static MethodCallExpression GetConvertToIEnumerableExpression(Type type, ParameterExpression pValues) {
				var convert = GetConvertExpression(type);
				var mInfoSelect = selectInfo.MakeGenericMethod(typeof(object), type);
				return Expression.Call(mInfoSelect, pValues, convert);
			}
			static MethodInfo convertAllInfo = typeof(Array).GetMember("ConvertAll",
						BindingFlags.Static | BindingFlags.Public).First() as MethodInfo;
			internal static MethodCallExpression GetConvertToArrayExpression(Type type, ParameterExpression pArray) {
				var mInfoConvertAll = convertAllInfo.MakeGenericMethod(typeof(object), type);
				var convertDelegateType = typeof(Converter<,>).MakeGenericType(typeof(object), type);
				var converterDelegate = Delegate.CreateDelegate(convertDelegateType, GetConvertMethod(type));
				return Expression.Call(mInfoConvertAll, pArray, Expression.Constant(converterDelegate));
			}
			static LambdaExpression GetConvertExpression(Type type) {
				var p = Expression.Parameter(typeof(object), "x");
				return Expression.Lambda(Expression.Call(GetConvertMethod(type), p), p);
			}
		}
		#endregion
		#region Exceptions
		class MetricException : Exception {
			public MetricException(string path, string format, params object[] parameters)
				: base("[" + path + "] Error: " + string.Format(format, parameters)) { }
		}
		class ResolveMetricTypeDefinitionException : MetricException {
			public ResolveMetricTypeDefinitionException(string path, Type type)
				: base(path, "Unable to resolve metric type definition for type {0}", type) { }
		}
		class CreateMetricException : MetricException {
			public CreateMetricException(string path)
				: base(path, "Unable to create Metric") { }
		}
		#endregion
		#region Member Bindings
		readonly string[] members;
		readonly string[] unboundMembers;
		readonly MemberValueBox[] valueBoxes;
		protected MetricAttributes(string[] members) {
			this.members = members;
			this.unboundMembers = new string[members.Length];
			this.valueBoxes = new MemberValueBox[members.Length];
		}
		void IMetricAttributes.UpdateMemberBindings(object viewModel, string propertyName, IMetricAttributesQuery queryProvider) {
			if(string.IsNullOrEmpty(propertyName)) {
				var valuesHash = new Dictionary<string, object>();
				var queryMemberHash = new Dictionary<string, object>();
				for(int i = 0; i < members.Length; i++) {
					if(string.IsNullOrEmpty(members[i]))
						continue;
					object value = MemberReader.Read(viewModel, members[i], valuesHash);
					valueBoxes[i].Update(value);
					queryMemberHash[valueBoxes[i].propertyName] = value;
				}
				queryProvider.@Do(p => p.QueryValues(queryMemberHash));
				for(int i = 0; i < unboundMembers.Length; i++) {
					if(string.IsNullOrEmpty(unboundMembers[i]))
						continue;
					object value;
					if(queryMemberHash.TryGetValue(unboundMembers[i], out value))
						valueBoxes[i].Update(value);
				}
			}
			else {
				object value = MemberReader.Read(viewModel, propertyName);
				for(int i = 0; i < members.Length; i++) {
					if(members[i] == propertyName)
						valueBoxes[i].Update(value);
				}
			}
		}
		#region MemberValueBox
		class MemberValueBox {
			readonly MetricAttributes owner;
			readonly LambdaExpression propertyExpression;
			internal readonly string propertyName;
			protected readonly bool hasMember;
			public MemberValueBox(int memberIndex, MetricAttributes owner, LambdaExpression propertyExpression) {
				this.owner = owner;
				this.propertyExpression = propertyExpression;
				this.propertyName = ExpressionHelper.GetPropertyName(propertyExpression);
				owner.valueBoxes[memberIndex] = this;
				this.hasMember = !string.IsNullOrEmpty(owner.members[memberIndex]);
				if(!hasMember)
					owner.unboundMembers[memberIndex] = propertyName;
			}
			internal void Update(object value) {
				if(Equals(this.value, value)) return;
				this.value = value;
				OnUpdateValue();
			}
			protected virtual void OnUpdateValue() {
				owner.@RaisePropertyChanged(propertyExpression);
			}
			protected void NotifyOwner(string property) {
				owner.@RaisePropertyChanged(property);
			}
			object value;
			protected bool HasMemberValue {
				get { return !Equals(value, null); }
			}
			protected object GetValue() {
				return value;
			}
			protected T ConvertValue<T>() {
				return Converter.Convert<T>(value);
			}
		}
		class MemberLookupValuesBox<T> : MemberValueBox {
			readonly object defaultValue;
			internal MemberLookupValuesBox(object defaultValue, string valueMember, string displayMember, int memberIndex, MetricAttributes owner, Expression<Func<object>> propertyExpression)
				: base(memberIndex, owner, propertyExpression) {
				this.defaultValue = defaultValue;
				this.ValueMember = valueMember;
				this.DisplayMember = displayMember;
			}
			internal string ValueMember {
				get;
				private set;
			}
			internal string DisplayMember {
				get;
				private set;
			}
			internal object Value {
				get { return HasMemberValue ? GetValue() : defaultValue; }
			}
			internal bool HasMemberOrValue {
				get { return hasMember || !Equals(defaultValue, null); }
			}
		}
		class MemberNullableValueBox<T> : MemberValueBox where T : struct {
			readonly T? defaultValue;
			internal MemberNullableValueBox(T? defaultValue, int memberIndex, MetricAttributes owner, Expression<Func<T?>> propertyExpression)
				: base(memberIndex, owner, propertyExpression) {
				this.defaultValue = defaultValue;
			}
			internal T? Value {
				get { return HasMemberValue ? new Nullable<T>(ConvertValue<T>()) : defaultValue; }
			}
			internal bool HasMemberOrValue {
				get { return hasMember || defaultValue.HasValue; }
			}
		}
		#endregion MemberValueBox
		#endregion
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			var handler = (PropertyChangedEventHandler)PropertyChanged;
			if(handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
