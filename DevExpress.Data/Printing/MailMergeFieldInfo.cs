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
using System.Collections;
namespace DevExpress.Data {
	#region MailMergeFieldInfo
	public class MailMergeFieldInfo {
		#region static
		public static string MakeFormatString(string str) {
			return string.Format("{{0:{0}}}", str);
		}
		public static string WrapColumnInfoInBrackets(string columnName, string formatString) {
			string str = IsNullOrEmpty(formatString) ? columnName : string.Format("{0}{1}{2}", columnName, MailMergeFieldInfo.FormatStringDelimiter, formatString);
			return string.Format("{0}{1}{2}", OpeningBracket, str, ClosingBracket);
		}
		static bool IsNullOrEmpty(string str) {
			return str == null || str.Length == 0;
		}
		#endregion
		public const char FormatStringDelimiter = '!';
		public const char OpeningBracket = '[';
		public const char ClosingBracket = ']';
		#region Fields
		int startPosition;
		int endPosition;
		string fieldName = String.Empty;
		string displayName = String.Empty;
		string rawFormatString = String.Empty;
		string dataMember = String.Empty;
		#endregion
		#region Properties
		public int StartPosition { get { return startPosition; } set { startPosition = value; } }
		public int EndPosition { get { return endPosition; } set { endPosition = value; } }
		public string FieldName {
			get { return fieldName; }
			set {
				if(fieldName == value)
					return;
				fieldName = value;
				int formatStringDelimiterIndex = fieldName.IndexOf(FormatStringDelimiter);
				displayName = formatStringDelimiterIndex >= 0 ? fieldName.Substring(0, formatStringDelimiterIndex) : fieldName;
				rawFormatString = formatStringDelimiterIndex >= 0 ? fieldName.Substring(formatStringDelimiterIndex + 1, fieldName.Length - formatStringDelimiterIndex - 1) : string.Empty;
			}
		}
		public string TrimmedFieldName {
			get {
				int formatStringDelimiterPosition = fieldName.IndexOf(FormatStringDelimiter);
				return formatStringDelimiterPosition < 0 ? fieldName : fieldName.Substring(0, formatStringDelimiterPosition);
			}
		}
		public string DisplayName { get { return displayName; } set { displayName = value; } }
		public string FormatString { get { return MakeFormatString(rawFormatString); } }
		public string DataMember { get { return dataMember; } set { dataMember = value; } }
		public bool HasFormatStringInfo {
			get { return !IsNullOrEmpty(rawFormatString); }
		}
		#endregion
		public MailMergeFieldInfo() {
		}
		public MailMergeFieldInfo(MailMergeFieldInfo mailMergeFieldInfo) {
			this.displayName = mailMergeFieldInfo.displayName;
			this.dataMember = mailMergeFieldInfo.dataMember;
			this.rawFormatString = mailMergeFieldInfo.rawFormatString;
		}
		public override string ToString() {
			return WrapColumnInfoInBrackets(DisplayName, rawFormatString);
		}
		public override int GetHashCode() {
			return DevExpress.Utils.HashCodeHelper.CalcHashCode(DisplayName.GetHashCode(), DataMember.GetHashCode(), FormatString.GetHashCode(), StartPosition.GetHashCode());
		}
		public override bool Equals(object obj) {
			if(!(obj is MailMergeFieldInfo))
				return false;
			else {
				MailMergeFieldInfo other = obj as MailMergeFieldInfo;
				return other.DisplayName == DisplayName 
					&& other.DataMember == DataMember 
					&& other.FormatString == FormatString
					&& other.StartPosition == StartPosition;
			}
		}
	}
	public class MailMergeFieldInfoValue : MailMergeFieldInfo {
		public MailMergeFieldInfoValue(MailMergeFieldInfo mailMergeFieldInfoValue)
			: base(mailMergeFieldInfoValue) {
		}
		public override int GetHashCode() {
			return DevExpress.Utils.HashCodeHelper.CalcHashCode(DisplayName.GetHashCode(), DataMember.GetHashCode(), FormatString.GetHashCode());
		}
		public override bool Equals(object obj) {
			if(!(obj is MailMergeFieldInfoValue))
				return false;
			else {
				MailMergeFieldInfoValue other = obj as MailMergeFieldInfoValue;
				return other.DisplayName == DisplayName
					&& other.DataMember == DataMember
					&& other.FormatString == FormatString;
			}
		}
	}
	#endregion
#if !SL
	#region MailMergeFieldInfoCollection
	public class MailMergeFieldInfoCollection : CollectionBase {
		public MailMergeFieldInfo this[int index] { get { return (MailMergeFieldInfo)List[index]; } }
		#region ICollection strong typed implementation
		public void CopyTo(MailMergeFieldInfo[] array, int index) {
			((ICollection)this).CopyTo(array, index);
		}
		#endregion
		#region IList strong typed implementation
		public int Add(MailMergeFieldInfo item) {
			int index = IndexOf(item);
			return index >= 0 ? index : List.Add(item);
		}
		public int IndexOf(MailMergeFieldInfo item) {
			return List.IndexOf(item);
		}
		public bool Contains(MailMergeFieldInfo item) {
			return List.Contains(item);
		}
		public void Remove(MailMergeFieldInfo item) {
			int index = IndexOf(item);
			if(index >= 0)
				List.RemoveAt(index);
		}
		public void Insert(int index, MailMergeFieldInfo item) {
			List.Insert(index, item);
		}
		#endregion
	}
	#endregion
#endif
}
