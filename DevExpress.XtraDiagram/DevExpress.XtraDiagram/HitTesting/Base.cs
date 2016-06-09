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
using System.Linq;
using System.Text;
using DevExpress.XtraDiagram.Base;
namespace DevExpress.XtraDiagram.ViewInfo {
	public enum DiagramControlHitTest {
		Item, ShapeSizeGrip, RotationGrip, ShapeParameter, Page, None, Client, Border, EditSurface, ConnectorBeginPoint, ConnectorEndPoint, ConnectorIntermediatePoint
	}
	public class DiagramControlHitInfo {
		Point hitPoint;
		object hitObject;
		DiagramControlHitTest hitTest;
		public DiagramControlHitInfo() : this(Point.Empty) { }
		public DiagramControlHitInfo(Point hitPoint) {
			this.hitPoint = hitPoint;
			this.hitObject = null;
			this.hitTest = DiagramControlHitTest.None;
		}
		protected internal DiagramControlHitInfo SetHitTest(DiagramControlHitTest hitTest) {
			this.hitTest = hitTest;
			return this;
		}
		protected internal DiagramControlHitInfo SetHitObject(object hitObject) {
			this.hitObject = hitObject;
			return this;
		}
		public void Reset() {
			this.hitPoint = Point.Empty;
			this.hitObject = null;
		}
		public static readonly DiagramControlHitInfo Empty = new DiagramControlHitInfo();
		public bool IsEmpty {
			get { return Equals(Empty); }
		}
		public override bool Equals(object obj) {
			DiagramControlHitInfo other = obj as DiagramControlHitInfo;
			if(other == null) return false;
			return other.HitTest == HitTest && other.hitObject == hitObject;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public Point HitPoint { get { return hitPoint; } }
		public DiagramControlHitTest HitTest { get { return hitTest; } }
		public DiagramItemInfo ItemInfo { get { return hitObject as DiagramItemInfo; } }
		public DiagramContainerInfo ContainerItemInfo {
			get { return hitObject as DiagramContainerInfo; }
		}
		public DiagramShapeInfo ShapeInfo {
			get { return hitObject as DiagramShapeInfo; }
		}
		public DiagramSizeGripItem SizeGripItem {
			get { return hitObject as DiagramSizeGripItem; }
		}
		public DiagramShapeParameterItem ShapeParameterItem {
			get { return hitObject as DiagramShapeParameterItem; }
		}
		public DiagramConnectorPointItem ConnectorPointItem {
			get { return hitObject as DiagramConnectorPointItem; }
		}
		public bool InItem {
			get { return HitTest == DiagramControlHitTest.Item; }
		}
		public bool InPage {
			get { return HitTest == DiagramControlHitTest.Page; }
		}
		public bool InRotationGrip {
			get { return HitTest == DiagramControlHitTest.RotationGrip; }
		}
		public bool InShapeSizeGrip {
			get { return HitTest == DiagramControlHitTest.ShapeSizeGrip; }
		}
		public bool InShapeParameter {
			get { return HitTest == DiagramControlHitTest.ShapeParameter; }
		}
		public bool InClient{
			get { return HitTest == DiagramControlHitTest.Client; }
		}
		public bool InBorder {
			get { return HitTest == DiagramControlHitTest.Border; }
		}
		public bool InEditSurface {
			get { return HitTest == DiagramControlHitTest.EditSurface; }
		}
		public bool InConnectorBeginPoint {
			get { return HitTest == DiagramControlHitTest.ConnectorBeginPoint; }
		}
		public bool InConnectorEndPoint {
			get { return HitTest == DiagramControlHitTest.ConnectorEndPoint; }
		}
		public bool InConnectorIntermediatePoint {
			get { return HitTest == DiagramControlHitTest.ConnectorIntermediatePoint; }
		}
	}
}
