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
using System.Text;
using DevExpress.Utils;
namespace DevExpress.Office.Utils {
	#region ReplacementItem
	public class ReplacementItem {
		readonly int charIndex;
		readonly string replaceWith;
		public ReplacementItem(int charIndex, string replaceWith) {
			this.charIndex = charIndex;
			this.replaceWith = replaceWith;
		}
		public int CharIndex { get { return charIndex; } }
		public string ReplaceWith { get { return replaceWith; } }
	}
	#endregion
	#region ReplacementInfo
	public class ReplacementInfo {
		readonly List<ReplacementItem> items = new List<ReplacementItem>();
		int deltaLength;
		public void Add(int charIndex, string replaceWith) {
			items.Add(new ReplacementItem(charIndex, replaceWith));
			deltaLength += replaceWith.Length - 1; 
		}
		public int DeltaLength { get { return deltaLength; } }
		public IList<ReplacementItem> Items { get { return items; } }
	}
	#endregion
	#region FastCharacterMultiReplacement
	public class FastCharacterMultiReplacement {
		readonly StringBuilder buffer;
		public FastCharacterMultiReplacement(StringBuilder stringBuilder) {
			Guard.ArgumentNotNull(stringBuilder, "stringBuilder");
			this.buffer = stringBuilder;
		}
		public ReplacementInfo CreateReplacementInfo(string text, Dictionary<char, string> replaceTable) {
			ReplacementInfo result = null;
			for (int i = text.Length - 1; i >= 0; i--) {
				string replaceWith;
				if (replaceTable.TryGetValue(text[i], out replaceWith)) {
					if (result == null)
						result = new ReplacementInfo();
					result.Add(i, replaceWith);
				}
			}
			return result;
		}
		public string PerformReplacements(string text, ReplacementInfo replacementInfo) {
			if (replacementInfo == null)
				return text;
			buffer.Capacity = Math.Max(buffer.Capacity, text.Length + replacementInfo.DeltaLength);
			buffer.Append(text);
			IList<ReplacementItem> replacementItems = replacementInfo.Items;
			int count = replacementItems.Count;
			for (int i = 0; i < count; i++) {
				ReplacementItem item = replacementItems[i];
				buffer.Remove(item.CharIndex, 1);
				string replaceString = item.ReplaceWith;
				if (!String.IsNullOrEmpty(replaceString))
					buffer.Insert(item.CharIndex, replaceString);
			}
			string result = buffer.ToString();
			buffer.Length = 0;
			return result;
		}
		public string PerformReplacements(string text, Dictionary<char, string> replaceTable) {
			return PerformReplacements(text, CreateReplacementInfo(text, replaceTable));
		}
	}
	#endregion
}
