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
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native.Sql;
namespace DevExpress.DataAccess.Native {
	public class ActionTypeData {
		readonly ActionType actionType;
		readonly string text;
		public ActionType ActionType { get { return actionType; } }
		public ActionTypeData(ActionType actionType) {
			this.actionType = actionType;
			switch (actionType) {
				case ActionType.InnerJoin:
				text = DataAccessLocalizer.GetString(DataAccessStringId.RelationEditorRelationTypeInnerJoin);
				break;
				case ActionType.LeftOuterJoin:
				text = DataAccessLocalizer.GetString(DataAccessStringId.RelationEditorRelationTypeLeftOuterJoin);
				break;
				case ActionType.MasterDetailRelation:
				text = DataAccessLocalizer.GetString(DataAccessStringId.RelationEditorRelationTypeMasterDetail);
				break;
				default:
				Type enumType = actionType.GetType();
				throw new NotSupportedException(string.Format("'{0}.{1}' value isn't supported", enumType.Name, Enum.GetName(enumType, actionType)));
			}
		}
		public override string ToString() {
			return text;
		}
		public override bool Equals(object obj) {
			ActionTypeData item = obj as ActionTypeData;
			return item != null && item.actionType == actionType;
		}
		public override int GetHashCode() {
			return actionType.GetHashCode();
		}
	}
}
