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
using DevExpress.Spreadsheet;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class FormulaBarController {		
		#region SelectionChanged
		EventHandler onSelectionChanged;
		public event EventHandler SelectionChanged { add { onSelectionChanged += value; } remove { onSelectionChanged -= value; } }
		protected internal virtual void RaiseSelectionChanged() {
			if (onSelectionChanged != null)
				onSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
	}
	#region VisibleDefinedNamesChangedEventHandler
	public delegate void VisibleDefinedNamesChangedEventHandler(object sender, VisibleDefinedNamesChangedEventArgs e);
	public class VisibleDefinedNamesChangedEventArgs : EventArgs {
		readonly List<string> visibleDefinedNames;
		public VisibleDefinedNamesChangedEventArgs(List<string> visibleDefinedNames) {
			this.visibleDefinedNames = visibleDefinedNames;
		}
		public List<string> VisibleDefinedNames { get { return visibleDefinedNames; } }
	}
	#endregion
	#region TextChangedEventHandler
	public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);
	public class TextChangedEventArgs : EventArgs {
		readonly string text;
		public TextChangedEventArgs(string text) {
			this.text = text;
		}
		public string Text { get { return text; } }
	}
	#endregion
}
