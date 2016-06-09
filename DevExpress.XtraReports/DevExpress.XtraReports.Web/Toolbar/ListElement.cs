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
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Web {
	[ControlBuilder(typeof(ListItemControlBuilder))]
	public class ListElement : CollectionItem {
		const string
			ImageUrlName = "ImageUrl",
			TextName = "Text",
			ValueName = "Value";
		public ListElement() {
		}
		public ListElement(string value) {
			Value = value;
		}
		#region properties
		[DefaultValue("")]
		[SRCategory(ReportStringId.CatAppearance)]
		[NotifyParentProperty(true)]
		[Editor(typeof(UrlEditor), typeof(UITypeEditor))]
		[UrlProperty]
		[AutoFormatUrlProperty]
		public string ImageUrl {
			get { return GetStringProperty(ImageUrlName, ""); }
			set { SetStringProperty(ImageUrlName, "", value); }
		}
		[DefaultValue("")]
		[SRCategory(ReportStringId.CatBehavior)]
		[Localizable(true)]
		public string Text {
			get { return GetStringProperty(TextName, ""); }
			set { SetStringProperty(TextName, string.Empty, value); }
		}
		[DefaultValue("")]
		[SRCategory(ReportStringId.CatBehavior)]
		[Localizable(true)]
		public string Value {
			get { return GetStringProperty(ValueName, ""); }
			set { SetStringProperty(ValueName, "", value); }
		}
		#endregion
		public override string ToString() {
			return Text;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var element = source as ListElement;
			if(element != null) {
				Text = element.Text;
				Value = element.Value;
				ImageUrl = element.ImageUrl;
			}
		}
	}
}
