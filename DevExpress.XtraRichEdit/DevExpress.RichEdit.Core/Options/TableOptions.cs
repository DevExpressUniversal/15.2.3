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
using System.Runtime.InteropServices;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	#region RichEditFormattingMarkVisibility
	[ComVisible(true)]
	public enum RichEditTableGridLinesVisibility {
		Auto,
		Visible,
		Hidden,
		VisibleWhileDragging
	}
	#endregion
	#region TableOptions
	[ComVisible(true)]
	public class TableOptions : RichEditNotificationOptions {
		#region Fields
		const RichEditTableGridLinesVisibility defaultGridLinesVisibility = RichEditTableGridLinesVisibility.Auto;
		RichEditTableGridLinesVisibility gridLines;
		#endregion
		#region Properties
		#region GridLines
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("TableOptionsGridLines"),
#endif
		NotifyParentProperty(true), DefaultValue(defaultGridLinesVisibility), XtraSerializableProperty()]
		public RichEditTableGridLinesVisibility GridLines {
			get { return gridLines; }
			set {
				if (gridLines == value)
					return;
				RichEditTableGridLinesVisibility oldValue = gridLines;
				gridLines = value;
				OnChanged("GridLines", oldValue, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			GridLines = defaultGridLinesVisibility;
		}
	}
	#endregion
}
