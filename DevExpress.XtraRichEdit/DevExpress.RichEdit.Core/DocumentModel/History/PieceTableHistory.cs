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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Model.History;
namespace DevExpress.XtraRichEdit.Model.History {
	public class ChangePieceTableHistoryItem : RichEditHistoryItem {
		PieceTable newPieceTable;
		Section section;
		readonly IRichEditControl control;
		public ChangePieceTableHistoryItem(PieceTable pieceTable, IRichEditControl control)
			: base(pieceTable) {
				this.control = control;
		}
		public PieceTable NewPieceTable { get { return newPieceTable; } set { newPieceTable = value; } }
		public Section Section { get { return section; } set { section = value; } }
		protected override void UndoCore() {
			ChangeActivePieceTable(PieceTable, Section);
		}
		protected override void RedoCore() {
			ChangeActivePieceTable(NewPieceTable, Section);
		}
		protected void ChangeActivePieceTable(PieceTable pieceTable, Section section) {
			ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(control, pieceTable, section, 0);
			command.Execute();
		}
	}
}
