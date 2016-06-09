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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxRichEdit {
	public class RichEditStyles : StylesBase {
		public RichEditStyles(ISkinOwner owner)
			: base(owner) { }
		const string CssClassPrefix = "dxre";
		public const string MainElementCssClass = CssClassPrefix + "ControlSys";
		public const string ViewClassName = CssClassPrefix + "-view";
		public const string PageClassName = CssClassPrefix + "-page";
		public const string InputTargetContainerClassName = CssClassPrefix + "-inputTargetContainer";
		public const string StatusBarClassName = CssClassPrefix + "-bar";
		public const string LoadingPanelClassName = CssClassPrefix + "-loadingPanel";
		public const string NotInternalRibbonClassName = CssClassPrefix + "-notInternalRibbon";
		public const string DialogClassName = CssClassPrefix + "-dialog";
		protected override string GetCssClassNamePrefix() {
			return CssClassPrefix;
		}
		public override string ToString() {
			return string.Empty;
		}
		[PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingPanelStyle LoadingPanel {
			get { return base.LoadingPanelInternal; }
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
		}
		protected internal virtual LoadingPanelStyle GetLoadingPanelStyle() {
			LoadingPanelStyle style = new LoadingPanelStyle();
			style.CssClass = LoadingPanelClassName;
			style.CopyFrom(LoadingPanel);
			return style;
		}
	}
	public class RichEditRibbonStyles : RibbonStyles {
		public RichEditRibbonStyles(ASPxRichEdit richedit)
			: base(richedit) {
		}
	}
	public class RichEditDialogFormStyles : PopupControlStyles {
		public RichEditDialogFormStyles(ISkinOwner owner)
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
	public class RichEditMenuStyles : MenuStyles {
		public RichEditMenuStyles(ASPxRichEdit richedit)
			: base(richedit) {
		}
		public override string ToString() {
			return string.Empty;
		}
	}
	public class RichEditButtonStyles : ButtonControlStyles {
		public RichEditButtonStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		public override string ToString() {
			return string.Empty;
		}
	}
	public class RichEditEditorsStyles : EditorStyles {
		public RichEditEditorsStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		public override string ToString() {
			return string.Empty;
		}
	}
	public class RichEditFileManagerStyles : DevExpress.Web.FileManagerStyles {
		const string ControlStyleName = "ControlStyle";
		public RichEditFileManagerStyles(ISkinOwner owner)
			: base(owner) {
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
	DevExpressWebASPxRichEditLocalizedDescription("RichEditFileManagerStylesControl"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyleBase Control {
			get { return GetStyle(ControlStyleName); }
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ControlStyleName, delegate() { return new AppearanceStyleBase(); }));
		}
	}
	public class RichEditStatusBarStyles : StylesBase {
		public RichEditStatusBarStyles(ISkinOwner owner)
			: base(owner) {
		}
		public override string ToString() {
			return string.Empty;
		}
	}
	public class RichEditRulerStyles : StylesBase {
		public const string ControlStyleName = "ruler",
							WrapperStyleName = "rulerWrapper",
							LineStyleName = "rulerLine",
							LeftIdentDragHandleStyleName = "leftIdentDragHandle",
							RightIdentDragHandleStyleName = "rightIdentDragHandle",
							FirstLineIdentDragHandleStyleName = "firstLineIdentDragHandle",
							TabDragHandleStyleName = "tabDragHandle";
		public RichEditRulerStyles(ISkinOwner owner)
			: base(owner) { }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public AppearanceStyle Control {
			get { return (AppearanceStyle)GetStyle(ControlStyleName); }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public AppearanceStyle Line {
			get { return (AppearanceStyle)GetStyle(LineStyleName); }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public AppearanceStyle TabDragHandle {
			get { return (AppearanceStyle)GetStyle(TabDragHandleStyleName); }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public AppearanceStyle LeftIdentDragHandle {
			get { return (AppearanceStyle)GetStyle(LeftIdentDragHandleStyleName); }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public AppearanceStyle RightIdentDragHandle {
			get { return (AppearanceStyle)GetStyle(RightIdentDragHandleStyleName); }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public AppearanceStyle FirstLineIdentDragHandle {
			get { return (AppearanceStyle)GetStyle(FirstLineIdentDragHandleStyleName); }
		}
		protected override string GetCssClassNamePrefix() {
			return "dxre";
		}
		protected internal CreateStyleHandler GetStyleHandler<T>() where T : AppearanceStyleBase, new() {
			return delegate() { return new T(); };
		}
		protected T GetDefaultStyle<T>(string cssClassName) where T : AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = string.Format("{0}-{1}", GetCssClassNamePrefix(), cssClassName);
			return style;
		}
		protected internal AppearanceStyle GetControlStyle() {
			return GetDefaultStyle<AppearanceStyle>(ControlStyleName);
		}
		protected internal virtual AppearanceStyle GetDefaultWrapperStyle() {
			return GetDefaultStyle<AppearanceStyle>(WrapperStyleName);
		}
		protected internal virtual AppearanceStyle GetDefaultLineStyle() {
			return GetDefaultStyle<AppearanceStyle>(LineStyleName);
		}
		protected internal virtual AppearanceStyle GetDefaultLeftIdentDragHandleStyle() {
			return GetDefaultStyle<AppearanceStyle>(LeftIdentDragHandleStyleName);
		}
		protected internal virtual AppearanceStyle GetDefaultRightIdentDragHandleStyle() {
			return GetDefaultStyle<AppearanceStyle>(RightIdentDragHandleStyleName);
		}
		protected internal virtual AppearanceStyle GetDefaultFirstLineIdentDragHandleStyle() {
			return GetDefaultStyle<AppearanceStyle>(FirstLineIdentDragHandleStyleName);
		}
		protected internal virtual AppearanceStyle GetDefaultTabDragHandleStyle() {
			return GetDefaultStyle<AppearanceStyle>(TabDragHandleStyleName);
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ControlStyleName, GetStyleHandler<AppearanceStyle>()));
			list.Add(new StyleInfo(WrapperStyleName, GetStyleHandler<AppearanceStyle>()));
			list.Add(new StyleInfo(LineStyleName, GetStyleHandler<AppearanceStyle>()));
			list.Add(new StyleInfo(LeftIdentDragHandleStyleName, GetStyleHandler<AppearanceStyle>()));
			list.Add(new StyleInfo(RightIdentDragHandleStyleName, GetStyleHandler<AppearanceStyle>()));
			list.Add(new StyleInfo(FirstLineIdentDragHandleStyleName, GetStyleHandler<AppearanceStyle>()));
			list.Add(new StyleInfo(TabDragHandleStyleName, GetStyleHandler<AppearanceStyle>()));
		}
	}
}
