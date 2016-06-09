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

using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Design;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking.Design;
namespace DevExpress.XtraSpreadsheet.Design {
	#region SpreadsheetDockManagerDesigner
	public class SpreadsheetDockManagerDesigner : DockManagerDesigner {
		IDesignerHost designerHost;
		public SpreadsheetDockManager SpreadsheetDockManager { get { return Component as SpreadsheetDockManager; } }
		protected override void OnInitialize() {
			base.OnInitialize();
			BarManager barManager = DesignHelpers.GetBarManager(SpreadsheetDockManager.Container);
			if (barManager != null)
				barManager.DockManager = SpreadsheetDockManager;
			SpreadsheetDockManager.InitializeCore();
			designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (designerHost == null)
				return;
			SpreadsheetControl spreadsheetControl =
				DesignHelpers.FindComponent(designerHost.Container, typeof(SpreadsheetControl)) as SpreadsheetControl;
			if (spreadsheetControl != null)
				SpreadsheetDockManager.SpreadsheetControl = spreadsheetControl;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(CreateSpreadsheetDockManagerActionList());
			base.RegisterActionLists(list);
		}
		protected virtual DesignerActionList CreateSpreadsheetDockManagerActionList() { return new SpreadsheetDockManagerActionList(this); }
	}
	#endregion
	#region SpreadsheetFieldListDockPanelActionList
	public class SpreadsheetDockManagerActionList : DesignerActionList {
		readonly SpreadsheetDockManagerDesigner designer;
		public SpreadsheetDockManagerActionList(SpreadsheetDockManagerDesigner designer)
			: base(designer.Component) { this.designer = designer; }
		public SpreadsheetDockManagerDesigner Designer { get { return designer; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			PopulateCreateBarsItems(result);
			return result;
		}
		protected internal virtual void PopulateCreateBarsItems(DesignerActionItemCollection items) {
			foreach (DockPanel dockPanel in Designer.SpreadsheetDockManager.Panels) {
				if (dockPanel is FieldListDockPanel || dockPanel is MailMergeParametersDockPanel) {
					items.Add(new DesignerActionPropertyItem("SpreadsheetControl", "SpreadsheetControl", "Toolbar"));
					break;
				}
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		public SpreadsheetControl SpreadsheetControl { get { return Designer.SpreadsheetDockManager.SpreadsheetControl; } set { Designer.SpreadsheetDockManager.SpreadsheetControl = value; } }
	}
	#endregion
}
