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
using System.Windows;
using System;
using System.Linq;
using DevExpress.Internal;
using System.Windows.Controls;
using System.Windows.Documents;
namespace DevExpress.Diagram.Core.Layout {
	public struct PositionInfo<T> {
		public readonly T Item;
		public readonly Point Position;
		public PositionInfo(T item, Point position) {
			Item = item;
			Position = position;
		}
		public PositionInfo<T> Offset(Point offset) {
			return new PositionInfo<T>(Item, Position.OffsetPoint(offset));
		}
		public override string ToString() {
			return Position.ToString() + ": " + Item.ToString();
		}
	}
	public struct LayoutInfo<T> {
		public readonly IEnumerable<PositionInfo<T>> Positions;
		public readonly Size TotalSize;
		public LayoutInfo(IEnumerable<PositionInfo<T>> positions, Size totalSize) {
			Positions = positions;
			TotalSize = totalSize;
		}
	}
	public struct GraphTreeLayoutSettings {
		public readonly TreeLayoutSettings TreeLayout;
		public readonly Size PageSize;
		public GraphTreeLayoutSettings(TreeLayoutSettings treeLayout, Size pageSize) {
			TreeLayout = treeLayout;
			PageSize = pageSize;
		}
	}
	public struct TreeLayoutSettings {
		public readonly Size Buffer;
		public readonly double Margin;
		readonly Direction? direction;
		public Direction Direction { get { return direction ?? Direction.Down; } }
		public Orientation DepthOrientation { get { return Direction.GetOrientation(); } }
		public Orientation BreadthOrientation { get { return Direction.GetOrientation().Rotate(); } }
		public LogicalDirection LogicalDirection { get { return Direction.GetLogicalDirection(); } }
		public TreeLayoutSettings(double horizontalBuffer, double verticalBuffer, Direction direction = Direction.Down, double margin = 0) {
			Buffer = new Size(horizontalBuffer, verticalBuffer);
			this.direction = direction;
			this.Margin = margin;
		}
	}
	public static class TreeLayout {
		static Tuple<double, Point> GetOffsetAndAdvanceDepth(this GraphTreeLayoutSettings graphSettings, Size size, double totalDepth) {
			var settings = graphSettings.TreeLayout;
			var depthDelta = settings.LogicalDirection.GetDirectedValue(settings.DepthOrientation.GetSize(size));
			if(settings.LogicalDirection == LogicalDirection.Backward)
				totalDepth += depthDelta;
			var offset = settings.BreadthOrientation.MakePoint(
				(settings.BreadthOrientation.GetSize(graphSettings.PageSize) - settings.BreadthOrientation.GetSize(size)) / 2,
				totalDepth
			);
			if(settings.LogicalDirection == LogicalDirection.Forward)
				totalDepth += depthDelta;
			totalDepth += settings.LogicalDirection.GetDirectedValue(settings.DepthOrientation.GetSize(settings.Buffer));
			return Tuple.Create(totalDepth, offset);
		}
		public static IEnumerable<PositionInfo<T>> LayoutGraph<T>(IEnumerable<Connection<T>> connections, Func<T, Size> getSize, GraphTreeLayoutSettings settings) {
			var totalDepth = settings.TreeLayout.LogicalDirection.ChooseDirectionStartValue(settings.TreeLayout.Margin, settings.TreeLayout.DepthOrientation.GetSize(settings.PageSize) - settings.TreeLayout.Margin);
			foreach(var treeWithItems in GraphOperations.SplitToForest(connections)) {
				var treeLayout = LayoutTree(treeWithItems.Tree.Root, getSize, treeWithItems.Tree.GetChildren, settings.TreeLayout);
				var offset = settings.GetOffsetAndAdvanceDepth(treeLayout.TotalSize, totalDepth)
					.Do(x => totalDepth = x.Item1)
					.Item2;
				foreach(var position in treeLayout.Positions) {
					yield return position.Offset(offset);
				}
				foreach(var itemNotInTree in treeWithItems.Items) {
					var position = settings.GetOffsetAndAdvanceDepth(getSize(itemNotInTree), totalDepth)
						.Do(x => totalDepth = x.Item1)
						.Item2;
					yield return new PositionInfo<T>(itemNotInTree, position);
				}
			}
		}
		public static LayoutInfo<T> LayoutTree<T>(T root, Func<T, Size> getSize, Func<T, IEnumerable<T>> getChildren, TreeLayoutSettings settings) {
			var originalGetSize = getSize;
			getSize = x => {
				var size = originalGetSize(x);
				if(size.IsNotaSize())
					throw new InvalidOperationException();
				return size;
			};
			Func<TreeWrapper<T, double>, double> getTotalBreadth = x => Math.Max(x.Value, settings.BreadthOrientation.GetSize(getSize(x.Item)));
			Func<Size, double> getDepthSize = x => settings.DepthOrientation.GetSize(x);
			Func<Size, double> getBreadthSize = x => settings.BreadthOrientation.GetSize(x);
			Func<T, double> getItemDepthSize = x => getDepthSize(getSize(x));
			Func<T, double> getItemBreadthSize = x => getBreadthSize(getSize(x));
			#region measure
			var maxDepth = 0d;
			var sizeTree = TreeExtensions.TransformTree<T, T, double, double>(
				root: root,
				state: 0,
				getChildren: getChildren,
				getValue: (children, item, depth) => {
					maxDepth = Math.Max(maxDepth, depth + getItemDepthSize(item));
					return children
						.Select(getTotalBreadth)
						.InsertDelimeter(getBreadthSize(settings.Buffer))
						.Sum();
				},
				getItem: x => x,
				getFirstChildState: (depth, item) => depth + getItemDepthSize(item) + getDepthSize(settings.Buffer),
				advanceChildState: (depth, _) => depth
			);
			#endregion
			#region arrange
			var positionTree = TreeExtensions.TransformTree<TreeWrapper<T, double>, T, Point, Point>(
				root: sizeTree,
				state: new Point(0, settings.LogicalDirection.ChooseDirectionStartValue(0, maxDepth)),
				getChildren: x => x.Children,
				getValue: (children, item, position) => {
					var itemSize = getSize(item.Item);
					var itemOffset = Math.Max(0, (item.Value - getBreadthSize(itemSize)) / 2);
					var directedStartValue = settings.LogicalDirection.ChooseDirectionStartValue(0, getDepthSize(itemSize));
					var absolutePoint = position.OffsetPoint(new Point(itemOffset, settings.LogicalDirection.GetDirectedValue(directedStartValue)));
					return settings.BreadthOrientation.MakePoint(absolutePoint.X, absolutePoint.Y);
				},
				getItem: x => x.Item,
				getFirstChildState: (position, item) => {
					var childrenOffset = Math.Max(0, (-item.Value + getItemBreadthSize(item.Item)) / 2);
					var offset = getItemDepthSize(item.Item) + getDepthSize(settings.Buffer);
					return position.OffsetPoint(new Point(childrenOffset, settings.LogicalDirection.GetDirectedValue(offset)));
				},
				advanceChildState: (position, item) => {
					return position.OffsetPoint(new Point(getTotalBreadth(item) + getBreadthSize(settings.Buffer), 0));
				}
			);
			#endregion
			return new LayoutInfo<T>(
				positionTree.Yield()
					.Flatten(x => x.Children)
					.Select(x => new PositionInfo<T>(x.Item, x.Value)),
				settings.DepthOrientation.MakeSize(maxDepth, Math.Max(sizeTree.Value, getItemBreadthSize(sizeTree.Item)))
			);
		}
	}
}
