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
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using System.Web.UI;
using DevExpress.ExpressApp.Actions;
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.ExpressApp.Web.Layout;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu {
	public abstract class TemplatedMenuActionItem : MenuActionItemBase, ITemplate, ITestable {
		private Control control;
		protected abstract Control CreateControlCore();
		public Control Control {
			get { return control; }
		}
		protected TemplatedMenuActionItem(ActionBase action)
			: base(action) {
			MenuItem.Template = this;
		}
		public override void Dispose() {
			if(control != null) {
				control.Dispose();
				control = null;
			}
			base.Dispose();
		}
		private void CreateControl() {
			if(control == null) {
				control = CreateControlCore();
				control.Load += control_Load;
				control.ID = WebIdHelper.GetCorrectedActionId(Action);
				SynchronizeWithAction();
			}
		}
		void control_Load(object sender, EventArgs e) {
			if(focusControlOnLoad) {
				((Control)sender).Load -= control_Load;
				focusControlOnLoad = false;
				if(WebApplicationStyleManager.IsNewStyle) {
					string clientControlID;
					if(Control is ParametrizedActionTextBoxControl) {
						clientControlID = ((ParametrizedActionTextBoxControl)Control).Editor.ClientID;
					}
					else {
						clientControlID = Control.ClientID;
					}
					string focusItemByIndexPathFunctionBody = @"function(){ window['" + clientControlID + @"'].Focus();}";
					string focusItemByIndexPathFunction = "attachWindowEvent('load', function(){ WaitAnimateComplete(" + focusItemByIndexPathFunctionBody + ")});";
					if(Control.Page.IsCallback) {
						WebWindow.CurrentRequestWindow.RegisterClientScript("FocusTemplatedMenuActionItem", focusItemByIndexPathFunction, true);
					}
					else {
						Control.Page.ClientScript.RegisterClientScriptBlock(GetType(), "FocusTemplatedMenuActionItem", focusItemByIndexPathFunction, true);
					}
				}
				else {
					Control.Focus();
				}
			}
		}
		protected override void SetEnabled(bool enabled) {
		}
		void ITemplate.InstantiateIn(Control container) {
			CreateControl();
			SynchronizeWithAction();
			((System.Web.UI.WebControls.WebControl)this.Control).CssClass = "TemplatedItem";
			container.Controls.Add(Control);
			container.Load += new EventHandler(Control_LoadUnload);
			container.Unload += new EventHandler(Control_LoadUnload);
		}
		private void Control_LoadUnload(object sender, EventArgs e) {
			OnControlInitialized(Control);
		}
		#region EasyTest
		private IJScriptTestControl jScriptTestControl;
		protected virtual IJScriptTestControl CreateITestableControl() {
			return new JSASPxMenuActionsHolderItem();
		}
		private IJScriptTestControl JScriptTestControl {
			get {
				if(jScriptTestControl == null) {
					jScriptTestControl = CreateITestableControl();
				}
				return jScriptTestControl;
			}
		}
		public override IJScriptTestControl TestControl {
			get {
				return JScriptTestControl;
			}
		}
		#endregion
		private bool focusControlOnLoad;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void Focus() {
			if(Control != null) {
				Control.Focus();
			}
			else {
				focusControlOnLoad = true;
			}
		}
	}
}
