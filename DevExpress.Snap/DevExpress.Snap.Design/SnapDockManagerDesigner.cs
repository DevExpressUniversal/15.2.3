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

using System.ComponentModel.Design;
using DevExpress.XtraBars.Docking.Design;
using DevExpress.XtraBars;
using DevExpress.Snap.Extensions;
using DevExpress.Snap;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Snap.Design {
	public class SnapDockManagerDesigner : DockManagerDesigner {
		IDesignerHost designerHost;
		SnapDockManager SnapDockManager { get { return Component as SnapDockManager; } }
		protected override void OnInitialize() {
			base.OnInitialize();
			BarManager barManager = DesignHelpers.GetBarManager(SnapDockManager.Container);
			if (barManager != null)
				barManager.DockManager = SnapDockManager;
			SnapDockManager.InitializeCore();
			designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (designerHost == null)
				return;
			SnapControl snapControl = DesignHelpers.FindComponent(designerHost.Container, typeof(SnapControl)) as SnapControl;
			if (snapControl != null)
				SnapDockManager.SnapControl = snapControl;
			SnapBarController snapBarController = DesignHelpers.FindComponent(designerHost.Container, typeof(SnapBarController)) as SnapBarController;
			if (snapBarController != null)
				snapBarController.SnapDockManager = SnapDockManager;
		}
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if (DesignHelpers.FindComponent(designerHost.Container, typeof(SnapDocumentManager)) == null) {
				DesignHelpers.CreateComponent(designerHost, typeof(SnapDocumentManager));
			}
		}
	}
}
