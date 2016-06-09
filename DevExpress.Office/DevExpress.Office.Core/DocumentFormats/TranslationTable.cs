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
using System.Collections.ObjectModel;
using System.Globalization;
namespace DevExpress.Office {
	#region TranslationTable
	public class TranslationTable<T> where T : struct {
		Collection<TranslationTableEntry<T>> innerCollection = new Collection<TranslationTableEntry<T>>();
		public Collection<TranslationTableEntry<T>> InnerCollection { get { return innerCollection; } }
		public void Add(T key, string value) {
			TranslationTableEntry<T> entry = new TranslationTableEntry<T>(key, value);
			innerCollection.Add(entry);
		}
		public T GetEnumValue(string str, T defaultValue, bool caseSensitive) {
			if (String.IsNullOrEmpty(str))
				return defaultValue;
			if (!caseSensitive)
				str = str.ToLowerInvariant();
			foreach (TranslationTableEntry<T> entry in innerCollection) {
				if (str == entry.Value)
					return entry.Key;
			}
			return defaultValue;
		}
		public T GetEnumValue(string str, T defaultValue) {
			return GetEnumValue(str, defaultValue, false);
		}
		public string GetStringValue(T key, T defaultKey) {
			string defaultValue = String.Empty;
			foreach (TranslationTableEntry<T> entry in innerCollection) {
				if (key.Equals(entry.Key))
					return entry.Value;
				if (String.IsNullOrEmpty(defaultValue) && defaultKey.Equals(entry.Key))
					defaultValue = entry.Value;
			}
			return defaultValue;
		}
	}
#endregion
#region TranslationTableEntry
	public class TranslationTableEntry<T> where T : struct {
		T key;
		string value;
		public TranslationTableEntry(T key, string value) {
			this.key = key;
			this.value = value;
		}
		public T Key { get { return key; } }
		public string Value { get { return value; } }
	}
#endregion
}
