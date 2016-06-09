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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Commands;
namespace DevExpress.Snap.Extensions.Native.ActionLists {
	public class FieldActionList<T> : ActionListBase where T : CalculatedFieldBase {
		readonly SnapFieldInfo fieldInfo;
		readonly ComponentImplementation component;
		readonly IFieldChanger fieldChanger;
		readonly IParsedInfoProvider parsedInfoProvider;
		protected FieldActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(fieldInfo, "fieldInfo");
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.fieldInfo = fieldInfo;
			this.component = new ComponentImplementation(this, serviceProvider);
			this.fieldChanger = (IFieldChanger)serviceProvider.GetService(typeof(IFieldChanger));
			this.parsedInfoProvider = (IParsedInfoProvider)serviceProvider.GetService(typeof(IParsedInfoProvider));
		}
		#region Properties
		public override ComponentImplementation Component { get { return component; } }
		SnapPieceTable PieceTable { get { return fieldInfo.PieceTable; } }
		Field Field { get { return fieldInfo.Field; } }
		protected SnapFieldInfo FieldInfo { get { return fieldInfo; } }
		protected T ParsedInfo {
			get { return (T)parsedInfoProvider.GetParsedInfo(); }
		}
		protected IFieldChanger FieldChanger { get { return fieldChanger; } }
		#endregion
		protected void AddPropertyItem(ActionItemCollection actionItems, string name, string reflectName) {
			AddPropertyItem(actionItems, name, reflectName, this);
		}
		protected void ApplyNewValue<U>(ChangeFieldAction<U> changeFieldAction, U newValue) {
			fieldChanger.ApplyNewValue(changeFieldAction, newValue);
		}
	}
}
