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
namespace DevExpress.Spreadsheet {
	#region Mode
	public enum CalculationMode {
		Automatic = DevExpress.XtraSpreadsheet.Model.ModelCalculationMode.Automatic,
		AutomaticExceptTables = DevExpress.XtraSpreadsheet.Model.ModelCalculationMode.AutomaticExceptTables,
		Manual = DevExpress.XtraSpreadsheet.Model.ModelCalculationMode.Manual
	}
	#endregion
	#region DocumentSettings
	public interface DocumentSettings {
		bool R1C1ReferenceStyle { get; set; }
		CalculationOptions Calculation { get; }
		bool ShowPivotTableFieldList { get; set; }
	}
	#endregion
	#region CalculationOptions
	public interface CalculationOptions {
		CalculationMode Mode { get; set; }
		bool RecalculateBeforeSaving { get; set; }
		bool Iterative { get; set; }
		int MaxIterationCount { get; set; }
		double MaxChange { get; set; }
		bool Use1904DateSystem { get; set; }
		bool FullCalculationOnLoad { get; set; }
		bool PrecisionAsDisplayed { get; set; }
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Utils;
	using DevExpress.Spreadsheet;
	using ModelWorkbookProperties = DevExpress.XtraSpreadsheet.Model.WorkbookProperties;
	using ModelCalculationOptions = DevExpress.XtraSpreadsheet.Model.CalculationOptions;
	#region NativeDocumentSettings
	partial class NativeDocumentSettings : DocumentSettings {
		#region Fields
		readonly ModelWorkbookProperties modelProperties;
		readonly NativeCalculationOptions calculationOptions;
		#endregion
		public NativeDocumentSettings(ModelWorkbookProperties modelProperties) {
			Guard.ArgumentNotNull(modelProperties, "modelProperties");
			this.modelProperties = modelProperties;
			this.calculationOptions = new NativeCalculationOptions(modelProperties.CalculationOptions);
		}
		protected internal ModelWorkbookProperties ModelProperties { get { return this.modelProperties; } }
		#region Properties
		public bool R1C1ReferenceStyle {
			get { return modelProperties.UseR1C1ReferenceStyle; }
			set { modelProperties.UseR1C1ReferenceStyle = value; }
		}
		public CalculationOptions Calculation { get { return calculationOptions; } }
		public bool ShowPivotTableFieldList {
			get { return !modelProperties.HidePivotFieldList; }
			set { modelProperties.HidePivotFieldList = !value; }
		}
		#endregion
	}
	#endregion
	#region NativeCalculationOptions
	partial class NativeCalculationOptions : CalculationOptions {
		#region Fields
		readonly ModelCalculationOptions modelOptions;
		#endregion
		public NativeCalculationOptions(ModelCalculationOptions modelOptions) {
			Guard.ArgumentNotNull(modelOptions, "modelOptions");
			this.modelOptions = modelOptions;
		}
		#region Properties
		public CalculationMode Mode {
			get { return (CalculationMode)modelOptions.CalculationMode; }
			set {
				modelOptions.CalculationMode = (Model.ModelCalculationMode)value;
				if (modelOptions.CalculationMode == Model.ModelCalculationMode.Automatic)
					modelOptions.DocumentModel.IncrementContentVersion();
			}
		}
		public bool RecalculateBeforeSaving {
			get { return modelOptions.RecalculateBeforeSaving; }
			set { modelOptions.RecalculateBeforeSaving = value; }
		}
		public bool Iterative {
			get { return modelOptions.IterationsEnabled; }
			set { modelOptions.IterationsEnabled = value; }
		}
		public int MaxIterationCount {
			get { return modelOptions.MaximumIterations; }
			set { modelOptions.MaximumIterations = value; }
		}
		public double MaxChange {
			get { return modelOptions.IterativeCalculationDelta; }
			set { modelOptions.IterativeCalculationDelta = value; }
		}
		public bool FullCalculationOnLoad {
			get { return modelOptions.FullCalculationOnLoad; }
			set { modelOptions.FullCalculationOnLoad = value; }
		}
		public bool PrecisionAsDisplayed {
			get { return modelOptions.PrecisionAsDisplayed; }
			set { modelOptions.PrecisionAsDisplayed = value; }
		}
		public bool Use1904DateSystem {
			get { return modelOptions.DateSystem == DevExpress.XtraSpreadsheet.Model.DateSystem.Date1904; }
			set {
				if (value == Use1904DateSystem)
					return;
				if (value)
					modelOptions.DateSystem = DevExpress.XtraSpreadsheet.Model.DateSystem.Date1904;
				else
					modelOptions.DateSystem = DevExpress.XtraSpreadsheet.Model.DateSystem.Date1900;
			}
		}
		#endregion
	}
	#endregion
}
