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
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class FlipPanel : VirtualizingScrollPanel {
		#region static
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ItemCacheModeProperty;
		static FlipPanel() {
			var dProp = new DependencyPropertyRegistrator<FlipPanel>();
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal, (d, e) => ((UIElement)d).InvalidateMeasure());
			dProp.Register("ItemCacheMode", ref ItemCacheModeProperty, ItemCacheMode.None, (d, e) => ((FlipPanel)d).OnViewCachaModeChanged((ItemCacheMode)e.OldValue, (ItemCacheMode)e.NewValue));
		}
		#endregion
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public ItemCacheMode ItemCacheMode {
			get { return (ItemCacheMode)GetValue(ItemCacheModeProperty); }
			set { SetValue(ItemCacheModeProperty, value); }
		}
		private TranslateTransform translateTransform = new TranslateTransform();
		public FlipPanel() {
			this.RenderTransform = translateTransform;
		}
		protected override Size CalculateExtent(Size availableSize, int itemCount) {
			return new Size(availableSize.Width * itemCount, availableSize.Height * itemCount);
		}
		bool IsHorizontal { get { return Orientation == System.Windows.Controls.Orientation.Horizontal; } }
		protected override bool CanUpdateScrollInfoOnMeasure(Size availableSize) {
			return IsHorizontal ? !double.IsInfinity(availableSize.Width) : !double.IsInfinity(availableSize.Height);
		}
		protected override void GetVisibleRange(Size size, out int firstItemIndex, out int lastItemIndex) {
			ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
			int itemCount = itemsControl.Items.Count;
			if (ItemCacheMode == ItemCacheMode.CacheAll) {
				firstItemIndex = 0;
				lastItemIndex = itemCount - 1;
				return;
			}
			double s;
			double offset;
			if(Orientation == Orientation.Horizontal) {
				s = size.Width;
				offset = HorizontalOffset;
			} else {
				s = size.Height;
				offset = VerticalOffset;
			}
			firstItemIndex = (int)Math.Floor(offset / s);
			lastItemIndex = (int)Math.Ceiling((offset + ViewportWidth) / s) - 1;
			if(lastItemIndex >= itemCount)
				lastItemIndex = itemCount - 1;
		}
		void CleanUpAllItems() {
			forceCleanUp = true;
			CleanUpItems(Children.Count - 1, 0);
			forceCleanUp = false;
		}
		bool forceCleanUp;
		protected override void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated) {
			if (ItemCacheMode == ItemCacheMode.None || forceCleanUp)
				base.CleanUpItems(minDesiredGenerated, maxDesiredGenerated);
		}
		protected virtual void OnViewCachaModeChanged(ItemCacheMode oldValue, ItemCacheMode newValue) {
			if(oldValue == ItemCacheMode.CacheAll)
				CleanUpAllItems();
			InvalidateMeasure();
		}
		protected override void ArrangeChild(int itemIndex, UIElement child, Size finalSize) {
			if(Orientation == Orientation.Horizontal)
				child.Arrange(new Rect(itemIndex * finalSize.Width, VerticalOffset, finalSize.Width, finalSize.Height));
			else
				child.Arrange(new Rect(HorizontalOffset, itemIndex * finalSize.Height, finalSize.Width, finalSize.Height));
		}
		protected override double CalculateOffset(double extent, double viewport, double desiredOffset) {
			if(desiredOffset < 0 || viewport >= extent) desiredOffset = 0;
			else {
				if(desiredOffset + viewport >= extent) {
					desiredOffset = extent - viewport;
				}
			}
			return desiredOffset;
		}
		protected override void OnHorizontalOffsetChanged(double offset) {
			if(ScrollOwner != null)
				ScrollOwner.InvalidateScrollInfo();
			InvalidateMeasure();
		}
		protected override void OnVerticalOffsetChanged(double offset) {
			if(ScrollOwner != null)
				ScrollOwner.InvalidateScrollInfo();
			InvalidateMeasure();
		}
		protected override Rect MakeVisibleCore(Rect rectangle) {
			int itemIndex = Orientation == Orientation.Horizontal ? (int)(rectangle.X / ViewportWidth) : (int)(rectangle.Y / ViewportHeight);
			return Orientation == Orientation.Horizontal ? new Rect(itemIndex * ViewportWidth, VerticalOffset, ViewportWidth, ViewportHeight) : 
				new Rect(HorizontalOffset, itemIndex * ViewportHeight, ViewportWidth, ViewportHeight);
		}
		protected override double LineSizeHorizontal {
			get { return 0d; }
		}
		protected override void AfterArrange() {
			base.AfterArrange();
			translateTransform.X = -GetScrollOfset().X;
			translateTransform.Y = -GetScrollOfset().Y;
		}
	}
}
