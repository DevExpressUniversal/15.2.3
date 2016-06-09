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

using System.ComponentModel;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public class RowDefinition : BaseRelativeLengthElement {
		public RowDefinition()
			: base(null) {
		}
	}
	public class ColumnDefinition : BaseRelativeLengthElement {
		public ColumnDefinition()
			: base(null) {
		}
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
	RefreshProperties(RefreshProperties.All), ListBindable(false)]
	public class RowDefinitionCollection : BaseMutableListEx<RowDefinition> {
		public bool Insert(int index, RowDefinition group) {
			return InsertCore(index, group);
		}
		public override string ToString() {
			if(Count == 0) return "None";
			return string.Format("Count {0}", Count);
		}
		public void AddRow(double unitValue = 1, LengthUnitType unitType = LengthUnitType.Star) {
			RowDefinition definition = new RowDefinition() { Length = new Length(unitValue, unitType) };
			Add(definition);
		}
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
	RefreshProperties(RefreshProperties.All), ListBindable(false)]
	public class ColumnDefinitionCollection : BaseMutableListEx<ColumnDefinition> {
		public bool Insert(int index, ColumnDefinition group) {
			return InsertCore(index, group);
		}
		public override string ToString() {
			if(Count == 0) return "None";
			return string.Format("Count {0}", Count);
		}
		public void AddColumn(double unitValue = 1, LengthUnitType unitType = LengthUnitType.Star) {
			ColumnDefinition definition = new ColumnDefinition() { Length = new Length(unitValue, unitType) };
			Add(definition);
		}
	}
}
