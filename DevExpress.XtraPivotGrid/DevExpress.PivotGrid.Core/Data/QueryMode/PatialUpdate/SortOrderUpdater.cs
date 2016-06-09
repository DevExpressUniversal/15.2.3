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
using DevExpress.PivotGrid.QueryMode.Sorting;
namespace DevExpress.PivotGrid.QueryMode {
	abstract class FieldValuesLevelUpdaterBase<TColumn> : UpdaterBase<TColumn> where TColumn : QueryColumn {
		bool isColumn;
		int level;
		TColumn column;
		protected TColumn Column { get { return column; } }
		protected bool IsColumn { get { return isColumn; } }
		protected int Level { get { return level; } }
		public FieldValuesLevelUpdaterBase(IPartialUpdaterOwner<TColumn> owner, bool isColumn, int level)
			: base(owner) {
			this.isColumn = isColumn;
			this.level = level;
			this.column = owner.Areas.GetArea(isColumn)[level];
		}
		public override void Update() {
			ForEachLevelGroupInfo(GetUpdateAction());
		}
		protected void ForEachLevelGroupInfo(Action<List<GroupInfo>> action) {
			UpdateFieldValues(Owner.Areas.GetFieldValues(isColumn), action, level);
		}
		protected abstract Action<List<GroupInfo>> GetUpdateAction();
	}
	class SortOrderUpdater<TColumn> : FieldValuesLevelUpdaterBase<TColumn> where TColumn : QueryColumn {
		public SortOrderUpdater(IPartialUpdaterOwner<TColumn> owner, bool isColumn, int level)
			: base(owner, isColumn, level) {
		}
		protected override Action<List<GroupInfo>> GetUpdateAction() {
			if(Level == 0)
				return (g) => g.Reverse(1, g.Count - 1);
			else
				return (g) => g.Reverse();
		}
	}
	class SortModeUpdater<TColumn> : FieldValuesLevelUpdaterBase<TColumn> where TColumn : QueryColumn {
		public SortModeUpdater(IPartialUpdaterOwner<TColumn> owner, bool isColumn, int level)
			: base(owner, isColumn, level) {
		}
		protected override Action<List<GroupInfo>> GetUpdateAction() {
			IComparer<GroupInfo> comparer = CellTableSorter<TColumn>.GetComparer(Areas, Column, IsColumn);
			if(Level == 0)
				return (g) => g.Sort(1, g.Count - 1, comparer);
			else
				return (g) => g.Sort(comparer);
		}
	}
}
