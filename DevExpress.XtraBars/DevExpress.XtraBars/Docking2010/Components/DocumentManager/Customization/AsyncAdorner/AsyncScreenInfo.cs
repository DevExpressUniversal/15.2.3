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
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.Utils.Animation;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public abstract class AsyncAdornerElementInfoArgs : BaseAsyncAdornerElementInfoArgs {
		BaseView ownerCore;
		protected AsyncAdornerElementInfoArgs(BaseView owner) {
			ownerCore = owner;
		}
		public BaseView Owner {
			get { return ownerCore; }
		}
	}
	public sealed class AsyncAdornerOpaquePainter : AdornerOpaquePainter {
		public sealed override void DrawObject(ObjectInfoArgs e) {
			IAsyncAdornerElementInfoArgs ea = e as IAsyncAdornerElementInfoArgs;
			if(ea == null) return;
			ObjectPainter.DrawObject(e.Cache, ea.GetPainter(), ea.InfoArgs);
		}
	}
	public sealed class AsyncAdornerElementInfo : BaseAsyncAdornerElementInfo {
		public AsyncAdornerElementInfo(AdornerPainter painter, AdornerOpaquePainter opaquePainter, AsyncAdornerElementInfoArgs info) : base(opaquePainter, info){
			painterCore = painter;		 
		}
		public AsyncAdornerElementInfo(AdornerOpaquePainter opaquePainter, AsyncAdornerElementInfoArgs info)
			: this(null, opaquePainter, info) {
		}
		public AsyncAdornerElementInfo(AdornerPainter painter, AsyncAdornerElementInfoArgs info)
			: this(painter, null, info) {
		}
		AdornerPainter painterCore;
		public AdornerPainter Painter {
			get { return painterCore; }
		}
	}
}
