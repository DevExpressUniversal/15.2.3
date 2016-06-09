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

using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class NestedDonutSeries2DPointLayout : PieSeries2DPointLayoutBase {
		Rect bounds;
		DonutSegment finalDonutSegment;
		DonutSegment animatedDonutSegment;
		double finalOuterRadiusOfOuterDonut;
		public override Rect Bounds {
			get { return bounds; }
		}
		public double FinalInnerRadius {
			get { return finalDonutSegment.InnerRadius; }
		}
		public double FinalOuterRadius {
			get { return finalDonutSegment.OuterRadius; }
		}
		public double FinalMedianAngleDeg {
			get { return finalDonutSegment.MedianAngleDeg; }
		}
		public double ArcAngleDeg {
			get { return finalDonutSegment.ArcAngleDeg; }
		}
		public Point FinalCenter { 
			get { return finalDonutSegment.Center; } 
		}
		public Point FinalRelativeCenter {
			get { return finalDonutSegment.RelativeCenter; }
		}
		public double FinalOuterRadiusOfOuterDonut { 
			get {return finalOuterRadiusOfOuterDonut; }
		}
		public override Geometry ClipGeometry {
			get {
				return CalculateClipGeometry(animatedDonutSegment);
			}
		}
		public NestedDonutSeries2DPointLayout(Rect viewport, DonutSegment donutSegment, double outerDonutRadius)
			: base(viewport) {
			Point boundsLocation = donutSegment.Center.MoveByVector(-outerDonutRadius, -outerDonutRadius);
			Size boundsSize = new Size(2 * outerDonutRadius, 2 * outerDonutRadius);
			this.bounds = new Rect(boundsLocation, boundsSize);
			this.finalOuterRadiusOfOuterDonut = outerDonutRadius;
			this.finalDonutSegment = donutSegment;
			this.animatedDonutSegment = donutSegment;
		}
		public void ChangeDuringPointAnimation(DonutSegment segment, double animatedOuterRadiusOfOuterDonut) {
			this.animatedDonutSegment = segment;
			Point boundsLocation = animatedDonutSegment.Center.MoveByVector(-animatedOuterRadiusOfOuterDonut, -animatedOuterRadiusOfOuterDonut);
			Size boundsSize = new Size(2 * animatedOuterRadiusOfOuterDonut, 2 * animatedOuterRadiusOfOuterDonut);
			this.bounds = new Rect(boundsLocation, boundsSize);
		}
	}
}
