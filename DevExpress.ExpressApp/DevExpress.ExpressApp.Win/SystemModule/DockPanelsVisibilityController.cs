#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraBars.Docking;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public interface IDockManagerHolder {
		DockManager DockManager { get; }
	}
	public class DockPanelsVisibilityController : WindowController {
		private const string VisibilityActionNamePostfix = "VisibilityAction";
		private DockManager dockManager;
		private Dictionary<ActionBase, DockPanel> actionToPanels;
		public DockPanelsVisibilityController() {
			actionToPanels = new Dictionary<ActionBase, DockPanel>();
		}
		protected override void OnActivated() {
			base.OnActivated();
			Application.CustomizeTemplate += new EventHandler<CustomizeTemplateEventArgs>(Application_CustomizeTemplate);
		}
		protected override void OnDeactivated() {
			Application.CustomizeTemplate -= new EventHandler<CustomizeTemplateEventArgs>(Application_CustomizeTemplate);
			dockManager = null;
			if(actionToPanels != null) {
				actionToPanels.Clear();
				actionToPanels = null;
			}
			base.OnDeactivated();
		}
		private void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
			if((Window.Template == null || Window.Template == e.Template) && e.Template is IDockManagerHolder) {
				dockManager = ((IDockManagerHolder)e.Template).DockManager;
				foreach(DockPanel panel in dockManager.RootPanels) {
					CreatePanelVisibilityAction(panel);
				}
			}
		}
		protected virtual void CustomizePanelsAction(SingleChoiceAction action, DockPanel panel) {
			UpdateActionSelection(action, panel.Visibility);
		}
		private void CreatePanelVisibilityAction(DockPanel panel) {
			SingleChoiceAction action = new SingleChoiceAction(this, panel.Name + VisibilityActionNamePostfix, "Panels");
			action.Caption = panel.Text;
			EnumDescriptor enumDescriptor = new EnumDescriptor(typeof(DockVisibility));
			action.Items.Add(new ChoiceActionItem("Visible", enumDescriptor.GetCaption("Visible"), DockVisibility.Visible));
			action.Items.Add(new ChoiceActionItem("AutoHide", enumDescriptor.GetCaption("AutoHide"), DockVisibility.AutoHide));
			action.Items.Add(new ChoiceActionItem("Hidden", enumDescriptor.GetCaption("Hidden"), DockVisibility.Hidden));
			action.Disposed += new EventHandler(action_Disposed);
			action.Execute += new SingleChoiceActionExecuteEventHandler(action_Execute);
			panel.Disposed += new EventHandler(panel_Disposed);
			panel.TextChanged += new EventHandler(panel_TextChanged);
			panel.VisibilityChanged += new VisibilityChangedEventHandler(panel_VisibilityChanged);
			actionToPanels.Add(action, panel);
			CustomizePanelsAction(action, panel);
		}
		void action_Disposed(object sender, EventArgs e) {
			SingleChoiceAction action = (SingleChoiceAction)sender;
			action.Disposed -= new EventHandler(action_Disposed);
			action.Execute -= new SingleChoiceActionExecuteEventHandler(action_Execute);
		}
		private void panel_Disposed(object sender, EventArgs e) {
			DockPanel panel = (DockPanel)sender;
			panel.Disposed -= new EventHandler(panel_Disposed);
			panel.TextChanged -= new EventHandler(panel_TextChanged);
			panel.VisibilityChanged -= new VisibilityChangedEventHandler(panel_VisibilityChanged);
		}
		private void panel_TextChanged(object sender, EventArgs e) {
			foreach (KeyValuePair<ActionBase, DockPanel> actionPanelPair in actionToPanels) {
				if (actionPanelPair.Value == sender) {
					actionPanelPair.Key.Caption = actionPanelPair.Value.Text;
				}
			}
		}
		protected virtual void UpdateActionSelection(SingleChoiceAction action, DockVisibility visibility) {
			switch(visibility) {
				case DockVisibility.AutoHide: action.SelectedItem = action.Items[1]; break;
				case DockVisibility.Hidden: action.SelectedItem = action.Items[2]; break;
				case DockVisibility.Visible: action.SelectedItem = action.Items[0]; break;
			}
		}
		private void panel_VisibilityChanged(object sender, VisibilityChangedEventArgs e) {
			foreach(SingleChoiceAction action in actionToPanels.Keys) {
				if(actionToPanels[action] == e.Panel) {
					UpdateActionSelection(action, e.Visibility);
					break;
				}
			}
		}
		private void action_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			actionToPanels[e.Action].Visibility = (DockVisibility)e.SelectedChoiceActionItem.Data;
		}
#if DebugTest
		public const string DebugTest_VisibilityActionNamePostfix = VisibilityActionNamePostfix;
#endif
	}
}
