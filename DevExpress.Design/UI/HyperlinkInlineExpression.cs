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

namespace DevExpress.Design.UI {
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Documents;
	public static class HyperlinkInlineExpression {
		#region Properties
		public static readonly DependencyProperty InlineExpressionProperty = DependencyProperty.RegisterAttached(
			"InlineExpression", typeof(string), typeof(HyperlinkInlineExpression), new PropertyMetadata(null, OnInlineExpressionChanged));
		public static void SetInlineExpression(DependencyObject dObj, string value) {
			dObj.SetValue(InlineExpressionProperty, value);
		}
		public static string GetInlineExpression(DependencyObject dObj) {
			return (string)dObj.GetValue(InlineExpressionProperty);
		}
		#endregion Properties
		static void OnInlineExpressionChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			TextBlock textBlock = dObj as TextBlock;
			if(textBlock != null) {
				textBlock.RemoveHandler(Hyperlink.ClickEvent, new RoutedEventHandler(OnHyperlinkClick));
				textBlock.Inlines.Clear();
				string expression = e.NewValue as string;
				if(string.IsNullOrEmpty(expression))
					return;
				foreach(Inline inline in GetInlines(expression))
					textBlock.Inlines.Add(inline);
				textBlock.AddHandler(Hyperlink.ClickEvent, new RoutedEventHandler(OnHyperlinkClick));
			}
		}
		static void OnHyperlinkClick(object sender, RoutedEventArgs e) {
			Hyperlink hyperlink = e.OriginalSource as Hyperlink;
			if(hyperlink != null && hyperlink.NavigateUri != null) {
				try {
					System.Diagnostics.Process.Start(hyperlink.NavigateUri.OriginalString);
				}
				catch { }
			}
		}
		static Regex hyperlinkRegex = new Regex(@"\[(?<link>.*?)\]\((?<uri>.*?)\)|(?<break>\s*</br>\s*)|<b>(?<bold>.*?)</b>", RegexOptions.Compiled);
		static IEnumerable<Inline> GetInlines(string s) {
			do {
				Match match = hyperlinkRegex.Match(s);
				if(match.Success) {
					if(match.Index > 0)
						yield return new Run(s.Substring(0, match.Index));
					string br = match.Groups["break"].Value;
					if(br.Trim() == "</br>") {
						s = s.Substring(match.Index + match.Length);
						yield return new LineBreak();
						continue;
					}
					string bold = match.Groups["bold"].Value;
					if(!string.IsNullOrEmpty(bold)) {
						s = s.Substring(match.Index + match.Length);
						yield return new Bold(new Run(bold));
						continue;
					}
					string link = match.Groups["link"].Value;
					string uri = match.Groups["uri"].Value;
					s = s.Substring(match.Index + match.Length);
					yield return new Hyperlink(new Run(link)) { NavigateUri = new Uri(uri) };
				}
				else {
					if(s.Length > 0)
						yield return new Run(s);
					break;
				}
			}
			while(s.Length > 0);
		}
	}
}
