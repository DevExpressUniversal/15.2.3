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
using DevExpress.Compatibility.System;
using System.Collections.Generic;
namespace DevExpress.PivotGrid.QueryMode.TuplesTree {
	public class TuplesIndexedTreeCache<TColumn> where TColumn : QueryColumn {
		public static object ConvertDBNull(object value) {
			if(value is DBNull)
				return null;
			return value;
		}
		readonly List<TColumn> area;
		readonly Dictionary<IQueryMetadataColumn, QueryMember> others = new Dictionary<IQueryMetadataColumn, QueryMember>();
		PreLastLevelRecord root = new PreLastLevelRecord(new QueryMember(null, QueryMember.TotalValue)) { GroupInfo = GroupInfo.GrandTotalGroup };
		public Dictionary<IQueryMetadataColumn, QueryMember> Others { get { return others; } }
		public List<TColumn> Area {
			get {
				return area;
			}
		}
		public PreLastLevelRecord Root {
			get {
				return root;
			}
		}
		public TuplesIndexedTreeCache(List<TColumn> area) {
			this.area = area;
		}
		public LevelRecord GetMembersIndex(List<QueryMember> sortByMembers) {
			PreLastLevelRecord currentRoot = root;
			LevelRecord nextTotal = null;
			int index = 0;
			Dictionary<IQueryMetadataColumn, object> dic = new Dictionary<IQueryMetadataColumn, object>();
			foreach(QueryMember member in sortByMembers)
				dic[member.Column] = member.UniqueLevelValue;
			while(currentRoot != null && index != sortByMembers.Count) {
				object val = null;
				if(index < Area.Count && dic.TryGetValue(Area[index].Metadata, out val))
					nextTotal = currentRoot.GetValue(val, false);
				else
					nextTotal = null;
				if(nextTotal == null)
					nextTotal = currentRoot.GetValue(QueryMember.TotalValue, true);
				currentRoot = nextTotal as PreLastLevelRecord;
				index++;
			}
			if(nextTotal == null)
				return currentRoot.GetValue(null, true) ?? currentRoot;
			return nextTotal;
		}
		public virtual LevelRecord AddResultValues(MemberCreatorBase creator) {
			LevelRecord currentRoot = root;
			int i;
			for(i = 0; i < creator.Length; i++) {
				LevelRecord newRoot = currentRoot.TryGetRecord(creator.GetValue(i));
				if(ReferenceEquals(null, newRoot))
					break;
				else
					currentRoot = newRoot;
			}
			if(i != creator.Length)
				return CreateFromIndex((PreLastLevelRecord)currentRoot, creator, i);
			if(i == 0)
				return root;
			return currentRoot;
		}
		protected virtual LevelRecord CreateFromIndex(PreLastLevelRecord currentRoot, MemberCreatorBase creator, int startIndex) {
			int length = creator.Length;
			for(int i = startIndex; i < length - 1; i++) {
				PreLastLevelRecord newRoot = new PreLastLevelRecord(creator.GetMember(i));
				creator.Add(i, currentRoot, newRoot);
				currentRoot = newRoot;
			}
			LevelRecord rec = new LevelRecord(creator.GetMember(length - 1));
			creator.Add(length - 1, currentRoot, rec);
			return rec;
		}
		public int BuildLastLevel(QueryTuple parentTuple, IComparer<LevelRecord> sort, bool sortByEmpty, ref LevelRecord[] list) {
			int count = parentTuple.MemberCount;
			PreLastLevelRecord currentRoot = Root;
			int i;
			for(i = 0; i < count; i++) {
				PreLastLevelRecord newRoot = currentRoot.TryGetRecord(parentTuple[i].UniqueLevelValue) as PreLastLevelRecord;
				if(newRoot == null)
					break;
				else
					currentRoot = newRoot;
			}
			if(i != count)
				return 0;
			return currentRoot.BuildLevelList(sort, ref list);
		}
		public void SetTupleGroup(QueryTuple parentTuple, GroupInfo group) {
			int count = parentTuple.MemberCount;
			if(count == 0)
				return;
			LevelRecord currentRoot = Root;
			int i;
			for(i = 0; i < count; i++) {
				LevelRecord newRoot = currentRoot.TryGetRecord(parentTuple[i].UniqueLevelValue);
				if(newRoot == null)
					break;
				else
					currentRoot = newRoot;
			}
			if(i != count && !QueryMember.TotalValue.Equals(parentTuple[i].UniqueLevelValue))
				return;
			currentRoot.GroupInfo = group;
		}
	}
}
