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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region GoToPageFooterCommand (abstract class)
	public class GoToPageFooterCommand : GoToPageHeaderFooterCommand<SectionFooter> {
		public GoToPageFooterCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPageFooterCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_GoToPageFooter; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPageFooterCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_GoToPageFooterDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPageFooterCommandImageName")]
#endif
		public override string ImageName { get { return "GoToFooter"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPageFooterCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.GoToPageFooter; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			SectionHeader header = ActivePieceTable.ContentType as SectionHeader;
			if (header != null)
				return new InsertPageFooterCoreCommand(Control, header.Type);
			else
				return null;
		}
		protected internal override SectionFooter GetCorrespondingHeaderFooter() {
			SectionHeader header = ActivePieceTable.ContentType as SectionHeader;
			if (header != null) {
				Section section = DocumentModel.GetActiveSectionBySelectionEnd();
				return section.GetCorrespondingFooter(header);
			}
			else
				return null;
		}
	}
	#endregion
}
