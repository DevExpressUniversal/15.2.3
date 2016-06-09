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
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public abstract class StockElement {
		public static StockElement CreateInstance(StockElements elementKind, StockSeries2DPointLayout pointLayout) {
			switch (elementKind) {
				case StockElements.CenterLine:
					return new CenterLineStockElement(pointLayout);
				case StockElements.OpenLine:
					return new OpenLineStockElement(pointLayout);
				case StockElements.CloseLine:
					return new CloseLineStockElement(pointLayout);
				default:
					ChartDebug.Fail("Unkown StockElements value.");
					return null;
			}
		}
		readonly StockSeries2DPointLayout pointLayout;
		protected StockSeries2DPointLayout PointLayout { get { return pointLayout; } }
		public abstract bool IsVisible { get; }
		public StockElement(StockSeries2DPointLayout pointLayout) {
			this.pointLayout = pointLayout;
		}
		public abstract Size CalculateMeasureSize(Size constraint);
		public abstract Rect CalculateArrangeRect(UIElement element, Size constraint);
	}
	public class CenterLineStockElement : StockElement {
		public override bool IsVisible { get { return PointLayout != null; } }
		public CenterLineStockElement(StockSeries2DPointLayout pointLayout) : base(pointLayout) {
		}
		public override Size CalculateMeasureSize(Size constraint) {
			return constraint;
		}
		public override Rect CalculateArrangeRect(UIElement element, Size constraint) {
			if (PointLayout != null && element != null && IsVisible) {
				double centerElementTop = (1.0 -  Math.Max(PointLayout.LowPortion, PointLayout.HighPortion)) * PointLayout.Bounds.Height;
				double centerElementHeight = Math.Abs(PointLayout.HighPortion - PointLayout.LowPortion) * PointLayout.Bounds.Height;
				double width = CalculateMeasureSize(constraint).Width;
				return new Rect(0, centerElementTop, width, Math.Min(centerElementHeight, constraint.Height));
			}
			else
				return RectExtensions.Zero;
		}
	}
	public class OpenLineStockElement : StockElement {
		public override bool IsVisible { get { return PointLayout != null ? PointLayout.ShowOpenClose != StockType.Close : false; } }
		public OpenLineStockElement(StockSeries2DPointLayout pointLayout) : base(pointLayout) {
		}
		public override Size CalculateMeasureSize(Size constraint) {
			return new Size((int)(constraint.Width / 2), constraint.Height);
		}
		public override Rect CalculateArrangeRect(UIElement element, Size constraint) {
			if (PointLayout != null && element != null && IsVisible) {
				double openElementTop = (1.0 - PointLayout.OpenPortion) * PointLayout.Bounds.Height - element.DesiredSize.Height / 2;
				double width = CalculateMeasureSize(constraint).Width;
				return new Rect(0, openElementTop, width, Math.Min(element.DesiredSize.Height, constraint.Height));
			}
			else
				return RectExtensions.Zero;
		}
	}
	public class CloseLineStockElement : StockElement {
		public override bool IsVisible { get { return PointLayout != null ? PointLayout.ShowOpenClose != StockType.Open : false; } }
		public CloseLineStockElement(StockSeries2DPointLayout pointLayout) : base(pointLayout) {
		}
		public override Size CalculateMeasureSize(Size constraint) {
			return new Size((int)(constraint.Width / 2), constraint.Height);
		}
		public override Rect CalculateArrangeRect(UIElement element, Size constraint) {
			if (PointLayout != null && element != null && IsVisible) {
				double closeElementTop = (1.0 - PointLayout.ClosePortion) * PointLayout.Bounds.Height - element.DesiredSize.Height / 2;
				double width = CalculateMeasureSize(constraint).Width;
				return new Rect((int)(constraint.Width / 2 + 0.5), closeElementTop, width, Math.Min(element.DesiredSize.Height, constraint.Height));
			}
			else
				return RectExtensions.Zero;
		}
	}
}
