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
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils;
using System.Windows.Controls;
namespace DevExpress.Xpf.Ribbon {
	public class ButtonGroupSetLayoutCalculator {
		[ThreadStatic]
		static ButtonGroupSetLayoutCalculator instance;
		static object lockObject = new object();
		public static ButtonGroupSetLayoutCalculator Instance {
			get {
				if (instance == null) {
					lock (lockObject)
						if (instance == null)
							instance = new ButtonGroupSetLayoutCalculator();
				}
				return instance;
			}			
		}
		#region static
		static readonly DependencyPropertyKey RowCountPropertyKey;
		public static readonly DependencyProperty RowCountProperty;
		static ButtonGroupSetLayoutCalculator() {
			RowCountPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("RowCount", typeof(int), typeof(ButtonGroupSetLayoutCalculator), new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
			RowCountProperty = RowCountPropertyKey.DependencyProperty;
		}
		internal static void SetRowCount(DependencyObject d, int value) { d.SetValue(RowCountPropertyKey, value); }
		public static int GetRowCount(DependencyObject d) { return (int)d.GetValue(RowCountProperty); }
		#endregion
		double[] itemWidthArray;
		int[] rowsConfig = { 0, 0, 0 };
		int[] bestRowsConfig = { 0, 0, 0 };
		public ButtonGroupSetLayoutCalculator() {
		}
		protected void CalcItemsWidth(Panel owner, int firstIndex, int lastIndex) {
			itemWidthArray = new double[lastIndex - firstIndex + 1];
			for (int i = firstIndex; i <= lastIndex; i++) {
				BarItemLinkInfo info = owner.Children[i] as BarItemLinkInfo;
				itemWidthArray[i - firstIndex] = info.DesiredSize.Width;
				BarButtonGroupLinkControl buttonGroup = info.LinkControl as BarButtonGroupLinkControl;
				if (buttonGroup != null && buttonGroup.Separator != null) {
					itemWidthArray[i - firstIndex] -= buttonGroup.Separator.DesiredSize.Width;
				}
			}
		}
		protected int GetFirstElementIndexOfRowImpl(int offset, int rowIndex) {
			int index = offset;
			for (int i = 0; i < rowIndex; i++)
				index += bestRowsConfig[i];
			return index;
		}
		protected int GetLastElementIndexOfRowImpl(int offset, int rowIndex) {
			return GetFirstElementIndexOfRow(offset, rowIndex) + bestRowsConfig[rowIndex] - 1;
		}
		protected int[] RowElementCountImpl { get { return bestRowsConfig; } }
		protected void SaveRowsConfig() {
			for (int rowIndex = 0; rowIndex < 3; rowIndex++) {
				bestRowsConfig[rowIndex] = rowsConfig[rowIndex];
			}
		}
		protected int GetBeginIndex(int[] rowCfg, int row) {
			int resIndex = 0;
			for (int rowIndex = 0; rowIndex < row; rowIndex++)
				resIndex += rowCfg[rowIndex];
			return resIndex;
		}
		protected double GetRowWidth(int rowIndex) {
			int beginIndex = GetBeginIndex(rowsConfig, rowIndex);
			double resWidth = 0;
			for (int itemIndex = beginIndex; itemIndex < beginIndex + rowsConfig[rowIndex]; itemIndex++) {
				resWidth += itemWidthArray[itemIndex];
			}
			return resWidth;
		}
		protected double GetColumnMaxWidth(int rowCount) {
			double maxWidth = 0;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				maxWidth = Math.Max(maxWidth, GetRowWidth(rowIndex));
			}
			return maxWidth;
		}
		protected double CheckCombination(double prevMaxWidth, int rowCount) {
			double maxWidth = GetColumnMaxWidth(rowCount);
			if (prevMaxWidth > maxWidth) {
				prevMaxWidth = maxWidth;
				SaveRowsConfig();
			}
			return prevMaxWidth;
		}
		protected double FindOptimal3RowsConfiguration() {
			double prevMaxWidth = double.MaxValue;
			int itemCount = itemWidthArray.GetLength(0);
			for (rowsConfig[0] = 1; rowsConfig[0] <= itemCount - 2; rowsConfig[0]++) {
				for (rowsConfig[1] = 1; rowsConfig[1] <= itemCount - 1 - rowsConfig[0]; rowsConfig[1]++) {
					rowsConfig[2] = itemCount - rowsConfig[0] - rowsConfig[1];
					prevMaxWidth = CheckCombination(prevMaxWidth, 3);
				}
			}
			return prevMaxWidth;
		}
		protected double FindOptimal2RowsConfiguration() {
			double prevMaxWidth = double.MaxValue;
			int itemCount = itemWidthArray.GetLength(0);
			for (rowsConfig[0] = 1; rowsConfig[0] <= itemCount - 1; rowsConfig[0]++) {
				rowsConfig[1] = itemCount - rowsConfig[0];
				prevMaxWidth = CheckCombination(prevMaxWidth, 2);
			}
			return prevMaxWidth;
		}
		protected int CalcGroupsLayoutImpl(Panel owner, int firstIndex, int lastIndex, int rowCount) {
			CalcItemsWidth(owner, firstIndex, lastIndex);
			bestRowsConfig[0] = 0;
			bestRowsConfig[1] = 0;
			bestRowsConfig[2] = 0;
			int itemCount = lastIndex - firstIndex + 1;
			if (itemCount == 1) {
				bestRowsConfig[0] = 1;
				return 1;
			} else if (rowCount == 2) {
				FindOptimal2RowsConfiguration();
				return 2;
			} else {
				FindOptimal3RowsConfiguration();
				return 3;
			}
		}
		public static int[] RowElementCount { get { return Instance.RowElementCountImpl; } }
		public static int CalcGroupsLayout(Panel owner, int firstIndex, int lastIndex, int rowCount) {
			return Instance.CalcGroupsLayoutImpl(owner, firstIndex, lastIndex, rowCount);
		}
		public static int GetFirstElementIndexOfRow(int offset, int rowIndex) {
			return Instance.GetFirstElementIndexOfRowImpl(offset, rowIndex);
		}
		public static int GetLastElementIndexOfRow(int offset, int rowIndex) {
			return Instance.GetLastElementIndexOfRowImpl(offset, rowIndex);
		}
	}
}
