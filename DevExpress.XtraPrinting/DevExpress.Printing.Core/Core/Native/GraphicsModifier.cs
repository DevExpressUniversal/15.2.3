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
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraPrinting.Native {
	public abstract class GraphicsModifier {
		public abstract void Dispose();
		public abstract void OnGraphicsDispose();
		public abstract void SetPageUnit(Graphics gr, GraphicsUnit value);
		public abstract void DrawString(Graphics gr, string s, Font font, Brush brush, RectangleF bounds, StringFormat format);
		public abstract void DrawImage(Graphics gr, Image image, RectangleF bounds);
		public abstract void DrawImage(Graphics gr, Image image, Point position);
		public abstract void ScaleTransform(Graphics gr, float sx, float sy, MatrixOrder order);
	}
	public class GdiPlusGraphicsModifier : GraphicsModifier {
		public override void Dispose() {
		}
		public override void SetPageUnit(Graphics gr, GraphicsUnit value) {
			gr.PageUnit = value;
		}
		public override void ScaleTransform(Graphics gr, float sx, float sy, MatrixOrder order) {
			gr.ScaleTransform(sx, sy, order);
		}
		public override void DrawString(Graphics gr, string s, Font font, Brush brush, RectangleF bounds, StringFormat format) {
			lock(font) {
				lock(brush) {
					lock(ValidateObject(format)) {
						try {
							gr.DrawString(s, font, brush, bounds, format);
						} catch { }
					}
				}
			}
		}
		public override void DrawImage(Graphics gr, Image image, RectangleF bounds) {
			gr.DrawImage(image, bounds);
		}
		public override void DrawImage(Graphics gr, Image image, Point position) {
			gr.DrawImage(image, position);
		}
		object ValidateObject(object obj) {
			return obj != null ? obj : new Object();
		}
		public override void OnGraphicsDispose() { 
		}
	}
}
