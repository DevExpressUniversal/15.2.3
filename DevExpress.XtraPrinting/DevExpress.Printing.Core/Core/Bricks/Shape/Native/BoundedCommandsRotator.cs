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
namespace DevExpress.XtraPrinting.Shape.Native {
	public abstract class BoundedCommandsRotator {
		#region static
		public static void Rotate(ShapeCommandCollection commands, RectangleF bounds, int angleInDeg, bool stretch) {
			BoundedCommandsRotator rotator;
			if(stretch)
				rotator = new BoundedCommandsStretchRotator(commands, bounds, angleInDeg);
			else
				rotator = new BoundedCommandsUnstretchRotator(commands, bounds, angleInDeg);
			rotator.Rotate();
		}
		static float CalcExcess(float begin, float end, float max, float min) {
			float beginExcess = begin - min;
			float endExcess = max - end;
			float excess = Math.Max(Math.Abs(beginExcess), Math.Abs(endExcess));
			if(beginExcess > 0 || endExcess > 0)
				excess = -excess;
			return excess;
		}
		#endregion
		protected abstract float ZeroScaleFactor { get; }
		ShapeCommandCollection commands;
		int angleInDeg;
		RectangleF bounds;
		PointF center;
		protected BoundedCommandsRotator(ShapeCommandCollection commands, RectangleF bounds, int angleInDeg) {
			this.commands = commands;
			this.angleInDeg = angleInDeg;
			this.bounds = bounds;
			this.center = RectHelper.CenterOf(bounds);
		}
		protected abstract void CorrectScale(float scalingFactorX, float scalingFactorY);
		protected void ScaleAtCenter(float correctCoeffX, float correctCoeffY) {
			commands.ScaleAt(center, correctCoeffX, correctCoeffY);
		}
		void Rotate() {
			commands.RotateAt(center, angleInDeg);
			CorrectCenter();
			CriticalPointsCalculator calculator = commands.GetCriticalPointsCalculator();
			float scalingFactorX = CalcScaleFactor(bounds.Left, bounds.Right, calculator.MaxX, calculator.MinX);
			float scalingFactorY = CalcScaleFactor(bounds.Top, bounds.Bottom, calculator.MaxY, calculator.MinY);
			CorrectScale(scalingFactorX, scalingFactorY);
		}
		void CorrectCenter() {
			CriticalPointsCalculator calculator = commands.GetCriticalPointsCalculator();
			float dx = center.X - ((calculator.MaxX + calculator.MinX) / 2);
			float dy = center.Y - ((calculator.MaxY + calculator.MinY) / 2);
			commands.Offset(new PointF(dx, dy));
		}
		float CalcScaleFactor(float begin, float end, float max, float min) {
			if (max == min)
				return ZeroScaleFactor;
			float excess = CalcExcess(begin, end, max, min);
			float sizeHalf = (end - begin) / 2;
			if(Math.Abs(sizeHalf - excess) < 0.0001)
				return ZeroScaleFactor;
			return sizeHalf / (sizeHalf - excess);
		}
	}
	public class BoundedCommandsUnstretchRotator : BoundedCommandsRotator {
		protected override float ZeroScaleFactor { get { return float.MaxValue; } }
		public BoundedCommandsUnstretchRotator(ShapeCommandCollection commands, RectangleF bounds, int angleInDeg)
			: base(commands, bounds, angleInDeg) {
		}
		protected override void CorrectScale(float scalingFactorX, float scalingFactorY) {
			float scalingFactor = Math.Min(scalingFactorX, scalingFactorY);
			ScaleAtCenter(scalingFactor, scalingFactor);
		}
	}
	public class BoundedCommandsStretchRotator : BoundedCommandsRotator {
		protected override float ZeroScaleFactor { get { return 1; } }
		public BoundedCommandsStretchRotator(ShapeCommandCollection commands, RectangleF bounds, int angleInDeg)
			: base(commands, bounds, angleInDeg) {
		}
		protected override void CorrectScale(float scalingFactorX, float scalingFactorY) {
			ScaleAtCenter(scalingFactorX, scalingFactorY);
		}
	}
}
