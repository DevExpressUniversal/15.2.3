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
using System.Text;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Ruler;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeColumnSizeCommand
	public abstract class ChangeColumnSizeCommand : RichEditCommand {
		#region Fields
		readonly int columnIndex;
		readonly int offset;
		readonly SectionProperties sectionProperties;
		readonly int defaultTabWidth;
		#endregion
		protected ChangeColumnSizeCommand(IRichEditControl control, SectionProperties sectionProperties, int columnIndex, int offset)
			: base(control) {
			this.sectionProperties = sectionProperties;
			this.columnIndex = columnIndex;
			this.offset = offset;
			this.defaultTabWidth = DocumentModel.DocumentProperties.DefaultTabWidth;
		}
		#region Properties
		public int Offset { get { return offset; } }
		public int ColumnIndex { get { return columnIndex; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeColumnSize; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeColumnSizeDescription; } }
		protected internal int DefaultTabWidth { get { return defaultTabWidth; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		public override void ForceExecute(ICommandUIState state) {
			ParagraphIndex paragraphIndex = DocumentModel.Selection.Interval.End.ParagraphIndex;
			SectionIndex sectionIndex = DocumentModel.MainPieceTable.LookupSectionIndexByParagraphIndex(paragraphIndex);
			if (columnIndex >= sectionProperties.ColumnCount || sectionIndex < new SectionIndex(0))
				return;
			Control.BeginUpdate();
			try {
				SectionProperties newSectionProperties = GetNewSection(sectionProperties);
				Section targetSection = DocumentModel.Sections[sectionIndex];
				DocumentModel.BeginUpdate();
				try {
					newSectionProperties.CopyToSection(targetSection);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
			finally {
				Control.EndUpdate();
			}
		}
		protected internal int GetLeftMargin(int oldMargin) {
			int newMargin = oldMargin - Offset;
			if (newMargin > 0)
				return newMargin;
			return oldMargin;
		}
		protected internal int GetRightMargin(int oldMargin) {
			int newMargin = oldMargin + Offset;
			if (newMargin > 0)
				return newMargin;
			return oldMargin;
		}
		protected internal abstract SectionProperties GetNewSection(SectionProperties section);
	}
	#endregion
	#region ChangeColumnWidthCommand
	public class ChangeColumnWidthCommand : ChangeColumnSizeCommand {
		public ChangeColumnWidthCommand(IRichEditControl control, SectionProperties sectionProperties, int columnIndex, int offset)
			: base(control, sectionProperties, columnIndex, offset) {
		}
		protected internal override SectionProperties GetNewSection(SectionProperties sectionProperties) {
			int width = sectionProperties.ColumnInfoCollection[ColumnIndex].Width - Offset;
			if (width > DefaultTabWidth)
				ChangeColumnSize(sectionProperties, width);
			return sectionProperties;
		}
		protected internal void ChangeColumnSize(SectionProperties sectionProperties, int width) {
			ColumnInfoCollection columns = sectionProperties.ColumnInfoCollection;
			ColumnInfo column = columns[ColumnIndex].Clone();
			if (ColumnIndex == columns.Count - 1)
				ChangeColumnSizeByMargin(sectionProperties, width, column);
			else
				ChangeColumnSizeBySpace(width, column);
			columns[ColumnIndex] = column;
		}
		protected internal void ChangeColumnSizeBySpace(int width, ColumnInfo column) {
			int space = column.Space + Offset;
			if (width > DefaultTabWidth && space > 0) {
				column.Space = space;
				column.Width = width;
			}
		}
		protected internal void ChangeColumnSizeByMargin(SectionProperties sectionProperties, int width, ColumnInfo column) {
			int newMargin = sectionProperties.RightMargin + Offset;
			if (newMargin > 0) {
				sectionProperties.RightMargin = newMargin;
				column.Width = width;
			}
		}
	}
	#endregion
	#region ChangeColumnSizeByLeftCommand
	public class ChangeColumnSizeByLeftCommand : ChangeColumnSizeCommand {
		public ChangeColumnSizeByLeftCommand(IRichEditControl control, SectionProperties sectionProperties, int columnIndex, int offset)
			: base(control, sectionProperties, columnIndex, offset) {
		}
		protected internal override SectionProperties GetNewSection(SectionProperties sectionProperties) {
			int width = sectionProperties.ColumnInfoCollection[ColumnIndex].Width + Offset;
			if (width > DefaultTabWidth)
				ChangeColumnSize(sectionProperties, width);
			return sectionProperties;
		}
		protected internal void ChangeColumnSize(SectionProperties sectionProperties, int width) {
			ColumnInfoCollection columns = sectionProperties.ColumnInfoCollection;
			ColumnInfo column = columns[ColumnIndex].Clone();
			if (ColumnIndex == 0)
				ChangeColumnSizeByMargin(sectionProperties, column, width);
			else
				ChangeColumnSizeBySpace(columns, column, width);
			columns[ColumnIndex] = column;
		}
		protected internal void ChangeColumnSizeByMargin(SectionProperties sectionProperties, ColumnInfo column, int width) {
			int newMargin = sectionProperties.LeftMargin - Offset;
			if (newMargin > 0) {
				sectionProperties.LeftMargin = newMargin;
				column.Width = width;
			}
		}
		protected internal void ChangeColumnSizeBySpace(ColumnInfoCollection columns, ColumnInfo column, int width) {
			ColumnInfo previousColumn = columns[ColumnIndex - 1].Clone();
			int space = previousColumn.Space - Offset;
			if (space > 0) {
				column.Width = width;
				previousColumn.Space = space;
				columns[ColumnIndex - 1] = previousColumn;
			}
		}
	}
	#endregion
	#region MoveColumnCommand
	public class MoveColumnCommand : ChangeColumnSizeCommand {
		public MoveColumnCommand(IRichEditControl control, SectionProperties sectionProperties, int columnIndex, int offset)
			: base(control, sectionProperties, columnIndex, offset) {
		}
		protected internal override SectionProperties GetNewSection(SectionProperties sectionProperties) {
			ColumnInfoCollection columns = sectionProperties.ColumnInfoCollection;
			ColumnInfo column = columns[ColumnIndex].Clone();
			ColumnInfo nextColumn = columns[ColumnIndex + 1].Clone();
			int width = column.Width + Offset;
			int nextWidth = nextColumn.Width - Offset;
			if (width > DefaultTabWidth && nextWidth > DefaultTabWidth) {
				column.Width = width;
				nextColumn.Width = nextWidth;
			}
			columns[ColumnIndex] = column;
			columns[ColumnIndex + 1] = nextColumn;
			return sectionProperties;
		}
	}
	#endregion
	#region ChangeWidthEqualWidthColumnsByLeftCommand
	public class ChangeWidthEqualWidthColumnsByLeftCommand : ChangeColumnSizeCommand {
		public ChangeWidthEqualWidthColumnsByLeftCommand(IRichEditControl control, SectionProperties sectionProperties, int columnIndex, int offset)
			: base(control, sectionProperties, columnIndex, offset) {
		}
		protected internal override SectionProperties GetNewSection(SectionProperties sectionProperties) {
			if (ColumnIndex == 0) {
				int newMargin = sectionProperties.LeftMargin - Offset;
				int width = (sectionProperties.PageWidth - newMargin - sectionProperties.RightMargin - sectionProperties.Space * sectionProperties.ColumnCount) / sectionProperties.ColumnCount;
				if (newMargin > 0 && width > DefaultTabWidth)
					sectionProperties.LeftMargin = newMargin;
			}
			else {
				int space = sectionProperties.Space - Offset;
				int width = (sectionProperties.PageWidth - sectionProperties.LeftMargin - sectionProperties.RightMargin - space * sectionProperties.ColumnCount) / sectionProperties.ColumnCount;
				if (space > 0 && width > DefaultTabWidth)
					sectionProperties.Space = space;
			}
			return sectionProperties;
		}
	}
	#endregion
	#region ChangeWidthEqualWidthColumnsByRightCommand
	public class ChangeWidthEqualWidthColumnsByRightCommand : ChangeColumnSizeCommand {
		public ChangeWidthEqualWidthColumnsByRightCommand(IRichEditControl control, SectionProperties sectionProperties, int columnIndex, int offset)
			: base(control, sectionProperties, columnIndex, offset) {
		}
		protected internal override SectionProperties GetNewSection(SectionProperties sectionProperties) {
			if (ColumnIndex == sectionProperties.ColumnCount - 1) {
				int newMargin = sectionProperties.RightMargin + Offset;
				int width = (sectionProperties.PageWidth - newMargin - sectionProperties.LeftMargin - sectionProperties.Space * sectionProperties.ColumnCount) / sectionProperties.ColumnCount;
				if (newMargin > 0 && width > DefaultTabWidth)
					sectionProperties.RightMargin = newMargin;
			}
			else {
				int space = sectionProperties.Space + Offset;
				int width = (sectionProperties.PageWidth - sectionProperties.LeftMargin - sectionProperties.RightMargin - space * sectionProperties.ColumnCount) / sectionProperties.ColumnCount;
				if (space > 0 && width > DefaultTabWidth)
					sectionProperties.Space = space;
			}
			return sectionProperties;
		}
	}
	#endregion
	#region ChangeSectionHeightByTopCommand
	public class ChangeSectionHeightByTopCommand : ChangeColumnSizeCommand {
		public ChangeSectionHeightByTopCommand(IRichEditControl control, SectionProperties sectionProperties, int columnIndex, int offset)
			: base(control, sectionProperties, columnIndex, offset) {
		}
		protected internal override SectionProperties GetNewSection(SectionProperties sectionProperties) {
			int newMargin = sectionProperties.TopMargin - Offset;
			if (newMargin > 0 && sectionProperties.BottomMargin + newMargin < sectionProperties.PageHeight)
				sectionProperties.TopMargin = newMargin;
			return sectionProperties;
		}
	}
	#endregion
	#region ChangeSectionHeightByBottomCommand
	public class ChangeSectionHeightByBottomCommand : ChangeColumnSizeCommand {
		public ChangeSectionHeightByBottomCommand(IRichEditControl control, SectionProperties sectionProperties, int columnIndex, int offset)
			: base(control, sectionProperties, columnIndex, offset) {
		}
		protected internal override SectionProperties GetNewSection(SectionProperties sectionProperties) {
			int newMargin = sectionProperties.BottomMargin + Offset;
			if (newMargin > 0 && sectionProperties.TopMargin + newMargin < sectionProperties.PageHeight)
				sectionProperties.BottomMargin = newMargin;
			return sectionProperties;
		}
	}
	#endregion
}
