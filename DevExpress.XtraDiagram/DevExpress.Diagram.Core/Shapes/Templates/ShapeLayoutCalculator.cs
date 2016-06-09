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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Diagram.Core.TypeConverters;
namespace DevExpress.Diagram.Core.Shapes.Native {
	class ShapeLayoutCalculator {
		readonly Size size;
		readonly double[] parameters;
		readonly double[] Rows;
		readonly double[] Columns;
		readonly ShapeContext context;
		public ShapeLayoutCalculator(UnitCollection rows, UnitCollection columns, Size size, double[] parameters) {
			this.size = size;
			this.parameters = parameters;
			this.context = new ShapeContext(size, parameters);
			Rows = ConvertUnits(rows.Units, size.Height);
			Columns = ConvertUnits(columns.Units, size.Width);
		}
		public object Evaluate(CriteriaOperator op, ICollection<ICustomFunctionOperator> functions) {
			return context.Evaluate(op, functions);
		}
		public Point GetPoint(CriteriaOperator xOperator, CriteriaOperator yOperator) {
			double xFactor = Evaluate(xOperator);
			double x = GetHorizontalOffset(xFactor);
			double yFactor = Evaluate(yOperator);
			double y = GetVerticalOffset(yFactor);
			return new Point(x, y);
		}
		#region Private methods
		double GetUnitValue(Unit unit) {
			return Evaluate(unit.Value);
		}
		double[] ConvertUnits(Unit[] units, double totalLength) {
			double[] lengths = new double[units.Length];
			double fixedLenght = 0;
			double starCount = 0;
			for(int index = 0; index < units.Length; index++) {
				Unit unit = units[index];
				if(unit.UnitType == UnitType.CriteriaOperator) {
					double length = GetUnitValue(unit);
					lengths[index] = length;
					fixedLenght += length;
				}
				else if(unit.UnitType == UnitType.Star) {
					starCount += GetUnitValue(unit);
				}
			}
			double lengthRest = totalLength - fixedLenght;
			for(int index = 0; index < units.Length; index++) {
				Unit unit = units[index];
				if(unit.UnitType == UnitType.Star) {
					lengths[index] = GetUnitValue(unit) / starCount * lengthRest;
				}
			}
			return lengths;
		}
		double GetRowOffset(int index) {
			double offset = 0;
			for(int i = 0; i < index; i++)
				offset += Rows[i];
			return offset;
		}
		double GetHorizontalOffset(double factor) {
			if(factor >= Columns.Length)
				return size.Width;
			int columnIndex = (int)Math.Floor(factor);
			return GetColumnOffset(columnIndex) + Columns[columnIndex] * (factor - columnIndex);
		}
		double GetColumnOffset(int index) {
			double offset = 0;
			for(int i = 0; i < index; i++)
				offset += Columns[i];
			return offset;
		}
		double GetVerticalOffset(double factor) {
			if(factor >= Rows.Length)
				return size.Height;
			int rowIndex = (int)Math.Floor(factor);
			return GetRowOffset(rowIndex) + Rows[rowIndex] * (factor - rowIndex);
		}
		double Evaluate(CriteriaOperator op) {
			return Convert.ToDouble(Evaluate(op, null));
		}
		#endregion
	}
	public enum UnitType {
		Star,
		CriteriaOperator
	}
	public class Unit {
		readonly CriteriaOperator valueCore;
		readonly UnitType unitTypeCore;
		public CriteriaOperator Value { get { return valueCore; } }
		public UnitType UnitType { get { return unitTypeCore; } }
		public Unit(UnitType unitType, CriteriaOperator value) {
			this.unitTypeCore = unitType;
			this.valueCore = value;
		}
		internal static Unit Parse(string source) {
			UnitType unitType;
			string numberString = source;
			if(source.EndsWith("*")) {
				unitType = UnitType.Star;
				numberString = source.Substring(0, source.Length - 1);
				if(string.IsNullOrEmpty(numberString))
					numberString = "1";
			}
			else {
				unitType = UnitType.CriteriaOperator;
			}
			CriteriaOperator value = CriteriaOperator.Parse(numberString);
			return new Unit(unitType, value);
		}
	}
	[TypeConverter(typeof(UnitCollectionConverter))]
	public class UnitCollection {
		internal static UnitCollection Default = new UnitCollection(new Unit(UnitType.Star, 1));
		readonly Unit[] units;
		public Unit[] Units { get { return units; } }
		public UnitCollection(params Unit[] units) {
			this.units = units;
		}
		internal static UnitCollection Parse(string source) {
			string[] tokens = source.Split(';');
			var units = tokens.Select(token => Unit.Parse(token)).ToArray();
			return new UnitCollection(units);
		}
	}
}
