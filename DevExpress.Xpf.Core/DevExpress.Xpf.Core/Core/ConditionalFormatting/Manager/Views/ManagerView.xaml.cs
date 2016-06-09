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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.ConditionalFormatting;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public partial class ManagerView : UserControl {
		public ManagerView() {
			InitializeComponent();
#if DEBUGTEST
			if(LoadedTestCallback != null)
				Loaded += new RoutedEventHandler(ManagerView_Loaded);
#endif
		}
#if DEBUGTEST
		public static Action<ManagerView> LoadedTestCallback;
		public UIElement GridForTests { get { return gridContent.Grid; } }
		void ManagerView_Loaded(object sender, RoutedEventArgs e) {
			LoadedTestCallback(this);
			LoadedTestCallback = null;
		}
#endif
	}
	public abstract class ManagerContainerBase : Control {
		protected ContentPresenter Container { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Container = GetTemplateChild("PART_Container") as ContentPresenter;
			Container.Content = CreateContent();
		}
		protected abstract object CreateContent();
	}
	public class GridContainer : ManagerContainerBase {
		public GridContainer() {
			this.SetDefaultStyleKey(typeof(GridContainer));
		}
		public UIElement Grid { get; private set; }
		protected override object CreateContent() {
			Grid = GridAssemblyHelper.Instance.CreateGrid();
			return Grid;
		}
	}
	public class FormatPreviewContainer : ManagerContainerBase {
		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(Freezable), typeof(FormatPreviewContainer), new PropertyMetadata(null, (d, e) => ((FormatPreviewContainer)d).UpdatePreview()));
		public Freezable Format {
			get { return (Freezable)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}
		ContentControl previewControl;
		public FormatPreviewContainer() {
			this.SetDefaultStyleKey(typeof(FormatPreviewContainer));
		}
		protected override object CreateContent() {
			previewControl = GridAssemblyHelper.Instance.CreatePreviewControl();
			UpdatePreview();
			return previewControl;
		}
		void UpdatePreview() {
			previewControl.Do(x => x.Content = Format);
		}
	}
}
namespace DevExpress.Xpf.Core.ConditionalFormatting {
	public class ConditionalFormattingManagerView : DevExpress.Xpf.Core.ConditionalFormattingManager.ManagerView { }
}
