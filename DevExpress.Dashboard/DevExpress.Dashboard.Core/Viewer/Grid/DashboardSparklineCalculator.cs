#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
namespace DevExpress.DashboardCommon.Viewer {
	public class DashboardSparklineCalculator {
		class ValueItem {
			readonly string valueString;
			readonly int valueWidth;
			public string ValueString { get { return valueString; } }
			public int ValueWidth { get { return valueWidth; } }
			public ValueItem(string valueString, int valueWidth) {
				this.valueString = valueString;
				this.valueWidth = valueWidth;
			}
		}
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
#endif        
		readonly IList<double> data;
		readonly IList<string> texts;
		readonly bool showStartEndValues;
		readonly Dictionary<double, ValueItem> valueItemsRepository = new Dictionary<double, ValueItem>();
		int? maxValueWidth;
		public bool ShowStartEndValues { get { return showStartEndValues; } }
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
#endif
		public DashboardSparklineCalculator(IList<double> data, IList<string> texts, bool showStartEndValues) {
			this.data = data;
			this.texts = texts;
			this.showStartEndValues = showStartEndValues;
		}
		void AddValueItem(double value, ValueItem item) {
			if(!valueItemsRepository.ContainsKey(value))
				valueItemsRepository.Add(value, item);
		}
		void Calculate(Graphics graphics, Font font) {
			for(int i = 0; i < data.Count; i++) {
				double value = data[i];
				string text = texts[i];
				int textWidth = Convert.ToInt32(graphics.MeasureString(text, font).Width);
				AddValueItem(value, new ValueItem(text, textWidth));
				if(!maxValueWidth.HasValue || textWidth > maxValueWidth.Value)
					maxValueWidth = textWidth;
			}
		}
		public int GetMaxValueWidth(Graphics graphics, Font font) {
			if(maxValueWidth == null)
				Calculate(graphics, font);
			return maxValueWidth.HasValue ? maxValueWidth.Value : 0;
		}
		public int GetValueWidth(double value, Graphics graphics, Font font) {
			if(!valueItemsRepository.ContainsKey(value))
				Calculate(graphics, font);
			return valueItemsRepository[value].ValueWidth;
		}
		public Rectangle GetStartValueBounds(double startValue, Rectangle bounds, Graphics graphics, Font font) {
			int startValueWidth = GetValueWidth(startValue, graphics, font);
			int maxValueWidth = GetMaxValueWidth(graphics, font);
			return bounds.Width > 2 * maxValueWidth ? new Rectangle(bounds.X + (maxValueWidth - startValueWidth), bounds.Y, startValueWidth, bounds.Height) : Rectangle.Empty;
		}
		public Rectangle GetEndValueBounds(double endValue, Rectangle bounds, Graphics graphics, Font font) {
			int endValueWidth = GetValueWidth(endValue, graphics, font);
			return new Rectangle(Math.Max(bounds.Right - endValueWidth, bounds.Left), bounds.Y, Math.Min(endValueWidth, bounds.Width), bounds.Height);
		}
		public Rectangle GetSparklineStartEndBounds(Rectangle bounds, Graphics graphics, Font font) {
			int maxValueWidth = GetMaxValueWidth(graphics, font);
			return new Rectangle(bounds.X + maxValueWidth, bounds.Y, bounds.Width - 2 * maxValueWidth, bounds.Height);
		}
		public string GetValueString(double value, Graphics graphics, Font font) {
			if(!valueItemsRepository.ContainsKey(value))
				Calculate(graphics, font);
			return valueItemsRepository[value].ValueString;
		}
		public void Clear() {
			valueItemsRepository.Clear();
			maxValueWidth = null;
		}
	}
}
