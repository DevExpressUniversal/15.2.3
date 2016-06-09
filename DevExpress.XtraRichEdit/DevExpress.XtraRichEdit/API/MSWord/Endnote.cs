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
using System.CodeDom.Compiler;
using System.Collections;
namespace DevExpress.XtraRichEdit.API.Word {
	#region Endnote
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Endnote : IWordObject {
		Range Range { get; }
		Range Reference { get; }
		int Index { get; }
		void Delete();
	}
	#endregion
	#region Endnotes
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Endnotes : IWordObject, IEnumerable {
		int Count { get; }
		WdEndnoteLocation Location { get; set; }
		WdNoteNumberStyle NumberStyle { get; set; }
		int StartingNumber { get; set; }
		WdNumberingRule NumberingRule { get; set; }
		Range Separator { get; }
		Range ContinuationSeparator { get; }
		Range ContinuationNotice { get; }
		Endnote this[int Index] { get; }
		Endnote Add(Range Range, ref object Reference, ref object Text);
		void Convert();
		void SwapWithFootnotes();
		void ResetSeparator();
		void ResetContinuationSeparator();
		void ResetContinuationNotice();
	}
	#endregion
	#region EndnoteOptions
	[GeneratedCode("Suppress FxCop check", "")]
	public interface EndnoteOptions : IWordObject {
		WdEndnoteLocation Location { get; set; }
		WdNoteNumberStyle NumberStyle { get; set; }
		int StartingNumber { get; set; }
		WdNumberingRule NumberingRule { get; set; }
	}
	#endregion
	#region WdEndnoteLocation
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdEndnoteLocation {
		wdEndOfSection,
		wdEndOfDocument
	}
	#endregion
}
