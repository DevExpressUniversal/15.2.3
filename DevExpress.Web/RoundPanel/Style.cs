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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class CollapseButtonStyle : ButtonStyle {
		private const string defaultHoverCssClass = "dxrp-collapseBtnHover";
		private const string defaultPressedCssClass = "dxrp-collapseBtnPressed";
		private const string defaultCheckedCssClass = "dxrp-collapseBtnChecked";
		protected internal const string defaultDisabledCssClass = "dxrp-collapseBtnDisabled";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new AppearanceSelectedStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CollapseButtonStyleCheckedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual AppearanceSelectedStyle CheckedStyle {
			get { return base.SelectedStyle; }
		}
		protected internal virtual AppearanceStyleBase GetDefaultHoverStyle() {
			return GetDefaultStyle<AppearanceStyleBase>(defaultHoverCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultPressedStyle() {
			return GetDefaultStyle<AppearanceStyleBase>(defaultPressedCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultCheckedStyle() {
			return GetDefaultStyle<AppearanceStyleBase>(defaultCheckedCssClass);
		}
		protected internal DisabledStyle GetDisabledStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CssClass = defaultDisabledCssClass;
			return style;
		}
		protected T GetDefaultStyle<T>(string cssClassName) where T : AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = cssClassName;
			return style;
		}
	}
	public class HeaderStyleBase : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class HeaderStyle : HeaderStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("HeaderStyleHeight"),
#endif
		Browsable(true), NotifyParentProperty(true), PersistenceMode(PersistenceMode.Attribute),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DefaultValue(typeof(Unit), "")]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
	}
	public class GroupBoxHeaderStyle : HeaderStyleBase {
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
	}
	public class RoundPanelStyles : StylesBase {
		public const string HeaderStyleName = "Header";
		public const string GroupBoxHeaderStyleName = "GroupBoxHeader";
		public const string CollapseButtonStyleName = "CollapseButton";
		public RoundPanelStyles(ISkinOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelStylesHeader"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public HeaderStyle Header {
			get { return (HeaderStyle)GetStyle(HeaderStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelStylesCollapseButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public CollapseButtonStyle CollapseButton {
			get { return (CollapseButtonStyle)GetStyle(CollapseButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RoundPanelStylesGroupBoxHeader"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public GroupBoxHeaderStyle GroupBoxHeader {
			get { return (GroupBoxHeaderStyle)GetStyle(GroupBoxHeaderStyleName); }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxrp";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(HeaderStyleName, delegate() { return new HeaderStyle(); }));
			list.Add(new StyleInfo(GroupBoxHeaderStyleName, delegate() { return new GroupBoxHeaderStyle(); }));
			list.Add(new StyleInfo(CollapseButtonStyleName, delegate() { return new CollapseButtonStyle(); }));
		}
		protected internal virtual AppearanceStyle GetDefaultControlStyle(bool showHeader, bool isGroupBox) {
			AppearanceStyle style = new AppearanceStyle();
			string styleName = string.Format("Control{0}Style", showHeader && isGroupBox ? "GB" : string.Empty);
			style.CopyFrom(CreateStyleByName(styleName));
			return style;
		}
		protected internal HeaderStyle GetDefaultHeaderStyle() {
			HeaderStyle style = new HeaderStyle();
			style.CopyFrom(CreateStyleByName("HeaderStyle"));
			return style;
		}
		protected internal AppearanceStyle GetWithoutHeaderControlStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("WithoutHeader"));
			return style;
		}
		protected override bool HideLoadingPanelBorder() {
			return true;
		}
	}
}
