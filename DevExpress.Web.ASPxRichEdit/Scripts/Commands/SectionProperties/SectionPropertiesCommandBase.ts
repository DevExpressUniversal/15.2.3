module __aspxRichEdit {
    export class SectionPropertiesCommandBase extends CommandBase<IntervalCommandStateEx> {
        getState(): IntervalCommandStateEx {
            return new IntervalCommandStateEx(this.isEnabled(), SectionPropertiesCommandBase.getIntervals(this.control), this.getValue());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.sections);
        }
        getValue(): any { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        static getIntervals(control: IRichEditControl): FixedInterval[] {
            if(control.model.activeSubDocument.isMain())
                return control.selection.getIntervalsClone();
            else if(control.model.activeSubDocument.isHeaderFooter()) {
                let layoutPage = control.forceFormatPage(control.selection.pageIndex);
                let sectionIndex = HeaderFooterCommandBase.getSectionIndex(layoutPage, control.model);
                let section = control.model.sections[sectionIndex]
                return [new FixedInterval(section.startLogPosition.value, section.getLength())];
            }
            else
                throw new Error("Unknown subDocument type");
        }
    }
}