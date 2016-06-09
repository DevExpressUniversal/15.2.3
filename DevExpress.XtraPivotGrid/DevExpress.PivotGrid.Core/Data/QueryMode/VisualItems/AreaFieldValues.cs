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
using DevExpress.Data.IO;
namespace DevExpress.PivotGrid.QueryMode {
	public class AreaFieldValues {
		static bool CompareValues(QueryMember member, object value, bool useULV) {
			return useULV ? object.Equals(member.UniqueLevelValue, value) : object.Equals(member.Value, value);
		}
		public static QueryMember[] GetHierarchyMembers(GroupInfo info) {
			int currIndex = info.Level;
			QueryMember[] values = new QueryMember[currIndex + 1];
			foreach(GroupInfo ninfo in info.GetInversedHierarchy()) {
				values[currIndex] = ninfo.Member;
				currIndex--;
			}
			return values;
		}
		public static int GetMaxLevel(IEnumerable groups) {
			int maxLevel = -1;
			foreach(GroupInfo group in groups)
				if(group.Level > maxLevel)
					maxLevel = group.Level;
			return maxLevel;
		}
		enum InsertState { NotStarted, InProgress, Completed }
		List<GroupInfo> firstLevel = new List<GroupInfo>();
		InsertState insertState = InsertState.NotStarted;
		readonly List<GroupInfo> groups;
		public AreaFieldValues() {
			this.groups = new List<GroupInfo>();
		}
		public int Count { get { return Math.Max(0, GroupCount - 1); } }
		int GroupCount { get { return groups.Count; } }		
		public int MaxLevel { get { return GetMaxLevel(groups); } }
		public GroupInfo this[int index] {
			get { return index >= -1 && index + 1 < groups.Count ? groups[index + 1] : GroupInfo.GrandTotalGroup; }
		}
		public GroupInfo this[int index, int level] {
			get {
				GroupInfo group = this[index];
				while(level < group.Level)
					group = this[--index];
				return group.Level == level ? group : null;
			}
		}
		public void Add(GroupInfo group) {
			if(GroupCount == 0) {
				if(group != GroupInfo.GrandTotalGroup)
					AddGroup(GroupInfo.GrandTotalGroup);
			} else
				if(group == GroupInfo.GrandTotalGroup)
					throw new ArgumentException("GrandTotal group should be placed first");
			AddGroup(group);
		}
		void AddGroup(GroupInfo group) {
			if(group.Level > 0)
				throw new ArgumentException("It's impossible to add a group with level > 0");
			firstLevel.Add(group);
			groups.Add(group);
			if(insertState == InsertState.Completed) {
				insertState = InsertState.NotStarted;
			}
		}
		int updateCount = 0;
		public void BeginUpdate() {
			updateCount++;
			insertState = InsertState.InProgress;
		}
		public void EndUpdate() {
			updateCount--;
			if(updateCount < 0)
				throw new Exception("Invalid end update");
			if(updateCount > 0)
				return;
			groups.Clear();
			foreach(GroupInfo group in firstLevel)
				InsertRecursive(group);
			insertState = InsertState.Completed;
		}
		void InsertRecursive(GroupInfo current) {
			groups.Add(current);
			IList<GroupInfo> children = current.GetChildren();
			if(children != null)
				for(int i = 0; i < children.Count; i++)
					InsertRecursive(children[i]);
		}
		List<GroupInfo> GetRange(int startIndex, int endIndex) {
			return groups.GetRange(startIndex + 1, endIndex - startIndex);
		}
		internal void RemoveChildren(List<GroupInfo> sums) {
			foreach(GroupInfo info in sums) {
				RemoveFromTree(info);
				groups.Remove(info);
			}
		}
		public void Clear(int capacity) {
			Clear();
			groups.Capacity = capacity;
		}
		public void Clear() {
			groups.Clear();
			firstLevel.Clear();
		}
		public List<GroupInfo> RemoveChildren(ICollection<GroupInfo> groups) {
			if(groups.Count == 0)
				return new List<GroupInfo>();
			BeginUpdate();
			List<GroupInfo> result = new List<GroupInfo>(groups.Count);
			foreach(GroupInfo group in groups) {
				List<GroupInfo> children = group.GetChildren();
				if(children != null) {
					ForEachGroupInfoCore((c, d) => result.AddRange(c), children, group.Level, Int32.MaxValue);
					group.ClearChildren();
				}
			}
			EndUpdate();
			return result;
		}
		public List<GroupInfo> RemoveChildren(int index) {
			int count = Count;
			if(index < 0 || index >= count)
				throw new Exception("Field values doesn't contain the group.");
			if(index == count - 1) {
				return new List<GroupInfo>();
			} else {
				int nextIndex = GetNextOrPrevIndex(index, true);
				if(nextIndex < 0)
					nextIndex = count;
				List<GroupInfo> res = GetRange(index + 1, nextIndex);
				this[index].ClearChildren();
				groups.RemoveRange(index + 2, nextIndex - index - 1);
				return res;
			}
		}
		void RemoveFromTree(GroupInfo info) {
			info.RemoveSelf();
			if(info.Level == 0)
				firstLevel.Remove(info);
		}
		public QueryMember[] GetHierarchyMembers(int index) {
			if(index < -1 || index >= Count)
				throw new Exception("Invalid group index.");
			return GetHierarchyMembers(this[index]);
		}
		public ICollection<GroupInfo> GetAllGroupsByLevel(int level) {
			if(level < 0)
				return new GroupInfo[] { GroupInfo.GrandTotalGroup };
			List<GroupInfo> result = new List<GroupInfo>();
			int count = Count;
			for(int i = 0; i < count; i++)
				if(this[i].Level == level)
					result.Add(this[i]);
			return result;
		}
		public ICollection<GroupInfo> GetAllGroupsByValue(int level, object value) {
			if(level < 0)
				return new GroupInfo[] { GroupInfo.GrandTotalGroup };
			List<GroupInfo> result = new List<GroupInfo>();
			for(int i = 0; i < Count; i++) {
				GroupInfo info = this[i];
				if(info.Level == level && object.Equals(info.Member.Value, value))
					result.Add(info);
			}
			return result;
		}
		public GroupInfo[] GetOppositeStateGroups(bool expanded, Dictionary<GroupInfo, int> infos, int level) {
			List<GroupInfo> result = new List<GroupInfo>();
			ForEachGroupInfo((list, glevel) => {
				if(glevel == level)
					foreach(GroupInfo info in list) {
						if(!infos.ContainsKey(info) && ((info.GetChildren() == null) == expanded))
							if(level != 0 || info != GroupInfo.GrandTotalGroup)
								result.Add(info);
					}
			}, level);
			return result.ToArray();
		}
		public GroupInfo GetClosestGroupInfo(List<QueryMember> members) {
			int index = 0;
			GroupInfo currInfo = GroupInfo.GrandTotalGroup;
			if(members.Count == 0)
				return currInfo;
			List<GroupInfo> currLevel = firstLevel;
			bool fp = true;
			bool found = true;
			while(found && currLevel != null && currLevel.Count > 0 && index != members.Count) {
				QueryMember currMember = members[index];
				found = false;
				int start = fp ? 1 : 0;
				for(int i = start; i < currLevel.Count; i++)
					if(object.Equals(currLevel[i].Member.Value, currMember.Value)) {
						currInfo = currLevel[i];
						currLevel = currLevel[i].GetChildren();
						index++;
						found = true;
						break;
					}
				if(!found)
					currLevel = null;
				fp = false;
			}
			return currInfo;
		}
		public int GetIndex(object[] values) {
			return GetIndex(values, false);
		}
		public int GetIndexUseULV(object[] values) {
			return GetIndex(values, true);
		}
		public int GetIndex(object[] values, bool useULV) {
			if(values != null && values.Length > 0) {
				int level = 0;
				for(int i = 0; i < Count; i++) {
					if(this[i].Level == level) {
						if(CompareValues(this[i].Member, values[level], useULV)) {
							level++;
							if(level == values.Length)
								return i;
						}
					} else if(this[i].Level < level)
						break;
				}
			}
			return -1;
		}
		public GroupInfo GetByValues(object[] values, bool useULV) {
			if(values != null && values.Length > 0) {
				int level = 0;
				for(int i = 0; i < Count; i++) {
					if(this[i].Level == level) {
						if(CompareValues(this[i].Member, values[level], useULV)) {
							level++;
							if(level == values.Length)
								return this[i];
						}
					} else if(this[i].Level < level)
						break;
				}
			}
			return null;
		}
#if DEBUGTEST
		public object[] GetValues() {
			object[] result = new object[Count];
			for(int i = 0; i < Count; i++) {
				result[i] = this[i].Member.Value;
			}
			return result;
		}
#endif
		public int GetNextOrPrevIndex(int index, bool isNext) {
			int level = this[index].Level;
			if(isNext) {
				for(int i = index + 1; i < Count; i++)
					if(this[i].Level <= level)
						return i;
			} else {
				for(int i = index - 1; i >= 0; i--)
					if(this[i].Level <= level)
						return i;
			}
			return -1;
		}
		public List<GroupInfo> GetGroupIndexesList() {
			List<GroupInfo> res = new List<GroupInfo>(groups.Count);
			if(GroupCount == 0)
				res.Add(GroupInfo.GrandTotalGroup);
			else
				for(int i = 0; i < GroupCount; i++)
					res.Add(groups[i]);
			return res;
		}
		public Dictionary<GroupInfo, int> GetGroupIndexesDictionary() {
			Dictionary<GroupInfo, int> res = new Dictionary<GroupInfo, int>(groups.Count);
			if(GroupCount == 0)
				res.Add(GroupInfo.GrandTotalGroup, 0);
			else
				for(int i = 0; i < GroupCount; i++)
					res.Add(groups[i], i);
			return res;
		}
		public List<GroupInfo> GetGroups() {
			List<GroupInfo> res = new List<GroupInfo>(groups);
			if(res.Count == 0)
				res.Add(GroupInfo.GrandTotalGroup);
			return res;
		}
		public IEnumerable<GroupInfo> EnumerateChildren(int index) {
			if(index < Count - 1 && this[index + 1].Level > this[index].Level) {
				if(this[index].Level == -1) {
					return EnumerateLevel(0);
				} else {
					return this[index].GetChildren() ?? Enumerable.Empty<GroupInfo>();
				}
			} else {
				return new GroupInfo[] { this[index] };
			}
		}
		public GroupInfo[] ToArray() {
			return ToArray(true);
		}
		protected GroupInfo[] ToArray(bool includeGrandTotal) {
			if(includeGrandTotal) {
				if(GroupCount <= 1)
					return new GroupInfo[0];
				GroupInfo[] result = new GroupInfo[GroupCount - 1];
				groups.CopyTo(1, result, 0, GroupCount - 1);
				return result;
			} else
				return groups.ToArray();
		}
		public virtual void SaveToStream(TypedBinaryWriter writer, Dictionary<IQueryMetadataColumn, int> columnIndexes, Dictionary<QueryMember, int> memberIndexes) {
			writer.Write(GroupCount);
			for(int i = 0; i < GroupCount; i++)
				groups[i].SaveToStream(writer, columnIndexes, memberIndexes);
		}
		public void ForEachGroupInfo(Action<List<GroupInfo>, int> action, int maxLevel = Int32.MaxValue) {
			if(firstLevel.Count > 1)
				ForEachGroupInfoCore(action, firstLevel, 0, maxLevel);
		}
		void ForEachGroupInfoCore(Action<List<GroupInfo>, int> action, List<GroupInfo> level, int currLevel, int maxLevel) {
			action(level, currLevel);
			if(currLevel == maxLevel)
				return;
			List<GroupInfo> children;
			foreach(GroupInfo info in level) {
				children = info.GetChildren();
				if(children != null && children.Count != 0)
					ForEachGroupInfoCore(action, children, currLevel + 1, maxLevel);
			}
		}
		public IEnumerable<GroupInfo> EnumerateLevel(int level) {
			int maxLevel = MaxLevel;
			if(level > maxLevel)
				yield break;
			if(level <= 0) {
				foreach(GroupInfo info in firstLevel)
					if(info.Level == level)
						yield return info;
			} else {
				IEnumerator<GroupInfo>[] enums = new IEnumerator<GroupInfo>[maxLevel + 1];
				enums[0] = firstLevel.GetEnumerator();
				for(int currLevel = 0; currLevel >= 0; ) {
					IEnumerator<GroupInfo> current = enums[currLevel];
					if(current.MoveNext()) {
						if(currLevel == level)
							yield return current.Current;
						else {
							List<GroupInfo> children = current.Current.GetChildren();
							if(children != null && children.Count != 0) {
								currLevel++;
								enums[currLevel] = children.GetEnumerator();
							}
						}
					} else {
						currLevel--;
					}
				}
			}
		}
		public void LoadFromStream(TypedBinaryReader reader, List<IQueryMetadataColumn> columnIndexes, List<QueryMember> memberIndexes, int levelCount) {
			Clear();
			int groupsCount = reader.ReadInt32();
			List<GroupInfo> parents = new List<GroupInfo>();
			for(int i = 0; i < levelCount; i++)
				parents.Add(null);
			for(int i = 0; i < groupsCount; i++) {
				GroupInfo group = LoadFromStreamCore(reader, columnIndexes, memberIndexes, parents);
				if(group.Level != -1)
					parents[group.Level] = group;
				if(group.Level <= 0)
					firstLevel.Add(group);
				groups.Add(group);
			}
		}
		protected virtual GroupInfo LoadFromStreamCore(TypedBinaryReader reader, List<IQueryMetadataColumn> columnIndexes, List<QueryMember> memberIndexes, List<GroupInfo> parents) {
			int level = reader.ReadInt32(),
	memberIndex = reader.ReadInt32();
			if(memberIndex == GroupInfo.GrandTotalIndex)
				return GroupInfo.GrandTotalGroup;
			QueryMember member = memberIndex < 0 ? columnIndexes[-memberIndex].GetAllMember() : memberIndexes[memberIndex];
			if(level > 0)
				return new GroupInfo(level, member, parents[level - 1]);
			else
				return new GroupInfo(level, member, null);
		}
		internal CollapsedState GetCollapsedState(int areaCount, bool fe, bool autoExpandAllGroups, Func<bool> isCrossCollapsed) {
			areaCount = Math.Max(0, areaCount - 1);
			List<LevelCollapsedState2> expanded = new List<LevelCollapsedState2>(areaCount);
			List<LevelCollapsedState2> collapsed = new List<LevelCollapsedState2>(areaCount);
			for(int i = 0; i < areaCount; i++) {
				expanded.Add(new LevelCollapsedState2(false));
				collapsed.Add(new LevelCollapsedState2(true));
			}
			List<GroupInfo> nextLevel = new List<GroupInfo>(firstLevel);
			if(firstLevel.Count > 0 && firstLevel[0] == GroupInfo.GrandTotalGroup)
				nextLevel.RemoveAt(0);
			int currLevel = 0;
			while(nextLevel.Count > 0 && currLevel < areaCount) {
				List<GroupInfo> currLevelItems = nextLevel;
				nextLevel = new List<GroupInfo>();
				for(int i = 0; i < currLevelItems.Count; i++) {
					GroupInfo info = currLevelItems[i];
					List<GroupInfo> ch = info.GetChildren();
					if(ch == null || ch.Count == 0) {
						collapsed[currLevel].Add(info);
					} else {
						expanded[currLevel].Add(info);
						nextLevel.AddRange(ch);
					}
				}
				currLevel++;
			}
			List<LevelCollapsedState> exState = new List<LevelCollapsedState>(areaCount + 1);
			if(fe)
				for(int i = 0; i < areaCount; i++)
					exState.Add(expanded[i].Convert());
			else
				for(int i = 0; i < areaCount; i++)
					if(expanded[i].Count > collapsed[i].Count)
						exState.Add(collapsed[i].Convert());
					else
						if(expanded[i].Count < collapsed[i].Count)
							exState.Add(expanded[i].Convert());
						else
							if(autoExpandAllGroups && (Count < 2 || i == 0 || !exState[i - 1].IsFullyCollapsed) || Count > 1 && i != 0 && exState[i - 1].IsFullyExpanded)
								exState.Add(collapsed[i].Convert());
							else
								exState.Add(expanded[i].Convert());
			bool isLastFullyExp = autoExpandAllGroups;
			if(exState.Count > 0)
				isLastFullyExp = isLastFullyExp && !exState[exState.Count - 1].IsFullyCollapsed;
			else 
				isLastFullyExp = isLastFullyExp && !isCrossCollapsed();
			exState.Add(new LevelCollapsedState(isLastFullyExp));
			return new CollapsedState(false, false, exState);
		}
		public bool ForAnyChild(int startIndex, Func<int, bool> method) {
			int level = this[startIndex - 1].Level;
			for(int i = startIndex; i < Count + 1; i++) {
				if(this[i].Level <= level)
					return true;
				if(!method(i))
					return false;
			}
			return true;
		}
		public bool ForAnyGroup(Func<GroupInfo, bool> method) {
			int count = groups.Count;
			for(int i = 0; i < count; i++)
				if(method(groups[i]))
					return true;
			return false;
		}
		internal int IndexOf(GroupInfo igroupInfo) {
			return IndexOf(0, igroupInfo);
		}
		int IndexOf(int searchFromIndex, GroupInfo igroupInfo) {
			if(igroupInfo != GroupInfo.GrandTotalGroup) {
				int index = groups.IndexOf(igroupInfo, searchFromIndex);
				if(index < 0)
					index = groups.IndexOf(igroupInfo, 0, searchFromIndex);
				return index;
			} else {
				return 0;
			}
		}
		public int GetLastNotExpandedLevel() {
			int lastUnexpanded = -1;
			if(groups.Count == 0)
				return -1;
			GroupInfo lastGroupInfo = groups[0];
			for(int i = 1; i < groups.Count; i++) {
				GroupInfo newGroupInfo = groups[i];
				if(lastGroupInfo.Level <= newGroupInfo.Level)
					lastUnexpanded = lastGroupInfo.Level;
				lastGroupInfo = newGroupInfo;
			}
			return Math.Max(lastUnexpanded, lastGroupInfo.Level);
		}
		public int[] GetAllNotExpandedIndexes(int level) {
			if(level < 0)
				return new int[] { -1 };
			List<int> result = new List<int>();
			for(int i = 0; i < Count - 1; i++)
				if(this[i].Level == level && this[i + 1].Level <= level)
					result.Add(i);
			if(this[Count - 1].Level == level)
				result.Add(Count - 1);
			return result.ToArray();
		}
		public bool IsFullyExpanded() {
			int count = Count;
			if(count == 0)
				return true;
			int lastLevel = -1;
			int lastGroupInfoLevel = this[0].Level;
			for(int i = 1; i < count; i++) {
				GroupInfo gi = this[i];
				if(lastGroupInfoLevel >= gi.Level) {
					if(lastLevel == -1)
						lastLevel = lastGroupInfoLevel;
					else
						if(lastLevel != lastGroupInfoLevel)
							return false;
				}
				lastGroupInfoLevel = gi.Level;
			}
			return this[count - 1].Level == lastLevel;
		}
		public bool IsFullyCollapsed() {
			return firstLevel.Count > 1 && !firstLevel.Any(f => f.HasChildren);
		}
	}
}
