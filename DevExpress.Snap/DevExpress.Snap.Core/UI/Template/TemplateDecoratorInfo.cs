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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native;
using DevExpress.Office.Utils;
namespace DevExpress.Snap.Core.UI.Template {	
	public enum TemplateDecoratorType {
		DataRow = 1,
		GroupHeader = 2,
		GroupFooter = 3,
		ListHeader = 4,
		ListFooter = 5,
		Separator = 6,
		GroupSeparator = 7,
		WholeList = 1000,
		WholeGroup = 1001
	}
	public interface ITemplateDecoratorInfo {
		SnapPieceTable PieceTable { get; }
		RunInfo Interval { get; }
		TemplateDecoratorType DecoratorType { get; }
	}
	public abstract class TemplateDecoratorInfoBase : ITemplateDecoratorInfo {		
		protected TemplateDecoratorInfoBase(SnapPieceTable pieceTable, RunInfo contentInterval) {
			PieceTable = pieceTable;
			Interval = contentInterval;			
		}
		public SnapPieceTable PieceTable { get; private set; }
		public RunInfo Interval { get; private set; }
		public abstract TemplateDecoratorType DecoratorType { get; }
	}
	public abstract class TemplateDecoratorInfoSingleItem : TemplateDecoratorInfoBase {
		public static TemplateDecoratorInfoSingleItem Create(SnapPieceTable pieceTable, RunInfo contentInterval, TemplateDecoratorType decoratorType) {
			switch(decoratorType) {
				case TemplateDecoratorType.DataRow: return new TemplateDecoratorInfoDataRow(pieceTable, contentInterval);
				case TemplateDecoratorType.GroupFooter: return new TemplateDecoratorInfoGroupFooter(pieceTable, contentInterval);
				case TemplateDecoratorType.GroupHeader: return new TemplateDecoratorInfoGroupHeader(pieceTable, contentInterval);
				case TemplateDecoratorType.ListHeader: return new TemplateDecoratorInfoListHeader(pieceTable, contentInterval);
				case TemplateDecoratorType.ListFooter: return new TemplateDecoratorInfoListFooter(pieceTable, contentInterval);
				case TemplateDecoratorType.Separator: return new TemplateDecoratorInfoSeparator(pieceTable, contentInterval);
				case TemplateDecoratorType.GroupSeparator: return new TemplateDecoratorInfoGroupSeparator(pieceTable, contentInterval);
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
		protected TemplateDecoratorInfoSingleItem(SnapPieceTable pieceTable, RunInfo contentInterval)
			: base(pieceTable, contentInterval) {			
		}
	}
	public class TemplateDecoratorInfoDataRow : TemplateDecoratorInfoSingleItem {
		public TemplateDecoratorInfoDataRow(SnapPieceTable pieceTable, RunInfo contentInterval)
			: base(pieceTable, contentInterval) {			
		}
		public override TemplateDecoratorType DecoratorType { get { return TemplateDecoratorType.DataRow; } }
	}
	public class TemplateDecoratorInfoSeparator : TemplateDecoratorInfoSingleItem {
		public TemplateDecoratorInfoSeparator(SnapPieceTable pieceTable, RunInfo contentInterval)
			: base(pieceTable, contentInterval) {			
		}
		public override TemplateDecoratorType DecoratorType { get { return TemplateDecoratorType.Separator; } }
	}
	public class TemplateDecoratorInfoGroupSeparator : TemplateDecoratorInfoSingleItem {
		public TemplateDecoratorInfoGroupSeparator(SnapPieceTable pieceTable, RunInfo contentInterval)
			: base(pieceTable, contentInterval) {			
		}
		public override TemplateDecoratorType DecoratorType { get { return TemplateDecoratorType.GroupSeparator; } }
	}
	public class TemplateDecoratorInfoGroupFooter : TemplateDecoratorInfoSingleItem {
		public TemplateDecoratorInfoGroupFooter(SnapPieceTable pieceTable, RunInfo contentInterval)
			: base(pieceTable, contentInterval) {
		}
		public override TemplateDecoratorType DecoratorType { get { return TemplateDecoratorType.GroupFooter; } }
	}
	public class TemplateDecoratorInfoGroupHeader : TemplateDecoratorInfoSingleItem {
		public TemplateDecoratorInfoGroupHeader(SnapPieceTable pieceTable, RunInfo contentInterval)
			: base(pieceTable, contentInterval) {
		}
		public override TemplateDecoratorType DecoratorType { get { return TemplateDecoratorType.GroupHeader; } }
	}
	public class TemplateDecoratorInfoListFooter : TemplateDecoratorInfoSingleItem {
		public TemplateDecoratorInfoListFooter(SnapPieceTable pieceTable, RunInfo contentInterval)
			: base(pieceTable, contentInterval) {
		}
		public override TemplateDecoratorType DecoratorType { get { return TemplateDecoratorType.ListFooter; } }
	}
	public class TemplateDecoratorInfoListHeader : TemplateDecoratorInfoSingleItem {
		public TemplateDecoratorInfoListHeader(SnapPieceTable pieceTable, RunInfo contentInterval)
			: base(pieceTable, contentInterval) {
		}
		public override TemplateDecoratorType DecoratorType { get { return TemplateDecoratorType.ListHeader; } }
	}
	public abstract class TemplateDecoratorInfoSeverItemsBase : TemplateDecoratorInfoBase {
		RunInfo selectionInterval;
		protected TemplateDecoratorInfoSeverItemsBase(SnapPieceTable pieceTable, RunInfo contentInterval, RunInfo selectionInterval)
			: base(pieceTable, contentInterval) {
				this.selectionInterval = selectionInterval;
		}
	}
	public class TemplateDecoratorInfoWholeList : TemplateDecoratorInfoSeverItemsBase {
		public TemplateDecoratorInfoWholeList(SnapPieceTable pieceTable, RunInfo contentInterval, RunInfo selectionInterval)
			: base(pieceTable, contentInterval, selectionInterval) {
		}
		public override TemplateDecoratorType DecoratorType { get { return TemplateDecoratorType.WholeList; } }
	}
	public class TemplateDecoratorInfoWholeGroup : TemplateDecoratorInfoSeverItemsBase {
		public TemplateDecoratorInfoWholeGroup(SnapPieceTable pieceTable, RunInfo contentInterval, RunInfo selectionInterval)
			: base(pieceTable, contentInterval, selectionInterval) {
		}
		public override TemplateDecoratorType DecoratorType { get { return TemplateDecoratorType.WholeGroup; } }
	}
}
