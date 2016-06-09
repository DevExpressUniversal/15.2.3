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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraRichEdit.Model {
	public class SectionFooter : SectionHeaderFooterBase {
		public SectionFooter(DocumentModel documentModel, HeaderFooterType type)
			: base(documentModel, type) {
		}
		public override bool IsFooter { get { return true; } }		
		protected internal override SectionHeadersFootersBase GetContainer(Section section) {
			return section.Footers;
		}
		protected internal override XtraRichEditStringId GetCaptionStringId() {
			if (Type == HeaderFooterType.First)
				return XtraRichEditStringId.Caption_FirstPageFooter;
			if (DocumentModel.DocumentProperties.DifferentOddAndEvenPages) {
				if (Type == HeaderFooterType.Odd)
					return XtraRichEditStringId.Caption_OddPageFooter;
				else
					return XtraRichEditStringId.Caption_EvenPageFooter;
			}
			else
				return XtraRichEditStringId.Caption_PageFooter;
		}
	}
	#region FooterCollection
	public class FooterCollection : HeaderFooterCollectionBase<SectionFooter, FooterIndex> {
	}
	#endregion
	#region FooterIndex
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct FooterIndex : IConvertToInt<FooterIndex>, IComparable<FooterIndex> {
		readonly int m_value;
		public static FooterIndex MinValue = new FooterIndex(0);
		public static readonly FooterIndex Zero = new FooterIndex(0);
		public static readonly FooterIndex Invalid = new FooterIndex(-1);
		public static FooterIndex MaxValue = new FooterIndex(int.MaxValue);
		[DebuggerStepThrough]
		public FooterIndex(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is FooterIndex) && (this.m_value == ((FooterIndex)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static FooterIndex operator +(FooterIndex index, int value) {
			return new FooterIndex(index.m_value + value);
		}
		public static int operator -(FooterIndex index1, FooterIndex index2) {
			return index1.m_value - index2.m_value;
		}
		public static FooterIndex operator -(FooterIndex index, int value) {
			return new FooterIndex(index.m_value - value);
		}
		public static FooterIndex operator ++(FooterIndex index) {
			return new FooterIndex(index.m_value + 1);
		}
		public static FooterIndex operator --(FooterIndex index) {
			return new FooterIndex(index.m_value - 1);
		}
		public static bool operator <(FooterIndex index1, FooterIndex index2) {
			return index1.m_value < index2.m_value;
		}
		public static bool operator <=(FooterIndex index1, FooterIndex index2) {
			return index1.m_value <= index2.m_value;
		}
		public static bool operator >(FooterIndex index1, FooterIndex index2) {
			return index1.m_value > index2.m_value;
		}
		public static bool operator >=(FooterIndex index1, FooterIndex index2) {
			return index1.m_value >= index2.m_value;
		}
		public static bool operator ==(FooterIndex index1, FooterIndex index2) {
			return index1.m_value == index2.m_value;
		}
		public static bool operator !=(FooterIndex index1, FooterIndex index2) {
			return index1.m_value != index2.m_value;
		}
		#region IConvertToInt<FooterIndex> Members
		[DebuggerStepThrough]
		int IConvertToInt<FooterIndex>.ToInt() {
			return m_value;
		}
		[DebuggerStepThrough]
		FooterIndex IConvertToInt<FooterIndex>.FromInt(int value) {
			return new FooterIndex(value);
		}
		#endregion
		#region IComparable<FooterIndex> Members
		public int CompareTo(FooterIndex other) {
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
