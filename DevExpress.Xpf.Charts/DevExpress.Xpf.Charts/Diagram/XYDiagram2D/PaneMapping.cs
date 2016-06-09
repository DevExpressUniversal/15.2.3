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
using System.Windows;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class PaneMapping : IMapping {
		readonly Pane pane;
		readonly IAxisMapping axisXMapping;
		readonly IAxisMapping axisYMapping;
		public bool Rotated { get { return pane.Rotated; } }
		public bool NavigationEnabled { get { return pane.NavigationEnabled; } }
		public Rect Viewport { get { return pane.Viewport; } }
		public IAxisMapping AxisXMapping { get { return axisXMapping; } }
		public IAxisMapping AxisYMapping { get { return axisYMapping; } }
		public PaneMapping(Pane pane, IXYSeriesView view) : this(pane, ((AxisBase)view.AxisXData), ((AxisBase)view.AxisYData)) {
		}
		public PaneMapping(Pane pane, AxisBase axisX, AxisBase axisY) {
			this.pane = pane;
			Rect viewport = pane.Viewport;
			axisXMapping = axisX.CreateMapping(viewport);
			axisYMapping = axisY.CreateMapping(viewport);
		}
		public bool IsLabelVisibleForResolveOverlapping(Point initialAnchorPoint) {
			if (!NavigationEnabled) {
				Point roundedInitialPoint = new Point(MathUtils.StrongRound(initialAnchorPoint.X), MathUtils.StrongRound(initialAnchorPoint.Y));
				return Viewport.Contains(roundedInitialPoint);
			}
			return true;
		}
		public Point GetDiagramPoint(double argument, double value) {
			return new Point(axisXMapping.GetAxisValue(argument), axisYMapping.GetAxisValue(value));
		}
		public Point GetDiagramPoint(GRealPoint2D point) {
			return new Point(axisXMapping.GetAxisValue(point.X), axisYMapping.GetAxisValue(point.Y));
		}
		public Point GetRoundedDiagramPoint(double argument, double value) {
			return new Point(axisXMapping.GetRoundedAxisValue(argument), axisYMapping.GetRoundedAxisValue(value));
		}
	}	
}
