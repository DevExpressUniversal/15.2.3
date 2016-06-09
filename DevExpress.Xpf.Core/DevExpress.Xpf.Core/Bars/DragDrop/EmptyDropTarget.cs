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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars.Customization;
using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public class BarEmptyDropTarget : IDropTarget {
		public BarManager Manager { get; set; }
		#region IDropTarget Members
		void IDropTarget.Drop(UIElement source, Point pt) {
			var li = BarDragDropElementHelper.Current.With(x => x.Owner as BarItemLinkControl).With(x => x.Link);
			if (li != null) {
				BarNameScope.GetService<ICustomizationService>(li).With(x => x.CustomizationHelper.Strategy).Do(x => x.OnRemoveLink(li));
			}
		}
		void IDropTarget.OnDragLeave() {
		}
		void IDropTarget.OnDragOver(System.Windows.UIElement source, System.Windows.Point pt) {
			if(!(source is BarItemList))
				source = BarDragDropElementHelper.Current.With(x => x.Owner as BarItemLinkControl);
			BarDragProvider.SetDragTypeCore(source, DragType.Remove);
		}
		#endregion
	}
}
