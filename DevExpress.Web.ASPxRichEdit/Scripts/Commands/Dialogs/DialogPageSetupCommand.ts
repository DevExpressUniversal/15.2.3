module __aspxRichEdit {
    export class DialogPageSetupCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            return new IntervalCommandStateEx(this.isEnabled(), SectionPropertiesCommandBase.getIntervals(this.control));
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.sections);
        }

        createParameters(): PageSetupDialogParameters {
            var dialogParams: PageSetupDialogParameters = new PageSetupDialogParameters();
            dialogParams.init(this.control.inputPosition.getMergedSectionPropertiesRaw(), this.getInitialTab());
            return dialogParams;
        }

        applyParameters(state: IntervalCommandStateEx, newParams: PageSetupDialogParameters) {
            var initParams: PageSetupDialogParameters = <PageSetupDialogParameters>this.initParams;
            var interval: FixedInterval = this.getInterval(newParams.applyTo, state);
            var isEqualIntervals : boolean = this.getInitInterval().equals(interval);
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var subDocument: SubDocument = modelManipulator.model.activeSubDocument;
            var history: IHistory = this.control.history;
            this.control.inputPosition.resetSectionMergedProperties();

            history.beginTransaction();
            if(newParams.marginBottom !== undefined && (newParams.marginBottom !== initParams.marginBottom || !isEqualIntervals))
                history.addAndRedo(new SectionMarginBottomHistoryItem(modelManipulator, subDocument, interval, newParams.marginBottom));
            if(newParams.marginLeft !== undefined && (newParams.marginLeft !== initParams.marginLeft || !isEqualIntervals))
                history.addAndRedo(new SectionMarginLeftHistoryItem(modelManipulator, subDocument, interval, newParams.marginLeft));
            if(newParams.marginRight !== undefined && (newParams.marginRight !== initParams.marginRight || !isEqualIntervals))
                history.addAndRedo(new SectionMarginRightHistoryItem(modelManipulator, subDocument, interval, newParams.marginRight));
            if(newParams.marginTop !== undefined && (newParams.marginTop !== initParams.marginTop || !isEqualIntervals))
                history.addAndRedo(new SectionMarginTopHistoryItem(modelManipulator, subDocument, interval, newParams.marginTop));
            if ((newParams.pageWidth !== undefined && newParams.pageHeight !== undefined) &&
                (newParams.pageWidth !== initParams.pageWidth || newParams.pageHeight !== initParams.pageHeight || !isEqualIntervals)) {
                var sections: Section[] = this.control.model.getSectionsByInterval(interval);
                for(var i = 0, section: Section; section = sections[i]; i++) {
                    var sectionInterval: FixedInterval = new FixedInterval(section.startLogPosition.value, section.getLength() - 1);
                    if(section.sectionProperties.landscape !== (newParams.pageWidth > newParams.pageHeight))
                        history.addAndRedo(new SectionLandscapeHistoryItem(modelManipulator, subDocument, sectionInterval, newParams.pageWidth > newParams.pageHeight));
                    history.addAndRedo(new SectionPageWidthHistoryItem(modelManipulator, subDocument, sectionInterval, newParams.pageWidth));
                    history.addAndRedo(new SectionPageHeightHistoryItem(modelManipulator, subDocument, sectionInterval, newParams.pageHeight));
                }
            }
            if (newParams.startType !== undefined && newParams.startType !== initParams.startType)
                history.addAndRedo(new SectionStartTypeHistoryItem(modelManipulator, subDocument, interval, newParams.startType));
            history.endTransaction();
        }

        getInterval(applyTo: SectionPropertiesApplyType, state: IntervalCommandStateEx): FixedInterval {
            if(applyTo == SectionPropertiesApplyType.WholeDocument)
                return new FixedInterval(0, this.control.model.mainSubDocument.getDocumentEndPosition() - 1);
            if(this.control.model.activeSubDocument.isMain()) {
                let sectionIndices = this.control.model.getSectionIndicesByIntervals(this.control.selection.intervals);
                var firstSection = this.control.model.sections[sectionIndices[0]];
                if(applyTo == SectionPropertiesApplyType.SelectedSections) {
                    var lastSection = this.control.model.sections[sectionIndices[sectionIndices.length - 1]];
                    return FixedInterval.fromPositions(firstSection.startLogPosition.value, lastSection.startLogPosition.value + lastSection.getLength() - 1);
                }
                if(applyTo == SectionPropertiesApplyType.ThisPointForward)
                    return FixedInterval.fromPositions(firstSection.startLogPosition.value, this.control.model.mainSubDocument.getDocumentEndPosition() - 1);
                return new FixedInterval(firstSection.startLogPosition.value, firstSection.getLength() - 1);
            }
            else if(this.control.model.activeSubDocument.isHeaderFooter()) {
                var layoutPage = this.control.layout.pages[this.control.selection.pageIndex];
                if(layoutPage) {
                    var position = layoutPage.getStartOffsetContentOfMainSubDocument();
                    var section = this.control.model.getSectionByPosition(position);
                    if(applyTo === SectionPropertiesApplyType.CurrentSection || applyTo == SectionPropertiesApplyType.SelectedSections)
                        return new FixedInterval(section.startLogPosition.value, section.getLength());
                    else if(applyTo === SectionPropertiesApplyType.ThisPointForward)
                        return FixedInterval.fromPositions(section.startLogPosition.value, this.control.model.mainSubDocument.getDocumentEndPosition() - 1);
                }
            }
        }

        getInitInterval(): FixedInterval {
            if(this.control.model.activeSubDocument.isMain()) {
                let sectionIndices = this.control.model.getSectionIndicesByIntervals(this.control.selection.intervals);
                let firstSection = this.control.model.sections[sectionIndices[0]];
                let lastSection = this.control.model.sections[sectionIndices[sectionIndices.length - 1]];
                return FixedInterval.fromPositions(firstSection.startLogPosition.value, lastSection.startLogPosition.value + lastSection.getLength() - 1);
            }
            else {
                var layoutPage = this.control.layout.pages[this.control.selection.pageIndex];
                var section = this.control.model.getSectionByPosition(layoutPage.getStartOffsetContentOfMainSubDocument());
                return new FixedInterval(section.startLogPosition.value, section.getLength());
            }
        }

        getInitialTab(): PageSetupDialogTab {
            return PageSetupDialogTab.Margins;
        }

        getDialogName() {
            return "PageSetup";
        }

        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return false;
        }
    }

    export class ShowPagePaperSetupFormCommand extends DialogPageSetupCommand {
        getInitialTab(): PageSetupDialogTab {
            return PageSetupDialogTab.Paper;
        }
    }

    export class PageSetupDialogParameters extends DialogParametersBase {
        marginTop: number;
        marginBottom: number;
        marginLeft: number;
        marginRight: number;

        landscape: boolean;
        applyTo: SectionPropertiesApplyType;

        pageWidth: number;
        pageHeight: number;

        startType: SectionStartType;
        headerDifferentOddAndEven: boolean;
        headerDifferentFirstPage: boolean;

        initialTab: PageSetupDialogTab;

        baseInit() {
            this.applyTo = SectionPropertiesApplyType.WholeDocument;
            this.headerDifferentOddAndEven = false;
            this.headerDifferentFirstPage = false;
        }

        init(initSecProps: SectionProperties, tabs: PageSetupDialogTab) {
            this.baseInit();

            this.marginBottom = initSecProps.marginBottom;
            this.marginLeft = initSecProps.marginLeft;
            this.marginRight = initSecProps.marginRight;
            this.marginTop = initSecProps.marginTop;
            this.landscape = initSecProps.landscape;
            this.pageHeight = initSecProps.pageHeight;
            this.pageWidth = initSecProps.pageWidth;
            this.startType = initSecProps.startType;

            this.initialTab = tabs;
        }

        getNewInstance(): DialogParametersBase {
            return new PageSetupDialogParameters();
        }

        copyFrom(obj: PageSetupDialogParameters) {
            this.marginBottom = obj.marginBottom;
            this.marginLeft = obj.marginLeft;
            this.marginRight = obj.marginRight;
            this.marginTop = obj.marginTop;

            this.landscape = obj.landscape;
            this.applyTo = obj.applyTo;

            this.pageHeight = obj.pageHeight;
            this.pageWidth = obj.pageWidth;

            this.startType = obj.startType;
            this.headerDifferentFirstPage = obj.headerDifferentFirstPage;
            this.headerDifferentOddAndEven = obj.headerDifferentOddAndEven;

            this.initialTab = obj.initialTab;
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
            if (this.marginBottom) this.marginBottom = converterFunc(this.marginBottom);
            if (this.marginLeft) this.marginLeft = converterFunc(this.marginLeft);
            if (this.marginRight) this.marginRight = converterFunc(this.marginRight);
            if (this.marginTop) this.marginTop = converterFunc(this.marginTop);

            if (this.pageWidth) this.pageWidth = converterFunc(this.pageWidth);
            if (this.pageHeight) this.pageHeight = converterFunc(this.pageHeight);
        }
    }

    export enum SectionPropertiesApplyType {
        WholeDocument = 0,
        CurrentSection = 1,
        SelectedSections = 2,
        ThisPointForward = 4
    }

    export enum PageSetupDialogTab {
        Margins = 0,
        Paper = 1,
        Layout = 2
    }
}