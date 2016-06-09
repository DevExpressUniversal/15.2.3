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
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Diagram.Native;
namespace DevExpress.Xpf.Reports.UserDesigner {
	public class ReportDesignerBrowserView : ReportDesignerDocumentsViewBase {
		public static readonly DependencyProperty NewWindowStyleProperty;
		static ReportDesignerBrowserView() {
			DependencyPropertyRegistrator<ReportDesignerBrowserView>.New()
				.Register(d => d.NewWindowStyle, out NewWindowStyleProperty, null)
				.OverrideMetadata(ShowNewDocumentButtonProperty, false)
				.OverrideMetadata(ShowPreviewButtonProperty, false)
				.OverrideDefaultStyleKey()
			;
		}
		public Style NewWindowStyle {
			get { return (Style)GetValue(NewWindowStyleProperty); }
			set { SetValue(NewWindowStyleProperty, value); }
		}
	}
	public class ReportDesignerTabbedWindowStyleExtension : MarkupExtension {
		static Style style;
		public static Style Style {
			get {
				if(style == null) {
					style = new Style(typeof(DXTabbedWindow), ReportDesignerWindowStyleExtension.Style);
					style.Setters.Add(new Setter(DXWindow.ShowTitleProperty, false));
					style.Setters.Add(new Setter(DXWindow.ShowIconProperty, false));
				}
				return style;
			}
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return Style;
		}
	}
}
