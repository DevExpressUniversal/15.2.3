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
using System.Windows.Input;
using DevExpress.Office.Design.Internal;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Office.UI;
using DevExpress.Xpf.Spreadsheet;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Utils.Commands;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Native;
#else
using DevExpress.Xpf.Utils;
using System.Drawing.Printing;
#endif
namespace DevExpress.Xpf.Spreadsheet.UI {
	#region PaperKindBarListItem
	public class PaperKindBarListItem : PaperKindBarListItemBase, ISpreadsheetControlDependencyPropertyOwner {
		static PaperKindBarListItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(PaperKindBarListItem), typeof(PaperKindBarListItemLink), delegate(object arg) { return new PaperKindBarListItemLink(); });
		}
		#region Properties
		#region SpreadsheetControl
		public static readonly DependencyProperty SpreadsheetControlProperty = DependencyPropertyManager.Register("SpreadsheetControl", typeof(SpreadsheetControl), typeof(PaperKindBarListItem), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSpreadsheetControlChanged)));
		protected static void OnSpreadsheetControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PaperKindBarListItem instance = d as PaperKindBarListItem;
			if (instance != null)
				instance.OnSpreadsheetControlChanged((SpreadsheetControl)e.OldValue, (SpreadsheetControl)e.NewValue);
		}
		public SpreadsheetControl SpreadsheetControl {
			get { return (SpreadsheetControl)GetValue(SpreadsheetControlProperty); }
			set { SetValue(SpreadsheetControlProperty, value); }
		}
		protected internal virtual void OnSpreadsheetControlChanged(SpreadsheetControl oldValue, SpreadsheetControl newValue) {
			UpdateItems();
		}
		#endregion
		#region ISpreadsheetControlDependencyPropertyOwner Members
		DependencyProperty ISpreadsheetControlDependencyPropertyOwner.DependencyProperty { get { return SpreadsheetControlProperty; } }
		#endregion
		protected override Control Control { get { return SpreadsheetControl; } }
		protected override IList<PaperKind> DefaultPaperKindList { get { return PageSetupSetPaperKindCommand.DefaultPaperKindList; } }
		#endregion
		protected override ICommand CreateChangeSectionPaperKindCommand(PaperKind paperKind) {
			return new ChangePaperKindSpreadsheetUICommand(paperKind);
		}
		protected override string ObtainPaperKindCaption(PaperKind paperKind, string displayName) {
			if (SpreadsheetControl == null)
				return String.Empty;
			return PaperKindDescriptionHelper.CalculateDescription(paperKind, displayName, SpreadsheetControl.Unit, SpreadsheetControl.DocumentModel.InternalAPI.UnitConverters, SpreadsheetControl.DocumentModel.UnitConverter);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChangePaperKindSpreadsheetUICommand
	public class ChangePaperKindSpreadsheetUICommand : SpreadsheetUICommand {
		readonly PaperKind paperKind;
		public ChangePaperKindSpreadsheetUICommand(PaperKind paperKind)
			: base(SpreadsheetCommandId.PageSetupSetPaperKind) {
			this.paperKind = paperKind;
		}
		protected internal override void ExecuteCommand(SpreadsheetControl control, SpreadsheetCommandId commandId, object parameter) {
			base.ExecuteCommand(control, commandId, paperKind);
		}
	}
	#endregion
}
