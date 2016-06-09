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
using System.ComponentModel.Design;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Design {
	public class DashboardActionList : DesignerActionList {
		readonly Control control;
		DesignerActionUIService designerActionUIService;
		DesignerActionUIService DesignerActionUIService {
			get {
				if (designerActionUIService == null) {
					ISite site = control.Site;
					if (site != null)
						designerActionUIService = site.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
				}
				return designerActionUIService;
			}
		}
		public DashboardActionList(Control control) : base(control) {
			this.control = control;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = new DesignerActionItemCollection();
			if (control.Dock == DockStyle.Fill)
				collection.Add(new DesignerActionMethodItem(this, "UndockInParentContainer", "Undock in parent container", "Layout"));
			else
				collection.Add(new DesignerActionMethodItem(this, "DockInParentContainer", "Dock in parent container", "Layout"));
			return collection;
		}
		public void DockInParentContainer() {
			control.Dock = DockStyle.Fill;
			DesignerActionUIService designerActionUIService = DesignerActionUIService;
			if (designerActionUIService != null)
				designerActionUIService.Refresh(control);
		}
		public void UndockInParentContainer() {
			control.Dock = DockStyle.None;
			DesignerActionUIService designerActionUIService = DesignerActionUIService;
			if (designerActionUIService != null)
				designerActionUIService.Refresh(control);
		}
	}
}
