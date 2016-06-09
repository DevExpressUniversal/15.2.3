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
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Core;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
#if !SILVERLIGHT
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public interface IFocusedRowBorderObject {
		FrameworkElement RowDataContent { get; }
		double LeftIndent { get; }
	}
	public class GridDataContentPresenter : DataContentPresenter {
#if SL
		static readonly ControlTemplate DefaultTemplate = (ControlTemplate)XamlHelper.LoadObjectCore("<ControlTemplate xmlns=\"http://schemas.microsoft.com/client/2007\" TargetType=\"ContentControl\"><ContentPresenter /></ControlTemplate>");
#endif
		public GridDataContentPresenter() {
#if SL
			Template = DefaultTemplate;
#endif
		}
	}
	public abstract class CellItemsControlBase : CachedItemsControl, INotifyCurrentViewChanged, ILayoutNotificationHelperOwner {
		LayoutNotificationHelper layoutNotificationHelper;
		public CellItemsControlBase() {
			layoutNotificationHelper = new LayoutNotificationHelper(this);
		}
		protected DataViewBase View { get { return (DataViewBase)DataControlBase.GetCurrentView(this); } }
		protected override Size MeasureOverride(Size constraint) {
			layoutNotificationHelper.Subscribe();
			return base.MeasureOverride(constraint);
		}
#if DEBUGTEST
		internal int ValidateElementCallCountForTests { get; private set; }
#endif
		protected sealed override void ValidateElement(FrameworkElement element, object item) {
#if DEBUGTEST
			ValidateElementCallCountForTests++;
#endif
			GridCellData cellData = ((GridCellData)item);
			if(cellData.Column != null)
				ValidateElementCore(element, cellData);
		}
		protected abstract void ValidateElementCore(FrameworkElement element, GridCellData cellData);
		protected sealed override FrameworkElement CreateChild(object item) {
			return CreateChildCore((GridCellData)item);
		}
		protected abstract FrameworkElement CreateChildCore(GridCellData cellData);
		#region INotifyCurrentViewChanged Members
		void INotifyCurrentViewChanged.OnCurrentViewChanged(DependencyObject d) {
			OnCurrentViewChanged();
		}
		protected virtual void OnCurrentViewChanged() {
			InvalidateMeasure();
		}
		#endregion
		DependencyObject ILayoutNotificationHelperOwner.NotificationManager {
			get { return View != null ? View.DataControl : null; }
		}
		protected override void OnCollectionChanged() {
			base.OnCollectionChanged();
			DoInvalidateMeasure();
		}
		void DoInvalidateMeasure() {
			if(View == null) return;
			UIElement parent = this;
			do {
				parent = VisualTreeHelper.GetParent(parent) as UIElement;
				if(parent == null || parent == View.DataPresenter) return;
				parent.InvalidateMeasure();
			} while(true);
		}
	}
}
