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

using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public interface ICommandState {
		void Load(ArrayList list);
	}
	public class IntervalCommandState : ICommandState {
		public DocumentLogPosition Position { get; private set; }
		public int Length { get; private set; }
		public object Value { get; private set; }
		public virtual void Load(ArrayList list) {
			Position = new DocumentLogPosition((int)list[0]);
			Length = (int)list[1];
			Value = list[2];
		}
	}
	public class IntervalWithUseCommandState : IntervalCommandState {
		public bool UseValue { get; private set; }
		public override void Load(ArrayList list) {
			base.Load(list);
			UseValue = (int)list[3] == 1;
		}
	}
	public class ListLevelCommandState : ICommandState {
		public bool IsAbstract { get; private set; }
		public int ListIndex { get; private set; }
		public int ListLevelIndex { get; private set; }
		public object Value { get; private set; }
		public virtual void Load(ArrayList list) {
			IsAbstract = (int)list[0] == 1;
			ListIndex = (int)list[1];
			ListLevelIndex = (int)list[2];
			Value = list[3];
		}
	}
	public class ListLevelWithUseCommandState : ListLevelCommandState {
		public bool UseValue { get; private set; }
		public override void Load(ArrayList list) {
			base.Load(list);
			UseValue = (int)list[4] == 1;
		}
	}
	public class SectionCommandState : ICommandState {
		public SectionIndex SectionIndex { get; private set; }
		public object Value { get; private set; }
		public void Load(ArrayList list) {
			SectionIndex = new SectionIndex(Convert.ToInt32(list[0]));
			Value = list[1];
		}
	}
	public class DeleteBookmarkCommandState : ICommandState {
		public string name { get; private set; }
		public virtual void Load(ArrayList list) {
			name = (string)list[0];
		}
	}
	public class CreateBookmarkCommandState : DeleteBookmarkCommandState {
		public int start { get; private set; }
		public int end { get; private set; }
		public override void Load(ArrayList list) {
			base.Load(list);
			start = (int)list[1];
			end = (int)list[2];
		}
	}
	public class TableState : ICommandState {
		public DocumentLogPosition TablePosition { get; private set; }
		public int TableNestedLevel { get; private set; }
		public object Value { get; private set; }
		public virtual void Load(ArrayList list) {
			TablePosition = new DocumentLogPosition(Convert.ToInt32(list[0]));
			TableNestedLevel = (int)list[1];
			Value = list[2];
		}
	}
	public class TablePropertyState : TableState {
		public bool UseValue { get; private set; }
		public bool[] ComplexUseValues { get; private set; }
		public override void Load(ArrayList list) {
			base.Load(list);
			if (list[3] is ArrayList)
				ComplexUseValues = ((ArrayList)list[3]).Cast<int>().Select(i => i == 1).ToArray();
			else
				UseValue = (int)list[3] == 1;
		}
	}
	public class TableRowState : ICommandState {
		public DocumentLogPosition TablePosition { get; private set; }
		public int TableNestedLevel { get; private set; }
		public int RowIndex { get; private set; }
		public object Value { get; private set; }
		public virtual void Load(ArrayList list) {
			TablePosition = new DocumentLogPosition(Convert.ToInt32(list[0]));
			TableNestedLevel = (int)list[1];
			RowIndex = (int)list[2];
			Value = list[3];
		}
	}
	public class TableRowPropertyState : TableRowState {
		public bool UseValue { get; private set; }
		public override void Load(ArrayList list) {
			base.Load(list);
			UseValue = (int)list[4] == 1;
		}
	}
	public class TableCellState : ICommandState {
		public DocumentLogPosition TablePosition { get; private set; }
		public int TableNestedLevel { get; private set; }
		public int RowIndex { get; private set; }
		public int CellIndex { get; private set; }
		public object Value { get; private set; }
		public virtual void Load(ArrayList list) {
			TablePosition = new DocumentLogPosition(Convert.ToInt32(list[0]));
			TableNestedLevel = (int)list[1];
			RowIndex = (int)list[2];
			CellIndex = (int)list[3];
			Value = list[4];
		}
	}
	public class TableCellPropertyState : TableCellState {
		public bool UseValue { get; private set; }
		public bool[] ComplexUseValues { get; private set; }
		public override void Load(ArrayList list) {
			base.Load(list);
			if (list[5] is ArrayList)
				ComplexUseValues = ((ArrayList)list[5]).Cast<int>().Select(i => i == 1).ToArray();
			else
				UseValue = (int)list[5] == 1;
		}
	}
}
