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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.TestScripts;
namespace DevExpress.ExpressApp.ScriptRecorder.Web {
	public class WebActionsListener : ScriptRecorderActionsListenerBase {
		public WebActionsListener(List<ActionBase> actions) : base(actions) { }
		public override string GetActionFullName(ActionBase action) {
			string result;
			IActionContainer container = GetActionContainer(action);
			if(container is ITestable) {
				result = ((ITestable)container).TestCaption;
			}
			else {
				result = base.GetActionFullName(action);
			}
			return result;
		}
		protected override void WriteComplexAction(ActionBase action, string value) {
			base.WriteComplexAction(action, GetActionValue(action, value));
		}
		private string GetActionValue(ActionBase action, string value) {
			string result = value;
			IActionContainer container = GetActionContainer(action);
			if(container is NavigationHistoryActionContainer) {
				result = ((NavigationHistoryActionContainer)container).GetVisibleLinkText(value);
			}
			return result;
		}
		private IActionContainer GetActionContainer(ActionBase action) {
			IFrameTemplate template = action.Controller.Frame.Template;
			if(template != null) {
				foreach(IActionContainer container in template.GetContainers()) {
					foreach(ActionBase act in container.Actions) {
						if(act == action) {
							return container;
						}
					}
				}
			}
			return null;
		}
	}
}
