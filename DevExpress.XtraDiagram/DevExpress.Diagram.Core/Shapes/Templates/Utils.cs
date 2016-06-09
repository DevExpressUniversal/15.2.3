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

using DevExpress.Data.Filtering;
using System;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.Diagram.Core.Shapes.Native {
	static class ImageSourceHelper {
		[ThreadStatic]
		static bool isLoaded = false;
		public static void RegisterPackScheme() {
			if(!isLoaded) {
				new System.Windows.Documents.FlowDocument();
				isLoaded = true;
			}
		}
	}
	class CriteriaOperatorContextBase {
		readonly double width;
		readonly double height;
		public double W { get { return width; } }
		public double H { get { return height; } }
		public CriteriaOperatorContextBase(Size size) {
			this.width = size.Width;
			this.height = size.Height;
		}
		internal object Evaluate(CriteriaOperator op, ICollection<ICustomFunctionOperator> functions) {
			var properties = TypeDescriptor.GetProperties(this);
			ExpressionEvaluator evaluator = new ExpressionEvaluator(properties, op, functions);
			return evaluator.Evaluate(this);
		}
	}
	class ValueGetterContext : CriteriaOperatorContextBase {
		readonly Point point;
		public Point P { get { return point; } }
		public ValueGetterContext(Size size, Point p)
			: base(size) {
			this.point = p;
		}
	}
	class PointGetterContext : CriteriaOperatorContextBase {
		readonly double parameter;
		public double P { get { return parameter; } }
		public PointGetterContext(Size size, double p)
			: base(size) {
			this.parameter = p;
		}
	}
	partial class ShapeContext : CriteriaOperatorContextBase {
		readonly double[] parameters;
		public double[] P { get { return parameters; } }
		public ShapeContext(Size size, double[] parameters) : base(size) {
			this.parameters = parameters;
		}
	}
	class CreatePointFunction : ICustomFunctionOperator {
		public CreatePointFunction() {
		}
		#region ICustomFunctionOperator Members
		public object Evaluate(params object[] operands) {
			return new Point(Convert.ToDouble(operands[0]), Convert.ToDouble(operands[1]));
		}
		public string Name {
			get { return "CreatePoint"; }
		}
		public Type ResultType(params Type[] operands) {
			return typeof(Point);
		}
		#endregion
	}
	class CreateRectFunction : ICustomFunctionOperator {
		public CreateRectFunction() {
		}
		#region ICustomFunctionOperator Members
		public object Evaluate(params object[] operands) {
			return new Rect(Convert.ToDouble(operands[0]), Convert.ToDouble(operands[1]), Convert.ToDouble(operands[2]), Convert.ToDouble(operands[3]));
		}
		public string Name {
			get { return "CreateRect"; }
		}
		public Type ResultType(params Type[] operands) {
			return typeof(Rect);
		}
		#endregion
	}
	class CreateSizeFunction : ICustomFunctionOperator {
		public CreateSizeFunction() {
		}
		#region ICustomFunctionOperator Members
		public object Evaluate(params object[] operands) {
			return new Size(Convert.ToDouble(operands[0]), Convert.ToDouble(operands[1]));
		}
		public string Name {
			get { return "CreateSize"; }
		}
		public Type ResultType(params Type[] operands) {
			return typeof(Size);
		}
		#endregion
	}
}
