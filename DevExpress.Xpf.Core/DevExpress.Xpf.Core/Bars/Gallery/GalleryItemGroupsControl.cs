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

using System.Windows.Controls;
using System.Windows;
using System;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars {
	public class GalleryItemGroupsControl : ItemsControl {
		#region static
		public static readonly DependencyProperty ItemGlyphBorderTemplateProperty;
		public static readonly DependencyProperty ItemBorderTemplateProperty;
		public static readonly DependencyProperty ScrollBarVisibilityProperty;
		public static readonly DependencyProperty ScrollBarPositionProperty;
		public static readonly DependencyProperty GalleryControlProperty;
		static GalleryItemGroupsControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryItemGroupsControl), new FrameworkPropertyMetadata(typeof(GalleryItemGroupsControl)));
			ItemGlyphBorderTemplateProperty = DependencyPropertyManager.Register("ItemGlyphBorderTemplate", typeof(ControlTemplate), typeof(GalleryItemGroupsControl), new FrameworkPropertyMetadata(null));
			ItemBorderTemplateProperty = DependencyPropertyManager.Register("ItemBorderTemplate", typeof(ControlTemplate), typeof(GalleryItemGroupsControl), new FrameworkPropertyMetadata(null));
			ScrollBarVisibilityProperty = DependencyPropertyManager.Register("ScrollBarVisibility", typeof(Visibility), typeof(GalleryItemGroupsControl), new FrameworkPropertyMetadata(Visibility.Collapsed));
			ScrollBarPositionProperty = DependencyPropertyManager.Register("ScrollBarPosition", typeof(double), typeof(GalleryItemGroupsControl), new FrameworkPropertyMetadata(0d));
			GalleryControlProperty = DependencyPropertyManager.Register("GalleryControl", typeof(GalleryControl), typeof(GalleryItemGroupsControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGalleryControlPropertyChanged)));
		}
		protected static void OnGalleryControlPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryItemGroupsControl)obj).OnGalleryControlChanged(e.OldValue as GalleryControl);
		}
		#endregion
		#region dep props
		public GalleryControl GalleryControl {
			get { return (GalleryControl)GetValue(GalleryControlProperty); }
			set { SetValue(GalleryControlProperty, value); }
		}
		public ControlTemplate ItemBorderTemplate {
			get { return (ControlTemplate)GetValue(ItemBorderTemplateProperty); }
			set { SetValue(ItemBorderTemplateProperty, value); }
		}
		public ControlTemplate ItemGlyphBorderTemplate {
			get { return (ControlTemplate)GetValue(ItemGlyphBorderTemplateProperty); }
			set { SetValue(ItemGlyphBorderTemplateProperty, value); }
		}
		public Visibility ScrollBarVisibility {
			get { return (Visibility)GetValue(ScrollBarVisibilityProperty); }
			set { SetValue(ScrollBarVisibilityProperty, value); }
		}
		public double ScrollBarPosition {
			get { return (double)GetValue(ScrollBarPositionProperty); }
			set { SetValue(ScrollBarPositionProperty, value); }
		}		
		#endregion
		public GalleryItemGroupsControl() {
			LayoutUpdated += new EventHandler(OnLayoutUpdated);
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
			IsTabStop = false;
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			LayoutUpdated -= OnLayoutUpdated;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			LayoutUpdated -= OnLayoutUpdated;
			LayoutUpdated += new EventHandler(OnLayoutUpdated);
		}
		protected virtual void OnLayoutUpdated(object sender, EventArgs e) {
			MaximizeItems();
		}
		protected virtual void GetMaxItemSize(out Size maxContentSize, out Size maxGlyphSize) {
			maxContentSize = new Size(0, 0);
			maxGlyphSize = new Size(0, 0);
			for(int i = 0; i < Items.Count; i++) {
				GalleryItemGroupControl child = GetItem(i);
				if(child == null) continue;
				for(int j = 0; j < child.Items.Count; j++) {
					GalleryItemControl item = child.GetItem(j);
					if(item == null) continue;
					if(item.ContentViewport != null) {
						maxContentSize.Height = Math.Max(item.ContentViewport.ContentSize.Height, maxContentSize.Height);
						maxContentSize.Width = Math.Max(item.ContentViewport.ContentSize.Width, maxContentSize.Width);
					}
					if(item.GlyphViewport != null) {
						maxGlyphSize.Height = Math.Max(item.GlyphViewport.ContentSize.Height, maxGlyphSize.Height);
						maxGlyphSize.Width = Math.Max(item.GlyphViewport.ContentSize.Width, maxGlyphSize.Width);
					}
				}
			}
		}
		protected virtual void MaximizeItems() {
			Size maxContentSize, maxGlyphSize;
			GetMaxItemSize(out maxContentSize, out maxGlyphSize);	  
			if(GalleryControl != null && GalleryControl.Gallery != null) {
				if(!double.IsNaN(GalleryControl.Gallery.ItemGlyphRegionSize.Width)) {
					maxGlyphSize.Width = GalleryControl.Gallery.ItemGlyphRegionSize.Width;
				}
				if(!double.IsNaN(GalleryControl.Gallery.ItemGlyphRegionSize.Height)) {
					maxGlyphSize.Height = GalleryControl.Gallery.ItemGlyphRegionSize.Height;
				}
			}
			for(int i = 0; i < Items.Count; i++) {
				GalleryItemGroupControl child = GetItem(i);
				if(child == null)
					return;
				for(int j = 0; j < child.Items.Count; j++) {
					GalleryItemControl item = child.GetItem(j);
					if(item == null || item.ContentViewport == null || item.GlyphViewport == null)
						continue;
					item.GlyphViewport.Width = maxGlyphSize.Width;
					item.GlyphViewport.Height = maxGlyphSize.Height == 0d ? double.NaN : maxGlyphSize.Height;
					item.ContentViewport.Width = maxContentSize.Width;
					item.ContentViewport.Height = maxContentSize.Height;
				}
			}
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is GalleryItemGroupControl;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			(element as GalleryItemGroupControl).Group = item as GalleryItemGroup;
			(element as GalleryItemGroupControl).GroupsControl = this;			
			base.PrepareContainerForItemOverride(element, item);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new GalleryItemGroupControl();
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			(element as GalleryItemGroupControl).GroupsControl = null;
			(element as GalleryItemGroupControl).Group = null;
			base.ClearContainerForItemOverride(element, item);
		}
		public GalleryItemGroupControl GetItem(int index) {
			return ItemContainerGenerator.ContainerFromIndex(index) as GalleryItemGroupControl;
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size retValue = base.MeasureOverride(availableSize);
			return retValue;
		}
		protected virtual void OnGalleryControlChanged(GalleryControl oldValue) {
			for(int i = 0; i<Items.Count; i++){
				var groupControl = ItemContainerGenerator.ContainerFromIndex(i) as GalleryItemGroupControl;
				if(groupControl == null) continue;
				groupControl.OnGroupsControlGalleryControlChanged();
			}
		}
	}
}
