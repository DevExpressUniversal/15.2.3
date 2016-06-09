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
using System.ComponentModel;
using DevExpress.Office;
using DevExpress.Spreadsheet.Drawings;
namespace DevExpress.Spreadsheet.Charts {
	public enum DisplayUnitType {
		None = DevExpress.XtraSpreadsheet.Model.DisplayUnitType.None,
		Billions = DevExpress.XtraSpreadsheet.Model.DisplayUnitType.Billions,
		HundredMillions = DevExpress.XtraSpreadsheet.Model.DisplayUnitType.HundredMillions,
		Hundreds = DevExpress.XtraSpreadsheet.Model.DisplayUnitType.Hundreds,
		HundredThousands = DevExpress.XtraSpreadsheet.Model.DisplayUnitType.HundredThousands,
		Millions = DevExpress.XtraSpreadsheet.Model.DisplayUnitType.Millions,
		TenMillions = DevExpress.XtraSpreadsheet.Model.DisplayUnitType.TenMillions,
		TenThousands = DevExpress.XtraSpreadsheet.Model.DisplayUnitType.TenThousands,
		Thousands = DevExpress.XtraSpreadsheet.Model.DisplayUnitType.Thousands,
		Trillions = DevExpress.XtraSpreadsheet.Model.DisplayUnitType.Trillions,
		Custom = DevExpress.XtraSpreadsheet.Model.DisplayUnitType.Custom
	}
	public interface DisplayUnitOptions : ShapeFormat, ShapeTextFormat {
		DisplayUnitType UnitType { get; set; }
		double CustomUnit { get; set; }
		bool ShowLabel { get; set; }
		ChartText Label { get; }
		LayoutOptions Layout { get; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	partial class NativeDisplayUnitOptions : NativeShapeTextFormat, DisplayUnitOptions {
		#region Fields
		readonly Model.DisplayUnitOptions modelDisplayUnits;
		readonly NativeWorkbook nativeWorkbook;
		NativeShapeFormat nativeShapeFormat;
		NativeChartText nativeChartText;
		NativeLayoutOptions nativeLayout;
		#endregion
		public NativeDisplayUnitOptions(Model.DisplayUnitOptions modelDisplayUnits, NativeWorkbook nativeWorkbook)
			: base(modelDisplayUnits) {
			this.modelDisplayUnits = modelDisplayUnits;
			this.nativeWorkbook = nativeWorkbook;
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeShapeFormat != null)
				nativeShapeFormat.IsValid = value;
			if (nativeChartText != null)
				nativeChartText.IsValid = value;
			if (nativeLayout != null)
				nativeLayout.IsValid = value;
		}
		#region DisplayUnitOptions Members
		#region UnitType
		public DisplayUnitType UnitType {
			get {
				CheckValid();
				return (DisplayUnitType)modelDisplayUnits.UnitType;
			}
			set {
				CheckValid();
				modelDisplayUnits.UnitType = (Model.DisplayUnitType)value;
			}
		}
		#endregion
		#region CustomUnit
		public double CustomUnit {
			get {
				CheckValid();
				return modelDisplayUnits.CustomUnit;
			}
			set {
				CheckValid();
				modelDisplayUnits.CustomUnit = value;
			}
		}
		#endregion
		#region ShowLabel
		public bool ShowLabel {
			get {
				CheckValid();
				return modelDisplayUnits.ShowLabel;
			}
			set {
				CheckValid();
				modelDisplayUnits.ShowLabel = value;
			}
		}
		#endregion
		#region Label
		public ChartText Label {
			get {
				CheckValid();
				if (nativeChartText == null)
					nativeChartText = new NativeChartText(modelDisplayUnits);
				return nativeChartText;
			}
		}
		#endregion
		#region Layout
		public LayoutOptions Layout {
			get {
				CheckValid();
				if (nativeLayout == null)
					nativeLayout = new NativeLayoutOptions(modelDisplayUnits.Layout);
				return nativeLayout;
			}
		}
		#endregion
		#region ShapeFormat Members
		public ShapeFill Fill {
			get {
				CheckValid();
				CheckShapeFormat();
				return nativeShapeFormat.Fill;
			}
		}
		public ShapeOutline Outline {
			get {
				CheckValid();
				CheckShapeFormat();
				return nativeShapeFormat.Outline;
			}
		}
		public void ResetToMatchStyle() {
			CheckValid();
			CheckShapeFormat();
			nativeShapeFormat.ResetToMatchStyle();
		}
		#endregion
		#endregion
		#region Internal
		void CheckShapeFormat() {
			if (nativeShapeFormat == null)
				nativeShapeFormat = new NativeShapeFormat(modelDisplayUnits.ShapeProperties, nativeWorkbook);
		}
		#endregion
	}
}
