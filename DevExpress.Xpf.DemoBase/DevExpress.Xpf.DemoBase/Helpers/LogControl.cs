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

using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public interface ILogService {
		void LogLine(string line);
	}
	public class LogService : ServiceBase, ILogService {
		public void LogLine(string line) {
			((LogControl)AssociatedObject).LogLine(line);
		}
	}
	public class LogControl : Control {
		TextEdit textBox;
		Queue<string> logQueue = new Queue<string>();
		const int logEntriesCount = 100;
		static LogControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LogControl), new FrameworkPropertyMetadata(typeof(LogControl)));
		}
		public override void OnApplyTemplate() {
			((Button)GetTemplateChild("clearButton")).Click += ClearButton_Click;
			textBox = (TextEdit)GetTemplateChild("textBox");
		}
		public void LogLine(string str) {
			if(!IsLoaded)
				return;
			logQueue.Enqueue(str);
			if(logQueue.Count > logEntriesCount)
				logQueue.Dequeue();
			textBox.Text = string.Empty;
			StringBuilder builder = new StringBuilder();
			foreach(string text in logQueue) {
				builder.Append((builder.Length != 0 ? Environment.NewLine : string.Empty) + text);
			}
			textBox.Text = builder.ToString();
			TextBox textBoxCore = (TextBox)LayoutHelper.FindElement(textBox, (element) => (element is TextBox));
			textBoxCore.ScrollToEnd();
		}
		void ClearButton_Click(object sender, RoutedEventArgs e) {
			logQueue.Clear();
			textBox.Text = String.Empty;
		}
	}
}
