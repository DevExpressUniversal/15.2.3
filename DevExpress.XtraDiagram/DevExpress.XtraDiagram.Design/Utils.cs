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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Diagram.Core.Localization;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraDiagram.Bars;
using DevExpress.XtraDiagram.Utils;
using DevExpress.Utils.Design;
namespace DevExpress.XtraDiagram.Design {
	public class DiagramDesignUtils {
		public static T FindComponent<T>(DiagramControl owner) where T : class {
			foreach(IComponent component in owner.Container.Components) {
				T comp = component as T;
				if(comp != null) return comp;
			}
			return null;
		}
		public static void DoAction(IServiceProvider site, object component, Action action) {
			IComponentChangeService svc = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
			if(svc != null) {
				svc.OnComponentChanging(component, null);
			}
			action();
			if(svc != null) {
				svc.OnComponentChanged(component, null, null, null);
			}
		}
		public static void CreateRibbon(DiagramControl diagram, bool refreshSmartPanel) {
			ISite site = diagram.Site;
			IDesignerHost designerHost = (IDesignerHost)site.GetService(typeof(IDesignerHost));
			string transactionName = DiagramControlLocalizer.GetString(DiagramControlStringId.CreateRibbonDesignerActionListCommand);
			using(DesignerTransaction transaction = designerHost.CreateTransaction(transactionName)) {
				Control parent = diagram.Parent;
				RibbonControl ribbon = FindComponent<RibbonControl>(diagram);
				if(ribbon == null) {
					ribbon = (RibbonControl)designerHost.CreateComponent(typeof(RibbonControl));
					BarAndDockingController controller = FindComponent<BarAndDockingController>(diagram);
					if(controller != null) {
						ISupportLookAndFeel lookAndFeelControl = diagram as ISupportLookAndFeel;
						if(lookAndFeelControl != null) {
							DoAction(site, controller, () => controller.LookAndFeel.ParentLookAndFeel = lookAndFeelControl.LookAndFeel);
						}
						DoAction(site, ribbon, () => ribbon.Controller = controller);
					}
					DiagramUtils.SetupRibbon(ribbon);
					DoAction(site, parent, () => parent.Controls.Add(ribbon));
				}
				RibbonForm ribbonForm = parent as RibbonForm;
				if(ribbonForm != null) {
					DoAction(site, ribbonForm, () => ribbonForm.Ribbon = ribbon);
				}
				DiagramDesignTimeBarsGenerator bg = new DiagramDesignTimeBarsGenerator(designerHost, diagram, typeof(RibbonControl));
				DiagramBarsHelper.AddDefaultBars(bg);
				RibbonPage page = DiagramUtils.FindPage(ribbon, typeof(DiagramHomeRibbonPage));
				if(page != null) {
					ribbon.SelectedPage = page;
				}
				transaction.Commit();
				if(refreshSmartPanel) EditorContextHelperEx.RefreshSmartPanel(diagram);
			}
		}
	}
}
