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
using System;
namespace DevExpress.Charts.Native {
	public interface IPieLabelLayout : ILabelLayout {
		double Angle { get; }
		bool ResolveOverlapping { get; }
	}
	public interface IFunnelLabelLayout : ILabelLayout {
		bool ResolveOverlapping { get; }
		bool IsLeftColumn { get; }
	}
	public enum PointsSweepDirection {
		Counterclockwise = 0,
		Clockwise = 1,
	}
	public abstract class SimpleDiagrammResiolveOverlapping {
		public static void ArrangeByEllipse(IList<IPieLabelLayout> labels, GRealEllipse ellipse, int resolveOverlappingMinIndent, PointsSweepDirection direction, GRealRect2D diagramBounds) {
			ResolveOverlappingByEllipse algorithm = new ResolveOverlappingByEllipse();
			algorithm.ArrangeLabels(labels, ellipse, resolveOverlappingMinIndent, direction, diagramBounds);
		}
		public static void ArrangeByColumn(IList<IPieLabelLayout> labels, GRealRect2D bounds, int resolveOverlappingMinIndent) {
			ResolveOverlappingByColumn algorithm = new ResolveOverlappingByColumn();
			algorithm.ArrangeLabels(labels, bounds, resolveOverlappingMinIndent);
		}
	}
	public class ResolveOverlappingByColumn : SimpleDiagrammResiolveOverlapping {
		#region Inner Class
		class LabelInfoComparer : IComparer<LabelInfo> {
			public int Compare(LabelInfo x, LabelInfo y) {
				if (x == null)
					return -1;
				if (y == null)
					return 1;
				IPieLabelLayout l1 = (IPieLabelLayout)x.Label;
				IPieLabelLayout l2 = (IPieLabelLayout)y.Label;
				double centerY1 = l1.LabelBounds.Bottom + l1.LabelBounds.Height * 0.5;
				double centerY2 = l2.LabelBounds.Top + l2.LabelBounds.Height * 0.5;
				return l1.Angle == l2.Angle ? centerY1.CompareTo(centerY2) : Math.Sin(l1.Angle).CompareTo(Math.Sin(l2.Angle));
			}
		}
		class LabelInfo {
			LabelInfo prev, next;
			GRealRect2D labelBounds, correctedLabelBounds;
			ILabelLayout label;
			GRealRect2D bounds;
			public GRealRect2D Bounds { get { return bounds; } set { bounds = value; } }
			public ILabelLayout Label { get { return label; } }
			public double Center { get { return labelBounds.Center.Y; } }
			public LabelInfo Prev { get { return prev; } set { prev = value; } }
			public LabelInfo Next { get { return next; } set { next = value; } }
			public GRealRect2D LabelBounds { get { return labelBounds; } }
			public GRealRect2D CorrectedLabelBounds { get { return correctedLabelBounds; } }
			public void Flush(GRealRect2D bounds) {
				Label.LabelBounds = GeometricUtils.StrongRound(bounds);
			}
			public LabelInfo(IPieLabelLayout label, GRealRect2D bounds, int resolveOverlappingMinIndent) {
				this.label = label;
				labelBounds = new GRealRect2D(label.LabelBounds.Left, label.LabelBounds.Top, label.LabelBounds.Width, label.LabelBounds.Height);
				correctedLabelBounds = GRealRect2D.Inflate(labelBounds, resolveOverlappingMinIndent / 2.0, resolveOverlappingMinIndent / 2.0);
				Bounds = bounds;
			}
			public double GetOverlap() {
				double topMargin = Prev == null ? LabelBounds.Top - Bounds.Top : CorrectedLabelBounds.Top - Prev.CorrectedLabelBounds.Bottom;
				double bottomMargin = Next == null ? Bounds.Bottom - LabelBounds.Bottom : Next.CorrectedLabelBounds.Top - CorrectedLabelBounds.Bottom;
				double overlap = 0;
				if (topMargin < 0)
					overlap += Math.Abs(topMargin);
				if (bottomMargin < 0)
					overlap += Math.Abs(bottomMargin);
				return overlap;
			}
			public void Offset(double offset) {
				labelBounds.Offset(0, offset);
				correctedLabelBounds.Offset(0, offset);
			}
		}
		#endregion
		public void ArrangeLabels(IList<IPieLabelLayout> labels, GRealRect2D bounds, int resolveOverlappingMinIndent) {
			if (labels == null || labels.Count == 0)
				return;
			List<IPieLabelLayout> leftGroup = new List<IPieLabelLayout>(), rightGroup = new List<IPieLabelLayout>();
			GroupLablesByColumns(labels, leftGroup, rightGroup);
			ArrangeGroup(leftGroup, bounds, resolveOverlappingMinIndent);
			ArrangeGroup(rightGroup, bounds, resolveOverlappingMinIndent);
		}
		public void ArrangeGroup(List<IPieLabelLayout> labels, GRealRect2D bounds, int resolveOverlappingMinIndent) {
			List<LabelInfo> actualLabels = GetLabelsForArrangByColumn(labels, bounds, resolveOverlappingMinIndent);
			double overlap = CalcWholeOverlap(actualLabels, bounds);
			List<double> focuses = CalcFocuses(bounds);
			if (overlap > 0)
				ArrangeOverlapping(actualLabels, bounds, overlap);
			else
				ArrangeNonOverlapping(actualLabels, bounds, focuses);
			foreach (LabelInfo label in actualLabels)
				label.Flush(label.LabelBounds);
		}
		void GroupLablesByColumns(IList<IPieLabelLayout> labels, List<IPieLabelLayout> left, List<IPieLabelLayout> right) {
			foreach (IPieLabelLayout label in labels) {
				if (Math.Cos(label.Angle) >= 0)
					right.Add(label);
				else
					left.Add(label);
			}
		}
		bool Arrange(LabelInfo labelInf, bool bottomPriority) {
			if (bottomPriority) {
				BottomArrange(labelInf);
				TopArrange(labelInf);
			}
			else {
				TopArrange(labelInf);
				BottomArrange(labelInf);
			}
			return labelInf.GetOverlap() == 0;
		}
		bool IsBottomPriority(LabelInfo label, IList<double> focuses) {
			double nearesFocus = double.NaN;
			foreach (double focus in focuses) {
				double distance = Math.Abs(label.Center - focus);
				if (double.IsNaN(nearesFocus) || distance < Math.Abs(label.Center - nearesFocus))
					nearesFocus = focus;
			}
			return double.IsNaN(nearesFocus) ? false : label.Center < nearesFocus;
		}
		List<LabelInfo> GetLabelsForArrangByColumn(IList<IPieLabelLayout> labels, GRealRect2D bounds, int resolveOverlappingMinIndent) {
			List<LabelInfo> actualLabels = new List<LabelInfo>();
			foreach (IPieLabelLayout label in labels) {
				LabelInfo info = new LabelInfo(label, bounds, resolveOverlappingMinIndent);
				if (label.Visible && label.ResolveOverlapping && info.LabelBounds.Width > 0 && info.LabelBounds.Height > 0)
					actualLabels.Add(info);
			}
			actualLabels.Sort(new LabelInfoComparer());
			return actualLabels;
		}
		List<double> CalcFocuses(GRealRect2D bounds) {
			List<double> focuses = new List<double>();
			focuses.Add(bounds.Top);
			focuses.Add(bounds.Top + bounds.Height);
			return focuses;
		}
		double CalcWholeOverlap(IList<LabelInfo> labels, GRealRect2D bounds) {
			double height = 0;
			for (int i = 0; i < labels.Count; i++) {
				if (i == 0 || i == labels.Count - 1)
					height += labels[i].LabelBounds.Height / 2.0 + labels[i].CorrectedLabelBounds.Height / 2.0;
				else
					height += labels[i].CorrectedLabelBounds.Height;
			}
			return bounds.Height >= height ? 0 : height - bounds.Height;
		}
		void ArrangeOverlapping(IList<LabelInfo> labels, GRealRect2D bounds, double wholeOverlap) {
			if (labels.Count == 0)
				return;
			labels[0].Offset(-CalculateTopMargin(labels[0]));
			if (labels.Count == 1)
				return;
			double overlap = wholeOverlap / (labels.Count - 1);
			for (int i = 1; i < labels.Count; i++) {
				LabelInfo prev = labels[i - 1];
				LabelInfo curr = labels[i];
				double center = prev.CorrectedLabelBounds.Bottom + curr.CorrectedLabelBounds.Height / 2.0 - overlap;
				curr.Offset(center - curr.Center);
				if (CalculateBottomMargin(curr) < 0)
					curr.Offset(CalculateBottomMargin(curr));
				if (CalculateTopMargin(curr) < 0)
					curr.Offset(-CalculateTopMargin(curr));
			}
		}
		void ArrangeNonOverlapping(IList<LabelInfo> labels, GRealRect2D bounds, IList<double> focuses) {
			List<LabelInfo> actualLabels = new List<LabelInfo>(labels);
			LabelInfo prev = null;
			LabelInfo next = null;
			for (int i = 0; i < labels.Count; i++) {
				bool isFirst = i % 2 == 0;
				int index = isFirst ? 0 : actualLabels.Count - 1;
				LabelInfo label = actualLabels[index];
				actualLabels.RemoveAt(index);
				label.Prev = prev;
				label.Next = next;
				if (prev != null)
					prev.Next = label;
				if (next != null)
					next.Prev = label;
				bool bottomPriority = IsBottomPriority(label, focuses);
				if (!Arrange(label, bottomPriority))
					break;
				if (isFirst)
					prev = label;
				else
					next = label;
			}
		}
		void PushToBottom(LabelInfo labelInf, double val) {
			if (CalculateBottomMargin(labelInf) >= val)
				labelInf.Offset(val);
			else {
				PushNextToBottom(labelInf, val - CalculateBottomMargin(labelInf));
				labelInf.Offset(CalculateBottomMargin(labelInf));
			}
		}
		double CalculateTopMargin(LabelInfo labelInf) {
			return labelInf.Prev == null ? labelInf.LabelBounds.Top - labelInf.Bounds.Top : labelInf.CorrectedLabelBounds.Top - labelInf.Prev.CorrectedLabelBounds.Bottom;
		}
		double CalculateBottomMargin(LabelInfo labelInf) {
			return labelInf.Next == null ? labelInf.Bounds.Bottom - labelInf.LabelBounds.Bottom : labelInf.Next.CorrectedLabelBounds.Top - labelInf.CorrectedLabelBounds.Bottom;
		}
		void PushToTop(LabelInfo labelInf, double val) {
			if (CalculateTopMargin(labelInf) >= val)
				labelInf.Offset(-val);
			else {
				PushPrevToTop(labelInf, val - CalculateTopMargin(labelInf));
				labelInf.Offset(-CalculateTopMargin(labelInf));
			}
		}
		void PushNextToBottom(LabelInfo labelInf, double val) {
			if (labelInf.Next != null)
				PushToBottom(labelInf.Next, val);
		}
		void PushPrevToTop(LabelInfo labelInf, double val) {
			if (labelInf.Prev != null)
				PushToTop(labelInf.Prev, val);
		}
		void BottomArrange(LabelInfo labelInf) {
			double overlap = labelInf.GetOverlap();
			if (overlap != 0) {
				if (CalculateBottomMargin(labelInf) < 0)
					labelInf.Offset(CalculateBottomMargin(labelInf));
				PushToBottom(labelInf, overlap);
			}
		}
		void TopArrange(LabelInfo labelInf) {
			double overlap = labelInf.GetOverlap();
			if (overlap != 0) {
				if (CalculateTopMargin(labelInf) < 0)
					labelInf.Offset(CalculateTopMargin(labelInf));
				PushToTop(labelInf, overlap);
			}
		}
	}
	public class ResolveOverlappingByEllipse : SimpleDiagrammResiolveOverlapping {
		#region Inner Classes
		class LabelInfo : IComparable<LabelInfo> {
			GRealPoint2D center;
			GRealEllipse ellipse;
			double refAngle;
			LabelInfo next, prev;
			bool currentSrc, arranged;
			bool? nextLabelNearest;
			double ocupiedHeight;
			double ocupiedWidth;
			ILabelLayout label;
			GRealRect2D bounds;
			public GRealRect2D Bounds { get { return bounds; } set { bounds = value; } }
			public ILabelLayout Label { get { return label; } }
			public GRealEllipse Ellipse { get { return ellipse; } }
			public bool CurrentSrc { get { return currentSrc; } set { currentSrc = value; } }
			public bool Arranged { get { return arranged; } set { arranged = value; } }
			public bool? NextLabelNearest { get { return nextLabelNearest; } set { nextLabelNearest = value; } }
			public LabelInfo Prev { get { return prev; } set { prev = value; } }
			public LabelInfo Next { get { return next; } set { next = value; } }
			public LabelInfo(ILabelLayout label, GRealEllipse ellipse, int resolveOverlappingMinIndent) {
				this.label = label;
				this.center = ellipse.Center;
				this.ellipse = ellipse.Inflate(label.LabelBounds.Width / 2.0, label.LabelBounds.Height / 2.0);
				ocupiedWidth = label.LabelBounds.Width + resolveOverlappingMinIndent;
				ocupiedHeight = label.LabelBounds.Height + resolveOverlappingMinIndent;
				SetCenter(new GRealPoint2D((label.LabelBounds.Left + label.LabelBounds.Right) / 2.0, (label.LabelBounds.Top + label.LabelBounds.Bottom) / 2.0));
			}
			public void Flush(GRealRect2D bounds, int resolveOverlappingMinIndent) {
				Label.LabelBounds = new GRect2D((int)Math.Round(bounds.Left), (int)Math.Round(bounds.Top), (int)(bounds.Width - resolveOverlappingMinIndent), (int)(bounds.Height - resolveOverlappingMinIndent));
			}
			public void SetCenter(GRealPoint2D point) {
				Bounds = new GRealRect2D(point.X - 0.5 * ocupiedWidth, point.Y - 0.5 * ocupiedHeight, ocupiedWidth, ocupiedHeight);
				refAngle = Math.Atan2(center.Y - Bounds.Center.Y, Bounds.Center.X - center.X);
			}
			int IComparable<LabelInfo>.CompareTo(LabelInfo other) {
				if (other == null)
					return 1;
				return refAngle.CompareTo(other.refAngle);
			}
		}
		class LabelInfoGroup : List<LabelInfo>, IComparable<LabelInfoGroup> {
			public LabelInfoGroup() {
			}
			public LabelInfoGroup(LabelInfo info) {
				Add(info);
			}
			int IComparable<LabelInfoGroup>.CompareTo(LabelInfoGroup other) {
				return this.Count.CompareTo(other.Count);
			}
		}
		#endregion
		List<LabelInfo> list;
		List<LabelInfo> sortedList;
		GRealPoint2D center;
		public void ArrangeLabels(IList<IPieLabelLayout> labels, GRealEllipse ellipse, int resolveOverlappingMinIndent, PointsSweepDirection direction, GRealRect2D diagramBounds) {
			center = ellipse.Center;
			List<LabelInfo> actualInfo = GetLabelsInfoForArrangeByEllipse(labels, ellipse, resolveOverlappingMinIndent);
			if (actualInfo.Count < 2)
				return;
			ArrangeByEllipseInternal(actualInfo, direction, diagramBounds);
			foreach (LabelInfo info in actualInfo)
				info.Flush(info.Bounds, resolveOverlappingMinIndent);
		}
		List<LabelInfo> GetLabelsInfoForArrangeByEllipse(IList<IPieLabelLayout> labels, GRealEllipse ellipse, int resolveOverlappingMinIndent) {
			List<LabelInfo> info = new List<LabelInfo>();
			if (labels != null) {
				foreach (IPieLabelLayout label in labels) {
					if (label == null || !label.Visible || !label.ResolveOverlapping)
						continue;
					GRealRect2D bounds = new GRealRect2D(label.LabelBounds.Left, label.LabelBounds.Top, label.LabelBounds.Width, label.LabelBounds.Height);
					if (bounds.Width > 0 && bounds.Height > 0) {
						bounds.Inflate(resolveOverlappingMinIndent / 2.0, resolveOverlappingMinIndent / 2.0);
						if (bounds.Width > 0 && bounds.Height > 0)
							info.Add(new LabelInfo(label, ellipse, resolveOverlappingMinIndent));
					}
				}
			}
			return info;
		}
		List<LabelInfoGroup> CalcGroups(List<LabelInfo> labelsInfo) {
			List<LabelInfoGroup> groups = new List<LabelInfoGroup>();
			LabelInfoGroup group = new LabelInfoGroup();
			for (int i = 0; i < labelsInfo.Count; i++) {
				if (IsPositionValidByBounds(labelsInfo[i])) {
					if (group.Count > 0) {
						groups.Add(group);
						group = new LabelInfoGroup();
					}
					groups.Add(new LabelInfoGroup(labelsInfo[i]));
				}
				else
					group.Add(labelsInfo[i]);
			}
			if (group.Count > 0) {
				if (groups.Count == 0 || IsPositionValidByBounds(labelsInfo[0]))
					groups.Add(group);
				else {
					group.AddRange(groups[0]);
					groups[0] = group;
				}
			}
			groups.Sort();
			return groups;
		}
		void ArrangeByEllipseInternal(List<LabelInfo> labelsInfo, PointsSweepDirection direction, GRealRect2D diagramBounds) {
			for (int i = 0; i < labelsInfo.Count; i++) {
				int nextIndex = i == labelsInfo.Count - 1 ? 0 : i + 1;
				int prevIndex = i == 0 ? labelsInfo.Count - 1 : i - 1;
				labelsInfo[i].Next = labelsInfo[nextIndex];
				labelsInfo[i].Prev = labelsInfo[prevIndex];
			}
			List<LabelInfoGroup> groups = CalcGroups(labelsInfo);
			list = new List<LabelInfo>();
			foreach (LabelInfoGroup group in groups) {
				if (group.Count == 1)
					Arrange(group[0], direction, diagramBounds);
				else {
					for (int i = 0, k = group.Count - 1; i <= k; i++, k--) {
						Arrange(group[i], direction, diagramBounds);
						if (i != k)
							Arrange(group[k], direction, diagramBounds);
					}
				}
			}
		}
		bool Arrange(LabelInfo labelInf, PointsSweepDirection direction, GRealRect2D diagramBounds) {
			list.Add(labelInf);
			sortedList = list;
			sortedList.Sort();
			labelInf.Arranged = true;
			if (IsPositionValid(labelInf))
				return true;
			labelInf.CurrentSrc = true;
			Push(GetArrangedNext(labelInf), direction, diagramBounds, true);
			Push(GetArrangedPrev(labelInf), direction, diagramBounds, false);
			labelInf.CurrentSrc = false;
			return IsPositionValid(labelInf);
		}
		void Push(LabelInfo labelInf, PointsSweepDirection direction, GRealRect2D diagramBounds, bool forward) {
			if (labelInf.CurrentSrc || IsPositionValid(labelInf, forward))
				return;
			GRealPoint2D point = CalcCenter(labelInf, forward, direction);
			labelInf.SetCenter(point);
			CorrectCenterByDiagramBounds(labelInf, point, diagramBounds, direction);
			if (sortedList != null)
				sortedList.Sort();
			if (forward)
				Push(GetArrangedNext(labelInf), direction, diagramBounds, forward);
			else
				Push(GetArrangedPrev(labelInf), direction, diagramBounds, forward);
		}
		void CorrectCenterByDiagramBounds(LabelInfo labelInf, GRealPoint2D center, GRealRect2D diagramBounds, PointsSweepDirection direction) {
			GRealRect2D prevLabelBounds = labelInf.Prev.Bounds;
			GRealRect2D nextLabelBounds = labelInf.Next.Bounds;
			bool needTopCorrection = labelInf.Bounds.Top - diagramBounds.Top <= 0;
			bool needBottomCorrection = diagramBounds.Bottom - labelInf.Bounds.Bottom <= 0;
			bool isClockWiseDirection = direction == PointsSweepDirection.Clockwise;
			if (needTopCorrection || needBottomCorrection) {
				GRealPoint2D correctedCenter = needTopCorrection ? new GRealPoint2D(center.X, diagramBounds.Top + labelInf.Bounds.Height / 2) : new GRealPoint2D(center.X, diagramBounds.Bottom - labelInf.Bounds.Height / 2);
				labelInf.SetCenter(correctedCenter);
				bool nexLabelIsNearest = NexLabelIsNearest(labelInf);
				if (!labelInf.NextLabelNearest.HasValue)
					labelInf.NextLabelNearest = NexLabelIsNearest(labelInf);
				if (!IsCorrectedPositionValid(labelInf, !labelInf.NextLabelNearest)) {
					if (!isClockWiseDirection ^ needTopCorrection)
						labelInf.SetCenter(new GRealPoint2D(NexLabelIsNearest(labelInf) ? nextLabelBounds.Left - labelInf.Bounds.Width / 2 : prevLabelBounds.Right + labelInf.Bounds.Width / 2, correctedCenter.Y));
					else
						labelInf.SetCenter(new GRealPoint2D(NexLabelIsNearest(labelInf) ? nextLabelBounds.Right + labelInf.Bounds.Width / 2 : prevLabelBounds.Left - labelInf.Bounds.Width / 2, correctedCenter.Y));
				}
			}
		}
		bool NexLabelIsNearest(LabelInfo inf) {
			double distanceToPrevLabel = GeometricUtils.CalcDistance(inf.Bounds.Center, inf.Prev.Bounds.Center);
			double distanceToNextLabel = GeometricUtils.CalcDistance(inf.Bounds.Center, inf.Next.Bounds.Center);
			return distanceToNextLabel < distanceToPrevLabel;
		}
		GRealPoint2D CalcCenter(LabelInfo labelInf, bool fromPrev, PointsSweepDirection direction) {
			GRealRect2D actualBounds = GRealRect2D.Inflate(fromPrev ? GetArrangedPrev(labelInf).Bounds : GetArrangedNext(labelInf).Bounds, labelInf.Bounds.Width / 2.0, labelInf.Bounds.Height / 2.0);
			double radius = GeometricUtils.CalcDistance(center, labelInf.Bounds.Center);
			GRealEllipse ellipse = new GRealEllipse(center, radius, radius);
			List<GRealPoint2D> points = GeometricUtils.CalcRectWithEllipseIntersection(actualBounds, ellipse);
			if (points.Count == 0)
				return labelInf.Bounds.Center;
			return FindPoint(points, actualBounds, center, fromPrev, direction);
		}
		GRealPoint2D FindPoint(IList<GRealPoint2D> points, GRealRect2D bounds, GRealPoint2D center, bool forward, PointsSweepDirection direction) {
			double angle1 = CalcAngle(center, bounds.Center);
			double angle2 = CalcAngle(center, new GRealPoint2D(center.X - (bounds.Center.X - center.X), center.Y + (center.Y - bounds.Center.Y)));
			double maxCos = -double.MaxValue;
			GRealPoint2D point = bounds.Center;
			GRealVector2D v1 = new GRealVector2D(bounds.Center.X - center.X, bounds.Center.Y - center.Y);
			v1.Normalize();
			foreach (GRealPoint2D p in points) {
				double angle = CalcAngle(center, p);
				GRealVector2D v2 = new GRealVector2D(p.X - bounds.Center.X, p.Y - bounds.Center.Y);
				v2.Normalize();
				double cos = GeometricUtils.ScalarProduct(v1, v2);
				bool isClockwiseDirection = (direction == PointsSweepDirection.Clockwise) ? !forward : forward;
				if (cos > maxCos && ((!isClockwiseDirection && IsAngleInRange(angle, angle1, angle2)) || (isClockwiseDirection && IsAngleInRange(angle, angle2, angle1)))) {
					maxCos = cos;
					point = p;
				}
			}
			return point;
		}
		bool IsAngleInRange(double angle, double angle1, double angle2) {
			return angle1 < angle2 ?
				angle >= angle1 && angle <= angle2 :
				angle <= angle2 || angle >= angle1;
		}
		double CalcAngle(GRealPoint2D p1, GRealPoint2D p2) {
			return Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
		}
		bool IsPositionValid(LabelInfo labelInf, bool fromPrev) {
			GRealRect2D actualBounds = GRealRect2D.Inflate(fromPrev ? GetArrangedPrev(labelInf).Bounds : GetArrangedNext(labelInf).Bounds, labelInf.Bounds.Width / 2.0, labelInf.Bounds.Height / 2.0);
			return !actualBounds.Contains(labelInf.Bounds.Center) && GetNextByRefAngle(labelInf).Arranged && GetPrevByRefAngle(labelInf).Arranged;
		}
		bool IsCorrectedPositionValid(LabelInfo labelInf, bool? fromPrev) {
			GRealRect2D actualBounds = GRealRect2D.Inflate(fromPrev.Value ? GetArrangedPrev(labelInf).Bounds : GetArrangedNext(labelInf).Bounds, labelInf.Bounds.Width / 2.0, labelInf.Bounds.Height / 2.0);
			return !actualBounds.ContainsIncludeBounds(labelInf.Bounds.Center);
		}
		bool IsPositionValidByBounds(LabelInfo labelInf) {
			double widthCorrection = labelInf.Bounds.Width / 2.0;
			double heightCorrection = labelInf.Bounds.Height / 2.0;
			GRealRect2D prevRect = GRealRect2D.Inflate(labelInf.Prev.Bounds, widthCorrection, heightCorrection);
			GRealRect2D nextRect = GRealRect2D.Inflate(labelInf.Next.Bounds, widthCorrection, heightCorrection);
			return !prevRect.Contains(labelInf.Bounds.Center) && !nextRect.Contains(labelInf.Bounds.Center);
		}
		bool IsPositionValid(LabelInfo labelInf) {
			double widthCorrection = labelInf.Bounds.Width / 2.0;
			double heightCorrection = labelInf.Bounds.Height / 2.0;
			LabelInfo prevInfo = GetArrangedPrev(labelInf);
			LabelInfo nextInfo = GetArrangedNext(labelInf);
			GRealRect2D prev = GRealRect2D.Inflate(prevInfo.Bounds, widthCorrection, heightCorrection);
			GRealRect2D next = GRealRect2D.Inflate(nextInfo.Bounds, widthCorrection, heightCorrection);
			return (labelInf == prevInfo || !prev.Contains(labelInf.Bounds.Center)) && (labelInf == nextInfo || !next.Contains(labelInf.Bounds.Center)) &&
				GetNextByRefAngle(labelInf).Arranged && GetPrevByRefAngle(labelInf).Arranged;
		}
		LabelInfo GetNextByRefAngle(LabelInfo labelInf) {
			int index = sortedList.IndexOf(labelInf);
			return sortedList[index == sortedList.Count - 1 ? 0 : index + 1];
		}
		LabelInfo GetPrevByRefAngle(LabelInfo labelInf) {
			int index = sortedList.IndexOf(labelInf);
			return sortedList[index == 0 ? sortedList.Count - 1 : index - 1];
		}
		LabelInfo GetArrangedPrev(LabelInfo labelInf) {
			return labelInf.Prev.Arranged ? labelInf.Prev : GetArrangedPrev(labelInf.Prev);
		}
		LabelInfo GetArrangedNext(LabelInfo labelInf) {
			return labelInf.Next.Arranged ? labelInf.Next : GetArrangedNext(labelInf.Next);
		}
	}
}
