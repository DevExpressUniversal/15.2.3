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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Localization;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.Shape.Native;
using DevExpress.XtraPrinting.Shape;
using System.Collections.Generic;
using DevExpress.XtraReports.Design.MouseTargets;
namespace DevExpress.XtraReports.Design {
	[MouseTarget(typeof(ShapeMouseTarget))]
	public class XRShapeDesigner : XRControlDesigner {
		#region static
		internal static int AngleBetweenPointsInDeg(Point startPoint, Point endPoint) {
			if(startPoint == Point.Empty || endPoint == Point.Empty)
				return 0;
			double cos = (startPoint.X * endPoint.X + startPoint.Y * endPoint.Y) / (CalcPointRadius(startPoint) * CalcPointRadius(endPoint));
			double angle = Math.Acos(cos);
			int direction = -startPoint.X * endPoint.Y + startPoint.Y * endPoint.X;
			if(direction < 0)
				angle = 2 * Math.PI - angle;
			return (int)Math.Round(ShapeHelper.RadToDeg((float)angle));
		}
		static double CalcPointRadius(Point p) {
			return Math.Sqrt(p.X * p.X + p.Y * p.Y);
		}
		#endregion
		bool inRotation;
		public bool InRotation {
			get { return inRotation; }
			set { inRotation = value; }
		}
		public override DesignerActionListCollection ActionLists {
			get { return CreateActionLists(); }
		}
		protected internal override bool IsRotatable { get { return true; } }
		protected XRShape XRShape { get { return Component as XRShape; } }
		public XRShapeDesigner()
			: base() {
		}
		public void ApplyAngle(int previousValue) {
			if(XRShape.Angle != previousValue) {
				int value = XRShape.Angle;
				XRShape.Angle = previousValue;
				DesignerTransaction trans = DesignerHost.CreateTransaction(String.Format(DesignSR.TransFmt_Angle, XRShape.Site.Name));
				try {
					XRControlDesignerBase.RaiseComponentChanging(changeService, XRShape, "Angle");
					XRShape.Angle = value;
					XRControlDesignerBase.RaiseComponentChanged(changeService, XRShape);
					trans.Commit();
				} catch {
					trans.Cancel();
				}
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRShapeDesignerActionList1(this));
			list.Add(new XRShapeDesignerActionList2(this));
			list.Add(new XRControlBookmarkDesignerActionList(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
			list.Add(ShapeActionListFactory.CreateShapeActionList(XRShape.Shape, this));
		}
		protected override SelectionTypes GetSelectionTypes() {
			return InRotation ? SelectionTypes.Add : base.GetSelectionTypes();
		}
		protected override string[] GetFilteredProperties() {
			return new string[] { XRComponentPropertyNames.Text };
		}
	}
	public static class ShapeActionListFactory {
		static Dictionary<Type, Type> shapeActionListTypes = new Dictionary<Type, Type>();
		static ShapeActionListFactory() {
			shapeActionListTypes[typeof(DevExpress.XtraPrinting.Shape.FilletShapeBase)] = typeof(FilletShapeBaseActionList);
			shapeActionListTypes[typeof(DevExpress.XtraPrinting.Shape.ShapeArrow)] = typeof(ShapeArrowActionList);
			shapeActionListTypes[typeof(DevExpress.XtraPrinting.Shape.ShapeBrace)] = typeof(ShapeBraceActionList);
			shapeActionListTypes[typeof(DevExpress.XtraPrinting.Shape.ShapeBracket)] = typeof(ShapeBracketActionList);
			shapeActionListTypes[typeof(DevExpress.XtraPrinting.Shape.ShapeCross)] = typeof(ShapeCrossActionList);
			shapeActionListTypes[typeof(DevExpress.XtraPrinting.Shape.ShapePolygon)] = typeof(ShapePolygonActionList);
			shapeActionListTypes[typeof(DevExpress.XtraPrinting.Shape.ShapeStar)] = typeof(ShapeStarActionList);
		}
		public static ShapeBaseActionList CreateShapeActionList(ShapeBase shape, XRShapeDesigner designer) {
			Type shapeType = shape.GetType();
			while(shapeType.BaseType != null) {
				if(shapeActionListTypes.ContainsKey(shapeType))
					return (ShapeBaseActionList)Activator.CreateInstance(shapeActionListTypes[shapeType], new object[] { designer });
				shapeType = shapeType.BaseType;
			}
			return new ShapeBaseActionList(designer);
		}
	}
}
