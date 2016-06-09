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
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Design;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Design;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Design;
using DevExpress.XtraReports.UI;
using DevExpress.Utils.About;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.Design {
	public class XRDesignBarManagerDesigner : SpecializedBarManagerDesigner {
		XRDesignBarManager XRDesignBarManager { get { return (XRDesignBarManager)Component; } }
		public XRDesignBarManagerDesigner() {
		}
		protected override BarManagerConfigurator[] CreateUpdaters() {
			return new BarManagerConfigurator[] { 
				new SaveAllBarItemDesignBarManagerConfigurator(XRDesignBarManager), 
				new MdiDesignBarManagerConfigurator(XRDesignBarManager), 
				new CloseBarItemDesignBarManagerConfigurator(XRDesignBarManager), 
				new ZoomDesignBarManagerConfigurator(XRDesignBarManager), 
				new ToolBoxBarConfigurator(XRDesignBarManager) };
		}
		protected override void InitSpecializedBarManager() {
			BarManagerConfigurator.Configure(
				new DesignBarManagerConfigurator(XRDesignBarManager),
				new SaveAllBarItemDesignBarManagerConfigurator(XRDesignBarManager),
				new MdiDesignBarManagerConfigurator(XRDesignBarManager),
				new CloseBarItemDesignBarManagerConfigurator(XRDesignBarManager),
				new ZoomDesignBarManagerConfigurator(XRDesignBarManager),
				new ToolBoxBarConfigurator(XRDesignBarManager)
			);
		}
		protected override DockManager FindDockManager() {
			return DevExpress.XtraPrinting.Native.DesignHelpers.FindInheritedComponent(Manager.Container, typeof(DockManager)) as DockManager;
		}
		protected override BarManagerActionList CreateBarManagerActionList() {
			return new XRDesignBarManagerActionList(this);
		}
		protected override void OnAboutClick(object sender, EventArgs e) {
			About();
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(About));
		}
		void About() {
		}
	}
	class ToolBoxBarConfigurator : BarManagerConfigurator {
		XRDesignBarManager XRDesignBarManager {
			get { return manager as XRDesignBarManager; }
		}
		public override bool UpdateNeeded {
			get {
				foreach(Bar bar in XRDesignBarManager.Bars) {
					if(bar.BarName == XRDesignBarManagerBarNames.Toolbox)
						return false;
				}
				return true;
			}
		}
		public ToolBoxBarConfigurator(XRDesignBarManager manager)
			: base(manager) {
		}
		public override void ConfigInternal() {
			this.XRDesignBarManager.BeginUpdate();
			Bar bar = new Bar(this.XRDesignBarManager);
			bar.OptionsBar.AllowQuickCustomization = false;
			string category = ReportLocalizer.GetString(ReportStringId.UD_XtraReportsToolboxCategoryName);
			AddBar(bar, XRDesignBarManagerBarNames.Toolbox, 0, 0, BarDockStyle.Left, category);
			this.XRDesignBarManager.SetToolboxType(bar, ToolboxType.Standard);
			if(!this.XRDesignBarManager.Updates.Contains(XRDesignBarManagerBarNames.Toolbox))
				this.XRDesignBarManager.Updates.Add(XRDesignBarManagerBarNames.Toolbox);
			this.XRDesignBarManager.EndUpdate();
		}
	}
}
