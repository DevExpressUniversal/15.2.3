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
	public abstract class BarSeries : ColorEachCartesianSeriesBase, ISupportBarWidthSeries, ISupportBar3DModelSeries, ISupportTransparencySeries {
		public double BarWidth {
			get { return Properties.GetSupport<ISupportBarWidthSeries>().BarWidth; }
			set { Properties.GetSupport<ISupportBarWidthSeries>().BarWidth = value; }
		}
		public Bar3DModel Model {
			get { return Properties.GetSupport<ISupportBar3DModelSeries>().Model; }
			set { Properties.GetSupport<ISupportBar3DModelSeries>().Model = value; }
		}
		public byte Transparency {
			get { return Properties.GetSupport<ISupportTransparencySeries>().Transparency; }
			set { Properties.GetSupport<ISupportTransparencySeries>().Transparency = value; }
		}
		protected override SeriesPropertyBagBase CreatePropertyBag() {
			return new GenericPropertyBag4<ColorEachBagSupport, BarWidthBagSupport, Bar3DModelBagSupport, TransparencyBagSupport>(this);
		}
	}
	public class SideBySideBarSeries : BarSeries {
	}
	public class StackedBarSeries : BarSeries {
	}
	public class FullStackedBarSeries : StackedBarSeries {
	}
	public class SideBySideStackedBarSeries : StackedBarSeries {
	}
	public class SideBySideFullStackedBarSeries : SideBySideStackedBarSeries {
	}
	public class RangeBarSeries : BarSeries {
	}
	public class SideBySideRangeBarSeries : RangeBarSeries {
	}
	public class ManhattanBarSeries : SeriesModel, ISupportColorEachSeries, ISupportBarWidthSeries, ISupportBar3DModelSeries {
		public double BarWidth {
			get { return Properties.GetSupport<ISupportBarWidthSeries>().BarWidth; }
			set { Properties.GetSupport<ISupportBarWidthSeries>().BarWidth = value; }
		}
		public bool ColorEach {
			get { return Properties.GetSupport<ISupportColorEachSeries>().ColorEach; }
			set { Properties.GetSupport<ISupportColorEachSeries>().ColorEach = value; }
		}
		public Bar3DModel Model {
			get { return Properties.GetSupport<ISupportBar3DModelSeries>().Model; }
			set { Properties.GetSupport<ISupportBar3DModelSeries>().Model = value; }
		}
		protected override SeriesPropertyBagBase CreatePropertyBag() {
			return new GenericPropertyBag3<ColorEachBagSupport, BarWidthBagSupport, Bar3DModelBagSupport>(this);
		}
	}
}
