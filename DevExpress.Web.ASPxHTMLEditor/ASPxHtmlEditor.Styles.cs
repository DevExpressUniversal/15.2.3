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

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	public partial class ASPxHtmlEditor {
		[Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorTagInspectorStyles StylesTagInspector { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorStyles Styles { get { return StylesInternal as HtmlEditorStyles; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorStylesToolbars"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ToolbarsStyles StylesToolbars { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorStylesButton"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorButtonStyles StylesButton { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorStylesContextMenu"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorMenuStyles StylesContextMenu { get; private set; }
		[Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorPasteOptionsBarStyles StylesPasteOptionsBar { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorStylesEditors"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorEditorStyles StylesEditors { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorStylesDialogForm"),
#endif
		 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorDialogFormStyles StylesDialogForm { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorStylesRoundPanel"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorRoundPanelStyles StylesRoundPanel { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorStylesSpellChecker"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorSpellCheckerStyles StylesSpellChecker { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorStylesStatusBar"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorStatusBarStyles StylesStatusBar { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorStylesFileManager"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorFileManagerStyles StylesFileManager { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorStylesDocument"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorDocumentStyles StylesDocument { get; private set; }
		protected void InitializeStyles() {
			StylesTagInspector = new HtmlEditorTagInspectorStyles(this);
			StylesToolbars = new ToolbarsStyles(this);
			StylesButton = new HtmlEditorButtonStyles(this);
			StylesContextMenu = new HtmlEditorMenuStyles(null);
			StylesPasteOptionsBar = new HtmlEditorPasteOptionsBarStyles(this);
			StylesEditors = new HtmlEditorEditorStyles(this);
			StylesDialogForm = new HtmlEditorDialogFormStyles(this);
			StylesRoundPanel = new HtmlEditorRoundPanelStyles(this);
			StylesStatusBar = new HtmlEditorStatusBarStyles(this);
			StylesSpellChecker = new HtmlEditorSpellCheckerStyles(this);
			StylesFileManager = new HtmlEditorFileManagerStyles(this);
			StylesDocument = new HtmlEditorDocumentStyles(this);
		}
		protected internal AppearanceStyle GetContentAreaStyle() {
			AppearanceStyle ret = new AppearanceStyle();
			ret.CopyFrom(Styles.GetDefaultContentAreaStyle());
			ret.CopyFrom(ControlStyle, true);
			ret.CopyFrom(Styles.ContentArea);
			return ret;
		}
		protected internal AppearanceStyleBase GetDesignViewAreaStyle() {
			AppearanceStyleBase ret = new AppearanceStyleBase();
			ret.CopyFrom(Styles.GetDefaultDesignViewAreaStyle());
			ret.CopyFrom(GetViewAreaStyle());
			ret.CopyFrom(Styles.DesignViewArea);
			RenderUtils.AppendCssClass(ret, GetDocTypeCssClassMarker());
			if(IsRightToLeft())
				ret.CssClass = RenderUtils.CombineCssClasses(ret.CssClass, HtmlEditorStyles.RightToLeftCssClass);
			return ret;
		}
		protected virtual string GetDocTypeCssClassMarker() {
			return string.Format("dxhe-docType{0}", SettingsHtmlEditing.AllowedDocumentType.ToString());
		}
		AppearanceStyleBase GetIFrameImportedStyle(AppearanceStyleBase baseStyle) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(baseStyle);
			style.Border.Reset();
			style.BorderLeft.Reset();
			style.BorderRight.Reset();
			style.BorderTop.Reset();
			style.BorderBottom.Reset();
			return style;
		}
		protected internal AppearanceStyleBase GetDesignViewAreaStyleForScript() {
			return GetIFrameImportedStyle(GetDesignViewAreaStyle());
		}
		protected internal AppearanceStyle GetHtmlViewAreaStyle() {
			AppearanceStyle ret = new AppearanceStyle();
			ret.CopyFrom(Styles.GetDefaultHtmlViewAreaStyle());
			ret.CopyFrom(GetViewAreaStyle());
			ret.CopyFrom(Styles.HtmlViewArea);
			return ret;
		}
		protected internal AppearanceStyleBase GetPreviewAreaStyle() {
			AppearanceStyleBase ret = new AppearanceStyleBase();
			ret.CopyFrom(Styles.GetDefaultPreviewAreaStyle());
			ret.CopyFontFrom(GetContentAreaStyle());
			ret.CopyFrom(GetViewAreaStyle());
			ret.CopyFrom(Styles.PreviewArea);
			if(IsRightToLeft())
				ret.CssClass = RenderUtils.CombineCssClasses(ret.CssClass, HtmlEditorStyles.RightToLeftCssClass);
			return ret;
		}
		protected internal AppearanceStyleBase GetPreviewAreaStyleForScript() {
			return GetIFrameImportedStyle(GetPreviewAreaStyle());
		}
		protected AppearanceStyleBase GetViewAreaStyleInternal() {
			AppearanceStyleBase ret = new AppearanceStyleBase();
			ret.CopyFrom(Styles.GetDefaultViewAreaStyle());
			ret.CopyFrom(Styles.ViewArea);
			return ret;
		}
		protected internal string GetDocumentStyleCssText() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			string cssText = StylesDocument.HorizontalAlign == HorizontalAlign.NotSet ? "" : string.Format("text-align:{0};", StylesDocument.HorizontalAlign);
			cssText += StylesDocument.MergeStyles(style).GetStyleAttributes(Page).Value ?? "";
			return cssText;
		}
		protected internal AppearanceStyleBase GetViewAreaStyle() {
			AppearanceStyleBase ret = new AppearanceStyleBase();
			ret.CopyFrom(GetViewAreaStyleInternal());
			if(ShowViewSwitcher)
				ret.BorderBottom.BorderWidth = 0;
			return ret;
		}
		protected internal HtmlEditorPasteOptionsBarStyle GetPasteOptionsBarStyle() {
			HtmlEditorPasteOptionsBarStyle style = new HtmlEditorPasteOptionsBarStyle();
			style.CopyFrom(StylesPasteOptionsBar.GetDefaultPasteOptionsStyle());
			style.CopyFrom(StylesPasteOptionsBar.PasteOptionsBar);
			return style;
		}
		protected internal HtmlEditorPasteOptionsBarItemStyle GetPasteOptionsBarItemStyle() {
			HtmlEditorPasteOptionsBarItemStyle style = new HtmlEditorPasteOptionsBarItemStyle();
			style.CopyFrom(StylesPasteOptionsBar.PasteOptionsBarItem);
			return style;
		}
		protected internal HtmlEditorStatusBarStyle GetStatusBarStyle() {
			HtmlEditorStatusBarStyle style = new HtmlEditorStatusBarStyle();
			style.CopyFrom(StylesStatusBar.GetDefaultStatusBarStyle());
			style.CopyFrom(GetContentAreaStyle());
			style.CopyFrom(StylesStatusBar.StatusBar);
			return style;
		}
		protected internal AppearanceStyleBase GetStatusBarTabStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(StylesStatusBar.GetDefaultTabStyle());
			style.CopyFontFrom(GetStatusBarStyle());
			style.CopyFrom(StylesStatusBar.Tab);
			return style;
		}
		protected internal AppearanceStyleBase GetStatusBarActiveTabStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(StylesStatusBar.GetDafaultActiveTabStyle());
			style.CopyFontFrom(GetStatusBarStyle());
			style.CopyFrom(StylesStatusBar.ActiveTab);
			return style;
		}
		protected internal AppearanceStyleBase GetStatusBarContentStyle() {
			return GetViewAreaStyleInternal();
		}
		protected internal HtmlEditorStatusBarSizeGripStyle GetStatusBarSizeGripStyle() {
			HtmlEditorStatusBarSizeGripStyle style = new HtmlEditorStatusBarSizeGripStyle();
			style.CopyFrom(StylesStatusBar.SizeGrip);
			return style;
		}
		protected internal AppearanceStyleBase GetCustomDialogStyle(string customDialogName) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultCustomDialogStyle());
			return style;
		}
		protected internal HtmlEditorErrorFrameCloseButtonStyle GetErrorFrameCloseButtonStyle() {
			HtmlEditorErrorFrameCloseButtonStyle style = new HtmlEditorErrorFrameCloseButtonStyle();
			style.CopyFrom(Styles.GetDefaultErrorFrameCloseButtonStyle());
			style.CopyFrom(SettingsValidation.ErrorFrameCloseButtonStyle);
			return style;
		}
		protected internal HtmlEditorErrorFrameCloseButtonStyle GetErrorFrameCloseButtonHoverStyle() {
			HtmlEditorErrorFrameCloseButtonStyle style = new HtmlEditorErrorFrameCloseButtonStyle();
			style.CopyFrom(Styles.GetDefaultErrorFrameCloseButtonHoverStyle());
			style.CopyFrom(SettingsValidation.ErrorFrameCloseButtonStyle.HoverStyle);
			return style;
		}
		protected internal Paddings GetErrorFrameCloseButtonPaddings() {
			return GetErrorFrameCloseButtonStyle().Paddings;
		}
		protected internal HtmlEditorErrorFrameStyle GetErrorFrameStyle() {
			HtmlEditorErrorFrameStyle style = new HtmlEditorErrorFrameStyle();
			style.CopyFrom(Styles.GetDefaultErrorFrameStyle());
			style.CopyFrom(SettingsValidation.ErrorFrameStyle);
			return style;
		}
		protected internal Paddings GetStatusBarPaddings() {
			Paddings ret = new Paddings();
			ret.CopyFrom(GetContentAreaStyle().Paddings);
			ret.PaddingTop = Unit.Pixel(0);
			return ret;
		}
		protected internal Paddings GetStatusBarTabControlPaddings() {
			Paddings ret = new Paddings();
			ret.CopyFrom(StylesStatusBar.GetDefaultTabControlPaddings());
			ret.CopyFrom(StylesStatusBar.StatusBar.Paddings);
			return ret;
		}
		protected internal Unit GetStatusBarTabSpacing() {
			return GetStatusBarStyle().TabSpacing;
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new HtmlEditorStyles(this);
		}
	}
}
