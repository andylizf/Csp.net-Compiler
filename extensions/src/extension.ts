import * as path from 'path';
import * as serverUtils from './omnisharp/utils';
import * as vscode from 'vscode';
vscode.commands.registerCommand('extension.mock-debug.getProgramName', config => {
    return vscode.window.showInputBox({
        placeHolder: "Please enter the name of a markdown file in the workspace folder",
        value: "readme.md"
    });
});