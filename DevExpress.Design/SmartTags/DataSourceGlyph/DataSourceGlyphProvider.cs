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
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.Design.DataAccess.UI {
	public interface IDataSourceGlyphProvider : IDisposable {
		bool ConnectedToDataSource { get; }
		Control DataAwareControl { get; set; }
		Image Glyph { get; }
		void DataSourceChanged();
		ContentAlignment Alignment { get; }
		bool ShowText { get; }
		bool AdaptableSize { get; }
		bool RunDataSourceWizard();
	}
	class DataSourceGlyphProvider : IDataSourceGlyphProvider {
		public void Dispose() {
			if(DataAwareControl != null)
				DataAwareControl.SizeChanged -= DataAwareControl_SizeChanged;
			if(dataSourceGlyph != null)
				dataSourceGlyph.Dispose();
			this.dataSourceGlyph = null;
			this.dataSourcePropertyDescriptorCore = null;
			this.glyphCore = null;
			this.controlCore = null;
		}
		public ContentAlignment Alignment {
			get;
			set;
		}
		public bool ShowText {
			get;
			set;
		}
		public bool AdaptableSize {
			get;
			set;
		}
		Control controlCore;
		public Control DataAwareControl {
			get { return controlCore; }
			set {
				if(controlCore == value) return;
				if(DataAwareControl != null)
					DataAwareControl.SizeChanged -= DataAwareControl_SizeChanged;
				controlCore = value;
				if(DataAwareControl != null)
					DataAwareControl.SizeChanged += DataAwareControl_SizeChanged;
				OnDataAwareControlChanged();
			}
		}
		void DataAwareControl_SizeChanged(object sender, EventArgs e) {
			if(dataSourceGlyph != null)
				dataSourceGlyph.UpdatePosition();
		}
		public bool ConnectedToDataSource {
			get {
				return (DataAwareControl != null) &&
					(DataSourcePropertyDescriptor != null) &&
					(DataSourcePropertyDescriptor.GetValue(DataAwareControl) != null);
			}
		}
		Image glyphCore;
		public Image Glyph {
			get {
				if(glyphCore == null) {
					if(DataAwareControl != null && DataSourcePropertyDescriptor != null) {
						object dataSource = DataSourcePropertyDescriptor.GetValue(DataAwareControl);
						if(dataSource != null)
							glyphCore = GetGlyph(dataSource.GetType());
					}
				}
				return glyphCore;
			}
		}
		public void DataSourceChanged() {
			if(dataSourceGlyph == null) return;
			dataSourceGlyph.UpdatePosition();
			dataSourceGlyph.Invalidate();
		}
		PropertyDescriptor dataSourcePropertyDescriptorCore;
		protected PropertyDescriptor DataSourcePropertyDescriptor {
			get {
				if(dataSourcePropertyDescriptorCore == null) {
					if(DataAwareControl != null) {
						Type dataAwareControlType = DataAwareControl.GetType();
						DataAccess.IDataAccessMetadata metadata = DataAccess.DataAccessMetadataLoader.Load(dataAwareControlType);
						if(metadata != null) {
							var propertyDescriptors = TypeDescriptor.GetProperties(dataAwareControlType);
							dataSourcePropertyDescriptorCore = propertyDescriptors[metadata.DataSourceProperty];
						}
					}
				}
				return dataSourcePropertyDescriptorCore;
			}
		}
		Image GetGlyph(Type dataSourceType) {
			Image result = null;
			if(!TryGetStandardDataFieldPickerImage(dataSourceType, out result)) {
				object[] attributes = dataSourceType.GetCustomAttributes(typeof(ToolboxBitmapAttribute), true);
				if(attributes.Length == 0) {
					try {
						result = DevExpress.Utils.ResourceImageHelper.CreateBitmapFromResources(
							dataSourceType.FullName + ".bmp", dataSourceType.Assembly);
					}
					catch { }
				}
				else {
					try {
						result = (attributes[0] as ToolboxBitmapAttribute).GetImage(DataAwareControl, false);
					}
					catch { }
				}
			}
			if(result != null)
				result = DevExpress.Utils.Controls.ImageHelper.MakeTransparent(result);
			return result;
		}
		bool TryGetStandardDataFieldPickerImage(Type dataSourceType, out Image result) {
			result = null;
			if(typeof(System.Data.DataSet).IsAssignableFrom(dataSourceType) || typeof(System.Windows.Forms.BindingSource).IsAssignableFrom(dataSourceType)) {
				object[] attributes = typeof(System.Data.DataSet).GetCustomAttributes(typeof(ToolboxItemAttribute), true);
				if(attributes.Length == 1) {
					try {
						var msVSDesignerAssembly = ((ToolboxItemAttribute)attributes[0]).ToolboxItemType.Assembly;
						var imgList = DevExpress.Utils.ResourceImageHelper.CreateImageListFromResources(
							"Microsoft.VSDesigner.DataFieldPicker.DataPickerImages.bmp", msVSDesignerAssembly, new Size(16, 16));
						if(typeof(System.Data.DataSet).IsAssignableFrom(dataSourceType))
							result = imgList.Images[1];
						if(msVSDesignerAssembly.GetName().Version.Major > 10) { 
							if(typeof(System.Windows.Forms.BindingSource).IsAssignableFrom(dataSourceType))
								result = imgList.Images[4];
						}
					}
					catch { result = null; }
				}
			}
			return result != null;
		}
		DataSourceGlyph dataSourceGlyph;
		void OnDataAwareControlChanged() {
			this.dataSourcePropertyDescriptorCore = null;
			this.glyphCore = null;
			if(DataAwareControl != null) {
				if(dataSourceGlyph != null)
					dataSourceGlyph.Dispose();
				dataSourceGlyph = new DataSourceGlyph(this);
				DataAwareControl.Controls.Add(dataSourceGlyph);
				dataSourceGlyph.UpdatePosition();
				UnhookDataSourceGlyph();
			}
			else {
				if(dataSourceGlyph != null)
					dataSourceGlyph.Dispose();
			}
		}
		static System.Reflection.MethodInfo mInfoUnhookChildControls;
		void UnhookDataSourceGlyph() {
			if(mInfoUnhookChildControls == null)
				mInfoUnhookChildControls = typeof(System.Windows.Forms.Design.ControlDesigner).GetMethod("UnhookChildControls", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var host = DataAwareControl.Site.GetService(typeof(System.ComponentModel.Design.IDesignerHost)) as System.ComponentModel.Design.IDesignerHost;
			var designer = host.GetDesigner(DataAwareControl) as System.Windows.Forms.Design.ControlDesigner;
			if(designer != null)
				mInfoUnhookChildControls.Invoke(designer, new object[] { dataSourceGlyph });
		}
		public bool RunDataSourceWizard() {
			if(DataAwareControl == null || DataAwareControl.Site == null) return false;
			var serviceContainer = DataAwareControl.Site.GetService(typeof(System.ComponentModel.Design.IDesignerHost))
				as System.ComponentModel.Design.IServiceContainer;
			if(Utils.Design.DebugInfoDesigner.IsDebugging(serviceContainer))
				return false;
			if(serviceContainer != null) 
				DataSourceWizard.Run(serviceContainer, DataAwareControl);
			return serviceContainer != null;
		}
	}
}
