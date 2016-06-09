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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.XtraSpreadsheet {
	[DXToolboxItem(false)]
	public partial class FormulaBarButtons : NavigatorBase, IGetNavigatableControl {
		INavigatableControl navigatableControl;
		public FormulaBarButtons() {
			InitializeComponent();
			this.navigatableControl = null;
			SetStyle(ControlStyles.Selectable, false);
			this.ShowToolTips = true;
		}
		protected override NavigatorButtonsBase CreateButtons() {
			return new FormulaBarControlButtons(this);
		}
		[DXCategory(CategoryName.Behavior),  DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FormulaBarControlButtons Buttons { get { return ButtonsCore as FormulaBarControlButtons; } }
		[DXCategory(CategoryName.Behavior),  DefaultValue(null)]
		public virtual INavigatableControl NavigatableControl {
			get { return navigatableControl; }
			set {
				if (value == navigatableControl)
					return;
				if (navigatableControl != null)
					navigatableControl.RemoveNavigator(this);
				this.navigatableControl = value;
				if (navigatableControl != null)
					navigatableControl.AddNavigator(this);
			}
		}
		public override string ToString() {
			return string.Empty;
		}
		protected override int RecordCount { get { return NavigatableControl != null ? NavigatableControl.RecordCount : 0; } }
		protected override int CurrentRecordIndex { get { return NavigatableControl != null && RecordCount > 0 ? NavigatableControl.Position + 1 : 0; } }
		INavigatableControl IGetNavigatableControl.Control { get { return NavigatableControl; } }
	}
}
