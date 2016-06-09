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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class ModelValidator {
		private FastModelEditorHelper modelEditorHelper;
		private IObjectSpace objectSpace;
		private RuleSet CreateRuleSet(ModelNode node) {
			RuleSet result = new RuleSet();
			ITypeInfo nodeTypeInfo = XafTypesInfo.Instance.FindTypeInfo(node.GetType());
			result.RegisterRules(nodeTypeInfo);
			foreach(ITypeInfo item in nodeTypeInfo.ImplementedInterfaces) {
				string keyProperty = "";
				if(item.FindAttribute<KeyPropertyAttribute>() != null) {
					keyProperty = item.FindAttribute<KeyPropertyAttribute>().KeyPropertyName;
				}
				result.RegisterRules(item); 
				foreach(IMemberInfo memberInfo in item.OwnMembers) {
					Boolean isRequired = modelEditorHelper.IsRequired(node, memberInfo.Name);
					if(isRequired || (memberInfo.Name == keyProperty)) {
						result.RegisteredRules.Add(new RuleRequiredField(memberInfo.Name, memberInfo, DefaultContexts.Save));
					}
				}
			}
			for(int counter = result.RegisteredRules.Count - 1; counter >= 0; counter--) {
				IRule rule = result.RegisteredRules[counter];
				foreach(string property in rule.UsedProperties) {
					if(!modelEditorHelper.IsPropertyModelBrowsableVisible(node, property)) {
						result.RegisteredRules.Remove(rule);
						break;
					}
				}
			}
			return result;
		}
		public ModelValidator(FastModelEditorHelper modelEditorHelper) {
			this.modelEditorHelper = modelEditorHelper;
			objectSpace = new NonPersistentObjectSpace(XafTypesInfo.Instance);
		}
		public RuleSetValidationResult ValidateNode(ModelNode node) {
			if(node != null && (node.HasModification || node.IsNewNode)) { 
				RuleSet rules = CreateRuleSet(node);
				RuleSetValidationResult validationResult = rules.ValidateTarget(objectSpace, node, DefaultContexts.Save);
				return validationResult;
			}
			return null;
		}
	}
}
