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

using System.Collections;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.XtraRichEdit.Utils;
using System;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class InsertTextCommand : WebRichEditUpdateModelCommandBase {
		public InsertTextCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.InsertSimpleRun; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModel() {
			string text = (string)Parameters["text"];
			if (string.IsNullOrEmpty(text)) 
				return;
			ClientTextRunType runType = (ClientTextRunType)Parameters["type"];
			if(runType == ClientTextRunType.TextRun) {
				text = RestoreTextFromRequest(text);
				if(text.Length != (int)Parameters["length"])
					throw new Exception("Text.Length is not equal to Parameters.Length: " + text);
				PieceTable.InsertTextCore(CreateInputPosition(), text);
			}
			else if(runType == ClientTextRunType.LayoutDependentTextRun) {
				var logPosition = new DocumentLogPosition((int)Parameters["position"]);
				var modelPosition = PositionConverter.ToDocumentModelPosition(PieceTable, logPosition);
				PieceTable.InsertLayoutDependentTextRun(modelPosition.ParagraphIndex, logPosition, null);
			}
			else if(runType == ClientTextRunType.FieldCodeStartRun) {
				PieceTable.InsertFieldCodeStartRunCore(CreateInputPosition());
			}
			else if(runType == ClientTextRunType.FieldCodeEndRun) {
				PieceTable.InsertFieldCodeEndRunCore(CreateInputPosition());
			}
			else if(runType == ClientTextRunType.FieldResultEndRun) {
				PieceTable.InsertFieldResultEndRunCore(CreateInputPosition());
			}
			else
				throw new Exception("Get unknown run type");
		}
		InputPosition CreateInputPosition() {
			var logPosition = new DocumentLogPosition((int)Parameters["position"]);
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(PieceTable, logPosition);
			InputPosition pos = new InputPosition(PieceTable);
			pos.LogPosition = position.LogPosition;
			pos.ParagraphIndex = position.ParagraphIndex;
			ApplyCharacterFormatting(pos);
			pos.CharacterStyleIndex = DocumentModel.CharacterStyles.GetStyleIndexByName((string)Parameters["characterStyleName"]);
			return pos;
		}
		void ApplyCharacterFormatting(InputPosition pos) {
			Hashtable maskedProperties = Parameters["characterProperties"] as Hashtable;
			if (maskedProperties != null)
				CopyCharacterPropertiesFromHashtable(pos.CharacterFormatting, maskedProperties);
			else if (Parameters.ContainsKey("characterPropertiesIndex")) {
				int? index = (int?)Parameters["characterPropertiesIndex"];
				if (index.HasValue)
					pos.CharacterFormatting.CopyFrom(DocumentModel.Cache.CharacterFormattingCache[index.Value]);
			}
		}
	}
}
