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
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid.Data;
#if SL
using ApplicationException = System.Exception;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	[
	TemplatePart(Name = FieldHeaders.TemplatePartEmptyText, Type = typeof(FrameworkElement)),
	TemplatePart(Name = FieldHeaders.TemplatePartPanel, Type = typeof(FieldHeadersPanel)),
	TemplatePart(Name = FieldHeaders.TemplatePartBestFitDecorator, Type = typeof(BestFitDecorator)),
	TemplatePart(Name = FieldHeaders.TemplatePartContainer, Type = typeof(FrameworkElement)),
	TemplateVisualState(Name = FieldHeaders.EmptyStateName),
	TemplateVisualState(Name = FieldHeaders.NonEmptyStateName),
	TemplateVisualState(Name = FieldHeaders.FullEmptyStateName),
	]
	public class FieldHeaders : FieldHeadersBase {
		const string EmptyStateName = "Empty", NonEmptyStateName = "NonEmpty", FullEmptyStateName = "FullEmpty";
		internal const string TemplatePartPanel = "PART_Panel", TemplatePartEmptyText = "PART_EmptyText",
			TemplatePartBestFitDecorator = "PART_HeaderBestFitControlDecorator",
			TemplatePartContainer = "PART_Container";
		#region static stuff
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty LeftOffsetProperty;
		public static readonly DependencyProperty LeftPixelsOffsetProperty;
		public static readonly DependencyProperty LeftPixelsProperty;
		public static readonly DependencyProperty MeasureModeProperty;
		protected static readonly DependencyPropertyKey LeftPixelsPropertyKey;
		public static readonly DependencyProperty EmptyTextVAlignmentProperty;
		public static readonly DependencyProperty EmptyTextHAlignmentProperty;
		public static readonly DependencyProperty EmptyTextVisibilityProperty;
		public static readonly DependencyProperty HeadersVAlignmentProperty;
		static FieldHeaders() {
			Type ownerType = typeof(FieldHeaders);
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(ControlTemplate),
				ownerType, new UIPropertyMetadata(null));
			EmptyTextVAlignmentProperty = DependencyPropertyManager.Register("EmptyTextVAlignment", typeof(VerticalAlignment),
				ownerType, new UIPropertyMetadata(VerticalAlignment.Center));
			EmptyTextHAlignmentProperty = DependencyPropertyManager.Register("EmptyTextHAlignment", typeof(HorizontalAlignment),
				ownerType, new UIPropertyMetadata(HorizontalAlignment.Left));
			EmptyTextVisibilityProperty = DependencyPropertyManager.Register("EmptyTextVisibility", typeof(Visibility),
				ownerType, new UIPropertyMetadata(Visibility.Visible, (d, e) => ((FieldHeaders)d).EnsureVisualState()));
			HeadersVAlignmentProperty = DependencyPropertyManager.Register("HeadersVAlignment", typeof(VerticalAlignment),
				ownerType, new UIPropertyMetadata(VerticalAlignment.Center));
			LeftOffsetProperty = DependencyPropertyManager.Register("LeftOffset", typeof(int),
				ownerType, new UIPropertyMetadata(0, (d,e) => ((FieldHeaders)d).OnLeftOffsetChanged()));
			LeftPixelsOffsetProperty = DependencyPropertyManager.Register("LeftPixelsOffset", typeof(double),
				ownerType, new UIPropertyMetadata(0d, (d,e) => ((FieldHeaders)d).OnLeftOffsetChanged()));
			LeftPixelsPropertyKey = DependencyPropertyManager.RegisterReadOnly("LeftPixels", typeof(int),
				ownerType, new UIPropertyMetadata(0));
			LeftPixelsProperty = LeftPixelsPropertyKey.DependencyProperty;
			MeasureModeProperty = DependencyPropertyManager.Register("MeasureMode", typeof(FieldHeadersMeasureMode),
				ownerType, new UIPropertyMetadata(FieldHeadersMeasureMode.Default));
		}   
		#endregion        
		List<PivotGridField> boundFields;
		List<PivotGridGroup> boundGroups;
		public FieldHeaders()
			: base() {
			this.boundFields = new List<PivotGridField>();
			this.boundGroups = new List<PivotGridGroup>();
			SetDefaultStyleKey();
			Loaded += FieldHeaders_Loaded;
			Unloaded += FieldHeaders_Unloaded;
		}
		bool subscribed;
		void FieldHeaders_Unloaded(object sender, RoutedEventArgs e) {
			UnsubscribeEvents();
		}
		protected virtual void UnsubscribeEvents() {
			if(subscribed && PivotGrid != null)
				PivotGrid.FieldSizeChanged -= PivotGrid_FieldSizeChanged;
		}
		void FieldHeaders_Loaded(object sender, RoutedEventArgs e) {
			SubscribeEvents();
		}
		protected virtual void SubscribeEvents() {
			if(!subscribed && PivotGrid != null) {
				PivotGrid.FieldSizeChanged += PivotGrid_FieldSizeChanged;
				subscribed = true;
			}
		}
		void PivotGrid_FieldSizeChanged(object sender, PivotFieldEventArgs e) {
			if(Area == FieldListArea.RowArea && e.Field != null && e.Field.Area == FieldArea.RowArea && e.Field.Visible && PivotGrid != null &&
				PivotGrid.RowTotalsLocation != FieldRowTotalsLocation.Tree && FieldsSource is RealFieldsSource && Panel != null &&
				PivotGrid.PivotGridScroller.Cells.ActualWidth == 0)
					Panel.InvalidateMeasure();
		}
		protected virtual void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(FieldHeaders));
		}
		protected internal FieldHeadersPanel Panel { get; private set; }
		protected internal FrameworkElement EmptyAreaTextElement { get; private set; }
		protected internal System.Windows.Controls.Control Container { get; private set; }
		public ControlTemplate ContentTemplate {
			get { return (ControlTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public VerticalAlignment EmptyTextVAlignment {
			get { return (VerticalAlignment)GetValue(EmptyTextVAlignmentProperty); }
			set { SetValue(EmptyTextVAlignmentProperty, value); }
		}
		public HorizontalAlignment EmptyTextHAlignment {
			get { return (HorizontalAlignment)GetValue(EmptyTextHAlignmentProperty); }
			set { SetValue(EmptyTextHAlignmentProperty, value); }
		}
		public Visibility EmptyTextVisibility {
			get { return (Visibility)GetValue(EmptyTextVisibilityProperty); }
			set { SetValue(EmptyTextVisibilityProperty, value); }
		}
		public VerticalAlignment HeadersVAlignment {
			get { return (VerticalAlignment)GetValue(HeadersVAlignmentProperty); }
			set { SetValue(HeadersVAlignmentProperty, value); }
		}
		public int LeftOffset {
			get { return (int)GetValue(LeftOffsetProperty); }
			set { SetValue(LeftOffsetProperty, value); }
		}
		public double LeftPixelsOffset {
			get { return (double)GetValue(LeftPixelsOffsetProperty); }
			set { SetValue(LeftPixelsOffsetProperty, value); }
		}		
		public int LeftPixels {
			get { return (int)GetValue(LeftPixelsProperty); }
			private set { this.SetValue(LeftPixelsPropertyKey, value); }
		}
		public FieldHeadersMeasureMode MeasureMode {
			get { return (FieldHeadersMeasureMode)GetValue(MeasureModeProperty); }
			set { SetValue(MeasureModeProperty, value); }
		}
		protected override void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e) {
			base.OnItemsSourceChanged(e);
			BindHeaders(ItemsSource);
		}
		protected void BindHeaders(IList<PivotGridField> fields) {
			if(!AllowBind(fields) || !AreFieldsChanged(fields))
				return;
			if(PivotGrid != null && PivotGrid.IsDesignMode && MeasureMode == FieldHeadersMeasureMode.Cut)
				MeasureMode = FieldHeadersMeasureMode.Default;
			using(HeaderGenerator generator = new HeaderGenerator(Panel, CreateFieldHeader, CreateGroupHeader)) {
				FieldHeader lastHeader = null;
				PivotGridGroup lastGroup = null;
				int headerIndex = 0;
				for(int i = 0; i < fields.Count; i++) {
					PivotGridField field = fields[i];
					FieldHeader header = null;
					if(field.Group == null) {
						header = generator.GetNextFieldHeader();
					} else if(field.Group != lastGroup) {
						header = generator.GetNextGroupHeader();
					} else
						continue;
					header.Bind(field, headerIndex, EnableDragDrop && !EnableHeaderMenu ? FieldListArea.All : Area);
					FieldHeadersPanel.SetIndex(header, headerIndex);
					if(lastHeader == null) {
						header.HeaderPosition = HeaderPosition.Left;
					} else {
						if(headerIndex == 0 || fields[headerIndex - 1].Group == null)
							header.HeaderPosition = HeaderPosition.Middle;
						else
							header.HeaderPosition = HeaderPosition.MiddlePreGroup;
					}
					lastHeader = header;
					lastGroup = field.Group;
					headerIndex++;
				}
				if(lastHeader != null) {
					if(headerIndex == 1) {
						lastHeader.HeaderPosition = HeaderPosition.Single;
					} else {
						if(headerIndex <= 1 || fields[headerIndex - 2].Group == null)
							lastHeader.HeaderPosition = HeaderPosition.Right;
						else
							lastHeader.HeaderPosition = HeaderPosition.RightPreGroup;
					}
				}
			}
			boundFields.Clear();
			boundFields.AddRange(fields);
			boundGroups.Clear();
			for(int i = 0; i < fields.Count; i++) {
				boundGroups.Add(fields[i].Group);
			}
		}
		protected virtual bool AllowBind(IList<PivotGridField> fields) {
			return Panel != null && fields != null;
		}
		protected bool AreFieldsChanged(IList<PivotGridField> fields) {
			if(fields.Count != boundFields.Count) return true;
			for(int i = 0; i < fields.Count; i++) {
				if(fields[i] != boundFields[i] || fields[i].Group != boundGroups[i])
					return true;
			}
			return false;
		}
		protected virtual FieldHeader CreateFieldHeader() {
			return new FieldHeader();
		}
		protected virtual GroupHeader CreateGroupHeader() {
			return new GroupHeader();
		}
		protected override void OnAreaChanged() {
			SetPanelFieldListArea();
			base.OnAreaChanged();
		}
		void SetPanelFieldListArea() {
			if(Panel != null) {
				FieldHeadersBase.SetFieldListArea(Panel, Area);
				PivotGridControl.SetPivotGrid(Panel, PivotGrid);
			}
		}
		protected override void OnIsEmptyChanged() {
			if(EmptyAreaTextElement != null)
				FieldHeader.SetActualAreaIndex(EmptyAreaTextElement, IsEmpty ? 0 : -1);
			EnsureVisualState();
			SetValue(DragManager.DropTargetFactoryProperty, IsEmpty && MeasureMode != FieldHeadersMeasureMode.Cut ? new PivotGridDropTargetFactoryExtension() : null);
		}
		protected internal virtual void EnsureVisualState() {
			VisualStateManager.GoToState(this, IsEmpty ? EmptyTextVisibility == Visibility.Visible ? EmptyStateName : FullEmptyStateName : NonEmptyStateName, true);
			if(Container != null)
				VisualStateManager.GoToState(Container, IsEmpty ? EmptyStateName : NonEmptyStateName, true);
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			EnsureVisualState();
			BindHeaders(ItemsSource);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Panel = GetTemplateChild(TemplatePartPanel) as FieldHeadersPanel;
			if(Panel == null)
				throw new TemplateException(TemplatePartPanel);
			EmptyAreaTextElement = GetTemplateChild(TemplatePartEmptyText) as FrameworkElement;
			Container = GetTemplateChild(TemplatePartContainer) as  System.Windows.Controls.Control;
			SetBestFitDecorator();
			SetPanelFieldListArea();
			BindHeaders(ItemsSource);
		}
		protected override void OnPivotGridChanged() {
			base.OnPivotGridChanged();
			SetPanelFieldListArea();
			SubscribeEvents();
		}
		protected virtual void SetBestFitDecorator() {
			if(Area == FieldListArea.RowArea && !EnableDragDrop) {
				Data.BestWidthCalculator.RowAreaDecorator =
					Data.BestHeightCalculator.RowAreaDecorator =
						GetTemplateChild(TemplatePartBestFitDecorator) as BestFitDecorator;
			}
		}
		void OnLeftOffsetChanged() {
		   if(Data != null)
			   LeftPixels = Data.VisualItems.GetWidthDifference(0, LeftOffset, false) + Convert.ToInt32(LeftPixelsOffset);
		}
		protected override System.Windows.Controls.Control GetEmtyStateElement() {
			return Container;
		}
		protected override void OnItemsSourceCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsSourceCollectionChanged(sender, e);
			BindHeaders(ItemsSource);
		}
#if DEBUGTEST
		internal bool isItemsValid { get { return ((RealFieldsSource)FieldsSource).isItemsValid; } }
#endif
	}
	class HeaderGenerator : IDisposable {
		public delegate FieldHeader CreateFieldHeaderDelegate();
		public delegate GroupHeader CreateGroupHeaderDelegate();
		Queue<FieldHeader> fields;
		Queue<GroupHeader> groups;
		Panel panel;
		CreateFieldHeaderDelegate fieldHeaderDelegate;
		CreateGroupHeaderDelegate groupHeaderDelegate;
		public HeaderGenerator(Panel panel, CreateFieldHeaderDelegate fieldHeaderDelegate,
				CreateGroupHeaderDelegate groupHeaderDelegate) {
			this.panel = panel;
			this.fields = new Queue<FieldHeader>();
			this.groups = new Queue<GroupHeader>();
			this.fieldHeaderDelegate = fieldHeaderDelegate;
			this.groupHeaderDelegate = groupHeaderDelegate;
			FillCollections(panel);
		}
		void FillCollections(Panel panel) {
			for(int i = 0; i < panel.Children.Count; i++) {
				FillCollectionProcessItem(panel.Children[i]);
			}
		}
		void FillCollectionProcessItem(UIElement child) {
			GroupHeader group = child as GroupHeader;
			if(group != null) {
				groups.Enqueue(group);
				group.Unbind();
				return;
			}
			FieldHeader field = child as FieldHeader;
			if(field != null) {
				fields.Enqueue(field);
				field.Unbind();
				return;
			}			
		}
		public FieldHeader GetNextFieldHeader() {
			FieldHeader header = null;
			if(fields.Count > 0) {
				header = fields.Dequeue();
				header.Visibility = Visibility.Visible;
				return header;
			}
			header = fieldHeaderDelegate();
			header.DataContext = null;
			panel.Children.Add(header);
			return header;
		}
		public GroupHeader GetNextGroupHeader() {
			GroupHeader header = null;
			if(groups.Count > 0) {
				header = groups.Dequeue();
				header.Visibility = Visibility.Visible;
				return header;
			}
			header = groupHeaderDelegate();
			panel.Children.Add(header);
			return header;
		}
		#region IDisposable Members
		void IDisposable.Dispose() {
			while(fields.Count > 0) {
				FieldHeader header = fields.Dequeue();
				header.Visibility = Visibility.Collapsed;
			}
			while(groups.Count > 0) {
				GroupHeader header = groups.Dequeue();
				header.Visibility = Visibility.Collapsed;
			}
		}
		#endregion
	}
}
