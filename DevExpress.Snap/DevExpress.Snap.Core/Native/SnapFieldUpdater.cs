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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Native {
	public class SnapFieldUpdater : FieldUpdater {
		public SnapFieldUpdater(PieceTable pieceTable) : base(pieceTable) { }
		public override List<Field> GetInnerCodeFields(IList<Field> fields, Field parentField) {
			List<Field> result = base.GetInnerCodeFields(fields, parentField);
			if(parentField == null || result.Count == 0)
				return result;
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			TemplatedFieldBase parsedInfo = calculator.ParseField(PieceTable, parentField) as TemplatedFieldBase;
			if(parsedInfo == null)
				return result;
			List<TemplateFieldInterval> templateIntervals = parsedInfo.GetTemplateIntervals(PieceTable);
			List<RunInfo> templateRunInfos = GetTemplateRunInfos(templateIntervals);
			for(int i = result.Count - 1; i >= 0; i--) {
				if(IsFieldInsideTemplate(result[i], templateRunInfos))
					result.RemoveAt(i);
			}
			return result;
		}
		List<RunInfo> GetTemplateRunInfos(List<TemplateFieldInterval> templateIntervals) {
			List<RunInfo> result = new List<RunInfo>(templateIntervals.Count);
			int count = templateIntervals.Count;
			for(int i = 0; i < count; i++) {
				DocumentLogInterval interval = templateIntervals[i].Interval;
				RunInfo runInfo = new RunInfo(PieceTable);
				PieceTable.CalculateRunInfoStart(interval.Start, runInfo);
				PieceTable.CalculateRunInfoEnd(interval.Start + interval.Length, runInfo);
				result.Add(runInfo);
			}
			return result;
		}
		bool IsFieldInsideTemplate(Field field, List<RunInfo> templateRunInfos) {
			int count = templateRunInfos.Count;
			for(int i = 0; i < count; i++) {
				RunInfo runInfo = templateRunInfos[i];
				if(field.FirstRunIndex >= runInfo.Start.RunIndex && field.LastRunIndex <= runInfo.End.RunIndex)
					return true;
			}
			return false;
		}
	}
}
