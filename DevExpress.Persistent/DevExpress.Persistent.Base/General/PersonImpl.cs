#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
namespace DevExpress.Persistent.Base.General {
	public class PersonImpl {
		private string firstName = "";
		private string lastName = "";
		private string middleName = "";
		private DateTime birthday;
		private string email = "";
		private static string fullNameFormat;
		private static string ReplaceIgnoreCase(string str, string oldString, string newString) {
			string result = "";
			int lastNameEntryStartIndex = str.IndexOf(oldString, StringComparison.InvariantCultureIgnoreCase);
			if((lastNameEntryStartIndex >= 0)
				&& (str.IndexOf(newString) < 0)) {
				if(lastNameEntryStartIndex > 0) {
					result = str.Substring(0, lastNameEntryStartIndex);
				}
				result += newString;
				int oldStringLength = oldString.Length;
				if((lastNameEntryStartIndex + oldStringLength) < str.Length) {
					result += str.Substring(lastNameEntryStartIndex + oldStringLength, str.Length - lastNameEntryStartIndex - oldStringLength);
				}
			}
			else {
				result = str;
			}
			return result;
		}
		private static string ConvertIfItIsInOldFormat(string formatStr) {
			string result = ReplaceIgnoreCase(formatStr, "FirstName", "{FirstName}");
			result = ReplaceIgnoreCase(result, "LastName", "{LastName}");
			return ReplaceIgnoreCase(result, "MiddleName", "{MiddleName}");
		}
		public static string FullNameFormat {
			get { return fullNameFormat; }
			set {
				fullNameFormat = value;
				fullNameFormat = ConvertIfItIsInOldFormat(fullNameFormat);
			}
		}
		public void SetFullName(string fullName) {
			FirstName = MiddleName = LastName = "";
			int index = fullName.IndexOf(',');
			if(index > 0) { 
				fullName = fullName.Remove(0, index + 1).Trim() + " " + fullName.Substring(0, index);
			}
			string[] names = fullName.Split(' ');
			FirstName = names[0];
			if(names.Length == 2) {
				LastName = names[1];
			}
			else
				if(names.Length == 3) {
					MiddleName = names[1];
					LastName = names[2];
				}
				else {
					for(int i = 2; i < names.Length; i++) {
						LastName += " " + names[i];
					}
				}
		}
		public string FirstName {
			get { return firstName; }
			set { firstName = value; }
		}
		public string LastName {
			get { return lastName; }
			set { lastName = value; }
		}
		public string MiddleName {
			get { return middleName; }
			set { middleName = value; }
		}
		public DateTime Birthday {
			get { return birthday; }
			set { birthday = value; }
		}
		public string FullName {
			get { return ObjectFormatter.Format(fullNameFormat, this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty); }
		}
		public string Email {
			get { return email; }
			set { email = value; }
		}
	}
}
