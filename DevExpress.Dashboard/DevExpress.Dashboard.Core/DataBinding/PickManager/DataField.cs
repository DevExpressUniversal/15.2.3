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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace DevExpress.DashboardCommon.Native {
	public enum DataFieldType {
		Text,
		DateTime,
		Bool,
		Integer,
		Float,
		Double,
		Decimal,
		Enum,
		Custom,
		Unknown
	}
	public enum DataFieldFilterEditorType {
		Text,
		ComboBox,
		DateTime
	}
	public class DataField : DataMemberNode {		
		readonly DataFieldType fieldType;
		readonly bool isConvertible;
		readonly bool isComparable;
		readonly DataFieldFilterEditorType editorType;
		readonly IList editorValues;
		readonly Type type;
		public DataFieldType FieldType { get { return fieldType; } }
		public Type SourceType { get { return type; } }
		public DataFieldFilterEditorType EditorType { get { return editorType; } }
		public IList EditorValues { get { return editorValues; } }
		public bool IsConvertible { get { return isConvertible; } }
		public bool IsComparable { get { return isComparable; } }
		public override DataFieldType ActualFieldType { get { return FieldType; } }
		public override bool IsDataFieldNode { get { return true; } }
		public override DataNodeType NodeType { get { return DataNodeType.DataField; } }
		public string Caption { get { return DisplayName; } }
		public bool ContainsCaption { get { return Caption != DataMember; } }
		public DataField(PickManager pickManager, string dataMember, string displayName, Type type)
			: base(pickManager, dataMember, displayName, false) {
			this.type = type;
			fieldType = DataBindingHelper.GetDataFieldType(type);
			editorType = DataBindingHelper.GetEditorType(type);
			editorValues = DataBindingHelper.GetEditorValues(type);
			isConvertible = typeof(IConvertible).IsAssignableFrom(type);		   
			isComparable = typeof(IComparable).IsAssignableFrom(type);
		}
		protected override void FillDataFields(List<DataField> dataFields) {
			dataFields.Add(this);
		}
	}
}
