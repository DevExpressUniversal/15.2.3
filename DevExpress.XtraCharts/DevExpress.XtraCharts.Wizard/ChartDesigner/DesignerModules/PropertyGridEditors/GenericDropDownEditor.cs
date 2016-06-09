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
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class GenericModelTypePickerControl<T> : GenericTypePickerControl<T> where T : class {
		protected GenericModelTypePickerControl(GenericTypePickerEditor<T> editor, object editValue)
			: base(editor, editValue) {
		}
		protected override Type[] GetSupportedTypes(ITypeDescriptorContext context) {
			return ReflectionHelper.GetTypeDescendants(ReflectionHelper.WizardAssembly, typeof(T), new List<Type>());
		}
	}
	public abstract class GenericModelTypePickerEditor<T> : GenericTypePickerEditor<T> where T : class {
		protected virtual object CreateModel(Type modelType, ChartElement chartElement, CommandManager commandManager) {
			return Activator.CreateInstance(modelType, chartElement, commandManager);
		}
		protected virtual ChartElement CreateChartElement(Type type) {
			return (ChartElement)Activator.CreateInstance(type);
		}
		protected virtual void CacheOldModel(DesignerChartElementModelBase model) {
		}
		public override object CreateInstanceByType(Type type, ITypeDescriptorContext context) {
			DesignerChartElementModelBase model = context.Instance as DesignerChartElementModelBase;
			object[] attributes = type.GetCustomAttributes(typeof(ModelOf), false);
			if (model != null && attributes.Length > 0 && attributes[0] is ModelOf) {
				CacheOldModel(model);
				ChartElement chartElement = CreateChartElement(((ModelOf)attributes[0]).ChartElementType);
				return CreateModel(type, chartElement, model.CommandManager);
			}
			return null;
		}
	}
	public class ToolTipPositionModelPickerControl : GenericModelTypePickerControl<ToolTipPositionModel> {
		protected internal override bool SupportNoneItem { get { return false; } }
		public ToolTipPositionModelPickerControl(GenericTypePickerEditor<ToolTipPositionModel> editor, object editValue)
			: base(editor, editValue) {
		}
	}
	public class ToolTipPositionModelPickerEditor : GenericModelTypePickerEditor<ToolTipPositionModel> {
		protected override GenericTypePickerControl<ToolTipPositionModel> CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new ToolTipPositionModelPickerControl(this, value);
		}
	}
	public class CrosshairLabelPositionModelPickerControl : GenericModelTypePickerControl<CrosshairLabelPositionModel> {
		protected internal override bool SupportNoneItem { get { return false; } }
		public CrosshairLabelPositionModelPickerControl(GenericTypePickerEditor<CrosshairLabelPositionModel> editor, object editValue)
			: base(editor, editValue) {
		}
	}
	public class CrosshairLabelPositionModelPickerEditor : GenericModelTypePickerEditor<CrosshairLabelPositionModel> {
		protected override GenericTypePickerControl<CrosshairLabelPositionModel> CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new CrosshairLabelPositionModelPickerControl(this, value);
		}
	}
	public class AnnotationAnchorPointModelPickerControl : GenericModelTypePickerControl<AnnotationAnchorPointModel> {
		protected internal override bool SupportNoneItem { get { return false; } }
		public AnnotationAnchorPointModelPickerControl(GenericTypePickerEditor<AnnotationAnchorPointModel> editor, object editValue)
			: base(editor, editValue) {
		}
		protected override Type[] GetSupportedTypes(ITypeDescriptorContext context) {
			List<Type> anchorPointTypes = base.GetSupportedTypes(context).ToList();
			AnnotationModel annotationModel = (AnnotationModel)context.Instance;
			Annotation annotation = (Annotation)annotationModel.ChartElement;
			if(!AnnotationHelper.IsPaneAnchorPointSupported(annotation))
				anchorPointTypes.Remove(typeof(PaneAnchorPointModel));
			if(!AnnotationHelper.IsSeriesPointAnchorPointSupported(annotation))
				anchorPointTypes.Remove(typeof(SeriesPointAnchorPointModel));
			return anchorPointTypes.ToArray();
		}
	}
	public class AnnotationAnchorPointModelPickerEditor : GenericModelTypePickerEditor<AnnotationAnchorPointModel> {
		AnnotationModel oldAnnotation;
		protected override void CacheOldModel(DesignerChartElementModelBase model) {
			this.oldAnnotation = (AnnotationModel)model;
		}
		protected override ChartElement CreateChartElement(Type type) {
			DesignerChartModel chartModel = oldAnnotation.FindParent<DesignerChartModel>();
			if(type.Equals(typeof(ChartAnchorPoint)))
				return AnnotationHelper.CreateChartAnchorPoint(oldAnnotation.Annotation, (Chart)chartModel.ChartElement);
			if(type.Equals(typeof(PaneAnchorPoint)))
				return AnnotationHelper.CreatePaneAnchorPoint(oldAnnotation.Annotation, (Chart)chartModel.ChartElement);
			return base.CreateChartElement(type);
		}
		protected override GenericTypePickerControl<AnnotationAnchorPointModel> CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new AnnotationAnchorPointModelPickerControl(this, value);
		}
	}
	public class AnnotationShapePositionModelPickerControl : GenericModelTypePickerControl<AnnotationShapePositionModel> {
		protected internal override bool SupportNoneItem { get { return false; } }
		public AnnotationShapePositionModelPickerControl(GenericTypePickerEditor<AnnotationShapePositionModel> editor, object editValue)
			: base(editor, editValue) {
		}
	}
	public class AnnotationShapePositionModelPickerEditor : GenericModelTypePickerEditor<AnnotationShapePositionModel> {
		protected override GenericTypePickerControl<AnnotationShapePositionModel> CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new AnnotationShapePositionModelPickerControl(this, value);
		}
	}
}
