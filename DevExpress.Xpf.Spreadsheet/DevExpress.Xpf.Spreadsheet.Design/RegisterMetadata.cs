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

extern alias Platform;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using Platform::System.Windows;
using Platform::System.Windows.Data;
using Platform::System.Windows.Controls;
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.XtraSpreadsheet;
using Platform::DevExpress.Xpf.Spreadsheet;
using Platform::DevExpress.XtraSpreadsheet.Forms;
using Platform::DevExpress.XtraSpreadsheet.Menu;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Design;
#if SILVERLIGHT
using Platform::DevExpress.Xpf.Spreadsheet.Extensions;
using Platform::DevExpress.Xpf.Spreadsheet.Extensions.Forms;
#endif
[assembly: ProvideMetadata(typeof(DevExpress.Xpf.Spreadsheet.Design.MetadataProvider))]
namespace DevExpress.Xpf.Spreadsheet.Design {
#if !SL
	class SpreadsheetControlInitializer : DefaultInitializer {
		readonly double defaultWidth = 600d;
		readonly double defaultheight = 300d;
		public override void InitializeDefaults(ModelItem item) {
			item.Properties["Width"].SetValue(defaultWidth);
			item.Properties["Height"].SetValue(defaultheight);
			InitializerHelper.Initialize(item);
		}
	}
#endif
	class MetadataProvider : MetadataProviderBase {
		protected override System.Reflection.Assembly RuntimeAssembly { get { return typeof(Platform::DevExpress.Xpf.Spreadsheet.SpreadsheetControl).Assembly; } }
		protected override string ToolboxCategoryPath {
			get {
#if SILVERLIGHT
				return Platform::AssemblyInfo.DXTabNameSpreadsheet;
#else
				return Platform::AssemblyInfo.DXTabWpfSpreadsheet;
#endif
			}
		}
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			base.PrepareAttributeTable(builder);
			RegisterFeatures(builder);
#if SILVERLIGHT
			HideControls(builder);
#endif
#if SILVERLIGHT && !DEBUG
#endif
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new SpreadsheetPropertyLinesProvider(typeof(SpreadsheetControl)));
#if !SL
			builder.AddCustomAttributes(typeof(SpreadsheetControl), new FeatureAttribute(typeof(SpreadsheetControlInitializer)));
#endif
		}
#if SILVERLIGHT
		void HideControls(AttributeTableBuilder builder) {
			builder.HideControls(
				);
		}
#endif
		void RegisterFeatures(AttributeTableBuilder builder) {
			builder.AddAttributes(typeof(SpreadsheetControl), new FeatureAttribute(typeof(SpreadsheetContextMenuProvider)));
		}
	}
}
