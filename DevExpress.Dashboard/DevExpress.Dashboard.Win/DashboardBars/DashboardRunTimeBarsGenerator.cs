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

using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Bars;
namespace DevExpress.DashboardWin.Bars {
	[DXToolboxItem(false)]
	public class DashboardBarController : ControlCommandBarControllerBase<DashboardDesigner, DashboardCommandId> {
		protected override void UpdateBarItem(BarItem item) {
			base.UpdateBarItem(item);
			DashboardSkinsBarItem skinItem = item as DashboardSkinsBarItem;
			if (skinItem != null)
				SkinHelper.InitSkinGallery(skinItem);
		}
	}
}
namespace DevExpress.DashboardWin.Native {
	public class DashboardRunTimeBarsGenerator : RunTimeBarsGenerator<DashboardDesigner, DashboardCommandId> {
		readonly DashboardDesigner designer;
		readonly BarManager barManager;
		public DashboardRunTimeBarsGenerator(DashboardDesigner designer, BarManager barManager) : base(designer) {
			this.designer = designer;
			this.barManager = barManager;
		}
		public override Component GetBarContainer() {
			List<Component> supportedBarContainerCollection = new List<Component>();
			Control control = designer.Parent;
			while (control != null) {
				foreach (Control ctrl in control.Controls)
					if (ctrl is RibbonControl)
						supportedBarContainerCollection.Add(ctrl);
				control = control.Parent;
			}
			if (barManager != null)
				supportedBarContainerCollection.Add(barManager);
			return ChooseBarContainer(supportedBarContainerCollection);
		}
		protected override BarGenerationManagerFactory<DashboardDesigner, DashboardCommandId> CreateBarGenerationManagerFactory() {
			return new DashboardBarGenerationManagerFactory();
		}
		protected override ControlCommandBarControllerBase<DashboardDesigner, DashboardCommandId> CreateBarController() {
			return new DashboardBarController();
		}
	}
}
