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
namespace DevExpress.Xpf.Gauges {
	public abstract class LinearScaleIndicator : ValueIndicatorBase {
		new protected LinearGaugeControl Gauge { get { return base.Gauge as LinearGaugeControl; } }
		internal new LinearScale Scale { get { return base.Scale as LinearScale; } }
		protected double GetRotationAngleByScaleLayoutMode() {
			double angle = 0.0;
			switch (Scale.LayoutMode) {
				case LinearScaleLayoutMode.TopToBottom: angle = 180.0;
					break;
				case LinearScaleLayoutMode.LeftToRight: angle = 90.0;
					break;
				case LinearScaleLayoutMode.RightToLeft: angle = -90.0;
					break;
			}
			return angle;
		}
		protected double GetPointYByScaleLayoutMode(double value) {
			double pointCoordinate = 0.0;
			if (Scale.LayoutMode == LinearScaleLayoutMode.TopToBottom || Scale.LayoutMode == LinearScaleLayoutMode.BottomToTop) {
				double maxPoint = Math.Abs(Scale.Mapping.Layout.ScaleVector.Y);
				pointCoordinate = Scale.Mapping.Layout.AnchorPoint.Y + Math.Sign(Scale.Mapping.Layout.ScaleVector.Y) * (maxPoint - Scale.Mapping.GetPointByValue(value).Y);
				if (pointCoordinate > maxPoint)
					pointCoordinate = maxPoint;
			}
			else {
				double maxPoint = Math.Abs(Scale.Mapping.Layout.ScaleVector.X);
				pointCoordinate = Scale.Mapping.Layout.AnchorPoint.X + Math.Sign(Scale.Mapping.Layout.ScaleVector.X) * (maxPoint - Scale.Mapping.GetPointByValue(value).X);
				if (pointCoordinate > maxPoint)
					pointCoordinate = maxPoint;
			}
			if (pointCoordinate < 0)
				pointCoordinate = 0;
			return pointCoordinate;
		}
	}
	public class LinearScaleIndicatorCollection<T> : ValueIndicatorCollection<T> where T : LinearScaleIndicator{
		public LinearScaleIndicatorCollection(LinearScale scale) : base(scale) {
		}
	}	   
}
