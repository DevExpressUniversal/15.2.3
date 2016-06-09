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

using DevExpress.XtraPrinting.Native;
using System;
using System.Drawing;
using DevExpress.XtraPrinting.Shape.Native;
using System.ComponentModel;
namespace DevExpress.XtraPrinting.Shape {
	public abstract class ShapePolygonBase : FilletShapeBase {
		int numberOfSides = 3;
		protected double AngleStep {
			get { return 2 * Math.PI / numberOfSides; }
		}
		protected internal int NumberOfSides {
			get { return numberOfSides; }
			set { numberOfSides = ShapeHelper.ValidateRestrictedValue(value, 3, int.MaxValue, "NumberOfSide"); }
		}
		protected ShapePolygonBase() {
		}
		protected ShapePolygonBase(ShapePolygonBase source)
			: base(source) {
			numberOfSides = source.NumberOfSides;
		}
		protected PointF[] CreatePointsCore(RectangleF bounds, double startAngle, float radius) {
			double currentAngle = startAngle;
			PointF[] vertexes = new PointF[numberOfSides];
			PointF center = RectHelper.CenterOf(bounds);
			for(int i = 0; i < numberOfSides; i++) {
				vertexes[i] = new PointF(center.X + radius * (float)Math.Sin(currentAngle), center.Y - radius * (float)Math.Cos(currentAngle));
				currentAngle += AngleStep;
			}
			return vertexes;
		}
	}
}
