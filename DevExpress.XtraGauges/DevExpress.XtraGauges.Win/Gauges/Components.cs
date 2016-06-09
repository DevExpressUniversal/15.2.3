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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.XtraGauges.Win.Customization;
using DevExpress.XtraGauges.Win.Data;
using DevExpress.XtraGauges.Win.Gauges;
using DevExpress.XtraGauges.Win.Gauges.Circular;
using DevExpress.XtraGauges.Win.Wizard;
namespace DevExpress.XtraGauges.Win.Base {
	public class LabelComponentCollection :
		ComponentCollection<LabelComponent> {
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.LabelComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class LabelComponent : DevExpress.XtraGauges.Core.Model.Label, ISupportInitialize, IBindableComponent, ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<LabelComponent> {
		BaseBindableProvider bindableProviderCore;
		bool useColorSchemeCore;
		public LabelComponent()
			: base() {
		}
		public LabelComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
			useColorSchemeCore = true;
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			EndUpdate();
		}
		protected BaseTextAppearance GetActualAppearanceText() {
			FrozenTextAppearance target = new FrozenTextAppearance();
			ShapeAppearanceHelper.Combine(target, AppearanceText, GetDefaultAppearance());
			return target;
		}
		FrozenTextAppearance GetDefaultAppearance() {
			FrozenTextAppearance result = new FrozenTextAppearance();
			IGauge owner = GetOwner();
			if(owner != null) {
				GaugeControlBase gaugeControl = owner.Container as GaugeControlBase;
				if(gaugeControl == null) return result;
				AppearanceDefault defaultAppearanceText = gaugeControl.DefaultAppearanceText;
				ShapeAppearanceHelper.Init(result, defaultAppearanceText);
				ColorScheme scheme = owner.GetColorScheme();
				if(UseColorScheme && scheme.TargetElements.HasFlag(TargetElement.Label) && scheme.Color != Color.Empty) {
					result.TextBrush = BrushesCache.GetBrushByColor(scheme.Color);
				}
			}
			return result;
		}
		protected override void ShapeProcessing(BaseShape shape) {
			IGauge owner = GetOwner();
			if(owner != null) {
			   (shape as TextShape).AppearanceText.Assign(GetActualAppearanceText());
			}
		}
		bool CanUseControlSheme {
			get {
				IGauge owner = GetOwner();
				ColorScheme scheme = null;
				if(owner != null) 
					scheme = owner.GetColorScheme();
				return scheme != null && UseColorScheme && (scheme.TargetElements & TargetElement.Label) != 0 && scheme.Color != Color.Empty;
			}
		}
		IGauge GetOwner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model != null)
				return model.Owner;
			return null;
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new MoveFrame(this),
				new SelectionFrame(this), 
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get {
				PointF[] pts = MathHelper.ModelPointsToPoints(this, new PointF[] { 
						new PointF(Position.X - Size.Width * 0.5f, Position.Y - Size.Height * 0.5f),
						new PointF(Position.X + Size.Width * 0.5f, Position.Y + Size.Height * 0.5f)
						}
					);
				return new Rectangle(Point.Round(pts[0]), new Size((int)(pts[1].X - pts[0].X), (int)(pts[1].Y - pts[0].Y)));
			}
			set {
				TextShape.BeginUpdate();
				PointF pt = MathHelper.PointToModelPoint(this, MathHelper.GetCenterOfRect(value));
				this.Position = new PointF2D((float)Math.Round((double)pt.X, 1), (float)Math.Round((double)pt.Y, 1));
				TextShape.EndUpdate(); 
				Self.SetViewInfoDirty();
				Self.ResetCache();
				RaiseChanged(EventArgs.Empty);
			}
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open Labels Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[0])
			};
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(LabelComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			BaseDesignerElementVisualizerHelpers.DrawBoundsDesignerElements(g);
		}
		void ISupportAssign<LabelComponent>.Assign(LabelComponent label) {
			this.useColorSchemeCore = label.UseColorScheme;
			Assign(label);
		}
		bool ISupportAssign<LabelComponent>.IsDifferFrom(LabelComponent label) {
			return useColorSchemeCore != label.UseColorScheme  || IsDifferFrom(label);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<LabelComponent>(9,"Labels", UIHelper.CircularGaugeElementImages[0],new LabelComponent[]{this},model.Owner) 
				};
				designerform.ShowDialog();
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LabelComponentDataBindings"),
#endif
ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
		[XtraSerializableProperty, Category("Appearance"), DefaultValue(true)]
		public bool UseColorScheme {
			get { return useColorSchemeCore; }
			set {
				if(useColorSchemeCore == value) return;
				useColorSchemeCore = value;
				OnObjectChanged("UseColorScheme");
			}
		}
		protected override void OnUpdateObjectCore() { Update(); }
	}
	public class ImageIndicatorComponentCollection : ComponentCollection<ImageIndicatorComponent> { }
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.ImageIndicatorComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class ImageIndicatorComponent : DevExpress.XtraGauges.Core.Model.ImageIndicator, ISupportInitialize, IBindableComponent, ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<ImageIndicatorComponent> {
		BaseBindableProvider bindableProviderCore;
		public ImageIndicatorComponent()
			: base() {
		}
		public ImageIndicatorComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			UpdateActualImage();
			EndUpdate();
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new MoveFrame(this),
				new SelectionFrame(this), 
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get {
				PointF[] pts = MathHelper.ModelPointsToPoints(this, new PointF[] { 
						new PointF(Position.X - Size.Width * 0.5f, Position.Y - Size.Height * 0.5f),
						new PointF(Position.X + Size.Width * 0.5f, Position.Y + Size.Height * 0.5f)
						}
					);
				return new Rectangle(Point.Round(pts[0]), new Size((int)(pts[1].X - pts[0].X), (int)(pts[1].Y - pts[0].Y)));
			}
			set {
				ImageIndicatorShape.BeginUpdate();
				PointF pt = MathHelper.PointToModelPoint(this, MathHelper.GetCenterOfRect(value));
				this.Position = new PointF2D((float)Math.Round((double)pt.X, 1), (float)Math.Round((double)pt.Y, 1));
				ImageIndicatorShape.EndUpdate();
				Self.SetViewInfoDirty();
				Self.ResetCache();
				RaiseChanged(EventArgs.Empty);
			}
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open ImageIndicators Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[0])
			};
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(ImageIndicatorComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			BaseDesignerElementVisualizerHelpers.DrawBoundsDesignerElements(g);
		}
		void ISupportAssign<ImageIndicatorComponent>.Assign(ImageIndicatorComponent imageIndicator) {
			Assign(imageIndicator);
		}
		bool ISupportAssign<ImageIndicatorComponent>.IsDifferFrom(ImageIndicatorComponent imageIndicator) {
			return IsDifferFrom(imageIndicator);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<ImageIndicatorComponent>(9, "Images", UIHelper.CircularGaugeElementImages[0], new ImageIndicatorComponent[] {this}, model.Owner) 
				};
				designerform.ShowDialog();
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ImageIndicatorComponentDataBindings"),
#endif
		ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
		protected override Image GetColoredImage(Image image) {
			Color color = Color.Empty;
			if(AllowImageSkinning && image != null) {
				if(Color != Color.Empty) {
					color = Color;
				}
				else {
					color = (GetDefaultAppearance().ContentBrush as SolidBrushObject).Color;
				}
			}
			Color schemeColor = GetColorFromScheme(TargetElement.ImageIndicator);
			if(!schemeColor.IsEmpty) {
				color = schemeColor;
			}
			if(!color.IsEmpty)
				return Utils.Helpers.ColoredImageHelper.GetColoredImage(image, color);
			return image;
		}
		protected Color GetColorFromScheme(TargetElement element) {
			IGauge owner = GetOwner();
			if(owner != null) {
				ColorScheme scheme = owner.GetColorScheme();
				if((scheme.TargetElements & element) != 0 && scheme.Color != Color.Empty)
					return scheme.Color;
			}
			return Color.Empty;
		}
		RangeBarAppearance GetDefaultAppearance() {
			FrozenRangeBarAppearance result = new FrozenRangeBarAppearance();
			IGauge owner = GetOwner();
			if(owner != null && owner.Container is GaugeControlBase) {
				GaugeControlBase gaugeControl = owner.Container as GaugeControlBase;
				AppearanceDefault defaultAppearanceRangeBar = gaugeControl.DefaultAppearanceRangeBar;
				ShapeAppearanceHelper.Init(result, defaultAppearanceRangeBar);
			}
			return result;
		}
		IGauge GetOwner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model != null)
				return model.Owner;
			return null;
		}
		[XtraSerializableProperty, Category("Image")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Color Color {
			get { return Provider.Color; }
			set {
				if(Color == value) return;
				Provider.Color = value;
				OnObjectChanged("Color");
				UpdateActualImage();
			}
		}
		protected override void OnUpdateObjectCore() {
			UpdateActualImage();
			base.OnUpdateObjectCore();
		}
		protected override void UpdateActualImage() {
			SetActualImage(null);
		}
		internal void ResetColor() {
			Color = Color.Empty;
		}
		internal bool ShouldSerializeColor() {
			return Color != Color.Empty;
		}
	}
	public class StateImageIndicatorComponentCollection : ComponentCollection<StateImageIndicatorComponent> { }
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.StateImageIndicatorComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class StateImageIndicatorComponent : DevExpress.XtraGauges.Core.Model.ImageIndicator, ISupportInitialize, IBindableComponent, ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<StateImageIndicatorComponent>, IStateImageIndicator {
		BaseBindableProvider bindableProviderCore;
		public StateImageIndicatorComponent()
			: base() {
		}
		public StateImageIndicatorComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
			if(Provider.ImageStateCollection != null)
				Provider.ImageStateCollection.CollectionChanged += OnImageStateCollectionChanged;
		}
		private void OnImageStateCollectionChanged(Core.Base.CollectionChangedEventArgs<ImageIndicatorState> ea) {
			UpdateActualImage();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			if(Provider.IndicatorScale != null)
				Provider.IndicatorScale.ValueChanged -= OnIndicatorScaleValueChanged;
			if(Provider.ImageStateCollection != null)
				Provider.ImageStateCollection.CollectionChanged -= OnImageStateCollectionChanged;
			base.OnDispose();
		}
		void OnIndicatorScaleValueChanged(object sender, EventArgs e) {
			if(ImageStateCollection != null && StateImages != null && IndicatorScale != null) {
				UpdateActualImage();
				CalculateScaleIndependentComponent();
			}
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			UpdateActualImage();
			EndUpdate();
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new MoveFrame(this),
				new SelectionFrame(this), 
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get {
				PointF[] pts = MathHelper.ModelPointsToPoints(this, new PointF[] { 
						new PointF(Position.X - Size.Width * 0.5f, Position.Y - Size.Height * 0.5f),
						new PointF(Position.X + Size.Width * 0.5f, Position.Y + Size.Height * 0.5f)
						}
					);
				return new Rectangle(Point.Round(pts[0]), new Size((int)(pts[1].X - pts[0].X), (int)(pts[1].Y - pts[0].Y)));
			}
			set {
				ImageIndicatorShape.BeginUpdate();
				PointF pt = MathHelper.PointToModelPoint(this, MathHelper.GetCenterOfRect(value));
				this.Position = new PointF2D((float)Math.Round((double)pt.X, 1), (float)Math.Round((double)pt.Y, 1));
				ImageIndicatorShape.EndUpdate();
				Self.SetViewInfoDirty();
				Self.ResetCache();
				RaiseChanged(EventArgs.Empty);
			}
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open StateImageIndicators Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[0])
			};
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(StateImageIndicatorComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			BaseDesignerElementVisualizerHelpers.DrawBoundsDesignerElements(g);
		}
		void ISupportAssign<StateImageIndicatorComponent>.Assign(StateImageIndicatorComponent imageIndicator) {
			Assign(imageIndicator);
		}
		bool ISupportAssign<StateImageIndicatorComponent>.IsDifferFrom(StateImageIndicatorComponent imageIndicator) {
			return (IsDifferFrom(imageIndicator));
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<StateImageIndicatorComponent>(9, "ImageIndicators", UIHelper.CircularGaugeElementImages[0], new StateImageIndicatorComponent[] {this}, model.Owner) 
				};
				designerform.ShowDialog();
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("StateImageIndicatorComponentDataBindings"),
#endif
		ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
		protected override Image GetColoredImage(Image image) {
			IGauge owner = GetOwner();
			ColorScheme scheme = owner != null ? owner.GetColorScheme() : null;
			if(image != null) {
				if(scheme != null && (scheme.TargetElements & TargetElement.ImageIndicator) != 0) {
					scheme = (owner.Container as GaugeControlBase).ColorScheme;
					if(scheme.TargetElements.HasFlag(TargetElement.ImageIndicator) && scheme.Color != Color.Empty)
						return Utils.Helpers.ColoredImageHelper.GetColoredImage(image, scheme.Color);
				}
				if(AllowImageSkinning) {
					if(Color != Color.Empty) {
						return Utils.Helpers.ColoredImageHelper.GetColoredImage(image, Color);
					}
					else {
						BaseShapeAppearance defaultAppearance = GetDefaultAppearance();
						return Utils.Helpers.ColoredImageHelper.GetColoredImage(image, (defaultAppearance.ContentBrush as SolidBrushObject).Color);
					}
				}
			}
			return image;
		}
		RangeBarAppearance GetDefaultAppearance() {
			FrozenRangeBarAppearance result = new FrozenRangeBarAppearance();
			IGauge owner = GetOwner();
			if(owner != null && owner.Container is GaugeControlBase) {
				GaugeControlBase gaugeControl = owner.Container as GaugeControlBase;
				ColorScheme scheme = owner.GetColorScheme();
				AppearanceDefault defaultAppearanceRangeBar = gaugeControl.DefaultAppearanceRangeBar;
				if((scheme.TargetElements & TargetElement.ImageIndicator) != 0 & scheme.Color != Color.Empty) 
					defaultAppearanceRangeBar.ForeColor = scheme.Color;
				if(defaultAppearanceRangeBar != null)
					ShapeAppearanceHelper.Init(result, defaultAppearanceRangeBar);
			}
			return result;
		}
		IGauge GetOwner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model != null)
				return model.Owner;
			return null;
		}
		protected Image GetImageState(float val) {
			foreach(ImageIndicatorState state in ImageStateCollection) {
				float min = Math.Min(state.StartValue, state.StartValue + state.IntervalLength);
				float max = Math.Max(state.StartValue, state.StartValue + state.IntervalLength);
				if(val >= min && val <= max) {
					return ImageCollection.GetImageListImage(StateImages, state.StateIndex);
				}
			}
			return null;
		}
		[XtraSerializableProperty, Category("Image")]
		[DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public int? StateIndex {
			get { return Provider.StateIndex; }
			set {
				if(StateIndex == value) return;
				Provider.StateIndex = value;
				OnObjectChanged("StateIndex");
			}
		}
		[XtraSerializableProperty, Category("Image"), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		[DefaultValue(null)]
		public object StateImages {
			get { return Provider.StateImages; }
			set {
				if(StateImages == value) return;
				Provider.StateImages = value;
				OnObjectChanged("StateImages");
			}
		}
		[XtraSerializableProperty, Category("Image")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Color Color {
			get { return Provider.Color; }
			set {
				if(Color == value) return;
				Provider.Color = value;
				OnObjectChanged("Color");
				UpdateActualImage();
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true), Category("Image")]
		[DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageIndicatorStateCollection ImageStateCollection {
			get { return Provider.ImageStateCollection; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateImageStateCollectionItem(XtraItemEventArgs e) {
			XtraPropertyInfo propInfo = e.Item;
			ImageIndicatorStateCollection collection = e.Collection as ImageIndicatorStateCollection;
			if(propInfo == null || collection == null) return null;
			BeginUpdate();
			string stateName = (string)propInfo.ChildProperties["Name"].Value;
			ImageIndicatorState state = CreateState(stateName);
			if(!collection.Contains(stateName)) collection.Add(state);
			CancelUpdate();
			return state;
		}
		ImageIndicatorState CreateState(string stateName) {
			return new ImageIndicatorState();
		}
		[XtraSerializableProperty, Category("Image")]
		[DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public IScale IndicatorScale {
			get { return Provider.IndicatorScale; }
			set {
				if(Provider.IndicatorScale != null)
					Provider.IndicatorScale.ValueChanged -= OnIndicatorScaleValueChanged;
				Provider.IndicatorScale = value;
				if(Provider.IndicatorScale != null)
					Provider.IndicatorScale.ValueChanged += OnIndicatorScaleValueChanged;
			}
		}
		protected override void UpdateActualImage() {
			if(StateImages != null && StateIndex.HasValue) {
				SetActualImage(ImageCollection.GetImageListImage(StateImages, (int)StateIndex));
				return;
			}
			if(!StateIndex.HasValue && StateImages != null) {
				SetActualImage(GetImageState(Provider.IndicatorScale.Value));
				return;
			}
			SetActualImage(null);
		}
		protected override void OnUpdateObjectCore() {
			UpdateActualImage();
			base.OnUpdateObjectCore();
		}
		internal void ResetColor() {
			Color = Color.Empty;
		}
		internal bool ShouldSerializeColor() {
			return Color != Color.Empty;
		}
		internal void ResetStateIndex() {
			StateIndex = null;
		}
		internal bool ShouldSerializeStateIndex() {
			return StateIndex.HasValue;
		}
		internal void ResetStateImages() {
			StateImages = null;
		}
		internal bool ShouldSerializeStateImages() {
			return StateImages != null;
		}
		internal bool ShouldSerializeImageStateCollection() {
			return ImageStateCollection != null;
		}
		#region IScaleComponent Members
		IScale IScaleComponent.Scale {
			get { return IndicatorScale; }
		}
		#endregion
	}
}
