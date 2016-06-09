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

using System.Collections.Generic;
using System.Windows.Media;
using DevExpress.Map;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map.Native {
	public class WpfColorProportionsCalculator : ProportionsCalculator<Color> {
		protected override Color DefaultValue {
			get { return MapColorizer.DefaultColor; }
		}
		protected override bool SupportsMinMaxValues {
			get { return true; }
		}
		protected override Color TransformValue(Color fromUnit, Color toUnit, double ratio) {
			return Color.FromArgb(ConvertColorValue(fromUnit.A, toUnit.A, ratio),
								  ConvertColorValue(fromUnit.R, toUnit.R, ratio),
								  ConvertColorValue(fromUnit.G, toUnit.G, ratio),
								  ConvertColorValue(fromUnit.B, toUnit.B, ratio));
		}
		byte ConvertColorValue(byte fromValue, byte toValue, double ratio) {
			return (byte)(fromValue * (1.0 - ratio) + toValue * ratio);
		}
	}
	public static class WpfColorizerColorHelper {
		static WpfColorProportionsCalculator calculator = new WpfColorProportionsCalculator();
		public static Color CalculateValue(IList<Color> colors, IList<double> ranges, bool approx, IRangeDistribution distribution, double value) {
			return calculator.CalculateProportionalValue(colors, ranges, approx, distribution, value);
		}
	}
}
