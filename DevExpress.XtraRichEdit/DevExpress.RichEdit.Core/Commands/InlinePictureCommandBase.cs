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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.XtraRichEdit.Commands {
	#region RectangularObjectCommandBase<T> (abstract class)
	public abstract class RectangularObjectCommandBase<T> : SelectionBasedPropertyChangeCommandBase {
		protected RectangularObjectCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			SelectionFieldHelper fieldHelper = new SelectionFieldHelper(DocumentModel.Selection);
			if (end.LogPosition - start.LogPosition != 1 && !fieldHelper.ShouldCreateRectangularObjectSelection())
				return DocumentModelChangeActions.None;
			RunIndex startIndex = CalculateStartRunIndex(start, fieldHelper);
			IRectangularObject run = ActivePieceTable.Runs[startIndex].GetRectangularObject();
			if (run == null)
				return DocumentModelChangeActions.None;
			RectangularObjectPropertyModifier<T> modifier = CreateModifier(state);
			modifier.ModifyRectangularObject(run, startIndex);
			return DocumentModelChangeActions.None;
		}
		RunIndex CalculateStartRunIndex(DocumentModelPosition start, SelectionFieldHelper fieldHelper) {
			if (fieldHelper.ShouldCreateRectangularObjectSelection())
				return fieldHelper.GetFieldResultStart();
			return start.RunIndex;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
		protected internal abstract RectangularObjectPropertyModifier<T> CreateModifier(ICommandUIState state);
	}
	#endregion
	#region InlinePictureCommandBase<T> (abstract class)
	public abstract class InlinePictureCommandBase<T> : SelectionBasedPropertyChangeCommandBase {
		protected InlinePictureCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			if (end.LogPosition - start.LogPosition != 1)
				return DocumentModelChangeActions.None;
			InlinePictureRun run = ActivePieceTable.Runs[start.RunIndex] as InlinePictureRun;
			if (run == null)
				return DocumentModelChangeActions.None;
			InlinePictureRunPropertyModifier<T> modifier = CreateModifier(state);
			modifier.ModifyPictureRun(run, start.RunIndex);
			return DocumentModelChangeActions.None;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
		protected internal abstract InlinePictureRunPropertyModifier<T> CreateModifier(ICommandUIState state);
	}
	#endregion
}
