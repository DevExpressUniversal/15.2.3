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
using System.Globalization;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using System.IO;
using DevExpress.Office.Utils;
namespace DevExpress.Snap.Core.Fields {
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.ContentTypeActionList," + AssemblyInfo.SRAssemblySnapExtensions, 0)]
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.DataFieldNameActionList," + AssemblyInfo.SRAssemblySnapExtensions, 1)]
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.SNHyperlinkActionList," + AssemblyInfo.SRAssemblySnapExtensions, 2)]
	public class SNHyperlinkField : SNMergeFieldSupportsEmptyFieldDataAlias {
		static readonly Dictionary<string, bool> switchesWithArgument;
		static SNHyperlinkField() {
			switchesWithArgument = CreateSwitchesWithArgument(ScreenTipSwitch, TargetSwitch, DisplayFieldSwitch, EmptyFieldDataAliasSwitch);
			foreach (KeyValuePair<string, bool> sw in MergefieldField.SwitchesWithArgument)
				switchesWithArgument.Add(sw.Key, sw.Value);
		}
		public static new readonly string FieldType = "SNHYPERLINK";
		public static readonly string ScreenTipSwitch = "o";
		public static readonly string TargetSwitch = "t";
		public static readonly string DisplayFieldSwitch = "d";
		public static new CalculatedFieldBase Create() {
			return new SNHyperlinkField();
		}
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		public string ScreenTip { get; private set; }
		public string Target { get; private set; }
		public string DisplayFieldName { get; private set; }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			ScreenTip = instructions.GetString(ScreenTipSwitch);
			Target = instructions.GetString(TargetSwitch);
			DisplayFieldName = Switches.GetString(DisplayFieldSwitch);
		}
		public override CalculatedFieldValue Update(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			return base.Update(sourcePieceTable, mailMergeDataMode, documentField).AddOptions(FieldResultOptions.SuppressUpdateInnerCodeFields | FieldResultOptions.DoNotApplyFieldCodeFormatting);
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			CalculatedFieldValue baseValue = base.GetCalculatedValueCore(sourcePieceTable, mailMergeDataMode, documentField);
			if (baseValue == CalculatedFieldValue.Null) {
				return new CalculatedFieldValue(EmptyFieldDataAlias);
			}
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			string navigateUri;
			if (baseValue.RawValue != FieldNull.Value)
				navigateUri = !String.IsNullOrEmpty(baseValue.Text) ? baseValue.Text : DataFieldName;
			else
				navigateUri = String.Empty;
			if (!String.IsNullOrEmpty(DisplayFieldName)) {
				IFieldDataService fieldDataService = sourcePieceTable.DocumentModel.GetService<IFieldDataService>();
				object value = fieldDataService.GetFieldValue(sourcePieceTable.DocumentModel.MailMergeProperties, DisplayFieldName, false, mailMergeDataMode, sourcePieceTable, documentField);
				if (value is byte[]) {
					InsertInlinePicture(targetModel, (byte[])value);
					CreateHyperlink(targetModel, navigateUri, 1);
				}
				else {
					string textToInsert = value != null ? value.ToString() : DisplayFieldName;
					CreateHyperlink(targetModel, navigateUri, textToInsert);
				}
			}
			else if (!String.IsNullOrEmpty(navigateUri))
				CreateHyperlink(targetModel, navigateUri, navigateUri);
			return new CalculatedFieldValue(targetModel);
		}
		void CreateHyperlink(DocumentModel targetModel, string navigateUri, int length) {
			targetModel.BeginUpdate();
			try {
				Field field = targetModel.MainPieceTable.CreateHyperlinkField(DocumentLogPosition.Zero, length, CreateHyperlinkInfo(navigateUri), false);
				field.HideByParent = true;
				targetModel.MainPieceTable.ToggleFieldCodes(field);
				targetModel.MainPieceTable.ApplyHyperlinkStyle(field, true);
			}
			finally {
				targetModel.EndUpdate();
			}
		}
		void CreateHyperlink(DocumentModel targetModel, string navigateUri, string textToInsert) {
			targetModel.MainPieceTable.InsertText(DocumentLogPosition.Zero, textToInsert);
			CreateHyperlink(targetModel, navigateUri, textToInsert.Length);
		}
		void InsertInlinePicture(DocumentModel targetModel, byte[] bytes) {
			MemoryStream stream = new MemoryStream(bytes);
			try {
				OfficeImage image = OfficeImage.CreateImage(stream);
				targetModel.MainPieceTable.InsertInlinePicture(DocumentLogPosition.Zero, image);
			}
			catch {
				stream.Dispose();
			}
		}
		HyperlinkInfo CreateHyperlinkInfo(string uri) {
			HyperlinkInfo result = new HyperlinkInfo();
			result.NavigateUri = uri;
			result.Target = Target;
			result.ToolTip = ScreenTip;
			return result;
		}
		protected internal override string[] GetNativeSwithes() {
			return new string[] { ScreenTipSwitch, TargetSwitch, DisplayFieldSwitch, EmptyFieldDataAliasSwitch, EnableEmptyFieldDataAliasSwitch };
		}
		protected internal override string[] GetInvariableSwitches() {
			return new string[] {
				EmptyFieldDataAliasSwitch,
				EnableEmptyFieldDataAliasSwitch
			};
		}
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			return base.GetAllowedUpdateFieldTypes(options);
		}
	}
}
