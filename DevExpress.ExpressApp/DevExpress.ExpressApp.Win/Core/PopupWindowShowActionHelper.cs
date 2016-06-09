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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Win {
	public class PopupWindowShowActionHelper : IDisposable {
		private PopupWindowShowAction action;
		private bool isSizable = true;
		private void action_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
			if(e.Context == TemplateContext.PopupWindow) {
				IWindowTemplate template = (IWindowTemplate)e.Template;
				template.IsSizeable = isSizable;
			}
		}
		public PopupWindowShowActionHelper(PopupWindowShowAction action) {
			Guard.ArgumentNotNull(action, "action");
			this.action = action;
		}
		public void ShowPopupWindow() {
			ShowPopupWindow(true);
		}
		public void ShowPopupWindow(bool createAllControllers) {
			CustomizePopupWindowParamsEventArgs args = action.GetPopupWindowParams();
			if(args.View == null) {
				throw new ArgumentNullException("args.View");
			}
			isSizable = args.IsSizeable;
			ShowViewParameters newShowViewParameters = new ShowViewParameters(args.View);
			newShowViewParameters.Context = ((WinApplication)action.Application).CalculateContext(args.Context, args.View.Id);
			newShowViewParameters.Controllers.Add(args.DialogController);
			if(action.IsModal) {
				newShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
			}
			newShowViewParameters.CreateAllControllers = createAllControllers;
			action.CustomizeTemplate += action_CustomizeTemplate;
			action.Application.ShowViewStrategy.ShowView(newShowViewParameters, new ShowViewSource(null, null));
		}
		public void Dispose() {
			action.CustomizeTemplate -= action_CustomizeTemplate;
		}
		#region Obsolete 10.2
		[Obsolete("Use the ShowPopupWindow method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public WinWindow CreatePopupWindow(bool createAllControllers) {
			CustomizePopupWindowParamsEventArgs args = action.GetPopupWindowParams();
			if(args.View == null) {
				throw new ArgumentNullException("args.View");
			}
			List<Controller> controllers = new List<Controller>(args.DialogController.Controllers);
			controllers.Add(args.DialogController);
			WinWindow result = (WinWindow)action.Application.CreatePopupWindow(args.Context, args.View.Id, createAllControllers, controllers.ToArray());
			result.SetView(args.View, null);
			result.Template.IsSizeable = args.IsSizeable;
			if(result.Template is ISupportStoreSettings) {
				((ISupportStoreSettings)result.Template).SetSettings(action.Application.GetTemplateCustomizationModel(result.Template));
			}
			action.OnCustomizeTemplate(result.Template, result.Context);
			return result;
		}
		#endregion
	}
}
