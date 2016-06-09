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
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Layout.Core;
using Microsoft.Win32;
using DevExpress.Xpf.Docking.Base;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_ButtonSave", Type = typeof(Button))]
	[TemplatePart(Name = "PART_ButtonRestore", Type = typeof(Button))]
	public class ButtonsPanel : psvControl {
		#region static
		static ButtonsPanel() {
			var dProp = new DependencyPropertyRegistrator<ButtonsPanel>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		protected Button ButtonSave { get; private set; }
		protected Button ButtonRestore { get; private set; }
		protected IEnumerator Children { get; private set; }
		SerializationHelper serializationHelper;
		public ButtonsPanel() {
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ButtonSave = GetTemplateChild("PART_ButtonSave") as Button;
			ButtonRestore = GetTemplateChild("PART_ButtonRestore") as Button;
			ButtonSave.Content = DockingLocalizer.GetString(DockingStringId.ButtonSave);
			ButtonRestore.Content = DockingLocalizer.GetString(DockingStringId.ButtonRestore);
			ButtonSave.Click += OnSaveButtonClick;
			ButtonRestore.Click += OnRestoreButtonClick;
			Focusable = false;
			serializationHelper = new SerializationHelper(Container);
		}
		protected virtual void OnRestoreButtonClick(object sender, RoutedEventArgs e) {
			if(serializationHelper != null)
				serializationHelper.RestoreLayout();
		}
		protected virtual void OnSaveButtonClick(object sender, RoutedEventArgs e) {
			if(serializationHelper != null)
				serializationHelper.SaveLayout();
		}
	}
	class SerializationHelper {
		DockLayoutManager Container;
		Window OwnerWindow;
		Stream memoryStream;
		public SerializationHelper(DockLayoutManager container) {
			Container = container;
			OwnerWindow = Window.GetWindow(Container);
		}
		public void RestoreLayout() {
			if(WindowHelper.IsXBAP) RestoreLayoutFromStream(memoryStream);
			else RestoreLayoutFromFile();
		}
		public void SaveLayout() {
			if(WindowHelper.IsXBAP) SaveLayoutToStream();
			else SaveLayoutToFile();
		}
		void PrepareFileDialog(FileDialog dialog) {
			dialog.FileName = "Layout";
			dialog.DefaultExt = ".xml";
			dialog.Filter = "*.xml|*.xml";
		}
		void SaveLayoutToFile() {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			PrepareFileDialog(saveFileDialog);
			bool? result = saveFileDialog.ShowDialog(OwnerWindow);
			if(result != null && result.Value) {
				Container.SaveLayoutToXml(saveFileDialog.FileName);
			}
		}
		void RestoreLayoutFromFile() {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			PrepareFileDialog(openFileDialog);
			bool? result = openFileDialog.ShowDialog(OwnerWindow);
			if(result != null && result.Value) {
				Container.RestoreLayoutFromXml(openFileDialog.FileName);
			}
		}
		void RestoreLayoutFromStream(Stream stream) {
			if(stream == null) return;
			stream.Seek(0, SeekOrigin.Begin);
			Container.RestoreLayoutFromStream(stream);
		}
		void SaveLayoutToStream() {
			Ref.Dispose(ref memoryStream);
			memoryStream = new MemoryStream();
			Container.SaveLayoutToStream(memoryStream);
		}
	}
}
