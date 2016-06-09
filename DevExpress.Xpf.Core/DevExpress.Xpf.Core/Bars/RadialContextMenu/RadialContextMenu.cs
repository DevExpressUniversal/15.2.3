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
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.ComponentModel;
using DevExpress.Xpf.Bars.Converters;
namespace DevExpress.Xpf.Bars {
	public class RadialContextMenu : PopupMenu, ILinksHolder {
		#region static
		public static readonly DependencyProperty GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(RadialContextMenu), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty BackButtonGlyphProperty = DependencyPropertyManager.Register("BackButtonGlyph", typeof(ImageSource), typeof(RadialContextMenu), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty MinSectorCountProperty = DependencyPropertyManager.Register("MinSectorCount", typeof(int), typeof(RadialContextMenu), new FrameworkPropertyMetadata(8, FrameworkPropertyMetadataOptions.AffectsMeasure, null, (o, v) => { return (int)v <= 2 ? 2 : v; }));
		public static readonly DependencyProperty AutoExpandProperty = DependencyProperty.Register("AutoExpand", typeof(bool?), typeof(RadialContextMenu), new PropertyMetadata(null));
		public static readonly DependencyProperty FirstSectorIndexProperty = DependencyProperty.Register("FirstSectorIndex", typeof(int), typeof(RadialContextMenu), new PropertyMetadata(0));
		public static readonly DependencyProperty GlyphTemplateProperty = DependencyProperty.Register("GlyphTemplate", typeof(DataTemplate), typeof(RadialContextMenu), new PropertyMetadata(null));
		public static readonly DependencyProperty BackButtonGlyphTemplateProperty = DependencyProperty.Register("BackButtonGlyphTemplate", typeof(DataTemplate), typeof(RadialContextMenu), new PropertyMetadata(null));
		static RadialContextMenu() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialContextMenu), new FrameworkPropertyMetadata(typeof(RadialContextMenu)));
		}
		#endregion
		#region dep props
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		public ImageSource BackButtonGlyph {
			get { return (ImageSource)GetValue(BackButtonGlyphProperty); }
			set { SetValue(BackButtonGlyphProperty, value); }
		}
		public DataTemplate GlyphTemplate {
			get { return (DataTemplate)GetValue(GlyphTemplateProperty); }
			set { SetValue(GlyphTemplateProperty, value); }
		}
		public DataTemplate BackButtonGlyphTemplate {
			get { return (DataTemplate)GetValue(BackButtonGlyphTemplateProperty); }
			set { SetValue(BackButtonGlyphTemplateProperty, value); }
		}
		public int MinSectorCount {
			get { return (int)GetValue(MinSectorCountProperty); }
			set { SetValue(MinSectorCountProperty, value); }
		}
		public bool? AutoExpand {
			get { return (bool?)GetValue(AutoExpandProperty); }
			set { SetValue(AutoExpandProperty, value); }
		}
		public int FirstSectorIndex {
			get { return (int)GetValue(FirstSectorIndexProperty); }
			set { SetValue(FirstSectorIndexProperty, value); }
		}
		#endregion
		internal ContextMenuOpenReason OpenReason { get; set; }
		RadialMenuControl RadialMenuControl { get { return PopupContent as RadialMenuControl; } }
		public RadialContextMenu() : base() {
			FocusManager.SetIsFocusScope(PopupContent as DependencyObject, false);
			CustomPopupPlacementCallback = CalculatePlacement;
		}
		private CustomPopupPlacement[] CalculatePlacement(Size popupSize, Size targetSize, Point offset) {
			if(FlowDirection == System.Windows.FlowDirection.LeftToRight) {
				return new CustomPopupPlacement[] {
				new CustomPopupPlacement(new Point(Math.Round(targetSize.Width), Math.Round((targetSize.Height - popupSize.Height) / 2)), PopupPrimaryAxis.None),
				new CustomPopupPlacement(new Point(Math.Round(targetSize.Width - 1), Math.Round(- popupSize.Height)), PopupPrimaryAxis.Horizontal),
				new CustomPopupPlacement(new Point(Math.Round(targetSize.Width - 1), Math.Round(targetSize.Height)), PopupPrimaryAxis.None)
				};
			}
			else {
				return new CustomPopupPlacement[] {
				new CustomPopupPlacement(new Point(Math.Round(- popupSize.Width - targetSize.Width), Math.Round((targetSize.Height - popupSize.Height) / 2)), PopupPrimaryAxis.None),
				new CustomPopupPlacement(new Point(Math.Round(- popupSize.Width - targetSize.Width + 1), Math.Round(- popupSize.Height)), PopupPrimaryAxis.Horizontal),
				new CustomPopupPlacement(new Point(Math.Round(- popupSize.Width - targetSize.Width + 1), Math.Round(targetSize.Height)), PopupPrimaryAxis.None)
				};
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.Key == Key.Escape && RadialMenuControl.IsRegularPopupOpened) {
				RadialMenuControl.CloseRegularPopup();
				e.Handled = true;
			}
		}
		protected override PopupBorderControl CreateBorderControl() {
			return new RadialMenuPopupBorderControl();
		}
		protected override object CreatePopupContent() {
			var popupContent = new RadialMenuControl(this);
			popupContent.ContainerType = GetLinkContainerType();
			return popupContent;
		}
		protected override LinkContainerType GetLinkContainerType() {
			return LinkContainerType.RadialMenu;
		}
		LinksHolderType ILinksHolder.HolderType { get { return LinksHolderType.RadialMenu; } }
	}
}
