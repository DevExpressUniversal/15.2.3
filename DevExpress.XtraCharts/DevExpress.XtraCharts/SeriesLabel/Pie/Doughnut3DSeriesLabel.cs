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

using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(Doughnut3DSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Doughnut3DSeriesLabel : Pie3DSeriesLabel {
		public Doughnut3DSeriesLabel() : base() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new DoughnutSeriesLabel();
		}
		protected override GRealPoint2D CalculateAnchorPointAndAngles(ISimpleDiagramDomain domain, PieSeriesPointLayout pieLayout, ref RectangleF labelsBounds, out double lineAngle, out double crossAngle) {
			SimpleDiagram3DDomain domain3D = domain as SimpleDiagram3DDomain;
			DiagramPoint labelPoint = DiagramPoint.Zero;
			if (domain3D == null || !IsInside ||
				!DoughnutSeriesLabel.CalculateDoughnutLabelPosition(pieLayout, out labelPoint))
					return base.CalculateAnchorPointAndAngles(domain, pieLayout, ref labelsBounds, out lineAngle, out crossAngle);
			labelPoint.X = labelPoint.X - pieLayout.BasePoint.X;
			labelPoint.Y = pieLayout.BasePoint.Y - labelPoint.Y;
			return Project(domain, labelPoint, pieLayout.Pie.MajorSemiaxis, ref labelsBounds, out lineAngle, out crossAngle);
		}
	}
}
