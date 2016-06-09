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

using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Docking {
	public class TabbedGroup : LayoutGroup {
		#region static
		public static readonly DependencyProperty ShowTabForSinglePageProperty;
		public static readonly DependencyProperty IsMaximizedProperty;
		protected internal static readonly DependencyPropertyKey IsMaximizedPropertyKey;
		static TabbedGroup() {
			var dProp = new DependencyPropertyRegistrator<TabbedGroup>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.OverrideMetadata(AllowFloatProperty, true);
			dProp.OverrideMetadata(AllowSelectionProperty, false);
			dProp.OverrideMetadata(GroupTemplateSelectorProperty, (DataTemplateSelector)new DefaultTemplateSelector(),
				(dObj, e) => ((TabbedGroup)dObj).OnGroupTemplateChanged());
			dProp.Register("ShowTabForSinglePage", ref ShowTabForSinglePageProperty, false);
			dProp.RegisterReadonly("IsMaximized", ref IsMaximizedPropertyKey, ref IsMaximizedProperty, false,
				(dObj, e) => ((TabbedGroup)dObj).OnIsMaximizedChanged((bool)e.NewValue));
		}
		#endregion static
		public TabbedGroup() {
		}
		protected override Size CalcMinSizeValue(Size value) {
			Size[] minSizes, maxSizes;
			Items.CollectConstraints(out minSizes, out maxSizes);
			Size groupMinSize = MathHelper.MeasureMinSize(minSizes);
			return new Size(Math.Max(groupMinSize.Width, value.Width), Math.Max(groupMinSize.Height, value.Height));
		}
		protected override Size CalcMaxSizeValue(Size value) {
			Size[] minSizes, maxSizes;
			Items.CollectConstraints(out minSizes, out maxSizes);
			Size groupMinSize = MathHelper.MeasureMinSize(minSizes);
			Size groupMaxSize = MathHelper.MeasureMaxSize(maxSizes);
			return MathHelper.MeasureSize(groupMinSize, groupMaxSize, value);
		}
		protected override LayoutItemType GetLayoutItemTypeCore() {
			return LayoutItemType.TabPanelGroup;
		}
		protected internal override bool IgnoreOrientation { get { return true; } }
		protected override bool CanCreateItemsInternal() { return false; }
		protected internal override bool IsTabHost { get { return true; } }
		protected internal override bool IsMaximizable { get { return IsFloatingRootItem; } }
		protected override void OnTabHeaderHasScrollChanged(bool hasScroll) {
			OnLayoutChanged();
		}
		protected override void OnTabHeaderLayoutTypeChanged(TabHeaderLayoutType type) {
			CoerceValue(TabHeadersAutoFillProperty);
			OnLayoutChanged();
		}
		protected override void OnTabHeadersAutoFillChanged(bool autoFill) {
			OnLayoutChanged();
		}
		protected override bool HasTabHeader() {
			return true;
		}
		protected override bool GetIsAutoHidden() {
			return base.GetIsAutoHidden() || LayoutGroup.GetOwnerGroup(this) is AutoHideGroup;
		}
		protected override LayoutGroup GetContainerHost(ContentItem container) {
			LayoutGroup ownerGroup = LayoutGroup.GetOwnerGroup(this);
			return IsAutoHidden && ownerGroup != null ? ownerGroup : base.GetContainerHost(container);
		}
		internal override void PrepareForModification(bool isDeserializing) {
			base.PrepareForModification(isDeserializing);
			if(isDeserializing) LayoutGroup.SetOwnerGroup(this, null);
		}
		protected override void OnIsFloatingRootItemChanged(bool newValue) {
			base.OnIsFloatingRootItemChanged(newValue);
			Items.Accept((x) => {
				x.SetIsMaximized(IsMaximized);
				x.UpdateButtons();
			});
		}
		protected override void OnItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsCollectionChanged(sender, e);
			if(IsFloatingRootItem)
				Items.Accept(x => x.SetIsMaximized(IsMaximized));
		}
		protected virtual void OnIsMaximizedChanged(bool maximized) {
			if(IsFloatingRootItem) {
				Items.Accept(x => x.SetIsMaximized(maximized));
			}
		}
		internal override void SetIsMaximized(bool isMaximized) {
			SetValue(IsMaximizedPropertyKey, isMaximized);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool SerializableIsMaximized {
			get { return IsMaximized; }
			set { }
		}
		[Category("Layout")]
		public bool IsMaximized {
			get { return (bool)GetValue(IsMaximizedProperty); }
			internal set { SetValue(IsMaximizedPropertyKey, value); }
		}
		[XtraSerializableProperty]
		public bool ShowTabForSinglePage {
			get { return (bool)GetValue(ShowTabForSinglePageProperty); }
			set { SetValue(ShowTabForSinglePageProperty, value); }
		}
		#region Internal classes
		class DefaultTemplateSelector : DefaultItemTemplateSelectorWrapper.DefaultItemTemplateSelector {
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				TabbedPaneContentPresenter presenter = container as TabbedPaneContentPresenter;
				TabbedGroup tabbedGroup = item as TabbedGroup;
				if(tabbedGroup != null && presenter != null && presenter.Owner != null)
					return presenter.Owner.TabbedTemplate;
				return null;
			}
		}
		#endregion Internal classes
	}
}
