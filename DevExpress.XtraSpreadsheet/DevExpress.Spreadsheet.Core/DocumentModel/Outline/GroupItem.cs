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

namespace DevExpress.XtraSpreadsheet.Model {
	#region GroupItem
	public class GroupItem {
		#region fields
		int level;
		int start;
		bool buttonBeforeStart;
		bool row;
		#endregion
		public GroupItem(bool row, int start, int level, bool buttonBeforeStart) {
			this.row = row;
			this.start = start;
			this.level = level;
			this.buttonBeforeStart = buttonBeforeStart;
		}
		public GroupItem(bool row, int start, int end, int level, bool buttonBeforeStart)
			: this(row, start, level, buttonBeforeStart) {
			this.End = end;
		}
		#region Properties
		public int Level { get { return level; } }
		public int Start { get { return start; } set { start = value; } }
		public int End { get; set; }
		public int ButtonPosition { get { return buttonBeforeStart ? Start - 1: End + 1; } }
		public bool Collapsed { get; set; }
		public bool HasHidenElement { get; set; }
		public bool Hiden { get; set; }
		protected internal bool ButtonBeforeStart { get { return buttonBeforeStart; } }
		public bool RowGroup { get { return row; } }
		#endregion
		public bool EqualsPosition(GroupItem other) {
			return this.Start == other.Start && this.End == other.End;
		}
		public override bool Equals(object obj) {
			GroupItem other = obj as GroupItem;
			if (other != null)
				return EqualsPosition(other) && other.RowGroup == this.RowGroup;
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
}
