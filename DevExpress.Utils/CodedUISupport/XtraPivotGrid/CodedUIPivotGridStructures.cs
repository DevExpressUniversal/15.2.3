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
namespace DevExpress.Utils.CodedUISupport {
	[Serializable]
	public struct PivotGridElementInfo {
		public PivotGridElements ElementType;
		public string ColumnFieldName;
		public ValueStruct ColumnFieldValue;
		public string RowFieldName;
		public ValueStruct RowFieldValue;
		public string DataFieldName;
		public string FieldName;
		public ValueStruct FieldValueValue;
		public ValueStruct CellValue;
		public int ColumnIndex;
		public int RowIndex;
		public PivotAreaType Area;
		public PivotFieldValueType FieldValueType;
		public int FieldValueLastLevelIndex;
		public int FieldValueLevel;
		public string DisplayText;
		public int AreaIndex;
		public bool Visible;
		public int FieldValueWidth;
		public int CustomizationListBoxItemIndex;
	}
	public enum PivotGridElements {
		Unknown,
		Cell,
		FieldHeader,
		FieldHeaderFilterButton,
		FieldValue,
		HeadersArea,
		FieldValueExpandButton,
		FieldValueEdge,
		FieldHeaderExpandButton,
		CustomizationListBoxItem
	}
	public enum PivotFieldValueType : int {
		Value,
		RowGrandTotal,
		ColumnGrandTotal
	}
	public enum PivotAreaType : int {
		RowArea = 0,
		ColumnArea = 1,
		FilterArea = 2,
		DataArea = 3,
	}
	public enum PivotElementPropertiesForSet {
		Undefined,
		Visibility,
		Position,
		Width
	}
}
