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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands.Helper;
using DevExpress.XtraRichEdit.Internal;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertSimpleNumerationForParagraphCommand
	public class InsertSimpleListCommand : InsertMultiLevelListCommand {
		public InsertSimpleListCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertSimpleListCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertSimpleList; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertSimpleListCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertSimpleListDescription; } }
		protected internal override NumberingType NumberingListType { get { return NumberingType.Simple; } }
		#endregion
		protected internal override void FillParagraphsLevelIndex(List<ParagraphInterval> paragraphIntervals) {
			if (EqualIndent) {
				int count = paragraphIntervals.Count;
				for (int paragraphIntervalIndex = 0; paragraphIntervalIndex < count; paragraphIntervalIndex++) {
					ParagraphInterval paragraphInterval = paragraphIntervals[paragraphIntervalIndex];
					ParagraphIndex startParagraphIndex = paragraphInterval.Start;
					ParagraphIndex endParagraphIndex = paragraphInterval.End;
					for (ParagraphIndex i = startParagraphIndex; i <= endParagraphIndex; i++)
						ParagraphsLevelIndex.Add(i, 0);
					if (!ContinueList)
						AssignLevelsIndents(startParagraphIndex);
				}
			}
			else
				base.FillParagraphsLevelIndex(paragraphIntervals);
		}
		protected override void StoreOriginalLevelLeftIndent(IListLevel level) {
			level.ListLevelProperties.OriginalLeftIndent = level.ParagraphProperties.LeftIndent;
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Numbering.Simple);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
}
