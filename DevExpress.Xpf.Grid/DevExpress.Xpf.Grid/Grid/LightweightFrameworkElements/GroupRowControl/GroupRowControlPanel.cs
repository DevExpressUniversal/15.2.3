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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid.GroupRowLayout;
namespace DevExpress.Xpf.Grid {
	public class GroupRowControlPanel : Panel {
		GroupContainer groupsCore;
		public GroupContainer Groups {
			get {
				if(groupsCore == null) {
					groupsCore = new GroupContainer();
					groupsCore.Parent = new GroupPanelVisualItemOwner(this);
				}
				return groupsCore;
			}
		}
		public void ResetGroups() {
			groupsCore = null;
			Children.Clear();
		}
		protected override Size MeasureOverride(Size availableSize) {
			double panelWidth = 0d;
			double panelHeight = 0d;
			foreach(Group group in Groups) {
				Size groupSize = MeasureGroup(group, Math.Max(0d, availableSize.Width - panelWidth), availableSize.Height);
				panelWidth += groupSize.Width;
				if(groupSize.Height > panelHeight)
					panelHeight = groupSize.Height;
			}
			return new Size(panelWidth, panelHeight);
		}
		static Size MeasureGroup(Group group, double availableWidth, double availableHeight) {
			double groupWidth = 0d;
			double groupHeight = 0d;
			foreach(Layer layer in group) {
				Size layerSize = MeasureLayer(layer, availableWidth, availableHeight);
				if(layerSize.Width > groupWidth)
					groupWidth = layerSize.Width;
				if(layerSize.Height > groupHeight)
					groupHeight = layerSize.Height;
			}
			return new Size(groupWidth, groupHeight);
		}
		static Size MeasureLayer(Layer layer, double availableWidth, double availableHeight) {
			double layerWidth = 0d;
			double layerHeight = 0d;
			foreach(Column column in layer) {
				UIElement child = column.Element;
				child.Measure(new Size(Math.Max(0d, availableWidth - layerWidth), availableHeight));
				layerWidth += child.DesiredSize.Width;
				if(child.DesiredSize.Height > layerHeight)
					layerHeight = child.DesiredSize.Height;
			}
			return new Size(layerWidth, layerHeight);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double x = 0.0;
			int counter = 0;
			foreach(Group group in Groups) {
				counter++;
				x += ArrangeGroup(group, x, finalSize, counter == Groups.Count);
			}
			return finalSize;
		}
		static double ArrangeGroup(Group group, double x, Size finalSize, bool isLastGroup) {
			double groupWidth = 0d;
			foreach(Layer layer in group) {
				double layerWidth = ArrangeLayer(layer, x, finalSize, isLastGroup);
				if(layerWidth > groupWidth)
					groupWidth = layerWidth;
			}
			return groupWidth;
		}
		static double ArrangeLayer(Layer layer, double x, Size finalSize, bool isLastGroup) {
			double layerWidth = 0d;
			int counter = 0;
			foreach(Column column in layer) {
				counter++;
				UIElement child = column.Element;
				double childX = x + layerWidth;
				double arrangeWidth = isLastGroup && counter == layer.Count ? Math.Max(0d, finalSize.Width - childX) : child.DesiredSize.Width;
				child.Arrange(new Rect(childX, 0.0d, arrangeWidth, finalSize.Height));
				layerWidth += child.DesiredSize.Width;
			}
			return layerWidth;
		}
	}
}
