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

using DevExpress.Utils;
using System;
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System;
namespace DevExpress.Office.NumberConverters {
	#region LanguageId
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct LanguageId : IConvertToInt<LanguageId> {
		public static readonly LanguageId English = new LanguageId(1033);
		public static readonly LanguageId French = new LanguageId(1036);
		public static readonly LanguageId German = new LanguageId(1031);
		public static readonly LanguageId Italian = new LanguageId(1040);
		public static readonly LanguageId Russian = new LanguageId(1049);
		public static readonly LanguageId Swedish = new LanguageId(1053);
		public static readonly LanguageId Turkish = new LanguageId(1055);
		public static readonly LanguageId Greek = new LanguageId(1057);
		public static readonly LanguageId Spanish = new LanguageId(1059);
		public static readonly LanguageId Portuguese = new LanguageId(1061);
		public static readonly LanguageId Ukrainian = new LanguageId(1063);
		public static readonly LanguageId Hindi = new LanguageId(1065);
		readonly int m_value;
		internal LanguageId(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is LanguageId) && (this.m_value == ((LanguageId)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static bool operator ==(LanguageId id1, LanguageId id2) {
			return id1.m_value == id2.m_value;
		}
		public static bool operator !=(LanguageId id1, LanguageId id2) {
			return id1.m_value != id2.m_value;
		}
		#region IConvertToInt<LanguageId> Members
		int IConvertToInt<LanguageId>.ToInt() {
			return m_value;
		}
		LanguageId IConvertToInt<LanguageId>.FromInt(int value) {
			return new LanguageId(value);
		}
		#endregion
	}
	#endregion
}
