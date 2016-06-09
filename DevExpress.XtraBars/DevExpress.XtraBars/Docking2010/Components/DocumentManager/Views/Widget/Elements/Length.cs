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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public enum LengthUnitType { Pixel, Star }
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class Length {
		LengthUnitType unitTypeCore;
		double unitValueCore;
		public Length() {
			UnitValue = 1;
			UnitType = LengthUnitType.Star;
		}
		public Length(double pixels) {
			UnitValue = pixels;
			UnitType = LengthUnitType.Pixel;
		}
		public Length(double unitValue, LengthUnitType widgetUnitType) {
			UnitValue = unitValue;
			UnitType = widgetUnitType;
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged!= null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		[DefaultValue(1.0), DevExpress.Utils.Serializing.XtraSerializableProperty()]
		public double UnitValue {
			get { return unitValueCore; }
			set { 
				if(unitValueCore == value) return;
				unitValueCore = value;
				RaisePropertyChanged("UnitValue");
			}
		}
		[DefaultValue(LengthUnitType.Star), DevExpress.Utils.Serializing.XtraSerializableProperty()]
		public LengthUnitType UnitType {
			get { return unitTypeCore; }
			set {
				if(unitTypeCore == value) return;
				unitTypeCore = value;
				RaisePropertyChanged("UnitType");
			}
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
	}
	public class StackGroupLengthHelper {
		public static void CalcActualGroupLength(int length, IEnumerable<BaseRelativeLengthElement> collection) {
			int remainingLength = length;
			double sectionCount = 0;
			List<BaseRelativeLengthElement> groupsWithRelativeLength = new List<BaseRelativeLengthElement>();
			List<BaseRelativeLengthElement> groupsWithFixedLength = new List<BaseRelativeLengthElement>();
			foreach(BaseRelativeLengthElement group in collection) {
				if(group.Length.UnitType == LengthUnitType.Pixel) {
					group.SetAcutalLength((int)group.Length.UnitValue);
					remainingLength -= group.ActualLength;
					groupsWithFixedLength.Add(group);
				}
				else {
					groupsWithRelativeLength.Add(group);
					sectionCount += group.Length.UnitValue;
				}
			}
			CalcGroupVisibility(remainingLength, sectionCount, groupsWithRelativeLength, groupsWithFixedLength);
		}
		static void CalcGroupVisibility(int remainingLength, double sectionCount, List<BaseRelativeLengthElement> groupsWithRelativeLength, List<BaseRelativeLengthElement> groupsWithFixedLength) {
			List<BaseRelativeLengthElement> visibleGroups = new List<BaseRelativeLengthElement>();
			groupsWithRelativeLength.Sort(PositiveCompare);
			foreach(BaseRelativeLengthElement group in groupsWithRelativeLength) {
				int newActualLength = (int)(remainingLength * (group.Length.UnitValue / sectionCount));
				if(!CalcGroupVisibility(group as StackGroup, newActualLength)) {
					group.SetGroupVisibility(false);
					group.SetAcutalLength(0);
					sectionCount -= group.Length.UnitValue;
				}
				else {
					visibleGroups.Add(group);
				}
			}
			foreach(BaseRelativeLengthElement group in visibleGroups) {
				group.SetGroupVisibility(true);
				group.SetAcutalLength((int)(remainingLength * (group.Length.UnitValue / sectionCount)));
			}
		}
		static int PositiveCompare(BaseRelativeLengthElement x, BaseRelativeLengthElement y) {
			if(x == y) return 0;
			if(x == null || y == null)
				return 0;
			return x.Length.UnitValue.CompareTo(y.Length.UnitValue);
		}
		static bool CalcGroupVisibility(StackGroup group, int newActualLength) {
			if(group == null) return true;
			foreach(Document document in group.Items) {
				int minLength = group.IsHorizontal ? document.Info.MinSize.Height : document.Info.MinSize.Width;
				minLength = group.IsHorizontal ? document.Info.MinSize.Width : minLength;
				if(newActualLength < minLength)
					return false;
			}
			return newActualLength > 0;
		}
	}
}
