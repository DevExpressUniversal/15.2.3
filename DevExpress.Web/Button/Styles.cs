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
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class ButtonControlStyle : ButtonStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonControlStyleWidth"),
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
	DevExpressWebLocalizedDescription("ButtonControlStyleCheckedStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual AppearanceSelectedStyle CheckedStyle {
			get { return base.SelectedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonControlStyleHoverStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonControlStylePressedStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle PressedStyle {
			get { return base.PressedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonControlStyleSpacing"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Spacing
		{
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonControlStyleImageSpacing"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit ImageSpacing
		{
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
	}
	public class ButtonControlStyles : StylesBase {
		public const string StyleStyleName = "Style";
		public const string FocusRectStyleName = "FocusRect";
		internal const string ButtonSystemClassName = "dxbButtonSys";
		internal const string ButtonTableSystemClassName = "dxbTSys";
		public ButtonControlStyles(ISkinOwner button)
			: base(button) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonControlStylesNative"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatEnable]
		public new bool Native {
			get { return base.Native; }
			set { base.Native = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonControlStylesStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonControlStyle Style {
			get { return (ButtonControlStyle)GetStyle(StyleStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonControlStylesFocusRectStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public AppearanceStyle FocusRectStyle {
			get { return (AppearanceStyle)GetStyle(FocusRectStyleName); }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxb";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(StyleStyleName, delegate() { return new ButtonControlStyle(); }));
			list.Add(new StyleInfo(FocusRectStyleName, delegate() { return new AppearanceStyle(); } ));
		}
		protected internal AppearanceStyle GetDefaultButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ButtonStyle"));
			style.ImageSpacing = 4;
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultButtonCheckedStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("ButtonCheckedStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultButtonPressedStyle(bool isLink) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			if(!isLink)
				style.CopyFrom(CreateStyleByName("ButtonPressedStyle"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultButtonHoverStyle(bool isLink) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			if(!isLink)
				style.CopyFrom(CreateStyleByName("ButtonHoverStyle"));
			return style;
		}
		protected internal virtual AppearanceStyleBase GetDefaultButtonTextCellStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CssClass = GetCssClassNamePrefix();
			return style;
		}
		protected internal virtual AppearanceStyleBase GetDefaultButtonContentDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CssClass = GetCssClassNamePrefix();
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultButtonContentDivFocusedStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CssClass = GetCssClassNamePrefix() + "f";
			return style;
		}
	}
}
