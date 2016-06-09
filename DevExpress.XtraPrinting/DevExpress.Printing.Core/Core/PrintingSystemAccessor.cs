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
using System.Text;
using DevExpress.XtraPrinting.Native;
using System.IO;
namespace DevExpress.XtraPrinting.InternalAccess {
	public static class BrickAccessor {
		public static Brick GetRealBrick(Brick brick) {
			return brick.GetRealBrick();
		}
	}
	public static class DocumentAccessor {
		public static void SetInfoString(PSDocument doc, string value) {
			doc.InfoString = value;
		}
		public static void LoadPage(Document document, int pageIndex) {
			document.LoadPage(pageIndex);
		}
	}
	public static class PrintingSystemAccessor {
		public static void SaveIndependentPages(PrintingSystemBase ps, Stream stream) {
			ps.SaveIndependentPages(stream);
		}
		public static void ForceLoadDocument(PrintingSystemBase printingSystem) {
			printingSystem.Document.ForceLoad();
		}
		public static void LoadVirtualDocument(PrintingSystemBase ps, Stream stream) {
			ps.LoadVirtualDocument(stream);
		}
		public static void LoadVirtualDocument(PrintingSystemBase ps, string filePath) {
			ps.LoadVirtualDocument(filePath);
		}
		public static void LoadVirtualDocument(PrintingSystemBase ps, Stream stream, int pageIndex) {
			ps.LoadVirtualDocument(stream);
			ps.Document.LoadPage(pageIndex);
		}
		public static void LoadVirtualDocument(PrintingSystemBase ps, string filePath, int pageIndex) {
			ps.LoadVirtualDocument(filePath);
			ps.Document.LoadPage(pageIndex);
		}
	}
}
#if DEBUGTEST
namespace DevExpress.XtraPrinting.Tests {
	using DevExpress.XtraPrinting.InternalAccess;
	using NUnit.Framework;
	public class PrintingSystemBaseHelper {
		protected virtual PrintingSystemBase CreatePrintingSystemCore() {
			PrintingSystemBase printingSystem = new PrintingSystemBase();
			printingSystem.PageSettings.AssignDefaultPageSettings();
			return printingSystem;
		}
		public static PrintingSystemBase CreatePrintingSystem() {
			return new PrintingSystemBaseHelper().CreatePrintingSystemCore();
		}
	}
}
#endif
