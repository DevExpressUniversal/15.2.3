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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Model.History {
	#region SectionHeaderFooterIndexChangedHistoryItem<T> (abstract class)
	public abstract class SectionHeaderFooterIndexChangedHistoryItem<T> : RichEditHistoryItem where T : struct {
		SectionIndex sectionIndex;
		HeaderFooterType type;
		T previousIndex;
		T nextIndex;
		protected SectionHeaderFooterIndexChangedHistoryItem(Section section)
			: base(GetMainPieceTable(section)) {
			this.sectionIndex = DocumentModel.Sections.IndexOf(section);
		}
		static PieceTable GetMainPieceTable(Section section) {
			Guard.ArgumentNotNull(section, "section");
			return section.DocumentModel.MainPieceTable;
		}
		public HeaderFooterType Type { get { return type; } set { type = value; } }
		public T PreviousIndex { get { return previousIndex; } set { previousIndex = value; } }
		public T NextIndex { get { return nextIndex; } set { nextIndex = value; } }
		protected override void RedoCore() {
			ReplaceHeaderFooter(NextIndex);
		}
		protected override void UndoCore() {
			ReplaceHeaderFooter(PreviousIndex);
		}
		protected internal virtual void ReplaceHeaderFooter(T index) {
			Section section = DocumentModel.Sections[sectionIndex];
			SetCurrentHeaderFooterIndex(section, index);
			ApplyChanges(section);
		}
		protected internal virtual void ApplyChanges(Section section) {
			ObtainAffectedRangeEventArgs args = new ObtainAffectedRangeEventArgs();
			section.OnObtainAffectedRange(this, args);
			if (args.Start >= RunIndex.Zero)
				PieceTable.ApplyChangesCore(SectionMarginsChangeActionsCalculator.CalculateChangeActions(SectionMarginsChangeType.Top), args.Start, args.End);
		}
		protected internal abstract void SetCurrentHeaderFooterIndex(Section section, T index);
	}
	#endregion
	#region SectionPageHeaderIndexChangedHistoryItem
	public class SectionPageHeaderIndexChangedHistoryItem : SectionHeaderFooterIndexChangedHistoryItem<HeaderIndex> {
		public SectionPageHeaderIndexChangedHistoryItem(Section section)
			: base(section) {
		}
		protected internal override void SetCurrentHeaderFooterIndex(Section section, HeaderIndex index) {
			section.Headers.SetObjectIndex(Type, index);
		}
	}
	#endregion
	#region SectionPageFooterIndexChangedHistoryItem
	public class SectionPageFooterIndexChangedHistoryItem : SectionHeaderFooterIndexChangedHistoryItem<FooterIndex> {
		public SectionPageFooterIndexChangedHistoryItem(Section section)
			: base(section) {
		}
		protected internal override void SetCurrentHeaderFooterIndex(Section section, FooterIndex index) {
			section.Footers.SetObjectIndex(Type, index);
		}
	}
	#endregion
}
