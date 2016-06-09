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
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.Ribbon;
using DevExpress.ExpressApp.Win.Templates.Ribbon.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Utils;
using DevExpress.Persistent.Base;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class RibbonFillActionContainersController : WindowController {
		private ActionControlsSiteController actionControlsSiteController;
		private ActionCollection CollectActions(IEnumerable<Controller> controllers) {
			ActionCollection actions = new ActionCollection();
			foreach(Controller controller in controllers) {
				actions.AddRange(controller.Actions);
			}
			return actions;
		}
		private void FillQuickAccess(RibbonForm ribbonForm) {
			IActionControlsSite actionControlsSite = Window.Template as IActionControlsSite;
			if(actionControlsSite != null && ribbonForm != null && ribbonForm.Ribbon is XafRibbonControlV2 && ribbonForm.Ribbon.Toolbar != null) {
				QuickAccessHelper helper = new QuickAccessHelper((XafRibbonControlV2)ribbonForm.Ribbon, actionControlsSite, CollectActions(Frame.Controllers));
				helper.FillToolbar();
				helper.SetRibbonGroupLinkHints();
			}
		}
		private void LocalizeByModel(RibbonForm ribbonForm) {
			if(ribbonForm != null && ribbonForm.Ribbon != null) {
				RibbonLocalizationHelper localizationHelper = new RibbonLocalizationHelper(ribbonForm.Ribbon);
				localizationHelper.LocalizeByModel();
			}
		}
		private void Form_Load(object sender, EventArgs e) {
			if(!(Window.Template is XtraFormTemplateBase)) {
				RibbonForm ribbonForm = Window.Template as RibbonForm;
				FillQuickAccess(ribbonForm);
				LocalizeByModel(ribbonForm);
			}
		}
		private void Window_TemplateChanging(object sender, EventArgs e) {
			if(Window.Template is Form) {
				((Form)Window.Template).Load -= Form_Load;
			}
		}
		private void Window_TemplateChanged(object sender, EventArgs e) {
			if(Window.Template is Form) {
				((Form)Window.Template).Load += Form_Load;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			Window.TemplateChanging += Window_TemplateChanging;
			Window.TemplateChanged += Window_TemplateChanged;
			actionControlsSiteController = Frame.GetController<ActionControlsSiteController>();
			if(actionControlsSiteController != null) {
				actionControlsSiteController.CustomizeContainerActions += actionControlsSiteController_CustomizeContainerActions;
			}
		}
		protected override void OnDeactivated() {
			Window.TemplateChanging -= Window_TemplateChanging;
			Window.TemplateChanged -= Window_TemplateChanged;
			if(actionControlsSiteController != null) {
				actionControlsSiteController.CustomizeContainerActions -= actionControlsSiteController_CustomizeContainerActions;
			}
			if(Window.Template is Form) {
				((Form)Window.Template).Load -= Form_Load;
			}
			base.OnDeactivated();
		}
		#region Obsolete 15.1
		private static readonly string editCategoryId = PredefinedCategory.Edit.ToString();
		private static readonly string undoRedoCategoryId = PredefinedCategory.UndoRedo.ToString();
		private static readonly string viewCategoryId = PredefinedCategory.View.ToString();
		private static readonly string reportsCategoryId = PredefinedCategory.Reports.ToString();
		private void FillActionsList(IList<IModelActionLink> sourceCategoryActionLinks, ActionCollection sourceActions, ICollection<ActionBase> actions) {
			if(sourceCategoryActionLinks == null) return;
			foreach(IModelActionLink actionLink in sourceCategoryActionLinks) {
				ActionBase action = sourceActions.Find(actionLink.ActionId);
				if(action != null && !actions.Contains(action)) {
					actions.Add(action);
				}
			}
		}
		private void actionControlsSiteController_CustomizeContainerActions(object sender, CustomizeContainerActionsEventArgs e) {
			if(!(Window.Template is XtraFormTemplateBase) && Frame.Template is RibbonForm && e.Container is RibbonGroupActionControlContainer) {
				if(e.Category == editCategoryId) {
					FillActionsList(e.ActionToContainerMapping[undoRedoCategoryId], e.AllActions, e.ContainerActions);
				}
				if(e.Category == viewCategoryId) {
					FillActionsList(e.ActionToContainerMapping[reportsCategoryId], e.AllActions, e.ContainerActions);
				}
			}
		}
		#endregion
	}
}
