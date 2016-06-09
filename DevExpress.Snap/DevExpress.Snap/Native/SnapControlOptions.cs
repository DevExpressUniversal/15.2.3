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
using System.ComponentModel;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Snap.Core.Native.Options;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Options;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Options;
namespace DevExpress.Snap {
	public class SnapControlOptions : RichEditControlOptions, ISnapControlOptions {
		static readonly DataSourceWizardOptions defaultDataSourceWizardOptions = new DataSourceWizardOptions();
		readonly DataSourceWizardOptions dataSourceWizardOptions = new DataSourceWizardOptions();
		public SnapControlOptions(InnerRichEditDocumentServer documentServer)
			: base(documentServer) {
		}
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlOptionsFields")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public new SnapFieldOptions Fields { get { return (SnapFieldOptions)base.Fields; } }
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlOptionsDocumentSaveOptions")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public new SnxDocumentSaveOptions DocumentSaveOptions { get { return (SnxDocumentSaveOptions)base.DocumentSaveOptions; } }
		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new RichEditMailMergeOptions MailMerge { get { return base.MailMerge; } }
		[
#if !SL
	DevExpressSnapLocalizedDescription("SnapControlOptionsSnapMailMergeVisualOptions"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter("DevExpress.Snap.Core.Native.Options.NativeSnapMailMergeOptionsTypeConverter," + AssemblyInfo.SRAssemblySnapCore),
		NotifyParentProperty(true)]
		public SnapMailMergeVisualOptions SnapMailMergeVisualOptions {
			get {
				if (DocumentServer == null)
					return null;
				return ((SnapDocumentModel)DocumentServer.DocumentModel).SnapMailMergeVisualOptions;
			}
		}
		[
#if !SL
	DevExpressSnapLocalizedDescription("SnapControlOptionsFileExportOptions"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)]
		public DocumentSaveOptions FileExportOptions {
			get {
				if (DocumentServer == null)
					return null;
				return ((SnapDocumentModel)DocumentServer.DocumentModel).FileExportOptions;
			}
		}
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlOptionsDataSourceWizardOptions")]
#endif
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataSourceWizardOptions DataSourceWizardOptions { get { return dataSourceWizardOptions; } }
		bool ShouldSerializeDataSourceWizardOptions() {
			return !DataSourceWizardOptions.Equals(defaultDataSourceWizardOptions);
		}
		protected override void SubscribeInnerOptionsEvents() {
			base.SubscribeInnerOptionsEvents();
			RichEditNotificationOptions mailMergeOptions = SnapMailMergeVisualOptions as RichEditNotificationOptions;
			if (mailMergeOptions != null)
				mailMergeOptions.Changed += OnInnerOptionsChanged;
		}
		protected override void UnsubscribeInnerOptionsEvents() {
			base.UnsubscribeInnerOptionsEvents();
			RichEditNotificationOptions mailMergeOptions = SnapMailMergeVisualOptions as RichEditNotificationOptions;
			if (mailMergeOptions != null)
				mailMergeOptions.Changed -= OnInnerOptionsChanged;
		}
	}
}
