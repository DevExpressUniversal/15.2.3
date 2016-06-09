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
using System.Web.UI;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public class DockZoneControl : ASPxInternalWebControl {
		ASPxDockZone zone = null;
		WebControl panelPlaceholder = null;
		public DockZoneControl(ASPxDockZone zone) {
			this.zone = zone;
		}
		protected ASPxDockZone Zone { get { return this.zone; } }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override bool HasRootTag() {
			return true;
		}
		protected override void ClearControlFields() {
			this.panelPlaceholder = null;
		}
		protected override void CreateControlHierarchy() {
			if(!DesignMode && Zone.Enabled) {
				this.panelPlaceholder = RenderUtils.CreateDiv();
				Controls.Add(this.panelPlaceholder);
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(Zone, this);
			Zone.GetControlStyle().AssignToControl(this, true);
			RenderUtils.SetVisibility(this, Zone.IsClientVisible(), true);
			if(this.panelPlaceholder != null)
				Zone.GetPanelPlaceholderStyle().AssignToControl(this.panelPlaceholder);
			if(DesignMode) {
				Style.Add(HtmlTextWriterStyle.Position, "static");
				if(Zone.Border.BorderStyle == BorderStyle.NotSet || Zone.Border.BorderStyle == BorderStyle.None) {
					Style.Add(HtmlTextWriterStyle.BorderStyle, "dotted");
					Style.Add(HtmlTextWriterStyle.BorderWidth, "1px");
					Style.Add(HtmlTextWriterStyle.BorderColor, "#000000");
				}
			}
		}
	}
}
