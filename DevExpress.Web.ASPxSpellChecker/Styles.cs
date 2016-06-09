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
using System.Drawing;
using System.Web.UI;
using System.ComponentModel;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxSpellChecker {
	public class CheckedTextContainerStyle : AppearanceStyle {
		[
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
	   PersistenceMode(PersistenceMode.InnerProperty), Browsable(false)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[
	   Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable, Browsable(false)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[
	   Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable, Browsable(false)]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false)]
		public new virtual AppearanceSelectedStyle SelectedStyle {
			get { return base.SelectedStyle; }	   
		}
		[Browsable(false)]
		public new virtual AppearanceSelectedStyle PressedStyle  {
			get { return base.PressedStyle; }
		}
	}
	public class ErrorWordStyle : AppearanceStyle {
		[
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), Browsable(false)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[
	   Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable, Browsable(false)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false)]
		public new virtual AppearanceSelectedStyle SelectedStyle {
			get { return base.SelectedStyle; }
		}
		[Browsable(false)]
		public new virtual AppearanceSelectedStyle PressedStyle {
			get { return base.PressedStyle; }
		}
	}
	public class SpellCheckerEditorStyles : EditorStyles {
		public SpellCheckerEditorStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		public override string ToString() { return string.Empty; }
	}
	public class SpellCheckerEditorImages : EditorImages {
		public SpellCheckerEditorImages(ISkinOwner skinOwner) : base(skinOwner) { }
	}
	public class SpellCheckerButtonStyles : ButtonControlStyles {
		public SpellCheckerButtonStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		public override string ToString() { return string.Empty; }
	}
	public class SpellCheckerDialogFormStyles : PopupControlStyles {
		public SpellCheckerDialogFormStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override PopupControlModalBackgroundStyle GetDefaultModalBackgroundStyle() {
			PopupControlModalBackgroundStyle style = base.GetDefaultModalBackgroundStyle();
			style.Opacity = 0;
			return style;
		}
		protected override PopupWindowContentStyle GetDefaultContentStyle() {
			PopupWindowContentStyle style = base.GetDefaultContentStyle();
			style.Paddings.Assign(new Paddings(Unit.Pixel(0)));
			return style;
		}
	}
	public class SpellCheckerStyles : StylesBase {
		public const string CssClassPrefix = "dxwsc";
		public const string ErrorWordStyleName = "ErrorWord";
		public const string CheckedTextContainerStyleName = "CheckedTextContainer";
		public const string RightToLeftCssClass = CssClassPrefix + "-rtl";
		public const string MainElementCssClass = CssClassPrefix + "-main";
		public SpellCheckerStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("SpellCheckerStylesCheckedTextContainer"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public CheckedTextContainerStyle CheckedTextContainer {
			get { return (CheckedTextContainerStyle)GetStyle(CheckedTextContainerStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("SpellCheckerStylesErrorWord"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ErrorWordStyle ErrorWord {
			get { return (ErrorWordStyle)GetStyle(ErrorWordStyleName); }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("SpellCheckerStylesLoadingPanel"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public LoadingPanelStyle LoadingPanel {
			get { return base.LoadingPanelInternal; }
		}
		protected override string GetCssClassNamePrefix() {
			return CssClassPrefix;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ErrorWordStyleName, delegate() { return new ErrorWordStyle(); }));
			list.Add(new StyleInfo(CheckedTextContainerStyleName, delegate() { return new CheckedTextContainerStyle(); }));
		}
		protected internal CheckedTextContainerStyle GetDefaultCheckedTextContainerStyle() { 
			CheckedTextContainerStyle style = new CheckedTextContainerStyle();
			style.CopyFrom(CreateStyleByName("CheckedTextContainerStyle"));
			return style;
		}
		protected internal ErrorWordStyle GetDefaultErrorWordStyle() {
			ErrorWordStyle style = new ErrorWordStyle();
			style.CopyFrom(CreateStyleByName("ErrorWordStyle"));
			return style;
		}
	}
}
