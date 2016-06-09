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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.Snap.Core.Native {
	public abstract class ChartBoxInfoPanelViewInfo {
		public const int Margin = 8;
		public const int Padding = 24;
		public const int TextPadding = 3;
		readonly List<ChartBoxInfoPanelSectionViewInfo> sections = new List<ChartBoxInfoPanelSectionViewInfo>();
		readonly Rectangle boxBounds;
		readonly ITextMeasurer textMeasurer;
		readonly int fontHeight;
		Rectangle bounds;
		bool isVisible;
		protected ChartBoxInfoPanelViewInfo(Rectangle boxBounds, List<string> values, List<string> arguments, ITextMeasurer textMeasurer) {
			this.boxBounds = boxBounds;
			this.textMeasurer = textMeasurer;
			this.fontHeight = textMeasurer.GetStringHeight("Wq");
			Calculate(values, arguments);
			this.isVisible = boxBounds.Height >= Bounds.Height + 2 * Margin && boxBounds.Width >= Bounds.Width + 2 * Margin;
		}
		protected Rectangle BoxBounds { get { return boxBounds; } }
		protected ITextMeasurer TextMeasurer { get { return textMeasurer; } }
		public Rectangle Bounds { get { return bounds; } protected set { bounds = value; } }
		public int FontHeight { get { return fontHeight; } }
		public List<ChartBoxInfoPanelSectionViewInfo> Sections { get { return sections; } }
		public bool IsVisible { get { return isVisible; } }
		protected ChartBoxInfoPanelStringViewInfo CalculateStringViewInfo(string text, int top) {
			return new ChartBoxInfoPanelStringViewInfo { Text = text, Location = new Point(Padding, top) };
		}
		public Rectangle GetPanelBoundsCore(int width, int height) {
			int doublePadding = 2 * Padding;
			return new Rectangle(Margin, (int)BoxBounds.Height - height - Margin - doublePadding, width + doublePadding, height + doublePadding);
		}
		protected abstract void Calculate(List<string> values, List<string> arguments);
		public static ChartBoxInfoPanelViewInfo Create(Rectangle boxBounds, List<string> values, List<string> arguments, ITextMeasurer textMeasurer) {
			if (values.Count > 0 || arguments.Count > 0)
				return new ChartBoxInfoPanelViewInfoBound(boxBounds, values, arguments, textMeasurer);
			return null;
		}
	}
	public class ChartBoxInfoPanelViewInfoBound : ChartBoxInfoPanelViewInfo {
		public const int AdditionalPanelWidth = 45;
		public const string UIStringValues = "VALUES";
		public const string UIStringArguments = "ARGUMENTS";
		public const int HeadersCount = 2;
		public ChartBoxInfoPanelViewInfoBound(Rectangle boxBounds, List<string> values, List<string> arguments, ITextMeasurer textMeasurer)
			: base(boxBounds, values, arguments, textMeasurer) {
		}
		protected override void Calculate(List<string> values, List<string> arguments) {
			ChartBoxInfoPanelSectionViewInfo valuesSection = CalculateSection(UIStringValues, values, 0);
			if (valuesSection != null)
				Sections.Add(valuesSection);
			int top = valuesSection != null ? GetSectionBottom(valuesSection) : 0;
			ChartBoxInfoPanelSectionViewInfo argumentsSection = CalculateSection(UIStringArguments, arguments, top);
			if (argumentsSection != null)
				Sections.Add(argumentsSection);
			top = argumentsSection != null ? GetSectionBottom(argumentsSection) : top;
			Bounds = GetPanelBounds(values, arguments, top - Padding);
		}
		protected ChartBoxInfoPanelSectionViewInfo CalculateSection(string header, List<string> values, int top) {
			if (values == null || values.Count == 0)
				return null;
			top += Padding;
			ChartBoxInfoPanelSectionViewInfo sectionViewInfo = new ChartBoxInfoPanelSectionViewInfo();
			sectionViewInfo.Header = CalculateStringViewInfo(header, top);
			foreach(string value in values) {
				top += FontHeight + TextPadding;
				sectionViewInfo.Items.Add(CalculateStringViewInfo(value, top));
			}
			return sectionViewInfo;
		}
		Rectangle GetPanelBounds(List<string> values, List<string> arguments, int height) {
			return GetPanelBoundsCore(GetMaxWidth(values, arguments, UIStringValues, UIStringArguments) + AdditionalPanelWidth, height);
		}
		int GetSectionBottom(ChartBoxInfoPanelSectionViewInfo section) {
			return (section.Items.Any() ? section.Items.Last() : section.Header).Location.Y + FontHeight;
		}
		int GetMaxWidth(List<string> values, List<string> arguments, params string[] strings) {
			return (int)values.Concat(arguments).Concat(strings).Select(text => TextMeasurer.GetStringWidth(text)).Max();
		}
	}
	public class ChartBoxInfoPanelSectionViewInfo {
		readonly List<ChartBoxInfoPanelStringViewInfo> items = new List<ChartBoxInfoPanelStringViewInfo>();
		public ChartBoxInfoPanelStringViewInfo Header { get; set; }
		public List<ChartBoxInfoPanelStringViewInfo> Items { get { return items; } }
	}
	public class ChartBoxInfoPanelStringViewInfo {
		public string Text { get; set; }
		public Point Location { get; set; }
	}
	public interface ITextMeasurer {
		Size GetStringSize(string text);
		int GetStringWidth(string text);
		int GetStringHeight(string text);
	}
	public abstract class TextMeasurerBase : ITextMeasurer {
		public abstract Size GetStringSize(string text);
		public int GetStringWidth(string text) {
			return GetStringSize(text).Width;
		}
		public int GetStringHeight(string text) {
			return GetStringSize(text).Height;
		}
	}
	public class GDIPlusPreciseTextMeasurer : TextMeasurerBase, IDisposable {
		readonly Graphics graphics;
		readonly Font font;
		public GDIPlusPreciseTextMeasurer(Graphics graphics, Font font) {
			this.graphics = graphics;
			this.font = font;
		}
		protected Graphics Graphics { get { return graphics; } }
		protected Font Font { get { return font; } }
		public override Size GetStringSize(string text) {
			return Graphics.MeasureString(text, Font, int.MaxValue, StringFormat.GenericTypographic).ToSize();
		}
		public void Dispose() {
		}
	}
}
