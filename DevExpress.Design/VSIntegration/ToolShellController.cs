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
using System.ComponentModel.Design;
using DevExpress.Data.Utils;
namespace DevExpress.Design.VSIntegration {
	public interface IToolShellProvider {
		IToolShell ToolShell { get; }
	}
	public class ToolShellController {
		bool activeDesignerChanging;
		public ToolShellController(IServiceProvider serviceProvider) {
			IDesignerEventService eventService = serviceProvider.GetService(typeof(IDesignerEventService)) as IDesignerEventService;
			eventService.ActiveDesignerChanged += new ActiveDesignerEventHandler(OnActiveDesignerChanged);
			IDesignerHost designerHost = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			SubscribeEvents(designerHost);
			ChangeToolVisibility(designerHost, null);
		}
		void OnActiveDesignerChanged(object sender, ActiveDesignerEventArgs e) {
			UnsubscibeEvents(e.OldDesigner);
			UnsubscibeEvents(e.NewDesigner);
			SubscribeEvents(e.NewDesigner);
			ChangeToolVisibility(e.NewDesigner, e.OldDesigner);
		}
		void SubscribeEvents(IDesignerHost designerHost) {
			if(designerHost == null)
				return;
			designerHost.LoadComplete += new EventHandler(designerHost_LoadComplete);
		}
		void UnsubscibeEvents(IDesignerHost designerHost) {
			if(designerHost == null)
				return;
			designerHost.LoadComplete -= new EventHandler(designerHost_LoadComplete);
		}
		void designerHost_LoadComplete(object sender, EventArgs e) {
			ChangeToolVisibility(sender as IDesignerHost, null);
		}
		void ChangeToolVisibility(IDesignerHost newDesignerHost, IDesignerHost oldDesignerHost) {
			if(activeDesignerChanging) return;
			activeDesignerChanging = true;
			try {
				IToolShell oldToolShell = GetToolShell(oldDesignerHost);
				IToolShell toolShell = GetToolShell(newDesignerHost);
				if(oldToolShell != null) {
					if(toolShell != null)
						(oldToolShell).HideIfNotContains(toolShell);
					else
						oldToolShell.Hide();
				}
				if(toolShell != null) {
					toolShell.UpdateToolItems();
					toolShell.ShowNoActivate();
				}
			} finally {
				activeDesignerChanging = false;
			}
		}
		static IToolShell GetToolShell(IDesignerHost host) {
			if(host == null || host.RootComponent == null)
				return null;
			IToolShellProvider toolShellProvider = host.GetDesigner(host.RootComponent) as IToolShellProvider;
			if(toolShellProvider == null)
				return null;
			return toolShellProvider.ToolShell;
		}
	}
}
