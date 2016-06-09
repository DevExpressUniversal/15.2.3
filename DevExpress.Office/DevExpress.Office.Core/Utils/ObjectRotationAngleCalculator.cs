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
using DevExpress.Utils.KeyboardHandler;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.Office.Utils {
	#region ObjectRotationAngleCalculator
	public static class ObjectRotationAngleCalculator {
		public static float CalculateAngle(Point point, Rectangle objectBounds, float initialRotationAngle) {
			return CalculateAngle(point, objectBounds, initialRotationAngle, null);
		}
		public static float CalculateAngle(Point point, Rectangle objectBounds, float initialRotationAngle, Matrix transformMatrix) {
			if (transformMatrix != null)
				point = transformMatrix.TransformPoint(point);
			Point center = RectangleUtils.CenterPoint(objectBounds);
			point.X -= center.X;
			point.Y -= center.Y;
			if (point.X == 0 && point.Y == 0)
				return initialRotationAngle;
			float deltaAngle = (float)(180 * Math.Atan(point.X / (float)point.Y) / Math.PI);
			if (point.Y > 0)
				deltaAngle += 180;
			if (Math.Abs(deltaAngle) == 90)
				deltaAngle = -deltaAngle;
			return SnapAngle(initialRotationAngle - deltaAngle);
		}
		static float SnapAngle(float angle) {
			if ((KeyboardHandler.IsShiftPressed))
				return SnapAngle(angle, 15, 7.5f);
			else
				return SnapAngle(angle, 90, 3);
		}
		static float SnapAngle(float angle, int step, float delta) {
			angle = NormalizeAngle(angle);
			for (int i = 0; i <= 360; i += step)
				angle = CalculateSnap(angle, i, delta);
			return NormalizeAngle(angle);
		}
		static float NormalizeAngle(float angle) {
			angle %= 360f;
			if (angle < 0)
				angle += 360;
			return angle;
		}
		static float CalculateSnap(float angle, float baseAngle, float delta) {
			if (baseAngle - delta <= angle && angle < baseAngle + delta)
				return baseAngle;
			else
				return angle;
		}
	}
	#endregion
}
