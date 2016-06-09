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

using Microsoft.SharePoint.WebPartPages;
using System.Xml.Serialization;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.ComponentModel;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Collections;
using System;
namespace DevExpress.SharePoint {
	using DevExpress.SharePoint.Internal;
	using System.Collections.Generic;
	[XmlRoot(Namespace = "DevExpress.SharePoint")]
	public class SPxHtmlEditorWebPart : SPxWebPartBase {
		private SPxHtmlEditor contentEditor = null;
		private bool inEditMode = false;
		private string html = "";
		private Literal literalContent = null;
		[Browsable(false), WebPartStorage(Storage.Personal), Description("Html Property"), DefaultValue(""), FriendlyName("Html")]
		public string Html {
			get { return html ?? ""; }
			set { html = value; }
		}
		protected internal SPxHtmlEditor ContentEditor { get { return contentEditor; } }
		protected internal Literal LiteralContent { get { return literalContent; } }
		protected internal bool InEditMode {
			get { return inEditMode; }
			set { inEditMode = value; }
		}
		public override ToolPart[] GetToolParts() {
			List<ToolPart> ret = new List<ToolPart>();
			ret.Add(new WebPartToolPart());
			ret.Add(new SPxHtmlEditorToolPart());
			return ret.ToArray();
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			try {
				this.literalContent = new Literal();
				Controls.Add(LiteralContent);
				this.contentEditor = new SPxHtmlEditor();
				Controls.Add(ContentEditor);
				if (!string.IsNullOrEmpty(Width))
					ContentEditor.Width = Unit.Percentage(100);
				if (!string.IsNullOrEmpty(base.Height))
					ContentEditor.Height = Unit.Percentage(100);
			} catch (Exception exception) {
				Controls.Add(new LiteralControl("CreateChildControls:" + exception.Message));
			}
		}
		protected override void OnLoad(EventArgs e) {
			this.EnsureChildControls();
			base.OnLoad(e);
		}
		protected override void RenderWebPart(HtmlTextWriter output) {
			try {
				if (InEditMode)
					ContentEditor.Html = Html;
				else {
					LiteralContent.Text = Html;
					LiteralContent.Visible = true;
					ContentEditor.Visible = false;
				}
				this.RenderChildren(output);
			} catch (Exception exception) {
				output.Write("RenderWebPart:" + exception.Message + "<br/>" + exception.StackTrace);
			}
		}
	}
}
