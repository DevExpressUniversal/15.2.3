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
using System.Collections.Generic;
namespace DevExpress.Xpf.Diagram {
	using System;
	using System.Collections;
	using System.Windows;
	using System.Windows.Media;
	using DevExpress.Diagram.Core;
	using DevExpress.Mvvm.DataAnnotations;
	static class DiagramGenerator {
		public static IEnumerable<string> Generate(MetaContext context) {
			yield return GenerateCore(context, CommonProperties.DiagramControl);
			yield return GenerateCore(context, CommonProperties.DiagramItem);
			yield return GenerateCore(context, CommonProperties.DiagramShape);
			yield return GenerateCore(context, CommonProperties.DiagramConnector);
			yield return GenerateCore(context, CommonProperties.DiagramContainer);
			yield return GenerateCore(context, CommonProperties.DiagramOrgChartBehavior);
		}
		static string GenerateCore(MetaContext context, PropertiesInfo properties) {
			var result = DXPropertiesGenerator.GenerateDX(properties);
			return context.WrapMembers(result);
		}
	}
}
#endif
