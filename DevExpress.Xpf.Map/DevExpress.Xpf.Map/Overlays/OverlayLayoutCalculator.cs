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
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Map.Native {
	public struct AlignmentItem {
		static VerticalAlignment GetActualVerticalAlignment(VerticalAlignment verticalAlignment) {
			return verticalAlignment != VerticalAlignment.Stretch ? verticalAlignment : VerticalAlignment.Center;
		}
		static HorizontalAlignment GetActualHorizontalAlignment(HorizontalAlignment horizontalAlignment) {
			return horizontalAlignment != HorizontalAlignment.Stretch ? horizontalAlignment : HorizontalAlignment.Center;
		}
		VerticalAlignment verticalAlignment;
		HorizontalAlignment horizontalAlignment;
		public VerticalAlignment VerticalAlignment { get { return verticalAlignment; } }
		public HorizontalAlignment HorizontalAlignment { get { return horizontalAlignment; } }
		public AlignmentItem(VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment) {
			this.verticalAlignment = GetActualVerticalAlignment(verticalAlignment);
			this.horizontalAlignment = GetActualHorizontalAlignment(horizontalAlignment);
		}
	}
	public class CompositeNavigationOverlay : IOverlayInfo {
		readonly MapOverlayLayout layout = new MapOverlayLayout();
		readonly IOverlayInfo scrollInfo;
		readonly IOverlayInfo zoomInfo;
		public IOverlayInfo ScrollInfo { get { return scrollInfo; } }
		public IOverlayInfo ZoomInfo { get { return zoomInfo; } }
		public MapOverlayLayout Layout { get { return layout; } }
		public CompositeNavigationOverlay(IOverlayInfo zoomInfo, IOverlayInfo scrollInfo) {
			this.scrollInfo = scrollInfo;
			this.zoomInfo = zoomInfo;
			double width = Math.Max(zoomInfo.Layout.Width, scrollInfo.Layout.Width);
			double height = zoomInfo.Layout.Height + scrollInfo.Layout.Height;
			layout.Size = new Size(width, height);
			layout.Location = new Point();
		}
		#region IOverlayInfo implementation
		HorizontalAlignment IOverlayInfo.HorizontalAlignment { get { return scrollInfo.HorizontalAlignment; } }
		VerticalAlignment IOverlayInfo.VerticalAlignment { get { return scrollInfo.VerticalAlignment; } }
		Control IOverlayInfo.GetPresentationControl() {
			return null;
		}
		void IOverlayInfo.OnAlignmentUpdated() {
		}
		#endregion
	}
	public class OverlayLayoutCalculator {
		const double controlBoundsCorrection = 1.0;
		readonly Dictionary<AlignmentItem, List<IOverlayInfo>> overlays = new Dictionary<AlignmentItem, List<IOverlayInfo>>();
		internal Dictionary<AlignmentItem, List<IOverlayInfo>> Overlays { get { return overlays; } }
		void AlignmentCore(List<IOverlayInfo> sourceItems, Rect controlBounds, AlignmentItem alignment) {
			if (sourceItems == null || sourceItems.Count < 1)
				return;
			if (alignment.HorizontalAlignment == HorizontalAlignment.Right)
				sourceItems.Reverse();
			CompositeNavigationOverlay navigationOverlay = TryCreateNavigationOverlay(sourceItems);
			List<IOverlayInfo> items = ReplaceNavigationInfos(sourceItems, navigationOverlay);
			double fullWidth = CalculateFullWidth(items);
			double left = AlignRectangleX(fullWidth, controlBounds, alignment.HorizontalAlignment);
			foreach (IOverlayInfo item in items) {
				Rect rect = item.Layout.Bounds;
				rect.X = left;
				rect.Y = AlignRectangleY(rect.Height, controlBounds, alignment.VerticalAlignment);
				SetNewBounds(item, controlBounds, rect);
				left += rect.Width;
			}
			if (navigationOverlay != null)
				LayoutNavigation(controlBounds, alignment, navigationOverlay);
		}
		void LayoutNavigation(Rect controlBounds, AlignmentItem alignment, CompositeNavigationOverlay compositeInfo) {
			IOverlayInfo scrollInfo = compositeInfo.ScrollInfo;
			IOverlayInfo zoomInfo = compositeInfo.ZoomInfo;
			Rect compositeRect = compositeInfo.Layout.Bounds;
			Rect zoomRect = AlignRectangle(zoomInfo.Layout.Bounds, compositeRect, GetZoomAlignment(alignment));
			Rect scrollRect = AlignRectangle(scrollInfo.Layout.Bounds, compositeRect, GetScrollAlignment(alignment));
			SetNewBounds(zoomInfo, controlBounds, zoomRect);
			SetNewBounds(scrollInfo, controlBounds, scrollRect);
		}
		double CalculateFullWidth(List<IOverlayInfo> items) {
			double fullWidth = 0.0;
			foreach (IOverlayInfo item in items) {
				fullWidth += item.Layout.Width;
			}
			return fullWidth;
		}
		AlignmentItem GetZoomAlignment(AlignmentItem navigationAlignment) {
			VerticalAlignment zoomAlignment;
			if (navigationAlignment.VerticalAlignment == VerticalAlignment.Bottom)
				zoomAlignment = VerticalAlignment.Top;
			else
				zoomAlignment = VerticalAlignment.Bottom;
			return new AlignmentItem(zoomAlignment, HorizontalAlignment.Center);
		}
		AlignmentItem GetScrollAlignment(AlignmentItem navigationAlignment) {
			VerticalAlignment zoomAlignment;
			if (navigationAlignment.VerticalAlignment == VerticalAlignment.Bottom)
				zoomAlignment = VerticalAlignment.Bottom;
			else
				zoomAlignment = VerticalAlignment.Top;
			return new AlignmentItem(zoomAlignment, HorizontalAlignment.Center);
		}
		IOverlayInfo FindScrollInfo(List<IOverlayInfo> items) {
			foreach(IOverlayInfo item in items)
				if(item is ScrollButtonsInfo) return item;
			return null;
		}
		IOverlayInfo FindZoomInfo(List<IOverlayInfo> items) {
			foreach (IOverlayInfo item in items)
				if (item is ZoomTrackbarInfo) return item;
			return null;
		}
		CompositeNavigationOverlay TryCreateNavigationOverlay(List<IOverlayInfo> items) {
			IOverlayInfo scrollInfo = FindScrollInfo(items);
			IOverlayInfo zoomInfo = FindZoomInfo(items);
			if (scrollInfo != null && zoomInfo != null)
				return new CompositeNavigationOverlay(zoomInfo, scrollInfo);
			return null;
		}
		List<IOverlayInfo> ReplaceNavigationInfos(List<IOverlayInfo> items, CompositeNavigationOverlay compositeNavigationInfo) {
			if (compositeNavigationInfo == null)
				return items;
			List<IOverlayInfo> newItems = new List<IOverlayInfo>();
			foreach (IOverlayInfo item in items) {
				if (item is ZoomTrackbarInfo)
					continue;
				else if (item is ScrollButtonsInfo)
					newItems.Add(compositeNavigationInfo);
				else newItems.Add(item);
			}
			return newItems;
		}
		Rect CorrectControlBounds(Rect controlBounds) {
			controlBounds.Inflate(controlBoundsCorrection, controlBoundsCorrection);
			return controlBounds;
		}
		void SetNewBounds(IOverlayInfo item, Rect controlRect, Rect itemBounds) {
			Rect correctedControlBounds = CorrectControlBounds(controlRect);
			if (!correctedControlBounds.Contains(itemBounds)) {
				item.Layout.Location = new Point();
				item.Layout.Size = new Size();
			}
			else {
				item.Layout.Location = itemBounds.Location;
				item.Layout.Size = itemBounds.Size;
			}
		}
		Rect AlignRectangle(Rect rect, Rect baseRect, AlignmentItem aligment) {
			rect.X = AlignRectangleX(rect.Width, baseRect, aligment.HorizontalAlignment);
			rect.Y = AlignRectangleY(rect.Height, baseRect, aligment.VerticalAlignment);
			return rect;
		}
		double AlignRectangleX(double rectWidth, Rect baseRect, HorizontalAlignment alignment) {
			switch (alignment) {
				case HorizontalAlignment.Left:
					return baseRect.X;
				case HorizontalAlignment.Right:
					return baseRect.X + baseRect.Width - rectWidth;
				default:
					return baseRect.X + (baseRect.Width - rectWidth) / 2;
			}
		}
		double AlignRectangleY(double rectHeight, Rect baseRect, VerticalAlignment alignment) {
			switch (alignment) {
				case VerticalAlignment.Top:
					return baseRect.Y;
				case VerticalAlignment.Bottom:
					return baseRect.Y + baseRect.Height - rectHeight;
				default:
					return baseRect.Y + (baseRect.Height - rectHeight) / 2;
			}
		}
		public void PushOverlay(IOverlayInfo overlay) {
			AlignmentItem key = new AlignmentItem(overlay.VerticalAlignment, overlay.HorizontalAlignment);
			List<IOverlayInfo> list;
			overlays.TryGetValue(key, out list);
			if(list == null) {
				overlays[key] = new List<IOverlayInfo>();
				overlays[key].Add(overlay);
			}
			else
				list.Add(overlay);
		}
		public void CalculateLayout(Size availableSize) {
			Rect controlRect = new Rect(availableSize);
			foreach(KeyValuePair<AlignmentItem, List<IOverlayInfo>> item in overlays)
				AlignmentCore(item.Value, controlRect, item.Key);
		}
		public void Clear() {
			Overlays.Clear();
		}
	}
}
