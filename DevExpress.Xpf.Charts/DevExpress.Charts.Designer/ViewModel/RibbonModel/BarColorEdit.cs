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

using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
namespace DevExpress.Charts.Designer.Native {
	public class BarColorEditViewModel : BarEditValueItemViewModel {
		public BarColorEditViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath)
			: base(command, source, editValuePath) {
			PropertyChanged += ColorEditItemViewModel_PropertyChanged;
		}
		public BarColorEditViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath, IValueConverter converter)
			: base(command, source, editValuePath, converter) {
			PropertyChanged += ColorEditItemViewModel_PropertyChanged;
		}
		void ColorEditItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName != "EditValue" || EditValue == null)
				return;
			SolidColorBrush brush;
			if (EditValue is SolidColorBrush)
				brush = (SolidColorBrush)EditValue;
			else
				brush = new SolidColorBrush((Color)EditValue);
			Rect smallRect = new Rect(new Point(0, 0), new Size(16, 16));
			RectangleGeometry smallGeometry = new RectangleGeometry(smallRect);
			GeometryDrawing smallSquare = new GeometryDrawing(brush, new Pen(), smallGeometry);
			DrawingImage smallGlyph = new DrawingImage(smallSquare);
			smallGlyph.Freeze();
			Glyph = smallGlyph;
			Rect largeRect = new Rect(new Point(0, 0), new Size(16, 16));
			RectangleGeometry largeGeometry = new RectangleGeometry(largeRect);
			GeometryDrawing largeSquare = new GeometryDrawing(brush, new Pen(), largeGeometry);
			DrawingImage largeGlyph = new DrawingImage(largeSquare);
			largeGlyph.Freeze();
			LargeGlyph = largeGlyph;
		}
	}
}
