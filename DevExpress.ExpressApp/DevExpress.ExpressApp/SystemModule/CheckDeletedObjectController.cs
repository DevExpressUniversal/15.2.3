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
using DevExpress.ExpressApp.Localization;
namespace DevExpress.ExpressApp.SystemModule {
	public class CheckDeletedObjectController : ViewController {
		private const String EditDeletedObjectKey = "EditDeletedObject";
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			CheckObjectDeleted();
		}
		private void windowTemplateController_CustomizeWindowCaption(object sender, CustomizeWindowCaptionEventArgs e) {
			if(IsCurrentObjectDeleted()) {
				e.WindowCaption.SecondPart = UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.RequestedObjectHasBeenDeleted);
			}
		}
		protected Boolean IsCurrentObjectDeleted() {
			Object currentObject = ((DetailView)View).CurrentObject;
			return 
				((currentObject != null)
				&& !ObjectSpace.IsDisposedObject(currentObject) 
				&& ObjectSpace.IsDeletedObject(currentObject));
		}
		protected virtual void CheckObjectDeleted() {
			if(IsCurrentObjectDeleted()) {
				View.AllowEdit[EditDeletedObjectKey] = false;
				View.AllowDelete[EditDeletedObjectKey] = false;
			}
			else {
				View.AllowEdit.RemoveItem(EditDeletedObjectKey);
				View.AllowDelete.RemoveItem(EditDeletedObjectKey);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			((DetailView)View).CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			WindowTemplateController windowTemplateController = Frame.GetController<WindowTemplateController>();
			if(windowTemplateController != null) {
				windowTemplateController.CustomizeWindowCaption += new EventHandler<CustomizeWindowCaptionEventArgs>(windowTemplateController_CustomizeWindowCaption);
			}
			CheckObjectDeleted();
		}
		protected override void OnDeactivated() {
			WindowTemplateController windowTemplateController = Frame.GetController<WindowTemplateController>();
			if(windowTemplateController != null) {
				windowTemplateController.CustomizeWindowCaption -= new EventHandler<CustomizeWindowCaptionEventArgs>(windowTemplateController_CustomizeWindowCaption);
			}
			((DetailView)View).CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			base.OnDeactivated();
		}
		public CheckDeletedObjectController() {
			TypeOfView = typeof(DetailView);
			TargetViewNesting = Nesting.Root;
		}
	}
}
