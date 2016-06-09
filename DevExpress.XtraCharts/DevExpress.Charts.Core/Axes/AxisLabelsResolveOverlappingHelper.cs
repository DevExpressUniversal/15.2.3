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
namespace DevExpress.Charts.Native {
	public interface IAxisLabelLayout {
		string Text { get; }
		GRealSize2D Size { get; }
		GRealPoint2D Pivot { get; }
		GRealRect2D Bounds { get; }
		double Angle { get; set; }
		GRealPoint2D Offset { get; set; }
		GRealPoint2D LimitsOffset { get; set; }
		bool Visible { get; set; }
		bool IsCustomLabel { get; }
		int GridIndex { get; }
	}
	public interface IAxisLabelResolveOverlappingOptions {
		bool AllowHide { get; }
		bool AllowStagger { get; }
		bool AllowRotate { get; }
		int MinIndent { get; }
	}
	class RotationMatrix {
		double m00 = 1.0;
		double m01 = 0.0;
		double m10 = 0.0;
		double m11 = 1.0;
		double GetRadianAngle(double degreeAngle) {
			return (degreeAngle % 360.0) * Math.PI / 180.0;
		}
		public void Rotate(double angle) {
			double radAngle = GetRadianAngle(angle);
			m00 = Math.Cos(radAngle);
			m01 = -Math.Sin(radAngle);
			m10 = Math.Sin(radAngle);
			m11 = Math.Cos(radAngle);
		}
		public GRealPoint2D TransformPoint(GRealPoint2D point) {
			double x = point.X * m00 + point.Y * m01;
			double y = point.X * m10 + point.Y * m11;
			return new GRealPoint2D(x, y);
		}
	}
	public class AxisLabelOverlappingResolver {
		enum State {
			Check,
			Stagger,
			Rotate,
			ThinOut,
			Panic
		}
		struct LabelItem {
			IAxisLabelLayout label;
			GRealRect2D rect;
			bool isOverlapped;
			public IAxisLabelLayout Label {
				get {
					return label;
				}
			}
			public GRealRect2D Rect {
				get {
					return rect;
				}
			}
			public bool IsOverlapped {
				get {
					return isOverlapped;
				}
				set {
					isOverlapped = value;
				}
			}
			public LabelItem(IAxisLabelLayout label, GRealRect2D rect) {
				this.label = label;
				this.rect = rect;
				this.isOverlapped = false;
			}
		}
		struct OverlapState {
			public RotationMatrix Transform { get; set; }
			public LabelItem[] Items { get; set; }
			public LabelItem? FirstItem { get; set; }
			public void PushItem(LabelItem item) {
				if (FirstItem == null)
					FirstItem = item;
				for (int i = 0; i < Items.Length - 1; i++)
					Items[i] = Items[i + 1];
				Items[Items.Length - 1] = item;
			}
			public bool CheckRect(GRealRect2D rect) {
				bool overlapped = false;
				for (int i = 0; i < Items.Length; i++) {
					LabelItem item = Items[i];
					if ((item.Label != null) && (GRealRect2D.IsIntersected(item.Rect, rect))) {
						Items[i].IsOverlapped = true;
						overlapped = true;
					}
					else
						Items[i].IsOverlapped = false;
				}
				if (!overlapped && FirstItem.HasValue)
					overlapped = GRealRect2D.IsIntersected(FirstItem.Value.Rect, rect);
				return overlapped;
			}
			public bool CheckLimitedRect(GRealRect2D rect) {
				bool overlapped = false;
				for (int i = 0; i < Items.Length; i++) {
					LabelItem item = Items[i];
					IAxisLabelLayout label = item.Label;
					Items[i].IsOverlapped = (label != null) && (label.LimitsOffset != new GRealPoint2D()) && (GRealRect2D.IsIntersected(item.Rect, rect));
					overlapped |= Items[i].IsOverlapped;
				}
				return overlapped;
			}
			public void HideOverlappedItems() {
				foreach (LabelItem item in Items) {
					if (item.Label != null && item.IsOverlapped)
						item.Label.Visible = false;
				}	   
			}
			public bool GetOverlappedState() {
				foreach (LabelItem item in Items)
					if (item.IsOverlapped)
						return true;
				return false;
			}
		}
		const float angleMax = 90;
		const float angleStep = 15;
		State state;
		OverlapState overlapState;
		double maxHeight;
		double maxWidth;
		double currentAngle;
		bool staggered;
		readonly bool isVertical;
		readonly List<IAxisLabelLayout> totalLabels;
		readonly List<IAxisLabelLayout> autoLabels = new List<IAxisLabelLayout>();
		readonly List<IAxisLabelLayout> customLabels = new List<IAxisLabelLayout>();
		readonly bool allowStagger;
		readonly bool allowRotate;
		readonly bool allowThinOut;
		readonly bool mirrored;
		readonly bool forceStagger;
		readonly bool useBoundsRound;
		readonly int indent;
		readonly bool adaptive;
		readonly bool canShowCustomWithAutoLabels;
		bool RotationLimit { get { return (currentAngle < 0) || ((angleMax - currentAngle) < angleStep); } }
		public AxisLabelOverlappingResolver(List<IAxisLabelLayout> labels, IAxisData axis, bool isRadar, bool mirrored, bool useBoundsRound) {
			this.isVertical = axis.IsVertical;
			IAxisLabel settings = axis.Label;
			IAxisLabelResolveOverlappingOptions overlappingOptions = settings.ResolveOverlappingOptions;
			this.totalLabels = labels;
			this.currentAngle = isRadar ? 0 : settings.Angle;
			this.forceStagger = !isRadar && settings.Staggered;
			this.allowStagger = !isRadar && overlappingOptions.AllowStagger && !isVertical && !forceStagger && (currentAngle == 0);
			this.allowRotate = isRadar || isVertical ? false : overlappingOptions.AllowRotate;
			this.allowThinOut = overlappingOptions.AllowHide;
			this.mirrored = mirrored;
			this.indent = overlappingOptions.MinIndent;
			this.adaptive = !isRadar;
			this.useBoundsRound = useBoundsRound;
			this.canShowCustomWithAutoLabels = axis.CanShowCustomWithAutoLabels;
			if (canShowCustomWithAutoLabels)
				DivideLabels();
			AnalyzeLabels();
			if (forceStagger) {
				this.state = State.Stagger;
				if (canShowCustomWithAutoLabels)
					DoLabelsModifications(autoLabels, customLabels);
				else
					DoLabelsModifications(totalLabels);
			}
			else
				this.state = State.Check;
		}
		void DivideLabels() {
			foreach (IAxisLabelLayout label in totalLabels) {
				if (label.IsCustomLabel)
					customLabels.Add(label);
				else
					autoLabels.Add(label);
			}
		}
		State NextState(State state) {
			switch (state) {
				case State.Check:
					if (allowStagger)
						return State.Stagger;
					return NextState(State.Stagger);
				case State.Stagger:
					if (allowRotate && !RotationLimit)
						return State.Rotate;
					return NextState(State.Rotate);
				case State.Rotate:
					if (allowRotate && !RotationLimit)
						return State.Rotate;
					if (allowThinOut)
						return State.ThinOut;
					return NextState(State.ThinOut);
				default:
				case State.ThinOut:
					return State.Panic;
			}
		}
		void ResetOverlapState() {
			overlapState = CreateOverlapState();
		}
		OverlapState CreateOverlapState(int itemsCount) {
			OverlapState overlapState = new OverlapState();
			overlapState.Transform = new RotationMatrix();
			overlapState.Transform.Rotate(-currentAngle);
			overlapState.Items = new LabelItem[itemsCount];
			return overlapState;
		}
		OverlapState CreateOverlapState() {
			return CreateOverlapState(2);
		}
		GRealRect2D GetBounds(GRealRect2D bounds, bool roundBounds) {
			if (roundBounds) {
				GRect2D roundedBounds = GeometricUtils.StrongRound(bounds);
				return new GRealRect2D(roundedBounds.Left, roundedBounds.Top, roundedBounds.Width, roundedBounds.Height);
			}
			return bounds;
		}
		double GetValue(double value) {
			return useBoundsRound ? GeometricUtils.StrongRound(value) : Math.Ceiling(value);
		}
		GRealRect2D InflateLabel(GRealRect2D rect) {
			if (((state == State.Stagger) || (state == State.ThinOut)) && adaptive)
				if (!isVertical)
					return GRealRect2D.Inflate(rect, indent, 0);
				else
					return GRealRect2D.Inflate(rect, 0, indent);
			else if (state == State.Rotate)
				return GRealRect2D.Inflate(rect, 0, indent);
			return GRealRect2D.Inflate(rect, indent, indent);
		}
		bool IsOverlapped(List<IAxisLabelLayout> labels) {
			DetectOverlapping(labels);
			return overlapState.GetOverlappedState();
		}
		bool IsLabelVisible(IAxisLabelLayout label) {
			return label.Visible && !string.IsNullOrEmpty(label.Text);
		}
		GRealSize2D RotatedSize(RotationMatrix rotation, GRealSize2D size) {
			GRealPoint2D vecA = rotation.TransformPoint(new GRealPoint2D(0, size.Height));
			GRealPoint2D vecB = rotation.TransformPoint(new GRealPoint2D(size.Width, 0));
			GRealPoint2D vecC = rotation.TransformPoint(new GRealPoint2D(size.Width, size.Height));
			GRealPoint2D min = new GRealPoint2D(Math.Min(Math.Min(vecA.X, vecB.X), vecC.X), Math.Min(Math.Min(vecA.Y, vecB.Y), vecC.Y));
			GRealPoint2D max = new GRealPoint2D(Math.Max(Math.Max(vecA.X, vecB.X), vecC.X), Math.Max(Math.Max(vecA.Y, vecB.Y), vecC.Y));
			return new GRealSize2D(max.X - min.X, max.Y - min.Y);
		}
		void AnalyzeLabels() {
			double? maxHeight = null;
			double? maxWidth = null;
			RotationMatrix rotation = null;
			if (currentAngle != 0.0) {
				rotation = new RotationMatrix();
				rotation.Rotate(currentAngle);
			}
			for (int i = 0; i < totalLabels.Count; i++) {
				GRealSize2D size = (rotation != null) ? RotatedSize(rotation, totalLabels[i].Size) : totalLabels[i].Size;
				if ((maxHeight == null) || ((maxHeight != null) && (maxHeight.Value < size.Height) && (i & 1) == 0))
					maxHeight = size.Height;
				if ((maxWidth == null) || ((maxWidth != null) && (maxWidth.Value < size.Width) && (i & 1) == 0))
					maxWidth = size.Width;
			}
			this.maxHeight = (maxHeight.HasValue) ? maxHeight.Value : 0.0f;
			this.maxWidth = (maxWidth.HasValue) ? maxWidth.Value : 0.0f;
		}
		GRealPoint2D GetStaggerOffset(bool rollback) {
			if (isVertical) {
				double offset = GetValue(rollback ? 0 : maxWidth);
				return mirrored ? new GRealPoint2D(offset, 0) : new GRealPoint2D(-offset, 0);
			}
			else {
				double offset = GetValue(rollback ? 0 : maxHeight);
				return mirrored ? new GRealPoint2D(0, -offset) : new GRealPoint2D(0, offset);
			}
		}
		void DoStagger(List<IAxisLabelLayout> labels, bool rollback) {
			int staggerIndex = labels.Count > 0 ? labels[0].GridIndex : 0;
			GRealPoint2D offset = GetStaggerOffset(rollback);
			foreach (IAxisLabelLayout label in labels) {
				if ((label.Visible || forceStagger) && !string.IsNullOrEmpty(label.Text)) {
					if ((staggerIndex % 2) == 0)
						label.Offset = offset;
					staggerIndex++;
				}
			}
		}
		void DoRotate(List<IAxisLabelLayout> labels) {
			foreach (IAxisLabelLayout label in labels)
				label.Angle = currentAngle;
		}
		void DetectOverlapping(List<IAxisLabelLayout> labels) {
			ResetOverlapState();
			foreach (IAxisLabelLayout label in labels) {
				if (IsLabelVisible(label)) {
					GRealRect2D rect = CalculateLabelRect(label.Pivot, label.Size, overlapState.Transform);
					if (overlapState.CheckRect(InflateLabel(rect)))
						return;
					overlapState.PushItem(new LabelItem(label, rect));
				}
			}
		}
		void DoLimitedLabelsThinOut(List<IAxisLabelLayout> labels) {
			OverlapState overlapState = CreateOverlapState();
			foreach (IAxisLabelLayout label in labels) {
				if (IsLabelVisible(label) && !label.Bounds.IsEmpty) {
					GRealRect2D rect = CalculateLabelRect(label.Pivot, label.Size, overlapState.Transform);
					if (label.LimitsOffset != new GRealPoint2D() && overlapState.CheckRect(rect))
						label.Visible = false;
					else {
						if (overlapState.CheckLimitedRect(rect))
							overlapState.HideOverlappedItems();
						overlapState.PushItem(new LabelItem(label, rect));
					}
				}
			}
		}
		void DoCustomLabelsThinOut() {
			OverlapState overlapState = CreateOverlapState(customLabels.Count);
			foreach (IAxisLabelLayout customLabel in customLabels) {
				GRealRect2D rect = CalculateLabelRect(customLabel.Pivot, customLabel.Size, overlapState.Transform);
				overlapState.PushItem(new LabelItem(customLabel, rect));
			}
			foreach (IAxisLabelLayout label in autoLabels) {
				if (IsLabelVisible(label)) {
					GRealRect2D rect = CalculateLabelRect(label.Pivot, label.Size, overlapState.Transform);
					if (overlapState.CheckRect(rect))
						label.Visible = false;
				}
			}
		}
		GRealRect2D CalculateLabelRect(GRealPoint2D location, GRealSize2D size, RotationMatrix transform) {
			GRealPoint2D rectLocation = transform.TransformPoint(location);
			rectLocation = new GRealPoint2D(rectLocation.X - (size.Width * 0.5), rectLocation.Y - (size.Height * 0.5));
			return GetBounds(new GRealRect2D(rectLocation.X, rectLocation.Y, size.Width, size.Height), useBoundsRound);
		}
		void DoThinOut(List<IAxisLabelLayout> labels) {
			ResetOverlapState();
			int hidingCount = 0, maxHided = 0, toHide = 0;
			foreach (IAxisLabelLayout label in labels) {
				if (IsLabelVisible(label)) {
					GRealRect2D rect = CalculateLabelRect(label.Pivot, label.Size, overlapState.Transform);
					if (overlapState.CheckRect(InflateLabel(rect)) || (toHide > 0)) {
						label.Visible = false;
						if (adaptive && !allowStagger) {
							hidingCount++;
							if (toHide > 0)
								toHide--;
						}
					}
					else {
						overlapState.PushItem(new LabelItem(label, rect));
						if (adaptive && !allowStagger) {
							if (maxHided < hidingCount)
								maxHided = hidingCount;
							hidingCount = 0;
							toHide = maxHided;
						}
					}
				}
			}
		}
		void LimitBounds(GRealRect2D labelsLimits) {
			foreach (IAxisLabelLayout label in totalLabels) {
				if (IsLabelVisible(label) && !label.Bounds.IsEmpty) {
					GRealRect2D bounds = label.Bounds;
					double minX = bounds.Left;
					if (bounds.Right > labelsLimits.Right)
						minX = labelsLimits.Right - bounds.Width;
					if (minX < labelsLimits.Left)
						minX = labelsLimits.Left;
					double offsetX = minX - bounds.Left;
					label.LimitsOffset = new GRealPoint2D(offsetX, 0.0);
				}
			}
		}
		void DoLabelsModifications(params List<IAxisLabelLayout>[] labels) {
			switch (state) {
				case State.Stagger:
					foreach (List<IAxisLabelLayout> labelsList in labels)
						DoStagger(labelsList, false);
					staggered = true;
					break;
				case State.Rotate:
					if (staggered) {
						foreach (List<IAxisLabelLayout> labelsList in labels)
							DoStagger(labelsList, true);
						staggered = false;
					}
					currentAngle += angleStep;
					foreach (List<IAxisLabelLayout> labelsList in labels)
						DoRotate(labelsList);
					break;
				case State.ThinOut:
					foreach (List<IAxisLabelLayout> labelsList in labels)
						DoThinOut(labelsList);
					break;
			}
		}
		void ProcessWholeLabels() {
			while (IsOverlapped(totalLabels)) {
				state = NextState(state);
				if (state == State.Panic)
					break;
				DoLabelsModifications(totalLabels);
			}
		}
		void ProcessSeparateLabels() {
			while (IsOverlapped(autoLabels) || IsOverlapped(customLabels)) {
				state = NextState(state);
				if (state == State.Panic)
					break;
				DoLabelsModifications(autoLabels, customLabels);
			}
		}
		public void ProcessCustomLabels() {
			if (canShowCustomWithAutoLabels && customLabels.Count != 0) {
				currentAngle = customLabels[0].Angle;
				DoCustomLabelsThinOut();
			}
		}
		public void ProcessLabelsLimits(GRealRect2D labelsLimits) {
			LimitBounds(labelsLimits);
			if (canShowCustomWithAutoLabels) {
				DoLimitedLabelsThinOut(autoLabels);
				DoLimitedLabelsThinOut(customLabels);
			}
			else
				DoLimitedLabelsThinOut(totalLabels);
		}
		public void Process() {
			if (canShowCustomWithAutoLabels)
				ProcessSeparateLabels();
			else
				ProcessWholeLabels();
		}
	}
	public class AxisLabelResolveOverlappingCache {
		struct CacheItem {
			public double Angle { get; set; }
			public GRealPoint2D Offset { get; set; }
			public bool Visible { get; set; }
		}
		readonly CacheItem[] items;
		readonly IResolveLabelsOverlappingAxis axis;
		public int Count {
			get {
				return (items != null) ? items.Length : 0;
			}
		}
		public AxisLabelResolveOverlappingCache(IResolveLabelsOverlappingAxis axis, List<IAxisLabelLayout> labels) {
			this.axis = axis;
			if (labels.Count != 0) {
				items = new CacheItem[labels.Count];
				int index = 0;
				foreach (IAxisLabelLayout label in labels)
					items[index++] = new CacheItem() { Angle = label.Angle, Offset = label.Offset, Visible = label.Visible };
			}
		}
		public void Apply(List<IAxisLabelLayout> labels) {
			if (items != null) {
				if (items.Length != labels.Count)
					return;
				int index = 0;
				foreach (IAxisLabelLayout label in labels) {
					label.Visible = items[index].Visible;
					label.Offset = items[index].Offset;
					label.Angle = items[index].Angle;
					index++;
				}
			}
		}
		public void Store() {
			axis.OverlappingCache = this;
		}
	}
}
