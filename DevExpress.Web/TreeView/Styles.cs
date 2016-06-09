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
using DevExpress.Web;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web {
	public class TreeViewNodeCheckBoxStyle : AppearanceStyle {
		public TreeViewNodeCheckBoxStyle()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeCheckBoxStyleMargins"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new Margins Margins {
			get { return base.Margins; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Paddings Paddings {
			get { return base.Paddings; }
		}
	}
	public class TreeViewNodeStyle : AppearanceItemStyle {
		public TreeViewNodeStyle()
			: base() {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Utils.DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
	}
	public class TreeViewNodeTextStyle : AppearanceStyle {
		public TreeViewNodeTextStyle()
			: base() {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Utils.DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
	}
	public class TreeViewStyles : StylesBase {
		const string
			LineStyleName = "Line",
			ElbowStyleName = "Elbow",
			NodeStyleName = "Node",
			NodeTextStyleName = "NodeText",
			NodeImageStyleName = "NodeImage",
			NodeCheckBoxStyleName = "NodeCheckBox",
			NodeCheckBoxFocusedStyleName = "NodeFocusedCheckBox",
			ControlCssClassName = "Control",
			DisabledCssClassName = "Disabled",
			RtlCssClassName = "dxtvRtl",
			SubnodeCssClassName = "dxtv-subnd",
			LineCssClassName = "dxtv-ln",
			ElbowCssClassName = "dxtv-elb",
			ElbowWithoutLineCssClassName = "dxtv-elbNoLn",
			ButtonCssClassName = "dxtv-btn",
			NodeTextCssClassName = "dxtv-ndTxt",
			NodeImageCssClassName = "dxtv-ndImg",
			NodeCssClassName = "dxtv-nd",
			NodeTemplateCssClassName = "dxtv-ndTmpl",
			SystemNodeCheckBoxCssClassName = "dxtv-ndChk",
			HoveredNodeCssClassName = "dxtv-ndHov",
			SelectedNodeCssClassName = "dxtv-ndSel",
			LoadingPanelWithContentCssClassName = "LoadingPanelWithContent";
		protected TreeViewStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		public TreeViewStyles(ASPxTreeView treeView)
			: this((ISkinOwner)treeView) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewStylesElbow"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle Elbow { get { return (AppearanceStyle)GetStyle(ElbowStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewStylesNodeText"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeViewNodeTextStyle NodeText { get { return (TreeViewNodeTextStyle)GetStyle(NodeTextStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewStylesNodeImage"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle NodeImage { get { return (AppearanceStyle)GetStyle(NodeImageStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewStylesNodeCheckBox"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeViewNodeCheckBoxStyle NodeCheckBox { get { return (TreeViewNodeCheckBoxStyle)GetStyle(NodeCheckBoxStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewStylesNodeCheckBoxFocused"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeViewNodeCheckBoxStyle NodeCheckBoxFocused { get { return (TreeViewNodeCheckBoxStyle)GetStyle(NodeCheckBoxFocusedStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewStylesLoadingPanel"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingPanelStyle LoadingPanel {
			get { return base.LoadingPanelInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewStylesDisabled"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DisabledStyle Disabled {
			get { return base.DisabledInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewStylesLink"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinkStyle Link {
			get { return base.LinkInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewStylesNode"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeViewNodeStyle Node { get { return (TreeViewNodeStyle)GetStyle(NodeStyleName); } }
		protected override bool MakeLinkStyleAttributesImportant {
			get { return true; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxtv";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ElbowStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(ElbowStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(NodeTextStyleName, delegate() { return new TreeViewNodeTextStyle(); }));
			list.Add(new StyleInfo(NodeImageStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(NodeCheckBoxStyleName, delegate() { return new TreeViewNodeCheckBoxStyle(); }));
			list.Add(new StyleInfo(NodeCheckBoxFocusedStyleName, delegate() { return new TreeViewNodeCheckBoxStyle(); }));
			list.Add(new StyleInfo(NodeStyleName, delegate() { return new TreeViewNodeStyle(); }));
		}
		protected T GetDefaultStyle<T>(string cssClassName) where T :
			AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = cssClassName;
			return style;
		}
		protected T GetDefaultStyleWithThemePostfix<T>(string cssClassName) where T :
			AppearanceStyleBase, new() {
			return GetDefaultStyle<T>(GetCssClassName(GetCssClassNamePrefix(), cssClassName));
		}
		protected internal override AppearanceStyle GetDefaultControlStyle() {
			AppearanceStyle controlStyle = GetDefaultStyleWithThemePostfix<AppearanceStyle>(ControlCssClassName);
			if(!(Owner as ASPxTreeView).IsRightToLeft())
				return controlStyle;
			AppearanceStyle rtlStyle = GetDefaultStyle<AppearanceStyle>(RtlCssClassName);
			rtlStyle.MergeWith(controlStyle);
			return rtlStyle;
		}
		protected internal override AppearanceStyleBase GetDefaultDisabledStyle() {
			return GetDefaultStyleWithThemePostfix<AppearanceStyleBase>(DisabledCssClassName);
		}
		protected override string GetLoadingPanelCssClassName() {
			return LoadingPanelWithContentCssClassName;
		}
		protected internal virtual AppearanceStyle GetDefaultSubnodeStyle() {
			return GetDefaultStyle<AppearanceStyle>(SubnodeCssClassName);
		}
		protected internal virtual AppearanceStyle GetDefaultLineStyle() {
			return GetDefaultStyle<AppearanceStyle>(LineCssClassName);
		}
		protected internal virtual AppearanceStyle GetDefaultElbowStyle() {
			return GetDefaultStyle<AppearanceStyle>(ElbowCssClassName);
		}
		protected internal virtual AppearanceStyle GetDefaultElbowWithoutLineStyle() {
			return GetDefaultStyle<AppearanceStyle>(ElbowWithoutLineCssClassName);
		}
		protected internal virtual AppearanceStyle GetDefaultButtonStyle() {
			return GetDefaultStyle<AppearanceStyle>(ButtonCssClassName);
		}
		protected internal virtual TreeViewNodeTextStyle GetDefaultNodeTextStyle() {
			return GetDefaultStyle<TreeViewNodeTextStyle>(NodeTextCssClassName);
		}
		protected internal virtual AppearanceStyle GetDefaultNodeImageStyle() {
			return GetDefaultStyle<AppearanceStyle>(NodeImageCssClassName);
		}
		protected internal virtual TreeViewNodeStyle GetDefaultNodeStyle() {
			return GetDefaultStyle<TreeViewNodeStyle>(NodeCssClassName);
		}
		protected internal virtual AppearanceStyle GetDefaultNodeTemplateStyle() {
			return GetDefaultStyle<AppearanceStyle>(NodeTemplateCssClassName);
		}
		protected internal virtual AppearanceSelectedStyle GetDefaultNodeHoverStyle() {
			return GetDefaultStyle<AppearanceSelectedStyle>(HoveredNodeCssClassName);
		}
		protected internal virtual AppearanceSelectedStyle GetDefaultNodeSelectedStyle() {
			return GetDefaultStyle<AppearanceSelectedStyle>(SelectedNodeCssClassName);
		}
		protected internal virtual TreeViewNodeCheckBoxStyle GetSystemNodeCheckBoxStyle() {
			return GetDefaultStyle<TreeViewNodeCheckBoxStyle>(SystemNodeCheckBoxCssClassName);
		}
		protected internal virtual TreeViewNodeCheckBoxStyle GetDefaultNodeCheckBoxStyle() {
			return GetDefaultStyle<TreeViewNodeCheckBoxStyle>(GetCssClassName(string.Empty, InternalCheckboxControl.CheckBoxClassName));
		}
		protected internal virtual TreeViewNodeCheckBoxStyle GetDefaultNodeCheckBoxFocusedStyle() {
			return GetDefaultStyle<TreeViewNodeCheckBoxStyle>(GetCssClassName(string.Empty, InternalCheckboxControl.FocusedCheckBoxClassName));
		}
	}
}
