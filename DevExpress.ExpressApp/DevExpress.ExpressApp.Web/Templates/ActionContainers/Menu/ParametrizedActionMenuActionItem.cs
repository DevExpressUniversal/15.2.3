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
#if DebugTest
using DevExpress.ExpressApp.Tests;
#endif
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using System.Web.UI;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu {
	public class ParametrizedActionMenuActionItem : TemplatedMenuActionItem
#if DebugTest
, IParametrizedActionItemUnitTestable
#endif
 {
		private ActionContainerOrientation orientation;
		private bool isExecuted = false;
		private int executionLockCount;
		private void UpdateEditorValue() {
			executionLockCount++;
			try {
				if(Control != null) {
					Control.Value = ((ParametrizedAction)Action).Value;
				}
			}
			finally {
				executionLockCount--;
			}
		}
		private void action_CurrentValueChanged(object sender, EventArgs e) {
			UpdateEditorValue();
		}
		private void ExecuteWithCurrentValue() {
			if(executionLockCount == 0 && Control.Editor != null && !isExecuted) {
				isExecuted = true;
				((ParametrizedAction)Action).DoExecute(Control.Value);
			}
		}
		protected override void SetImage(ImageInfo imageInfo) {
			if(Control != null) {
				Control.SetImage(imageInfo, Action.ShortCaption);
			}
		}
		protected override void SetCaption(string caption) {
			if(Control != null) {
				Control.Caption = caption;
				if(Control.ActButton.Image.IsEmpty) {
					Control.ActButton.Text = Action.ShortCaption;
				}
				Control.SetNullValuePrompt(Action.NullValuePrompt);
			}
		}
		protected override void SetPaintStyle(ActionItemPaintStyle paintStyle) {
			base.SetPaintStyle(paintStyle);
			if(Control != null) {
				Control.CaptionVisible = paintStyle != ActionItemPaintStyle.Image;
			}
		}
		protected override void SetEnabled(bool enabled) {
			if(Control != null) {
				Control.ClientEnabled = enabled;
			}
		}
		protected override void SetToolTip(string toolTip) {
			if(Control != null) {
				Control.ToolTip = toolTip;
			}
		}
		protected override void SetConfirmationMessage(string message) { }
		protected override Control CreateControlCore() {
			isExecuted = false;
			ParametrizedActionControl result = ParametrizedActionControl.CreateControl(Action.ValueType, Orientation);
			if(result == null) {
				throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.InvalidParametrizedActionValueType, Action.Caption));
			}
			result.ID = WebIdHelper.GetCorrectedActionId(Action);
			result.Value = ((ParametrizedAction)Action).Value;
			result.SetNullValuePrompt(Action.NullValuePrompt);
			return result;
		}
		public ParametrizedActionMenuActionItem(ParametrizedAction action)
			: base(action) {
			action.ValueChanged += new EventHandler(action_CurrentValueChanged);
		}
		public override void Dispose() {
			if(Action != null) {
				Action.ValueChanged -= new EventHandler(action_CurrentValueChanged);
			}
			base.Dispose();
		}
		public override void ProcessAction() {
			ExecuteWithCurrentValue();
		}
		public override void SetClientClickHandler(XafCallbackManager callbackManager, string controlID) {
			string clientScript = callbackManager.GetScript(controlID, string.Format("'{0}'", MenuItem.IndexPath), Action.GetFormattedConfirmationMessage(), IsPostBackRequired);
			string clientClickHandler = "function(s, e) { if(e.buttonIndex < 0) { return; } " + clientScript + "e.processOnServer = false;}";
			if(Control != null) {
				Control.SetButtonClickScript(clientClickHandler);
			}
		}
		public new ParametrizedActionControl Control {
			get { return (ParametrizedActionControl)base.Control; }
		}
		public new ParametrizedAction Action {
			get { return (ParametrizedAction)base.Action; }
		}
		public ActionContainerOrientation Orientation {
			get { return orientation; }
			set { orientation = value; }
		}
		#region IParametrizedActionItemUnitTestable Members
#if DebugTest
		string IParametrizedActionItemUnitTestable.GoButtonCaption {
			get { return Control.ActButton.Text; }
		}
		object IParametrizedActionItemUnitTestable.Value {
			get { return Control.Value; }
		}
#endif
		#endregion
		#region IActionBaseItemUnitTestable Members
#if DebugTest
		bool IActionBaseItemUnitTestable.ControlEnabled {
			get { return Control.ClientEnabled; }
		}
		string IActionBaseItemUnitTestable.ControlToolTip {
			get { return Control.ToolTip; }
		}
		bool IActionBaseItemUnitTestable.ImageVisible {
			get { return Control != null && Control.ActButton.Image != null && !Control.ActButton.Image.IsEmpty; }
		}
		string IActionBaseItemUnitTestable.ControlCaption {
			get { return Control.Caption; }
		}
		bool IActionBaseItemUnitTestable.CaptionVisible {
			get { return Control.CaptionVisible; }
		}
#endif
		#endregion
		#region EasyTest
		public override string ClientId {
			get {
				if(Control != null) {
					return Control.ClientID;
				}
				else {
					return base.ClientId;
				}
			}
		}
		public override string TestCaption {
			get {
				return Action.Caption;
			}
		}
		#endregion
		protected override IJScriptTestControl CreateITestableControl() {
			return new JSASPxTestParametrizedActionControl();
		}
	}
}
