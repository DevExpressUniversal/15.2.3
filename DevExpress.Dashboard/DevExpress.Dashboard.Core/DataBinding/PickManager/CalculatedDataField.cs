#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
namespace DevExpress.DashboardCommon.Native {
	public class CalculatedDataField : DataField {
		static Type GetActualType(CalculatedFieldType type) {
			switch(type) {
				case CalculatedFieldType.Boolean:
					 return typeof(bool);
				case CalculatedFieldType.DateTime:
					return typeof(DateTime);
				case CalculatedFieldType.Decimal:
					return typeof(Decimal);
				case CalculatedFieldType.Integer:
					return typeof(int);
				case CalculatedFieldType.String:
					return typeof(string);
				default:
					return typeof(object);
			}
		}
		CalculatedField field;
		public override DataNodeType NodeType { get { return DataNodeType.CalculatedDataField; } }
		public override string DataMember { get { return field.Name; } }
		public override bool IsDataFieldNode {
			get { return true; }
		}
		public override string DisplayName {
			get { return field.Name; }
			set { }
		}
		public CalculatedFieldType CalculatedFieldType { get { return field.DataType; } }
		internal bool IsAggregateCalculatedField {
			get {
				if(DataSource == null || field == null)
					return false;
				return field.CheckHasAggregate(DataSource.CalculatedFields);
			}
		}
		public CalculatedDataField(PickManager pickManager, CalculatedField field) : base(pickManager, field.Name, field.Name, GetActualType(field.DataType)) {
			this.field = field;
		}
		internal bool GetIsCorrupted(IActualParametersProvider provider) {
			return DataSource != null && !DataSource.IsCalculatedFieldValid(field, provider);
		}
	}
}
