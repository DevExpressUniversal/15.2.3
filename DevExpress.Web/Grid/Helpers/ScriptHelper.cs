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

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Rendering {
	public abstract class ASPxGridScripts {
		public ASPxGridScripts(ASPxGridBase grid) {
			Grid = grid;
		}
		public ASPxGridBase Grid { get; private set; }
		protected string Name { get { return Grid.ClientID; } }
		protected bool IsDisabled { get { return !Grid.IsEnabled(); } }
		public virtual string GetAccessibleSortClick(int columnIndex) {
			return string.Format("ASPx.GSort('{0}',{1})", Name, columnIndex);
		}
		public string GetMainTableClickFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("ASPx.GTableClick('{0}', event);", Name);
		}
		public string GetMainTableDblClickFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("ASPx.GVTableDblClick('{0}', event);", Name);
		}
		public string GetHeaderColumnMouseDown() {
			if(IsDisabled) return string.Empty;
			return string.Format("ASPx.GHeaderMouseDown('{0}', this, event);", Name);
		}
		public string GetPagerOnClickFunction(string id) {
			if(IsDisabled) return string.Empty;
			return string.Format("ASPx.GVPagerOnClick('{0}','{1}');", Name, id);
		}
		public string GetPagerOnPageSizeChange() {
			if(IsDisabled) return string.Empty;
			return String.Format("function(s, e) {{ ASPx.GVPagerOnClick('{0}',e.value); }}", Name);
		}
		public string GetCustomizationWindowCloseUpHandler() {
			if(IsDisabled) return string.Empty;
			return string.Format("function(s, event) {{ ASPx.GVCustWindowCloseUp('{0}'); }}", Name);
		}
		public string GetPopupEditFormOnInitFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("function(s, e) {{ ASPx.GVPopupEditFormOnInit('{0}', s); }}", Name);
		}
		public string GetHFInitHandler(int columnIndex) {
			return string.Format("function(s){{ASPx.GHFInit('{0}',{1});}}", Name, columnIndex);
		}
		public string GetHFOkButtonClickHandler() {
			return string.Format("function(){{ASPx.GVApplyFilterPopup('{0}');}}", Name);
		}
		public string GetHFCancelButtonClickHandler() {
			return string.Format("function(){{ASPx.GVHFCancelButtonClick('{0}');}}", Name);
		}
		public string GetShowFilterControl() {
			return string.Format("ASPx.GVShowFilterControl('{0}');", Name);
		}
		public string GetApplyFilterControl() {
			return string.Format("ASPx.GVApplyFilterControl('{0}');", Name);
		}
		public string GetCloseFilterControl() {
			return string.Format("ASPx.GVCloseFilterControl('{0}');", Name);
		}
		public string GetSetFilterEnabledForCheckBox() {
			return string.Format("ASPx.GVSetFilterEnabled('{0}', this.checked);", Name);
		}
		public string GetClearFilterFunction() {
			return GetScheduledCommandHandler(GetClearFilterFuncArgs(string.Empty, -1));
		}
		public string GetClosePopupEditFormFunction() {
			return string.Format("function(s,e){{if(e.closeReason === ASPxClientPopupControlCloseReason.Escape || e.closeReason === ASPxClientPopupControlCloseReason.CloseButton){0};}}",
				GetScheduledCommandHandler(GetCancelEditFuncArgs(string.Empty, -1), false, true));
		}
		public string GetScheduledCommandHandler(object[] args) {
			return GetScheduledCommandHandler(args, false);
		}
		public string GetScheduledCommandHandler(object[] args, bool postponed) {
			return GetScheduledCommandHandler(args, postponed, false);
		}
		public string GetScheduledCommandHandler(object[] args, bool postponed, bool usedAsInnerFunc) {
			if(IsDisabled) return string.Empty;
			var format = "ASPx.GVScheduleCommand({0},{1},{2})";
			if((Grid.RenderHelper.UseEndlessPaging || Grid.RenderHelper.AllowBatchEditing) && !usedAsInnerFunc)
				format = "ASPx.GVScheduleCommand({0},{1},{2},event)";
			return string.Format(format, HtmlConvertor.ToScript(Name), HtmlConvertor.ToJSON(args), HtmlConvertor.ToScript(postponed ? 1 : 0));
		}
		public object[] GetAddNewRowFuncArgs(string id, int visibleIndex) {
			return new object[] { "AddNew" };
		}
		public object[] GetDeleteRowFuncArgs(string id, int visibleIndex) {
			return new object[] { "Delete", visibleIndex };
		}
		public object[] GetStartEditFuncArgs(string id, int visibleIndex) {
			return new object[] { "StartEdit", visibleIndex };
		}
		public object[] GetUpdateEditFuncArgs(string id, int visibleIndex) {
			return new object[] { "UpdateEdit" };
		}
		public object[] GetCancelEditFuncArgs(string id, int visibleIndex) {
			return new object[] { "CancelEdit" };
		}
		public object[] GetSelectRowFuncArgs(string id, int visibleIndex) {
			return new object[] { "Select", visibleIndex };
		}
		public object[] GetCustomButtonFuncArgs(string id, int visibleIndex) {
			return new object[] { "CustomButton", id, visibleIndex };
		}
		public object[] GetApplySearchFilterFuncArgs(string id, int visibleIndex) {
			return new object[] { "ApplySearchFilter" };
		}
		public object[] GetClearSearchFilterFuncArgs(string id, int visibleIndex) {
			return new object[] { "ClearSearchFilter" };
		}
		public object[] GetClearFilterFuncArgs(string id, int visibleIndex) {
			return new object[] { "ClearFilter" };
		}
		public object[] GetShowAdaptiveDetailFuncArgs(string id, int visibleIndex) {
			return new object[] { "ShowAdaptiveDetail", visibleIndex };
		}
		public object[] GetHideAdaptiveDetailFuncArgs(string id, int visibleIndex) {
			return new object[] { "HideAdaptiveDetail", visibleIndex };
		}
		public string GetUpdateEditFunction() {
			return GetScheduledCommandHandler(GetUpdateEditFuncArgs(string.Empty, -1));
		}
		public string GetCancelEditFunction() {
			return GetScheduledCommandHandler(GetCancelEditFuncArgs(string.Empty, -1));
		}
	}
	public class ASPxGridViewScripts : ASPxGridScripts {
		public ASPxGridViewScripts(ASPxGridView grid)
			: base(grid) {
		}
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public string GetContextMenu() {
			if(IsDisabled || !Grid.RenderHelper.RequireRenderContextMenu && string.IsNullOrEmpty(Grid.ClientSideEvents.ContextMenu))
				return string.Empty;
			return string.Format("return ASPx.GVContextMenu('{0}',event);", Name);
		}
		public string GetExpandRowFunction(int visibleIndex) {
			if(IsDisabled) return string.Empty;
			string evt = Grid.IsAccessibilityCompliantRender() ? string.Empty : ",event";
			return string.Format("ASPx.GVExpandRow('{0}',{1}{2})", Name, visibleIndex, evt);
		}
		public string GetCollapseRowFunction(int visibleIndex) {
			if(IsDisabled) return string.Empty;
			string evt = Grid.IsAccessibilityCompliantRender() ? string.Empty : ",event";
			return string.Format("ASPx.GVCollapseRow('{0}',{1}{2})", Name, visibleIndex, evt);
		}
		public string GetShowDetailRowFunction(int visibleIndex) {
			if(IsDisabled) return string.Empty;
			string evt = Grid.IsAccessibilityCompliantRender() ? string.Empty : ",event";
			return string.Format("ASPx.GVShowDetailRow('{0}',{1}{2})", Name, visibleIndex, evt);
		}
		public string GetHideDetailRowFunction(int visibleIndex) {
			if(IsDisabled) return string.Empty;
			string evt = Grid.IsAccessibilityCompliantRender() ? string.Empty : ",event";
			return string.Format("ASPx.GVHideDetailRow('{0}',{1}{2})", Name, visibleIndex, evt);
		}
		public string GetFilterOnKeyPressFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("function(s, event) {{ ASPx.GVFilterKeyPress('{0}',s,event); }}", Name);
		}
		public string GetFilterOnSpecKeyPressFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("function(s, event) {{ ASPx.GVFilterSpecKeyPress('{0}', s, event); }}", Name);
		}
		public string GetFilterOnChangedFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("function(s, event) {{ ASPx.GVFilterChanged('{0}',s); }}", Name);
		}
		public string GetShowParentRowsWindowFunction() {
			if(IsDisabled) return string.Empty;
			return string.Format("ASPx.GVShowParentRows('{0}', event, this);", Name);
		}
		public string GetHideParentRowsWindowFunction(bool always) {
			if(IsDisabled) return string.Empty;
			string evt = always ? string.Empty : ", event";
			return string.Format("ASPx.GVShowParentRows('{0}'{1});", Name, evt);
		}
		public string GetFilterRowMenuImageClick(int columnIndex) {
			return string.Format("ASPx.GVFilterRowMenu('{0}',{1},this)", Name, columnIndex);
		}
		public string GetFilterRowMenuItemClick() {
			return string.Format("function(s,e){{ASPx.GVFilterRowMenuClick('{0}',e)}}", Name);
		}		
		public string GetContextMenuItemClick() {
			return string.Format("function(s,e){{ASPx.GVContextMenuItemClick('{0}',e)}}", Name);
		}
		public string GetSelectRowFunction(int visibleIndex) {
			return GetScheduledCommandHandler(GetSelectRowFuncArgs(string.Empty, visibleIndex), true);
		}
		public string GetApplyFilterFunction() {
			return GetScheduledCommandHandler(GetApplyOnClickRowFilterFuncArgs(string.Empty, -1));
		}
		public object[] GetApplyOnClickRowFilterFuncArgs(string id, int visibleIndex) {
			return new object[] { "ApplyMultiColumnAutoFilter" };
		}
	}
}
