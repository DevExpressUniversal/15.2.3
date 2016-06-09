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
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
namespace DevExpress.Charts.Designer.Native {
	public class BarButtonViewModel : RibbonItemViewModelBase {
		public static readonly DependencyProperty commandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(BarButtonViewModel));
		ICommand command;
		RibbonItemStyles ribbonStyle = RibbonItemStyles.Large | RibbonItemStyles.SmallWithText;
		ImageSource glyph;
		ImageSource largeGlyph;
		public object CommandParameter {
			get { return GetValue(commandParameterProperty); }
			set { SetValue(commandParameterProperty, value); }
		}
		public RibbonItemStyles RibbonStyle {
			get { return ribbonStyle; }
			set {
				if (ribbonStyle != value) {
					ribbonStyle = value;
					OnPropertyChanged("RibbonStyle");
				}
			}
		}
		public ImageSource Glyph {
			get { return glyph; }
			set {
				if (glyph != value) {
					glyph = value;
					OnPropertyChanged("Glyph");
				}
			}
		}
		public ImageSource LargeGlyph {
			get { return largeGlyph; }
			set {
				if (largeGlyph != value) {
					largeGlyph = value;
					OnPropertyChanged("LargeGlyph");
				}
			}
		}
		public ICommand Command { 
			get { return command; }
			set {
				if (command != value) {
					command = value;
					OnPropertyChanged("Command");
				}
			}
		}
		public BarButtonViewModel(ChartCommandBase command, IRibbonBehavior behavior) {
			Command = command;
			if (Command != null) {
				command.RegisterForUpdate();
				if (!string.IsNullOrEmpty(command.ImageName)) {
					Glyph = GlyphUtils.GetGlyphByPath(command.ImageName);
					LargeGlyph = GlyphUtils.GetLargeGlyphByPath(command.ImageName);
				}
				this.Caption = command.Caption;
				Command.CanExecuteChanged += new EventHandler(CommandCanExecuteChanged);
				UpdateIsEnabled();
			}
			if (behavior != null)
				behavior.Initialize(this);
		}
		public BarButtonViewModel(ChartCommandBase command) : this(command, null) { }
		void CommandCanExecuteChanged(object sender, EventArgs e) {
			UpdateIsEnabled();
		}
		void UpdateIsEnabled() {
			IsEnabled = Command != null ? Command.CanExecute(CommandParameter) : false;
		}
		public void UpdateVisibility(object sender, EventArgs e) {
			IsVisible = Command != null ? Command.CanExecute(CommandParameter) : false;
		}
		public override void CleanUp() {
			base.CleanUp();
			if (Command != null) {
				Command.CanExecuteChanged -= CommandCanExecuteChanged;
				Command = null;
			}
		}
	}
}
