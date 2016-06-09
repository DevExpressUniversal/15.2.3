#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using System.Collections.Generic;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.Utils;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering.Exceptions;
using System.Reflection;
namespace DevExpress.DashboardCommon.Native {
	public class DataReaderExEnumerableWrapper : IDisposable, ITypedDataReader  {
		public static bool IsNullableType(Type type) {
			return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		public static Type CorrectType(Type type) {
			return IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;
		}
		List<string> dataMembers = new List<string>();
		List<Type> types = new List<Type>();		
		IEnumerator enumerator;
		Delegate[] accessors;
		List<Func<object, bool>> isNullPredicates = new List<Func<object, bool>>(); 
		CriteriaCompilerDescriptor criteriaCompilerDescriptor;
		protected object Current { get { return enumerator != null ? enumerator.Current : null; } }
		public DataReaderExEnumerableWrapper(IEnumerable<PropertyDescriptor> properties, string[] dataMembers, IEnumerable source) :
			this(new PropertyDescriptorCollection(properties.ToArray()), dataMembers, source) {
		}
		public DataReaderExEnumerableWrapper(PropertyDescriptorCollection propertyDescriptorCollection, string[] dataMembers, IEnumerable source) {			
			if(source != null) {
				enumerator = source.GetEnumerator();								
				criteriaCompilerDescriptor = new CriteriaCompiledContextDescriptorDescripted(propertyDescriptorCollection);
				accessors = new Delegate[dataMembers.Length];
				foreach(string dataMember in dataMembers.Distinct()) {
					this.dataMembers.Add(dataMember);
					Type type = null;
					try {
						type = DiscoverType(dataMember);
						this.types.Add(type);
						isNullPredicates.Add(CreateIsNullPredicate(dataMember));
					} catch(CriteriaCompilerException) {
						if(type == null)
							types.Add(typeof(object));
						isNullPredicates.Add(obj => true);
					}
				}			 
			}
		}
		Type DiscoverType(string dataMember) {
			CriteriaOperator criteria = new OperandProperty(dataMember);
			LambdaExpression lambda = CriteriaCompiler.ToLambda(criteria, criteriaCompilerDescriptor);
			return lambda.Body.Type;
		}
		Func<object, bool> CreateIsNullPredicate(string dataMember) {
			if(string.IsNullOrWhiteSpace(dataMember))
				return obj=>true;
			CriteriaOperator criteria = new UnaryOperator(UnaryOperatorType.IsNull,new OperandProperty(dataMember));
			LambdaExpression lambda = CriteriaCompiler.ToLambda(criteria, criteriaCompilerDescriptor);
			return ((Expression<Func<object, bool>>)lambda).Compile();
		}
		Delegate CreateAccessorDelegate<TOutput>(int i){
			Type type = types[i];
			CriteriaOperator criteria = new OperandProperty(dataMembers[i]);
			LambdaExpression getter = CriteriaCompiler.ToLambda(criteria, criteriaCompilerDescriptor);
			if(IsNullableType(types[i])) {
				ParameterExpression obj = Expression.Parameter(typeof(object));
				Type nullableType = typeof(Nullable<>).MakeGenericType(typeof(TOutput));
				var variable = Expression.Variable(nullableType);
				InvocationExpression getterInvoke = Expression.Invoke(getter, obj);
				BinaryExpression assignExpression = Expression.Assign(variable, Expression.Convert(getterInvoke, nullableType));
				IEnumerable<PropertyInfo> nullableProperties = nullableType.GetProperties().Cast<PropertyInfo>();
				PropertyInfo hasValueProperty = nullableProperties.Single(pi => pi.Name == "HasValue");
				PropertyInfo valueProperty = nullableProperties.Single(pi => pi.Name == "Value");
				var condition = Expression.Condition(Expression.MakeMemberAccess(variable, hasValueProperty),
										Expression.MakeMemberAccess(variable, valueProperty),
										Expression.Default(typeof(TOutput)));
				var block = Expression.Block(new [] { variable},
								 assignExpression,								 
								 condition);
				getter = Expression.Lambda(block, obj);
			}
			return ((Expression<Func<object, TOutput>>)getter).Compile();
		}
		Func<object, T> GetTypedDelegate<T>(int i) {
			if(accessors[i] == null) {
				accessors[i] = CreateAccessorDelegate<T>(i);
			}
			return (Func<object, T>)accessors[i];
		}		
		void CheckEnumerator() {
			if(enumerator == null)
				throw new InvalidOperationException();
		}
		void DisposeEnumerator() {
			IDisposable disposable = enumerator as IDisposable;
			if(disposable != null)
				disposable.Dispose();
			enumerator = null;
		}
		public void Dispose() {
			DisposeEnumerator();
		}
		public int FieldCount {
			get { return dataMembers.Count; }
		}
		public Type GetFieldType(int i) {
			CheckEnumerator();
			return CorrectType(types[i]);
		}
		public string GetFieldName(int i) {
			CheckEnumerator();
			return dataMembers[i];
		}
		public bool Read() {
			bool result = false;
			if(enumerator != null) {
				result = enumerator.MoveNext();
				if(!result) 
					DisposeEnumerator();								   
			}
			return result;
		}
		public bool IsNull(int i) {
			CheckEnumerator();
			bool isNull = isNullPredicates[i](enumerator.Current);
			if(!isNull && types[i] == typeof(object)) 
				isNull |= GetPropertyValue<object>(enumerator.Current, i) == DBNull.Value;
			return isNull;
		}
		public T GetValue<T>(int i) {
			CheckEnumerator();
			return GetPropertyValue<T>(enumerator.Current, i);
		}
		T GetPropertyValue<T>(object obj, int i) {
			Func<object, T> accessor = GetTypedDelegate<T>(i);
			return accessor(obj);
		}
	}
}
