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

using DevExpress.Mvvm;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.Reflection;
using DevExpress.Xpf.DocumentViewer;
namespace DevExpress.Xpf.PdfViewer {
	public class RecentFileViewModel : BindableBase {
		string name;
		object documentSource;
		ICommand command;
		Uri smallGlyph;
		public Uri SmallGlyph {
			get { return smallGlyph; }
			set { SetProperty(ref smallGlyph, value, () => SmallGlyph); }
		}
		public ICommand Command {
			get { return command; }
			set { SetProperty(ref command, value, () => Command); }
		}
		public string Name {
			get { return name; }
			set { SetProperty(ref name, value, () => Name); }
		}
		public object DocumentSource {
			get { return documentSource; }
			set { SetProperty(ref documentSource, value, () => DocumentSource); }
		}
		public RecentFileViewModel() {
			SmallGlyph = UriHelper.GetUri(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, @"\Images\RecentDocument_16x16.png");
		}
		public override int GetHashCode() {
			return DocumentSource.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (obj == null)
				return false;
			RecentFileViewModel viewModel = obj as RecentFileViewModel;
			if (viewModel == null)
				return false;
			return Equals(viewModel);
		}
		public bool Equals(RecentFileViewModel obj) {
			return Equals(obj.DocumentSource, DocumentSource);
		}
	}
	public class StartScreenControl : Control {
		static readonly DependencyPropertyKey ActualRecentFilesPropertyKey;
		public static readonly DependencyProperty ActualRecentFilesProperty;
		public static readonly DependencyProperty RecentFilesProperty;
		public static readonly DependencyProperty NumberOfRecentFilesProperty;
		public static readonly DependencyProperty ShowOpenFileButtonProperty;
		static StartScreenControl() {
			Type ownerType = typeof(StartScreenControl);
			RecentFilesProperty = DependencyPropertyManager.Register("RecentFiles", typeof(ObservableCollection<RecentFileViewModel>), ownerType,
				new PropertyMetadata(null, (obj, args) => ((StartScreenControl)obj).OnRecentFilesChanged((ObservableCollection<RecentFileViewModel>)args.OldValue, (ObservableCollection<RecentFileViewModel>)args.NewValue)));
			NumberOfRecentFilesProperty = DependencyPropertyManager.Register("NumberOfRecentFiles", typeof(int), ownerType,
				new PropertyMetadata(5, (obj, args) => ((StartScreenControl)obj).OnNumberOfRecentFilesChanged((int)args.NewValue)));
			ShowOpenFileButtonProperty = DependencyPropertyManager.Register("ShowOpenFileButton", typeof(bool), ownerType, 
				new PropertyMetadata(true));
			ActualRecentFilesPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualRecentFiles", typeof(ObservableCollection<RecentFileViewModel>), ownerType, 
				new PropertyMetadata(null));
			ActualRecentFilesProperty = ActualRecentFilesPropertyKey.DependencyProperty;
		}
		protected virtual void OnRecentFilesChanged(ObservableCollection<RecentFileViewModel> oldValue, ObservableCollection<RecentFileViewModel> newValue) {
			oldValue.Do(x => x.CollectionChanged -= OnRecentFilesCollectionChanged);
			UpdateActualRecentFiles();
			newValue.Do(x => x.CollectionChanged += OnRecentFilesCollectionChanged);
		}
		protected virtual void OnRecentFilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateActualRecentFiles();
		}
		protected virtual void OnNumberOfRecentFilesChanged(int newValue) {
			UpdateActualRecentFiles();
		}
		void UpdateActualRecentFiles() {
			if (RecentFiles == null)
				return;
			ObservableCollection<RecentFileViewModel> result = new ObservableCollection<RecentFileViewModel>();
			if (RecentFiles.Count <= NumberOfRecentFiles) {
				foreach (RecentFileViewModel model in RecentFiles)
					result.Insert(0, model);
			}
			else {
				for (int i = RecentFiles.Count - 1; i >= RecentFiles.Count - NumberOfRecentFiles; --i)
					result.Add(RecentFiles[i]);
			}
			ActualRecentFiles = result;
		}
		public ObservableCollection<RecentFileViewModel> RecentFiles {
			get { return (ObservableCollection<RecentFileViewModel>)GetValue(RecentFilesProperty); }
			set { SetValue(RecentFilesProperty, value); }
		}
		public ObservableCollection<RecentFileViewModel> ActualRecentFiles {
			get { return (ObservableCollection<RecentFileViewModel>)GetValue(ActualRecentFilesProperty); }
			private set { SetValue(ActualRecentFilesPropertyKey, value); }
		}
		public int NumberOfRecentFiles {
			get { return (int)GetValue(NumberOfRecentFilesProperty); }
			set { SetValue(NumberOfRecentFilesProperty, value); }
		}
		public bool ShowOpenFileButton {
			get { return (bool)GetValue(ShowOpenFileButtonProperty); }
			set { SetValue(ShowOpenFileButtonProperty, value); }
		}
		public StartScreenControl() {
			this.SetDefaultStyleKey(typeof(StartScreenControl));
		}
	}
	public class PdfResourceExtension : MarkupExtension {
		public string ResourcePath { get; set; }
		readonly string dllName;
		public PdfResourceExtension(string resourcePath) {
			ResourcePath = resourcePath;
			dllName = Assembly.GetExecutingAssembly().GetName().Name;
		}
		public sealed override object ProvideValue(IServiceProvider serviceProvider) {
			return UriHelper.GetUri(dllName, ResourcePath);
		}
	}
}
