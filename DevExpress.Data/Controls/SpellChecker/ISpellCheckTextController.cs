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
using System.Globalization;
namespace DevExpress.XtraSpellChecker.Parser {
	public interface ISpellCheckTextController {
		bool CanDoNextStep(Position position);
		bool DeleteWord(ref Position start, ref Position finish);
		string GetWord(Position start, Position finish);
		string GetPreviousWord(Position pos);
		string Text { get; set;}
		char this[Position position] { get; }
		bool ReplaceWord(Position start, Position finish, string word);
		Position GetNextPosition(Position pos);
		Position GetPrevPosition(Position pos);
		Position GetWordStartPosition(Position pos);
		Position GetTextLength(string text);
		bool HasLetters(Position start, Position finish);
		Position GetSentenceStartPosition(Position pos);
		Position GetSentenceEndPosition(Position pos);
	}
	public interface IUndoController {
		IUndoItem GetUndoItemForReplace();
		IUndoItem GetUndoItemForSilentReplace();
		IUndoItem GetUndoItemForDelete();
		IUndoItem GetUndoItemForIgnore();
		IUndoItem GetUndoItemForIgnoreAll();
	}
	public interface IUndoItem {
		void DoUndo();
		Position StartPosition { get; set; }
		Position FinishPosition { get; set; }
		string OldText { get; set; }
		bool NeedRecheckWord { get; }
		bool ShouldUpdateItemPosition { get; }
	}
	public interface ISupportMultiCulture {
		CultureInfo GetCulture(Position start, Position end);
		bool ShouldCheckWord(Position start, Position end);
	}
}
