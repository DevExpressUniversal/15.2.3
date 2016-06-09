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
	public struct VerticalGridElementInfo {
		public VerticalGridElements ElementType;
		public int RecordIndex;
		public int CellIndex;
		public string RowName;
		public string RowCaption;
		public int SeparatorIndex;
		public int CaptionIndex;
	}
	[Serializable]
	public struct VerticalGridElementVariableInfo {
		public bool IsElementFound;
		public string DisplayText;
		public ValueStruct Value;
		public bool Expanded;
		public int VisibleIndex;
		public int Width;
		public int Height;
		public string Style;
		public string Fixed;
		public string ParentRow;
		public int ChildrenCount;
		public string CellLengths;
		public string HeaderCellLengths;
	}
	[Serializable]
	public struct PropertyDescriptionVariableInfo {
		public bool IsElementFound;
		public string DisplayedProperty;
		public string Description;
		public string ParentCategory;
	}
	public enum VerticalGridElements {
		Unknown,
		Row,
		CategoryRow,
		Cell,
		HeaderCell,
		ExpandButton,
		RecordValueEdge,
		HeaderSeparator,
		RowEdge,
		MultiEditorCellSeparator,
		BandEdge,
		HeaderCellImage,
		CustomizationForm
	}
} 
