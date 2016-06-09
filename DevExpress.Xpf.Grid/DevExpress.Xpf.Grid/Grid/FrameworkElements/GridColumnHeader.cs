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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Data;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using Thumb = DevExpress.Xpf.Core.DXThumb;
using DevExpress.Xpf.Grid.HitTest;
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class GridColumnHeader : BaseGridColumnHeader {
		public GridColumnHeader() {
			this.SetDefaultStyleKey(typeof(GridColumnHeader));
		}
#if !SL
		protected override FrameworkElement CreateSortIndicator() {
			return new SortIndicatorControl();
		}
		protected override void UpdateSortIndicator(bool isAscending) {
			((SortIndicatorControl)SortIndicator).SortOrder = isAscending ? ListSortDirection.Ascending : ListSortDirection.Descending;
		}
		protected override Thumb CreateGripper() {
			return new GridThumb();
		}
		protected override XPFContentControl CreateCustomHeaderPresenter() {
			return new HeaderContentControl();
		}
		protected override FrameworkElement CreateDesignTimeSelectionControl() {
			return new DesignTimeSelectionControl() { IsTabStop = false };
		}
		Lazy<DataViewHitTestAcceptorBase> defaultFilterHitTestAcceptor = new Lazy<DataViewHitTestAcceptorBase>(() => new ColumnHeaderFilterButtonTableViewHitTestAcceptor());
		Lazy<DataViewHitTestAcceptorBase> groupPanelFilterHitTestAcceptor = new Lazy<DataViewHitTestAcceptorBase>(() => new GroupPanelColumnHeaderFilterButtonTableViewHitTestAcceptor());
		protected override void SetFilterHitTestAcceptor(PopupBaseEdit popup) {
			GridViewHitInfoBase.SetHitTestAcceptor(popup, HeaderPresenterType == Grid.HeaderPresenterType.GroupPanel ? groupPanelFilterHitTestAcceptor.Value : defaultFilterHitTestAcceptor.Value);
		}
#endif
	}
	public class FitColumnHeader : GridColumnHeaderBase {
		public FitColumnHeader() {
			this.SetDefaultStyleKey(typeof(FitColumnHeader));
		}
	}
	public class IndicatorColumnHeader : GridColumnHeaderBase {
		public IndicatorColumnHeader() {
			this.SetDefaultStyleKey(typeof(IndicatorColumnHeader));
		}
	}
	public class ExpandButtonColumnHeader : GridColumnHeaderBase {
		public ExpandButtonColumnHeader() {
			this.SetDefaultStyleKey(typeof(ExpandButtonColumnHeader));
		}
	}
	public class DesignTimeSelectionControl : Control {
		public DesignTimeSelectionControl() {
			this.SetDefaultStyleKey(typeof(DesignTimeSelectionControl));
		}
	}
	public class HeaderContentControl : XPFContentControl {
		public HeaderContentControl() {
			this.SetDefaultStyleKey(typeof(HeaderContentControl));
		}
	}
	public class ColumnHeaderPanel : Control {
		public ColumnHeaderPanel() {
			this.SetDefaultStyleKey(typeof(ColumnHeaderPanel));
		}
	}
#if !SL
	public class SortIndicatorControl : Control {
		public static readonly DependencyProperty SortOrderProperty;
		static SortIndicatorControl() {
			Type ownerType = typeof(SortIndicatorControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			SortOrderProperty = DependencyProperty.Register("SortOrder", typeof(ListSortDirection), ownerType, new PropertyMetadata(ListSortDirection.Ascending));
		}
		public ListSortDirection SortOrder {
			get { return (ListSortDirection)GetValue(SortOrderProperty); }
			set { SetValue(SortOrderProperty, value); }
		}
	}
#endif
}
