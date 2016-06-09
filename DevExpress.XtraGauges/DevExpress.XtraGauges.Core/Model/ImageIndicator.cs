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
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
#if !DXPORTABLE
using System.Drawing.Design;
#endif
namespace DevExpress.XtraGauges.Core.Model {
	public class ImageIndicatorProvider : BaseShapeProvider, IImageIndicator {
		string nameCore;
		PointF2D positionCore;
		Image imageCore;
		object stateImagesCore;
		int? stateIndexCore;
		IScale scaleCore;
		Color colorCore;
		ImageLayoutMode imageLayoutModeCore;
		ImageIndicatorStateCollection imageStateCollectionCore;
		public ImageIndicatorProvider(OwnerChangedAction ownerChanged)
			: base(ownerChanged) {
			BeginUpdate();
			Position = new PointF2D(125, 125);
			Size = new PointF2D(32, 32);
			scaleCore = null;
			CancelUpdate();
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.positionCore = PointF2D.Empty;
			this.imageLayoutModeCore = ImageLayoutMode.Default;
			imageStateCollectionCore = new ImageIndicatorStateCollection();
		}
		protected override bool UseShapeChanged {
			get { return false; }
		}
		protected override BaseShape CreateShape() {
			ImageIndicatorShape imageIndicatorShape = new ImageIndicatorShape();
			imageIndicatorShape.BeginUpdate();
			imageIndicatorShape.Name = PredefinedShapeNames.ImageIndicator;
			imageIndicatorShape.Box = new RectangleF2D(-25, -15, 50, 30);
			imageIndicatorShape.Bounds = new RectangleF2D(-25, -15, 50, 30);
			imageIndicatorShape.EndUpdate();
			imageIndicatorShape.Appearance.Changed += OnAppearanceBackgroundChanged;
			return imageIndicatorShape;
		}
		protected override void DestroyShape() {
			if(ImageIndicatorShape != null) {
				ImageIndicatorShape.Appearance.Changed -= OnAppearanceBackgroundChanged;
			}
			base.DestroyShape();
		}
		void OnAppearanceBackgroundChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceBackground");
		}
		int ISupportAcceptOrder.AcceptOrder {
			get { return 0; }
			set { }
		}
		[XtraSerializableProperty]
		public string Name {
			get { return nameCore; }
			set {
				if(nameCore == value) return;
				nameCore = value;
				ImageIndicatorShape.Name = value + "ImageIndicatorShape";
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ImageIndicatorShape ImageIndicatorShape {
			[System.Diagnostics.DebuggerStepThrough]
			get { return Shape as ImageIndicatorShape; }
		}
		[XtraSerializableProperty, Category("Geometry")]
		public PointF2D Position {
			get { return positionCore; }
			set {
				if(Position == value) return;
				positionCore = value;
				OnObjectChanged("Position");
			}
		}
		[XtraSerializableProperty, Category("Image")]
		public Image Image {
			get { return imageCore; }
			set {
				if(Image == value) return;
				if(imageCore != value)
					UpdateImageSize(value);
				imageCore = value;
				OnObjectChanged("Image");
			}
		}
		private void UpdateImageSize(Image value) {
			if(sizeCore != new SizeF(32,32)) return;
			if(value != null)
				SetSize(value.Size);
			else
				Size = new SizeF(32, 32);
		}
		[XtraSerializableProperty, Category("Image")]
		public object StateImages {
			get { return stateImagesCore; }
			set {
				if(StateImages == value) return;
				stateImagesCore = value;
				OnObjectChanged("StateImages");
			}
		}
		[XtraSerializableProperty, Category("Image")]
		public int? StateIndex {
			get { return stateIndexCore; }
			set {
				if(StateIndex == value) return;
				stateIndexCore = value;
				OnObjectChanged("StateIndex");
			}
		}
		[XtraSerializableProperty, Category("Image"), DefaultValue(ImageLayoutMode.Default)]
		public ImageLayoutMode ImageLayoutMode {
			get { return imageLayoutModeCore; }
			set {
				if(ImageLayoutMode == value) return;
				imageLayoutModeCore = value;
				OnObjectChanged("ImageLayoutMode");
			}
		}
		SizeF sizeCore = new SizeF(32, 32);
		[XtraSerializableProperty, Category("Geometry")]
		public SizeF Size {
			get { return ImageIndicatorShape.Box.Size; }
			set {
				if(Size == value) return;
				if(value == new SizeF(0, 0)) return;
				sizeCore = value;
				SetSize(sizeCore);
			}
		}
		void SetSize(SizeF value) {
			RectangleF2D rect = new RectangleF2D(-value.Width * 0.5f, -value.Height * 0.5f, value.Width, value.Height);
			ImageIndicatorShape.Box = rect;
			ImageIndicatorShape.Bounds = rect;
			OnObjectChanged("Size");
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceBackground {
			get { return ImageIndicatorShape.Appearance; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Image")]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public ImageIndicatorStateCollection ImageStateCollection {
			get { return imageStateCollectionCore; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Visible), Category("Image"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Color Color {
			get { return colorCore; }
			set {
				if(colorCore == value || (value == null)) return;
				colorCore = value;
			}
		}
		[XtraSerializableProperty, Category("Image")]
		public IScale IndicatorScale {
			get { return scaleCore; }
			set {
				if(scaleCore == value || (value == null)) return;
				SetScaleCore(value);
				PerformOwnerChanged("IndicatorScale");
			}
		}
		void SetScaleCore(IScale value) {
			UnsubscribeScaleEvents();
			this.scaleCore = value;
			SubscribeScaleEvents();
		}
		protected override void OnDispose() {
			UnsubscribeScaleEvents();
			scaleCore = null;
			base.OnDispose();
		}
		void SubscribeScaleEvents() {
			if(IndicatorScale != null) IndicatorScale.ValueChanged += OnScaleValueChanged;
		}
		void UnsubscribeScaleEvents() {
			if(IndicatorScale != null) IndicatorScale.ValueChanged -= OnScaleValueChanged;
		}
		protected void OnScaleValueChanged(object sender, EventArgs ea) {
			if(IsDisposing) return;
			RaiseChanged(ea);
		}
		protected override void OnUpdateObjectCore() {
			CalculateImageIndicatorShape(ImageIndicatorShape);
			base.OnUpdateObjectCore();
		}
		protected override void CopyToCore(BaseObject clone) {
			ImageIndicatorProvider clonedImageIndicator = clone as ImageIndicatorProvider;
			if(clonedImageIndicator != null) {
				clonedImageIndicator.Name = this.Name;
				clonedImageIndicator.Image = this.Image;
				clonedImageIndicator.StateImages = this.StateImages;
				clonedImageIndicator.StateIndex = this.StateIndex;
				clonedImageIndicator.ImageLayoutMode = this.ImageLayoutMode;
				clonedImageIndicator.AssignStates(this.ImageStateCollection);
				clonedImageIndicator.Position = this.Position;
				clonedImageIndicator.SetShape(ImageIndicatorShape.Clone());
				clonedImageIndicator.ImageIndicatorShape.Appearance.Changed += clonedImageIndicator.OnAppearanceBackgroundChanged;
			}
		}
		public void Assign(IImageIndicator source) {
			BeginUpdate();
			AssignCore(source);
			EndUpdate();
		}
		public bool IsDifferFrom(IImageIndicator source) {
			return IsDifferFromCore(source);
		}
		protected virtual void AssignCore(IImageIndicator source) {
			if(source != null) {
				this.nameCore = source.Name;
				this.positionCore = source.Position;
				this.imageCore = source.Image;
				this.stateImagesCore = source.StateImages;
				this.colorCore = source.Color;
				this.stateIndexCore = source.StateIndex;
				this.imageLayoutModeCore = source.ImageLayoutMode;
				this.sizeCore = source.Size;
				AssignStates(source.ImageStateCollection);
				this.IndicatorScale = source.IndicatorScale;
				SetShape(source.ImageIndicatorShape.Clone());
			}
		}
		public void AssignStates(ImageIndicatorStateCollection states) {
			ImageStateCollection.Clear();
			for(int i = 0; i < states.Count; i++) {
				ImageIndicatorState state = CreateState(states[i].Name);
				state.Assign(states[i]);
				ImageStateCollection.Add(state);
			}
		}
		ImageIndicatorState CreateState(string name) {
			ImageIndicatorState state = new ImageIndicatorState();
			state.Name = name;
			return state;
		}
		protected virtual bool IsDifferFromCore(IImageIndicator source) {
			return (source == null) ? true :
				(this.nameCore != source.Name) ||
				(this.positionCore != source.Position) ||
				(this.imageCore != source.Image) ||
				(this.stateImagesCore != source.StateImages) ||
				(this.stateIndexCore != source.StateIndex) ||
				(this.ImageStateCollection != source.ImageStateCollection) ||
				(this.imageLayoutModeCore != source.ImageLayoutMode) ||
				(this.colorCore != source.Color) ||
				(this.Size != source.Size) ||
				(this.AppearanceBackground.IsDifferFrom(source.AppearanceBackground));
		}
		internal bool ShouldSerializeImage() {
			return imageCore != null;
		}
		internal void ResetStateImages() {
			stateImagesCore = null;
		}
		internal bool ShouldSerializeStateImages() {
			return stateImagesCore != null;
		}
		internal void ResetStateIndex() {
			stateIndexCore = null;
		}
		internal bool ShouldSerializeStateIndex() {
			return stateIndexCore.HasValue;
		}
		internal bool ShouldSerializeSize() {
			return sizeCore != new SizeF(32, 32);
		}
		internal void ResetSize() {
			sizeCore = new SizeF(32, 32);
			SetSize(new SizeF(32, 32));
		}
		internal bool ShouldSerializeImageLayoutMode() {
			return ImageLayoutMode != ImageLayoutMode.Default;
		}
		internal void ResetImageLayoutMode() {
			ImageLayoutMode = ImageLayoutMode.Default;
		}
		internal void ResetPosition() {
			Position = PointF2D.Empty;
		}
		internal bool ShouldSerializePosition() {
			return Position != PointF2D.Empty;
		}
		internal bool ShouldSerializeAppearanceBackground() {
			return AppearanceBackground.ShouldSerialize();
		}
		internal void ResetAppearanceBackground() {
			AppearanceBackground.Reset();
		}
		internal void ResetImageStateCollection() {
			imageStateCollectionCore = null;
		}
		internal bool ShouldSerializeImageStateCollection() {
			return imageStateCollectionCore != null;
		}
		internal bool ShouldSerializeColor() {
			return colorCore != Color.Empty;
		}
		internal void ResetColor() {
			colorCore = Color.Empty;
		}
		protected internal virtual void CalculateImageIndicatorShape(ImageIndicatorShape shape) {
			shape.BeginUpdate();
			shape.Image = Image;
			shape.ImageLayoutMode = ImageLayoutMode;
			SizeF imageOffsetAbs = new SizeF(1, 0);
			shape.Transform = MathHelper.CalcTransform(Position, Position + imageOffsetAbs, new SizeF(1, 1));
			shape.CancelUpdate();
		}
	}
	public class ImageIndicator : ScaleIndependentLayerComponent<ImageIndicatorProvider>,
	   ISupportAssign<ImageIndicator>, IImageIndicator {
		public ImageIndicator() : base() { }
		public ImageIndicator(string name) : base(name) { }
		protected override ImageIndicatorProvider CreateProvider() {
			return new ImageIndicatorProvider(OnScaleIndependentComponentChanged);
		}
		protected override bool AllowCacheRenderOperation {
			get { return false; }
		}
		protected override void CalculateScaleIndependentComponent() {
			string key = Provider.ImageIndicatorShape.Name;
			if(!string.IsNullOrEmpty(key)) {
				ImageIndicatorShape shape = Shapes[key] as ImageIndicatorShape;
				if(shape != null) {
					Provider.Image = GetColoredImage(Image);
					Provider.CalculateImageIndicatorShape(shape);
				}
			}
		}
		protected virtual Image GetColoredImage(Image image) { return image; }
		[ XtraSerializableProperty]
		public PointF2D Position {
			get { return Provider.Position; }
			set { Provider.Position = value; }
		}
		[ XtraSerializableProperty]
		public SizeF Size {
			get { return Provider.Size; }
			set { Provider.Size = value; }
		}
		Image imageCore;
		[
		XtraSerializableProperty]
		public Image Image {
			get { 
				if(imageCore != null)
					return imageCore;
				return Provider.Image;
			}
			set {
				if(imageCore == null && value == null)
					return;
				imageCore = value;
				if(imageCore == null)
					UpdateActualImage();
				else
					SetActualImage(value);
			}
		}
		protected virtual void UpdateActualImage() { Update(); }
		protected internal bool ShouldSerializeImage() {
			return imageCore != null;
		}
		internal void ResetImage() {
			imageCore = null;
			ImageIndicatorShape.Image = null;
			UpdateActualImage();
		}
		protected virtual void SetActualImage(Image value) {
			if(imageCore != null)
				Provider.Image = imageCore;
			else
				Provider.Image = value;
		}
		[
		XtraSerializableProperty, DefaultValue(ImageLayoutMode.Default)]
		public ImageLayoutMode ImageLayoutMode {
			get { return Provider.ImageLayoutMode; }
			set { Provider.ImageLayoutMode = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceBackground {
			get { return Provider.AppearanceBackground; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ImageIndicatorShape ImageIndicatorShape {
			get { return Provider.ImageIndicatorShape; }
		}
		bool allowImageSkinningCore;
		[XtraSerializableProperty, Category("Image")]
		[DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool AllowImageSkinning {
			get { return allowImageSkinningCore; }
			set {
				if(allowImageSkinningCore == value) return;
				allowImageSkinningCore = value;
				CalculateScaleIndependentComponent();
				OnObjectChanged("AllowImageSkinning");
			}
		}
		public void Assign(ImageIndicator source) {
			BeginUpdate();
			if(source != null) {
				allowImageSkinningCore = source.AllowImageSkinning;
				imageCore = source.imageCore;
				AssignPrimitiveProperties(source);
				Provider.Assign(source);
			}
			EndUpdate();
		}
		public bool IsDifferFrom(ImageIndicator source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) || AllowImageSkinning != source.AllowImageSkinning ||
					Provider.IsDifferFrom(source);
		}
		public void Assign(IImageIndicator source) {
			Provider.Assign(source);
		}
		public bool IsDifferFrom(IImageIndicator source) {
			return Provider.IsDifferFrom(source);
		}
		[Browsable(false), Bindable(false)]
		[XtraSerializableProperty]
		public override string Name {
			get { return base.Name; }
			set {
				if(base.Name == value) return;
				base.Name = value;
				Provider.Name = Name;
			}
		}
		internal bool ShouldSerializeAppearanceBackground() {
			return Provider.ShouldSerializeAppearanceBackground();
		}
		internal bool ShouldSerializeImageLayoutMode() {
			return ImageLayoutMode != ImageLayoutMode.Default;
		}
		internal void ResetImageLayoutMode() {
			ImageLayoutMode = ImageLayoutMode.Default;
		}
		internal bool ShouldSerializeSize() {
			return (Image != null) ? Size != Image.Size : Size != new SizeF(32, 32);
		}
		internal void ResetSize() {
			if(Image != null)
				Size = Image.Size;
			else
				Size = new SizeF(32, 32);
		}
		internal bool ShouldSerializePosition() {
			return Position != new PointF2D(125, 125);
		}
		internal void ResetPosition() {
			Position = new PointF2D(125, 125);
		}
		#region IImageIndicator Members
		object IImageIndicator.StateImages {
			get { return Provider.StateImages; }
			set { }
		}
		int? IImageIndicator.StateIndex {
			get { return Provider.StateIndex; }
			set { }
		}
		IScale IImageIndicator.IndicatorScale {
			get { return Provider.IndicatorScale; }
			set { }
		}
		ImageIndicatorStateCollection IImageIndicator.ImageStateCollection {
			get { return Provider.ImageStateCollection; }
		}
		Color IImageIndicator.Color {
			get { return Provider.Color; }
			set { }
		}
		#endregion
	}
	public class ImageIndicatorState : ISupportAcceptOrder, INamed, IDisposable {
		float startValueCore;
		float intervalLengthCore;
		int stateIndexCore;
		string nameCore;
		public event EventHandler Changed;
		public event EventHandler Disposed;
		public ImageIndicatorState() {
			OnCreate();
		}
		public ImageIndicatorState(string name) {
			OnCreate();
			Name = name;
		}
		[XtraSerializableProperty, DefaultValue(0F)]
		public float StartValue {
			get { return startValueCore; }
			set {
				if(value == startValueCore) return;
				startValueCore = value;
				RaiseChanged();
			}
		}
		[XtraSerializableProperty, DefaultValue(1.0F)]
		public float IntervalLength {
			get { return intervalLengthCore; }
			set {
				if(value == intervalLengthCore) return;
				intervalLengthCore = value;
				RaiseChanged();
			}
		}
		[XtraSerializableProperty, DefaultValue(0)]
		public int StateIndex {
			get { return stateIndexCore; }
			set {
				if(value == stateIndexCore) return;
				stateIndexCore = value;
				RaiseChanged();
			}
		}
		[XtraSerializableProperty, DefaultValue("")]
		public string Name {
			get { return nameCore; }
			set {
				if(value == null || value == nameCore) return;
				nameCore = value;
				RaiseChanged();
			}
		}
		public void Assign(ImageIndicatorState state) {
			stateIndexCore = state.StateIndex;
			intervalLengthCore = state.IntervalLength;
			startValueCore = state.StartValue;
		}
		void OnCreate() {
			stateIndexCore = 0;
			intervalLengthCore = 1.0F;
			startValueCore = 0F;
		}
		public void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		public void RaiseDisposed() {
			if(Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		#region ISupportAcceptOrder Members
		int ISupportAcceptOrder.AcceptOrder {
			get { return String.IsNullOrEmpty(Name) ? 0 : (int)((char)Name[0]); }
			set { }
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			RaiseDisposed();
		}
		#endregion
	}
#if !DXPORTABLE
	[Editor("DevExpress.XtraGauges.Design.ImageIndicatorStateCollectionEditor, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(UITypeEditor))]
#endif
	[TypeConverter(typeof(IndicatorStateCollectionObjectTypeConverter))]
	public class ImageIndicatorStateCollection : BaseChangeableList<ImageIndicatorState> {
		public int IndexOf(ImageIndicatorState state) {
			return List.IndexOf(state);
		}
		public bool Contains(string name) {
			string[] names = CollectionHelper.GetNames(this);
			return CollectionHelper.NamesContains(name, names);
		}
		protected override void OnBeforeElementAdded(ImageIndicatorState element) {
			string[] names = CollectionHelper.GetNames(this);
			if(element != null && !CollectionHelper.NamesContains(element.Name, names)) {
				element.Name = UniqueNameHelper.GetUniqueName("State", names, names.Length + 1);
			}
		}
		protected override void OnElementAdded(ImageIndicatorState state) {
			if(state != null) {
				state.Changed += OnElementChanged;
				state.Disposed += OnElementDisposed;
			}
		}
		protected override void OnElementRemoved(ImageIndicatorState state) {
			if(state != null) {
				state.Disposed -= OnElementDisposed;
				state.Changed -= OnElementChanged;
			}
		}
		void OnElementChanged(object sender, EventArgs ea) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<ImageIndicatorState>(sender as ImageIndicatorState, ElementChangedType.ElementUpdated));
		}
		void OnElementDisposed(object sender, EventArgs ea) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<ImageIndicatorState>(sender as ImageIndicatorState, ElementChangedType.ElementDisposed));
			if(List != null) Remove(sender as ImageIndicatorState);
		}
	}
}
