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

using System.ComponentModel;
using System;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraRichEdit {
#region DocumentFormat
#if !DXPORTABLE
	[TypeConverter(typeof(DevExpress.XtraRichEdit.Design.DocumentFormatTypeConverter))]
#endif
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1413")]
	public struct DocumentFormat : IConvertToInt<DocumentFormat> {
		public static readonly DocumentFormat Undefined = new DocumentFormat(0);
		public static readonly DocumentFormat PlainText = new DocumentFormat(1);
		public static readonly DocumentFormat Rtf = new DocumentFormat(2);
		public static readonly DocumentFormat Html = new DocumentFormat(3);
		public static readonly DocumentFormat OpenXml = new DocumentFormat(4);
		public static readonly DocumentFormat Mht = new DocumentFormat(5);
		public static readonly DocumentFormat WordML = new DocumentFormat(6);
		public static readonly DocumentFormat OpenDocument = new DocumentFormat(7);
		internal static readonly DocumentFormat Xaml = new DocumentFormat(8);
		public static readonly DocumentFormat ePub = new DocumentFormat(9);
		public static readonly DocumentFormat Doc = new DocumentFormat(10);
		internal static readonly DocumentFormat Last = new DocumentFormat(10);
		int m_value;
		public DocumentFormat(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is DocumentFormat) && (this.m_value == ((DocumentFormat)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static bool operator ==(DocumentFormat id1, DocumentFormat id2) {
			return id1.m_value == id2.m_value;
		}
		public static bool operator !=(DocumentFormat id1, DocumentFormat id2) {
			return id1.m_value != id2.m_value;
		}
#region IConvertToInt<DocumentFormat> Members
		int IConvertToInt<DocumentFormat>.ToInt() {
			return m_value;
		}
		DocumentFormat IConvertToInt<DocumentFormat>.FromInt(int value) {
			return new DocumentFormat(value);
		}
#endregion
	}
#endregion
}
