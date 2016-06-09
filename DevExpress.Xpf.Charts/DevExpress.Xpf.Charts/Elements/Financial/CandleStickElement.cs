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
	public abstract class CandleStickElement {
		public static CandleStickElement CreateInstance(CandleStickElements elementKind, CandleStickSeries2DPointLayout pointLayout) {
			switch (elementKind) {
				case CandleStickElements.Candle:
					return new CandleElement(pointLayout);
				case CandleStickElements.InvertedCandle:
					return new InvertedCandleElement(pointLayout);
				case CandleStickElements.TopStick:
					return new TopStickElement(pointLayout);
				case CandleStickElements.BottomStick:
					return new BottomStickElement(pointLayout);
				default:
					ChartDebug.Fail("Unkown CandleStickElements value.");
					return null;
			}
		}
		CandleStickSeries2DPointLayout pointLayout;
		protected CandleStickSeries2DPointLayout PointLayout { get { return pointLayout; } }
		public abstract bool IsVisible { get; }
		public CandleStickElement(CandleStickSeries2DPointLayout pointLayout) {
			this.pointLayout = pointLayout;
		}
		public abstract Rect CalculateArrangeRect(Size constraint);
	}
	public abstract class CandleElementBase : CandleStickElement {
		public CandleElementBase(CandleStickSeries2DPointLayout pointLayout) : base(pointLayout) {
		}
		public override Rect CalculateArrangeRect(Size constraint) {
			if (PointLayout != null && IsVisible) {
				double candleElementHeight = Math.Abs(PointLayout.ClosePortion - PointLayout.OpenPortion) * PointLayout.Bounds.Height;
				double candleElementTop = (1.0 - Math.Max(PointLayout.OpenPortion, PointLayout.ClosePortion)) * PointLayout.Bounds.Height;
				return new Rect(0, candleElementTop, constraint.Width, Math.Min(candleElementHeight, constraint.Height));
			}
			else
				return RectExtensions.Zero;
		}
	}
	public class CandleElement : CandleElementBase {
		public override bool IsVisible { get { return PointLayout != null ? !PointLayout.IsCandleInverted : false; } }
		public CandleElement(CandleStickSeries2DPointLayout pointLayout) : base(pointLayout) {
		}
		public override Rect CalculateArrangeRect(Size constraint) {
			if (PointLayout != null && IsVisible) {
				double candleElementHeight = Math.Abs(PointLayout.ClosePortion - PointLayout.OpenPortion) * PointLayout.Bounds.Height;
				double candleElementTop = (1.0 - Math.Max(PointLayout.OpenPortion, PointLayout.ClosePortion)) * PointLayout.Bounds.Height;
				if (candleElementHeight < 1.0) {
					candleElementTop -= 1.0 - candleElementHeight;
					candleElementHeight = 1.0;
				}
				return new Rect(0, candleElementTop, constraint.Width, Math.Min(candleElementHeight, constraint.Height));
			}
			else
				return RectExtensions.Zero;
		}
	}
	public class InvertedCandleElement : CandleElement {
		public override bool IsVisible { get { return PointLayout != null ? PointLayout.IsCandleInverted : false; } }
		public InvertedCandleElement(CandleStickSeries2DPointLayout pointLayout) : base(pointLayout) {
		}
	}
	public class TopStickElement : CandleStickElement {
		public override bool IsVisible { get { return PointLayout != null; } }
		public TopStickElement(CandleStickSeries2DPointLayout pointLayout) : base(pointLayout) {
		}
		public override Rect CalculateArrangeRect(Size constraint) {
			if (PointLayout != null) {
				double actualClosePortion = PointLayout.IsCandleInverted ? PointLayout.OpenPortion : PointLayout.ClosePortion;
				double topStickElementTop = (1.0 - Math.Max(actualClosePortion, PointLayout.HighPortion)) * PointLayout.Bounds.Height;
				double topStickElementHeight = Math.Abs(actualClosePortion - PointLayout.HighPortion) * PointLayout.Bounds.Height;
				return new Rect(0, topStickElementTop, constraint.Width, Math.Min(topStickElementHeight, constraint.Height));
			}
			else
				return RectExtensions.Zero;
		}
	}
	public class BottomStickElement : CandleStickElement {
		public override bool IsVisible { get { return PointLayout != null; } }
		public BottomStickElement(CandleStickSeries2DPointLayout pointLayout) : base(pointLayout) {
		}
		public override Rect CalculateArrangeRect(Size constraint) {
			if (PointLayout != null) {
				double actualOpenPortion = PointLayout.IsCandleInverted ? PointLayout.ClosePortion : PointLayout.OpenPortion;
				double bottomStickElementTop = (1.0 - Math.Max(actualOpenPortion, PointLayout.LowPortion)) * PointLayout.Bounds.Height;
				double bottomStickElementHeight = Math.Abs(actualOpenPortion - PointLayout.LowPortion) * PointLayout.Bounds.Height;
				return new Rect(0, bottomStickElementTop, constraint.Width, Math.Min(bottomStickElementHeight, constraint.Height));
			}
			else
				return RectExtensions.Zero;
		}
	}
}
