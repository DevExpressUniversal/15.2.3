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
using System.Linq;
using System.Collections.Generic;
using Model = DevExpress.Charts.Model;
namespace DevExpress.XtraCharts.ModelSupport.Implementation {
	public abstract class DiagramConfigurator {
		static Dictionary<Type, Type> supportedTypes;
		static DiagramConfigurator() {
			supportedTypes = new Dictionary<Type, Type>();
			supportedTypes[typeof(Model.CartesianChart)] = typeof(XYDiagramConfigurator);
			supportedTypes[typeof(Model.PolarChart)] = typeof(PolarDiagramConfigurator);
			supportedTypes[typeof(Model.RadarChart)] = typeof(RadarDiagramConfigurator);
			supportedTypes[typeof(Model.PieChart)] = typeof(PieDiagramConfigurator);
			supportedTypes[typeof(Model.Cartesian3DChart)] = typeof(XYDiagram3DConfigurator);
			supportedTypes[typeof(Model.Pie3DChart)] = typeof(Pie3DDiagramConfigurator);
		}
		public static DiagramConfigurator CreateInstance(Model.Chart model) {
			Type type;
			if(model.Series.Count > 0 && supportedTypes.TryGetValue(model.GetType(), out type))
				return (DiagramConfigurator)Activator.CreateInstance(type);
			return EmptyDiagramConfigurator.Instance;
		}
		public abstract void Configure(Diagram diagram, Model.Chart model);
		public virtual void ConfigureAxes(Diagram diagram, Model.Chart model, Model.IModelElementContainer container, DiagrammAppearanceConfiguratorBase appearanceConfigurator) {
		}
	}
	public abstract class GenericDiagramConfigurator<T> : DiagramConfigurator where T : Model.Chart {
		public override void Configure(Diagram diagram, Model.Chart model) {
			if(diagram != null) 
				ConfigureCore(diagram, (T)model);
		}
		protected virtual void ConfigureCore(Diagram diagram, T model) {
			Model.IChart3D chart3D = model as Model.IChart3D;
			Diagram3D diagram3D = diagram as Diagram3D;
			if(chart3D != null && diagram3D != null)
				Configurate3DOptions(diagram3D, chart3D.Options3D);
		}
		protected virtual void Configurate3DOptions(Diagram3D diagram, Model.IOptions3D options3D) {
			diagram.RotationType = RotationType.UseAngles;
			diagram.PerspectiveEnabled = true;
			diagram.RotationAngleX = options3D.RotationAngleX;
			diagram.RotationAngleY = options3D.RotationAngleY;
			diagram.RotationAngleZ = options3D.RotationAngleZ;
			diagram.PerspectiveAngle = options3D.PerspectiveAngle;
		}
	}
	public class EmptyDiagramConfigurator : DiagramConfigurator {
		public static readonly EmptyDiagramConfigurator Instance = new EmptyDiagramConfigurator();
		public override void Configure(Diagram diagram, Model.Chart model) {
		}
	}
}
