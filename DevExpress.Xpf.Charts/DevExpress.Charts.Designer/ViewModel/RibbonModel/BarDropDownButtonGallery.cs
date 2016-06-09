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

using System.Windows.Media;
using DevExpress.Xpf.Bars;
namespace DevExpress.Charts.Designer.Native {
	public class BarDropDownButtonGalleryViewModel : DropDownGalleryViewModelBase {
		RibbonItemStyles ribbonStyle = RibbonItemStyles.Large | RibbonItemStyles.SmallWithText;
		ImageSource glyph;
		ImageSource largeGlyph;
		bool allowToolTips = true;
		GalleryItemCheckMode itemCheckMode = GalleryItemCheckMode.None;
		public BarDropDownButtonGalleryViewModel() { }
		public BarDropDownButtonGalleryViewModel(int initialVisibleColCount, int initialVisibleRowCount, 
			bool isItemCaptionVisible, bool isItemDescriptionVisible, GalleryItemCheckMode checkMode) {
				InitialVisibleColCount = initialVisibleColCount;
				InitialVisibleRowCount = initialVisibleRowCount;
				IsItemCaptionVisible = isItemCaptionVisible;
				IsItemDescriptionVisible = isItemDescriptionVisible;
				ItemCheckMode = checkMode;
		}
		public RibbonItemStyles RibbonStyle {
			get { return ribbonStyle; }
			set {
				if (ribbonStyle != value) {
					ribbonStyle = value;
					OnPropertyChanged("RibbonStyle");
				}
			}
		}
		public ImageSource Glyph {
			get { return glyph; }
			set {
				if (glyph != value) {
					glyph = value;
					OnPropertyChanged("Glyph");
				}
			}
		}
		public ImageSource LargeGlyph {
			get { return largeGlyph; }
			set {
				if (largeGlyph != value) {
					largeGlyph = value;
					OnPropertyChanged("LargeGlyph");
				}
			}
		}
		public string ImageName {
			set {
				if (!string.IsNullOrEmpty(value)) {
					Glyph = GlyphUtils.GetGlyphByPath(value);
					LargeGlyph = GlyphUtils.GetLargeGlyphByPath(value);
				}
			}
		}
		public bool AllowToolTips {
			get { return allowToolTips; }
			set {
				if (allowToolTips != value) {
					allowToolTips = value;
					OnPropertyChanged("AllowToolTips");
				}
			}
		}
		public GalleryItemCheckMode ItemCheckMode {
			get { return itemCheckMode; }
			set {
				if (itemCheckMode != value) {
					itemCheckMode = value;
					OnPropertyChanged("ItemCheckMode");
				}
			}
		}
	}
}
