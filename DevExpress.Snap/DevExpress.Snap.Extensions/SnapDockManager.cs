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
using DevExpress.Snap;
using DevExpress.Snap.Extensions.UI;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.Snap.Extensions {
	[Designer("DevExpress.Snap.Design.SnapDockManagerDesigner, " + AssemblyInfo.SRAssemblySnapDesign, typeof(IDesigner))]
	  [ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "SnapDockManager.bmp")]
	  [DXToolboxItem(false)]
	public class SnapDockManager : DockManager {
		bool createTypedDockPanels;
		SnapControl snapControl;
		DesignDockPanelType designDockPanelType;
		public SnapDockManager() : base() { 
		}
		public SnapDockManager(ContainerControl form) : base(form) { 
		}
		public SnapDockManager(IContainer container) : base(container) { 
		}
#if !SL
	[DevExpressSnapExtensionsLocalizedDescription("SnapDockManagerSnapControl")]
#endif
		public SnapControl SnapControl {
			get { return snapControl; }
			set { 
				snapControl = value;
				SetSnapControlProperty();
			}
		}
		void SetSnapControlProperty() {
			foreach (DockPanel panel in Panels) {
				SnapDockPanelBase snapDockPanel = panel as SnapDockPanelBase;
				if (snapDockPanel != null)
					snapDockPanel.SnapControl = snapControl;
			}
		}
		public void InitializeCore() {
			Clear();
			createTypedDockPanels = true;
			SetDefaultPanels();
			createTypedDockPanels = false;
		}
		protected void SetDefaultPanels() {
			SnapDockPanelBase fieldListPanel = AddDockPanel(DesignDockPanelType.FieldList, DockingStyle.Right);
			SnapDockPanelBase reportExplorer = AddDockPanel(DesignDockPanelType.ReportExplorer, DockingStyle.Right);
			reportExplorer.DockTo(fieldListPanel);
		}
		protected SnapDockPanelBase AddDockPanel(DesignDockPanelType panelType, DockingStyle dockingStyle) {
			designDockPanelType = panelType;
			return AddPanel(dockingStyle) as SnapDockPanelBase;
		}
		protected override DockPanel CreateDockPanel(DockingStyle dock, bool createControlContainer) {
			if (!createControlContainer || !createTypedDockPanels) return base.CreateDockPanel(dock, createControlContainer);
			switch (designDockPanelType) {
				case DesignDockPanelType.FieldList:
					return new FieldListDockPanel(this, dock);
				case DesignDockPanelType.ReportExplorer:
					return new ReportExplorerDockPanel(this, dock);
			}
			return null;
		}
		protected override string GetDockPanelText(DockPanel panel) {
			if (panel is SnapDockPanelBase)
				return panel.Text;
			return base.GetDockPanelText(panel);
		}
	}
	public enum DesignDockPanelType { 
		FieldList,
		ReportExplorer
	}
}
