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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Navigation.Internal;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Navigation {
	public abstract class TileBase : ClickableBase, ITileSizeManagerOwner {
		#region static
		public static readonly DependencyProperty SizeProperty;
		public static readonly DependencyProperty TileGlyphProperty;
		public static readonly DependencyProperty HorizontalTileGlyphAlignmentProperty;
		public static readonly DependencyProperty VerticalTileGlyphAlignmentProperty;
		public static readonly DependencyProperty ActualTileContentProperty;
		protected static readonly DependencyPropertyKey ActualTileContentPropertyKey;
		public static readonly DependencyProperty ActualTileContentTemplateProperty;
		protected static readonly DependencyPropertyKey ActualTileContentTemplatePropertyKey;
		public static readonly DependencyProperty ActualTileContentTemplateSelectorProperty;
		protected static readonly DependencyPropertyKey ActualTileContentTemplateSelectorPropertyKey;
		static TileBase() {
			Type ownerType = typeof(TileBase);
			SizeProperty = DependencyProperty.Register("Size", typeof(TileSize), ownerType, new UIPropertyMetadata(TileSize.Default, new PropertyChangedCallback(OnSizeChanged)));
			TileGlyphProperty = DependencyProperty.Register("TileGlyph", typeof(ImageSource), ownerType);
			HorizontalTileGlyphAlignmentProperty = DependencyProperty.Register("HorizontalTileGlyphAlignment", typeof(HorizontalAlignment), ownerType, new PropertyMetadata(HorizontalAlignment.Left));
			VerticalTileGlyphAlignmentProperty = DependencyProperty.Register("VerticalTileGlyphAlignment", typeof(VerticalAlignment), ownerType, new PropertyMetadata(VerticalAlignment.Top));
			ActualTileContentPropertyKey = DependencyProperty.RegisterReadOnly("ActualTileContent", typeof(object), ownerType, new PropertyMetadata(null));
			ActualTileContentProperty = ActualTileContentPropertyKey.DependencyProperty;
			ActualTileContentTemplatePropertyKey = DependencyProperty.RegisterReadOnly("ActualTileContentTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null));
			ActualTileContentTemplateProperty = ActualTileContentTemplatePropertyKey.DependencyProperty;
			ActualTileContentTemplateSelectorPropertyKey = DependencyProperty.RegisterReadOnly("ActualTileContentTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null));
			ActualTileContentTemplateSelectorProperty = ActualTileContentTemplateSelectorPropertyKey.DependencyProperty;
		}
		private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TileBase tileBase = d as TileBase;
			if(tileBase != null)
				tileBase.OnSizeChanged((TileSize)e.OldValue, (TileSize)e.NewValue);
		}
		#endregion
		protected TileBase() {
			UpdateTileSize();
		}
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			EnsureSizeManager();
		}
		protected virtual void EnsureSizeManager() {
		}
		protected virtual void OnSizeChanged(TileSize oldValue, TileSize newValue) {
			UpdateTileSize();
		}
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			base.OnRenderSizeChanged(sizeInfo);
			UpdateTileSize();
		}
		protected virtual void UpdateTileSize() {
			if(SizeManager != null) {
				bool isInDesignTime = DesignerProperties.GetIsInDesignMode(this);
				bool hasWidth = isInDesignTime ? !this.HasDefaultValue(WidthProperty) : this.IsPropertySet(WidthProperty);
				bool hasHeight = isInDesignTime ? !this.HasDefaultValue(HeightProperty) : this.IsPropertySet(HeightProperty);
				SizeManager.UpdateActualSize(Size, false, hasWidth, 1, 0, hasHeight);
			}
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			UpdateActualTileContent();
		}
		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate) {
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);
			UpdateActualTileContent();
		}
		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector) {
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);
			UpdateActualTileContent();
		}
		protected virtual void UpdateActualTileContent() {
			SetValue(ActualTileContentPropertyKey, Content);
			SetValue(ActualTileContentTemplatePropertyKey, ContentTemplate);
			SetValue(ActualTileContentTemplateSelectorPropertyKey, ContentTemplateSelector);
		}
		protected virtual Size GetSizeForTileSize(TileSize tileSize) {
			return new SizeProvider().GetSize(tileSize);
		}
		protected internal TileSizeManager SizeManager;
		public TileSize Size {
			get { return (TileSize)GetValue(SizeProperty); }
			set { SetValue(SizeProperty, value); }
		}
		public ImageSource TileGlyph {
			get { return (ImageSource)GetValue(TileGlyphProperty); }
			set { SetValue(TileGlyphProperty, value); }
		}
		public HorizontalAlignment HorizontalTileGlyphAlignment {
			get { return (HorizontalAlignment)GetValue(HorizontalTileGlyphAlignmentProperty); }
			set { SetValue(HorizontalTileGlyphAlignmentProperty, value); }
		}
		public VerticalAlignment VerticalTileGlyphAlignment {
			get { return (VerticalAlignment)GetValue(VerticalTileGlyphAlignmentProperty); }
			set { SetValue(VerticalTileGlyphAlignmentProperty, value); }
		}
		public object ActualTileContent {
			get { return GetValue(ActualTileContentProperty); }
		}
		public DataTemplate ActualTileContentTemplate {
			get { return (DataTemplate)GetValue(ActualTileContentTemplateProperty); }
		}
		public DataTemplateSelector ActualTileContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualTileContentTemplateSelectorProperty); }
		}
		#region ITileSizeManagerOwner Members
		void ITileSizeManagerOwner.OnSizeChanged() {
			UpdateTileSize();
		}
		#endregion
		#region ITileSizeProvider Members
		Size ITileSizeProvider.GetSize(TileSize tileSize) {
			return GetSizeForTileSize(tileSize);
		}
		#endregion
		internal class SizeProvider {
			public Size GetSize(TileSize tileSize) {
				return GetSizeCore(tileSize);
			}
			protected virtual Size GetSizeCore(TileSize tileSize) {
				switch(tileSize) {
					case TileSize.Auto:
						return new Size(double.NaN, double.NaN);
					case TileSize.Small:
						return new Size(70, 68);
					case TileSize.Wide:
						return new Size(310, 68);
					case TileSize.Default:
					case TileSize.Medium:
					default:
						return new Size(150, 68);
				}
			}
		}
	}
}
