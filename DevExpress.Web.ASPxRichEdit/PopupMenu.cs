#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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

using DevExpress.Web.ASPxRichEdit.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxRichEdit {
	public class RichEditPopupMenuItemCollection : Collection<RichEditPopupMenuItem> {
		protected internal RichEditPopupMenuItemCollection()
			: base() {
		}
		public RichEditPopupMenuItem Add() {
			return AddInternal(new RichEditPopupMenuItem());
		}
		public RichEditPopupMenuItem Add(string text, string commandName) {
			return AddInternal(new RichEditPopupMenuItem(text, commandName));
		}
		protected RichEditPopupMenuItem[] CreateDesktopMenuItems() {
			List<RichEditPopupMenuItem> list = new List<RichEditPopupMenuItem>();
			list.Add(CreateItem(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.OpenHyperlink), ((int)RichEditClientCommand.OpenHyperlink).ToString(), true));
			list.Add(CreateItem(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.EditHyperlink), ((int)RichEditClientCommand.ShowEditHyperlinkForm).ToString(), false, RichEditIconImages.Hyperlink));
			list.Add(CreateItem(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RemoveHyperlink), ((int)RichEditClientCommand.RemoveHyperlink).ToString(), false, RichEditIconImages.Delete_Hyperlink));
			list.Add(CreateItem(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UpdateField), ((int)RichEditClientCommand.UpdateField).ToString(), true, RichEditIconImages.UpdateField));
			list.Add(CreateItem(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ToggleFieldCodes), ((int)RichEditClientCommand.ToggleFieldCodes).ToString(), false, RichEditIconImages.ToggleFieldCodes));
			list.Add(CreateItem(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RestartNumbering), ((int)RichEditClientCommand.RestartNumberingList).ToString(), true, RichEditIconImages.RestartNumbering));
			list.Add(CreateItem(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ContinueNumbering), ((int)RichEditClientCommand.ContinueNumberingList).ToString(), true, RichEditIconImages.ContinueNumbering));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_CutSelection), ((int)RichEditClientCommand.CutSelection).ToString(), true, RichEditIconImages.Cut));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_CopySelection), ((int)RichEditClientCommand.CopySelection).ToString(), RichEditIconImages.Copy));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_Paste), ((int)RichEditClientCommand.PasteSelection).ToString(), RichEditIconImages.Paste));
			RichEditPopupMenuItem insertTableElementItem = CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableElement), ((int)RichEditClientCommand.ContextItem_Tables).ToString(), true);
			insertTableElementItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableColumnToTheLeft), ((int)RichEditClientCommand.InsertTableColumnToTheLeft).ToString(), RichEditIconImages.InsertTableColumnsToTheLeft));
			insertTableElementItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableColumnToTheRight), ((int)RichEditClientCommand.InsertTableColumnToTheRight).ToString(), RichEditIconImages.InsertTableColumnsToTheRight));
			insertTableElementItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableRowAbove), ((int)RichEditClientCommand.InsertTableRowAbove).ToString(), RichEditIconImages.InsertTableRowsAbove));
			insertTableElementItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableRowBelow), ((int)RichEditClientCommand.InsertTableRowBelow).ToString(), RichEditIconImages.InsertTableRowsBelow));
			insertTableElementItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableCells), ((int)RichEditClientCommand.ShowInsertTableCellsForm).ToString(), RichEditIconImages.InsertTableCells));
			list.Add(insertTableElementItem);
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableCells), ((int)RichEditClientCommand.ShowDeleteTableCellsForm).ToString()));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SplitTableCellsMenuItem), ((int)RichEditClientCommand.ShowSplitTableCellsForm).ToString(), RichEditIconImages.SplitTableCells));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_MergeTableCells), ((int)RichEditClientCommand.MergeTableCells).ToString(), RichEditIconImages.MergeTableCells));
			RichEditPopupMenuItem tableCellsAlignmentItem = CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeTableCellsContentAlignment), ((int)RichEditClientCommand.ContextItem_Tables).ToString());
			tableCellsAlignmentItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopLeftAlignment), ((int)RichEditClientCommand.TableCellAlignTopLeft).ToString(), true, RichEditIconImages.AlignTopLeft, "tableCellAlign"));
			tableCellsAlignmentItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopCenterAlignment), ((int)RichEditClientCommand.TableCellAlignTopCenter).ToString(), RichEditIconImages.AlignTopCenter, "tableCellAlign"));
			tableCellsAlignmentItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopRightAlignment), ((int)RichEditClientCommand.TableCellAlignTopRight).ToString(), RichEditIconImages.AlignTopRight, "tableCellAlign"));
			tableCellsAlignmentItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleLeftAlignment), ((int)RichEditClientCommand.TableCellAlignMiddleLeft).ToString(), true, RichEditIconImages.AlignMiddleLeft, "tableCellAlign"));
			tableCellsAlignmentItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleCenterAlignment), ((int)RichEditClientCommand.TableCellAlignMiddleCenter).ToString(), RichEditIconImages.AlignMiddleCenter, "tableCellAlign"));
			tableCellsAlignmentItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleRightAlignment), ((int)RichEditClientCommand.TableCellAlignMiddleRight).ToString(), RichEditIconImages.AlignMiddleRight, "tableCellAlign"));
			tableCellsAlignmentItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomLeftAlignment), ((int)RichEditClientCommand.TableCellAlignBottomLeft).ToString(), true, RichEditIconImages.AlignBottomLeft, "tableCellAlign"));
			tableCellsAlignmentItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomCenterAlignment), ((int)RichEditClientCommand.TableCellAlignBottomCenter).ToString(), RichEditIconImages.AlignBottomCenter, "tableCellAlign"));
			tableCellsAlignmentItem.Items.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomRightAlignment), ((int)RichEditClientCommand.TableCellAlignBottomRight).ToString(), RichEditIconImages.AlignBottomRight, "tableCellAlign"));
			list.Add(tableCellsAlignmentItem);
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowTablePropertiesFormMenuItem), ((int)RichEditClientCommand.ShowTablePropertiesForm).ToString(), RichEditIconImages.TableProperties));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DecrementIndent), ((int)RichEditClientCommand.DecreaseIndent).ToString(), true, RichEditIconImages.IndentDecrease));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_IncrementIndent), ((int)RichEditClientCommand.IncreaseIndent).ToString(), RichEditIconImages.IndentIncrease));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowFontForm), ((int)RichEditClientCommand.ShowFontForm).ToString(), true, RichEditIconImages.Font));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowParagraphForm), ((int)RichEditClientCommand.ShowParagraphForm).ToString(), RichEditIconImages.Paragraph));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowNumberingList), ((int)RichEditClientCommand.ShowNumberingListForm).ToString(), RichEditIconImages.ListBullets));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_Bookmark), ((int)RichEditClientCommand.ShowBookmarkForm).ToString(), true, RichEditIconImages.Bookmark));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_Hyperlink), ((int)RichEditClientCommand.ShowCreateHyperlinkForm).ToString(), RichEditIconImages.Hyperlink));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectAll), ((int)RichEditClientCommand.SelectAll).ToString(), true, RichEditIconImages.SelectAll));
			return list.ToArray();
		}
		protected RichEditPopupMenuItem[] CreateTouchMenuItems() {
			List<RichEditPopupMenuItem> list = new List<RichEditPopupMenuItem>();
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_CutSelection), ((int)RichEditClientCommand.CutSelection).ToString(), true, RichEditIconImages.Cut));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_CopySelection), ((int)RichEditClientCommand.CopySelection).ToString(), RichEditIconImages.Copy));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_Paste), ((int)RichEditClientCommand.PasteSelection).ToString(), RichEditIconImages.Paste));
			list.Add(CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowNumberingList), ((int)RichEditClientCommand.ShowNumberingListForm).ToString(), RichEditIconImages.ListBullets));
			list.Add(CreateItem(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.OpenHyperlink), ((int)RichEditClientCommand.OpenHyperlink).ToString(), true));
			return list.ToArray();
		}
		public void CreateDefaultItems() {
			Add(RenderUtils.Browser.Platform.IsTouchUI && !RenderUtils.Browser.Platform.IsMSTouchUI ? CreateTouchMenuItems() : CreateDesktopMenuItems());
		}
		protected RichEditPopupMenuItem CreateItem(string text, string commandName) {
			return CreateItem(text, commandName, false, "");
		}
		protected RichEditPopupMenuItem CreateItem(string text, string commandName, bool beginGroup) {
			return CreateItem(text, commandName, beginGroup, "");
		}
		protected RichEditPopupMenuItem CreateItem(string text, string commandName, string imageName) {
			return CreateItem(text, commandName, false, imageName);
		}
		protected RichEditPopupMenuItem CreateItem(string text, string commandName, bool beginGroup, string imageName) {
			return CreateItem(text, commandName, beginGroup, imageName, "");
		}
		protected RichEditPopupMenuItem CreateItem(string text, string commandName, string imageName, string checkedGroupName) {
			return CreateItem(text, commandName, false, imageName, checkedGroupName);
		}
		protected RichEditPopupMenuItem CreateItem(string text, string commandName, bool beginGroup, string imageName, string checkedGroupName) {
			RichEditPopupMenuItem item = new RichEditPopupMenuItem(text, commandName);
			item.BeginGroup = beginGroup;
			item.ImageName = imageName;
			item.CheckedGroupName = checkedGroupName;
			return item;
		}
	}
	public class RichEditPopupMenuItem : CollectionItem {
		private List<RichEditPopupMenuItem> items = new List<RichEditPopupMenuItem>();
		public RichEditPopupMenuItem() { }
		public RichEditPopupMenuItem(string text) : this(text, string.Empty) { }
		public RichEditPopupMenuItem(string text, string commandName) : this() {
			Text = text;
			CommandName = commandName;
		}
		public string Text
		{
			get { return GetStringProperty("Text", ""); }
			set { SetStringProperty("Text", "", value); }
		}
		public string CommandName
		{
			get { return GetStringProperty("CommandName", ""); }
			set { SetStringProperty("CommandName", "", value); }
		}
		public string ImageName
		{
			get { return GetStringProperty("ImageName", ""); }
			set { SetStringProperty("ImageName", "", value); }
		}
		public bool BeginGroup
		{
			get { return GetBoolProperty("BeginGroup", false); }
			set { SetBoolProperty("BeginGroup", false, value); }
		}
		public string CheckedGroupName {
			get { return GetStringProperty("CheckedGroupName", ""); }
			set { SetStringProperty("CheckedGroupName", "", value); }
		}
		public List<RichEditPopupMenuItem> Items {
			get { return items; }
		}
		public bool Visible
		{
			get { return GetVisible(); }
			set { SetVisible(value); }
		}
		public override string ToString() {
			return Text;
		}
	}
}
