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
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class CloseAfterSaveInitializer {
		private Frame frame;
		private XafApplication application;
		private void UnsubscribeFromWebModificationsController(WebModificationsController modificationsController) {
			modificationsController.QueryCloseAfterSave -= new EventHandler<QueryCloseAfterSaveEventArgs>(modificationsController_QueryCloseAfterSave);
			modificationsController.Deactivated -= new EventHandler(modificationsController_Deactivated);
			modificationsController.Disposed -= new EventHandler(modificationsController_Disposed);
		}
		private void modificationsController_Deactivated(Object sender, EventArgs e) {
			UnsubscribeFromWebModificationsController((WebModificationsController)sender);
		}
		private void modificationsController_Disposed(Object sender, EventArgs e) {
			UnsubscribeFromWebModificationsController((WebModificationsController)sender);
		}
		private void modificationsController_QueryCloseAfterSave(Object sender, QueryCloseAfterSaveEventArgs args) {
			args.CloseAfterSave = true;
		}
		private void Application_ViewShown(Object sender, ViewShownEventArgs e) {
			if((application != null) && (e.TargetFrame == frame)) {
				WebModificationsController modificationsController = e.TargetFrame.GetController<WebModificationsController>();
				if(modificationsController != null) {
					modificationsController.QueryCloseAfterSave += new EventHandler<QueryCloseAfterSaveEventArgs>(modificationsController_QueryCloseAfterSave);
					modificationsController.Deactivated += new EventHandler(modificationsController_Deactivated);
					modificationsController.Disposed += new EventHandler(modificationsController_Disposed);
				}
				application.ViewShown -= new EventHandler<ViewShownEventArgs>(Application_ViewShown);
				application = null;
				frame = null;
			}
		}
		public CloseAfterSaveInitializer(XafApplication application, Frame frame) {
			this.frame = frame;
			this.application = application;
			this.application.ViewShown += new EventHandler<ViewShownEventArgs>(Application_ViewShown);
		}
	}
	public class CloseAfterSaveController : ViewController {
		private void Application_ViewShowing(Object sender, ViewShowingEventArgs e) {
			if(
				(e.SourceFrame == Frame)
				&&
				(e.TargetFrame is Window)
				&&
				(View is ObjectView)
				&&
				(e.View is ObjectView)
				&&
				((ObjectView)View).ObjectTypeInfo.IsAssignableFrom(((ObjectView)e.View).ObjectTypeInfo)) {
				new CloseAfterSaveInitializer(Application, e.TargetFrame);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			Application.ViewShowing += new EventHandler<ViewShowingEventArgs>(Application_ViewShowing);
		}
		protected override void OnDeactivated() {
			Application.ViewShowing -= new EventHandler<ViewShowingEventArgs>(Application_ViewShowing);
			base.OnDeactivated();
		}
		public CloseAfterSaveController() {
			TargetViewNesting = Nesting.Nested;
		}
	}
}
