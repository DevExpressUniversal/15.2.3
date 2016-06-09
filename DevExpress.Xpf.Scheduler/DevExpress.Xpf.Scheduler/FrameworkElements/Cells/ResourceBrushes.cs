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

using System.Windows;
using System.Windows.Media;
using DevExpress.XtraScheduler;
#if SL
using DependencyPropertyChangedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventHandler;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualResourceBrushes : DependencyObject {
		#region ResourceHeader
		public static readonly DependencyProperty ResourceHeaderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceBrushes, Brush>("ResourceHeader", null, (d, e) => d.OnResourceHeaderChanged(e.OldValue, e.NewValue));
		public Brush ResourceHeader { get { return (Brush)GetValue(ResourceHeaderProperty); } set { SetValue(ResourceHeaderProperty, value); } }
		#endregion
		#region Cell
		public static readonly DependencyProperty CellProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceBrushes, Brush>("Cell", null, (d, e) => d.OnCellChanged(e.OldValue, e.NewValue));
		public Brush Cell { get { return (Brush)GetValue(CellProperty); } set { SetValue(CellProperty, value); } }
		#endregion
		#region CellLight
		public static readonly DependencyProperty CellLightProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceBrushes, Brush>("CellLight", null, (d, e) => d.OnCellLightChanged(e.OldValue, e.NewValue));
		public Brush CellLight { get { return (Brush)GetValue(CellLightProperty); } set { SetValue(CellLightProperty, value); } }
		#endregion
		#region CellLightBorder
		public static readonly DependencyProperty CellLightBorderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceBrushes, Brush>("CellLightBorder", null, (d, e) => d.OnCellLightBorderChanged(e.OldValue, e.NewValue));
		public Brush CellLightBorder { get { return (Brush)GetValue(CellLightBorderProperty); } set { SetValue(CellLightBorderProperty, value); } }
		#endregion
		#region CellLightBorderDark
		public static readonly DependencyProperty CellLightBorderDarkProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceBrushes, Brush>("CellLightBorderDark", null, (d, e) => d.OnCellLightBorderDarkChanged(e.OldValue, e.NewValue));
		public Brush CellLightBorderDark { get { return (Brush)GetValue(CellLightBorderDarkProperty); } set { SetValue(CellLightBorderDarkProperty, value); } }
		#endregion
		#region CellBorder
		public static readonly DependencyProperty CellBorderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceBrushes, Brush>("CellBorder", null, (d, e) => d.OnCellBorderChanged(e.OldValue, e.NewValue));
		public Brush CellBorder { get { return (Brush)GetValue(CellBorderProperty); } set { SetValue(CellBorderProperty, value); } }
		#endregion
		#region CellBorderDark
		public static readonly DependencyProperty CellBorderDarkProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceBrushes, Brush>("CellBorderDark", null, (d, e) => d.OnCellBorderDarkChanged(e.OldValue, e.NewValue));
		public Brush CellBorderDark { get { return (Brush)GetValue(CellBorderDarkProperty); } set { SetValue(CellBorderDarkProperty, value); } }
		#endregion
		#region BrushChanged
		DependencyPropertyChangedEventHandler onBrushChanged;
		public event DependencyPropertyChangedEventHandler BrushChanged { add { onBrushChanged += value; } remove { onBrushChanged -= value; } }
		protected virtual void RaiseBrushChanged(DependencyProperty property, Brush oldBrush, Brush newBrush) {
			if (onBrushChanged != null)
				onBrushChanged(this, new DependencyPropertyChangedEventArgs(property, oldBrush, newBrush));
		}
		#endregion
		protected virtual void OnResourceHeaderChanged(Brush oldBrush, Brush newBrush) {
			RaiseBrushChanged(CellProperty, oldBrush, newBrush);
			lastSettedResourceHeader = newBrush;
		}
		protected virtual void OnCellChanged(Brush oldBrush, Brush newBrush) {
			RaiseBrushChanged(CellProperty, oldBrush, newBrush);
			lastSettedCell = newBrush;
		}
		protected virtual void OnCellLightChanged(Brush oldBrush, Brush newBrush) {
			RaiseBrushChanged(CellLightProperty, oldBrush, newBrush);
			lastSettedCellLight = newBrush;
		}
		protected virtual void OnCellLightBorderChanged(Brush oldBrush, Brush newBrush) {
			RaiseBrushChanged(CellLightBorderProperty, oldBrush, newBrush);
			lastSettedCellLightBorder = newBrush;
		}
		protected virtual void OnCellLightBorderDarkChanged(Brush oldBrush, Brush newBrush) {
			RaiseBrushChanged(CellLightBorderDarkProperty, oldBrush, newBrush);
			lastSettedCellLightBorderDark = newBrush;
		}
		protected virtual void OnCellBorderChanged(Brush oldBrush, Brush newBrush) {
			RaiseBrushChanged(CellBorderProperty, oldBrush, newBrush);
			lastSettedCellBorder = newBrush;
		}
		protected virtual void OnCellBorderDarkChanged(Brush oldBrush, Brush newBrush) {
			RaiseBrushChanged(CellBorderDarkProperty, oldBrush, newBrush);
			lastSettedCellBorderDark = newBrush;
		}
		public bool CopyFrom(ResourceBrushes source, object resourceId) {
			return CopyFromCore(source, resourceId);
		}
		Brush lastSettedResourceHeader;
		Brush lastSettedCell;
		Brush lastSettedCellBorder;
		Brush lastSettedCellBorderDark;
		Brush lastSettedCellLight;
		Brush lastSettedCellLightBorder;
		Brush lastSettedCellLightBorderDark;
		protected virtual bool CopyFromCore(ResourceBrushes source, object resourceId) {
			bool wasChanged = false;
			Brush sourceResourceBrush = GetSourceResourceBrush(source, resourceId);
			if (lastSettedResourceHeader != sourceResourceBrush) {
				ResourceHeader = sourceResourceBrush;
				wasChanged = true;
			}
			if (lastSettedCell != source.Cell) {
				Cell = source.Cell;
				wasChanged = true;
			}
			if (lastSettedCellBorder != source.CellBorder) {
				CellBorder = source.CellBorder;
				wasChanged = true;
			}
			if (lastSettedCellBorderDark != source.CellBorderDark) {
				CellBorderDark = source.CellBorderDark;
				wasChanged = true;
			}
			if (lastSettedCellLight != source.CellLight) {
				CellLight = source.CellLight;
				wasChanged = true;
			}
			if (lastSettedCellLightBorder != source.CellLightBorder) {
				CellLightBorder = source.CellLightBorder;
				wasChanged = true;
			}
			if (lastSettedCellLightBorderDark != source.CellLightBorderDark) {
				CellLightBorderDark = source.CellLightBorderDark;
				wasChanged = true;
			}
			return wasChanged;
		}
		protected virtual Brush GetSourceResourceBrush(ResourceBrushes source, object resourceId) {
			if (resourceId == EmptyResourceId.Id)
				return null;
			else
				return source.Cell;
		}
	}
}
