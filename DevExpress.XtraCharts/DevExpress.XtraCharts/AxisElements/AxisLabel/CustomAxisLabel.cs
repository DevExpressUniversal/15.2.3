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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using System.Drawing;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(CustomAxisLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public sealed class CustomAxisLabel : ChartElementNamed, ICustomAxisLabel {
		const bool DefaultVisible = true;
		static readonly Color DefaultTextColor = Color.Empty;
		static readonly Color DefaultBackColor = Color.Empty;
		object axisValue;
		string axisValueSerializable;
		double value = 1.0;
		bool visible = DefaultVisible;
		Font font = null;
		Color textColor = DefaultTextColor;
		Color backColor = DefaultBackColor;
		readonly RectangleFillStyle fillStyle;
		readonly RectangularBorder border;
		Axis2D Axis { get { return (Axis2D)base.Owner; } }
		double FinalValue { get { return Axis.ScaleTypeMap.Transformation.TransformForward(value); } }
		object ICustomAxisLabel.Content { get { return Name; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CustomAxisLabelAxisValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CustomAxisLabel.AxisValue"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(AxisValueTypeConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public object AxisValue {
			get { return axisValue; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectCustomAxisLabelAxisValue));
				if (value != axisValue) {
					SendNotification(new ElementWillChangeNotification(this));
					axisValue = (Axis == null || Loading) ? value : Axis.ConvertBasedOnScaleType(value);
					RaiseControlChanged(new AxisElementUpdateInfo(this, Axis));
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string AxisValueSerializable {
			get { return SerializingUtils.ConvertToSerializable(AxisValue); }
			set {
				AxisValue = value;
				if (Axis == null || Loading)
					axisValueSerializable = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CustomAxisLabelVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CustomAxisLabel.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return visible; }
			set {
				if (value != visible) {
					SendNotification(new ElementWillChangeNotification(this));
					visible = value;
					RaiseControlChanged(new AxisElementUpdateInfo(this, Axis));
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CustomAxisLabelFont"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CustomAxisLabel.Font"),
		TypeConverter(typeof(FontTypeConverter)),
		Category(Categories.Appearance),
		Localizable(true),
		XtraSerializableProperty
		]
		public Font Font {
			get { return font; }
			set {
				if (font != value) {
					SendNotification(new ElementWillChangeNotification(this));
					font = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CustomAxisLabelTextColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CustomAxisLabel.TextColor"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public Color TextColor {
			get { return textColor; }
			set {
				if (value != textColor) {
					SendNotification(new ElementWillChangeNotification(this));
					textColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CustomAxisLabelBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CustomAxisLabel.BackColor"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public Color BackColor {
			get { return backColor; }
			set {
				if (value != backColor) {
					SendNotification(new ElementWillChangeNotification(this));
					backColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CustomAxisLabelFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CustomAxisLabel.FillStyle"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleFillStyle FillStyle {
			get { return fillStyle; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CustomAxisLabelBorder"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CustomAxisLabel.Border"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangularBorder Border {
			get { return border; }
		}
		public CustomAxisLabel(string name)
			: base(name) {
			fillStyle = new RectangleFillStyle(this, Color.Empty, FillMode.Solid);
			border = new InsideRectangularBorder(this, false, Color.Empty);
		}
		public CustomAxisLabel()
			: this(String.Empty) {
		}
		public CustomAxisLabel(string name, object value)
			: this(name) {
			if (value == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectCustomAxisLabelAxisValue));
			axisValue = value;
		}
		public CustomAxisLabel(string name, double value)
			: this(name, (object)value) {
		}
		public CustomAxisLabel(string name, string value)
			: this(name, (object)value) {
		}
		public CustomAxisLabel(string name, DateTime value)
			: this(name, (object)value) {
		}
		#region IAxisValueContainer implementation
		IAxisData IAxisValueContainer.Axis { get { return Axis; } }
		bool IAxisValueContainer.IsEnabled { get { return Visible; } }
		CultureInfo IAxisValueContainer.Culture {
			get {
				return (axisValueSerializable != null && axisValueSerializable.Equals(axisValue)) ?
					CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
			}
		}
		object IAxisValueContainer.GetAxisValue() {
			return axisValue;
		}
		void IAxisValueContainer.SetAxisValue(object axisValue) {
			if (!Object.ReferenceEquals(axisValue, this.axisValue)) {
				this.axisValue = axisValue;
				axisValueSerializable = null;
			}
		}
		double IAxisValueContainer.GetValue() {
			return value;
		}
		void IAxisValueContainer.SetValue(double value) {
			this.value = value;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeVisible() {
			return visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
		}
		bool ShouldSerializeTextColor() {
			return textColor != DefaultTextColor;
		}
		void ResetTextColor() {
			TextColor = DefaultTextColor;
		}
		bool ShouldSerializeBackColor() {
			return backColor != DefaultBackColor;
		}
		void ResetBackColor() {
			BackColor = DefaultBackColor;
		}
		bool ShouldSerializeFont() {
			return font != null;
		}
		void ResetFont() {
			Font = null;
		}
		bool ShouldSerializeFillStyle() {
			return fillStyle.ShouldSerialize();
		}
		bool ShouldSerializeBorder() {
			return Border.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "AxisValue":
					return true;
				case "Visible":
					return ShouldSerializeVisible();
				case "TextColor":
					return ShouldSerializeTextColor();
				case "Font":
					return ShouldSerializeFont();
				case "BackColor":
					return ShouldSerializeBackColor();
				case "FillStyle":
					return ShouldSerializeFillStyle();
				case "Border":
					return ShouldSerializeBorder();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new CustomAxisLabel();
		}
		protected override void Dispose(bool disposing) {
			if (disposing && !IsDisposed) {
				fillStyle.Dispose();
				border.Dispose();
			}
			base.Dispose(disposing);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			CustomAxisLabel label = obj as CustomAxisLabel;
			if (label != null) {
				axisValue = label.axisValue;
				visible = label.visible;
				axisValueSerializable = null;
				textColor = label.textColor;
				font = label.font;
				backColor = label.backColor;
				fillStyle.Assign(label.fillStyle);
				border.Assign(label.border);
			}
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public sealed class CustomAxisLabelCollection : ChartElementNamedCollection, IEnumerable<ICustomAxisLabel> {
		protected override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.CustomAxisLabelPrefix); } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CustomAxisLabelCollectionItem")]
#endif
		public CustomAxisLabel this[int index] { get { return (CustomAxisLabel)List[index]; } }
		internal Axis2D Axis { get { return (Axis2D)base.Owner; } }
		internal CustomAxisLabelCollection(Axis2D axis)
			: base(axis) {
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<ICustomAxisLabel>)this).GetEnumerator();
		}
		IEnumerator<ICustomAxisLabel> IEnumerable<ICustomAxisLabel>.GetEnumerator() {
			foreach (ICustomAxisLabel customLabel in this)
				yield return customLabel;
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(ChartCollectionOperation operation, object oldItem, int oldIndex, object newItem, int newIndex) {
			return new AxisElementUpdateInfo(this, Axis);
		}
		protected override ChartUpdateInfoBase CreateBatchUpdateInfo(ChartCollectionOperation operation, ICollection oldItems, int oldIndex, ICollection newItems, int newIndex) {
			return new AxisElementUpdateInfo(this, Axis);
		}
		public int Add(CustomAxisLabel label) {
			if (Axis != null)
				AxisValueContainerHelper.UpdateAxisValue(label, Axis.ScaleTypeMap);
			return base.Add(label);
		}
		public void AddRange(CustomAxisLabel[] coll) {
			if (Axis != null)
				foreach (CustomAxisLabel label in coll)
					AxisValueContainerHelper.UpdateAxisValue(label, Axis.ScaleTypeMap);
			base.AddRange(coll);
		}
		public void Remove(CustomAxisLabel label) {
			base.Remove(label);
		}
		public bool Contains(CustomAxisLabel label) {
			return base.Contains(label);
		}
		public CustomAxisLabel GetAxisLabelByName(string name) {
			return (CustomAxisLabel)base.GetElementByName(name);
		}
	}
}
