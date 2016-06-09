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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid.Native;
namespace DevExpress.Xpf.Grid {
	public abstract class GridDropTargetFactoryBase : IDropTargetFactory {
		IDropTarget IDropTargetFactory.CreateDropTarget(UIElement dropTargetElement) {
			return CreateDropTarget(dropTargetElement);
		}
		public bool IsCompatibleDropTargetFactory(UIElement dropTargetElement, BaseGridHeader sourceHeader) {
			return IsCompatibleDataControl(DataControlBase.FindCurrentView(dropTargetElement).DataControl, sourceHeader.GridView.DataControl);
		}
		protected virtual bool IsCompatibleDataControl(DataControlBase sourceDataControl, DataControlBase targetDataControl) {
			return sourceDataControl.GetOriginationDataControl() == targetDataControl.GetOriginationDataControl();
		}
		protected abstract IDropTarget CreateDropTarget(UIElement dropTargetElement);
	}
	public class BandChooserDropTarget : ColumnChooserDropTarget {
	}
	public class ColumnChooserDropTarget : IDropTarget {
		internal static void DropColumnCore(UIElement source) {
			if(source is ColumnChooserControlBase) return;
			if(IsForbiddenUngroupGesture(source)) return;
			BaseColumn column = GetColumn(source);
			if(column == null) return;
			HeaderPresenterType sourceType = BaseGridColumnHeader.GetHeaderPresenterTypeFromGridColumnHeader(source);
			column.View.BeforeMoveColumnToChooser(column, sourceType);
			if(!column.View.IsLastVisibleColumn(column) || column.View.DataControl.BandsLayoutCore != null)
				column.Visible = false;
		}
		internal static bool IsForbiddenUngroupGesture(UIElement source) {
			BaseColumn column = ColumnChooserDropTarget.GetColumn(source);
			if(column != null && !column.ActualAllowGroupingCore) {
				HeaderPresenterType sourceType = BaseGridColumnHeader.GetHeaderPresenterTypeFromGridColumnHeader(source);
				if(sourceType == HeaderPresenterType.GroupPanel)
					return true;
			}
			return false;
		}
		static BaseColumn GetColumn(UIElement source) {
			DataViewBase view = DataControlBase.FindCurrentView(source);
			BaseColumn column = DropTargetHelper.GetColumnFromDragSource((FrameworkElement)source);
#if DEBUGTEST
			System.Diagnostics.Debug.Assert(column != null);
#endif
			if(view == null) return null;
			return column;
		}
		internal void DropColumn(UIElement source) {
			DropColumnCore(source);
		}
		public ColumnChooserDropTarget() {
		}
		#region IDropTarget Members
		public void Drop(UIElement source, Point pt) {
			DropColumn(source);
		}
		public void OnDragLeave() {
		}
		public void OnDragOver(UIElement source, Point pt) {
		}
		#endregion
	}
	public class ColumnChooserDropTargetFactory : GridDropTargetFactoryBase {
		protected sealed override IDropTarget CreateDropTarget(UIElement dropTargetElement) {
			return new ColumnChooserDropTarget();
		}
	}
	public class BandChooserDropTargetFactory : GridDropTargetFactoryBase {
		protected sealed override IDropTarget CreateDropTarget(UIElement dropTargetElement) {
			return new BandChooserDropTarget();
		}
	}
}
