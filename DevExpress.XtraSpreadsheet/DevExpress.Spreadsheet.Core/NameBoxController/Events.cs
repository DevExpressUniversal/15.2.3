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
using System.Linq;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class NameBoxController {
		#region SelectionChanged
		EventHandler onSelectionChanged;
		public event EventHandler SelectionChanged { add { onSelectionChanged += value; } remove { onSelectionChanged -= value; } }
		protected internal virtual void RaiseSelectionChanged() {
			if (onSelectionChanged != null)
				onSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region VisibleDefinedNamesChanged
		VisibleDefinedNamesChangedEventHandler onVisibleDefinedNamesChanged;
		public event VisibleDefinedNamesChangedEventHandler VisibleDefinedNamesChanged { add { onVisibleDefinedNamesChanged += value; } remove { onVisibleDefinedNamesChanged -= value; } }
		protected internal virtual void RaiseVisibleDefinedNamesChanged() {
			if (onVisibleDefinedNamesChanged != null) {
				List<string> names = GetNames();
				VisibleDefinedNamesChangedEventArgs args = new VisibleDefinedNamesChangedEventArgs(names);
				onVisibleDefinedNamesChanged(this, args);
			}
		}
		List<string> GetNames() {
			List<string> names = new List<string>();
			foreach (DefinedNameBase name in VisibleDefinedNames)
				names.Add(name.Name);
			foreach (Table table in Tables)
				names.Add(table.Name);
			return names;
		}
		#endregion
	}
}
