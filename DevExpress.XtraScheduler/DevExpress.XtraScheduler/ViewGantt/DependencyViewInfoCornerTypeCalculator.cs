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

using DevExpress.XtraScheduler.Internal.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraScheduler.Drawing {
	#region DependencyCornerViewInfo
	public class DependencyCornerViewInfo {
		public DependencyCornerViewInfo() {
			AdjacentDependencyViewInfos = new DependencyViewInfoCollection();
		}
		public DependencyCornerType Type { get; set; }
		public Point ConnectionPoint { get; set; }
		public DependencyViewInfoCollection AdjacentDependencyViewInfos { get; set; }
	}
	#endregion
	#region DependencyViewInfoCornerTypeCalculator
	public static class DependencyViewInfoCornerTypeCalculator {
		public static DependencyCornerType GetCornerType(DependencyViewInfo current, DependencyCornerViewInfo corner) {
			IList<DependencyViewInfo> adjacentDependencies = corner.AdjacentDependencyViewInfos;
			Point connectionPoint = corner.ConnectionPoint;
			return GetCornerType(current, adjacentDependencies, connectionPoint);
		}
		internal static DependencyCornerType GetCornerType(DependencyViewInfo current, IList<DependencyViewInfo> adjacentDependencies, Point connectionPoint) {
			if (adjacentDependencies.Count == 1 
				&& IsStraightAngle(current, adjacentDependencies[0])) {
				DependencyViewInfo horizontalViewInfo = GetHorizontalViewInfo(current, adjacentDependencies[0]);
				DependencyViewInfo verticalViewInfo = GetVerticalViewInfo(current, adjacentDependencies[0]);
				if (IsPointNotBelongsDependency(horizontalViewInfo, connectionPoint) || IsPointNotBelongsDependency(verticalViewInfo, connectionPoint))
					throw new ArgumentException("connectionPoint: invalid");
				return GetCornerTypeCore(verticalViewInfo, horizontalViewInfo, connectionPoint);
			}
			return DependencyCornerType.None;
		}
		static bool IsPointNotBelongsDependency(DependencyViewInfo dependencyViewInfo, Point connectionPoint) {
			return dependencyViewInfo.Start != connectionPoint && dependencyViewInfo.End != connectionPoint; 
		}
		static bool IsStraightAngle(DependencyViewInfo current, DependencyViewInfo adjacentDependency) {
			return current.GetDirection() != adjacentDependency.GetDirection();
		}
		static DependencyViewInfo GetVerticalViewInfo(DependencyViewInfo first, DependencyViewInfo second) {
			return first.Direction == DependencyDirection.Vertical ? first : second;
		}
		static DependencyViewInfo GetHorizontalViewInfo(DependencyViewInfo first , DependencyViewInfo second) {
			return first.Direction == DependencyDirection.Horizontal ? first : second;
		}
		static DependencyCornerType GetCornerTypeCore(DependencyViewInfo vertical, DependencyViewInfo horizontal, Point connectionPoint) {
			Point hVector = GetDependencyVector(horizontal, connectionPoint);
			Point vVector = GetDependencyVector(vertical, connectionPoint);
			if (hVector.X * vVector.Y < 0)
				return vVector.Y < 0 ? DependencyCornerType.BottomLeft : DependencyCornerType.TopRight;
			return hVector.X < 0 ? DependencyCornerType.BottomRight : DependencyCornerType.TopLeft;
		}	
		static Point GetDependencyVector(DependencyViewInfo dependency, Point connectionPoint) {
			XtraSchedulerDebug.Assert(connectionPoint == dependency.Start || connectionPoint == dependency.End);
			int x = dependency.End.X - dependency.Start.X;
			int y = dependency.End.Y - dependency.Start.Y;
			return dependency.Start == connectionPoint ? new Point(x, y) : new Point(-x, -y);
		}	   
	}
	#endregion
}
