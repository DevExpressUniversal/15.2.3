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
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Model;
using System.Drawing;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraGauges.Win.Gauges.State {
	public class StateIndicatorComponentWrapper : BasePropertyGridObjectWrapper {
		protected StateIndicatorComponent Component {
			get { return WrappedObject as StateIndicatorComponent; }
		}
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		[Category("State")]
		public IndicatorStateCollection States { get { return Component.States; } }
		[Category("State")]
		public int StateIndex { get { return Component.StateIndex; } set { Component.StateIndex = value; } }
		[Category("Geometry")]
		public PointF2D Center { get { return Component.Center; } set { Component.Center = value; } }
		[Category("Geometry")]
		public SizeF Size { get { return Component.Size; } set { Component.Size = value; } }
		internal bool ShouldSerializeSize() {
			return Size != new SizeF(200, 200);
		}
		internal bool ShouldSerializeCenter() {
			return Center != new PointF2D(125, 125);
		}
		internal void ResetCenter() {
			Center = new PointF2D(125, 125);
		}
		internal void ResetSize() {
			Size = new SizeF(200, 200);
		}
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
}
