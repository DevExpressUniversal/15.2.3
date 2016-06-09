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
using System.Linq;
using System.Text;
using DevExpress.XtraBars.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraBars;
using DevExpress.Snap.Extensions;
using System.ComponentModel.Design;
using System.Windows.Forms;
namespace DevExpress.Snap.Design {
	public class SnapDocumentManagerDesigner : DocumentManagerDesigner {
		protected override void OnInitialize() {
			if (Manager == null || Manager.Container == null) return;
			if (Manager.MdiParent != null || Manager.ContainerControl != null || Manager.Container.Components == null) return;
			if (Manager.MenuManager == null)
				Manager.MenuManager = ControlDesignerHelper.GetBarManager(Manager.Container);
			if (Manager.BarAndDockingController == null)
				Manager.BarAndDockingController = DesignHelpers.FindComponent(Manager.Container, typeof(BarAndDockingController)) as BarAndDockingController;
			SnapDockManager snapDockManager = DesignHelpers.FindComponent(Manager.Container, typeof(SnapDockManager)) as SnapDockManager;
			if(snapDockManager != null)
				Manager.ClientControl = snapDockManager.SnapControl;
			if (Manager.ClientControl == null) {
				Manager.ContainerControl = DesignHelpers.GetContainerControl(Manager.Container);
			}
		}
		protected override DocumentManagerActionList CreateDocumentManagerActionList() {
			return new SnapDocumentManagerActionList(this);
		}
	}
	public class SnapDocumentManagerActionList : DocumentManagerActionList {
		public SnapDocumentManagerActionList(SnapDocumentManagerDesigner designer)
			: base(designer) {
		}		
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "About", "About", null, true));
			return res;
		}
	}
}
