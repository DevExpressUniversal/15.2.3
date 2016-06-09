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

using DevExpress.Diagram.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Diagram.Core {
	public class MeasureUnit {
		public readonly double Dpi;
		public readonly int Multiplier;
		public readonly string Name;
		public readonly TickStepsData[] StepsData;
		public MeasureUnit(double dpi, int multiplier, string name, TickStepsData[] stepsData) {
			this.Dpi = dpi;
			this.Multiplier = multiplier;
			this.Name = name;
			this.StepsData = stepsData;
		}
		public override string ToString() {
			return Name;
		}
	}
	public struct TickStepsData {
		public double ZoomLevel;
		public double SmallStep;
		public int MediumStep;
		public int LargeStep;
		public TickStepsData(double zoomLevel, double smallStep, int mediumStep, int largeStep) {
			ZoomLevel = zoomLevel;
			SmallStep = smallStep;
			MediumStep = mediumStep;
			LargeStep = largeStep;
		}
	}
	public static class MeasureUnits {
		static TickStepsData[] LargeStepsTable = new TickStepsData[] {
			new TickStepsData(0.01, 1000.0, 1, 4),
			new TickStepsData(0.02, 500.0, 1, 4),
			new TickStepsData(0.04, 500.0, 1, 2),
			new TickStepsData(0.07, 250.0, 1, 4),
			new TickStepsData(0.1, 100.0, 1, 5),
			new TickStepsData(0.15, 100.0, 1, 2),
			new TickStepsData(0.2, 50.0, 1, 5),
			new TickStepsData(0.25, 20.0, 5, 10),
			new TickStepsData(0.5, 10.0, 2, 10),
			new TickStepsData(0.75, 8.0, 2, 10),
			new TickStepsData(1.0, 5.0, 2, 10),
			new TickStepsData(2.0, 2.0, 5, 10),
			new TickStepsData(4.0, 2.5, 2, 4),
			new TickStepsData(6.0, 1.0, 5, 10),
			new TickStepsData(10.0, 0.5, 2, 10),
			new TickStepsData(20.0, 0.5, 2, 4),
			new TickStepsData(28.0, 0.2, 5, 10)
		};
		static TickStepsData[] SmallStepsTable = new TickStepsData[] {
			new TickStepsData(0.01, 1000.0, 5, 10),
			new TickStepsData(0.02, 500.0, 5, 10),
			new TickStepsData(0.04, 200.0, 5, 10),
			new TickStepsData(0.07, 200.0, 2, 10),
			new TickStepsData(0.1, 100.0, 5, 10),
			new TickStepsData(0.15, 100.0, 2, 10),
			new TickStepsData(0.2, 50.0, 5, 10),
			new TickStepsData(0.25, 50.0, 2, 10),
			new TickStepsData(0.4, 20.0, 10, 10),
			new TickStepsData(0.5, 20.0, 5, 10),
			new TickStepsData(0.75, 20.0, 2, 10),
			new TickStepsData(1.0, 10.0, 5, 10),
			new TickStepsData(2.0, 5.0, 5, 10),
			new TickStepsData(4.0, 4.0, 5, 5),
			new TickStepsData(6.0, 4.0, 1, 5),
			new TickStepsData(10.0, 2.0, 1, 5),
			new TickStepsData(20.0, 1.0, 2, 10),
			new TickStepsData(28.0, 1.0, 1, 10)
		};
		public static readonly MeasureUnit Pixels = new MeasureUnit(DevExpress.XtraPrinting.GraphicsDpi.Pixel, 1, DiagramControlLocalizer.GetString(DiagramControlStringId.MeasureUnit_Pixels), LargeStepsTable);
		public static readonly MeasureUnit Millimeters = new MeasureUnit(DevExpress.XtraPrinting.GraphicsDpi.TenthsOfAMillimeter, 10, DiagramControlLocalizer.GetString(DiagramControlStringId.MeasureUnit_Millimeters), SmallStepsTable);
		public static readonly MeasureUnit Inches = new MeasureUnit(DevExpress.XtraPrinting.GraphicsDpi.HundredthsOfAnInch, 100, DiagramControlLocalizer.GetString(DiagramControlStringId.MeasureUnit_Inches), LargeStepsTable);
		public static double ToPixels(this MeasureUnit unit, double value) {
			return value * Pixels.Dpi / unit.Dpi;
		}
		public static double FromPixels(this MeasureUnit unit, double value) {
			return value * unit.Dpi / Pixels.Dpi;
		}
	}
}
