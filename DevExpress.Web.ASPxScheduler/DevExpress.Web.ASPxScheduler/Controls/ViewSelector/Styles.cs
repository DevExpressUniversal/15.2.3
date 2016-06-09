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
using DevExpress.Utils;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler {
	public class ViewSelectorStyles : StylesBase {
		public const string ButtonStyleName = "Button";
		public const string ControlStyleStyleName = "ControlStyle";
		public const string ViewSelectorIndentStyleName = "ViewSelectorIndent";
		public ViewSelectorStyles(ISkinOwner owner)
			: base(owner) {
		}
		#region Properties
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ButtonControlStyle Button {
			get { return (ButtonControlStyle)GetStyle(ButtonStyleName); }
		}
		[NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit ButtonSpacing {
			get { return GetUnitProperty("ButtonSpacing", Unit.Empty); }
			set { SetUnitProperty("ButtonSpacing", Unit.Empty, value); }
		}
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ViewSelectorControlStyle ControlStyle {
			get { return (ViewSelectorControlStyle)GetStyle(ControlStyleStyleName); }
		}
		#endregion
		protected override string GetCssClassNamePrefix() {
			return "dxsc";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ButtonStyleName, delegate() { return new ButtonControlStyle(); }));
			list.Add(new StyleInfo(ControlStyleStyleName, delegate() { return new ViewSelectorControlStyle(); } ));
		}
		protected internal new AppearanceStyleBase GetDefaultControlStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ViewSelector));
			return style;
		}
		protected internal ButtonControlStyle GetButtonStyle() {
			ButtonControlStyle style = new ButtonControlStyle();
			style.CopyFrom(Button);
			return style;
		}
		protected internal ViewSelectorControlStyle GetControlStyle() {
			ViewSelectorControlStyle style = new ViewSelectorControlStyle();
			style.CopyFrom(GetDefaultControlStyle());
			style.CopyFrom(ControlStyle);
			return style;
		}
		protected internal Unit GetButtonSpacing() {
			return ButtonSpacing.IsEmpty ? 0 : ButtonSpacing;
		}
		public override void Reset() {
			base.Reset();
			ButtonSpacing = Unit.Empty;
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			ViewSelectorStyles src = source as ViewSelectorStyles;
			if (src != null) {
				ButtonSpacing = src.ButtonSpacing;
			}
		}
		public AppearanceStyleBase CreateDefaultViewSelectorIndentStyle() {
			return CreateStyleByName(ViewSelectorIndentStyleName);
		}
	}
	public class ViewSelectorControlStyle : AppearanceStyle {
		#region Hide non-used props
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { base.Wrap = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		#endregion
	}
}
