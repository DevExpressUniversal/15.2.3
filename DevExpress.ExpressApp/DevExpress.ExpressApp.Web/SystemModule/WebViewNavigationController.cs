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
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class WebViewNavigationController : ViewNavigationController {
		private System.ComponentModel.IContainer components;
		private DevExpress.ExpressApp.Actions.SingleChoiceAction navigateToAction;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.navigateToAction = new SingleChoiceAction(this.components);
			this.navigateToAction.Caption = "Navigate To";
			this.navigateToAction.Category = "ViewsHistoryNavigation";
			this.navigateToAction.Id = "NavigateTo";
			this.navigateToAction.EmptyItemsBehavior = EmptyItemsBehavior.Deactivate;
			this.navigateToAction.Execute += new SingleChoiceActionExecuteEventHandler(navigateToAction_Execute);
		}
		private void navigationHistory_Changed(object sender, EventArgs e) {
			navigateToAction.BeginUpdate();
			navigateToAction.Items.Clear();
			foreach(HistoryItem<ViewShortcut> historyItem in navigationHistory) {
				ChoiceActionItem choiceItem = new ChoiceActionItem(historyItem.Item.GetHashCode().ToString(), historyItem.Caption, historyItem.Item);
				navigateToAction.Items.Add(choiceItem);
				if(historyItem.Item.Equals(navigationHistory.CurrentPosition.Item)) {
					navigateToAction.SelectedItem = choiceItem;
				}
			}
			navigateToAction.EndUpdate();
		}
		private void navigateToAction_Execute(object sender, SingleChoiceActionExecuteEventArgs args) {
			ViewShortcut navigateToShortcut = (ViewShortcut)args.SelectedChoiceActionItem.Data;
			NavigateTo(navigateToShortcut);
		}
#if DebugTest
		public override void RefreshActionsState() {
#else
		protected override void RefreshActionsState() {
#endif
			NavigateBackAction.Active.SetItemValue("WebNavigation", false);
			NavigateForwardAction.Active.SetItemValue("WebNavigation", false);
			navigateToAction.Active.SetItemValue("Main window", IsNavigationAllowed());
		}
		protected override NavigationHistory<ViewShortcut> CreateNavigationHistory() {
			return new NavigationHistory<ViewShortcut>(new BreadCrumbsNavigationHistorySequenceStrategy<ViewShortcut>(HistoryDepth));
		}
		public WebViewNavigationController() : base() {
			InitializeComponent();
			RegisterActions(components);
			navigationHistory.OnChanged += new EventHandler(navigationHistory_Changed);
		}
		public SingleChoiceAction NavigateToAction {
			get { return navigateToAction; }
		}
	}
}
