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
namespace DevExpress.XtraCharts.Native { 
	public class SimpleDiagram3DDomain : Diagram3DDomain, ISimpleDiagramDomain {
		public static SimpleDiagram3DDomain Create(SimpleDiagram3D diagram, Rectangle bounds, Rectangle labelsBounds, Rectangle diagramBounds, ISimpleDiagram3DSeriesView view) {
			SimpleDiagram3DDomain domain = new SimpleDiagram3DDomain(diagram, bounds, labelsBounds, diagramBounds, view);
			domain.CalculateParameters();
			return domain;
		}
		readonly Rectangle labelsBounds;
		readonly Rectangle diagramBounds;
		readonly double depthFactor;
		readonly double heightToWidthRatio;
		public Rectangle LabelsBounds { get { return CorrectLabelsBoundsAccordingZoomingAndScrolling(labelsBounds); }  }
		public override Rectangle ElementBounds { get { return diagramBounds; } }
		SimpleDiagram3DDomain(SimpleDiagram3D diagram, Rectangle bounds, Rectangle labelsBounds, Rectangle diagramBounds, ISimpleDiagram3DSeriesView view) : base(diagram, bounds) {
			this.labelsBounds = labelsBounds;
			this.diagramBounds = diagramBounds;
			depthFactor = view.DepthFactor;
			heightToWidthRatio = view.HeightToWidthRatio;
		}
		Rectangle CorrectLabelsBoundsAccordingZoomingAndScrolling(Rectangle bounds) {
			if (Diagram.ZoomPercent == Diagram3D.DefaultZoomPercent && 
				Diagram.HorizontalScrollPercent == Diagram3D.DefaultScrollPercent && 
				Diagram.VerticalScrollPercent == Diagram3D.DefaultScrollPercent)
					return bounds;
			double width = bounds.Width * ZoomFactor;
			double height = bounds.Height * ZoomFactor;
			double xc = bounds.Left + bounds.Width / 2.0 + width * Diagram.HorizontalScrollPercent / 100.0;
			double yc = bounds.Top + bounds.Height / 2.0 + height * Diagram.VerticalScrollPercent / 100.0;
			double dx = bounds.Width * Diagram.ZoomPercent / 200.0;
			double dy = bounds.Height * Diagram.ZoomPercent / 200.0;
			int left = (int)Math.Round(xc - dx);
			int top = (int)Math.Round(yc - dy);
			int right = (int)Math.Round(xc + dx);
			int bottom = (int)Math.Round(yc + dy);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		GraphicsCommand CreateLightingCommand(out GraphicsCommand innerLightingCommand) {
			double scale = ViewRadius * 0.7;
			DiagramPoint location = new DiagramPoint(-scale, 1.5 * scale, scale * 2);
			GraphicsCommand light0Command = new LightGraphicsCommand(0, Color.FromArgb(255, 0, 0, 0), 
				Color.FromArgb(255, 130, 130, 130), Color.FromArgb(255, 50, 50, 50), location, 
				DiagramVector.CreateNormalized(-location.X, -location.Y, -location.Z), 0.0f, 180.0f, 1.0f, 0.0f, 0.0f);
			location = new DiagramPoint(-scale * 2, -scale * 2, scale * 4);
			GraphicsCommand light1Command = new LightGraphicsCommand(1, Color.FromArgb(255, 0, 0, 0),
				Color.FromArgb(255, 80, 80, 80), Color.FromArgb(255, 100, 100, 100), location, 
				DiagramVector.CreateNormalized(-location.X, -location.Y, -location.Z), 0.0f, 180.0f, 1.0f, 0.0f, 0.0f);
			light0Command.AddChildCommand(light1Command);
			innerLightingCommand = light1Command;
			return light0Command;
		}
		protected override void SetDimensions() {
			SetWidth(diagramBounds.Width);
			SetHeight(diagramBounds.Height);			
			SetDepth(Math.Min(diagramBounds.Width, diagramBounds.Height / heightToWidthRatio) * depthFactor);
		}
		protected override double CalcViewRadius() {
			double height = Math.Min(Height, Width * heightToWidthRatio);
			return Math.Sqrt(height * height + Depth * Depth) * 0.5 * 1.001;
		}
		public GraphicsCommand CreateGraphicsCommand(out GraphicsCommand innerCommand, out GraphicsCommand innerCommandForLabelConnector) {
			GraphicsCommand depthCommand = new DepthTestGraphicsCommand();
			GraphicsCommand saveStateCommand = new SaveStateGraphicsCommand();
			depthCommand.AddChildCommand(saveStateCommand);
			GraphicsCommand ambientLightingCommand = new LightingGraphicsCommand(Color.FromArgb(255, 130, 130, 130), Color.White, Color.Black, 10.0f);
			saveStateCommand.AddChildCommand(ambientLightingCommand);
			ambientLightingCommand.AddChildCommand(new IdentityTransformGraphicsCommand(TransformMatrix.ModelView));
			ambientLightingCommand.AddChildCommand(new IdentityTransformGraphicsCommand(TransformMatrix.Projection));
			GraphicsCommand projectionCommand = CreateProjectionGraphicsCommand();
			ambientLightingCommand.AddChildCommand(projectionCommand);
			GraphicsCommand antialiasing = new PolygonAntialiasingGraphicsCommand();
			projectionCommand.AddChildCommand(antialiasing);
			GraphicsCommand saveState = new SaveStateGraphicsCommand();
			antialiasing.AddChildCommand(saveState);
			saveState.AddChildCommand(CreateLightingCommand(out innerCommand));
			innerCommand.AddChildCommand(CreateModelViewGraphicsCommand());
			GraphicsCommand saveStateForLabelConnector = new SaveStateGraphicsCommand();
			antialiasing.AddChildCommand(saveStateForLabelConnector);
			innerCommandForLabelConnector = new LightGraphicsCommand(0, Color.FromArgb(255, 130, 130, 130),
				Color.Black, Color.Black, DiagramPoint.Zero, DiagramVector.Zero, 0.0f, 180.0f, 1.0f, 0.0f, 0.0f);
			innerCommandForLabelConnector.AddChildCommand(CreateModelViewGraphicsCommand());			
			saveStateForLabelConnector.AddChildCommand(innerCommandForLabelConnector);						
			return depthCommand;
		}
	}
}
