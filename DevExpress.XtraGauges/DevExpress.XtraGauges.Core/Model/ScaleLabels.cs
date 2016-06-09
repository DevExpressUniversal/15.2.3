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
using System.Globalization;
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
	public abstract class BaseLabel : BaseShapeProvider, ILabel, ILabelEx {
		string nameCore;
		string formatCore;
		string textCore;
		PointF2D positionCore;
		LabelOrientation orientationCore;
		protected BaseLabel(OwnerChangedAction ownerChanged)
			: base(ownerChanged) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.positionCore = PointF2D.Empty;
			this.formatCore = "{0} {1:F2}";
			this.textCore = "Value:";
		}
		protected override bool UseShapeChanged {
			get { return false; }
		}
		protected override BaseShape CreateShape() {
			TextShape textShape = new TextShape();
			textShape.BeginUpdate();
			textShape.Name = PredefinedShapeNames.Label;
			textShape.Box = new RectangleF2D(-25, -15, 50, 30);
			textShape.Bounds = new RectangleF2D(-25, -15, 50, 30);
			textShape.EndUpdate();
			textShape.Appearance.Changed += OnAppearanceBackgroundChanged;
			textShape.AppearanceText.Changed += OnAppearanceTextChanged;
			return textShape;
		}
		protected override void DestroyShape() {
			if(TextShape != null) {
				TextShape.Appearance.Changed -= OnAppearanceBackgroundChanged;
				TextShape.AppearanceText.Changed -= OnAppearanceTextChanged;
			}
			base.DestroyShape();
		}
		protected void OnAppearanceTextChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceText");
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
				TextShape.Name = value + "TextShape";
			}
		}
		[XtraSerializableProperty, DefaultValue(false), Category("Appearance")]
		public bool AllowHTMLString {
			get { return TextShape.AllowHtmlString; }
			set {
				if(AllowHTMLString == value) return;
				TextShape.AllowHtmlString = value;
				OnObjectChanged("AllowHTMLString");
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TextShape TextShape {
			[System.Diagnostics.DebuggerStepThrough]
			get { return Shape as TextShape; }
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
		[XtraSerializableProperty, Category("Geometry")]
		[DefaultValue(LabelOrientation.Default)]
		public LabelOrientation TextOrientation {
			get { return orientationCore; }
			set {
				if(TextOrientation == value) return;
				orientationCore = value;
				OnObjectChanged("TextOrientation");
			}
		}
		[XtraSerializableProperty, Localizable(true), Category("Appearance")]
		[DefaultValue("Value:")]
		public string Text {
			get { return textCore; }
			set {
				if(Text == value) return;
				textCore = value;
				OnObjectChanged("Text");
			}
		}
		[XtraSerializableProperty, Localizable(true), Category("Appearance")]
		[DefaultValue("{0} {1:F2}")]
		public string FormatString {
			get { return formatCore; }
			set {
				if(FormatString == value) return;
				formatCore = value;
				OnObjectChanged("FormatString");
			}
		}
		[XtraSerializableProperty, Category("Geometry")]
		public SizeF Size {
			get { return TextShape.Box.Size; }
			set {
				if(Size == value) return;
				RectangleF2D rect = new RectangleF2D(-value.Width * 0.5f, -value.Height * 0.5f, value.Width, value.Height);
				TextShape.Box = rect;
				TextShape.Bounds = rect;
				OnObjectChanged("Size");
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceBackground {
			get { return TextShape.Appearance; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseTextAppearance AppearanceText {
			get { return TextShape.AppearanceText; }
		}
		protected override void OnUpdateObjectCore() {
			CalculateTextShape(TextShape);
			base.OnUpdateObjectCore();
		}
		protected override void CopyToCore(BaseObject clone) {
			BaseLabel clonedLabel = clone as BaseLabel;
			if(clonedLabel != null) {
				clonedLabel.nameCore = this.Name;
				clonedLabel.positionCore = this.Position;
				clonedLabel.orientationCore = this.TextOrientation;
				clonedLabel.textCore = this.Text;
				clonedLabel.formatCore = this.FormatString;
				clonedLabel.SetShape(TextShape.Clone());
				clonedLabel.TextShape.Appearance.Changed += clonedLabel.OnAppearanceBackgroundChanged;
				clonedLabel.TextShape.AppearanceText.Changed += clonedLabel.OnAppearanceTextChanged;
			}
		}
		public void Assign(ILabel source) {
			BeginUpdate();
			AssignCore(source);
			EndUpdate();
		}
		public bool IsDifferFrom(ILabel source) {
			return IsDifferFromCore(source);
		}
		protected virtual void AssignCore(ILabel source) {
			if(source != null) {
				this.nameCore = source.Name;
				this.positionCore = source.Position;
				this.orientationCore = source.TextOrientation;
				this.formatCore = source.FormatString;
				this.textCore = source.Text;
				this.TextShape.AppearanceText.Changed -= OnAppearanceTextChanged;
				this.TextShape.Appearance.Changed -= OnAppearanceBackgroundChanged;
				var newShape = source.TextShape.Clone();
				newShape.AppearanceText.Changed += OnAppearanceTextChanged;
				newShape.Appearance.Changed += OnAppearanceBackgroundChanged;
				SetShape(newShape);
			}
		}
		protected virtual bool IsDifferFromCore(ILabel source) {
			return (source == null) ? true :
				(this.nameCore != source.Name) ||
				(this.positionCore != source.Position) ||
				(this.Size != source.Size) ||
				(this.formatCore != source.FormatString) ||
				(this.textCore != source.Text) ||
				(this.AppearanceBackground.IsDifferFrom(source.AppearanceBackground)) ||
				(this.AppearanceText.IsDifferFrom(source.AppearanceText)) ||
				(this.orientationCore != source.TextOrientation);
		}
		internal void ResetSize() {
			Size = new SizeF(50, 30);
		}
		internal bool ShouldSerializeSize() {
			return Size != new SizeF(50, 30);
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
		internal bool ShouldSerializeAppearanceText() {
			return AppearanceText.ShouldSerialize();
		}
		internal void ResetAppearanceText() {
			AppearanceText.Reset();
		}
		protected virtual string CalcDisplayText(string text) {
			return String.Format(FormatString, new object[] { text });
		}
		protected internal virtual void CalculateTextShape(TextShape shape) {
			shape.BeginUpdate();
			shape.Text = CalcDisplayText(Text);
			SizeF textOffsetAbs = new SizeF(1, 0);
			switch(TextOrientation) {
				case LabelOrientation.Tangent:
				case LabelOrientation.BottomToTop:
					textOffsetAbs = new SizeF(0, -1);
					break;
				case LabelOrientation.TopToBottom:
					textOffsetAbs = new SizeF(0, 1);
					break;
			}
			shape.Transform = MathHelper.CalcTransform(Position, Position + textOffsetAbs, new SizeF(1, 1));
			shape.CancelUpdate();
		}
	}
#if !DXPORTABLE
	[Editor("DevExpress.XtraGauges.Design.LabelCollectionEditor, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(UITypeEditor))]
#endif
	[TypeConverter(typeof(LabelCollectionObjectTypeConverter))]
	public abstract class LabelCollection : BaseChangeableList<ILabel>, ISupportAssign<LabelCollection> {
		protected override void OnDispose() {
			DisposeLabels();
			base.OnDispose();
		}
		protected sealed override void OnBeforeElementAdded(ILabel element) {
			string[] names = CollectionHelper.GetNames(this);
			if(String.IsNullOrEmpty(element.Name) || CollectionHelper.NamesContains(element.Name, names)) {
				element.Name = UniqueNameHelper.GetUniqueName("Label", names, names.Length);
			}
			base.OnBeforeElementAdded(element);
		}
		protected override void OnElementAdded(ILabel element) {
			base.OnElementAdded(element);
			BaseLabel label = element as BaseLabel;
			if(label != null) {
				label.Changed += OnElementChanged;
				label.Disposed += OnElementDisposed;
			}
		}
		protected sealed override void OnElementRemoved(ILabel element) {
			BaseLabel label = element as BaseLabel;
			if(label != null) {
				label.Changed -= OnElementChanged;
				label.Disposed -= OnElementDisposed;
			}
			base.OnElementRemoved(element);
		}
		void OnElementDisposed(object sender, EventArgs ea) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<ILabel>(sender as ILabel, ElementChangedType.ElementDisposed));
			if(List != null) Remove(sender as ILabel);
		}
		void OnElementChanged(object sender, EventArgs ea) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<ILabel>(sender as ILabel, ElementChangedType.ElementUpdated));
		}
		public void Assign(LabelCollection labels) {
			DisposeLabels();
			for(int i = 0; i < labels.Count; i++) {
				ILabel label = CreateLabel();
				label.Assign(labels[i]);
				Add(label);
			}
		}
		void DisposeLabels() {
			List<ILabel> exisingLabels = new List<ILabel>(this);
			foreach(ILabel l in exisingLabels) l.Dispose();
			Clear();
		}
		public bool IsDifferFrom(LabelCollection labels) {
			if(labels.Count != Count) return true;
			for(int i = 0; i < labels.Count; i++) {
				if(this[i].IsDifferFrom(labels[i])) return true;
			}
			return false;
		}
		protected internal abstract ILabel CreateLabel();
		protected internal virtual void UpdateLabels() {
			for(int i = 0; i < Count; i++) {
				BaseLabel label = this[i] as BaseLabel;
				if(label != null) label.CalculateTextShape(label.TextShape);
			}
		}
	}
	public class ScaleLabel : BaseLabel, IScaleLabel {
		internal IScale scaleCore;
		float addendCore;
		float multiplierCore;
		public ScaleLabel()
			: base(null) {
				appearanceTextCore = new BaseTextAppearance();
				appearanceTextCore.Changed += OnAppearanceTextChanged;
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.addendCore = 0f;
			this.multiplierCore = 1f;
		}
		protected override void OnDispose() {
			scaleCore = null;
			appearanceTextCore.Changed -= OnAppearanceTextChanged;
			base.OnDispose();
		}
		protected override BaseObject CloneCore() {
			return new ScaleLabel();
		}
		protected override void AssignCore(ILabel source) {
			base.AssignCore(source);
			ScaleLabel label = source as ScaleLabel;
			if(label != null) {
				this.AppearanceText.Assign(source.AppearanceText);
				this.addendCore = label.addendCore;
				this.multiplierCore = label.multiplierCore;
			}
		}
		protected override bool IsDifferFromCore(ILabel source) {
			ScaleLabel label = source as ScaleLabel;
			return base.IsDifferFromCore(source) || ((label == null) ?
					true :
					(this.addendCore != label.addendCore) ||
					(this.multiplierCore != label.multiplierCore)
				);
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			ScaleLabel clonedLabel = clone as ScaleLabel;
			if(clonedLabel != null) {
				clonedLabel.multiplierCore = this.multiplierCore;
				clonedLabel.addendCore = this.addendCore;
			}
		}
		[Browsable(false)]
		public IScale Scale {
			get { return scaleCore; }
		}
		[XtraSerializableProperty, DefaultValue(1f)]
		public float Multiplier {
			get { return multiplierCore; }
			set {
				if(Multiplier == value) return;
				multiplierCore = value;
				OnObjectChanged("Multiplier");
			}
		}
		[XtraSerializableProperty, DefaultValue(0f)]
		public float Addend {
			get { return addendCore; }
			set {
				if(Addend == value) return;
				addendCore = value;
				OnObjectChanged("Addend");
			}
		}
		protected override string CalcDisplayText(string text) {
			float displayedValue = (Scale != null) ? Scale.Value * Multiplier + Addend : 0f;
			float percent = (Scale != null) ? ((IConvertibleScale)Scale).Percent : 0f;
			return String.Format(FormatString, new object[] { text, displayedValue, percent });
		}
		public virtual Color GetActualColor() {
			if(Scale is ILabelColorProvider)
				return (Scale as ILabelColorProvider).GetLabelShemeColor();
			return Color.Empty;
		}
		BaseTextAppearance appearanceTextCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new BaseTextAppearance AppearanceText {
			get { return appearanceTextCore; }
		}
		protected internal override void CalculateTextShape(TextShape shape) {
			if(Scale == null) {
				base.CalculateTextShape(shape);
				return;
			}
			shape.BeginUpdate();
			if(AppearanceText.ShouldSerialize())
				shape.AppearanceText.Assign(AppearanceText);
			if(AppearanceText.TextBrush == BrushObject.Empty) {
				Color actualColor = GetActualColor();
				if(actualColor == Color.Empty)
					actualColor = Color.Black;
				shape.AppearanceText.TextBrush = new SolidBrushObject(actualColor);
			}
			shape.Text = CalcDisplayText(Text);
			SizeF textOffsetAbs = new SizeF(1, 0);
			if(Scale is IArcScale) {
				IArcScale arcScale = Scale as IArcScale;
				textOffsetAbs = new SizeF(Position.X - arcScale.Center.X, Position.Y - arcScale.Center.Y);
				if(textOffsetAbs.Height == 0 && textOffsetAbs.Width == 0) textOffsetAbs = new SizeF(1, 0);
			}
			switch(TextOrientation) {
				case LabelOrientation.BottomToTop:
					textOffsetAbs = new SizeF(0, -1);
					break;
				case LabelOrientation.TopToBottom:
					textOffsetAbs = new SizeF(0, 1);
					break;
				case LabelOrientation.Default:
				case LabelOrientation.LeftToRight:
					textOffsetAbs = new SizeF(1, 0);
					break;
				case LabelOrientation.Radial:
					textOffsetAbs = new SizeF(textOffsetAbs.Width, textOffsetAbs.Height);
					break;
				case LabelOrientation.Tangent:
					textOffsetAbs = new SizeF(textOffsetAbs.Height, -textOffsetAbs.Width);
					break;
			}
			if(textOffsetAbs.Width < 0) textOffsetAbs = new SizeF(-textOffsetAbs.Width, -textOffsetAbs.Height);
			shape.Transform = MathHelper.CalcTransform(Position, Position + textOffsetAbs, new SizeF(1, 1));
			shape.CancelUpdate();
		}
	}
	public class ScaleLabelCollection : LabelCollection {
		IScale scaleCore;
		public ScaleLabelCollection(IScale scale) {
			scaleCore = scale;
			if(Scale != null) 
				Scale.ValueChanged += OnScaleValueChanged;
			IBaseScale baseScale = Scale as IBaseScale;
			if(baseScale != null)
				baseScale.AnimationCompleted += OnScaleAnimationCompleted;
		}
		protected override void OnDispose() {
			IBaseScale baseScale = Scale as IBaseScale;
			if(baseScale != null)
				baseScale.AnimationCompleted -= OnScaleAnimationCompleted;
			if(Scale != null) {
				Scale.ValueChanged -= OnScaleValueChanged;
				scaleCore = null;
			}
			base.OnDispose();
		}
		void OnScaleValueChanged(object sender, EventArgs e) {
			if(Scale != null && !Scale.IsAnimating)
				UpdateLabels();
		}
		void OnScaleAnimationCompleted(object sender, EventArgs e) {
			if(Scale != null)
				UpdateLabels();
		}
		protected internal override ILabel CreateLabel() {
			return new ScaleLabel();
		}
		protected IScale Scale {
			get { return scaleCore; }
		}
		protected override void OnElementAdded(ILabel element) {
			ScaleLabel label = element as ScaleLabel;
			if(label != null) label.scaleCore = Scale;
			base.OnElementAdded(element);
		}
		protected internal override void UpdateLabels() {
			base.UpdateLabels();
			IRenderableElement p = Scale as IRenderableElement;
			if(p != null) {
				for(int i = 0; i < Count; i++) {
					BaseLabel label = this[i] as BaseLabel;
					if(label != null) {
						string key = label.TextShape.Name;
						if(string.IsNullOrEmpty(key)) continue;
						TextShape shape = p.Shapes[key] as TextShape;
						if(shape != null) label.CalculateTextShape(shape);
					}
				}
			}
		}
	}
	public class LabelCollectionObjectTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return destType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			return "<Labels...>";
		}
	}
}
