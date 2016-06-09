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
#if !SL
using Platform::System.CodeDom.Compiler;
#endif
using Platform::DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.RichEdit.Design {
#if !SL
	[GeneratedCode("Suppress FxCop check", "")]
#endif
	public static partial class BarInfos {
		#region File
		public static BarInfo File { get { return file; } }
		static readonly BarInfo file = new BarInfo(
			String.Empty,
			"File",
			"Common",
			new BarInfoItems(
				new string[] { "FileNew", "FileOpen",
#if !SILVERLIGHT
					"FileSave",
#endif
					"FileSaveAs",
#if !SILVERLIGHT
					"FileQuickPrint", 
#endif
					"FilePrint",
					"FilePrintPreview",
#if SILVERLIGHT
					"FileBrowserPrint", "FileBrowserPrintPreview",
#endif
					"EditUndo", "EditRedo"
				},
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button,
#if !SILVERLIGHT
					BarItemInfos.Button, 
#endif
					BarItemInfos.Button, 
#if !SILVERLIGHT
					BarItemInfos.Button, 
#endif
					BarItemInfos.Button, 
					BarItemInfos.Button, 
#if SILVERLIGHT
					BarItemInfos.Button, BarItemInfos.Button,
#endif
					BarItemInfos.Button, BarItemInfos.Button,
				}
			),
			String.Empty,
			String.Empty,
			"Caption_PageFile",
			"Caption_GroupCommon"
		);
		#endregion
	}
}
