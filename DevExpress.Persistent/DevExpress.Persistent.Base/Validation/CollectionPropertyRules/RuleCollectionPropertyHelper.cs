#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Localization;
using System.Collections;
using System.ComponentModel;
namespace DevExpress.Persistent.Validation {
	public static class RuleCollectionPropertyTargetCriteriaHelper {
		public static IMemberInfo GetCollectionProperty(IRuleCollectionPropertyProperties ruleProperties) {
			return XafTypesInfo.Instance.FindTypeInfo(ruleProperties.TargetCollectionOwnerType).FindMember(ruleProperties.TargetCollectionPropertyName);
		}
		public static IMemberInfo GetAssociatedMember(IRuleCollectionPropertyProperties ruleProperties) {
			Guard.ArgumentNotNull(ruleProperties, "ruleProperties");
			IMemberInfo collectionProperty = GetCollectionProperty(ruleProperties);
			return collectionProperty.AssociatedMemberInfo;
		}
		public static object GetCollectionMemberValue(IMemberInfo collectionPropertyInfo, object target) {
			object result = null;
			if(collectionPropertyInfo != null && target != null) {
				if(collectionPropertyInfo.AssociatedMemberInfo != null) {
					IMemberInfo memberInfo = XafTypesInfo.Instance.FindTypeInfo(target.GetType()).FindMember(collectionPropertyInfo.AssociatedMemberInfo.Name);
					if(memberInfo != null) {
						result = memberInfo.GetValue(target);
					}
				}
				else {
					string message = SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ValidationCannotFindTheAssociatedMemberInTargetClassForCollectionProperty,
						collectionPropertyInfo.Name, collectionPropertyInfo.Owner.Type, XafTypesInfo.Instance.FindTypeInfo(target.GetType()).Type);
					throw new InvalidOperationException(message);
				}
			}
			return result;
		}
		#region Obsolete 15.1
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static CriteriaOperator GetTargetCollectionCriteriaOperator(IRuleCollectionPropertyProperties ruleProperties, object target) {
			Guard.ArgumentNotNull(ruleProperties, "ruleProperties");
			Guard.ArgumentNotNull(target, "target");
			if(!ruleProperties.TargetType.IsAssignableFrom(target.GetType())) {
				throw new InvalidCastException("Unable to cast target object to rule's TargetType");
			}
			CriteriaOperator criteria = null;
			IMemberInfo associatedMember = GetAssociatedMember(ruleProperties);
			if(associatedMember != null) {
				criteria = new NotOperator(new NullOperator(associatedMember.Name));
				object owner = associatedMember.GetValue(target);
				if(owner != null) {
					IMemberInfo collectionProperty = GetCollectionProperty(ruleProperties);
					IMemberInfo keyProperty = collectionProperty.Owner.KeyMember;
					if(ruleProperties.TargetCollectionOwnerType != null && !string.IsNullOrEmpty(ruleProperties.TargetCollectionPropertyName)) {
						ITypeInfo ownerTypeInfo = XafTypesInfo.Instance.FindTypeInfo(ruleProperties.TargetCollectionOwnerType);
						if(ownerTypeInfo.FindMember(ruleProperties.TargetCollectionPropertyName).IsManyToMany) {
							return new ContainsOperator(ownerTypeInfo.FindMember(ruleProperties.TargetCollectionPropertyName).AssociatedMemberInfo.Name, new ContainsOperator(ruleProperties.TargetCollectionPropertyName, new BinaryOperator("This", target)));
						}
					}
					string collectionCriteria = string.Format("[{0}.{1}] == '{2}'", associatedMember.Name, keyProperty.Name, keyProperty.GetValue(owner));
					if(!string.IsNullOrEmpty(collectionCriteria.ToString())) {
						criteria = GroupOperator.And(criteria, CriteriaOperator.Parse(collectionCriteria));
					}
				}
			}
			else {
				string message = SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ValidationCannotFindTheAssociatedMemberInTargetClassForCollectionProperty,
					ruleProperties.TargetCollectionPropertyName, ruleProperties.TargetCollectionOwnerType, XafTypesInfo.Instance.FindTypeInfo(target.GetType()).Type);
				throw new InvalidOperationException(message);
			}
			return criteria;
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTargetCollectionCriteria(IRuleCollectionPropertyProperties ruleProperties, object target) {
			return GetTargetCollectionCriteriaOperator(ruleProperties, target).ToString();
		}
		#endregion
	}
}
