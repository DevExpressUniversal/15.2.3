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
using System.Windows.Media.Animation;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows.Markup;
using System.Windows.Controls;
using DevExpress.Xpf.Utils.Native;
using System.Collections;
namespace DevExpress.Xpf.Core {
	public class CardsPanelInfo {
		CardsPanel panel;
		protected virtual CardsPanel Panel { get { return panel; } }
		public virtual Alignment Alignment { get { return Panel.CardAlignment; } }
		public virtual Orientation Orientation { get { return Panel.Orientation; } }
		public virtual SizeHelperBase SizeHelper { get { return SizeHelperBase.GetDefineSizeHelper(Orientation); } }
		public virtual double FixedSize { get { return Panel.FixedSize; } }
		public virtual int MaxCardCountInRow { get { return Panel.MaxCardCountInRow; } }
		public virtual double SeparatorThickness { get { return Panel.SeparatorThickness; } }
		public virtual Thickness CardMargin { get { return Panel.CardMargin; } }
		public CardsPanelInfo(CardsPanel panel) {
			this.panel = panel;
		}
	}
	public class CardLayoutCalculator {
		RowInfoCollection rowInfo;
		public List<LineInfo> RowSeparators { get { return rowInfo.RowSeparators; } }
		public int RowsCount { get { return rowInfo != null ? rowInfo.Count : 1; } }
		public RowInfoCollection Rows { get { return rowInfo; } }
		public Size MeasureElements(Size availableSize, CardsPanelInfo panelInfo, IList<UIElement> sortedChildren) {
			rowInfo = new RowInfoCollection(panelInfo);
			return rowInfo.Measure(availableSize, sortedChildren);
		}
		public IList<Rect> ArrangeElements(Size finalSize, IList<UIElement> sortedChildren) {
			if(rowInfo == null)
				return new List<Rect>();
			return rowInfo.Arrange(finalSize, sortedChildren);
		}
	}
	public class CardsPanel : OrderPanelBase {
		public const double DefaultFixedSize = double.NaN;
		public const int DefaultMaxCardCountInRow = int.MaxValue;
		public const Alignment DefaultCardAlignment = Alignment.Near;
		static readonly DependencyPropertyKey CardsSeparatorsPropertyKey;
		public static readonly DependencyProperty CardsSeparatorsProperty;
		public static readonly DependencyProperty CardRowProperty;
		public static readonly DependencyProperty FixedSizeProperty;
		public static readonly DependencyProperty MaxCardCountInRowProperty;
		public static readonly DependencyProperty CardAlignmentProperty;
		public static readonly DependencyProperty SeparatorThicknessProperty;
		public static readonly DependencyProperty CardMarginProperty;
		public static readonly DependencyProperty PaddingProperty;
		public static readonly DependencyProperty OwnerProperty;
		public const int InvalidCardRowIndex = int.MinValue;
		static CardsPanel() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CardsPanel), new FrameworkPropertyMetadata(typeof(CardsPanel)));
			FixedSizeProperty = DependencyProperty.Register("FixedSize", typeof(double), typeof(CardsPanel), new FrameworkPropertyMetadata(CardsPanel.DefaultFixedSize, FrameworkPropertyMetadataOptions.AffectsMeasure, null, null));
			MaxCardCountInRowProperty = DependencyProperty.Register("MaxCardCountInRow", typeof(int), typeof(CardsPanel), new FrameworkPropertyMetadata(CardsPanel.DefaultMaxCardCountInRow, FrameworkPropertyMetadataOptions.AffectsMeasure, null, ((d, baseValue) => (((int)baseValue <= 0) ? ((CardsPanel)d).MaxCardCountInRow : (int)baseValue))));
			CardAlignmentProperty = DependencyProperty.Register("CardAlignment", typeof(Alignment), typeof(CardsPanel), new FrameworkPropertyMetadata(CardsPanel.DefaultCardAlignment, FrameworkPropertyMetadataOptions.AffectsMeasure, null, null));
			SeparatorThicknessProperty = DependencyProperty.Register("SeparatorThickness", typeof(double), typeof(CardsPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, null, null));
			CardsSeparatorsPropertyKey = DependencyProperty.RegisterAttachedReadOnly("CardsSeparators", typeof(IEnumerable<CardsSeparator>), typeof(CardsPanel), new PropertyMetadata(null));
			CardsSeparatorsProperty = CardsSeparatorsPropertyKey.DependencyProperty;
			CardMarginProperty = DependencyProperty.Register("CardMargin", typeof(Thickness), typeof(CardsPanel), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsMeasure, null, null));
			PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(CardsPanel), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsMeasure, null, null));
			CardRowProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedProperty<CardsPanel, int>("CardRow", InvalidCardRowIndex, FrameworkPropertyMetadataOptions.Inherits, null, null);
			OwnerProperty = DependencyProperty.Register("Owner", typeof(ICardsPanelOwner), typeof(CardsPanel), new FrameworkPropertyMetadata(null));
		}
		CardLayoutCalculator layoutCalculator = new CardLayoutCalculator();
		CardsPanelInfo panelInfo;
		Dictionary<int, CardsSeparator> cache = new Dictionary<int, CardsSeparator>();
		public static void SetCardRow(DependencyObject dependencyObject, int rowIndex) {
			dependencyObject.SetValue(CardRowProperty, rowIndex);
		}
		public static int GetCardRow(DependencyObject dependencyObject) {
			if(dependencyObject == null)
				return InvalidCardRowIndex;
			return (int)dependencyObject.GetValue(CardRowProperty);
		}
		public double FixedSize {
			get { return (double)GetValue(FixedSizeProperty); }
			set { SetValue(FixedSizeProperty, value); }
		}
		public int MaxCardCountInRow {
			get { return (int)GetValue(MaxCardCountInRowProperty); }
			set { SetValue(MaxCardCountInRowProperty, value); }
		}
		public Alignment CardAlignment {
			get { return (Alignment)GetValue(CardAlignmentProperty); }
			set { SetValue(CardAlignmentProperty, value); }
		}
		public double SeparatorThickness {
			get { return (double)GetValue(SeparatorThicknessProperty); }
			set { SetValue(SeparatorThicknessProperty, value); }
		}
		public Thickness CardMargin {
			get { return (Thickness)GetValue(CardMarginProperty); }
			set { SetValue(CardMarginProperty, value); }
		}
		public Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}
		public IEnumerable<CardsSeparator> CardsSeparators {
			get { return (IEnumerable<CardsSeparator>)GetValue(CardsSeparatorsProperty); }
			internal set { SetValue(CardsSeparatorsPropertyKey, value); }
		}
		public ICardsPanelOwner Owner {
			get { return (ICardsPanelOwner)GetValue(OwnerProperty); }
			set { SetValue(OwnerProperty, value); }
		}
		public CardsPanel() {
			panelInfo = new CardsPanelInfo(this);
		}
		void AssignToOwner() {
			if(SizeHelper.GetDefineSize(new Size(this.ActualWidth, this.ActualHeight)) == 0) {
				return;
			}
			if(Owner != null) {
				Owner.ActualizePanels();
				if(!Owner.Panels.Contains(this)) {
					Owner.Panels.Add(this);
				}
			}
		}
		protected virtual void UpdateCardRows(IList<UIElement> sortedChildren) {
			int cardIndex = 0;
			if(layoutCalculator.Rows != null) {
				for(int rowIndex = 0; rowIndex < layoutCalculator.Rows.Count; rowIndex++) {
					for(int colIndex = 0; colIndex < layoutCalculator.Rows[rowIndex].ElementCount; colIndex++, cardIndex++) {
						if(sortedChildren[cardIndex] != null)
							SetCardRow(sortedChildren[cardIndex], rowIndex);
					}
				}
			}
		}
		protected override Size MeasureSortedChildrenOverride(Size availableSize, IList<UIElement> sortedChildren) {
			AssignToOwner();
			Size size = availableSize;
			size.Width -= Padding.Left + Padding.Right;
			size.Height -= Padding.Bottom + Padding.Top;
			return layoutCalculator.MeasureElements(size, panelInfo, sortedChildren);
		}
		protected override Size ArrangeSortedChildrenOverride(Size finalSize, IList<UIElement> sortedChildren) {
			Size actualFinalSize = finalSize;
			actualFinalSize.Height = Math.Max(0, actualFinalSize.Height - Padding.Top - Padding.Bottom);
			actualFinalSize.Width = Math.Max(0, actualFinalSize.Width - Padding.Left - Padding.Right);
			IList<Rect> rects = layoutCalculator.ArrangeElements(actualFinalSize, sortedChildren);
			for(int i = 0; i < rects.Count; i++) {
				Rect rect = rects[i];
				Point location = rect.Location;
				location.Offset(Padding.Left + CardMargin.Left, Padding.Top + CardMargin.Top);
				Size size = new Size(
					rect.Size.Width - (CardMargin.Left + CardMargin.Right),
					rect.Size.Height - (CardMargin.Top + CardMargin.Bottom));
				sortedChildren[i].Arrange(new Rect(location, size));
			}
			UpdateCardRows(sortedChildren);
			UpdateCardsSeparators();
			return finalSize;
		}
		SizeHelperBase GetSizeHelper() {
			return SizeHelperBase.GetDefineSizeHelper(Orientation);
		}
		void UpdateCardsSeparators() {
			IList<CardsSeparator> separators = new List<CardsSeparator>();
			for(int i = 0; i < layoutCalculator.RowSeparators.Count; i++) {
				LineInfo info = layoutCalculator.RowSeparators[i];
				Size size = GetSizeHelper().CreateSize(double.NaN, info.Length);
				Point location = GetSizeHelper().CreatePoint(GetSizeHelper().GetDefinePoint(info.Location), GetSizeHelper().GetSecondaryPoint(info.Location));
				CardsSeparator separator = GetSeparator(i);
				separator.Margin = new Thickness(location.X, location.Y, 0, 0);
				separator.Length = info.Length;
				separator.Orientation = this.Orientation;
				separators.Add(separator);
			}
			CardsSeparators = separators;
		}
		CardsSeparator GetSeparator(int index) {
			CardsSeparator separator;
			if(!cache.TryGetValue(index, out separator)) {
				separator = new CardsSeparator(index + 1);
				cache.Add(index, separator);
			}
			return separator;
		}
	}
	public interface ICardsPanelOwner {
		List<CardsPanel> Panels { get; }
		void ActualizePanels();
	}
}
