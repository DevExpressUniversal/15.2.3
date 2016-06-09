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
using System.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region BoxInfo
	public class BoxInfo  {
		FormatterPosition startPos;
		FormatterPosition endPos;
		ParagraphIteratorResult iteratorResult;
		Size size;
		Box box;
		public FormatterPosition StartPos { get { return startPos; } set { startPos = value; } }
		public FormatterPosition EndPos { get { return endPos; } set { endPos = value; } }
		public ParagraphIteratorResult IteratorResult { get { return iteratorResult; } set { iteratorResult = value; } }
		public Size Size { get { return size; } set { size = value; } }
		public virtual bool ForceUpdateCurrentRowHeight { get { return false; } }
		internal Box Box { get { return box; } set { box = value; } }
		public virtual TextRunBase GetRun(PieceTable pieceTable) {
			return pieceTable.Runs[startPos.RunIndex];
		}
		public virtual FontInfo GetFontInfo(PieceTable pieceTable) {
			return pieceTable.DocumentModel.FontCache[GetRun(pieceTable).FontCacheIndex];
		}
	}
	#endregion
	#region NumberingListBoxInfo
	public class NumberingListBoxInfo : BoxInfo {
		public override bool ForceUpdateCurrentRowHeight { get { return true; } }
		public override FontInfo GetFontInfo(PieceTable pieceTable) {
			return Box.GetFontInfo(pieceTable);
		}
	}
	#endregion
}
