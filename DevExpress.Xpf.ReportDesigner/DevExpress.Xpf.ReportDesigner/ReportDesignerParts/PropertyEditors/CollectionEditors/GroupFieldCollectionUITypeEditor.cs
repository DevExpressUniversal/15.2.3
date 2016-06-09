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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Reports.UserDesigner.Editors.Native;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm;
using DevExpress.Xpf.Diagram;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class SetAlternationIndexBehavior : Behavior<ListBoxEdit> {
		protected override void OnDetaching() {
			base.OnDetaching();
			((ListBoxEdit)AssociatedObject).Loaded -= listBoxEdit_Loaded;
		}
		protected override void OnAttached() {
			base.OnAttached();
			((ListBoxEdit)AssociatedObject).Loaded += listBoxEdit_Loaded;
		}
		void listBoxEdit_Loaded(object sender, RoutedEventArgs e) {
			var listBox = (ListBox)((ListBoxEdit)sender).EditCore;
			listBox.AlternationCount = 10000;
		}
	}
	public class GroupFieldCollectionUITypeEditor : ExtendedSelectionCollectionEditor {
		static GroupFieldCollectionUITypeEditor() {
			DependencyPropertyRegistrator<GroupFieldCollectionUITypeEditor>.New()
			  .OverrideDefaultStyleKey()
			;
		}
		public override object CreateItem() {
			return new GroupField();
		}
	}
}
