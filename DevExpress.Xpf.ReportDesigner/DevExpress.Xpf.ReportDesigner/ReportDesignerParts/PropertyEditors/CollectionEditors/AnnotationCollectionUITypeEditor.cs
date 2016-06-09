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
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Reports.UserDesigner.Editors.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using System.Reflection;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class ChooseAnnotationType {
		public static ChooseAnnotationType Create(IEnumerable<AnnotationType> types) {
			return ViewModelSource.Create(() => new ChooseAnnotationType(types));
		}
		public ChooseAnnotationType(IEnumerable<AnnotationType> types) {
			this.types = types;
			SelectedType = types.First();
		}
		readonly IEnumerable<AnnotationType> types;
		public virtual IEnumerable<AnnotationType> Types { get { return types; } }
		public virtual AnnotationType SelectedType { get; set; }
	}
	public class AnnotationType {
		public AnnotationType(ImageSource image, string text) {
			this.image = image;
			this.text = text;
		}
		readonly ImageSource image;
		public ImageSource Image { get { return image; } }
		readonly string text;
		public string Text { get { return text; } }
	}
	public class AnnotationCollectionUITypeEditor : SingleSelectionCollectionEditor {
		public static readonly DependencyProperty OpenDialogServiceTemplateProperty;
		static AnnotationCollectionUITypeEditor() {
			DependencyPropertyRegistrator<AnnotationCollectionUITypeEditor>.New()
				.RegisterServiceTemplateProperty(d => d.OpenDialogServiceTemplate, out OpenDialogServiceTemplateProperty, out openDialogServiceAccessor)
				.OverrideDefaultStyleKey()
			;
		}
		static readonly Action<AnnotationCollectionUITypeEditor, Action<IDialogService>> openDialogServiceAccessor;
		readonly Lazy<ChooseAnnotationType> chooseAnnotationType = new Lazy<ChooseAnnotationType>(() => {
			return ChooseAnnotationType.Create(new List<AnnotationType>() {
					new AnnotationType(ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(typeof(AnnotationCollectionUITypeEditor).Assembly, "Images/AnnotationCollectionEditor100x93/TextAnnotation.png")), "Text Annotation"),
					new AnnotationType(ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(typeof(AnnotationCollectionUITypeEditor).Assembly, "Images/AnnotationCollectionEditor100x93/ImageAnnotation.png")), "Image Annotation")
				});
		});
		protected void DoWithOpenDialogService(Action<IDialogService> action) { openDialogServiceAccessor(this, action); }
		public DataTemplate OpenDialogServiceTemplate {
			get { return (DataTemplate)GetValue(OpenDialogServiceTemplateProperty); }
			set { SetValue(OpenDialogServiceTemplateProperty, value); }
		}
		public override object CreateItem() {
			throw new NotSupportedException();
		}
		protected override void AddItem(object item) {
			DoWithOpenDialogService(dialog => {
				var provider = (PropertiesProvider<IDiagramItem, IList<DiagramItem>>)EditValue.GetType().GetField("provider", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(EditValue);
				var chartDiagramItem = (XRChartDiagramItem)provider.MainComponent;
				if(dialog.ShowDialog(MessageButton.OKCancel, "Choose Annotation Type", chooseAnnotationType.Value) == MessageResult.OK) {
					var annotation = chartDiagramItem.ItemFactory(new TextAnnotation("Text Annotation", "Annotation"));
					base.AddItem(annotation);
				}
			});
		}
	}
}
