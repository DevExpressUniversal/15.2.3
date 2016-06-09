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
using System.Drawing;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	public class ChangeTableCellsShadingCommand : ChangeTableCellsCommandBase {
		Color color;
		public ChangeTableCellsShadingCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeTableCellsShadingCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeTableCellShading; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeTableCellsShadingCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeTableCellShadingDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeTableCellsShadingCommandImageName")]
#endif
		public override string ImageName { get { return "Shading"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeTableCellsShadingCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeTableCellsShading; } }
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<Color> valueBasedState = state as IValueBasedCommandUIState<Color>;
			if (valueBasedState == null)
				return;
			this.color = valueBasedState.Value;
			if(DXColor.IsEmpty(color))
				this.color = DXColor.White;
			base.ForceExecute(state);
		}
		protected internal override void ModifyCell(TableCell tableCell) {
			tableCell.Properties.BackgroundColor = color;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<Color>();
		}
	}
}
