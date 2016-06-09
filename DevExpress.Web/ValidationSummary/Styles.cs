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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class ValidationSummaryErrorStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSummaryErrorStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), AutoFormatEnable]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		#region Hidden AppearanceStyle's Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new System.Drawing.Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new FontInfo Font {
			get { return base.Font; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new System.Drawing.Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		#endregion
	}
	public class ValidationSummaryHeaderStyle : AppearanceStyle {
		#region Hidden AppearanceStyle's Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		#endregion
	}
	public class ValidationSummaryStyles : StylesBase {
		private const string ValidationSummaryStyleName = "ValidationSummary";
		private const string HeaderTableStyleName = "HT";
		private const string HeaderStyleName = "H";
		private const string RootCellStyleName = "RC";
		private const string ErrorStyleName = "E";
		private const string HyperlinkStyleName = "HL";
		private const string TableErrorContainerStyleName = "T";
		private const string ErrorTextCellStyleName = "ETC";
		private const string ListErrorContainerStyleName = "L";
		public ValidationSummaryStyles(ISkinOwner validationSummary)
			: base(validationSummary) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("ValidationSummaryStylesError")]
#endif
		public ValidationSummaryErrorStyle Error {
			get { return (ValidationSummaryErrorStyle)GetStyle(ErrorStyleName); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ValidationSummaryStylesHeader")]
#endif
		public ValidationSummaryHeaderStyle Header {
			get { return (ValidationSummaryHeaderStyle)GetStyle(HeaderStyleName); }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxvs";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(HeaderStyleName, delegate() { return new ValidationSummaryHeaderStyle(); }));
			list.Add(new StyleInfo(ErrorStyleName, delegate() { return new ValidationSummaryErrorStyle(); }));
		}
		private AppearanceStyleBase CreateDefaultStyleByName(string styleName) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName(styleName));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultValidationSummaryStyle() {
			return CreateDefaultStyleByName(ValidationSummaryStyleName);
		}
		protected internal AppearanceStyleBase GetDefaultHeaderTableStyle() {
			return CreateDefaultStyleByName(HeaderTableStyleName);
		}
		protected internal AppearanceStyleBase GetDefaultHeaderStyle() {
			return CreateDefaultStyleByName(HeaderStyleName);
		}
		protected internal AppearanceStyleBase GetDefaultRootCellStyle() {
			return CreateDefaultStyleByName(RootCellStyleName);
		}
		protected internal AppearanceStyleBase GetDefaultErrorStyle() {
			return CreateDefaultStyleByName(ErrorStyleName);
		}
		protected internal AppearanceStyleBase GetDefaultHyperlinkStyle() {
			return CreateDefaultStyleByName(HyperlinkStyleName);
		}
		protected internal AppearanceStyleBase GetDefaultTableErrorContainerStyle() {
			return CreateDefaultStyleByName(TableErrorContainerStyleName);
		}
		protected internal AppearanceStyleBase GetDefaultErrorTextCellStyle() {
			return CreateDefaultStyleByName(ErrorTextCellStyleName);
		}
		protected internal AppearanceStyleBase GetDefaultListErrorContainerStyle() {
			return CreateDefaultStyleByName(ListErrorContainerStyleName);
		}
	}
}
