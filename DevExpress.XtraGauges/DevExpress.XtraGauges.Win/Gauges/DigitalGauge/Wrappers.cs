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
using DevExpress.XtraGauges.Win.Base;
using System.ComponentModel;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Drawing;
namespace DevExpress.XtraGauges.Win.Gauges.Digital {
	public class DigitalBackgroundLayerComponentWrapper : BasePropertyGridObjectWrapper {
		protected DigitalBackgroundLayerComponent Component {
			get { return WrappedObject as DigitalBackgroundLayerComponent; }
		}
		[Category("Geometry")]
		public PointF2D TopLeft { get { return Component.TopLeft; } set { Component.TopLeft = value; } }
		[Category("Geometry")]
		public PointF2D BottomRight { get { return Component.BottomRight; } set { Component.BottomRight = value; } }
		[Category("Appearance")]
		[DefaultValue(DigitalBackgroundShapeSetType.Default)]
		public DigitalBackgroundShapeSetType ShapeType { get { return Component.ShapeType; } set { Component.ShapeType = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		internal bool ShouldSerializeTopLeft() {
			return TopLeft != PointF2D.Empty;
		}
		internal bool ShouldSerializeBottomRight() {
			return BottomRight != new PointF2D(200, 100);
		}
		internal void ResetTopLeft() {
			TopLeft = PointF2D.Empty;
		}
		internal void ResetBottomRight() {
			BottomRight = new PointF2D(200, 100);
		}
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class DigitalEffectLayerComponentWrapper : BasePropertyGridObjectWrapper {
		protected DigitalEffectLayerComponent Component {
			get { return WrappedObject as DigitalEffectLayerComponent; }
		}
		[Category("Geometry")]
		public PointF2D TopLeft { get { return Component.TopLeft; } set { Component.TopLeft = value; } }
		[Category("Geometry")]
		public PointF2D BottomRight { get { return Component.BottomRight; } set { Component.BottomRight = value; } }
		[Category("Appearance")]
		[DefaultValue(DigitalEffectShapeType.Default)]
		public DigitalEffectShapeType ShapeType { get { return Component.ShapeType; } set { Component.ShapeType = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		internal bool ShouldSerializeTopLeft() {
			return TopLeft != PointF2D.Empty;
		}
		internal bool ShouldSerializeBottomRight() {
			return BottomRight != new PointF2D(200, 100);
		}
		internal void ResetTopLeft() {
			TopLeft = PointF2D.Empty;
		}
		internal void ResetBottomRight() {
			BottomRight = new PointF2D(200, 100);
		}
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
}
