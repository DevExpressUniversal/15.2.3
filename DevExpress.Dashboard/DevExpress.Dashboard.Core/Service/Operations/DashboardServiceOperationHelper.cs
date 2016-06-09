#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Server;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.Service {
	public static class DashboardServiceOperationHelper {
		static bool ContentTypeContainsValue(ContentType contentType, ContentType value) {
			return (contentType & value) == value;
		}
		static ActionModel CreateActionModel(DashboardSession session, string itemName) {
			return new ActionModel {
				AffectedItems = GetAllAffectedItems(session, itemName),
				DrillUpButtonState = session.GetDrillUpButtonState(itemName),
				ClearMasterFilterButtonState = session.GetClearMasterFilterButtonState(itemName)
			};
		}
		static Dictionary<ActionType, string[]> GetAllAffectedItems(DashboardSession session, string itemName) {
			Dictionary<ActionType, string[]> affectedItems = new Dictionary<ActionType, string[]>();
			AddAffectedItems(affectedItems, session, itemName, ActionType.SetMasterFilter);
			AddAffectedItems(affectedItems, session, itemName, ActionType.SetMultipleValuesMasterFilter);
			AddAffectedItems(affectedItems, session, itemName, ActionType.ClearMasterFilter);
			AddAffectedItems(affectedItems, session, itemName, ActionType.DrillDown);
			AddAffectedItems(affectedItems, session, itemName, ActionType.DrillUp);
			AddAffectedItems(affectedItems, session, itemName, ActionType.SetSelectedElementIndex);
			AddAffectedItems(affectedItems, session, itemName, ActionType.ExpandValue);
			AddAffectedItems(affectedItems, session, itemName, ActionType.DataRequest);
			return affectedItems;
		}
		static void AddAffectedItems(Dictionary<ActionType, string[]> affectedItemsDict, DashboardSession session, string itemName, ActionType actionType) {
			IList<string> affectedItems = GetAffectedItems(session, itemName, actionType);
			if (affectedItems != null)
				affectedItemsDict.Add(actionType, affectedItems.ToArray());
		}
		static IList<string> GetAffectedItems(DashboardSession session, string itemName, ActionType actionType) {
			switch(actionType) {
				case ActionType.SetMasterFilter:
					return session.GetSetMasterFilterAffectedItems(itemName);
				case ActionType.SetMultipleValuesMasterFilter:
					return session.GetSetMultipleMasterFilterAffectedItems(itemName);
				case ActionType.ClearMasterFilter:
					return session.GetClearMasterFilterAffectedItems(itemName);
				case ActionType.DrillDown:
					return session.GetDrillDownAffectedItems(itemName);
				case ActionType.DrillUp:
					return session.GetDrillUpAffectedItems(itemName);
				case ActionType.SetSelectedElementIndex:
					return session.GetSetSelectedElementIndexAffectedItems(itemName);
				case ActionType.ExpandValue:
					return session.GetExpandValueAffectedItems(itemName);
				case ActionType.DataRequest:
				case ActionType.GetDrillThroughData:
					return new List<string> { itemName }; 
				default:
					throw new Exception(Helper.GetUnknownEnumValueMessage(actionType));
			}
		}
		public static DashboardExportData CreateExportData(DashboardSession session, ExportInfo exportInfo) {
			List<DashboardItemExportData> exportData = new List<DashboardItemExportData>();
			ViewerState viewerState = exportInfo.ViewerState;
			IEnumerable<string> itemNames = viewerState.ItemsState.Keys;
			if(exportInfo.Mode == DashboardExportMode.SingleItem)
				itemNames = itemNames.Take(1);
			session.EnsureDashboardColoringCache();
			DashboardClientData clientData = session.CalculateClientData(itemNames);
			foreach(string itemName in itemNames)
				exportData.Add(CreateItemExportData(session, itemName, clientData));
			DashboardExportData dashboardExportData = new DashboardExportData() {
				ItemsData = exportData,
				TitleLayoutModel = session.GetTitleLayoutViewModel(),
				MasterFilterValues = session.GetMasterFilterValues(exportInfo.GroupName, false).ToList(),
				FontInfo = exportInfo.FontInfo
			};
			UpdateViewerState(dashboardExportData, viewerState);
			return dashboardExportData;
		}
		static DashboardItemExportData CreateItemExportData(DashboardSession session, string itemName,  DashboardClientData clientData) {
			DashboardItemExportData exportData = new DashboardItemExportData();
			exportData.Name = itemName;
			DashboardItemServerData serverData = new DashboardItemServerData();
			serverData.Type = session.GetItemType(itemName);
			serverData.ViewModel = session.GetViewModel(itemName);
			serverData.ConditionalFormattingModel = session.GetConditionalFormattingModel(itemName);
			serverData.DrillDownValues = session.GetDrillDownValues(itemName);
			serverData.FilterValues = session.GetMasterFilterValues(itemName, false).ToList();
			IList<IList> selectedValues = session.GetSelectedValues(itemName).RowListEmptyNull;
			serverData.SelectedValues = (IList)selectedValues;
			serverData.MultiDimensionalData = clientData[itemName];
			serverData.Metadata = session.GetMetadata(itemName);
			exportData.ServerData = serverData;
			IList<object> drillDownValues = session.GetDrillDownUniqueValues(itemName);
			serverData.DrillDownState = drillDownValues != null ?  drillDownValues.ToList() : null;
			return exportData;
		}
		public static void UpdateViewerState(DashboardExportData exportData, ViewerState viewerState) {
			exportData.TitleBounds = new Rectangle(0, 0, viewerState.Size.Width, viewerState.TitleHeight);
			exportData.ClientSize = viewerState.Size;
			foreach(DashboardItemExportData itemExportData in exportData.ItemsData) {
				UpdateItemViewerState(itemExportData, viewerState.ItemsState[itemExportData.Name]);
			}
		}
		public static void UpdateItemViewerState(DashboardItemExportData itemData, ItemViewerClientState itemViewerState) {
			itemData.ViewerClientState = itemViewerState;
		}
		public static List<AffectedItemInfo> GetAffectedItemsInfo(DashboardSession session, string itemName, ActionType actionType) {			
			IList<string> affectedItems = DashboardServiceOperationHelper.GetAffectedItems(session, itemName, actionType);
			if(affectedItems != null) {
				List<AffectedItemInfo> affectedItemsInfo = new List<AffectedItemInfo>();
				if(!affectedItems.Contains(itemName) && (actionType == ActionType.SetMasterFilter || actionType == ActionType.ClearMasterFilter || actionType == ActionType.SetMultipleValuesMasterFilter))
					affectedItemsInfo.Add(new AffectedItemInfo(itemName, ContentType.ActionModel));
				foreach(string affectedItem in affectedItems) {
					AffectedItemInfo info = null;
					if(affectedItem == itemName && actionType == ActionType.ExpandValue)
						info = new AffectedItemInfo(affectedItem, ContentType.PartialDataSource);
					if(affectedItem == itemName && actionType == ActionType.DataRequest)
						info = new AffectedItemInfo(affectedItem, ContentType.CompleteDataSource);
					if(affectedItem == itemName && actionType == ActionType.GetDrillThroughData)
						affectedItemsInfo.Add(new AffectedItemInfo(itemName, ContentType.Empty));
					if(info == null)
						info = new AffectedItemInfo(affectedItem, ContentType.FullContent);
					affectedItemsInfo.Add(info);
				}
				return affectedItemsInfo;
			}
			return null;
		}
		public static List<AffectedItemInfo> GetDataBoundAffectedItemsInfo(DashboardSession session) {
			List<AffectedItemInfo> affectedItemsInfo = new List<AffectedItemInfo>();
			foreach(string itemName in session.GetDataBoundItemNames())
				affectedItemsInfo.Add(new AffectedItemInfo(itemName, ContentType.FullContent));
			return affectedItemsInfo;
		}
		public static DashboardPaneContent CreatePaneContent(DashboardSession session, AffectedItemInfo affectedItemInfo, DashboardClientData clientData, bool ignoreData) {
			string itemName = affectedItemInfo.ItemName;
			ContentType contentType = affectedItemInfo.ContentType;
			DashboardItemViewModel viewModel = null;
			if(ContentTypeContainsValue(contentType, ContentType.ViewModel)) {
				viewModel = session.GetViewModel(itemName);
				if(viewModel != null)
					viewModel.ShouldIgnoreUpdate = ignoreData;
			}
			DashboardItemCaptionViewModel captionViewModel = null;
			if (ContentTypeContainsValue(contentType, ContentType.CaptionViewModel))
				captionViewModel = session.GetCaptionViewModel(itemName);
			ActionModel actionModel = null;
			if(ContentTypeContainsValue(contentType, ContentType.ActionModel))
				actionModel = CreateActionModel(session, itemName);
			ConditionalFormattingModel cfModel = null;
			if(ContentTypeContainsValue(contentType, ContentType.ConditionalFormattingModel))
				cfModel = session.GetConditionalFormattingModel(itemName);
			HierarchicalItemData itemData = null;
			if(ContentTypeContainsValue(contentType, ContentType.CompleteDataSource)) {
				MultidimensionalDataDTO multiDataDTO = clientData[itemName];
				HierarchicalDataParams newDto = multiDataDTO != null ? multiDataDTO.HierarchicalDataParams : null;
				if(newDto != null) {
					itemData = new HierarchicalItemData {
						MetaData = session.GetMetadata(itemName),
						DataStorageDTO = newDto != null ? newDto.Storage.GetDTO() : null,
						SortOrderSlices = newDto != null ? newDto.SortOrderSlices : new string[][] { new string[0] }
					};
				}
			}
			if(ContentTypeContainsValue(contentType, ContentType.PartialDataSource)) {
				MultidimensionalDataDTO multiDataDTO = affectedItemInfo.PartialDataSource;
				HierarchicalDataParams newDto = multiDataDTO != null ? multiDataDTO.HierarchicalDataParams : null;
				itemData = multiDataDTO == null ? null : new HierarchicalItemData {
					DataStorageDTO = newDto != null ? newDto.Storage.GetDTO() : null,
					SortOrderSlices = newDto != null ? newDto.SortOrderSlices : new string[][] { new string[0] }
				};
			}
			IList<FormattableValue> formattableDrillDownValues = null;
			IEnumerable<DimensionFilterValues> drillDownValues = session.GetDrillDownValues(itemName);
			if(drillDownValues != null)
				formattableDrillDownValues = drillDownValues.SelectMany(values => values.Values).ToList();
			return new DashboardPaneContent {
				Name = itemName,
				Type = session.GetItemType(itemName),
				Group = session.GetItemGroup(itemName),
				ContentType = contentType,
				AxisNames = session.GetAxisName(itemName),
				DimensionIds = session.GetDimensionIds(itemName),
				ViewModel = viewModel,
				CaptionViewModel = captionViewModel,
				ConditionalFormattingModel = cfModel,
				ActionModel = actionModel,
				ItemData = itemData,
				SelectedValues = session.GetSelectedValues(itemName).RowListEmptyNull,
				Parameters = affectedItemInfo.Parameters,
				DrillDownValues = formattableDrillDownValues,
				DrillDownUniqueValues = session.GetDrillDownUniqueValues(itemName)
			};
		}
	}
}
