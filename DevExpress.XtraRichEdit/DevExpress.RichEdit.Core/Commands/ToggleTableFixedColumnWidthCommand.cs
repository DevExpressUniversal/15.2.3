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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleTableFixedColumnWidthCommand
	public class ToggleTableFixedColumnWidthCommand : ToggleTableAutoFitCommandBase {
		public ToggleTableFixedColumnWidthCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableFixedColumnWidthCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableFixedColumnWidth; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableFixedColumnWidthCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableFixedColumnWidth; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableFixedColumnWidthCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableFixedColumnWidthDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleTableFixedColumnWidthCommandImageName")]
#endif
		public override string ImageName { get { return "TableFixedColumnWidth"; } }
		#endregion
		protected internal override void ApplyTableProperties() {
			TableProperties properties = Table.TableProperties;
			properties.BeginUpdate();
			try {
				properties.TableLayout = TableLayoutType.Fixed;
				if (Table.PreferredWidth.Type != WidthUnitType.Nil) {
					PreferredWidth preferredWidth = properties.PreferredWidth;
					preferredWidth.BeginUpdate();
					try {
						preferredWidth.Type = WidthUnitType.Nil;
						preferredWidth.Value = 0;
					}
					finally {
						preferredWidth.EndUpdate();
					}					
				}
			}
			finally {
				properties.EndUpdate();
			}
		}
		protected internal override void ApplyTableCellProperties() {
		}
		protected override WidthUnitInfo GetTableWidthInfo() {
			return null;
		}
		protected override WidthUnitInfo GetTableCellWidthInfo(TableCell cell, double rowWidth) {
			return null;
		}
		protected override int CalculateRowWidth(TableRow row) {
			return 0;
		}
	}
	#endregion
}
