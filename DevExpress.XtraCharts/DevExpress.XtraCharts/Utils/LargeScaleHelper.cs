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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public static class LargeScaleHelper {
		const int mistakeCriterion = 0x7FFFF8;
		static bool IsValid(double value) {
			return value <= mistakeCriterion && value >= -mistakeCriterion;
		}
		static bool IsValid(GRealPoint2D point) {
			return IsValid(point.X) && IsValid(point.Y);
		}
		static double Validate(double value) {
			return value > mistakeCriterion ? mistakeCriterion : value < -mistakeCriterion ? -mistakeCriterion : value;
		}
		static GRealPoint2D Validate(GRealPoint2D point) {
			if (IsValid(point.X))
				return IsValid(point.Y) ? point : new GRealPoint2D(point.X, Validate(point.Y));
			if (IsValid(point.Y))
				return new GRealPoint2D(Validate(point.X), point.Y);
			return Math.Abs(point.X) > Math.Abs(point.Y) ?
				new GRealPoint2D(Validate(point.X), point.Y * Math.Abs(mistakeCriterion / point.X)) :
				new GRealPoint2D(point.X * Math.Abs(mistakeCriterion / point.Y), Validate(point.Y));
		}
		public static bool IsValid(LineStrip strip) {
			foreach (GRealPoint2D point in strip)
				if (!IsValid(point))
					return false;
			return true;
		}
		public static bool IsValid(List<IGeometryStrip> strips) {
			foreach (LineStrip strip in strips)
				if (!IsValid(strip))
					return false;
			return true;
		}
		public static void Validate(List<GRealPoint2D> points) {
			for (int i = 0; i < points.Count; i++)
				if (!IsValid(points[i]))
					points[i] = Validate(points[i]);
		}
		public static ZPlaneRectangle Validate(ZPlaneRectangle rect) {
			return (rect.Width <= mistakeCriterion && rect.Height <= mistakeCriterion) ? rect :
				new ZPlaneRectangle(rect.Location, Validate(rect.Width), Validate(rect.Height));
		}
		public static DiagramPoint Validate(DiagramPoint point) {
			if (IsValid(point.X))
				return IsValid(point.Y) ? point : new DiagramPoint(point.X, Validate(point.Y));
			if (IsValid(point.Y))
				return new DiagramPoint(Validate(point.X), point.Y);
			return Math.Abs(point.X) > Math.Abs(point.Y) ?
				new DiagramPoint(Validate(point.X), point.Y * Math.Abs(mistakeCriterion / point.X)) :
				new DiagramPoint(point.X * Math.Abs(mistakeCriterion / point.Y), Validate(point.Y));
		}
	}
}
