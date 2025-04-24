import {
  advanceCursor,
  newlineCursor,
  resetCursor,
  showCursor
} from './terminalCursor.js';

import { getTerminalCols } from './canvasTerminal.js';


const buffer = [];
const maxLines = 1000;
let currentLine = 0;

export function writeText(text) {
  const cols = getTerminalCols(); // now dynamic per-call
  if (buffer.length === 0) buffer.push('');

  let remaining = text;

  while (remaining.length > 0) {
    const line = buffer[currentLine] || '';
    const spaceLeft = cols - line.length;

    const chunk = remaining.slice(0, spaceLeft);
    buffer[currentLine] = line + chunk;

    advanceCursor(chunk.length);
    showCursor();

    remaining = remaining.slice(chunk.length);

    if (remaining.length > 0) {
      buffer.push('');
      currentLine = buffer.length - 1;
      newlineCursor();
    }
  }

  clampScrollback();
}

export function writeLine(text) {
  if (buffer.length === 0 || (buffer.length === 1 && buffer[0] === '')) {
    buffer[0] = text;
    currentLine = 0;
  } else {
    buffer.push(text);
    currentLine = buffer.length - 1;
  }

  newlineCursor();
  showCursor();
  clampScrollback();
}

export function clearBuffer() {
  buffer.length = 0;
  currentLine = 0;
  resetCursor();
  showCursor();
}

export function getVisibleBuffer() {
  return buffer;
}

function clampScrollback() {
  if (buffer.length > maxLines) {
    const overflow = buffer.length - maxLines;
    buffer.splice(0, overflow);
    currentLine -= overflow;
    if (currentLine < 0) currentLine = 0;
  }
}
