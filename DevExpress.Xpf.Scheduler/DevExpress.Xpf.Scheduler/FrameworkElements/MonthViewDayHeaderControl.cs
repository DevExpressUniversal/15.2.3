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
using System.Collections.Generic;
using System.Windows.Media;
using DevExpress.Xpf.Scheduler.Drawing;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Scheduler.Drawing { 
	public class TextMeasurer {
		TextBlock textBlock = new TextBlock();
#if SL
		public Size PerformMeasure() {
			textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			return textBlock.DesiredSize;
		}
#else
		public Size PerformMeasure() {
			textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			return textBlock.DesiredSize;
		}
#endif
		public string Text { get { return textBlock.Text; } set { textBlock.Text = value; } }
		public double FontSize { get { return textBlock.FontSize; } set { textBlock.FontSize = value; } }
		public void BeginMeasure(Control target) {
			textBlock.FontFamily = target.FontFamily;
			textBlock.FontSize = target.FontSize;
			textBlock.FontStretch = target.FontStretch;
			textBlock.FontStyle = target.FontStyle;
			textBlock.FontWeight = target.FontWeight;
		}
		public void EndMeasure(Control target) {
		}
		public string ChooseBestCaption(Control target, List<string> captions) {
			if (captions.Count == 1)
				return captions[0];
			BeginMeasure(target);
			try {
				return ChooseBestCaptionCore(target, captions);
			}
			finally {
				EndMeasure(target);
			}
		}
		string ChooseBestCaptionCore(Control target, List<string> captions) {
			int count = captions.Count;
			if (count <= 0)
				return String.Empty;
			if (count == 1)
				return captions[0];
			string result = String.Empty;
			double minFitRatio = double.MaxValue;
			double maxFitRatio = double.MinValue;
			int index = 0;
			for (int i = 0; i < count; i++) {
				double fitRatio = CalculateFitRatio(target, captions[i]);
				if (fitRatio < minFitRatio) {
					if (fitRatio >= 0) {
						result = captions[i];
						minFitRatio = fitRatio;
					}
				}
				if (fitRatio > maxFitRatio) {
					if (fitRatio < 0) {
						maxFitRatio = fitRatio;
						index = i;
					}
				}
			}
			if (String.IsNullOrEmpty(result))
				return captions[index];
			else
				return result;
		}
		double CalculateFitRatio(Control target, string caption) {
			textBlock.Text = caption;
			Size size = PerformMeasure();
			return target.ActualWidth - size.Width;
		}
	}
	public class DayOfWeekHeaderControl : AutoTextFormatControlBase {
		readonly string[] DayNames = DateTimeFormatInfo.CurrentInfo.DayNames;
		readonly string[] AbbrDayNames = DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames;
		static DayOfWeekHeaderControl() {
		}
#if !SL
		public DayOfWeekHeaderControl() {
			DefaultStyleKey = typeof(DayOfWeekHeaderControl);
		}
#endif
		#region IsCompressed
		public static readonly DependencyProperty IsCompressedProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayOfWeekHeaderControl, bool>("IsCompressed", false, (d, e) => d.InvalidateContent());
		public bool IsCompressed { get { return (bool)GetValue(IsCompressedProperty); } set { SetValue(IsCompressedProperty, value); } }
		#endregion
		#region DayOfWeek
		public static readonly DependencyProperty DayOfWeekProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayOfWeekHeaderControl, DayOfWeek>("DayOfWeek", DayOfWeek.Sunday, (d, e) => d.InvalidateContent());
		public DayOfWeek DayOfWeek { get { return (DayOfWeek)GetValue(DayOfWeekProperty); } set { SetValue(DayOfWeekProperty, value); } }
		#endregion
		protected override string[] CalculateCaptions() {
			List<string> captions = new List<string>();
			if (IsCompressed) {
				captions.Add(String.Format("{0}/{1}", DayNames[6], DayNames[0]));
				captions.Add(String.Format("{0}/{1}", AbbrDayNames[6], AbbrDayNames[0]));
			}
			else {
				int dayOfWeekNumber = (int)DayOfWeek;
				captions.Add(DayNames[dayOfWeekNumber]);
				captions.Add(AbbrDayNames[dayOfWeekNumber]);
			}
			return captions.ToArray();
		}
	}
}
