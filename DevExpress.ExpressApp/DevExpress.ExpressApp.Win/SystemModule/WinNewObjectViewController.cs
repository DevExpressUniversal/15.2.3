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
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class WinNewObjectViewController : NewObjectViewController {
		private bool NeedAddRootObjectItems() {
			if(Frame != null) {
				return Frame.Context == TemplateContext.ApplicationWindow || Frame.Context == TemplateContext.View;
			}
			return View.IsRoot;
		}
		protected override void New(SingleChoiceActionExecuteEventArgs args) {
			WinWindow window = Frame as WinWindow;
			if((window != null) && window.Form.Modal) {
				if(((ObjectView)View).CanChangeCurrentObject()) {
					base.New(args);
				}
			}
			else {
				base.New(args);
			}
		}
		protected override bool IsViewShownFromNestedObjectSpace(Frame sourceFrame) {
			ListView listView = (sourceFrame != null) ? sourceFrame.View as ListView : null;
			DetailView detailView = (sourceFrame != null) ? sourceFrame.View as DetailView : null;
			if(listView != null
				&& (listView.CollectionSource is PropertyCollectionSource)
				&& ((PropertyCollectionSource)listView.CollectionSource).MemberInfo.IsAggregated
				&& (listView.EditView == null)) {
				return true;
			}
			if((detailView != null) && (detailView.ObjectSpace is INestedObjectSpace)) {
				return true;
			}
			return false;
		}
		protected override void UpdateActionState() {
			base.UpdateActionState();
			NewObjectAction.BeginUpdate();
			try {
				NewObjectAction.Items.Clear();
				NewObjectAction.Items.AddRange(CollectDescendantItems());
				if(NeedAddRootObjectItems()) {
					IList<ChoiceActionItem> rootObjectItems = CollectRootObjectItems();
					if(rootObjectItems.Count > 0) {
						rootObjectItems[0].BeginGroup = NewObjectAction.Items.Count > 0;
					}
					NewObjectAction.Items.AddRange(rootObjectItems);
				}
				if(NewObjectAction.Items.Count > 0) {
					NewObjectAction.SelectedItem = NewObjectAction.Items.FirstActiveItem;
				}
			}
			finally {
				NewObjectAction.EndUpdate();
			}
		}
	}
}
