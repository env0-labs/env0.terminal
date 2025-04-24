import { createCanvas, redraw } from './canvasTerminal.js';
import { writeText, writeLine, clearBuffer } from './terminalBuffer.js';
import { startBlink } from './terminalCursor.js';

let containerEl = null;

export function initTerminal(container) {
  containerEl = container;
  console.log("[terminal] initTerminal called");
  createCanvas(containerEl);
}

export function print(text) {
  writeText(text);
  redraw();
}

export function println(text) {
  writeLine(text);
  redraw();
}

export function clearTerminal() {
  clearBuffer();
  redraw();
}

export { redraw };
