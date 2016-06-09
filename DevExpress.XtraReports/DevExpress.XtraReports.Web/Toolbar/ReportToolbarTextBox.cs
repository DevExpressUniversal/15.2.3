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
using System.Web.UI.WebControls;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Web {
	public class ReportToolbarTextBox : ReportToolbarItem {
		const string
			TextName = "Text",
			IsReadOnlyName = "IsReadOnly",
			WidthName = "Width";
		const bool DefaultIsReadOnly = false;
		public ReportToolbarTextBox() {
		}
		public ReportToolbarTextBox(ReportToolbarItemKind itemKind, string text, bool isReadOnly)
			: base(itemKind) {
			SetTextCore(text);
			IsReadOnly = isReadOnly;
		}
		[DefaultValue("")]
		[SRCategory(ReportStringId.CatBehavior)]
		[Localizable(true)]
		[NotifyParentProperty(true)]
		public string Text {
			get {
				return GetStringProperty(TextName, "");
			}
			set {
				SetTextCore(value);
				LayoutChanged();
			}
		}
		[DefaultValue(DefaultIsReadOnly)]
		[SRCategory(ReportStringId.CatBehavior)]
		[NotifyParentProperty(true)]
		public bool IsReadOnly {
			get { return GetBoolProperty(IsReadOnlyName, DefaultIsReadOnly); }
			set { SetBoolProperty(IsReadOnlyName, DefaultIsReadOnly, value); }
		}
		[Editor("DevExpress.XtraReports.Web.Design.TextBoxItemKindEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public new ReportToolbarItemKind ItemKind {
			get { return base.ItemKind; }
			set { base.ItemKind = value; }
		}
		[Category("Layout")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Unit), "")]
		public Unit Width {
			get { return GetUnitProperty(WidthName, Unit.Empty); }
			set { SetUnitProperty(WidthName, Unit.Empty, value); }
		}
		void SetTextCore(string value) {
			SetStringProperty(TextName, "", value);
		}
	}
}
