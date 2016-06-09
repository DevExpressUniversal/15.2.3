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

using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Web;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Web {
	public class ReportToolbarLabel : ReportToolbarItem {
		const string
			TextName = "Text",
			AssociatedItemNameName = "AssociatedItemName";
		public ReportToolbarLabel() {
		}
		public ReportToolbarLabel(string text) {
			SetTextCore(text);
		}
		public ReportToolbarLabel(ReportToolbarItemKind itemKind) {
			ItemKind = itemKind;
		}
		#region properties
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarLabelItemKind")]
#endif
		[NotifyParentProperty(true)]
		[Editor("DevExpress.XtraReports.Web.Design.LabelItemKindEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public new ReportToolbarItemKind ItemKind {
			get { return base.ItemKind; }
			set { base.ItemKind = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarLabelText")]
#endif
		[DefaultValue("")]
		[SRCategory(ReportStringId.CatBehavior)]
		[NotifyParentProperty(true)]
		[Localizable(true)]
		public string Text {
			get {
				return GetStringProperty(TextName, "");
			}
			set {
				SetTextCore(value);
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarLabelAssociatedItemName")]
#endif
		[DefaultValue("")]
		[SRCategory(ReportStringId.CatBehavior)]
		[NotifyParentProperty(true)]
		[Localizable(true)]
		public string AssociatedItemName {
			get { return GetStringProperty(AssociatedItemNameName, ""); }
			set { SetStringProperty(AssociatedItemNameName, "", value); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override string Name {
			get { return ""; }
			set { }
		}
		#endregion
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var label = source as ReportToolbarLabel;
			if(label != null) {
				Text = label.Text;
			}
		}
		void SetTextCore(string value) {
			SetStringProperty(TextName, "", value);
		}
	}
}
