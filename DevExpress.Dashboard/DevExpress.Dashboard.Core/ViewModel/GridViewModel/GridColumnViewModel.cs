#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.ViewModel {
	public enum GridDeltaDisplayMode { None, Value, Delta };
	public enum GridColumnDisplayMode { Value, Delta, Bar, Image, Sparkline };
	public enum GridColumnType { Dimension, Measure, Delta, Sparkline };
	public enum GridColumnHorzAlignment { Left, Right };
	public enum GridColumnFilterMode { Value = 0, DisplayText = 1 }
	public class GridColumnViewModel {
		public string Caption { get; set; }
		public string DataId { get; set; }
		public GridColumnType ColumnType { get; set; }
		public GridDeltaDisplayMode DeltaDisplayMode { get; set; }
		public GridColumnDisplayMode DisplayMode { get; set; }
		public bool IgnoreDeltaColor { get; set; }
		public bool IgnoreDeltaIndication { get; set; }
		public GridBarViewModel BarViewModel { get; set; }
		public bool AllowCellMerge { get; set; }
		public GridColumnHorzAlignment HorzAlignment { get; set; }
		public bool ShowStartEndValues { get; set; }
		public SparklineOptionsViewModel SparklineOptions { get; set; }
		public GridColumnFilterMode GridColumnFilterMode { get; set; }
		public double Weight { get; set; }
		public double FixedWidth { get; set; }
		public GridColumnFixedWidthType WidthType { get; set; }
		public int ActualIndex { get; set; }
		public double DefaultBestCharacterCount { get; set; }
		public DeltaValueType DeltaValueType { get; set; }
		public IList<GridColumnTotalViewModel> Totals { get; set; }
		public DataFieldType DataType { get; set; }
		public GridColumnViewModel() {
			Totals = new List<GridColumnTotalViewModel>();
		}
	}
}
