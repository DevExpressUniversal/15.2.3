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
using System.Text;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.Xpo.Exceptions;
using DevExpress.Data.Filtering.Helpers;
using System.Collections;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.Xpo.Helpers;
namespace DevExpress.Xpo {
	public interface ISecurityRule {
		XPClassInfo[] SupportedObjectTypes { get; }
		bool GetSelectFilterCriteria(SecurityContext context, XPClassInfo classInfo, out CriteriaOperator criteria);
		bool GetSelectMemberExpression(SecurityContext context, XPClassInfo classInfo, XPMemberInfo memberInfo, out CriteriaOperator expression);
		bool ValidateObjectOnSelect(SecurityContext context, XPClassInfo classInfo, object realObjectOnLoad);
		bool ValidateObjectOnSave(SecurityContext context, XPClassInfo classInfo, object theObject, object realObjectOnLoad);
		bool ValidateObjectOnDelete(SecurityContext context, XPClassInfo classInfo, object theObject, object realObjectOnLoad);
		ValidateMemberOnSaveResult ValidateMemberOnSave(SecurityContext context, XPMemberInfo memberInfo, object theObject, object realObjectOnLoad, object value, object valueOnLoad, object realValueOnLoad);
	}
	public interface IGenericSecurityRule {
		bool ValidateObjectsOnCommit(SecurityContext context, SecurityContextValidateItem[] objectsToSave, SecurityContextValidateItem[] objectsToDelete);
	}
	public interface ISecurityRuleProvider {
		ISecurityRule GetRule(XPClassInfo classInfo);
	}
	public enum ValidateMemberOnSaveResult {
		DoSaveMember,
		DoNotSaveMember,
		DoRaiseException
	}
	public struct SecurityContextValidateItem {
		public readonly XPClassInfo ClassInfo;
		public readonly object TheObject;
		public readonly object RealObject;
		internal SecurityContextValidateItem(XPClassInfo classInfo, object theObject, object realObject) {
			this.ClassInfo = classInfo;
			this.TheObject = theObject;
			this.RealObject = realObject;
		}
	}
	public class SecurityContext {
		readonly object customContext;
		readonly IGenericSecurityRule genericSecurityRule;
		readonly ISecurityRuleProvider securityRuleProvider;
		readonly Session nestedSession;
		readonly SessionObjectLayer parentObjectLayer;
		readonly Dictionary<ExpressonEvaluatorCasheItem, ExpressionEvaluator> evaluatorDictionary = new Dictionary<ExpressonEvaluatorCasheItem, ExpressionEvaluator>(ExpressonEvaluatorCasheItemComparer.Instance);
		readonly Dictionary<SelectMemberExpressionCacheItem, SelectMemberExpressionCacheItemResult> selectMemberExpressionDictionary = new Dictionary<SelectMemberExpressionCacheItem, SelectMemberExpressionCacheItemResult>(SelectMemberExpressionCacheItemComparer.Instance);
		XPDictionary Dictionary { get { return parentObjectLayer.Dictionary; } }
		public object CustomContext { get { return customContext; } }
		public IGenericSecurityRule GenericSecurityRule { get { return genericSecurityRule; } }
		public ISecurityRuleProvider SecurityRuleProvider { get { return securityRuleProvider; } }
		public SecurityContext(SessionObjectLayer parentObjectLayer, IGenericSecurityRule genericSecurityRule, ISecurityRuleProvider securityRuleProvide, object customContext) {
			if(securityRuleProvide == null) throw new ArgumentNullException("securityDictionary");
			this.securityRuleProvider = securityRuleProvide;
			this.customContext = customContext;
			this.parentObjectLayer = parentObjectLayer;
			this.genericSecurityRule = genericSecurityRule;
		}
		public SecurityContext(SessionObjectLayer parentObjectLayer, IGenericSecurityRule genericSecurityRule, ISecurityRuleProvider securityRuleProvide, object customContext, Session nestedSession)
			: this(parentObjectLayer, genericSecurityRule, securityRuleProvide, customContext) {
			this.nestedSession = nestedSession;
		}
		public ExpressionEvaluator GetEvaluator(XPClassInfo classInfo, CriteriaOperator criteria) {
			ExpressonEvaluatorCasheItem casheItem = new ExpressonEvaluatorCasheItem(classInfo, criteria);
			ExpressionEvaluator evaluator;
			if(evaluatorDictionary.TryGetValue(casheItem, out evaluator)) return evaluator;
			evaluator = new ExpressionEvaluator(classInfo.GetEvaluatorContextDescriptor(), criteria, nestedSession.CaseSensitive, Dictionary.CustomFunctionOperators);
			evaluatorDictionary.Add(casheItem, evaluator);
			return evaluator;
		}
		public CriteriaOperator ParseCriteria(string expressionString, out OperandValue[] criteriaParameterList) {
			return nestedSession.ParseCriteria(expressionString, out criteriaParameterList);
		}
		public CriteriaOperator ParseCriteria(string expressionString, params object[] parameters) {
			return nestedSession.ParseCriteria(expressionString, parameters);
		}
		public CriteriaOperator ParseCriteriaOnParentSession(string expressionString, out OperandValue[] criteriaParameterList) {
			return parentObjectLayer.ParentSession.ParseCriteria(expressionString, out criteriaParameterList);
		}
		public CriteriaOperator ParseCriteriaOnParentSession(string expressionString, params object[] parameters) {
			return parentObjectLayer.ParentSession.ParseCriteria(expressionString, parameters);
		}
		public object Evaluate(XPClassInfo classInfo, CriteriaOperator expression, object theObject) {
			return GetEvaluator(classInfo, expression).Evaluate(theObject);
		}
		public bool Fit(XPClassInfo classInfo, CriteriaOperator criteria, object theObject) {
			return GetEvaluator(classInfo, criteria).Fit(theObject);
		}
		public object EvaluateOnParentSession(XPClassInfo classInfo, CriteriaOperator expression, CriteriaOperator criteria) {
			return parentObjectLayer.ParentSession.Evaluate(classInfo, expression, criteria);
		}
		public CriteriaOperator ExpandToLogical(XPClassInfo classInfo, CriteriaOperator op) {			
			return PersistentCriterionExpander.ExpandToLogical(nestedSession == null ? parentObjectLayer.ParentSession : nestedSession, classInfo, op, false).ExpandedCriteria;
		}
		public CriteriaOperator ExpandToValue(XPClassInfo classInfo, CriteriaOperator op) {
			return PersistentCriterionExpander.ExpandToValue(nestedSession == null ? parentObjectLayer.ParentSession : nestedSession, classInfo, op, false).ExpandedCriteria;
		}
		public XPClassInfo GetClassInfo(object theObject) {
			return Dictionary.QueryClassInfo(theObject);
		}
		public SecurityContext Clone(Session nestedSession) {
			return new SecurityContext(parentObjectLayer, genericSecurityRule, securityRuleProvider, customContext, nestedSession);
		}
		public bool IsObjectMarkedDeleted(object theObject) {
			return nestedSession.IsObjectMarkedDeleted(theObject);
		}
		public static bool IsSystemProperty(XPMemberInfo mi) {
			return mi != null && (mi.IsKey || mi is ServiceField);
		}
		public static bool IsSystemClass(XPClassInfo ci) {
			return ci != null && typeof(IXPOServiceClass).IsAssignableFrom(ci.ClassType);
		}
		public bool GetSelectMemberExpression(ISecurityRule rule, XPClassInfo ci, XPMemberInfo mi, out CriteriaOperator memberExpression) {
			if(rule == null || ci == null || mi == null) {
				memberExpression = null;
				return false;
			}
			SelectMemberExpressionCacheItem item = new SelectMemberExpressionCacheItem(rule, ci, mi);
			SelectMemberExpressionCacheItemResult resultItem;
			if(!selectMemberExpressionDictionary.TryGetValue(item, out resultItem)) {
				CriteriaOperator expression;
				resultItem = new SelectMemberExpressionCacheItemResult(rule.GetSelectMemberExpression(this, ci, mi, out expression), expression);
				selectMemberExpressionDictionary.Add(item, resultItem);
			}
			memberExpression = resultItem.Expression;
			return resultItem.Result;
		}
		struct ExpressonEvaluatorCasheItem {
			public readonly XPClassInfo ClassInfo;
			public readonly CriteriaOperator Criteria;
			public ExpressonEvaluatorCasheItem(XPClassInfo classInfo, CriteriaOperator criteria) {
				if(classInfo == null) throw new ArgumentNullException("classInfo");
				this.ClassInfo = classInfo;
				this.Criteria = criteria;
			}
		}
		class ExpressonEvaluatorCasheItemComparer : IEqualityComparer<ExpressonEvaluatorCasheItem> {
			static ExpressonEvaluatorCasheItemComparer instance = new ExpressonEvaluatorCasheItemComparer();
			public static ExpressonEvaluatorCasheItemComparer Instance{
				get{ return instance; }
			}
			ExpressonEvaluatorCasheItemComparer() { }
			bool IEqualityComparer<ExpressonEvaluatorCasheItem>.Equals(ExpressonEvaluatorCasheItem x, ExpressonEvaluatorCasheItem y) {
				return (x.ClassInfo == y.ClassInfo) && CriteriaOperator.CriterionEquals(x.Criteria, y.Criteria);				
			}
			int IEqualityComparer<ExpressonEvaluatorCasheItem>.GetHashCode(ExpressonEvaluatorCasheItem obj) {
				return obj.ClassInfo.GetHashCode() ^ (ReferenceEquals(obj.Criteria, null) ? 0x135A185C : obj.Criteria.GetHashCode());
			}
		}
		struct SelectMemberExpressionCacheItem {
			public readonly ISecurityRule Rule;
			public readonly XPClassInfo ClassInfo;
			public readonly XPMemberInfo MemberInfo;
			public SelectMemberExpressionCacheItem(ISecurityRule rule, XPClassInfo ci, XPMemberInfo mi) {
				Rule = rule;
				ClassInfo = ci;
				MemberInfo = mi;
			}
		}
		struct SelectMemberExpressionCacheItemResult {
			public readonly bool Result;
			public readonly CriteriaOperator Expression;
			public SelectMemberExpressionCacheItemResult(bool result, CriteriaOperator expression) {
				Result = result;
				Expression = expression;
			}
		}
		class SelectMemberExpressionCacheItemComparer : IEqualityComparer<SelectMemberExpressionCacheItem> {
			static SelectMemberExpressionCacheItemComparer instance = new SelectMemberExpressionCacheItemComparer();
			public static SelectMemberExpressionCacheItemComparer Instance {
				get{ return instance; }
			}
			SelectMemberExpressionCacheItemComparer() { }
			bool IEqualityComparer<SelectMemberExpressionCacheItem>.Equals(SelectMemberExpressionCacheItem x, SelectMemberExpressionCacheItem y) {
				return x.Rule == y.Rule && x.ClassInfo == y.ClassInfo && x.MemberInfo == y.MemberInfo;
			}
			int IEqualityComparer<SelectMemberExpressionCacheItem>.GetHashCode(SelectMemberExpressionCacheItem obj) {
				return obj.Rule.GetHashCode() ^ obj.ClassInfo.GetHashCode() ^ obj.MemberInfo.GetHashCode();
			}
		}
	}
	public class SecurityRuleDictionary : CustomMultiKeyDictionaryCollection<XPClassInfo, ISecurityRule>, ISecurityRuleProvider {
		public SecurityRuleDictionary() : base() { }
		public SecurityRuleDictionary(IEqualityComparer<XPClassInfo> comparer) : base(comparer) { }
		protected override XPClassInfo[] GetKey(ISecurityRule item) {
			return item.SupportedObjectTypes;
		}
		public ISecurityRule GetRule(XPClassInfo classInfo) {
			return GetItem(classInfo);
		}
	}
	public class SecurityOneRuleProvider : ISecurityRuleProvider {
		ISecurityRule rule;
		public SecurityOneRuleProvider(ISecurityRule rule) {
			this.rule = rule;
		}
		public ISecurityRule GetRule(XPClassInfo classInfo) {
			return rule;
		}
	}
	public class SecurityExpressionCleaner : ClientCriteriaVisitorBase {
		readonly CriteriaOperator baseFilterCriteria;
		public SecurityExpressionCleaner(CriteriaOperator baseFilterCriteria) {
			this.baseFilterCriteria = baseFilterCriteria;
		}
		public static CriteriaOperator Clean(CriteriaOperator baseFilterCriteria, CriteriaOperator securityExpression) {
			return new SecurityExpressionCleaner(baseFilterCriteria).Process(securityExpression);
		}
		public CriteriaOperator Clean(CriteriaOperator securityExpression) {
			return Process(securityExpression);
		}
		protected override CriteriaOperator Visit(FunctionOperator theOperator) {
			if(theOperator.OperatorType == FunctionOperatorType.Iif) {
				if(theOperator.Operands.Count > 2 && object.Equals(theOperator.Operands[0], baseFilterCriteria)) {
					return theOperator.Operands[1];
				}
			}
			return base.Visit(theOperator);
		}
	}
	public class SecurityCriteriaPatcher : ClientCriteriaVisitorBase {
		XPClassInfo currentState;
		readonly SecurityContext securityContext;
		readonly Stack<XPClassInfo> stateStack = new Stack<XPClassInfo>();
		readonly SecurityExpressionCleaner securityExpressionCleaner;
		public SecurityCriteriaPatcher(XPClassInfo currentClassInfo, SecurityContext securityContext) {
			this.currentState = currentClassInfo;
			this.securityContext = securityContext;
			if(!SecurityContext.IsSystemClass(currentClassInfo)) {
				ISecurityRule rule = securityContext.SecurityRuleProvider.GetRule(currentClassInfo);
				CriteriaOperator baseFilterCriteria;
				if(rule != null && rule.GetSelectFilterCriteria(securityContext, currentClassInfo, out baseFilterCriteria)) {
					securityExpressionCleaner = new SecurityExpressionCleaner(baseFilterCriteria);
				}
			}
		}
		public static CriteriaOperator Patch(XPClassInfo currentClassInfo, SecurityContext securityContext, CriteriaOperator criteria) {
			return new SecurityCriteriaPatcher(currentClassInfo, securityContext).Process(criteria);
		}
		protected override CriteriaOperator Visit(JoinOperand theOperand) {
			XPClassInfo joinedCi = null;
			if(!MemberInfoCollection.TryResolveTypeAlsoByShortName(theOperand.JoinTypeName, currentState, out joinedCi)) {
				throw new CannotResolveClassInfoException(string.Empty, theOperand.JoinTypeName);
			}
			XPClassInfo newState = joinedCi;
			stateStack.Push(currentState);
			currentState = newState;
			try {
				CriteriaOperator aggregatedExpression = Process(theOperand.AggregatedExpression);
				CriteriaOperator condition = Process(theOperand.Condition);
				return ReferenceEquals(aggregatedExpression, theOperand.AggregatedExpression)
					&& ReferenceEquals(condition, theOperand.Condition) ? theOperand : new JoinOperand(theOperand.JoinTypeName, condition, theOperand.AggregateType, aggregatedExpression);
			} finally {
				currentState = stateStack.Pop();
			}
		}
		protected override CriteriaOperator Visit(OperandProperty theOperand) {
			Stack<XPClassInfo> localStateStack = null;
			string propertyName = theOperand.PropertyName;
			while(propertyName.StartsWith("^.")) {
				if(localStateStack == null) {
					localStateStack = new Stack<XPClassInfo>();
				}
				if(stateStack.Count == 0) throw new InvalidOperationException("^.");
				propertyName = propertyName.Substring(2);
				localStateStack.Push(currentState);
				currentState = stateStack.Pop();
			}
			try {
				MemberInfoCollection mic = currentState.ParsePath(propertyName);
				if(mic == null) return theOperand;
				if(mic.Count > 0) {
					XPClassInfo currentClassInfo = currentState;
					for(int i = 0; i < (mic.Count - 1); i++) {
						currentClassInfo = mic[i].ReferenceType;
						if(currentClassInfo == null) throw new InvalidOperationException(); 
					}
					XPMemberInfo mi = mic[mic.Count - 1];
					if(!SecurityContext.IsSystemClass(currentClassInfo) && !SecurityContext.IsSystemProperty(mi)) {
						ISecurityRule rule = securityContext.SecurityRuleProvider.GetRule(currentClassInfo);
						CriteriaOperator expression;
						if(rule != null && securityContext.GetSelectMemberExpression(rule, currentClassInfo, mi, out expression)) {
							if(securityExpressionCleaner != null && stateStack.Count == 0) {
								expression = securityExpressionCleaner.Clean(expression);
							}
							CriteriaOperator expanded = securityContext.ExpandToValue(currentClassInfo, expression);
							StringBuilder prefix = null;
							if(localStateStack != null) {
								prefix = new StringBuilder();
								for(int i = 0; i < localStateStack.Count; i++) {
									prefix.Append("^.");
								}
							}
							if(mic.Count > 1) {
								MemberInfoCollection prefixMic = new MemberInfoCollection(currentState, mic.GetRange(0, mic.Count - 1).ToArray());
								string prefixMicString = prefixMic.ToString();
								if(prefix == null)
									prefix = new StringBuilder(prefixMicString.Length + 1);
								prefix.Append(prefixMicString);
								prefix.Append('.');
							}
							if(prefix != null) {
								return TopLevelPropertiesPrefixer.PatchAliasPrefix(prefix.ToString(), expanded);
							}
							return expanded;
						}
					}
				}
				return theOperand;
			} finally {
				if(localStateStack != null) {
					while(localStateStack.Count > 0) {
						stateStack.Push(currentState);
						currentState = localStateStack.Pop();
					}
				}
			}
		}
		protected override CriteriaOperator Visit(AggregateOperand theOperand, bool processCollectionProperty) {
			if(theOperand.IsTopLevel) {
				CriteriaOperator aggregatedExpression = Process(theOperand.AggregatedExpression);
				CriteriaOperator condition = Process(theOperand.Condition);
				return ReferenceEquals(aggregatedExpression, theOperand.AggregatedExpression)
					&& ReferenceEquals(condition, theOperand.Condition) ? theOperand : new AggregateOperand(null, aggregatedExpression, theOperand.AggregateType, condition);
			}
			Stack<XPClassInfo> localStateStack = null;
			string propertyName = theOperand.CollectionProperty.PropertyName;
			while(propertyName.StartsWith("^.")) {
				if(localStateStack == null) {
					localStateStack = new Stack<XPClassInfo>();
				}
				if(stateStack.Count == 0) throw new InvalidOperationException("^.");
				propertyName = propertyName.Substring(2);
				localStateStack.Push(currentState);
				currentState = stateStack.Pop();
			}
			try {
				MemberInfoCollection mic = currentState.ParsePath(propertyName);
				if(mic.Count == 0) throw new InvalidPropertyPathException(Res.GetString(Res.MetaData_IncorrectPath, currentState.FullName, propertyName));
				for(int i = 0; i < (mic.Count - 1); i++) {
					XPMemberInfo mi = mic[i];
					stateStack.Push(currentState);
					currentState = mi.ReferenceType == null ? mi.CollectionElementType : mi.ReferenceType;
				}
				XPMemberInfo collectionMi = mic[mic.Count - 1];
				if(!collectionMi.IsAssociationList && !collectionMi.IsManyToManyAlias) throw new InvalidPropertyPathException(Res.GetString(Res.MetaData_IncorrectPath, currentState.FullName, propertyName));
				stateStack.Push(currentState);
				currentState = collectionMi.CollectionElementType;
				try {
					CriteriaOperator aggregatedExpression = Process(theOperand.AggregatedExpression);
					CriteriaOperator condition = Process(theOperand.Condition);
					return ReferenceEquals(aggregatedExpression, theOperand.AggregatedExpression) 
						&& ReferenceEquals(condition, theOperand.Condition) ? theOperand : new AggregateOperand(theOperand.CollectionProperty, aggregatedExpression, theOperand.AggregateType, condition);
				} finally {
					for(int i = 0; i < mic.Count && stateStack.Count > 0; i++)
						currentState = stateStack.Pop();
				}
			} finally {
				if(localStateStack != null) {
					while(localStateStack.Count > 0) {
						stateStack.Push(currentState);
						currentState = localStateStack.Pop();
					}
				}
			}
		}
	}
}
