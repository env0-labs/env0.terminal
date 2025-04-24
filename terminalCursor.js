// terminalCursor.js

import { config } from './terminalConfig.js';

let cursorX = 0;
let cursorY = 0;
let charWidth = 0;
let charHeight = 0;
let ctx = null;
let visible = true;

export function setCursorContext(context, cw, ch) {
  ctx = context;
  charWidth = cw;
  charHeight = ch;
}

export function moveCursorTo(x, y) {
  cursorX = x;
  cursorY = y;
}

export function advanceCursor(chars = 1) {
  cursorX += chars;
}

export function resetCursor() {
  cursorX = 0;
  cursorY += 1;
}

export function drawCursor() {
  if (!ctx || !visible) return;

  const x = cursorX * charWidth;
  const y = cursorY * charHeight + config.linePadding;

  ctx.fillStyle = config.fgColor;
  ctx.fillRect(x, y, charWidth, charHeight);
}

export function showCursor() {
  visible = true;
  drawCursor();
}
