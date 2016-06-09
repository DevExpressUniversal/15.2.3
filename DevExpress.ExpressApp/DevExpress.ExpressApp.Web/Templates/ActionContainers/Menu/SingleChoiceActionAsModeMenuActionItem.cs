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
using System.Web.UI;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu {
	public class SingleChoiceActionAsModeMenuActionItem : TemplatedMenuActionItem, ITestable
#if DebugTest
, DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable
#endif
 {
		private DropDownSingleChoiceActionControlHelper helper;
		private SingleChoiceActionItemOrientation orientation;
		private string clientSelectedItemChangedHandler;
		public SingleChoiceActionItemOrientation Orientation {
			get { return orientation; }
			set { orientation = value; }
		}
		private void helper_DropDownSelectedItemChanged(object sender, DropDownSelectedItemChangedEventArgs e) {
			Action.DoExecute(e.Item);
		}
		private void DisposeHelper() {
			if(helper != null) {
				helper.DropDownSelectedItemChanged -= new EventHandler<DropDownSelectedItemChangedEventArgs>(helper_DropDownSelectedItemChanged);
				helper.Dispose();
				helper = null;
			}
		}
		protected new DropDownSingleChoiceActionControlBase Control {
			get { return (DropDownSingleChoiceActionControlBase)base.Control; }
		}
		protected override Control CreateControlCore() {
			DropDownSingleChoiceActionControlBase control;
			if(Orientation == SingleChoiceActionItemOrientation.Horizontal) {
				control = new DropDownSingleChoiceActionControlHorizontal();
			}
			else {
				control = new DropDownSingleChoiceActionControlVertical();
			}
			control.ID = WebIdHelper.GetCorrectedActionId(Action);
			control.ComboBox.ClientSideEvents.SelectedIndexChanged = clientSelectedItemChangedHandler;
			control.ComboBox.ClientSideEvents.Init = "function(s, e){ s.currentSelectedIndex = s.GetSelectedIndex(); }";
			control.ComboBox.AutoPostBack = false;
			if(helper != null) {
				DisposeHelper();
			}
			helper = new DropDownSingleChoiceActionControlHelper(Action, control);
			helper.DropDownSelectedItemChanged += new EventHandler<DropDownSelectedItemChangedEventArgs>(helper_DropDownSelectedItemChanged);
			return control;
		}
		protected override void SetCaption(string caption) {
			if(Control != null) {
				if(!WebApplicationStyleManager.IsNewStyle) {
					Control.Label.Text = caption;
				}
				else {
				}
			}
		}
		protected override void SetEnabled(bool enabled) {
			if(Control != null) {
				Control.ClientEnabled = enabled;
			}
		}
		protected virtual void SetToolTip(string toolTip, MenuItem menuItem) {
			if(menuItem != null) {
				menuItem.ToolTip = toolTip;
			}
		}
		protected virtual string GetToolTip() {
			return MenuItem != null ? MenuItem.ToolTip : null;
		}
		protected override void SetToolTip(string toolTip) {
			SetToolTip(toolTip, this.MenuItem);
		}
		public override void ProcessAction() {
			if(Control != null && Control.ComboBox != null) {
				ListEditItem listItem = Control.ComboBox.SelectedItem;
				ChoiceActionItem actionItem = helper.GetActionItemByListItem(listItem);
				if(actionItem != null) {
					Action.DoExecute(actionItem);
				}
			}
		}
		public override void Dispose() {
			try {
				DisposeHelper();
			}
			finally {
				base.Dispose();
			}
		}
		public SingleChoiceActionAsModeMenuActionItem(SingleChoiceAction action)
			: base(action) {
		}
		public new SingleChoiceAction Action {
			get { return (SingleChoiceAction)base.Action; }
		}
		public override void SetClientClickHandler(XafCallbackManager callbackManager, string controlID) {
			string registerClientId = this.MenuItem.Menu.ClientID + '_' + this.Action.Id;
			string clientScript = callbackManager.GetScript(controlID, string.Format("'{0}'", MenuItem.IndexPath), Action.GetFormattedConfirmationMessage(), IsPostBackRequired);
			clientSelectedItemChangedHandler = string.Format(@"function(s, e) {{ 
                if(typeof xaf == 'undefined' || !xaf.ConfirmUnsavedChangedController || xaf.ConfirmUnsavedChangedController.CanProcessCallbackForChoiceAction(s,e, '{0}')){{    
                    if (!{1}) {{
                        s.SetSelectedIndex(s.currentSelectedIndex != undefined ? s.currentSelectedIndex : -1);    
                    }}
                }}
            }}", registerClientId, clientScript.Replace(";", ""));
			if(Control != null && Control.ComboBox != null) {
				Control.ComboBox.ClientSideEvents.SelectedIndexChanged = clientSelectedItemChangedHandler;
			}
		}
#if DebugTest
		#region ISingleChoiceActionItemUnitTestable Members
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemVisible(string itemPath) {
			return helper.GetListEditItemByPath(itemPath) != null;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemEnabled(string itemPath) {
			return helper.GetListEditItemByPath(itemPath) != null;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemBeginsGroup(string itemPath) {
			ChoiceActionItem item = Action.FindItemByCaptionPath(itemPath);
			return item == null ? false : item.BeginGroup;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemImageVisible(string itemPath) {
			ListEditItem item = helper.GetListEditItemByPath(itemPath);
			return item == null ? false : !string.IsNullOrEmpty(item.ImageUrl);
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemSelected(string itemPath) {
			ListEditItem item = helper.GetListEditItemByPath(itemPath);
			return Control.ComboBox.SelectedItem == item;
		}
		#endregion
		#region IActionBaseItemUnitTestable Members
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlEnabled {
			get { return Control.ClientEnabled; }
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlCaption {
			get { return Control.Label.Text; }
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlToolTip {
			get { return MenuItem.ToolTip; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.SupportPaintStyle {
			get { return false; }
		}
		#endregion
#endif
		string ITestable.ClientId {
			get {
				if(Control != null) {
					return Control.ClientID;
				}
				return base.ClientId;
			}
		}
		protected override IJScriptTestControl CreateITestableControl() {
			return new JSASPxDropDownSingleChoiceActionControl();
		}
	}
}
