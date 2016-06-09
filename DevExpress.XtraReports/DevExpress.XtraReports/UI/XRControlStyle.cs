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
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraPrinting;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using DevExpress.XtraReports.Localization;
#if SL
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Collections;
using System.Windows.Media;
#endif
namespace DevExpress.XtraReports.UI {
	[
	DesignTimeVisible(false),
	ToolboxItem(false),
	TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
	XRDesigner("DevExpress.XtraReports.Design.XRControlStyleDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design.XRControlStyleDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlStyle"),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRControlStyle.bmp")
	]
#if DEBUGTEST
	[System.Diagnostics.DebuggerTypeProxy(typeof(XRControlStyle.XRControlStyleDebugView))]
#endif
	public class XRControlStyle : BrickStyle, IXRSerializable, IComponent {
		#region Debug Helper
#if DEBUGTEST
		class XRControlStyleDebugView {
			XRControlStyle style;
			public object BackColor {
				get { return GetPropertyValue(StyleProperty.BackColor); }
			}
			public object BorderColor {
				get { return GetPropertyValue(StyleProperty.BorderColor); }
			}
			public object BorderDashStyle {
				get { return GetPropertyValue(StyleProperty.BorderDashStyle); }
			}
			public object Borders {
				get { return GetPropertyValue(StyleProperty.Borders); }
			}
			public object BorderWidth {
				get { return GetPropertyValue(StyleProperty.BorderWidth); }
			}
			public object Font {
				get { return GetPropertyValue(StyleProperty.Font); }
			}
			public object ForeColor {
				get { return GetPropertyValue(StyleProperty.ForeColor); }
			}
			public object Padding {
				get { return GetPropertyValue(StyleProperty.Padding); }
			}
			public object TextAlignment {
				get { return GetPropertyValue(StyleProperty.TextAlignment); }
			}
			public XRControlStyleDebugView(XRControlStyle style) {
				this.style = style;
			}
			object GetPropertyValue(StyleProperty property) {
				return BrickStyle.PropertyIsSet(style, property) ? style.GetValue(property) : ReportLocalizer.GetString(ReportStringId.UD_PropertyGrid_NotSetText);
			}
		}
#endif
		#endregion
		public static readonly new XRControlStyle Default = new XRControlStyle();
		#region IComponent serialization
		ISite site;
		[
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public event EventHandler Disposed;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyleSite"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)
		]
		public virtual ISite Site {
			get { return this.site; }
			set { this.site = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public IContainer Container {
			get { return site != null ? site.Container : null; }
		}
		#endregion
		#region properties
		string name = string.Empty;
		bool isDisposed;
		StyleUsing styleUsing = new StyleUsing();
		bool dirty = true;
		float dpi = PaddingInfo.Empty.Dpi;
		internal IEnumerable<XRControlStyle> Owner { get; set; }
		[Browsable(false)]
		public new bool IsDisposed { get { return isDisposed; } }
		internal bool Dirty {
			get { return dirty; }
			set { dirty = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override BrickBorderStyle BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStylePadding"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlStyle.Padding"),
		SRCategory(ReportStringId.CatAppearance),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStylePaddingConverter)),
		]
		public override PaddingInfo Padding {
			get {
				return new PaddingInfo(base.Padding, dpi);
			}
			set {
				if (base.Padding != value || !IsSetPadding)
					SetPadding(value);
			}
		}
		[
		DefaultValue(""),
		Browsable(false),
		XtraSerializableProperty(-1),
		]
		public string Name {
			get { return (site != null) ? site.Name : name; }
			set { name = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyleFont"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlStyle.Font"),
		SRCategory(ReportStringId.CatAppearance),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleFontConverter)),
		]
		public override Font Font {
			get { return base.Font; }
			set {
				if (base.Font != value || !IsSetFont) {
					base.Font = value;
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyleForeColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlStyle.ForeColor"),
		SRCategory(ReportStringId.CatAppearance),
		Editor("DevExpress.XtraReports.Design.XrControlStyleColorEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleColorConverter)),
		]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set {
				if (base.ForeColor != value || !IsSetForeColor)
					base.ForeColor = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyleBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlStyle.BackColor"),
		SRCategory(ReportStringId.CatAppearance),
		Editor("DevExpress.XtraReports.Design.XrControlStyleColorEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleColorConverter)),
		]
		public override Color BackColor {
			get { return base.BackColor; }
			set {
				if (base.BackColor != value || !IsSetBackColor)
					base.BackColor = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyleBorderColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlStyle.BorderColor"),
		SRCategory(ReportStringId.CatAppearance),
		Editor("DevExpress.XtraReports.Design.XrControlStyleColorEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleColorConverter)),
		]
		public override Color BorderColor {
			get { return base.BorderColor; }
			set {
				if (base.BorderColor != value || !IsSetBorderColor)
					base.BorderColor = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyleBorders"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlStyle.Borders"),
		SRCategory(ReportStringId.CatAppearance),
		Editor("DevExpress.XtraReports.Design.BordersEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleBordersConverter)),
		]
		public virtual BorderSide Borders {
			get { return base.Sides; }
			set {
				if (base.Sides != value || !IsSetBorders)
					base.Sides = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyleBorderDashStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlStyle.BorderDashStyle"),
		SRCategory(ReportStringId.CatAppearance),
		Editor("DevExpress.XtraReports.Design.BorderDashStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleBorderDashStyleConverter)),
		]
		public override BorderDashStyle BorderDashStyle {
			get { return base.BorderDashStyle; }
			set {
				if (base.BorderDashStyle != value || !IsSetBorderDashStyle)
					base.BorderDashStyle = value;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override BorderSide Sides {
			get { return base.Sides; }
			set { base.Sides = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override BrickStringFormat StringFormat {
			get { return base.StringFormat; }
			set { base.StringFormat = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyleBorderWidth"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlStyle.BorderWidth"),
		SRCategory(ReportStringId.CatAppearance),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleBorderWidthConverter)),
		]
		public override float BorderWidth {
			get { return base.BorderWidth; }
			set {
				if (base.BorderWidth != value || !IsSetBorderWidth) {
					if (value < 0)
						throw (new ArgumentOutOfRangeException("BorderWidth"));
					base.BorderWidth = value;
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyleTextAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlStyle.TextAlignment"),
		SRCategory(ReportStringId.CatAppearance),
		Editor("DevExpress.XtraReports.Design.TextAlignmentEditor," + AssemblyInfo.SRAssemblyUtilsUIFull, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleTextAlignmentConverter)),
		]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set {
				if(base.TextAlignment != value || !IsSetTextAlignment) {
					base.TextAlignment = value;
					StringFormat.Value.Alignment = GraphicsConvertHelper.ToHorzStringAlignment(value);
					StringFormat.Value.LineAlignment = GraphicsConvertHelper.ToVertStringAlignment(value);
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyleStyleUsing"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRControlStyle.StyleUsing"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public StyleUsing StyleUsing { get { return styleUsing; } }
		#endregion properties
		public XRControlStyle() {
			base.BorderStyle = BrickBorderStyle.Inset;
		}
		public XRControlStyle(float dpi)
			: base(dpi) {
			base.BorderStyle = BrickBorderStyle.Inset;
		}
		public XRControlStyle(XRControlStyle src)
			: base(src) {
			CopySpecificProperties(src);
		}
		void CopySpecificProperties(XRControlStyle src) {
			styleUsing = (StyleUsing)src.styleUsing.Clone();
			name = src.Name;
			dpi = src.dpi;
		}
		public XRControlStyle(Color backColor, Color borderColor, BorderSide sides, float borderWidth, Font font, Color foreColor, TextAlignment textAlignment)
			: this(backColor, borderColor, sides, borderWidth, font, foreColor, textAlignment, PaddingInfo.Empty) {
		}
		public XRControlStyle(Color backColor, Color borderColor, BorderSide sides, float borderWidth, Font font, Color foreColor, TextAlignment textAlignment, PaddingInfo padding)
			: this(backColor, borderColor, sides, borderWidth, font, foreColor, textAlignment, PaddingInfo.Empty, BorderDashStyle.Solid) {
		}
		public XRControlStyle(Color backColor, Color borderColor, BorderSide sides, float borderWidth, Font font, Color foreColor, TextAlignment textAlignment, PaddingInfo padding, BorderDashStyle borderDashStyle)
			: this() {
			base.Font = font;
			base.BorderWidth = borderWidth;
			base.ForeColor = foreColor;
			base.BackColor = backColor;
			base.BorderColor = borderColor;
			base.Sides = sides;
			base.TextAlignment = textAlignment;
			base.BorderDashStyle = borderDashStyle;
			SetPadding(padding);
		}
		#region Serialization
		void IXRSerializable.SerializeProperties(XRSerializer serializer) {
#if !SL
			serializer.SerializeFont("Font", base.Font);
			serializer.SerializeColor("ForeColor", base.ForeColor);
			serializer.SerializeColor("BackColor", base.BackColor);
			serializer.SerializeColor("BorderColor", base.BorderColor);
			serializer.SerializeEnum("Borders", base.Sides);
			serializer.SerializeEnum("TextAlignment", base.TextAlignment);
			serializer.SerializeSingle("BorderWidth", base.BorderWidth);
			serializer.Serialize("StyleUsing", styleUsing);
			serializer.SerializeString("Name", Name);
#endif
		}
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
#if !SL
			base.Font = serializer.DeserializeFont("Font", DefaultFont);
			base.ForeColor = serializer.DeserializeColor("ForeColor", DXColor.Black);
			base.BackColor = serializer.DeserializeColor("BackColor", Color.Transparent);
			base.BorderColor = serializer.DeserializeColor("BorderColor", DXColor.Black);
			XRBorderSide borderSide = (XRBorderSide)serializer.DeserializeEnum("BorderSide", typeof(XRBorderSide), XRBorderSide.None);
			base.Sides = (BorderSide)serializer.DeserializeEnum("Borders", typeof(BorderSide), (BorderSide)borderSide);
			ContentAlignment textAlign = (ContentAlignment)serializer.DeserializeEnum("TextAlign", typeof(ContentAlignment), ContentAlignment.TopLeft);
			base.TextAlignment = (TextAlignment)serializer.DeserializeEnum("TextAlignment", typeof(TextAlignment), XRConvert.ToTextAlignment(textAlign));
			base.BorderWidth = serializer.DeserializeSingle("BorderWidth", 0);
			serializer.Deserialize("StyleUsing", styleUsing);
			name = serializer.DeserializeString("Name", string.Empty);
#endif
		}
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		#endregion
		#region ShouldSerialize functions
		bool ShouldSerializeBackColor() {
			return IsSetBackColor;
		}
		bool ShouldSerializeBorderColor() {
			return IsSetBorderColor;
		}
		bool ShouldSerializeBorderDashStyle() {
			return IsSetBorderDashStyle;
		}
		bool ShouldSerializeBorders() {
			return IsSetBorders;
		}
		bool ShouldSerializeSides() {
			return ShouldSerializeBorders();
		}
		bool ShouldSerializeBorderWidth() {
			return IsSetBorderWidth;
		}
		bool ShouldSerializeBorderWidthSerializable() {
			return ShouldSerializeBorderWidth();
		}
		bool ShouldSerializeFont() {
			return IsSetFont;
		}
		bool ShouldSerializeForeColor() {
			return IsSetForeColor;
		}
		bool ShouldSerializePadding() {
			return IsSetPadding;
		}
		bool ShouldSerializeTextAlignment() {
			return IsSetTextAlignment;
		}
		#endregion
		void SetPadding(PaddingInfo padding) {
			base.Padding = padding;
			dpi = padding.Dpi;
		}
		public override object Clone() {
			XRControlStyle clone = (XRControlStyle)base.Clone();
			clone.CopySpecificProperties(this);
			return clone;
		}
		protected override BrickStyle CreateClone() {
			return new XRControlStyle();
		}
		protected virtual void OnDisposed() {
			if(Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				OnDisposed();
				isDisposed = true;
				Owner = null;
			}
			base.Dispose(disposing);
		}
		public override bool Equals(object obj) {
			XRControlStyle style = obj as XRControlStyle;
			if (style == null)
				return false;
			return Name != style.Name ? false : base.Equals(style);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		[Obsolete("Use Equals instead")]
		public bool StyleEquals(XRControlStyle style) {
			return Equals(style) && styleUsing.Equals(style.styleUsing);
		}
		protected internal void SyncDpi(float toDpi) {
			this.dpi = toDpi;
		}
		internal void ChangeStyle(Action2<StyleProperty, XRControlStyle> changeProperty) {
			changeProperty(StyleProperty.BackColor, this);
			changeProperty(StyleProperty.BorderColor, this);
			changeProperty(StyleProperty.BorderDashStyle, this);
			changeProperty(StyleProperty.Borders, this);
			changeProperty(StyleProperty.BorderWidth, this);
			changeProperty(StyleProperty.Font, this);
			changeProperty(StyleProperty.ForeColor, this);
			changeProperty(StyleProperty.Padding, this);
			changeProperty(StyleProperty.TextAlignment, this);
		}
		internal void ApplyStyleUsing() {
			if (dirty)
				ChangeStyle(ApplyStyleUsing);
		}
		void ApplyStyleUsing(StyleProperty property, XRControlStyle style) {
			if(!StyleUsing.IsSet(property))
				Reset(property);
			else if(!IsSet(property))
				XRControlStyle.Default.CopyProperties(this, property);
		}
		#region Get initial values
		protected override Color GetInitialBackColor() {
			return DXColor.Transparent;
		}
		protected override Color GetInitialBorderColor() {
			return DXColor.Black;
		}
		protected override float GetInitialBorderWidth() {
			return 1;
		}
		protected override Color GetInitialForeColor() {
			return DXColor.Black;
		}
		#endregion
		#region Set base values
		internal void SetBaseBackColor(Color value) {
			base.BackColor = value;
		}
		internal void SetBaseBorderColor(Color value) {
			base.BorderColor = value;
		}
		internal void SetBaseBorders(BorderSide value) {
			base.Sides = value;
		}
		internal void SetBaseBorderWidth(float value) {
			base.BorderWidth = value;
		}
		internal void SetBaseBorderDashStyle(BorderDashStyle value) {
			base.BorderDashStyle = value;
		}
		internal void SetBaseFont(Font value) {
			base.Font = value;
		}
		internal void SetBaseForeColor(Color value) {
			base.ForeColor = value;
		}
		internal void SetBasePadding(PaddingInfo value) {
			base.Padding = value;
		}
		internal void SetBaseTextAlignment(TextAlignment value) {
			base.TextAlignment = value;
		}
		#endregion
	}
}
