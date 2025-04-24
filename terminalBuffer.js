// terminalBuffer.js

const buffer = [];
const maxLines = 1000;

let cursorX = 0;
let cursorY = 0;

export function writeText(text) {
  if (buffer.length === 0) buffer.push('');
  buffer[cursorY] += text;
  cursorX += text.length;
  clampScrollback();
}

export function writeLine(text) {
  if (buffer.length === 0 || (buffer.length === 1 && buffer[0] === '')) {
    buffer[0] = text;
  } else {
    buffer.push(text);
  }
  cursorY = buffer.length - 1;
  cursorX = 0;
  clampScrollback();
}


export function clearBuffer() {
  buffer.length = 0;
  cursorX = 0;
  cursorY = 0;
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
