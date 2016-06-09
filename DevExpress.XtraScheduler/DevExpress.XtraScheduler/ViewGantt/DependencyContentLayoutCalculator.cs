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

using System.Drawing;
using System.Drawing.Imaging;
using DevExpress.Utils;
using System.Drawing.Drawing2D;
using System;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Native;
using System.Threading;
namespace DevExpress.XtraScheduler.Drawing {
	#region DependencyContentLayoutCalculator
	public class DependencyContentLayoutCalculator {
		#region Fields
		GanttDependenciesPainter painter;
		Image leftArrow;
		Image rightArrow;
		Image topArrow;
		Image bottomArrow;
		Image topRightCorner;
		Image topLeftCorner;
		Image bottomRightCorner;
		Image bottomLeftCorner;
		Dictionary<Point, int> processedPoints;
		#endregion
		public DependencyContentLayoutCalculator(GanttDependenciesPainter painter) {
			Guard.ArgumentNotNull(painter, "painter");
			this.painter = painter;
			Initialize();
		}
		#region Properties
		protected internal GanttDependenciesPainter Painter { get { return painter; } }
		protected internal Dictionary<Point, int> ProcessedPoints { get { return processedPoints; } }
		#endregion
		protected internal virtual void Initialize() {
			this.processedPoints = new Dictionary<Point, int>();
			InitializeArrowImages();
			InitializeCornerImages();
		}
		protected internal virtual void InitializeArrowImages() {
			this.rightArrow = Painter.GetArrowImage();
			this.leftArrow = CreateImage(rightArrow, RotateFlipType.Rotate180FlipNone);
			this.topArrow = CreateImage(rightArrow, RotateFlipType.Rotate270FlipNone);
			this.bottomArrow = CreateImage(rightArrow, RotateFlipType.Rotate90FlipNone);
		}
		protected internal virtual void InitializeCornerImages() {
			this.bottomLeftCorner = Painter.GetCornerImage();
			if (bottomLeftCorner == null)
				return;
			this.bottomRightCorner = CreateImage(bottomLeftCorner, RotateFlipType.RotateNoneFlipX);
			this.topLeftCorner = CreateImage(bottomLeftCorner, RotateFlipType.RotateNoneFlipY);
			this.topRightCorner = CreateImage(bottomLeftCorner, RotateFlipType.Rotate180FlipNone);
		}
		protected internal virtual Image CreateImage(Image originalImage, RotateFlipType flipType) {
			Bitmap newImage = new Bitmap(originalImage.Width, originalImage.Height);
			using (Graphics gr = Graphics.FromImage(newImage)) {
				gr.DrawImage(originalImage, new Rectangle(Point.Empty, newImage.Size));
				newImage.RotateFlip(flipType);
			}
			return newImage;
		}
		public virtual void CalculateContentLayout(CancellationToken token, GanttViewAppearance viewAppearance, GanttViewAppearance paintAppearance, DependencyViewInfoCollection viewInfos) {
			CalculateArrowsAndCorrectPoints(token, viewInfos);
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested)
					return;
				CalculateContentLayoutCore(viewAppearance, paintAppearance, viewInfos[i]);
			}
		}
		protected internal virtual void CalculateArrowsAndCorrectPoints(CancellationToken token, DependencyViewInfoCollection viewInfos) {
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested)
					return;
				DependencyViewInfo currentViewinfo = viewInfos[i];
				SynchronizePoints(currentViewinfo);
				CalculateArrow(currentViewinfo);
			}
		}
		protected internal virtual void CalculateContentLayoutCore(GanttViewAppearance viewAppearance, GanttViewAppearance paintAppearance, DependencyViewInfo viewInfo) {
			CalculateAppearancies(viewAppearance, paintAppearance, viewInfo);
			CalculateCorner(viewInfo);
		}
		protected internal virtual void CalculateAppearancies(GanttViewAppearance viewAppearance, GanttViewAppearance paintAppearance, DependencyViewInfo viewInfo) {
			viewInfo.Appearance.Assign(CalculateAppearance(viewAppearance, paintAppearance, Painter));
			bool useSelectedForeColor = viewAppearance.SelectedDependency.Options.UseForeColor;
			viewInfo.SelectedAppearance.Assign(CalculateAppearanciesCore(paintAppearance.SelectedDependency, useSelectedForeColor, Painter.GetSelectedDependencyColor()));
		}
		protected internal static AppearanceObject CalculateAppearance(GanttViewAppearance viewAppearance, GanttViewAppearance paintAppearance, GanttDependenciesPainter painter) {
			bool useForeColor = viewAppearance.Dependency.Options.UseForeColor;
			return CalculateAppearanciesCore(paintAppearance.Dependency, useForeColor, painter.GetDependencyColor());
		}
		protected internal static AppearanceObject CalculateAppearanciesCore(AppearanceObject owner, bool useOwnerColor, Color color) {
			AppearanceObject result = new AppearanceObject();
			result.Combine(owner);
			if (!useOwnerColor && color != Color.Empty)
				result.ForeColor = color;
			return result;
		}
		protected internal virtual void SynchronizePoints(DependencyViewInfo viewInfo) {
			viewInfo.LineStart = viewInfo.Start;
			viewInfo.LineEnd = viewInfo.End;
		}
		protected internal virtual void CalculateCorner(DependencyViewInfo viewInfo) {
			CalculateCornerCore(viewInfo, true);
			CalculateCornerCore(viewInfo, false);
		}
		protected internal virtual void CalculateCornerCore(DependencyViewInfo dependencyViewInfo, bool isStartCorner) {
			if (dependencyViewInfo.Direction == DependencyDirection.Vertical)
				return;
			DependencyCornerViewInfo cornerViewInfo = isStartCorner ? dependencyViewInfo.StartCorner : dependencyViewInfo.EndCorner;
			DependencyCornerType cornerType = cornerViewInfo.Type;
			if (cornerType == DependencyCornerType.None) {
				CorrectAdjacentViewInfos(cornerViewInfo);
				return;
			}
			Image cornerImage = GetCornerImage(cornerType);
			if (cornerImage == null)
				return;
			Size cornerSize = cornerImage.Size;
			DependencyViewInfo vertical = cornerViewInfo.AdjacentDependencyViewInfos[0];
			if (ForbidInsertCorner(cornerSize, dependencyViewInfo.GetLineLenght(), vertical.GetLineLenght())) {
				CorrectAdjacentViewInfos(cornerViewInfo);
				return;
			}
			ViewInfoImageItem imageItem = new DependencyViewInfoCornerItem(cornerImage, cornerType);
			int cornerWidth = cornerSize.Width;
			int cornerHeight = cornerSize.Height;
			Point point = isStartCorner ? dependencyViewInfo.Start : dependencyViewInfo.End;
			imageItem.Bounds = CalculateCornerBounds(cornerType, point, cornerWidth, cornerHeight);
			dependencyViewInfo.Items.Add(imageItem);
			CorrectCornerPoint(dependencyViewInfo, cornerType, cornerWidth, isStartCorner);
			CorrectCornerPointVerticalViewInfo(cornerViewInfo, vertical, cornerHeight);
		}
		protected internal virtual void CorrectAdjacentViewInfos(DependencyCornerViewInfo cornerViewInfo) {
			Point connectionPoint = cornerViewInfo.ConnectionPoint;
			int value;
			if (ProcessedPoints.TryGetValue(connectionPoint, out value))
				return;
			int adjacentDependencyCount = cornerViewInfo.AdjacentDependencyViewInfos.Count;
			for (int i = 0; i < adjacentDependencyCount; i++) {
				DependencyViewInfo currentViewInfo = cornerViewInfo.AdjacentDependencyViewInfos[i];
				CorrectAdjacentViewInfosCore(currentViewInfo, connectionPoint);
			}
			ProcessedPoints.Add(connectionPoint, 0);
		}
		protected internal virtual void CorrectAdjacentViewInfosCore(DependencyViewInfo viewInfo, Point connectionPoint) {
			Point lineStart = viewInfo.LineStart;
			Point lineEnd = viewInfo.LineEnd;
			if (lineStart == lineEnd) {
				viewInfo.LineVisible = false;
				return;
			}
			bool isVerticalViewInfo = viewInfo.Direction == DependencyDirection.Vertical;
			if (lineStart == connectionPoint)
				viewInfo.LineStart = GetNewPointForAdjacentDependency(lineStart, lineStart, lineEnd, isVerticalViewInfo);
			else
				viewInfo.LineEnd = GetNewPointForAdjacentDependency(lineEnd, lineEnd, lineStart, isVerticalViewInfo);
		}
		protected internal virtual Point GetNewPointForAdjacentDependency(Point targetPoint, Point start, Point end, bool isVerticalDependency) {
			if (isVerticalDependency)
				return GetNewVerticalPoint(targetPoint, start, end);
			return GetNewHorizontalPoint(targetPoint, start, end);
		}
		protected internal virtual Point GetNewVerticalPoint(Point targetPoint, Point start, Point end) {
			if (start.Y < end.Y)
				return new Point(targetPoint.X, targetPoint.Y + 1);
			return new Point(targetPoint.X, targetPoint.Y - 1);
		}
		protected internal virtual Point GetNewHorizontalPoint(Point targetPoint, Point start, Point end) {
			if (start.X < end.X)
				return new Point(targetPoint.X + 1, targetPoint.Y);
			return new Point(targetPoint.X - 1, targetPoint.Y);
		}
		protected internal virtual Image GetCornerImage(DependencyCornerType cornerType) {
			switch (cornerType) {
				case DependencyCornerType.TopRight:
					return topRightCorner;
				case DependencyCornerType.TopLeft:
					return topLeftCorner;
				case DependencyCornerType.BottomRight:
					return bottomRightCorner;
				case DependencyCornerType.BottomLeft:
					return bottomLeftCorner;
			}
			return null;
		}
		protected internal virtual bool ForbidInsertCorner(Size cornerSize, int dependencyLenght, int verticalDependencyLenght) {
			return cornerSize.Width >= dependencyLenght || cornerSize.Height >= verticalDependencyLenght;
		}
		protected internal virtual Rectangle CalculateCornerBounds(DependencyCornerType cornerType, Point point, int cornerWidth, int cornerHeight) {
			XtraSchedulerDebug.Assert(cornerType != DependencyCornerType.None);
			switch (cornerType) {
				case DependencyCornerType.BottomLeft:
					return new Rectangle(point.X, point.Y - cornerHeight + 1, cornerWidth, cornerHeight);
				case DependencyCornerType.BottomRight:
					return new Rectangle(point.X - cornerWidth + 1, point.Y - cornerHeight + 1, cornerWidth, cornerHeight);
				case DependencyCornerType.TopLeft:
					return new Rectangle(point.X, point.Y, cornerWidth, cornerHeight);
				case DependencyCornerType.TopRight:
					return new Rectangle(point.X - cornerWidth + 1, point.Y, cornerWidth, cornerHeight);
			}
			return Rectangle.Empty;
		}
		protected internal virtual void CorrectCornerPoint(DependencyViewInfo dependencyViewInfo, DependencyCornerType cornerType, int cornerWidth, bool isStartCorner) {
			if (isStartCorner)
				dependencyViewInfo.LineStart = CalculateCornerPoint(cornerType, dependencyViewInfo.LineStart, cornerWidth);
			else
				dependencyViewInfo.LineEnd = CalculateCornerPoint(cornerType, dependencyViewInfo.LineEnd, cornerWidth);
		}
		protected internal virtual Point CalculateCornerPoint(DependencyCornerType cornerType, Point point, int cornerWidth) {
			XtraSchedulerDebug.Assert(cornerType != DependencyCornerType.None);
			if (cornerType == DependencyCornerType.BottomRight || cornerType == DependencyCornerType.TopRight)
				return new Point(point.X - cornerWidth, point.Y);
			return new Point(point.X + cornerWidth, point.Y);
		}
		protected internal virtual void CorrectCornerPointVerticalViewInfo(DependencyCornerViewInfo cornerViewInfo, DependencyViewInfo verticalViewInfo, int cornerHeight) {
			DependencyCornerType cornerType = cornerViewInfo.Type;
			if (cornerViewInfo.ConnectionPoint == verticalViewInfo.End)
				verticalViewInfo.LineEnd = CalculateCornerPointVerticalViewInfo(cornerType, verticalViewInfo.LineEnd, cornerHeight);
			else
				verticalViewInfo.LineStart = CalculateCornerPointVerticalViewInfo(cornerType, verticalViewInfo.LineStart, cornerHeight);
		}
		protected internal virtual Point CalculateCornerPointVerticalViewInfo(DependencyCornerType cornerType, Point point, int cornerHeight) {
			XtraSchedulerDebug.Assert(cornerType != DependencyCornerType.None);
			if (cornerType == DependencyCornerType.BottomRight || cornerType == DependencyCornerType.BottomLeft)
				return new Point(point.X, point.Y - cornerHeight);
			return new Point(point.X, point.Y + cornerHeight);
		}
		protected internal virtual void CalculateArrow(DependencyViewInfo viewInfo) {
			GanttAppointmentViewInfoWrapper apt = viewInfo.EndGanttViewInfo;
			bool hasArrow = (apt != null) && apt.Visibility.Visible;
			if (hasArrow)
				CalculateArrowAndCorrectEndPoint(viewInfo);
		}
		protected internal virtual void CalculateArrowAndCorrectEndPoint(DependencyViewInfo viewInfo) {
			if (viewInfo.Start.Y == viewInfo.End.Y)
				CalculateArrowAndCorrectEndPointHorizontally(viewInfo);
			else
				CalculateArrowAndCorrectEndPointVertically(viewInfo);
		}
		protected internal virtual void CalculateArrowAndCorrectEndPointHorizontally(DependencyViewInfo viewInfo) {
			Point endPoint = viewInfo.End;
			if (viewInfo.Start.X > endPoint.X) {
				viewInfo.Items.Add(CreateLeftArrowImageItem(endPoint));
				viewInfo.LineEnd = CalculateNewLeftPoint(endPoint);
			}
			else {
				viewInfo.Items.Add(CreateRightArrowImageItem(endPoint));
				viewInfo.LineEnd = CalculateNewRightPoint(endPoint);
			}
		}
		protected internal virtual void CalculateArrowAndCorrectEndPointVertically(DependencyViewInfo viewInfo) {
			Point endPoint = viewInfo.End;
			if (viewInfo.Start.Y < viewInfo.End.Y) {
				viewInfo.Items.Add(CreateBottomArrowImageItem(endPoint));
				viewInfo.LineEnd = CalculateNewBottomPoint(endPoint);
			}
			else {
				viewInfo.Items.Add(CreateTopArrowImageItem(endPoint));
				viewInfo.LineEnd = CalculateNewTopPoint(endPoint);
			}
		}
		protected internal virtual ViewInfoImageItem CreateLeftArrowImageItem(Point point) {
			ViewInfoImageItem result = new DependencyViewInfoArrowItem(leftArrow);
			Size imageSize = leftArrow.Size;
			int imageHeight = imageSize.Height;
			result.Bounds = new Rectangle(point.X, point.Y - imageHeight / 2, imageSize.Width, imageHeight);
			return result;
		}
		protected internal virtual ViewInfoImageItem CreateRightArrowImageItem(Point point) {
			ViewInfoImageItem result = new DependencyViewInfoArrowItem(rightArrow);
			Size imageSize = rightArrow.Size;
			int imageWidth = imageSize.Width;
			int imageHeight = imageSize.Height;
			result.Bounds = new Rectangle(point.X - imageWidth, point.Y - imageHeight / 2, imageWidth, imageHeight);
			return result;
		}
		protected internal virtual ViewInfoImageItem CreateTopArrowImageItem(Point point) {
			ViewInfoImageItem result = new DependencyViewInfoArrowItem(topArrow);
			Size imageSize = topArrow.Size;
			int imageWidth = imageSize.Width;
			result.Bounds = new Rectangle(point.X - imageWidth / 2, point.Y, imageWidth, imageSize.Height);
			return result;
		}
		protected internal virtual ViewInfoImageItem CreateBottomArrowImageItem(Point point) {
			ViewInfoImageItem result = new DependencyViewInfoArrowItem(bottomArrow);
			Size imageSize = bottomArrow.Size;
			int imageWidth = imageSize.Width;
			int imageHeight = imageSize.Height;
			result.Bounds = new Rectangle(point.X - imageWidth / 2, point.Y - imageHeight, imageWidth, imageHeight);
			return result;
		}
		protected internal virtual Point CalculateNewLeftPoint(Point target) {
			int newX = target.X + leftArrow.Size.Width;
			return new Point(newX, target.Y);
		}
		protected internal virtual Point CalculateNewRightPoint(Point target) {
			int newX = target.X - rightArrow.Size.Width - 1;
			return new Point(newX, target.Y);
		}
		protected internal virtual Point CalculateNewTopPoint(Point target) {
			int newY = target.Y + topArrow.Size.Height;
			return new Point(target.X, newY);
		}
		protected internal virtual Point CalculateNewBottomPoint(Point target) {
			int newY = target.Y - bottomArrow.Size.Height - 1;
			return new Point(target.X, newY);
		}
	}
	#endregion
}
