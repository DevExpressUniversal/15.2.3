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
using System.Diagnostics;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Fields {
	public sealed class FieldNull {
		static readonly FieldNull value = new FieldNull();
		public static FieldNull Value { get { return value; } }
		private FieldNull() {
		}
		public override string ToString() {
			return String.Empty;
		}
	}
	public partial class MergefieldField : CalculatedFieldBase, IDataFieldNameOwner {		
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "MERGEFIELD";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument("b", "f");
		protected static Dictionary<string, bool> SwitchesWithArgument { get { return switchesWithArgument; } }
		public static CalculatedFieldBase Create() {
			return new MergefieldField();
		}
		#endregion
		string dataFieldName;
		string textBeforeIfFieldNotBlank;
		string textAfterIfFieldNotBlank;
		bool mappedField;
		bool enableConversionForVerticalFormatting;
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } } 
		public string DataFieldName { get { return dataFieldName; } }
		public string TextBeforeIfFieldNotBlank { get { return textBeforeIfFieldNotBlank; } }
		public string TextAfterIfFieldNotBlank { get { return textAfterIfFieldNotBlank; } }
		public bool MappedField { get { return mappedField; } }
		public bool EnableConversionForVerticalFormatting { get { return enableConversionForVerticalFormatting; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			dataFieldName = instructions.GetArgumentAsString(0);
			textBeforeIfFieldNotBlank = instructions.GetString("b");
			textAfterIfFieldNotBlank = instructions.GetString("f");
			mappedField = instructions.GetBool("m");
			enableConversionForVerticalFormatting = instructions.GetBool("v");
		}
		#endregion
		protected override FieldMailMergeType MailMergeType() {
			return FieldMailMergeType.MailMerge;
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			IFieldDataService fieldDataService = sourcePieceTable.DocumentModel.GetService<IFieldDataService>();
			Debug.Assert(fieldDataService != null);
			object value = fieldDataService.GetFieldValue(sourcePieceTable.DocumentModel.MailMergeProperties, dataFieldName, mappedField, mailMergeDataMode, sourcePieceTable, documentField);
			if (value == null || value == DBNull.Value)
				return GetNullValue();
			if (value == FieldNull.Value)
				return new CalculatedFieldValue(value);
			if (String.IsNullOrEmpty(TextBeforeIfFieldNotBlank) && String.IsNullOrEmpty(TextAfterIfFieldNotBlank))
				return new CalculatedFieldValue(value);
			return new CalculatedFieldValue(String.Concat(textBeforeIfFieldNotBlank, value.ToString(), textAfterIfFieldNotBlank));
		}
		protected virtual CalculatedFieldValue GetNullValue() {
				return new CalculatedFieldValue(String.Empty);
		}
	}
	public interface IDataFieldNameOwner {
		string DataFieldName { get; }
	}
}
