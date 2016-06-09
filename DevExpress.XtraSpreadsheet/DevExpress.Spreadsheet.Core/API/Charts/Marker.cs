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

namespace DevExpress.Spreadsheet.Charts {
	public enum MarkerStyle {
		Auto = DevExpress.XtraSpreadsheet.Model.MarkerStyle.Auto,
		None = DevExpress.XtraSpreadsheet.Model.MarkerStyle.None,
		Circle = DevExpress.XtraSpreadsheet.Model.MarkerStyle.Circle,
		Dash = DevExpress.XtraSpreadsheet.Model.MarkerStyle.Dash,
		Diamond = DevExpress.XtraSpreadsheet.Model.MarkerStyle.Diamond,
		Dot = DevExpress.XtraSpreadsheet.Model.MarkerStyle.Dot,
		Picture = DevExpress.XtraSpreadsheet.Model.MarkerStyle.Picture,
		Plus = DevExpress.XtraSpreadsheet.Model.MarkerStyle.Plus,
		Square = DevExpress.XtraSpreadsheet.Model.MarkerStyle.Square,
		Star = DevExpress.XtraSpreadsheet.Model.MarkerStyle.Star,
		Triangle = DevExpress.XtraSpreadsheet.Model.MarkerStyle.Triangle,
		X = DevExpress.XtraSpreadsheet.Model.MarkerStyle.X
	}
	public interface Marker : ShapeFormat {
		MarkerStyle Symbol { get; set; }
		int Size { get; set; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.XtraSpreadsheet;
	using DevExpress.Spreadsheet.Charts;
	#region NativeMarker
	partial class NativeMarker : NativeShapeFormat, Marker {
		readonly Model.Marker modelMarker;
		public NativeMarker(Model.Marker modelMarker, NativeWorkbook nativeWorkbook)
			: base(modelMarker.ShapeProperties, nativeWorkbook) {
			this.modelMarker = modelMarker;
		}
		#region DevExpress.Spreadsheet.Marker Members
		public MarkerStyle Symbol {
			get {
				CheckValid();
				return (MarkerStyle)modelMarker.Symbol;
			}
			set {
				CheckValid();
				modelMarker.Symbol = (Model.MarkerStyle)value;
			}
		}
		public int Size {
			get {
				CheckValid();
				return modelMarker.Size;
			}
			set {
				CheckValid();
				modelMarker.Size = value;
			}
		}
		#endregion
	}
	#endregion
}
