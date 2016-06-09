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

namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows;
	using Diagram;
	using Mvvm.UI.Native;
	public abstract partial class DiagramItemLayout : DependencyObject {
		static DiagramItemLayout() {
			DependencyPropertyRegistrator<DiagramItemLayout>.New()
				.Register(d => d.Dpi, out DpiProperty, 1.0f)
				.Register(d => d.PositionF, out PositionFProperty, new System.Drawing.PointF())
				.Register(d => d.WidthF, out WidthFProperty, 0.0f)
				.Register(d => d.HeightF, out HeightFProperty, 0.0f)
				.Register(d => d.Position, out PositionProperty, new Point())
				.Register(d => d.Width, out WidthProperty, 0.0)
				.Register(d => d.Height, out HeightProperty, 0.0)
				.Register(d => d.Left, out LeftProperty, 0.0)
				.Register(d => d.Top, out TopProperty, 0.0)
				.Register(d => d.Right, out RightProperty, 0.0)
				.Register(d => d.Bottom, out BottomProperty, 0.0)
				.Register(d => d.ParentLeftAbsolute, out ParentLeftAbsoluteProperty, 0.0)
				.Register(d => d.ParentTopAbsolute, out ParentTopAbsoluteProperty, 0.0)
				.Register(d => d.LeftAbsolute, out LeftAbsoluteProperty, 0.0)
				.Register(d => d.TopAbsolute, out TopAbsoluteProperty, 0.0)
				.Register(d => d.RightAbsolute, out RightAbsoluteProperty, 0.0)
				.Register(d => d.BottomAbsolute, out BottomAbsoluteProperty, 0.0)
				.RegisterAttached((DiagramItem d) => GetLayout(d), out LayoutProperty, (DiagramItemLayout)null)
				.Register(d => d.Item, out ItemProperty, (DiagramItem)null, (d, e) => d.OnItemChanged(e))
			;
		}
		void OnItemChanged(DependencyPropertyChangedEventArgs e) {
			var oldValue = (DiagramItem)e.OldValue;
			var newValue = (DiagramItem)e.NewValue;
			if(oldValue == newValue) return;
			if(oldValue != null)
				throw new InvalidOperationException();
			BindItem(newValue);
		}
		protected abstract void BindItem(DiagramItem item);
	}
	public interface IDiagramItemLayout {
		Point PositionAbsolute { get; }
		Size Size { get; }
	}
}
