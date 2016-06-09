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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Imaging;
namespace DevExpress.XtraGauges.Core.Primitive {
	public abstract class BaseLeafPrimitive : BaseLeaf<IRenderableElement>, IRenderableElement, ISerizalizeableElement {
		BasePrimitiveGeometry geometryProviderCore;
		BasePrimitiveInfo primitiveInfoProviderCore;
		BasePrimitiveHandler interactionProviderCore;
		ComplexShape shapesCore;
		BaseShapePainter painterCore;
		BaseColorShader shaderCore;
		bool renderableCore = true;
		bool hitTestEnabledCore = true;
		bool fCalculationDelayedCore = true;
		bool fEnabledCore = true;
		bool isSerializingCore = false;
		public event CustomDrawElementEventHandler CustomDrawElement;
		public BaseLeafPrimitive()
			: base() {
			UpdateShapes();
		}
		public BaseLeafPrimitive(string name)
			: base(name) {
			UpdateShapes();
		}
		protected void OnShapesChanged() {
			if(IsDisposing) return;
			Shapes.Clear();
			UpdateShapes();
		}
		void UpdateShapes() {
			OnLoadShapes();
			OnShaderProcessing();
		}
		protected virtual void OnLoadShapes() { }
		protected void OnShaderProcessing() {
			foreach(BaseShape shape in Shapes) {
				ShapeProcessing(shape);
				ShadingHelper.ProcessShape(shape, Shader, Enabled);
			}
		}
		protected virtual void ShapeProcessing(BaseShape shape) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.geometryProviderCore = CreateISupportGeometryProvider();
			this.primitiveInfoProviderCore = CreateIViewInfoProvider();
			this.interactionProviderCore = CreateISupportInteractionProvider();
			this.shapesCore = new ComplexShape(CreateShapeCollection());
			this.painterCore = CreateShapePainter();
			this.shaderCore = BaseColorShader.Empty;
		}
		protected override void OnDispose() {
			CustomDrawElement = null;
			Ref.Dispose(ref geometryProviderCore);
			Ref.Dispose(ref primitiveInfoProviderCore);
			Ref.Dispose(ref interactionProviderCore);
			Ref.Dispose(ref shapesCore);
			Ref.Dispose(ref painterCore);
			Ref.Dispose(ref shaderCore);
			DestroyShader();
			base.OnDispose();
		}
		void DestroyShader() {
			if(Shader != null)
				Shader.Changed -= OnShaderChanged;
			if(Shader != null && !Shader.IsEmpty)
				Ref.Dispose(ref shaderCore);
		}
		protected virtual BasePrimitiveGeometry CreateISupportGeometryProvider() {
			return new BasePrimitiveGeometry(OnTransformChanged);
		}
		protected virtual BasePrimitiveInfo CreateIViewInfoProvider() {
			return new BasePrimitiveInfo(this);
		}
		protected virtual BasePrimitiveHandler CreateISupportInteractionProvider() {
			return new BasePrimitiveHandler(this);
		}
		protected virtual BaseShapeCollection CreateShapeCollection() {
			return new BaseShapeCollection();
		}
		protected virtual BaseShapePainter CreateShapePainter() {
			return new BaseShapePainter();
		}
		protected void AssignPrimitiveProperties(BaseLeafPrimitive source) {
			this.ZOrder = source.ZOrder;
			SetShaderCore(source.Shader.Clone() as BaseColorShader);
		}
		protected bool IsDifferFromPrimitive(BaseLeafPrimitive source) {
			return (source == null) ? true : (this.ZOrder != source.ZOrder || Shader.IsDifferFrom(source.Shader));
		}
		protected void OnTransformChanged() {
			Accept(
				delegate(IElement<IRenderableElement> e) {
					e.Self.ResetCache();
					e.Self.SetViewInfoDirty();
				}
			);
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("BaseLeafPrimitiveEnabled"),
#endif
DefaultValue(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public bool Enabled {
			get { return fEnabledCore; }
			set {
				if(Enabled == value) return;
				fEnabledCore = value;
				OnShaderChanged(this, EventArgs.Empty);
			}
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("BaseLeafPrimitiveShader"),
#endif
XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		[Category("Appearance")]
		public BaseColorShader Shader {
			get { return shaderCore; }
			set {
				if(Shader == value) return;
				SetShaderCore(value);
				OnShaderChanged(this, EventArgs.Empty);
			}
		}
		void SetShaderCore(BaseColorShader value) {
			DestroyShader();
			shaderCore = value;
			if(Shader != null && !Shader.IsEmpty)
				Shader.Changed += OnShaderChanged;
		}
		void OnShaderChanged(object sender, EventArgs e) {
			OnShaderChangedCore();
			RaiseChanged(e);
		}
		protected virtual void OnShaderChangedCore() {
			if(AllowCacheRenderOperation)
				Self.ResetCache(CacheKeys.RenderedImage);
			SetCalculationDelayed();
			OnShapesChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShapeCollection Shapes {
			get { return shapesCore.Collection; }
		}
		protected BaseShapePainter Painter {
			get { return painterCore; }
		}
		protected void OnRender(IRenderingContext context) {
			using(PaintInfo paintInfo = new PaintInfo(context)) {
				if(!RaiseCustomDrawElement(context))
					RenderShapes(paintInfo);
			}
		}
		protected virtual void RenderShapes(PaintInfo paintInfo) {
			Painter.Draw(paintInfo, shapesCore);
		}
		protected virtual bool RaiseCustomDrawElement(IRenderingContext context) {
			CustomDrawElementEventArgs e = new CustomDrawElementEventArgs(context, Painter, Info, Shapes);
			if(CustomDrawElement != null)
				CustomDrawElement(this, e);
			return e.Handled;
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("BaseLeafPrimitiveZOrder"),
#endif
DefaultValue(0)]
		[Category("Geometry")]
		[XtraSerializableProperty]
		public int ZOrder {
			get { return -AcceptOrder; }
			set { AcceptOrder = -value; }
		}
		[Browsable(false), DefaultValue(true)]
		public bool Renderable {
			get { return renderableCore; }
			set { renderableCore = value; }
		}
		[Browsable(false), DefaultValue(true)]
		public bool HitTestEnabled {
			get { return hitTestEnabledCore; }
			set { hitTestEnabledCore = value; }
		}
		protected virtual bool AllowCacheRenderOperation {
			get { return false; }
		}
		void IRenderable.Render(Stream stream) {
			using(IRenderingContext context = RenderingContext.FromStream(stream)) {
				Self.Render(context);
			}
		}
		void IRenderable.Render(Graphics g) {
			using(IRenderingContext context = RenderingContext.FromGraphics(g)) {
				Self.Render(context);
			}
		}
		void IRenderable.Render(IRenderingContext context) {
			this.RenderCore(context);
		}
		protected void RenderCore(IRenderingContext context) {
			if(IsDisposing || !IsValidGeometry()) return;
			WaitForPendingDelayedCalculation();
			BeginTransform();
			if(!Self.IsCachingLocked && AllowCacheRenderOperation) {
				RenderOperation(context, true);
				object cacheValue = Self.TryGetValue(CacheKeys.RenderedImage);
				if(cacheValue == null) {
					cacheValue = CacheRenderOperation(context);
				}
				RenderFromCache(context, cacheValue);
			}
			else RenderOperation(context, false);
			CancelTransform();
		}
		void RenderOperation(IRenderingContext context, bool layzy) {
			using(new SmartContext(context, this)) {
				if(layzy) return;
				if(Renderable) OnRender(context);
			}
		}
		void RenderFromCache(IRenderingContext context, object cacheValue) {
			RectangleF rect = Self.ViewInfo.RelativeBoundBox;
			GraphicsState state = context.Graphics.Save();
			context.Transform = new Matrix(1, 0, 0, 1, rect.X, rect.Y);
			context.Graphics.DrawImageUnscaled((Image)cacheValue, Point.Empty);
			context.Graphics.Restore(state);
		}
		object CacheRenderOperation(IRenderingContext context) {
			RectangleF rect = Self.ViewInfo.RelativeBoundBox;
			Bitmap bmp = new Bitmap((int)Math.Ceiling(rect.Width), (int)Math.Ceiling(rect.Height), PixelFormat.Format32bppArgb);
			Self.LockCaching();
			using(Graphics tempG = Graphics.FromImage(bmp)) {
				Matrix m = context.Transform.Clone();
				m.Translate(-rect.X, -rect.Y, MatrixOrder.Append);
				tempG.Transform = m;
				tempG.SmoothingMode = SmoothingMode.HighQuality;
				RenderOperation(RenderingContext.FromGraphics(tempG), false);
			}
			Self.UnlockCaching();
			Self.CacheValue(CacheKeys.RenderedImage, bmp);
			return bmp;
		}
		public void WaitForPendingDelayedCalculation() {
			if(CalculationDelayed) {
				Self.ResetCache();
				OnDelayedCalculation();
				this.fCalculationDelayedCore = false;
			}
		}
		protected internal bool IsValidGeometry() {
			Model.IScaleComponent scaleComponent = this as Model.IScaleComponent;
			return (scaleComponent == null) || IsValidGeometry(scaleComponent.Scale);
		}
		bool IsValidGeometry(Model.IScale scale) {
			return (scale != null) && (!DependsOnValueBounds || scale.MaxValue != scale.MinValue);
		}
		protected virtual bool DependsOnValueBounds { get { return false; } }
		protected void SetCalculationDelayed() {
			this.fCalculationDelayedCore = true;
		}
		protected bool CalculationDelayed {
			get { return fCalculationDelayedCore; }
		}
		protected virtual void OnDelayedCalculation() { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PointF Location {
			get { return geometryProviderCore.Location; }
			set { geometryProviderCore.Location = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FactorF2D ScaleFactor {
			get { return geometryProviderCore.ScaleFactor; }
			set { geometryProviderCore.ScaleFactor = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float Angle {
			get { return geometryProviderCore.Angle; }
			set { geometryProviderCore.Angle = value; }
		}
		[Browsable(false)]
		public bool IsTransformLocked {
			get { return geometryProviderCore.IsTransformLocked; }
		}
		public void BeginTransform() {
			geometryProviderCore.BeginTransform();
		}
		public void EndTransform() {
			geometryProviderCore.EndTransform();
		}
		public void CancelTransform() {
			geometryProviderCore.CancelTransform();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Matrix Transform {
			get { return geometryProviderCore.Transform; }
			set { geometryProviderCore.Transform = value; }
		}
		public void ResetTransform() {
			geometryProviderCore.ResetTransform();
		}
		protected BasePrimitiveInfo Info {
			get { return primitiveInfoProviderCore; }
		}
		IViewInfo ISupportViewInfo.ViewInfo {
			get { return primitiveInfoProviderCore; }
		}
		void ISupportViewInfo.SetViewInfoDirty() {
			Info.SetDirty();
		}
		void ISupportViewInfo.CalcViewInfo(Matrix local) {
			if(IsDisposing) return;
			Info.CalcInfo(local);
		}
		protected BasePrimitiveHandler Handler {
			get { return interactionProviderCore; }
		}
		void ISupportInteraction.ProcessMouseDown(MouseEventArgsInfo ea) {
			if(IsDisposing) return;
			Handler.ProcessMouseDown(ea);
		}
		void ISupportInteraction.ProcessMouseUp(MouseEventArgsInfo ea) {
			if(IsDisposing) return;
			Handler.ProcessMouseUp(ea);
		}
		void ISupportInteraction.ProcessMouseMove(MouseEventArgsInfo ea) {
			if(IsDisposing) return;
			Handler.ProcessMouseMove(ea);
		}
		public BasePrimitiveHitInfo CalcHitInfo(Point pt) {
			return Handler.CalcHitInfo(pt);
		}
		#region ISerizalizeableGaugeElement
		void IXtraSerializable.OnEndDeserializing(string layoutVersion) {
			OnEndDeserializingCore();
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			OnStartDeserializingCore();
		}
		protected virtual void OnEndDeserializingCore() {
			EndTransform();
			EndUpdate();
		}
		protected virtual void OnStartDeserializingCore() {
			BeginUpdate();
			BeginTransform();
		}
		void IXtraSerializable.OnEndSerializing() {
			isSerializingCore = false;
			Site = storedSite;
		}
		ISite storedSite = null;
		void IXtraSerializable.OnStartSerializing() {
			isSerializingCore = true;
			storedSite = Site;
			Site = null;
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsXtraSerializing { get { return isSerializingCore; } }
		String parentNameCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public virtual string ParentName { get { return parentNameCore; } set { parentNameCore = value; } }
		String parentColNameCore;
		String boundElementNameCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public virtual string ParentCollectionName { get { return parentColNameCore; } set { parentColNameCore = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public virtual string BoundElementName { get { return boundElementNameCore; } set { boundElementNameCore = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public virtual string TypeNameEx { get { return this.GetType().Name; } set { } }
		List<ISerizalizeableElement> ISerizalizeableElement.GetChildren() { return GetChildernCore(); }
		protected virtual List<ISerizalizeableElement> GetChildernCore() { return null; }
		#endregion
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateShader(XtraItemEventArgs e) {
			if(e.Item.HasChildren) {
				XtraPropertyInfo xpInfo = e.Item.ChildProperties["TypeTag"];
				return (xpInfo != null) ? CreateShader((string)xpInfo.Value) : Shader;
			}
			return BaseColorShader.Empty;
		}
		protected BaseColorShader CreateShader(string typeTag) {
			switch(typeTag) {
				case "Opacity": return new OpacityShader();
				case "Style": return new StyleShader();
				case "Complex": return new ComplexShader();
				case "Gray": return new GrayShader();
				case "Disabled": return new DisabledShader();
				case "Empty":
				default: return BaseColorShader.Empty;
			}
		}
	}
	public abstract class BaseCompositePrimitive : BaseComposite<IRenderableElement>, IRenderableElement {
		BasePrimitiveGeometry geometryProviderCore;
		BasePrimitiveInfo primitiveInfoProviderCore;
		BasePrimitiveHandler interactionProviderCore;
		ComplexShape shapesCore;
		BaseShapePainter painterCore;
		BaseColorShader shaderCore;
		bool renderableCore = true;
		bool hitTestEnabledCore = true;
		bool fCalculationDelayedCore = false;
		bool fEnabledCore = true;
		public event CustomDrawElementEventHandler CustomDrawElement;
		public BaseCompositePrimitive()
			: base() {
			UpdateShapes();
		}
		public BaseCompositePrimitive(string name)
			: base(name) {
			UpdateShapes();
		}
		protected void OnShapesChanged() {
			if(IsDisposing) return;
			Shapes.Clear();
			UpdateShapes();
		}
		void UpdateShapes() {
			OnLoadShapes();
			OnShaderProcessing();
		}
		protected virtual void OnLoadShapes() { }
		protected void OnShaderProcessing() {
			foreach(BaseShape shape in Shapes) {
				ShadingHelper.ProcessShape(shape, Shader, Enabled);
			}
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.geometryProviderCore = CreateISupportGeometryProvider();
			this.primitiveInfoProviderCore = CreateIViewInfoProvider();
			this.interactionProviderCore = CreateISupportInteractionProvider();
			this.shapesCore = new ComplexShape(CreateShapeCollection());
			this.painterCore = CreateShapePainter();
			this.shaderCore = BaseColorShader.Empty;
		}
		protected override void OnDispose() {
			CustomDrawElement = null;
			Ref.Dispose(ref geometryProviderCore);
			Ref.Dispose(ref primitiveInfoProviderCore);
			Ref.Dispose(ref interactionProviderCore);
			Ref.Dispose(ref shapesCore);
			Ref.Dispose(ref painterCore);
			Ref.Dispose(ref shaderCore);
			DestroyShader();
			base.OnDispose();
		}
		void DestroyShader() {
			if(Shader != null)
				Shader.Changed -= OnShaderChanged;
			if(Shader != null && !Shader.IsEmpty)
				Ref.Dispose(ref shaderCore);
		}
		protected virtual BasePrimitiveGeometry CreateISupportGeometryProvider() {
			return new BasePrimitiveGeometry(OnTransformChanged);
		}
		protected virtual BasePrimitiveInfo CreateIViewInfoProvider() {
			return new BasePrimitiveInfo(this);
		}
		protected virtual BasePrimitiveHandler CreateISupportInteractionProvider() {
			return new BasePrimitiveHandler(this);
		}
		protected virtual BaseShapeCollection CreateShapeCollection() {
			return new BaseShapeCollection();
		}
		protected virtual BaseShapePainter CreateShapePainter() {
			return new BaseShapePainter();
		}
		protected void AssignPrimitiveProperties(BaseCompositePrimitive source) {
			this.ZOrder = source.ZOrder;
			SetShaderCore((BaseColorShader)source.Shader.Clone());
		}
		protected bool IsDifferFromPrimitive(BaseCompositePrimitive source) {
			return (source == null) ? true : (this.ZOrder != source.ZOrder || Shader.IsDifferFrom(source.Shader));
		}
		protected void OnTransformChanged() {
			Accept(
				delegate(IElement<IRenderableElement> e) {
					e.Self.ResetCache();
					e.Self.SetViewInfoDirty();
				}
			);
		}
		[DefaultValue(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public bool Enabled {
			get { return fEnabledCore; }
			set {
				if(Enabled == value) return;
				fEnabledCore = value;
				OnShaderChanged(this, EventArgs.Empty);
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public BaseColorShader Shader {
			get { return shaderCore; }
			set {
				if(Shader == value) return;
				SetShaderCore(value);
				OnShaderChanged(this, EventArgs.Empty);
			}
		}
		void SetShaderCore(BaseColorShader value) {
			DestroyShader();
			shaderCore = value;
			if(Shader != null && !Shader.IsEmpty)
				Shader.Changed += OnShaderChanged;
		}
		void OnShaderChanged(object sender, EventArgs e) {
			OnShapesChanged();
			RaiseChanged(e);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShapeCollection Shapes {
			get { return shapesCore.Collection; }
		}
		protected BaseShapePainter Painter {
			get { return painterCore; }
		}
		protected void OnRender(IRenderingContext context) {
			using(PaintInfo paintInfo = new PaintInfo(context)) {
				if(!RaiseCustomDrawElement(context))
					RenderShapes(paintInfo);
			}
		}
		protected virtual void RenderShapes(PaintInfo paintInfo) {
			Painter.Draw(paintInfo, shapesCore);
		}
		protected virtual bool RaiseCustomDrawElement(IRenderingContext context) {
			CustomDrawElementEventArgs e = new CustomDrawElementEventArgs(context, Painter, Info, Shapes);
			if(CustomDrawElement != null)
				CustomDrawElement(this, e);
			return e.Handled;
		}
		[DefaultValue(0)]
		public int ZOrder {
			get { return -AcceptOrder; }
			set { AcceptOrder = -value; }
		}
		[Browsable(false), DefaultValue(true)]
		public bool Renderable {
			get { return renderableCore; }
			set { renderableCore = value; }
		}
		[Browsable(false), DefaultValue(true)]
		public bool HitTestEnabled {
			get { return hitTestEnabledCore; }
			set {
				if(HitTestEnabled == value) return;
				this.hitTestEnabledCore = value;
				Elements.Accept(
						delegate(IElement<IRenderableElement> e) { e.Self.HitTestEnabled = value; }
					);
			}
		}
		void IRenderable.Render(Stream stream) {
			using(IRenderingContext context = RenderingContext.FromStream(stream)) {
				Self.Render(context);
			}
		}
		void IRenderable.Render(Graphics g) {
			using(IRenderingContext context = RenderingContext.FromGraphics(g)) {
				Self.Render(context);
			}
		}
		void IRenderable.Render(IRenderingContext context) {
			this.RenderCore(context);
		}
		protected void RenderCore(IRenderingContext context) {
			if(IsDisposing) return;
			WaitForPendingDelayedCalculation();
			BeginTransform();
			using(new SmartContext(context, this)) {
				if(Renderable) OnRender(context);
				Elements.Accept(
					delegate(IElement<IRenderableElement> e) { e.Self.Render(context); }
				);
			}
			CancelTransform();
		}
		public void WaitForPendingDelayedCalculation() {
			if(CalculationDelayed) {
				OnDelayedCalculation();
				this.fCalculationDelayedCore = false;
			}
		}
		protected void SetCalculationDelayed() {
			this.fCalculationDelayedCore = true;
		}
		protected bool CalculationDelayed {
			get { return fCalculationDelayedCore; }
		}
		protected virtual void OnDelayedCalculation() { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PointF Location {
			get { return geometryProviderCore.Location; }
			set { geometryProviderCore.Location = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FactorF2D ScaleFactor {
			get { return geometryProviderCore.ScaleFactor; }
			set { geometryProviderCore.ScaleFactor = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float Angle {
			get { return geometryProviderCore.Angle; }
			set { geometryProviderCore.Angle = value; }
		}
		[Browsable(false)]
		public bool IsTransformLocked {
			get { return geometryProviderCore.IsTransformLocked; }
		}
		public void BeginTransform() {
			geometryProviderCore.BeginTransform();
		}
		public void EndTransform() {
			geometryProviderCore.EndTransform();
		}
		public void CancelTransform() {
			geometryProviderCore.CancelTransform();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Matrix Transform {
			get { return geometryProviderCore.Transform; }
			set { geometryProviderCore.Transform = value; }
		}
		public void ResetTransform() {
			geometryProviderCore.ResetTransform();
		}
		protected BasePrimitiveInfo Info {
			get { return primitiveInfoProviderCore; }
		}
		IViewInfo ISupportViewInfo.ViewInfo {
			get { return primitiveInfoProviderCore; }
		}
		void ISupportViewInfo.SetViewInfoDirty() {
			Info.SetDirty();
		}
		void ISupportViewInfo.CalcViewInfo(Matrix local) {
			if(IsDisposing) return;
			Info.CalcInfo(local);
		}
		protected BasePrimitiveHandler Handler {
			get { return interactionProviderCore; }
		}
		void ISupportInteraction.ProcessMouseDown(MouseEventArgsInfo ea) {
			if(IsDisposing) return;
			Handler.ProcessMouseDown(ea);
		}
		void ISupportInteraction.ProcessMouseUp(MouseEventArgsInfo ea) {
			if(IsDisposing) return;
			Handler.ProcessMouseUp(ea);
		}
		void ISupportInteraction.ProcessMouseMove(MouseEventArgsInfo ea) {
			if(IsDisposing) return;
			Handler.ProcessMouseMove(ea);
		}
		public BasePrimitiveHitInfo CalcHitInfo(Point pt) {
			return Handler.CalcHitInfo(pt);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateShader(XtraItemEventArgs e) {
			if(e.Item.HasChildren) {
				XtraPropertyInfo xpInfo = e.Item.ChildProperties["TypeTag"];
				return (xpInfo != null) ? CreateShader((string)xpInfo.Value) : Shader;
			}
			return BaseColorShader.Empty;
		}
		protected BaseColorShader CreateShader(string typeTag) {
			switch(typeTag) {
				case "Opacity": return new OpacityShader();
				case "Style": return new StyleShader();
				case "Complex": return new ComplexShader();
				case "Gray": return new GrayShader();
				case "Disabled": return new DisabledShader();
				case "Empty":
				default: return BaseColorShader.Empty;
			}
		}
	}
}
