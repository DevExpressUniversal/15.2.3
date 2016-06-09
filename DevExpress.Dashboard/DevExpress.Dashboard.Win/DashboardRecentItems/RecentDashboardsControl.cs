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
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Bars.Native;
namespace DevExpress.DashboardWin.Bars {
	[DXToolboxItem(false)]
	public partial class RecentDashboardsControl : RibbonApplicationUserControl, IRecentDashboardsController {
		const string fileName = "RecentDashboards.xml";
		const string xmlRecentItems = "RecentItems";
		const string xmlRecentItem = "RecentItem";
		const string xmlRecentDashboards = "RecentDashboards";
		const string xmlRecentFolders = "RecentFolders";
		const string xmlDescription = "Description";
		const string xmlDashboardMenuFileLabelChecked = "Checked";
		const string xmlRecentDashboardsNumber = "RecentDashboardsNumber";
		const int maxRecentItemsCount = 25;
		static string RecentDashboardsFilePath {
			get {
				return string.Format(@"{0}\{1}", GetRecentDashboardsFilePath(Application.CompanyName, Application.ProductName, Application.ProductVersion), fileName);
			}
		}
		internal static string GetRecentDashboardsFilePath(string companyName, string productName, string productVersion) {
			string initialPath = string.Format(@"{0}\{1}\{2}\{3}",
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				GetCorrectString(companyName),
				GetCorrectString(productName),
				GetCorrectString(productVersion));
			string path = string.Concat(initialPath.Split(Path.GetInvalidPathChars()));
			if(!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
		static string GetCorrectString(string str) {
			return string.IsNullOrEmpty(str) ? string.Empty : string.Concat(str.Split(Path.GetInvalidFileNameChars()));
		}
		static void SaveDashboardMenuLabelsToXml(XElement rootElement, List<DashboardMenuFileLabel> recentItems) {
			foreach(DashboardMenuFileLabel label in recentItems) {
				XElement el = new XElement(xmlRecentItem);
				el.Add(new XAttribute(xmlDescription, label.Path));
				if(label.Checked)
					el.Add(new XAttribute(xmlDashboardMenuFileLabelChecked, String.Empty));
				rootElement.Add(el);
			}
		}
		readonly List<DashboardMenuFileLabel> recentDashboards = new List<DashboardMenuFileLabel>();
		readonly List<DashboardMenuFileLabel> recentFolders = new List<DashboardMenuFileLabel>();
		readonly Locker locker = new Locker();
		IServiceProvider serviceProvider;
		int currentRecentCount;
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DashboardDesigner DashboardDesigner { get { return null; } set { ServiceProvider = value; } }
		public IServiceProvider ServiceProvider {
			get { return serviceProvider; }
			set {
				serviceProvider = value;
				if(serviceProvider != null) {
					locker.Lock();
					DashboardBackstageViewControl viewControl = ViewControl;
					if(viewControl != null)
						viewControl.SetSeparatorIndex();
					try {
						string path = RecentDashboardsFilePath;
						if(File.Exists(path))
							using(Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
							using(XmlTextReader reader = new XmlTextReader(stream)) {
								XDocument document = XDocument.Load(reader);
								if(document != null) {
									XElement rootElement = document.Root;
									if(rootElement != null && rootElement.Name == xmlRecentItems) {
										XElement dashboardsElement = rootElement.Element(xmlRecentDashboards);
										if(dashboardsElement != null) {
											LoadDashboardMenuLabelsFromXml(dashboardsElement);
											string numString = XmlHelper.GetAttributeValue(dashboardsElement, xmlRecentDashboardsNumber);
											if(!String.IsNullOrEmpty(numString)) {
												int newCurrentRecentCount = XmlHelper.FromString<int>(numString);
												if(newCurrentRecentCount > 0) {
													currentRecentCount = newCurrentRecentCount;
													seRecentCount.Value = newCurrentRecentCount;
													ceRecentCount.Checked = true;
												}
											}
										}
										XElement foldersElement = rootElement.Element(xmlRecentFolders);
										if(foldersElement != null)
											LoadDashboardMenuLabelsFromXml(foldersElement);
									}
								}
							}
					}
					catch {
					}
					finally {
						locker.Unlock();
					}
				}
				((IRecentDashboardsController)this).InitializeRecentItems(false);
			}
		}
		internal BarAndDockingController Controller {
			set {
				if(value != null)
					barManager.Controller = value;
			}
		}
		DashboardBackstageViewControl ViewControl {
			get {
				Control parent = Parent;
				return parent == null ? null : parent.Parent as DashboardBackstageViewControl;
			}
		}
		IEnumerable<DashboardMenuFileLabel> IRecentDashboardsController.RecentDashboards { get { return recentDashboards; } }
		IEnumerable<DashboardMenuFileLabel> IRecentDashboardsController.RecentFolders { get { return recentFolders; } }
		public RecentDashboardsControl() {
			InitializeComponent();
			DashboardBackstageViewControl viewControl = ViewControl;
			if(viewControl != null)
				viewControl.SetSeparatorIndex();
			currentRecentCount = Convert.ToInt32(seRecentCount.Value);
		}
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
				recentDashboards.ForEach(recentDashboard => recentDashboard.Dispose());
				recentFolders.ForEach(rf => rf.Dispose());
				serviceProvider = null;
			}
			base.Dispose(disposing);
		}
		void LoadDashboardMenuLabelsFromXml(XElement rootElement) {
			bool isDashboardsElement = rootElement.Name == xmlRecentDashboards;
			foreach(XElement el in rootElement.Elements()) {
				string description = XmlHelper.GetAttributeValue(el, xmlDescription);
				if(!String.IsNullOrEmpty(description)) {
					DashboardMenuFileLabel label = null;
					if(isDashboardsElement) {
						if(!Path.HasExtension(description))
							continue;
						label = new RecentDashboardFileLabel(serviceProvider, this, description);
						recentDashboards.Add(label);
					}
					else {
						if(Path.HasExtension(description))
							continue;
						label = new RecentFolderFileLabel(serviceProvider, this, description);
						recentFolders.Add(label);
					}
					label.Checked = XmlHelper.GetAttributeValue(el, xmlDashboardMenuFileLabelChecked) != null;
					label.MouseClick += RecentDashboardLabelMouseClick;
				}
			}
		}
		void SaveRecentDashboards() {
			if(!locker.IsLocked)
				using(Stream stream = new FileStream(RecentDashboardsFilePath, FileMode.Create, FileAccess.Write))
				using(XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8)) {
					writer.Formatting = Formatting.Indented;
					XElement rootElement = new XElement(xmlRecentItems);
					try {
						XElement dashboardsElement = new XElement(xmlRecentDashboards);
						SaveDashboardMenuLabelsToXml(dashboardsElement, recentDashboards);
						rootElement.Add(dashboardsElement);
						XElement foldersElement = new XElement(xmlRecentFolders);
						SaveDashboardMenuLabelsToXml(foldersElement, recentFolders);
						rootElement.Add(foldersElement);
						if(ceRecentCount.Checked)
							dashboardsElement.Add(new XAttribute(xmlRecentDashboardsNumber, seRecentCount.Value));
						new XDocument(rootElement).WriteTo(writer);
					}
					finally {
						writer.Flush();
					}
				}
		}
		void CheckRecentItemsCount(IList<DashboardMenuFileLabel> recentItems) {
			if(recentItems.Count >= maxRecentItemsCount) {
				DashboardMenuFileLabel label = recentItems[0];
				recentItems.Remove(label);
				label.MouseClick -= RecentDashboardLabelMouseClick;
				label.Dispose();
			}
		}
		void RemoveDashboardMenuFileLabel(DashboardMenuFileLabel label, IList<DashboardMenuFileLabel> recentItems, ControlCollection controls) {
			if(recentItems.Contains(label))
				recentItems.Remove(label);
			if(controls.Contains(label))
				controls.Remove(label);
			label.MouseClick -= RecentDashboardLabelMouseClick;
			label.Dispose();
			int lastControlIndex = controls.Count - 1;
			if(lastControlIndex >= 0) {
				LabelControl labelControl = controls[lastControlIndex] as LabelControl;
				if(labelControl != null) {
					controls.Remove(labelControl);
					labelControl.Dispose();
				}
			}
		}
		void RecentCountEditValueChanged(object sender, EventArgs e) {
			int newRecentCount = Convert.ToInt32(seRecentCount.Value);
			if(ViewControl != null && newRecentCount != currentRecentCount) {
				RefreshQuickAccessRecentItems(newRecentCount, false);
				SaveRecentDashboards();
			}
		}
		void RecentCountCheckedChanged(object sender, EventArgs e) {
			bool recentCountChecked = ceRecentCount.Checked;
			seRecentCount.Enabled = recentCountChecked;
			DashboardBackstageViewControl viewControl = ViewControl;
			if(viewControl != null) {
				BackstageViewControlItemCollecton items = viewControl.Items;
				if(recentCountChecked) {
					int index = viewControl.SeparatorIndex;
					int maxRecentCount = (int)seRecentCount.Value;
					if(maxRecentCount > 0) {
						items.Insert(index++, new BackstageViewItemSeparator());
						for(int i = panelRecentDashboards.Controls.Count - 1, counter = 0; i >= 0 && counter < maxRecentCount; i--, counter++) {
							DashboardMenuFileLabel label = panelRecentDashboards.Controls[i] as DashboardMenuFileLabel;
							if(label != null)
								items.Insert(index++, CreateRecentItem(label.Path, label.Text));
							else
								counter--;
						}
					}
				}
				else
					viewControl.ClearRecentItems();
				SaveRecentDashboards();
			}
		}
		DashboardRecentItem CreateRecentItem(string fileName, string caption) {
			return new DashboardRecentItem(fileName) {
				ServiceProvider = serviceProvider,
				Glyph = ImageHelper.GetImage("Bars.NewDashboard_16x16"),
				Caption = caption
			};
		}
		void RefreshQuickAccessRecentItems(int newRecentCount, bool isNewDashboardAdded) {
			DashboardBackstageViewControl viewControl = ViewControl;
			if(viewControl != null && ceRecentCount.Checked) {
				int separatorIndex = viewControl.SeparatorIndex;
				BackstageViewControlItemCollecton items = viewControl.Items;
				IList<DashboardRecentItem> recentItems = viewControl.RecentItems;
				int recentItemsCount = recentItems.Count;
				int diff = newRecentCount - currentRecentCount;
				if(diff > 0) {
					if(currentRecentCount == 0)
						items.Insert(separatorIndex, new BackstageViewItemSeparator());
					int dashboardsToAdd = Math.Min(diff, recentDashboards.Count - recentItemsCount);
					if(dashboardsToAdd > 0) {
						List<DashboardMenuFileLabel> labels = new List<DashboardMenuFileLabel>();
						foreach(Control control in panelRecentDashboards.Controls) {
							DashboardMenuFileLabel label = control as DashboardMenuFileLabel;
							if(label != null)
								labels.Add(label);
						}
						int index = isNewDashboardAdded ? recentDashboards.Count : (recentDashboards.Count - recentItemsCount);
						int position = isNewDashboardAdded ? separatorIndex : (separatorIndex + recentItemsCount);
						for(int i = 0; i < dashboardsToAdd; i++) {
							DashboardMenuFileLabel label = labels[--index];
							items.Insert(++position, CreateRecentItem(label.Path, label.Text));
						}
					}
				}
				else if(diff < 0) {
					diff = -diff;
					for(int i = 0; i < diff && recentItemsCount > newRecentCount; i++) {
						DashboardRecentItem recentItem = recentItems[--recentItemsCount];
						items.Remove(recentItem);
						recentItems.Remove(recentItem);
						recentItem.Dispose();
					}
					if(newRecentCount == 0)
						viewControl.RemoveSeparator();
				}
				else if(ceRecentCount.Checked) {
					viewControl.BeginUpdate();
					try {
						viewControl.ClearRecentItems();
						ControlCollection controls = panelRecentDashboards.Controls;
						int lastControlIndex = controls.Count - 1;
						if(lastControlIndex >= 0) {
							if(!(controls[0] is LabelControl))
								items.Insert(separatorIndex, new BackstageViewItemSeparator());
							for(int i = lastControlIndex, counter = 0, index = separatorIndex + 1; i >= 0 && counter < newRecentCount; i--) {
								RecentDashboardFileLabel label = controls[i] as RecentDashboardFileLabel;
								if(label != null) {
									counter++;
									items.Insert(index++, CreateRecentItem(label.Path, label.Text));
								}
							}
						}
					}
					finally {
						viewControl.EndUpdate();
					}
				}
			}
			currentRecentCount = newRecentCount;
		}
		void RecentDashboardLabelMouseClick(object sender, MouseEventArgs e) {
			DashboardMenuFileLabel label = sender as DashboardMenuFileLabel;
			if(label != null && e.Button == MouseButtons.Right) {
				PopulatePopupMenu(label);
				barManager.Controller = ViewControl.Controller;
				popupMenu.ShowPopup(label.PointToScreen(e.Location));
			}
		}
		void PopulatePopupMenu(DashboardMenuFileLabel label) {
			popupMenu.ClearLinks();
			popupMenu.AddItem(new OpenRecentBarButtonItem(label));
			popupMenu.AddItem(new PinRecentBarButtonItem(label));
			popupMenu.AddItem(new RemoveRecentBarButtonItem(label));
			if(label is RecentDashboardFileLabel)
				popupMenu.AddItem(new ClearUnpinnedDashboardsRecentBarButtonItem(label));
			else
				popupMenu.AddItem(new ClearUnpinnedFoldersRecentBarButtonItem(label));
		}
		void InitializeRecentMenuLabels(Control parent, List<DashboardMenuFileLabel> recentLabels) {
			parent.SuspendLayout();
			try {
				ControlCollection controls = parent.Controls;
				int recentLabelsCount = recentLabels.Count;
				if(recentLabelsCount == 0 && parent == panelRecentDashboards) {
					LabelControl label = new LabelControl();
					label.Text = DashboardWinLocalizer.GetString(DashboardWinStringId.RecentItemsMenuEmptyText);
					controls.Add(label);
				}
				else {
					controls.Clear();
					List<DashboardMenuFileLabel> pinnedLabels = new List<DashboardMenuFileLabel>();
					foreach(DashboardMenuFileLabel label in recentLabels)
						if(label.Checked)
							pinnedLabels.Add(label);
						else
							controls.Add(label);
					int pinnedLabelsCount = pinnedLabels.Count;
					if(pinnedLabelsCount > 0) {
						int index = controls.Count;
						LabelControl splitter = null;
						if(recentLabelsCount > pinnedLabelsCount) {
							splitter = new LabelControl();
							splitter.AutoSizeMode = LabelAutoSizeMode.None;
							splitter.Dock = DockStyle.Top;
							splitter.LineLocation = LineLocation.Center;
							splitter.LineVisible = true;
							splitter.ShowLineShadow = false;
							controls.Add(splitter);
						}
						int controlsCount = controls.Count;
						pinnedLabels.Sort();
						foreach(DashboardMenuFileLabel label in pinnedLabels) {
							controls.Add(label);
							controls.SetChildIndex(label, ++controlsCount);
						}
						if(splitter != null)
							controls.SetChildIndex(splitter, index);
					}
				}
			}
			finally {
				parent.ResumeLayout();
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			BackstageViewControl viewControl = ViewControl;
			if(viewControl != null)
				BackstageViewPainter.DrawBackstageViewImage(e, this, viewControl);
		}
		void IRecentDashboardsController.InitializeRecentItems(bool saveRecentDashboards) {
			SuspendLayout();
			try {
				InitializeRecentMenuLabels(panelRecentDashboards, recentDashboards);
				InitializeRecentMenuLabels(panelRecentPlaces, recentFolders);
				RefreshQuickAccessRecentItems(currentRecentCount, true);
				if(saveRecentDashboards)
					SaveRecentDashboards();
			}
			finally {
				ResumeLayout();
			}
		}
		internal void AddRecentDashboard(string name) {
			if(serviceProvider != null) {
				CheckRecentItemsCount(recentDashboards);
				RecentDashboardFileLabel recentDashboard = null;
				foreach(RecentDashboardFileLabel recentItem in recentDashboards)
					if(recentItem.Path == name) {
						recentDashboard = recentItem;
						recentDashboards.Remove(recentDashboard);
						break;
					}
				if(recentDashboard == null) {
					recentDashboard = new RecentDashboardFileLabel(serviceProvider, this, name);
					recentDashboard.MouseClick += RecentDashboardLabelMouseClick;
				}
				recentDashboards.Insert(recentDashboards.Count, recentDashboard);
				CheckRecentItemsCount(recentFolders);
				string description = DashboardMenuFileLabel.GetCorrectedDirectoryPath(name);
				DashboardMenuFileLabel recentFolder = null;
				foreach(DashboardMenuFileLabel recentItem in recentFolders)
					if(recentItem.Description == description) {
						recentFolder = recentItem;
						recentFolders.Remove(recentFolder);
						break;
					}
				if(recentFolder == null) {
					recentFolder = new RecentFolderFileLabel(serviceProvider, this, description);
					recentFolder.MouseClick += RecentDashboardLabelMouseClick;
				}
				recentFolders.Insert(recentFolders.Count, recentFolder);
				((IRecentDashboardsController)this).InitializeRecentItems(true);
			}
		}
		void IRecentDashboardsController.RemoveDashboardMenuFileLabel(DashboardMenuFileLabel label) {
			try {
				label.MouseClick -= RecentDashboardLabelMouseClick;
				RemoveDashboardMenuFileLabel(label, recentDashboards, panelRecentDashboards.Controls);
				RemoveDashboardMenuFileLabel(label, recentFolders, panelRecentPlaces.Controls);
				DashboardBackstageViewControl viewControl = ViewControl;
				if(viewControl != null)
					viewControl.RemoveRecentItem(label);
			}
			finally {
				SaveRecentDashboards();
			}
		}
		void IRecentDashboardsController.HideRecentControl() {
			BackstageViewClientControl clientControl = Parent as BackstageViewClientControl;
			if(clientControl != null) {
				DashboardBackstageViewControl viewControl = clientControl.Parent as DashboardBackstageViewControl;
				if(viewControl != null)
					viewControl.HideContentControl();
			}
		}
	}
}
