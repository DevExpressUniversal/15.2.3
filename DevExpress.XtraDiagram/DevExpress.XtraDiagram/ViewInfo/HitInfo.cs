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
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Base;
namespace DevExpress.XtraDiagram.ViewInfo {
	public class DiagramSizeGripItem {
		readonly SizeGripKind gripKind;
		public DiagramSizeGripItem(SizeGripKind gripKind) {
			this.gripKind = gripKind;
		}
		public SizeGripKind GripKind { get { return gripKind; } }
	}
	public class DiagramShapeParameterItem {
		readonly DiagramShape shape;
		readonly ParameterDescription parameter;
		public DiagramShapeParameterItem(DiagramShape shape, ParameterDescription parameter) {
			this.shape = shape;
			this.parameter = parameter;
		}
		public DiagramShape Shape { get { return shape; } }
		public ParameterDescription Parameter { get { return parameter; } }
	}
	public class DiagramEditSurfaceItem {
		readonly DiagramItem item;
		public DiagramEditSurfaceItem(DiagramItem item) {
			this.item = item;
		}
		public DiagramItem Item { get { return item; } }
	}
	public class DiagramConnectorPointItem {
		readonly DiagramConnector connector;
		readonly int pointIndex;
		public DiagramConnectorPointItem(DiagramConnector connector)
			: this(connector, -1) {
		}
		public DiagramConnectorPointItem(DiagramConnector connector, int pointIndex) {
			this.connector = connector;
			this.pointIndex = pointIndex;
		}
		public DiagramConnector Connector { get { return connector; } }
		public int PointIndex { get { return pointIndex; } }
	}
}
