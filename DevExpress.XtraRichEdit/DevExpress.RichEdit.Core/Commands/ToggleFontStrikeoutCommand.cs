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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleFontStrikeoutCommandBase (abstract class)
	public abstract class ToggleFontStrikeoutCommandBase : ToggleChangeCharacterFormattingCommandBase<StrikeoutType> {
		protected ToggleFontStrikeoutCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal abstract StrikeoutType StrikeoutType { get; }
		protected internal override RunPropertyModifier<StrikeoutType> CreateModifier(ICommandUIState state) {
			StrikeoutType StrikeoutType;
			if (state.Checked)
				StrikeoutType = StrikeoutType.None;
			else
				StrikeoutType = this.StrikeoutType;
			return new RunFontStrikeoutTypeModifier(StrikeoutType);
		}
		protected internal override bool IsCheckedValue(StrikeoutType value) {
			return value == this.StrikeoutType;
		}
	}
	#endregion
	#region ToggleFontStrikeoutCommand
	public class ToggleFontStrikeoutCommand : ToggleFontStrikeoutCommandBase {
		public ToggleFontStrikeoutCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontStrikeoutCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ToggleFontStrikeout; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontStrikeoutCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFontStrikeout; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontStrikeoutCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFontStrikeoutDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontStrikeoutCommandImageName")]
#endif
		public override string ImageName { get { return "Strikeout"; } }
		protected internal override StrikeoutType StrikeoutType { get { return StrikeoutType.Single; } }
		#endregion
	}
	#endregion
	#region ToggleFontDoubleStrikeoutCommand
	public class ToggleFontDoubleStrikeoutCommand : ToggleFontStrikeoutCommandBase {
		public ToggleFontDoubleStrikeoutCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontDoubleStrikeoutCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ToggleFontDoubleStrikeout; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontDoubleStrikeoutCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFontDoubleStrikeout; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontDoubleStrikeoutCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFontDoubleStrikeoutDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontDoubleStrikeoutCommandImageName")]
#endif
		public override string ImageName { get { return "StrikeoutDouble"; } }
		protected internal override StrikeoutType StrikeoutType { get { return StrikeoutType.Double; } }
		#endregion
	}
	#endregion
}
