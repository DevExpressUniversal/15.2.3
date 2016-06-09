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
using System.Globalization;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Data.Filtering.Exceptions;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Utils;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
	using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
	using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Data.Filtering.Helpers {
	public interface IEvaluatorDataAccess {
		object GetValue(PropertyDescriptor descriptor, object theObject);
	}
	public abstract class EvaluatorContextDescriptor {
		public abstract object GetPropertyValue(object source, EvaluatorProperty propertyPath);
		public abstract EvaluatorContext GetNestedContext(object source, string propertyPath);
		public abstract IEnumerable GetCollectionContexts(object source, string collectionName);
		public virtual IEnumerable GetQueryContexts(object source, string queryTypeName, CriteriaOperator condition, int top) {
			throw new NotSupportedException();
		}
		public virtual bool IsTopLevelCollectionSource {
			get { return false; }
		}
	}
	public class EvaluatorContextDescriptorDefault : EvaluatorContextDescriptor {
		readonly Type ReflectionType;
		readonly PropertyDescriptorCollection Properties;
		public IEvaluatorDataAccess DataAccess;
		public EvaluatorContextDescriptorDefault(PropertyDescriptorCollection properties, Type reflectionType) {
			this.Properties = properties;
			this.ReflectionType = reflectionType;
		}
		public EvaluatorContextDescriptorDefault(PropertyDescriptorCollection properties) : this(properties, null) { }
		public EvaluatorContextDescriptorDefault(Type reflectionType) {
			this.ReflectionType = reflectionType;
		}
		static object noResult = new object();
		object GetPropertyValue(object source, string property, bool isPath) {
			if(source == null)
				return null;
			if(Properties != null) {
				PropertyDescriptor pd = Properties.Find(property, false);
				if(pd == null) {
					pd = Properties.Find(property, true);
				}
				if(pd != null) {
					if(DataAccess == null)
						return pd.GetValue(source);
					else
						return DataAccess.GetValue(pd, source);
				}
			}
			if(!isPath) {
				if(ReflectionType != null) {
					if(source is System.Dynamic.ExpandoObject) {
						var expando = (IDictionary<string, object>)source;
						object rv;
						if(expando.TryGetValue(property, out rv))
							return rv;
					}
#if DXPORTABLE
					PropertyInfo pInfo = ReflectionType.GetProperty(property);
					if(pInfo != null)
						return pInfo.GetValue(source, null);
					FieldInfo fInfo = ReflectionType.GetField(property);
					if (fInfo != null)
						return fInfo.GetValue(source);
#else
					const BindingFlags ReflectionFlags = BindingFlags.Instance | BindingFlags.Public;
					PropertyInfo pInfo = ReflectionType.GetProperty(property, ReflectionFlags);
					if(pInfo != null)
						return pInfo.GetValue(source, null);
					FieldInfo fInfo = ReflectionType.GetField(property, ReflectionFlags);
					if(fInfo != null)
						return fInfo.GetValue(source);
#endif
				}
				if (EvaluatorProperty.GetIsThisProperty(property))
					return source;
				throw new InvalidPropertyPathException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorInvalidPropertyPath, property));
			}
			return noResult;
		}
		public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) {
			object res = GetPropertyValue(source, propertyPath.PropertyPath, propertyPath.PropertyPathTokenized.Length > 1);
			if(res != noResult)
				return res;
			EvaluatorContext nestedContext = this.GetNestedContext(source, propertyPath.PropertyPathTokenized[0]);
			if(nestedContext == null)
				return null;
			return nestedContext.GetPropertyValue(propertyPath.SubProperty);
		}
		public override EvaluatorContext GetNestedContext(object source, string propertyPath) {
			object nestedObject = GetPropertyValue(source, propertyPath, false);
			if(nestedObject == null)
				return null;
#if !SL
			IList list = nestedObject as IList;
			if(list == null) {
				IListSource listSource = nestedObject as IListSource;
				if(listSource != null) {
					if(listSource.ContainsListCollection)
						list = listSource.GetList();
				}
			}
			object nestedSource;
			PropertyDescriptorCollection pdc;
			if(list != null && list is ITypedList) {
				switch(list.Count) {
					case 0:
						return null;
					default:
						throw new ArgumentException("single row expected at '" + propertyPath + "', provided: " + list.Count.ToString());	
					case 1:
						break;
				}
				nestedSource = list[0];
				pdc = ((ITypedList)list).GetItemProperties(null);
			} else if(nestedObject is ITypedList) {
				nestedSource = nestedObject;
				pdc = ((ITypedList)nestedObject).GetItemProperties(null);
			} else {
				nestedSource = nestedObject;
				pdc = null;
			}
			EvaluatorContextDescriptor descriptor = new EvaluatorContextDescriptorDefault(pdc, nestedSource.GetType());
			return new EvaluatorContext(descriptor, nestedSource);
#else
			EvaluatorContextDescriptor descriptor = new EvaluatorContextDescriptorDefault(nestedObject.GetType());
			return new EvaluatorContext(descriptor, nestedObject);
#endif
		}
		public override IEnumerable GetCollectionContexts(object source, string collectionName) {
			object collectionSrc = GetPropertyValue(source, collectionName, false);
			if(collectionSrc == null)
				return null;
			IList list = collectionSrc as IList;
#if !SL
			if(list == null) {
				IListSource listSource = collectionSrc as IListSource;
				if(listSource != null)
					list = listSource.GetList();
			}
			if(list == null)
				throw new ArgumentException("not a collection: " + collectionName);
			EvaluatorContextDescriptor descriptor = null;
			ITypedList pdcSrc = list as ITypedList;
			if(pdcSrc != null) {
				descriptor = new EvaluatorContextDescriptorDefault(pdcSrc.GetItemProperties(null));
			}
			return new CollectionContexts(descriptor, list);
#else
			if (list == null)
				throw new ArgumentException("not a collection: " + collectionName);
			return new CollectionContexts(null, list);
#endif
		}
		public override IEnumerable GetQueryContexts(object source, string collectionTypeName, CriteriaOperator condition, int top) {
			throw new NotSupportedException();
		}
	}
	public class CollectionContexts : IEnumerable {
		public readonly EvaluatorContextDescriptor Descriptor;
		public readonly IEnumerable DataSource;
		public CollectionContexts(EvaluatorContextDescriptor descriptor, IEnumerable dataSource) {
			this.Descriptor = descriptor;
			this.DataSource = dataSource;
		}
		public virtual IEnumerator GetEnumerator() {
			return new CollectionContextsEnumerator(Descriptor, DataSource.GetEnumerator());
		}
	}
	public class CollectionContextsEnumerator : IEnumerator {
		public readonly EvaluatorContextDescriptor Descriptor;
		public readonly IEnumerator DataSource;
		public CollectionContextsEnumerator(EvaluatorContextDescriptor descriptor, IEnumerator dataSource) {
			this.Descriptor = descriptor;
			this.DataSource = dataSource;
		}
		public virtual object Current {
			get {
				EvaluatorContextDescriptor descr = Descriptor;
				if(descr == null) {
					Type objectType = typeof(object);
					if(DataSource.Current != null)
						objectType = DataSource.Current.GetType();
					descr = new EvaluatorContextDescriptorDefault(objectType);
				}
				return new EvaluatorContext(descr, DataSource.Current);
			}
		}
		public virtual bool MoveNext() {
			return DataSource.MoveNext();
		}
		public virtual void Reset() {
			DataSource.Reset();
		}
	}
	public class EvaluatorContext {
		public readonly EvaluatorContextDescriptor Descriptor;
		public readonly object Source;
		public EvaluatorContext(EvaluatorContextDescriptor descriptor, object source) {
			this.Descriptor = descriptor;
			this.Source = source;
		}
		public object GetPropertyValue(EvaluatorProperty propertyPath) {
			return Descriptor.GetPropertyValue(Source, propertyPath);
		}
		public EvaluatorContext GetNestedContext(string propertyPath) {
			return Descriptor.GetNestedContext(Source, propertyPath);
		}
		public IEnumerable GetCollectionContexts(string collectionName) {
			return Descriptor.GetCollectionContexts(Source, collectionName);
		}
		public IEnumerable GetQueryContexts(string queryTypeName, CriteriaOperator condition, int top) {
			return Descriptor.GetQueryContexts(Source, queryTypeName, condition, top);
		}
	}
	public delegate object EvaluateCustomFunctionHandler(string functionName, params object[] operands);
	class JoinEvaluationContextCache {
		readonly Stack<IEnumerable> collectionContextStack = new Stack<IEnumerable>();
		readonly Stack<Dictionary<JoinEvaluationContextCacheKey, JoinEvaluationCacheInfo>> cacheStack = new Stack<Dictionary<JoinEvaluationContextCacheKey, JoinEvaluationCacheInfo>>();
		Dictionary<JoinEvaluationContextCacheKey, JoinEvaluationCacheInfo> cacheDict = new Dictionary<JoinEvaluationContextCacheKey, JoinEvaluationCacheInfo>();
		public JoinEvaluationContextCache() {
		}
		public void PushCollectionContext(IEnumerable context) {
			collectionContextStack.Push(context);
			cacheStack.Push(cacheDict);
			cacheDict = new Dictionary<JoinEvaluationContextCacheKey, JoinEvaluationCacheInfo>();
		}
		public IEnumerable PopCollectionContext() {
			cacheDict.Clear();
			cacheDict = cacheStack.Pop();
			return collectionContextStack.Pop();
		}
		public IEnumerable GetQueryContexts(EvaluatorContext[] contexts, string queryTypeName, CriteriaOperator condition, int top, out bool filtered) {
			if(collectionContextStack.Count == 0 || top != 0) {
				filtered = true;
				return contexts[1].GetQueryContexts(queryTypeName, JoinContextCriteriaCreator.Process(contexts, condition), top);
			}
			JoinContextPropertyInfoSet zeroLevelProperties;
			CriteriaOperator cacheCondition = JoinContextCriteriaCreator.ProcessZeroLevelLeave(contexts, condition, out zeroLevelProperties);
			JoinEvaluationCacheInfo cacheInfo;
			var cacheKey = new JoinEvaluationContextCacheKey(queryTypeName, cacheCondition);
			if(!cacheDict.TryGetValue(cacheKey, out cacheInfo)) {
				IEnumerable fullCollectionContext = collectionContextStack.Peek();
				List<JoinEvaluationCacheChunk> chunks = new List<JoinEvaluationCacheChunk>();
				Dictionary<object, int> chunkObjects = new Dictionary<object, int>();
				Dictionary<CriteriaOperatorKey, bool> chunkGroup = new Dictionary<CriteriaOperatorKey, bool>();
				foreach(EvaluatorContext context in fullCollectionContext) {
					if(context.Source == null) continue;
					CriteriaOperator currentCondition = JoinContextCriteriaPatcher.Process(zeroLevelProperties.GetJoinContextValueInfoSet(context), cacheCondition);
					chunkObjects.Add(context.Source, chunks.Count);
					chunkGroup[new CriteriaOperatorKey(currentCondition)] = true;
					if(chunkGroup.Count > 25) {
						chunks.Add(new JoinEvaluationCacheChunk(new GroupOperator(GroupOperatorType.Or, chunkGroup.Keys.Select(key => key.Condition))));
						chunkGroup.Clear();
					}
				}
				if(!ReferenceEquals(chunkGroup, null) && chunkGroup.Count > 0) {
					switch(chunkGroup.Count) {
						case 0:
							chunks.Add(new JoinEvaluationCacheChunk(null));
							break;
						case 1:
							foreach(CriteriaOperatorKey criteriaKey in chunkGroup.Keys) {
								chunks.Add(new JoinEvaluationCacheChunk(criteriaKey.Condition));
								break;
							}
							break;
						default:
							chunks.Add(new JoinEvaluationCacheChunk(new GroupOperator(GroupOperatorType.Or, chunkGroup.Keys.Select(key => key.Condition))));
							break;
					}
					chunkGroup.Clear();
				}
				cacheInfo = new JoinEvaluationCacheInfo(chunks, chunkObjects);
				cacheDict.Add(cacheKey, cacheInfo);
			}
			filtered = false;
			int foundChunkIndex;
			if(!cacheInfo.AllChunksObjects.TryGetValue(contexts[1].Source, out foundChunkIndex)) {
				throw new InvalidOperationException();
			}
			JoinEvaluationCacheChunk foundChunk = cacheInfo.Chunks[foundChunkIndex];
			if(foundChunk.IsEmpty) foundChunk.Fill(contexts[1], queryTypeName);
			if(ReferenceEquals(foundChunk.Criteria, null)) {
				filtered = true;
				return contexts[1].GetQueryContexts(queryTypeName, JoinContextCriteriaCreator.Process(contexts, condition), top);
			}
			return foundChunk.Objects;
		}
		class JoinEvaluationCacheChunk {
			bool isEmpty = true;
			IEnumerable objects;
			CriteriaOperator criteria;
			public bool IsEmpty { get { return isEmpty; } }
			public IEnumerable Objects { get { return objects; } }
			public CriteriaOperator Criteria { get { return criteria; } }
			public JoinEvaluationCacheChunk(CriteriaOperator criteria) {
				this.criteria = criteria;
				if(ReferenceEquals(criteria, null)) isEmpty = false;
			}
			public void Fill(EvaluatorContext context, string queryTypeName) {
				objects = context.GetQueryContexts(queryTypeName, criteria, 0);
				isEmpty = false;
			}
		}
		class JoinEvaluationCacheInfo {
			readonly List<JoinEvaluationCacheChunk> chunks;
			readonly Dictionary<object, int> allChunksObjects;
			public List<JoinEvaluationCacheChunk> Chunks { get { return chunks; } }
			public Dictionary<object, int> AllChunksObjects { get { return allChunksObjects; } }
			public JoinEvaluationCacheInfo(List<JoinEvaluationCacheChunk> chunks, Dictionary<object, int> allChunksObjects) {
				this.chunks = chunks;
				this.allChunksObjects = allChunksObjects;
			}
		}
		class JoinEvaluationContextCacheKey {
			public readonly string QueryTypeName;
			public readonly CriteriaOperator Condition;
			public JoinEvaluationContextCacheKey(string queryTypeName, CriteriaOperator condition) {
				QueryTypeName = queryTypeName;
				Condition = condition;
			}
			public override int GetHashCode() {
				return (QueryTypeName == null ? 0x23416643 : QueryTypeName.GetHashCode()) ^ (ReferenceEquals(Condition, null) ? 0x73423562 : Condition.GetHashCode());
			}
			public override bool Equals(object obj) {
				JoinEvaluationContextCacheKey other = obj as JoinEvaluationContextCacheKey;
				if(other == null)
					return false;
				return QueryTypeName == other.QueryTypeName && Equals(Condition, other.Condition);
			}
		}
		class CriteriaOperatorKey {
			public readonly CriteriaOperator Condition;
			public CriteriaOperatorKey(CriteriaOperator condition) {
				Condition = condition;
			}
			public override int GetHashCode() {
				return (ReferenceEquals(Condition, null) ? 0x73423562 : Condition.GetHashCode());
			}
			public override bool Equals(object obj) {
				CriteriaOperatorKey other = obj as CriteriaOperatorKey;
				if(other == null)
					return false;
				return Equals(Condition, other.Condition);
			}
		}
	}
	public class ExpressionEvaluatorCore : ExpressionEvaluatorCoreBase, IClientCriteriaVisitor<object> {
		public ExpressionEvaluatorCore(bool caseSensitive) : base(caseSensitive) { contexts = null; }
		public ExpressionEvaluatorCore(bool caseSensitive, EvaluateCustomFunctionHandler evaluateCustomFunction) : base(caseSensitive, evaluateCustomFunction) { contexts = null; }
		EvaluatorContext[] contexts;
		readonly JoinEvaluationContextCache JoinCache = new JoinEvaluationContextCache();
		protected readonly EvaluatorPropertyCache PropertyCache = new EvaluatorPropertyCache();
		protected sealed override void SetContext(EvaluatorContext context) {
			if(contexts == null) contexts = new EvaluatorContext[] { context };
			else contexts[0] = context;
		}
		protected sealed override void ClearContext() {
			contexts = null;
		}
		protected sealed override bool HasContext {
			get { return contexts != null; }
		}
		protected sealed override EvaluatorContext GetContext() {
			return contexts[0];
		}
		protected sealed override EvaluatorContext GetContext(int upDepth) {
			return contexts[upDepth];
		}
		protected sealed override void PushCollectionContext(IEnumerable context) {
			JoinCache.PushCollectionContext(context);
		}
		protected sealed override IEnumerable PopCollectionContext() {
			return JoinCache.PopCollectionContext();
		}
		IEnumerable CreateNestedJoinContext(string joinTypeName, CriteriaOperator condition, int top, out bool filtered) {
			EvaluatorContext[] parentContext = contexts;
			contexts = new EvaluatorContext[parentContext.Length + 1];
			Array.Copy(parentContext, 0, contexts, 1, parentContext.Length);
			return JoinCache.GetQueryContexts(contexts, joinTypeName, condition, top, out filtered);
		}
		IEnumerable CreateNestedContext(EvaluatorProperty collectionProperty) {
			if(collectionProperty == null) {
				EvaluatorContext[] old = contexts;
				contexts = new EvaluatorContext[old.Length + 1];
				Array.Copy(old, 0, contexts, 1, old.Length);
				return contexts[1].GetCollectionContexts(null);
			}
			EvaluatorContext[] parentContext = contexts;
			contexts = new EvaluatorContext[parentContext.Length - collectionProperty.UpDepth + collectionProperty.PropertyPathTokenized.Length];
			Array.Copy(parentContext, collectionProperty.UpDepth, contexts, collectionProperty.PropertyPathTokenized.Length, contexts.Length - collectionProperty.PropertyPathTokenized.Length);
			int currentContextIndex = collectionProperty.PropertyPathTokenized.Length;
			int currentPathIndex = 0;
			while(currentContextIndex > 1) {
				EvaluatorContext currentContext = contexts[currentContextIndex];
				string currentProperty = collectionProperty.PropertyPathTokenized[currentPathIndex];
				currentContext = currentContext.GetNestedContext(currentProperty);
				if(currentContext == null)
					return null;
				--currentContextIndex;
				++currentPathIndex;
				contexts[currentContextIndex] = currentContext;
			}
			return contexts[1].GetCollectionContexts(collectionProperty.PropertyPathTokenized[collectionProperty.PropertyPathTokenized.Length - 1]);
		}
		object IClientCriteriaVisitor<object>.Visit(AggregateOperand theOperand) {
			if(theOperand.IsTopLevel && !this.contexts[0].Descriptor.IsTopLevelCollectionSource)
				throw new InvalidOperationException("can't evaluate top level aggregate on single object, collection property expected");	
			EvaluatorContext[] rememberedContexts = this.contexts;
			try {
				EvaluatorProperty property = theOperand.IsTopLevel ? null : PropertyCache[theOperand.CollectionProperty];
				IEnumerable nestedContextsCollection = CreateNestedContext(property);
				return DoAggregate(theOperand.AggregateType, nestedContextsCollection, theOperand.Condition, theOperand.AggregatedExpression);
			} finally {
				this.contexts = rememberedContexts;
			}
		}
		object IClientCriteriaVisitor<object>.Visit(JoinOperand theOperand) {
			EvaluatorContext[] rememberedContexts = this.contexts;
			try {
				bool filtered;
				IEnumerable nestedContextsCollection = CreateNestedJoinContext(theOperand.JoinTypeName, theOperand.Condition, theOperand.AggregateType == Aggregate.Single ? 1 : 0, out filtered);
				return DoAggregate(theOperand.AggregateType, nestedContextsCollection, filtered ? null : theOperand.Condition, theOperand.AggregatedExpression);
			} finally {
				this.contexts = rememberedContexts;
			}
		}
		object IClientCriteriaVisitor<object>.Visit(OperandProperty theOperand) {
			EvaluatorProperty property = PropertyCache[theOperand];
			object objectResult = contexts[property.UpDepth].GetPropertyValue(property);
			if(objectResult == null)
				return null;
#if !SL
			IList list = objectResult as IList;
			if(list == null) {
				IListSource listSource = objectResult as IListSource;
				if(listSource != null) {
					if(listSource.ContainsListCollection)
						list = listSource.GetList();
				}
			}
			if(list != null && list is ITypedList) {
				switch(list.Count) {
					case 0:
						return null;
					default:
						throw new ArgumentException("single row expected at '" + theOperand.PropertyName + "', provided: " + list.Count.ToString());	
					case 1:
						return list[0];
				}
			}
#endif
			return objectResult;
		}
	}
	public abstract class ExpressionEvaluatorCoreBase : ICriteriaVisitor<object> {
		EvaluateCustomFunctionHandler evaluateCustomFunction;
		protected ExpressionEvaluatorCoreBase(bool caseSensitive) : this(caseSensitive, null) { }
		protected ExpressionEvaluatorCoreBase(bool caseSensitive, EvaluateCustomFunctionHandler evaluateCustomFunction) {
			this.caseSensitive = caseSensitive;
			this.evaluateCustomFunction = evaluateCustomFunction;
		}
		IComparer customComparer;
		protected IComparer CustomComparer {
			get { return customComparer; }
		}
		bool caseSensitive;
		protected internal bool CaseSensitive {
			get { return caseSensitive; }
		}
		object FixValue(object value) {
			if(ReferenceEquals(value, DBNull.Value))
				return null;
			return value;
		}
		protected object Process(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null))
				return null;
			object value = operand.Accept(this);
			return FixValue(value);
		}
		object ICriteriaVisitor<object>.Visit(BetweenOperator theOperator) {
			object val = Process(theOperator.TestExpression);
			if(Compare(val, Process(theOperator.BeginExpression)) < 0)
				return false;
			if(Compare(val, Process(theOperator.EndExpression)) > 0)
				return false;
			return true;
		}
		int Compare(object left, object right, bool isEqualityCompare) {
			return EvalHelpers.CompareObjects(left, right, isEqualityCompare, caseSensitive, customComparer);
		}
		protected int Compare(object left, object right) {
			return Compare(left, right, false);
		}
		static object TrueValue = true;
		static object FalseValue = false;
		protected internal static object GetBool(bool? value) {
			return value == true ? TrueValue : FalseValue;
		}
		object ICriteriaVisitor<object>.Visit(BinaryOperator theOperator) {
#pragma warning disable 618
			if(theOperator.OperatorType == BinaryOperatorType.Like)
				return Process(LikeCustomFunction.Convert(theOperator));
#pragma warning restore 618
			object left = Process(theOperator.LeftOperand);
			object right = Process(theOperator.RightOperand);
			switch(theOperator.OperatorType) {
				case BinaryOperatorType.Equal:
					return GetBool(Compare(left, right, true) == 0);
				case BinaryOperatorType.NotEqual:
					return GetBool(Compare(left, right, true) != 0);
				case BinaryOperatorType.Less:
					return GetBool(Compare(left, right) < 0);
				case BinaryOperatorType.LessOrEqual:
					return GetBool(Compare(left, right) <= 0);
				case BinaryOperatorType.Greater:
					return GetBool(Compare(left, right) > 0);
				case BinaryOperatorType.GreaterOrEqual:
					return GetBool(Compare(left, right) >= 0);
				case BinaryOperatorType.Plus:
					return EvalHelpers.DoObjectsPlus(left, right);
				case BinaryOperatorType.Minus:
					return EvalHelpers.DoObjectsMinus(left, right);
				case BinaryOperatorType.Multiply:
					return EvalHelpers.DoObjectsMultiply(left, right);
				case BinaryOperatorType.Divide:
					return EvalHelpers.DoObjectsDivide(left, right);
				case BinaryOperatorType.Modulo:
					return EvalHelpers.DoObjectsModulo(left, right);
				case BinaryOperatorType.BitwiseAnd:
					return EvalHelpers.DoObjectsBitwiseAnd(left, right);
				case BinaryOperatorType.BitwiseOr:
					return EvalHelpers.DoObjectsBitwiseOr(left, right);
				case BinaryOperatorType.BitwiseXor:
					return EvalHelpers.DoObjectsBitwiseXor(left, right);
				default:
					throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotImplemented, typeof(BinaryOperator).Name, theOperator.OperatorType.ToString()));
			}
		}
		object ICriteriaVisitor<object>.Visit(InOperator theOperator) {
			object val = Process(theOperator.LeftOperand);
			foreach(CriteriaOperator op in theOperator.Operands)
				if(Compare(val, Process(op), true) == 0)
					return GetBool(true);
			return GetBool(false);
		}
		object ICriteriaVisitor<object>.Visit(GroupOperator theOperator) {
			int count = theOperator.Operands.Count;
			if(count == 0)
				return null;
			bool shortCircuitValue = theOperator.OperatorType == GroupOperatorType.Or;
			bool nullsDetected = false;
			for(int i = 0; i < count; i++) {
				bool? processed = (bool?)Process((CriteriaOperator)theOperator.Operands[i]);
				if(!processed.HasValue)
					nullsDetected = true;
				else if(processed.Value == shortCircuitValue)
					return GetBool(shortCircuitValue);
			}
			if(nullsDetected)
				return null;
			return GetBool(!shortCircuitValue);
		}
		object ICriteriaVisitor<object>.Visit(FunctionOperator theOperator) {
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.Iif:
					return FnIif(theOperator);
				case FunctionOperatorType.IsNull:
					return FnIsNull(theOperator);
				case FunctionOperatorType.Custom:
				case FunctionOperatorType.CustomNonDeterministic:
					return FnCustom(theOperator);
				case FunctionOperatorType.Concat: {
						EvalHelpers.FnConcater concater = new EvalHelpers.FnConcater();
						foreach(CriteriaOperator op in theOperator.Operands) {
							object processed = Process(op);
							if(processed == null)
								return null;
							if(!concater.Append(processed.ToString()))
								return null;
						}
						return concater.ToString();
					}
				default:
					return EvaluateLambdableFunction(theOperator.OperatorType, caseSensitive, theOperator.Operands.Select(o => Process(o)).ToArray());
			}
		}
		object FnIif(FunctionOperator theOperator) {
			if(theOperator.Operands.Count < 3 || ((theOperator.Operands.Count % 2) == 0)) throw new ArgumentException(); 
			int index = -2;
			do{
				index += 2;
				bool? iifDescriminator = (bool?)Process((CriteriaOperator)theOperator.Operands[index]);
				if(iifDescriminator ?? false) {
					return Process((CriteriaOperator)theOperator.Operands[index + 1]);
				}
			}while(theOperator.Operands.Count > index + 3);
			return Process((CriteriaOperator)theOperator.Operands[index + 2]);
		}
		object FnCustom(FunctionOperator theOperator) {
			if (evaluateCustomFunction == null) throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotImplemented, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()));
			object functionNameObject = Process(theOperator.Operands[0]);
			if(functionNameObject == null || !(functionNameObject is string)) {
				throw new ArgumentException("Custom function name not found.");
			}
			string functionName = (string)(functionNameObject);
			if (theOperator.Operands.Count > 1) {
				object[] operands = new object[theOperator.Operands.Count - 1];
				for (int i = 1; i < theOperator.Operands.Count; i++) {
					operands[i - 1] = Process((CriteriaOperator)theOperator.Operands[i]);
				}
				return evaluateCustomFunction(functionName, operands);
			}
			return evaluateCustomFunction(functionName);
		}
		object FnIsNull(FunctionOperator theOperator) {
			if(theOperator.Operands.Count == 1) {
				return GetBool(Process((CriteriaOperator)theOperator.Operands[0]) == null);
			}
			else {
				foreach(CriteriaOperator op in theOperator.Operands) {
					object obj = Process(op);
					if(obj != null) { return obj; }
				}
				return null;
			}
		}
		struct FnFuncKey: IEquatable<FnFuncKey> {
			public readonly bool CaseSensitive;
			public readonly FunctionOperatorType FnType;
			public readonly int ArgsCount;
			public FnFuncKey(FunctionOperatorType fnType, bool caseSensitive, int argsCount) {
				this.FnType = fnType;
				this.CaseSensitive = caseSensitive;
				this.ArgsCount = argsCount;
			}
			public override int GetHashCode() {
				unchecked{
					return ((CaseSensitive ? 42 : 31) + FnType.GetHashCode()) * 31 + ArgsCount;
				}
			}
			public override bool Equals(object obj) {
				if(obj is FnFuncKey)
					return Equals((FnFuncKey)obj);
				else
					return false;
			}
			public bool Equals(FnFuncKey other) {
				return FnType == other.FnType
					&& ArgsCount == other.ArgsCount
					&& CaseSensitive == other.CaseSensitive;
			}
		}
		static Dictionary<FnFuncKey, Func<object[], object>> FnFuncs = new Dictionary<FnFuncKey, Func<object[], object>>();
		static object EvaluateLambdableFunction(FunctionOperatorType fnType, bool caseSensitive, params object[] args) {
			FnFuncKey funcKey = new FnFuncKey(fnType, caseSensitive, args.Length);
			Func<object[], object> fn;
			lock(FnFuncs) {
				if(!FnFuncs.TryGetValue(funcKey, out fn)) {
					LambdaExpression coreLambda = EvalHelpers.MakeFnLambda(fnType, Enumerable.Repeat(typeof(object), args.Length).ToArray(), caseSensitive);
					ParameterExpression argsParam = Expression.Parameter(typeof(object[]), "args");
					var parameters = Enumerable.Range(0, args.Length).Select(i => Expression.ArrayIndex(argsParam, Expression.Constant(i))).Cast<Expression>().ToArray();
					Expression body = Expression.Invoke(coreLambda, parameters);
					if(body.Type == typeof(bool)) {
						body = Expression.Convert(body, typeof(bool?));
					}
					if(body.Type == typeof(bool?)) {
						Func<bool?, object> getBool = GetBool;
						body = Expression.Invoke(Expression.Constant(getBool), body);
					}
					if(body.Type != typeof(object))
						body = Expression.Convert(body, typeof(object));
					fn = Expression.Lambda<Func<object[], object>>(body, argsParam).Compile();
					FnFuncs.Add(funcKey, fn);
				}
			}
			return fn(args);
		}
		object ICriteriaVisitor<object>.Visit(OperandValue theOperand) {
			object operandValue = theOperand.Value;
			return operandValue;
		}
		object UnaryNumericPromotions(object operand) {
			switch(DXTypeExtensions.GetTypeCode(operand.GetType())) {
				case TypeCode.SByte:
					return (int)(SByte)operand;
				case TypeCode.Byte:
					return (int)(Byte)operand;
				case TypeCode.Int16:
					return (int)(Int16)operand;
				case TypeCode.UInt16:
					return (int)(UInt16)operand;
				case TypeCode.Char:
					return (int)(Char)operand;
				default:
					return operand;
			}
		}
		object ICriteriaVisitor<object>.Visit(UnaryOperator theOperator) {
			object operand = Process(theOperator.Operand);
			object converted;
			switch(theOperator.OperatorType) {
				case UnaryOperatorType.IsNull:
					return operand == null;
				case UnaryOperatorType.Not:
					bool? boolOperand = (bool?)operand;
					if(!boolOperand.HasValue)
						return null;
					return GetBool(!boolOperand.Value);
				case UnaryOperatorType.Plus:
					if(operand == null)
						return null;
					converted = UnaryNumericPromotions(operand);
					switch(DXTypeExtensions.GetTypeCode(converted.GetType())) {
						case TypeCode.Decimal:
						case TypeCode.Double:
						case TypeCode.Int32:
						case TypeCode.Int64:
						case TypeCode.Single:
						case TypeCode.UInt32:
						case TypeCode.UInt64:
							return converted;
						default:
							throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(UnaryOperator).Name, UnaryOperatorType.Plus.ToString(), operand.GetType().FullName));
					}
				case UnaryOperatorType.Minus:
					if(operand == null)
						return null;
					converted = (operand is UInt32) ? (Int64)(UInt32)operand : UnaryNumericPromotions(operand);
					switch(DXTypeExtensions.GetTypeCode(converted.GetType())) {
						case TypeCode.Int32:
							return -(Int32)converted;
						case TypeCode.Int64:
							return -(Int64)converted;
						case TypeCode.Single:
							return -(Single)converted;
						case TypeCode.Double:
							return -(Double)converted;
						case TypeCode.Decimal:
							return -(Decimal)converted;
						default:
							throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(UnaryOperator).Name, UnaryOperatorType.Minus.ToString(), operand.GetType().FullName));
					}
				case UnaryOperatorType.BitwiseNot:
					if(operand == null)
						return null;
					converted = UnaryNumericPromotions(operand);
					switch(DXTypeExtensions.GetTypeCode(converted.GetType())) {
						case TypeCode.Int32:
							return ~(Int32)converted;
						case TypeCode.Int64:
							return ~(Int64)converted;
						case TypeCode.UInt32:
							return ~(UInt32)converted;
						case TypeCode.UInt64:
							return ~(UInt64)converted;
						default:
							throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(UnaryOperator).Name, UnaryOperatorType.BitwiseNot.ToString(), operand.GetType().FullName));
					}
				default:
					throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotImplemented, typeof(UnaryOperator).Name, theOperator.OperatorType.ToString()));
			}
		}
		abstract class AggregateProcessingParam {
			public readonly ExpressionEvaluatorCoreBase Evaluator;
			protected AggregateProcessingParam(ExpressionEvaluatorCoreBase evaluator) {
				this.Evaluator = evaluator;
			}
			public abstract object GetResult();
			public abstract bool Process(object operand);	
		}
		class ExistsProcessingParam : AggregateProcessingParam {
			bool result = false;
			public override object GetResult() {
				return result;
			}
			public override bool Process(object operand) {
				result = true;
				return true;
			}
			public ExistsProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		class SingleProcessingParam : AggregateProcessingParam {
			bool processed;
			object result;
			public override object GetResult() {
				return result;
			}
			public override bool Process(object operand) {
				if(processed)
					throw new InvalidOperationException("The collection to which the Single aggregate is applied must be empty or contain exactly one item");
				result = operand;
				processed = true;
				return false;
			}
			public SingleProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		class CountProcessingParam : AggregateProcessingParam {
			int result = 0;
			public override object GetResult() {
				return result;
			}
			public override bool Process(object operand) {
				++result;
				return false;
			}
			public CountProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		class MinProcessingParam : AggregateProcessingParam {
			object result = null;
			public override object GetResult() {
				return result;
			}
			public override bool Process(object operand) {
				if(operand != null) {
					if(result == null || Evaluator.Compare(operand, result) < 0) {
						result = operand;
					}
				}
				return false;
			}
			public MinProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		class MaxProcessingParam : AggregateProcessingParam {
			object result = null;
			public override object GetResult() {
				return result;
			}
			public override bool Process(object operand) {
				if(operand != null) {
					if(result == null || Evaluator.Compare(operand, result) > 0) {
						result = operand;
					}
				}
				return false;
			}
			public MaxProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		class SumProcessingParam : AggregateProcessingParam {
			object result = null;
			public override object GetResult() {
				return result;
			}
			public override bool Process(object operand) {
				if(operand != null) {
					if(result == null) {
						result = operand;
					} else {
						result = Evaluator.Process(new BinaryOperator(new OperandValue(result), new OperandValue(operand), BinaryOperatorType.Plus));
					}
				}
				return false;
			}
			public SumProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		class AvgProcessingParam : AggregateProcessingParam {
			object result = null;
			int count = 0;
			public override object GetResult() {
				if(count == 0)
					return null;
				else {
					if(result != null && !(result is Single) && !(result is Decimal)) {
						result = Convert.ToDouble(result);
					}
					return Evaluator.Process(new BinaryOperator(new OperandValue(result), new OperandValue(count), BinaryOperatorType.Divide));
				}
			}
			public override bool Process(object operand) {
				if(operand != null) {
					if(result == null) {
						result = operand;
					} else {
						result = Evaluator.Process(new BinaryOperator(new OperandValue(result), new OperandValue(operand), BinaryOperatorType.Plus));
					}
					++count;
				}
				return false;
			}
			public AvgProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		void DoAggregate(AggregateProcessingParam param, IEnumerable contextsCollection, CriteriaOperator filterExpression, CriteriaOperator expression) {
			if(contextsCollection != null) {
				PushCollectionContext(contextsCollection);
				try {
					foreach(EvaluatorContext subContext in contextsCollection) {
						SetContext(subContext);
						if(this.Fit(filterExpression)) {
							object candidate = null;
							if(!ReferenceEquals(expression, null)) {
								candidate = this.Process(expression);
								if(candidate == null)
									continue;
							}
							if(param.Process(candidate))
								return;
						}
					}
				} finally {
					PopCollectionContext();
				}
			}
		}
		protected object DoAggregate(Aggregate aggregateType, IEnumerable contextsCollection, CriteriaOperator filterExpression, CriteriaOperator expression) {
			AggregateProcessingParam param;
			switch(aggregateType) {
				case Aggregate.Exists:
					param = new ExistsProcessingParam(this);
					break;
				case Aggregate.Count:
					param = new CountProcessingParam(this);
					break;
				case Aggregate.Avg:
					param = new AvgProcessingParam(this);
					break;
				case Aggregate.Max:
					param = new MaxProcessingParam(this);
					break;
				case Aggregate.Min:
					param = new MinProcessingParam(this);
					break;
				case Aggregate.Sum:
					param = new SumProcessingParam(this);
					break;
				case Aggregate.Single:
					param = new SingleProcessingParam(this);
					break;
				default:
					throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotImplemented, typeof(AggregateOperand).Name, aggregateType.ToString()));
			}
			DoAggregate(param, contextsCollection, filterExpression, expression);
			return param.GetResult();
		}
		public object Evaluate(EvaluatorContext evaluationContext, CriteriaOperator evaluatorCriteria) {
			return Evaluate(evaluationContext, evaluatorCriteria, null);
		}
		public object Evaluate(EvaluatorContext evaluationContext, CriteriaOperator evaluatorCriteria, IComparer customComparer) {
			System.Diagnostics.Debug.Assert(!HasContext);
			try {
				this.customComparer = customComparer;
				SetContext(evaluationContext);
				return this.Process(evaluatorCriteria);
			} finally {
				ClearContext();
			}
		}
		public object[] EvaluateOnObjects(IEnumerable evaluatorContextCollection, CriteriaOperator filterCriteria) {
			return EvaluateOnObjects(evaluatorContextCollection, filterCriteria, null);
		}
		public object[] EvaluateOnObjects(IEnumerable evaluatorContextCollection, CriteriaOperator filterCriteria, IComparer customComparer) {
			PushCollectionContext(evaluatorContextCollection);
			try {
				List<object> result = evaluatorContextCollection is ICollection ? new List<object>(((ICollection)evaluatorContextCollection).Count) : new List<object>();
				foreach(EvaluatorContext evaluatorContext in evaluatorContextCollection) {
					result.Add(Evaluate(evaluatorContext, filterCriteria, customComparer));
				}
				return result.ToArray();
			} finally {
				PopCollectionContext();
			}
		}
		protected abstract bool HasContext { get; }
		protected abstract void SetContext(EvaluatorContext context);
		protected abstract void ClearContext();
		protected abstract EvaluatorContext GetContext();
		protected abstract EvaluatorContext GetContext(int upDepth);
		protected virtual void PushCollectionContext(IEnumerable context) { }
		protected virtual IEnumerable PopCollectionContext() { return null; }
		public ICollection<EvaluatorContext> Filter(ICollection<EvaluatorContext> evaluatorContextCollection, CriteriaOperator filterCriteria) {
			PushCollectionContext(evaluatorContextCollection);
			try {
				List<EvaluatorContext> result = new List<EvaluatorContext>();
				foreach(EvaluatorContext evaluatorContext in evaluatorContextCollection) {
					if(Fit(evaluatorContext, filterCriteria)) {
						result.Add(evaluatorContext);
					}
				}
				return result;
			} finally {
				PopCollectionContext();
			}
		}
		public bool Fit(EvaluatorContext evaluationContext, CriteriaOperator filterCriteria) {
			if(ReferenceEquals(filterCriteria, null))
				return true;
			return (bool?)Evaluate(evaluationContext, filterCriteria) ?? false;
		}
		protected bool Fit(CriteriaOperator filterCriteria) {
			if(ReferenceEquals(filterCriteria, null))
				return true;
			return (bool?)Process(filterCriteria) ?? false;
		}
	}
	public class EvaluatorCriteriaValidator : IClientCriteriaVisitor {
		readonly PropertyDescriptorCollection Properties;
		public EvaluatorCriteriaValidator(PropertyDescriptorCollection properties) {
			this.Properties = properties;
		}
		public virtual void Visit(BetweenOperator theOperator) {
			Validate(theOperator.TestExpression);
			Validate(theOperator.EndExpression);
			Validate(theOperator.BeginExpression);
		}
		public virtual void Visit(BinaryOperator theOperator) {
			Validate(theOperator.LeftOperand);
			Validate(theOperator.RightOperand);
		}
		public virtual void Visit(UnaryOperator theOperator) {
			Validate(theOperator.Operand);
		}
		public virtual void Visit(InOperator theOperator) {
			Validate(theOperator.LeftOperand);
			Validate(theOperator.Operands);
		}
		public virtual void Visit(GroupOperator theOperator) {
			Validate(theOperator.Operands);
		}
		public virtual void Visit(OperandValue theOperand) {
		}
		public virtual void Visit(FunctionOperator theOperator) {
			Validate(theOperator.Operands);
		}
		public virtual void Visit(OperandProperty theOperand) {
			if(theOperand.PropertyName.IndexOf('.') < 0
				&& Properties.Find(theOperand.PropertyName, false) == null
				&& !EvaluatorProperty.GetIsThisProperty(theOperand.PropertyName)) {
				throw new InvalidPropertyPathException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorInvalidPropertyPath, theOperand.PropertyName));
			}
		}
		public virtual void Visit(AggregateOperand theOperand) {
			if(theOperand.IsTopLevel)
				throw new InvalidOperationException("can't evaluate top level aggregate on single object, collection property expected");	
			Validate(theOperand.CollectionProperty);
		}
		public virtual void Visit(JoinOperand theOperand) {
		}
		public void Validate(CriteriaOperator criteria) {
			if(!ReferenceEquals(criteria, null))
				criteria.Accept(this);
		}
		public void Validate(IList operands) {
			foreach(CriteriaOperator operand in operands)
				Validate(operand);
		}
	}
	public class ExpressionEvaluator {
		protected virtual ExpressionEvaluatorCoreBase EvaluatorCore { get { return evaluatorCore; } }
		protected readonly EvaluatorContextDescriptor DefaultDescriptor;
		protected readonly CriteriaOperator evaluatorCriteria;
		readonly ExpressionEvaluatorCoreBase evaluatorCore;
		protected ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore) {
			if(doCreateEvaluatorCore) this.evaluatorCore = new ExpressionEvaluatorCore(caseSensitive, new EvaluateCustomFunctionHandler(EvaluateCustomFunction));
			this.DefaultDescriptor = descriptor;
			this.evaluatorCriteria = criteria;
		}
		public ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive) : this(descriptor, criteria, caseSensitive, true) { }
		public ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria) : this(descriptor, criteria, true) { }
		public ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore, ICollection<ICustomFunctionOperator> customFunctions)
			: this(descriptor, criteria, caseSensitive, doCreateEvaluatorCore) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) 
			: this(descriptor, criteria, caseSensitive) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, ICollection<ICustomFunctionOperator> customFunctions)
			: this(descriptor, criteria, true) {
			RegisterCustomFunctions(customFunctions);
		}
		public IEvaluatorDataAccess DataAccess { set { ((EvaluatorContextDescriptorDefault)DefaultDescriptor).DataAccess = value; } }
		protected ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvauluatorCore)
			: this(new EvaluatorContextDescriptorDefault(properties), criteria, caseSensitive, doCreateEvauluatorCore) {
			new EvaluatorCriteriaValidator(properties).Validate(this.evaluatorCriteria);
		}
		public ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive) : this(properties, criteria, caseSensitive, true) { }
		public ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria) : this(properties, criteria, true) { }
		public ExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, bool caseSensitive) : this(properties, CriteriaOperator.Parse(criteria), caseSensitive) { }
		public ExpressionEvaluator(PropertyDescriptorCollection properties, string criteria) : this(properties, criteria, true) { }
		public ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, criteria, caseSensitive, doCreateEvaluatorCore) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, criteria, caseSensitive) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, criteria) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, criteria, caseSensitive) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, criteria) {
			RegisterCustomFunctions(customFunctions);
		}
		protected virtual EvaluatorContext PrepareContext(object valuesSource) {
			return new EvaluatorContext(DefaultDescriptor, valuesSource);
		}
		public object Evaluate(object theObject) {
			return Evaluate(theObject, null);
		}
		public object Evaluate(object theObject, IComparer customComparer) {
			return EvaluatorCore.Evaluate(PrepareContext(theObject), evaluatorCriteria, customComparer);
		}
		public object[] EvaluateOnObjects(IEnumerable objects) {
			return EvaluateOnObjects(objects, null);
		}
		public object[] EvaluateOnObjects(IEnumerable objects, IComparer customComparer) {
			List<EvaluatorContext> contextList = objects is ICollection ? new List<EvaluatorContext>(((ICollection)objects).Count) : new List<EvaluatorContext>();
			foreach(object theObject in objects) {
				contextList.Add(PrepareContext(theObject));
			}
			return EvaluatorCore.EvaluateOnObjects(contextList, evaluatorCriteria, customComparer);
		}
		public bool Fit(object theObject) {
			return EvaluatorCore.Fit(PrepareContext(theObject), evaluatorCriteria);
		}
		public ICollection Filter(IEnumerable objects) {
			List<EvaluatorContext> contextList = objects is ICollection ? new List<EvaluatorContext>(((ICollection)objects).Count) : new List<EvaluatorContext>();
			foreach(object theObject in objects) {
				contextList.Add(PrepareContext(theObject));
			}
			ICollection<EvaluatorContext> contextResultList = EvaluatorCore.Filter(contextList, evaluatorCriteria);
			List<object> resultList = new List<object>(contextResultList.Count);
			foreach(EvaluatorContext context in contextResultList) {
				resultList.Add(context.Source);
			}
			return resultList.ToArray();
		}
		bool throwExceptionIfNotFoundCustomFunction = true;
		public bool ThrowExceptionIfNotFoundCustomFunction {
			get { return throwExceptionIfNotFoundCustomFunction; }
			set { throwExceptionIfNotFoundCustomFunction = value; }
		}
		CustomFunctionCollection customFunctionCollection;
		void RegisterCustomFunctions(ICollection<ICustomFunctionOperator> customFunctions) {
			if(customFunctionCollection == null) {
				if(customFunctions is CustomFunctionCollection) {
					customFunctionCollection = (CustomFunctionCollection)customFunctions;
					return;
				}
				customFunctionCollection = new CustomFunctionCollection();
			}
			if(customFunctions == null) return;
			foreach(ICustomFunctionOperator customFunction in customFunctions) {
				customFunctionCollection.Add(customFunction);
			}
		}
		protected virtual object EvaluateCustomFunction(string functionName, params object[] operands) {
			ICustomFunctionOperator customFunction = null;
			if(customFunctionCollection != null) {
				customFunction = customFunctionCollection.GetCustomFunction(functionName);
			}			 
			if(customFunction == null) {
				if(CriteriaOperator.CustomFunctionCount > 0) {
					customFunction = CriteriaOperator.GetCustomFunction(functionName);
				}
				if(customFunction == null) {
					if(ThrowExceptionIfNotFoundCustomFunction) throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Custom function '{0}' not found.", functionName)); 
					else return null;
				}
			}
			if(EvaluatorCore != null) {
				var evaluatableWithCaseSensetivity = customFunction as ICustomFunctionOperatorEvaluatableWithCaseSensitivity;
				if(evaluatableWithCaseSensetivity != null)
					return evaluatableWithCaseSensetivity.Evaluate(EvaluatorCore.CaseSensitive, operands);
			}
			return customFunction.Evaluate(operands);
		}
	}
	public enum BooleanCriteriaState {
		Logical,
		Value,
		Undefined
	}
	public partial class BooleanComplianceChecker: IClientCriteriaVisitor<BooleanCriteriaState> {
		const string MustBeArithmetical = "Must be arithmetical bool";
		const string MustBeLogical = "Must be logical bool";
		static BooleanComplianceChecker instance = new BooleanComplianceChecker();
		public static BooleanComplianceChecker Instance { get { return instance; } }
		public BooleanCriteriaState Visit(AggregateOperand theOperand) {
			if(theOperand.AggregateType == Aggregate.Exists){
				return BooleanCriteriaState.Logical;
			}
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(JoinOperand theOperand) {
			if(theOperand.AggregateType == Aggregate.Exists) {
				return BooleanCriteriaState.Logical;
			}
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(OperandProperty theOperand) {
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(BetweenOperator theOperator) {
			BooleanCriteriaState testExRes = Process(theOperator.TestExpression);
			BooleanCriteriaState beginExRes = Process(theOperator.BeginExpression);
			BooleanCriteriaState endExRes = Process(theOperator.EndExpression);
			if (testExRes == BooleanCriteriaState.Logical 
				|| beginExRes == BooleanCriteriaState.Logical 
				|| endExRes == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			return BooleanCriteriaState.Logical;
		}
		public BooleanCriteriaState Visit(BinaryOperator theOperator) {
			BooleanCriteriaState leftRes = Process(theOperator.LeftOperand);
			BooleanCriteriaState rightRes = Process(theOperator.RightOperand);
			if (leftRes == BooleanCriteriaState.Logical || rightRes == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			switch (theOperator.OperatorType) {
				case BinaryOperatorType.Equal:
				case BinaryOperatorType.Greater:
				case BinaryOperatorType.GreaterOrEqual:
				case BinaryOperatorType.Less:
				case BinaryOperatorType.LessOrEqual:
#pragma warning disable 618
				case BinaryOperatorType.Like:
#pragma warning restore 618
				case BinaryOperatorType.NotEqual:
					return BooleanCriteriaState.Logical;
			}
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(UnaryOperator theOperator) {
			BooleanCriteriaState res = Process(theOperator.Operand);
			if (theOperator.OperatorType == UnaryOperatorType.Not) {
				if (res == BooleanCriteriaState.Value) throw new ArgumentException(MustBeLogical);
				return BooleanCriteriaState.Logical;
			}
			if (res == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			if(theOperator.OperatorType == UnaryOperatorType.IsNull) return BooleanCriteriaState.Logical;
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(InOperator theOperator) {
			BooleanCriteriaState leftRes = Process(theOperator.LeftOperand);
			if (leftRes == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			foreach (CriteriaOperator co in theOperator.Operands) {
				BooleanCriteriaState coRes = Process(co);
				if (coRes == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			}
			return BooleanCriteriaState.Logical;
		}
		public BooleanCriteriaState Visit(GroupOperator theOperator) {
			foreach (CriteriaOperator co in theOperator.Operands) {
				BooleanCriteriaState coRes = Process(co);
				if (coRes == BooleanCriteriaState.Value) throw new ArgumentException(MustBeLogical);
			}
			return BooleanCriteriaState.Logical;
		}
		public BooleanCriteriaState Visit(OperandValue theOperand) {
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(FunctionOperator theOperator) {
			if (theOperator.OperatorType == FunctionOperatorType.Iif) {
				int i = 0;
				for(; i < theOperator.Operands.Count - 1; i += 2) {
					if(Process(theOperator.Operands[i]) == BooleanCriteriaState.Value) throw new ArgumentException(MustBeLogical);
					if(Process(theOperator.Operands[i+1]) == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
				}
				if(Process(theOperator.Operands[theOperator.Operands.Count - 1]) == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
				return BooleanCriteriaState.Value;
			}else if(theOperator.OperatorType == FunctionOperatorType.IsNull){
				if(Process(theOperator.Operands[0]) == BooleanCriteriaState.Logical)throw new ArgumentException(MustBeArithmetical);
				if (theOperator.Operands.Count == 2) {
					if (Process(theOperator.Operands[1]) == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
					return BooleanCriteriaState.Value;
				}
				return BooleanCriteriaState.Logical;
			}
			foreach (CriteriaOperator co in theOperator.Operands) {
				if (Process(co) == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			}
			switch (theOperator.OperatorType) {
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
				case FunctionOperatorType.IsOutlookIntervalToday:
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
				case FunctionOperatorType.IsOutlookIntervalYesterday:
				case FunctionOperatorType.IsNullOrEmpty:
				case FunctionOperatorType.StartsWith:
				case FunctionOperatorType.EndsWith:
				case FunctionOperatorType.Contains:
				case FunctionOperatorType.IsThisMonth:
				case FunctionOperatorType.IsThisWeek:
				case FunctionOperatorType.IsThisYear:
					return BooleanCriteriaState.Logical;
				case FunctionOperatorType.Custom:
				case FunctionOperatorType.CustomNonDeterministic:
					if(FunctionOperator.GuessIsLogicalCustomFunction(theOperator)) 
						return BooleanCriteriaState.Logical;
					return BooleanCriteriaState.Undefined;
			}
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Process(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null)) return BooleanCriteriaState.Logical;
			return operand.Accept(this);
		}
		public void Process(CriteriaOperator operand, bool mustBeLogical) {
			if (ReferenceEquals(operand, null)) return;
			if (mustBeLogical) {
				if ((BooleanCriteriaState)operand.Accept(this) == BooleanCriteriaState.Value) throw new ArgumentException(MustBeLogical);
			} else {
				if ((BooleanCriteriaState)operand.Accept(this) == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			}
		}
	}
	public partial class IsLogicalCriteriaChecker: IClientCriteriaVisitor<BooleanCriteriaState> {
		static IsLogicalCriteriaChecker instance = new IsLogicalCriteriaChecker();
		public static IsLogicalCriteriaChecker Instance { get { return instance; } }
		public static BooleanCriteriaState GetBooleanState(CriteriaOperator operand) {
			return Instance.Process(operand);
		}
		public BooleanCriteriaState Visit(AggregateOperand theOperand) {
			if (theOperand.AggregateType == Aggregate.Exists) {
				return BooleanCriteriaState.Logical;
			}
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(JoinOperand theOperand) {
			if(theOperand.AggregateType == Aggregate.Exists) {
				return BooleanCriteriaState.Logical;
			}
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(OperandProperty theOperand) {
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(BetweenOperator theOperator) {
			return BooleanCriteriaState.Logical;
		}
		public BooleanCriteriaState Visit(BinaryOperator theOperator) {
			switch (theOperator.OperatorType) {
				case BinaryOperatorType.Equal:
				case BinaryOperatorType.Greater:
				case BinaryOperatorType.GreaterOrEqual:
				case BinaryOperatorType.Less:
				case BinaryOperatorType.LessOrEqual:
#pragma warning disable 618
				case BinaryOperatorType.Like:
#pragma warning restore 618
				case BinaryOperatorType.NotEqual:
					return BooleanCriteriaState.Logical;
			}
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(UnaryOperator theOperator) {
			if (theOperator.OperatorType == UnaryOperatorType.Not) {
				return BooleanCriteriaState.Logical;
			}
			if(theOperator.OperatorType == UnaryOperatorType.IsNull) return BooleanCriteriaState.Logical;
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(InOperator theOperator) {
			return BooleanCriteriaState.Logical;
		}
		public BooleanCriteriaState Visit(GroupOperator theOperator) {
			return BooleanCriteriaState.Logical;
		}
		public BooleanCriteriaState Visit(OperandValue theOperand) {
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(FunctionOperator theOperator) {
			if (theOperator.OperatorType == FunctionOperatorType.IsNull) {
				if (theOperator.Operands.Count == 2) {
					return BooleanCriteriaState.Value;
				}
				return BooleanCriteriaState.Logical;
			}
			switch (theOperator.OperatorType) {
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
				case FunctionOperatorType.IsOutlookIntervalToday:
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
				case FunctionOperatorType.IsOutlookIntervalYesterday:
				case FunctionOperatorType.IsNullOrEmpty:
				case FunctionOperatorType.StartsWith:
				case FunctionOperatorType.EndsWith:
				case FunctionOperatorType.Contains:
				case FunctionOperatorType.IsThisMonth:
				case FunctionOperatorType.IsThisWeek:
				case FunctionOperatorType.IsThisYear:
					return BooleanCriteriaState.Logical;
				case FunctionOperatorType.Custom:
				case FunctionOperatorType.CustomNonDeterministic:
					if(FunctionOperator.GuessIsLogicalCustomFunction(theOperator))
						return BooleanCriteriaState.Logical;
					return BooleanCriteriaState.Undefined;
			}
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Process(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null)) return BooleanCriteriaState.Logical;
			return operand.Accept(this);
		}
	}
	public class JoinContextPropertyInfoSet{
		Dictionary<JoinContextPropertyInfo, bool> properties;
		public int Count{ get {return properties == null ? 0 : properties.Count; }}
		public JoinContextPropertyInfoSet(Dictionary<JoinContextPropertyInfo, bool> properties){
			this.properties = properties;
		}
		public JoinContextValueInfoSet GetJoinContextValueInfoSet(EvaluatorContext context) {
			Dictionary<string, object> result = new Dictionary<string, object>();
			foreach(JoinContextPropertyInfo propertyInfo in properties.Keys) {
				result[propertyInfo.PropertyNameInCriteria] = context.GetPropertyValue(propertyInfo.Property);
			}
			return new JoinContextValueInfoSet(result);
		}
	}
	public class JoinContextPropertyInfo{
		public readonly EvaluatorProperty Property;
		public readonly string PropertyNameInCriteria;
		public JoinContextPropertyInfo(EvaluatorProperty property, string propertyNameInCriteria) {
			Property = property;
			PropertyNameInCriteria = propertyNameInCriteria;
		}
		public override int GetHashCode() {
			return (Property == null ? 0x1D45A594 : Property.GetHashCode()) ^ (PropertyNameInCriteria == null ? 0x3F436A32 : PropertyNameInCriteria.GetHashCode());
		}
		public override bool Equals(object obj) {
			JoinContextPropertyInfo other = obj as JoinContextPropertyInfo;
			if(other == null)return false;
			return string.Equals(Property, other.Property) && string.Equals(PropertyNameInCriteria, other.PropertyNameInCriteria);
		}
		public override string ToString() {
			return string.Format("{0}({1})", Property, PropertyNameInCriteria);
		}
	}
	public class JoinContextValueInfoSet {
		Dictionary<string, object> properties;
		public Dictionary<string, object> Properties { get { return properties; } }
		public JoinContextValueInfoSet(Dictionary<string, object> properties) {
			this.properties = properties;
		}
	}
	public class JoinContextCriteriaPatcher : ClientCriteriaVisitorBase {
		readonly JoinContextValueInfoSet valueInfoSet;
		public JoinContextCriteriaPatcher(JoinContextValueInfoSet valueInfoSet) {
			this.valueInfoSet = valueInfoSet;
		}
		public static CriteriaOperator Process(JoinContextValueInfoSet valueInfoSet, CriteriaOperator criteria) {
			return new JoinContextCriteriaPatcher(valueInfoSet).Process(criteria);
		}
		protected override CriteriaOperator Visit(OperandProperty theOperand) {
			object value;
			if(valueInfoSet.Properties.TryGetValue(theOperand.PropertyName, out value)) {
				return new OperandValue(value);
			}
			return theOperand;
		}
		protected override CriteriaOperator Visit(AggregateOperand theOperand, bool processCollectionProperty) {
			return base.Visit(theOperand, false);
		}
	}
	public class JoinContextCriteriaCreator : ClientCriteriaVisitorBase {
		int level = 1;
		readonly bool zeroLevelLeave;
		readonly EvaluatorContext[] contexts;
		readonly Dictionary<JoinContextPropertyInfo, bool> zeroLevelProperies;
		Dictionary<string, EvaluatorProperty> propertyCache = new Dictionary<string, EvaluatorProperty>();
		public JoinContextCriteriaCreator(EvaluatorContext[] contexts) {
			this.contexts = contexts;
		}
		public JoinContextCriteriaCreator(EvaluatorContext[] contexts, bool zeroLevelLeave)
			: this(contexts) {
			this.zeroLevelLeave = zeroLevelLeave;
			this.zeroLevelProperies = new Dictionary<JoinContextPropertyInfo, bool>();
		}
		public static CriteriaOperator ProcessZeroLevelLeave(EvaluatorContext[] contexts, CriteriaOperator criteria, out JoinContextPropertyInfoSet zeroLevelProperties) {
			JoinContextCriteriaCreator jccc = new JoinContextCriteriaCreator(contexts, true);
			CriteriaOperator result = jccc.Process(criteria);
			zeroLevelProperties = new JoinContextPropertyInfoSet(jccc.zeroLevelProperies);
			return result;
		}
		public static CriteriaOperator Process(EvaluatorContext[] contexts, CriteriaOperator criteria) {
			return new JoinContextCriteriaCreator(contexts).Process(criteria);
		}
		EvaluatorProperty GetProperty(string propertyPath) {
			EvaluatorProperty result;
			if(!propertyCache.TryGetValue(propertyPath, out result)) {
				result = EvaluatorProperty.Create(new OperandProperty(propertyPath));
				propertyCache.Add(propertyPath, result);
			}
			return result;
		}
		protected override CriteriaOperator Visit(AggregateOperand theOperand, bool processCollectionProperty) {
			++level;
			try {
				return base.Visit(theOperand, false);
			} finally {
				--level;
			}
		}
		protected override CriteriaOperator Visit(OperandProperty theOperand) {
			string propertyPath = theOperand.PropertyName;
			int currentLevel = level;
			while(propertyPath.StartsWith("^.")) {
				propertyPath = propertyPath.Substring(2);
				--currentLevel;
			}
			if(currentLevel <= 0) {
				if(zeroLevelLeave && currentLevel == 0) {
					string propertyIdName = string.Format("{0}#|#{1}", level, theOperand.PropertyName);
					zeroLevelProperies[new JoinContextPropertyInfo(GetProperty(propertyPath), propertyIdName)] = true;
					return new OperandProperty(propertyIdName);
				}
				return new OperandValue(contexts[1 - currentLevel].GetPropertyValue(GetProperty(propertyPath)));
			}
			return theOperand;
		}
		protected override CriteriaOperator Visit(JoinOperand theOperand) {
			++level;
			try {
				return base.Visit(theOperand);
			} finally {
				--level;
			}
		}
	}
}
namespace DevExpress.Data.Filtering.Helpers {
	using DevExpress.Data.Filtering;
	using System.Collections.Generic;
	public class ClientCriteriaVisitorBase: IClientCriteriaVisitor<CriteriaOperator> {
		protected virtual CriteriaOperator Visit(OperandProperty theOperand) {
			return theOperand;
		}
		protected virtual CriteriaOperator Visit(AggregateOperand theOperand, bool processCollectionProperty) {
			OperandProperty collectionProperty = processCollectionProperty ? (OperandProperty)Process(theOperand.CollectionProperty) : theOperand.CollectionProperty;
			CriteriaOperator aggregatedExpression = Process(theOperand.AggregatedExpression);
			CriteriaOperator condition = Process(theOperand.Condition);
			if(ReferenceEquals(collectionProperty, theOperand.CollectionProperty) && ReferenceEquals(aggregatedExpression, theOperand.AggregatedExpression)
				&& ReferenceEquals(condition, theOperand.Condition)) {
				return theOperand;
			}
			return new AggregateOperand(collectionProperty, aggregatedExpression, theOperand.AggregateType, condition);
		}
		protected virtual CriteriaOperator Visit(JoinOperand theOperand) {
			CriteriaOperator aggregatedExpression = Process(theOperand.AggregatedExpression);
			CriteriaOperator condition = Process(theOperand.Condition);
			if(ReferenceEquals(aggregatedExpression, theOperand.AggregatedExpression) && ReferenceEquals(condition, theOperand.Condition)) {
				return theOperand;
			}
			return new JoinOperand(theOperand.JoinTypeName, Process(theOperand.Condition), theOperand.AggregateType, Process(theOperand.AggregatedExpression));
		}
		protected virtual CriteriaOperator Visit(FunctionOperator theOperator) {
			bool modified;
			List<CriteriaOperator> resultOperands = ProcessCollection(theOperator.Operands, out modified);
			return modified ? new FunctionOperator(theOperator.OperatorType, resultOperands) : theOperator;
		}
		protected virtual CriteriaOperator Visit(OperandValue theOperand) {
			return theOperand;
		}
		protected virtual CriteriaOperator Visit(GroupOperator theOperator) {
			bool modified;
			List<CriteriaOperator> resultOperands = ProcessCollection(theOperator.Operands, out modified);
			return modified ? new GroupOperator(theOperator.OperatorType, resultOperands) : theOperator;
		}
		protected virtual CriteriaOperator Visit(InOperator theOperator) {
			bool modified;
			CriteriaOperator leftOperand = Process(theOperator.LeftOperand);
			List<CriteriaOperator> resultOperands = ProcessCollection(theOperator.Operands, out modified);
			return modified || !ReferenceEquals(leftOperand, theOperator.LeftOperand) ? new InOperator(leftOperand, resultOperands) : theOperator;
		}
		protected virtual CriteriaOperator Visit(UnaryOperator theOperator) {
			CriteriaOperator operand = Process(theOperator.Operand);
			return ReferenceEquals(operand, theOperator.Operand) ? theOperator : new UnaryOperator(theOperator.OperatorType, operand);
		}
		protected virtual CriteriaOperator Visit(BinaryOperator theOperator) {
			CriteriaOperator leftOperand = Process(theOperator.LeftOperand);
			CriteriaOperator rightOperand = Process(theOperator.RightOperand);
			if(ReferenceEquals(leftOperand, theOperator.LeftOperand) && ReferenceEquals(rightOperand, theOperator.RightOperand)) {
				return theOperator;
			}
			return new BinaryOperator(leftOperand, rightOperand, theOperator.OperatorType);
		}
		protected virtual CriteriaOperator Visit(BetweenOperator theOperator) {
			CriteriaOperator test = Process(theOperator.TestExpression);
			CriteriaOperator begin = Process(theOperator.BeginExpression);
			CriteriaOperator end = Process(theOperator.EndExpression);
			if(ReferenceEquals(test, theOperator.TestExpression) && ReferenceEquals(begin, theOperator.BeginExpression) && ReferenceEquals(end, theOperator.EndExpression)) {
				return theOperator;
			}
			return new BetweenOperator(test, begin, end);
		}
		protected CriteriaOperator Process(CriteriaOperator input) {
			if(ReferenceEquals(input, null))
				return null;
			return input.Accept(this);
		}
		protected CriteriaOperatorCollection ProcessCollection(CriteriaOperatorCollection operands, out bool modified) {
			modified = false;
			CriteriaOperatorCollection resultOperands = new CriteriaOperatorCollection();
			resultOperands.Capacity = operands.Count;
			for(int i = 0; i < operands.Count; i++) {
				CriteriaOperator resultOp = Process(operands[i]);
				if(!modified && !ReferenceEquals(resultOp, operands[i])) {
					modified = true;
				}
				resultOperands.Add(resultOp);
			}
			return resultOperands;
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(JoinOperand theOperand) { return Visit(theOperand); }
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(OperandProperty theOperand) { return Visit(theOperand); }
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(AggregateOperand theOperand) { return Visit(theOperand, true); }
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(FunctionOperator theOperator) { return Visit(theOperator); }
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(OperandValue theOperand) { return Visit(theOperand); }
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(GroupOperator theOperator) { return Visit(theOperator); }
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(InOperator theOperator) { return Visit(theOperator); }
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(UnaryOperator theOperator) { return Visit(theOperator); }
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BinaryOperator theOperator) { return Visit(theOperator); }
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BetweenOperator theOperator) { return Visit(theOperator); }
	}
}
