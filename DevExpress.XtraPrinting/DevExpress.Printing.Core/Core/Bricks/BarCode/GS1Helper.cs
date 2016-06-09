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
namespace DevExpress.XtraPrinting.BarCode {
	static class GS1Helper {
		internal class AIElement {
			public readonly string id;
			public readonly int length;
			public readonly bool predefined;
			public AIElement(string ai, int length, bool predefined) {
				this.id = ai;
				this.length = length;
				this.predefined = predefined;
			}
		}
		internal struct ElementResult {
			public string AI;
			public string Value;
		}
		internal static List<AIElement> knownAI = new List<AIElement>();
		static GS1Helper() {
			AddKnownAI("00", 18, true);
			AddKnownAI("01", 14, true);
			AddKnownAI("02", 14, true);
			AddKnownAI("10", 20, false);
			AddKnownAI("11", 6, true);
			AddKnownAI("12", 6, true);
			AddKnownAI("13", 6, true);
			AddKnownAI("15", 6, true);
			AddKnownAI("17", 6, true);
			AddKnownAI("20", 2, true);
			AddKnownAI("21", 20, false);
			AddKnownAI("22", 29, false);
			AddKnownAI_X("23");
			AddKnownAI("240", 30, false);
			AddKnownAI("241", 30, false);
			AddKnownAI("242", 6, false);
			AddKnownAI("250", 30, false);
			AddKnownAI("251", 30, false);
			AddKnownAI("253", 30, false);
			AddKnownAI("30", 8, false);
			AddKnownAI_Y("310", 6, true);
			AddKnownAI_Y("311", 6, true);
			AddKnownAI_Y("312", 6, true);
			AddKnownAI_Y("313", 6, true);
			AddKnownAI_Y("314", 6, true);
			AddKnownAI_Y("315", 6, true);
			AddKnownAI_Y("316", 6, true);
			AddKnownAI_Y("320", 6, true);
			AddKnownAI_Y("321", 6, true);
			AddKnownAI_Y("322", 6, true);
			AddKnownAI_Y("323", 6, true);
			AddKnownAI_Y("324", 6, true);
			AddKnownAI_Y("325", 6, true);
			AddKnownAI_Y("326", 6, true);
			AddKnownAI_Y("327", 6, true);
			AddKnownAI_Y("328", 6, true);
			AddKnownAI_Y("329", 6, true);
			AddKnownAI_Y("330", 6, true);
			AddKnownAI_Y("331", 6, true);
			AddKnownAI_Y("332", 6, true);
			AddKnownAI_Y("333", 6, true);
			AddKnownAI_Y("334", 6, true);
			AddKnownAI_Y("335", 6, true);
			AddKnownAI_Y("336", 6, true);
			AddKnownAI_Y("337", 6, true);
			AddKnownAI_Y("340", 6, true);
			AddKnownAI_Y("341", 6, true);
			AddKnownAI_Y("342", 6, true);
			AddKnownAI_Y("343", 6, true);
			AddKnownAI_Y("344", 6, true);
			AddKnownAI_Y("345", 6, true);
			AddKnownAI_Y("346", 6, true);
			AddKnownAI_Y("347", 6, true);
			AddKnownAI_Y("348", 6, true);
			AddKnownAI_Y("349", 6, true);
			AddKnownAI_Y("350", 6, true);
			AddKnownAI_Y("351", 6, true);
			AddKnownAI_Y("352", 6, true);
			AddKnownAI_Y("353", 6, true);
			AddKnownAI_Y("354", 6, true);
			AddKnownAI_Y("355", 6, true);
			AddKnownAI_Y("356", 6, true);
			AddKnownAI_Y("360", 6, true);
			AddKnownAI_Y("361", 6, true);
			AddKnownAI_Y("362", 6, true);
			AddKnownAI_Y("363", 6, true);
			AddKnownAI_Y("364", 6, true);
			AddKnownAI_Y("365", 6, true);
			AddKnownAI_Y("366", 6, true);
			AddKnownAI_Y("367", 6, true);
			AddKnownAI_Y("368", 6, true);
			AddKnownAI_Y("369", 6, true);
			AddKnownAI_Y("369", 6, true);
			AddKnownAI("37", 8, false);
			AddKnownAI_Y("390", 15, false);
			AddKnownAI_Y("391", 15, false);
			AddKnownAI_Y("392", 15, false);
			AddKnownAI_Y("393", 18, false); 
			AddKnownAI("400", 29, false);
			AddKnownAI("401", 30, false);
			AddKnownAI("402", 17, false);
			AddKnownAI("403", 30, false);
			AddKnownAI("410", 13, true);
			AddKnownAI("411", 13, true);
			AddKnownAI("412", 13, true);
			AddKnownAI("413", 13, false);
			AddKnownAI("414", 13, false);
			AddKnownAI("415", 13, false);
			AddKnownAI("420", 9, false);
			AddKnownAI("421", 12, false);
			AddKnownAI("422", 3, true);
			AddKnownAI("423", 15, false);
			AddKnownAI("424", 3, true);
			AddKnownAI("425", 3, true);
			AddKnownAI("426", 3, true);
			AddKnownAI("7001", 13, true);
			AddKnownAI("7002", 30, false);
			AddKnownAI("7003", 10, true);
			AddKnownAI("7030", 30, false);
			AddKnownAI("7031", 30, false);
			AddKnownAI("7032", 30, false);
			AddKnownAI("7033", 30, false);
			AddKnownAI("7034", 30, false);
			AddKnownAI("7035", 30, false);
			AddKnownAI("7036", 30, false);
			AddKnownAI("7037", 30, false);
			AddKnownAI("7038", 30, false);
			AddKnownAI("7039", 30, false);
			AddKnownAI("8001", 14, true);
			AddKnownAI("8002", 20, false);
			AddKnownAI("8003", 30, false);
			AddKnownAI("8004", 30, false);
			AddKnownAI("8005", 6, true);
			AddKnownAI("8006", 18, true);
			AddKnownAI("8007", 30, false);
			AddKnownAI("8008", 12, false);
			AddKnownAI("8018", 18, true);
			AddKnownAI("8020", 25, false);
			AddKnownAI("8100", 6, true);
			AddKnownAI("8101", 10, true);
			AddKnownAI("8102", 2, true);
			AddKnownAI("90", 30, false);
			AddKnownAI("91", 30, false);
			AddKnownAI("92", 30, false);
			AddKnownAI("93", 30, false);
			AddKnownAI("94", 30, false);
			AddKnownAI("95", 30, false);
			AddKnownAI("96", 30, false);
			AddKnownAI("97", 30, false);
			AddKnownAI("98", 30, false);
			AddKnownAI("99", 30, false);
		}
		public static string MakeDisplayText(string text, char fnc1Char, string fnc1Subst, bool decodeText) {
			if(decodeText) {
				StringBuilder sb = new StringBuilder();
				foreach(ElementResult aiElement in GetAIElements(text, fnc1Char, fnc1Subst))
					if(string.IsNullOrEmpty(aiElement.AI))
						sb.Append(aiElement.Value);
					else
						sb.Append(string.Format("({0}){1}", aiElement.AI, aiElement.Value));
				return sb.ToString();
			} else {
				if(!String.IsNullOrEmpty(fnc1Subst))
					text = text.Replace(fnc1Subst, String.Empty);
				return text;
			}
		}
		internal static IEnumerable<ElementResult> GetAIElements(string text, char fnc1Char, string fnc1Subst) {
			if(!String.IsNullOrEmpty(fnc1Subst))
				text = text.Replace(fnc1Subst, new string(fnc1Char, 1));
			int from = 0;
			int count = text.Length;
			do {
				yield return GS1Helper.ConvertAIElement(text, ref from, fnc1Char);
			}
			while(from >= 0 && from < count);
		}
		static void AddKnownAI_X(string prefix) {
			for(int i = 0; i < 9; i++) {
				string aiId = String.Format("{0}{1}", prefix, i);
				int length = i * 2 + 1;
				knownAI.Add(new AIElement(aiId, length, true));
			}
		}
		static void AddKnownAI_Y(string prefix, int length, bool predefined) {
			for (int i = 0; i < 9; i++) {
				string aiId = String.Format("{0}{1}", prefix, i);
				knownAI.Add(new AIElement(aiId, length, predefined));	
			}
		}
		static void AddKnownAI(string prefix, int length, bool predefined) {
			knownAI.Add(new AIElement(prefix, length, predefined));
		}
		static ElementResult ExtractAIElementValue(string text, ref int from, AIElement ai, char fnc1Char) {
			int start = from + ai.id.Length;
			int length;
			if (ai.predefined) {
				int availableLength = text.Length - start;
				if(ai.length > availableLength) 
					length = availableLength;
				else
					length = ai.length;
			}
			else {
				int index = text.IndexOf(fnc1Char, from);
				int availableLength = (index > 0 ? index : text.Length) - start;
				if(ai.length > availableLength) {
					length = availableLength;
					if(index > 0) from++;
				} else
					length = ai.length;
			}
			from += length + ai.id.Length;
			return new ElementResult() { AI = ai.id, Value = text.Substring(start, length)};
		}
		static ElementResult ConvertAIElement(string text, ref int from, char fnc1Char) {
			ElementResult result = new ElementResult();
			int count = text.Length;
			for(int i = from; i < count; i++, from++)
				if(text[i] != fnc1Char)
					break;
			text = text.Substring(from);
			count = knownAI.Count;
			for(int i = 0; i < count; i++) {
				AIElement ai = knownAI[i];
				if(text.StartsWith(ai.id)) {
					int substringFrom = 0;
					result = ExtractAIElementValue(text, ref substringFrom, ai, fnc1Char);
					from += substringFrom;
					break;
				}
			}
			if(String.IsNullOrEmpty(result.AI)) {
				int index = text.IndexOf(fnc1Char);
				if(index <= 0) {
					result.Value = text;
					from = index;
				} else {
					result.Value = text.Substring(0, index);
					from += index + 1;
				}
			}
			result.Value = result.Value.Replace(new string(fnc1Char, 1), String.Empty);
			return result;
		}
#if DEBUGTEST
		internal static string Test_ExtractAIElementValue(string text, ref int from, AIElement ai) {
			ElementResult aiElement = ExtractAIElementValue(text, ref from, ai, EAN128Generator.fnc1Char[0]);
			return new StringBuilder().AppendFormat("({0}){1}", aiElement.AI, aiElement.Value).ToString();
		}
#endif
	}
}
