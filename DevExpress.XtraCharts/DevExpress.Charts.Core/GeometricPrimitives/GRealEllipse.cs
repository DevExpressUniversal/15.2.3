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
namespace DevExpress.Charts.Native {
	public class GRealEllipse {
		readonly GRealPoint2D center;
		readonly double radiusX;
		readonly double radiusY;
		public GRealPoint2D Center { get { return center; } }
		public double RadiusX { get { return radiusX; } }
		public double RadiusY { get { return radiusY; } }
		public GRealEllipse(GRealPoint2D leftTop, GRealPoint2D rightBottom)
			: this(new GRealRect2D(leftTop, rightBottom)) {
		}
		public GRealEllipse(GRealRect2D bounds)
			: this(bounds.Center, bounds.Width, bounds.Height) {
		}
		public GRealEllipse(GRealPoint2D center, double radiusX, double radiusY) {
			this.center = center;
			this.radiusX = radiusX;
			this.radiusY = radiusY;
		}
		public GRealEllipse Inflate(double dx, double dy) {
			double newRadiusX = Math.Max(0.0, this.radiusX + dx);
			double newRadiusY = Math.Max(0.0, this.radiusY + dy);
			return new GRealEllipse(center, newRadiusX, newRadiusY);
		}
	}
}
