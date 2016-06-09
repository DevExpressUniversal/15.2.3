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
using System.Linq;
namespace DevExpress.XtraVerticalGrid.Internal {
	public class UniqueNameStore {
		internal static string idEmptyString = Guid.NewGuid().ToString();
		Dictionary<string, SortedList<int, string>> names = new Dictionary<string, SortedList<int, string>>();
		internal Dictionary<string, SortedList<int, string>> NameGroups { get { return names; } }
		int maxLength;
		internal int MaxLength { get { return maxLength; } }
		public UniqueNameStore() {
			this.maxLength = int.MaxValue.ToString().Length - 1;
		}
		public string AddName(string supposedName) {
			string keyName = GetKeyName(supposedName);
			SortedList<int, string> uniqueNames = GetUniqueNames(keyName);
			int index = GetUniqueIndex(uniqueNames, keyName, supposedName);
			if(keyName + index.ToString() == supposedName) return uniqueNames[index] = supposedName;
			return uniqueNames[index] = index == 0 ? keyName : keyName + index.ToString();
		}
		public void RemoveName(string name) {
			string keyName = GetKeyName(name);
			SortedList<int, string> uniqueNames = GetUniqueNames(keyName);
			if(uniqueNames == null) return;
			int index = GetIndexFromName(keyName, name);
			if(index < 0) return;
			uniqueNames[index] = idEmptyString;
		}
		SortedList<int, string> GetUniqueNames(string keyName) {
			SortedList<int, string> uniqueNames;
			if(!NameGroups.TryGetValue(keyName, out uniqueNames)) {
				uniqueNames = new SortedList<int, string>();
				NameGroups.Add(keyName, uniqueNames);
			}
			return uniqueNames;
		}
		int GetUniqueIndex(SortedList<int, string> uniqueNames, string keyName, string supposedName) {
			int index = GetIndexFromName(keyName, supposedName);
			if(!uniqueNames.Keys.Contains(index) || uniqueNames[index] == idEmptyString) return index;
			int emptyIndex = uniqueNames.IndexOfValue(idEmptyString);
			if(emptyIndex < 0)
				return GetFirstEmptyIndex(uniqueNames, 0);
			return emptyIndex;
		}
		int GetFirstEmptyIndex(SortedList<int, string> uniqueNames, int startIndex) {
			int lastIndex = uniqueNames.Keys[uniqueNames.Keys.Count - 1];
			if(lastIndex == uniqueNames.Keys.Count - 1) return uniqueNames.Keys.Count;
			for(int i = 0; i < lastIndex; i++) {
				if(uniqueNames.IndexOfKey(i) < 0) return i;
			}
			return lastIndex + 1;
		}
		int GetIndexFromName(string keyName, string supposedName) {
			if(keyName == supposedName) return 0;
			string index = supposedName.Replace(keyName, string.Empty);
			int result;
			if(int.TryParse(index, out result)) return result;
			return -1;
		}
		static Char[] trimChars = new Char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
		string GetKeyName(string supposedName) {
			string keyName = supposedName.TrimEnd(trimChars);
			if(CanParse(keyName, supposedName)) return keyName;
			return supposedName;
		}
		bool CanParse(string keyName, string supposedName) {
			if(keyName.Length <= 0) return false;
			return (supposedName.Length - keyName.Length <= MaxLength);
		}
	}
}
