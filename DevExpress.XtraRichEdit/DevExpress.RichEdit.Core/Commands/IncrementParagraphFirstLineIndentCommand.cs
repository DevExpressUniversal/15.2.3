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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.XtraRichEdit.Commands {
	#region IncrementParagraphFirstLineIndentCommand
	public class IncrementParagraphFirstLineIndentCommand : ChangeParagraphIndentCommandBase<int> {
		public IncrementParagraphFirstLineIndentCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal CaretPosition CaretPosition { get { return ActiveView.CaretPosition; } }
		protected internal override void ModifyDocumentModel(ICommandUIState state) {
			CaretPosition.Update(DocumentLayoutDetailsLevel.Column);
			base.ModifyDocumentModel(state);
		}
		protected internal override ParagraphPropertyModifier<int> CreateModifier(ICommandUIState state) {
			int maxValue = Int32.MaxValue / 4;
			if (CaretPosition != null && CaretPosition.LayoutPosition.IsValid(DocumentLayoutDetailsLevel.Column))
				maxValue = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(CaretPosition.LayoutPosition.Column.Bounds.Width);
			DocumentModel documentModel = DocumentModel;
			Paragraph paragraph = ActivePieceTable.Paragraphs[StartParagraphIndex];
			FillTabsList();
			int nearestRightTab = GetNearRightTab(paragraph.FirstLineIndent);
			int defaultTabWidth = documentModel.DocumentProperties.DefaultTabWidth;
			if (defaultTabWidth < nearestRightTab || nearestRightTab == paragraph.FirstLineIndent)
				return new ParagraphFirstLineIndentModifier(defaultTabWidth, maxValue);
			else
				return new ParagraphFirstLineIndentModifier(nearestRightTab, maxValue);
		}
	}
	#endregion
}
