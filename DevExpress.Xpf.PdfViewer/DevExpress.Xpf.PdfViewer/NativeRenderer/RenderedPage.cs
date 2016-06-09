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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.PdfViewer.Internal {
	public abstract class RenderedPageBase : IDisposable {
		public double ZoomFactor { get; set; }
		public double Angle { get; set; }
		protected bool Equals(RenderedPageBase other) {
			return PageIndex == other.PageIndex;
		}
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
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
		public virtual bool Match(double zoomFactor, double angle) {
			return ZoomFactor.AreClose(zoomFactor) && Angle.AreClose(angle);
		}
	}
	public class NotRenderedStub : RenderedPage {
		public NotRenderedStub()
			: base(-1) {
		}
		public override bool Match(double zoomFactor, double angle) {
			return false;
		}
	}
	public class RenderedPagesCache : IEnumerable<RenderedPage> {
		readonly Dictionary<int, RenderedPage> renderedPages = new Dictionary<int, RenderedPage>();
		public void AddPage(RenderedPage page) {
			renderedPages[page.PageIndex] = page;
		}
		public void RemovePage(int pageIndex) {
			renderedPages.Remove(pageIndex);
		}
		public RenderedPage GetPage(int pageIndex) {
			return renderedPages.GetValueOrDefault(pageIndex);
		}
		public void Clear() {
			renderedPages.Clear();
		}
		public IEnumerator<RenderedPage> GetEnumerator() {
			return renderedPages.Values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
