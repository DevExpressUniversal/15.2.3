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
using System.Drawing;
using System.ComponentModel;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[RuntimeObject]
	public abstract class DrawOptions : ICloneable {
		Color color;
		Color color2 = Color.Empty;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DrawOptionsActualColor2")]
#endif
		public Color ActualColor2 {
			get { return color2; }
			internal set { color2 = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DrawOptionsColor")]
#endif
		public Color Color { get { return color; } set { color = value; } }
		protected DrawOptions() {
		}
		protected DrawOptions(SeriesViewBase view) {
			color = view.ActualColor;
			color2 = view.ActualColor2;
		}
		protected abstract DrawOptions CreateInstance();
		protected virtual void DeepClone(object obj) {
			DrawOptions drawOptions = obj as DrawOptions;
			if(drawOptions != null) {
				color = drawOptions.color;
				color2 = drawOptions.color2;
			}
		}
		protected internal virtual void InitializeSeriesPointDrawOptions(SeriesViewBase view, IRefinedSeries refinedSeries, int pointIndex) {
			if (view.ActualColorEach) {
				int pointsCount = refinedSeries.Points.Count;
				color = view.GetPointColor(pointIndex, pointsCount);
				color2 = view.GetPointColor2(pointIndex, pointsCount);
			}
		}
		public object Clone() {
			DrawOptions drawOptions = CreateInstance();
			drawOptions.DeepClone(this);
			return drawOptions;
		}
	}
}
