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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.HitTest;
using DevExpress.Xpf.Grid.Native;
using System.Windows.Media;
namespace DevExpress.Xpf.Grid {
	public class CellMergingPanel : StackVisibleIndexPanel {
		RowData RowData { get { return DataContext as RowData; } }
		TableView View { get { return RowData.View as TableView; } }
		protected override Size ArrangeSortedChildrenOverride(Size finalSize, IList<UIElement> sortedChildren) {
			double width = 0;
			foreach(LightweightCellEditor cell in sortedChildren) {
				int visibleIndex = View.DataControl.GetRowVisibleIndexByHandleCore(RowData.RowHandle.Value);
				Rect rect = new Rect(width, 0, cell.DesiredSize.Width, finalSize.Height);
				if(View.IsNextRowCellMerged(visibleIndex, cell.Column, true)) {
					rect.Y = double.MinValue;
				} else if(RowData.IsRowInView()) {
					do {
						if(!View.IsPrevRowCellMerged(visibleIndex, cell.Column, true))
							break;
						FrameworkElement row = View.GetRowElementByRowHandle(View.DataControl.GetRowHandleByVisibleIndexCore(visibleIndex - 1));
						rect.Y -= row.DesiredSize.Height;
						rect.Height += row.DesiredSize.Height;
						visibleIndex--;
					} while(true);
				}
				cell.Arrange(rect);
				width += rect.Width;
			}
			return finalSize;
		}
		protected override Size MeasureSortedChildrenOverride(Size availableSize, IList<UIElement> sortedChildren) {
			Size size = base.MeasureSortedChildrenOverride(availableSize, sortedChildren);
			if(TableViewProperties.GetFixedAreaStyle(this) == FixedStyle.None)
				return new Size(0, size.Height);
			else
				return size;
		}
	}
}
