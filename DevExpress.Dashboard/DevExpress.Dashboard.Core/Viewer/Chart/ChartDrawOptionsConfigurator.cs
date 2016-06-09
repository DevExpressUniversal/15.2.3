#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Drawing;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class ChartDrawOptionsConfigurator {
		public static ChartDrawOptionsConfigurator CreateInstance(DrawOptions drawOptions) {
			BarDrawOptions barDrawOptions = drawOptions as BarDrawOptions;
			if(barDrawOptions != null)
				return new BarDrawOptionsConfigurator(barDrawOptions);
			PointDrawOptions pointDrawOptions = drawOptions as PointDrawOptions;
			if(pointDrawOptions != null)
				return new PointDrawOptionsConfigurator(pointDrawOptions);
			PointDrawOptionsBase pointDrawOptionsBase = drawOptions as PointDrawOptionsBase;
			if(pointDrawOptionsBase != null)
				return new PointDrawOptionsBaseConfigurator(pointDrawOptionsBase);
			FinancialDrawOptions financialDrawOptions = drawOptions as FinancialDrawOptions;
			if(financialDrawOptions != null)
				return new FinancialDrawOptionsConfigurator(financialDrawOptions);
			PieDrawOptions pieDrawOptions = drawOptions as PieDrawOptions;
			if(pieDrawOptions != null)
				return new PieDrawOptionsConfigurator(pieDrawOptions);
			return null;
		}
		protected static Color ChangeLuminance(Color color, float delta) {
			ColorHSL colorHsl = (ColorHSL)color;
			float resultLuminance = colorHsl.Luminance + delta;
			colorHsl.Luminance = resultLuminance > 1f ? 1f : (resultLuminance < 0f ? 0 : resultLuminance);
			return (Color)colorHsl;
		}
		public abstract void ConfigureDrawOptions();
	}
	public class BarDrawOptionsConfigurator : ChartDrawOptionsConfigurator {
		readonly BarDrawOptions barDrawOptions;
		public BarDrawOptionsConfigurator(BarDrawOptions drawOptions) {
			this.barDrawOptions = drawOptions;
		}
		public override void ConfigureDrawOptions() {
			barDrawOptions.FillStyle.FillMode = FillMode.Hatch;
			HatchFillOptions fillOptions = barDrawOptions.FillStyle.Options as HatchFillOptions;
			if(fillOptions != null) {
				fillOptions.Color2 = barDrawOptions.Color;
				barDrawOptions.Color = barDrawOptions.ActualColor2;
				barDrawOptions.Color = ChangeLuminance(barDrawOptions.Color, -0.1f);
			}
		}
	}
	public class PointDrawOptionsBaseConfigurator : ChartDrawOptionsConfigurator {
		readonly PointDrawOptionsBase pointDrawOptionsBase;
		public PointDrawOptionsBaseConfigurator(PointDrawOptionsBase pointDrawOptionsBase) {
			this.pointDrawOptionsBase = pointDrawOptionsBase;
		}
		public override void ConfigureDrawOptions() {
			pointDrawOptionsBase.Color = ChangeLuminance(pointDrawOptionsBase.Color, -0.15f);
		}
	}
	public class PointDrawOptionsConfigurator : PointDrawOptionsBaseConfigurator {
		readonly PointDrawOptions pointDrawOptions;
		public PointDrawOptionsConfigurator(PointDrawOptions pointDrawOptions)
			: base(pointDrawOptions) {
			this.pointDrawOptions = pointDrawOptions;
		}
		public override void ConfigureDrawOptions() {
			base.ConfigureDrawOptions();
			pointDrawOptions.Marker.Size += 3;
		}
	}
	public class FinancialDrawOptionsConfigurator : ChartDrawOptionsConfigurator {
		readonly FinancialDrawOptions financialDrawOptions;
		public FinancialDrawOptionsConfigurator(FinancialDrawOptions financialDrawOptions) {
			this.financialDrawOptions = financialDrawOptions;
		}
		public override void ConfigureDrawOptions() {
			financialDrawOptions.LineThickness *= 2;
			financialDrawOptions.Color = ChangeLuminance(financialDrawOptions.Color, -0.1f);
		}
	}
	public class PieDrawOptionsConfigurator : ChartDrawOptionsConfigurator {
		readonly PieDrawOptions pieDrawOptions;
		public PieDrawOptionsConfigurator(PieDrawOptions pieDrawOptions) {
			this.pieDrawOptions = pieDrawOptions;
		}
		public override void ConfigureDrawOptions() {
			pieDrawOptions.FillStyle.FillMode = FillMode.Hatch;
			HatchFillOptions fillOptions = pieDrawOptions.FillStyle.Options as HatchFillOptions;
			if(fillOptions != null) {
				fillOptions.Color2 = pieDrawOptions.Color;
				pieDrawOptions.Color = pieDrawOptions.ActualColor2;
				pieDrawOptions.Color = ChangeLuminance(pieDrawOptions.Color, -0.1f);
			}
		}
	}
}
