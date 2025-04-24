// terminalRenderer.js

import { config } from './terminalConfig.js';
import { getVisibleBuffer } from './terminalBuffer.js';
import { drawCursor } from './terminalCursor.js';

let ctx, charWidth, charHeight;

export function setContext(newCtx, width, height) {
  ctx = newCtx;
  charWidth = width;
  charHeight = height;
}

export function drawFromBuffer() {
  const lines = getVisibleBuffer();
  ctx.fillStyle = config.bgColor;
  ctx.fillRect(0, 0, ctx.canvas.width, ctx.canvas.height);

  ctx.fillStyle = config.fgColor;
  for (let row = 0; row < lines.length; row++) {
    const line = lines[row];
    if (!line) continue;
    ctx.fillText(line, 0, row * charHeight + config.linePadding);
  }

  drawCursor(); // final layer
}
