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
using System;
namespace DevExpress.Utils.Design {
	public interface IMenuCommandServiceEx : IMenuCommandService {
		bool GlobalInvoke(CommandID commandID, object[] args);
		event EventHandler MenuCommandChanged;
	}
	public class MenuCommandServiceHelper {
		public static IMenuCommandService RegisterMenuCommandService(IDesignerHost host) {
			if (host == null)
				return null;
			IMenuCommandService service = host.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if (service == null)
				return null;
			DXMenuCommandService dxService = service as DXMenuCommandService;
			if (dxService != null)
				return dxService;
			dxService = new DXMenuCommandService(service);
			host.RemoveService(typeof(IMenuCommandService));
			host.AddService(typeof(IMenuCommandService), dxService);
			return dxService;
		}
	}
	class DXMenuCommandService : IMenuCommandServiceEx {
		IMenuCommandService defaultService;
		public DesignerVerbCollection Verbs { get { return defaultService.Verbs; } }
		public DXMenuCommandService(IMenuCommandService defaultService) {
			this.defaultService = defaultService;
		}
		public void AddCommand(MenuCommand command)	{
			MenuCommand currentCommand = FindCommand(command.CommandID);
			if (currentCommand != null)
				RemoveCommand(currentCommand);
			defaultService.AddCommand(command);
		}
		public void RemoveCommand(MenuCommand command) {
			defaultService.RemoveCommand(command);
		}
		public MenuCommand FindCommand(CommandID commandID) {
			return defaultService.FindCommand(commandID);
		}
		public void AddVerb(DesignerVerb verb) {
			defaultService.AddVerb(verb);
		}
		public void RemoveVerb(DesignerVerb verb) {
			defaultService.RemoveVerb(verb);
		}
		public void ShowContextMenu(CommandID menuID, int x, int y) {
			defaultService.ShowContextMenu(menuID, x, y);
		}
		public bool GlobalInvoke(CommandID commandID) {
			return defaultService.GlobalInvoke(commandID);
		}
		public bool GlobalInvoke(CommandID commandID, object[] args) {
			IMenuCommandServiceEx defaultServiceEx = defaultService as IMenuCommandServiceEx;
			return defaultServiceEx != null && defaultServiceEx.GlobalInvoke(commandID, args);
		}
		event EventHandler IMenuCommandServiceEx.MenuCommandChanged {
			add { throw new NotSupportedException(); }
			remove { throw new NotSupportedException(); }
		}
	}
}
