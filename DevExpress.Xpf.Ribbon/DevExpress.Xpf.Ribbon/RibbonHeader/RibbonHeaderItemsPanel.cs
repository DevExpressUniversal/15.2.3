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
using System.Linq;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonHeaderItemsPanel : RibbonItemsPanel {
		public RibbonHeaderItemsPanel() : base() {
			DefaultStyleKey = typeof(RibbonHeaderItemsPanel);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			var ribbon = Ribbon ?? RibbonControl.GetRibbon(this);
			RibbonPageCategoryCaptionAlignment actualAlignment = RibbonPageCategoryCaptionAlignment.Default;
			if (ribbon != null) {
				actualAlignment = ribbon.PageCategoryAlignment;
			}
			List<RibbonPageCategoryHeaderControl> headers = new List<RibbonPageCategoryHeaderControl>();
			RibbonPageCategoryHeaderControl defaultHeader = null;
			for (int i = 0; i < Children.Count; i++) {
				RibbonPageCategoryHeaderControl header = Children[i] as RibbonPageCategoryHeaderControl;
				if (header.Category != null && header.Category.IsDefault) {
					defaultHeader = header;
					continue;
				}
				headers.Add(header);
			}
			DefaultHeaderWidth = GetDefaultInfoMinWidth();
			if(defaultHeader != null) {
				defaultHeader.Arrange(new Rect(new Point(), defaultHeader.DesiredSize));
				DefaultHeaderWidth = Math.Max(DefaultHeaderWidth, defaultHeader.RenderSize.Width);
			}
			UpdateControlBoxWidth(finalSize.Width);
			double rightOffset = (Ribbon == null || !Ribbon.IsInRibbonWindow) ? 0d : Math.Max(0, ControlBoxWidth);
			double leftOffset = DefaultHeaderWidth;
			if (actualAlignment == RibbonPageCategoryCaptionAlignment.Right) {
				double headersWidth = headers.Sum(header => header.DesiredSize.Width);
				leftOffset = Math.Max(leftOffset, finalSize.Width - rightOffset - headersWidth);
			}
			headers = headers.OrderBy(header => GetChildOrder(header)).ToList();
			for (int i = 0; i < headers.Count; i++) {
				Size sz = headers[i].DesiredSize;
				sz.Width = Math.Max(0d, Math.Min(sz.Width, ActualWidth - leftOffset - rightOffset));
				sz.Height = Math.Max(sz.Height, finalSize.Height);
				headers[i].Arrange(new Rect(new Point(leftOffset, 0d), sz));
				leftOffset += headers[i].DesiredSize.Width;
			}
			return finalSize;
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size result = new Size(0d, 0d);
			foreach (UIElement item in Children) {
				(item as RibbonPageCategoryHeaderControl).Do(x => x.UpdateSizeInfo(false));
				item.Measure(SizeHelper.Infinite);
				result.Width += item.DesiredSize.Width;
				result.Height = Math.Max(result.Height, item.DesiredSize.Height);
			}
			return result;
		}
		protected override int GetChildOrder(UIElement child) {
			RibbonPageCategoryHeaderControl cat = child as RibbonPageCategoryHeaderControl;
			if (cat == null || cat.Ribbon == null || !cat.Ribbon.IsMerged)
				return 0;
			return cat.Category.ActualMergeOrder;
		}
	}
}
