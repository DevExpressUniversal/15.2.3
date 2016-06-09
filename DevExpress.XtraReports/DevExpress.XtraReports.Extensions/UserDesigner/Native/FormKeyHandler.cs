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
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Design;
using System.Collections.Generic;
namespace DevExpress.XtraReports.UserDesigner.Native
{
	public class XRFormKeyHandler : IMessageFilter {
		#region static
		static XRFormKeyHandler instance = new XRFormKeyHandler();
		static readonly object padlock = new object();
		public static void RegisterDesignPanel(XRDesignPanel designPanel) {
			lock(padlock) {
				instance.Add(designPanel);
			}
		}
		public static void UnregisterDesignPanel(XRDesignPanel designPanel) {
			lock(padlock) {
				instance.Remove(designPanel);
			}
		}
		static bool IsSupportedCommand(MenuCommand menuCommand) {
			return menuCommand.Supported && menuCommand.Enabled;
		}
		static Dictionary<Keys, CommandWrapper> keyTable = new Dictionary<Keys, CommandWrapper>();
		static XRFormKeyHandler() {
			keyTable[Keys.Left] = new CommandWrapper(MenuCommands.KeyMoveLeft);
			keyTable[Keys.Right] = new CommandWrapper(MenuCommands.KeyMoveRight);
			keyTable[Keys.Up] = new CommandWrapper(MenuCommands.KeyMoveUp);
			keyTable[Keys.Down] = new CommandWrapper(MenuCommands.KeyMoveDown);
			keyTable[Keys.Tab] = new CommandWrapper(MenuCommands.KeySelectNext, false);
			keyTable[Keys.Delete] = new CommandWrapper(MenuCommands.Delete, false);
			keyTable[Keys.Back] = new CommandWrapper(MenuCommands.Delete, false);
			keyTable[Keys.Left | Keys.Shift] = new CommandWrapper(MenuCommands.KeySizeWidthDecrease);
			keyTable[Keys.Right | Keys.Shift] = new CommandWrapper(MenuCommands.KeySizeWidthIncrease);
			keyTable[Keys.Up | Keys.Shift] = new CommandWrapper(MenuCommands.KeySizeHeightDecrease);
			keyTable[Keys.Down | Keys.Shift] = new CommandWrapper(MenuCommands.KeySizeHeightIncrease);
			keyTable[Keys.Tab | Keys.Shift] = new CommandWrapper(MenuCommands.KeySelectPrevious, false);
			keyTable[Keys.Delete | Keys.Shift] = new CommandWrapper(MenuCommands.Delete, false);
			keyTable[Keys.Back | Keys.Shift] = new CommandWrapper(MenuCommands.Delete, false);
			keyTable[Keys.Left | Keys.Control] = new CommandWrapper(MenuCommands.KeyNudgeLeft);
			keyTable[Keys.Right | Keys.Control] = new CommandWrapper(MenuCommands.KeyNudgeRight);
			keyTable[Keys.Up | Keys.Control] = new CommandWrapper(MenuCommands.KeyNudgeUp);
			keyTable[Keys.Down | Keys.Control] = new CommandWrapper(MenuCommands.KeyNudgeDown);
			keyTable[Keys.Left | Keys.Control | Keys.Shift] = new CommandWrapper(MenuCommands.KeyNudgeWidthDecrease);
			keyTable[Keys.Right | Keys.Control | Keys.Shift] = new CommandWrapper(MenuCommands.KeyNudgeWidthIncrease);
			keyTable[Keys.Up | Keys.Control | Keys.Shift] = new CommandWrapper(MenuCommands.KeyNudgeHeightDecrease);
			keyTable[Keys.Down | Keys.Control | Keys.Shift] = new CommandWrapper(MenuCommands.KeyNudgeHeightIncrease);
			keyTable[Keys.Escape] = new CommandWrapper(MenuCommands.KeyCancel, false);
			keyTable[Keys.Control | Keys.Z] = new CommandWrapper(StandardCommands.Undo, false);
			keyTable[Keys.Control | Keys.Y] = new CommandWrapper(StandardCommands.Redo, false);
		}
		#endregion
		protected class CommandWrapper {
			CommandID commandID;
			bool restoreSelection;
			public CommandID CommandID { get { return commandID; }
			}
			public bool RestoreSelection { get { return restoreSelection; }
			}
			public CommandWrapper(CommandID commandID) : this(commandID, true) {
			}
			public CommandWrapper(CommandID commandID, bool restoreSelection) {
				this.commandID = commandID;
				this.restoreSelection = restoreSelection;
			}
		}
		List<XRDesignPanel> designPanels = new List<XRDesignPanel>();
		public void Add(XRDesignPanel designPanel) {
			if(designPanel == null)
				throw new ArgumentNullException();
			if(designPanels.Count == 0)
				Application.AddMessageFilter(this);
			designPanels.Add(designPanel);
		}
		public void Remove(XRDesignPanel designPanel) {
			if(designPanel == null)
				throw new ArgumentNullException();
			designPanels.Remove(designPanel);
			if(designPanels.Count == 0)
				Application.RemoveMessageFilter(this);
		}
		public bool PreFilterMessage(ref Message m) {
			const int keyPressedMessage = 0x100;
			if(m.Msg == keyPressedMessage) {
				foreach(XRDesignPanel designPanel in designPanels) {
					if(CanProcess(designPanel))
						return ProcessKey(ref m, designPanel);
				}
			}
			return false;
		}
		static bool CanProcess(XRDesignPanel designPanel) {
			return designPanel.ContainsInternalFocus();
		}
		static bool ProcessKey(ref Message m, XRDesignPanel designPanel) {
			if(designPanel.SelectedTabIndex != TabIndices.Designer)
				return false;
			Keys keyPressed = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;
			CommandWrapper commandWrapper;
			if(keyTable.TryGetValue(keyPressed, out commandWrapper)) {
				IServiceProvider servProvider = (IServiceProvider)designPanel;
				IMenuCommandService menuCommandService = (IMenuCommandService)servProvider.GetService(typeof(IMenuCommandService));
				ISelectionService   selectionService = (ISelectionService)servProvider.GetService(typeof(ISelectionService));
				ICollection components = selectionService.GetSelectedComponents();
				MenuCommand menuCommand = menuCommandService.FindCommand(commandWrapper.CommandID);
				if(menuCommand == null || IsSupportedCommand(menuCommand) ) {
					menuCommandService.GlobalInvoke(commandWrapper.CommandID);
					if (commandWrapper.RestoreSelection)
						selectionService.SetSelectedComponents(components);
				}
				return !IsInplaceEditingMode(servProvider);
			}
			return false;
		}
		static bool IsInplaceEditingMode(IServiceProvider servProvider) {
			ISelectionService selectionService = (ISelectionService)servProvider.GetService(typeof(ISelectionService));
			IDesignerHost designerHost = (IDesignerHost)servProvider.GetService(typeof(IDesignerHost));
			Design.XRTextControlBaseDesigner designer = designerHost.GetDesigner(selectionService.PrimarySelection as System.ComponentModel.IComponent) as Design.XRTextControlBaseDesigner;
			return designer != null && designer.IsInplaceEditingMode;
		}
	}
}
