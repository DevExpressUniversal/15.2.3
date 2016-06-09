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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public class SpecialDataValueAccessExeption : ArgumentException {
		public SpecialDataValueAccessExeption() : base("Can't evaluate expression over SpecialDataValue.Error") { }
	}
	static class ProjectExpressionConstructor {
		class DataVectorDescriptor : CriteriaCompilerDescriptor {
			Type[] argumentTypes;
			public DataVectorDescriptor(IEnumerable<Type> argumentTypes) {
				this.argumentTypes = argumentTypes.ToArray();
			}
			public override Type ObjectType { get { return typeof(ProjectDataVectorContainer); } }
			public override Expression MakePropertyAccess(Expression baseExpression, string propertyPath) {
				DXContract.Requires(propertyPath[0] == 'v', "Use 'v' letter for naming formal parameter for projection expression.");
				int valueIndex;
				if (!Int32.TryParse(propertyPath.Substring(1), out valueIndex))
					throw new ArgumentException();
				return MakeDataVectorAccessExpression(baseExpression, valueIndex);
			}
			Expression MakeDefaultValueExpression(int typeIndex) {
				return Expression.Default(argumentTypes[typeIndex]);
			}
			Expression MakeDataVectorAccessExpression(Expression vectorContainer, int valueIndex) {
				Type vectorType = argumentTypes[valueIndex];
				Type wrapperType = vectorType.IsValueType() ? typeof(Nullable<>).MakeGenericType(vectorType) : null;
				Expression vectorBase = Expression.ArrayAccess(Expression.Property(vectorContainer, "Vectors"), Expression.Constant(valueIndex));
				Expression vector = Expression.Convert(vectorBase, typeof(DataVector<>).MakeGenericType(vectorType));
				Expression index = Expression.Property(vectorContainer, "Index");
				Expression dataValue = Expression.ArrayIndex(Expression.Property(vector, "Data"), index);
				Expression specialValue = Expression.ArrayIndex(Expression.Property(vector, "SpecialData"), index);
				MethodInfo throwExceptionFunc = typeof(DataVectorDescriptor)
					.GetMethod("TrowSpecialDataValueAccessExeption", BindingFlags.Static | BindingFlags.NonPublic)
					.MakeGenericMethod(wrapperType ?? vectorType);
				Expression specialValuesAccess = Expression.Condition(Expression.Equal(specialValue, Expression.Constant(SpecialDataValue.Null, typeof(SpecialDataValue))),
					wrapperType == null ? (Expression)Expression.Constant(null, vectorType) : (Expression)Expression.New(wrapperType),
					Expression.Call(throwExceptionFunc));
				Expression resultSelector = Expression.Condition(Expression.Equal(specialValue, Expression.Constant(SpecialDataValue.None, typeof(SpecialDataValue))),
					wrapperType == null ? dataValue : Expression.New(wrapperType.GetConstructor(new[] { vectorType }), dataValue),
					specialValuesAccess);
				return resultSelector;
			}
			static T TrowSpecialDataValueAccessExeption<T>() {
				throw new SpecialDataValueAccessExeption();
			}
		}
		struct ProjectWorkerPrecompiledCacheKey : IEquatable<ProjectWorkerPrecompiledCacheKey> {
			Type[] dataTypes;
			string expression;
			public ProjectWorkerPrecompiledCacheKey(string criteria, IEnumerable<Type> typesList) {
				this.dataTypes = typesList.ToArray();
				this.expression = criteria;
			}
			public bool Equals(ProjectWorkerPrecompiledCacheKey other) {
				return other.expression == expression
					&& other.dataTypes.SequenceEqual(dataTypes);
			}
			public override int GetHashCode() {
				return HashcodeHelper.GetCompositeHashCode<int>(
					expression.GetHashCode(),
					HashcodeHelper.GetCompositeHashCode(dataTypes));
			}
		}
		static LRUCache<ProjectWorkerPrecompiledCacheKey, Delegate> precompiledCache = new LRUCache<ProjectWorkerPrecompiledCacheKey, Delegate>(DataProcessingOptions.ProjectWorkerCacheSize);
		static LRUCache<ProjectWorkerPrecompiledCacheKey, Type> precompiledTypeCache = new LRUCache<ProjectWorkerPrecompiledCacheKey, Type>(DataProcessingOptions.ProjectWorkerCacheSize);
		public static Delegate GetCompiledExpression(Project operation, LambdaExpression workerExpression) {
			Type[] dataTypes = operation.Operands.OfType<SingleFlowOperation>().Select(o => o.OperationType).Append(operation.OperationType).ToArray();
			string expression = operation.ExpressionString;
			return precompiledCache.GetOrAdd(new ProjectWorkerPrecompiledCacheKey(expression, dataTypes), () => workerExpression.Compile());
		}
		public static Type InferResultType(string criteria, IEnumerable<Type> typesList) {
			ProjectWorkerPrecompiledCacheKey key = new ProjectWorkerPrecompiledCacheKey(criteria, typesList);
			Type expressionType = precompiledTypeCache.GetOrAdd(key, () => GetWorkerExpression(CriteriaOperator.Parse(criteria), typesList).ReturnType);
			return Nullable.GetUnderlyingType(expressionType) ?? expressionType;
		}
		public static LambdaExpression GetWorkerExpression(CriteriaOperator criteria, IEnumerable<Type> typesList) {
			return CriteriaCompiler.ToLambda(criteria, new DataVectorDescriptor(typesList));
		}
	}
	public struct ProjectDataVectorContainer {
		int index;
		DataVectorBase[] vectors;
		public int Index { get { return index; } set { index = value; } }
		public DataVectorBase[] Vectors { get { return vectors; } }
		public ProjectDataVectorContainer(int index, DataVectorBase[] vectors) {
			this.index = index;
			this.vectors = vectors;
		}
	}
}
