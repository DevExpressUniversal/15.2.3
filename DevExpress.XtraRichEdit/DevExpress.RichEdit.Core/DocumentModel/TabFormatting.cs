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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Model {
	#region TabAlignmentType
	public enum TabAlignmentType {
		Left,
		Center,
		Right,
		Decimal
	}
	#endregion
	#region TabLeaderType
	public enum TabLeaderType {
		None,
		Dots,
		MiddleDots,
		Hyphens,
		Underline,
		ThickLine,
		EqualSign
	}
	#endregion
	#region TabInfo
	public class TabInfo {
		#region Fields
		public const TabAlignmentType DefaultAlignment = TabAlignmentType.Left;
		public const TabLeaderType DefaultLeader = TabLeaderType.None;
		TabAlignmentType alignment = DefaultAlignment;
		TabLeaderType leader = DefaultLeader;
		int position;
		bool deleted;
		#endregion
		#region Constructors
		public TabInfo(int position)
			: this(position, DefaultAlignment, DefaultLeader) {
		}
		public TabInfo(int position, TabAlignmentType alignment)
			: this(position, alignment, DefaultLeader) {
		}
		public TabInfo(int position, TabLeaderType leader)
			: this(position, DefaultAlignment, leader) {
		}
		public TabInfo(int position, TabAlignmentType alignment, TabLeaderType leader)
			: this(position, alignment, leader, false) {
		}
		public TabInfo(int position, TabAlignmentType alignment, TabLeaderType leader, bool deleted) {
			this.position = position;
			this.alignment = alignment;
			this.leader = leader;
			this.deleted = deleted;
		}
		#endregion
		#region Properties
		public TabAlignmentType Alignment { get { return alignment; } }
		public TabLeaderType Leader { get { return leader; } }
		public int Position { get { return position; } }
		public bool Deleted { get { return deleted; } }
		public virtual bool IsDefault { get { return false; } }
		#endregion
		public int GetLayoutPosition(DocumentModelUnitToLayoutUnitConverter unitConverter) {
			return unitConverter.ToLayoutUnits(Position);
		}
		public override bool Equals(object obj) {
			TabInfo info = (TabInfo)obj;
			return this.Position == info.Position &&
				this.Alignment == info.Alignment &&
				this.Leader == info.Leader &&
				this.Deleted == info.Deleted;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
	#region DefaultTabInfo
	public class DefaultTabInfo : TabInfo {
		public DefaultTabInfo(int position)
			: base(position) {
		}
		public override bool IsDefault { get { return true; } }
	}
	#endregion
	#region TabInfoComparer
	public class TabInfoComparer : IComparer<TabInfo> {
		public int Compare(TabInfo x, TabInfo y) {
			return x.Position - y.Position;
		}
	}
	#endregion
	#region TabFormattingInfo
	public class TabFormattingInfo : ICloneable<TabFormattingInfo>, ISupportsCopyFrom<TabFormattingInfo>, ISupportsSizeOf {
		static TabInfoComparer comparer = new TabInfoComparer();
		List<TabInfo> tabInfos;
		public TabFormattingInfo() {
			this.tabInfos = new List<TabInfo>();
		}
		public TabInfo this[int index] {
			get { return tabInfos[index]; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentException("value", value);
				tabInfos[index] = value;
				OnChanged();
			}
		}
		public int Count { get { return tabInfos.Count; } }
		public int Add(TabInfo item) {
			int index = FindTabItemIndexByPosition(item.Position);
			if (index >= 0) {
				this[index] = item;
				return index;
			}
			else {
				tabInfos.Add(item);
				OnChanged();
				return Count - 1;
			}
		}
		public int IndexOf(TabInfo item) {
			return tabInfos.IndexOf(item);
		}
		public bool Contains(TabInfo item) {
			return tabInfos.Contains(item);
		}
		public void Remove(TabInfo item) {
			int index = IndexOf(item);
			if (index >= 0)
				tabInfos.RemoveAt(index);
		}
		public void Clear() {
			tabInfos.Clear();
		}
		public void AddRange(TabFormattingInfo from) {
			int count = from.Count;
			for (int i = 0; i < count; i++)
				Add(from[i]);
			OnChanged();
		}
		int FindTabItemIndexByPosition(int position) {
			int count = Count;
			for (int i = 0; i < count; i++)
				if (this[i].Position == position)
					return i;
			return -1;
		}
		public TabInfo FindNextTab(int pos) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (this[i].Position > pos)
					return this[i];
			}
			return null;
		}
		public TabFormattingInfo Clone() {
			TabFormattingInfo clone = new TabFormattingInfo();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(TabFormattingInfo info) {
			this.tabInfos.Clear();
			this.tabInfos.AddRange(info.tabInfos);
		}
		public override bool Equals(object obj) {
			TabFormattingInfo info = (TabFormattingInfo)obj;
			if (info.Count != this.Count)
				return false;
			int count = Count;
			for (int i = 0; i < count; i++)
				if (!this[i].Equals(info[i]))
					return false;
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		internal static TabFormattingInfo Merge(TabFormattingInfo master, TabFormattingInfo slave) {
			TabFormattingInfoWithoutSort result = new TabFormattingInfoWithoutSort();
			result.BeginUpdate();
			try {
				MergeCore(master, slave, result);
			}
			finally {
				result.EndUpdate();
			}
			return result;
		}
		static void MergeCore(TabFormattingInfo master, TabFormattingInfo slave, TabFormattingInfo result) {
			int count = slave.Count;
			for (int i = 0; i < count; i++)
				if (!slave[i].Deleted)
					result.Add(slave[i]);
			count = master.Count;
			for (int i = 0; i < count; i++)
				result.Add(master[i]);
			count = result.Count;
			for (int i = count - 1; i >= 0; i--)
				if (result[i].Deleted)
					result.tabInfos.RemoveAt(i);
		}
		protected virtual void OnChanged() {
			this.tabInfos.Sort(comparer);
		}
		#region TabFormattingInfoWithoutSort
		private class TabFormattingInfoWithoutSort : TabFormattingInfo, IBatchUpdateable, IBatchUpdateHandler {
			BatchUpdateHelper batchUpdateHelper;
			public TabFormattingInfoWithoutSort() {
				this.batchUpdateHelper = new BatchUpdateHelper(this);
			}
			#region IBatchUpdateable implementation
			public void BeginUpdate() {
				batchUpdateHelper.BeginUpdate();
			}
			public void EndUpdate() {
				batchUpdateHelper.EndUpdate();
			}
			public void CancelUpdate() {
				batchUpdateHelper.CancelUpdate();
			}
			BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
			public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
			#endregion
			#region IBatchUpdateHandler implementation
			void IBatchUpdateHandler.OnFirstBeginUpdate() {
			}
			void IBatchUpdateHandler.OnBeginUpdate() {
			}
			void IBatchUpdateHandler.OnEndUpdate() {
			}
			void IBatchUpdateHandler.OnLastEndUpdate() {
				OnChanged();
			}
			void IBatchUpdateHandler.OnCancelUpdate() {
			}
			void IBatchUpdateHandler.OnLastCancelUpdate() {
			}
			#endregion
			protected override void OnChanged() {
				if (!IsUpdateLocked)
					base.OnChanged();
			}
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region TabFormattingInfoCache
	public class TabFormattingInfoCache : UniqueItemsCache<TabFormattingInfo> {
		internal const int EmptyTabFormattingOptionIndex = 0;
		public TabFormattingInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override TabFormattingInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			TabFormattingInfo defaultItem = new TabFormattingInfo();
			return defaultItem;
		}
	}
	#endregion
}
