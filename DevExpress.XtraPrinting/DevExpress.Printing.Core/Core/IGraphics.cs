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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
#if SL
using DevExpress.Xpf.Drawing;
using System.Windows.Controls;
using DevExpress.Xpf.Drawing.Drawing2D;
using DevExpress.Xpf.Windows.Forms;
#else
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endif
namespace DevExpress.XtraPrinting {
	public interface IPrintingSystemContext : IServiceProvider {
		Page DrawingPage { get; }
		PrintingSystemBase PrintingSystem { get; }
		Measurer Measurer { get; }
		bool CanPublish(Brick brick);
	}
	public interface IGraphics : IGraphicsBase, IPrintingSystemContext, IDisposable {
		float Dpi { get; }
		int GetPageCount(int basePageNumber, DefaultBoolean continuousPageNumbering);
		void SetDrawingPage(Page page);
		void ResetDrawingPage();
	}
}
namespace DevExpress.XtraPrinting.Native {
	public interface IBrickPublisher {
		bool CanPublish(Brick brick, IPrintingSystemContext context);
	}
	public class DefaultBrickPublisher : IBrickPublisher {
		bool IBrickPublisher.CanPublish(Brick brick, IPrintingSystemContext context) {
			return brick.IsVisible;
		}
	}
	class ExportBrickPublisher : IBrickPublisher {
		bool IBrickPublisher.CanPublish(Brick brick, IPrintingSystemContext context) {
			return brick.IsVisible && brick.CanPublish;
		}
	}
}
