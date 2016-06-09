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
using Platform::DevExpress.XtraRichEdit;
using Platform::DevExpress.Xpf.RichEdit;
using Platform::DevExpress.Xpf.RichEdit.Controls.Internal;
using Platform::DevExpress.XtraRichEdit.Forms;
using Platform::DevExpress.XtraRichEdit.Menu;
using Platform::DevExpress.Xpf.RichEdit.UI;
using Platform::DevExpress.Xpf.Bars;
#if SILVERLIGHT
using Platform::DevExpress.Xpf.RichEdit.Extensions;
using Platform::DevExpress.Xpf.RichEdit.Extensions.Forms;
#endif
[assembly: ProvideMetadata(typeof(DevExpress.Xpf.RichEdit.Design.MetadataProvider))]
namespace DevExpress.Xpf.RichEdit.Design {
	class MetadataProvider : MetadataProviderBase {
		protected override System.Reflection.Assembly RuntimeAssembly { get { return typeof(Platform::DevExpress.Xpf.RichEdit.RichEditControl).Assembly; } }
		protected override string ToolboxCategoryPath {
			get {
#if SILVERLIGHT
				return Platform::AssemblyInfo.DXTabNameRichEdit;
#else
				return Platform::AssemblyInfo.DXTabWpfRichEdit;
#endif
			}
		}
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			base.PrepareAttributeTable(builder);
			RegisterFeatures(builder);
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new RichEditPropertyLinesProvider(), false);
#if SILVERLIGHT
			HideControls(builder);
#endif
#if SILVERLIGHT && !DEBUG
#endif
		}
#if SILVERLIGHT
		void HideControls(AttributeTableBuilder builder) {
			builder.HideControls(
				typeof(ClipboardControl),
				typeof(DocumentStatisticsControl)
				);
		}
#endif
		void RegisterFeatures(AttributeTableBuilder builder) {
			builder.AddAttributes(typeof(RichEditControl), new FeatureAttribute(typeof(RichEditContextMenuProvider)));
		}
	}
}
