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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[TemplatePart(Name = "PART_InterlaceItems", Type = typeof(ItemsControl)), NonCategorized]
	public class InterlaceControl : ChartElementBase, IAxis2DElement {
		public static readonly DependencyProperty InterlaceItemsProperty = DependencyPropertyManager.Register("InterlaceItems", typeof(ObservableCollection<InterlaceItem>), typeof(InterlaceControl));
		readonly AxisBase axis;
		[NonTestableProperty]
		public ObservableCollection<InterlaceItem> InterlaceItems {
			get { return (ObservableCollection<InterlaceItem>)GetValue(InterlaceItemsProperty); }
			set { SetValue(InterlaceItemsProperty, value); }
		}
		public AxisBase Axis { get { return axis; } }
		#region IAxis2DElement implementation
		AxisBase IAxis2DElement.Axis { get { return axis; } }
		bool IAxis2DElement.Visible { get { return true; } }
		#endregion
		public InterlaceControl(AxisBase axis) {
			DefaultStyleKey = typeof(InterlaceControl);
			this.axis = axis;
		}
		internal void UpdateInterlaceItems(IList<InterlacedData> interlacedData, Rect viewport) {
			IAxisMapping axisMapping = axis.CreateMapping(viewport);
			int count = interlacedData.Count;
			ObservableCollection<InterlaceItem> interlaceItems = InterlaceItems;
			if (interlaceItems == null || interlaceItems.Count != count) {
				interlaceItems = new ObservableCollection<InterlaceItem>();
				for (int i = 0; i < count; i++)
					interlaceItems.Add(new InterlaceItem(axis));
				InterlaceItems = interlaceItems;
			}
			double itemsLength = axis.IsVertical ? viewport.Width : viewport.Height;
			for (int i = 0; i < count; i++)
				interlaceItems[i].Geometry = axis.CreateInterlaceGeometry(viewport, axisMapping, interlacedData[i].Near, interlacedData[i].Far);
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size constraint = new Size(MathUtils.ConvertInfinityToDefault(availableSize.Width, 0), MathUtils.ConvertInfinityToDefault(availableSize.Height, 0));
			base.MeasureOverride(constraint);
			return constraint;
		}
	}
}
