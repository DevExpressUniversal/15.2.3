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
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotCacheHierarchyCollection
	public class PivotCacheHierarchyCollection : List<PivotCacheHierarchy> { 
	}
	#endregion
	#region PivotCacheHierarchy
	public class PivotCacheHierarchy {
		bool isMeasure;
		bool isSet;
		bool attribute;
		bool time;
		bool isKeyAttribute;
		bool measures;
		bool oneField;
		bool unbalanced;
		bool unbalancedGroup;
		bool hidden;
		int parentSet = -1;
		int kpiIconSet;
		int memberValueDataType = -1;
		string uniqueName;
		string caption;
		string defaultMemberUniqueName;
		string allUniqueName;
		string allCaption;
		string dimensionUniqueName;
		string displayFolder;
		string measureGroupName;
		PivotCacheFieldUsageCollection fieldsUsage;
		PivotCacheGroupingLevelCollection groupingLevels;
		public int Count { get { return GroupingLevels == null ? 0 : GroupingLevels.Count; } } 
		public PivotCacheFieldUsageCollection FieldsUsage { get { return fieldsUsage; } set { fieldsUsage = value; } }
		public PivotCacheGroupingLevelCollection GroupingLevels { get { return groupingLevels; } set { groupingLevels = value; } }
		public bool IsMeasure { get { return isMeasure; } set { isMeasure = value; } }
		public bool IsSet { get { return isSet; } set { isSet = value; } }
		public bool Attribute { get { return attribute; } set { attribute = value; } }
		public bool Time { get { return time; } set { time = value; } }
		public bool IsKeyAttribute { get { return isKeyAttribute; } set { isKeyAttribute = value; } }
		public bool Measures { get { return measures; } set { measures = value; } }
		public bool OneField { get { return oneField; } set { oneField = value; } }
		public bool Unbalanced { get { return unbalanced; } set { unbalanced = value; } }
		public bool UnbalancedGroup { get { return unbalancedGroup; } set { unbalancedGroup = value; } }
		public bool Hidden { get { return hidden; } set { hidden = value; } }
		public int ParentSet { get { return parentSet; } set { parentSet = value; } }
		public int KpiIconSet { get { return kpiIconSet; } set { kpiIconSet = value; } }
		public int MemberValueDataType { get { return memberValueDataType; } set { memberValueDataType = value; } }
		public string UniqueName { get { return uniqueName; } set { uniqueName = value; } }
		public string Caption { get { return caption; } set { caption = value; } }
		public string DefaultMemberUniqueName { get { return defaultMemberUniqueName; } set { defaultMemberUniqueName = value; } }
		public string AllUniqueName { get { return allUniqueName; } set { allUniqueName = value; } }
		public string AllCaption { get { return allCaption; } set { allCaption = value; } }
		public string DimensionUniqueName { get { return dimensionUniqueName; } set { dimensionUniqueName = value; } }
		public string DisplayFolder { get { return displayFolder; } set { displayFolder = value; } }
		public string MeasureGroupName { get { return measureGroupName; } set { measureGroupName = value; } }
		public void CopyFrom(PivotCacheHierarchy sourceItem) {
			this.isMeasure = sourceItem.isMeasure;
			this.isSet = sourceItem.isSet;
			this.attribute = sourceItem.attribute;
			this.time = sourceItem.time;
			this.isKeyAttribute = sourceItem.isKeyAttribute;
			this.measures = sourceItem.measures;
			this.oneField = sourceItem.oneField;
			this.unbalanced = sourceItem.unbalanced;
			this.unbalancedGroup = sourceItem.unbalancedGroup;
			this.hidden = sourceItem.hidden;
			this.parentSet = sourceItem.parentSet;
			this.kpiIconSet = sourceItem.kpiIconSet;
			this.memberValueDataType = sourceItem.memberValueDataType;
			this.uniqueName = sourceItem.uniqueName;
			this.caption = sourceItem.caption;
			this.defaultMemberUniqueName = sourceItem.defaultMemberUniqueName;
			this.allUniqueName = sourceItem.allUniqueName;
			this.allCaption = sourceItem.allCaption;
			this.dimensionUniqueName = sourceItem.dimensionUniqueName;
			this.displayFolder = sourceItem.displayFolder;
			this.measureGroupName = sourceItem.measureGroupName;
			this.fieldsUsage.AddRange(sourceItem.fieldsUsage);
			PivotCacheGroupingLevelCollection targetGroupingLevels = this.groupingLevels;
			PivotCacheGroupingLevelCollection sourceGroupingLevels = sourceItem.groupingLevels;
			targetGroupingLevels.Capacity = sourceGroupingLevels.Count;
			Debug.Assert(targetGroupingLevels.Count == 0);
			foreach (PivotCacheGroupingLevel sourceGroupLevel in sourceGroupingLevels) {
				PivotCacheGroupingLevel targetGroupLevel = new PivotCacheGroupingLevel();
				targetGroupLevel.CopyFrom(sourceGroupLevel);
				targetGroupingLevels.Add(targetGroupLevel);
			}
		}
	}
	#endregion
	#region PivotCacheFieldUsageCollection
	public class PivotCacheFieldUsageCollection : List<int> {
	}
	#endregion
	#region PivotCacheGroupingLevelCollection
	public class PivotCacheGroupingLevelCollection : List<PivotCacheGroupingLevel> {
	}
	#endregion
	#region PivotCacheGroupingLevel
	public class PivotCacheGroupingLevel {
		bool customRollUp;
		bool user;
		string caption;
		string uniqueName;
		PivotCacheGroupCollection groups;
		public PivotCacheGroupCollection Groups { get { return groups; } set { groups = value; } }
		public bool CustomRollUp { get { return customRollUp; } set { customRollUp = value; } }
		public bool User { get { return user; } set { user = value; } }
		public string Caption { get { return caption; } set { caption = value; } }
		public string UniqueName { get { return uniqueName; } set { uniqueName = value; } }
		public void CopyFrom(PivotCacheGroupingLevel source) {
			this.customRollUp = source.customRollUp;
			this.user = source.user;
			this.caption = source.caption;
			this.uniqueName = source.uniqueName;
			PivotCacheGroupCollection targetGroups = this.groups;
			targetGroups.Capacity = source.groups.Count;
			System.Diagnostics.Debug.Assert(targetGroups.Count == 0);
			foreach (PivotCacheGroup sourcePivotCacheGroup in source.groups) {
				PivotCacheGroup targetPivotCacheGroup = new PivotCacheGroup();
				targetPivotCacheGroup.CopyFrom(sourcePivotCacheGroup);
				targetGroups.Add(targetPivotCacheGroup);
			}
		}
	}
	#endregion
	#region PivotCacheGroupCollection
	public class PivotCacheGroupCollection : List<PivotCacheGroup> {
	}
	#endregion
	#region PivotCacheGroup
	public class PivotCacheGroup {
		int id = int.MinValue;
		string caption;
		string name;
		string uniqueName;
		string uniqueParent;
		PivotCacheGroupMemberCollection members = new PivotCacheGroupMemberCollection();
		public PivotCacheGroupMemberCollection Members { get { return members; } set { members = value; } }
		public int Id { get { return id; } set { id = value; } }
		public string Caption { get { return caption; } set { caption = value; } }
		public string Name { get { return name; } set { name = value; } }
		public string UniqueName { get { return uniqueName; } set { uniqueName = value; } }
		public string UniqueParent { get { return uniqueParent; } set { uniqueParent = value; } }
		public void CopyFrom(PivotCacheGroup source) {
			this.id = source.id;
			this.caption = source.caption;
			this.name = source.name;
			this.uniqueName = source.uniqueName;
			this.uniqueParent = source.uniqueParent;
			this.members.Capacity = source.members.Count;
			System.Diagnostics.Debug.Assert(this.members.Count == 0);
			foreach (PivotCacheGroupMember sourceGroupMember in source.members) {
				PivotCacheGroupMember targetGm = new PivotCacheGroupMember();
				targetGm.CopyFrom(sourceGroupMember);
				this.members.Add(targetGm);
			}
		}
	}
	#endregion
	#region PivotCacheGroupMemberCollection
	public class PivotCacheGroupMemberCollection : List<PivotCacheGroupMember> {
	}
	#endregion
	#region PivotCacheGroupMember
	public class PivotCacheGroupMember {
		bool group;
		string uniqueName;
		public bool Group { get { return group; } set { group = value; } }
		public string UniqueName { get { return uniqueName; } set { uniqueName = value; } }
		public void CopyFrom(PivotCacheGroupMember source) {
			this.group = source.group;
			this.uniqueName = source.uniqueName;
		}
	}
	#endregion
}
