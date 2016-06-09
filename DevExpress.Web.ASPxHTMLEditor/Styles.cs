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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxSpellChecker;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	public class HtmlEditorUploadControlStyles : UploadControlStyles {
		public HtmlEditorUploadControlStyles(ASPxUploadControl owner)
			: base(owner) {
		}
	}
	public class HtmlEditorAreaStyleBase : AppearanceStyleBase {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
	}
	public class HtmlEditorAreaStyle : HtmlEditorAreaStyleBase {
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorAreaStylePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new Paddings Paddings {
			get { return base.Paddings; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string CssClass {
			get { return base.CssClass; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font {
			get { return base.Font; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor {
			get { return base.ForeColor; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop { get { return base.BorderTop; } }
	}
	public class HtmlEditorCommonViewAreaStyle : HtmlEditorAreaStyleBase {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Paddings Paddings {
			get { return base.Paddings; }
		}
	}
	public class HtmlEditorViewAreaStyle : HtmlEditorAreaStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Paddings Paddings {
			get { return base.Paddings; }
		}
	}
	public class HtmlEditorStatusBarStyle : HtmlEditorAreaStyleBase {
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStatusBarStyleTabSpacing"),
#endif
		AutoFormatEnable, DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public Unit TabSpacing {
			get { return base.Spacing; }
			set {
				base.Spacing = value;
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStatusBarStylePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new Paddings Paddings { get { return base.Paddings; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop { get { return base.BorderTop; } }
	}
	public class HtmlEditorStatusBarTabStyle : TabStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return Unit.Empty; }
			set { }
		}
	}
	public class HtmlEditorStatusBarSizeGripStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Width {
			get { return Unit.Empty; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height {
			get { return Unit.Empty; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage {
			get { return base.BackgroundImage; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font {
			get { return base.Font; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor {
			get { return base.ForeColor; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
	}
	public class HtmlEditorErrorFrameStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorErrorFrameStyleImageSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
	}
	public class HtmlEditorErrorFrameCloseButtonStyle : ButtonStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle PressedStyle {
			get { return base.PressedStyle; }
		}
	}
	public class HtmlEditorSpellCheckerStyles : SpellCheckerStyles {
		public HtmlEditorSpellCheckerStyles(ASPxHtmlEditor editor)
			: base(editor) {
		}
	}
	public class HtmlEditorStatusBarStyles : StylesBase {
		private const string StatusBarStyleName = "StatusBar";
		private const string TabStyleName = "StatusBarTab";
		private const string ActiveTabStyleName = "StatusBarActiveTab";
		private const string SizeGripStyleName = "SizeGrip";
		private const string SizeGripContainerStyleName = "SizeGripContainer";
		private HtmlEditorViewModeTabControlStyles viewModeTabControlStyles;
		public HtmlEditorStatusBarStyles(ISkinOwner owner)
			: base(owner) {
			this.viewModeTabControlStyles = new HtmlEditorViewModeTabControlStyles(owner);
		}
		[Browsable(false)]
		public new string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStatusBarStylesTab"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public HtmlEditorStatusBarTabStyle Tab { get { return ViewModeTabControlStyles.Tab; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStatusBarStylesActiveTab"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public HtmlEditorStatusBarTabStyle ActiveTab { get { return ViewModeTabControlStyles.ActiveTab; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStatusBarStylesStatusBar"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public HtmlEditorStatusBarStyle StatusBar { get { return (HtmlEditorStatusBarStyle)GetStyle(StatusBarStyleName); } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStatusBarStylesSizeGrip"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public HtmlEditorStatusBarSizeGripStyle SizeGrip { get { return (HtmlEditorStatusBarSizeGripStyle)GetStyle(SizeGripStyleName); } }
		protected HtmlEditorViewModeTabControlStyles ViewModeTabControlStyles {
			get { return viewModeTabControlStyles; }
		}
		protected override string GetCssClassNamePrefix() {
			return HtmlEditorStyles.CssClassPrefix;
		}
		protected internal HtmlEditorStatusBarStyle GetDefaultStatusBarStyle() {
			HtmlEditorStatusBarStyle style = new HtmlEditorStatusBarStyle();
			style.CopyFrom(CreateStyleByName(StatusBarStyleName));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultTabStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName(TabStyleName));
			return style;
		}
		protected internal AppearanceStyleBase GetDafaultActiveTabStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName(ActiveTabStyleName));
			return style;
		}
		protected internal AppearanceStyle GetLightStatusBarStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.Paddings.Padding = 0;
			style.Paddings.PaddingTop = Unit.Pixel(5);
			style.HorizontalAlign = HorizontalAlign.Right;
			return style;
		}
		protected internal AppearanceStyleBase GetSizeGripContainerStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName(SizeGripContainerStyleName));
			return style;
		}
		protected internal Paddings GetDefaultTabControlPaddings() {
			Paddings ret = new Paddings();
			ret.PaddingLeft = Unit.Pixel(8);
			ret.PaddingRight = Unit.Pixel(0);
			return ret;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(StatusBarStyleName, delegate() { return new HtmlEditorStatusBarStyle(); }));
			list.Add(new StyleInfo(SizeGripStyleName, delegate() { return new HtmlEditorStatusBarSizeGripStyle(); }));
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			HtmlEditorStatusBarStyles styles = source as HtmlEditorStatusBarStyles;
			if(styles != null) {
				ActiveTab.CopyFrom(styles.ActiveTab);
				Tab.CopyFrom(styles.Tab);
			}
		}
		public override void Reset() {
			base.Reset();
			ActiveTab.Reset();
			Tab.Reset();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				Tab, ActiveTab
			});
		}
	}
	public class HtmlEditorViewModeTabControlStyles : TabControlStyles {
		public HtmlEditorViewModeTabControlStyles(ISkinOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorViewModeTabControlStylesActiveTab"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new HtmlEditorStatusBarTabStyle ActiveTab {
			get { return (HtmlEditorStatusBarTabStyle)GetStyle(ActiveTabStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorViewModeTabControlStylesTab"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new HtmlEditorStatusBarTabStyle Tab {
			get { return (HtmlEditorStatusBarTabStyle)GetStyle(TabStyleName); }
		}
		protected override StyleInfo CreateActiveTabStyleInfo() {
			return new StyleInfo(ActiveTabStyleName, delegate() { return new HtmlEditorStatusBarTabStyle(); });
		}
		protected override StyleInfo CreateTabStyleInfo() {
			return new StyleInfo(TabStyleName, delegate() { return new HtmlEditorStatusBarTabStyle(); });
		}
		protected override string GetCssClassNamePrefix() {
			return HtmlEditorStyles.CssClassPrefix;
		}
	}
	public class HtmlEditorStyles : StylesBase {
		protected internal const string CssClassPrefix = "dxhe";
		public const string ContentAreaStyleName = "ContentArea";
		public const string DesignViewAreaStyleName = "DesignViewArea";
		public const string HtmlViewAreaStyleName = "HtmlViewArea";
		public const string PreviewAreaStyleName = "PreviewArea";
		public const string ViewAreaStyleName = "ViewArea";
		public const string RightToLeftCssClass = CssClassPrefix + "-rtl";
		public const string TagInspectorWrapperCssClass = CssClassPrefix + "-tiWrapper";
		public const string TagInspectorControlsTagsContainerCssClass = CssClassPrefix + "-tagsContainer";
		protected internal const string CustomDialogCssClassPrefix = CssClassPrefix + "cd-";
		protected internal const string CustomDialogContentCssClass = CustomDialogCssClassPrefix + "Content";
		protected internal const string CustomDialogButtonsCssClass = CustomDialogCssClassPrefix + "Buttons";
		protected internal const string CustomDialogOkButtonCssClass = CustomDialogCssClassPrefix + "Ok";
		protected internal const string CustomDialogCancelButtonCssClass = CustomDialogCssClassPrefix + "Cancel";
		public HtmlEditorStyles(ASPxHtmlEditor editor)
			: base(editor) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStylesLoadingPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingPanelStyle LoadingPanel {
			get { return base.LoadingPanelInternal; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStylesLoadingDiv"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingDivStyle LoadingDiv {
			get { return base.LoadingDivInternal; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStylesContentArea"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorAreaStyle ContentArea {
			get { return (HtmlEditorAreaStyle)GetStyle(ContentAreaStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStylesDesignViewArea"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorViewAreaStyle DesignViewArea {
			get { return (HtmlEditorViewAreaStyle)GetStyle(DesignViewAreaStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStylesHtmlViewArea"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorViewAreaStyle HtmlViewArea {
			get { return (HtmlEditorViewAreaStyle)GetStyle(HtmlViewAreaStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStylesPreviewArea"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorViewAreaStyle PreviewArea {
			get { return (HtmlEditorViewAreaStyle)GetStyle(PreviewAreaStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorStylesViewArea"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorCommonViewAreaStyle ViewArea {
			get { return (HtmlEditorCommonViewAreaStyle)GetStyle(ViewAreaStyleName); }
		}
		public override string ToString() {
			return string.Empty;
		}
		protected override string GetCssClassNamePrefix() {
			return HtmlEditorStyles.CssClassPrefix;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ContentAreaStyleName, delegate() { return new HtmlEditorAreaStyle(); }));
			list.Add(new StyleInfo(DesignViewAreaStyleName, delegate() { return new HtmlEditorViewAreaStyle(); }));
			list.Add(new StyleInfo(HtmlViewAreaStyleName, delegate() { return new HtmlEditorViewAreaStyle(); }));
			list.Add(new StyleInfo(PreviewAreaStyleName, delegate() { return new HtmlEditorViewAreaStyle(); }));
			list.Add(new StyleInfo(ViewAreaStyleName, delegate() { return new HtmlEditorCommonViewAreaStyle(); }));
		}
		protected override AppearanceStyle GetDefaultControlStyle() {
			AppearanceStyle ret = new AppearanceStyle();
			ret.CopyFrom(base.GetDefaultControlStyle());
			ret.Width = Unit.Pixel(710);
			ret.Height = Unit.Pixel(400);
			return ret;
		}
		protected internal AppearanceStyleBase GetDefaultContentAreaStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("ContentAreaStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultDesignViewAreaStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("DesignViewAreaStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultHtmlViewAreaStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("HtmlViewAreaStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultPreviewAreaStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("PreviewAreaStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultViewAreaStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("ViewAreaStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultCustomDialogStyle() {
			return CreateStyleCopyByName<AppearanceStyleBase>("CustomDialogStyle");
		}
		protected internal HtmlEditorErrorFrameStyle GetDefaultErrorFrameStyle() {
			HtmlEditorErrorFrameStyle style = new HtmlEditorErrorFrameStyle();
			style.CopyFrom(CreateStyleByName("ErrorFrameStyle"));
			style.ImageSpacing = GetErrorFrameImageSpacing();
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultErrorFrameCloseButtonStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("ErrorFrameCloseButtonStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultErrorFrameCloseButtonHoverStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("ErrorFrameCloseButtonHoverStyle"));
			return style;
		}
		protected virtual Unit GetErrorFrameImageSpacing() {
			return GetImageSpacing();
		}
	}
	public class HtmlEditorEditorStyles : EditorStyles {
		public const string DropDownItemPickerStyleName = "DropDownItemPickerStyle";
		public const string DropDownItemPickerItemStyleName = "DropDownItemPickerItemStyle";
		public HtmlEditorEditorStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		public override string ToString() { return string.Empty; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorEditorStylesDropDownItemPickerStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorDropDownItemPickerStyle DropDownItemPickerStyle {
			get { return (HtmlEditorDropDownItemPickerStyle)GetStyle(DropDownItemPickerStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorEditorStylesDropDownItemPickerItemStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorDropDownItemPickerItemStyle DropDownItemPickerItemStyle {
			get { return (HtmlEditorDropDownItemPickerItemStyle)GetStyle(DropDownItemPickerItemStyleName); }
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(DropDownItemPickerStyleName, delegate() { return new HtmlEditorDropDownItemPickerStyle(); }));
			list.Add(new StyleInfo(DropDownItemPickerItemStyleName, delegate() { return new HtmlEditorDropDownItemPickerItemStyle(); }));
		}
	}
	[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
	public class HtmlEditorDropDownItemPickerStyle : ItemPickerTableStyle {
	}
	[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
	public class HtmlEditorDropDownItemPickerItemStyle : ItemPickerTableCellStyle {
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDropDownItemPickerItemStyleImageSpacing"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDropDownItemPickerItemStyleFont"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new FontInfo Font {
			get { return base.Font; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle SelectedStyle {
			get { return base.SelectedStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected override DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
	}
	public class HtmlEditorButtonStyles : ButtonControlStyles {
		public HtmlEditorButtonStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		public override string ToString() { return string.Empty; }
	}
	public class HtmlEditorMenuStyles : MenuStyles {
		public HtmlEditorMenuStyles(ASPxHtmlEditor editor)
			: base(editor) {
		}
		public override string ToString() { return string.Empty; }
	}
	public class HtmlEditorPasteOptionsBarStyle : MenuStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage GutterBackgroundImage {
			get { return base.GutterBackgroundImage; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit GutterWidth {
			get { return base.GutterWidth; }
			set { base.GutterWidth = value; }
		}
	}
	public class HtmlEditorPasteOptionsBarItemStyle : DevExpress.Web.MenuItemStyle {
	}
	public class HtmlEditorPasteOptionsBarStyles : StylesBase {
		public const string PasteOptionsBarStyleName = "PasteOptionsBar";
		public const string PasteOptionsBarItemStyleName = "PasteOptionsBarItem";
		public HtmlEditorPasteOptionsBarStyles(ISkinOwner owner)
			: base(owner) {
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HtmlEditorPasteOptionsBarStyle PasteOptionsBar {
			get { return (HtmlEditorPasteOptionsBarStyle)GetStyle(PasteOptionsBarStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HtmlEditorPasteOptionsBarItemStyle PasteOptionsBarItem {
			get { return (HtmlEditorPasteOptionsBarItemStyle)GetStyle(PasteOptionsBarItemStyleName); }
		}
		[Browsable(false)]
		public new string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		public override void CopyFrom(StylesBase source) {
			HtmlEditorPasteOptionsBarStyles pasteOptionsStyles = source as HtmlEditorPasteOptionsBarStyles;
			if(pasteOptionsStyles != null) {
				PasteOptionsBar.CopyFrom(pasteOptionsStyles.PasteOptionsBar as Style);
				PasteOptionsBarItem.CopyFrom(pasteOptionsStyles.PasteOptionsBarItem as Style);
			}
			base.CopyFrom(source);
		}
		protected override string GetCssClassNamePrefix() {
			return MenuStyles.ToolbarCssClassPrefix;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(PasteOptionsBarStyleName, delegate() { return new HtmlEditorPasteOptionsBarStyle(); }));
			list.Add(new StyleInfo(PasteOptionsBarItemStyleName, delegate() { return new HtmlEditorPasteOptionsBarItemStyle(); }));
		}
		protected internal BarDockStyle GetDefaultPasteOptionsStyle() {
			BarDockStyle style = new BarDockStyle();
			style.CopyFrom(CreateStyleByName("ControlStyle"));
			return style;
		}
	}
	public class HtmlEditorDialogFormStylesLite : PopupControlStyles {
		public HtmlEditorDialogFormStylesLite(ISkinOwner owner)
			: base(owner) {
		}
		protected override PopupControlModalBackgroundStyle GetDefaultModalBackgroundStyle() {
			PopupControlModalBackgroundStyle style = new PopupControlModalBackgroundStyle();
			style.CopyFrom(CreateStyleByName("ModalBackLite"));
			style.Opacity = 1;
			return style;
		}
		protected override PopupWindowContentStyle GetDefaultContentStyle() {
			PopupWindowContentStyle style = base.GetDefaultContentStyle();
			style.Paddings.Assign(new Paddings(Unit.Pixel(0)));
			return style;
		}
	}
	public class HtmlEditorDialogFormStyles : PopupControlStyles {
		public HtmlEditorDialogFormStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override PopupControlModalBackgroundStyle GetDefaultModalBackgroundStyle() {
			PopupControlModalBackgroundStyle style = new PopupControlModalBackgroundStyle();
			style.CopyFrom(CreateStyleByName("ModalBackgroundStyle"));
			style.Opacity = 1;
			return style;
		}
		protected override PopupWindowContentStyle GetDefaultContentStyle() {
			PopupWindowContentStyle style = base.GetDefaultContentStyle();
			style.Paddings.Assign(new Paddings(Unit.Pixel(0)));
			return style;
		}
	}
	public class HtmlEditorTagInspectorSelectionStyles : AppearanceStyleBase {
		[
		Category("Appearance"), NotifyParentProperty(true), DefaultValue(DefaultOpacity), AutoFormatEnable()]
		public new virtual int Opacity {
			get { return base.Opacity; }
			set { base.Opacity = value; }
		}
	}
	public class HtmlEditorTagInspectorSelectedTagContainerStyles : AppearanceStyleBase {
		[
		Category("Appearance"), NotifyParentProperty(true), DefaultValue(DefaultOpacity), AutoFormatEnable()]
		public new virtual Paddings Paddings {
			get { return base.Paddings; }
		}
	}
	public class HtmlEditorTagInspectorStyles : StylesBase {
		public const string TagStyleName = "Tag",
							SelectedTagStyleName = "SelectedTag",
							SelectionStyleName = "Selection",
							EllipsisStyleName = "Ellipsis",
							SelectedTagContainerStyleName = "SelectedTagContainer";
		const string SysTagNameClassName = "-tiTagName",
					 SysTagNameHoverClassName = "-tiTagNameHover",
					 SysSelectedTagNameHoverClassName = "-tiSelected",
					 SysTagEllipsisClassName = "-tagEllipsis",
					 SysControlsWrapperClassName = "-tiControlsWrapper",
					 SysSelectionClassName = "-tiSelection";
		public HtmlEditorTagInspectorStyles(ISkinOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorTagInspectorStylesTag"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), 
		AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle Tag {
			get { return (AppearanceStyle)GetStyle(TagStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorTagInspectorStylesSelectedTag"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty),
		AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle SelectedTag {
			get { return (AppearanceStyle)GetStyle(SelectedTagStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorTagInspectorStylesSelection"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty),
		AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorTagInspectorSelectionStyles Selection {
			get { return (HtmlEditorTagInspectorSelectionStyles)GetStyle(SelectionStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorTagInspectorStylesEllipsis"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty),
		AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle Ellipsis {
			get { return (AppearanceStyle)GetStyle(EllipsisStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorTagInspectorStylesSelectedTagContainer"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty),
		AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorTagInspectorSelectedTagContainerStyles SelectedTagContainer {
			get { return (HtmlEditorTagInspectorSelectedTagContainerStyles)GetStyle(SelectedTagContainerStyleName); }
		}
		[Browsable(false)]
		public new string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			HtmlEditorTagInspectorStyles styles = source as HtmlEditorTagInspectorStyles;
			if(styles != null) {
				Tag.CopyFrom(styles.Tag as Style);
				SelectedTag.CopyFrom(styles.SelectedTag as Style);
				Selection.CopyFrom(styles.Selection as Style);
				Ellipsis.CopyFrom(styles.Ellipsis as Style);
				SelectedTagContainer.CopyFrom(styles.SelectedTagContainer as Style);
			}
		}
		protected override string GetCssClassNamePrefix() {
			return MenuStyles.ToolbarCssClassPrefix;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(TagStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(SelectedTagStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(SelectionStyleName, delegate() { return new HtmlEditorTagInspectorSelectionStyles(); }));
			list.Add(new StyleInfo(EllipsisStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(SelectedTagContainerStyleName, delegate() { return new HtmlEditorTagInspectorSelectedTagContainerStyles(); }));
		}
		protected internal AppearanceStyleBase GetTagStyle() {
			return GetMergedStyle(Tag, TagStyleName, SysTagNameClassName);
		}
		protected internal AppearanceStyleBase GetTagHoverStyle() {
			return GetMergedStyle(Tag.HoverStyle, TagStyleName, SysTagNameClassName, SysTagNameHoverClassName);
		}
		protected internal AppearanceStyleBase GetSelectedTagStyle() {
			return GetMergedStyle(SelectedTag, SelectedTagStyleName, SysTagNameClassName, SysSelectedTagNameHoverClassName);
		}
		protected internal AppearanceStyleBase GetSelectedTagHoverStyle() {
			return GetMergedStyle(SelectedTag.HoverStyle, SelectedTagStyleName, SysTagNameClassName, SysSelectedTagNameHoverClassName, SysTagNameHoverClassName);
		}
		protected internal AppearanceStyleBase GetEllipsisStyle() {
			return GetMergedStyle(Ellipsis, EllipsisStyleName, SysTagEllipsisClassName);
		}
		protected internal AppearanceStyleBase GetSelectedTagContainerStyle() {
			return GetMergedStyle(SelectedTagContainer, SelectedTagContainerStyleName, SysControlsWrapperClassName);
		}
		protected internal AppearanceStyleBase GetSelectionStyle() {
			return GetMergedStyle(Selection, SelectionStyleName, SysSelectionClassName);
		}
		protected internal AppearanceStyleBase GetMergedStyle(AppearanceStyleBase style, string themableCssClass, params string[] cssClassNames) {
			AppearanceStyleBase result = new AppearanceStyleBase();
			result.CssClass = string.Join(" ", cssClassNames.Select(n => HtmlEditorStyles.CssClassPrefix + n));
			AppendStyleCssClass(result, HtmlEditorStyles.CssClassPrefix, themableCssClass);
			result.CopyFrom(style);
			return result;
		}
	}
}
