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
using System.Windows.Input;
using DevExpress.Xpf.DocumentViewer.Extensions;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.DocumentViewer {
	public class SearchControlContainer : Control {
		public static readonly DependencyProperty SearchParameterProperty;
		public static readonly DependencyProperty WholeWordProperty;
		public static readonly DependencyProperty IsCaseSensitiveProperty;
		public static readonly DependencyProperty SearchTextProperty;
		public static readonly DependencyProperty IsSearchControlVisibleProperty;
		public static readonly DependencyProperty ActualSearchContainerProperty;
		static SearchControlContainer() {
			Type ownerType = typeof(SearchControlContainer);
			IsSearchControlVisibleProperty = DependencyPropertyManager.Register("IsSearchControlVisible", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((SearchControlContainer)d).IsSearchControlVisibleChanged((bool)e.NewValue)));
			SearchParameterProperty = DependencyPropertyManager.Register("SearchParameter", typeof(TextSearchParameter), ownerType, new FrameworkPropertyMetadata(null));
			WholeWordProperty = DependencyPropertyManager.Register("WholeWord", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (o, args) => ((SearchControlContainer)o).SearchPropertyChanged()));
			IsCaseSensitiveProperty = DependencyPropertyManager.Register("IsCaseSensitive", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (o, args) => ((SearchControlContainer)o).SearchPropertyChanged()));
			SearchTextProperty = DependencyPropertyManager.Register("SearchText", typeof(string), ownerType,
				new FrameworkPropertyMetadata(string.Empty, (o, args) => ((SearchControlContainer)o).SearchPropertyChanged()));
			ActualSearchContainerProperty = DependencyPropertyManager.RegisterAttached("ActualSearchContainer", typeof(SearchControlContainer), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		}
		public static SearchControlContainer GetActualSearchContainer(DependencyObject d) {
			return (SearchControlContainer)d.GetValue(ActualSearchContainerProperty);
		}
		public static void SetActualSearchContainer(DependencyObject d, SearchControlContainer value) {
			d.SetValue(ActualSearchContainerProperty, value);
		}
		public ICommand FindNextTextCommand { get; private set; }
		public ICommand FindPreviousTextCommand { get; private set; }
		public ICommand CloseCommand { get; private set; }
		public bool IsSearchControlVisible {
			get { return (bool)GetValue(IsSearchControlVisibleProperty); }
			set { SetValue(IsSearchControlVisibleProperty, value); }
		}
		public string SearchText {
			get { return (string)GetValue(SearchTextProperty); }
			set { SetValue(SearchTextProperty, value); }
		}
		public bool WholeWord {
			get { return (bool)GetValue(WholeWordProperty); }
			set { SetValue(WholeWordProperty, value); }
		}
		public bool IsCaseSensitive {
			get { return (bool)GetValue(IsCaseSensitiveProperty); }
			set { SetValue(IsCaseSensitiveProperty, value); }
		}
		public TextSearchParameter SearchParameter {
			get { return (TextSearchParameter)GetValue(SearchParameterProperty); }
			set { SetValue(SearchParameterProperty, value); }
		}
		DocumentViewerControl ActualViewer { get { return DocumentViewerControl.GetActualViewer(this) as DocumentViewerControl; } }
		CommandProvider CommandProvider { get { return ActualViewer.With(x => x.ActualCommandProvider) as CommandProvider; } }
		public SearchControlContainer() {
			SetActualSearchContainer(this, this);
			DefaultStyleKeyHelper.SetDefaultStyleKey(this, typeof(SearchControlContainer));
			FindNextTextCommand = DelegateCommandFactory.Create(FindNextText, CanFindText);
			FindPreviousTextCommand = DelegateCommandFactory.Create(FindPreviousText, CanFindText);
			CloseCommand = DelegateCommandFactory.Create(Close);
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			SearchPropertyChanged();
		}
		bool CanFindText() {
			return SearchParameter.Return(x => !string.IsNullOrWhiteSpace(x.Text), () => false);
		}
		void FindPreviousText() {
			if (SearchParameter == null)
				return;
			SearchParameter.SearchDirection = TextSearchDirection.Backward;
			CommandProvider.Do(x => x.FindNextTextCommand.TryExecute(SearchParameter));
		}
		void Close() {
			CommandProvider.ShowFindTextCommand.TryExecute(false);
		}
		void FindNextText() {
			if (SearchParameter == null)
				return;
			SearchParameter.SearchDirection = TextSearchDirection.Forward;
			CommandProvider.Do(x => x.FindNextTextCommand.TryExecute(SearchParameter));
		}
		protected virtual void SearchPropertyChanged() {
			if (SearchParameter == null || !IsLoaded)
				return;
			SearchParameter.WholeWord = WholeWord;
			SearchParameter.IsCaseSensitive = IsCaseSensitive;
			SearchParameter.Text = SearchText;
		}
		protected virtual void IsSearchControlVisibleChanged(bool newValue) {
		}
	}
}
