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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.PivotGrid.QueryMode {
	public class QueryTuple : IEnumerable {
		static QueryMember[] BuildMembers(QueryMember[] members) {
			QueryMember[] list = new QueryMember[members.Length];
			int count = 0;
			for(int i = 0; i < members.Length; i++) {
				QueryMember member = members[i];
				if(member.IsTotal)
					throw new ArgumentException("Invalid hierarchy");
				if((i == members.Length - 1 || !members[i].Column.IsParent(members[i + 1].Column))) {
					list[count] = member;
					count++;
				}
			}
			Array.Resize<QueryMember>(ref list, count);
			return list;
		}
		QueryMember[] allMembers;
		QueryMember[] members;
		GroupInfo baseGroup;
		public int MemberCount { get { return members.Length; } }
		public QueryMember this[int index] { get { return members[index]; } }
		public QueryMember Last {
			get {
				if(MemberCount == 0)
					return null;
				return this[MemberCount - 1];
			}
		}
		public GroupInfo BaseGroup { get { return baseGroup; } set { baseGroup = value; } }
		public QueryTuple(params QueryMember[] members)
			: this((IEnumerable<QueryMember>)members) {
		}
		public QueryTuple(IEnumerable<QueryMember> members) {
			this.members = members == null ? new QueryMember[0] : members.ToArray();
			this.allMembers = this.members;
		}
		public QueryTuple(GroupInfo baseGroup, params QueryMember[] members)
			: this(members) {
			this.baseGroup = baseGroup;
		}
		public QueryTuple(GroupInfo group, QueryMember[] members, bool createMembers) {
			this.baseGroup = group;
			this.allMembers = members;
			if(createMembers)
				this.members = BuildMembers(members);
			else
				this.members = members;
		}
		public bool ContainsOthers() {
			foreach(QueryMember member in members) {
				if(member.IsOthers)
					return true;
			}
			return false;
		}
		public bool ContainsTotal() {
			foreach(QueryMember member in members) {
				if(member.IsTotal)
					return true;
			}
			return false;
		}
		public override bool Equals(object obj) {
			QueryTuple tuple = obj as QueryTuple;
			if(tuple == null || tuple.MemberCount != MemberCount)
				return false;
			for(int i = 0; i < MemberCount; i++)
				if(tuple[i] != this[i])
					return false;
			return true;
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder();
			result.Append("{ ");
			for(int i = 0; i < MemberCount; i++) {
				result.Append(this[i]);
				if(i != MemberCount - 1)
					result.Append(", ");
			}
			result.Append(" }");
			return result.ToString();
		}
		public override int GetHashCode() {
			return (Last.UniqueLevelValue ?? string.Empty).GetHashCode();
		}
		public IList<QueryMember> AllMembers { get { return allMembers; } }
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return members.GetEnumerator();
		}
		#endregion
	}
}
