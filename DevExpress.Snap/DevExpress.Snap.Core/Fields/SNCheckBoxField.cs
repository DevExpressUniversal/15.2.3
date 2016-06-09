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
namespace DevExpress.Snap.Core.Fields {
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.ContentTypeActionList," + AssemblyInfo.SRAssemblySnapExtensions, 0)]
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.DataFieldNameActionList," + AssemblyInfo.SRAssemblySnapExtensions, 1)]
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.SNCheckBoxActionList," + AssemblyInfo.SRAssemblySnapExtensions, 2)]
	public class SNCheckBoxField : SNMergeFieldSupportsEmptyFieldDataAlias {
		internal static readonly Dictionary<CheckState, string> checkStateDictionary = new Dictionary<CheckState, string>();
		internal static readonly Dictionary<string, CheckState> checkStates = new Dictionary<string, CheckState>();
		static readonly Dictionary<string, bool> checkboxSwitchesWithArgument;
		static SNCheckBoxField() {
			checkboxSwitchesWithArgument = CreateSwitchesWithArgument(CheckStateSwitch, ValueCheckedSwitch, ValueUncheckedSwitch, EmptyFieldDataAliasSwitch);
			foreach (KeyValuePair<string, bool> sw in MergefieldField.SwitchesWithArgument)
				checkboxSwitchesWithArgument.Add(sw.Key, sw.Value);
			checkStateDictionary.Add(CheckState.Checked, "c");
			checkStateDictionary.Add(CheckState.Unchecked, "u");
			checkStateDictionary.Add(CheckState.Indeterminate, "i");
			checkStates.Add("c", CheckState.Checked);
			checkStates.Add("u", CheckState.Unchecked);
			checkStates.Add("i", CheckState.Indeterminate);
		}
		public static string GetCheckStateString(CheckState state) {
			return checkStateDictionary[state];
		}
		public static new readonly string FieldType = "SNCHECKBOX";
		public static readonly string CheckStateSwitch = "c";
		public static readonly string ValueCheckedSwitch = "vc";
		public static readonly string ValueUncheckedSwitch = "vu";
		public static new CalculatedFieldBase Create() {
			return new SNCheckBoxField();
		}
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return checkboxSwitchesWithArgument; } }
		public CheckState CheckState { get; private set; }
		public string ValueChecked { get; private set; }
		public string ValueUnchecked { get; private set; }
		protected bool IsValueCheckedDefined { get { return ValueChecked != null; } }
		protected bool IsValueUncheckedDefined { get { return ValueUnchecked != null; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			SetCheckState(instructions);
			ValueChecked = instructions.GetString(ValueCheckedSwitch);
			ValueUnchecked = instructions.GetString(ValueUncheckedSwitch);
		}
		void SetCheckState(InstructionCollection instructions) {
			string checkStateString = instructions.GetString(CheckStateSwitch);
			if (String.IsNullOrEmpty(checkStateString))
				return;
			CheckState checkState;
			if (checkStates.TryGetValue(checkStateString.ToLower(), out checkState))
				CheckState = checkState;
		}
		protected override bool SholdApplyFormating {
			get { return false; }
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			CheckState result = CheckState.Unchecked;
			if (string.IsNullOrEmpty(DataFieldName))
				result = CheckState;
			else {
				CalculatedFieldValue baseValue = base.GetCalculatedValueCore(sourcePieceTable, mailMergeDataMode, documentField);
				if (baseValue == CalculatedFieldValue.Null) {
					return new CalculatedFieldValue(EmptyFieldDataAlias);
				}
				object value = baseValue.RawValue;
				Type valueType = value.GetType();
				object valueChecked = IsValueCheckedDefined ? FromString(ValueChecked, valueType) : true;
				object valueUnchecked = IsValueUncheckedDefined ? FromString(ValueUnchecked, valueType) : false;
				result = GetCheckStateByValue(value, valueChecked, valueUnchecked);
			}
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			targetModel.MainPieceTable.InsertCustomRun(DocumentLogPosition.Zero, new CheckBoxRunObject() { CheckState = result }, false);
			return new CalculatedFieldValue(targetModel);
		}
		bool IsNullValue(string text) {
			return String.Equals(text, "null", StringComparison.OrdinalIgnoreCase);
		}
		object FromString(string text, Type type) {
			if (IsNullValue(text))
				return String.Empty;
			try {
				return Convert.ChangeType(text, type, CultureInfo.InvariantCulture);
			}
			catch {
				return text;
			}
		}
		CheckState GetCheckStateByValue(object value, object valueChecked, object valueUnchecked) {
			if (IsValueCheckedDefined && !IsValueUncheckedDefined)
				return AreEquals(value, valueChecked) ? CheckState.Checked : CheckState.Unchecked;
			else if (!IsValueCheckedDefined && IsValueUncheckedDefined)
				return AreEquals(value, valueUnchecked) ? CheckState.Unchecked : CheckState.Checked;
			else {
				if (AreEquals(value, valueChecked))
					return CheckState.Checked;
				else if (AreEquals(value, valueUnchecked))
					return CheckState.Unchecked;
				else
					return CheckState.Indeterminate;
			}
		}
		bool AreEquals(object val1, object val2) {
			return val1 == val2 || (val1 != null && val2 != null && val1.Equals(val2));
		}
		protected internal override string[] GetNativeSwithes() {
			return new string[] { CheckStateSwitch, ValueCheckedSwitch, ValueUncheckedSwitch, EmptyFieldDataAliasSwitch, EnableEmptyFieldDataAliasSwitch };
		}
		protected internal override string[] GetInvariableSwitches() {
			return new string[] {
				EmptyFieldDataAliasSwitch,
				EnableEmptyFieldDataAliasSwitch
			};
		}
	}   
}
