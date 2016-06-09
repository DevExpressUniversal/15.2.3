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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.Office.Drawing {
	public static class SnapToDevicePixelsHelper {
		public static RectangleF GetCorrectedBounds(Graphics graphics, Size sizeInPixel, RectangleF bounds) {
			PointF[] points = new PointF[] { new PointF(bounds.Left, bounds.Top), new PointF(bounds.Right, bounds.Bottom) };
			graphics.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, points);
			int screenWidth = (int)(points[1].X - points[0].X);
			int screenHeight = (int)(points[1].Y - points[0].Y);
			if (Math.Abs(sizeInPixel.Width - screenWidth) <= 5 && Math.Abs(sizeInPixel.Height - screenHeight) <= 5) {
				points[0].X = (int)(points[0].X + 0.5);
				points[0].Y = (int)(points[0].Y + 0.5);
				points[1].X = points[0].X + sizeInPixel.Width;
				points[1].Y = points[0].Y + sizeInPixel.Height;
				graphics.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, points);
				float width = points[1].X - points[0].X;
				float height = points[1].Y - points[0].Y;
				return new RectangleF(points[0].X, points[0].Y, width, height);
			} else
				return bounds;
		}
	}
}
