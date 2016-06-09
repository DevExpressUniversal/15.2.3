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
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office;
using DevExpress.Office.Commands.Internal;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Forms {
	public class PasteSpecialLocalFormControllerParameters : FormControllerParameters {
		readonly ModelPasteSpecialOptions pasteSpecialOptions;
		public PasteSpecialLocalFormControllerParameters(ISpreadsheetControl control, ModelPasteSpecialOptions pasteSpecialOptions)
			: base(control) {
			this.pasteSpecialOptions = pasteSpecialOptions;
		}
		public ModelPasteSpecialOptions PasteSpecialOptions { get { return pasteSpecialOptions; } }
	}
	#region PasteSpecialFormControllerParameters
	public class PasteSpecialFormControllerParameters : FormControllerParameters {
		readonly PasteSpecialInfo pasteSpecialInfo;
		public PasteSpecialFormControllerParameters(ISpreadsheetControl control, PasteSpecialInfo pasteSpecialInfo)
			: base(control) {
			Guard.ArgumentNotNull(pasteSpecialInfo, "pasteSpecialInfo");
			this.pasteSpecialInfo = pasteSpecialInfo;
		}
		public PasteSpecialInfo PasteSpecialInfo { get { return pasteSpecialInfo; } }
	}
	#endregion
	public class PasteSpecialLocalFormController : FormController {
		bool skipBlanks;
		int currentPasteSpecialIndex;
		static List<ModelPasteSpecialFlags> values = CreateValues();
		readonly ModelPasteSpecialOptions sourcePasteSpecialOptions;
		public PasteSpecialLocalFormController(PasteSpecialLocalFormControllerParameters controllerParameters) {
			currentPasteSpecialIndex = 0;
			skipBlanks = false;
			sourcePasteSpecialOptions = controllerParameters.PasteSpecialOptions;
		}
		public ModelPasteSpecialOptions SourcePasteSpecialOptions { get { return sourcePasteSpecialOptions; } }
		public override void ApplyChanges() {
			ModelPasteSpecialFlags flags = values[currentPasteSpecialIndex];
			sourcePasteSpecialOptions.InnerFlags = flags;
			sourcePasteSpecialOptions.ShouldSkipEmptyCellsInSourceRange = SkipBlanks;
		}
		static List<ModelPasteSpecialFlags> CreateValues() {
			List<ModelPasteSpecialFlags> result = new List<ModelPasteSpecialFlags>();
			result.Add(ModelPasteSpecialFlags.All);
			result.Add(ModelPasteSpecialFlags.Formulas);
			result.Add(ModelPasteSpecialFlags.Values);
			result.Add(ModelPasteSpecialFlags.FormatAndStyle);
			result.Add(ModelPasteSpecialFlags.Comments);
			result.Add(ModelPasteSpecialFlags.AllExceptBorders);
			result.Add(ModelPasteSpecialFlags.ColumnWidths);
			result.Add(ModelPasteSpecialFlags.FormulasAndNumberFormats);
			result.Add(ModelPasteSpecialFlags.ValuesAndNumberFormats);
			return result;
		}
		public IList<string> GetPasteSpecialItems() {
			IList<string> result = new List<string>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PasteSpecial_All));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PasteSpecial_Formulas));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PasteSpecial_Values)); 
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PasteSpecial_Formats)); 
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PasteSpecial_Comments));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PasteSpecial_AllExceptBorders));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PasteSpecial_ColumnWidths));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PasteSpecial_FormulasAndNumberFormats));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PasteSpecial_ValuesAndNumberFormats)); 
			return result;
		}
		public int CurrentPasteSpecialItemIndex { get { return currentPasteSpecialIndex; } set { currentPasteSpecialIndex = value; } }
		public bool SkipBlanks { get { return skipBlanks; } set { skipBlanks = value; } }
	}
	#region PasteSpecialFormController
	public class PasteSpecialFormController : FormController {
		readonly ISpreadsheetControl control;
		readonly PasteSpecialInfo sourcePasteSpecialInfo;
		readonly PasteSpecialInfo pasteSpecialInfo;
		public PasteSpecialFormController(PasteSpecialFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.sourcePasteSpecialInfo = controllerParameters.PasteSpecialInfo;
			this.pasteSpecialInfo = sourcePasteSpecialInfo.Clone();
		}
		public ISpreadsheetControl Control { get { return control; } }
		public PasteSpecialInfo SourcePasteSpecialInfo { get { return sourcePasteSpecialInfo; } }
		public Type PasteCommandType { get { return pasteSpecialInfo.PasteCommandType; } set { pasteSpecialInfo.PasteCommandType = value; } }
		public override void ApplyChanges() {
			sourcePasteSpecialInfo.CopyFrom(pasteSpecialInfo);
		}
		public IList<Type> AvailableCommandTypes {
			get {
				List<Type> result = new List<Type>();
				PasteSelectionCommand command = new PasteSelectionCommand(Control);
				CommandCollection commands = command.Commands;
				int count = commands.Count;
				for(int i = 0; i < count; i++) {
					PasteCommandBase pasteCommand = commands[i] as PasteCommandBase;
					if(pasteCommand != null && pasteCommand.CanExecute())
						result.Add(pasteCommand.GetType());
				}
				return result;
			}
		}
	}
	#endregion
	#region PasteSpecialInfo
	public class PasteSpecialInfo : ICloneable<PasteSpecialInfo>, ISupportsCopyFrom<PasteSpecialInfo> {
		Type pasteCommandType = typeof(PasteSelectionCommand);
		public Type PasteCommandType { get { return pasteCommandType; } set { pasteCommandType = value; } }
		#region ICloneable<PasteSpecialInfo> Members
		public PasteSpecialInfo Clone() {
			PasteSpecialInfo clone = new PasteSpecialInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<PasteSpecialInfo> Members
		public void CopyFrom(PasteSpecialInfo value) {
			this.PasteCommandType = value.PasteCommandType;
		}
		#endregion
		internal PasteCommandBase CreateCommand(ISpreadsheetControl control) {
			return CreateCommand(control, PasteCommandType);
		}
		internal static PasteCommandBase CreateCommand(ISpreadsheetControl control, Type commandType) {
			try {
				ConstructorInfo ci = commandType.GetConstructor(new Type[] { typeof(ISpreadsheetControl) });
				if(ci == null)
					return null;
				PasteCommandBase result = ci.Invoke(new object[] { control }) as PasteCommandBase;
				if(result != null)
					result.PasteSource = new ClipboardPasteSource();
				return result;
			}
			catch {
				return null;
			}
		}
	}
	#endregion
	#region PasteSpecialListBoxItem
	public class PasteSpecialListBoxItem {
		readonly ISpreadsheetControl control;
		readonly Type commandType;
		public PasteSpecialListBoxItem(ISpreadsheetControl control, Type commandType) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.commandType = commandType;
		}
		public ISpreadsheetControl Control { get { return control; } }
		public Type CommandType { get { return commandType; } }
		public override string ToString() {
			PasteCommandBase command = PasteSpecialInfo.CreateCommand(control, CommandType);
			if(command == null)
				return String.Empty;
			else
				return command.MenuCaption;
		}
	}
	#endregion
}
