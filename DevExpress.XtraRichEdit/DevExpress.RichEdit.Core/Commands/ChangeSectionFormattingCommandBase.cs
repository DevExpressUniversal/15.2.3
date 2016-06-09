﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeSectionFormattingCommandBase<T> (abstract class)
	public abstract class ChangeSectionFormattingCommandBase<T> : SelectionBasedPropertyChangeCommandBase {
		protected ChangeSectionFormattingCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			SectionPropertyModifier<T> modifier = CreateModifier(state);
			ChangeSectionFormatting(start.LogPosition, end.LogPosition, modifier);
			return DocumentModelChangeActions.None;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Sections);
			ApplyDocumentProtectionToSelectedSections(state);
		}
		protected internal virtual void ChangeSectionFormatting(DocumentLogPosition logPositionFrom, DocumentLogPosition logPositionTo, SectionPropertyModifier<T> modifier) {
			int length = Math.Max(1, logPositionTo - logPositionFrom);
			ActivePieceTable.ApplySectionFormatting(logPositionFrom, length, modifier);
		}
		protected internal abstract SectionPropertyModifier<T> CreateModifier(ICommandUIState state);
	}
	#endregion
}
