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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export;
using DevExpress.Data;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.Printing;
using System.Text;
using System.IO;
namespace DevExpress.XtraPrinting.Native {
	public class HtmlExportContext : ExportContext {
		readonly IScriptContainer scriptContainer;
		readonly IImageRepository imageRepository;
		readonly HtmlExportMode exportMode;
		public HtmlExportContext(PrintingSystemBase ps, IScriptContainer scriptContainer, IImageRepository imageRepository)
			: this(ps, scriptContainer, imageRepository, HtmlExportMode.SingleFile) {
		}
		public HtmlExportContext(PrintingSystemBase ps, IScriptContainer scriptContainer, IImageRepository imageRepository, HtmlExportMode exportMode)
			: base(ps) {
			Guard.ArgumentNotNull(scriptContainer, "scriptContainer");
			Guard.ArgumentNotNull(imageRepository, "imageRepository");
			this.scriptContainer = scriptContainer;
			this.imageRepository = imageRepository;
			this.exportMode = exportMode;
		}
		public bool CrossReferenceAvailable {
			get {
				return MainExportMode == HtmlExportMode.SingleFilePageByPage || (MainExportMode == HtmlExportMode.SingleFile && !IsPageExport);
			}
		}
		public HtmlExportMode MainExportMode { get { return exportMode; } }
		public IScriptContainer ScriptContainer { get { return scriptContainer; } }
		public IImageRepository ImageRepository { get { return imageRepository; } }
		public virtual bool CopyStyleWhenClipping { get { return false; } }
		protected bool InMhtContext { get { return imageRepository is MhtImageRepository; } }
		public virtual bool ShouldBlockBookmarks {
			get { return false; }
		}
		public virtual bool IsPageExport {
			get { return exportMode == HtmlExportMode.DifferentFiles || exportMode == HtmlExportMode.SingleFilePageByPage; }
		}
		public override BrickViewData[] GetData(Brick brick, RectangleF rect, RectangleF clipRect) {
			BrickExporter exporter = BrickBaseExporter.GetExporter(this, brick) as BrickExporter;
			return exporter.GetHtmlData(this, rect, clipRect);
		}
		HtmlCellImageContentCreator htmlCellImageContentCreator;
		public HtmlCellImageContentCreator HtmlCellImageContentCreator {
			get {
				if(htmlCellImageContentCreator == null) {
					htmlCellImageContentCreator = imageRepository is MhtImageRepository ? CreateMhtCellImageContentCreator() :
						imageRepository is CssImageRepository ? CreateCssHtmlCellImageContentCreator() :
						CreateHtmlCellImageContentCreator();
				}
				return htmlCellImageContentCreator;
			}
		}
		protected virtual HtmlCellImageContentCreator CreateCssHtmlCellImageContentCreator() {
			return new CssHtmlCellImageContentCreator((CssImageRepository)ImageRepository);
		}
		protected virtual HtmlCellImageContentCreator CreateHtmlCellImageContentCreator() {
			return new HtmlCellImageContentCreator(ImageRepository);
		}
		protected virtual HtmlCellImageContentCreator CreateMhtCellImageContentCreator() {
			return new MhtCellImageContentCreator(ImageRepository, ScriptContainer);
		}
		protected internal virtual void RegisterNavigationScript() {
			const string navigationScript = "NavigationScript";
			if(!ScriptContainer.IsClientScriptBlockRegistered(navigationScript))
				using(StreamReader reader = new StreamReader(DevExpress.Printing.ResFinder.GetManifestResourceStream("Core.HtmlExport.navigation.js"), Encoding.ASCII))
					ScriptContainer.RegisterClientScriptBlock(navigationScript, reader.ReadToEnd());
		}
	}
}
