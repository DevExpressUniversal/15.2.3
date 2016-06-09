#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using System.Drawing;
using DevExpress.DashboardCommon.Service;
using DevExpress.Utils.Controls;
using System.Drawing.Imaging;
using System;
namespace DevExpress.DashboardExport {
	public class ExportContentScrollableControl : IDisposable {
		public static Color SelectedBackColor = Color.FromArgb(255, 204, 204, 204);
		public static Image BlackoutImage(Image image) {
			ImageAttributes ia = new ImageAttributes();
			ColorMatrix cm = new ColorMatrix();
			cm.Matrix00 = cm.Matrix11 = cm.Matrix22 = 0;
			cm.Matrix33 = 0.2f;
			ia.SetColorMatrix(cm);
			Bitmap bmp = new Bitmap(image);
			Graphics g = Graphics.FromImage(bmp);
			g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);
			return bmp;
		}
		ItemViewerClientState clientState;
		readonly bool useClientScrollState;
		ContentScrollableControlModel contentScrollableControl;
		public ContentScrollableControlModel ContentScrollableControlModel { get { return contentScrollableControl; } }
		public ExportContentScrollableControl(IContentProvider viewControl, ContentDescriptionViewModel contentDescription, ItemViewerClientState clientState, bool useClientScrollState) {
			this.clientState = clientState;
			this.useClientScrollState = useClientScrollState;
			contentScrollableControl = new ContentScrollableControlModel(viewControl);
			contentScrollableControl.InitializeScrollBars(new ExportScrollBar(clientState.HScrollingState), new ExportScrollBar(clientState.VScrollingState));
			contentScrollableControl.InitializeContent(contentDescription);
			contentScrollableControl.RequestClientSize += OnRequestClientSize;
		}
		public void UpdateClientState(ItemViewerClientState clientState) {
			this.clientState = clientState;
			contentScrollableControl.Arrange();
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing)
				contentScrollableControl.RequestClientSize -= OnRequestClientSize;
		}
		void OnRequestClientSize(object sender, RequestClientSizeEventArgs e) {
			Size size = new Size { Width = clientState.ViewerArea.Width, Height = clientState.ViewerArea.Height };
			if(!useClientScrollState) {
				if(clientState.HScrollingState != null && clientState.VScrollingState != null)
					size = new Size(clientState.HScrollingState.VirtualSize, clientState.VScrollingState.VirtualSize);
				else {
					if(clientState.HScrollingState != null) {
						size.Width = clientState.HScrollingState.VirtualSize;
						size.Height -= clientState.HScrollingState.ScrollBarSize;
					}
					else if(clientState.VScrollingState != null) {
						size.Height = clientState.VScrollingState.VirtualSize;
						size.Width -= clientState.VScrollingState.ScrollBarSize;
					}
				}
			}
			e.ClientSize = size;
		}
	}
}
