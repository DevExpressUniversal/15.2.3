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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Validation {
	public abstract class InplaceEditorsValidationControllerBase : ViewController<ObjectView> {
		protected static string GetErrorImageName(ValidationResultType validationResultType) {
			switch(validationResultType) {
				case ValidationResultType.Error: return "Error";
				case ValidationResultType.Warning: return "Warning";
				case ValidationResultType.Information: return "Information";
			}
			return string.Empty;
		}
		protected static ImageInfo GetErrorImageInfo(ValidationResultType validationResultType) {
			string imageName = GetErrorImageName(validationResultType);
			return ImageLoader.Instance.GetImageInfo(imageName);
		}		
		protected override void OnActivated() {
			base.OnActivated();
			if(View is DetailView) {
				DetailView detailView = (DetailView)View;
				detailView.LayoutManager.MarkRequiredFieldCaption += LayoutManager_MarkRequiredFieldCaption;
				foreach(PropertyEditor propertyEditor in detailView.GetItems<PropertyEditor>()) {					
					propertyEditor.ControlCreated += PropertyEditor_ControlCreated;
				}
			}			
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(View is DetailView) {
				DetailView detailView = (DetailView)View;
				detailView.LayoutManager.MarkRequiredFieldCaption -= LayoutManager_MarkRequiredFieldCaption;
				foreach(PropertyEditor propertyEditor in detailView.GetItems<PropertyEditor>()) {
					propertyEditor.ControlCreated -= PropertyEditor_ControlCreated;
				}
			}
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			if(View is ListView) { 
				ListView listView = (ListView)View;
				if(listView.Editor != null) {
					ApplyListEditorValidationSettings(listView.Editor);
				}
			}
		}
		protected void PropertyEditor_ControlCreated(object sender, EventArgs e) {
			PropertyEditor propertyEditor = (PropertyEditor)sender;						
			IList<IRule> rulesList = GetRuleListForMember(View.ObjectTypeInfo.Type, propertyEditor.PropertyName, true);
			if(rulesList != null) {
				ApplyPropertyEditorValidationSettings(propertyEditor, rulesList);
			}
		}
		private void LayoutManager_MarkRequiredFieldCaption(object sender, Layout.MarkRequiredFieldCaptionEventArgs e) {
			if(e.ViewItem is PropertyEditor) {
				PropertyEditor propertyEditor = (PropertyEditor)e.ViewItem;
				IList<IRule> rulesList = GetRuleListForMember(View.ObjectTypeInfo.Type, propertyEditor.PropertyName);
				if(rulesList != null && rulesList.FirstOrDefault(s => typeof(RuleRequiredField).IsAssignableFrom(s.GetType())) != null && !propertyEditor.MemberInfo.IsReadOnly) {
					IModelLayoutManagerOptionsValidation layoutManagerOptions = Application.Model.Options.LayoutManagerOptions as IModelLayoutManagerOptionsValidation;
					if(layoutManagerOptions != null) {
						e.NeedMarkRequiredField = true;
						e.RequiredFieldMark = layoutManagerOptions.RequiredFieldMark;
					}
				}
			}
		}
		private bool IsSimpleMemberType(Type objectType, string propertyName) {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(objectType);
			if(typeInfo != null) {
				IMemberInfo memberInfo = typeInfo.FindMember(propertyName);
				if(memberInfo != null) {
					return SimpleTypes.IsSimpleType(memberInfo.MemberType) && !typeof(System.Drawing.Image).IsAssignableFrom(memberInfo.MemberType);
				}
			}
			return false;
		}
		protected IList<IRule> GetRuleListForMember(Type objectType, string propertyName, bool onlyForSimpleTypes) {
			if(onlyForSimpleTypes && !IsSimpleMemberType(objectType, propertyName)) {
				return null;
			}
			else {
				return GetRuleListForMember(objectType, propertyName);
			}
		}
		protected IList<IRule> GetRuleListForMember(Type objectType, string propertyName) {
			IList<string> contexts = ((IModelApplicationValidation)Application.Model).Validation.Contexts.Where(s => s.AllowInplaceValidation).Select(s => s.Id).ToList<string>();
			IList<IRule> rules = Validator.RuleSet.GetRules(objectType, new ContextIdentifiers(contexts)).Where(s => s.UsedProperties.Contains(propertyName) && string.IsNullOrEmpty(s.Properties.TargetCriteria)).ToList<IRule>();
			return rules;
		}
		protected abstract void ApplyPropertyEditorValidationSettings(PropertyEditor propertyEditor, IList<IRule> rulesList);
		protected abstract void ApplyListEditorValidationSettings(ListEditor listEditor);
	}
}
