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
	public abstract class PrintPreviewGalleryDropDown : PrintPreviewGalleryDropDownBase {
		PrintPreviewGalleryItemData[] itemsData;
		PrintPreviewGalleryItem[] items;
		public override PrintPreviewGalleryItem[] Items {
			get {
				if(items == null) 
					CreateItems();
				return items;
			}
		}
		protected PrintPreviewGalleryDropDown(RibbonPreviewItemsLogicBase logic, PrintPreviewGalleryItemData[] itemsData, IContainer container)
			: base(logic, container) {
			this.itemsData = itemsData;
		}
		protected void CreateItems() {
			items = new PrintPreviewGalleryItem[itemsData.Length];
			for(int i = 0; i < itemsData.Length; i++)
				items[i] = CreateGalleryItem(itemsData[i]);
		}
		protected abstract PrintPreviewGalleryItem CreateGalleryItem(PrintPreviewGalleryItemData itemData);
	}
	public abstract class PrintPreviewGalleryItemData {
		public abstract PrintingSystemCommand Command { get; }
		public abstract string Alias { get; }
	}
}
