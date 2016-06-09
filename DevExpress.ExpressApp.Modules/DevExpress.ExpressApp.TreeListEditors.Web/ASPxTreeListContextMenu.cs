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
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Web.ASPxTreeList;
namespace DevExpress.ExpressApp.TreeListEditors.Web {
	public class ASPxTreeListActionItem : WebActionBaseItem {
		private TreeListCommandColumnCustomButton actionButton;
		private string confirmationMessage;
		public TreeListCommandColumnCustomButton ActionButton {
			get { return actionButton; }
		}
		private Predicate<ActionBase> isActionVisibleInContextMenu;
		private TreeListCommandColumn commandColumn;
		private ActionBase action;
		private void UpdateVisibility() {
			actionButton.Visibility = isActionVisibleInContextMenu(action) ? TreeListCustomButtonVisibility.AllNodes : TreeListCustomButtonVisibility.Hidden;
		}
		protected override void SetEnabled(bool enabled) {
			UpdateVisibility();
		}
		protected override void SetVisible(bool visible) {
			UpdateVisibility();
		}
		protected override void SetCaption(string caption) {
			actionButton.Text = caption;
			actionButton.Image.AlternateText = caption;
		}
		protected override void SetToolTip(string toolTip) {
		}
		protected override void SetImage(ImageInfo imageInfo) {
			if(!imageInfo.IsEmpty) {
				ASPxImageHelper.SetImageProperties(actionButton.Image, imageInfo);
				commandColumn.ButtonType = ButtonType.Image;
			}
		}
		protected override void SetConfirmationMessage(string message) {
			confirmationMessage = message;
		}
		public ASPxTreeListActionItem(TreeListCommandColumn commandColumn, ActionBase action, Predicate<ActionBase> isActionVisibleInContextMenu)
			: base(action) {
			this.commandColumn = commandColumn;
			this.action = action;
			this.isActionVisibleInContextMenu = isActionVisibleInContextMenu;
			actionButton = new TreeListCommandColumnCustomButton();
			commandColumn.CustomButtons.Add(actionButton);
			actionButton.ID = action.Id;
			actionButton.Index = commandColumn.CustomButtons.Count - 1;
			SynchronizeWithAction();
		}
		public override void Dispose() {
			try {
				actionButton = null;
				action = null;
				isActionVisibleInContextMenu = null;
				commandColumn = null;
			}
			finally {
				base.Dispose();
			}
		}
		public string ConfirmationMessage {
			get { return confirmationMessage; }
		}
		#region ITestable
		public override string TestCaption {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		public override string ClientId {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		public override DevExpress.ExpressApp.Web.TestScripts.IJScriptTestControl TestControl {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		#endregion
	}
	public class ASPxTreeListContextMenu : ListEditorContextMenuBase<WebContextMenuActionContainer>, IXafCallbackHandler {
		private List<TreeListCommandColumn> commandColumns;
		private List<ASPxTreeListActionItem> actionItems = new List<ASPxTreeListActionItem>();
		private Dictionary<TreeListCommandColumnCustomButton, ASPxTreeListActionItem> buttonToItemMap = new Dictionary<TreeListCommandColumnCustomButton, ASPxTreeListActionItem>();
		private void SubscribeToTreeList() {
			TreeList.Load += new EventHandler(TreeList_Load);
			TreeList.CommandColumnButtonInitialize += new TreeListCommandColumnButtonEventHandler(TreeList_CommandColumnButtonInitialize);
		}
		private void UnsubscribeFromTreeList() {
			if(TreeList != null) {
				TreeList.Load -= new EventHandler(TreeList_Load);
				TreeList.CommandColumnButtonInitialize -= new TreeListCommandColumnButtonEventHandler(TreeList_CommandColumnButtonInitialize);
				foreach(TreeListCommandColumn commandColumn in commandColumns) {
					if(commandColumn != null && TreeList.Columns.IndexOf(commandColumn) != -1) {
						TreeList.Columns.Remove(commandColumn);
					}
				}
				commandColumns.Clear();
			}
		}
		private void TreeList_Load(object sender, EventArgs e) {
			Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), TreeList.Page.GetType(), "Page");
			XafCallbackManager callbackManager = ((ICallbackManagerHolder)TreeList.Page).CallbackManager;
			callbackManager.RegisterHandler(TreeList.UniqueID, this);
			TreeList.ClientSideEvents.CustomButtonClick = GetCustomButtonClickScript(callbackManager);
			TreeList.ClientSideEvents.NodeClick = GetNodeClickScript(callbackManager);
		}
		private void TreeList_CommandColumnButtonInitialize(object sender, TreeListCommandColumnButtonEventArgs e) {
			bool isVisible = false;
			if(e.ButtonType == TreeListCommandColumnButtonType.Custom) {
				TreeListCommandColumnCustomButton button = e.CommandColumn.CustomButtons[e.CustomButtonIndex];
				ASPxTreeListActionItem item = buttonToItemMap[button];
				if(item != null) {
					TreeListNode node = TreeList.FindNodeByKeyValue(e.NodeKey);
					isVisible = node != null && GetRowActionActivity(item.Action, node[ASPxTreeListEditor.RowObjectColumnName]);
				}
			}
			e.Visible = isVisible ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
		}
		private TreeListCommandColumn CreateCommandColumn() {
			TreeListCommandColumn commandColumn = new TreeListCommandColumn();
			commandColumn.ButtonType = ButtonType.Link;
			commandColumn.Width = Unit.Percentage(1);
			commandColumn.Caption = "&nbsp;";
			commandColumns.Add(commandColumn);
			return commandColumn;
		}
		private string GetCustomButtonClickScript(XafCallbackManager callbackManager) {
			StringBuilder script = new StringBuilder();
			script.AppendLine("function(s, e) {");
			string parameters = string.Format("'{0}:' + e.buttonID + ':' + e.nodeKey", WebListEditorCommand.XafCommandPrefix);
			string confirmationExpression;
			AppendConfirmationSection(script, actionItems, out confirmationExpression);
			string usePostBackExpression;
			AppendUsePostBackSection(script, actionItems, out usePostBackExpression);
			script.Append("    ").AppendLine(callbackManager.GetScript(TreeList.UniqueID, parameters, confirmationExpression, usePostBackExpression));
			script.Append("}");
			return script.ToString();
		}
		private static void AppendConfirmationSection(StringBuilder script, IEnumerable<ASPxTreeListActionItem> actionItems, out string confirmationExpression) {
			bool containsConfirmations = false;
			foreach(ASPxTreeListActionItem item in actionItems) {
				string confirmation = item.ConfirmationMessage.Replace("'", "`");
				if(!string.IsNullOrEmpty(confirmation)) {
					if(!containsConfirmations) {
						containsConfirmations = true;
						script.AppendLine("    var confirmations = {};");
					}
					script.AppendFormat("    confirmations['{0}'] = '{1}';", item.ActionButton.ID, confirmation).AppendLine();
				}
			}
			if(containsConfirmations) {
				script.AppendLine("    var confirmation = confirmations[e.buttonID] || '';");
			}
			confirmationExpression = containsConfirmations ? "confirmation" : "''";
		}
		private static void AppendUsePostBackSection(StringBuilder script, IEnumerable<ASPxTreeListActionItem> actionItems, out string usePostBackExpression) {
			bool containsUsePostBack = false;
			foreach(ASPxTreeListActionItem item in actionItems) {
				if(GetPostBackRequired(item)) {
					if(!containsUsePostBack) {
						containsUsePostBack = true;
						script.AppendLine("    var usePostBackCache = {};");
					}
					script.AppendFormat("    usePostBackCache['{0}'] = true;", item.ActionButton.ID).AppendLine();
				}
			}
			if(containsUsePostBack) {
				script.AppendLine("    var usePostBack = usePostBackCache[e.buttonID] || false;");
			}
			usePostBackExpression = containsUsePostBack ? "usePostBack" : "false";
		}
		private static bool GetPostBackRequired(ASPxTreeListActionItem item) {
			return item.Action.Model.GetValue<bool>("IsPostBackRequired");
		}
		private string GetNodeClickScript(XafCallbackManager callbackManager) {
			string parameters = string.Format("'{0}::' + e.nodeKey", WebListEditorCommand.XafCommandPrefix);
			return string.Format("function(s, e) {{ {0} }}", callbackManager.GetScript(TreeList.UniqueID, parameters));
		}
		protected virtual void ProcessCustomCallback(string parameters) {
			WebListEditorCommand command = new WebListEditorCommand(parameters);
			if(!command.IsEmpty) {
				SelectObject(command.ObjectId);
				if(string.IsNullOrEmpty(command.CommandId)) {
					Editor.DoOnProcessSelectedItem();
				}
				else {
					ProcessAction(command.CommandId);
				}
			}
		}
		private void SelectObject(string objectId) {
			TreeListNode node = TreeList.FindNodeByKeyValue(objectId);
			if(node != null) {
				Editor.SetTemporarySelectedObject(node[ASPxTreeListEditor.RowObjectColumnName]);
			}
		}
		public ASPxTreeListContextMenu(ASPxTreeListEditor editor)
			: base(editor) {
			commandColumns = new List<TreeListCommandColumn>();
		}
		public override void CreateControlsCore() {
			UnsubscribeFromTreeList();
			TreeList.Styles.CommandButton.Cursor = "pointer";
			actionItems.Clear();
			buttonToItemMap.Clear();
			foreach(WebContextMenuActionContainer container in Containers) {
				foreach(ActionBase action in container.Actions) {
					if(IsActionVisibleInContextMenu(action)) {
						if(action is SimpleAction || action is PopupWindowShowAction) {
							TreeListCommandColumn commandColumn = CreateCommandColumn();
							TreeList.Columns.Add(commandColumn);
							ASPxTreeListActionItem actionItem = new ASPxTreeListActionItem(commandColumn, action, IsActionVisibleInContextMenu);
							actionItems.Add(actionItem);
							buttonToItemMap.Add(actionItem.ActionButton, actionItem);
						}
					}
				}
			}
			SubscribeToTreeList();
		}
		public override void Dispose() {
			try {
				buttonToItemMap.Clear();
				if(actionItems != null) {
					foreach(IDisposable disposable in actionItems) {
						disposable.Dispose();
					}
					actionItems.Clear();
				}
				BreakLinksToControls();
			}
			finally {
				base.Dispose();
			}
		}
		public override void BreakLinksToControls() {
			UnsubscribeFromTreeList();
		}
		public ASPxTreeList TreeList {
			get { return Editor == null ? null : Editor.TreeList; }
		}
		public new ASPxTreeListEditor Editor {
			get { return (ASPxTreeListEditor)base.Editor; }
		}
		public List<TreeListCommandColumn> CommandColumns {
			get { return commandColumns; }
		}
		public IList<ASPxTreeListActionItem> ActionItems {
			get { return actionItems; }
		}
		#region IXafCallbackHandler Members
		void IXafCallbackHandler.ProcessAction(string parameter) {
			ProcessCustomCallback(parameter);
		}
		#endregion
	}
}
