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
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public interface ILayout {
		bool Visible { get; }
		Rect Bounds { get; }
		Rect ClipBounds { get; }
		Size Size { get; }
		Point Location { get; }
		double Angle { get; }
	}
	public interface ILayoutElement {
		ILayout Layout { get; }
	}
	public static class LayoutElementHelper {
		public static Rect UnionRect(Rect rect, ILayoutElement layoutElement) {
			if (layoutElement != null) {
				ILayout layout = layoutElement.Layout;
				if (layout != null && layout.Visible) 
					rect.Union(layout.Bounds);
			}
			return rect;
		}
	}
	   public class SeriesLabel2DLayoutCacheItem {
		readonly SeriesLabelItem labelItem;
		readonly Size size;
		GRect2D relativeLabelBounds;
		bool isVisibleForOverlapping;
		public SeriesLabelItem LabelItem { get { return labelItem; } }
		public Size Size { get { return size; } }
		public SeriesLabel2DLayoutCacheItem(SeriesLabelItem labelItem, Size size) {
			this.labelItem = labelItem;
			this.size = size;
		}
		public void Complete(Rect rangeBounds) {
			IXYDiagramLabelLayout layout = labelItem.Layout as IXYDiagramLabelLayout;
			relativeLabelBounds = layout != null ? new GRect2D(layout.LabelBounds.Left - MathUtils.StrongRound(rangeBounds.Left),
				layout.LabelBounds.Top - MathUtils.StrongRound(rangeBounds.Top), layout.LabelBounds.Width, layout.LabelBounds.Height) : new GRect2D(0, 0, 0, 0);
			isVisibleForOverlapping = layout != null ? layout.Visible : false;
		}
		public void UpdateLayout(Rect rangeBounds) {
			IXYDiagramLabelLayout layout = labelItem.Layout as IXYDiagramLabelLayout;
			if (layout != null) {
				layout.LabelBounds = new GRect2D(relativeLabelBounds.Left + MathUtils.StrongRound(rangeBounds.Left),
					relativeLabelBounds.Top + MathUtils.StrongRound(rangeBounds.Top), relativeLabelBounds.Width, relativeLabelBounds.Height);
				layout.Visible = isVisibleForOverlapping;
			}
		}
	}
	public class SeriesLabel2DLayoutCache {
		List<SeriesLabel2DLayoutCacheItem> cacheItems;
		Rect viewport;
		Rect rangeBounds;
		public Rect Viewport {
			get { return viewport; }
			set { viewport = value; }
		}
		public Rect RangeBounds {
			get { return rangeBounds; }
			set { rangeBounds = value; }
		}
		public bool UpdateCacheItems(List<XYSeries> seriesCollection) {
			List<SeriesLabel2DLayoutCacheItem> currentCacheItems = new List<SeriesLabel2DLayoutCacheItem>();
			foreach (XYSeries series in seriesCollection) {
				if (series.ActualLabel != null && series.ActualLabel.Items != null) {
					foreach (SeriesLabelItem labelItem in series.ActualLabel.Items)
						currentCacheItems.Add(new SeriesLabel2DLayoutCacheItem(labelItem, labelItem.LabelSize));
				}
			}
			if (cacheItems == null || cacheItems.Count != currentCacheItems.Count) {
				cacheItems = currentCacheItems;
				return true;
			}
			for (int i = 0; i < currentCacheItems.Count; i++) {
				if (cacheItems[i].LabelItem != currentCacheItems[i].LabelItem || cacheItems[i].Size != currentCacheItems[i].Size) {
					cacheItems = currentCacheItems;
					return true;
				}
			}
			return false;
		}
		public void CompleteCacheItems() {
			foreach (SeriesLabel2DLayoutCacheItem cacheItem in cacheItems)
				cacheItem.Complete(rangeBounds);
		}
		public void UpdateLabelBounds() {
			foreach (SeriesLabel2DLayoutCacheItem cacheItem in cacheItems)
				cacheItem.UpdateLayout(rangeBounds);
		}
		public void Invalidate() {
			cacheItems = null;
		}
	}
}
