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
using System.Text;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using DevExpress.XtraGauges.Base;
using DevExpress.Utils.Serializing;
using System.Drawing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Customization;
using System.Collections;
using DevExpress.Utils;
using DevExpress.XtraGauges.Core.Layout;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Styles;
using System.ComponentModel.Design;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraGauges.Core.Model {
	public delegate void BaseGaugeChangedHandler(object sender, EventArgs e);
	public abstract class BaseGauge : Component, IGauge, ISupportLockUpdate, ISupportAcceptOrder, ILayoutManagerClient, ISupportCustomizeAction, ISerizalizeableElement, ISupportStyles {
		IGaugeContainer containerCore;
		bool disposingInProgressCore = false;
		BaseGaugeModel modelCore;
		BaseGaugeModel oldModel;
		Rectangle boundsCore;
		bool proportionalStretchCore = true;
		bool enabledStateCore = true;
		int lockUpdateCounter = 0;
		Rectangle prevInvalidateRect;
		int acceptOrderCore = 0;
		bool supressDrawBorderCore = false;
		public event CustomDrawElementEventHandler CustomDrawElement;
		GraphicsPath savedEmptyPath;
		public BaseGauge() {
			savedEmptyPath = BaseShape.SaveEmptyPathForFinalizerThread();
			this.prevInvalidateRect = Rectangle.Empty;
			this.boundsCore = new Rectangle(6, 6, 100, 100);
			this.nameCore = String.Empty;
			OnCreate();
		}
		public BaseGauge(IGaugeContainer container)
			: this() {
			(this as IGauge).SetContainer(container);
		}
		protected override void Dispose(bool disposing) {
			if(!disposing) {
				if(savedEmptyPath != null)
					BaseShape.EnsureEmptyPathFromFinalizerThread(savedEmptyPath);
				savedEmptyPath = null;
			}
			if(!IsDisposing) {
				disposingInProgressCore = true;
				CustomDrawElement = null;
				DisposeChildren();
				RemoveFromGaugesCollection();
				OnDispose();
				DestroyModel();
				containerCore = null;
				oldModel = null;
				savedEmptyPath = null;
			}
			base.Dispose(disposing);
		}
		public void InitializeDefault() {
			DisposeChildren();
			DestroyModel();
			InitializeDefaultCore();
		}
		protected abstract void InitializeDefaultCore();
		protected abstract void OnCreate();
		protected abstract void OnDispose();
		void RemoveFromGaugesCollection() {
			if (GaugeContainer != null && GaugeContainer.Gauges != null) {
				GaugeContainer.Gauges.Remove(this);
			}
		}
		string nameCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty]
		public virtual string Name {
			get { return nameCore; }
			set { nameCore = value; }
		}
		string parentNameCore = String.Empty;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty]
		public string ParentName {
			get { return parentNameCore; }
			set { parentNameCore = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty]
		public string ParentCollectionName {
			get { return "Gauges"; }
			set { }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty]
		public string TypeNameEx {
			get { return this.GetType().Name; }
			set { }
		}
		[Browsable(false)]
		public bool IsDisposing {
			get { return disposingInProgressCore; }
		}
		protected void SetContainerCore(IGaugeContainer container) {
			this.containerCore = container;
		}
		public virtual ColorScheme GetColorScheme() {
			return this.containerCore != null ? this.containerCore.ColorScheme : null; 
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseGaugeModel Model {
			get {
				if (modelCore == null) {
					modelCore = CreateModel();
					modelCore.Name = GetModelName();
					if (GaugeContainer != null)
						GaugeContainer.OnModelChanged(oldModel, modelCore);
					oldModel = null;
					ForceUpdateChildrenCore();
				}
				return modelCore;
			}
		}
		string GetModelName() {
			return (Site != null && !string.IsNullOrEmpty(Site.Name)) ? Site.Name : Guid.NewGuid().ToString();
		}
		protected abstract BaseGaugeModel CreateModel();
		protected void DestroyModel() {
			if (modelCore != null) {
				if (modelCore.Parent != null) {
					if (!(modelCore.Parent as BaseObject).IsDisposing)
						modelCore.Parent.Remove(modelCore);
				}
				oldModel = modelCore;
				modelCore.Dispose();
				modelCore = null;
			}
		}
		protected internal bool RaiseCustomDrawElement(IRenderingContext context, BaseShapePainter painter, IViewInfo info, BaseShapeCollection shapes) {
			CustomDrawElementEventArgs e = new CustomDrawElementEventArgs(context, painter, info, shapes);
			if (CustomDrawElement != null)
				CustomDrawElement(this, e);
			return e.Handled;
		}
		int ISupportAcceptOrder.AcceptOrder {
			get { return acceptOrderCore; }
			set { acceptOrderCore = value; }
		}
		protected bool EnabledState {
			get { return enabledStateCore; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Size MinimumSize {
			get { return new Size(20, 20); }
		}
		#region ILayoutManagerClient
		Rectangle ILayoutManagerClient.Bounds {
			get { return Bounds; }
			set { Bounds = value; }
		}
		ILayoutManagerContainer ILayoutManagerClient.LayoutContainer {
			get { return GaugeContainer as ILayoutManagerContainer; }
		}
		PreferredLayoutType ILayoutManagerClient.PreferredLayoutType {
			get { return CalcPreferredLayoutType(); }
		}
		protected virtual PreferredLayoutType CalcPreferredLayoutType() {
			return PreferredLayoutType.Fill;
		}
		#endregion
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("BaseGaugeBounds")]
#endif
#if !DXPORTABLE
		[TypeConverter(typeof(RectangleConverter))]
#endif
		[XtraSerializableProperty, Category("Layout")]
		public Rectangle Bounds {
			get { return boundsCore; }
			set {
				if (MinimumSize.Width > value.Width) value.Width = MinimumSize.Width;
				if (MinimumSize.Height > value.Height) value.Height = MinimumSize.Height;
				if (Bounds == value) return;
				SetBoundsCore(value);
			}
		}
		DefaultBoolean autoSizeByActualBoundsCore = DefaultBoolean.Default;
		protected virtual DefaultBoolean AutoSizeByActualBounds {
			get { return autoSizeByActualBoundsCore; }
			set {
				if (autoSizeByActualBoundsCore == value) return;
				autoSizeByActualBoundsCore = value;
				OnModelChanged(false);
			}
		}
		protected virtual void SetBoundsCore(Rectangle value) {
			this.boundsCore = value;
			OnModelChanged(false);
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("BaseGaugeProportionalStretch"),
#endif
		DefaultValue(true), XtraSerializableProperty, Category("Layout")]
		public bool ProportionalStretch {
			get { return proportionalStretchCore; }
			set {
				if (proportionalStretchCore == value) return;
				SetProportionalStretchCore(value);
			}
		}
#region Tag
		object tagCore;
		bool ShouldSerializeTag() { return tagCore != null; }
		void ResetTag() { tagCore = null; }
		[DefaultValue(null), Category("Data")]
#if !DXPORTABLE
		[TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		[Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
		public virtual object Tag {
			get { return tagCore; }
			set { tagCore = value; }
		}
#endregion Tag
		protected virtual void SetProportionalStretchCore(bool value) {
			this.proportionalStretchCore = value;
			OnModelChanged(false);
		}
		protected virtual void CalculateModel() {
			Model.Calc(this as IGauge, Bounds);
		}
		protected virtual void OnModelChanged(bool reset) {
			if (lockModelUpdate > 0) {
				updateModelRequest++;
				return;
			}
			if (IsUpdateLocked || IsDisposing || GaugeContainer == null) return;
			if (reset) {
				DestroyModel();
			}
			CalculateModelTransform();
			GaugeContainer.UpdateRect(reset ? RectangleF.Empty : Bounds);
		}
		protected void CalculateModelTransform() {
			Model.BeginTransform();
			Model.Location = Bounds.Location;
			CalculateModel();
			RaiseChanged();
			SetModelProportions();
			Model.EndTransform();
			Model.CalculateBackgroundShape(Bounds);
		}
		RectangleF cachedContentRect = RectangleF.Empty;
		protected virtual void SetModelProportions() {
			if (GaugeContainer == null) return;
			SizeF contentSize = Model.ContentSize;
			bool isSmartLayout = Model.SmartLayout && AutoSizeByActualBounds != DefaultBoolean.False;
			if (!ProportionalStretch && !isSmartLayout) {
				float sx = (float)Bounds.Width / contentSize.Width;
				float sy = (float)Bounds.Height / contentSize.Height;
				Model.ScaleFactor = new FactorF2D(sx, sy);
				Model.Location = new PointF(
					(float)Bounds.Left + (Bounds.Width - contentSize.Width * sx) * 0.5f,
					(float)Bounds.Top + (Bounds.Height - contentSize.Height * sy) * 0.5f);
			}
			else {
				RectangleF contentRect = RectangleF.Empty;
				if (isSmartLayout) {
					if (cachedContentRect.IsEmpty)
						cachedContentRect = CalcContentRect(Model.Composite);
					if (cachedContentRect.IsEmpty)
						return;
					contentRect = cachedContentRect;
				}
				else contentRect = new RectangleF(Point.Empty, Model.ContentSize);
				float sx = (float)Bounds.Width / contentRect.Width;
				float sy = (float)Bounds.Height / contentRect.Height;
				float scaleFactor = Math.Min(sx, sy);
				Model.ScaleFactor = new FactorF2D(scaleFactor, scaleFactor);
				Model.Location = new PointF(
					(float)Bounds.Left - contentRect.Left * scaleFactor + (Bounds.Width - contentRect.Width * scaleFactor) * 0.5f,
					(float)Bounds.Top - contentRect.Top * scaleFactor + (Bounds.Height - contentRect.Height * scaleFactor) * 0.5f);
			}
		}
		protected RectangleF CalcContentRect(IComposite<IRenderableElement> composite) {
			RectangleF result = RectangleF.Empty;
			foreach (var e in composite.Elements) {
				if (e is ModelRoot)
					return MathHelper.CalcRelativeBoundBox(CalcContentRect(e.Composite), e.Self.Transform);
				if (!e.Self.Renderable || !IsValidGeometry(e.Self))
					continue;
				e.Self.WaitForPendingDelayedCalculation();
				RectangleF r = (e.Self.ViewInfo.IsReady) ?
					e.Self.ViewInfo.BoundBox : MathHelper.CalcBoundBox(e.Self);
				if (r.IsEmpty)
					continue;
				r = MathHelper.CalcRelativeBoundBox(r, e.Self.Transform);
				if (result.IsEmpty)
					result = r;
				result = RectangleF.Union(result, r);
			}
			return result;
		}
		bool IsValidGeometry(IRenderableElement element) {
			BaseLeafPrimitive primitive = element as BaseLeafPrimitive;
			return (primitive == null) || primitive.IsValidGeometry();
		}
		protected void OnComponentsChanged(object sender, EventArgs ea) {
			cachedContentRect = RectangleF.Empty;
			if (!IsUpdateLocked && GaugeContainer != null)
				GaugeContainer.InvalidateRect(Bounds);
		}
		Rectangle CalcRectToInvalidate(object sender) {
			IRenderableElement element = sender as IRenderableElement;
			if (element == null) return Bounds;
			Rectangle currentInvalidRect = Rectangle.Round(element.ViewInfo.RelativeBoundBox);
			Rectangle rect = prevInvalidateRect.IsEmpty ? Bounds : prevInvalidateRect;
			prevInvalidateRect = currentInvalidRect;
			return Rectangle.Union(rect, currentInvalidRect);
		}
		protected IGaugeContainer GaugeContainer {
			get { return containerCore; }
		}
		protected bool IsUpdateLocked {
			get { return lockUpdateCounter > 0; }
		}
		public void BeginUpdate() {
			lockUpdateCounter++;
		}
		public void CancelUpdate() {
			lockUpdateCounter--;
		}
		public void EndUpdate() {
			if (--lockUpdateCounter == 0) OnModelChanged(true);
		}
		protected void RaiseChanged() {
			if (Changed != null) Changed(this, EventArgs.Empty);
		}
		public event EventHandler Changed;
		IGaugeContainer IGauge.Container {
			get { return GaugeContainer; }
		}
		void IGauge.SetContainer(IGaugeContainer container) {
			if (container == null) RemoveFromGaugesCollection();
			SetContainerCore(container);
			OnModelChanged(true);
		}
		string IGauge.Category {
			get { return GetCategoryCore(); }
		}
		protected virtual string GetCategoryCore() {
			return string.Empty;
		}
		bool IGauge.SuppressDrawBorder {
			get { return supressDrawBorderCore; }
			set {
				if (supressDrawBorderCore == value) return;
				supressDrawBorderCore = value;
				Model.CalculateBackgroundShape(Bounds);
			}
		}
		bool IGauge.CanRemoveGaugeElement(BaseElement<IRenderableElement> element) {
			return FindDependentGaugeElements(element).Count == 0;
		}
		void IGauge.RemoveGaugeElement(BaseElement<IRenderableElement> element) {
			BeginUpdate();
			RemoveGaugeElementCore(element);
			CancelUpdate();
			RaiseModelChanged();
		}
		void RaiseModelChanged() {
			if (IsUpdateLocked || IsDisposing || GaugeContainer == null) return;
			GaugeContainer.OnModelChanged(Model, Model);
		}
		BaseElement<IRenderableElement> IGauge.DuplicateElement(BaseElement<IRenderableElement> element) {
			BaseElement<IRenderableElement> duplicate = DuplicateGaugeElementCore(element);
			List<string> names = new List<string>(GetNamesCore());
			foreach (IGauge gauge in GaugeContainer.Gauges)
				names.AddRange(gauge.GetNames());
			if (duplicate != null) duplicate.Name = UniqueNameHelper.GetUniqueName(element.Name + "Copy", names, 0);
			return duplicate;
		}
		void IGauge.AddGaugeElement(BaseElement<IRenderableElement> element) {
			if (element == null) return;
			BeginUpdate();
			AddGaugeElementCore(element);
			EndUpdate();
		}
		void IGauge.CheckEnabledState(bool value) {
			if (EnabledState == value) return;
			this.enabledStateCore = value;
			SetEnabledCore(value);
		}
		int lockModelUpdate = 0;
		int updateModelRequest = 0;
		void IGauge.Clear() {
			BeginModelUpdate();
			ClearCore();
			EndModelUpdate();
		}
		protected void BeginModelUpdate() {
			lockModelUpdate++;
		}
		protected void EndModelUpdate() {
			if (--lockModelUpdate == 0) {
				if (updateModelRequest > 0) {
					OnModelChanged(true);
					updateModelRequest = 0;
				}
			}
		}
		protected abstract void ClearCore();
		List<string> IGauge.GetNames() {
			return GetNamesCore();
		}
		void IGauge.ForceUpdateChildren() {
			ForceUpdateChildrenCore();
		}
		protected virtual void ForceUpdateChildrenCore() {
			List<ISerizalizeableElement> children = GetChildernCore();
			foreach (ISerizalizeableElement child in children) {
				ISupportLockUpdate e = child as ISupportLockUpdate;
				if (e != null) {
					e.BeginUpdate();
					e.EndUpdate();
				}
			}
		}
		protected abstract List<string> GetNamesCore();
		protected virtual List<BaseElement<IRenderableElement>> FindDependentGaugeElements(BaseElement<IRenderableElement> element) {
			return new List<BaseElement<IRenderableElement>>();
		}
		protected abstract void SetEnabledCore(bool enabled);
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return GetActionsCore();
		}
		protected virtual CustomizeActionInfo[] GetActionsCore() {
			return (Model as ISupportCustomizeAction).GetActions();
		}
		protected void AddGaugeElementCore(BaseElement<IRenderableElement> element) {
			AddGaugeElementToComponentCollection(element as IComponent);
			AddComponentToDesignTimeSurface(element as IComponent);
		}
		protected void RemoveGaugeElementCore(BaseElement<IRenderableElement> element) {
			RemoveDependentElements(element);
			RemoveElementCore(element);
		}
		void RemoveDependentElements(BaseElement<IRenderableElement> element) {
			List<BaseElement<IRenderableElement>> dependent = FindDependentGaugeElements(element);
			for (int i = 0; i < dependent.Count; i++) RemoveElementCore(dependent[i]);
		}
		void RemoveElementCore(BaseElement<IRenderableElement> element) {
			RemoveElementFromModel(element);
			RemoveComponentFromDesignTimeSurface(element as IComponent);
			if (!element.IsDisposing) element.Dispose();
		}
		protected virtual void RemoveElementFromModel(BaseElement<IRenderableElement> element) {
			if (modelCore != null) Model.Composite.Remove(element);
		}
		protected abstract void AddGaugeElementToComponentCollection(IComponent component);
		protected abstract BaseElement<IRenderableElement> DuplicateGaugeElementCore(BaseElement<IRenderableElement> element);
		protected virtual void AddComponentToDesignTimeSurface(IComponent component) {
			if (Site == null) return;
			Site.Container.Add(component);
			INamed namedComponent = component as INamed;
			if (namedComponent != null) {
				if (string.IsNullOrEmpty(namedComponent.Name)) {
					namedComponent.Name = component.Site.Name;
				}
			}
		}
		protected virtual void RemoveComponentFromDesignTimeSurface(IComponent component) {
#if !DXPORTABLE
			ISite site = (Site != null) ? Site : (component != null ? component.Site : null);
			if (site != null && site.Container != null) {
				IDesignerHost designerHost = site.Container as IDesignerHost;
				IGaugeDesigner gaugeDesigner = designerHost.GetDesigner(site.Component) as IGaugeDesigner;
				if (gaugeDesigner == null || !gaugeDesigner.IsUndoInProgress)
					site.Container.Remove(component);
			}
#endif
		}
		protected void ForceUpdateModel() {
			cachedContentRect = Rectangle.Empty;
			OnModelChanged(true);
		}
		protected void DisposeChildren() {
			List<ISerizalizeableElement> elements = (this as ISerizalizeableElement).GetChildren();
			if (elements == null) return;
			foreach (ISerizalizeableElement e in elements) {
				IComponent component = e as IComponent;
				if (component != null) {
					RemoveComponentFromDesignTimeSurface(component);
					component.Dispose();
				}
			}
		}
		ISite storedSite = null;
		void IXtraSerializable.OnStartSerializing() {
			storedSite = Site;
			Site = null;
		}
		void IXtraSerializable.OnEndSerializing() {
			Site = storedSite;
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			BeginModelUpdate();
		}
		void IXtraSerializable.OnEndDeserializing(string layoutVersion) {
			EndModelUpdate();
		}
		List<ISerizalizeableElement> ISerizalizeableElement.GetChildren() {
			return new List<ISerizalizeableElement>(GetChildernCore());
		}
		string ISerizalizeableElement.BoundElementName { get { return null; } set { } }
		protected virtual List<ISerizalizeableElement> GetChildernCore() {
			return new List<ISerizalizeableElement>();
		}
		protected void CollectChildren(List<ISerizalizeableElement> list, IEnumerable col, string parentCollectionName) {
			foreach (object obj in col) {
				ISerizalizeableElement element = obj as ISerizalizeableElement;
				if (element != null) {
					element.ParentCollectionName = parentCollectionName;
					list.Add(element);
				}
			}
		}
		public virtual void ApplyTextSettings(TextSettings settings) { }
		public TextSettings ReadTextSettings() {
			TextSettings settings = new TextSettings();
			ReadTextSettingsCore(settings);
			return settings;
		}
		protected virtual void ReadTextSettingsCore(TextSettings settings) { }
		public virtual void ApplyStyles(Styles.StyleCollection styles) {
			if (styles == null) return;
			BeginUpdate();
			IEnumerable children = GetChildernCore();
			foreach (object child in children)
				styles.Apply(child);
			Styles.Style gaugeStyle = styles.FindStyle(GetType());
			if (gaugeStyle != null)
				gaugeStyle.Apply(this);
			EndUpdate();
			if (Model != null)
				UpdateModelTransform();
		}
		void UpdateModelTransform() {
			using (var modelTransform = new Matrix()) {
				Model.Accept(delegate (IElement<IRenderableElement> e) {
					e.Self.SetViewInfoDirty();
					CalcTransform(modelTransform, e.Self);
				});
			}
			cachedContentRect = Rectangle.Empty;
			CalculateModelTransform();
		}
		void CalcTransform(Matrix modelTransform, IRenderableElement e) {
			e.WaitForPendingDelayedCalculation();
			e.BeginTransform();
			e.CalcViewInfo(CalcWorkMatrix(modelTransform, e));
			e.CancelTransform();
		}
		Matrix CalcWorkMatrix(Matrix contextMatrix, IRenderableElement element) {
			var workMatrix = contextMatrix.Clone();
			if (!element.Transform.IsIdentity)
				workMatrix.Multiply(element.Transform, MatrixOrder.Prepend);
			return workMatrix;
		}
	}
	public static class BaseGaugeExtension {
		public static void InitializeLabelDefault(Label label) {
			label.BeginUpdate();
			label.Position = new PointF2D(125, 125);
			label.ZOrder = -1001;
			label.AppearanceText.TextBrush = new SolidBrushObject(Color.Black);
			label.FormatString = "{0}";
			label.Text = label.Name;
			label.EndUpdate();
		}
		public static void InitializeImageIndicatorDefault(ImageIndicator imageIndicator) {
			imageIndicator.BeginUpdate();
			imageIndicator.Position = new PointF2D(125, 125);
			imageIndicator.ImageLayoutMode = ImageLayoutMode.Default;
			imageIndicator.Size = new SizeF(32, 32);
			imageIndicator.ZOrder = -1001;
			imageIndicator.EndUpdate();
		}
	}
	public static class CircularGaugeExtention {
		public static void InitializeScaleDefault(ArcScale scale, PointF2D center) {
			scale.BeginUpdate();
			scale.MajorTickmark.TextShape.AppearanceText.Font = new Font("Tahoma", 11F);
			scale.MajorTickmark.TextShape.AppearanceText.TextBrush = new SolidBrushObject("Color:Black");
			scale.Center = center;
			scale.EndAngle = 60F;
			scale.MajorTickmark.FormatString = "{0:F0}";
			scale.MajorTickmark.ShapeOffset = -3.5F;
			scale.MajorTickmark.ShapeType = TickmarkShapeType.Circular_Style1_4;
			scale.MajorTickmark.TextOffset = -15F;
			scale.MajorTickmark.TextOrientation = LabelOrientation.LeftToRight;
			scale.MaxValue = 100F;
			scale.MinorTickCount = 4;
			scale.MinorTickmark.ShapeType = TickmarkShapeType.Circular_Style1_3;
			scale.StartAngle = -240F;
			scale.Value = 50F;
			scale.EndUpdate();
		}
		public static void InitializeBackgroundLayerDefault(ArcScaleBackgroundLayer backgroundLayer, IArcScale scale) {
			backgroundLayer.BeginUpdate();
			backgroundLayer.ArcScale = scale;
			backgroundLayer.Size = new SizeF(scale.RadiusX * 2.5f, scale.RadiusY * 2.5f);
			backgroundLayer.ZOrder = ((IRenderableElement)scale).ZOrder + 1000;
			backgroundLayer.EndUpdate();
		}
		public static void InitializeEffectLayerDefault(ArcScaleEffectLayer effectLayer, IArcScale scale) {
			effectLayer.BeginUpdate();
			effectLayer.ArcScale = scale;
			effectLayer.Size = new SizeF(scale.RadiusX * 2f, scale.RadiusY);
			effectLayer.ZOrder = ((IRenderableElement)scale).ZOrder - 1000;
			effectLayer.EndUpdate();
		}
		public static void InitializeSpindleCapDefault(ArcScaleSpindleCap spindleCap, IArcScale scale) {
			spindleCap.BeginUpdate();
			spindleCap.ArcScale = scale;
			spindleCap.Size = new SizeF(scale.RadiusX * 0.4f, scale.RadiusY * 0.4f);
			spindleCap.ZOrder = ((IRenderableElement)scale).ZOrder - 100;
			spindleCap.EndUpdate();
		}
		public static void InitializeMarkerDefault(ArcScaleMarker marker, IArcScale scale) {
			marker.BeginUpdate();
			marker.ArcScale = scale;
			marker.ZOrder = ((IRenderableElement)scale).ZOrder - 100;
			marker.EndUpdate();
		}
		public static void InitializeStateIndicatorDefault(ScaleStateIndicator indicator, IArcScale scale) {
			indicator.BeginUpdate();
			indicator.IndicatorScale = scale;
			indicator.ZOrder = ((IRenderableElement)scale).ZOrder - 100;
			indicator.States.Add(indicator.CreateState("Default"));
			((IScaleIndicatorState)indicator.States[0]).StartValue = scale.MinValue;
			((IScaleIndicatorState)indicator.States[0]).IntervalLength = scale.ScaleLength;
			indicator.Center = new PointF2D(scale.Center.X + scale.RadiusX, scale.Center.Y);
			indicator.EndUpdate();
		}
		public static void InitializeNeedleDefault(ArcScaleNeedle needle, IArcScale scale) {
			needle.BeginUpdate();
			needle.ArcScale = scale;
			needle.StartOffset = -4.4F;
			needle.ZOrder = ((IRenderableElement)scale).ZOrder - 50;
			needle.EndUpdate();
		}
		public static void InitializeRangeBarDefault(ArcScaleRangeBar rangeBar, IArcScale scale) {
			rangeBar.BeginUpdate();
			rangeBar.ArcScale = scale;
			rangeBar.ZOrder = ((IRenderableElement)scale).ZOrder - 10;
			rangeBar.EndUpdate();
		}
	}
	public static class LinearGaugeExtention {
		public static void InitializeScaleDefault(LinearScale scale, PointF2D startPoint, PointF2D endPoint) {
			scale.BeginUpdate();
			scale.MajorTickmark.TextShape.AppearanceText.TextBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:Black");
			scale.MajorTickCount = 6;
			scale.MajorTickmark.FormatString = "{0:F0}";
			scale.MajorTickmark.ShapeOffset = -20F;
			scale.MajorTickmark.ShapeType = DevExpress.XtraGauges.Core.Model.TickmarkShapeType.Linear_Style1_1;
			scale.MajorTickmark.TextOffset = -32F;
			scale.MaxValue = 100F;
			scale.MinorTickCount = 4;
			scale.MinorTickmark.ShapeOffset = -14F;
			scale.MinorTickmark.ShapeType = DevExpress.XtraGauges.Core.Model.TickmarkShapeType.Linear_Style1_2;
			scale.StartPoint = startPoint;
			scale.EndPoint = endPoint;
			scale.EndUpdate();
		}
		public static void InitializeLevelDefault(LinearScaleLevel level, LinearScale scale) {
			level.BeginUpdate();
			level.LinearScale = scale;
			level.ZOrder = ((IRenderableElement)scale).ZOrder - 50;
			level.EndUpdate();
		}
		public static void InitializeBackgroundLayerDefault(LinearScaleBackgroundLayer backgroundLayer, LinearScale scale) {
			backgroundLayer.BeginUpdate();
			backgroundLayer.LinearScale = scale;
			backgroundLayer.ZOrder = ((IRenderableElement)scale).ZOrder + 1000;
			backgroundLayer.ScaleStartPos = new SizeF(0.5f, 0.86f);
			backgroundLayer.ScaleEndPos = new SizeF(0.5f, 0.14f);
			backgroundLayer.EndUpdate();
		}
		public static void InitializeEffectLayerDefault(LinearScaleEffectLayer effectLayer, LinearScale scale) {
			effectLayer.BeginUpdate();
			effectLayer.LinearScale = scale;
			effectLayer.ZOrder = ((IRenderableElement)scale).ZOrder - 1000;
			effectLayer.EndUpdate();
		}
		public static void InitializeMarkerDefault(LinearScaleMarker marker, LinearScale scale) {
			marker.BeginUpdate();
			marker.LinearScale = scale;
			marker.ZOrder = ((IRenderableElement)scale).ZOrder - 150;
			marker.EndUpdate();
		}
		public static void InitializeRangeBarDefault(LinearScaleRangeBar rangeBar, LinearScale scale) {
			rangeBar.BeginUpdate();
			rangeBar.LinearScale = scale;
			rangeBar.ZOrder = ((IRenderableElement)scale).ZOrder - 100;
			rangeBar.EndUpdate();
		}
		public static void InitializeStateIndicatorDefault(ScaleStateIndicator indicator, LinearScale scale) {
			indicator.BeginUpdate();
			indicator.IndicatorScale = scale;
			indicator.ZOrder = ((IRenderableElement)scale).ZOrder - 100;
			indicator.States.Add(indicator.CreateState("Default"));
			((IScaleIndicatorState)indicator.States[0]).StartValue = scale.MinValue;
			((IScaleIndicatorState)indicator.States[0]).IntervalLength = scale.ScaleLength;
			indicator.Center = new PointF2D(scale.StartPoint.X, scale.StartPoint.Y + 15);
			indicator.EndUpdate();
		}
	}
	public static class StateIndicatorGaugeExtention {
		public static void InitializeIndicatorDefault(StateIndicator indicator) {
			indicator.BeginUpdate();
			indicator.States.Add(indicator.CreateState("Default"));
			indicator.EndUpdate();
		}
	}
	public static class DigitalGaugeExtention {
		public static void InitializeBackgroundLayerDefault(DigitalBackgroundLayer backgroundLayer, PointF2D bottomRight) {
			backgroundLayer.BeginUpdate();
			backgroundLayer.BottomRight = bottomRight;
			backgroundLayer.ZOrder = 1000;
			backgroundLayer.EndUpdate();
		}
		public static void InitializeEffectLayerDefault(DigitalEffectLayer effectLayer, PointF2D bottomRight) {
			effectLayer.BeginUpdate();
			effectLayer.BottomRight = bottomRight;
			effectLayer.ZOrder = -1000;
			effectLayer.EndUpdate();
		}
	}
}
