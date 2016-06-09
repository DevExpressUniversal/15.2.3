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

using DevExpress.Charts.Model.Native;
using System;
namespace DevExpress.Charts.Model {
	public abstract class AreaSeriesBase : ColorEachMarkerCartesianSeriesBase, ISupportTransparencySeries {
		public byte Transparency {
			get {  return Properties.GetSupport<ISupportTransparencySeries>().Transparency; }
			set { Properties.GetSupport<ISupportTransparencySeries>().Transparency = value; }
		}
		protected override SeriesPropertyBagBase CreatePropertyBag() {
			return new GenericPropertyBag3<MarkerBagSupport, ColorEachBagSupport, TransparencyBagSupport>(this);
		}
	}
	public class AreaSeries : AreaSeriesBase, ISupportColorEachSeries {
		public bool ColorEach {
			get { return Properties.GetSupport<ISupportColorEachSeries>().ColorEach; }
			set { Properties.GetSupport<ISupportColorEachSeries>().ColorEach = value; }
		}
	}
	public class SplineAreaSeries : AreaSeries {
	}
	public class RangeAreaSeries : AreaSeries {
	}
	public class StepAreaSeries : AreaSeries {
	}
	public class StackedAreaSeries : AreaSeriesBase {
	}
	public class StackedSplineAreaSeries : StackedAreaSeries {
	}
	public class FullStackedAreaSeries : AreaSeriesBase {
	}
	public class FullStackedSplineAreaSeries : AreaSeriesBase {
	}
	public class RadarAreaSeries : RadarLineSeries, ISupportTransparencySeries {
		public byte Transparency { 
			get { return Properties.GetSupport<ISupportTransparencySeries>().Transparency; }
			set { Properties.GetSupport<ISupportTransparencySeries>().Transparency = value; } 
		}
		protected override SeriesPropertyBagBase CreatePropertyBag() {
			return new GenericPropertyBag3<MarkerBagSupport, ColorEachBagSupport, TransparencyBagSupport>(this);
		}
	}
	public class PolarAreaSeries : PolarLineSeries, ISupportTransparencySeries {
		public byte Transparency {
			get { return Properties.GetSupport<ISupportTransparencySeries>().Transparency; }
			set { Properties.GetSupport<ISupportTransparencySeries>().Transparency = value; }
		}
		protected override SeriesPropertyBagBase CreatePropertyBag() {
			return new GenericPropertyBag3<MarkerBagSupport, ColorEachBagSupport, TransparencyBagSupport>(this);
		}
	}
}
