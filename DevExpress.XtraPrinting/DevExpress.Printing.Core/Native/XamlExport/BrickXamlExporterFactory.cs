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
using System.Diagnostics;
using DevExpress.XtraPrinting.NativeBricks;
namespace DevExpress.XtraPrinting.XamlExport {
	public static class BrickXamlExporterFactory {
		readonly static Dictionary<string, Type> exportersHashTable = new Dictionary<string, Type>();
		static BrickXamlExporterFactory() {
			RegisterDefaultExporters();
		}
		public static BrickXamlExporterBase CreateExporter(BrickBase brick) {
			return CreateExporter(brick.GetType());
		}
		public static BrickXamlExporterBase CreateExporter(Type brickType) {
			Type exporterType;
			if(exportersHashTable.TryGetValue(brickType.FullName, out exporterType)) {
				BrickXamlExporterBase exporter = Activator.CreateInstance(exporterType) as BrickXamlExporterBase;
				if(exporter != null)
					return exporter;
			}
			return brickType.BaseType == null ? null : CreateExporter(brickType.BaseType);
		}
		public static void RegisterExporter(Type brickType, Type exporterType) {
			RegisterExporter(brickType.FullName, exporterType);
		}
		public static void RegisterExporter(string brickTypeName, Type exporterType) {
			Debug.Assert(typeof(BrickXamlExporterBase).IsAssignableFrom(exporterType));
			exportersHashTable[brickTypeName] = exporterType;
		}
		static void RegisterDefaultExporters() {
			RegisterExporter<TextBrick, TextBrickXamlExporter>();
			RegisterExporter<BrickContainerBase, BrickContainerBaseXamlExporter>();
			RegisterExporter<BrickContainer, BrickContainerXamlExporter>();
			RegisterExporter<CompositeBrick, CompositeBrickXamlExporter>();
			RegisterExporter<PanelBrick, PanelBrickXamlExporter>();
			RegisterExporter<Page, PageXamlExporter>();
			RegisterExporter<LineBrick, LineBrickXamlExporter>();
			RegisterExporter<CheckBoxBrick, CheckBoxBrickXamlExporter>();
			RegisterExporter<PageInfoTextBrick, PageInfoTextBrickXamlExporter>();
			RegisterExporter<ImageBrick, ImageBrickXamlExporter>();
			RegisterExporter<VisualBrick, BrickImageXamlExporter>();
			RegisterExporter("DevExpress.XtraPrinting.RichTextBrickBase", typeof(RichTextBrickXamlExporter));
#if !SL
			RegisterExporter<SeparableBrick, PanelBrickXamlExporter>();
#endif
		}
		static void RegisterExporter<T, V>() {
			RegisterExporter(typeof(T), typeof(V));
		}
	}
}
