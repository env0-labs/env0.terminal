// terminalBuffer.js

import { advanceCursor, newlineCursor, resetCursor, showCursor } from './terminalCursor.js';
let currentLine = 0;

const buffer = [];
const maxLines = 1000;

export function writeText(text) {
  if (buffer.length === 0) buffer.push('');
  buffer[currentLine] += text;
  advanceCursor(text.length);
  showCursor();
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
    cursorY -= overflow;
    if (cursorY < 0) cursorY = 0;
  }
}
