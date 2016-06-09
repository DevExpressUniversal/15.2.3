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
using DevExpress.Data.PivotGrid;
namespace DevExpress.PivotGrid.QueryMode.TuplesTree {
	public class PreLastLevelRecord : LevelRecord {
		NullableDictionary<object, LevelRecord> children = new NullableDictionary<object, LevelRecord>();
		LevelRecord total, others;
		public int Count { get { return children.Count; } }
		public LevelRecord Others { get { return others; } }
		public PreLastLevelRecord(QueryMember member)
			: base(member) {
		}
		public void AddTotalRecord(LevelRecord record) {
			total = record;
		}
		public void AddOthersRecord(LevelRecord record) {
			others = record;
		}
		public LevelRecord GetValue(object value, bool isTotal) {
			if(isTotal && total != null)
				return total;
			return TryGetRecord(value);
		}
		public override LevelRecord TryGetRecord(object value) {
			LevelRecord record = null;
			if(children.TryGetValue(value, out record))
				return record;
			if(total != null && object.Equals(value, total.Member.UniqueLevelValue)) 
				return total;
			if(others != null && object.Equals(value, DevExpress.PivotGrid.ServerMode.OthersValueColumn.OthersValue))
				return others;
			return null;
		}
		internal override int BuildLevelList(IComparer<LevelRecord> sort, ref LevelRecord[] list) {
			int currCount = 0;
			if(total != null)
				currCount++;
			int othersCount = others == null ? 0 : 1;
			int allCount = currCount + children.Count;
			if(list.Length < allCount + othersCount)
				list = new LevelRecord[allCount + othersCount];
			children.CopyValuesTo(list, currCount);
			if(total != null) {
				list[0] = total;
			} else
				if(others != null) {
					list[allCount] = others;
					allCount++;
				}
			try {
				if(sort != null)
					Array.Sort<LevelRecord>(list, currCount, children.Count, sort);
			} catch { }
			return allCount;
		}
		public virtual void AddRecord(LevelRecord record) {
			QueryMember member = record.member;
			if(member.Column == null || member.IsTotal) { 
				total = record;
			} else
				children.Add(member.UniqueLevelValue, record);
		}
		public void Clear() {
			children = new NullableDictionary<object, LevelRecord>();
			total = null;
			others = null;
		}
	}
	public class LevelRecord : Sorting.IQueryMemberProvider {
		GroupInfo groupInfo;
		internal readonly QueryMember member;
		public GroupInfo GroupInfo {
			get { return groupInfo; }
			set { groupInfo = value; }
		}
		public QueryMember Member {
			get { return member; }
		}
		public LevelRecord(QueryMember member) {
			this.member = member;
		}
		public virtual LevelRecord TryGetRecord(object value) {
			return null;
		}
		internal virtual int BuildLevelList(IComparer<LevelRecord> sort, ref LevelRecord[] list) {
			return 0;
		}
	}
}
