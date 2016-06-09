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
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetCommandBarButtonGalleryDropDownItem
	public class SpreadsheetCommandBarButtonGalleryDropDownItem : ControlCommandBarButtonGalleryDropDownItem<SpreadsheetControl, SpreadsheetCommandId> {
		string commandName = String.Empty;
		SpreadsheetCommandId id;
		public static SpreadsheetCommandBarButtonGalleryDropDownItem Create(SpreadsheetCommandId command) {
			SpreadsheetCommandBarButtonGalleryDropDownItem result = new SpreadsheetCommandBarButtonGalleryDropDownItem();
			result.CommandName = SpreadsheetCommandId.GetCommandName(command);
			return result;
		}
		public SpreadsheetCommandBarButtonGalleryDropDownItem()
			: base() {
		}
		#region Properties
		protected virtual bool CorrectItemSizeWhenUseHighDPI { get { return true; } }
		protected override SpreadsheetCommandId CommandId { get { return id; } }
		[DefaultValue(""), Localizable(false)]
		public string CommandName {
			get { return commandName; }
			set {
				if (commandName == value)
					return;
				commandName = value;
				id = SpreadsheetCommandId.GetCommandId(commandName);
			}
		}
		protected override Size GalleryImageSize { get { return new Size(32, 32); } }
		#endregion
		protected override void AfterLoad() {
			if (!DesignMode && CorrectItemSizeWhenUseHighDPI) {
				Size newSize = HighDpiUnitCalculator.GetCorrectedSize(GalleryDropDown.Gallery.ItemSize);
				GalleryDropDown.Gallery.ItemSize = newSize;
			}
			base.AfterLoad();
		}
	}
	#endregion
	#region HighDpiUnitCalculator
	public static class HighDpiUnitCalculator {
		public static Size GetCorrectedSize(Size currentSize) {
			int newWidth = GetCorrectedWidth(currentSize.Width);
			int newHeight = GetCorrectedHeight(currentSize.Height);
			return new Size(newWidth, newHeight);
		}
		public static int GetCorrectedWidth(int currentWidth) {
			double result = currentWidth * DocumentModel.DpiX / 96.0;
			return (int)result;
		}
		public static int GetCorrectedHeight(int currentHeight) {
			double result = currentHeight * DocumentModel.DpiY / 96.0;
			return (int)result;
		}
	}
	#endregion
}
