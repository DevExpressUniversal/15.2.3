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

using DevExpress.Mvvm.DataAnnotations;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.Metadata {
	public class XRChartScriptsMetadata : IMetadataProvider<XRChartScripts> {
		public void BuildMetadata(MetadataBuilder<XRChartScripts> builder) {
			builder.PropertyGridEditor(MetadataPropertyGridEditorTemplateKeys.ForUseTypeConverterToStringConversion);
			builder.Property(x => x.OnCustomDrawSeries).PropertyGridEditor(MetadataPropertyGridEditorTemplateKeys.Scripts);
			builder.Property(x => x.OnCustomDrawSeriesPoint).PropertyGridEditor(MetadataPropertyGridEditorTemplateKeys.Scripts);
			builder.Property(x => x.OnCustomDrawCrosshair).PropertyGridEditor(MetadataPropertyGridEditorTemplateKeys.Scripts);
			builder.Property(x => x.OnCustomDrawAxisLabel).PropertyGridEditor(MetadataPropertyGridEditorTemplateKeys.Scripts);
			builder.Property(x => x.OnCustomPaint).PropertyGridEditor(MetadataPropertyGridEditorTemplateKeys.Scripts);
			builder.Property(x => x.OnBoundDataChanged).PropertyGridEditor(MetadataPropertyGridEditorTemplateKeys.Scripts);
			builder.Property(x => x.OnPieSeriesPointExploded).PropertyGridEditor(MetadataPropertyGridEditorTemplateKeys.Scripts);
			builder.Property(x => x.OnAxisScaleChanged).PropertyGridEditor(MetadataPropertyGridEditorTemplateKeys.Scripts);
			builder.Property(x => x.OnAxisVisualRangeChanged).PropertyGridEditor(MetadataPropertyGridEditorTemplateKeys.Scripts);
			builder.Property(x => x.OnAxisWholeRangeChanged).PropertyGridEditor(MetadataPropertyGridEditorTemplateKeys.Scripts);
		}
	}
}
