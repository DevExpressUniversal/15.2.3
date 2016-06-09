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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public enum AxisElementLocation {
		Outside,
		Inside
	}
	public abstract class AxisElementViewDataBase {
		readonly Axis2D axis;
		readonly AxisMappingBase mapping;
		readonly int sign;
		readonly int fullElementOffset;
		protected Axis2D Axis { get { return axis; } }
		protected HitTestController HitTestController { get { return axis.Diagram.Chart.HitTestController; } }
		protected HitTestState HitState { get { return ((IHitTest)axis).State; } }
		public AxisElementViewDataBase(Axis2D axis, AxisMappingBase mapping, int axisOffset, int elementOffset, AxisElementLocation location) {
			this.axis = axis;
			this.mapping = mapping;
			sign = GetSign(location);
			fullElementOffset = axisOffset * GetSign(AxisElementLocation.Outside) + elementOffset * sign;
		}
		protected Point GetScreenPoint(double axisValue, int selfOffset, int offset) {
			return (Point)mapping.GetScreenPoint(axisValue, selfOffset, fullElementOffset + offset * sign);
		}
		protected Point GetNotClampedScreenPoint(double axisValue, int selfOffset, int offset) {
			return (Point)((AxisIntervalMapping)mapping).GetNotClampedScreenPoint(axisValue, selfOffset, fullElementOffset + offset * sign);
		}
		int GetSign(AxisElementLocation location) {
			return ((location == AxisElementLocation.Outside && axis.Alignment == AxisAlignment.Far) ||
					(location == AxisElementLocation.Inside && axis.Alignment != AxisAlignment.Far)) ? 1 : -1;
		}
		public abstract void CalculateDiagramBoundsCorrection(RectangleCorrection correction);
		public abstract void Render(IRenderer renderer);
	}
}
