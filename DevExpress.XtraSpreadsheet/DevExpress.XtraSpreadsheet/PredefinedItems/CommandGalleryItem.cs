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
using DevExpress.Utils;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetCommandGalleryItem
	public class SpreadsheetCommandGalleryItem : ControlCommandGalleryItem<SpreadsheetControl, SpreadsheetCommandId> {
		#region Fields
		string commandName = String.Empty;
		SpreadsheetCommandId id;
		#endregion
		public static SpreadsheetCommandGalleryItem Create(SpreadsheetCommandId command) {
			return Create(command, false, false, false);
		}
		public static SpreadsheetCommandGalleryItem Create(SpreadsheetCommandId command, bool captionAsValue, bool alwaysUpdateDescription, bool isEmptyHint) {
			SpreadsheetCommandGalleryItem item = new SpreadsheetCommandGalleryItem();
			item.CommandName = SpreadsheetCommandId.GetCommandName(command);
			item.CaptionAsValue = captionAsValue;
			item.AlwaysUpdateDescription = alwaysUpdateDescription;
			item.IsEmptyHint = isEmptyHint;
			return item;
		}
		public SpreadsheetCommandGalleryItem()
			: base() {
		}
		#region Properties
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
		#endregion
	}
	#endregion
}
