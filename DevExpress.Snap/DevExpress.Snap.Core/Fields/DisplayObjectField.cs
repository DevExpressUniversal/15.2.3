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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.Snap.Core.Native;
using System;
namespace DevExpress.Snap.Core.Fields {
	public class DisplayObjectField : TemplatedFieldBase {
		public static readonly string FieldType = "DISPLAYOBJECT";
		public static CalculatedFieldBase Create() {
			return new DisplayObjectField();
		}
		protected override string FieldTypeName { get { return FieldType; } }
		protected override ResultItemInfoCollection GetResultItems(SnapPieceTable sourcePieceTable, IFieldDataAccessService fieldDataAccessService, FieldDataValueDescriptor fieldDataValueDescriptor, bool modelForExport, Field documentField) {			
			TemplateController templateController = new TemplateController(sourcePieceTable);
			TemplateInfo itemTemplateInfo = templateController.GetTemplateInfo(TemplateInterval, SnapTemplateIntervalType.DataRow);
			ResultItemInfoCollection resultItems = new ResultItemInfoCollection();
			ICalculationContext calculationContext = fieldDataAccessService.FieldContextService.BeginCalculation(sourcePieceTable.DocumentModel.DataSourceDispatcher);
			try {			  
				IFieldContext fieldContext = calculationContext.GetRawValueFieldContext(fieldDataValueDescriptor.ParentDataContext, fieldDataValueDescriptor.RelativePath);
				if(fieldContext != null)
					resultItems.AddTemplateIfExists(itemTemplateInfo, fieldContext);
			}
			finally {
				fieldDataAccessService.FieldContextService.EndCalculation(calculationContext);				
			}
			return resultItems;
		}
		public override List<TemplateFieldInterval> GetTemplateIntervals(PieceTable pieceTable) {
			List<TemplateFieldInterval> result = new List<TemplateFieldInterval>();
			AddTemplateIntervalIfExists(result, TemplateSwitch);
			return result;
		}
	}
}
