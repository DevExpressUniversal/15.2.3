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
namespace DevExpress.XtraSpreadsheet.Model {
	#region GroupCollector
	public abstract class GroupCollector {
		#region fields
		Worksheet sheet;
		#endregion
		protected GroupCollector(Worksheet sheet) {
			this.sheet = sheet;
		}
		#region Properties
		protected Worksheet Sheet { get { return sheet; } }
		#endregion
		public GroupList StrongCollectGroups(int firstIndex, int lastIndex) {
			GroupList result = CreateGroupList();
			if (!HaveGroups())
				return result;
			for (int i = GetFirstVisibleIndex(firstIndex); i <= lastIndex; i++)
				result.ProcessLevel(GetOutlineLevel(i), i, IsHidenElement(i));
			result.AllowOpenNewGroups = false;
			while (result.OpenedGroups.Count > 0) {
				lastIndex++;
				result.ProcessLevel(GetOutlineLevel(lastIndex), lastIndex, IsHidenElement(lastIndex));
			}
			result.EndProcessGroups();
			return result;
		}
		public GroupList CollectGroups(int firstIndex, int lastIndex) {
			GroupList result = CreateGroupList();
			if (!HaveGroups())
				return result;
			GroupCache cache = GetGroupCache();
			if (cache == null || !cache.GroupsIsCached(firstIndex, lastIndex) || GetOutlineLevel(firstIndex) != GetOutlineLevel(Math.Max(firstIndex - 1, 0))) {
				for (int i = GetFirstVisibleIndex(firstIndex - 1); i <= lastIndex + 1; i++)
					result.ProcessLevel(GetOutlineLevel(i), i, IsHidenElement(i));
				while (result.OpenedGroups.Count > 0) {
					lastIndex++;
					result.ProcessLevel(GetOutlineLevel(lastIndex), lastIndex, IsHidenElement(lastIndex));
				}
				result.EndProcessGroups();
				SetGroupCache(new GroupCache(result, GetFirstVisibleIndex(firstIndex - 1), lastIndex));
			}
			else
				result = cache.Groups;
			return result;
		}
		int GetFirstVisibleIndex(int firstIndex) {
			for (int i = firstIndex; i >= 0; i--)
				if (GetOutlineLevel(i) == 0)
					return i;
			return 0;
		}
		protected abstract int GetOutlineLevel(int modelIndex);
		protected abstract bool IsHidenElement(int modelIndex);
		protected abstract GroupList CreateGroupList();
		protected abstract bool HaveGroups();
		protected abstract GroupCache GetGroupCache();
		protected abstract void SetGroupCache(GroupCache cache);
	}
	#endregion
	#region RowGroupCollector
	public class RowGroupCollector : GroupCollector {
		public RowGroupCollector(Worksheet sheet)
			: base(sheet) {
		}
		protected override int GetOutlineLevel(int modelIndex) {
			Row row = Sheet.Rows.TryGetRow(modelIndex);
			return row == null ? 0 : row.OutlineLevel;
		}
		protected override bool IsHidenElement(int modelIndex) {
			Row row = Sheet.Rows.TryGetRow(modelIndex);
			return row == null ? false : row.IsHidden;
		}
		protected override GroupList CreateGroupList() {
			return new GroupList(Sheet, true);
		}
		protected override bool HaveGroups() {
			return Sheet.Properties.FormatProperties.OutlineLevelRow != 0;
		}
		protected override GroupCache GetGroupCache() {
			return Sheet.RowGroupCache;
		}
		protected override void SetGroupCache(GroupCache cache) {
			Sheet.RowGroupCache = cache;
		}
	}
	#endregion
	#region ColGroupCollector
	public class ColumnGroupCollector : GroupCollector {
		public ColumnGroupCollector(Worksheet sheet)
			: base(sheet) {
		}
		protected override int GetOutlineLevel(int modelIndex) {
			Column column = Sheet.Columns.TryGetColumn(modelIndex);
			return column == null ? 0 : column.OutlineLevel;
		}
		protected override bool IsHidenElement(int modelIndex) {
			Column column = Sheet.Columns.TryGetColumn(modelIndex);
			return column == null ? false : column.IsHidden;
		}
		protected override GroupList CreateGroupList() {
			return new GroupList(Sheet, false);
		}
		protected override bool HaveGroups() {
			return Sheet.Properties.FormatProperties.OutlineLevelCol != 0;
		}
		protected override GroupCache GetGroupCache() {
			return Sheet.ColumnGroupCache;
		}
		protected override void SetGroupCache(GroupCache cache) {
			Sheet.ColumnGroupCache = cache;
		}
	}
	#endregion
}
