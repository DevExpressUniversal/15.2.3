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
using System.Collections.Generic;
using DevExpress.ExpressApp.Templates;
namespace DevExpress.ExpressApp.Web.Templates {
	public partial class NestedFrameControlNew : NestedFrameControlBase, ISupportActionsToolbarVisibility {
		private bool toolBarVisibility = true;
		private void UpdateToolbarVisibility() {
			if(ToolBar != null) {
				ToolBar.Visible = toolBarVisibility;
			}
			if(ObjectsCreation != null) {
				ObjectsCreation.Visible = toolBarVisibility;
			}
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			UpdateToolbarVisibility();
		}
		protected override ContextActionsMenu CreateContextMenu() {
			return new ContextActionsMenu(this, "Edit", "RecordEdit", "ListView");
		}
		public override void SetStatus(ICollection<string> statusMessages) {
		}
		public override IActionContainer DefaultContainer {
			get { return ToolBar != null ? ToolBar.FindActionContainerById("View") : null; }
		}
		public override object ViewSiteControl {
			get { return viewSiteControl; }
		}
		public override void BeginUpdate() {
			base.BeginUpdate();
			if(ObjectsCreation != null) {
				ObjectsCreation.BeginUpdate();
			}
			if(ToolBar != null) {
				ToolBar.BeginUpdate();
			}
		}
		public override void EndUpdate() {
			if(ObjectsCreation != null) {
				ObjectsCreation.EndUpdate();
			}
			if(ToolBar != null) {
				ToolBar.EndUpdate();
			}
			base.EndUpdate();
		}
		void ISupportActionsToolbarVisibility.SetVisible(bool isVisible) {
			toolBarVisibility = isVisible;
			UpdateToolbarVisibility();
		}
	}
}
