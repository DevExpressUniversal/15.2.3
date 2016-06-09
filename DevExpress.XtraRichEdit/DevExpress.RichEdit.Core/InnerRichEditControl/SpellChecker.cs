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
using DevExpress.XtraSpellChecker;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.SpellChecker;
using DevExpress.XtraRichEdit.Model;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Internal {
	public partial class InnerRichEditControl {
		ISpellChecker spellChecker;
		#region SpellChecker
		public override ISpellChecker SpellChecker {
			get { return spellChecker; }
			set {
				if (spellChecker == value)
					return;
				spellChecker = value;
				OnSpellCheckerChanged();
			}
		}
		#endregion
		#region SpellChecker
		public virtual void OnSpellCheckerChanged() {
			if (DocumentModel == null)
				return;
			DocumentModel.BeginUpdate();
			try {
				RecreateSpellCheckerController();
				RecreateSpellCheckerManager();
				DocumentModel.ApplyChangesCore(DocumentModel.MainPieceTable, DocumentModelChangeActions.ResetSecondaryLayout, RunIndex.Zero, RunIndex.MaxValue);
			}
			finally {
				DocumentModel.EndUpdate();
			}
			RedrawEnsureSecondaryFormattingComplete();
		}
		protected internal virtual void RecreateSpellCheckerController() {
			if (Formatter == null)
				return;
			Formatter.BeginDocumentUpdate();
			try {
				Formatter.SpellCheckerController.Dispose();
				Formatter.SpellCheckerController = CreateSpellCheckerController();
			}
			finally {
				Formatter.EndDocumentUpdate();
			}
		}
		protected internal void SubscribeToSpellCheckerEvents() {
			if (Formatter != null && Formatter.SpellCheckerController != null)
				Formatter.SpellCheckerController.SubscribeToEvents();
		}
		protected internal void UnsubscribeToSpellCheckerEvents() {
			if (Formatter != null && Formatter.SpellCheckerController != null)
				Formatter.SpellCheckerController.UnsubscribeToEvents();
		}
		protected internal virtual SpellCheckerControllerBase CreateSpellCheckerController() {
			ISyntaxCheckService syntaxCheckService = this.DocumentModel.GetService<ISyntaxCheckService>();
			if (syntaxCheckService != null)
				return new SyntaxCheckController(this, syntaxCheckService);
			if (SpellChecker != null)
				return new SpellCheckerController(this);
			else
				return new EmptySpellCheckerController();
		}
		protected internal virtual void RecreateSpellCheckerManager() {
			List<PieceTable> pieceTables = DocumentModel.GetPieceTables(false);
			int count = pieceTables.Count;
			for (int i = 0; i < count; i++) {
				PieceTable pieceTable = pieceTables[i];
				if (pieceTable.SpellCheckerManager != null)
					pieceTable.SpellCheckerManager.Dispose();
				pieceTable.SpellCheckerManager = CreateSpellCheckerManager(pieceTable);
				pieceTable.SpellCheckerManager.Initialize();
			}
		}
		protected internal virtual SpellCheckerManager CreateSpellCheckerManager(PieceTable pieceTable) {
			if (SpellChecker != null)
				return new SpellCheckerManager(pieceTable);
			else
				return new EmptySpellCheckerManager(pieceTable);
		}
		#endregion
	}
}
