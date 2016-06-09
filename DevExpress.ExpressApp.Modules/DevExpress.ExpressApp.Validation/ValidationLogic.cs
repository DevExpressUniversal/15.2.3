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
using System.Diagnostics.CodeAnalysis;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Validation {
	[DomainLogic(typeof(IRuleBaseProperties))]
	public static class RuleBasePropertiesLogic {
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static void BeforeSet_TargetType(IRuleBaseProperties self, object value) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(value, "value");
		}
		public static string Get_Name(IRuleBaseProperties self) {
			return self.Id;
		}
	}
	[DomainLogic(typeof(IRuleRangeProperties))]
	public static class RuleRangePropertiesLogic {
		public static void BeforeSet_MinimumValueExpression(IRuleRangeProperties self, object value) {
			self.MinimumValue = value;
		}
		public static void BeforeSet_MaximumValueExpression(IRuleRangeProperties self, object value) {
			self.MaximumValue = value;
		}
	}
	[DomainLogic(typeof(IRuleValueComparisonProperties))]
	public static class RuleValueComparisonPropertiesLogic {
		public static void BeforeSet_RightOperandExpression(IRuleValueComparisonProperties self, object value) {
			self.RightOperand = value;
		}
	}
	[DomainLogic(typeof(IRuleCollectionPropertyProperties))]
	public static class RuleCollectionPropertyPropertiesLogic {
		public static void BeforeSet_TargetCollectionPropertyName(IRuleCollectionPropertyProperties self, object value) {
			if(!string.IsNullOrEmpty((string)value)) {
				if(self.TargetCollectionOwnerType == null) {
					throw new InvalidOperationException("TargetCollectionOwnerType must be assigned prior to TargetCollectionPropertyName");
				}
				ITypeInfo targetCollectionOwnerTypeInfo = XafTypesInfo.Instance.FindTypeInfo(self.TargetCollectionOwnerType);
				IMemberInfo collectionPropertyMemberInfo = targetCollectionOwnerTypeInfo.FindMember((string)value);
				if(collectionPropertyMemberInfo == null) {
					throw new MemberNotFoundException(self.TargetCollectionOwnerType, (string)value);
				}
				if(!collectionPropertyMemberInfo.IsList) {
					throw new InvalidOperationException(string.Format("The {0}.{1} must be a collection property.", self.TargetCollectionOwnerType, value));
				}
			}
		}
	}
	[DomainLogic(typeof(IRulePropertyValueProperties))]
	public static class RulePropertyValuePropertiesLogic {
		public static void BeforeSet_TargetPropertyName(IRulePropertyValueProperties self, object value) {
			if(string.IsNullOrEmpty((string)value)) {
				if(!(self is IRuleSupportsCollectionAggregatesProperties)) {
					throw new InvalidOperationException(string.Format("TargetPropertyName must have a value"));
				}
			}
			else {
				if(self.TargetType != null) {
					ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(self.TargetType);
					IMemberInfo targetMemberInfo = targetTypeInfo.FindMember((string)value);
					if(targetMemberInfo == null) {
						throw new MemberNotFoundException(self.TargetType, (string)value);
					}
				}
			}
		}
	}
	[DomainLogic(typeof(IRuleObjectExistsProperties))]
	public class RuleObjectExistsPropertiesLogic {
		public static Type Get_LooksFor(IRuleObjectExistsProperties self) {
			return self.TargetType;
		}
	}
}
