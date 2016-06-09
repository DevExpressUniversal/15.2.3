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

using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DashboardWin.Bars.Native {
	[DXToolboxItem(false)]
	public abstract class DashboardMenuFileLabel : AppMenuFileLabel, IComparable<DashboardMenuFileLabel> {
		internal static string GetCorrectedDirectoryPath(string path) {
			string description = PatchForSpecialFolder(PatchForSpecialFolder(path, Environment.SpecialFolder.DesktopDirectory), Environment.SpecialFolder.MyDocuments);
			if(!EndsWithDirectorySeparator(description) && !System.IO.Path.HasExtension(description))
				description = String.Concat(description, System.IO.Path.DirectorySeparatorChar);
			string directoryName = System.IO.Path.GetDirectoryName(description);
			if(!String.IsNullOrEmpty(directoryName))
				description = directoryName;
			return EndsWithDirectorySeparator(description) ? description.Substring(0, description.Length - 1) : description;
		}
		static string PatchForSpecialFolder(string path, Environment.SpecialFolder specialFolder) {
			string specialFolderPath = Environment.GetFolderPath(specialFolder);
			return path.Contains(specialFolderPath) ? String.Format("{0}{1}", System.IO.Path.GetFileName(specialFolderPath), path.Remove(0, specialFolderPath.Length)) : path;
		}
		static bool EndsWithDirectorySeparator(string path) {
			int length = path.Length;
			return length > 0 && path[length - 1] == System.IO.Path.DirectorySeparatorChar;
		}
		IServiceProvider serviceProvider;
		readonly IRecentDashboardsController controller;
		readonly string path;
		protected abstract string FilePath { get; }
		protected abstract string DirectoryPath { get; }
		protected internal IServiceProvider ServiceProvider { get { return serviceProvider; } }
		protected internal IRecentDashboardsController Controller { get { return controller; } }
		internal string Path { get { return path; } }
		protected DashboardMenuFileLabel(IServiceProvider serviceProvider, IRecentDashboardsController controller, string path) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			Guard.ArgumentNotNull(controller, "controller");
			this.serviceProvider = serviceProvider;
			this.controller = controller;
			this.path = path;
			Image = ImageHelper.GetImage("Bars.PinButton");
			SelectedImage = ImageHelper.GetImage("Bars.UnpinButton");
			AutoHeight = true;
			Dock = DockStyle.Top;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (serviceProvider != null)
					serviceProvider = null;
			}
			base.Dispose(disposing);
		}
		internal void Execute() {
			IDashboardCommandService commandService = ServiceProvider.GetService<IDashboardCommandService>();
			if (commandService != null)
				commandService.ExecuteCommand(DashboardCommandId.OpenDashboardPath, new OpenDashboardPathCommandUIState(FilePath, DirectoryPath));
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			AppMenuFileLabelHitInfo info = ViewInfo.CalcHitInfo(e.Location, e.Button);
			if(e.Button == MouseButtons.Left) {
				if(info.HitTest == AppMenuFileLabelHitTest.Label) {
					controller.HideRecentControl();
					Execute();
				}
				else if(info.HitTest == AppMenuFileLabelHitTest.LabelImage)
					controller.InitializeRecentItems(true);
			}
		}
		int IComparable<DashboardMenuFileLabel>.CompareTo(DashboardMenuFileLabel label) {
			return -path.CompareTo(label.path);
		}
	}
}
