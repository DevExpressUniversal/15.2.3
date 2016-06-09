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
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.DocumentViewer;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfPagesSelector : DocumentViewerItemsControl {
		public static readonly DependencyProperty IsSearchControlVisibleProperty;
		public static readonly DependencyProperty SearchParameterProperty;
		static PdfPagesSelector() {
			Type ownerType = typeof(PdfPagesSelector);
			IsSearchControlVisibleProperty = DependencyPropertyManager.Register("IsSearchControlVisible", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			SearchParameterProperty = DependencyPropertyManager.Register("SearchParameter", typeof(TextSearchParameter), ownerType, new FrameworkPropertyMetadata(null));
		}
		public TextSearchParameter SearchParameter {
			get { return (TextSearchParameter)GetValue(SearchParameterProperty); }
			set { SetValue(SearchParameterProperty, value); }
		}
		public bool IsSearchControlVisible {
			get { return (bool)GetValue(IsSearchControlVisibleProperty); }
			set { SetValue(IsSearchControlVisibleProperty, value); }
		}
		protected internal PdfPresenterDecorator PresenterDecorator { get; private set; }
		public PdfPagesSelector() {
			InitScrolling();
		}
		void InitScrolling() {
			var sup = typeof(VirtualizingPanel).GetField("ScrollUnitProperty");
			if (sup == null)
				return;
			DependencyProperty su = (DependencyProperty)sup.GetValue(this);
			SetValue(su, Enum.GetValues(su.DefaultMetadata.DefaultValue.GetType()).Cast<object>().First());
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new PdfPageControl();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is PdfPageControl;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			PdfPageControl control = (PdfPageControl)element;
			var wrapper = item as PdfPageWrapper;
			if (control == null)
				return;
			control.Pages = wrapper.Pages.Cast<PdfPageViewModel>();
			control.DataContext = wrapper;
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			PdfPageControl control = (PdfPageControl)element;
			control.Pages = null;
			control.DataContext = null;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PresenterDecorator = (PdfPresenterDecorator)GetTemplateChild("PART_Decorator");
		}
	}
}
