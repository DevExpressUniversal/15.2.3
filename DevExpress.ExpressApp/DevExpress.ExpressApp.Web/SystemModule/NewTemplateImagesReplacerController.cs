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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Web;
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Web.SystemModule {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class NewTemplateImagesReplacerController : ViewController {
		private static Dictionary<string, ActionItemPaintStyle> actionPaintStyles = new Dictionary<string, ActionItemPaintStyle>() { 
			{"Save", ActionItemPaintStyle.Caption},
			{"SaveAndClose", ActionItemPaintStyle.Caption},
			{"SaveAndNew", ActionItemPaintStyle.Caption},
			{"Link", ActionItemPaintStyle.Caption},
			{"Unlink", ActionItemPaintStyle.Caption},
			{"Logoff", ActionItemPaintStyle.Caption},
			{"Export", ActionItemPaintStyle.Caption},
			{"New", ActionItemPaintStyle.Caption},
			{"MyDetails", ActionItemPaintStyle.Caption},
		};
		public NewTemplateImagesReplacerController() {
			Active.SetItemValue("NewTemplate", TemplateContentFactory.Instance.NewStyle);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Frame != null) {
				ChangeActionsPaintStyle();
				ChangeActionsImages();
			}
		}
		private void ChangeActionsPaintStyle() {
			foreach(Controller controller in Frame.Controllers) {
				foreach(ActionBase action in controller.Actions) {
					ActionItemPaintStyle paintStyle;
					if(actionPaintStyles.TryGetValue(action.Id, out paintStyle)) {
						ChangeActionsPaintStyle(action, paintStyle);
					}
				}
			}
		}
		private void ChangeActionsImages() {
			FilterController filterController = Frame.GetController<FilterController>();
			if(filterController != null) {
				ChangeActionImage(filterController.FullTextFilterAction, "Action_Search");
			}
		}
		protected virtual void ChangeActionImage(ActionBase action, string newImageName) {
			action.ImageName = "Action_Search";
		}
		protected virtual void ChangeActionsPaintStyle(ActionBase action, ActionItemPaintStyle paintStyle) {
			action.PaintStyle = paintStyle;
		}
	}
}
