﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

extern alias Platform;
using System;
using Platform::DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.RichEdit.Design {
	public static partial class BarInfos {
		#region DocumentViews
		public static BarInfo DocumentViews { get { return documentViews; } }
		static readonly BarInfo documentViews = new BarInfo(
			String.Empty,
			"View",
			"Document Views",
			new BarInfoItems(
				new string[] { "ViewDraft", "ViewPrintLayout", "ViewSimple" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check }
			),
			String.Empty,
			String.Empty,
			"Caption_PageView",
			"Caption_GroupDocumentViews"
		);
		#endregion
		#region Show
		public static BarInfo Show { get { return show; } }
		static readonly BarInfo show = new BarInfo(
			String.Empty,
			"View",
			"Show",
			new BarInfoItems(
				new string[] { "ViewHorizontalRuler", "ViewVerticalRuler" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check }
			),
			String.Empty,
			String.Empty,
			"Caption_PageView",
			"Caption_GroupShow"
		);
		#endregion
		#region Zoom
		public static BarInfo Zoom { get { return zoom; } }
		static readonly BarInfo zoom = new BarInfo(
			String.Empty,
			"View",
			"Zoom",
			new BarInfoItems(
				new string[] { "ViewZoomOut", "ViewZoomIn" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageView",
			"Caption_GroupZoom"
		);
		#endregion
	}
}
