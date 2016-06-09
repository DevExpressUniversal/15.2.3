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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Validation;
using System.Collections.ObjectModel;
namespace DevExpress.ExpressApp.Validation.AllContextsView {
	public class CheckContextsAvailableController : WindowController {
		private void UpdateActionRecursiveFromRoot() {
			UpdateActionRecursive(Frame, Validator.RuleSet.GetRules());
		}
		private bool IsRuleFitsTargetObjectType(IRule rule, Type targetObjectType) {
			if(rule.Properties.TargetType.IsAssignableFrom(targetObjectType)) {
				return true;
			}
			IRuleCollectionPropertyProperties collectionProperties = rule.Properties as IRuleCollectionPropertyProperties;
			if(collectionProperties != null && collectionProperties.TargetCollectionOwnerType != null && collectionProperties.TargetCollectionOwnerType.IsAssignableFrom(targetObjectType)) {
				return true;
			}
			return false;
		}
		private void UpdateActionRecursive(Frame frame, ReadOnlyCollection<IRule> rules) {
			if(frame == null || frame.View == null) {
				return;
			}
			bool isRuleFound = false;
			if(frame.View is ObjectView) {
				foreach(IRule rule in rules) {
					if(IsRuleFitsTargetObjectType(rule, ((ObjectView)frame.View).ObjectTypeInfo.Type)) {
						isRuleFound = true;
						break;
					}
				}
			}
			frame.GetController<ShowAllContextsController>().Action.Active.SetItemValue("Contexts presented", isRuleFound);
			DetailView detailView = frame.View as DetailView;
			if(detailView != null) {
				foreach(IFrameContainer frameContainer in detailView.GetItems<IFrameContainer>()) {
					UpdateActionRecursive(frameContainer.Frame, rules);
				}
			}
		}
		private void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
			UpdateActionRecursiveFromRoot();
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Frame.View != null) {
				UpdateActionRecursiveFromRoot();
			}
			Frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			Frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
		}
	}
}
