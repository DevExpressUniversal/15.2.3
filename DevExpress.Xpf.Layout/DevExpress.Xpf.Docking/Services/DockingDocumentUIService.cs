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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Docking.Native;
namespace DevExpress.Xpf.Docking {
	[TargetType(typeof(LayoutGroup))]
	[TargetType(false, typeof(DocumentGroup))]
	public class DockingDocumentUIService : DockingDocumentUIServiceBase<LayoutPanel, LayoutGroup> {
		#region Dependency Properties
		public static readonly DependencyProperty LayoutPanelStyleProperty =
			DependencyProperty.Register("LayoutPanelStyle", typeof(Style), typeof(DockingDocumentUIService), new PropertyMetadata(null));
		public static readonly DependencyProperty LayoutPanelStyleSelectorProperty =
			DependencyProperty.Register("LayoutPanelStyleSelector", typeof(StyleSelector), typeof(DockingDocumentUIService), new PropertyMetadata(null));
		public static readonly DependencyProperty LayoutGroupProperty =
			DependencyProperty.Register("LayoutGroup", typeof(LayoutGroup), typeof(DockingDocumentUIService), new PropertyMetadata(null));
		#endregion
		public LayoutGroup LayoutGroup { get { return (LayoutGroup)GetValue(LayoutGroupProperty); } set { SetValue(LayoutGroupProperty, value); } }
		public StyleSelector LayoutPanelStyleSelector { get { return (StyleSelector)GetValue(LayoutPanelStyleSelectorProperty); } set { SetValue(LayoutPanelStyleSelectorProperty, value); } }
		public Style LayoutPanelStyle { get { return (Style)GetValue(LayoutPanelStyleProperty); } set { SetValue(LayoutPanelStyleProperty, value); } }
		public LayoutGroup ActualLayoutGroup { get { return AssociatedObject as LayoutGroup ?? LayoutGroup; } }
		protected override LayoutPanel CreateDocumentPanel() { return new LayoutPanel(); }
		protected override LayoutGroup GetActualDocumentGroup() { return ActualLayoutGroup; }
		protected override Style GetDocumentPanelStyle(LayoutPanel documentPanel, object documentContentView) {
			return GetDocumentContainerStyle(documentPanel, documentContentView, LayoutPanelStyle, LayoutPanelStyleSelector);
		}
	}
}
