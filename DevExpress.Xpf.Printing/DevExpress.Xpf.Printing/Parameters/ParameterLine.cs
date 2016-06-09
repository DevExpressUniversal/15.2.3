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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Printing.Parameters {
	public class ParameterLineLayout : ContentControl {
		static ParameterLineLayout() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ParameterLineLayout), new FrameworkPropertyMetadata(typeof(ParameterLineLayout)));
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class AutoFitColumnGrid : Grid {
		public AutoFitColumnGrid() {
			SizeChanged += OnSizeChanged;
		}
		protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint) {
			var size = base.MeasureOverride(constraint);
			return new Size(constraint.Width, size.Height);
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			if(this.ColumnDefinitions.Count <= 1)
				return;
			var availableWidth = CalculateFirstColumnAvailableWidth();
			var measuredWidth = GetFirstColumnDesiredWidth();
			this.ColumnDefinitions[0].Width = new GridLength(Math.Min(availableWidth, measuredWidth), GridUnitType.Pixel);
		}
		double CalculateFirstColumnAvailableWidth() {
			var columnsMinWidth = 0d;
			for(int i = 1; i < this.ColumnDefinitions.Count; i++) {
				var maxMinWidth = Math.Max(this.ColumnDefinitions[i].Width.Value, this.ColumnDefinitions[i].MinWidth);
				var columnChildren = GetColumnChildren(i);
				columnChildren.ForEach(x => maxMinWidth = Math.Max(maxMinWidth, x.MinWidth));
				columnsMinWidth += maxMinWidth;
			}
			var result = ActualWidth - columnsMinWidth;
			return result < 0 ? 0 : result;
		}
		double GetFirstColumnDesiredWidth() {
			double width = 0d;
			var columnChildren = GetColumnChildren(0);
			columnChildren.ForEach(x => {
				x.Measure(new Size(double.PositiveInfinity, x.ActualHeight));
				width = Math.Max(width, x.DesiredSize.Width);
			});
			return width + 1;
		}
		List<FrameworkElement> GetColumnChildren(int columnIndex) {
			return Children
				.OfType<FrameworkElement>()
				.Where(x => Grid.GetColumn(x) == columnIndex)
				.ToList();
		}
	}
}
