module __aspxRichEdit {
    export class RulerSectionColumnsSettingsCommand extends CommandBase<RulerSectionColumnsSettingsState> {
        columnsCalculator: ColumnsCalculator;

        constructor(control: IRichEditControl) {
            super(control);
            this.columnsCalculator = new ColumnsCalculator(new TwipsUnitConverter());
        }

        getActualInterval(): FixedInterval {
            var lastSelectedInterval = this.control.selection.getLastSelectedInterval();
            return new FixedInterval(this.control.selection.forwardDirection ? lastSelectedInterval.end() : lastSelectedInterval.start, 0);
        }
        getState(): RulerSectionColumnsSettingsState {
            var interval = this.getActualInterval();
            var position = interval.start;
            var sectionIndex = Utils.normedBinaryIndexOf(this.control.model.sections, (s: Section) => s.startLogPosition.value - position);
            var section = this.control.model.sections[sectionIndex];

            var columnsBounds = this.columnsCalculator.generateSectionColumns(section.sectionProperties);
            var columns: SectionColumnProperties[] = [];
            for(var i = 0, columnBound: Rectangle; columnBound = columnsBounds[i]; i++) {
                columns.push(new SectionColumnProperties(columnBound.width, 0));
                if(i > 0) {
                    var prevBound = columnsBounds[i - 1];
                    columns[i - 1].space = columnBound.x - (prevBound.x + prevBound.width);
                }
            }
            var layoutPosition: LayoutPosition = this.control.model.activeSubDocument.isMain() ?
                new LayoutPositionMainSubDocumentCreator(this.control.layout, this.control.model.activeSubDocument, position, DocumentLayoutDetailsLevel.Column)
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true)) : null;

            return new RulerSectionColumnsSettingsState(this.isEnabled(), interval, columns, section.sectionProperties.equalWidthColumns, layoutPosition ? layoutPosition.columnIndex : 0);
        }
        executeCore(state: RulerSectionColumnsSettingsState, parameter: SectionColumnProperties[]): boolean {
            var rulerState: RulerSectionColumnsSettingsState = <RulerSectionColumnsSettingsState>state;
            var changed: boolean = false;
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            if(rulerState.equalWidth) {
                var stateColumn: SectionColumnProperties = (<SectionColumnProperties[]>rulerState.value)[0];
                if(parameter[0].space !== stateColumn.space) {
                    this.control.history.addAndRedo(new SectionSpaceHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, rulerState.interval,
                        UnitConverter.pixelsToTwips(parameter[0].space)));
                    changed = true;
                }
            }
            else {
                var newColumnsInfo: SectionColumnProperties[] = [];
                var oldColumnsInfo: SectionColumnProperties[] = <SectionColumnProperties[]>rulerState.value;
                for(var i = 0; i < parameter.length; i++) {
                    changed = changed || parameter[i].width !== oldColumnsInfo[i].width || parameter[i].space !== oldColumnsInfo[i].space;
                    newColumnsInfo.push(new SectionColumnProperties(UnitConverter.pixelsToTwips(parameter[i].width), UnitConverter.pixelsToTwips(parameter[i].space || 0)));
                }
                if(changed)
                    this.control.history.addAndRedo(new SectionColumnsInfoHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, rulerState.interval, newColumnsInfo));
            }
            return changed;
        }
    }

    export class RulerSectionColumnsSettingsState extends IntervalCommandState {
        equalWidth: boolean;
        activeIndex: number;

        constructor(enabled: boolean, interval: FixedInterval, columns: SectionColumnProperties[], equalWidth: boolean, activeIndex: number) {
            super(enabled, interval, columns);
            this.equalWidth = equalWidth;
            this.activeIndex = activeIndex;
        }
    }
} 