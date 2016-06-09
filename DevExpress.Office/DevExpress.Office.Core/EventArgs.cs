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

using DevExpress.Compatibility.System.Windows.Forms;
using System.Windows.Forms;
namespace DevExpress.Office {
	#region OfficeMouseEventArgs
	public class OfficeMouseEventArgs : MouseEventArgs {
		public static OfficeMouseEventArgs Convert(MouseEventArgs e) {
			OfficeMouseEventArgs result = e as OfficeMouseEventArgs;
			if (result == null)
				result = new OfficeMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta);
			return result;
		}
		bool horizontal;
		public OfficeMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, bool horizontal)
			: base(button, clicks, x, y, delta) {
			this.horizontal = horizontal;
		}
		public OfficeMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
			: this(button, clicks, x, y, delta, false) {
		}
		public bool Horizontal { get { return horizontal; } }
	}
	#endregion
}
