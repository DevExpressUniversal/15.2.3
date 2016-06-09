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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraGauges.Core.Model {
	public abstract class BaseScaleTickMarkProvider : BaseShapeProvider<TickmarkShapeType>, ISupportAcceptOrder {
		string nameCore;
		FactorF2D scaleCore;
		float shapeOffsetCore;
		PointF2D originCore;
		PointF2D orientationPointCore;
		bool showTickCore;
		bool showFirstCore;
		bool showLastCore;
		bool fIsTemplateCore;
		float shapeHeight;
		protected BaseScaleTickMarkProvider(bool createShape)
			: base(null, createShape) {
		}
		public BaseScaleTickMarkProvider()
			: this(true) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.showTickCore = true;
			this.showFirstCore = true;
			this.showLastCore = true;
			this.fIsTemplateCore = true;
			this.shapeOffsetCore = 0;
			this.scaleCore = new FactorF2D(1f, 1f);
			this.orientationPointCore = new PointF2D(1f, 0f);
		}
		protected override TickmarkShapeType DefaultShapeType {
			get { return TickmarkShapeType.Default; }
		}
		protected override BaseShape GetShape(TickmarkShapeType value) {
			return TickmarkShapeFactory.GetDefaultTickmarkShape(value);
		}
		int ISupportAcceptOrder.AcceptOrder {
			get { return 0; }
			set { }
		}
		protected bool IsTemplate {
			get { return fIsTemplateCore; }
		}
		public bool IsNameLocked {
			get { return false; }
		}
		protected void UpdateShape() {
			BaseShape newShape = CreateShape();
			newShape.AssignInternal(Shape);
			SetShape(newShape);
		}
		protected override void SetShapeTypeCore(TickmarkShapeType value) {
			var shape = GetShape(value);
			shape.AssignInternal(Shape);
			base.SetShapeTypeCore(value, shape);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Name {
			get { return nameCore; }
			set { nameCore = value; }
		}
		[DefaultValue(true)]
		[XtraSerializableProperty]
		public bool ShowTick {
			get { return showTickCore; }
			set {
				if(ShowTick == value) return;
				showTickCore = value;
				OnObjectChanged("ShowTick");
			}
		}
		[DefaultValue(true)]
		[XtraSerializableProperty]
		public bool ShowFirst {
			get { return showFirstCore; }
			set {
				if(ShowFirst == value) return;
				showFirstCore = value;
				OnObjectChanged("ShowFirst");
			}
		}
		[DefaultValue(true)]
		[XtraSerializableProperty]
		public bool ShowLast {
			get { return showLastCore; }
			set {
				if(ShowLast == value) return;
				showLastCore = value;
				OnObjectChanged("ShowLast");
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PointF2D Origin {
			get { return originCore; }
			set { originCore = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PointF2D Orientation {
			get { return orientationPointCore; }
			set { orientationPointCore = value; }
		}
		[XtraSerializableProperty]
		public FactorF2D ShapeScale {
			get { return scaleCore; }
			set {
				if(ShapeScale == value) return;
				scaleCore = value;
				OnObjectChanged("ShapeScale");
			}
		}
		[DefaultValue(0f)]
		[XtraSerializableProperty]
		public float ShapeOffset {
			get { return shapeOffsetCore; }
			set {
				if(ShapeOffset == value) return;
				shapeOffsetCore = value;
				OnObjectChanged("ShapeOffset");
			}
		}
		protected override void RaiseChanged(EventArgs e) {
			if(IsTemplate) base.RaiseChanged(e);
		}
		protected override void OnUpdateObjectCore() {
			if(IsTemplate || Shape.IsEmpty) return;
			if(!Origin.IsEmpty && !Orientation.IsEmpty)
				OnTransformTickmarkShapes();
		}
		protected virtual void OnTransformTickmarkShapes() {
			float l = (float)MathHelper.CalcVectorLength(Origin, Orientation);
			float sx = -ShapeOffset / l;
			float sy = -0.5f * shapeHeight * ShapeScale.XFactor / l;
			SizeF shapeOffset1 = new SizeF((Origin.X - Orientation.X) * sx, (Origin.Y - Orientation.Y) * sx);
			SizeF shapeOffset2 = new SizeF((Origin.Y - Orientation.Y) * sy, -(Origin.X - Orientation.X) * sy);
			PointF2D shapeOrigin = Origin + shapeOffset1 + shapeOffset2;
			PointF2D shapeOrientation = Orientation + shapeOffset1 + shapeOffset2;
			Matrix transform = MathHelper.CalcTransform(shapeOrigin, shapeOrientation, ShapeScale);
			Shape.Accept(
					delegate(BaseShape shape) {
						shape.BeginUpdate();
						shape.Transform = transform;
						shape.CancelUpdate();
					}
				);
		}
		protected override void CopyToCore(BaseObject clone) {
			BaseScaleTickMarkProvider tickmark = clone as BaseScaleTickMarkProvider;
			if(tickmark != null) {
				tickmark.originCore = this.Origin;
				tickmark.orientationPointCore = this.Orientation;
				tickmark.scaleCore = this.ShapeScale;
				tickmark.shapeOffsetCore = this.ShapeOffset;
				tickmark.showTickCore = this.ShowTick;
				tickmark.showFirstCore = this.ShowFirst;
				tickmark.showLastCore = this.ShowLast;
				tickmark.AssignShape(this);
				tickmark.fIsTemplateCore = false;
				tickmark.shapeHeight = Shape.BoundingBox.Height;
			}
		}
		protected virtual void AssignCore(BaseScaleTickMarkProvider source) {
			if(source != null) {
				this.scaleCore = source.ShapeScale;
				this.showTickCore = source.ShowTick;
				this.showFirstCore = source.ShowFirst;
				this.showLastCore = source.ShowLast;
				this.shapeOffsetCore = source.ShapeOffset;
				this.AssignShape(source);
				this.shapeHeight = source.shapeHeight;
			}
		}
		protected virtual bool IsDifferFrom(BaseScaleTickMarkProvider source) {
			return (source == null) ? true :
				(this.shapeOffsetCore != source.ShapeOffset) ||
				(this.showTickCore != source.ShowTick) ||
				(this.scaleCore != source.ShapeScale) ||
				(this.showFirstCore != source.ShowFirst) ||
				(this.showLastCore != source.ShowLast) ||
				(this.ShapeType != source.ShapeType);
		}
		internal void ResetShapeScale() {
			ShapeScale = new FactorF2D(1f, 1f);
		}
		internal bool ShouldSerializeShapeScale() {
			return ShapeScale != new FactorF2D(1f, 1f);
		}
	}
	public class MinorTickmarkProvider : BaseScaleTickMarkProvider, IMinorTickmark, ISupportAssign<IMinorTickmark> {
		protected MinorTickmarkProvider(bool createShapes)
			: base(createShapes) {
		}
		public MinorTickmarkProvider() : this(true) { }
		protected override BaseObject CloneCore() {
			return new MinorTickmarkProvider(false);
		}
		public void Assign(IMinorTickmark source) {
			BeginUpdate();
			if(source != null) AssignCore(source as BaseScaleTickMarkProvider);
			EndUpdate();
		}
		public bool IsDifferFrom(IMinorTickmark source) {
			return (source == null) ? true : base.IsDifferFrom(source as BaseScaleTickMarkProvider);
		}
	}
	public class MinorTickmarkCollection : BaseReadOnlyDictionary<IMinorTickmark> {
		public MinorTickmarkCollection() : base() { }
		public void Add(IMinorTickmark tickmark) {
			bool nameIsEmpty = String.IsNullOrEmpty(tickmark.Name);
			if(nameIsEmpty) {
				tickmark.Name = UniqueNameHelper.GetMinorTickmarkUniqueName(this);
				tickmark.Shape.Name = tickmark.Name + "Shape";
			}
			Collection[tickmark.Name] = tickmark;
		}
	}
	public class MajorTickmarkProvider : MinorTickmarkProvider, IMajorTickmark, ISupportAssign<IMajorTickmark> {
		public static readonly string FormatStringDefault = "{0:F2}";
		float addendCore;
		float multiplierCore;
		bool allowTickOverlapCore;
		bool showTextCore;
		float textOffsetCore;
		LabelOrientation textOrientationCore;
		string formatStringCore;
		BaseShape textShapeCore;
		protected MajorTickmarkProvider(bool createShapes)
			: base(createShapes) {
			if(createShapes)
				SetShapeCore(ref textShapeCore, CreateTextShape());
		}
		public MajorTickmarkProvider()
			: this(true) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.addendCore = 0f;
			this.multiplierCore = 1f;
			this.allowTickOverlapCore = false;
			this.showTextCore = true;
			this.textOffsetCore = -25f;
			this.textOrientationCore = LabelOrientation.Default;
			this.formatStringCore = FormatStringDefault;
		}
		protected override void OnDispose() {
			DestroyShapeCore(ref textShapeCore);
			base.OnDispose();
		}
		protected TextShape CreateTextShape() {
			TextShape textShape = new TextShape();
			textShape.BeginUpdate();
			textShape.AppearanceText.TextBrush = BrushObject.Empty;
			textShape.Text = "0";
			textShape.Box = new RectangleF(-25, -15, 50, 30);
			textShape.Bounds = new RectangleF(-25, -15, 50, 30);
			textShape.EndUpdate();
			return textShape;
		}
		[DefaultValue(1f)]
		[XtraSerializableProperty]
		public float Multiplier {
			get { return multiplierCore; }
			set {
				if(Multiplier == value) return;
				multiplierCore = value;
				OnObjectChanged("Multiplier");
			}
		}
		[DefaultValue(0f)]
		[XtraSerializableProperty]
		public float Addend {
			get { return addendCore; }
			set {
				if(Addend == value) return;
				addendCore = value;
				OnObjectChanged("Addend");
			}
		}
		[DefaultValue(-25f)]
		[XtraSerializableProperty]
		public float TextOffset {
			get { return textOffsetCore; }
			set {
				if(TextOffset == value) return;
				textOffsetCore = value;
				OnObjectChanged("TextOffset");
			}
		}
		[DefaultValue(true)]
		[XtraSerializableProperty]
		public bool ShowText {
			get { return showTextCore; }
			set {
				if(ShowText == value) return;
				showTextCore = value;
				OnObjectChanged("ShowText");
			}
		}
		[DefaultValue(false)]
		[XtraSerializableProperty]
		public bool AllowTickOverlap {
			get { return allowTickOverlapCore; }
			set {
				if(AllowTickOverlap == value) return;
				allowTickOverlapCore = value;
				OnObjectChanged("AllowTickOverlap");
			}
		}
		[DefaultValue(LabelOrientation.Default)]
		[XtraSerializableProperty]
		public LabelOrientation TextOrientation {
			get { return textOrientationCore; }
			set {
				if(TextOrientation == value) return;
				textOrientationCore = value;
				OnObjectChanged("TextOrientation");
			}
		}
		[DefaultValue("{0:F2}")]
		[XtraSerializableProperty, Localizable(true)]
		public string FormatString {
			get { return formatStringCore; }
			set {
				if(FormatString == value) return;
				formatStringCore = value;
				OnObjectChanged("FormatString");
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Text {
			get { return TextShape.Text; }
			set {
				if(Text == value || IsTemplate) return;
				TextShape.Text = value;
			}
		}
		public TextShape TextShape {
			get { return textShapeCore as TextShape; }
		}
		protected override BaseObject CloneCore() {
			return new MajorTickmarkProvider(false);
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			MajorTickmarkProvider clonedTickmark = clone as MajorTickmarkProvider;
			if(clonedTickmark != null) {
				clonedTickmark.allowTickOverlapCore = this.AllowTickOverlap;
				clonedTickmark.textOffsetCore = this.TextOffset;
				clonedTickmark.textOrientationCore = this.TextOrientation;
				clonedTickmark.formatStringCore = this.FormatString;
				clonedTickmark.textShapeCore = TextShape.Clone() as TextShape;
			}
		}
		protected override void OnTransformTickmarkShapes() {
			base.OnTransformTickmarkShapes();
			float s = -TextOffset / (float)MathHelper.CalcVectorLength(Origin, Orientation);
			SizeF textOffsetAbs = new SizeF(Origin.X - Orientation.X, Origin.Y - Orientation.Y);
			PointF2D textOrigin = Origin + new SizeF((Origin.X - Orientation.X) * s, (Origin.Y - Orientation.Y) * s);
			switch(TextOrientation) {
				case LabelOrientation.BottomToTop:
					textOffsetAbs = new SizeF(0, -1);
					break;
				case LabelOrientation.TopToBottom:
					textOffsetAbs = new SizeF(0, 1);
					break;
				case LabelOrientation.LeftToRight:
					textOffsetAbs = new SizeF(1, 0);
					break;
				case LabelOrientation.Default:
				case LabelOrientation.Radial:
					textOffsetAbs = new SizeF(textOffsetAbs.Width, textOffsetAbs.Height);
					break;
				case LabelOrientation.Tangent:
					textOffsetAbs = new SizeF(textOffsetAbs.Height, -textOffsetAbs.Width);
					break;
			}
			if(textOffsetAbs.Width < 0) textOffsetAbs = new SizeF(-textOffsetAbs.Width, -textOffsetAbs.Height);
			TextShape.Transform = MathHelper.CalcTransform(textOrigin, textOrigin + textOffsetAbs, new SizeF(1, 1));
		}
		public void Assign(IMajorTickmark source) {
			BeginUpdate();
			if(source != null) {
				AssignCore(source as BaseScaleTickMarkProvider);
				this.addendCore = source.Addend;
				this.multiplierCore = source.Multiplier;
				this.allowTickOverlapCore = source.AllowTickOverlap;
				this.formatStringCore = source.FormatString;
				this.showTextCore = source.ShowText;
				this.textOffsetCore = source.TextOffset;
				this.textOrientationCore = source.TextOrientation;
				AssignTextShape(source);
			}
			EndUpdate();
		}
		protected void AssignTextShape(IMajorTickmark source) {
			DestroyShapeCore(ref textShapeCore);
			SetShapeCore(ref textShapeCore, source.TextShape.Clone());
		}
		public bool IsDifferFrom(IMajorTickmark source) {
			return (source == null) ? true : base.IsDifferFrom(source as IMinorTickmark) ||
								(this.addendCore != source.Addend) ||
								(this.multiplierCore != source.Multiplier) ||
								(this.allowTickOverlapCore != source.AllowTickOverlap) ||
								(this.formatStringCore != source.FormatString) ||
								(this.textOffsetCore != source.TextOffset) ||
								(this.showTextCore != source.ShowText) ||
								(this.textOrientationCore != source.TextOrientation);
		}
	}
	public class MajorTickmarkCollection : BaseReadOnlyDictionary<IMajorTickmark> {
		public MajorTickmarkCollection() : base() { }
		public void Add(IMajorTickmark tickmark) {
			bool nameIsEmpty = String.IsNullOrEmpty(tickmark.Name);
			if(nameIsEmpty) {
				string name = UniqueNameHelper.GetMajorTickmarkUniqueName(this);
				tickmark.Name = name;
				tickmark.Shape.Name = name + "Shape";
				tickmark.TextShape.Name = name + "Text";
			}
			Collection[tickmark.Name] = tickmark;
		}
	}
	public class TickmarkObjectTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return FilterPropertiesHelper.FilterProperties(base.GetProperties(context, value, attributes), GetExpectedProperties(value));
		}
		static string[] GetExpectedProperties(object value) {
			ArrayList filteredProperties = new ArrayList();
			if(value is IMinorTickmark) {
				filteredProperties.AddRange(new string[] { "ShapeType", "ShapeScale", "ShapeOffset", "ShowTick", "ShowFirst", "ShowLast" });
			}
			if(value is IMajorTickmark) {
				filteredProperties.AddRange(new string[] { "FormatString", "Addend", "Multiplier", "TextOrientation", "TextOffset", "AllowTickOverlap", "ShowText" });
			}
			return (string[])filteredProperties.ToArray(typeof(string));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return destType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			if(value is IMajorTickmark) return "<IMajorTickmark>";
			return "<IMinorTickmark>";
		}
	}
}
