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
	#region ToggleFontUnderlineCommandBase (abstract class)
	public abstract class ToggleFontUnderlineCommandBase : ToggleChangeCharacterFormattingCommandBase<UnderlineType> {
		protected ToggleFontUnderlineCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal abstract UnderlineType UnderlineType { get; }
		protected internal override RunPropertyModifier<UnderlineType> CreateModifier(ICommandUIState state) {
			UnderlineType underlineType;
			if (state.Checked)
				underlineType = UnderlineType.None;
			else
				underlineType = this.UnderlineType;
			return new RunFontUnderlineTypeModifier(underlineType);
		}
		protected internal override bool IsCheckedValue(UnderlineType value) {
			return value == this.UnderlineType;
		}
	}
	#endregion
	#region ToggleFontUnderlineCommand
	public class ToggleFontUnderlineCommand : ToggleFontUnderlineCommandBase {
		public ToggleFontUnderlineCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontUnderlineCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ToggleFontUnderline; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontUnderlineCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFontUnderline; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontUnderlineCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFontUnderlineDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontUnderlineCommandImageName")]
#endif
		public override string ImageName { get { return "Underline"; } }
		protected internal override UnderlineType UnderlineType { get { return UnderlineType.Single; } }
		#endregion
	}
	#endregion
	#region ToggleFontDoubleUnderlineCommand
	public class ToggleFontDoubleUnderlineCommand : ToggleFontUnderlineCommandBase {
		public ToggleFontDoubleUnderlineCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontDoubleUnderlineCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ToggleFontDoubleUnderline; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontDoubleUnderlineCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFontDoubleUnderline; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontDoubleUnderlineCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFontDoubleUnderlineDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFontDoubleUnderlineCommandImageName")]
#endif
		public override string ImageName { get { return "UnderlineDouble"; } }
		protected internal override UnderlineType UnderlineType { get { return UnderlineType.Double; } }
		#endregion
	}
	#endregion
}
