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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Charts.Designer.Native {
	public abstract class WpfChartFontModel : ChartModelElement {
		public Control FontHolder { get { return (Control)ChartElement; } }
		public override IEnumerable<ChartModelElement> Children { get { return null; } }
		public FontFamily FontFamily {
			get { return FontHolder.FontFamily; }
			set {
				if (FontHolder.FontFamily != value) {
					FontHolder.FontFamily = value;
					OnPropertyChanged("FontFamily");
				}
			}
		}
		public FontStyle FontStyle {
			get { return FontHolder.FontStyle; }
			set {
				if (FontHolder.FontStyle != value) {
					FontHolder.FontStyle = value;
					OnPropertyChanged("FontStyle");
				}
			}
		}
		public FontWeight FontWeight {
			get { return FontHolder.FontWeight; }
			set {
				if (FontHolder.FontWeight != value) {
					FontHolder.FontWeight = value;
					OnPropertyChanged("FontWeight");
				}
			}
		}
		public double FontSize {
			get { return FontHolder.FontSize; }
			set {
				if (FontHolder.FontSize != value) {
					FontHolder.FontSize = value;
					OnPropertyChanged("FontSize");
				}
			}
		}
		public WpfChartFontModel(ChartModelElement parent, Control fontHolder) : base(parent, fontHolder) {
		}
	}
}
