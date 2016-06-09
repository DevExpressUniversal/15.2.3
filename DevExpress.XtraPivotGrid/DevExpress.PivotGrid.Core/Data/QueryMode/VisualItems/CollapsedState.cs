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
using System.Linq;
using System.Collections.Generic;
using System.IO;
using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
namespace DevExpress.PivotGrid.QueryMode {
	class OLAPCollapsedStateManager : CollapsedStateManager<DevExpress.PivotGrid.OLAP.OLAPCubeColumn> {
		public OLAPCollapsedStateManager(QueryAreas<DevExpress.PivotGrid.OLAP.OLAPCubeColumn> areas, IDataSourceHelpersOwner<DevExpress.PivotGrid.OLAP.OLAPCubeColumn> owner)
			: base(areas, owner) {
		}
		protected override CollapsedStateLoader<DevExpress.PivotGrid.OLAP.OLAPCubeColumn> CreateCollapsedStateLoaderV2() {
			return new OLAPCollapsedStateKeeperV2<DevExpress.PivotGrid.OLAP.OLAPCubeColumn>(Areas, Owner);
		}
		protected override CollapsedStateLoader<DevExpress.PivotGrid.OLAP.OLAPCubeColumn> CreateCollapsedStateLoaderV3() {
			return new OLAPCollapsedStateKeeperV3<DevExpress.PivotGrid.OLAP.OLAPCubeColumn>(Areas, Owner);
		}
	}
	public class CollapsedStateManager<TColumn> where TColumn : QueryColumn {
		QueryAreas<TColumn> areas;
		IDataSourceHelpersOwner<TColumn> owner;
		CollapsedStateLoader<TColumn> v1, v2, v3;
		 CollapsedStateLoader<TColumn> V1 {
			 get { 
				 if(v1 == null)
					 v1 = CreateCollapsedStateLoaderV1();
				 return v1;
			 }
		 }
		 CollapsedStateLoader<TColumn> V2 {
			 get { 
				 if(v2 == null)
					 v2 = CreateCollapsedStateLoaderV2();
				 return v2;
			 }
		 }
		 CollapsedStateLoader<TColumn> V3 {
			 get { 
				 if(v3 == null)
					 v3 = CreateCollapsedStateLoaderV3();
				 return v3;
			 }
		 }
		protected QueryAreas<TColumn> Areas { get { return areas; } }
		protected IDataSourceHelpersOwner<TColumn> Owner { get { return owner; } }
		public CollapsedStateManager(QueryAreas<TColumn> areas, IDataSourceHelpersOwner<TColumn> owner) {
			this.areas = areas;
			this.owner = owner;
		}
		public CollapsedState ReadCollapsedStateFromStream(Stream stream, bool autoExpandGroups) {
			TypedBinaryReader reader = new TypedBinaryReader(stream);
			int readedValue = reader.ReadInt32();
			if(readedValue >= 0)
				return V1.ReadCollapsedStateFromStream(reader, readedValue , true , autoExpandGroups);
			if(readedValue == -2) {
				readedValue = reader.ReadInt32();
				return V1.ReadCollapsedStateFromStream(reader, readedValue, true , autoExpandGroups);
			}
			if(readedValue == -3)
				return V2.ReadCollapsedStateFromStream(reader, readedValue, false, autoExpandGroups);
			if(readedValue == -4)
				return V3.ReadCollapsedStateFromStream(reader, readedValue, false, autoExpandGroups);
			throw new NotImplementedException();
		}
		CollapsedStateLoader<TColumn> CreateCollapsedStateLoaderV1() {
			return new CollapsedStateLoader<TColumn>(areas, owner);
		}
		protected virtual CollapsedStateLoader<TColumn> CreateCollapsedStateLoaderV2() {
			return new CollapsedStateLoaderV2<TColumn>(areas, owner);
		}
		protected virtual CollapsedStateLoader<TColumn> CreateCollapsedStateLoaderV3() {
			return new CollapsedStateLoaderV3<TColumn>(areas, owner);
		}
		public void WriteCollapsedStateToStream(Stream stream, CollapsedState collapsedState) {
			V3.WriteCollapsedStateToStream(stream, collapsedState);
		}
		public CollapsedState SaveCollapsedState(bool isColumn) {
			return V3.SaveCollapsedState(isColumn, false);
		}
		public void LoadCollapsedState(bool isColumn, CollapsedState state) {
			V3.LoadCollapsedState(isColumn, state);
		}
		public void LoadCollapsedStateFromStream(Stream stream, bool autoExpandGroups) {
			LoadCollapsedState(true, ReadCollapsedStateFromStream(stream, autoExpandGroups));
			LoadCollapsedState(false, ReadCollapsedStateFromStream(stream, autoExpandGroups));
		}
		public void SaveCollapsedStateToStream(Stream stream) {
			WriteCollapsedStateToStream(stream, SaveCollapsedState(true));
			WriteCollapsedStateToStream(stream, SaveCollapsedState(false));
		}
	}
	public class CollapsedStateLoader<TColumn> where TColumn : QueryColumn {
		bool IsCollapsedStateEquals(CollapsedState state1, CollapsedState state2) { 
			return ListComparer.ListsEqual(state1.State, state2.State, delegate(object x, object y) {
				LevelCollapsedState val1 = x as LevelCollapsedState,
					val2 = y as LevelCollapsedState;
				return val1.Expanded == val2.Expanded && ListComparer.ListsEqual(val1, val2, delegate(object x1, object y1) {
					object[] array1 = x1 as object[],
						array2 = y1 as object[];
					return ListComparer.ListsEqual(array1, array2, delegate(object x2, object y2) {
						return object.Equals(x2, y2);
					});
				});
			});
		}
		QueryAreas<TColumn> areas;
		IDataSourceHelpersOwner<TColumn> owner;
		public CollapsedStateLoader(QueryAreas<TColumn> areas, IDataSourceHelpersOwner<TColumn> owner) {
			this.areas = areas;
			this.owner = owner;
		}
		public virtual CollapsedState ReadCollapsedStateFromStream(TypedBinaryReader reader, int levelCount, bool useValues, bool autoExpand) {
			CollapsedState result = new CollapsedState(levelCount, useValues, autoExpand, null);
			for(int i = 0; i < levelCount; i++) {
				int groupCount = reader.ReadInt32();
				result.Add(new LevelCollapsedState(false, groupCount));
				for(int j = 0; j < groupCount; j++) {
					object[] values = new object[i + 1];
					for(int k = 0; k < values.Length; k++)
						values[k] = ReadColumnValue(reader, k);
					result[i].Add(values);
				}
			}
			return result;
		}
		protected virtual object ReadColumnValue(TypedBinaryReader reader, int columnIndex) {
			return reader.ReadTypedObject();
		}
		public void WriteCollapsedStateToStream(Stream stream, CollapsedState state) {
			TypedBinaryWriter writer = new TypedBinaryWriter(stream);
			writer.Write(-4);
			writer.Write(state.Count);
			List<NullableDictionary<object, int>> uniqueValues = new List<NullableDictionary<object, int>>();
			int[] counters = new int[state.Count];
			for(int i = 0; i < state.Count; i++)
				uniqueValues.Add(new NullableDictionary<object,int>());
			for(int j = 0; j < state.Count; j++) {
				LevelCollapsedState cstate = state[j];
				for(int i = 0; i < cstate.Count; i++) {
					object[] data = cstate[i];
					for(int l = 0; l < data.Length; l++)
						if(!uniqueValues[l].ContainsKey(data[l])) {
							uniqueValues[l].Add(data[l], counters[l]);
							counters[l] = counters[l] + 1;
						}
				}
			}			
			for(int i = 0; i < uniqueValues.Count; i++) {
				writer.Write(uniqueValues[i].Count);
				foreach(KeyValuePair<object, int> pair in uniqueValues[i])
					WriteColumnObject(writer, pair.Key);
			}
			for(int i = 0; i < state.Count; i++) {
				writer.Write(state[i].Count);
				foreach(object[] values in state[i]) {
					for(int j = 0; j < values.Length; j++)
						writer.Write(uniqueValues[j][values[j]]);
				}
			}
			byte currByte = 0;
			int index = 0;
			for(int i = state.Count - 1; i >= 0; i--) {
				currByte = (byte)(currByte << 1);
				if(state[i].Expanded)
					currByte = (byte)(currByte | 1);
				index++;
				if(index == 8) {
					writer.Write(currByte);
					currByte = 0;
					index = 0;
				}
			}
			if(state.Count != 0)
				writer.Write(currByte);
		}
		protected virtual void WriteColumnObject(TypedBinaryWriter writer, object value) {
			writer.WriteTypedObject(value);
		}
		protected virtual object ReadColumnObject(TypedBinaryReader reader) {
			return reader.ReadTypedObject();
		}
		public CollapsedState SaveCollapsedState(bool isColumn, bool fe) {
			return areas.GetFieldValues(isColumn).GetCollapsedState(areas.GetArea(isColumn).Count, fe, owner.AutoExpandGroups, () => areas.GetArea(!isColumn).Count > 1 && areas.GetFieldValues(!isColumn).IsFullyCollapsed());
		}
		public void LoadCollapsedState(bool isColumn, CollapsedState state) {
			if(IsCollapsedStateEquals(state, SaveCollapsedState(isColumn, state.State.All(f => f.Expanded))))
				return;
			int levelCount = Math.Min(areas.GetArea(isColumn).Count, state.Count);
			if(state.AllLevelsExpanded || levelCount == 0)
				owner.ChangeExpandedAll(isColumn, state.AutoExpandGroups);
			AreaFieldValues fieldValues = areas.GetFieldValues(isColumn);
			for(int i = 0; i < levelCount; i++) {
				Dictionary<GroupInfo, int> expandedIndices = new Dictionary<GroupInfo, int>(state[i].Count);
				for(int j = 0; j < state[i].Count; j++) {
					GroupInfo group = fieldValues.GetByValues(state[i][j], !state.UseValues);
					if(group != null)
						expandedIndices[group] = i;
				}
				owner.ChangeFieldExpanded(!state[i].Expanded, isColumn, expandedIndices.Keys);
				owner.ChangeFieldExpanded(state[i].Expanded, isColumn, fieldValues.GetOppositeStateGroups(state[i].Expanded, expandedIndices, i));
			}
		}
	}
	class CollapsedStateLoaderV2<TColumn> : CollapsedStateLoader<TColumn> where TColumn : QueryColumn {
		List<List<object>> valuesCore;
		public CollapsedStateLoaderV2(QueryAreas<TColumn> areas, IDataSourceHelpersOwner<TColumn> owner)
			: base(areas, owner) {
		}
		public override CollapsedState ReadCollapsedStateFromStream(TypedBinaryReader reader, int readedValue, bool useValues, bool autoExpand) {
			valuesCore = new List<List<object>>();
			int levelCount = reader.ReadInt32();
			for(int i = 0; i < levelCount; i++) {
				int count = reader.ReadInt32();
				List<object> list = new List<object>(count);
				for(int j = 0; j < count; j++)
					list.Add(ReadColumnObject(reader));
				valuesCore.Add(list);
			}
			return base.ReadCollapsedStateFromStream(reader, levelCount, useValues, autoExpand);
		}
		protected override object ReadColumnValue(TypedBinaryReader reader, int columnIndex) {
			return valuesCore[columnIndex][reader.ReadInt32()];
		}
	}
	class CollapsedStateLoaderV3<TColumn> : CollapsedStateLoaderV2<TColumn> where TColumn : QueryColumn {
		public CollapsedStateLoaderV3(QueryAreas<TColumn> areas, IDataSourceHelpersOwner<TColumn> owner)
			: base(areas, owner) {
		}
		public override CollapsedState ReadCollapsedStateFromStream(TypedBinaryReader reader, int readedValue, bool useValues, bool autoExpand) {
			CollapsedState state = base.ReadCollapsedStateFromStream(reader, readedValue, useValues, autoExpand);
			int bit = 8;
			byte currByte = 0;
			int bitOff = 0;
			for(int i = state.State.Count - 1; i >= 0; i--) {
				if(bit == 8) {
					currByte = reader.ReadByte();
					bit = 0;
					if(i >= 8) 
						bitOff = state.State.Count % 8;
					else
						bitOff = 0;
				}
				state.State[i].Expanded = ((currByte >> (i - bitOff) % 8) & 1) != 0;
				bit++;
			}
			return state;
		}
	}
	class OLAPCollapsedStateKeeperV2<TColumn> : CollapsedStateLoaderV2<TColumn> where TColumn : QueryColumn {
		public OLAPCollapsedStateKeeperV2(QueryAreas<TColumn> areas, IDataSourceHelpersOwner<TColumn> owner)
			: base(areas, owner) {
		}
		protected override object ReadColumnObject(TypedBinaryReader reader) {
			return reader.ReadString();
		}
		protected override void WriteColumnObject(TypedBinaryWriter writer, object value) {
			writer.Write((string)value);
		}
	}
	class OLAPCollapsedStateKeeperV3<TColumn> : CollapsedStateLoaderV3<TColumn> where TColumn : QueryColumn {
		public OLAPCollapsedStateKeeperV3(QueryAreas<TColumn> areas, IDataSourceHelpersOwner<TColumn> owner)
			: base(areas, owner) {
		}
		protected override object ReadColumnObject(TypedBinaryReader reader) {
			return reader.ReadString();
		}
		protected override void WriteColumnObject(TypedBinaryWriter writer, object value) {
			writer.Write((string)value);
		}
	}
	public class LevelCollapsedState2 : List<GroupInfo> {
		bool expanded;
		public LevelCollapsedState2(bool expanded) : base() {
			this.expanded = expanded;
		}
		public LevelCollapsedState Convert() {
			LevelCollapsedState state = new LevelCollapsedState(expanded);
			for(int i = 0; i< Count; i++)
				state.Add(this[i].GetInversedHierarchy().Select((f) => f.Member.UniqueLevelValue).Reverse().ToArray());
			return state;
		}
	}
	public class LevelCollapsedState : List<object[]> {
		bool expanded;
		public bool Expanded { 
			get { return expanded; } 
			set { expanded = value; }
		}
		public LevelCollapsedState(bool expanded) : base() {
			this.expanded = expanded;
		}
		public LevelCollapsedState(bool expanded, int count) : base(count) {
			this.expanded = expanded;
		}
		public bool IsFullyCollapsed {
			get {
				return !expanded && Count == 0;
			}
		}
		public bool IsFullyExpanded {
			get {
				return expanded && Count == 0;
			}
		}
	}
	public class CollapsedState {
		bool useValues;
		bool autoExpandGroups;
		public List<LevelCollapsedState> State { get; private set; }
		public int Count { get { return State.Count; } }
		public LevelCollapsedState this[int index]  { get { return State[index]; } }
		public bool UseValues {
			get { return useValues; }
			set { useValues = value; }
		}
		public bool AutoExpandGroups {
			get { return autoExpandGroups; }
			set { autoExpandGroups = value; }
		}
		public bool IsFullyCollapsed {
			get {
				return IsFullState(false);
			}
		}
		public bool IsFullyExpanded {
			get {
				return IsFullState(true);
			}
		}
		public bool AllLevelsExpanded {
			get {
				if(State.Count == 0)
					return AutoExpandGroups;
				return State.All((f) => f.Expanded);
			}
		}
		bool IsFullState(bool expand) {
			if(State.Count == 0)
				return AutoExpandGroups == expand;
			return State.All((f) => expand == f.Expanded && f.Count == 0);
		}
		public CollapsedState(bool useValues, bool autoExpandGroups, List<LevelCollapsedState> state2) {
			this.useValues = useValues;
			this.autoExpandGroups = autoExpandGroups;
			this.State = state2;
		}
		public CollapsedState(int capacity, bool useValues, bool autoExpandGroups, bool[] expanded)
			: base() {
			this.useValues = useValues;
			this.autoExpandGroups = autoExpandGroups;
			State = new List<LevelCollapsedState>();
		}
		public void Add(LevelCollapsedState s3) {
			State.Add(s3);
		}
		public bool IsExpanded(List<QueryMember> members) {
			if(members.Count > 1 && members.Count > State.Count)
				return false;
			object[] vals = new object[members.Count - 1];
			for(int i = 0; i < vals.Length; i++)
				vals[i] = members[i].UniqueLevelValue;
			for(int i = 0; i < vals.Length; i++) {
				LevelCollapsedState level = State[i];
				if(level.IsFullyExpanded) {
					if(i == vals.Length - 1)
						return true;
					continue;
				}
				if(i != 0 && level.IsFullyCollapsed)
					return false;
				bool contains = false;
				for(int j = 0; j < level.Count; j++) {
					object[] toTest = level[i];
					int index = 0;
					while(index != i && toTest[index] == vals[index]) {
						index++;
						if(index == i) {
							contains = true;
						}
						break;
					}
					if(contains)
						break;
				}
				if(contains == level.Expanded)
					return false;
			}
			return true;
		}
	}
}
