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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Design.Internal;
using DevExpress.Xpf.Office.UI;
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
namespace DevExpress.Xpf.RichEdit.UI {
	#region PaperKindBarListItem
	public class PaperKindBarListItem : PaperKindBarListItemBase, IRichEditControlDependencyPropertyOwner {
		static PaperKindBarListItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(PaperKindBarListItem), typeof(PaperKindBarListItemLink), delegate(object arg) { return new PaperKindBarListItemLink(); });
		}
		#region Properties
		#region RichEditControl
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(PaperKindBarListItem), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRichEditControlChanged)));
		protected static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PaperKindBarListItem instance = d as PaperKindBarListItem;
			if (instance != null)
				instance.OnRichEditControlChanged((RichEditControl)e.OldValue, (RichEditControl)e.NewValue);
		}
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		protected internal virtual void OnRichEditControlChanged(RichEditControl oldValue, RichEditControl newValue) {
			UpdateItems();
		}
		#endregion
		#region IRichEditControlDependencyPropertyOwner Members
		DependencyProperty IRichEditControlDependencyPropertyOwner.DependencyProperty { get { return RichEditControlProperty; } }
		#endregion
		protected override Control Control { get { return RichEditControl; } }
		protected override IList<PaperKind> DefaultPaperKindList { get { return ChangeSectionPaperKindCommand.DefaultPaperKindList; } }
		#endregion
		protected override ICommand CreateChangeSectionPaperKindCommand(PaperKind paperKind) {
			return new ChangeSectionPaperKindRichEditUICommand(paperKind);
		}
		protected override string ObtainPaperKindCaption(PaperKind paperKind, string displayName) {
			if (RichEditControl == null)
				return String.Empty;
			return PaperKindDescriptionHelper.CalculateDescription(paperKind, displayName, RichEditControl.Unit, RichEditControl.DocumentModel.InternalAPI.UnitConverters, RichEditControl.DocumentModel.UnitConverter);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeSectionPaperKindRichEditUICommand
	public class ChangeSectionPaperKindRichEditUICommand : RichEditUICommand {
		readonly PaperKind paperKind;
		public ChangeSectionPaperKindRichEditUICommand(PaperKind paperKind)
			: base(RichEditCommandId.ChangeSectionPaperKind) {
			this.paperKind = paperKind;
		}
		protected internal override void ExecuteCommand(RichEditControl control, RichEditCommandId commandId, object parameter) {
			base.ExecuteCommand(control, commandId, paperKind);
		}
	}
	#endregion
}
