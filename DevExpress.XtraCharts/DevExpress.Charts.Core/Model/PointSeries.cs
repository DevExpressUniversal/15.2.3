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
using DevExpress.Charts.Model.Native;
namespace DevExpress.Charts.Model {
	public class PointSeriesBase : ColorEachMarkerCartesianSeriesBase, ISupportColorEachSeries {
		public bool ColorEach {
			get { return Properties.GetSupport<ISupportColorEachSeries>().ColorEach; }
			set { Properties.GetSupport<ISupportColorEachSeries>().ColorEach = value; }
		}
	}
	public class BubbleSeries : PointSeriesBase, ISupportTransparencySeries {
		double minSize = 0.3;
		double maxSize = 0.9;
		public byte Transparency {
			get { return Properties.GetSupport<ISupportTransparencySeries>().Transparency; }
			set { Properties.GetSupport<ISupportTransparencySeries>().Transparency = value; }
		}
		public double MinSize {
			get { return minSize; }
			set {
				if (minSize != value) {
					if (value >= maxSize || value < 0)
						throw new ArgumentException("The minimum size should be greater than or equal to 0, and less than the maximum size.");
					minSize = value;
					NotifyParent(this, "MinSize", value);
				}
			}
		}
		public double MaxSize {
			get { return maxSize; }
			set {
				if (maxSize != value) {
					if (value <= minSize)
						throw new ArgumentException("The maximum size should be greater than the minimum size.");
					maxSize = value;
					NotifyParent(this, "MaxSize", value);
				}
			}
		}
		protected override SeriesPropertyBagBase CreatePropertyBag() {
			return new GenericPropertyBag3<MarkerBagSupport, ColorEachBagSupport, TransparencyBagSupport>(this);
		}
	}
	public class PointSeries : PointSeriesBase {
	}
	public class RadarPointSeries : RadarSeriesBase {
	}
	public class PolarPointSeries :  PolarSeriesBase {
	}
}
