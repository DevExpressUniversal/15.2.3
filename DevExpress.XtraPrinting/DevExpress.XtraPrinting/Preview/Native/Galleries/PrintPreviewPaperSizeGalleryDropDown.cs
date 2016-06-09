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
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Utils;
using System.Drawing.Printing;
using DevExpress.DocumentView;
namespace DevExpress.XtraPrinting.Preview.Native.Galleries {
	public class PrintPreviewPaperSizeGalleryDropDown : PrintPreviewGalleryDropDownBase {
		#region static
		public static readonly PrintPreviewGalleryItemData[] PredefinedItemsData = new PrintPreviewGalleryItemData[] {
			new PaperSizeGalleryItemData("A3"),
			new PaperSizeGalleryItemData("A4"),
			new PaperSizeGalleryItemData("A5"),
			new PaperSizeGalleryItemData("A6"),
			new PaperSizeGalleryItemData("Executive"),
			new PaperSizeGalleryItemData("Legal"),
			new PaperSizeGalleryItemData("Letter"),
			new PaperSizeGalleryItemData("Tabloid")
		};
		static string GetPaperSizeDescription(PaperSize paperSize, bool isMetric) {
			string widthDesc = PageMargin.GetMarginSideDesc(paperSize.Width, isMetric, GraphicsDpi.HundredthsOfAnInch);
			string heightDesc = PageMargin.GetMarginSideDesc(paperSize.Height, isMetric, GraphicsDpi.HundredthsOfAnInch);
			return string.Format(PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_GalleryItem_PaperSize_Description), widthDesc, heightDesc);
		}
		static string GetPredefinedAlias(string paperName) {
			foreach(PaperSizeGalleryItemData itemData in PredefinedItemsData) {
				if(itemData.PaperName == paperName)
					return itemData.Alias;
			}
			return null;
		}
		#endregion
		const int MaxPaperSizeGalleryRowCount = 7;
		public override PrintPreviewGalleryItem[] Items {
			get {
				PageSettings pageSettings = Logic.PrintingSystem.Extend().PageSettings;
				try {
					PageSettingsHelper.SetPrinterName(pageSettings.PrinterSettings, Logic.PrintingSystem.PageSettings.PrinterName);
				} catch (Win32Exception exception) {
					Tracer.TraceError(NativeSR.TraceSource, exception);
					NotificationService.ShowException<PrintingSystemBase>(Logic.PrintingSystem.Extend().LookAndFeel, Logic.PrintingSystem.Extend().FindForm(), exception);
					return new PrintPreviewGalleryItem[] { };
				}
				PrinterSettings.PaperSizeCollection paperSizes = pageSettings.PrinterSettings.PaperSizes;
				List<PrintPreviewGalleryItem> itemsWithImages = new List<PrintPreviewGalleryItem>();
				List<PrintPreviewGalleryItem> itemsWithoutImages = new List<PrintPreviewGalleryItem>();
				foreach(PaperSize paperSize in paperSizes) {
					if(String.IsNullOrEmpty(paperSize.PaperName) || ((PaperKind)paperSize.RawKind) == PaperKind.Custom) continue;
					string alias = GetPredefinedAlias(paperSize.PaperName);
					Image image = alias != null ? GetImage(alias) : null;
					PrintPreviewGalleryItem item = new PrintPreviewGalleryItem(image, paperSize.PaperName, GetPaperSizeDescription(paperSize, Logic.PrintControl.IsMetric), PrintingSystemCommand.PaperSize, paperSize);
					(image == null ? itemsWithoutImages : itemsWithImages).Add(item);
				}
				PrintPreviewGalleryItem[] items = new PrintPreviewGalleryItem[itemsWithImages.Count + itemsWithoutImages.Count];
				itemsWithImages.CopyTo(items, 0);
				itemsWithoutImages.CopyTo(items, itemsWithImages.Count);
				return items;
			}
		}
		protected override int RowCount { get { return Math.Min(MaxPaperSizeGalleryRowCount, base.RowCount); } }
		public PrintPreviewPaperSizeGalleryDropDown(RibbonPreviewItemsLogic logic, IContainer container)
			: base(logic, container) {
			Gallery.ImageSize = new Size(32, 32);
		}
		protected override bool IsItemChecked(PrintPreviewGalleryItem item) {
			return Logic.PrintingSystem.PageSettings.PaperKind == PaperKind.Custom ?
				Logic.PrintingSystem.PageSettings.PaperName == ((PaperSize)item.CommandParameter).PaperName :
				Logic.PrintingSystem.PageSettings.PaperKind == ((PaperSize)item.CommandParameter).Kind;
		}
	}
	public class PaperSizeGalleryItemData : PrintPreviewGalleryItemData {
		string paperName;
		public override PrintingSystemCommand Command {
			get { throw new NotImplementedException(); }
		}
		public string PaperName {
			get { return paperName; }
		}
		public override string Alias {
			get { return "PaperKind_" + paperName; }
		}
		public PaperSizeGalleryItemData(string paperName) {
			this.paperName = paperName;
		}
	}
}
