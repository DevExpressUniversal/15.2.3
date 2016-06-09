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
	#region Revision
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Revision : IWordObject {
		string Author { get; }
		DateTime Date { get; }
		Range Range { get; }
		WdRevisionType Type { get; }
		int Index { get; }
		void Accept();
		void Reject();
		Style Style { get; }
		string FormatDescription { get; }
		Range MovedRange { get; }
		Cells Cells { get; }
	}
	#endregion
	#region Revisions
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Revisions : IWordObject, IEnumerable {
		int Count { get; }
		Revision this[int Index] { get; }
		void AcceptAll();
		void RejectAll();
	}
	#endregion
	#region WdRevisionType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdRevisionType {
		wdNoRevision,
		wdRevisionInsert,
		wdRevisionDelete,
		wdRevisionProperty,
		wdRevisionParagraphNumber,
		wdRevisionDisplayField,
		wdRevisionReconcile,
		wdRevisionConflict,
		wdRevisionStyle,
		wdRevisionReplace,
		wdRevisionParagraphProperty,
		wdRevisionTableProperty,
		wdRevisionSectionProperty,
		wdRevisionStyleDefinition,
		wdRevisionMovedFrom,
		wdRevisionMovedTo,
		wdRevisionCellInsertion,
		wdRevisionCellDeletion,
		wdRevisionCellMerge
	}
	#endregion
}
