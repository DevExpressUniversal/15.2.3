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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
namespace DevExpress.XtraPrinting.Native {
	public class MetafileCreator {
		public static Metafile CreateInstance(RectangleF frameRect, MetafileFrameUnit frameUnit) {
			using(Graphics gr = GraphicsHelper.CreateGraphics()) {
				IntPtr hdc = gr.GetHdc();
				Metafile instance = new Metafile(hdc, frameRect, frameUnit);
				gr.ReleaseHdc(hdc);
				return instance;
			}
		}
		public static Metafile CreateInstance(Stream stream, int width, int height, MetafileFrameUnit frameUnit) {
			using(Graphics gr = GraphicsHelper.CreateGraphics()) {
				IntPtr hdc = gr.GetHdc();
				Metafile instance = new Metafile(stream, hdc, new Rectangle(0, 0, width, height), frameUnit);
				gr.ReleaseHdc(hdc);
				return instance;
			}
		}
		public static Metafile CreateInstance(Stream stream, int width, int height, MetafileFrameUnit frameUnit, EmfType type) {
			using(Graphics gr = GraphicsHelper.CreateGraphics()) {
				IntPtr hdc = gr.GetHdc();
				Metafile instance = new Metafile(stream, hdc, new Rectangle(0, 0, width, height), frameUnit, type);
				gr.ReleaseHdc(hdc);
				return instance;
			}
		}
		public static Metafile CreateInstance(Stream stream, EmfType type) {
			using(Graphics gr = GraphicsHelper.CreateGraphics()) {
				IntPtr hdc = gr.GetHdc();
				Metafile instance = new Metafile(stream, hdc, type);
				gr.ReleaseHdc(hdc);
				return instance;
			}
		}
		public static Metafile CreateInstance(Stream stream) {
			using(Graphics gr = GraphicsHelper.CreateGraphics()) {
				IntPtr hdc = gr.GetHdc();
				Metafile instance = new Metafile(stream, hdc);
				gr.ReleaseHdc(hdc);
				return instance;
			}
		}
	}
}
