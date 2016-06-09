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
using System.Text;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraPrinting.Design;
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.Utils.Design;
using DevExpress.Utils.About;
using DevExpress.XtraReports.UI;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Design {
	public class XRDesignRibbonControllerDesigner : RibbonControllerDesignerBase {
		const string ResourceFileName = "XRDesignRibbonControllerResources.resx";
		XRDesignRibbonController DesignRibbonController { get { return (XRDesignRibbonController)this.Controller; } }
		protected override Dictionary<string, System.Drawing.Image> GetImagesForInitialize() {
			Dictionary<string, System.Drawing.Image> designImages = CreateImagesResource(XRDesignRibbonControllerConfigurator.GetImagesFromAssembly(), ResourceFileName, Component.Site);
			Dictionary<string, System.Drawing.Image> printImages = CreateImagesResource(PrintRibbonControllerConfigurator.GetImagesFromAssembly(), PrintRibbonControllerDesigner.ResourceFileName, Component.Site);
			return RibbonControllerHelper.JoinImageDictionaries(designImages, printImages);
		}
		protected override void AssignMainControl() {
		}
		protected override void UpdateVerbEventHandler(object sender, EventArgs e) {
			PrintRibbonController printRibbonController = XRDesignRibbonControllerHelper.GetPrintRibbonController(DesignRibbonController);
			PrintRibbonControllerDesigner.UpdatePrintRibbonController(printRibbonController, RibbonControl, RibbonStatusBar);
			if(ScriptsRibbonUpdater.GetUpdateNeeded(RibbonControl.Manager)) {
				ScriptsRibbonUpdater scriptsUpdater = new ScriptsRibbonUpdater(RibbonControl, RibbonStatusBar, XRDesignRibbonControllerConfigurator.GetImagesFromAssembly());
				scriptsUpdater.Configure(((ISupportContextSpecifier)printRibbonController).ContextSpecifier);
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Insert(0, new XRDesignDockManagerActionList(Component));
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(About));
		}
		void About() {
		}
	}
	class XRDesignDockManagerActionList : DesignerActionList {
		public XRDesignDockManagerActionList(IComponent component)
			: base(component) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			IDesignerHost host = Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host.FirstComponent<XRDesignDockManager>() == null)
				result.Add(new DesignerActionMethodItem(this, "CreateToolWindows", "Create Tool Windows", ReportStringId.CatLayout.GetString(), false));
			return result;
		}
		void CreateToolWindows() {
			AddComponent(new XRDesignDockManager());
		}
		void AddComponent(Component comp) {
			IDesignerHost host = Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			host.AddToContainer(comp, true);
			DesignerActionUIService serv = host.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
			if(serv != null)
				serv.Refresh(Component);
		}
	}
}
