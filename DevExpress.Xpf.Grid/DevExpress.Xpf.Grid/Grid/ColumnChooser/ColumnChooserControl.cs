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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.Collections;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class ColumnChooserControl : ColumnChooserControlBase {
		public static readonly DependencyProperty DragTextProperty = DependencyProperty.Register("DragText", typeof(string), typeof(ColumnChooserControl), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(IList), typeof(ColumnChooserControl), new PropertyMetadata(null));
		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ColumnChooserControl), new PropertyMetadata(null));
		public ColumnChooserControl() {
			this.SetDefaultStyleKey(typeof(ColumnChooserControl));
		}
		public IList Columns {
			get { return (IList)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}
		public string DragText {
			get { return (string)GetValue(DragTextProperty); }
			set { SetValue(DragTextProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		protected override void OnOwnerChanged(ILogicalOwner oldView, ILogicalOwner newView) {
			DataControlBase.SetCurrentViewInternal(this, (DataViewBase)newView);
			base.OnOwnerChanged(oldView, newView);
		}
		protected override ILogicalOwner GetLogicalOwnerCore() {
			return ((DataViewBase)Owner).RootView;
		}
	}
}
