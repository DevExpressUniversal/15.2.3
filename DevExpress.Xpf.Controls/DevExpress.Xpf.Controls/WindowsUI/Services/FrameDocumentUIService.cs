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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.WindowsUI.Navigation {
	[DevExpress.Mvvm.UI.Interactivity.TargetTypeAttribute(typeof(NavigationFrame))]
	public class FrameDocumentUIService : FrameNavigationService {
		public static readonly DependencyProperty PageAdornerControlStyleProperty =
			DependencyProperty.Register("PageAdornerControlStyle", typeof(Style), typeof(FrameDocumentUIService), new PropertyMetadata(null));
		public static readonly DependencyProperty PageAdornerControlStyleSelectorProperty =
			DependencyProperty.Register("PageAdornerControlStyleSelector", typeof(StyleSelector), typeof(FrameDocumentUIService), new PropertyMetadata(null));
		public static readonly DependencyProperty ActiveDocumentProperty =
			DependencyProperty.Register("ActiveDocument", typeof(IDocument), typeof(FrameDocumentUIService),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((FrameDocumentUIService)d).OnActiveDocumentChanged(e.OldValue as IDocument, e.NewValue as IDocument)));
		static readonly DependencyPropertyKey ActiveViewPropertyKey =
			DependencyProperty.RegisterReadOnly("ActiveView", typeof(object), typeof(FrameDocumentUIService), new PropertyMetadata(null));
		public static readonly DependencyProperty ActiveViewProperty = ActiveViewPropertyKey.DependencyProperty;
		static FrameDocumentUIService() {
			ShowSplashScreenProperty.OverrideMetadata(typeof(FrameDocumentUIService), new PropertyMetadata(false));
		}
		bool syncActiveDocument = true;
		public Style PageAdornerControlStyle {
			get { return (Style)GetValue(PageAdornerControlStyleProperty); }
			set { SetValue(PageAdornerControlStyleProperty, value); }
		}
		public StyleSelector PageAdornerControlStyleSelector {
			get { return (StyleSelector)GetValue(PageAdornerControlStyleSelectorProperty); }
			set { SetValue(PageAdornerControlStyleSelectorProperty, value); }
		}
		public new IDocument ActiveDocument {
			get { return (IDocument)GetValue(ActiveDocumentProperty); }
			set { SetValue(ActiveDocumentProperty, value); }
		}
		public object ActiveView {
			get { return GetValue(ActiveViewProperty); }
			private set { SetValue(ActiveViewPropertyKey, value); }
		}
		protected override void SetActiveDocument(IDocument newActiveDocument) {
			syncActiveDocument = false;
			try {
				ActiveDocument = newActiveDocument;
			} finally {
				syncActiveDocument = true;
			}
			base.SetActiveDocument(newActiveDocument);
		}
		protected override Style GetDocumentContainerStyle(DependencyObject documentContainer, object view) {
			return GetDocumentContainerStyle(documentContainer, view, PageAdornerControlStyle, PageAdornerControlStyleSelector);
		}
		protected override void OnActiveDocumentChanged(IDocument oldValue, IDocument newValue) {
			if(syncActiveDocument)
				base.OnActiveDocumentChanged(oldValue, newValue);
			if(ActiveDocumentChanged != null)
				ActiveDocumentChanged(this, new ActiveDocumentChangedEventArgs(oldValue, newValue));
		}
		protected override void SetActiveView(object newActiveView) {
			base.SetActiveView(newActiveView);
			ActiveView = newActiveView;
		}
		public event ActiveDocumentChangedEventHandler ActiveDocumentChanged;
	}
}
