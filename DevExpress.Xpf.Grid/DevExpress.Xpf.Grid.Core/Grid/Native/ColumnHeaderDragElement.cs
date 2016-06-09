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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Data;
namespace DevExpress.Xpf.Grid.Native {
	public class ColumnHeaderDragElement : HeaderDragElementBase {
		protected BaseGridHeader ColumnHeaderElement { get { return (BaseGridHeader)HeaderElement; } }
		DataViewBase OwnerView { get { return ColumnHeaderElement.GridView.RootView; } }
		protected override FrameworkElement HeaderButton {
			get { return ColumnHeaderElement.HeaderContent; }
		}
		protected override string DragElementTemplatePropertyName {
			get { return BaseGridColumnHeader.DragElementTemplatePropertyName; }
		}
		public ColumnHeaderDragElement(BaseGridHeader columnHeaderElement, Point offset)
			: base(columnHeaderElement, columnHeaderElement.CreateDragElementDataContext(), offset) {
			DataControlBase.SetCurrentViewInternal(container, (DataViewBase)columnHeaderElement.GridView);
		}
		protected override void AddGridChild(object child) {
			OwnerView.AddChild(child);
		}
		protected override void RemoveGridChild(object child) {
			OwnerView.RemoveChild(child);
		}
		protected override void SetDragElementSize(FrameworkElement elem, Size size) {
			BaseGridColumnHeader.SetDragElementSize(elem, size);
		}
		protected override void SetDragElementAllowTransparency(FrameworkElement elem, bool allowTransparency) {
			BaseGridColumnHeader.SetDragElementAllowTransparency(elem, allowTransparency);
		}
	}
}
