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
namespace DevExpress.XtraPrinting.Preview.Native.Galleries {
	public class PrintPreviewPageOrientationGalleryDropDown : PrintPreviewGalleryDropDown {
		public static readonly PrintPreviewGalleryItemData[] PredefinedItemsData = new PrintPreviewGalleryItemData[] {
			new PageOrientationGalleryItemData(PageOrientation.Portrait),
			new PageOrientationGalleryItemData(PageOrientation.Landscape)
		};
		public PrintPreviewPageOrientationGalleryDropDown(RibbonPreviewItemsLogic logic, IContainer container)
			: base(logic, PredefinedItemsData, container) {
			Gallery.ImageSize = new Size(32, 32);
		}
		protected override bool IsItemChecked(PrintPreviewGalleryItem item) {
			return Logic.PrintingSystem != null && Logic.PrintingSystem.PageSettings.Landscape == ((PageOrientation)item.CommandParameter == PageOrientation.Landscape);
		}
		protected override PrintPreviewGalleryItem CreateGalleryItem(PrintPreviewGalleryItemData itemData) {
			PageOrientationGalleryItemData data = (PageOrientationGalleryItemData)itemData;
			return new PrintPreviewGalleryItem(GetImage(data.Alias), PrintRibbonControllerConfigurator.GetGalleryItemCaption(data.Alias), PrintRibbonControllerConfigurator.GetGalleryItemDescription(data.Alias), data.Command, data.Argument);
		}
	}
	public class PageOrientationGalleryItemData : PrintPreviewGalleryItemData {
		PageOrientation argument;
		public override PrintingSystemCommand Command {
			get { return PrintingSystemCommand.PageOrientation; }
		}
		public PageOrientation Argument {
			get { return argument; }
		}
		public override string Alias {
			get { return Command.ToString() + Argument.ToString(); }
		}
		public PageOrientationGalleryItemData(PageOrientation orientation) {
			this.argument = orientation;
		}
	}
}
