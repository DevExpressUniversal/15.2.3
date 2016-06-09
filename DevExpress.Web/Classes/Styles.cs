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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web {
	public enum BackgroundImageRepeat { Repeat, NoRepeat, RepeatX, RepeatY };
	[TypeConverter(typeof(ExpandableObjectConverter)), AutoFormatUrlPropertyClass]
	public class BackgroundImage: StateManager {
		private static string[] RepeatAtributeValues = new string[] { "repeat", "no-repeat", "repeat-x", "repeat-y" };
		[
#if !SL
	DevExpressWebLocalizedDescription("BackgroundImageImageUrl"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public string ImageUrl {
			get { return GetStringProperty("ImageUrl", ""); }
			set { SetStringProperty("ImageUrl", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BackgroundImageRepeat"),
#endif
		DefaultValue(BackgroundImageRepeat.Repeat), NotifyParentProperty(true), AutoFormatEnable]
		public BackgroundImageRepeat Repeat {
			get { return (BackgroundImageRepeat)GetEnumProperty("Repeat", BackgroundImageRepeat.Repeat); }
			set { SetEnumProperty("Repeat", BackgroundImageRepeat.Repeat, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BackgroundImageHorizontalPosition"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		TypeConverter("DevExpress.Web.Design.BackgroundHorizontalPositionConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), AutoFormatEnable]
		public string HorizontalPosition {
			get { return GetStringProperty("HPosition", ""); }
			set { SetStringProperty("HPosition", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BackgroundImageVerticalPosition"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		TypeConverter("DevExpress.Web.Design.BackgroundVerticalPositionConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), AutoFormatEnable]
		public string VerticalPosition {
			get { return GetStringProperty("VPosition", ""); }
			set { SetStringProperty("VPosition", "", value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsEmpty {
			get { return (ImageUrl == ""); }
		}
		public BackgroundImage() {
		}
		public void Assign(BackgroundImage source) {
			Reset();
			CopyFrom(source);
		}
		public void AssignToControl(WebControl control) {
			bool isEmpty = string.IsNullOrEmpty(ImageUrl);
			if(isEmpty) {
				RenderUtils.SetStyleStringAttribute(control, "background-image", "");
				RenderUtils.SetStyleAttribute(control, "background-repeat", "", "");
				RenderUtils.SetStyleAttribute(control, "background-position", "", "");
			}
			else {
				RenderUtils.SetStyleStringAttribute(control, "background-image", ResourceManager.ResolveClientUrl(control, ImageUrl));
				RenderUtils.SetStyleAttribute(control, "background-repeat", RepeatAtributeValues[(int)Repeat], RepeatAtributeValues[(int)BackgroundImageRepeat.Repeat]);
				if(!string.IsNullOrEmpty(HorizontalPosition) || !string.IsNullOrEmpty(VerticalPosition)) {
					string hPosition = !string.IsNullOrEmpty(HorizontalPosition) ? HorizontalPosition : "left";
					string vPosition = !string.IsNullOrEmpty(VerticalPosition) ? VerticalPosition : "top";
					RenderUtils.SetStyleStringAttribute(control, "background-position", hPosition + " " + vPosition);
				}
			}
		}
		public void CopyFrom(BackgroundImage backgroundImage) {
			if (backgroundImage.ImageUrl != "")
				ImageUrl = backgroundImage.ImageUrl;
			if (backgroundImage.Repeat != BackgroundImageRepeat.Repeat)
				Repeat = backgroundImage.Repeat;
			if (backgroundImage.HorizontalPosition != "")
				HorizontalPosition = backgroundImage.HorizontalPosition;
			if (backgroundImage.VerticalPosition != "")
				VerticalPosition = backgroundImage.VerticalPosition;
		}
		public void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver) {
			if (ImageUrl != "") {
				attributes.Add(HtmlTextWriterStyle.BackgroundImage, ResourceManager.ResolveClientUrl(urlResolver, ImageUrl));
				if (Repeat != BackgroundImageRepeat.Repeat)
					attributes.Add("background-repeat", RepeatAtributeValues[(int)Repeat]);
				if (HorizontalPosition != "" || VerticalPosition != "") {
					string hPosition = (HorizontalPosition != "") ? HorizontalPosition : "left";
					string vPosition = (VerticalPosition != "") ? VerticalPosition : "top";
					attributes.Add("background-position", hPosition + " " + vPosition);
				}
			}
		}
		public void MergeWith(BackgroundImage backgroundImage) {
			if (backgroundImage.ImageUrl != "" && ImageUrl == "")
				ImageUrl = backgroundImage.ImageUrl;
			if (backgroundImage.Repeat != BackgroundImageRepeat.Repeat && Repeat == BackgroundImageRepeat.Repeat)
				Repeat = backgroundImage.Repeat;
			if (backgroundImage.HorizontalPosition != "" && HorizontalPosition == "")
				HorizontalPosition = backgroundImage.HorizontalPosition;
			if (backgroundImage.VerticalPosition != "" && VerticalPosition == "")
				VerticalPosition = backgroundImage.VerticalPosition;
		}
		public void Reset() {
			ImageUrl = "";
			Repeat = BackgroundImageRepeat.Repeat;
			HorizontalPosition = "";
			VerticalPosition = "";
		}
		public override string ToString() {
			return ImageUrl;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class BorderBase: StateManager {
		[
#if !SL
	DevExpressWebLocalizedDescription("BorderBaseBorderColor"),
#endif
 AutoFormatEnable]
		public abstract Color BorderColor {
			get;
			set;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BorderBaseBorderStyle"),
#endif
 AutoFormatEnable]
		public abstract BorderStyle BorderStyle {
			get;
			set;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BorderBaseBorderWidth"),
#endif
 AutoFormatEnable]
		public abstract Unit BorderWidth {
			get;
			set;
		}
		public void Assign(BorderBase source) {
			Reset();
			CopyFrom(source);
		}
		public void CopyFrom(BorderBase border) {
			if(!border.BorderColor.IsEmpty)
				BorderColor = border.BorderColor;
			if(!border.BorderWidth.IsEmpty)
				BorderWidth = border.BorderWidth;
			if(border.BorderStyle != BorderStyle.NotSet)
				BorderStyle = border.BorderStyle;
		}
		public void MergeWith(BorderBase border) {
			if(!border.BorderColor.IsEmpty && BorderColor == Color.Empty)
				BorderColor = border.BorderColor;
			if(!border.BorderWidth.IsEmpty && BorderWidth.IsEmpty)
				BorderWidth = border.BorderWidth;
			if(border.BorderStyle != BorderStyle.NotSet && BorderStyle == BorderStyle.NotSet)
				BorderStyle = border.BorderStyle;
		}
		public void Reset() {
			BorderColor = Color.Empty;
			BorderWidth = Unit.Empty;
			BorderStyle = BorderStyle.NotSet;
		}
		public override string ToString() {
			string s = "";
			if (!BorderWidth.IsEmpty)
				s += HtmlConvertor.EncodeUnit(BorderWidth) + " ";
			if (BorderStyle != BorderStyle.NotSet)
				s += BorderStyle.ToString() + " ";
			if (!BorderColor.IsEmpty) 
				s += HtmlConvertor.ToHtml(BorderColor) + " ";
			return s;
		}
	}
	public interface IChartControlBuilder {
		string BorderTagName { get; }
		Type BorderType { get; }
	}
	public class BorderWrapperControlBuilder : System.Web.UI.ControlBuilder {
		public override void Init(System.Web.UI.TemplateParser parser, System.Web.UI.ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs) {
			IChartControlBuilder chartBuilder = parentBuilder as IChartControlBuilder;
			if(chartBuilder != null) {
				type = chartBuilder.BorderType;
				tagName = chartBuilder.BorderTagName;
			}
			base.Init(parser, parentBuilder, type, tagName, id, attribs);
		}
	}
	[ControlBuilder(typeof(BorderWrapperControlBuilder))]
	public class BorderWrapper: BorderBase {
		Style fOwner;
		[
#if !SL
	DevExpressWebLocalizedDescription("BorderWrapperBorderColor"),
#endif
		DefaultValue(typeof(Color), ""), NotifyParentProperty(true),
	   TypeConverter(typeof(WebColorConverter)), AutoFormatEnable()]
		public override Color BorderColor {
			get { return fOwner.BorderColor; }
			set { fOwner.BorderColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BorderWrapperBorderStyle"),
#endif
	   DefaultValue(BorderStyle.NotSet), NotifyParentProperty(true), AutoFormatEnable()]
		public override BorderStyle BorderStyle {
			get { return fOwner.BorderStyle; }
			set { fOwner.BorderStyle = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BorderWrapperBorderWidth"),
#endif
	   DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable()]
		public override Unit BorderWidth {
			get { return fOwner.BorderWidth; }
			set { fOwner.BorderWidth = value; }
		}
		public BorderWrapper(Style owner) {
			this.fOwner = owner;
		}
		protected internal Border GetBorder() {
			return new Border(BorderColor, BorderStyle, BorderWidth);
		}
	}
	public class Border: BorderBase {
		public static readonly Border NullBorder = new Border(Color.Empty, BorderStyle.None, 0);
		[
#if !SL
	DevExpressWebLocalizedDescription("BorderBorderColor"),
#endif
		DefaultValue(typeof(Color), ""), NotifyParentProperty(true),
		TypeConverter(typeof(WebColorConverter)), AutoFormatEnable()]
		public override Color BorderColor {
			get { return GetColorProperty("Color", System.Drawing.Color.Empty); }
			set { SetColorProperty("Color", System.Drawing.Color.Empty, value); }
		}
		static object borderStyleNotSet = BorderStyle.NotSet;
		[
#if !SL
	DevExpressWebLocalizedDescription("BorderBorderStyle"),
#endif
	   DefaultValue(BorderStyle.NotSet), NotifyParentProperty(true), AutoFormatEnable()]
		public override BorderStyle BorderStyle {
			get { return (BorderStyle)GetEnumProperty("Style", borderStyleNotSet); }
			set { SetEnumProperty("Style", borderStyleNotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BorderBorderWidth"),
#endif
	   DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable()]
		public override Unit BorderWidth {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set { SetUnitProperty("Width", Unit.Empty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsEmpty {
			get {
				return (BorderColor.IsEmpty && BorderWidth.IsEmpty && BorderStyle == BorderStyle.NotSet);
			}
		}
		public Border() {
		}
		public Border(Color color, BorderStyle style, Unit width) {
			BorderColor = color;
			BorderStyle = style;
			BorderWidth = width;
		}
		public void AssignToControl(WebControl control) {
			RenderUtils.SetStyleColorAttribute(control, GetColorRenderAttribute(), BorderColor);
			RenderUtils.SetStyleAttribute(control, GetStyleRenderAttribute(), BorderStyle, borderStyleNotSet);
			RenderUtils.SetStyleUnitAttribute(control, GetWidthRenderAttribute(), BorderWidth);
		}
		public void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver) {
			if (!BorderColor.IsEmpty)
				attributes.Add(GetColorRenderAttribute(), ColorTranslator.ToHtml(BorderColor));
			if (BorderStyle != BorderStyle.NotSet)
				attributes.Add(GetStyleRenderAttribute(), BorderStyle.ToString());
			if (!BorderWidth.IsEmpty)
				attributes.Add(GetWidthRenderAttribute(), HtmlConvertor.EncodeUnit(BorderWidth));
		}
		protected virtual string GetColorRenderAttribute() { return "border-color"; }
		protected virtual string GetStyleRenderAttribute() { return "border-style"; }
		protected virtual string GetWidthRenderAttribute() { return "border-width"; }
	}
	public class BorderLeft: Border {
		public BorderLeft()
			: base() {
		}
		public BorderLeft(Color color, BorderStyle style, Unit width)
			: base(color, style, width) {
		}
		protected override string GetColorRenderAttribute() { return "border-left-color"; }
		protected override string GetStyleRenderAttribute() { return "border-left-style"; }
		protected override string GetWidthRenderAttribute() { return "border-left-width"; }
	}
	public class BorderTop: Border {
		public BorderTop()
			: base() {
		}
		public BorderTop(Color color, BorderStyle style, Unit width)
			: base(color, style, width) {
		}
		protected override string GetColorRenderAttribute() { return "border-top-color"; }
		protected override string GetStyleRenderAttribute() { return "border-top-style"; }
		protected override string GetWidthRenderAttribute() { return "border-top-width"; }
	}
	public class BorderRight: Border {
		public BorderRight()
			: base() {
		}
		public BorderRight(Color color, BorderStyle style, Unit width)
			: base(color, style, width) {
		}
		protected override string GetColorRenderAttribute() { return "border-right-color"; }
		protected override string GetStyleRenderAttribute() { return "border-right-style"; }
		protected override string GetWidthRenderAttribute() { return "border-right-width"; }
	}
	public class BorderBottom: Border {
		public BorderBottom()
			: base() {
		}
		public BorderBottom(Color color, BorderStyle style, Unit width)
			: base(color, style, width) {
		}
		protected override string GetColorRenderAttribute() { return "border-bottom-color"; }
		protected override string GetStyleRenderAttribute() { return "border-bottom-style"; }
		protected override string GetWidthRenderAttribute() { return "border-bottom-width"; }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Paddings: StateManager {
		public static readonly Paddings NullPaddings = new Paddings(0, 0, 0, 0, 0);
		[
#if !SL
	DevExpressWebLocalizedDescription("PaddingsPadding"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit Padding {
			get { return GetUnitProperty("Padding", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "Padding");
				SetUnitProperty("Padding", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PaddingsPaddingLeft"),
#endif
	   DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit PaddingLeft {
			get { return GetUnitProperty("PaddingLeft", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "PaddingLeft");
				SetUnitProperty("PaddingLeft", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PaddingsPaddingTop"),
#endif
	   DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit PaddingTop {
			get { return GetUnitProperty("PaddingTop", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "PaddingTop");
				SetUnitProperty("PaddingTop", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PaddingsPaddingRight"),
#endif
	   DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit PaddingRight {
			get { return GetUnitProperty("PaddingRight", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "PaddingRight");
				SetUnitProperty("PaddingRight", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PaddingsPaddingBottom"),
#endif
	   DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit PaddingBottom {
			get { return GetUnitProperty("PaddingBottom", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "PaddingBottom");
				SetUnitProperty("PaddingBottom", Unit.Empty, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsEmpty {
			get {
				return Padding.IsEmpty && PaddingLeft.IsEmpty && PaddingTop.IsEmpty &&
					PaddingRight.IsEmpty && PaddingBottom.IsEmpty;
			}
		}
		public Paddings() {
		}
		public Paddings(Unit padding, Unit paddingLeft, Unit paddingTop, Unit paddingRight, Unit paddingBottom) {
			Padding = padding;
			PaddingLeft = paddingLeft;
			PaddingTop = paddingTop;
			PaddingRight = paddingRight;
			PaddingBottom = paddingBottom;
		}
		public Paddings(Unit padding)
			: this(padding, Unit.Empty, Unit.Empty, Unit.Empty, Unit.Empty) {
		}
		public Paddings(Unit paddingLeft, Unit paddingTop, Unit paddingRight, Unit paddingBottom)
			: this(Unit.Empty, paddingLeft, paddingTop, paddingRight, paddingBottom) {
		}
		public Unit GetPaddingLeft() {
			return PaddingLeft.IsEmpty ? Padding : PaddingLeft;
		}
		public Unit GetPaddingTop() {
			return PaddingTop.IsEmpty ? Padding : PaddingTop;
		}
		public Unit GetPaddingRight() {
			return PaddingRight.IsEmpty ? Padding : PaddingRight;
		}
		public Unit GetPaddingBottom() {
			return PaddingBottom.IsEmpty ? Padding : PaddingBottom;
		}
		public override string ToString() {
			string s = "";
			if (!GetPaddingBottom().IsEmpty)
				s += HtmlConvertor.EncodeUnit(GetPaddingBottom()) + " ";
			if (!GetPaddingLeft().IsEmpty)
				s += HtmlConvertor.EncodeUnit(GetPaddingLeft()) + " ";
			if (!GetPaddingRight().IsEmpty)
				s += HtmlConvertor.EncodeUnit(GetPaddingRight()) + " ";
			if (!GetPaddingTop().IsEmpty)
				s += HtmlConvertor.EncodeUnit(GetPaddingTop()) + " ";
			return s;
		}
		public void Assign(Paddings source) {
			Reset();
			CopyFrom(source);
		}
		public void CopyFrom(Paddings paddings) {
			if (!paddings.Padding.IsEmpty) {
				Padding = paddings.Padding;
				PaddingLeft = Unit.Empty;
				PaddingTop = Unit.Empty;
				PaddingRight = Unit.Empty;
				PaddingBottom = Unit.Empty;
			}
			if (!paddings.PaddingLeft.IsEmpty)
				PaddingLeft = paddings.PaddingLeft;
			if (!paddings.PaddingTop.IsEmpty)
				PaddingTop = paddings.PaddingTop;
			if (!paddings.PaddingRight.IsEmpty)
				PaddingRight = paddings.PaddingRight;
			if (!paddings.PaddingBottom.IsEmpty)
				PaddingBottom = paddings.PaddingBottom;
		}
		public void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver) {
			if (!GetPaddingLeft().IsEmpty)
				attributes.Add("padding-left", HtmlConvertor.EncodeUnit(GetPaddingLeft()));
			if (!GetPaddingTop().IsEmpty)
				attributes.Add("padding-top", HtmlConvertor.EncodeUnit(GetPaddingTop()));
			if (!GetPaddingRight().IsEmpty)
				attributes.Add("padding-right", HtmlConvertor.EncodeUnit(GetPaddingRight()));
			if (!GetPaddingBottom().IsEmpty)
				attributes.Add("padding-bottom", HtmlConvertor.EncodeUnit(GetPaddingBottom()));
		}
		public void MergeWith(Paddings paddings) {
			if (!paddings.Padding.IsEmpty && Padding.IsEmpty) {
				Padding = paddings.Padding;
				PaddingLeft = Unit.Empty;
				PaddingTop = Unit.Empty;
				PaddingRight = Unit.Empty;
				PaddingBottom = Unit.Empty;
			}
			if (!paddings.PaddingLeft.IsEmpty && PaddingLeft.IsEmpty)
				PaddingLeft = paddings.PaddingLeft;
			if (!paddings.PaddingTop.IsEmpty && PaddingTop.IsEmpty)
				PaddingTop = paddings.PaddingTop;
			if (!paddings.PaddingRight.IsEmpty && PaddingRight.IsEmpty)
				PaddingRight = paddings.PaddingRight;
			if (!paddings.PaddingBottom.IsEmpty && PaddingBottom.IsEmpty)
				PaddingBottom = paddings.PaddingBottom;
		}
		public void Reset() {
			Padding = Unit.Empty;
			PaddingLeft = Unit.Empty;
			PaddingTop = Unit.Empty;
			PaddingRight = Unit.Empty;
			PaddingBottom = Unit.Empty;
		}
		public void AssignToControl(WebControl control) {
			RenderUtils.SetPaddings(control, this);
		}
		internal static  Unit[] ToArray(Paddings paddings) {
			return paddings.IsEmpty ? null : new Unit[] { paddings.GetPaddingTop(), 
				paddings.GetPaddingRight(), paddings.GetPaddingBottom(), paddings.GetPaddingLeft() };
		}
		internal static Paddings LoadFromArray(Unit[] array) {
			return array == null ? new Paddings() : new Paddings(array[0], array[1], array[2], array[3]);
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Margins : StateManager {
		[
#if !SL
	DevExpressWebLocalizedDescription("MarginsMargin"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit Margin
		{
			get { return GetUnitProperty("Margin", Unit.Empty); }
			set { SetUnitProperty("Margin", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MarginsMarginLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit MarginLeft
		{
			get { return GetUnitProperty("MarginLeft", Unit.Empty); }
			set { SetUnitProperty("MarginLeft", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MarginsMarginTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit MarginTop
		{
			get { return GetUnitProperty("MarginTop", Unit.Empty); }
			set { SetUnitProperty("MarginTop", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MarginsMarginRight"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit MarginRight
		{
			get { return GetUnitProperty("MarginRight", Unit.Empty); }
			set { SetUnitProperty("MarginRight", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MarginsMarginBottom"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit MarginBottom
		{
			get { return GetUnitProperty("MarginBottom", Unit.Empty); }
			set { SetUnitProperty("MarginBottom", Unit.Empty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsEmpty {
			get {
				return Margin.IsEmpty && MarginLeft.IsEmpty && MarginTop.IsEmpty &&
					MarginRight.IsEmpty && MarginBottom.IsEmpty;
			}
		}
		public Margins() {
		}
		public Margins(Unit margin, Unit marginLeft, Unit marginTop, Unit marginRight, Unit marginBottom) {
			Margin = margin;
			MarginLeft = marginLeft;
			MarginTop = marginTop;
			MarginRight = marginRight;
			MarginBottom = marginBottom;
		}
		public Margins(Unit margin)
			: this(margin, Unit.Empty, Unit.Empty, Unit.Empty, Unit.Empty) {
		}
		public Margins(Unit marginLeft, Unit marginTop, Unit marginRight, Unit marginBottom)
			: this(Unit.Empty, marginLeft, marginTop, marginRight, marginBottom) {
		}
		public Unit GetMarginLeft() {
			return MarginLeft.IsEmpty ? Margin : MarginLeft;
		}
		public Unit GetMarginTop() {
			return MarginTop.IsEmpty ? Margin : MarginTop;
		}
		public Unit GetMarginRight() {
			return MarginRight.IsEmpty ? Margin : MarginRight;
		}
		public Unit GetMarginBottom() {
			return MarginBottom.IsEmpty ? Margin : MarginBottom;
		}
		public override string ToString() {
			string s = "";
			if(!GetMarginBottom().IsEmpty)
				s += HtmlConvertor.EncodeUnit(GetMarginBottom()) + " ";
			if(!GetMarginLeft().IsEmpty)
				s += HtmlConvertor.EncodeUnit(GetMarginLeft()) + " ";
			if(!GetMarginRight().IsEmpty)
				s += HtmlConvertor.EncodeUnit(GetMarginRight()) + " ";
			if(!GetMarginTop().IsEmpty)
				s += HtmlConvertor.EncodeUnit(GetMarginTop()) + " ";
			return s;
		}
		public void Assign(Margins source) {
			Reset();
			CopyFrom(source);
		}
		public void CopyFrom(Margins margins) {
			if(!margins.Margin.IsEmpty) {
				Margin = margins.Margin;
				MarginLeft = Unit.Empty;
				MarginTop = Unit.Empty;
				MarginRight = Unit.Empty;
				MarginBottom = Unit.Empty;
			}
			if(!margins.MarginLeft.IsEmpty)
				MarginLeft = margins.MarginLeft;
			if(!margins.MarginTop.IsEmpty)
				MarginTop = margins.MarginTop;
			if(!margins.MarginRight.IsEmpty)
				MarginRight = margins.MarginRight;
			if(!margins.MarginBottom.IsEmpty)
				MarginBottom = margins.MarginBottom;
		}
		public void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver) {
			if(!GetMarginLeft().IsEmpty)
				attributes.Add("margin-left", HtmlConvertor.EncodeUnit(GetMarginLeft()));
			if(!GetMarginTop().IsEmpty)
				attributes.Add("margin-top", HtmlConvertor.EncodeUnit(GetMarginTop()));
			if(!GetMarginRight().IsEmpty)
				attributes.Add("margin-right", HtmlConvertor.EncodeUnit(GetMarginRight()));
			if(!GetMarginBottom().IsEmpty)
				attributes.Add("margin-bottom", HtmlConvertor.EncodeUnit(GetMarginBottom()));
		}
		public void MergeWith(Margins margins) {
			if(!margins.Margin.IsEmpty && Margin.IsEmpty) {
				Margin = margins.Margin;
				MarginLeft = Unit.Empty;
				MarginTop = Unit.Empty;
				MarginRight = Unit.Empty;
				MarginBottom = Unit.Empty;
			}
			if(!margins.MarginLeft.IsEmpty && MarginLeft.IsEmpty)
				MarginLeft = margins.MarginLeft;
			if(!margins.MarginTop.IsEmpty && MarginTop.IsEmpty)
				MarginTop = margins.MarginTop;
			if(!margins.MarginRight.IsEmpty && MarginRight.IsEmpty)
				MarginRight = margins.MarginRight;
			if(!margins.MarginBottom.IsEmpty && MarginBottom.IsEmpty)
				MarginBottom = margins.MarginBottom;
		}
		public void Reset() {
			Margin = Unit.Empty;
			MarginLeft = Unit.Empty;
			MarginTop = Unit.Empty;
			MarginRight = Unit.Empty;
			MarginBottom = Unit.Empty;
		}
		public void AssignToControl(WebControl control) {
			RenderUtils.SetMargins(control, this);
		}
	}
	[Flags]
	public enum AttributesRange { All = 0xFF, Common = 1, Cell = 2, Font = 4, Paddings = 8, Margins = 16 };
	[AutoFormatUrlPropertyClass]
	public class AppearanceStyleBase : Style, IStateManager, IPropertiesDirtyTracker {
		private BackgroundImage backgroundImage;
		private BorderWrapper border;
		private BorderLeft borderLeft;
		private BorderTop borderTop;
		private BorderRight borderRight;
		private BorderBottom borderBottom;
		private DisabledStyle disabledStyle;
		private AppearanceSelectedStyle hoverStyle;
		private AppearanceSelectedStyle pressedStyle;
		private AppearanceSelectedStyle selectedStyle;
		private Paddings paddings;
		private Margins margins;
		static GetStateManagerObject[] getObjects;
		private bool isModified;
		protected BrowserInfo Browser {
			get { return RenderUtils.Browser; }
		}
		protected T GetObject<T>(ref T field, bool create) where T : IStateManager, new() {
			if(field == null && create)
				TrackViewState(field = new T());
			return field;
		}
		protected T CreateObject<T>(ref T field) where T : IStateManager, new() {
			return GetObject(ref field, true);
		}
		protected Paddings GetObject(ref Paddings field, bool create) {
			if(field == null && create)
				TrackViewState(field = new Paddings());
			return field;
		}
		protected Paddings CreateObject(ref Paddings field) {
			return GetObject(ref field, true);
		}
		protected AppearanceStyle GetObject(ref AppearanceStyle field, bool create) {
			if(field == null && create)
				TrackViewState(field = new AppearanceStyle());
			return field;
		}
		protected AppearanceStyle CreateObject(ref AppearanceStyle field) {
			return GetObject(ref field, true);
		}
		protected AppearanceSelectedStyle GetObject(ref AppearanceSelectedStyle field, bool create) {
			if(field == null && create)
				TrackViewState(field = new AppearanceSelectedStyle());
			return field;
		}
		protected AppearanceSelectedStyle CreateObject(ref AppearanceSelectedStyle field) {
			return GetObject(ref field, true);
		}
		BorderTop GetObject(ref BorderTop field, bool create) {
			if(field == null && create)
				TrackViewState(field = new BorderTop());
			return field;
		}
		BorderLeft GetObject(ref BorderLeft field, bool create) {
			if(field == null && create)
				TrackViewState(field = new BorderLeft());
			return field;
		}
		BorderRight GetObject(ref BorderRight field, bool create) {
			if(field == null && create)
				TrackViewState(field = new BorderRight());
			return field;
		}
		BorderBottom GetObject(ref BorderBottom field, bool create) {
			if(field == null && create)
				TrackViewState(field = new BorderBottom());
			return field;
		}
		protected static StateBag emptyViewState = new StateBag();
		public const int DefaultOpacity = -1;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color BorderColor {
			get { return base.BorderColor; }
			set { base.BorderColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BorderStyle BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Unit BorderWidth {
			get { return base.BorderWidth; }
			set { base.BorderWidth = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleBaseCursor"),
#endif
		Category("Appearance"), NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		TypeConverter("DevExpress.Web.Design.CursorConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), AutoFormatEnable]
		public virtual string Cursor {
			get { return ViewStateUtils.GetStringProperty(ReadOnlyViewState, "Cursor", ""); }
			set { ViewStateUtils.SetStringProperty(ViewState, "Cursor", "", value); }
		}
		static object horizontalAlignNotSet = HorizontalAlign.NotSet;
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleBaseHorizontalAlign"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(HorizontalAlign.NotSet), AutoFormatEnable]
		public virtual HorizontalAlign HorizontalAlign {
			get { return (HorizontalAlign)ViewStateUtils.GetEnumProperty(ReadOnlyViewState, "HorizontalAlign", horizontalAlignNotSet); }
			set { ViewStateUtils.SetEnumProperty(ViewState, "HorizontalAlign", horizontalAlignNotSet, value); }
		}
		static object verticalAlignNotSet = VerticalAlign.NotSet;
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleBaseVerticalAlign"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(VerticalAlign.NotSet), AutoFormatEnable]
		public virtual VerticalAlign VerticalAlign {
			get { return (VerticalAlign)ViewStateUtils.GetEnumProperty(ReadOnlyViewState, "VerticalAlign", verticalAlignNotSet); }
			set { ViewStateUtils.SetEnumProperty(ViewState, "VerticalAlign", verticalAlignNotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleBaseWrap"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(DefaultBoolean.Default), AutoFormatEnable]
		public virtual DefaultBoolean Wrap {
			get { return (DefaultBoolean)ViewStateUtils.GetDefaultBooleanProperty(ReadOnlyViewState, "Wrap", DefaultBoolean.Default); }
			set { ViewStateUtils.SetDefaultBooleanProperty(ViewState, "Wrap", DefaultBoolean.Default, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual new Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual new Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleBaseBackgroundImage"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BackgroundImage BackgroundImage {
			get {
				return CreateObject(ref backgroundImage);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleBaseBorder"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BorderWrapper Border {
			get {
				if(border == null)
					border = new BorderWrapper(this);
				return border;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleBaseBorderLeft"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderLeft {
			get {
				return GetObject(ref borderLeft, true);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleBaseBorderTop"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderTop {
			get {
				return GetObject(ref borderTop, true);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleBaseBorderRight"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderRight {
			get {
				return GetObject(ref borderRight, true);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleBaseBorderBottom"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderBottom {
			get {
				return GetObject(ref borderBottom, true);
			}
		}
		DisabledStyle CreateDisabledStyle(bool create) {
			if(disabledStyle == null && create)
				TrackViewState(disabledStyle = CreateDisabledStyle());
			return disabledStyle;
		}
		protected internal virtual DisabledStyle DisabledStyle {
			get {
				return CreateDisabledStyle(true);
			}
		}
		AppearanceSelectedStyle CreateHoverStyle(bool create) {
			if(hoverStyle == null && create)
				TrackViewState(hoverStyle = CreateHoverStyle());
			return hoverStyle;
		}
		protected internal virtual AppearanceSelectedStyle HoverStyle {
			get {
				return CreateHoverStyle(true);
			}
		}
		AppearanceSelectedStyle CreatePressedStyle(bool create) {
			if(pressedStyle == null && create)
				TrackViewState(pressedStyle = CreatePressedStyle());
			return pressedStyle;
		}
		protected internal virtual AppearanceSelectedStyle PressedStyle {
			get {
				return CreatePressedStyle(true);
			}
		}
		AppearanceSelectedStyle CreateSelectedStyle(bool create) {
			if(selectedStyle == null && create)
				TrackViewState(selectedStyle = CreateSelectedStyle());
			return selectedStyle;
		}
		protected internal virtual AppearanceSelectedStyle SelectedStyle {
			get {
				return CreateSelectedStyle(true);
			}
		}
		protected internal virtual Unit ImageSpacing {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "ImageSpacing", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ImageSpacing");
				ViewStateUtils.SetUnitProperty(ViewState, "ImageSpacing", Unit.Empty, value);
			}
		}
		protected internal virtual Unit LineHeight {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "LineHeight", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "LineHeight");
				ViewStateUtils.SetUnitProperty(ViewState, "LineHeight", Unit.Empty, value);
			}
		}
		protected internal virtual int Opacity {
			get { return ViewStateUtils.GetIntProperty(ReadOnlyViewState, "Opacity", DefaultOpacity); }
			set {
				CommonUtils.CheckValueRange(value, -1, 100, "Opacity");
				ViewStateUtils.SetIntProperty(ViewState, "Opacity", DefaultOpacity, value);
			}
		}
		protected internal virtual Paddings Paddings {
			get {
				return CreateObject(ref paddings);
			}
		}
		protected internal virtual Margins Margins {
			get {
				return CreateObject(ref margins);
			}
		}
		protected internal virtual Unit Spacing {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "Spacing", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "Spacing");
				ViewStateUtils.SetUnitProperty(ViewState, "Spacing", Unit.Empty, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsEmpty {
			get {
				return base.IsEmpty &&
					(backgroundImage == null || BackgroundImage.IsEmpty) &&
					(borderLeft == null || BorderLeft.IsEmpty) &&
					(borderTop == null || BorderTop.IsEmpty) &&
					(borderRight == null || BorderRight.IsEmpty) &&
					(borderBottom == null || BorderBottom.IsEmpty) &&
					(disabledStyle == null || DisabledStyle.IsEmpty) &&
					(hoverStyle == null || HoverStyle.IsEmpty) &&
					(pressedStyle == null || PressedStyle.IsEmpty) &&
					(selectedStyle == null || SelectedStyle.IsEmpty) && 
					(paddings == null || Paddings.IsEmpty) &&
					(margins == null || Margins.IsEmpty) &&
					(!IsModified || (LineHeight.IsEmpty && ImageSpacing.IsEmpty && Spacing.IsEmpty &&
					Cursor == "" && HorizontalAlign == HorizontalAlign.NotSet &&
					VerticalAlign == VerticalAlign.NotSet &&
					Wrap == DefaultBoolean.Default && Opacity == DefaultOpacity));
			}
		}
		protected bool IsModified {
			get { return isModified; }
			private set { isModified = value; }
		}
		protected StateBag ReadOnlyViewState {
			get {
				return IsModified ? base.ViewState : emptyViewState;
			}
		}
		protected new StateBag ViewState {
			get {
				IsModified = true;
				return base.ViewState;
			}
		}
		protected void TrackViewState(IStateManager stateManaged) {
			if(stateManaged != null && IsTrackingViewState)
				stateManaged.TrackViewState();
		}
		public void AssignWithoutBorders(AppearanceStyleBase source) {
			Reset();
			CopyFrom(source, true);
		}
		public void Assign(AppearanceStyleBase source) {
			Reset();
			CopyFrom(source);
		}
		public override void AddAttributesToRender(HtmlTextWriter writer, WebControl owner) {
			base.AddAttributesToRender(writer, owner);
			if (Wrap == DefaultBoolean.False)
				writer.AddAttribute(HtmlTextWriterAttribute.Nowrap, "nowrap");
			if (HorizontalAlign != HorizontalAlign.NotSet)
				writer.AddAttribute(HtmlTextWriterAttribute.Align, HorizontalAlign.ToString());
			if (VerticalAlign != VerticalAlign.NotSet)
				writer.AddAttribute(HtmlTextWriterAttribute.Valign, VerticalAlign.ToString());
		}
		public override void CopyFrom(Style style) {
			CopyFrom(style, false);
		}
		public void CopyFrom(Style style, bool copyWithoutBorders) {
			if ((style != null) && !style.IsEmpty) {
				string cssClass = RenderUtils.CombineCssClasses(CssClass, style.CssClass);
				if(copyWithoutBorders) {
					Color savedBorderColor = BorderColor;
					BorderStyle savedBorderStyle = BorderStyle;
					Unit savedBorderWidth = BorderWidth;
					base.CopyFrom(style);
					BorderColor = savedBorderColor;
					BorderStyle = savedBorderStyle;
					BorderWidth = savedBorderWidth;
				} else 
					base.CopyFrom(style);
				if(CssClass != cssClass)
					CssClass = cssClass;
				if (style is AppearanceStyleBase) {
					AppearanceStyleBase appearanceStyle = style as AppearanceStyleBase;
					if(appearanceStyle.IsModified) {
						if(appearanceStyle.Cursor != "")
							Cursor = appearanceStyle.Cursor;
						if(appearanceStyle.HorizontalAlign != HorizontalAlign.NotSet)
							HorizontalAlign = appearanceStyle.HorizontalAlign;
						if(appearanceStyle.VerticalAlign != VerticalAlign.NotSet)
							VerticalAlign = appearanceStyle.VerticalAlign;
						if(appearanceStyle.Wrap != DefaultBoolean.Default)
							Wrap = appearanceStyle.Wrap;
						if(!appearanceStyle.ImageSpacing.IsEmpty)
							ImageSpacing = appearanceStyle.ImageSpacing;
						if(!appearanceStyle.LineHeight.IsEmpty)
							LineHeight = appearanceStyle.LineHeight;
						if(appearanceStyle.Opacity != DefaultOpacity)
							Opacity = appearanceStyle.Opacity;
						if(!appearanceStyle.Spacing.IsEmpty)
							Spacing = appearanceStyle.Spacing;
					}
					if(appearanceStyle.backgroundImage != null)
						BackgroundImage.CopyFrom(appearanceStyle.BackgroundImage);
					if(!copyWithoutBorders) {
						if(!appearanceStyle.BorderColor.IsEmpty)
							BorderColor = appearanceStyle.BorderColor;
						if(appearanceStyle.BorderStyle != BorderStyle.NotSet)
							BorderStyle = appearanceStyle.BorderStyle;
						if(!appearanceStyle.BorderWidth.IsEmpty)
							BorderWidth = appearanceStyle.BorderWidth;
						if(appearanceStyle.borderLeft != null)
							BorderLeft.CopyFrom(appearanceStyle.BorderLeft);
						if(appearanceStyle.borderTop != null)
							BorderTop.CopyFrom(appearanceStyle.BorderTop);
						if(appearanceStyle.borderRight != null)
							BorderRight.CopyFrom(appearanceStyle.BorderRight);
						if(appearanceStyle.borderBottom != null)
							BorderBottom.CopyFrom(appearanceStyle.BorderBottom);
					}
					if(appearanceStyle.disabledStyle != null && DisabledStyle != null)
						DisabledStyle.CopyFrom(appearanceStyle.DisabledStyle);
					if(appearanceStyle.hoverStyle != null && HoverStyle != null)
						HoverStyle.CopyFrom(appearanceStyle.HoverStyle);
					if(appearanceStyle.pressedStyle != null && PressedStyle != null)
						PressedStyle.CopyFrom(appearanceStyle.PressedStyle);
					if(appearanceStyle.selectedStyle != null && SelectedStyle != null)
						SelectedStyle.CopyFrom(appearanceStyle.SelectedStyle);
					if(appearanceStyle.paddings != null)
						Paddings.CopyFrom(appearanceStyle.Paddings);
					if(appearanceStyle.margins != null)
						Margins.CopyFrom(appearanceStyle.Margins);
				}
			}
		}
		public void CopyFontFrom(Style style) {
			Font.CopyFrom(style.Font);
			if (!style.ForeColor.IsEmpty)
				ForeColor = style.ForeColor;
		}
		public void CopyFontAndCursorFrom(Style style) {
			CopyFontFrom(style);
			if(style is AppearanceStyleBase) {
				AppearanceStyleBase appearanceStyle = (AppearanceStyleBase)style;
				if(appearanceStyle.Cursor != "")
					Cursor = appearanceStyle.Cursor;
			}
		}
		public void CopyBordersFrom(Style style) {
			if (!style.BorderColor.IsEmpty)
				BorderColor = style.BorderColor;
			if (style.BorderStyle != BorderStyle.NotSet)
				BorderStyle = style.BorderStyle;
			if (!style.BorderWidth.IsEmpty)
				BorderWidth = style.BorderWidth;
			if (style is AppearanceStyleBase) {
				AppearanceStyleBase appearanceStyle = (AppearanceStyleBase)style;
				if(appearanceStyle.borderLeft != null)
					BorderLeft.CopyFrom(appearanceStyle.BorderLeft);
				if(appearanceStyle.borderTop != null)
					BorderTop.CopyFrom(appearanceStyle.BorderTop);
				if(appearanceStyle.borderRight != null)
					BorderRight.CopyFrom(appearanceStyle.BorderRight);
				if(appearanceStyle.borderBottom != null)
					BorderBottom.CopyFrom(appearanceStyle.BorderBottom);
			}
		}
		public void CopyTextDecorationFrom(Style style) {
			if(style.Font.Underline)
				Font.Underline = style.Font.Underline;
			if(style.Font.Overline)
				Font.Overline = style.Font.Overline;
			if(style.Font.Strikeout)
				Font.Strikeout = style.Font.Strikeout;
		}
		public override void MergeWith(Style style) {
			if ((style != null) && !style.IsEmpty) {
				CssClass = RenderUtils.CombineCssClasses(style.CssClass, CssClass);
				string cssClass = CssClass;
				base.MergeWith(style);
				CssClass = cssClass;
				if (style is AppearanceStyleBase) {
					AppearanceStyleBase appearanceStyle = style as AppearanceStyleBase;
					if (appearanceStyle.Cursor != "" && Cursor == "")
						Cursor = appearanceStyle.Cursor;
					if (appearanceStyle.HorizontalAlign != HorizontalAlign.NotSet && HorizontalAlign == HorizontalAlign.NotSet)
						HorizontalAlign = appearanceStyle.HorizontalAlign;
					if (appearanceStyle.VerticalAlign != VerticalAlign.NotSet && VerticalAlign == VerticalAlign.NotSet)
						VerticalAlign = appearanceStyle.VerticalAlign;
					if (appearanceStyle.Wrap != DefaultBoolean.Default && Wrap == DefaultBoolean.Default)
						Wrap = appearanceStyle.Wrap;
					if(appearanceStyle.backgroundImage != null)
						BackgroundImage.MergeWith(appearanceStyle.BackgroundImage);
					if(appearanceStyle.borderLeft != null)
						BorderLeft.MergeWith(appearanceStyle.BorderLeft);
					if(appearanceStyle.borderTop != null)
						BorderTop.MergeWith(appearanceStyle.BorderTop);
					if(appearanceStyle.borderRight != null)
						BorderRight.MergeWith(appearanceStyle.BorderRight);
					if(appearanceStyle.borderBottom != null)
						BorderBottom.MergeWith(appearanceStyle.BorderBottom);
					if(appearanceStyle.disabledStyle != null && DisabledStyle != null)
						DisabledStyle.MergeWith(appearanceStyle.DisabledStyle);
					if(appearanceStyle.hoverStyle != null && HoverStyle != null)
						HoverStyle.MergeWith(appearanceStyle.HoverStyle);
					if(appearanceStyle.pressedStyle != null && PressedStyle != null)
						PressedStyle.MergeWith(appearanceStyle.PressedStyle);
					if(appearanceStyle.selectedStyle != null && SelectedStyle != null)
						SelectedStyle.MergeWith(appearanceStyle.SelectedStyle);
					if(!appearanceStyle.ImageSpacing.IsEmpty && ImageSpacing.IsEmpty)
						ImageSpacing = appearanceStyle.ImageSpacing;
					if(!appearanceStyle.LineHeight.IsEmpty && LineHeight.IsEmpty)
						LineHeight = appearanceStyle.LineHeight;
					if(appearanceStyle.Opacity != DefaultOpacity && Opacity == DefaultOpacity)
						Opacity = appearanceStyle.Opacity;
					if(appearanceStyle.paddings != null)
						Paddings.MergeWith(appearanceStyle.Paddings);
					if(appearanceStyle.margins != null)
						Margins.MergeWith(appearanceStyle.Margins);
					if(!appearanceStyle.Spacing.IsEmpty && Spacing.IsEmpty)
						Spacing = appearanceStyle.Spacing;
				}
			}
		}
		public void MergeBordersWith(Style style) {
			if (!style.BorderColor.IsEmpty && BorderColor.IsEmpty)
				BorderColor = style.BorderColor;
			if (style.BorderStyle != BorderStyle.NotSet && BorderStyle == BorderStyle.NotSet)
				BorderStyle = style.BorderStyle;
			if (!style.BorderWidth.IsEmpty && BorderWidth.IsEmpty)
				BorderWidth = style.BorderWidth;
			if (style is AppearanceStyleBase) {
				AppearanceStyleBase appearanceStyle = (AppearanceStyleBase)style;
				BorderLeft.MergeWith(appearanceStyle.BorderLeft);
				BorderTop.MergeWith(appearanceStyle.BorderTop);
				BorderRight.MergeWith(appearanceStyle.BorderRight);
				BorderBottom.MergeWith(appearanceStyle.BorderBottom);
			}
		}
		public void MergeFontWith(Style style) {
			Font.MergeWith(style.Font);
			if (!style.ForeColor.IsEmpty && ForeColor.IsEmpty)
				ForeColor = style.ForeColor;
		}
		public override void Reset() {
			backgroundImage = null;
			borderLeft = null;
			borderTop = null;
			borderRight = null;
			borderBottom = null;
			disabledStyle = null;
			hoverStyle = null;
			pressedStyle = null;
			selectedStyle = null;
			paddings = null;
			margins = null;
			if(isModified) {
				Cursor = "";
				HorizontalAlign = HorizontalAlign.NotSet;
				VerticalAlign = VerticalAlign.NotSet;
				Wrap = DefaultBoolean.Default;
				ImageSpacing = Unit.Empty;
				LineHeight = Unit.Empty;
				Opacity = DefaultOpacity;
				Spacing = Unit.Empty;
			}
			base.Reset();
		}
		public virtual void AssignToControl(WebControl control, AttributesRange range, bool exceptTextDecoration) {
			AssignToControl(control, range, exceptTextDecoration, false);
		}
		public virtual void AssignToControl(WebControl control, AttributesRange range, bool exceptTextDecoration, bool useBlockAlignment) {
			AssignToControl(control, range, exceptTextDecoration, useBlockAlignment, false);
		}
		public virtual void AssignToControl(WebControl control, AttributesRange range, bool exceptTextDecoration, bool useBlockAlignment, bool exceptOpacity) {
			if ((range & AttributesRange.Common) != 0) {
				if(BackColor != control.ControlStyle.BackColor)
					control.ControlStyle.BackColor = BackColor;
				if(BorderColor != control.ControlStyle.BorderColor)
					control.ControlStyle.BorderColor = BorderColor;
				if(BorderStyle != control.ControlStyle.BorderStyle)
					control.ControlStyle.BorderStyle = BorderStyle;
				if(BorderWidth != control.ControlStyle.BorderWidth)
					control.ControlStyle.BorderWidth = BorderWidth;
				if(CssClass != control.ControlStyle.CssClass)
					control.ControlStyle.CssClass = RenderUtils.CombineCssClasses(control.ControlStyle.CssClass, CssClass);
				if(backgroundImage != null) {
					BackgroundImage.AssignToControl(control);
				}
				if(borderLeft != null)
					BorderLeft.AssignToControl(control);
				if(borderTop != null)
					BorderTop.AssignToControl(control);
				if(borderRight != null)
					BorderRight.AssignToControl(control);
				if(borderBottom != null)
					BorderBottom.AssignToControl(control);
				if(Cursor != string.Empty)
					RenderUtils.SetCursor(control, Cursor);
				if(!exceptOpacity && Opacity != DefaultOpacity)
					RenderUtils.SetOpacity(control, Opacity);
			}
			if((range & AttributesRange.Cell) != 0) {
				if(useBlockAlignment && HorizontalAlign != HorizontalAlign.NotSet && HorizontalAlign != HorizontalAlign.Justify && control is TableCell) 
					((TableCell)control).HorizontalAlign = HorizontalAlign;
				else
					RenderUtils.SetStyleStringAttribute(control, "text-align", HorizontalAlign != HorizontalAlign.NotSet ? HorizontalAlign.ToString() : "");
				RenderUtils.SetStyleStringAttribute(control, "vertical-align", VerticalAlign != VerticalAlign.NotSet ? VerticalAlign.ToString() : "");
				RenderUtils.SetWrap(control, Wrap);
			}
			if((range & AttributesRange.Font) != 0) {
				ResetFont(control.ControlStyle.Font);
				if(ForeColor != control.ControlStyle.ForeColor)
					control.ControlStyle.ForeColor = ForeColor;
				if (IsBoldStored(Font))
					control.ControlStyle.Font.Bold = Font.Bold;
				if (IsItalicStored(Font))
					control.ControlStyle.Font.Italic = Font.Italic;
				if(!CommonUtils.AreEqualsArrays(Font.Names, control.ControlStyle.Font.Names))
					control.ControlStyle.Font.Names = Font.Names;
				if(Font.Size != control.ControlStyle.Font.Size)
					control.ControlStyle.Font.Size = Font.Size;
				if (!exceptTextDecoration) {
					if (IsOverlineStored(Font))
						control.ControlStyle.Font.Overline = Font.Overline;
					if (IsStrikeoutStored(Font))
						control.ControlStyle.Font.Strikeout = Font.Strikeout;
					if (IsUnderlineStored(Font))
						control.ControlStyle.Font.Underline = Font.Underline;
				}
			}
			if((range & AttributesRange.Paddings) != 0) {
			}
			if((range & AttributesRange.Margins) != 0) {
			}
		}
		public virtual void AssignToControl(WebControl control, AttributesRange range) {
			AssignToControl(control, range, false);
		}
		public void AssignToControl(WebControl control) {
			AssignToControl(control, AttributesRange.All);
		}
		public void AssignToControl(WebControl control, bool applyPaddings) {
			AssignToControl(control);
			if(applyPaddings && paddings != null)
				Paddings.AssignToControl(control);
		}
		public void AssignToHyperLink(HyperLink hyperLink) {
			AssignToHyperLink(hyperLink, false);
		}
		public void AssignToHyperLink(HyperLink hyperLink, bool cursorOnly) {
			AssignToHyperLink((WebControl)hyperLink, cursorOnly);
		}
		public void AssignToHyperLink(WebControl hyperLink, bool cursorOnly) {
			if(!cursorOnly) {
				AppearanceStyleBase style = this;
				if(style.IsFontSizeRelative()) {
					style = new AppearanceStyleBase();
					style.CopyFrom(this);
					style.Font.Size = FontUnit.Empty;
				}
				style.AssignToControl(hyperLink, AttributesRange.Font);
			}
			if(!string.IsNullOrEmpty(Cursor)){
				if(hyperLink is InternalHyperLink && (((InternalHyperLink)hyperLink).NavigateUrl == "" || Cursor != RenderUtils.GetPointerCursor()))
					RenderUtils.SetCursor(hyperLink, Cursor);
				else if(Cursor != RenderUtils.GetPointerCursor())
					RenderUtils.SetCursor(hyperLink, Cursor);
			}
		}
		protected internal void FillStyleAttributesInternal(CssStyleCollection attributes, IUrlResolutionService urlResolver) {
			base.FillStyleAttributes(attributes, urlResolver);
			if(Cursor != "")
				attributes.Add(HtmlTextWriterStyle.Cursor, Cursor);
			if(Opacity != DefaultOpacity)
				RenderUtils.SetOpacity(attributes, Opacity);
			if(backgroundImage != null && !BackgroundImage.IsEmpty)
				BackgroundImage.FillStyleAttributes(attributes, urlResolver);
			if(borderLeft != null || borderRight != null || borderTop != null || borderBottom != null) {
				attributes.Remove(HtmlTextWriterStyle.BorderColor);
				attributes.Remove(HtmlTextWriterStyle.BorderStyle);
				attributes.Remove(HtmlTextWriterStyle.BorderWidth);
				FillBorder(attributes, urlResolver, new BorderBottom(), borderBottom);
				FillBorder(attributes, urlResolver, new BorderTop(), borderTop);
				FillBorder(attributes, urlResolver, new BorderLeft(), borderLeft);
				FillBorder(attributes, urlResolver, new BorderRight(), borderRight);
			}
			if(paddings != null && !Paddings.IsEmpty)
				Paddings.FillStyleAttributes(attributes, urlResolver);
			if(margins != null && !Margins.IsEmpty)
				Margins.FillStyleAttributes(attributes, urlResolver);
		}
		void FillBorder(CssStyleCollection attributes, IUrlResolutionService urlResolver, Border border, Border srcBorder) {
			if(srcBorder != null)
				border.CopyFrom(srcBorder);
			border.MergeWith(Border);
			border.FillStyleAttributes(attributes, urlResolver);
		}
		protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver) {
			FillStyleAttributesInternal(attributes, urlResolver);
		}
		protected virtual DisabledStyle CreateDisabledStyle() {
			return new DisabledStyle();
		}
		protected virtual AppearanceSelectedStyle CreateHoverStyle() {
			return new AppearanceSelectedStyle();
		}
		protected virtual AppearanceSelectedStyle CreatePressedStyle() {
			return new AppearanceSelectedStyle();
		}
		protected virtual AppearanceSelectedStyle CreateSelectedStyle() {
			return new AppearanceSelectedStyle();
		}
		protected bool IsBoldStored(FontInfo font) {
			return IsFlagStored(font, "Font_Bold");
		}
		protected bool IsItalicStored(FontInfo font) {
			return IsFlagStored(font, "Font_Italic");
		}
		protected bool IsOverlineStored(FontInfo font) {
			return IsFlagStored(font, "Font_Overline");
		}
		protected bool IsStrikeoutStored(FontInfo font) {
			return IsFlagStored(font, "Font_Strikeout");
		}
		protected bool IsUnderlineStored(FontInfo font) {
			return IsFlagStored(font, "Font_Underline");
		}
		protected bool IsNamesStored(FontInfo font) {
			return IsFlagStored(font, "Font_Names");
		}
		bool IsFlagStored(FontInfo font, string flagName) {
			return ViewState[flagName] != null;
		}
		public Color GetBorderColorLeft() {
			return borderLeft == null || BorderLeft.BorderColor.IsEmpty ? BorderColor : BorderLeft.BorderColor;
		}
		public Color GetBorderColorTop() {
			return borderTop == null || BorderTop.BorderColor.IsEmpty ? BorderColor : BorderTop.BorderColor;
		}
		public Color GetBorderColorRight() {
			return borderRight == null || BorderRight.BorderColor.IsEmpty ? BorderColor : BorderRight.BorderColor;
		}
		public Color GetBorderColorBottom() {
			return borderBottom == null || BorderBottom.BorderColor.IsEmpty ? BorderColor : BorderBottom.BorderColor;
		}
		public BorderStyle GetBorderStyleLeft() {
			return borderLeft == null || BorderLeft.BorderStyle == BorderStyle.NotSet ? BorderStyle : BorderLeft.BorderStyle;
		}
		public BorderStyle GetBorderStyleTop() {
			return borderTop == null || BorderTop.BorderStyle == BorderStyle.NotSet ? BorderStyle : BorderTop.BorderStyle;
		}
		public BorderStyle GetBorderStyleRight() {
			return borderRight == null || BorderRight.BorderStyle == BorderStyle.NotSet ? BorderStyle : BorderRight.BorderStyle;
		}
		public BorderStyle GetBorderStyleBottom() {
			return borderBottom == null || BorderBottom.BorderStyle == BorderStyle.NotSet ? BorderStyle : BorderBottom.BorderStyle;
		}
		public Unit GetBorderWidthLeft() {
			return borderLeft == null || BorderLeft.BorderWidth.IsEmpty ? BorderWidth : BorderLeft.BorderWidth;
		}
		public Unit GetBorderWidthTop() {
			return borderTop == null || BorderTop.BorderWidth.IsEmpty ? BorderWidth : BorderTop.BorderWidth;
		}
		public Unit GetBorderWidthRight() {
			return borderRight == null || BorderRight.BorderWidth.IsEmpty ? BorderWidth : BorderRight.BorderWidth;
		}
		public Unit GetBorderWidthBottom() {
			return borderBottom == null || BorderBottom.BorderWidth.IsEmpty ? BorderWidth : BorderBottom.BorderWidth;
		}
		protected void ResetFont(FontInfo font) {
			if(font.Name != String.Empty)
				font.Name = String.Empty;
			if(font.Size != FontUnit.Empty)
				font.Size = FontUnit.Empty;
			if(font.Bold != false)
				font.Bold = false;
			if(font.Italic != false)
				font.Italic = false;
			if(font.Underline != false)
				font.Underline = false;
			if(font.Overline != false)
				font.Overline = false;
			if(font.Strikeout != false)
				font.Strikeout = false;
			font.ClearDefaults();
		}
		protected internal bool IsFontSizeRelative() {
			return Font.Size.Unit.Type == UnitType.Em || Font.Size.Unit.Type == UnitType.Percentage;
		}
		protected virtual GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			if(getObjects == null) {
#pragma warning disable 197
				getObjects = new GetStateManagerObject[] {
			delegate(object style, bool create) { return ((AppearanceStyleBase)style).GetObject(ref ((AppearanceStyleBase)style).backgroundImage, create); },
			delegate(object style, bool create) { return ((AppearanceStyleBase)style).GetObject(ref ((AppearanceStyleBase)style).borderLeft, create); },
			delegate(object style, bool create) { return ((AppearanceStyleBase)style).GetObject(ref ((AppearanceStyleBase)style).borderRight, create); },
			delegate(object style, bool create) { return ((AppearanceStyleBase)style).GetObject(ref ((AppearanceStyleBase)style).borderTop, create); },
			delegate(object style, bool create) { return ((AppearanceStyleBase)style).GetObject(ref ((AppearanceStyleBase)style).borderBottom, create); },
			delegate(object style, bool create) { return ((AppearanceStyleBase)style).GetObject(ref ((AppearanceStyleBase)style).paddings, create); },
			delegate(object style, bool create) { return ((AppearanceStyleBase)style).GetObject(ref ((AppearanceStyleBase)style).margins, create); },
			delegate(object style, bool create) { return ((AppearanceStyleBase)style).CreateDisabledStyle(create); },
			delegate(object style, bool create) { return ((AppearanceStyleBase)style).CreateSelectedStyle(create); },
			delegate(object style, bool create) { return ((AppearanceStyleBase)style).CreateHoverStyle(create); },
			delegate(object style, bool create) { return ((AppearanceStyleBase)style).CreatePressedStyle(create); },
		};
			}
#pragma warning restore 197
			return getObjects;
		}
		protected virtual void LoadStyleViewState(object savedState) {
			if (savedState != null) {
				object[] stateArray = savedState as object[];
				object baseState = (stateArray.Length > 0 && stateArray[0] != null) ? stateArray[0] : null;
				base.LoadViewState(baseState);
				ViewStateUtils.LoadObjectsViewState(this, stateArray, GetStateManagedObjectsDelegates());
			}
			else
				base.LoadViewState(null);
			IsModified = true;
		}
		protected virtual object SaveStyleViewState() {
			return ViewStateUtils.SaveViewState(this, base.SaveViewState(), GetStateManagedObjectsDelegates());
		}
		protected virtual void TrackStyleViewState() {
			base.TrackViewState();
			ViewStateUtils.TrackObjectsViewState(this, GetStateManagedObjectsDelegates());
		}
		bool System.Web.UI.IStateManager.IsTrackingViewState {
			get { return base.IsTrackingViewState; }
		}
		void System.Web.UI.IStateManager.LoadViewState(object savedState) {
			LoadStyleViewState(savedState);
		}
		object System.Web.UI.IStateManager.SaveViewState() {
			return SaveStyleViewState();
		}
		void System.Web.UI.IStateManager.TrackViewState() {
			TrackStyleViewState();
		}
		void IPropertiesDirtyTracker.SetPropertiesDirty() {
			SetDirty();
			ViewStateUtils.SetPropertiesDirty(this, null, GetStateManagedObjectsDelegates());
		}
	}
	public class AppearanceSelectedStyle : AppearanceStyleBase { 
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		protected override DisabledStyle CreateDisabledStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreatePressedStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateSelectedStyle() {
			return null;
		}
	}
	public class AppearanceInputStyle : AppearanceStyle {
		public override void AssignToControl(WebControl control, AttributesRange range, bool exceptTextDecoration, bool useBlockAlignment, bool exceptOpacity) {
			base.AssignToControl(control, range, exceptTextDecoration, useBlockAlignment, exceptOpacity);
			if(Browser.IsSafari && ForeColor.Name == Color.Black.Name) {
				control.Style.Add("-webkit-text-fill-color", ForeColor.Name);
				if(Browser.Platform.IsWebKitTouchUI)
					control.Style.Add("-webkit-opacity", "1");
			}
		}
	}
	public class AppearanceStyle : AppearanceStyleBase {
		private static AppearanceStyle empty = new AppearanceStyle();
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleHoverStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleImageSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStylePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Paddings Paddings {
			get { return base.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceStyleSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static AppearanceStyle Empty {
			get { return empty; }
		}
	}
	public class AppearanceItemStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("AppearanceItemStyleSelectedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual AppearanceSelectedStyle SelectedStyle {
			get { return base.SelectedStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class LinkStyleComponentStyle : AppearanceStyleBase {
		bool makeAttributesImportant;
		public LinkStyleComponentStyle()
			: this(false) {
		}
		public LinkStyleComponentStyle(bool makeAttributesImportant) {
			this.makeAttributesImportant = makeAttributesImportant;
		}
		protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver) {
			base.FillStyleAttributes(attributes, urlResolver);
			if(this.makeAttributesImportant)
				RenderUtils.MakeCssAttributesImportant(attributes, urlResolver);
		}
	}
	public class DisabledStyle : AppearanceSelectedStyle {
		public DisabledStyle()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DisabledStyleCursor"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LinkStyle: StateManager {
		private LinkStyleComponentStyle style;
		private LinkStyleComponentStyle hoverStyle;
		private LinkStyleComponentStyle visitedStyle;
		[
#if !SL
	DevExpressWebLocalizedDescription("LinkStyleColor"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Color), ""), AutoFormatEnable,
		TypeConverter(typeof(WebColorConverter))]
		public Color Color {
			get { return Style.ForeColor; }
			set { Style.ForeColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LinkStyleFont"),
#endif
 AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FontInfo Font {
			get { return Style.Font; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LinkStyleHoverColor"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Color), ""), AutoFormatEnable,
		TypeConverter(typeof(WebColorConverter))]
		public Color HoverColor {
			get { return HoverStyle.ForeColor; }
			set { HoverStyle.ForeColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LinkStyleHoverFont"),
#endif
 AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FontInfo HoverFont {
			get { return HoverStyle.Font; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LinkStyleVisitedColor"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Color), ""), AutoFormatEnable,
		TypeConverter(typeof(WebColorConverter))]
		public Color VisitedColor {
			get { return VisitedStyle.ForeColor; }
			set { VisitedStyle.ForeColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LinkStyleVisitedFont"),
#endif
 AutoFormatEnable,
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FontInfo VisitedFont {
			get { return VisitedStyle.Font; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LinkStyleComponentStyle Style {
			get { return style; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LinkStyleComponentStyle HoverStyle {
			get { return hoverStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LinkStyleComponentStyle VisitedStyle {
			get { return visitedStyle; }
		}
		public LinkStyle()
			: this(false) {
		}
		public LinkStyle(bool makeAttributesImportant) {
			this.style = new LinkStyleComponentStyle();
			this.hoverStyle = new LinkStyleComponentStyle(makeAttributesImportant);
			this.visitedStyle = new LinkStyleComponentStyle(makeAttributesImportant);
		}
		public void Assign(LinkStyle source) {
			Reset();
			CopyFrom(source);
		}
		public void CopyFrom(LinkStyle style) {
			Style.CopyFrom(style.Style);
			HoverStyle.CopyFrom(style.HoverStyle);
			VisitedStyle.CopyFrom(style.VisitedStyle);
		}
		public void Reset() {
			Style.Reset();
			HoverStyle.Reset();
			VisitedStyle.Reset();
		}
		public override string ToString() {
			return "";
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Style, HoverStyle, VisitedStyle };
		}
	}
	public class LoadingPanelStyle: AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return Unit.Empty; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
	}
	public class BackToTopStyle : AppearanceStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class LoadingDivStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("LoadingDivStyleOpacity"),
#endif
		Category("Appearance"), NotifyParentProperty(true), DefaultValue(DefaultOpacity), AutoFormatEnable()]
		public new virtual int Opacity {
			get { return base.Opacity; }
			set { base.Opacity = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font {
			get { return null; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor {
			get { return Color.Empty; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { base.Wrap = value; }
		}
	}
	public class ButtonStyle: AppearanceStyle { 
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonStyleDisabledStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual AppearanceSelectedStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonStylePressedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual AppearanceSelectedStyle PressedStyle {
			get { return base.PressedStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
	}
	public class StylesBase : PropertiesBase {
		public const string DisabledStyleName = "Disabled";
		public const string LoadingPanelStyleName = "LoadingPanel";
		public const string LoadingDivStyleName = "LoadingDiv";
		private LinkStyle link;
		private Dictionary<string, AppearanceStyleBase> stylesCache = new Dictionary<string, AppearanceStyleBase>();
		private Dictionary<string, AppearanceStyleBase> styles = new Dictionary<string, AppearanceStyleBase>();
		private List<StyleInfo> infoList;
		private Dictionary<string, StyleInfo> infoIndex;
		static Dictionary<Type, List<StyleInfo>> typedInfoList = new Dictionary<Type, List<StyleInfo>>();
		static Dictionary<Type, Dictionary<string, StyleInfo>> typedInfoIndex = new Dictionary<Type, Dictionary<string, StyleInfo>>();
		public static string CreateCssClassName(string prefix, string body, string postfix) {
			string result = prefix + body;
			result += !string.IsNullOrEmpty(postfix) ? "_" + postfix : string.Empty;
			return result;
		}
		public StylesBase(ISkinOwner owner) : base(owner) {
			CreateStyles();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("StylesBaseCssPostfix"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false), AutoFormatEnable]
		public virtual string CssPostfix {
			get { return GetStringProperty("CssPostfix", string.Empty); }
			set {
				if(value == CssPostfix) return;
				SetStringProperty("CssPostfix", string.Empty, value);
				StylesCache.Clear();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("StylesBaseCssFilePath"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string CssFilePath {
			get { return GetStringProperty("CssFilePath", string.Empty); }
			set {
				if(value == CssFilePath) return;
				SetStringProperty("CssFilePath", string.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("StylesBaseEnableDefaultAppearance"),
#endif
		Obsolete("This property is now obsolete. Use the corresponding style settings to override control elements' appearance."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable]
		public bool EnableDefaultAppearance {
			get { return true; }
			set { }
		}
		[DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		protected internal bool AccessibilityCompliant {
			get { return GetBoolProperty("AccessibilityCompliant", false); }
			set { SetBoolProperty("AccessibilityCompliant", false, value); }
		}
		[DefaultValue(false), NotifyParentProperty(true)]
		protected internal bool Native {
			get { return GetBoolProperty("Native", false); }
			set {
				SetBoolProperty("Native", false, value);
				StylesCache.Clear();
			}
		}
		protected internal DefaultBoolean RightToLeft {
			get { return GetDefaultBooleanProperty("RightToLeftInternal", DefaultBoolean.Default); }
			set { SetDefaultBooleanProperty("RightToLeftInternal", DefaultBoolean.Default, value); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		protected internal DisabledStyle DisabledInternal {
			get { return (DisabledStyle)GetStyle(DisabledStyleName); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Theme {
			get { return GetStringProperty("Theme", string.Empty); }
			set { SetStringProperty("Theme", string.Empty, value); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		protected internal LinkStyle LinkInternal {
			get {
				if(link == null)
					TrackViewState(link = new LinkStyle(MakeLinkStyleAttributesImportant));
				return link;
			}
		}
		protected internal bool HasLink {
			get { return link != null; }
		}
		protected virtual bool MakeLinkStyleAttributesImportant {
			get { return false; }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		protected internal LoadingPanelStyle LoadingPanelInternal {
			get { return (LoadingPanelStyle)GetStyle(LoadingPanelStyleName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		protected internal LoadingDivStyle LoadingDivInternal {
			get { return (LoadingDivStyle)GetStyle(LoadingDivStyleName); }
		}
		protected Dictionary<string, AppearanceStyleBase> StylesCache {
			get { return stylesCache; }
		}
		protected ISkinOwner SkinOwner {
			get { return Owner as ISkinOwner; }
		}
		Dictionary<string, AppearanceStyleBase> Styles {
			get { return styles; }
		}
		List<StyleInfo> InfoList {
			get { return infoList; }
		}
		Dictionary<string, StyleInfo> InfoIndex {
			get { return infoIndex; }
		}
		protected BrowserInfo Browser {
			get { return RenderUtils.Browser; }
		}
		#region Assign, CopyFrom, Reset
		public sealed override void Assign(PropertiesBase source) {
			base.Assign(source);
			StylesBase srcStyles = source as StylesBase;
			if(srcStyles != null) {
				Reset();
				CopyFrom(srcStyles);
			}
		}
		public virtual void CopyFrom(StylesBase source) {
			if (!string.IsNullOrEmpty(source.Theme))
				Theme = source.Theme;
			if(!string.IsNullOrEmpty(source.CssFilePath))
				CssFilePath = source.CssFilePath;
			if(!string.IsNullOrEmpty(source.CssPostfix))
				CssPostfix = source.CssPostfix;
			if(source.Native)
				Native = source.Native;
			AccessibilityCompliant = source.AccessibilityCompliant;
			RightToLeft = source.RightToLeft;
			if(source.link != null)
				LinkInternal.CopyFrom(source.LinkInternal);
			foreach(KeyValuePair<string, AppearanceStyleBase> style in source.Styles) {
				AppearanceStyleBase newStyle = GetStyle(style.Key);
				if(newStyle != null)
					newStyle.CopyFrom(style.Value);
			}
		}
		public virtual void Reset() {
			CssFilePath = string.Empty;
			CssPostfix = string.Empty;
			link = null;
			Styles.Clear();
		}
		#endregion
		public override string ToString() {
			return string.Empty;
		}
		protected void TrackStylesViewState() {
			foreach(StyleInfo info in InfoList) {
				AppearanceStyleBase style = GetStyle(info.Name);
				if(style is IStateManager && style is DevExpress.Utils.Serializing.Helpers.IXtraSerializable2)
					((IStateManager)style).TrackViewState();
			}
		}
		public virtual string GetCssPostFix() {
			if(SkinOwner == null) return CssPostfix;
			return SkinOwner.GetCssPostFix();
		}
		public virtual string GetCssFilePath() {
			if(SkinOwner == null) return CssFilePath;
			string filePath = SkinOwner.GetCssFilePath();
			return string.Format(filePath, SkinOwner.GetControlName());
		}
		public virtual bool IsDefaultAppearanceEnabled() {
			return (SkinOwner != null) ? SkinOwner.IsDefaultAppearanceEnabled() : true;
		}
		public virtual bool IsNative() {
			if(SkinOwner == null) return Native;
			return SkinOwner.IsNative() && SkinOwner.IsNativeSupported();
		}
		public T CreateStyleCopyByName<T>(string styleName) where T : AppearanceStyleBase, new() {
			T style = new T();
			SetStyleCssClass(style, GetCssClassNamePrefix(), styleName);
			return style;
		}
		public AppearanceStyleBase CreateStyleByName(string styleName) {
			return CreateStyleByName(GetCssClassNamePrefix(), styleName);
		}
		public virtual AppearanceStyleBase CreateStyleByName(string styleNamePrefix, string styleName) {
			return CreateCachedStyle<AppearanceStyleBase>(styleNamePrefix, styleName, null);
		}
		protected T CreateCachedStyle<T>(string styleName, Action<T> prepareStyle) where T : AppearanceStyleBase, new() {
			return CreateCachedStyle<T>(GetCssClassNamePrefix(), styleName, prepareStyle);
		}
		protected T CreateCachedStyle<T>(string styleNamePrefix, string styleName, Action<T> prepareStyle) where T : AppearanceStyleBase, new() {
			if(StylesCache.ContainsKey(styleName))
				return (T)StylesCache[styleName];
			T style = new T();
			SetStyleCssClass(style, styleNamePrefix, styleName);
			if(prepareStyle != null)
				prepareStyle(style);
			StylesCache.Add(styleName, style);
			return style;
		}
		protected void SetStyleCssClass(AppearanceStyleBase style, string styleNamePrefix, string styleName) {
			string cssClass = GetCssClassName(styleNamePrefix, styleName);
			if(!string.IsNullOrEmpty(cssClass))
				style.CssClass = cssClass;
		}
		protected void AppendStyleCssClass(AppearanceStyleBase style, string styleNamePrefix, string styleName) {
			string cssClass = GetCssClassName(styleNamePrefix, styleName);
			if(!string.IsNullOrEmpty(cssClass))
				style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, cssClass);
		}
		protected virtual string GetCssClassName(string styleNamePrefix, string styleName) {
			if (IsNative())
				return string.Empty;
			if(IsEmptyCssClassName(styleName) || (!IsDefaultAppearanceEnabled() && string.IsNullOrEmpty(GetCssPostFix()))) 
				return string.Empty;
			return StylesBase.CreateCssClassName(styleNamePrefix, styleName.Replace("Style", string.Empty), GetCssPostFix());
		}
		protected internal virtual string GetCssClassNamePrefix() {
			return "dx";
		}
		protected virtual bool IsEmptyCssClassName(string cssName) {
			return false;
		}
		private void CreateStyles() {
			if(!typedInfoList.TryGetValue(GetType(), out infoList)) {
				this.infoList = new List<StyleInfo>();
				PopulateStyleInfoList(InfoList);
				CreateIndex();
				lock(typedInfoList) {
					typedInfoIndex[GetType()] = InfoIndex;
					typedInfoList[GetType()] = InfoList;
				}
			}
			else
				infoIndex = typedInfoIndex[GetType()];
		}
		protected void CreateIndex() {
			this.infoIndex = new Dictionary<string, StyleInfo>(InfoList.Count);
			foreach(StyleInfo info in InfoList)
				InfoIndex[info.Name] = info;
		}
		protected virtual void PopulateStyleInfoList(List<StyleInfo> list) {
			list.Add(new StyleInfo(DisabledStyleName, delegate() { return new DisabledStyle(); } ));
			list.Add(new StyleInfo(LoadingPanelStyleName, delegate() { return new LoadingPanelStyle(); } ));
			list.Add(new StyleInfo(LoadingDivStyleName, delegate() { return new LoadingDivStyle(); } ));
		}
		protected AppearanceStyleBase GetStyle(string styleName) {
			return GetStyle(styleName, true);
		}
		protected AppearanceStyleBase GetStyle(string styleName, bool create) {
			if(string.IsNullOrEmpty(styleName))
				return null;
			AppearanceStyleBase style = null;
			if(!Styles.TryGetValue(styleName, out style) && create) {
				StyleInfo info;
				if(infoIndex.TryGetValue(styleName, out info)) {
					style = info.CreateStyle();
					TrackViewState(style);
					Styles.Add(styleName, style);
				}
			}
			return style;
		}
		static Color DefaultDisabledColor = Color.FromArgb(0x80, 0x80, 0x80);
		protected internal virtual AppearanceStyleBase GetDefaultDisabledStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("DisabledStyle"));
			if(!IsDefaultAppearanceEnabled() || IsNative())
				style.ForeColor = DefaultDisabledColor;
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultControlStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ControlStyle"));
			return style;
		}
		#region LoadingElements style processing
		protected internal virtual LoadingPanelStyle GetDefaultLoadingPanelStyle(bool appendLoadingPanelCssClass = false) {
			LoadingPanelStyle style = 
				GetDefaultLoadingElementStyle<LoadingPanelStyle>(appendLoadingPanelCssClass, GetLoadingPanelCssClassName());
			style.ImageSpacing = GetLoadingPanelImageSpacing();
			if(HideLoadingPanelBorder())
				AppendLoadingPanelWithoutBordersClassName(style);
			return style;
		}
		private void AppendLoadingPanelWithoutBordersClassName(LoadingPanelStyle style) {
			string cssClass = string.Format("{0}-withoutBorders", LoadingPanelStyles.DefaultLoadingPanelClassNamePrefix);
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, cssClass);
		}
		protected internal virtual LoadingDivStyle GetDefaultLoadingDivStyle(bool appendLoadingPanelCssClass = false) {
			return GetDefaultLoadingElementStyle<LoadingDivStyle>(appendLoadingPanelCssClass, GetLoadingDivCssClassName());
		}
		private T GetDefaultLoadingElementStyle<T>(bool appendLoadingPanelCssClass, string styleName) 
			where T : AppearanceStyleBase, new() {
			T style = new T();
			style.CopyFrom(CreateStyleByName(styleName));
			if(appendLoadingPanelCssClass)
				AppendLoadingPanelCssClass(style, styleName);
			return style;
		}
		protected virtual bool HideLoadingPanelBorder() {
			return false;
		}
		protected internal void AppendLoadingPanelCssClass(AppearanceStyleBase style, string styleName) {
			AppendStyleCssClass(style, LoadingPanelStyles.DefaultLoadingPanelClassNamePrefix, styleName);
		}
		protected virtual string GetLoadingDivCssClassName() {
			return "LoadingDiv";
		}
		protected virtual string GetLoadingPanelCssClassName() {
			return "LoadingPanel";
		}
		#endregion
		public virtual Unit GetImageSpacing() {
			return 4;
		}
		public virtual Unit GetLoadingPanelImageSpacing() {
			return Unit.Empty;
		}
		public virtual Unit GetBulletIndent() {
			if(Browser.IsOpera)
				return new Unit(Browser.MajorVersion == 8 ? 17 : -24, UnitType.Pixel);
			else if(Browser.Family.IsNetscape || Browser.Family.IsWebKit)
				return new Unit(-22, UnitType.Pixel);
			else if(Browser.IsIE)
				return new Unit(17, UnitType.Pixel);
			else
				return new Unit(21, UnitType.Pixel);
		}
		static Dictionary<Type, GetStateManagerObject[]> stateDelegates = new Dictionary<Type, GetStateManagerObject[]>();
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			GetStateManagerObject[] state;
			if(!stateDelegates.TryGetValue(GetType(), out state)) {
				List<GetStateManagerObject> res = new List<GetStateManagerObject>();
				foreach(StyleInfo info in InfoList) {
					string name = info.Name.ToString(); 
					res.Add(delegate(object styles, bool create) { return ((StylesBase)styles).GetStyle(name, create); });
				}
				res.Add(delegate(object styles, bool create) { return ((StylesBase)styles).GetObject(ref ((StylesBase)styles).link, create); });
				state = res.ToArray();
				lock(stateDelegates)
					stateDelegates[GetType()] = state;
			}
			return state;
		}
	}
}
namespace DevExpress.Web.Internal {
	public class StyleInfo {
		string name;
		Type styleType;
		CreateStyleHandler create;
		public StyleInfo(string name, Type styleType) {
			this.name = name;
			if(styleType != null)
				this.styleType = styleType;
		}
		public StyleInfo(string name, CreateStyleHandler createStyle) {
			this.name = name;
			create = createStyle;
		}
		public StyleInfo(string name)
			: this(name, typeof(AppearanceStyleBase)) {
		}
		public AppearanceStyleBase CreateStyle() {
			if(create == null)
				return styleType == typeof(AppearanceStyleBase) ? new AppearanceStyleBase() : (AppearanceStyleBase)Activator.CreateInstance(styleType);
			else
				return create();
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
	}
	public static class FontHelper {
		public static bool IsBoldStored(FontInfo font) {
			return IsFlagStored(font, "Font_Bold");
		}
		public static bool IsItalicStored(FontInfo font) {
			return IsFlagStored(font, "Font_Italic");
		}
		public static bool IsOverlineStored(FontInfo font) {
			return IsFlagStored(font, "Font_Overline");
		}
		public static bool IsStrikeoutStored(FontInfo font) {
			return IsFlagStored(font, "Font_Strikeout");
		}
		public static bool IsUnderlineStored(FontInfo font) {
			return IsFlagStored(font, "Font_Underline");
		}
		private static bool IsFlagStored(FontInfo font, string flagName) {
			StateBag viewState = new StateBag();
			Style style = new Style(viewState);
			style.Font.CopyFrom(font);
			return viewState[flagName] != null;
		}
	}
}
