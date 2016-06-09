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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	public class ToolbarStyle : MenuStyle {
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
	public class ToolbarItemStyle : DevExpress.Web.MenuItemStyle {
	}
	public class BarDockStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("BarDockStyleToolbarSpacing"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), Localizable(false),
		AutoFormatEnable, AutoFormatUrlProperty]
		public Unit ToolbarSpacing {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "ToolbarSpacing", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ToolbarSpacing");
				ViewStateUtils.SetUnitProperty(ViewState, "ToolbarSpacing", Unit.Empty, value);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return Unit.Empty; }
			set { }
		}
		public override void CopyFrom(Style style) {
			if ((style != null) && !style.IsEmpty) {
				base.CopyFrom(style);
				BarDockStyle barDockStyle = style as BarDockStyle;
				if(barDockStyle != null) {
					ToolbarSpacing = barDockStyle.ToolbarSpacing;
				}
			}
		}
		public override void MergeWith(Style style) {
			if ((style != null) && !style.IsEmpty) {
				base.MergeWith(style);
				BarDockStyle barDockStyle = style as BarDockStyle;
				if(barDockStyle != null) {
					if(barDockStyle.ToolbarSpacing != Unit.Empty)
						ToolbarSpacing = barDockStyle.ToolbarSpacing;
				}
			}
		}
		public override void Reset() {
			base.Reset();
			ToolbarSpacing = Unit.Empty;
		}
#if !SL
	[DevExpressWebASPxHtmlEditorLocalizedDescription("BarDockStyleIsEmpty")]
#endif
		public override bool IsEmpty {
			get {
				return base.IsEmpty && ToolbarSpacing.IsEmpty;
			}
		}
	}
	public class ToolbarsStyles : StylesBase {
		public const string BarDockStyleName = "BarDockControl";
		public const string ToolbarStyleName = "Toolbar";
		public const string ToolbarItemStyleName = "ToolbarItem";
		HtmlEditorRibbonStyles stylesRibbon;
		public ToolbarsStyles(ISkinOwner owner)
			: base(owner) {
				this.stylesRibbon = new HtmlEditorRibbonStyles(owner);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarsStylesBarDockControl"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public BarDockStyle BarDockControl {
			get { return (BarDockStyle)GetStyle(BarDockStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarsStylesToolbar"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public ToolbarStyle Toolbar {
			get { return (ToolbarStyle)GetStyle(ToolbarStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarsStylesToolbarItem"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public ToolbarItemStyle ToolbarItem {
			get { return (ToolbarItemStyle)GetStyle(ToolbarItemStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarsStylesRibbon"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public HtmlEditorRibbonStyles Ribbon { get { return stylesRibbon; } }
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
			ToolbarsStyles barDockStyles = source as ToolbarsStyles;
			if(barDockStyles != null){
				BarDockControl.CopyFrom(barDockStyles.BarDockControl as Style);
				Toolbar.CopyFrom(barDockStyles.Toolbar as Style);
				ToolbarItem.CopyFrom(barDockStyles.ToolbarItem as Style);
				Ribbon.CopyFrom(barDockStyles.Ribbon);
			}
			base.CopyFrom(source);
		}
		public override void Reset() {
			BarDockControl.Reset();
			Toolbar.Reset();
			ToolbarItem.Reset();
			base.Reset();
		}
		protected override string GetCssClassNamePrefix() {
			return MenuStyles.ToolbarCssClassPrefix;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(BarDockStyleName, delegate() { return new BarDockStyle(); }));
			list.Add(new StyleInfo(ToolbarStyleName, delegate() { return new ToolbarStyle(); }));
			list.Add(new StyleInfo(ToolbarItemStyleName, delegate() { return new ToolbarItemStyle(); }));
		}
		protected internal BarDockStyle GetDefaultBarDockStyle() {
			BarDockStyle style = new BarDockStyle();
			style.CopyFrom(CreateStyleByName("ControlStyle"));
			return style;
		}
		protected internal BarDockStyle GetRibbonBarDockStyle() {
			BarDockStyle style = new BarDockStyle();
			style.CopyFrom(CreateStyleByName("rStyle"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultToolbarSpacingStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("SpacingStyle"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultComboBoxToolbarItemStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CssClass = MenuStyles.ToolbarComboBoxCssClass;
			return style;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Ribbon });
		}
	}
	public class CustomCssItemPreviewStyle : AppearanceStyle {
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
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return Unit.Empty; }
			set { }
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public class ToolbarColorButtonStyles : StylesBase {
		public ToolbarColorButtonStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override string GetCssClassNamePrefix() {
			return "dxtcb";
		}
		protected internal AppearanceStyle GetDefaultColorDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ColorDivStyle"));
			return style;
		}
	}
	public class CustomDropDownStyles : ItemPickerStyles {
		public CustomDropDownStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected internal AppearanceStyle GetDefaultItemPickerTableStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(GetControlClassName() + "_T"));
			return style;
		}
	}
	public class ToolbarCustomDropDownButtonStyles : StylesBase {
		public ToolbarCustomDropDownButtonStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override string GetCssClassNamePrefix() {
			return "dxhetip";
		}
	}
}
