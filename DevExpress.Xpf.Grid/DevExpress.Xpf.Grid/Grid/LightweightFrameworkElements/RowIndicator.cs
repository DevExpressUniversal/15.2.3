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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.Editors.Validation;
namespace DevExpress.Xpf.Grid {
	public class RowIndicator : RowIndicatorBase {
		#region template properties
		public DataTemplate NoneContentTemplate {
			get { return (DataTemplate)GetValue(NoneContentTemplateProperty); }
			set { SetValue(NoneContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty NoneContentTemplateProperty =
			DependencyProperty.Register("NoneContentTemplate", typeof(DataTemplate), typeof(RowIndicator), new PropertyMetadata(null, (d, _) => ((RowIndicator)d).OnContentTemplateChanged()));
		public DataTemplate FocusedContentTemplate {
			get { return (DataTemplate)GetValue(FocusedContentTemplateProperty); }
			set { SetValue(FocusedContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty FocusedContentTemplateProperty =
			DependencyProperty.Register("FocusedContentTemplate", typeof(DataTemplate), typeof(RowIndicator), new PropertyMetadata(null, (d, _) => ((RowIndicator)d).OnContentTemplateChanged()));
		public DataTemplate ChangedContentTemplate {
			get { return (DataTemplate)GetValue(ChangedContentTemplateProperty); }
			set { SetValue(ChangedContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty ChangedContentTemplateProperty =
			DependencyProperty.Register("ChangedContentTemplate", typeof(DataTemplate), typeof(RowIndicator), new PropertyMetadata(null, (d, _) => ((RowIndicator)d).OnContentTemplateChanged()));
		public DataTemplate NewItemRowContentTemplate {
			get { return (DataTemplate)GetValue(NewItemRowContentTemplateProperty); }
			set { SetValue(NewItemRowContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty NewItemRowContentTemplateProperty =
			DependencyProperty.Register("NewItemRowContentTemplate", typeof(DataTemplate), typeof(RowIndicator), new PropertyMetadata(null, (d, _) => ((RowIndicator)d).OnContentTemplateChanged()));
		public DataTemplate EditingContentTemplate {
			get { return (DataTemplate)GetValue(EditingContentTemplateProperty); }
			set { SetValue(EditingContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty EditingContentTemplateProperty =
			DependencyProperty.Register("EditingContentTemplate", typeof(DataTemplate), typeof(RowIndicator), new PropertyMetadata(null, (d, _) => ((RowIndicator)d).OnContentTemplateChanged()));
		public DataTemplate ErrorContentTemplate {
			get { return (DataTemplate)GetValue(ErrorContentTemplateProperty); }
			set { SetValue(ErrorContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty ErrorContentTemplateProperty =
			DependencyProperty.Register("ErrorContentTemplate", typeof(DataTemplate), typeof(RowIndicator), new PropertyMetadata(null, (d, _) => ((RowIndicator)d).OnContentTemplateChanged()));
		public DataTemplate FocusedErrorContentTemplate {
			get { return (DataTemplate)GetValue(FocusedErrorContentTemplateProperty); }
			set { SetValue(FocusedErrorContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty FocusedErrorContentTemplateProperty =
			DependencyProperty.Register("FocusedErrorContentTemplate", typeof(DataTemplate), typeof(RowIndicator), new PropertyMetadata(null, (d, _) => ((RowIndicator)d).OnContentTemplateChanged()));
		public DataTemplate AutoFilterRowContentTemplate {
			get { return (DataTemplate)GetValue(AutoFilterRowContentTemplateProperty); }
			set { SetValue(AutoFilterRowContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty AutoFilterRowContentTemplateProperty =
			DependencyProperty.Register("AutoFilterRowContentTemplate", typeof(DataTemplate), typeof(RowIndicator), new PropertyMetadata(null, (d, _) => ((RowIndicator)d).OnContentTemplateChanged()));
		#endregion
		public IndicatorState IndicatorState {
			get { return (IndicatorState)GetValue(IndicatorStateProperty); }
			set { SetValue(IndicatorStateProperty, value); }
		}
		public static readonly DependencyProperty IndicatorStateProperty =
			DependencyProperty.Register("IndicatorState", typeof(IndicatorState), typeof(RowIndicator), new PropertyMetadata(IndicatorState.None, (d, _) => ((RowIndicator)d).OnIndicatorStateChanged()));
		public bool ShowRowBreak {
			get { return (bool)GetValue(ShowRowBreakProperty); }
			set { SetValue(ShowRowBreakProperty, value); }
		}
		public static readonly DependencyProperty ShowRowBreakProperty =
			DependencyProperty.Register("ShowRowBreak", typeof(bool), typeof(RowIndicator), new PropertyMetadata(false));
		static RowIndicator() {
			Type ownerType = typeof(RowIndicator);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
		void OnIndicatorStateChanged() {
			UpdateContent();
		}
		void OnContentTemplateChanged() {
			UpdateContent();
		}
		internal Border ContentBorder { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ContentBorder = GetTemplateChild("PART_ContentBorder") as Border;
#if DEBUGTEST
			DevExpress.Xpf.Grid.Tests.GridTestHelper.SetSkipChildrenWhenStoreVisualTree(ContentBorder, true);
#endif
			UpdateContent();
		}
		DataTemplate contentTemplate;
		ContentPresenter contentPresenter;
		internal void UpdateContent() {
			if(TableView == null)
				return;
			var newContentTemplate = GetContentTemplate();
			if(ContentBorder != null && newContentTemplate != contentTemplate) {
				contentTemplate = newContentTemplate;
				if(contentTemplate != null && contentPresenter == null) {
					contentPresenter = new ContentPresenter();
				}
				if(contentPresenter != null) {
					contentPresenter.Content = null;
					contentPresenter.ContentTemplate = contentTemplate;
					if(contentTemplate != null)
						contentPresenter.Content = RowData;
				}
				ContentBorder.Child = contentPresenter;
			}
		}
		RowDataBase RowData { get { return DataContext as RowDataBase; } }
		ITableView TableView { get { return RowData != null ? RowData.View as ITableView : null; } }
		internal DataTemplate GetContentTemplate() {
			if(TableView.RowIndicatorContentTemplate != null && !(TableView.RowIndicatorContentTemplate is DefaultDataTemplate))
				return TableView.RowIndicatorContentTemplate;
			switch(IndicatorState) {
				case IndicatorState.AutoFilterRow:
					return AutoFilterRowContentTemplate;
				case IndicatorState.Changed:
					return ChangedContentTemplate;
				case IndicatorState.Editing:
					return EditingContentTemplate;
				case IndicatorState.Error:
					return ErrorContentTemplate;
				case IndicatorState.Focused:
					return FocusedContentTemplate;
				case IndicatorState.FocusedError:
					return FocusedErrorContentTemplate;
				case IndicatorState.NewItemRow:
					return NewItemRowContentTemplate;
				default:
					return NoneContentTemplate;;
			}
		}
	}
	public class GroupRowIndicator : RowIndicator {
		static GroupRowIndicator() {
			Type ownerType = typeof(GroupRowIndicator);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
	}
}
