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

using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor.InternalDesigner {
	internal class ServerModeGroupHelper : IServerModeGroupHelperTestable {
		ColumnView view;
		List<ListSourceGroupInfo> firstLevelGroups = new List<ListSourceGroupInfo>();
		private FakeServerModeGroupInfo totalGroupInfo = null;
		public ServerModeGroupHelper(ColumnView view) {
			this.view = view;
		}
		public FakeServerModeGroupInfo TotalGroupInfo {
			get { return totalGroupInfo; }
		}
		public List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup) {
			if(parentGroup == null) {
				return firstLevelGroups;
			}
			else {
				if(parentGroup is FakeServerModeGroupInfo) {
					return ((FakeServerModeGroupInfo)parentGroup).Children;
				}
			}
			return new List<ListSourceGroupInfo>();
		}
		public void Apply(ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo, List<FakeObject> objectsList) {
			firstLevelGroups.Clear();
			totalGroupInfo = null;
			if(view != null) {
				IModelSynchronizersHolder modelSynchronizersHolder = view as IModelSynchronizersHolder;
				if(modelSynchronizersHolder != null) {
					List<IGridColumnModelSynchronizer> groupedColumns = new List<IGridColumnModelSynchronizer>();
					foreach(GridColumn column in view.GroupedColumns) {
						IGridColumnModelSynchronizer columnInfo = modelSynchronizersHolder.GetSynchronizer(column) as IGridColumnModelSynchronizer;
						if(columnInfo != null) {
							groupedColumns.Add(columnInfo);
						}
					}
					PopulateGroups(groupedColumns, groupSummaryInfo, totalSummaryInfo, objectsList);
				}
			}
		}
		private void PopulateGroups(List<IGridColumnModelSynchronizer> groupedColumns, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo, List<FakeObject> objectsList) {
			totalGroupInfo = new FakeServerModeGroupInfo(objectsList, totalSummaryInfo);
			firstLevelGroups.Clear();
			List<ListSourceGroupInfo> currentLevelGroups = new List<ListSourceGroupInfo>();
			int groupLevel = 0;
			foreach(IGridColumnModelSynchronizer columnInfo in groupedColumns) {
				if(firstLevelGroups.Count == 0) {
					foreach(KeyValuePair<string, List<FakeObject>> item in GetGroupedObjects(columnInfo.MemberInfo, objectsList)) {
						FakeServerModeGroupInfo firstLevelGroup = new FakeServerModeGroupInfo(item.Value, groupSummaryInfo);
						firstLevelGroup.Level = groupLevel;
						firstLevelGroup.ChildDataRowCount = item.Value.Count;
						firstLevelGroup.GroupValue = item.Key;
						firstLevelGroups.Add(firstLevelGroup);
						currentLevelGroups.Add(firstLevelGroup);
					}
				}
				else {
					List<ListSourceGroupInfo> parentGroups = new List<ListSourceGroupInfo>(currentLevelGroups);
					currentLevelGroups.Clear();
					foreach(FakeServerModeGroupInfo parentGroup in parentGroups) {
						foreach(KeyValuePair<string, List<FakeObject>> item in GetGroupedObjects(columnInfo.MemberInfo, parentGroup.InnerItems)) {
							FakeServerModeGroupInfo childGroup = new FakeServerModeGroupInfo(item.Value, groupSummaryInfo);
							childGroup.Level = groupLevel;
							childGroup.ChildDataRowCount = item.Value.Count;
							childGroup.GroupValue = item.Key;
							parentGroup.Children.Add(childGroup);
							currentLevelGroups.Add(childGroup);
						}
					}
				}
				groupLevel++;
			}
		}
		private Dictionary<string, List<FakeObject>> GetGroupedObjects(IMemberInfo memberInfo, List<FakeObject> objects) {
			Dictionary<string, List<FakeObject>> result = new Dictionary<string, List<FakeObject>>();
			foreach(FakeObject item in objects) {
				object propertyValue = item.GetValue(memberInfo);
				string value = propertyValue != null ? propertyValue.ToString() : string.Empty;
				List<FakeObject> itemsByPropertyValue;
				if(!result.TryGetValue(value, out itemsByPropertyValue)) {
					itemsByPropertyValue = new List<FakeObject>();
					result[value] = itemsByPropertyValue;
				}
				itemsByPropertyValue.Add(item);
			}
			return result;
		}
		#region IServerModeGroupHelperTestable Members
		void IServerModeGroupHelperTestable.Apply(ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo, List<FakeObject> objectsList) {
			Apply(groupSummaryInfo, totalSummaryInfo, objectsList);
		}
		List<ListSourceGroupInfo> IServerModeGroupHelperTestable.GetGroupInfo(ListSourceGroupInfo parentGroup) {
			return GetGroupInfo(parentGroup);
		}
		void IServerModeGroupHelperTestable.PopulateGroups(List<IGridColumnModelSynchronizer> groupedColumns, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo, List<FakeObject> objectsList) {
			PopulateGroups(groupedColumns, groupSummaryInfo, totalSummaryInfo, objectsList);
		}
		#endregion
	}
	internal class FakeServerModeGroupInfo : ListSourceGroupInfo {
		private List<FakeObject> innerItems;
		private List<object> summaryInfo = new List<object>();
		private List<ListSourceGroupInfo> children = new List<ListSourceGroupInfo>();
		public FakeServerModeGroupInfo(List<FakeObject> innerItems, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo) {
			this.innerItems = innerItems;
			CalcGroupSummaryInfo(groupSummaryInfo);
		}
		private void CalcGroupSummaryInfo(ICollection<ServerModeSummaryDescriptor> groupSummaryInfo) {
			if(groupSummaryInfo != null) {
				foreach(ServerModeSummaryDescriptor item in groupSummaryInfo) {
					if(item.SummaryType == Aggregate.Count) {
						summaryInfo.Add(innerItems.Count);
					}
					else {
						summaryInfo.Add(item.SummaryType.ToString() + "Value");
					}
				}
			}
		}
		public List<ListSourceGroupInfo> Children {
			get { return children; }
		}
		public List<FakeObject> InnerItems {
			get { return innerItems; }
		}
		public override List<object> Summary {
			get { return summaryInfo; }
		}
	}
	internal interface IServerModeGroupHelperTestable {
		void Apply(ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo, List<FakeObject> objectsList);
		List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup);
		void PopulateGroups(List<IGridColumnModelSynchronizer> groupedColumns, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo, List<FakeObject> objectsList);
	}
}
