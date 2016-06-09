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
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageHeadersPanel : Panel {
		public static readonly DependencyProperty ToolbarProperty =
			DependencyProperty.Register("Toolbar", typeof(UIElement), typeof(RibbonPageHeadersPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty PageHeadersContainerProperty =
			DependencyProperty.Register("PageHeadersContainer", typeof(UIElement), typeof(RibbonPageHeadersPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty RibbonProperty =
			DependencyProperty.Register("Ribbon", typeof(RibbonControl), typeof(RibbonPageHeadersPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public UIElement PageHeadersContainer {
			get { return (UIElement)GetValue(PageHeadersContainerProperty); }
			set { SetValue(PageHeadersContainerProperty, value); }
		}
		public UIElement Toolbar {
			get { return (UIElement)GetValue(ToolbarProperty); }
			set { SetValue(ToolbarProperty, value); }
		}
		public RibbonControl Ribbon {
			get { return (RibbonControl)GetValue(RibbonProperty); }
			set { SetValue(RibbonProperty, value); }
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size desiredSize = new Size();
			foreach(UIElement child in Children) {
				child.Measure(availableSize);
				desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height);
				desiredSize.Width += child.DesiredSize.Width;
			}
			if(desiredSize.Width > availableSize.Width && Toolbar != null) {
				desiredSize.Width -= Toolbar.DesiredSize.Width;
				Toolbar.Measure(new Size(Math.Max(Ribbon.Toolbar.GetMinDesiredWidth(), availableSize.Width - PageHeadersContainer.DesiredSize.Width), desiredSize.Height));
				desiredSize.Width += Toolbar.DesiredSize.Width;
				desiredSize.Width -= PageHeadersContainer.DesiredSize.Width;
				PageHeadersContainer.Measure(new Size(Math.Max(0, availableSize.Width - Toolbar.DesiredSize.Width), desiredSize.Height));
				desiredSize.Width += PageHeadersContainer.DesiredSize.Width;
			}
			return desiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(Toolbar == null || PageHeadersContainer == null) {
				Point leftTop = new Point();
				foreach(UIElement child in Children) {
					child.Arrange(new Rect(leftTop, new Size(child.DesiredSize.Width, finalSize.Height)));
					leftTop.X += child.RenderSize.Width;
				}
			} else {
				var toolBarSize = new Size(Toolbar.DesiredSize.Width, finalSize.Height);
				Toolbar.Arrange(new Rect(toolBarSize));
				PageHeadersContainer.Arrange(new Rect(new Point(toolBarSize.Width, 0), new Size(finalSize.Width - toolBarSize.Width, finalSize.Height)));
			}
			return finalSize;
		}
	}
}
