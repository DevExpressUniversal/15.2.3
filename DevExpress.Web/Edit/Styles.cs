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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web {
	public class ReadOnlyStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { base.Wrap = value; }
		}
	}
	public class ErrorFrameStyle : AppearanceStyle {
		Paddings errorTextPaddings;
		[
#if !SL
	DevExpressWebLocalizedDescription("ErrorFrameStyleErrorTextPaddings"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings ErrorTextPaddings {
			get {
				return CreateObject(ref errorTextPaddings);
			}
		}
		[Browsable(false)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ErrorFrameStyleIsEmpty")]
#endif
		public override bool IsEmpty {
			get { return base.IsEmpty && ErrorTextPaddings.IsEmpty; }
		}
		public override void CopyFrom(Style style) {
			if(style != null && !style.IsEmpty) {
				base.CopyFrom(style);
				ErrorFrameStyle fstyle = style as ErrorFrameStyle;
				if(fstyle != null && fstyle.errorTextPaddings != null)
					ErrorTextPaddings.CopyFrom(fstyle.errorTextPaddings);
			}
		}
		public override void MergeWith(Style style) {
			if(style != null && !style.IsEmpty) {
				base.MergeWith(style);
				ErrorFrameStyle fstyle = style as ErrorFrameStyle;
				if(fstyle != null && fstyle.errorTextPaddings != null)
					ErrorTextPaddings.MergeWith(fstyle.errorTextPaddings);
			}
		}
		public override void Reset() {
			if(errorTextPaddings != null)
				errorTextPaddings.Reset();
			base.Reset();
		}
		protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver) {
			base.FillStyleAttributes(attributes, urlResolver);
			if(errorTextPaddings != null && !errorTextPaddings.IsEmpty)
				ErrorTextPaddings.FillStyleAttributes(attributes, urlResolver);
		}
		static GetStateManagerObject[] delegates;
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			if(delegates == null) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>();
				list.AddRange(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((ErrorFrameStyle)style).GetObject(ref ((ErrorFrameStyle)style).errorTextPaddings, create); ; });
#pragma warning restore 197
				delegates = list.ToArray();
			}
			return delegates;
		}
	}
	public class EditButtonStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), AutoFormatEnable()]
		public new Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonStyleDisabledStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonStylePressedStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle PressedStyle {
			get { return base.PressedStyle; }
		}
		[Browsable(false)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class SpinButtonStyle : EditButtonStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Paddings Paddings {
			get { return base.Paddings; }
		}
	}
	public class EditStyleBase : AppearanceStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
	}
	public class EditAreaStyle : AppearanceStyle {
		[Browsable(false)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false)]
		public override Unit Width {
			get { return Unit.Empty; }
		}
	}
	public class ListBoxItemStyle : AppearanceItemStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxItemStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), AutoFormatEnable]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
	}
	public class CalendarElementStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { base.Wrap = value; }
		}
	}
	public class CalendarFastNavStyle : CalendarElementStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarFastNavStyleImageSpacing"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarFastNavStyleMonthYearSpacing"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit MonthYearSpacing {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "MonthYearSpacing", Unit.Empty); }
			set { ViewStateUtils.SetUnitProperty(ViewState, "MonthYearSpacing", Unit.Empty, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("CalendarFastNavStyleIsEmpty")]
#endif
		public override bool IsEmpty {
			get { return base.IsEmpty && MonthYearSpacing.IsEmpty; }
		}
		public override void CopyFrom(Style style) {
			if(style != null && !style.IsEmpty) {
				base.CopyFrom(style);
				CalendarFastNavStyle fastNavStyle = style as CalendarFastNavStyle;
				if(fastNavStyle != null) 
					MonthYearSpacing = fastNavStyle.MonthYearSpacing;
			}
		}
		public override void Reset() {
			base.Reset();
			if(IsModified)
				MonthYearSpacing = Unit.Empty;
		}
		public override void MergeWith(Style style) {
			if(style != null && !style.IsEmpty) {
				base.MergeWith(style);
				CalendarFastNavStyle fastNavStyle = style as CalendarFastNavStyle;
				if(fastNavStyle != null) {
					if( !fastNavStyle.IsEmpty == MonthYearSpacing.IsEmpty )
						MonthYearSpacing = fastNavStyle.MonthYearSpacing;
				}
			}
		}
	}
	public class BinaryImageButtonPanelStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageButtonPanelStyleMargins"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new Margins Margins {
			get { return base.Margins; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			var src = style as BinaryImageButtonPanelStyle;
			if(src != null) {
				HoverStyle.CopyFrom(src.HoverStyle);
				ImageSpacing = src.ImageSpacing;
				Margins.CopyFrom(src.Margins);
				Spacing = src.Spacing;
			}
		}
	}
	public class CalendarHeaderFooterStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
	}
	public class CalendarFastNavItemStyle : AppearanceItemStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { base.Wrap = value; }
		}
	}
	public class DateEditTimeSectionCellStyle : AppearanceItemStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap { get { return base.Wrap; } set { base.Wrap = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle SelectedStyle { get { return base.SelectedStyle; } }
	}
	public class MaskHintStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign { get { return HorizontalAlign.NotSet; } set {  } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return Unit.Empty; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing { get { return Unit.Empty; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign { get { return VerticalAlign.NotSet; } set { } }
	}
	public class ProgressBarStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), AutoFormatEnable()]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), AutoFormatEnable()]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
	}
	public class DropDownWindowStyle : AppearanceStyleBase{
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownWindowStylePaddings"),
#endif
		AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Paddings Paddings {
			get { return base.Paddings; }
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ItemPickerTableStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign { get { return HorizontalAlign.NotSet; } set { } }
		[
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Paddings Paddings { get { return base.Paddings; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign { get { return VerticalAlign.NotSet; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap { get { return base.Wrap; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ItemPickerTableCellStyle : AppearanceItemStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color BackColor { get { return base.BackColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage { get { return base.BackgroundImage; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign { get { return HorizontalAlign.NotSet; } set { } }
		[
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Paddings Paddings { get { return base.Paddings; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign { get { return VerticalAlign.NotSet; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap { get { return base.Wrap; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { 
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
	}
	public class TrackBarTrackElementStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
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
	}
	public class ToolTipStyleBase : AppearanceStyleBase {
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
		protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver) {
			base.FillStyleAttributes(attributes, urlResolver);
			if(Wrap != DefaultBoolean.Default)
				attributes[HtmlTextWriterStyle.WhiteSpace] = RenderUtils.GetWrapStyleValue(Wrap);
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
	public class HelpTextStyle : ToolTipStyleBase {
	}
	public class TrackBarValueToolTipStyle : ToolTipStyleBase { }
	public class TrackBarButtonStyle : EditButtonStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage {
			get {
				return base.BackgroundImage;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get {
				return base.ImageSpacing;
			}
			set {
				base.ImageSpacing = value;
			}
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
		}
	}
	public class TrackBarTickStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color BackColor { get { return base.BackColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage {
			get { return base.BackgroundImage; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderWrapper Border {
			get { return base.Border; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom {
			get { return base.BorderBottom; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight {
			get { return base.BorderRight; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft {
			get { return base.BorderLeft; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop {
			get { return base.BorderTop; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height {
			get { return base.Height; }
			set { }
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
		public override Unit Width {
			get { return base.Width; }
			set { }
		}
	}
	[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
	public class ColorTableStyle : ItemPickerTableStyle {
	}
	[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
	public class ColorPickerStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign { get { return HorizontalAlign.NotSet; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign { get { return VerticalAlign.NotSet; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap { get { return base.Wrap; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
	}
	[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
	public class ColorTableCellStyle : ItemPickerTableCellStyle {
		protected internal AppearanceStyleBase ColorTableDivStyle {
			get { return PressedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorTableCellStyleColorBorder"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BorderWrapper ColorBorder { get { return ColorTableDivStyle.Border; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorTableCellStyleColorBorderLeft"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border ColorBorderLeft { get { return ColorTableDivStyle.BorderLeft; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorTableCellStyleColorBorderTop"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border ColorBorderTop { get { return ColorTableDivStyle.BorderTop; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorTableCellStyleColorBorderRight"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border ColorBorderRight { get { return ColorTableDivStyle.BorderRight; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorTableCellStyleColorBorderBottom"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border ColorBorderBottom { get { return ColorTableDivStyle.BorderBottom; } }
	}
	public class ColorIndicatorStyle : ColorTableStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage { get { return base.BackgroundImage; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color BackColor { get { return base.BackColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Paddings Paddings { get { return base.Paddings; } }
	}
	public class EditorDecorationStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]		
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
	}
	public class EditorStyles : StylesBase {
		public const string StyleStyleName = "Style";
		public const string ReadOnlyStyleName = "ReadOnly";
		public const string CheckEditStyleName = "CheckEdit";
		public const string ListBoxStyleName = "ListBox";
		public const string ListBoxItemStyleName = "ListBoxItem";
		public const string ListBoxItemRowStyleName = "ListBoxItemRow";
		public const string RadioButtonListStyleName = "RadioButtonList";
		public const string CheckBoxListStyleName = "CheckBoxList";
		public const string SpinEditIncrementButtonStyleName = "SpinEditIncrementButton";
		public const string SpinEditDecrementButtonStyleName = "SpinEditDecrementButton";
		public const string SpinEditLargeIncrementButtonStyleName = "SpinEditLargeIncrementButton";
		public const string SpinEditLargeDecrementButtonStyleName = "SpinEditLargeDecrementButton";
		public const string LabelStyleName = "Label";
		public const string HyperlinkStyleName = "Hyperlink";
		public const string ImageStyleName = "Image";
		public const string MemoStyleName = "Memo";
		public const string CaptionCellSafariSystemClassName = "CaptionCellSafariSys";
		public const string TextBoxStyleName = "TextBox";
		public const string HelpTextStyleName = "HelpText";
		public const string RootStyleName = "Root";
		public const string CaptionCellStyleName = "CaptionCell";
		public const string CaptionStyleName = "Caption";
		public const string RequiredMarkStyleName = "RequiredMark";
		public const string OptionalMarkStyleName = "OptionalMark";
		public const string CaptionHALeftSystemClassName = "CaptionHALSys";
		public const string CaptionHARightSystemClassName = "CaptionHARSys";
		public const string CaptionHACenterSystemClassName = "CaptionHACSys";
		public const string CaptionVABottomSystemClassName = "CaptionVABSys";
		public const string CaptionVAMiddleSystemClassName = "CaptionVAMSys";
		public const string CaptionVATopSystemClassName = "CaptionVATSys";
		public const string CaptionPositionTopSystemClassName = "CLTSys";
		public const string CaptionPositionBottomSystemClassName = "CLBSys";
		public const string CaptionPositionLeftSystemClassName = "CLLSys";
		public const string CaptionPositionRightSystemClassName = "CLRSys";
		public const string OutOfRangeWarningStyleName = "OutOfRWarn";
		public const string OutOfRangeWarningRightPosStyleName = "OutOfRWarnRight";
		public const string OutOfRangeWarningBottomPosStyleName = "OutOfRWarnBottom";
		public const string BinaryImageStyleName = "BinaryImage";
		public const string BinaryImageButtonPanelStyleName = "BinaryImageButtonPanel";
		public const string BinaryImageButtonStyleName = "BinaryImageButton";
		public const string BinaryImageDropZoneStyleName = "BinaryImageDropZone";
		public const string BinaryImageEmptyValueTextStyleName = "BinaryImageEmptyValueText";
		public const string ButtonEditStyleName = "ButtonEdit";
		public const string ButtonEditButtonStyleName = "ButtonEditButton";
		public const string ButtonEditClearButtonStyleName = "ButtonEditClearButton";
		public const string CalendarStyleName = "Calendar";
		public const string CalendarDayHeaderStyleName = "CalendarDayHeader";
		public const string CalendarWeekNumberStyleName = "CalendarWeekNumber";
		public const string CalendarDayStyleName = "CalendarDay";
		public const string CalendarDayOtherMonthStyleName = "CalendarDayOtherMonth";
		public const string CalendarDaySelectedStyleName = "CalendarDaySelected";
		public const string CalendarDayWeekendStyleName = "CalendarDayWeekend";
		public const string CalendarDayOutOfRangeStyleName = "CalendarDayOutOfRange";
		public const string CalendarDayDisabledStyleName = "CalendarDayDisabled";
		public const string CalendarTodayStyleName = "CalendarToday";
		public const string CalendarHeaderStyleName = "CalendarHeader";
		public const string CalendarFooterStyleName = "CalendarFooter";
		public const string CalendarButtonStyleName = "CalendarButton";
		public const string CalendarFastNavStyleName = "CalendarFastNav";
		public const string CalendarFastNavMonthAreaStyleName = "CalendarFastNavMonthArea";
		public const string CalendarFastNavYearAreaStyleName = "CalendarFastNavYearArea";
		public const string CalendarFastNavMonthStyleName = "CalendarFastNavMonth";
		public const string CalendarFastNavYearStyleName = "CalendarFastNavYear";
		public const string CalendarFastNavFooterStyleName = "CalendarFastNavFooter";
		public const string DateEditTimeEditCellStyleName = "DateEditTimeEditCell";
		public const string DateEditClockCellStyleName = "DateEditClockCell";
		public const string MaskHintStyleName = "MaskHint";
		public const string ProgressBarStyleName = "ProgressBar";
		public const string ProgressBarIndicatorStyleName = "ProgressBarIndicator";
		public const string DropDownWindowStyleName = "DropDownWindow";
		public const string ColorTableStyleName = "ColorTable";
		public const string ColorPickerStyleName = "ColorPicker";
		public const string ColorTableCellStyleName = "ColorTableCell";
		public const string ColorIndicatorStyleName = "ColorIndicator";
		public const string DisplayColorIndicatorStyleName = "DisplayColorIndicator";
		public const string FocusedStyleName = "Focused";
		public const string NullTextStyleName = "NullText";
		public const string InvalidStyleName = "Invalid";
		public const string IRBFocusedStyleName = "IRBFocused";
		public const string ICBFocusedStyleName = "ICBFocused";
		public const string IRadioButtonStyleName = "IRadioButton";
		public const string ICheckBoxStyleName = "ICheckBox";
		public const string TokenBoxTokenStyleName = "Token";
		public const string TokenBoxTokenHoverStyleName = "TokenHover";
		public const string TokenBoxTokenTextStyleName = "TokenText";
		public const string TokenBoxInputStyleName = "TokenBoxInput";
		public const string TokenBoxTokenRemoveButtonStyleName = "TokenRemoveButton";
		public const string TokenBoxTokenRemoveButtonHoverStyleName = "TokenRemoveButtonHover";
		public const string TBIncrementButtonStyleName = "TBIncBtn";
		public const string TBDecrementButtonStyleName = "TBDecBtn";
		public const string TBBarHighlightStyleName = "TBBarHighlight";
		public const string TBMainDragHandleStyleName = "TBMainDH";
		public const string TBSecondaryDragHandleStyleName = "TBSecondaryDH";
		public const string TBTrackStyleName = "TBTrack";
		public const string TBLargeTickStyleName = "TBLargeTick";
		public const string TBSmallTickStyleName = "TBSmallTick";
		public const string TBScaleStyleName = "TBScale";
		public const string TBLeftTopLabelStyleName = "TBLTLabel";
		public const string TBRightBottomLabelStyleName = "TBRBLabel";
		public const string TrackBarItemStyleName = "TBItem";
		public const string TrackBarValueToolTipStyleName = "TBValueToolTip";
		public const string TrackBarSelectedItemStyleName = "TBSelectedItem";
		public const string TrackBarSelectedTickStyleName = "TBSelectedTick";
		public const string TrackBarStyleName = "TrackBar";
		internal const string
			TextBoxSystemClassName = "dxeTextBoxSys",
			TextBoxDefaultWidthSystemClassName = "dxeTextBoxDefaultWidthSys",
			ButtonEditSystemClassName = "dxeButtonEditSys",
			ButtonEditButtonSystemClassName = "dxeButton",
			ButtonEditButtonLeftSystemClassName = "dxeButtonLeft",
			SpinEditHasLargeButtonsSystemClassName = "dxeHasLarge",
			MemoSystemClassName = "dxeMemoSys",
			CalendarHeaderSpacerClassName = "dxeCHS",
			CalendarFooterSpacerClassName = "dxeCFS",
			CalendarFastNavFooterSpacerClassName = "dxeCFNFS",
			EditAreaSystemClassName = "dxeEditAreaSys",
			HideDefaultIEClearButton = "dxeHideDefaultIEClearBtnSys",
			MemoEditAreaSystemClassName = "dxeMemoEditAreaSys",
			TBVerticalSystemClassName = "dxeTBVSys",
			TBHorizontalSystemClassName = "dxeTBHSys",
			TBLargeTickSystemClassName = "dxeTBLargeTickSys",
			TBSmallTickSystemClassName = "dxeTBSmallTickSys",
			CustomColorButtonSystemClassName = "dxeCustomColorButtonSys",
			AutomaticColorItemSystemClassName = "dxeAutomaticColorItemSys",
			ErrorFrameSystemClassName = "dxeErrorFrameSys",
			ErrorCellSystemClassName = "dxeErrorCellSys",
			BinaryImageSystemClassName = "dxeBinImgSys",
			BinaryImageControlPanelSystemClassName = "dxeBinImgCPnlSys",
			BinaryImageContentContainerSystemClassName = "dxeBinImgContentContainer",
			BinaryImageDisabledCoverSystemClassName = "dxeBinImgDisablCoverSys",
			BinaryImageButtonSystemClassName = "dxeBinImgBtnSys",
			BinaryImageButtonHoverSystemClassName = "dxeBinImgBtnHoverSys",
			BinaryImageButtonShaderSystemClassName = "dxeBinImgBtnShaderSys",
			BinaryImageTextPanelSystemClassName = "dxeBinImgTxtPnlSys",
			BinaryImageDropZoneSystemClassName = "dxeBinImgDropZoneSys",
			BinaryImageEmptySystemClassName = "dxeBinImgEmptySys",
			BinaryImagePlaceholderSystemClassName = "dxeBinImgPlaceHolderSys",
			BinaryImagePreviewContainerSystemClassName = "dxeBinImgPreviewContainerSys",
			BinaryImageProgressPanelSystemClassName = "dxeBinImgProgressPnlSys",
			BinaryImageProgressBarContainerSystemClassName = "dxeBinImgProgressBarContainerSys",
			BottomSystemClassName = "dxeBottomSys",
			FillParentSystemClassName = "dxeFillParentSys",
			TopSystemClassName = "dxeTopSys",
			ContentLeftSystemClassName = "dxeContentLeftSys",
			ContentRightSystemClassName = "dxeContentRightSys",
			ContentCenterSystemClassName = "dxeContentCenterSys",
			DisplayTableSystemClassName = "dxeTblSys",
			DisplayInlineTableSystemClassName = "dxeInlineTblSys",
			DisplayRowSystemClassName = "dxeRowSys",
			DisplayCellSystemClassName = "dxeCellSys";
		internal const int
			DefaultButtonEditCellSpacing = 1,
			DefaultSpinButtonsHorizontalSpacing = 1;
		private Paddings calendarMonthGridPaddings;
		public EditorStyles(ISkinOwner properties)
			: base(properties) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesNative"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatEnable]
		public new bool Native {
			get { return base.Native; }
			set { base.Native = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditStyleBase Style {
			get { return (EditStyleBase)GetStyle(StyleStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the ReadOnly property instead."),
		NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ReadOnlyStyle ReadOnlyStyle {
			get { return ReadOnly; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesReadOnly"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ReadOnlyStyle ReadOnly {
			get { return (ReadOnlyStyle)GetStyle(ReadOnlyStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTokenBoxTokenRemoveButtonStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenBoxTokenRemoveButtonStyle
		{
			get { return (ExtendedStyleBase)GetStyle(TokenBoxTokenRemoveButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTokenBoxTokenRemoveButtonHoverStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenBoxTokenRemoveButtonHoverStyle
		{
			get { return (ExtendedStyleBase)GetStyle(TokenBoxTokenRemoveButtonHoverStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTokenBoxTokenStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenBoxTokenStyle
		{
			get { return (ExtendedStyleBase)GetStyle(TokenBoxTokenStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTokenBoxTokenHoverStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenBoxTokenHoverStyle {
			get { return (ExtendedStyleBase)GetStyle(TokenBoxTokenHoverStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTokenBoxTokenTextStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenBoxTokenTextStyle
		{
			get { return (ExtendedStyleBase)GetStyle(TokenBoxTokenTextStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTokenBoxInputStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenBoxInputStyle {
			get { return (ExtendedStyleBase)GetStyle(TokenBoxInputStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarIncrementButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarButtonStyle TrackBarIncrementButton {
			get { return (TrackBarButtonStyle)GetStyle(TBIncrementButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarDecrementButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarButtonStyle TrackBarDecrementButton {
			get { return (TrackBarButtonStyle)GetStyle(TBDecrementButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarBarHighlight"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTrackElementStyle TrackBarBarHighlight {
			get { return (TrackBarTrackElementStyle)GetStyle(TBBarHighlightStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarMainDragHandle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarButtonStyle TrackBarMainDragHandle {
			get { return (TrackBarButtonStyle)GetStyle(TBMainDragHandleStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarSecondaryDragHandle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarButtonStyle TrackBarSecondaryDragHandle {
			get { return (TrackBarButtonStyle)GetStyle(TBSecondaryDragHandleStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarTrack"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTrackElementStyle TrackBarTrack {
			get { return (TrackBarTrackElementStyle)GetStyle(TBTrackStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarLargeTick"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTickStyle TrackBarLargeTick {
			get { return (TrackBarTickStyle)GetStyle(TBLargeTickStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarSmallTick"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTickStyle TrackBarSmallTick {
			get { return (TrackBarTickStyle)GetStyle(TBSmallTickStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarScale"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase TrackBarScale {
			get { return (AppearanceStyleBase)GetStyle(TBScaleStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarLeftTopLabel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase TrackBarLeftTopLabel {
			get { return (AppearanceStyleBase)GetStyle(TBLeftTopLabelStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarRightBottomLabel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase TrackBarRightBottomLabel {
			get { return (AppearanceStyleBase)GetStyle(TBRightBottomLabelStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBar"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase TrackBar {
			get { return GetStyle(TrackBarStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTickStyle TrackBarItem {
			get { return (TrackBarTickStyle)GetStyle(TrackBarItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarSelectedItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTickStyle TrackBarSelectedItem {
			get { return (TrackBarTickStyle)GetStyle(TrackBarSelectedItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarSelectedTick"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarTickStyle TrackBarSelectedTick {
			get { return (TrackBarTickStyle)GetStyle(TrackBarSelectedTickStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTrackBarValueToolTip"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TrackBarValueToolTipStyle TrackBarValueToolTip {
			get { return (TrackBarValueToolTipStyle)GetStyle(TrackBarValueToolTipStyleName); }
		}
		internal HelpTextStyle HelpText {
			get { return (HelpTextStyle)GetStyle(HelpTextStyleName); }
		}
		internal EditorCaptionCellStyle CaptionCell {
			get { return (EditorCaptionCellStyle)GetStyle(CaptionCellStyleName); }
		}
		internal EditorCaptionStyle Caption {
			get { return (EditorCaptionStyle)GetStyle(CaptionStyleName); }
		}
		internal EditorRootStyle Root {
			get { return (EditorRootStyle)GetStyle(RootStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCheckEdit"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase CheckEdit {
			get { return GetStyle(CheckEditStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesListBox"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase ListBox {
			get { return GetStyle(ListBoxStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesListBoxItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ListBoxItemStyle ListBoxItem {
			get { return (ListBoxItemStyle)GetStyle(ListBoxItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesRadioButtonList"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase RadioButtonList {
			get { return GetStyle(RadioButtonListStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCheckBoxList"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase CheckBoxList {
			get { return GetStyle(CheckBoxListStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the SpinEditIncrementButton property instead."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual SpinButtonStyle IncrementButtonStyle {
			get { return SpinEditIncrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesSpinEditIncrementButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual SpinButtonStyle SpinEditIncrementButton {
			get { return (SpinButtonStyle)GetStyle(SpinEditIncrementButtonStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the SpinEditDecrementButton property instead."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual SpinButtonStyle DecrementButtonStyle {
			get { return SpinEditDecrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesSpinEditDecrementButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual SpinButtonStyle SpinEditDecrementButton {
			get { return (SpinButtonStyle)GetStyle(SpinEditDecrementButtonStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the SpinEditLargeIncrementButton property instead."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual SpinButtonStyle LargeIncrementButtonStyle {
			get { return SpinEditLargeIncrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesSpinEditLargeIncrementButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual SpinButtonStyle SpinEditLargeIncrementButton {
			get { return (SpinButtonStyle)GetStyle(SpinEditLargeIncrementButtonStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the SpinEditLargeDecrementButton property instead."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual SpinButtonStyle LargeDecrementButtonStyle {
			get { return SpinEditLargeDecrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesSpinEditLargeDecrementButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual SpinButtonStyle SpinEditLargeDecrementButton {
			get { return (SpinButtonStyle)GetStyle(SpinEditLargeDecrementButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesLabel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase Label {
			get { return GetStyle(LabelStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesHyperlink"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase Hyperlink {
			get { return GetStyle(HyperlinkStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase Image {
			get { return GetStyle(ImageStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesMemo"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditStyleBase Memo {
			get { return (EditStyleBase)GetStyle(MemoStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesTextBox"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditStyleBase TextBox {
			get { return (EditStyleBase)GetStyle(TextBoxStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesButtonEdit"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditStyleBase ButtonEdit {
			get { return (EditStyleBase)GetStyle(ButtonEditStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesButtonEditButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditButtonStyle ButtonEditButton {
			get { return (EditButtonStyle)GetStyle(ButtonEditButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesButtonEditClearButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditButtonStyle ButtonEditClearButton {
			get { return (EditButtonStyle)GetStyle(ButtonEditClearButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesBinaryImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditStyleBase BinaryImage {
			get { return (EditStyleBase)GetStyle(BinaryImageStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesBinaryImageButtonPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BinaryImageButtonPanelStyle BinaryImageButtonPanel {
			get { return (BinaryImageButtonPanelStyle)GetStyle(BinaryImageButtonPanelStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesBinaryImageButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ButtonStyle BinaryImageButton {
			get { return (ButtonStyle)GetStyle(BinaryImageButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesBinaryImageDropZone"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyle BinaryImageDropZone {
			get { return (AppearanceStyle)GetStyle(BinaryImageDropZoneStyleName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyle BinaryImageEmptyValueText {
			get { return (AppearanceStyle)GetStyle(BinaryImageEmptyValueTextStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendar"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceStyleBase Calendar {
			get { return GetStyle(CalendarStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarDayHeader"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarElementStyle CalendarDayHeader {
			get { return (CalendarElementStyle)GetStyle(CalendarDayHeaderStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarWeekNumber"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarElementStyle CalendarWeekNumber {
			get { return (CalendarElementStyle)GetStyle(CalendarWeekNumberStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarDay"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarElementStyle CalendarDay {
			get { return (CalendarElementStyle)GetStyle(CalendarDayStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarDayOtherMonth"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarElementStyle CalendarDayOtherMonth {
			get { return (CalendarElementStyle)GetStyle(CalendarDayOtherMonthStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarDaySelected"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarElementStyle CalendarDaySelected {
			get { return (CalendarElementStyle)GetStyle(CalendarDaySelectedStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarDayWeekEnd"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarElementStyle CalendarDayWeekEnd {
			get { return (CalendarElementStyle)GetStyle(CalendarDayWeekendStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarDayOutOfRange"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarElementStyle CalendarDayOutOfRange {
			get { return (CalendarElementStyle)GetStyle(CalendarDayOutOfRangeStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarDayDisabled"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarElementStyle CalendarDayDisabled {
			get { return (CalendarElementStyle)GetStyle(CalendarDayDisabledStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarToday"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarElementStyle CalendarToday {
			get { return (CalendarElementStyle)GetStyle(CalendarTodayStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarHeader"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarHeaderFooterStyle CalendarHeader {
			get { return (CalendarHeaderFooterStyle)GetStyle(CalendarHeaderStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarFooter"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarHeaderFooterStyle CalendarFooter {
			get { return (CalendarHeaderFooterStyle)GetStyle(CalendarFooterStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditButtonStyle CalendarButton {
			get { return (EditButtonStyle)GetStyle(CalendarButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarMonthGridPaddings"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Paddings CalendarMonthGridPaddings {
			get { return CreateObject(ref calendarMonthGridPaddings); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarFastNav"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarFastNavStyle CalendarFastNav {
			get { return (CalendarFastNavStyle)GetStyle(CalendarFastNavStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarFastNavMonthArea"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarElementStyle CalendarFastNavMonthArea {
			get { return (CalendarElementStyle)GetStyle(CalendarFastNavMonthAreaStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarFastNavYearArea"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarElementStyle CalendarFastNavYearArea {
			get { return (CalendarElementStyle)GetStyle(CalendarFastNavYearAreaStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarFastNavMonth"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarFastNavItemStyle CalendarFastNavMonth {
			get { return (CalendarFastNavItemStyle)GetStyle(CalendarFastNavMonthStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarFastNavYear"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarFastNavItemStyle CalendarFastNavYear {
			get { return (CalendarFastNavItemStyle)GetStyle(CalendarFastNavYearStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCalendarFastNavFooter"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CalendarHeaderFooterStyle CalendarFastNavFooter {
			get { return (CalendarHeaderFooterStyle)GetStyle(CalendarFastNavFooterStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesDateEditTimeEditCell"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DateEditTimeSectionCellStyle DateEditTimeEditCell
		{
			get { return (DateEditTimeSectionCellStyle)GetStyle(DateEditTimeEditCellStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesDateEditClockCell"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DateEditTimeSectionCellStyle DateEditClockCell
		{
			get { return (DateEditTimeSectionCellStyle)GetStyle(DateEditClockCellStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesMaskHint"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual MaskHintStyle MaskHint {
			get { return (MaskHintStyle)GetStyle(MaskHintStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesProgressBar"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ProgressBarStyle ProgressBar {
			get { return (ProgressBarStyle)GetStyle(ProgressBarStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesProgressBarIndicator"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ProgressBarIndicatorStyle ProgressBarIndicator {
			get { return (ProgressBarIndicatorStyle)GetStyle(ProgressBarIndicatorStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesDropDownWindow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DropDownWindowStyle DropDownWindow {
			get { return (DropDownWindowStyle)GetStyle(DropDownWindowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesColorTable"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ColorTableStyle ColorTable {
			get { return (ColorTableStyle)GetStyle(ColorTableStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesColorPicker"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ColorPickerStyle ColorPicker {
			get { return (ColorPickerStyle)GetStyle(ColorPickerStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesColorTableCell"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ColorTableCellStyle ColorTableCell {
			get { return (ColorTableCellStyle)GetStyle(ColorTableCellStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesColorIndicator"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ColorIndicatorStyle ColorIndicator {
			get { return (ColorIndicatorStyle)GetStyle(ColorIndicatorStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesDisplayColorIndicator"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ColorIndicatorStyle DisplayColorIndicator {
			get { return (ColorIndicatorStyle)GetStyle(DisplayColorIndicatorStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesFocused"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle Focused {
			get { return (EditorDecorationStyle)GetStyle(FocusedStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCheckBoxFocused"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle CheckBoxFocused {
			get { return (EditorDecorationStyle)GetStyle(ICBFocusedStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesCheckBox"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle CheckBox { get { return (EditorDecorationStyle)GetStyle(ICheckBoxStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesRadioButtonFocused"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle RadioButtonFocused { get { return (EditorDecorationStyle)GetStyle(IRBFocusedStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesRadioButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle RadioButton { get { return (EditorDecorationStyle)GetStyle(IRadioButtonStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesNullText"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle NullText {
			get { return (EditorDecorationStyle)GetStyle(NullTextStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesInvalid"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle Invalid {
			get { return (EditorDecorationStyle)GetStyle(InvalidStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesEnableFocusedStyle"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnableFocusedStyle {
			get { return GetBoolProperty("EnableFocusedStyle", true); }
			set { SetBoolProperty("EnableFocusedStyle", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesLoadingPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual LoadingPanelStyle LoadingPanel {
			get { return base.LoadingPanelInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesLoadingDiv"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual LoadingDivStyle LoadingDiv {
			get { return base.LoadingDivInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesButtonEditCellSpacing"),
#endif
		DefaultValue(DefaultButtonEditCellSpacing), NotifyParentProperty(true), AutoFormatEnable]
		public int ButtonEditCellSpacing {
			get { return GetIntProperty("ButtonEditCellSpacing", DefaultButtonEditCellSpacing); }
			set { SetIntProperty("ButtonEditCellSpacing", DefaultButtonEditCellSpacing, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorStylesSpinButtonsHorizontalSpacing"),
#endif
		Obsolete("This property is now obsolete. Use the ButtonEditCellSpacing property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(DefaultSpinButtonsHorizontalSpacing), NotifyParentProperty(true), AutoFormatEnable]
		public int SpinButtonsHorizontalSpacing {
			get { return GetIntProperty("SpinButtonsHorizontalSpacing", DefaultSpinButtonsHorizontalSpacing); }
			set { SetIntProperty("SpinButtonsHorizontalSpacing", DefaultSpinButtonsHorizontalSpacing, value); }
		}
		protected internal bool NativeInternal {
			get { return Native; }
			set { Native = value; }
		}
		protected internal bool AccessibilityCompliantInternal {
			get { return AccessibilityCompliant; }
			set { AccessibilityCompliant = value; }
		}
		protected internal DefaultBoolean RightToLeftInternal {
			get { return RightToLeft; }
			set { RightToLeft = value; }
		}
		#region CopyFrom, Reset
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			EditorStyles src = source as EditorStyles;
			if(src != null) {
				if(src.calendarMonthGridPaddings != null)
					CalendarMonthGridPaddings.CopyFrom(src.CalendarMonthGridPaddings);
				if(!src.EnableFocusedStyle)
					EnableFocusedStyle = src.EnableFocusedStyle;
				if(src.ButtonEditCellSpacing != DefaultButtonEditCellSpacing)
					ButtonEditCellSpacing = src.ButtonEditCellSpacing;
			}
		}
		public override void Reset() {
			base.Reset();
			calendarMonthGridPaddings = null;
		}
		#endregion
		protected internal override string GetCssClassNamePrefix() {
			return "dxe";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(StyleStyleName, delegate() { return new EditStyleBase(); } ));
			list.Add(new StyleInfo(ReadOnlyStyleName, delegate() { return new ReadOnlyStyle(); } ));
			list.Add(new StyleInfo(CheckEditStyleName, delegate() { return new AppearanceStyleBase(); } ));
			list.Add(new StyleInfo(ListBoxStyleName, delegate() { return new AppearanceStyleBase(); } ));
			list.Add(new StyleInfo(ListBoxItemStyleName, delegate() { return new ListBoxItemStyle(); } ));
			list.Add(new StyleInfo(RadioButtonListStyleName, delegate() { return new AppearanceStyleBase(); } ));
			list.Add(new StyleInfo(CheckBoxListStyleName, delegate() { return new AppearanceStyleBase(); } ));
			list.Add(new StyleInfo(SpinEditIncrementButtonStyleName, delegate() { return new SpinButtonStyle(); } ));
			list.Add(new StyleInfo(SpinEditDecrementButtonStyleName, delegate() { return new SpinButtonStyle(); } ));
			list.Add(new StyleInfo(SpinEditLargeIncrementButtonStyleName, delegate() { return new SpinButtonStyle(); } ));
			list.Add(new StyleInfo(SpinEditLargeDecrementButtonStyleName, delegate() { return new SpinButtonStyle(); } ));
			list.Add(new StyleInfo(LabelStyleName, delegate() { return new AppearanceStyleBase(); } ));
			list.Add(new StyleInfo(HyperlinkStyleName, delegate() { return new AppearanceStyleBase(); } ));
			list.Add(new StyleInfo(ImageStyleName, delegate() { return new AppearanceStyleBase(); } ));
			list.Add(new StyleInfo(MemoStyleName, delegate() { return new EditStyleBase(); } ));
			list.Add(new StyleInfo(TextBoxStyleName, delegate() { return new EditStyleBase(); } ));
			list.Add(new StyleInfo(ButtonEditStyleName, delegate() { return new EditStyleBase(); } ));
			list.Add(new StyleInfo(ButtonEditButtonStyleName, delegate() { return new EditButtonStyle(); } ));
			list.Add(new StyleInfo(ButtonEditClearButtonStyleName, delegate() { return new EditButtonStyle(); } ));
			list.Add(new StyleInfo(BinaryImageStyleName, () => new EditStyleBase()));
			list.Add(new StyleInfo(BinaryImageButtonPanelStyleName, () => new BinaryImageButtonPanelStyle()));
			list.Add(new StyleInfo(BinaryImageButtonStyleName, () => new ButtonStyle()));
			list.Add(new StyleInfo(BinaryImageDropZoneStyleName, () => new AppearanceStyle()));
			list.Add(new StyleInfo(BinaryImageEmptyValueTextStyleName, () => new AppearanceStyle()));
			list.Add(new StyleInfo(CalendarStyleName, delegate() { return new AppearanceStyleBase(); } ));
			list.Add(new StyleInfo(CalendarDayHeaderStyleName, delegate() { return new CalendarElementStyle(); } ));
			list.Add(new StyleInfo(CalendarWeekNumberStyleName, delegate() { return new CalendarElementStyle(); } ));
			list.Add(new StyleInfo(CalendarDayStyleName, delegate() { return new CalendarElementStyle(); } ));
			list.Add(new StyleInfo(CalendarDayOtherMonthStyleName, delegate() { return new CalendarElementStyle(); } ));
			list.Add(new StyleInfo(CalendarDaySelectedStyleName, delegate() { return new CalendarElementStyle(); } ));
			list.Add(new StyleInfo(CalendarDayWeekendStyleName, delegate() { return new CalendarElementStyle(); } ));
			list.Add(new StyleInfo(CalendarDayOutOfRangeStyleName, delegate() { return new CalendarElementStyle(); } ));
			list.Add(new StyleInfo(CalendarDayDisabledStyleName, delegate() { return new CalendarElementStyle(); }));
			list.Add(new StyleInfo(CalendarTodayStyleName, delegate() { return new CalendarElementStyle(); } ));
			list.Add(new StyleInfo(CalendarHeaderStyleName, delegate() { return new CalendarHeaderFooterStyle(); } ));
			list.Add(new StyleInfo(CalendarFooterStyleName, delegate() { return new CalendarHeaderFooterStyle(); } ));
			list.Add(new StyleInfo(CalendarButtonStyleName, delegate() { return new EditButtonStyle(); } ));
			list.Add(new StyleInfo(CalendarFastNavStyleName, delegate() { return new CalendarFastNavStyle(); } ));
			list.Add(new StyleInfo(CalendarFastNavMonthAreaStyleName, delegate() { return new CalendarElementStyle(); } ));
			list.Add(new StyleInfo(CalendarFastNavYearAreaStyleName, delegate() { return new CalendarElementStyle(); } ));
			list.Add(new StyleInfo(CalendarFastNavMonthStyleName, delegate() { return new CalendarFastNavItemStyle(); } ));
			list.Add(new StyleInfo(CalendarFastNavYearStyleName, delegate() { return new CalendarFastNavItemStyle(); } ));
			list.Add(new StyleInfo(CalendarFastNavFooterStyleName, delegate() { return new CalendarHeaderFooterStyle(); } ));
			list.Add(new StyleInfo(DateEditTimeEditCellStyleName, delegate() { return new DateEditTimeSectionCellStyle(); }));
			list.Add(new StyleInfo(DateEditClockCellStyleName, delegate() { return new DateEditTimeSectionCellStyle(); }));
			list.Add(new StyleInfo(MaskHintStyleName, delegate() { return new MaskHintStyle(); }));
			list.Add(new StyleInfo(ProgressBarStyleName, delegate() { return new ProgressBarStyle(); }));
			list.Add(new StyleInfo(ProgressBarIndicatorStyleName, delegate() { return new ProgressBarIndicatorStyle(); }));
			list.Add(new StyleInfo(DropDownWindowStyleName, delegate() { return new DropDownWindowStyle(); }));
			list.Add(new StyleInfo(ColorTableStyleName, delegate() {return new ColorTableStyle(); }));
			list.Add(new StyleInfo(ColorPickerStyleName, delegate() {return new ColorPickerStyle(); }));
			list.Add(new StyleInfo(ColorTableCellStyleName, delegate() {return new ColorTableCellStyle(); }));
			list.Add(new StyleInfo(ColorIndicatorStyleName, delegate() { return new ColorIndicatorStyle(); }));
			list.Add(new StyleInfo(DisplayColorIndicatorStyleName, delegate() { return new ColorIndicatorStyle(); }));
			list.Add(new StyleInfo(FocusedStyleName, delegate() { return new EditorDecorationStyle(); }));
			list.Add(new StyleInfo(NullTextStyleName, delegate() { return new EditorDecorationStyle(); }));
			list.Add(new StyleInfo(InvalidStyleName, delegate() { return new EditorDecorationStyle(); }));
			list.Add(new StyleInfo(ICBFocusedStyleName, delegate() { return new EditorDecorationStyle(); }));
			list.Add(new StyleInfo(IRBFocusedStyleName, delegate() { return new EditorDecorationStyle(); }));
			list.Add(new StyleInfo(ICheckBoxStyleName, delegate() { return new EditorDecorationStyle(); }));
			list.Add(new StyleInfo(IRadioButtonStyleName, delegate() { return new EditorDecorationStyle(); }));
			list.Add(new StyleInfo(TokenBoxTokenStyleName, delegate() { return new ExtendedStyleBase(); }));
			list.Add(new StyleInfo(TokenBoxTokenHoverStyleName, delegate() { return new ExtendedStyleBase(); }));
			list.Add(new StyleInfo(TokenBoxTokenTextStyleName, delegate() { return new ExtendedStyleBase(); }));
			list.Add(new StyleInfo(TokenBoxInputStyleName, delegate() { return new ExtendedStyleBase(); }));
			list.Add(new StyleInfo(TokenBoxTokenRemoveButtonStyleName, delegate() { return new ExtendedStyleBase(); }));
			list.Add(new StyleInfo(TokenBoxTokenRemoveButtonHoverStyleName, delegate() { return new ExtendedStyleBase(); }));
			list.Add(new StyleInfo(TBIncrementButtonStyleName, delegate() { return new TrackBarButtonStyle(); }));
			list.Add(new StyleInfo(TBDecrementButtonStyleName, delegate() { return new TrackBarButtonStyle(); }));
			list.Add(new StyleInfo(TBMainDragHandleStyleName, delegate() { return new TrackBarButtonStyle(); }));
			list.Add(new StyleInfo(TBSecondaryDragHandleStyleName, delegate() { return new TrackBarButtonStyle(); }));
			list.Add(new StyleInfo(TBBarHighlightStyleName, delegate() { return new TrackBarTrackElementStyle(); }));
			list.Add(new StyleInfo(TBTrackStyleName, delegate() { return new TrackBarTrackElementStyle(); }));
			list.Add(new StyleInfo(TBLargeTickStyleName, delegate() { return new TrackBarTickStyle(); }));
			list.Add(new StyleInfo(TBSmallTickStyleName, delegate() { return new TrackBarTickStyle(); }));
			list.Add(new StyleInfo(TBLeftTopLabelStyleName, delegate() { return new AppearanceStyleBase(); }));
			list.Add(new StyleInfo(TBRightBottomLabelStyleName, delegate() { return new AppearanceStyleBase(); }));
			list.Add(new StyleInfo(TBScaleStyleName, delegate() { return new AppearanceStyleBase(); }));
			list.Add(new StyleInfo(TrackBarItemStyleName, delegate() { return new TrackBarTickStyle(); }));
			list.Add(new StyleInfo(TrackBarSelectedItemStyleName, delegate() { return new TrackBarTickStyle(); }));
			list.Add(new StyleInfo(TrackBarSelectedTickStyleName, delegate() { return new TrackBarTickStyle(); }));
			list.Add(new StyleInfo(TrackBarValueToolTipStyleName, delegate() { return new TrackBarValueToolTipStyle(); }));
			list.Add(new StyleInfo(TrackBarStyleName, delegate() { return new AppearanceStyleBase(); }));
			list.Add(new StyleInfo(HelpTextStyleName, delegate() { return new HelpTextStyle(); }));
			list.Add(new StyleInfo(CaptionCellStyleName, delegate() { return new EditorCaptionCellStyle(); }));
			list.Add(new StyleInfo(CaptionStyleName, delegate() { return new EditorCaptionStyle(); }));
			list.Add(new StyleInfo(RootStyleName, delegate() { return new EditorRootStyle(); }));
		}
		protected internal ReadOnlyStyle GetDefaultReadOnlyStyle() {
			ReadOnlyStyle style = new ReadOnlyStyle();
			style.CopyFrom(CreateStyleByName("ReadOnlyStyle"));
			return style;
		}
		protected internal LoadingPanelStyle GetDefaultLoadingPanelWithContentStyle(bool appendLoadingPanelCssClass = false) {
			LoadingPanelStyle style = new LoadingPanelStyle();
			string styleName = "LoadingPanelWithContent";
			style.CssClass = GetCssClassName(GetCssClassNamePrefix(), styleName);
			if(appendLoadingPanelCssClass)
				AppendLoadingPanelCssClass(style, styleName);
			return style;
		}
		protected internal LoadingDivStyle GetDefaultLoadingDivWithContentStyle(bool appendLoadingPanelCssClass = false) {
			LoadingDivStyle style = new LoadingDivStyle();
			string styleName = "LoadingDivWithContent";
			style.CssClass = GetCssClassName(GetCssClassNamePrefix(), styleName);
			if(appendLoadingPanelCssClass)
				AppendLoadingPanelCssClass(style, styleName);
			return style;
		}
		protected internal virtual ExtendedStyleBase GetDefaultTokenBoxTokenStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(CreateStyleByName(TokenBoxTokenStyleName));
			return style;
		}
		protected internal virtual ExtendedStyleBase GetDefaultTokenBoxTokenHoverStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(CreateStyleByName(TokenBoxTokenHoverStyleName));
			return style;
		}
		protected internal virtual ExtendedStyleBase GetDefaultTokenBoxTokenTextStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(CreateStyleByName(TokenBoxTokenTextStyleName));
			return style;
		}
		protected internal virtual ExtendedStyleBase GetDefaultTokenBoxInputStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(CreateStyleByName(TokenBoxInputStyleName));
			return style;
		}
		protected internal virtual ExtendedStyleBase GetDefaultTokenBoxTokenRemoveButtonStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(CreateStyleByName(TokenBoxTokenRemoveButtonStyleName));
			return style;
		}
		protected internal virtual ExtendedStyleBase GetDefaultTokenBoxTokenRemoveButtonHoverStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(CreateStyleByName(TokenBoxTokenRemoveButtonHoverStyleName));
			return style;
		}
		protected internal virtual TrackBarButtonStyle GetDefaultTBIncrementButtonStyle() {
			return CreateTrackBarDefaultStyle<TrackBarButtonStyle>(TBIncrementButtonStyleName);
		}
		protected internal virtual TrackBarButtonStyle GetDefaultTBDecrementButtonStyle() {
			return CreateTrackBarDefaultStyle<TrackBarButtonStyle>(TBDecrementButtonStyleName);
		}
		protected internal virtual TrackBarTrackElementStyle GetDefaultTBBarHighlightStyle() {
			return CreateTrackBarDefaultStyle<TrackBarTrackElementStyle>(TBBarHighlightStyleName);
		}
		protected internal virtual TrackBarButtonStyle GetDefaultTBMainDragHandleStyle() {
			return CreateTrackBarDefaultStyle<TrackBarButtonStyle>(TBMainDragHandleStyleName);
		}
		protected internal virtual TrackBarButtonStyle GetDefaultTBSecondaryDragHandleStyle() {
			return CreateTrackBarDefaultStyle<TrackBarButtonStyle>(TBSecondaryDragHandleStyleName);
		}
		protected internal virtual AppearanceStyle GetDefaultTBTrackStyle() {
			return CreateTrackBarDefaultStyle<AppearanceStyle>(TBTrackStyleName);
		}
		protected internal virtual AppearanceStyle GetDefaultTBScaleStyle() {
			return CreateTrackBarDefaultStyle<AppearanceStyle>(TBScaleStyleName);
		}
		protected internal virtual TrackBarTickStyle GetDefaultTBLargeTickStyle() {
			return CreateTrackBarDefaultStyle<TrackBarTickStyle>(TBLargeTickStyleName);
		}
		protected internal virtual TrackBarTickStyle GetDefaultTBSmallTickStyle() {
			return CreateTrackBarDefaultStyle<TrackBarTickStyle>(TBSmallTickStyleName);
		}
		protected internal virtual AppearanceStyle GetDefaultTBLeftTopLabelStyle() {
			return CreateTrackBarDefaultStyle<AppearanceStyle>(TBLeftTopLabelStyleName);
		}
		protected internal virtual AppearanceStyle GetDefaultTBRightBottomLabelStyle() {
			return CreateTrackBarDefaultStyle<AppearanceStyle>(TBRightBottomLabelStyleName);
		}
		protected internal virtual AppearanceStyle GetDefaultTrackBarStyle() {
			return CreateTrackBarDefaultStyle<AppearanceStyle>(TrackBarStyleName);
		}
		protected internal virtual TrackBarTickStyle GetDefaultTrackBarItemStyle() {
			return CreateTrackBarDefaultStyle<TrackBarTickStyle>(TrackBarItemStyleName);
		}
		protected internal virtual TrackBarTickStyle GetDefaultTrackBarSelectedItemStyle() {
			return CreateTrackBarDefaultStyle<TrackBarTickStyle>(TrackBarSelectedItemStyleName);
		}
		protected internal virtual TrackBarTickStyle GetDefaultTrackBarSelectedTickStyle() {
			return CreateTrackBarDefaultStyle<TrackBarTickStyle>(TrackBarSelectedTickStyleName);
		}
		protected internal virtual TrackBarValueToolTipStyle GetDefaultTBValueToolTipStyle() {
			return CreateTrackBarDefaultStyle<TrackBarValueToolTipStyle>(TrackBarValueToolTipStyleName);
		}
		protected internal virtual HelpTextStyle GetDefaultHelpTextStyle() {
			HelpTextStyle style = new HelpTextStyle();
			style.CopyFrom(CreateStyleByName(HelpTextStyleName));
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultCaptionCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(CaptionCellStyleName));
			return style;
		}
		protected internal virtual AppearanceStyleBase GetDefaultCaptionStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName(CaptionStyleName));
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultRootStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(RootStyleName));
			return style;
		}
		protected internal virtual AppearanceStyleBase GetDefaultRequiredMarkStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName(RequiredMarkStyleName));
			return style;
		}
		protected internal virtual AppearanceStyleBase GetDefaultOptionalMarkStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName(OptionalMarkStyleName));
			return style;
		}
		protected internal string[] GetCaptionAlignmentClassNames(EditorCaptionHorizontalAlign hAlign, EditorCaptionVerticalAlign vAlign) {
			return new string[] {
				GetCaptionHorizontalAlignSystemClassName(hAlign),
				GetCaptionVerticalAlignSystemClassName(vAlign),
			};
		}
		protected internal string GetCaptionHorizontalAlignSystemClassName(EditorCaptionHorizontalAlign align) {
			string cssPrefix = GetCssClassNamePrefix();
			switch (align) {
				case EditorCaptionHorizontalAlign.Left: return cssPrefix + CaptionHALeftSystemClassName;
				case EditorCaptionHorizontalAlign.Right: return cssPrefix + CaptionHARightSystemClassName;
				case EditorCaptionHorizontalAlign.Center: return cssPrefix + CaptionHACenterSystemClassName;
			}
			return string.Empty;
		}
		protected internal string GetCaptionVerticalAlignSystemClassName(EditorCaptionVerticalAlign align) {
			string cssPrefix = GetCssClassNamePrefix();
			switch (align) {
				case EditorCaptionVerticalAlign.Bottom: return cssPrefix + CaptionVABottomSystemClassName;
				case EditorCaptionVerticalAlign.Middle: return cssPrefix + CaptionVAMiddleSystemClassName;
				case EditorCaptionVerticalAlign.Top: return cssPrefix + CaptionVATopSystemClassName;
			}
			return string.Empty;
		}
		protected internal string GetCaptionPositionSystemClassName(EditorCaptionPosition captionPosition) {
			string cssPrefix = GetCssClassNamePrefix();
			switch (captionPosition) {
				case EditorCaptionPosition.Top: return cssPrefix + CaptionPositionTopSystemClassName;
				case EditorCaptionPosition.Bottom: return cssPrefix + CaptionPositionBottomSystemClassName;
				case EditorCaptionPosition.Left: return cssPrefix + CaptionPositionLeftSystemClassName;
				case EditorCaptionPosition.Right: return cssPrefix + CaptionPositionRightSystemClassName;
			}
			return string.Empty;
		}
		protected internal string GetControlTypeSystemClassNameTemplate() {
			return GetCssClassNamePrefix() + "{0}CTypeSys";
		}
		protected internal string GetCaptionCellSafariSystemClassName() {
			return GetCssClassNamePrefix() + CaptionCellSafariSystemClassName;
		}
		protected internal AppearanceStyleBase GetOutOfRangeWarningStyle(bool assignBottomPosCssClass) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName(OutOfRangeWarningStyleName));
			if(assignBottomPosCssClass)
				style.CopyFrom(CreateStyleByName(OutOfRangeWarningBottomPosStyleName));
			else
				style.CopyFrom(CreateStyleByName(OutOfRangeWarningRightPosStyleName));
			return style;
		}
		protected internal T CreateTrackBarDefaultStyle<T>(string className) where T : AppearanceStyleBase, new() {
			T style = new T();
			style.CopyFrom(CreateStyleByName(className + "Style"));
			EditButtonStyle editButtonStyle = style as EditButtonStyle;
			if(editButtonStyle != null) {
				editButtonStyle.HoverStyle.CopyFrom(CreateStyleByName(className + "HoverStyle"));
				editButtonStyle.PressedStyle.CopyFrom(CreateStyleByName(className + "PressedStyle"));
			}
			return style;
		}
		protected internal EditButtonStyle GetDefaultCalendarButtonStyle() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(CreateStyleByName("CalendarButtonStyle"));
			style.HoverStyle.CopyFrom(CreateStyleByName("CalendarButtonHoverStyle"));
			style.PressedStyle.CopyFrom(CreateStyleByName("CalendarButtonPressedStyle"));
			style.ImageSpacing = GetButtonImageSpacing();
			return style;
		}
		protected internal EditButtonStyle GetDefaultButtonEditButtonStyle() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(CreateStyleByName("ButtonEditButtonStyle"));
			style.HoverStyle.CopyFrom(CreateStyleByName("ButtonEditButtonHoverStyle"));
			style.PressedStyle.CopyFrom(CreateStyleByName("ButtonEditButtonPressedStyle"));
			style.ImageSpacing = GetButtonImageSpacing();
			return style;
		}
		protected internal EditButtonStyle GetDefaultButtonEditClearButtonStyle() {
			EditButtonStyle style = GetDefaultButtonEditButtonStyle();
			style.CopyFrom(CreateStyleByName("ButtonEditClearButtonStyle"));
			return style;
		}
		protected internal DisabledStyle GetDefaultButtonDisabledStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(CreateStyleByName("ButtonDisabledStyle"));
			return style;
		}
		protected internal EditAreaStyle GetDefaultEditAreaStyle() {
			EditAreaStyle style = new EditAreaStyle();
			style.CopyFrom(CreateStyleByName("EditAreaStyle"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultErrorFrameStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ErrorFrame"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultErrorFrameStyleErrorIsNotDisplayed() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ErrorFrameWithoutError"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultControlCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ControlsCell"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultErrorCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ErrorCell"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultTextBoxStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("TextBoxStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultButtonEditStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("ButtonEditStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultHyperlinkStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("HyperlinkStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultImageStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("ImageStyle"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultBinaryImageStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("BinaryImageStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultBinaryImageButtonPanelStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("BinaryImageButtonPanelStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultBinaryImageButtonStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("BinaryImageButtonStyle"));
			style.HoverStyle.CopyFrom(CreateStyleByName("BinaryImageButtonHoverStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultBinaryImageDropZoneStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("BinaryImageDropZoneStyle"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultBinaryImageEmptyValueTextStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("BinaryImageEmptyValueTextStyle"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultLabelStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("BaseStyle"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultCheckEditStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("BaseStyle"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultInternalCheckBoxStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(InternalCheckboxControl.FocusedCheckBoxClassName));
			return style;
		}
		protected internal AppearanceStyle GetDefaultInternalRadioButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(InternalCheckboxControl.FocusedRadioButtonClassName));
			return style;
		}
		protected internal AppearanceStyle GetDefaultCheckBoxListStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("CheckBoxListStyle"));
			style.Spacing = 2;
			return style;
		}
		protected internal AppearanceStyle GetDefaultRadioButtonListStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("RadioButtonListStyle"));
			style.Spacing = 2;
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultMemoStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("MemoStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultMemoEditAreaStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("MemoEditAreaStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultListBoxStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("ListBoxStyle"));
			return style;
		}
		protected internal ListBoxItemStyle GetDefaultListBoxItemStyle() {
			ListBoxItemStyle style = new ListBoxItemStyle();			
			style.CopyFrom(CreateStyleByName("ListBoxItemStyle"));
			style.HoverStyle.CopyFrom(CreateStyleByName("ListBoxItemHoverStyle"));
			style.SelectedStyle.CopyFrom(CreateStyleByName("ListBoxItemSelectedStyle"));
			style.ImageSpacing = GetImageSpacing();
			if(SkinOwner.IsRightToLeft())
				style.HorizontalAlign = HorizontalAlign.Right;
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultListBoxItemRowStyle() {
			return CreateStyleByName(ListBoxItemRowStyleName);
		}
		protected internal AppearanceStyleBase GetDefaultSpinIncrementButtonStyle() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(CreateStyleByName("SpinIncButtonStyle"));
			style.HoverStyle.CopyFrom(CreateStyleByName("SpinIncButtonHoverStyle"));
			style.PressedStyle.CopyFrom(CreateStyleByName("SpinIncButtonPressedStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultSpinDecrementButtonStyle() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(CreateStyleByName("SpinDecButtonStyle"));
			style.HoverStyle.CopyFrom(CreateStyleByName("SpinDecButtonHoverStyle"));
			style.PressedStyle.CopyFrom(CreateStyleByName("SpinDecButtonPressedStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultSpinLargeIncrementButtonStyle() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(CreateStyleByName("SpinLargeIncButtonStyle"));
			style.HoverStyle.CopyFrom(CreateStyleByName("SpinLargeIncButtonHoverStyle"));
			style.PressedStyle.CopyFrom(CreateStyleByName("SpinLargeIncButtonPressedStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultSpinLargeDecrementButtonStyle() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(CreateStyleByName("SpinLargeDecButtonStyle"));
			style.HoverStyle.CopyFrom(CreateStyleByName("SpinLargeDecButtonHoverStyle"));
			style.PressedStyle.CopyFrom(CreateStyleByName("SpinLargeDecButtonPressedStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultCalendarStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("CalendarStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefaultCalendarDayHeaderStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarDayHeaderStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefaultCalendarWeekNumberStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarWeekNumberStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefaultCalendarDayStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarDayStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefaultCalendarOtherMonthStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarOtherMonthStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefaultCalendarSelectedStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarSelectedStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefaultCalendarWeekendStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarWeekendStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefaultCalendarDayEmptyStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarDayEmptyStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefaultCalendarOutOfRangeStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarOutOfRangeStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefaultCalendarDayDisabledStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarDayDisabledStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefaultCalendarTodayStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarTodayStyle"));
			return style;
		}
		protected internal CalendarHeaderFooterStyle GetDefaultCalendarHeaderStyle() {
			CalendarHeaderFooterStyle style = new CalendarHeaderFooterStyle();
			style.CopyFrom(CreateStyleByName("CalendarHeaderStyle"));
			return style;
		}
		protected internal CalendarHeaderFooterStyle GetDefaultCalendarFooterStyle() {
			CalendarHeaderFooterStyle style = new CalendarHeaderFooterStyle();
			style.CopyFrom(CreateStyleByName("CalendarFooterStyle"));
			return style;
		}
		protected internal CalendarFastNavStyle GetDefaultCalendarFastNavStyle() {
			CalendarFastNavStyle style = new CalendarFastNavStyle();
			style.CopyFrom(CreateStyleByName("CalendarFastNavStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefaultCalendarFastNavMonthAreaStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarFastNavMonthAreaStyle"));
			return style;
		}
		protected internal CalendarElementStyle GetDefatultCalendarFastNavYearAreaStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(CreateStyleByName("CalendarFastNavYearAreaStyle"));
			return style;
		}
		protected internal CalendarFastNavItemStyle GetDefaultCalendarFastNavMonthStyle() {
			CalendarFastNavItemStyle style = new CalendarFastNavItemStyle();
			style.CopyFrom(CreateStyleByName("CalendarFastNavMonthStyle"));
			style.HoverStyle.CopyFrom(CreateStyleByName("CalendarFastNavMonthHoverStyle"));
			style.SelectedStyle.CopyFrom(CreateStyleByName("CalendarFastNavMonthSelectedStyle"));
			return style;
		}
		protected internal CalendarFastNavItemStyle GetDefaultCalendarFastNavYearStyle() {
			CalendarFastNavItemStyle style = new CalendarFastNavItemStyle();
			style.CopyFrom(CreateStyleByName("CalendarFastNavYearStyle"));
			style.HoverStyle.CopyFrom(CreateStyleByName("CalendarFastNavYearHoverStyle"));
			style.SelectedStyle.CopyFrom(CreateStyleByName("CalendarFastNavYearSelectedStyle"));
			return style;
		}
		protected internal CalendarHeaderFooterStyle GetDefaultCalendarFastNavFooterStyle() {
			CalendarHeaderFooterStyle style = new CalendarHeaderFooterStyle();
			style.CopyFrom(CreateStyleByName("CalendarFastNavFooter"));
			return style;
		}
		protected internal MaskHintStyle GetDefaultMaskHintStyle() {
			MaskHintStyle style = new MaskHintStyle();
			style.CssClass = CreateStyleByName(MaskHintStyleName).CssClass;
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultProgressBarStyle() {
			ProgressBarStyle style = new ProgressBarStyle();
			style.CopyFrom(CreateStyleByName("ProgressBar"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultProgressBarIndicatorStyle() {
			ProgressBarIndicatorStyle style = new ProgressBarIndicatorStyle();
			style.CopyFrom(CreateStyleByName("ProgressBarIndicator"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultDropDownWindowStyle() {
			DropDownWindowStyle style = new DropDownWindowStyle();
			style.CopyFrom(CreateStyleByName("DropDownWindow"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultColorIndicatorStyle() {
			ColorIndicatorStyle style = new ColorIndicatorStyle();
			style.CopyFrom(CreateStyleByName("ColorIndicator"));
			return style;
		}
		protected internal EditorDecorationStyle GetDefaultFocusedStyle() {
			EditorDecorationStyle style = new EditorDecorationStyle();
			style.CopyFrom(CreateStyleByName("Focused"));
			return style;
		}
		protected internal EditorDecorationStyle GetDefaultICBFocusedClass() {
			EditorDecorationStyle style = new EditorDecorationStyle();
			style.CopyFrom(CreateStyleByName(string.Empty, InternalCheckboxControl.FocusedCheckBoxClassName));
			return style;
		}
		protected internal EditorDecorationStyle GetDefaultIRBFocusedClass() {
			EditorDecorationStyle style = new EditorDecorationStyle();
			style.CopyFrom(CreateStyleByName(string.Empty, InternalCheckboxControl.FocusedRadioButtonClassName));
			return style;
		}
		protected internal EditorDecorationStyle GetDefaultICBClass() {
			EditorDecorationStyle style = new EditorDecorationStyle();
			style.CopyFrom(CreateStyleByName(string.Empty, InternalCheckboxControl.CheckBoxClassName));
			return style;
		}
		protected internal EditorDecorationStyle GetDefaultIRBClass() {
			EditorDecorationStyle style = new EditorDecorationStyle();
			style.CopyFrom(CreateStyleByName(string.Empty, InternalCheckboxControl.RadioButtonClassName));
			return style;
		}
		protected internal EditorDecorationStyle GetDefaultNullTextStyle() {
			EditorDecorationStyle style = new EditorDecorationStyle();
			style.CopyFrom(CreateStyleByName("NullText"));
			return style;
		}
		protected internal EditorDecorationStyle GetDefaultInvalidStyle() {
			EditorDecorationStyle style = new EditorDecorationStyle();
			style.CopyFrom(CreateStyleByName("Invalid"));
			return style;
		}
		protected internal Unit GetButtonImageSpacing() {
			return GetImageSpacing();
		}
		protected internal Paddings GetListBoxItemPaddings() {
			return new Paddings(1);
		}
		protected internal Unit GetDefaultDisplayColorIndicatorSpacing() {
			return Unit.Pixel(3);
		}
		static Dictionary<Type, GetStateManagerObject[]> stateDelegates = new Dictionary<Type, GetStateManagerObject[]>();
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			GetStateManagerObject[] state;
			if(!stateDelegates.TryGetValue(GetType(), out state)) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>(base.GetStateManagedObjectsDelegates());
				list.Add(delegate(object styles, bool create) { return ((EditorStyles)styles).GetObject(ref ((EditorStyles)styles).calendarMonthGridPaddings, create); });
				state = list.ToArray();
				lock(stateDelegates)
					stateDelegates[GetType()] = state;
			}
			return state;
		}		
	}
	public class EditorCaptionCellStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorCaptionCellStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), AutoFormatEnable()]
		public new Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorCaptionCellStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), AutoFormatEnable()]
		public new Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
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
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
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
	public class EditorCaptionStyle : AppearanceStyleBase {
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
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
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
	public class EditorRootStyle : AppearanceStyle {
		[Browsable(false)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
}
