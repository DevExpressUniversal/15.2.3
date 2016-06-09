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

using DevExpress.DataAccess;
namespace DevExpress.DashboardCommon {
	public class CalculatedFieldCollection : NamedItemCollection<CalculatedField> {
		public CalculatedField Add(string expression) {
			CalculatedField field = new CalculatedField(expression);
			Add(field);
			return field;
		}
		public CalculatedField Add(string expression, string name) {
		   return Add(null, expression, name);
		}
		public CalculatedField Add(string expression, string name, CalculatedFieldType dataType) {
			return Add(null, expression, name, dataType);
		}
		public CalculatedField Add(string dataMember, string expression, string name) {
			CalculatedField field = new CalculatedField(dataMember, expression, name);
			Add(field);
			return field;
		}
		public CalculatedField Add(string dataMember, string expression, string name, CalculatedFieldType dataType) {
			CalculatedField field = new CalculatedField(dataMember, expression, name);
			Add(field);
			return field;
		}
		protected override string GetName(CalculatedField item) {
			return item.Name;
		}
	}
}
