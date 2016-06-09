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
#if SL
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
using Brush = DevExpress.Xpf.Drawing.Brush;
#endif
namespace DevExpress.XtraPrinting.Native {
	public abstract class GraphicsBase : IPrintingSystemContext {
		PrintingSystemBase ps;
		Page drawingPage;
		Measurer measurer;
		IBrickPublisher brickPublisher;
		protected IBrickPublisher BrickPublisher {
			get {
				if(brickPublisher == null) {
					brickPublisher = GetService(typeof(IBrickPublisher)) as IBrickPublisher;
					if(brickPublisher == null)
						brickPublisher = new DefaultBrickPublisher();
				}
				return brickPublisher;
			}
		}
		public Measurer Measurer {
			get {
				if(measurer == null)
					measurer = GetService(typeof(Measurer)) as Measurer;
				return measurer;
			}
		}
		public ProgressReflector ProgressReflector {
			get { return ps.ProgressReflector; }
		}
		public PrintingSystemBase PrintingSystem { get { return ps; } }
		public Page DrawingPage { get { return drawingPage; } }
		protected GraphicsBase(PrintingSystemBase ps) {
			if(ps == null)
				throw new ArgumentNullException("ps");
			this.ps = ps;
		}
		public Brush GetBrush(Color color) {
			return PrintingSystem.Gdi.GetBrush(color);
		}
		public virtual bool CanPublish(Brick brick) {
			return BrickPublisher.CanPublish(brick, this);
		}
		public virtual int GetPageCount(int basePageNumber, DefaultBoolean continuousPageNumbering) {
			return PageInfoTextBrick.GetPageCountFromPS(PrintingSystem, basePageNumber, continuousPageNumbering, drawingPage);
		}
		public void SetDrawingPage(Page page) {
			drawingPage = page;
			page.PerformLayout(this);
		}
		public void ResetDrawingPage() {
			drawingPage = null;
		}
		protected internal static void EnsureStringFormat(Font font, RectangleF bounds, GraphicsUnit unit, StringFormat format, Action<StringFormat> action) {
			if(format != null && (format.FormatFlags & StringFormatFlags.LineLimit) > 0 && (format.FormatFlags & StringFormatFlags.NoWrap) == 0) {
				float height = font.GetHeight(GraphicsDpi.UnitToDpi(unit));
				if(FloatsComparer.Default.FirstLessSecond(bounds.Height, height)) {
					format.FormatFlags &= ~StringFormatFlags.LineLimit;
					action(format);
					format.FormatFlags |= StringFormatFlags.LineLimit;
					return;
				}
			}
			action(format);
		}
		#region IServiceProvider Members
		public object GetService(Type serviceType) {
			return ((IServiceProvider)ps).GetService(serviceType);
		}
		#endregion
	}
}
