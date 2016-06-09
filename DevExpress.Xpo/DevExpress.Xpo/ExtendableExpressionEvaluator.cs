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
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Xpo.Helpers {
using DevExpress.Xpo.DB.Helpers;
	public abstract class QuereableEvaluatorContextDescriptor : EvaluatorContextDescriptor {
		public abstract object GetQueryResult(DevExpress.Xpo.DB.JoinNode root);
		public abstract object GetOperandValue(object source, DevExpress.Xpo.DB.QueryOperand theOperand);
		public abstract void PushNestedSource(object source);
		public abstract void PopNestedSource();
	}
	class QuereableEvaluatorContext : EvaluatorContext {
		public QuereableEvaluatorContext(EvaluatorContextDescriptor descriptor, object source) : base(descriptor, source) { }
		public object GetOperandValue(DevExpress.Xpo.DB.QueryOperand theOperand) {
			return ((QuereableEvaluatorContextDescriptor)Descriptor).GetOperandValue(Source, theOperand);
		}
	}
	class QuereableExpressionEvaluatorCore : ExpressionEvaluatorCoreBase, IQueryCriteriaVisitor<object> {
		QuereableEvaluatorContext defaultContext;
		public QuereableExpressionEvaluatorCore(bool caseSensitive) : base(caseSensitive) { }
		public QuereableExpressionEvaluatorCore(bool caseSensitive, EvaluateCustomFunctionHandler evaluateCustomFunction) : base(caseSensitive, evaluateCustomFunction) { }
		object IQueryCriteriaVisitor<object>.Visit(DevExpress.Xpo.DB.QuerySubQueryContainer theOperand) {
			QuereableEvaluatorContextDescriptor descriptor = (QuereableEvaluatorContextDescriptor)defaultContext.Descriptor;
			object source = defaultContext.Source;
			descriptor.PushNestedSource(source);
			try {
				if(!ReferenceEquals(theOperand.Node, null)) source = descriptor.GetQueryResult(theOperand.Node);
				List<QuereableEvaluatorContext> currentContexts = new List<QuereableEvaluatorContext>();
				IEnumerable rows = source as IEnumerable;
				if(rows == null) {
					currentContexts.Add(new QuereableEvaluatorContext(descriptor, source));
				} else {
					foreach(object row in rows) {
						currentContexts.Add(new QuereableEvaluatorContext(descriptor, row));
					}
				}
				QuereableEvaluatorContext saveContext = defaultContext;
				try {
					return DoAggregate(theOperand.AggregateType, currentContexts, null, theOperand.AggregateProperty);
				} finally {
					defaultContext = saveContext;
				}
			} finally {
				descriptor.PopNestedSource();
			}
		}
		class CriteriaOperatorReferenceComparer : IEqualityComparer<CriteriaOperator> {
			readonly static CriteriaOperatorReferenceComparer instance = new CriteriaOperatorReferenceComparer();
			public static CriteriaOperatorReferenceComparer Instance { get { return instance; } }
			bool IEqualityComparer<CriteriaOperator>.Equals(CriteriaOperator x, CriteriaOperator y) {
				return ReferenceEquals(x, y);
			}
			int IEqualityComparer<CriteriaOperator>.GetHashCode(CriteriaOperator obj) {
				if(ReferenceEquals(obj, null)) return 0x3450232;
				return obj.GetHashCode();
			}
		}
		class InOperatorCacheItem {
			public readonly Dictionary<object, bool> Dictionary;
			public readonly bool HasNullValue;
			public readonly bool IsNonDeterministic;
			public InOperatorCacheItem(Dictionary<object, bool> dictionary, bool hasNullValue){
				Dictionary = dictionary;
				HasNullValue = hasNullValue;
			}
			public InOperatorCacheItem() {
				IsNonDeterministic = true;
			}
			public bool Contains(object value) {
				if(value == null)
					return HasNullValue;
				return Dictionary.ContainsKey(value);
			}
		}
		class InOperatorCacheItemComparer : IEqualityComparer<object> {
			public readonly bool CaseSensitive;
			public readonly IComparer CustomComparer;
			public readonly bool IsEqulityComparer;
			public InOperatorCacheItemComparer(bool isEqulityComparer, bool caseSensitive, IComparer customComparer) {
				IsEqulityComparer = isEqulityComparer;
				CaseSensitive = caseSensitive;
				CustomComparer = customComparer;
			}
			bool IEqualityComparer<object>.Equals(object x, object y) {
				return EvalHelpers.CompareObjects(x, y, IsEqulityComparer, CaseSensitive, CustomComparer) == 0;
			}
			int IEqualityComparer<object>.GetHashCode(object obj) {
				return obj.GetHashCode();
			}
		}
		Dictionary<CriteriaOperator, InOperatorCacheItem> inOperatorOptimizer;
		object ICriteriaVisitor<object>.Visit(InOperator theOperator) {
			object val = Process(theOperator.LeftOperand);
			if(theOperator.Operands.Count > 3) {
				if(inOperatorOptimizer == null) inOperatorOptimizer = new Dictionary<CriteriaOperator, InOperatorCacheItem>(CriteriaOperatorReferenceComparer.Instance);
				InOperatorCacheItem cacheItem;
				if(!inOperatorOptimizer.TryGetValue(theOperator, out cacheItem)) {
					bool hasNullValue = false;
					bool isNonDeterministic = false;
					Dictionary<object, bool> dictionary = new Dictionary<object, bool>(new InOperatorCacheItemComparer(true, CaseSensitive, CustomComparer));
					foreach(CriteriaOperator op in theOperator.Operands) {
						if(!(op is OperandValue)) {
							isNonDeterministic = true;
							break;
						}
						object operand = Process(op);
						if(operand == null)
							hasNullValue = true;
						else
							dictionary[operand] = true;
					}
					cacheItem = isNonDeterministic ? new InOperatorCacheItem() : new InOperatorCacheItem(dictionary, hasNullValue);
					inOperatorOptimizer.Add(theOperator, cacheItem);
				}
				if(!cacheItem.IsNonDeterministic)
					return GetBool(cacheItem.Contains(val));
			}
			foreach(CriteriaOperator op in theOperator.Operands)
				if(EvalHelpers.CompareObjects(val, Process(op), true, CaseSensitive, CustomComparer) == 0)
					return GetBool(true);
			return GetBool(false);
		}
		object IQueryCriteriaVisitor<object>.Visit(DevExpress.Xpo.DB.QueryOperand theOperand) {
			return defaultContext.GetOperandValue(theOperand);
		}
		protected sealed override void SetContext(EvaluatorContext context) {
			defaultContext = (QuereableEvaluatorContext)context;
		}
		protected sealed override EvaluatorContext GetContext(int upDepth) {
			return defaultContext;
		}
		protected sealed override EvaluatorContext GetContext() {
			return defaultContext;
		}
		protected sealed override void ClearContext() {
			defaultContext = null;
		}
		protected sealed override bool HasContext {
			get { return defaultContext != null; }
		}
	}
	class QuereableExpressionEvaluator : ExpressionEvaluator {
		readonly ExpressionEvaluatorCoreBase evaluatorCore;
		protected override ExpressionEvaluatorCoreBase EvaluatorCore { get { return evaluatorCore; } }
		public QuereableExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive)
			: base(descriptor, criteria, caseSensitive, false) {
			this.evaluatorCore = new QuereableExpressionEvaluatorCore(caseSensitive, new EvaluateCustomFunctionHandler(EvaluateCustomFunction));
		}
		public QuereableExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria) : this(descriptor, criteria, true) { }
		public QuereableExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions)
			: base(descriptor, criteria, caseSensitive, false, customFunctions) {
			this.evaluatorCore = new QuereableExpressionEvaluatorCore(caseSensitive, new EvaluateCustomFunctionHandler(EvaluateCustomFunction));
		}
		public QuereableExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, ICollection<ICustomFunctionOperator> customFunctions)
			: this(descriptor, criteria, true, customFunctions) { }
#if !SL
		public QuereableExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive)
			: base(properties, criteria, caseSensitive, false) {
			this.evaluatorCore = new QuereableExpressionEvaluatorCore(caseSensitive, new EvaluateCustomFunctionHandler(EvaluateCustomFunction));
		}
		public QuereableExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria) : this(properties, criteria, true) { }
		public QuereableExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, bool caseSensitive) : this(properties, CriteriaOperator.Parse(criteria), caseSensitive) { }
		public QuereableExpressionEvaluator(PropertyDescriptorCollection properties, string criteria) : this(properties, criteria, true) { }
		public QuereableExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions)
			: base(properties, criteria, caseSensitive, false ,customFunctions) {
			this.evaluatorCore = new QuereableExpressionEvaluatorCore(caseSensitive, new EvaluateCustomFunctionHandler(EvaluateCustomFunction));
		}
		public QuereableExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, criteria, true, customFunctions) { }
		public QuereableExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, CriteriaOperator.Parse(criteria), caseSensitive, customFunctions) { }
		public QuereableExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, CriteriaOperator.Parse(criteria), true, customFunctions) { }
#endif
		protected override EvaluatorContext PrepareContext(object valuesSource) {
			return new QuereableEvaluatorContext((QuereableEvaluatorContextDescriptor)DefaultDescriptor, valuesSource);
		}
	}
}
