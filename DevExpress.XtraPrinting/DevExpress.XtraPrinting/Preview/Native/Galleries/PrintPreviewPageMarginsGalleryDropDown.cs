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
	public class PrintPreviewPageMarginsGalleryDropDown : PrintPreviewGalleryDropDown {
		#region static
		public static readonly PrintPreviewGalleryItemData[] PredefinedItemsData = new PrintPreviewGalleryItemData[] {
			new PageMarginsGalleryItemData(new MarginsF(new Margins(100, 100, 100, 100)), "Normal"),
			new PageMarginsGalleryItemData(new MarginsF(new Margins(50, 50, 50, 50)), "Narrow"),
			new PageMarginsGalleryItemData(new MarginsF(new Margins(75, 75, 100, 100)), "Moderate"),
			new PageMarginsGalleryItemData(new MarginsF(new Margins(200, 200, 100, 100)), "Wide")
		};
		static string GetMarginsDescription(MarginsF margins, bool isMetric) {
			string topDesc = PageMargin.GetMarginSideDesc(margins.Top, isMetric, GraphicsDpi.Document);
			string bottomDesc = PageMargin.GetMarginSideDesc(margins.Bottom, isMetric, GraphicsDpi.Document);
			string leftDesc = PageMargin.GetMarginSideDesc(margins.Left, isMetric, GraphicsDpi.Document);
			string rightDesc = PageMargin.GetMarginSideDesc(margins.Right, isMetric, GraphicsDpi.Document);
			return string.Format(PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_GalleryItem_PageMargins_Description), topDesc, bottomDesc, leftDesc, rightDesc);
		}
		#endregion
		public PrintPreviewPageMarginsGalleryDropDown(RibbonPreviewItemsLogic logic, IContainer container)
			: base(logic, PredefinedItemsData, container) {
			Gallery.ImageSize = new Size(48, 48);
		}
		protected override PrintPreviewGalleryItem CreateGalleryItem(PrintPreviewGalleryItemData itemData) {
			PageMarginsGalleryItemData data = (PageMarginsGalleryItemData)itemData;
			return new PrintPreviewGalleryItem(GetImage(data.Alias), PrintRibbonControllerConfigurator.GetGalleryItemCaption(data.Alias), GetMarginsDescription(data.Argument, Logic.PrintControl.IsMetric), data.Command, data.Argument);
		}
		protected override bool IsItemChecked(PrintPreviewGalleryItem item) {
			return Logic.PrintingSystem != null && object.Equals(Logic.PrintingSystem.PageSettings.MarginsF, item.CommandParameter);
		}
		protected override void OnBeforePopup(CancelEventArgs e) {
			base.OnBeforePopup(e);
			ItemLinks.Clear();
			ItemLinks.Add(Logic.GetBarItemByCommand(PrintingSystemCommand.PageSetup));
		}
	}
	public class PageMarginsGalleryItemData : PrintPreviewGalleryItemData {
		MarginsF argument;
		string standardMargins;
		public override PrintingSystemCommand Command {
			get { return PrintingSystemCommand.PageMargins; }
		}
		public MarginsF Argument {
			get { return argument; }
		}
		public override string Alias {
			get { return Command.ToString() + standardMargins.ToString(); }
		}
		public PageMarginsGalleryItemData(MarginsF margins, string standardMargins) {
			this.argument = margins;
			this.standardMargins = standardMargins;
		}
	}
}
