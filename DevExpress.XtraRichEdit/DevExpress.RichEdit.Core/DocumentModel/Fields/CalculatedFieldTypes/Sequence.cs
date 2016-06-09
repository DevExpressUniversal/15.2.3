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
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Fields {
	#region SequenceField
	public partial class SequenceField : CalculatedFieldBase {
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "SEQ";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument("r", "s");
		public static CalculatedFieldBase Create() {
			return new SequenceField();
		}
		#endregion
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		string id;
		bool incrementCounter;
		bool hideResult;
		int newCounterValue = Int32.MinValue;
		public string Id { get { return id; } }
		public bool IncrementCounter { get { return incrementCounter; } }
		public int NewCounterValue { get { return newCounterValue; } }
		public bool HideResult { get { return hideResult; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			this.id = instructions.GetArgumentAsString(0);
			this.incrementCounter = !instructions.GetBool("c");
			this.hideResult = instructions.GetBool("h");
			string newCounterValueString = instructions.GetString("r");
			if (newCounterValueString == null)
				this.newCounterValue = Int32.MinValue;
			else if (!Int32.TryParse(newCounterValueString, out this.newCounterValue))
				this.newCounterValue = Int32.MinValue;
		}
		#endregion
		protected override FieldMailMergeType MailMergeType() {
			return FieldMailMergeType.NonMailMerge;
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			int value = sourcePieceTable.Fields.SequentialFieldManager.CalculateCounterValue(this, documentField);
			if (hideResult && GeneralFormatting == null)
				return new CalculatedFieldValue(String.Empty);
			return new CalculatedFieldValue(value);
		}
	}
	#endregion
	#region SequentialCounterItem
	public class SequentialCounterItem {
		readonly Field field;
		readonly int value;
		public SequentialCounterItem(Field field, int value) {
			this.field = field;
			this.value = value;
		}
		public Field Field { get { return field; } }
		public int Value { get { return value; } }
	}
	#endregion
	#region SequentialFieldCounter
	public class SequentialFieldCounter {
		readonly string id;
		readonly List<SequentialCounterItem> values = new List<SequentialCounterItem>();
		public SequentialFieldCounter(string id) {
			this.id = id;
		}
		public string Id { get { return id; } }
		protected internal virtual int CalculateValue(Field field, PieceTable pieceTable) {
			int index = Algorithms.BinarySearch(values, new SequentialCounterItemAndFieldComparable(field));
			if (index >= 0)
				return values[index].Value;
			return UpdateCounter(field, pieceTable);
		}
		protected internal virtual int UpdateCounter(Field field, PieceTable pieceTable) {
			int firstFieldIndex;
			int value;
			if (values.Count > 0) {
				Debug.Assert(field.Code.Start > values[values.Count - 1].Field.Code.Start);
				firstFieldIndex = values[values.Count - 1].Field.Index;
				value = values[values.Count - 1].Value;
			}
			else {
				firstFieldIndex = -1;
				value = 0;
			}
			int lastFieldIndex = field.Index;
			for (int i = firstFieldIndex + 1; i <= lastFieldIndex; i++)
				value = UpdateCounterCore(pieceTable.Fields[i], value, pieceTable);
			return value;
		}
		protected internal virtual int UpdateCounterCore(Field field, int value, PieceTable pieceTable) {
			DocumentFieldIterator iterator = new DocumentFieldIterator(pieceTable, field);
			FieldScanner scanner = new FieldScanner(iterator, pieceTable.DocumentModel.MaxFieldSwitchLength, pieceTable.DocumentModel.EnableFieldNames, pieceTable.SupportFieldCommonStringFormat);
			Token token = scanner.Scan();
			if (token.ActualKind == TokenKind.OpEQ || token.ActualKind == TokenKind.Eq)
				return value;
			SequenceField calculatedField = FieldCalculatorService.CreateField(token.Value) as SequenceField;
			if (calculatedField == null)
				return value;
			InstructionCollection instructions = FieldCalculatorService.ParseInstructions(scanner, calculatedField);
			calculatedField.Initialize(pieceTable, instructions);
			if (calculatedField.Id != this.Id)
				return value;
			if (calculatedField.NewCounterValue != Int32.MinValue)
				value = calculatedField.NewCounterValue;
			else if (calculatedField.IncrementCounter)
				value++;
			values.Add(new SequentialCounterItem(field, value));
			return value;
		}
	}
	#endregion
	#region SequentialCounterItemAndFieldComparable
	public class SequentialCounterItemAndFieldComparable : IComparable<SequentialCounterItem> {
		readonly Field field;
		public SequentialCounterItemAndFieldComparable(Field field) {
			Guard.ArgumentNotNull(field, "field");
			this.field = field;
		}
		#region IComparable<SequentialCounterItem> Members
		public int CompareTo(SequentialCounterItem other) {
			RunIndex otherStart = other.Field.Code.Start;
			RunIndex fieldStart = field.Code.Start;
			if (otherStart == fieldStart)
				return 0;
			if (otherStart < fieldStart)
				return -1;
			return 1;
		}
		#endregion
	}
	#endregion
	#region SequentialFieldManager
	public class SequentialFieldManager {
		readonly PieceTable pieceTable;
		readonly Dictionary<string, SequentialFieldCounter> counterTable = new Dictionary<string, SequentialFieldCounter>();
		public SequentialFieldManager(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public int CalculateCounterValue(SequenceField field, Field documentField) {
			if (String.IsNullOrEmpty(field.Id))
				return Int32.MinValue;
			SequentialFieldCounter counter;
			if (!counterTable.TryGetValue(field.Id, out counter)) {
				counter = new SequentialFieldCounter(field.Id);
				counterTable.Add(field.Id, counter);
			}
			return counter.CalculateValue(documentField, pieceTable);
		}
		public void ClearCounters() {
			counterTable.Clear();
		}
	}
	#endregion
}
