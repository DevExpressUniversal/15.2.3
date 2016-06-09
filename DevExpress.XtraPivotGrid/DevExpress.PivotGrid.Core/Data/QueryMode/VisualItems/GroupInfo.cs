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
using DevExpress.Data.IO;
namespace DevExpress.PivotGrid.QueryMode {
	public class GroupInfo : Sorting.IQueryMemberProvider {
		public const int GrandTotalIndex = Int32.MinValue;
		static GroupInfo grandTotalGroup;
		List<GroupInfo> children;
		readonly GroupInfo parent;
  		readonly QueryMember member;
		readonly int level;
		public static GroupInfo GrandTotalGroup {
			get {
				if(grandTotalGroup == null)
					grandTotalGroup = new GrandTotalGroupInfo();
				return grandTotalGroup;
			}
		}
		public bool HasChildren {
			get { return children == null; }
		}
		public GroupInfo(QueryMember queryMember, GroupInfo parent, int desiredCapacity) : this(parent.Level + 1, queryMember, parent, desiredCapacity) { }
		public GroupInfo(int level, QueryMember queryMember, GroupInfo parent) : this(level, queryMember, parent, 0) { }
		public GroupInfo(int level, QueryMember queryMember, GroupInfo parent, int parentCapacity) {
			if(queryMember == null)
				throw new ArgumentException("member can not be null");
			this.parent = parent;
			if(parent != null) {
				if(parent.children == null)
					parent.children = new List<GroupInfo>(parentCapacity);
				parent.children.Add(this);
			}
			this.level = level;
			this.member = queryMember;
		}
		public virtual bool IsTotal { get { return member.IsTotal; } }
		public int Level { get { return level; } }
		public IQueryMetadataColumn Column { get { return Member.Column; } }
		public QueryMember Member { get { return member; } }
		public override string ToString() {
			return String.Format("{0}: {1}", Column, Member);
		}
		public virtual void SaveToStream(TypedBinaryWriter writer, Dictionary<IQueryMetadataColumn, int> columnIndexes, Dictionary<QueryMember, int> memberIndexes) {
			writer.Write(Level);
			if(this == GrandTotalGroup)
				writer.Write(GrandTotalIndex);
			else if(IsTotal)
					writer.Write(-columnIndexes[Column]);
				else
				writer.Write(memberIndexes[Member]);
		}
		public override bool Equals(object obj) {
			if(base.Equals(obj)) {
				GroupInfo groupInfo = (GroupInfo)obj;
				return (level == groupInfo.Level) && (member.UniqueLevelValue == groupInfo.Member.UniqueLevelValue);
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public IEnumerable<GroupInfo> GetInversedHierarchy() {
			GroupInfo parent = this;
			while(parent != null && parent.Level >= 0) {
				yield return parent;
				parent = parent.parent;
			}
		}
		public List<GroupInfo> GetChildren() {
			return children;
		}
		public void ClearChildren() {
			children = null;
		}
		internal void ReverseChildren() {
			if(children != null)
				children.Reverse();
		}
		public void RemoveSelf() {
			ClearChildren();
			if(parent != null && parent.children != null)
				parent.children.Remove(this);
		}
	}
	public class GrandTotalGroupInfo : GroupInfo {
		public GrandTotalGroupInfo()
			: base(-1, new DevExpress.PivotGrid.OLAP.OLAPVirtualMember(null, MetadataColumnBase.TotalMemberString), null) { 
		}
		public override bool IsTotal { get { return true; } }
		public override string ToString() {
			return "Grand Total";
		}
	}
}
