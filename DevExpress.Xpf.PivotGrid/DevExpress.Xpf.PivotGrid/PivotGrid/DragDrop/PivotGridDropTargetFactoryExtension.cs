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
using System.Windows;
using DevExpress.XtraPivotGrid.Data;
using System.Windows.Controls;
using DevExpress.XtraPivotGrid;
using System.Collections.Generic;
using System.Windows.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using System.Windows.Markup;
using System.Collections;
using DevExpress.Xpf.PivotGrid.Internal;
using System.Windows.Media;
using DevExpress.Xpf.Utils;
using System.Windows.Controls.Primitives;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class PivotGridDropTargetFactoryExtension : MarkupExtension, IDropTargetFactory {
		public PivotGridDropTargetFactoryExtension() { }
		IDropTarget IDropTargetFactory.CreateDropTarget(UIElement dropTargetElement) {
			FrameworkElement presenter = dropTargetElement as FrameworkElement;
			if(presenter != null && presenter.Name == CellsAreaPresenter.DragBorderName)
				return CreateCellsDropTargetCore((CellsAreaPresenter)LayoutHelper.FindElement(presenter, (d) => d is CellsAreaPresenter));
			Panel panel = dropTargetElement as Panel;
			if(panel != null)
				return CreateDropTargetCore(panel);
			FieldListControlBase fieldList = dropTargetElement as FieldListControlBase;
			if(fieldList != null)
				return CreateFieldListDropTargetCore(fieldList);
			InnerFieldListControl innerFieldList = dropTargetElement as InnerFieldListControl;
			if(innerFieldList != null)
				return CreateInnerFieldListDropTargetCore(innerFieldList);
			PivotGridControl pivot = dropTargetElement as PivotGridControl;
			if(pivot != null)
				return CreateFieldListDropTargetCore(pivot);
			FieldHeaders headers = dropTargetElement as FieldHeaders;
			if(headers != null)
				return CreateHeadersDropTargetCore(headers);
			DataAreaPopupEdit edit = dropTargetElement as DataAreaPopupEdit;
			if(edit != null)
				return CreateDataAreaHeadersDropTargetCore(edit);
			throw new ArgumentException("Cannot create a drop target for " + dropTargetElement.GetType().Name);
		}
		protected virtual PivotGridDropTargetBase CreateDataAreaHeadersDropTargetCore(DataAreaPopupEdit edit) {
			return DataAreaDropTarget.Create(edit);
		}
		protected virtual IDropTarget CreateCellsDropTargetCore(CellsAreaPresenter presenter) {
			return presenter.Data.DataFieldCount <= 1 || presenter.Data.DataFieldArea != PivotDataArea.RowArea ? new CellsDropTarget(presenter) : new VerticalCellsDropTarget(presenter);
		}
		protected virtual PivotGridDropTargetBase CreateDropTargetCore(Panel panel) {
			return new PivotGridDropTarget(panel);
		}
		protected virtual PivotGridDropTargetBase CreateFieldListDropTargetCore(Control fieldList) {
			PivotFieldListControl list = fieldList as PivotFieldListControl;
			InnerFieldListControl innerList = null;
			if(list != null)
				innerList = (InnerFieldListControl)LayoutHelper.FindElement(list, (d) => d is InnerFieldListControl);
			if(innerList != null)
				return CreateInnerFieldListDropTargetCore(innerList);
			return new PivotEmptyDropTarget(fieldList);
		}
		protected virtual PivotGridDropTargetBase CreateInnerFieldListDropTargetCore(InnerFieldListControl innerFieldList) {
			return new InnerFieldListDropTarget(innerFieldList);
		}
		protected virtual PivotGridDropTargetBase CreateHeadersDropTargetCore(FieldHeaders headers) {
			return new PivotHeadersDropTarget(headers);
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class PivotDropTargetMouseOverEventArgs : EventArgs {
		bool isMouseOver;
		public PivotDropTargetMouseOverEventArgs(bool isMouseOver) {
			this.isMouseOver = isMouseOver;
		}
		public bool IsMouseOver { get { return isMouseOver; } }
	}
	public delegate void PivotDropTargetMouseOverEventHandler(object sender, PivotDropTargetMouseOverEventArgs e);
}
