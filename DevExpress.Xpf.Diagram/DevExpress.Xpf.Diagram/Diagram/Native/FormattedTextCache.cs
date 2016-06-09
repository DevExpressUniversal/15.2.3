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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.Diagram.Native {
	public class FormattedTextCache {
		public static IEnumerable<DependencyProperty> DependentProperties {
			get {
				yield return TextBlock.ForegroundProperty;
				yield return TextBlock.FontSizeProperty;
				yield return FrameworkElement.FlowDirectionProperty;
				yield return NumberSubstitution.SubstitutionProperty;
				yield return TextOptions.TextFormattingModeProperty;
				yield return TextBlock.FontFamilyProperty;
				yield return TextBlock.FontStyleProperty;
				yield return TextBlock.FontStretchProperty;
			}
		}
		readonly FrameworkElement owner;
		Typeface typeface;
		Typeface Typeface { get { return typeface ?? (typeface = CreateTypeface()); } }
		Dictionary<string, FormattedText> cache = new Dictionary<string, FormattedText>();
		public FormattedTextCache(FrameworkElement owner) {
			this.owner = owner;
		}
		public void Invalidate() {
			cache.Clear();
			typeface = null;
		}
		public FormattedText Get(string text) {
			return cache.GetOrAdd(text, () => CreateFormattedText(text));
		}
		FormattedText CreateFormattedText(string text) {
			var formattedText = new FormattedText(
				text,
				System.Globalization.CultureInfo.CurrentCulture,
				owner.FlowDirection,
				Typeface,
				TextBlock.GetFontSize(owner),
				TextBlock.GetForeground(owner),
				new NumberSubstitution() { Substitution = NumberSubstitution.GetSubstitution(owner) },
				TextOptions.GetTextFormattingMode(owner));
			formattedText.Trimming = TextTrimming.None;
			formattedText.TextAlignment = TextAlignment.Left;
			return formattedText;
		}
		Typeface CreateTypeface() {
			return new Typeface(
				TextBlock.GetFontFamily(owner),
				TextBlock.GetFontStyle(owner),
				TextBlock.GetFontWeight(owner),
				TextBlock.GetFontStretch(owner)
			);
		}
	}
}
