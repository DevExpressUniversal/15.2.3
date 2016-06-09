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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Utils;
namespace DevExpress.Diagram.Core {
	public sealed class ZoomHelper {
		readonly int stepDelta;
		readonly int zoom10xStepsCount;
		readonly List<double> zoomFactors;
		readonly int factorSignificantDigitsCount;
		public ZoomHelper(int stepDelta = 12, int zoom10xStepsCount = 500) {
			if(stepDelta < 0)
				throw new ArgumentException("", "stepDelta");
			if(zoom10xStepsCount < 2)
				throw new ArgumentException("", "zoom10xStepsCount");
			this.stepDelta = stepDelta;
			this.zoom10xStepsCount = zoom10xStepsCount;
			this.factorSignificantDigitsCount = -(int)Math.Floor(Math.Log10((10.0 - 1.0) / (this.stepDelta * this.zoom10xStepsCount))) + 1;
			zoomFactors = new List<double>();
			for(int i = 0; i < this.zoom10xStepsCount; ++i)
				zoomFactors.Add(Math.Round(Math.Pow(10.0, (double)i / this.zoom10xStepsCount), factorSignificantDigitsCount));
			zoomFactors.Add(10.0);
		}
		public double ModZoomFactor(double zoomFactor, int delta) {
			int normalizedZoomFactor = NormalizeZoomFactor(zoomFactor);
			normalizedZoomFactor += delta;
			return DenormalizeZoomFactor(normalizedZoomFactor);
		}
		internal int NormalizeZoomFactor(double zoomFactor) {
			int zoomFactorOrder = (int)Math.Floor(Math.Log10(zoomFactor));
			double zoomFactorNormal = Math.Round(zoomFactor / Math.Pow(10.0, zoomFactorOrder), factorSignificantDigitsCount);
			int zoomFactorIndex = zoomFactors.BinarySearch(zoomFactorNormal);
			if(zoomFactorIndex < 0)
				zoomFactorIndex = ~zoomFactorIndex - 1;
			int zoomFactorInterpolate = (int)Math.Round(stepDelta * (zoomFactorNormal - zoomFactors[zoomFactorIndex]) / (zoomFactors[zoomFactorIndex + 1] - zoomFactors[zoomFactorIndex]));
			return stepDelta * (zoomFactorOrder * zoom10xStepsCount + zoomFactorIndex) + zoomFactorInterpolate;
		}
		internal double DenormalizeZoomFactor(int zoomFactor) {
			int zoomFactorInt = zoomFactor / stepDelta;
			int zoomFactorIndex = zoomFactorInt < 0 ? (zoom10xStepsCount - ((-zoomFactorInt) % zoom10xStepsCount)) % zoom10xStepsCount : zoomFactorInt % zoom10xStepsCount;
			int zoomFactorPower = (zoomFactorInt - zoomFactorIndex) / zoom10xStepsCount;
			double zoomFactorInterpolate = (zoomFactor - stepDelta * zoomFactorInt);
			return Math.Pow(10.0, zoomFactorPower) * Math.Round((zoomFactors[zoomFactorIndex] * stepDelta + zoomFactorInterpolate * (zoomFactors[zoomFactorIndex + 1] - zoomFactors[zoomFactorIndex])) / stepDelta, factorSignificantDigitsCount);
		}
	}
}
