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
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using System.Windows;
using System;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
namespace DevExpress.Xpf.Bars {
	public class GalleryItemGroupsPanel : Panel {
		public static readonly DependencyProperty ViewportSizeProperty;
		static GalleryItemGroupsPanel() {
			ViewportSizeProperty = DependencyPropertyManager.Register("ViewportSize", typeof(Size), typeof(GalleryItemGroupsPanel), new FrameworkPropertyMetadata(Size.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
		}
		public GalleryItemGroupsPanel() {
			Loaded += OnLoaded;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			if (GalleryControl == null)
				return;
			SetBinding(ViewportSizeProperty, new Binding("ViewportSize") { Source = GalleryControl.ScrollHost });
		}
		public Size ViewportSize {
			get { return (Size)GetValue(ViewportSizeProperty); }
			set { SetValue(ViewportSizeProperty, value); }
		}
		GalleryControl GalleryControl {
			get { return FindParentGalleryControl(); }
		}
		private GalleryControl FindParentGalleryControl() {
			return LayoutHelper.FindParentObject<GalleryControl>(this);
		}
		protected override Size MeasureOverride(Size availableSize) {			
			Size sz = new Size(double.PositiveInfinity, double.PositiveInfinity);
			if(!double.IsInfinity(availableSize.Width)) sz.Width = availableSize.Width;
			double height = 0;
			double width = 0;
			foreach(UIElement child in Children) {
				child.Measure(sz);
				height += child.DesiredSize.Height;
				width = Math.Max(width, child.DesiredSize.Width);
			}
			SetIsFirstVisibleGroup();
			return new Size(width, height);
		}
		double GetMaxWidth() {
			double maxWidth = 0;
			foreach(GalleryItemGroupControl child in Children) {
				maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
			}
			return maxWidth;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Rect pos = new Rect(0, 0, 0, 0);
			double actualWidth = Math.Max(GetMaxWidth(), finalSize.Width);
			foreach(GalleryItemGroupControl child in Children) {
				pos.Width = actualWidth;
				pos.Height = child.DesiredSize.Height;
				child.Arrange(pos);
				pos.Y += pos.Height;
			}
			SetIsFirstVisibleGroup();
			return finalSize;
		}
		void SetIsFirstVisibleGroup() {
			bool isFirstVisibleGroupSet = false;
			foreach(GalleryItemGroupControl child in Children) {
				if(child.Visibility == Visibility.Visible && !isFirstVisibleGroupSet) {
					child.IsFirstVisibleGroup = true;
					isFirstVisibleGroupSet = true;
				}
				else {
					child.IsFirstVisibleGroup = false;
				}
			}
		}
	}
}
