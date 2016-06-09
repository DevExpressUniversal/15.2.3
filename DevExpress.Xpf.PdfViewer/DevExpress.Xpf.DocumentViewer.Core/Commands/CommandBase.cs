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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using System;
using System.Windows.Input;
namespace DevExpress.Xpf.DocumentViewer {
	public class CommandWrapper : ICommand {
		Func<ICommand> GetCommand { get; set; }
		event EventHandler ICommand.CanExecuteChanged { add { } remove { } }
		public CommandWrapper(Func<ICommand> getCommand) {
			GetCommand = getCommand;
		}
		public bool CanExecute(object parameter) {
			return GetCommand().Return(x => x.CanExecute(parameter), () => false);
		}
		public void Execute(object parameter) {
			GetCommand().Do(x => x.Execute(parameter));
		}
	}
	public abstract class CommandBase : BindableBase, ICommand {
		ICommand command;
		Uri smallGlyph;
		Uri largeGlyph;
		string caption;
		string hint;
		string group;
		public ICommand Command {
			get { return command; }
			set { SetProperty(ref command, value, () => Command); }
		}
		public Uri SmallGlyph {
			get { return smallGlyph; }
			set { SetProperty(ref smallGlyph, value, () => SmallGlyph); }
		}
		public Uri LargeGlyph {
			get { return largeGlyph; }
			set { SetProperty(ref largeGlyph, value, () => LargeGlyph); }
		}
		public string Caption {
			get { return caption; }
			set { SetProperty(ref caption, value, () => Caption); }
		}
		public string Hint {
			get { return hint; }
			set { SetProperty(ref hint, value, () => Hint); }
		}
		public string Group {
			get { return group; }
			set { SetProperty(ref group, value, () => Group); }
		}
		public event EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
		public bool CanExecute(object parameter) {
			return CanExecuteInternal(parameter);
		}
		public void Execute(object parameter) {
			ExecuteInternal(parameter);
		}
		protected virtual bool CanExecuteInternal(object parameter) {
			return Command.Return(x => x.CanExecute(parameter), () => false);
		}
		protected virtual void ExecuteInternal(object parameter) {
			Command.Do(x => x.Execute(parameter));
		}
	}
}
