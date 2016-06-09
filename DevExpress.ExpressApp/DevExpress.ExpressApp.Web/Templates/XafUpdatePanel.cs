#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.ExpressApp.Web.Templates {
	[ToolboxItem(false)] 
	public class XafUpdatePanel : Panel {
		private bool isRendered;
		private bool updateAlways = true;
		private bool updatePanelForASPxGridListCallback = true;
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			IXafUpdatePanelsProvider provider = Page as IXafUpdatePanelsProvider;
			if(provider != null) {
				provider.RegisterPanel(this);
			}
		}
		protected override void Render(HtmlTextWriter writer) {
			base.Render(writer);
			isRendered = true;
		}
		public override void Dispose() {
			IXafUpdatePanelsProvider provider = Page as IXafUpdatePanelsProvider;
			if(provider != null) {
				provider.UnregisterPanel(this);
			}
			base.Dispose();
		}
		internal bool IsRendered {
			get { return isRendered; }
		}
		public bool UpdateAlways {
			get { return updateAlways; }
			set { updateAlways = value; }
		}
		public bool UpdatePanelForASPxGridListCallback {
			get { return updatePanelForASPxGridListCallback; }
			set { updatePanelForASPxGridListCallback = value; }
		}
	}
}
