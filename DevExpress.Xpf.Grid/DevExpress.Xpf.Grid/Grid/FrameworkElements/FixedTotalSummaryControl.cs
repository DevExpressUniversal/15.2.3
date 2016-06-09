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
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors;
#if SL
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class FixedTotalSummaryControl : ContentControl {
		public FixedTotalSummaryControl() {
			DefaultStyleKey = typeof(FixedTotalSummaryControl);
		}
#if SL
		FixedTotalSummaryContentPresenter Presenter { get; set; }
		UIElement ellipsis { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Presenter = GetTemplateChild("PART_Presenter") as FixedTotalSummaryContentPresenter;
			ellipsis = GetTemplateChild("PART_Ellipsis") as UIElement;
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size res = base.MeasureOverride(availableSize);
			if(ellipsis != null)
				ellipsis.Visibility = availableSize.Width < Presenter.FullWidth ? Visibility.Visible : Visibility.Collapsed;
			return res;
		}
#endif
	}
	public class FixedTotalSummaryContentPresenter : ContentPresenter {
		internal double FullWidth { get; private set; }
		protected override Size MeasureOverride(Size availableSize) {
			Size res = base.MeasureOverride(new Size(double.PositiveInfinity, availableSize.Height));
			FullWidth = res.Width;
			return res;
		}
	}
	public class FixedTotalSummaryPrintPanel : Panel {
		const string LeftPart = "PART_EditLeft";
		const string RightPart = "PART_EditRight";
		FrameworkElement leftControl, rightControl;
		public FrameworkElement LeftControl {
			get {
				if(leftControl == null)
					leftControl = FindPart(LeftPart);
				return leftControl;
			}
		}
		public FrameworkElement RightControl {
			get {
				if(rightControl == null)
					rightControl = FindPart(RightPart);
				return rightControl;
			}
		}
		bool AllowLeftSide {
			get {
				return LeftControl != null && LeftControl.Visibility == System.Windows.Visibility.Visible;
			}
		}
		FrameworkElement FindPart(string name) { 
			FrameworkElement result = null;
			foreach(FrameworkElement el in Children) {
				if(el.Name == name) {
					result = el;
					break;
				}
			}
			return result;
		}
		protected override Size MeasureOverride(Size constraint) {
			Size remainingSize = Size.Empty;
			if(!AllowLeftSide) {
				RightControl.Measure(constraint);
				return new Size(constraint.Width, RightControl.DesiredSize.Height);
			}
			double usedWidth = 0.0;
			double usedHeight = 0.0;
			double maximumHeight = 0.0;
			remainingSize = new Size(Math.Max(0.0, constraint.Width - usedWidth), Math.Max(0.0, constraint.Height - usedHeight));
			RightControl.Measure(remainingSize);
			usedWidth += RightControl.DesiredSize.Width;
			maximumHeight = Math.Max(maximumHeight, usedHeight + RightControl.DesiredSize.Height);
			remainingSize = new Size(Math.Max(0.0, constraint.Width - usedWidth), Math.Max(0.0, constraint.Height - usedHeight));
			LeftControl.Measure(remainingSize);
			usedWidth += LeftControl.DesiredSize.Width;
			maximumHeight = Math.Max(maximumHeight, usedHeight + LeftControl.DesiredSize.Height);
			maximumHeight = Math.Max(maximumHeight, usedHeight);
			return new Size(constraint.Width, maximumHeight);
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			Rect remainingRect = Rect.Empty;
			if(!AllowLeftSide) {
				remainingRect = GetRemainingRect(arrangeSize, 0, 0, 0, 0);
				RightControl.Arrange(remainingRect);
				return arrangeSize;
			}
			double left = 0.0;
			double top = 0.0;
			double right = 0.0;
			double bottom = 0.0;
			remainingRect = GetRemainingRect(arrangeSize, left, top, right, bottom);
			right += RightControl.DesiredSize.Width;
			remainingRect.X = Math.Max(0.0, arrangeSize.Width - right);
			remainingRect.Width = RightControl.DesiredSize.Width;
			RightControl.Arrange(remainingRect);
			remainingRect = GetRemainingRect(arrangeSize, left, top, right, bottom);
			left += LeftControl.DesiredSize.Width;
			LeftControl.Arrange(remainingRect);
			return arrangeSize;
		}
		Rect GetRemainingRect(Size arrangeSize, double left, double top, double right, double bottom) {
			return new Rect(
				   left,
				   top,
				   Math.Max(0.0, arrangeSize.Width - left - right),
				   Math.Max(0.0, arrangeSize.Height - top - bottom));
		}
	}
}
