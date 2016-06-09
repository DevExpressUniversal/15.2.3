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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.DataAccess.Editors {
	public class ChooseEFStoredProceduresEditor : Control {
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		static ChooseEFStoredProceduresEditor() {
			DependencyPropertyRegistrator<ChooseEFStoredProceduresEditor>.New()
				.Register(d => d.EditValue, out EditValueProperty, new List<object>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.Register(d => d.ItemsSource, out ItemsSourceProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		public List<object> EditValue {
			get { return (List<object>)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public IEnumerable<StoredProcedureViewInfo> ItemsSource {
			get { return (IEnumerable<StoredProcedureViewInfo>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
	}
}
