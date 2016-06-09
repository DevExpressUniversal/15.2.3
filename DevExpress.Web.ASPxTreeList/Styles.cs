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
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web.ASPxTreeList {
	public abstract class TreeListIndentStyleBase : AppearanceStyleBase {
		#region Hide unusable props
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderTop { get { return base.BorderTop; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable]
		public new Color ForeColor { get { return base.ForeColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override DefaultBoolean Wrap { get { return base.Wrap; } set { base.Wrap = value; } }
		#endregion
	}
	public class TreeListTreeLineStyle : TreeListIndentStyleBase {
	}
	public class TreeListIndentStyle : TreeListIndentStyleBase {
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListIndentStylePaddings"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Paddings Paddings { get { return base.Paddings; } }
	}
	public class TreeListNodeStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderTop { get { return base.BorderTop; } }
	}	
	public class TreeListFooterStyle : TreeListNodeStyle { }
	public class TreeListHeaderStyle : AppearanceStyleBase {
		protected internal const string MSTouchDraggableMarkerCssClassName = "dxtlHDR_MSDraggable";
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListHeaderStylePaddings"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
		new public Paddings Paddings { get { return base.Paddings; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListHeaderStyleSortImageSpacing"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit SortImageSpacing { get { return ImageSpacing; } set { ImageSpacing = value; } }
	}
	public class TreeListCellStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
	}
	public class TreeListAlternatingNodeStyle : TreeListNodeStyle {
		IPropertiesOwner owner;
		public TreeListAlternatingNodeStyle() 
			: this(null) {
		}
		public TreeListAlternatingNodeStyle(IPropertiesOwner owner) 
			: base() {
			this.owner = owner;
		}
		protected IPropertiesOwner Owner { get { return owner; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListAlternatingNodeStyleEnabled"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean Enabled {
			get { return (DefaultBoolean)ViewStateUtils.GetEnumProperty(ViewState, "Enabled", DefaultBoolean.Default); }
			set {
				ViewStateUtils.SetEnumProperty(ViewState, "Enabled", DefaultBoolean.Default, value);
				if(Owner != null)
					Owner.Changed(null);
			}
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListAlternatingNodeStyleIsEmpty")]
#endif
		public override bool IsEmpty { get { return base.IsEmpty && Enabled == DefaultBoolean.Default; } }
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			TreeListAlternatingNodeStyle altStyle = style as TreeListAlternatingNodeStyle;
			if(altStyle == null)
				return;
			Enabled = altStyle.Enabled;
		}		
	}
	public class TreeListPagerPanelStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListPagerPanelStyleSpacing"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		new public Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListPagerPanelStylePaddings"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		new public Paddings Paddings { get { return base.Paddings; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap { get { return base.Wrap; } set { base.Wrap = value; } }
	}
	public class TreeListCommandCellStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
	}
	public class TreeListPopupEditFormStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
	}
	public class TreeListStyles : StylesBase {
		internal const string Prefix = "dxtl";
		#region Names
		internal const string
			IndentName = "Indent",
			IndentWithButtonName = "IndentWithButton",
			SelectionCellName = "SelectionCell",
			LineRootName = "LineRoot",
			LineFirstName = "LineFirst",
			LineMiddleName = "LineMiddle",
			LineLastName = "LineLast",
			LineFirstRtlName = "LineFirstRtl",
			LineMiddleRtlName = "LineMiddleRtl",
			LineLastRtlName = "LineLastRtl",			
			HeaderName = "Header",
			NodeName = "Node",
			SelectedNodeName = "SelectedNode",
			FocusedNodeName = "FocusedNode",
			InlineEditNodeName = "InlineEditNode",
			EditFormDisplayNodeName = "EditFormDisplayNode",
			GroupFooterName = "GroupFooter",
			FooterName = "Footer",
			PreviewName = "Preview",
			ErrorName = "Error",
			EditFormName = "EditForm",
			EditFormEditCellName = "EditFormEditCell",
			EditFormColumnCaptionName = "EditFormCaption",
			PopupEditFormName = "PopupEditForm",
			CommandCellName = "CommandCell",
			PagerTopPanelName = "PagerTopPanel",
			PagerBottomPanelName = "PagerBottomPanel",
			CellName = "Cell",
			GroupFooterCellName = "GroupFooterCell",
			FooterCellName = "FooterCell",
			InlineEditCellName = "InlineEditCell",
			CommandButtonName = "CommandButton",
			CustWinName = "CustWin",
			CustWinHeaderName = "CustWinHeader",
			CustWinCloseButtonName = "CustWinCloseButton",
			CustWinContentName = "CustWinContent",
			PopEditFormWindow = "PopupEditFormWindow",
			PopEditFormWindowHeader = "PopupEditFormWindowHeader",
			PopEditFormWindowContent = "PopupEditFormWindowContent",
			PopEditFormWindowCloseButton = "PopupEditFormWindowCloseButton";
		#endregion
		TreeListAlternatingNodeStyle altNode; 
		public TreeListStyles(ISkinOwner owner) 
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesIndent"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListIndentStyle Indent { get { return GetStyle(IndentName) as TreeListIndentStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesIndentWithButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListIndentStyle IndentWithButton { get { return GetStyle(IndentWithButtonName) as TreeListIndentStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesSelectionCell"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListCellStyle SelectionCell { get { return GetStyle(SelectionCellName) as TreeListCellStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesTreeLineRoot"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListTreeLineStyle TreeLineRoot { get { return GetStyle(LineRootName) as TreeListTreeLineStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesTreeLineFirst"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListTreeLineStyle TreeLineFirst { get { return GetStyle(LineFirstName) as TreeListTreeLineStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesTreeLineMiddle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListTreeLineStyle TreeLineMiddle { get { return GetStyle(LineMiddleName) as TreeListTreeLineStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesTreeLineLast"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListTreeLineStyle TreeLineLast { get { return GetStyle(LineLastName) as TreeListTreeLineStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesTreeLineFirstRtl"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListTreeLineStyle TreeLineFirstRtl { get { return GetStyle(LineFirstRtlName) as TreeListTreeLineStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesTreeLineMiddleRtl"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListTreeLineStyle TreeLineMiddleRtl { get { return GetStyle(LineMiddleRtlName) as TreeListTreeLineStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesTreeLineLastRtl"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListTreeLineStyle TreeLineLastRtl { get { return GetStyle(LineLastRtlName) as TreeListTreeLineStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesHeader"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListHeaderStyle Header { get { return GetStyle(HeaderName) as TreeListHeaderStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesNode"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListNodeStyle Node { get { return GetStyle(NodeName) as TreeListNodeStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesAlternatingNode"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListAlternatingNodeStyle AlternatingNode {
			get {
				if(altNode == null)
					altNode = new TreeListAlternatingNodeStyle(Owner);
				return altNode;
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesSelectedNode"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListNodeStyle SelectedNode { get { return GetStyle(SelectedNodeName) as TreeListNodeStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesFocusedNode"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListNodeStyle FocusedNode { get { return GetStyle(FocusedNodeName) as TreeListNodeStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesInlineEditNode"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListNodeStyle InlineEditNode { get { return GetStyle(InlineEditNodeName) as TreeListNodeStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesEditFormDisplayNode"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListNodeStyle EditFormDisplayNode { get { return GetStyle(EditFormDisplayNodeName) as TreeListNodeStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesGroupFooter"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListFooterStyle GroupFooter { get { return GetStyle(GroupFooterName) as TreeListFooterStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesFooter"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListFooterStyle Footer { get { return GetStyle(FooterName) as TreeListFooterStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesPagerTopPanel"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListPagerPanelStyle PagerTopPanel { get { return GetStyle(PagerTopPanelName) as TreeListPagerPanelStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesPagerBottomPanel"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListPagerPanelStyle PagerBottomPanel { get { return GetStyle(PagerBottomPanelName) as TreeListPagerPanelStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesCell"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListCellStyle Cell { get { return GetStyle(CellName) as TreeListCellStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesGroupFooterCell"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListCellStyle GroupFooterCell { get { return GetStyle(GroupFooterCellName) as TreeListCellStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesFooterCell"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListCellStyle FooterCell { get { return GetStyle(FooterCellName) as TreeListCellStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesPreview"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListCellStyle Preview { get { return GetStyle(PreviewName) as TreeListCellStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesError"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListCellStyle Error { get { return GetStyle(ErrorName) as TreeListCellStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesEditForm"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListCellStyle EditForm { get { return GetStyle(EditFormName) as TreeListCellStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesInlineEditCell"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListCellStyle InlineEditCell { get { return GetStyle(InlineEditCellName) as TreeListCellStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesEditFormEditCell"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListCellStyle EditFormEditCell { get { return GetStyle(EditFormEditCellName) as TreeListCellStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesEditFormColumnCaption"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListCellStyle EditFormColumnCaption { get { return GetStyle(EditFormColumnCaptionName) as TreeListCellStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesCommandCell"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListCommandCellStyle CommandCell { get { return GetStyle(CommandCellName) as TreeListCommandCellStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesCommandButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle CommandButton { get { return GetStyle(CommandButtonName) as AppearanceStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesCustomizationWindow"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle CustomizationWindow { get { return GetStyle(CustWinName) as AppearanceStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesCustomizationWindowHeader"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowStyle CustomizationWindowHeader { get { return GetStyle(CustWinHeaderName) as PopupWindowStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesCustomizationWindowCloseButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowButtonStyle CustomizationWindowCloseButton { get { return GetStyle(CustWinCloseButtonName) as PopupWindowButtonStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesCustomizationWindowContent"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowContentStyle CustomizationWindowContent { get { return GetStyle(CustWinContentName) as PopupWindowContentStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesPopupEditForm"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListPopupEditFormStyle PopupEditForm { get { return GetStyle(PopupEditFormName) as TreeListPopupEditFormStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesPopupEditFormWindow"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle PopupEditFormWindow { get { return GetStyle(PopEditFormWindow) as AppearanceStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesPopupEditFormWindowHeader"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowStyle PopupEditFormWindowHeader { get { return GetStyle(PopEditFormWindowHeader) as PopupWindowStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesPopupEditFormWindowContent"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowContentStyle PopupEditFormWindowContent { get { return GetStyle(PopEditFormWindowContent) as PopupWindowContentStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesPopupEditFormWindowCloseButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowButtonStyle PopupEditFormWindowCloseButton { get { return GetStyle(PopEditFormWindowCloseButton) as PopupWindowButtonStyle; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesLoadingPanel"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingPanelStyle LoadingPanel { get { return base.LoadingPanelInternal; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListStylesLoadingDiv"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingDivStyle LoadingDiv { get { return base.LoadingDivInternal; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(IndentName, delegate() { return new TreeListIndentStyle(); }));
			list.Add(new StyleInfo(IndentWithButtonName, delegate() { return new TreeListIndentStyle(); }));
			list.Add(new StyleInfo(SelectionCellName, delegate() { return new TreeListCellStyle(); }));
			list.Add(new StyleInfo(LineRootName, delegate() { return new TreeListTreeLineStyle(); }));
			list.Add(new StyleInfo(LineFirstName, delegate() { return new TreeListTreeLineStyle(); }));
			list.Add(new StyleInfo(LineMiddleName, delegate() { return new TreeListTreeLineStyle(); }));
			list.Add(new StyleInfo(LineLastName, delegate() { return new TreeListTreeLineStyle(); }));
			list.Add(new StyleInfo(LineFirstRtlName, delegate() { return new TreeListTreeLineStyle(); }));
			list.Add(new StyleInfo(LineMiddleRtlName, delegate() { return new TreeListTreeLineStyle(); }));
			list.Add(new StyleInfo(LineLastRtlName, delegate() { return new TreeListTreeLineStyle(); }));			
			list.Add(new StyleInfo(HeaderName, delegate() { return new TreeListHeaderStyle(); }));
			list.Add(new StyleInfo(NodeName, delegate() { return new TreeListNodeStyle(); }));
			list.Add(new StyleInfo(SelectedNodeName, delegate() { return new TreeListNodeStyle(); }));
			list.Add(new StyleInfo(FocusedNodeName, delegate() { return new TreeListNodeStyle(); }));
			list.Add(new StyleInfo(InlineEditNodeName, delegate() { return new TreeListNodeStyle(); }));
			list.Add(new StyleInfo(EditFormDisplayNodeName, delegate() { return new TreeListNodeStyle(); }));
			list.Add(new StyleInfo(GroupFooterName, delegate() { return new TreeListFooterStyle(); }));
			list.Add(new StyleInfo(FooterName, delegate() { return new TreeListFooterStyle(); }));
			list.Add(new StyleInfo(PreviewName, delegate() { return new TreeListCellStyle(); }));
			list.Add(new StyleInfo(ErrorName, delegate() { return new TreeListCellStyle(); }));
			list.Add(new StyleInfo(EditFormName, delegate() { return new TreeListCellStyle(); }));
			list.Add(new StyleInfo(EditFormEditCellName, delegate() { return new TreeListCellStyle(); }));
			list.Add(new StyleInfo(EditFormColumnCaptionName, delegate() { return new TreeListCellStyle(); }));
			list.Add(new StyleInfo(CommandCellName, delegate() { return new TreeListCommandCellStyle(); }));
			list.Add(new StyleInfo(PagerTopPanelName, delegate() { return new TreeListPagerPanelStyle(); }));
			list.Add(new StyleInfo(PagerBottomPanelName, delegate() { return new TreeListPagerPanelStyle(); }));
			list.Add(new StyleInfo(PopupEditFormName, delegate() { return new TreeListPopupEditFormStyle(); }));
			list.Add(new StyleInfo(CellName, delegate() { return new TreeListCellStyle(); }));
			list.Add(new StyleInfo(GroupFooterCellName, delegate() { return new TreeListCellStyle(); }));
			list.Add(new StyleInfo(FooterCellName, delegate() { return new TreeListCellStyle(); }));
			list.Add(new StyleInfo(InlineEditCellName, delegate() { return new TreeListCellStyle(); }));
			list.Add(new StyleInfo(CommandButtonName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(CustWinName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(CustWinHeaderName, delegate() { return new PopupWindowStyle(); }));
			list.Add(new StyleInfo(CustWinCloseButtonName, delegate() { return new PopupWindowButtonStyle(); }));
			list.Add(new StyleInfo(CustWinContentName, delegate() { return new PopupWindowContentStyle(); }));
			list.Add(new StyleInfo(PopEditFormWindow, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(PopEditFormWindowHeader, delegate() { return new PopupWindowStyle(); }));
			list.Add(new StyleInfo(PopEditFormWindowContent, delegate() { return new PopupWindowContentStyle(); }));
			list.Add(new StyleInfo(PopEditFormWindowCloseButton, delegate() { return new PopupWindowButtonStyle(); }));
		}
		internal TreeListIndentStyle MergeIndentStyle() {
			return MergeStyleCore<TreeListIndentStyle>(IndentName, Indent);
		}
		internal TreeListIndentStyle MergeIndentWithButtonStyle() {
			return MergeStyleCore<TreeListIndentStyle>(IndentWithButtonName, IndentWithButton);
		}
		internal TreeListCellStyle MergeSelectionCellStyle() {
			return MergeStyleCore<TreeListCellStyle>(SelectionCellName, SelectionCell);
		}
		internal TreeListTreeLineStyle MergeTreeLineFirstStyle() {
			if(SkinOwner.IsRightToLeft())
				return MergeStyleCore<TreeListTreeLineStyle>(LineFirstRtlName, TreeLineFirstRtl);
			return MergeStyleCore<TreeListTreeLineStyle>(LineFirstName, TreeLineFirst);
		}
		internal TreeListTreeLineStyle MergeTreeLineMiddleStyle() {
			if(SkinOwner.IsRightToLeft())
				return MergeStyleCore<TreeListTreeLineStyle>(LineMiddleRtlName, TreeLineMiddleRtl);
			return MergeStyleCore<TreeListTreeLineStyle>(LineMiddleName, TreeLineMiddle);
		}
		internal TreeListTreeLineStyle MergeTreeLineLastStyle() {
			if(SkinOwner.IsRightToLeft())
				return MergeStyleCore<TreeListTreeLineStyle>(LineLastRtlName, TreeLineLastRtl);
			return MergeStyleCore<TreeListTreeLineStyle>(LineLastName, TreeLineLast);		
		}
		internal TreeListTreeLineStyle MergeTreeLineRootStyle() {
			return MergeStyleCore<TreeListTreeLineStyle>(LineRootName, TreeLineRoot);
		}
		internal TreeListHeaderStyle MergeHeaderStyle() {
			return MergeStyleCore<TreeListHeaderStyle>(HeaderName, Header);
		}
		internal TreeListNodeStyle MergeNodeStyle() {
			return MergeStyleCore<TreeListNodeStyle>(NodeName, Node);
		}
		internal TreeListAlternatingNodeStyle MergeAlternatingNodeStyle() {
			return MergeStyleCore<TreeListAlternatingNodeStyle>("AltNode", AlternatingNode);
		}
		internal TreeListNodeStyle MergeSelectedNodeStyle() {
			return MergeStyleCore<TreeListNodeStyle>(SelectedNodeName, SelectedNode);
		}
		internal TreeListNodeStyle MergeFocusedNodeStyle() {
			return MergeStyleCore<TreeListNodeStyle>(FocusedNodeName, FocusedNode);
		}
		internal TreeListNodeStyle MergeInlineEditNodeStyle() {
			return MergeStyleCore<TreeListNodeStyle>(InlineEditNodeName, InlineEditNode);
		}
		internal TreeListNodeStyle MergeEditFormDisplayNodeStyle() {
			return MergeStyleCore<TreeListNodeStyle>(EditFormDisplayNodeName, EditFormDisplayNode);
		}
		internal TreeListCellStyle MergePreviewStyle() {
			return MergeStyleCore<TreeListCellStyle>(PreviewName, Preview);
		}
		internal TreeListCellStyle MergeErrorStyle() {
			return MergeStyleCore<TreeListCellStyle>(ErrorName, Error);
		}
		internal TreeListCellStyle MergeEditFormStyle() {
			return MergeStyleCore<TreeListCellStyle>(EditFormName, EditForm);
		}
		internal TreeListCellStyle MergeEditFormEditCellStyle() {
			return MergeStyleCore<TreeListCellStyle>(EditFormEditCellName, EditFormEditCell);
		}
		internal TreeListCellStyle MergeEditFormCaptionStyle() {
			return MergeStyleCore<TreeListCellStyle>(EditFormColumnCaptionName, EditFormColumnCaption);
		}
		internal TreeListCommandCellStyle MergeCommandCellStyle() {
			return MergeStyleCore<TreeListCommandCellStyle>(CommandCellName, CommandCell);
		}
		internal TreeListFooterStyle MergeGroupFooterStyle() {
			return MergeStyleCore<TreeListFooterStyle>(GroupFooterName, GroupFooter);
		}
		internal TreeListFooterStyle MergeFooterStyle() {
			return MergeStyleCore<TreeListFooterStyle>(FooterName, Footer);
		}
		internal TreeListPagerPanelStyle MergePagerTopPanelStyle() {
			return MergeStyleCore<TreeListPagerPanelStyle>(PagerTopPanelName, PagerTopPanel);
		}
		internal TreeListPagerPanelStyle MergePagerBottomPanelStyle() {
			return MergeStyleCore<TreeListPagerPanelStyle>(PagerBottomPanelName, PagerBottomPanel);
		}
		internal TreeListPopupEditFormStyle MergePopupEditFormStyle() {
			return MergeStyleCore<TreeListPopupEditFormStyle>(PopupEditFormName, PopupEditForm);
		}
		protected internal override string GetCssClassNamePrefix() { return Prefix; }
		internal static void AppendDefaultClassName(params WebControl[] controls) {
			foreach(WebControl control in controls)
				RenderUtils.AppendDefaultDXClassName(control, Prefix);
		}
		T MergeStyleCore<T>(string name, T userStyle) where T : AppearanceStyleBase, new() {
			T style = CreateStyleCopyByName<T>(name);
			style.CopyFrom(userStyle);
			return style;
		}
	}
}
