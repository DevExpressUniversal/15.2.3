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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Editors.DataPager {
	public enum DataPagerDisplayMode { FirstLast, FirstLastNumeric, FirstLastPreviousNext, FirstLastPreviousNextNumeric, Numeric, PreviousNextNumeric, PreviousNext }
	public enum DataPagerButtonType { PageFirst, PageLast, PageNext, PagePrevious, PageNumeric };
	public class DataPagerPageIndexChangedEventArgs : EventArgs {
		public DataPagerPageIndexChangedEventArgs(int oldValue, int newValue) {
			NewValue = newValue;
			OldValue = oldValue;
		}
		public int OldValue { get; private set; }
		public int NewValue { get; private set; }
	}
	public class DataPagerPageIndexChangingEventArgs : DataPagerPageIndexChangedEventArgs {
		public DataPagerPageIndexChangingEventArgs(int oldValue, int newValue) : base(oldValue, newValue) { }
		public bool IsCancel { get; set; }
	}
#if !SL
	public interface IPagedCollectionView {
		bool CanChangePage { get; }
		int ItemCount { get; }
		int PageIndex { get; }
		int PageSize { get; set; }
		int TotalItemCount { get; }
		bool MoveToFirstPage();
		bool MoveToPage(int pageIndex);
	}
	public class PagedCollectionView : List<object>, IPagedCollectionView {
		public PagedCollectionView() { }
		public PagedCollectionView(IEnumerable source){ }
		public bool CanChangePage {
			get { return true; }
		}
		public int ItemCount { get { return 25; } }
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
		public int TotalItemCount {
			get { return 100; }
		}
		public bool MoveToFirstPage() {
			PageIndex = 0;
			return true;
		}
		public bool MoveToPage(int pageIndex) {
			PageIndex = pageIndex;
			return true;
		}
		public void Refresh() {
		}
	}
#endif
	public struct DataPagerCurrentPageParams {
		public int PageCount;
		public int PageIndex;
	}
#if SL
	[DXToolboxBrowsable(true)]
#endif
	public class DataPager : Control {
		public static readonly DependencyProperty ActualNumericButtonCountProperty;
		public static readonly DependencyProperty ActualPageIndexProperty;
		public static readonly DependencyProperty ActualPageSizeProperty;
		public static readonly DependencyProperty AutoEllipsisProperty;
		public static readonly DependencyProperty CanChangePageProperty;
		public static readonly DependencyProperty CanMoveToFirstPageProperty;
		public static readonly DependencyProperty CanMoveToLastPageProperty;
		public static readonly DependencyProperty CanMoveToNextPageProperty;
		public static readonly DependencyProperty CanMoveToPreviousPageProperty;
		public static readonly DependencyProperty ContainerFirstButtonPageNumberProperty;
		public static readonly DependencyProperty ContainerSecondButtonPageNumberProperty;
		public static readonly DependencyProperty CurrentPageParamsProperty;
		public static readonly DependencyProperty DataPagerProperty;
		public static readonly DependencyProperty DisplayModeProperty;
		public static readonly DependencyProperty IsAutoNumericButtonCountProperty;
		public static readonly DependencyProperty IsTotalItemCountFixedProperty;
		public static readonly DependencyProperty ItemCountProperty;
		public static readonly DependencyProperty NumericButtonCountProperty;
		public static readonly DependencyProperty PageCountProperty;
		public static readonly DependencyProperty PageIndexProperty;
		public static readonly DependencyProperty PagedSourceProperty;
		public static readonly DependencyProperty PageSizeProperty;
		public static readonly DependencyProperty SourceProperty;
		public static readonly DependencyProperty ShowTotalPageCountProperty;
		static readonly DependencyPropertyKey ActualNumericButtonCountPropertyKey;
		static readonly DependencyPropertyKey ActualPageIndexPropertyKey;
		static readonly DependencyPropertyKey ActualPageSizePropertyKey;
		static readonly DependencyPropertyKey CanChangePagePropertyKey;
		static readonly DependencyPropertyKey CanMoveToFirstPagePropertyKey;
		static readonly DependencyPropertyKey CanMoveToLastPagePropertyKey;
		static readonly DependencyPropertyKey CanMoveToNextPagePropertyKey;
		static readonly DependencyPropertyKey CanMoveToPreviousPagePropertyKey;
		static readonly DependencyPropertyKey ContainerFirstButtonPageNumberPropertyKey;
		static readonly DependencyPropertyKey ContainerSecondButtonPageNumberPropertyKey;
		static readonly DependencyPropertyKey IsAutoNumericButtonCountPropertyKey;
		static readonly DependencyPropertyKey PageCountPropertyKey;
		static readonly DependencyPropertyKey PagedSourcePropertyKey;
		static DataPager() {
			Type ownerType = typeof(DataPager);
			ActualNumericButtonCountPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualNumericButtonCount", typeof(int), ownerType,
				new PropertyMetadata(0, (d, e) => ((DataPager)d).OnActualNumericButtonCountChanged((int)e.NewValue)));
			ActualNumericButtonCountProperty = ActualNumericButtonCountPropertyKey.DependencyProperty;
			ActualPageIndexPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPageIndex", typeof(int), ownerType,
				new PropertyMetadata(-1, (d, e) => ((DataPager)d).OnActualPageIndexChanged((int)e.OldValue, (int)e.NewValue)));
			ActualPageIndexProperty = ActualPageIndexPropertyKey.DependencyProperty;
			ActualPageSizePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPageSize", typeof(int), ownerType,
				new PropertyMetadata(0, (d, e) => ((DataPager)d).OnActualPageSizeChanged((int)e.NewValue)));
			ActualPageSizeProperty = ActualPageSizePropertyKey.DependencyProperty;
			AutoEllipsisProperty = DependencyPropertyManager.Register("AutoEllipsis", typeof(bool), ownerType,
			   new PropertyMetadata(true, (d, e) => ((DataPager)d).OnAutoEllipsisChanged((bool)e.NewValue)));
			CanChangePagePropertyKey = DependencyPropertyManager.RegisterReadOnly("CanChangePage", typeof(bool), ownerType,
				new PropertyMetadata(false, (d, e) => ((DataPager)d).OnCanChangePageChanged((bool)e.NewValue)));
			CanChangePageProperty = CanChangePagePropertyKey.DependencyProperty;
			CanMoveToFirstPagePropertyKey = DependencyPropertyManager.RegisterReadOnly("CanMoveToFirstPage", typeof(bool), ownerType,
				new PropertyMetadata(false, (d, e) => ((DataPager)d).OnCanMoveToFirstPageChanged((bool)e.NewValue)));
			CanMoveToFirstPageProperty = CanMoveToFirstPagePropertyKey.DependencyProperty;
			CanMoveToLastPagePropertyKey = DependencyPropertyManager.RegisterReadOnly("CanMoveToLastPage", typeof(bool), ownerType,
				new PropertyMetadata(false, (d, e) => ((DataPager)d).OnCanMoveToLastPageChanged((bool)e.NewValue)));
			CanMoveToLastPageProperty = CanMoveToLastPagePropertyKey.DependencyProperty;
			CanMoveToNextPagePropertyKey = DependencyPropertyManager.RegisterReadOnly("CanMoveToNextPage", typeof(bool), ownerType,
				new PropertyMetadata(false, (d, e) => ((DataPager)d).OnCanMoveToNextPageChanged((bool)e.NewValue)));
			CanMoveToNextPageProperty = CanMoveToNextPagePropertyKey.DependencyProperty;
			CanMoveToPreviousPagePropertyKey = DependencyPropertyManager.RegisterReadOnly("CanMoveToPreviousPage", typeof(bool), ownerType,
				new PropertyMetadata(false, (d, e) => ((DataPager)d).OnCanMoveToPreviousPageChanged((bool)e.NewValue)));
			CanMoveToPreviousPageProperty = CanMoveToPreviousPagePropertyKey.DependencyProperty;
			ContainerFirstButtonPageNumberPropertyKey = DependencyPropertyManager.RegisterReadOnly("ContainerFirstButtonPageNumber", typeof(int), ownerType,
				new PropertyMetadata(1, (d, e) => ((DataPager)d).OnContainerFirstButtonPageNumberChanged((int)e.NewValue), (d, baseValue) => ((DataPager)d).OnContainerFirstButtonPageNumberCoerce(baseValue)));
			ContainerFirstButtonPageNumberProperty = ContainerFirstButtonPageNumberPropertyKey.DependencyProperty;
			ContainerSecondButtonPageNumberPropertyKey = DependencyPropertyManager.RegisterReadOnly("ContainerSecondButtonPageNumber", typeof(int), ownerType,
				new PropertyMetadata(2, (d, e) => ((DataPager)d).OnContainerSecondButtonPageNumberChanged((int)e.NewValue), (d, baseValue) => ((DataPager)d).OnContainerSecondButtonPageNumberCoerce(baseValue)));
			ContainerSecondButtonPageNumberProperty = ContainerSecondButtonPageNumberPropertyKey.DependencyProperty;
			CurrentPageParamsProperty = DependencyPropertyManager.Register("CurrentPageParams", typeof(DataPagerCurrentPageParams), ownerType,
				new PropertyMetadata((d, e) => ((DataPager)d).OnCurrentPageParamsChanged()));
			DataPagerProperty = DependencyPropertyManager.RegisterAttached("DataPager", typeof(DataPager), typeof(DataPager), new PropertyMetadata(null));
			DisplayModeProperty = DependencyPropertyManager.Register("DisplayMode", typeof(DataPagerDisplayMode), ownerType,
				new PropertyMetadata(DataPagerDisplayMode.FirstLastPreviousNextNumeric, (d, e) => ((DataPager)d).OnDisplayModeChanged((DataPagerDisplayMode)e.NewValue)));
			IsAutoNumericButtonCountPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsAutoNumericButtonCount", typeof(bool), ownerType, new PropertyMetadata(false));
			IsAutoNumericButtonCountProperty = IsAutoNumericButtonCountPropertyKey.DependencyProperty;
			IsTotalItemCountFixedProperty = DependencyPropertyManager.Register("IsTotalItemCountFixed", typeof(bool), ownerType,
				new PropertyMetadata(false, (d, e) => ((DataPager)d).OnIsTotalItemCountFixedChanged((bool)e.NewValue)));
			ItemCountProperty = DependencyPropertyManager.Register("ItemCount", typeof(int), ownerType,
				new PropertyMetadata((d, e) => ((DataPager)d).OnItemCountChanged((int)e.NewValue)));
			NumericButtonCountProperty = DependencyPropertyManager.Register("NumericButtonCount", typeof(int), ownerType,
				new PropertyMetadata(5, (d, e) => ((DataPager)d).OnNumericButtonCountChanged((int)e.OldValue, (int)e.NewValue), (d, baseValue) => ((DataPager)d).OnNumericButtonCountCoerce(baseValue)));
			PageCountPropertyKey = DependencyPropertyManager.RegisterReadOnly("PageCount", typeof(int), ownerType,
				new PropertyMetadata((d, e) => ((DataPager)d).OnPageCountChanged((int)e.NewValue)));
			PageCountProperty = PageCountPropertyKey.DependencyProperty;
			PageIndexProperty = DependencyPropertyManager.Register("PageIndex", typeof(int), ownerType,
				new PropertyMetadata(-1, (d, e) => ((DataPager)d).OnPageIndexChanged((int)e.OldValue, (int)e.NewValue), (d, baseValue) => ((DataPager)d).OnPageIndexCoerce(baseValue)));
			PagedSourcePropertyKey = DependencyPropertyManager.RegisterReadOnly("PagedSource", typeof(IPagedCollectionView), ownerType,
				new PropertyMetadata((d, e) => ((DataPager)d).OnPagedSourceChanged((IPagedCollectionView)e.OldValue, (IPagedCollectionView)e.NewValue)));
			PagedSourceProperty = PagedSourcePropertyKey.DependencyProperty;
			PageSizeProperty = DependencyPropertyManager.Register("PageSize", typeof(int), ownerType,
				new PropertyMetadata(1, (d, e) => ((DataPager)d).OnPageSizeChanged((int)e.NewValue), (d, baseValue) => ((DataPager)d).OnPageSizeCoerce(baseValue)));
			SourceProperty = DependencyPropertyManager.Register("Source", typeof(IEnumerable), ownerType,
				new PropertyMetadata((d, e) => ((DataPager)d).OnSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue)));
			ShowTotalPageCountProperty = DependencyPropertyManager.Register("ShowTotalPageCount", typeof(bool), ownerType,
				new PropertyMetadata((d, e) => ((DataPager)d).OnShowTotalPageCountChanged((bool)e.NewValue)));
		}
		public static DataPager GetDataPager(DependencyObject obj) {
			return (DataPager)obj.GetValue(DataPagerProperty);
		}
		public static void SetDataPager(DependencyObject obj, DataPager value) {
			obj.SetValue(DataPagerProperty, value);
		}
		internal DataPagerNumericButtonContainer container;
		int beforeAutoNumericButtonCount;
		bool leftScroll = true, rightScroll = true;
		Locker lockerPagedSourcePageSize = new Locker();
		Locker UpdateSourceLocker { get; set; }
		public DataPager() {
			this.SetDefaultStyleKey(typeof(DataPager));
			SetDataPager(this, this);
			SetCommands();
			UpdateSourceLocker = new Locker();
		}
		public IPagedCollectionView PagedSource {
			get { return (IPagedCollectionView)GetValue(PagedSourceProperty); }
			private set { this.SetValue(PagedSourcePropertyKey, value); }
		}
		public int ActualNumericButtonCount {
			get { return (int)GetValue(ActualNumericButtonCountProperty); }
			private set { this.SetValue(ActualNumericButtonCountPropertyKey, value); }
		}
		public int ActualPageIndex {
			get { return (int)GetValue(ActualPageIndexProperty); }
			private set { this.SetValue(ActualPageIndexPropertyKey, value); }
		}
		public int ActualPageSize {
			get { return (int)GetValue(ActualPageSizeProperty); }
			private set { this.SetValue(ActualPageSizePropertyKey, value); }
		}
		public bool AutoEllipsis {
			get { return (bool)GetValue(AutoEllipsisProperty); }
			set { SetValue(AutoEllipsisProperty, value); }
		}
		public bool CanChangePage {
			get { return (bool)GetValue(CanChangePageProperty); }
			private set { this.SetValue(CanChangePagePropertyKey, value); }
		}
		public bool CanMoveToFirstPage {
			get { return (bool)GetValue(CanMoveToFirstPageProperty); }
			private set { this.SetValue(CanMoveToFirstPagePropertyKey, value); }
		}
		public bool CanMoveToLastPage {
			get { return (bool)GetValue(CanMoveToLastPageProperty); }
			private set { this.SetValue(CanMoveToLastPagePropertyKey, value); }
		}
		public bool CanMoveToNextPage {
			get { return (bool)GetValue(CanMoveToNextPageProperty); }
			private set { this.SetValue(CanMoveToNextPagePropertyKey, value); }
		}
		public bool CanMoveToPreviousPage {
			get { return (bool)GetValue(CanMoveToPreviousPageProperty); }
			private set { this.SetValue(CanMoveToPreviousPagePropertyKey, value); }
		}
		public int ContainerFirstButtonPageNumber {
			get { return (int)GetValue(ContainerFirstButtonPageNumberProperty); }
			private set { this.SetValue(ContainerFirstButtonPageNumberPropertyKey, value); }
		}
		public int ContainerSecondButtonPageNumber {
			get { return (int)GetValue(ContainerSecondButtonPageNumberProperty); }
			private set { this.SetValue(ContainerSecondButtonPageNumberPropertyKey, value); }
		}
		public DataPagerCurrentPageParams CurrentPageParams {
			get { return (DataPagerCurrentPageParams)GetValue(CurrentPageParamsProperty); }
			set { SetValue(CurrentPageParamsProperty, value); }
		}
		public DataPagerDisplayMode DisplayMode {
			get { return (DataPagerDisplayMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		public bool IsAutoNumericButtonCount {
			get { return (bool)GetValue(IsAutoNumericButtonCountProperty); }
			private set { this.SetValue(IsAutoNumericButtonCountPropertyKey, value); }
		}
		public int NumericButtonCount {
			get { return (int)GetValue(NumericButtonCountProperty); }
			set { SetValue(NumericButtonCountProperty, value); }
		}
		public bool IsTotalItemCountFixed {
			get { return (bool)GetValue(IsTotalItemCountFixedProperty); }
			set { SetValue(IsTotalItemCountFixedProperty, value); }
		}
		public int ItemCount {
			get { return (int)GetValue(ItemCountProperty); }
			set { SetValue(ItemCountProperty, value); }
		}
		public int PageCount {
			get { return (int)GetValue(PageCountProperty); }
			private set { this.SetValue(PageCountPropertyKey, value); }
		}
		public int PageIndex {
			get { return (int)GetValue(PageIndexProperty); }
			set { SetValue(PageIndexProperty, value); }
		}
		public int PageSize {
			get { return (int)GetValue(PageSizeProperty); }
			set { SetValue(PageSizeProperty, value); }
		}
		public IEnumerable Source {
			get { return (GetValue(SourceProperty) as IEnumerable); }
			set { SetValue(SourceProperty, value); }
		}
		public bool ShowTotalPageCount {
			get { return (bool)GetValue(ShowTotalPageCountProperty); }
			set { SetValue(ShowTotalPageCountProperty, value); }
		}
		public ICommand FirstPageCommand { get; private set; }
		public ICommand LastPageCommand { get; private set; }
		public ICommand NextPageCommand { get; private set; }
		public ICommand NumericPageCommand { get; private set; }
		public ICommand PreviousPageCommand { get; private set; }
		protected DataPagerButtonContainer ButtonContainer { get; private set; }
		public event EventHandler<DataPagerPageIndexChangedEventArgs> PageIndexChanged;
		public event EventHandler<DataPagerPageIndexChangingEventArgs> PageIndexChanging;
		void CheckOnShowFirstLastPage(ref bool leftScroll, ref bool rightScroll) {
			if (ContainerFirstButtonPageNumber == 1)
				leftScroll = false;
			if (ContainerFirstButtonPageNumber + ActualNumericButtonCount - 1 == PageCount)
				rightScroll = false;
		}
		bool ScrollNumericButton(int delta, int deltaSecond) {
			ContainerSecondButtonPageNumber += deltaSecond;
			ContainerFirstButtonPageNumber += delta;
			if (container != null) {
				container.UpdateButtons();
				return true;
			} else
				return false;
		}
		void SetBtnType(DataPagerButton btn, int i, int countButton) {
			if (i == 0)
				btn.ButtonType = DataPagerButtonType.PageFirst;
			else if (i == 1)
				btn.ButtonType = DataPagerButtonType.PagePrevious;
			else if (i == countButton - 2)
				btn.ButtonType = DataPagerButtonType.PageNext;
			else if (i == countButton - 1)
				btn.ButtonType = DataPagerButtonType.PageLast;
			else
				btn.ButtonType = DataPagerButtonType.PageNumeric;
		}
		void SetButtonType(DataPagerButton btn, int i, int countButton) {
			switch (DisplayMode) {
				case DataPagerDisplayMode.FirstLast:
					btn.ButtonType = (i == 0) ? DataPagerButtonType.PageFirst : DataPagerButtonType.PageLast;
					break;
				case DataPagerDisplayMode.FirstLastNumeric:
				case DataPagerDisplayMode.FirstLastPreviousNext:
				case DataPagerDisplayMode.FirstLastPreviousNextNumeric:
					SetBtnType(btn, i, countButton);
					break;
				case DataPagerDisplayMode.Numeric:
					btn.ButtonType = DataPagerButtonType.PageNumeric;
					break;
				case DataPagerDisplayMode.PreviousNext:
					btn.ButtonType = (i == 0) ? DataPagerButtonType.PagePrevious : DataPagerButtonType.PageNext;
					break;
				case DataPagerDisplayMode.PreviousNextNumeric:
					if (i == 0)
						btn.ButtonType = DataPagerButtonType.PagePrevious;
					else if (i == countButton - 1)
						btn.ButtonType = DataPagerButtonType.PageNext;
					else
						btn.ButtonType = DataPagerButtonType.PageNumeric;
					break;
			}
		}
		void SetCommands() {
			FirstPageCommand = DelegateCommandFactory.Create<object>(parameter => MoveToFirstPage(), parameter => { return CanMoveToFirstPage; }, false);
			LastPageCommand = DelegateCommandFactory.Create<object>(parameter => MoveToLastPage(), parameter => { return CanMoveToLastPage; }, false);
			NextPageCommand = DelegateCommandFactory.Create<object>(parameter => MoveToNextPage(), parameter => { return CanMoveToNextPage; }, false);
			NumericPageCommand = DelegateCommandFactory.Create<object>(pageNumber => MoveToPage((int)pageNumber), parameter => { return CanChangePage; }, false);
			PreviousPageCommand = DelegateCommandFactory.Create<object>(parameter => MoveToPreviousPage(), parameter => { return CanMoveToPreviousPage; }, false);
		}
		void SetMoveToPageFlags() {
			if (PageCount < 2)
				CanChangePage = false;
			else
				CanChangePage = true;
			if (CanChangePage) {
				CanMoveToPreviousPage = true;
				CanMoveToFirstPage = true;
				CanMoveToNextPage = true;
				CanMoveToLastPage = true;
				if (ActualPageIndex == 0) {
					CanMoveToPreviousPage = false;
					CanMoveToFirstPage = false;
				} else if (ActualPageIndex == PageCount - 1) {
					if (IsTotalItemCountFixed)
						CanMoveToNextPage = false;
					CanMoveToLastPage = false;
				}
			} else {
				CanMoveToFirstPage = false;
				CanMoveToLastPage = false;
				CanMoveToNextPage = false;
				CanMoveToPreviousPage = false;
			}
		}
		int GetDelta(int oldPageIndex, int newPageIndex, int offset) {
			return newPageIndex - oldPageIndex + offset;
		}
		int GetRelativeDelta(int firstPageNumber, int secondPageNumber, int absoluteDelta, int offset) {
			return firstPageNumber + absoluteDelta + offset - secondPageNumber;
		}
		bool TryScrolling(int oldPageIndex, int newPageIndex) {
			int delta = 0, deltaSecond = 0, deltaOverflow = 0, deltaSecondOverflow = 0;
			if (DisplayMode != DataPagerDisplayMode.PreviousNext && DisplayMode != DataPagerDisplayMode.FirstLast) {
				if (ActualNumericButtonCount == 2 || beforeAutoNumericButtonCount == 2) {
					if (oldPageIndex < newPageIndex) {
						if (newPageIndex != PageCount - 1)
							deltaSecond = GetDelta(oldPageIndex, newPageIndex, 0);
						else
							deltaSecond = GetDelta(oldPageIndex, newPageIndex, -1);
						if (newPageIndex > 1 && newPageIndex < PageCount - 1)
							delta = GetRelativeDelta(ContainerSecondButtonPageNumber, ContainerFirstButtonPageNumber, deltaSecond, -2);
						else if (newPageIndex == PageCount - 1)
							delta = GetRelativeDelta(ContainerSecondButtonPageNumber, ContainerFirstButtonPageNumber, deltaSecond, 0);
					} else {
						if (newPageIndex != 0)
							delta = GetDelta(oldPageIndex, newPageIndex, 0);
						else
							delta = GetDelta(oldPageIndex, newPageIndex, 1);
						if (newPageIndex > 0 && newPageIndex < PageCount - 1)
							deltaSecond = GetRelativeDelta(ContainerFirstButtonPageNumber, ContainerSecondButtonPageNumber, delta, 2);
						else if (newPageIndex == 0)
							deltaSecond = GetRelativeDelta(ContainerFirstButtonPageNumber, ContainerSecondButtonPageNumber, delta, 0);
					}
				} else
					delta = newPageIndex - (ContainerFirstButtonPageNumber - 1 + (beforeAutoNumericButtonCount / 2));
				leftScroll = true;
				rightScroll = true;
				CheckOnShowFirstLastPage(ref leftScroll, ref rightScroll);
				if ((delta > 0 && rightScroll) || (beforeAutoNumericButtonCount == 2 && deltaSecond > 0)) {
					deltaOverflow = (ContainerFirstButtonPageNumber + beforeAutoNumericButtonCount - 1 + delta) - PageCount;
					deltaSecondOverflow = (ContainerSecondButtonPageNumber + deltaSecond) - PageCount;
					if (deltaOverflow > 0)
						delta -= deltaOverflow;
					if (deltaSecondOverflow > 0)
						deltaSecond -= deltaSecondOverflow;
					return ScrollNumericButton(delta, deltaSecond);
				} else if ((delta < 0 && leftScroll) || (beforeAutoNumericButtonCount == 2 && deltaSecond < 0)) {
					deltaOverflow = 1 - (ContainerFirstButtonPageNumber + delta);
					deltaSecondOverflow = 2 - (ContainerSecondButtonPageNumber + deltaSecond);
					if (deltaOverflow > 0)
						delta += deltaOverflow;
					if (deltaSecondOverflow > 0)
						deltaSecond += deltaSecondOverflow;
					return ScrollNumericButton(delta, deltaSecond);
				}
			}
			return false;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (ButtonContainer != null)
				ButtonContainer.NumericButtonContainerChanged -= OnButtonContainerNumericButtonContainerChanged;
			ButtonContainer = GetTemplateChild("PART_ButtonContainer") as DataPagerButtonContainer;
			if (ButtonContainer != null)
				ButtonContainer.NumericButtonContainerChanged += OnButtonContainerNumericButtonContainerChanged;
		}
		void OnButtonContainerNumericButtonContainerChanged(object sender, EventArgs e) {
			container = ButtonContainer.NumericButtonContainer;
		}
		protected virtual void OnActualNumericButtonCountChanged(int newValue) { }
		protected virtual void OnActualPageIndexChanged(int oldValue, int newValue) {
			if(container != null && container.Panel != null)
				beforeAutoNumericButtonCount = container.Panel.Children.Count;
			else
				beforeAutoNumericButtonCount = NumericButtonCount;
			SetMoveToPageFlags();
			if (PagedSource != null)
				MoveToPage(newValue + 1);
			if (!TryScrolling(oldValue, newValue) && container != null)
				container.UpdateButtons();
			UpdateCurrentPageParams();
			RaisePageIndexChanged(oldValue, newValue);
		}
		protected virtual void OnActualPageSizeChanged(int newValue) {
			UpdatePageCount();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			UpdateCurrentPageParams();
			UpdateIsAutoNumericButtonCount();
		}
		protected virtual void OnAutoEllipsisChanged(bool newValue) { }
		protected virtual void OnCanChangePageChanged(bool newValue) {
			(NumericPageCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
		}
		protected virtual void OnCanMoveToFirstPageChanged(bool newValue) {
			(FirstPageCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
		}
		protected virtual void OnCanMoveToLastPageChanged(bool newValue) {
			(LastPageCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
		}
		protected virtual void OnCanMoveToNextPageChanged(bool newValue) {
			(NextPageCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
		}
		protected virtual void OnCanMoveToPreviousPageChanged(bool newValue) {
			(PreviousPageCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
		}
		protected virtual void OnContainerFirstButtonPageNumberChanged(int newValue) { }
		protected object OnContainerFirstButtonPageNumberCoerce(object baseValue) {
			if (ActualNumericButtonCount == 1)
				return Math.Max(1, Math.Min((int)baseValue, PageCount));
			else
				return Math.Max(1, Math.Min((int)baseValue, PageCount - ActualNumericButtonCount + 1));
		}
		protected virtual void OnContainerSecondButtonPageNumberChanged(int newValue) { }
		protected virtual object OnContainerSecondButtonPageNumberCoerce(object baseValue) {
			return Math.Max(2, Math.Min((int)baseValue, PageCount));
		}
		protected virtual void OnCurrentPageParamsChanged() {
			if (!IsInitialized)
				return;
			if (CurrentPageParams.PageCount == -1)
				CurrentPageParams = new DataPagerCurrentPageParams() { PageCount = PageCount, PageIndex = CurrentPageParams.PageIndex };
			else {
				PageIndex = CurrentPageParams.PageIndex;
			}
		}
		protected virtual void OnNumericButtonCountChanged(int oldValue, int newValue) {
			UpdateActualNumericButtonCount(newValue - oldValue);
			UpdateIsAutoNumericButtonCount();
		}
		protected void UpdateIsAutoNumericButtonCount() {
			IsAutoNumericButtonCount = NumericButtonCount == 0;
		}
		protected virtual object OnNumericButtonCountCoerce(object baseValue) {
			return Math.Max(0, (int)baseValue);
		}
		protected virtual void OnDisplayModeChanged(DataPagerDisplayMode newValue) { }
		protected virtual void OnIsTotalItemCountFixedChanged(bool newValue) {
			SetMoveToPageFlags();
		}
		protected virtual void OnItemCountChanged(int newValue) {
			UpdateSource();
		}
		protected virtual void OnPageCountChanged(int newValue) {
			UpdateActualPageIndex();
			UpdateActualNumericButtonCount(0);
			SetMoveToPageFlags();
			UpdateCurrentPageParams();
		}
		protected virtual void OnPagedSourceChanged(IPagedCollectionView oldValue, IPagedCollectionView newValue) {
			var oldNotifyCollectionChanged = oldValue as INotifyCollectionChanged;
			if (oldNotifyCollectionChanged != null)
				oldNotifyCollectionChanged.CollectionChanged -= PagedSourceCollectionChanged;
			var newNotifyCollectionChanged = newValue as INotifyCollectionChanged;
			if (newNotifyCollectionChanged != null)
				newNotifyCollectionChanged.CollectionChanged += PagedSourceCollectionChanged;
		}
		protected virtual void OnPageIndexChanged(int oldValue, int newValue) {
			UpdateActualPageIndex();
		}
		protected virtual object OnPageIndexCoerce(object baseValue) {
			int oldValue = (int)DependencyObjectHelper.GetCoerceValue(this, PageIndexProperty);
			if (RaisePageIndexChanging(oldValue, (int)baseValue))
				return oldValue;
			else
				return Math.Max(0, (int)baseValue);
		}
		protected virtual void OnPageSizeChanged(int newValue) {
			if (PagedSource != null && !lockerPagedSourcePageSize.IsLocked)
				PagedSource.PageSize = newValue;
			SetMoveToPageFlags();
		}
		protected virtual object OnPageSizeCoerce(object baseValue) {
			return Math.Max(1, (int)baseValue);
		}
		protected virtual void OnSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
			if (newValue != null) {
				if (newValue is IPagedCollectionView)
					PagedSource = newValue as IPagedCollectionView;
				else
					PagedSource = new PagedCollectionView(newValue);
				UpdateSource();
			} else {
				PagedSource = null;
				ItemCount = 0;
			}
			if (oldValue != null)
				UnsubscribeOnSourcePropertiesChanged(oldValue);
			if (newValue != null)
				SubscribeOnSourcePropertiesChanged(newValue);
		}
		void SubscribeOnSourcePropertiesChanged(IEnumerable newValue) {
			if (newValue is INotifyPropertyChanged)
				(newValue as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(OnSourcePropertyChanged);
		}
		void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (PagedSource == null)
				return;
			if (e.PropertyName == "Count" && Source != PagedSource) {
				(PagedSource as PagedCollectionView).Do(x => x.Refresh());
				UpdateSource();
			}
			if (e.PropertyName == "ItemCount" && ItemCount != PagedSource.ItemCount)
				lockerPagedSourcePageSize.DoLockedAction(() => UpdateSource());
			if (e.PropertyName == "PageIndex")
				PageIndex = PagedSource.PageIndex;
			if (e.PropertyName == "PageSize")
				UpdateActualPageSize();
		}
		void UnsubscribeOnSourcePropertiesChanged(IEnumerable oldValue) {
			if (oldValue is INotifyPropertyChanged)
				(oldValue as INotifyPropertyChanged).PropertyChanged -= new PropertyChangedEventHandler(OnSourcePropertyChanged);
		}
		protected virtual void OnShowTotalPageCountChanged(bool newValue) { }
		protected virtual void RaisePageIndexChanged(int oldValue, int newValue) {
			if (PageIndexChanged != null)
				PageIndexChanged(this, new DataPagerPageIndexChangedEventArgs(oldValue, newValue));
		}
		protected virtual bool RaisePageIndexChanging(int oldValue, int newValue) {
			if (PageIndexChanging != null && oldValue != newValue) {
				DataPagerPageIndexChangingEventArgs eventArgs = new DataPagerPageIndexChangingEventArgs(oldValue, newValue);
				PageIndexChanging(this, eventArgs);
				return eventArgs.IsCancel;
			}
			return false;
		}
		void UpdateContainerButtonPageNumber(int offset) {
			ContainerFirstButtonPageNumber = PageIndex;
			ContainerSecondButtonPageNumber = ContainerFirstButtonPageNumber + offset;
		}
		protected void UpdateCurrentPageParams() {
			CurrentPageParams = new DataPagerCurrentPageParams() { PageCount = PageCount, PageIndex = ActualPageIndex };
		}
		protected virtual void UpdateActualNumericButtonCount(int deltaCount) {
			int beforeLastPageNumber = ContainerFirstButtonPageNumber + ActualNumericButtonCount - 1;
			ActualNumericButtonCount = Math.Min(NumericButtonCount, PageCount);
			int lastPageNumber = ContainerFirstButtonPageNumber + ActualNumericButtonCount - 1;
			if (ActualNumericButtonCount != 2) {
				if (deltaCount > 0 && ContainerFirstButtonPageNumber > 1)
					ContainerFirstButtonPageNumber -= deltaCount;
				else if (deltaCount < 0 && ContainerFirstButtonPageNumber > 1 && lastPageNumber == PageCount - 1)
					ContainerFirstButtonPageNumber -= deltaCount;
				if (deltaCount < 0 && PageIndex >= lastPageNumber && lastPageNumber != PageCount - 1)
					MoveToPage(lastPageNumber);
			}
			else if (ActualNumericButtonCount == 2) {
				if (beforeLastPageNumber != PageCount) {
					if (deltaCount < 0 && PageIndex >= lastPageNumber)
						PageIndex = lastPageNumber - 1;
					if (lastPageNumber != PageCount - 1)
						UpdateContainerButtonPageNumber(1);
				} else {
					if (deltaCount < 0 && PageIndex < ContainerFirstButtonPageNumber)
						PageIndex = ContainerFirstButtonPageNumber - 1;
						UpdateContainerButtonPageNumber(1);
				}
			}
			if (container != null)
				container.UpdateButtons();
		}
		protected virtual void UpdateActualPageIndex() {
			ActualPageIndex = Math.Min(PageCount - 1, PageIndex);
		}
		protected virtual void UpdateActualPageSize() {
			if (ItemCount == 0)
				ActualPageSize = PageSize;
			else
				ActualPageSize = Math.Min(PageSize, ItemCount);
		}
		protected virtual void UpdatePageCount() {
			PageCount = (ActualPageSize == 0) ? 0 : (int)Math.Ceiling((double)this.ItemCount / (double)this.ActualPageSize);
		}
		public void MoveToFirstPage() {
			PageIndex = 0;
		}
		public void MoveToLastPage() {
			PageIndex = PageCount - 1;
		}
		public void MoveToNextPage() {
			if (CanMoveToNextPage && PageIndex < PageCount - 1)
				PageIndex += 1;
		}
		public void MoveToPage(int pageNumber) {
			if (PageIndex != pageNumber - 1)
				PageIndex = pageNumber - 1;
			else
				if (PagedSource != null && PagedSource.PageIndex != pageNumber - 1)
					PagedSource.MoveToPage(pageNumber - 1);
		}
		public void MoveToPreviousPage() {
			PageIndex -= 1;
		}
		protected virtual void UpdatePagedSourceProperties() {
			if (PagedSource == null) return;
			if (!lockerPagedSourcePageSize.IsLocked)
				PagedSource.PageSize = PageSize;
			PagedSource.MoveToPage(ActualPageIndex);
		}
		protected virtual void PagedSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateSource();
		}
		protected virtual void UpdateSource() {
			UpdateSourceLocker.DoLockedActionIfNotLocked(() => {
				if (PagedSource != null)
					ItemCount = Math.Max(0, PagedSource.ItemCount);
				UpdateActualPageSize();
				UpdatePageCount();
				UpdateActualPageIndex();
				UpdateActualNumericButtonCount(0);
				if (Source == null) {
				}
				else
					if (PageIndex == -1 && PageCount > 0)
						MoveToFirstPage();
					else
						if (PageIndex >= PageCount)
							MoveToPage(PageCount - 1);
				UpdatePagedSourceProperties();
				SetMoveToPageFlags();
			});
		}
	}
	public class DataPagerButton : Button {
		public static readonly DependencyProperty ButtonTypeProperty;
		public static readonly DependencyProperty DisplayModeProperty;
		public static readonly DependencyProperty IsCurrentPageProperty;
		public static readonly DependencyProperty ShowEllipsisProperty;
		public static readonly DependencyProperty PageNumberProperty;
		static DataPagerButton() {
			Type ownerType = typeof(DataPagerButton);
			ButtonTypeProperty = DependencyPropertyManager.Register("ButtonType", typeof(DataPagerButtonType), ownerType, new PropertyMetadata((PropertyChangedCallback)null));
			DisplayModeProperty = DependencyPropertyManager.Register("DisplayMode", typeof(DataPagerDisplayMode), ownerType,
				new PropertyMetadata(DataPagerDisplayMode.FirstLastPreviousNextNumeric, (d, e) => ((DataPagerButton)d).OnDisplayModeChanged((DataPagerDisplayMode)e.OldValue)));
			IsCurrentPageProperty = DependencyPropertyManager.Register("IsCurrentPage", typeof(bool), ownerType,
				new PropertyMetadata((d, e) => ((DataPagerButton)d).OnIsCurrentPageChanged((bool)e.OldValue)));
			ShowEllipsisProperty = DependencyPropertyManager.Register("ShowEllipsis", typeof(bool), ownerType, new PropertyMetadata((PropertyChangedCallback)null));
			PageNumberProperty = DependencyPropertyManager.Register("PageNumber", typeof(int), ownerType, new PropertyMetadata(0));
		}
		public DataPagerButton() {
			this.SetDefaultStyleKey(typeof(DataPagerButton));
		}
		public DataPagerButtonType ButtonType {
			get { return (DataPagerButtonType)GetValue(ButtonTypeProperty); }
			set { SetValue(ButtonTypeProperty, value); }
		}
		public DataPagerDisplayMode DisplayMode {
			get { return (DataPagerDisplayMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		public bool IsCurrentPage {
			get { return (bool)GetValue(IsCurrentPageProperty); }
			set { SetValue(IsCurrentPageProperty, value); }
		}
		public bool ShowEllipsis {
			get { return (bool)GetValue(ShowEllipsisProperty); }
			set { SetValue(ShowEllipsisProperty, value); }
		}
		public int PageNumber {
			get { return (int)GetValue(PageNumberProperty); }
			set { SetValue(PageNumberProperty, value); }
		}
		public override void OnApplyTemplate() {
			SetStates(false);
		}
		protected virtual void OnDisplayModeChanged(DataPagerDisplayMode oldValue) {
			SetStates(true);
		}
		protected virtual void OnIsCurrentPageChanged(bool oldValue) {
			UpdateNumericButtonState(false);
		}
		void SetStates(bool useTransitions) {
			switch (DisplayMode) {
				case DataPagerDisplayMode.FirstLast:
				case DataPagerDisplayMode.FirstLastNumeric:
					SetPairState(DataPagerButtonType.PageFirst, "FirstLeft", DataPagerButtonType.PageLast, "FirstRight", useTransitions);
					UpdateNumericButtonState(useTransitions);
					break;
				case DataPagerDisplayMode.FirstLastPreviousNext:
				case DataPagerDisplayMode.FirstLastPreviousNextNumeric:
					SetPairState(DataPagerButtonType.PageFirst, "FirstLeft", DataPagerButtonType.PageLast, "FirstRight", useTransitions);
					SetPairState(DataPagerButtonType.PagePrevious, "SecondLeft", DataPagerButtonType.PageNext, "SecondRight", useTransitions);
					UpdateNumericButtonState(useTransitions);
					break;
				case DataPagerDisplayMode.PreviousNext:
				case DataPagerDisplayMode.PreviousNextNumeric:
					SetPairState(DataPagerButtonType.PagePrevious, "FirstLeft", DataPagerButtonType.PageNext, "FirstRight", useTransitions);
					UpdateNumericButtonState(useTransitions);
					break;
			}
		}
		void SetPairState(DataPagerButtonType firstType, string stateName, DataPagerButtonType secondType, string secondStateName, bool useTransitions) {
			if (ButtonType == firstType)
				VisualStateManager.GoToState(this, stateName, useTransitions);
			else if (ButtonType == secondType)
				VisualStateManager.GoToState(this, secondStateName, useTransitions);
		}
		void UpdateNumericButtonState(bool useTransitions) {
			if (ButtonType == DataPagerButtonType.PageNumeric)
				VisualStateManager.GoToState(this, (this.IsCurrentPage) ? "Selected" : "NotSelected", useTransitions);
		}
	}
	public class DataPagerNumericButton : DataPagerButton {
		public DataPagerNumericButton(){
			this.SetDefaultStyleKey(typeof(DataPagerNumericButton));
		}
	}
	public class DataPagerNumericButtonContainer : Control {
		public static readonly DependencyProperty AutoEllipsisProperty;
		public static readonly DependencyProperty ButtonCountProperty;
		public static readonly DependencyProperty CurrentIndexProperty;
		public static readonly DependencyProperty FirstButtonPageNumberProperty;
		public static readonly DependencyProperty NumericButtonContainerProperty;
		public static readonly DependencyProperty PageCountProperty;
		public static readonly DependencyProperty SecondButtonPageNumberProperty;
		static DataPagerNumericButtonContainer() {
			AutoEllipsisProperty = DependencyPropertyManager.Register("AutoEllipsis", typeof(bool), typeof(DataPagerNumericButtonContainer),
				new PropertyMetadata((d, e) => ((DataPagerNumericButtonContainer)d).OnAutoEllipsisChanged()));
			ButtonCountProperty = DependencyPropertyManager.Register("ButtonCount", typeof(int), typeof(DataPagerNumericButtonContainer),
				new PropertyMetadata((PropertyChangedCallback)null));
			CurrentIndexProperty = DependencyPropertyManager.Register("CurrentIndex", typeof(int), typeof(DataPagerNumericButtonContainer),
				new PropertyMetadata((d, e) => ((DataPagerNumericButtonContainer)d).OnCurrentIndexChanged()));
			FirstButtonPageNumberProperty = DependencyPropertyManager.Register("FirstButtonPageNumber", typeof(int), typeof(DataPagerNumericButtonContainer),
				new PropertyMetadata((d, e) => ((DataPagerNumericButtonContainer)d).OnFirstButtonPageNumberChanged()));
			NumericButtonContainerProperty = DependencyPropertyManager.RegisterAttached("NumericButtonContainer", typeof(DataPagerNumericButtonContainer), typeof(DataPagerNumericButtonContainer), new PropertyMetadata(null));
			PageCountProperty = DependencyPropertyManager.Register("PageCount", typeof(int), typeof(DataPagerNumericButtonContainer),
				new PropertyMetadata((d, e) => ((DataPagerNumericButtonContainer)d).OnPageCountChanged()));
			SecondButtonPageNumberProperty = DependencyPropertyManager.Register("SecondButtonPageNumber", typeof(int), typeof(DataPagerNumericButtonContainer),
				new PropertyMetadata(2, (d, e) => ((DataPagerNumericButtonContainer)d).OnSecondButtonPageNumberChanged()));
		}
		public static DataPagerNumericButtonContainer GetNumericButtonContainer(DependencyObject obj) {
			return (DataPagerNumericButtonContainer)obj.GetValue(NumericButtonContainerProperty);
		}
		public static void SetNumericButtonContainer(DependencyObject obj, DataPagerNumericButtonContainer value) {
			obj.SetValue(NumericButtonContainerProperty, value);
		}
		public DataPagerNumericButtonContainer() {
			DataPagerNumericButtonContainer.SetNumericButtonContainer(this, this);
			this.SetDefaultStyleKey(typeof(DataPagerNumericButtonContainer));
		}
		public bool AutoEllipsis {
			get { return (bool)GetValue(AutoEllipsisProperty); }
			set { SetValue(AutoEllipsisProperty, value); }
		}
		public int ButtonCount {
			get { return (int)GetValue(ButtonCountProperty); }
			set { SetValue(ButtonCountProperty, value); }
		}
		public int CurrentIndex {
			get { return (int)GetValue(CurrentIndexProperty); }
			set { SetValue(CurrentIndexProperty, value); }
		}
		public int FirstButtonPageNumber {
			get { return (int)GetValue(FirstButtonPageNumberProperty); }
			set { SetValue(FirstButtonPageNumberProperty, value); }
		}
		public int PageCount {
			get { return (int)GetValue(PageCountProperty); }
			set { SetValue(PageCountProperty, value); }
		}
		public int SecondButtonPageNumber {
			get { return (int)GetValue(SecondButtonPageNumberProperty); }
			set { SetValue(SecondButtonPageNumberProperty, value); }
		}
		public Panel Panel { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Panel = GetTemplateChild("PART_Panel") as Panel;
			if (Panel != null)
				UpdateButtons();
		}
		void ChangeButtonProperties(DataPagerButton dpButton, int pageNumber) {
			dpButton.PageNumber = pageNumber;
			dpButton.IsCurrentPage = false;
			if (dpButton.PageNumber == CurrentIndex + 1)
				dpButton.IsCurrentPage = true;
		}
		protected virtual void OnAutoEllipsisChanged() {
			UpdateButtons();
		}
		protected virtual void OnCurrentIndexChanged() {
			UpdateButtons(); 
		}
		protected virtual void OnFirstButtonPageNumberChanged() {
			if (ButtonCount != 2)
				UpdateButtons();
		}
		protected virtual void OnPageCountChanged() {
			UpdateButtons();
			if (Panel != null)
				Panel.InvalidateMeasure();
		}
		protected virtual void OnSecondButtonPageNumberChanged() { }
		public void UpdateButtons() {
			if (Panel == null) return;
			DataPagerButton dpButton;
			for (int i = 0; i < Panel.Children.Count; i++) {
				dpButton = GetButton(i);
				if (dpButton != null) {
					dpButton.ShowEllipsis = false;
					if (Panel.Children.Count == 2) {
						if (i == 0)
							ChangeButtonProperties(dpButton, FirstButtonPageNumber);
						else
							ChangeButtonProperties(dpButton, SecondButtonPageNumber);
						if (dpButton.PageNumber > 1 && dpButton.PageNumber < PageCount && AutoEllipsis)
							dpButton.ShowEllipsis = true;
					} else {
						ChangeButtonProperties(dpButton, FirstButtonPageNumber + i);
						if (((i == 0 && dpButton.PageNumber > 1) || (i == Panel.Children.Count - 1 && dpButton.PageNumber < PageCount)) && AutoEllipsis)
							dpButton.ShowEllipsis = true;
					}
				}
			}
		}
		internal DataPagerButton GetButton(int index) {
			return (DataPagerButton)Panel.Children[index];
		}
		public DataPagerButton CreateNumericButton(int pageNumber) {
			DataPagerButton button = new DataPagerNumericButton();
			DataPager pager = LayoutHelper.FindParentObject<DataPager>(this);
			if (pager != null)
				button.Command = pager.NumericPageCommand;
			button.SetBinding(ButtonBase.CommandParameterProperty, new Binding("PageNumber") { Source = button });
			ChangeButtonProperties(button, pageNumber);
			button.ButtonType = DataPagerButtonType.PageNumeric;
			button.ShowEllipsis = false;
			if (Panel != null)
				if ((pageNumber == FirstButtonPageNumber && pageNumber > 1 || pageNumber == FirstButtonPageNumber + Panel.Children.Count - 1 && pageNumber < PageCount) && AutoEllipsis)
					button.ShowEllipsis = true;
			return button;
		}
		public DataPagerNumericButtonContainerPanel GetPanel() {
			return Panel as DataPagerNumericButtonContainerPanel;
		}
	}
	public class DataPagerDisplayModeToButtonVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			int index = ((string)parameter).Split(new char[] { '_' }).ToList<string>().IndexOf(value.ToString());
			return (index != -1) ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DataPagerBoolToVisibilityInvertConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (bool)value ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DataPagerPageIndexToPageNumberConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			DataPagerCurrentPageParams currentPageParams = (DataPagerCurrentPageParams)value;
			return Math.Min(currentPageParams.PageIndex + 1, currentPageParams.PageCount);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return new DataPagerCurrentPageParams() { PageCount = -1, PageIndex = (int)(((decimal)value) - 1) };
		}
	}
	public class DataPagerCurrentPageEditMinValueConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (decimal?)(Math.Min((int)value, 1));
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class DataPagerCurrentPageEditMaxValueConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (decimal?)((int)value);
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class DataPagerPageCountToTitleConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return String.Format(EditorLocalizer.Active.GetLocalizedString(EditorStringId.Of), value.ToString());
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DataPagerPageExtract : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return EditorLocalizer.Active.GetLocalizedString(EditorStringId.Page);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class FixedNumericButtonCountHorizontalAlignmentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return ((HorizontalAlignment)value == HorizontalAlignment.Stretch) ? HorizontalAlignment.Left : value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DataPagerNumericButtonContainerPanel : Panel {
		public static readonly DependencyProperty ButtonCountProperty;
		public static readonly DependencyProperty HorizontalContentAlignmentProperty;
		static DataPagerNumericButtonContainerPanel() {
			Type ownerType = typeof(DataPagerNumericButtonContainerPanel);
			ButtonCountProperty = DependencyPropertyManager.Register("ButtonCount", typeof(int), ownerType, new PropertyMetadata((d, e) => ((DataPagerNumericButtonContainerPanel)d).OnButtonCountChanged((int)e.OldValue)));
			HorizontalContentAlignmentProperty = DependencyPropertyManager.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), ownerType,
			  new FrameworkPropertyMetadata(HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange, (d, e) => ((DataPagerNumericButtonContainerPanel)d).PropertyChangedHorizontalContentAlignment((HorizontalAlignment)e.OldValue)));
		}
		double oldAvaibleWidth;
		double newAvaibleWidth;
		int lastAddPageIndex;
		int oldButtonCount;
		public int ButtonCount {
			get { return (int)GetValue(ButtonCountProperty); }
			set { SetValue(ButtonCountProperty, value); }
		}
		public HorizontalAlignment HorizontalContentAlignment {
			get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
			set { SetValue(HorizontalContentAlignmentProperty, value); }
		}
		protected DataPagerNumericButtonContainer Container {
			get { return DataPagerNumericButtonContainer.GetNumericButtonContainer(this); }
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double x = 0;
			double totalChildWidth = 0;
			foreach (UIElement child in Children)
				totalChildWidth += child.DesiredSize.Width;
			switch (HorizontalContentAlignment) {
				case HorizontalAlignment.Center:
					x = Math.Floor((finalSize.Width - totalChildWidth) / 2);
					break;
				case HorizontalAlignment.Left:
				case HorizontalAlignment.Stretch:
					x = 0;
					break;
				case HorizontalAlignment.Right:
					x = Math.Floor(finalSize.Width - totalChildWidth);
					break;
			}
			foreach (UIElement child in Children) {
				child.Arrange(new Rect(x, 0, child.DesiredSize.Width, child.DesiredSize.Height));
				x += child.DesiredSize.Width;
			}
			return finalSize;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (Container.PageCount == 0)
				Children.Clear();
			newAvaibleWidth = availableSize.Width;
			if (oldAvaibleWidth == 0)
				oldAvaibleWidth = availableSize.Width;
			Size result = new Size();
			foreach (UIElement child in Children) {
				child.Measure(availableSize);
				result.Width += child.DesiredSize.Width; 
			}
			if (ButtonCount == 0) {
				int firstPageNumber;
				if (Children.Count == 0)
					firstPageNumber = 1;
				else
					firstPageNumber = ((DataPagerNumericButton)Children[0]).PageNumber;
				double deltaAvaibleWidth = newAvaibleWidth - oldAvaibleWidth;
				DataPager pager = LayoutHelper.FindParentObject<DataPager>(this);
				if (deltaAvaibleWidth > 0 || oldButtonCount != 0) {
					while (result.Width < availableSize.Width) {
						if (Children.Count >= Container.PageCount) break;
						if (firstPageNumber + Children.Count - 1 < Container.PageCount) {
							if (Children.Count != 0) {
								((DataPagerNumericButton)Children[Children.Count - 1]).ShowEllipsis = false;
								result.Width -= Children[Children.Count - 1].DesiredSize.Width;
								Children[Children.Count - 1].Measure(availableSize);
								result.Width += Children[Children.Count - 1].DesiredSize.Width;
								Children.Add(Container.CreateNumericButton(firstPageNumber + Children.Count));
							}
							else
								Children.Add(Container.CreateNumericButton(firstPageNumber));
							if (((DataPagerNumericButton)Children[Children.Count - 1]).PageNumber != Container.PageCount && pager.AutoEllipsis)
								((DataPagerNumericButton)Children[Children.Count - 1]).ShowEllipsis = true;
							Children[Children.Count - 1].Measure(availableSize);
							result.Width += Children[Children.Count - 1].DesiredSize.Width;
							lastAddPageIndex = Children.Count - 1;
						} else {
							if (Children.Count != 0) {
								((DataPagerNumericButton)Children[0]).ShowEllipsis = false;
								result.Width -= Children[0].DesiredSize.Width;
								Children[0].Measure(availableSize);
								result.Width += Children[0].DesiredSize.Width;
							}
							Children.Insert(0, Container.CreateNumericButton(firstPageNumber - 1));
							if (((DataPagerNumericButton)Children[0]).PageNumber != 1 && pager.AutoEllipsis)
								((DataPagerNumericButton)Children[0]).ShowEllipsis = true;
							Children[0].Measure(availableSize);
							result.Width += Children[0].DesiredSize.Width;
							lastAddPageIndex = 0;
						}
						firstPageNumber = ((DataPagerNumericButton)Children[0]).PageNumber;
					}
					while (result.Width > availableSize.Width) {
						DataPagerNumericButton lastPageButton = (Children.Count != 0) ? (DataPagerNumericButton)Children[Children.Count - 1] : null;
						if (lastPageButton != null && lastPageButton.IsCurrentPage) {
							result.Width -= Children[0].DesiredSize.Width;
							Children.RemoveAt(0);
							if (((DataPagerNumericButton)Children[0]).PageNumber != 1 && pager.AutoEllipsis)
								((DataPagerNumericButton)Children[0]).ShowEllipsis = true;
						} else {
							result.Width -= Children[lastAddPageIndex].DesiredSize.Width;
							Children.RemoveAt(lastAddPageIndex);
							if (((DataPagerNumericButton)Children[Children.Count - 1]).PageNumber != Container.PageCount && pager.AutoEllipsis)
								((DataPagerNumericButton)Children[Children.Count - 1]).ShowEllipsis = true;
							if (((DataPagerNumericButton)Children[0]).PageNumber != 1 && pager.AutoEllipsis)
								((DataPagerNumericButton)Children[0]).ShowEllipsis = true;
							if (lastAddPageIndex != 0)
								lastAddPageIndex--;
						}
					}
				} else if (deltaAvaibleWidth < 0) {
					while (result.Width > availableSize.Width) {
						DataPagerNumericButton lastPageButton = (Children.Count != 0) ? (DataPagerNumericButton)Children[Children.Count - 1] : null;
						if (lastPageButton != null && lastPageButton.PageNumber == Container.PageCount && lastPageButton.IsCurrentPage) {
							result.Width -= Children[0].DesiredSize.Width;
							Children.RemoveAt(0);
							if (((DataPagerNumericButton)Children[0]).PageNumber != 1 && pager.AutoEllipsis)
								((DataPagerNumericButton)Children[0]).ShowEllipsis = true;
						} else if (lastPageButton != null && !lastPageButton.IsCurrentPage) {
							result.Width -= Children[Children.Count - 1].DesiredSize.Width;
							Children.RemoveAt(Children.Count - 1);
						} else if (lastPageButton != null && lastPageButton.IsCurrentPage) {
							if (pager != null)
								pager.MoveToPreviousPage();
						}
						if (((DataPagerNumericButton)Children[Children.Count - 1]).PageNumber != Container.PageCount && pager.AutoEllipsis)
							((DataPagerNumericButton)Children[Children.Count - 1]).ShowEllipsis = true;
					}
				} else if (deltaAvaibleWidth == 0) {
					while (result.Width < availableSize.Width && Children.Count < Container.PageCount) {
						if (Children.Count != 0)
							((DataPagerNumericButton)Children[Children.Count - 1]).ShowEllipsis = false;
						Children.Add(Container.CreateNumericButton(firstPageNumber + Children.Count));
						Children[Children.Count - 1].Measure(availableSize);
						result.Width += Children[Children.Count - 1].DesiredSize.Width;
						if (((DataPagerNumericButton)Children[Children.Count - 1]).PageNumber != Container.PageCount && pager.AutoEllipsis)
							((DataPagerNumericButton)Children[Children.Count - 1]).ShowEllipsis = true;
					}
					while (result.Width > availableSize.Width) {
						DataPagerNumericButton lastPageButton = (Children.Count != 0) ? (DataPagerNumericButton)Children[Children.Count - 1] : null;
						if (lastPageButton != null && lastPageButton.IsCurrentPage) {
							result.Width -= Children[0].DesiredSize.Width;
							Children.RemoveAt(0);
							if (((DataPagerNumericButton)Children[0]).PageNumber != 1 && pager.AutoEllipsis)
								((DataPagerNumericButton)Children[0]).ShowEllipsis = true;
						} else {
							result.Width -= Children[Children.Count - 1].DesiredSize.Width;
							Children.RemoveAt(Children.Count - 1);
							if (((DataPagerNumericButton)Children[Children.Count - 1]).PageNumber != Container.PageCount && pager.AutoEllipsis)
								((DataPagerNumericButton)Children[Children.Count - 1]).ShowEllipsis = true;
							if (((DataPagerNumericButton)Children[0]).PageNumber != 1 && pager.AutoEllipsis)
								((DataPagerNumericButton)Children[0]).ShowEllipsis = true;
							if (lastAddPageIndex != 0)
								lastAddPageIndex--;
						}
					}
				}
				oldButtonCount = 0;
			}
			oldAvaibleWidth = newAvaibleWidth;
			foreach (UIElement child in Children)
				result.Height = Math.Max(result.Height, child.DesiredSize.Height);
			return result;
		}
		protected virtual void PropertyChangedHorizontalContentAlignment(HorizontalAlignment oldValue) {
		}
		protected virtual void OnButtonCountChanged(int oldValue) {
			if (ButtonCount > 0) {
				if (ButtonCount > Children.Count)
					while (ButtonCount > Children.Count)
						Children.Add(Container.CreateNumericButton(Children.Count + 1));
				else if (ButtonCount < Children.Count)
					while (ButtonCount < Children.Count)
						Children.RemoveAt(Children.Count - 1);
			} else if (ButtonCount == 0) {
				oldButtonCount = oldValue;
				InvalidateMeasure();
			}
		}
	}
	public class DataPagerButtonContainer : ContentControl {
		public DataPagerNumericButtonContainer NumericButtonContainer { get; private set; }
		public event EventHandler NumericButtonContainerChanged;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			DataPagerNumericButtonContainer prevNumericButtonContainer = NumericButtonContainer;
			NumericButtonContainer = (DataPagerNumericButtonContainer)GetTemplateChild("PART_NumButtonContainer");
			if (NumericButtonContainer != prevNumericButtonContainer)
				RaiseNumericButtonContainerChanged();
		}
		protected void RaiseNumericButtonContainerChanged() {
			if (NumericButtonContainerChanged != null)
				NumericButtonContainerChanged(this, EventArgs.Empty);
		}
	}
}
