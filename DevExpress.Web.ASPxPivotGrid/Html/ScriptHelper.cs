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
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using System.Web.UI;
using System.Globalization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxPivotGrid.Html {
	public static class CallbackCommands {
		public const char ArgumentsSeparator = '|';
		public const char ExpandChangedChar = 'E';
		public const string ExpandColumnChanged = "EC";
		public const string ExpandRowChanged = "ER";
		public const string ExpandFieldChanged = "EA";
		public const string CollapseFieldChanged = "CA";
		public const string FilterChanged = "F";
		public const string DeferredFilterChanged = "DF";
		public const string FilterShowWindow = "FS";
		public const string FilterChildrenRetrieved = "FC";
		public const string HeaderDrag = "D";
		public const string HeaderSort = "S";
		public const string HeaderSortBySummaryChanged = "SS";
		public const string HideField = "H";
		public const string Pager = "P";
		public const string VirtualScrolling = "VS";
		public const string CustomCallback = "C";
		public const string GroupExpanded = "G";
		public const string ShowPrefilter = "PREFILTER";
		public const string ReloadDataCallback = "RELOAD";
		public const string SortAZ = "SAZ";
		public const string SortZA = "SZA";
		public const string ClearSort = "CS";
		public const string FieldListDefer = "FL";
		public static string GetPostBackId(string eventArgument) {
			int separatorIndex = eventArgument.IndexOf(CallbackCommands.ArgumentsSeparator);
			return separatorIndex >= 0 ? eventArgument.Substring(0, separatorIndex) : eventArgument;
		}
		internal static bool GetIsVirtualScrollingAction(string postBackId) {
			return string.Equals(postBackId, CallbackCommands.VirtualScrolling);
		}
		internal static bool GetIsFilterShowWindowAction(string postBackId) {
			return string.Equals(postBackId, CallbackCommands.FilterShowWindow);
		}
		internal static bool GetIsDefereFilterAction(string postBackId) {
			return string.Equals(postBackId, CallbackCommands.DeferredFilterChanged);
		}
		internal static bool GetIsFilterChildrenRetrievedAction(string postBackId) {
			return string.Equals(postBackId, CallbackCommands.FilterChildrenRetrieved);
		}
		public static PivotGridPostBackActionBase CreatePostBackAction(ASPxPivotGrid pivotGrid, string postbackId, string eventArgument) {
			switch(postbackId) {
				case CallbackCommands.ExpandColumnChanged:
				case CallbackCommands.ExpandRowChanged:
					return new PivotGridPostbackExpandedItemAction(pivotGrid, eventArgument);
				case CallbackCommands.FilterChanged:
					return new PivotGridPostBackFilterFieldAction(pivotGrid, eventArgument);
				case CallbackCommands.DeferredFilterChanged:
					return new PivotGridDefereFieldFilterAction(pivotGrid, eventArgument);
				case CallbackCommands.HeaderDrag:
					return new PivotGridPostBackDragFieldAction(pivotGrid, eventArgument);
				case CallbackCommands.FieldListDefer:
					return new PivotGridPostBackFieldListDeferAction(pivotGrid, eventArgument);
				case CallbackCommands.HeaderSort:
					return new PivotGridPostBackFieldSortAction(pivotGrid, eventArgument);
				case CallbackCommands.HeaderSortBySummaryChanged:
					return new PivotGridPostBackSortByColumnAction(pivotGrid, eventArgument);
				case CallbackCommands.HideField:
					return new PivotGridPostBackHideFieldAction(pivotGrid, eventArgument);
				case CallbackCommands.GroupExpanded:
					return new PivotGridPostBackChangeGroupExpandedAction(pivotGrid, eventArgument);
				case CallbackCommands.CustomCallback:
					return new PivotGridCustomPostbackAction(pivotGrid, eventArgument.Substring(2));
				case CallbackCommands.Pager:
					return new PivotGridPostBackPagerAction(pivotGrid, eventArgument);
				case CallbackCommands.ShowPrefilter:
					return new PivotGridPostBackPrefilterAction(pivotGrid, eventArgument);
				case CallbackCommands.ReloadDataCallback:
					return null;
				case CallbackCommands.SortAZ:
				case CallbackCommands.SortZA:
				case CallbackCommands.ClearSort:
					return new PivotGridPostBackSortModeNoneAction(pivotGrid, eventArgument);
				case CallbackCommands.VirtualScrolling:
					return new PivotGridPostBackVirtualScrollingAction(pivotGrid, eventArgument);
				default:
					return null;
			}
		}
	}
	public static class ElementNames {
		public const string MainElement = "ME";
		public const string FilterPopupContent = "FPC";
		public const string CustomizationFieldsId = "DXCustFields";
		public const string CustomizationFieldsHiddenInputId = "CFP";
		public const string HeaderMenu = "HM";
		public const string FieldValueMenu = "FVM";
		public const string FieldListMenu = "FM";
		public const string FilterPopupWindow = "FPW";
		public const string FilterPopupWindowOKButton = "FPWOK";
		public const string FilterPopupWindowCancelButton = "FPWCA";
		public const string FilterPopupWindowResizer = "FPWR";
		public const string FilterLoadingPanel = "FLP";
		public const string MainTable = "MT";
		public const string MainTD = "MTD";
		public const string AreaCellContainer = "ACC";
		public const string DataHeadersPopup = "DHP";
		public const string DataHeadersPopupCell = "DHPC";
		public const string ScrollMainDiv = "SMDIV";
		public const string HorzScrollableContainer = "HSCCELL";
		public const string HorzScrollTable = "HZST";
		public const string HorzScrollTableDiv = "HZSTDIV";
		public const string HorzScrollSpacerDiv = "HSSDIV";
		public const string HorzScrollDiv = "HZSDIV";
		public const string HorzScrollContentDiv = "HZSCDIV";
		public const string HorzScrollTieRodDiv = "HZSTRDIV";
		public const string PivotTableContainerDiv = "PTCDiv";
		public const string PivotTableCell = "PTCell";
		public const string PivotTable = "PT";
		public const string HorzScrollBarContainerCell = "HSBCCell";
		public const string VertScrollBarContainerCell = "VSBCCell";
		public const string HorzScrollBarRowAreaCell = "HSBRACell";
		public const string VertScrollBarColumnAreaCell = "VSBCACell";
		public const string ScrollBarEdgeCell = "SBECell";
		public const string ScrollBarOuterDiv = "SBODiv";
		public const string ScrollBarInnerDiv = "SBIDiv";
		public const string ColumnValuesScrollableCell = "CVSCell";
		public const string RowValuesScrollableCell = "RVSCell";
		public const string DataCellsScrollableCell = "DCSCell";
		public const string ScrollableCellRootDiv = "SCRootDiv";
		public const string ScrollableCellDecoratorDiv = "SCDecorDiv";
		public const string ScrollableCellViewPortDiv = "SCVPDiv";
		public const string ScrollableCellScrollableDiv = "SCSDiv";
		public const string ScrollableCellDataTable = "SCDTable";
		public const string ScrollableTableColGroup = "CG";
		public const string FieldValueCellContainerDiv = "CDiv";
	}
	public class ScriptHelper {
		const string AreaId = "pgArea";
		const string HeaderId = "pgHeader";
		const string SortedHeaderId = "sorted";
		const string OlapSortModeNoneHeaderId = "osmn";
		const string OlapSortModeNoneSortAZHeaderId = "SAZ";
		const string OlapSortModeNoneSortZAHeaderId = "SZA";
		const string GroupID = "pgGroupHeader";
		const string SecondInGroup = "scig";
		const string DataField = "pgdthdr";
		public static readonly string[] FieldHeaderIdPostfixes = new string[] { "T", "F", "S", "GB" };
		static string GetLastPart(string stringId) {
			int separatorIndex = stringId.LastIndexOf('_');
			return separatorIndex >= 0 ? stringId.Substring(separatorIndex + 1) : stringId;
		}
		static int GetLastNumberIndex(string str, int startPosition) {
			string numbers = "-0123456789";
			int lastNumber = -1;
			for(int i = startPosition; i < str.Length; i++) {
				if(numbers.Contains(str[i].ToString()))
					lastNumber = i;
				else
					break;
			}
			return lastNumber;
		}
		static string GetIDConst(HeaderType headerType) {
			switch(headerType) {
				case HeaderType.Group:
					return GroupID;
				case HeaderType.Area:
					return AreaId;
				case HeaderType.Header:
				default:
					return HeaderId;
			}
		}
		internal static int GetStringIndex(string stringId, HeaderType headerType) {
			if(string.IsNullOrEmpty(stringId))
				return -1;
			stringId = GetLastPart(stringId);
			string idConst = GetIDConst(headerType);
			int index = stringId.IndexOf(idConst);
			int result = -1;
			if(index == -1)
				return result;
			int start = index + idConst.Length;
			int end = GetLastNumberIndex(stringId, start);
			if(int.TryParse(stringId.Substring(start, end + 1 - start), out result))
				return result;
			return -1;
		}
		readonly ASPxPivotGrid owner;
		public ScriptHelper(ASPxPivotGrid owner) {
			this.owner = owner;
		}
		public ASPxPivotGrid Owner { get { return owner; } }
		protected PivotGridWebData Data { get { return Owner.Data; } }
		public string FilterPopupOkButtonClick {
			get {
				return "function(){ASPx.pivotGrid_ApplyFilter('" + Owner.ClientID + "'); }";
			}
		}
		public string FilterPopupCancelButtonClick {
			get {
				return "function(){ASPx.pivotGrid_HideFilter('" + Owner.ClientID + "'); }";
			}
		}
		public string GetHeaderTableID(PivotArea area) {
			return ElementNames.AreaCellContainer + area.ToString();
		}
		public string GetID(PivotFieldItemBase field) {
			if(field.InnerGroupIndex == -1 || field.Area == PivotArea.RowArea && field.Visible)
				return GetHeaderID(field);
			else
				return GetGroupHeaderID(field.Group);
		}
		public string GetHeaderID(PivotFieldItemBase field) {
			string sorted = (field.ShowSortButton || field.CanSortOLAP) && field.Visible ? SortedHeaderId : string.Empty;
			string osmn = (field.IsOLAPSortModeNone && field.Visible && field.CanSortCore) ? OlapSortModeNoneHeaderId : string.Empty;
			StringBuilder res = new StringBuilder();
			if(field.Group != null && field.InnerGroupIndex != 0)
				res.Append(SecondInGroup);
			if(field.IsDataField)
				res.Append(DataField);
			res.Append(osmn);
			if(field.IsOLAPSorted && field.Visible) {
				string osmnOrder = (field.SortOrder == PivotSortOrder.Ascending) ?
					OlapSortModeNoneSortAZHeaderId : OlapSortModeNoneSortZAHeaderId;
				res.Append(osmnOrder);
			}
			res.Append(sorted);
			res.Append(HeaderId);
			res.Append(field.Index.ToString());
			return res.ToString();
		}
		public string GetFullHeaderID(PivotFieldItemBase field, bool isFieldListHeader) {
			return Owner.ClientID + '_'
				+ (isFieldListHeader ? (PivotGridHtmlCustomizationFields.ElementName_ID + "_") : string.Empty)
				+ GetHeaderID(field);
		}
		public string GetGroupHeaderID(PivotGroupItem group) {
			return GroupID + group.Index.ToString();
		}
		public string GetGroupButtonID(PivotFieldItemBase field) {
			return GetHeaderID(field) + FieldHeaderIdPostfixes[3];
		}
		public string GetHeaderTextCellID(PivotFieldItemBase field) {
			return GetHeaderID(field) + FieldHeaderIdPostfixes[0];
		}
		public string GetHeaderFilterCellID(PivotFieldItemBase field) {
			return GetHeaderID(field) + FieldHeaderIdPostfixes[1];
		}
		public string GetHeaderSortCellID(PivotFieldItemBase field) {
			return GetHeaderID(field) + FieldHeaderIdPostfixes[2];
		}
		public string GetGroupHeaderTextCellID(PivotGroupItem group) {
			return GetGroupHeaderID(group) + FieldHeaderIdPostfixes[0];
		}		
		public string GetCustomizationAreaID(PivotArea area) {
			return PivotGridHtmlCustomizationFields.ElementName_ID + "_" + AreaId + ((int)area).ToString();
		}
		public string GetAreaID(PivotArea area) {
			return AreaId + ((int)area).ToString();
		}
		public string GetAreaContainerID(PivotArea area) {
			return area.ToString();
		}
		public bool GetAreaByID(string areaClientId, out PivotArea area) {
			area = PivotArea.ColumnArea;
			int areaIndex = GetStringIndex(areaClientId, HeaderType.Area);
			if(areaIndex < 0 || areaIndex >= Enum.GetValues(typeof(PivotArea)).Length)
				return false;
			area = (PivotArea)areaIndex;
			return true;
		}
		public PivotGridField GetFieldByHeaderID(string name) {
			int index = GetStringIndex(name, HeaderType.Header);
			if(index >= 0)
				return Owner.Fields[index];
			index = GetStringIndex(name, HeaderType.Group);
			if(index >= 0)
				return (PivotGridField)Owner.Groups[index].Fields[0];
			if(name.Contains(GetHeaderID(Data.GetFieldItem(Data.DataField))))
				return Data.DataField;
			return null;
		}
		public string GetCollapsedImageOnClick(PivotFieldValueItem item, bool includeDoubleClick) {
			if (!HasOwnerWithPage && !IsMvcRender) return string.Empty;
			PivotFieldValueItem actualItem = item.IsVisible ? item : Data.VisualItems.GetRowItemByInvisibleItem(item);
			return string.Format("ASPx.pivotGrid_PerformCallback('{0}', this, '{1}');",
				Owner.ClientID, GetCollapsedFieldValueChangeState(actualItem, includeDoubleClick));
		}
		bool HasOwnerWithPage { get { return Owner != null && Owner.Page != null; } }
		bool IsMvcRender { get { return MvcUtils.RenderMode != MvcRenderMode.None; } }
		public string GetCollapsedFieldValueChangeState(PivotFieldValueItem item, bool includeDoubleClick) {
			if(!(includeDoubleClick && item.AllowExpand && item.AllowExpandOnDoubleClick || item.ShowCollapsedButton)) 
				return string.Empty;
			return (item.IsColumn ? CallbackCommands.ExpandColumnChanged : CallbackCommands.ExpandRowChanged) +
				CallbackCommands.ArgumentsSeparator + item.UniqueIndex.ToString();
		}
		public string GetFilterButtonOnClick(PivotGridHeaderTemplateItem item, bool isFieldList) {
			return string.Format("ASPx.pivotGrid_ShowFilterPopup(\"{0}\", \"{1}\", {2});", Owner.ClientID, 
				GetFullHeaderID(item.FieldItem, item.IsFieldListItem), HtmlConvertor.ToScript(isFieldList));
		}
		public string GetPagerOnClick(string id) {
			return string.Format("ASPx.pivotGrid_PagerClick('{0}', this, '{1}');", Owner.ClientID, id);
		}
		public string GetPagerOnPageSizeChange() {
			return string.Format("function(s, e) {{ ASPx.pivotGrid_PagerClick('{0}', this, e.value); }}", Owner.ClientID);
		}
		public string GetGroupButtonOnClick(string id) {
			return string.Format("ASPx.pivotGrid_PerformCallback('{0}', this, '{1}');", Owner.ClientID,
				CallbackCommands.GroupExpanded.ToString() + CallbackCommands.ArgumentsSeparator + id);
		}
		public string GetHeaderMouseDown() {
			return string.Format("ASPx.pivotGrid_HeaderMouseDown('{0}', this, event);", Owner.ClientID);
		}
		public string GetHeaderClick(bool isFieldListItem) {
			if(!isFieldListItem)
				return string.Format("ASPx.pivotGrid_HeaderClick('{0}', this, event);", Owner.ClientID);
			return string.Format("ASPx.pivotGrid_CustFormHeaderClick('{0}', this, event);", Owner.ClientID);
		}
		public string GetCellClick(PivotGridCellItem cellItem) {
			return GetClickArgs("ASPx.pivotGrid_CellClick", cellItem);
		}
		public string GetCellDblClick(PivotGridCellItem cellItem) {
			return GetClickArgs("ASPx.pivotGrid_CellDoubleClick", cellItem);
		}
		public string GetAccessibleSortUrl(PivotFieldItemBase field) {
			return string.Format("javascript:ASPx.pivotGrid_Sort508('{0}','{1}')", Owner.ClientID, GetHeaderID(field));
		}
		string GetClickArgs(string functionName, PivotGridCellItem cellItem) {
			StringBuilder str = new StringBuilder(functionName);
			str.Append("(\'").Append(Owner.ClientID)
				.Append("\',").Append("event")
				.Append(",\'").Append(Convert.ToString(cellItem.Value, CultureInfo.InvariantCulture))
				.Append("\',").Append(cellItem.ColumnIndex)
				.Append(",").Append(Data.VisualItems.GetUnpagedRowIndex(cellItem.RowIndex))
				.Append(",\'").Append(GetClickArgsFieldValueCore(cellItem, true))
				.Append("\',\'").Append(GetClickArgsFieldValueCore(cellItem, false))
				.Append("\',\'").Append(cellItem.ColumnField != null ? FieldToString(cellItem.ColumnField) : "")
				.Append("\',\'").Append(cellItem.RowField != null ? FieldToString(cellItem.RowField) : "")
				.Append("\',\'").Append(cellItem.ColumnValueType.ToString())
				.Append("\',\'").Append(cellItem.RowValueType.ToString())
				.Append("\',").Append(cellItem.DataIndex).Append(");");
			return str.ToString();
		}
		string GetClickArgsFieldValueCore(PivotGridCellItem cellItem, bool isColumn) {
			PivotFieldValueItem valueItem = isColumn ? cellItem.ColumnFieldValueItem : cellItem.RowFieldValueItem;
			object value = valueItem.Value;
			if(valueItem.Field != null && valueItem.Field.Area == PivotArea.DataArea) {
				PivotFieldItemBase actualField = Data.GetFieldItem(Data.GetLastFieldByArea(isColumn));
				value = actualField != null ? cellItem.GetFieldValue(actualField) : null;
			}
			return EscapeString(Convert.ToString(value, CultureInfo.InvariantCulture));
		}
		string FieldToString(PivotFieldItemBase field) {
			return EscapeString(string.IsNullOrEmpty(field.Name) ? field.FieldName : field.Name);
		}
		string EscapeString(string s) {
			return HtmlConvertor.EscapeString(s);
		}
		public string GetDragHeaderCallbackArgsString(PivotGridField field, PivotGridField toField, bool left) {
			StringBuilder sb = new StringBuilder();
			sb.Append(GetDragHeaderCallbackArgsStringBegin(field));
			sb.Append(GetHeaderID(Data.GetFieldItem(toField)));
			sb.Append(CallbackCommands.ArgumentsSeparator);
			sb.Append(left.ToString());
			return sb.ToString();
		}
		string GetDragHeaderCallbackArgsStringBegin(PivotGridField field) {
			StringBuilder sb = new StringBuilder();
			sb.Append(CallbackCommands.HeaderDrag);
			sb.Append(CallbackCommands.ArgumentsSeparator);
			sb.Append(GetHeaderID(Data.GetFieldItem(field)));
			sb.Append(CallbackCommands.ArgumentsSeparator);
			return sb.ToString();
		}
		public string GetDragHeaderCallbackArgsString(PivotGridField field, PivotArea toArea) {
			PivotGridField[] fields = Data.GetFieldsByArea(toArea);
			if(fields.Length > 0)
				return GetDragHeaderCallbackArgsString(field, fields[fields.Length - 1], true);
			StringBuilder sb = new StringBuilder();
			sb.Append(GetDragHeaderCallbackArgsStringBegin(field));
			sb.Append(GetAreaID(toArea));
			return sb.ToString();
		}
	}
}
