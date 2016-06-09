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
using System.Drawing.Drawing2D;
using DevExpress.XtraCharts.GLGraphics;
namespace DevExpress.XtraCharts.Native {
	public abstract class GraphicsCommand : IDisposable {
		List<GraphicsCommand> children = null;
		bool disposed = false;
		internal bool Disposed { get { return disposed; } } 
		public List<GraphicsCommand> Children { get { return children; } }
		public GraphicsCommand() {
		}
		protected void ExecuteNative(OpenGLGraphics gr) {
			try {
				ExecuteChildrenCommands(gr);
			} 
			finally {
			}
		}
		void ExecuteOpenGL(OpenGLGraphics gr) {
			BeforeExecute(gr);
			try {
				ExecuteInternal(gr);
				ExecuteChildrenCommands(gr);
			} 
			finally {
				AfterExecute(gr);
			}
		}
		protected abstract void ExecuteInternal(OpenGLGraphics gr);
		protected virtual void DisposeInternal() {
		}
		protected virtual void BeforeExecute(OpenGLGraphics gr) {
		}
		protected virtual void AfterExecute(OpenGLGraphics gr) {
		}
		public void AddChildCommand(GraphicsCommand command) {
			if (command != null) {
				if (children == null)
					children = new List<GraphicsCommand>();
				children.Add(command);
			}
		}
		public virtual void Execute(OpenGLGraphics gr) {
			if (gr.NativeDrawing)
				ExecuteNative(gr);
			else
				ExecuteOpenGL(gr);
		}
		protected void ExecuteChildrenCommands(OpenGLGraphics gr) {
			if (children != null)
				foreach (GraphicsCommand command in children)
					command.Execute(gr);
		}
		public void Dispose() {
			DisposeInternal();
			if (children != null) {
				foreach (GraphicsCommand command in children)
					command.Dispose();
				children = null;
			}
			GC.SuppressFinalize(this);
			this.disposed = true;
		}
	}
	public class ContainerGraphicsCommand : GraphicsCommand {
		public ContainerGraphicsCommand() : base() {
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
	}
	public class DisposeGraphicsCommand : ContainerGraphicsCommand {
		IDisposable disposable;
		public DisposeGraphicsCommand(IDisposable disposable) {
			this.disposable = disposable;
		}
		protected override void DisposeInternal() {
			if (disposable != null) {
				disposable.Dispose();
				disposable = null;
			}
		}
	}
	public class DrawingTypeGraphicsCommand : ContainerGraphicsCommand {
		bool nativeDrawing;
		public DrawingTypeGraphicsCommand(bool nativeDrawing) {
			this.nativeDrawing = nativeDrawing;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			gr.SetDrawingType(nativeDrawing);
		}
	}
	public class HitTestingGraphicsCommand : GraphicsCommand {
		HitTestController hitTestController;
		IHitTest hitTest;
		IHitRegion hitRegion;
		object additionalObject;
		public HitTestController HitTestController { get { return hitTestController; } }
		public IHitTest HitTest { get { return hitTest; } }
		public IHitRegion HitRegion { get { return hitRegion; } }
		public HitTestingGraphicsCommand(HitTestController hitTestController, IHitTest hitTest, IHitRegion hitRegion) : this(hitTestController, hitTest, hitRegion, null) {
		}
		public HitTestingGraphicsCommand(HitTestController hitTestController, IHitTest hitTest, IHitRegion hitRegion, object additionalObject) {
			this.hitTestController = hitTestController;
			this.hitTest = hitTest;
			this.hitRegion = hitRegion;
			this.additionalObject = additionalObject;
		}
		public override void Execute(OpenGLGraphics gr) {
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
		protected override void DisposeInternal() {
			if(hitRegion != null) {
				hitRegion.Dispose();
				hitRegion = null;
			}
		}
	}
	public class ClippingGraphicsCommand : GraphicsCommand {
		PlaneRectangle clipRectangle;
		Region clipRegion;
		GraphicsPath clipPath;
		int plane;
		protected virtual CombineMode ActualCombineMode { get { return CombineMode.Replace; } }
		public ZPlaneRectangle ClipRectangle { get { return clipRectangle as ZPlaneRectangle; } }
		public Region ClipRegion { get { return clipRegion; } }
		public GraphicsPath ClipPath { get { return clipPath; } }
		public ClippingGraphicsCommand(PlaneRectangle clipRectangle) : base() {
			Initialize(clipRectangle, null, null);
		}
		public ClippingGraphicsCommand(PlaneRectangle clipRectangle, int plane) : base() {
			this.plane = plane;
			Initialize(clipRectangle, null, null);
		}
		public ClippingGraphicsCommand(Region clipRegion) : base() {
			Initialize(ZPlaneRectangle.Empty, clipRegion, null);
		}
		public ClippingGraphicsCommand(GraphicsPath path) : base() {
			Initialize(ZPlaneRectangle.Empty, null, path);
		}
		void Initialize(PlaneRectangle clipRectangle, Region clipRegion, GraphicsPath clipPath) {
			this.clipRectangle = clipRectangle;
			this.clipRegion = clipRegion;
			this.clipPath = clipPath;
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			PlaneEquation eq = MathUtils.CalcPlaneEquation(clipRectangle);
			GL.ClipPlane(plane, eq.Equation);
			GL.Enable(plane);
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			GL.Disable(plane);
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
		protected override void DisposeInternal() {
			if (clipRegion != null) {
				clipRegion.Dispose();
				clipRegion = null;
			}
			if (clipPath != null) {
				clipPath.Dispose();
				clipPath = null;
			}
		}
	}
	public class IntersectClippingGraphicsCommand : ClippingGraphicsCommand { 
		protected override CombineMode ActualCombineMode { get { return CombineMode.Intersect; } }
		public IntersectClippingGraphicsCommand(PlaneRectangle clipRectangle) : base(clipRectangle){
		}
		public IntersectClippingGraphicsCommand(Region clipRegion) : base(clipRegion){
		}
		public IntersectClippingGraphicsCommand(GraphicsPath clipPath) : base(clipPath) {
		}
	}
	public class LightingGraphicsCommand : GraphicsCommand {
		Color ambientColor;
		Color materialSpecularColor;
		Color materialEmissionColor;
		float materialShininess;
		public LightingGraphicsCommand(Color ambientColor, Color materialSpecularColor, Color materialEmissionColor, float materialShininess) {
			this.ambientColor = ambientColor;
			this.materialSpecularColor = materialSpecularColor;
			this.materialEmissionColor = materialEmissionColor;
			this.materialShininess = materialShininess;
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			GL.Disable(GL.LIGHTING);
			GL.Disable(GL.COLOR_MATERIAL);
			GL.Disable(GL.NORMALIZE);
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			float[] color = new float[4];
			GL.LightModeli(GL.LIGHT_MODEL_TWO_SIDE, GL.TRUE);
			OpenGLGraphics.CalculateColorComponents(ambientColor, color);
			GL.LightModelfv(GL.LIGHT_MODEL_AMBIENT, color);
			GL.ColorMaterial(GL.FRONT_AND_BACK, GL.AMBIENT_AND_DIFFUSE);
			OpenGLGraphics.CalculateColorComponents(materialSpecularColor, color);
			GL.Materialfv(GL.FRONT_AND_BACK, GL.SPECULAR, color);
			OpenGLGraphics.CalculateColorComponents(materialEmissionColor, color);
			GL.Materialfv(GL.FRONT_AND_BACK, GL.EMISSION, color);
			GL.Materialf(GL.FRONT_AND_BACK, GL.SHININESS, materialShininess);
			GL.BlendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
			GL.Enable(GL.NORMALIZE);
			GL.Enable(GL.LIGHTING);
			GL.Enable(GL.COLOR_MATERIAL);
		}
	}
	public class LightGraphicsCommand : GraphicsCommand {
		int index;
		Color ambientColor;
		Color diffuseColor;
		Color specularColor;
		DiagramPoint position;
		DiagramVector spotDirection;
		float spotExponent;
		float spotCutoff;
		float constantAttenuation;
		float linearAttenuation;
		float quadraticAttenuation;
		bool directional;
		public LightGraphicsCommand(int index, Color ambientColor, Color diffuseColor, Color specularColor, DiagramPoint position, DiagramVector spotDirection, float spotExponent, float spotCutoff, float constantAttenuation, float linearAttenuation, float quadraticAttenuation) {
			this.index = index;
			this.ambientColor = ambientColor;
			this.diffuseColor = diffuseColor;
			this.specularColor = specularColor;
			this.position = position;
			this.spotDirection = spotDirection;
			this.spotExponent = spotExponent;
			this.spotCutoff = spotCutoff;
			this.constantAttenuation = constantAttenuation;
			this.linearAttenuation = linearAttenuation;
			this.quadraticAttenuation = quadraticAttenuation;
			directional = false;
		}
		public LightGraphicsCommand(int index, Color ambientColor, Color diffuseColor, Color specularColor, DiagramPoint position, DiagramVector spotDirection) : this(index, ambientColor, diffuseColor, specularColor, position, spotDirection, 0.0f, 180.0f, 1.0f, 0.0f, 0.0f) {
			directional = true;
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			GL.Disable(GL.LIGHT0 + index);
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			int actualIndex = GL.LIGHT0 + index;
			float[] color = new float[4];
			OpenGLGraphics.CalculateColorComponents(ambientColor, color);
			GL.Lightfv(actualIndex, GL.AMBIENT, color);
			OpenGLGraphics.CalculateColorComponents(diffuseColor, color);
			GL.Lightfv(actualIndex, GL.DIFFUSE, color);
			OpenGLGraphics.CalculateColorComponents(specularColor, color);
			GL.Lightfv(actualIndex, GL.SPECULAR, color);
			GL.Lightfv(actualIndex, GL.POSITION, new float[4] { (float)position.X, (float)position.Y, (float)position.Z, directional ? 0.0f : 1.0f });
			GL.Lightfv(actualIndex, GL.SPOT_DIRECTION, new float[4] { (float)spotDirection.DX, (float)spotDirection.DY, (float)spotDirection.DZ, 0.0f });
			GL.Lightf(actualIndex, GL.SPOT_EXPONENT, spotExponent);
			GL.Lightf(actualIndex, GL.SPOT_CUTOFF, spotCutoff);
			GL.Lightf(actualIndex, GL.CONSTANT_ATTENUATION, constantAttenuation);
			GL.Lightf(actualIndex, GL.LINEAR_ATTENUATION, linearAttenuation);
			GL.Lightf(actualIndex, GL.QUADRATIC_ATTENUATION, quadraticAttenuation);
			GL.Enable(GL.LIGHT0 + index);
		}
	}
	public class DepthTestGraphicsCommand : GraphicsCommand {
		public DepthTestGraphicsCommand() {
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			GL.Enable(GL.DEPTH_TEST);
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			GL.Disable(GL.DEPTH_TEST);
		}
	}
	public class StencilBufferGraphicsCommand : GraphicsCommand {
		public StencilBufferGraphicsCommand() {
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			GL.ClearStencil(0x0);
			GL.Enable(GL.STENCIL_TEST);
			GL.Clear(GL.STENCIL_BUFFER_BIT);
			GL.StencilFunc(GL.NOTEQUAL, 1, 1);
			GL.StencilOp(GL.REPLACE, GL.REPLACE, GL.REPLACE);
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			GL.Disable(GL.STENCIL_TEST);
		}
	}
	public class StencilFuncGraphicsCommand : GraphicsCommand {
		readonly int groupId;
		public StencilFuncGraphicsCommand(int groupId) {
			this.groupId = groupId;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			int index = groupId % gr.StencilBufferMaxValue;
			if (index == 0)
				GL.Clear(GL.STENCIL_BUFFER_BIT);
			index++;
			GL.StencilFunc(GL.NOTEQUAL, index, (uint)index);
		}
		protected override void AfterExecute(OpenGLGraphics gr) {			
		}
	}
	public class PolygonOffsetGraphicsCommand : GraphicsCommand {
		public PolygonOffsetGraphicsCommand() {
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			GL.Enable(GL.POLYGON_OFFSET_FILL);
			GL.PolygonOffset(1.0f, 1.0f);
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			GL.Disable(GL.POLYGON_OFFSET_FILL);
		}
	}
	public class MaskColorBufferGraphicsCommand : GraphicsCommand {
		public MaskColorBufferGraphicsCommand() {
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			GL.ColorMask(false, false, false, false);
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			GL.ColorMask(true, true, true, true);
		}
	}
	public class MaskDepthBufferGraphicsCommand : GraphicsCommand {
		public MaskDepthBufferGraphicsCommand() {
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			GL.DepthMask(false);
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			GL.DepthMask(true);
		}
	}
 }
