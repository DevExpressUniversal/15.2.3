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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraDiagram.ViewInfo {
	public abstract class GraphicsStateBase : IDisposable {
		GraphicsCache cache;
		Matrix matrix;
		public GraphicsStateBase(GraphicsCache cache) {
			this.cache = cache;
		}
		public virtual void Save() {
			this.matrix = Cache.Graphics.Transform;
		}
		public virtual void Restore() {
			if(Matrix != null) {
				Cache.Graphics.Transform = Matrix;
				DestroyMatrix();
				this.matrix = null;
			}
		}
		public Matrix Matrix { get { return matrix; } }
		protected void DestroyMatrix() {
			if(Matrix != null) Matrix.Dispose();
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				DestroyMatrix();
			}
			this.cache = null;
			this.matrix = null;
		}
		#endregion
		public GraphicsCache Cache { get { return cache; } }
	}
	public class GraphicsScaleState : GraphicsStateBase {
		GraphicsClipState clip;
		public GraphicsScaleState(GraphicsCache cache) : base(cache) {
			this.clip = null;
		}
		public override void Save() {
			base.Save();
			this.clip = Cache.ClipInfo.SaveClip();
		}
		public override void Restore() {
			base.Restore();
			if(Clip != null) {
				Cache.ClipInfo.RestoreClipRelease(Clip);
				this.clip = null;
			}
		}
		public GraphicsClipState Clip { get { return clip; } }
		#region IDiposable
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Clip != null) Clip.Dispose();
			}
			this.clip = null;
			base.Dispose(disposing);
		}
		#endregion
	}
	public class GraphicsRotationState : GraphicsStateBase {
		public GraphicsRotationState(GraphicsCache cache)
			: base(cache) {
		}
	}
}
