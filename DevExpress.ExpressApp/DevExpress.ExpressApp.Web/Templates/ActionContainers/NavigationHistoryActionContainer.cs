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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	[ToolboxItem(false)] 
	[PersistChildren(false), ParseChildren(false)]
	public class NavigationHistoryActionContainer : WebControl, IActionContainer, ITestable, IXafCallbackHandler {
		private int updateLockCounter = 0;
		private int visibleItemCount = 5;
		private int visibleItemLength = 30;
		private string delimiter = "&nbsp;»&nbsp;";
		private string name;
		private SingleChoiceAction action;
		private Dictionary<HtmlAnchor, ChoiceActionItem> ActionButtonToItemMap = new Dictionary<HtmlAnchor, ChoiceActionItem>();
		protected void RebuildLinks() {
			Controls.Clear();
			ActionButtonToItemMap.Clear();
			if(action != null && action.Active && action.Items.Count > 1) {
				List<ChoiceActionItem> items = GetNonDuplicateItems(action.Items);
				int currentItemIndex = items.IndexOf(action.SelectedItem);
				int leftSideIndex = currentItemIndex - visibleItemCount / 2;
				int rightSideIndex = currentItemIndex + (visibleItemCount - visibleItemCount / 2);
				if(rightSideIndex > items.Count - 1) {
					rightSideIndex = items.Count - 1;
					leftSideIndex = rightSideIndex - visibleItemCount + 1;
				}
				if(leftSideIndex < 0) {
					leftSideIndex = 0;
					rightSideIndex = visibleItemCount;
					if(rightSideIndex > items.Count - 1) {
						rightSideIndex = items.Count - 1;
					}
				}
				for(int i = leftSideIndex; i <= rightSideIndex; i++) {
					ChoiceActionItem navigationItem = items[i];
					HtmlAnchor button = new HtmlAnchor();
					button.ID = "L" + i.ToString();
					Literal text = new Literal();
					button.Controls.Add(text);
					text.Mode = LiteralMode.Encode;
					text.Text = GetLinkText(navigationItem);
					button.Disabled = !(navigationItem.Enabled && action.Enabled);
					ActionButtonToItemMap.Add(button, navigationItem);
					if(!button.Disabled) {
						if(!IsItemSelected(navigationItem)) {
							UpdateClientScript(button);
						} else {
							button.Attributes["class"] = "Current";
						}
					}
					Controls.Add(button);
					button.Style["white-space"] = "nowrap";
					Literal delim = new Literal();
					delim.Text = Delimiter;
					Controls.Add(delim);
				}
				if(Controls.Count > 0 && Controls[Controls.Count - 1] is Literal) {
					Controls.RemoveAt(Controls.Count - 1);
				}
			}
		}
		private void UpdateClientScript(HtmlAnchor button) {
			ICallbackManagerHolder holder = Page as ICallbackManagerHolder;
			if(holder != null) {
				bool isPostBackRequired = action.Model.GetValue<bool>("IsPostBackRequired");
				button.HRef = "javascript:{}";
				button.Attributes["onclick"] = holder.CallbackManager.GetScript(UniqueID, string.Format("'{0}'", ActionButtonToItemMap[button].GetIdPath()), String.Empty, isPostBackRequired); 
			}
		}
		private string GetLinkText(ChoiceActionItem navigationItem) {
			return GetVisibleLinkText(navigationItem.Caption);
		}
		protected List<ChoiceActionItem> GetNonDuplicateItems(ChoiceActionItemCollection items) {
			List<ChoiceActionItem> result = new List<ChoiceActionItem>();
			ArrayList dataList = new ArrayList();
			foreach(ChoiceActionItem item in items) {
				if(!dataList.Contains(item.Data)) {
					dataList.Add(item.Data);
					result.Add(item);
				}
			}
			return result;
		}
		private bool IsItemSelected(ChoiceActionItem item) {
			if(item.Data != null && action.SelectedItem != null) {
				return item.Data.Equals(action.SelectedItem.Data);
			}
			return false;
		}
		private void SetVisible() {
			Visible = Controls.Count > 0;
		}
		private void button_ServerClick(object sender, EventArgs e) {
			action.DoExecute(ActionButtonToItemMap[(HtmlAnchor)sender]);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			SetVisible();
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if(DesignMode) {
				SingleChoiceAction navigationAction = new SingleChoiceAction(null, "NavigateTo", "");
				navigationAction.Items.Add(new ChoiceActionItem("Contacts", ""));
				navigationAction.Items.Add(new ChoiceActionItem("Today Task", ""));
				navigationAction.Items.Add(new ChoiceActionItem("Inbox", ""));
				navigationAction.Items.Add(new ChoiceActionItem("My Details", ""));
				navigationAction.SelectedItem = navigationAction.Items[2];
				Register(navigationAction);
			}
			ICallbackManagerHolder holder = Page as ICallbackManagerHolder;
			if(holder != null) {
				holder.CallbackManager.RegisterHandler(UniqueID, this);
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			WebActionContainerHelper.TryRegisterActionContainer(this, new IActionContainer[] { this });
			ICallbackManagerHolder holder = Page as ICallbackManagerHolder;
			if(holder != null) {				
				holder.CallbackManager.PreRenderInternal += new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			}
			RebuildLinks();
		}
		private void CallbackManager_PreRenderInternal(object sender, EventArgs e) {
			((XafCallbackManager)sender).PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			SetVisible();
		}
		protected override void OnUnload(EventArgs e) {
			OnControlInitialized(this);
			ICallbackManagerHolder holder = Page as ICallbackManagerHolder;
			if(holder != null) {				
				holder.CallbackManager.PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
		}
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		public NavigationHistoryActionContainer()
			: base() {
			CssClass = "xafBreadCrumbs";
		}
		public override void Dispose() {
			try {
				foreach(HtmlAnchor control in ActionButtonToItemMap.Keys) {
					control.ServerClick -= new EventHandler(button_ServerClick);
					control.Dispose();
				}
				ActionButtonToItemMap.Clear();
				if (this.action != null) {
					this.action.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
				}
				action = null;
			}
			finally {
				base.Dispose();
			}
		}
		#region IActionContainer Members
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyCollection<ActionBase> Actions {
			get { return new ReadOnlyCollection<ActionBase>(new ActionBase[] { action }); }
		}
		[DefaultValue(null), TypeConverter(typeof(DevExpress.ExpressApp.Core.Design.ContainerIdConverter)), Category("Design")]
		public string ContainerId {
			get { return name; }
			set {
				name = value;
			}
		}
		public void Register(ActionBase action) {
			if(action is SingleChoiceAction) {
				if (this.action != null) {
					this.action.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
				}
				this.action = (SingleChoiceAction)action;
				this.action.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
				RebuildLinks();
			}
		}
		void action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			RebuildLinks();
		}
		#endregion
		#region ISupportUpdate Members
		public void BeginUpdate() {
			updateLockCounter++;
		}
		public void EndUpdate() {
			if(updateLockCounter > 0) {
				updateLockCounter--;
			}
			if(updateLockCounter == 0) {
				RebuildLinks();
			}
		}
		#endregion
		[DefaultValue("&nbsp;»&nbsp;")]
		public string Delimiter{
			get { return delimiter; }
			set {
				if(delimiter != value && !string.IsNullOrEmpty(value)) {
					delimiter = value;
					RebuildLinks();
				}
			}
		}
		[DefaultValue(5)]
		public int VisibleItemCount {
			get { return visibleItemCount; }
			set {
				if(visibleItemCount != value) {
					visibleItemCount = value > 2 ? value : 2;
					RebuildLinks();
				}
			}
		}
		public int VisibleItemLength {
			get { return visibleItemLength; }
			set {
				if(visibleItemLength != value) {
					visibleItemLength = value > 0 ? value : 1;
					RebuildLinks();
				}
				visibleItemLength = value; 
			}
		}
		public string GetVisibleLinkText(string text) {
			string result = text;
			if(text.Length > visibleItemLength) {
				result = result.Substring(0, visibleItemLength) + "...";
			}
			return result;
		}
		#region ITestable Members
		public string TestCaption {
			get { return "NavigationHistory"; }
		}
		public string ClientId {
			get { return ClientID; }
		}
		public IJScriptTestControl TestControl {
			get { return new NavigationHistoryActionContainerJScriptTestControl(); }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get {
				return TestControlType.Action;
			}
		}
		#endregion
		#region IXafCallbackHandler Members
		public void ProcessAction(string parameter) {
			if(action.Active && action.Enabled) {
				ChoiceActionItem item = action.FindItemByIdPath(parameter);
				if(item != null) {
					action.DoExecute(item);
	}
			}
		}
		#endregion
	}
	public class NavigationHistoryActionContainerJScriptTestControl : IJScriptTestControl {
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return "NavigationHistoryActionContainer"; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.IsEnabledFunctionBody = @"return true;";
				result.ActFunctionBody = @"
                var item = this.GetItem(value);
                if(item) {
					if(this.IsActionItemEnabled(value)) {
						item.click();
					}
					else {
						this.LogOperationError('The ""' + value + '"" item of the ""' + this.caption + '"" Action is disabled');
					}
				}
				else {
					this.LogOperationError('The ""' + this.caption + '"" Action does not contain the ""' + value + '"" item');
				}
				";
				result.GetTextFunctionBody = @"
				var aspxControl = window[this.id];
				var i, result = '';
				for(i = 0; i < aspxControl.children.length; i++) {
					result += aspxControl.children[i].innerText;
					if(i != aspxControl.children.length - 1) {
						result += ';';
					}
				}
				return result;
				";
				result.AdditionalFunctions = new JSFunctionDeclaration[] {
					new JSFunctionDeclaration("GetItem", "value", @"
				        var aspxControl = window[this.id];
				        var i;
                        var executableControl = null;
				        for(i = 0; i < aspxControl.children.length; i++) {
					        if(aspxControl.children[i].innerText == value) {
						        executableControl = aspxControl.children[i];
						        break;
					        }
				        }
                        return executableControl;
                    "),
					new JSFunctionDeclaration("IsActionItemVisible", "actionItemName", @"
                        var item = this.GetItem(actionItemName);
                        return item != null;
                    "),
					new JSFunctionDeclaration("IsActionItemEnabled", "actionItemName", @"
                        var item = this.GetItem(actionItemName);
                        if(item) {
                            if(item.click != 'undefined' && !item.disabled && item.href != '') {
                                return true;
                            }
                        }
                        return false;
                    ")
				};
				return result;
			}
		}
		#endregion
	}
}
