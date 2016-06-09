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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class ExpandBarStyle : AppearanceStyleBase {
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public virtual new Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public virtual new Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
	}
	public class ExpandButtonStyle : AppearanceItemStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.Spacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ExpandButtonStylePressedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual AppearanceSelectedStyle PressedStyle {
			get { return base.PressedStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
	}
	public class PanelStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelStylePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Paddings Paddings {
			get { return base.Paddings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
	}
	public class PanelStyles : StylesBase {
		public const string ExpandBarStyleName = "ExpandBar";
		public const string ExpandedExpandBarStyleName = "ExpandedExpandBar";
		public const string ExpandButtonStyleName = "ExpandButton";
		public const string PanelStyleName = "Panel";
		public const string ExpandedPanelStyleName = "ExpandedPanel";
		internal const string
			ExpandBarCssMarker = "dxpnl-bar",
			ExpandBarTemplateContainerCssMarker = "dxpnl-bar-tmpl",
			ExpandButtonCssMarker = "dxpnl-btn",
			ExpandButtonHoverCssMarker = "dxpnl-btnHover",
			ExpandButtonPressedCssMarker = "dxpnl-btnPressed",
			ExpandButtonSelectedCssMarker = "dxpnl-btnSelected",
			FixedPositionCssMarker = "dxpnl-edge",
			TopFixedPositionCssMarker = "t",
			BottomFixedPositionCssMarker = "b",
			LeftFixedPositionCssMarker = "l",
			RightFixedPositionCssMarker = "r",
			NearButtonPositionCssMarker = "dxpnl-np",
			FarButtonPositionCssMarker = "dxpnl-fp",
			CenterButtonPositionCssMarker = "dxpnl-cp",
			CollapsibleCssMarker = "dxpnl-collapsible",
			ExpandedCssMarker = "dxpnl-expanded",
			ExpandedTemplateContainerCssMarker = "dxpnl-expanded-tmpl",
			ScrollableContentContainerCssMarker = "dxpnl-scc",
			AnimationContentContainerCssMarker = "dxpnl-acc",
			ContentContainerCssMarker = "dxpnl-cc";
		public PanelStyles(ASPxCollapsiblePanel panel)
			: base(panel) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelStylesExpandBar"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, 
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExpandBarStyle ExpandBar {
			get { return (ExpandBarStyle)GetStyle(ExpandBarStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelStylesExpandBar"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExpandBarStyle ExpandedExpandBar {
			get { return (ExpandBarStyle)GetStyle(ExpandedExpandBarStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelStylesExpandButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, 
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExpandButtonStyle ExpandButton {
			get { return (ExpandButtonStyle)GetStyle(ExpandButtonStyleName); }
		}
		[
		NotifyParentProperty(true), AutoFormatEnable, 
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelStyle Panel {
			get { return (PanelStyle)GetStyle(PanelStyleName); }
		}
		[
		NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelStyle ExpandedPanel {
			get { return (PanelStyle)GetStyle(ExpandedPanelStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelStylesDisabled"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DisabledStyle Disabled {
			get { return base.DisabledInternal; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxpnl";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ExpandBarStyleName, delegate() { return new ExpandBarStyle(); }));
			list.Add(new StyleInfo(ExpandedExpandBarStyleName, delegate() { return new ExpandBarStyle(); }));
			list.Add(new StyleInfo(ExpandButtonStyleName, delegate() { return new ExpandButtonStyle(); }));
			list.Add(new StyleInfo(PanelStyleName, delegate() { return new PanelStyle(); }));
			list.Add(new StyleInfo(ExpandedPanelStyleName, delegate() { return new PanelStyle(); }));
		}
		protected internal Style GetDefaultExpandedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CssClass = ExpandedCssMarker;
			return style;
		}
		protected internal Style GetDefaultExpandButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CssClass = ExpandButtonCssMarker;
			return style;
		}
		protected internal Style GetDefaultExpandButtonHoverStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CssClass = ExpandButtonHoverCssMarker;
			return style;
		}
		protected internal Style GetDefaultExpandButtonPressedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CssClass = ExpandButtonPressedCssMarker;
			return style;
		}
		protected internal Style GetDefaultExpandButtonSelectedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CssClass = ExpandButtonSelectedCssMarker;
			return style;
		}
	}
}
