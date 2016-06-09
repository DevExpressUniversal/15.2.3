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
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI;
using DevExpress.Web.Internal;
using System.IO;
namespace DevExpress.SharePoint.Internal {
	public static class Utils {
		const string DefaultWebPartCssPath = "/_layouts/15/1033/styles/dxWebPartStyles.css";
		public static void RegisterCSS(Page page){
			string key = Path.GetFileName(DefaultWebPartCssPath);
			if (!page.ClientScript.IsClientScriptBlockRegistered(key))
				page.ClientScript.RegisterClientScriptBlock(typeof(SPxWebPartBase), key,
					RenderUtils.GetLinkHtml(page.ResolveClientUrl(DefaultWebPartCssPath)));
		}
	}
}
namespace DevExpress.SharePoint {
	using DevExpress.SharePoint.Internal;
	public class SPxWebPartBase : WebPart {
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			Utils.RegisterCSS(Page);
		}
	}
	public class SPxWebPart : SPxWebPartBase {
		const string OpenToolPaneLinkStringTempalte = "<a href=\"javascript:MSOTlPn_ShowToolPane2Wrapper('Edit', this, '{0}')\">{1}</a>";
		protected override void CreateChildControls() {
				ClearControlFields();
				if (HasContent())
					CreateControlHierarchy();
				else
					if (IsShowDefaultMessage())
						CreateDefaultMessageControl();
			base.CreateChildControls();
		}
		protected virtual void ClearControlFields() {
		}
		protected virtual void CreateControlHierarchy() {
		}
		protected virtual void CreateDefaultMessageControl() {
			string message = GetDefaultMessage();
			if (!string.IsNullOrEmpty(message))
				Controls.Add(RenderUtils.CreateLiteralControl(message));
		}
		protected virtual string GetDefaultMessage() {
			return "";
		}
		protected virtual bool HasContent() {
			return true;
		}
		protected virtual bool IsShowDefaultMessage() {
			return WebPartManager.DisplayMode.AllowPageDesign;
		}
		protected string GetOpenToolPaneLinkString(string message) {
			return string.Format(OpenToolPaneLinkStringTempalte, ID, message);			
		}
		protected void ResetControlHierarchy() {
			ChildControlsCreated = false;
		}
	}
}
