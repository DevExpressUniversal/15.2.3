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
using DevExpress.Xpf.Docking;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Docking.Native;
using DevExpress.Xpf.Docking.Base;
using System.ComponentModel;
using DevExpress.Xpf.Layout.Core;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Docking {
	[TargetTypeAttribute(typeof(UserControl))]
	[TargetTypeAttribute(typeof(Window))]
	[TargetTypeAttribute(typeof(DocumentGroup))]
	[TargetTypeAttribute(typeof(DockLayoutManager))]
	public class TabbedDocumentUIService : DockingDocumentUIServiceBase<DocumentPanel, DocumentGroup> {
		public static readonly DependencyProperty DocumentPanelStyleProperty =
			DependencyProperty.Register("DocumentPanelStyle", typeof(Style), typeof(TabbedDocumentUIService), new PropertyMetadata(null));
		public static readonly DependencyProperty UseActiveDocumentGroupAsDocumentHostProperty =
			DependencyProperty.Register("UseActiveDocumentGroupAsDocumentHost", typeof(bool), typeof(TabbedDocumentUIService), new PropertyMetadata(true));
		public static readonly DependencyProperty DocumentPanelStyleSelectorProperty =
			DependencyProperty.Register("DocumentPanelStyleSelector", typeof(StyleSelector), typeof(TabbedDocumentUIService), new PropertyMetadata(null));
		public static readonly DependencyProperty DocumentGroupProperty =
			DependencyProperty.Register("DocumentGroup", typeof(DocumentGroup), typeof(TabbedDocumentUIService),
			new PropertyMetadata(null, (d, e) => ((TabbedDocumentUIService)d).OnDocumentGroupChanged((DocumentGroup)e.OldValue, (DocumentGroup)e.NewValue)));
		public DocumentGroup DocumentGroup {
			get { return (DocumentGroup)GetValue(DocumentGroupProperty); }
			set { SetValue(DocumentGroupProperty, value); }
		}
		public StyleSelector DocumentPanelStyleSelector {
			get { return (StyleSelector)GetValue(DocumentPanelStyleSelectorProperty); }
			set { SetValue(DocumentPanelStyleSelectorProperty, value); }
		}
		public Style DocumentPanelStyle {
			get { return (Style)GetValue(DocumentPanelStyleProperty); }
			set { SetValue(DocumentPanelStyleProperty, value); }
		}
		public bool UseActiveDocumentGroupAsDocumentHost {
			get { return (bool)GetValue(UseActiveDocumentGroupAsDocumentHostProperty); }
			set { SetValue(UseActiveDocumentGroupAsDocumentHostProperty, value); }
		}
		public DocumentGroup ActualDocumentGroup {
			get {
				if(UseActiveDocumentGroupAsDocumentHost && ActiveDocument != null) {
					DocumentGroup activeDocumentGroup = ((Document)ActiveDocument).DocumentPanel.Parent as DocumentGroup;
					if(activeDocumentGroup != null && activeDocumentGroup.ItemType != LayoutItemType.FloatGroup)
						return activeDocumentGroup;
				}
				if(DocumentGroup != null) return DocumentGroup;
				return AssociatedObject as DocumentGroup;
			}
		}
		protected override void OnAttached() {
			base.OnAttached();
			(AssociatedObject as DocumentGroup).Do(x => x.DestroyOnClosingChildren = false);
		}
		protected virtual void OnDocumentGroupChanged(DocumentGroup oldValue, DocumentGroup newValue) {
			newValue.Do(x => x.DestroyOnClosingChildren = false);
		}
		protected override DocumentPanel CreateDocumentPanel() { return new DocumentPanel(); }
		protected override DocumentGroup GetActualDocumentGroup() { return ActualDocumentGroup; }
		protected override Style GetDocumentPanelStyle(DocumentPanel documentPanel, object documentContentView) {
			return GetDocumentContainerStyle(documentPanel, documentContentView, DocumentPanelStyle, DocumentPanelStyleSelector);
		}
	}
}
