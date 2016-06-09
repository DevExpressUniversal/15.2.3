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
using System.Windows;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
#if !SILVERLIGHT
using DevExpress.XtraPrinting.Drawing;
using DevExpress.Utils.Serializing;
using System.IO;
using DevExpress.ReportServer.ServiceModel.Native;
#else
using DevExpress.XtraPrinting.Drawing;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class WatermarkService : IWatermarkService {
		WatermarkEditor watermarkEditor;
		WatermarkEditorViewModel wmeViewModel;
		public event EventHandler<WatermarkServiceEventArgs> EditCompleted;
		public void Edit(Window ownerWindow, XtraPageSettingsBase pageSettings, int pagesCount, Watermark currentWatermark) {
			Guard.ArgumentNotNull(pageSettings, "pageSettings");
			Guard.ArgumentPositive(pagesCount, "pagesCount");
			Guard.ArgumentNotNull(currentWatermark, "currentWatermark");
			XpfWatermark watermarkCopy = CopyWatermark(currentWatermark);
			Page pageStub = CreatePageStub(pageSettings);
			EditInternal(ownerWindow, pageStub, pagesCount, watermarkCopy);
		}
#if !SILVERLIGHT
		public void Edit(Window ownerWindow, Page page, int pagesCount, Watermark currentWatermark) {
			Guard.ArgumentNotNull(page, "page");
			Guard.ArgumentPositive(pagesCount, "pagesCount");
			Guard.ArgumentNotNull(currentWatermark, "currentWatermark");
			XpfWatermark watermarkCopy = CopyWatermark(currentWatermark);
			Page pageCopy = CopyPage(page);
			EditInternal(ownerWindow, pageCopy, pagesCount, watermarkCopy);
		}
#endif
		void EditInternal(Window ownerWindow, Page page, int pagesCount, XpfWatermark watermark) {
			wmeViewModel = new WatermarkEditorViewModel();
			wmeViewModel.PageCount = pagesCount;
			wmeViewModel.Page = page;
			wmeViewModel.Watermark = watermark;
			watermarkEditor = new WatermarkEditor();
			watermarkEditor.Model = wmeViewModel;
#if !SILVERLIGHT
			watermarkEditor.Owner = ownerWindow;
			if(ownerWindow != null) {
				watermarkEditor.FlowDirection = ownerWindow.FlowDirection;
			}
#endif
			watermarkEditor.Closed += watermarkEditor_Closed;
			watermarkEditor.ShowDialog();
		}
		void watermarkEditor_Closed(object sender, EventArgs e) {
			watermarkEditor.Closed -= watermarkEditor_Closed;
			WatermarkEditor editor = (WatermarkEditor)sender;
			if(EditCompleted != null) {
				EditCompleted(this, new WatermarkServiceEventArgs(wmeViewModel.Watermark, editor.DialogResult));
			}
		}
		XpfWatermark CopyWatermark(Watermark source) {
			XpfWatermark watermark = new XpfWatermark();
			watermark.CopyFrom(source);
			return watermark;
		}
		Page CreatePageStub(XtraPageSettingsBase pageSettings) {
			PrintingSystem psTemp = new PrintingSystem();
			PSPage page = new PSPage(pageSettings.Data);
			psTemp.Document.Pages.Add(page);
			return page;
		}
#if !SILVERLIGHT
		Page CopyPage(Page source) {
			PrintingSystemXmlSerializer serializer = new PrintingSystemXmlSerializer();
			using(MemoryStream stream = new MemoryStream()) {
				source.Document.SerializeCore(stream, serializer, ContinuousExportInfo.Empty, new Page[] { source });
				stream.Position = 0;
				DeserializedPrintingSystem ps = new DeserializedPrintingSystem();
				ps.Document.Deserialize(stream, new PrintingSystemXmlSerializer());
				Page pageCopy = ps.Document.Pages[0];
				return pageCopy;
			}
		}
#endif
	}
}
