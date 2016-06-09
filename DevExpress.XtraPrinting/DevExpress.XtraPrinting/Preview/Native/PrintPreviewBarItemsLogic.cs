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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Control;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Controls;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraPrinting.Preview.Native {
	public class PrintPreviewBarItemsLogic : PreviewItemsLogicBase {
		#region inner classes
		class BarPopupMenu : PrintPreviewPopupMenu {
			public BarPopupMenu(PreviewItemsLogicBase logic, int groupIndex, BarBaseButtonItem[] buttons, IContainer container)
				: base(logic, groupIndex, buttons, container) {
				GetCheckedItem();
			}
			protected override void OnBeforePopup(CancelEventArgs e) {
				GetCheckedItem();
				foreach(BarItemLink barItemLink in ItemLinks) {
					PrintPreviewBarCheckItem item = barItemLink.Item as PrintPreviewBarCheckItem;
					if(item != null)
						barItemLink.Visible = logic.IsCommandEnabled(item.Command);
				}
				base.OnBeforePopup(e);
			}
			PrintPreviewBarCheckItem GetCheckedItem() {
				foreach(BarItemLink barItemLink in ItemLinks) {
					PrintPreviewBarCheckItem item = barItemLink.Item as PrintPreviewBarCheckItem;
					if(item != null && (item.Command == logic.DefaultExportFormat || item.Command == logic.DefaultSendFormat)) {
						item.Checked = true;
						return item;
					}
				}
				return null;
			}
			PrintPreviewBarCheckItem GetItemByCommand(PrintingSystemCommand command) {
				foreach(BarItemLink barItemLink in ItemLinks) {
					PrintPreviewBarCheckItem item = barItemLink.Item as PrintPreviewBarCheckItem;
					if(item != null && System.Collections.Comparer.Equals(item.Command, command))
						return item;
				}
				return null;
			}
		}
		#endregion
		int groupIndex = 1;
		protected internal override ZoomBarEditItem ZoomItem { 
			get { 
				return (ZoomBarEditItem)GetBarItemByCommand(Manager.Items, PrintingSystemCommand.Zoom); 
			}
		}
		public PrintPreviewBarItemsLogic(BarManager manager) : base(manager) { 
		}
		protected override PopupControl CreateExportMenuPopupControl(PrintingSystemCommand[] commands, PrintingSystemCommand parentCommand) {
			List<BarBaseButtonItem> buttons = new List<BarBaseButtonItem>();
			foreach(PrintingSystemCommand command in commands) {
				if(command == PrintingSystemCommand.ExportXps || command == PrintingSystemCommand.SendXps)
					continue;
				BarBaseButtonItem item = GetBarItemByCommand(command) as BarBaseButtonItem;
				if(item != null) buttons.Add(item);
			}			
			return new BarPopupMenu(this, groupIndex++, buttons.ToArray(), components);
		}
		protected override void HandleToolCommand(PrintingSystemCommand command, bool down) {
			switch(command) {
				case PrintingSystemCommand.Magnifier:
					ButtonHandTool.Down = false;
					PrintControl.ExecCommand(command, new object[] { down });
					break;
				case PrintingSystemCommand.HandTool:
					ButtonMagnifier.Down = false;
					PrintControl.ExecCommand(command, new object[] { down });
					break;
			}
		}
	}
}
