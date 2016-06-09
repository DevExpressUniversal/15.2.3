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
using System.Windows.Media;
namespace DevExpress.Xpf.Charts {
	public class ColorObjectColorizer : ChartColorizerBase {
		public override Color? GetPointColor(object argument, object[] values, object colorKey, Palette palette) {
			Color? resultColor = null;
			if (colorKey is Color)
				resultColor = (Color)colorKey;
			else if (colorKey is int) {
				byte[] bytes = BitConverter.GetBytes((int)colorKey);
				resultColor = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
			}
			else if (colorKey is uint) {
				byte[] bytes = BitConverter.GetBytes((uint)colorKey);
				resultColor = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
			}
			else if (colorKey is string) {
				string colorString = (string)colorKey;
				if (!string.IsNullOrWhiteSpace(colorString))
					try {
						resultColor = (Color)ColorConverter.ConvertFromString(colorString);
					}
					catch {
					}
			}
			return resultColor;
		}
		protected override ChartDependencyObject CreateObject() {
			return new ColorObjectColorizer();
		}
	}
}
