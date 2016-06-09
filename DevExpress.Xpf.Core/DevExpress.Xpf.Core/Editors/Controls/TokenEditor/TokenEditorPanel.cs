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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Mvvm.Native;
using System.Diagnostics;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Editors.Internal {
	public class TokenEditorPanel : Panel, IScrollInfo {
		const double Epsilon = 0.000000001;
		const double TokenMagicWidth = 100;
		const int TokensInLine = 2;
		public static readonly DependencyProperty DisplayItemsProperty;
		public static readonly DependencyProperty EnableTokenWrappingProperty;
		public static readonly DependencyProperty DeleteItemButtonTemplateProperty;
		public static readonly DependencyProperty TokenContainerTemplateProperty;
		public static readonly DependencyProperty EmptyTokenContainerTemplateProperty;
		public static readonly DependencyProperty ShowTokenButtonsProperty;
		public static readonly DependencyProperty TokenButtonsProperty;
		static TokenEditorPanel() {
			Type ownerType = typeof(TokenEditorPanel);
			DisplayItemsProperty = DependencyProperty.Register("DisplayItems", typeof(List<CustomItem>), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((TokenEditorPanel)d).OnDisplayItemsChanged((List<CustomItem>)e.OldValue, (List<CustomItem>)e.NewValue)));
			EnableTokenWrappingProperty = DependencyProperty.Register("EnableTokenWrapping", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
			DeleteItemButtonTemplateProperty = DependencyProperty.Register("DeleteItemButtonTemplate", typeof(DataTemplate), ownerType);
			TokenContainerTemplateProperty = DependencyProperty.Register("TokenContainerTemplate", typeof(ControlTemplate), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((TokenEditorPanel)d).OnTokenContainerTemplateChanged(), (d, e) => ((TokenEditorPanel)d).CoerceTokenContainerTemplate(e)));
			EmptyTokenContainerTemplateProperty = DependencyProperty.Register("EmptyTokenContainerTemplate", typeof(ControlTemplate), ownerType);
			TokenButtonsProperty = DependencyProperty.Register("TokenButtons", typeof(ButtonInfoCollection), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((TokenEditorPanel)d).OnTokenButtonsChanged()));
			ShowTokenButtonsProperty = DependencyProperty.Register("ShowTokenButtons", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, (d, e) => ((TokenEditorPanel)d).OnShowTokenButtonsChanged()));
		}
		public TokenEditorPanel() {
			Items = new List<CustomItem>();
			DefaultStyleKey = typeof(TokenEditorPanel);
			this.Loaded += TokenEditorPanel_Loaded;
			MakeVisibleLocker = new Locker();
			MakeVisibleLocker.LockOnce();
		}
		public ButtonInfoCollection TokenButtons {
			get { return (ButtonInfoCollection)GetValue(TokenButtonsProperty); }
			set { SetValue(TokenButtonsProperty, value); }
		}
		public bool ShowTokenButtons {
			get { return (bool)GetValue(ShowTokenButtonsProperty); }
			set { SetValue(ShowTokenButtonsProperty, value); }
		}
		public List<CustomItem> DisplayItems {
			get { return (List<CustomItem>)GetValue(DisplayItemsProperty); }
			set { SetValue(DisplayItemsProperty, value); }
		}
		public bool EnableTokenWrapping {
			get { return (bool)GetValue(EnableTokenWrappingProperty); }
			set { SetValue(EnableTokenWrappingProperty, value); }
		}
		public DataTemplate DeleteItemButtonTemplate {
			get { return (DataTemplate)GetValue(DeleteItemButtonTemplateProperty); }
			set { SetValue(DeleteItemButtonTemplateProperty, value); }
		}
		public ControlTemplate TokenContainerTemplate {
			get { return (ControlTemplate)GetValue(TokenContainerTemplateProperty); }
			set { SetValue(TokenContainerTemplateProperty, value); }
		}
		public ControlTemplate EmptyTokenContainerTemplate {
			get { return (ControlTemplate)GetValue(EmptyTokenContainerTemplateProperty); }
			set { SetValue(EmptyTokenContainerTemplateProperty, value); }
		}
		public int StartMeasureIndex { get; private set; }
		public bool CanShowDefaultToken { get { return Owner.ShowDefaultToken; } }
		public int MinVisibleIndex { get { return MeasureStrategy.Return(x => x.MinVisibleIndex, () => -1); } }
		public int MaxVisibleIndex { get { return MeasureStrategy.Return(x => x.MaxVisibleIndex, () => -1); } }
		public double ExtentHeight { get { return scrollData.ExtentSize.Height; } }
		public double ExtentWidth { get { return scrollData.ExtentSize.Width; } }
		public double HorizontalOffset { get { return scrollData.HorizontalOffset; } }
		public double VerticalOffset { get { return scrollData.VerticalOffset; } }
		public double ViewportWidth { get { return scrollData.ViewportSize.Width; } }
		public double ViewportHeight { get { return scrollData.ViewportSize.Height; } }
		public ScrollViewer ScrollOwner {
			get { return scrollData.ScrollOwner; }
			set { scrollData.ScrollOwner = value; }
		}
		public double PreviousHorizontalOffset { get; private set; }
		public double PreviousVerticalOffset { get; private set; }
		public bool ShowNewTokenFromEnd { get { return Owner.ShowNewTokenFromEnd; } }
		public Orientation Orientation { get { return MeasureStrategy.Orientation; } }
		public bool HasMeasuredTokens { get { return measuredItems.Count > 0; } }
		public bool LockBringIntoView { get; set; }
		TokenEditor owner;
		internal TokenEditor Owner {
			get { return owner; }
			set {
				owner = value;
				owner.PreviewMouseDown += OnTokenEditorPreviewMouseDown;
				owner.PreviewMouseMove += OnTokenEditorPreviewMouseMove;
			}
		}
		internal TokenEditorPresenter DefaultTokenPresenter { get; set; }
		internal List<CustomItem> Items { get; set; }
		internal TokenEditorMeasureStrategyBase MeasureStrategy { get; private set; }
		bool ShouldScrollToEnd { get; set; }
		bool ShouldCorrectHorizontalOffset { get; set; }
		bool ShouldEnsureHorizontalOffset { get; set; }
		bool ShouldCalculateFirstVisibleIndex { get; set; }
		int MaxLineIndex { get; set; }
		Locker MakeVisibleLocker { get; set; }
		double PreviousTokenStartPosition { get; set; }
		public void EnsureOffset() {
			if (MeasureStrategy == null) return;
			if (Orientation == System.Windows.Controls.Orientation.Horizontal) {
				if (HorizontalOffset > GetMaxHorizontalOffset())
					SetHorizontalOffset(GetMaxHorizontalOffset());
			}
			else if (VerticalOffset > GetMaxVerticalOffset())
				SetVerticalOffset(GetMaxVerticalOffset());
		}
		public void PageDown() {
			SetVerticalOffset(VerticalOffset + ViewportHeight);
		}
		public void PageLeft() {
			SetHorizontalOffset(HorizontalOffset - ViewportWidth);
		}
		public void PageRight() {
			SetHorizontalOffset(HorizontalOffset + ViewportWidth);
		}
		public void PageUp() {
			SetVerticalOffset(VerticalOffset - ViewportHeight);
		}
		double lineStep = 20;
		public void LineDown() {
			SetVerticalOffset(VerticalOffset + lineStep);
		}
		public void LineUp() {
			SetVerticalOffset(VerticalOffset - lineStep);
		}
		public void UpdateMeasureStrategy() {
			if (Owner != null) {
				if (!Owner.EnableTokenWrapping)
					MeasureStrategy = Owner.ShowNewTokenFromEnd ? new TokenEditorLineFromEndMeasureStrategy(this) : new TokenEditorLineMeasureStrategy(this);
				else
					MeasureStrategy = Owner.ShowNewTokenFromEnd ? new TokenEditorWrapLineFromEndMeasureStrategy(this) : new TokenEditorWrapLineMeasureStrategy(this);
			}
			ShouldEnsureHorizontalOffset = true;
		}
		public List<UIElement> GetInplaceEditorContainers() {
			return measuredItems.Values.ToList();
		}
		public void OnThumbDragDelta(DragDeltaEventArgs e) {
			if (Orientation == System.Windows.Controls.Orientation.Vertical)
				TryHandlesVerticalChange(e);
		}
		public void ClearDefaultTokenValue() {
			UpdateDefaultTokenPresenter();
		}
		public void SetHorizontalOffset(double offset) {
			PreviousHorizontalOffset = HorizontalOffset;
			SetHorizontalOffsetInternal(offset);
			InvalidateMeasure();
		}
		public double IndexToOffset(int index) {
			double offset = 0;
			for (int i = 0; i < index; i++) {
				offset += GetTokenSize(i);
			}
			return offset;
		}
		public int OffsetToIndex(double offset) {
			int maxIndex = GetMaxIndex();
			for (int i = 0; i <= maxIndex; i++) {
				offset -= GetTokenSize(i);
				if (offset < 0)
					return i;
			}
			return maxIndex;
		}
		public void SetVerticalOffset(double offset) {
			MeasureStrategy.InvalidateLines();
			PreviousVerticalOffset = VerticalOffset;
			ShouldCalculateFirstVisibleIndex = true;
			SetVerticalOffsetInternal(offset);
			InvalidateMeasure();
		}
		public void LineLeft() {
			SetHorizontalOffset(HorizontalOffset - TokenMagicWidth);
		}
		public void LineRight() {
			SetHorizontalOffset(HorizontalOffset + TokenMagicWidth);
		}
		public Rect MakeVisible(Visual visual, Rect rectangle) {
			if (!MakeVisibleLocker.IsLocked) {
				MakeVisibleLocker.LockOnce();
				TokenEditorPresenter token = LayoutHelper.FindParentObject<TokenEditorPresenter>(visual);
				if (token != null) {
					Rect rect = LayoutHelper.GetRelativeElementRect(token, this);
					if (Orientation == System.Windows.Controls.Orientation.Horizontal)
						rect = MakeVisibleHorizontal(rect);
					else
						rect = MakeVisibleVertical(rect);
					return rect;
				}
			}
			return Rect.Empty;
		}
		public UIElement PrepareContainer(int editableIndex) {
			var data = Items[editableIndex];
			var container = GenerateContainer(editableIndex, data);
			PrepareContainer(container, Items[editableIndex]);
			measuredItems.Add(editableIndex, container);
			return container;
		}
		public UIElement GetContainer(int editableIndex) {
			return measuredItems.ContainsKey(editableIndex) ? measuredItems[editableIndex] : null;
		}
		Locker horizontalOffsetCorrectionLocker = new Locker();
		public void ScrollRight(int index) {
			if (!BringIntoViewByIndex(index)) {
				ScrollToHorizontalEnd();
			}
		}
		public void ScrollToHorizontalEnd() {
			ShouldEnsureHorizontalOffset = true;
			ShouldScrollToEnd = true;
			InvalidateMeasure();
		}
		public void ScrollLeft(int index) {
			if (!BringIntoViewByIndex(index)) {
				SetHorizontalOffset(IndexToOffset(index));
			}
		}
		public void ScrollUp() {
			ScrollVertical(true);
		}
		public void ScrollDown() {
			ScrollVertical(false);
		}
		public void BringIntoView(TokenEditorPresenter token) {
			if (token != null && !LockBringIntoView) {
				MakeVisibleLocker.Unlock();
				MakeVisible(token, new Rect(0, 0, token.ActualWidth, token.ActualHeight));
			}
		}
		public void ScrollToDefaultToken() {
			if (ShowNewTokenFromEnd)
				ScrollToEnd();
			else
				ScrollToStart();
		}
		private void ScrollToStart() {
			if (Orientation == System.Windows.Controls.Orientation.Horizontal)
				SetHorizontalOffset(0);
			else
				SetVerticalOffset(0);
		}
		private void ScrollToEnd() {
			if (Orientation == System.Windows.Controls.Orientation.Horizontal)
				ScrollToHorizontalEnd();
			else
				ScrollToVerticalEnd();
		}
		protected override Size MeasureOverride(Size constraint) {
			if (Owner == null || MeasureStrategy == null) return new Size(0, 0);
			if (DefaultTokenPresenter == null)
				CreateDefaultTokenEditor();
			try {
				StartMeasure(constraint);
				return MeasureStrategy.Measure(constraint);
			}
			finally {
				if (Orientation == System.Windows.Controls.Orientation.Horizontal)
					TryCorrectHorizontalOffset();
				else
					TryCorrectVerticalOffset();
				EndMeasure();
			}
		}
		private void TryCorrectVerticalOffset() {
			var line = MeasureStrategy.GetContainedLine(MinVisibleIndex);
			if (MeasureStrategy.OffsetTokenIndex > Items.Count) {
				MeasureStrategy.DestroyLines();
				SetVerticalOffset(0);
			}
			else if (line == null) {
				var firstLine = MeasureStrategy.GetLine(0);
				if (firstLine != null && firstLine.Index <= 0) {
					scrollData.ExtentSize = CalcExtentSize();
					scrollData.VerticalOffset = EnsureVerticalOffset(VerticalOffset + MeasureStrategy.CalcMaxLineHeight());
					PreviousVerticalOffset = VerticalOffset;
					InvalidateScrollInfo();
					MeasureStrategy.RenumerateLines();
				}
			}
			else if (line.Index != 0) {
				scrollData.ExtentSize = CalcExtentSize();
				MeasureStrategy.DestroyLines();
				var lineHeight = MeasureStrategy.CalcMaxLineHeight();
				if (line.Index < 0)
					scrollData.VerticalOffset = EnsureVerticalOffset(VerticalOffset + lineHeight);
				else
					scrollData.VerticalOffset = EnsureVerticalOffset(VerticalOffset - lineHeight);
				InvalidateScrollInfo();
			}
			int offsetIndex = OffsetToIndex(VerticalOffset);
			if (!correctOffsetLocker.IsLocked && (StartMeasureIndex != offsetIndex || IndexToOffset(StartMeasureIndex) != PreviousTokenStartPosition) && Math.Abs(PreviousVerticalOffset - VerticalOffset) < ViewportHeight) {
				correctOffsetLocker.LockOnce();
				scrollData.ExtentSize = CalcExtentSize();
				scrollData.VerticalOffset = EnsureVerticalOffset(IndexToOffset(StartMeasureIndex) + (VerticalOffset - PreviousTokenStartPosition));
				PreviousVerticalOffset = scrollData.VerticalOffset;
				InvalidateScrollInfo();
				MeasureStrategy.RenumerateLines();
			}
			else
				correctOffsetLocker.Unlock();
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (Owner == null || MeasureStrategy == null) return finalSize;
			UpdateScrollData(finalSize);
			var arranged = MeasureStrategy.Arrange(finalSize);
			EndArrange(arranged);
			Clip = new RectangleGeometry(new Rect(0, 0, finalSize.Width, finalSize.Height));
			return finalSize;
		}
		void ScrollVertical(bool up) {
			int increment = up ? -1 : 1;
			var line = MeasureStrategy.GetLineByAbsolutIndex(OffsetToIndex(VerticalOffset) + increment);
			if (line != null && line.Tokens.Count > 0) {
				BringIntoViewByIndex(line.Tokens[0].VisibleIndex);
			}
		}
		Rect MakeVisibleHorizontal(Rect rect) {
			if (rect.X < 0)
				SetHorizontalOffset(HorizontalOffset - Math.Abs(rect.X) + Epsilon);
			else if (rect.Right > Owner.ActualWidth) {
				SetHorizontalOffset(HorizontalOffset + (rect.Right - Owner.ActualWidth));
				UpdateLayout();
				InvalidateMeasure();
			}
			return rect;
		}
		Rect MakeVisibleVertical(Rect rect) {
			if (rect.Y < 0)
				SetVerticalOffset(VerticalOffset - Math.Abs(rect.Y) + Epsilon);
			else if (rect.Bottom > Owner.ActualHeight) {
				SetVerticalOffset(VerticalOffset + (rect.Bottom - Owner.ActualHeight));
				UpdateLayout();
				InvalidateMeasure();
			}
			return rect;
		}
		Point prevMovePoint;
		bool isLeftScrollDirection = false;
		bool isUpScrollDirection = false;
		void OnTokenEditorPreviewMouseMove(object sender, MouseEventArgs e) {
			var current = e.GetPosition(this);
			isLeftScrollDirection = current.X < prevMovePoint.X;
			isUpScrollDirection = current.Y < prevMovePoint.Y;
			prevMovePoint = current;
		}
		void OnTokenEditorPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			prevMovePoint = e.GetPosition(this);
		}
		private object CoerceTokenContainerTemplate(object baseValue) {
			return baseValue == null ? TokenContainerTemplate : baseValue;
		}
		void TokenEditorPanel_Loaded(object sender, RoutedEventArgs e) {
			UpdateMeasureStrategy();
			correctOffsetLocker.LockOnce();
			MeasureStrategy.DestroyLines();
			SetHorizontalOffset(0);
			SetVerticalOffset(0);
			InvalidateMeasure();
		}
		void TryHandlesVerticalChange(DragDeltaEventArgs e) {
			if ((e.VerticalChange < 0 && !isUpScrollDirection) || (isUpScrollDirection && e.VerticalChange > 0)) {
				e.Handled = true;
			}
		}
		void TryHandlesHorizontalChange(DragDeltaEventArgs e) {
			if ((e.HorizontalChange < 0 && !isLeftScrollDirection) || (isLeftScrollDirection && e.HorizontalChange > 0)) {
				e.Handled = true;
				SetHorizontalOffset(isLeftScrollDirection ? HorizontalOffset - 1 : HorizontalOffset + 1);
			}
		}
		int GetMaxIndex() {
			return Orientation == System.Windows.Controls.Orientation.Horizontal ? MeasureStrategy.MaxVisibleIndex : MaxLineIndex;
		}
		double GetTokenSize(int visibleIndex) {
			return Orientation == System.Windows.Controls.Orientation.Horizontal ? GetTokenWidth(visibleIndex) : GetLineHeight(visibleIndex);
		}
		double GetLineHeight(int lineIndex) {
			return GetLine(lineIndex).Return(x => x.Height, () => MeasureStrategy.CalcMaxLineHeight());
		}
		TokenEditorLineInfo GetLine(int lineIndex) {
			return MeasureStrategy.GetLineByAbsolutIndex(lineIndex);
		}
		double GetTokenWidth(int visibleIndex) {
			if (IsDefaultTokenIndex(visibleIndex) && !CanShowDefaultToken)
				return 0;
			return GetTokenByVisibleIndex(visibleIndex).Return(x => x.DesiredSize.Width, () => TokenMagicWidth);
		}
		void UpdateDefaultTokenPresenter() {
			if (DefaultTokenPresenter == null) return;
			DefaultTokenPresenter.Item = new CustomItem();
			DefaultTokenPresenter.NullText = Owner.GetNullText();
			DefaultTokenPresenter.UpdateEditorEditValue();
		}
		void OnShowTokenButtonsChanged() {
			ClearTokens();
		}
		void OnTokenButtonsChanged() {
			ClearTokens();
		}
		void OnTokenContainerTemplateChanged() {
			for (int i = 1; i < InternalChildren.Count; i++) {
				var presenter = InternalChildren[i] as TokenEditorPresenter;
				presenter.BorderTemplate = TokenContainerTemplate;
			}
			ClearTokens();
		}
		void ClearTokens() {
			InvalidateMeasure();
		}
		void OnItemTemplateChanged(ControlTemplate newValue) {
			InvalidateMeasure();
		}
		void UpdateItems(List<CustomItem> oldValue, List<CustomItem> newValue) {
			var newItems = new List<CustomItem>();
			for (int i = 0; i < newValue.Count; i++) {
				var value = newValue[i];
				var editValue = value.EditValue;
				if (editValue is LookUpEditableItem)
					editValue = ((LookUpEditableItem)value.EditValue).EditValue;
				if (!(editValue == null && string.IsNullOrEmpty(value.DisplayText))) {
					newItems.Add(new CustomItem() { EditValue = editValue, DisplayText = value.DisplayText });
				}
			}
			if ((Items == null && newItems != null) || (Items != null && newItems == null) || Items.Count != newItems.Count)
				MeasureStrategy.Do(x => x.DestroyLines());
			Items = newItems;
		}
		void OnDisplayItemsChanged(List<CustomItem> oldValue, List<CustomItem> newValue) {
			UpdateItems(oldValue, newValue);
			InvalidateMeasure();
		}
		Locker correctOffsetLocker = new Locker();
		void TryCorrectHorizontalOffset() {
			int offsetIndex = OffsetToIndex(HorizontalOffset);
			if (!correctOffsetLocker.IsLocked && (StartMeasureIndex != offsetIndex || IndexToOffset(StartMeasureIndex) != PreviousTokenStartPosition) && Math.Abs(PreviousHorizontalOffset - HorizontalOffset) < ViewportWidth) {
				correctOffsetLocker.LockOnce();
				scrollData.ExtentSize = CalcExtentSize();
				scrollData.HorizontalOffset = EnsureHorizontalOffset(IndexToOffset(StartMeasureIndex) + (HorizontalOffset - PreviousTokenStartPosition));
				PreviousHorizontalOffset = scrollData.HorizontalOffset;
				InvalidateScrollInfo();
			}
			else
				correctOffsetLocker.Unlock();
		}
		private double CalcLocation(Track track, double wholeRange, double viewport, double offset, out double thumbLength) {
			double length = wholeRange + viewport;
			thumbLength = track.ActualWidth * viewport / length;
			double trackLength = track.ActualWidth - thumbLength;
			double decreaseButtonLength = trackLength * offset / wholeRange;
			double increaseButtonLength = trackLength - decreaseButtonLength;
			var location = track.IsDirectionReversed ? increaseButtonLength : decreaseButtonLength;
			return location;
		}
		bool IsTokenActivated(int index) {
			var container = measuredItems.ContainsKey(index) ? measuredItems[index] : null;
			return IsEditorActivated(container);
		}
		bool IsEditorActivated(UIElement container) {
			return ((TokenEditorPresenter)container).IsEditorActivated;
		}
		double CalcVerticalViewport(Size constraint) {
			return constraint.Height;
		}
		bool CalcStopCriteria2(int index, double wholeHeight, double viewport) {
			return wholeHeight < viewport && index < Items.Count;
		}
		void EndMeasure() {
			foreach (var key in previousMeasuredItems.Keys) {
				var container = previousMeasuredItems[key];
				if (container != DefaultTokenPresenter) {
					((TokenEditorPresenter)container).Clear();
					recycled.Push(container);
				}
			}
			previousMeasuredItems.Clear();
		}
		void StartMeasure(Size constraint) {
			if (ShouldScrollToEnd) {
				ClearMeasuredItems();
				ShouldScrollToEnd = false;
				double offsetDelta = 0;
				int index = MeasureStrategy.MeasureFromEnd(constraint, out offsetDelta);
				var offset = IndexToOffset(index) + offsetDelta;
				if (Orientation == System.Windows.Controls.Orientation.Horizontal)
					SetHorizontalOffset(offset);
				else {
					scrollData.ExtentSize = CalcExtentSize();
					scrollData.VerticalOffset = EnsureVerticalOffset(GetMaxVerticalOffset());
					PreviousVerticalOffset = scrollData.VerticalOffset;
					InvalidateScrollInfo();
					MeasureStrategy.RenumerateLines();
					MeasureStrategy.DestroyLines();
				}
			}
			CalcStartMeasureIndex();
			ClearMeasuredItems();
		}
		void ClearMeasuredItems() {
			var value = measuredItems.Where(x => x.Value == DefaultTokenPresenter).Select(x => x).FirstOrDefault();
			if (value.Value != null)
				measuredItems.Remove(value.Key);
			previousMeasuredItems.AddRange(measuredItems);
			measuredItems.Clear();
		}
		void CalcStartMeasureIndex() {
			StartMeasureIndex = OffsetToIndex(Orientation == System.Windows.Controls.Orientation.Horizontal ? HorizontalOffset : VerticalOffset);
			PreviousTokenStartPosition = IndexToOffset(StartMeasureIndex);
		}
		void PrepareContainer(UIElement container, CustomItem item) {
			TokenEditorPresenter presenter = (TokenEditorPresenter)container;
			if (!presenter.IsEditorActivated) {
				presenter.Item = item;
				presenter.UpdateEditorEditValue();
			}
		}
		bool CalcStopCriteria(int index, double wholeWidth, double viewport) {
			return wholeWidth < viewport && index < Items.Count;
		}
		Dictionary<int, UIElement> measuredItems = new Dictionary<int, UIElement>();
		Dictionary<int, UIElement> previousMeasuredItems = new Dictionary<int, UIElement>();
		Stack<UIElement> recycled = new Stack<UIElement>();
		UIElement GenerateContainer(int index, CustomItem value) {
			UIElement container = null;
			if (previousMeasuredItems.TryGetValue(index, out container)) {
				previousMeasuredItems.Remove(index);
			}
			else {
				if (recycled.Count > 0) container = recycled.Pop();
				else {
					container = CreateTokenPresenter();
					InternalChildren.Add(container);
				}
			}
			return container;
		}
		TokenEditorPresenter CreateTokenPresenter() {
			return new TokenEditorPresenter() {
				DeleteItemButtonTemplate = DeleteItemButtonTemplate,
				BorderTemplate = Owner.TokenBorderTemplate,
				ShowButtons = ShowTokenButtons,
				TokenButtons = TokenButtons,
				IsTextEditable = Owner.CanActivateToken(),
				TokenTextTrimming = Owner.TokenTextTrimming
			};
		}
		void CreateDefaultTokenEditor() {
			DefaultTokenPresenter = new TokenEditorPresenter() {
				BorderTemplate = EmptyTokenContainerTemplate,
				ShowBorder = false,
				ShowButtons = false,
				IsTextEditable = Owner.ShowDefaultToken,
				NullText = Owner.GetNullText(),
				Item = new CustomItem(),
				IsNewTokenEditorPresenter = true
			};
			InternalChildren.Add(DefaultTokenPresenter);
			DefaultTokenPresenter.UpdateEditor();
		}
		void EndArrange(List<UIElement> arranged) {
			for (int i = 0; i < InternalChildren.Count; i++) {
				var container = InternalChildren[i];
				if (!arranged.Contains(container)) container.Arrange(new Rect(-1, 0, 0, 0));
			}
		}
		bool IsValueEquals(TokenItemData data, CustomItem item) {
			var editValue = item.EditValue;
			if (editValue != null && editValue is LookUpEditableItem)
				return ((LookUpEditableItem)editValue).EditValue == data.Value && item.DisplayText == data.DisplayText;
			return item.EditValue == data.Value && item.DisplayText == data.DisplayText;
		}
		double Floor(double value) {
			double rounded = Math.Round(value);
			if (AreClose(rounded, value))
				return rounded;
			return Math.Floor(value);
		}
		void SetHorizontalOffsetInternal(double offset) {
			scrollData.HorizontalOffset = EnsureHorizontalOffset(offset);
			InvalidateScrollInfo();
		}
		void SetVerticalOffsetInternal(double offset) {
			scrollData.VerticalOffset = EnsureVerticalOffset(offset);
			InvalidateScrollInfo();
		}
		double EnsureVerticalOffset(double offset) {
			offset = Math.Max(0, offset);
			if (offset + ViewportHeight > ExtentHeight)
				offset = GetMaxVerticalOffset();
			return offset;
		}
		double EnsureHorizontalOffset(double offset) {
			offset = Math.Max(0, offset);
			if (offset + ViewportWidth > ExtentWidth)
				offset = GetMaxHorizontalOffset();
			return offset;
		}
		void InvalidateScrollData(Size viewport, Size extent) {
			scrollData.ViewportSize = viewport;
			scrollData.ExtentSize = extent;
			ScrollOwner.Do(x => x.InvalidateScrollInfo());
		}
		void InvalidateScrollInfo() {
			ScrollOwner.Do(x => {
				x.InvalidateScrollInfo();
			});
		}
		bool AreClose(double value1, double value2) {
			return Math.Abs(value1 - value2) < Epsilon;
		}
		void UpdateScrollData(Size size) {
			var viewport = CalcViewport(size);
			var extent = CalcExtentSize();
			if (ShouldEnsureHorizontalOffset) {
				ShouldEnsureHorizontalOffset = false;
				scrollData.HorizontalOffset = EnsureHorizontalOffset(HorizontalOffset);
			}
			if (extent.Width <= viewport.Width)
				scrollData.HorizontalOffset = 0;
			if (extent.Height <= viewport.Height)
				scrollData.VerticalOffset = 0;
			InvalidateScrollData(viewport, extent);
		}
		bool IsChildRectIntoOwner(TokenEditorPresenter token, Rect ownerRect) {
			var childRect = LayoutHelper.GetRelativeElementRect(token, this);
			return ownerRect.Contains(childRect);
		}
		Size CalcViewport(Size size) {
			return size;
		}
		Size CalcExtentSize() {
			return Orientation == System.Windows.Controls.Orientation.Horizontal ? CalcHorizontalExtentSize() : CalcVerticalExtentSize();
		}
		Size CalcVerticalExtentSize() {
			int linesCount = 0;
			double height = 0;
			double lineHeight = MeasureStrategy.CalcMaxLineHeight();
			var firstLine = MeasureStrategy.GetLine(0);
			double heightBefore = 0;
			if (firstLine != null)
				heightBefore = IndexToOffset(firstLine.Index);
			else
				heightBefore = ViewportHeight;
			double linesBefore = heightBefore / lineHeight;
			height = linesBefore * lineHeight + MeasureStrategy.CalcLinesHeight();
			var line = MeasureStrategy.GetLine(MeasureStrategy.LinesCount - 1);
			int lastIndex = line.Return(x => x.Tokens.Count > 0 ? x.Tokens[x.Tokens.Count - 1].VisibleIndex : 0, () => 0);
			double linesAfter = Math.Ceiling(((double)(Items.Count - lastIndex) / TokensInLine));
			height += linesAfter * lineHeight;
			linesCount = (int)(linesBefore + linesAfter + MeasureStrategy.LinesCount);
			MaxLineIndex = linesCount;
			return new Size(ViewportWidth, height);
		}
		Size CalcHorizontalExtentSize() {
			double width = 0;
			for (int i = 0; i <= MeasureStrategy.MaxVisibleIndex; i++) {
				width += GetTokenSize(i);
			}
			return new Size(width, ViewportHeight);
		}
		double GetMaxHorizontalOffset() {
			return Math.Max(0, ExtentWidth - ViewportWidth);
		}
		double GetMaxVerticalOffset() {
			return Math.Max(0, ExtentHeight - ViewportHeight);
		}
		List<int> GetIndexesInLine(int index, bool after) {
			List<int> indexes = new List<int>();
			var line = MeasureStrategy.GetContainedLine(index);
			if (line != null) {
				foreach (var token in line.Tokens) {
					if (token.VisibleIndex >= index && after)
						indexes.Add(token.VisibleIndex);
					else if (token.VisibleIndex <= index && !after)
						indexes.Add(token.VisibleIndex);
				}
			}
			return indexes;
		}
		internal bool BringIntoViewByIndex(int index) {
			var container = GetTokenByVisibleIndex(index);
			BringIntoView(container);
			return container != null;
		}
		internal TokenEditorPresenter GetTokenByEditableIndex(int index) {
			UIElement container = null;
			measuredItems.TryGetValue(index, out container);
			return container as TokenEditorPresenter;
		}
		internal int GetEditableIndexOfContainer(TokenEditorPresenter container) {
			if (container == null) return -1;
			if (container.Equals(DefaultTokenPresenter)) return Items.Count;
			return measuredItems.ContainsValue(container) ? measuredItems.Where(x => x.Value.Equals(container)).Select(x => x.Key).FirstOrDefault() : -1;
		}
		internal IList<UIElement> GetContainers() {
			return measuredItems.Values != null ? measuredItems.Values.ToList() : new List<UIElement>();
		}
		internal bool ShouldScroll(int index) {
			var container = GetTokenByEditableIndex(MeasureStrategy.ConvertToEditableIndex(index));
			if (container == null) return true;
			var containerBounds = LayoutHelper.GetRelativeElementRect(container, this);
			var bounds = new Rect(0, 0, ActualWidth, ActualHeight);
			return !bounds.Contains(containerBounds);
		}
		internal int GetVisibleIndex(TokenEditorPresenter container) {
			return MeasureStrategy.ConvertToVisibleIndex(GetEditableIndexOfContainer(container));
		}
		internal void Clear() {
			DefaultTokenPresenter = null;
			InternalChildren.Clear();
			measuredItems.Clear();
			previousMeasuredItems.Clear();
			recycled.Clear();
		}
		internal Dictionary<int, UIElement> GetVisibleTokens() {
			return measuredItems;
		}
		internal bool IsLastToken(TokenEditorPresenter token) {
			int index = MeasureStrategy.ConvertToVisibleIndex(GetEditableIndexOfContainer(token));
			return MeasureStrategy.IsMaxVisibleIndex(index);
		}
		internal bool IsFirstToken(TokenEditorPresenter token) {
			int index = MeasureStrategy.ConvertToVisibleIndex(GetEditableIndexOfContainer(token));
			return MeasureStrategy.IsMinVisibleIndex(index);
		}
		internal bool IsDefaultTokenIndex(int visibleIndex) {
			return MeasureStrategy.ConvertToEditableIndex(visibleIndex) == GetEditableIndexOfContainer(DefaultTokenPresenter);
		}
		internal TokenEditorPresenter GetTokenByVisibleIndex(int visibleIndex) {
			return GetTokenByEditableIndex(MeasureStrategy.ConvertToEditableIndex(visibleIndex));
		}
		internal int ConvertToEditableIndex(int visibleIndex) {
			return MeasureStrategy.ConvertToEditableIndex(visibleIndex);
		}
		internal int ConvertToVisibleIndex(int editableIndex) {
			return MeasureStrategy.ConvertToVisibleIndex(editableIndex);
		}
		internal void AddDefaultTokenContainerToMeasured() {
			measuredItems.Add(Items.Count, DefaultTokenPresenter);
		}
		internal void CalcRelativeOffsetAndIndex(ref double x, ref int index) {
			index = OffsetToIndex(HorizontalOffset);
			x = IndexToOffset(index) - HorizontalOffset;
			var tokens = measuredItems.Keys.Select(k => MeasureStrategy.ConvertToVisibleIndex(k)).OrderBy(k => k).ToList();
			int from = tokens.Count > 0 ? tokens[0] : 0;
			for (int i = from; i < index; i++) {
				x -= GetTokenSize(i);
			}
			index = from;
		}
		internal void RemoveFromMeasure(int index) {
			var editableIndex = MeasureStrategy.ConvertToEditableIndex(index);
			if (measuredItems.ContainsKey(editableIndex)) {
				recycled.Push(measuredItems[editableIndex]);
				measuredItems.Remove(editableIndex);
			}
		}
		internal bool IsTokenInFirstLine(TokenEditorPresenter token) {
			int index = MeasureStrategy.ConvertToVisibleIndex(GetEditableIndexOfContainer(token));
			if (index > -1) {
				var line = MeasureStrategy.GetLineByAbsolutIndex(OffsetToIndex(VerticalOffset));
				if (line != null)
					return IsFirstLineVisible() && line.Tokens.Where(x => x.VisibleIndex == index).FirstOrDefault() != null;
			}
			return false;
		}
		internal bool IsTokenInEndLine(TokenEditorPresenter token) {
			int index = MeasureStrategy.ConvertToVisibleIndex(GetEditableIndexOfContainer(token));
			if (index > -1) {
				var line = MeasureStrategy.GetLineByAbsolutIndex(OffsetToIndex(VerticalOffset + ViewportHeight));
				if (line != null)
					return IsEndLineVisible() && line.Tokens.Where(x => x.VisibleIndex == index).FirstOrDefault() != null;
			}
			return false;
		}
		internal bool IsFirstLineVisible() {
			int index = MeasureStrategy.GetLineByAbsolutIndex(OffsetToIndex(VerticalOffset)).Return(x => x.Index, () => -1);
			return index == 0;
		}
		internal bool IsEndLineVisible() {
			var line = MeasureStrategy.GetContainedLine(MeasureStrategy.MaxVisibleIndex);
			if (line != null) {
				var token = GetTokenByVisibleIndex(line.Tokens[0].VisibleIndex);
				var bounds = LayoutHelper.GetRelativeElementRect(token, this);
				return ContainsBounds(bounds);
			}
			return false;
		}
		private bool ContainsBounds(Rect bounds) {
			var ownBounds = new Rect(0, 0, ActualWidth, ActualHeight);
			return LessOrClose(ownBounds.X, bounds.X) && LessOrClose(ownBounds.Y, bounds.Y) &&
				   GreaterOrClose(ownBounds.Right, bounds.Right) && GreaterOrClose(ownBounds.Bottom, bounds.Bottom);
		}
		private bool GreaterOrClose(double p1, double p2) {
			return p1 > p2 || AreClose(p1, p2);
		}
		private bool LessOrClose(double p1, double p2) {
			return p1 < p2 || AreClose(p1, p2);
		}
		internal int GetFirstTokenInLineByTokenIndex(int index, bool up) {
			int lineIndex = MeasureStrategy.GetContainedLine(index).Return(x => x.Index, () => -1);
			if (lineIndex > -1) {
				int increment = up ? -1 : 1;
				var line = MeasureStrategy.GetLineByAbsolutIndex(lineIndex + increment);
				if (line != null)
					return line.Tokens.Count > 0 ? line.Tokens[0].VisibleIndex : -1;
			}
			return -1;
		}
		internal int GetFirstTokenInLine(int lineIndex) {
			return MeasureStrategy.GetLineByAbsolutIndex(lineIndex).Return(x => x.Tokens.Count > 0 ? x.Tokens[0].VisibleIndex : -1, () => -1);
		}
		internal void ScrollToVerticalStart() {
			SetVerticalOffset(0);
		}
		internal void ScrollToVerticalEnd() {
			ShouldScrollToEnd = true;
			InvalidateMeasure();
		}
		internal List<int> GetIndexesInLineAfterToken(int index) {
			return GetIndexesInLine(index, true);
		}
		internal List<int> GetIndexesInLineBeforeToken(int index) {
			return GetIndexesInLine(index, false);
		}
		internal List<int> GetIndexesInLine(int index) {
			List<int> result = new List<int>();
			var line = MeasureStrategy.GetContainedLine(index);
			if (line != null) {
				foreach (var token in line.Tokens)
					result.Add(token.VisibleIndex);
			}
			return result;
		}
		internal TokenEditorLineInfo GetLineRelativeToken(int index, bool up) {
			var line = MeasureStrategy.GetContainedLine(index);
			int increment = up ? -1 : 1;
			if (line != null) {
				int lineIndex = Math.Min(Math.Max(MinVisibleIndex, MeasureStrategy.GetIndexOfLine(line) + increment), MaxVisibleIndex);
				return MeasureStrategy.GetLine(lineIndex);
			}
			return null;
		}
		internal double GetTokenMaxWidth() {
			return Owner.TokenMaxWidth;
		}
		internal void OnActiveTokenEditValueChanged(TokenEditorPresenter token) {
			BringIntoView(token);
			MeasureStrategy.DestroyLines();
		}
		internal void OnThumbMouseMove(MouseEventArgs e) {
			var track = ((IScrollBarThumbDragDeltaListener)Owner).ScrollBar.Track;
			var thumb = track.Thumb;
			if (Orientation == System.Windows.Controls.Orientation.Horizontal && e.MouseDevice.LeftButton == MouseButtonState.Pressed) {
				var point = e.GetPosition(track);
				var newPoint = e.GetPosition(thumb);
				double thumbLength = 0;
				var location = CalcLocation(track, ExtentWidth - ViewportWidth, ViewportWidth, HorizontalOffset, out thumbLength);
				if (point.X < location)
					newPoint.X = newPoint.X + (location - point.X) / 5;
				else if (point.X > location + thumbLength)
					newPoint.X = newPoint.X - (point.X - (location + thumbLength)) / 5;
				typeof(Thumb).GetField("_originThumbPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(thumb, newPoint);
			}
		}
		#region IScrollInfo Members
		class ScrollData {
			public double HorizontalOffset { get; set; }
			public double VerticalOffset { get; set; }
			public Size ViewportSize { get; set; }
			public Size ExtentSize { get; set; }
			public double WheelSize { get { return 50; } }
			public bool CanHorizontallyScroll { get; set; }
			public bool CanVerticallyScroll { get; set; }
			public ScrollViewer ScrollOwner { get; set; }
		}
		ScrollData scrollData = new ScrollData();
		bool IScrollInfo.CanHorizontallyScroll { get; set; }
		bool IScrollInfo.CanVerticallyScroll { get; set; }
		void IScrollInfo.MouseWheelDown() {
		}
		void IScrollInfo.MouseWheelLeft() {
		}
		void IScrollInfo.MouseWheelRight() {
		}
		void IScrollInfo.MouseWheelUp() {
		}
		#endregion
	}
}
