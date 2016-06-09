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
namespace DevExpress.XtraCharts.Native {
	public class Axis3DMapping {
		readonly XYDiagram3DCoordsCalculator coordsCalculator;
		readonly bool vertical;
		public Axis3DMapping(XYDiagram3DCoordsCalculator coordsCalculator, Axis3D axis) {
			this.coordsCalculator = coordsCalculator;
			vertical = axis.IsVertical;
		}
		DiagramPoint GetDiagramPoint(double argument, double value, int selfOffset, int normalOffset) {
			double x, y;
			if(vertical) {
				x = value;
				y = argument;
			}
			else {
				x = argument;
				y = value;
			}
			DiagramPoint point = coordsCalculator.GetDiagramPointForDiagram(x, y);
			point.X += selfOffset;
			point.Y += normalOffset;
			return point;
		}
		public DiagramPoint GetNearDiagramPoint(double axisValue, int selfOffset, int normalOffset) {
			return GetDiagramPoint(axisValue, Double.NegativeInfinity, selfOffset, normalOffset);
		}
		public DiagramPoint GetFarDiagramPoint(double axisValue, int selfOffset, int normalOffset) {
			return GetDiagramPoint(axisValue, Double.PositiveInfinity, selfOffset, normalOffset);
		}
	}
}
