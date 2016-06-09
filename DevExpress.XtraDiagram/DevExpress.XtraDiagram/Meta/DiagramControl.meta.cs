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

#if METASHARP
using MetaSharp;
using DevExpress.Diagram.Core.Native.Generation;
using MetaSharp.Native;
using System.Collections.Generic;
[assembly: MetaReference("DevExpress.Data." + MetaConstants.Vsuffix + ".dll", ReferenceRelativeLocation.TargetPath)]
[assembly: MetaReference("DevExpress.Diagram." + MetaConstants.Vsuffix + ".Core.dll", ReferenceRelativeLocation.TargetPath)]
[assembly: MetaReference("System.Drawing.dll", ReferenceRelativeLocation.Framework)]
[assembly: MetaReference("System.Windows.Forms.dll", ReferenceRelativeLocation.Framework)]
static class MetaConstants {
	public const string Vsuffix = "v15.2";
}
namespace DevExpress.XtraDiagram {
	using System;
	using System.Collections;
	using System.Windows;
	using DevExpress.Diagram.Core;
	using DevExpress.XtraDiagram;
	using System.Drawing;
	using Size = System.Drawing.SizeF;
	using Thickness = System.Windows.Forms.Padding;
	using System.ComponentModel;
	using DevExpress.Utils.Design;
	using DevExpress.Utils;
	using DevExpress.XtraEditors;
	using Point = DevExpress.Utils.PointFloat;
	using Brush = System.Drawing.Color;
	static class DiagramGenerator {
		public static IEnumerable<string> Generate(MetaContext context) {
			yield return GenerateCore(context, CommonProperties.DiagramControl);
			yield return GenerateCore(context, CommonProperties.DiagramItem);
			yield return GenerateCore(context, CommonProperties.DiagramShape, "using TextDecorationCollection = DevExpress.XtraDiagram.TextDecoration;");
			yield return GenerateCore(context, CommonProperties.DiagramConnector, "using ConnectorPointsCollection = DevExpress.XtraDiagram.Base.DiagramConnectorPointCollection;");
			yield return GenerateCore(context, CommonProperties.DiagramContainer);
			yield return GenerateCore(context, CommonProperties.DiagramOrgChartBehavior);
		}
		static string GenerateCore(MetaContext context, PropertiesInfo properties, params string[] usings) {
			var result = DXPropertiesGenerator.GenerateXtra(properties);
			return context.WrapMembers(usings.ConcatStringsWithNewLines() + Environment.NewLine + result);
		}
	}
}
#endif
