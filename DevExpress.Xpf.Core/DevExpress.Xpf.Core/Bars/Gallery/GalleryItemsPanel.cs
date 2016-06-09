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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
namespace DevExpress.Xpf.Bars {
	public class GalleryItemsPanel : Panel {
		#region static
		public static readonly DependencyProperty MaxColCountProperty;
		public static readonly DependencyProperty DesiredColCountProperty;
		public static readonly DependencyProperty MinColCountProperty;
		public static readonly DependencyProperty ItemAutoWidthProperty;
		public static readonly DependencyProperty ItemAutoHeightProperty;				
		public static readonly DependencyProperty ViewportSizeProperty;				
		static GalleryItemsPanel() {
			MaxColCountProperty = DependencyPropertyManager.Register("MaxColCount", typeof(int), typeof(GalleryItemsPanel),
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure));
			DesiredColCountProperty = DependencyPropertyManager.Register("DesiredColCount", typeof(int), typeof(GalleryItemsPanel),
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure));
			MinColCountProperty = DependencyPropertyManager.Register("MinColCount", typeof(int), typeof(GalleryItemsPanel),
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ItemAutoWidthProperty = DependencyPropertyManager.Register("ItemAutoWidth", typeof(bool), typeof(GalleryItemsPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ItemAutoHeightProperty = DependencyPropertyManager.Register("ItemAutoHeight", typeof(bool), typeof(GalleryItemsPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ViewportSizeProperty = DependencyPropertyManager.Register("ViewportSize", typeof(Size), typeof(GalleryItemsPanel), new FrameworkPropertyMetadata(Size.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
		}
		#endregion
		#region dep props
		public int MinColCount {
			get { return (int)GetValue(MinColCountProperty); }
			set { SetValue(MinColCountProperty, value); }
		}
		public int DesiredColCount {
			get { return (int)GetValue(DesiredColCountProperty); }
			set { SetValue(DesiredColCountProperty, value); }
		}
		public int MaxColCount {
			get { return (int)GetValue(MaxColCountProperty); }
			set { SetValue(MaxColCountProperty, value); }
		}
		public bool ItemAutoWidth {
			get { return (bool)GetValue(ItemAutoWidthProperty); }
			set { SetValue(ItemAutoWidthProperty, value); }
		}
		public bool ItemAutoHeight {
			get { return (bool)GetValue(ItemAutoHeightProperty); }
			set { SetValue(ItemAutoHeightProperty, value); }
		}
		public Size ViewportSize {
			get { return (Size)GetValue(ViewportSizeProperty); }
			set { SetValue(ViewportSizeProperty, value); }
		}
		#endregion
		public GalleryItemsPanel() {
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
		}	   
		void OnLoaded(object sender, RoutedEventArgs e) {
			SetBindings();
		}
		IEnumerable<GalleryItemControl> VisibleChildren {
			get {
				foreach(GalleryItemControl child in Children)
					if(child.Item.IsVisible)
						yield return child;
				yield break;
			}
		}
		GalleryControl BindingSource { get; set; }
		protected void SetBindings() {
			if(BindingSource == GalleryControl) return;
			BindingSource = GalleryControl;
			SetBinding(MinColCountProperty,
				new Binding() {
					Path = new PropertyPath("(0).(1).(2)", GalleryControl.GalleryControlProperty, GalleryControl.GalleryProperty, Gallery.MinColCountProperty)
				, RelativeSource = RelativeSource.Self
				});
			SetBinding(MaxColCountProperty,
				new Binding() {
					Path = new PropertyPath("(0).(1).(2)", GalleryControl.GalleryControlProperty, GalleryControl.GalleryProperty, Gallery.ColCountProperty)
				, RelativeSource = RelativeSource.Self
				});
			SetBinding(DesiredColCountProperty,
				new Binding() {
					Path = new PropertyPath("(0).(1)", GalleryControl.GalleryControlProperty, GalleryControl.DesiredColCountProperty)
				, RelativeSource = RelativeSource.Self
				});
			SetBinding(ItemAutoWidthProperty,
				new Binding() {
					Path = new PropertyPath("(0).(1).(2)", GalleryControl.GalleryControlProperty, GalleryControl.GalleryProperty, Gallery.ItemAutoWidthProperty)
				, RelativeSource = RelativeSource.Self
				});			
			SetBinding(ItemAutoHeightProperty,
				new Binding() {
					Path = new PropertyPath("(0).(1).(2)", GalleryControl.GalleryControlProperty, GalleryControl.GalleryProperty, Gallery.ItemAutoHeightProperty)
				, RelativeSource = RelativeSource.Self
				});			
			var sHost = GalleryControl.GetGalleryControl(this).With(x=>x.ScrollHost);
			if (sHost != null) {
				SetBinding(ViewportSizeProperty,
				new Binding() {
					Path = new PropertyPath(GalleryGroupsViewer.ViewportSizeProperty)
				, Source = sHost
				});
			}			
		}
		int ActualDesiredColCount {
			get {
				return GalleryControl == null ? DesiredColCount : GalleryControl.DesiredColCount;
			}
		}
		bool ActualItemAutoWidth {
			get {
				return GalleryControl == null || GalleryControl.Gallery == null ? ItemAutoWidth : GalleryControl.Gallery.ItemAutoWidth;
			}
		}
		int ActualMinColCount {
			get {
				if(GalleryControl == null || GalleryControl.Gallery == null) return MinColCount;
				return GalleryControl.Gallery.MinColCount;
			}
		}
		int ActualMaxColCount {
			get {
				if(GalleryControl == null || GalleryControl.Gallery == null) return MaxColCount;
				return GalleryControl.Gallery.ColCount;
			}
		}
		GalleryControl GalleryControl {
			get {
				return LayoutHelper.FindParentObject<GalleryControl>(this);
			}
		}
		GalleryItemGroupControl Owner {
			get {
				return LayoutHelper.FindParentObject<GalleryItemGroupControl>(this);
			}
		}
		protected virtual double GetActualMinWidth() {
			PopupMenuBarControl borderControl = LayoutHelper.FindParentObject<PopupMenuBarControl>(this);
			double popupMinWidth = 0;
			if(borderControl != null && borderControl.Popup != null) popupMinWidth = borderControl.Popup.MinWidth;
			double actualMinWidth = popupMinWidth;
			if(MinWidth != 0) actualMinWidth = MinWidth;
			return actualMinWidth;
		}
		protected virtual double GetBestWidth(double availableWidth, double itemWidth) {
			int actualColCount = 0;
			if(!double.IsPositiveInfinity(availableWidth)) {
				actualColCount = (int)(availableWidth / itemWidth);
			} else {
				actualColCount = ActualDesiredColCount;
				if(actualColCount == 0)
					actualColCount = Math.Max(ActualMaxColCount, VisibleChildren.Count());
			}
			actualColCount = Math.Min(actualColCount, VisibleChildren.Count());
			if(ActualMaxColCount != 0) actualColCount = Math.Min(actualColCount, ActualMaxColCount);
			actualColCount = Math.Max(actualColCount, ActualMinColCount);
			return Math.Max(actualColCount * itemWidth, GetActualMinWidth());
		}
		bool IsFixedItemSize {
			get {
				Size itemSize = GalleryControl.Gallery.ItemSize;
				return !double.IsNaN(itemSize.Width) && !double.IsNaN(itemSize.Height);
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(!IsLoaded) SetBindings();
			Size sz = new Size(double.PositiveInfinity, double.PositiveInfinity);
			Size itemSize = GalleryControl.Gallery.ItemSize;
			if(!IsFixedItemSize) {
				foreach(GalleryItemControl child in Children) child.Measure(sz);			   
			}
			double bestWidth = 0;
			double itemWidth = GetMaxItemSize().Width;
			Size retVal = new Size();
			if(GalleryControl.HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch) {
				bestWidth = GetBestWidth(availableSize.Width, itemWidth);
				retVal = LayoutChildren(bestWidth);
			}
			else {
				bestWidth = GetBestWidth(double.PositiveInfinity, itemWidth);
				if(!double.IsPositiveInfinity(availableSize.Width) && bestWidth > availableSize.Width) {
					bestWidth = GetBestWidth(availableSize.Width, itemWidth);
				}
				retVal = LayoutChildren(bestWidth);
			}		   
			return retVal;
		}
		Rect? GetViewportRect() {
			if(!IsFixedItemSize) return null;
			GalleryItemGroupsPanel groupsPanel = LayoutHelper.FindParentObject<GalleryItemGroupsPanel>(this);
			GalleryItemGroupControl owner = LayoutHelper.FindParentObject<GalleryItemGroupControl>(this);
			GalleryGroupsViewer viewer = LayoutHelper.FindParentObject<GalleryGroupsViewer>(this);
			Rect rect = new Rect();
			foreach(GalleryItemGroupControl group in groupsPanel.Children) {
				if(group == owner) {
					rect.Y += group.GetCaptionOffset();
					break;
				}
				rect.Y += group.DesiredSize.Height;
			}
			rect.Y = viewer.ContentVerticalOffset - rect.Y;
			rect.Height = viewer.ViewportSize.Height;
			return new Rect?(rect);
		}
		protected override Size ArrangeOverride(Size finalSize) {			
			Size maxItemSize = GetMaxItemSize();			
			Rect? viewPort = GetViewportRect();
			Rect pos = new Rect();
			int childrenCount = VisibleChildren.Count();
			for(int i = 0; i < childrenCount; ) {
				List<GalleryItemControl> line = new List<GalleryItemControl>();
				do {
					line.Add(VisibleChildren.ElementAt(i));
					if(++i >= childrenCount) 
						break;
				} while(!VisibleChildren.ElementAt(i).DesiredStartOfLine);
				ArrangeRow(line, finalSize.Width, maxItemSize, ref pos, viewPort);
				pos.X = 0;
				pos.Y += maxItemSize.Height;
			}
			return finalSize;	 
		}
		protected internal virtual Size GetMaxItemSize() {
			var maxHeight = double.NaN;
			if(GalleryHelper.GetIsInRibbonControl(GalleryControl) && GalleryControl.Gallery.ItemAutoHeight) {
				maxHeight = GalleryControl.ScrollHost.Return(x => x.ViewportSize.Height == 0d ? double.NaN : x.ViewportSize.Height, () => double.NaN);
				if(double.IsNaN(maxHeight))
					maxHeight = GalleryControl.ScrollHost.AvailableHeight;
			}
			var result = new Size(0,0);
			if (!IsFixedItemSize) {
				Size sz = new Size(0, 0);
				foreach (GalleryItemControl child in Children) {
					if (child == null)
						return new Size();
					result.Height = Math.Max(child.DesiredSize.Height, result.Height);
					result.Width = Math.Max(child.DesiredSize.Width, result.Width);
				}
			} else {
				result = GalleryControl.Gallery.ItemSize;
			}
			if(!double.IsNaN(maxHeight) && !double.IsInfinity(maxHeight) && VisibleChildren.Count() > 0)
				result.Height = maxHeight;
			return result;
		}
		protected virtual void ArrangeRow(List<GalleryItemControl> row, double maxWidth, Size maxItemSize, ref Rect pos, Rect? viewportRect) {
			double width = maxItemSize.Width;
			if(!double.IsPositiveInfinity(maxWidth) && ActualItemAutoWidth && maxItemSize.Width * row.Count < maxWidth) {
				width += maxWidth / row.Count - maxItemSize.Width;
			}
			pos.Width = width;
			pos.Height = maxItemSize.Height;
			foreach(GalleryItemControl child in row) {			
				if(viewportRect == null) {
					child.Arrange(pos);
				}
				else {
					if(pos.Y + pos.Height >= viewportRect.Value.Y && pos.Y + pos.Height <= viewportRect.Value.Y + viewportRect.Value.Height ||
						pos.Y >= viewportRect.Value.Y && pos.Y <= viewportRect.Value.Y + viewportRect.Value.Height || VisualTreeHelper.GetChildrenCount(child) != 0) {
						if(VisualTreeHelper.GetChildrenCount(child) == 0) {
							child.Measure(new Size(pos.Width, pos.Height));
						}
						child.Arrange(pos);
					}
				}
				pos.X += width;
			}
		}
		protected internal virtual Size LayoutChildren(double maxWidth) {
			Rect pos = new Rect(0, 0, 0, 0);
			double lineWidth = 0;
			double maxLineWidth = 0;
			int rowCount = 0;
			int colCount = 0;
			Size maxItemSize = GetMaxItemSize();
			int startLineIndex = 0;
			foreach(GalleryItemControl child in VisibleChildren) {
				if(child == null) return new Size();
				if(!child.Item.IsVisible) continue;
				child.DesiredRowIndex = rowCount;
				child.DesiredColIndex = colCount;
				child.DesiredStartOfLine = (colCount == 0);
				colCount++;
				if(lineWidth + maxItemSize.Width > maxWidth + 0.5 && lineWidth != 0 || (colCount > MaxColCount && MaxColCount > 0)) {
					lineWidth = 0;
					pos.X = 0;
					pos.Y += maxItemSize.Height;
					startLineIndex += colCount - 1;
					rowCount++;
					colCount = 1;
					child.DesiredRowIndex = rowCount;
					child.DesiredColIndex = 0;
					child.DesiredStartOfLine = true;
				}
				lineWidth += maxItemSize.Width;
				maxLineWidth = Math.Max(lineWidth, maxLineWidth);
			}
			return new Size(maxLineWidth, pos.Y + maxItemSize.Height);
		}
	}
}
