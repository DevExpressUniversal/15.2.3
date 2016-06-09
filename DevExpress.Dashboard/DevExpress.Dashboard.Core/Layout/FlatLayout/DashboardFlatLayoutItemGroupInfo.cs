#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardFlatLayoutItemGroupInfo : IDashboardFlatLayoutItemInfo {
		static int SortByTop(IDashboardFlatLayoutItemInfo o1, IDashboardFlatLayoutItemInfo o2) {
			return Comparer<double>.Default.Compare(o1.Top, o2.Top);
		}
		static int SortByLeft(IDashboardFlatLayoutItemInfo o1, IDashboardFlatLayoutItemInfo o2) {
			return Comparer<double>.Default.Compare(o1.Left, o2.Left);
		}
		readonly List<IDashboardFlatLayoutItemInfo> items = new List<IDashboardFlatLayoutItemInfo>();
		public DashboardFlatLayoutItemGroupInfo() { }
		public List<IDashboardFlatLayoutItemInfo> Items { get { return items; } }
		public double Left { get { return this.items.Min(item => item.Left); } }
		public double Top { get { return this.items.Min(item => item.Top); } }
		public double Right { get { return this.items.Max(item => item.Right); } }
		public double Bottom { get { return this.items.Max(item => item.Bottom); } }
		public DashboardLayoutGroupOrientation Orientation {
			get {
				if(Items.Count < 2) return DashboardLayoutGroupOrientation.Vertical;
				if(items[0].Left == Items[1].Left && items[0].Right == Items[1].Right) return DashboardLayoutGroupOrientation.Vertical;
				return DashboardLayoutGroupOrientation.Horizontal;
			}
		}
		public void AddItem(IDashboardFlatLayoutItemInfo item) {
			this.items.Add(item);
			if(Orientation == DashboardLayoutGroupOrientation.Vertical) {
				this.items.Sort(SortByTop);
			}
			else {
				this.items.Sort(SortByLeft);
			}
		}
		public bool CanAdd(IDashboardFlatLayoutItemInfo item) {
			if (Items.Count == 0) return true;
			if (Items.Contains(item)) return false;
			if (Left == item.Left && Right == item.Right && (Top == item.Bottom || Bottom == item.Top)) {
				return Items[0].Left == item.Left && Items[0].Right == item.Right;
			}
			if (Top == item.Top && Bottom == item.Bottom && (Left == item.Right || Right == item.Left)) {
				return Items[0].Top == item.Top && Items[0].Bottom == item.Bottom;
			}
			return false;
		}
		public bool HasOnlyParallelPanes {
			get {
				foreach (IDashboardFlatLayoutItemInfo item in items) {
					if (GetOrientationSize(this) != GetOrientationSize(item))
						return false;
				}
				return true;
			}
		}
		public double GetSize(IDashboardFlatLayoutItemInfo item) {
			return Orientation == DashboardLayoutGroupOrientation.Vertical ? item.Bottom - item.Top : item.Right - item.Left;
		}
		double GetOrientationSize(IDashboardFlatLayoutItemInfo item) {
			return Orientation == DashboardLayoutGroupOrientation.Vertical ? item.Right - item.Left : item.Bottom - item.Top;
		}
	}
}
