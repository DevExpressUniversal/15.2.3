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
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.Map;
using DevExpress.XtraMap.Drawing;
using System.IO;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Design {
	public class CoordPointCollectionEditor : CollectionEditor {
		public CoordPointCollectionEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type itemType) {
			MapItem item = Context.Instance as MapItem;
			if (item == null)
				return new GeoPoint();
			InnerMap map = DesignHelper.FindInnerMapByMapItem(item);
			if (map == null)
				return new GeoPoint();
			var cs = map.CoordinateSystem as CartesianMapCoordinateSystem;
			if (cs != null)
				return new CartesianPoint();
			else 
				return new GeoPoint();
		}
	}
	public abstract class MapEditorBase : UITypeEditor {
		#region static
		protected static DialogResult ShowDialog(Form form, IServiceProvider provider) {
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (edSvc != null)
				return edSvc.ShowDialog(form);
			form.StartPosition = FormStartPosition.CenterScreen;
			return form.ShowDialog();
		}
		#endregion
		object value;
		object instance;
		protected object Value { get { return value; } set { this.value = value; } }
		protected object Instance { get { return instance; } set { instance = value; } }
		protected abstract Form CreateForm(IServiceProvider provider);
		protected virtual void AfterShowDialog(Form form, DialogResult dialogResult) {
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			this.value = objValue;
			instance = context.Instance;
			if (provider != null) {
				Form form = CreateForm(provider);
				try {
					if (form != null) {
						DesignerTransaction transaction = null;
						IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
						if (designerHost != null && !designerHost.InTransaction)
							transaction = designerHost.CreateTransaction(CreateTransactionName());
						try {
							AfterShowDialog(form, ShowDialog(form, provider));
						} catch {
							if (transaction != null) {
								transaction.Cancel();
								transaction = null;
							}
						}
						if (transaction != null)
							transaction.Commit();
					}
				} finally {
					if (form != null)
						form.Dispose();
				}
			}
			return value;
		}
		string CreateTransactionName() {
			return string.Format("{0}_update", GetType().Name);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null)
				return UITypeEditorEditStyle.Modal;
			return base.GetEditStyle(context);
		}
	}
	public class LayerCollectionEditor : MapEditorBase {
		protected override Form CreateForm(IServiceProvider provider) {
			LayerCollection layers = Value as LayerCollection;
			if (layers == null)
				return null;
			LayerCollectionEditorForm form = new LayerCollectionEditorForm(provider);
			form.EditValue = layers;
			return form;
		}
	}
	public class MiniMapLayerCollectionEditor : MapEditorBase {
		protected override Form CreateForm(IServiceProvider provider) {
			MiniMapLayerCollection layers = Value as MiniMapLayerCollection;
			if(layers == null)
				return null;
			MiniMapLayerCollectionEditorForm form = new MiniMapLayerCollectionEditorForm(provider);
			form.EditValue = layers;
			return form;
		}
	}
	public class MapItemCollectionEditor : MapEditorBase {
		protected override Form CreateForm(IServiceProvider provider) {
			MapItemCollection mapItems = Value as MapItemCollection;
			if (mapItems == null)
				return null;
			MapItemCollectionEditorForm form = new MapItemCollectionEditorForm(provider);
			form.EditValue = mapItems;
			return form;
		}
	}
	public class PropertyMappingsCollectionEditor : MapEditorBase {
		protected override Form CreateForm(IServiceProvider provider) {
			MapItemPropertyMappingCollection mappings = Value as MapItemPropertyMappingCollection;
			if(mappings == null)
				return null;
			PropertyMappingsCollectionEditorForm form = new PropertyMappingsCollectionEditorForm(provider);
			form.EditValue = mappings;
			return form;
		}
	}
	public abstract class MapLegendItemCollectionEditor : MapEditorBase {
		protected override Form CreateForm(IServiceProvider provider) {
			MapLegendItemCollection legendItems = Value as MapLegendItemCollection;
			if(legendItems == null)
				return null;
			MapLegendItemsCollectionForm form = CreateLegendItemsCollectionForm(provider);
			form.EditValue = legendItems;
			return form;
		}
		protected abstract MapLegendItemsCollectionForm CreateLegendItemsCollectionForm(IServiceProvider provider);
	}
	public class MapColorLegendItemCollectionEditor : MapLegendItemCollectionEditor {
		protected override MapLegendItemsCollectionForm CreateLegendItemsCollectionForm(IServiceProvider provider) {
			return new MapColorLegendItemsCollectionForm(provider);
		}
	}
	public class MapSizeLegendItemCollectionEditor : MapLegendItemCollectionEditor {
		protected override MapLegendItemsCollectionForm CreateLegendItemsCollectionForm(IServiceProvider provider) {
			return new MapSizeLegendItemsCollectionForm(provider);
		}
	}
	public class LegendCollectionEditor : MapEditorBase {
		protected override Form CreateForm(IServiceProvider provider) {
			LegendCollection legends = Value as LegendCollection;
			if(legends == null)
				return null;
			LegendCollectionEditorForm form = new LegendCollectionEditorForm(provider);
			form.EditValue = legends;
			return form;
		}
	}
	public class MapOverlayCollectionEditor : MapEditorBase {
		protected override Form CreateForm(IServiceProvider provider) {
			MapOverlayCollection overlays = Value as MapOverlayCollection;
			if(overlays == null)
				return null;
			MapOverlayCollectionEditorForm form = new MapOverlayCollectionEditorForm(provider);
			form.EditValue = overlays;
			return form;
		}
	}
	public class MapOverlayItemCollectionEditor : MapEditorBase {
		protected override Form CreateForm(IServiceProvider provider) {
			MapOverlayItemCollection overlayItems = Value as MapOverlayItemCollection;
			if(overlayItems == null)
				return null;
			MapOverlayItemCollectionEditorForm form = new MapOverlayItemCollectionEditorForm(provider);
			form.EditValue = overlayItems;
			return form;
		}
	}
	public class LegendAlignmentEditor : UITypeEditor {
		LegendAlignmentControl legendAlignmentEdit;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			if (context != null && context.Instance != null && provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					if (legendAlignmentEdit == null)
						legendAlignmentEdit = new LegendAlignmentControl();
					legendAlignmentEdit.DropDown(val, edSvc);
					val = legendAlignmentEdit.Value;
				}
			}
			return val;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
	}
	public abstract class MapUrlEditor : UITypeEditor {
		protected abstract string FileDialogFilter { get; } 
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			return DesignHelper.SelectFileUri(objValue as Uri, FileDialogFilter);  
		}
	}
	public class ShapefileUrlEditor : MapUrlEditor {
		protected override string FileDialogFilter { get { return DesignSR.ShapefileFilter; } }
	}
	public class KmlFileUrlEditor : MapUrlEditor {
		protected override string FileDialogFilter { get { return DesignSR.KmlFileFilter; } }
	}
	public class SvgFileUrlEditor : MapUrlEditor {
		protected override string FileDialogFilter { get { return DesignSR.SvgFileFilter; } }
	}
	public class DirectoryUrlEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			using(FolderBrowserDialog folderDialog = new FolderBrowserDialog()) {
				return folderDialog.ShowDialog() == DialogResult.OK ? folderDialog.SelectedPath : string.Empty;
			}
		}
	}
}
