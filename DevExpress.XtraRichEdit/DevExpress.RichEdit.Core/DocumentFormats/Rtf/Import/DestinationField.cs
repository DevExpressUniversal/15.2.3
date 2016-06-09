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
using System.Drawing;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region FieldDestination
	public class FieldDestination : FieldSubDestination {
		#region CreateFieldKeywords
		static KeywordTranslatorTable keywordHT = CreateFieldKeywords();
		static KeywordTranslatorTable CreateFieldKeywords() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("fldinst", OnFieldInstructionStartKeyword);
			table.Add("fldrslt", OnFieldResultStartKeyword);
			table.Add("fldlock", OnFieldLockKeyword);
			table.Add("fldedit", OnFieldEditKeyword);
			table.Add("flddirty", OnFieldDirtyKeyword);
			table.Add("fldpriv", OnFieldPrivateKeyword);
			table.Add("dxfldcodeview", OnFieldCodeViewKeyword);
			return table;
		}
		#endregion
		#region Keyword handlers
		static void OnFieldInstructionStartKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			FieldDestination destination = (FieldDestination)importer.Destination;
			if (destination.NestedGroupLevel <= 1)
				RtfImporter.ThrowInvalidRtfFile();
			RtfFieldInfo fieldInfo = importer.Fields.Peek();
			if (fieldInfo.Field != null)
				importer.Position.LogPosition--;
			importer.Destination = CreateFieldCodeDestination(importer);
		}
		static DestinationBase CreateFieldCodeDestination(RtfImporter importer) {
			if (importer.DocumentModel.DocumentCapabilities.HyperlinksAllowed)
				return new CodeFieldDestination(importer);
			else
				return new CodeFieldHyperlinksDisabledDestination(importer);
		}
		static void OnFieldResultStartKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			FieldDestination destination = (FieldDestination)importer.Destination;
			if (destination.NestedGroupLevel <= 1)
				RtfImporter.ThrowInvalidRtfFile();
			importer.Destination = CreateFieldResultDestination(importer);
		}
		static DestinationBase CreateFieldResultDestination(RtfImporter importer) {
			RtfFieldInfo fieldInfo = importer.Fields.Peek();
			if (!importer.DocumentModel.DocumentCapabilities.HyperlinksAllowed && fieldInfo.IsHyperlink)
				return new ResultFieldDestination(importer);
			else {
				if (fieldInfo.Field == null)
					return CreateFieldCodeDestination(importer);
				else
					return new ResultFieldDestination(importer);
			}
		}
		static void OnFieldLockKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Fields.Peek().Locked = true;
		}
		static void OnFieldCodeViewKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!importer.DocumentModel.FieldOptions.UpdateFieldsOnPaste)
				importer.Fields.Peek().Field.IsCodeView = true;
		}
		static void OnFieldEditKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnFieldDirtyKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnFieldPrivateKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		#endregion
		public FieldDestination(RtfImporter importer)
			: base(importer) {
		}
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override void StartNewField() {
		}
		protected override void OnDestinationClose() {
			RtfFieldInfo fieldInfo = Importer.Fields.Pop();
			ProcessField(fieldInfo);
		}
		void ProcessField(RtfFieldInfo fieldInfo) {
			if (fieldInfo.Field == null) {
				if (fieldInfo.IsHyperlink)
					return;
				fieldInfo.Field = CreateField();
			}
			else
				Importer.Position.LogPosition++;
			fieldInfo.Field.Locked = fieldInfo.Locked;
			fieldInfo.Field.IsCodeView = false;
		}
		protected override FieldSubDestination CreateInstance() {
			return new FieldDestination(Importer);
		}
	}
	#endregion
	#region FieldSubDestination (abstract class)
	public abstract class FieldSubDestination : DefaultDestination {
		#region Fields
		int nestedGroupLevel = 1;
		#endregion
		protected FieldSubDestination(RtfImporter importer)
			: base(importer, importer.PieceTable) {
		}
		#region Properties
		protected internal int NestedGroupLevel { get { return nestedGroupLevel; } }
		#endregion
		protected override DestinationBase CreateClone() {
			FieldSubDestination clone = CreateInstance();
			clone.nestedGroupLevel = this.nestedGroupLevel;
			return clone;
		}
		public override void BeforePopRtfState() {
			base.BeforePopRtfState();
			this.nestedGroupLevel--;
			if (this.nestedGroupLevel == 0)
				OnDestinationClose();
		}
		public override void IncreaseGroupLevel() {
			base.IncreaseGroupLevel();
			this.nestedGroupLevel++;
		}
		protected virtual void OnDestinationClose() {
		}
		protected virtual void EnsureFieldCreated(RtfFieldInfo fieldInfo) {
			if (fieldInfo.Field != null)
				return;
			DocumentLogPosition logPosition = Importer.Position.LogPosition;
			fieldInfo.Field = CreateField();
			Importer.Position.LogPosition = logPosition + 1;
		}
		protected Field CreateField() {
			DevExpress.XtraRichEdit.Model.History.AddFieldHistoryItem item = new DevExpress.XtraRichEdit.Model.History.AddFieldHistoryItem(PieceTable);
			item.CodeStartRunIndex = PieceTable.InsertFieldCodeStartRunCore(Importer.Position);
			item.CodeEndRunIndex = PieceTable.InsertFieldCodeEndRunCore(Importer.Position);
			item.ResultEndRunIndex = PieceTable.InsertFieldResultEndRunCore(Importer.Position);
			item.Execute();
			return PieceTable.Fields[item.InsertedFieldIndex];
		}
		protected abstract FieldSubDestination CreateInstance();
	}
	#endregion
	#region CodeFieldDestination
	public class CodeFieldDestination : FieldSubDestination {
		public CodeFieldDestination(RtfImporter importer)
			: base(importer) {
		}
		protected override FieldSubDestination CreateInstance() {
			return new CodeFieldDestination(Importer);
		}
		protected override void OnDestinationClose() {
			EnsureFieldCreated(Importer.Fields.Peek());
			Importer.Position.LogPosition++;
		}
		protected override void ProcessTextCore(string text) {
			EnsureFieldCreated(Importer.Fields.Peek());
			base.ProcessTextCore(text);
		}
		protected override void ProcessCharCore(char ch) {
			EnsureFieldCreated(Importer.Fields.Peek());
			base.ProcessCharCore(ch);
		}
		protected override void StartNewField() {
			EnsureFieldCreated(Importer.Fields.Peek());
			base.StartNewField();
		}
	}
	#endregion
	#region CodeFieldHyperlinksDisabledDestination
	public class CodeFieldHyperlinksDisabledDestination : CodeFieldDestination {
		public CodeFieldHyperlinksDisabledDestination(RtfImporter importer)
			: base(importer) {
		}
		protected override FieldSubDestination CreateInstance() {
			return new CodeFieldHyperlinksDisabledDestination(Importer);
		}
		protected override void OnDestinationClose() {
			RtfFieldInfo fieldInfo = Importer.Fields.Peek();
			if (fieldInfo.IsHyperlink)
				return;
			base.OnDestinationClose();
		}
		protected override void ProcessTextCore(string text) {
			RtfFieldInfo fieldInfo = Importer.Fields.Peek();
			if (fieldInfo.IsHyperlink)
				return;
			if (DetectHyperlink(text)) {
				fieldInfo.IsHyperlink = true;
				return;
			}
			base.ProcessTextCore(text);
		}
		protected override void ProcessCharCore(char ch) {
			RtfFieldInfo fieldInfo = Importer.Fields.Peek();
			if (fieldInfo.IsHyperlink)
				return;
			base.ProcessCharCore(ch);
		}
		bool DetectHyperlink(string text) {
			return text.IndexOf("HYPERLINK", StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}
	#endregion
	#region ResultFieldDestination
	public class ResultFieldDestination : FieldSubDestination {
		public ResultFieldDestination(RtfImporter importer)
			: base(importer) {
		}
		protected override FieldSubDestination CreateInstance() {
			return new ResultFieldDestination(Importer);
		}
		protected override void OnDestinationClose() {
		}
	}
	#endregion
	#region RtfFieldInfo
	public class RtfFieldInfo {
		public bool Locked { get; set; }
		public bool IsHyperlink { get; set; }
		public Field Field { get; set; }
	}
	#endregion
}
