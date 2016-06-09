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
using System;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.Internal;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualTimelineHeader : DependencyObject, ISupportCopyFrom<TimelineHeader> {
		#region HeaderLevels
		static readonly DependencyPropertyKey HeaderLevelsPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualTimelineHeader, VisualTimeScaleHeaderLevelCollection>("HeaderLevels", null);
		public static readonly DependencyProperty HeaderLevelsProperty = HeaderLevelsPropertyKey.DependencyProperty;
		public VisualTimeScaleHeaderLevelCollection HeaderLevels { get { return (VisualTimeScaleHeaderLevelCollection)GetValue(HeaderLevelsProperty); } protected set { this.SetValue(HeaderLevelsPropertyKey, value); } }
		internal const string HeaderLevelsPropertyName = "HeaderLevels";
		#endregion
		#region ISupportCopyFrom<TimelineHeader> Members
		void ISupportCopyFrom<TimelineHeader>.CopyFrom(TimelineHeader source) {
			CopyFrom(source);
		}
		#endregion
		protected virtual void CopyFrom(TimelineHeader source) {
			if (HeaderLevels == null)
				HeaderLevels = new VisualTimeScaleHeaderLevelCollection();
			CollectionCopyHelper.Copy(HeaderLevels, source.HeaderLevels);
			TimeScaleHeaderCollection headers = source.HeaderLevels[source.HeaderLevels.Count - 1].Headers;
			if (headers.Count > 0 && !headers[0].Scale.Visible) {
				if (HeaderLevels.Count > 0)
					HeaderLevels.RemoveAt(HeaderLevels.Count - 1);
			}
		}
	}
	public class VisualTimeScaleHeaderLevel : DependencyObject, ISupportCopyFrom<TimeScaleHeaderLevel> {
		#region View
		static readonly DependencyPropertyKey ViewPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualTimeScaleHeaderLevel, SchedulerViewBase>("View", null);
		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;
		public SchedulerViewBase View { get { return (SchedulerViewBase)GetValue(ViewProperty); } protected set { this.SetValue(ViewPropertyKey, value); } }
		#endregion
		#region Headers
		static readonly DependencyPropertyKey HeadersPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualTimeScaleHeaderLevel, VisualTimeScaleHeaderContentCollection>("Headers", null);
		public static readonly DependencyProperty HeadersProperty = HeadersPropertyKey.DependencyProperty;
		public VisualTimeScaleHeaderContentCollection Headers { get { return (VisualTimeScaleHeaderContentCollection)GetValue(HeadersProperty); } protected set { this.SetValue(HeadersPropertyKey, value); } }
		internal const string HeadersPropertyName = "Headers";
		#endregion
		#region IsBaseLevel
		static readonly DependencyPropertyKey IsBaseLevelPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualTimeScaleHeaderLevel, bool>("IsBaseLevel", false);
		public static readonly DependencyProperty IsBaseLevelProperty = IsBaseLevelPropertyKey.DependencyProperty;
		public bool IsBaseLevel { get { return (bool)GetValue(IsBaseLevelProperty); } protected set { this.SetValue(IsBaseLevelPropertyKey, value); } }
		internal const string IsBaseLevelPropertyName = "IsBaseLevel";
		#endregion
		#region ISupportCopyFrom<TimeScaleHeaderLevel> Members
		void ISupportCopyFrom<TimeScaleHeaderLevel>.CopyFrom(TimeScaleHeaderLevel source) {
			CopyFrom(source);
		}
		#endregion
		protected virtual void CopyFrom(TimeScaleHeaderLevel source) {
			if (Headers == null)
				Headers = new VisualTimeScaleHeaderContentCollection();
			CollectionCopyHelper.Copy(Headers, source.Headers);
			this.View = source.View;
		}
	}
	public class VisualTimeScaleHeaderLevelCollection : ObservableCollection<VisualTimeScaleHeaderLevel> {
	}
	public class TimelineLevelHeadersPanel : PixelSnappedUniformGrid {
		const double Epsilon = 2.2204460492503131E-16;
		#region LayoutHelpers
		static List<Rect> CalculateNormalizedChildrenBounds(int cellCount) {
			List<Rect> result = new List<Rect>();
			if (cellCount <= 0)
				return result;
			double w = 1.0 / cellCount;
			for (int i = 0; i < cellCount; i++) {
				double left = w * i;
				double right = w * (i + 1);
				result.Add(new Rect(left, 0, right - left, 1));
			}
			Rect lastRect = result[cellCount - 1];
			lastRect.Width = 1.0 - result[cellCount - 1].X;
			result[cellCount - 1] = lastRect;
			return result;
		}
		static Rect GetChildFinalRect(Size finalSize, Rect normalizedRect) {
			double left = GetSnappedPosition(finalSize.Width, normalizedRect.Left);
			double right = GetSnappedPosition(finalSize.Width, normalizedRect.Right);
			double top = GetSnappedPosition(finalSize.Height, normalizedRect.Top);
			if (double.IsInfinity(top))
				top = 0;
			double bottom = GetSnappedPosition(finalSize.Height, normalizedRect.Bottom);
			return new Rect(new Point(left, top), new Point(right, bottom));
		}
		static Size GetChildFinalSize(Size finalSize, Rect normalizedRect) {
			double right = GetSnappedPosition(finalSize.Width, normalizedRect.Right - normalizedRect.Left);
			double bottom = GetSnappedPosition(finalSize.Height, normalizedRect.Bottom - normalizedRect.Top);
			return new Size(right, bottom);
		}
		static double GetSnappedPosition(double size, double normalizedPosition) {
			if (Double.IsInfinity(size))
				return size;
			if (Math.Abs(normalizedPosition) <= Epsilon || Math.Abs(normalizedPosition - 1.0) <= Epsilon)
				return size * normalizedPosition;
			else
				return (int)(size * normalizedPosition);
		}   
		#endregion
		LoadedUnloadedSubscriber loadSubscriber;
		VisualTreeDependBaseCellsDateHeaderLevelInfo baseCellsInfo;
		public TimelineLevelHeadersPanel() {
			this.loadSubscriber = new LoadedUnloadedSubscriber(this, OnPanelLoad, OnPanelUnload);
		}
		public VisualTreeDependBaseCellsDateHeaderLevelInfo BaseCellsInfo { get { return baseCellsInfo; } }
		protected virtual void OnPanelLoad(FrameworkElement fe) {
			UpdateBaseCellsInfo();
		}
		protected virtual void OnPanelUnload(FrameworkElement fe) {
			BaseCellsInfo.UnregisterTimelineLevelHeadersPanel(this);
			this.baseCellsInfo = null;
		}
		void UpdateBaseCellsInfo() {
			VisualTreeDependBaseCellsDateHeaderLevelInfo info = VisualTreeDependBaseCellsDateHeaderLevelInfo.GetActive(this);
			if (info == null)
				return;
			this.baseCellsInfo = info;
			BaseCellsInfo.RegisterTimelineLevelHeadersPanel(this);
		}
		protected override Size MeasureOverrideCore(Size availableSize) {
			double totalWidth = 0;
			double maxHeight = 0;
			int count = Children.Count;
			for (int i = 0; i < count; i++) {
				UIElement child = Children[i];
				VisualTimeScaleHeader header = TryGetTimeScaleHeader(child);
				if (header == null)
					continue;
				child.Measure(availableSize);
				Size desiredSize = child.DesiredSize;
				totalWidth += desiredSize.Width;
				maxHeight = Math.Max(maxHeight, desiredSize.Height);
			}
			return new Size(double.IsInfinity(availableSize.Width) ? totalWidth : availableSize.Width, maxHeight);
		}
		protected override Size ArrangeOverrideCore(Size finalSize) {
			List<VisualTimeScaleHeader> headers = GetTimeScaleHeaders();
			int baseCellCount = GetBaseCellCount(headers);
			int count = Children.Count;
			List<Rect> childrenBounds;
			if (BaseCellsInfo == null)
				UpdateBaseCellsInfo();
			if (BaseCellsInfo != null) {
				childrenBounds = BaseCellsInfo.GetBaseCellsRects();
				if (childrenBounds.Count < 1)
					return finalSize;
			}
			else {
				childrenBounds = new List<Rect>();
				childrenBounds.AddRange(SplitHorizontally(finalSize, baseCellCount, GetActualContentPadding()));
			}
			for (int i = 0; i < count; i++) {
				UIElement child = Children[i];
				Rect rect = GetChildFinalRect(childrenBounds, child, baseCellCount, finalSize);
				child.Arrange(rect);
				FrameworkElement element = child as FrameworkElement;
				if (element != null)
					element.MaxWidth = rect.Width;
			}
			return finalSize;
		}
		Rect[] SplitHorizontally(Size arrangeSize, int cellCount, Thickness contentPadding) {
			if (cellCount <= 0)
				return new Rect[] { new Rect(new Point(), arrangeSize) };
			Rect[] cells = new Rect[cellCount];
			if (double.IsInfinity(arrangeSize.Width)) {
				Point zeroPoint = new Point(0, 0);
				for (int i = 0; i < cellCount; i++)
					cells[i] = new Rect(zeroPoint, arrangeSize);
				return cells;
			}
			List<Rect> normalizedChildrenBounds = CalculateNormalizedChildrenBounds(cellCount);
			for (int i = 0; i < cellCount; i++) {
				Rect normalizedRect = normalizedChildrenBounds[i];
				Rect finalRect = GetChildFinalRect(arrangeSize, normalizedRect);
				finalRect.Y += contentPadding.Top;
				finalRect.Height -= contentPadding.Top + contentPadding.Bottom;
				if (i == 0) {
					finalRect.X += contentPadding.Left;
					finalRect.Width -= contentPadding.Left;
				}
				if (i == cellCount - 1)
					finalRect.Width -= contentPadding.Right;
				cells[i] = finalRect;
			}
			return cells;
		}
		protected Rect GetChildFinalRect(List<Rect> childrenBounds, UIElement child, int baseCellCount, Size finalSize) {
			VisualTimeScaleHeader header = TryGetTimeScaleHeader(child);
			if (header == null)
				return new Rect();
			double left = GetPosition(childrenBounds[header.StartOffset.LinkCellIndex], header.StartOffset, baseCellCount);
			double right = GetPosition(childrenBounds[header.EndOffset.LinkCellIndex], header.EndOffset, baseCellCount);
			return new Rect(left, 0, right - left, finalSize.Height);
		}
		double GetPosition(Rect childrenBounds, SingleTimelineHeaderCellOffset cellOffset, int baseCellCount) {
			return childrenBounds.Left + childrenBounds.Width * cellOffset.RelativeOffset;
		}
		protected virtual int GetBaseCellCount(List<VisualTimeScaleHeader> headers) {
			if (headers.Count > 0)
				return headers[headers.Count - 1].EndOffset.LinkCellIndex + 1;
			else
				return 0;
		}
		protected virtual List<VisualTimeScaleHeader> GetTimeScaleHeaders() {
			List<VisualTimeScaleHeader> result = new List<VisualTimeScaleHeader>();
			int count = Children.Count;
			for (int i = 0; i < count; i++) {
				VisualTimeScaleHeader header = TryGetTimeScaleHeader(Children[i]);
				if (header != null)
					result.Add(header);
			}
			return result;
		}
		private VisualTimeScaleHeader TryGetTimeScaleHeader(UIElement child) {
			return child as VisualTimeScaleHeader;
		}
	}
}
