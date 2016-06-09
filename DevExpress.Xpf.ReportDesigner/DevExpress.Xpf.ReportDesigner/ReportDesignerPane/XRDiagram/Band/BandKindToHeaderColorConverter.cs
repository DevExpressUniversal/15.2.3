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
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	public enum BandHeaderMouseState {
		Normal,
		Hover,
		Focused
	}
	public class BandKindToHeaderColorConverter : IValueConverter {
		enum InnerBandKind {
			Header,
			Body,
			Footer
		}
		static Color DefaultColor {
			get { return Color.FromRgb(0xB3, 0xDC, 0xEE); }
		}
		readonly Dictionary<InnerBandKind, Dictionary<BandHeaderMouseState, Color>> colorsMap = new Dictionary<InnerBandKind, Dictionary<BandHeaderMouseState, Color>> {
			{ InnerBandKind.Header, new Dictionary<BandHeaderMouseState, Color> {
				{ BandHeaderMouseState.Normal, Color.FromRgb(0xB4, 0xE1, 0xDB) },
				{ BandHeaderMouseState.Hover, Color.FromRgb(0xC9, 0xF0, 0xEB) },
				{ BandHeaderMouseState.Focused, Color.FromRgb(0x93, 0xD3, 0xCA) }
			} },
			{ InnerBandKind.Body, new Dictionary<BandHeaderMouseState, Color> {
				{ BandHeaderMouseState.Normal, DefaultColor },
				{ BandHeaderMouseState.Hover, Color.FromRgb(0xC7, 0xEC, 0xF9) },
				{ BandHeaderMouseState.Focused, Color.FromRgb(0x92, 0xCB, 0xE6) }
			} },
			{ InnerBandKind.Footer, new Dictionary<BandHeaderMouseState, Color> {
				{ BandHeaderMouseState.Normal, Color.FromRgb(0xC5, 0xCC, 0xE7) },
				{ BandHeaderMouseState.Hover, Color.FromRgb(0xD7, 0xDD, 0xF3) },
				{ BandHeaderMouseState.Focused, Color.FromRgb(0xAA, 0xB4, 0xDB) }
			} }
		};
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var bandHeaderMouseState = parameter is BandHeaderMouseState
				? (BandHeaderMouseState)parameter
				: BandHeaderMouseState.Normal;
			InnerBandKind bandKind = GetInnerBandKind((BandKind)value);
			Color color = GetColorByBandKind(bandKind, bandHeaderMouseState);
			return new SolidColorBrush(color);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
		static InnerBandKind GetInnerBandKind(BandKind bandKind) {
			switch(bandKind) {
				case BandKind.GroupHeader:
				case BandKind.PageHeader:
				case BandKind.ReportHeader:
				case BandKind.TopMargin:
					return InnerBandKind.Header;
				case BandKind.Detail:
				case BandKind.DetailReport:
				case BandKind.SubBand:
				case BandKind.None:
					return InnerBandKind.Body;
				case BandKind.BottomMargin:
				case BandKind.GroupFooter:
				case BandKind.PageFooter:
				case BandKind.ReportFooter:
					return InnerBandKind.Footer;
				default:
					Debug.Fail(string.Format("BandKind {0} is not supported", bandKind));
					return InnerBandKind.Body;
			}
		}
		Color GetColorByBandKind(InnerBandKind bandKind, BandHeaderMouseState mouseState) {
			Dictionary<BandHeaderMouseState, Color> map;
			Color result;
			return colorsMap.TryGetValue(bandKind, out map) && map.TryGetValue(mouseState, out result)
				? result
				: DefaultColor;
		}
	}
}
