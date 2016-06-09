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
using DevExpress.XtraCharts.GLGraphics;
namespace DevExpress.XtraCharts.Native {
	public abstract class Diagram3DDomain {
		readonly Diagram3D diagram;
		readonly Rectangle bounds;
		double width, height, depth;
		double viewRadius;
		double viewOffsetX, viewOffsetY;
		double zoomFactor;
		int[] viewport;
		double[] modelView;
		double[] projection;
		protected Diagram3D Diagram { get { return diagram; } }
		protected double ViewRadius { get { return viewRadius; } }
		protected double ZoomFactor { get { return zoomFactor; } }		
		public Rectangle Bounds { get { return bounds; } }
		public double Width { get { return width; } }
		public double Height { get { return height; } }
		public double Depth { get { return depth; } }
		public double[] ModelViewMatrix { get { return modelView; } }
		public abstract Rectangle ElementBounds { get; }
		protected Diagram3DDomain(Diagram3D diagram, Rectangle bounds) {
			this.diagram = diagram;
			this.bounds = bounds;
		}
		void CalcBoundsAccordingZoomingAndScrolling(double distance, out DiagramPoint p1, out DiagramPoint p2) {
			p1 = new DiagramPoint(viewOffsetX, viewOffsetY, distance);
			p2 = new DiagramPoint(-viewOffsetX, -viewOffsetY, distance);
			double dx = (bounds.Width - ElementBounds.Width) * zoomFactor * 0.5;
			double dy = (bounds.Height - ElementBounds.Height) * zoomFactor * 0.5;
			p1 = DiagramPoint.Offset(p1, -dx, -dy, 0);
			p2 = DiagramPoint.Offset(p2, dx, dy, 2 * viewRadius);
			if (Diagram.ZoomPercent == Diagram3D.DefaultZoomPercent &&
				Diagram.HorizontalScrollPercent == Diagram3D.DefaultScrollPercent &&
				Diagram.VerticalScrollPercent == Diagram3D.DefaultScrollPercent)
				return;
			double width = p2.X - p1.X;
			double height = p2.Y - p1.Y;
			double xc = (p1.X + p2.X) / 2.0 - width * Diagram.HorizontalScrollPercent / 100.0;
			double yc = (p1.Y + p2.Y) / 2.0 - height * Diagram.VerticalScrollPercent / 100.0;
			dx = width / Diagram.ZoomPercent * 50.0;
			dy = height / Diagram.ZoomPercent * 50.0;
			p1.X = xc - dx;
			p1.Y = yc - dy;
			p2.X = xc + dx;
			p2.Y = yc + dy;
		}
		GraphicsCommand CreateOrthographicProjectionGraphicsCommand() {
			GraphicsCommand command = new ContainerGraphicsCommand();
			command.AddChildCommand(new TranslateGraphicsCommand(0.0, 0.0, -viewRadius * 2));
			DiagramPoint p1, p2;
			CalcBoundsAccordingZoomingAndScrolling(viewRadius, out p1, out p2);
			command.AddChildCommand(new OrthographicProjectionGraphicsCommand(p1, p2));
			return command;
		}
		GraphicsCommand CreatePerspectiveGraphicsCommand() {
			if (diagram.PerspectiveAngle == 0)
				return CreateOrthographicProjectionGraphicsCommand();
			double min = Math.Min(ElementBounds.Width, ElementBounds.Height);
			GraphicsCommand command = new ContainerGraphicsCommand();
			double distance = 0.5 * min * zoomFactor / Math.Tan(diagram.PerspectiveAngle / 360.0 * Math.PI);
			command.AddChildCommand(new TranslateGraphicsCommand(0.0, 0.0, -distance - viewRadius));
			DiagramPoint p1, p2;
			CalcBoundsAccordingZoomingAndScrolling(distance, out p1, out p2);
			command.AddChildCommand(new FrustumProjectionGraphicsCommand(p1, p2));
			return command;
		}
		protected void SetWidth(double width) {
			this.width = width;
		}
		protected void SetHeight(double height) {
			this.height = height;
		}
		protected void SetDepth(double depth) {
			this.depth = depth;
		}
		protected void CalculateParameters() {
			SetDimensions();
			viewRadius = CalcViewRadius();
			if (!diagram.PerspectiveEnabled || diagram.PerspectiveAngle == 0) {
				zoomFactor = viewRadius * 2.0;
				viewOffsetX = -viewRadius;
				viewOffsetY = -viewRadius;
			} 
			else {
				double radianAngle = diagram.PerspectiveAngle / 360.0 * Math.PI;
				zoomFactor = 2.0 * viewRadius * (1.0 - Math.Sin(radianAngle)) / Math.Cos(radianAngle);
				viewOffsetX = -zoomFactor / 2.0;
				viewOffsetY = -zoomFactor / 2.0;
			}
			if (ElementBounds.Width > ElementBounds.Height) {
				zoomFactor /= ElementBounds.Height;
				viewOffsetX -= (ElementBounds.Width - ElementBounds.Height) * zoomFactor * 0.5;
			}
			else {
				zoomFactor /= ElementBounds.Width;
				viewOffsetY -= (ElementBounds.Height - ElementBounds.Width) * zoomFactor * 0.5;
			}
			viewport = new int[4] { bounds.Left, bounds.Top, bounds.Width, bounds.Height };
			using (GraphicsCommand command = new ContainerGraphicsCommand()) {
				command.AddChildCommand(CreateProjectionGraphicsCommand());
				command.AddChildCommand(CreateModelViewGraphicsCommand());
				GLHelper.CalculateMatrices(command, out modelView, out projection);				
			}
		}
		protected abstract void SetDimensions();
		protected virtual double CalcViewRadius() {
			return Math.Sqrt(width * width + height * height + depth * depth) * 0.5;
		}
		protected virtual GraphicsCommand CreateAdditionalModelViewCommand() {
			return null;
		}
		public GraphicsCommand CreateModelViewGraphicsCommand() {
			GraphicsCommand command = new ContainerGraphicsCommand();
			command.AddChildCommand(diagram.CreateRotationGraphicsCommand());
			command.AddChildCommand(CreateAdditionalModelViewCommand());
			return command;
		}
		public GraphicsCommand CreateProjectionGraphicsCommand() {
			return diagram.PerspectiveEnabled ? CreatePerspectiveGraphicsCommand() : CreateOrthographicProjectionGraphicsCommand();
		}
		public DiagramPoint Project(DiagramPoint point) {
			double x, y, z;
			GLU.Project(point.X, point.Y, point.Z, modelView, projection, viewport, out x, out y, out z);
			return new DiagramPoint(x, bounds.Height - y + bounds.Top + bounds.Top, z);
		}
		public DiagramPoint UnProject(DiagramPoint point) {
			double x, y, z;
			GLU.UnProject(point.X, point.Y, point.Z, modelView, projection, viewport, out x, out y, out z);
			return new DiagramPoint(x, y, z);
		}
	}
}
