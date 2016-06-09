#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using System;
namespace DevExpress.DashboardWin.Native {
	public abstract class ImageButton : ResourceColorImage {
		DragAreaButtonState buttonState = DragAreaButtonState.Normal;
		protected internal virtual string Tooltip { get { return null; } }
		protected ImageButton(string imageName) : base(imageName) {
		}
		public void SetOptionsButtonState(DragAreaButtonState state) {
			buttonState = state;
		}
		public override void Paint(Color color, GraphicsCache cache) {
			Bitmap bitmap = new Bitmap(Glyph);
			float opacity = GetStateColor(buttonState);
			bitmap = ImageHelper.ColorBitmap(bitmap, color, opacity);
			cache.Graphics.DrawImage(bitmap, Bounds.Location);
		}
		protected virtual float GetStateColor(DragAreaButtonState state) {
			switch(state) {
				case DragAreaButtonState.Hot:
					return 0.75f;
				case DragAreaButtonState.Normal:
					return 0.35f;
				case DragAreaButtonState.Selected:
					return 0.25f;
				default:
					return 0.35f;
			}
		}
	}
	public class ResourceColorImage : IDisposable {
		protected Image Glyph { get; set; }
		public Rectangle Bounds { get; protected set; }
		public int Width { get { return (Glyph == null ? 0 : Glyph.Width); } }
		public int Height { get { return (Glyph == null ? 0 : Glyph.Height); } }
		public ResourceColorImage(string imageName) {
			Glyph = ImageHelper.GetImage(imageName);
		}
		public virtual void Paint(Color color, GraphicsCache cache) {
			Bitmap bitmap = new Bitmap(Glyph);
			bitmap = ImageHelper.ColorBitmap(bitmap, color);
			cache.Graphics.DrawImage(bitmap, Bounds.Location);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) 
				Glyph.Dispose();
		}
	}
}
