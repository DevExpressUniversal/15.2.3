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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking {
	public static class DockPreviewCalculator {
		public static Size DockPreviewItem(Rect bounds, BaseLayoutItem dockItem, BaseLayoutItem targetItem, DockType type) {
			bool fHorz = type.ToOrientation() == Orientation.Horizontal;
			Space[] spaces = CreateItemSpaces(targetItem);
			return MeasureSpaces(bounds, spaces, CreateSpace(dockItem, fHorz), type);
		}
		public static Size DockPreviewGroup(Rect bounds, BaseLayoutItem dockItem, LayoutGroup targetGroup, DockType type) {
			bool fHorz = type.ToOrientation() == Orientation.Horizontal;
			Space[] spaces = !targetGroup.IgnoreOrientation && (type.ToOrientation() == targetGroup.Orientation) ?
				CreateGroupSpaces(targetGroup, fHorz) : new Space[] { CreateSpace(targetGroup, fHorz) };
			return MeasureSpaces(bounds, spaces, CreateSpace(dockItem, fHorz), type);
		}
		static Size MeasureSpaces(Rect bounds, Space[] panels, Space measuredPanel, DockType type) {
			Grid grid = new Grid();
			int index = (type.ToInsertType() == InsertType.Before) ? 0 : panels.Length;
			if(type.ToOrientation() == Orientation.Horizontal) {
				int c = (index == 0) ? 1 : 0;
				for(int i = 0; i < panels.Length; i++) {
					grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = panels[i].Length });
					Grid.SetColumn(panels[i], c++);
					grid.Children.Add(panels[i]);
				}
				grid.ColumnDefinitions.Insert(index, new ColumnDefinition() { Width = measuredPanel.Length });
				Grid.SetColumn(measuredPanel, index);
			}
			else {
				int r = (index == 0) ? 1 : 0;
				for(int i = 0; i < panels.Length; i++) {
					grid.RowDefinitions.Add(new RowDefinition() { Height = panels[i].Length });
					Grid.SetRow(panels[i], r++);
					grid.Children.Add(panels[i]);
				}
				grid.RowDefinitions.Insert(index, new RowDefinition() { Height = measuredPanel.Length });
				Grid.SetRow(measuredPanel, index);
			}
			grid.Children.Add(measuredPanel);
			grid.Arrange(bounds);
			return new Size(measuredPanel.ActualWidth, measuredPanel.ActualHeight);
		}
		static Space[] CreateGroupSpaces(LayoutGroup group, bool fHorz) {
			Space[] spaces = new Space[group.Items.Count];
			for(int i = 0; i < spaces.Length; i++) {
				BaseLayoutItem item = group[i];
				spaces[i] = new Space(fHorz ? item.ItemWidth : item.ItemHeight);
			}
			return spaces;
		}
		static Space[] CreateItemSpaces(BaseLayoutItem item) {
			if(LayoutItemsHelper.IsEmptyLayoutGroup(item)) return new Space[0];
			return new Space[] { CreateSpace(item) };
		}
		static Space CreateSpace(BaseLayoutItem item, bool horz) {
			GridLength length = horz ? item.ItemWidth : item.ItemHeight;
			DockSituation situation = item.GetLastDockSituation();
			if(situation != null) {
				GridLength savedLength = horz ? situation.Width : situation.Height;
				if(savedLength.IsAbsolute) length = savedLength;
			}
			return new Space(length);
		}
		static Space CreateSpace(BaseLayoutItem item) {
			double avg = 1.0;
			if(item is LayoutGroup) avg = GetStarDockSize(item as LayoutGroup);
			return new Space(new GridLength(avg, GridUnitType.Star));
		}
		public static double GetStarDockSize(LayoutGroup group) {
			if(group.Items.Count == 0) return 1;
			bool fHorz = group.Orientation == Orientation.Horizontal;
			double total = 0;
			for(int i = 0; i < group.Items.Count; i++) {
				GridLength length = fHorz ? group[i].ItemWidth : group[i].ItemHeight;
				if(length.IsStar) total += length.Value;
				else {
					total = (double)group.Items.Count;
					break;
				}
			}
			return total / (double)group.Items.Count;
		}
		class Space : FrameworkElement {
			public readonly GridLength Length;
			public Space(GridLength length) {
				Length = length;
			}
		}
	}
}
