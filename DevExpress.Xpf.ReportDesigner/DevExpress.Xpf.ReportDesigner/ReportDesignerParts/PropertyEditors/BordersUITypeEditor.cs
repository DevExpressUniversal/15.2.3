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
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.XtraPrinting;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class BordersUITypeEditor : Control {
		public static readonly DependencyProperty EditValueProperty;
		static BordersUITypeEditor() {
			DependencyPropertyRegistrator<BordersUITypeEditor>.New()
				.Register(d => d.EditValue, out EditValueProperty, BorderSide.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.OverrideDefaultStyleKey()
			;
		}
		public BordersUITypeEditor() {
			selectBordersCommand = DelegateCommandFactory.Create<BorderSide>(SelectBorders);
		}
		readonly ICommand selectBordersCommand;
		public ICommand SelectBordersCommand { get { return selectBordersCommand; } }
		public BorderSide EditValue {
			get { return (BorderSide)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		void SelectBorders(BorderSide borders) {
			if(borders == BorderSide.All) {
				EditValue = borders;
				return;
			}
			if(borders == BorderSide.None) {
				EditValue = borders;
				return;
			}
			if((EditValue & borders) != BorderSide.None)
				EditValue ^= borders;
			else
				EditValue |= borders;
		}
	}
}
