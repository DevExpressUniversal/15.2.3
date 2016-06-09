#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
namespace DevExpress.ExpressApp.Design.Core {
	public static class CommandIds {
		public const int icmdForward = 0x0001;
		public const int icmdBack = 0x0002;
		public const int icmdLanguage = 0x0003;
		public const int icmdLanguageGetList = 0x0004;
		public const int icmdSearch = 0x0005;
		public const int icmdLocalization = 0x0006;
		public const int icmdReload = 0x0007;
		public const int icmdShowUnusableData = 0x0008;
		public const int EasyTestRunCommandId = 0x1001;
		public const int EasyTestRunNextStepCommandId = 0x1002;
		public const int EasyTestRunToCursorCommandId = 0x1003;
		public const int EasyTestStopRunningCommandId = 0x1004;
		public const int cmdidUpdateModelCommand = 0x2001;
		public const int cmdidUpdateModelsCommand = 0x2002;
		public const int cmdidOpenModelEditorCommand = 0x2003;
		public const int cmdidMergeDifferencesCommand = 0x2004;
		public const int cmdidMergeUserDifferencesCommand = 0x2005;
	}
}
