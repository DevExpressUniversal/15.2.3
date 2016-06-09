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
using DevExpress.ExpressApp.Web.SystemModule;
namespace DevExpress.ExpressApp.Web.Controls {
	[ToolboxItem(false)] 
	[Designer("DevExpress.ExpressApp.Web.Design.ViewSiteControlDesigner, DevExpress.ExpressApp.Web.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.ComponentModel.Design.IDesigner))]
	public class ViewSiteControl : SimpleViewDependentControl<Panel>, INamingContainer {
		private View view;
		private void view_ControlsCreated(object sender, EventArgs e) {
			if(Control != null) {
				Control.Controls.Clear();
				Control layout = (Control)((View)sender).Control;
				Control.Controls.Add(layout);
			}
		}
		public override void SetView(View view) {
			Control.Controls.Clear();
			if(this.view != null) {
				this.view.ControlsCreated -= new EventHandler(view_ControlsCreated);
			}
			this.view = view;
			if(view != null) {
				this.ID = WebIdHelper.GetViewShortName(view); 
				view.ControlsCreated += new EventHandler(view_ControlsCreated);
				view.CreateControls();
			}
		}
		protected override void OnUnload(EventArgs e) {
			if(view != null) {
				view.ControlsCreated -= new EventHandler(view_ControlsCreated);
				view = null;
			}
			base.OnUnload(e);
		}
		protected override void SetupControl() {
			base.SetupControl();
			Control.Width = Unit.Percentage(100);
			Control.ID = "VSP";
			Control.EnableViewState = false;
		}
	}
}
