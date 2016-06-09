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
using System.Collections;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class LoadPieceTableCommand : WebRichEditLoadPieceTableCommandBase {
		protected int Start { get; set; }
		protected int Length { get; set; }
		public override CommandType Type { get { return CommandType.FixedLengthText; } }
		protected override bool IsEnabled() { return true; }
		public LoadPieceTableCommand(WebRichEditLoadModelCommandBase parentCommand, PieceTable pieceTable, int length = 0)
			: base(parentCommand, new Hashtable(), pieceTable) {
				Start = 0;
				Length = length;
		}
		protected override void FillHashtable(Hashtable result) {
			result["fields"] = new FieldListCommand(this, PieceTable).Execute();
			result["tables"] = new LoadTablesCommand(this, PieceTable).Execute();
			result["bookmarks"] = new LoadBookmarksCommand(this, PieceTable).Execute();
			result["fixedLengthFormattedText"] = new LoadFixedLengthTextCommand(this, PieceTable, Start, Length).Execute();
		}
	}
	public class LoadFixedLengthTextCommand : FixedLengthTextCommand { 
		public LoadFixedLengthTextCommand(WebRichEditLoadModelCommandBase parentCommand, PieceTable pieceTable, int start = 0, int length = 0)
			: base(parentCommand, new Hashtable() { { "serverParams", new Hashtable { { "start", start } } } }, pieceTable) {
			if(length > 0)
				Length = length;
		}
	}
	public class FixedLengthTextCommand : WebRichEditLoadPieceTableCommandBase {
		public FixedLengthTextCommand(CommandManager commandManager, Hashtable parameters) 
			: base(commandManager, parameters) { }
		public FixedLengthTextCommand(CommandManager commandManager, Hashtable parameters, PieceTable pieceTable)
			: base(commandManager, parameters, pieceTable) { }
		public FixedLengthTextCommand(WebRichEditLoadModelCommandBase parentCommand, Hashtable parameters, PieceTable pieceTable)
			: base(parentCommand, parameters, pieceTable) { }
		public override CommandType Type { get { return CommandType.FixedLengthText; } }
		protected override bool IsEnabled() { return true; }
		private const int DefaultTextLength = Chunk.DefaultMaxBufferSize * 64;
		public DocumentLogPosition Start { get; private set; }
		public int Length { get; set; }
		public int? MaxBufferSize { get; set; }
		public int PieceTableID { get; set; }
		protected override void FillHashtable(Hashtable result) {
			Start = new DocumentLogPosition(Convert.ToInt32(Parameters["start"]));
			if (Parameters["maxBufferSize"] != null)
				MaxBufferSize = Convert.ToInt32(Parameters["maxBufferSize"]);
			else if(!MaxBufferSize.HasValue)
				MaxBufferSize = Chunk.DefaultMaxBufferSize;
			Length = Parameters["length"] != null ? Convert.ToInt32(Parameters["length"]) : DefaultTextLength;
			RichEditServerResponseGenerator generator = new RichEditServerResponseGenerator(PieceTable);
			RichEditServerResponse response = generator.GetResponse(WorkSession.FontInfoCache, Start, Length, MaxBufferSize.Value);
			response.FillHashtable(result);
		}
	}
}
