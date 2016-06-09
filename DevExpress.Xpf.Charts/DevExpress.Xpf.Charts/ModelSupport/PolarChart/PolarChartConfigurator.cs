﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Diagnostics;
using System.Reflection;
using DevExpress.Utils;
using Model = DevExpress.Charts.Model;
namespace DevExpress.Xpf.Charts.ModelSupport {
	public class PolarChartConfigurator : ChartConfigurator {
		new Model.PolarChart ChartModel { get { return base.ChartModel as Model.PolarChart; } }
		public PolarChartConfigurator(Model.Chart chartModel, Model.IModelElementContainer container)
			: base(chartModel, container) {
		}
		protected override void ConfigureDiagram(ChartControl chart) {
			PolarDiagram2D diagram = new PolarDiagram2D();
			diagram.RotationDirection = ChartModel.Direction == Model.DirectionMode.Clockwise ? CircularDiagramRotationDirection.Clockwise : CircularDiagramRotationDirection.Counterclockwise;
			diagram.ShapeStyle = ChartModel.Style == Model.CircularDiagramStyle.Circle ? CircularDiagramShapeStyle.Circle : CircularDiagramShapeStyle.Polygon;
			chart.Diagram = diagram;
		}
	}
}
