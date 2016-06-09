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
using DevExpress.XtraRichEdit.Model.History;
namespace DevExpress.XtraRichEdit.Fields {
	public partial class HyperlinkField : CalculatedFieldBase {
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "HYPERLINK";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument("l", "o", "t");
		public static CalculatedFieldBase Create() {
			return new HyperlinkField();
		}
		#endregion
		string location;
		string locationInFile;
		bool appendCoordinates;
		bool openInNewWindow;
		string screenTip;
		string target;
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		public string Location { get { return location; } }
		public string LocationInFile { get { return locationInFile; } }
		public bool AppendCoordinates { get { return appendCoordinates; } }
		public bool OpenInNewWindow { get { return openInNewWindow; } }
		public string ScreenTip { get { return screenTip; } }
		public string Target { get { return target; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			location = instructions.GetArgumentAsString(0);
			locationInFile = instructions.GetString("l");
			appendCoordinates = instructions.GetBool("m");
			openInNewWindow = instructions.GetBool("n");
			screenTip = instructions.GetString("o");
			target = instructions.GetString("t");
		}
		#endregion
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			return CalculatedFieldValue.Null;
		}
		public override CalculatedFieldValue Update(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			if (!sourcePieceTable.DocumentModel.DocumentCapabilities.HyperlinksAllowed)
				return new CalculatedFieldValue(null, FieldResultOptions.KeepOldResult);
			HyperlinkInfo info = CreateHyperlinkInfo();
			UpdateVisited(sourcePieceTable, info);
			HyperlinkInfo oldInfo = GetHyperlinkInfo(sourcePieceTable, documentField);
			if (oldInfo != null)
				sourcePieceTable.ReplaceHyperlinkInfo(documentField.Index, info);
			else {
				sourcePieceTable.InsertHyperlinkInfo(documentField.Index, info);
				if (documentField.Result.Start == documentField.Result.End) {
					string result = !String.IsNullOrEmpty(location) ? location : locationInFile;
					if (!String.IsNullOrEmpty(result))
						sourcePieceTable.ModifyHyperlinkResult(documentField, result);
				}
			}
			return new CalculatedFieldValue(null, FieldResultOptions.HyperlinkField);
		}
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			return UpdateFieldOperationType.Normal | UpdateFieldOperationType.Load | UpdateFieldOperationType.Copy | UpdateFieldOperationType.PasteFromIE | UpdateFieldOperationType.CreateModelForExport;
		}
		HyperlinkInfo CreateHyperlinkInfo() {
			HyperlinkInfo result = new HyperlinkInfo();
			result.NavigateUri = location;
			result.Anchor = locationInFile;
			result.Target = target;
			result.ToolTip = screenTip;
			return result;
		}
		private void UpdateVisited(PieceTable sourcePieceTable, HyperlinkInfo info) {
			info.Visited = false;
		}
		protected virtual HyperlinkInfo GetHyperlinkInfo(PieceTable sourcePieceTable, Field field) {
			HyperlinkInfoCollection hyperlinkInfos = sourcePieceTable.HyperlinkInfos;
			if (hyperlinkInfos.IsHyperlink(field.Index))
				return hyperlinkInfos[field.Index];
			return null;
		}
	}
}
