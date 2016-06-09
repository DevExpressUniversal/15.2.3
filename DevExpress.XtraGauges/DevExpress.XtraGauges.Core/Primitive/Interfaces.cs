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
using System.Drawing.Drawing2D;
using System.IO;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraGauges.Core.Primitive {
	public interface IPrimitive : IBaseObject {
	}
	public interface ISupportTransform : IPrimitive {
		Matrix Transform { get;set;}
		void ResetTransform();
		void BeginTransform();
		void EndTransform();
		void CancelTransform();
		bool IsTransformLocked { get; }
	}
	public interface ISupportGeometry : ISupportTransform {
		PointF Location { get; set;}
		float Angle { get; set;}
		FactorF2D ScaleFactor { get; set;}
	}
	public interface ISupportViewInfo : IPrimitive {
		IViewInfo ViewInfo { get;}
		void SetViewInfoDirty();
		void CalcViewInfo(Matrix local);
		bool HitTestEnabled { get;set; }
	}
	public interface ISupportInteraction : IPrimitive {
		BasePrimitiveHitInfo CalcHitInfo(Point pt);
		void ProcessMouseDown(MouseEventArgsInfo ea);
		void ProcessMouseUp(MouseEventArgsInfo ea);
		void ProcessMouseMove(MouseEventArgsInfo ea);
	}
	public interface IRenderable : ISupportGeometry, ISupportViewInfo, ISupportInteraction, ISupportCaching {
		void Render(Graphics graphics);
		void Render(Stream stream);
		void Render(IRenderingContext context);
		void WaitForPendingDelayedCalculation();
		bool Renderable { get;set; }
		int ZOrder { get;set; }
	}
	public interface IRenderableElement : IRenderable, IElement<IRenderableElement> {
		BaseShapeCollection Shapes { get;}
		BaseColorShader Shader { get;set;}
		bool Enabled { get;set;}
	}
	public interface IRenderingContext : IDisposable {
		Stream Stream { get;}
		Graphics Graphics { get;}
		Matrix Transform { get; set; }
	}
	public interface IViewInfo : IDisposable {
		bool IsReady { get;}
		IRenderableElement Owner { get;}
		PrimitiveState State { get;}
		PointF[] Points { get;}
		RectangleF BoundBox { get;}
		RectangleF RelativeBoundBox { get;}
		bool HitTest(Point pt);
	}
}
