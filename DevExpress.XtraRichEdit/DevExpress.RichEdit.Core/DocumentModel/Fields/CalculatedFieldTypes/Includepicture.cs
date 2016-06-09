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
using DevExpress.XtraRichEdit.Services;
using System.Drawing;
using System.IO;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Utils;
using DevExpress.Office.Utils.Internal;
namespace DevExpress.XtraRichEdit.Fields {
	public partial class IncludepictureField : CalculatedFieldBase {
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "INCLUDEPICTURE";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument("c");
		public static CalculatedFieldBase Create() {
			return new IncludepictureField();
		}
		#endregion
		string documentName;
		string graphicsFilter;
		bool suppressStoreGraphicsDataWithDocument;
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } } 
		public string DocumentName { get { return documentName; } }
		public string GraphicsFilter { get { return graphicsFilter; } }
		public bool SuppressStoreGraphicsDataWithDocument { get { return suppressStoreGraphicsDataWithDocument; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			documentName = instructions.GetArgumentAsString(0);
			graphicsFilter = instructions.GetString("c");
			suppressStoreGraphicsDataWithDocument = instructions.GetBool("d");
		}
		#endregion
		protected override FieldMailMergeType MailMergeType() {
			return FieldMailMergeType.Mixed;
		}
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			UpdateFieldOperationType result = base.GetAllowedUpdateFieldTypes(options);
			if(suppressStoreGraphicsDataWithDocument)
				result |= UpdateFieldOperationType.Copy | UpdateFieldOperationType.Load | UpdateFieldOperationType.PasteFromIE | UpdateFieldOperationType.CreateModelForExport;
			return result;
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			List<string> uriList = new List<string>();
			uriList.Add(documentName);
			string currentFileName = sourcePieceTable.DocumentModel.DocumentSaveOptions.CurrentFileName;
			try {
				if (!String.IsNullOrEmpty(currentFileName))
					uriList.Add(Path.Combine(Path.GetDirectoryName(currentFileName), documentName));
			}
			catch {
			}
			UriBasedOfficeImage image = new UriBasedOfficeImage(uriList.ToArray(), 0, 0, sourcePieceTable.DocumentModel, true);
			InternalUriBasedOfficeImageHelper.SetSuppressStorePlaceholder(image, true);
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			InsertInlinePictureInTargetModel(image, targetModel, suppressStoreGraphicsDataWithDocument && mailMergeDataMode != MailMergeDataMode.FinalMerging);
			return new CalculatedFieldValue(targetModel);
		}
		protected internal void InsertInlinePictureInTargetModel(OfficeImage image, DocumentModel targetModel, bool suppressStore) {
			InternalOfficeImageHelper.SetSuppressStore(image, suppressStore);
			targetModel.MainPieceTable.InsertInlinePicture(DocumentLogPosition.Zero, image);			
		}
	}
	public class ShapeField : CalculatedFieldBase {
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "SHAPE";
		static readonly Dictionary<string, bool> switchesWithArgument = new Dictionary<string,bool>();
		public static CalculatedFieldBase Create() {
			return new ShapeField();
		}
		#endregion
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		#endregion
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			return CalculatedFieldValue.Null;
		}
		public override CalculatedFieldValue Update(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			CalculatedFieldValue result = new CalculatedFieldValue(null, FieldResultOptions.KeepOldResult);
			RunIndex startRunIndex = documentField.Result.Start;
			RunIndex endRunIndex = documentField.Result.End - 1;
			TextRunCollection runs = sourcePieceTable.Runs;
			if (endRunIndex <= startRunIndex || (endRunIndex - startRunIndex) > 1)
				return result;
			FloatingObjectAnchorRun anchorRun = runs[startRunIndex] as FloatingObjectAnchorRun;
			InlinePictureRun inlinePicture = runs[endRunIndex] as InlinePictureRun;
			if (anchorRun == null || inlinePicture == null || !anchorRun.FloatingObjectProperties.PseudoInline || !inlinePicture.Properties.PseudoInline)
				return result;
			DocumentModel documentModel = sourcePieceTable.DocumentModel;
			documentModel.BeginUpdate();
			try {
				documentModel.UnsafeEditor.DeleteRuns(sourcePieceTable, endRunIndex, 1);
			}
			finally {
				documentModel.EndUpdate();
			}
			return result;
		}
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			return UpdateFieldOperationType.Load;
		}
	}
}
