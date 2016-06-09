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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(DoughnutSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class DoughnutSeriesLabel : PieSeriesLabel {
		static float CalculateCorrection(float size, float maxSize) {
			float correction = size - maxSize / 2.0f;
			return correction > 0.0f ? correction : 0.0f;
		}
		internal static bool CalculateDoughnutLabelPosition(PieSeriesPointLayout pieLayout, out DiagramPoint position) {
			position = DiagramPoint.Zero;
			double fraction = pieLayout.Pie.HoleFraction;
			double width = pieLayout.PieBounds.Width / 2.0;
			double height = pieLayout.PieBounds.Height / 2.0;
			double minWidth = width * fraction;
			double minHeight = height * fraction;
			if (minWidth <= 0.0 || minHeight <= 0.0)
				return false;
			double startAngle = pieLayout.Pie.StartAngle;
			double halfAngle = pieLayout.Pie.HalfAngle;
			GRealPoint2D center = pieLayout.Pie.CalculateCenter(pieLayout.BasePoint);
			Ellipse maxEllipse = new Ellipse(center, width, height);
			Ellipse minEllipse = new Ellipse(center, minWidth, minHeight);
			Pie maxPie = new Pie(startAngle, halfAngle, maxEllipse, 0, 0);
			Pie minPie = new Pie(startAngle, halfAngle, minEllipse, 0, 0);
			position = new DiagramPoint((maxPie.FinishPoint.X + minPie.FinishPoint.X) / 2.0, (maxPie.FinishPoint.Y + minPie.FinishPoint.Y) / 2.0);
			return true;
		}
		public DoughnutSeriesLabel() : base() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new DoughnutSeriesLabel();
		}
		protected override GRealPoint2D CalculateAnchorPointAndAngles(ISimpleDiagramDomain domain, PieSeriesPointLayout pieLayout, ref RectangleF labelsBounds, out double lineAngle, out double crossAngle) {
			lineAngle = pieLayout.Pie.HalfAngle;
			crossAngle = lineAngle + Math.PI / 2.0;
			DiagramPoint anchorPoint = DiagramPoint.Zero;
			return (IsInside && CalculateDoughnutLabelPosition(pieLayout, out anchorPoint)) ? new GRealPoint2D(anchorPoint.X, anchorPoint.Y) : 
				base.CalculateAnchorPointAndAngles(domain, pieLayout, ref labelsBounds, out lineAngle, out crossAngle);
		}
	}
}
