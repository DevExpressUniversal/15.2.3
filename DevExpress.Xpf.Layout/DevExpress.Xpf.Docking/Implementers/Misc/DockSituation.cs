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
using DevExpress.Xpf.Layout.Core;
using System.Windows;
using System.Windows.Controls;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking {
	public class DockSituation {
		public DockSituation(DockType type, BaseLayoutItem target)
			: this(type, target, SWC.Dock.Left) {
		}
		public DockSituation(DockType type, BaseLayoutItem target, SWC.Dock desiredDock) {
			Type = type;
			DesiredDock = desiredDock;
			DockTarget = target;
			Root = target.GetRoot();
			Width = Height = new GridLength(1, GridUnitType.Star);
		}
		public Point FloatLocation { get; set; }
		public GridLength Width { get; set; }
		public GridLength Height { get; set; }
		public SWC.Dock DesiredDock { get; private set; }
		public AutoHideType AutoHideType { get; internal set; }
		public DockType Type { get; internal set; }
		BaseLayoutItem dockTargetCore;
		GridLength[] Lengths;
		Orientation? LengthsOrientation;
		public BaseLayoutItem DockTarget {
			get { return dockTargetCore; }
			internal set {
				dockTargetCore = value;
				LayoutGroup group = value as LayoutGroup;
				Lengths = GetLengths(group);
				LengthsOrientation = (Lengths != null) ? (Orientation?)group.Orientation : null;
			}
		}
		GridLength[] GetLengths(LayoutGroup group) {
			GridLength[] result = null;
			if(group != null && group.ItemType == LayoutItemType.Group) {
				result = new GridLength[group.Items.Count];
				bool fHorz = group.Orientation == Orientation.Horizontal;
				for(int i = 0; i < result.Length; i++) {
					result[i] = fHorz ? group[i].ItemWidth : group[i].ItemHeight;
				}
			}
			return result;
		}
		public bool TheSameLengths(LayoutGroup group) {
			GridLength[] groupLengths = GetLengths(group);
			if(groupLengths == null || Lengths == null) return false;
			if(groupLengths.Length != Lengths.Length) return false;
			if(LengthsOrientation.HasValue && LengthsOrientation.Value != group.Orientation)
				return false;
			for(int i = 0; i < Lengths.Length; i++) {
				if(Lengths[i] != groupLengths[i])
					return false;
			}
			return true;
		}
		public LayoutGroup Root { get; private set; }
		public static DockSituation GetDockSituation(BaseLayoutItem item) {
			return GetDockSituation(item, GetDesiredDock(item));
		}
		public static DockSituation GetDockSituation(BaseLayoutItem item, SWC.Dock dock) {
			DockSituation situation = new DockSituation(GetDockType(item), item.Parent, dock);
			situation.Width = item.ItemWidth;
			situation.Height = item.ItemHeight;
			return situation;
		}
		internal static SWC.Dock GetDesiredDock(BaseLayoutItem item) {
			Rect r = new Rect(0, 0, 1, 1);
			Stack<SWC.Dock> dockStack = new Stack<SWC.Dock>(4);
			while(item.Parent != null) {
				LayoutGroup group = item.Parent;
				if(!group.IgnoreOrientation && group.GetVisibleItemsCount() > 1) {
					dockStack.Push(GetDockInGroup(item, group));
				}
				item = item.Parent;
			}
			SWC.Dock lastDock = SWC.Dock.Left;
			while(dockStack.Count > 0) {
				lastDock = dockStack.Pop();
				switch(lastDock) {
					case SWC.Dock.Left: r = new Rect(r.Left, r.Top, r.Width * 0.5, r.Height); break;
					case SWC.Dock.Right: r = new Rect(r.Left + r.Width * 0.5, r.Top, r.Width * 0.5, r.Height); break;
					case SWC.Dock.Top: r = new Rect(r.Left, r.Top, r.Width, r.Height * 0.5); break;
					case SWC.Dock.Bottom: r = new Rect(r.Left, r.Top + r.Height * 0.5, r.Width, r.Height * 0.5); break;
				}
			}
			double x = r.Left + r.Width * 0.5; double y = r.Top + r.Height * 0.5;
			return Calc(new Rect(0, 0, 1, 1), new Point(x, y), lastDock);
		}
		static SWC.Dock GetDockInGroup(BaseLayoutItem item, LayoutGroup group) {
			bool isFirst = IsOneOfFirstItems(item, group);
			bool fHorz = group.Orientation == Orientation.Horizontal;
			return isFirst ? (fHorz ? SWC.Dock.Left : SWC.Dock.Top) : (fHorz ? SWC.Dock.Right : SWC.Dock.Bottom);
		}
		static bool IsOneOfFirstItems(BaseLayoutItem item, LayoutGroup group) {
			BaseLayoutItem[] items = group.GetVisibleItems();
			return ((double)System.Array.IndexOf(items, item) < (double)items.Length * 0.5);
		}
		static DockType GetDockType(BaseLayoutItem item) {
			LayoutGroup group = item.Parent;
			if(group is TabbedGroup) return DockType.Fill;
			bool isFirst = IsOneOfFirstItems(item, group);
			bool fHorz = group.IgnoreOrientation || group.Orientation == Orientation.Horizontal;
			return isFirst ? (fHorz ? DockType.Left : DockType.Top) : (fHorz ? DockType.Right : DockType.Bottom);
		}
		static SWC.Dock Calc(Rect r, Point p, SWC.Dock lastDockType) {
			SWC.Dock dock = lastDockType;
			if(r.Contains(p)) {
				if((p.X == p.Y) || (p.X == 1.0 - p.Y)) {
					return lastDockType;
				}
				bool f1 = (p.Y - p.X) < (r.Top - r.Left);
				bool f2 = (p.Y + p.X) < (r.Top + r.Right);
				bool f3 = (p.Y - p.X) > (r.Bottom - r.Right);
				bool f4 = (p.Y + p.X) > (r.Bottom + r.Left);
				if(f1 && f2 && (p.Y < r.Top + r.Height * 0.5)) {
					dock = SWC.Dock.Top;
				}
				if(f3 && f4 && (p.Y > r.Top + r.Height * 0.5)) {
					dock = SWC.Dock.Bottom;
				}
				if(!f1 && !f4 && (p.X < r.Left + r.Width * 0.5)) {
					dock = SWC.Dock.Left;
				}
				if(!f2 && !f3 && (p.X > r.Left + r.Width * 0.5)) {
					dock = SWC.Dock.Right;
				}
			}
			return dock;
		}
		internal BaseLayoutItem OriginalItem { get; set; }
	}
}
