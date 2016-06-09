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

using System.Globalization;
using System.Text;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Internal;
#if SL
using DevExpress.Xpf.Windows.Forms;
#endif
namespace DevExpress.XtraSpreadsheet.Forms {
	#region InsertFunctionViewModel
	public class InsertFunctionViewModel : ViewModelBase {
		const int AllIndex = 0;
		const int GroupCount = 14;
		#region Fields
		int categoryIndex;
		int functionIndex;
		readonly ISpreadsheetControl control;
		readonly int selectionStart;
		readonly int selectionLength;
		#endregion
		public InsertFunctionViewModel(ISpreadsheetControl control) {
			localizedFunctions = new List<string>[GroupCount + 1];
			this.control = control;
			if (control.InnerControl.IsInplaceEditorActive) {
				InnerCellInplaceEditor editor = control.InnerControl.InplaceEditor;
				this.selectionLength = editor.SelectionLength;
				this.selectionStart = editor.SelectionStart;
			}
			else {
				this.selectionLength = 0;
				this.selectionStart = 0;
			}
			fakeCategoryIndex = new Dictionary<int, int>();
			functions = CreateFunctions();
			functionCategories = CreateFunctionsCategories();
		}
		#region Properties
		ISpreadsheetControl Control { get { return control; } }
		DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		#endregion
		#region Categories
		readonly Dictionary<int, int> fakeCategoryIndex;
		readonly List<string> functionCategories;
		List<string> CreateFunctionsCategories() {
			List<string> groups = new List<string> {
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsFinancialCommandGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsDateAndTimeCommandGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsMathAndTrigonometryCommandGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsStatisticalCommandGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsLookupAndReferenceCommandGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.CaptionDatabaseFunctionsGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsTextCommandGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsLogicalCommandGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInformationCommandGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.CaptionUserDefinedFunctionsGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsEngineeringCommandGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsCubeCommandGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsCompatibilityCommandGroup),
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsWebCommandGroup)
			};
			for(int i = 0; i < groups.Count; i++) {
				groups[i] = groups[i].Replace("&&", "&");
			}
			List<string> result = new List<string> { XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.CaptionAllFunctionsGroup) };			
			for(int i = 0; i < groups.Count; i++) {
				if(functions[i].Count == 0)
					continue;
				result.Add(groups[i]);
				fakeCategoryIndex.Add(result.Count - 1, i);
			}
			return result;
		}
		public List<string> GetFunctionCategories() {
			return functionCategories;
		}
		public string Category {
			get { return categoryIndex >= 0 ? functionCategories[categoryIndex] : String.Empty; }
			set {
				int newIndex = functionCategories.IndexOf(value);
				if(newIndex == categoryIndex)
					return;
				categoryIndex = newIndex;
				OnPropertyChanged("Category");
				OnPropertyChanged("Functions");
			}
		}
		#endregion
		#region Functions
		delegate void AddFunctionGroupDelegate(Dictionary<string, ISpreadsheetFunction> dictionary);
		readonly List<string>[] localizedFunctions;
		readonly List<string>[] functions;
		List<string>[] CreateFunctions() {
			List<string>[] result = new List<string>[GroupCount];  
			result[0] = GenerateFunctionsGroup(FormulaCalculator.AddFinancialFunctions);
			result[1] = GenerateFunctionsGroup(FormulaCalculator.AddDateTimeFunctions);
			result[2] = GenerateFunctionsGroup(FormulaCalculator.AddMathAndTrigonometryFunctions);
			result[3] = GenerateFunctionsGroup(FormulaCalculator.AddStatisticalFunctions);
			result[4] = GenerateFunctionsGroup(FormulaCalculator.AddLookupAndReferenceFunctions);
			result[5] = GenerateFunctionsGroup(FormulaCalculator.AddDatabaseFunctions);
			result[6] = GenerateFunctionsGroup(FormulaCalculator.AddTextFunctions);
			result[7] = GenerateFunctionsGroup(FormulaCalculator.AddLogicalFunctions);
			result[8] = GenerateFunctionsGroup(FormulaCalculator.AddInformationalFunctions);
			result[9] = GetFunctionsNames(FormulaCalculator.CustomFunctionProvider.FunctionsByInvariantName);
			result[9].AddRange(GetFunctionsNames(DocumentModel.CustomFunctionProvider.FunctionsByInvariantName));
			result[10] = GenerateFunctionsGroup(FormulaCalculator.AddEngineeringFunctions);
			result[11] = GenerateFunctionsGroup(FormulaCalculator.AddCubeFunctions);
			result[12] = GenerateFunctionsGroup(FormulaCalculator.AddCompatibilityFunctions);
			result[13] = GenerateFunctionsGroup(FormulaCalculator.AddWebFunctions);
			return result;
		}
		static List<string> GenerateFunctionsGroup(AddFunctionGroupDelegate addFunctionGroupMethod) {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			addFunctionGroupMethod(dictionary);
			return GetFunctionsNames(dictionary);
		}
		static List<string> GetFunctionsNames(Dictionary<string, ISpreadsheetFunction> dictionary) {
			List<string> result = new List<string>();
			foreach(KeyValuePair<string, ISpreadsheetFunction> spreadsheetFunction in dictionary) {
				if(!(spreadsheetFunction.Value is NotSupportedFunction)) {
					result.Add(spreadsheetFunction.Key);
				}
			}
			return result;
		}
		List<string> GetLocalizedFunctions(int index) {
			if(localizedFunctions[index] == null) {
				localizedFunctions[index] = new List<string>();
				for(int i = 0; i < functions[index].Count; i++) {
					localizedFunctions[index].Add(FormulaCalculator.GetLocalizedFunctionName(functions[index][i], DocumentModel.DataContext));
				}
				localizedFunctions[index].Sort();
			}
			return localizedFunctions[index];
		}
		List<string> GetFunctions() {
			if(localizedFunctions[GroupCount] != null) {
				localizedFunctions[GroupCount].Clear();
			}
			if(categoryIndex < 0)
				return new List<string>();
			if(categoryIndex == AllIndex) {
				if(localizedFunctions[GroupCount] == null)
					localizedFunctions[GroupCount] = new List<string>();
				for(int i = 0; i < GroupCount; i++) {
					localizedFunctions[GroupCount].AddRange(GetLocalizedFunctions(i));
				}
				localizedFunctions[GroupCount].Sort();
				return localizedFunctions[GroupCount];
			}
			return GetLocalizedFunctions(fakeCategoryIndex[categoryIndex]);
		}
		public List<string> Functions { get { return GetFunctions(); } }
		public int FunctionIndex {
			get { return functionIndex; }
			set {
				if(functionIndex == value)
					return;
				functionIndex = value;
				OnPropertyChanged("FunctionIndex");
				OnPropertyChanged("FunctionDescription");
				OnPropertyChanged("FunctionShortDescription");
			}
		}
		public string Function { get { return functionIndex < 0 || functionIndex >= Functions.Count ? String.Empty : Functions[functionIndex]; } }
		public string FunctionDescription {
			get {
				if(functionIndex < 0)
					return String.Empty;
				ISpreadsheetFunction function = FormulaCalculator.GetFunctionByName(Function, DocumentModel.DataContext);
				return function is CustomFunction ? GetDescriptionCustomFunction(function) : GetDescriptionBuiltInFunction(function);
			}
		}
		string GetDescriptionCustomFunction(ISpreadsheetFunction function) {
			CustomFunctionDescription customFunctionDescription;
			if(!DocumentModel.CustomFunctionsDescriptions.TryGetValue(function.Name, out customFunctionDescription)) {
				return String.Empty;
			}
			return customFunctionDescription.Description;
		}
		string GetDescriptionBuiltInFunction(ISpreadsheetFunction function) {
			int functionCode = function.Code;
			string description = XtraSpreadsheetFunctionDescriptionResLocalizer.GetString((XtraSpreadsheetFunctionDescriptionStringId) functionCode, DocumentModel.Culture);
			return description;
		}
		public string FunctionShortDescription {
			get {
				if(functionIndex < 0)
					return String.Empty;
				List<string> parametersNames = new List<string>();
				char listSeparator = DocumentModel.DataContext.GetListSeparator();
				FillParametersNames(parametersNames);
				StringBuilder result = new StringBuilder(Function);
				result.Append("(");
				result.Append(String.Join(listSeparator.ToString(), parametersNames));
				result.Append(")");
				return result.ToString();
			}
		}
		void FillParametersNames(List<string> parametersNames) {
			ISpreadsheetFunction function = FormulaCalculator.GetFunctionByName(Function, DocumentModel.DataContext);
			int functionCode = function.Code;
			CultureInfo culture = DocumentModel.Culture;
			for(int i = 0; i < function.Parameters.Count; i++) {
				FunctionParameter parameter = function.Parameters[i];
				string parameterName = parameter.Name;
				if(String.IsNullOrEmpty(parameterName)) {
					parameterName = XtraSpreadsheetFunctionArgumentsNamesResLocalizer.GetString((XtraSpreadsheetFunctionArgumentNameStringId) (functionCode * 1000 + i), culture);
				}
				if(String.IsNullOrEmpty(parameterName)) {
					CustomFunctionDescription customFunctionDescription;
					if(DocumentModel.CustomFunctionsDescriptions.TryGetValue(function.Name, out customFunctionDescription) && customFunctionDescription.ParametersName.Count > i)
						parameterName = customFunctionDescription.ParametersName[i];
				}
				if(String.IsNullOrEmpty(parameterName)) {
					break;
				}
				parametersNames.Add(parameterName);
			}
			if(function.Parameters.Count > 0 && function.Parameters[function.Parameters.Count - 1].Unlimited) {
				parametersNames.Add("...");
			}
		}
		#endregion
		public void ApplyChanges() {
			InsertFunctionCommand command = new InsertFunctionCommand(control);
			command.ApplyChanges(Function, selectionStart, selectionLength);
		}
		public void DiscardChanges() {
			InplaceCancelEditCommand command = new InplaceCancelEditCommand(control);
			command.Execute();
		}
	}
	#endregion
}
