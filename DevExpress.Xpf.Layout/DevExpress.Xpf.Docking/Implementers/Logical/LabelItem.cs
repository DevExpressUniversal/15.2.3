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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking {
	[ContentProperty("Content")]
	public class LabelItem : FixedItem {
		#region static
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
		public static readonly DependencyProperty HasContentProperty;
		internal static readonly DependencyPropertyKey HasContentPropertyKey;
		public static readonly DependencyProperty ContentHorizontalAlignmentProperty;
		public static readonly DependencyProperty ContentVerticalAlignmentProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty DesiredSizeInternalProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HasDesiredSizeProperty;
		internal static readonly DependencyPropertyKey HasDesiredSizePropertyKey;
		static LabelItem() {
			var dProp = new DependencyPropertyRegistrator<LabelItem>();
			dProp.OverrideMetadata(ItemHeightProperty, new GridLength(1, GridUnitType.Auto));
			dProp.Register("Content", ref ContentProperty, (object)null,
				(dObj, e) => ((LabelItem)dObj).OnContentChanged(e.NewValue));
			dProp.Register("ContentTemplate", ref ContentTemplateProperty, (DataTemplate)null);
			dProp.Register("ContentTemplateSelector", ref ContentTemplateSelectorProperty, (DataTemplateSelector)null);
			dProp.RegisterReadonly("HasContent", ref HasContentPropertyKey, ref HasContentProperty, false, null);
			dProp.Register("ContentHorizontalAlignment", ref ContentHorizontalAlignmentProperty, HorizontalAlignment.Stretch, null);
			dProp.Register("ContentVerticalAlignment", ref ContentVerticalAlignmentProperty, VerticalAlignment.Stretch, null);
			dProp.Register("DesiredSizeInternal", ref DesiredSizeInternalProperty, Size.Empty,
				(dObj, e) => ((LabelItem)dObj).OnDesiredSizeChanged((Size)e.NewValue),
				(dObj, value) => ((LabelItem)dObj).CoerceDesiredSize((Size)value));
			dProp.RegisterReadonly("HasDesiredSize", ref HasDesiredSizePropertyKey, ref HasDesiredSizeProperty, false);
		}
		#endregion
		public LabelItem() {
		}
		protected override LayoutItemType GetLayoutItemTypeCore() {
			return LayoutItemType.Label;
		}
		protected virtual void OnDesiredSizeChanged(Size value) {
			SetDesiredSize(value);
			SetValue(HasDesiredSizePropertyKey, !value.IsEmpty);
			CoerceValue(ActualMinSizeProperty);
		}
		protected virtual void OnContentChanged(object content) {
			SetValue(HasContentPropertyKey, Content != null);
		}
		Size desiredSizeCore;
		void SetDesiredSize(Size value) {
			if(desiredSizeCore == value) return;
			desiredSizeCore = value;
		}
		protected virtual object CoerceDesiredSize(Size value) {
			if(HasDesiredSize) return desiredSizeCore;
			return value;
		}
		protected override Size CalcMinSizeValue(Size value) {
			if(HasDesiredSize) return MathHelper.MeasureMinSize(new Size[] { DesiredSizeInternal, value });
			return base.CalcMinSizeValue(value);
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LabelItemContent"),
#endif
		XtraSerializableProperty, Category("Content")]
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LabelItemContentTemplate")]
#endif
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LabelItemContentTemplateSelector")]
#endif
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LabelItemHasContent")]
#endif
		public bool HasContent {
			get { return (bool)GetValue(HasContentProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LabelItemContentHorizontalAlignment"),
#endif
		XtraSerializableProperty, Category("Content")]
		public HorizontalAlignment ContentHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(ContentHorizontalAlignmentProperty); }
			set { SetValue(ContentHorizontalAlignmentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LabelItemContentVerticalAlignment"),
#endif
		XtraSerializableProperty, Category("Content")]
		public VerticalAlignment ContentVerticalAlignment {
			get { return (VerticalAlignment)GetValue(ContentVerticalAlignmentProperty); }
			set { SetValue(ContentVerticalAlignmentProperty, value); }
		}
		internal Size DesiredSizeInternal {
			get { return (Size)GetValue(DesiredSizeInternalProperty); }
			set { SetValue(DesiredSizeInternalProperty, value); }
		}
		internal bool HasDesiredSize {
			get { return (bool)GetValue(HasDesiredSizeProperty); }
		}
	}
}
