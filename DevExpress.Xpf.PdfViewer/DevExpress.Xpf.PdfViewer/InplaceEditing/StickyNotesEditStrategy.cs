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

using System.Windows.Controls;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.PdfViewer.Themes;
using DevExpress.Xpf.Editors.Helpers;
using System.Windows;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.PdfViewer {
	public class StickyNotesEditStrategy : EditStrategyBase {
		new StickyNotesEdit Editor { get { return (StickyNotesEdit)base.Editor; } }
		public StickyNotesEditStrategy(StickyNotesEdit editor)
			: base(editor) {
		}
		public override void OnLoaded() {
			base.OnLoaded();
			var tt = new ToolTip() { Content = CreateSuperTipControl() };
			Editor.ToolTip = tt;
			tt.IsOpen = true;
		}
		protected override void OnUnloaded() {
			base.OnUnloaded();
			(Editor.ToolTip as ToolTip).Do(x => x.IsOpen = false);
		}
		SuperTipControl CreateSuperTipControl() {
			var sp = new SuperTip();
			if (!string.IsNullOrEmpty(Editor.Title)) {
				SuperTipHeaderItem h = new SuperTipHeaderItem() { Content = Editor.Title };
				sp.Items.Add(h);
			}
			SuperTipItem item = new SuperTipItem() { Content = Editor.EditValue };
			sp.Items.Add(item);
			return new SuperTipControl(sp);
		}
	}
}
