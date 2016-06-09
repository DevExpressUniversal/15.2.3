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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
using DevExpress.Web.Rendering;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxGridViewContextMenu : ListEditorContextMenuBase<WebContextMenuActionContainer>, IXafCallbackHandler {
		private const string ActionEventArgumentPrefix = "XafAction:";
		public const string InlineEditCommandColumnName = "InlineEditCommandColumn";
		public const string SelectionCommandColumnName = "SelectionCommandColumn";
		private ASPxGridView gridControl;
		private List<GridViewColumn> actionColumns;
		private GridViewCommandColumn selectColumn;
		private GridViewCommandColumn editColumn;
		private GridCommandButtonRenderMode commandButtonType = GridCommandButtonRenderMode.Image;
		private void UnsubscribeGridEvents(ASPxGridView gridControl) {
			gridControl.HtmlCommandCellPrepared -= gridControl_HtmlCommandCellPrepared;
			gridControl.HtmlDataCellPrepared -= new ASPxGridViewTableDataCellEventHandler(gridControl_HtmlDataCellPrepared);
			gridControl.CommandButtonInitialize -= new ASPxGridViewCommandButtonEventHandler(gridControl_CommandButtonInitialize);
			gridControl.HtmlRowPrepared -= new ASPxGridViewTableRowEventHandler(gridControl_HtmlRowPrepared);
			gridControl.CustomCallback -= new ASPxGridViewCustomCallbackEventHandler(gridControl_CustomCallback);
			gridControl.Load -= new EventHandler(gridControl_Load);
		}
		private void UnlinkGrid() {
			if(gridControl != null) {
				if(actionColumns != null) {
					foreach(GridViewColumn actionColumn in actionColumns) {
						gridControl.Columns.Remove(actionColumn);
						IDisposable disposable = actionColumn as IDisposable;
						if(disposable != null) {
							disposable.Dispose();
						}
					}
				}
				if(selectColumn != null) {
					gridControl.Columns.Remove(selectColumn);
				}
				if(editColumn != null) {
					gridControl.Columns.Remove(editColumn);
				}
			}
			if(gridControl != null) {
				UnsubscribeGridEvents(gridControl);
				gridControl = null;
			}
			if(actionColumns != null) {
				actionColumns.Clear();
			}
			actionColumns = null;
			selectColumn = null;
			editColumn = null;
		}
		private void ProcessCommand(string args) {
			if(Editor == null) {
				return;
			}
			WebListEditorCommand command = new WebListEditorCommand(args);
			if(!command.IsEmpty) {
				if(gridControl.IsNewRowEditing || gridControl.IsEditing) {
					gridControl.CancelEdit();
				}
				SelectObject(command.ObjectId);
				if(string.IsNullOrEmpty(command.CommandId)) {
					Editor.DoOnProcessSelectedItem();
				}
				else {
					ProcessAction(command.CommandId);
					if(Editor != null && Editor.Control != null) {
						Page page = ((Control)Editor.Control).Page;
						if(page != null && page.IsCallback) {
							Editor.TryRebindData(page);
						}
						if(!Editor.CanSelectRows) {
							Editor.ClearTemporarySelectedObject();
						}
					}
				}
			}
		}
		private void SelectObject(string objectId) {
			Object key = null;
			if(UseVisibleIndexAsKey) {
				key = GetKeyConverter(null).ConvertFromString(objectId);
			}
			else {
				if(Editor.KeyMembersInfo.Count == 1) {
					key = GetKeyConverter(Editor.KeyMembersInfo[0].MemberType).ConvertFromString(objectId);
				}
				else if(Editor.KeyMembersInfo.Count > 1) {
					key = new List<Object>();
					String[] objectIdParts = objectId.Split(',', ';');
					for(Int32 i = 0; i < objectIdParts.Length; i++) {
						((List<Object>)key).Add(GetKeyConverter(Editor.KeyMembersInfo[i].MemberType).ConvertFromString(objectIdParts[i]));
					}
				}
			}
			object objectToFocus;
			if(UseVisibleIndexAsKey) {
				objectToFocus = Editor.GetRowObject((int)key);
			}
			else {
				objectToFocus = Editor.GetObjectByKey(key);
			}
			Editor.SetTemporarySelectedObject(objectToFocus);
		}
		private bool CancelClickEventPropagationForColumn(GridViewDataColumn column) {
			bool result = column is GridViewDataHyperLinkColumn;
			if(column.DataItemTemplate is IBehaviourTemplate) {
				result |= ((IBehaviourTemplate)column.DataItemTemplate).CancelClickEventPropagation;
			}
			return result;
		}
		private void gridControl_HtmlCommandCellPrepared(object sender, ASPxGridViewTableCommandCellEventArgs e) {
			if(e.CommandCellType != GridViewTableCommandCellType.Filter) {
				if(e.CommandColumn is GridViewDataActionColumn) {
					string script = "";
					ActionBase action = ((GridViewDataActionColumn)e.CommandColumn).Action;
					if(TryGetCellClickScript(action, e.VisibleIndex, out script)) {
						e.Cell.Attributes["onclick"] = string.Format(@"
                            var grid = ASPx.GetControlCollection().Get('{2}');
                            var actionID = '{3}_{4}';
                            if(!grid.XafGridViewUpdateWatcherHelper || grid.XafGridViewUpdateWatcherHelper.CanProcessCallbackForGridAction(actionID)){{
                                {0}{1} 
                            }}", RenderHelper.EventCancelBubbleCommand, script, ((ASPxGridView)sender).ClientID, action.Category, action.Id);
					}
					else {
						e.Cell.Controls.Clear();
						e.Cell.Controls.Add(new LiteralControl("&nbsp;"));
					}
					e.Cell.CssClass += " ActionCell";
				}
				SetupCellAttributes(e.Cell);
			}
		}
		private void gridControl_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e) {
			if(CancelClickEventPropagationForColumn(e.DataColumn)) {
				e.Cell.Attributes["onclick"] = RenderHelper.EventCancelBubbleCommand;
			}
			if(!Editor.IsBatchMode) {
				SetupCellAttributes(e.Cell);
			}
		}
		private void SetupCellAttributes(TableCell cell) {
			cell.Style.Add("cursor", "pointer");
		}
		private void gridControl_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e) {
			if(e.ButtonType == ColumnCommandButtonType.Edit) {
				ActionBase inlineEditAction = FindInlineEditAction();
				if(inlineEditAction != null) {
					inlineEditAction.Enabled.RemoveItem(ActionsCriteriaViewController.EnabledByCriteriaKey);
					e.Visible = GetRowActionActivity(inlineEditAction, Editor.GetRowObject(e.VisibleIndex));
				}
				else {
					e.Visible = false;
				}
			}
		}
		private bool UseVisibleIndexAsKey {
			get { return Editor.KeyMembersInfo.Count == 0 || !Editor.CanConvertKeyMembersInfoFromToString(); }
		}
		private TypeConverter GetKeyConverter(Type memberType) {
			if(UseVisibleIndexAsKey) {
				return TypeDescriptor.GetConverter(typeof(int));
			}
			else {
				return TypeDescriptor.GetConverter(memberType);
			}
		}
		private Object GetKeyByVisibleIndex(Int32 visibleIndex) {
			if(UseVisibleIndexAsKey) {
				return visibleIndex;
			}
			else {
				if(Editor.KeyMembersInfo.Count == 1) {
					return gridControl.GetRowValues(visibleIndex, Editor.KeyMembersInfo[0].Name);
				}
				else {
					List<Object> result = new List<Object>();
					foreach(IMemberInfo keyMemberInfo in Editor.KeyMembersInfo) {
						result.Add(gridControl.GetRowValues(visibleIndex, keyMemberInfo.Name));
					}
					return result;
				}
			}
		}
		private bool TryGetCellClickScript(ActionBase action, int visibleIndex, out string script) {
			object itemObject = Editor.GetRowObject(visibleIndex);
			bool isActive = GetRowActionActivity(action, itemObject);
			script = isActive ? GetClickScript(action, visibleIndex, itemObject) : "";
			return isActive;
		}
		private string GetClickScript(ActionBase action, int visibleIndex, object itemObject) {
			string confirmationMessage = GetConfirmationMessage(action, itemObject);
			string postBackArgument = GetPostBackArgument(action, visibleIndex);
			bool isPostBackRequired = GetPostBackRequired(action);
			Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), gridControl.Page.GetType(), "Page");
			return ((ICallbackManagerHolder)gridControl.Page).CallbackManager.GetScript(gridControl.UniqueID, string.Format("'{0}'", postBackArgument), confirmationMessage, isPostBackRequired);
		}
		private static string GetConfirmationMessage(ActionBase action, object itemObject) {
			string confirmationMessage = string.Empty;
			if(!string.IsNullOrEmpty(action.ConfirmationMessage)) {
				string objectName;
				ITypeInfo itemType = XafTypesInfo.Instance.FindTypeInfo(itemObject.GetType());
				IMemberInfo defaultMember = itemType.DefaultMember;
				if(defaultMember != null) {
					object defaultMemberValue = defaultMember.GetValue(itemObject);
					objectName = defaultMemberValue != null ? defaultMemberValue.ToString() : string.Empty;
				}
				else {
					objectName = itemObject.ToString();
				}
				confirmationMessage = string.Format(action.ConfirmationMessage, objectName);
			}
			return confirmationMessage;
		}
		private bool GetPostBackRequired(ActionBase action) {
			return action.Model.GetValue<bool>("IsPostBackRequired");
		}
		private string GetPostBackArgument(ActionBase action, int visibleIndex) {
			Object key = GetKeyByVisibleIndex(visibleIndex);
			String stringKey = "";
			if(Editor.KeyMembersInfo.Count > 1) {
				for(Int32 i = 0; i < Editor.KeyMembersInfo.Count; i++) {
					stringKey = stringKey + GetKeyConverter(Editor.KeyMembersInfo[i].MemberType).ConvertToString(((IList)key)[i]) + ";";
				}
				stringKey = stringKey.TrimEnd(';');
			}
			else {
				stringKey = GetKeyConverter(Editor.KeyMembersInfo.Count > 0 ? Editor.KeyMembersInfo[0].MemberType : null).ConvertToString(key);
			}
			return ActionEventArgumentPrefix + action.Id + ":" + HttpUtility.UrlEncode(stringKey);
		}
		private string GetRowClickScript() {
			bool isPostBackRequired = GetPostBackRequired(ProcessSelectedItemAction);
			Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), gridControl.Page.GetType(), "Page");
			string callbackScript = ((ICallbackManagerHolder)gridControl.Page).CallbackManager.GetScript(gridControl.UniqueID, "ActionArg", "Cnfrm", isPostBackRequired).Replace("'", "\\'");
			string scriptBody = string.Format("function(s, e) {{ if(!s.XafGridViewUpdateWatcherHelper || s.XafGridViewUpdateWatcherHelper.CanProcessCallbackForGridView(s, e)) {{ ProcessGridRowClick(s, e.visibleIndex, e.htmlEvent, '{0}'); }} }}", callbackScript);
			return scriptBody;
		}
		private void gridControl_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e) {
			if(e.RowType == GridViewRowType.Data) {
				object itemObject = Editor.GetRowObject(e.VisibleIndex);
				bool isActive = GetRowActionActivity(ProcessSelectedItemAction, itemObject);
				if(isActive) {
					e.Row.Attributes["Cnfrm"] = HttpUtility.HtmlEncode(GetConfirmationMessage(ProcessSelectedItemAction, itemObject));
					e.Row.Attributes["ActionArg"] = GetPostBackArgument(ProcessSelectedItemAction, e.VisibleIndex);
				}
				else if(TestScriptsManager.EasyTestEnabled) {
					e.Row.Attributes["IsClickDisabled"] = "true";
				}
				e.Row.Attributes["onmouseenter"] = "this.className += ' over'";
				e.Row.Attributes["onmouseleave"] = "this.className = this.className.replace(' over', '')";
				for(int i = 0; i < e.Row.Cells.Count; i++) {
					TableCell cell = e.Row.Cells[i];
					if(cell is GridViewTableCommandCell) {
						cell.Attributes["IsCommand"] = "true";
					}
					if(cell is GridViewTableDetailButtonCell || cell is GridViewTableAdaptiveDetailButtonCell) {
						cell.Attributes["IsDetail"] = "true";
					}
				}
			}
		}
		private void gridControl_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e) {
			ProcessCustomCallback(e.Parameters);
		}
		private void ProcessCustomCallback(string parameters) {
			if(parameters.StartsWith("XafAction:")) {
				ProcessCommand(parameters);
			}
		}
		private void SetNewItemRowPosition(GridViewCommandColumn editColumn) {
			if(Editor.NewItemRowPosition != NewItemRowPosition.None) {
				editColumn.ShowNewButton = true;
			}
			else {
				editColumn.ShowNewButton = false;
			}
		}
		internal void UpdateNewItemRowPosition() {
			if(editColumn != null) {
				SetNewItemRowPosition(editColumn);
				UpdateEditColumnVisability();
			}
		}
		private void UpdateEditColumnVisability() {
			if(Editor.IsBatchMode) {
				editColumn.Visible = editColumn.ShowEditButton || editColumn.ShowNewButton;
			}
		}
		public ASPxGridViewContextMenu(ASPxGridListEditor editor)
			: base(editor) {
		}
		public override void Dispose() {
			try {
				foreach(WebContextMenuActionContainer container in Containers) {
					container.Dispose();
				}
				BreakLinksToControls();
			}
			finally {
				base.Dispose();
			}
		}
		private bool IsContextMenuActual() {
			List<ActionBase> expectedActionsData = new List<ActionBase>();
			List<ActionBase> actualActionsData = new List<ActionBase>();
			foreach(WebContextMenuActionContainer container in Containers) {
				foreach(ActionBase action in container.Actions) {
					if(IsActionVisibleInContextMenu(action)) {
						expectedActionsData.Add(action);
					}
				}
			}
			int expectedCommandColumnsCount = 0;
			int actualCommandColumnsCount = 0;
			if(Editor.AllowEdit && FindInlineEditAction() != null) {
				expectedCommandColumnsCount++;
			}
			if(Editor.CanSelectRows) {
				expectedCommandColumnsCount++;
			}
			foreach(GridViewColumn column in Editor.Grid.Columns) {
				if(column is IActionHolder) {
					if(expectedActionsData.Contains(((IActionHolder)column).Action)) {
						actualActionsData.Add(((IActionHolder)column).Action);
					}
					else {
						return false;
					}
				}
				else if(column is XafGridViewCommandColumn) {
					actualCommandColumnsCount++;
				}
			}
			if(expectedCommandColumnsCount != actualCommandColumnsCount) {
				return false;
			}
			foreach(ActionBase action in expectedActionsData) {
				if(!actualActionsData.Contains(action)) {
					return false;
				}
			}
			return true;
		}
		private void CreateActionColumns() {
			actionColumns = new List<GridViewColumn>();
			foreach(WebContextMenuActionContainer container in Containers) {
				foreach(ActionBase action in container.Actions) {
					if(IsActionVisibleInContextMenu(action)) {
						GridViewColumn column;
						if(action is ActionUrl) {
							column = new GridViewDataActionUrlColumn((ActionUrl)action);
						}
						else if(action is SimpleAction || action is PopupWindowShowAction) {
							column = new GridViewDataActionColumn(action);
						}
						else {
							continue;
						}
						column.Name = action.Id;
						actionColumns.Add(column);
					}
				}
			}
			for(int i = actionColumns.Count - 1; i >= 0; i--) {
				gridControl.Columns.Add(actionColumns[i]);
				actionColumns[i].VisibleIndex = 0;
			}
		}
		public override void CreateControlsCore() {
			if(gridControl != null && IsContextMenuActual()) {
				return;
			}
			UnlinkGrid();
			gridControl = Editor.Grid;
			gridControl.SettingsContextMenu.Enabled = true;
			gridControl.SettingsContextMenu.EnableRowMenu = DevExpress.Utils.DefaultBoolean.False;
			gridControl.HtmlDataCellPrepared += new ASPxGridViewTableDataCellEventHandler(gridControl_HtmlDataCellPrepared);
			gridControl.HtmlCommandCellPrepared += gridControl_HtmlCommandCellPrepared;
			gridControl.CommandButtonInitialize += new ASPxGridViewCommandButtonEventHandler(gridControl_CommandButtonInitialize);
			gridControl.CustomCallback += new ASPxGridViewCustomCallbackEventHandler(gridControl_CustomCallback);
			gridControl.HtmlRowPrepared += new ASPxGridViewTableRowEventHandler(gridControl_HtmlRowPrepared);
			gridControl.Load += new EventHandler(gridControl_Load);
			if(Editor.DisplayActionColumns) {
				CreateActionColumns();
			}
			if(Editor.AllowEdit && FindInlineEditAction() != null) {
				editColumn = new XafGridViewCommandColumn();
				editColumn.Width = new Unit(60);
				editColumn.AllowDragDrop = DevExpress.Utils.DefaultBoolean.False;
				gridControl.Styles.CommandColumnItem.Cursor = "pointer";
				editColumn.ShowEditButton = true;
				editColumn.ShowUpdateButton = true;
				editColumn.ShowCancelButton = true;
				if(Editor.IsBatchMode) {
					editColumn.ShowEditButton = false;
					editColumn.ShowDeleteButton = true;
				}
				ASPxImageHelper.SetImageProperties(gridControl.SettingsCommandButton.EditButton.Image, "Action_Inline_Edit");
				ASPxImageHelper.SetImageProperties(gridControl.SettingsCommandButton.CancelButton.Image, "Action_Cancel");
				ASPxImageHelper.SetImageProperties(gridControl.SettingsCommandButton.UpdateButton.Image, "Action_Save");
				ASPxImageHelper.SetImageProperties(gridControl.SettingsCommandButton.NewButton.Image, "Action_Inline_New");
				ASPxImageHelper.SetImageProperties(gridControl.SettingsCommandButton.DeleteButton.Image, "Action_Delete");
				bool hasImages = !gridControl.SettingsCommandButton.EditButton.Image.IsEmpty && !gridControl.SettingsCommandButton.CancelButton.Image.IsEmpty && !gridControl.SettingsCommandButton.UpdateButton.Image.IsEmpty;
				editColumn.ButtonType = hasImages ? CommandButtonType : GridCommandButtonRenderMode.Link;
				editColumn.Name = InlineEditCommandColumnName;
				editColumn.Caption = " ";
				gridControl.Columns.Add(editColumn);
				editColumn.VisibleIndex = 0;
				UpdateNewItemRowPosition();
			}
			if(Editor.CanSelectRows) {
				selectColumn = new XafGridViewCommandColumn();
				selectColumn.Name = SelectionCommandColumnName;
				selectColumn.Caption = " ";
				selectColumn.ShowSelectCheckbox = true;
				selectColumn.Width = Unit.Pixel(30);
				selectColumn.AllowDragDrop = DevExpress.Utils.DefaultBoolean.False;
				selectColumn.SelectAllCheckboxMode = GridViewSelectAllCheckBoxMode.AllPages;
				gridControl.Columns.Add(selectColumn);
				selectColumn.VisibleIndex = 0;
			}
			gridControl.JSProperties["cpCanSelectRows"] = Editor.CanSelectRows;
		}
		void gridControl_Load(object sender, EventArgs e) {
			if(ProcessSelectedItemAction != null) {
				gridControl.ClientSideEvents.RowClick = GetRowClickScript();
			}
			ICallbackManagerHolder holder = gridControl.Page as ICallbackManagerHolder;
			if(holder != null) {
				holder.CallbackManager.RegisterHandler(gridControl.UniqueID, this);
			}
			gridControl.ClientSideEvents.SelectionChanged = @"function(s, e) { if(!e.isChangedOnServer) { s.gridCallBack([ASPxClientGridViewCallbackCommand.Selection]); } }";
		}
		public static string GetCommandName(ActionBase action) {
			return action.Id + action.GetHashCode();
		}
		public override void BreakLinksToControls() {
			UnlinkGrid();
		}
		public new ASPxGridListEditor Editor {
			get { return (ASPxGridListEditor)base.Editor; }
		}
		public GridCommandButtonRenderMode CommandButtonType {
			get { return commandButtonType; }
			set { commandButtonType = value; }
		}
		#region IXafCallbackHandler Members
		void IXafCallbackHandler.ProcessAction(string parameter) {
			ProcessCustomCallback(parameter);
		}
		#endregion
	}
}
