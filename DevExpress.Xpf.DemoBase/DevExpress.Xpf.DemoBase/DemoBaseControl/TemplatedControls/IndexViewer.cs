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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.DemoBase {
	class IndexViewer : Control {
		public static readonly DependencyProperty ItemsCountProperty =
			DependencyProperty.Register("ItemsCount", typeof(int), typeof(IndexViewer), new PropertyMetadata(1));
		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register("SelectedIndex", typeof(int), typeof(IndexViewer), new PropertyMetadata(0));
		public int ItemsCount { get { return (int)GetValue(ItemsCountProperty); } set { SetValue(ItemsCountProperty, value); } }
		public int SelectedIndex { get { return (int)GetValue(SelectedIndexProperty); } set { SetValue(SelectedIndexProperty, value); } }
		public IndexViewer() {
			this.SetDefaultStyleKey(typeof(IndexViewer));
		}
	}
	class IndexViewerPanel : Panel {
		public static readonly DependencyProperty ItemsCountProperty =
			DependencyProperty.Register("ItemsCount", typeof(int), typeof(IndexViewerPanel), new PropertyMetadata(1,
			   (d, e) => ((IndexViewerPanel)d).OnItemsCountChanged(e)));
		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register("SelectedIndex", typeof(int), typeof(IndexViewerPanel), new PropertyMetadata(0,
				(d, e) => ((IndexViewerPanel)d).OnSelectedIndexChanged(e)));
		public int ItemsCount { get { return (int)GetValue(ItemsCountProperty); } set { SetValue(ItemsCountProperty, value); } }
		public int SelectedIndex { get { return (int)GetValue(SelectedIndexProperty); } set { SetValue(SelectedIndexProperty, value); } }
		protected override Size MeasureOverride(Size availableSize) {
			double thumbWidth = availableSize.Width / ItemsCount;
			double childMaxWidth = 0.0;
			double childMaxHeight = 0.0;
			foreach(UIElement child in Children) {
				child.Measure(new Size(thumbWidth, availableSize.Height));
				if(child.DesiredSize.Width > childMaxWidth)
					childMaxWidth = child.DesiredSize.Width;
				if(child.DesiredSize.Height > childMaxHeight)
					childMaxHeight = child.DesiredSize.Height;
			}
			return new Size(double.IsInfinity(availableSize.Width) ? childMaxWidth : availableSize.Width, double.IsInfinity(availableSize.Height) ? childMaxHeight : availableSize.Height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double thumbWidth = ItemsCount > 0 ? finalSize.Width / ItemsCount : 0.0;
			double thumbLeft = (finalSize.Width * (SelectedIndex - 1)) / ItemsCount;
			foreach(UIElement child in Children) {
				child.Arrange(new Rect(new Point(thumbLeft, 0.0), new Size(thumbWidth, finalSize.Height)));
			}
			return finalSize;
		}
		void OnItemsCountChanged(DependencyPropertyChangedEventArgs e) { InvalidateMeasure(); }
		void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e) { InvalidateMeasure(); }
	}
}
