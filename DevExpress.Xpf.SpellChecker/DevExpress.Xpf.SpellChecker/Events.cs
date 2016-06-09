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
using System.ComponentModel;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
#if SL || WPF
using DevExpress.Xpf.SpellChecker.Forms;
using System.Windows.Input;
using System.Windows;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#endif
namespace DevExpress.XtraSpellChecker {
	public enum PaintReason { CharPressed, LayoutChanged, Unknown };
	public delegate void PaintEventHandler(object sender, PaintEventArgs e);
	public class PaintEventArgs : EventArgs {
		PaintReason paintReason = PaintReason.Unknown;
		public PaintEventArgs(PaintReason paintReason) {
			this.paintReason = paintReason;
		}
		public PaintReason PaintReason { get { return this.paintReason; } }
	}
	public delegate void NeedPaintEventHandler(object sender, NeedPaintEventArgs e);
	public class NeedPaintEventArgs : EventArgs {
		bool layoutChanged = false;
		bool textChanged = false;
		public NeedPaintEventArgs(bool textChanged, bool layoutChanged) {
			this.layoutChanged = layoutChanged;
			this.textChanged = textChanged;
		}
		public bool LayoutChanged { get { return layoutChanged; } }
		public bool TextChanged { get { return textChanged; } }
	}
	public delegate void ScrollingEventHandler(object sender, ScrollingEventArgs e);
	public enum ScrollReason { Keyboard, Mouse, Wheel }
	public enum ScrollDirection { Up, Down, Undefined }
	public class ScrollingEventArgs : EventArgs {
		ScrollReason scrollReason = ScrollReason.Mouse;
		ScrollDirection scrollDirection = ScrollDirection.Undefined;
		public ScrollingEventArgs(ScrollReason scrollReason, ScrollDirection direction) {
			this.scrollReason = scrollReason;
			this.scrollDirection = direction;
		}
		public ScrollReason ScrollReason { get { return scrollReason; } }
		public ScrollDirection ScrollDirection { get { return scrollDirection; } }
	}
#if SL || WPF
	public class WindowClosedEventArgs : EventArgs {
		bool dialogResult = false;
		public WindowClosedEventArgs(bool dialogResult) {
			this.dialogResult = dialogResult;
		}
		public bool DialogResult {
			get { return dialogResult; }
		}
	}
	public delegate void WindowClosedEventHandler(object sender, WindowClosedEventArgs e);
	#region ControlCreatedEventArgs<TControl>
	public class ControlCreatedEventArgs<TControl> : EventArgs {
		public ControlCreatedEventArgs(TControl control) {
			Control = control;
		}
		public virtual TControl Control { get; set; }
	}
	#endregion
	public class SpellingControlBaseEventArgs : ControlCreatedEventArgs<SpellingControlBase> {
		public SpellingControlBaseEventArgs(SpellingControlBase control) : base(control) { }
	}
	public class SpellingOutlookStyleEventArgs : SpellingControlBaseEventArgs {
		public SpellingOutlookStyleEventArgs(SpellingOutlookStyleControl control) : base(control) { }
		public new SpellingOutlookStyleControl Control { get { return base.Control as SpellingOutlookStyleControl; } set { base.Control = value; } }
	}
	public class SpellingWordStyleEventArgs : SpellingControlBaseEventArgs {
		public SpellingWordStyleEventArgs(SpellingWordStyleControl control) : base(control) { }
		public new SpellingWordStyleControl Control { get { return base.Control as SpellingWordStyleControl; } set { base.Control = value; } }
	}
	public class SpellingOptionsEventArgs : ControlCreatedEventArgs<SpellingOptionsControl> {
		public SpellingOptionsEventArgs(SpellingOptionsControl control) : base(control) { }
	}
	public class CustomDictionaryEventArgs : ControlCreatedEventArgs<CustomDictionaryControl> {
		public CustomDictionaryEventArgs(CustomDictionaryControl control) : base(control) { }
	}
	public delegate void SpellingOutlookStyleEventHandler(object sender, SpellingOutlookStyleEventArgs e);
	public delegate void SpellingWordStyleEventHandler(object sender, SpellingWordStyleEventArgs e);
	public delegate void SpellingOptionsEventHandler(object sender, SpellingOptionsEventArgs e);
	public delegate void CustomDictionaryEventHandler(object sender, CustomDictionaryEventArgs e);
	public delegate void SelectionStartChangedEventHandler(object sender, SelectionStartChangedEventArgs e);
	public enum SelectionChangeReason { Keyboard, Mouse }
	#region SelectionStartChangedEventArgs
#if !SL
	public class SelectionStartChangedEventArgs : RoutedEventArgs {
#else
	public class SelectionStartChangedEventArgs : SLRoutedEventArgs {
#endif
		Position selectionStart;
		SelectionChangeReason reason;
		Key key;
		public SelectionStartChangedEventArgs(Position selectionStart, SelectionChangeReason reason, Key key) {
			this.selectionStart = selectionStart;
			this.reason = reason;
			this.key = key;
		}
		public Position SelectionStart { get { return this.selectionStart; } }
		public SelectionChangeReason Reason { get { return this.reason; } }
		public Key Key { get { return this.key; } }
	}
	#endregion
#endif
}
