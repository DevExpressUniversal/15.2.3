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
using System.Windows;
using System.Windows.Input;
using DevExpress.Data;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Spreadsheet;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Utils.Controls;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.API.Native;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Utils.Commands;
using DevExpress.Xpf.Bars.Native;
#else
using DevExpress.Xpf.Utils;
using DevExpress.XtraSpreadsheet.Forms;
#endif
namespace DevExpress.Xpf.Spreadsheet.UI {
	#region InsertFunctionsBarSubItemBase (abstract class)
	public abstract class InsertFunctionsBarSubItemBase : BarSubItem {
		public static readonly DependencyProperty SpreadsheetControlProperty = DependencyPropertyManager.Register("SpreadsheetControl", typeof(SpreadsheetControl), typeof(InsertFunctionsBarSubItemBase));
		protected InsertFunctionsBarSubItemBase() {
			this.GetItemData += OnGetItemData;
		}
		public SpreadsheetControl SpreadsheetControl {
			get { return (SpreadsheetControl)GetValue(SpreadsheetControlProperty); }
			set { SetValue(SpreadsheetControlProperty, value); }
		}
		protected static IList<string> GetFunctionList(Dictionary<string, ISpreadsheetFunction> functions) {
			List<string> result = new List<string>();
			foreach (string key in functions.Keys) {
				ISpreadsheetFunction function = functions[key];
				if (!(function is NotSupportedFunction))
					result.Add(key);
			}
			result.Sort();
			return result;
		}
		void OnGetItemData(object sender, EventArgs e) {
			UpdateItems();
		}
		protected internal void UpdateItems() {
			if (SpreadsheetControl != null)
				UpdateItemsCore();
		}
		protected internal virtual void UpdateItemsCore() {
			foreach (string functionName in GetFunctionList())
				AppendItem(functionName);
		}
		protected internal virtual void AppendItem(string functionName) {
			BarButtonItem item = new BarButtonItem();
			item.Content = functionName;
			item.Command = CreateInsertFunctionCommand(functionName);
			item.CommandParameter = SpreadsheetControl;
			ItemLinks.Add(item);
		}
		protected internal ICommand CreateInsertFunctionCommand(string name) {
			return new InsertFunctionSpreadsheetUICommand(name);
		}
		protected abstract IList<string> GetFunctionList();
	}
	#endregion
	#region InsertFinancialFunctionsBarSubItem
	public class InsertFinancialFunctionsBarSubItem : InsertFunctionsBarSubItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddFinancialFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region InsertLogicalFunctionsBarSubItem
	public class InsertLogicalFunctionsBarSubItem : InsertFunctionsBarSubItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddLogicalFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region InsertTextFunctionsBarSubItem
	public class InsertTextFunctionsBarSubItem : InsertFunctionsBarSubItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddTextFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region InsertDateAndTimeFunctionsBarSubItem
	public class InsertDateAndTimeFunctionsBarSubItem : InsertFunctionsBarSubItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddDateTimeFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region InsertLookupAndReferenceFunctionsBarSubItem
	public class InsertLookupAndReferenceFunctionsBarSubItem : InsertFunctionsBarSubItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddLookupAndReferenceFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region InsertMathAndTrigonometryFunctionsBarSubItem
	public class InsertMathAndTrigonometryFunctionsBarSubItem : InsertFunctionsBarSubItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddMathAndTrigonometryFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region InsertStatisticalFunctionsBarSubItem
	public class InsertStatisticalFunctionsBarSubItem : InsertFunctionsBarSubItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddStatisticalFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region InsertEngineeringFunctionsBarSubItem
	public class InsertEngineeringFunctionsBarSubItem : InsertFunctionsBarSubItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddEngineeringFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region InsertCubeFunctionsBarSubItem
	public class InsertCubeFunctionsBarSubItem : InsertFunctionsBarSubItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddCubeFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region InsertInformationFunctionsBarSubItem
	public class InsertInformationFunctionsBarSubItem : InsertFunctionsBarSubItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddInformationalFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region InsertCompatibilityFunctionsBarSubItem
	public class InsertCompatibilityFunctionsBarSubItem : InsertFunctionsBarSubItemBase {
		static readonly IList<string> functions = CreateFunctionList();
		static IList<string> CreateFunctionList() {
			Dictionary<string, ISpreadsheetFunction> dictionary = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddCompatibilityFunctions(dictionary);
			return GetFunctionList(dictionary);
		}
		protected override IList<string> GetFunctionList() {
			return functions;
		}
	}
	#endregion
	#region InsertDefinedNamesBarSubItem
	public class InsertDefinedNamesBarSubItem : BarSubItem {
		public static readonly DependencyProperty SpreadsheetControlProperty = DependencyPropertyManager.Register("SpreadsheetControl", typeof(SpreadsheetControl), typeof(InsertDefinedNamesBarSubItem));
		public InsertDefinedNamesBarSubItem() {
			this.GetItemData += OnGetItemData;
		}
		public SpreadsheetControl SpreadsheetControl {
			get { return (SpreadsheetControl)GetValue(SpreadsheetControlProperty); }
			set { SetValue(SpreadsheetControlProperty, value); }
		}
		void OnGetItemData(object sender, EventArgs e) {
			UpdateItems();
		}
		protected internal void UpdateItems() {
			if (SpreadsheetControl != null)
				UpdateItemsCore();
		}
		protected internal virtual void UpdateItemsCore() {
			foreach (string definedName in GetDefinedNameList())
				AppendItem(definedName);
		}
		protected internal virtual void AppendItem(string definedName) {
			BarButtonItem item = new BarButtonItem();
			item.Content = definedName;
			item.Command = CreateInsertDefinedNameCommand(definedName);
			item.CommandParameter = SpreadsheetControl;
			ItemLinks.Add(item);
		}
		protected internal ICommand CreateInsertDefinedNameCommand(string name) {
			return new InsertDefinedNameSpreadsheetUICommand(name);
		}
		protected IList<string> GetDefinedNameList() {
			if (SpreadsheetControl == null)
				return new List<string>();
			NameManagerViewModel viewModel = new NameManagerViewModel(SpreadsheetControl);
			return viewModel.GetAvailableDefinedNameList(SpreadsheetControl.DocumentModel.ActiveSheet);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertFunctionSpreadsheetUICommand
	public class InsertFunctionSpreadsheetUICommand : SpreadsheetUICommand {
		readonly string fieldName;
		public InsertFunctionSpreadsheetUICommand(string fieldName)
			: base(SpreadsheetCommandId.FunctionsInsertSpecificFunction) {
			this.fieldName = fieldName;
		}
		protected internal override void ExecuteCommand(SpreadsheetControl control, SpreadsheetCommandId commandId, object parameter) {
			base.ExecuteCommand(control, commandId, fieldName);
		}
	}
	#endregion
	#region InsertDefinedNameSpreadsheetUICommand
	public class InsertDefinedNameSpreadsheetUICommand : SpreadsheetUICommand {
		readonly string fieldName;
		public InsertDefinedNameSpreadsheetUICommand(string fieldName)
			: base(SpreadsheetCommandId.FormulasInsertDefinedName) {
			this.fieldName = fieldName;
		}
		protected internal override void ExecuteCommand(SpreadsheetControl control, SpreadsheetCommandId commandId, object parameter) {
			base.ExecuteCommand(control, commandId, fieldName);
		}
	}
	#endregion
}
