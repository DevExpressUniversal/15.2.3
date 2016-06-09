module __aspxRichEdit {
    // tests in \Tests\Scripts\Commands\Insert_CommandTests.ts
    export class InsertTextCommand extends CommandBase<SimpleCommandState> {
        removedInterval: boolean;

        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        executeCore(state: SimpleCommandState, parameter: string): boolean {
            if (parameter.length === 0)
                return false;
            this.removedInterval = !this.control.selection.isCollapsed();
            var canMergeWithPreviousCommand = this.control.commandManager.assertLastExecutedCommandsChain(true, InsertTextCommand);
            this.control.modelManipulator.insertText(this.control, this.control.selection.getLastSelectedInterval().clone(), parameter, canMergeWithPreviousCommand);
            return true;
        }

        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return !this.removedInterval;
        }
        lockUIUpdating(prevModifiedState: IsModified): boolean {
            return !this.removedInterval && prevModifiedState === this.control.getModifiedState();
        }
    }
}