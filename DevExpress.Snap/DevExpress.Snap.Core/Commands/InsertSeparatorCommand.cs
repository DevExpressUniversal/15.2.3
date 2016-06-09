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

using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.Snap.Core.Commands {
	[CommandLocalization(Localization.SnapStringId.InsertGroupSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertGroupSeparatorCommand_Description)]
	public class InsertGroupSeparatorCommand : DropDownCommandBase {
		public InsertGroupSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertGroupSeparator; } }
		public override string ImageName { get { return "Separator"; } }
		public override RichEditCommandId[] GetChildCommandIds() {
			return new RichEditCommandId[] {
				SnapCommandId.InsertNoneGroupSeparator,
				SnapCommandId.InsertPageBreakGroupSeparator,
				SnapCommandId.InsertEmptyParagraphGroupSeparator,
				SnapCommandId.InsertEmptyRowGroupSeparator,
				SnapCommandId.InsertSectionBreakNextPageGroupSeparator,
				SnapCommandId.InsertSectionBreakEvenPageGroupSeparator,
				SnapCommandId.InsertSectionBreakOddPageGroupSeparator				
			};
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertDataRowSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertDataRowSeparatorCommand_Description)]
	public class InsertDataRowSeparatorCommand : DropDownCommandBase {
		public InsertDataRowSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertDataRowSeparator; } }
		public override string ImageName { get { return "SeparatorList"; } }
		public override RichEditCommandId[] GetChildCommandIds() {
			return new RichEditCommandId[] {
				SnapCommandId.InsertNoneDataRowSeparator,
				SnapCommandId.InsertPageBreakDataRowSeparator,
				SnapCommandId.InsertEmptyParagraphDataRowSeparator,
				SnapCommandId.InsertEmptyRowDataRowSeparator,
				SnapCommandId.InsertSectionBreakNextPageDataRowSeparator,
				SnapCommandId.InsertSectionBreakEvenPageDataRowSeparator,
				SnapCommandId.InsertSectionBreakOddPageDataRowSeparator				
			};
		}
	}
}
