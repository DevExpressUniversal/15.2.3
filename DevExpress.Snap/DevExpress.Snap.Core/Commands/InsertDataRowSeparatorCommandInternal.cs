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

using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Commands {
	public class InsertPageBreakSeparatorCommandInternal : InternalDocumentModelTemplateCommandBase {
		public InsertPageBreakSeparatorCommandInternal(SnapDocumentModel sourceDocumentModel)
			: base(sourceDocumentModel) {
		}
		protected override void CreateModelCore(DocumentModel result) {
			new SeparatorTemplateExecutor().InsertPageBreakSeparator(result.MainPieceTable);
		}
	}
	public class InsertEmptyParagraphSeparatorCommandInternal : InternalDocumentModelTemplateCommandBase {
		public InsertEmptyParagraphSeparatorCommandInternal(SnapDocumentModel sourceDocumentModel)
			: base(sourceDocumentModel) {
		}
		protected override void CreateModelCore(DocumentModel result) {
			new SeparatorTemplateExecutor().InsertEmptyParagraphSeparator(result.MainPieceTable);
		}
	}
	public class InsertNoneSeparatorCommandInternal : ITemplateModifier {		
		public void Execute(InstructionController controller, string templateSwitch) {
			controller.RemoveSwitch(templateSwitch);
		}
	}
	public class InsertEmptyRowSeparatorCommandInternal : InternalDocumentModelTemplateCommandBase {
		public InsertEmptyRowSeparatorCommandInternal(SnapDocumentModel sourceDocumentModel)
			: base(sourceDocumentModel) {
		}
		protected override DocumentModel CreateModel(SnapPieceTable pieceTable, DocumentLogInterval templateInterval, IFieldInfo listFieldInfo) {
			return TableCommandsHelper.CreateModel(SourceDocumentModel, pieceTable, templateInterval, null, TableCommandsHelper.MergeTablesTotally, null);
		}
	}
	public abstract class InsertSectionBreakSeparatorCommandInternalBase : InternalDocumentModelTemplateCommandBase {
		protected InsertSectionBreakSeparatorCommandInternalBase(SnapDocumentModel sourceDocumentModel)
			: base(sourceDocumentModel) {
		}
		protected internal abstract SectionStartType StartType { get; }
		protected override void CreateModelCore(DocumentModel result) {
			new SeparatorTemplateExecutor().InsertSectionSeparator(result.MainPieceTable, StartType);
		}
	}
	public class InsertSectionBreakNextPageSeparatorCommandInternal : InsertSectionBreakSeparatorCommandInternalBase {
		public InsertSectionBreakNextPageSeparatorCommandInternal(SnapDocumentModel sourceDocumentModel)
			: base(sourceDocumentModel) {
		}
		protected internal override SectionStartType StartType { get { return SectionStartType.NextPage; } }
	}
	public class InsertSectionBreakEvenPageSeparatorCommandInternal : InsertSectionBreakSeparatorCommandInternalBase {
		public InsertSectionBreakEvenPageSeparatorCommandInternal(SnapDocumentModel sourceDocumentModel)
			: base(sourceDocumentModel) {
		}
		protected internal override SectionStartType StartType { get { return SectionStartType.EvenPage; } }
	}
	public class InsertSectionBreakOddPageSeparatorCommandInternal : InsertSectionBreakSeparatorCommandInternalBase {
		public InsertSectionBreakOddPageSeparatorCommandInternal(SnapDocumentModel sourceDocumentModel)
			: base(sourceDocumentModel) {
		}
		protected internal override SectionStartType StartType { get { return SectionStartType.OddPage; } }
	}
}
