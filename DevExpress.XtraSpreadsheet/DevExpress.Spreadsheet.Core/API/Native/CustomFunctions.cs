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

using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Spreadsheet.Functions {
	#region ICustomFunction
	[Obsolete("Use ICustomFunction interface instead.", false)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface CustomFunction : ICustomFunction {
	}
	public interface IFunction {
		string Name { get; }
		ParameterType ReturnType { get; }
		ParameterInfo[] Parameters { get; }
		bool Volatile { get; }
		ParameterValue Evaluate(IList<ParameterValue> parameters, EvaluationContext context);
		string GetName(CultureInfo culture);
	}
	public interface ICustomFunction : IFunction {
	}
	public interface IBuiltInFunction : IFunction {
		ParameterValue Evaluate(params ParameterValue[] parameters);
	}
	#endregion
	#region CustomFunctions
	public interface CustomFunctionCollection : ICollection<ICustomFunction> {
		ICustomFunction this[string name] { get; }
		bool Remove(string name);
		bool Contains(string name);
	}
	#endregion
	#region ICustomFunctionDescriptionsRegisterService
	public interface ICustomFunctionDescriptionsRegisterService {
		void RegisterFunctionDescriptions(string functionName, string functionDescription, CustomFunctionArgumentsDescriptionsCollection argumentsDescriptions);
		void UnregisterFunction(string functionName);
	}
	#endregion
	#region CustomFunctionArgumentsDescriptionsCollection
	public class CustomFunctionArgumentsDescriptionsCollection : List<CustomFunctionArgumentDescription> { }
	#endregion
	#region CustomFunctionArgumentDescription
	public class CustomFunctionArgumentDescription {
		public string Name { get; set; }
		public string Description { get; set; }
		public string ResultType { get; set; }
		public CustomFunctionArgumentDescription(string name, string description, string resultType) {
			Name = name;
			Description = description;
			ResultType = resultType;
		}
	}
	#endregion
}
#region Custom functions implementation
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Office.Utils;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Functions;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Utils;
	using ModelCustomFunction = DevExpress.XtraSpreadsheet.Model.CustomFunction;
	using ModelFormulaCalculator = DevExpress.XtraSpreadsheet.Model.FormulaCalculator;
	using ModelWorkbookDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
	using ModelCustomFunctionsDescription = DevExpress.XtraSpreadsheet.Model.CustomFunctionDescription;
	using DevExpress.Spreadsheet.Formulas;
	using DevExpress.XtraSpreadsheet.Model;
	#region NativeCustomFunctions
	partial class NativeCustomFunctions : NativeFunctionsBase, CustomFunctionCollection {
		#region static
		[ThreadStatic]
		static Dictionary<string, ICustomFunction> globalInnerList;
		public static Dictionary<string, ICustomFunction> GlobalInnerList {
			get {
				if (globalInnerList == null)
					globalInnerList = new Dictionary<string, ICustomFunction>();
				return globalInnerList;
			}
		}
		#endregion
		public NativeCustomFunctions(NativeWorkbook workbook, Model.ICustomFunctionProvider customFunctionProvider)
			: this(workbook, customFunctionProvider, false) {
		}
		public NativeCustomFunctions(NativeWorkbook workbook, Model.ICustomFunctionProvider customFunctionProvider, bool isGlobal)
			: base(workbook, customFunctionProvider) {
			if (isGlobal)
				InnerList = GlobalInnerList;
			else
				Initialize();
		}
		public void Initialize() {
			InnerList = new Dictionary<string, ICustomFunction>();
			foreach (Model.ISpreadsheetFunction function in FunctionProvider.FunctionsByInvariantName.Values) {
				Model.CustomFunction customFunction = function as Model.CustomFunction;
				if (customFunction != null)
					InnerList.Add(customFunction.Name, customFunction.NativeCustomFunction);
			}
		}
		bool CheckParameters(ParameterInfo[] parameters) {
			if (parameters == null || parameters.Length == 0)
				return true;
			int length = parameters.Length;
			if (length > 255)
				return false;
			bool wasOptional = false;
			for (int i = 0; i < parameters.Length; i++) {
				ParameterInfo parameter = parameters[i];
				if (parameter == null)
					return false;
				if (parameter.Attributes == ParameterAttributes.OptionalUnlimited && i < (length - 1))
					return false;
				if ((parameter.Attributes & ParameterAttributes.Optional) > 0)
					wasOptional = true;
				else
					if (wasOptional)
						return false;
			}
			return true;
		}
		#region CustomFunctions Members
		public ICustomFunction this[string name] {
			get {
				if (string.IsNullOrEmpty(name))
					return null;
				ICustomFunction result = null;
				if (InnerList.TryGetValue(name, out result))
					return result;
				return null;
			}
		}
		public bool Remove(string name) {
			name = name.ToUpper();
			if (!String.IsNullOrEmpty(name)) {
				InnerList.Remove(name);
				return FunctionProvider.UnregisterFunction(name);
			}
			return false;
		}
		#endregion
		#region ICollection<ICustomFunction> Members
		public void Add(ICustomFunction item) {
			if (item == null)
				throw new ArgumentNullException("item");
			string name = item.Name;
			if (Contains(name) || FunctionProvider.IsFunctionRegistered(name) || ModelFormulaCalculator.FunctionProvider.IsFunctionRegistered(name))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_CustomFunctionAlreadyDefined);
			if (!ModelCustomFunction.ValidateName(name))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_CustomFunctionInvalidName);
			if (!CheckParameters(item.Parameters))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_CustomFunctionInvalidParameters);
			Model.FunctionParameterCollection parameters = PrepareParameters(item.Parameters);
			ModelCustomFunction modelCustomFunction = new ModelCustomFunction(item, parameters);
			FunctionProvider.RegisterFunction(name, modelCustomFunction, Workbook.ModelWorkbook.DataContext.Culture);
			InnerList.Add(name, item);
		}
		public void Clear() {
			FunctionProvider.ClearFunctions();
			InnerList.Clear();
		}
		public bool Contains(ICustomFunction item) {
			if (item == null)
				return false;
			string name = item.Name;
			if (!string.IsNullOrEmpty(name))
				return InnerList.ContainsKey(name);
			return false;
		}
		public void CopyTo(ICustomFunction[] array, int index) {
			InnerList.Values.CopyTo(array, index);
		}
		public int Count { get { return InnerList.Count; } }
		public bool IsReadOnly { get { return false; } }
		public bool Remove(ICustomFunction item) {
			return Remove(item.Name);
		}
		public new bool Contains(string name) {
			return base.Contains(name);
		}
		#endregion
		#region IEnumerable<ICustomFunction> Members
		public IEnumerator<ICustomFunction> GetEnumerator() {
			return InnerList.Values.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
	}
	#endregion
	#region NativeCustomFunctionDescriptionsRegisterService
	partial class NativeCustomFunctionDescriptionsRegisterService : ICustomFunctionDescriptionsRegisterService {
		#region Properties
		DocumentModel Workbook { get; set; }
		Dictionary<string, ModelCustomFunctionsDescription> CustomFunctionDescriptions { get { return Workbook.CustomFunctionsDescriptions; } }
		#endregion
		#region Implementation of ICustomFunctionDescriptionsRegisterService
		public void RegisterFunctionDescriptions(string functionName, string functionDescription, CustomFunctionArgumentsDescriptionsCollection argumentsDescriptions) {
			if(CustomFunctionDescriptions.ContainsKey(functionName))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_CustomFunctionAlreadyDefined);
			ModelCustomFunctionsDescription modelFunctionDescription = new ModelCustomFunctionsDescription();
			modelFunctionDescription.Description = functionDescription;
			if(argumentsDescriptions == null)
				return;
			foreach(CustomFunctionArgumentDescription argumentDescription in argumentsDescriptions) {
				modelFunctionDescription.ParametersDescription.Add(argumentDescription.Description);
				modelFunctionDescription.ParametersName.Add(argumentDescription.Name);
				modelFunctionDescription.ReturnTypes.Add(argumentDescription.ResultType);
			}
			CustomFunctionDescriptions.Add(functionName, modelFunctionDescription);
		}
		public void UnregisterFunction(string functionName) {
			if(CustomFunctionDescriptions.ContainsKey(functionName))
				CustomFunctionDescriptions.Remove(functionName);
		}
		#endregion
		public NativeCustomFunctionDescriptionsRegisterService(DocumentModel workbook) {
			Workbook = workbook;
		}
	}
	#endregion
}
#endregion
