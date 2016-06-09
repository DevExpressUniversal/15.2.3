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
using System.Globalization;
namespace DevExpress.Xpf.TreeMap.Native {
	public enum PatternConstants {
		Label,
		Value,
	}
	public interface IPatternHolder {
		PatternDataProvider GetDataProvider(PatternConstants patternConstant);
	}
	public abstract class PatternDataProvider {
		object context = null;
		public object Context {
			get { return context; }
			set {
				if (context != value)
					context = value;
			}
		}
		public abstract bool CheckContext(object value);
		protected abstract object GetValue();
		public string GetText(string format) {
			object value = GetValue();
			if (value != null) {
				if (!string.IsNullOrEmpty(format))
					return PatternUtils.Format(value, format);
				return value.ToString();
			}
			return string.Empty;
		}
	}
	public class ToolTipPatternProvider : PatternDataProvider {
		readonly PatternConstants patternConstant;
		protected ITreeMapItem Item { get { return Context as ITreeMapItem; } }
		public override bool CheckContext(object value) {
			return value is ITreeMapItem;
		}
		public ToolTipPatternProvider(PatternConstants patternConstant) {
			this.patternConstant = patternConstant;
		}
		object GetValueTreeMapItem() {
			object value = null;
			if (Context != null) {
				switch (patternConstant) {
					case PatternConstants.Label:
						value = Item.Label;
						break;
					case PatternConstants.Value:
						value = Item.Weight;
						break;
				}
			}
			return value;
		}
		protected override object GetValue() {
			if (Item != null)
				return GetValueTreeMapItem();
			return string.Empty;
		}
	}
	public class PatternParser {
		class PatternFragment {
			readonly string fragment;
			readonly string format;
			readonly PatternDataProvider dataProvider;
			public string Fragment { get { return fragment; } }
			public PatternFragment(string fragment, string format, PatternDataProvider dataProvider) {
				this.fragment = fragment;
				this.format = format;
				this.dataProvider = dataProvider;
			}
			public string GetText() {
				return dataProvider.GetText(format);
			}
			public void SetContext(object context) {
				if (dataProvider.CheckContext(context))
					dataProvider.Context = context;
			}
		}
		static Dictionary<string, PatternConstants> constants;
		static PatternParser() {
			constants = new Dictionary<string, PatternConstants>();
			constants.Add("L", PatternConstants.Label);
			constants.Add("V", PatternConstants.Value);
		}
		readonly string template;
		readonly IPatternHolder patternHolder;
		readonly List<PatternFragment> fragments = new List<PatternFragment>();
		public PatternParser(string pattern, IPatternHolder patternHolder) {
			this.template = pattern;
			this.patternHolder = patternHolder;
			Parse();
		}
		void Parse() {
			List<string> parsedPattern = SplitString(template, '{', '}');
			if (parsedPattern != null)
				foreach (string fragment in parsedPattern) {
					string format;
					PatternConstants? patternConstant;
					if (PrepareFragment(fragment, out patternConstant, out format)) {
						PatternDataProvider dataProvider = patternHolder.GetDataProvider(patternConstant.Value);
						if (dataProvider != null)
							fragments.Add(new PatternFragment(fragment, format, dataProvider));
					}
				}
		}
		bool PrepareFragment(string fragment, out PatternConstants? patternConstant, out string format) {
			patternConstant = null;
			format = string.Empty;
			if (!(fragment.StartsWith("{") && fragment.EndsWith("}")))
				return false;
			string pattern = fragment.Substring(1, fragment.Length - 2);
			int formatIndex = pattern.IndexOf(":");
			if (formatIndex >= 0) {
				format = pattern.Substring(formatIndex + 1).Trim();
				pattern = pattern.Substring(0, formatIndex);
			}
			pattern = pattern.Trim().ToUpper();
			if (!constants.ContainsKey(pattern))
				return false;
			patternConstant = constants[pattern];
			return true;
		}
		List<string> SplitString(string splitingString, char leftSeparator, char rightSeparator) {
			List<string> substrings = new List<string>();
			int leftStringIndex = 0;
			int rightStringIndex = 0;
			int currentIndex = 0;
			if (!String.IsNullOrEmpty(splitingString)) {
				foreach (char charElement in splitingString) {
					if (charElement == leftSeparator)
						leftStringIndex = currentIndex;
					else
						if (charElement == rightSeparator) {
							rightStringIndex = currentIndex;
							substrings.Add(splitingString.Substring(leftStringIndex, rightStringIndex - leftStringIndex + 1));
						}
					currentIndex++;
				}
				return substrings;
			}
			else
				return null;
		}
		public void SetContext(params object[] contexts) {
			foreach (PatternFragment fragment in fragments)
				foreach (var context in contexts)
					fragment.SetContext(context);
		}
		public string GetText() {
			string result = template;
			foreach (PatternFragment fragment in fragments)
				result = result.Replace(fragment.Fragment, fragment.GetText());
			return result;
		}
	}
	public static class PatternUtils {
		public static string Format(object value, string format) {
			IFormattable formattable = value as IFormattable;
			try {
				if (formattable != null)
					return formattable.ToString(format, CultureInfo.CurrentCulture);
			}
			catch {
			}
			return value is string ? (string)value : value.ToString() + ":" + format;
		}
	}
}
