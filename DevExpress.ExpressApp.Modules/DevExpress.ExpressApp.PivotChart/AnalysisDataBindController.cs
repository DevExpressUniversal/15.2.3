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
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.PivotChart {
	public class AnalysisDataBindController : AnalysisViewControllerBase {
		private SimpleAction bindDataAction;
		private SimpleAction unbindDataAction;
		private void bindDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			BindDataToControl();
		}
		private void unbindDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			UnbindDataFromControl();
		}
		private void analysisEditor_IsDataSourceReadyChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		protected virtual void SubscribeToEvents() {
			if(analysisEditor != null) {
				analysisEditor.IsDataSourceReadyChanged += new EventHandler<EventArgs>(analysisEditor_IsDataSourceReadyChanged);
			}
		}
		protected virtual void UnsubscribeFromEvents() {
			if(analysisEditor != null) {
				analysisEditor.IsDataSourceReadyChanged -= new EventHandler<EventArgs>(analysisEditor_IsDataSourceReadyChanged);
			}
		}
		protected virtual void BindDataToControl() {
			if(analysisEditor != null) {
				analysisEditor.IsDataSourceReady = true;
				UpdateBindUnbindActionsState();
			}
		}
		protected virtual void UnbindDataFromControl() {
			if(analysisEditor != null) {
				analysisEditor.IsDataSourceReady = false;
				UpdateBindUnbindActionsState();
			}
		}
		protected virtual void UpdateBindUnbindActionsState() {
			if(analysisEditor != null) {
				bindDataAction.Active["IsDataSourceReady"] = !analysisEditor.IsDataSourceReady;
				unbindDataAction.Active["IsDataSourceReady"] = analysisEditor.IsDataSourceReady;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				bindDataAction.Execute -= new SimpleActionExecuteEventHandler(bindDataAction_Execute);
				unbindDataAction.Execute -= new SimpleActionExecuteEventHandler(bindDataAction_Execute);
			}
			base.Dispose(disposing);
		}
		public AnalysisDataBindController() {
			bindDataAction = new SimpleAction(this, "BindAnalysisData", PredefinedCategory.Edit);
			bindDataAction.Caption = "Bind Analysis Data";
			bindDataAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			bindDataAction.Execute += new SimpleActionExecuteEventHandler(bindDataAction_Execute);
			bindDataAction.ImageName = "MenuBar_BindAnalysisData";
			bindDataAction.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage;
			unbindDataAction = new SimpleAction(this, "UnbindAnalysisData", PredefinedCategory.Edit);
			unbindDataAction.Caption = "Unbind Analysis Data";
			unbindDataAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			unbindDataAction.Execute += new SimpleActionExecuteEventHandler(unbindDataAction_Execute);
			unbindDataAction.ImageName = "MenuBar_UnbindAnalysisData";
			unbindDataAction.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage;
		}
		protected override void OnActivated() {
			base.OnActivated();
			SubscribeToEvents();
			if (analysisEditor != null) {
				analysisEditor.IsDataSourceReady = false;
			}
			UpdateBindUnbindActionsState();
		}
		protected override void OnDeactivated() {
			UnsubscribeFromEvents();
			base.OnDeactivated();
		}
		public SimpleAction BindDataAction {
			get { return bindDataAction; }
		}
		public SimpleAction UnbindDataAction {
			get { return unbindDataAction; }
		}
	}
}
