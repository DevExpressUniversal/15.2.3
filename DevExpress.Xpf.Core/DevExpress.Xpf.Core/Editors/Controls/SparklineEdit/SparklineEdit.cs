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

using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[ToolboxTabName(AssemblyInfo.DXTabData)]
	public class SparklineEdit : BaseEdit, IImageExportSettings {
		static readonly DependencyPropertyKey SparklineTypePropertyKey;
		public static readonly DependencyProperty SparklineTypeProperty;
		public static readonly DependencyProperty ItemsProperty;
		public static readonly DependencyProperty PointArgumentMemberProperty;
		public static readonly DependencyProperty PointValueMemberProperty;
		public static readonly DependencyProperty PointArgumentSortOrderProperty;
		public static readonly DependencyProperty FilterCriteriaProperty;
		public static readonly DependencyProperty PointArgumentRangeProperty;
		public static readonly DependencyProperty PointValueRangeProperty;
		static SparklineEdit() {
			Type ownerType = typeof(SparklineEdit);
			SparklineTypePropertyKey = DependencyPropertyManager.RegisterReadOnly("SparklineType", typeof(SparklineViewType), ownerType, new FrameworkPropertyMetadata(SparklineViewType.Line));
			SparklineTypeProperty = SparklineTypePropertyKey.DependencyProperty;
			ItemsProperty = DependencyPropertyManager.Register("Items", typeof(IEnumerable), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None,
					(o, args) => ((SparklineEdit)o).ItemsChanged((IEnumerable)args.OldValue, (IEnumerable)args.NewValue),
					(o, value) => ((SparklineEdit)o).CoerceItems(value)));
			PointArgumentMemberProperty = DependencyPropertyManager.Register("PointArgumentMember", typeof(string), ownerType,
				new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None, (o, args) => ((SparklineEdit)o).ArgumentMemberChanged((string)args.NewValue)));
			PointValueMemberProperty = DependencyPropertyManager.Register("PointValueMember", typeof(string), ownerType,
				new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None, (o, args) => ((SparklineEdit)o).ValueMemberChanged((string)args.NewValue)));
			PointArgumentSortOrderProperty = DependencyPropertyManager.Register("PointArgumentSortOrder", typeof(SparklineSortOrder), ownerType,
				new FrameworkPropertyMetadata(DefaultSortOrder, FrameworkPropertyMetadataOptions.None, (o, args) => ((SparklineEdit)o).PointArgumentSortOrderChanged((SparklineSortOrder)args.NewValue)));
			FilterCriteriaProperty = DependencyPropertyManager.Register("FilterCriteria", typeof(CriteriaOperator), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (o, args) => ((SparklineEdit)o).FilterCriteriaChanged((CriteriaOperator)args.NewValue)));
			PointArgumentRangeProperty = DependencyProperty.Register("PointArgumentRange", typeof(Range), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (o, args) => ((SparklineEdit)o).OnPointArgumentRangeChanged((Range)args.NewValue)));
			PointValueRangeProperty = DependencyProperty.Register("PointValueRange", typeof(Range), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (o, args) => ((SparklineEdit)o).OnPointValueRangeChanged((Range)args.NewValue)));
		}
		ItemsProviderChangedEventHandler<SparklineEdit> ItemsProviderChangedEventHandler { get; set; }
		const SparklineSortOrder DefaultSortOrder = SparklineSortOrder.Ascending;
		protected internal SparklineItemsProvider ItemsProvider { get; private set; }
		public string PointArgumentMember {
			get { return (string)GetValue(PointArgumentMemberProperty); }
			set { SetValue(PointArgumentMemberProperty, value); }
		}
		public string PointValueMember {
			get { return (string)GetValue(PointValueMemberProperty); }
			set { SetValue(PointValueMemberProperty, value); }
		}
		public SparklineSortOrder PointArgumentSortOrder {
			get { return (SparklineSortOrder)GetValue(PointArgumentSortOrderProperty); }
			set { SetValue(PointArgumentSortOrderProperty, value); }
		}
		public CriteriaOperator FilterCriteria {
			get { return (CriteriaOperator)GetValue(FilterCriteriaProperty); }
			set { SetValue(FilterCriteriaProperty, value); }
		}
		public IEnumerable Items {
			get { return (IEnumerable)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}
		public SparklineViewType SparklineType {
			get { return (SparklineViewType)GetValue(SparklineTypeProperty); }
			internal set { SetValue(SparklineTypePropertyKey, value); }
		}
		public Range PointArgumentRange {
			get { return (Range)GetValue(PointArgumentRangeProperty); }
			set { SetValue(PointArgumentRangeProperty, value); }
		}
		public Range PointValueRange {
			get { return (Range)GetValue(PointValueRangeProperty); }
			set { SetValue(PointValueRangeProperty, value); }
		}
		new SparklinePropertyProvider PropertyProvider { get { return base.PropertyProvider as SparklinePropertyProvider; } }
		protected internal override Type StyleSettingsType { get { return typeof(SparklineStyleSettings); } }
		protected internal override BaseEditStyleSettings CreateStyleSettings() {
			return new LineSparklineStyleSettings();
		}
		new SparklineEditStrategy EditStrategy { get { return base.EditStrategy as SparklineEditStrategy; } }
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new SparklinePropertyProvider(this);
		}
		public SparklineEdit() {
			this.SetDefaultStyleKey(typeof(SparklineEdit));
			ItemsProvider = CreateItemsProvider();
			ItemsProviderChangedEventHandler = new ItemsProviderChangedEventHandler<SparklineEdit>(this, (owner, o, e) => owner.EditStrategy.ItemProviderChanged(e));
		}
		#region IImageExportSettings Members
		FrameworkElement IImageExportSettings.SourceElement {
			get {
				RenderTargetBitmap bitmapSource = new RenderTargetBitmap(
					(int)this.ActualWidth, 
					(int)this.ActualHeight, 
					GraphicsDpi.DeviceIndependentPixel, 
					GraphicsDpi.DeviceIndependentPixel, 
					PixelFormats.Pbgra32
					);
				VisualBrush brush = new VisualBrush(this);
				DrawingVisual visual = new DrawingVisual();
				DrawingContext context = visual.RenderOpen();
				context.DrawRectangle(brush, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
				context.Close();
				bitmapSource.Render(visual);
				Image image = new Image();
				image.Source = bitmapSource;
				return image;
			}
		}
		ImageRenderMode IImageExportSettings.ImageRenderMode { get { return ImageRenderMode.UseImageSource; } }
		bool IImageExportSettings.ForceCenterImageMode { get { return false; } }
		object IImageExportSettings.ImageKey { get { return null; } }
		#endregion
		protected virtual SparklineItemsProvider CreateItemsProvider() {
			return new SparklineItemsProvider();
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new SparklineEditStrategy(this);
		}
		protected virtual void ItemsChanged(IEnumerable oldValue, IEnumerable newValue) {
			EditStrategy.ItemsChanged(oldValue, newValue);
		}
		protected virtual object CoerceItems(object baseValue) {
			return EditStrategy.CoerceItems(baseValue);
		}
		protected virtual void ArgumentMemberChanged(string argument) {
			EditStrategy.ArgumentMemberChanged(argument);
		}
		protected virtual void ValueMemberChanged(string newValue) {
			EditStrategy.ValueMemberChanged(newValue);
		}
		protected virtual void FilterCriteriaChanged(CriteriaOperator newCriteriaOperator) {
			EditStrategy.FilterCriteriaChanged(newCriteriaOperator);
		}
		protected virtual void PointArgumentSortOrderChanged(SparklineSortOrder newColumnSortOrder) {
			EditStrategy.PointArgumentSortOrderChanged(newColumnSortOrder);
		}
		protected virtual void OnPointValueRangeChanged(Range range) {
			EditStrategy.PointValueRangeChanged(range);
			AddLogicalChild(range);
		}
		protected virtual void OnPointArgumentRangeChanged(Range range) {
			EditStrategy.PointArgumentRangeChanged(range);
			AddLogicalChild(range);
		}
		protected internal virtual void UnsubscribeToItemsProviderChanged() {
			ItemsProvider.ItemsProviderChanged -= ItemsProviderChangedEventHandler.Handler;
		}
		protected internal virtual void SubscribeToItemsProviderChanged() {
			ItemsProvider.ItemsProviderChanged += ItemsProviderChangedEventHandler.Handler;
		}
		protected override void UnsubscribeFromSettings(BaseEditSettings settings) {
			base.UnsubscribeFromSettings(settings);
			if (settings != null)
				ItemsProvider.ItemsProviderChanged -= ItemsProviderChangedEventHandler.Handler;
		}
		protected override void SubscribeToSettings(BaseEditSettings settings) {
			base.SubscribeToSettings(settings);
			if (settings != null)
				ItemsProvider.ItemsProviderChanged += ItemsProviderChangedEventHandler.Handler;
		}
	}
}
