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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Paint;
using DevExpress.XtraDiagram.Utils;
using DevExpress.XtraDiagram.ViewInfo;
namespace DevExpress.XtraDiagram.Adorners {
	public class DiagramShapeParametersAdorner : DiagramItemAdornerBase, IAdorner<IShapeParametersAdorner>, IShapeParametersAdorner {
		Size gripSize;
		ParameterViewInfo[] parameters;
		ConfigurableArea points;
		public DiagramShapeParametersAdorner(IDiagramShape shape, Size gripSize) : base(shape) {
			this.parameters = null;
			this.points = null;
			this.gripSize = gripSize;
		}
		public Size GripSize { get { return gripSize; } }
		public DiagramShape Shape { get { return (DiagramShape)Item; } }
		public ConfigurableArea Points { get { return points; } }
		public ParameterViewInfo[] Parameters { get { return parameters; } }
		#region IShapeParametersAdorner
		ParameterViewInfo[] IShapeParametersAdorner.Parameters {
			get { return parameters; }
			set {
				if(parameters == value)
					return;
				parameters = value;
				OnParametersChanged();
			}
		}
		#endregion
		#region IAdorner<IShapeParametersAdorner>
		IShapeParametersAdorner IAdorner<IShapeParametersAdorner>.Model { get { return this; } }
		#endregion
		public override bool IsRotationSupports { get { return true; } }
		protected virtual void OnParametersChanged() {
			UpdatePoints();
		}
		protected override void UpdateBounds() {
			base.UpdateBounds();
			UpdatePoints();
		}
		protected void UpdatePoints() {
			this.points = GetPoints(Parameters);
		}
		public bool InParameter(Point point) {
			return Points.InParameter(AdjustLogicalPoint(point));
		}
		public DiagramShapeParameterItem GetShapeParameterItem(Point pt) {
			ParameterViewInfo parameter = Points.GetParameter(AdjustLogicalPoint(pt)).Parameter;
			return new DiagramShapeParameterItem(Shape, parameter.ParameterDescription);
		}
		protected virtual ConfigurableArea GetPoints(ParameterViewInfo[] parameters) {
			if(parameters == null) return null;
			return new ConfigurableArea(parameters.Select(parameterViewInfo => CreateConfigurationParameter(parameterViewInfo)));
		}
		protected ConfigurationParameter CreateConfigurationParameter(ParameterViewInfo parameterViewInfo) {
			DiagramElementPoint displayPoint = PlatformPointToDisplayPoint(GetParameterLogicalPos(parameterViewInfo.ParameterDescription));
			displayPoint.OffsetDisplay(-RotationOrigin.X, -RotationOrigin.Y);
			DiagramElementBounds bounds = displayPoint.CreateRect(GripSize);
			return new ConfigurationParameter(bounds, parameterViewInfo);
		}
		protected Point GetParameterLogicalPos(ParameterDescription param) {
			double value = GetShapeParameterValue(param);
			Point point = param.GetPoint(Shape.Size.ToPlatformSize(), Shape.GetParameters().ToArray(), value).ToWinPoint();
			return PointUtils.ApplyOffset(Shape.Bounds.Location, point);
		}
		protected double GetShapeParameterValue(ParameterDescription param) {
			if(Shape.Parameters == null) return param.DefaultValue;
			int arrayPos = Array.IndexOf(Shape.Shape.ParameterCollection.Parameters, param);
			return Shape.Parameters[arrayPos];
		}
		public override DiagramAdornerPainterBase GetPainter() {
			return new DiagramShapeParametersAdornerPainter();
		}
		public override DiagramAdornerObjectInfoArgsBase GetDrawArgs() {
			return new DiagramShapeParametersAdornerObjectInfoArgs();
		}
	}
}
