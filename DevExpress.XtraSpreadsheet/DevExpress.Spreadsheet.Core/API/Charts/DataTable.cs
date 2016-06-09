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
	public interface DataTableOptions : ShapeFormat {
		bool Visible { get; set; }
		bool ShowHorizontalBorder { get; set; }
		bool ShowVerticalBorder { get; set; }
		bool ShowOutline { get; set; }
		bool ShowLegendKeys { get; set; }
		ShapeTextFont Font { get; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	partial class NativeDataTableOptions : NativeShapeFormat, DataTableOptions {
		#region Fields
		readonly Model.DataTableOptions modelDataTableOptions;
		NativeShapeTextFont font;
		#endregion
		public NativeDataTableOptions(Model.DataTableOptions modelDataTableOptions, NativeWorkbook nativeWorkbook)
			: base(modelDataTableOptions.ShapeProperties, nativeWorkbook) {
			this.modelDataTableOptions = modelDataTableOptions;
		}
		#region DataTableOptions Members
		public bool Visible {
			get {
				CheckValid();
				return modelDataTableOptions.Visible;
			}
			set {
				CheckValid();
				modelDataTableOptions.Visible = value;
			}
		}
		public bool ShowHorizontalBorder {
			get {
				CheckValid();
				return modelDataTableOptions.ShowHorizontalBorder;
			}
			set {
				CheckValid();
				modelDataTableOptions.ShowHorizontalBorder = value;
			}
		}
		public bool ShowVerticalBorder {
			get {
				CheckValid();
				return modelDataTableOptions.ShowVerticalBorder;
			}
			set {
				CheckValid();
				modelDataTableOptions.ShowVerticalBorder = value;
			}
		}
		public bool ShowOutline {
			get {
				CheckValid();
				return modelDataTableOptions.ShowOutline;
			}
			set {
				CheckValid();
				modelDataTableOptions.ShowOutline = value;
			}
		}
		public bool ShowLegendKeys {
			get {
				CheckValid();
				return modelDataTableOptions.ShowLegendKeys;
			}
			set {
				CheckValid();
				modelDataTableOptions.ShowLegendKeys = value;
			}
		}
		public ShapeTextFont Font {
			get {
				CheckValid();
				if (font == null)
					font = new NativeShapeTextFont(modelDataTableOptions.TextProperties);
				return font; 
			}
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (font != null)
				font.IsValid = value;
		}
		public override void ResetToMatchStyle() {
			CheckValid();
			modelDataTableOptions.DocumentModel.BeginUpdate();
			try {
				modelDataTableOptions.ResetToStyle();
			}
			finally {
				modelDataTableOptions.DocumentModel.EndUpdate();
			}
		}
	}
}
