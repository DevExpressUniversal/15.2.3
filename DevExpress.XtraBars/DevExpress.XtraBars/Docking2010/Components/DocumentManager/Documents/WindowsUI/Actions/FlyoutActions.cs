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
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	using System.Windows.Forms;
	using System.Drawing;
	public class FlyoutAction : UIActionPropertiesCore, IContentContainerActionList {
		public FlyoutAction() {
			Commands = new List<FlyoutCommand>();
		}
		public FlyoutAction(DialogResult[] commands)
			: this() {
			for(int i = 0; i < commands.Length; i++)
				Commands.Add(FlyoutCommand.FromDialogResult(commands[i]));
		}
		public IList<FlyoutCommand> Commands { get; private set; }
		IEnumerable<ICommand<IContentContainer>> IUIActionList<IContentContainer>.Commands {
			get { return Commands; }
		}
	}
	public class FlyoutCommand : ICommand<Flyout>, ICommand<IContentContainer> {
		#region static
		public readonly static FlyoutCommand Abort;
		public readonly static FlyoutCommand Cancel;
		public readonly static FlyoutCommand Ignore;
		public readonly static FlyoutCommand No;
		public readonly static FlyoutCommand OK;
		public readonly static FlyoutCommand Retry;
		public readonly static FlyoutCommand Yes;
		static FlyoutCommand() {
			FlyoutCommand.Abort = new AbortCommand();
			FlyoutCommand.Cancel = new CancelCommand();
			FlyoutCommand.Ignore = new IgnoreCommand();
			FlyoutCommand.No = new NoCommand();
			FlyoutCommand.OK = new OKCommand();
			FlyoutCommand.Retry = new RetryCommand();
			FlyoutCommand.Yes = new YesCommand();
		}
		#endregion static
		Predicate<Flyout> canExecuteCore;
		Action<Flyout> executeCore;
		public FlyoutCommand() :
			this((Predicate<Flyout>)null, (Action<Flyout>)null) {
		}
		public FlyoutCommand(Func<bool> canExecute, Action execute) :
			this((Flyout f) => { return canExecute(); }, (Flyout f) => execute()) {
		}
		public FlyoutCommand(Predicate<Flyout> canExecute, Action<Flyout> execute) {
			this.canExecuteCore = canExecute;
			this.executeCore = execute;
			this.Result = DialogResult.None;
		}
		public virtual string Text { get; set; }
		public virtual Image Image { get; set; }
		public virtual DialogResult Result { get; set; }
		bool ICommand<IContentContainer>.CanExecute(IContentContainer parameter) {
			return CanExecute((Flyout)parameter);
		}
		void ICommand<IContentContainer>.Execute(IContentContainer parameter) {
			Execute((Flyout)parameter);
		}
		public virtual bool CanExecute(Flyout parameter) {
			if(canExecuteCore != null) return canExecuteCore(parameter);
			return true;
		}
		public virtual void Execute(Flyout parameter) {
			if(executeCore != null)
				executeCore(parameter);
		}
		internal static FlyoutCommand FromDialogResult(DialogResult dialogResult) {
			switch(dialogResult) {
				case DialogResult.Abort: return FlyoutCommand.Abort;
				case DialogResult.Cancel: return FlyoutCommand.Cancel;
				case DialogResult.Ignore: return FlyoutCommand.Ignore;
				case DialogResult.No: return FlyoutCommand.No;
				case DialogResult.OK: return FlyoutCommand.OK;
				case DialogResult.Retry: return FlyoutCommand.Retry;
				case DialogResult.Yes: return FlyoutCommand.Yes;
			}
			throw new NotSupportedException(dialogResult.ToString());
		}
	}
	public class DialogResultFlyoutCommand : FlyoutCommand {
		DialogResult resultCore;
		public DialogResultFlyoutCommand(DialogResult result) {
			resultCore = result;
		}
		public sealed override string Text {
			get { return DialogResultFlyoutCommandLocalizer.GetText(resultCore); }
		}
		public sealed override DialogResult Result { get { return resultCore; } }
		#region static
		static class DialogResultFlyoutCommandLocalizer {
			public static string GetText(DialogResult target) {
				string result;
				switch(target) {
					case DialogResult.Abort:
						result = XtraEditors.Controls.Localizer.Active.GetLocalizedString(XtraEditors.Controls.StringId.XtraMessageBoxAbortButtonText);
						break;
					case DialogResult.Cancel:
						result = XtraEditors.Controls.Localizer.Active.GetLocalizedString(XtraEditors.Controls.StringId.XtraMessageBoxCancelButtonText);
						break;
					case DialogResult.Ignore:
						result = XtraEditors.Controls.Localizer.Active.GetLocalizedString(XtraEditors.Controls.StringId.XtraMessageBoxIgnoreButtonText);
						break;
					case DialogResult.No:
						result = XtraEditors.Controls.Localizer.Active.GetLocalizedString(XtraEditors.Controls.StringId.XtraMessageBoxNoButtonText);
						break;
					case DialogResult.OK:
						result = XtraEditors.Controls.Localizer.Active.GetLocalizedString(XtraEditors.Controls.StringId.XtraMessageBoxOkButtonText);
						break;
					case DialogResult.Retry:
						result = XtraEditors.Controls.Localizer.Active.GetLocalizedString(XtraEditors.Controls.StringId.XtraMessageBoxRetryButtonText);
						break;
					case DialogResult.Yes:
						result = XtraEditors.Controls.Localizer.Active.GetLocalizedString(XtraEditors.Controls.StringId.XtraMessageBoxYesButtonText);
						break;
					default:
						return target.ToString();
				}
				return result.Trim('&');
			}
		}
		#endregion static
	}
	#region Commands
	public class OKCommand : DialogResultFlyoutCommand {
		public OKCommand() : base(DialogResult.OK) { }
	}
	public class CancelCommand : DialogResultFlyoutCommand {
		public CancelCommand() : base(DialogResult.Cancel) { }
	}
	public class AbortCommand : DialogResultFlyoutCommand {
		public AbortCommand() : base(DialogResult.Abort) { }
	}
	public class RetryCommand : DialogResultFlyoutCommand {
		public RetryCommand() : base(DialogResult.Retry) { }
	}
	public class IgnoreCommand : DialogResultFlyoutCommand {
		public IgnoreCommand() : base(DialogResult.Ignore) { }
	}
	public class YesCommand : DialogResultFlyoutCommand {
		public YesCommand() : base(DialogResult.Yes) { }
	}
	public class NoCommand : DialogResultFlyoutCommand {
		public NoCommand() : base(DialogResult.No) { }
	}
	#endregion Commands
}
