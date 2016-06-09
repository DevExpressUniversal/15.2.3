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

using System.Collections.Generic;
using System;
using System.Drawing;
namespace DevExpress.XtraCharts.Native {
	public class Pyramid : Cone {
		const int pyramidSegmentsCount = 4;
		protected override bool IsSmoothing { get { return false; } }
		public Pyramid(YPlaneRectangle bottomPyramidBase, YPlaneRectangle topPyramidBase) : base(bottomPyramidBase, topPyramidBase, pyramidSegmentsCount) {
		}
		public Pyramid(YPlaneRectangle pyramidBase, double pyramidHeight, double beginHeight, double endHeight)
			: base(pyramidBase, pyramidHeight, beginHeight, endHeight, pyramidSegmentsCount) {
		}
		protected override PlanePolygon CalcBase(YPlaneRectangle rectBase, int segmentsCount) {
			DiagramPoint[] points = new DiagramPoint[6];
			points[5] = rectBase.LeftBottom;
			points[4] = rectBase.RightBottom;
			points[3] = new DiagramPoint(rectBase.RightCenter.X, rectBase.RightBottom.Y, rectBase.RightBottom.Z);
			points[2] = rectBase.RightTop;
			points[1] = rectBase.LeftTop;
			points[0] = new DiagramPoint(rectBase.LeftCenter.X, rectBase.LeftTop.Y, rectBase.LeftTop.Z);
			PlanePolygon result = new PlanePolygon(points);
			result.SameNormals = true;
			result.Normal = MathUtils.CalcNormal(result);
			return result;
		}
	}
}
