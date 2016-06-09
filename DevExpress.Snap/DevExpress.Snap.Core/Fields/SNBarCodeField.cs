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

using DevExpress.Snap.Core.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.Snap.Core.Fields {
	[
	ActionListDesigner("DevExpress.Snap.Extensions.Native.ActionLists.BarCodeActionListDesigner,"+ AssemblyInfo.SRAssemblySnapExtensions)
	]
	public class SNBarCodeField : SNMergeFieldSupportsEmptyFieldDataAlias {
		#region static
		public static new readonly string FieldType = "SNBARCODE";
		public static readonly string BarCodeAlignmentSwitch = "a";
		public static readonly string BarCodeTextAlignmentSwitch = "ta";
		public static readonly string BarCodeOrientationSwitch = "o";
		public static readonly string BarCodeDataSwitch = "d";
		public static readonly string BarCodeGeneratorSwitch = "g";
		public static readonly string BarCodeAutoModuleSwitch = "am";
		public static readonly string BarCodeModuleSwitch = "m";
		public static readonly string BarCodeShowTextSwitch = "s";
		public static readonly string BarCodeWidthSwitch = "w";
		public static readonly string BarCodeHeightSwitch = "h";
		public static new CalculatedFieldBase Create() {
			return new SNBarCodeField();
		}
		static readonly Dictionary<string, bool> barCodeSwitchesWithArgument;
		static SNBarCodeField() {
			barCodeSwitchesWithArgument = CreateSwitchesWithArgument(BarCodeAlignmentSwitch, BarCodeTextAlignmentSwitch, BarCodeOrientationSwitch, BarCodeDataSwitch, BarCodeGeneratorSwitch,
				BarCodeShowTextSwitch, BarCodeAutoModuleSwitch, BarCodeModuleSwitch, BarCodeWidthSwitch, BarCodeHeightSwitch, EmptyFieldDataAliasSwitch);
			foreach (KeyValuePair<string, bool> sw in MergefieldField.SwitchesWithArgument)
				barCodeSwitchesWithArgument.Add(sw.Key, sw.Value);
		}
		#endregion
		public SNBarCodeField() {
			BarCodeGenerator = new Code128Generator();
		}
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return barCodeSwitchesWithArgument; } }
		public TextAlignment Alignment { get; private set; }
		public TextAlignment TextAlignment { get; private set; }
		public BarCodeOrientation Orientation { get; private set; }
		public string Data { get; private set; }
		public BarCodeSymbology Symbology { get { return BarCodeGenerator.SymbologyCode; } }
		public BarCodeGeneratorBase BarCodeGenerator { get; private set; }
		public int Module { get; private set; }
		public bool AutoModule { get; private set; }
		public bool ShowText { get; private set; }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			SetAlignment(instructions);
			SetTextAlignment(instructions);
			SetOrientation(instructions);
			SetData(instructions);
			SetGenerator(pieceTable, instructions);
			SetAutoModule(instructions);
			SetModule();
			SetShowText(instructions);
		}
		protected override bool SholdApplyFormating {
			get { return false; }
		}
		void SetAlignment(InstructionCollection instructions) {
			string alignment = instructions.GetString(BarCodeAlignmentSwitch);
			Alignment = !String.IsNullOrEmpty(alignment) ? (TextAlignment)Enum.Parse(typeof(TextAlignment), alignment, true)
				: BarCodeRunObject.DefaultAlignment;
		}
		void SetTextAlignment(InstructionCollection instructions) {
			string alignment = instructions.GetString(BarCodeTextAlignmentSwitch);
			TextAlignment = !String.IsNullOrEmpty(alignment) ? (TextAlignment)Enum.Parse(typeof(TextAlignment), alignment, true)
				: BarCodeRunObject.DefaultAlignment;
		}
		void SetOrientation(InstructionCollection instructions) {
			string orientation = instructions.GetString(BarCodeOrientationSwitch);
			Orientation = !String.IsNullOrEmpty(orientation) ? (BarCodeOrientation)Enum.Parse(typeof(BarCodeOrientation), orientation, true)
				: BarCodeRunObject.DefaultOrientation;
		}
		void SetData(InstructionCollection instructions) {
			string data = instructions.GetString(BarCodeDataSwitch);
			if (String.IsNullOrEmpty(data))
				return;
			 Data = data;
		}
		void SetGenerator(PieceTable pieceTable, InstructionCollection instructions) {
			string generator = instructions.GetBase64String(BarCodeGeneratorSwitch, pieceTable);
			if (string.IsNullOrEmpty(generator))
				generator = instructions.GetString(BarCodeGeneratorSwitch);
			if (!string.IsNullOrEmpty(generator))
				BarCodeGenerator = SNBarCodeHelper.GetBarCodeGenerator(generator);
		}
		void SetAutoModule(InstructionCollection instructions) {
			string autoModuleString = instructions.GetString(BarCodeAutoModuleSwitch);
			AutoModule = !String.IsNullOrEmpty(autoModuleString) ? Convert.ToBoolean(autoModuleString)
				: BarCodeRunObject.DefaultAutoModule;
		}
		void SetModule() {
			int module = Switches.GetInt("m");
			Module = module != 0 ? module : BarCodeRunObject.DefaultModule;
		}
		void SetShowText(InstructionCollection instructions) {
			string showTextString = instructions.GetString(BarCodeShowTextSwitch);
			ShowText = !String.IsNullOrEmpty(showTextString) ? Convert.ToBoolean(showTextString)
				: BarCodeRunObject.DefaultShowText;
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			BarCodeRunObject barCodeObject;
			CustomRun oldRun = sourcePieceTable.Runs[documentField.Result.Start] as CustomRun;
			if (oldRun != null && oldRun.CustomRunObject is BarCodeRunObject)
				barCodeObject = (BarCodeRunObject)oldRun.CustomRunObject.Clone();
			else
				barCodeObject = new BarCodeRunObject();
			string text = String.Empty;
			if (string.IsNullOrEmpty(DataFieldName)) {
				text = Data;
			}
			else {
				try {
					CalculatedFieldValue baseValue = base.GetCalculatedValueCore(sourcePieceTable, mailMergeDataMode, documentField);
					if (baseValue == CalculatedFieldValue.Null ){
						return new CalculatedFieldValue(EmptyFieldDataAlias);
					}
					text = Convert.ToString(baseValue.RawValue);
				}
				catch { }
			}
			barCodeObject.Alignment = Alignment;
			barCodeObject.TextAlignment = TextAlignment;
			barCodeObject.Orientation = Orientation;
			barCodeObject.Text = text;
			barCodeObject.BarCodeGenerator = BarCodeGenerator;
			barCodeObject.Module = Module;
			barCodeObject.AutoModule = AutoModule;
			barCodeObject.ShowText = ShowText;
			SetSize(barCodeObject);
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			targetModel.MainPieceTable.InsertCustomRun(DocumentLogPosition.Zero, barCodeObject, false);
			return new CalculatedFieldValue(targetModel);
		}
		void SetSize(BarCodeRunObject barCodeObject) {
			int? width = Switches.GetNullableInt(BarCodeWidthSwitch);
			int? height = Switches.GetNullableInt(BarCodeHeightSwitch);
			if(!width.HasValue || !height.HasValue)
				return;
			barCodeObject.ActualSize = new Size(width.Value, height.Value);
		}
		protected internal override string[] GetNativeSwithes() {
			return new string[] {
				BarCodeAlignmentSwitch,
				BarCodeTextAlignmentSwitch,
				BarCodeOrientationSwitch,
				BarCodeDataSwitch,
				BarCodeGeneratorSwitch,
				BarCodeAutoModuleSwitch,
				BarCodeModuleSwitch,
				BarCodeShowTextSwitch,
				BarCodeWidthSwitch,
				BarCodeHeightSwitch,
				EmptyFieldDataAliasSwitch,
				EnableEmptyFieldDataAliasSwitch
			};
		}
		protected internal override string[] GetInvariableSwitches() {
			return new string[] {
				EmptyFieldDataAliasSwitch,
				EnableEmptyFieldDataAliasSwitch
			};
		}
	}
	public class SNBarCodeFieldController : SizeAndScaleFieldController<SNBarCodeField> {
		public SNBarCodeFieldController(InstructionController controller)
			: base(controller, GetRectangularObject(controller)) {
		}
		static IRectangularObject GetRectangularObject(InstructionController controller) {
			return ((CustomRun)controller.PieceTable.Runs[controller.Field.Result.Start]).CustomRunObject as IRectangularObject;
		}
		protected override void SetImageSizeInfoCore() {
			SetSize();
		}
	}
}
