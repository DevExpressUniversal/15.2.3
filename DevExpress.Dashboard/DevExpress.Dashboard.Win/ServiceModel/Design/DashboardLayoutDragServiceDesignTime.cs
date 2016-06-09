#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardWin.ServiceModel.Design;
using DevExpress.Utils;
using DevExpress.XtraLayout;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.ServiceModel.Design {
	public class DashboardLayoutDragServiceDesignTime : IDisposable {
		readonly LayoutControl layoutControl;
		readonly IDesignerHost designerHost;
		IDragHandler dragHandler;
		public DashboardLayoutDragServiceDesignTime(LayoutControl layoutControl, IDesignerHost designerHost) {
			Guard.ArgumentNotNull(layoutControl, "layoutControl");
			Guard.ArgumentNotNull(designerHost, "designerHost");
			this.layoutControl = layoutControl;
			this.designerHost = designerHost;
			layoutControl.DragOver += OnDragOver;
			layoutControl.DragEnter += OnDragEnter;
			layoutControl.DragDrop += OnDragDrop;
			layoutControl.DragLeave += OnDragLeave;
		}
		public void Dispose() {
			layoutControl.DragOver -= OnDragOver;
			layoutControl.DragEnter -= OnDragEnter;
			layoutControl.DragDrop -= OnDragDrop;
			layoutControl.DragLeave -= OnDragLeave;
		}
		void OnDragEnter(object sender, DragEventArgs e) {
			IDragDropService dragDropService = (IDragDropService)designerHost.GetService(typeof(IDragDropService));
			dragHandler = dragDropService.GetDragHandler(e.Data);
			dragHandler.HandleDragEnter(this, e);
		}
		void OnDragOver(object sender, DragEventArgs e) {
			dragHandler.HandleDragOver(this, e);
		}
		void OnDragLeave(object sender, EventArgs e) {
			dragHandler.HandleDragLeave(this, e);
		}
		void OnDragDrop(object sender, DragEventArgs e) {
			dragHandler.HandleDragDrop(this, e);
		}
	}
}
