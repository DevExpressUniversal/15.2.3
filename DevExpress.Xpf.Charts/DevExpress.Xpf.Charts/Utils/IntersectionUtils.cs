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
using System.Windows.Controls;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public static class IntersectionUtils {
		const int alphaLimit = 10;
		static bool isCopyPixelsSupported = true;
		public static GRealPoint2D CalcIntersectionPointWithCustomShape(UIElement presentation, GRealPoint2D anchorPoint, GRealPoint2D labelCenter, GRect2D rect) {
			if (isCopyPixelsSupported) {
				try {
					CustomShape shape = new CustomShape(presentation);
					GRealPoint2D? intersectionPoint = IntersectionUtils.CalcLineWithShapeIntersection(anchorPoint, labelCenter, new Rect(rect.Left, rect.Top, rect.Width, rect.Height), shape);
					if (intersectionPoint.HasValue)
						return intersectionPoint.Value;
				}
				catch {
					isCopyPixelsSupported = false;
					return labelCenter;
				}
			}
			return labelCenter;
		}
		public static GRealPoint2D? CalcLineWithShapeIntersection(GRealPoint2D startPoint, GRealPoint2D endPoint, Rect rect, CustomShape shape) {
			if (shape.IsEmpty)
				return null;
			GRealPoint2D prevPoint = startPoint;
			DDAAlgorithm algorithm = new DDAAlgorithm(prevPoint, endPoint);
			bool toLeft = endPoint.X < startPoint.X;
			while (algorithm.NextPoint()) {
				GRealPoint2D currentPoint = new GRealPoint2D(MathUtils.StrongRound(algorithm.CurrentPoint.X), MathUtils.StrongRound(algorithm.CurrentPoint.Y));
				if (rect.Contains(new Point(currentPoint.X, currentPoint.Y))) {
					currentPoint = new GRealPoint2D(toLeft ? currentPoint.X - rect.Left - 1 : currentPoint.X - rect.Left, currentPoint.Y - rect.Top);
					byte? alpha = shape.GetAlpha((int)currentPoint.X, (int)currentPoint.Y);
					if (alpha != null && (byte)alpha > alphaLimit)
						return prevPoint;
				}
				prevPoint = algorithm.CurrentPoint;
			}
			return null;
		}
	}
}
