#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.XtraReports.Web {
	public class ReportToolbarStyles : StylesBase {
		const string
			LabelStyleName = "LabelStyle",
			ComboBoxStyleName = "ComboBoxStyle",
			ComboBoxListStyleName = "ComboBoxListStyle",
			ComboBoxButtonStyleName = "ComboBoxButtonStyle",
			ComboBoxItemStyleName = "ComboBoxItemStyle",
			TextBoxStyleName = "TextBoxStyle",
			ButtonStyleName = "ButtonStyle",
			EditorStyleName = "EditorStyle",
			CaptionCellStyleName = "CaptionCellStyle",
			CaptionStyleName = "CaptionStyle";
		public ReportToolbarStyles(ISkinOwner owner)
			: base(owner) {
		}
		#region properties
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarStylesLabelStyle")]
#endif
		public ReportToolbarLabelStyle LabelStyle {
			get { return (ReportToolbarLabelStyle)GetStyle(LabelStyleName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarStylesComboBoxStyle")]
#endif
		public ReportToolbarComboBoxStyle ComboBoxStyle {
			get { return (ReportToolbarComboBoxStyle)GetStyle(ComboBoxStyleName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarStylesComboBoxListStyle")]
#endif
		public AppearanceStyleBase ComboBoxListStyle {
			get { return (AppearanceStyleBase)GetStyle(ComboBoxListStyleName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarStylesComboBoxButtonStyle")]
#endif
		public ButtonControlStyle ComboBoxButtonStyle {
			get { return (ButtonControlStyle)GetStyle(ComboBoxButtonStyleName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarStylesComboBoxItemStyle")]
#endif
		public ListBoxItemStyle ComboBoxItemStyle {
			get { return (ListBoxItemStyle)GetStyle(ComboBoxItemStyleName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarStylesTextBoxStyle")]
#endif
		public ReportToolbarBoxStyle TextBoxStyle {
			get { return (ReportToolbarBoxStyle)GetStyle(TextBoxStyleName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarStylesButtonStyle")]
#endif
		public ReportToolbarButtonStyle ButtonStyle {
			get { return (ReportToolbarButtonStyle)GetStyle(ButtonStyleName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarStylesEditorStyle")]
#endif
		public ReportToolbarButtonStyle EditorStyle {
			get { return (ReportToolbarButtonStyle)GetStyle(EditorStyleName); }
		}
		[AutoFormatEnable]
		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarStylesCaptionCellStyle")]
#endif
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public EditorCaptionCellStyle CaptionCellStyle {
			get { return (EditorCaptionCellStyle)GetStyle(CaptionCellStyleName); }
		}
		[AutoFormatEnable]
		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarStylesCaptionStyle")]
#endif
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public EditorCaptionStyle CaptionStyle {
			get { return (EditorCaptionStyle)GetStyle(CaptionStyleName); }
		}
		#endregion
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			var src = source as ReportToolbarStyles;
			if(src == null) {
				return;
			}
			LabelStyle.CopyFrom(src.LabelStyle);
			ComboBoxStyle.CopyFrom(src.ComboBoxStyle);
			ComboBoxButtonStyle.CopyFrom(src.ComboBoxButtonStyle);
			ComboBoxListStyle.CopyFrom(src.ComboBoxListStyle);
			ComboBoxItemStyle.CopyFrom(src.ComboBoxItemStyle);
			TextBoxStyle.CopyFrom(src.TextBoxStyle);
			ButtonStyle.CopyFrom(src.ButtonStyle);
			EditorStyle.CopyFrom(src.EditorStyle);
			CaptionCellStyle.CopyFrom(src.CaptionCellStyle);
			CaptionStyle.CopyFrom(src.CaptionStyle);
		}
		protected override string GetCssClassNamePrefix() {
			return "dxxr";
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(
				base.GetStateManagedObjects(),
				new IStateManager[] {
					ButtonStyle,
					TextBoxStyle,
					ComboBoxStyle,
					ComboBoxItemStyle,
					ComboBoxButtonStyle,
					ComboBoxListStyle,
					LabelStyle,
					EditorStyle,
					CaptionCellStyle,
					CaptionStyle
				});
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(LabelStyleName, () => new ReportToolbarLabelStyle()));
			list.Add(new StyleInfo(TextBoxStyleName, () => new ReportToolbarBoxStyle()));
			list.Add(new StyleInfo(ButtonStyleName, () => new ReportToolbarButtonStyle()));
			list.Add(new StyleInfo(ComboBoxItemStyleName, () => new ListBoxItemStyle()));
			list.Add(new StyleInfo(ComboBoxButtonStyleName, () => new ButtonControlStyle()));
			list.Add(new StyleInfo(ComboBoxListStyleName, () => new AppearanceStyleBase()));
			list.Add(new StyleInfo(ComboBoxStyleName, () => new ReportToolbarComboBoxStyle()));
			list.Add(new StyleInfo(EditorStyleName, () => new ReportToolbarButtonStyle()));
			list.Add(new StyleInfo(CaptionCellStyleName, () => new EditorCaptionCellStyle()));
			list.Add(new StyleInfo(CaptionStyleName, () => new EditorCaptionStyle()));
		}
	}
}
