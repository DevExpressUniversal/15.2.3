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
namespace DevExpress.PivotGrid.QueryMode {
	static class ActionProviderHelper {
		public delegate void SetGroupIndex(int index, GroupInfo group);
		public static List<QueryTuple> GetTuples<TColumn>(AreaFieldValues fieldValues, GroupInfo[] groups, List<TColumn> area)
			where TColumn : QueryColumn {
			List<QueryTuple> result = new List<QueryTuple>();
			int maxCount = fieldValues.MaxLevel;
			bool createAllMembers = false;
			for(int i = 0; i < maxCount; i++)
				if(area[i].Metadata.ChildColumn != null) {
					createAllMembers = true;
					break;
				}
			for(int i = 0; i < groups.Length; i++) {
				GroupInfo group = groups[i];
				if(group.IsTotal)
					throw new QueryException("Total value can't be expanded");
				result.Add(new QueryTuple(group, AreaFieldValues.GetHierarchyMembers(group), createAllMembers));
			}
			return result;
		}
		public static void CreateChildGroups<TTuple>(IList<TTuple> tuples, AreaFieldValues fieldValues,
				Dictionary<TTuple, List<QueryMember>> tupleMembers, Dictionary<TTuple, int> indexesByTuples,
				SetGroupIndex setGroupIndex) where TTuple : QueryTuple {
			foreach(TTuple tuple in tuples) {
				List<QueryMember> membersUniqueValues;
				if(!tupleMembers.TryGetValue(tuple, out membersUniqueValues) || membersUniqueValues.Count == 0)
					continue;
				int startIndex = indexesByTuples[tuple];
				CreateChildGroupsCore(tuple, startIndex, membersUniqueValues, setGroupIndex);
			}
		}
		static void CreateChildGroupsCore(QueryTuple tuple, int startIndex, List<QueryMember> members, SetGroupIndex setGroupIndex) {
			GroupInfo tupleGroup = tuple.BaseGroup, lastGroupInfo = null;
			for(int i = 0; i < members.Count; i++) {
				QueryMember member = members[i];
				if(member == null)
					continue;
				if(i > 0 && object.Equals(member.UniqueLevelValue, members[i - 1].UniqueLevelValue)) {
					setGroupIndex(i + startIndex, lastGroupInfo);
					continue;
				}
				GroupInfo groupInfo = new GroupInfo(tupleGroup.Level + 1, member, tupleGroup);
				setGroupIndex(i + startIndex, groupInfo);
				lastGroupInfo = groupInfo;
			}
		}
	}
}
