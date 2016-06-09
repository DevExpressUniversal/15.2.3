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

using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Data.Summary;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Selection;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using WarningException = System.Exception;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using Visual = System.Windows.UIElement;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using FrameworkContentElement = System.Windows.DependencyObject;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
using ContentPresenter = DevExpress.Xpf.Core.XPFContentPresenter;
using System.Drawing;
using System;
#else
using System.Timers;
using System;
using System.Drawing;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class CellsSelectionScroller : SelectionScrollerBase {
		ScrollableCellsAreaPresenter cells;
		public CellsSelectionScroller(ScrollableCellsAreaPresenter cells) : base(null) {
			this.cells = cells;
		}
		protected override SelectionVisualItems VisualItems {
			get { return cells.Data != null ? cells.Data.VisualItems : null; }
		}
		protected override System.Drawing.Point GetLeftTopCoordOffset(System.Drawing.Point pt) {
			System.Drawing.Point offset = Point.Empty;
			if(this.cells.GetMinVisibleLeft() > 0) {
				if(pt.X < 0)
					offset.X = -1;
				if(pt.X > this.cells.ActualWidth)
					offset.X = 1;
			}
			if(this.cells.GetMinVisibleTop() > 0) {
				if(pt.Y < 0)
					offset.Y = -1;
				if(pt.Y > this.cells.ActualHeight)
					offset.Y = 1;
			}
			return offset;
		}
		protected override Point LeftTopCoord {
			get { return new Point(this.cells.Left, this.cells.Top); }
			set {
				this.cells.Left = value.X;
				this.cells.Top = value.Y;
			}
		}
		protected override Point GetCellAt(Point pt) {
			Point res = this.cells.GetCellIndexByCoord(pt.X, pt.Y);
			if(res.X < 0)
				res.X = pt.X < 0 ? Math.Max(this.cells.Left - 1, 0) : this.cells.Left + this.cells.GetViewPortWidth(this.cells.ActualWidth, true);
			if(res.Y < 0)
				res.Y = pt.Y < 0 ? Math.Max(this.cells.Top - 1, 0) : this.cells.Top + this.cells.GetViewPortHeight(this.cells.ActualHeight, true);
			return res;
		}
#if DEBUGTEST
		internal System.Drawing.Point GetCellAtInternal(System.Drawing.Point pt) {
			return GetCellAt(pt);
		}
#endif
	}
	internal class FieldValueSelectionScroller : CellsSelectionScroller {
		FieldValuesPresenter valuePresenter;
		public FieldValueSelectionScroller(ScrollableCellsAreaPresenter presenter, FieldValuesPresenter valuePresenter) : base(presenter) {
			this.valuePresenter = valuePresenter;
		}
		protected override void DoScroll(Point pt) {
			int level = valuePresenter.IsColumn ? pt.X : pt.Y;
			VisualItems.PerformColumnRowSelection(VisualItems.GetLastLevelItem(valuePresenter.IsColumn, Math.Max(0, level - 1)));
		}
		protected override Point GetLeftTopCoordOffset(Point pt) {
			System.Drawing.Point point = base.GetLeftTopCoordOffset(pt);
			if(valuePresenter.IsColumn)
				point.Y = 0;
			else
				point.X = 0;
			return point;
		}
	}
}
