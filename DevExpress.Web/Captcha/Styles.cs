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
using DevExpress.Web.Internal;
using System.Web.UI;
using System.ComponentModel;
using DevExpress.Web;
using System.Web.UI.WebControls;
using DevExpress.Utils;
namespace DevExpress.Web.Captcha {
	public class RefreshButtonStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
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
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class CaptchaTextBoxStyle : EditStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaTextBoxStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), AutoFormatEnable]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
	}
	public class CaptchaStyles : StylesBase {
		const string CssClassNamePrefix = "dxca";
		const string RefreshButtonStyleName = "RefreshButton";
		const string DisabledRefreshButtonStyleName = "DisabledRefreshButton";
		const string NullTextStyleName = "NullText";
		const string TextBoxStyleName = "TextBox";
		public CaptchaStyles(ASPxCaptcha owner)
			: base(owner) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected ASPxCaptcha Captcha {
			get { return (ASPxCaptcha)Owner; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return CssClassNamePrefix;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaStylesRefreshButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RefreshButtonStyle RefreshButton
		{
			get { return (RefreshButtonStyle)GetStyle(RefreshButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaStylesDisabledRefreshButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RefreshButtonStyle DisabledRefreshButton
		{
			get { return (RefreshButtonStyle)GetStyle(DisabledRefreshButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaStylesNullText"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorDecorationStyle NullText
		{
			get { return (EditorDecorationStyle)GetStyle(NullTextStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaStylesTextBox"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CaptchaTextBoxStyle TextBox
		{
			get { return (CaptchaTextBoxStyle)GetStyle(TextBoxStyleName); }
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(RefreshButtonStyleName, delegate() { return new RefreshButtonStyle(); }));
			list.Add(new StyleInfo(DisabledRefreshButtonStyleName, delegate() { return new RefreshButtonStyle(); }));
			list.Add(new StyleInfo(NullTextStyleName, delegate() { return new EditorDecorationStyle(); }));
			list.Add(new StyleInfo(TextBoxStyleName, delegate() { return new CaptchaTextBoxStyle(); }));
		}
		protected internal virtual RefreshButtonStyle GetDefaultRefreshButtonStyle() {
			RefreshButtonStyle style = new RefreshButtonStyle();
			style.CopyFrom(CreateStyleByName("RefreshButtonStyle"));
			style.ImageSpacing = 4;
			return style;
		}
		protected internal virtual RefreshButtonStyle GetDefaultDisabledRefreshButtonStyle() {
			RefreshButtonStyle style = new RefreshButtonStyle();
			style.CopyFrom(CreateStyleByName("DisabledRefreshButtonStyle"));
			style.ImageSpacing = 4;
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultRefreshButtonCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("RefreshButtonCellStyle"));
			style.Paddings.Assign(GetCellPaddings(Captcha.RefreshButton.Position));
			if (Captcha.RefreshButton.Position == ControlPosition.Left ||
				Captcha.RefreshButton.Position == ControlPosition.Right) {
				style.VerticalAlign = VerticalAlign.Bottom;
			}
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultTextBoxCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("TextBoxCellStyle"));
			style.Paddings.Assign(GetCellPaddings(Captcha.TextBox.Position));
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultTextBoxCellNoIndentStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("TextBoxCellNoIndentStyle"));
			style.Paddings.Assign(GetCellPaddings(Captcha.TextBox.Position));
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultTextBoxLabelStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("TextBoxLabelStyle"));
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultRefreshButtonTextStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("RefreshButtonTextStyle"));
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultDisabledRefreshButtonTextStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("DisabledRefreshButtonTextStyle"));
			return style;
		}
		protected Paddings CreateVerticalPaddings(Unit paddingTop, Unit paddingBottom) {
			Paddings paddings = new Paddings();
			paddings.PaddingLeft = Unit.Empty;
			paddings.PaddingRight = Unit.Empty;
			paddings.PaddingTop = paddingTop;
			paddings.PaddingBottom = paddingBottom;
			return paddings;
		}
		protected Paddings GetCellPaddings(ControlPosition controlPosition) {
			if (controlPosition == ControlPosition.Left && !SkinOwner.IsRightToLeft() || controlPosition == ControlPosition.Right && SkinOwner.IsRightToLeft())
				return new Paddings(0, 0, 16, 0);
			if (controlPosition == ControlPosition.Right && !SkinOwner.IsRightToLeft() || controlPosition == ControlPosition.Left && SkinOwner.IsRightToLeft())
				return new Paddings(16, 0, 0, 0);
			if (RenderUtils.Browser.IsIE)
				return CreateVerticalPaddings(5, 5);
			return CreateVerticalPaddings(3, 4);
		}
	}
}
