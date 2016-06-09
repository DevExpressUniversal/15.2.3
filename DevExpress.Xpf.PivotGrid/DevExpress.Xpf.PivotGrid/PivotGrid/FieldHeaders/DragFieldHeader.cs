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
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class DragFieldHeader : FieldHeader {
		public static readonly DependencyProperty BestFitWCorrectionProperty;
		public static readonly DependencyProperty CanHideProperty;
		static DragFieldHeader() {
			CanHideProperty = DependencyProperty.RegisterAttached("CanHide", typeof(bool), typeof(DragFieldHeader), new PropertyMetadata(false));
			BestFitWCorrectionProperty = DependencyProperty.RegisterAttached("BestFitWCorrection", typeof(int), typeof(DragFieldHeader), new PropertyMetadata(1));
		}
		public static void SetCanHide(DependencyObject element, bool value) {
			element.SetValue(CanHideProperty, value);
		}
		public static bool GetCanHide(DependencyObject element) {
			return (bool)element.GetValue(CanHideProperty);
		}
		public int BestFitWCorrection {
			get { return (int)GetValue(BestFitWCorrectionProperty); }
			set { SetValue(BestFitWCorrectionProperty, value); }
		}
		protected override void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(DragFieldHeader));
		}
		protected override bool? GetIsMustBindToFieldWidth() {
			return null;
		}
		protected override FirstHeaderPosition GetIsFirst() {
			return base.GetIsFirst() == FirstHeaderPosition.RowArea ? FirstHeaderPosition.RowArea : FirstHeaderPosition.None;
		}
		protected override void EnsureChangeFieldSortCommand(bool can) { }
		protected override void CreateDragDropElementHelper() { }
		protected override void SubscribeEvents(PivotGridField field) { }
		protected override void UnsubscribeEvents(PivotGridField field) { }
	}
	public class DragInnerGroupHeader : DragFieldHeader {
		public DragInnerGroupHeader() { }
		protected override void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(DragInnerGroupHeader));
		}
	}
	public class DragGroupHeader : GroupHeader {
		public DragGroupHeader() { }
		protected override void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(DragGroupHeader));
		}
		protected override FieldHeader CreateHeader() {
			return new DragInnerGroupHeader();
		}
		protected override bool? GetIsMustBindToFieldWidth() {
			return null;
		}
		protected override FirstHeaderPosition GetIsFirst() {
			return base.GetIsFirst() == FirstHeaderPosition.RowArea ? FirstHeaderPosition.RowArea : FirstHeaderPosition.None;
		}
		protected override void SubscribeEvents(PivotGridField field) { }
		protected override void UnsubscribeEvents(PivotGridField field) { }
		protected override void CreateDragDropElementHelper() { }
	}
}
