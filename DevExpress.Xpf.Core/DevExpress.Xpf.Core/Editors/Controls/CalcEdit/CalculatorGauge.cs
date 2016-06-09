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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Utils;
#if !SL
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Editors {
	public class CalculatorGauge : Control {
		public static readonly DependencyProperty TextProperty;
		static CalculatorGauge() {
			TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CalculatorGauge),
			  new PropertyMetadata((d, e) => ((CalculatorGauge)d).PropertyChangedText((string)e.OldValue)));
		}
		public CalculatorGauge() {
			this.SetDefaultStyleKey(typeof(CalculatorGauge));
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		string DecimalSeparator {
			get { return CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator; }
		}
		Panel SegmentPanel { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SegmentPanel = GetTemplateChild("ElementSegmentPanel") as Panel;
			UpdateSegments();
		}
		protected virtual CalculatorGaugeSegment CreateSegment() {
			return new CalculatorGaugeSegment();
		}
		protected virtual string GetTextToDisplay() {
			if (Text == null) return null;
			int i = 0;
			string decimalSeparator = DecimalSeparator;
			while (i < Text.Length)
				if (Text.IndexOf(decimalSeparator, i) == i)
					i += decimalSeparator.Length;
				else {
					if (Text[i] != '-' && !Char.IsDigit(Text[i]))
						return "Error";
					i++;
				}
			return Text;
		}
		protected virtual void PropertyChangedText(string oldValue) {
			UpdateSegments();
		}
		protected virtual void UpdateSegments() {
			if (SegmentPanel == null) return;
			string text = GetTextToDisplay();
			if (text == null) {
				SegmentPanel.Children.Clear();
				return;
			}
			int segmentCount = 0;
			int i = 0;
			string decimalSeparator = DecimalSeparator;
			while (i < text.Length) {
				if (text.IndexOf(decimalSeparator, i) == 0)
					throw new Exception();
				if (SegmentPanel.Children.Count == segmentCount)
					SegmentPanel.Children.Add(CreateSegment());
				CalculatorGaugeSegment segment = (CalculatorGaugeSegment)SegmentPanel.Children[segmentCount];
				segmentCount++;
				segment.Char = text[i];
				i++;
				segment.ShowDot = text.IndexOf(decimalSeparator, i) == i;
				if (segment.ShowDot)
					i += decimalSeparator.Length;
			}
			while (SegmentPanel.Children.Count > segmentCount)
				SegmentPanel.Children.RemoveAt(SegmentPanel.Children.Count - 1);
		}
	}
	public class CalculatorGaugeSegment : Control {
		[Flags]
		protected enum ElementType {
			SegmentLeftBottom = 1,
			SegmentLeftTop = 2,
			SegmentTop = 4,
			SegmentRightTop = 8,
			SegmentRightBottom = 16,
			SegmentBottom = 32,
			SegmentMiddle = 64,
			Dot = 128
		}
		public static readonly DependencyProperty CharProperty;
		public static readonly DependencyProperty ShowDotProperty;
		static CalculatorGaugeSegment() {
			CharProperty = DependencyProperty.Register("Char", typeof(char), typeof(CalculatorGaugeSegment),
			  new PropertyMetadata((d, e) => ((CalculatorGaugeSegment)d).PropertyChangedChar((char)e.OldValue)));
			ShowDotProperty = DependencyProperty.Register("ShowDot", typeof(bool), typeof(CalculatorGaugeSegment),
			  new PropertyMetadata((d, e) => ((CalculatorGaugeSegment)d).PropertyChangedShowDot()));
		}
		public CalculatorGaugeSegment() {
			this.SetDefaultStyleKey(typeof(CalculatorGaugeSegment));
		}
		public char Char {
			get { return (char)GetValue(CharProperty); }
			set { SetValue(CharProperty, value); }
		}
		public bool ShowDot {
			get { return (bool)GetValue(ShowDotProperty); }
			set { SetValue(ShowDotProperty, value); }
		}
		protected Style ElementStyleOff { get; private set; }
		protected Style ElementStyleOn { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateElementStyles();
			UpdateElements();
		}
		protected virtual ElementType GetVisibleElements() {
			ElementType result = 0;
			switch (Char) {
				case '0':
					result = ElementType.SegmentLeftBottom | ElementType.SegmentLeftTop | ElementType.SegmentTop | ElementType.SegmentRightTop | ElementType.SegmentRightBottom | ElementType.SegmentBottom;
					break;
				case '1':
					result = ElementType.SegmentRightTop | ElementType.SegmentRightBottom;
					break;
				case '2':
					result = ElementType.SegmentTop | ElementType.SegmentRightTop | ElementType.SegmentMiddle | ElementType.SegmentLeftBottom | ElementType.SegmentBottom;
					break;
				case '3':
					result = ElementType.SegmentTop | ElementType.SegmentRightTop | ElementType.SegmentMiddle | ElementType.SegmentRightBottom | ElementType.SegmentBottom;
					break;
				case '4':
					result = ElementType.SegmentLeftTop | ElementType.SegmentRightTop | ElementType.SegmentMiddle | ElementType.SegmentRightBottom;
					break;
				case '5':
					result = ElementType.SegmentTop | ElementType.SegmentLeftTop | ElementType.SegmentMiddle | ElementType.SegmentRightBottom | ElementType.SegmentBottom;
					break;
				case '6':
					result = ElementType.SegmentLeftBottom | ElementType.SegmentLeftTop | ElementType.SegmentTop | ElementType.SegmentMiddle | ElementType.SegmentRightBottom | ElementType.SegmentBottom;
					break;
				case '7':
					result = ElementType.SegmentTop | ElementType.SegmentRightTop | ElementType.SegmentRightBottom;
					break;
				case '8':
					result = ElementType.SegmentLeftBottom | ElementType.SegmentLeftTop | ElementType.SegmentTop | ElementType.SegmentRightTop | ElementType.SegmentRightBottom | ElementType.SegmentBottom | ElementType.SegmentMiddle;
					break;
				case '9':
					result = ElementType.SegmentLeftTop | ElementType.SegmentTop | ElementType.SegmentRightTop | ElementType.SegmentRightBottom | ElementType.SegmentBottom | ElementType.SegmentMiddle;
					break;
				case 'E':
					result = ElementType.SegmentLeftBottom | ElementType.SegmentLeftTop | ElementType.SegmentTop | ElementType.SegmentBottom | ElementType.SegmentMiddle;
					break;
				case 'o':
					result = ElementType.SegmentLeftBottom | ElementType.SegmentRightBottom | ElementType.SegmentBottom | ElementType.SegmentMiddle;
					break;
				case 'r':
					result = ElementType.SegmentLeftBottom | ElementType.SegmentMiddle;
					break;
				case '-':
					result = ElementType.SegmentMiddle;
					break;
			}
			if (ShowDot)
				result |= ElementType.Dot;
			return result;
		}
		protected virtual void PropertyChangedChar(char oldValue) {
			UpdateElements();
		}
		protected virtual void PropertyChangedShowDot() {
			UpdateElements();
		}
		protected virtual void UpdateElements() {
			if (ElementStyleOff == null || ElementStyleOn == null) return;
			ElementType visibleElements = GetVisibleElements();
			foreach(ElementType elementType in typeof(ElementType).GetValues()) {
				FrameworkElement element = GetTemplateChild("Element" + elementType.ToString()) as FrameworkElement;
				if (element == null) continue;
				if ((elementType & visibleElements) != 0)
					element.Style = ElementStyleOn;
				else
					element.Style = ElementStyleOff;
			}
		}
		void UpdateElementStyles() {
			FrameworkElement root = GetTemplateChild("ElementRoot") as FrameworkElement;
			if (root != null) {
				if (root.Resources.Contains("ElementStyleOff"))
					ElementStyleOff = root.Resources["ElementStyleOff"] as Style;
				if (root.Resources.Contains("ElementStyleOn"))
					ElementStyleOn = root.Resources["ElementStyleOn"] as Style;
			}
		}
	}
}
