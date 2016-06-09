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
using System.Collections.ObjectModel;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPLevelFilter {
		OLAPCubeColumn column;
		CriteriaOperator criteria;
		bool excludeCalculatedMembers;
		List<OLAPMember> members;
		public OLAPLevelFilter(OLAPCubeColumn column, CriteriaOperator criteria) : this(column) {
			this.criteria = criteria;
		}
		public OLAPLevelFilter(OLAPCubeColumn column, List<OLAPMember> members) : this(column) {
			this.members = members;
		}
		protected OLAPLevelFilter(OLAPCubeColumn column) {
			this.column = column;
		}
		public void ExcludeCalculatedMembers() {
			OLAPMetadataColumn meta = column.Metadata;
			if(!ReferenceEquals(criteria, null))
				excludeCalculatedMembers = true;
			else
				for(int i = members.Count - 1; i >= 0; i--)
					if(meta.GetIsCalculatedMember(members[i]))
						members.RemoveAt(i);
		}
		public OLAPCubeColumn Column {
			get { return column; }
		}
		public string GetMDX(bool includeBrackets) {
			string mdx = GetMDX();
			if(excludeCalculatedMembers)
				return MDX.Intersect(MDX.Members(column), mdx);
			else
				return includeBrackets ? "{" + mdx + "}" : mdx;
		}
		string GetMDX() {
			return members == null ? criteria.ToString() : MDX.GetSet(members, false);
		}
		public int GetCount() {
			return members == null ? ReferenceEquals(criteria, null) ? 0 : -1 : members.Count;
		}
		public List<OLAPMember> GetProcessedMembers() { 
			return members;
		}
	}
	public class OLAPFilterValues {
		int? depth = null;
		bool isIncluded;
		OLAPCubeColumn column;
		IList<OLAPMember> coreMembers;		
		ReadOnlyCollection<OLAPMember> members;
		public bool IsIncluded { get { return isIncluded; } }
#if DEBUGTEST
		public OLAPFilterValues(bool isIncluded, IList<OLAPMember> members) : this(isIncluded, members, (OLAPCubeColumn)null) { }
#endif
		public OLAPFilterValues(bool isIncluded, IList<OLAPMember> members, OLAPCubeColumn column) : this(isIncluded) {
			this.column = column;
			this.coreMembers = members ?? new List<OLAPMember>();
			   this.isIncluded = isIncluded;
		}
		public OLAPFilterValues(bool isIncluded, IList<OLAPMember> members, IOLAPHelpersOwner owner)
			: this(isIncluded, members, members != null && members.Count > 0 ? owner.CubeColumns[((OLAPMetadataColumn)members[0].Column).UniqueName] : null) {
		}
		OLAPFilterValues(bool isIncluded) {
			this.isIncluded = true;
		}
		public OLAPLevelFilter GetLevelFilter() {
			List<OLAPMember> list = new List<OLAPMember>();
			list.AddRange(GetProcessedMembers());
			return new OLAPLevelFilter(column, list);
		}
		public bool IsCustomDefaultMemberFilter {
			get {
				if(!IsIncluded)
					return GetMemberCount() == 0;
				if(GetMemberCount() == 0)
					return false;
				return GetMemberCount() == Column.Metadata.NonTotalMembersCount;
			}
		}
		public bool IsSingleValueFilter {
			get {
				if(IsIncluded)
					return GetMemberCount() == 1;
				if(GetMemberCount() == 0)
					return false;
				return GetMemberCount() == Column.Metadata.NonTotalMembersCount - 1;
			}
		}
		public int GetMemberCount() {
			return GetProcessedMembers() == null ? -1 : members.Count;
		}
		public string GetSingleMember() {
			return GetProcessedMembers() == null || members.Count > 1 ? null : members[0].UniqueName;
		}
		public ReadOnlyCollection<OLAPMember> GetProcessedMembers() {
			if(members == null) {
				List<OLAPMember> mems = new List<OLAPMember>();
				for(int i = 0; i < coreMembers.Count; i++)
					mems.Add((OLAPMember)coreMembers[i]);
				members = new ReadOnlyCollection<OLAPMember>(mems);
			}
			return members;
		}
		public OLAPLevelFilter[] GetMembersByLevels() {
			List<List<OLAPMember>> res = new List<List<OLAPMember>>();
			IQueryMetadataColumn lastColumn = null;
			List<OLAPMember> currentList = null;
			for(int i = 0; i < GetProcessedMembers().Count; i++) {
				OLAPMember member = (OLAPMember)members[i];
				if(member.Column != lastColumn) {
					currentList = new List<OLAPMember>();
					currentList.Add(member);
					res.Add(currentList);
					lastColumn = member.Column;
				} else
					currentList.Add(member);
			}
			OLAPLevelFilter[] membersByLevels = new OLAPLevelFilter[res.Count];
			for(int i = 0; i < res.Count; i++) {
				membersByLevels[i] = new OLAPLevelFilter(column.Owner.CubeColumns[res[i][0].Column.UniqueName], res[i]);
			}
			return membersByLevels;
		}
		public OLAPLevelFilter[] GetMembersByHierarchy(List<IQueryMetadataColumn> hierarchy) {
			if(hierarchy.Count == 0)
				throw new ArgumentOutOfRangeException("hierarchy can't be empty");
			List<OLAPMember>[] res = new List<OLAPMember>[hierarchy.Count];
			List<OLAPMember> currentList = null;
			for(int i = 0; i < GetProcessedMembers().Count; i++) {
				OLAPMember member = members[i];
				int columnIndex = hierarchy.IndexOf(member.Column);
				if(columnIndex < 0)
					throw new Exception("Column wasn't found in the hierarchy");
				currentList = res[columnIndex];
				if(currentList == null) {
					currentList = new List<OLAPMember>();
					res[columnIndex] = currentList;
				}
				currentList.Add(member);
			}
			OLAPLevelFilter[] membersByLevels = new OLAPLevelFilter[res.Length];
			int lastNotNull = -1;
			for(int i = 0; i < res.Length; i++) {
				if(res[i] != null) {
					membersByLevels[i] = new OLAPLevelFilter(column.Owner.CubeColumns[res[i][0].Column.UniqueName], res[i]);
					lastNotNull = i;
				}
			}
			Array.Resize<OLAPLevelFilter>(ref membersByLevels, lastNotNull + 1);
			return membersByLevels;
		}
		public OLAPLevelFilter[] GetMembersByLevels(List<IQueryMetadataColumn> hierarchy) { 
			if(hierarchy.Count == 0)
				throw new ArgumentOutOfRangeException("hierarchy can't be empty");
			List<OLAPMember>[] res = new List<OLAPMember>[hierarchy.Count];
			OLAPMetadataColumn lastColumn = null;
			List<OLAPMember> currentList = null;
			for(int i = 0; i < GetProcessedMembers().Count; i++) {
				OLAPMember member = members[i];
				if(member.Column != lastColumn) {
					currentList = new List<OLAPMember>();
					currentList.Add(member);
					int columnIndex = hierarchy.IndexOf(member.Column);
					if(columnIndex < 0)
						throw new Exception("Column wasn't found in the hierarchy");
					res[columnIndex] = currentList;
					lastColumn = member.Column;
				} else
					currentList.Add(member);
			}
			OLAPLevelFilter[] membersByLevels = new OLAPLevelFilter[res.Length];
			int lastNotNull = -1;
			for(int i = 0; i < res.Length; i++) {
				if(res[i] != null) {
					membersByLevels[i] = new OLAPLevelFilter(column.Owner.CubeColumns[res[i][0].Column.UniqueName], res[i]);
					lastNotNull = i;
				}
			}
			Array.Resize<OLAPLevelFilter>(ref membersByLevels, lastNotNull + 1);
			return membersByLevels;
		}
		public OLAPCubeColumn Column { get { return column ?? column.Owner.CubeColumns[GetProcessedMembers()[0].Column.UniqueName]; } }
		public bool IsEmpty {
			get { return !IsIncluded && GetMemberCount() == 0; }
		}
		public int Depth { 
			get {
				if(depth == null) {
					depth = 0;
					IQueryMetadataColumn lastColumn = null;
					for(int i = 0; i < GetProcessedMembers().Count; i++) {
						QueryMember member = members[i];
						if(member.Column != lastColumn) {
							lastColumn = member.Column;
							depth++;
						}
					}
				}
				return depth.Value;
			}
		}
		public override string ToString() {
			StringBuilder stb = new StringBuilder();
			stb.Append("{").Append(IsIncluded ? "Included" : "Excluded").Append(", ").Append(Depth)
				.Append(", [");
			stb.Append(GetLevelFilter().GetMDX(false));
			stb.Append("]}");
			return stb.ToString();
		}
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			return ToString() == obj.ToString();
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
