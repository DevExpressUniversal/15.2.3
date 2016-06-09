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

using DevExpress.DemoData.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Windows;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.DemoBase {
	class DemoBaseScrollViewer : SlideViewScrollPanel {
		public double ScrollableWidth {
			get { return (double)GetValue(ScrollableWidthProperty); }
			set { SetValue(ScrollableWidthProperty, value); }
		}
		public static readonly DependencyProperty ScrollableWidthProperty =
			DependencyProperty.Register("ScrollableWidth", typeof(double), typeof(DemoBaseScrollViewer), new PropertyMetadata(0d));
		public DemoBaseScrollViewer() {
			HorizontalScrollBarStyle = (Style)new GlobalResource {
				Assembly = string.Format("DevExpress.DemoData{0}.Core", AssemblyInfo.VSuffix),
				XamlPath = demoScrollBarXamlName,
				Key = "DemoScrollBar"
			}.Resource;
			ScrollChanged += panel_ScrollChanged;
			SizeChanged += panel_ScrollChanged;
		}
		const string demoScrollBarXamlName =
		"DemoWindow/DemoScrollBar.xaml"
		;
		void panel_ScrollChanged(object sender, EventArgs e) {
			ScrollableWidth = ScrollAreaSize.Width - ViewportBounds.Width;
		}
		protected override WindowsUI.Internal.LayoutProviderBase CreateLayoutProvider() {
			return new MySlideViewLayoutProvider();
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size baseMeasure = base.MeasureOverride(availableSize);
			if(!double.IsInfinity(availableSize.Width) &&
				!double.IsInfinity(availableSize.Height))
				return availableSize;
			return baseMeasure;
		}
	}
	class MySlideViewLayoutProvider : SlideViewLayoutProvider {
		protected override Size GetItemSize(SlideViewLayoutProvider.SlideViewLayoutItemSize itemSize, Size maxSize) {
			if(double.IsInfinity(itemSize.Width))
				itemSize.Width = maxSize.Height;
			return GetItemBounds(new SlideViewLayoutItemPosition(0, 0), itemSize).Size();
		}
	}
}
