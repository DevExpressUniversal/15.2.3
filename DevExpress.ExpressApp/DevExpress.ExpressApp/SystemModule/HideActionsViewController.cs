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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Updating;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using System.Collections;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp.SystemModule {
	public class HideActionsViewController : ViewController, IModelExtender {
		private void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
			if(Frame.View == null)
				return;
			HideActions();
		}
		protected override void OnActivated() {
			base.OnActivated();
			Frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			Frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
		}
		protected virtual void HideActions() {
			IModelViewHiddenActions hiddenActions = Frame.View.Model as IModelViewHiddenActions;
			if(hiddenActions != null && hiddenActions.HiddenActions != null) {
				string key = GetType().Name;
				if (hiddenActions.HiddenActions.Count > 0) {
					foreach (Controller controller in Frame.Controllers) {
						foreach (ActionBase action in controller.Actions) {
							IModelActionLink modelAction = hiddenActions.HiddenActions[action.Id];
							if (modelAction == null) {
								action.Active.RemoveItem(key);
							} else {
								action.Active.SetItemValue(key, false);
							}
						}
					}
				} else {
					foreach (Controller controller in Frame.Controllers) {
						foreach (ActionBase action in controller.Actions) {
							action.Active.RemoveItem(key);
						}
					}
				}
			}
		}
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelView, IModelViewHiddenActions>();
		}
		#endregion
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("SystemModuleIModelHiddenActions")]
#endif
	public interface IModelHiddenActions : IModelNode, IModelList<IModelActionLink> { }
	public interface IModelViewHiddenActions {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelViewHiddenActionsHiddenActions")]
#endif
		IModelHiddenActions HiddenActions { get; }
	}
}
