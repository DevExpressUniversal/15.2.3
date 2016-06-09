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
using Model = DevExpress.Charts.Model;
namespace DevExpress.XtraCharts.ModelSupport.Implementation {
	public class RadarDiagramConfigurator : GenericDiagramConfigurator<Model.RadarChart> {
		protected override void ConfigureCore(Diagram diagram, Model.RadarChart model) {
			base.ConfigureCore(diagram, model);
			RadarDiagram radarDiagram = (RadarDiagram)diagram;
			radarDiagram.RotationDirection = (RadarDiagramRotationDirection)model.Direction;
			radarDiagram.DrawingStyle = model.Style == Model.CircularDiagramStyle.Circle ? RadarDiagramDrawingStyle.Circle : RadarDiagramDrawingStyle.Polygon;
		}
		public override void ConfigureAxes(Diagram diagram, Model.Chart model, Model.IModelElementContainer container, DiagrammAppearanceConfiguratorBase appearanceConfigurator) {
			base.ConfigureAxes(diagram, model, container, appearanceConfigurator);
			Model.RadarChart chartModel = model as Model.RadarChart;
			if (chartModel != null) {
				RadarAxesConfigurator configurator = new RadarAxesConfigurator(appearanceConfigurator);
				configurator.Configure(diagram as RadarDiagram, chartModel, container);
			}
		}
	}
}
