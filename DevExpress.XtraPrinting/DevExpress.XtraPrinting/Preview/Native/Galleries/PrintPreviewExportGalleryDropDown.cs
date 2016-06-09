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

using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Utils;
using System.Drawing.Printing;
using System;
namespace DevExpress.XtraPrinting.Preview.Native.Galleries {
	public class PrintPreviewExportGalleryDropDown : PrintPreviewGalleryDropDown {
		public static PrintPreviewGalleryItemData[] GetItemsData(PrintingSystemCommand[] commands) {
			return Array.ConvertAll<PrintingSystemCommand, PrintPreviewGalleryItemData>(commands,
				delegate(PrintingSystemCommand command) { return new ExportGalleryItemData(command); }
			);
		}
		PrintingSystemCommand parentCommand;
		public PrintPreviewExportGalleryDropDown(RibbonPreviewItemsLogicBase logic, PrintingSystemCommand[] commands, PrintingSystemCommand parentCommand, IContainer container)
			: base(logic, GetItemsData(commands), container) {
			this.parentCommand = parentCommand;
			Gallery.ImageSize = new Size(32, 32);
			CreateItems();
		}
		public void UpdateButtonImage() {
			BarItem barItem = (BarItem)Logic.GetBarItemByCommand(parentCommand);
			foreach(PrintPreviewGalleryItem item in Items)
				if(this.Logic.PrintingSystem == null || item.Command == this.Logic.DefaultExportFormat || item.Command == this.Logic.DefaultSendFormat) {
					barItem.LargeGlyph = item.Image;
					barItem.Glyph = item.SmallImage;
					break;
				}
		}
		protected override bool IsItemChecked(PrintPreviewGalleryItem item) {
			return item.Command == this.Logic.DefaultExportFormat || item.Command == this.Logic.DefaultSendFormat;
		}
		protected override void OnPrintPreviewGalleryItemClick(PrintPreviewGalleryItem commandItem) {
			if(commandItem != null) {
				this.Logic.DefaultExportFormat = commandItem.Command;
				this.Logic.DefaultSendFormat = commandItem.Command;
				UpdateButtonImage();
			}
		}
		protected override PrintPreviewGalleryItem CreateGalleryItem(PrintPreviewGalleryItemData itemData) {
			return new PrintPreviewGalleryItem(GetLargeImage(itemData.Alias), PrintRibbonControllerConfigurator.GetCommandCaption(itemData.Alias), PrintRibbonControllerConfigurator.GetCommandDescription(itemData.Alias), itemData.Command, null, GetImage(itemData.Alias));
		}
	}
	public class ExportGalleryItemData : PrintPreviewGalleryItemData {
		PrintingSystemCommand command;
		public override PrintingSystemCommand Command {
			get { return command; }
		}
		public override string Alias {
			get { return Command.ToString(); }
		}
		public ExportGalleryItemData(PrintingSystemCommand command) {
			this.command = command;
		}
	}
}
