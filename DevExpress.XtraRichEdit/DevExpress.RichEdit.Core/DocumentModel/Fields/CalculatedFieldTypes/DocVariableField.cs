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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraRichEdit.Model;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit.Fields {
	#region DocVariableField
	public class DocVariableField : CalculatedFieldBase {
		#region Fields
		bool isUpdateFieldOperationTypeLoad = true;
		#endregion
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "DOCVARIABLE";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument();
		public static CalculatedFieldBase Create() {
			return new DocVariableField();
		}
		#endregion
		string variableName;
		readonly ArgumentCollection arguments = new ArgumentCollection();
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			variableName = instructions.GetArgumentAsString(0);
			arguments.Clear();
			IList<Token> tokens = instructions.Arguments;
			int count = tokens.Count;
			for (int i = 1; i < count; i++)
				arguments.Add(new Argument(tokens[i]));
		}
		#endregion
		protected override FieldMailMergeType MailMergeType() {
			return FieldMailMergeType.Mixed;
		}		
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			UpdateFieldOperationType result = UpdateFieldOperationType.Copy | UpdateFieldOperationType.Normal | UpdateFieldOperationType.CreateModelForExport;
			if (isUpdateFieldOperationTypeLoad)
				result |= UpdateFieldOperationType.Load;
			return result;
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, DevExpress.XtraRichEdit.Model.Field documentField) {
			object value = sourcePieceTable.DocumentModel.Variables.GetVariableValue(variableName, arguments, documentField, sourcePieceTable.DocumentModel.FieldOptions.ClearUnhandledDocVariableField ? null : DocVariableValue.Current);
			if (value == DocVariableValue.Current)
				return new CalculatedFieldValue(null, FieldResultOptions.KeepOldResult);
			DocumentModel valueDocumentModel = TryToGetValueDocumentModel(value);
			if (valueDocumentModel == null) {
				if (value == null)
					this.isUpdateFieldOperationTypeLoad = false;
				return new CalculatedFieldValue(value);
			}
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			PieceTableInsertContentConvertedToDocumentModelCommand command = new PieceTableInsertContentConvertedToDocumentModelCommand(targetModel.MainPieceTable, valueDocumentModel, DocumentLogPosition.Zero, false);
			command.CopyBetweenInternalModels = true;
			command.Execute();
			return new CalculatedFieldValue(targetModel, FieldResultOptions.DoNotApplyFieldCodeFormatting | FieldResultOptions.SuppressMergeUseFirstParagraphStyle);
		}
		private DocumentModel TryToGetValueDocumentModel(object value) {
			InternalRichEditDocumentServer server = InternalRichEditDocumentServer.TryConvertInternalRichEditDocumentServer(value);
			if (server != null)
				return server.InnerServer.DocumentModel;
			NativeDocument document = value as NativeDocument;
			if (document != null)
				return document.DocumentModel;
			return value as DocumentModel;
		}
	}
	#endregion
}
