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

using System.Windows.Input;
using DevExpress.XtraPrinting;
#if SL
using System.Windows;
#else
using System.Windows.Interop;
#endif
namespace DevExpress.Xpf.Printing {
	public class DocumentPreviewInputController : PreviewInputController {
		protected override void CreateShortcuts() {
			base.CreateShortcuts();
#if SL
			if(Application.Current.IsRunningOutOfBrowser) {
#else
			{
#endif
				TryAddShortcutForPSCommand(new KeyShortcut(ModifierKeys.Control, Key.O), PrintingSystemCommand.Open);
				TryAddShortcutForPSCommand(new KeyShortcut(ModifierKeys.Control, Key.S), PrintingSystemCommand.Save);
				TryAddShortcutForPSCommand(new KeyShortcut(ModifierKeys.Control, Key.P), PrintingSystemCommand.Print);
				TryAddShortcutForPSCommand(new KeyShortcut(ModifierKeys.Control, Key.F), PrintingSystemCommand.Find);
			}
			TryAddShortcutForPSCommand(new KeyShortcut(Key.Home), PrintingSystemCommand.ShowFirstPage);
			TryAddShortcutForPSCommand(new KeyShortcut(new ModifierKeys[] { ModifierKeys.Control, ModifierKeys.Shift }, Key.PageUp), PrintingSystemCommand.ShowFirstPage);
			TryAddShortcutForPSCommand(new KeyShortcut(new ModifierKeys[] { ModifierKeys.Control, ModifierKeys.Shift }, Key.Up), PrintingSystemCommand.ShowFirstPage);
			TryAddShortcutForPSCommand(new KeyShortcut(Key.End), PrintingSystemCommand.ShowLastPage);
			TryAddShortcutForPSCommand(new KeyShortcut(new ModifierKeys[] { ModifierKeys.Control, ModifierKeys.Shift }, Key.PageDown), PrintingSystemCommand.ShowLastPage);
			TryAddShortcutForPSCommand(new KeyShortcut(new ModifierKeys[] { ModifierKeys.Control, ModifierKeys.Shift }, Key.Down), PrintingSystemCommand.ShowLastPage);
			TryAddShortcutForPSCommand(new KeyShortcut(Key.Left), PrintingSystemCommand.ShowPrevPage);
			TryAddShortcutForPSCommand(new KeyShortcut(ModifierKeys.Control, Key.PageUp), PrintingSystemCommand.ShowPrevPage);
			TryAddShortcutForPSCommand(new KeyShortcut(Key.Right), PrintingSystemCommand.ShowNextPage);
			TryAddShortcutForPSCommand(new KeyShortcut(ModifierKeys.Control, Key.PageDown), PrintingSystemCommand.ShowNextPage);
		}
	}
}
