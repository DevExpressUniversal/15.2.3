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
using System.Diagnostics;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraRichEdit.Model {
	public abstract class SectionHeaderFooterBase : ContentTypeBase {
		HeaderFooterType type;
		protected SectionHeaderFooterBase(DocumentModel documentModel, HeaderFooterType type) : base(documentModel) {
			this.type = type;
		}
		public HeaderFooterType Type { get { return type; } }
		public override bool IsMain { get { return false; } }
		public override bool IsHeaderFooter { get { return true; } }
		#region Events
		#region RequestSectionIndex
		RequestSectionIndexEventHandler onRequestSectionIndex;
		public event RequestSectionIndexEventHandler RequestSectionIndex { add { onRequestSectionIndex += value; } remove { onRequestSectionIndex -= value; } }
		protected internal virtual SectionIndex RaiseRequestSectionIndex() {
			if (onRequestSectionIndex != null) {
				RequestSectionIndexEventArgs args = new RequestSectionIndexEventArgs();
				onRequestSectionIndex(this, args);
				if (args.SectionIndex == new SectionIndex(int.MaxValue))
					args.SectionIndex = new SectionIndex(-1);
				return args.SectionIndex;
			}
			else
				return new SectionIndex(-1);
		}
		#endregion
		#endregion
		protected internal virtual SectionIndex GetSectionIndex() {
			if (DocumentModel.IsUpdateLocked && DocumentModel.DeferredChanges.IsSetContentMode)
				return new SectionIndex(0);
			SectionIndex index = RaiseRequestSectionIndex();
			if (index < new SectionIndex(0))
				return new SectionIndex(0); 
			return index;
		}
		protected internal virtual Section GetSection() {
			return DocumentModel.Sections[GetSectionIndex()];
		}
		protected internal virtual RunIndex CalculateMainPieceTableStartRunIndex(RunIndex runIndex) {
			if (runIndex == RunIndex.DontCare)
				return runIndex;
			Section actualSection = GetSection();
			while (GetContainer(actualSection).IsLinkedToPrevious(Type))
				actualSection = actualSection.GetPreviousSection();
			PieceTable pieceTable = DocumentModel.MainPieceTable;
			return pieceTable.Paragraphs[actualSection.FirstParagraphIndex].FirstRunIndex;
		}
		protected internal virtual RunIndex CalculateMainPieceTableEndRunIndex(RunIndex runIndex) {
			if (runIndex == RunIndex.DontCare)
				return runIndex;
			Section actualSection = GetSection();
			for (; ; ) {
				Section nextSection = actualSection.GetNextSection();
				if (nextSection == null)
					break;
				if (GetContainer(nextSection).IsLinkedToPrevious(Type))
					actualSection = nextSection;
				else
					break;
			}
			PieceTable pieceTable = DocumentModel.MainPieceTable;
			return pieceTable.Paragraphs[actualSection.LastParagraphIndex].LastRunIndex;
		}
		protected internal virtual void OverrideType(HeaderFooterType type) {
			this.type = type;
		}
		protected internal virtual string GetCaption() {
			XtraRichEditStringId id = GetCaptionStringId();
			if (id != (XtraRichEditStringId)(-1))
				return XtraRichEditLocalizer.GetString(id);
			else
				return String.Empty;
		}
		protected internal abstract XtraRichEditStringId GetCaptionStringId();
		protected internal abstract SectionHeadersFootersBase GetContainer(Section section);
		protected internal override SectionIndex LookupSectionIndexByParagraphIndex(ParagraphIndex paragraphIndex) {
			return base.LookupSectionIndexByParagraphIndex(GetSection().FirstParagraphIndex);
		}
		protected internal override void ApplyChanges(DocumentModelChangeType changeType, RunIndex startRunIndex, RunIndex endRunIndex) {
			base.ApplyChanges(changeType, CalculateMainPieceTableStartRunIndex(startRunIndex), CalculateMainPieceTableEndRunIndex(endRunIndex));
		}
		protected internal override void ApplyChangesCore(DocumentModelChangeActions actions, RunIndex startRunIndex, RunIndex endRunIndex) {
			if ((actions & DocumentModelChangeActions.SplitRunByCharset) != 0) { 
				base.ApplyChangesCore(DocumentModelChangeActions.SplitRunByCharset, startRunIndex, endRunIndex);
				actions &= ~DocumentModelChangeActions.SplitRunByCharset;
			}
			base.ApplyChangesCore(actions, CalculateMainPieceTableStartRunIndex(startRunIndex), CalculateMainPieceTableEndRunIndex(endRunIndex));
		}
		protected internal override void ApplySectionFormatting(DocumentLogPosition logPositionStart, int length, SectionPropertyModifierBase modifier) {
			DocumentModel.ApplySectionFormatting(DocumentModel.MainPieceTable.Paragraphs[GetSection().FirstParagraphIndex].LogPosition, 1, modifier);
		}
		protected internal override Nullable<T> ObtainSectionsPropertyValue<T>(DocumentLogPosition logPositionStart, int length, SectionPropertyModifier<T> modifier) {
			return DocumentModel.ObtainSectionsPropertyValue<T>(DocumentModel.MainPieceTable.Paragraphs[GetSection().FirstParagraphIndex].LogPosition, 1, modifier);
		}
		protected internal override void FixLastParagraphOfLastSection(int originalParagraphCount) {
		}
		protected internal override SpellChecker.SpellCheckerManager CreateSpellCheckerManager(PieceTable pieceTable) {
			return pieceTable.DocumentModel.MainPieceTable.SpellCheckerManager.CreateInstance(pieceTable);
		}
	}
	#region HeaderFooterCollectionBase (abstract class)
	public abstract class HeaderFooterCollectionBase<T, U> : List<T, U>
		where T : SectionHeaderFooterBase
		where U : struct, IConvertToInt<U> {
		public override void Clear() {
			int count = Count;
			for (int i = 0; i < count; i++)
				InnerList[i].PieceTable.Clear();
			base.Clear();
		}
	}
	#endregion
	public delegate void RequestSectionIndexEventHandler(object sender, RequestSectionIndexEventArgs args);
	public class RequestSectionIndexEventArgs : EventArgs {
		SectionIndex sectionIndex = new SectionIndex(int.MaxValue);
		public SectionIndex SectionIndex { get { return sectionIndex; } set { sectionIndex = value; } }
	}
	public class SectionHeader : SectionHeaderFooterBase {
		public SectionHeader(DocumentModel documentModel, HeaderFooterType type) : base(documentModel, type) {
		}
		public override bool IsHeader { get { return true; } }
		protected internal override SectionHeadersFootersBase GetContainer(Section section) {
			return section.Headers;
		}		
		protected internal override XtraRichEditStringId GetCaptionStringId() {
			if (Type == HeaderFooterType.First)
				return XtraRichEditStringId.Caption_FirstPageHeader;
			if (DocumentModel.DocumentProperties.DifferentOddAndEvenPages) {
				if (Type == HeaderFooterType.Odd)
					return XtraRichEditStringId.Caption_OddPageHeader;
				else
					return XtraRichEditStringId.Caption_EvenPageHeader;
			}
			else
				return XtraRichEditStringId.Caption_PageHeader;
		}
	}
	#region HeaderCollection
	public class HeaderCollection : HeaderFooterCollectionBase<SectionHeader, HeaderIndex> {
	}
	#endregion
	#region HeaderIndex
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct HeaderIndex : IConvertToInt<HeaderIndex>, IComparable<HeaderIndex> {
		readonly int m_value;
		public static HeaderIndex MinValue = new HeaderIndex(0);
		public static readonly HeaderIndex Zero = new HeaderIndex(0);
		public static readonly HeaderIndex Invalid = new HeaderIndex(-1);
		public static HeaderIndex MaxValue = new HeaderIndex(int.MaxValue);
		[DebuggerStepThrough]
		public HeaderIndex(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is HeaderIndex) && (this.m_value == ((HeaderIndex)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static HeaderIndex operator +(HeaderIndex index, int value) {
			return new HeaderIndex(index.m_value + value);
		}
		public static int operator -(HeaderIndex index1, HeaderIndex index2) {
			return index1.m_value - index2.m_value;
		}
		public static HeaderIndex operator -(HeaderIndex index, int value) {
			return new HeaderIndex(index.m_value - value);
		}
		public static HeaderIndex operator ++(HeaderIndex index) {
			return new HeaderIndex(index.m_value + 1);
		}
		public static HeaderIndex operator --(HeaderIndex index) {
			return new HeaderIndex(index.m_value - 1);
		}
		public static bool operator <(HeaderIndex index1, HeaderIndex index2) {
			return index1.m_value < index2.m_value;
		}
		public static bool operator <=(HeaderIndex index1, HeaderIndex index2) {
			return index1.m_value <= index2.m_value;
		}
		public static bool operator >(HeaderIndex index1, HeaderIndex index2) {
			return index1.m_value > index2.m_value;
		}
		public static bool operator >=(HeaderIndex index1, HeaderIndex index2) {
			return index1.m_value >= index2.m_value;
		}
		public static bool operator ==(HeaderIndex index1, HeaderIndex index2) {
			return index1.m_value == index2.m_value;
		}
		public static bool operator !=(HeaderIndex index1, HeaderIndex index2) {
			return index1.m_value != index2.m_value;
		}
		#region IConvertToInt<HeaderIndex> Members
		[DebuggerStepThrough]
		int IConvertToInt<HeaderIndex>.ToInt() {
			return m_value;
		}
		[DebuggerStepThrough]
		HeaderIndex IConvertToInt<HeaderIndex>.FromInt(int value) {
			return new HeaderIndex(value);
		}
		#endregion
		#region IComparable<HeaderIndex> Members
		public int CompareTo(HeaderIndex other) {
			if (m_value < other.m_value)
				return -1;
			if (m_value > other.m_value)
				return 1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
}
