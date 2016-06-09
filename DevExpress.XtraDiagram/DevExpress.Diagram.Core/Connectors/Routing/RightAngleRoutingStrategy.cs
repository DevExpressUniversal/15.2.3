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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Diagram.Core.Native;
using DevExpress.Diagram.Core.Routing;
using DevExpress.Internal;
using DevExpress.Utils;
namespace DevExpress.Diagram.Core.Routing {
	public static class HeuristicFunctions {
		public static double Manhattan(Point point1, Point point2) {
			return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
		}
		public static double Euclidean(Point point1, Point point2) {
			return point1.Distance(point2);
		}
		public static double DistanceMagnitude(Point point1, Point point2) {
			return Point.Subtract(point1, point2).LengthSquared;
		}
	}
	public class RightAngleRoutingStrategy : RoutingStrategy {
		public int ItemMargin { get; set; }
		public bool AvoidShapes {
			get { return runner.AvoidShapes; }
			set { runner.AvoidShapes = value; }
		}
		readonly AStarRoutingRunner<IDiagramItem> runner;
		public RightAngleRoutingStrategy() {
			ItemMargin = 15;
			runner = new AStarRoutingRunner<IDiagramItem>(item => BoundsCache[item]);
		}
		Dictionary<IDiagramItem, Rect> BoundsCache;
		protected override IEnumerable<Point> GetRoutePoints(IEnumerable<IDiagramItem> items, ConnectorProxy proxy) {
			var begin = proxy.ActualPoint(ConnectorPointType.Begin);
			var end = proxy.ActualPoint(ConnectorPointType.End);
			Predicate<IDiagramItem> func = item => proxy.Begin.Item != item && proxy.End.Item != item && CanIgnoreItem(item, begin, end);
			BoundsCache = items.ToDictionary(item => item, item => item.RotatedDiagramBounds().RotatedRect);
			runner.ItemMargin = ItemMargin;
			var route = runner.FindPath(items, func, begin, end);
			BoundsCache = null;
			return route;
		}
		bool CanIgnoreItem(IDiagramItem item, Point begin, Point end) {
			var itemBounds = BoundsCache[item].InflateRect(ItemMargin);
			return itemBounds.Contains(begin) || itemBounds.Contains(end);
		}
	}
	class AStarRoutingRunner<TItem> {
		public Func<Point, Point, double> HeuristicFunction { get; set; }
		public int ItemMargin { get; set; }
		readonly Func<TItem, Rect> GetBoundsFunc;
		const int StepWeight = 10;
		int Penalty { get; set; }
		const int IntersectionWegiht = StepWeight * 1000;
		const int MaxStepsCount = 10000;
		public bool AvoidShapes { get; set; }
		public AStarRoutingRunner(Func<TItem, Rect> getBounds) {
			HeuristicFunction = HeuristicFunctions.Manhattan;
			ItemMargin = 10;
			GetBoundsFunc = item => {
				Rect bounds = getBounds(item);
				return new Rect(bounds.TopLeft.RoundPoint(), bounds.BottomRight.RoundPoint());
			};
			AvoidShapes = true;
		}
		public IEnumerable<Point> FindPath(IEnumerable<TItem> items, Predicate<TItem> ignoreItem, Point start, Point target) {
			SortedSet<AStarNode> openSet = new SortedSet<AStarNode>();
			HashSet<Point> closedSet = new HashSet<Point>();
			var begin = start.RoundPoint();
			var end = target.RoundPoint();
			var grid = new DiagramGrid(begin, end, items, GetBoundsFunc, ItemMargin);
			var intersectionHelper = GetIntersectionInfo(items);
			intersectionHelper.ItemMargin = ItemMargin - 1;
			AStarNode current = new AStarNode(begin);
			current.H = CalculateHeuristic(current.Position, end);
			openSet.Add(current);
			Penalty = (int)current.H;
			int stepsCount = 0;
			while(openSet.Any() && ++stepsCount < MaxStepsCount) {
				current = openSet.Min;
				if(end.Equals(current.Position))
					return current.GetPath(begin, end);
				openSet.Remove(current);
				closedSet.Add(current.Position);
				var neighborPoints = grid.GetNeighbors(current.Position)
					.Where(p => !closedSet.Contains(p));
				foreach(Point neighborPoint in neighborPoints) {
					var openPoint = openSet.FirstOrDefault(p => p.Position.Equals(neighborPoint));
					var intersections = intersectionHelper.GetIntersections(neighborPoint, ignoreItem);
					double g = current.G + CalculateG(current, neighborPoint, begin, end, intersections);
					if(openPoint == null) {
						openPoint = new AStarNode(neighborPoint);
						openPoint.Parent = current;
						openPoint.G = g;
						openPoint.H = CalculateHeuristic(neighborPoint, end);
						openSet.Add(openPoint);
					} else if(openPoint.G > g) {
						openSet.Remove(openPoint);
						openPoint.Parent = current;
						openPoint.G = g;
						openSet.Add(openPoint);
					}
				}
			}
			return Enumerable.Empty<Point>();
		}
		double CalculateG(AStarNode node, Point point, Point start, Point target, IEnumerable<TItem> intersections) {
			if(start.Equals(point) || target.Equals(point))
				return StepWeight;
			int penaltyCoef = 0;
			if(node.Parent != null && MathHelper.IsCorner(point, node.Position, node.Parent.Position))
				penaltyCoef++;
			if(AvoidShapes) {
				intersections.ForEach(item => {
					var bounds = GetBoundsFunc(item);
					if(bounds.Top == point.Y || bounds.Bottom == point.Y || bounds.Left == point.X || bounds.Right == point.X) {
						penaltyCoef += 3;
					} else if(bounds.Contains(point)) {
						penaltyCoef += 4;
					}
				});
			}
			return StepWeight + penaltyCoef * Penalty;
		}
		int CalculateHeuristic(Point point1, Point point2) {
			return 2 * (int)HeuristicFunction(point1, point2);
		}
		IIntersectionInfo<TItem> GetIntersectionInfo(IEnumerable<TItem> items) {
			return new GridIntersectionInfo(GetBoundsFunc) { Nodes = items };
		}
		[DebuggerDisplay("P={Position} (F={F}; G={G}; H={H})")]
		class AStarNode : IComparable<AStarNode> {
			public readonly Point Position;
			public double F { get { return G + H; } }
			public double G { get; set; }
			public double H { get; set; }
			public AStarNode Parent { get; set; }
			public AStarNode(Point position) {
				Position = position;
			}
			public IEnumerable<Point> GetPath(Point begin, Point end) {
				var result = new List<Point>();
				var currentNode = this;
				while(currentNode != null) {
					result.Add(currentNode.Position);
					currentNode = currentNode.Parent;
				}
				result.Reverse();
				return RemoveIntermediatePoints(result, begin, end);
			}
			public override int GetHashCode() {
				return Position.GetHashCode();
			}
			public override bool Equals(object obj) {
				var other = obj as AStarNode;
				return other != null && Position.Equals(other.Position);
			}
			static IEnumerable<Point> RemoveIntermediatePoints(IEnumerable<Point> points, Point begin, Point end) {
				List<Point> path = new List<Point>();
				int idx = 0;
				var fullPath = begin.Yield().Concat(points).Concat(end.Yield());
				int count = points.Count();
				while(count > idx) {
					Point point1 = points.ElementAt(idx);
					if(path.Count > 1 && !MathHelper.IsCorner(point1, path[path.Count - 1], path[path.Count - 2]))
						path.RemoveAt(path.Count - 1);
					path.Add(point1);
					idx++;
				}
				path.RemoveAt(0);
				return path.Take(path.Count - 1).ToArray();
			}
			public int CompareTo(AStarNode other) {
				if(F == other.F)
					return 0;
				return F > other.F ? 1 : -1;
			}
		}
		class DiagramGrid {
			public Rect Bounds { get; private set; }
			readonly double ItemMargin;
			readonly Func<TItem, Rect> GetBounds;
			List<double> HorizontalGridLines { get; set; }
			List<double> VerticalGridLines { get; set; }
			public DiagramGrid(Point start, Point end, IEnumerable<TItem> nodes, Func<TItem, Rect> getBounds, double itemMargin = 20d) {
				GetBounds = getBounds;
				ItemMargin = itemMargin;
				Initialize(start, end, nodes);
			}
			public IEnumerable<Point> GetNeighbors(Point point) {
				var searchPoint = point.RoundPoint();
				int verticalIndex = VerticalGridLines.BinarySearch(searchPoint.X);
				int idx = verticalIndex - 1;
				if(VerticalGridLines.IsValidIndex(idx))
					yield return new Point(VerticalGridLines[idx], searchPoint.Y);
				idx = verticalIndex + 1;
				if(VerticalGridLines.IsValidIndex(idx))
					yield return new Point(VerticalGridLines[idx], searchPoint.Y);
				int horizontalIndex = HorizontalGridLines.BinarySearch(searchPoint.Y);
				idx = horizontalIndex - 1;
				if(HorizontalGridLines.IsValidIndex(idx))
					yield return new Point(searchPoint.X, HorizontalGridLines[idx]);
				idx = horizontalIndex + 1;
				if(HorizontalGridLines.IsValidIndex(idx))
					yield return new Point(searchPoint.X, HorizontalGridLines[idx]);
			}
			void Initialize(Point start, Point end, IEnumerable<TItem> nodes) {
				var horizontalLines = new List<double>();
				var verticalLines = new List<double>();
				foreach(var node in nodes) {
					var originBounds = GetBounds(node);
					var bounds = originBounds.InflateRect(ItemMargin);
					AddIfPossible(horizontalLines, originBounds.Top);
					AddIfPossible(horizontalLines, originBounds.Bottom);
					AddIfPossible(horizontalLines, bounds.Top);
					AddIfPossible(horizontalLines, bounds.Bottom);
					AddIfPossible(horizontalLines, bounds.Top + bounds.Height / 2);
					AddIfPossible(verticalLines, bounds.Left);
					AddIfPossible(verticalLines, bounds.Right);
					AddIfPossible(verticalLines, originBounds.Left);
					AddIfPossible(verticalLines, originBounds.Right);
					AddIfPossible(verticalLines, bounds.Left + bounds.Width / 2);
				}
				AddIfPossible(verticalLines, start.X);
				AddIfPossible(verticalLines, end.X);
				AddIfPossible(horizontalLines, start.Y);
				AddIfPossible(horizontalLines, end.Y);
				horizontalLines.TrimExcess();
				verticalLines.TrimExcess();
				horizontalLines.Sort();
				verticalLines.Sort();
				HorizontalGridLines = horizontalLines;
				VerticalGridLines = verticalLines;
				Bounds = new Rect(new Point(VerticalGridLines.Min(), HorizontalGridLines.Min()), new Point(VerticalGridLines.Max(), HorizontalGridLines.Max()));
			}
			static void AddIfPossible(IList<double> list, double value) {
				double roundedValue = Math.Round(value);
				if(!list.Any(num => roundedValue == num))
					list.Add(roundedValue);
			}
		}
		class GridIntersectionInfo : IIntersectionInfo<TItem> {
			public int ItemMargin { get; set; }
			const int CellSize = 200;
			Dictionary<Point, HashSet<TItem>> cells;
			public IEnumerable<TItem> Nodes {
				get { return nodes; }
				set {
					if(nodes == value)
						return;
					nodes = value;
					OnNodesChanged();
				}
			}
			IEnumerable<TItem> nodes;
			readonly Func<TItem, Rect> GetBoundsFunc;
			public GridIntersectionInfo(Func<TItem, Rect> getBounds) {
				GetBoundsFunc = getBounds;
				Nodes = Enumerable.Empty<TItem>();
				Initialize();
			}
			public IEnumerable<TItem> GetIntersections(Point point, Predicate<TItem> ignoreItem) {
				Point cellPosition = GetCellPosition(point);
				HashSet<TItem> cell = cells.GetValueOrDefault(cellPosition);
				if(cell == null)
					return Enumerable.Empty<TItem>();
				return cell.Where(item => !ignoreItem(item))
					.Where(item => GetBounds(item).InflateRect(ItemMargin).Contains(point));
			}
			Rect IIntersectionInfo<TItem>.GetBounds(TItem item) {
				return GetBounds(item);
			}
			Rect GetBounds(TItem item) {
				return GetBoundsFunc(item);
			}
			void Initialize() {
				cells = new Dictionary<Point, HashSet<TItem>>();
				foreach(var node in Nodes)
					AddNode(node);
			}
			void AddNode(TItem node) {
				IList<HashSet<TItem>> cells = GetItemCells(node);
				foreach(HashSet<TItem> cell in cells)
					cell.Add(node);
			}
			IList<HashSet<TItem>> GetItemCells(TItem node) {
				IList<HashSet<TItem>> list = new List<HashSet<TItem>>();
				Rect boundingBox = GetBounds(node);
				Point topLeft = GetCellPosition(boundingBox.TopLeft.OffsetPoint(new Point(-1, -1)));
				Point bottomRight = GetCellPosition(boundingBox.BottomRight.OffsetPoint(new Point(1, 1)));
				double x = topLeft.X;
				double y = topLeft.Y;
				do {
					do {
						var cell = cells.GetOrAdd(new Point(x, y), () => new HashSet<TItem>());
						list.Add(cell);
						y += CellSize;
					}
					while(y <= bottomRight.Y);
					x += CellSize;
					y = topLeft.Y;
				}
				while(x <= bottomRight.X);
				return list;
			}
			Point GetCellPosition(Point point) {
				return new Point((int)point.X / CellSize * CellSize, (int)point.Y / CellSize * CellSize);
			}
			void OnNodesChanged() {
				Initialize();
			}
		}
	}
	interface IIntersectionInfo<TItem> {
		int ItemMargin { get; set; }
		Rect GetBounds(TItem item);
		IEnumerable<TItem> GetIntersections(Point point, Predicate<TItem> ignore);
	}
}
