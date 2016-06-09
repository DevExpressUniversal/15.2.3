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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
namespace DevExpress.Xpf.Core {
	public class ScrollViewerBase : ScrollViewer {
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ScrollBar verticalScrollBar = GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
			if(verticalScrollBar != null) {
				verticalScrollBar.Orientation = Orientation.Vertical;
				SetScrollBarBinding(verticalScrollBar, ScrollBar.ValueProperty, VerticalOffsetProperty);
				SetScrollBarBinding(verticalScrollBar, ScrollBar.MaximumProperty, ScrollableHeightProperty);
				SetScrollBarBinding(verticalScrollBar, ScrollBar.ViewportSizeProperty, ViewportHeightProperty);
				SetScrollBarBinding(verticalScrollBar, ScrollBar.VisibilityProperty, ComputedVerticalScrollBarVisibilityProperty);
			}
			ScrollBar horizontalScrollBar = GetTemplateChild("PART_HorizontalScrollBar") as ScrollBar;
			if(horizontalScrollBar != null) {
				horizontalScrollBar.Orientation = Orientation.Horizontal;
				SetScrollBarBinding(horizontalScrollBar, ScrollBar.ValueProperty, HorizontalOffsetProperty);
				SetScrollBarBinding(horizontalScrollBar, ScrollBar.MaximumProperty, ScrollableWidthProperty);
				SetScrollBarBinding(horizontalScrollBar, ScrollBar.ViewportSizeProperty, ViewportWidthProperty);
				SetScrollBarBinding(horizontalScrollBar, ScrollBar.VisibilityProperty, ComputedHorizontalScrollBarVisibilityProperty);
			}
		}
		void SetScrollBarBinding(ScrollBar scrollBar, DependencyProperty scrollBarProperty, DependencyProperty scrollViewerProperty) {
			scrollBar.SetBinding(scrollBarProperty, new Binding(scrollViewerProperty.Name) { Source = this, Mode = BindingMode.OneWay });
		}
	}
}
