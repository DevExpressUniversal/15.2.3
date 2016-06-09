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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.ConditionalAppearance {
	public interface IModelConditionalAppearance {
		IModelAppearanceRules AppearanceRules { get; }
	}
	public class AppearanceRulesModelNodesGenerator : ModelNodesGeneratorBase {
		internal const BindingFlags MethodRuleBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		private IEnumerable<AppearanceAttribute> FindAttributes(MethodInfo methodInfo) {
			if(methodInfo != null) {
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if(methodInfo.ReturnType == typeof(bool) && !methodInfo.ContainsGenericParameters && parameters.Length == 0 ) {
					foreach(AppearanceAttribute attribute in methodInfo.GetCustomAttributes(typeof(AppearanceAttribute), false))
						yield return attribute;
				}
			}
		}
		private void AddAppearanceRulesFromAttributes(ModelNode node, IEnumerable<AppearanceAttribute> appearanceRulesAttributes) {
			AddAppearanceRulesFromAttributes(node, appearanceRulesAttributes, "", "");
		}
		private void AddAppearanceRulesFromAttributes(ModelNode node, IEnumerable<AppearanceAttribute> appearanceRulesAttributes, string propertyName, string methodName) {
			foreach(AppearanceAttribute appearanceAttribute in appearanceRulesAttributes) {
				IModelAppearanceRule appearanceRuleNode = node.AddNode<IModelAppearanceRule>();
				appearanceRuleNode.Attribute = appearanceAttribute;
				if(!string.IsNullOrEmpty(appearanceAttribute.Id)) {
					((ModelNode)appearanceRuleNode).Id = appearanceAttribute.Id;
				}
				if(!string.IsNullOrEmpty(propertyName)) {
					appearanceRuleNode.TargetItems = propertyName;
				}
				if(string.IsNullOrEmpty(appearanceRuleNode.TargetItems)) {
					throw new ArgumentException(string.Format("TargetItems property of the AppearanceAttribute with Id='{0}' is empty.", appearanceAttribute.Id));
				}
				if(!string.IsNullOrEmpty(methodName)) {
					appearanceRuleNode.Method = methodName;
				}
			}
		}
		protected override void GenerateNodesCore(ModelNode node) {
			IModelClass classModel = node.Parent as IModelClass;
			AddAppearanceRulesFromAttributes(node, classModel.TypeInfo.FindAttributes<AppearanceAttribute>(false));
			foreach(IModelMember ownMember in classModel.OwnMembers) {
				if(ownMember.MemberInfo != null) {
					AddAppearanceRulesFromAttributes(node, ownMember.MemberInfo.FindAttributes<AppearanceAttribute>(false), ownMember.Name, null);
				}
			}
			foreach(MethodInfo methodInfo in classModel.TypeInfo.Type.GetMethods(MethodRuleBindingFlags)) {
				AddAppearanceRulesFromAttributes(node, FindAttributes(methodInfo), null, methodInfo.Name);
			}
		}
	}
	[ModelNodesGenerator(typeof(AppearanceRulesModelNodesGenerator))]
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("ConditionalAppearanceIModelAppearanceRules")]
#endif
	public interface IModelAppearanceRules : IModelNode, IModelList<IModelAppearanceRule> {
	}
	[ModelInterfaceImplementor(typeof(IAppearanceRuleProperties), "Attribute")]
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("ConditionalAppearanceIModelAppearanceRule")]
#endif
	public interface IModelAppearanceRule : IModelNode, IAppearanceRuleProperties {
		[Browsable(false)]
		IAppearanceRuleProperties Attribute { get; set; }
		[Browsable(false)]
		IEnumerable<string> MethodNames { get; }
	}
	[DomainLogic(typeof(IModelAppearanceRule))]
	public static class ModelAppearanceRuleLogic {
		public static IEnumerable<string> Get_MethodNames(IModelAppearanceRule modelAppearanceRule) {
			if(modelAppearanceRule == null || modelAppearanceRule.Parent == null || !(modelAppearanceRule.Parent.Parent is IModelClass)) return null;
			List<string> validMethodNames = new List<string>();
			Type modelClassType = ((IModelClass)modelAppearanceRule.Parent.Parent).TypeInfo.Type;
			MethodInfo[] methods = modelClassType.GetMethods(AppearanceRulesModelNodesGenerator.MethodRuleBindingFlags);
			foreach(MethodInfo method in methods) {
				if(!method.IsSpecialName && method.GetParameters().Length == 0 && method.ReturnType == typeof(bool)) {
					validMethodNames.Add(method.Name);
				}
			}
			return validMethodNames;
		}
		public static Type Get_DeclaringType(IModelAppearanceRule modelAppearanceRule) {
			return ((IModelClass)modelAppearanceRule.Parent.Parent).TypeInfo.Type;
		}
		public static string Get_AppearanceItemType(IModelAppearanceRule modelAppearanceRule) {
			if(modelAppearanceRule.Attribute != null) {
				return modelAppearanceRule.Attribute.AppearanceItemType;
			}
			return AppearanceController.AppearanceViewItemType;
		}
		public static string Get_Context(IModelAppearanceRule modelAppearanceRule) {
			if(modelAppearanceRule.Attribute != null) {
				return modelAppearanceRule.Attribute.Context;
			}
			return AppearanceController.AppearanceContextAny;
		}
	}
}
