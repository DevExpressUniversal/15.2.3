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

using System.Windows.Media;
namespace DevExpress.Xpf.Charts {
	public class PredefinedIndicatorsPalette : IndicatorsPalette {
		readonly ColorCollection predefinedColors = new ColorCollection();
		protected internal override ColorCollection ActualColors { get { return predefinedColors; } }
		PredefinedIndicatorsPalette() { }
		protected PredefinedIndicatorsPalette(Color[] colors) {
			foreach (Color color in colors)
				predefinedColors.Add(color);
		}
		protected override ChartDependencyObject CreateObject() {
			return new PredefinedIndicatorsPalette();
		}
	}
	public class DefaultIndicatorsPalette : PredefinedIndicatorsPalette {
		public override string PaletteName { get { return "Chameleon"; } }
		public DefaultIndicatorsPalette()
			: base(new Color[] {
			Color.FromArgb(255, 255, 9, 233),
			Color.FromArgb(255, 11, 197, 255),
			Color.FromArgb(255, 147, 8, 255),
			Color.FromArgb(255, 0, 194, 155) }) { }
	}
}
