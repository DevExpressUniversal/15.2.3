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
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Xpf.Office.UI;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using FrameworkContentElement = System.Windows.FrameworkElement;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PlatformIndependentColor = System.Windows.Media.Color;
#else
using DevExpress.Xpf.Utils;
using PlatformIndependentColor = System.Drawing.Color;
#endif
namespace DevExpress.Xpf.RichEdit.UI {
	#region DefaultBarItemDataTemplates
	public class DefaultBarItemDataTemplates : Control {
#if !SL
		static readonly DependencyPropertyKey OfficeDefaultBarItemDataTemplatesPropertyKey = DependencyProperty.RegisterReadOnly("OfficeDefaultBarItemDataTemplates", typeof(OfficeDefaultBarItemDataTemplates), typeof(DefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty OfficeDefaultBarItemDataTemplatesProperty = OfficeDefaultBarItemDataTemplatesPropertyKey.DependencyProperty;
		public OfficeDefaultBarItemDataTemplates OfficeDefaultBarItemDataTemplates {
			get { return (OfficeDefaultBarItemDataTemplates)GetValue(OfficeDefaultBarItemDataTemplatesProperty); }
			private set { SetValue(OfficeDefaultBarItemDataTemplatesPropertyKey, value); }
		}
#else
		public static readonly DependencyProperty OfficeDefaultBarItemDataTemplatesProperty = DependencyPropertyManager.Register("OfficeDefaultBarItemDataTemplates", typeof(OfficeDefaultBarItemDataTemplates), typeof(DefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public OfficeDefaultBarItemDataTemplates OfficeDefaultBarItemDataTemplates {
			get { return (OfficeDefaultBarItemDataTemplates)GetValue(OfficeDefaultBarItemDataTemplatesProperty); }
			set { SetValue(OfficeDefaultBarItemDataTemplatesProperty, value); }
		}
#endif
		public static readonly DependencyProperty SectionMarginBarItemContentTemplateProperty = DependencyPropertyManager.Register("SectionMarginBarItemContentTemplate", typeof(DataTemplate), typeof(DefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public DataTemplate SectionMarginBarItemContentTemplate {
			get { return (DataTemplate)GetValue(SectionMarginBarItemContentTemplateProperty); }
			set { SetValue(SectionMarginBarItemContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty SectionPaperKindBarItemContentTemplateProperty = DependencyPropertyManager.Register("SectionPaperKindBarItemContentTemplate", typeof(DataTemplate), typeof(DefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public DataTemplate SectionPaperKindBarItemContentTemplate {
			get { return (DataTemplate)GetValue(SectionPaperKindBarItemContentTemplateProperty); }
			set { SetValue(SectionPaperKindBarItemContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty StyleGalleryItemCaptionTemplateProperty = DependencyPropertyManager.Register("StyleGalleryItemCaptionTemplate", typeof(DataTemplate), typeof(DefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public DataTemplate StyleGalleryItemCaptionTemplate {
			get { return (DataTemplate)GetValue(StyleGalleryItemCaptionTemplateProperty); }
			set { SetValue(StyleGalleryItemCaptionTemplateProperty, value); }
		}
		public DefaultBarItemDataTemplates() {
			OfficeDefaultBarItemDataTemplates = new OfficeDefaultBarItemDataTemplates();
			this.DefaultStyleKey = typeof(DefaultBarItemDataTemplates);
		}
	}
	#endregion
	#region CharactersBackgroundColorPaletteCollection
	public class CharactersBackgroundColorPaletteCollection : PaletteCollection {
		public CharactersBackgroundColorPaletteCollection()
			: base("Background", new ColorPalette[] { new CustomPalette() }) {
		}
		class CustomPalette : ColorPalette {
			static readonly List<Color> DefaultColors = CreateDefaultColors();
			internal CustomPalette()
				: base(String.Empty, DefaultColors, false) {
			}
			static List<Color> CreateDefaultColors() {
				List<Color> colors = new List<Color>();
				colors.Add(Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00)); 
				colors.Add(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00)); 
				colors.Add(Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF)); 
				colors.Add(Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF)); 
				colors.Add(Color.FromArgb(0xFF, 0x00, 0x00, 0xFF)); 
				colors.Add(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00)); 
				colors.Add(Color.FromArgb(0xFF, 0x00, 0x00, 0x80)); 
				colors.Add(Color.FromArgb(0xFF, 0x00, 0x80, 0x80)); 
				colors.Add(Color.FromArgb(0xFF, 0x00, 0x80, 0x00)); 
				colors.Add(Color.FromArgb(0xFF, 0x80, 0x00, 0x80)); 
				colors.Add(Color.FromArgb(0xFF, 0x80, 0x00, 0x00)); 
				colors.Add(Color.FromArgb(0xFF, 0x80, 0x80, 0x00)); 
				colors.Add(Color.FromArgb(0xFF, 0x80, 0x80, 0x80)); 
				colors.Add(Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0)); 
				colors.Add(Color.FromArgb(0xFF, 0x00, 0x00, 0x00)); 
				return colors;
			}
		}
	}
	#endregion
}
