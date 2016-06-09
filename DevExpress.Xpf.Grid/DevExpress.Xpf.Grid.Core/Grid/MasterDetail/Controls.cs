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
using System.Windows.Data;
using System.Collections.Generic;
using DevExpress.Xpf.Grid.Native;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using DevExpress.Xpf.Grid.Hierarchy;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class DetailRowControlBase : ContentControl {
		public RowData MasterRowData {
			get { return (RowData)GetValue(MasterRowDataProperty); }
			set { SetValue(MasterRowDataProperty, value); }
		}
		public static readonly DependencyProperty MasterRowDataProperty =
			DependencyPropertyManager.Register("MasterRowData", typeof(RowData), typeof(DetailRowControlBase), new PropertyMetadata(null));
	}
	public class DetailRowContentPresenter : ContentPresenter {
		public RowData MasterRowData {
			get { return (RowData)GetValue(MasterRowDataProperty); }
			set { SetValue(MasterRowDataProperty, value); }
		}
		public static readonly DependencyProperty MasterRowDataProperty =
			DependencyPropertyManager.Register("MasterRowData", typeof(RowData), typeof(DetailRowContentPresenter), new PropertyMetadata(null));
	}
	public class EmptyDetailRowControl : DetailRowControlBase {
		public EmptyDetailRowControl() {
			Height = 0;
		}
	}
	public abstract class DetailHeaderControlBase : DetailRowControlBase {
		public DetailDescriptorBase DetailDescriptor {
			get { return (DetailDescriptorBase)GetValue(DetailDescriptorProperty); }
			set { SetValue(DetailDescriptorProperty, value); }
		}
		public static readonly DependencyProperty DetailDescriptorProperty =
			DependencyPropertyManager.Register("DetailDescriptor", typeof(DetailDescriptorBase), typeof(DetailHeaderControlBase), new PropertyMetadata(null));
		public bool ShowBottomLine {
			get { return (bool)GetValue(ShowBottomLineProperty); }
			set { SetValue(ShowBottomLineProperty, value); }
		}
		public static readonly DependencyProperty ShowBottomLineProperty =
			DependencyPropertyManager.Register("ShowBottomLine", typeof(bool), typeof(DetailHeaderControlBase), new PropertyMetadata(false));
	}
	public abstract class DetailTabHeadersControlBase : DetailHeaderControlBase {
	}
	public abstract class DetailControlPartContainer : Control {
		public DetailDescriptorBase DetailDescriptor {
			get { return (DetailDescriptorBase)GetValue(DetailDescriptorProperty); }
			set { SetValue(DetailDescriptorProperty, value); }
		}
		public static readonly DependencyProperty DetailDescriptorProperty =
			DependencyPropertyManager.Register("DetailDescriptor", typeof(DetailDescriptorBase), typeof(DetailControlPartContainer), new PropertyMetadata(null, (d, e) => ((DetailControlPartContainer)d).OnDetailDescriptorChanged()));
		public DataViewBase View {
			get { return (DataViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public static readonly DependencyProperty ViewProperty =
			DependencyPropertyManager.Register("View", typeof(DataViewBase), typeof(DetailControlPartContainer), new PropertyMetadata(null, (d, e) => ((DetailControlPartContainer)d).OnViewChanged()));
		public ControlTemplate DetailPartTemplate {
			get { return (ControlTemplate)GetValue(DetailPartTemplateProperty); }
			set { SetValue(DetailPartTemplateProperty, value); }
		}
		public static readonly DependencyProperty DetailPartTemplateProperty =
			DependencyPropertyManager.Register("DetailPartTemplate", typeof(ControlTemplate), typeof(DetailControlPartContainer), new PropertyMetadata(null, (d, e) => ((DetailControlPartContainer)d).UpdateTemplate()));
		void OnDetailDescriptorChanged() {
			if(DetailDescriptor != null)
				SetBinding(ViewProperty, new Binding(DataControlDetailDescriptor.DataControlProperty.GetName() + ".View") { Source = DetailDescriptor });
			else
				ClearValue(ViewProperty);
		}
		ItemsControl nextLevelItemsControl;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			nextLevelItemsControl = GetTemplateChild("PART_NextLevelItemsControl") as ItemsControl;
			BindNextLevelItemsControl();
		 }
		void BindNextLevelItemsControl() {
			if(nextLevelItemsControl != null && View != null) {
				View.BindDetailContainerNextLevelItemsControl(nextLevelItemsControl);
			}
		}
		void OnViewChanged() {
			DataControlBase.SetCurrentView(this, View);
			UpdateTemplate();
			BindNextLevelItemsControl();
		}
		void UpdateTemplate() {
			Template = View != null ? DetailPartTemplate : null;
		}
		internal DependencyObject GetTemplateChildInternal(string name) {
			return GetTemplateChild(name);
		}
	}
}
