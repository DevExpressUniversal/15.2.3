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

using System.Collections.Generic;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using System.Linq;
using System;
using System.Collections;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPUniqueValues : UniqueValues<OLAPCubeColumn> {
		public OLAPUniqueValues(IOLAPHelpersOwner owner) : base(owner) { }
		protected new IOLAPHelpersOwner Owner { get { return (IOLAPHelpersOwner)base.Owner; } }
		protected new IOLAPMetadata Metadata { get { return (IOLAPMetadata)base.Metadata; } }
		protected override List<object> GetSortedUniqueValues(OLAPCubeColumn column) {
			return GetSortedUniqueValuesCore(column, column.Metadata.AllMembersLoaded ? column.Metadata.EnumerateMembers() : null);
		}
		protected IList<OLAPMember> GetMembersByValue(IOLAPEditableMemberCollection collection, bool resursive, int level, object value) {
			IList<OLAPMember> childMembers = null;
			if(!Owner.CubeColumns[((OLAPMetadataColumn)collection.Column).UniqueName].OLAPFilterByUniqueName) {
				childMembers = collection.GetMembersByValue(resursive, level, value);
			} else {
				childMembers = new List<OLAPMember>();
				OLAPMember member = null;
				string uniqueName = value as string;
				if(uniqueName != null)
					member = collection.GetMemberByUniqueLevelValue(uniqueName);
				if(member != null)
					childMembers.Add(member);
			}
			return childMembers;
		}
		public override List<object> GetSortedUniqueGroupValues(PivotGridGroup group, object[] parentValues) {
			if(!IsValidGroup(group))
				return new List<object>();
			if(parentValues == null)
				return GetSortedUniqueValues(group[0]);
			OLAPMember member = (OLAPMember)FindGroupMember(group, parentValues);
			if(member == null || member.Column.ChildColumn == null)
				return new List<object>();
			if(member.ChildMembers.Count == 0)
				Metadata.QueryChildMembers(Owner.CubeColumns[member.Column.ChildColumn.UniqueName], member);
			if(member.ChildMembers.Count == 0)
				return new List<object>();
			OLAPCubeColumn childGroupColumn = GetChildGroupColumn(group, member.Column);
			if(childGroupColumn == null)
				return new List<object>();
			List<QueryMember> members = GetChildMembers(member, childGroupColumn.Metadata);
			if(members.Count == 0)
				return new List<object>();
			return GetSortedUniqueValuesCore(childGroupColumn, members);
		}
		protected bool IsValidGroup(PivotGridGroup group) {
			for(int i = 0; i < group.Count; i++) {
				if(!IsValidField(group[i]))
					return false;
			}
			return true;
		}
		protected QueryMember FindGroupMember(PivotGridGroup group, IList<object> parentValues) {
			OLAPCubeColumn rootColumn = Owner.CubeColumns[group[0]];
			object rootValue = parentValues[0];
			IList<OLAPMember> members = GetMembersByValue(rootColumn.Metadata, false, -1, rootValue);
			if(members.Count == 0) {
				rootColumn.EnsureColumnMembersLoaded();
				members = GetMembersByValue(rootColumn.Metadata, false, -1, rootValue);
			}
			if(members.Count != 1)
				return null;
			OLAPMember member = members[0];
			for(int i = 1; i < parentValues.Count; i++) {
				if(member.ChildMembers.Count == 0)
					Metadata.QueryChildMembers(Owner.CubeColumns[member.Column.ChildColumn.UniqueName], member);
				members = GetMembersByValue(member.ChildMembers, true, group[i].Level, parentValues[i]);
				if(members.Count != 1)
					return null;
				member = members[0];
			}
			return member;
		}
		List<object> GetSortedUniqueValuesCore(OLAPCubeColumn childGroupColumn, IEnumerable<QueryMember> unSortedMembers) {
			 OLAPMember[] sortedMembers = Owner.Metadata.QuerySortMembers(Owner, childGroupColumn, unSortedMembers);
			List<object> result = new List<object>(sortedMembers.Length);
			if(childGroupColumn.OLAPFilterByUniqueName) {
				for(int i = 0; i < sortedMembers.Length; i++) {
					OLAPMember cmember = sortedMembers[i];
					if(cmember == null)
						continue;
					result.Add(cmember.UniqueName);
				}
			} else {
				NullableHashtable uniqueHash = new NullableHashtable(sortedMembers.Length);
				for(int i = 0; i < sortedMembers.Length; i++) {
					QueryMember cmember = sortedMembers[i];
					if(cmember == null)
						continue;
					object value = cmember.Value;
					if(uniqueHash.Contains(value))
						continue;
					result.Add(value);
					uniqueHash.Add(value, null);
				}
			}
			return result;
		}
		protected List<QueryMember> GetChildMembers(OLAPMember member, MetadataColumnBase toChildColumn) {
			IQueryMetadataColumn childColumn = member.Column.ChildColumn;
			List<QueryMember> childMembers = new List<QueryMember>();
			if(childColumn == null)
				return childMembers;
			if(member.ChildMembers.Count == 0)
				Metadata.QueryChildMembers(Owner.CubeColumns[member.Column.ChildColumn.UniqueName], member);
			if(childColumn == toChildColumn) {
				foreach(OLAPMember childMember in member.ChildMembers)
					childMembers.Add(childMember);
				return childMembers;
			}
			foreach(OLAPMember childMember in member.ChildMembers)
				childMembers.AddRange(GetChildMembers(childMember, toChildColumn));
			return childMembers;
		}
		OLAPCubeColumn GetChildGroupColumn(PivotGridGroup group, OLAPMetadataColumn parentColumn) {
			for(int i = 0; i < group.Count; i++) {
				if(group[i].FieldName == parentColumn.UniqueName)
					return ++i < group.Count ? Owner.CubeColumns[group[i].FieldName] : null;
			}
			return null;
		}
		protected override IEnumerable<object> GetUniqueValueMembers(OLAPCubeColumn column) {
			column.EnsureColumnMembersLoaded();
			if(column.OLAPFilterByUniqueName) {
				return column.Metadata.EnumerateUniqueNames();
			} else {
				return column.Metadata.EnumerateValues();
			}
		}
		public override bool? IsGroupFilterValueChecked(PivotGridGroup group, object[] parentValues, object value) {
			if(!IsValidGroup(group))
				return null;
			List<object> values = new List<object>();
			if(parentValues != null)
				values.AddRange(parentValues);
			values.Add(value);
			PivotGroupFilterValue filterValue = group.FilterValues.FindFilterValue(values.ToArray());
			if(filterValue == null)
				return false;
			QueryMember member = FindGroupMember(group, values);
			if(member == null)
				return true;
			return IsGroupFilterValueCheckedCore(member, filterValue);
		}
		bool? IsGroupFilterValueCheckedCore(QueryMember member, PivotGroupFilterValue filterValue) {
			OLAPMember olapMember = (OLAPMember)member;
			if(filterValue.IsLastLevel || filterValue.ChildValues.Count == 0)
				return true;
			if(olapMember.ChildMembers.Count == 0)
				Metadata.QueryChildMembers(Owner.CubeColumns[olapMember.Column.ChildColumn.UniqueName], olapMember);
			PivotGridFieldBase childValuesField;
			if(filterValue.ChildValues.Count > 0)
				childValuesField = filterValue.ChildValues.Items[0].Field;
			else
				childValuesField = filterValue.Field;
			OLAPCubeColumn column = Owner.CubeColumns[childValuesField.FieldName];
			List<QueryMember> childMembers = GetChildMembers(olapMember, column.Metadata);
			if(childMembers.Count > filterValue.ChildValues.Count)
				return null;
			int foundItemsCount = 0;
			for(int i = 0; i < childMembers.Count; i++) {
				if(filterValue.ChildValues.ContainsValue(column.OLAPFilterByUniqueName ? childMembers[i].UniqueLevelValue : childMembers[i].Value))
					foundItemsCount++;
			}
			if(foundItemsCount == 0)
				return false;
			if(foundItemsCount < childMembers.Count)
				return null;
			foundItemsCount = 0;
			bool hasNullChildren = false;
			for(int i = 0; i < childMembers.Count; i++) {
				QueryMember childMember = childMembers[i];
				PivotGroupFilterValue childFilterValue = filterValue.ChildValues[column.OLAPFilterByUniqueName ? childMember.UniqueLevelValue : childMember.Value];
				bool? res = IsGroupFilterValueCheckedCore(childMember, childFilterValue);
				if(res == true)
					foundItemsCount++;
				if(!res.HasValue)
					hasNullChildren = true;
			}
			if(!hasNullChildren && foundItemsCount == 0)
				return false;
			if(foundItemsCount < childMembers.Count)
				return null;
			return true;
		}
	}
}
