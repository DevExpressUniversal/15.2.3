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

using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region GroupList
	public class GroupList {
		#region fields
		int currentLevel;
		int maxLevel;
		int minCollapsedLevel;
		List<GroupItem> openedGroups;
		List<GroupItem> groups;
		Worksheet sheet;
		List<int> dotIndices;
		Dictionary<int, int> buttonPossitionLevels;
		List<int> repeatedButtonPositions;
		bool rows;
		bool buttonBeforeStart;
		#endregion
		public GroupList(Worksheet sheet, bool rows) {
			this.sheet = sheet;
			this.rows = rows;
			this.buttonBeforeStart = rows ? !sheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow : !sheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight;
			this.AllowOpenNewGroups = true;
			openedGroups = new List<GroupItem>();
			groups = new List<GroupItem>();
			dotIndices = new List<int>();
			buttonPossitionLevels = new Dictionary<int, int>();
			repeatedButtonPositions = new List<int>();
			minCollapsedLevel = -1;
		}
		#region Properties
		public int MaxLevel { get { return maxLevel; } }
		public int MinCollapsedLevel { get { return minCollapsedLevel; } }
		public List<GroupItem> Groups { get { return groups; } }
		internal List<GroupItem> OpenedGroups { get { return openedGroups; } }
		public List<int> DotIndices { get { return dotIndices; } }
		public bool AllowOpenNewGroups { get; set; }
		#endregion
		public void ProcessLevel(int level, int modelIndex, bool hidenElement) {
			if (level > 0)
				dotIndices.Add(modelIndex);
			if (level == currentLevel || (!AllowOpenNewGroups && level > currentLevel)) {
				if (hidenElement && openedGroups.Count > 0)
					openedGroups[openedGroups.Count -1].HasHidenElement = true;
				return;
			}
			int count = level - currentLevel;
			if (count > 0)
				OpenGroups(count, modelIndex);
			else CloseGroups(Math.Abs(count), modelIndex - 1);
			currentLevel = level;
			maxLevel = Math.Max(level, maxLevel);
		}
		public void EndProcessGroups() {
			List<GroupItem> removedGroups = new List<GroupItem>();
			foreach (GroupItem group in groups) {
				if (group.ButtonPosition < 0)
					removedGroups.Add(group);
				if (dotIndices.Contains(group.ButtonPosition))
					dotIndices.Remove(group.ButtonPosition);
				if (repeatedButtonPositions.Contains(group.ButtonPosition) && buttonPossitionLevels[group.ButtonPosition] != group.Level)
					group.Hiden = true;
			}
			foreach (GroupItem group in removedGroups)
				groups.Remove(group);
		}
		void OpenGroups(int count, int start) {
			for (int i = 1; i <= count; i++)
				openedGroups.Add(new GroupItem(rows, start, currentLevel + i, buttonBeforeStart));
		}
		void CloseGroups(int count, int end) {
			int openedGroupsCount = openedGroups.Count;
			for (int i = openedGroupsCount - 1; i >= Math.Max(openedGroupsCount - count, 0); i--) {
				GroupItem currentGroup = openedGroups[i];
				currentGroup.End = end;
				FinishGroup(currentGroup);
				openedGroups.RemoveAt(i);
			}
		}
		void FinishGroup(GroupItem finishingGroup) {
			if (finishingGroup.ButtonPosition < 0)
				return;
			if (rows) {
				if (finishingGroup.ButtonPosition > IndicesChecker.MaxRowIndex)
					return;
			}
			else if (finishingGroup.ButtonPosition > IndicesChecker.MaxColumnIndex)
				return;
			foreach (GroupItem group in groups)
				if (group.EqualsPosition(finishingGroup)) {
					if (group.Level > finishingGroup.Level) {
						groups.Remove(group);
						break;
					}
					else return;
				}
			bool hidenButton = false;
			if (rows) {
				Row row = sheet.Rows.TryGetRow(finishingGroup.ButtonPosition);
				if (row != null) {
					finishingGroup.Collapsed = row.IsCollapsed;
					hidenButton = row.IsHidden;
				}
			}
			else {
				Column column = sheet.Columns.TryGetColumn(finishingGroup.ButtonPosition);
				if (column != null) {
					finishingGroup.Collapsed = column.IsCollapsed;
					hidenButton = column.IsHidden;
				}
			}
			if(!hidenButton)
				finishingGroup.Collapsed = finishingGroup.Collapsed || finishingGroup.HasHidenElement;
			groups.Add(finishingGroup);
			if (finishingGroup.Collapsed)
				minCollapsedLevel = Math.Min(finishingGroup.Level, minCollapsedLevel > 0 ? minCollapsedLevel : finishingGroup.Level);
			int buttonPosition = finishingGroup.ButtonPosition;
			if (buttonPossitionLevels.ContainsKey(buttonPosition)) {
				buttonPossitionLevels[buttonPosition] = Math.Min(buttonPossitionLevels[buttonPosition], finishingGroup.Level);
				if (!repeatedButtonPositions.Contains(buttonPosition))
					repeatedButtonPositions.Add(buttonPosition);
			}
			else buttonPossitionLevels.Add(buttonPosition, finishingGroup.Level);
		}
	}
	#endregion
	#region GroupCash
	public class GroupCache {
		#region fields
		GroupList groups;
		int startIndex;
		int endIndex;
		#endregion
		public GroupCache(GroupList groupList, int startIndex, int endIndex) {
			this.groups = groupList;
			this.startIndex = startIndex;
			this.endIndex = endIndex;
		}
		#region Properties
		public GroupList Groups { get { return groups; } }
		#endregion
		public bool GroupsIsCached(int startIndex, int endIndex) {
			return startIndex >= this.startIndex && endIndex <= this.endIndex;
		}
	}
	#endregion
}
