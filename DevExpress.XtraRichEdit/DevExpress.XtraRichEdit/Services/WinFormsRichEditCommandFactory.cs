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
using System.Reflection;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.XtraRichEdit.Services.Implementation {
	public class WinFormsRichEditCommandFactoryService : RichEditCommandFactoryService {
		static readonly Type[] constructorParametersInterface = new Type[] { typeof(RichEditControl) };
		public WinFormsRichEditCommandFactoryService(RichEditControl control)
			: base(control) {
		}
		protected internal override ConstructorInfo GetConstructorInfo(Type commandType) {
			ConstructorInfo ci = base.GetConstructorInfo(commandType);
			if (ci == null)
				ci = commandType.GetConstructor(constructorParametersInterface);
			return ci;
		}
		protected internal override void PopulateConstructorTable(RichEditCommandConstructorTable table) {
			base.PopulateConstructorTable(table);
#if !SL
			AddCommandConstructor(table, RichEditCommandId.FindNext, typeof(FindNextCommand));
			AddCommandConstructor(table, RichEditCommandId.FindPrev, typeof(FindPrevCommand));
			AddCommandConstructor(table, RichEditCommandId.FileSave, typeof(SaveDocumentCommand));
			AddCommandConstructor(table, RichEditCommandId.QuickPrint, typeof(QuickPrintCommand));
#endif
		}
	}
}
