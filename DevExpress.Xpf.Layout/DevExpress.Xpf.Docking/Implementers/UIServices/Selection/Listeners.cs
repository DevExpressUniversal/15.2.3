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

using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Selection;
namespace DevExpress.Xpf.Docking.Platform {
#if DEBUGTEST
	static class TestParams {
		public static SelectionMode SelectionMode;
	}
#endif
	public class LayoutViewSelectionListener : SelectionListener {
		public LayoutView View {
			get { return ServiceProvider as LayoutView; }
		}
		public override SelectionMode CheckMode(ILayoutElement item) {
#if DEBUGTEST
			return TestParams.SelectionMode;
#else
			if(KeyHelper.IsCtrlPressed) return SelectionMode.MultipleItems;
			if(KeyHelper.IsShiftPressed) return SelectionMode.ItemRange;
			return SelectionMode.SingleItem;
#endif
		}
		public override bool OnSelectionChanging(ILayoutElement element, bool selected) {
			BaseLayoutItem itemToSelect = ((IDockLayoutElement)element).Item;
			return !View.Container.RaiseItemSelectionChangingEvent(itemToSelect, selected);
		}
		public override void OnSelectionChanged(ILayoutElement element, bool selected) {
			BaseLayoutItem itemToSelect = ((IDockLayoutElement)element).Item;
			itemToSelect.SetSelected(View.Container, selected);
		}
	}
}
