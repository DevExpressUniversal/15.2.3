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
using DevExpress.DemoData.Helpers;
using System.Windows.Controls;
using System.Windows;
namespace DevExpress.Xpf.DemoBase.Helpers {
	class WidthPanel : Panel {
		#region Dependency Properties
		public static readonly DependencyProperty MeasureTypeAutoProperty;
		static WidthPanel() {
			Type ownerType = typeof(WidthPanel);
			MeasureTypeAutoProperty = DependencyProperty.RegisterAttached("MeasureTypeAuto", typeof(bool), ownerType, new PropertyMetadata(false));
		}
		#endregion
		public static bool GetMeasureTypeAuto(UIElement e) { return (bool)e.GetValue(MeasureTypeAutoProperty); }
		public static void SetMeasureTypeAuto(UIElement e, bool v) { e.SetValue(MeasureTypeAutoProperty, v); }
		protected override Size MeasureOverride(Size availableSize) {
			double maxWidth = 0.0;
			double maxHeight = 0.0;
			foreach(UIElement child in Children) {
				child.Measure(availableSize);
				if(child.DesiredSize.Width > maxWidth)
					maxWidth = child.DesiredSize.Width;
				if(child.DesiredSize.Height > maxHeight)
					maxHeight = child.DesiredSize.Height;
			}
			Size size = new Size(maxWidth, maxHeight);
			foreach(UIElement child in Children) {
				if(GetMeasureTypeAuto(child)) continue;
				child.Measure(size);
			}
			return size;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach(UIElement child in Children) {
				child.Arrange(new Rect(new Point(), child.DesiredSize));
			}
			return finalSize;
		}
	}
}
