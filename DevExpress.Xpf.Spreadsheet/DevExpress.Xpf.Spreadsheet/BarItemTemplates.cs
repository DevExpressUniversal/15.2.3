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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Office.UI;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Spreadsheet {
	#region SpreadsheetDefaultBarItemDataTemplates
	public class SpreadsheetDefaultBarItemDataTemplates : Control {
		public static readonly DependencyPropertyKey OfficeDefaultBarItemDataTemplatesPropertyKey =
			DependencyProperty.RegisterReadOnly("OfficeDefaultBarItemDataTemplates", typeof(OfficeDefaultBarItemDataTemplates), typeof(SpreadsheetDefaultBarItemDataTemplates),
			new FrameworkPropertyMetadata());
		public static readonly DependencyProperty OfficeDefaultBarItemDataTemplatesProperty = OfficeDefaultBarItemDataTemplatesPropertyKey.DependencyProperty;
		public OfficeDefaultBarItemDataTemplates OfficeDefaultBarItemDataTemplates {
			get { return (OfficeDefaultBarItemDataTemplates)GetValue(OfficeDefaultBarItemDataTemplatesProperty); }
			private set { SetValue(OfficeDefaultBarItemDataTemplatesPropertyKey, value); }
		}
		public static readonly DependencyProperty MarginBarItemContentTemplateProperty = DependencyPropertyManager.Register("MarginBarItemContentTemplate", typeof(DataTemplate), typeof(SpreadsheetDefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public DataTemplate MarginBarItemContentTemplate {
			get { return (DataTemplate)GetValue(MarginBarItemContentTemplateProperty); }
			set { SetValue(MarginBarItemContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty PaperKindBarItemContentTemplateProperty = DependencyPropertyManager.Register("PaperKindBarItemContentTemplate", typeof(DataTemplate), typeof(SpreadsheetDefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public DataTemplate PaperKindBarItemContentTemplate {
			get { return (DataTemplate)GetValue(PaperKindBarItemContentTemplateProperty); }
			set { SetValue(PaperKindBarItemContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty ChartLayoutGalleryGroupTemplateProperty = DependencyPropertyManager.Register("ChartLayoutGalleryGroupTemplate", typeof(DataTemplate), typeof(SpreadsheetDefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public DataTemplate ChartLayoutGalleryGroupTemplate {
			get { return (DataTemplate)GetValue(ChartLayoutGalleryGroupTemplateProperty); }
			set { SetValue(ChartLayoutGalleryGroupTemplateProperty, value); }
		}
		public static readonly DependencyProperty CheckEditTemplateProperty = DependencyPropertyManager.Register("CheckEditTemplate", typeof(DataTemplate), typeof(SpreadsheetDefaultBarItemDataTemplates), new FrameworkPropertyMetadata());
		public DataTemplate CheckEditTemplate {
			get { return (DataTemplate)GetValue(CheckEditTemplateProperty); }
			set { SetValue(CheckEditTemplateProperty, value); }
		}
		public SpreadsheetDefaultBarItemDataTemplates() {
			OfficeDefaultBarItemDataTemplates = new OfficeDefaultBarItemDataTemplates();
			this.DefaultStyleKey = typeof(SpreadsheetDefaultBarItemDataTemplates);
		}
	}
	#endregion
}
