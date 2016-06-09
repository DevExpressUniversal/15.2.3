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
using System.Linq;
using System.Text;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Printing.PreviewControl.Rendering {
	public abstract class RenderedPageBase : IDisposable {
		protected bool Equals(RenderedPageBase other) {
			return PageIndex == other.PageIndex;
		}
		public override bool Equals(object obj) {
			if(ReferenceEquals(null, obj))
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != this.GetType())
				return false;
			return Equals((RenderedPageBase)obj);
		}
		public override int GetHashCode() {
			return PageIndex;
		}
		public void Dispose() {
			DisposeInternal();
		}
		protected virtual void DisposeInternal() {
		}
		public int PageIndex { get; private set; }
		protected RenderedPageBase(int pageIndex) {
			PageIndex = pageIndex;
		}
		public virtual int GetAllocatedSize() {
			return 0;
		}
	}
	public class RenderedPage : RenderedPageBase {
		public Bitmap RenderedContent { get; set; }
		public RenderedPage(int pageIndex)
			: base(pageIndex) {
		}
		protected override void DisposeInternal() {
			base.DisposeInternal();
			RenderedContent.Do(x => x.Dispose());
			RenderedContent = null;
		}
		public override int GetAllocatedSize() {
			return RenderedContent.Return(x => x.Width * x.Height * 32 / 8, () => 0);
		}
	}
}
