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
using System.Collections;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPMemberCollection : QueryMemberCollectionBase, IOLAPEditableMemberCollection, IQueryMemberCollection {
		readonly NullableDictionary<object, IList<OLAPMember>> valuesHash = new NullableDictionary<object, IList<OLAPMember>>();
		readonly Dictionary<string, OLAPMember> namesHash = new Dictionary<string, OLAPMember>();
		protected IEnumerator<OLAPMember> GetEnumeratorCore() {
			foreach(KeyValuePair<string, OLAPMember> pair in namesHash)
				yield return pair.Value;
		}
		public override int Count {
			get { return namesHash.Count; }
		}
		static IList<OLAPMember> FilterMembersByLevel(IList<OLAPMember> members, int level) {
			List<OLAPMember> newMembers = new List<OLAPMember>();
			foreach(OLAPMember member in members)
				newMembers.Add(member);
			members = newMembers;
			for(int i = members.Count - 1; i >= 0; i--) {
				OLAPMember member = members[i];
				if(member.Column.Level != level)
					members.Remove(member);
			}
			return members;
		}
		public OLAPMember this[string uniqueName] {
			get {
				if(string.IsNullOrEmpty(uniqueName))
					return null;
				OLAPMember res = null;
				return namesHash.TryGetValue(uniqueName, out res) ? res : null;
			}
		}
		public ICollection<string> UniqueNames { get { return namesHash.Keys; } }
		public OLAPMemberCollection(OLAPMetadataColumn column)
			: base(column) {
		}
		protected IQueryMetadata Owner { get { return ((OLAPMetadataColumn)Column).Owner; } }
		public void Clear() {
			valuesHash.Clear();
			namesHash.Clear();
		}
		public override void Add(QueryMember member) {
			OLAPMember oMember = (OLAPMember)member;
			AddToNamesHash(member);
			if(member.IsTotal)
				return;
			object value = member.Value;
			IList<OLAPMember> list;
			if(!valuesHash.TryGetValue(value, out list)) {
				valuesHash.Add(value, oMember);
				return;
			}
			if(!list.Contains(oMember)) {
				OLAPMember wrapper = list as OLAPMember;
				if(wrapper != null)
					valuesHash[value] = new List<OLAPMember> { wrapper, oMember };
				else
					list.Add(oMember);
			}
		}
		public bool Remove(QueryMember member) {
			OLAPMember oMember = (OLAPMember)member;
			if(member == null)
				return false;
			RemoveFromNamesHash(member);
			if(member.IsTotal)
				return false;
			IList<OLAPMember> list;
			if(!valuesHash.TryGetValue(member.Value, out list))
				return false;
			if(list is OLAPMember) {
				return valuesHash.Remove(member.Value);
			} else {
				int index = list.IndexOf(oMember);
				if(index >= 0) {
					list.RemoveAt(index);
					return true;
				}
				return false;
			}
		}
		void AddToNamesHash(QueryMember member) {
			OLAPMember olapMember = (OLAPMember)member;
			namesHash[olapMember.UniqueName] = olapMember;
		}
		void RemoveFromNamesHash(QueryMember member) {
			namesHash.Remove(((OLAPMember)member).UniqueName);
		}
		 IList<OLAPMember> IOLAPEditableMemberCollection.GetMembersByValue(bool recursive, int level, object value) {
			return GetMembersByValue(recursive, level, value);
		}
		public IList<OLAPMember> GetMembersByValue(bool recursive, int level, object value) {
			IList<OLAPMember> members;
			if(!valuesHash.TryGetValue(value, out members))
				members = new List<OLAPMember>();
			if(!recursive)
				return members;
			members = FilterMembersByLevel(members, level);
			if(members.Count == 0 && valuesHash.Count > 0) {
				foreach(IList<OLAPMember> memberList in valuesHash.Values) {
					foreach(OLAPMember member in memberList) {
						if(member.Column.Level >= level)
							continue;
						if(member.ChildMembers.Count == 0)
							((IOLAPMetadata)Owner).QueryChildMembers(null, member);
						((List<OLAPMember>)members).AddRange(member.ChildMembers.GetMembersByValue(true, level, value));
					}
				}
			}
			return members;
		}
		OLAPMember IOLAPEditableMemberCollection.GetMemberByUniqueLevelValue(string uniqueLevelValue) {
			return this[uniqueLevelValue];
		}
		internal IEnumerable<object> EnumerateUniqueNames() {
			return namesHash.Keys;
		}
		internal IEnumerable<object> EnumerateValues() {
			return valuesHash.Keys;
		}
		internal IEnumerable<OLAPMember> EnumerateMembers() {
			foreach(KeyValuePair<string, OLAPMember> pair in namesHash)
				if(!pair.Value.IsTotal)
					yield return pair.Value;
		}
		IEnumerator<OLAPMember> IEnumerable<OLAPMember>.GetEnumerator() {
			return GetEnumeratorCore();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumeratorCore();
		}
	}
}
