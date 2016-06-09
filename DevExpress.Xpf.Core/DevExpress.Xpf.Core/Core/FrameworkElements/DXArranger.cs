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
using DevExpress.Xpf.Core.Native;
#if SL
using Decorator = DevExpress.Xpf.Core.WPFCompatibility.Decorator;
#endif
namespace DevExpress.Xpf.Core {
	public class DXArranger : Decorator {
#if !SL
		public DXArranger() {
			LayoutUpdated += DXArranger_LayoutUpdated;
		}
		UIElement topElement;
		Point lastOffset;
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.Property == FrameworkElement.IsVisibleProperty)
				topElement = LayoutHelper.GetTopLevelVisual(this);
		}
#if DEBUGTEST
		internal int InvalidateArrangeCount { get; set; }
#endif
		void DXArranger_LayoutUpdated(object sender, EventArgs e) {
			if(SnapsToDevicePixels && IsVisible && lastOffset != GetOffset()){
#if DEBUGTEST
				InvalidateArrangeCount++;
#endif
				InvalidateArrange();
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			if(!SnapsToDevicePixels)
				return base.MeasureOverride(constraint);
			base.MeasureOverride(new Size(Floor(constraint.Width), Floor(constraint.Height)));
			return new Size(Ceiling(Child.DesiredSize.Width), Ceiling(Child.DesiredSize.Height));
		}
		public static double Ceiling(double value) {
			return Math.Ceiling(value * ScreenHelper.ScaleX) / ScreenHelper.ScaleX;
		}
		public static double Floor(double value) {
			return Math.Floor(value * ScreenHelper.ScaleX) / ScreenHelper.ScaleX;
		}
		public static double Round(double value, int digits) {
			return Math.Round(value * ScreenHelper.ScaleX, digits) / ScreenHelper.ScaleX;
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			if(!SnapsToDevicePixels)
				return base.ArrangeOverride(arrangeSize);
			lastOffset = GetOffset();
			Child.Arrange(new Rect(lastOffset.X, lastOffset.Y, Math.Floor(arrangeSize.Width), Math.Floor(arrangeSize.Height)));
			return arrangeSize;
		}
		Point GetOffset() {
			if(!IsVisible)
				return new Point();
			Rect r = LayoutHelper.GetRelativeElementRect(this, topElement);
			return new Point(CalculateOffset(r.Left), CalculateOffset(r.Top));
		}
		double CalculateOffset(double value) {
			double result = value - Math.Floor(value);
			if(result < 0.5)
				return -result;
			return 1 - result;
		}
#endif
	}
}
