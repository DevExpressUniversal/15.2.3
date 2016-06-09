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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Shapes;
using DevExpress.Internal;
using ResizeMode = DevExpress.Diagram.Core.ResizeMode;
namespace DevExpress.Diagram.Core {
	public class AxisLine {
		public readonly Point From;
		public Point To {
			get {
				var offset = Orientation.SetPoint(default(Point), length);
				return From.OffsetPoint(offset);
			}
		}
		public readonly double length;
		public readonly Orientation Orientation;
		public AxisLine(Point from, double length, Orientation orientation) {
			From = from;
			this.length = length;
			this.Orientation = orientation;
		}
		public bool IsHorizontalOrientation { get { return Orientation == Orientation.Horizontal; } }
	}
	public sealed class BoundsSnapLine : AxisLine {
		public readonly Side Side;
		public BoundsSnapLine(Point from, double length, Orientation orientation, Side side)
			: base(from, length, orientation) {
			this.Side = side;
		}
	}
	public sealed class SizeSnapLine : AxisLine {
		public SizeSnapLine(Point from, double length, Orientation orientation) : base(from, length, orientation) {
		}
	}
	public interface ISnapLinesProvider {
		SnapPointResult SnapPointOnResize(Point point, Point snappedPoint, Rect_Angle bounds, Size minSize, ResizeMode mode);
		SnapOffsetResult SnapRectOnMove(Rect rect, Point snappedOffset);
	}
	public sealed class EmptySnaplLinesProvider : ISnapLinesProvider {
		public static readonly ISnapLinesProvider Instance = new EmptySnaplLinesProvider();
		EmptySnaplLinesProvider() { }
		SnapPointResult ISnapLinesProvider.SnapPointOnResize(Point point, Point snappedPoint, Rect_Angle bounds, Size minSize, ResizeMode mode) {
			return new SnapPointResult(new AxisLine[0], snappedPoint);
		}
		SnapOffsetResult ISnapLinesProvider.SnapRectOnMove(Rect rect, Point snappedOffset) {
			return new SnapOffsetResult(new AxisLine[0], snappedOffset);
		}
	}
	public sealed class SnapLinesProvider : ISnapLinesProvider {
		readonly IEnumerable<Rect> snapRects;
		readonly double snapToItemsDistance;
		public SnapLinesProvider(IEnumerable<Rect> snapRects, double snapToItemsDistance) {
			this.snapRects = snapRects;
			this.snapToItemsDistance = snapToItemsDistance;
		}
		SnapOffsetResult ISnapLinesProvider.SnapRectOnMove(Rect rect, Point snappedOffset) {
			var snapResult = Snap(
				preSnappedPoint: rect.Location.OffsetPoint(snappedOffset),
				getBoundsByPoint: x => rect.SetLocation(x),
				unSnappedBounds: rect,
				snappersProvider: GetMoveSnappers,
				buildLines: BuildBoundsSnapLines
			);
			return new SnapOffsetResult(snapResult.SnapLines, MathHelper.GetOffset(rect.Location, snapResult.Point));
		}
		SnapPointResult ISnapLinesProvider.SnapPointOnResize(Point point, Point snappedPoint, Rect_Angle bounds, Size minSize, ResizeMode mode) {
			var rotatedResizeMode = mode.Rotate(bounds.Angle);
			Point startPoint = ResizeHelper.SnapPointToRect(point, bounds.RotatedRect, rotatedResizeMode);
			Func<Point, Rect> getResizedBounds = x => ResizeHelper.ResizeRects(new[] { new RectInfo(bounds, minSize) }, mode, MathHelper.GetOffset(startPoint, x), bounds.Angle).Single().Rotate(bounds.Angle);
			return Snap(
				preSnappedPoint: snappedPoint,
				getBoundsByPoint: getResizedBounds,
				unSnappedBounds: getResizedBounds(point),
				snappersProvider: orientation => GetResizeSnappers(orientation, rotatedResizeMode),
				buildLines: BuildLines
			);
		}
		SnapPointResult Snap(Point preSnappedPoint, Func<Point, Rect> getBoundsByPoint, Rect unSnappedBounds, Func<Orientation, Func<Rect, Rect, SnapInfo>[]> snappersProvider, Func<Orientation, IEnumerable<Rect>, Rect, IEnumerable<AxisLine>> buildLines) {
			double? snappedPositionX, snappedPositionY;
			IEnumerable<Rect> snapItemHBoundsList = GetSnapItemBounds(snappersProvider(Orientation.Horizontal), unSnappedBounds, out snappedPositionX);
			IEnumerable<Rect> snapItemVBoundsList = GetSnapItemBounds(snappersProvider(Orientation.Vertical), unSnappedBounds, out snappedPositionY);
			if(snappedPositionX == null && snappedPositionY == null)
				return new SnapPointResult(new AxisLine[0], preSnappedPoint);
			var snappedPoint = new Point(snappedPositionX ?? preSnappedPoint.X, snappedPositionY ?? preSnappedPoint.Y);
			var snappedBounds = getBoundsByPoint(snappedPoint);
			var lines = buildLines(Orientation.Horizontal, snapItemHBoundsList, snappedBounds)
				.Concat(buildLines(Orientation.Vertical, snapItemVBoundsList, snappedBounds));
			return new SnapPointResult(lines.ToArray(), snappedPoint);
		}
		static IEnumerable<AxisLine> BuildLines(Orientation orientation, IEnumerable<Rect> snapItemBoundsList, Rect snappedBounds) {
			return BuildBoundsSnapLines(orientation, snapItemBoundsList, snappedBounds)
				.Concat(BuildSizeSnapLines(orientation, snapItemBoundsList, snappedBounds));
		}
		static IEnumerable<AxisLine> BuildSizeSnapLines(Orientation orientation, IEnumerable<Rect> snapItemBoundsList, Rect snappedBounds) {
			var sizeSnapLines = snapItemBoundsList.Select(snapItemBounds => {
				var size = orientation.GetSize(snapItemBounds);
				if(!MathHelper.AreEqual(size, orientation.GetSize(snappedBounds)))
					return null;
				return CreateSizeSnapLine(orientation, snapItemBounds);
			}).Where(x => x != null);
			if(sizeSnapLines.Any())
				sizeSnapLines = sizeSnapLines.Concat(CreateSizeSnapLine(orientation, snappedBounds).Yield());
			return sizeSnapLines;
		}
		static SizeSnapLine CreateSizeSnapLine(Orientation orientation, Rect snapItemBounds) {
			var location = snapItemBounds.Location.OffsetPoint(orientation.Rotate().SetPoint(default(Point), orientation.Rotate().GetSize(snapItemBounds)));
			return new SizeSnapLine(location, orientation.GetSize(snapItemBounds), orientation);
		}
		static IEnumerable<AxisLine> BuildBoundsSnapLines(Orientation orientation, IEnumerable<Rect> snapItemBoundsList, Rect snappedBounds) {
			return Enum.GetValues(typeof(Side)).Cast<Side>()
				.Select(side => {
					var boundary = orientation.GetSide(snappedBounds, side);
					var filteredBoundsList = snapItemBoundsList.Where(x => MathHelper.AreEqual(orientation.GetSide(x, side), boundary));
					if(!filteredBoundsList.Any())
						return null;
					var containingRect = MathHelper.GetContainingRect(filteredBoundsList.Concat(snappedBounds.Yield()));
					return new BoundsSnapLine(orientation.SetPoint(containingRect.Location, boundary), orientation.Rotate().GetSize(containingRect.Size), orientation.Rotate(), side);
				}).Where(x => x != null);
		}
		struct SnapInfo {
			public readonly double Distance;
			public readonly double? SnapPos;
			public SnapInfo(double distance, double? snapPos) {
				Distance = distance;
				SnapPos = snapPos;
			}
		}
		IEnumerable<Rect> GetSnapItemBounds(Func<Rect, Rect, SnapInfo>[] snappers, Rect unSnappedBounds, out double? snapPos) {
			if(!snappers.Any()) {
				snapPos = null;
				return Enumerable.Empty<Rect>();
			}
			var bestSnapInfo = new SnapInfo(snapToItemsDistance, null);
			var result = snapRects.Aggregate(new List<Rect>(), (list, item) => {
				var currentSnapInfo = snappers.Select(snapper => snapper(item, unSnappedBounds)).MinBy(x => x.Distance);
				if(MathHelper.AreEqual(currentSnapInfo.Distance, bestSnapInfo.Distance)) {
					list.Add(item);
					return list;
				}
				if(currentSnapInfo.Distance > bestSnapInfo.Distance)
					return list;
				bestSnapInfo = currentSnapInfo;
				return new List<Rect>() { item };
			});
			snapPos = bestSnapInfo.SnapPos;
			return result;
		}
		static Func<Rect, Rect, SnapInfo>[] GetResizeSnappers(Orientation orientation, ResizeMode mode) {
			if(!orientation.IsCompatibleWithResizeMode(mode)) {
				return new Func<Rect, Rect, SnapInfo>[0];
			}
			Func<Rect, double> getBoundary = rect => orientation.GetResizeBoundary(rect, mode);
			var snappers = new Func<Rect, Rect, SnapInfo>[] {
				(item, bounds) => new SnapInfo(Math.Abs(getBoundary(item) - getBoundary(bounds)), getBoundary(item)),
				(item, bounds) => {
					var delta = orientation.GetSize(item) - orientation.GetSize(bounds);
					if((orientation == Orientation.Horizontal && mode.IsLeft()) || (orientation == Orientation.Vertical && mode.IsTop()))
						delta = -delta;
					return new SnapInfo(Math.Abs(orientation.GetSize(item) - orientation.GetSize(bounds)), getBoundary(bounds) + delta);
				},
			};
			return snappers;
		}
		static double GetDistance(Rect r1, Rect r2, Func<Rect, double> getBoundary) {
			return Math.Abs(getBoundary(r1) - getBoundary(r2));
		}
		static Func<Rect, Rect, SnapInfo>[] GetMoveSnappers(Orientation orientation) {
			Func<Rect, double> getNearBoundary = rect => orientation.GetSide(rect, Side.Near);
			Func<Rect, double> getFarBoundary = rect => orientation.GetSide(rect, Side.Far);
			Func<Rect, double> getCenterBoundary = rect => orientation.GetSide(rect, Side.Center);
			var snappers = new Func<Rect, Rect, SnapInfo>[] {
				(item, bounds) => new SnapInfo(GetDistance(item, bounds, getNearBoundary), getNearBoundary(item)),
				(item, bounds) => new SnapInfo(GetDistance(item, bounds, getFarBoundary), getFarBoundary(item) - orientation.GetSize(bounds)),
				(item, bounds) => new SnapInfo(GetDistance(item, bounds, getCenterBoundary), getCenterBoundary(item) - orientation.GetSize(bounds) / 2),
			};
			return snappers;
		}
	}
	public abstract class SnapResultBase<T> {
		public readonly AxisLine[] SnapLines;
		protected readonly T value;
		public SnapResultBase(AxisLine[] snapLines, T value) {
			this.SnapLines = snapLines;
			this.value = value;
		}
		public SnapResult<TNew> WithResult<TNew>(TNew newResult) {
			return new SnapResult<TNew>(SnapLines, newResult);
		}
	}
	public class SnapResult<T> : SnapResultBase<T> {
		public T Result { get { return value; } }
		public SnapResult(AxisLine[] snapLines, T result)
			: base(snapLines, result) {
		}
	}
	public class SnapPointResult : SnapResultBase<Point> {
		public Point Point { get { return value; } }
		public SnapPointResult(AxisLine[] snapLines, Point point) 
			: base(snapLines, point) {
		}
	}
	public class SnapOffsetResult : SnapResultBase<Point> {
		public Point Offset { get { return value; } }
		public SnapOffsetResult(AxisLine[] snapLines, Point offset)
			: base(snapLines, offset) {
		}
	}
	public class SnapInfo {
		public static readonly SnapInfo Empty = new SnapInfo(EmptySnaplLinesProvider.Instance);
		readonly Size? gridSize;
		readonly Point snapOffset;
		readonly ISnapLinesProvider snapLinesProvider;
		public SnapInfo(Size? gridSize, Point snapOffset, ISnapLinesProvider snapLinesProvider) {
			this.gridSize = gridSize;
			this.snapOffset = snapOffset;
			this.snapLinesProvider = snapLinesProvider;
		}
		public SnapInfo(ISnapLinesProvider snapLinesProvider)
			: this(null, default(Point), snapLinesProvider) {
		}
		public Point SnapPoint(Point point) {
			if(gridSize != null)
				point = SnapPoint(point.OffsetPoint(snapOffset.InvertPoint()), gridSize.Value).OffsetPoint(snapOffset);
			return point;
		}
		public SnapOffsetResult SnapRectOnMove(Rect rect) {
			var snappedPoint = SnapPoint(rect.Location);
			var offset = MathHelper.GetOffset(rect.Location, snappedPoint);
			return snapLinesProvider.SnapRectOnMove(rect, offset);
		}
		public SnapPointResult SnapPointOnResize(Point point, Rect_Angle bounds, Size minSize, ResizeMode mode) {
			var snappedPoint = SnapPoint(point);
			return snapLinesProvider.SnapPointOnResize(point, snappedPoint, bounds, minSize, mode);
		}
		public Point GetOffset(Direction direction) {
			return direction.GetOffset(gridSize ?? new Size(1, 1));
		}
		static Point SnapPoint(Point point, Size gridSize) {
			return new Point(SnapValue(point.X, gridSize.Width), SnapValue(point.Y, gridSize.Height));
		}
		static double SnapValue(double value, double gridSize) {
			return Math.Round(value / gridSize) * gridSize;
		}
	}
	public static class SnapInfoExtensions {
		const double RenderCorrection = 0.5;
		public static AxisLine CorrectForRender(this AxisLine line) {
			Point correctedFrom;
			if(line.Orientation == Orientation.Horizontal) {
				correctedFrom = line.From.OffsetY(RenderCorrection);
			} else
				correctedFrom = line.From.OffsetX(RenderCorrection);
			return new AxisLine(correctedFrom, line.length, line.Orientation);
		}
		public static SnapOffsetResult GetRectSnapOffset(this SnapInfo snapInfo, Point offset, Rect originalBounds) {
			var unsnappedNewBounds = originalBounds.OffsetRect(offset);
			return snapInfo.SnapRectOnMove(unsnappedNewBounds);
		}
		public static SnapResult<Rect> SnapRectLocation(this SnapInfo snapInfo, Rect rect, Point relativeTo = default(Point)) {
			var actualRect = rect.OffsetRect(relativeTo);
			var snapResult = snapInfo.SnapRectOnMove(actualRect);
			rect = rect.OffsetRect(snapResult.Offset);
			return snapResult.WithResult(rect);
		}
		public static Rect SnapRect(this SnapInfo snapInfo, Rect rect) {
			return new Rect(snapInfo.SnapPoint(rect.TopLeft), snapInfo.SnapPoint(rect.BottomRight));
		}
		public static SnapOffsetResult SnapRectResizing<T>(this SnapInfo snapInfo, IEnumerable<SizeInfo<T>> items, ResizeMode mode, Point startPosition, Point endPosition, double rotationAngle) {
			var containingRect = items.Select(x => x.Rect).GetContainingRect();
			var snappedStartPosition = ResizeHelper.SnapPointToRect(startPosition, containingRect, mode.Rotate(rotationAngle));
			var snappedEndPosition = endPosition.OffsetPoint(MathHelper.GetOffset(startPosition, snappedStartPosition));
			var snapResult = snapInfo.SnapPointOnResize(snappedEndPosition, items.First().Rect, items.First().MinSize, mode);
			return new SnapOffsetResult(snapResult.SnapLines, MathHelper.GetOffset(snappedStartPosition, snapResult.Point));
		}
		public static SnapInfo GetSnapInfo(this IDiagramControl diagram, IEnumerable<IDiagramItem> itemsToSnap = null, IDiagramItem snapScopeItem = null, bool isSnappingEnabled = true, bool allowSnapToItems = true) {
			itemsToSnap = itemsToSnap ?? Enumerable.Empty<IDiagramItem>();
			if(!isSnappingEnabled)
				return SnapInfo.Empty;
			var snapLinesProvider = (diagram.SnapToItems && allowSnapToItems && itemsToSnap.IsSingle() && itemsToSnap.Single().CanSnapToOtherItems) ?
				new SnapLinesProvider(GetSnapRects(diagram, itemsToSnap.Single()), diagram.SnapToItemsDistance) :
				EmptySnaplLinesProvider.Instance;
			if(diagram.SnapToGrid)
				return new SnapInfo(GetActualGridSize(diagram), GetSnapScope(diagram, snapScopeItem).ActualDiagramBounds().Location, snapLinesProvider);
			else
				return new SnapInfo(snapLinesProvider);
		}
		public static SnapInfo GetSnapInfo(this IDiagramControl diagram, IEnumerable<IDiagramItem> itemsToSnap, Point point, bool isSnappingEnabled = true, bool allowSnapToItems = true) {
			return diagram.GetSnapInfo(itemsToSnap, diagram.RootItem().FindContainerItemAtPoint(point), isSnappingEnabled, allowSnapToItems);
		}
		static IEnumerable<Rect> GetSnapRects(IDiagramControl diagram, IDiagramItem itemToSnap) {
			return diagram.RootItem().GetChildren()
							.Where(item => item != itemToSnap && item.CanSnapToThisItem)
							.Select(item => item.RotatedDiagramBounds().RotatedRect);
		}
		static IDiagramItem GetSnapScope(IDiagramControl diagram, IDiagramItem snapItem) {
			IDiagramItem snapScope = diagram.RootItem();
			if(snapItem != null)
				snapScope = snapItem.GetParentsIncludingSelf().FirstOrDefault(parent => (parent as IDiagramContainer).Return(x => x.IsSnapScope, () => false)) ?? snapScope;
			return snapScope;
		}
		static Size GetActualGridSize(IDiagramControl diagram) {
			return diagram.GridSize ?? RulerRenderHelper.GetGridSize(diagram.MeasureUnit, diagram.ZoomFactor);
		}
	}
}
