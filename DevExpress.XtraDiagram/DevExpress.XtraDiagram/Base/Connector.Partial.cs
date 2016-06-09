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
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Utils;
using System.Drawing;
using DevExpress.Utils;
using PlatformSize = System.Windows.Size;
using PlatformPoint = System.Windows.Point;
namespace DevExpress.XtraDiagram {
	public partial class DiagramConnector : IDiagramConnector {
		PlatformPoint IDiagramConnector.BeginPoint {
			get { return BeginPoint.ToPlatformPoint(); }
			set { BeginPoint = value.ToPointFloat(); }
		}
		protected virtual bool ShouldSerializeBeginPoint() { return BeginPoint != PointFloat.Empty; }
		protected virtual void ResetBeginPoint() { BeginPoint = PointFloat.Empty; }
		PlatformSize IDiagramConnector.BeginArrowSize {
			get { return BeginArrowSize.ToPlatformSize(); }
			set { BeginArrowSize = value.ToWinSize(); }
		}
		protected virtual bool ShouldSerializeBeginArrowSize() {
			return BeginArrowSize != DefaultArrowSize;
		}
		protected virtual void ResetBeginArrowSize() { BeginArrowSize = DefaultArrowSize; }
		protected virtual bool ShouldSerializeBeginArrow() {
			return BeginArrow != null;
		}
		protected virtual void ResetBeginArrow() { BeginArrow = null; }
		PlatformPoint IDiagramConnector.EndPoint {
			get { return EndPoint.ToPlatformPoint(); }
			set { EndPoint = value.ToPointFloat(); }
		}
		protected virtual bool ShouldSerializeEndPoint() { return EndPoint != PointFloat.Empty; }
		protected virtual void ResetEndPoint() { EndPoint = PointFloat.Empty; }
		PlatformSize IDiagramConnector.EndArrowSize {
			get { return EndArrowSize.ToPlatformSize(); }
			set { EndArrowSize = value.ToWinSize(); }
		}
		protected virtual bool ShouldSerializeEndArrowSize() {
			return EndArrowSize != DefaultArrowSize;
		}
		protected virtual void ResetEndArrowSize() { EndArrowSize = DefaultArrowSize; }
		protected virtual bool ShouldSerializeEndArrow() {
			return EndArrow != null;
		}
		protected virtual void ResetEndArrow() { EndArrow = null; }
		protected virtual bool ShouldSerializeType() {
			return Type != ConnectorType.RightAngle;
		}
		protected virtual void ResetType() { Type = ConnectorType.RightAngle; }
		ConnectorPointsCollection IDiagramConnector.Points {
			get { return IntermediatePoints.ToPlatformCollection(); }
			set {
				IntermediatePoints.BeginUpdate();
				try {
					IntermediatePoints.Clear();
					if(value != null) IntermediatePoints.AddRange(CollectionUtils.Create(value));
				}
				finally {
					IntermediatePoints.EndUpdate();
				}
			}
		}
		protected virtual bool ShouldSerializeText() { return !string.IsNullOrEmpty(Text); }
		protected virtual void ResetText() { Text = string.Empty; }
		void IDiagramConnector.InvalidateAppearance() { }
		public void UpdateRoute() {
			this.Controller().UpdateRoute();
		}
	}
}
