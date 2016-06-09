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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.Office.Services.Implementation;
#if !SL
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
#else
#endif
namespace DevExpress.XtraRichEdit.Internal {
	#region DocumentDeferredChanges
	public class DocumentDeferredChanges {
		RunIndex startRunIndex;
		RunIndex endRunIndex;
		DocumentModelChangeActions changeActions;
		public DocumentModelChangeActions ChangeActions { get { return changeActions; } set { changeActions = value; } }
		public RunIndex StartRunIndex { get { return startRunIndex; } set { startRunIndex = value; } }
		public RunIndex EndRunIndex { get { return endRunIndex; } set { endRunIndex = value; } }
	}
	#endregion
	#region RichEditControlDeferredChanges
	public class RichEditControlDeferredChanges {
		bool redraw;
		bool resize;
		bool raiseUpdateUI;
		public bool Redraw { get { return redraw; } set { redraw = value; } }
		public bool Resize { get { return resize; } set { resize = value; } }
		public bool RaiseUpdateUI { get { return raiseUpdateUI; } set { raiseUpdateUI = value; } }
		RefreshAction redrawAction;
		public RefreshAction RedrawAction { get { return redrawAction; } set { redrawAction = value; } }
	}
	#endregion
}
